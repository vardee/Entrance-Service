using adv_Backend_Entrance.Common.Data;
using adv_Backend_Entrance.Common.DTO.EntranceService;
using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.UserService.DAL.Data.Entities;
using adv_Backend_Entrance.UserService.DAL.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adv_Backend_Entrance.EntranceService.DAL.Data;
using System.Net.WebSockets;
using adv_Backend_Entrance.EntranceService.DAL.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml.Linq;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using System.Text.Json;
using adv_Backend_Entrance.Common.Enums;
using StackExchange.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.Common.DTO.UserService;

namespace adv_Backend_Entrance.EntranceService.BL.Services
{
    public class ApplicantEntranceService : IEntranceService
    {
        private readonly EntranceDBContext _entranceDBContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly TokenHelper _tokenHelper;

        private string _baseUrl;
        private string _getProfileUrl;
        public ApplicantEntranceService(EntranceDBContext entranceDBContext, IConfiguration configuration, HttpClient httpClient, TokenHelper tokenHelper)
        {
            _entranceDBContext = entranceDBContext;
            _configuration = configuration;
            _httpClient = httpClient;
            _tokenHelper = tokenHelper;
            _baseUrl = _configuration.GetValue<string>("BaseUrl");
            _getProfileUrl = _configuration.GetValue<string>("GetProfileUrl");
            var token = _tokenHelper.GetTokenFromHeader();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task AddEducationLevel(AddEducationLevelDTO addEducationLevelDTO, string token)
        {
            string userId = GetUserIdFromToken(token);
            var educationLevel = new EducationDocument
            {
                UserId = Guid.Parse(userId),
                EducationLevel = addEducationLevelDTO.EducationLevel,
            };
            _entranceDBContext.EducationDocuments.Add(educationLevel);
            await _entranceDBContext.SaveChangesAsync();
        }

        public async Task AddPassport(AddPassportDTO addPassportDTO, string token)
        {
            string userId = GetUserIdFromToken(token);
            var passport = new Passport
            {
                UserId = Guid.Parse(userId),
                PassportNumber = addPassportDTO.PassportNumber,
                IssuedWhen = addPassportDTO.IssuedWhen,
                IssuedWhom = addPassportDTO.IssuedWhom,
                BirthPlace = addPassportDTO.BirthPlace,
            };
            _entranceDBContext.Passports.Add(passport);
            await _entranceDBContext.SaveChangesAsync();
        }

        public async Task CreateApplication(CreateApplicationDTO createApplicationDTO, string token)
        {
            string userId = GetUserIdFromToken(token);
            var userPassport = await _entranceDBContext.Passports.FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId));
            if (userPassport == null)
            {
                throw new NotFoundException("Your passport not found!");
            }

            var userEducation = await _entranceDBContext.EducationDocuments.FirstOrDefaultAsync(eD => eD.UserId == Guid.Parse(userId));
            if (userEducation == null)
            {
                throw new NotFoundException("Your education document not found!");
            }
            HttpResponseMessage getProfileReponse = await _httpClient.GetAsync(_getProfileUrl);

            if (getProfileReponse.IsSuccessStatusCode)
            {
                string getProfileResponseBody = await getProfileReponse.Content.ReadAsStringAsync();
                var userProfileResponse = JsonSerializer.Deserialize<UserGetProfileDTO>(getProfileResponseBody);
                Console.WriteLine(getProfileResponseBody);
                var application = new Application
                {
                    FirstName = userProfileResponse.firstname,
                    LastName = userProfileResponse.lastname,
                    Patronymic = userProfileResponse.patronymic,
                    EducationId = createApplicationDTO.EducationId,
                    PassportId = createApplicationDTO.PassportId,
                    UserId = Guid.Parse(userId),
                    ApplicationStatus = EntranceApplicationStatus.Created,
                };
                _entranceDBContext.Applications.Add(application);
                await _entranceDBContext.SaveChangesAsync();
                foreach (var program in createApplicationDTO.ProgramsPriority)
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var programsWithPagination = JsonSerializer.Deserialize<GetProgramsWithPaginationDTO>(responseBody);
                        var programs = programsWithPagination.programs;

                        Console.WriteLine(responseBody);

                        foreach (var programApp in programs)
                        {
                            if (programApp.id == program.ProgramId)
                            {
                                var applicationProgram = new ApplicationProgram
                                {
                                    ApplicationId = application.Id,
                                    ProgramId = program.ProgramId,
                                    Priority = program.Priority,
                                };
                                _entranceDBContext.ApplicationPrograms.Add(applicationProgram);

                            }
                        }
                    }
                    await _entranceDBContext.SaveChangesAsync();
                }
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
    }
}
