using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ASPA.DAL.Security
{
    public class User : IdentityUser<int> 
    {
        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        [PersonalData]
        public bool IsAdmin { get; set; }

        public virtual ICollection<IdentityUserClaim<int>> Claims { get; set; }
    }
}
