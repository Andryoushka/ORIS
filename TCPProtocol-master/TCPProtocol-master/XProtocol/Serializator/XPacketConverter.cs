using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XProtocol.Serializator
{
    public class XPacketConverter
    {
        public static XPacket Serialize(XPacketType type, object obj, bool strict = false)
        {
            var t = XPacketTypeManager.GetType(type);
            return Serialize(t.Item1, t.Item2, obj, strict);
        }

        public static XPacket Serialize(byte type, byte subtype, object obj, bool strict = false)
        {
            var fields = GetFields(obj.GetType());

            if (strict)
            {
                var usedUp = new List<byte>();

                foreach (var field in fields)
                {
                    if (usedUp.Contains(field.Item2))
                    {
                        throw new Exception("One field used two times.");
                    }

                    usedUp.Add(field.Item2);
                }
            }

            var packet = XPacket.Create(type, subtype);

            foreach (var field in fields)
            {
                if (field.Item1.FieldType.Name.ToLower() == "string")
                {
                    var value = (string)field.Item1.GetValue(obj);
                    packet.SetValueRaw(field.Item2, Encoding.UTF8.GetBytes(value));
                }
                else
                    packet.SetValue(field.Item2, field.Item1.GetValue(obj));
            }

            return packet;
        }

        public static T Deserialize<T>(XPacket packet, bool strict = false)
        {
            var fields = GetFields(typeof(T));
            var instance = Activator.CreateInstance<T>();

            if (fields.Count == 0)
            {
                return instance;
            }

            foreach (var tuple in fields)
            {
                var field = tuple.Item1;
                var packetFieldId = tuple.Item2;

                if (!packet.HasField(packetFieldId))
                {
                    if (strict)
                    {
                        throw new Exception($"Couldn't get field[{packetFieldId}] for {field.Name}");
                    }

                    continue;
                }

                bool isNotString = field.FieldType.Name.ToLower() != "string";
                var value = isNotString ?
                    typeof(XPacket)
                    .GetMethod("GetValue")?
                    .MakeGenericMethod(field.FieldType)
                    .Invoke(packet, new object[] { packetFieldId })
                    :
                    typeof(XPacket)
                    .GetMethod("GetValueRaw")?
                    .Invoke(packet, new object[] { packetFieldId });

                if (value == null)
                {
                    if (strict)
                    {
                        throw new Exception($"Couldn't get value for field[{packetFieldId}] for {field.Name}");
                    }

                    continue;
                }

                if (isNotString)
                    field.SetValue(instance, value);
                else
                    field.SetValue(instance, Encoding.UTF8.GetString((byte[])value));

            }

            return instance;
        }

        private static List<Tuple<FieldInfo, byte>> GetFields(Type t)
        {
            return t.GetFields(BindingFlags.Instance |
                                     BindingFlags.NonPublic |
                                     BindingFlags.Public)
                .Where(field => field.GetCustomAttribute<XFieldAttribute>() != null)
                .Select(field => Tuple.Create(field, field.GetCustomAttribute<XFieldAttribute>().FieldID))
                .ToList();
        }
    }
}
