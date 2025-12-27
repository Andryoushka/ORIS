using GameServer.shared;

namespace GameServer;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("SERVER");

        //Создает экземпляр сервера
        var server = new ZServer();
        //Начинает прослушивать подключения клиентов
        server.Start();
        //Регистрирует клиентов
        server.AcceptClients();
        //Цикл не позволяет серверу закончить работу
        while (true) { }
    }
}
