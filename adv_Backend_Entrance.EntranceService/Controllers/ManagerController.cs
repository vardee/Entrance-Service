using adv_Backend_Entrance.Common.Data.Models;
using adv_Backend_Entrance.Common.DTO.EntranceService.Applicant;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
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
        [Authorize(Roles = "User")]
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

    }
}
