using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Discord.WebSocket;

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

                if (info.Name == "Roles")
                {
                    foreach (SocketRole role in (IEnumerable<SocketRole>) ((ReadOnlyCollection<SocketRole>) info.GetValue(oldT)).GetEnumerator())
                    {
                        // TODO: Cant be fucked with this rn fucking fix my own shit I am so fucking stuuuupid!!
                    }
                }
                
                if (info.GetValue(oldT)?.GetHashCode() != info.GetValue(newT)?.GetHashCode())
                {
                    yield return info;
                }
            }
        }
    }
}