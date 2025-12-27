using System.ComponentModel;

namespace ZombieCats.Controls;

// класс Карты
public partial class GameCard : ContentView, INotifyPropertyChanged
{
	public GameCard()
	{
		InitializeComponent();
        HeightRequest = 150;
        WidthRequest = 100;
	}

    public GameCard(byte type)
    {
        BindingContext = this;
		InitializeComponent();
        HeightRequest = 150;
        WidthRequest = 100;

        //в зависимости от типа карты - назначаем ему изображение
        switch (type)
        {
            case 0: // nothing
                CardImageSource = "card_0.jpg";
                break;

            case 2: // skip
                CardImageSource = "card_2.jpg";
                break;

            case 3: // look
                CardImageSource = "card_3.jpg";
                break;

            default:
                break;
        }

        Type = type;
    }

    public byte Type { get; set; } = 0;
    private string _image = "card_0.jpg";
    public event PropertyChangedEventHandler PropertyChanged;
    public string CardImageSource
    {
        get { return _image; }
        set 
        { 
            _image = value;
            PropertyChanged?.Invoke(this , new PropertyChangedEventArgs("CardImageSource"));
        }
    }
}