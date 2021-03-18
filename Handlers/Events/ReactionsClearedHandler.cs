using System;
using System.Threading.Tasks;
using Auditor.Services;
using Auditor.Structures;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class ReactionsClearedHandler: EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<RoleDeletedHandler>();

        public ReactionsClearedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.ReactionsCleared += ShardOnReactionsCleared;
            this.logger.Information("Registered");
        }

        private async Task ShardOnReactionsCleared(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel textChannel)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(((SocketTextChannel) textChannel).Guild.Id);

            if (GetRestTextChannel(this.shard, guild.ReactionsClearedEvent.Key, out RestTextChannel restTextChannel))
            {
                EmbedBuilder embedBuilder = new()
                {
                    Title = "Reactions Cleared",
                    Description = $"All reactions from Message ID: {message.Value.Id} in {message.Value.Channel.Name} where removed.",
                    Color = Color.Blue,
                    Footer = new EmbedFooterBuilder{Text = $"Message ID {message.Value.Id}, removed at {DateTime.UtcNow} UTC"}
                };
                
                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}