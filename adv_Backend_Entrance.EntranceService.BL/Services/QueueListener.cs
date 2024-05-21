using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.Interfaces.EntranceService;
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
            var fullManagerService = serviceProvider.GetRequiredService<IManagerService>();
            bus.PubSub.Subscribe<EditApplicantProfileInformationDTO>("applicantProfile_edit", async data =>
            {
                await managerService.EditApplicantInformation(data);
            });
            bus.PubSub.Subscribe<EditApplicantProfileInformationDTO>("profile_edit", async data =>
            {
                await managerService.EditApplicantInformation(data);
            });
            bus.PubSub.Subscribe<TakeApplicationMVCDTO>("takeApplication", async data =>
            {
                var applicationId = new TakeApplicationDTO
                {
                    ApplicationId = data.ApplicationId,
                };
                await fullManagerService.TakeApplication(applicationId, data.UserId);
            });
            bus.PubSub.Subscribe<RejectApplicationMVCDTO>("rejectApplication", async data =>
            {
                var applicationId = new RejectApplicationDTO
                {
                    ApplicationId = data.ApplicationId,
                };
                await fullManagerService.RejectApplication(applicationId, data.UserId);
            });
            bus.PubSub.Subscribe<ChangeApplicationStatusMVCDTO>("changeApplicationStatusMVC", async data =>
            {
                var application = new ChangeApplicationStatusDTO
                {
                    Status = data.Status,
                    ApplicationId = data.ApplicationId,
                };
                await fullManagerService.ChangeApplicationStatus(application, data.UserId);
            });
            bus.PubSub.Subscribe<AddManagerInDbDTO>("addManagerInDB_User", async data =>
            {
                await managerService.AddManagerInDb(data);
            });
            bus.Rpc.Respond<GetApplicationsMVCDTO, GetAllQuerybleApplicationsDTO>(async request =>
            {
                return await fullManagerService.GetQuerybleApplications(request.size,request.page,request.name,request.ProgramId,request.Faculties,request.entranceApplicationStatuses,request.haveManager, request.isMy,request.managerId,request.timeSorting);
            }, x => x.WithQueueName("getApplicationsMVC"));
        }
    }
}
