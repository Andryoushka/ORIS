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

    [ZField(1)]
    public int Skip = 0;

    [ZField(2)]
    public int LookIntoDeck = 0;

    public void AddCard(byte type)
    {
        switch (type)
        {
            case 0:
                Nothing++;
                break;

            case 2:
                Skip++;
                break;

            case 3:
                LookIntoDeck++;
                break;

            default:
                break;
        }
    }
}
