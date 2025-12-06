using MiniHttpServer.Framework.Attributes;
using MiniHttpServer.Framework.Core.Abstract;
using MiniHttpServer.Framework.share;
using MiniHttpServer.Models;
using MiniHttpServer.Services;
using MyORMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiniHttpServer.EndPoints;

[Endpoint("mainpage")]
public class MainEndpoint : BaseEndPoint
{
    [HttpGet()]
    public IResponseResult MainEndpointPage()
    {
        var settings = SettingsManager.Instance;
        var orm = new ORMContext(settings.Settings.ConnectionString);

        var tourCollection = orm.ReadByAll<Tour>("Tour");
        var tourCardCollection = orm.ReadByAll<TourCard>("TourCard");

        var tourJoin = tourCollection.Join(tourCardCollection, t => t.Id, c => c.TourId,
            (t, c) => new
            {
                Id = t.Id,
                Name = t.Name,
                Location = t.Location,
                CardImage = c.CardImage,
                NearestMetro = c.NearestMetro,
                Rate = c.Rate,
                Type = t.Type
            }).ToList();

        var data = new Dictionary<string, object>();
        data.Add("tours", tourJoin);
        //data.Add("toursCard", tourCardCollection);
        data.Add("userName", ((UserService)AuthService).GetUserBySession(Context.Request.Cookies["sessionId"]?.Value));

        return Page("Static/Pages/Main.html", data);
    }

    [HttpPost("registration")]
    public IResponseResult Registration()
    {
        var response = Context.Response;

        var username = FormData["UserLogin"];
        var password = FormData["UserPassword"];

        var auth = (UserService)AuthService;

        if (auth.ValidateUser(username, password) /*&& !auth.IsAuthorized(Context.Request)*/)
        {
            var sessionId = auth.CreateSession(username);

            // Создаем cookie
            //var sessionCookie = request.Cookies["sessionId"];
            var sessionCookie = new Cookie("sessionId", sessionId)
            {
                Expires = DateTime.Now.AddHours(24),
                HttpOnly = true,
                Path = "/"
            };

            response.Cookies.Add(sessionCookie);

            if (auth.IsAdmin(username, password))
                return Redirect("/admin");
        }


        return RefreshPage();
    }

    [HttpPost("redirect")]
    public IResponseResult MainRedirect()
    {
        return Redirect("/FuckGerman");
    }

    [HttpGet("search")]
    public IResponseResult SearchEvent(string template)
    {
        var orm = new ORMContext(SettingsManager.Instance.Settings.ConnectionString);

        var tourCollection = orm.ReadByAll<Tour>("Tour");
        var tourCardCollection = orm.ReadByAll<TourCard>("TourCard");

        var tourJoin = tourCollection.Join(tourCardCollection, t => t.Id, c => c.TourId,
            (t, c) => new
            {
                Id = t.Id,
                Name = t.Name,
                Location = t.Location,
                CardImage = c.CardImage,
                NearestMetro = c.NearestMetro,
                Rate = c.Rate,
                Type = t.Type
            }).ToList();

        var result = tourJoin
            .Where(e => e.Name.Contains(template, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var data = new Dictionary<string, object>();
        data.Add("tours", result);
        data.Add("userName", ((UserService)AuthService).GetUserBySession(Context.Request.Cookies["sessionId"]?.Value));

        return Page("Static/Pages/Main.html", data);
    }

    [HttpPost("filter")]
    public IResponseResult Filter()
    {
        var form = FormData;

        var orm = new ORMContext(SettingsManager.Instance.Settings.ConnectionString);

        var tourCollection = orm.ReadByAll<Tour>("Tour");
        var tourCardCollection = orm.ReadByAll<TourCard>("TourCard");
        var tourInfoCollection = orm.ReadByAll<TourInfo>("TourInfo");
        var filterResult = FilterToursAdvanced(tourInfoCollection.ToList(), FormData);

        var tourJoin = tourCollection.Join(tourCardCollection, t => t.Id, c => c.TourId,
            (t, c) => new
            {
                Tour = t,
                Card = c
            }).Join(filterResult, tc => tc.Tour.Id, i => i.TourId,
            (tc,i) => new
            {
                Id = tc.Tour.Id,
                Name = tc.Tour.Name,
                Location = tc.Tour.Location,
                CardImage = tc.Card.CardImage,
                NearestMetro = tc.Card.NearestMetro,
                Rate = tc.Card.Rate,
                Type = tc.Tour.Type,
                NightCount = i.NightCount,
                OrganizationType = i.OrganizationType,
                WithFlight = i.WithFlight,
                WithAccommodation = i.WithAccommodation,
                WithFood = i.WithFood,
                WeekendTour = i.WeekendTour,
                LowCost = i.LowCost,
                WithKids = i.WithKids
            })
            .ToList();

        var data = new Dictionary<string, object>();
        data.Add("tours", tourJoin);
        data.Add("userName", ((UserService)AuthService).GetUserBySession(Context.Request.Cookies["sessionId"]?.Value));

        return Page("Static/Pages/Main.html", data);
    }

    [HttpPost("admin")]
    public IResponseResult AdminPage()
    {
        return Redirect("/admin");
    }

    [HttpGet("showevent")]
    public IResponseResult ShowEventPage(string index)
    {
        return Redirect($"/event/page?index={index}");
    }

    private static List<TourInfo> FilterToursAdvanced(List<TourInfo> tours, Dictionary<string, string> formData)
    {
        var query = tours.AsQueryable();

        foreach (var filter in formData)
        {
            switch (filter.Key)
            {
                case "NightCount":
                    var nightValues = filter.Value.Split(',')
                        .Select(v => int.Parse(v.Trim()))
                        .ToList();
                    query = query.Where(t => nightValues.Contains(t.NightCount));
                    break;

                case "OrganizationType":
                    var orgValues = filter.Value.Split(',')
                        .Select(v => v.Trim())
                        .ToList();
                    query = query.Where(t => orgValues.Contains(t.OrganizationType));
                    break;

                case "WithFlight":
                    if ((bool)GetValue(formData, filter.Key, typeof(bool)))
                        query = query.Where(t => t.WithFlight);
                    break;

                case "WithAccommodation":
                    if ((bool)GetValue(formData, filter.Key, typeof(bool)))
                        query = query.Where(t => t.WithAccommodation);
                    break;

                case "WithFood":
                    if ((bool)GetValue(formData, filter.Key, typeof(bool)))
                        query = query.Where(t => t.WithFood);
                    break;

                case "WeekendTour":
                    if ((bool)GetValue(formData, filter.Key, typeof(bool)))
                        query = query.Where(t => t.WeekendTour);
                    break;

                case "LowCost":
                    if ((bool)GetValue(formData, filter.Key, typeof(bool)))
                        query = query.Where(t => t.LowCost);
                    break;

                case "WithKids":
                    if ((bool)GetValue(formData, filter.Key, typeof(bool)))
                        query = query.Where(t => t.WithKids);
                    break;
            }
        }

        return query.ToList();
    }

    public static object GetValue(Dictionary<string, string> formData, string key, Type type)
    {
        if (!formData.ContainsKey(key) || string.IsNullOrEmpty(formData[key]))
        {
            // Возвращаем default значение для типа
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        string value = formData[key];
        try
        {
            if ((value == "on" || value == "off") && type.Name == "Boolean")
            {
                value = value == "on" ? "true" : "false";
            }
            if (type.Name == "Decimal")
                value = value.Replace(".", ",");

            return Convert.ChangeType(value, type);
        }
        catch
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }

}
