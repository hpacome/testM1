using System;
using System.Collections.Generic;
using System.Text;

namespace ASPA.BOL.Dto
{
    public class ApplicationUserDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public IEnumerable<int> RoleId { get; set; }

        public IEnumerable<string> RoleString { get; set; }
    }
}
