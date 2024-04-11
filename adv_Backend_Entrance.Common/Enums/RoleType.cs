using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Enums
{
    public enum RoleType
    {
        [Display(Name = ApplicationRoleNames.Admin)]
        Admin,
        [Display(Name = ApplicationRoleNames.Manager)]
        Manager,
        [Display(Name = ApplicationRoleNames.MainManager)]
        MainManager,
        [Display(Name = ApplicationRoleNames.Applicant)]
        Applicant,
    }
    public class ApplicationRoleNames
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string MainManager = "Cook";
        public const string Applicant = "Courier";
    }
}
