using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Models;

public class Tour
{
    [NotMapped]
    public int Id { get; set; }

    //public string CardImage { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string? Type { get; set; }
    //public string? NearestMetro { get; set; }
    //public decimal Rate { get; set; }
    public decimal Price { get; set; }

    //

    //public int NightCount { get; set; }
    //public string OrganizationType { get; set; } // групповой, индивидуальный, автобус
    //public bool WithFlight { get; set; }
    //public bool WithAccommodation { get; set; } // проживание
    //public bool WithFood { get; set; }
    //public bool WeekendTour { get; set; }
    //public bool LowCost { get; set; }
    //public bool WithKids { get; set; }

    //public string? Description { get; set; }
    //public string? TourProgram { get; set; }
    //public string EventImages { get; set; }
}
