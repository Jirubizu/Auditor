﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Auditor.Services;
using Auditor.Structures;
using Auditor.Utilities;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class ChannelUpdatedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.Logger.ForContext<ChannelUpdatedHandler>();

        public ChannelUpdatedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.ChannelUpdated += ShardOnChannelUpdated;
            this.logger.Information("Registered");
        }

        private async Task ShardOnChannelUpdated(SocketChannel oldChannel, SocketChannel newChannel)
        {
            GuildBson guild;
            RestTextChannel restTextChannel;
            string[] channelConstantChanges = {"Users", "CachedMessages"};
            IEnumerable<PropertyInfo> differentPropertyInfos;
            
            List<EmbedFieldBuilder> fields = new()
            {
                new EmbedFieldBuilder {Name = "Channel", Value = $"<#{oldChannel.Id}>"}
            };

            // TODO: Fix permissions overwrites, should output what permissions are changed.
            switch (oldChannel)
            {
                case SocketTextChannel socketTextChannel:
                    guild = await this.database.LoadRecordsByGuildId(socketTextChannel.Guild.Id);

                    if (!GetRestTextChannel(this.shard, guild.ChannelUpdatedEvent, out restTextChannel)) return;

                    SocketTextChannel newSocketTextChannel = newChannel as SocketTextChannel;

                    differentPropertyInfos = EnumeratingUtilities.GetDifferentProperties(
                        socketTextChannel,
                        newSocketTextChannel, channelConstantChanges);

                    foreach (PropertyInfo info in differentPropertyInfos)
                    {
                        if (info.GetType() == typeof(IEnumerable<Overwrite>))
                        {
                            foreach (Overwrite overwrite in (IEnumerable<Overwrite>) info) 
                            {
                                fields.Add(new EmbedFieldBuilder
                                {
                                    Name = overwrite.TargetType.ToString(),
                                    Value = info.GetValue(socketTextChannel) + " to " + info.GetValue(newSocketTextChannel),
                                    IsInline = true
                                });
                            }
                        }

                        fields.Add(new EmbedFieldBuilder
                        {
                            Name = info.Name,
                            Value = info.GetValue(socketTextChannel) + " to " + info.GetValue(newSocketTextChannel),
                            IsInline = true
                        });
                    }

                    break;

                case SocketVoiceChannel socketVoiceChannel:
                    guild = await this.database.LoadRecordsByGuildId(socketVoiceChannel.Guild.Id);

                    if (!GetRestTextChannel(this.shard, guild.ChannelUpdatedEvent, out restTextChannel)) return;

                    SocketVoiceChannel newSocketVoiceChannel = newChannel as SocketVoiceChannel;

                    differentPropertyInfos = EnumeratingUtilities.GetDifferentProperties(
                        socketVoiceChannel,
                        newSocketVoiceChannel, channelConstantChanges);
                        
                    foreach (PropertyInfo info in differentPropertyInfos)
                    {
                        fields.Add(new EmbedFieldBuilder
                        {
                            Name = info.Name,
                            Value = info.GetValue(socketVoiceChannel) + " to " + info.GetValue(newSocketVoiceChannel),
                            IsInline = true
                        });
                    }

                    break;

                default:
                    return;
            }


            EmbedBuilder embedBuilder = new()
            {
                Color = Color.Blue,
                Fields = fields,
                Footer = new EmbedFooterBuilder {Text = $"Modified at {DateTime.UtcNow} UTC"}
            };

            await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}