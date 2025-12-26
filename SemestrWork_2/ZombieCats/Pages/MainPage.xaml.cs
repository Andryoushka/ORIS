using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Networking.NetworkOperators;
using ZombieCats.Controls;
using ZombieCats.Entities;
using ZProtocol;
using ZProtocol.Message;
using ZProtocol.Serializator;

namespace ZombieCats;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    public MainPage()
    {
        _player = new ZClient();
        _player.OnPacketRecieve += MainActions;
        _player.Connect("127.0.0.1", 4910);
        
        BindingContext = this;
        InitializeComponent();
        //Loaded += LoadMethod;
        GameWindow.GameWindowClosing += GameWindow_GameWindowClosing;

        var message = new PlayersCountMessage()
        {
            PlayersCount = 1,
        };
        var packet = ZPacketConverter.Serialize(ZPacketType.GetPlayerCount, message).ToPacket();
        _player.QueuePacketSend(packet);
    }

    private void GameWindow_GameWindowClosing()
    {
        var message = new PlayerJoinMessage();
        var packet = ZPacketConverter.Serialize(ZPacketType.PlayerLeftGame, message).ToPacket();
        _player.QueuePacketSend(packet);

        _player.OnPacketRecieve = null;
        _player.CloseConnection();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private ZClient _player;
    private int _playersCountToBegin = 2;

    #region PROPERTIES
        private bool _canBeginGame = false;
        public bool CanBeginGame
        {
            get => _canBeginGame;
            set
            {
                _canBeginGame = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanBeginGame"));
            }
        }

        private int _playersCount = 1;
        public int PlayersCount
        {
            get => _playersCount;
            set
            {
                _playersCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlayersCount"));
            }
        }

        private string _playerName = string.Empty;
        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlayerName"));
            }
        }
    #endregion

    private void MainActions(byte[] obj)
    {
        var packet = ZPacket.Parse(obj);
        if (packet == null)
            return;
        var packetType = ZPacketTypeManager.GetTypeFromPacket(packet);

        switch (packetType)
        {
            case ZPacketType.GetPlayerCount:
                var message = ZPacketConverter.Deserialize<PlayersCountMessage>(packet);
                PlayersCount = message.PlayersCount;
                CanBeginGame = PlayersCount == _playersCountToBegin ? true : false;
                break;

            default:
                break;
        }
    }

    private async void StartGame(object sender, EventArgs e)
    {
        if (PlayersCount == _playersCountToBegin)
        {
            // назначить имя игрока
            var message = new PlayerJoinMessage()
            {
                PlayerName = EntryPlayerName.Text,
            };
            var packet = ZPacketConverter.Serialize(ZPacketType.PlayerJoinToGame, message).ToPacket();
            _player.QueuePacketSend(packet);

            _player.OnPacketRecieve = null;
            await Navigation.PushAsync(new GameField(_player), false);
        }
    }

}