using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XProtocol.Serializator;

namespace XProtocol.Message
{
    public class XEventMessage
    {
        //[XField(0)]
        //public EventType Type;
        [XField(1)]
        public string PlayerName  = string.Empty;
        [XField(2)]
        public string PlayerId = string.Empty;
        //[XField(3)]
        public List<string> Players;
        [XField(3)]
        public string PlayersString = string.Empty;
        [XField(4)]
        public int X;
        [XField(5)]
        public int Y;
        [XField(6)]
        public string Color = "Red";
    }

}

