using System;
using System.Data;
using System.IO;
using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Server.Auth;
using Server.Auth.BLL.GameService;
using Server.Auth.Repository;
using Server.Auth.Repository.UserRepo;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Server.Auth;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts => {
            opts.AddCodeParameter = true;
            opts.Documents = new [] {
                new SwaggerDocument {
                    Name = "v1",
                    Title = "Swagger document",
                    Description = "Integrate Swagger UI With Azure Functions",
                    Version = "v2"
                }
            };
            opts.ConfigureSwaggerGen = x => {
                x.CustomOperationIds(apiDesc => apiDesc
                    .TryGetMethodInfo(out MethodInfo mInfo) ? mInfo.Name : default (Guid).ToString());
            };
        });
        builder.Services.AddOptions<Config>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(nameof(Config)).Bind(settings);
            });
        
        //add db
         builder.Services.AddTransient<IDbConnection>((sp) => new NpgsqlConnection(
            sp.GetService<IConfiguration>()
                .Get<Config>()
                .ConnectionString));
         
         //add services
         builder.Services.AddScoped<IGameService, GameService>();
         
         //add repos
         builder.Services.AddScoped<IUserRepository, UserRepository>();
         builder.Services.AddScoped<IGameService, GameService>();
    }
}