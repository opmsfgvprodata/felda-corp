using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.ModelsCorporate;
using tbl_SevicesProcess = MVC_SYSTEM.Models.tbl_SevicesProcess;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Admin 1,Admin 2,Admin 3")]
    public class UploadController : Controller
    {
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private GetIdentity getidentity = new GetIdentity();
        errorlog geterror = new errorlog();
        GetWilayah getwilyah = new GetWilayah();
        GetNSWL GetNSWL = new GetNSWL();
        GetConfig GetConfig = new GetConfig();
        // GET: Upload
        public ActionResult Index()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            ViewBag.Upload = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResUpload.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResUpload.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResUpload.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
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

            ViewBag.YearList = yearlist;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.ProcessList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "processlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string ProcessList, int MonthList, int YearList, int WilayahIDList, int LadangIDList)
        {
            try
            {
                int[] wlyhid = new int[] { };
                int year = timezone.gettimezone().Year;
                int drpyear = 0;
                int drprangeyear = 0;
                //string mywlyid = "";
                int? NegaraID = 0;
                int? SyarikatID = 0;
                int? WilayahID = 0;
                int? LadangID = 0;
                int? getuserid = getidentity.ID(User.Identity.Name);
                var getaudittrailwilayahID = new List<int?>();
                bool success = false;
                string status = "";
                string msg = "";

                ViewBag.Upload = "class = active";

                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

                List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
                List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

                if (WilayahID == 0 && LadangID == 0)
                {
                    wlyhid = getwilyah.GetWilayahID(SyarikatID);
                    //mywlyid = String.Join("", wlyhid); ;
                    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                    WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResUpload.sltAll, Value = "0" }));
                    if (WilayahIDList == 0)
                    {
                        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    }
                    else
                    {
                        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    }
                    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResUpload.sltAll, Value = "0" }));
                }
                else if (WilayahID != 0 && LadangID == 0)
                {
                    //mywlyid = String.Join("", WilayahID); ;
                    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                    if (WilayahIDList == 0)
                    {
                        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    }
                    else
                    {
                        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    }
                    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResUpload.sltAll, Value = "0" }));
                }
                else if (WilayahID != 0 && LadangID != 0)
                {
                    //mywlyid = String.Join("", WilayahID); ;
                    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
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

                int? getclientid = getidentity.ClientID(User.Identity.Name);
                var servicesname = db.tbl_ServicesList.Where(x => x.fld_ClientID == getclientid).Select(s => s.fld_ServicesName).FirstOrDefault();

                var deleteprocess = db.tbl_SevicesProcess.Where(x => x.fld_ClientID == getclientid && x.fld_Flag == 0).FirstOrDefault();
                if (deleteprocess != null)
                {
                    //var deleteitem = db.tbl_SevicesProcess.Find(deleteprocess);
                    db.tbl_SevicesProcess.Remove(deleteprocess);
                    db.SaveChanges();
                }

                int categoryselected = 0;
                int categoryselectedvalue = 0;

                if (WilayahIDList == 0 && LadangIDList == 0)
                {
                    categoryselected = 1;
                    categoryselectedvalue = 0;
                }
                else if (WilayahIDList != 0 && LadangIDList == 0)
                {
                    categoryselected = 2;
                    categoryselectedvalue = WilayahIDList;
                }

                else if (WilayahIDList != 0 && LadangIDList != 0)
                {
                    categoryselected = 3;
                    categoryselectedvalue = LadangIDList;
                }

                var getexistingprocess = db.tbl_SevicesProcess.Where(x => x.fld_ServicesName == servicesname).FirstOrDefault();

                if (getexistingprocess == null)
                {
                    ModelsCorporate.tbl_SevicesProcess tbl_SevicesProcess = new ModelsCorporate.tbl_SevicesProcess();
                    tbl_SevicesProcess.fld_ProcessName = ProcessList;
                    tbl_SevicesProcess.fld_ServicesName = servicesname;
                    tbl_SevicesProcess.fld_Flag = 1;
                    tbl_SevicesProcess.fld_UserID = getidentity.ID(User.Identity.Name);
                    tbl_SevicesProcess.fld_DTProcess = timezone.gettimezone();
                    tbl_SevicesProcess.fld_Year = YearList;
                    tbl_SevicesProcess.fld_Month = MonthList;
                    tbl_SevicesProcess.fld_UplSelCat = categoryselected;
                    tbl_SevicesProcess.fld_SelCatVal = categoryselectedvalue;
                    tbl_SevicesProcess.fld_ClientID = getclientid;
                    tbl_SevicesProcess.fld_NegaraID = NegaraID;
                    tbl_SevicesProcess.fld_SyarikatID = SyarikatID;
                    tbl_SevicesProcess.fld_ProcessPercentage = 0;
                    db.tbl_SevicesProcess.Add(tbl_SevicesProcess);
                    db.SaveChanges();

                    StartService(servicesname, 100);

                    success = true;
                    status = "success";
                    msg = "Upload process will be starting";

                }
                else
                {
                    msg = "Upload already started please wait until finish";
                    status = "warning";
                    success = true;
                }

                return Json(new { success = success, msg = msg, status = status });
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = "Sila laporkan kepada Prodata.", status = "danger" });
            }
            
        }
        public void StartService(string serviceName, int timeoutMilliseconds)
        {
            try
            {
                ServiceController sc = new ServiceController(serviceName);
                if (sc.Status != ServiceControllerStatus.Running && sc.Status != ServiceControllerStatus.StartPending)
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }
        }

        public JsonResult GetLadang(int WilayahID)
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
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db2.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}