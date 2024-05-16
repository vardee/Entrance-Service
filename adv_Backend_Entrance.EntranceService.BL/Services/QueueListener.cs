using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.Interfaces.UserService;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.EntranceService.BL.Services
{
    public static class QueueListener
    {
        public static void QueueSubscribe(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var bus = RabbitHutch.CreateBus("host=localhost");
            var managerService = serviceProvider.GetRequiredService<ManagerHelperService>();
            bus.PubSub.Subscribe<EditApplicantProfileInformationDTO>("applicantProfile_edit", async data =>
            {
                await managerService.EditApplicantInformation(data);
            });
            bus.PubSub.Subscribe<EditApplicantProfileInformationDTO>("profile_edit", async data =>
            {
                await managerService.EditApplicantInformation(data);
            });
        }
    }
}
