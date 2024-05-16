using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService
{
    public class EditApplicantProfileInformationDTO
    { 
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Nationality {  get; set; }
    }
}
