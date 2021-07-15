using System;
using System.ComponentModel.DataAnnotations;

namespace XsollaSchoolBackend.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Sku { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public double Price { get; set; }

        public Item() { }
    }
}
