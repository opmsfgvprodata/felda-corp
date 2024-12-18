using Itenso.TimePeriod;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsCustom;
using MVC_SYSTEM.ModelsEstate;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.ViewingModelsOPMS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
//using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using static MVC_SYSTEM.Class.GlobalFunction;

namespace MVC_SYSTEM.Controllers
{

    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
    public class EstateDataManagementController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetConfig GetConfig = new GetConfig();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        errorlog geterror = new errorlog();
        private GlobalFunction GlobalFunction = new GlobalFunction();
        GetWilayah getwilyah = new GetWilayah();

        // GET: EstateDataManagement
        public ActionResult Index()
        {
            ViewBag.EstateDataManage = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Comment by fatin - 24/04/2023
            //ViewBag.MaintenanceList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "estatedatamanage" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();

            //fatin modified - 24/04/2023
            ViewBag.MaintenanceList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "estatedatamanage" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_Desc).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            //end
            db.Dispose();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string MaintenanceList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int maintenancelist = int.Parse(MaintenanceList);
            var action = db.tblMenuLists.Where(x => x.fld_ID == maintenancelist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
            db.Dispose();
            return RedirectToAction(action, "EstateDataManagement");
        }

        public ActionResult EstateWorkerMasterDataMaintenance()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.EstateDataManage = "class = active";

            List<SelectListItem> wilayahList = new List<SelectListItem>();
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
            //end

            wilayahList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.WilayahList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();
            ladangList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.LadangList = ladangList;

