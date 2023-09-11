namespace Server.Auth.Repository.DapperDbos;

public sealed class UserInfoDbo
{
    public long UserId { get; set; }
    public string UserName { get; set; } = null!;
    public uint GamesCount { get; set; }
}