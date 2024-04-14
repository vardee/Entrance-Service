﻿using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using adv_Backend_Entrance.Common.DTO;
using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.FacultyService.DAL.Data.Models;
using adv_Backend_Entrance.FacultyService.DAL.Migrations;
using adv_Backend_Entrance.FacultyService.MVCPanel.Data;
using adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace adv_Backend_Entrance.FacultyService.BL.Services
{
    public class FacultyInformationService : IFacultyService
    {
        private readonly FacultyDBContext _facultyDBContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        private string _baseUrl = "https://1c-mockup.kreosoft.space/api/dictionary/";
        private string _username = "student";
        private string _password = "ny6gQnyn4ecbBrP9l1Fz";

        public FacultyInformationService(FacultyDBContext facultyDBContext, IConfiguration configuration, HttpClient httpClient)
        {
            _facultyDBContext = facultyDBContext;
            _configuration = configuration;
            _httpClient = httpClient;

            var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Credentials);
        }

        private async Task GetEducationLevels()
        {
            string endpoint = "education_levels";
            string _apiUrl = _baseUrl + endpoint;
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
                        _facultyDBContext.EducationLevelModels.Add(new EducationLevelModel
                        {
                            Id = level.id,
                            Name = level.name
                        });
                    }
                    else
                    {

                        Console.WriteLine($"Record with ID {level.id} already exists.");
                    }
                }

                await _facultyDBContext.SaveChangesAsync();
            }
            else
            {
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
                        var newDocumentType = new EducationDocumentTypeModel
                        {
                            Id = documentType.id,
                            Name = documentType.name,
                            CreateTime = createTimeUtc,
                            EducationLevelId = documentType.educationLevel.id,
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
                                    EducationLevelId = nextLevel.id
                                });
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Record with ID {documentType.id} already exists.");
                    }
                }

                await _facultyDBContext.SaveChangesAsync();
            }
            else
            {
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
            }
            else
            {
                throw new BadRequestException("Bad request bruh!");
            }

        }
        private async Task GetPrograms()
        {
            string endpoint = "programs?page=1&size=10";
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

                    var existingProgram = await _facultyDBContext.EducationProgrammModels.FindAsync(program.id);
                    if (existingProgram == null)
                    {
                        _facultyDBContext.EducationProgrammModels.Add(new EducationProgrammModel
                        {
                            Id = program.id,
                            CreateTime = createTimeUtc,
                            Name = program.name,
                            Code = program.code,
                            Language = program.language,
                            EducationForm = program.educationForm,
                            FacultyId = program.faculty.id,
                            EducationLevelId = program.educationLevel.id
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Record with ID {program.faculty.id} already exists.");
                    }
                }

                await _facultyDBContext.SaveChangesAsync();
            }
            else
            {
                throw new BadRequestException("Bad request bruh!");
            }
        }
        public async Task GetDictionary()
        {

        }


    }
}
