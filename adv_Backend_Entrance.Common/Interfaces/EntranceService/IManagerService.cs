using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Interfaces.EntranceService
{
    public interface IManagerService
    {
        Task TakeApplication(TakeApplicationDTO takeApplicationDTO, Guid userId);
        Task RejectApplication(RejectApplicationDTO rejectApplicationDTO, Guid userId);    
    }
}
