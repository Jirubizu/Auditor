using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Discord.Commands;

namespace Auditor.Utilities.Comparators
{
    public class CommandInfoComparator: IEqualityComparer<CommandInfo>
    {

        public bool Equals(CommandInfo x, CommandInfo y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }
            
            return x.Name == y.Name;
        }

        public int GetHashCode([DisallowNull] CommandInfo obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}