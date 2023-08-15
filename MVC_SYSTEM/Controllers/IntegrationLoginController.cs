using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.LoginModels;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace MVC_SYSTEM.Controllers
{
    public class IntegrationLoginController : Controller
    {
        private MVC_SYSTEM_Login db2 = new MVC_SYSTEM_Login();
        private MVC_SYSTEM_Auth db3 = new MVC_SYSTEM_Auth();
        private errorlog geterror = new errorlog();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetConfig getConfig = new GetConfig();
        // GET: IntegrationLogin
        public ActionResult Index(string TokenID, string PassID, string Code)
        {
            Session["TokenID"] = TokenID;
            Session["PassID"] = PassID;
            Session["Code"] = Code;
            return RedirectToAction("VerifiedUser", "IntegrationLogin");
        }

        public ActionResult VerifiedUser()
        {
            EncryptDecrypt Encrypt = new EncryptDecrypt();
            string Encryptusername = Session["TokenID"].ToString();
            string Encryptpassword = Session["PassID"].ToString();
            string Encryptcode = Session["Code"].ToString();
            string decryptusername = Encrypt.Decrypt(Encryptusername);
            string decryptpassword = Encrypt.Decrypt(Encryptpassword);

            int day = timezone.gettimezone().Day;
            int month = timezone.gettimezone().Month;
            int year = timezone.gettimezone().Year;
            string code = day.ToString() + month.ToString() + year.ToString();
            code = Encrypt.Encrypt(code);
            Encryptcode = Encrypt.Decrypt(Encryptcode);
            Encryptcode = Encrypt.Encrypt(Encryptcode);
            if (Encryptcode == code)
            {
                var user = db2.tblUsers.Where(u => u.fldUserName == decryptusername.ToUpper() && u.fldUserPassword == decryptpassword).SingleOrDefault();
                if (user != null)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string data = js.Serialize(user);
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.fldUserShortName, timezone.gettimezone(), timezone.gettimezone().Add(FormsAuthentication.Timeout), false, data);
                    string encToken = FormsAuthentication.Encrypt(ticket);
                    HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, encToken);
                    Response.Cookies.Add(authoCookies);

                    getConfig.AddUserAuditTrail(user.fldUserID, "Login to HQ from estate URL");

                    return RedirectToAction("Index", "Main");
                }
                else
                {
                    ModelState.AddModelError("", GlobalResLogin.InvalidLogin);
                    return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                ModelState.AddModelError("", GlobalResLogin.InvalidLogin);
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult RedirectLogin(string TokenID, string PassID, string Code)
        {
            Session["TokenID"] = TokenID;
            Session["PassID"] = PassID;
            Session["Code"] = Code;
            return RedirectToAction("VerifiedAuthUser", "IntegrationLogin");
        }

        public ActionResult VerifiedAuthUser()
        {
            EncryptDecrypt Encrypt = new EncryptDecrypt();
            string Encryptusername = Session["TokenID"].ToString();
            string Encryptpassword = Session["PassID"].ToString();
            string Encryptcode = Session["Code"].ToString();
            string decryptusername = Encrypt.Decrypt(Encryptusername);
            string decryptpassword = Encrypt.Decrypt(Encryptpassword);

            int day = timezone.gettimezone().Day;
            int month = timezone.gettimezone().Month;
            int year = timezone.gettimezone().Year;
            string code = day.ToString() + month.ToString() + year.ToString();
            code = Encrypt.Encrypt(code);
            Encryptcode = Encrypt.Decrypt(Encryptcode);
            Encryptcode = Encrypt.Encrypt(Encryptcode);
            if (Encryptcode == code)
            {
                var user = db2.tblUsers.Where(u => u.fldUserName == decryptusername.ToUpper() && u.fldUserPassword == decryptpassword).SingleOrDefault();
                if (user != null)
                {
                    if (user.fldLadangID == 0)
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        string data = js.Serialize(user);
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.fldUserShortName, timezone.gettimezone(), timezone.gettimezone().Add(FormsAuthentication.Timeout), false, data);
                        string encToken = FormsAuthentication.Encrypt(ticket);
                        HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, encToken);
                        Response.Cookies.Add(authoCookies);
                        getConfig.AddUserAuditTrail(user.fldUserID, "Login to HQ");
                        return RedirectToAction("Index", "Main");
                    }
                    else
                    {
                        var routeurl = db3.tbl_Wilayah.Where(x => x.fld_SyarikatID == user.fldSyarikatID && x.fld_ID == user.fldWilayahID).Select(s => s.fld_UrlRoute).FirstOrDefault();
                        string passwordencrypt = Encrypt.Encrypt(user.fldUserPassword);
                        string usernameencrypt = Encrypt.Encrypt(user.fldUserName);
                        int day1 = timezone.gettimezone().Day;
                        int month1 = timezone.gettimezone().Month;
                        int year1 = timezone.gettimezone().Year;
                        string code1 = day1.ToString() + month1.ToString() + year1.ToString();
                        code1 = Encrypt.Encrypt(code1);
                        routeurl = routeurl + "IntegrationLogin?TokenID=" + usernameencrypt + "&PassID=" + passwordencrypt + "&Code=" + code1;

                        getConfig.AddUserAuditTrail(user.fldUserID, "Login to estate");

                        return Redirect(routeurl);
                    }
                }
                else
                {
                    ModelState.AddModelError("", GlobalResLogin.InvalidLogin);
                    return RedirectToAction("Index", "Login", new { status = 1});
                }
            }
            else
            {
                ModelState.AddModelError("", GlobalResLogin.InvalidLogin);
                return RedirectToAction("Index", "Login", new { status = 2 });
            }
        }
    }
}