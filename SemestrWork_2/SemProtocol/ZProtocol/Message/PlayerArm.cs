using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZProtocol.Serializator;

namespace ZProtocol.Message;

public class PlayerArm
{
    [ZField(0)]
    public int Nothing = 0;

    public void AddCard(byte type)
    {
        switch (type)
        {
            case 0:
                Nothing++;
                break;

            default:
                break;
        }
    }
}
