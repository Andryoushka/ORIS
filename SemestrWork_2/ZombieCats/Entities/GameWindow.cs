using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieCats.Entities;

public class GameWindow : Window
{
    public GameWindow() : base()
    {
        
    }

    public GameWindow(Page page) : base(page)
    {
        
    }

    public static event Action GameWindowClosing;
    public static event Action GameWindowCreated;

    protected override void OnDestroying()
    {
        GameWindowClosing?.Invoke();

        base.OnDestroying();
    }

    protected override void OnCreated()
    {
        GameWindowCreated?.Invoke();

        base.OnCreated();
    }
}
