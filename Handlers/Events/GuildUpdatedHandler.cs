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
    public class GuildUpdatedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<GuildUpdatedHandler>();

        public GuildUpdatedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.GuildUpdated += OnGuildUpdated;
            this.logger.Information("Registered");
        }

        private async Task OnGuildUpdated(SocketGuild prevGuild, SocketGuild newGuild)
        {
            GuildBson guild = await this.database.LoadRecordsByGuildId(prevGuild.Id);

            if (GetRestTextChannel(this.shard, guild.GuildUpdatedEvent.Key, out RestTextChannel restTextChannel))
            {
                List<EmbedFieldBuilder> fields = new();

                foreach (PropertyInfo info in EnumeratingUtilities.GetDifferentProperties(prevGuild, newGuild, new[] {""}))
                {
                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = info.Name, Value = $"{info.GetValue(prevGuild) ?? "null"} to {info.GetValue(newGuild) ?? "null"}"
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