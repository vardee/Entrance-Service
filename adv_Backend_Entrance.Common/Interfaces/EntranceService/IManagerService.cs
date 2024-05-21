using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Interfaces.EntranceService
{
    public interface IManagerService
    {
        Task TakeApplication(TakeApplicationDTO takeApplicationDTO, Guid userId);
        Task RejectApplication(RejectApplicationDTO rejectApplicationDTO, Guid userId);
        Task ChangeApplicationStatus(ChangeApplicationStatusDTO changeApplicationStatusDTO, Guid userId);
        Task<GetAllQuerybleApplicationsDTO> GetQuerybleApplications(int size, int page, string? name, Guid? ProgramId, List<Guid>? Faculties, List<EntranceApplicationStatus>? entranceApplicationStatuses, bool? haveManager,bool? isMy, Guid? managerId, Sorting? timeSorting);
        Task<GetApplicationsDTO> GetApplicantion(GetApplicantDTO getApplicantDTO);
        Task<GetApplicantInformationDTO> GetApplicantInformation(GetApplicantDTO getApplicantDTO);
        Task<GetAllQuerybleManagersDTO> GetManagers(int size, int page, string? name, RoleType? roleType);
    }
}
