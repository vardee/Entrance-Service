using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;

[Route("Entrance")]
public class EntranceController : Controller
{
    private readonly ILogger<EntranceController> _logger;
    private readonly IBus _bus;
    private readonly TokenHelper _tokenHelper;

    public EntranceController(ILogger<EntranceController> logger, TokenHelper tokenHelper)
    {
        _logger = logger;
        _bus = RabbitHutch.CreateBus("host=localhost");
        _tokenHelper = tokenHelper;
    }

    [HttpGet]
    public async Task<IActionResult> GetApplications()
    {
        var model = new GetApplicationsFilter();
        var viewModel = await FetchApplications(model);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("GettingApplications")]
    public async Task<IActionResult> GettingApplications(GetApplicationsFilter model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            Console.WriteLine(model.haveManager);
            Console.WriteLine(model.isMy);
            Console.WriteLine(model.size);
            var viewModel = await FetchApplications(model);
            return View("GetApplications", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during application fetch process");
            ModelState.AddModelError("", "Error fetching applications.");
            return View(model);
        }
    }

    [HttpPost]
    [Route("TakeApplicantApplication")]
    public async Task<IActionResult> TakeApplicantApplication(Guid applicationId)
    {
        try
        {
            Guid currentId = GetCurrentManager();
            var request = new TakeApplicationMVCDTO
            {
                UserId = currentId,
                ApplicationId = applicationId,
            };
            await _bus.PubSub.PublishAsync(request, "takeApplication");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during taking application");
            return Json(new { success = false, message = "Error taking application." });
        }
    }

    [HttpPost]
    [Route("RejectApplicantApplication")]
    public async Task<IActionResult> RejectApplicantApplication(Guid applicationId)
    {
        try
        {
            Guid currentId = GetCurrentManager();
            var request = new RejectApplicationMVCDTO
            {
                UserId = currentId,
                ApplicationId = applicationId,
            };
            await _bus.PubSub.PublishAsync(request, "rejectApplication");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during rejecting application");
            return Json(new { success = false, message = "Error rejecting application." });
        }
    }

    [HttpPost]
    [Route("ChangeApplicationStatus")]
    public async Task<IActionResult> ChangeApplicationStatus(Guid applicationId, EntranceApplicationStatus status)
    {
        try
        {
            Guid currentId = GetCurrentManager();
            var request = new ChangeApplicationStatusMVCDTO
            {
                UserId = currentId,
                ApplicationId = applicationId,
                Status = status,
            };
            await _bus.PubSub.PublishAsync(request, "changeApplicationStatusMVC");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during rejecting application");
            return Json(new { success = false, message = "Error rejecting application." });
        }
    }


    private Guid GetCurrentManager()
    {
        var token = _tokenHelper.GetTokenFromSession();
        var id = _tokenHelper.GetUserIdFromToken(token);
        var userId = Guid.Parse(id);
        return userId;
    }

    private async Task<ApplicationFilterViewModel> FetchApplications(GetApplicationsFilter model)
    {
        try
        {
                Console.WriteLine(model.size);
            Console.WriteLine(model.haveManager);
            Console.WriteLine(model.isMy);
            var applicationDto = new GetApplicationsMVCDTO
                {
                    size = model.size,
                    page = model.page,
                    entranceApplicationStatuses = model.entranceApplicationStatuses,
                    managerId = model.managerId,
                    haveManager = model.haveManager,
                    isMy = model.isMy,
                    Faculties = model.Faculties,
                    name = model.name,
                    ProgramId = model.ProgramId,
                    timeSorting = model.timeSorting,
                };
                var response = await _bus.Rpc.RequestAsync<GetApplicationsMVCDTO, GetAllQuerybleApplicationsDTO>(applicationDto, c => c.WithQueueName("getApplicationsMVC"));

                Guid currentManagerId = GetCurrentManager();

                var viewModel = new ApplicationFilterViewModel
                {
                    Filter = model,
                    Applications = response.Applications.Select(a => new ApplicationModel
                    {
                        ApplicationId = a.ApplicationId,
                        ApplicationStatus = a.ApplicationStatus,
                        ApplicantFullName = a.ApplicantFullName,
                        ManagerId = a.ManagerId
                    }).ToList(),
                    CurrentManagerId = currentManagerId
                };

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching applications");
            throw; 
        }
    }
}
