using adv_Backend_Entrance.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace adv_Backend_Entrance.Common.Interfaces
{
    public interface IAuthDbContext
    {
        DbSet<BlackToken> BlackListTokens { get; set; }
    }
}
