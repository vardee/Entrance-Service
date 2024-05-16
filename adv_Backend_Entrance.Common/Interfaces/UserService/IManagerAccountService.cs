using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Interfaces.UserService
{
    public interface IManagerAccountService
    {
        Task EditApplicantProfile(EditApplicantProfileInformationDTO  editApplicantProfileInformationDTO);
    }
}
