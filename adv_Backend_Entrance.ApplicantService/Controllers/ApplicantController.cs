using adv_Backend_Entrance.Common.Data.Models;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.ApplicantService;
using adv_Backend_Entrance.Common.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance.ApplicantService.Controllers
{
    [ApiController]
    [Route("applicant")]
    public class ApplicantController : ControllerBase
    {
        private readonly IApplicantService _applicantService;
        private readonly IApplicantDocumentsFiles _applicantDocumentsFiles;
        private readonly TokenHelper _tokenHelper;
        public ApplicantController(IApplicantService applicantService, TokenHelper tokenHelper, IApplicantDocumentsFiles applicantDocumentsFiles)
        {
            _applicantService = applicantService;
            _tokenHelper = tokenHelper;
            _applicantDocumentsFiles = applicantDocumentsFiles;
        }
        [HttpPost]
        [Route("education/information")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> AddEducation([FromQuery] AddEducationLevelDTO addEducationLevelDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _applicantService.AddEducationLevel(addEducationLevelDTO, token);
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
            await _applicantService.AddPassport(addPassportDTO, token);
            return Ok();
        }
        [HttpPut]
        [Route("passport/information")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(GetPassportInformationDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetPassportInformationDTO>> EditPassport([FromBody] AddPassportDTO editPassportDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var result = await _applicantService.EditPaspportInformation(editPassportDTO, token);
            return Ok(result);
        }
        [HttpPut]
        [Route("education/information")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(GetEducationInformationDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetEducationInformationDTO>> EditPassport([FromQuery] AddEducationLevelDTO editEdcuationLevelDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var result = await _applicantService.EditEducationInformation(editEdcuationLevelDTO, token);
            return Ok(result);
        }
        [HttpGet]
        [Route("passport/information")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(GetPassportInformationDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetPassportInformationDTO>> GetPassport()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var result = await _applicantService.GetPassportInformation(token);
            return Ok(result);
        }
        [HttpGet]
        [Route("education/information")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(GetEducationInformationDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetEducationInformationDTO>> GetEducationInformation()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var result = await _applicantService.GetEducationInformation(token);
            return Ok(result);
        }
        [HttpDelete]
        [Route("education/information")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> DeleteEducationInformation()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _applicantService.DeleteEducationInformation(token);
            return Ok();
        }
        [HttpDelete]
        [Route("passport/information")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> DeletePassportInformation()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _applicantService.DeletePassportInformation(token);
            return Ok();
        }
        [HttpPost]
        [Route("education/document")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> UploadEducationDocument([FromForm] AddFileDTO addFileDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _applicantDocumentsFiles.UploadEducationDocumentFile(addFileDTO, token);
            return Ok();
        }
        [HttpPost]
        [Route("passport/document")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> UploadPassportDocument([FromForm] AddFileDTO addFileDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _applicantDocumentsFiles.UploadPassportFile(addFileDTO, token);
            return Ok();
        }
        [HttpGet]
        [Route("education/document")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(byte[]),200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<byte[]>> GetFileEducationDocument()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var result = await _applicantDocumentsFiles.GetEducationDocumentFile(token);
            return Ok(result);
        }
        [HttpGet]
        [Route("passport/document")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(byte[]), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<byte[]>> GetPassportFile()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            var result = await _applicantDocumentsFiles.GetPassportFile(token);
            return Ok(result);
        }
        [HttpDelete]
        [Route("passport/document")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> DeletePassportFile()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _applicantDocumentsFiles.DeletePassport(token);
            return Ok();
        }
        [HttpDelete]
        [Route("education/document")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> DeleteEducationDocumentFile()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _applicantDocumentsFiles.DeleteEducationLevel(token);
            return Ok();
        }
    }
}
