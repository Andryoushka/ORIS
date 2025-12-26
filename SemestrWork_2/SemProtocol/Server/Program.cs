using ZProtocol.shared;

namespace Server;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Server");
        var server = new ZServer();
        server.Start();
        server.AcceptClients();

        Console.ReadLine();

    }
}
