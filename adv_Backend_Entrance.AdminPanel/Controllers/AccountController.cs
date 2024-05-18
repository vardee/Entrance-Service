using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.UserService;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.Helpers;

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
        public IActionResult EditProfile()
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
                var loginDto = new UserLoginDTO { Email = login.Email, Password = login.Password };
                var response = await _bus.Rpc.RequestAsync<UserLoginDTO, TokenResponseDTO>(loginDto, c => c.WithQueueName("userLogin_token"));

                TokenResponseDTO loginResponse = response;
                HttpContext.Session.SetString("AccessToken", loginResponse.AccessToken);
                HttpContext.Session.SetString("RefreshToken", loginResponse.RefreshToken);

                return RedirectToAction("Index", "Home");
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
        public async Task<IActionResult> EditProfile(EditProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var token =  _tokenHelper.GetTokenFromSession();
                Console.WriteLine(token);
                var id = _tokenHelper.GetUserIdFromToken(token);
                Console.WriteLine(id);
                var userId = Guid.Parse(id);
                var editProfile = new EditApplicantProfileInformationDTO { UserId = userId, FirstName = model.FirstName, LastName = model.LastName, Patronymic = model.Patronymic, BirthDate = model.BirthDate, Phone = model.Phone, Email = model.Email };
                await _bus.PubSub.PublishAsync(editProfile, "editUserProfile");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
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
