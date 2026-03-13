using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum MoodType
    {
        // С кем (0–9)
        WithCompany = 0,
        Solo = 1,
        WithFamily = 2,
        RomanticDate = 3,

        // Вайб (10–19)
        Special = 10,
        Calm = 11,
        Surprise = 12,
        Active = 13,
        NightOut = 16
    }
}
