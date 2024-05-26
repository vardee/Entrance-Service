using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Helpers.Validators;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.EntranceService.DAL.Data;
using adv_Backend_Entrance.UserService.DAL.Data.Entities;
using adv_Backend_Entrance.UserService.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adv_Backend_Entrance.EntranceService.DAL.Data.Models;
using static System.Net.Mime.MediaTypeNames;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.DTO.NotificationService;
using EasyNetQ;
using adv_Backend_Entrance.Common.DTO.UserService;

namespace adv_Backend_Entrance.EntranceService.BL.Services
{
    public class ManagerHelperService
    {
        private readonly EntranceDBContext _entranceDBContext;
        private readonly TokenHelper _tokenHelper;
        private readonly IBus _bus;
        public ManagerHelperService(EntranceDBContext entranceDBContext, TokenHelper tokenHelper)
        {
            _entranceDBContext = entranceDBContext;
            _tokenHelper = tokenHelper;
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public async Task EditApplicantInformation(EditApplicantProfileInformationDTO editApplicantProfileInformation)
        {
            var applicant = await _entranceDBContext.Applicants.FirstOrDefaultAsync(a => a.UserId == editApplicantProfileInformation.UserId);
            var manager = await _entranceDBContext.Managers.FirstOrDefaultAsync(m=> m.UserId == editApplicantProfileInformation.UserId);
            if(manager != null)
            {
                string fullname = "";
                if (editApplicantProfileInformation.FirstName != "")
                {
                    fullname = editApplicantProfileInformation.FirstName;
                }
                if (editApplicantProfileInformation.LastName != "")
                {
                    fullname = editApplicantProfileInformation.FirstName + editApplicantProfileInformation.LastName;
                }
                if (editApplicantProfileInformation.Patronymic != "")
                {
                    fullname = editApplicantProfileInformation.FirstName + editApplicantProfileInformation.LastName+ editApplicantProfileInformation.Patronymic;
                }
                if(editApplicantProfileInformation.Email != "")
                {
                    manager.Email = editApplicantProfileInformation.Email;
                }
            }
            if(applicant != null)
            {
                if (editApplicantProfileInformation.FirstName != "")
                {
                    applicant.FirstName = editApplicantProfileInformation.FirstName;
                }
                if (editApplicantProfileInformation.LastName != "")
                {
                    applicant.LastName = editApplicantProfileInformation.LastName;
                }
                if (editApplicantProfileInformation.Patronymic != "")
                {
                    applicant.Patronymic = editApplicantProfileInformation.Patronymic;
                }
                if (editApplicantProfileInformation.Nationality != "")
                {
                    applicant.Nationality = editApplicantProfileInformation.Nationality;
                }
            }
            else if(applicant== null && manager == null)
            {
                throw new NotFoundException("This person not found!");
            }
            await _entranceDBContext.SaveChangesAsync();
        }
        public async Task AddManagerInDb(AddManagerInDbDTO addManagerInDb)
        {
            var currentUser = await _entranceDBContext.Managers.FirstOrDefaultAsync(m => m.UserId == addManagerInDb.UserId);
            var sameRole = await _entranceDBContext.Managers.FirstOrDefaultAsync(m => m.UserId == addManagerInDb.UserId && m.Role == addManagerInDb.Role);
            if (sameRole != null)
            {
                throw new BadRequestException($"This user is already have this role {addManagerInDb.Role}");
            }
            if (currentUser != null && currentUser.Role == RoleType.MainManager && addManagerInDb.Role == RoleType.Manager)
            {
                throw new BadRequestException("You cant add Manager role to Main manager user!");
            }
            else if (currentUser != null && currentUser.Role == RoleType.Manager && addManagerInDb.Role == RoleType.MainManager)
            {
                currentUser.Role = RoleType.MainManager;
            }
            if (currentUser == null)
            {
                var manager = new ManagerModel
                {
                    Email = addManagerInDb.Email,
                    FullName = addManagerInDb.FullName,
                    Role = addManagerInDb.Role,
                    UserId = addManagerInDb.UserId,
                };
                _entranceDBContext.Managers.Add(manager);
                await _entranceDBContext.SaveChangesAsync();
            }
            var mail = new SendNotificationDTO
            {
                Message = $"Вам была успешно выдана роль {addManagerInDb.Role}",
                SendTo = addManagerInDb.Email,
            };
            await SendNotification(mail);
        }
        private async Task SendNotification(SendNotificationDTO sendNotification)
        {
            var message = new SendNotificationDTO
            {
                Message = sendNotification.Message,
                SendTo = sendNotification.SendTo,
            };
            await _bus.PubSub.PublishAsync(message, "notification_send_queue");
        }
        public async Task RemoveManagerFromDb(RemoveManagerFromDbDTO removeManagerFromDb)
        {
            var currentUser = await _entranceDBContext.Managers.FirstOrDefaultAsync(m => m.UserId == removeManagerFromDb.UserId && m.Role == removeManagerFromDb.Role);
            if (currentUser == null)
            {
                throw new BadRequestException($"This user is already have this role {removeManagerFromDb.Role}");
            }
            _entranceDBContext.Managers.Remove(currentUser);
            await _entranceDBContext.SaveChangesAsync();
            var mail = new SendNotificationDTO
            {
                Message = $"У вас была роль {removeManagerFromDb.Role}!",
                SendTo = removeManagerFromDb.Email,
            };
            await SendNotification(mail);
        }
        public async Task SyncPassport(SyncPassportDTO syncPassportDTO)
        {
            var applicant = await _entranceDBContext.Applicants.FirstOrDefaultAsync(a => a.UserId == syncPassportDTO.UserId);
            if(applicant != null)
            {
                applicant.PassportNumber = syncPassportDTO.passportNumber;
                applicant.PassportId = syncPassportDTO.PassportId;
                await _entranceDBContext.SaveChangesAsync();
            }
            else
            {
                throw new NotFoundException("This applicant not found");
            }
        }
        public async Task SyncEducationLevel(SyncEducationLevelDTO syncEducationLevel)
        {
            var applicant = await _entranceDBContext.Applicants.FirstOrDefaultAsync(a => a.UserId == syncEducationLevel.UserId);
            if (applicant == null)
            {
                throw new NotFoundException("This applicant not found!");
            }
            else
            {
                applicant.EducationId = syncEducationLevel.EducationDocumentId;
                var application = await _entranceDBContext.Applications.FirstOrDefaultAsync(a => a.ApplicantId == applicant.Id);
                if (application != null)
                {
                    var programs = await _entranceDBContext.ApplicationPrograms.Where(aP => aP.ApplicationId == application.Id).ToListAsync();
                    _entranceDBContext.ApplicationPrograms.RemoveRange(programs); 
                    _entranceDBContext.Applications.Remove(application);
                    await _entranceDBContext.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("This applicant doesn't have an application!");
                }
            }
        }
        public async Task SyncPrograms(List<SyncProgramsEntranceDTO> syncProgramsEntrance)
        {
            var deletedProgramIds = syncProgramsEntrance.Select(p => p.Id).ToList();
            var affectedApplicationPrograms = await _entranceDBContext.ApplicationPrograms
                .Where(ap => deletedProgramIds.Contains(ap.ProgramId))
                .ToListAsync();
            var affectedApplicationIds = affectedApplicationPrograms
                .Select(ap => ap.ApplicationId)
                .Distinct()
                .ToList();
            foreach (var applicationId in affectedApplicationIds)
            {
                var application = await _entranceDBContext.Applications.FindAsync(applicationId);
                if (application != null)
                {
                    var programsToRemove = affectedApplicationPrograms
                        .Where(ap => ap.ApplicationId == applicationId)
                        .ToList();
                    var applicant = await _entranceDBContext.Applicants.FirstOrDefaultAsync(aP => aP.Id == application.ApplicantId);
                    var userInfo = await _bus.Rpc.RequestAsync<Guid, UserGetProfileDTO>(applicant.UserId, x => x.WithQueueName("getProfileFromEntrance"));
                    var mail = new SendNotificationDTO
                    {
                        Message = $"Ваша заявка была удалена из-за обновлений в системе, была удалена программа!",
                        SendTo = userInfo.Email,
                    };
                    await SendNotification(mail);
                    _entranceDBContext.ApplicationPrograms.RemoveRange(programsToRemove);
                    _entranceDBContext.Applications.Remove(application);
                }
            }
            await _entranceDBContext.SaveChangesAsync();
        }
        public async Task SyncFaculties(List<SyncFacultiesDTO> syncFacultiesEntrance)
        {
            var deletedFacultyIds = syncFacultiesEntrance.Select(p => p.Id).ToList();
            var affectedApplicationPrograms = await _entranceDBContext.ApplicationPrograms
                .Where(ap => deletedFacultyIds.Contains(ap.FacultyId))
                .ToListAsync();
            var affectedApplicationIds = affectedApplicationPrograms
                .Select(ap => ap.ApplicationId)
                .Distinct()
                .ToList();
            foreach (var applicationId in affectedApplicationIds)
            {
                var application = await _entranceDBContext.Applications.FindAsync(applicationId);
                if (application != null)
                {
                    var programsToRemove = affectedApplicationPrograms
                        .Where(ap => ap.ApplicationId == applicationId)
                        .ToList();
                    var applicant = await _entranceDBContext.Applicants.FirstOrDefaultAsync(aP => aP.Id == application.ApplicantId);
                    var userInfo = await _bus.Rpc.RequestAsync<Guid, UserGetProfileDTO>(applicant.UserId, x => x.WithQueueName("getProfileFromEntrance"));
                    var mail = new SendNotificationDTO
                    {
                        Message = $"Ваша заявка была удалена из-за обновлений в системе, был удален факультет!",
                        SendTo = userInfo.Email,
                    };
                    _entranceDBContext.ApplicationPrograms.RemoveRange(programsToRemove);
                    _entranceDBContext.Applications.Remove(application);
                }
            }
            await _entranceDBContext.SaveChangesAsync();
        }
    }
}
