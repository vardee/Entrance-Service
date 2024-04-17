using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.UserService.DAL.Data;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance.UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthDbContextController : ControllerBase
    {
        private readonly AuthDbContext _authDbContext;

        public AuthDbContextController(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        [HttpGet]
        public IActionResult GetAuthDbContext()
        {
            return Ok(_authDbContext);
        }
    }
}
