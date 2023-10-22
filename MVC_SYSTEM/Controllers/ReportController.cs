using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsEstate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using MVC_SYSTEM.ModelsSP;
//kamalia 03022021
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web;
//
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.ModelsCustom;

namespace MVC_SYSTEM.Controllers
{

    public class ReportController : Controller
    {
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        //private MVC_SYSTEM_SP_Models db3 = new MVC_SYSTEM_SP_Models();
        //private MVC_SYSTEM_SP_Models db4 = new MVC_SYSTEM_SP_Models();
        private MVC_SYSTEM_ModelsCorporate db5 = new MVC_SYSTEM_ModelsCorporate();
        private GetIdentity getidentity = new GetIdentity();
        errorlog geterror = new errorlog();
        GetWilayah getwilyah = new GetWilayah();
        GetNSWL GetNSWL = new GetNSWL();
        GetConfig GetConfig = new GetConfig();
        ConvertToPdf ConvertToPdf = new ConvertToPdf();
        GetTriager GetTriager = new GetTriager();

        //Added by Shazana 3/4/2023
        private GeneralClass GeneralClass = new GeneralClass();

        //new Models
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();

        // GET: Report
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Resource,Viewer")]
        public ActionResult Index()
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? getroleid = getidentity.getRoleID(getuserid);
            int?[] reportid = new int?[] { };

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.Report = "class = active";
            reportid = dbC.tblRoleReports.Where(x => x.fldRoleID == getroleid && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).Select(s => s.fldReportID).ToArray();

            List<SelectListItem> SubReportList = new List<SelectListItem>();
            if (!getidentity.NegaraSumber(User.Identity.Name))
            {
                ViewBag.ReportList = new SelectList(dbC.tblReportLists.Where(x => reportid.Contains((x.fldReportListID)) && x.fldDeleted == false).OrderBy(o => o.fldReportListID).Select(s => new SelectListItem { Value = s.fldReportListID.ToString(), Text = s.fldReportListName }), "Value", "Text").ToList();
            }
            else
            {
                ViewBag.ReportList = new SelectList(dbC.tblReportLists.Where(x => reportid.Contains((x.fldReportListID)) && x.fldDeleted == false).OrderBy(o => o.fldReportListID).Select(s => new SelectListItem { Value = s.fldReportListID.ToString(), Text = s.fldReportListName }), "Value", "Text").ToList();
            }
            ViewBag.SubReportList = SubReportList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User, Resource,Viewer")]
        public ActionResult Index(int ReportList, int SubReportList)
        {
            string action = "", controller = "";
            var getreport = dbC.tblReportLists.Find(ReportList);

            if (getreport.fldSubReport == true && SubReportList > 0)
            {
                var getsubreport = dbC.tblSubReportLists.Where(x => x.fldSubReportListID == SubReportList && x.fldMainReportID == ReportList).FirstOrDefault();
                action = getsubreport.fldSubReportListAction;
                controller = getsubreport.fldSubReportListController;
            }
            else
            {
                action = getreport.fldReportListAction;
                controller = getreport.fldReportListController;
            }
            return RedirectToAction(action, controller);
        }

        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ActionResult ActiveWorker()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                incldg = 1;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahID;
                incldg = 2;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 3;
            }

            //sarah comment
            //List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            //List<SelectListItem> LadangIDList = new List<SelectListItem>();

            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    wilayahselection = WilayahID;
            //    incldg = 1;

            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

            //    wilayahselection = WilayahID;
            //    ladangselection = LadangID;
            //    incldg = 1;
            //}

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> status = new List<SelectListItem>();

