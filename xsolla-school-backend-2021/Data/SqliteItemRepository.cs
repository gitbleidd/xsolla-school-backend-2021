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

            var res = connection.Execute("DELETE FROM catalog WHERE id = @id", new { id = id});
            return res == 1;
        }

        public bool DeleteItemBySku(string sku)
        {
            throw new NotImplementedException();
        }

        public List<Item> GetAllItems(string type, string sortBy, int page, int pageSize)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            switch (sortBy)
            {
                case "price":
                    sortBy = "ASC";
                    break;
                case "-price":
                    sortBy = "DESC";
                    break;
                default:
                    return new List<Item>();
            }

            // Ограничение на размер страницы
            if (pageSize < 1 || pageSize > 100)
                pageSize = 5;

            string query;
            if (type != null)
                query = $"SELECT * FROM catalog WHERE type = '{type}' ORDER BY price {sortBy} LIMIT @startId, @count;";
            else
                query = $"SELECT * FROM catalog ORDER BY price {sortBy} LIMIT @startId, @count;";

            var res = connection.Query<Item>(query, new { startId = (page - 1) * pageSize, count = pageSize });

            return res.ToList();
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
            using var connection = new SqliteConnection(_databaseConfig.Name);
            var res = connection.Query<Item>("UPDATE catalog SET sku = @Sku, name = @Name, type = @Type, count = @Count, price = @Price WHERE id = @id;", 
                new { updatedItem, id }).ToList();

            return res.Count == 1;
        }

        public bool UpdateItemBySku(string sku, Item updatedItem)
        {
            throw new NotImplementedException();
        }
    }
}
