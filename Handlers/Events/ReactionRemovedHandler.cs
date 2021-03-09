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
    public class ReactionRemovedHandler: EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<ReactionRemovedHandler>();

        public ReactionRemovedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.ReactionRemoved += ShardOnReactionRemoved;
            logger.Information("Registered");
        }

        private async Task ShardOnReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel textChannel, SocketReaction reaction)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(((SocketTextChannel) textChannel).Guild.Id);

            if (GetRestTextChannel(this.shard, guild.ReactionAddedEvent.Key, out RestTextChannel restTextChannel))
            {
                EmbedBuilder embedBuilder = new()
                {
                    Title = $"{reaction.User.Value.Mention} removed a reaction",
                    Description = $"{reaction.Emote.Name} was removed from Message ID: {message.Value.Id} in {message.Value.Channel.Name}",
                    Color = Color.Blue,
                    Footer = new EmbedFooterBuilder{Text = $"User ID: {reaction.UserId}, Message ID {message.Value.Id}, removed at {DateTime.UtcNow} UTC"}
                };
                
                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}