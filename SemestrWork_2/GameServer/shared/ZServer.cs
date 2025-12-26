using GameServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.shared;

public class ZServer
{
    private readonly Socket _socket;
    public readonly List<ConnectedClient> Clients;
    public readonly object ClientsLock = new object();

    private bool _listening;
    private bool _stopListening;

    public List<CardType> CardDeck = new List<CardType>(); // колода
    private List<CardType> CardDrop = new List<CardType>(); // сброс
    private int _playerTurnId; // ID игрока по очереди

    public ZServer()
    {
        var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostInfo.AddressList[1]; // [0]

        _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Clients = new List<ConnectedClient>();
    }

    public void AcceptClients()
    {
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

        _socket.Bind(new IPEndPoint(IPAddress.Any, 4910));
        _socket.Listen(10);

        _listening = true;
    }

    public void Stop()
    {
        if (!_listening)
        {
            throw new Exception("Server is already not listening incoming requests.");
        }

        _stopListening = true;
        _socket.Shutdown(SocketShutdown.Both);
        _listening = false;
    }
}
