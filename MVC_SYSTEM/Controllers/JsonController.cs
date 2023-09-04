using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsEstate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
    public class JsonController : Controller
    {
        private GetIdentity GetIdentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetWilayah getwilyah = new GetWilayah();
        private Connection Connection = new Connection();
        private GetConfig GetConfig = new GetConfig();
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        // GET: Json
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

        //sarah added
        public JsonResult GetLadang2(int WilayahID, String SyarikatList)
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
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        public JsonResult GetPkj(int? WilayahList, int? LadangList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> pkjList = new List<SelectListItem>();

            var estateWorkerData = estateConnection.tbl_Pkjmast
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList).OrderBy(o => o.fld_Nama);

            foreach (var estateWorker in estateWorkerData)
            {
                pkjList.Add(new SelectListItem
                {
                    Text = estateWorker.fld_Nopkj.ToString() + " - " +
                           GetConfig.GetPkjNameFromNoPkj(estateWorker.fld_Nopkj, NegaraID, SyarikatID, WilayahList, LadangList),
                    Value = estateWorker.fld_Nopkj.ToString()
                });
            }

            return Json(pkjList);
        }

        public JsonResult GetDaerah(string postCode)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var daerah = "";
            var districtData = db.tbl_Poskod
                .SingleOrDefault(x => x.fld_Postcode == postCode);

            if (districtData == null)
            {
                daerah = "";
            }

            else
            {
                daerah = districtData.fld_DistrictArea;
            }

            return Json(daerah);
        }

        public JsonResult checkCategoryType(string jenisPelan)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> UnitList = new List<SelectListItem>();

            if (jenisPelan == "A")
            {
                UnitList = new SelectList(
                    db.tblOptionConfigsWebs
                        .Where(x => x.fldOptConfFlag1 == "unit" && x.fldOptConfFlag2 == "A" &&
                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                        .OrderBy(o => o.fldOptConfDesc)
                        .Select(
                            s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                    "Value", "Text").ToList();
            }

            else if (jenisPelan == "B")
            {
                UnitList = new SelectList(
                    db.tblOptionConfigsWebs
                        .Where(x => x.fldOptConfFlag1 == "unit" && x.fldOptConfFlag2 == "B" &&
                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                    x.fldDeleted == false)
                        .OrderBy(o => o.fldOptConfDesc)
                        .Select(
                            s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                    "Value", "Text").ToList();
                UnitList.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" }));
            }

            else
            {
                UnitList.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" }));
            }

            return Json(new { UnitList = UnitList });
        }

        public JsonResult checkIncentiveRecord(string incentiveCategoryType, string nopkj, int? wilayahID, int? ladangID, int? year, int? month)
        {
            List<SelectListItem> incentiveAvailableForWorkerList = new List<SelectListItem>();

            if (String.IsNullOrEmpty(incentiveCategoryType))
            {
                return Json(new
                {
                    incentiveAvailableForWorkerList,
                    noData = true
                });
            }

            else
            {
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = GetIdentity.ID(User.Identity.Name);
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, wilayahID, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

                var workerData = estateConnection.tbl_Pkjmast
                    .Where(x => x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == wilayahID && x.fld_LadangID == ladangID &&
                                x.fld_Kdaktf == "1");

                var workerDesignation = workerData.Select(s => s.fld_Ktgpkj).SingleOrDefault();

                var incentiveEligibilityData = db.tbl_KelayakanInsentifPkjLdg
                    .Where(x => x.fld_KodPkj == workerDesignation &&
                                x.fld_KodInsentif.Contains(incentiveCategoryType)
                                && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahID && x.fld_LadangID == ladangID && x.fld_Deleted == false)
                    .Select(s => s.fld_KodInsentif)
                    .ToList();

                var incentiveData = db.tbl_JenisInsentif
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

                List<String> incentiveDataList = new List<String>();

                //if local worker
                if (GetConfig.GetWebConfigFlag2FromValue(workerData.Select(s => s.fld_Jenispekerja).SingleOrDefault(),
                        "jnsPkj", NegaraID, SyarikatID) == "1")
                {
                    incentiveDataList = incentiveData
                        .Where(x => x.fld_KelayakanKepada == 0 || x.fld_KelayakanKepada == 2)
                        .Select(s => s.fld_KodInsentif)
                        .ToList();
                }

                //foreign worker
                else
                {
                    incentiveDataList = incentiveData
                        .Where(x => x.fld_KelayakanKepada == 1 || x.fld_KelayakanKepada == 2)
                        .Select(s => s.fld_KodInsentif)
                        .ToList();
                }

                var trueIncentiveEligibility = incentiveDataList.Intersect(incentiveEligibilityData);

                var workerIncentiveDataList = estateConnection.vw_MaklumatInsentif
                    .Where(x => x.fld_Nopkj == nopkj && x.fld_Month == month &&
                                x.fld_Year == year && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahID &&
                                x.fld_LadangID == ladangID && x.fld_Deleted == false)
                    .Select(s => s.fld_KodInsentif)
                    .ToList();

                var trueIncentiveEligibilityMinusExistingIncentive =
                    trueIncentiveEligibility.Except(workerIncentiveDataList);

                if (trueIncentiveEligibilityMinusExistingIncentive.ToList().Count == 0)
                {
                    incentiveAvailableForWorkerList.Insert(0,
                        new SelectListItem { Text = GlobalResCorp.lblNoIncentiveEligibility, Value = "" });
                }

                else
                {
                    incentiveAvailableForWorkerList.Insert(0,
                        new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
                }

                var i = 1;

                foreach (var incentive in trueIncentiveEligibilityMinusExistingIncentive)
                {
                    incentiveAvailableForWorkerList.Insert(i,
                        new SelectListItem
                        {

                            Value = incentive,
                            Text = incentiveData.Where(x => x.fld_KodInsentif == incentive)
                                .Select(s => s.fld_Keterangan)
                                .SingleOrDefault()
                        });
                    i++;
                }

                return Json(new
                {
                    incentiveAvailableForWorkerList,
                    noData = false
                });
            }
        }

        public JsonResult getIncentiveLimit(string incentiveCode)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> incentiveAvailableForWorkerList = new List<SelectListItem>();

            incentiveAvailableForWorkerList.Insert(0,
                new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

            if (String.IsNullOrEmpty(incentiveCode))
            {
                return Json(new
                {
                    noData = true
                });
            }

            else
            {
                var incentiveData = db.tbl_JenisInsentif
                    .SingleOrDefault(x => x.fld_KodInsentif == incentiveCode && x.fld_NegaraID == NegaraID &&
                                          x.fld_SyarikatID == SyarikatID);

                return Json(new
                {
                    incentiveData,
                    noData = false
                });
            }
        }

        public JsonResult getDailyIncentiveAmount(string incentiveCode, string nopkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (String.IsNullOrEmpty(incentiveCode))
            {
                return Json(new
                {
                    noData = true
                });
            }

            else
            {
                var incentiveData = db.tbl_JenisInsentif
                    .SingleOrDefault(x => x.fld_KodInsentif == incentiveCode && x.fld_NegaraID == NegaraID &&
                                          x.fld_SyarikatID == SyarikatID);

                return Json(new
                {
                    msg1 = GlobalResCorp.msgIncentiveMaximumValue,
                    msg2 = GlobalResCorp.lblDay1,
                    ratePerDay = incentiveData.fld_DailyFixedValue,
                    maxIncentive = incentiveData.fld_MaxValue,
                    noData = false
                });
            }
        }
    }
}