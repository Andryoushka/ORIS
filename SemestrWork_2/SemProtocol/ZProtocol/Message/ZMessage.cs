using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZProtocol.Serializator;

namespace ZProtocol.Message;

public class ZMessage
{
    [ZField(0)]
    public string Message = string.Empty;

    [ZField(1)]
    public int PlayerId = 0;

    [ZField(2)]
    public byte CardType = 0;

    [ZField(3)]
    public bool PlayerIsAlive = true;
}
