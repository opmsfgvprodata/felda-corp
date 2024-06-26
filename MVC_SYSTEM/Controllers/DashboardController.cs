//Add file by aini 14/3/2023
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsSP;
using MVC_SYSTEM.ModelsCorporate;
using Antlr.Runtime.Misc;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.Http;

namespace MVC_SYSTEM.Controllers
{
    public class DashboardController : Controller
    {
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();
        GetNSWL GetNSWL = new GetNSWL();
        private GetIdentity getidentity = new GetIdentity();
        private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        EncryptDecrypt crypto = new EncryptDecrypt();
        private MVC_SYSTEM_ModelsCorporate db2 = new MVC_SYSTEM_ModelsCorporate();
        GetWilayah getwilyah = new GetWilayah(); //aini add 19/4/23
        ChangeTimeZone timezone = new ChangeTimeZone(); //aini add 19/4/23
        GetConfig GetConfig = new GetConfig(); //aini add 19/4/23

        // GET: Dashboard
        [Localization("bm")]

        public ActionResult Index()
        {

            ViewBag.Dashboard = "class = active";

            return View();
        }
        //aini update by costcentre 18072023
        public ActionResult DashWilayah(int? Type)
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            List<sp_DashWilayah_Result> dashwilayahresult = new List<sp_DashWilayah_Result>();

            dashwilayahresult = dbSP.sp_DashWilayah(SyarikatID, Type).Where(x => x.fld_Jumlah != 0).ToList();

            return Json(dashwilayahresult);
        }

        //aini modified 19/4/2023
        public ActionResult WilayahResult(string wilayah, int? costcentre)
        {
            ViewBag.WilayahName = wilayah;
            if (costcentre == 1)
            {
                ViewBag.title = "Felda & FPM";
            }
            else if (costcentre == 2)
            {
                ViewBag.title = "Felda";
            }
            else
            {
                ViewBag.title = "FPM";
            }

            ViewBag.CostCentre = costcentre;

            int currentMonth = DateTime.Now.Month - 1;
            int currentYear = DateTime.Now.Year;

            ViewBag.month = currentMonth;
            ViewBag.year = currentYear;

            return View("WilayahResult");
        }

        public ActionResult DashKerakyatan(string wilayah, int? costcentre)
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            var WilayahID = db2.tbl_Wilayah.Where(x => x.fld_WlyhName == wilayah).FirstOrDefault();

            List<sp_DashKerakyatan_Result> dashkerakyatanresult = new List<sp_DashKerakyatan_Result>();

            dashkerakyatanresult = dbSP.sp_DashKerakyatan(SyarikatID, WilayahID.fld_ID, costcentre).Where(x => x.fld_Jumlah != 0).ToList();

