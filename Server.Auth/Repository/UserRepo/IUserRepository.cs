using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Server.Auth.BLL.Models;

namespace Server.Auth.Repository.UserRepo;

public interface IUserRepository
{
    //it should be a userId, but we do not have users claims roles and oidc server for login
    public Task<UserInfoModel> GetUserInfoModel(string nickName, CancellationToken ct);

    /// <summary>
    /// update user model
    /// </summary>
    /// <param name="newUserModel">user model for update</param>
    /// <param name="ct"></param>
    /// <returns>set of successfully changed user ids</returns>
    public Task<IReadOnlySet<long>> UpdateUserModel(UserInfoModel newUserModel, CancellationToken ct);
}