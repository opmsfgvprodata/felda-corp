using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.Security;
using MVC_SYSTEM.ConfigModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Admin 1")]
    public class UserController : Controller
    {
        private GetConfig getconfig = new GetConfig();
        private GetIdentity getidentity = new GetIdentity();
        private errorlog geterror = new errorlog();
        private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        private MVC_SYSTEM_Config db2 = new MVC_SYSTEM_Config();
        private MVC_SYSTEM_Models db3 = new MVC_SYSTEM_Models();
        private EncryptDecrypt crypto = new EncryptDecrypt();
        GetWilayah getwilyah = new GetWilayah();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetConfig Getconfig = new GetConfig();
        private GetWilayah GetWilayah = new GetWilayah();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();

        // GET: User
        public ActionResult Index(string filter, int page = 1, string sort = "fld_BlokStatus",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            GetWilayah getwilyah = new GetWilayah();

            //yana tambah 180623
            List<SelectListItem> SyarikatList = new List<SelectListItem>();
            SyarikatList = new SelectList(
                dbC.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.SyarikatList = SyarikatList;

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            int[] wlyhid = new int[] { };

            wlyhid = getwilyah.GetWilayahID(SyarikatID);
            WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;

            ViewBag.User = "class = active";
            return View();
        }

        public ActionResult _User(string filter = "", int fldUserID = 0, int page = 1, string sort = "fldUserName", string sortdir = "DESC", int? WilayahIDList = 0, int? LadangIDList = 0)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            String username_first = User.Identity.Name.Substring(0, 3); //sepul tambah untuk panggil 3 character dpn username


            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.User = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(getconfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tblUser>();
            ViewBag.filter = filter;

            if (WilayahIDList == 0 && LadangIDList == 0)
            {
                if (filter == "")
                {
                    if (getidentity.MySuperAdmin(User.Identity.Name))
                    {
                        records.Content = dbview.tblUsers
                            .Where(x => x.fldUserID != getuserid)
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        records.TotalRecords = dbview.tblUsers.Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                    else
                    {
                        records.Content = dbview.tblUsers
                            .Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldUserID != getuserid && x.fldUserName.Substring(0, 3) == username_first) //sepul tambah x.fldUserName.Substring(0, 3) == username_first untuk papar FEL/FTP usernamehaja
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                }
                else
                {
                    if (getidentity.MySuperAdmin(User.Identity.Name))
                    {
                        records.Content = dbview.tblUsers.Where(x => x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter) && x.fldUserID != getuserid && x.fldUserName.Substring(0, 3) == username_first) //sepul tambah x.fldUserName.Substring(0, 3) == username_first untuk papar FEL/FTP username sahaja
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                    else
                    {
                        records.Content = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID) && x.fldUserID != getuserid && x.fldUserName.Substring(0, 3) == username_first) //sepul tambah x.fldUserName.Substring(0, 3) == username_first untuk papar FEL/FTP username sahaja
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID)).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                }

            }
            else if (WilayahIDList != 0 && LadangIDList == 0)
            {
                if (filter == "")
                {
                    if (getidentity.MySuperAdmin(User.Identity.Name))
                    {
                        records.Content = dbview.tblUsers
                            .Where(x => x.fldUserID != getuserid)
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        records.TotalRecords = dbview.tblUsers.Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                    else
                    {
                        records.Content = dbview.tblUsers
                            .Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldUserID != getuserid && x.fldWilayahID == WilayahIDList)
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldWilayahID == WilayahIDList).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                }
                else
                {
                    if (getidentity.MySuperAdmin(User.Identity.Name))
                    {
                        records.Content = dbview.tblUsers.Where(x => x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter) && x.fldUserID != getuserid && x.fldUserName.Substring(0, 3) == username_first) //sepul tambah x.fldUserName.Substring(0, 3) == username_first untuk papar FEL/FTP username sahaja
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                    else
                    {
                        records.Content = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID) && x.fldUserID != getuserid && x.fldUserName.Substring(0, 3) == username_first) //sepul tambah x.fldUserName.Substring(0, 3) == username_first untuk papar FEL/FTP username sahaja
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID)).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                }
            }
            else if (WilayahIDList != 0 && LadangIDList != 0)
            {
                if (filter == "")
                {
                    if (getidentity.MySuperAdmin(User.Identity.Name))
                    {
                        records.Content = dbview.tblUsers
                            .Where(x => x.fldUserID != getuserid)
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        records.TotalRecords = dbview.tblUsers.Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                    else
                    {
                        records.Content = dbview.tblUsers
                            .Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldUserID != getuserid && x.fldWilayahID == WilayahIDList && x.fldLadangID == LadangIDList)
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldWilayahID == WilayahIDList && x.fldLadangID == LadangIDList).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                }
                else
                {
                    if (getidentity.MySuperAdmin(User.Identity.Name))
                    {
                        records.Content = dbview.tblUsers.Where(x => x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter) && x.fldUserID != getuserid && x.fldUserName.Substring(0, 3) == username_first) //sepul tambah x.fldUserName.Substring(0, 3) == username_first untuk papar FEL/FTP username sahaja
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                    else
                    {
                        records.Content = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID) && x.fldUserID != getuserid && x.fldUserName.Substring(0, 3) == username_first) //sepul tambah x.fldUserName.Substring(0, 3) == username_first untuk papar FEL/FTP username sahaja
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID)).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                }
            }

            return View(records);
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            AuthModels.tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return RedirectToAction("Index");
            }
            return PartialView("Details", tblUser);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };
            //string mywlyid = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //yana tambah 180623
            List<SelectListItem> SyarikatList = new List<SelectListItem>();
            SyarikatList = new SelectList(
                dbC.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            // end here

            //sepul tambah untuk panggil 3 character dpn username 07/05/2021
            String username_first = User.Identity.Name.Substring(0, 3);
            ViewBag.myID = username_first;


            if (getidentity.MySuperAdmin(User.Identity.Name))
            {
                ViewBag.fldRoleID = new SelectList(db.tblRoles.Where(x => x.fldDeleted == false), "fldRoleID", "fldDescriptionRole");
            }
            else
            {
                ViewBag.fldRoleID = new SelectList(db.tblRoles.Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldDeleted == false), "fldRoleID", "fldDescriptionRole");
            }

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = "HQ", Value = "0" }));

                //Added by kamalia 19/11/2020
                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                //Add by yana 080923
                SyarikatList = new SelectList(
                dbC.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
                // end here 080923

            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName").ToList();

                //Added by kamalia 19/11/2020
                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                //Add by yana 080923
                SyarikatList = new SelectList(
                dbC.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
                // end here 080923

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                //Added by kamalia 19/11/2020

                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                //Add by yana 080923
                SyarikatList = new SelectList(
                dbC.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
                // end here 080923
            }

            ViewBag.fldWilayahID = WilayahIDList;
            //Added by kamalia 19/11/2020
            ViewBag.fldLadangID = LadangIDList;
            // yana add 070823
            ViewBag.fldSyarikatList = SyarikatList;
            // end here 070823

            return PartialView("Create");
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AuthModels.tblUser tblUser)
        {
            DateTime getdatetime = timezone.gettimezone();
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int? getclientid = db3.tbl_ServicesList.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == tblUser.fldWilayahID).Select(s => s.fld_ClientID).FirstOrDefault();

            int? getkmplnid = db.tbl_Negara.Where(x => x.fld_NegaraID == NegaraID).Select(s => s.fld_KmplnSyrktID).FirstOrDefault();

            //string default_pass = "init123";


            //if (ModelState.IsValid)
            //{
            try
            {
                if (ModelState.IsValid)
                {
                    var checkdata = db.tblUsers.Where(x => x.fldUserName == tblUser.fldUserName).FirstOrDefault();
                    if (checkdata == null)
                    {
                        tblUser.fldUserName = tblUser.fldUserName.ToUpper();
                        tblUser.fldUserFullName = tblUser.fldUserFullName.ToUpper();
                        tblUser.fldUserShortName = tblUser.fldUserShortName.ToUpper();
                        tblUser.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
                        tblUser.fldDeleted = false;
                        tblUser.fldClientID = getclientid;
                        tblUser.fld_CreatedBy = getuserid;
                        tblUser.fld_CreatedDT = getdatetime;
                        tblUser.fld_ModifiedBy = getuserid;
                        tblUser.fld_ModifiedDT = getdatetime;
                        tblUser.fld_KmplnSyrktID = getkmplnid;
                        tblUser.fldFirstTimeLogin = 1;
                        //comment by kamalia 19/11/20
                        // tblUser.fldLadangID = 0;
                        tblUser.fldNegaraID = NegaraID;
                        tblUser.fldSyarikatID = SyarikatID;
                        tblUser.fld_KmplnSyrktID = getkmplnid;
                        tblUser.fldUserCategory = "CHECKROLL";
                        db.tblUsers.Add(tblUser);
                        db.SaveChanges();
                        var getid = db.tblUsers.Where(w => w.fldUserName == tblUser.fldUserName).FirstOrDefault();

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
                            method = "1",
                            div = "UserDetails",
                            rootUrl = domain,
                            action = "_User",
                            controller = "User"
                        });
                    }
                }
                return Json(new
                {
                    success = false,
                    msg = GlobalResCorp.msgErrorData,
                    status = "danger",
                    checkingdata = "0"
                });
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

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };
            //string mywlyid = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            if (id == null)
            {
                return RedirectToAction("Index");
            }
            AuthModels.tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return RedirectToAction("Index");
            }

            if (getidentity.MySuperAdmin(User.Identity.Name))
            {
                ViewBag.fldRoleID = new SelectList(db.tblRoles.Where(x => x.fldDeleted == false), "fldRoleID", "fldDescriptionRole", tblUser.fldRoleID);
            }
            else
            {
                ViewBag.fldRoleID = new SelectList(db.tblRoles.Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldDeleted == false), "fldRoleID", "fldDescriptionRole", tblUser.fldRoleID);
            }

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                // yana comment 051023
                //wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                ////mywlyid = String.Join("", wlyhid); ;
                //WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName", tblUser.fldWilayahID).ToList();
                //WilayahIDList.Insert(0, (new SelectListItem { Text = "HQ", Value = "0" }));

                ////Added by kamalia 19/11/2020

                //LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false), "fld_ID", "fld_LdgName", tblUser.fldLadangID).ToList();
                //LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" })); //sepul tambah ni untuk select all kalau default HQ 07/01/2021
                //SyarikatList = new SelectList(dbC.tblOptionConfigsWebs
                //        .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                //        .OrderBy(o => o.fldOptConfDesc)
                //        .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                //    "Value", "Text").ToList();
                // end here 051023

                if (tblUser.fldWilayahID == 0 && tblUser.fldLadangID == 0)
                {

                    //var LadangInfo = db.tbl_Ladang.Where(x => x.fld_ID == tblUser.fldLadangID).FirstOrDefault();
                    //var syarikatInfo = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fldOptConfValue== LadangInfo.fld_CostCentre).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                    //int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);

                    var listladang3 = db.tbl_Ladang.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();

                    var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang3.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName).ToList();
                    WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", tblUser.fldWilayahID).ToList();
                    WilayahIDList.Insert(0, (new SelectListItem { Text = "HQ", Value = "0" }));
                    var listladang2 = db.tbl_Ladang.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).ToList();

                    //LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text", tblUser.fldLadangID).ToList();
                    LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                    //SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                    SyarikatList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                }
                else if (tblUser.fldWilayahID != 0 && tblUser.fldLadangID == 0)
                {
                    //var LadangInfo = db.tbl_Ladang.Where(x => x.fld_ID == tblUser.fldLadangID).FirstOrDefault();
                    //var syarikatInfo = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID ).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                    //int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);

                    //var listladang3 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();
                    var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(x => x.fld_WlyhName).ToList();
                    WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", tblUser.fldWilayahID).ToList();
                    WilayahIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
                    //var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).ToList();

                    //LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false && x.fld_WlyhID == tblUser.fldWilayahID).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text", LadangInfo.fld_ID).ToList();
                    LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                    //SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", LadangInfo.fld_CostCentre).ToList();
                    SyarikatList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                }
                else
                {
                    var LadangInfo = db.tbl_Ladang.Where(x => x.fld_ID == tblUser.fldLadangID).FirstOrDefault();
                    var syarikatInfo = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fldOptConfValue == LadangInfo.fld_CostCentre).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                    int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);

                    var listladang3 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();

                    var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang3.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName).ToList();
                    WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", LadangInfo.fld_WlyhID).ToList();
                    WilayahIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
                    var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).ToList();

                    LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false && x.fld_WlyhID == tblUser.fldWilayahID).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text", LadangInfo.fld_ID).ToList();
                    LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                    SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", LadangInfo.fld_CostCentre).ToList();


                }

            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                // yana comment 051023
                ////mywlyid = String.Join("", WilayahID); ;
                //wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName", tblUser.fldWilayahID).ToList();
                ////Added by kamalia 19/11/2020


                //LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID)  && x.fld_Deleted == false), "fld_ID", "fld_LdgName", tblUser.fldLadangID).ToList();
                //SyarikatList = new SelectList(dbC.tblOptionConfigsWebs
                //        .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                //        .OrderBy(o => o.fldOptConfDesc)
                //        .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                //    "Value", "Text").ToList();
                // end here 051023

                var LadangInfo = db.tbl_Ladang.Where(x => x.fld_ID == tblUser.fldLadangID).FirstOrDefault();
                var syarikatInfo = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fldOptConfValue == LadangInfo.fld_CostCentre).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);

                var listladang3 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();

                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_Deleted == false && listladang3.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", LadangInfo.fld_WlyhID).ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).ToList();

                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text", LadangInfo.fld_ID).ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", LadangInfo.fld_CostCentre).ToList();

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                // yana comment 051023
                ////mywlyid = String.Join("", WilayahID); ;
                //wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", tblUser.fldWilayahID).ToList();
                ////Added by kamalia 19/11/2020

                //LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false), "fld_ID", "fld_LdgName", tblUser.fldLadangID).ToList();
                //SyarikatList = new SelectList(dbC.tblOptionConfigsWebs
                //        .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                //        .OrderBy(o => o.fldOptConfDesc)
                //        .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                //    "Value", "Text").ToList();
                // end here 051023

                var LadangInfo = db.tbl_Ladang.Where(x => x.fld_ID == tblUser.fldLadangID).FirstOrDefault();
                var syarikatInfo = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fldOptConfValue == LadangInfo.fld_CostCentre).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);

                var listladang3 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();

                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_Deleted == false && listladang3.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", LadangInfo.fld_WlyhID).ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false && x.fld_ID == LadangID).OrderBy(x => x.fld_LdgName).ToList();

                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false && x.fld_ID == LadangID).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text", LadangInfo.fld_ID).ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", LadangInfo.fld_CostCentre).ToList();


            }
            //Added by kamalia 19/11/2020

            ViewBag.fldLadangID = LadangIDList;
            ViewBag.fldWilayahID = WilayahIDList;
            // yana add 070823
            ViewBag.fldSyarikatList = SyarikatList;
            // end here 070823
            ViewBag.fldDeleted = new SelectList(db2.tblSystemConfigs.Where(x => x.fldFlag1 == "useractivation" && x.fldDeleted == false), "fldConfigValue", "fldConfigDesc", tblUser.fldDeleted);
            tblUser.fldUserPassword = crypto.Decrypt(tblUser.fldUserPassword);
            return PartialView("Edit", tblUser);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AuthModels.tblUser tblUser, int fldWilayahID, int fldLadangID)
        {
            DateTime getdatetime = timezone.gettimezone();
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int? getclientid = db3.tbl_ServicesList.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == tblUser.fldWilayahID).Select(s => s.fld_ClientID).FirstOrDefault();
            //panggil wilayah & estate baru berdasarkan pemilihan dropdowrn- kamalia 19/11/20
            int? wilayahbaru = db.tbl_Wilayah.Where(x => x.fld_ID == fldWilayahID).Select(s => s.fld_ID).FirstOrDefault();
            int? ladangbaru = db.tbl_Ladang.Where(x => x.fld_WlyhID == fldWilayahID && x.fld_ID == fldLadangID).Select(s => s.fld_ID).FirstOrDefault();
            //
            int? getkmplnid = db.tbl_Negara.Where(x => x.fld_NegaraID == NegaraID).Select(s => s.fld_KmplnSyrktID).FirstOrDefault();

            //if (ModelState.IsValid)
            //{
            try
            {
                var getdata = db.tblUsers.Find(id);
                getdata.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
                getdata.fldUserFullName = tblUser.fldUserFullName.ToUpper();
                getdata.fldUserShortName = tblUser.fldUserShortName.ToUpper();
                getdata.fldUserEmail = tblUser.fldUserEmail;
                getdata.fldClientID = getclientid;
                getdata.fldRoleID = tblUser.fldRoleID;
                getdata.fldDeleted = tblUser.fldDeleted;
                //ubah kepada wilayah dan estate baru - kamalia 19/11/20
                getdata.fldWilayahID = wilayahbaru;
                getdata.fldLadangID = ladangbaru;
                //
                getdata.fld_ModifiedBy = getuserid;
                getdata.fld_ModifiedDT = getdatetime;
                getdata.fld_KmplnSyrktID = getkmplnid;
                getdata.fldUserCategory = "CHECKROLL";
                db.Entry(getdata).State = EntityState.Modified;
                db.SaveChanges();
                var getid = id;

                //sepul tambah untuk refresh data lepas save 07/01/2021
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
                    msg = "Data successfully edited.",
                    status = "success",
                    checkingdata = "0",
                    method = "1",
                    div = "UserDetails",
                    rootUrl = domain,
                    action = "_User",
                    controller = "User"
                });
                //sepul tambah sampai sini

                //return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });

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

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            AuthModels.tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return RedirectToAction("Index");
            }
            return PartialView("Delete", tblUser);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                AuthModels.tblUser tblUser = db.tblUsers.Find(id);

                //        if (tblUser == null)
                //        {
                //            return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                //        }
                //        else
                //        {
                //            db.tblUsers.Remove(tblUser);
                //            db.SaveChanges();
                //            return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                //        }

                //    }
                //    catch(Exception ex)
                //    {
                //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                //        return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                //    }

                //}
                bool status = true;

                var message = "";
                if (tblUser.fldDeleted == false)
                {
                    status = true;
                    message = GlobalResCorp.msgDelete2;
                }

                else
                {
                    status = false;
                    message = GlobalResCorp.msgUndelete;
                }

                tblUser.fldDeleted = status;

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
                    method = "1",
                    div = "UserDetails",
                    rootUrl = domain,
                    action = "_User",
                    controller = "User"
                });
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
        //Tmbah getladang untuk display ladang yg dipilih mengikut wilayah kamy 26/4/2022
        public JsonResult GetLadang(int WilayahID, int LadangID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID2 = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            var tbl_Ladang = db.tbl_Ladang.Find(LadangID);
            int[] wlyhid = new int[] { };


            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID2, getuserid, User.Identity.Name);

            if (getwilyah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID != 0)
                {
                    //yana comment - 6/4/2023
                    //ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false && x.fld_CostCentre == "1000").OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }).Distinct(), "Value", "Text", tbl_Ladang.fld_ID).ToList();// original code
                    // }

                    //}

                    // return Json(ladanglist);
                    // }
                    //yana comment sampai sini


                    //yana tambah condition ladang - 6/4/2023
                    if (tbl_Ladang != null)
                    {

                        ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false && x.fld_CostCentre == "1000").OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }).Distinct(), "Value", "Text", tbl_Ladang.fld_ID).ToList();
                    }
                    else
                    {

                        ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false && x.fld_CostCentre == "1000").OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }).Distinct(), "Value", "Text").ToList();
                        ladanglist.Insert(0, (new SelectListItem { Text = "All", Value = "0" })); //added by yana - 06/04/2023
                    }
                    //yana tambah sampai sini
                }
            }

            return Json(ladanglist);
        }
        //Tmbah GetWilayahChange untuk display ladang yg dipilih mengikut wilayah selepas wilayah bertukar - kamy 26/4/2022

        // yana add 270923 - string SyarikatList
        public JsonResult GetWilayahChange(int WilayahID, string SyarikatList)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID2 = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };


            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID2, getuserid, User.Identity.Name);

            if (getwilyah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID != 0)
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }).Distinct(), "Value", "Text").ToList();
                    ladanglist.Insert(0, (new SelectListItem { Text = "All", Value = "0" })); //added by yana - 06/04/2023
                }
            }

            return Json(ladanglist);
        }

        //yana added - 11/10/2023
        public JsonResult GetWilayah2(string SyarikatID)
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
                    wilayahlist.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
                    ladanglist.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                }
                else
                {
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID2 && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    ladanglist.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
                }
            }

            return Json(wilayahlist);
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
