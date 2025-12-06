using MiniHttpServer.Framework.Attributes;
using MiniHttpServer.Framework.Core.Abstract;
using MiniHttpServer.Framework.share;
using MiniHttpServer.Models;
using MyORMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniHttpServer.EndPoints;

[Endpoint("admin")]
public class AdminEndpoint : BaseEndPoint
{
    [HttpGet]
    public IResponseResult AdminPage()
    {
        return Page(@"Static/Pages/Admin/Admin_EventPage.html", null);
    }

    [HttpGet("back-to-menu")]
    public IResponseResult BackToMainMenu()
    {
        return Redirect("/mainpage");
    }

    [HttpPost("create")]
    public IResponseResult CreateTour()
    {
        // Имя свойств должны совпадать с данными формы

        var tour = new Tour();
        SetEntity(tour);
        var tourCard = new TourCard();
        SetEntity(tourCard);
        var tourInfo = new TourInfo();
        SetEntity(tourInfo);
        var tourPage = new TourPage();
        SetEntity(tourPage);
        //var tourProgram = new TourProgram();
        //SetEntity(tourProgram);

        var tourProgramCollection = JsonSerializer.Deserialize<List<ProgramDay>>(FormData["TourProgram"]);

        /*foreach (var prop in tourProps)
        {
            if (!FormData.ContainsKey(prop.Name))
            {
                if (prop.Name != "Id")
                {
                    prop.SetValue(tour, string.Empty);
                    continue;
                }
            }
            prop.SetValue(tour, GetValue(FormData, prop.Name, prop.PropertyType));
        }*/

        var orm = new ORMContext(SettingsManager.Instance.Settings.ConnectionString);
        orm.AddToTable<Tour>(tour, "Tour");
        var tourId = orm.GetCurrentId("Tour");
        tourCard.TourId = tourId;
        tourInfo.TourId = tourId;
        tourPage.TourId = tourId;
        orm.AddToTable<TourCard>(tourCard, "TourCard");
        orm.AddToTable<TourInfo>(tourInfo, "TourInfo");
        orm.AddToTable<TourPage>(tourPage, "TourPage");
        foreach (var tourProgram in tourProgramCollection)
        {
            foreach (var task in tourProgram.Tasks)
            {
                orm.AddToTable<TourProgram>(new TourProgram() 
                { 
                    Day = tourProgram.Id, 
                    Task = task,
                    TourId = tourId,
                }, "TourProgram");
            }
        }

        return RefreshPage();
    }

    private void SetEntity<T>(T tour)
    {
        var tourProps = tour.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in tourProps)
        {
            if (prop.Name != "Id")
            {
                object value = GetValue(FormData, prop.Name, prop.PropertyType);
                prop.SetValue(tour, value ?? string.Empty);
            }
        }
    }

    /*public static object GetValue(Dictionary<string, string> formData, string key, Type type)
    {
        if (!formData.ContainsKey(key)) return null;

        string value = formData[key];
        if (string.IsNullOrEmpty(value)) return null;

        try
        {
            return Convert.ChangeType(value, type);
        }
        catch
        {
            return null;
        }
    }*/

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
