using System;
using System.Collections.Generic;
using System.Text;

namespace ASPA.BOL.InputModel.ApplicationUser
{
    public class ConfirmEmailInputModel
    {
        public string UserId { get; set; }

        public string ConfirmationCode { get; set; }
    }
}
