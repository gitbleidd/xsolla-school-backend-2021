using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XsollaSchoolBackend.Models
{
    public class ResponseItem<T>
    {
        public List<T> items;

        public Dictionary<string, string> headers;
    }
}
