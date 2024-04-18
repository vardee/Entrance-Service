using adv_Backend_Entrance.Common.Data;
using adv_Backend_Entrance.Common.Data.Models;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.DTO;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.UserService.DAL.Data;
using adv_Backend_Entrance.UserService.DAL.Data.Entities;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using adv_Backend_Entrance.Common.Helpers.Validators;

namespace adv_Backend_Entrance.UserService.BL.Services
{
    public class UsersService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly AuthDbContext _authDBContext;
        private readonly RedisDBContext _redisDBContext;
        private readonly IConfiguration _configuration;


        public UsersService(UserManager<User> userManager, SignInManager<User> signInManager, AuthDbContext authDbContext, IConfiguration configuration, RoleManager<IdentityRole<Guid>> roleManager, RedisDBContext redisDBContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _authDBContext = authDbContext;
            _configuration = configuration;
            _redisDBContext = redisDBContext;
        }

        public async Task<TokenResponseDTO> UserRegister(UserRegisterDTO userRegisterDTO)
        {
            if (userRegisterDTO.Email == null)
            {
                throw new BadRequestException("Email is empty");
            }

            if (userRegisterDTO.Password == null)
            {
                throw new BadRequestException("Password is empty");
            }
            if (userRegisterDTO.Password != userRegisterDTO.ConfirmPassword)
            {
                throw new BadRequestException("Passwords don't match");
            }
            if (!BirthDateValidator.ValidateDateOfBirth(userRegisterDTO.BirthDate))
            {
                throw new BadRequestException("Неверная дата рождения. Вам должно быть не менее 13 лет и не более 100 лет.");
            }

            var user = new User
            {
                FullName = userRegisterDTO.FullName,
                LastName = userRegisterDTO.LastName,
                Patronymic = userRegisterDTO.Patronymic,
                Gender = userRegisterDTO.Gender,
                Email = userRegisterDTO.Email,
                PhoneNumber = userRegisterDTO.Phone,
                BirthDate = userRegisterDTO.BirthDate,
                UserName = userRegisterDTO.Email,
            };
            var result = await _userManager.CreateAsync(user, userRegisterDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Applicant");
                return await UserLogin(new UserLoginDTO()
                {
                    Email = userRegisterDTO.Email,
                    Password = userRegisterDTO.Password,
                });
            }
            else
            {
                throw new Exception("User registration failed");
            }
        }

