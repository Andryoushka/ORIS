using MiniHttpServer.Framework;
using MiniHttpServer.Framework.share;
using System.Net;

namespace InvoiceStatusProcessorServer;

internal class Program
{
    static void Main(string[] args)
    {
        var settings = SettingsManager.Instance;
        var httpServer = new HttpServer();
        httpServer.StartAsync();

        while (true) { }
    }
}
