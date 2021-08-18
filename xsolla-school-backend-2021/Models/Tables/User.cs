using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace XsollaSchoolBackend.Models.Tables
{
    public class User
    {
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string GoogleId { get; set; }

        public int RoleId { get; set; }
    }
}