            //status.Add(new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" });
            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = true });
            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2" });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.YearList = yearlist;
            ViewBag.Year = year;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            //sarah added
            ViewBag.SyarikatList = SyarikatList;
            ViewBag.LadangID = 0;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Link = domain;
            ViewBag.Status1 = 1;

            List<sp_RptRumKedKepPekLad_Result> resultreport = new List<sp_RptRumKedKepPekLad_Result>();

            return View("ActiveWorker", resultreport);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]

        //sarah added SyarikatList parameter
        public ActionResult ActiveWorker(int? YearList, int WilayahIDList, int LadangIDList, int Status, string SyarikatList)
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int drpyear = 0;
            int drprangeyear = 0;
            bool activestatus0 = false, activestatus1 = false, activestatus2 = false;
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            int incldg2 = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();
            ///sarah added
            List<SelectListItem> SyarikatList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {

                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahIDList2 = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                wlyhid = getwilyah.GetWilayahID(SyarikatID);

                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();

                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();

                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }

            //sarah comment
            //List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            //List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
            //    WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    if (WilayahIDList == 0)
            //    {
            //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    else
            //    {
            //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //    wilayahselection = WilayahIDList;
            //    ladangselection = LadangIDList;
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
            //    if (WilayahIDList == 0)
            //    {
            //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    else
            //    {
            //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //    wilayahselection = WilayahIDList;
            //    ladangselection = LadangIDList;
            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
            //    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();

            //    wilayahselection = WilayahIDList;
            //    ladangselection = LadangIDList;
            //}

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<sp_RptRumKedKepPekLad_Result> resultreport = new List<sp_RptRumKedKepPekLad_Result>();

            if (WilayahIDList == 0)
            {
                dbSP.SetCommandTimeout(120);
                resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid, SyarikatList).ToList();
            }
            else
            {
                if (LadangIDList == 0)
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid, SyarikatList).ToList();
                    incldg = 1;
                }
                else
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid, SyarikatList).ToList();
                    incldg = 1;
                }
            }

            ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();
            ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
            ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();

            if (WilayahIDList == 0)
            {
                incldg2 = 1;
            }
            else
            {
                if (LadangIDList == 0)
                {
                    incldg2 = 2;
                }
                else
                {
                    incldg2 = 3;
                }
            }

            switch (Status)
            {
                case 0:
                    activestatus0 = true;
                    break;
                case 1:
                    activestatus1 = true;
                    break;
                case 2:
                    activestatus2 = true;
                    break;
            }

            List<SelectListItem> status = new List<SelectListItem>();

            //status.Add(new SelectListItem { Text = GlobalResReport.sltAll, Value = "2", Selected = activestatus2 });
            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = activestatus1 });
            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2", Selected = activestatus2 });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.Incldg2 = incldg2;
            ViewBag.YearList = yearlist; // list dalam dropdown
            ViewBag.Year = YearList; // year yg user select
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.SyarikatList = SyarikatList2;
            ViewBag.LadangID = LadangIDList;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.Link = domain;
            ViewBag.Status1 = Status;

            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            return View("ActiveWorker", resultreport);
        }

        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User, Viewer")]
        public ActionResult ActiveWorkerGMN()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                incldg = 1;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahID;
                incldg = 2;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 3;
            }


            //sarah comment
            //List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            //List<SelectListItem> LadangIDList = new List<SelectListItem>();

            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    wilayahselection = WilayahID;
            //    incldg = 1;

            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

            //    wilayahselection = WilayahID;
            //    ladangselection = LadangID;
            //    incldg = 1;
            //}

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> status = new List<SelectListItem>();

            //status.Add(new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" });
            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = true });
            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2" });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.YearList = yearlist;
            ViewBag.Year = year;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.SyarikatIDList = SyarikatList;
            ViewBag.LadangID = 0;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Link = domain;
            ViewBag.Status1 = 1;

            List<sp_RptRumKedKepPekLad_Result> resultreport = new List<sp_RptRumKedKepPekLad_Result>();

            return View("ActiveWorkerGMN", resultreport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ActionResult ActiveWorkerGMN(int? YearList, int WilayahIDList, int LadangIDList, int Status, string SyarikatList)
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int drpyear = 0;
            int drprangeyear = 0;
            bool activestatus0 = false, activestatus1 = false, activestatus2 = false;
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            int incldg2 = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();
            ///sarah added
            List<SelectListItem> SyarikatList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {

                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahIDList2 = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                wlyhid = getwilyah.GetWilayahID(SyarikatID);

                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();

                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();

                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }

            //List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            //List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
            //    WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    if (WilayahIDList == 0)
            //    {
            //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    else
            //    {
            //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //    wilayahselection = WilayahIDList;
            //    ladangselection = LadangIDList;
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
            //    if (WilayahIDList == 0)
            //    {
            //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    else
            //    {
            //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //    wilayahselection = WilayahIDList;
            //    ladangselection = LadangIDList;
            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
            //    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();

            //    wilayahselection = WilayahIDList;
            //    ladangselection = LadangIDList;
            //}

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<sp_RptRumKedKepPekLad_Result> resultreport = new List<sp_RptRumKedKepPekLad_Result>();

            if (WilayahIDList == 0)
            {
                dbSP.SetCommandTimeout(120);
                resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid, SyarikatList).ToList();
            }
            else
            {
                if (LadangIDList == 0)
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid, SyarikatList).ToList();
                    incldg = 1;
                }
                else
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid, SyarikatList).ToList();
                    incldg = 1;
                }
            }

            ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();
            ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
            ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();

            if (WilayahIDList == 0)
            {
                incldg2 = 1;
            }
            else
            {
                if (LadangIDList == 0)
                {
                    incldg2 = 2;
                }
                else
                {
                    incldg2 = 3;
                }
            }

            switch (Status)
            {
                case 0:
                    activestatus0 = true;
                    break;
                case 1:
                    activestatus1 = true;
                    break;
                case 2:
                    activestatus2 = true;
                    break;
            }

            List<SelectListItem> status = new List<SelectListItem>();

            //status.Add(new SelectListItem { Text = GlobalResReport.sltAll, Value = "2", Selected = activestatus2 });
            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = activestatus1 });
            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2", Selected = activestatus2 });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.Incldg2 = incldg2;
            ViewBag.YearList = yearlist; // list dalam dropdown
            ViewBag.Year = YearList; // year yg user select
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.SyarikatList = SyarikatList2;
            ViewBag.LadangID = LadangIDList;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.Link = domain;
            ViewBag.Status1 = Status;

            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            return View("ActiveWorkerGMN", resultreport);
        }

        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ActionResult WorkerSalary()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            //sarah added
            List<SelectListItem> SyarikatList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {

                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                //WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                WilayahIDList.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "0" });
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                //wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {

                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahID;
                ladangselection = LadangID;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }

            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
            //    int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
            //    var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
            //    var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
            //    WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            //    WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            //    //wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //    //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    //WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
            //    int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
            //    SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    wilayahselection = WilayahID;


            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
            //    int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
            //    SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    wilayahselection = WilayahID;
            //    ladangselection = LadangID;

            //}


            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> KerakyatanList = new List<SelectListItem>();
            List<ModelsSP.sp_RptBulPenPekLad_Result> resultreport = new List<ModelsSP.sp_RptBulPenPekLad_Result>();
            KerakyatanList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
            KerakyatanList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.YearList = yearlist;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            //sarah added
            ViewBag.SyarikatList = SyarikatList;
            ViewBag.KerakyatanList = KerakyatanList;
            ViewBag.UserID = getuserid;
            ViewBag.GetFlag = 1;

            return View(resultreport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ActionResult WorkerSalary(int? MonthList, int YearList, int WilayahIDList, int LadangIDList, string KerakyatanList, string SyarikatList)
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int drpyear = 0;
            int drprangeyear = 0;
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();
            //sarah added
            List<SelectListItem> SyarikatList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {

                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahIDList2 = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                // WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                // WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false ).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();

                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();

                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }


            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
            //    int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
            //    var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
            //    var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
            //    WilayahIDList2 = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            //    WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //    SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            //    wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //    //WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
            //    //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    if (WilayahIDList == 0)
            //    {
            //        //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //        LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    else
            //    {
            //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //        //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    wilayahselection = WilayahIDList;
            //    ladangselection = LadangIDList;
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
            //    int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
            //    SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
            //    if (WilayahIDList == 0)
            //    {
            //        LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //        //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    else
            //    {
            //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //        //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    }
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    wilayahselection = WilayahIDList;
            //    ladangselection = LadangIDList;
            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
            //    int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
            //    SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();

            //    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    wilayahselection = WilayahIDList;
            //    ladangselection = LadangIDList;
            //}

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> KerakyatanList2 = new List<SelectListItem>();
            KerakyatanList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", KerakyatanList).ToList();
            KerakyatanList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

            if (WilayahIDList == 0)
            {
                incldg = 1;
            }
            else
            {
                if (LadangIDList == 0)
                {
                    incldg = 2;
                }
                else
                {
                    incldg = 3;
                }
            }

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", MonthList);

            List<ModelsSP.sp_RptBulPenPekLad_Result> resultreport = new List<ModelsSP.sp_RptBulPenPekLad_Result>();

            dbSP.SetCommandTimeout(120);
            resultreport = dbSP.sp_RptBulPenPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, KerakyatanList, MonthList, YearList, getuserid, SyarikatList).ToList();


            ViewBag.YearList = yearlist;
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.SyarikatList = SyarikatList2;
            ViewBag.KerakyatanList = KerakyatanList2;
            ViewBag.NeragaID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.WilayahSelection = WilayahIDList;
            ViewBag.LadangSelection = LadangIDList;
            ViewBag.IncLdg = incldg;
            ViewBag.UserID = getuserid;
            ViewBag.GetFlag = 2;

            ViewBag.Month = MonthList;
            ViewBag.Year = YearList;

            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            return View(resultreport);
        }


        ////role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        //public ActionResult WorkerSalary()
        //{
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    List<SelectListItem> KerakyatanList = new List<SelectListItem>();
        //    List<ModelsSP.sp_RptBulPenPekLad_Result> resultreport = new List<ModelsSP.sp_RptBulPenPekLad_Result>();
        //    KerakyatanList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
        //    KerakyatanList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

        //    ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.KerakyatanList = KerakyatanList;

        //    ViewBag.UserID = getuserid;
        //    ViewBag.GetFlag = 1;

        //    return View(resultreport);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        ////role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        //public ActionResult WorkerSalary(int? MonthList, int YearList, int WilayahIDList, int LadangIDList, string KerakyatanList)
        //{
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == YearList)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    List<SelectListItem> KerakyatanList2 = new List<SelectListItem>();
        //    KerakyatanList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", KerakyatanList).ToList();
        //    KerakyatanList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

        //    if (WilayahIDList == 0)
        //    {
        //        incldg = 1;
        //    }
        //    else
        //    {
        //        if (LadangIDList == 0)
        //        {
        //            incldg = 2;
        //        }
        //        else
        //        {
        //            incldg = 3;
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", MonthList);

        //    List<ModelsSP.sp_RptBulPenPekLad_Result> resultreport = new List<ModelsSP.sp_RptBulPenPekLad_Result>();

        //    dbSP.SetCommandTimeout(120);
        //    resultreport = dbSP.sp_RptBulPenPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, KerakyatanList, MonthList, YearList, getuserid).ToList();


        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.KerakyatanList = KerakyatanList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = WilayahIDList;
        //    ViewBag.LadangSelection = LadangIDList;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.GetFlag = 2;

        //    ViewBag.Month = MonthList;
        //    ViewBag.Year = YearList;

        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //    return View(resultreport);
        //}

        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ActionResult MasterDataPkjReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //sarah added
            int incldg = 0;
            int[] wlyhid = new int[] { };
            int? wilayahselection = 0;
            int? ladangselection = 0;

            List<SelectListItem> statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWeb
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            statusList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            ViewBag.StatusList = statusList;

            List<SelectListItem> WilayahList = new List<SelectListItem>();
            List<SelectListItem> LadangList = new List<SelectListItem>();
            List<SelectListItem> SyarikatList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                WilayahList.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "0" });
                LadangList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                //wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                incldg = 1;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                LadangList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahID;
                incldg = 2;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                LadangList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 3;
            }

            //List<SelectListItem> wilayahList = new List<SelectListItem>();
            //List<SelectListItem> ladangList = new List<SelectListItem>(); // fatin added - 17/04/2023
            //List<SelectListItem> SyarikatList = new List<SelectListItem>(); //sarah added

            ////comment by 17/04/2023
            ///*wilayahList = new SelectList(
            //    db.tbl_Wilayah
            //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
            //        .OrderBy(o => o.fld_WlyhName)
            //        .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }),
            //    "Value", "Text").ToList();*/

            ////fatin added - 17/04/2023
            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
            //    int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
            //    var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
            //    var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
            //    wilayahList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            //    ladangList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            //    //sarah comment
            //    //wilayahList = new SelectList(
            //    //db.tbl_Wilayah
            //    //    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
            //    //    .Select(
            //    //        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            //    //wilayahList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            //    //ladangList = new SelectList(
            //    //db.tbl_Ladang
            //    //    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
            //    //    .OrderBy(o => o.fld_LdgName)
            //    //    .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text").ToList();
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{

            //    var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
            //    int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
            //    SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    wilayahList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

            //    ladangList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    ladangList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //    wilayahselection = WilayahID;
            //    ladangselection = LadangID;

            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
            //    int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
            //    SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    wilayahList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

            //    ladangList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    ladangList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    wilayahselection = WilayahID;
            //    ladangselection = LadangID;
            //}

            ////else
            ////{
            ////    wilayahList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName").ToList();

            ////    ladangList = new SelectList(
            ////    db.tbl_Ladang
            ////        .Where(x => x.fld_NegaraID == NegaraID && x.fld_WlyhID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
            ////        .OrderBy(o => o.fld_LdgName)
            ////        .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text").ToList();

            ////}
            ////end


            //ViewBag.WilayahList = wilayahList;

            ////comment by fatin - 17/04/2023
            ////List<SelectListItem> ladangList = new List<SelectListItem>();

            ///*ladangList = new SelectList(
            //    db.tbl_Ladang
            //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
            //        .OrderBy(o => o.fld_LdgName)
            //        .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }),
            //    "Value", "Text").ToList();*/

            //ViewBag.LadangList = ladangList;
            //ViewBag.SyarikatList = SyarikatList;

            List<SelectListItem> kategoriPekerjaList = new List<SelectListItem>();
            kategoriPekerjaList = new SelectList(
                db.tblOptionConfigsWeb
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            kategoriPekerjaList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            ViewBag.KategoriPekerjaList = kategoriPekerjaList;

            List<SelectListItem> kerakyatanList = new List<SelectListItem>();
            kerakyatanList = new SelectList(
                db.tblOptionConfigsWeb
                    .Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            kerakyatanList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            ViewBag.KerakyatanList = kerakyatanList;
            ViewBag.WilayahList = WilayahList;
            ViewBag.LadangList = LadangList;
            ViewBag.SyarikatList = SyarikatList;

            return View();
        }


        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ViewResult _MasterDataPkjReport(string StatusList, int? WilayahList, int? LadangList, string KategoriPekerjaList, string KerakyatanList, string filter, string print, string SyarikatList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<sp_RptMasterDataPkj_Result> rptMasterDataPkjResults = new List<sp_RptMasterDataPkj_Result>();

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;

            if (String.IsNullOrEmpty(WilayahList.ToString()) || String.IsNullOrEmpty(LadangList.ToString()))
            {
                ViewBag.Message = @GlobalResCorp.lblChooseWorkerMasterDataReport;
            }

            else
            {

                //kamalia 27/1/2021
                if (!String.IsNullOrEmpty(filter))
                {
                    if (WilayahID == 0)
                    {
                        dbSP.SetCommandTimeout(600);
                        rptMasterDataPkjResults = dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, WilayahList, LadangList,
                               KerakyatanList, StatusList, KategoriPekerjaList, getuserid, SyarikatList)
                           .ToList();

                        var filter1 = rptMasterDataPkjResults.Where(x => x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
                                    x.fld_NoPkj.ToUpper().Contains(filter.ToUpper()) ||
                                    x.fld_NoKP.ToUpper().Contains(filter.ToUpper())).Select(s => s.fld_WilayahID).FirstOrDefault();
                        dbSP.SetCommandTimeout(600);
                        rptMasterDataPkjResults = dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, filter1, LadangList,
                                KerakyatanList, StatusList, KategoriPekerjaList, getuserid, SyarikatList)
                            .Where(x => x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
                                        x.fld_NoPkj.ToUpper().Contains(filter.ToUpper()) ||
                                        x.fld_NoKP.ToUpper().Contains(filter.ToUpper())).ToList();
                    }
                    else
                    {
                        dbSP.SetCommandTimeout(600);
                        rptMasterDataPkjResults = dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, WilayahList, LadangList,
                                KerakyatanList, StatusList, KategoriPekerjaList, getuserid, SyarikatList)
                            .Where(x => x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
                                        x.fld_NoPkj.ToUpper().Contains(filter.ToUpper()) ||
                                        x.fld_NoKP.ToUpper().Contains(filter.ToUpper())).ToList();
                    }
                }
                //kamalia 27/1/2021
                //
                else
                {
                    dbSP.SetCommandTimeout(600);
                    rptMasterDataPkjResults = dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, WilayahList, LadangList,
                               KerakyatanList, StatusList, KategoriPekerjaList, getuserid, SyarikatList)
                           .ToList();
                }

                if (rptMasterDataPkjResults.Count == 0)
                {
                    ViewBag.Message = @GlobalResCorp.msgNoRecord;
                }
            }

            return View(rptMasterDataPkjResults);
        }

        ////role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        //public ActionResult MasterDataPkjReport()
        //{
        //    ViewBag.Report = "class = active";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> statusList = new List<SelectListItem>();
        //    statusList = new SelectList(
        //        db.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();
        //    statusList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

        //    ViewBag.StatusList = statusList;

        //    List<SelectListItem> wilayahList = new List<SelectListItem>();
        //    List<SelectListItem> ladangList = new List<SelectListItem>(); // fatin added - 17/04/2023

        //    //comment by 17/04/2023
        //    /*wilayahList = new SelectList(
        //        db.tbl_Wilayah
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //            .OrderBy(o => o.fld_WlyhName)
        //            .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }),
        //        "Value", "Text").ToList();*/

        //    //fatin added - 17/04/2023
        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wilayahList = new SelectList(
        //        db.tbl_Wilayah
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
        //            .Select(
        //                s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
        //        wilayahList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //        ladangList = new SelectList(
        //        db.tbl_Ladang
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //            .OrderBy(o => o.fld_LdgName)
        //            .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text").ToList();
        //    }

        //    else
        //    {
        //        wilayahList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName").ToList();

        //        ladangList = new SelectList(
        //        db.tbl_Ladang
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_WlyhID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //            .OrderBy(o => o.fld_LdgName)
        //            .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text").ToList();

        //    }
        //    //end


        //    ViewBag.WilayahList = wilayahList;

        //    //comment by fatin - 17/04/2023
        //    //List<SelectListItem> ladangList = new List<SelectListItem>();

        //    /*ladangList = new SelectList(
        //        db.tbl_Ladang
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //            .OrderBy(o => o.fld_LdgName)
        //            .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }),
        //        "Value", "Text").ToList();*/

        //    ladangList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    ViewBag.LadangList = ladangList;

        //    List<SelectListItem> kategoriPekerjaList = new List<SelectListItem>();
        //    kategoriPekerjaList = new SelectList(
        //        db.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();
        //    kategoriPekerjaList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

        //    ViewBag.KategoriPekerjaList = kategoriPekerjaList;

        //    List<SelectListItem> kerakyatanList = new List<SelectListItem>();
        //    kerakyatanList = new SelectList(
        //        db.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();
        //    kerakyatanList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

        //    ViewBag.KerakyatanList = kerakyatanList;

        //    return View();
        //}


        ////role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        //public ViewResult _MasterDataPkjReport(string StatusList, int? WilayahList, int? LadangList, string KategoriPekerjaList, string KerakyatanList, string filter, string print)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<sp_RptMasterDataPkj_Result> rptMasterDataPkjResults = new List<sp_RptMasterDataPkj_Result>();

        //    ViewBag.NamaSyarikat = db.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NamaSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NoSyarikat = db.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NoSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NegaraID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.UserName = User.Identity.Name;
        //    ViewBag.Date = DateTime.Now.ToShortDateString();
        //    ViewBag.Print = print;

        //    if (String.IsNullOrEmpty(WilayahList.ToString()) || String.IsNullOrEmpty(LadangList.ToString()))
        //    {
        //        ViewBag.Message = @GlobalResCorp.lblChooseWorkerMasterDataReport;
        //    }

        //    else
        //    {

        //            //kamalia 27/1/2021
        //            if (!String.IsNullOrEmpty(filter))
        //            {
        //                if (WilayahID == 0)
        //                {
        //                    rptMasterDataPkjResults = dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, WilayahList, LadangList,
        //                       KerakyatanList, StatusList, KategoriPekerjaList, getuserid)
        //                   .ToList();

        //                    var filter1 = rptMasterDataPkjResults.Where(x => x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
        //                                x.fld_NoPkj.ToUpper().Contains(filter.ToUpper()) ||
        //                                x.fld_NoKP.ToUpper().Contains(filter.ToUpper())).Select(s => s.fld_WilayahID).FirstOrDefault();

        //                    rptMasterDataPkjResults = dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, filter1, LadangList,
        //                        KerakyatanList, StatusList, KategoriPekerjaList, getuserid)
        //                    .Where(x => x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
        //                                x.fld_NoPkj.ToUpper().Contains(filter.ToUpper()) ||
        //                                x.fld_NoKP.ToUpper().Contains(filter.ToUpper())).ToList();
        //                }
        //                else
        //                {
        //                    rptMasterDataPkjResults = dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, WilayahList, LadangList,
        //                        KerakyatanList, StatusList, KategoriPekerjaList, getuserid)
        //                    .Where(x => x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
        //                                x.fld_NoPkj.ToUpper().Contains(filter.ToUpper()) ||
        //                                x.fld_NoKP.ToUpper().Contains(filter.ToUpper())).ToList();
        //                }
        //            }
        //            //kamalia 27/1/2021
        //            //
        //            else
        //            {
        //                rptMasterDataPkjResults = dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, WilayahList, LadangList,
        //                       KerakyatanList, StatusList, KategoriPekerjaList, getuserid)
        //                   .ToList();
        //            }

        //        if (rptMasterDataPkjResults.Count == 0)
        //        {
        //            ViewBag.Message = @GlobalResCorp.msgNoRecord;
        //        }
        //    }

        //    return View(rptMasterDataPkjResults);
        //}

        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ActionResult GajiMinimaReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //farahin tambah declaration
            int[] wlyhid = new int[] { };
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int? syarikatselection = 0;
            int incldg = 0;

            //farahin comment yg asal sbb textbox ladang tak filter ikut wilayah
            //List<SelectListItem> wilayahID = new List<SelectListItem>();

            //wilayahID = new SelectList(
            //    db.tbl_Wilayah
            //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
            //        .OrderBy(o => o.fld_WlyhName)
            //        .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }),
            //    "Value", "Text").ToList();
            //wilayahID.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            //ViewBag.WilayahList = wilayahID;

            //List<SelectListItem> ladangList = new List<SelectListItem>();

            //ladangList = new SelectList(
            //    db.tbl_Ladang
            //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_Deleted == false)
            //        .OrderBy(o => o.fld_LdgName)
            //        .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }),
            //    "Value", "Text").ToList();
            //ladangList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
            // ViewBag.LadangList = ladangList;

            //farahin tambah- 22012021
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatIDList = new List<SelectListItem>();//Added by Shazana 1/8/2023

            //Added by Shazana 1/8/2023
            if (WilayahID == 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatIDList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatIDList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID) && x.fld_Deleted == false), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatIDList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID) && x.fld_Deleted == false), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            ViewBag.SyarikatList = SyarikatIDList; //Added by Shazana 1/8/2023
            ViewBag.WilayahList = WilayahIDList;
            ViewBag.LadangList = LadangIDList;

            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            int month = timezone.gettimezone().Month;

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month-1);

            return View();
        }

        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        //Modified by Shazana 1/8/2023
        public ViewResult _GajiMinimaReport(int? WilayahList, int? LadangList, int? MonthList, int? YearList, string print, string SyarikatList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<sp_RptGajiMinima_Result> rptGajiMinimaResults = new List<sp_RptGajiMinima_Result>();

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;

            //Modified by Shazana 1/8/2023
            try
            {

                if (String.IsNullOrEmpty(WilayahList.ToString()) || String.IsNullOrEmpty(LadangList.ToString()))
                {
                    ViewBag.Message = @GlobalResCorp.lblChooseMinimumWageReport;
                }

                else
                {
                    rptGajiMinimaResults = dbSP.sp_RptGajiMinima(NegaraID, SyarikatID, WilayahList, LadangList, MonthList, YearList, getuserid, SyarikatList)
                        .ToList();

                    if (rptGajiMinimaResults.Count == 0)
                    {
                        ViewBag.Message = @GlobalResCorp.msgNoRecord;
                    }
                }

            }
            catch (Exception ex)
            {

                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                ViewBag.Message = @GlobalResCorp.msgDataNotFound;
            }

            return View(rptGajiMinimaResults);
        }

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public PartialViewResult WorkerSalaryDetail(int selection, int negaraid, int syarikatid, int wilayahid, int ladangid, string krytan, int month, int year, int bill, int incldg)
        //{
        //    List<sp_RptBulPenPekLad_Result> resultreport = new List<sp_RptBulPenPekLad_Result>();
        //    dbC.SetCommandTimeout(120);
        //    resultreport = db3.sp_RptBulPenPekLad(selection, negaraid, syarikatid, wilayahid, ladangid, krytan, month, year).ToList();
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaKyrtan = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == krytan && x.fldOptConfFlag1 == "krytnlist").Select(s => s.fldOptConfDesc).FirstOrDefault();
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.Bill = bill;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.DataCount = resultreport.Count(); //db3.sp_RptBulPenPekLad(selection, negaraid, syarikatid, wilayahid, ladangid, krytan, month, year).Count();
        //    return View("WorkerSalaryDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerSalarySummary(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year)
        //{
        //    List<sp_RptBulPenPekLadSum_Result> resultreport = new List<sp_RptBulPenPekLadSum_Result>();
        //    //db4.SetCommandTimeout(120);
        //    resultreport = db4.sp_RptBulPenPekLadSum(negaraid, syarikatid, wilayahid, ladangid, "MA", month, year).ToList();
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    return View("WorkerSalarySummary", resultreport);
        //}
        public ActionResult EstateTransactionListing()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.Report = "class = active";

            List<SelectListItem> wilayahList = new List<SelectListItem>();
            //comment by fatin - 17/04/2023
            /*wilayahList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();*/

            //fatin added - 17/04/2023
            if (WilayahID == 0 && LadangID == 0)
            {
                wilayahList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            }

            else
            {
                wilayahList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName").ToList();

            }
            wilayahList.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "0" });
            //end

            // wilayahList.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "0" });
            ViewBag.WilayahList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();
            //    ladangList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            ViewBag.LadangList = ladangList;

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            return View();
        }

        public ViewResult _TransactionListingRptSearch(int? WilayahList, int? LadangList, int? YearList, int? MonthList)
        {
            Connection Connection = new Connection();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (!string.IsNullOrEmpty(WilayahList.ToString()))
            {
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

                ViewBag.MonthList = MonthList;
                ViewBag.YearList = YearList;

                ViewBag.NamaSyarikat = db.tbl_Syarikat
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .Select(s => s.fld_NamaSyarikat)
                    .FirstOrDefault();
                ViewBag.NoSyarikat = db.tbl_Syarikat
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .Select(s => s.fld_NoSyarikat)
                    .FirstOrDefault();
                ViewBag.NegaraID = NegaraID;
                ViewBag.SyarikatID = SyarikatID;
                ViewBag.UserID = getuserid;
                ViewBag.UserName = User.Identity.Name;
                ViewBag.Date = DateTime.Now.ToShortDateString();
                //ViewBag.NamaPengurus = dbview2.tbl_Ladang
                //    .Where(x => x.fld_ID == LadangID)
                //    .Select(s => s.fld_Pengurus).Single();
                //ViewBag.NamaPenyelia = dbview2.tblUsers
                //    .Where(x => x.fldUserID == getuserid)
                //    .Select(s => s.fldUserFullName).Single();
                //ViewBag.IDPenyelia = getuserid;
                //ViewBag.Print = print;


                if (MonthList == null && YearList == null)
                {
                    ViewBag.Message = "Sila Pilih Bulan Dan Tahun";
                    return View();
                }

                else
                {
                    var TransactionListingList = estateConnection.vw_RptSctran
                        .Where(x => x.fld_KodAktvt != "3803" && x.fld_KodAktvt != "3800" && x.fld_KodAktvt != "3807" && x.fld_Month == MonthList &&
                                    x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList &&
                                    x.fld_LadangID == LadangList)
                        .OrderBy(o => new { o.fld_Kategori, o.fld_Amt });

                    if (!TransactionListingList.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    ViewBag.UserID = getuserid;
                    return View(TransactionListingList);
                }
            }
            else
            {
                var TransactionListingList = new List<vw_RptSctran>();
                return View(TransactionListingList);
            }

        }


        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ActionResult LocalWorkerInfo()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                WilayahIDList.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "0" });
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                //wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                incldg = 1;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahID;
                incldg = 2;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 3;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> status = new List<SelectListItem>();

            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = true });

            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2" });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.YearList = yearlist;
            ViewBag.Year = year;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.SyarikatList = SyarikatList;
            ViewBag.LadangID = 0;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Link = domain;
            ViewBag.Status1 = 1;

            List<sp_RptMakPekTem_Result> resultreport = new List<sp_RptMakPekTem_Result>();

            return View("LocalWorkerInfo", resultreport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ActionResult LocalWorkerInfo(int? YearList, int WilayahIDList, int LadangIDList, int Status, string SyarikatList)
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int drpyear = 0;
            int drprangeyear = 0;
            bool activestatus0 = false, activestatus1 = false;
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();
            List<SelectListItem> SyarikatList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {

                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahIDList2 = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                wlyhid = getwilyah.GetWilayahID(SyarikatID);

                //sarah comment
                // WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                // WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();

                    //sarah commment
                    //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    //sarah comment
                    //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false ).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();

                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();

                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatList && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    //sarah comment
                    //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    //sarah comment
                    //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var syarikatInfo = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();

                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                //sarah comment
                //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();
            ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
            ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();

            List<sp_RptMakPekTem_Result> resultreport = new List<sp_RptMakPekTem_Result>();

            if (WilayahIDList == 0)
            {
                dbSP.SetCommandTimeout(120);
                resultreport = dbSP.sp_RptMakPekTem(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid, SyarikatList).Where(x => x.fld_LadangID != null).ToList();
                incldg = 1;
            }
            else
            {
                if (LadangIDList == 0)
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptMakPekTem(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid, SyarikatList).Where(x => x.fld_LadangID != null).ToList();
                    incldg = 2;
                }
                else
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptMakPekTem(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid, SyarikatList).Where(x => x.fld_LadangID != null).ToList();
                    incldg = 3;
                }

            }

            switch (Status)
            {
                case 1:
                    activestatus0 = true;
                    break;
                case 2:
                    activestatus1 = true;
                    break;
            }

            List<SelectListItem> status = new List<SelectListItem>();

            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = activestatus0 });

            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2", Selected = activestatus1 });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.YearList = yearlist; // list dalam dropdown
            ViewBag.Year = YearList; // year yg user select
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.SyarikatList = SyarikatList2;
            ViewBag.LadangID = LadangIDList;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.Link = domain;
            ViewBag.Status1 = Status;

            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            return View("LocalWorkerInfo", resultreport);
        }

        ////role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        //public ActionResult LocalWorkerInfo()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    string appname = Request.ApplicationPath;
        //    string domain = Request.Url.GetLeftPart(UriPartial.Authority);

        //    if (appname != "/")
        //    {
        //        domain = domain + appname;
        //    }

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahID;
        //        incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    List<SelectListItem> status = new List<SelectListItem>();

        //    status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = true });

        //    status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2" });

        //    ViewBag.Status = status;
        //    ViewBag.Incldg = incldg;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.Year = year;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.LadangID = 0;
        //    ViewBag.ladangvalue = LadangID;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.Link = domain;
        //    ViewBag.Status1 = 1;

        //    List<sp_RptMakPekTem_Result> resultreport = new List<sp_RptMakPekTem_Result>();

        //    return View("LocalWorkerInfo", resultreport);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        ////role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        //public ActionResult LocalWorkerInfo(int? YearList, int WilayahIDList, int LadangIDList, int Status)
        //{
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    bool activestatus0 = false, activestatus1 = false;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    string appname = Request.ApplicationPath;
        //    string domain = Request.Url.GetLeftPart(UriPartial.Authority);

        //    if (appname != "/")
        //    {
        //        domain = domain + appname;
        //    }

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == YearList)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();

        //    List<sp_RptMakPekTem_Result> resultreport = new List<sp_RptMakPekTem_Result>();

        //    if (WilayahIDList == 0)
        //    {
        //        dbSP.SetCommandTimeout(120);
        //        resultreport = dbSP.sp_RptMakPekTem(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
        //        incldg = 1;
        //    }
        //    else
        //    {
        //        if (LadangIDList == 0)
        //        {
        //            dbSP.SetCommandTimeout(120);
        //            resultreport = dbSP.sp_RptMakPekTem(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
        //            incldg = 2;
        //        }
        //        else
        //        {
        //            dbSP.SetCommandTimeout(120);
        //            resultreport = dbSP.sp_RptMakPekTem(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
        //            incldg = 3;
        //        }

        //    }

        //    switch (Status)
        //    {
        //        case 1:
        //            activestatus0 = true;
        //            break;
        //        case 2:
        //            activestatus1 = true;
        //            break;
        //    }

        //    List<SelectListItem> status = new List<SelectListItem>();

        //    status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = activestatus0 });

        //    status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2", Selected = activestatus1 });

        //    ViewBag.Status = status;
        //    ViewBag.Incldg = incldg;
        //    ViewBag.YearList = yearlist; // list dalam dropdown
        //    ViewBag.Year = YearList; // year yg user select
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.LadangID = LadangIDList;
        //    ViewBag.ladangvalue = LadangID;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.Link = domain;
        //    ViewBag.Status1 = Status;

        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //    return View("LocalWorkerInfo", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransac()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int blgn = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }
        //    var getAllWilayahID = db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).Select(s => s.fld_ID).Distinct().ToList();
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getAllWilayahID = getAllWilayahID;
        //    ViewBag.getflag = getflag;
        //    ViewBag.blgn = blgn;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransac(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 2;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    var getAllWilayahID = db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).Select(s => s.fld_ID).Distinct().ToList();
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = yearlist;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getAllWilayahID = getAllWilayahID;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransacDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int bill, int incldg)
        //{
        //    string monthstring = month.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }
        //    //db3.SetCommandTimeout(120);
        //    var resultreport = db3.sp_RptTransPek(negaraid, syarikatid, wilayahid, ladangid, monthstring, year);
        //    var skbno = db.tbl_Sctran.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_NoSkb != "").Select(s => s.fld_NoSkb.Trim()).Distinct().FirstOrDefault();
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.Bill = bill;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.SkbNo = skbno;
        //    ViewBag.DataCount = db3.sp_RptTransPek(negaraid, syarikatid, wilayahid, ladangid, monthstring, year).Count();

        //    return View("WorkerTransacDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransacSummary()
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransacSummary(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    string monthstring = MonthList.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }
        //    //db3.SetCommandTimeout(120);
        //    ViewBag.SumDbt = db3.sp_RptTransPek(NegaraID, SyarikatID, WilayahIDList, LadangIDList, monthstring, yearlist).Select(s => s.fldDebit).Sum();
        //    ViewBag.SumKrdt = db3.sp_RptTransPek(NegaraID, SyarikatID, WilayahIDList, LadangIDList, monthstring, yearlist).Select(s => s.fldKredit).Sum();

        //    ViewBag.Month = MonthList;
        //    ViewBag.Year = yearlist;
        //    ViewBag.GetAllLdg = LadangIDList2;
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransacSalary()
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransacSalary(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    string monthstring = MonthList.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }

        //    ViewBag.Month = MonthList;
        //    ViewBag.Year = yearlist;
        //    ViewBag.GetAllLdg = LadangIDList2;
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerMyeg()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        //WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //    }

        //    List<SelectListItem> KerakyatanList = new List<SelectListItem>();
        //    KerakyatanList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
        //    KerakyatanList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.GetAllKrytan = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.KerakyatanList = KerakyatanList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.GetFlag = 1;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerMyeg(int PassportMonth, int MonthList, int WilayahIDList, int LadangIDList, string KerakyatanList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //    }

        //    List<SelectListItem> KerakyatanList2 = new List<SelectListItem>();
        //    KerakyatanList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
        //    KerakyatanList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.GetAllKrytan = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();

        //    List<Models.vw_DetailPekerja> resultreport = new List<Models.vw_DetailPekerja>();
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.KerakyatanList = KerakyatanList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.RakyatSelection = KerakyatanList;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.GetFlag = 2;
        //    ViewBag.PassportMonth = PassportMonth;

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerMyegDetail(int radio, int negaraid, int syarikatid, int month, int wilayahid, int ladangid, string kerakyatan, int incldg)
        //{
        //    ViewBag.Month = month;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    if (radio == 0)
        //    {
        //        if (kerakyatan == "ALL")
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt != null && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == "0");
        //            return View("WorkerMyegDetail", resultreport);
        //        }
        //        else
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt != null && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0");
        //            return View("WorkerMyegDetail", resultreport);
        //        }
        //    }
        //    else
        //    {
        //        if (month >= 0)
        //        {
        //            if (kerakyatan == "ALL")
        //            {
        //                var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt == month && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == "0");
        //                return View("WorkerMyegDetail", resultreport);
        //            }
        //            else
        //            {
        //                var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt == month && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0");
        //                return View("WorkerMyegDetail", resultreport);
        //            }
        //        }
        //        else
        //        {
        //            if (kerakyatan == "ALL")
        //            {
        //                var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt <= -1 && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == "0");
        //                return View("WorkerMyegDetail", resultreport);
        //            }
        //            else
        //            {
        //                var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt <= -1 && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0");
        //                return View("WorkerMyegDetail", resultreport);
        //            }
        //        }
        //    }
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerMyegSummary(int radio, int negaraid, int syarikatid, int month, int wilayahid, int ladangid, string kerakyatan, int incldg)
        //{
        //    ViewBag.RadioSelect = radio;
        //    ViewBag.Month = month;
        //    ViewBag.WilayahID = wilayahid;
        //    ViewBag.LadangID = ladangid;

        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();

        //    if (kerakyatan == "ALL")
        //    {
        //        var resultreport = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldOptConfFlag2 != null);
        //        return View("WorkerMyegSummary", resultreport);
        //    }
        //    else
        //    {
        //        var resultreport = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldOptConfFlag2 != null && x.fldOptConfValue == kerakyatan);
        //        return View("WorkerMyegSummary", resultreport);
        //    }
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NoSkb()
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NoSkb(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.GetAllLdg = LadangIDList2;
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NoSkbDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int bill, int incldg)
        //{
        //    string monthstring = month.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }
        //    //db3.SetCommandTimeout(120);
        //    var resultreport = db3.sp_RptTransPek(negaraid, syarikatid, wilayahid, ladangid, monthstring, year).Where(x => x.fldLejar == "452");//db.vw_skb.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid);
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.Bill = bill;
        //    ViewBag.IncLdg = incldg;

        //    return View("NoSkbDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NewWorkerApp()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.StatusApprove = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusapproval2" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;


        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NewWorkerApp(string StatusApprove, int MonthList, int yearlist, int WilayahIDList, int LadangIDList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.StatusApprove = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusapproval2" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();

        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    ViewBag.Status = StatusApprove;
        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NewWorkerAppDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int incldg, string statusapprove)
        //{
        //    ViewBag.IncLdg = incldg;
        //    //ViewBag.Status = statusapprove;
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    int status = int.Parse(statusapprove);

        //    List<Models.tblPkjmastApp> resultreport = new List<Models.tblPkjmastApp>();

        //    if (status == 0)
        //    {
        //        resultreport = db.tblPkjmastApps.Where(x => x.fldNegaraID == negaraid && x.fldSyarikatID == syarikatid && x.fldWilayahID == wilayahid && x.fldLadangID == ladangid && x.fldDateTimeApprove.Value.Month == month && x.fldDateTimeApprove.Value.Year == year).ToList();
        //        ViewBag.DataCount = resultreport.Count();
        //        //return View("NewWorkerAppDetail", resultreport);
        //    }
        //    else if (status == 1)
        //    {
        //        resultreport = db.tblPkjmastApps.Where(x => x.fldNegaraID == negaraid && x.fldSyarikatID == syarikatid && x.fldWilayahID == wilayahid && x.fldLadangID == ladangid && x.fldDateTimeApprove.Value.Month == month && x.fldDateTimeApprove.Value.Year == year && x.fldStatus == 1).ToList();
        //        ViewBag.DataCount = resultreport.Count();
        //        //return View("NewWorkerAppDetail", resultreport);
        //    }
        //    else
        //    {
        //        resultreport = db.tblPkjmastApps.Where(x => x.fldNegaraID == negaraid && x.fldSyarikatID == syarikatid && x.fldWilayahID == wilayahid && x.fldLadangID == ladangid && x.fldDateTimeApprove.Value.Month == month && x.fldDateTimeApprove.Value.Year == year && x.fldStatus == 0).ToList();
        //        ViewBag.DataCount = resultreport.Count();

        //    }
        //    return View("NewWorkerAppDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerReport()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        var ldgID = db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = wlyhID;
        //        ladangselection = ldgID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }
        //    List<SelectListItem> NoPekerjaList = new List<SelectListItem>();
        //    var getpekerja = db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection).Select(s => s.fld_Nopkj).ToArray();
        //    var getpekerja2 = getpekerja.Distinct().ToList();
        //    NoPekerjaList = new SelectList(db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection && getpekerja2.Contains(x.fld_Nopkj.Trim())).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().ToList();
        //    //NoPekerjaList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    //NoPekerjaList= db.vw_KerjaHarian.Where(x => x.fld_WilayahID == wlyhid && x.fld_LadangID == ldgid && x.fld_Tarikh.Month == mont && x.fld_Tarikh.Year == year).Select(s => s.fld_Nopkj.Trim()).Distinct().ToList();

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.NoPekerjaList = NoPekerjaList;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.PekerjaSelection = 0;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;


        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerReport(int MonthList, int yearlist, int WilayahIDList, int LadangIDList, string NoPekerjaList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        //LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        //LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }
        //    List<SelectListItem> NoPekerjaList2 = new List<SelectListItem>();
        //    var getpekerja = db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection).Select(s => s.fld_Nopkj).Distinct().ToArray();
        //    NoPekerjaList2 = new SelectList(db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection && getpekerja.Contains(x.fld_Nopkj.Trim())).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text", NoPekerjaList.Trim()).Distinct().ToList();
        //    //NoPekerjaList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.StatusApprove = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusapproval2" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();

        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.NoPekerjaList = NoPekerjaList2;
        //    ViewBag.PekerjaSelection = NoPekerjaList;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    List<sp_RptLapMakKer_Result> resultreport = new List<sp_RptLapMakKer_Result>();
        //    if (NoPekerjaList == "0")
        //    {
        //        //db3.SetCommandTimeout(120);
        //        resultreport = db3.sp_RptLapMakKer(NegaraID, SyarikatID, WilayahIDList, LadangIDList, MonthList, yearlist, "0").ToList();
        //        ViewBag.DataCount = resultreport.Count();
        //        ViewBag.TotalUnit = resultreport.Select(s => s.fld_Kti1).Sum();
        //        ViewBag.TotalTask = resultreport.Select(s => s.fld_Kti3).Sum();
        //        ViewBag.TotalKong = resultreport.Select(s => s.fld_Kong).Sum();
        //        ViewBag.TotalKdrot = resultreport.Select(s => s.fld_Kdrot).Sum();
        //        ViewBag.TotalOt = resultreport.Select(s => s.fld_Jamot).Sum();
        //        ViewBag.TotalKuantiti = resultreport.Select(s => s.fld_Qty).Sum();
        //        ViewBag.TotalAmt = resultreport.Select(s => s.fld_Amt).Sum();
        //        ViewBag.TotalJumlah = resultreport.Select(s => s.fld_Jumlah).Sum();
        //    }
        //    else
        //    {
        //        //db3.SetCommandTimeout(120);
        //        resultreport = db3.sp_RptLapMakKer(NegaraID, SyarikatID, WilayahIDList, LadangIDList, MonthList, yearlist, NoPekerjaList).ToList();
        //        ViewBag.DataCount = resultreport.Count();
        //        ViewBag.TotalUnit = resultreport.Select(s => s.fld_Kti1).Sum();
        //        ViewBag.TotalTask = resultreport.Select(s => s.fld_Kti3).Sum();
        //        ViewBag.TotalKong = resultreport.Select(s => s.fld_Kong).Sum();
        //        ViewBag.TotalKdrot = resultreport.Select(s => s.fld_Kdrot).Sum();
        //        ViewBag.TotalOt = resultreport.Select(s => s.fld_Jamot).Sum();
        //        ViewBag.TotalKuantiti = resultreport.Select(s => s.fld_Qty).Sum();
        //        ViewBag.TotalAmt = resultreport.Select(s => s.fld_Amt).Sum();
        //        ViewBag.TotalJumlah = resultreport.Select(s => s.fld_Jumlah).Sum();
        //    }
        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerReportDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int incldg, string nopkj)
        //{
        //    ViewBag.IncLdg = incldg;
        //    //ViewBag.Status = statusapprove;
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NoPekerja = nopkj;

        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();

        //    List<sp_RptLapMakKer_Result> resultreport = new List<sp_RptLapMakKer_Result>();
        //    //db3.SetCommandTimeout(120);
        //    resultreport = db3.sp_RptLapMakKer(negaraid, syarikatid, wilayahid, ladangid, month, year, nopkj).ToList();
        //    ViewBag.DataCount = resultreport.Count();
        //    ViewBag.Kump = resultreport.Select(s => s.fld_Kum).Take(1).FirstOrDefault();
        //    ViewBag.NamaPekerja = resultreport.Select(s => s.fld_Nama1).Take(1).FirstOrDefault();
        //    ViewBag.NegaraID = negaraid;
        //    ViewBag.SyarikatID = syarikatid;
        //    ViewBag.WilayahID = wilayahid;
        //    ViewBag.LadangID = ladangid;
        //    return View("WorkerReportDetail", resultreport);
        //}

        //public ActionResult WorkerReportSummary(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int incldg, string nopkj, int bill)
        //{
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.NoPekerja = nopkj;

        //    List<sp_RptLapMakKer_Result> resultreport = new List<sp_RptLapMakKer_Result>();
        //    //db3.SetCommandTimeout(120);
        //    resultreport = db3.sp_RptLapMakKer(negaraid, syarikatid, wilayahid, ladangid, month, year, nopkj).ToList();
        //    ViewBag.DataCount = resultreport.Count();
        //    ViewBag.Kump = resultreport.Select(s => s.fld_Kum).FirstOrDefault();
        //    ViewBag.NamaPekerja = resultreport.Select(s => s.fld_Nama1).FirstOrDefault();
        //    //ViewBag.TotalUnit = resultreport.Select(s => s.fld_Kti1).Sum();
        //    //ViewBag.TotalTask = resultreport.Select(s => s.fld_Kti3).Sum();
        //    //ViewBag.TotalKong = resultreport.Select(s => s.fld_Kong).Sum();
        //    //ViewBag.TotalKdrot = resultreport.Select(s => s.fld_Kdrot).Sum();
        //    //ViewBag.TotalOt = resultreport.Select(s => s.fld_Jamot).Sum();
        //    //ViewBag.TotalKuantiti = resultreport.Select(s => s.fld_Qty).Sum();
        //    //ViewBag.TotalAmt = resultreport.Select(s => s.fld_Amt).Sum();
        //    ViewBag.Bill = bill;
        //    ViewBag.TotalJumlah = resultreport.Select(s => s.fld_Jumlah).Sum();

        //    return View("WorkerReportSummary", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult Paysheet()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult Paysheet(int MonthList, int yearlist, int WilayahIDList, int LadangIDList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult PaysheetDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int incldg)
        //{
        //    decimal? JumlahGajiKasar = 0;
        //    decimal? JumlahKwspMajikan = 0;
        //    decimal? JumlahSocsoMajikan = 0;
        //    decimal? JumlahPendapatanSumbangan = 0;
        //    decimal? JumlahKwspMajikanPekerja = 0;
        //    decimal? JumlahSocsoMajikanPekerja = 0;
        //    decimal? JUmlahPotongan = 0;
        //    decimal? JumlahBersih = 0;

        //    var resultreport = db.vw_GajiBulanan.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Month == month && x.fld_Year == year);
        //    ViewBag.DataCount = resultreport.Count();
        //    if (ViewBag.DataCount >= 1)
        //    {
        //        JumlahGajiKasar = resultreport.Sum(s => s.fld_Gaji_Kasar);
        //        JumlahKwspMajikan = resultreport.Sum(s => s.fld_Epf_Mjk);
        //        JumlahSocsoMajikan = resultreport.Sum(s => s.fld_Socso_Mjk);
        //        JumlahPendapatanSumbangan = (JumlahGajiKasar) + (JumlahKwspMajikan + JumlahSocsoMajikan);
        //        JumlahKwspMajikanPekerja = (JumlahKwspMajikan) + (resultreport.Sum(s => s.fld_Epf_Pkj));
        //        JumlahSocsoMajikanPekerja = (JumlahSocsoMajikan) + (resultreport.Sum(s => s.fld_Socso_Pkj));
        //        JUmlahPotongan = JumlahKwspMajikanPekerja + JumlahSocsoMajikanPekerja;
        //        JumlahBersih = JumlahPendapatanSumbangan + JUmlahPotongan;
        //    }

        //    ViewBag.JumlahGajiKasar = GetTriager.GetTotalForMoney(JumlahGajiKasar);
        //    ViewBag.JumlahKwspMajikan = GetTriager.GetTotalForMoney(JumlahKwspMajikan);
        //    ViewBag.JumlahSocsoMajikan = GetTriager.GetTotalForMoney(JumlahSocsoMajikan);
        //    ViewBag.JumlahPendapatanSumbangan = GetTriager.GetTotalForMoney(JumlahPendapatanSumbangan);
        //    ViewBag.JumlahKwspMajikanPekerja = GetTriager.GetTotalForMoney(JumlahKwspMajikanPekerja);
        //    ViewBag.JumlahSocsoMajikanPekerja = GetTriager.GetTotalForMoney(JumlahSocsoMajikanPekerja);
        //    ViewBag.JUmlahPotongan = GetTriager.GetTotalForMoney(JUmlahPotongan);
        //    ViewBag.JumlahBersih = GetTriager.GetTotalForMoney(JumlahBersih);

        //    ViewBag.IncLdg = incldg;
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();

        //    return View("PaysheetDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerList()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    string statusselection = " ";
        //    string pekerjaselection = " ";

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //    }

        //    List<SelectListItem> StatusList = new List<SelectListItem>();
        //    StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", 1).ToList();
        //    StatusList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    statusselection = "0";

        //    List<SelectListItem> NoPekerjaList = new List<SelectListItem>();
        //    NoPekerjaList = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
        //    NoPekerjaList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    pekerjaselection = "0";
        //    //var getpekerja = db.vw_KerjaHarian.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection).Select(s => s.fld_Nopkj.Trim()).Distinct().ToArray();
        //    //NoPekerjaList = new SelectList(db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection && getpekerja.Contains(x.fld_Nopkj.Trim())).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().ToList();
        //    //NoPekerjaList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

        //    //ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    //ViewBag.GetAllKrytan = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.StatusList = StatusList;
        //    ViewBag.NoPekerjaList = NoPekerjaList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.StatusSelection = statusselection;
        //    ViewBag.PekerjaSelection = pekerjaselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.GetFlag = 1;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerList(string StatusList, int WilayahIDList, int LadangIDList, string NoPekerjaList, string PekerjaSearch)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    string statusselection = " ";
        //    string pekerjaselection = " ";

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //    }

        //    List<SelectListItem> StatusList2 = new List<SelectListItem>();
        //    StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
        //    StatusList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    statusselection = StatusList;

        //    List<SelectListItem> NoPekerjaList2 = new List<SelectListItem>();
        //    if (wilayahselection == 0)
        //    {
        //        NoPekerjaList2 = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
        //        NoPekerjaList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        pekerjaselection = NoPekerjaList;
        //    }
        //    else if (wilayahselection != 0 && ladangselection == 0)
        //    {
        //        NoPekerjaList2 = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
        //        NoPekerjaList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        pekerjaselection = NoPekerjaList;
        //    }
        //    else
        //    {
        //        NoPekerjaList2 = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
        //        NoPekerjaList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        pekerjaselection = NoPekerjaList;
        //    }


        //    //List<SelectListItem> KerakyatanList2 = new List<SelectListItem>();
        //    //KerakyatanList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
        //    //KerakyatanList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

        //    //ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    //ViewBag.GetAllKrytan = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();

        //    //List<vw_DetailPekerja> resultreport = new List<vw_DetailPekerja>();
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.StatusList = StatusList2;
        //    ViewBag.NoPekerjaList = NoPekerjaList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.StatusSelection = statusselection;
        //    ViewBag.PekerjaSelection = pekerjaselection;
        //    ViewBag.Search = PekerjaSearch;
        //    //ViewBag.RakyatSelection = KerakyatanList;
        //    ViewBag.Year = year;
        //    //ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.GetFlag = 2;

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerListDetail(int negaraid, int syarikatid, string statuslist, int wilayahid, int ladangid, string nopekerja, int incldg, string searchpekerja)
        //{
        //    //ViewBag.Month = month;
        //    string status = "";
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    switch (statuslist)
        //    {
        //        case "0":
        //            status = " ";
        //            break;
        //        case "1":
        //            status = "0";
        //            break;
        //        case "2":
        //            status = "1";
        //            break;
        //    }
        //    if (searchpekerja == "")
        //    {
        //        if (statuslist == "0" && nopekerja == "0")
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid);
        //            ViewBag.DataCount = resultreport.Count();
        //            return View("WorkerListDetail", resultreport);
        //        }
        //        else if (statuslist == "0" && nopekerja != "0")
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Nopkj == nopekerja);
        //            ViewBag.DataCount = resultreport.Count();
        //            return View("WorkerListDetail", resultreport);
        //        }
        //        else if (statuslist != "0" && nopekerja == "0")
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == status);
        //            ViewBag.DataCount = resultreport.Count();
        //            return View("WorkerListDetail", resultreport);
        //        }
        //        else
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Nopkj == nopekerja && x.fld_Kdaktf == status);
        //            ViewBag.DataCount = resultreport.Count();
        //            return View("WorkerListDetail", resultreport);
        //        }
        //    }
        //    else
        //    {
        //        if (statuslist == "0")
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid);
        //            var result = resultreport.Where(x => x.fld_Nopkj.Contains(searchpekerja) || x.fld_Nama1.Contains(searchpekerja));
        //            ViewBag.DataCount = result.Count();
        //            return View("WorkerListDetail", result);
        //        }
        //        else
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == status);
        //            var result = resultreport.Where(x => x.fld_Nopkj.Contains(searchpekerja) || x.fld_Nama1.Contains(searchpekerja));
        //            ViewBag.DataCount = result.Count();
        //            return View("WorkerListDetail", result);
        //        }


        //        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid);
        //        //var result = resultreport.Where(x =>x.fld_Nopkj.Contains(searchpekerja) || x.fld_Nama1.Contains(searchpekerja));
        //        //ViewBag.DataCount = result.Count();
        //        //return View("WorkerListDetail", result);
        //    }
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult PaysheetByLadang()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult PaysheetByLadang(int MonthList, int yearlist, int WilayahIDList, int LadangIDList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult PaysheetByLadangDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int incldg, int bill)
        //{
        //    string monthstring = month.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }

        //    string namaldg = db.tbl_Ladang.Where(x => x.fld_ID == ladangid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    var result = db.tbl_Sctran.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid);
        //    decimal? GajiKasar = result.Where(x => x.fld_KdCaj == "D").Select(s => s.fld_Amt).Sum();
        //    decimal? kwsp = result.Where(x => x.fld_Lejar == "401").Select(s => s.fld_Amt).Sum();
        //    decimal? kwspP = result.Where(x => x.fld_Akt == "2101").Select(s => s.fld_Amt).Sum();
        //    decimal? socso = result.Where(x => x.fld_Lejar == "402").Select(s => s.fld_Amt).Sum();
        //    decimal? socsoP = result.Where(x => x.fld_Akt == "2102").Select(s => s.fld_Amt).Sum();

        //    decimal? pendahuluanTunai = result.Where(x => x.fld_Lejar == "426").Select(s => s.fld_Amt).Sum();
        //    decimal? lain2 = result.Where(x => x.fld_Lejar == "440").Select(s => s.fld_Amt).Sum();
        //    decimal? Levi = result.Where(x => x.fld_Lejar == "051").Select(s => s.fld_Amt).Sum();
        //    decimal? elektrik = result.Where(x => x.fld_Lejar == "601" && x.fld_Akt == "2601").Select(s => s.fld_Amt).Sum();
        //    decimal? air = result.Where(x => x.fld_Lejar == "601" && x.fld_Akt == "2602").Select(s => s.fld_Amt).Sum();
        //    decimal? insurans = result.Where(x => x.fld_Lejar == "405").Select(s => s.fld_Amt).Sum();
        //    decimal? lain = GetTriager.GetTotalForMoney2(elektrik) + GetTriager.GetTotalForMoney2(air) + GetTriager.GetTotalForMoney2(pendahuluanTunai) + GetTriager.GetTotalForMoney2(lain2) + GetTriager.GetTotalForMoney2(Levi) + GetTriager.GetTotalForMoney2(insurans);

        //    decimal? GajiBersih = result.Where(x => x.fld_Lejar == "452").Select(s => s.fld_Amt).Sum();
        //    decimal? kwspsocsoP = GetTriager.GetTotalForMoney2(kwspP) + GetTriager.GetTotalForMoney2(socsoP);
        //    decimal? kwspsocsoM = GetTriager.GetTotalForMoney2(kwsp) + GetTriager.GetTotalForMoney2(socso) - GetTriager.GetTotalForMoney2(kwspsocsoP);

        //    DateTime? tarikh = result.Select(s => s.fld_Tarikh).Distinct().FirstOrDefault();
        //    string skb = result.Where(x => x.fld_Lejar == "452").Select(s => s.fld_NoSkb).Distinct().FirstOrDefault();

        //    ViewBag.NamaLadang = namaldg;
        //    ViewBag.GajiKasar = GajiKasar;
        //    ViewBag.KWSP = kwspP;
        //    ViewBag.SOCSO = socsoP;
        //    ViewBag.Lain = lain;
        //    ViewBag.GajiBersih = GajiBersih;
        //    ViewBag.Bill = bill;
        //    ViewBag.Date = tarikh;
        //    ViewBag.Skb = skb;
        //    ViewBag.KwspSocsoP = kwspsocsoP;
        //    ViewBag.KwspSocsoM = kwspsocsoM;


        //    var resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_TerimaHQ_Status == 1);
        //    decimal? manual = resultreport.Select(x => x.fld_JumlahManual).FirstOrDefault();
        //    decimal? CheckrolPekerja = GetTriager.GetTotalForMoney2(kwspP) + GetTriager.GetTotalForMoney2(socsoP) + GetTriager.GetTotalForMoney2(lain) + GetTriager.GetTotalForMoney2(GajiBersih);

        //    ViewBag.CheckrolPekerja = CheckrolPekerja;
        //    ViewBag.DataCount = resultreport.Count();
        //    return View("PaysheetByLadangDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult PaysheetByLadangSummary(int ldgid, int negaraid, int syarikatid, int wilayahid, int month, int year, int incldg)
        //{
        //    string monthstring = month.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }
        //    var result = db.tbl_Sctran.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year);
        //    var resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_TerimaHQ_Status == 1);
        //    if (ldgid == 0)
        //    {
        //        result = db.tbl_Sctran.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wilayahid);
        //        resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_TerimaHQ_Status == 1);
        //    }
        //    else
        //    {
        //        result = db.tbl_Sctran.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ldgid);
        //        resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ldgid && x.fld_TerimaHQ_Status == 1);
        //    }
        //    //var result = db.tbl_Sctran.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wilayahid);
        //    decimal? GajiKasar = result.Where(x => x.fld_KdCaj == "D").Select(s => s.fld_Amt).Sum();
        //    decimal? kwsp = result.Where(x => x.fld_Lejar == "401").Select(s => s.fld_Amt).Sum();
        //    decimal? kwspP = result.Where(x => x.fld_Akt == "2101").Select(s => s.fld_Amt).Sum();
        //    decimal? socso = result.Where(x => x.fld_Lejar == "402").Select(s => s.fld_Amt).Sum();
        //    decimal? socsoP = result.Where(x => x.fld_Akt == "2102").Select(s => s.fld_Amt).Sum();

        //    decimal? pendahuluanTunai = result.Where(x => x.fld_Lejar == "426").Select(s => s.fld_Amt).Sum();
        //    decimal? lain2 = result.Where(x => x.fld_Lejar == "440").Select(s => s.fld_Amt).Sum();
        //    decimal? Levi = result.Where(x => x.fld_Lejar == "051").Select(s => s.fld_Amt).Sum();
        //    decimal? elektrik = result.Where(x => x.fld_Lejar == "601" && x.fld_Akt == "2601").Select(s => s.fld_Amt).Sum();
        //    decimal? air = result.Where(x => x.fld_Lejar == "601" && x.fld_Akt == "2602").Select(s => s.fld_Amt).Sum();
        //    decimal? Insurans = result.Where(x => x.fld_Lejar == "405").Select(s => s.fld_Amt).Sum();
        //    //decimal? lain = elektrik + air;
        //    decimal? lain = GetTriager.GetTotalForMoney2(elektrik) + GetTriager.GetTotalForMoney2(air) + GetTriager.GetTotalForMoney2(pendahuluanTunai) + GetTriager.GetTotalForMoney2(lain2) + GetTriager.GetTotalForMoney2(Levi) + GetTriager.GetTotalForMoney2(Insurans);
        //    decimal? GajiBersih = result.Where(x => x.fld_Lejar == "452").Select(s => s.fld_Amt).Sum();
        //    decimal? kwspsocsoP = GetTriager.GetTotalForMoney2(kwspP) + GetTriager.GetTotalForMoney2(socsoP);
        //    decimal? kwspsocsoM = GetTriager.GetTotalForMoney2(kwsp) + GetTriager.GetTotalForMoney2(socso) - GetTriager.GetTotalForMoney2(kwspsocsoP);

        //    ViewBag.GajiKasar = GajiKasar;
        //    ViewBag.KWSP = kwspP;
        //    ViewBag.SOCSO = socsoP;
        //    ViewBag.Lain = lain;
        //    ViewBag.GajiBersih = GajiBersih;
        //    ViewBag.KwspSocsoP = kwspsocsoP;
        //    ViewBag.KwspSocsoM = kwspsocsoM;

        //    //var resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_TerimaHQ_Status == 1);
        //    decimal? manual = resultreport.Select(x => x.fld_JumlahManual).Sum();
        //    decimal? CheckrolPekerja = GetTriager.GetTotalForMoney2(kwspP) + GetTriager.GetTotalForMoney2(socsoP) + GetTriager.GetTotalForMoney2(lain) + GetTriager.GetTotalForMoney2(GajiBersih);
        //    ViewBag.CheckrolPekerja = CheckrolPekerja;
        //    ViewBag.PDP = resultreport.Select(s => s.fld_JumlahPDP).Sum();
        //    ViewBag.TT = resultreport.Select(s => s.fld_JumlahTT).Sum();
        //    ViewBag.CIT = resultreport.Select(s => s.fld_JumlahCIT).Sum();
        //    ViewBag.DataCount = resultreport.Count();
        //    ViewBag.Manual = manual;
        //    return View("PaysheetByLadangSummary", resultreport.Take(1));
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Resource")]
        //public ActionResult ListResumeWorker()
        //{
        //    ViewBag.Report = "class = active";
        //    ViewBag.KumpulanSyarikatList = new SelectList(db.vw_NeragaSumberDetail.Where(x => x.fldUserName == User.Identity.Name).OrderBy(o => o.fld_NamaKmplnSyrkt), "fldKmplnSyrktID", "fld_NamaKmplnSyrkt");
        //    var getSatuSyarikat = db.vw_NeragaSumberDetail.Where(x => x.fldUserName == User.Identity.Name).OrderBy(o => o.fld_NamaKmplnSyrkt).Take(1).Select(s => s.fldKmplnSyrktID).FirstOrDefault();
        //    ViewBag.SyarikatList = new SelectList(db.vw_NSWL.Where(x => x.fld_KmplnSyrktID == getSatuSyarikat && x.fld_Deleted_S == false).Select(s => new { s.fld_SyarikatID, s.fld_NamaSyarikat }).Distinct(), "fld_SyarikatID", "fld_NamaSyarikat");
        //    var getSatuSyarikat2 = db.vw_NSWL.Where(x => x.fld_KmplnSyrktID == getSatuSyarikat && x.fld_Deleted_S == false).Select(s => s.fld_SyarikatID).Take(1).Distinct().FirstOrDefault();
        //    ViewBag.BatchList = new SelectList(db.tblTKABatches.Where(x => x.fldKmplnSyrktID == getSatuSyarikat && x.fldSyrktID == getSatuSyarikat2), "fldID", "fldNoBatch").ToList();
        //    List<vw_TKADetail> TKADetail = new List<vw_TKADetail>();

        //    return View("ListResumeWorker", TKADetail);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Resource")]
        //public ActionResult ListResumeWorker(int BatchList, int KumpulanSyarikatList, int SyarikatList)
        //{
        //    ViewBag.Report = "class = active";
        //    int getuserid = getidentity.ID(User.Identity.Name);

        //    ViewBag.KumpulanSyarikatList = new SelectList(db.vw_NeragaSumberDetail.Where(x => x.fldUserName == User.Identity.Name).OrderBy(o => o.fld_NamaKmplnSyrkt), "fldKmplnSyrktID", "fld_NamaKmplnSyrkt", KumpulanSyarikatList);
        //    var getSatuSyarikat = db.vw_NeragaSumberDetail.Where(x => x.fldUserName == User.Identity.Name).OrderBy(o => o.fld_NamaKmplnSyrkt).Take(1).Select(s => s.fldKmplnSyrktID).FirstOrDefault();
        //    ViewBag.SyarikatList = new SelectList(db.vw_NSWL.Where(x => x.fld_KmplnSyrktID == getSatuSyarikat && x.fld_Deleted_S == false).Select(s => new { s.fld_SyarikatID, s.fld_NamaSyarikat }).Distinct(), "fld_SyarikatID", "fld_NamaSyarikat", SyarikatList);
        //    var getSatuSyarikat2 = db.vw_NSWL.Where(x => x.fld_KmplnSyrktID == getSatuSyarikat && x.fld_Deleted_S == false).Select(s => s.fld_SyarikatID).Take(1).Distinct().FirstOrDefault();
        //    ViewBag.BatchList = new SelectList(db.tblTKABatches.Where(x => x.fldKmplnSyrktID == getSatuSyarikat && x.fldSyrktID == getSatuSyarikat2), "fldID", "fldNoBatch", BatchList).ToList();
        //    ViewBag.NamaSyarikatTo = db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatList).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NamaSyarikatFrom = db2.tblUsers.Where(x => x.fldUserID == getuserid).Select(s => s.fldUserFullName).FirstOrDefault();
        //    List<vw_TKADetail> TKADetail = new List<vw_TKADetail>();
        //    TKADetail = db.vw_TKADetail.Where(x => x.fldTKABatchID == BatchList).ToList();
        //    ViewBag.BatchName = TKADetail.Select(s => s.fldNoBatch).Distinct().FirstOrDefault();
        //    ViewBag.BatchDate = TKADetail.Select(s => s.fldDTCreated).Take(1).FirstOrDefault();

        //    return View("ListResumeWorker", TKADetail);
        //}

        //public ActionResult SysVersion()
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    //ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    //ViewBag.Selectionrpt = selectionrpt;
        //    //ViewBag.IncLdg = incldg;
        //    //ViewBag.UserID = getuserid;
        //    //ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SysVersion(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    //ViewBag.GetAllLdg = LadangIDList2;
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = yearlist;
        //    ViewBag.Month = MonthList;
        //    //ViewBag.Selectionrpt = selectionrpt;
        //    //ViewBag.IncLdg = incldg;
        //    //ViewBag.UserID = getuserid;
        //    //ViewBag.getflag = getflag;
        //    //ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    //ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //    return View();
        //}

        //public ActionResult SysVersionDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year)
        //{
        //    var resultreport = db.tbl_Version.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid);

        //    if (ladangid != 0)
        //    {
        //        resultreport = db.tbl_Version.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid);

        //    }

        //    ViewBag.DataCount = resultreport.Count();
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    //ViewBag.Bill = bill;
        //    //ViewBag.IncLdg = incldg;

        //    return View("SysVersionDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Resource,Viewer")]
        //public ActionResult DataFileUploaded()
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int WilayahIDSelect = 0;

        //    CheckSharedFolder CheckSharedFolder = new CheckSharedFolder();
        //    IOrderedEnumerable<FileInfo> getFiles;
        //    FileInfo[] files = null;
        //    DirectoryInfo salesFTPDirectory = null;
        //    List<FileUploadedData> FileUploadedDatas = new List<FileUploadedData>();
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    string getpathDB = CheckSharedFolder.GetSourceTargetPath("filesourcepath", NegaraID, SyarikatID);
        //    string getpath = getpathDB;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDSelect = int.Parse(WilayahIDList.Take(1).Select(s => s.Value).FirstOrDefault());
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDSelect && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;

        //    salesFTPDirectory = new DirectoryInfo(getpath);
        //    files = salesFTPDirectory.GetFiles();
        //    getFiles = files.Where(f => f.Extension == ".zip").OrderBy(o => o.CreationTime);
        //    foreach (var getFile in getFiles)
        //    {
        //        FileUploadedDatas.Add(new FileUploadedData() { FileName = getFile.Name, DateTimeCreated = getFile.CreationTime, SizeFile = getFile.Length });
        //    }
        //    return View(FileUploadedDatas);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Resource,Viewer")]
        //public ActionResult DataFileUploaded(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    string stringyear = "";
        //    string stringmonth = "";
        //    IOrderedEnumerable<FileInfo> getFiles;
        //    FileInfo[] files = null;
        //    DirectoryInfo salesFTPDirectory = null;
        //    List<FileUploadedData> FileUploadedDatas = new List<FileUploadedData>();
        //    List<FileUploadedDataName> FileUploadedDataNames = new List<FileUploadedDataName>();
        //    FileUploadedDataName FileData = new FileUploadedDataName();
        //    CheckSharedFolder CheckSharedFolder = new CheckSharedFolder();
        //    List<AuthModels.tbl_Ladang> getladangdetails = new List<AuthModels.tbl_Ladang>();
        //    stringyear = yearlist.ToString();
        //    stringyear = stringyear.Substring(2, 2);
        //    stringmonth = MonthList.ToString();
        //    stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    string getpathDB = CheckSharedFolder.GetSourceTargetPath("filesourcepath", NegaraID, SyarikatID);
        //    string getpath = getpathDB + stringmonth + stringyear + "\\";

        //    if (!Directory.Exists(getpath))
        //    {
        //        getpath = getpathDB;
        //    }

        //    ViewBag.Report = "class = active";

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = yearlist;
        //    ViewBag.Month = MonthList;
        //    getladangdetails = LadangIDList == 0 ?
        //    db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).ToList()
        //    :
        //    db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList && x.fld_Deleted == false).ToList();
        //    foreach (var getladangdetail in getladangdetails)
        //    {
        //        FileUploadedDataNames.Add(new FileUploadedDataName() { FileName = getladangdetail.fld_LdgCode + stringmonth + stringyear + ".zip", KodLadang = getladangdetail.fld_LdgCode, NamaLadang = getladangdetail.fld_LdgName });
        //    }
        //    salesFTPDirectory = new DirectoryInfo(getpath);
        //    files = salesFTPDirectory.GetFiles();
        //    getFiles = files.Where(f => f.Extension == ".zip" && FileUploadedDataNames.Select(s => s.FileName).Contains(f.Name)).OrderBy(o => o.Name);
        //    foreach (var getFile in getFiles)
        //    {
        //        FileData = FileUploadedDataNames.Where(x => x.KodLadang == getFile.Name.Substring(0, 3)).FirstOrDefault();
        //        FileUploadedDatas.Add(new FileUploadedData() { FileName = getFile.Name, DateTimeCreated = getFile.CreationTime, SizeFile = getFile.Length, fld_LdgCode = FileData.KodLadang, fld_LdgName = FileData.NamaLadang });
        //    }
        //    ViewBag.FolderSelected = stringmonth + stringyear;
        //    return View(FileUploadedDatas);
        //}

        //public FileResult Download(string file, string from)
        //{
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    DownloadFiles.FileDownloads objs = new DownloadFiles.FileDownloads();
        //    CheckSharedFolder CheckSharedFolder = new CheckSharedFolder();
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    string getpathDB = CheckSharedFolder.GetSourceTargetPath("filesourcepath", NegaraID, SyarikatID);
        //    var filesCol = objs.GetFiles(getpathDB + from + @"\");
        //    var CurrentFileName = filesCol.Where(x => x.FileName == file).FirstOrDefault();

        //    //using (var memoryStream = new MemoryStream())
        //    //{
        //    //    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //    //    {
        //    //        for (int i = 0; i < CurrentFileName.Count; i++)
        //    //        {
        //    //            ziparchive.CreateEntryFromFile(CurrentFileName[i].FilePath, CurrentFileName[i].FileName);
        //    //            //  ziparchive.CreateEntryFromFile(Server.MapPath("~/images/img_Download.PNG"), "img_Download.PNG");
        //    //        }
        //    //    }

        //    //    return File(memoryStream.ToArray(), "application/zip", file);
        //    //}
        //    string contentType = string.Empty;
        //    contentType = "application/pdf";
        //    return File(CurrentFileName.FilePath, contentType, CurrentFileName.FileName);

        //}

        public JsonResult GetPekerja(int WilayahID, int LadangID)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID2 = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID2, getuserid, User.Identity.Name);

            List<SelectListItem> pekerjalist = new List<SelectListItem>();
            var getpekerja = db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj.Trim()).Distinct().ToArray();
            pekerjalist = new SelectList(db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && getpekerja.Contains(x.fld_Nopkj.Trim())).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama.Trim() }), "Value", "Text").Distinct().ToList();

            return Json(pekerjalist);
        }

        public JsonResult GetPekerja2(int WilayahID, int LadangID)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID2 = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID2, getuserid, User.Identity.Name);

            List<SelectListItem> pekerjalist = new List<SelectListItem>();

            //pekerjalist = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().ToList();
            //pekerjalist.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            if (WilayahID == 0)
            {
                pekerjalist = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                pekerjalist = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
            }
            else
            {
                pekerjalist = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
            }

            return Json(pekerjalist);
        }

        //kamalia - 03022021
        [HttpPost]
        public ActionResult ConvertPDF2(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

            tblHtmlReport.fldHtlmCode = myHtml;
            tblHtmlReport.fldFileName = filename;
            tblHtmlReport.fldReportName = reportname;

            db.tblHtmlReports.Add(tblHtmlReport);
            db.SaveChanges();

            success = true;
            status = "success";

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF", "Report", null, "http") + "/" + tblHtmlReport.fldID });
        }

        //kamalia - 03022021
        public ActionResult GetPDF(int id)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string width = "1700", height = "1190";
            string imagepath = Server.MapPath("~/Asset/Images/");

            var gethtml = db.tblHtmlReports.Find(id);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();


            Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
            {
                using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
                {
                    htmlWorker.Open();
                    htmlWorker.Parse(sr);
                }
            }
            pdfDoc.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            db.Entry(gethtml).State = EntityState.Deleted;
            db.SaveChanges();
            return View();
        }

        //[HttpPost]
        //public ActionResult ConvertPDF(string myHtml, string filename, string reportname)
        //{
        //    string linkfile = "";
        //    bool success = false;
        //    string msg = "";
        //    string status = "";
        //    string appname = Request.ApplicationPath;
        //    string domain = Request.Url.GetLeftPart(UriPartial.Authority);

        //    if (appname != "/")
        //    {
        //        domain = domain + appname;
        //    }

        //    linkfile = ConvertToPdf.DownloadAsPDF(myHtml, filename, User.Identity.Name, reportname, domain);

        //    if (linkfile != "")
        //    {
        //        success = true;
        //        status = "success";
        //    }
        //    else
        //    {
        //        success = false;
        //        msg = "Something wrong.";
        //        status = "danger";
        //    }

        //    return Json(new { success = success, id = linkfile, msg = msg, status = status });
        //}

        //[HttpPost]
        //public ActionResult ConvertPDF2(string myHtml, string filename, string reportname)
        //{
        //    bool success = false;
        //    string msg = "";
        //    string status = "";
        //    tblHtmlReport tblHtmlReport = new tblHtmlReport();

        //    tblHtmlReport.fldHtlmCode = myHtml;
        //    tblHtmlReport.fldFileName = filename;
        //    tblHtmlReport.fldReportName = reportname;

        //    db.tblHtmlReports.Add(tblHtmlReport);
        //    db.SaveChanges();

        //    success = true;
        //    status = "success";

        //    return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF", "Report", null, "http") + "/" + tblHtmlReport.fldID });
        //}

        //public ActionResult GetPDF(int id)
        //{
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string width = "", height = "";
        //    string imagepath = Server.MapPath("~/Asset/Images/");

        //    var gethtml = db.tblHtmlReports.Find(id);
        //    var getsize = db.tblReportLists.Where(x => x.fldReportListAction == gethtml.fldReportName.ToString()).FirstOrDefault();
        //    if (getsize != null)
        //    {
        //        width = getsize.fldWidthReport.ToString();
        //        height = getsize.fldHeightReport.ToString();
        //    }
        //    else
        //    {
        //        var getsizesubreport = db.tblSubReportLists.Where(x => x.fldSubReportListAction == gethtml.fldReportName.ToString()).FirstOrDefault();
        //        width = getsizesubreport.fldSubWidthReport.ToString();
        //        height = getsizesubreport.fldSubHeightReport.ToString();
        //    }
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();

        //    //Export HTML String as PDF.
        //    //Image logo = Image.GetInstance(imagepath + logosyarikat);
        //    //Image alignment
        //    //logo.ScaleToFit(50f, 50f);
        //    //logo.Alignment = Image.TEXTWRAP | Image.ALIGN_CENTER;
        //    //StringReader sr = new StringReader(gethtml.fldHtlmCode);
        //    Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);
        //    //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //    pdfDoc.Open();
        //    //pdfDoc.Add(logo);
        //    using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
        //    {
        //        using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
        //        {
        //            htmlWorker.Open();
        //            htmlWorker.Parse(sr);
        //        }
        //    }
        //    pdfDoc.Close();
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdfDoc);
        //    Response.End();

        //    db.Entry(gethtml).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    return View();
        //}

        public JsonResult GetLadang(int WilayahID, string CompCode)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if (getwilyah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db5.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db5.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        public JsonResult GetSubReportList(int ReportList)
        {
            List<SelectListItem> getsubreportlist = new List<SelectListItem>();

            getsubreportlist = new SelectList(dbC.tblSubReportLists.Where(x => x.fldMainReportID == ReportList && x.fldDeleted == false).OrderBy(o => o.fldSubReportListName).Select(s => new SelectListItem { Value = s.fldSubReportListID.ToString(), Text = s.fldSubReportListName }), "Value", "Text").ToList();

            return Json(getsubreportlist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db2.Dispose();
                //db3.Dispose();
                //db4.Dispose();
            }
            base.Dispose(disposing);
        }

        //public ActionResult TransactionListingReport()
        //{
        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    ViewBag.Report = "class = active";
        //    //string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    //MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
        //    int[] wlyhid = new int[] { };
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int month = timezone.gettimezone().Month;

        //    //List<SelectListItem> SelectionList = new List<SelectListItem>();
        //    //SelectionList = new SelectList(
        //    //    dbr.tbl_Pkjmast
        //    //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
        //    //                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
        //    //        .OrderBy(o => o.fld_Nopkj)
        //    //        .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
        //    //    "Value", "Text").ToList();
        //    //SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    //ViewBag.SelectionList = SelectionList;
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //selectionrpt = 1;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //selectionrpt = 2;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //selectionrpt = 3;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 3;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(
        //        db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false),
        //        "fldOptConfValue", "fldOptConfDesc");

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    return View();
        //}

        //public ViewResult _TransactionListingRptSearch(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList)
        //{
        //    if (WilayahIDList == null && LadangIDList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Wilayah Dan Ladang ";
        //        return View();
        //    }
        //    else if (WilayahIDList == 0 && LadangIDList == 0)
        //    {
        //        ViewBag.Message = "Sila Pilih Wilayah Dan Ladang ";
        //        return View();
        //    }
        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value,
        //        NegaraID.Value);
        //    MVC_SYSTEM_ViewingModels dbview = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ViewingModels dbview2 = new MVC_SYSTEM_ViewingModels();
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();

        //    ViewBag.MonthList = MonthList;
        //    ViewBag.YearList = YearList;

        //    ViewBag.NamaSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NamaSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NoSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NoSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NegaraID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.UserName = User.Identity.Name;
        //    ViewBag.Date = DateTime.Now.ToShortDateString();
        //    ViewBag.IDPenyelia = getuserid;


        //    if (MonthList == null && YearList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan Dan Tahun ";
        //        return View();
        //    }

        //    else
        //    {
        //        if (WilayahIDList == 2 && LadangIDList == 0)
        //        {
        //            LadangIDList = 92;
        //        }

        //        var TransactionListingList = dbview.vw_RptSctran
        //            .Where(x => x.fld_KodAktvt != "3803" && x.fld_KodAktvt != "3800" && x.fld_Month == MonthList &&
        //                        x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahIDList &&
        //                        x.fld_LadangID == LadangIDList)
        //            .OrderBy(o => o.fld_Kategori);

        //        if (!TransactionListingList.Any())
        //        {
        //            ViewBag.Message = "Tiada Rekod";
        //            return View();

        //        }
        //        ViewBag.NamaPengurus = dbhq.tbl_Ladang
        //        .Where(x => x.fld_ID == LadangIDList)
        //        .Select(s => s.fld_Pengurus).Single();
        //        ViewBag.NamaPenyelia = dbhq.tblUsers
        //            .Where(x => x.fldUserID == getuserid)
        //            .Select(s => s.fldUserFullName).Single();
        //        return View(TransactionListingList);
        //    }
        //}

        //public ActionResult PaySlipReport()
        //{
        //    ViewBag.Report = "class = active";
        //    //Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    //string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    //MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
        //    int[] wlyhid = new int[] { };
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int month = timezone.gettimezone().Month;

        //    //List<SelectListItem> SelectionList = new List<SelectListItem>();
        //    //SelectionList = new SelectList(
        //    //    dbr.tbl_Pkjmast
        //    //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
        //    //                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
        //    //        .OrderBy(o => o.fld_Nopkj)
        //    //        .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
        //    //    "Value", "Text").ToList();
        //    //SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    //ViewBag.SelectionList = SelectionList;

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //selectionrpt = 1;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //selectionrpt = 2;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //selectionrpt = 3;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 3;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.YearList = yearlist;

        //    //var statusList = new List<SelectListItem>();
        //    //statusList = new SelectList(
        //    //    dbhq.tblOptionConfigsWeb
        //    //        .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false)
        //    //        .OrderBy(o => o.fldOptConfDesc)
        //    //        .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //    //    "Value", "Text").ToList();

        //    var monthList = new SelectList(
        //        dbhq.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false),
        //        "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.MonthList = monthList;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    //ViewBag.StatusList = statusList;

        //    return View();
        //}

        //public ActionResult _WorkerPaySlipRptAdvanceSearch()
        //{
        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_ViewingModels dbview = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();

        //    var statusList = new SelectList(
        //        dbhq.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();

        //    ViewBag.StatusList = statusList;

        //    var workCategoryList = new SelectList(
        //        dbhq.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();

        //    ViewBag.WorkCategoryList = workCategoryList;

        //    return View();
        //}

        //public ViewResult _WorkerPaySlipRptSearch(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList)
        //{
        //    List<vw_PaySlipPekerja> PaySlipPekerja = new List<vw_PaySlipPekerja>();
        //    if (WilayahIDList == null && LadangIDList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySlipPekerja);
        //    }
        //    else if (WilayahIDList == 0 && LadangIDList == 0)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySlipPekerja);
        //    }
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;

        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value,
        //        NegaraID.Value);
        //    MVC_SYSTEM_ViewingModels dbview = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();



        //    ViewBag.MonthList = MonthList;
        //    ViewBag.YearList = YearList;
        //    //ViewBag.WorkerList = SelectionList;
        //    ViewBag.NamaSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NamaSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NoSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NoSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NegaraID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.UserName = User.Identity.Name;
        //    ViewBag.Date = DateTime.Now.ToShortDateString();

        //    if (MonthList == null && YearList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySlipPekerja);
        //    }
        //    else
        //    {
        //        IOrderedQueryable<ViewingModelsOPMS.vw_GajiPekerja> workerData;

        //        if (WilayahIDList == 2 && LadangIDList == 0)
        //        {
        //            LadangIDList = 92;
        //        }

        //        workerData = dbview.vw_GajiPekerja
        //            .Where(x => x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID &&
        //                        x.fld_Year == YearList && x.fld_Month == MonthList &&
        //                        x.fld_SyarikatID == SyarikatID &&
        //                        x.fld_WilayahID == WilayahIDList && x.fld_LadangID == LadangIDList)
        //            .OrderBy(x => x.fld_Nama);

        //        foreach (var i in workerData)
        //        {

        //            List<ViewingModelsOPMS.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModelsOPMS.vw_MaklumatInsentif>();

        //            List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

        //            var workerMonthlySalary = dbview.tbl_GajiBulanan
        //                .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
        //                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
        //                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahIDList &&
        //                            x.fld_LadangID == LadangIDList);

        //            var workerIncentiveRecord = dbview.vw_MaklumatInsentif
        //                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
        //                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
        //                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahIDList &&
        //                            x.fld_LadangID == LadangIDList && x.fld_Deleted == false);

        //            foreach (var insentifRecord in workerIncentiveRecord)
        //            {
        //                workerIncentiveRecordList.Add(insentifRecord);
        //            }

        //            List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

        //            var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
        //               .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList)
        //                .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
        //                .OrderBy(o => o.Key.fld_KodAktvt)
        //                .ThenBy(t => t.Key.fld_KodPkt)
        //                .ThenBy(t2 => t2.Key.fld_Kdhdct)
        //                .Select(lg =>
        //                    new
        //                    {
        //                        fld_ID = lg.FirstOrDefault().fld_ID,
        //                        fld_Desc = lg.FirstOrDefault().fld_Desc,
        //                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
        //                        fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
        //                        fld_Unit = lg.FirstOrDefault().fld_Unit,
        //                        fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
        //                        fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
        //                        fld_TotalAmount = lg.Sum(w => w.fld_Amount)
        //                    });

        //            foreach (var work in workerWorkRecordGroupBy)
        //            {
        //                KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

        //                kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
        //                kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
        //                kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
        //                kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
        //                kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
        //                kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
        //                kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
        //                kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

        //                kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
        //            }

        //            List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

        //            var workerOTRecordGroupBy = dbview.vw_OTPekerja
        //                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList)
        //                .GroupBy(x => x.fld_Kdhdct)
        //                .OrderBy(o => o.Key)
        //                .Select(lg =>
        //                    new
        //                    {
        //                        fld_ID = lg.FirstOrDefault().fld_ID,
        //                        fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
        //                        fld_Desc = lg.FirstOrDefault().fldDesc,
        //                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
        //                        fld_Gandaan = lg.FirstOrDefault().fldRate,
        //                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
        //                    });

        //            foreach (var ot in workerOTRecordGroupBy)
        //            {
        //                OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

        //                otPekerjaCustomModel.fld_ID = ot.fld_ID;
        //                otPekerjaCustomModel.fld_Desc = "Lebih Masa " + ot.fld_Desc;
        //                otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
        //                otPekerjaCustomModel.fld_Unit = "JAM";
        //                otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
        //                otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
        //                otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

        //                otPekerjaCustomModelList.Add(otPekerjaCustomModel);

        //                FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

        //                otFootNoteCustomModel.fld_Desc = "Lebih Masa " + ot.fld_Desc;
        //                otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

        //                footNoteCustomModelList.Add(otFootNoteCustomModel);
        //            }

        //            List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

        //            var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
        //                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList)
        //                .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus })
        //                .OrderBy(o => o.Key.fld_KodPkt)
        //                .ThenBy(t => t.Key.fld_Bonus)
        //                .Select(lg =>
        //                    new
        //                    {
        //                        fld_ID = lg.FirstOrDefault().fld_ID,
        //                        fld_Desc = lg.FirstOrDefault().fld_Desc,
        //                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
        //                        fld_BilanganHari = lg.Count(),
        //                        fld_Bonus = lg.FirstOrDefault().fld_Bonus,
        //                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
        //                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
        //                    });

        //            foreach (var ot in workerBonusRecordGroupBy)
        //            {
        //                BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

        //                bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
        //                bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
        //                bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
        //                bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
        //                bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
        //                bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
        //                bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

        //                bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
        //            }

        //            List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

        //            var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
        //                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList)
        //                .GroupBy(x => new { x.fld_Kdhdct })
        //                .OrderBy(o => o.Key.fld_Kdhdct)
        //                .Select(lg =>
        //                    new
        //                    {
        //                        fld_ID = lg.FirstOrDefault().fld_ID,
        //                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
        //                        fld_BilanganHari = lg.Count(),
        //                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
        //                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
        //                    });

        //            foreach (var ot in workerLeaveRecordGroupBy)
        //            {
        //                CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

        //                cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
        //                cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
        //                cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
        //                cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
        //                cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

        //                cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
        //            }

        //            var workerWorkingDay = dbview.vw_KehadiranPekerja
        //                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList)
        //                .GroupBy(x => new { x.fld_Kdhdct })
        //                .OrderBy(o => o.Key.fld_Kdhdct)
        //                .Select(lg =>
        //                    new
        //                    {
        //                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
        //                        fld_Bilangan = lg.Count(),
        //                    });

        //            foreach (var workingDay in workerWorkingDay)
        //            {
        //                FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

        //                footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
        //                footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

        //                footNoteCustomModelList.Add(footNoteCustomModel);
        //            }

        //            var workerRainDay = dbview.vw_KehadiranPekerja
        //                .Count(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList && x.fld_Hujan == 1);

        //            if (workerRainDay != 0)
        //            {
        //                FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

        //                footNoteHariHujanCustomModel.fld_Desc = "Jumlah Hari Hujan";
        //                footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

        //                footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
        //            }

        //            PaySlipPekerja.Add(
        //                new vw_PaySlipPekerja()
        //                {
        //                    Pkjmast = i,
        //                    GajiBulanan = workerMonthlySalary,
        //                    InsentifPekerja = workerIncentiveRecordList,
        //                    KerjaPekerja = kerjaPekerjaCustomModelList,
        //                    OTPekerja = otPekerjaCustomModelList,
        //                    BonusPekerja = bonusPekerjaCustomModelList,
        //                    CutiPekerja = cutiPekerjaCustomModelList,
        //                    FootNote = footNoteCustomModelList
        //                });
        //        }

        //        if (PaySlipPekerja.Count == 0)
        //        {
        //            ViewBag.Message = "Tiada Rekod";
        //        }

        //        return View(PaySlipPekerja);
        //    }
        //}





        public JsonResult GetList(int wlyh, int ldg, int RadioGroup, string StatusList)
        {
            Connection Connection = new Connection();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            string SelectionLabel = "";

            if (RadioGroup == 0)
            {
                if (String.IsNullOrEmpty(StatusList))
                {
                    //Individu Semua
                    SelectionLabel = "Pekerja";

                    SelectionList = new SelectList(
                        dbr.tbl_Pkjmast
                            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                        x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                            .OrderBy(o => o.fld_Nopkj)
                            .Select(
                                s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                        "Value", "Text").ToList();
                    SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                }

                else
                {
                    //Individu Semua
                    SelectionLabel = "Pekerja";
                    if (StatusList == "0")
                    {
                        SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                    }
                    else
                    {
                        if (ldg == 0)
                        {
                            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                            SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                        }
                        else
                        {
                            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg && x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                            SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                        }

                    }
                }

            }
            else
            {
                //Group
                if (ldg == 0)
                {
                    SelectionLabel = "Kumpulan";
                    SelectionList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
                    SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                }
                else
                {
                    SelectionLabel = "Kumpulan";
                    SelectionList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
                    SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                }

            }
            return Json(new { SelectionList = SelectionList, SelectionLabel = SelectionLabel });
        }


        //added by faeza 24.11.2021
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ActionResult PaymentModeReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            int[] wlyhid = new int[] { };
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;

            List<SelectListItem> CompCodeList = new List<SelectListItem>();
            List<SelectListItem> wilayahList = new List<SelectListItem>();
            List<SelectListItem> ladangList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                CompCodeList = new SelectList(db2.tblOptionConfigsWeb
                        .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                        .OrderBy(o => o.fldOptConfDesc)
                        .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                    "Value", "Text").ToList();
                //CompCodeList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                wilayahList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                wilayahList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                ladangList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                ladangList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                CompCodeList = new SelectList(db2.tblOptionConfigsWeb
                .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .OrderBy(o => o.fldOptConfDesc)
                .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
                //CompCodeList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                wilayahList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                wilayahList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                incldg = 1;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                CompCodeList = new SelectList(db2.tblOptionConfigsWeb
                .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .OrderBy(o => o.fldOptConfDesc)
                .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                wilayahList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                ladangList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 1;
            }

            ViewBag.CompCodeList = CompCodeList;
            ViewBag.WilayahList = wilayahList;
            ViewBag.LadangList = ladangList;

            //List<SelectListItem> wilayahList = new List<SelectListItem>();
            //List<SelectListItem> ladangList = new List<SelectListItem>(); //fatin added - 17/04/2023

            ////comment by fatin - 17/04/2023
            ///*wilayahList = new SelectList(
            //    db.tbl_Wilayah
            //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
            //        .OrderBy(o => o.fld_WlyhName)
            //        .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }),
            //    "Value", "Text").ToList();*/

            ////fatin added - 17/04/2023
            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wilayahList = new SelectList(
            //    db.tbl_Wilayah
            //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
            //        .Select(
            //            s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            //    wilayahList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            //    ladangList = new SelectList(
            //    db.tbl_Ladang
            //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_CostCentre == "1000")
            //        .OrderBy(o => o.fld_LdgName)
            //        .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text").ToList();
            //}

            //else
            //{
            //    wilayahList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName").ToList();
            //    ladangList = new SelectList(db.tbl_Ladang
            //       .Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_CostCentre == "1000")
            //       .OrderBy(o => o.fld_LdgName)
            //       .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //}
            ////end

            //ViewBag.WilayahList = wilayahList;

            //ladangList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            //ViewBag.LadangList = ladangList;

            ////comment by fatin - 17/04/2023
            ////List<SelectListItem> ladangList = new List<SelectListItem>();

            ///*ladangList = new SelectList(
            //    db.tbl_Ladang
            //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_CostCentre=="1000")
            //        .OrderBy(o => o.fld_LdgName)
            //        .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }),
            //    "Value", "Text").ToList();*/


            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            int month = timezone.gettimezone().Month;

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month - 1);

            return View();
        }

        //added by faeza 24.11.2021
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ViewResult _PaymentModeReport(int? WilayahList, int? LadangList, int? MonthList, int? YearList, string CompCodeList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<sp_PaymentModeReport_Result> rptPaymentMode = new List<sp_PaymentModeReport_Result>();

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;
            ViewBag.YearList = YearList;
            ViewBag.MonthList = MonthList;

            // Added by Shazana 3 / 4 / 2023
            String namabulan = db.tblOptionConfigsWeb.Where(x => x.fldOptConfValue == MonthList.ToString()).Select(x => x.fldOptConfFlag2).FirstOrDefault();
            if (namabulan != null)
            { ViewBag.namabulan = namabulan.ToUpper(); }
            else
            { ViewBag.namabulan = ""; }
            ViewBag.WilayahList = WilayahList;
            ViewBag.LadangList = LadangList;
            try
            {

                if (String.IsNullOrEmpty(MonthList.ToString()) || String.IsNullOrEmpty(YearList.ToString()) || String.IsNullOrEmpty(WilayahList.ToString()))
                {
                    ViewBag.Message = @GlobalResCorp.lblChooseWorkerMasterDataReport;
                }
                else
                {
                    if (WilayahList == 0)
                    {
                        if (LadangList == 0)
                        {
                            dbSP.SetCommandTimeout(600);
                            rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                            .Where(x => x.fld_Month == MonthList &&
                                    x.fld_Year == YearList).OrderBy(o => o.fld_WilayahID).ToList();
                        }
                        else
                        {
                            dbSP.SetCommandTimeout(600);
                            rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                            .Where(x => x.fld_Month == MonthList &&
                                    x.fld_Year == YearList && x.fld_LadangID == LadangList).OrderBy(o => o.fld_WilayahID).ToList();
                        }
                    }
                    else
                    {
                        if (LadangList == 0)
                        {
                            dbSP.SetCommandTimeout(600);
                            rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                            .Where(x => x.fld_Month == MonthList &&
                                    x.fld_Year == YearList && x.fld_WilayahID == WilayahList).OrderBy(o => o.fld_WilayahID).ToList();
                            //added by kamalia 24/12/21
                            ViewBag.Tajuk = db.tbl_Wilayah
                            .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == WilayahList)
                               .Select(s => "Wilayah " + s.fld_WlyhName.Substring(0, 1).ToUpper() + s.fld_WlyhName.Substring(1).ToLower()).FirstOrDefault();
                        }
                        else
                        {
                            dbSP.SetCommandTimeout(600);
                            rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                            .Where(x => x.fld_Month == MonthList &&
                                    x.fld_Year == YearList && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList).OrderBy(o => o.fld_WilayahID).ToList();
                            //added by kamalia 24/12/21
                            ViewBag.Tajuk = db.tbl_Ladang
                  .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangList)
                  .Select(s => s.fld_LdgName.Substring(0, 1).ToUpper() + s.fld_LdgName.Substring(1).ToLower())
                  .FirstOrDefault();
                        }

                    }

                    if (rptPaymentMode.Count == 0)
                    {
                        ViewBag.Message = @GlobalResCorp.msgNoRecord;
                    }
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            return View(rptPaymentMode);
        }

        // yana added 010623
        public JsonResult GetLadang2(int WilayahID, string SyarikatList)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if (getwilyah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db5.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db5.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        //Added by Shazana 1/8/2023
        public JsonResult GetWilayah4(string SyarikatID)
        {
            List<SelectListItem> wilayahlist = new List<SelectListItem>();
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var syarikatCodeId = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fldOptConfValue == SyarikatID.ToString() && x.fld_NegaraID == NegaraID).Select(x => x.fld_SyarikatID).FirstOrDefault();
            int SyarikatCode = Convert.ToInt16(syarikatCodeId);

            if (getwilyah.GetAvailableWilayah(SyarikatCode))
            {
                if (WilayahID == 0)
                {
                    //dapatkan ladang filter by costcenter
                    var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatID && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                    var listwilayah1 = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                    wilayahlist = new SelectList(listwilayah1.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    wilayahlist.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                    ladanglist.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                }
                else
                {
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID2 && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    ladanglist.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                }
            }

            return Json(wilayahlist);
        }

        //sarah added
        public JsonResult GetWilayah5(string SyarikatID)
        {
            List<SelectListItem> wilayahlist = new List<SelectListItem>();
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var syarikatCodeId = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fldOptConfValue == SyarikatID.ToString() && x.fld_NegaraID == NegaraID).Select(x => x.fld_SyarikatID).FirstOrDefault();
            int SyarikatCode = Convert.ToInt16(syarikatCodeId);

            if (getwilyah.GetAvailableWilayah(SyarikatCode))
            {
                if (WilayahID == 0)
                {
                    //dapatkan ladang filter by costcenter
                    var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatID && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                    var listwilayah1 = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                    wilayahlist = new SelectList(listwilayah1.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    wilayahlist.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "0" });
                    ladanglist.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                }
                else
                {
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID2 && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    ladanglist.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                }
            }

            return Json(wilayahlist);
        }

        //Added by Shazana 1/8/2023
        public JsonResult GetLadang4(int WilayahID, string SyarikatID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID2, out LadangID, getuserid, User.Identity.Name);


            if (getwilyah.GetAvailableWilayah(SyarikatID2))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_CostCentre == "0" && x.fld_Deleted_L == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList(); //modified by kamalia 1/2/2022
                }
                else
                {
                    ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_CostCentre == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList(); //modified by kamalia 1/2/2022
                }
            }

            return Json(ladanglist);
        }

        //Added by Shazana 3/4/2023
        public FileStreamResult exportPDF(int? WilayahList, int? LadangList, int? MonthList, int? YearList, string CompCodeList, string print)
        {
            ChangeTimeZone ChangeTimeZone = new ChangeTimeZone();
            DateTime Todaydate = ChangeTimeZone.gettimezone();
            string uniquefilename = "LaporanBayaranGajiPekerja" + "_" + Todaydate.Year.ToString() + Todaydate.Month.ToString() + Todaydate.Day.ToString() + Todaydate.Hour.ToString() + Todaydate.Minute.ToString() + Todaydate.Second.ToString();

            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 6, 5);
            MemoryStream ms = new MemoryStream();
            MemoryStream output = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
            Chunk chunk = new Chunk();
            Paragraph para = new Paragraph();

            pdfDoc.Open();

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //MVC_SYSTEM_MasterModels MasterModel = new MVC_SYSTEM_MasterModels();
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            int? bil1 = 0;
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var message = "";
            var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            String namabulan = db.tblOptionConfigsWeb.Where(x => x.fldOptConfValue == MonthList.ToString()).Select(x => x.fldOptConfFlag2.ToUpper()).FirstOrDefault();

            List<sp_PaymentModeReport_Result> rptPaymentMode = new List<sp_PaymentModeReport_Result>();
            if (WilayahList == 0)
            {
                if (LadangList == 0)
                {
                    dbSP.SetCommandTimeout(600);
                    rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                    .Where(x => x.fld_Month == MonthList &&
                            x.fld_Year == YearList).OrderBy(o => o.fld_WilayahID).ToList();
                }
                else
                {
                    dbSP.SetCommandTimeout(600);
                    rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                    .Where(x => x.fld_Month == MonthList &&
                            x.fld_Year == YearList && x.fld_LadangID == LadangList).OrderBy(o => o.fld_WilayahID).ToList();
                }
            }
            else
            {
                if (LadangList == 0)
                {
                    dbSP.SetCommandTimeout(600);
                    rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                    .Where(x => x.fld_Month == MonthList &&
                            x.fld_Year == YearList && x.fld_WilayahID == WilayahList).OrderBy(o => o.fld_WilayahID).ToList();
                    //added by kamalia 24/12/21
                    ViewBag.Tajuk = db.tbl_Wilayah
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == WilayahList)
                       .Select(s => "Wilayah " + s.fld_WlyhName.Substring(0, 1).ToUpper() + s.fld_WlyhName.Substring(1).ToLower()).FirstOrDefault();
                }
                else
                {
                    dbSP.SetCommandTimeout(600);
                    rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                    .Where(x => x.fld_Month == MonthList &&
                            x.fld_Year == YearList && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList).OrderBy(o => o.fld_WilayahID).ToList();
                    //added by kamalia 24/12/21
                    ViewBag.Tajuk = db.tbl_Ladang
          .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangList)
          .Select(s => s.fld_LdgName.Substring(0, 1).ToUpper() + s.fld_LdgName.Substring(1).ToLower())
          .FirstOrDefault();
                }

            }
            int bil = 1;
            decimal? totaleachladang = 0;
            decimal? totallastladang = 0;
            decimal? grandtotal = 0;
            decimal? totalcash = 0;
            decimal? totalcheque = 0;
            decimal? totalcdmas = 0;
            decimal? totalewallet = 0;
            decimal? totalkwsp = 0;
            decimal? totalsocso = 0;
            decimal? totalsbkp = 0;
            decimal? totalsip = 0;
            decimal? totallainpotongan = 0;
            decimal? totalgajibersih = 0;
            int? totalcashpax = 0;
            int? totalchequepax = 0;
            int? totalcdmaspax = 0;
            int? totalewalletpax = 0;


            decimal? grandtotaleachladang = 0;
            decimal? grandtotallastladang = 0;
            decimal? grandgrandtotal = 0;
            decimal? grandtotalcash = 0;
            decimal? grandtotalcheque = 0;
            decimal? grandtotalcdmas = 0;
            decimal? grandtotalewallet = 0;
            decimal? grandtotalkwsp = 0;
            decimal? grandtotalsocso = 0;
            decimal? grandtotalsbkp = 0;
            decimal? grandtotalsip = 0;
            decimal? grandtotallainpotongan = 0;
            decimal? grandtotalgajibersih = 0;
            int? grandtotalcashpax = 0;
            int? grandtotalchequepax = 0;
            int? grandtotalcdmaspax = 0;
            int? grandtotalewalletpax = 0;

            grandtotalcash = 0;
            grandtotalcheque = 0;
            grandtotalcdmas = 0;
            grandtotalewallet = 0;
            grandtotalgajibersih = 0;
            grandtotalkwsp = 0;
            grandtotalsocso = 0;
            grandtotalsbkp = 0;
            grandtotalsip = 0;
            grandtotallainpotongan = 0;
            grandtotalcashpax = 0;
            grandtotalchequepax = 0;
            grandtotalcdmaspax = 0;
            grandtotalewalletpax = 0;

            if (rptPaymentMode.Count() > 0)
            {

                foreach (var wilayah in rptPaymentMode.Select(s => s.fld_WilayahID).Distinct())
                //foreach (var wilayahselec in WilayahSelection)
                {
                    bil = 1;
                    totalcash = 0;
                    totalcheque = 0;
                    totalcdmas = 0;
                    totalewallet = 0;
                    totalgajibersih = 0;
                    totalkwsp = 0;
                    totalsocso = 0;
                    totalsbkp = 0;
                    totalsip = 0;
                    totallainpotongan = 0;
                    totalcashpax = 0;
                    totalchequepax = 0;
                    totalcdmaspax = 0;
                    totalewalletpax = 0;

                    PdfPTable table = new PdfPTable(23);
                    table.WidthPercentage = 100;
                    table.SpacingBefore = 10f;
                    float[] widths = new float[] { 1, 1, 1.5f, 1.5f, 1.2f, 1.2f, 1.5f, 1.5f, 1, 1, 1.2f, 1, 1, 2, 1.5f, 1.5f, 1, 1, 1, 1.5f, 1, 2, 2.2f };
                    table.SetWidths(widths);
                    PdfPCell cell = new PdfPCell();
                    string headertitle = "PERMOHONAN WANG GAJI MAPA & PEKERJA TEMPATAN " + (namabulan == null ? "" : namabulan.ToUpper()) + " " + YearList + "";
                    //var getdata = db.vw_PermohonanKewangan.Where(x => DataID.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
                    //var namawilayah = db.tbl_Wilayah.Where(x => x.fld_ID == wilayahselec).Select(x => x.fld_WlyhName).FirstOrDefault();
                    // if (WilayahSelection.Count() > 0)
                    var namawilayah = db.tbl_Wilayah.Where(x => x.fld_ID == wilayah).Select(x => x.fld_WlyhName).FirstOrDefault();
                    pdfDoc.NewPage();
                    //Header
                    pdfDoc = GeneralClass.Header(pdfDoc, Syarikat.fld_NamaSyarikat, "(" + GlobalResCompanyDetail.lblCompanyNo + " : " + Syarikat.fld_NoSyarikat + ")", headertitle);

                    int i = 1;
                    string KdrGaji = "";
                    decimal Jumlah = 0;
                    GetTriager GetTriager = new GetTriager();

                    chunk = new Chunk("WILAYAH " + namawilayah, FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 26;
                    cell.Border = Rectangle.NO_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(namawilayah, FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 26;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("Bil", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("Kod \r\n Projek", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("Nama \r\n Projek", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)); ;
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("NAMA BANK \r\n PROJEK UNTUK \r\n HANTARAN PDP", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("AKAUN NO. (M2E)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);


                    chunk = new Chunk("MOD BAYARAN", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Colspan = 8;
                    table.AddCell(cell);

                    chunk = new Chunk("JUMLAH GAJI BERSIH(RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("KWSP (RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("SOCSO (RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("SBKP (RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("SIP (RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)); ;
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("POTONGAN LAIN-LAIN (RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("JUMLAH KESELURUHAN (RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);


                    chunk = new Chunk("TARIKH HANTARAN SAP", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("NO. DOKUMEN 34", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("TINDAKAN", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("BIL PEKERJA", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("TUNAI (RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);


                    chunk = new Chunk("BIL PEKERJA", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("CEK (RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("BIL PEKERJA", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("CDMAS (RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("BIL PEKERJA", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("EWALLET (RM)", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);


                    foreach (var paymentModeData in rptPaymentMode.Where(x => x.fld_WilayahID == wilayah).OrderBy(o => o.fld_LdgName))
                    {

                        totaleachladang = paymentModeData.fld_Cash + paymentModeData.fld_Cheque + paymentModeData.fld_Cdmas + paymentModeData.fld_Ewallet;
                        totallastladang = totaleachladang + paymentModeData.fld_Kwsp + paymentModeData.fld_Socso + paymentModeData.fld_Sbkp + paymentModeData.fld_Sip + paymentModeData.fld_LainPotongan;

                        grandtotaleachladang = paymentModeData.fld_Cash + paymentModeData.fld_Cheque + paymentModeData.fld_Cdmas + paymentModeData.fld_Ewallet;
                        grandtotallastladang = grandtotaleachladang + paymentModeData.fld_Kwsp + paymentModeData.fld_Socso + paymentModeData.fld_Sbkp + paymentModeData.fld_Sip + paymentModeData.fld_LainPotongan;

                        chunk = new Chunk(bil.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(paymentModeData.fld_LdgCode.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(paymentModeData.fld_LdgName.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(paymentModeData.fld_BranchName == null ? "" : paymentModeData.fld_BranchName.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(paymentModeData.fld_NoAcc == null ? "" : paymentModeData.fld_NoAcc.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk((paymentModeData.fld_CashPax == null ? "0" : paymentModeData.fld_CashPax.ToString()), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);


                        chunk = new Chunk(paymentModeData.fld_Cash.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk((paymentModeData.fld_ChequePax == null ? "0" : paymentModeData.fld_ChequePax.ToString()), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);


                        chunk = new Chunk(@GetTriager.GetTotalForMoney(paymentModeData.fld_Cheque).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);


                        chunk = new Chunk((paymentModeData.fld_CdmasPax == null ? "0" : paymentModeData.fld_CdmasPax.ToString()), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);


                        chunk = new Chunk(@GetTriager.GetTotalForMoney(paymentModeData.fld_Cdmas).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk((paymentModeData.fld_EwalletPax == null ? "0" : paymentModeData.fld_EwalletPax.ToString()), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);



                        chunk = new Chunk(@GetTriager.GetTotalForMoney(paymentModeData.fld_Ewallet).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);


                        chunk = new Chunk(GetTriager.GetTotalForMoney(totaleachladang).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(GetTriager.GetTotalForMoney(paymentModeData.fld_Kwsp).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);


                        chunk = new Chunk(GetTriager.GetTotalForMoney(paymentModeData.fld_Socso).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(GetTriager.GetTotalForMoney(paymentModeData.fld_Sbkp).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(GetTriager.GetTotalForMoney(paymentModeData.fld_Sip).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(GetTriager.GetTotalForMoney(paymentModeData.fld_LainPotongan).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(GetTriager.GetTotalForMoney(totallastladang).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(paymentModeData.fld_PostingDate.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        string NoDocSAP = "";
                        if (paymentModeData.fld_NoDocSAP != null)
                        {
                            NoDocSAP = paymentModeData.fld_NoDocSAP.Replace("\\r\\n", "\r\n");
                            //kk = string.Join(" ", Regex.Split(paymentModeData.fld_NoDocSAP, @"(?:\r\n|\n|\r)"));
                        }

                        chunk = new Chunk(NoDocSAP, FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);


                        string DisediakanOleh = paymentModeData.fld_Verify_Name == null ? "-" : paymentModeData.fld_Verify_Name.ToString();
                        string DisahkanOleh = paymentModeData.fld_SemakWil_Name == null ? "-" : paymentModeData.fld_SemakWil_Name.ToString();
                        string DisokongOleh = paymentModeData.fld_SokongWilGM_Name == null ? "-" : paymentModeData.fld_SokongWilGM_Name.ToString();
                        string DiluluskanOleh = paymentModeData.fld_TerimaHQ_Name == null ? "-" : paymentModeData.fld_TerimaHQ_Name.ToString();

                        string tindakan = "Disediakan Oleh : " + "\r\n" + DisediakanOleh + "\r\n\r\n" + " Disahkan Oleh : " + "\r\n" + DisahkanOleh + "\r\n\r\n" + " Disokong Oleh : " + "\r\n" + DisokongOleh + "\r\n\r\n" + " Diluluskan Oleh : " + "\r\n" + DiluluskanOleh;

                        chunk = new Chunk(tindakan, FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        totalcash = totalcash + paymentModeData.fld_Cash;
                        totalcheque = totalcheque + paymentModeData.fld_Cheque;
                        totalcdmas = totalcdmas + paymentModeData.fld_Cdmas;
                        totalewallet = totalewallet + paymentModeData.fld_Ewallet;
                        totalkwsp = totalkwsp + paymentModeData.fld_Kwsp;
                        totalsocso = totalsocso + paymentModeData.fld_Socso;
                        totalsbkp = totalsbkp + paymentModeData.fld_Sbkp;
                        totalsip = totalsip + paymentModeData.fld_Sip;
                        totallainpotongan = totallainpotongan + paymentModeData.fld_LainPotongan;


                        paymentModeData.fld_CashPax = paymentModeData.fld_CashPax == null ? 0 : paymentModeData.fld_CashPax;
                        paymentModeData.fld_ChequePax = paymentModeData.fld_ChequePax == null ? 0 : paymentModeData.fld_ChequePax;
                        paymentModeData.fld_CdmasPax = paymentModeData.fld_CdmasPax == null ? 0 : paymentModeData.fld_CdmasPax;
                        paymentModeData.fld_EwalletPax = paymentModeData.fld_EwalletPax == null ? 0 : paymentModeData.fld_EwalletPax;
                        totalcashpax = totalcashpax + paymentModeData.fld_CashPax;
                        totalchequepax = totalchequepax + paymentModeData.fld_ChequePax;
                        totalcdmaspax = totalcdmaspax + paymentModeData.fld_CdmasPax;
                        totalewalletpax = totalewalletpax + paymentModeData.fld_EwalletPax;

                        grandtotalcash = grandtotalcash + paymentModeData.fld_Cash;
                        grandtotalcheque = grandtotalcheque + paymentModeData.fld_Cheque;
                        grandtotalcdmas = grandtotalcdmas + paymentModeData.fld_Cdmas;
                        grandtotalewallet = grandtotalewallet + paymentModeData.fld_Ewallet;
                        grandtotalkwsp = grandtotalkwsp + paymentModeData.fld_Kwsp;
                        grandtotalsocso = grandtotalsocso + paymentModeData.fld_Socso;
                        grandtotalsbkp = grandtotalsbkp + paymentModeData.fld_Sbkp;
                        grandtotalsip = grandtotalsip + paymentModeData.fld_Sip;
                        grandtotallainpotongan = grandtotallainpotongan + paymentModeData.fld_LainPotongan;
                        grandtotalcashpax = grandtotalcashpax + paymentModeData.fld_CashPax;
                        grandtotalchequepax = grandtotalchequepax + paymentModeData.fld_ChequePax;
                        grandtotalcdmaspax = grandtotalcdmaspax + paymentModeData.fld_CdmasPax;
                        grandtotalewalletpax = grandtotalewalletpax + paymentModeData.fld_EwalletPax;

                        bil++;
                    }



                    {
                        totalgajibersih = totalcash + totalcheque + totalcdmas + totalewallet;
                        grandtotal = totalgajibersih + totalkwsp + totalsocso + totalsbkp + totalsip + totallainpotongan;


                        grandtotalgajibersih = grandtotalcash + grandtotalcheque + grandtotalcdmas + grandtotalewallet;
                        grandgrandtotal = grandtotalgajibersih + grandtotalkwsp + grandtotalsocso + grandtotalsbkp + grandtotalsip + grandtotallainpotongan;
                    }


                    //Footer
                    //--------------------------------------------------------------------------
                    chunk = new Chunk("Jumlah ", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Colspan = 5;
                    table.AddCell(cell);

                    chunk = new Chunk(totalcashpax.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(GetTriager.GetTotalForMoney(totalcash).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(totalchequepax.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);


                    chunk = new Chunk(GetTriager.GetTotalForMoney(totalcheque).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);


                    chunk = new Chunk(totalcdmaspax.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(GetTriager.GetTotalForMoney(totalcdmas).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(totalewalletpax.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);


                    chunk = new Chunk(GetTriager.GetTotalForMoney(totalewallet).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(GetTriager.GetTotalForMoney(totalgajibersih).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);


                    chunk = new Chunk(GetTriager.GetTotalForMoney(totalkwsp).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(GetTriager.GetTotalForMoney(totalsocso).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(GetTriager.GetTotalForMoney(totalsbkp).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(GetTriager.GetTotalForMoney(totalsip).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(GetTriager.GetTotalForMoney(totallainpotongan).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotal).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);
                    chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);
                    chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);
                    chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);
                    chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);
                    chunk = new Chunk("".ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    pdfDoc.Add(table);


                }


                PdfPTable tablegrand = new PdfPTable(23);
                tablegrand.WidthPercentage = 100;
                tablegrand.SpacingBefore = 10f;
                float[] widthsgrand = new float[] { 1, 1, 1.5f, 1.5f, 1.2f, 1.2f, 1.5f, 1.5f, 1, 1, 1.2f, 1, 1, 2, 1.5f, 1.5f, 1, 1, 1, 1.5f, 1, 2, 2.2f };
                tablegrand.SetWidths(widthsgrand);
                PdfPCell cellgrand = new PdfPCell();
                //Grand Total
                //--------------------------------------------------------------------------
                chunk = new Chunk("Jumlah Keseluruhan ", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                cellgrand.Colspan = 5;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(grandtotalcashpax.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotalcash).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(grandtotalchequepax.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);


                chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotalcheque).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);


                chunk = new Chunk(grandtotalcdmaspax.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotalcdmas).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(grandtotalewalletpax.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);


                chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotalewallet).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotalgajibersih).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);


                chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotalkwsp).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotalsocso).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotalsbkp).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotalsip).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(GetTriager.GetTotalForMoney(grandtotallainpotongan).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk(GetTriager.GetTotalForMoney(grandgrandtotal).ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);

                chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);
                chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);
                chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                cellgrand = new PdfPCell(new Phrase(chunk));
                cellgrand.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellgrand.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellgrand.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                cellgrand.BorderColor = BaseColor.BLACK;
                tablegrand.AddCell(cellgrand);




                pdfDoc.Add(tablegrand);


            }
            else
            {
                PdfPTable table1 = new PdfPTable(1);
                table1.WidthPercentage = 100;
                PdfPCell cell1 = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                cell1 = new PdfPCell(new Phrase(chunk));
                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell1.Border = 0;
                table1.AddCell(cell1);
                pdfDoc.Add(table1);
            }

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            Response.AppendHeader("Content-Disposition", "inline; filename=" + uniquefilename + ".pdf");
            return new FileStreamResult(output, "application/pdf");

        }

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult MyegReport()
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> wilayahList = new List<SelectListItem>();
        //    List<SelectListItem> ldgList = new List<SelectListItem>();
        //    List<SelectListItem> krytnlist = new List<SelectListItem>();
        //    wilayahList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
        //    ldgList = new SelectList(db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + "-" + s.fld_LdgName }), "Value", "Text").ToList();
        //    krytnlist = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
        //    krytnlist.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblAll, Value = "0" }));


        //    ViewBag.Report = "class = active";
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc");
        //    ViewBag.KrytnList = krytnlist;
        //    ViewBag.WilayahList = wilayahList;
        //    ViewBag.LdgList = ldgList;

        //    return View();
        //}

        //public ActionResult _MyegReport(string RadioGroup, string MonthList, int? WilayahList, int? LdgList, string KrytnList, int page = 1, string sort = "fld_Nopkj", string sortdir = "ASC")
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<CustMod_Myeg> InfoMyegList = new List<CustMod_Myeg>();

        //    if (MonthList == null && KrytnList == null)
        //    {
        //        ViewBag.Message = GlobalResCorp.msgChooseWork;
        //        return View();
        //    }

        //    DateTime todaydate = DateTime.Today;
        //    DateTime startdate = DateTime.Today.AddMonths(int.Parse(MonthList));


        //    if (RadioGroup != "0")
        //    {

        //        dbSP.SetCommandTimeout(120);
        //        var result = dbSP.sp_MyegDetail(NegaraID, SyarikatID, WilayahList, LdgList).ToList();


        //        return View(result);
        //    }
        //    else
        //    {
        //        dbSP.SetCommandTimeout(120);
        //        var result1 = dbSP.sp_MyegDetail(NegaraID, SyarikatID, WilayahList, LdgList).ToList();

        //        return View(result1);
        //    }

        //}

        //public ActionResult PaySheetReport()
        //{

        //    ViewBag.Report = "class = active";
        //    //Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    //string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    //MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
        //    int[] wlyhid = new int[] { };
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int month = timezone.gettimezone().Month;

        //    //List<SelectListItem> SelectionList = new List<SelectListItem>();
        //    //SelectionList = new SelectList(
        //    //    dbr.tbl_Pkjmast
        //    //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
        //    //                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
        //    //        .OrderBy(o => o.fld_Nopkj)
        //    //        .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
        //    //    "Value", "Text").ToList();
        //    //SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    //ViewBag.SelectionList = SelectionList;
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.YearList = yearlist;

        //    //var statusList = new List<SelectListItem>();
        //    //statusList = new SelectList(
        //    //    dbhq.tblOptionConfigsWeb
        //    //        .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false)
        //    //        .OrderBy(o => o.fldOptConfDesc)
        //    //        .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //    //    "Value", "Text").ToList();

        //    var monthList = new SelectList(
        //        dbhq.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false),
        //        "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.MonthList = monthList;
        //    //ViewBag.StatusList = statusList;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    return View();
        //}

        //public ActionResult _WorkerPaySheetRptAdvanceSearch()
        //{
        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_ViewingModels dbview = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();

        //    var statusList = new SelectList(
        //        dbhq.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();

        //    ViewBag.StatusList = statusList;

        //    var workCategoryList = new SelectList(
        //        dbhq.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();

        //    ViewBag.WorkCategoryList = workCategoryList;

        //    return View();
        //}

        //public ViewResult _WorkerPaySheetRptSearch(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList)
        //{
        //    List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();
        //    if (WilayahIDList == null && LadangIDList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySheetPekerjaList);
        //    }
        //    else if (WilayahIDList == 0 && LadangIDList == 0)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySheetPekerjaList);
        //    }
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;



        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value,
        //        NegaraID.Value);
        //    MVC_SYSTEM_ViewingModels dbview = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ViewingModels dbview2 = new MVC_SYSTEM_ViewingModels();
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();

        //    //List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();

        //    ViewBag.MonthList = MonthList;
        //    ViewBag.YearList = YearList;
        //    //ViewBag.WorkerList = SelectionList;
        //    ViewBag.NamaSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NamaSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NoSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NoSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NegaraID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.UserName = User.Identity.Name;
        //    ViewBag.Date = DateTime.Now.ToShortDateString();
        //    ViewBag.NamaPengurus = dbhq.tbl_Ladang
        //        .Where(x => x.fld_ID == LadangIDList)
        //        .Select(s => s.fld_Pengurus).Single();
        //    ViewBag.NamaPenyelia = dbhq.tblUsers
        //       .Where(x => x.fldUserID == getuserid)
        //       .Select(s => s.fldUserFullName).Single();
        //    ViewBag.IDPenyelia = getuserid;

        //    if (MonthList == null && YearList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySheetPekerjaList);
        //    }

        //    else
        //    {
        //        IOrderedQueryable<ViewingModelsOPMS.vw_PaySheetPekerja> salaryData;

        //        if (WilayahIDList == 2 && LadangIDList == 0)
        //        {
        //            LadangIDList = 92;
        //        }

        //        salaryData = dbview.vw_PaySheetPekerja
        //            .Where(x => x.fld_NegaraID == NegaraID &&
        //                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
        //                                    x.fld_SyarikatID == SyarikatID &&
        //                                    x.fld_WilayahID == WilayahIDList && x.fld_LadangID == LadangIDList)
        //                        .OrderBy(x => x.fld_Nama);

        //        foreach (var salary in salaryData)
        //        {
        //            PaySheetPekerjaList.Add(
        //                new vw_PaySheetPekerjaCustomModel()
        //                {
        //                    PaySheetPekerja = salary
        //                });
        //        }

        //        if (PaySheetPekerjaList.Count == 0)
        //        {
        //            ViewBag.Message = "Tiada Rekod";
        //        }

        //        return View(PaySheetPekerjaList);
        //    }
        //}

        public ActionResult MapaReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            int[] wlyhid = new int[] { };
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;

            List<SelectListItem> CompCodeList = new List<SelectListItem>();
            List<SelectListItem> wilayahList = new List<SelectListItem>();
            List<SelectListItem> ladangList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                CompCodeList = new SelectList(db2.tblOptionConfigsWeb
                        .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                        .OrderBy(o => o.fldOptConfDesc)
                        .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                    "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                wilayahList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                wilayahList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                //ladangList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                ladangList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                CompCodeList = new SelectList(db2.tblOptionConfigsWeb
                .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .OrderBy(o => o.fldOptConfDesc)
                .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                wilayahList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                wilayahList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                incldg = 1;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                CompCodeList = new SelectList(db2.tblOptionConfigsWeb
                .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .OrderBy(o => o.fldOptConfDesc)
                .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                wilayahList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                ladangList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 1;
            }

            ViewBag.CompCodeList = CompCodeList;
            ViewBag.WilayahList = wilayahList;
            ViewBag.LadangList = ladangList;

            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            int month = timezone.gettimezone().Month;

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month - 1);

            return View();
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Viewer")]
        public ViewResult _MapaReport(int? WilayahList, int? LadangList, int? MonthList, int? YearList, string CompCodeList, string print)
        {

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;
            ViewBag.YearList = YearList;
            ViewBag.MonthList = MonthList;
            //Added by Shazana 15/8/2023
            var namapenuhsyarikat = GetConfig.GetSyarikatFullName(CompCodeList);
            if (namapenuhsyarikat == "") { namapenuhsyarikat = "-"; }
            var namasyarikat = GetConfig.GetSyarikatName(CompCodeList);
            ViewBag.costcentre = CompCodeList;
            ViewBag.namapenuhsyarikat = namapenuhsyarikat.ToUpper();
            ViewBag.namasyarikat = namasyarikat.ToUpper();

            List<sp_MapaReport_Result> MapaData = new List<sp_MapaReport_Result>();
            List<vw_MapaCustomModel> PaySheetPekerjaList = new List<vw_MapaCustomModel>();

            String namabulan = db.tblOptionConfigsWeb.Where(x => x.fldOptConfValue == MonthList.ToString()).Select(x => x.fldOptConfFlag2).FirstOrDefault();
            if (namabulan != null)
            { ViewBag.namabulan = namabulan.ToUpper(); }
            else
            { ViewBag.namabulan = ""; }
            ViewBag.WilayahList = WilayahList;
            ViewBag.LadangList = LadangList;

            try
            {
                if (String.IsNullOrEmpty(MonthList.ToString()) || String.IsNullOrEmpty(YearList.ToString()) || String.IsNullOrEmpty(WilayahList.ToString()))
                {
                    ViewBag.Message = @GlobalResCorp.lblChooseWorkerMasterDataReport;
                }
                else
                {
                    if (WilayahList == 0)
                    {

                        if (LadangList == 0)
                        {
                            dbSP.SetCommandTimeout(3600);
                            MapaData = dbSP.sp_MapaReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                            .Where(x => x.fld_Month == MonthList && x.fld_Year == YearList).OrderBy(o => o.fld_WilayahID).ToList();
                        }
                    }
                    else
                    {
                        if (LadangList == 0)
                        {
                            dbSP.SetCommandTimeout(3600);
                            MapaData = dbSP.sp_MapaReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                            .Where(x => x.fld_Month == MonthList &&
                                    x.fld_Year == YearList && x.fld_WilayahID == WilayahList).OrderBy(o => o.fld_WilayahID).ToList();
                            ViewBag.Tajuk = db.tbl_Wilayah
                            .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == WilayahList)
                               .Select(s => "Wilayah " + s.fld_WlyhName.Substring(0, 1).ToUpper() + s.fld_WlyhName.Substring(1).ToLower()).FirstOrDefault();
                        }
                        else
                        {
                            dbSP.SetCommandTimeout(3600);
                            MapaData = dbSP.sp_MapaReport(NegaraID, SyarikatID, WilayahList, LadangList, YearList, MonthList, getuserid, CompCodeList)
                            .Where(x => x.fld_Month == MonthList &&
                                    x.fld_Year == YearList && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList).OrderBy(o => o.fld_WilayahID).ToList();

                            ViewBag.Tajuk = db.tbl_Ladang
                  .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangList)
                  .Select(s => s.fld_LdgName.Substring(0, 1).ToUpper() + s.fld_LdgName.Substring(1).ToLower())
                  .FirstOrDefault();
                        }
                    }
                    if (MapaData.Count() != 0)
                    {
                        foreach (var salary in MapaData)
                        {
                            Connection Connection = new Connection();
                            string host, catalog, user, pass = "";


                            Connection.GetConnection(out host, out catalog, out user, out pass, salary.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
                            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

                            var SbkpSipMajikan = estateConnection.tbl_ByrCarumanTambahan.Where(x => x.fld_NegaraID == salary.fld_NegaraID && x.fld_SyarikatID == salary.fld_SyarikatID && x.fld_WilayahID == salary.fld_WilayahID && x.fld_LadangID == salary.fld_LadangID && x.fld_Month == MonthList && x.fld_Year == YearList).Select(x => x.fld_CarumanMajikan).ToList();
                            var summajikan = SbkpSipMajikan.Sum();

                            var socsoMajikan = estateConnection.vw_PaySheetPekerja.Where(x => x.fld_NegaraID == salary.fld_NegaraID && x.fld_SyarikatID == salary.fld_SyarikatID && x.fld_WilayahID == salary.fld_WilayahID && x.fld_LadangID == salary.fld_LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).Select(x => x.fld_SocsoMjk).ToList();
                            var sumsocsomajikan = socsoMajikan.Sum();

                            var kwspMajikan = estateConnection.vw_PaySheetPekerja.Where(x => x.fld_NegaraID == salary.fld_NegaraID && x.fld_SyarikatID == salary.fld_SyarikatID && x.fld_WilayahID == salary.fld_WilayahID && x.fld_LadangID == salary.fld_LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).Select(x => x.fld_KWSPMjk).ToList();
                            var sumKwspMajikan = socsoMajikan.Sum();

                            var ladangInfo = db.tbl_Ladang.Where(x => x.fld_ID == salary.fld_LadangID && x.fld_WlyhID == salary.fld_WilayahID && x.fld_NegaraID == salary.fld_NegaraID && x.fld_SyarikatID == salary.fld_SyarikatID).FirstOrDefault();

                            List<ModelsCustom.CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<ModelsCustom.CarumanTambahanCustomModel>();

                            ModelsCustom.CarumanTambahanCustomModel carumanTambahanCustomModel = new ModelsCustom.CarumanTambahanCustomModel();
                            carumanTambahanCustomModel.fld_KodCarumanTambahan = "SBKPSIP";
                            carumanTambahanCustomModel.fld_CarumanMajikan = summajikan + sumsocsomajikan;
                            carumanTambahanCustomModel.fld_CarumanMajikanKwsp = sumKwspMajikan;
                            carumanTambahanCustomModel.fld_accountNo = ladangInfo.fld_NoAcc == null ? "" : ladangInfo.fld_NoAcc;
                            carumanTambahanCustomModel.fld_NoGL = ladangInfo.fld_NoGL == null ? "" : ladangInfo.fld_NoGL;
                            carumanTambahanCustomModel.fld_zonCIT = ladangInfo.fld_NoCIT == null ? "" : ladangInfo.fld_NoCIT;
                            carumanTambahanCustomModel.ladangid = salary.fld_LadangID;
                            carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);

                            PaySheetPekerjaList.Add(new ModelsCustom.vw_MapaCustomModel()
                            {
                                sp_RptMAPA = salary,
                                CarumanTambahan = carumanTambahanCustomModelList
                            });
                        }
                    }
                    else
                    {
                        if (PaySheetPekerjaList.Count == 0)
                        {
                            ViewBag.Message = @GlobalResCorp.msgNoRecord;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }
            return View(PaySheetPekerjaList);
        }

    }
}