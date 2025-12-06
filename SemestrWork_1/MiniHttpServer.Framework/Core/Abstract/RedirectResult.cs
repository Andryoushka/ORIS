using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Framework.Core.Abstract;

public class RedirectResult : IResponseResult
{
    private string _path { get; set; }
    public RedirectResult(string path)
    {
        _path = path;
    }
    public string Execute(HttpListenerContext context)
    {
/*
        ResponseInfo
StatusCode
Headers
Body
Coockie
 */
        var response = context.Response;
        response.StatusCode = 302;
        response.Headers.Add("Location", _path);
        response.Close();
        return null;
    }
}
