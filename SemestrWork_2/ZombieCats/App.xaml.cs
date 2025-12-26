using ZombieCats.Entities;

namespace ZombieCats;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new GameWindow(new AppShell());
        //return new Window(new AppShell());
    }
}