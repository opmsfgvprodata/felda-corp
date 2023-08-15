using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsCorporate;
using tblTKABatch = MVC_SYSTEM.Models.tblTKABatch;
using tblTKADetail = MVC_SYSTEM.Models.tblTKADetail;

namespace MVC_SYSTEM.Controllers
{
    public class ResourceController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private GetIdentity GetIdentity = new GetIdentity();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        // GET: Resource
        public ActionResult Index()
        {
            ViewBag.DataNegaraSumber = "class = active";

            var getSatuSyarikat = db.vw_NeragaSumberDetail.Where(x => x.fldUserName == User.Identity.Name).OrderBy(o => o.fld_NamaKmplnSyrkt).Take(1).Select(s => s.fldKmplnSyrktID).FirstOrDefault();
            ViewBag.KumpulanSyarikatList = new SelectList(db.vw_NeragaSumberDetail.Where(x => x.fldUserName == User.Identity.Name).OrderBy(o => o.fld_NamaKmplnSyrkt), "fldKmplnSyrktID", "fld_NamaKmplnSyrkt", getSatuSyarikat);
            ViewBag.SyarikatList = new SelectList(db.vw_NSWL.Where(x => x.fld_KmplnSyrktID == getSatuSyarikat && x.fld_Deleted_S == false).Select(s => new { s.fld_SyarikatID, s.fld_NamaSyarikat }).Distinct(), "fld_SyarikatID", "fld_NamaSyarikat");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int KumpulanSyarikatList, int SyarikatList)
        {
            ViewBag.DataNegaraSumber = "class = active";
            Session["KumpulanSyarikatList"] = KumpulanSyarikatList;
            Session["SyarikatList"] = SyarikatList;
            return RedirectToAction("DataForm", "Resource");
        }

