using System.Net.Mail;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.Domain.Models;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace EventMaster.BLL.Services.Implementation;

public class EmailService:IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(EventUpdateEmail eventUpdateEmail)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("EventMaster", _configuration["Email:SenderAddress"]));
        emailMessage.To.Add(new MailboxAddress("", eventUpdateEmail.ToEmail));
        emailMessage.Subject = "Изменение события, на которое вы зарегистрированы";

        var bodyBuilder = new BodyBuilder { HtmlBody = RenderEmail(eventUpdateEmail) };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:SmtpPort"]), false);
            await client.AuthenticateAsync(_configuration["Email:SmtpUser"], _configuration["Email:SmtpPassword"]);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }

    private string RenderEmail(EventUpdateEmail eventUpdateEmail)
    {
        return $@"
        <!DOCTYPE html>
        <html lang='ru'>
        <head>
            <meta charset='UTF-8'>
            <title>Изменение события</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    line-height: 1.6;
                }}
                .container {{
                    width: 80%;
                    margin: auto;
                    padding: 10px;
                }}
                .header {{
                    font-size: 20px;
                    font-weight: bold;
                }}
                .sub-header {{
                    font-size: 18px;
                }}
            </style>
        </head>
        <body>
        <div class='container'>
            <p class='header'>Изменение события</p>
            <h4>Здравствуйте, {eventUpdateEmail.Name}!</h4>
            <p>Информируем вас о том, что ваше событие было изменено.</p>
            <p>Вот обновленная информация:</p>
        
            <div class='details'>
                <h4 class='sub-header'>Детали события</h4>
                <p>Название: {eventUpdateEmail.EventName}</p>
                <p>Дата: {eventUpdateEmail.EventDate}</p>
                <p>Место: {eventUpdateEmail.EventLocation}</p>
            </div>
        </div>
        </body>
        </html>";
    }
}