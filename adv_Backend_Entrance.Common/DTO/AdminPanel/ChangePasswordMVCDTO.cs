using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.AdminPanel
{
    public class ChangePasswordMVCDTO
    {
        public Guid UserId { get; set; }
        public string OldPasssword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
