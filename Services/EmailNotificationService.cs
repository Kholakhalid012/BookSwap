using System.Threading.Tasks;
using BookSwap.Models.Interfaces;
using System.Net.Mail;
using System.Net;

namespace BookSwap.Models.Services
{
    public class EmailNotificationService : INotificationService
    {
        public async Task NotifyAsync(string recipientEmail, string message)
        {
            if (string.IsNullOrEmpty(recipientEmail)) return;

           
            var mail = new MailMessage();
            mail.To.Add(recipientEmail);
            mail.Subject = "BookSwap Notification";
            mail.Body = message;
            mail.From = new MailAddress("your_email@example.com");
            
            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential("your_email@example.com", "your_app_password"); // use app password
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
        }
    }
}
