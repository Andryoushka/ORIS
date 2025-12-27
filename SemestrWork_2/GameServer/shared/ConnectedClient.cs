using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZProtocol;
using ZProtocol.Enums;
using ZProtocol.Message;
using ZProtocol.Serializator;

namespace GameServer.shared;

public class ConnectedClient
{
/*
 * Очередь игроков на сервере
 * Колода, сброс, "руки" игроков
 * 
 * Действия на сервере
 * Менеджер действий - словарь
 * 
 * Типы карт:
 * Взрывной кот (4) - смерть
 * 1. Зомби кот (5) - бомбу в колоду, оживи игрока
 * 2. Пропусти всё (2) - заверши ход
 * 3. Атака мертвецов (3) - пропусти ход, след игрок берет карты = количесвту мертвецов
 * 4. Ясновидение (2) - посмотри куда игрок спрятал бомбу после Зомби кота
 * 5. Корми мертвеца (2) - другие отдают 1 карту мертвецу
 * 6. Нападай (2) - заверши ход не бери карту, след игрок ходит дважды
 * 7. Неть (5) - отмени действие игрока
 * 8. Заглянуть в будущее (4) - посмотри три верхние карты с колоды
 * 9. Клонирование (3) - становится копией карты под ней
 * 10. Копни глубже (4) - бери 1 , не понравилась - бери вторую, но первую назад
 * 11. Обезкотленный (16) - ничего
 * 12. Одолжение (3) - забери 1 карту у игрока по его выбору
 * 13. Перемешать (2)
 * 14. Пропустить (3) - закончи ход не бери карты
 * 15. Расхититель гробниц (1) - мертвецы выбрать по 1 карте в колоду рандомно
 */
    protected string UserName { get; set; } = string.Empty;
    protected string Id { get; } = Guid.NewGuid().ToString();
    protected List<object> Hand = new List<object>(); // рука
    protected bool IsDead { get; set; } = false;
    protected bool IsReady { get; set; } = false;
    //protected bool OwnTurn { get; set; } = false;

    private ZServer _server { get; set; }
    public Socket Client { get; }

    private readonly Queue<Tuple<byte[], SendType>> _packetSendingQueue 
        = new Queue<Tuple<byte[], SendType>>();

    public ConnectedClient(Socket client, ZServer server)
    {
        Client = client;
        _server = server;
        Task.Run((Action)ProcessIncomingPackets);
        Task.Run((Action)SendPackets);
    }

    private void ProcessIncomingPackets()
    {
        while (true) // Слушаем пакеты, пока клиент не отключится.
        {
            try
            {
                var buff = new byte[256]; // Максимальный размер пакета - 256 байт.
                Client.Receive(buff);

                buff = buff.TakeWhile((b, i) =>
                {
                    if (b != 0xFF) return true;
                    return buff[i + 1] != 0;
                }).Concat(new byte[] { 0xFF, 0 }).ToArray();

                var parsed = ZPacket.Parse(buff);

                if (parsed != null)
                {
                    ProcessIncomingPacket(parsed);
                }
            }
            catch
            {
                /*_server.Clients.Remove(this);
                var LeftMessage = new PlayersCountMessage()
                {
                    PlayersCount = _server.Clients.Count,
                };
                var Left = ZPacketConverter.Serialize(ZPacketType.GetPlayerCount, LeftMessage).ToPacket();
                QueuePacketSend(Left, SendType.ToOther);
                Client.Close();
                Client.Dispose();*/
            }
        }
    }

