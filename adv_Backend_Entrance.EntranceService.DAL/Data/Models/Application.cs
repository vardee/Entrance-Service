using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.EntranceService.DAL.Data.Models
{
    public class Application
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public string? Nationality { get; set; }
        public Guid UserId { get; set; }
        public int PassportId { get; set; }
        public Guid EducationId { get; set; }

        public EntranceApplicationStatus ApplicationStatus { get; set; }
        public Guid ManagerId { get; set; }
    }
}
