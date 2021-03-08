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
            s.GuildMemberUpdated += ShardOnGuildMemberUpdated;
            shard = s;
            logger.Information("Registered");
        }
        
        private async Task ShardOnGuildMemberUpdated(SocketGuildUser prevUser, SocketGuildUser newUser)
        {
            GuildBson guild = await database.LoadRecordsByGuildId(prevUser.Guild.Id);

            if (GetRestTextChannel(this.shard, guild.GuildMemberUpdatedEvent.Key, out RestTextChannel restTextChannel))
            {
                List<EmbedFieldBuilder> fields = new()
                {
                    new EmbedFieldBuilder {Name = "User Id", Value = newUser.Id}
                };
            
                foreach (PropertyInfo info in EnumeratingUtilities.GetDifferentProperties(prevUser, newUser, new[] {"MutualGuilds", "Roles", "JoinedAt"}))
                {
                    fields.Add(new EmbedFieldBuilder{Name = info.Name, Value = $"{info.GetValue(prevUser) ?? "null"} to {info.GetValue(newUser) ?? "null"}"});
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