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
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Npgsql;
using Server.Auth;
using Server.Auth.BLL.GameService;
using Server.Auth.Repository;
using Server.Auth.Repository.GameRepository;
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
                    Version = "v2",
                }
            };
            opts.ConfigureSwaggerGen = x =>
            {
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                x.CustomOperationIds(apiDesc => apiDesc
                    .TryGetMethodInfo(out MethodInfo mInfo) ? mInfo.Name : default (Guid).ToString());
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            };
        });

        builder.Services.AddOptions<Config>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(nameof(Config)).Bind(settings);
            });

        builder.Services.AddTransient<IDbConnection>((sp) =>
        {
            var connectionString = sp.GetService<IOptions<Config>>();
            var connection = new NpgsqlConnection(connectionString.Value.ConnectionString);
            return connection;
        });
         //add services
         builder.Services.AddScoped<IGameService, GameService>();
         
         //add repos
         builder.Services.AddScoped<IUserRepository, UserRepository>();
         builder.Services.AddScoped<IGameRepository, GameRepository>();
    }
}