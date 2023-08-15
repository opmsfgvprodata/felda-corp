using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin")]
    public class SuperAdminSelectionController : Controller
    {
        private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        GetIdentity getidentity = new GetIdentity();
        // GET: SuperAdminSelection
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SuperAdminSelection()
        {
            int negaraid = 0;

            ViewBag.KumpulanSyarikatList = new SelectList(db.tbl_KumpulanSyarikat.Where(x => x.fld_Deleted == false), "fld_KmplnSyrktID", "fld_NamaKmplnSyrkt");
            if (getidentity.SuperPowerAdmin(User.Identity.Name))
            {
                ViewBag.NegaraList = new SelectList(db.tbl_Negara.Where(x => x.fld_Deleted == false), "fld_NegaraID", "fld_NamaNegara");
                negaraid = db.tbl_Negara.Where(x => x.fld_Deleted == false).Select(s => s.fld_NegaraID).Take(1).FirstOrDefault();
            }
            else
            {
                var kmplnsyrkatID = getidentity.getKmplnSyrktID(User.Identity.Name);
                ViewBag.NegaraList = new SelectList(db.tbl_Negara.Where(x => x.fld_KmplnSyrktID == kmplnsyrkatID && x.fld_Deleted == false), "fld_NegaraID", "fld_NamaNegara");
                negaraid = db.tbl_Negara.Where(x => x.fld_KmplnSyrktID == kmplnsyrkatID && x.fld_Deleted == false).Select(s => s.fld_NegaraID).Take(1).FirstOrDefault();
            }
            ViewBag.SyarikatList = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == negaraid && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuperAdminSelection(int NegaraList, int SyarikatList)
        {
            int getuserid = getidentity.ID(User.Identity.Name);
            var getsuperadminselection = db.tbl_SuperAdminSelection.Where(x => x.fld_SuperAdminID == getuserid).FirstOrDefault();
            tbl_SuperAdminSelection tbl_SuperAdminSelection = new tbl_SuperAdminSelection();
            if (getsuperadminselection != null)
            {
                getsuperadminselection.fld_NegaraID = NegaraList;
                getsuperadminselection.fld_SyarikatID = SyarikatList;
                db.Entry(getsuperadminselection).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                tbl_SuperAdminSelection.fld_NegaraID = NegaraList;
                tbl_SuperAdminSelection.fld_SyarikatID = SyarikatList;
                tbl_SuperAdminSelection.fld_SuperAdminID = getuserid;
                db.tbl_SuperAdminSelection.Add(tbl_SuperAdminSelection);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Main");
        }

        public JsonResult GetNegara(int KumpulanSyarikatID)
        {
            List<SelectListItem> negaralist = new List<SelectListItem>();

            negaralist = new SelectList(db.tbl_Negara.Where(x => x.fld_KmplnSyrktID == KumpulanSyarikatID && x.fld_Deleted == false), "fld_NegaraID", "fld_NamaNegara").ToList();

            return Json(negaralist);
        }

        public JsonResult GetSyarikat(int NegaraID)
        {
            List<SelectListItem> syarikatlist = new List<SelectListItem>();

            syarikatlist = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat").ToList();

            return Json(syarikatlist);
        }

        public JsonResult GetSyarikat1(int KumpulanSyarikatID)
        {
            List<SelectListItem> syarikatlist = new List<SelectListItem>();

            var getnegaraid = db.tbl_Negara.Where(x => x.fld_KmplnSyrktID == KumpulanSyarikatID && x.fld_Deleted == false).Select(s => s.fld_NegaraID).Take(1).FirstOrDefault();
            syarikatlist = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == getnegaraid && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat").ToList();

            return Json(syarikatlist);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}