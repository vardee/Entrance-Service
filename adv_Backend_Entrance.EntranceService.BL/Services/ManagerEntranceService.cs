using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.EntranceService;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.EntranceService.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.EntranceService.BL.Services
{
    public class ManagerEntranceService : IManagerService
    {
        private readonly EntranceDBContext _entranceDBContext;
        private readonly TokenHelper _tokenHelper;
        public ManagerEntranceService(EntranceDBContext entranceDBContext, TokenHelper tokenHelper)
        {
            _entranceDBContext = entranceDBContext;
            _tokenHelper = tokenHelper;
        }
        
        public async Task TakeApplication(TakeApplicationDTO takeApplicationDTO, Guid userId)
        {
            var application = await _entranceDBContext.Applications.FirstOrDefaultAsync(a => a.Id == takeApplicationDTO.ApplicationId);
            if(application == null)
            {
                throw new BadRequestException("This application not found!");
            }
            application.ManagerId = userId;
            application.ApplicationStatus = EntranceApplicationStatus.UnderConsideration;
            await _entranceDBContext.SaveChangesAsync();     
        }

        public async Task RejectApplication(RejectApplicationDTO rejectApplicationDTO, Guid userId)
        {
            var application = await _entranceDBContext.Applications.FirstOrDefaultAsync(a => a.Id == rejectApplicationDTO.ApplicationId);
            if (application == null)
            {
                throw new BadRequestException("This application not found!");
            }
            application.ManagerId =  Guid.Empty;
            application.ApplicationStatus = EntranceApplicationStatus.Created;
            await _entranceDBContext.SaveChangesAsync();
        }
    }
}
