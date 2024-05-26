using adv_Backend_Entrance.Common.Data;
using adv_Backend_Entrance.Common.DTO;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.UserService.DAL.Data;
using adv_Backend_Entrance.UserService.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using adv_Backend_Entrance.Common.Helpers.Validators;
using adv_Backend_Entrance.Common.Interfaces.UserService;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using EasyNetQ;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.DTO.ApplicantService;


namespace adv_Backend_Entrance.UserService.BL.Services
{
    public class UsersService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AuthDbContext _authDBContext;
        private readonly RedisDBContext _redisDBContext;
        private readonly IConfiguration _configuration;
        private readonly TokenHelper _tokenHelper;
        private readonly AdditionTokenService _additionTokenService;
        private readonly IBus _bus;


        public UsersService(UserManager<User> userManager, SignInManager<User> signInManager, AuthDbContext authDbContext, IConfiguration configuration, RedisDBContext redisDBContext, AdditionTokenService additionTokenService, TokenHelper tokenHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authDBContext = authDbContext;
            _configuration = configuration;
            _redisDBContext = redisDBContext;
            _tokenHelper = tokenHelper;
            _additionTokenService = additionTokenService;
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public async Task<TokenResponseDTO> UserRegister(UserRegisterDTO userRegisterDTO)
        {
            if (userRegisterDTO.Email == null)
            {
                throw new BadRequestException("Email is empty");
            }
            if (!FullNameValidator.ValidateFullName(userRegisterDTO.FullName))
            {
                throw new BadRequestException("The name must be from 2 to 30 letters!");
            }
            if (!FullNameValidator.ValidateFullName(userRegisterDTO.LastName))
            {
                throw new BadRequestException("The lastname must be from 2 to 30 letters!");
            }
            if (!FullNameValidator.ValidateFullName(userRegisterDTO.Patronymic))
            {
                throw new BadRequestException("The patronymic must be from 2 to 30 letters!");
            }
            if (!EmailValidator.ValidateEmail(userRegisterDTO.Email))
            {
                throw new BadRequestException("You should to write the correct Email!");
            }
            if (userRegisterDTO.Password == null)
            {
                throw new BadRequestException("Password is empty");
            }
            if (!PasswordValidator.ValidatePassword(userRegisterDTO.Password))
            {
                throw new BadRequestException("Password should be 8-15 symbols, and should include uppercase and lowercase letters and numbers!");
            }
            if (userRegisterDTO.Password != userRegisterDTO.ConfirmPassword)
            {
                throw new BadRequestException("Passwords don't match");
            }
            if (!BirthDateValidator.ValidateDateOfBirth(userRegisterDTO.BirthDate))
            {
                throw new BadRequestException("Incorrect date of birth. You must be at least 18 years old and no more than 60 years old.");
            }
            if (!PhoneValidator.IsValidPhoneNumber(userRegisterDTO.Phone))
            {
                throw new BadRequestException("Incorrect phone number format: for example, 79991234567 or 89991234567");
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
                await _userManager.AddToRoleAsync(user, "User");
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

            var claims = new List<Claim>{new Claim(ClaimTypes.Email, user.Id.ToString())};

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



            var tokenRefresh = _additionTokenService.GenerateRefreshToken();

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
        public async Task ChangePassword(Guid userId, changePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            if (changePasswordDTO.Password != changePasswordDTO.ConfirmPassword)
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

            var principal = _additionTokenService.GetUserIdFromToken(refreshTokenRequestDTO.AccessToken);

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

            var claims = new List<Claim>{new Claim(ClaimTypes.Email, user.Id.ToString())};

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



            var tokenRefresh = _additionTokenService.GenerateRefreshToken();

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

            var id = _tokenHelper.GetUserIdFromToken(token);
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }
            await _redisDBContext.AddToken(token);
        }


        public async Task<UserGetProfileDTO> GetProfile(Guid userId)
        {
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user != null)
                {
                    return new UserGetProfileDTO
                    {
                        firstname = user.FullName,
                        lastname = user.LastName,
                        patronymic = user.Patronymic,
                        BirthDate = user.BirthDate,
                        Email = user.Email,
                        Phone = user.PhoneNumber,
                        Nationality = user.Nationality,
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
        public async Task EditProfile(Guid userId, EditUserProfileDTO editUserProfileDTO)
        {
            if (userId != null)
            {
                
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user != null)
                {
                    if (editUserProfileDTO.FirstName != "")
                    {
                        if (!FullNameValidator.ValidateFullName(editUserProfileDTO.FirstName))
                        {
                            throw new BadRequestException("The lastname must be from 2 to 30 letters!");
                        }
                        user.FullName = editUserProfileDTO.FirstName;
                    }
                    if (editUserProfileDTO.LastName != "")
                    {
                        if (!FullNameValidator.ValidateFullName(editUserProfileDTO.LastName))
                        {
                            throw new BadRequestException("The lastname must be from 2 to 30 letters!");
                        }
                        user.LastName = editUserProfileDTO.LastName;
                    }
                    if (editUserProfileDTO.Email != "")
                    {
                        if (!EmailValidator.ValidateEmail(editUserProfileDTO.Email))
                        {
                            throw new BadRequestException("You should to write the correct Email!");
                        }
                        user.Email = editUserProfileDTO.Email;
                    }
                    if (editUserProfileDTO.Nationality != "")
                    {
                        user.Nationality = editUserProfileDTO.Nationality;
                    }
                    if (editUserProfileDTO.Phone != null)
                    {
                        if (!PhoneValidator.IsValidPhoneNumber(editUserProfileDTO.Phone))
                        {
                            throw new BadRequestException("Incorrect phone number format: for example, 79991234567 or 89991234567");
                        }
                        user.PhoneNumber = editUserProfileDTO.Phone;
                    }
                    if (editUserProfileDTO.Gender != null)
                    {
                        user.Gender = editUserProfileDTO.Gender;
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
                        if (!FullNameValidator.ValidateFullName(editUserProfileDTO.Patronymic))
                        {
                            throw new BadRequestException("The lastname must be from 2 to 30 letters!");
                        }
                        user.Patronymic = editUserProfileDTO.Patronymic;
                    }
                    await _authDBContext.SaveChangesAsync();
                    var editedProfile = new EditApplicantProfileInformationDTO
                    {
                        UserId = userId,
                        Email = editUserProfileDTO.Email,
                        FirstName = editUserProfileDTO.FirstName,
                        LastName = editUserProfileDTO.LastName,
                        Patronymic = editUserProfileDTO.Patronymic,
                        Nationality = editUserProfileDTO.Nationality,
                    };
                    await SyncProfile(editedProfile);
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
        private async Task SyncProfile(EditApplicantProfileInformationDTO editApplicantProfile)
        {
            var message = new EditApplicantProfileInformationDTO
            {
                Email = editApplicantProfile.Email,
                FirstName = editApplicantProfile.FirstName,
                LastName = editApplicantProfile.LastName,
                Patronymic = editApplicantProfile.Patronymic,
                Nationality = editApplicantProfile.Nationality,
                UserId = editApplicantProfile.UserId
            };
            await _bus.PubSub.PublishAsync(message, "profile_edit");
        }

        public async Task AddUserRole(Guid userId, AddUserRoleDTO addUserRoleDTO, Guid Id)
        {
            
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    await _additionTokenService.AddRoleToUser(addUserRoleDTO.Role, Id);
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
        public async Task RemoveUserRole(Guid userId, AddUserRoleDTO addUserRoleDTO, Guid Id)
        {

            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    await _additionTokenService.RemoveRoleFromUser(addUserRoleDTO.Role, Id);
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


        public async Task<GetMyRolesDTO> GetMyRoles(Guid userId)
        {
            
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
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
        public async Task<GetUsersPageDTO> GetQuerybleUsers(int page, int size, string? email, string? lastName, string? firstName)
        {
            if (page <= 0)
            {
                page = 1;
            }
            if (size <= 0)
            {
                size = 10;
            }

            var userQuery = _authDBContext.Users.AsQueryable();

            if (!string.IsNullOrEmpty(email))
            {
                userQuery = userQuery.Where(u => u.Email.Contains(email));
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                userQuery = userQuery.Where(u => u.LastName.Contains(lastName));
            }
            if (!string.IsNullOrEmpty(firstName))
            {
                userQuery = userQuery.Where(u => u.FullName.Contains(firstName));
            }

            int totalUsers = await userQuery.CountAsync();
            var countOfPages = (int)Math.Ceiling((double)totalUsers / size);

            if (page > countOfPages || totalUsers == 0)
            {
                return new GetUsersPageDTO
                {
                    Users = Enumerable.Empty<GetUsersDTO>().AsQueryable(),
                    Pagination = new PaginationInformation
                    {
                        Current = page,
                        Page = countOfPages,
                        Size = size
                    }
                };
            }

            var users = await userQuery
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var usersDTO = new List<GetUsersDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                usersDTO.Add(new GetUsersDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    LastName = user.LastName,
                    Patronymic = user.Patronymic,
                    Email = user.Email,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender,
                    Roles = roles.Select(r => Enum.Parse<RoleType>(r)).ToList()
                });
            }

            return new GetUsersPageDTO
            {
                Users = usersDTO.AsQueryable(),
                Pagination = new PaginationInformation
                {
                    Current = page,
                    Page = countOfPages,
                    Size = size
                }
            };
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