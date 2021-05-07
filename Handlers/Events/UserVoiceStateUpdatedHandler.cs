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
    public class UserVoiceStateUpdatedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<UserVoiceStateUpdatedHandler>();

        public UserVoiceStateUpdatedHandler(DatabaseService d, DiscordShardedClient s)
        {
            this.database = d;
            this.shard = s;
            this.shard.UserVoiceStateUpdated += ShardOnUserVoiceStateUpdated;
            this.logger.Information("Registered");
        }

        private async Task ShardOnUserVoiceStateUpdated(SocketUser user, SocketVoiceState prevVoiceState,
            SocketVoiceState newVoiceState)
        {
            ulong? guildId = prevVoiceState.VoiceChannel?.Guild.Id ?? newVoiceState.VoiceChannel?.Guild.Id;

            GuildBson guild = await this.database.LoadRecordsByGuildId(guildId.Value);

            if (GetRestTextChannel(this.shard, guild.UserVoiceStateUpdatedEvent,
                out RestTextChannel restTextChannel))
            {
                List<EmbedFieldBuilder> fields = new();

                if (prevVoiceState.VoiceChannel != null)
                {
                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = $"Left {prevVoiceState.VoiceChannel.Name}",
                        Value = DateTime.UtcNow + " UTC"
                    });
                }

                if (newVoiceState.VoiceChannel != null)
                {
                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = $"Joined {newVoiceState.VoiceChannel.Name}",
                        Value = DateTime.UtcNow + " UTC"
                    });
                }

                EmbedBuilder embedBuilder = new()
                {
                    Title = user.Username,
                    Fields = fields,
                    Color = Color.Blue,
                    ThumbnailUrl = user.GetAvatarUrl()
                };

                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}