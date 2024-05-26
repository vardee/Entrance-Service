using adv_Backend_Entrance.Common.DTO;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Interfaces.FacultyService;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.FacultyService.MVCPanel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.FacultyService.BL.Services
{
    public class FacultyInteractionsService : IFacultyInteractionsService
    {
        private readonly FacultyDBContext _facultyDBContext;
        private readonly IConfiguration _configuration;
        public FacultyInteractionsService(FacultyDBContext facultyDBContext, IConfiguration configuration)
        {
            _facultyDBContext = facultyDBContext;
            _configuration = configuration;
        }

        public async Task<List<GetDocumentTypesDTO>> GetDocumentTypes()
        {
            var documentTypes = await _facultyDBContext.EducationDocumentTypes.ToListAsync();
            var documentTypesDTOList = new List<GetDocumentTypesDTO>();
            foreach (var documentType in documentTypes)
            {
                var educationLevel = await _facultyDBContext.EducationLevelModels.FindAsync(documentType.EducationLevelId);
                var nextEducationLevels = await _facultyDBContext.EducationDocumentTypeNextEducationLevels
                    .Where(x => x.EducationDocumentTypeId == documentType.Id)
                    .Select(x => x.EducationLevelId)
                    .ToListAsync();

                var educationLevelDTO = new GetEducationLevelsDTO
                {
                    id = educationLevel.Id,
                    name = educationLevel.Name,
                };

                var nextEducationLevelsDTO = await _facultyDBContext.EducationLevelModels
                    .Where(x => nextEducationLevels.Contains(x.Id))
                    .Select(x => new GetEducationLevelsDTO
                    {
                        id = x.Id,
                        name = x.Name
                    })
                    .ToListAsync();

                var documentTypeDTO = new GetDocumentTypesDTO
                {
                    id = documentType.Id,
                    createTime = documentType.CreateTime,
                    name = documentType.Name,
                    educationLevel = educationLevelDTO,
                    nextEducationLevels = nextEducationLevelsDTO
                };

                documentTypesDTOList.Add(documentTypeDTO);
            }

            return documentTypesDTOList;
        }

        public async Task<List<GetFacultiesDTO>> GetFaculties()
        {
            var facultiesDTOList = await _facultyDBContext.FacultyModels
                .Select(faculty => new GetFacultiesDTO
                {
                    id = faculty.Id,
                    name = faculty.Name,
                    createTime = faculty.CreateTime
                })
                .ToListAsync();

            return facultiesDTOList;
        }
        public async Task<List<GetEducationLevelsDTO>> GetEducationLevels()
        {
            var educationLevels = await _facultyDBContext.EducationLevelModels.Select(level => new GetEducationLevelsDTO
            {
                id = level.Id,
                name = level.Name,
            }).ToListAsync(); 
            return educationLevels;
        }

        public async Task<GetQuerybleProgramsDTO> GetQueryblePrograms(int size, int page, List<EducationLanguage> LanguageEnum, List<EducationLevel> EducationLevelEnum, List<EducationForm> EducationFormEnum, string? name,string? code, Guid? Id)
        {
            var programsQuery = _facultyDBContext.EducationProgrammModels.AsQueryable();
            var programs = await programsQuery.ToListAsync();
            if (EducationLevelEnum != null && EducationLevelEnum.Any())
            {
                programsQuery = programsQuery.Where(p => EducationLevelEnum.Contains(p.EducationLevelEnum));
            }
            if (LanguageEnum != null && LanguageEnum.Any())
            {
                programsQuery = programsQuery.Where(p => LanguageEnum.Contains(p.LanguageEnum));
            }
            if (EducationFormEnum != null && EducationFormEnum.Any())
            {
                programsQuery = programsQuery.Where(p => EducationFormEnum.Contains(p.EducationFormEnum));
            }
            if (name != null)
            {
                programsQuery = programsQuery.Where(p => p.Name.Contains(name));
            }
            if (code != null)
            {
                programsQuery = programsQuery.Where(p => p.Code.Contains(code));
            }
            if (Id != null)
            {
                programsQuery = programsQuery.Where(p => p.Id == Id);
            }
            int sizeOfPage = size;
            var countOfPages = (int)Math.Ceiling((double)programsQuery.Count() / sizeOfPage);
            if (page <= countOfPages)
            {
                var lowerBound = page == 1 ? 0 : (page - 1) * sizeOfPage;
                if (page < countOfPages)
                {
                    programsQuery = programsQuery.Skip(lowerBound).Take(sizeOfPage);
                }
                else
                {
                    programsQuery = programsQuery.Skip(lowerBound).Take(programsQuery.Count() - lowerBound);
                }
            }
            else
            {
                throw new BadRequestException("Такой страницы нет");
            }

            var pagination = new PaginationInformation
            {
                Current = page,
                Page = countOfPages,
                Size = size
            };

            var programsDTO = new GetQuerybleProgramsDTO
            {
                Programs = programsQuery.ToList().Select(p => new GetProgramsDTO
                {
                    id = p.Id,
                    createTime = p.CreateTime,
                    name = p.Name,
                    code = p.Code,
                    language = p.Language,
                    educationForm = p.EducationForm,
                    faculty = new GetFacultiesDTO
                    {
                        id = p.FacultyId,
                        name = p.FacultyName,
                        createTime = p.FacultyCreateTime
                    },
                    educationLevel = new GetEducationLevelsDTO
                    {
                        id = p.EducationLevelId,
                        name = p.EducationLevelName
                    }
                }).AsQueryable(),
                PaginationInformation = pagination
            };

            return programsDTO;

        }
        public async Task<GetAllQuerybleImportsDTO> GetAllImprots(int size, List<ImportType>? types)
        {
            {
                if (size <= 0)
                {
                    size = 10;
                }
                int page = 1;
                var importsQuery = _facultyDBContext.Imports.AsQueryable();

                if (types != null && types.Any())
                {
                    importsQuery = importsQuery.Where(p => types.Contains(p.Type));
                }


                int totalImports = await importsQuery.CountAsync();
                var countOfPages = (int)Math.Ceiling((double)totalImports / size);

                if (page > countOfPages || totalImports == 0)
                {
                    return new GetAllQuerybleImportsDTO
                    {
                        Imports = Enumerable.Empty<GetImprotsDTO>().AsQueryable(),
                        PaginationInformation = new PaginationInformation
                        {
                            Current = page,
                            Page = countOfPages,
                            Size = size
                        }
                    };
                }

                var imports = await importsQuery
                    .Skip((page - 1) * size)
                    .Take(size)
                .ToListAsync();

                var importDTO = new List<GetImprotsDTO>();

                foreach (var import in imports)
                {

                    importDTO.Add(new GetImprotsDTO
                    {
                        ImportWas = import.ImportWas,
                        Status = import.Status,
                        Type = import.Type,

                    });
                }

                return new GetAllQuerybleImportsDTO
                {
                    Imports = importDTO.AsQueryable(),
                    PaginationInformation = new PaginationInformation
                    {
                        Current = page,
                        Page = countOfPages,
                        Size = size
                    }
                };
            }
        }

    }
}
