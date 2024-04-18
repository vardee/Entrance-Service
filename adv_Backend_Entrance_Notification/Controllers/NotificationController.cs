using adv_Backend_Entrance.Common.Data.Models;
using adv_Backend_Entrance.Common.DTO.NotificationService;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.Common.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance_Notification.Controllers
{
    [ApiController]
    [Route("notification")]
    public class NotificationController : ControllerBase
    {
        private readonly TokenHelper _tokenHelper;
        public NotificationController(TokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper;
        }
        [HttpPost]
        [Route("send")]
        [ProducesResponseType(typeof(Error), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> SendNotification()
        {
            return Ok();
        }

    }
}
