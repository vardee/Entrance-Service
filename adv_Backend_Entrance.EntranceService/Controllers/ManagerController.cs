using adv_Backend_Entrance.Common.Data.Models;
using adv_Backend_Entrance.Common.DTO.EntranceService.Applicant;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.EntranceService;
using adv_Backend_Entrance.Common.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance.EntranceService.Controllers
{
    [ApiController]
    [Route("entrance")]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;
        private readonly TokenHelper _tokenHelper;
        public ManagerController(IManagerService managerService, TokenHelper tokenHelper)
        {
            _managerService = managerService;
            _tokenHelper = tokenHelper;
        }
        [HttpPost]
        [Route("application/take")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Manager,MainManager,Administrator")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> TakeApplication([FromBody] TakeApplicationDTO takeApplicationDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            await _managerService.TakeApplication(takeApplicationDTO, userId);
            return Ok();
        }

        [HttpPost]
        [Route("application/reject")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Manager,MainManager,Administrator")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> RejectApplication([FromBody] RejectApplicationDTO rejectApplicationDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            await _managerService.RejectApplication(rejectApplicationDTO, userId);
            return Ok();
        }
        [HttpPost]
        [Route("application/status")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Manager,MainManager,Administrator")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> ChangeStatus([FromQuery] ChangeApplicationStatusDTO changeApplicationStatusDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            await _managerService.ChangeApplicationStatus(changeApplicationStatusDTO, userId);
            return Ok();
        }
        [HttpGet]
        [Route("applications")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Manager,MainManager,Administrator")]
        [ProducesResponseType(typeof(GetAllQuerybleApplicationsDTO),200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetAllQuerybleApplicationsDTO>> GetApplications([FromQuery] Sorting? timeSorting, [FromQuery] string? name, [FromQuery] Guid? ProgramId, [FromQuery] List<Guid>? Faculties, [FromQuery] List<EntranceApplicationStatus>? entranceApplicationStatuses, bool haveManager, bool isMy,int size = 10, int page = 1)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            var result = await _managerService.GetQuerybleApplications(size, page,name,ProgramId,Faculties,entranceApplicationStatuses,haveManager,isMy, userId, timeSorting);
            return Ok(result);
        }
        [HttpGet]
        [Route("application/applicant")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Manager,MainManager,Administrator")]
        [ProducesResponseType(typeof(GetApplicationsDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetApplicationsDTO>> GetApplications([FromQuery] GetApplicantDTO getApplicantDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            var result = await _managerService.GetApplicantion(getApplicantDTO);
            return Ok(result);
        }
        [HttpGet]
        [Route("applicant")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Manager,MainManager,Administrator")]
        [ProducesResponseType(typeof(GetApplicantInformationDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetApplicantInformationDTO>> GetApplicant([FromQuery] GetApplicantDTO getApplicantDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            var result = await _managerService.GetApplicantInformation(getApplicantDTO);
            return Ok(result);
        }
        [HttpGet]
        [Route("managers")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "MainManager,Administrator")]
        [ProducesResponseType(typeof(GetAllQuerybleManagersDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetAllQuerybleManagersDTO>> GetManagers([FromQuery] string? name, [FromQuery] RoleType? roleType, [FromQuery] int size = 10, [FromQuery] int page = 1)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            var result = await _managerService.GetManagers(size,page,name,roleType);
            return Ok(result);
        }
    }
}
