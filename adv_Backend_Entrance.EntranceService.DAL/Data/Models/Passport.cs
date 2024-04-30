using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.EntranceService.DAL.Data.Models
{
    public class Passport
    {
        public int PassportNumber { get; set; }
        public Guid UserId { get; set; }
        public string BirthPlace { get; set; }
        public DateOnly IssuedWhen { get; set; }
        public string IssuedWhom { get; set; }

        public static explicit operator int(Passport v)
        {
            throw new NotImplementedException();
        }
    }
}
