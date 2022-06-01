using System;
using System.Collections.Generic;
using System.Text;

namespace ASPA.BOL.OutputModel.ApplicationUser
{
    public class LoginOutputModel
    {
        public bool Succeded { get; set; }

        public int Id { get; set; }

        public string Token { get; set; }

        public IEnumerable<OutputMessage> Info { get; set; }
    }
}
