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
    public class RoleDeletedHandler: EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<RoleCreatedHandler>();

        public RoleDeletedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.RoleDeleted += ShardOnRoleDeleted;
            logger.Information("Registered");
        }

        private async Task ShardOnRoleDeleted(SocketRole arg)
        {
            GuildBson guild = await database.LoadRecordsByGuildId(arg.Guild.Id);
            
            if (GetRestTextChannel(this.shard, guild.RoleDeletedEvent.Key, out RestTextChannel restTextChannel))
            {
                EmbedBuilder embedBuilder = new()
                {
                    Color = arg.Color,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new() {Name = "Role", Value = arg.Name},
                        new() {Name = "Permissions", Value = arg.Permissions.ToList().Aggregate("", (current, permission) => current + permission + ", ")}
                    },
                    Footer = new EmbedFooterBuilder {Text = "Deleted at " + DateTime.Now}
                };

                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}