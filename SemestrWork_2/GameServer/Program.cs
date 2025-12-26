using GameServer.shared;

namespace GameServer;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("SERVER");

        var server = new ZServer();
        server.Start();
        server.AcceptClients();

        while (true) { }
    }
}
