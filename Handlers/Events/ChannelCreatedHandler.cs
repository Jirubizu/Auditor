using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auditor.Services;
using Auditor.Structures;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Auditor.Handlers.Events
{
    public class ChannelCreatedHandler : IEventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;

        public ChannelCreatedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.shard = s;
            this.database = d;
            this.shard.ChannelCreated += ShardOnChannelCreated;
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
                    if (guild.ChannelCreatedEvent.Key == null) return;
                    restTextChannel =
                        await this.shard.Rest.GetChannelAsync((ulong) guild.ChannelCreatedEvent.Key) as RestTextChannel;

                    if (restTextChannel == null) return;

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
                    if (guild.ChannelCreatedEvent.Key == null) return;
                    restTextChannel =
                        await this.shard.Rest.GetChannelAsync((ulong) guild.ChannelCreatedEvent.Key) as RestTextChannel;

                    if (restTextChannel == null) return;

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
                Footer = new EmbedFooterBuilder{Text = "Created at " + DateTime.Now}
            };
            await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}