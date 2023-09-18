namespace Server.Auth.Controller.dto;

public sealed class UserInfoDto
{
    private string _userName = null!;

    public string UserName
    {
        get => _userName.ToLower();
        set => _userName = value;
    }

    public long? Number { get; set; }
}