using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XsollaSchoolBackend.Models.Tables;
using System.Security.Claims;

namespace XsollaSchoolBackend.Utils
{
    public static class ClaimUtil
    {
        public static User ParseClaims(IEnumerable<Claim> claims)
        {
            User user = new User();
            foreach (var claim in claims)
            {
                if (claim.Type == ClaimTypes.Email)
                    user.Email = claim.Value;
                if (claim.Type == ClaimTypes.NameIdentifier)
                    user.GoogleId = claim.Value;
            }

            return user;
        }
    }
}
