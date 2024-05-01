using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.ApplicantService.DAL.Data.Entites
{
    public class PassportImportFile
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public Guid UserId { get; set; }
    }
}
