using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XsollaSchoolBackend.Models
{
    public class ItemInput
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public double Price { get; set; }

    }
}
