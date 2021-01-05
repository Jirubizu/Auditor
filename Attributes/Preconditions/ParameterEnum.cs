using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Auditor.Attributes.Preconditions
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = true)]
    public class ParameterEnumAttribute : Attribute
    {
    }
}