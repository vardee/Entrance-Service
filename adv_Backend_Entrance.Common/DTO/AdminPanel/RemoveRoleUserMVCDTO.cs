using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.AdminPanel
{
    public class RemoveRoleUserMVCDTO
    {
        public Guid UserId { get; set; }
        public RoleType Role { get; set; }
    }
}
