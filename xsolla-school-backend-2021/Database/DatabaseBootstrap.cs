using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;
using XsollaSchoolBackend.Data;
using XsollaSchoolBackend.Models;
using XsollaSchoolBackend.Models.Tables;

namespace XsollaSchoolBackend.Database
{
    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig _databaseConfig;
        private readonly IItemRepository _itemsRepo;
        private readonly ICommentRepository _commentsRepo;
        private readonly IAccountRepository _accountRepo;

        public DatabaseBootstrap(DatabaseConfig databaseConfig, IItemRepository itemsRepo, ICommentRepository commentsRepo, IAccountRepository accountRepo)
        {
            _databaseConfig = databaseConfig;
            _itemsRepo = itemsRepo;
            _commentsRepo = commentsRepo;
            _accountRepo = accountRepo;
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
                "item_id INTEGER REFERENCES catalog(id), " +
                "text TEXT NOT NULL);");

            connection.Execute("CREATE TABLE IF NOT EXISTS users(" +
                "id INTEGER PRIMARY KEY, " +
                "email TEXT UNIQUE NOT NULL, " +
                "google_id TEXT UNIQUE NOT NULL," +
                "role_id INTEGER DEFAULT 2," +
                "FOREIGN KEY(role_id) REFERENCES roles(id) );");

            connection.Execute("CREATE TABLE IF NOT EXISTS roles(" +
                "id INTEGER PRIMARY KEY, " +
                "role_name TEXT UNIQUE NOT NULL);");

            // Заполняем исходными данными пустые таблицы:

            // Таблица catalog
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

            // Таблица comments
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

            // Таблица roles
            if (connection.Query("SELECT * FROM roles LIMIT 5;").Count() == 0)
            {
                connection.ExecuteScalar<int>("INSERT INTO roles(id, role_name) VALUES(1, 'vendor');");
                connection.ExecuteScalar("INSERT INTO roles(id, role_name) VALUES(2, 'consumer');");
            }

            // Таблица users
            if (connection.Query("SELECT * FROM users LIMIT 5;").Count() == 0)
            {
                // Hint - как соотносятся id и название ролей:
                // role_id = 1: vendor (полный доступ к взаимодействию с товарами).
                // role_id = 2: consumer (только просмотр товаров).
                connection.ExecuteScalar<int>("INSERT INTO users(email, google_id, role_id) VALUES('adonis7952@gmail.com', 'abcd', 1);");
                connection.ExecuteScalar<int>("INSERT INTO users(email, google_id, role_id) VALUES('gitbleidd@gmail.com', 'ef', 2);");
            }
        }
    }
}
