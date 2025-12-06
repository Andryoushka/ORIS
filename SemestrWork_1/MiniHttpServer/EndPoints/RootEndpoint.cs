using MiniHttpServer.Framework.Attributes;
using MiniHttpServer.Framework.Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.EndPoints;

[Endpoint("/")]
public class RootEndpoint : BaseEndPoint
{
    [HttpGet]
    public IResponseResult RedirectToMainPage()
    {
        return Redirect("/mainpage");
    }
}
