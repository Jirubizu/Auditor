using System;
using System.Collections.Generic;
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
    public class RoleUpdatedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<RoleUpdatedHandler>();

        public RoleUpdatedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.RoleUpdated += ShardOnRoleUpdated;
            this.logger.Information("Registered");
        }

        private async Task ShardOnRoleUpdated(SocketRole oldRole, SocketRole newRole)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(oldRole.Guild.Id);

            if (GetRestTextChannel(this.shard, guild.RoleUpdatedEvent.Key, out RestTextChannel restTextChannel))
            {
                List<EmbedFieldBuilder> fields = new();

                foreach (PropertyInfo info in EnumeratingUtilities.GetDifferentProperties(oldRole, newRole,
                    new[] {"Members"}))
                {
                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = $"Old {info.Name}",
                        Value = info.GetValue(oldRole),
                        IsInline = true
                    });
                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = $"New {info.Name}",
                        Value = info.GetValue(newRole),
                        IsInline = true
                    });
                    fields.Add(new EmbedFieldBuilder {Name = "|", Value = "|", IsInline = true});
                }

                EmbedBuilder embedBuilder = new()
                {
                    Color = newRole.Color,
                    Fields = fields,
                    Footer = new EmbedFooterBuilder {Text = $"Modified on {DateTime.UtcNow} UTC "}
                };

                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}