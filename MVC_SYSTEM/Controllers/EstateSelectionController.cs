using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    //role id authorization ( adding super power user)  - modified by farahin - 28/06/2022
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User")]
    public class EstateSelectionController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetIdentity getidentity = new GetIdentity();
        private GetWilayah getwilyah = new GetWilayah();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetConfig GetConfig = new GetConfig();
        private errorlog geterror = new errorlog();
        private GetTriager GetTriager = new GetTriager();
        private EncryptDecrypt Encrypt = new EncryptDecrypt();
        // GET: EstateSelection
        public ActionResult Index()
        {
            ViewBag.EstateSelection = "class = active";
            return View();
        }

        public ActionResult EstateSelection()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int wilayahid = 0;
            //bool bln1tri, bln2tri, bln3tri, bln4tri, bln5tri, bln6tri, bln7tri, bln8tri, bln9tri, bln10tri, bln11tri, bln12tri = false;
            
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
                wilayahid = db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                //modified by faeza 09.07.2023 - OrderBy(o => o.fld_CostCentre).ThenBy(t => t.fld_LdgName)
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wilayahid && x.fld_Deleted == false).OrderBy(o => o.fld_CostCentre).ThenBy(t => t.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //modified by faeza 09.07.2023 - OrderBy(o => o.fld_CostCentre).ThenBy(t => t.fld_LdgName)
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_CostCentre).ThenBy(t => t.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //modified by faeza 09.07.2023 - OrderBy(o => o.fld_CostCentre).ThenBy(t => t.fld_LdgName)
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_CostCentre).ThenBy(t => t.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }
            
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.LadangID = 0;
            ViewBag.ladangvalue = LadangID;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstateSelection(int WilayahIDList, int LadangIDList)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int getuserid = getidentity.ID(User.Identity.Name);
            var user = db2.tblUsers.Where(u => u.fldUserID == getuserid).SingleOrDefault();
            var getestateselection = db2.tbl_EstateSelection.Where(x => x.fld_UserID == getuserid).FirstOrDefault();
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            AuthModels.tbl_EstateSelection tbl_EstateSelection = new AuthModels.tbl_EstateSelection();
            if (getestateselection != null)
            {
                getestateselection.fld_NegaraID = NegaraID;
                getestateselection.fld_SyarikatID = SyarikatID;
                getestateselection.fld_WilayahID = WilayahIDList;
                getestateselection.fld_LadangID = LadangIDList;
                getestateselection.fld_HQUrl = Url.Action("", "", null, this.Request.Url.Scheme);   
                db2.Entry(getestateselection).State = EntityState.Modified;
                db2.SaveChanges();
            }
            else
            {
                tbl_EstateSelection.fld_NegaraID = NegaraID;
                tbl_EstateSelection.fld_SyarikatID = SyarikatID;
                tbl_EstateSelection.fld_WilayahID = WilayahIDList;
                tbl_EstateSelection.fld_LadangID = LadangIDList;
                tbl_EstateSelection.fld_UserID = getuserid;
                tbl_EstateSelection.fld_HQUrl = Url.Action("", "", null, this.Request.Url.Scheme);
                db2.tbl_EstateSelection.Add(tbl_EstateSelection);
                db2.SaveChanges();
            }

            var routeurl = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == user.fldSyarikatID && x.fld_ID == WilayahIDList).Select(s => s.fld_UrlRoute).FirstOrDefault();
            string passwordencrypt = Encrypt.Encrypt(user.fldUserPassword);
            string usernameencrypt = Encrypt.Encrypt(user.fldUserName);
            int day = timezone.gettimezone().Day;
            int month = timezone.gettimezone().Month;
            int year = timezone.gettimezone().Year;
            string code = day.ToString() + month.ToString() + year.ToString();
            code = Encrypt.Encrypt(code);
            routeurl = routeurl + "IntegrationLogin?TokenID=" + usernameencrypt + "&PassID=" + passwordencrypt + "&Code=" + code;

            GetConfig.AddUserAuditTrail(user.fldUserID, "Login to estate from HQ estate selection");

            return Redirect(routeurl);
        }

        // yana update 030823 - add string SyarikatList
        public JsonResult GetLadang(int WilayahID, string SyarikatList)
        // end here 030823
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
                    //modified by faeza 09.07.2023 - OrderBy(o => o.fld_CostCentre).ThenBy(t => t.fld_NamaLadang)
                    // yana update 030823 x.fld_CostCentre == SyarikatList
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_CostCentre).ThenBy(t => t.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    //modified by faeza 09.07.2023 - OrderBy(o => o.fld_CostCentre).ThenBy(t => t.fld_NamaLadang)
                    // yana update 030823 x.fld_CostCentre == SyarikatList
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_CostCentre).ThenBy(t => t.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

    }
}