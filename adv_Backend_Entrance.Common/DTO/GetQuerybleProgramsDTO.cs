﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO
{
    public class GetQuerybleProgramsDTO
    {
        public IQueryable<GetProgramsDTO> Programs { get; set; }
        public PaginationInformation PaginationInformation { get; set; }
    }
}
