using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieCats.Entities;

// класс Игрового Окна
public class GameWindow : Window
{
    public GameWindow() : base()
    {
        
    }

    public GameWindow(Page page) : base(page)
    {
        //Width = 1920;
        //Height = 1080;
    }

    public static event Action GameWindowClosing; // событие при закрытии игры
    public static event Action GameWindowCreated; // событие при запуске игры

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
