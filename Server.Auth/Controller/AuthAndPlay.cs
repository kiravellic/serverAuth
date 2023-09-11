using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Server.Auth.BLL;
using Server.Auth.BLL.JWT;

namespace Server.Auth.Controller;

public static class Auth
{
    [FunctionName(nameof(UserAuthentication))]
    public static async Task<IActionResult> UserAuthentication(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")] 
        UserCredentials userCredentials,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        // TODO: Perform custom authentication here; we're just using a simple hard coded check for this example
        var authenticated = userCredentials?.User.Equals("Jay", StringComparison.InvariantCultureIgnoreCase) ?? false;
        if (!authenticated)
        {
            return await Task.FromResult(new UnauthorizedResult()).ConfigureAwait(false);
        }
        else
        {
            var generateJwtToken = new GenerateJwt();
            var token = generateJwtToken.IssuingJwt(userCredentials.User);
            return await Task.FromResult(new OkObjectResult(token)).ConfigureAwait(false);
        }

    }

    [FunctionName(nameof(GetData))]
    public static async Task<IActionResult> GetData(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "data")] HttpRequest req,
        ILogger log)
    {
        // Check if we have authentication info.
        ValidateJwt auth = new ValidateJwt(req);

        if (!auth.IsValid)
        {
            return new UnauthorizedResult(); // No authentication info.
        }

        string postData = await req.ReadAsStringAsync();

        return new OkObjectResult($"{postData}");

    }
}

public class UserCredentials
{
    public string User { get; set; }
    public string Password { get; set; }
}