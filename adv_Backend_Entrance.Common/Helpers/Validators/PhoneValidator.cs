using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Helpers.Validators
{
    public static class PhoneValidator
    {
        private static readonly string PhoneNumberRegex = @"^[78]\d{10}$";

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return true;
            }

            return Regex.IsMatch(phoneNumber, PhoneNumberRegex);
        }
    }
}
