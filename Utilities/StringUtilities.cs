using System;
using System.Text.RegularExpressions;

namespace Auditor.Utilities
{
    public static class StringUtilities
    {
        public static string TypeFormat(string type)
        {
            string spacedType = Regex.Replace(type, "([A-Z])", " $1").Trim();
            string t = spacedType.Split(" ").Length > 1 ? type : Regex.Replace(type, @"[\d-]", string.Empty);

            if (t.GetType().IsPrimitive || t.GetType().Name.ToLowerInvariant() == "string")
            {
                return t.ToLowerInvariant();
            }

            return t;
        }
    }
}