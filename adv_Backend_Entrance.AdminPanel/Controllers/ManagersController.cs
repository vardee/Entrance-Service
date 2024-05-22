using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Helpers;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance.AdminPanel.Controllers
{
    [Route("Managers")]
    public class ManagersController : Controller
    {
        private readonly ILogger<ManagersController> _logger;
        private readonly IBus _bus;
        private readonly TokenHelper _tokenHelper;

        public ManagersController(ILogger<ManagersController> logger, TokenHelper tokenHelper)
        {
            _logger = logger;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _tokenHelper = tokenHelper;
        }

        [HttpGet]
        [HttpGet("AllManagers")]
        public async Task<IActionResult> AllManagers()
        {
            var model = new ManagerFilterModel();
            var viewModel = await FetchManagers(model);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("GettingManagers")]
        public async Task<IActionResult> GettingManagers(ManagerFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AllManagers", model);
            }

            try
            {
                var viewModel = await FetchManagers(model);
                return View("AllManagers", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during application fetch process");
                ModelState.AddModelError("", "Error fetching applications.");
                return View("AllManagers", model);
            }
        }

        private Guid GetCurrentManager()
        {
            var token = _tokenHelper.GetTokenFromSession();
            var id = _tokenHelper.GetUserIdFromToken(token);
            var userId = Guid.Parse(id);
            return userId;
        }
        private async Task<ManagersViewModel> FetchManagers(ManagerFilterModel model)
        {
            try
            {
                var managersDto = new GetManagersMVCDTO
                {
                    Size = model.Size,
                    Page = model.Page,
                    Name = model.Name,
                    Role = model.Role
                };

                var response = await _bus.Rpc.RequestAsync<GetManagersMVCDTO, GetAllQuerybleManagersDTO>(managersDto, c => c.WithQueueName("gettingManagers_withMvc"));
                Guid myId = GetCurrentManager();
                var viewModel = new ManagersViewModel
                {
                    Manager = response.Managers.Select(u => new ManagerModel
                    {
                        Email = u.Email,
                        FullName = u.FullName,
                        ManagerId= u.ManagerId,
                        Role = u.Role,
                    }).ToList(),
                    Filter = model,
                    CurrentId = myId,
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
