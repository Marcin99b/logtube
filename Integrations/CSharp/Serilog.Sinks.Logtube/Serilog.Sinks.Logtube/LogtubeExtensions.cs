using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Sinks.Logtube;

public static class LogtubeExtensions
{
    public static LoggerConfiguration Logtube(this LoggerSinkConfiguration sinkConfiguration, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
    {
        return sinkConfiguration.Sink(new LogtubeSink(), restrictedToMinimumLevel);
    }
}
