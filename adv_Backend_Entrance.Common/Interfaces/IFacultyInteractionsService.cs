using adv_Backend_Entrance.Common.DTO.FacultyService;
using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Interfaces
{
    public interface IFacultyInteractionsService
    {

        Task<List<GetDocumentTypesDTO>> GetDocumentTypes();
        Task<List<GetFacultiesDTO>> GetFaculties();
        Task <List<GetEducationLevelsDTO>> GetEducationLevels();
        Task<GetQuerybleProgramsDTO> GetQueryblePrograms(int size, int page, List<EducationLanguage> LanguageEnum, List<EducationLevel> EducationLevelEnum, List<EducationForm> EducationFormEnum, Guid? FacultyId,Guid? Id);
    }
}
