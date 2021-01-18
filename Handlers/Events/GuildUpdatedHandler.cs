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
    public class GuildUpdatedHandler : IEventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<GuildUpdatedHandler>();

        public GuildUpdatedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            s.GuildUpdated += OnGuildUpdated;
            shard = s;
        }

        private async Task OnGuildUpdated(SocketGuild arg1, SocketGuild arg2)
        {
            GuildBson guild = await database.LoadRecordsByGuildId(arg1.Id);
            
            if (guild.GuildMemberUpdatedEvent.Key == null)
            {
                return;
            }

            if (!(await this.shard.Rest.GetChannelAsync((ulong) guild.GuildMemberUpdatedEvent.Key) is RestTextChannel restTextChannel))
            {
                logger.Warning("restTextChannel is null");
                return;
            }

            List<EmbedFieldBuilder> fields = new();

            foreach (PropertyInfo info in EnumeratingUtilities.GetDifferentProperties(arg1, arg2, new []{""}))
            {
                fields.Add(new EmbedFieldBuilder{Name = info.Name, Value = $"{info.GetValue(arg1) ?? "null"} to {info.GetValue(arg2) ?? "null"}"});
            }

            EmbedBuilder embedBuilder = new()
            {
                Color = Color.Blue,
                Fields = fields,
                Footer = new EmbedFooterBuilder {Text = "Modified at " + DateTime.Now}
            };

            await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}