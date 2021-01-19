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
            this.shard = s;
            this.database = d;
            this.logger.Information("Registered");
            this.shard.UserUnbanned += ShardOnUserUnbanned;
        }

        private async Task ShardOnUserUnbanned(SocketUser user, SocketGuild socketGuild)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(socketGuild.Id);

            if (GetRestTextChannel(this.shard, guild.UserUnbannedEvent.Key, out RestTextChannel restTextChannel))
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
                    Footer = new EmbedFooterBuilder {Text = "Unbanned on " + DateTime.Now}
                };
                
                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}