using adv_Backend_Entrance.Common.Data.Models;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.Common.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance.UserService.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly TokenHelper _tokenHelper;
        public UserController(IUserService userService, TokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(Error), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<TokenResponseDTO>> Register([FromBody] UserRegisterDTO userRegisterDto)
        {
            return Ok(await _userService.UserRegister(userRegisterDto));
        }
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(Error), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<TokenResponseDTO>> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            return Ok(await _userService.UserLogin(userLoginDTO));
        }

        [HttpPost]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("refreshToken")]
        [ProducesResponseType(typeof(Error), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken([FromBody] RefreshTokenRequestDTO refreshTokenRequestDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            return Ok(await _userService.RefreshToken(refreshTokenRequestDTO));
        }
        [HttpPost]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("logout")]
        [ProducesResponseType(typeof(Error), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> Logout()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _userService.Logout(token);
            return Ok();
        }
        [HttpGet]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("profile")]
        [ProducesResponseType(typeof(UserGetProfileDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<UserGetProfileDTO>> GetProfile()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            return Ok(await _userService.GetProfile(token));
        }
        [HttpPut]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("profile")]
        [ProducesResponseType(typeof(UserGetProfileDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> EditProfile([FromBody] EditUserProfileDTO editUserProfileDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _userService.EditProfile(token, editUserProfileDTO);
            return Ok();
        }
        [HttpPut]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("role/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> AddUserRole([FromQuery] AddUserRoleDTO addUserRoleDTO,Guid userId)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            await _userService.AddUserRole(token, addUserRoleDTO, userId);
            return Ok();
        }
        [HttpGet]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("role/my")]
        [ProducesResponseType(typeof(GetMyRolesDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetMyRolesDTO>> GetMyRoles()
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            return Ok(await _userService.GetMyRoles(token));
        }

    }
}
