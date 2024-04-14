using adv_Backend_Entrance.Common.DTO;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance.FacultyService.MVCPanel.Controllers
{
    [ApiController]
    [Route("faculty")]
    public class FacultyController : ControllerBase
    {
        private readonly IFacultyService _facultyService;
        private readonly TokenHelper _tokenHelper;
        public FacultyController(IFacultyService facultyService, TokenHelper tokenHelper)
        {
            _facultyService = facultyService;
            _tokenHelper = tokenHelper;
        }

        [HttpGet]
        [Route("get/educationlevels")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> GetDictionary()
        {
            await _facultyService.GetDictionary();
            return Ok();
        }

    }
}
