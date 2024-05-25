using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Helpers;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.ApplicantService;

namespace adv_Backend_Entrance.AdminPanel.Controllers
{
    [Route("ApplicantProfile")]
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
            Console.WriteLine(userId);
            var viewModel = new EditApplicantProfileModel
            {
                Id = userId,
                Email = response.Email,
                BirthDate = response.BirthDate,
                FirstName = response.firstname,
                LastName = response.lastname,
                Patronymic = response.patronymic,
                Phone = response.Phone,
            };
            Console.WriteLine(viewModel.Id);
            

            return View("ApplicantProfile", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditApplicantProfile")]
        public async Task<IActionResult> EditApplicantProfile(EditApplicantProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ApplicantProfile", model);
            }
            try
            {
                var response = await _bus.Rpc.RequestAsync<Guid, GetApplicantInformationDTO>(model.Id, c => c.WithQueueName("getApplicantInformationMVC"));
                var editProfile = new EditApplicantProfileInformationDTO
                {
                    UserId = response.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Patronymic = model.Patronymic,
                    BirthDate = DateTime.SpecifyKind(model.BirthDate, DateTimeKind.Utc),
                    Phone = model.Phone,
                    Email = model.Email
                };
                await _bus.PubSub.PublishAsync(editProfile, "applicantEditProfileMVC");
                var request = new GetUserProfileMVCDTO
                {
                    UserId = response.UserId
                };
                var applicantProfile = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
                var viewModel = new EditApplicantProfileModel
                {
                    Email = applicantProfile.Email,
                    BirthDate = applicantProfile.BirthDate,
                    FirstName = applicantProfile.firstname,
                    LastName = applicantProfile.lastname,
                    Patronymic = applicantProfile.patronymic,
                    Phone = applicantProfile.Phone,
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
