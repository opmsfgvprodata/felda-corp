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

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
    public class MaybankFileGenController : Controller
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
        //private GetGenerateFile GetGenerateFile = new GetGenerateFile();
        // GET: MaybankFileGen
        public ActionResult Index()
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
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

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

            dbC.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult Index(int Month, int Year, string CompCode, int Wilayah, string filter)
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
                List<sp_MaybankRcms_Result> maybankrcmsList = new List<sp_MaybankRcms_Result>();

                maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).ToList();

                var WilayahDetail = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == Wilayah).FirstOrDefault();

                filePath = GetGenerateFile.GenFileMaybank(maybankrcmsList, WilayahDetail, stringmonth, stringyear, NegaraID, SyarikatID, Wilayah, CompCode, filter, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

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

        public JsonResult CheckGenDataDetail(int Month, int Year, string CompCode, int Wilayah)
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

            List<sp_MaybankRcms_Result> maybankrcmsList = new List<sp_MaybankRcms_Result>();
            maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).ToList();
            var WilayahDetail = dbC.tbl_Wilayah.Where(x => x.fld_ID == Wilayah).FirstOrDefault();
            string filename = "M2E BURUH (" + InitialName + ") " + WilayahDetail.fld_WlyhName.ToUpper() + " " + stringmonth + stringyear + ".txt";

            if (maybankrcmsList.Count() != 0)
            {
                TotalGaji = maybankrcmsList.Sum(s => s.fld_GajiBersih);
                CountData = maybankrcmsList.Count();
                msg = GlobalResCorp.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResCorp.msgDataNotFound;
                statusmsg = "warning";
            }

            //if (GetGaji.Count() != 0)
            //{
            //    TotalGaji = GetGaji.Sum(s => s.fld_GajiBersih);
            //    CountData = GetGaji.Count();
            //    msg = GlobalResEstate.msgDataFound;
            //    statusmsg = "success"; 
            //}
            //else
            //{
            //    msg = GlobalResEstate.msgDataNotFound;
            //    statusmsg = "warning";
            //}


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

        public ActionResult RcmsGen()
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

        public ViewResult _rcms(int? WilayahList, string CompCodeList, int? MonthList, int? YearList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string WilayahName = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcms_Result> maybankrcmsList = new List<sp_MaybankRcms_Result>();

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
            //var LdgDetail = db.tbl_Ladang
            //    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
            //    .FirstOrDefault();
            //LdgName = db.tbl_Ladang
            //    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
            //    .Select(s => s.fld_LdgName)
            //    .FirstOrDefault();
            //LdgCode = db.tbl_Ladang
            //    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
            //    .Select(s => s.fld_LdgCode)
            //    .FirstOrDefault();
            //ViewBag.Ladang = LdgName.Trim();
            //ViewBag.LadangCode = LdgCode.Trim();
            //ViewBag.OriginatorId = LdgDetail.fld_OriginatorID;
            //ViewBag.OriginatorName = LdgDetail.fld_OriginatorName;
            //ViewBag.AccNo = LdgDetail.fld_NoAcc;
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
                return View(maybankrcmsList);
            }
            else
            {
                maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID, SyarikatID, WilayahList, YearList, MonthList, getuserid, CompCodeList).ToList();

                var BankList = dbC.tbl_Bank
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .ToList();

                ViewBag.RecordNo = maybankrcmsList.Count();

                if (maybankrcmsList.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(maybankrcmsList);
            }
        }

    }
}