using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MiniHttpServer.Framework.share;

namespace MiniHttpServer.Services;

public class UserService : IService
{
    private readonly Dictionary<string, string> _users = new()
    {
        {"admin", "admin"},
        {"user", "123"}
    };

    private readonly Dictionary<string, string> _sessions = new();

    public bool IsAuthorized(HttpListenerRequest request)
    {
        var sessionCookie = request.Cookies["sessionId"];
        return sessionCookie != null &&
               GetUserBySession(sessionCookie.Value) != null;
    }

    public bool IsAdmin(string username, string password)
    {
        return _users.ContainsKey(username) && _users[username] == password && username == "admin";
    }

    /// <summary>
    /// Проверка на наличие пользователя в списке.
    /// </summary>
    /// <param name="username">Логин</param>
    /// <param name="password">Пароль</param>
    /// <returns></returns>
    public bool ValidateUser(string username, string password)
    {
        return _users.ContainsKey(username) && _users[username] == password;
    }

    /// <summary>
    /// Создает сессию для пользователя.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public string CreateSession(string username)
    {
        var sessionId = Guid.NewGuid().ToString();
        _sessions[sessionId] = username;
        return sessionId;
    }

    /// <summary>
    /// Возвращает пользователя по сессии.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public string GetUserBySession(string sessionId)
    {
        if (sessionId == null)
            return "гость";
        return _sessions.ContainsKey(sessionId) ? _sessions[sessionId] : "гость";
    }

    /// <summary>
    /// Удаляет сессию.
    /// </summary>
    /// <param name="sessionId"></param>
    public void RemoveSession(string sessionId)
    {
        _sessions.Remove(sessionId);
    }
}
