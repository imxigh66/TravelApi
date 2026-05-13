using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum TripMemberRole
    {
        Owner = 0,   // полный доступ, передаётся автоматически при создании трипа
        Editor = 1,   // добавлять места, заметки, писать в чат
        Viewer = 2,   // только читать и писать в чат
    }
}