        public ActionResult DataForm()
        {
            ViewBag.KumpulanSyarikatList = Session["KumpulanSyarikatList"];
            ViewBag.SyarikatList = Session["SyarikatList"];
            ViewBag.DataNegaraSumber = "class = active";
            if (ViewBag.KumpulanSyarikatList != null && ViewBag.KumpulanSyarikatList != null)
            {
                string Kmplnsyarikatidstring = Session["KumpulanSyarikatList"].ToString();
                int Kmplnssyarikatid = int.Parse(Kmplnsyarikatidstring);
                string syarikatidstring = Session["SyarikatList"].ToString();
                int syarikatid = int.Parse(syarikatidstring);
                ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.KerakyatanList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false), "fldOptConfDesc", "fldOptConfDesc").ToList();
                ViewBag.BusinesList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "businesnature" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
                ViewBag.RelationshipList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "relationship" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
                ViewBag.GenderList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "gender" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
                ViewBag.BatchList = new SelectList(db.tblTKABatches.Where(x => x.fldKmplnSyrktID == Kmplnssyarikatid && x.fldSyrktID == syarikatid).OrderBy(o => o.fldNoBatch), "fldID", "fldNoBatch").Take(20).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("DataForm", "Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DataForm(int BatchID, string PassportNoCopy, string WorkerName, DateTime PDE, string GenderList, DateTime BOD, string KerakyatanList, string BusinesList, string DependantName, string RelationshipList, string DependantPhone, string DependantAdd)
        {
            //PDE = PDE.AddMonths(1);
            //BOD = BOD.AddMonths(1);
            string tablelisting = "";
            string msg = "";
            string statusmsg = "";
            bool status = false;
            int itemno = 1;

            ModelsCorporate.tblTKADetail tblTKADetail = new ModelsCorporate.tblTKADetail();

            tblTKADetail.fldPassNo = PassportNoCopy.ToUpper();
            tblTKADetail.fldWorkerName = WorkerName.ToUpper();
            tblTKADetail.fldPassExpDT = PDE;
            tblTKADetail.fldGender = GenderList.ToUpper();
            tblTKADetail.fldBOD = BOD;
            tblTKADetail.fldNationality = KerakyatanList.ToUpper();
            tblTKADetail.fldNatureWork = BusinesList.ToUpper();
            tblTKADetail.fldDpdntName = DependantName.ToUpper();
            tblTKADetail.fldDpdntRelationship = RelationshipList.ToUpper();
            tblTKADetail.fldDpdntTelNo = DependantPhone.ToUpper();
            tblTKADetail.fldDpdntAdd = DependantAdd.ToUpper();
            tblTKADetail.fldTKABatchID = BatchID;
            tblTKADetail.fldStatusArrive = 0;

            db.tblTKADetails.Add(tblTKADetail);
            db.SaveChanges();

            msg = "Detail has been added into the system.";
            statusmsg = "success";
            status = true;

            var getTKADetails = db.tblTKADetails.Where(x => x.fldTKABatchID == BatchID).ToList();

            //tablelisting += "<table class=\"table table-hover table - bordered\" style=\"font - size: 11px; \" border=\"0\"";
            tablelisting += "<thead>";
            tablelisting += "<tr>";
            tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\"> ITEM NO</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">NAME OF WORKERS</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">PASSPORT NO.</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">PASSPORT EXPIRY DATE</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">GENDER</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">DATE OF BIRTH</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">NATIONALITY</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">WORKER'S NATURE OF WORK</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">ACTION</th>";
            tablelisting += "</tr>";
            tablelisting += "</thead>";
            tablelisting += "<tbody>";
            foreach (var getTKADetail in getTKADetails)
            {
                tablelisting += "<tr>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + itemno + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getTKADetail.fldWorkerName + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getTKADetail.fldPassNo + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + Convert.ToString(string.Format("{0:dd/MM/yyyy}", getTKADetail.fldPassExpDT)) + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getTKADetail.fldGender + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + Convert.ToString(string.Format("{0:dd/MM/yyyy}", getTKADetail.fldBOD)) + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getTKADetail.fldNationality + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getTKADetail.fldNatureWork + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\"><a data-modal = \"\" href = \"" + Url.Action("Edit", "Resource") + "/"+ getTKADetail.fldID + "\" title=\"Edit\"> <span class=\"glyphicon glyphicon-edit\"> </span> </a></td>";
                tablelisting += "</tr>";
                itemno++;
            }
            tablelisting += "</tbody>";
            msg = "Detail has been added into the system.";
            statusmsg = "success";
            status = true;
            return Json(new { tablelisting = tablelisting, msg = msg, statusmsg = statusmsg, status = status });
        }

        public ActionResult Edit(int? id)
        {
            int getuserid = GetIdentity.ID(User.Identity.Name);

            ViewBag.KumpulanSyarikatList = Session["KumpulanSyarikatList"];
            ViewBag.SyarikatList = Session["SyarikatList"];

            string Kmplnsyarikatidstring = Session["KumpulanSyarikatList"].ToString();
            int Kmplnssyarikatid = int.Parse(Kmplnsyarikatidstring);
            string syarikatidstring = Session["SyarikatList"].ToString();
            int syarikatid = int.Parse(syarikatidstring);
            
            var getAuthDetailTKA = db.vw_TKADetail.Where(x => x.fldID == id && x.fldCreatedBy == getuserid && x.fldKmplnSyrktID == Kmplnssyarikatid && x.fldSyrktID == syarikatid).FirstOrDefault();

            ModelsCorporate.tblTKADetail tblTKADetail = new ModelsCorporate.tblTKADetail();

            if (getAuthDetailTKA != null)
            {
                tblTKADetail = db.tblTKADetails.Where(x => x.fldID == id).FirstOrDefault();
            }
            ViewBag.KerakyatanList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false), "fldOptConfDesc", "fldOptConfDesc", tblTKADetail.fldNationality.ToString()).ToList();
            ViewBag.BusinesList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "businesnature" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", tblTKADetail.fldNatureWork.ToString()).ToList();
            ViewBag.RelationshipList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "relationship" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", tblTKADetail.fldDpdntRelationship.ToString()).ToList();
            ViewBag.GenderList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "gender" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", tblTKADetail.fldGender.ToString()).ToList();
            return PartialView("Edit", tblTKADetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblTKADetail tblTKADetail, string GenderList, string KerakyatanList, string BusinesList, string RelationshipList, string DependantAdd)
        {
            try
            {
                var getdata = db.tblTKADetails.Find(tblTKADetail.fldID);

                getdata.fldPassNo = getdata.fldPassNo.ToUpper();
                getdata.fldWorkerName = tblTKADetail.fldWorkerName.ToUpper();
                getdata.fldPassExpDT = tblTKADetail.fldPassExpDT;
                getdata.fldGender = GenderList.ToUpper();
                getdata.fldBOD = tblTKADetail.fldBOD;
                getdata.fldNationality = KerakyatanList.ToUpper();
                getdata.fldNatureWork = BusinesList.ToUpper();
                getdata.fldDpdntName = tblTKADetail.fldDpdntName.ToUpper();
                getdata.fldDpdntRelationship = RelationshipList.ToUpper();
                getdata.fldDpdntTelNo = tblTKADetail.fldDpdntTelNo.ToUpper();
                getdata.fldDpdntAdd = DependantAdd.ToUpper();
                getdata.fldTKABatchID = tblTKADetail.fldTKABatchID;
                getdata.fldStatusArrive = 0;

                db.Entry(getdata).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, msg = "Data successfully edited.", status = "success", linktbllist = Url.Action("UpdateListData", "Resource", null, this.Request.Url.Scheme), batchid = tblTKADetail.fldTKABatchID });
            }
            catch (Exception ex)
            {
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger" });
            }
            
        }

        public JsonResult UpdateListData(int id)
        {
            string tablelisting = "";
            int itemno = 1;
            string msg = "";
            string statusmsg = "";
            bool status = false;
            int batchid = 0;
            string batchno = "";
            var getTKADetails = db.tblTKADetails.Where(x => x.fldTKABatchID == id).ToList();
            var getBatchdetail = db.tblTKABatches.Where(x => x.fldID == id).FirstOrDefault();
            if (getTKADetails.Count != 0 && getBatchdetail != null)
            {
                //tablelisting += "<table class=\"table table-hover table - bordered\" style=\"font - size: 11px; \" border=\"0\"";
                tablelisting += "<thead>";
                tablelisting += "<tr>";
                tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\"> ITEM NO</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">NAME OF WORKERS</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">PASSPORT NO.</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">PASSPORT EXPIRY DATE</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">GENDER</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">DATE OF BIRTH</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">NATIONALITY</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">WORKER'S NATURE OF WORK</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">ACTION</th>";
                tablelisting += "</tr>";
                tablelisting += "</thead>";
                tablelisting += "<tbody>";
                foreach (var getTKADetail in getTKADetails)
                {
                    tablelisting += "<tr>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + itemno + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getTKADetail.fldWorkerName + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getTKADetail.fldPassNo + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + Convert.ToString(string.Format("{0:dd/MM/yyyy}", getTKADetail.fldPassExpDT)) + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getTKADetail.fldGender + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + Convert.ToString(string.Format("{0:dd/MM/yyyy}", getTKADetail.fldBOD)) + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getTKADetail.fldNationality + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getTKADetail.fldNatureWork + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\"><a data-modal = \"\" href = \"" + Url.Action("Edit", "Resource") + "/" + getTKADetail.fldID + "\" title=\"Edit\"> <span class=\"glyphicon glyphicon-edit\"> </span> </a></td>";
                    tablelisting += "</tr>";
                    itemno++;
                }
                tablelisting += "</tbody>";

                batchid = getBatchdetail.fldID;
                batchno = getBatchdetail.fldNoBatch;

                msg = "Data found.";
                statusmsg = "success";
                status = true;
            }
            else if (getTKADetails.Count == 0 && getBatchdetail != null)
            {
                batchid = getBatchdetail.fldID;
                batchno = getBatchdetail.fldNoBatch;
                msg = "No data in this batch.";
                statusmsg = "warning";
                status = true;
            }
            else
            {
                batchid = 0;
                batchno = "No Data";
                msg = "No data in this batch.";
                statusmsg = "warning";
                status = true;
            }
           
            return Json(new { tablelisting = tablelisting, BatchID = batchid, BatchName = batchno, msg = msg, statusmsg = statusmsg, status = status });
        }

        public JsonResult SaveBatch(string NoBatch, int KmplnSyrktID, int SyrktID)
        {
            string msg = "";
            string statusmsg = "";
            bool status = false;
            int getuserid = GetIdentity.ID(User.Identity.Name);
            int batchID = 0;
            DateTime DT = timezone.gettimezone();
            var getNoBatchAvailable = db.tblTKABatches.Where(x => x.fldNoBatch == NoBatch).ToList();
            if (getNoBatchAvailable.Count == 0)
            {
                ModelsCorporate.tblTKABatch tblTKABatch = new ModelsCorporate.tblTKABatch();

                tblTKABatch.fldNoBatch = NoBatch;
                tblTKABatch.fldCreatedBy = getuserid;
                tblTKABatch.fldDTCreated = DT;
                tblTKABatch.fldModifiedBy = getuserid;
                tblTKABatch.fldDTModified = DT;
                tblTKABatch.fldKmplnSyrktID = KmplnSyrktID;
                tblTKABatch.fldSyrktID = SyrktID;

                db.tblTKABatches.Add(tblTKABatch);
                db.SaveChanges();

                batchID = tblTKABatch.fldID;
                msg = "Batch done to created. You may proceed to check passport.";
                statusmsg = "success";
                status = true;
            }
            else
            {
                msg = "Batch already created. Please use other batch no.";
                statusmsg = "warning";
                status = false;
            }
            return Json(new { batchID = batchID, msg = msg, statusmsg = statusmsg, status = status });
        }

        public JsonResult GetSyarikat(int KumpulanSyarikatID)
        {
            List<SelectListItem> syarikatlist = new List<SelectListItem>();

            syarikatlist = new SelectList(db.vw_NSWL.Where(x => x.fld_KmplnSyrktID == KumpulanSyarikatID && x.fld_Deleted_S == false).Select(s => new { s.fld_SyarikatID, s.fld_NamaSyarikat }).Distinct(), "fld_SyarikatID", "fld_NamaSyarikat").ToList();

            return Json(syarikatlist);
        }

        public JsonResult CheckPassport(string Passportno)
        {
            string msg = "";
            string statusmsg = "";
            bool status = false;

            var getTKADetailExist = db.tblTKADetails.Where(x => x.fldPassNo == Passportno).ToList();
            if (getTKADetailExist.Count == 0)
            {
                msg = "Passport No. not exist in the system. You may proceed to fill the worker detail";
                statusmsg = "success";
                status = true;
            }
            else
            {
                msg = "Passport No. already exist in the system. You cannot fill worker detail";
                statusmsg = "warning";
                status = false;
            }

            return Json(new { Passportno = Passportno, msg = msg, statusmsg = statusmsg, status = status });
        }

        public JsonResult GetBatch(string NoBatch, int KmplnSyrktID, int SyrktID)
        {
            List<SelectListItem> BatchList = new List<SelectListItem>();
            bool nodata = false;
            BatchList = new SelectList(db.tblTKABatches.Where(x => x.fldNoBatch.Contains(NoBatch) && x.fldKmplnSyrktID == KmplnSyrktID && x.fldSyrktID == SyrktID).OrderBy("fldNoBatch").Take(20), "fldID", "fldNoBatch").ToList();
            if (BatchList.Count() == 0)
            {
                BatchList.Insert(0, (new SelectListItem { Text = "No Data", Value = "0" }));
                nodata = true;
            }
            else
            {
                nodata = false;
            }
            return Json(new { BatchList , nodata });
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