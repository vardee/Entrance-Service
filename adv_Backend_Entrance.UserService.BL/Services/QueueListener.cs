using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Interfaces.UserService;
using adv_Backend_Entrance.UserService.BL.Services;

namespace adv_Backend_Entrance.UserService.BL.Services
{
    public static class QueueListener
    {
        public static void QueueSubscribe(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.PubSub.Subscribe<AddRoleDTO>("application_created", async data =>
            {
                var additionService = serviceProvider.GetRequiredService<AdditionTokenService>();
                await additionService.AddRoleToUser(data.Role, data.UserId);
            });
        }
    }
}
