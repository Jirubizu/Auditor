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
    public class GuildMemberUpdatedHandler : EventHandler
    {
        private readonly DiscordShardedClient shard;
        private readonly DatabaseService database;
        private readonly ILogger logger = Log.ForContext<GuildMemberUpdatedHandler>();

        public GuildMemberUpdatedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.GuildMemberUpdated += ShardOnGuildMemberUpdated;
            this.logger.Information("Registered");
        }

        private async Task ShardOnGuildMemberUpdated(SocketGuildUser prevUser, SocketGuildUser newUser)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(prevUser.Guild.Id);

            if (GetRestTextChannel(this.shard, guild.GuildMemberUpdatedEvent, out RestTextChannel restTextChannel))
            {
                List<EmbedFieldBuilder> fields = new()
                {
                    new EmbedFieldBuilder {Name = "User Id", Value = newUser.Id}
                };

                IEnumerable<PropertyInfo> differentPropertyInfos = EnumeratingUtilities.GetDifferentProperties(prevUser,
                    newUser,
                    new[] {"MutualGuilds", "Roles", "JoinedAt"});

                foreach (PropertyInfo info in differentPropertyInfos)
                {
                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = info.Name,
                        Value = $"{info.GetValue(prevUser) ?? "null"} to {info.GetValue(newUser) ?? "null"}"
                    });
                }

                EmbedBuilder embedBuilder = new()
                {
                    Color = Color.Blue,
                    Fields = fields,
                    Footer = new EmbedFooterBuilder {Text = $"Modified at {DateTime.UtcNow} UTC"}
                };

                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}