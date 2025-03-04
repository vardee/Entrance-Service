﻿using adv_Backend_Entrance.Common.DTO.EntranceService;
using Microsoft.Extensions.Configuration;
using System.Text;
using adv_Backend_Entrance.EntranceService.DAL.Data;
using adv_Backend_Entrance.EntranceService.DAL.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using System.Text.Json;
using adv_Backend_Entrance.Common.Enums;
using Microsoft.EntityFrameworkCore;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.Interfaces.EntranceService;
using Microsoft.EntityFrameworkCore.Storage.Json;
using static System.Net.Mime.MediaTypeNames;
using adv_Backend_Entrance.Common.DTO;
using System.Runtime.CompilerServices;
using EasyNetQ;
using JsonSerializer = System.Text.Json.JsonSerializer;
using adv_Backend_Entrance.UserService.DAL.Data.Entities;
using adv_Backend_Entrance.Common.DTO.EntranceService.Applicant;
using Newtonsoft.Json.Linq;

namespace adv_Backend_Entrance.EntranceService.BL.Services
{
    public class ApplicantEntranceService : IEntranceService
    {
        private readonly EntranceDBContext _entranceDBContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly TokenHelper _tokenHelper;
        private readonly IBus _bus;

        public ApplicantEntranceService(EntranceDBContext entranceDBContext, IConfiguration configuration, HttpClient httpClient, TokenHelper tokenHelper)
        {
            _entranceDBContext = entranceDBContext;
            _configuration = configuration;
            _httpClient = httpClient;
            _tokenHelper = tokenHelper;
            _bus = RabbitHutch.CreateBus("host=localhost");
            var token = _tokenHelper.GetTokenFromHeader();

        }

        public async Task CreateApplication(CreateApplicationDTO createApplicationDTO, string token)
        {
            string userId = _tokenHelper.GetUserIdFromToken(token);
            var passportInfo = await _bus.Rpc.RequestAsync<Guid, GetPassportInformationDTO>(Guid.Parse(userId), x => x.WithQueueName("application_passport"));
            if (passportInfo == null)
            {
                throw new BadRequestException("Server response is bad, server problems. Oops!");
            }

            var educationInfo = await _bus.Rpc.RequestAsync<Guid, GetEducationInformationDTO>(Guid.Parse(userId), x => x.WithQueueName("application_educationInfo"));
            if (educationInfo == null)
            {
                throw new BadRequestException("You don't have an education document!");
            }

            if (educationInfo.id != createApplicationDTO.EducationId)
            {
                throw new BadRequestException("Your education document is not valid! Check your document value and retry!");
            }

            if (passportInfo.PassportId != createApplicationDTO.PassportId)
            {
                throw new BadRequestException("Your passport is not valid! Check your passport value and retry!");
            }

            var userProfileResponse = await _bus.Rpc.RequestAsync<Guid, UserGetProfileDTO>(Guid.Parse(userId), x => x.WithQueueName("application_profileResponse"));
            if (userProfileResponse != null)
            {
                var applicant = new ApplicantModel
                {
                    FirstName = userProfileResponse.firstname,
                    LastName = userProfileResponse.lastname,
                    Patronymic = userProfileResponse.patronymic,
                    EducationId = createApplicationDTO.EducationId,
                    PassportId = createApplicationDTO.PassportId,
                    PassportNumber = passportInfo.passportNumber,
                    UserId = Guid.Parse(userId)
                };
                _entranceDBContext.Applicants.Add(applicant);

                var application = new ApplicationModel
                {
                    ApplicantId = applicant.Id,
                    ApplicationStatus = EntranceApplicationStatus.Created,
                    CreatedTime = DateTime.UtcNow,
                    ChangedTime = DateTime.UtcNow,
                };
                _entranceDBContext.Applications.Add(application);

                if (createApplicationDTO.ProgramsPriority.Count() < 1)
                {
                    throw new BadRequestException("You must choose at least 1 program in your application!");
                }

                if (createApplicationDTO.ProgramsPriority.Count() > 5)
                {
                    throw new BadRequestException("You can't choose more than 5 programs in your application!");
                }

                var documentTypes = await _bus.Rpc.RequestAsync<Guid, List<GetDocumentTypesDTO>>(Guid.Parse(userId), x => x.WithQueueName("application_facultyDocuments"));
                var programsWithPagination = await _bus.Rpc.RequestAsync<Guid, GetQuerybleProgramsDTO>(Guid.Parse(userId), x => x.WithQueueName("application_facultyPrograms"));

                var nextEducationLevels = documentTypes
                    .Where(dt => dt.educationLevel.id == educationInfo.educationId)
                    .SelectMany(dt => dt.nextEducationLevels)
                    .ToList();

                var usedPriorities = new HashSet<int>();
                var selectedProgramLevels = new HashSet<int>();

                foreach (var program in createApplicationDTO.ProgramsPriority)
                {
                    if (program.Priority < 1 || program.Priority > createApplicationDTO.ProgramsPriority.Count())
                    {
                        throw new BadRequestException("You can't put priority less than 1, and more than the count of your chosen programs");
                    }

                    if (usedPriorities.Contains(program.Priority))
                    {
                        throw new BadRequestException("You cannot use the same priority for multiple programs!");
                    }
                    usedPriorities.Add(program.Priority);

                    var programs = programsWithPagination.Programs;
                    var matchingProgram = programs.FirstOrDefault(p => p.id == program.ProgramId);
                    if (matchingProgram == null)
                    {
                        throw new BadRequestException("This program was not found!");
                    }

                    if (matchingProgram.educationLevel.id != educationInfo.educationId && !nextEducationLevels.Any(e => e.id == matchingProgram.educationLevel.id))
                    {
                        throw new BadRequestException("You cannot apply for this level of education!");
                    }

                    if (selectedProgramLevels.Count > 0 && !selectedProgramLevels.Contains(matchingProgram.educationLevel.id))
                    {
                        throw new BadRequestException("All selected programs must be of the same education level!");
                    }
                    selectedProgramLevels.Add(matchingProgram.educationLevel.id);

                    var applicationProgram = new ApplicationProgram
                    {
                        ApplicationId = application.Id,
                        ProgramId = program.ProgramId,
                        ProgramName = matchingProgram.name,
                        Priority = program.Priority,
                        ProgramCode = matchingProgram.code,
                        FacultyId = matchingProgram.faculty.id,
                        FacultyName = matchingProgram.faculty.name,
                    };
                    _entranceDBContext.ApplicationPrograms.Add(applicationProgram);
                }

                await _entranceDBContext.SaveChangesAsync();
                await AddUserRole(Guid.Parse(userId));
            }
        }




