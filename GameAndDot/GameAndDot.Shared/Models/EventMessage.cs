using GameAndDot.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAndDot.Shared.Models;

public class EventMessage
{
    public EventType Type { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public List<string> Players { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public string Color { get; set; } = "Red";
}
