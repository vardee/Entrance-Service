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
        Task AddPassport(AddPassportDTO addPassportDTO, string token);
        Task AddEducationLevel(AddEducationLevelDTO addEducationLevelDTO, string token);
        Task<GetEducationInformationDTO> GetEducationInformation(string token);
        Task<GetPassportInformationDTO> GetPassportInformation(string token);
        Task<GetPassportInformationDTO> EditPaspportInformation(AddPassportDTO editPassportDTO, string token);
        Task<GetEducationInformationDTO> EditEducationInformation(AddEducationLevelDTO editEducationLevelDTO, string token);
        Task DeletePassportInformation(string token);
        Task DeleteEducationInformation(string token);
    }
}
