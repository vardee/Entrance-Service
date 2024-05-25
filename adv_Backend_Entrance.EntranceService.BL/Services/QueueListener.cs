using adv_Backend_Entrance.Common.DTO;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.EntranceService.Applicant;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.Interfaces.ApplicantService;
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
            var applicantService = serviceProvider.GetRequiredService<IEntranceService>();
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
            bus.PubSub.Subscribe<RemoveManagerFromDbDTO>("addManagerInDB_User", async data =>
            {
                await managerService.RemoveManagerFromDb(data);
            });
            bus.PubSub.Subscribe<ChangeManagerMVCDTO>("changeApplicationManagerMVC", async data =>
            {
                await fullManagerService.ChangeApplicationManager(data.ApplicationId,data.ManagerId);
            });
            bus.PubSub.Subscribe<DeleteProgramFromApplicationDTO>("deleteApplicationProgramMVC", async data =>
            {
                await applicantService.DeleteProgramFromApplication(data);
            });
            bus.Rpc.Respond<GetApplicationsMVCDTO, GetAllQuerybleApplicationsDTO>(async request =>
            {
                return await fullManagerService.GetQuerybleApplications(request.size,request.page,request.name,request.ProgramId,request.Faculties,request.entranceApplicationStatuses,request.haveManager, request.isMy,request.managerId,request.timeSorting);
            }, x => x.WithQueueName("getApplicationsMVC"));
            bus.Rpc.Respond<GetManagersMVCDTO, GetAllQuerybleManagersDTO>(async request =>
            {
                return await fullManagerService.GetManagers(request.Size, request.Page,request.Name,request.Role);
            }, x => x.WithQueueName("gettingManagers_withMvc"));
            bus.Rpc.Respond<Guid, GetAllQuerybleManagersDTO>(async request =>
            {
                return await fullManagerService.GetManagers(1000, 1, "", null);
            }, x => x.WithQueueName("gettingForFilterManagers_withMvc"));
            bus.Rpc.Respond<Guid, GetApplicantInformationDTO>(async request =>
            {
                var applicantInfo = new GetApplicantDTO
                {
                    ApplicantId = request
                };
                return await fullManagerService.GetApplicantInformation(applicantInfo);
            }, x => x.WithQueueName("getApplicantInformationMVC"));
            bus.Rpc.Respond<GetApplicantDTO, GetApplicationsDTO>(async request =>
            {
                return await fullManagerService.GetApplicantion(request);
            }, x => x.WithQueueName("getApplicantProgramsMVC"));
            bus.Rpc.Respond<Guid, GetAllQuerybleManagersDTO>(async request =>
            {
                return await fullManagerService.GetManagers(1000, 1, "", null);
            }, x => x.WithQueueName("getManagerInformationMVC"));
            bus.PubSub.Subscribe<ChangeProgramPriorityDTO>("changePriorityProgramMVC", async data =>
            {
                await applicantService.ChangeProgramPriority(data);
            });

        }
    }
}
