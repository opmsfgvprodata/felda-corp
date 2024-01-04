using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsSP;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{

    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User,Resource,Viewer")]
    public class MainController : Controller
    {
        // GET: Main
        private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        GetIdentity getidentity = new GetIdentity();
        EncryptDecrypt crypto = new EncryptDecrypt();
        GetNSWL GetNSWL = new GetNSWL();
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();
        private MVC_SYSTEM_ModelsCorporate db2 = new MVC_SYSTEM_ModelsCorporate(); //aini add 24/3/2023

        //aini modified 19/4/23
        public ActionResult Index()
        {
            ViewBag.Main = "class = active";
            ViewBag.Dropdown = "dropdown";
            //modified by aini 14/3/2023
            int? getuserid = getidentity.ID(User.Identity.Name);

            var getinfo = getidentity.AuthUserLogin(User.Identity.Name);
            var getinfo2 = getidentity.AuthUserLogin2(getuserid); //aini update 21062023
            ViewBag.AuthUserLogin = getinfo;
            ViewBag.AuthUserLogin2 = getinfo2;

            int? SyarikatID = 0;

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            List<sp_DashAllKerakyatan_Result> kerakyatanresult = new List<sp_DashAllKerakyatan_Result>();

            kerakyatanresult = dbSP.sp_DashAllKerakyatan(SyarikatID, 1).Where(x => x.fld_Jumlah != 0).ToList();

            //int currentMonth = DateTime.Now.Month - 1;
            //int currentYear = DateTime.Now.Year;

            DateTime currentDate = DateTime.Now;
            DateTime previousMonthDate = currentDate.AddMonths(-1);
            int currentMonth = previousMonthDate.Month;
            int currentYear = (currentDate.Month == 1) ? currentDate.Year - 1 : currentDate.Year;

            ViewBag.month = currentMonth;
            ViewBag.year = currentYear;

            var position = getidentity.Penjawatan(User.Identity.Name); //aini update 18072023
            var getdata = db.tblOptionConfigsWeb.Where(x => x.fldOptConfValue == position && x.fldOptConfFlag1 == "position" && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            if (getdata == null)
            {
                ViewBag.Position = "Admin";
            }
            else
            {
                ViewBag.Position = getdata.fldOptConfDesc;
            }

            return View(kerakyatanresult);
        }

        public JsonResult ChangePassword(string oldpswd, string newpswd, string confirmpswd)
        {
            if (!string.IsNullOrEmpty(oldpswd))
            {
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = getidentity.ID(User.Identity.Name);
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

                // mas delete x.fldWilayahID==WilayahID && x.fldLadangID==LadangID pd 15/9/2020
                var getdata = db.tblUsers.Where(x => x.fldUserID == getuserid && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).FirstOrDefault();
                string userpswd = crypto.Encrypt(oldpswd);
                if (getdata != null && getdata.fldUserPassword == userpswd)
                {
                    if (!string.IsNullOrEmpty(newpswd) && confirmpswd == newpswd && newpswd != oldpswd)
                    {
                        //var pswdpattern = "((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{6,20})";
                        var pswdpattern = new Regex(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20})");

                        // mas tambah crypto.Encrypt pd 15/9/2020
                        if (pswdpattern.IsMatch(crypto.Encrypt((newpswd))))
                        {
                            getdata.fldUserPassword = crypto.Encrypt(newpswd);
                            db.Entry(getdata).State = EntityState.Modified;
                            db.SaveChanges();
                            return Json(new { success = true, msg = "Password successfully changed.", status = "success" });
                        }
                        else
                        {
                            return Json(new { success = false, msg = "Password tidak sah.", status = "warning" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, msg = "Error.", status = "warning" });
                    }

                }
                else
                {
                    return Json(new { success = false, msg = "Please contact IT", status = "warning" });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Please enter your password", status = "warning" });
            }

        }

        public ActionResult pwd()
        {
            return View();
        }

        [HttpPost]
        public JsonResult pwdchnge(string pass, int processType)
        {
            string code = "";
            if (!string.IsNullOrEmpty(pass))
            {
                if (processType == 1)
                {
                    code = crypto.Encrypt(pass);
                }
                else
                {
                    code = crypto.Decrypt(pass);
                }
            }
            return Json(code);
        }

        //aini add function calendar 24/3/2023
        public ActionResult Calendar()
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            var cuti2 = db2.tbl_CutiUmumMaster.Where(x => x.fld_SyarikatID == 1).ToArray();
            return Json(cuti2);
        }

    }
}