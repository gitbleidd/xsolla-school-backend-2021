using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using XsollaSchoolBackend.Database;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Data
{
    public class SqliteItemRepository : IItemRepository
    {
        private readonly DatabaseConfig _databaseConfig;

        public SqliteItemRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public Item CreateNewItem(Item item)
        {
            // Добавляем объект в бд и получаем его id, необходимый для SKU;
            // Генерируем SKU и возвращаем объект

            using var connection = new SqliteConnection(_databaseConfig.Name);
            var id = connection.ExecuteScalar<int>("INSERT INTO catalog(sku, name, type, count, price) " +
                "VALUES(@Sku, @Name, @Type, @Count, @Price); SELECT last_insert_rowid();", item);
            item.Sku = Utils.SkuUtil.GenerateSku(item, id);
            item.Id = id;
            connection.Execute("UPDATE catalog SET sku = @sku WHERE id = @id", new { sku = item.Sku, id = id });

            return item;
        }

        public bool DeleteItem(int id)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            var affectedRows = connection.Execute("DELETE FROM catalog WHERE id = @id", new { id = id});
            return affectedRows == 1;
        }

        public bool DeleteItemBySku(string sku)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            var affectedRows = connection.Execute("DELETE FROM catalog WHERE sku = @sku", new { sku = sku });
            return affectedRows == 1;
        }

        public ResponseItem<Item> GetAllItems(string type, string sortBy, int page, int pageSize)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            switch (sortBy)
            {
                case "price":
                    sortBy = "price ASC";
                    break;
                case "-price":
                    sortBy = "price DESC";
                    break;
                case "count":
                    sortBy = "count ASC";
                    break;
                case "-count":
                    sortBy = "count DESC";
                    break;
                default:
                    return new ResponseItem<Item>
                    {
                        items = new List<Item>(),
                        headers = new Dictionary<string, string>()
                    };
            }

            // Ограничение на размер страницы
            if (pageSize < 1 || pageSize > 100)
                pageSize = 5;

            string getItemsQuery;
            string itemsCountQuery;
            if (type != null)
            {
                getItemsQuery = $"SELECT * FROM catalog WHERE type = '{type}' ORDER BY {sortBy} LIMIT @startId, @count;";
                itemsCountQuery = $"SELECT COUNT(*) FROM catalog WHERE type = '{type}';";
            }
            else
            {
                getItemsQuery = $"SELECT * FROM catalog ORDER BY {sortBy} LIMIT @startId, @count;";
                itemsCountQuery = "SELECT MAX(_ROWID_) FROM 'catalog' LIMIT 1;";
            }

            var items = connection.Query<Item>(getItemsQuery, new { startId = (page - 1) * pageSize, count = pageSize }).ToList();
            int paginationCount = connection.ExecuteScalar<int>(itemsCountQuery); // Общее кол-во предметов по данному запросу

            var headers = new Dictionary<string, string>();
            headers.Add("X-Total-Count", paginationCount.ToString());
            return new ResponseItem<Item> { items = items, headers = headers };
        }

        public Item GetItemById(int id)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            var res = connection.Query<Item>("SELECT * FROM catalog WHERE id = @id", new { id = id }).ToList();

            if (res.Count == 0)
                return null;
            else
                return res.First();
        }

        public Item GetItemBySku(string sku)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            var res = connection.Query<Item>("SELECT * FROM catalog WHERE sku = @sku", new { sku = sku }).ToList();

            if (res.Count == 0)
                return null;
            else
                return res.First();
        }

        public bool UpdateItem(int id, Item updatedItem)
        {
            // Генерируем SKU на основе полученных данных и пытаемся обновить в бд по id
            using var connection = new SqliteConnection(_databaseConfig.Name);

            updatedItem.Sku = Utils.SkuUtil.GenerateSku(updatedItem, id);
            var affectedRows = connection.Execute("UPDATE catalog SET sku = @sku, name = @name, type = @type, count = @count, price = @price WHERE id = @id;", 
                new { sku = updatedItem.Sku, name = updatedItem.Name, type = updatedItem.Type, count = updatedItem.Count, price = updatedItem.Price, id = id });

            return affectedRows == 1;
        }

        public bool UpdateItemBySku(string sku, Item updatedItem)
        {
            // Находим id, необходим для SKU;
            // Генерируем SKU на основе полученных данных и пытаемся обновить в бд по SKU
            using var connection = new SqliteConnection(_databaseConfig.Name);

            int id = connection.ExecuteScalar<int>("SELECT id FROM catalog WHERE sku = @searchSku", new { searchSku = sku });
            updatedItem.Sku = Utils.SkuUtil.GenerateSku(updatedItem, id);
            var affectedRows = connection.Execute("UPDATE catalog SET sku = @sku, name = @name, type = @type, count = @count, price = @price WHERE sku = @searchSku;",
                new { sku = updatedItem.Sku, name = updatedItem.Name, type = updatedItem.Type, count = updatedItem.Count, price = updatedItem.Price, searchSku = sku });

            return affectedRows == 1;
        }
    }
}
