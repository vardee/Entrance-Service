﻿using adv_Backend_Entrance.ApplicantService.BL.Helpers;
using adv_Backend_Entrance.ApplicantService.DAL.Data;
using adv_Backend_Entrance.ApplicantService.DAL.Data.Entites;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.ApplicantService;
using adv_Backend_Entrance.Common.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.ApplicantService.BL.Services
{
    public class ApplicantDocumentService : IApplicantService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicantDBContext _applicantDBContext;
        private readonly HttpClient _httpClient;
        private string _getEducationLevels;
        private readonly TokenHelper _tokenHelper;
        public ApplicantDocumentService(IConfiguration configuration, ApplicantDBContext applicantDBContext,HttpClient httpClient, TokenHelper tokenHelper)
        {
            _configuration = configuration;
            _applicantDBContext = applicantDBContext;
            _httpClient = httpClient;
            _getEducationLevels = _configuration.GetValue<string>("GetEducationLevels");
            _tokenHelper = tokenHelper;
            var token = _tokenHelper.GetTokenFromHeader();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task AddEducationLevel(AddEducationLevelDTO addEducationLevelDTO, string token)
        {
            HttpResponseMessage getEducationLevelsResponse = await _httpClient.GetAsync(_getEducationLevels);
            if (!getEducationLevelsResponse.IsSuccessStatusCode)
            {
                throw new BadRequestException("Server response is bad, server problems. Oops!");
            }

            string getEducationLevels = await getEducationLevelsResponse.Content.ReadAsStringAsync();
            var educationLevels = JsonSerializer.Deserialize<List<GetEducationLevelsDTO>>(getEducationLevels);
            var educationLevelDescription = addEducationLevelDTO.EducationLevel.GetDescription();
            var selectedEducationLevel = educationLevels.FirstOrDefault(ed => ed.name == educationLevelDescription);

            if (selectedEducationLevel == null)
            {
                throw new BadRequestException("Specified education level not found.");
            }


            if (selectedEducationLevel == null)
            {
                throw new BadRequestException("Specified education level not found.");
            }


            string userId = _tokenHelper.GetUserIdFromToken(token);

            var educationLevel = new EducationDocument
            {
                UserId = Guid.Parse(userId),
                EducationLevel = addEducationLevelDTO.EducationLevel,
                EducationLevelId = selectedEducationLevel.id
            };

            _applicantDBContext.EducationDocuments.Add(educationLevel);
            await _applicantDBContext.SaveChangesAsync();
        }


        public async Task AddPassport(AddPassportDTO addPassportDTO, string token)
        {
            string userId = _tokenHelper.GetUserIdFromToken(token);
            var passport = new Passport
            {
                UserId = Guid.Parse(userId),
                PassportNumber = addPassportDTO.PassportNumber,
                IssuedWhen = addPassportDTO.IssuedWhen,
                IssuedWhom = addPassportDTO.IssuedWhom,
                BirthPlace = addPassportDTO.BirthPlace,
            };
            _applicantDBContext.Passports.Add(passport);
            await _applicantDBContext.SaveChangesAsync();
        }
        public async Task DeleteEducationInformation(string token)
        {
            var userId = _tokenHelper.GetUserIdFromToken(token);
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var education = await _applicantDBContext.EducationDocuments.FirstOrDefaultAsync(eD => eD.UserId == Guid.Parse(userId));
            if (education == null)
            {
                throw new NotFoundException("This education document not found!");
            }
            _applicantDBContext.EducationDocuments.Remove(education);
            await _applicantDBContext.SaveChangesAsync();
        }

        public async Task DeletePassportInformation(string token)
        {
            var userId = _tokenHelper.GetUserIdFromToken(token);
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var passport = await _applicantDBContext.Passports.FirstOrDefaultAsync(pD => pD.UserId == Guid.Parse(userId));
            if (passport == null)
            {
                throw new NotFoundException("This passport not found!");
            }
            _applicantDBContext.Passports.Remove(passport);
            await _applicantDBContext.SaveChangesAsync();
        }

        public async Task<GetEducationInformationDTO> EditEducationInformation(AddEducationLevelDTO editEducationLevelDTO, string token)
        {
            var userId = _tokenHelper.GetUserIdFromToken(token);
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var education = await _applicantDBContext.EducationDocuments.FirstOrDefaultAsync(eD => eD.UserId == Guid.Parse(userId));
            if (education == null)
            {
                throw new NotFoundException("This education document not found!");
            }
            if (editEducationLevelDTO.EducationLevel != null)
            {
                education.EducationLevel = editEducationLevelDTO.EducationLevel;
                HttpResponseMessage getEducationLevelsResponse = await _httpClient.GetAsync(_getEducationLevels);
                if (!getEducationLevelsResponse.IsSuccessStatusCode)
                {
                    throw new BadRequestException("Server response is bad, server problems. Oops!");
                }

                string getEducationLevels = await getEducationLevelsResponse.Content.ReadAsStringAsync();
                var educationLevels = JsonSerializer.Deserialize<List<GetEducationLevelsDTO>>(getEducationLevels);
                var educationLevelDescription = editEducationLevelDTO.EducationLevel.GetDescription();
                var selectedEducationLevel = educationLevels.FirstOrDefault(ed => ed.name == educationLevelDescription);
                education.EducationLevelId = selectedEducationLevel.id;
            }
            await _applicantDBContext.SaveChangesAsync();
            var educationInfo = new GetEducationInformationDTO
            {
                id = education.Id,
                EducationLevel = education.EducationLevel
            };
            return educationInfo;
        }

        public async Task<GetPassportInformationDTO> EditPaspportInformation(AddPassportDTO editPassportDTO, string token)
        {
            var userId = _tokenHelper.GetUserIdFromToken(token);
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var passport = await _applicantDBContext.Passports.FirstOrDefaultAsync(pD => pD.UserId == Guid.Parse(userId));
            if (passport == null)
            {
                throw new NotFoundException("This passport not found!");
            }
            if (editPassportDTO.IssuedWhen != null)
            {
                passport.IssuedWhen = editPassportDTO.IssuedWhen;
            }
            if (editPassportDTO.IssuedWhom != null)
            {
                passport.IssuedWhom = editPassportDTO.IssuedWhom;
            }
            if (editPassportDTO.PassportNumber != null)
            {
                passport.PassportNumber = editPassportDTO.PassportNumber;
            }
            if (editPassportDTO.BirthPlace != null)
            {
                passport.BirthPlace = editPassportDTO.BirthPlace;
            }
            await _applicantDBContext.SaveChangesAsync();

            var editedPassportInfo = new GetPassportInformationDTO
            {
                birthPlace = passport.BirthPlace,
                issuedWhen = passport.IssuedWhen,
                issuedWhom = passport.IssuedWhom,
                passportNumber = passport.PassportNumber,
            };
            return editedPassportInfo;
        }

        public async Task<GetEducationInformationDTO> GetEducationInformation(string token)
        {
            var userId = _tokenHelper.GetUserIdFromToken(token);
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var education = await _applicantDBContext.EducationDocuments.FirstOrDefaultAsync(eD => eD.UserId == Guid.Parse(userId));
            if (education == null)
            {
                throw new NotFoundException("This education document not found!");
            }
            var educationInfo = new GetEducationInformationDTO
            {
                EducationLevel = education.EducationLevel,
                id = education.Id,
                educationId = education.EducationLevelId
            };
            return educationInfo;
        }

        public async Task<GetPassportInformationDTO> GetPassportInformation(string token)
        {
            var userId = _tokenHelper.GetUserIdFromToken(token);
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var passport = await _applicantDBContext.Passports.FirstOrDefaultAsync(pD => pD.UserId == Guid.Parse(userId));
            if (passport == null)
            {
                throw new NotFoundException("This passport not found!");
            }
            var passportInfo = new GetPassportInformationDTO
            {
                birthPlace = passport.BirthPlace,
                issuedWhen = passport.IssuedWhen,
                issuedWhom = passport.IssuedWhom,
                passportNumber = passport.PassportNumber,
            };
            return passportInfo;
        }
    }
}
