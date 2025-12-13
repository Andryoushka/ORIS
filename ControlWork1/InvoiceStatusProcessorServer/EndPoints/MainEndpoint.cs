using InvoiceStatusProcessorServer.Models;
using MiniHttpServer.Framework.Attributes;
using MiniHttpServer.Framework.Core.Abstract;
using MiniHttpServer.Framework.share;
using MyORMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceStatusProcessorServer.EndPoints;

[Endpoint("cw1")]
public class MainEndpoint : BaseEndPoint
{
    [HttpGet("health")]
    public IResponseResult GetHealth()
    {
        var connectionString = SettingsManager.Instance.Settings.ConnectionString;
        var orm = new ORMContext(connectionString);
        var inv = orm.ReadByAll<Invoice>("Invoices");

        foreach (var item in inv)
        {
            item.ToString();
        }

        var random = new Random();
        foreach (var item in inv)
        {
            var status = random.NextDouble() >= 0.7 ? "error" : "success";
            item.Status = status;
            orm.Update(item.Id, item , "Invoices");
        }

        return null;
    }

    [HttpGet("config")]
    public IResponseResult GetConfig()
    {


        return null;
    }

    [HttpPost("config/reload")]
    public IResponseResult GetReload()
    {


        return null;
    }
}
