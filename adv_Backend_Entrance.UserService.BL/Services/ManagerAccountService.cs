using adv_Backend_Entrance.Common.Data;
using adv_Backend_Entrance.Common.Interfaces.UserService;
using adv_Backend_Entrance.UserService.DAL.Data.Entities;
using adv_Backend_Entrance.UserService.DAL.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Helpers.Validators;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using EasyNetQ;
using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.UserService.BL.Services
{
    public class ManagerAccountService : IManagerAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AuthDbContext _authDBContext;
        private readonly RedisDBContext _redisDBContext;
        private readonly IConfiguration _configuration;
        private readonly AdditionTokenService _additionTokenService;
        private readonly IBus _bus;


        public ManagerAccountService(UserManager<User> userManager, SignInManager<User> signInManager, AuthDbContext authDbContext, IConfiguration configuration, RedisDBContext redisDBContext, AdditionTokenService additionTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authDBContext = authDbContext;
            _configuration = configuration;
            _redisDBContext = redisDBContext;
            _additionTokenService = additionTokenService;
            _bus = RabbitHutch.CreateBus("host=localhost");
        }
        public async Task EditApplicantProfile(EditApplicantProfileInformationDTO editApplicantProfileInformationDTO)
        {
            if (editApplicantProfileInformationDTO.UserId != null)
            {
                var user = await _userManager.FindByIdAsync(editApplicantProfileInformationDTO.UserId.ToString());
                if (user != null)
                {
                    if (editApplicantProfileInformationDTO.FirstName != "")
                    {
                        user.FullName = editApplicantProfileInformationDTO.FirstName;
                    }
                    if (editApplicantProfileInformationDTO.LastName != "")
                    {
                        user.LastName = editApplicantProfileInformationDTO.LastName;
                    }
                    if (editApplicantProfileInformationDTO.Email != "")
                    {
                        user.Email = editApplicantProfileInformationDTO.Email;
                    }
                    if (editApplicantProfileInformationDTO.Phone != "")
                    {
                        user.PhoneNumber = editApplicantProfileInformationDTO.Phone;
                    }
                    if (editApplicantProfileInformationDTO.BirthDate != null)

                        if (!BirthDateValidator.ValidateDateOfBirth(editApplicantProfileInformationDTO.BirthDate))
                        {
                            throw new BadRequestException("Неверная дата рождения. Вам должно быть не менее 13 лет и не более 100 лет.");
                        }
                    {
                        user.BirthDate = (DateTime)editApplicantProfileInformationDTO.BirthDate;
                    }
                    if (editApplicantProfileInformationDTO.Patronymic != "")
                    {
                        user.Patronymic = editApplicantProfileInformationDTO.Patronymic;
                    }
                    await _authDBContext.SaveChangesAsync();
                    await SyncApplicantProfile(editApplicantProfileInformationDTO);

                }
                else
                {
                    throw new NotFoundException("User is not found");
                }
            }
            else
            {
                throw new UnauthorizedException("User is not authorized");
            }
        }
        private async Task SyncApplicantProfile(EditApplicantProfileInformationDTO editApplicantProfile)
        {
            var message = new EditApplicantProfileInformationDTO 
            {
                Email = editApplicantProfile.Email, 
                BirthDate = editApplicantProfile.BirthDate,
                Phone = editApplicantProfile.Phone,
                FirstName = editApplicantProfile.FirstName,
                LastName = editApplicantProfile.LastName,
                Patronymic = editApplicantProfile.Patronymic,
                Nationality = editApplicantProfile.Nationality,
                UserId = editApplicantProfile.UserId
            };
            await _bus.PubSub.PublishAsync(message, "applicantProfile_edit");
        }
    }
}
