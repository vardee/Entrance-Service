using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Interfaces.UserService
{
    public interface IUserService
    {
        Task<TokenResponseDTO> UserRegister(UserRegisterDTO userRegisterDTO);
        Task<TokenResponseDTO> UserLogin(UserLoginDTO userLoginDTO);
        Task<TokenResponseDTO> RefreshToken(RefreshTokenRequestDTO refreshTokenRequestDTO);
        Task Logout(string token);
        Task<UserGetProfileDTO> GetProfile(Guid userId);
        Task EditProfile(Guid userId, EditUserProfileDTO editUserProfileDTO);

        Task AddUserRole(Guid userId, AddUserRoleDTO addUserRoleDTO,Guid id);

        Task RemoveUserRole(Guid userId, AddUserRoleDTO addUserRoleDTO, Guid id);
        Task<GetMyRolesDTO> GetMyRoles(Guid userId);

        Task ChangePassword(Guid userId, changePasswordDTO changePasswordDTO);

        Task<GetUsersPageDTO> GetQuerybleUsers(int page, int size, string? email, string? Lastname, string? Firstname);
    }
}
