using MiniHttpServer.Framework.Attributes;
using MiniHttpServer.Framework.Core.Abstract;
using MiniHttpServer.Framework.share;
using MiniHttpServer.Models;
using MyORMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniHttpServer.EndPoints;

[Endpoint("event")]
public class EventEndpoint : BaseEndPoint
{
    [HttpGet("page")]
    public IResponseResult EventPage(string index)
    {
        //throw new NotImplementedException();//-----------------------------------//

        var settings = SettingsManager.Instance;
        var orm = new ORMContext(settings.Settings.ConnectionString);

        var data = new Dictionary<string, object>();
        var id = int.Parse(index);

        var tour = orm.ReadById<Models.Tour>(id, "Tour");
        var tourInfo = orm.ReadById<Models.TourInfo>(id, "TourInfo");
        // исправить
        //var tourInfoN = orm.FirstOrDefault<Models.TourInfo>(ti => ti.TourId == tour.Id);
        var tourPage = orm.ReadById<Models.TourPage>(id, "TourPage");
        //исправить
        //var tourProgram = orm.Where<TourProgram>(p => p.TourId == id)
        //    //.GroupBy(p => p.Day)
        //    ;
        var tourProgram = orm.ReadByAll<TourProgram>("TourProgram")
            .Where(x => x.TourId == id)
            .GroupBy(x => x.Day)
            .ToList();

        object tourJoin = new
        {
            Name = tour.Name,
            Price = tour.Price,
            Type = tour.Type,
            Location = tour.Location,
            NightCount = tourInfo.NightCount,
            Description = tourPage.Description,
            Image_0 = tourPage.PageImage_0,
            Image_1 = tourPage.PageImage_1,
            Image_2 = tourPage.PageImage_2,
            Image_3 = tourPage.PageImage_3,
        };

        data.Add("tour", tourJoin);
        data.Add("tourPrograms", tourProgram);

        return Page("Static/Pages/EventPage.html", data);
    }
}
