using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.FacultyService;
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
        var faculties = await GetFaculties();
        var programs = await GetPrograms();
        var model = new GetApplicationsFilter();
        if (faculties.Any() && faculties != null)
        {
            foreach (var faculty in faculties)
            {
                model.FacultyId = faculty.id;
                var newFaculty = new FacultiesModel
                {
                    Id = faculty.id,
                    Name = faculty.name,
                };
                model.Faculties.Add(newFaculty);
            }
        }
        if (programs.Programs.Any() && programs.Programs != null)
        {
            foreach (var program in programs.Programs)
            {
                model.ProgramId = program.id;
                var newProgram = new ProgramModel
                {
                    ProgramId = program.id,
                    Name = program.name,
                };
                model.Programs.Add(newProgram);
            }
        }
        var managers = await GetManagers();
        if (managers.Managers.Any() && managers.Managers != null)
        {
            foreach (var manager in managers.Managers)
            {
                model.managerId = manager.ManagerId;
                var newManager = new ManagerModel
                {
                    Email = manager.Email,
                    ManagerId = manager.ManagerId,
                };
                model.Managers.Add(newManager);
            }
        }

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
    [HttpPost]
    [Route("ChangeApplicationManager")]
    public async Task<IActionResult> ChangeApplicationManager(Guid applicationId, Guid managerId)
    {
        try
        {
            Guid currentId = GetCurrentManager();
            var request = new ChangeManagerMVCDTO
            {
                ManagerId = managerId,
                ApplicationId = applicationId,
            };
            await _bus.PubSub.PublishAsync(request, "changeApplicationManagerMVC");
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
            var facultyIds = model.Faculties?.Select(f => f.Id).ToList() ?? new List<Guid>();

            var applicationDto = new GetApplicationsMVCDTO
            {
                size = model.size,
                page = model.page,
                entranceApplicationStatuses = model.entranceApplicationStatuses,
                managerId = model.managerId,
                haveManager = model.haveManager,
                isMy = model.isMy,
                Faculties = facultyIds,
                name = model.name,
                ProgramId = model.ProgramId,
                timeSorting = model.timeSorting,
            };

            var response = await _bus.Rpc.RequestAsync<GetApplicationsMVCDTO, GetAllQuerybleApplicationsDTO>(applicationDto, c => c.WithQueueName("getApplicationsMVC"));

            Guid currentManagerId = GetCurrentManager();
            var managersResponse = await GetManagers();
            var managers = managersResponse.Managers.Select(m => new ManagerModel
            {
                ManagerId = m.ManagerId,
                FullName = m.FullName,
                Email = m.Email,
                Role = m.Role
            }).ToList();

            var viewModel = new ApplicationFilterViewModel
            {
                Filter = model,
                Applications = response.Applications.Select(a => new ApplicationModel
                {
                    ApplicationId = a.ApplicationId,
                    ApplicationStatus = a.ApplicationStatus,
                    ApplicantFullName = a.ApplicantFullName,
                    ManagerId = a.ManagerId,
                    ManagerEmail = a.ManagerEmail,
                    ManagerModels = managers
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




    private async Task<List<GetFacultiesDTO>> GetFaculties()
    {
        Guid currentManagerId = GetCurrentManager();
        var response = await _bus.Rpc.RequestAsync<Guid, List<GetFacultiesDTO>>(currentManagerId, c => c.WithQueueName("getFacultiesMVC"));
        return response;
    }
    private async Task<GetQuerybleProgramsDTO> GetPrograms()
    {
        Guid currentManagerId = GetCurrentManager();
        var response = await _bus.Rpc.RequestAsync<Guid, GetQuerybleProgramsDTO>(currentManagerId, c => c.WithQueueName("getProgramsForAppMVC"));
        return response;
    }
    private async Task<GetAllQuerybleManagersDTO> GetManagers()
    {
        Guid currentManagerId = GetCurrentManager();
        var response = await _bus.Rpc.RequestAsync<Guid, GetAllQuerybleManagersDTO>(currentManagerId, c => c.WithQueueName("gettingForFilterManagers_withMvc"));
        return response;
    }

}
