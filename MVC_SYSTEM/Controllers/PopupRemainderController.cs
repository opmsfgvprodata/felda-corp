using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsSP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsEstate;
using System.Reflection.Emit;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User,Viewer")]
    public class PopupRemainderController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_ModelsEstate estateDB = new MVC_SYSTEM_ModelsEstate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private GetIdentity getidentity = new GetIdentity();
        private GetWilayah getwilyah = new GetWilayah();
        private GetWilayah GetWilayah = new GetWilayah();
        private GetLadang GetLadang = new GetLadang();
        private CheckSharedFolder CheckSharedFolder = new CheckSharedFolder();
        private errorlog geterror = new errorlog();

        //new update model
        private MVC_SYSTEM_SP2_Models sp_db = new MVC_SYSTEM_SP2_Models();
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        private GetNSWL GetNSWL = new GetNSWL();

        // GET: PermitPassportPopup
        public ActionResult Index()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

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
            // end here 030823

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                // yana add 060923
                SyarikatList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
                // end here 060923
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                // yana add 060923
                SyarikatList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                // yana add 060923
                SyarikatList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            }

            // yana add 060923
            var compCode = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).Select(s => s.fld_CostCentre).FirstOrDefault();
            // yana add compcode
            var getPermitLadangID = sp_db.sp_PermitPassportDetail(NegaraID, SyarikatID, wlyhid.Take(1).FirstOrDefault(), LadangID, "1", compCode).Where(x => x.fld_BilBlnTmtPrmt <= 3 && x.fld_KdRyt != "MA").Select(s => s.fld_LadangID).Distinct().ToList();
            var getPassportLadangID = sp_db.sp_PermitPassportDetail(NegaraID, SyarikatID, wlyhid.Take(1).FirstOrDefault(), LadangID, "1", compCode).Where(x => x.fld_BilBlnTmtPsprt <= 3 && x.fld_KdRyt != "MA").Select(s => s.fld_LadangID).Distinct().ToList();
            var getTaskRemainder = dbC.tblTaskRemainders.Where(x => x.fldWilayahID == wlyhid.Take(1).FirstOrDefault() && x.fldStatus == 0).Select(s => new { s.fldLadangID, s.fldPurpose }).ToList();
            var getNewUsrIDAppLadangID = getTaskRemainder.Where(x => x.fldPurpose == "02").Select(s => s.fldLadangID).Distinct().ToList();
            var getSlryIncAppLadangID = getTaskRemainder.Where(x => x.fldPurpose == "03").Select(s => s.fldLadangID).Distinct().ToList();

            ViewBag.getpermitladangID = getPermitLadangID;
            ViewBag.getpassportladangID = getPassportLadangID;
            ViewBag.getNewUsrIDAppLadangID = getNewUsrIDAppLadangID;
            ViewBag.getSlryIncAppLadangID = getSlryIncAppLadangID;
            //var getnewworkerappwilayahID = db.tblTaskRemainders.Where(x => x.fldPurpose == "01" && wlyhid.Contains((int)x.fldWilayahID) && x.fldStatus == 0).Select(s => s.fldWilayahID).Distinct().ToArray();
            //var getnewuseridappwilayahID = db.tblTaskRemainders.Where(x => x.fldPurpose == "02" && wlyhid.Contains((int)x.fldWilayahID) && x.fldStatus == 0).Select(s => s.fldWilayahID).Distinct().ToArray();
            //Array.Sort(getpermitwilayahID);
            //Array.Sort(getpassportwilayahID);
            //Array.Sort(getnewworkerappwilayahID);
            //Array.Sort(getnewuseridappwilayahID);

            //ViewBag.getpermitwilayahID = wlyhid.Take(1).FirstOrDefault();
            //ViewBag.getpassportwilayahID = wlyhid.Take(1).FirstOrDefault();
            //ViewBag.getnewworkerappwilayahID = getnewworkerappwilayahID;
            //ViewBag.getnewuseridappwilayahID = getnewuseridappwilayahID;
            //ViewBag.ladangvalue = LadangID;
            ViewBag.SyarikatList = SyarikatList;
            ViewBag.WilayahIDList = WilayahIDList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // yana add 070923 - string SyarikatList
        public ActionResult Index(int WilayahIDList, string SyarikatList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

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

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            }
            //var costcentercode = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldOptConfDesc == costcentre).FirstOrDefault();
            // yana update 070923 - add SyarikatList
            var getPermitLadangID = sp_db.sp_PermitPassportDetail(NegaraID, SyarikatID, WilayahIDList, LadangID, "1", SyarikatList).Where(x => x.fld_BilBlnTmtPrmt <= 3 && x.fld_KdRyt != "MA" && x.fld_CostCentre == SyarikatList).Select(s => s.fld_LadangID).Distinct().ToList();
            var getPassportLadangID = sp_db.sp_PermitPassportDetail(NegaraID, SyarikatID, WilayahIDList, LadangID, "1", SyarikatList).Where(x => x.fld_BilBlnTmtPsprt <= 3 && x.fld_KdRyt != "MA" && x.fld_CostCentre == SyarikatList).Select(s => s.fld_LadangID).Distinct().ToList();
            var getTaskRemainder = dbC.tblTaskRemainders.Where(x => x.fldWilayahID == WilayahIDList && x.fldStatus == 0).Select(s => new { s.fldLadangID, s.fldPurpose }).ToList();
            var getNewUsrIDAppLadangID = getTaskRemainder.Where(x => x.fldPurpose == "02").Select(s => s.fldLadangID).Distinct().ToList();
            var getSlryIncAppLadangID = getTaskRemainder.Where(x => x.fldPurpose == "03").Select(s => s.fldLadangID).Distinct().ToList();

            ViewBag.getpermitladangID = getPermitLadangID;
            ViewBag.getpassportladangID = getPassportLadangID;
            ViewBag.getNewUsrIDAppLadangID = getNewUsrIDAppLadangID;
            ViewBag.getSlryIncAppLadangID = getSlryIncAppLadangID;
            //ViewBag.getNewWrkerAppLadangID = getNewWrkerAppLadangID;
            //var getpermitwilayahID = db.vw_PermitPassportDetail.Where(x => x.fld_BilBlnTmtPrmnt <= 3 && x.fld_WilayahID == WilayahIDList && x.fld_Kdaktf == "0" && x.fld_Kdrkyt != "MA").OrderBy(o => o.fld_WilayahID).Select(s => s.fld_WlyhID).Distinct().ToArray();
            //var getpassportwilayahID = db.vw_PermitPassportDetail.Where(x => x.fld_BilBlnTmtPsprt <= 3 && x.fld_WilayahID == WilayahIDList && x.fld_Kdaktf == "0" && x.fld_Kdrkyt != "MA").OrderBy(o => o.fld_WilayahID).Select(s => s.fld_WlyhID).Distinct().ToArray();
            //var getnewworkerappwilayahID = db.tblTaskRemainders.Where(x => x.fldPurpose == "01" && wlyhid.Contains((int)x.fldWilayahID) && x.fldStatus == 0).Select(s => s.fldWilayahID).Distinct().ToArray();
            //var getnewuseridappwilayahID = db.tblTaskRemainders.Where(x => x.fldPurpose == "02" && wlyhid.Contains((int)x.fldWilayahID) && x.fldStatus == 0).Select(s => s.fldWilayahID).Distinct().ToArray();
            //Array.Sort(getpermitwilayahID);
            //Array.Sort(getpassportwilayahID);
            //Array.Sort(getnewworkerappwilayahID);
            //Array.Sort(getnewuseridappwilayahID);
            //ViewBag.getpermitwilayahID = getpermitwilayahID.Take(1);
            //ViewBag.getpassportwilayahID = getpassportwilayahID.Take(1);
            //ViewBag.getnewworkerappwilayahID = getnewworkerappwilayahID;
            //ViewBag.getnewuseridappwilayahID = getnewuseridappwilayahID;
            //ViewBag.ladangvalue = LadangID;
            ViewBag.WilayahIDList = WilayahIDList2;
            return View();
        }
        // yana update 070923 - add string SyarikatList
        public ActionResult LaporanTamatPermit(int LadangID, string SyarikatList)
        {
            var NSWL = GetNSWL.GetLadangDetail(LadangID);
            // yana update 070923 - add SyarikatList
            var LaporanTamatPermit = sp_db.sp_PermitPassportDetail(NSWL.fld_NegaraID, NSWL.fld_SyarikatID, NSWL.fld_WilayahID, NSWL.fld_LadangID, "1", SyarikatList).Where(x => x.fld_BilBlnTmtPrmt <= 3 && x.fld_KdRyt != "MA").OrderBy(o => o.fld_BilBlnTmtPrmt).ToList();
            ViewBag.WilayahName = NSWL.fld_NamaWilayah;
            ViewBag.LadangName = NSWL.fld_NamaLadang;
            ViewBag.TotalCount = LaporanTamatPermit.Count();
            return View(LaporanTamatPermit);
        }
        // yana update 070923 - add string SyarikatList
        public ActionResult LaporanTamatPassport(int LadangID, string SyarikatList)
        {
            var NSWL = GetNSWL.GetLadangDetail(LadangID);
            // yana update 070923 - add SyarikatList
            var LaporanTamatPassport = sp_db.sp_PermitPassportDetail(NSWL.fld_NegaraID, NSWL.fld_SyarikatID, NSWL.fld_WilayahID, NSWL.fld_LadangID, "1", SyarikatList).Where(x => x.fld_BilBlnTmtPsprt <= 3 && x.fld_KdRyt != "MA").OrderBy(o => o.fld_BilBlnTmtPsprt).ToList();
            ViewBag.WilayahName = NSWL.fld_NamaWilayah;
            ViewBag.LadangName = NSWL.fld_NamaLadang;
            ViewBag.TotalCount = LaporanTamatPassport.Count();
            return View(LaporanTamatPassport);
        }

        public ActionResult TaskNewWorkerApp(int wilid, int ladcd)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string salesFTPPath = "";
            string codeladang = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            string sourceFile = CheckSharedFolder.GetSourceTargetPath("filesourcepathworkerapp", NegaraID, SyarikatID); //db.tbl_OptionConfig.Where(x => x.fldOptConfFlag1 == "filesourcepathuseridapp" && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
            DirectoryInfo salesFTPDirectory = null;
            FileInfo[] files = null;
            string[] getFiles;
            codeladang = GetLadang.GetCodeLadangFromID2(ladcd);
            try
            {
                if (Directory.Exists(sourceFile + codeladang))
                {
                    salesFTPPath = sourceFile + codeladang;
                }
                else
                {
                    salesFTPPath = sourceFile;
                }
                salesFTPDirectory = new DirectoryInfo(salesFTPPath);
                files = salesFTPDirectory.GetFiles();
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            files = files.Where(f => f.Extension == ".asc").OrderBy(f => f.Name).ToArray();
            getFiles = files.Where(f => f.Extension == ".asc").OrderBy(o => o.CreationTime).Select(s => s.Name).ToArray();

            var TaskNewWorkerApp = db.tblTaskRemainders.Where(x => x.fldWilayahID == wilid && x.fldLadangID == ladcd && x.fldStatus == 0 && x.fldPurpose == "01" && getFiles.Contains(x.fldFileName)).OrderBy(o => o.fldDateTimeStamp).ToList();
            ViewBag.WilayahName = GetWilayah.GetWilayahName(wilid); //LaporanTamatPassport.Select(s => s.fld_WlyhName).Take(1).FirstOrDefault();//db.tbl_Wilayah.Where(x => x.fld_ID == wilid && x.fld_Deleted == false).Select(s => s.fld_WlyhName).FirstOrDefault();
            ViewBag.LadangName = TaskNewWorkerApp.Select(s => s.fldCodeLadang).Distinct().FirstOrDefault() + " - " + GetLadang.GetLadangName(ladcd, wilid);//LaporanTamatPassport.Select(s => s.fld_LdgName).Take(1).FirstOrDefault();//db.tbl_Ladang.Where(x => x.fld_ID == ladcd && x.fld_Deleted == false).Select(s => s.fld_LdgName).FirstOrDefault();
            ViewBag.TotalCount = TaskNewWorkerApp.Count();
            return View(TaskNewWorkerApp);
        }

        //  yana add - string SyarikatList
        public ActionResult TaskNewUserIDApp(int LadangID, string SyarikatList)
        {
            int? getuserid = getidentity.ID(User.Identity.Name);

            var NSWL = GetNSWL.GetLadangDetail(LadangID);

            //var compCode = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).Select(s => s.fld_CostCentre).FirstOrDefault();

            //var ladangid = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_CostCentre == compCode).FirstOrDefault();

            var checknotascfiles = dbC.tblASCApprovalFileDetails.Where(x => x.fldASCFileStatus == 1 && x.fldPurpose == 1 && x.fldNegaraID == NSWL.fld_NegaraID && x.fldSyarikatID == NSWL.fld_SyarikatID && x.fldWilayahID == NSWL.fld_WilayahID && x.fldLadangID == NSWL.fld_LadangID && x.fldGenStatus == 0).Select(s => s.fldFileName).ToList();

            // yana add - NSWL.fld_CostCentre == SyarikatList
            var TaskNewUserIDApp = dbC.tblTaskRemainders.Where(x => x.fldNegaraID == NSWL.fld_NegaraID && x.fldSyarikatID == NSWL.fld_SyarikatID && x.fldWilayahID == NSWL.fld_WilayahID && x.fldLadangID == NSWL.fld_LadangID && x.fldStatus == 0 && x.fldPurpose == "02" && checknotascfiles.Contains(x.fldFileName) && NSWL.fld_CostCentre == SyarikatList).OrderBy(o => o.fldDateTimeStamp).ToList();
            ViewBag.WilayahName = NSWL.fld_NamaWilayah;
            ViewBag.LadangName = NSWL.fld_NamaLadang;
            //ViewBag.compCode = NSWL.fld_CostCentre;
            ViewBag.TotalCount = TaskNewUserIDApp.Count();

            return View(TaskNewUserIDApp);
        }

        //  yana add - string SyarikatList
        public ActionResult TaskSalaryIncrmntApp(int LadangID, string SyarikatList)
        {
            int? getuserid = getidentity.ID(User.Identity.Name);

            var NSWL = GetNSWL.GetLadangDetail(LadangID);

            var checknotascfiles = dbC.tblASCApprovalFileDetails.Join(dbC.tblTaskRemainders, j => new { j.fldFileName, j.fldNegaraID, j.fldSyarikatID, j.fldWilayahID, j.fldLadangID }, k => new { k.fldFileName, k.fldNegaraID, k.fldSyarikatID, k.fldWilayahID, k.fldLadangID }, (j, k) => new { j.fldASCFileStatus, j.fldPurpose, j.fldGenStatus, j.fldNegaraID, j.fldSyarikatID, j.fldWilayahID, j.fldLadangID, j.fldFileName }).Where(x => x.fldASCFileStatus == 1 && x.fldPurpose == 3 && x.fldNegaraID == NSWL.fld_NegaraID && x.fldSyarikatID == NSWL.fld_SyarikatID && x.fldWilayahID == NSWL.fld_WilayahID && x.fldLadangID == NSWL.fld_LadangID && x.fldGenStatus == 0).Select(s => s.fldFileName).ToList();

            // yana add - NSWL.fld_CostCentre == SyarikatList
            var TaskSalaryIncrmntApp = dbC.tblTaskRemainders.Where(x => x.fldNegaraID == NSWL.fld_NegaraID && x.fldSyarikatID == NSWL.fld_SyarikatID && x.fldWilayahID == NSWL.fld_WilayahID && x.fldLadangID == NSWL.fld_LadangID && x.fldStatus == 0 && x.fldPurpose == "03" && checknotascfiles.Contains(x.fldFileName) && NSWL.fld_CostCentre == SyarikatList).OrderBy(o => o.fldDateTimeStamp).ToList();
            ViewBag.WilayahName = NSWL.fld_NamaWilayah;
            ViewBag.LadangName = NSWL.fld_NamaLadang;
            ViewBag.TotalCount = TaskSalaryIncrmntApp.Count();

            return View(TaskSalaryIncrmntApp);
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