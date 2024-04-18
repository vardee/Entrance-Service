using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.UserService
{
    public class changePasswordDTO
    {
        public string OldPasssword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
