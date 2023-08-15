using MVC_SYSTEM.LoginModels;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    public class MobileLoginController : Controller
    {
        private MVC_SYSTEM_Login db2 = new MVC_SYSTEM_Login();
        // GET: MobileLogin
        public string Index()
        {
            return "Hello World";
        }

        [HttpPost]
        public string Index(string username, string password2)
        {
            string password;
            string result = "";
            try
            {
                if (string.IsNullOrEmpty(username) == false && string.IsNullOrEmpty(password2) == false)
                {
                    //getUser user = null;
                    EncryptDecrypt Encrypt = new EncryptDecrypt();
                    password = Encrypt.Encrypt(password2);
                    var user = db2.tblUsers.Where(u => u.fldUserName == username.ToUpper() && u.fldUserPassword == password && u.fldDeleted == false).SingleOrDefault();
                    if (user != null)
                    {
                        result = "success";
                    }
                    else
                    {
                        result = "failure";
                    }
                }
                else
                {
                    result = "failure";
                }
            }
            catch (Exception)
            {
                result = "failure";
            }
            return result;
        }
    }
}