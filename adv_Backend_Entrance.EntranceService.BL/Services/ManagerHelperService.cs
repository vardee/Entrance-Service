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
                if (editApplicantProfileInformation.LastName != "")
                {
                    applicant.Patronymic = editApplicantProfileInformation.Patronymic;
                }
                if (editApplicantProfileInformation.Nationality != "")
                {
                    applicant.Nationality = editApplicantProfileInformation.Nationality;
                }
            }
            else
            {
                throw new NotFoundException("This applicant not found!");
            }
            await _entranceDBContext.SaveChangesAsync();
        }
    }
}
