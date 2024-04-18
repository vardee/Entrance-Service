using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Interfaces
{
    public interface IUserService
    {
        Task<TokenResponseDTO> UserRegister(UserRegisterDTO userRegisterDTO);
        Task<TokenResponseDTO> UserLogin(UserLoginDTO userLoginDTO);
        Task<TokenResponseDTO> RefreshToken(RefreshTokenRequestDTO refreshTokenRequestDTO);
        Task Logout(string token);
        Task <UserGetProfileDTO> GetProfile(string token);
        Task EditProfile(string token, EditUserProfileDTO editUserProfileDTO);

        Task AddUserRole(string token, AddUserRoleDTO addUserRoleDTO, Guid userId);
        Task<GetMyRolesDTO> GetMyRoles(string token);

        Task ChangePassword(string token,changePasswordDTO changePasswordDTO);

        Task <GetUsersPageDTO> GetQuerybleUsers(int page, int size, string token, string? email, string? Lastname, string? Firstname);
    }
}
