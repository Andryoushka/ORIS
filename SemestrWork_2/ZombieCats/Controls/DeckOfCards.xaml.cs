namespace ZombieCats.Controls;

// класс  олоды
public partial class DeckOfCards : ContentView
{
	public DeckOfCards()
	{
		InitializeComponent();
		WidthRequest = _width;
		HeightRequest = _height;
    }

	private double _width = 100;
	private double _height = 150;

	public event EventHandler? DeckTapped; // обработчик нажати€ на колоду
	private bool _canClick = false; // можем ли вз€ть карту?

    private void Deck_Clicked(object sender, EventArgs e)
    {
		var curW = Width;
		var vurH = Height;
        
		// берем карту из колоды - логика в класе GameField
        DeckTapped?.Invoke(button_Deck, EventArgs.Empty);
    }
}