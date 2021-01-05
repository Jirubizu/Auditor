using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Auditor.Handlers;
using Auditor.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using EventHandler = Auditor.Handlers.EventHandler;

namespace Auditor
{
    public class Auditor
    {
        private readonly DiscordShardedClient client;
        private readonly CommandService command;
        private ConfigService config;
        private HelpService helpService;

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
            config = new ConfigService(configLoc);
            helpService = new HelpService(command);
            await client.LoginAsync(TokenType.Bot, config.Config.Token);
            await client.StartAsync();

            client.Log += LogAsync;

            IServiceProvider services = SetupServices();

            CommandHandler commandHandler = services.GetRequiredService<CommandHandler>();
            await commandHandler.SetupAsync();

            helpService.Setup();

            await Task.Delay(-1);
        }

        private IServiceProvider SetupServices()
        {
            ServiceCollection collection = new();
            collection.AddSingleton(this);
            collection.AddSingleton(client);
            collection.AddSingleton(command);
            collection.AddSingleton(config);
            collection.AddSingleton(helpService);
            collection.AddSingleton<CommandHandler>();
            collection.AddSingleton<DatabaseService>();
            collection.AddSingleton<HttpService>();
            collection.AddSingleton<PaginationService>();

            foreach (Type handler in typeof(EventHandler).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(EventHandler))))
            {
                collection.AddSingleton(handler);
            }

            return collection.BuildServiceProvider();
        }

        private Task LogAsync(LogMessage message)
        {
            Console.WriteLine(message.Message);
            return Task.CompletedTask;
        }
    }
}