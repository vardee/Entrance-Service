using adv_Backend_Entrance.Common.DTO.ApplicantService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Interfaces.ApplicantService
{
    public interface IApplicantService
    {
        Task AddPassport(AddPassportDTO addPassportDTO, Guid userId);
        Task AddEducationLevel(AddEducationLevelDTO addEducationLevelDTO, Guid userId);
        Task<GetEducationInformationDTO> GetEducationInformation(Guid userId);
        Task<GetPassportInformationDTO> GetPassportInformation(Guid userId);
        Task<GetPassportInformationDTO> EditPaspportInformation(AddPassportDTO editPassportDTO, Guid userId);
        Task<GetEducationInformationDTO> EditEducationInformation(AddEducationLevelDTO editEducationLevelDTO, Guid userId);
        Task DeletePassportInformation(Guid userId);
        Task DeleteEducationInformation(Guid userId);
    }
}
