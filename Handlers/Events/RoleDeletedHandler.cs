using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auditor.Services;
using Auditor.Structures;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class RoleDeletedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<RoleDeletedHandler>();

        public RoleDeletedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.RoleDeleted += ShardOnRoleDeleted;
            this.logger.Information("Registered");
        }

        private async Task ShardOnRoleDeleted(SocketRole role)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(role.Guild.Id);

            if (GetRestTextChannel(this.shard, guild.RoleDeletedEvent.Key, out RestTextChannel restTextChannel))
            {
                string permissions = role.Permissions.ToList()
                    .Aggregate("", (current, permission) => current + permission + ", ");

                EmbedBuilder embedBuilder = new()
                {
                    Color = role.Color,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new() {Name = "Role", Value = role.Name},
                        new()
                        {
                            Name = "Permissions",
                            Value = permissions
                        }
                    },
                    Footer = new EmbedFooterBuilder {Text = $"Deleted at {DateTime.UtcNow} UTC"}
                };

                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}