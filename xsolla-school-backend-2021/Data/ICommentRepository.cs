using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Data
{
    public interface ICommentRepository
    {
        public Comment GetCommentById(int commentId);
        public List<Comment> GetAllComments(int itemId);
        public Comment CreateNewComment(int itemId, Comment comment);
        public bool UpdateComment(int commentId, Comment newComment);
        public bool DeleteComment(int commentId);
    }
}
