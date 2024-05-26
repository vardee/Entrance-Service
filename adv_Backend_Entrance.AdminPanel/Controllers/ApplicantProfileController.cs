using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Helpers;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using Microsoft.AspNetCore.Authorization;
using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Controllers
{
    [Route("ApplicantProfile")]
    [Authorize(Roles = "Admin,MainManager,Manager")]
    public class ApplicantProfileController : Controller
    {
        private readonly ILogger<ApplicantProfileController> _logger;
        private readonly IBus _bus;
        private readonly TokenHelper _tokenHelper;

        public ApplicantProfileController(ILogger<ApplicantProfileController> logger, TokenHelper tokenHelper)
        {
            _logger = logger;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _tokenHelper = tokenHelper;
        }

        [HttpGet]
        public async Task<IActionResult> ApplicantProfile(Guid userId)
        {
            var applicantInfo = await _bus.Rpc.RequestAsync<Guid, GetApplicantInformationDTO>(userId, c => c.WithQueueName("getApplicantInformationMVC"));
            var request = new GetUserProfileMVCDTO
            {
                UserId = applicantInfo.UserId,
            };
            var response = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
            var applicantManager = await _bus.Rpc.RequestAsync<Guid, GetManagerIdMVCDTO>(userId, c => c.WithQueueName("getApplicantManagerMVC"));
            var token = _tokenHelper.GetTokenFromSession();
            var IdFromToken = _tokenHelper.GetUserIdFromToken(token);
            var roles = _tokenHelper.GetRolesFromToken(token).Select(r => (RoleType)Enum.Parse(typeof(RoleType), r)).ToList();
            Guid currentManager = applicantManager.ManagerId;
            Guid currentPerson = Guid.Parse(IdFromToken);

            var viewModel = new EditApplicantProfileModel
            {
                Id = userId,
                Email = response.Email,
                BirthDate = response.BirthDate,
                FirstName = response.firstname,
                LastName = response.lastname,
                Patronymic = response.patronymic,
                Phone = response.Phone,
                Nationality = response.Nationality,
                Gender = response.Gender,
                CurrentManager = currentManager,
                Person = currentPerson,
                Roles = roles
            };
            Console.WriteLine(viewModel.Id);
            

            return View("ApplicantProfile", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditApplicantProfile")]
        public async Task<IActionResult> EditApplicantProfile(EditApplicantProfileModel model)
        {
            try
            {
                var response = await _bus.Rpc.RequestAsync<Guid, GetApplicantInformationDTO>(model.Id, c => c.WithQueueName("getApplicantInformationMVC"));
                var applicantManager = await _bus.Rpc.RequestAsync<Guid, GetManagerIdMVCDTO>(response.Id, c => c.WithQueueName("getApplicantManagerMVC"));
                var token = _tokenHelper.GetTokenFromSession();
                var IdFromToken = _tokenHelper.GetUserIdFromToken(token);
                var roles = _tokenHelper.GetRolesFromToken(token).Select(r => (RoleType)Enum.Parse(typeof(RoleType), r)).ToList();
                Guid currentManager = applicantManager.ManagerId;
                Guid currentPerson = Guid.Parse(IdFromToken);

                var editProfile = new EditApplicantProfileInformationDTO
                {
                    UserId = response.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Patronymic = model.Patronymic,
                    Nationality = model.Nationality,
                    BirthDate = DateTime.SpecifyKind(model.BirthDate, DateTimeKind.Utc),
                    Phone = model.Phone,
                    Email = model.Email,
                    Gender = model.Gender,
                };
                await _bus.PubSub.PublishAsync(editProfile, "applicantEditProfileMVC");
                var request = new GetUserProfileMVCDTO
                {
                    UserId = response.UserId
                };
                var applicantProfile = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
                var viewModel = new EditApplicantProfileModel
                {
                    Id = model.Id,
                    Email = applicantProfile.Email,
                    BirthDate = applicantProfile.BirthDate,
                    FirstName = applicantProfile.firstname,
                    LastName = applicantProfile.lastname,
                    Patronymic = applicantProfile.patronymic,
                    Phone = applicantProfile.Phone,
                    Nationality = applicantProfile.Nationality,
                    Gender = applicantProfile.Gender,
                    CurrentManager = currentManager,
                    Person = currentPerson,
                    Roles = roles
                };
                return PartialView("ApplicantProfile", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during profile edit process");
                ModelState.AddModelError("", "Error during profile edit.");
                return PartialView("ApplicantProfile", model);
            }
        }
    }
}
