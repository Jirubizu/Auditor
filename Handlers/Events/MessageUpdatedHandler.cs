using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auditor.Services;
using Auditor.Structures;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class MessageUpdatedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly PaginationService paginationService;
        private readonly ILogger logger = Log.ForContext<MessageUpdatedHandler>();

        public MessageUpdatedHandler(DiscordShardedClient s, DatabaseService d, PaginationService p)
        {
            this.database = d;
            this.shard = s;
            this.paginationService = p;
            this.shard.MessageUpdated += ShardOnMessageUpdated;
            this.logger.Information("Registered");
        }

        private async Task ShardOnMessageUpdated(Cacheable<IMessage, ulong> cachedMessage, SocketMessage newMessage,
            ISocketMessageChannel textChannel)
        {
            if (cachedMessage.Value.Author.IsBot)
            {
                return;
            }

            GuildBson guild = await this.database.LoadRecordsByGuildId(((SocketTextChannel) textChannel).Guild.Id);

            if (GetRestTextChannel(this.shard, guild.MessageUpdatedEvent.Key, out RestTextChannel restTextChannel))
            {
                List<EmbedBuilder> pages = new();
                pages.Add(new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = newMessage.Author.Mention,
                        IconUrl = newMessage.Author.GetAvatarUrl(),
                        Url = newMessage.GetJumpUrl()
                    },
                    Title = "Message Updated: Before",
                    Description = cachedMessage.Value.Content,
                    Footer = new EmbedFooterBuilder
                    {
                        Text =
                            $"Author ID: {newMessage.Author.Id}, Message ID: {newMessage.Id}, created at {cachedMessage.Value.Timestamp.UtcDateTime} UTC"
                    }
                });

                pages.Add(new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = newMessage.Author.Mention,
                        IconUrl = newMessage.Author.GetAvatarUrl(),
                        Url = newMessage.GetJumpUrl()
                    },
                    Title = "Message Updated: After",
                    Description = newMessage.Content,
                    Footer = new EmbedFooterBuilder
                    {
                        Text =
                            $"Author ID: {newMessage.Author.Id}, Message ID: {newMessage.Id}, updated at {DateTime.UtcNow} UTC"
                    }
                });

                PaginatedMessage paginatedMessage = new(pages, "Message Updated", Color.Blue, null, new AppearanceOptions{Style = DisplayStyle.Minimal});
                await this.paginationService.SendMessageAsync(restTextChannel, paginatedMessage);
            }
        }
    }
}