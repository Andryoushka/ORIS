using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Framework.Core.Abstract;

public class RefreshPageResult : IResponseResult
{
    public string Execute(HttpListenerContext context)
    {
        var response = context.Response;
        var request = context.Request;

        response.StatusCode = 200;
        response.Redirect(request.UrlReferrer.AbsolutePath);
        response.Close();
        return null;
    }
}
