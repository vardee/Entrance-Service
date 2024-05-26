using adv_Backend_Entrance.Common.DTO.NotificationService;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Interfaces.NotificationService;
using adv_Backend_Entrance.Common.Interfaces.UserService;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace adv_Backend_Entrance_Notification.BL.Services
{
    public static class NotificationService
    {
        public static void QueueNotificationSubscribe(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var bus = RabbitHutch.CreateBus("host=localhost");
            var notificationService = serviceProvider.GetRequiredService<INotificationService>();
            bus.PubSub.Subscribe<SendNotificationDTO>("notification_send_queue", async data =>
            {
                await notificationService.SendNotificationAsync(data);
            });
        }

    }
}