    private void ProcessIncomingPacket(ZPacket packet)
    {
        var type = ZPacketTypeManager.GetTypeFromPacket(packet);

        switch (type)
        {
            case ZPacketType.Handshake:
                //ProcessHandshake(packet);
                break;

            case ZPacketType.Unknown:
                break;

            case ZPacketType.GetPlayerCount:
                var joinMessage = new PlayersCountMessage()
                {
                    PlayersCount = _server.Clients.Count,
                };
                var joinPacket = ZPacketConverter.Serialize(ZPacketType.GetPlayerCount, joinMessage).ToPacket();
                QueuePacketSend(joinPacket, SendType.ToAll);
                break;

            case ZPacketType.PlayerJoinToGame:
                var message = ZPacketConverter.Deserialize<PlayerJoinMessage>(packet);
                if (!string.IsNullOrEmpty(message.PlayerName))
                    UserName = message.PlayerName;
                break;

            case ZPacketType.PlayerLeftGame:
                lock (_server.ClientsLock)
                {
                    _server.Clients.Remove(this);
                }
                var LeftMessage = new PlayersCountMessage()
                {
                    PlayersCount = _server.Clients.Count,
                };
                var Left = ZPacketConverter.Serialize(ZPacketType.GetPlayerCount, LeftMessage).ToPacket();
                QueuePacketSend(Left, SendType.ToOther);
                Client.Close();
                Client.Dispose();
                break;

            case ZPacketType.ReadyToPlay:
                //var readyMessage = // Принимает сообщение
                IsReady = true;
                if (_server.Clients.All(c => c.IsReady))
                {
                    // создать колоду по количеству игроков
                    var playersCount = _server.Clients.Count;
                    _server.CardDeck.Clear();
                    for (int i = 0; i < 8; i++)
                        _server.CardDeck.Add(Enums.CardType.Nothing);
                    for (int i = 0; i < 3; i++)
                        _server.CardDeck.Add(Enums.CardType.Skip);
                    for (int i = 0; i < 4; i++)
                        _server.CardDeck.Add(Enums.CardType.LookIntoDeck);

                    // тасуем и раздаём игрокам по 7 карт
                    Shuffle(_server.CardDeck);
                    var playersArm = new List<PlayerArm>();
                    for (int i = 0; i < playersCount; i++)
                        playersArm.Add(new PlayerArm());

                    for (int i = 0; i < playersCount * 7; i++)
                    {
                        var playerId = i % playersCount;
                        playersArm[playerId].AddCard((byte)_server.CardDeck[0]);
                        _server.CardDeck.RemoveAt(0);
                    }

                    // отправляем карты игроку
                    for (int i = 0; i < _server.Clients.Count; i++)
                    {
                        _server.Clients[i].Client.Send(ZPacketConverter.Serialize(ZPacketType.GivePlayerCards,
                        playersArm[i]).ToPacket());
                    }

                    // добавляем бомбы и тасуем ещё раз
                    for (int i = 0; i < playersCount - 1; i++)
                        _server.CardDeck.Add(Enums.CardType.Bomb);
                    Shuffle(_server.CardDeck);

                    // начинаем игру (отослать список игроков)
                    var playersList = string.Join('/', _server.Clients.Select(c => c.UserName));

                    for(int i = 0; i< _server.Clients.Count; i++)
                    {
                        _server.Clients[i].Client.Send(ZPacketConverter.Serialize(ZPacketType.StartGame,
                        new ZMessage { Message = playersList, PlayerId = i }).ToPacket());
                    }

                    QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.NextTurn,
                        new ZMessage { PlayerId = 0 }).ToPacket(),
                         SendType.ToAll);
                }
                break;

            case ZPacketType.NextTurn:
                var curIndex = _server.Clients.IndexOf(this);
                QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.ClearPlayedCards,
                        new ZMessage { PlayerId = curIndex }).ToPacket()
                        , SendType.ToAll);

                var nextPlayer = (curIndex + 1) % _server.Clients.Count;
                while (_server.Clients[nextPlayer].IsDead)
                {
                    nextPlayer = (nextPlayer + 1) % _server.Clients.Count;
                }

                QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.NextTurn,
                        new ZMessage { PlayerId = nextPlayer }).ToPacket(),
                            SendType.ToAll);
                break;

            case ZPacketType.DropCard:
                var dropMessage = ZPacketConverter.Deserialize<ZMessage>(packet);
                QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.DropCard,
                        new ZMessage { CardType = dropMessage.CardType }).ToPacket()
                        , SendType.ToOther);
                break;

            case ZPacketType.Deck_GiveCard:
                if (_server.CardDeck.Count == 0)
                    return;
                var cardType = _server.CardDeck.First();
                _server.CardDeck.RemoveAt(0);
                QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.Deck_GiveCard,
                        new ZMessage { CardType = (byte)cardType }).ToPacket()
                        , SendType.ToClient);
                break;

            case ZPacketType.PlayerIsDead:
                IsDead = true;
                QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.PlayerIsDead,
                        new ZMessage { PlayerId = _server.Clients.IndexOf(this) }).ToPacket()
                        , SendType.ToAll);

                var alivePlayers = _server.Clients.Where(c => c.IsDead == false);
                if (alivePlayers.Count() == 1)
                {
                    var winner = alivePlayers.First();
                    QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.Winner,
                        new ZMessage { PlayerId = _server.Clients.IndexOf(winner), Message = winner.UserName }).ToPacket()
                        , SendType.ToAll);
                }
                break;

            case ZPacketType.LookIntoDeck:
                var cards = string.Join( '/',_server.CardDeck.Take(3));
                QueuePacketSend(ZPacketConverter.Serialize(ZPacketType.LookIntoDeck,
                new ZMessage { Message = cards }).ToPacket()
                , SendType.ToClient);
                break;

            default:
                break;
        }
    }

    public void QueuePacketSend(byte[] packet, SendType sendType = SendType.ToClient)
    {
        if (packet.Length > 256)
        {
            throw new Exception("Max packet size is 256 bytes.");
        }

        _packetSendingQueue.Enqueue(Tuple.Create(packet, sendType));
    }

    private void SendPackets()
    {
        while (true)
        {
            if (_packetSendingQueue.Count == 0)
            {
                //Thread.Sleep(100);
                continue;
            }

            var packet = _packetSendingQueue.Dequeue();

            switch (packet.Item2)
            {
                case SendType.ToAll:
                    foreach (var client in _server.Clients)
                    {
                        client.Client.Send(packet.Item1);
                    }
                    break;
                case SendType.ToOther:
                    foreach (var client in _server.Clients)
                    {
                        if (client.Id != Id)
                            client.Client.Send(packet.Item1);
                    }
                    break;
                case SendType.ToClient:
                    Client.Send(packet.Item1);
                    break;
                default:
                    break;
            }

            //Thread.Sleep(100);
        }
    }

    public static void Shuffle<T>(IList<T> list)
    {
        var rng = new Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
