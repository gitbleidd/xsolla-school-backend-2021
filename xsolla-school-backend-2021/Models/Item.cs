using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        public Item(string name, string type, double price)
        {
            Id = Utils.IdCounter.GetNextId();
            Name = name;
            Type = type;
            Price = price;
            Sku = Utils.SkuUtil.GenerateSku(this, Id);
        }

        public Item(Item item)
        {
            Id = Utils.IdCounter.GetNextId();
            Name = item.Name;
            Type = item.Type;
            Price = item.Price;
            Count = item.Count;
            Sku = Utils.SkuUtil.GenerateSku(this, Id);
        }

        public void UpdateItem(Item updatedItem)
        {
            Name = updatedItem.Name;
            Type = updatedItem.Type;
            Price = updatedItem.Price;
            Count = updatedItem.Count;
            Sku = Utils.SkuUtil.GenerateSku(this, Id);
        }
    }
}
