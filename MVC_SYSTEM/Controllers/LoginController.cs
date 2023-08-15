using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.LoginModels;
using MVC_SYSTEM.Security;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.AuthFTPModels;

namespace MVC_SYSTEM.Controllers
{
    public class LoginController : Controller
    {
        //private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        private MVC_SYSTEM_Login db2 = new MVC_SYSTEM_Login();
        private MVC_SYSTEM_Auth db3 = new MVC_SYSTEM_Auth();
        private errorlog geterror = new errorlog();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetConfig getConfig = new GetConfig();

        // GET: Login
        [Localization("bm")]
        public ActionResult Index(int? status)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Main");
            }
            else
            {
                if (status == 1)
                {
                    ModelState.AddModelError("", GlobalResLogin.InvalidLogin);
                }
                else if (status == 2)
                {
                    ModelState.AddModelError("", GlobalResLogin.InvalidLogin);
                }
                return View();
            }
        }

        // POST: Login/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AntiForgeryHandleError]
        public ActionResult Index(LoginModels.tblUser Login, string returnUrl)
        {
            string password;
            try
            {
                if (string.IsNullOrEmpty(Login.fldUserName) == false && string.IsNullOrEmpty(Login.fldUserPassword) == false)
                {
                    //getUser user = null;
                    EncryptDecrypt Encrypt = new EncryptDecrypt();
                    password = Encrypt.Encrypt(Login.fldUserPassword);
                    var user = db2.tblUsers.Where(u => u.fldUserName == Login.fldUserName.ToUpper() && u.fldUserPassword == password && u.fldDeleted == false).SingleOrDefault();
                    
                    // mas tambah - 05/11/2020
                    // tambah condition user.fldSyarikatID == 1 || user.fldSyarikatID == 2
                    // untuk filter user felda/gmn & ftp
                    if (user != null && user.fldSyarikatID == 1 || user.fldSyarikatID == 2)
                    {
                        if(user.fldWilayahID != 0 && user.fldLadangID !=0)
                        {
                            var routeurl = db3.tbl_Wilayah.Where(x => x.fld_SyarikatID == user.fldSyarikatID && x.fld_ID == user.fldWilayahID).Select(s => s.fld_UrlRoute).FirstOrDefault();
                            string passwordencrypt = Encrypt.Encrypt(user.fldUserPassword);
                            string usernameencrypt = Encrypt.Encrypt(user.fldUserName);
                            int day = timezone.gettimezone().Day;
                            int month = timezone.gettimezone().Month;
                            int year = timezone.gettimezone().Year;
                            string code = day.ToString() + month.ToString() + year.ToString();
                            code = Encrypt.Encrypt(code);
                            routeurl = routeurl + "IntegrationLogin?TokenID=" + usernameencrypt + "&PassID=" + passwordencrypt + "&Code=" + code;

                            getConfig.AddUserAuditTrail(user.fldUserID, "Login to HQ");

                            return Redirect(routeurl);
                        }
                        else
                        {
                            JavaScriptSerializer js = new JavaScriptSerializer();
                            string data = js.Serialize(user);
                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.fldUserShortName, timezone.gettimezone(), timezone.gettimezone().Add(FormsAuthentication.Timeout), false, data);
                            string encToken = FormsAuthentication.Encrypt(ticket);
                            HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, encToken);
                            Response.Cookies.Add(authoCookies);

                            getConfig.AddUserAuditTrail(user.fldUserID, "Login to HQ");

                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                if (user.fldRoleID == 1 || user.fldRoleID == 2)
                                {
                                    return RedirectToAction("Index", "SuperAdminSelection");
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Main");
                                }
                            }
                        }
                    }
                    else
                    {
                        MVC_SYSTEM_Auth_FTP FTPDb = new MVC_SYSTEM_Auth_FTP();

                        var UserFTP = FTPDb.tblUsers.Where(u => u.fldUserName == Login.fldUserName.ToUpper() && u.fldUserPassword == password && u.fldDeleted == false).SingleOrDefault();
                        if (UserFTP != null)
                        {
                            var routeurl = FTPDb.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "opmslink" && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
                            string passwordencrypt = Encrypt.Encrypt(UserFTP.fldUserPassword);
                            string usernameencrypt = Encrypt.Encrypt(UserFTP.fldUserName);
                            int day = timezone.gettimezone().Day;
                            int month = timezone.gettimezone().Month;
                            int year = timezone.gettimezone().Year;
                            string code = day.ToString() + month.ToString() + year.ToString();
                            code = Encrypt.Encrypt(code);
                            routeurl = routeurl + "IntegrationLogin?TokenID=" + usernameencrypt + "&PassID=" + passwordencrypt + "&Code=" + code;
                            return Redirect(routeurl);
                        }
                        else
                        {
                            ModelState.AddModelError("", GlobalResLogin.InvalidLogin);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Please enter username and password.");
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                // mas comment - 05/11/2020
                //ModelState.AddModelError("", "Error. Please see error log file");

                // mas tukar error mesej - 05/11/2020
                ModelState.AddModelError("", "Sila pastikan Nama Pengguna bermula dengan 'FEL' atau 'GMN'");
                return View();
            }
            return View(Login);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login", null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db2.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
