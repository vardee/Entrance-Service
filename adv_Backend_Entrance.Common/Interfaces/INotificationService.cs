using adv_Backend_Entrance.Common.DTO.NotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Interfaces
{
    public interface INotificationService
    {
        Task SendEmailNotification(SendNotificationDTO sendNotificationDTO);
    }
}
