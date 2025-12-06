using System.Net.Sockets;
using System.Text.Json;

namespace GameAndDot.Shared.Models;

public class ClientObject
{
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    protected internal string UserName { get; set; } = string.Empty;
    protected internal StreamWriter Writer { get; }
    protected internal StreamReader Reader { get; }

    TcpClient client;
    ServerObject server; // объект сервера

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        client = tcpClient;
        server = serverObject;
        // получаем NetworkStream для взаимодействия с сервером
        var stream = client.GetStream();
        // создаем StreamReader для чтения данных
        Reader = new StreamReader(stream);
        // создаем StreamWriter для отправки данных
        Writer = new StreamWriter(stream);
    }

    public async Task ProcessAsync()
    {
        while (true)
        {
            try
            {
                // получаем имя пользователя
                string json = await Reader.ReadLineAsync();
                var message = JsonSerializer.Deserialize<EventMessage>(json);

                switch (message.Type)
                {
                    case Enums.EventType.PlayerConnected:
                        UserName = message.PlayerName;

                        var response = new EventMessage()
                        {
                            PlayerName = message.PlayerName,
                            Type = Enums.EventType.PlayerConnected,
                            PlayerId = Id,
                            Players = server.Clients.Select(p => p.UserName).ToList()
                        };

                        var responseJson = JsonSerializer.Serialize(response);

                        await server.BroadcastMessageAllAsync(responseJson);
                        break;

                    case Enums.EventType.PlayerDisconnected:
                        // Сообщаем остальным, что клиент вышел
                        var disconnectMsg = new EventMessage
                        {
                            Type = Enums.EventType.PlayerDisconnected,
                            PlayerName = UserName,
                            PlayerId = Id
                        };
                        var disconnectJson = JsonSerializer.Serialize(disconnectMsg);
                        await server.BroadcastMessageAllAsync(disconnectJson);

                        server.RemoveConnection(this.Id);
                        //return;
                        break;

                    case Enums.EventType.PointPlased:
                        await server.BroadcastMessageAllAsync(json);
                        break;

                    default:
                        break;
                }

                /*string? message = $"{userName} вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                await server.BroadcastMessageAsync(message, Id);
                Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();
                        if (message == null) continue;
                        message = $"{userName}: {message}";
                        Console.WriteLine(message);
                        await server.BroadcastMessageAsync(message, Id);
                    }
                    catch
                    {
                        message = $"{userName} покинул чат";
                        Console.WriteLine(message);
                        await server.BroadcastMessageAsync(message, Id);
                        break;
                    }
                }*/
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(Id);
                break;
            }
        }
    }
    // закрытие подключения
    protected internal void Close()
    {
        Writer.Close();
        Reader.Close();
        client.Close();
    }
}
