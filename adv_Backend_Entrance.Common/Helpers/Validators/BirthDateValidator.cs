using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Helpers.Validators
{
    public class BirthDateValidator
    {
        public static bool ValidateDateOfBirth(DateTime dateOfBirth)
        {
            DateOnly now = DateOnly.FromDateTime(DateTime.UtcNow);
            int age = now.Year - dateOfBirth.Year;

            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
            {
                age--;
            }

            if (age < 18 || age > 60)
            {
                return false;
            }

            return true;
        }
    }
}
