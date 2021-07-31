using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XsollaSchoolBackend.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string Text { get; set; }
    }
}
