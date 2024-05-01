using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.ApplicantService
{
    public class GetFileDTO
    {
        public IFormFile File { get; set; }
    }
}
