using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auditor.Services;
using Auditor.Utilities;
using Discord;
using Discord.Commands;
using CService = Discord.Commands.CommandService;
using HService = Auditor.Services.HelpService;

namespace Auditor.Commands
{
    public class HelpModule : AuditorModule
    {
        public CService CommandService { get; set; }
        public DatabaseService Database { get; set; }
        public HService HelpService { get; set; }

        [Command("help"), Alias("h"), Summary("Display the help window")]
        public async Task Help()
        {
            StringBuilder builder = new ();
            builder.Append("```cs\n");
            string prefix = Database.LoadRecordsByGuildId(Context.Guild.Id).Result.Prefix;
            
            foreach (CommandInfo command in HelpService.NoNameDubplicates)
            {
                builder.Append(
                    $"{prefix}{command.Name.ToLowerInvariant()} {"//".PadLeft(20 - command.Name.Length)} {command.Summary} \n");
            }

            builder.Append("```");
            EmbedBuilder embed = new ()
            {
                Description = builder.ToString(),
                Color = Color.Gold
            };

            await SendEmbedAsync(embed.Build());
        }

        [Command("help")]
        public async Task Help(string module)
        {
            StringBuilder builder = new ("```cs\n");
            
            if (HelpService.AvailableEnums.TryGetValue(module, out Type e))
            {
                builder.AppendLine("Available Enum options\n");
                builder.Append(string.Join(", ", Enum.GetNames(e)));
            }
            else
            {
                var commands =
                    CommandService.Commands.Select(c => c)
                        .Where(c => c.Name.ToLowerInvariant().Equals(module.ToLowerInvariant()));
                foreach (CommandInfo command in commands)
                {
                    if (command.Summary != null)
                    {
                        builder.AppendLine($"// {command.Summary}");
                    }

                    if (command.Aliases.Count > 1)
                    {
                        builder.AppendLine($"// Aliases: {string.Join(", ", command.Aliases.Skip(1))}");
                    }

                    if (command.Remarks != null)
                    {
                        builder.AppendLine($"/*\n{command.Remarks}\n*/");
                    }

                    StringBuilder paras = new ();
                    foreach (ParameterInfo commandParameter in command.Parameters)
                    {
                        paras.Append(
                            $"[{StringUtilities.TypeFormat(commandParameter.Type.Name)} {commandParameter.Name}]");
                    }

                    builder.AppendLine(
                        $"{Database.LoadRecordsByGuildId(Context.Guild.Id).Result.Prefix}{command.Name.ToLowerInvariant()} {paras}\n");
                }
            }

            builder.Append("```");

            await ReplyAsync(builder.ToString());
        }
    }
}