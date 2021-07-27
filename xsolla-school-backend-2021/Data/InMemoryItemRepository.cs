using System;
using System.Collections.Generic;
using System.Linq;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Data
{
    public class InMemoryItemRepository : IItemRepository
    {
        private List<Item> s_items;

        public InMemoryItemRepository()
        {
            var items = new List<Item> {
                new Item { Name = "Pokemon T-shirt", Type = "Clothes", Price = 1000},
                new Item { Name = "Doom T-shirt", Type = "Clothes", Price = 1000},
                new Item { Name = "Sonic Hoodie", Type = "Clothes", Price = 2500},
                new Item { Name = "Pokemon Cap", Type = "Clothes", Price = 1200},
                new Item { Name = "Doom Cap", Type = "Clothes", Price = 1666},
                new Item { Name = "Witcher 3 - Poster", Type = "Poster", Price = 500},
                new Item { Name = "Zelda BoTW - Poster", Type = "Poster", Price = 450},
                new Item { Name = "Dark Souls Statue", Type = "Statue", Price = 3000},
                new Item { Name = "Zelda Statue", Type = "Statue", Price = 4560},
                new Item { Name = "Cyberpunk 2077 Statue", Type = "Statue", Price = 7702},
                new Item { Name = "Pokemon Backpack", Type = "Backpack", Price = 2200},
                new Item { Name = "Mario Backpack", Type = "Backpack", Price = 1999},
                new Item { Name = "Crash Bandicoot Toy", Type = "Toys", Price = 2999},
                new Item { Name = "Hollow Knight Collector's Edition", Type = "Games", Price = 10999},
                new Item { Name = "Animal Crossing", Type = "Games", Price = 5999}
            };

            foreach (var item in items)
            {
                CreateNewItem(item);
            }
        }

        // Получение информации о товаре по его id
        public Item GetItemById(int id)
        {
            int index = s_items.FindIndex(item => item.Id == id);
            if (index == -1)
                return null;
            return s_items[index];
        }

        // Получение информации о товаре по его sku
        public Item GetItemBySku(string sku)
        {
            var res = s_items.Find(item => item.Sku == sku);
            if (res == null)
                return null;
            return res;
        }

        // Получение каталога товаров по частям, с возможностью сортировки по стоимости и типам
        public ResponseItem<Item> GetAllItems(string type, string sortBy, int page, int pageSize)
        {
            IEnumerable<Item> data = s_items;
            if (type != null)
                data = data.Where(item => item.Type == type);

            // Ограничение на размер страницы
            if (pageSize < 1 || pageSize > 100)
                pageSize = 5;

            switch (sortBy)
            {
                case "price":
                    data = data.OrderBy(item => item.Price);
                    break;
                case "-price":
                    data = data.OrderByDescending(item => item.Price);
                    break;
                case "count":
                    data = data.OrderBy(item => item.Count);
                    break;
                case "-count":
                    data = data.OrderByDescending(item => item.Count);
                    break;
                default:
                    return new ResponseItem<Item>
                    {
                        items = new List<Item>(),
                        headers = new Dictionary<string, string>()
                    };
            }

            var items = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int paginationCount = data.Count();

            var headers = new Dictionary<string, string>();
            headers.Add("X-Total-Count", paginationCount.ToString());
            return new ResponseItem<Item> { items = items, headers = headers };
        }

        // Создает товар на основе входящего JSON и возвращает его id
        public Item CreateNewItem(Item newItem)
        {
            newItem.Id = Utils.IdCounter.GetNextId();
            newItem.Sku = Utils.SkuUtil.GenerateSku(newItem, newItem.Id);

            s_items.Add(newItem);
            return newItem;
        }

        // Редактирование всех данных о товаре по его id
        public bool UpdateItem(int id, Item updatedItem)
        {
            int index = s_items.FindIndex(item => item.Id == id);
            if (index == -1)
                return false;

            s_items[index].Name = updatedItem.Name;
            s_items[index].Type = updatedItem.Type;
            s_items[index].Price = updatedItem.Price;
            s_items[index].Count = updatedItem.Count;
            s_items[index].Sku = Utils.SkuUtil.GenerateSku(s_items[index], s_items[index].Id);

            return true;
        }

        // Редактирование всех данных о товаре по его sku
        public bool UpdateItemBySku(string sku, Item updatedItem)
        {
            var index = s_items.FindIndex(item => item.Sku == sku);
            if (index == -1)
                return false;

            s_items[index].Name = updatedItem.Name;
            s_items[index].Type = updatedItem.Type;
            s_items[index].Price = updatedItem.Price;
            s_items[index].Count = updatedItem.Count;
            s_items[index].Sku = Utils.SkuUtil.GenerateSku(s_items[index], s_items[index].Id);

            return true;
        }

        // Удаление товара по его id
        public bool DeleteItem(int id)
        {
            int index = s_items.FindIndex(item => item.Id == id);
            if (index == -1)
                return false;
            s_items.RemoveAt(index);
            return true;
        }

        // Удаление товара по его sku
        public bool DeleteItemBySku(string sku)
        {
            var index = s_items.FindIndex(item => item.Sku == sku);
            if (index == -1)
                return false;
            s_items.RemoveAt(index);
            return true;
        }
    }
}
