using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.ApplicantService
{
    public class SyncEducationLevelDTO
    {
        public Guid UserId {  get; set; }
        public Guid EducationDocumentId { get; set; }
        public EducationLevel EducationLevel { get; set; }  
    };
}
