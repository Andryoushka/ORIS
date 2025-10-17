﻿using MiniHttpServer.Core.Abstract;
using MiniHttpServer.share;
using System.Net;
using System.Text;

namespace MiniHttpServer.Core.Handlers;

public class StaticFilesHandler : Handler
{
    public override async Task HandleRequest(HttpListenerContext context)
    {

        var request = context.Request;
        var isGetMethod = request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase);
        var isStaticFile = request.Url.AbsolutePath.Split('/').Any(x => x.Contains("."));

        if (isGetMethod && isStaticFile)
        {
            Task.Run(async () =>
            {
                await SendResponse(context);
            });

            /*
               if (path == null || path == "/")
                buffer = GetResponseBytes.Invoke($"Public/index.html");
            */

            /*buffer = GetResponseBytes.Invoke(path);

            response.ContentType = MiniHttpServer.Shared.ContentType.GetContentType(path.Trim('/'));

            if (buffer == null)
            {
                response.StatusCode = 404;
                string errorText = "<html><body>404 - Not Found</html></body>";
                buffer = Encoding.UTF8.GetBytes(errorText);
            }

            response.ContentLength64 = buffer.Length;

            using Stream output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            await output.FlushAsync();

            if (response.StatusCode == 200)
                Console.WriteLine($"Запрос обработан: {request.Url.AbsolutePath} - Status: {response.StatusCode}");
            else
                Console.WriteLine($"Ошибка запроса: {request.Url.AbsolutePath} - Status: {response.StatusCode}");
*/
        }
        // передача запроса дальше по цепи при наличии в ней обработчиков
        else if (Successor != null)
        {
            Successor.HandleRequest(context);
        }
    }

    private async Task SendResponse(HttpListenerContext context)
    {
        var _SettingsModel = SettingsManager.Instance;

        var response = context.Response;
        var path = context.Request.Url.AbsolutePath;

        try // Отправка ответа
        {
            response.ContentType = ContentExtension.GetExtension(path);

            var responseBytes = GetResponseBytes.Invoke(path);
            response.ContentLength64 = (long) responseBytes?.Length;
            response.OutputStream.Write(responseBytes);

            //using (var fileStream = new FileStream(path, FileMode.Open))
            //{
            //    response.ContentLength64 = fileStream.Length;
            //    fileStream.CopyTo(response.OutputStream);
            //}

            Logger.Print("Запрос обработан. Отправлен файл.");
        }
        catch (FileNotFoundException)
        {
            Logger.PrintError("Ошибка : файл не найден.");
            response.StatusCode = 404;
            Console.Beep();
            throw new Exception();
        }
        catch (KeyNotFoundException)
        {
            Logger.PrintError("Ошибка : расширение для файла не добавлено в словарь.");
        }
        finally
        {
            response.Close();
        }
    }
}
