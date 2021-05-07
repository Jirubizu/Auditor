using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Discord;

namespace Auditor.Utilities
{
    public static class EnumeratingUtilities
    {
        public static IEnumerable<PropertyInfo> GetDifferentProperties<T>(T oldT, T newT, string[] ignorePropertyNames)
        {
            foreach (PropertyInfo info in oldT.GetType().GetProperties())
            {
                // if (info.GetValue(oldT) == null) continue;
                if (ignorePropertyNames.Contains(info.Name)) continue;

                
                // Possibly can be removed completely due to discord having its own event
                // if (info.Name == "Roles")
                // {
                //     IEnumerable<SocketRole> roles =
                //         (IEnumerable<SocketRole>) ((ReadOnlyCollection<SocketRole>) info.GetValue(oldT))
                //         .GetEnumerator();
                //     
                //     foreach (SocketRole role in roles)
                //     {
                //         // 
                //     }
                // }
                if (info.GetValue(oldT)?.GetHashCode() != info.GetValue(newT)?.GetHashCode())
                {
                    yield return info;
                }
            }
        }
    }
}