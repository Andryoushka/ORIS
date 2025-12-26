using System.Runtime.InteropServices;

namespace ZProtocol;

public class ZPacket
{
    public byte PacketType { get; private set; }
    public byte PacketSubtype { get; private set; }
    public List<ZPacketField> Fields { get; set; } = new List<ZPacketField>();
    public bool Protected { get; set; }
    private bool ChangeHeaders { get; set; }

    private ZPacket() { }

    public static ZPacket Create(ZPacketType type)
    {
        var t = ZPacketTypeManager.GetType(type);
        return Create(t.Item1, t.Item2);
    }

    public static ZPacket Create(byte type, byte subtype)
    {
        return new ZPacket
        {
            PacketType = type,
            PacketSubtype = subtype
        };
    }

    public ZPacketField GetField(byte id)
    {
        foreach (var field in Fields)
        {
            if (field.FieldID == id)
            {
                return field;
            }
        }

        return null;
    }

    public bool HasField(byte id)
    {
        return GetField(id) != null;
    }

    public void SetValue(byte id, object structure)
    {
        if (!structure.GetType().IsValueType)
        {
            throw new Exception("Only value types are available.");
        }

        var field = GetField(id);

        if (field == null)
        {
            field = new ZPacketField
            {
                FieldID = id
            };

            Fields.Add(field);
        }

        var bytes = FixedObjectToByteArray(structure);

        if (bytes.Length > byte.MaxValue)
        {
            throw new Exception("Object is too big. Max length is 255 bytes.");
        }

        field.FieldSize = (byte)bytes.Length;
        field.Contents = bytes;
    }

    public void SetValueRaw(byte id, byte[] rawData)
    {
        var field = GetField(id);

        if (field == null)
        {
            field = new ZPacketField
            {
                FieldID = id
            };

            Fields.Add(field);
        }

        if (rawData.Length > byte.MaxValue)
        {
            throw new Exception("Object is too big. Max length is 255 bytes.");
        }

        field.FieldSize = (byte)rawData.Length;
        field.Contents = rawData;
    }

    public byte[] ToPacket()
    {
        var packet = new MemoryStream();

        packet.Write(
            ChangeHeaders
                ? new byte[] { 0x95, 0xAA, 0xFF, PacketType, PacketSubtype }
                : new byte[] { 0xAF, 0xAA, 0xAF, PacketType, PacketSubtype }, 0, 5);

        // Сортируем поля по ID
        var fields = Fields.OrderBy(field => field.FieldID);

        // Записываем поля
        foreach (var field in fields)
        {
            packet.Write(new[] { field.FieldID, field.FieldSize }, 0, 2);
            packet.Write(field.Contents, 0, field.Contents.Length);
        }

        // Записываем конец пакета
        packet.Write(new byte[] { 0xFF, 0x00 }, 0, 2);

        return packet.ToArray();
    }

    public static ZPacket Parse(byte[] packet, bool markAsEncrypted = false)
    {
        /*
         * Минимальный размер пакета - 7 байт
         * HEADER (3) + TYPE (1) + SUBTYPE (1) + PACKET ENDING (2)
         */
        if (packet.Length < 7)
        {
            return null;
        }

        var encrypted = false;

        // Проверяем заголовок
        if (packet[0] != 0xAF ||
            packet[1] != 0xAA ||
            packet[2] != 0xAF)
        {
            if (packet[0] == 0x95 ||
                packet[1] == 0xAA ||
                packet[2] == 0xFF)
            {
                encrypted = true;
            }
            else
            {
                return null;
            }
        }

        var mIndex = packet.Length - 1;

        // Проверяем, что бы пакет заканчивался нужными байтами
        if (packet[mIndex - 1] != 0xFF ||
            packet[mIndex] != 0x00)
        {
            return null;
        }

        var type = packet[3];
        var subtype = packet[4];

        var zpacket = new ZPacket { PacketType = type, PacketSubtype = subtype, Protected = markAsEncrypted };

        var fields = packet.Skip(5).ToArray();

        while (true)
        {
            if (fields.Length == 2) // Остались последние два байта, завершающие пакет.
            {
                return encrypted ? DecryptPacket(zpacket) : zpacket;
            }

            var id = fields[0];
            var size = fields[1];

            var contents = size != 0 ?
                fields.Skip(2).Take(size).ToArray() : null;

            zpacket.Fields.Add(new ZPacketField
            {
                FieldID = id,
                FieldSize = size,
                Contents = contents
            });

            fields = fields.Skip(2 + size).ToArray();
        }
    }

    public T GetValue<T>(byte id) where T : struct
    {
        var field = GetField(id);

        if (field == null)
        {
            throw new Exception($"Field with ID {id} wasn't found.");
        }

        var neededSize = Marshal.SizeOf(typeof(T));

        if (field.FieldSize != neededSize)
        {
            throw new Exception($"Can't convert field to type {typeof(T).FullName}.\n" +
                                $"We have {field.FieldSize} bytes but we need exactly {neededSize}.");
        }

        return ByteArrayToFixedObject<T>(field.Contents);
    }

    public byte[] GetValueRaw(byte id)
    {
        var field = GetField(id);

        if (field == null)
        {
            throw new Exception($"Field with ID {id} wasn't found.");
        }

        return field.Contents;
    }

    public byte[] FixedObjectToByteArray(object value)
    {
        var rawsize = Marshal.SizeOf(value);
        var rawdata = new byte[rawsize];

        var handle =
            GCHandle.Alloc(rawdata,
                GCHandleType.Pinned);

        Marshal.StructureToPtr(value,
            handle.AddrOfPinnedObject(),
            false);

        handle.Free();
        return rawdata;
    }

    private T ByteArrayToFixedObject<T>(byte[] bytes) where T : struct
    {
        T structure;

        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

        try
        {
            structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        }
        finally
        {
            handle.Free();
        }

        return structure;
    }

    public ZPacket Encrypt()
    {
        return EncryptPacket(this);
    }

    public static ZPacket EncryptPacket(ZPacket packet)
    {
        if (packet == null)
        {
            return null; // Нам попросту нечего шифровать
        }

        var rawBytes = packet.ToPacket(); // получаем пакет в байтах
        var encrypted = ZProtocolEncryptor.Encrypt(rawBytes); // шифруем его

        var p = Create(0, 0); // создаем пакет
        p.SetValueRaw(0, encrypted); // записываем данные
        p.ChangeHeaders = true; // помечаем, что нам нужен другой заголовок

        return p;
    }

    public ZPacket Decrypt()
    {
        return DecryptPacket(this);
    }

    private static ZPacket DecryptPacket(ZPacket packet)
    {
        if (!packet.HasField(0))
        {
            return null; // Зашифрованные данные должны быть в 0 поле
        }

        var rawData = packet.GetValueRaw(0); // получаем зашифрованный пакет
        var decrypted = ZProtocolEncryptor.Decrypt(rawData);

        return Parse(decrypted, true);
    }
}
