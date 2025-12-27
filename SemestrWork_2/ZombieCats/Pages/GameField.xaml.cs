using GameServer.Enums;
using System.Collections.ObjectModel;
using ZombieCats.Abstract;
using ZombieCats.Controls;
using ZProtocol;
using ZProtocol.Enums;
using ZProtocol.Message;
using ZProtocol.Serializator;

namespace ZombieCats;

// класс »грового ѕол€
public partial class GameField : ContentPage
{
	public GameField(ZClient player)
	{
		_player = player; // клиент - игрок

		Arm = new ObservableCollection<GameCard>(); // рука игрока
        Players = new ObservableCollection<PlayerView>(); // список игроков
        PlayedCards = new ObservableCollection<GameCard>(); // использованные карты

        BindingContext = this;
		InitializeComponent();
		_player.OnPacketRecieve += PlayerActions; // обрабатываем всю логику вход€щих пакетов
		Deck.DeckTapped += (s, e) => // логика при вз€тии карты с колоды (на нажатие колоды)
		{
            if (!_canMakeAction)
                return;

            // запрос серверу на карту
            _player.QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.Deck_GiveCard,
                        new ZMessage { }).ToPacket());
            _canMakeAction = false;
        };
        // устанавливаем цвета дл€ кнопок использовани€ карт
        Use_Button.BackgroundColor = DISABLE_BUTTON;
        Exchange_Button.BackgroundColor = DISABLE_BUTTON;
        ExchangeDefinite_Button.BackgroundColor = DISABLE_BUTTON;
    }

    private ZClient _player;
    private bool _canMakeAction = false; // блокирует руку дл€ действий с картами
    private bool _readyToPlay = false; // отвечает за темный экран игры
    private int _playerIndex = 0; // индекс игрока
    private bool _playerIsDead = false;
    private int Index = 0; // не важно
    private readonly Color ENABLE_BUTTON = Color.FromArgb("#c0dc87"); // цвет активированной карты
    private readonly Color DISABLE_BUTTON = Color.FromArgb("#3ba273");// цвет не активированной карты

    public ObservableCollection<PlayerView> Players {  get; set; }
    public ObservableCollection<GameCard> PlayedCards { get; set; }
    public ObservableCollection<GameCard> Arm { get; set; }

    private void PlayerActions(byte[] obj)
    {
        //обрабатываем пакеты с информацией и в зависимости от типа сообщени€ реагируем на них
        var packet = ZPacket.Parse(obj);
        if (packet == null)
            return;
        var packetType = ZPacketTypeManager.GetTypeFromPacket(packet);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            switch (packetType)
            {
                case ZPacketType.Unknown:
                    break;
                case ZPacketType.Handshake:
                    break;
                case ZPacketType.PlayerJoinToGame:
                    break;
                case ZPacketType.GetPlayerCount:
                    break;
                case ZPacketType.PlayerLeftGame:
                    break;
                case ZPacketType.ReadyToPlay:
                    break;

                case ZPacketType.StartGame:
                    _readyToPlay = true;
                
                        GameShield.IsVisible = false;

                        var startMessage = ZPacketConverter.Deserialize<ZMessage>(packet);
                        var playersList = startMessage.Message.Split('/');
                        foreach (var playerName in playersList)
                        {
                            Players.Add(new PlayerView() { PlayerName = playerName });
                        }
                        _playerIndex = startMessage.PlayerId;
                
                    break;

                case ZPacketType.NextTurn:
                    //if (_playerIsDead)
                    //    return;
                    var turnMessage = ZPacketConverter.Deserialize<ZMessage>(packet);
                    Players[turnMessage.PlayerId].ChangeColor(Brush.Green);

                    if (turnMessage.PlayerId == _playerIndex)
                    {
                        _canMakeAction = true;
                        DisplayAlert("”ведомление", "“вой ход", "OK");
                    }
                    break;

                case ZPacketType.DropCard:
                    var dropMessage = ZPacketConverter.Deserialize<ZMessage>(packet);
                    PlayedCards.Add(new GameCard(dropMessage.CardType));
                break;

                case ZPacketType.ClearPlayedCards:
                    var clearMessage = ZPacketConverter.Deserialize<ZMessage>(packet);
                    Players[clearMessage.PlayerId].ChangeColor(Brush.Grey);
                    PlayedCards.Clear();
                break;

                case ZPacketType.GivePlayerCards:
                    var armMessage = ZPacketConverter.Deserialize<PlayerArm>(packet);
                    var cardType = armMessage.GetType().GetFields();
                    foreach (var ct in cardType)
                    {
                        switch (ct.Name)
                        {
                            case "Nothing":
                                for (int i = 0; i < (int)ct.GetValue(armMessage); i++)
                                    Arm.Add(new GameCard((byte)CardType.Nothing));
                                break;

                            case "Skip":
                                for (int i = 0; i < (int)ct.GetValue(armMessage); i++)
                                    Arm.Add(new GameCard((byte)CardType.Skip));
                                break;

                            case "LookIntoDeck":
                                for (int i = 0; i < (int)ct.GetValue(armMessage); i++)
                                    Arm.Add(new GameCard((byte)CardType.LookIntoDeck));
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case ZPacketType.Deck_GiveCard:
                    var givenCard = ZPacketConverter.Deserialize<ZMessage>(packet);
                    if (givenCard.CardType == 1)
                    {
                        // отправка сообщени€ всем что игрок умер
                        _playerIsDead = true;
                        _player.QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.PlayerIsDead,
                                new ZMessage { PlayerId = _playerIndex}).ToPacket());
                        DisplayAlert("”ведомление", "¬ы взорвались!!", "OK");
                    }
                    else
                    {
                        PlayedCards.Clear();
                        Arm.Add(new GameCard(givenCard .CardType));
                    }

                    _player.QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.NextTurn,
                                new ZMessage { }).ToPacket());
                    break;

                case ZPacketType.PlayerIsDead:
                    var deadPlayer = ZPacketConverter.Deserialize<ZMessage>(packet);
                    Players[deadPlayer.PlayerId].ChangeColor(Brush.Red);
                    Players[deadPlayer.PlayerId].Dead();
                    break;

                case ZPacketType.Winner:
                    var winner = ZPacketConverter.Deserialize<ZMessage>(packet);
                    DisplayAlert("”ведомление", $"»грок {winner.Message} - победил!", "OK");
                    break;

                case ZPacketType.LookIntoDeck:
                    var cardsInDeck = ZPacketConverter.Deserialize<ZMessage>(packet);
                    var cards = string.Join(',', cardsInDeck.Message.Split('/').Select(c => CardInformer.GetCardInfo(c).Name));
                    DisplayAlert("”ведомление", $"“ри верхние карты в колоде: {cards}", "OK");
                    break;

                default:
                    break;
            }
        });
    }
    protected override void OnAppearing()
    {
        // событие при загрузки элементов игрового пол€ (игра загрузилась и игрок готов начать)
        base.OnAppearing();

		var message = new ZMessage()
		{
			Message = string.Empty
		};
        var packet = ZPacketConverter.Serialize(ZPacketType.ReadyToPlay, message).ToPacket();
        _player.QueuePacketSend(packet); // готов к игре
    }

    private void UseCard(object sender, EventArgs e)
    {
        // используем карту
        if (!_canMakeAction)
            return;
        var arm = Arm_Collection.SelectedItems.Cast<GameCard>();
        if (arm.Count() != 1)
            return;
        // выкидываем на поле
        var card = arm.FirstOrDefault<GameCard>();
        PlayedCards.Add(card);
        Arm.Remove(card);
        // выкидываем на поле дл€ всех
        var message = new ZMessage()
        {
            CardType = card.Type
        };
        var packet = ZPacketConverter.Serialize(ZPacketType.DropCard, message).ToPacket();
        _player.QueuePacketSend(packet);

        // активируетс€ действие карты
        switch (card.Type)
        {
            case 2:
                //пропускаем ход
                _canMakeAction = false;
                Thread.Sleep(1000);
                _player.QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.NextTurn,
                                new ZMessage { }).ToPacket());
                break;

            case 3:
                // смотрим 3 верхние карты в колоде
                _player.QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.LookIntoDeck,
                                new ZMessage { }).ToPacket());
                break;

            default:
                break;
        }
    }
    private void ExchangeCard(object sender, EventArgs e)
    {
        // здесь должен быть обмен двух карт
        if (!_canMakeAction)
            return;
    }
    private void ExchangeDefiniteCard(object sender, EventArgs e)
    {
        // здесь должен быть обмен трех карт
        if (!_canMakeAction)
            return;
    }

    private void Arm_Collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // обработчик выбранного количества карт в руке
        var select = e.CurrentSelection;

        switch (select.Count)
        {
            case 1:
                Use_Button.BackgroundColor = ENABLE_BUTTON;
                Exchange_Button.BackgroundColor = DISABLE_BUTTON;
                ExchangeDefinite_Button.BackgroundColor = DISABLE_BUTTON;
                break;

            case 2:
                Exchange_Button.BackgroundColor = ENABLE_BUTTON;
                Use_Button.BackgroundColor = DISABLE_BUTTON;
                ExchangeDefinite_Button.BackgroundColor = DISABLE_BUTTON;
                break;

            case 3:
                ExchangeDefinite_Button.BackgroundColor = ENABLE_BUTTON;
                Use_Button.BackgroundColor = DISABLE_BUTTON;
                Exchange_Button.BackgroundColor = DISABLE_BUTTON;
                break;

            default:
                Use_Button.BackgroundColor = DISABLE_BUTTON;
                Exchange_Button.BackgroundColor = DISABLE_BUTTON;
                ExchangeDefinite_Button.BackgroundColor = DISABLE_BUTTON;
                break;
        }
    }
}