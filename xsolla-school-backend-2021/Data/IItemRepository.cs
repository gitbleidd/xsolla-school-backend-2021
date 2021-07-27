using System;
using System.Collections.Generic;
using System.Linq;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Data
{
    public interface IItemRepository
    {
        public Item GetItemById(int id);
        public Item GetItemBySku(string sku);
        public ResponseItem<Item> GetAllItems(string type, string sortBy, int page, int pageSize);
        public Item CreateNewItem(Item newItem);
        public bool UpdateItem(int id, Item updatedItem);
        public bool UpdateItemBySku(string sku, Item updatedItem);
        public bool DeleteItem(int id);
        public bool DeleteItemBySku(string sku);
    }
}
