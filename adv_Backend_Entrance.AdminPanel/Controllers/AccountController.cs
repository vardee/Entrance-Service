using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.UserService;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using NuGet.Protocol.Core.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace adv_Backend_Entrance.AdminPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IBus _bus;
        private readonly TokenHelper _tokenHelper;

        public AccountController(ILogger<AccountController> logger, TokenHelper tokenHelper)
        {
            _logger = logger;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _tokenHelper = tokenHelper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,MainManager,Manager")]
        public async Task<IActionResult> EditProfile()
        {
            var token = _tokenHelper.GetTokenFromSession();
            var id = _tokenHelper.GetUserIdFromToken(token);
            var userId = Guid.Parse(id);
            var request = new GetUserProfileMVCDTO
            {
                UserId = userId
            };
            var response = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
            var viewModel = new EditProfileModel
            {
                Email = response.Email,
                BirthDate = response.BirthDate,
                FirstName = response.firstname,
                LastName = response.lastname,
                Patronymic = response.patronymic,
                Phone = response.Phone,
            };
            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,MainManager,Manager")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            try
            {
                Console.WriteLine(login.Email);
                var loginDto = new UserLoginDTO { Email = login.Email, Password = login.Password };
                var response = await _bus.Rpc.RequestAsync<UserLoginDTO, TokenResponseDTO>(loginDto, c => c.WithQueueName("userLogin_token"));

                TokenResponseDTO loginResponse = response;
                HttpContext.Session.SetString("AccessToken", loginResponse.AccessToken);
                HttpContext.Session.SetString("RefreshToken", loginResponse.RefreshToken);
                return Json(new { redirectUrl = "/Home/Index" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(login);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,MainManager,Manager")]
        public async Task<IActionResult> EditProfile(EditProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var token = _tokenHelper.GetTokenFromSession();
                var id = _tokenHelper.GetUserIdFromToken(token);
                var userId = Guid.Parse(id);
                var editProfile = new EditApplicantProfileInformationDTO
                {
                    UserId = userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Patronymic = model.Patronymic,
                    BirthDate = DateTime.SpecifyKind(model.BirthDate, DateTimeKind.Utc),
                    Phone = model.Phone,
                    Email = model.Email
                };
                var request = new GetUserProfileMVCDTO
                {
                    UserId = userId
                };
                await _bus.PubSub.PublishAsync(editProfile, "editUserProfile");
                var response = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
                var viewModel = new EditProfileModel
                {
                    Email = response.Email,
                    BirthDate = response.BirthDate,
                    FirstName = response.firstname,
                    LastName = response.lastname,
                    Patronymic = response.patronymic,
                    Phone = response.Phone,
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during profile edit process");
                ModelState.AddModelError("", "Error during profile edit.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var token = _tokenHelper.GetTokenFromSession();
            var logoutdto = new LogoutDTO
            {
                Token = token,
            };
            await _bus.PubSub.PublishAsync(logoutdto, "logoutUser");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,MainManager,Manager")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var token = _tokenHelper.GetTokenFromSession();
                var id = _tokenHelper.GetUserIdFromToken(token);
                var userId = Guid.Parse(id);
                var changePassword = new ChangePasswordMVCDTO
                {
                    ConfirmPassword = model.ConfirmPassword,
                    OldPasssword = model.OldPassword,
                    Password = model.Password,
                    UserId = userId
                };
                await _bus.PubSub.PublishAsync(changePassword, "changePassword");
                return Json(new { redirectUrl = "/Account/ChangePassword" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change process");
                ModelState.AddModelError("", "Error during password change.");
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,MainManager,Manager")]
        public IActionResult CheckSession()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var refreshToken = HttpContext.Session.GetString("RefreshToken");

            if (accessToken != null && refreshToken != null)
            {
                return Content($"AccessToken: {accessToken}, RefreshToken: {refreshToken}");
            }
            else
            {
                return Content("No tokens in session.");
            }
        }
    }
}
