using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsCorporate;
//added by kamalia 24/11/21
using MVC_SYSTEM.ModelsEstate;
using MVC_SYSTEM.ViewingModelsOPMS;

//Added by Shazana 3/4/2023
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;

namespace MVC_SYSTEM.Controllers
{
    public class ApplicationSupportController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetWilayah getwilyah = new GetWilayah();
        private DatabaseAction DatabaseAction = new DatabaseAction();
        private GetIdentity GetIdentity = new GetIdentity();
        private GlobalFunction GlobalFunction = new GlobalFunction(); //Added by Shazana 1/8/2023
        private SendEmailNotification SendEmailNotification = new SendEmailNotification();
        //added by kamalia 24/11/21
        Connection Connection = new Connection();

        //Added by Shazana 3/4/2023
        private GeneralClass GeneralClass = new GeneralClass();
        private GetIDDataFromDatabase GetIDDataFromDatabase = new GetIDDataFromDatabase();

        private errorlog geterror = new errorlog();
        // GET: ApplicationSupportS
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        //Modified by Shazana 23/3/2023
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult Index()
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.ApplicationSupport = "class = active";


            //Commented by Shazana 6/3/2023
            //ViewBag.ApplicationSupportList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "applicationsupportlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc), "fldOptConfValue", "fldOptConfDesc");

            //Added by Shazana 6/3/2023
            var ApplicationSupportListItem = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "applicationsupportlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderByDescending(o => o.fldOptConfDesc).ToList();
            List<SelectListItem> ApplicationSupportList1 = new List<SelectListItem>();
            var checkingRole = db.tblUsers.Where(x => x.fldUserID == getuserid).FirstOrDefault();

