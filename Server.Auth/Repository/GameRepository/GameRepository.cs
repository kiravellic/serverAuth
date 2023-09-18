using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Server.Auth.BLL.Models;
using Server.Auth.Repository.DapperDbos;

namespace Server.Auth.Repository.GameRepository;

public class GameRepository : IGameRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IOptions<Config> _config;

    public GameRepository(IDbConnection dbConnection, IOptions<Config> config)
    {
        _dbConnection = dbConnection;
        _config = config;
    }
    
    public async Task SaveGame(GameModel game, CancellationToken ct)
    {
        await using (var connection = new NpgsqlConnection(_config.Value.ConnectionString))
        {
            await connection.OpenAsync(ct);
            await using (var transaction = await connection.BeginTransactionAsync(ct))
            {
                var oldActiveGames = await GetGames(
                    game.UserId,
                    new HashSet<GameStateType>
                    {
                        GameStateType.InProgress,
                    },
                    ct,
                    connection,
                    transaction);
                
                switch (game.GameStateType)
                {
                    case GameStateType.InProgress:
                        await CreateOrUpdateGame(
                            game,
                            ct, 
                            oldActiveGames,
                            connection,
                            transaction);
                        break;
                    case GameStateType.Finished:
                        if (oldActiveGames.Count == 0)
                        {
                            throw new ApplicationException("no game to complete");
                        }
                        await UpdateGame(
                            oldActiveGames.Keys.First(),
                            game,
                            connection,
                            transaction,
                            ct);
                        break;
                }

                await transaction.CommitAsync(ct);
            }
        }
        
    }
    
    /// <summary>
    /// Достаем игры по статусам
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="gameStates"></param>
    /// <param name="ct"></param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<IReadOnlyDictionary<long,GameModel>> GetGames(
        long userId,
        IReadOnlySet<GameStateType> gameStates,
        CancellationToken ct,
        NpgsqlConnection? connection = null,
        NpgsqlTransaction? transaction = null)
    {
        if (gameStates.Count == 0)
        {
            throw new ArgumentException("provide states of games");
        }
        
        var gameStatuses = gameStates.Select(x=>(int)x).ToArray();

        const string query = @$"
            select
            id as GameId,
            game_state as GameStateType,
            user_id as UserId,
            number as Number
        from database.public.games_info
        where user_id = @{nameof(userId)} and game_state in (select unnest(@{nameof(gameStatuses)}));";
    
        var games = connection == null
            ? await _dbConnection.QueryAsync<GameDbo>(
                new CommandDefinition(
                    query,
                    new
                    {
                        gameStatuses,
                        userId
                    },
                    cancellationToken: ct))
            : await connection.QueryAsync<GameDbo>(
                new CommandDefinition(
                    query,
                    new
                    {
                        gameStatuses,
                        userId
                    },
                    transaction: transaction,
                    cancellationToken: ct));
        
        return games.ToDictionary(
            game => game.GameId,
            game => new GameModel(
                game.UserId,
                game.Number,
                (GameStateType)game.GameStateType));
    
    }


#region Private
    private async Task CreateOrUpdateGame(
        GameModel game,
        CancellationToken ct,
        IReadOnlyDictionary<long, GameModel> oldActiveGames,
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        switch (oldActiveGames.Count)
        {
            case > 1:
                throw new ApplicationException(
                    "sorry, you can't start a game before ended previous one");
            case 1:
                await UpdateGame(
                    oldActiveGames.Keys.First(),
                    game,
                    connection,
                    transaction,
                    ct);
                break;
            case 0:
                await CreateGame(
                    game,
                    connection,
                    transaction,
                    ct);
                break;
        }
    }

    private async Task CreateGame(
        GameModel game,
        NpgsqlConnection connection, 
        NpgsqlTransaction transaction,
        CancellationToken ct)
    {
        var userId = game.UserId;
        var number = game.Number;
        var status = (int)game.GameStateType;
        const string query = $@"
            insert into database.public.games_info
                values (DEFAULT, @{nameof(status)}, @{nameof(userId)}, @{nameof(number)});";
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                query,
                new
                {
                    userId,
                    number,
                    status
                },
                transaction,
                cancellationToken: ct));
    }

    private async Task UpdateGame(
        long gameId,
        GameModel game,
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        CancellationToken ct)
    {
        var number = game.Number;
        var status = (int)game.GameStateType;
        
        const string query = $@"
            update public.games_info set
                    game_state = @{nameof(status)},
                    number = @{nameof(number)}
                        where id = @{nameof(gameId)};";
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                query,
                new
                {
                    number,
                    status,
                    gameId
                },
                transaction,
                cancellationToken: ct));
    }

    #endregion
}