namespace Server.Auth.BLL.Models;

public record GameModel(
    long UserId,
    long Number,
    bool IsFinished);