using adv_Backend_Entrance.Common.Data.Models;
using adv_Backend_Entrance.Common.DTO;
using adv_Backend_Entrance.Common.DTO.EntranceService.Applicant;
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
        [HttpDelete]
        [Route("statement")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Applicant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> DeleteProgramFromApplication([FromBody] DeleteProgramFromApplicationDTO deleteProgramFromApplicationDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _entranceService.DeleteProgramFromApplication(deleteProgramFromApplicationDTO, token);
            return Ok();
        }
        [HttpPut]
        [Route("statement/programs")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Applicant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> AddProgramInApplication([FromBody] AddProgramsDTO addProgramsDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _entranceService.AddProgramsInApplication(addProgramsDTO, token);
            return Ok();
        }
        [HttpPut]
        [Route("statement/programs/priority")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Applicant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> ChangeProgramPriority([FromBody] ChangeProgramPriorityDTO changeProgramPriorityDTO)
        {
            await _entranceService.ChangeProgramPriority(changeProgramPriorityDTO);
            return Ok();
        }
        [HttpGet]
        [Route("statement")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Applicant")]
        [ProducesResponseType(typeof(GetMyApplicationDTO),200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetMyApplicationDTO>> GetMyApplication()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var result = await _entranceService.GetApplication(token);
            return Ok(result);
        }
    }
}
