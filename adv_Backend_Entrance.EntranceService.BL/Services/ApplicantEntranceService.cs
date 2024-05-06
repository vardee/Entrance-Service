using adv_Backend_Entrance.Common.DTO.EntranceService;
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
        private string _getEducationInformation;
        private string _getPassportInformation;
        private string _getDocumentTypes;
        public ApplicantEntranceService(EntranceDBContext entranceDBContext, IConfiguration configuration, HttpClient httpClient, TokenHelper tokenHelper)
        {
            _entranceDBContext = entranceDBContext;
            _configuration = configuration;
            _httpClient = httpClient;
            _tokenHelper = tokenHelper;
            _baseUrl = _configuration.GetValue<string>("BaseUrl");
            _getProfileUrl = _configuration.GetValue<string>("GetProfileUrl");
            _getEducationInformation = _configuration.GetValue<string>("GetEducationInfo");
            _getPassportInformation = _configuration.GetValue<string>("GetPassportInfo");
            _getDocumentTypes = _configuration.GetValue<string>("GetDocumentTypes");
            var token = _tokenHelper.GetTokenFromHeader();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task CreateApplication(CreateApplicationDTO createApplicationDTO, string token)
        {
            string userId = _tokenHelper.GetUserIdFromToken(token);

            HttpResponseMessage getPassportResponse = await _httpClient.GetAsync(_getPassportInformation);
            if (!getPassportResponse.IsSuccessStatusCode)
            {
                throw new BadRequestException("Server response is bad, server problems. Oops!");
            }
            string getPassportResponseBody = await getPassportResponse.Content.ReadAsStringAsync();
            var userPassportResponse = JsonSerializer.Deserialize<GetPassportInformationDTO>(getPassportResponseBody);
            Console.WriteLine(userPassportResponse);
            if (getPassportResponseBody == null)
            {
                throw new NotFoundException("Your passport not found!");
            }
            HttpResponseMessage getEducationResponse = await _httpClient.GetAsync(_getEducationInformation);
            if (!getEducationResponse.IsSuccessStatusCode)
            {
                throw new BadRequestException("Server response is bad, server problems. Oops!");
            }
            string getEducationResponseBody = await getEducationResponse.Content.ReadAsStringAsync();
            var userEducationResponse = JsonSerializer.Deserialize<GetEducationInformationDTO>(getEducationResponseBody);
            Console.WriteLine(userEducationResponse);
            if (getEducationResponseBody == null)
            {
                throw new NotFoundException("Your education document not found!");
            }
            if (userEducationResponse.id != createApplicationDTO.EducationId)
            {
                throw new BadRequestException("Your education document is not valid! Check your document value and retry!");
            }
            if (userPassportResponse.passportNumber != createApplicationDTO.PassportId)
            {
                throw new BadRequestException("Your passport is not valid! Check your passport value and retry!");
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
                HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl);
                if (createApplicationDTO.ProgramsPriority.Count() < 1 )
                {
                    throw new BadRequestException("You must choose at least 1 program in your application!");
                }
                if (createApplicationDTO.ProgramsPriority.Count() > 5 )
                {
                    throw new BadRequestException("You cant choose more than 5 program in your application!");
                }
                HttpResponseMessage getDocumentTypesReponse = await _httpClient.GetAsync(_getDocumentTypes);
                if (!getPassportResponse.IsSuccessStatusCode)
                {
                    throw new BadRequestException("Server response is bad, server problems. Oops!");
                }
                string getDocumentTypes = await getDocumentTypesReponse.Content.ReadAsStringAsync();
                var documentTypes = JsonSerializer.Deserialize<List<GetDocumentTypesDTO>>(getDocumentTypes);
                var nextEducationLevels = documentTypes
                    .Where(dt => dt.educationLevel.id == userEducationResponse.educationId)
                    .Select(dt => dt.nextEducationLevels)
                    .FirstOrDefault();
                foreach (var program in createApplicationDTO.ProgramsPriority)
                {
                    if(program.Priority < 1 && program.Priority > createApplicationDTO.ProgramsPriority.Count())
                    {
                        throw new BadRequestException("You cant put priority less than 1, and more than count of your choosen programs");
                    }
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
                                if (nextEducationLevels != null && nextEducationLevels.Any() && nextEducationLevels.Select(e => e.id).Contains(programApp.educationLevel.id))
                                {
                                    var applicationProgram = new ApplicationProgram
                                    {
                                        ApplicationId = application.Id,
                                        ProgramId = program.ProgramId,
                                        Priority = program.Priority,
                                    };
                                    _entranceDBContext.ApplicationPrograms.Add(applicationProgram);
                                }
                                else
                                {
                                    throw new BadRequestException("You cannot apply for these levels of education!");
                                }
                            }
                        }
                    }
                    await _entranceDBContext.SaveChangesAsync();
                }
            }
        }

    }
}
