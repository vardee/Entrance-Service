using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.FacultyService.DAL.Data.Models;
using adv_Backend_Entrance.FacultyService.MVCPanel.Data;
using adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using adv_Backend_Entrance.Common.Enums;
using System.Xml.Linq;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using RabbitMQ.Client;
using adv_Backend_Entrance.Common.DTO.NotificationService;
using adv_Backend_Entrance.Common.Interfaces.FacultyService;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace adv_Backend_Entrance.FacultyService.BL.Services
{
    public class FacultyInformationService : IFacultyService
    {
        private readonly FacultyDBContext _facultyDBContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        private string _baseUrl;
        private string _username;
        private string _password;
        public FacultyInformationService(FacultyDBContext facultyDBContext, IConfiguration configuration, HttpClient httpClient)
        {
            _facultyDBContext = facultyDBContext;
            _configuration = configuration;
            _httpClient = httpClient;
            _baseUrl = _configuration.GetValue<string>("BaseUrl");
            _username = "student";
            _password = _configuration.GetValue<string>("Password");


            var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Credentials);
        }


        private async Task GetEducationLevels()
        {
            string endpoint = "education_levels";
            string _apiUrl = _baseUrl + endpoint;
            Console.WriteLine(_username);
            Console.WriteLine(_password);
            HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var educationLevels = JsonSerializer.Deserialize<List<GetEducationLevelsDTO>>(responseBody);
                Console.WriteLine(responseBody);

                foreach (var level in educationLevels)
                {
                    Console.WriteLine($"Id: {level.id}, Name: {level.name}");

                    var existingLevel = await _facultyDBContext.EducationLevelModels.FindAsync(level.id);
                    if (existingLevel == null)
                    {
                        if (EducationChecker.TryParseEducationLevel(level.name, out EducationLevel enumValue))
                        {
                            _facultyDBContext.EducationLevelModels.Add(new EducationLevelModel
                            {
                                Id = level.id,
                                Name = level.name,
                                EducationLevelName = enumValue
                            });
                        }
                        else
                        {
                            Console.WriteLine($"Failed to parse EducationLevel from: {level.name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Record with ID {level.id} already exists.");
                    }
                }

                await _facultyDBContext.SaveChangesAsync();
                await AddImportStatus(ImportStatus.Imported, ImportType.EducationLevels);
            }
            else
            {
                Console.WriteLine("Error occurred while fetching education levels.");
                await AddImportStatus(ImportStatus.Failed, ImportType.EducationLevels);
                throw new BadRequestException("Bad request bruh!");
            }
        }

        private async Task GetDocumentType()
        {
            string endpoint = "document_types";
            string _apiUrl = _baseUrl + endpoint;
            HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                var documentTypes = JsonSerializer.Deserialize<List<GetDocumentTypesDTO>>(responseBody);

                Console.WriteLine(responseBody);

                foreach (var documentType in documentTypes)
                {
                    DateTime createTimeUtc = documentType.createTime.ToUniversalTime();
                    var existingDocumentType = await _facultyDBContext.EducationDocumentTypes.FindAsync(documentType.id);
                    if (existingDocumentType == null)
                    {
                        if (EducationChecker.TryParseEducationLevel(documentType.educationLevel.name, out EducationLevel enumValue))
                        {
                            var newDocumentType = new EducationDocumentTypeModel
                            {
                                Id = documentType.id,
                                Name = documentType.name,
                                CreateTime = createTimeUtc,
                                EducationLevelId = documentType.educationLevel.id,
                                EducationLevelName = documentType.educationLevel.name,
                                EducationLevelEnum = enumValue
                            };

                            _facultyDBContext.EducationDocumentTypes.Add(newDocumentType);

                            foreach (var nextLevel in documentType.nextEducationLevels)
                            {
                                var educationLevel = await _facultyDBContext.EducationLevelModels.FindAsync(nextLevel.id);
                                if (educationLevel != null)
                                {
                                    _facultyDBContext.EducationDocumentTypeNextEducationLevels.Add(new EducationDocumentTypeNextEducationLevel
                                    {
                                        EducationDocumentTypeId = documentType.id,
                                        EducationLevelId = nextLevel.id,
                                        EducationLevelName = nextLevel.name,
                                    });
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to parse EducationLevel from: {documentType.educationLevel.name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Record with ID {documentType.id} already exists.");
                    }
                }

                await _facultyDBContext.SaveChangesAsync();
                await AddImportStatus(ImportStatus.Imported, ImportType.DocumentTypes);
            }
            else
            {
                Console.WriteLine("Error occurred while fetching document types.");
                await AddImportStatus(ImportStatus.Failed, ImportType.DocumentTypes);
                throw new BadRequestException("Bad request bruh!");
            }
        }

        private async Task GetFaculties()
        {
            string endpoint = "faculties";
            string _apiUrl = _baseUrl + endpoint;
            HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var faculties = JsonSerializer.Deserialize<List<GetFacultiesDTO>>(responseBody);
                Console.WriteLine(responseBody);

                foreach (var faculty in faculties)
                {
                    Console.WriteLine($"Id: {faculty.id}, Name: {faculty.name}");
                    DateTime createTimeUtc = faculty.createTime.ToUniversalTime();

                    var existingFaculty = await _facultyDBContext.FacultyModels.FindAsync(faculty.id);
                    if (existingFaculty == null)
                    {
                        _facultyDBContext.FacultyModels.Add(new FacultyModel
                        {
                            Id = faculty.id,
                            CreateTime = createTimeUtc,
                            Name = faculty.name
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Record with ID {faculty.id} already exists.");
                    }
                }

                await _facultyDBContext.SaveChangesAsync();
                await AddImportStatus(ImportStatus.Imported, ImportType.Faculties);
            }
            else
            {
                Console.WriteLine("Error occurred while fetching faculties.");
                await AddImportStatus(ImportStatus.Failed, ImportType.Faculties);
                throw new BadRequestException("Bad request bruh!");
            }

        }

        private async Task GetPrograms()
        {
            string endpoint = "programs?page=1&size=10000";
            string _apiUrl = _baseUrl + endpoint;
            HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var programsWithPagination = JsonSerializer.Deserialize<GetProgramsWithPaginationDTO>(responseBody);
                var programs = programsWithPagination.programs;

                Console.WriteLine(responseBody);

                foreach (var program in programs)
                {
                    Console.WriteLine($"Id: {program.id}, Name: {program.name}");

                    DateTime createTimeUtc = program.createTime.ToUniversalTime();
                    DateTime createFacultyTime = program.faculty.createTime.ToUniversalTime();

                    var existingProgram = await _facultyDBContext.EducationProgrammModels.FindAsync(program.id);
                    if (existingProgram == null)
                    {
                        if (EducationChecker.TryParseEducationLanguage(program.language, out EducationLanguage languageEnum) &&
                            EducationChecker.TryParseEducationForm(program.educationForm, out EducationForm formEnum)
                            && EducationChecker.TryParseEducationLevel(program.educationLevel.name, out EducationLevel enumValue))
                        {
                            _facultyDBContext.EducationProgrammModels.Add(new EducationProgrammModel
                            {
                                Id = program.id,
                                CreateTime = createTimeUtc,
                                Name = program.name,
                                Code = program.code,
                                Language = program.language,
                                LanguageEnum = languageEnum,
                                EducationForm = program.educationForm,
                                EducationFormEnum = formEnum,
                                FacultyId = program.faculty.id,
                                EducationLevelId = program.educationLevel.id,
                                EducationLevelEnum = enumValue,
                                EducationLevelName = program.educationLevel.name,
                                FacultyCreateTime = createFacultyTime,
                                FacultyName = program.faculty.name
                            });
                        }
                        else
                        {
                            Console.WriteLine($"Failed to parse language or education form for program ID {program.id}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Record with ID {program.faculty.id} already exists.");
                    }
                }

                await _facultyDBContext.SaveChangesAsync();
                await AddImportStatus(ImportStatus.Imported, ImportType.Programs);
            }
            else
            {
                await AddImportStatus(ImportStatus.Failed, ImportType.Programs);
                throw new BadRequestException("Bad request bruh!");
            }
        }

        private async Task AddImportStatus(ImportStatus status, ImportType type)
        {
            var import = new Import { Id = Guid.NewGuid(), Status = status, ImportWas = DateTime.UtcNow, Type = type };
            _facultyDBContext.Imports.Add(import);
            await _facultyDBContext.SaveChangesAsync();
        }

        public async Task GetDictionary(List<ImportType>? importTypes)
        {
            try
            {
                if (importTypes == null)
                {
                    await GetEducationLevels();
                    await GetDocumentType();
                    await GetFaculties();
                    await GetPrograms();
                }
                else if (importTypes != null && importTypes.Contains(ImportType.DocumentTypes))
                {
                    await GetDocumentType();
                }
                else if (importTypes != null && importTypes.Contains(ImportType.EducationLevels))
                {
                    await GetEducationLevels();
                }
                else if (importTypes != null && importTypes.Contains(ImportType.Faculties))
                {
                    await GetFaculties();
                }
                else if (importTypes != null && importTypes.Contains(ImportType.Programs))
                {
                    await GetPrograms();
                }
            }
            catch (BadRequestException ex)
            {

                throw;
            }
        }
    }
}
