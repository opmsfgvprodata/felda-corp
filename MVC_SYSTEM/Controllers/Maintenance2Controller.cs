using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsCorporate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsCustom;
using System.Linq.Dynamic;
using static MVC_SYSTEM.Class.GlobalFunction;
using MVC_SYSTEM.ViewingModels;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
    public class Maintenance2Controller : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private errorlog geterror = new errorlog();
        private GetConfig GetConfig = new GetConfig();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        GetWilayah getwilyah = new GetWilayah();
        // GET: Maintenance2
        public ActionResult MinimumWageValueMaintenance(string filter, int page = 1, string sort = "fldOptConfFlag1",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            ViewBag.Maintenance = "class = active";

            return View();
        }

        public ActionResult _MinimumWageValueMaintenance(string filter, int page = 1,
            string sort = "fldOptConfFlag1",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsCorporate.tblOptionConfigsWeb>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var minimumWageValueData = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "gajiMinimaHarian" &&
                            x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false);

            records.Content = minimumWageValueData.OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = minimumWageValueData
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;

            return View(records);
        }

        public ActionResult _MinimumWageValueMaintenanceEdit(int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            var minimumWageValueData = db.tblOptionConfigsWebs.SingleOrDefault(
                x => x.fldOptConfID == id && x.fldOptConfFlag1 == "gajiMinimaHarian" && x.fld_NegaraID == NegaraID &&
                     x.fld_SyarikatID == SyarikatID && x.fldDeleted == false);

            ModelsCorporate.tblOptionConfigsWebMinimumWageValueViewModel minimumWagevalueViewModel = new ModelsCorporate.tblOptionConfigsWebMinimumWageValueViewModel();

            PropertyCopy.Copy(minimumWagevalueViewModel, minimumWageValueData);

            return PartialView(minimumWagevalueViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _MinimumWageValueMaintenanceEdit(ModelsCorporate.tblOptionConfigsWeb optionConfigsWeb)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            try
            {
                if (ModelState.IsValid)
                {
                    var minimumWageValueData = db.tblOptionConfigsWebs.SingleOrDefault(
                        x => x.fldOptConfID == optionConfigsWeb.fldOptConfID && x.fldOptConfFlag1 == "gajiMinimaHarian" &&
                             x.fld_NegaraID == NegaraID &&
                             x.fld_SyarikatID == SyarikatID && x.fldDeleted == false);

                    minimumWageValueData.fldOptConfValue = optionConfigsWeb.fldOptConfValue;
                    minimumWageValueData.fldOptConfDesc = optionConfigsWeb.fldOptConfDesc;

                    db.SaveChanges();

                    var UpdateGajiMinimumLdg = db.tbl_GajiMinimaLdg.Where(x => x.fld_OptConfigID == optionConfigsWeb.fldOptConfID).ToList();
                    if (UpdateGajiMinimumLdg.Count() > 0)
                    {
                        UpdateGajiMinimumLdg.ForEach(x => x.fld_NilaiGajiMinima = decimal.Parse(optionConfigsWeb.fldOptConfValue));
                        db.SaveChanges();
                    }

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
                        div = "minimumWageValueMaintenanceDetails",
                        rootUrl = domain,
                        action = "_MinimumWageValueMaintenance",
                        controller = "Maintenance2"
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
        public ActionResult _MinimumWageValueMaintenanceCreate()
        {
            ModelsCorporate.tblOptionConfigsWebMinimumWageValueViewModel minimumWagevalueViewModel = new ModelsCorporate.tblOptionConfigsWebMinimumWageValueViewModel();
            return View(minimumWagevalueViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _MinimumWageValueMaintenanceCreate(ModelsCorporate.tblOptionConfigsWeb optionConfigsWeb)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            try
            {
                if (ModelState.IsValid)
                {
                    var tblOptionConfigsWebs = new ModelsCorporate.tblOptionConfigsWeb
                    {
                        fldOptConfFlag1 = "gajiMinimaHarian",
                        fldDeleted = false,
                        fldOptConfValue = optionConfigsWeb.fldOptConfValue,
                        fldOptConfDesc = optionConfigsWeb.fldOptConfDesc,
                        fld_NegaraID = NegaraID,
                        fld_SyarikatID = SyarikatID

                    };
                    db.tblOptionConfigsWebs.Add(tblOptionConfigsWebs);

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
                        div = "minimumWageValueMaintenanceDetails",
                        rootUrl = domain,
                        action = "_MinimumWageValueMaintenance",
                        controller = "Maintenance2"
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
        public ActionResult _MinimumWageEstateAssigned(int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.ValueGajiMinima = db.tblOptionConfigsWebs.Where(x => x.fldOptConfID == id).Select(s => s.fldOptConfValue).FirstOrDefault();

            List<SelectListItem> wilayahList = new List<SelectListItem>();
            wilayahList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            wilayahList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.WilayahList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();
            ladangList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.LadangList = ladangList;
            //added by kamalia 22/02/2021
            var fld_OptConfigID = db.tblOptionConfigsWebs.Where(x => x.fldOptConfID == id).Select(s => s.fldOptConfID).FirstOrDefault();
            //
            var GetAdaGajiMinima = db.tbl_GajiMinimaLdg.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            var GetAdaGajiMinimaIni = GetAdaGajiMinima.Where(x => x.fld_OptConfigID == id).ToList();
            var GetSemuaLadang = db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).ToList();
            var GetSemuaLadangID = GetSemuaLadang.Select(s => s.fld_LadangID).ToArray();
            var GetAdaGajiMinimaLdgID = GetAdaGajiMinima.Select(s => s.fld_LadangID.Value).ToArray();
            var GetTiadaAdaGajiMinimaLdgID = GetSemuaLadangID.Except(GetAdaGajiMinimaLdgID);
            var GetTiadaAdaGajiMinimaLdgDetails = GetSemuaLadang.Where(x => GetTiadaAdaGajiMinimaLdgID.Contains(x.fld_LadangID)).OrderBy(o => o.fld_NamaWilayah).ToList();
            var TiadaGajiMinima = new List<TiadaGajiMinima>();

            foreach (var GetTiadAdaGajiMinimaLdgDetail in GetTiadaAdaGajiMinimaLdgDetails)
            {
                TiadaGajiMinima.Add(new TiadaGajiMinima { LadangID = GetTiadAdaGajiMinimaLdgDetail.fld_LadangID, NamaLadang = GetTiadAdaGajiMinimaLdgDetail.fld_LdgCode + " - " + GetTiadAdaGajiMinimaLdgDetail.fld_NamaLadang + " (" + GetTiadAdaGajiMinimaLdgDetail.fld_NamaWilayah + ")" });
            }
            var GetAdaGajiMinimaIniID = GetAdaGajiMinimaIni.Select(s => s.fld_LadangID.Value).ToArray();
            var GetAdaGajiMinimaLdgDetails = GetSemuaLadang.Where(x => GetAdaGajiMinimaIniID.Contains(x.fld_LadangID)).OrderBy(o => o.fld_NamaWilayah).ToList();
            var AdaGajiMinima = new List<AdaGajiMinima>();
            foreach (var GetAdaGajiMinimaLdgDetail in GetAdaGajiMinimaLdgDetails)
            {
                AdaGajiMinima.Add(new AdaGajiMinima { LadangID = GetAdaGajiMinimaLdgDetail.fld_LadangID, NamaLadang = GetAdaGajiMinimaLdgDetail.fld_LdgCode + " - " + GetAdaGajiMinimaLdgDetail.fld_NamaLadang + " (" + GetAdaGajiMinimaLdgDetail.fld_NamaWilayah + ")" });
            }
            var CustMod_GajiMinima = new CustMod_GajiMinima
            {
                AdaGajiMinima = AdaGajiMinima,
                TiadaGajiMinima = TiadaGajiMinima,
                 //added by kamalia 22/02/2021
                fld_OptConfigID = fld_OptConfigID
                //
            };
            ViewBag.ID = id;
            return View(CustMod_GajiMinima);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _MinimumWageEstateAssigned(int id, int[] PilihGajiMinima, ModelsCorporate.vw_NSWL gajiladang)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            try
            {
                var GetGajiMinima = db.tblOptionConfigsWebs.Where(x => x.fldOptConfID == id).Select(s => s.fldOptConfValue).FirstOrDefault();
                var GetGajiMinimaYgAda = db.tbl_GajiMinimaLdg.Where(x => x.fld_OptConfigID == id && x.fld_Deleted == false).ToList();
                if (PilihGajiMinima != null)
                {
                    var GetGajiMinimaYgAdaLdgId = GetGajiMinimaYgAda.Select(s => s.fld_LadangID.Value).ToArray();
                    var GetGajiMinimaYgBaruLdgId = PilihGajiMinima.Except(GetGajiMinimaYgAdaLdgId);
                    var GetGajiMinimaYgDeleteLdgId = GetGajiMinimaYgAdaLdgId.Except(PilihGajiMinima);
                    var GetDelete = GetGajiMinimaYgAda.Where(x => GetGajiMinimaYgDeleteLdgId.Contains(x.fld_LadangID.Value)).ToList();
                    if (GetDelete.Count() > 0)
                    {
                        //modified by kamalia 30/3/2021
                        GetDelete.ForEach(x => x.fld_ModifiedDT = timezone.gettimezone());
                        GetDelete.ForEach(x => x.fld_ModifiedBy = getuserid);
                        GetDelete.ForEach(x => x.fld_Deleted = true);
                        db.SaveChanges();

                    }
                    var GetLadangDetails = db.vw_NSWL.Where(x => GetGajiMinimaYgBaruLdgId.Contains(x.fld_LadangID)).ToList();
                    var tbl_GajiMinimaLdg = new List<tbl_GajiMinimaLdg>();

                    //modified by kamalia 31/3/2021
                    foreach (var GetLadangDetail in GetLadangDetails)
                    {
                        var GetLadangYgDahAda = db.tbl_GajiMinimaLdg.Where(x => x.fld_LadangID == GetLadangDetail.fld_LadangID && x.fld_Deleted == true && x.fld_OptConfigID == id).ToList();
                        if (GetLadangYgDahAda.Count() > 0)
                        {
                            GetLadangYgDahAda.ForEach(x => x.fld_ModifiedDT = timezone.gettimezone());
                            GetLadangYgDahAda.ForEach(x => x.fld_ModifiedBy = getuserid);
                            GetLadangYgDahAda.ForEach(x => x.fld_Deleted = false);
                            db.SaveChanges();
                        }

                        else
                        {
                            tbl_GajiMinimaLdg.Add(new tbl_GajiMinimaLdg { fld_NegaraID = GetLadangDetail.fld_NegaraID, fld_SyarikatID = GetLadangDetail.fld_SyarikatID, fld_WilayahID = GetLadangDetail.fld_WilayahID, fld_LadangID = GetLadangDetail.fld_LadangID, fld_Deleted = false, fld_NilaiGajiMinima = decimal.Parse(GetGajiMinima), fld_OptConfigID = id, fld_CreatedDT = timezone.gettimezone(), fld_CreatedBy = getuserid });
                        }
                    }
                    //
                    if (tbl_GajiMinimaLdg.Count() > 0)
                    {
                        db.tbl_GajiMinimaLdg.AddRange(tbl_GajiMinimaLdg);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (GetGajiMinimaYgAda.Count() > 0)
                    {
                        //modified by kamalia 30/3/2021
                        GetGajiMinimaYgAda.ForEach(x => x.fld_ModifiedDT = timezone.gettimezone());
                        GetGajiMinimaYgAda.ForEach(x => x.fld_ModifiedBy = getuserid);
                        GetGajiMinimaYgAda.ForEach(x => x.fld_Deleted = true);
                        db.SaveChanges();
                    }
                }
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
                    div = "minimumWageValueMaintenanceDetails",
                    rootUrl = domain,
                    action = "_MinimumWageValueMaintenance",
                    controller = "Maintenance2",
                    paramName = "WilayahList",
                    paramValue = gajiladang.fld_WilayahID,
                    paramName2 = "LadangList",
                    paramValue2 = "0"
                });

            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = false,
                    msg = GlobalResCorp.msgErrorData,
                    status = "danger",
                    checkingdata = "0"
                });

            }
        }



        //modified by kamalia 24/02/2021
        public ActionResult GajiMinimaInfo(int id, int page = 1, string sort = "WilayahID", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.ValueGajiMinima = db.tblOptionConfigsWebs.Where(x => x.fldOptConfID == id).Select(s => s.fldOptConfValue).FirstOrDefault();

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsCustom.ListLadangAdaGajiMinima>();
            int role = GetIdentity.RoleID(getuserid).Value;



            var GetAdaGajiMinima = db.tbl_GajiMinimaLdg.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            var GetSemuaLadang = db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).ToList();
            var GetSemuaLadangID = GetSemuaLadang.Select(s => s.fld_LadangID).ToArray();
            var GetAdaGajiMinimaLdgID = GetAdaGajiMinima.Select(s => s.fld_LadangID.Value).ToArray();
            var GetAdaGajiMinimaIni = GetAdaGajiMinima.Where(x => x.fld_OptConfigID == id).ToList();
            var GetAdaGajiMinimaIniID = GetAdaGajiMinimaIni.Select(s => s.fld_LadangID.Value).ToArray();
            var ListLadangAdaGajiMinima = new List<ListLadangAdaGajiMinima>();
            var GetAdaGajiMinimaLdgDetails = GetSemuaLadang.Where(x => GetAdaGajiMinimaIniID.Contains(x.fld_LadangID)).OrderBy(o => o.fld_NamaWilayah).ToList();

            foreach (var GetAdaGajiMinimaLdgDetail in GetAdaGajiMinimaLdgDetails)
            {

                ListLadangAdaGajiMinima.Add(new ListLadangAdaGajiMinima { LadangID = GetAdaGajiMinimaLdgDetail.fld_LadangID, WilayahID = GetAdaGajiMinimaLdgDetail.fld_WilayahID, Namawilayah = GetAdaGajiMinimaLdgDetail.fld_NamaWilayah, NamaLadang = GetAdaGajiMinimaLdgDetail.fld_LdgCode + " - " + GetAdaGajiMinimaLdgDetail.fld_NamaLadang });

            }
            //modified by kamalia 31/3/2021
            records.Content = ListLadangAdaGajiMinima.OrderBy(o => o.Namawilayah).ThenBy(c => c.NamaLadang)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            records.TotalRecords = ListLadangAdaGajiMinima.Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;


            return View(records);
        }

        public JsonResult GetLadang(int WilayahID, int id)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);
            var GetAdaGajiMinima = db.tbl_GajiMinimaLdg.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            var GetAdaGajiMinimaIni = GetAdaGajiMinima.Where(x => x.fld_OptConfigID == id).ToList();
            var GetSemuaLadang = db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).ToList();
            var GetSemuaLadangID = GetSemuaLadang.Select(s => s.fld_LadangID).ToArray();
            var GetAdaGajiMinimaLdgID = GetAdaGajiMinima.Select(s => s.fld_LadangID.Value).ToArray();
            var GetTiadaAdaGajiMinimaLdgID = GetSemuaLadangID.Except(GetAdaGajiMinimaLdgID);
            var GetTiadaAdaGajiMinimaLdgDetails = GetSemuaLadang.Where(x => GetTiadaAdaGajiMinimaLdgID.Contains(x.fld_LadangID)).OrderBy(o => o.fld_NamaWilayah).ToList();

            if (getwilyah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false && GetTiadaAdaGajiMinimaLdgID.Contains(x.fld_LadangID)).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }
    }
}