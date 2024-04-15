using System;
using System.ComponentModel;

namespace adv_Backend_Entrance.Common.Enums
{
    public static class EducationChecker
    {
        public static bool TryParseEducationLanguage(string language, out EducationLanguage result)
        {
            var enumValues = Enum.GetValues(typeof(EducationLanguage));
            foreach (EducationLanguage enumValue in enumValues)
            {
                var field = enumValue.GetType().GetField(enumValue.ToString());
                var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                if (descriptionAttribute != null && descriptionAttribute.Description.Equals(language, StringComparison.OrdinalIgnoreCase))
                {
                    result = enumValue;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public static bool TryParseEducationForm(string form, out EducationForm result)
        {
            var enumValues = Enum.GetValues(typeof(EducationForm));
            foreach (EducationForm enumValue in enumValues)
            {
                var field = enumValue.GetType().GetField(enumValue.ToString());
                var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                if (descriptionAttribute != null && descriptionAttribute.Description.Equals(form, StringComparison.OrdinalIgnoreCase))
                {
                    result = enumValue;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public static bool TryParseEducationLevel(string level, out EducationLevel result)
        {
            var enumValues = Enum.GetValues(typeof(EducationLevel));
            foreach (EducationLevel enumValue in enumValues)
            {
                var field = enumValue.GetType().GetField(enumValue.ToString());
                var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                if (descriptionAttribute != null && descriptionAttribute.Description.Equals(level, StringComparison.OrdinalIgnoreCase))
                {
                    result = enumValue;
                    return true;
                }
            }
            result = default;
            return false;
        }
    }
}