            return View();
        }

        public ActionResult _EstateWorkerMasterDataMaintenance(int? WilayahList, int? LadangList, int page = 1, string sort = "fldOptConfFlag1", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsEstate.tbl_Pkjmast>();
            int role = GetIdentity.RoleID(getuserid).Value;

            List<ModelsEstate.tbl_Pkjmast> pkjMastList = new List<ModelsEstate.tbl_Pkjmast>();

            var message = "";

            if (!String.IsNullOrEmpty(WilayahList.ToString()) && !String.IsNullOrEmpty(LadangList.ToString()))
            {
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

                var estateWorkerMasterData = estateConnection.tbl_Pkjmast.Where(x =>
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList &&
                    x.fld_LadangID == LadangList).OrderBy(o => o.fld_Nama);

                pkjMastList = estateWorkerMasterData.ToList();

                message = GlobalResCorp.msgNoRecord;
            }

            else
            {
                message = GlobalResCorp.lblChooseEsateIncentiveEligibility;
            }

            records.Content = pkjMastList;
            records.TotalRecords = pkjMastList.Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;
            ViewBag.Message = message;

            return View(records);
        }

        public ActionResult _EstateWorkerMasterDataMaintenanceEdit(Guid id, int? WilayahList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            var estateWorkerMasterDataData = estateConnection.tbl_Pkjmast.SingleOrDefault(x =>
                x.fld_UniqueID == id);

            ModelsEstate.tbl_PkjmastModelViewEdit pkjmastModelViewEdit = new tbl_PkjmastModelViewEdit();

            PropertyCopy.Copy(pkjmastModelViewEdit, estateWorkerMasterDataData);

            List<SelectListItem> genderList = new List<SelectListItem>();
            genderList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jantina" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            genderList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.GenderList = genderList;

            List<SelectListItem> maritalStatusList = new List<SelectListItem>();
            maritalStatusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "tarafKahwin" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            maritalStatusList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.MaritalStatusList = maritalStatusList;

            List<SelectListItem> settlerStatusList = new List<SelectListItem>();
            settlerStatusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusPeneroka" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            settlerStatusList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.SettlerStatusList = settlerStatusList;

            List<SelectListItem> religionList = new List<SelectListItem>();
            religionList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "agama" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            religionList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.ReligionList = religionList;

            List<SelectListItem> raceList = new List<SelectListItem>();
            raceList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "bangsa" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            raceList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.RaceList = raceList;

            List<SelectListItem> countryList = new List<SelectListItem>();
            countryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            countryList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.CountryList = countryList;

            List<SelectListItem> activeStatusList = new List<SelectListItem>();
            activeStatusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            activeStatusList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.ActiveStatusList = activeStatusList;

            List<SelectListItem> inactiveReasonList = new List<SelectListItem>();
            inactiveReasonList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "sbbTakAktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            inactiveReasonList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.InactiveReasonList = inactiveReasonList;

            List<SelectListItem> companyCountryList = new List<SelectListItem>();
            companyCountryList = new SelectList(
                db.tbl_Negara
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_NegaraID)
                    .Select(
                        s => new SelectListItem { Value = s.fld_NegaraID.ToString(), Text = s.fld_NegaraID + " - " + s.fld_NamaNegara }), "Value", "Text").ToList();
            companyCountryList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.CompanyCountryList = companyCountryList;

            List<SelectListItem> companyList = new List<SelectListItem>();
            companyList = new SelectList(
                db.tbl_Syarikat
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_SyarikatID)
                    .Select(
                        s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_SyarikatID + " - " + s.fld_NamaSyarikat }), "Value", "Text").ToList();
            companyList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.CompanyList = companyList;

            List<SelectListItem> regionList = new List<SelectListItem>();
            regionList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_ID)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_ID + " - " + s.fld_WlyhName }), "Value", "Text").ToList();
            regionList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.RegionList = regionList;

            List<SelectListItem> estateList = new List<SelectListItem>();
            estateList = new SelectList(
                db.tbl_Ladang
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_ID)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_ID + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            estateList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.EstateList = estateList;

            List<SelectListItem> workerTypeList = new List<SelectListItem>();
            workerTypeList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            workerTypeList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.WorkerTypeList = workerTypeList;

            List<SelectListItem> designationList = new List<SelectListItem>();
            designationList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            designationList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.DesignationList = designationList;

            List<SelectListItem> supplierList = new List<SelectListItem>();
            supplierList = new SelectList(
                db.tbl_Pembekal
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodPbkl)
                    .Select(
                        s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_KodPbkl + " - " + s.fld_NamaPbkl }), "Value", "Text").ToList();
            supplierList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.SupplierList = supplierList;

            List<SelectListItem> kwspSocsoActiveList = new List<SelectListItem>();
            kwspSocsoActiveList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            kwspSocsoActiveList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.KwspSocsoActiveList = kwspSocsoActiveList;

            List<SelectListItem> bankActiveList = new List<SelectListItem>();
            bankActiveList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            bankActiveList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.BankActiveList = bankActiveList;

            List<SelectListItem> kwspContributionList = new List<SelectListItem>();
            kwspContributionList = new SelectList(
                db.tbl_JenisCaruman
                    .Where(x => x.fld_JenisCaruman == "KWSP" && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodCaruman)
                    .Select(
                        s => new SelectListItem { Value = s.fld_KodCaruman, Text = s.fld_KodCaruman + " - " + s.fld_Keterangan }), "Value", "Text").ToList();
            kwspContributionList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.KwspContributionList = kwspContributionList;

            List<SelectListItem> socsoContributionList = new List<SelectListItem>();
            socsoContributionList = new SelectList(
                db.tbl_JenisCaruman
                    .Where(x => x.fld_JenisCaruman == "SOCSO" && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodCaruman)
                    .Select(
                        s => new SelectListItem { Value = s.fld_KodCaruman, Text = s.fld_KodCaruman + " - " + s.fld_Keterangan }), "Value", "Text").ToList();
            socsoContributionList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.SocsoContributionList = socsoContributionList;

            List<SelectListItem> bankList = new List<SelectListItem>();
            bankList = new SelectList(
                db.tbl_Bank
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodBank)
                    .Select(
                        s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_KodBank + " - " + s.fld_NamaBank }), "Value", "Text").ToList();
            bankList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.BankList = bankList;

            List<SelectListItem> negeriList = new List<SelectListItem>();
            negeriList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "negeri" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            negeriList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.NegeriList = negeriList;

            return PartialView(pkjmastModelViewEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateWorkerMasterDataMaintenanceEdit(ModelsEstate.tbl_PkjmastModelViewEdit pkjmastModelViewEdit)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            try
            {
                if (ModelState.IsValid)
                {
                    Connection.GetConnection(out host, out catalog, out user, out pass, pkjmastModelViewEdit.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
                    MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

                    var estateWorkerMasterDataData = estateConnection.tbl_Pkjmast.SingleOrDefault(x =>
                        x.fld_UniqueID == pkjmastModelViewEdit.fld_UniqueID);

                    estateWorkerMasterDataData.fld_Nama = pkjmastModelViewEdit.fld_Nama;
                    estateWorkerMasterDataData.fld_Nokp = pkjmastModelViewEdit.fld_Nokp;
                    estateWorkerMasterDataData.fld_Kdjnt = pkjmastModelViewEdit.fld_Kdjnt;
                    estateWorkerMasterDataData.fld_Trlhr = pkjmastModelViewEdit.fld_Trlhr;
                    estateWorkerMasterDataData.fld_Kdkwn = pkjmastModelViewEdit.fld_Kdkwn;
                    estateWorkerMasterDataData.fld_Kpenrka = pkjmastModelViewEdit.fld_Kpenrka;
                    estateWorkerMasterDataData.fld_Kdagma = pkjmastModelViewEdit.fld_Kdagma;
                    estateWorkerMasterDataData.fld_Kdbgsa = pkjmastModelViewEdit.fld_Kdbgsa;
                    estateWorkerMasterDataData.fld_Kdrkyt = pkjmastModelViewEdit.fld_Kdrkyt;
                    estateWorkerMasterDataData.fld_Prmtno = pkjmastModelViewEdit.fld_Prmtno;
                    estateWorkerMasterDataData.fld_T2prmt = pkjmastModelViewEdit.fld_T2prmt;
                    estateWorkerMasterDataData.fld_Psptno = pkjmastModelViewEdit.fld_Psptno;
                    estateWorkerMasterDataData.fld_T2pspt = pkjmastModelViewEdit.fld_T2pspt;
                    estateWorkerMasterDataData.fld_Kdaktf = pkjmastModelViewEdit.fld_Kdaktf;
                    estateWorkerMasterDataData.fld_Trtakf = pkjmastModelViewEdit.fld_Trtakf;
                    estateWorkerMasterDataData.fld_Sbtakf = pkjmastModelViewEdit.fld_Sbtakf;
                    estateWorkerMasterDataData.fld_Almt1 = pkjmastModelViewEdit.fld_Almt1;
                    estateWorkerMasterDataData.fld_Poskod = pkjmastModelViewEdit.fld_Poskod;
                    estateWorkerMasterDataData.fld_Daerah = pkjmastModelViewEdit.fld_Daerah;
                    estateWorkerMasterDataData.fld_Neg = pkjmastModelViewEdit.fld_Neg;
                    estateWorkerMasterDataData.fld_Negara = pkjmastModelViewEdit.fld_Negara;
                    estateWorkerMasterDataData.fld_Notel = pkjmastModelViewEdit.fld_Notel;
                    estateWorkerMasterDataData.fld_Nofax = pkjmastModelViewEdit.fld_Nofax;
                    estateWorkerMasterDataData.fld_Trmlkj = pkjmastModelViewEdit.fld_Trmlkj;
                    estateWorkerMasterDataData.fld_Trshjw = pkjmastModelViewEdit.fld_Trshjw;
                    estateWorkerMasterDataData.fld_Jenispekerja = pkjmastModelViewEdit.fld_Jenispekerja;
                    estateWorkerMasterDataData.fld_Ktgpkj = pkjmastModelViewEdit.fld_Ktgpkj;
                    estateWorkerMasterDataData.fld_Kodbkl = pkjmastModelViewEdit.fld_Kodbkl;
                    estateWorkerMasterDataData.fld_StatusKwspSocso = pkjmastModelViewEdit.fld_StatusKwspSocso;
                    estateWorkerMasterDataData.fld_KodKWSP = pkjmastModelViewEdit.fld_KodKWSP;
                    estateWorkerMasterDataData.fld_Nokwsp = pkjmastModelViewEdit.fld_Nokwsp;
                    estateWorkerMasterDataData.fld_KodSocso = pkjmastModelViewEdit.fld_KodSocso;
                    estateWorkerMasterDataData.fld_Noperkeso = pkjmastModelViewEdit.fld_Noperkeso;
                    estateWorkerMasterDataData.fld_StatusAkaun = pkjmastModelViewEdit.fld_StatusAkaun;
                    estateWorkerMasterDataData.fld_Kdbank = pkjmastModelViewEdit.fld_Kdbank;
                    estateWorkerMasterDataData.fld_NoAkaun = pkjmastModelViewEdit.fld_NoAkaun;

                    estateConnection.SaveChanges();

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
                        method = "3",
                        div = "estateWorkerMasterDataMaintenanceDetails",
                        rootUrl = domain,
                        action = "_EstateWorkerMasterDataMaintenance",
                        controller = "Maintenance",
                        paramName = "WilayahList",
                        paramValue = pkjmastModelViewEdit.fld_WilayahID,
                        paramName2 = "LadangList",
                        paramValue2 = pkjmastModelViewEdit.fld_LadangID
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

        public ActionResult EstateWorkerProductivityRegistrationMaintenance()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.EstateDataManage = "class = active";

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
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            return View();
        }

        public ActionResult _EstateWorkerProductivityRegistrationMaintenance(int? WilayahList, int? LadangList, int? YearList, int? MonthList, int page = 1, string sort = "fldOptConfFlag1", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<tbl_Produktiviti>();
            int role = GetIdentity.RoleID(getuserid).Value;

            List<tbl_Produktiviti> produktivitiList = new List<tbl_Produktiviti>();

            var message = "";
            var estateAccountStatus = false;

            if (!String.IsNullOrEmpty(WilayahList.ToString()) && !String.IsNullOrEmpty(LadangList.ToString()))
            {
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

                var estateAccountStatusData = estateConnection.tbl_TutupUrusNiaga.SingleOrDefault(x =>
                    x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList);

                if (estateAccountStatusData != null)
                {
                    estateAccountStatus = (bool)estateAccountStatusData.fld_StsTtpUrsNiaga;
                }

                var estateWorkerData = estateConnection.tbl_Pkjmast.Where(x =>
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList &&
                    x.fld_LadangID == LadangList && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama);

                foreach (var estateWorker in estateWorkerData)
                {
                    var estateWorkerProductivityData = estateConnection.tbl_Produktiviti
                        .SingleOrDefault(a => a.fld_Nopkj == estateWorker.fld_Nopkj && a.fld_Year == YearList &&
                                    a.fld_Month == MonthList && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahList &&
                                    a.fld_LadangID == LadangList && a.fld_Deleted == false);

                    if (estateWorkerProductivityData != null)
                    {
                        produktivitiList.Add(new tbl_Produktiviti()
                        {
                            fld_Nopkj = estateWorkerProductivityData.fld_Nopkj,
                            fld_JenisPelan = estateWorkerProductivityData.fld_JenisPelan,
                            fld_Targetharian = estateWorkerProductivityData.fld_Targetharian,
                            fld_Unit = estateWorkerProductivityData.fld_Unit,
                            fld_HadirKerja = estateWorkerProductivityData.fld_HadirKerja,
                            fld_Year = estateWorkerProductivityData.fld_Year,
                            fld_Month = estateWorkerProductivityData.fld_Month,
                            fld_NegaraID = estateWorkerProductivityData.fld_NegaraID,
                            fld_SyarikatID = estateWorkerProductivityData.fld_SyarikatID,
                            fld_WilayahID = estateWorkerProductivityData.fld_WilayahID,
                            fld_LadangID = estateWorkerProductivityData.fld_LadangID,
                            fld_Deleted = estateWorkerProductivityData.fld_Deleted,
                            fld_ProduktivitifID = estateWorkerProductivityData.fld_ProduktivitifID
                        });
                    }

                    else
                    {
                        produktivitiList.Add(new tbl_Produktiviti()
                        {
                            fld_Nopkj = estateWorker.fld_Nopkj,
                            fld_NegaraID = estateWorker.fld_NegaraID,
                            fld_SyarikatID = estateWorker.fld_SyarikatID,
                            fld_WilayahID = estateWorker.fld_WilayahID,
                            fld_LadangID = estateWorker.fld_LadangID
                        });
                    }
                }

                message = GlobalResCorp.msgNoRecord;
            }

            else
            {
                message = GlobalResCorp.lblChooseEsateIncentiveEligibility;
            }

            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            ViewBag.Message = message;
            ViewBag.EstateAccountStatus = estateAccountStatus;
            ViewBag.Month = MonthList;
            ViewBag.Year = YearList;

            return View(produktivitiList);
        }

        public ActionResult _EstateWorkerProductivityRegistrationMaintenanceCreate(string nopkj, int EstateWilayahID, int EstateLadangID, int Year, int Month)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ModelsEstate.tbl_ProduktivitiViewModelCreate produktivitiViewModelCreate = new tbl_ProduktivitiViewModelCreate();

            produktivitiViewModelCreate.fld_Nopkj = nopkj;
            produktivitiViewModelCreate.fld_Year = Year;
            produktivitiViewModelCreate.fld_Month = Month;
            produktivitiViewModelCreate.fld_NegaraID = NegaraID;
            produktivitiViewModelCreate.fld_SyarikatID = SyarikatID;
            produktivitiViewModelCreate.fld_WilayahID = EstateWilayahID;
            produktivitiViewModelCreate.fld_LadangID = EstateLadangID;

            var estateWorkingDay = db.tbl_HariBekerjaLadang.SingleOrDefault(x =>
                x.fld_Year == Year && x.fld_Month == Month &&
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == EstateWilayahID &&
                x.fld_LadangID == EstateLadangID && x.fld_Deleted == false);

            if (estateWorkingDay != null)
            {
                produktivitiViewModelCreate.fld_HadirKerja = estateWorkingDay.fld_BilHariBekerja;
            }

            List<SelectListItem> jenisPelanList = new List<SelectListItem>();

            jenisPelanList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jenisPelan" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            jenisPelanList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

            ViewBag.jenisPelanList = jenisPelanList;

            List<SelectListItem> UnitList = new List<SelectListItem>();

            UnitList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "unit" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            UnitList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

            ViewBag.UnitList = UnitList;

            return PartialView(produktivitiViewModelCreate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateWorkerProductivityRegistrationMaintenanceCreate(ModelsEstate.tbl_ProduktivitiViewModelCreate produktivitiViewModelCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, produktivitiViewModelCreate.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    tbl_Produktiviti produktiviti = new tbl_Produktiviti();

                    PropertyCopy.Copy(produktiviti, produktivitiViewModelCreate);

                    produktiviti.fld_Deleted = false;
                    produktiviti.fld_CreatedBy = getuserid;
                    produktiviti.fld_CreatedDT = timezone.gettimezone();

                    estateConnection.tbl_Produktiviti.Add(produktiviti);
                    estateConnection.SaveChanges();

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
                        method = "5",
                        div = "estateWorkerProductivityRegistrationMaintenanceDetails",
                        rootUrl = domain,
                        action = "_EstateWorkerProductivityRegistrationMaintenance",
                        controller = "Maintenance",
                        paramName = "WilayahList",
                        paramValue = produktivitiViewModelCreate.fld_WilayahID,
                        paramName2 = "LadangList",
                        paramValue2 = produktivitiViewModelCreate.fld_LadangID,
                        paramName3 = "YearList",
                        paramValue3 = produktivitiViewModelCreate.fld_Year,
                        paramName4 = "MonthList",
                        paramValue4 = produktivitiViewModelCreate.fld_Month
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

        public ActionResult _EstateWorkerProductivityRegistrationMaintenanceEdit(Guid produktivitiID, int EstateWilayahID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, EstateWilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            var productivityData =
                estateConnection.tbl_Produktiviti.SingleOrDefault(x => x.fld_ProduktivitifID == produktivitiID);

            tbl_ProduktivitiViewModelCreate produktivitiViewModelEdit = new tbl_ProduktivitiViewModelCreate();

            PropertyCopy.Copy(produktivitiViewModelEdit, productivityData);

            var estateWorkingDay = db.tbl_HariBekerjaLadang.SingleOrDefault(x =>
                x.fld_Year == productivityData.fld_Year && x.fld_Month == productivityData.fld_Month &&
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == EstateWilayahID &&
                x.fld_LadangID == productivityData.fld_LadangID && x.fld_Deleted == false);

            if (estateWorkingDay != null)
            {
                produktivitiViewModelEdit.fld_HadirKerja = estateWorkingDay.fld_BilHariBekerja;
            }

            List<SelectListItem> jenisPelanList = new List<SelectListItem>();

            jenisPelanList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jenisPelan" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            jenisPelanList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

            ViewBag.jenisPelanList = jenisPelanList;

            List<SelectListItem> UnitList = new List<SelectListItem>();

            UnitList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "unit" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            UnitList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

            ViewBag.UnitList = UnitList;

            return PartialView(produktivitiViewModelEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateWorkerProductivityRegistrationMaintenanceEdit(ModelsEstate.tbl_ProduktivitiViewModelCreate produktivitiViewModelEdit)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, produktivitiViewModelEdit.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    var productivityData = estateConnection.tbl_Produktiviti.SingleOrDefault(x =>
                        x.fld_ProduktivitifID == produktivitiViewModelEdit.fld_ProduktivitifID);

                    productivityData.fld_JenisPelan = produktivitiViewModelEdit.fld_JenisPelan;
                    productivityData.fld_Targetharian = produktivitiViewModelEdit.fld_Targetharian;
                    productivityData.fld_Unit = produktivitiViewModelEdit.fld_Unit;
                    productivityData.fld_HadirKerja = produktivitiViewModelEdit.fld_HadirKerja;
                    productivityData.fld_Year = produktivitiViewModelEdit.fld_Year;
                    productivityData.fld_Month = produktivitiViewModelEdit.fld_Month;
                    productivityData.fld_CreatedBy = getuserid;
                    productivityData.fld_CreatedDT = timezone.gettimezone();
                    estateConnection.Entry(productivityData).State = EntityState.Modified;
                    estateConnection.SaveChanges();

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
                        method = "5",
                        div = "estateWorkerProductivityRegistrationMaintenanceDetails",
                        rootUrl = domain,
                        action = "_EstateWorkerProductivityRegistrationMaintenance",
                        controller = "Maintenance",
                        paramName = "WilayahList",
                        paramValue = produktivitiViewModelEdit.fld_WilayahID,
                        paramName2 = "LadangList",
                        paramValue2 = produktivitiViewModelEdit.fld_LadangID,
                        paramName3 = "YearList",
                        paramValue3 = produktivitiViewModelEdit.fld_Year,
                        paramName4 = "MonthList",
                        paramValue4 = produktivitiViewModelEdit.fld_Month
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

        public ActionResult _EstateWorkerProductivityRegistrationMaintenanceDelete(Guid produktivitiID, int EstateWilayahID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, EstateWilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            var productivityData =
                estateConnection.tbl_Produktiviti.SingleOrDefault(x => x.fld_ProduktivitifID == produktivitiID);

            return PartialView(productivityData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateWorkerProductivityRegistrationMaintenanceDelete(ModelsEstate.tbl_Produktiviti produktiviti)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, produktiviti.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            try
            {

                var productivityData = estateConnection.tbl_Produktiviti.SingleOrDefault(x =>
                    x.fld_ProduktivitifID == produktiviti.fld_ProduktivitifID);

                productivityData.fld_Deleted = true;

                estateConnection.SaveChanges();

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
                    method = "5",
                    div = "estateWorkerProductivityRegistrationMaintenanceDetails",
                    rootUrl = domain,
                    action = "_EstateWorkerProductivityRegistrationMaintenance",
                    controller = "Maintenance",
                    paramName = "WilayahList",
                    paramValue = produktiviti.fld_WilayahID,
                    paramName2 = "LadangList",
                    paramValue2 = produktiviti.fld_LadangID,
                    paramName3 = "YearList",
                    paramValue3 = produktiviti.fld_Year,
                    paramName4 = "MonthList",
                    paramValue4 = produktiviti.fld_Month
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

        public ActionResult _EstateWorkerProductivityRegistrationMaintenanceCopy()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

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

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearList = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            yearList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.YearList = yearList;

            var monthList = new List<SelectListItem>();
            monthList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            monthList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.MonthList = monthList;

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateWorkerProductivityRegistrationMaintenanceCopy(ModelsEstate.tbl_ProduktivitiViewModelCopy produktivitiViewModelCopy)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, produktivitiViewModelCopy.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    List<tbl_Produktiviti> produktivitiList = new List<tbl_Produktiviti>();

                    var estateAccountStatusData = estateConnection.tbl_TutupUrusNiaga.SingleOrDefault(x =>
                        x.fld_Year == produktivitiViewModelCopy.fld_YearTo &&
                        x.fld_Month == produktivitiViewModelCopy.fld_MonthTo &&
                        x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                        x.fld_WilayahID == produktivitiViewModelCopy.fld_WilayahID &&
                        x.fld_LadangID == produktivitiViewModelCopy.fld_LadangID);

                    if (estateAccountStatusData == null || estateAccountStatusData.fld_StsTtpUrsNiaga == false)
                    {
                        var workerProductivityData = estateConnection.tbl_Produktiviti.Where(x =>
                            x.fld_Year == produktivitiViewModelCopy.fld_YearFrom &&
                            x.fld_Month == produktivitiViewModelCopy.fld_MonthFrom && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == produktivitiViewModelCopy.fld_WilayahID &&
                            x.fld_LadangID == produktivitiViewModelCopy.fld_LadangID && x.fld_Deleted == false);

                        var estateWorkingDay = db.tbl_HariBekerjaLadang.SingleOrDefault(x =>
                            x.fld_Year == produktivitiViewModelCopy.fld_YearTo &&
                            x.fld_Month == produktivitiViewModelCopy.fld_MonthTo && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == produktivitiViewModelCopy.fld_WilayahID &&
                            x.fld_LadangID == produktivitiViewModelCopy.fld_LadangID && x.fld_Deleted == false);

                        if (workerProductivityData.Count() != 0)
                        {
                            foreach (var workerProduktivity in workerProductivityData)
                            {
                                var checkExistingProductivity = estateConnection.tbl_Produktiviti.SingleOrDefault(x =>
                                    x.fld_Nopkj == workerProduktivity.fld_Nopkj &&
                                    x.fld_Year == produktivitiViewModelCopy.fld_YearTo &&
                                    x.fld_Month == produktivitiViewModelCopy.fld_MonthTo &&
                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                    x.fld_WilayahID == workerProduktivity.fld_WilayahID &&
                                    x.fld_LadangID == workerProduktivity.fld_LadangID && x.fld_Deleted == false);

                                if (checkExistingProductivity == null)
                                {
                                    tbl_Produktiviti produktiviti = new tbl_Produktiviti();

                                    produktiviti.fld_Nopkj = workerProduktivity.fld_Nopkj;
                                    produktiviti.fld_JenisPelan = workerProduktivity.fld_JenisPelan;
                                    produktiviti.fld_Targetharian = workerProduktivity.fld_Targetharian;
                                    produktiviti.fld_Unit = workerProduktivity.fld_Unit;
                                    produktiviti.fld_Year = produktivitiViewModelCopy.fld_YearTo;
                                    produktiviti.fld_Month = produktivitiViewModelCopy.fld_MonthTo;
                                    produktiviti.fld_NegaraID = NegaraID;
                                    produktiviti.fld_SyarikatID = SyarikatID;
                                    produktiviti.fld_WilayahID = workerProduktivity.fld_WilayahID;
                                    produktiviti.fld_LadangID = workerProduktivity.fld_LadangID;
                                    produktiviti.fld_Deleted = false;
                                    produktiviti.fld_CreatedBy = getuserid;
                                    produktiviti.fld_CreatedDT = timezone.gettimezone();

                                    if (estateWorkingDay != null)
                                    {
                                        produktiviti.fld_HadirKerja = estateWorkingDay.fld_BilHariBekerja;
                                    }

                                    else
                                    {
                                        produktiviti.fld_HadirKerja = 0;
                                    }

                                    produktivitiList.Add(produktiviti);
                                }
                            }

                            if (produktivitiList.Count != 0)
                            {
                                estateConnection.tbl_Produktiviti.AddRange(produktivitiList);
                                estateConnection.SaveChanges();
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
                                msg = GlobalResCorp.msgAdd,
                                status = "success",
                                checkingdata = "0",
                                method = "5",
                                div = "estateWorkerProductivityRegistrationMaintenanceDetails",
                                rootUrl = domain,
                                action = "_EstateWorkerProductivityRegistrationMaintenance",
                                controller = "Maintenance",
                                paramName = "WilayahList",
                                paramValue = produktivitiViewModelCopy.fld_WilayahID,
                                paramName2 = "LadangList",
                                paramValue2 = produktivitiViewModelCopy.fld_LadangID,
                                paramName3 = "YearList",
                                paramValue3 = produktivitiViewModelCopy.fld_YearTo,
                                paramName4 = "MonthList",
                                paramValue4 = produktivitiViewModelCopy.fld_MonthTo
                            });
                        }

                        else
                        {
                            return Json(new
                            {
                                success = false,
                                msg = GlobalResCorp.msgAlert3,
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
                            msg = GlobalResCorp.msgAlert2,
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

        public ActionResult EstateWorkerIncentiveRegistrationMaintenance()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.EstateDataManage = "class = active";

            List<SelectListItem> wilayahList = new List<SelectListItem>();
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
            //end

            wilayahList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.WilayahList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();
            ladangList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
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
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            return View();
        }

        public ActionResult _EstateWorkerIncentiveRegistrationMaintenance(int? WilayahList, int? LadangList, int? YearList, int? MonthList, int page = 1, string sort = "fldOptConfFlag1", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<CustMod_WorkerIncentiveList>();
            int role = GetIdentity.RoleID(getuserid).Value;

            List<CustMod_WorkerIncentiveList> WorkerIncentiveList = new List<CustMod_WorkerIncentiveList>();

            var message = "";
            var estateAccountStatus = false;

            if (!String.IsNullOrEmpty(WilayahList.ToString()) && !String.IsNullOrEmpty(LadangList.ToString()))
            {
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

                var estateAccountStatusData = estateConnection.tbl_TutupUrusNiaga.SingleOrDefault(x =>
                    x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList);

                if (estateAccountStatusData != null)
                {
                    estateAccountStatus = (bool)estateAccountStatusData.fld_StsTtpUrsNiaga;
                }

                var estateWorkerData = estateConnection.tbl_Pkjmast.Where(x =>
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList &&
                    x.fld_LadangID == LadangList && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama);

                foreach (var estateWorker in estateWorkerData)
                {
                    var estateWorkerIncentiveData = estateConnection.tbl_Insentif
                        .Where(a => a.fld_Nopkj == estateWorker.fld_Nopkj && a.fld_Year == YearList &&
                                    a.fld_Month == MonthList && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahList &&
                                    a.fld_LadangID == LadangList && a.fld_Deleted == false)
                        .OrderBy(x => x.fld_KodInsentif)
                        .ToList();
                    WorkerIncentiveList.Add(new CustMod_WorkerIncentiveList { Pkjmast = estateWorker, Insentif = estateWorkerIncentiveData });
                }

                message = GlobalResCorp.msgNoRecord;
            }

            else
            {
                message = GlobalResCorp.lblChooseEsateIncentiveEligibility;
            }

            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            ViewBag.Message = message;
            ViewBag.EstateAccountStatus = estateAccountStatus;
            ViewBag.Month = MonthList;
            ViewBag.Year = YearList;

            return View(WorkerIncentiveList);
        }

        public ActionResult _EstateWorkerIncentiveRegistrationMaintenanceCreate(string nopkj, int EstateWilayahID, int EstateLadangID, int Year, int Month)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> incentiveTypeList = new List<SelectListItem>();

            incentiveTypeList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jenisInsentif" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            incentiveTypeList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

            ViewBag.IncentiveTypeList = incentiveTypeList;

            List<SelectListItem> incentiveList = new List<SelectListItem>();
            incentiveList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

            ViewBag.IncentiveList = incentiveList;

            tbl_InsentifViewModelCreate insentif = new tbl_InsentifViewModelCreate();

            insentif.fld_Nopkj = nopkj;
            insentif.fld_Year = Year;
            insentif.fld_Month = Month;
            insentif.fld_NegaraID = NegaraID;
            insentif.fld_SyarikatID = SyarikatID;
            insentif.fld_WilayahID = EstateWilayahID;
            insentif.fld_LadangID = EstateLadangID;

            return PartialView(insentif);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateWorkerIncentiveRegistrationMaintenanceCreate(ModelsEstate.tbl_InsentifViewModelCreate insentifViewModelCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, insentifViewModelCreate.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    tbl_Insentif insentif = new tbl_Insentif();

                    PropertyCopy.Copy(insentif, insentifViewModelCreate);

                    insentif.fld_Deleted = false;
                    insentif.fld_CreatedBy = getuserid;
                    insentif.fld_CreatedDT = timezone.gettimezone();

                    if (insentifViewModelCreate.fld_TetapanNilai == 0)
                    {
                        if (GetConfig.GetIncentiveIsValidRange(insentifViewModelCreate.fld_KodInsentif,
                            (decimal)insentifViewModelCreate.fld_NilaiTidakTetap, NegaraID, SyarikatID))
                        {
                            insentif.fld_NilaiInsentif = insentifViewModelCreate.fld_NilaiTidakTetap;
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

                    else if (insentifViewModelCreate.fld_TetapanNilai == 1)
                    {
                        insentif.fld_NilaiInsentif = insentifViewModelCreate.fld_NilaiTetap;
                    }

                    else
                    {
                        insentif.fld_NilaiInsentif = insentifViewModelCreate.fld_NilaiHarian;
                    }

                    estateConnection.tbl_Insentif.Add(insentif);
                    estateConnection.SaveChanges();

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
                        method = "5",
                        div = "estateWorkerIncentiveRegistrationMaintenanceDetails",
                        rootUrl = domain,
                        action = "_EstateWorkerIncentiveRegistrationMaintenance",
                        controller = "Maintenance",
                        paramName = "WilayahList",
                        paramValue = insentifViewModelCreate.fld_WilayahID,
                        paramName2 = "LadangList",
                        paramValue2 = insentifViewModelCreate.fld_LadangID,
                        paramName3 = "YearList",
                        paramValue3 = insentifViewModelCreate.fld_Year,
                        paramName4 = "MonthList",
                        paramValue4 = insentifViewModelCreate.fld_Month
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

        public ActionResult _EstateWorkerIncentiveRegistrationMaintenanceEdit(Guid InsentifID, int EstateWilayahID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, EstateWilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            var incentiveData =
                estateConnection.tbl_Insentif.SingleOrDefault(x => x.fld_InsentifID == InsentifID);

            tbl_InsentifViewModelCreate incentiveViewModelEdit = new tbl_InsentifViewModelCreate();

            PropertyCopy.Copy(incentiveViewModelEdit, incentiveData);

            List<SelectListItem> incentiveTypeList = new List<SelectListItem>();

            incentiveTypeList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jenisInsentif" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text", incentiveViewModelEdit.fld_JenisInsentif).ToList();

            ViewBag.IncentiveTypeList = incentiveTypeList;

            List<SelectListItem> incentiveList = new List<SelectListItem>();

            incentiveList = new SelectList(db.tbl_JenisInsentif
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_KodInsentif == incentiveViewModelEdit.fld_KodInsentif).Select(
                        s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }),
                "Value", "Text", incentiveViewModelEdit.fld_KodInsentif).ToList();

            ViewBag.IncentiveList = incentiveList;

            return PartialView(incentiveViewModelEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateWorkerIncentiveRegistrationMaintenanceEdit(ModelsEstate.tbl_InsentifViewModelCreate incentiveViewModelEdit)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, incentiveViewModelEdit.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                    var incentiveData = estateConnection.tbl_Insentif.SingleOrDefault(x =>
                        x.fld_InsentifID == incentiveViewModelEdit.fld_InsentifID);

                    incentiveData.fld_Month = incentiveViewModelEdit.fld_Month;
                    incentiveData.fld_Year = incentiveViewModelEdit.fld_Year;
                    estateConnection.Entry(incentiveData).State = EntityState.Modified;
                    estateConnection.SaveChanges();

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
                        method = "5",
                        div = "estateWorkerIncentiveRegistrationMaintenanceDetails",
                        rootUrl = domain,
                        action = "_EstateWorkerIncentiveRegistrationMaintenance",
                        controller = "Maintenance",
                        paramName = "WilayahList",
                        paramValue = incentiveViewModelEdit.fld_WilayahID,
                        paramName2 = "LadangList",
                        paramValue2 = incentiveViewModelEdit.fld_LadangID,
                        paramName3 = "YearList",
                        paramValue3 = incentiveViewModelEdit.fld_Year,
                        paramName4 = "MonthList",
                        paramValue4 = incentiveViewModelEdit.fld_Month
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

        public ActionResult _EstateWorkerIncentiveRegistrationMaintenanceDelete(Guid id, int wilayahID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, wilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            var estateWorkerIncentiveData = estateConnection.tbl_Insentif.SingleOrDefault(x => x.fld_InsentifID == id);

            return PartialView(estateWorkerIncentiveData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateWorkerIncentiveRegistrationMaintenanceDelete(ModelsEstate.tbl_Insentif insentif)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, insentif.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var estateWorkerIncentiveData =
                    estateConnection.tbl_Insentif.SingleOrDefault(x => x.fld_InsentifID == insentif.fld_InsentifID);

                estateWorkerIncentiveData.fld_Deleted = true;

                estateConnection.SaveChanges();

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
                    msg = GlobalResCorp.msgDelete2,
                    status = "success",
                    checkingdata = "0",
                    method = "5",
                    div = "estateWorkerIncentiveRegistrationMaintenanceDetails",
                    rootUrl = domain,
                    action = "_EstateWorkerIncentiveRegistrationMaintenance",
                    controller = "Maintenance",
                    paramName = "WilayahList",
                    paramValue = estateWorkerIncentiveData.fld_WilayahID,
                    paramName2 = "LadangList",
                    paramValue2 = estateWorkerIncentiveData.fld_LadangID,
                    paramName3 = "YearList",
                    paramValue3 = estateWorkerIncentiveData.fld_Year,
                    paramName4 = "MonthList",
                    paramValue4 = estateWorkerIncentiveData.fld_Month
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

        public ActionResult _EstateWorkerIncentiveRegistrationMaintenanceCopy()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

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

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearList = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            yearList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.YearList = yearList;

            var monthList = new List<SelectListItem>();
            monthList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            monthList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.MonthList = monthList;

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateWorkerIncentiveRegistrationMaintenanceCopy(ModelsEstate.tbl_InsentifViewModelCopy insentifViewModelCopy)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, insentifViewModelCopy.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    List<tbl_Insentif> insentifList = new List<tbl_Insentif>();

                    var estateAccountStatusData = estateConnection.tbl_TutupUrusNiaga.SingleOrDefault(x =>
                        x.fld_Year == insentifViewModelCopy.fld_YearTo &&
                        x.fld_Month == insentifViewModelCopy.fld_MonthTo &&
                        x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                        x.fld_WilayahID == insentifViewModelCopy.fld_WilayahID &&
                        x.fld_LadangID == insentifViewModelCopy.fld_LadangID);

                    if (estateAccountStatusData == null || estateAccountStatusData.fld_StsTtpUrsNiaga == false)
                    {
                        var workerIncentiveData = estateConnection.tbl_Insentif.Where(x =>
                            x.fld_Year == insentifViewModelCopy.fld_YearFrom &&
                            x.fld_Month == insentifViewModelCopy.fld_MonthFrom && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == insentifViewModelCopy.fld_WilayahID &&
                            x.fld_LadangID == insentifViewModelCopy.fld_LadangID && x.fld_Deleted == false);

                        if (workerIncentiveData.Count() != 0)
                        {
                            foreach (var workerIncentive in workerIncentiveData)
                            {
                                var checkExistingIncentive = estateConnection.tbl_Insentif.SingleOrDefault(x =>
                                    x.fld_Nopkj == workerIncentive.fld_Nopkj &&
                                    x.fld_KodInsentif == workerIncentive.fld_KodInsentif &&
                                    x.fld_Year == insentifViewModelCopy.fld_YearTo &&
                                    x.fld_Month == insentifViewModelCopy.fld_MonthTo &&
                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                    x.fld_WilayahID == workerIncentive.fld_WilayahID &&
                                    x.fld_LadangID == workerIncentive.fld_LadangID && x.fld_Deleted == false);

                                if (checkExistingIncentive == null)
                                {
                                    tbl_Insentif insentif = new tbl_Insentif();

                                    insentif.fld_Nopkj = workerIncentive.fld_Nopkj;
                                    insentif.fld_KodInsentif = workerIncentive.fld_KodInsentif;
                                    insentif.fld_NilaiInsentif = workerIncentive.fld_NilaiInsentif;
                                    insentif.fld_Year = insentifViewModelCopy.fld_YearTo;
                                    insentif.fld_Month = insentifViewModelCopy.fld_MonthTo;
                                    insentif.fld_NegaraID = NegaraID;
                                    insentif.fld_SyarikatID = SyarikatID;
                                    insentif.fld_WilayahID = workerIncentive.fld_WilayahID;
                                    insentif.fld_LadangID = workerIncentive.fld_LadangID;
                                    insentif.fld_Deleted = false;
                                    insentif.fld_CreatedBy = getuserid;
                                    insentif.fld_CreatedDT = timezone.gettimezone();

                                    insentifList.Add(insentif);
                                }
                            }

                            if (insentifList.Count != 0)
                            {
                                estateConnection.tbl_Insentif.AddRange(insentifList);
                                estateConnection.SaveChanges();
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
                                msg = GlobalResCorp.msgAdd,
                                status = "success",
                                checkingdata = "0",
                                method = "5",
                                div = "estateWorkerIncentiveRegistrationMaintenanceDetails",
                                rootUrl = domain,
                                action = "_EstateWorkerIncentiveRegistrationMaintenance",
                                controller = "Maintenance",
                                paramName = "WilayahList",
                                paramValue = insentifViewModelCopy.fld_WilayahID,
                                paramName2 = "LadangList",
                                paramValue2 = insentifViewModelCopy.fld_LadangID,
                                paramName3 = "YearList",
                                paramValue3 = insentifViewModelCopy.fld_YearTo,
                                paramName4 = "MonthList",
                                paramValue4 = insentifViewModelCopy.fld_MonthTo
                            });
                        }

                        else
                        {
                            return Json(new
                            {
                                success = false,
                                msg = GlobalResCorp.msgAlert3,
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
                            msg = GlobalResCorp.msgAlert2,
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

        //fatin added - 25/04/2023
        public ActionResult EstateLevelTemporaryManagement(int page = 1, string sort = "fld_CreatedDT", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.EstateMaintenance = "class = active";

            List<SelectListItem> wilayahList = new List<SelectListItem>();
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
            wilayahList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.WilayahList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();
            ladangList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.LadangList = ladangList;

            return View();
        }

        public ActionResult _EstateLevelTemporaryManagementList(int? WilayahList, int? LadangList, int page = 1, string sort = "fld_CreatedDT", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<tbl_PktPinjam>();
            int role = GetIdentity.RoleID(getuserid).Value;
            var message = "";

            List<tbl_PktPinjam> GetTransferPkt = new List<tbl_PktPinjam>();

            if (!String.IsNullOrEmpty(WilayahList.ToString()) && !String.IsNullOrEmpty(LadangList.ToString()))
            {
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
                GetTransferPkt = estateConnection.tbl_PktPinjam.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList).ToList();
                message = GlobalResCorp.msgNoRecord;
            }

            else
            {
                message = "Sila pilih untuk mencari data";
            }


            records.Content = GetTransferPkt;
            records.TotalRecords = GetTransferPkt.Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;
            ViewBag.Message = message;

            return View(records);
        }

        public ActionResult _EstateLevelTemporaryManagementCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.EstateMaintenance = "class = active";

            var GetWilayah = db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).ToList();

            List<SelectListItem> wilayahList1 = new List<SelectListItem>();
            wilayahList1 = new SelectList(
                GetWilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            wilayahList1.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.WilayahList1 = wilayahList1;

            List<SelectListItem> ladangList1 = new List<SelectListItem>();
            ladangList1.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.LadangList1 = ladangList1;

            //List<SelectListItem> JnisAktvt = new List<SelectListItem>();
            ////var GetJenisAktvtyDesc = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "activityLevel" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new { s.fldOptConfValue, s.fldOptConfDesc }).ToList(); JnisAktvt = new SelectList(GetJenisAktvtyDesc.Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            //var tbl_JenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            //JnisAktvt = new SelectList(tbl_JenisAktiviti.OrderBy(o => o.fld_KodJnsAktvt).Select(s => new SelectListItem { Value = s.fld_KodJnsAktvt, Text = s.fld_KodJnsAktvt + " - " + s.fld_Desc }), "Value", "Text").ToList();
            //JnisAktvt.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" }));
            //ViewBag.JnisAktvt = JnisAktvt;

            List<SelectListItem> JnisPkt = new List<SelectListItem>();
            JnisPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnisPkt.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.JnisPkt = JnisPkt;

            List<SelectListItem> PilihanPkt = new List<SelectListItem>();
            PilihanPkt.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.PilihanPkt = PilihanPkt;

            List<SelectListItem> wilayahList2 = new List<SelectListItem>();
            wilayahList2 = new SelectList(
                GetWilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            wilayahList2.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.wilayahList2 = wilayahList2;

            List<SelectListItem> ladangList2 = new List<SelectListItem>();
            ladangList2.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.ladangList2 = ladangList2;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateLevelTemporaryManagementCreate(CustMod_TransferPkt CustMod_TransferPkt)
        {
            if (CustMod_TransferPkt.ladangList1 != CustMod_TransferPkt.ladangList2)
            {
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = GetIdentity.ID(User.Identity.Name);
                DateTime? CurrentDT = timezone.gettimezone();
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, CustMod_TransferPkt.wilayahList1, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate EstateConnFrom = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

                Connection.GetConnection(out host, out catalog, out user, out pass, CustMod_TransferPkt.wilayahList2, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate EstateConnTo = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
                int NegaraIDAsl = 0;
                int SyarikatIDAsal = 0;
                int WilayahIDAsal = 0;
                int LadangIDAsal = 0;
                int DivisionIDAsal = 0;
                string KodPkt = "";
                string NamaPkt = "";
                string SAPCode = "";
                string JenisLot = "";
                tbl_PktPinjam tbl_PktPinjam = new tbl_PktPinjam();
                switch (CustMod_TransferPkt.JnisPkt)
                {
                    case 1:
                        var SelectPkt = EstateConnFrom.tbl_PktUtama.Where(x => x.fld_ID == CustMod_TransferPkt.PilihanPkt && x.fld_Deleted == false).FirstOrDefault();
                        NegaraIDAsl = SelectPkt.fld_NegaraID.Value;
                        SyarikatIDAsal = SelectPkt.fld_SyarikatID.Value;
                        WilayahIDAsal = SelectPkt.fld_WilayahID.Value;
                        LadangIDAsal = SelectPkt.fld_LadangID.Value;
                        //DivisionIDAsal = SelectPkt.fld_DivisionID.Value;
                        KodPkt = SelectPkt.fld_PktUtama;
                        NamaPkt = SelectPkt.fld_NamaPktUtama;
                        SAPCode = SelectPkt.fld_IOcode;
                        JenisLot = SelectPkt.fld_JnsLot;
                        break;
                    case 2:
                        var SelectPkt2 = EstateConnFrom.tbl_SubPkt.Join(EstateConnFrom.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktUtama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Pkt, j.fld_NamaPkt, j.fld_Deleted, k.fld_IOcode, j.fld_ID }).Where(x => x.fld_ID == CustMod_TransferPkt.PilihanPkt && x.fld_Deleted == false).FirstOrDefault();
                        NegaraIDAsl = SelectPkt2.fld_NegaraID.Value;
                        SyarikatIDAsal = SelectPkt2.fld_SyarikatID.Value;
                        WilayahIDAsal = SelectPkt2.fld_WilayahID.Value;
                        LadangIDAsal = SelectPkt2.fld_LadangID.Value;
                        //DivisionIDAsal = SelectPkt2.fld_DivisionID.Value;
                        KodPkt = SelectPkt2.fld_Pkt;
                        NamaPkt = SelectPkt2.fld_NamaPkt;
                        SAPCode = SelectPkt2.fld_IOcode;
                        JenisLot = SelectPkt2.fld_JnsLot;
                        break;
                    case 3:
                        var SelectPkt3 = EstateConnFrom.tbl_Blok.Join(EstateConnFrom.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktutama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Blok, j.fld_NamaBlok, j.fld_Deleted, k.fld_IOcode, j.fld_ID }).Where(x => x.fld_ID == CustMod_TransferPkt.PilihanPkt && x.fld_Deleted == false).FirstOrDefault();
                        NegaraIDAsl = SelectPkt3.fld_NegaraID.Value;
                        SyarikatIDAsal = SelectPkt3.fld_SyarikatID.Value;
                        WilayahIDAsal = SelectPkt3.fld_WilayahID.Value;
                        LadangIDAsal = SelectPkt3.fld_LadangID.Value;
                        //DivisionIDAsal = SelectPkt3.fld_DivisionID.Value;
                        KodPkt = SelectPkt3.fld_Blok;
                        NamaPkt = SelectPkt3.fld_NamaBlok;
                        SAPCode = SelectPkt3.fld_IOcode;
                        JenisLot = SelectPkt3.fld_JnsLot;
                        break;
                }

                //fatin added - 10/10/2023
                var pktPinjamExisting = EstateConnFrom.tbl_PktPinjam.SingleOrDefault(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == CustMod_TransferPkt.wilayahList2 && x.fld_LadangID == CustMod_TransferPkt.ladangList2 && x.fld_WilayahIDAsal == WilayahIDAsal && x.fld_LadangIDAsal == LadangIDAsal && x.fld_SAPCode == SAPCode);

                if (pktPinjamExisting == null)
                {
                //end
                    tbl_PktPinjam.fld_JenisPkt = CustMod_TransferPkt.JnisPkt;
                    tbl_PktPinjam.fld_JnsLot = JenisLot;
                    tbl_PktPinjam.fld_KodPkt = KodPkt + DivisionIDAsal + "P";
                    tbl_PktPinjam.fld_NamaPkt = NamaPkt + " - (Pinjam)";
                    tbl_PktPinjam.fld_DivisionID = 0;
                    //fld_LadangID & fld_WilayahID adalah merujuk kepada ladang dan wilayah asal
                    tbl_PktPinjam.fld_LadangID = CustMod_TransferPkt.ladangList2;
                    tbl_PktPinjam.fld_WilayahID = CustMod_TransferPkt.wilayahList2;
                    tbl_PktPinjam.fld_SyarikatID = SyarikatID;
                    tbl_PktPinjam.fld_NegaraID = NegaraID;
                    //tbl_PktPinjam.fld_DivisionIDAsal = DivisionIDAsal;
                    //fld_LadangIDAsal & fld_WilayahIDAsal adalah merujuk kepada ladang dan wilayah pinjam
                    tbl_PktPinjam.fld_LadangIDAsal = LadangIDAsal;
                    tbl_PktPinjam.fld_WilayahIDAsal = WilayahIDAsal;
                    tbl_PktPinjam.fld_SyarikatIDAsal = SyarikatID;
                    tbl_PktPinjam.fld_NegaraIDAsal = NegaraIDAsl;
                    tbl_PktPinjam.fld_SAPCode = SAPCode;
                    tbl_PktPinjam.fld_OriginPktID = CustMod_TransferPkt.PilihanPkt;
                    tbl_PktPinjam.fld_EndDT = CustMod_TransferPkt.DateEnd;
                    tbl_PktPinjam.fld_CreatedBy = getuserid;
                    tbl_PktPinjam.fld_CreatedDT = CurrentDT;
                    EstateConnTo.tbl_PktPinjam.Add(tbl_PktPinjam);
                    EstateConnTo.SaveChanges();

                    EstateConnFrom.Dispose();
                    EstateConnTo.Dispose();

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
                        div = "SearchingData",
                        rootUrl = domain,
                        action = "_EstateLevelTemporaryManagementList",
                        controller = "EstateDataManagement"
                    });
                //fatin added - 10/10/2023
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        msg = "IO pinjam telah wujud . Sila lanjutkan tarikh akhir",
                        status = "warning",
                        checkingdata = "0"
                    });
                }
                //end
            }
            else
            {
                return Json(new
                {
                    success = false,
                    msg = "Ladang Asal dan Ladang Pinjam Tidak boleh sama",
                    status = "warning",
                    checkingdata = "0"
                });
            }
        }

        //fatin added - 12/09/2023
        public ActionResult _EstateLevelTemporaryManagementEdit(int id, int wil)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, wil, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);



            List<SelectListItem> PilihanPkt = new List<SelectListItem>();
            PilihanPkt.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.PilihanPkt = PilihanPkt;

         
            var GetLevelDetail = estateConnection.tbl_PktPinjam.Find(id);

            return PartialView(GetLevelDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateLevelTemporaryManagementEdit(tbl_PktPinjam tbl_PktPinjam)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            Connection.GetConnection(out host, out catalog, out user, out pass, tbl_PktPinjam.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            ChangeTimeZone ChangeTimeZone = new ChangeTimeZone();
            var DateNow = ChangeTimeZone.gettimezone();
            var message = "";

            try
            {
                
                var pktPinjamData = estateConnection.tbl_PktPinjam.Find(tbl_PktPinjam.fld_ID);

                pktPinjamData.fld_EndDT = tbl_PktPinjam.fld_EndDT;
                                

                estateConnection.Entry(pktPinjamData).State = EntityState.Modified;
                estateConnection.SaveChanges();


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
                    div = "SearchingData",
                    rootUrl = domain,
                    action = "_EstateLevelTemporaryManagementList",
                    controller = "EstateDataManagement"
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
        //end

            public ActionResult _EstateLevelTemporaryManagementDelete(int id, int wil)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, wil, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            var GetLevelDetail = estateConnection.tbl_PktPinjam.Find(id);

            return PartialView(GetLevelDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateLevelTemporaryManagementDelete(tbl_PktPinjam tbl_PktPinjam)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, tbl_PktPinjam.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var GetLevelDetail = estateConnection.tbl_PktPinjam.Find(tbl_PktPinjam.fld_ID);
                estateConnection.tbl_PktPinjam.Remove(GetLevelDetail);
                estateConnection.SaveChanges();

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
                    msg = "Maklumat Berjaya Dihapus",
                    status = "success",
                    checkingdata = "0",
                    method = "1",
                    div = "SearchingData",
                    rootUrl = domain,
                    action = "_EstateLevelTemporaryManagementList",
                    controller = "EstateDataManagement"
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

        public JsonResult GetLadang(int WilayahID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);

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

        public JsonResult GetPkt(byte JnsPkt, int WilayahList, int LadangList) // fatin modified remove JnisAktvt - 13/09/2023
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> PilihPeringkat = new List<SelectListItem>();
            switch (JnsPkt)
            {
                case 1:
                    var SelectPkt = estateConnection.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt.Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama + " - (" + s.fld_IOcode + ")" }), "Value", "Text").ToList();
                    break;
                case 2:
                    var SelectPkt2 = estateConnection.tbl_SubPkt.Join(estateConnection.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktUtama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Pkt, j.fld_NamaPkt, j.fld_Deleted, k.fld_IOcode, j.fld_ID }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt2.Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Pkt + " - " + s.fld_NamaPkt + " - (" + s.fld_IOcode + ")" }), "Value", "Text").ToList();
                    break;
                case 3:
                    var SelectPkt3 = estateConnection.tbl_Blok.Join(estateConnection.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktutama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Blok, j.fld_NamaBlok, j.fld_Deleted, k.fld_IOcode, j.fld_ID }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt3.Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Blok + " - " + s.fld_NamaBlok + " - (" + s.fld_IOcode + ")" }), "Value", "Text").ToList();
                    break;
            }
            if (PilihPeringkat.Count > 0)
            {
                PilihPeringkat.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" }));
            }
            else
            {
                PilihPeringkat.Insert(0, (new SelectListItem { Text = "Tiada Data", Value = "" }));
            }

            estateConnection.Dispose();

            return Json(new { PilihPeringkat });
        }


        public ActionResult ReverseSAP(string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);

            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

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

            int[] wlyhid = new int[] { };
            List<SelectListItem> SyarikatIDList = new List<SelectListItem>();
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" }));
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" }));
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                var ladangInfo = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                var listladang = new SelectList(db.tbl_Ladang.Where(x => x.fld_ID == LadangID).OrderBy(x => x.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text", LadangID).ToList();

                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_ID == ladangInfo.fld_ID).FirstOrDefault();
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_ID == ladangInfo.fld_ID).OrderBy(x => x.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", WilayahID).ToList();
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == ladangInfo.fld_CostCentre).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == ladangInfo.fld_CostCentre).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", ladangInfo.fld_CostCentre).ToList();
            }

            ViewBag.SyarikatList = SyarikatIDList;
            ViewBag.WilayahList = WilayahIDList;
            ViewBag.LadangList = LadangIDList;
            ViewBag.SyarikatList = SyarikatIDList;

            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            ViewBag.YearList = yearlist;
            ViewBag.ClosingTransaction = "class = active";
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.WilayahID = WilayahID;
            ViewBag.LadangID = LadangID;

            return View();
        }

        public ActionResult _ReverseSAP(int? MonthList, int? YearList,int? SyarikatList, int? WilayahList, int? LadangList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_ModelsCorporate ModelsCorporate = new MVC_SYSTEM_ModelsCorporate();

            Connection Connection = new Connection();

            var message = "";

            var postingData = new List<vw_SAPPostData>();
            var postingData1 = new List<CustMod_ReverseSAP>();
            List<CustMod_ReverseSAP> resultreport = new List<CustMod_ReverseSAP>();
            if (!String.IsNullOrEmpty(MonthList.ToString()) && !String.IsNullOrEmpty(YearList.ToString()) && WilayahList != 0 && LadangList != 0)
            {              
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID, NegaraID);
                MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);


                postingData = estateConnection.vw_SAPPostData
                    .Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList &&
                                x.fld_LadangID == LadangList).ToList();
                
                List<CustMod_ReverseSAP> CustMod_ReverseSAP = new List<CustMod_ReverseSAP>();
                foreach (var postdetails in postingData)
                {
                    
                CustMod_ReverseSAP.Add(new CustMod_ReverseSAP() { fld_NoDocSAP = postdetails.fld_NoDocSAP, fld_HeaderText = postdetails.fld_HeaderText, fld_DocType = postdetails.fld_DocType, fld_DocDate = postdetails.fld_DocDate, fld_PostingDate = postdetails.fld_PostingDate, fld_Year = postdetails.fld_Year, fld_Month = postdetails.fld_Month, fld_SAPPostRefID = postdetails.fld_SAPPostRefID, fld_LadangID = postdetails.fld_LadangID, fld_Amount = postdetails.fld_Amount, fld_CompCode = postdetails.fld_CompCode, fld_flag = postdetails.fld_flag , fld_DocNoSAP = postdetails.fld_DocNoSAP, fld_RefNo = postdetails.fld_RefNo , fld_StatusProceed = postdetails.fld_StatusProceed});
                   
                }

                resultreport = CustMod_ReverseSAP.ToList();

                PropertyCopy.Copy(CustMod_ReverseSAP, postingData);
                
                if (!postingData.Any())
                {
                    message = "Tiada Data";
                }
            }

            else
            {
                message = "Sila pilih syarikat, wilayah, ladang, tahun dan bulan";
            }

            ViewBag.Message = message;

            ViewBag.Existing = db.tbl_SokPermhnWang.Where(x => x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_LadangID == LadangID).Any();
            ViewBag.GetTerimaHQ = ModelsCorporate.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TerimaHQ_Status == 1).Any();
            ViewBag.GetTolakHQ = ModelsCorporate.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakHQ_Status == 1).Any();
            ViewBag.GetTolakWilGM = ModelsCorporate.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakWilGM_Status == 1).Any();
            ViewBag.GetTolakWil = ModelsCorporate.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakWil_Status == 1).Any();
            ViewBag.GetJumPermohonan = ModelsCorporate.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).Select(s => s.fld_JumlahPermohonan).FirstOrDefault();
            ViewBag.Year = YearList;
            ViewBag.Month = MonthList;
            ViewBag.LadangID = LadangID;

            return View("_ReverseSAP", resultreport);
        }


        [HttpPost]
        public ActionResult Working(List<CustMod_ReverseSAP> HadirData)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string bodyview = "";
            DateTime DTCreated = timezone.gettimezone();
            List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
            bool TransferPkt = false;

            int? WilayahID1 = HadirData.FirstOrDefault().fld_WilayahID;
            int? LadangID1 = HadirData.FirstOrDefault().fld_LadangID;
            int? month1 = HadirData.FirstOrDefault().fld_Month;
            int? year1 = HadirData.FirstOrDefault().fld_Year;
            var syarikatnegara = db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID1 && x.fld_ID == LadangID1).FirstOrDefault();
            int? NegaraID1 = syarikatnegara.fld_NegaraID;
            int? SyarikatID1 = syarikatnegara.fld_SyarikatID;
            Guid saprefid = HadirData.FirstOrDefault().fld_SAPPostRefID;

            string hostEstate, catalogEstate, userEstate, passEstate = "";
            Connection.GetConnection(out hostEstate, out catalogEstate, out userEstate, out passEstate,
                WilayahID1, SyarikatID1, NegaraID1);
            MVC_SYSTEM_ModelsEstate estateConnection =
                MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(hostEstate, catalogEstate, userEstate, passEstate);

            string vendorcodevalue = "";
            string DocNoSAPvalue = "";
            var resultrowKR = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_flag != 0 && x.fld_SAPPostRefID == saprefid && x.fld_DocNoSAP != null).Count();
            var result = HadirData.Where(x => x.status_chkbx == "true").ToList();
            if (result != null)
            {
                //delete all
                if (resultrowKR == result.Count())
                {
                    foreach (var listflags in HadirData.Where(x => x.status_chkbx == "true"))
                    {
                        var listflag = listflags.fld_flag;
                        var listpostrefdetails = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == saprefid && x.fld_flag == listflag).ToList();
                        if (listpostrefdetails != null)
                        {
                            var listpostref = estateConnection.tbl_SAPPostRef.Where(x => x.fld_ID == saprefid && x.fld_DocType == "KR" && x.fld_Month== listflags.fld_Month && x.fld_Year == listflags.fld_Year && x.fld_LadangID== listflags.fld_LadangID).FirstOrDefault();
                            foreach (var detailsflag in listpostrefdetails)
                            {
                                Guid refdetails = detailsflag.fld_ID;
                                var checking = estateConnection.tbl_SAPHistory.Where(x => x.fld_ID_ref == saprefid && x.fld_flag == listflag && x.fld_Year == listflags.fld_Year && x.fld_Month == listflags.fld_Month).FirstOrDefault();
                                if (checking == null)
                                {
                                    var vendorcodeInfo = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == saprefid && x.fld_flag == listflag && x.fld_VendorCode != null).FirstOrDefault();
                                    if (vendorcodeInfo == null)
                                    { vendorcodevalue = ""; }
                                    else
                                    { vendorcodevalue = vendorcodeInfo.fld_VendorCode; }

                                    var DocNoSAPInfo = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == saprefid && x.fld_flag == listflag && x.fld_DocNoSAP != null).FirstOrDefault();
                                    if (DocNoSAPInfo == null)
                                    { DocNoSAPvalue = ""; }
                                    else
                                    { DocNoSAPvalue = DocNoSAPInfo.fld_DocNoSAP; }

                                    var amountInfo = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == saprefid && x.fld_flag == listflag && x.fld_Amount >= 0).Select(x => x.fld_Amount).ToList();
                                    decimal? amountvalue = amountInfo.Sum();

                                    tbl_SAPHistory SAPHistory = new tbl_SAPHistory();
                                    SAPHistory.fld_ID_ref = saprefid;
                                    SAPHistory.fld_NegaraID = listpostref.fld_NegaraID;
                                    SAPHistory.fld_SyarikatID = listpostref.fld_SyarikatID;
                                    SAPHistory.fld_WilayahID = listpostref.fld_WilayahID;
                                    SAPHistory.fld_LadangID = listpostref.fld_LadangID;
                                    SAPHistory.fld_CompCode = listpostref.fld_CompCode;
                                    SAPHistory.fld_Year = listpostref.fld_Year;
                                    SAPHistory.fld_Month = listpostref.fld_Month;
                                    SAPHistory.fld_HeaderText = listpostref.fld_HeaderText;
                                    SAPHistory.fld_DocDate = listpostref.fld_DocDate;
                                    SAPHistory.fld_PostingDate = listpostref.fld_PostingDate;
                                    SAPHistory.fld_DocType = listpostref.fld_DocType;
                                    SAPHistory.fld_RefNo = listpostref.fld_RefNo;
                                    SAPHistory.fld_NoDocSAP = listpostref.fld_NoDocSAP;
                                    SAPHistory.fld_Purpose = listpostref.fld_Purpose;
                                    SAPHistory.fld_CreatedBy = getuserid;
                                    SAPHistory.fld_CreatedDT = timezone.gettimezone();
                                    SAPHistory.fld_Amount = amountvalue;
                                    SAPHistory.fld_VendorCode = vendorcodevalue;
                                    SAPHistory.fld_DocNoSAP = DocNoSAPvalue;
                                    SAPHistory.fld_flag = listflag;

                                    estateConnection.tbl_SAPHistory.Add(SAPHistory);
                                    estateConnection.SaveChanges();
                                }

                                var postrefdetails = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_ID == refdetails).FirstOrDefault();
                                estateConnection.tbl_SAPPostDataDetails.Remove(postrefdetails);
                                estateConnection.SaveChanges();
                            }
                        }
                    }

                    var postref = estateConnection.tbl_SAPPostRef.Where(x => x.fld_SyarikatID == SyarikatID1 && x.fld_WilayahID == WilayahID1 && x.fld_LadangID == LadangID1 && x.fld_Month == month1 && x.fld_Year == year1 && x.fld_ID == saprefid && x.fld_DocType == "KR").FirstOrDefault();
                    if (postref == null)
                    { return View("NotFound"); }
                    else
                    {
                        estateConnection.tbl_SAPPostRef.Remove(postref);
                        estateConnection.SaveChanges();
                    }
                }
                else
                {
                    //delete selected data
                    foreach (var listflags in HadirData.Where(x => x.status_chkbx == "true"))
                    {
                        var listflag = listflags.fld_flag;
                        var listpostrefdetails = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == saprefid && x.fld_flag == listflag).ToList();
                        if (listpostrefdetails != null)
                        {
                            var listpostref = estateConnection.tbl_SAPPostRef.Where(x => x.fld_ID == saprefid && x.fld_DocType == "KR" && x.fld_Month == listflags.fld_Month && x.fld_Year == listflags.fld_Year && x.fld_LadangID == listflags.fld_LadangID).FirstOrDefault();
                            foreach (var detailsflag in listpostrefdetails)
                            {
                                Guid refdetails = detailsflag.fld_ID;

                                var checking = estateConnection.tbl_SAPHistory.Where(x => x.fld_ID_ref == saprefid && x.fld_flag == listflag && x.fld_Year == listflags.fld_Year && x.fld_Month == listflags.fld_Month).FirstOrDefault();
                                if (checking == null)
                                {
                                    var vendorcodeInfo = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == saprefid && x.fld_flag == listflag && x.fld_VendorCode != null).FirstOrDefault();
                                    if (vendorcodeInfo == null)
                                    { vendorcodevalue = ""; }
                                    else
                                    { vendorcodevalue = vendorcodeInfo.fld_VendorCode; }

                                    var DocNoSAPInfo = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == saprefid && x.fld_flag == listflag && x.fld_DocNoSAP != null).FirstOrDefault();
                                    if (DocNoSAPInfo == null)
                                    { DocNoSAPvalue = ""; }
                                    else
                                    { DocNoSAPvalue = DocNoSAPInfo.fld_DocNoSAP; }

                                    var amountInfo = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == saprefid && x.fld_flag == listflag && x.fld_Amount >= 0).Select(x => x.fld_Amount).ToList();
                                    decimal? amountvalue = amountInfo.Sum();

                                    tbl_SAPHistory SAPHistory = new tbl_SAPHistory();
                                    SAPHistory.fld_ID_ref = saprefid;
                                    SAPHistory.fld_NegaraID = listpostref.fld_NegaraID;
                                    SAPHistory.fld_SyarikatID = listpostref.fld_SyarikatID;
                                    SAPHistory.fld_WilayahID = listpostref.fld_WilayahID;
                                    SAPHistory.fld_LadangID = listpostref.fld_LadangID;
                                    SAPHistory.fld_CompCode = listpostref.fld_CompCode;
                                    SAPHistory.fld_Year = listpostref.fld_Year;
                                    SAPHistory.fld_Month = listpostref.fld_Month;
                                    SAPHistory.fld_HeaderText = listpostref.fld_HeaderText;
                                    SAPHistory.fld_DocDate = listpostref.fld_DocDate;
                                    SAPHistory.fld_PostingDate = listpostref.fld_PostingDate;
                                    SAPHistory.fld_DocType = listpostref.fld_DocType;
                                    SAPHistory.fld_RefNo = listpostref.fld_RefNo;
                                    SAPHistory.fld_NoDocSAP = listpostref.fld_NoDocSAP;
                                    SAPHistory.fld_Purpose = listpostref.fld_Purpose;
                                    SAPHistory.fld_CreatedBy = getuserid;
                                    SAPHistory.fld_CreatedDT = timezone.gettimezone();
                                    SAPHistory.fld_Amount = amountvalue;
                                    SAPHistory.fld_VendorCode = vendorcodevalue;
                                    SAPHistory.fld_DocNoSAP = DocNoSAPvalue;
                                    SAPHistory.fld_flag = listflag;

                                    estateConnection.tbl_SAPHistory.Add(SAPHistory);
                                    estateConnection.SaveChanges();
                                }


                                var postrefdetails = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_ID == refdetails).FirstOrDefault();
                                postrefdetails.fld_DocNoSAP = null;
                                estateConnection.Entry(postrefdetails).State = EntityState.Modified;
                                estateConnection.SaveChanges();
                            }                       
                        }
                    }
                    var postrefData = estateConnection.tbl_SAPPostRef.Where(x => x.fld_ID == saprefid).FirstOrDefault();
                    postrefData.fld_StatusProceed = false;
                    postrefData.fld_ModifiedBy = getuserid;
                    postrefData.fld_ModifiedDT = timezone.gettimezone();
                    estateConnection.Entry(postrefData).State = EntityState.Modified;
                    estateConnection.SaveChanges();
                }
            }

            return Json(new { msg, statusmsg, tablelisting = bodyview });

        }
        public string RenderRazorViewToString(string viewname, object dataview, bool CutOfDateStatus)
        {
            ViewData.Model = dataview;
            ViewBag.CutOfDateStatus = CutOfDateStatus;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewname);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public JsonResult UpdateData(Guid DataID, string CekrolRefNo, int NegaraId, int SyarikatId, int WilayahId, int LadangId, int Year, int Month)
        {
            string DescStatus = "";
            int getuserid = GetIdentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            DateTime getdatetime = timezone.gettimezone();

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int? WilayahID1 = WilayahId;
            int? LadangID1 = LadangId;
            var syarikatnegara = db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID1 && x.fld_ID == LadangID1).FirstOrDefault();
            int? NegaraID1 = syarikatnegara.fld_NegaraID;
            int? SyarikatID1 = syarikatnegara.fld_SyarikatID;

            string hostEstate, catalogEstate, userEstate, passEstate = "";
            Connection.GetConnection(out hostEstate, out catalogEstate, out userEstate, out passEstate,
                WilayahID1, SyarikatID1, NegaraID1);
            MVC_SYSTEM_ModelsEstate estateConnection =
                MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(hostEstate, catalogEstate, userEstate, passEstate);
            try
            {
                string vendorcodevalue = "";
                string DocNoSAPvalue = "";
                var postrefA2 = estateConnection.tbl_SAPPostRef.Where(x => x.fld_ID == DataID && x.fld_DocType=="A2").FirstOrDefault();
                var postdetailsA2 = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == postrefA2.fld_ID && x.fld_flag == 0).ToList();
                if (postrefA2 != null)
                {
                    if (postdetailsA2 != null)
                    {
                        foreach (var listflags in postdetailsA2)
                        {

                            var checking = estateConnection.tbl_SAPHistory.Where(x => x.fld_ID_ref == DataID && x.fld_flag == 0 && x.fld_Year == postrefA2.fld_Year && x.fld_Month == postrefA2.fld_Month).FirstOrDefault();
                            if (checking == null)
                            {
                                var vendorcodeInfo = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == DataID && x.fld_flag == 0 && x.fld_VendorCode != null).FirstOrDefault();
                                if (vendorcodeInfo == null)
                                { vendorcodevalue = ""; }
                                else
                                { vendorcodevalue = vendorcodeInfo.fld_VendorCode; }

                                var DocNoSAPInfo = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == DataID && x.fld_flag == 0 && x.fld_DocNoSAP != null).FirstOrDefault();
                                if (DocNoSAPInfo == null)
                                { DocNoSAPvalue = ""; }
                                else
                                { DocNoSAPvalue = DocNoSAPInfo.fld_DocNoSAP; }

                                var amountInfo = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == DataID && x.fld_flag == 0 && x.fld_Amount >= 0).Select(x => x.fld_Amount).ToList();
                                decimal? amountvalue = amountInfo.Sum();

                                tbl_SAPHistory SAPHistory = new tbl_SAPHistory();
                                SAPHistory.fld_ID_ref = DataID;
                                SAPHistory.fld_NegaraID = postrefA2.fld_NegaraID;
                                SAPHistory.fld_SyarikatID = postrefA2.fld_SyarikatID;
                                SAPHistory.fld_WilayahID = postrefA2.fld_WilayahID;
                                SAPHistory.fld_LadangID = postrefA2.fld_LadangID;
                                SAPHistory.fld_CompCode = postrefA2.fld_CompCode;
                                SAPHistory.fld_Year = postrefA2.fld_Year;
                                SAPHistory.fld_Month = postrefA2.fld_Month;
                                SAPHistory.fld_HeaderText = postrefA2.fld_HeaderText;
                                SAPHistory.fld_DocDate = postrefA2.fld_DocDate;
                                SAPHistory.fld_PostingDate = postrefA2.fld_PostingDate;
                                SAPHistory.fld_DocType = postrefA2.fld_DocType;
                                SAPHistory.fld_RefNo = postrefA2.fld_RefNo;
                                SAPHistory.fld_NoDocSAP = postrefA2.fld_NoDocSAP;
                                SAPHistory.fld_Purpose = postrefA2.fld_Purpose;
                                SAPHistory.fld_CreatedBy = getuserid;
                                SAPHistory.fld_CreatedDT = timezone.gettimezone();
                                SAPHistory.fld_Amount = amountvalue;
                                SAPHistory.fld_VendorCode = vendorcodevalue;
                                SAPHistory.fld_DocNoSAP = DocNoSAPvalue;
                                SAPHistory.fld_flag = 0;

                                estateConnection.tbl_SAPHistory.Add(SAPHistory);
                                estateConnection.SaveChanges();
                            }

                            var postrefdetails = estateConnection.tbl_SAPPostDataDetails.Where(x => x.fld_ID == listflags.fld_ID).FirstOrDefault();
                            estateConnection.tbl_SAPPostDataDetails.Remove(postrefdetails);
                            estateConnection.SaveChanges();

                        }
                    }
                    estateConnection.tbl_SAPPostRef.Remove(postrefA2);
                    estateConnection.SaveChanges();
                }

                DescStatus = "Data already reversed.";
            }
            catch (Exception ex)
            {

            }
            return Json(new { success = true, msg = DescStatus, status = "success" });

        }
        //end
        //public ActionResult ReverseSAP(string filter)
        //{
        //    ViewBag.Report = "class = active";

        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID = 0, SyarikatID = 0, WilayahID = 0, LadangID = 0;
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    int[] wlyhid = new int[] { };
        //    int month = timezone.gettimezone().Month;
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    int prevYear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    int currentYear = timezone.gettimezone().Year;
        //    var yearlist = new List<SelectListItem>();
        //    for (var i = prevYear; i <= currentYear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem
        //            { Text = i.ToString(), Value = i.ToString(), Selected = true }
        //            );
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem
        //            { Text = i.ToString(), Value = i.ToString() }
        //            );
        //        }
        //    }
        //    ViewBag.YearList = yearlist;

        //    List<SelectListItem> SyarikatIDList = new List<SelectListItem>(); //Added by Shazana 1/8/2023
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();


        //    //Modified by Shazana 15/8/2023
        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
        //        int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
        //        var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).OrderBy(x => x.fld_LdgName).Select(x => x.fld_WlyhID).ToList();
        //        var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName).ToList();
        //        WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
        //        int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
        //        SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(x => x.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();

        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        var ladangInfo = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_NegaraID == NegaraID).FirstOrDefault();
        //        var listladang = new SelectList(db.tbl_Ladang.Where(x => x.fld_ID == LadangID).OrderBy(x => x.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangID).ToList();

        //        var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_ID == ladangInfo.fld_ID).FirstOrDefault();
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_ID == ladangInfo.fld_ID).OrderBy(x => x.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", WilayahID).ToList();
        //        var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == ladangInfo.fld_CostCentre).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
        //        int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
        //        SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == ladangInfo.fld_CostCentre).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", ladangInfo.fld_CostCentre).ToList();
        //    }


        //    ViewBag.SyarikatList = SyarikatIDList; //Added by Shazana 1/8/2023
        //    ViewBag.WilayahList = WilayahIDList;
        //    ViewBag.LadangList = LadangIDList;
        //    //SyarikatList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
        //    ViewBag.SyarikatList = SyarikatIDList;

        //    //List<SelectListItem> wilayahList = new List<SelectListItem>();
        //    //wilayahList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
        //    //ViewBag.WilayahList = wilayahList;

        //    //List<SelectListItem> ladangList = new List<SelectListItem>();
        //    //ladangList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
        //    //ViewBag.LadangList = ladangList;

        //    return View();
        //}

        //public ActionResult _ReverseSAP(int? MonthList, int? YearList, string SyarikatList, int? WilayahList, int? LadangList, int page = 1)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    var message = "";
        //    MonthList = 1; YearList = 2024;
        //    SyarikatList = "1"; WilayahList = 1; LadangList =1;
        //        Connection Connection = new Connection();
        //        string host, catalog, user, pass;
        //        Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID, NegaraID);
        //        MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

        //        var result = estateConnection.vw_SAPPostData.Where(x => x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahList).ToList();

        //    ViewBag.Message = message;
        //    return View(result);
        //}
        //public JsonResult GetWilayah(string SyarikatID)
        //{
        //    List<SelectListItem> wilayahlist = new List<SelectListItem>();
        //    List<SelectListItem> ladanglist = new List<SelectListItem>();

        //    int? NegaraID = 0;
        //    int? SyarikatID2 = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);

        //    GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID, out LadangID, getuserid, User.Identity.Name); //Modified by Shazana 1/8/2023
        //    var syarikatCodeId = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fldOptConfValue == SyarikatID.ToString() && x.fld_NegaraID == NegaraID).Select(x => x.fld_SyarikatID).FirstOrDefault();
        //    int SyarikatCode = Convert.ToInt16(syarikatCodeId);

        //    if (getwilyah.GetAvailableWilayah(SyarikatCode))
        //    {
        //        if (WilayahID == 0)
        //        {
        //            var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatID && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
        //            var listwilayah1 = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
        //            wilayahlist = new SelectList(listwilayah1.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
        //            wilayahlist.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //            ladanglist.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

        //        }
        //        else
        //        {
        //            wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID2 && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
        //            ladanglist.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        }
        //    }

        //    return Json(wilayahlist);
        //}
        public JsonResult GetLadang2(int WilayahID, string SyarikatID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID2, out LadangID, getuserid, User.Identity.Name);


            if (getwilyah.GetAvailableWilayah(SyarikatID2))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_CostCentre == "0" && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_NamaLadang }), "Value", "Text").ToList(); //modified by kamalia 1/2/2022
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_CostCentre == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_NamaLadang }), "Value", "Text").ToList(); //modified by kamalia 1/2/2022
                }
            }

            return Json(ladanglist);
        }
        public JsonResult GetWilayah2(string SyarikatID)
        {
            List<SelectListItem> wilayahlist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            var listwilayahid = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatID && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
            listwilayahid = listwilayahid.Distinct().ToList();
            wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => listwilayahid.Contains(x.fld_ID) && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();

            return Json(wilayahlist);
        }
    }
}