        private async Task AddUserRole(Guid userId)
        {
            var message = new AddRoleDTO { UserId = userId, Role = RoleType.Applicant };
            await _bus.PubSub.PublishAsync(message, "application_created");
        }

        public async Task DeleteProgramFromApplication(DeleteProgramFromApplicationDTO deleteProgramFromApplicationDTO)
        {
            var userProgram = await _entranceDBContext.ApplicationPrograms.FirstOrDefaultAsync(aP => aP.ApplicationId == deleteProgramFromApplicationDTO.ApplicationId && aP.ProgramId == deleteProgramFromApplicationDTO.ProgramId);
            if (userProgram == null)
            {
                throw new BadRequestException("You don't have this program in your application!");
            }

            var maxPriority = await _entranceDBContext.ApplicationPrograms
                .Where(ap => ap.ApplicationId == deleteProgramFromApplicationDTO.ApplicationId)
                .MaxAsync(ap => ap.Priority);

            _entranceDBContext.ApplicationPrograms.Remove(userProgram);
            await _entranceDBContext.SaveChangesAsync();

            var remainingPrograms = await _entranceDBContext.ApplicationPrograms
                .Where(ap => ap.ApplicationId == deleteProgramFromApplicationDTO.ApplicationId && ap.ProgramId != deleteProgramFromApplicationDTO.ProgramId)
                .ToListAsync();
            if (userProgram.Priority != maxPriority)
            {
                foreach (var program in remainingPrograms)
                {
                    if (program.Priority != 1)
                    {
                        program.Priority--;
                    }
                }
            }
            await _entranceDBContext.SaveChangesAsync();
        }
        public async Task AddProgramsInApplication(AddProgramsDTO addProgramsDTO, string token)
        {
            string userId = _tokenHelper.GetUserIdFromToken(token);
            var userProgram = await _entranceDBContext.ApplicationPrograms.Where(ap => ap.ApplicationId == addProgramsDTO.ApplicationId).ToListAsync();
            if(userProgram == null)
            {
                throw new BadRequestException("You don't have any applications available!");
            }
            if (userProgram.Count() == 5)
            {
                throw new BadRequestException("You have maximum programs in your application, delete usless programs from application!");
            }
            if (addProgramsDTO.Programs.Count() < 1)
            {
                throw new BadRequestException("You must choose at least 1 program in your application!");
            }
            if (addProgramsDTO.Programs.Count() > 5)
            {
                throw new BadRequestException("You cant choose more than 5 program in your application!");
            }
            var educationInfo = await _bus.Rpc.RequestAsync<Guid, GetEducationInformationDTO>(Guid.Parse(userId), x => x.WithQueueName("application_educationInfo"));
            if (educationInfo == null)
            {
                throw new BadRequestException("You dont have education document!");
            }
            var documentTypes = await _bus.Rpc.RequestAsync<Guid, List<GetDocumentTypesDTO>>(Guid.Parse(userId), x => x.WithQueueName("application_facultyDocuments"));
            var nextEducationLevels = documentTypes
                .Where(dt => dt.educationLevel.id == educationInfo.educationId)
                .Select(dt => dt.nextEducationLevels)
                .FirstOrDefault();
            var queryblePrograms = await _bus.Rpc.RequestAsync<Guid, GetQuerybleProgramsDTO>(Guid.Parse(userId), x => x.WithQueueName("application_facultyPrograms"));
            foreach (var program in addProgramsDTO.Programs)
            {
                var currentProgram = await _entranceDBContext.ApplicationPrograms.FirstOrDefaultAsync(aP => aP.ApplicationId == addProgramsDTO.ApplicationId && program.Priority == aP.Priority);
                if (currentProgram != null)
                {
                    throw new BadRequestException($"Please change program priority {program.Priority}, because you have the same priority program!");
                }
                if (program.Priority < 1 || program.Priority > 5)
                {
                    throw new BadRequestException("You cant put priority less than 1, and more than count of your choosen programs");
                }
                var programs = queryblePrograms.Programs;

                var matchingProgram = programs.FirstOrDefault(p => p.id == program.ProgramId);
                if (matchingProgram != null)
                {
                    if (nextEducationLevels != null && nextEducationLevels.Any() && nextEducationLevels.Select(e => e.id).Contains(matchingProgram.educationLevel.id))
                    {
                        var applicationProgram = new ApplicationProgram
                        {
                            ApplicationId = addProgramsDTO.ApplicationId,
                            ProgramId = program.ProgramId,
                            Priority = program.Priority,
                            ProgramName = matchingProgram.name,
                        };
                        _entranceDBContext.ApplicationPrograms.Add(applicationProgram);
                    }
                    else
                    {
                        throw new BadRequestException("You cannot apply for these levels of education!");
                    }
                }
                else
                {
                    throw new BadRequestException("This program not found!");
                }
            }
            await _entranceDBContext.SaveChangesAsync();
        }
        public async Task ChangeProgramPriority(ChangeProgramPriorityDTO changeProgramPriorityDTO)
        {
            var userPrograms = await _entranceDBContext.ApplicationPrograms.Where(aP => aP.ApplicationId == changeProgramPriorityDTO.ApplicationId).ToListAsync();
            if (userPrograms == null)
            {
                throw new BadRequestException("You don't have any applications available!");
            }
            var currentProgram = await _entranceDBContext.ApplicationPrograms.FirstOrDefaultAsync(sP => sP.ApplicationId == changeProgramPriorityDTO.ApplicationId && sP.ProgramId == changeProgramPriorityDTO.ProgramId);
            if (currentProgram == null){
                throw new BadRequestException("You don't this program in application!");
            }
            var samePriorityProgram = await _entranceDBContext.ApplicationPrograms.FirstOrDefaultAsync(sP => sP.ApplicationId == changeProgramPriorityDTO.ApplicationId && sP.Priority == changeProgramPriorityDTO.ProgramPriority);
            if(samePriorityProgram == null)
            {
                throw new BadRequestException("You cant do this, you dont have program with this prirority in your application!");
            }
            samePriorityProgram.Priority = currentProgram.Priority;
            currentProgram.Priority = changeProgramPriorityDTO.ProgramPriority;
            await _entranceDBContext.SaveChangesAsync();
        }

        public async Task<GetMyApplicationDTO> GetApplication(string token)
        {
            string userId = _tokenHelper.GetUserIdFromToken(token);
            var applicant = await _entranceDBContext.Applicants.FirstOrDefaultAsync(a => a.UserId == Guid.Parse(userId));
            var userProgram = await _entranceDBContext.Applications.FirstOrDefaultAsync(a => a.ApplicantId == applicant.Id);
            var programsPriority = await _entranceDBContext.ApplicationPrograms
                .Where(aP => aP.ApplicationId == userProgram.Id)
                .Select(aP => new GetMyProgramsDTO
                {
                    ProgramId = aP.ProgramId,
                    Priority = aP.Priority,
                    ProgramName = aP.ProgramName,
                    FacultyId = aP.FacultyId,
                    FacultyName = aP.FacultyName,
                    ProgramCode = aP.ProgramCode
                })
                .ToListAsync();
            var myApplication = new GetMyApplicationDTO
            {
                ApplicationId = userProgram.Id,
                ProgramsPriority = programsPriority,
            };
            return myApplication;
        }


    }
}
