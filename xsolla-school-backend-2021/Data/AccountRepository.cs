using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XsollaSchoolBackend.Database;
using XsollaSchoolBackend.Models;
using XsollaSchoolBackend.Models.Tables;

namespace XsollaSchoolBackend.Data
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DatabaseConfig _databaseConfig;

        public AccountRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public User CreateNewUser(User user)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            var id = connection.ExecuteScalar<int>("INSERT INTO users(email, google_id) " +
                "VALUES(@email, @googleId); SELECT last_insert_rowid();", new { email = user.Email, googleId = user.GoogleId });
            return user;
        }

        public RoleUser GetUserById(int userId)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            var res = connection.Query<RoleUser>("SELECT u.id, u.email, u.google_id, r.role_name FROM users u INNER JOIN roles r ON u.role_id = r.id WHERE id = @userId;",
                new { userId = userId }).ToList();

            if (res.Count == 0)
                return null;
            else
                return res.First();
        }

        public RoleUser GetUserByEmail(string email)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            var res = connection.Query<RoleUser>("SELECT u.id, u.email, u.google_id, r.role_name FROM users u INNER JOIN roles r ON u.role_id = r.id WHERE email = @email;", 
                new { email = email }).ToList();

            if (res.Count == 0)
                return null;
            else
                return res.First();
        }

        public void UpdateUser(int userId, User user)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            var affectedRows = connection.Execute("UPDATE users SET email = @email, google_id = @googleId WHERE id = @id;",
                new { email = user.Email, googleId = user.GoogleId, id = userId });
        }

        public void DeleteUser(int userId)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            var affectedRows = connection.Execute("DELETE FROM users WHERE id = @id", new { id = userId });
        }
    }
}
