using adv_Backend_Entrance.Common.Helpers;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance.AdminPanel.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IBus _bus;
        private readonly TokenHelper _tokenHelper;

        public UsersController(ILogger<AccountController> logger, TokenHelper tokenHelper)
        {
            _logger = logger;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _tokenHelper = tokenHelper;
        }

        [HttpGet]
        public IActionResult AllUsers()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AllManagers()
        {
            return View();
        }
    }
}
