using System.Threading;
using System.Threading.Tasks;
using Server.Auth.BLL.Models;

namespace Server.Auth.Repository.GameRepository;

public class GameRepository : IGameRepository
{
    public Task SaveGame(GameModel game, CancellationToken ct)
    {
        throw new System.NotImplementedException();
    }
}