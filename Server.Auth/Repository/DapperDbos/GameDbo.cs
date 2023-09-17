namespace Server.Auth.Repository.DapperDbos;

public sealed class GameDbo
{
    public long GameId { get; set; }
    public long UserId { get; set; }
    public long Number { get; set; }
    public int GameStateType { get; set; }
}