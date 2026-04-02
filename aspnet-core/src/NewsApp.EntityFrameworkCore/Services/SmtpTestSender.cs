using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace NewsApp.EntityFrameworkCore.Services
{
    public class SmtpTestSender
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("NewsApp", "agustinvergara215@gmail.com"));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using var client = new SmtpClient();

            
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            await client.AuthenticateAsync("agustinvergara215@gmail.com", "xvhqahuixjrlwlrw");

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}