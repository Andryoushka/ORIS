using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZProtocol.Serializator;

namespace ZProtocol.Message;

public class PlayerJoinMessage
{
    [ZField(0)]
    public int MyProperty;

    [ZField(1)]
    public string PlayerName = string.Empty;
}
