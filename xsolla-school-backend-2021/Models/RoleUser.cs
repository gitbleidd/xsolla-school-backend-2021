using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XsollaSchoolBackend.Models
{
    public class RoleUser
    {
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string GoogleId { get; set; }

        public string RoleName { get; set; }
    }
}
