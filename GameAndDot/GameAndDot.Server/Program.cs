using System.Net;
using System.Net.Sockets;
using XProtocol.shared;

using GameAndDot.Server;
using GameAndDot.Shared.Models;

namespace GameAndDot.Server;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Server");

        // <_ Old Realization _>
        /*ServerObject server = new ServerObject();// создаем сервер
        await server.ListenAsync(); // запускаем сервер*/

        // <_ New Realization _>
        var server = new XServer();
        server.Start();
        server.AcceptClients();
    }
}
