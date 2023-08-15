using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq.Dynamic;
using System.Linq;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using System.IO;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.ModelsCorporate;
using static MVC_SYSTEM.Class.GlobalFunction;

namespace MVC_SYSTEM.Controllers
{
    
    public class DataEntryController : Controller
    {
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetIdentity getidentity = new GetIdentity();
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        //private MVC_SYSTEM_SP_Models db3 = new MVC_SYSTEM_SP_Models();
        GetConfig Getconfig = new GetConfig();
        GetNSWL GetNSWL = new GetNSWL();
        GetWilayah GetWilayah = new GetWilayah();
        errorlog geterror = new errorlog();
        // GET: DataEntry
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult Index()
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? getroleid = getidentity.getRoleID(getuserid);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            int?[] reportid = new int?[] { };
            //string myreportid = "";

            ViewBag.DataEntry = "class = active";

            //reportid = db.tblRoleReports.Where(x => x.fldRoleID == getroleid).Select(s => s.fldReportID).ToArray();

            //myreportid = String.Join("", reportid); ;

            //List<SelectListItem> SubReportList = new List<SelectListItem>();

            ViewBag.DataEntryList = new SelectList(db.tblDataEntryLists.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldDataEntryListID).Select(s => new SelectListItem { Value = s.fldDataEntryListID.ToString(), Text = s.fldDataEntryListName }), "Value", "Text").ToList();
            //ViewBag.ReportList = new SelectList(db.tblReportLists.Where(x => myreportid.Contains(x.fldReportListID.ToString()) && x.fldDeleted == false).OrderBy(o => o.fldReportListID).Select(s => new SelectListItem { Value = s.fldReportListID.ToString(), Text = s.fldReportListName }), "Value", "Text").ToList();
            //ViewBag.SubReportList = SubReportList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult Index(int DataEntryList)
        {
            string action = "", controller = "";
            var getentry = db.tblDataEntryLists.Find(DataEntryList);

            action = getentry.fldDataEntryListAction;
            controller = getentry.fldDataEntryListController;

            return RedirectToAction(action, controller);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult EstateNeed(string filter = "", int fldUserID = 0, int page = 1, string sort = "fld_LadangID", string sortdir = "DESC")
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int getyear = timezone.gettimezone().Year;
            GetLadang GetLadang = new GetLadang();
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.DataEntry = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(Getconfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_PerluLadang>();
            ViewBag.filter = filter;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (filter == "")
            {
                if (WilayahID == 0 && LadangID == 0)
                {
                    records.Content = dbview.tbl_PerluLadang.Where(x => x.fld_Tahun == getyear)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_PerluLadang.Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else if (WilayahID != 0 && LadangID == 0)
                {
                    records.Content = dbview.tbl_PerluLadang.Where(x => x.fld_WilayahID == WilayahID && x.fld_Tahun == getyear)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_PerluLadang.Where(x => x.fld_WilayahID == WilayahID && x.fld_Tahun == getyear).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else if (WilayahID != 0 && LadangID != 0)
                {
                    records.Content = dbview.tbl_PerluLadang.Where(x => x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tahun == getyear)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_PerluLadang.Where(x => x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tahun == getyear).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            else
            {
                //string ldgid = "";
                //var ldgid = GetLadang.GetSearchLadangID(filter);

                if (WilayahID == 0 && LadangID == 0)
                {
                    records.Content = dbview.tbl_PerluLadang.Where(x => (x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter) && x.fld_Tahun == getyear)
                    .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_PerluLadang.Where(x => (x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter) && x.fld_Tahun == getyear).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else if (WilayahID != 0 && LadangID == 0)
                {
                    records.Content = dbview.tbl_PerluLadang.Where(x => (x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter) && (x.fld_WilayahID == WilayahID) && x.fld_Tahun == getyear)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_PerluLadang.Where(x => (x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter) && (x.fld_WilayahID == WilayahID) && x.fld_Tahun == getyear).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else if (WilayahID != 0 && LadangID != 0)
                {
                    records.Content = dbview.tbl_PerluLadang.Where(x => (x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter) && (x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID) && x.fld_Tahun == getyear)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_PerluLadang.Where(x => (x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter) && (x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID) && x.fld_Tahun == getyear).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            return View(records);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult EstateNeedInsert()
        {
            int drpyear = 0;
            int drprangeyear = 0;
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int getyear = timezone.gettimezone().Year;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
                var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                var getexistingLadangID = db.tbl_PerluLadang.Where(x => x.fld_Tahun == getyear && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyhID).Select(s => s.fld_LadangID).ToArray();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && !getexistingLadangID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName").ToList();
                var getexistingLadangID = db.tbl_PerluLadang.Where(x => x.fld_Tahun == getyear && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID).Select(s => s.fld_LadangID).ToArray();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && !getexistingLadangID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            //drpyear = timezone.gettimezone().Year - int.Parse(Getconfig.GetData("yeardisplay")) + 1;
            //drprangeyear = timezone.gettimezone().Year;
            drpyear = timezone.gettimezone().Year;
            drprangeyear = timezone.gettimezone().Year + 1;
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
            ViewBag.fld_Tahun = yearlist;
            ViewBag.fld_WilayahID = WilayahIDList;
            ViewBag.fld_LadangID = LadangIDList;
            return PartialView("EstateNeedInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult EstateNeedInsert(ModelsCorporate.tbl_PerluLadang tbl_PerluLadang)
        {
            int getmonth = timezone.gettimezone().Month;
            DateTime getDT = timezone.gettimezone();
            GetLadang GetLadang = new GetLadang();
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //if (ModelState.IsValid)
            //{
            try
            {
                var checkdata = db.tbl_PerluLadang.Where(x => x.fld_LadangID == tbl_PerluLadang.fld_LadangID && x.fld_Tahun == tbl_PerluLadang.fld_Tahun).FirstOrDefault();
                if (checkdata == null)
                {
                    int ldgid = tbl_PerluLadang.fld_LadangID.Value;
                    int wlyhid = tbl_PerluLadang.fld_WilayahID.Value;
                    tbl_PerluLadang.fld_LadangCode = GetLadang.GetCodeLadangFromID2(ldgid);
                    tbl_PerluLadang.fld_LadangName = GetLadang.GetLadangName(ldgid, wlyhid);
                    tbl_PerluLadang.fld_SyarikatID = SyarikatID;
                    tbl_PerluLadang.fld_NegaraID = NegaraID;
                    tbl_PerluLadang.fld_Bulan = getmonth;
                    tbl_PerluLadang.fld_CreatedBy = getuserid;
                    tbl_PerluLadang.fld_CreatedDT = getDT;
                    tbl_PerluLadang.fld_ModifiedBy = getuserid;
                    tbl_PerluLadang.fld_ModifiedDT = getDT;

                    db.tbl_PerluLadang.Add(tbl_PerluLadang);
                    db.SaveChanges();

                    ModelsCorporate.tbl_PerluLadangHistory tbl_PerluLadangHistory = new ModelsCorporate.tbl_PerluLadangHistory();
                    tbl_PerluLadangHistory.fld_LadangCode = GetLadang.GetCodeLadangFromID2(ldgid);
                    tbl_PerluLadangHistory.fld_LadangName = GetLadang.GetLadangName(ldgid, wlyhid);
                    tbl_PerluLadangHistory.fld_SyarikatID = SyarikatID;
                    tbl_PerluLadangHistory.fld_NegaraID = NegaraID;
                    tbl_PerluLadangHistory.fld_Bulan = getmonth;
                    tbl_PerluLadangHistory.fld_CreatedBy = getuserid;
                    tbl_PerluLadangHistory.fld_CreatedDT = getDT;
                    tbl_PerluLadangHistory.fld_ModifiedBy = getuserid;
                    tbl_PerluLadangHistory.fld_ModifiedDT = getDT;
                    tbl_PerluLadangHistory.fld_LadangID = tbl_PerluLadang.fld_LadangID;
                    tbl_PerluLadangHistory.fld_WilayahID = tbl_PerluLadang.fld_WilayahID;
                    tbl_PerluLadangHistory.fld_Perlu = tbl_PerluLadang.fld_Perlu;
                    tbl_PerluLadangHistory.fld_Luas = tbl_PerluLadang.fld_Luas;
                    tbl_PerluLadangHistory.fld_Tahun = tbl_PerluLadang.fld_Tahun;
                    
                    db.tbl_PerluLadangHistory.Add(tbl_PerluLadangHistory);
                    db.SaveChanges();

                    var getid = db.tbl_PerluLadang.Where(w => w.fld_LadangID == tbl_PerluLadang.fld_LadangID).FirstOrDefault();
                    return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tbl_PerluLadang.fld_LadangID, data2 = tbl_PerluLadang.fld_LadangID, data3 = "" });
                }
                else
                {
                    return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
            //}
            //else
            //{
            //    return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            //}
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult EstateNeedUpdate(int? id, int? year)
        {
            int drpyear = 0;
            int drprangeyear = 0;
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int getmonth = timezone.gettimezone().Month;

            if (id == null)
            {
                return RedirectToAction("EstateNeed");
            }
            ModelsCorporate.tbl_PerluLadang tbl_PerluLadang = db.tbl_PerluLadang.Where(w => w.fld_LadangID == id && w.fld_Tahun == year).FirstOrDefault();
            if (tbl_PerluLadang == null)
            {
                return RedirectToAction("EstateNeed");
            }

            if (getmonth == tbl_PerluLadang.fld_Bulan)
            {
                ViewBag.DisableDataEdit = true;
            }
            else
            {
                ViewBag.DisableDataEdit = false;
            }

            if (getidentity.MySuperAdmin(User.Identity.Name) || getidentity.MyAdmin(User.Identity.Name))
            {
                ViewBag.DisableDataEdit = false;
            }

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName", tbl_PerluLadang.fld_WilayahID).ToList();
                //var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == tbl_PerluLadang.fld_WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", tbl_PerluLadang.fld_LadangID).ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", tbl_PerluLadang.fld_WilayahID).ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", tbl_PerluLadang.fld_LadangID).ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", tbl_PerluLadang.fld_WilayahID).ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", tbl_PerluLadang.fld_LadangID).ToList();
            }

            drpyear = timezone.gettimezone().Year - int.Parse(Getconfig.GetData("yeardisplay")) + 1;
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
            ViewBag.fld_Tahun = yearlist;
            ViewBag.fld_WilayahID = WilayahIDList;
            ViewBag.fld_LadangID = LadangIDList;
            //if (getidentity.MySuperAdmin(User.Identity.Name))
            //{
            //    ViewBag.fldRoleID = new SelectList(db.tblRoles, "fldRoleID", "fldRoleName", tblUser.fldRoleID);
            //}
            //else
            //{
            //    ViewBag.fldRoleID = new SelectList(db.tblRoles.Where(x => x.fldRoleID != 1), "fldRoleID", "fldRoleName", tblUser.fldRoleID);
            //}

            //ViewBag.fldClientID = new SelectList(db.tblClients, "fldClientID", "fldClientName", tblUser.fldClientID);
            //ViewBag.fldDeleted = new SelectList(db2.tblSystemConfigs.Where(x => x.fldFlag1 == "useractivation" && x.fldDeleted == false), "fldConfigValue", "fldConfigDesc", tblUser.fldDeleted);
            //tblUser.fldUserPassword = crypto.Decrypt(tblUser.fldUserPassword);
            return PartialView("EstateNeedUpdate", tbl_PerluLadang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult EstateNeedUpdate(int id, int year, ModelsCorporate.tbl_PerluLadang tbl_PerluLadang)
        {
            int getmonth = timezone.gettimezone().Month;
            DateTime getDT = timezone.gettimezone();
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //if (ModelState.IsValid)
            //{
            try
            {
                var getdata = db.tbl_PerluLadang.Where(w => w.fld_LadangID == id && w.fld_Tahun == year).FirstOrDefault();
                //getdata.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
                //getdata.fldUserFullName = tblUser.fldUserFullName;
                //getdata.fldUserShortName = tblUser.fldUserShortName;
                //getdata.fldUserEmail = tblUser.fldUserEmail;
                //getdata.fldClientID = tblUser.fldClientID;
                //getdata.fldRoleID = tblUser.fldRoleID;
                //getdata.fldDeleted = tblUser.fldDeleted;
                getdata.fld_Bulan = getmonth;
                getdata.fld_Luas = tbl_PerluLadang.fld_Luas;
                getdata.fld_Perlu = tbl_PerluLadang.fld_Perlu;
                getdata.fld_ModifiedBy = getuserid;
                getdata.fld_ModifiedDT = getDT;

                db.Entry(getdata).State = EntityState.Modified;
                db.SaveChanges();

                ModelsCorporate.tbl_PerluLadangHistory tbl_PerluLadangHistory = new ModelsCorporate.tbl_PerluLadangHistory();
                tbl_PerluLadangHistory.fld_LadangCode = getdata.fld_LadangCode;
                tbl_PerluLadangHistory.fld_LadangName = getdata.fld_LadangName;
                tbl_PerluLadangHistory.fld_SyarikatID = getdata.fld_SyarikatID;
                tbl_PerluLadangHistory.fld_NegaraID = getdata.fld_NegaraID;
                tbl_PerluLadangHistory.fld_CreatedBy = getdata.fld_CreatedBy;
                tbl_PerluLadangHistory.fld_CreatedDT = getdata.fld_CreatedDT;
                tbl_PerluLadangHistory.fld_ModifiedBy = getuserid;
                tbl_PerluLadangHistory.fld_ModifiedDT = getDT;
                tbl_PerluLadangHistory.fld_LadangID = getdata.fld_LadangID;
                tbl_PerluLadangHistory.fld_WilayahID = getdata.fld_WilayahID;
                tbl_PerluLadangHistory.fld_Perlu = tbl_PerluLadang.fld_Perlu;
                tbl_PerluLadangHistory.fld_Luas = tbl_PerluLadang.fld_Luas;
                tbl_PerluLadangHistory.fld_Tahun = getdata.fld_Tahun;
                tbl_PerluLadangHistory.fld_Bulan = getmonth;

                db.tbl_PerluLadangHistory.Add(tbl_PerluLadangHistory);
                db.SaveChanges();

                var getid = id;
                //RedirectToAction("EstateNeed", "DataEntry");
                return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
            //}
            //else
            //{
            //    return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            //}
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult EstateNeedDelete(int? id, int? year)
        {
            string WlyhName = "";
            string LdgName = "";
            string LdgCode = "";
            if (id == null)
            {
                return RedirectToAction("EstateNeed");
            }
            ModelsCorporate.tbl_PerluLadang tbl_PerluLadang = db.tbl_PerluLadang.Where(w => w.fld_LadangID == id && w.fld_Tahun == year).FirstOrDefault();
            if (tbl_PerluLadang == null)
            {
                return RedirectToAction("EstateNeed");
            }
            else
            {
                GetWilayah GetWilayah = new GetWilayah();
                GetLadang GetLadang = new GetLadang();
                WlyhName = GetWilayah.GetWilayahName(tbl_PerluLadang.fld_WilayahID.Value);
                LdgName = GetLadang.GetLadangName(id.Value, tbl_PerluLadang.fld_WilayahID.Value);
                LdgCode = GetLadang.GetLadangCode(id.Value);
            }

            ViewBag.WilayahName = WlyhName;
            ViewBag.LadangName = LdgCode + " - " + LdgName;
            return PartialView("EstateNeedDelete", tbl_PerluLadang);
        }

        [HttpPost, ActionName("EstateNeedDelete")]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult DeleteConfirmed(int id, int? year)
        {
            try
            {
                ModelsCorporate.tbl_PerluLadang tbl_PerluLadang = db.tbl_PerluLadang.Where(w => w.fld_LadangID == id && w.fld_Tahun == year).FirstOrDefault();
                if (tbl_PerluLadang == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tbl_PerluLadang.Remove(tbl_PerluLadang);
                    db.SaveChanges();
                    RedirectToAction("EstateNeed", "DataEntry");
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });

                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }

        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult EstateNeedHQ(string filter = "", int fldUserID = 0, int page = 1, string sort = "fld_LadangID", string sortdir = "DESC")
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetLadang GetLadang = new GetLadang();
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.DataEntry = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(Getconfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_QuotaPerluLadang>();
            ViewBag.filter = filter;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (filter == "")
            {
                if (WilayahID == 0 && LadangID == 0)
                {
                    records.Content = dbview.tbl_QuotaPerluLadang
                        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_QuotaPerluLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else if (WilayahID != 0 && LadangID == 0)
                {
                    records.Content = dbview.tbl_QuotaPerluLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_QuotaPerluLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else if (WilayahID != 0 && LadangID != 0)
                {
                    records.Content = dbview.tbl_QuotaPerluLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_QuotaPerluLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            else
            {
                //string ldgid = "";
                //var ldgid = GetLadang.GetSearchLadangID(filter);

                if (WilayahID == 0 && LadangID == 0)
                {
                    records.Content = dbview.tbl_QuotaPerluLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter)
                    .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_QuotaPerluLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&  x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else if (WilayahID != 0 && LadangID == 0)
                {
                    records.Content = dbview.tbl_QuotaPerluLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && (x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter) && (x.fld_WilayahID == WilayahID))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_QuotaPerluLadang.Where(x => (x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter) && (x.fld_WilayahID == WilayahID)).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else if (WilayahID != 0 && LadangID != 0)
                {
                    records.Content = dbview.tbl_QuotaPerluLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && (x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter) && (x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_QuotaPerluLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && (x.fld_LadangCode.Contains(filter) || x.fld_LadangName.Contains(filter) || x.fld_Tahun.ToString() == filter) && (x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            return View(records);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult EstateNeedHQInsert()
        {
            int drpyear = 0;
            int drprangeyear = 0;
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };
            //string mywlyid = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
                var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            //drpyear = timezone.gettimezone().Year - int.Parse(Getconfig.GetData("yeardisplay")) + 1;
            //drprangeyear = timezone.gettimezone().Year;
            drpyear = timezone.gettimezone().Year;
            drprangeyear = timezone.gettimezone().Year + 1;
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
            ViewBag.fld_Tahun = yearlist;
            ViewBag.fld_WilayahID = WilayahIDList;
            ViewBag.fld_LadangID = LadangIDList;
            return PartialView("EstateNeedHQInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult EstateNeedHQInsert(ModelsCorporate.tbl_QuotaPerluLadang tbl_QuotaPerluLadang)
        {
            GetLadang GetLadang = new GetLadang();
            DateTime getDT = timezone.gettimezone();
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //if (ModelState.IsValid)
            //{
            try
            {
                var checkdata = db.tbl_QuotaPerluLadang.Where(x => x.fld_LadangID == tbl_QuotaPerluLadang.fld_LadangID && x.fld_Tahun == tbl_QuotaPerluLadang.fld_Tahun).FirstOrDefault();
                if (checkdata == null)
                {
                    int ldgid = tbl_QuotaPerluLadang.fld_LadangID.Value;
                    int wlyhid = tbl_QuotaPerluLadang.fld_WilayahID.Value;
                    tbl_QuotaPerluLadang.fld_LadangCode = GetLadang.GetCodeLadangFromID2(ldgid);
                    tbl_QuotaPerluLadang.fld_LadangName = GetLadang.GetLadangName(ldgid, wlyhid);
                    tbl_QuotaPerluLadang.fld_SyarikatID = SyarikatID;
                    tbl_QuotaPerluLadang.fld_NegaraID = NegaraID;
                    tbl_QuotaPerluLadang.fld_CreatedBy = getuserid;
                    tbl_QuotaPerluLadang.fld_CreatedDT = getDT;
                    tbl_QuotaPerluLadang.fld_ModifiedBy = getuserid;
                    tbl_QuotaPerluLadang.fld_ModifiedDT = getDT;
                    db.tbl_QuotaPerluLadang.Add(tbl_QuotaPerluLadang);
                    db.SaveChanges();

                    ModelsCorporate.tbl_QuotaPerluLadangHistory tbl_QuotaPerluLadangHistory = new ModelsCorporate.tbl_QuotaPerluLadangHistory();
                    tbl_QuotaPerluLadangHistory.fld_LadangCode = tbl_QuotaPerluLadang.fld_LadangCode;
                    tbl_QuotaPerluLadangHistory.fld_LadangName = tbl_QuotaPerluLadang.fld_LadangName;
                    tbl_QuotaPerluLadangHistory.fld_LadangName = tbl_QuotaPerluLadang.fld_LadangName;
                    tbl_QuotaPerluLadangHistory.fld_LadangName = tbl_QuotaPerluLadang.fld_LadangName;
                    tbl_QuotaPerluLadangHistory.fld_CreatedBy = tbl_QuotaPerluLadang.fld_CreatedBy;
                    tbl_QuotaPerluLadangHistory.fld_CreatedDT = tbl_QuotaPerluLadang.fld_CreatedDT;
                    tbl_QuotaPerluLadangHistory.fld_ModifiedBy = tbl_QuotaPerluLadang.fld_ModifiedBy;
                    tbl_QuotaPerluLadangHistory.fld_ModifiedDT = tbl_QuotaPerluLadang.fld_ModifiedDT;
                    tbl_QuotaPerluLadangHistory.fld_LadangID = tbl_QuotaPerluLadang.fld_LadangID;
                    tbl_QuotaPerluLadangHistory.fld_WilayahID = tbl_QuotaPerluLadang.fld_WilayahID;
                    tbl_QuotaPerluLadangHistory.fld_Perlu = tbl_QuotaPerluLadang.fld_Perlu;
                    tbl_QuotaPerluLadangHistory.fld_Tahun = tbl_QuotaPerluLadang.fld_Tahun;

                    db.tbl_QuotaPerluLadangHistory.Add(tbl_QuotaPerluLadangHistory);
                    db.SaveChanges();

                    var getid = db.tbl_QuotaPerluLadang.Where(w => w.fld_LadangID == tbl_QuotaPerluLadang.fld_LadangID).FirstOrDefault();
                    return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tbl_QuotaPerluLadang.fld_LadangID, data2 = tbl_QuotaPerluLadang.fld_LadangID, data3 = "" });
                }
                else
                {
                    return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
            //}
            //else
            //{
            //    return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            //}
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult EstateNeedHQUpdate(int? id, int? year)
        {
            int drpyear = 0;
            int drprangeyear = 0;
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int getmonth = timezone.gettimezone().Month;

            if (id == null)
            {
                return RedirectToAction("EstateNeedHQ");
            }
            ModelsCorporate.tbl_QuotaPerluLadang tbl_QuotaPerluLadang = db.tbl_QuotaPerluLadang.Where(w => w.fld_LadangID == id && w.fld_Tahun == year).FirstOrDefault();
            if (tbl_QuotaPerluLadang == null)
            {
                return RedirectToAction("EstateNeedHQ");
            }

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName", tbl_QuotaPerluLadang.fld_WilayahID).ToList();
                //var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == tbl_QuotaPerluLadang.fld_WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", tbl_QuotaPerluLadang.fld_LadangID).ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", tbl_QuotaPerluLadang.fld_WilayahID).ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", tbl_QuotaPerluLadang.fld_LadangID).ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", tbl_QuotaPerluLadang.fld_WilayahID).ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", tbl_QuotaPerluLadang.fld_LadangID).ToList();
            }

            drpyear = timezone.gettimezone().Year - int.Parse(Getconfig.GetData("yeardisplay")) + 1;
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
            ViewBag.fld_Tahun = yearlist;
            ViewBag.fld_WilayahID = WilayahIDList;
            ViewBag.fld_LadangID = LadangIDList;
            //if (getidentity.MySuperAdmin(User.Identity.Name))
            //{
            //    ViewBag.fldRoleID = new SelectList(db.tblRoles, "fldRoleID", "fldRoleName", tblUser.fldRoleID);
            //}
            //else
            //{
            //    ViewBag.fldRoleID = new SelectList(db.tblRoles.Where(x => x.fldRoleID != 1), "fldRoleID", "fldRoleName", tblUser.fldRoleID);
            //}

            //ViewBag.fldClientID = new SelectList(db.tblClients, "fldClientID", "fldClientName", tblUser.fldClientID);
            //ViewBag.fldDeleted = new SelectList(db2.tblSystemConfigs.Where(x => x.fldFlag1 == "useractivation" && x.fldDeleted == false), "fldConfigValue", "fldConfigDesc", tblUser.fldDeleted);
            //tblUser.fldUserPassword = crypto.Decrypt(tblUser.fldUserPassword);
            return PartialView("EstateNeedHQUpdate", tbl_QuotaPerluLadang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult EstateNeedHQUpdate(int id, int year, Models.tbl_QuotaPerluLadang tbl_QuotaPerluLadang)
        {
            DateTime getDT = timezone.gettimezone();
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //if (ModelState.IsValid)
            //{
            try
            {
                var getdata = db.tbl_QuotaPerluLadang.Where(w => w.fld_LadangID == id && w.fld_Tahun == year).FirstOrDefault();
                //getdata.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
                //getdata.fldUserFullName = tblUser.fldUserFullName;
                //getdata.fldUserShortName = tblUser.fldUserShortName;
                //getdata.fldUserEmail = tblUser.fldUserEmail;
                //getdata.fldClientID = tblUser.fldClientID;
                //getdata.fldRoleID = tblUser.fldRoleID;
                //getdata.fldDeleted = tblUser.fldDeleted;
                //getdata.fld_LadangID = tbl_PerluLadang.fld_LadangID;
                getdata.fld_Perlu = tbl_QuotaPerluLadang.fld_Perlu;
                getdata.fld_ModifiedBy = getuserid;
                getdata.fld_ModifiedDT = getDT;

                db.Entry(getdata).State = EntityState.Modified;
                db.SaveChanges();

                ModelsCorporate.tbl_QuotaPerluLadangHistory tbl_QuotaPerluLadangHistory = new ModelsCorporate.tbl_QuotaPerluLadangHistory();
                tbl_QuotaPerluLadangHistory.fld_LadangCode = getdata.fld_LadangCode;
                tbl_QuotaPerluLadangHistory.fld_LadangName = getdata.fld_LadangName;
                tbl_QuotaPerluLadangHistory.fld_SyarikatID = getdata.fld_SyarikatID;
                tbl_QuotaPerluLadangHistory.fld_NegaraID = getdata.fld_NegaraID;
                tbl_QuotaPerluLadangHistory.fld_CreatedBy = getdata.fld_CreatedBy;
                tbl_QuotaPerluLadangHistory.fld_CreatedDT = getdata.fld_CreatedDT;
                tbl_QuotaPerluLadangHistory.fld_ModifiedBy = getuserid;
                tbl_QuotaPerluLadangHistory.fld_ModifiedDT = getDT;
                tbl_QuotaPerluLadangHistory.fld_LadangID = getdata.fld_LadangID;
                tbl_QuotaPerluLadangHistory.fld_WilayahID = getdata.fld_WilayahID;
                tbl_QuotaPerluLadangHistory.fld_Perlu = tbl_QuotaPerluLadang.fld_Perlu;
                tbl_QuotaPerluLadangHistory.fld_Tahun = getdata.fld_Tahun;

                db.tbl_QuotaPerluLadangHistory.Add(tbl_QuotaPerluLadangHistory);
                db.SaveChanges();

                var getid = id;
                //RedirectToAction("EstateNeed", "DataEntry");
                return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
            //}
            //else
            //{
            //    return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            //}
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult EstateNeedHQDelete(int? id, int? year)
        {
            string WlyhName = "";
            string LdgName = "";
            string LdgCode = "";
            if (id == null)
            {
                return RedirectToAction("EstateNeed");
            }
            ModelsCorporate.tbl_QuotaPerluLadang tbl_QuotaPerluLadang = db.tbl_QuotaPerluLadang.Where(w => w.fld_LadangID == id && w.fld_Tahun == year).FirstOrDefault();
            if (tbl_QuotaPerluLadang == null)
            {
                return RedirectToAction("EstateNeedHQ");
            }
            else
            {
                GetWilayah GetWilayah = new GetWilayah();
                GetLadang GetLadang = new GetLadang();
                WlyhName = GetWilayah.GetWilayahName(tbl_QuotaPerluLadang.fld_WilayahID.Value);
                LdgName = GetLadang.GetLadangName(id.Value, tbl_QuotaPerluLadang.fld_WilayahID.Value);
                LdgCode = GetLadang.GetLadangCode(id.Value);
            }

            ViewBag.WilayahName = WlyhName;
            ViewBag.LadangName = LdgCode + " - " + LdgName;
            return PartialView("EstateNeedHQDelete", tbl_QuotaPerluLadang);
        }

        [HttpPost, ActionName("EstateNeedHQDelete")]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult DeleteConfirmed2(int id, int? year)
        {
            try
            {
                ModelsCorporate.tbl_QuotaPerluLadang tbl_QuotaPerluLadang = db.tbl_QuotaPerluLadang.Where(w => w.fld_LadangID == id && w.fld_Tahun == year).FirstOrDefault();
                if (tbl_QuotaPerluLadang == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tbl_QuotaPerluLadang.Remove(tbl_QuotaPerluLadang);
                    db.SaveChanges();
                    RedirectToAction("EstateNeed", "DataEntry");
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });

                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }

        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public JsonResult GetLadang(int WilayahID, int Tahun)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int Bulan = timezone.gettimezone().Month;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            var getexistingLadangID = db.tbl_PerluLadang.Where(x => x.fld_Tahun == Tahun && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID).Select(s => s.fld_LadangID).ToArray() ;
            
            if (GetWilayah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && !getexistingLadangID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public JsonResult GetLadang2(int WilayahID, int Tahun)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int Bulan = timezone.gettimezone().Month;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            var getexistingLadangID = db.tbl_QuotaPerluLadang.Where(x => x.fld_Tahun == Tahun && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID).Select(s => s.fld_LadangID).ToArray();

            if (GetWilayah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && !getexistingLadangID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public JsonResult GetQoutaAvailable(int WilayahID, int LadangID, int Tahun)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID2 = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            bool noqouta = false;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID2, getuserid, User.Identity.Name);

            var getQouta = db.tbl_QuotaPerluLadang.Where(x => x.fld_Tahun == Tahun && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Perlu).FirstOrDefault();
            
            if(getQouta == null)
            {
                noqouta = false;
            }
            else
            {
                noqouta = true;
            }

            return Json(new { totalqouta = getQouta, noqouta = noqouta });
        }

        public ActionResult PalmPriceInfo()
        {
            ViewBag.DataEntry = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year - int.Parse(Getconfig.GetData("yeardisplay")) + 1;
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
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", timezone.gettimezone().Month);

            List<SelectListItem> jenisTanaman = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "jnsTanaman" && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                .OrderBy(o => o.fldOptConfDesc)
                .Select(s => new SelectListItem
                {
                    Value = s.fldOptConfValue,
                    Text = s.fldOptConfValue + " - " + s.fldOptConfDesc
                })
                .ToList();
            jenisTanaman.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

            ViewBag.JenisTanamanList = jenisTanaman;

            return View();
        }

        public ActionResult _PalmPriceSearch(String JenisTanamanList, int? MonthList, int? YearList, int page = 1, string sort = "JenisTanamanList",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_ModelsCorporate modelsCorporate = new MVC_SYSTEM_ModelsCorporate();

            var message = "";
            if (String.IsNullOrEmpty(JenisTanamanList) || String.IsNullOrEmpty(MonthList.ToString()) || String.IsNullOrEmpty(YearList.ToString()))
            {
                message = GlobalResCorp.msgPalmPriceSearch;
                ViewBag.Message = message;
            }

            else
            {
                message = GlobalResCorp.msgErrorSearch;
                ViewBag.Message = message;
            }

            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();

            int pageSize = int.Parse(Getconfig.GetData("paging"));
            var records = new PagedList<ModelsCorporate.vw_HargaSemasa>();
            int role = getidentity.RoleID(getuserid).Value;

            // fatin added - 21/06/2023
            if (JenisTanamanList == "R")
            {
                ViewBag.Title2 = "Maklumat Harga SMR20";
            }
            else
            {
                ViewBag.Title2 = GlobalResCorp.lblPalmPriceInfo;
            }
            //end

            var hargaSawitSemasa = modelsCorporate.vw_HargaSemasa
                    .Where(x => x.fld_JnsTnmn == JenisTanamanList && x.fld_Bulan == MonthList &&
                                          x.fld_Tahun == YearList &&
                                          x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

            records.Content = hargaSawitSemasa.ToList();

            records.TotalRecords = hargaSawitSemasa
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalRecord = hargaSawitSemasa
                .Count();

            return View(records);
        }

        public ActionResult _PalmPriceAdd()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> jenisTanaman = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "jnsTanaman" && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                .OrderBy(o => o.fldOptConfDesc)
                .Select(s => new SelectListItem
                {
                    Value = s.fldOptConfValue,
                    Text = s.fldOptConfValue + " - " + s.fldOptConfDesc,
                })
                .ToList();
            jenisTanaman.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

            ViewBag.JenisTanamanList = jenisTanaman;

            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year - int.Parse(Getconfig.GetData("yeardisplay")) + 1;
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

            List<SelectListItem> monthList = db.tblOptionConfigsWebs.Where(x =>
                    x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => new SelectListItem
                {
                    Value = s.fldOptConfValue,
                    Text = s.fldOptConfDesc,
                })
                .ToList();

            ViewBag.MonthList = monthList;

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PalmPriceAdd(ModelsCorporate.tbl_HargaSawitSemasaModelViewCreate hargaSawitSemasaModelViewCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            try
            {
                var palmPriceMaxData = db.tbl_HargaSawitRange
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderByDescending(o => o.fld_RangeHargaUpper).FirstOrDefault();

                if (ModelState.IsValid && palmPriceMaxData.fld_RangeHargaUpper >= hargaSawitSemasaModelViewCreate.fld_HargaSemasa)
                {
                    var palmPriceData = db.tbl_HargaSawitSemasa.SingleOrDefault(x =>
                        x.fld_Bulan == hargaSawitSemasaModelViewCreate.fld_Bulan &&
                        x.fld_Tahun == hargaSawitSemasaModelViewCreate.fld_Tahun &&
                        x.fld_JnsTnmn == hargaSawitSemasaModelViewCreate.fld_JnsTnmn &&
                        x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

                    if (ModelState.IsValid && palmPriceData == null)
                    {
                        tbl_HargaSawitSemasa hargaSawitSemasa = new tbl_HargaSawitSemasa();

                        hargaSawitSemasa.fld_Bulan = hargaSawitSemasaModelViewCreate.fld_Bulan;
                        hargaSawitSemasa.fld_Tahun = hargaSawitSemasaModelViewCreate.fld_Tahun;
                        hargaSawitSemasa.fld_JnsTnmn = hargaSawitSemasaModelViewCreate.fld_JnsTnmn;
                        hargaSawitSemasa.fld_NegaraID = NegaraID;
                        hargaSawitSemasa.fld_SyarikatID = SyarikatID;
                        hargaSawitSemasa.fld_Deleted = false;
                        hargaSawitSemasa.fld_HargaSemasa = hargaSawitSemasaModelViewCreate.fld_HargaSemasa;

                        var getIncentive = db.tbl_HargaSawitRange
                            .SingleOrDefault(x =>
                                x.fld_RangeHargaLower <= hargaSawitSemasaModelViewCreate.fld_HargaSemasa &&
                                x.fld_RangeHargaUpper >= hargaSawitSemasaModelViewCreate.fld_HargaSemasa &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).fld_Insentif;

                        hargaSawitSemasa.fld_Insentif = getIncentive;

                        db.tbl_HargaSawitSemasa.Add(hargaSawitSemasa);
                        db.SaveChanges();

                        string appname = Request.ApplicationPath;
                        string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                        var lang = Request.RequestContext.RouteData.Values["lang"];

                        if (appname != "/")
                        {
                            domain = domain + appname;
                        }

                        return Json(new
                        {
                            success = true,
                            msg = GlobalResCorp.msgAdd,
                            status = "success",
                            checkingdata = "0",
                            method = "4",
                            div = "palmPriceDetails",
                            rootUrl = domain,
                            action = "_PalmPriceSearch",
                            controller = "DataEntry",
                            paramName = "JenisTanamanList",
                            paramValue = hargaSawitSemasaModelViewCreate.fld_JnsTnmn,
                            paramName2 = "MonthList",
                            paramValue2 = hargaSawitSemasaModelViewCreate.fld_Bulan,
                            paramName3 = "YearList",
                            paramValue3 = hargaSawitSemasaModelViewCreate.fld_Tahun
                        });
                    }

                    else
                    {
                        return Json(new
                        {
                            success = false,
                            msg = GlobalResCorp.msgDataExist,
                            status = "danger",
                            checkingdata = "0"
                        });
                    }
                }
                
                else
                {
                    return Json(new
                    {
                        success = false,
                        msg = GlobalResCorp.msgErrorData,
                        status = "danger",
                        checkingdata = "0"
                    });
                }
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = false,
                    msg = GlobalResCorp.msgError,
                    status = "danger",
                    checkingdata = "0"
                });
            }

            finally
            {
                db.Dispose();
            }
        }

        public JsonResult IsPalmPriceExceedValue(decimal? fld_HargaSemasa)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var palmPriceMaxData = db.tbl_HargaSawitRange
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .OrderByDescending(o => o.fld_RangeHargaUpper).FirstOrDefault();

            if (palmPriceMaxData.fld_RangeHargaUpper >= fld_HargaSemasa)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult _PalmPriceEdit(int? id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_ModelsCorporate modelsCorporate = new MVC_SYSTEM_ModelsCorporate();

            var palmPriceData = db.tbl_HargaSawitSemasa.SingleOrDefault(x => x.fld_ID == id);

            tbl_HargaSawitSemasaModelViewEdit hargaSawitSemasaModelViewEdit = new tbl_HargaSawitSemasaModelViewEdit();

            PropertyCopy.Copy(hargaSawitSemasaModelViewEdit, palmPriceData);

            return PartialView(hargaSawitSemasaModelViewEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PalmPriceEdit(tbl_HargaSawitSemasaModelViewEdit hargaSawitSemasaModelViewEdit)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            try
            {
                var palmPriceMaxData = db.tbl_HargaSawitRange
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderByDescending(o => o.fld_RangeHargaUpper).FirstOrDefault();

                if (ModelState.IsValid && palmPriceMaxData.fld_RangeHargaUpper >= hargaSawitSemasaModelViewEdit.fld_HargaSemasa)
                {
                    var palmPriceData =
                        db.tbl_HargaSawitSemasa.SingleOrDefault(x => x.fld_ID == hargaSawitSemasaModelViewEdit.fld_ID);

                    palmPriceData.fld_HargaSemasa = hargaSawitSemasaModelViewEdit.fld_HargaSemasa;

                    var getIncentive = db.tbl_HargaSawitRange
                        .SingleOrDefault(x =>
                            x.fld_RangeHargaLower <= hargaSawitSemasaModelViewEdit.fld_HargaSemasa &&
                            x.fld_RangeHargaUpper >= hargaSawitSemasaModelViewEdit.fld_HargaSemasa &&
                            x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).fld_Insentif;

                    palmPriceData.fld_Insentif = getIncentive;

                    db.SaveChanges();

                    string appname = Request.ApplicationPath;
                    string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                    var lang = Request.RequestContext.RouteData.Values["lang"];

                    if (appname != "/")
                    {
                        domain = domain + appname;
                    }

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResCorp.msgUpdate,
                        status = "success",
                        checkingdata = "0",
                        method = "4",
                        div = "palmPriceDetails",
                        rootUrl = domain,
                        action = "_PalmPriceSearch",
                        controller = "DataEntry",
                        paramName = "JenisTanamanList",
                        paramValue = palmPriceData.fld_JnsTnmn,
                        paramName2 = "MonthList",
                        paramValue2 = palmPriceData.fld_Bulan,
                        paramName3 = "YearList",
                        paramValue3 = palmPriceData.fld_Tahun
                    });
                }

                else
                {
                    return Json(new
                    {
                        success = false,
                        msg = GlobalResCorp.msgErrorData,
                        status = "danger",
                        checkingdata = "0"
                    });
                }
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = false,
                    msg = GlobalResCorp.msgError,
                    status = "danger",
                    checkingdata = "0"
                });
            }

            finally
            {
                db.Dispose();
            }
        }
    }
}