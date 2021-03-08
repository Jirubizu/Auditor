﻿using System;
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
    public class ChannelCreatedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<ChannelCreatedHandler>();

        public ChannelCreatedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.shard = s;
            this.database = d;
            this.shard.ChannelCreated += ShardOnChannelCreated;
            logger.Information("Registered");
        }

        private async Task ShardOnChannelCreated(SocketChannel arg)
        {
            GuildBson guild;
            RestTextChannel restTextChannel;
            List<EmbedFieldBuilder> fields;

            switch (arg)
            {
                case SocketTextChannel socketTextChannel:
                    guild = await this.database.LoadRecordsByGuildId(socketTextChannel.Guild.Id);

                    if (!GetRestTextChannel(this.shard, guild.ChannelCreatedEvent.Key, out restTextChannel)) return;

                    fields = new List<EmbedFieldBuilder>
                    {
                        new()
                        {
                            Name = "Category",
                            Value = socketTextChannel.Category?.Name ?? "null"
                        },
                        new()
                        {
                            Name = "Channel Name",
                            Value = socketTextChannel.Name
                        },
                        new()
                        {
                            Name = "Type",
                            Value = "Text Channel"
                        }
                    };


                    break;
                case SocketVoiceChannel socketVoiceChannel:
                    guild = await this.database.LoadRecordsByGuildId(socketVoiceChannel.Guild.Id);

                    if (!GetRestTextChannel(this.shard, guild.ChannelCreatedEvent.Key, out restTextChannel)) return;

                    fields = new List<EmbedFieldBuilder>
                    {
                        new()
                        {
                            Name = "Category",
                            Value = socketVoiceChannel.Category?.Name ?? "null",
                            IsInline = true
                        },
                        new()
                        {
                            Name = "Channel Name",
                            Value = socketVoiceChannel.Name ?? "null",
                            IsInline = true
                        },
                        new()
                        {
                            Name = "Type",
                            Value = "Voice Channel",
                            IsInline = true
                        }
                    };


                    break;
                default:
                    return;
            }

            EmbedBuilder embedBuilder = new()
            {
                Color = Color.Blue,
                Fields = fields,
                Footer = new EmbedFooterBuilder {Text = $"Created at {DateTime.UtcNow} UTC"}
            };
            await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}