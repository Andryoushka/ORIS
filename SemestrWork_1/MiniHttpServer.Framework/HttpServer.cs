using MiniHttpServer.Framework.Core.Handlers;
using MiniHttpServer.Framework.share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Framework;

public class HttpServer
{
    private readonly SettingsManager _SettingsModel;
    private readonly HttpListener _HttpListener;
    private readonly List<IService> _services;
    public bool ServerIsStop = true;

    private List<HttpListenerContext> httpListenerContexts = new();

    public HttpServer()
    {
        _SettingsModel = SettingsManager.Instance;
        _HttpListener = new HttpListener();
        ServerIsStop = false;
        _services = new List<IService>();
    }

    public void AddService(IService service)
    {
        _services.Add(service);
    }

    public async Task StartAsync()
    {
        _HttpListener.Prefixes.Add($"http://{_SettingsModel.Settings.Domain}:{_SettingsModel.Settings.Port}/");
        _HttpListener.Start();
        Logger.Print("Сервер запущен.\nОжидаем запрос.");
        await Receive();
    }

    public void Stop()
    {
        _HttpListener.Stop();
        _HttpListener.Close();
        Logger.Print("Сервер закрыт.");
    }

    private async Task Receive()
    {
        while (_HttpListener.IsListening && !ServerIsStop)
        {
            try
            {
                var context = await _HttpListener.GetContextAsync();
                httpListenerContexts.Add(context);
            #region HANDLERS
                //Console.WriteLine("======================================================"); ;
                //Logger.Print("Запрос получен.");

                Handler staticFilesHandler = new StaticFilesHandler();
                EndpointsHandler endpointsHandler = new EndpointsHandler();
                endpointsHandler.AuthorizeService = _services.FirstOrDefault(s => s.GetType().Name == "UserService");
                staticFilesHandler.Successor = endpointsHandler;
                await staticFilesHandler.HandleRequest(context);
             #endregion
            }
            catch (HttpListenerException le)
            {
                Logger.Print($"HTTP Listener Error: {le.Message}.");
                break;
            }
            catch (Exception ex)
            {
                Logger.Print($"Ошибка : {ex}.");
                break;
            }
        }
    }
}
