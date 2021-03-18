using System;
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
    public class RoleCreatedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<RoleCreatedHandler>();

        public RoleCreatedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.RoleCreated += ShardOnRoleCreated;
            this.logger.Information("Registered");
        }

        private async Task ShardOnRoleCreated(SocketRole role)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(role.Guild.Id);

            if (GetRestTextChannel(this.shard, guild.RoleCreatedEvent.Key, out RestTextChannel restTextChannel))
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
                    Footer = new EmbedFooterBuilder {Text = $"Created at {DateTime.UtcNow} UTC"}
                };

                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}