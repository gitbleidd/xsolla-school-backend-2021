using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XsollaSchoolBackend.Utils
{
    public static class IdCounter
    {
        private static readonly object idLock = new object();
        public static int CurrentId { get; private set; } = 0;

        public static int GetNextId()
        {
            lock (idLock)
            {
                return CurrentId++;
            }
        }
    }
}
