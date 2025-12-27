using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ZombieCats;

// класс Клиента
public class ZClient
{
    // событие получения пакетов с информацией (для каждого игрового экрана назначаем разную логику обработки)
    public Action<byte[]> OnPacketRecieve { get; set; }

    private readonly Queue<byte[]> _packetSendingQueue = new Queue<byte[]>();

    private Socket _socket; // сокет клиента
    private IPEndPoint _serverEndPoint;

    public void Connect(string ip, int port)
    {
        // подключаемся к серверу
        Connect(new IPEndPoint(IPAddress.Parse(ip), port));
    }

    public void Connect(IPEndPoint server)
    {
        _serverEndPoint = server;

        var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostInfo.AddressList[1]; // [0]

        // здесь создаем сокет клиента и подключаемся к серверу
        _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(_serverEndPoint);

        Task.Run((Action)RecievePackets); // поток приема пакетов
        Task.Run((Action)SendPackets); // поток отправки пакетов
    }

    public void QueuePacketSend(byte[] packet)
    {
        // очередь отправки пакетов
        if (packet.Length > 256)
        {
            throw new Exception("Max packet size is 256 bytes.");
        }

        _packetSendingQueue.Enqueue(packet);
    }

    private void RecievePackets()
    {
        // пока клиент подключен - слушаем входящие сообщения
        while (_socket.Connected)
        {
            try
            {
                var buff = new byte[256];
                var received = _socket.Receive(buff);
                if (received == 0)
                    break;

                buff = buff.TakeWhile((b, i) =>
                {
                    if (b != 0xFF) return true;
                    return buff[i + 1] != 0;
                }).Concat(new byte[] { 0xFF, 0 }).ToArray();

                OnPacketRecieve?.Invoke(buff);
            }
            catch (Exception ex)
            {
                break;
            }
        }
    }

    private void SendPackets()
    {
        // отправляем сообщения
        while (_socket.Connected)
        {
            try
            {
                if (_packetSendingQueue.Count == 0)
                {
                    //Thread.Sleep(100);
                    continue;
                }

                var packet = _packetSendingQueue.Dequeue();
                _socket.Send(packet);

                //Thread.Sleep(100);
            }
            catch
            {
                break;
            }
        }
    }

    public void CloseConnection()
    {
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        _socket.Dispose();
    }
}
