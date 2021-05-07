using System;
using System.Collections.Generic;
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
        private readonly IEnumerable<Type> handlers;

        public Auditor(DiscordShardedClient shard = null, CommandService cmd = null)
        {
            this.client = shard ?? new DiscordShardedClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Verbose
            });

            this.command = cmd ?? new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async
            });

            this.handlers = typeof(EventHandler).Assembly.GetTypes().Where(h => h.BaseType == typeof(EventHandler));
        }

        public async Task SetupAsync(string configLoc)
        {
            this.configService = new ConfigService(configLoc);
            // Find a neater way of checking if the database is down.
            using (TcpClient tcpClient = new())
            {
                try
                {
                    await tcpClient.ConnectAsync("localhost", 27017);
                    this.logger.Information("Database is active");
                }
                catch (Exception)
                {
                    this.logger.Fatal("Could not connect to the database. Make sure your database is running");
                    throw;
                }
            }

            this.helpService = new HelpService(this.command);
            await this.client.LoginAsync(TokenType.Bot, configService.Config.Token);
            await this.client.StartAsync();

            this.client.Log += LogAsync;

            IServiceProvider services = SetupServices();

            CommandHandler commandHandler = services.GetRequiredService<CommandHandler>();
            await commandHandler.SetupAsync();

            foreach (Type handler in this.handlers)
            {
                services.GetRequiredService(handler);
            }

            this.helpService.Setup();

            await Task.Delay(-1);
        }

        private IServiceProvider SetupServices()
        {
            ServiceCollection collection = new();
            collection.AddSingleton(this);
            collection.AddSingleton(this.client);
            collection.AddSingleton(this.command);
            collection.AddSingleton(this.configService);
            collection.AddSingleton(this.helpService);
            collection.AddSingleton<CommandHandler>();
            collection.AddSingleton<DatabaseService>();
            collection.AddSingleton<PaginationService>();

            foreach (Type handler in this.handlers)
            {
                collection.AddSingleton(handler);
            }

            return collection.BuildServiceProvider();
        }

        private Task LogAsync(LogMessage message)
        {
            this.logger.Debug(message.Message);
            return Task.CompletedTask;
        }
    }
}