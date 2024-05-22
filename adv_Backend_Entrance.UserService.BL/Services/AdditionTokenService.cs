using adv_Backend_Entrance.UserService.DAL.Data.Entities;
using adv_Backend_Entrance.UserService.DAL.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.Common.DTO.UserService;
using Microsoft.EntityFrameworkCore;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using EasyNetQ;

namespace adv_Backend_Entrance.UserService.BL.Services
{
    public class AdditionTokenService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AuthDbContext _authDBContext;
        private readonly IConfiguration _configuration;
        private readonly IBus _bus;

        public AdditionTokenService(UserManager<User> userManager, SignInManager<User> signInManager, AuthDbContext authDbContext, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authDBContext = authDbContext;
            _configuration = configuration;
            _bus = RabbitHutch.CreateBus("host=localhost");
        }
        public async Task RemoveRoleFromUser(RoleType role, Guid userId)
        {

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (!Enum.IsDefined(typeof(RoleType), role))
            {
                throw new BadRequestException($"Role '{role}' is not defined.");
            }

            var roleName = Enum.GetName(typeof(RoleType), role);

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if ((role == RoleType.Manager) || (role == RoleType.MainManager))
            {
                string fullUserName = $"{user.FullName} {user.LastName} {user.Patronymic}";
                var message = new RemoveManagerFromDbDTO
                {
                    FullName = fullUserName,
                    Email = user.Email,
                    Role = role,
                    UserId = userId
                };
                await _bus.PubSub.PublishAsync(message, "removeManagerFromDB_User");
                await _signInManager.RefreshSignInAsync(user);
            }
            if (!result.Succeeded)
            {
                throw new BadRequestException($"Failed to remove user from role '{roleName}'.");
            }
        }
        public async Task AddRoleToUser(RoleType role, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (!Enum.IsDefined(typeof(RoleType), role))
            {
                throw new BadRequestException($"Role '{role}' is not defined.");
            }

            var roleName = Enum.GetName(typeof(RoleType), role);

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if ((role == RoleType.Manager) || (role == RoleType.MainManager))
            {
                string fullUserName = $"{user.FullName} {user.LastName} {user.Patronymic}";
                var message = new AddManagerInDbDTO
                {
                    FullName = fullUserName,
                    Email = user.Email,
                    Role = role,
                    UserId = userId
                };
                await _bus.PubSub.PublishAsync(message, "addManagerInDB_User");
            }
            if (!result.Succeeded)
            {
                throw new BadRequestException($"Failed to add user to role '{roleName}'.");
            }
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


    }
}
