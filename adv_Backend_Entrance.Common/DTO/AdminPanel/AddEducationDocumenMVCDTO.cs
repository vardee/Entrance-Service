﻿using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.AdminPanel
{
    public class AddEducationDocumentMVCDTO
    {
        public Guid Id { get; set; }
        public EducationLevel EducationLevel { get; set; }
    }
}
