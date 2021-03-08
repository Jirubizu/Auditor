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
    public class UserBannedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<UserBannedHandler>();

        public UserBannedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.shard = s;
            this.database = d;
            this.logger.Information("Registered");
            this.shard.UserBanned += ShardOnUserBanned;
        }

        private async Task ShardOnUserBanned(SocketUser user, SocketGuild socketGuild)
        {
            
            GuildBson guild = await this.database.LoadRecordsByGuildId(socketGuild.Id);

            if (GetRestTextChannel(this.shard, guild.UserBannedEvent.Key, out RestTextChannel restTextChannel))
            {
                RestBan ban = await socketGuild.GetBanAsync(user.Id);
                
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
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Ban reason",
                        Value = ban.Reason
                    }
                };

                EmbedBuilder embedBuilder = new()
                {
                    Color = Color.Red,
                    Fields = fields,
                    ImageUrl = user.GetAvatarUrl(),
                    Footer = new EmbedFooterBuilder {Text = $"Banned on {DateTime.UtcNow} UTC"}
                };
                
                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}