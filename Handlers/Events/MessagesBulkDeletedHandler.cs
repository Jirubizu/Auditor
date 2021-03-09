﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auditor.Services;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class MessagesBulkDeletedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly PaginationService paginationService;
        private readonly ILogger logger = Log.ForContext<MessagesBulkDeletedHandler>();


        public MessagesBulkDeletedHandler(DiscordShardedClient s, DatabaseService d, PaginationService p)
        {
            this.database = d;
            this.shard = s;
            this.paginationService = p;
            this.shard.MessagesBulkDeleted += ShardOnMessagesBulkDeleted;
            logger.Information("Registered");
        }

        private async Task ShardOnMessagesBulkDeleted(IReadOnlyCollection<Cacheable<IMessage, ulong>> cachedMessages,
            ISocketMessageChannel restTextChannel)
        {
            List<EmbedBuilder> pages = new();
            foreach (Cacheable<IMessage, ulong> message in cachedMessages)
            {
                pages.Add(new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = message.Value.Author.Mention, IconUrl = message.Value.Author.GetAvatarUrl()
                    },
                    Description = message.Value.Content,
                    Footer = new EmbedFooterBuilder
                    {
                        Text = $"Author ID: {message.Value.Author.Id}, at {DateTime.UtcNow} UTC"
                    }
                });
            }

            PaginatedMessage paginatedMessage = new(pages, "Bulk Delete", Color.Blue);
            await this.paginationService.SendMessageAsync(restTextChannel, paginatedMessage);
        }
    }
}