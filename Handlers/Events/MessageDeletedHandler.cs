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
    public class MessageDeletedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<MessageDeletedHandler>();

        public MessageDeletedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.MessageDeleted += ShardOnMessageDeleted;
            this.logger.Information("Registered");
        }

        private async Task ShardOnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage,
            ISocketMessageChannel textChannel)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(((SocketTextChannel) textChannel).Guild.Id);

            if (!GetRestTextChannel(this.shard, guild.MessageDeletedEvent.Key, out RestTextChannel restTextChannel))
            {
                return;
            }

            IMessage message = cachedMessage.Value;
            EmbedBuilder embedBuilder = new()
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = message.Author.Mention,
                    IconUrl = message.Author.GetAvatarUrl()
                },
                Title =
                    $"Message by {message.Author.Mention} was deleted in {((SocketTextChannel) textChannel).Mention}",
                Description = message.Content,
                Footer = new EmbedFooterBuilder
                {
                    Text = $"Author ID: {message.Author.Id}, Message ID: {message.Id}, at {DateTime.UtcNow} UTC"
                }
            };

            await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}