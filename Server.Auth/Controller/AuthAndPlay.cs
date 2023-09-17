using System;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Server.Auth.BLL;
using Server.Auth.BLL.GameService;
using Server.Auth.BLL.JWT;
using Server.Auth.Controller.dto;
using Server.Auth.Repository.UserRepo;

namespace Server.Auth.Controller;

public sealed class AuthAndPlay
{
    private readonly IGameService _gameService;
    private readonly IUserRepository _userRepository;

    public AuthAndPlay(
        IGameService gameService,
        IUserRepository userRepository)
    {
        _gameService = gameService;
        _userRepository = userRepository;
    }

    [FunctionName(nameof(UserAuthentication))]
    public async Task<IActionResult> UserAuthentication(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")]
        UserCredentials userCredentials,
        ILogger log)
    {
        return await Login(
            userCredentials,
            log);
    }

    [FunctionName(nameof(PlayGame))]
    public async Task<IActionResult> PlayGame(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "play")]
        HttpRequest req,
        ILogger log,
        CancellationToken ct)
    {
        var userInfo = await AuthAndGetBody<UserInfoDto>(req);
        
        if (userInfo.Number is null)
        {
            throw new ApplicationException("sorry, you should predict and input the number");
        }

        var number = await _gameService.PlayGame(
            nickName: userInfo.UserName,
            userNum: userInfo.Number.Value,
            ct: ct);

        return FormatResult(number, userInfo);
    }
    
    [FunctionName(nameof(GetInfo))]
    public async Task<IActionResult> GetInfo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "info")]
        HttpRequest req,
        ILogger log,
        CancellationToken ct)
    {
        var userInfo = await AuthAndGetBody<UserInfoDto>(req);

        var userInfoFromDb = await _userRepository.GetUserInfoModel(userInfo.UserName, ct);

        return new OkObjectResult(userInfoFromDb);
    }

    private static IActionResult FormatResult(long? number, UserInfoDto userInfo)
    {
        if (number is null)
        {
            return new OkObjectResult("You won!");
        }

        if (number > userInfo.Number)
        {
            return new OkObjectResult("No, our number is bigger");
        }

        return new OkObjectResult("No, our number is smaller");
    }


    #region Private
    private static async Task<IActionResult> Login(UserCredentials userCredentials, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        // TODO: Perform custom authentication here; we're just using a simple hard coded check for this example
        var authenticated = userCredentials?.User.Equals("Jay", StringComparison.InvariantCultureIgnoreCase) ?? false;

        if (!authenticated)
        {
            return await Task.FromResult(new UnauthorizedResult()).ConfigureAwait(false);
        }
        
        var generateJwtToken = new GenerateJwt();
        var token = generateJwtToken.IssuingJwt(userCredentials.User);
        return await Task.FromResult(new OkObjectResult(token)).ConfigureAwait(false);
    }
    
    private static async Task<T> AuthAndGetBody<T>(HttpRequest req)
    {
        var postData = await GetJsonBodyAndAuth(req);
        
        var userInfo = ValidateBody<T>(postData);
        
        return userInfo;
    }

    private static async Task<string> GetJsonBodyAndAuth(HttpRequest req)
    {
        // Check if we have authentication info.
        var auth = new ValidateJwt(req);

        if (!auth.IsValid)
        {
            throw new UnauthorizedAccessException("sorry, you are not 'Jay'");
        }

        var postData = await req.ReadAsStringAsync();
        return postData;
    }
    private static T ValidateBody<T>(string postData)
    {
        try
        {
            var info = JsonConvert.DeserializeObject<T>(postData);
            return info;
        }
        catch (Exception e)
        {
            throw new SerializationException(
                "sorry, smth went wrong. Try to contact support",
                e);
        }
    }

    //для документации в Open.Api
    [SwaggerIgnore]
    [FunctionName("Swagger")]
    public static Task<HttpResponseMessage> Swagger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/json")]
        HttpRequestMessage req,
        [SwashBuckleClient] ISwashBuckleClient swasBuckleClient)
    {
        return Task.FromResult(swasBuckleClient.CreateSwaggerJsonDocumentResponse(req));
    }

    [SwaggerIgnore]
    [FunctionName("SwaggerUI")]
    public static Task<HttpResponseMessage> SwaggerUI(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/ui")]
        HttpRequestMessage req,
        [SwashBuckleClient] ISwashBuckleClient swasBuckleClient)
    {
        return Task.FromResult(swasBuckleClient.CreateSwaggerUIResponse(req, "swagger/json"));
    }

    #endregion
}