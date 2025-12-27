using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZProtocol;

/// <summary>
/// Тип сообщения
/// </summary>
public enum ZPacketType
{
    Unknown,
    Handshake,
    PlayerJoinToGame,
    GetPlayerCount,
    PlayerLeftGame,
    ReadyToPlay,
    StartGame,
    NextTurn,
    DropCard,
    ClearPlayedCards,
    GivePlayerCards,
    Deck_GiveCard,
    PlayerIsDead,
    Winner,
    LookIntoDeck
}

