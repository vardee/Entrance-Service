using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace adv_Backend_Entrance.AdminPanel.Controllers
{
    [Route("Users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IBus _bus;
        private readonly TokenHelper _tokenHelper;

        public UsersController(ILogger<UsersController> logger, TokenHelper tokenHelper)
        {
            _logger = logger;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _tokenHelper = tokenHelper;
        }

        [HttpGet]
        [HttpGet("AllUsers")]
        public async Task<IActionResult> AllUsers()
        {
            var model = new UsersFilterModel();
            var viewModel = await FetchUsers(model);
            return View(viewModel);
        }

        private Guid GetCurrentManager()
        {
            var token = _tokenHelper.GetTokenFromSession();
            var id = _tokenHelper.GetUserIdFromToken(token);
            var userId = Guid.Parse(id);
            return userId;
        }

        [HttpPost]
        [Route("AddRoleUser")]
        public async Task<IActionResult> AddRoleUser(Guid userId, RoleType role)
        {
            try
            {
                var request = new AddRoleUserMVCDTO
                {
                    UserId = userId,
                    Role = role
                };
                await _bus.PubSub.PublishAsync(request, "addRoleToUserMVC");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ADDDING ROLE TO uSER");
                return Json(new { success = false, message = "Error ADDDING ROLE TO uSER" });
            }
        }
        [HttpPost]
        [Route("RemoveRoleUser")]
        public async Task<IActionResult> RemoveRoleUser(Guid userId, RoleType role)
        {
            try
            {
                var request = new RemoveRoleUserMVCDTO
                {
                    UserId = userId,
                    Role = role
                };
                await _bus.PubSub.PublishAsync(request, "removeRoleToUserMVC");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ADDDING ROLE TO uSER");
                return Json(new { success = false, message = "Error ADDDING ROLE TO uSER" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("GettingUsers")]
        public async Task<IActionResult> GettingUsers(UsersFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AllUsers", model);
            }

            try
            {
                var viewModel = await FetchUsers(model);
                return View("AllUsers", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during application fetch process");
                ModelState.AddModelError("", "Error fetching applications.");
                return View("AllUsers", model); 
            }
        }

        private async Task<UsersPageViewModel> FetchUsers(UsersFilterModel model)
        {
            try
            {
                var token = _tokenHelper.GetTokenFromSession();
                var roles = _tokenHelper.GetRolesFromToken(token).Select(r => (RoleType)Enum.Parse(typeof(RoleType), r)).ToList();
                var usersDto = new GetUsersMVCDTO
                {
                    Size = model.Size,
                    Page = model.Page,
                    Email = model.Email,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                };

                var response = await _bus.Rpc.RequestAsync<GetUsersMVCDTO, GetUsersPageDTO>(usersDto, c => c.WithQueueName("gettingUsers_withMvc"));
                Guid myId = GetCurrentManager();
                var viewModel = new UsersPageViewModel
                {
                    Users = response.Users.Select(u => new UsersModel
                    {
                        Email = u.Email,
                        BirthDate = u.BirthDate,
                        FullName = u.FullName,
                        Gender = u.Gender,
                        Id = u.Id,
                        LastName = u.LastName,
                        Patronymic = u.Patronymic,
                        Roles = u.Roles
                    }).ToList(),
                    Filters = model,
                    CurrentId = myId,
                    Roles = roles.ToList(),
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                throw;
            }
        }


    }
}
