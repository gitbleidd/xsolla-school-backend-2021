using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Data
{
    public class InMemoryItemRepository : IItemRepository
    {
        private List<Item> s_items;

        public InMemoryItemRepository()
        {
            s_items = new List<Item> {
                new Item("Pokemon T-shirt", "Clothes", 1000),
                new Item("Doom T-shirt", "Clothes", 1000),
                new Item("Sonic Hoodie", "Clothes", 2500),
                new Item("Pokemon Cap", "Clothes", 1200),
                new Item("Doom Cap", "Clothes", 1666),
                new Item("Witcher 3 - Poster", "Poster", 500),
                new Item("Zelda BoTW - Poster", "Poster", 450),
                new Item("Dark Souls Statue", "Statue", 3000),
                new Item("Zelda Statue", "Statue", 4560),
                new Item("Cyberpank 2077 Statue", "Statue", 7702),
                new Item("Pokemon Backpack", "Backpack", 2200),
                new Item("Mario Backpack", "Backpack", 1999),
                new Item("Crash Bandicoot Toy", "Toys", 2999),
                new Item("Hollow Knight Collector's Edition", "Games", 10999),
                new Item("Animal Crossing", "Games", 5999)};
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
        public List<Item> GetAllItems(string type, string sortBy, int page, int pageSize)
        {
            IEnumerable<Item> data = s_items;
            if (type != null)
                data = data.Where(item => item.Type == type);

            switch (sortBy)
            {
                case "price":
                    data = data.OrderBy(item => item.Price);
                    break;
                case "-price":
                    data = data.OrderByDescending(item => item.Price);
                    break;
                default:
                    break;
            }

            var res = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return res;
        }

        // Создает товар на основе входящего JSON и возвращает его id
        public Item CreateNewItem(Item newItem)
        {
            s_items.Add(new Item(newItem));
            return s_items.Last();
        }

        // Редактирование всех данных о товаре по его id
        public bool UpdateItem(int id, Item updatedItem)
        {
            int index = s_items.FindIndex(item => item.Id == id);
            if (index == -1)
                return false;
            s_items[index].UpdateItem(updatedItem);
            return true;
        }

        // Редактирование всех данных о товаре по его sku
        public bool UpdateItemBySku(string sku, Item updatedItem)
        {
            var index = s_items.FindIndex(item => item.Sku == sku);
            if (index == -1)
                return false;
            s_items[index].UpdateItem(updatedItem);
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