            foreach (var filterrole in ApplicationSupportListItem)
            {
                //Modified by Shazana 23/3/2023
                if (filterrole.fldOptConfValue == "ApplicationSupportRegionFirst" && (checkingRole.fldRoleID == 10 || checkingRole.fldRoleID == 1 || checkingRole.fldRoleID == 2 || checkingRole.fldRoleID == 3))
                { ApplicationSupportList1.Insert(0, (new SelectListItem { Text = filterrole.fldOptConfDesc, Value = filterrole.fldOptConfValue })); }
                if (filterrole.fldOptConfValue == "ApplicationSupportRegionSecond" && (checkingRole.fldRoleID == 1 || checkingRole.fldRoleID == 2 || checkingRole.fldRoleID == 3 || checkingRole.fldRoleID == 4))
                { ApplicationSupportList1.Insert(0, (new SelectListItem { Text = filterrole.fldOptConfDesc, Value = filterrole.fldOptConfValue })); }
            }
            ViewBag.ApplicationSupportList = ApplicationSupportList1;
            ViewBag.CountApplicationSupportList = ApplicationSupportList1.Count(); //Added by Shazana 30/3/2023
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User")]
        public ActionResult Index(string ApplicationSupportList)
        {
            return RedirectToAction(ApplicationSupportList, "ApplicationSupport");
        }

        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User")]
        public ActionResult ApplicationSupportRegion()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User")]
        public ActionResult ApplicationSupportRegion(int WilayahIDList, int LadangIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone();

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            if(WilayahIDList == 0)
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            }
            else
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID,SyarikatID,WilayahIDList, getdate.Month, getdate.Year);
            }
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            ViewBag.GetView = 0;
            return View();
        }
        
        public ActionResult ApplicationSupportRegionDetail(List<long> eachid)
        {
            var getdata = db.vw_PermohonanKewangan.Where(x=> eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
            ViewBag.getgmstatus = getdata.Where(x => x.fld_SokongWilGM_Status == 1).Count();
            return View(getdata);
        }

        // modify by kamalia 2/12/21
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2")]
        public ActionResult ApplicationSupportRegionGm()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost] //modified by kamalia 1/2/2022
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2")]
        public ActionResult ApplicationSupportRegionGm(int WilayahIDList, int LadangIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone();
            //int LadangIDList = 0;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                ////mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //  mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }

            //    WilayahID = getwilyah.GetWilayahID3(LadangIDList);

            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            if (WilayahIDList == 0)
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            }
            else
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, WilayahIDList, getdate.Month, getdate.Year);
            }
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            //modified by Kamalia 2/3/2022
            ViewBag.WilayahID = WilayahIDList;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            //ViewBag.Month = 8;
            //ViewBag.Year = 2020;
            ViewBag.GetView = 0;
            return View();
        }


        public ActionResult ApplicationSupportRegionGMDetail(List<long> eachid, int NegaraID, int SyarikatID, int LadangID, int Month, int Year)
        {
            bool matchtotal = false;
            var getdata = db.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
            var getcoundata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1).Count();
            if (getdata.Count() == getcoundata)
            {
                matchtotal = true;
            }

            ViewBag.matchtotal = matchtotal;
            return View(getdata);
        }

        
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult ApplicationSupportRegionHQ()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult ApplicationSupportRegionHQ(int WilayahIDList, int LadangIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone();
            //int LadangIDList = 0;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                ////mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                ////mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                ////mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }

            WilayahID = getwilyah.GetWilayahID3(LadangIDList);

            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            if (WilayahIDList == 0)
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            }
            else
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, WilayahIDList, getdate.Month, getdate.Year);
            }
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.LadangID = LadangIDList;
            ViewBag.WilayahID = WilayahID;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            //ViewBag.Month = 8;
            //ViewBag.Year = 2020;
            ViewBag.GetView = 0;
            return View();
        }
        public ActionResult ApplicationSupportRegionHQDetail(List<long> eachid, int NegaraID, int SyarikatID, int LadangID, int Month, int Year)
        {
            bool matchtotal = false;
            var getdata = db.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
            var getcoundata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1 && x.fld_SokongWilGM_Status == 1).Count();
            if (getdata.Count() == getcoundata)
            {
                matchtotal = true;
            }

            ViewBag.matchtotal = matchtotal;
            return View(getdata);
        }
        // end modify by kamalia 2/12/21

        public string ApplicationSupportHistoryDetail(long SPWID)
        {
            string returndetail = "";
            string fontcolor = "";
            var getdata = db.tblSokPermhnWangHisActions.Where(x => x.fldHisSPWID == SPWID).OrderBy(o => o.fldHisDT).ToArray();
            //Modified by Shazana 27/4/2023
            var getdatatblSokPermhnWang = db.tbl_SokPermhnWang.Where(x => x.fld_ID == SPWID).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == getdatatblSokPermhnWang.fld_LadangID && x.fld_NegaraID == getdatatblSokPermhnWang.fld_NegaraID && x.fld_SyarikatID == getdatatblSokPermhnWang.fld_SyarikatID && x.fld_WlyhID == getdatatblSokPermhnWang.fld_WilayahID).FirstOrDefault();
            string SemakanNama = "";
            string SokonganNama = "";
            string LulusNama = "";
            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                SemakanNama = " (Pengurus FELDA) ";
                SokonganNama = " (Pegawai Kewangan) ";
                LulusNama = " (Pegawai Wilayah FELDA) ";
            }
            else
            {
                SemakanNama = " (Pengurus FELDA) "; //Added by Shazana 6/6/2023
                SokonganNama = " (Pegawai Wilayah FPM) "; //Added by Shazana 6/6/2023
                LulusNama = " (Pegawai Wilayah FELDA) "; //Added by Shazana 6/6/2023
            }

            if (getdata != null)
            {
                foreach (var data in getdata)
                {

                    if (data.fldHisDesc == "Telah Ditolak" || data.fldHisDesc == "Urus Niaga Dibuka Semula")
                    {
                        fontcolor = "red";
                    }
                    else
                    {
                        fontcolor = "green";
                    }

                    string roleNama = "";

                    if (data.fldHisDesc.Contains("Telah Disemak"))
                    { roleNama = SemakanNama; }
                    else if (data.fldHisDesc.Contains("Telah Disokong"))
                    {
                        roleNama = SokonganNama;
                    }
                    else if (data.fldHisDesc.Contains("Telah Diluluskan"))
                    {
                        roleNama = LulusNama;
                    }

                    //Modified by Shazana 1/8/2023
                    returndetail = returndetail + "<font color=\"" + fontcolor + "\"><p class=\"specialClass\">" + data.fldHisDesc + " oleh " + getidentity.MyNameFullName(data.fldHisUserID) + "( " + GlobalFunction.getJawatanName(data.fldHisUserID, GetEstateDetail.fld_NegaraID, GetEstateDetail.fld_SyarikatID) + " )" + " pada " + data.fldHisDT + "</p>";
                }
            }
            return returndetail;
        }
        //added by kamalia 24/11/21
        public JsonResult UpdateData(long DataID, string UpdateFlag, int NegaraId, int SyarikatId, int WilayahId, decimal JumlahWang, int Month, int Year, string NoAcc, string NoCIT, string SebabTolak)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            string subject = "";
            string msg = "";
            string DepartmentHR = "";
            string DepartmentAM = "";
            string DepartmentCL = "";
            string DepartmentMGR = "";
            string[] to = new string[] { };
            List<string> tolist = new List<string>();
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            //bool matchtotal = false;

            var GetEstate = db.tbl_SokPermhnWang.Where(x => x.fld_ID == DataID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == GetEstate.fld_LadangID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId).FirstOrDefault();

            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_FELDA";
                DepartmentAM = "AM_FINANCE_APPROVAL_FELDA";
                DepartmentCL = "CL_FINANCE_APPROVAL_FELDA";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FELDA";
            }
            else
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_FPM";
                DepartmentAM = "AM_FINANCE_APPROVAL_FPM";
                DepartmentCL = "CL_FINANCE_APPROVAL_FPM";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FPM";
            }

            switch (UpdateFlag)
            {
                case "SemakWil":
                    DescStatus = "Telah Disemak";
                    //DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 1, 0, 0, 0, 0, 0, "SemakWil", getuserid, getdatetime, PDP, CIT, NoAcc, NoGL, NoCIT, Manual);
                    DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 1, 0, 0, 0, 0, 0, "SemakWil", getuserid, getdatetime, NoAcc, NoCIT);
                    DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, DataID, 1, "");

                    subject = "Sokongan Permohonan Gaji";

                    //var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == DepartmentHR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    //msg += "<p>Tn/Pn " + ToEmail.fldName + ",</p>";
                    msg += "<p>Tuan/Puan (RGM), </p>";
                    msg += "<p>Sokongan permohonan gaji (Gaji Pekerja Buruh) untuk kelulusan diperlukan dari pihak Tuan/Puan (RGM). Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan (RM)</th><th>Disahkan Oleh</th><th>Disemak Oleh</th><th>Waktu Disemak</th><th>Pautan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td>" + GetEstateDetail.fld_LdgCode + "</td><td>" + GetEstateDetail.fld_LdgName + "</td><td>" + JumlahWang + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td><a href=\"" + Url.Action("ApplicationSupportRegionGm", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emailtolist = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == DepartmentHR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailtolist != null)
                    {
                        foreach (var toemail in emailtolist)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var emailcclist = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentAM && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentCL && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailcclist != null)
                    {
                        foreach (var ccemail in emailcclist)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var emailbcclist = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist != null)
                    {
                        foreach (var bccemail in emailbcclist)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, ToEmail.fldEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
                case "TolakWil":
                    DescStatus = "Telah Ditolak";
                    //DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 0, 1, 0, 0, 0, 0, "TolakWil", getuserid, getdatetime, 0, 0, "", "", "", 0);
                    DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 0, 1, 0, 0, 0, 0, "TolakWil", getuserid, getdatetime, "", "");
                    DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, DataID, 1, SebabTolak);

                    subject = "Penolakkan Permohonan Gaji";

                    //var GetLdgID = db.tbl_SokPermhnWang.Where(x => x.fld_ID == DataID &&  x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
                    //var GetLdgDetail = db.tbl_Ladang.Where(x => x.fld_ID == GetLdgID.fld_LadangID && x.fld_WlyhID == WilayahId).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    //Modified by Shazana 28/4/2023
                    msg += "<p>Dukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh Pengurus. Mohon pihak ladang buat semakan kembali. Keterangan seperti dibawah :-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Sebab</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + JumlahWang + "</td><td align=\"center\">" + SebabTolak + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == GetEstateDetail.fld_ID && ((x.fldDepartment == DepartmentCL && x.fldCategory == "TO") || x.fldDepartment == DepartmentAM && x.fldCategory == "TO") && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmailT != null)
                    {
                        foreach (var toemailt in ToEmailT)
                        {
                            tolist.Add(toemailt.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var CcEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == GetEstateDetail.fld_ID)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmailT != null)
                    {
                        foreach (var ccemailt in CcEmailT)
                        {
                            cclist.Add(ccemailt.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmailT != null)
                    {
                        foreach (var bccemailt in BccEmailT)
                        {
                            bcclist.Add(bccemailt.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetEstateDetail.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, null, bcc);

                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime, SebabTolak = SebabTolak });
        }

        public JsonResult AcknowlagdeEmail(int NegaraId, int SyarikatId, int WilayahId, int Month, int Year)
        {
            string msg2 = "";
            string statusmsg = "";
            bool status = false;
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            DateTime getdatetime = timezone.gettimezone();
            var getcoundata = db.vw_PermohonanWang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1).Count();
            if (getcoundata > 0)
            {
                try
                {
                    string[] to = new string[] { };
                    List<string> tolist = new List<string>();
                    string[] cc = new string[] { };
                    List<string> cclist = new List<string>();
                    string[] bcc = new string[] { };
                    List<string> bcclist = new List<string>();
                    string subject = "";
                    string msg = "";
                    string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
                    var getJumlahKeseluruhanPermohonan = db.vw_PermohonanWang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_JumlahPermohonan).Sum();

                    subject = "Sokongan Permohonan Wang";

                    var GetSentToGM = db2.tblUsers.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldRoleID == 4).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Tn/Pn " + GetSentToGM.fldUserShortName + ",</p>";
                    msg += "<p>Sokongan permohonan kewangan (Gaji Pekerja Buruh) untuk kelulusan diperlukan dari pihak Tn/Pn. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Jumlah Permohonan(RM)</th><th>Disemak Oleh</th><th>Waktu Disemak</th><th>Pautan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + NamaWilayah + "</td><td align=\"center\">" + getJumlahKeseluruhanPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td><a href=\"" + Url.Action("ApplicationSupportRegionGm", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emaillist = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDeleted == false).ToList();
                    tolist.Add(GetSentToGM.fldUserEmail);
                    to = tolist.ToArray();
                    var emailbcclist1 = emaillist.Where(x => x.fldCategory == "BCC" && x.fldDepartment == "GENERAL").Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist1 != null)
                    {
                        foreach (var bccemail in emailbcclist1)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    if (SendEmailNotification.SendEmail(subject, msg, to, cc, bcc))
                    {
                        msg2 = "Email telah dihantar kepada GM Wilayah " + NamaWilayah;
                        statusmsg = "success";
                        status = true;
                    }
                    else
                    {
                        msg2 = "Email gagal dihantar kepada GM Wilayah " + NamaWilayah;
                        statusmsg = "warning";
                        status = true;
                    }
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    msg2 = "Email gagal untuk dihantar";
                    statusmsg = "warning";
                    status = true;
                }
            }
            else
            {
                msg2 = "Sila buat semakan terlebih dahulu sebelum email dihantar kepada GM Wilayah";
                statusmsg = "warning";
                status = true;
            }
            return Json(new { msg = msg2, statusmsg = statusmsg, status = status });
        }

        //add by kamalia 2/12/21
        public JsonResult UpdateDataGM(int LdgID, string UpdateFlag, int Month, int Year, int NegaraId, int SyarikatId, int WilayahId, string SebabTolak)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            string subject = "";
            string msg = "";
            string DepartmentRMGR = "";
            string DepartmentAM = "";
            string DepartmentMGR = "";
            string DepartmentCL = "";
            string DepartmentHR = "";
            string[] to = new string[] { };
            List<string> tolist = new List<string>();
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            int? SemakWilById;
            //var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s=>s.fld_ID).ToList();
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_LadangID == LdgID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_ID).ToList();
            //var getJumlahKeseluruhanPermohonan = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_JumlahPermohonan).Sum();

            var GetEstate = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LdgID && x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == LdgID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId).FirstOrDefault();

            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                DepartmentRMGR = "RMGR_FINANCE_APPROVAL_FELDA";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FELDA";
                DepartmentAM = "AM_FINANCE_APPROVAL_FELDA";
                DepartmentCL = "CL_FINANCE_APPROVAL_FELDA";
                DepartmentHR = "HR_FINANCE_APPROVAL_FELDA";
            }
            else
            {
                DepartmentRMGR = "RMGR_FINANCE_APPROVAL_FPM";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FPM";
                DepartmentAM = "AM_FINANCE_APPROVAL_FPM";
                DepartmentCL = "CL_FINANCE_APPROVAL_FPM";
                DepartmentHR = "HR_FINANCE_APPROVAL_FPM";
            }

            switch (UpdateFlag)
            {
                case "SokongGMWil":
                    DescStatus = "Telah Disokong";
                    DatabaseAction.UpdateDataTotblSokPermhnWangGM2(LdgID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2, "");
                    }
                    //Modified by Shazana 20/4/2023 
                    subject = "OPMS FELDA - Kelulusan Perladangan bagi  Semakan Permohonan Gaji";

                    SemakWilById = GetEstate.fld_SemakWil_By;
                    //var GetSentToFinManager = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstateDetail.fld_NegaraID && x.fldSyarikatID == GetEstateDetail.fld_SyarikatID && x.fldDepartment == DepartmentFGMR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();

                    //var GetSentToWil1 = db2.tbl_Wilayah.Where(x => x.fld_ID == DataID && x.fld_SyarikatID == SyarikatId).FirstOrDefault();
                    //var GetSentToHQ = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == GetSentToWil1.fld_ApprovalZone && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
                    //var GetSentCC = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "CCFINANCE" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Tuan/Puan (Unit Kewangan),</p>";
                    msg += "<p>Mohon kelulusan permohonan gaji (Gaji Pekerja Buruh) dari pihak Tuan/Puan (Unit Kewangan). Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    //Modified by Shazana 20/2/2023
                    //msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Disemak Oleh</th><th>Waktu Tindakan</th><th>Disokong Oleh</th><th>Waktu Tindakan</th><th>Pautan</th>";
                    //Modified by Shazana 20/4/2023
                    int? SokongWilById = GetEstate.fld_SokongWilGM_By;
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Disemak Oleh</th><th>Waktu Disemak</th><th>Disahkan Oleh</th><th>Waktu Disahkan</th><th>Disokong Oleh</th><th>Waktu Disokong</th><th>Pautan</th>";

                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    //Modified by Shazana 20/4/2023
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SemakWilById) + "</td><td align=\"center\">" + GetEstate.fld_SemakWil_DT + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SokongWilById) + "</td><td align=\"center\">" + GetEstate.fld_SokongWilGM_DT + "</td><td align=\"center\"><a href=\"" + Url.Action("ApplicationSupportRegionHQ", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstateDetail.fld_NegaraID && x.fldSyarikatID == GetEstateDetail.fld_SyarikatID && x.fldDepartment == DepartmentRMGR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmail != null)
                    {
                        foreach (var toemail in ToEmail)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var CcEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentAM && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentCL && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentRMGR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmail != null)
                    {
                        foreach (var ccemail in CcEmail)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmail != null)
                    {
                        foreach (var bccemail in BccEmail)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetSentToFinManager.fldEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
                case "TolakGMWil":
                    DescStatus = "Telah Ditolak";
                    DatabaseAction.UpdateDataTotblSokPermhnWangGM2(LdgID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2, SebabTolak);
                    }

                    subject = "Penolakkan Permohonan Tolak";

                    //var GetSentToWil = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahId && x.fld_SyarikatID == SyarikatId).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    //Modified by Shazana 20/4/2023
                    //Modified by Shazana 28/4/2023
                    msg += "<p>Dukacita dimaklumkan, kelulusan permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh Perladangan Mohon pihak ladang buat semakan kembali. Keterangan seperti dibawah :-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Sebab</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + SebabTolak + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID && ((x.fldDepartment == DepartmentCL && x.fldCategory == "TO") || x.fldDepartment == DepartmentAM && x.fldCategory == "TO") && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmailT != null)
                    {
                        foreach (var toemail in ToEmailT)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var CcEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmailT != null)
                    {
                        foreach (var ccemail in CcEmailT)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmailT != null)
                    {
                        foreach (var bccemail in BccEmailT)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetEstateDetail.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);
                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime, SebabTolak = SebabTolak });
        }

        public JsonResult UpdateDataHQ(int DataID, string UpdateFlag, int Month, int Year, int NegaraId, int SyarikatId, int WilayahId, string SebabTolak)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string subject = "";
            string msg = "";
            string DepartmentHR = "";
            string DepartmentAM = "";
            string DepartmentMGR = "";
            string DepartmentCL = "";
            string DepartmentRMGR = "";
            string[] to = new string[] { };
            List<string> tolist = new List<string>();
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            //var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_ID).ToList();
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_ID).ToList();
            //var getJumlahKeseluruhanPermohonan = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_JumlahPermohonan).Sum();

            var GetEstate = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == DataID && x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == DataID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId).FirstOrDefault();

            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_FELDA";
                DepartmentAM = "AM_FINANCE_APPROVAL_FELDA";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FELDA";
                DepartmentCL = "CL_FINANCE_APPROVAL_FELDA";
                DepartmentRMGR = "RMGR_FINANCE_APPROVAL_FELDA";
            }
            else
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_FPM";
                DepartmentAM = "AM_FINANCE_APPROVAL_FPM";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FPM";
                DepartmentCL = "CL_FINANCE_APPROVAL_FPM";
                DepartmentRMGR = "RMGR_FINANCE_APPROVAL_FPM";
            }

            switch (UpdateFlag)
            {
                case "TerimaHQ":
                    DescStatus = "Telah Diterima";
                    DatabaseAction.UpdateDataTotblSokPermhnWangHQ2(DataID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 3, "");
                    }

                    subject = "Kelulusan Permohonan Gaji";

                    //var GetSentToLdg = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_ID == GetEstateDetail.fld_ID).Select(s => new { s.fld_LdgEmail, s.fld_LdgName }).FirstOrDefault();

                    //var GetSentToWil = db2.tbl_Wilayah.Where(x => x.fld_ID == DataID && x.fld_SyarikatID == SyarikatId).FirstOrDefault();
                    //var GetSentToGM = db2.tblUsers.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == DataID && x.fldRoleID == 4).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    msg += "<p>Sukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah diluluskan oleh Pengurus Kewangan. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Diluluskan Oleh</th><th>Waktu Diluluskan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == DataID && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "TO") || (x.fldDepartment == DepartmentAM && x.fldCategory == "TO") || (x.fldDepartment == DepartmentCL && x.fldCategory == "TO")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmail != null)
                    {
                        foreach (var toemail in ToEmail)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var CcEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentRMGR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == DataID)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmail != null)
                    {
                        foreach (var ccemail in CcEmail)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmail != null)
                    {
                        foreach (var bccemail in BccEmail)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetSentToLdg.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
                case "TolakHQ":
                    DescStatus = "Telah Ditolak";
                    DatabaseAction.UpdateDataTotblSokPermhnWangHQ2(DataID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 3, SebabTolak);
                    }
                    subject = "Penolakkan Permohonan Gaji";

                    //var GetSentToLdgT = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_ID == GetEstateDetail.fld_ID).Select(s => new { s.fld_LdgEmail, s.fld_LdgName }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    //Modified by Shazana 28/4/2023
                    msg += "<p>Dukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh Pengurus Kewangan. Mohon pihak ladang buat semakan kembali. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Sebab</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + SebabTolak + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == DataID && ((x.fldDepartment == DepartmentAM && x.fldCategory == "TO") || (x.fldDepartment == DepartmentCL && x.fldCategory == "TO")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmailT != null)
                    {
                        foreach (var toemail in ToEmailT)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var CcEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == DataID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentRMGR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmailT != null)
                    {
                        foreach (var ccemail in CcEmailT)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmailT != null)
                    {
                        foreach (var bccemail in BccEmailT)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetSentToLdgT.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime });
        }  //end by kamalia 2/12/21

        //added by kamalia 24/11/21
        public ActionResult TransactionListingRptSearch(int NegaraID, int SyarikatID, int WilayahID, int LadangID, int Month, int Year)
        {
            //int? NegaraID = NegaraId;
            //int? SyarikatID = SyarikatId;
            //int? WilayahID = WilayahId;
            
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_ViewingModels dbest = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_ModelsEstate dbest2 = new MVC_SYSTEM_ModelsEstate();

            ViewBag.MonthList = Month;
            ViewBag.YearList = Year;

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Ladang = db.tbl_Ladang
               .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
               .Select(s => s.fld_LdgName.Substring(0, 1).ToUpper() + s.fld_LdgName.Substring(1).ToLower())
               .FirstOrDefault();//modified by kamalia 24/12/21
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            //ViewBag.UserID = getuserid;
            //ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            //ViewBag.NamaPengurus = dbest2.tbl_Ladang
            //    .Where(x => x.fld_ID == LadangID)
            //    .Select(s => s.fld_Pengurus).Single();
            //ViewBag.NamaPenyelia = dbest2.tblUsers
            //    .Where(x => x.fldUserID == getuserid)
            //    .Select(s => s.fldUserFullName).Single();
            //ViewBag.IDPenyelia = getuserid;


            //if (Month == 0 && Year == 0)
            //{
            //    ViewBag.Message = GlobalResEstateDetail.msgChooseMonthYear;
            //    //return View();
            //}

            //else
            //{
            var GetCotribution = db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag3 == "Employee" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();

            var TransactionListingList = dbest.vw_RptSctran
                .Where(x => !GetCotribution.Contains(x.fld_KodAktvt) && x.fld_Month == Month &&
                            x.fld_Year == Year && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID)
                .OrderBy(o => new { o.fld_Kategori, o.fld_Amt }).ToList();

            if (!TransactionListingList.Any())
            {
                ViewBag.Message = "Tiada Rekod";
                return View();
            }

            ViewBag.UserID = getuserid;
            return View(TransactionListingList);
            //}
        }

        public ActionResult WorkerPaySheetRptSearch(int NegaraID, int SyarikatID, int WilayahID, int LadangID, string PaymentModeList, int Month, int Year)
        {
            //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
           
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_ViewingModels dbest = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_ViewingModels dbest2 = new MVC_SYSTEM_ViewingModels();
            List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();

            ViewBag.MonthList = Month;
            ViewBag.YearList = Year;
            //ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Ladang = db.tbl_Ladang
                  .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                  .Select(s => s.fld_LdgName.Substring(0, 1).ToUpper() + s.fld_LdgName.Substring(1).ToLower())
                  .FirstOrDefault();//modified by kamalia 24/12/21
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            List<SelectListItem> PaymentModeList2 = new List<SelectListItem>();
            PaymentModeList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "paymentmode" &&
            x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PaymentModeList2.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.PaymentModeSelection = PaymentModeList;
            //ViewBag.NamaPengurus = dbview2.tbl_Ladang
            //    .Where(x => x.fld_ID == LadangID)
            //    .Select(s => s.fld_Pengurus).Single();
            //ViewBag.NamaPenyelia = dbview2.tblUsers
            //    .Where(x => x.fldUserID == getuserid)
            //    .Select(s => s.fldUserFullName).Single();
            //ViewBag.IDPenyelia = getuserid;
            //List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();
            //JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", JnsPkjList).ToList();
            //JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            //ViewBag.JnsPkjList = JnsPkjList2;
            //ViewBag.Print = print;

            IOrderedQueryable<ViewingModelsOPMS.vw_PaySheetPekerja> salaryData;
            salaryData = dbest.vw_PaySheetPekerja
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_Year == Year && x.fld_Month == Month &&
                            x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(x => x.fld_Nama);

            foreach (var salary in salaryData)
            {
                var workerAdditionalContribution = dbest2.tbl_ByrCarumanTambahan
                    .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID);
                List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                foreach (var caruman in workerAdditionalContribution)
                {
                    CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                    carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                    carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                    carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                    carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                    carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                }
                PaySheetPekerjaList.Add(
                    new vw_PaySheetPekerjaCustomModel()
                    {
                        PaySheetPekerja = salary,
                        CarumanTambahan = carumanTambahanCustomModelList
                    });
            }
            if (PaySheetPekerjaList.Count == 0)
            {
                ViewBag.Message = "Tiada Rekod";
            }
            return View(PaySheetPekerjaList);

        }

        //public JsonResult GetLadang(int WilayahID) 
        //{
        //    List<SelectListItem> ladanglist = new List<SelectListItem>();

        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID2 = 0;
        //    int? LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

        //    if (getwilyah.GetAvailableWilayah(SyarikatID))
        //    {
        //        if (WilayahID == 0)
        //        {
        //            ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L  == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList(); //modified by kamalia 1/2/2022
        //        }
        //        else
        //        {
        //            ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList(); //modified by kamalia 1/2/2022
        //        }
        //    }

        //    return Json(ladanglist);
        //}

        //Added by Shazana 1/8/2023
        public JsonResult GetLadang(int WilayahID, string SyarikatID)
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

        //Added by Shazana 1/8/2023
        public JsonResult GetWilayah(string SyarikatID)
        {
            List<SelectListItem> wilayahlist = new List<SelectListItem>();
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0; //Modified by Shazana 1/8/2023
            int? WilayahID = 0; //Modified by Shazana 1/8/2023
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID, out LadangID, getuserid, User.Identity.Name); //Modified by Shazana 1/8/2023
            var syarikatCodeId = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fldOptConfValue == SyarikatID.ToString() && x.fld_NegaraID == NegaraID).Select(x => x.fld_SyarikatID).FirstOrDefault();
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
                    //  ladanglist = new SelectList(listwilayah1.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
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

        //Added Shazana 15/12/2022

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2")]
        public ActionResult ApplicationSupportRegionFirst()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> SyarikatIDList = new List<SelectListItem>(); //Added by Shazana 1/8/2023
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();


            //Modified by Shazana 15/8/2023
            if (WilayahID == 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var ladangInfo = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                var listladang = new SelectList(db.tbl_Ladang.Where(x => x.fld_ID == LadangID).OrderBy(x => x.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangID).ToList();

                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_ID == ladangInfo.fld_ID).FirstOrDefault();
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_ID == ladangInfo.fld_ID).OrderBy(x => x.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", WilayahID).ToList();
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == ladangInfo.fld_CostCentre).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == ladangInfo.fld_CostCentre).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", ladangInfo.fld_CostCentre).ToList();
            }


            ViewBag.SyarikatIDList = SyarikatIDList; //Added by Shazana 1/8/2023
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2")]
        //Modified by Shazana 1/8/2023
        public ActionResult ApplicationSupportRegionFirst(int SyarikatIDList, int WilayahIDList, int LadangIDList)
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone(); //temporary to display currenth month

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> SyarikatIDList2 = new List<SelectListItem>(); //Added by Shazana 1/8/2023
            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            //Added by Shazana 1/8/2023
            var SyarikatIDCode = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == SyarikatIDList.ToString()).FirstOrDefault();

            //Modified by Shazana 15/8/2023
            if (WilayahID == 0 && LadangID == 0)
            {
                var listsyarikat = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).ToList();
                SyarikatIDList2 = new SelectList(listsyarikat.OrderBy(x => x.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", SyarikatIDList).ToList();

                var ladangInfo = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatIDList.ToString() && x.fld_Deleted == false && x.fld_SyarikatID == SyarikatIDCode.fld_SyarikatID).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();

                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => x.fld_Deleted == false && ladangInfo.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", WilayahIDList).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatIDList.ToString() && x.fld_Deleted == false && x.fld_SyarikatID == SyarikatIDCode.fld_SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WlyhID == WilayahIDList).OrderBy(x => x.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_ID + "-" + s.fld_LdgName }), "Value", "Text", WilayahIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList.ToString()).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                var listsyarikat = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).ToList();
                SyarikatIDList2 = new SelectList(listsyarikat.OrderBy(x => x.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", SyarikatIDList).ToList();

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList.ToString()).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                var listsyarikat = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).ToList();
                SyarikatIDList2 = new SelectList(listsyarikat.OrderBy(x => x.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", SyarikatIDList).ToList();
            }

            ViewBag.comcos = SyarikatIDList; //Added by Shazana 15/8/2023

            //        //Commented by Shazana 1/8/2023
            //        if (WilayahID == 0 && LadangID == 0)
            //        {
            //            wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //            WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            //            WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //            LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //        }
            //        else if (WilayahID != 0 && LadangID == 0)
            //        {
            //            wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //            WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //            LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //        }
            //        else if (WilayahID != 0 && LadangID != 0)
            //        {
            //            wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //            WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();

            //            //Added by Shazana 6/3/2023
            //LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //        }
            //Added by Shazana 1/8/2023
            ViewBag.SyarikatIDList = SyarikatIDList2;
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            if (WilayahIDList == 0)
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            }
            else
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, WilayahIDList, getdate.Month, getdate.Year);
            }
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID; //Added by Shazana 1/8/2023
            ViewBag.WilayahID = WilayahIDList;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            ViewBag.GetView = 0;
            ViewBag.SyarikatCc = SyarikatIDList; //Added by Shazana 1/8/2023

            return View();
        }

        //Modified by Shazana 1/8/2023
        //Modified by Shazana 15/8/2023
        public ActionResult ApplicationSupportRegionFirstDetail(List<long> eachid, int NegaraID, int SyarikatID, int LadangID, int Month, int Year, string SyarikatCc)
        {
            bool matchtotal = false;
            var getdata = db.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true && x.fld_CostCentre == SyarikatCc).ToList();
            var getcoundata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1 && x.fld_CostCentre == SyarikatCc).Count();
            if (getdata.Count() == getcoundata)
            {
                matchtotal = true;
            }

            ViewBag.matchtotal = matchtotal;
            //Added by Shazana 30/3/2023
            ViewBag.Month = Month;
            ViewBag.Year = Year;
            return View(getdata);
        }

        public JsonResult UpdateDataFirst(int LdgID, string UpdateFlag, int Month, int Year, int NegaraId, int SyarikatId, int WilayahId, string SebabTolak)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            string subject = "";
            string msg = "";
            string DepartmentRMGR = "";
            string DepartmentRMGR2 = "";
            string DepartmentAM = "";
            string DepartmentMGR = "";
            string DepartmentCL = "";
            string DepartmentHR = "";
            string[] to = new string[] { };
            List<string> tolist = new List<string>();
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            int? SemakWilById;

            //Added by Shazana 24/5/2023
            string CompanyShortName = GetIdentity.getCompanyShortName(SyarikatId);

            //var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s=>s.fld_ID).ToList();
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_LadangID == LdgID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_ID).ToList();
            //var getJumlahKeseluruhanPermohonan = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_JumlahPermohonan).Sum();

            var GetEstate = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LdgID && x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == LdgID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId).FirstOrDefault();

            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                DepartmentRMGR = "RMGR_FINANCE_APPROVAL_FELDA";
                DepartmentRMGR2 = "RMGR2_FINANCE_APPROVAL_FELDA";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FELDA";
                DepartmentAM = "AM_FINANCE_APPROVAL_FELDA";
                DepartmentCL = "CL_FINANCE_APPROVAL_FELDA";
                DepartmentHR = "HR_FINANCE_APPROVAL_FELDA";
            }
            else
            {
                DepartmentRMGR = "RMGR_FINANCE_APPROVAL_FPM";
                DepartmentRMGR2 = "RMGR2_FINANCE_APPROVAL_FPM";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FPM";
                DepartmentAM = "AM_FINANCE_APPROVAL_FPM";
                DepartmentCL = "CL_FINANCE_APPROVAL_FPM";
                DepartmentHR = "HR_FINANCE_APPROVAL_FPM";
            }

            //Added by Shazana 24/5/2023
            string SemakanNama = "";
            string SokonganNama = "";
            string LulusNama = "";
            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                SemakanNama = " (Pengurus FELDA) ";
                SokonganNama = " (Pegawai Kewangan) ";
                LulusNama = " (Pegawai Wilayah FELDA) ";
            }
            else
            {
                SemakanNama = " (Pengurus FELDA) "; //Added by Shazana 6/6/2023
                SokonganNama = " (Pegawai Wilayah FPM) "; //Added by Shazana 6/6/2023
                LulusNama = " (Pegawai Wilayah FELDA) "; //Added by Shazana 6/6/2023
            }


            switch (UpdateFlag)
            {
                case "SokongGMWil":
                    DescStatus = "Telah Disokong";
                    DatabaseAction.UpdateDataTotblSokPermhnWangGM2(LdgID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2, "");
                    }

                    //Modified by Shazana 20/2/2023
                    //subject = "Kelulusan Permohonan Gaji";
                    //Modified by Shazana 20/2/2023 petang
                    //Modified by Shazana 3/4/2023
                    //Commented by Shazana 24/5/2023
                    //subject = "Kelulusan Permohonan Gaji" + GetEstateDetail.fld_LdgName;
                    //Added by Shazana 24/5/2023
                    subject = CompanyShortName + " - Kelulusan Permohonan Gaji" + GetEstateDetail.fld_LdgName;

                    int? SokongWilGM_By;
                    SokongWilGM_By = GetEstate.fld_SokongWilGM_By;

                    SemakWilById = GetEstate.fld_SemakWil_By;

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    //Modified by Shazana 20/2/2023 petang
                    //msg += "<p>Tuan/Puan (Unit Kewangan),</p>";
                    msg += "<p>Tuan/Puan,</p>";
                    //Modify by Shazana 15/2/2023
                    //msg += "<p>Mohon sokongan kelulusan permohonan gaji (Gaji Pekerja Buruh) dari pihak Tuan/Puan (Unit Kewangan). Keterangan seperti dibawah:-</p>";
                    msg += "<p>Mohon kelulusan permohonan gaji (Gaji Pekerja Buruh) dari pihak Tuan/Puan (Wilayah(Perladangan)). Keterangan seperti dibawah:-</p>";

                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    //Modified by Shazana 20/2/2023
                    //msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Disemak Oleh</th><th>Waktu Tindakan</th><th>Disokong Oleh</th><th>Waktu Tindakan</th><th>Pautan</th>";

                    //Modified by Shazana 20/2/2023 petang
                    //msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Disemak Oleh</th><th>Waktu Tindakan</th><th>Disahkang Oleh</th><th>Waktu Tindakan</th><th>Pautan</th>";
                    //Commented by Shazana 24/5/2023
                    //msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Disokong Oleh</th><th>Waktu Tindakan</th><th>Disahkan Oleh</th><th>Waktu Tindakan</th><th>Pautan</th>";
                    //Added by Shazana 24/5/2023
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Disemak Oleh</th><th>Waktu disemak</th><th>Disokong Oleh</th><th>Waktu disokong</th><th>Pautan</th>";


                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    //Modified by Shazana 15/2/2023
                    //msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SemakWilById) + "</td><td align=\"center\">" + GetEstate.fld_SemakWil_DT + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td align=\"center\"><a href=\"" + Url.Action("ApplicationSupportRegionHQ", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";

                    //Modified by Shazana 20/4/2023
                    //Modified by Shazana 20/2/2023 petang
                    //msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SemakWilById) + "</td><td align=\"center\">" + GetEstate.fld_SemakWil_DT + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td align=\"center\"><a href=\"" + Url.Action("ApplicationSupportRegionSecond", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    //Commented by Shazana 24/5/2023
                    //msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SemakWilById) + "</td><td align=\"center\">" + GetEstate.fld_SemakWil_DT + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SokongWilGM_By) + "</td><td align=\"center\">" + GetEstate.fld_SokongWilGM_DT + "</td><td align=\"center\"><a href=\"" + Url.Action("ApplicationSupportRegionSecond", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    //Added by Shazana 24/5/2023
                    //Commented by Shazana 1/8/2023
                    ////msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SemakWilById) + SemakanNama + "</td><td align=\"center\">" + GetEstate.fld_SemakWil_DT + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SokongWilGM_By) + SokonganNama + "</td><td align=\"center\">" + GetEstate.fld_SokongWilGM_DT + "</td><td align=\"center\"><a href=\"" + Url.Action("ApplicationSupportRegionSecond", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    //Added by Shazana 1/8/2023
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SemakWilById) + "( " + GlobalFunction.getJawatanName(SemakWilById, NegaraId, SyarikatId) + " )" + "</td><td align=\"center\">" + GetEstate.fld_SemakWil_DT + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SokongWilGM_By) + "( " + GlobalFunction.getJawatanName(SokongWilGM_By, NegaraId, SyarikatId) + " )" + "</td><td align=\"center\">" + GetEstate.fld_SokongWilGM_DT + "</td><td align=\"center\"><a href=\"" + Url.Action("ApplicationSupportRegionSecond", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";

                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    //Modified by Shazana 6/3/2023
                    //var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstateDetail.fld_NegaraID && x.fldSyarikatID == GetEstateDetail.fld_SyarikatID && x.fldDepartment == DepartmentRMGR2 && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstateDetail.fld_NegaraID && x.fldSyarikatID == GetEstateDetail.fld_SyarikatID && x.fldWilayahID == WilayahId && x.fldDepartment == DepartmentRMGR2 && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    if (ToEmail != null)
                    {
                        foreach (var toemail in ToEmail)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    //Modified by Shazana 6/3/2023
                    var CcEmail = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId && x.fld_ID == GetEstate.fld_LadangID).Select(x => x.fld_LdgEmail).FirstOrDefault();
                    var CcEmail2 = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldDepartment == DepartmentRMGR && x.fldCategory == "CC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    //var CcEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentAM && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentCL && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentRMGR2 && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    if (CcEmail != null)
                    {
                        //MOdified by SHazana 3/4/2023
                        //foreach (var ccemail in CcEmail)
                        //{
                        //    //Commented by Shazana 6/3/2023
                        //    //cclist.Add(ccemail.fldEmail);
                        //    //Added by Shazana 6/3/2023
                        //    cclist.Add(CcEmail);
                        //}
                        cclist.Add(CcEmail);
                        cc = cclist.ToArray();
                    }

                    //Added by Shazana 6/3/2023
                    if (CcEmail2 != null)
                    {
                        foreach (var ccemail2 in CcEmail2)
                        {
                            cclist.Add(ccemail2.fldEmail);

                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmail != null)
                    {
                        foreach (var bccemail in BccEmail)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetSentToFinManager.fldEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
                case "TolakGMWil":
                    DescStatus = "Telah Ditolak";
                    DatabaseAction.UpdateDataTotblSokPermhnWangGM2(LdgID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2, SebabTolak);
                    }

                    //Modified by Shazana 20/2/2023
                    //subject = "Penolakkan Permohonan Tolak";
                    //Commented by Shazana 24/5/2023
                    //subject =  "Penolakkan Permohonan Gaji";
                    //Added by Shazana 24/5/2023
                    subject = CompanyShortName + " - Penolakkan Permohonan Gaji";

                    //var GetSentToWil = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahId && x.fld_SyarikatID == SyarikatId).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    //Modify by Shazana 15/2/2023
                    //msg += "<p>Dukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh HR JTK. Mohon pihak ladang buat semakkan kembali. Keterangan seperti dibawah :-</p>";
                    //Modified by Shazana 28/4/2023
                    msg += "<p>Dukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh Wilayah(Kewangan/RC). Mohon pihak ladang buat semakan kembali. Keterangan seperti dibawah :-</p>";

                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Sebab</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";

                    //Modified by Shazana 1/8/2023
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + SebabTolak + "</td><td align=\"center\">" + ActionBy + "( " + GlobalFunction.getJawatanName(getuserid, NegaraId, SyarikatId) + " )" + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";
                    //Modified by Shazana 6/3/2023
                    var ToEmailT = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId && x.fld_ID == GetEstate.fld_LadangID).Select(x => x.fld_LdgEmail).FirstOrDefault();
                    //var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID && ((x.fldDepartment == DepartmentCL && x.fldCategory == "TO") || x.fldDepartment == DepartmentAM && x.fldCategory == "TO") && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmailT != null)
                    {
                        //Modified by Shazana 3/4/2023
                        //foreach (var toemail in ToEmailT)
                        //{
                        //    //Commented by Shazana 6/3/2023
                        //    //tolist.Add(toemail.fldEmail);
                        //    //Added by Shazana 6/3/2023
                        //    tolist.Add(ToEmailT);
                        //}
                        tolist.Add(ToEmailT);
                        to = tolist.ToArray();
                    }

                    //Commented by Shazana 6/3/2023
                    //var CcEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    //if (CcEmailT != null)
                    //{
                    //    foreach (var ccemail in CcEmailT)
                    //    {
                    //        cclist.Add(ccemail.fldEmail);
                    //    }
                    //    cc = cclist.ToArray();
                    //}

                    var BccEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmailT != null)
                    {
                        foreach (var bccemail in BccEmailT)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetEstateDetail.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);
                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime, SebabTolak = SebabTolak });
        }

        //Modified by Shazana 30/3/2023
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin, Super Admin, Admin 1, Admin 2")]
        public ActionResult ApplicationSupportRegionSecond()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatIDList = new List<SelectListItem>(); //Added by Shazana 1/8/2023

            //Commented by Shazana 1/8/2023
            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    //Modified by Shazana 30/3/2023
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //}

            //Added by Shazana 1/8/2023
            //Modified by Shazana 15/8/2023
            if (WilayahID == 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var ladangInfo = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                var listladang = new SelectList(db.tbl_Ladang.Where(x => x.fld_ID == LadangID).OrderBy(x => x.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangID).ToList();

                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_ID == ladangInfo.fld_ID).FirstOrDefault();
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_ID == ladangInfo.fld_ID).OrderBy(x => x.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", WilayahID).ToList();
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == ladangInfo.fld_CostCentre).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == ladangInfo.fld_CostCentre).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", ladangInfo.fld_CostCentre).ToList();
            }


            ViewBag.SyarikatIDList = SyarikatIDList; //Added by Shazana 1/8/2023

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;


            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            ViewBag.Year = getdate.Year;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2")]
        public ActionResult ApplicationSupportRegionSecond(int SyarikatIDList, int WilayahIDList, int LadangIDList)
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone(); //temporary to display current month

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> SyarikatIDList2 = new List<SelectListItem>(); //Added by Shazana 1/8/2023
            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();


            //Commented by Shazana 1/8/2023
            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            //    WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            //    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            //    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //}

            //Added by Shazana 1/8/2023
            //Modified by Shazana 15/8/2023
            var SyarikatIDCode = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == SyarikatIDList.ToString()).FirstOrDefault();

            if (WilayahID == 0 && LadangID == 0)
            {
                var listsyarikat = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).ToList();
                SyarikatIDList2 = new SelectList(listsyarikat.OrderBy(x => x.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", SyarikatIDList).ToList();

                var ladangInfo = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatIDList.ToString() && x.fld_Deleted == false && x.fld_SyarikatID == SyarikatIDCode.fld_SyarikatID).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();

                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => x.fld_Deleted == false && ladangInfo.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", WilayahIDList).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatIDList.ToString() && x.fld_Deleted == false && x.fld_SyarikatID == SyarikatIDCode.fld_SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WlyhID == WilayahIDList).OrderBy(x => x.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_ID + "-" + s.fld_LdgName }), "Value", "Text", WilayahIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList.ToString()).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                var listsyarikat = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).ToList();
                SyarikatIDList2 = new SelectList(listsyarikat.OrderBy(x => x.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", SyarikatIDList).ToList();

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList.ToString()).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                var listsyarikat = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).ToList();
                SyarikatIDList2 = new SelectList(listsyarikat.OrderBy(x => x.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", SyarikatIDList).ToList();
            }

            ViewBag.comcos = SyarikatIDList; //Added by Shazana 15/8/2023
            ViewBag.SyarikatCc = SyarikatIDList; //Added by Shazana 15/8/2023


            ViewBag.SyarikatIDList = SyarikatIDList2; //Added by Shazana 1/8/2023
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            if (WilayahIDList == 0)
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            }
            else
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, WilayahIDList, getdate.Month, getdate.Year);
            }

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.WilayahID = WilayahIDList;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            ViewBag.GetView = 0;
            return View();
        }

        //Modified by Shazana 3/4/2023
        //Modified by Shazana 15/8/2023
        public ActionResult ApplicationSupportRegionSecondDetail(List<long> eachid, int NegaraID, int SyarikatID, int WilayahID, int LadangID, int Month, int Year,string SyarikatCc)
        {
            bool matchtotal = false;
            //Modified by Shazana 15/8/2023
            var getdata = db.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true && x.fld_CostCentre == SyarikatCc).ToList();
            var getcoundata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1 && x.fld_SokongWilGM_Status == 1 && x.fld_CostCentre == SyarikatCc).Count();
            if (getdata.Count() == getcoundata)
            {
                matchtotal = true;
            }

            ViewBag.matchtotal = matchtotal;

            //Added by Shazana 30/3/2023
            ViewBag.Month = Month;
            ViewBag.Year = Year;

            //Added by Shazana 3/4/2023
            ViewBag.WilayahID = WilayahID;
            return View(getdata);
        }

        public JsonResult UpdateDataSecond(int LdgID, string UpdateFlag, int Month, int Year, int NegaraId, int SyarikatId, int WilayahId, string SebabTolak)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            string subject = "";
            string msg = "";
            string DepartmentRMGR = "";
            string DepartmentRMGR2 = "";
            string DepartmentAM = "";
            string DepartmentMGR = "";
            string DepartmentCL = "";
            string DepartmentHR = "";
            string[] to = new string[] { };
            List<string> tolist = new List<string>();
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            int? SemakWilById;
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_LadangID == LdgID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_ID).ToList();

            var GetEstate = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LdgID && x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == LdgID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId).FirstOrDefault();

            //Added by Shazana 24/5/2023
            string CompanyShortName = GetIdentity.getCompanyShortName(SyarikatId);

            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                DepartmentRMGR = "RMGR_FINANCE_APPROVAL_FELDA";
                DepartmentRMGR2 = "RMGR2_FINANCE_APPROVAL_FELDA";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FELDA";
                DepartmentAM = "AM_FINANCE_APPROVAL_FELDA";
                DepartmentCL = "CL_FINANCE_APPROVAL_FELDA";
                DepartmentHR = "HR_FINANCE_APPROVAL_FELDA";
            }
            else
            {
                DepartmentRMGR = "RMGR_FINANCE_APPROVAL_FPM";
                DepartmentRMGR2 = "RMGR2_FINANCE_APPROVAL_FPM";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FPM";
                //Modified by Shazana 20/2/2023 
                //DepartmentAM = "AM_FINANCE_APPROVAL_FPM";
                DepartmentAM = "FC_FINANCE_APPROVAL_FPM";
                DepartmentCL = "CL_FINANCE_APPROVAL_FPM";
                DepartmentHR = "HR_FINANCE_APPROVAL_FPM";
            }

            //Added by Shazana 24/5/2023
            string SemakanNama = "";
            string SokonganNama = "";
            string LulusNama = "";
            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                SemakanNama = " (Pengurus FELDA) ";
                SokonganNama = " (Pegawai Kewangan) ";
                LulusNama = " (Pegawai Wilayah FELDA) ";
            }
            else
            {
                SemakanNama = " (Pengurus FELDA) ";//Added by Shazana 6/6/2023
                SokonganNama = " (Pegawai Wilayah FPM) ";//Added by Shazana 6/6/2023
                LulusNama = " (Pegawai Wilayah FELDA) ";//Added by Shazana 6/6/2023
            }

            switch (UpdateFlag)
            {
                case "SokongGMWil":
                    DescStatus = "Telah Diluluskan";
                    DatabaseAction.UpdateDataTotblSokPermhnWangGMSecond(LdgID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2, "");
                    }

                    //Modified by Shazana 20/2/2023
                    //subject = "Kelulusan Permohonan Gaji";
                    //Modified by Shazana 3/4/2023
                    subject = "Permohonan Wang Telah Diluluskan " + GetEstateDetail.fld_LdgName;

                    SemakWilById = GetEstate.fld_SemakWil_By;

                    //Added by Shazana 24/5/2023
                    string SemakWilbyName = GetIdentity.MyNameFullName(SemakWilById);
                    if (SemakWilbyName == null) { SemakWilbyName = ""; }

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    //Modified by Shazana 20/2/2023
                    //msg += "<p>Tuan/Puan (Unit Kewangan),</p>";
                    msg += "<p>Kepada Rancangan " + GetEstateDetail.fld_LdgName + "</p>";
                    //Modified by Shazana 20/2/2023
                    ////Modify by Shazana 15/2/2023
                    ////msg += "<p>Mohon kelulusan permohonan gaji (Gaji Pekerja Buruh) dari pihak Tuan/Puan (Unit Kewangan). Keterangan seperti dibawah:-</p>";
                    //msg += "<p>Sukacita dimaklumkan, permohonan gaji(Gaji Pekerja Buruh) telah diluluskan oleh Pengurus Kewangan.Keterangan seperti dibawah:</p>";
                    msg += "<p>Sukacita dimaklumkan, permohonan gaji(Gaji Pekerja Buruh) telah diluluskan oleh Wilayah.Keterangan seperti dibawah:</p>";


                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    //Modified by Shazana 20/2/2023
                    //msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Disemak Oleh</th><th>Waktu Tindakan</th><th>Disokong Oleh</th><th>Waktu Tindakan</th><th>Pautan</th>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Diluluskan Oleh</th><th>Waktu diluluskan</th><th>Disokong Oleh</th><th>Waktu disokong</th><th>Disemak Oleh</th><th>Waktu Disemak</th>";

                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    //Modify by Shazana 15/2/2023
                    //msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SemakWilById) + "</td><td align=\"center\">" + GetEstate.fld_SemakWil_DT + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td align=\"center\"><a href=\"" + Url.Action("ApplicationSupportRegionHQ", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    //Modified by Shazana 20/4/2023
                    int? SokongWilById;
                    SokongWilById = GetEstate.fld_SokongWilGM_By;
                    //Added by Shazana 24/5/2023
                    string SokongWilbyName = GetIdentity.MyNameFullName(SokongWilById);
                    if (SokongWilbyName == null) { SokongWilbyName = ""; }


                    //Commented by Shazana 24/5/2023
                    //msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td align=\"center\">" + SokongWilById + "</td><td align=\"center\">" + GetEstate.fld_SokongWilGM_DT + "</td><td align=\"center\">" + SemakWilById + "</td><td align=\"center\">" + GetEstate.fld_SemakWil_DT + "</td>";
                    //Added by Shazana 24/5/2023
                    //Modified by Shazana 1/8/2023
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + ActionBy + "( " + GlobalFunction.getJawatanName(getuserid, NegaraId, SyarikatId) + " )" + "</td><td align=\"center\">" + getdatetime + "</td><td align=\"center\">" + SokongWilbyName + "( " + GlobalFunction.getJawatanName(SokongWilById, NegaraId, SyarikatId) + " )" + "</td><td align=\"center\">" + GetEstate.fld_SokongWilGM_DT + "</td><td align=\"center\">" + SemakWilbyName + "( " + GlobalFunction.getJawatanName(SemakWilById, NegaraId, SyarikatId) + " )" + "</td><td align=\"center\">" + GetEstate.fld_SemakWil_DT + "</td>";

                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    //Modified by Shazana 6/3/2023
                    var ToEmail = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId && x.fld_ID == GetEstate.fld_LadangID).Select(x => x.fld_LdgEmail).FirstOrDefault();
                    //var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstateDetail.fld_NegaraID && x.fldSyarikatID == GetEstateDetail.fld_SyarikatID && (x.fldDepartment == DepartmentAM || x.fldDepartment == DepartmentCL) && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmail != null)
                    {
                        //Modified by Shazana 3/4/2023
                        //foreach (var toemail in ToEmail)
                        //{
                        //    //Commented by Shazana 6/3/2023
                        //    //tolist.Add(toemail.fldEmail);
                        //    //Added by Shazana 6/3/2023
                        //    tolist.Add(ToEmail);
                        //}
                        tolist.Add(ToEmail);
                        to = tolist.ToArray();
                    }

                    //Commented by Shazana 24/5/2023
                    //var CcEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentAM && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentCL && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentRMGR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    //Added by Shazana 6/6/2023
                    var CcEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentRMGR && x.fldCategory == "CC" && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmail != null)
                    {
                        foreach (var ccemail in CcEmail)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmail != null)
                    {
                        foreach (var bccemail in BccEmail)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetSentToFinManager.fldEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
                case "TolakGMWil":
                    DescStatus = "Telah Ditolak";
                    DatabaseAction.UpdateDataTotblSokPermhnWangGMSecond(LdgID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2, SebabTolak);
                    }

                    //Modify by Shazana 15/2/2023
                    //subject = "Penolakkan Permohonan Tolak";
                    //Added by Shazana 24/5/2023
                    subject = CompanyShortName + " - Penolakkan Permohonan Gaji";

                    //var GetSentToWil = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahId && x.fld_SyarikatID == SyarikatId).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    //Modify by Shazana 15/2/2023
                    //msg += "<p>Dukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh HR JTK. Mohon pihak ladang buat semakkan kembali. Keterangan seperti dibawah :-</p>";
                    //Modified by Shazana 28/4/2023
                    msg += "<p>Dukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh Wilayah(Perladangan). Mohon pihak ladang buat semakan kembali. Keterangan seperti dibawah :-</p>";

                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Sebab</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + SebabTolak + "</td><td align=\"center\">" + ActionBy + "( " + GlobalFunction.getJawatanName(getuserid, NegaraId, SyarikatId) + " )" + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    //Modified by Shazana 6/3/2023
                    var ToEmailT = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId && x.fld_ID == GetEstate.fld_LadangID).Select(x => x.fld_LdgEmail).FirstOrDefault();
                    //var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID && ((x.fldDepartment == DepartmentCL && x.fldCategory == "TO") || x.fldDepartment == DepartmentAM && x.fldCategory == "TO") && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmailT != null)
                    {
                        //Modified by Shazana 20/4/2023
                        //foreach (var toemail in ToEmailT)
                        //{
                        //    //Commented by Shazana 6/3/2023
                        //    //tolist.Add(toemail.fldEmail);
                        //    //Added by Shazana 6/3/2023
                        //    tolist.Add(ToEmailT);
                        //}
                        tolist.Add(ToEmailT);
                        to = tolist.ToArray();
                    }
                    //Commented by Shazana 6/3/2023
                    //var CcEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    //if (CcEmailT != null)
                    //{
                    //    foreach (var ccemail in CcEmailT)
                    //    {
                    //        cclist.Add(ccemail.fldEmail);
                    //    }
                    //    cc = cclist.ToArray();
                    //}

                    var BccEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmailT != null)
                    {
                        foreach (var bccemail in BccEmailT)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetEstateDetail.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);
                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime, SebabTolak = SebabTolak });
        }

        //Close Shazana 15/12/2022
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db2.Dispose();
            }
            base.Dispose(disposing);
        }

        //Modified by Shazana 27/4/2023
        public FileStreamResult exportPDF(int? WilayahIDList, int? LadangIDList, int? YearList, int? MonthList)
        {
            Document pdfDoc = new Document(PageSize.A4, 30, 30, 40, 40);
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

            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var message = "";
            var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            var WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, MonthList, YearList); ;
            if (WilayahIDList == 0)
            {

                WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, MonthList, YearList);
            }
            else
            {
                WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, WilayahIDList, MonthList, YearList);
            }

            string wilayah = "0";
            var DataID = GetIDDataFromDatabase.tbl_SokPermhnWangID2(99, 99, 99, 99, "ApplicationSupportAllLadangSelectedRegion", 1, 2000);

            if (WilayahSelection.Count() > 0)
            {
                foreach (var wilayahselec in WilayahSelection)
                {
                    decimal? totalprmhnnwng = 0;
                    decimal? totalpaymentmode = 0;
                    int ladangidlist = LadangIDList == null ? 0 : LadangIDList.Value;
                    int monthlist = MonthList == null ? 0 : MonthList.Value;
                    int yearlist = YearList == null ? 0 : YearList.Value;

                    if (ladangidlist == 0)
                    {
                        DataID = GetIDDataFromDatabase.tbl_SokPermhnWangID2(NegaraID, SyarikatID, wilayahselec, ladangidlist, "ApplicationSupportAllLadangSelectedRegion", monthlist, yearlist);
                    }
                    else
                    {
                        DataID = GetIDDataFromDatabase.tbl_SokPermhnWangID2(NegaraID, SyarikatID, wilayahselec, ladangidlist, "ApplicationSupportSelectedLadang", monthlist, yearlist);

                    }

                    var getdata = db.vw_PermohonanKewangan.Where(x => DataID.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
                    var namawilayah = db.tbl_Wilayah.Where(x => x.fld_ID == wilayahselec).Select(x => x.fld_WlyhName).FirstOrDefault();
                    // if (WilayahSelection.Count() > 0)

                    pdfDoc.NewPage();
                    //Header
                    pdfDoc = GeneralClass.Header(pdfDoc, Syarikat.fld_NamaSyarikat, "(" + GlobalResCompanyDetail.lblCompanyNo + " : " + Syarikat.fld_NoSyarikat + ")", "Permohonan Kelulusan Wang" + "");


                    PdfPTable tableHeader = new PdfPTable(1);
                    tableHeader.WidthPercentage = 100;
                    tableHeader.SpacingBefore = 10f;
                    float[] widthsHeader = new float[] { 1 };
                    tableHeader.SetWidths(widthsHeader);
                    PdfPCell cellHeader = new PdfPCell();

                    tableHeader = new PdfPTable(6);
                    tableHeader.HeaderRows = 1; //repeat header in each page
                    tableHeader.WidthPercentage = 100;
                    tableHeader.SpacingBefore = 5f;
                    widthsHeader = new float[] { 1, 2.5f, 1, 1, 1, 2.5f };
                    tableHeader.SetWidths(widthsHeader);


                    cellHeader = GeneralClass.CellNoBorder("Kelulusan Permohonan Wang Oleh Perladangan", 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableHeader.AddCell(cellHeader);
                    //----------------------------------------------------

                    PdfPTable table = new PdfPTable(6);
                    table.WidthPercentage = 100;
                    table.SpacingBefore = 10f;
                    float[] widths = new float[] { 0.6f, 0.7f, 1, 1.5f, 1.5f, 2.5f };
                    table.SetWidths(widths);
                    PdfPCell cell = new PdfPCell();

                    table = new PdfPTable(6);
                    table.HeaderRows = 1; //repeat header in each page
                    table.WidthPercentage = 100;
                    table.SpacingBefore = 5f;
                    widths = new float[] { 0.6f, 0.7f, 1, 1.5f, 1.5f, 2.5f };
                    table.SetWidths(widths);

                    int bil = 1;
                    int i = 1;
                    string KdrGaji = "";
                    decimal Jumlah = 0;
                    GetTriager GetTriager = new GetTriager();



                    chunk = new Chunk(namawilayah + " BULAN " + monthlist + " / " + yearlist, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 6;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("Bil", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("Kod \r\n Ladang", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("Nama Ladang", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)); ;
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("Gaji Buruh(RM)", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("Mod \r\n Pembayaran(RM)", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);


                    chunk = new Chunk("Sejarah Tindakan", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    foreach (var item in getdata)
                    {
                        var id = item.fld_ID;
                        var NoAcc = item.fld_NoAcc;
                        var NoGL = item.fld_NoGL;
                        var NoCIT = item.fld_NoCIT;
                        string fontcolor = "";
                        string returndetail = "";
                        var getdata1 = db.tblSokPermhnWangHisActions.Where(x => x.fldHisSPWID == item.fld_ID).OrderBy(o => o.fldHisDT).ToArray();

                        if (getdata1 != null)
                        {
                            var getdatatblSokPermhnWang = db.tbl_SokPermhnWang.Where(x => x.fld_ID == item.fld_ID).FirstOrDefault();
                            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == getdatatblSokPermhnWang.fld_LadangID && x.fld_NegaraID == getdatatblSokPermhnWang.fld_NegaraID && x.fld_SyarikatID == getdatatblSokPermhnWang.fld_SyarikatID && x.fld_WlyhID == getdatatblSokPermhnWang.fld_WilayahID).FirstOrDefault();
                            string SemakanNama = "";
                            string SokonganNama = "";
                            string LulusNama = "";
                            if (GetEstateDetail.fld_CostCentre == "1000")
                            {
                                SemakanNama = " (Pengurus FELDA) ";
                                SokonganNama = " (Pegawai Kewangan) ";
                                LulusNama = " (Pegawai Wilayah FELDA) ";
                            }
                            else
                            {
                                //Added by Shazana 6/6/2023
                                SemakanNama = " (Pengurus FELDA) ";
                                SokonganNama = " (Pegawai Kewangan FPM) ";
                                LulusNama = " (Pegawai Wilayah FELDA) ";
                            }


                            foreach (var data in getdata1)
                            {


                                string roleNama = "";

                                if (data.fldHisDesc.Contains("Telah Disemak"))
                                { roleNama = SemakanNama; }
                                else if (data.fldHisDesc.Contains("Telah Disokong"))
                                {
                                    roleNama = SokonganNama;
                                }
                                else if (data.fldHisDesc.Contains("Telah Diluluskan"))
                                {
                                    roleNama = LulusNama;
                                }


                                returndetail = returndetail + fontcolor + data.fldHisDesc + " oleh " + getidentity.MyNameFullName(data.fldHisUserID) + roleNama + " pada " + data.fldHisDT + "\r\n\r\n";
                            }
                        }



                        totalprmhnnwng = totalprmhnnwng + item.fld_JumlahPermohonan;
                        totalpaymentmode = totalpaymentmode + item.fld_JumlahCash + item.fld_JumlahCheque + item.fld_JumlahCdmas + item.fld_JumlahEwallet;


                        chunk = new Chunk(bil.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Rowspan = 5;
                        table.AddCell(cell);

                        chunk = new Chunk((item == null ? "" : item.fld_LdgCode.ToString()), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Rowspan = 5;
                        table.AddCell(cell);

                        chunk = new Chunk(item.fld_LdgName == null ? "" : item.fld_LdgName.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Rowspan = 5;
                        table.AddCell(cell);

                        chunk = new Chunk(item.fld_JumlahWorkerNet == null ? "GAJI BERSIH : 0" : "GAJI BERSIH  : " + (GetTriager.GetTotalForMoney(item.fld_JumlahWorkerNet)).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);


                        chunk = new Chunk(item.fld_JumlahCash == null ? "CASH : 0" : "CASH : " + (GetTriager.GetTotalForMoney(item.fld_JumlahCash)).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(returndetail.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Rowspan = 5;
                        table.AddCell(cell);

                        chunk = new Chunk(item.fld_JumlahKwsp == null ? "KWSP : 0" : "KWSP : " + (GetTriager.GetTotalForMoney(item.fld_JumlahKwsp)).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;

                        table.AddCell(cell);


                        chunk = new Chunk(item.fld_JumlahCheque == null ? "CHEQUE : 0" : "CHEQUE : " + (GetTriager.GetTotalForMoney(item.fld_JumlahCheque)).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);



                        chunk = new Chunk(item.fld_JumlahSocso == null ? "SOCSO : 0" : "SOCSO : " + (GetTriager.GetTotalForMoney(item.fld_JumlahSocso)).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(item.fld_JumlahCdmas == null ? "CDMAS : 0" : "CDMAS : " + (GetTriager.GetTotalForMoney(item.fld_JumlahCdmas)).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);




                        chunk = new Chunk(item.fld_JumlahSbkp == null ? "SBKP : 0" : "SBKP : " + (GetTriager.GetTotalForMoney(item.fld_JumlahSbkp)).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);


                        chunk = new Chunk(item.fld_JumlahEwallet == null ? "EWALLET : 0" : "EWALLET : " + (GetTriager.GetTotalForMoney(item.fld_JumlahEwallet)).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_TOP;
                        cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);


                        chunk = new Chunk(item.fld_JumlahSip == null ? "SIP : 0" : "SIP : " + (GetTriager.GetTotalForMoney(item.fld_JumlahSip)).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_TOP;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);



                        chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);



                        bil++;
                        i++;


                    }

                    //--------------------------------------------------------------------------
                    chunk = new Chunk("Total ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Colspan = 3;
                    table.AddCell(cell);

                    chunk = new Chunk((totalprmhnnwng == null ? "0" : GetTriager.GetTotalForMoney(totalprmhnnwng)).ToString(), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(totalpaymentmode == null ? "0" : (GetTriager.GetTotalForMoney(totalpaymentmode)).ToString(), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);
                    //--------------------------------------------------------------------------
                    pdfDoc.Add(tableHeader);
                    pdfDoc.Add(table);


                }
            }

            else
            {
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
            }

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            Response.AppendHeader("Content-Disposition", "inline; filename=" + "Kelulusan Permohonan Wang" + ".pdf");
            return new FileStreamResult(output, "application/pdf");

        }

    }
}