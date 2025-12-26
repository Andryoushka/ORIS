using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZProtocol.Serializator;

[AttributeUsage(AttributeTargets.Field)]
public class ZFieldAttribute : Attribute
{
    public byte FieldID { get; }

    public ZFieldAttribute(byte fieldId)
    {
        FieldID = fieldId;
    }
}
