using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XsollaSchoolBackend.Database;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Data
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DatabaseConfig _databaseConfig;

        public CommentRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public Comment CreateNewComment(int itemId, Comment comment)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            var id = connection.ExecuteScalar<int>("INSERT INTO comments(item_id, text) " +
                "VALUES(@itemId, @text); SELECT last_insert_rowid();", new {itemId = itemId, text = comment.Text });
            return comment;
        }

        public bool DeleteComment(int commentId)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            var affectedRows = connection.Execute("DELETE FROM comments WHERE id = @commentId", new { commentId = commentId });
            return affectedRows == 1;
        }

        public List<Comment> GetAllComments(int itemId)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            var res = connection.Query<Comment>("SELECT * FROM comments WHERE item_id = @itemId", new { itemId = itemId }).ToList();
            return res;
        }

        public Comment GetCommentById(int commentId)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            var res = connection.Query<Comment>("SELECT * FROM comments WHERE id = @commentId", new { commentId = commentId }).ToList();

            if (res.Count == 0)
                return null;
            else
                return res.First();
        }

        public bool UpdateComment(int commentId, Comment newComment)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);

            var affectedRows = connection.Execute("UPDATE comments SET text = @text WHERE id = @commentId;",
                new { text = newComment.Text, commentId = commentId });

            return affectedRows == 1;
        }
    }
}
