using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Helpers.Validators
{
    public class PasswordValidator
    {
        public static bool ValidatePassword(string password)
        {

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return false;
            }

            string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$";

            if (!Regex.IsMatch(password, passwordRegex))
            {
                return false;
            }

            return true;
        }
    }
}
