//Aini create model 14032023
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCustom
{
    public class AuthUserLogin
    {
        [Key]
        public int userID { get; set; }

        public string userName { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        public string LastAccess { get; set; }

    }
}