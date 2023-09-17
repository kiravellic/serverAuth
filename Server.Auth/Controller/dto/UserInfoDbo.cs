namespace Server.Auth.Controller.dto;

public sealed class UserInfoDto
{
    public string UserName { get; set; } = null!;
    public long? Number { get; set; }
}