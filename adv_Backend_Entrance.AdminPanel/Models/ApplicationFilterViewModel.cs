﻿using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ApplicationFilterViewModel
    {
        public GetApplicationsFilter Filter { get; set; }
        public List<ApplicationModel> Applications { get; set; }
        public Guid CurrentManagerId { get; set; }
        public List<RoleType> Roles { get; set; }   
    }
}
