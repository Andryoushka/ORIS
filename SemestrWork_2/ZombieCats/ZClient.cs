using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ZombieCats;

public class ZClient
{
    public Action<byte[]> OnPacketRecieve { get; set; }

    private readonly Queue<byte[]> _packetSendingQueue = new Queue<byte[]>();

    private Socket _socket;
    private IPEndPoint _serverEndPoint;

    public void Connect(string ip, int port)
    {
        Connect(new IPEndPoint(IPAddress.Parse(ip), port));
    }

    public void Connect(IPEndPoint server)
    {
        _serverEndPoint = server;

        var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostInfo.AddressList[1]; // [0]

        _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(_serverEndPoint);

        Task.Run((Action)RecievePackets);
        Task.Run((Action)SendPackets);
    }

    public void QueuePacketSend(byte[] packet)
    {
        if (packet.Length > 256)
        {
            throw new Exception("Max packet size is 256 bytes.");
        }

        _packetSendingQueue.Enqueue(packet);
    }

    private void RecievePackets()
    {
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
