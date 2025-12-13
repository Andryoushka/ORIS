using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XProtocol;
using XProtocol.Message;
using XProtocol.Serializator;


namespace XProtocol.shared
{
    public class ConnectedClient
    {
        protected string UserName { get; set; } = string.Empty;
        protected string Id { get; } = Guid.NewGuid().ToString();

        protected XServer _server { get; set; }

        public Socket Client { get; }

        private readonly Queue<byte[]> _packetSendingQueue = new Queue<byte[]>();

        public ConnectedClient(Socket client, XServer server)
        {
            Client = client;
            _server = server;
            Task.Run((Action)ProcessIncomingPackets);
            Task.Run((Action)SendPackets);
        }

        private void ProcessIncomingPackets()
        {
            while (true) // Слушаем пакеты, пока клиент не отключится.
            {
                var buff = new byte[256]; // Максимальный размер пакета - 256 байт.
                Client.Receive(buff);

                buff = buff.TakeWhile((b, i) =>
                {
                    if (b != 0xFF) return true;
                    return buff[i + 1] != 0;
                }).Concat(new byte[] { 0xFF, 0 }).ToArray();

                var parsed = XPacket.Parse(buff);

                if (parsed != null)
                {
                    ProcessIncomingPacket(parsed);
                }
            }
        }

        private void ProcessIncomingPacket(XPacket packet)
        {
            var type = XPacketTypeManager.GetTypeFromPacket(packet);
            var message = XPacketConverter.Deserialize<XEventMessage>(packet);
            XEventMessage newMessage;
            byte[] response;

            switch (type)
            {
                case XPacketType.Handshake:
                    ProcessHandshake(packet);
                    break;
                case XPacketType.Unknown:
                    break;
                case XPacketType.PlayerConnected:
                    UserName = message.PlayerName;

                    newMessage = new XEventMessage
                    {
                        PlayerName = message.PlayerName,
                        PlayerId = Id,
                        PlayersString = string.Join("/", _server._clients.Select(c => c.UserName))
                    };
                    response = XPacketConverter.Serialize(XPacketType.PlayerConnected,newMessage)
                        .ToPacket();

                    SendPacketsToAllClients(response);

                    break;
                case XPacketType.PlayerDisconnected:
                    break;

                case XPacketType.PointPlased:
                    newMessage = new XEventMessage()
                    {
                        PlayerName = message.PlayerName,
                        X = message.X,
                        Y = message.Y,
                    };
                    response = XPacketConverter.Serialize(XPacketType.PointPlased, newMessage)
                        .ToPacket();
                    SendPacketsToOtherClients(response);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ProcessHandshake(XPacket packet)
        {
            //Console.WriteLine("Recieved handshake packet.");

            var handshake = XPacketConverter.Deserialize<XPacketHandshake>(packet);
            handshake.MagicHandshakeNumber -= 15;

            //Console.WriteLine("Answering..");

            QueuePacketSend(XPacketConverter.Serialize(XPacketType.Handshake, handshake).ToPacket());
        }

        public void QueuePacketSend(byte[] packet)
        {
            if (packet.Length > 256)
            {
                throw new Exception("Max packet size is 256 bytes.");
            }

            _packetSendingQueue.Enqueue(packet);
        }

        private void SendPackets()
        {
            while (true)
            {
                if (_packetSendingQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                var packet = _packetSendingQueue.Dequeue();
                Client.Send(packet);

                Thread.Sleep(100);
            }
        }

        private void SendPacketsToAllClients(byte[] packet)
        {
            foreach (var client in _server._clients)
            {
                client.Client.Send(packet);
            }
        }

        private void SendPacketsToOtherClients(byte[] packet)
        {
            foreach (var client in _server._clients)
            {
                if (client.UserName != UserName)
                    client.Client.Send(packet);
            }
        }
    }
}
