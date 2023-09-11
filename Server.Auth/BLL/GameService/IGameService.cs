using System.Threading;
using System.Threading.Tasks;
using Server.Auth.BLL.Models;

namespace Server.Auth.BLL.GameService;

public interface IGameService
{
    /// <summary>
    /// start Game
    /// </summary>
    /// <param name="nickName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task StartGame(string nickName, CancellationToken ct);
}