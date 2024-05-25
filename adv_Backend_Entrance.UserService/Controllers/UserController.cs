using adv_Backend_Entrance.Common.Data.Models;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.UserService;
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
        private readonly IManagerAccountService _managerService;
        private readonly TokenHelper _tokenHelper;
        public UserController(IUserService userService, TokenHelper tokenHelper, IManagerAccountService managerService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _managerService = managerService;
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
            string id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
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
            string id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
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
            string id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            return Ok(await _userService.GetProfile(userId));
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
            string id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            await _userService.EditProfile(userId, editUserProfileDTO);
            return Ok();
        }
        [HttpPut]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Admin")]
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
            string id = _tokenHelper.GetUserIdFromToken(token);
            Guid myId = Guid.Parse(id);
            await _userService.AddUserRole(userId, addUserRoleDTO, myId);
            return Ok();
        }
        [HttpGet]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("role/my")]
        [Authorize(Roles = "User,Applicant,Admin,Manager,MainManager")]
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
            string id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            return Ok(await _userService.GetMyRoles(userId));
        }
        [HttpPut]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("password")]
        [Authorize(Roles = "User,Applicant,Admin,Manager,MainManager")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> ChangePassword([FromBody] changePasswordDTO changePasswordDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            string id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            await _userService.ChangePassword(userId, changePasswordDTO);
            return Ok();
        }
        [HttpGet]
        [Route("users")]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GetUsersPageDTO), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetUsersPageDTO>> GetUsersPage([FromQuery] string? email, string? Lastname, string? Firstname, [FromQuery] int size = 10, int page = 1)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            string id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            var result = await _userService.GetQuerybleUsers(page, size, email, Lastname, Firstname);
            return Ok(result);
        }
        [HttpPut]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("profile/applicant")]
        [Authorize(Roles = "Admin,MainManager,Manager")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> EditProfile([FromBody] EditApplicantProfileInformationDTO editApplicantProfileInformationDTO)
        {
            string token = _tokenHelper.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Данный пользователь не авторизован");
            }
            string id = _tokenHelper.GetUserIdFromToken(token);
            Guid userId = Guid.Parse(id);
            await _managerService.EditApplicantProfile(editApplicantProfileInformationDTO);
            return Ok();
        }

    }
}
