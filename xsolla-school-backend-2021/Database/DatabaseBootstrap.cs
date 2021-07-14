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
            "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
            "sku TEXT DEFAULT ''," +
            "name TEXT DEFAULT ''," +
            "type TEXT DEFAULT ''," +
            "count INTEGER DEFAULT 0," +
            "price REAL DEFAULT 0); ");

            // Если таблица пустая, то заполнить ее
            if (connection.Query("SELECT * FROM catalog LIMIT 15;").Count() == 0)
            {
                var items = new List<Item> {
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

                foreach (var item in items)
                {
                    _repository.CreateNewItem(item);
                }
            }
        }
    }
}
