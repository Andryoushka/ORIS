using GameServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer.Enums;

namespace ZombieCats.Abstract;
//здесь находятся классы для упрощения работы с информации о доступных картах
public class CardInformer
{
    public static CardInfo GetCardInfo(string cardType)
    {
        var type = Enum.Parse<CardType>(cardType);
        if (!_cards.ContainsKey(type))
            return null;

        return _cards[type];
    }

    private static Dictionary<CardType, CardInfo> _cards = new Dictionary<CardType, CardInfo>()
    {
        { CardType.Nothing, new CardInfo { Name = "Обычная" } },
        { CardType.Bomb, new CardInfo { Name = "Взрывная" } },
        { CardType.Skip, new CardInfo { Name = "Пропуск хода" } },
        { CardType.LookIntoDeck, new CardInfo { Name = "Подсмотреть 3 карты в колоде" } },
    };
}

public class CardInfo
{
    public string Name { get; set; }
}