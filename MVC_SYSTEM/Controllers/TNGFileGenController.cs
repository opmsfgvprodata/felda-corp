using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.ModelsSP;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using MVC_SYSTEM.DapperModels;
using Itenso.TimePeriod;
using System.Reflection.Emit;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
    public class TNGFileGenController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        private GetIdentity getidentity = new GetIdentity();
        private GetTriager GetTriager = new GetTriager();
        private GetNSWL GetNSWL = new GetNSWL();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private errorlog geterror = new errorlog();
        private GetConfig GetConfig = new GetConfig();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetWilayah GetWilayah = new GetWilayah();
        private Connection Connection = new Connection();
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();

        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ToString();
            con = new SqlConnection(constr);
        }
        // GET: TNGFileGen
        public ActionResult Index()
        {
            ViewBag.MaybankFileGen = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.MenuSubList = sublist;
            ViewBag.MenuList = new SelectList(dbC.tblMenuLists.Where(x => x.fld_Flag == "m2e" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string MenuList, string MenuSubList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (MenuSubList != null)
            {
                return RedirectToAction(MenuSubList, "TNGFileGen");
            }
            else
            {
                int menulist = int.Parse(MenuList);
                var action = dbC.tblMenuLists.Where(x => x.fld_ID == menulist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "TNGFileGen");
            }
        }

        public JsonResult GetSubList(int ListID)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var findsub = dbC.tblMenuLists.Where(x => x.fld_ID == ListID).Select(s => s.fld_Sub).FirstOrDefault();
            List<SelectListItem> sublist = new List<SelectListItem>();
            if (findsub != null)
            {
                sublist = new SelectList(dbC.tblMenuLists.Where(x => x.fld_Flag == findsub && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
            }
            db.Dispose();
            return Json(sublist);
        }

        [HttpPost]
        public ActionResult DownloadText(int Month, int Year, string CompCode, int Wilayah, string filter, string[] WorkerId, DateTime PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                List<TNGPaymentReport> tNGList = new List<TNGPaymentReport>();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID.Value);
                    parameters.Add("SyarikatID", SyarikatID.Value);
                    parameters.Add("WilayahID", Wilayah);
                    parameters.Add("Year", Year);
                    parameters.Add("Month", Month);
                    parameters.Add("UserID", getuserid);
                    parameters.Add("CompCode", CompCode);
                    connection();
                    con.Open();
                    tNGList = SqlMapper.Query<TNGPaymentReport>(con, "sp_MaybankTNG", parameters).ToList();
                    con.Close();
                }
                catch (Exception)
                {
                    throw;
                }

                if (WorkerId.Contains("0"))
                {
                    //Do Nothing
                }
                else
                {
                    tNGList = tNGList.Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var WilayahDetail = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == Wilayah).FirstOrDefault();

                filePath = GetGenerateFile.GenFileTNG(tNGList, WilayahDetail, stringmonth, stringyear, NegaraID, SyarikatID, Wilayah, CompCode, filter, PaymentDate, out filename);

                link = Url.Action("Download", "TNGFileGen", new { filePath, filename });

                //dbr.Dispose();

                msg = GlobalResCorp.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

        [HttpPost]
        public ActionResult DownloadTextIndividu(int Month, int Year, string CompCode, int Wilayah, string filter, string[] WorkerId, DateTime PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                List<TNGPaymentReport> tNGList = new List<TNGPaymentReport>();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID.Value);
                    parameters.Add("SyarikatID", SyarikatID.Value);
                    parameters.Add("WilayahID", Wilayah);
                    parameters.Add("Year", Year);
                    parameters.Add("Month", Month);
                    parameters.Add("UserID", getuserid);
                    parameters.Add("CompCode", CompCode);
                    connection();
                    con.Open();
                    tNGList = SqlMapper.Query<TNGPaymentReport>(con, "sp_MaybankTNG", parameters).ToList();
                    con.Close();
                }
                catch (Exception)
                {
                    throw;
                }

                if (WorkerId.Contains("0"))
                {
                   
                }
                else
                {
                    tNGList = tNGList.Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var WilayahDetail = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == Wilayah).FirstOrDefault();

                filePath = GetGenerateFile.GenFileTNG(tNGList, WilayahDetail, stringmonth, stringyear, NegaraID, SyarikatID, Wilayah, CompCode, filter, PaymentDate, out filename);

                link = Url.Action("Download", "TNGFileGen", new { filePath, filename });

                //dbr.Dispose();

                msg = GlobalResCorp.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }


        public JsonResult CheckGenDataDetail(int Month, int Year, string CompCode, int Wilayah, string filter, string[] WorkerId, DateTime PaymentDate)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            string stringyear = "";
            string stringmonth = "";
            string CorpID = "";
            string ClientID = "";
            string AccNo = "";
            string InitialName = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalGaji = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            GetNSWL.GetSyarikatRCMSDetail(CompCode, out CorpID, out ClientID, out AccNo, out InitialName);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<TNGPaymentReport> tNGList = new List<TNGPaymentReport>();
            if (WorkerId == null)
                WorkerId = new string[] { "0" };

            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID.Value);
                parameters.Add("SyarikatID", SyarikatID.Value);
                parameters.Add("WilayahID", Wilayah);
                parameters.Add("Year", Year);
                parameters.Add("Month", Month);
                parameters.Add("UserID", getuserid);
                parameters.Add("CompCode", CompCode);
                connection();
                con.Open();
                tNGList = SqlMapper.Query<TNGPaymentReport>(con, "sp_MaybankTNG", parameters).ToList();
                con.Close();
            }
            catch (Exception)
            {
                throw;
            }

            if (WorkerId.Contains("0"))
            {
                //Do Nothing
            }
            else
            {
                tNGList = tNGList.Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
            }
            var WilayahDetail = dbC.tbl_Wilayah.Where(x => x.fld_ID == Wilayah).FirstOrDefault();
            string filename = "M2E BURUH (" + InitialName + ") " + WilayahDetail.fld_WlyhName.ToUpper() + " " + stringmonth + stringyear + ".txt";

            if (tNGList.Count() != 0)
            {
                TotalGaji = tNGList.Sum(s => s.fld_GajiBersih);
                CountData = tNGList.Count();
                msg = GlobalResCorp.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResCorp.msgDataNotFound;
                statusmsg = "warning";
            }
            dbSP.Dispose();
            dbC.Dispose();
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData });
        }

        public FileResult Download(string filePath, string filename)
        {
            string path = HttpContext.Server.MapPath(filePath);

            DownloadFiles.FileDownloads objs = new DownloadFiles.FileDownloads();

            var filesCol = objs.GetFiles(path);
            var CurrentFileName = filesCol.Where(x => x.FileName == filename).FirstOrDefault();

            string contentType = string.Empty;
            contentType = "application/txt";
            return File(CurrentFileName.FilePath, contentType, CurrentFileName.FileName);

        }

        public ActionResult TNGGen()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> CompCodeList = new List<SelectListItem>();
            CompCodeList = new SelectList(
                dbC.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            CompCodeList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;

            List<SelectListItem> WilayahList = new List<SelectListItem>();
            WilayahList = new SelectList(dbC.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            WilayahList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.WilayahList = WilayahList;

            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ViewResult _TNG(int? WilayahList, string CompCodeList, int? MonthList, int? YearList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string WilayahName = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<TNGPaymentReport> tNGList = new List<TNGPaymentReport>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.CorpID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_CorporateID)
                .FirstOrDefault();
            ViewBag.ClientID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_ClientBatchID)
                .FirstOrDefault();
            ViewBag.AccNo = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_AccountNo)
                .FirstOrDefault();
            ViewBag.WilayahName = db.tbl_Wilayah
                .Where(x => x.fld_ID == WilayahList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_WlyhName)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            //ViewBag.WilayahName = WilayahName;
            ViewBag.Description = "Wilayah " + WilayahName + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = "Sila pilih Bulan, Tahun, Syarikat dan Wilayah";
                return View(tNGList);
            }
            else
            {
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID.Value);
                    parameters.Add("SyarikatID", SyarikatID.Value);
                    parameters.Add("WilayahID", WilayahList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Month", MonthList);
                    parameters.Add("UserID", getuserid);
                    parameters.Add("CompCode", CompCodeList);
                    connection();
                    con.Open();
                    tNGList = SqlMapper.Query<TNGPaymentReport>(con, "sp_MaybankTNG", parameters).ToList();
                    con.Close();
                }
                catch (Exception)
                {
                    throw;
                }

                var BankList = dbC.tbl_Bank
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .ToList();

                ViewBag.RecordNo = tNGList.Count();

                if (tNGList.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(tNGList);
            }
        }


        public ActionResult TNGGenIndividu()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> CompCodeList = new List<SelectListItem>();
            CompCodeList = new SelectList(
                dbC.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            CompCodeList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;

            List<SelectListItem> WilayahList = new List<SelectListItem>();
            WilayahList = new SelectList(dbC.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            WilayahList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.WilayahList = WilayahList;

            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ViewResult _tNGIndividu(int? WilayahList, string CompCodeList, int? MonthList, int? YearList, string print, string filter, string[] WorkerId)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string WilayahName = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<TNGPaymentReport> tNGList = new List<TNGPaymentReport>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.CorpID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_CorporateID)
                .FirstOrDefault();
            ViewBag.ClientID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_ClientBatchID)
                .FirstOrDefault();
            ViewBag.AccNo = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_AccountNo)
                .FirstOrDefault();
            ViewBag.WilayahName = db.tbl_Wilayah
                .Where(x => x.fld_ID == WilayahList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_WlyhName)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            //ViewBag.WilayahName = WilayahName;
            ViewBag.Description = "Wilayah " + WilayahName + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = "Sila pilih Bulan, Tahun, Syarikat dan Wilayah";
                return View(tNGList);
            }
            else
            {
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID.Value);
                    parameters.Add("SyarikatID", SyarikatID.Value);
                    parameters.Add("WilayahID", WilayahList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Month", MonthList);
                    parameters.Add("UserID", getuserid);
                    parameters.Add("CompCode", CompCodeList);
                    connection();
                    con.Open();
                    tNGList = SqlMapper.Query<TNGPaymentReport>(con, "sp_MaybankTNG", parameters).ToList();
                    con.Close();
                }
                catch (Exception)
                {
                    throw;
                }
                if (WorkerId.Contains("0"))
                {
                    //Do nothing
                }
                else
                {
                    tNGList = tNGList.Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var BankList = dbC.tbl_Bank
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .ToList();

                ViewBag.RecordNo = tNGList.Count();

                if (tNGList.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(tNGList);
            }
        }

        public JsonResult GetWorker(int? WilayahID, string CompCode, int Year, int Month)
        {
            List<SelectListItem> workerList = new List<SelectListItem>();
            var userDetail = getidentity.GetUserDetail(User.Identity.Name);
            var getuserid = userDetail.fldUserID;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("NegaraID", userDetail.fldNegaraID);
            parameters.Add("SyarikatID", userDetail.fldSyarikatID);
            parameters.Add("WilayahID", WilayahID);
            parameters.Add("Year", Year);
            parameters.Add("Month", Month);
            parameters.Add("UserID", getuserid);
            parameters.Add("CompCode", CompCode);
            connection();
            con.Open();
            var tNGList = SqlMapper.Query<TNGPaymentReport>(con, "sp_MaybankTNG", parameters).ToList();
            con.Close();
            var tNGSelectedList = tNGList.Select(s => new { s.fld_Nopkj, s.fld_Nama }).OrderBy(o => o.fld_Nama).ToList();

            if (tNGList.Count() > 0)
            {
                workerList = new SelectList(tNGSelectedList.Select(s => new SelectListItem { Value = s.fld_Nopkj.ToString(), Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();
            }

            return Json(workerList);
        }
    }
}