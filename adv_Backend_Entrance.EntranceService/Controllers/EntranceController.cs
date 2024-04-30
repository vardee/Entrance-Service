using adv_Backend_Entrance.Common.Data.Models;
using adv_Backend_Entrance.Common.DTO.EntranceService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.Common.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance.EntranceService.Controllers
{
    [ApiController]
    [Route("entrance")]
    public class EntranceController:ControllerBase
    {
        private readonly IEntranceService _entranceService;
        private readonly TokenHelper _tokenHelper;
        public EntranceController(IEntranceService entranceService, TokenHelper tokenHelper)
        {
            _entranceService = entranceService;
            _tokenHelper = tokenHelper;
        }
        [HttpPost]
        [Route("statement")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> CreateApplication([FromBody] CreateApplicationDTO createApplicationDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _entranceService.CreateApplication(createApplicationDTO, token);
            return Ok();
        }
        [HttpPost]
        [Route("education/information")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> AddEducation([FromBody] AddEducationLevelDTO addEducationLevelDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _entranceService.AddEducationLevel(addEducationLevelDTO, token);
            return Ok();
        }
        [HttpPost]
        [Route("passport/information")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> AddPassport([FromBody] AddPassportDTO addPassportDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _entranceService.AddPassport(addPassportDTO, token);
            return Ok();
        }
    }
}
