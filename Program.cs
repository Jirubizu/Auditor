using System;
using System.Threading.Tasks;
using Auditor.Utilities.Extensions.Serilog;
using Serilog;

namespace Auditor
{
    public static class Program
    {
        private static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate:"[{Timestamp:HH:mm:ss} {Level:u3}] {Message} (at {SourceContext}){NewLine})")
                .CreateLogger();
            try
            {
                await new Auditor().SetupAsync("./config.json");
            }
            catch (Exception e)
            { 
                Log.Logger.Fatal("{e}", e);
                throw;
            }
        }
    }
}