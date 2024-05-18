﻿using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.UserService
{
    public class EditUserProfileDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender Gender { get; set; }
        public string? Email { get; set; }
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
