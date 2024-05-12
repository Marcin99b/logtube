using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;
using System.Net.Sockets;
using System.Text;

namespace Serilog.Sinks.Logtube;

public class LogtubeSink : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        var json = JsonConvert.SerializeObject(logEvent);

        var client = new TcpClient("127.0.0.1", 8080);
        var stream = client.GetStream();

        var bytesToSend = Encoding.UTF8.GetBytes(json);
        stream.Write(bytesToSend, 0, bytesToSend.Length);

        client.Close();
    }
}
