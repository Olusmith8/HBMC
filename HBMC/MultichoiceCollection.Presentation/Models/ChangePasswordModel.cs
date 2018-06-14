using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultichoiceCollection.Presentation.Models
{
    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; } 

        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}