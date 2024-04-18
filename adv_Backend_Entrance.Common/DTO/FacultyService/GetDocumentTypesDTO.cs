using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.FacultyService
{
    public class GetDocumentTypesDTO
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public string name { get; set; }
        public GetEducationLevelsDTO educationLevel { get; set; }
        public List<GetEducationLevelsDTO> nextEducationLevels { get; set; }
    }
}
