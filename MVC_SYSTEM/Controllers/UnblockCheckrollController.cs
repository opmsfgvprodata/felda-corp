using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ViewingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using static MVC_SYSTEM.Class.GlobalFunction;

namespace MVC_SYSTEM.Controllers
{
    public class UnblockCheckrollController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        GetWilayah getwilyah = new GetWilayah();
        private ChangeTimeZone ChangeTimeZone = new ChangeTimeZone();
        private ChangeTimeZone timezone = new ChangeTimeZone();

        // GET: UnblockCheckroll

        //role id authorization ( block super power user)  - modified by farahin - 28/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult Index(string filter, int page = 1, string sort = "fld_BlokStatus",
            string sortdir = "ASC")
        {
            int[] wlyhid = new int[] { };
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            //yana add 030823
            List<SelectListItem> SyarikatList = new List<SelectListItem>();

            if(SyarikatID == 1)
            {
                SyarikatList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            }
            //fatin added - 16/11/2023
            if (SyarikatID == 2)
            {
                SyarikatList = new SelectList(db.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
            }
            SyarikatList.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" }));
            ViewBag.SyarikatList = SyarikatList;
            // end here 030823

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" }));
                LadangIDList = new SelectList(db.tbl_Ladang
                    .Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .OrderBy(o => o.fld_ID)
                    .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" }));
            }

            else
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" }));

                //fatin added - 12/04/2023
                LadangIDList = new SelectList(db.tbl_Ladang
                    .Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .OrderBy(o => o.fld_ID)
                    .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" }));

                /*LadangIDList = new SelectList(db.tbl_Ladang
                    .Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                    .OrderBy(o => o.fld_ID)
                    .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();*/
            }

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", DateTime.Now.Month);

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
            ViewBag.WlyhList = WilayahIDList;
            ViewBag.EstList = LadangIDList;

            ViewBag.UnblockCheckroll = "class = active";
            return View();
        }

        public ActionResult _UnblockCheckroll(int? MonthList, int? YearList, int? WlyhList, int? EstList, int page = 1,
            string sort = "fld_BlokStatus",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsCorporate.tbl_BlckKmskknDataKerja>();
            int role = GetIdentity.RoleID(getuserid).Value;

            //if (EstList == 0)
            //{
            //    ViewBag.Message = "Sila Pilih Bulan, Tahun, Wilayah dan Ladang";
            //    return View();
            //}
            
                var unitData = db.tbl_BlckKmskknDataKerja
                    .Where(x => x.fld_WilayahID == WlyhList &&
                                x.fld_LadangID == EstList &&
                                x.fld_Year == YearList &&
                                x.fld_Month == MonthList);

                if (unitData != null)
                {
                    records.Content = unitData
                        .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = unitData
                        .Count(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID);

                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                    ViewBag.RoleID = role;
                    ViewBag.pageSize = 1;

                    return View(records);
                }
                else
                {
                    ViewBag.message = "Tiada Maklumat";
                    return View();
                    //    records.Content = unitData.OrderBy(sort + " " + sortdir)
                    //        .Skip((page - 1) * pageSize)
                    //        .Take(pageSize)
                    //        .ToList();

                    //    records.TotalRecords = unitData
                    //        .Count();
                }
            

            
        }

        // yana update 030823 - add string SyarikatList
        public JsonResult GetSubEst(int Wlyh, string SyarikatList)
        // end here 030823
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //var findsub = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kmplnKategoriAktvt" && x.fldOptConfValue == KateAkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();

            List<SelectListItem> result = new List<SelectListItem>();
            // yana update 030823 - add x.fld_CostCentre == SyarikatList
            result = new SelectList(db.tbl_Ladang
                .Where(x => x.fld_Deleted == false && x.fld_WlyhID == Wlyh && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_CostCentre == SyarikatList)
                // end here 030823
                .OrderBy(o => o.fld_ID)
                .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

            return Json(result);
        }

        //fatin added - 26/10/2023
        public JsonResult GetWilayah(string SyarikatID)
        {
            List<SelectListItem> wilayahlist = new List<SelectListItem>();
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var syarikatCodeId = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fldOptConfValue == SyarikatID.ToString() && x.fld_NegaraID == NegaraID).Select(x => x.fld_SyarikatID).FirstOrDefault();
            int SyarikatCode = Convert.ToInt16(syarikatCodeId);

            if (SyarikatID2 == 1)
            {
                if (getwilyah.GetAvailableWilayah(SyarikatCode))
                {
                    if (WilayahID == 0)
                    {
                        //dapatkan ladang filter by costcenter
                        var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatID && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                        var listwilayah1 = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                        wilayahlist = new SelectList(listwilayah1.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                        wilayahlist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
                        ladanglist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));

                    }
                    else
                    {
                        wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID2 && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                        wilayahlist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" })); //fatin added - 16/10/2023
                        ladanglist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
                    }
                }
            }
            //fatin added - 16/11/2023
            if (SyarikatID2 == 2)
            {
                if (WilayahID == 0)
                {
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID.ToString() == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    wilayahlist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
                    ladanglist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));

                }
                else
                {
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID.ToString() == SyarikatID && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    wilayahlist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" })); //fatin added - 16/10/2023
                    ladanglist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
                }
            }
            
            return Json(wilayahlist);
        }

        public JsonResult GetSubEst2(int WilayahID, String SyarikatList)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if(SyarikatID == 1)
            {
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
            }
            //fatin added - 16/11/2023
            if (SyarikatID == 2)
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID.ToString() == SyarikatList && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID.ToString() == SyarikatList && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }
        //end

        public ActionResult _UnblockCheckrollEdit(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> pilihanYaTidak = new List<SelectListItem>();

            pilihanYaTidak = new SelectList(db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "pilihanyatidak" && x.fldDeleted == false && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fldOptConfID)
                .Select(s => new SelectListItem { Value = s.fldOptConfFlag2, Text = s.fldOptConfDesc })
                , "Value", "Text").ToList();

            var unitData = db.tbl_BlckKmskknDataKerja.SingleOrDefault(
                x => x.fld_ID == id &&
                            x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            tbl_BlckKmskknDataKerjaUpdate unitViewModel = new tbl_BlckKmskknDataKerjaUpdate();

            PropertyCopy.Copy(unitViewModel, unitData);

            ViewBag.fld_Selection = pilihanYaTidak;
            return View(unitViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _UnblockCheckrollEdit(ModelsCorporate.tbl_BlckKmskknDataKerjaUpdate optionConfigsWeb)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            tbl_BlckKmskknDataKerjaHistory BlkHistoryModel = new tbl_BlckKmskknDataKerjaHistory();

            try
            {
                if (ModelState.IsValid)
                {
                    var unitData = db.tbl_BlckKmskknDataKerja.SingleOrDefault(
                        x => x.fld_ID == optionConfigsWeb.fld_ID &&
                             x.fld_NegaraID == NegaraID &&
                             x.fld_SyarikatID == SyarikatID);

                    unitData.fld_ValidDT = ChangeTimeZone.gettimezone();
                    unitData.fld_BlokStatus = optionConfigsWeb.fld_BlokStatus;
                    unitData.fld_Remark = optionConfigsWeb.fld_Remark;
                    unitData.fld_UnBlockAppBy = getuserid;
                    unitData.fld_UnBlockAppDT = ChangeTimeZone.gettimezone();
                    
                    PropertyCopy.Copy(BlkHistoryModel, unitData);

                    db.tbl_BlckKmskknDataKerjaHistory.Add(BlkHistoryModel);
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
                        div = "UnblockCheckrollDetails",
                        rootUrl = domain,
                        action = "_UnblockCheckroll",
                        controller = "UnblockCheckroll"
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