using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Enums;

//Здесь хранятся типы карточек, что мы генерируем в колоде
public enum CardType
{
    Nothing,
    Bomb,
    Skip,
    LookIntoDeck
}
