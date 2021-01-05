using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Auditor.Attributes.Preconditions;
using Auditor.Utilities.Comparators;
using Discord.Commands;
using ParameterInfo = Discord.Commands.ParameterInfo;

namespace Auditor.Services
{
    public class HelpService
    {
        private readonly CommandService commandService;
        
        public IEnumerable<CommandInfo> NoNameDubplicates;
        public Dictionary<string, Type> AvailableEnums = new (); 

        public HelpService(CommandService c)
        {
            commandService = c;
        }

        public void Setup()
        {
            NoNameDubplicates = commandService.Commands.Distinct(new CommandInfoComparator());

            foreach (CommandInfo command in commandService.Commands)
            {
                foreach (ParameterInfo parameter in command.Parameters)
                {
                    if (parameter.Type.IsEnum)
                    {
                        if (!AvailableEnums.ContainsKey(parameter.Type.Name))
                        {
                            AvailableEnums.Add(parameter.Type.Name.ToLowerInvariant(), parameter.Type);
                        }
                    }
                }
            }
        }
    }
}