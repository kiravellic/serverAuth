using System.Threading;
using System.Threading.Tasks;
using Server.Auth.BLL.Models;

namespace Server.Auth.Repository.GameRepository;

public interface IGameRepository
{
    /// <summary>
    /// Saving to db a game
    /// </summary>
    /// <param name="game"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task SaveGame(GameModel game, CancellationToken ct);
}