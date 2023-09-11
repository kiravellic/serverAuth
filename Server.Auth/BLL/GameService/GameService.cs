using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Auth.BLL.Models;
using Server.Auth.Repository.GameRepository;
using Server.Auth.Repository.UserRepo;

namespace Server.Auth.BLL.GameService;

public class GameService : IGameService
{
    private readonly IUserRepository _userRepository;
    private readonly IGameRepository _gameRepository;
    private readonly Random _rnd;

    public GameService(
        IUserRepository userRepository,
        IGameRepository gameRepository)
    {
        _rnd = new Random();
        _userRepository = userRepository;
        _gameRepository = gameRepository;
    }
    
    public async Task StartGame(
        string nickName,
        CancellationToken ct)
    {
        var userModel = await _userRepository.GetUserInfoModel(nickName, ct);
        
        var game = GenerateNewGame(userModel);

        await _gameRepository.SaveGame(game,ct);

    }

    private GameModel GenerateNewGame(UserInfoModel userInfoModel)
    {
        return new GameModel(
            userInfoModel.UserId,
            _rnd.Next(),
            IsFinished: false);
    }
}