            return Json(dashkerakyatanresult);
        }

        public ActionResult DashJantina(string wilayah, int? costcentre)
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            var WilayahID = db2.tbl_Wilayah.Where(x => x.fld_WlyhName == wilayah).FirstOrDefault();

            List<sp_DashJantina_Result> dashjantinaresult = new List<sp_DashJantina_Result>();

            dashjantinaresult = dbSP.sp_DashJantina(SyarikatID, WilayahID.fld_ID, costcentre).Where(x => x.fld_Jumlah != 0).ToList();

            return Json(dashjantinaresult);
        }

        public ActionResult DashLadang(string wilayah, int? costcentre)
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            var WilayahID = db2.tbl_Wilayah.Where(x => x.fld_WlyhName == wilayah).FirstOrDefault();

            List<sp_DashLadang_Result> dashladangresult = new List<sp_DashLadang_Result>();

            dashladangresult = dbSP.sp_DashLadang(SyarikatID, WilayahID.fld_ID, costcentre).Where(x => x.fld_Jumlah != 0).ToList();

            return Json(dashladangresult);
        }

        public ActionResult DatatableKerakyatan(string wilayah, string kerakyatan, int? costcentre)
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            var WilayahID = db2.tbl_Wilayah.Where(x => x.fld_WlyhName == wilayah).FirstOrDefault();

            List<sp_DatatableKerakyatan_Result> datatablekerakyatanresult = new List<sp_DatatableKerakyatan_Result>();

            datatablekerakyatanresult = dbSP.sp_DatatableKerakyatan(SyarikatID, WilayahID.fld_ID, kerakyatan, costcentre).ToList();

            return Json(datatablekerakyatanresult);
        }

        //aini add function 24/3/2023
        public ActionResult DatatableJantina(string wilayah, string jantina, int? costcentre)
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            var WilayahID = db2.tbl_Wilayah.Where(x => x.fld_WlyhName == wilayah).FirstOrDefault();

            List<sp_DatatableJantina_Result> datatablejantinaresult = new List<sp_DatatableJantina_Result>();

            datatablejantinaresult = dbSP.sp_DatatableJantina(SyarikatID, WilayahID.fld_ID, jantina, costcentre).ToList();

            return Json(datatablejantinaresult);
        }

        //aini add function 24/3/2023
        public ActionResult DatatableLadang(string wilayah, string ladang, int? costcentre)
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            var WilayahID = db2.tbl_Wilayah.Where(x => x.fld_WlyhName == wilayah).FirstOrDefault();

            List<sp_DatatableLadang_Result> datatableladangresult = new List<sp_DatatableLadang_Result>();

            datatableladangresult = dbSP.sp_DatatableLadang(SyarikatID, WilayahID.fld_ID, ladang, costcentre).ToList();

            return Json(datatableladangresult);
        }

        //aini add 19/4/23
        public ActionResult DashPermitExpiry(int? Type)
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            List<sp_DashPermitExpired_Result> dashpermitresult = new List<sp_DashPermitExpired_Result>();

            dashpermitresult = dbSP.sp_DashPermitExpired(SyarikatID, Type).ToList();

            return Json(dashpermitresult);
        }
        //aini add 19/4/23
        public ActionResult DashStatusAkaun()
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            //original code
            //int month = DateTime.Now.Month - 1;
            //int year = DateTime.Now.Year;
            int type = 1;
            int wilayah = 0;

            //fatin modified - 02/01/2024
            DateTime currentDate = DateTime.Now;
            DateTime previousMonthDate = currentDate.AddMonths(-1);
            int month = previousMonthDate.Month;
            int year = (currentDate.Month == 1) ? currentDate.Year - 1 : currentDate.Year;

            List<sp_DashStatusAkaun_Result> dashStatusAkaun = new List<sp_DashStatusAkaun_Result>();

            dashStatusAkaun = dbSP.sp_DashStatusAkaun(SyarikatID, year, month, type, wilayah, null).ToList();

            return Json(dashStatusAkaun);
        }
        //aini add 19/4/23
        public ActionResult StatusAkaun()
        {
            ViewBag.WilayahName = "";

            return View("StatusAkaun");
        }
        //aini add 19/4/23
        public ActionResult DashStatusAkaunWilayah(string wilayah, string costcentre)
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            var WilayahID = db2.tbl_Wilayah.Where(x => x.fld_WlyhName == wilayah).FirstOrDefault();
            //original code
            //int month = DateTime.Now.Month - 1;
            //int year = DateTime.Now.Year;

            //fatin modified - 02/01/2023
            DateTime currentDate = DateTime.Now;
            DateTime previousMonthDate = currentDate.AddMonths(-1);
            int month = previousMonthDate.Month;
            int year = (currentDate.Month == 1) ? currentDate.Year - 1 : currentDate.Year;
            int type = 3;

            List<sp_DashStatusAkaun_Result> dashStatusAkaun = new List<sp_DashStatusAkaun_Result>();
            var costcentercode = db2.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldOptConfDesc == costcentre).FirstOrDefault();

            dashStatusAkaun = dbSP.sp_DashStatusAkaun(SyarikatID, year, month, type, WilayahID.fld_ID, costcentercode.fldOptConfValue).ToList();

            return Json(dashStatusAkaun);
        }
        //aini add 19/4/23
        public ActionResult DatatablePermit(string wilayah, string permit, int? costcentre)
        {
            int? SyarikatID = 0;
            int? type = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            var WilayahID = db2.tbl_Wilayah.Where(x => x.fld_WlyhName == wilayah).FirstOrDefault();

            if (permit == "3 Bulan")
                type = 1;
            else if (permit == "2 Bulan")
                type = 2;
            else if (permit == "1 Bulan")
                type = 3;
            else if (permit == "Semasa")
                type = 4;
            else
                type = 5;

            List<sp_DatatablePermitExpired_Result> datatablepermit = new List<sp_DatatablePermitExpired_Result>();

            datatablepermit = dbSP.sp_DatatablePermitExpired(SyarikatID, WilayahID.fld_ID, type, costcentre).ToList();

            return Json(datatablepermit);
        }
        //aini add 07072023
        public ActionResult DashTrans()
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            int year = DateTime.Now.Year;

            List<sp_DashTransactionListing_Result> dashTrans = new List<sp_DashTransactionListing_Result>();

            dashTrans = dbSP.sp_DashTransactionListing(SyarikatID, year).OrderBy(o => o.fld_Month2).ToList();

            return Json(dashTrans);
        }

        public ActionResult DashCostCentreKerakyatan(int? costcentre)
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            List<sp_DashAllKerakyatan_Result> kerakyatanresult = new List<sp_DashAllKerakyatan_Result>();

            kerakyatanresult = dbSP.sp_DashAllKerakyatan(SyarikatID, costcentre).Where(x => x.fld_Jumlah != 0).ToList();

            return Json(kerakyatanresult);
        }
    }
}