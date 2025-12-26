using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZProtocol;

public static class ZPacketTypeManager
{
    private static readonly Dictionary<ZPacketType, Tuple<byte, byte>> TypeDictionary =
        new Dictionary<ZPacketType, Tuple<byte, byte>>();

    static ZPacketTypeManager()
    {
        var types = Enum.GetValues(typeof(ZPacketType)).Cast<ZPacketType>().ToArray();

        for (byte i = 0; i < types.Length; i++)
        {
            RegisterType(types[i], i, 0);
        }
    }

    public static void RegisterType(ZPacketType type, byte btype, byte bsubtype)
    {
        if (TypeDictionary.ContainsKey(type))
        {
            throw new Exception($"Packet type {type:G} is already registered.");
        }

        TypeDictionary.Add(type, Tuple.Create(btype, bsubtype));
    }

    public static Tuple<byte, byte> GetType(ZPacketType type)
    {
        if (!TypeDictionary.ContainsKey(type))
        {
            throw new Exception($"Packet type {type:G} is not registered.");
        }

        return TypeDictionary[type];
    }

    public static ZPacketType GetTypeFromPacket(ZPacket packet)
    {
        var type = packet.PacketType;
        var subtype = packet.PacketSubtype;

        foreach (var tuple in TypeDictionary)
        {
            var value = tuple.Value;

            if (value.Item1 == type && value.Item2 == subtype)
            {
                return tuple.Key;
            }
        }

        return ZPacketType.Unknown;
    }
}
