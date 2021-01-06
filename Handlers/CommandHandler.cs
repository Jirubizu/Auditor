using System;
using System.Reflection;
using System.Threading.Tasks;
using Auditor.Services;
using Auditor.Structures;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordShardedClient client;
        private readonly CommandService commandService;
        private readonly IServiceProvider services;
        private readonly DatabaseService database;
        private readonly ILogger logger = Log.ForContext<CommandHandler>();
        
        public CommandHandler(DiscordShardedClient c, CommandService cs, IServiceProvider s, DatabaseService d)
        {
            client = c;
            commandService = cs;
            services = s;
            database = d;
        }

        public async Task SetupAsync()
        {
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            client.ShardReady += ClientOnShardReady;
            commandService.Log += LogAsync;
            client.MessageReceived += HandleCommandAsync;
        }

        private Task ClientOnShardReady(DiscordSocketClient arg)
        {
            logger.Information($"Invite bot: https://discord.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot");
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            int argPos = 0;
            if (!(msg is SocketUserMessage message))
            {
                return;
            }

            SocketGuildChannel guildChannel = (SocketGuildChannel) message.Channel;

            GuildBson r = await database.LoadRecordsByGuildId(guildChannel.Guild.Id);

            if (message.HasStringPrefix(r.Prefix, ref argPos))
            {
                ShardedCommandContext context = new (client, message);

                IResult result = await commandService.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }

        private Task LogAsync(LogMessage message)
        {
            logger.Debug(message.Message);
            return Task.CompletedTask;
        }
    }
}