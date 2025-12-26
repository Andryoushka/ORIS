using System.ComponentModel;

namespace ZombieCats.Controls;

public partial class GameCard : ContentView
{
	public GameCard()
	{
		InitializeComponent();
        HeightRequest = 150;
        WidthRequest = 100;
	}
    // ќписание , тип, цвет

    public byte Type { get; set; } = 0;


    /*// 1. ќбъ€вл€ем BindableProperty
    public static readonly BindableProperty BindTextProperty =
        BindableProperty.Create(
            propertyName: nameof(BindText),
            returnType: typeof(string),
            declaringType: typeof(TestControl),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay);

    // 2. CLR-обЄртка (опционально, но рекомендуетс€)
    public string BindText
    {
        get => (string)GetValue(BindTextProperty);
        set => SetValue(BindTextProperty, value);
    }*/
}