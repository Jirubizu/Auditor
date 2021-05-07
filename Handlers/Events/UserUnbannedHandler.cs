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
    public class UserUnbannedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<UserLeftHandler>();

        public UserUnbannedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.UserUnbanned += ShardOnUserUnbanned;
            this.logger.Information("Registered");
        }

        private async Task ShardOnUserUnbanned(SocketUser user, SocketGuild socketGuild)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(socketGuild.Id);

            if (GetRestTextChannel(this.shard, guild.UserUnbannedEvent, out RestTextChannel restTextChannel))
            {
                List<EmbedFieldBuilder> fields = new()
                {
                    new EmbedFieldBuilder
                    {
                        Name = "Username",
                        Value = user.Username
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "User Id",
                        Value = user.Id
                    }
                };

                EmbedBuilder embedBuilder = new()
                {
                    Color = Color.Green,
                    Fields = fields,
                    ImageUrl = user.GetAvatarUrl(),
                    Footer = new EmbedFooterBuilder {Text = $"Unbanned on {DateTime.UtcNow} UTC"}
                };

                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}