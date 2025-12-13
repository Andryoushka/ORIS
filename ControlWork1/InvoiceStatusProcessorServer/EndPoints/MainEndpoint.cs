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
