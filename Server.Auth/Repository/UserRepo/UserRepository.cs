using System.Collections.Generic;
using System.Data.Common;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Server.Auth.BLL.Models;
using Server.Auth.Repository.DapperDbos;

namespace Server.Auth.Repository.UserRepo;

public sealed class UserRepository : IUserRepository
{
    private readonly DbConnection _dbConnection;

    public UserRepository(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<UserInfoModel> GetUserInfoModel(string nickName,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(nickName))
        {
            throw new AuthenticationException("nicknames is empty");
        }
        
        const string query = @$"select 
                id as UserId,
                user_name as UserName,
                games_count as GamesCount
            from database.public.user_info where user_name = @{nameof(nickName)}";
        
        var userModel = await _dbConnection.QueryFirstAsync<UserInfoDbo>(
            new CommandDefinition(
                query, 
                new
                {
                    nickName
                }, 
                cancellationToken:ct));
        
        if (userModel is null)
        {
            throw new AuthenticationException("user not found");
        }

        return userModel.ToModel();
    }

    public Task<IReadOnlySet<long>> UpdateUserModel(
        UserInfoModel newUserModel,
        CancellationToken ct)
    {
        //TODO: update with transaction
        throw new System.NotImplementedException();
    }
}