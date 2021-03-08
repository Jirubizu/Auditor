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
    public class UserLeftHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<UserLeftHandler>();

        public UserLeftHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.shard = s;
            this.database = d;
            this.logger.Information("Registered");
            this.shard.UserLeft += ShardOnUserLeft;
        }

        private async Task ShardOnUserLeft(SocketGuildUser user)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(user.Guild.Id);

            if (GetRestTextChannel(this.shard, guild.UserLeftEvent.Key, out RestTextChannel restTextChannel))
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
                    Color = Color.Blue,
                    Fields = fields,
                    ImageUrl = user.GetAvatarUrl(),
                    Footer = new EmbedFooterBuilder {Text = $"Left on {DateTime.UtcNow} UTC"}
                };
                
                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}