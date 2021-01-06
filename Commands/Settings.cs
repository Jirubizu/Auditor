using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            PropertyInfo info = typeof(GuildBson).GetProperties().FirstOrDefault(o => string.Equals(o.Name, eventToggle+"Event", StringComparison.InvariantCultureIgnoreCase));
            if (info == null)
            {
                await SendErrorAsync("Failed getting info, make sure you type the right event name");
            }
            else
            {
                MutableKeyValuePair<ulong?, bool> kpv = (MutableKeyValuePair<ulong?, bool>)guild.GetType().GetProperty(info.Name).GetValue(guild);
                kpv.Value = !kpv.Value;
                kpv.Key = channel;
                guild.GetType().GetProperty(info.Name).SetValue(guild, kpv);
                await Database.UpdateGuild(guild);
            }
        }
        
        [Command("auditorsettings"), Summary("Change the state of the auditor"), Alias("settings", "s")]
        public async Task AuditorSettings()
        {
            string output = "";
            GuildBson guild = await Database.LoadRecordsByGuildId(Context.Guild.Id);

            foreach ((PropertyInfo key, bool value) in GetSettingStates(guild))
            {
                string v = value ? "🟢" : "🔴";
                output += $"{v} | {key.Name}\n";
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
            await categoryChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, OverwritePermissions.DenyAll(categoryChannel));
            foreach ((PropertyInfo key, bool value) in GetSettingStates(guild))
            {
                if (!value) continue;
                
                RestTextChannel textChannel = await Context.Guild.CreateTextChannelAsync(key.Name.Replace("Event", ""), o => o.CategoryId = categoryChannel.Id);
                MutableKeyValuePair<ulong?, bool> kpv = (MutableKeyValuePair<ulong?, bool>)guild.GetType().GetProperty(key.Name).GetValue(guild);
                kpv.Key = textChannel.Id;
                guild.GetType().GetProperty(key.Name).SetValue(guild, kpv);
                
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
            SocketCategoryChannel categoryChannel = Context.Guild.GetCategoryChannel(guild.CategoryId);
            foreach (SocketGuildChannel channel in categoryChannel.Channels)
            {
                await channel.DeleteAsync();
            }

            foreach (PropertyInfo info in guild.GetType().GetProperties().Where(c => c.PropertyType == typeof(MutableKeyValuePair<ulong?, bool>)))
            {
                MutableKeyValuePair<ulong?, bool> kvp = (MutableKeyValuePair<ulong?, bool>)info.GetValue(guild);
                kvp.Key = null;
                guild.GetType().GetProperty(info.Name).SetValue(guild, kvp);
            }

            await categoryChannel.DeleteAsync();
            guild.CategoryId = 0;

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

        private IEnumerable<KeyValuePair<PropertyInfo, bool>> GetSettingStates(GuildBson guild)
        {
            foreach (PropertyInfo setting in typeof(GuildBson).GetProperties())
            {
                if (setting.GetValue(guild)?.GetType() != typeof(MutableKeyValuePair<ulong?, bool>))
                {
                    continue;
                }

                MutableKeyValuePair<ulong?, bool> kvp = (MutableKeyValuePair<ulong?, bool>) setting.GetValue(guild);
                yield return new KeyValuePair<PropertyInfo, bool>(setting, kvp.Value);
            }
        }
            
    }
}