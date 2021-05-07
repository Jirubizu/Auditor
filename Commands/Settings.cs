using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Auditor.Enumerators;
using Auditor.Services;
using Auditor.Structures;
using Auditor.Utilities;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Auditor.Commands
{
    public class Settings : AuditorModule
    {
        public DatabaseService Database { get; set; }
        public DiscordShardedClient Shard { get; set; }
        private readonly ILogger logger = Log.ForContext<Settings>();

        [Command("toggle"), Summary("Change a state of one of the audits"), Alias("t")]
        public async Task Toggle(Events eventToggle, ulong? channel = null)
        {
            GuildBson guild = await Database.LoadRecordsByGuildId(Context.Guild.Id);
            PropertyInfo info = typeof(GuildBson).GetProperties().FirstOrDefault(o =>
                string.Equals(o.Name, eventToggle + "Event", StringComparison.InvariantCultureIgnoreCase));

            if (info == null)
            {
                await SendErrorAsync("Failed getting info, make sure you type the right event name");
            }
            else
            {
                if (info.GetValue(guild) != null)
                {
                    guild.GetType().GetProperty(info.Name)?.SetValue(guild, null);
                }
                else
                {
                    guild.GetType().GetProperty(info.Name)?.SetValue(guild, channel ?? (ulong?) 1);
                }
                await Database.UpdateGuild(guild);
            }
        }

        [Command("togglemultiple"), Summary("Change the state of multiple events"), Alias("tm")]
        public async Task ToggleMultiple(params Events[] eventToggle)
        {
            GuildBson guild = await Database.LoadRecordsByGuildId(Context.Guild.Id);

            foreach (Events e in eventToggle)
            {
                PropertyInfo info = typeof(GuildBson).GetProperties().FirstOrDefault(o =>
                    string.Equals(o.Name, e + "Event", StringComparison.InvariantCultureIgnoreCase));
                if (info == null)
                {
                    await SendErrorAsync($"Failed getting {e}, make sure you type the right event name");
                }
                else
                {
                    if (info.GetValue(guild) != null)
                    {
                        guild.GetType().GetProperty(info.Name)?.SetValue(guild, null);
                    }
                    else
                    {
                        guild.GetType().GetProperty(info.Name)?.SetValue(guild, (ulong?) 1);
                    }
                    
                    await Database.UpdateGuild(guild);
                }
            }
        }

        [Command("auditorsettings"), Summary("Change the state of the auditor"), Alias("settings", "s")]
        public async Task AuditorSettings()
        {
            string output = "";
            GuildBson guild = await Database.LoadRecordsByGuildId(Context.Guild.Id);

            foreach ((string name, ulong? channel) in GetSettingStates(guild))
            {
                string v = channel != null ? "🟢" : "🔴";
                output += $"{v} | {name.Replace("Event", "")}\n";
            }

            EmbedBuilder embed = new()
            {
                Title = "Settings",
                Description = output,
                Color = Color.Green,
                Timestamp = DateTimeOffset.Now
            };

            await SendEmbedAsync(embed.Build());
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("createsection"), Summary("Create a section of channels with currently enabled watchers"), Alias("cs")]
        public async Task CreateSection()
        {
            GuildBson guild = await Database.LoadRecordsByGuildId(Context.Guild.Id);

            RestCategoryChannel categoryChannel = await Context.Guild.CreateCategoryChannelAsync("Auditor");
            await categoryChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole,
                OverwritePermissions.DenyAll(categoryChannel));
            foreach ((string name, ulong? channel) in GetSettingStates(guild))
            {
                if (channel == null) continue;

                RestTextChannel textChannel = await Context.Guild.CreateTextChannelAsync(name.Replace("Event", ""),
                    o => o.CategoryId = categoryChannel.Id);
                guild.GetType().GetProperty(name)?.SetValue(guild, textChannel.Id);
            }

            guild.CategoryId = categoryChannel.Id;
            await Database.UpdateGuild(guild);

            await SendSuccessAsync("Created Auditor sections, please move to a more convenient place.");
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("deletesection"), Summary("Delete Auditor section (Cleanup)"), Alias("ds")]
        public async Task DeleteSection()
        {
            GuildBson guild = await Database.LoadRecordsByGuildId(Context.Guild.Id);
            if (guild.CategoryId != null)
            {
                SocketCategoryChannel categoryChannel = Context.Guild.GetCategoryChannel(guild.CategoryId.Value);
                foreach (SocketGuildChannel channel in categoryChannel.Channels)
                {
                    await channel.DeleteAsync();
                }

                foreach (PropertyInfo info in guild.GetType().GetProperties()
                    .Where(c => c.PropertyType == typeof(ulong?) && c.GetValue(guild) != null))
                {
                    guild.GetType().GetProperty(info.Name)?.SetValue(guild, (ulong?) 1);
                }

                await categoryChannel.DeleteAsync();
            }

            guild.CategoryId = null;

            await Database.UpdateGuild(guild);

            await SendSuccessAsync("Deleted Auditor sections.");
        }

        [Command("prefix"), Summary("Change the prefix of the bot")]
        public async Task Prefix(string prefix)
        {
            GuildBson guild = await Database.LoadRecordsByGuildId(Context.Guild.Id);
            guild.Prefix = prefix;
            await Database.UpdateGuild(guild);
            await SendSuccessAsync("Prefix Updated");
        }

        [Command("uploadSettings"), Summary("Upload settings via url")]
        public async Task UploadSettings(string url)
        {
            using (HttpClient client = new())
            {
                //client.get
            }

            await SendSuccessAsync("Updated Settings");
        }

        [Command("settingsUrl"), Summary("Generates a url that can be used to visually modify the settings")]
        public async Task SettingsUrl()
        {
            GuildBson guild = await this.Database.LoadRecordsByGuildId(Context.Guild.Id);

            string json = JsonConvert.SerializeObject(guild, Formatting.None, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore});
            JObject jobj = JObject.Parse(json);
            jobj.Remove("CollectionId");
            jobj.Remove("GuildId");
            string url = "http://localhost:5000/?existing_data=" + jobj.ToString().Replace("\r\n", "").Replace(" ", "");
            Uri uri = new(url);
            // }
            EmbedBuilder embedBuilder = new()
            {
                Author = new EmbedAuthorBuilder
                    {Name = "Click Me", Url = uri.AbsoluteUri}
            };

            await SendEmbedAsync(embedBuilder.Build());
        }

        private IEnumerable<(string, ulong?)> GetSettingStates(GuildBson guild)
        {
            List<string> IgnoredProperties = new List<string>
            {
                "CollectionId",
                "GuildId",
                "Prefix",
                "CategoryId"
            };
            foreach (PropertyInfo setting in typeof(GuildBson).GetProperties())
            {
                if (IgnoredProperties.Contains(setting.Name))
                {
                    continue;
                }

                ulong? val = (ulong?) setting.GetValue(guild);
                yield return (setting.Name, val);
            }
        }
    }
}