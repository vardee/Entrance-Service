using adv_Backend_Entrance.ApplicantService.DAL.Data;
using adv_Backend_Entrance.ApplicantService.DAL.Data.Entites;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.Interfaces.ApplicantService;
using adv_Backend_Entrance.Common.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.ApplicantService.BL.Services
{
    public class ApplicantDocumentService : IApplicantService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicantDBContext _applicantDBContext;
        public ApplicantDocumentService(IConfiguration configuration, ApplicantDBContext applicantDBContext)
        {
            _configuration = configuration;
            _applicantDBContext = applicantDBContext;
        }

        public async Task AddEducationLevel(AddEducationLevelDTO addEducationLevelDTO, string token)
        {
            string userId = GetUserIdFromToken(token);
            var educationLevel = new EducationDocument
            {
                UserId = Guid.Parse(userId),
                EducationLevel = addEducationLevelDTO.EducationLevel,
            };
            _applicantDBContext.EducationDocuments.Add(educationLevel);
            await _applicantDBContext.SaveChangesAsync();
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
            _applicantDBContext.Passports.Add(passport);
            await _applicantDBContext.SaveChangesAsync();
        }
        public async Task DeleteEducationInformation(string token)
        {
            var userId = GetUserIdFromToken(token);
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
            var userId = GetUserIdFromToken(token);
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
            var userId = GetUserIdFromToken(token);
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
            var userId = GetUserIdFromToken(token);
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
            var userId = GetUserIdFromToken(token);
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
            };
            return educationInfo;
        }

        public async Task<GetPassportInformationDTO> GetPassportInformation(string token)
        {
            var userId = GetUserIdFromToken(token);
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
