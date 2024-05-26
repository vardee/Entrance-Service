using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Helpers.Validators
{
    public class FullNameValidator
    {
        public static bool ValidateFullName(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }
            try
            {
                string fullNameRegex = @"^.{2,30}$";
                return Regex.IsMatch(line, fullNameRegex);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
