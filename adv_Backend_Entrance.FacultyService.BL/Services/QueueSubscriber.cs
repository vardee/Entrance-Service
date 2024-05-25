using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Interfaces.ApplicantService;
using adv_Backend_Entrance.Common.Interfaces.EntranceService;
using adv_Backend_Entrance.Common.Interfaces.FacultyService;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.FacultyService.BL.Services
{
    public static class QueueSubscriber
    {
        public static void QueueSubscribe(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var bus = RabbitHutch.CreateBus("host=localhost");
            var documentService = serviceProvider.GetRequiredService<IFacultyInteractionsService>();
            var facultyService = serviceProvider.GetRequiredService<IFacultyService>();
            bus.Rpc.Respond<Guid, List<GetDocumentTypesDTO>>(async request =>
            {
                return await documentService.GetDocumentTypes();
            }, x => x.WithQueueName("application_facultyDocuments"));


            bus.Rpc.Respond<Guid, List<GetFacultiesDTO>>(async request =>
            {
                return await documentService.GetFaculties();
            }, x => x.WithQueueName("getFacultiesMVC"));

            bus.PubSub.Subscribe<ImportInfoMVCDTO>("importInfoMVCDTO", async data =>
            {
                await facultyService.GetDictionary(data.Types);
            });

            bus.Rpc.Respond<Guid, GetQuerybleProgramsDTO>(async request =>
            {
                var result = await documentService.GetQueryblePrograms(1000, 1, null, null, null, null, null);
                Console.WriteLine(result);
                return result;
            }, x => x.WithQueueName("application_facultyPrograms"));

            bus.Rpc.Respond<Guid, GetQuerybleProgramsDTO>(async request =>
            {
                var result = await documentService.GetQueryblePrograms(1000, 1, null, null, null, null, null);
                Console.WriteLine(result);
                return result;
            }, x => x.WithQueueName("getProgramsForAppMVC"));
            bus.Rpc.Respond<GetImportMVCDTO, GetAllQuerybleImportsDTO>(async request =>
            {
                var result = await documentService.GetAllImprots(request.Size, request.Types);
                return result;
            }, x => x.WithQueueName("gettingImportsMVCDTO"));
            bus.Rpc.Respond<Guid, List<GetEducationLevelsDTO>>(async request =>
            {
                var result = await documentService.GetEducationLevels();
                return result;
            }, x => x.WithQueueName("getEducationLevelsFromEL"));
        }
    }
}
