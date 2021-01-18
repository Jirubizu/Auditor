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
            logger.Information("Registered");
        }

        private async Task ShardOnRoleCreated(SocketRole arg)
        {
            GuildBson guild = await database.LoadRecordsByGuildId(arg.Guild.Id);

            if (GetRestTextChannel(this.shard, guild.RoleCreatedEvent.Key, out RestTextChannel restTextChannel))
            {
                EmbedBuilder embedBuilder = new()
                {
                    Color = arg.Color,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new() {Name = "Role", Value = arg.Name},
                        new() {Name = "Permissions", Value = arg.Permissions.ToList().Aggregate("", (current, permission) => current + permission + ", ")}
                    },
                    Footer = new EmbedFooterBuilder {Text = "Created at " + DateTime.Now}
                };

                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}