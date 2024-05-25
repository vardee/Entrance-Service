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

namespace adv_Backend_Entrance.EntranceService.BL.Services
{
    public class ManagerHelperService
    {
        private readonly EntranceDBContext _entranceDBContext;
        private readonly TokenHelper _tokenHelper;
        public ManagerHelperService(EntranceDBContext entranceDBContext, TokenHelper tokenHelper)
        {
            _entranceDBContext = entranceDBContext;
            _tokenHelper = tokenHelper;
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
            if(sameRole != null)
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
        }
    }
}
