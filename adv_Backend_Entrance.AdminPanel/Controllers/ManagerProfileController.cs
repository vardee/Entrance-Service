using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Helpers;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.AspNetCore.Authorization;

namespace adv_Backend_Entrance.AdminPanel.Controllers
{
    [Route("ManagerProfile")]
    [Authorize(Roles = "Admin")]
    public class ManagerProfileController : Controller
    {
        private readonly ILogger<ManagerProfileController> _logger;
        private readonly IBus _bus;
        private readonly TokenHelper _tokenHelper;

        public ManagerProfileController(ILogger<ManagerProfileController> logger, TokenHelper tokenHelper)
        {
            _logger = logger;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _tokenHelper = tokenHelper;
        }

        [HttpGet]
        public async Task<IActionResult> ManagerProfile(Guid managerId)
        {
            var response = await _bus.Rpc.RequestAsync<Guid, GetAllQuerybleManagersDTO>(managerId, c => c.WithQueueName("getManagerInformationMVC"));
            var currentManager = response.Managers.FirstOrDefault(m => m.ManagerId == managerId);
            var request = new GetUserProfileMVCDTO
            {
                UserId = currentManager.UserId
            };
            var managerProfile = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
            var viewModel = new ManagerProfileViewModel
            {
                Id = managerId,
                Email = managerProfile.Email,
                BirthDate = managerProfile.BirthDate,
                FirstName = managerProfile.firstname,
                LastName = managerProfile.lastname,
                Patronymic = managerProfile.patronymic,
                Phone = managerProfile.Phone,
            };
            return PartialView("ManagerProfile", viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditManagerProfile")]
        public async Task<IActionResult> EditManagerProfile(ManagerProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ManagerProfile", model);
            }
            try
            {
                var response = await _bus.Rpc.RequestAsync<Guid, GetAllQuerybleManagersDTO>(model.Id, c => c.WithQueueName("getManagerInformationMVC"));
                var currentManager = response.Managers.FirstOrDefault(m => m.ManagerId == model.Id);

                var editProfile = new EditManagerProfileInformationMVCDTO
                {
                    UserId = currentManager.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Patronymic = model.Patronymic,
                    BirthDate = DateTime.SpecifyKind(model.BirthDate, DateTimeKind.Utc),
                    Phone = model.Phone,
                    Email = model.Email
                };
                await _bus.PubSub.PublishAsync(editProfile, "managerEditProfileMVC");
                var request = new GetUserProfileMVCDTO
                {
                    UserId = currentManager.UserId
                };
                var managerProfile = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
                var viewModel = new ManagerProfileViewModel
                {
                    Id = model.Id,
                    Email = managerProfile.Email,
                    BirthDate = managerProfile.BirthDate,
                    FirstName = managerProfile.firstname,
                    LastName = managerProfile.lastname,
                    Patronymic = managerProfile.patronymic,
                    Phone = managerProfile.Phone,
                };
                return PartialView("ManagerProfile", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during profile edit process");
                ModelState.AddModelError("", "Error during profile edit.");
                return PartialView("ManagerProfile", model);
            }
        }
    }
}
