using System.Net.Sockets;
using ZProtocol;
using ZProtocol.Message;
using ZProtocol.Serializator;
using ZProtocol.shared;

namespace Client;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Client");

        var client = new ZClient();
        client.OnPacketRecieve += OnPacketReceive;
        client.Connect("127.0.0.1", 4910);

        var message = new ZMessage()
        {
            Message = "string.Empty",
        };
        var response = ZPacketConverter.Serialize(ZPacketType.Handshake ,message).ToPacket();
        client.QueuePacketSend(response);

        Console.ReadLine();

    }

    private static void OnPacketReceive(byte[] packet)
    {
        if (packet == null)
            return;

        var parsed = ZPacket.Parse(packet);

        if (parsed != null)
        {
            ProcessIncomingPacket(parsed);
        }
    }

    private static void ProcessIncomingPacket(ZPacket packet)
    {
        var type = ZPacketTypeManager.GetTypeFromPacket(packet);

        switch (type)
        {
            case ZPacketType.Handshake:
                //ProcessHandshake(packet);
                var message = ZPacketConverter.Deserialize<ZMessage>(packet);
                Console.WriteLine(message.Message);
                break;
            case ZPacketType.Unknown:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}
