using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Auditor.Handlers;
using Auditor.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using EventHandler = Auditor.Handlers.EventHandler;

namespace Auditor
{
    public class Auditor
    {
        private readonly DiscordShardedClient client;
        private readonly CommandService command;
        private ConfigService configService;
        private HelpService helpService;
        private readonly ILogger logger = Log.ForContext<Auditor>();

        public Auditor(DiscordShardedClient shard = null, CommandService cmd = null)
        {
            client = shard ?? new DiscordShardedClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Verbose
            });

            command = cmd ?? new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async
            });
        }

        public async Task SetupAsync(string configLoc)
        {
            configService = new ConfigService(configLoc);
            // Find a neater way of checking if the database is down.
            using (TcpClient tcpClient = new())
            {
                try
                {
                    await tcpClient.ConnectAsync(configService.Config.DatabaseIp, configService.Config.DatabasePort);
                    logger.Information("Database is active");
                }
                catch (Exception)
                {
                    logger.Fatal("Could not connect to the database. Make sure your database running");
                    throw;
                }
            }
            
            helpService = new HelpService(command);
            await client.LoginAsync(TokenType.Bot, configService.Config.Token);
            await client.StartAsync();

            client.Log += LogAsync;

            IServiceProvider services = SetupServices();

            CommandHandler commandHandler = services.GetRequiredService<CommandHandler>();
            await commandHandler.SetupAsync();

            foreach (Type handler in typeof(EventHandler).Assembly.GetTypes()
                .Where(h => h.BaseType == typeof(EventHandler)))
            {
                services.GetRequiredService(handler);
            }

            helpService.Setup();

            await Task.Delay(-1);
        }

        private IServiceProvider SetupServices()
        {
            ServiceCollection collection = new();
            collection.AddSingleton(this);
            collection.AddSingleton(client);
            collection.AddSingleton(command);
            collection.AddSingleton(configService);
            collection.AddSingleton(helpService);
            collection.AddSingleton<CommandHandler>();
            collection.AddSingleton<DatabaseService>();
            collection.AddSingleton<PaginationService>();
            
            foreach (Type handler in typeof(EventHandler).Assembly.GetTypes()
                .Where(h => h.BaseType == typeof(EventHandler)))
            {
                collection.AddSingleton(handler);
            }

            return collection.BuildServiceProvider();
        }

        private Task LogAsync(LogMessage message)
        {
            logger.Debug(message.Message);
            return Task.CompletedTask;
        }
    }
}