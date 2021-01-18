using System;
using System.Collections.Generic;
using System.Linq;
using Auditor.Utilities.Comparators;
using Discord.Commands;
using ParameterInfo = Discord.Commands.ParameterInfo;

namespace Auditor.Services
{
    public class HelpService
    {
        private readonly CommandService commandService;

        public IEnumerable<CommandInfo> NoNameDubplicates;
        public Dictionary<string, Type> AvailableEnums = new();

        public HelpService(CommandService c)
        {
            commandService = c;
        }

        public void Setup()
        {
            NoNameDubplicates = commandService.Commands.Distinct(new CommandInfoComparator());

            // NoNameDubplicates = commandService.Commands.Where(c => c.Summary != null).GroupBy(c => c.Name)
            //     .Select(c => c.First());

            foreach (CommandInfo command in commandService.Commands)
            {
                foreach (ParameterInfo parameter in command.Parameters)
                {
                    if (!parameter.Type.IsEnum) continue;
                    if (!AvailableEnums.ContainsKey(parameter.Type.Name.ToLowerInvariant()))
                    {
                        AvailableEnums.Add(parameter.Type.Name.ToLowerInvariant(), parameter.Type);
                    }
                }
            }
        }
    }
}