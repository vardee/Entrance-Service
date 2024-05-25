using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.Interfaces.ApplicantService;
using adv_Backend_Entrance.Common.Interfaces.EntranceService;
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
            bus.Rpc.Respond<Guid, GetPassportInformationDTO>(async request =>
            {
                return await documentService.GetPassportInformation(request);
            }, x => x.WithQueueName("getPassportProfileMVC"));
            bus.Rpc.Respond<Guid, GetEducationInformationDTO>(async request =>
            {
                return await documentService.GetEducationInformation(request);
            }, x => x.WithQueueName("getEducationLevelProfileMVC"));
            bus.PubSub.Subscribe<AddEducationDocumentMVCDTO>("addApplicantEducationDocumentMVC", async data =>
            {
                var educationDocument = new AddEducationLevelDTO
                {
                    EducationLevel = data.EducationLevel,
                };
                await documentService.AddEducationLevel(educationDocument, data.Id);
            });
            bus.PubSub.Subscribe<AddPassportMVCDTO>("addApplicantPassportMVC", async data =>
            {
                var passport = new AddPassportDTO
                {
                    IssuedWhen = data.IssuedWhen,
                    BirthPlace = data.BirthPlace,
                    IssuedWhom = data.IssuedWhom,
                    PassportNumber = data.PassportNumber,
                };
                await documentService.AddPassport(passport,data.Id);
            });
            bus.PubSub.Subscribe<EditPassportMVCDTO>("editPassportInfoMVC", async data =>
            {
                var passport = new AddPassportDTO
                {
                    IssuedWhen = data.IssuedWhen,
                    BirthPlace = data.BirthPlace,
                    IssuedWhom = data.IssuedWhom,
                    PassportNumber = data.PassportNumber,
                };
                await documentService.EditPaspportInformation(passport, data.Id);
            });
            bus.PubSub.Subscribe<EditPassportInfoMVCDTO>("editPassportInformationMVC", async data =>
            {
                var passport = new AddPassportDTO
                {
                    IssuedWhen = data.IssuedWhen,
                    BirthPlace = data.BirthPlace,
                    IssuedWhom = data.IssuedWhom,
                    PassportNumber = data.PassportNumber,
                };
                await documentService.EditPaspportInformation(passport, data.Id); 
            });
            bus.PubSub.Subscribe<EditEducationInfoMVCDTO>("editApplicantEducationInformationMVC", async data =>
            {
                var educationDocument = new AddEducationLevelDTO
                {
                    EducationLevel = data.EducationLevel,
                };
                await documentService.EditEducationInformation(educationDocument, data.Id);
            });

        }
    }

}
