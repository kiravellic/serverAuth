using System.Data;
using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Server.Auth;
using Server.Auth.BLL.GameService;
using Server.Auth.Repository;
using Server.Auth.Repository.UserRepo;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Server.Auth;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
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