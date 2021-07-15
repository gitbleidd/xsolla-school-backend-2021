using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;
using XsollaSchoolBackend.Data;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Database
{
    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig _databaseConfig;
        private readonly IItemRepository _repository;

        public DatabaseBootstrap(DatabaseConfig databaseConfig, IItemRepository repository)
        {
            _databaseConfig = databaseConfig;
            _repository = repository;
        }

        public void Setup()
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            connection.Execute("CREATE TABLE IF NOT EXISTS catalog(" +
            "id INTEGER PRIMARY KEY," +
            "sku TEXT DEFAULT ''," +
            "name TEXT DEFAULT ''," +
            "type TEXT DEFAULT ''," +
            "count INTEGER DEFAULT 0," +
            "price REAL DEFAULT 0); ");

            // Если таблица пустая, то заполнить ее
            if (connection.Query("SELECT * FROM catalog LIMIT 15;").Count() == 0)
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
                    _repository.CreateNewItem(item);
                }
            }
        }
    }
}
