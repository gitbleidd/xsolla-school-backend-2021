using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XsollaSchoolBackend.Models.Tables;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Data
{
    public interface IAccountRepository
    {
        public RoleUser GetUserById(int userId);
        public RoleUser GetUserByEmail(string email);
        public User CreateNewUser(User user);
        public void UpdateUser(int userId, User user);
        public void DeleteUser(int userId);
    }
}
