﻿using adv_Backend_Entrance.Common.DTO.EntranceService.Applicant;
using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.EntranceService.Manager
{
    public class GetApplicationsDTO
    {
        public Guid ApplicationId { get; set; }
        public Guid ManagerId { get; set; }
        public string ApplicantFullName { get; set; }
        public EntranceApplicationStatus ApplicationStatus { get; set; }
        public List<GetApplicationPriorityDTO> ProgramsPriority { get; set; }
    }
}
