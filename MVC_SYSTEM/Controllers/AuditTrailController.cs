using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.Models;
using iTextSharp.text;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
    public class AuditTrailController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private MVC_SYSTEM_ModelsCorporate db3 = new MVC_SYSTEM_ModelsCorporate();
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetIdentity getidentity = new GetIdentity();
        GetWilayah getwilyah = new GetWilayah();
        GetNSWL GetNSWL = new GetNSWL();
        GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        GetTriager GetTriager = new GetTriager();

        //aini commented - original code
        // GET: AuditTrail
        //public ActionResult Index()
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
        //    //bool bln1tri, bln2tri, bln3tri, bln4tri, bln5tri, bln6tri, bln7tri, bln8tri, bln9tri, bln10tri, bln11tri, bln12tri = false;

        //    ViewBag.AuditTrail = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        //mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x=> wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //    }

        //    var getaudittrailwilayahID = db.vw_AuditTrail.Where(x => x.fld_Thn == year && wlyhid.Contains((int)x.fld_WilayahID)).Select(s=>s.fld_WilayahID).Distinct().ToList();

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if(i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    //ViewBag.getlastupload = db.tbl_SevicesProcessHistory.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_ID).Select(s => s.fld_DTEndProcess).Take(1).FirstOrDefault();
        //    ViewBag.YearList = yearlist;
        //    ViewBag.Year = year;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.LadangID = 0;
        //    ViewBag.getaudittrailwilayahID = getaudittrailwilayahID;
        //    ViewBag.ladangvalue = LadangID;
        //    ViewBag.NegaraID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.DataCount = getaudittrailwilayahID.Count();

        //    return View();
        //}

        //Aini update 16052023
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
            ViewBag.AuditTrail = "class = active";

            ViewBag.AuditList = new SelectList(db3.tblMenuLists.Where(x => x.fldDeleted == false && x.fld_Flag == "AuditTrail").OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();

            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Index(int ? YearList, int WilayahIDList, int LadangIDList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    var getaudittrailwilayahID = new List<int?>();

        //    ViewBag.AuditTrail = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        //mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
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
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
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


        //    if (WilayahIDList == 0)
        //    {
        //        getaudittrailwilayahID = db3.vw_AuditTrail.Where(x => x.fld_Thn == YearList && wlyhid.Contains((int)x.fld_WilayahID)).Select(s => s.fld_WilayahID).Distinct().ToList();
        //    }
        //    else
        //    {
        //        getaudittrailwilayahID = db3.vw_AuditTrail.Where(x => x.fld_Thn == YearList && x.fld_WilayahID == WilayahIDList).Select(s => s.fld_WilayahID).Distinct().ToList();
        //    }

        //    //ViewBag.getlastupload = db.tbl_SevicesProcessHistory.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_ID).Select(s => s.fld_DTEndProcess).Take(1).FirstOrDefault();
        //    ViewBag.YearList = yearlist; // list dalam dropdown
        //    ViewBag.Year = YearList; // year yg user select
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.LadangID = LadangIDList;
        //    ViewBag.getaudittrailwilayahID = getaudittrailwilayahID;
        //    ViewBag.ladangvalue = LadangID;
        //    ViewBag.NegaraID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.DataCount = getaudittrailwilayahID.Count();

        //    return View();
        //}

        //Aini update 16052023

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User, Resource,Viewer")]
        public ActionResult Index(int AuditList)
        {
            string action = "";
            var getreport = db3.tblMenuLists.Find(AuditList);
            action = getreport.fld_Val;

            return RedirectToAction(action, "AuditTrail");
        }
        //Aini update 16052023
        public ActionResult AuditTrail2()
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
            //bool bln1tri, bln2tri, bln3tri, bln4tri, bln5tri, bln6tri, bln7tri, bln8tri, bln9tri, bln10tri, bln11tri, bln12tri = false;

            ViewBag.AuditTrail = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            var getaudittrailwilayahID = db.vw_AuditTrail.Where(x => x.fld_Thn == year && wlyhid.Contains((int)x.fld_WilayahID)).Select(s => s.fld_WilayahID).Distinct().ToList();

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

            var getuserlist = db.tblUserAuditTrails.ToList();

            //ViewBag.getlastupload = db.tbl_SevicesProcessHistory.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_ID).Select(s => s.fld_DTEndProcess).Take(1).FirstOrDefault();
            ViewBag.YearList = yearlist;
            ViewBag.Year = year;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.LadangID = 0;
            ViewBag.getaudittrailwilayahID = getaudittrailwilayahID;
            ViewBag.ladangvalue = LadangID;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.DataCount = getuserlist.Count();
            ViewBag.getuserlist = getuserlist;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Aini update 16052023
        public ActionResult AuditTrail2(int? YearList, int WilayahIDList, int LadangIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            var getaudittrailwilayahID = new List<int?>();

            ViewBag.AuditTrail = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
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
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
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


            if (WilayahIDList == 0)
            {
                getaudittrailwilayahID = db3.vw_AuditTrail.Where(x => x.fld_Thn == YearList && wlyhid.Contains((int)x.fld_WilayahID)).Select(s => s.fld_WilayahID).Distinct().ToList();
            }
            else
            {
                getaudittrailwilayahID = db3.vw_AuditTrail.Where(x => x.fld_Thn == YearList && x.fld_WilayahID == WilayahIDList).Select(s => s.fld_WilayahID).Distinct().ToList();
            }

            //ViewBag.getlastupload = db.tbl_SevicesProcessHistory.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_ID).Select(s => s.fld_DTEndProcess).Take(1).FirstOrDefault();
            ViewBag.YearList = yearlist; // list dalam dropdown
            ViewBag.Year = YearList; // year yg user select
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.LadangID = LadangIDList;
            ViewBag.getaudittrailwilayahID = getaudittrailwilayahID;
            ViewBag.ladangvalue = LadangID;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.DataCount = getaudittrailwilayahID.Count();

            return View();
        }
        //Aini add 16052023
        public ActionResult UserAuditTrail()
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            var getdata = db.tblUserAuditTrails.Where(x => x.fld_CreatedDT.Value.Year == currentYear && x.fld_CreatedDT.Value.Month == currentMonth).Join(db.tblUsers,
                c => c.fld_CreatedBy,
                cm => cm.fldUserID,
                (c, cm) => new
                {
                    username = cm.fldUserFullName,
                    date = c.fld_CreatedDT.ToString(),
                    createdby = c.fld_CreatedBy,
                    activity = c.fld_UserActivity
                }).OrderByDescending(o => o.date).ToList();
            return Json(getdata);
        }
        //Aini update 16052023
        public ActionResult AuditTrail3()
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
            //bool bln1tri, bln2tri, bln3tri, bln4tri, bln5tri, bln6tri, bln7tri, bln8tri, bln9tri, bln10tri, bln11tri, bln12tri = false;

            ViewBag.AuditTrail = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //yana add 030823
            List<SelectListItem> SyarikatList = new List<SelectListItem>();
            SyarikatList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.SyarikatList = SyarikatList;
            // end here 030823

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            var getaudittrailwilayahID = db.vw_AuditTrail.Where(x => x.fld_Thn == year && wlyhid.Contains((int)x.fld_WilayahID)).Select(s => s.fld_WilayahID).Distinct().ToList();

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

            //ViewBag.getlastupload = db.tbl_SevicesProcessHistory.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_ID).Select(s => s.fld_DTEndProcess).Take(1).FirstOrDefault();
            ViewBag.YearList = yearlist;
            ViewBag.Year = year;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.LadangID = 0;
            ViewBag.getaudittrailwilayahID = getaudittrailwilayahID;
            ViewBag.ladangvalue = LadangID;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.DataCount = getaudittrailwilayahID.Count();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Aini update 16052023
        // yana update 070823 - add string SyarikatList
        public ActionResult AuditTrail3(int? YearList, string SyarikatList, int WilayahIDList, int LadangIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            var getaudittrailwilayahID = new List<int?>();

            ViewBag.AuditTrail = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //yana add 030823
            List<SelectListItem> SyarikatList2 = new List<SelectListItem>();
            SyarikatList2 = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.SyarikatList = SyarikatList2;
            // end here 030823

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
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
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResAuditTrail.sltAll, Value = "0" }));
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


            if (WilayahIDList == 0)
            {
                getaudittrailwilayahID = db3.vw_AuditTrail.Where(x => x.fld_Thn == YearList && wlyhid.Contains((int)x.fld_WilayahID)).Select(s => s.fld_WilayahID).Distinct().ToList();
            }
            // yana add 100823
            //else if (LadangIDList == 0)
            //{
            //    getaudittrailwilayahID = db3.vw_AuditTrail.Where(x => x.fld_Thn == YearList && x.fld_WilayahID == WilayahIDList && x.fld_CostCentre == SyarikatList).Select(s => s.fld_WilayahID).Distinct().ToList();
            //}
            // end here 100823
            else
            {
                getaudittrailwilayahID = db3.vw_AuditTrail.Where(x => x.fld_Thn == YearList && x.fld_WilayahID == WilayahIDList).Select(s => s.fld_WilayahID).Distinct().ToList();
            }

            //ViewBag.getlastupload = db.tbl_SevicesProcessHistory.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_ID).Select(s => s.fld_DTEndProcess).Take(1).FirstOrDefault();
            ViewBag.YearList = yearlist; // list dalam dropdown
            ViewBag.Year = YearList; // year yg user select
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.LadangID = LadangIDList;
            ViewBag.getaudittrailwilayahID = getaudittrailwilayahID;
            ViewBag.ladangvalue = LadangID;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.DataCount = getaudittrailwilayahID.Count();

            return View();
        }
        // yana add 070923 - string SyarikatList
        public ActionResult AuditTrailDetail(int wilid, int? ladcd, int year, int bil, string filesourcepath, string SyarikatList)
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            string stringyear = year.ToString();

            stringyear = stringyear.Substring(2, 2);
            // yana add 070923 - x.fld_CostCentre == SyarikatList
            var AuditTrailReport = db3.vw_AuditTrail.Where(x => x.fld_Thn == year && x.fld_WilayahID == wilid && x.fld_LadangID == ladcd && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_LdgName).ToList();

            ViewBag.bil = bil;
            ViewBag.FileSource = filesourcepath;
            ViewBag.Year = stringyear;

            ViewBag.bln1tri = GetTriager.BlnTriger(1);
            ViewBag.bln2tri = GetTriager.BlnTriger(2);
            ViewBag.bln3tri = GetTriager.BlnTriger(3);
            ViewBag.bln4tri = GetTriager.BlnTriger(4);
            ViewBag.bln5tri = GetTriager.BlnTriger(5);
            ViewBag.bln6tri = GetTriager.BlnTriger(6);
            ViewBag.bln7tri = GetTriager.BlnTriger(7);
            ViewBag.bln8tri = GetTriager.BlnTriger(8);
            ViewBag.bln9tri = GetTriager.BlnTriger(9);
            ViewBag.bln10tri = GetTriager.BlnTriger(10);
            ViewBag.bln11tri = GetTriager.BlnTriger(11);
            ViewBag.bln12tri = GetTriager.BlnTriger(12);

            return View(AuditTrailReport);
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
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        // yana add 070823
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
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }
        // end here 070823

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
