using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Interfaces.ApplicantService;
using AutoMapper.Internal;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.ApplicantService.BL.Services
{
    public static class QueueSubscriber
    {
        public static void QueueSubscribe(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var bus = RabbitHutch.CreateBus("host=localhost");
            var documentService = serviceProvider.GetRequiredService<IApplicantService>();
            bus.Rpc.Respond<Guid, GetPassportInformationDTO>(async request =>
            {
                return await documentService.GetPassportInformation(request);
            }, x => x.WithQueueName("application_passport"));
            bus.Rpc.Respond<Guid, GetEducationInformationDTO>(async request =>
            {
                return await documentService.GetEducationInformation(request);
            }, x => x.WithQueueName("application_educationInfo"));
        }
    }

}
