﻿using MVC_SYSTEM.Attributes;
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
using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using MVC_SYSTEM.ModelsDapper;
using Itenso.TimePeriod;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
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
        private GetWilayah getWilayah = new GetWilayah();
        private Connection Connection = new Connection();
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();
        //private GetGenerateFile GetGenerateFile = new GetGenerateFile();
        // GET: MaybankFileGen
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
                return RedirectToAction(MenuSubList, "MaybankFileGen");
            }
            else
            {
                int menulist = int.Parse(MenuList);
                var action = dbC.tblMenuLists.Where(x => x.fld_ID == menulist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "MaybankFileGen");
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
                List<sp_MaybankRcms_Result> maybankrcmsList = new List<sp_MaybankRcms_Result>();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).ToList();
                }
                else
                {
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var WilayahDetail = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == Wilayah).FirstOrDefault();

                filePath = GetGenerateFile.GenFileMaybank(maybankrcmsList, WilayahDetail, stringmonth, stringyear, NegaraID, SyarikatID, Wilayah, CompCode, filter, PaymentDate, out filename);

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
                List<sp_MaybankRcms_Result> maybankrcmsList = new List<sp_MaybankRcms_Result>();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).ToList();
                }
                else
                {
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var WilayahDetail = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == Wilayah).FirstOrDefault();

                filePath = GetGenerateFile.GenFileMaybank(maybankrcmsList, WilayahDetail, stringmonth, stringyear, NegaraID, SyarikatID, Wilayah, CompCode, filter, PaymentDate, out filename);

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

        [HttpPost]
        public ActionResult DownloadTextTNG(int Month, int Year, string CompCode, int Wilayah, string filter, string[] WorkerId, DateTime PaymentDate)
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
                List<sp_MaybankTNG_Result> maybankrcmsList = new List<sp_MaybankTNG_Result>();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbSP.sp_MaybankTNG(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).ToList();
                }
                else
                {
                    maybankrcmsList = dbSP.sp_MaybankTNG(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var WilayahDetail = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == Wilayah).FirstOrDefault();

                filePath = GetGenerateFile.GenFileTNG(maybankrcmsList, WilayahDetail, stringmonth, stringyear, NegaraID, SyarikatID, Wilayah, CompCode, filter, PaymentDate, out filename);

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

        //[HttpPost]
        //public ActionResult DownloadTextTNGIndividu(int Month, int Year, string CompCode, int Wilayah, string filter, string[] WorkerId, DateTime PaymentDate)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    string msg = "";
        //    string statusmsg = "";
        //    string filePath = "";
        //    string filename = "";

        //    string stringyear = "";
        //    string stringmonth = "";
        //    string link = "";
        //    stringyear = Year.ToString();
        //    stringmonth = Month.ToString();
        //    stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

        //    ViewBag.MaybankFileGen = "class = active";

        //    try
        //    {
        //        GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //        List<sp_MaybankTNG_Result> maybankrcmsList = new List<sp_MaybankTNG_Result>();

        //        if (WorkerId == null)
        //            WorkerId = new string[] { "0" };

        //        if (WorkerId.Contains("0"))
        //        {
        //            maybankrcmsList = dbSP.sp_MaybankTNG(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).ToList();
        //        }
        //        else
        //        {
        //            maybankrcmsList = dbSP.sp_MaybankTNG(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
        //        }

        //        var WilayahDetail = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == Wilayah).FirstOrDefault();

        //        filePath = GetGenerateFile.GenFileTNG(maybankrcmsList, WilayahDetail, stringmonth, stringyear, NegaraID, SyarikatID, Wilayah, CompCode, filter, PaymentDate, out filename);

        //        link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

        //        //dbr.Dispose();

        //        msg = GlobalResCorp.msgGenerateSuccess;
        //        statusmsg = "success";
        //    }
        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        msg = GlobalResCorp.msgGenerateFailed;
        //        statusmsg = "warning";
        //    }

        //    return Json(new { msg, statusmsg, link });
        //}

        public JsonResult CheckGenDataDetail(int Month, int Year, string CompCode, int Wilayah, string[] WorkerId)
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
            if (WorkerId == null)
                WorkerId = new string[] { "0" };

            if (WorkerId.Contains("0"))
            {
                maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).ToList();
            }
            else
            {
                maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
            }
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

            //List<SelectListItem> CompCodeList = new List<SelectListItem>();
            //CompCodeList = new SelectList(
            //    dbC.tblOptionConfigsWebs
            //        .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
            //        .OrderBy(o => o.fldOptConfDesc)
            //        .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
            //    "Value", "Text").ToList();
            //CompCodeList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            //ViewBag.CompCodeList = CompCodeList;

            //List<SelectListItem> WilayahList = new List<SelectListItem>();
            //WilayahList = new SelectList(dbC.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID)
            //    .OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            //WilayahList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            //ViewBag.WilayahList = WilayahList;


            List<SelectListItem> CompCodeList = new List<SelectListItem>();
            List<SelectListItem> WilayahList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                WilayahList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();

                CompCodeList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == 1 && x.fld_NegaraID == 1).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            }
            else
            {
                WilayahList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName").ToList();
                CompCodeList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == 1 && x.fld_NegaraID == 1).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            }
            CompCodeList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;

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
            ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NamaPndkSyarikat)
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
            if (MonthList == null || YearList == null || CompCodeList == "0" || WilayahList == 0)
            {
                ViewBag.Message = "Sila pilih Bulan, Tahun, Syarikat, Wilayah dan Tarikh Bayaran";
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


        public ActionResult RcmsGenIndividu()
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

        public ViewResult _rcmsIndividu(int? WilayahList, string CompCodeList, int? MonthList, int? YearList, string print, string filter, string[] WorkerId)
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
            ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NamaPndkSyarikat)
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
                return View(maybankrcmsList);
            }
            else
            {
                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID, SyarikatID, WilayahList, YearList, MonthList, getuserid, CompCodeList).ToList();
                }
                else
                {
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID, SyarikatID, WilayahList, YearList, MonthList, getuserid, CompCodeList).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

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

        public JsonResult GetWorker(int? WilayahID, string CompCode, int Year, int Month)
        {
            List<SelectListItem> workerList = new List<SelectListItem>();
            var userDetail = getidentity.GetUserDetail(User.Identity.Name);
            var getuserid = userDetail.fldUserID;
            var maybankrcmsList = dbSP.sp_MaybankRcms(userDetail.fldNegaraID, userDetail.fldSyarikatID, WilayahID, Year, Month, getuserid, CompCode).Select(s => new { s.fld_Nopkj, s.fld_Nama }).OrderBy(o => o.fld_Nama).ToList();

            if (maybankrcmsList.Count() > 0)
            {
                workerList = new SelectList(maybankrcmsList.Select(s => new SelectListItem { Value = s.fld_Nopkj.ToString(), Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();
            }

            return Json(workerList);
        }

        public JsonResult GetWorkerTNG(int? WilayahID, string CompCode, int Year, int Month)
        {
            List<SelectListItem> workerList = new List<SelectListItem>();
            var userDetail = getidentity.GetUserDetail(User.Identity.Name);
            var getuserid = userDetail.fldUserID;
            var maybankrcmsList = dbSP.sp_MaybankTNG(userDetail.fldNegaraID, userDetail.fldSyarikatID, WilayahID, Year, Month, getuserid, CompCode).Select(s => new { s.fld_Nopkj, s.fld_Nama }).OrderBy(o => o.fld_Nama).ToList();

            if (maybankrcmsList.Count() > 0)
            {
                workerList = new SelectList(maybankrcmsList.Select(s => new SelectListItem { Value = s.fld_Nopkj.ToString(), Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();
            }

            return Json(workerList);
        }

        public JsonResult GetWilayah(string SyarikatID)
        {
            List<SelectListItem> wilayahlist = new List<SelectListItem>();
            //List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var syarikatCodeId = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fldOptConfValue == SyarikatID.ToString() && x.fld_NegaraID == NegaraID).Select(x => x.fld_SyarikatID).FirstOrDefault();
            int SyarikatCode = Convert.ToInt16(syarikatCodeId);

            if (getWilayah.GetAvailableWilayah(SyarikatCode))
            {
                if (WilayahID == 0)
                {
                    //dapatkan ladang filter by costcenter
                    var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatID && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                    var listwilayah1 = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                    wilayahlist = new SelectList(listwilayah1.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    wilayahlist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
                    //ladanglist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
                }
                else
                {
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID2 && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    wilayahlist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
                    //ladanglist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
                }
            }

            return Json(wilayahlist);
        }

        public JsonResult CheckTngGenDataDetail(int Month, int Year, string CompCode, int Wilayah, string[] WorkerId)
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

            List<sp_MaybankTNG_Result> maybankrcmsList = new List<sp_MaybankTNG_Result>();
            if (WorkerId == null)
                WorkerId = new string[] { "0" };

            if (WorkerId.Contains("0"))
            {
                maybankrcmsList = dbSP.sp_MaybankTNG(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).ToList();
            }
            else
            {
                maybankrcmsList = dbSP.sp_MaybankTNG(NegaraID.Value, SyarikatID.Value, Wilayah, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
            }
            var WilayahDetail = dbC.tbl_Wilayah.Where(x => x.fld_ID == Wilayah).FirstOrDefault();
            string filename = "TNG BURUH (" + InitialName + ") " + WilayahDetail.fld_WlyhName.ToUpper() + " " + stringmonth + stringyear + ".txt";

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

        public ActionResult TngGen()
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
            List<SelectListItem> WilayahList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                WilayahList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();

                CompCodeList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == 1 && x.fld_NegaraID == 1).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            }
            else
            {
                WilayahList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName").ToList();
                CompCodeList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == 1 && x.fld_NegaraID == 1).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            }
            CompCodeList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;

            WilayahList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.WilayahList = WilayahList;


            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ViewResult _tng(int? WilayahList, string CompCodeList, int? MonthList, int? YearList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string WilayahName = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankTNG_Result> maybankrcmsList = new List<sp_MaybankTNG_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NamaPndkSyarikat)
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
            if (MonthList == null || YearList == null || CompCodeList == "0" || WilayahList == 0)
            {
                ViewBag.Message = "Sila pilih Bulan, Tahun, Syarikat, Wilayah dan Tarikh Bayaran";
                return View(maybankrcmsList);
            }
            else
            {
                maybankrcmsList = dbSP.sp_MaybankTNG(NegaraID, SyarikatID, WilayahList, YearList, MonthList, getuserid, CompCodeList).ToList();

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

        public ActionResult TngGenIndividu()
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

        public ViewResult _tngIndividu(int? WilayahList, string CompCodeList, int? MonthList, int? YearList, string print, string filter, string[] WorkerId)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string WilayahName = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankTNG_Result> maybankrcmsList = new List<sp_MaybankTNG_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NamaPndkSyarikat)
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
                return View(maybankrcmsList);
            }
            else
            {
                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbSP.sp_MaybankTNG(NegaraID, SyarikatID, WilayahList, YearList, MonthList, getuserid, CompCodeList).ToList();
                }
                else
                {
                    maybankrcmsList = dbSP.sp_MaybankTNG(NegaraID, SyarikatID, WilayahList, YearList, MonthList, getuserid, CompCodeList).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

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

        public ActionResult RcmsGenOthers()
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
            List<SelectListItem> WilayahList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                WilayahList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();

                CompCodeList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == 1 && x.fld_NegaraID == 1).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            }
            else
            {
                WilayahList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName").ToList();
                CompCodeList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == 1 && x.fld_NegaraID == 1).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            }
            CompCodeList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;

            WilayahList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.WilayahList = WilayahList;

            List<SelectListItem> IncentiveList = new List<SelectListItem>();
            IncentiveList = new SelectList(dbC.tbl_JenisInsentif.Where(x => x.fld_InclSecondPayslip == true && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodInsentif).Select(s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }), "Value", "Text").ToList(); IncentiveList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.IncentiveList = IncentiveList;
            ViewBag.UserID = getuserid;

            return View();
        }

        public ViewResult _rcmsOthers(string CompCodeList, int? WilayahList, int? MonthList, int? YearList, string IncentiveList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string NamaSyarikat = "";
            string ClientId = "";
            String WilayahName = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsOthers_Result> maybankrcmsList = new List<sp_MaybankRcmsOthers_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Syarikat
                 .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                 .Select(s => s.fld_NamaSyarikat)
                 .FirstOrDefault();
            ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NamaPndkSyarikat)
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


            if (ClientId == null || ClientId == "")
            {
                if (CompCodeList == "1000")
                {
                    ViewBag.clientid = "FELDA" + MonthList + YearList;
                }

                if (CompCodeList == "8800")
                {
                    ViewBag.clientid = "FPMSB" + MonthList + YearList;
                }
            }
            else
            {
                ViewBag.clientid = ClientId;
            }

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;

            ViewBag.Description = "Wilayah " + WilayahName + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null || CompCodeList == "0" || IncentiveList == "0")
            {
                ViewBag.Message = "Sila pilih Bulan, Tahun, Syarikat, Wilayah, Insentif dan Tarikh Bayaran";
                return View(maybankrcmsList);
            }
            else
            {
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("WilayahID", WilayahList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Month", MonthList);
                    parameters.Add("UserID", getuserid); 
                    parameters.Add("CompCode", CompCodeList);
                    parameters.Add("Incentive", IncentiveList);

                    con.Open();
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsOthers_Result>(con, "sp_MaybankRcmsOthers", parameters).ToList();
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }


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

        public ActionResult RcmsGenIndividuOthers()
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
            CompCodeList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            CompCodeList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;

            List<SelectListItem> WilayahList = new List<SelectListItem>();
            WilayahList = new SelectList(dbC.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            WilayahList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.WilayahList = WilayahList;

            List<SelectListItem> IncentiveList = new List<SelectListItem>();
            IncentiveList = new SelectList(dbC.tbl_JenisInsentif.Where(x => x.fld_InclSecondPayslip == true && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodInsentif).Select(s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }), "Value", "Text").ToList(); IncentiveList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.IncentiveList = IncentiveList;

            //List<SelectListItem> WilayahList = new List<SelectListItem>();
            //WilayahList = new SelectList(dbC.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID)
            //    .OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            //WilayahList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            //ViewBag.WilayahList = WilayahList;

            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ViewResult _rcmsIndividuOthers(string CompCodeList, int? WilayahList, int? MonthList, int? YearList, string IncentiveList, string print, string filter, string[] WorkerId)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string WilayahName = "";
            string NamaSyarikat = "";
            string ClientId = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsOthers_Result> maybankrcmsList = new List<sp_MaybankRcmsOthers_Result>();


            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Syarikat
                 .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                 .Select(s => s.fld_NamaSyarikat)
                 .FirstOrDefault();
            ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SAPComCode == CompCodeList)
                .Select(s => s.fld_NamaPndkSyarikat)
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

            if (ClientId == null || ClientId == "")
            {
                if (CompCodeList == "1000")
                {
                    ViewBag.clientid = "FELDA" + MonthList + YearList;
                }

                if (CompCodeList == "8800")
                {
                    ViewBag.clientid = "FPMSB" + MonthList + YearList;
                }
            }
            else
            {
                ViewBag.clientid = ClientId;
            }

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;

            ViewBag.Description = "Wilayah " + WilayahName + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null || CompCodeList == "0" || IncentiveList == "0")
            {
                ViewBag.Message = "Sila pilih Bulan, Tahun, Syarikat, Wilayah, Insentif dan Tarikh Bayaran";
                return View(maybankrcmsList);
            }
            else
            {
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("WilayahID", WilayahList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Month", MonthList);
                    parameters.Add("UserID", getuserid);
                    parameters.Add("CompCode", CompCodeList);
                    parameters.Add("Incentive", IncentiveList);

                    con.Open();

                    if (WorkerId.Contains("0"))
                    {
                        maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsOthers_Result>(con, "sp_MaybankRcmsOthers", parameters).ToList();
                    }
                    else
                    {
                        maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsOthers_Result>(con, "sp_MaybankRcmsOthers", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }

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


        public JsonResult CheckGenDataDetailOthers(int Month, int Year, string CompCode, int Wilayah, string Incentive, string[] WorkerId)
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
            string ClientIDText = "";
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


            List<sp_MaybankRcmsOthers_Result> maybankrcmsList = new List<sp_MaybankRcmsOthers_Result>();

            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("WilayahID", Wilayah);
                parameters.Add("Year", Year);
                parameters.Add("Month", Month);
                parameters.Add("UserID", getuserid);
                parameters.Add("CompCode", CompCode);
                parameters.Add("Incentive", Incentive);
                con.Open();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsOthers_Result>(con, "sp_MaybankRcmsOthers", parameters).ToList();
                }
                else
                {
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsOthers_Result>(con, "sp_MaybankRcmsOthers", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

           
            var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_SAPComCode == CompCode).FirstOrDefault();

            string filename = "M2E BURUH (" + SyarikatDetail.fld_NamaPndkSyarikat.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";

            if (ClientID == null || ClientID == " ")
            {
                if (CompCode == "1000")
                {
                    ClientIDText = "FELDA" + stringmonth + stringyear;
                }

                if (CompCode == "8800")
                {
                    ClientIDText = "FPMSB" + stringmonth + stringyear;
                }
            }
            else
            {
                ClientIDText = ClientID;
            }

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

            dbSP.Dispose();
            dbC.Dispose();
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText });
        }

        [HttpPost]
        public ActionResult DownloadTextOthers(int Month, int Year, string CompCode, int Wilayah, string filter, string[] WorkerId, DateTime PaymentDate, string Incentive)
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
                List<sp_MaybankRcmsOthers_Result> maybankrcmsList = new List<sp_MaybankRcmsOthers_Result>();

                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("WilayahID", Wilayah);
                    parameters.Add("Year", Year);
                    parameters.Add("Month", Month);
                    parameters.Add("UserID", getuserid);
                    parameters.Add("CompCode", CompCode);
                    parameters.Add("Incentive", Incentive);
                    con.Open();

                    if (WorkerId == null)
                        WorkerId = new string[] { "0" };

                    if (WorkerId.Contains("0"))
                    {
                        maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsOthers_Result>(con, "sp_MaybankRcmsOthers", parameters).ToList();
                    }
                    else
                    {
                        maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsOthers_Result>(con, "sp_MaybankRcmsOthers", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                    }

                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }


                var WilayahDetail = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == Wilayah).FirstOrDefault();

                filePath = GetGenerateFile.GenFileMaybankOthers(maybankrcmsList, WilayahDetail, stringmonth, stringyear, NegaraID, SyarikatID, Wilayah, CompCode, filter, PaymentDate, Incentive, out filename);

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


        public JsonResult GetWorkerOthers(string CompCode, int? WilayahID, int Year, int Month, string Incentive)
        {
            List<SelectListItem> workerList = new List<SelectListItem>();
            List<sp_MaybankRcmsOthers_Result> maybankrcmsList = new List<sp_MaybankRcmsOthers_Result>();
            IEnumerable<dynamic> maybankrcmsDataList = new List<dynamic>();

            var userDetail = getidentity.GetUserDetail(User.Identity.Name);
            var getuserid = userDetail.fldUserID;

            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", userDetail.fldNegaraID);
                parameters.Add("SyarikatID", userDetail.fldSyarikatID);
                parameters.Add("WilayahID", WilayahID);
                parameters.Add("Year", Year);
                parameters.Add("Month", Month);
                parameters.Add("UserID", getuserid);
                parameters.Add("CompCode", CompCode);
                parameters.Add("Incentive", Incentive);

                con.Open();
                maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsOthers_Result>(con, "sp_MaybankRcmsOthers", parameters).ToList();
                maybankrcmsDataList = maybankrcmsList.Select(s => new { s.fld_Nopkj, s.fld_Nama }).OrderBy(o => o.fld_Nama);
                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            //var maybankrcmsList = dbSP.sp_MaybankRcmsOthers(userDetail.fldNegaraID, userDetail.fldSyarikatID, Year, Month, getuserid, CompCode, Incentive).Select(s => new { s.fld_Nopkj, s.fld_Nama }).OrderBy(o => o.fld_Nama).ToList();

            if (maybankrcmsDataList.Count() > 0)
            {
                workerList = new SelectList(maybankrcmsList.Select(s => new SelectListItem { Value = s.fld_Nopkj.ToString(), Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();
            }

            return Json(workerList);
        }

    }
}