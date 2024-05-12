using Serilog;
using Serilog.Sinks.Logtube;

using var log = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Logtube()
            .CreateLogger();

Log.Logger = log;