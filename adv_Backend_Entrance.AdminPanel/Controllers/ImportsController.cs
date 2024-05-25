using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance.AdminPanel.Controllers
{
    [Route("Imports")]
    public class ImportsController : Controller
    {
        private readonly ILogger<ImportsController> _logger;
        private readonly IBus _bus;
        private readonly TokenHelper _tokenHelper;

        public ImportsController(ILogger<ImportsController> logger, TokenHelper tokenHelper)
        {
            _logger = logger;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _tokenHelper = tokenHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Imports()
        {
            var model = new ImportsFilterModel();
            var viewModel = await FetchImports(model);
            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("GettingImports")]
        public async Task<IActionResult> GettingImports(ImportsFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Imports", model);
            }

            try
            {
                var viewModel = await FetchImports(model);
                return View("Imports", viewModel);
            }
            catch (Exception ex)
            {
                return View("Imports", model);
            }
        }
        [HttpPost]
        [Route("ImportInformation")]
        public async Task<IActionResult> ImportInformation(List<ImportType> types)
        {
            try
            {
                var request = new ImportInfoMVCDTO
                {
                    Types = types,
                };
                await _bus.PubSub.PublishAsync(request, "importInfoMVCDTO");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during import");
                return Json(new { success = false, message = "Error during import" });
            }
        }

        private async Task<ImportsPageViewModel> FetchImports(ImportsFilterModel model)
        {
            try
            {
                var importsDto = new GetImportMVCDTO
                {
                    Size = model.size,
                    Types = model.Types,
                };

                var response = await _bus.Rpc.RequestAsync<GetImportMVCDTO, GetAllQuerybleImportsDTO>(importsDto, c => c.WithQueueName("gettingImportsMVCDTO"));
                var viewModel = new ImportsPageViewModel
                {
                    Imports = response.Imports.Select(u => new ImportsModel
                    {
                        ImportWas = u.ImportWas,
                        Status = u.Status,
                        Type = u.Type,
                    }).ToList(),
                    Filter = model,
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
