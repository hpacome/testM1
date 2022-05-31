using System;
using System.Collections.Generic;
using System.Text;

namespace ASPA.BOL.OutputModel.ApplicationUser
{
    public class RegistrationOutputModel
    {
        public bool Succeded { get; set; }

        public string RegistrationEmail { get; set; }

        public IEnumerable<OutputMessage> Info { get; set; }
    }
}
