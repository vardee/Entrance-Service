using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.EntranceService.DAL.Data.Models
{
    public class ManagerModel
    {
        public Guid Id { get; set;}
        public Guid UserId { get; set;}
        public string FullName { get; set;}
        public string Email { get; set;}
        public RoleType Role { get; set;}
    }
}
