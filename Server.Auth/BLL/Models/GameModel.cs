using System.Collections.Generic;

namespace Server.Auth.BLL.Models;

public record GameModel(
    long UserId,
    long Number,
    GameStateType GameStateType)
{
    public static IReadOnlySet<GameStateType> ActiveState => new HashSet<GameStateType>
    {
        GameStateType.InProgress
    };
};

public enum GameStateType
{
    InProgress = 2,
    Finished = 3
}