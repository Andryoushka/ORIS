using GameServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.shared;

//Класс сервера, что связывает клиентов с самим собой
public class ZServer
{
    //сокет сервера
    private readonly Socket _socket;
    //сокеты подключенных клиентов
    public readonly List<ConnectedClient> Clients;
    //пустой объект - нужен для lock , чтобы избежать ошибок многопоточности
    public readonly object ClientsLock = new object();

    private bool _listening; // слушает ли сервер новые подключения
    private bool _stopListening; // флаг для прекращения принятия покетов от клиентов

    public List<CardType> CardDeck = new List<CardType>(); // колода
    private int _playerTurnId; // ID игрока по очереди

    public ZServer()
    {
        var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostInfo.AddressList[1]; // [0]
        // создаем сокет сервера
        _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Clients = new List<ConnectedClient>();
    }

    public void AcceptClients()
    {
        //слушаем входящие подключения от клиентов
        //если такие имеются - создаем связку клиента с сервером в виде ConnectedClient
        while (true)
        {
            if (_stopListening)
            {
                return;
            }

            Socket client;

            try
            {
                client = _socket.Accept();
            }
            catch { return; }

            Console.WriteLine($"[!] Accepted client from {(IPEndPoint)client.RemoteEndPoint}");

            var c = new ConnectedClient(client, this);
            Clients.Add(c);
        }
    }

    public void Start()
    {
        if (_listening)
        {
            throw new Exception("Server is already listening incoming requests.");
        }
        //регистрируем хост для прослушки
        _socket.Bind(new IPEndPoint(IPAddress.Any, 4910));
        _socket.Listen(10);

        _listening = true;
    }

    public void Stop()
    {
        //отключает работу сервера
        if (!_listening)
        {
            throw new Exception("Server is already not listening incoming requests.");
        }

        _stopListening = true;
        _socket.Shutdown(SocketShutdown.Both);
        _listening = false;
    }
}
