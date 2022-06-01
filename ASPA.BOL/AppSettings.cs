using System;
using System.Collections.Generic;
using System.Text;

namespace ASPA.BOL
{
    public class AppSettings
    {
        public string JWT_Secret { get; set; }

        public string Client_URL { get; set; }

        public int Token_expiration_minutes { get; set; }

        public string AdminGroup { get; set; }

        public string Application_email_adresse { get; set; }

        public string Mail_Smtp_Host { get; set; }

        public string Application_email_password { get; set; }

        public string Application_email_port { get; set; }
    }
}
