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
    public class UserUpdatedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<UserUpdatedHandler>();

        public UserUpdatedHandler(DatabaseService d, DiscordShardedClient s)
        {
            this.database = d;
            this.shard = s;
            this.shard.UserUpdated += ShardOnUserUpdated;
            this.logger.Information("Registered");
        }

        private async Task ShardOnUserUpdated(SocketUser prevUser, SocketUser newUser)
        {
            List<GuildBson> guilds = await this.database.LoadRecords();

            foreach (GuildBson guild in guilds)
            {
                if (GetRestTextChannel(this.shard, guild.UserUpdatedEvent.Key, out RestTextChannel restTextChannel))
                {
                    List<EmbedFieldBuilder> fields = new();

                    IEnumerable<PropertyInfo> differentPropertyInfos = EnumeratingUtilities.GetDifferentProperties(
                        prevUser, newUser,
                        new[] {""});

                    foreach (PropertyInfo info in differentPropertyInfos)
                    {
                        fields.Add(new EmbedFieldBuilder
                        {
                            Name = $"Old {info.Name}", Value = info.GetValue(prevUser), IsInline = true
                        });
                        fields.Add(new EmbedFieldBuilder
                        {
                            Name = $"New {info.Name}", Value = info.GetValue(newUser), IsInline = true
                        });
                        fields.Add(new EmbedFieldBuilder {Name = "|", Value = "|", IsInline = true});
                    }

                    EmbedBuilder embedBuilder = new()
                    {
                        Color = Color.Blue,
                        Fields = fields,
                        Footer = new EmbedFooterBuilder {Text = $"Modified on {DateTime.UtcNow} UTC "}
                    };

                    await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
                }
            }
        }
    }
}