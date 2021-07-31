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
        private readonly IItemRepository _itemsRepo;
        private readonly ICommentRepository _commentsRepo;

        public DatabaseBootstrap(DatabaseConfig databaseConfig, IItemRepository itemsRepo, ICommentRepository commentsRepo)
        {
            _databaseConfig = databaseConfig;
            _itemsRepo = itemsRepo;
            _commentsRepo = commentsRepo;
        }

        public void Setup()
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            connection.Execute("CREATE TABLE IF NOT EXISTS catalog(" +
            "id INTEGER PRIMARY KEY," +
            "sku TEXT UNIQUE," +
            "name TEXT DEFAULT ''," +
            "type TEXT DEFAULT ''," +
            "count INTEGER DEFAULT 0," +
            "price REAL DEFAULT 0);");

            connection.Execute("CREATE TABLE IF NOT EXISTS comments(" +
                "id INTEGER PRIMARY KEY, " +
                "itemId INTEGER REFERENCES catalog(id), " +
                "text TEXT NOT NULL);");

            // Если таблица "catalog" пустая, то заполнить ее
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
                    _itemsRepo.CreateNewItem(item);
                }
            }

            // Если таблица "comments" пустая, то заполнить ее
            if (connection.Query("SELECT * FROM comments LIMIT 5;").Count() == 0)
            {
                var comments = new List<Comment> { 
                    new Comment { ItemId = 1, Text = "Отличная футболка!" },
                    new Comment { ItemId = 1, Text = "Хороший мерч, 9/10 ;)" },
                    new Comment { ItemId = 1, Text = "Заказал себе и всей семье, спасибо Rest API Shop!!!!" },
                    new Comment { ItemId = 4, Text = "Неплохое качество, но размер маловат, придется возвращать :(" },
                    new Comment { ItemId = 4, Text = "Неожидал, что возможно такое хорошее качество для бейсболки, однозначно рекомендую!" },
                    new Comment { ItemId = 6, Text = "Эх, отличный постер, аж все книги перечитать захотелось 😍" },
                    new Comment { ItemId = 8, Text = "Пришло с небольшим сколом, но фигурка отличная. Praise the Sun!" },
                    new Comment { ItemId = 8, Text = "Топ за свои деньги. Качество, детализация - все на высшем уровне" },
                    new Comment { ItemId = 14, Text = "К великой игре - великое коллекционное издание. Пришло в целости и сохранности 👍" }
                };

                foreach (var comment in comments)
                {
                    _commentsRepo.CreateNewComment(comment.ItemId, comment);
                }
            }
        }
    }
}
