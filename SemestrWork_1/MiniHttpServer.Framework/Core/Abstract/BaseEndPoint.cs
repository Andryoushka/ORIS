using MiniHttpServer.Framework.share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Framework.Core.Abstract;

abstract public class BaseEndPoint
{
    protected HttpListenerContext Context { get; private set; }
    protected Dictionary<string, string> FormData { get; private set; }

    protected IService AuthService { get; private set; }

    public void SetContext(HttpListenerContext context)
    {
        Context = context;
    }

    public void SetFormData(Dictionary<string, string> data)
    {
        FormData = data;
    }

    public void SetService(IService service)
    {
        AuthService = service;
    }

    protected IResponseResult Page(string pathTemplate, Dictionary<string, object> data) => new PageResult(pathTemplate, data);
    protected IResponseResult Redirect(string path) => new RedirectResult(path);
    protected IResponseResult RefreshPage() => new RefreshPageResult();
}
