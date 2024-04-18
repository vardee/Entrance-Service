using adv_Backend_Entrance_Notification.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace adv_Backend_Entrance_Notification.DAL.Data
{
    public class NotificationDBContext:DbContext
    {
        public NotificationDBContext(DbContextOptions<NotificationDBContext> options) : base(options) { }
        public DbSet<Message> Messages { get; set; }
    }
}