        public async Task<TokenResponseDTO> UserLogin(UserLoginDTO userLoginDTO)
        {
            var veryfiedUser = await CheckBasedUserInformation(userLoginDTO.Email, userLoginDTO.Password);
            if (veryfiedUser == null)
            {
                throw new NotFoundException("User not found");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userLoginDTO.Email);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
{
    new Claim(ClaimTypes.Email, user.Id.ToString())
};

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwt = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt")["Issuer"],
                audience: _configuration.GetSection("Jwt")["Audience"],
                notBefore: DateTime.UtcNow,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_configuration.GetSection("Jwt")
                    .GetValue<int>("AccessTokenLifetimeInMinutes"))),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty)),
                    SecurityAlgorithms.HmacSha256));



            var tokenRefresh = GenerateRefreshToken();

            user.RefreshToken = tokenRefresh;
            var refreshTokenLifetimeInDays = _configuration.GetSection("Jwt").GetValue<int>("RefreshTokenLifetimeInDays");
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenLifetimeInDays);
            user.RefreshTokenExpiry = refreshTokenExpiry;


            await _authDBContext.SaveChangesAsync();

            return new TokenResponseDTO
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
                RefreshToken = tokenRefresh
            };
        }

        public async Task ChangePassword(string token, changePasswordDTO changePasswordDTO)
        {
            var userId = GetUserIdFromToken(token);
            var user  = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            if(changePasswordDTO.Password != changePasswordDTO.ConfirmPassword)
            {
                throw new BadRequestException("Your new passswords dont sovpadat!");
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPasssword, changePasswordDTO.Password);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));
            }
        }
        public async Task<TokenResponseDTO> RefreshToken(RefreshTokenRequestDTO refreshTokenRequestDTO)
        {

            var principal = GetUserIdFromToken(refreshTokenRequestDTO.AccessToken);

            if (principal == null)
            {
                throw new ForbiddenException("This account is not authorized");
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.ToString() == principal);
            Console.WriteLine(principal);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
{
    new Claim(ClaimTypes.Email, user.Id.ToString())
};

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwt = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt")["Issuer"],
                audience: _configuration.GetSection("Jwt")["Audience"],
                notBefore: DateTime.UtcNow,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_configuration.GetSection("Jwt")
                    .GetValue<int>("AccessTokenLifetimeInMinutes"))),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty)),
                    SecurityAlgorithms.HmacSha256));



            var tokenRefresh = GenerateRefreshToken();

            user.RefreshToken = tokenRefresh;
            var refreshTokenLifetimeInDays = _configuration.GetSection("Jwt").GetValue<int>("RefreshTokenLifetimeInDays");
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenLifetimeInDays);
            user.RefreshTokenExpiry = refreshTokenExpiry;


            await _authDBContext.SaveChangesAsync();

            return new TokenResponseDTO
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
                RefreshToken = tokenRefresh
            };
        }


        public async Task Logout(string token)
        {
            if (token == null)
            {
                throw new UnauthorizedException("User is not authorized");
            }
            await _redisDBContext.AddToken(token);
        }

        public async Task<UserGetProfileDTO> GetProfile(string token)
        {
            var userId = GetUserIdFromToken(token);
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    return new UserGetProfileDTO
                    {
                        Firstname = user.FullName,
                        Lastname = user.LastName,
                        Patronymic = user.Patronymic,
                        BirthDate = user.BirthDate,
                        Email = user.Email,
                        Phone = user.PhoneNumber,
                    };
                }
                else
                {
                    throw new NotFoundException("User is not found");
                }
            }
            else
            {
                throw new UnauthorizedException("User is not authorized");
            }
        }
        public async Task EditProfile(string token, EditUserProfileDTO editUserProfileDTO)
        {
            var userId = GetUserIdFromToken(token);
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (editUserProfileDTO.FirstName != "")
                    {
                        user.FullName = editUserProfileDTO.FirstName;
                    }
                    if (editUserProfileDTO.LastName != "")
                    {
                        user.LastName = editUserProfileDTO.LastName;
                    }
                    if (editUserProfileDTO.Email != "")
                    {
                        user.Email = editUserProfileDTO.Email;
                    }
                    if (editUserProfileDTO.Phone != "")
                    {
                        user.PhoneNumber = editUserProfileDTO.Phone;
                    }
                    if (editUserProfileDTO.BirthDate != null)

                        if (!BirthDateValidator.ValidateDateOfBirth(editUserProfileDTO.BirthDate))
                        {
                            throw new BadRequestException("Неверная дата рождения. Вам должно быть не менее 13 лет и не более 100 лет.");
                        }
                    {
                        user.BirthDate = (DateTime)editUserProfileDTO.BirthDate;
                    }
                    if (editUserProfileDTO.Patronymic != "")
                    {
                        user.Patronymic = editUserProfileDTO.Patronymic;
                    }
                    await _authDBContext.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("User is not found");
                }
            }
            else
            {
                throw new UnauthorizedException("User is not authorized");
            }
        }


        public async Task AddUserRole(string token, AddUserRoleDTO addUserRoleDTO, Guid Id)
        {
            var userId = GetUserIdFromToken(token);
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        await AddRoleToUser(addUserRoleDTO.Role, Id);
                    }
                    else if (roles.Contains("MainManager"))
                    {
                        if (addUserRoleDTO.Role == RoleType.Manager)
                        {
                            await AddRoleToUser(addUserRoleDTO.Role, Id);
                        }
                        else
                        {
                            throw new ForbiddenException("MainManager can only assign the Manager role.");
                        }
                    }
                    else
                    {
                        throw new ForbiddenException("You dont have permissions to assign roles.");
                    }
                }
                else
                {
                    throw new NotFoundException("User is not found");
                }
            }
            else
            {
                throw new UnauthorizedException("User is not authorized");
            }
        }


        public async Task<GetMyRolesDTO> GetMyRoles(string token)
        {
            var userId = GetUserIdFromToken(token);
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);
                if (roles != null)
                {
                    return new GetMyRolesDTO
                    {
                        Roles = roles
                    };
                }
                else
                {
                    throw new BadRequestException("You dont have any roles");
                }
            }
            else
            {
                throw new UnauthorizedException("User is not authorized");
            }
        }
        private async Task AddRoleToUser(RoleType role, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (!Enum.IsDefined(typeof(RoleType), role))
            {
                throw new BadRequestException($"Role '{role}' is not defined.");
            }

            var roleName = Enum.GetName(typeof(RoleType), role);

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                throw new BadRequestException($"Failed to add user to role '{roleName}'.");
            }
        }
        public string? GetUserIdFromToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Secret").Value));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);

                if (claimsPrincipal is null || !(validatedToken is JwtSecurityToken jwtSecurityToken))
                    return null;

                var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);

                return emailClaim?.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<GetUsersPageDTO> GetQuerybleUsers(int page, int size, string token, string? email, string? Lastname, string? Firstname)
        {
            var userQuery =  _authDBContext.Users.AsQueryable();
            var users = await userQuery.ToListAsync();
            if (email != null)
            {
                userQuery = userQuery.Where(aR => aR.Email.Contains(email));
            }
            if (Lastname != null)
            {
                userQuery = userQuery.Where(aR => aR.LastName.Contains(Lastname));
            }
            if (Firstname != null)
            {
                userQuery = userQuery.Where(aR => aR.FullName.Contains(Firstname));
            }
            int sizeOfPage = size;
            var countOfPages = (int)Math.Ceiling((double)userQuery.Count() / sizeOfPage);
            if (page <= countOfPages)
            {
                var lowerBound = page == 1 ? 0 : (page - 1) * sizeOfPage;
                if (page < countOfPages)
                {
                    userQuery = userQuery.Skip(lowerBound).Take(sizeOfPage);
                }
                else
                {
                    userQuery = userQuery.Skip(lowerBound).Take(userQuery.Count() - lowerBound);
                }
            }
            else
            {
                throw new BadRequestException("Такой страницы нет");
            }

            var pagination = new PaginationInformation
            {
                Current = page,
                Page = countOfPages,
                Size = size
            };

            var usersDTO = new GetUsersPageDTO
            {
                Users = userQuery.Select(p => new GetUsersDTO
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    LastName = p.LastName,
                    Patronymic = p.Patronymic,
                    Email = p.Email,
                    BirthDate = p.BirthDate,
                    Gender = p.Gender,
                }).AsQueryable(),
                Pagination = pagination
            };
            return usersDTO;
        }
        public string GenerateRefreshToken()
        {
            var randomValues = new byte[128];
            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomValues);
            }
            return Convert.ToBase64String(randomValues);
        }
        private async Task<ClaimsIdentity> CheckBasedUserInformation(string email, string password)
        {
            Console.WriteLine(email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return null;
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, user.Id.ToString()) };

            return new ClaimsIdentity(claims, "Token", ClaimTypes.Email, ClaimTypes.Role);
        }
    }
}