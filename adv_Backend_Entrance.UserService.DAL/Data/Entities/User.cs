using adv_Backend_Entrance.Common.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.UserService.DAL.Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
