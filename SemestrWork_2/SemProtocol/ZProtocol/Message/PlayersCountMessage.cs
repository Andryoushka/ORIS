using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZProtocol.Serializator;

namespace ZProtocol.Message;

public class PlayersCountMessage
{
    [ZField(0)]
    public int PlayersCount = 1;
}
