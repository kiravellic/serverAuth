using Server.Auth.Repository.DapperDbos;

namespace Server.Auth.BLL.Models;

public sealed record UserInfoModel(
    long UserId,
    string UserName,
    uint GamesCount);

public static class UserInfoModelExtension
{
    public static UserInfoModel? ToModel(this UserInfoDbo? userInfoDbo)
    {
        if (userInfoDbo is null)
        {
            return null;
        }

        return new UserInfoModel(
            userInfoDbo.UserId,
            userInfoDbo.UserName,
            userInfoDbo.GamesCount);
    }
}