using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Services;

public static class EmailService
{
    /// <summary>
    ///  Отправляет письмо на почту.
    /// </summary>
    /// <param name="to">Кому (адрес почты)</param>
    /// <param name="subject">Тема письма</param>
    /// <param name="message">Содержимое письма</param>
    public static void SendEmail(string to, string subject, string message)
    {
        // TODO: ДЗ smtpClient

        // отправитель - устанавливаем адрес и отображаемое в письме имя
        MailAddress from = new MailAddress("dawidov.andrej2011@yandex.ru", "Andryoushka");
        // кому отправляем
        MailAddress toUser = new MailAddress(to);
        // создаем объект сообщения
        MailMessage m = new MailMessage(from, toUser);
        // тема письма
        m.Subject = subject;
        // текст письма
        m.Body = $"<h2>{message}</h2>";
        // письмо представляет код html
        m.IsBodyHtml = true;
        // адрес smtp-сервера и порт, с которого будем отправлять письмо
        SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 587);
        // логин и пароль
        smtp.Credentials = new NetworkCredential("dawidov.andrej2011@yandex.ru", "ypauztvyfwvywmrt");
        smtp.EnableSsl = true;
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.UseDefaultCredentials = false;
        smtp.Send(m);
        smtp.Dispose();
    }
}

