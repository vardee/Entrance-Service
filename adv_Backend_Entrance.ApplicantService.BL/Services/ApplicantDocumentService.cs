﻿using adv_Backend_Entrance.ApplicantService.BL.Helpers;
using adv_Backend_Entrance.ApplicantService.DAL.Data;
using adv_Backend_Entrance.ApplicantService.DAL.Data.Entites;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.ApplicantService;
using adv_Backend_Entrance.Common.Middlewares;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace adv_Backend_Entrance.ApplicantService.BL.Services
{
    public class ApplicantDocumentService : IApplicantService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicantDBContext _applicantDBContext;
        private readonly HttpClient _httpClient;
        private readonly IBus _bus;
        private readonly TokenHelper _tokenHelper;
        public ApplicantDocumentService(IConfiguration configuration, ApplicantDBContext applicantDBContext,HttpClient httpClient, TokenHelper tokenHelper)
        {
            _configuration = configuration;
            _applicantDBContext = applicantDBContext;
            _httpClient = httpClient;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _tokenHelper = tokenHelper;
            var token = _tokenHelper.GetTokenFromHeader();
        }

        public async Task AddEducationLevel(AddEducationLevelDTO addEducationLevelDTO, Guid userId)
        {

            var educationLevels = await _bus.Rpc.RequestAsync<Guid, List<GetEducationLevelsDTO>>(userId, c => c.WithQueueName("getEducationLevelsFromEL"));
            var educationLevelDescription = addEducationLevelDTO.EducationLevel.GetDescription();
            var selectedEducationLevel = educationLevels.FirstOrDefault(ed => ed.name == educationLevelDescription);
            var educationDocument = await _applicantDBContext.EducationDocuments.FirstOrDefaultAsync(e => e.UserId == userId);
            if(educationDocument != null)
            {
                throw new BadRequestException("You can add only 1 education document!");
            }
            if (selectedEducationLevel == null)
            {
                throw new BadRequestException("Specified education level not found.");
            }


            if (selectedEducationLevel == null)
            {
                throw new BadRequestException("Specified education level not found.");
            }


            var educationLevel = new EducationDocument
            {
                UserId = userId,
                EducationLevel = addEducationLevelDTO.EducationLevel,
                EducationLevelId = selectedEducationLevel.id
            };

            _applicantDBContext.EducationDocuments.Add(educationLevel);
            await _applicantDBContext.SaveChangesAsync();
        }


        public async Task AddPassport(AddPassportDTO addPassportDTO, Guid userId)
        {
            var passport = new Passport
            {
                UserId = userId,
                PassportNumber = addPassportDTO.PassportNumber,
                IssuedWhen = addPassportDTO.IssuedWhen,
                IssuedWhom = addPassportDTO.IssuedWhom,
                BirthPlace = addPassportDTO.BirthPlace,
            };
            var currentPassport = await _applicantDBContext.Passports.FirstOrDefaultAsync(p => p.PassportNumber == passport.PassportNumber);
            if (currentPassport != null)
            {
                throw new BadRequestException("This passport is already in  system");
            }
            _applicantDBContext.Passports.Add(passport);
            await _applicantDBContext.SaveChangesAsync();
        }
        public async Task DeleteEducationInformation(Guid userId)
        {
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var education = await _applicantDBContext.EducationDocuments.FirstOrDefaultAsync(eD => eD.UserId == userId);
            if (education == null)
            {
                throw new NotFoundException("This education document not found!");
            }
            _applicantDBContext.EducationDocuments.Remove(education);
            await _applicantDBContext.SaveChangesAsync();
        }

        public async Task DeletePassportInformation(Guid userId)
        {
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var passport = await _applicantDBContext.Passports.FirstOrDefaultAsync(pD => pD.UserId == userId);
            if (passport == null)
            {
                throw new NotFoundException("This passport not found!");
            }
            _applicantDBContext.Passports.Remove(passport);
            await _applicantDBContext.SaveChangesAsync();
        }

        public async Task<GetEducationInformationDTO> EditEducationInformation(AddEducationLevelDTO editEducationLevelDTO, Guid userId)
        {
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var education = await _applicantDBContext.EducationDocuments.FirstOrDefaultAsync(eD => eD.UserId == userId);
            var sameDocument = await _applicantDBContext.EducationDocuments.FirstOrDefaultAsync(eD => eD.UserId == userId && eD.EducationLevel == editEducationLevelDTO.EducationLevel);
            if(sameDocument != null)
            {
                throw new BadRequestException("You already have document like this!");
            }
            if (education == null)
            {
                throw new NotFoundException("This education document not found!");
            }
            if (editEducationLevelDTO.EducationLevel != null)
            {
                education.EducationLevel = editEducationLevelDTO.EducationLevel;
                var educationLevels = await _bus.Rpc.RequestAsync<Guid, List<GetEducationLevelsDTO>>(userId, c => c.WithQueueName("getEducationLevelsFromEL"));
                var educationLevelDescription = editEducationLevelDTO.EducationLevel.GetDescription();
                var selectedEducationLevel = educationLevels.FirstOrDefault(ed => ed.name == educationLevelDescription);
                education.EducationLevelId = selectedEducationLevel.id;
            }
            await _applicantDBContext.SaveChangesAsync();
            var syncInfo = new SyncEducationLevelDTO
            {
                UserId = userId,
                EducationDocumentId = education.Id,
                EducationLevel = education.EducationLevel
            };
            await SyncEducationLevel(syncInfo);
            var educationInfo = new GetEducationInformationDTO
            {
                id = education.Id,
                EducationLevel = education.EducationLevel
            };
            return educationInfo;
        }
        private async Task SyncEducationLevel(SyncEducationLevelDTO syncEducationLevel)
        {
            var message = new SyncEducationLevelDTO
            {
                UserId = syncEducationLevel.UserId,
                EducationDocumentId = syncEducationLevel.EducationDocumentId,
                EducationLevel = syncEducationLevel.EducationLevel,
            };
            await _bus.PubSub.PublishAsync(message, "educationLevelSyncEntrance");
        }

        public async Task<GetPassportInformationDTO> EditPaspportInformation(AddPassportDTO editPassportDTO, Guid userId)
        {
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var passport = await _applicantDBContext.Passports.FirstOrDefaultAsync(pD => pD.UserId == userId);
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
                PassportId = passport.Id,
                birthPlace = passport.BirthPlace,
                issuedWhen = passport.IssuedWhen,
                issuedWhom = passport.IssuedWhom,
                passportNumber = passport.PassportNumber,
            };
            var syncInfo = new SyncPassportDTO
            {
                PassportId = passport.Id,
                UserId = passport.UserId,
                birthPlace = passport.BirthPlace,
                issuedWhen = passport.IssuedWhen,
                issuedWhom = passport.IssuedWhom,
                passportNumber = passport.PassportNumber,
            };
            await SyncPassport(syncInfo);

            return editedPassportInfo;
        }
        private async Task SyncPassport(SyncPassportDTO syncPassportDTO)
        {
            var message = new SyncPassportDTO
            {
                UserId = syncPassportDTO.UserId,
                birthPlace = syncPassportDTO.birthPlace,
                issuedWhen = syncPassportDTO.issuedWhen,
                issuedWhom = syncPassportDTO.issuedWhom,
                PassportId = syncPassportDTO.PassportId,
                passportNumber = syncPassportDTO.passportNumber
            };
            await _bus.PubSub.PublishAsync(message, "passportSyncEntrance");
        }
        public async Task<GetEducationInformationDTO> GetEducationInformation(Guid userId)
        {
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var education = await _applicantDBContext.EducationDocuments.FirstOrDefaultAsync(eD => eD.UserId == userId);
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

        public async Task<GetPassportInformationDTO> GetPassportInformation(Guid userId)
        {
            if (userId == null)
            {
                throw new ForbiddenException("This user not found in system!");
            }
            var passport = await _applicantDBContext.Passports.FirstOrDefaultAsync(pD => pD.UserId == userId);
            if (passport == null)
            {
                throw new NotFoundException("This passport not found!");
            }
            var passportInfo = new GetPassportInformationDTO
            {
                PassportId = passport.Id,
                birthPlace = passport.BirthPlace,
                issuedWhen = passport.IssuedWhen,
                issuedWhom = passport.IssuedWhom,
                passportNumber = passport.PassportNumber,
            };
            return passportInfo;
        }
    }
}
