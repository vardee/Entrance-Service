﻿using adv_Backend_Entrance.Common.DTO.EntranceService.Applicant;
using adv_Backend_Entrance.Common.DTO.FacultyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.EntranceService.Manager
{
    public class GetAllQuerybleApplicationsDTO
    {
        public IQueryable<GetApplicationsDTO> Applications { get; set; }
        public PaginationInformation PaginationInformation { get; set; }
    }
}
