using adv_Backend_Entrance.Common.DTO;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace adv_Backend_Entrance.FacultyService.MVCPanel.Controllers
{
    [ApiController]
    [Route("faculty")]
    public class FacultyController : ControllerBase
    {
        private readonly IFacultyInteractionsService
            _interactionService;
        private readonly IFacultyService _facultyService;
        private readonly TokenHelper _tokenHelper;
        public FacultyController(IFacultyService facultyService, TokenHelper tokenHelper, IFacultyInteractionsService facultyInteractionsService)
        {
            _facultyService = facultyService;
            _tokenHelper = tokenHelper;
            _interactionService = facultyInteractionsService;
        }

        [HttpPost]
        [Route("import/dictionary")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> GetDictionary()
        {
            await _facultyService.GetDictionary();
            return Ok();
        }

        [HttpGet]
        [Route("document_types")]
        [ProducesResponseType(typeof(List<GetDocumentTypesDTO>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<List<GetDocumentTypesDTO>>> GetDocumentTypes()
        {
            var result = await _interactionService.GetDocumentTypes();
            return Ok(result);
        }
        [HttpGet]
        [Route("faculties")]
        [ProducesResponseType(typeof(List<GetFacultiesDTO>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<List<GetFacultiesDTO>>> GetFaculties()
        {
            var result = await _interactionService.GetFaculties();
            return Ok(result);
        }
        [HttpGet]
        [Route("education_levels")]
        [ProducesResponseType(typeof(List<GetEducationLevelsDTO>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<List<GetEducationLevelsDTO>>> GetEducationLevels()
        {
            var result = await _interactionService.GetEducationLevels();
            return Ok(result);
        }
        [HttpGet]
        [Route("programs")]
        [ProducesResponseType(typeof(GetQuerybleProgramsDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<List<GetEducationLevelsDTO>>> GetQueryblePrograms(
            [FromQuery] List<EducationLanguage> LanguageEnum, 
            [FromQuery] List<EducationLevel> EducationLevelEnum, 
            [FromQuery] List<EducationForm> EducationFormEnum,Guid? FacultyId, Guid? Id,
            [FromQuery] int size = 10, int page=1)
        {
            var result = await _interactionService.GetQueryblePrograms(size,page,LanguageEnum,EducationLevelEnum, EducationFormEnum,FacultyId,Id);
            return Ok(result);
        }
    }
}

