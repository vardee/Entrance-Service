using adv_Backend_Entrance.Common.DTO;
using adv_Backend_Entrance.Common.DTO.EntranceService.Applicant;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.EntranceService;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.EntranceService.DAL.Data;
using adv_Backend_Entrance.EntranceService.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace adv_Backend_Entrance.EntranceService.BL.Services
{
    public class ManagerEntranceService : IManagerService
    {
        private readonly EntranceDBContext _entranceDBContext;
        private readonly TokenHelper _tokenHelper;
        public ManagerEntranceService(EntranceDBContext entranceDBContext, TokenHelper tokenHelper)
        {
            _entranceDBContext = entranceDBContext;
            _tokenHelper = tokenHelper;
        }

        public async Task TakeApplication(TakeApplicationDTO takeApplicationDTO, Guid userId)
        {
            var application = await _entranceDBContext.Applications.FirstOrDefaultAsync(a => a.Id == takeApplicationDTO.ApplicationId);
            if (application == null)
            {
                throw new BadRequestException("This application not found!");
            }
            application.ManagerId = userId;
            application.ApplicationStatus = EntranceApplicationStatus.UnderConsideration;
            await _entranceDBContext.SaveChangesAsync();
        }

        public async Task RejectApplication(RejectApplicationDTO rejectApplicationDTO, Guid userId)
        {
            var application = await _entranceDBContext.Applications.FirstOrDefaultAsync(a => a.Id == rejectApplicationDTO.ApplicationId);
            if (application.ManagerId != userId)
            {
                throw new BadRequestException("You cant reject not your application!");
            }
            if (application == null)
            {
                throw new BadRequestException("This application not found!");
            }
            application.ManagerId = Guid.Empty;
            application.ApplicationStatus = EntranceApplicationStatus.Created;
            await _entranceDBContext.SaveChangesAsync();
        }

        private IQueryable<ApplicationModel> FilterApplications(Sorting? timeSorting, IQueryable<ApplicationModel> applications)
        {
            switch (timeSorting)
            {
                case Sorting.CreateAsc:
                    return applications.OrderBy(p => p.ChangedTime);
                default:
                    return applications.OrderByDescending(p => p.ChangedTime);
            }

        }

        public async Task<GetAllQuerybleApplicationsDTO> GetQuerybleApplications(int size, int page, string? name, Guid? ProgramId, List<Guid>? Faculties, List<EntranceApplicationStatus>? entranceApplicationStatuses, bool? haveManager, bool? isMy, Guid? managerId, Sorting? timeSorting)
        {
            var applicationsQuery = _entranceDBContext.Applications.AsQueryable();
            if (ProgramId != null)
            {
                var programs = await _entranceDBContext.ApplicationPrograms.Where(aP => aP.ProgramId == ProgramId).ToListAsync();
                if (programs.Any())
                {
                    foreach (var program in programs)
                    {
                        applicationsQuery = applicationsQuery.Where(p => p.Id == program.ApplicationId);
                    }
                }
                else
                {
                    throw new BadRequestException("This program not found!");
                }
            }
            if (Faculties.Any() && Faculties != null)
            {
                var programs = await _entranceDBContext.ApplicationPrograms.Where(aP => Faculties.Contains(aP.FacultyId)).ToListAsync();
                if (programs.Any())
                {
                    foreach (var program in programs)
                    {
                        applicationsQuery = applicationsQuery.Where(p => p.Id == program.ApplicationId);
                    }
                }
                else
                {
                    throw new BadRequestException("This program not found!");
                }
            }
            if (entranceApplicationStatuses != null && entranceApplicationStatuses.Any())
            {
                applicationsQuery = applicationsQuery.Where(p => entranceApplicationStatuses.Contains(p.ApplicationStatus));
            }
            if (haveManager == true)
            {
                applicationsQuery = applicationsQuery.Where(aP => aP.ManagerId != null);
            }
            if (isMy == true)
            {
                applicationsQuery = applicationsQuery.Where(aP => aP.ManagerId == managerId);
            }
            applicationsQuery = FilterApplications(timeSorting, applicationsQuery);
            int sizeOfPage = size;
            var countOfPages = (int)Math.Ceiling((double)applicationsQuery.Count() / sizeOfPage);
            if (page <= countOfPages)
            {
                var lowerBound = page == 1 ? 0 : (page - 1) * sizeOfPage;
                if (page < countOfPages)
                {
                    applicationsQuery = applicationsQuery.Skip(lowerBound).Take(sizeOfPage);
                }
                else
                {
                    applicationsQuery = applicationsQuery.Skip(lowerBound).Take(applicationsQuery.Count() - lowerBound);
                }
            }
            else
            {
                throw new BadRequestException("This page not found!");
            }

            var pagination = new PaginationInformation
            {
                Current = page,
                Page = countOfPages,
                Size = size
            };

            var applications = await applicationsQuery.ToListAsync();
            var applicationsDTO = new GetAllQuerybleApplicationsDTO
            {
                Applications = applications.Select(p =>
                {
                    var programsPriority = _entranceDBContext.ApplicationPrograms
                        .Where(ap => ap.ApplicationId == p.Id)
                        .Select(ap => new GetApplicationPriorityDTO
                        {
                            ApplicationId = ap.ApplicationId,
                            ProgramId = ap.ProgramId,
                            Priority = ap.Priority,
                            ProgramName = ap.ProgramName,
                            FacultyName = ap.FacultyName,
                            FacultyId = ap.FacultyId,
                            ProgramCode = ap.ProgramCode
                        }).ToList();

                    return new GetApplicationsDTO
                    {
                        ApplicationId = p.Id,
                        ProgramsPriority = programsPriority
                    };
                }).AsQueryable(),
                PaginationInformation = pagination
            };
            return applicationsDTO;
        }

        public async Task ChangeApplicationStatus(ChangeApplicationStatusDTO changeApplicationStatusDTO, Guid userId)
        {
            var application = await _entranceDBContext.Applications.FirstOrDefaultAsync(a => a.Id == changeApplicationStatusDTO.ApplicationId);
            if (application.ManagerId != userId)
            {
                throw new BadRequestException("You cant change status in this application!");
            }
            if (application == null)
            {
                throw new BadRequestException("This application not found!");
            }
            application.ApplicationStatus = changeApplicationStatusDTO.Status;
            await _entranceDBContext.SaveChangesAsync();
        }

        public async Task<GetApplicationsDTO> GetApplicantion(GetApplicantDTO getApplicantDTO)
        {
            if (getApplicantDTO.ApplicantId == null)
            {
                throw new NotFoundException("This applicant not found!");
            }
            var application = await _entranceDBContext.Applications.FirstOrDefaultAsync(aP => aP.ApplicantId == getApplicantDTO.ApplicantId);
            if (application == null)
            {
                throw new NotFoundException("This applicant not found");
            }
            var programsPriority = _entranceDBContext.ApplicationPrograms
                .Where(ap => ap.ApplicationId == application.Id)
                .Select(ap => new GetApplicationPriorityDTO
                {
                    ApplicationId = ap.ApplicationId,
                    ProgramId = ap.ProgramId,
                    Priority = ap.Priority,
                    ProgramName = ap.ProgramName,
                    FacultyName = ap.FacultyName,
                    FacultyId = ap.FacultyId,
                    ProgramCode = ap.ProgramCode
                }).ToList();
            var applicantApplication = new GetApplicationsDTO
            {
                ApplicationId = application.Id,
                ProgramsPriority = programsPriority,
            };
            return applicantApplication;
        }

        public async Task<GetApplicantInformationDTO> GetApplicantInformation(GetApplicantDTO getApplicantDTO)
        {
            if (getApplicantDTO.ApplicantId == null)
            {
                throw new NotFoundException("This applicant not found!");
            }
            var applicant = await _entranceDBContext.Applicants.FirstOrDefaultAsync(a => a.Id == getApplicantDTO.ApplicantId);
            if (applicant == null)
            {
                throw new NotFoundException("This applicant not found!");
            }
            var applicantInfo = new GetApplicantInformationDTO
            {
                FirstName = applicant.FirstName,
                LastName = applicant.LastName,
                Patronymic = applicant.Patronymic,
                Id = applicant.Id,
                Nationality = applicant.Nationality,
            };
        return applicantInfo;
        }
    }
}
