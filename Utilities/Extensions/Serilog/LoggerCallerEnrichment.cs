using Auditor.Utilities.Enricher;
using Serilog;
using Serilog.Configuration;

namespace Auditor.Utilities.Extensions.Serilog
{
    static class LoggerCallerEnrichment
    {
        public static LoggerConfiguration WithCaller(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.With<CallerEnricher>();
        }
    }
}