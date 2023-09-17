using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server.Auth.BLL.Models;
using Server.Auth.Repository.GameRepository;
using Server.Auth.Repository.UserRepo;

namespace Server.Auth.BLL.GameService;

public class GameService : IGameService
{
    private readonly IUserRepository _userRepository;
    private readonly IGameRepository _gameRepository;
    private readonly Random _rnd;

    public GameService(
        IUserRepository userRepository,
        IGameRepository gameRepository)
    {
        _rnd = new Random();
        _userRepository = userRepository;
        _gameRepository = gameRepository;
    }
    
    public async Task<long?> PlayGame(
        string nickName,
        long userNum,
        CancellationToken ct)
    {
        var userModel = await _userRepository.GetUserInfoModel(
            nickName, 
            ct);
        
        var activeGames = await _gameRepository.GetGames(
            userModel.UserId,
            GameModel.ActiveState,
            ct);

        if (activeGames.Count > 1)
            throw new ApplicationException("sorry, you should finish the first game");

        var activeGame = activeGames.Count == 0 
            ? await GetNewActiveGame(userModel, ct):
            activeGames.First() ;

        if (activeGame.Value.Number == userNum)
        {
            await _gameRepository.SaveGame(
                activeGame.Value with
                {
                    GameStateType = GameStateType.Finished,
                }, ct);
            
            return null;
        }

        return activeGame.Value.Number;
    }

    private async Task<KeyValuePair<long, GameModel>> GetNewActiveGame(UserInfoModel userModel, CancellationToken ct)
    {
        var game = GenerateNewGame(userModel);
        await _gameRepository.SaveGame(game,ct);
        return (await _gameRepository.GetGames(userModel.UserId, GameModel.ActiveState, ct)).First();
    }

    #region Private

    private GameModel GenerateNewGame(UserInfoModel userInfoModel)
    {
        return new GameModel(
            userInfoModel.UserId,
            _rnd.Next(),
            GameStateType: GameStateType.InProgress);
    }

    #endregion
    
}