using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO
{
    public class GetProgramsDTO
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string language { get; set; }
        public string educationForm { get; set; }
        public GetFacultiesDTO faculty { get; set; }
        public GetEducationLevelsDTO educationLevel { get; set; }
    }
}
