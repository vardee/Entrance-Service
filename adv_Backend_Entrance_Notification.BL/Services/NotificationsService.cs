using adv_Backend_Entrance.Common.DTO.NotificationService;
using adv_Backend_Entrance.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using adv_Backend_Entrance.Common.Middlewares;

namespace adv_Backend_Entrance_Notification.BL.Services
{
    public class NotificationService : BackgroundService
    {
        private readonly string _mailDevHost = "localhost";
        private readonly int _mailDevPort = 1025;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "notification-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    await ProcessNotification(message);
                };

                channel.BasicConsume(queue: "notification-queue", autoAck: true, consumer: consumer);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(100, stoppingToken);
                }
            }
        }

        private async Task ProcessNotification(string message)
        {
            try
            {
                var sendNotificationDTO = JsonSerializer.Deserialize<SendNotificationDTO>(message);

                await SendEmailNotification(sendNotificationDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing notification: {ex.Message}");
            }
        }
        private async Task SendEmailNotification(SendNotificationDTO sendNotificationDTO)
        {
            try
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
            catch (Exception ex)
            {
                throw new BadRequestException("BadRequest");
            }
        }

    }
}
