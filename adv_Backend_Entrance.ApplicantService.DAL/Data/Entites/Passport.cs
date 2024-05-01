using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.ApplicantService.DAL.Data.Entites
{
    public class Passport
    {
        public int PassportNumber { get; set; }
        public Guid UserId { get; set; }
        public string BirthPlace { get; set; }
        public DateOnly IssuedWhen { get; set; }
        public string IssuedWhom { get; set; }
        public Guid FileId { get; set; }
    }
}
