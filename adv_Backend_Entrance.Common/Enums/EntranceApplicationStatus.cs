using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Enums
{
    public enum EntranceApplicationStatus
    {
        Created,
        Rejected,
        Closed,
        Approved,
        UnderConsideration
    }
}
