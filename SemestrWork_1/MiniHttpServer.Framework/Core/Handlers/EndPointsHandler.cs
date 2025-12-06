using MiniHttpServer.Framework.Attributes;
using MiniHttpServer.Framework.Core.Abstract;
using MiniHttpServer.Framework.share;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiniHttpServer.Framework.Core.Handlers;

internal class EndpointsHandler : Handler
{
    public IService AuthorizeService { get; set; }

    public override async Task HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;

        var requestSections = request.RawUrl?.Split('?');

        var requestSplit = requestSections[0].TrimStart('/').Split('/').ToArray();
        //var isForm = IsFormSubmission(context.Request);
        var endpointName = requestSplit[0] == string.Empty ? "/" : requestSplit[0];

        var assembly = Assembly.GetEntryAssembly();
        var endpoint = assembly.GetTypes()
                                .Where(t =>
                                {
                                    var ep = t.GetCustomAttribute<EndpointAttribute>();
                                    if (ep != null && ep.Route == endpointName)
                                        return true;
                                    return false;
                                })
                                .FirstOrDefault(/*end => IsCheckedNameEndpoint(end.Name, endpointName)*/);

        if (endpoint == null)
        {
            Logger.PrintError("Контроллер не найден.");
            context.Response.StatusCode = 404;
            context.Response.Close();
            return; // TODO: 
        }

        bool isBaseEndpoint = assembly.GetTypes()
            .Any(t => typeof(BaseEndPoint).IsAssignableFrom(t) && !t.IsAbstract);

        var instanceEndpoint = Activator.CreateInstance(endpoint);

        Dictionary<string, string> data = null;
        if (request.HasEntityBody)
            data = ParseFormData(request);


        if (isBaseEndpoint)
        {
            var baseEndpoint = (instanceEndpoint as BaseEndPoint);
            baseEndpoint.SetContext(context);
            baseEndpoint.SetFormData(data);
            baseEndpoint.SetService(AuthorizeService);
        }

        // путь к методу в роуте
        var methodRoute = requestSplit.Length > 1 ? requestSplit[1].ToLower() : null;
        var method = endpoint.GetMethods().Where(t => t.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name.Equals($"Http{context.Request.HttpMethod}",StringComparison.OrdinalIgnoreCase)
                    && ((HttpRoute)attr).Route == methodRoute))
                    .FirstOrDefault(/*m => m.Name.ToLower() == methodName*/);

        if (method == null)
        {
            Logger.PrintError($"Метод контроллера {endpointName} не определён.");
            context.Response.StatusCode = 404;
            context.Response.Close();
            return;  // TODO:            
        }

        // Аргументы метода
        List<object> objects = null;
        if (requestSections.Length >= 2)
        {
            objects = new List<object>();
            var param = ParseFormData(requestSections[1]);
            var methodParam = method.GetParameters();
            foreach (var paramInfo in methodParam)
            {
                if (param.ContainsKey(paramInfo.Name))
                    objects.Add(param[paramInfo.Name]);
            }
            if (objects.Count == 0)
                objects = null;
        }
        //<Проверка: все ли аргументы в наличие>//
        SetCorsHeaders(context.Response);

        var result = method.Invoke(instanceEndpoint, objects?.ToArray());
        if (result is string str)
        {
            context.Response.ContentType = "text/plain";
            SendResponse(context.Response, str);
        }
        if (result is PageResult pageResult)
        {
            context.Response.ContentType = "text/html";
            SendResponse(context.Response, pageResult.Execute(context));
        }
        if (result is RedirectResult redirect)
        {
            redirect.Execute(context);
            return;
        }
        if (result is RefreshPageResult refresh)
        {
            refresh.Execute(context);
            return;
        }

            Console.WriteLine($"Метод -{method.Name}- выполнен!");

        // передача запроса дальше по цепи при наличии в ней обработчиков
        if (Successor != null)
        {
            Successor.HandleRequest(context);
        }
    }

    private void SendResponse(HttpListenerResponse response, string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        response.ContentLength64 = bytes.Length;
        response.OutputStream.Write(bytes);
        //response.StatusCode = (int)HttpStatusCode.OK;
        response.Close();
    }

    private bool IsCheckedNameEndpoint(string endpointName, string className) =>
        endpointName.Equals(className, StringComparison.OrdinalIgnoreCase) ||
        endpointName.Equals($"{className}Endpoint", StringComparison.OrdinalIgnoreCase);

    bool IsFormSubmission(HttpListenerRequest request)
    {
        return request.ContentType?.StartsWith("application/x-www-form-urlencoded") == true ||
               request.ContentType?.StartsWith("multipart/form-data") == true;
    }

    private static Dictionary<string, string> ParseFormData(string formData)
    {
        var result = new Dictionary<string, string>();
        var pairs = Uri.UnescapeDataString(formData).Split('&');

        // Группируем по ключам для обработки множественных значений
        var groupedData = pairs
            .Select(pair => pair.Split('='))
            .Where(parts => parts.Length == 2)
            .GroupBy(parts => parts[0], parts => parts[1]);

        foreach (var group in groupedData)
        {
            if (group.Count() > 1)
            {
                // Множественные значения объединяем через запятую
                result[group.Key] = string.Join(",", group);
            }
            else
            {
                result[group.Key] = group.First();
            }
        }

        //foreach (var pair in pairs)
        //{
        //    var keyValue = pair.Split('=');
        //    if (keyValue.Length == 2)
        //    {
        //        string key = Uri.UnescapeDataString(keyValue[0]);
        //        string value = Uri.UnescapeDataString(keyValue[1]);
        //        result[key] = value;
        //    }
        //}

        return result;
    }

    public static Dictionary<string, string> ParseFormData(HttpListenerRequest request)
    {
        var formData = new Dictionary<string, string>();

        using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
        string content = reader.ReadToEnd();
        content = Uri.UnescapeDataString(content).Replace('+',' ');

        var data = content.Split('&');

        // Группируем по ключам для обработки множественных значений
        var groupedData = data
            .Select(pair => pair.Split('='))
            .Where(parts => parts.Length == 2)
            .GroupBy(parts => parts[0], parts => parts[1]);

        // кастыль для иозображений из RUSSPASS
        if (request.RawUrl == "/admin/create" && groupedData.Any(cardImage => cardImage.Key != "CardImage"))
        {
            formData["CardImage"] = string.Join('&', data.Where(x => x.StartsWith("CardImage") || x.StartsWith("q") || x.StartsWith("w"))).Replace("CardImage=","");
        }

        foreach (var group in groupedData)
        {
            if (group.Count() > 1)
            {
                // Множественные значения объединяем через запятую
                formData[group.Key] = string.Join(",", group);
            }
            else
            {
                formData[group.Key] = group.First();
            }
        }
        //foreach (var pair in data)
        //{
        //    var dataKeyValue = pair.Split("=");
        //    formData[dataKeyValue[0]] = dataKeyValue[1];
        //}

        return formData;
    }

    public static T GetValue<T>(Dictionary<string, string> formData, string key, T defaultValue = default)
    {
        if (!formData.ContainsKey(key)) return defaultValue;

        string value = formData[key];
        if (string.IsNullOrEmpty(value)) return defaultValue;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    private void SetCorsHeaders(HttpListenerResponse response)
    {
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With");
        response.Headers.Add("Access-Control-Max-Age", "86400"); // 24 часа
        response.Headers.Add("Access-Control-Expose-Headers", "Content-Type, X-Custom-Header, X-Total-Count");
    }

}
