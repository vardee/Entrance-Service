﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.EntranceService
{
    public class DeleteProgramFromApplicationDTO
    {
        public Guid ApplicationId { get; set; }
        public Guid ProgramId { get; set; }
    }
}
