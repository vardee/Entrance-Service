using adv_Backend_Entrance.Common.DTO.NotificationService;
using adv_Backend_Entrance.Common.Interfaces.NotificationService;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace YourNamespace
{
    public class NotificationSendService : INotificationService
    {
        private readonly string _mailDevHost;
        private readonly int _mailDevPort;

        public NotificationSendService(IConfiguration configuration)
        {
            _mailDevHost = configuration["MailDev:Host"];
            _mailDevPort = configuration.GetValue<int>("MailDev:Port");
        }

        public async Task SendNotificationAsync(SendNotificationDTO sendNotificationDTO)
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress("system@admin.mail");
                message.To.Add(sendNotificationDTO.SendTo);
                message.Subject = "Notification";
                message.Body = sendNotificationDTO.Message;

                using (var client = new SmtpClient(_mailDevHost, _mailDevPort))
                {
                    client.EnableSsl = false;
                    await client.SendMailAsync(message);
                }
            }
        }
    }
}
