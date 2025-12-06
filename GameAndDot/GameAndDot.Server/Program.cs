using System.Net;
using System.Net.Sockets;

using GameAndDot.Server;
using GameAndDot.Shared.Models;

namespace GameAndDot.Server;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Server");

        ServerObject server = new ServerObject();// создаем сервер
        await server.ListenAsync(); // запускаем сервер
    }
}
