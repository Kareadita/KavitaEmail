using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Skeleton.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
