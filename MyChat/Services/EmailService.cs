﻿using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace MyChat.Services
{
    public class EmailService
    {
        public async Task SendMessageAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
        
            emailMessage.From.Add(new MailboxAddress(
                "Администратор сайта",
                "doskalii@yandex.ru"
            ));
        
            emailMessage.To.Add(new MailboxAddress("", email));
        
            emailMessage.Subject = subject;
        
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.yandex.ru", 25, false);
            await client.AuthenticateAsync("doskalii@yandex.ru", "mamapapa+18");
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}