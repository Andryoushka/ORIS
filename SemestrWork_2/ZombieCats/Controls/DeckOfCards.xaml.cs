namespace ZombieCats.Controls;

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

	public event EventHandler? DeckTapped;
	private bool _canClick = false;

    private void Deck_Clicked(object sender, EventArgs e)
    {
		/*
		 * Отправка запроса на получение карты и завершение хода
		 * от определенного игрока
		 * Сервер -> продвигает очередь
		 * 
		 * Визуальная отдача нажатия
		*/

		var curW = Width;
		var vurH = Height;
        
        DeckTapped?.Invoke(button_Deck, EventArgs.Empty);
    }
}