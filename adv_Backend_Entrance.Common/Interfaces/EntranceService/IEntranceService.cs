using adv_Backend_Entrance.Common.DTO;
using adv_Backend_Entrance.Common.DTO.EntranceService.Applicant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Interfaces.EntranceService
{
    public interface IEntranceService
    {
        Task CreateApplication(CreateApplicationDTO createApplicationDTO, string token);
        Task DeleteProgramFromApplication(DeleteProgramFromApplicationDTO deleteProgramFromApplicationDTO);
        Task AddProgramsInApplication(AddProgramsDTO addProgramsDTO, string token);
        Task ChangeProgramPriority(ChangeProgramPriorityDTO changeProgramPriorityDTO);
        Task<GetMyApplicationDTO> GetApplication(string token);
    }
}
