using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XsollaSchoolBackend.Utils
{
    public static class IdCounter
    {
        public static int CurrentId { get; private set; } = 0;

        public static int GetNextId()
        {
            return CurrentId++;
        }
    }
}
