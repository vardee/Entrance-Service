using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Enums
{
    public enum EducationLevel
    {
        [Description("Бакалавриат")]
        Bachelor,
        [Description("Магистратура")]
        Magistracy,
        [Description("Аспирантура")]
        Postgraduate,
        [Description("Специалитет")]
        Specialty,
        [Description("Основное общее образование")]
        BasicGeneralEducation,
        [Description("Среднее общее образование")]
        SecondaryGeneralEducation,
        [Description("Среднее профессиональное образование")]
        SecondaryVocationalEducation
    }
}
