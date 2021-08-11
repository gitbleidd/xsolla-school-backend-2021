using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XsollaSchoolBackend.Models
{
    public class Order
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNum { get; set; }

        [Required]
        public Dictionary<int, int> Items { get; set; } // Содержит ID товара и его кол-во
    }
}
