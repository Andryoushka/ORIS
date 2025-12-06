using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Models;

public class TourProgram
{
    [NotMapped]
    public int Id { get; set; }

    public int TourId { get; set; }
    public int Day { get; set; }
    public string Task { get; set; }
}

public class TourCard
{
    [NotMapped]
    public int Id { get; set; }
    public int TourId { get; set; }
    public string CardImage { get; set; }
    public string? NearestMetro { get; set; }
    public decimal Rate { get; set; }
    //public string Name { get; set; }
    //public string Location { get; set; }
    //public string? Type { get; set; }
    //public decimal Price { get; set; }

    
}

public class TourPage
{
    [NotMapped]
    public int Id { get; set; }
    public int TourId { get; set; }

    public string? Description { get; set; }
    public string PageImage_0 { get; set; }
    public string PageImage_1 { get; set; }
    public string PageImage_2 { get; set; }
    public string PageImage_3 { get; set; }
}

public class TourInfo
{
    //public List<ProgramDay> Days { get; set; }
    public int TourId { get; set; }

    public string OrganizationType { get; set; } // групповой, индивидуальный, автобус
    public int NightCount { get; set; }
    public bool WithFlight { get; set; }
    public bool WithAccommodation { get; set; } // проживание
    public bool WithFood { get; set; }
    public bool WeekendTour { get; set; }
    public bool LowCost { get; set; }
    public bool WithKids { get; set; }
}

public class ProgramDay
{
    public int Id { get; set; }
    public List<string> Tasks { get; set; }
}