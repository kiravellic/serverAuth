using System.Threading;
using System.Threading.Tasks;
using Server.Auth.BLL.Models;

namespace Server.Auth.BLL.GameService;

public interface IGameService
{
    /// <summary>
    /// play game
    /// </summary>
    /// <param name="nickName">user name</param>
    /// <param name="userNum"> number that was predicted by user</param>
    /// <param name="ct"></param>
    /// <returns>the actual number or null if won</returns>
    public Task<long?> PlayGame(
        string nickName,
        long userNum,
        CancellationToken ct);
}