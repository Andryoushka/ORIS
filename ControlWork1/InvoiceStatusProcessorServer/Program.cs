using MiniHttpServer.Framework;
using MiniHttpServer.Framework.share;
using System.Net;

namespace InvoiceStatusProcessorServer;

internal class Program
{
    static void Main(string[] args)
    {
        var settings = SettingsManager.Instance;
        Console.WriteLine($"http://{settings.Settings.Domain}:{settings.Settings.Port}");
        var httpServer = new HttpServer();
        httpServer.StartAsync();

        while (true) { }
    }
}
