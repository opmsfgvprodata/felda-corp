using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsEstate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Itenso.TimePeriod;
using System.Data.Entity;
using System.Transactions;



namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
    public class ApprovalController : Controller
    {
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetIdentity getidentity = new GetIdentity();
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        //private MVC_SYSTEM_SP_Models db3 = new MVC_SYSTEM_SP_Models();
        private MVC_SYSTEM_ModelsCorporate db4 = new MVC_SYSTEM_ModelsCorporate();

        private SendEmailNotification SendEmailNotification = new SendEmailNotification();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetWilayah getwilyah = new GetWilayah();
        private ReadASC ReadASC = new ReadASC();
        private GetConfig GetConfig = new GetConfig();
        private errorlog geterror = new errorlog();
        private CheckSharedFolder CheckSharedFolder = new CheckSharedFolder();
        private DatabaseAction DatabaseAction = new DatabaseAction();

        //new Models
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth dbA = new MVC_SYSTEM_Auth();

        // GET: Approval
        public ActionResult Index()
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.Approval = "class = active";
            ViewBag.ApprovalList = getidentity.MySuperAdmin(User.Identity.Name) ?
            new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "approvallist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc")
            :
            new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "approvallist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string ApprovalList)
        {
            return RedirectToAction(ApprovalList, "Approval");
        }

        public ActionResult ApprovalNewUserIDOPMS(string kdldg, string ascfilename)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            string[] filename = new string[] { };
            string myfilename = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getWilayahID = 0;
            int getLadangID = 0;

            ViewBag.Approval = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var getreceiverdetail = dbC.vw_NSWL.Where(x => x.fld_LdgCode.Trim() == kdldg.Trim()).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID }).FirstOrDefault();

            if (kdldg != null && ascfilename != null)
            {
                WilayahID = getreceiverdetail.fld_WilayahID;
                LadangID = getreceiverdetail.fld_LadangID;
            }

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatList = new List<SelectListItem>(); // fatin added - 27/07/2023

            if (WilayahID == 0 && LadangID == 0)
            {
                getWilayahID = dbC.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                getLadangID = dbC.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                getWilayahID = dbC.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == WilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                getLadangID = dbC.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                getWilayahID = dbC.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == WilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                getLadangID = dbC.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID && x.fld_ID == LadangID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            }

            if (kdldg != null && ascfilename != null)
            {
                getWilayahID = getreceiverdetail.fld_WilayahID;
                getLadangID = getreceiverdetail.fld_LadangID;
            }

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                //Fatin added - 27/07/2023
                WilayahIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                LadangIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                //end
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                //Fatin added - 27/07/2023
                WilayahIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                LadangIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                //end
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                //Fatin added - 27/07/2023
                WilayahIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                LadangIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                //end
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.SyarikatList = SyarikatList; // fatin added - 27/07/2023

            var codeladang = dbC.tbl_Ladang.Where(x => x.fld_ID == getLadangID && x.fld_WlyhID == getWilayahID).Select(s => s.fld_LdgCode).FirstOrDefault();

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

            filename = dbC.tblEmailNotiStatus.Where(x => x.fldLadangID == getLadangID && x.fldWilayahID == getWilayahID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldEmailNotiSource == "Ladang").Select(s => s.fldEmailNotiFlag).ToArray();
            myfilename = String.Join(",", filename); ;
            List<ModelsCorporate.tblASCApprovalFileDetail> batch = new List<ModelsCorporate.tblASCApprovalFileDetail>();
            if (kdldg != null && ascfilename != null)
            {
                batch = dbC.tblASCApprovalFileDetails.Where(x => x.fldLadangID == getLadangID && x.fldWilayahID == getWilayahID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 0 && x.fldFileName == ascfilename && x.fldPurpose == 1).ToList();
            }
            else
            {
                batch = dbC.tblASCApprovalFileDetails.Where(x => x.fldLadangID == getLadangID && x.fldWilayahID == getWilayahID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 0 && x.fldFileName.Contains(myfilename) && x.fldPurpose == 1).ToList();
                batch = dbC.tblASCApprovalFileDetails.Where(x => x.fldLadangID == getLadangID && x.fldWilayahID == getWilayahID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 0 && x.fldPurpose == 1 && x.fldFileName.Contains(myfilename)).ToList();
            }

            return View(batch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovalNewUserIDOPMS(int? SyarikatList, int? WilayahIDList, int LadangIDList, int YearList, string kodladang) // fatin add param SyarikatList - 01/08/2023
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            string[] filename = new string[] { };
            string myfilename = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            ViewBag.Approval = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var codeladang = "";
            int nocodeladang = 0;
            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();
            List<SelectListItem> SyarikatList2 = new List<SelectListItem>(); // fatin added - 27/07/2023

            var CompCode = dbC.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_CostCentre).FirstOrDefault(); // fatin added - 01/08/2023


            if (kodladang != "")
            {
                codeladang = kodladang;
                var ladangdetail = dbC.tbl_Ladang.Where(x => x.fld_LdgCode == codeladang).Select(s => new { s.fld_ID, s.fld_WlyhID }).FirstOrDefault();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); // fatin added - 01/08/2023
                if (ladangdetail != null)
                {
                    WilayahIDList = ladangdetail.fld_WlyhID;
                    LadangIDList = ladangdetail.fld_ID;
                    WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => x.fld_ID == ladangdetail.fld_WlyhID).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                    //fatin modified add costcentre - 01/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_ID == ladangdetail.fld_ID && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    nocodeladang = 1;
                }
            }
            else
            {
                codeladang = dbC.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
            }

            if ((WilayahID == 0 && LadangID == 0 && kodladang == "") || (nocodeladang == 1))
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                //fatin added - 01/08/2023
                WilayahIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList2 = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_LdgName).FirstOrDefault();
                //end
                if (WilayahIDList == 0)
                {
                    //fatin modified add costcentre - 01/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); // fatin added - 01/08/2023
                }
                else
                {
                    //fatin modified add costcentre - 01/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); // fatin added - 01/08/2023
                }
            }
            else if ((WilayahID != 0 && LadangID == 0 && kodladang == "") || (nocodeladang == 1))
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                //fatin added - 01/08/2023
                WilayahIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList2 = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_LdgName).FirstOrDefault();
                //end
                if (WilayahIDList == 0)
                {
                    //fatin modified add costcentre - 01/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); // fatin added - 01/08/2023
                }
                else
                {
                    //fatin modified add costcentre - 01/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); // fatin added - 01/08/2023
                }
            }
            else if ((WilayahID != 0 && LadangID != 0 && kodladang == "") || (nocodeladang == 1))
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName", WilayahID).ToList();

                //fatin added - 01/08/2023
                WilayahIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList2 = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_LdgName).FirstOrDefault();
                //end

                //fatin modified add costcentre - 01/08/2023
                LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist2 = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist2; // list dalam dropdown
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.SyarikatList = SyarikatList2; // fatin added - 27/07/2023
            filename = dbC.tblEmailNotiStatus.Where(x => x.fldLadangID == LadangIDList && x.fldWilayahID == WilayahIDList && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldEmailNotiSource == "Ladang").Select(s => s.fldEmailNotiFlag).ToArray();
            myfilename = String.Join(",", filename); ;
            List<ModelsCorporate.tblASCApprovalFileDetail> batch = new List<ModelsCorporate.tblASCApprovalFileDetail>();
            batch = dbC.tblASCApprovalFileDetails.Where(x => x.fldLadangID == LadangIDList && x.fldWilayahID == WilayahIDList && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 0 && filename.Contains(x.fldFileName) && x.fldPurpose == 1).ToList();
            //batch = dbC.tblASCApprovalFileDetails.Where(x => x.fldLadangID == LadangIDList && x.fldWilayahID == WilayahIDList && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 0 && x.fldPurpose == 1 && filename.Contains(x.fldFileName)).ToList();
            return View(batch);
        }

        public ActionResult GetUserIDApprovalFileOPMS(string filename, int ladangid, int wilayahid)
        {
            string linkfile = "";
            bool success = false;
            string msg = "";
            string status = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            var ladangcode = dbC.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
            int fileid = 0;

            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
            var lang = Request.RequestContext.RouteData.Values["lang"];
            //domain = domain + "/" + lang.ToString();

            if (appname != "/")
            {
                domain = domain + appname + "/" + lang.ToString();
            }

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            try
            {
                var fileexisting = dbC.tblASCApprovalFileDetails.Where(x => x.fldFileName == filename).FirstOrDefault();
                fileid = fileexisting.fldID;
                linkfile = domain + "/Approval/GetUserIDApprovalDetailOPMS/" + fileid;
                success = true;
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                success = false;
                msg = "System Problem.";
                status = "danger";
            }

            return Json(new { success = success, id = linkfile, msg = msg, status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserIDApprovalDetailOPMS(long id)
        {
            ViewBag.Approval = "class = active";
            ViewBag.fileid = id;

            var getnulldata = dbC.tblUserIDApps.Where(x => x.fldNama == null).ToList();
            if (getnulldata.Count() > 0)
            {
                dbC.tblUserIDApps.RemoveRange(getnulldata);
                dbC.SaveChanges();
            }

            var getapprovaldetail = dbC.tblUserIDApps.Where(x => x.fldFileID == id).ToList();
            ViewBag.getdata = dbC.tblUserIDApps.Where(x => x.fldFileID == id && x.fldStatus == "1").Count();
            return View(getapprovaldetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetUserIDApprovalDetailOPMS(long[] approval, int fileid, long[] duplicate, string Tolak)
        {
            ViewBag.Approval = "class = active";
            ViewBag.fileid = fileid;
            int status = 0;
            string msg2 = "";
            bool success = false;
            int? getuserid = getidentity.ID(User.Identity.Name);
            try
            {
                if (Tolak == null)
                {
                    if (approval != null)
                    {
                        var getusernameaccept = dbC.tblUserIDApps.Where(x => approval.Contains(x.fldID)).Select(s=>s.fldUserid.Trim()).ToArray();
                        var approvalupdateaccept = dbC.tblUserIDApps.Where(x => approval.Contains(x.fldID)).ToList();
                        approvalupdateaccept.ForEach(x => x.fldStatus = "1");
                        approvalupdateaccept.ForEach(x => x.fldDateTimeApprove = timezone.gettimezone());
                        approvalupdateaccept.ForEach(x => x.fldActionBy = getuserid);
                        dbC.SaveChanges();

                        var usersupdateaccept = dbA.tblUsers.Where(x => getusernameaccept.Contains(x.fldUserName)).ToList();
                        usersupdateaccept.ForEach(x => x.fldDeleted = false);
                        usersupdateaccept.ForEach(x => x.fld_ModifiedDT = timezone.gettimezone());
                        usersupdateaccept.ForEach(x => x.fld_ModifiedBy = getuserid);
                        dbA.SaveChanges();

                        var getusernamenotaccept = dbC.tblUserIDApps.Where(x => !approval.Contains(x.fldID) && x.fldFileID == fileid).Select(s => s.fldUserid.Trim()).ToArray();
                        var approvalupdatenotaccept = db4.tblUserIDApps.Where(x => !approval.Contains(x.fldID) && x.fldFileID == fileid).ToList();
                        approvalupdatenotaccept.ForEach(x => x.fldStatus = "0");
                        approvalupdateaccept.ForEach(x => x.fldDateTimeApprove = timezone.gettimezone());
                        approvalupdateaccept.ForEach(x => x.fldActionBy = getuserid);
                        dbC.SaveChanges();

                        var usersupdatenotaccept = dbC.tblUsers.Where(x => getusernamenotaccept.Contains(x.fldUserName)).ToList();
                        usersupdatenotaccept.ForEach(x => x.fldDeleted = true);
                        usersupdatenotaccept.ForEach(x => x.fld_ModifiedDT = timezone.gettimezone());
                        usersupdatenotaccept.ForEach(x => x.fld_ModifiedBy = getuserid);
                        dbC.SaveChanges();

                        GenUserIDApprovalFileOPMS(fileid, out status, out msg2, out success, false);

                        if (status == 1)
                        {
                            ViewBag.Status = msg2;
                            ViewBag.ClassStatus = "success";
                        }
                        else if (status == 2)
                        {
                            ViewBag.Status = msg2;
                            ViewBag.ClassStatus = "warning";
                        }
                        else if (status == 3)
                        {
                            ViewBag.Status = msg2;
                            ViewBag.ClassStatus = "warning";
                        }
                        else if (status == 4)
                        {
                            ViewBag.Status = msg2;
                            ViewBag.ClassStatus = "success";
                        }
                    }
                    else
                    {
                        var approvalupdatenotaccept = dbC.tblUserIDApps.Where(x => x.fldFileID == fileid).ToList();
                        approvalupdatenotaccept.ForEach(x => x.fldStatus = "0");
                        approvalupdatenotaccept.ForEach(x => x.fldDateTimeApprove = null);
                        approvalupdatenotaccept.ForEach(x => x.fldActionBy = getuserid);

                        dbC.SaveChanges();
                        ViewBag.Status = "Status berjaya dikemaskini";
                        ViewBag.ClassStatus = "success";
                    }
                    if (duplicate != null)
                    {
                        var duplicatedata = dbC.tblUserIDApps.Where(x => duplicate.Contains(x.fldID)).ToList();
                        duplicatedata.ForEach(x => x.fldStatus = "3");
                        dbC.SaveChanges();
                    }
                }
                else
                {
                    var approvalupdatenotaccept = dbC.tblUserIDApps.Where(x => x.fldFileID == fileid).ToList();
                    approvalupdatenotaccept.ForEach(x => x.fldStatus = "0");
                    approvalupdatenotaccept.ForEach(x => x.fldDateTimeApprove = null);
                    approvalupdatenotaccept.ForEach(x => x.fldActionBy = getuserid);
                    dbC.SaveChanges();

                    GenUserIDApprovalFileOPMS(fileid, out status, out msg2, out success, true);

                    if (status == 1)
                    {
                        ViewBag.Status = msg2;
                        ViewBag.ClassStatus = "success";
                    }
                    else if (status == 2)
                    {
                        ViewBag.Status = msg2;
                        ViewBag.ClassStatus = "warning";
                    }
                    else if (status == 3)
                    {
                        ViewBag.Status = msg2;
                        ViewBag.ClassStatus = "warning";
                    }
                    else if (status == 4)
                    {
                        ViewBag.Status = msg2;
                        ViewBag.ClassStatus = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                ViewBag.Status = "Masalah Sistem";
                ViewBag.ClassStatus = "warning";
            }

            var getapprovaldetail = dbC.tblUserIDApps.Where(x => x.fldFileID == fileid).ToList();
            ViewBag.getdata = dbC.tblUserIDApps.Where(x => x.fldFileID == fileid && x.fldStatus == "1").Count();

            return View(getapprovaldetail);
        }

        public void GenUserIDApprovalFileOPMS(long fileid, out int status, out string msg2, out bool success, bool rejectemail)
        {
            //string msg2 = "";
            //string status = "";
            //bool success = false;
            status = 0;
            msg2 = "";
            success = false;
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = getidentity.MyNameFullName(getuserid);
            string departmentto = "";
            string departmentcc = "";
            DateTime getdatetime = timezone.gettimezone();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            
            try
            {
                if (rejectemail == false)
                {
                    string subject = "Permohonan ID Pengguna Baru Telah Diluluskan";
                    string msg = "";
                    string[] to = new string[] { };
                    List<string> tolist = new List<string>();
                    string[] cc = new string[] { };
                    List<string> cclist = new List<string>();
                    string[] bcc = new string[] { };
                    List<string> bcclist = new List<string>();

                    var getfilename = dbC.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).FirstOrDefault();
                    var getreceiverdetail = dbC.vw_NSWL.Where(x => x.fld_LadangID == getfilename.fldLadangID && x.fld_WilayahID == getfilename.fldWilayahID && x.fld_SyarikatID == getfilename.fldSyarikatID && x.fld_NegaraID == getfilename.fldNegaraID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum,</p>";
                    //msg += "<p><font color=\"red\">INI ADALAH CUBAAN SEMATA - MATA </font></p>";
                    msg += "<p>Permohonan untuk ID pengguna baru telah diluluskan. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Nama File</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td>" + getreceiverdetail.fld_NamaWilayah + "</td><td>" + getreceiverdetail.fld_LdgCode + "</td><td>" + getreceiverdetail.fld_NamaLadang + "</td><td>" + getfilename.fldFileName + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Sila daftar masuk ke dalam OPMS menggunakan ID pengguna yang telah dipohon.</p>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    if (getreceiverdetail.fld_CostCentre == "1000")
                    {
                        departmentto = "REGION_USERID_APPROVAL_FELDA";
                        departmentcc = "HQ_USERID_APPROVAL_FELDA";
                    }

                    if (getreceiverdetail.fld_CostCentre == "8800")
                    {
                        departmentto = "REGION_USERID_APPROVAL_FPM";
                        departmentcc = "HQ_USERID_APPROVAL_FPM";
                    }

                    //modified by faeza 21.06.2023
                    //var emaillist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldDeleted == false).ToList();

                    tolist.Add(getreceiverdetail.fld_LdgEmail);
                    to = tolist.ToArray();

                    //var emailcclist = emaillist.Where(x => (x.fldCategory == "CC" && x.fldDepartment == "HQ_USERID_APPROVAL") || (x.fldCategory == "CC" && x.fldDepartment == "HQ_USERID_APPROVAL")).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    var emailcclist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldCategory == "CC" && (x.fldDepartment == departmentcc || (x.fldDepartment == departmentto && x.fldWilayahID == getreceiverdetail.fld_WilayahID)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    if (emailcclist != null)
                    {
                        foreach (var ccemail in emailcclist)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                    }
                    cc = cclist.ToArray();

                    //var emailbcclist = emaillist.Where(x => x.fldCategory == "BCC" && x.fldDepartment == "DEVELOPER").Select(s => new { s.fldEmail, s.fldName }).ToList();

                    var emailbcclist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldCategory == "BCC" && x.fldDepartment == "DEVELOPER" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    if (emailbcclist != null)
                    {
                        foreach (var bccemail in emailbcclist)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    if (SendEmailNotification.CheckEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "HQ"))
                    {
                        //if (SendEmailNotification.SendEmail(subject, msg, getreceiverdetail.fld_LdgEmail, cc, bcc))
                        if (SendEmailNotification.SendEmail(subject, msg, to, cc, bcc))
                        {
                            SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "Email From HQ To Ladang - New User ID Approval", "HQ", 1);
                            msg2 = "Email berjaya dihantar";
                            status = 1;
                        }
                        else
                        {
                            SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "Email From HQ To Ladang - New User ID Approval", "HQ", 0);
                            msg2 = "Email gagal dihantar";
                            status = 2;
                        }
                        DatabaseAction.UpdateDataTotbltblTaskRemainder(getfilename.fldFileName, getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, "02");
                    }
                    else
                    {
                        msg2 = "Email sudah dihantar sebelum ini";
                        status = 4;
                    }
                    DatabaseAction.UpdateDataTotblASCApprovalFileDetail(fileid);
                    success = true;
                }
                else
                {
                    string subject = "Permohonan ID Pengguna Baru Telah Ditolak";
                    string msg = "";
                    string[] to = new string[] { };
                    List<string> tolist = new List<string>();
                    string[] cc = new string[] { };
                    List<string> cclist = new List<string>();
                    string[] bcc = new string[] { };
                    List<string> bcclist = new List<string>();

                    var getfilename = dbC.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).FirstOrDefault();
                    var getreceiverdetail = dbC.vw_NSWL.Where(x => x.fld_LadangID == getfilename.fldLadangID && x.fld_WilayahID == getfilename.fldWilayahID && x.fld_SyarikatID == getfilename.fldSyarikatID && x.fld_NegaraID == getfilename.fldNegaraID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum,</p>";
                    msg += "<p>Permohonan untuk ID pengguna baru telah ditolak. Sila rujuk pihak wilayah untuk keterangan lanjut. Keterangan fail seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Nama File</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td>" + getreceiverdetail.fld_NamaWilayah + "</td><td>" + getreceiverdetail.fld_LdgCode + "</td><td>" + getreceiverdetail.fld_NamaLadang + "</td><td>" + getfilename.fldFileName + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    if (getreceiverdetail.fld_CostCentre == "1000")
                    {
                        departmentto = "REGION_USERID_APPROVAL_FELDA";
                        departmentcc = "HQ_USERID_APPROVAL_FELDA";
                    }

                    if (getreceiverdetail.fld_CostCentre == "8800")
                    {
                        departmentto = "REGION_USERID_APPROVAL_FPM";
                        departmentcc = "HQ_USERID_APPROVAL_FPM";
                    }

                    //var emaillist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldDeleted == false).ToList();

                    tolist.Add(getreceiverdetail.fld_LdgEmail);
                    to = tolist.ToArray();

                    //var emailcclist = emaillist.Where(x => (x.fldCategory == "CC" && x.fldDepartment == "HQ_USERID_APPROVAL") || (x.fldCategory == "CC" && x.fldDepartment == "HQ_USERID_APPROVAL")).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    var emailcclist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldCategory == "CC" && (x.fldDepartment == departmentcc || (x.fldDepartment == departmentto && x.fldWilayahID == getreceiverdetail.fld_WilayahID)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();


                    if (emailcclist != null)
                    {
                        foreach (var ccemail in emailcclist)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                    }
                    cc = cclist.ToArray();

                    //var emailbcclist = emaillist.Where(x => x.fldCategory == "BCC" && x.fldDepartment == "DEVELOPER").Select(s => new { s.fldEmail, s.fldName }).ToList();

                    var emailbcclist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldCategory == "BCC" && x.fldDepartment == "DEVELOPER" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    if (emailbcclist != null)
                    {
                        foreach (var bccemail in emailbcclist)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    if (SendEmailNotification.CheckEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "HQ"))
                    {
                        if (SendEmailNotification.SendEmail(subject, msg, to, cc, bcc))
                        {
                            SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "Email From HQ To Ladang - New User ID Approval", "HQ", 1);
                            msg2 = "Email berjaya dihantar";
                            status = 1;
                        }
                        else
                        {
                            SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "Email From HQ To Ladang - New User ID Approval", "HQ", 0);
                            msg2 = "Email gagal dihantar";
                            status = 2;
                        }
                        DatabaseAction.UpdateDataTotbltblTaskRemainder(getfilename.fldFileName, getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, "02");
                    }
                    else
                    {
                        msg2 = "Email sudah dihantar sebelum ini";
                        status = 4;
                    }
                    DatabaseAction.UpdateDataTotblASCApprovalFileDetail(fileid);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                success = false;
                msg2 = "System Problem.";
                status = 3;
            }

            //return Json(new { success = success, msg = msg2, status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApprovalIncrementSalaryOPMS(string kdldg, string ascfilename)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            string[] filename = new string[] { };
            string myfilename = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getWilayahID = 0;
            int getLadangID = 0;

            ViewBag.Approval = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var getreceiverdetail = dbC.vw_NSWL.Where(x => x.fld_LdgCode.Trim() == kdldg.Trim()).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID }).FirstOrDefault();

            if (kdldg != null && ascfilename != null)
            {
                WilayahID = getreceiverdetail.fld_WilayahID;
                LadangID = getreceiverdetail.fld_LadangID;
            }

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatList = new List<SelectListItem>(); //fatin added - 03/08/2023

            if (WilayahID == 0 && LadangID == 0)
            {
                getWilayahID = dbC.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                getLadangID = dbC.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                getWilayahID = dbC.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == WilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                getLadangID = dbC.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                getWilayahID = dbC.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == WilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                getLadangID = dbC.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID && x.fld_ID == LadangID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            }

            if (kdldg != null && ascfilename != null)
            {
                getWilayahID = getreceiverdetail.fld_WilayahID;
                getLadangID = getreceiverdetail.fld_LadangID;
            }

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                //Fatin added - 03/08/2023
                WilayahIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                LadangIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                //end
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                //Fatin added - 03/08/2023
                WilayahIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                LadangIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                //end
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                //Fatin added - 03/08/2023
                WilayahIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                LadangIDList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                //end
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.SyarikatList = SyarikatList;  //Fatin added - 03/08/2023

            var codeladang = dbC.tbl_Ladang.Where(x => x.fld_ID == getLadangID && x.fld_WlyhID == getWilayahID).Select(s => s.fld_LdgCode).FirstOrDefault();

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

            filename = dbC.tblEmailNotiStatus.Where(x => x.fldLadangID == getLadangID && x.fldWilayahID == getWilayahID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldEmailNotiSource == "Ladang").Select(s => s.fldEmailNotiFlag).ToArray();
            myfilename = String.Join(",", filename); ;
            List<ModelsCorporate.tblASCApprovalFileDetail> batch = new List<ModelsCorporate.tblASCApprovalFileDetail>();
            if (kdldg != null && ascfilename != null)
            {
                batch = dbC.tblASCApprovalFileDetails.Where(x => x.fldLadangID == getLadangID && x.fldWilayahID == getWilayahID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 0 && x.fldFileName == ascfilename && x.fldPurpose == 3).ToList();
            }
            else
            {
                batch = dbC.tblASCApprovalFileDetails.Where(x => x.fldLadangID == getLadangID && x.fldWilayahID == getWilayahID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 0 && x.fldFileName.Contains(myfilename) && x.fldPurpose == 3).ToList();
                batch = dbC.tblASCApprovalFileDetails.Where(x => x.fldLadangID == getLadangID && x.fldWilayahID == getWilayahID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 0 && x.fldPurpose == 3 && x.fldFileName.Contains(myfilename)).ToList();
            }

            return View(batch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovalIncrementSalaryOPMS(int? SyarikatList, int? WilayahIDList, int LadangIDList, int YearList, string kodladang) // fatin add param SyarikatList - 03/08/2023
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            string[] filename = new string[] { };
            string myfilename = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            ViewBag.Approval = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var codeladang = "";
            int nocodeladang = 0;
            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();
            List<SelectListItem> SyarikatList2 = new List<SelectListItem>(); //fatin added - 03/08/2023

            var CompCode = dbC.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_CostCentre).FirstOrDefault(); // fatin added - 01/08/2023

            if (kodladang != "")
            {
                codeladang = kodladang;
                var ladangdetail = dbC.tbl_Ladang.Where(x => x.fld_LdgCode == codeladang).Select(s => new { s.fld_ID, s.fld_WlyhID }).FirstOrDefault();
                if (ladangdetail != null)
                {
                    WilayahIDList = ladangdetail.fld_WlyhID;
                    LadangIDList = ladangdetail.fld_ID;
                    WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => x.fld_ID == ladangdetail.fld_WlyhID).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                    //fatin modified add costcentre - 03/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_ID == ladangdetail.fld_ID && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    //end
                }
                else
                {
                    nocodeladang = 1;
                }
            }
            else
            {
                codeladang = dbC.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
            }

            if ((WilayahID == 0 && LadangID == 0 && kodladang == "") || (nocodeladang == 1))
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                //Fatin added - 03/08/2023
                WilayahIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList2 = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_LdgName).FirstOrDefault();
                //end
                if (WilayahIDList == 0)
                {
                    //fatin modified add costcentre - 03/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); //Fatin added - 03/08/2023
                }
                else
                {
                    //fatin modified add costcentre - 03/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); //Fatin added - 03/08/2023
                }
            }
            else if ((WilayahID != 0 && LadangID == 0 && kodladang == "") || (nocodeladang == 1))
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                //Fatin added - 03/08/2023
                WilayahIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList2 = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_LdgName).FirstOrDefault();
                //end
                if (WilayahIDList == 0)
                {
                    //fatin modified add costcentre - 03/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); //Fatin added - 03/08/2023
                }
                else
                {
                    //fatin modified add costcentre - 03/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                    LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); //Fatin added - 03/08/2023
                }
            }
            else if ((WilayahID != 0 && LadangID != 0 && kodladang == "") || (nocodeladang == 1))
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                //fatin modified add costcentre - 03/08/2023
                LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();

                //Fatin added - 03/08/2023
                WilayahIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList2 = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_LdgName).FirstOrDefault();
                //end
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist2 = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist2; // list dalam dropdown
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.SyarikatList = SyarikatList2; //fatin added - 03/08/2023

            filename = dbC.tblEmailNotiStatus.Where(x => x.fldLadangID == LadangIDList && x.fldWilayahID == WilayahIDList && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldEmailNotiSource == "Ladang").Select(s => s.fldEmailNotiFlag).ToArray();
            myfilename = String.Join(",", filename);
            List<ModelsCorporate.tblASCApprovalFileDetail> batch = new List<ModelsCorporate.tblASCApprovalFileDetail>();
            batch = dbC.tblASCApprovalFileDetails.Where(x => x.fldLadangID == LadangIDList && x.fldWilayahID == WilayahIDList && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 0 && filename.Contains(x.fldFileName) && x.fldPurpose == 3).ToList();
            batch = dbC.tblASCApprovalFileDetails.Where(x => x.fldLadangID == LadangIDList && x.fldWilayahID == WilayahIDList && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 0 && x.fldPurpose == 3 && filename.Contains(x.fldFileName)).ToList();
            
            return View(batch);
        }

        public ActionResult GetIncrementSalaryApprovalFileOPMS(string filename, int ladangid, int wilayahid)
        {
            string linkfile = "";
            bool success = false;
            string msg = "";
            string status = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            var ladangcode = dbC.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
            int fileid = 0;

            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
            var lang = Request.RequestContext.RouteData.Values["lang"];
            //domain = domain + "/" + lang.ToString();

            if (appname != "/")
            {
                domain = domain + appname + "/" + lang.ToString();
            }

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            try
            {
                var fileexisting = dbC.tblASCApprovalFileDetails.Where(x => x.fldFileName == filename).FirstOrDefault();
                fileid = fileexisting.fldID;
                linkfile = domain + "/Approval/GetIncrementSalaryApprovalDetailOPMS/" + fileid;
                success = true;
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                success = false;
                msg = "System Problem.";
                status = "danger";
            }

            return Json(new { success = success, id = linkfile, msg = msg, status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIncrementSalaryApprovalDetailOPMS(long id)
        {
            ViewBag.Approval = "class = active";
            ViewBag.fileid = id;

            var getapprovaldetail = dbC.tbl_PkjIncrmntApp.Where(x => x.fld_FileID == id).ToList();
            ViewBag.getdata = getapprovaldetail.Where(x => x.fld_FileID == id).Count();
            return View(getapprovaldetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetIncrementSalaryApprovalDetailOPMS(long[] approval, int fileid, string Tolak)
        {
            ViewBag.Approval = "class = active";
            ViewBag.fileid = fileid;
            int status = 0;
            string msg2 = "";
            bool success = false;
            int? getuserid = getidentity.ID(User.Identity.Name);
            var GetNSWL = dbC.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).FirstOrDefault();

            Connection Connection = new Connection();
            string host, catalog, user, pass = "";

            Connection.GetConnection(out host, out catalog, out user, out pass, GetNSWL.fldWilayahID, GetNSWL.fldSyarikatID, GetNSWL.fldNegaraID);
            MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
            
            try
            {
                if (Tolak == null)
                {
                    if (approval != null)
                    {
                        var getIncrmntSlryAccpt = dbC.tbl_PkjIncrmntApp.Where(x=> approval.Contains(x.fld_ID) && x.fld_FileID == fileid).Select(s=>s.fld_Nopkj).ToArray();

                        var approvalupdateaccept = dbC.tbl_PkjIncrmntApp.Where(x => approval.Contains(x.fld_ID) && x.fld_FileID == fileid).ToList();
                        approvalupdateaccept.ForEach(u => u.fld_ProcessStage = 3);
                        approvalupdateaccept.ForEach(u => u.fld_AppStatus = true);
                        approvalupdateaccept.ForEach(u => u.fld_Deleted = false);
                        approvalupdateaccept.ForEach(u => u.fld_AppBy = getuserid);
                        approvalupdateaccept.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                        dbC.SaveChanges();

                        var Incrmtupdateaccept = dbr.tbl_PkjIncrmntSalary.Where(x => getIncrmntSlryAccpt.Contains(x.fld_Nopkj) && x.fld_FileID == fileid).ToList();
                        Incrmtupdateaccept.ForEach(u => u.fld_ProcessStage = 3);
                        Incrmtupdateaccept.ForEach(u => u.fld_AppStatus = true);
                        Incrmtupdateaccept.ForEach(u => u.fld_Deleted = false);
                        Incrmtupdateaccept.ForEach(u => u.fld_AppBy = getuserid);
                        Incrmtupdateaccept.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                        dbr.SaveChanges();

                        var Incrmtupdateaccepthistory = dbr.tbl_PkjIncrmntSalaryHistory.Where(x => getIncrmntSlryAccpt.Contains(x.fld_Nopkj) && x.fld_FileID == fileid).ToList();
                        Incrmtupdateaccepthistory.ForEach(u => u.fld_ProcessStage = 3);
                        Incrmtupdateaccepthistory.ForEach(u => u.fld_AppStatus = true);
                        Incrmtupdateaccepthistory.ForEach(u => u.fld_Deleted = false);
                        Incrmtupdateaccepthistory.ForEach(u => u.fld_AppBy = getuserid);
                        Incrmtupdateaccepthistory.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                        dbr.SaveChanges();

                        var getIncrmntSlryNotAccpt = dbC.tbl_PkjIncrmntApp.Where(x => !approval.Contains(x.fld_ID) && x.fld_FileID == fileid).Select(s => s.fld_Nopkj).ToArray();
                        if (getIncrmntSlryNotAccpt.Count() > 0)
                        {
                            var approvalupdatenotaccept = dbC.tbl_PkjIncrmntApp.Where(x => !approval.Contains(x.fld_ID) && x.fld_FileID == fileid).ToList();
                            approvalupdatenotaccept.ForEach(u => u.fld_ProcessStage = 3);
                            approvalupdatenotaccept.ForEach(u => u.fld_AppStatus = false);
                            approvalupdatenotaccept.ForEach(u => u.fld_Deleted = true);
                            approvalupdatenotaccept.ForEach(u => u.fld_AppBy = getuserid);
                            approvalupdatenotaccept.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                            dbC.SaveChanges();

                            var Incrmtupdatenotaccept = dbr.tbl_PkjIncrmntSalary.Where(x => getIncrmntSlryNotAccpt.Contains(x.fld_Nopkj) && x.fld_FileID == fileid).ToList();
                            Incrmtupdatenotaccept.ForEach(u => u.fld_ProcessStage = 3);
                            Incrmtupdatenotaccept.ForEach(u => u.fld_AppStatus = false);
                            Incrmtupdatenotaccept.ForEach(u => u.fld_Deleted = true);
                            Incrmtupdatenotaccept.ForEach(u => u.fld_AppBy = getuserid);
                            Incrmtupdatenotaccept.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                            dbr.SaveChanges();

                            var Incrmtupdatenotaccepthistory = dbr.tbl_PkjIncrmntSalaryHistory.Where(x => getIncrmntSlryNotAccpt.Contains(x.fld_Nopkj) && x.fld_FileID == fileid).ToList();
                            Incrmtupdatenotaccepthistory.ForEach(u => u.fld_ProcessStage = 3);
                            Incrmtupdatenotaccepthistory.ForEach(u => u.fld_AppStatus = false);
                            Incrmtupdatenotaccepthistory.ForEach(u => u.fld_Deleted = true);
                            Incrmtupdatenotaccepthistory.ForEach(u => u.fld_AppBy = getuserid);
                            Incrmtupdatenotaccepthistory.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                            dbr.SaveChanges();
                        }

                        GenIncrementSalaryApprovalFileOPMS(fileid, out status, out msg2, out success, false);

                        if (status == 1)
                        {
                            ViewBag.Status = msg2;
                            ViewBag.ClassStatus = "success";
                        }
                        else if (status == 2)
                        {
                            ViewBag.Status = msg2;
                            ViewBag.ClassStatus = "warning";
                        }
                        else if (status == 3)
                        {
                            ViewBag.Status = msg2;
                            ViewBag.ClassStatus = "warning";
                        }
                        else if (status == 4)
                        {
                            ViewBag.Status = msg2;
                            ViewBag.ClassStatus = "success";
                        }
                    }
                    else
                    {
                        var getIncrmntSlryNotAccpt = dbC.tbl_PkjIncrmntApp.Where(x => x.fld_FileID == fileid).Select(s => s.fld_Nopkj).ToArray();

                        var approvalupdatenotaccept = dbC.tbl_PkjIncrmntApp.Where(x => x.fld_FileID == fileid).ToList();
                        approvalupdatenotaccept.ForEach(u => u.fld_ProcessStage = 3);
                        approvalupdatenotaccept.ForEach(u => u.fld_AppStatus = false);
                        approvalupdatenotaccept.ForEach(u => u.fld_Deleted = true);
                        approvalupdatenotaccept.ForEach(u => u.fld_AppBy = getuserid);
                        approvalupdatenotaccept.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                        dbC.SaveChanges();

                        var Incrmtupdatenotaccept = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_FileID == fileid).ToList();
                        Incrmtupdatenotaccept.ForEach(u => u.fld_ProcessStage = 3);
                        Incrmtupdatenotaccept.ForEach(u => u.fld_AppStatus = false);
                        Incrmtupdatenotaccept.ForEach(u => u.fld_Deleted = true);
                        Incrmtupdatenotaccept.ForEach(u => u.fld_AppBy = getuserid);
                        Incrmtupdatenotaccept.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                        dbr.SaveChanges();

                        var Incrmtupdatenotaccepthistory = dbr.tbl_PkjIncrmntSalaryHistory.Where(x => x.fld_FileID == fileid).ToList();
                        Incrmtupdatenotaccepthistory.ForEach(u => u.fld_ProcessStage = 3);
                        Incrmtupdatenotaccepthistory.ForEach(u => u.fld_AppStatus = false);
                        Incrmtupdatenotaccepthistory.ForEach(u => u.fld_Deleted = true);
                        Incrmtupdatenotaccepthistory.ForEach(u => u.fld_AppBy = getuserid);
                        Incrmtupdatenotaccepthistory.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                        dbr.SaveChanges();
                        
                        ViewBag.Status = "Status berjaya dikemaskini";
                        ViewBag.ClassStatus = "success";
                    }
                }
                else
                {
                    var getIncrmntSlryNotAccpt = dbC.tbl_PkjIncrmntApp.Where(x => x.fld_FileID == fileid).Select(s => s.fld_Nopkj).ToArray();

                    var approvalupdatenotaccept = dbC.tbl_PkjIncrmntApp.Where(x => x.fld_FileID == fileid).ToList();
                    approvalupdatenotaccept.ForEach(u => u.fld_ProcessStage = 3);
                    approvalupdatenotaccept.ForEach(u => u.fld_AppStatus = false);
                    approvalupdatenotaccept.ForEach(u => u.fld_Deleted = true);
                    approvalupdatenotaccept.ForEach(u => u.fld_AppBy = getuserid);
                    approvalupdatenotaccept.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                    dbC.SaveChanges();

                    var Incrmtupdatenotaccept = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_FileID == fileid).ToList();
                    Incrmtupdatenotaccept.ForEach(u => u.fld_ProcessStage = 3);
                    Incrmtupdatenotaccept.ForEach(u => u.fld_AppStatus = false);
                    Incrmtupdatenotaccept.ForEach(u => u.fld_Deleted = true);
                    Incrmtupdatenotaccept.ForEach(u => u.fld_AppBy = getuserid);
                    Incrmtupdatenotaccept.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                    dbr.SaveChanges();

                    var Incrmtupdatenotaccepthistory = dbr.tbl_PkjIncrmntSalaryHistory.Where(x => x.fld_FileID == fileid).ToList();
                    Incrmtupdatenotaccepthistory.ForEach(u => u.fld_ProcessStage = 3);
                    Incrmtupdatenotaccepthistory.ForEach(u => u.fld_AppStatus = false);
                    Incrmtupdatenotaccepthistory.ForEach(u => u.fld_Deleted = true);
                    Incrmtupdatenotaccepthistory.ForEach(u => u.fld_AppBy = getuserid);
                    Incrmtupdatenotaccepthistory.ForEach(u => u.fld_AppDT = timezone.gettimezone());
                    dbr.SaveChanges();

                    GenIncrementSalaryApprovalFileOPMS(fileid, out status, out msg2, out success, true);

                    if (status == 1)
                    {
                        ViewBag.Status = msg2;
                        ViewBag.ClassStatus = "success";
                    }
                    else if (status == 2)
                    {
                        ViewBag.Status = msg2;
                        ViewBag.ClassStatus = "warning";
                    }
                    else if (status == 3)
                    {
                        ViewBag.Status = msg2;
                        ViewBag.ClassStatus = "warning";
                    }
                    else if (status == 4)
                    {
                        ViewBag.Status = msg2;
                        ViewBag.ClassStatus = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                ViewBag.Status = "Masalah Sistem";
                ViewBag.ClassStatus = "warning";
            }

            var getapprovaldetail = dbC.tbl_PkjIncrmntApp.Where(x => x.fld_FileID == fileid).ToList();
            ViewBag.getdata = dbC.tbl_PkjIncrmntApp.Where(x => x.fld_FileID == fileid && x.fld_AppStatus == true).Count();

            return View(getapprovaldetail);
        }

        public void GenIncrementSalaryApprovalFileOPMS(long fileid, out int status, out string msg2, out bool success, bool rejectemail)
        {
            //string msg2 = "";
            //string status = "";
            //bool success = false;
            status = 0;
            msg2 = "";
            success = false;
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = getidentity.MyNameFullName(getuserid);
            DateTime getdatetime = timezone.gettimezone();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            try
            {
                if (rejectemail == false)
                {
                    string subject = "Permohonan Gaji Pekerja Telah Diluluskan";
                    string msg = "";
                    string[] to = new string[] { };
                    List<string> tolist = new List<string>();
                    string[] cc = new string[] { };
                    List<string> cclist = new List<string>();
                    string[] bcc = new string[] { };
                    List<string> bcclist = new List<string>();

                    var getfilename = dbC.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).FirstOrDefault();
                    var getreceiverdetail = dbC.vw_NSWL.Where(x => x.fld_LadangID == getfilename.fldLadangID && x.fld_WilayahID == getfilename.fldWilayahID && x.fld_SyarikatID == getfilename.fldSyarikatID && x.fld_NegaraID == getfilename.fldNegaraID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum,</p>";
                    //msg += "<p><font color=\"red\">INI ADALAH CUBAAN SEMATA - MATA </font></p>";
                    msg += "<p>Permohonan Gaji Pekerja telah diluluskan. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Nama File</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td>" + getreceiverdetail.fld_NamaWilayah + "</td><td>" + getreceiverdetail.fld_LdgCode + "</td><td>" + getreceiverdetail.fld_NamaLadang + "</td><td>" + getfilename.fldFileName + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emaillist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldDeleted == false).ToList();

                    tolist.Add(getreceiverdetail.fld_LdgEmail);
                    to = tolist.ToArray();

                    var emailcclist = emaillist.Where(x => (x.fldCategory == "CC" && x.fldDepartment == "HQ_WORKER_APPROVAL") || (x.fldCategory == "CC" && x.fldDepartment == "REGION_WORKER_APPROVAL")).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailcclist != null)
                    {
                        foreach (var ccemail in emailcclist)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                    }
                    cc = cclist.ToArray();

                    var emailbcclist = emaillist.Where(x => x.fldCategory == "BCC" && x.fldDepartment == "DEVELOPER").Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist != null)
                    {
                        foreach (var bccemail in emailbcclist)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    if (SendEmailNotification.CheckEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "HQ"))
                    {
                        //if (SendEmailNotification.SendEmail(subject, msg, getreceiverdetail.fld_LdgEmail, cc, bcc))
                        if (SendEmailNotification.SendEmail(subject, msg, to, cc, bcc))
                        {
                            SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "Email From HQ To Ladang - New User ID Approval", "HQ", 1);
                            msg2 = "Email berjaya dihantar";
                            status = 1;
                        }
                        else
                        {
                            SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "Email From HQ To Ladang - New User ID Approval", "HQ", 0);
                            msg2 = "Email gagal dihantar";
                            status = 2;
                        }
                        DatabaseAction.UpdateDataTotbltblTaskRemainder(getfilename.fldFileName, getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, "02");
                    }
                    else
                    {
                        msg2 = "Email sudah dihantar sebelum ini";
                        status = 4;
                    }
                    DatabaseAction.UpdateDataTotblASCApprovalFileDetail(fileid);
                    success = true;
                }
                else
                {
                    string subject = "Permohonan ID Pengguna Baru Telah Ditolak";
                    string msg = "";
                    string[] to = new string[] { };
                    List<string> tolist = new List<string>();
                    string[] cc = new string[] { };
                    List<string> cclist = new List<string>();
                    string[] bcc = new string[] { };
                    List<string> bcclist = new List<string>();

                    var getfilename = dbC.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).FirstOrDefault();
                    var getreceiverdetail = dbC.vw_NSWL.Where(x => x.fld_LadangID == getfilename.fldLadangID && x.fld_WilayahID == getfilename.fldWilayahID && x.fld_SyarikatID == getfilename.fldSyarikatID && x.fld_NegaraID == getfilename.fldNegaraID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum,</p>";
                    msg += "<p>Permohonan untuk ID pengguna baru telah ditolak. Sila rujuk pihak wilayah untuk keterangan lanjut. Keterangan fail seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Nama File</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td>" + getreceiverdetail.fld_NamaWilayah + "</td><td>" + getreceiverdetail.fld_LdgCode + "</td><td>" + getreceiverdetail.fld_NamaLadang + "</td><td>" + getfilename.fldFileName + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emaillist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldDeleted == false).ToList();

                    tolist.Add(getreceiverdetail.fld_LdgEmail);
                    to = tolist.ToArray();

                    var emailcclist = emaillist.Where(x => (x.fldCategory == "CC" && x.fldDepartment == "HQ_USERID_APPROVAL") || (x.fldCategory == "CC" && x.fldDepartment == "HQ_USERID_APPROVAL")).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailcclist != null)
                    {
                        foreach (var ccemail in emailcclist)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                    }
                    cc = cclist.ToArray();

                    var emailbcclist = emaillist.Where(x => x.fldCategory == "BCC" && x.fldDepartment == "DEVELOPER").Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist != null)
                    {
                        foreach (var bccemail in emailbcclist)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    if (SendEmailNotification.CheckEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "HQ"))
                    {
                        if (SendEmailNotification.SendEmail(subject, msg, to, cc, bcc))
                        {
                            SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "Email From HQ To Ladang - New User ID Approval", "HQ", 1);
                            msg2 = "Email berjaya dihantar";
                            status = 1;
                        }
                        else
                        {
                            SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, getfilename.fldFileName, "Email From HQ To Ladang - New User ID Approval", "HQ", 0);
                            msg2 = "Email gagal dihantar";
                            status = 2;
                        }
                        DatabaseAction.UpdateDataTotbltblTaskRemainder(getfilename.fldFileName, getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, "02");
                    }
                    else
                    {
                        msg2 = "Email sudah dihantar sebelum ini";
                        status = 4;
                    }
                    DatabaseAction.UpdateDataTotblASCApprovalFileDetail(fileid);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                success = false;
                msg2 = "System Problem.";
                status = 3;
            }

            //return Json(new { success = success, msg = msg2, status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RejectApprovalWorker()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            string[] filename = new string[] { };
            string myfilename = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getWilayahID = 0;
            int getLadangID = 0;

            ViewBag.Approval = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                getWilayahID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                getLadangID = db2.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                getWilayahID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == WilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                getLadangID = db2.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                getWilayahID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == WilayahID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                getLadangID = db2.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID && x.fld_ID == LadangID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            }


            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;

            var codeladang = db2.tbl_Ladang.Where(x => x.fld_ID == getLadangID && x.fld_WlyhID == getWilayahID).Select(s => s.fld_LdgCode).FirstOrDefault();

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

            filename = db.tblEmailNotiStatus.Where(x => x.fldLadangID == getLadangID && x.fldWilayahID == getWilayahID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldEmailNotiSource == "HQ").Select(s => s.fldEmailNotiFlag).ToArray();
            myfilename = String.Join(",", filename); ;
            List<ModelsCorporate.tblASCApprovalFileDetail> batch = new List<ModelsCorporate.tblASCApprovalFileDetail>();

            batch = db.tblASCApprovalFileDetails.Where(x => x.fldLadangID == getLadangID && x.fldWilayahID == getWilayahID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 1 && x.fldFileName.Contains(myfilename)).ToList();

            return View(batch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectApprovalWorker(int? WilayahIDList, int LadangIDList, int YearList, string kodladang)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            string[] filename = new string[] { };
            string myfilename = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            ViewBag.Approval = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var codeladang = "";
            int nocodeladang = 0;
            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (kodladang != "")
            {
                codeladang = kodladang;
                var ladangdetail = db2.tbl_Ladang.Where(x => x.fld_LdgCode == codeladang).Select(s => new { s.fld_ID, s.fld_WlyhID }).FirstOrDefault();
                if (ladangdetail != null)
                {
                    WilayahIDList = ladangdetail.fld_WlyhID;
                    LadangIDList = ladangdetail.fld_ID;
                    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => x.fld_ID == ladangdetail.fld_WlyhID), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_ID == ladangdetail.fld_ID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    nocodeladang = 1;
                }
            }
            else
            {
                codeladang = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
            }

            if ((WilayahID == 0 && LadangID == 0 && kodladang == "") || (nocodeladang == 1))
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
            }
            else if ((WilayahID != 0 && LadangID == 0 && kodladang == "") || (nocodeladang == 1))
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
            }
            else if ((WilayahID != 0 && LadangID != 0 && kodladang == "") || (nocodeladang == 1))
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist2 = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist2; // list dalam dropdown
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            filename = db.tblEmailNotiStatus.Where(x => x.fldLadangID == LadangIDList && x.fldWilayahID == WilayahIDList && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldEmailNotiSource == "HQ").Select(s => s.fldEmailNotiFlag).ToArray();
            myfilename = String.Join(",", filename); ;
            List<ModelsCorporate.tblASCApprovalFileDetail> batch = new List<ModelsCorporate.tblASCApprovalFileDetail>();
            batch = db.tblASCApprovalFileDetails.Where(x => x.fldLadangID == LadangIDList && x.fldWilayahID == WilayahIDList && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldGenStatus == 1 && filename.Contains(x.fldFileName)).ToList();
            return View(batch);
        }

        public ActionResult GetRejectWorkerApprovalFile(string filename, int ladangid, int wilayahid)
        {
            string linkfile = "";
            bool success = false;
            string msg = "";
            string status = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            var ladangcode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
            int fileid = 0;

            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
            var lang = Request.RequestContext.RouteData.Values["lang"];
            //domain = domain + "/" + lang.ToString();

            if (appname != "/")
            {
                domain = domain + appname + "/" + lang.ToString();
            }

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            try
            {
                var fileexisting = db.tblASCApprovalFileDetails.Where(x => x.fldFileName == filename).FirstOrDefault();
                fileid = fileexisting.fldID;
                linkfile = domain + "/Approval/GetRejectWorkerApprovalDetail/" + fileid;
                success = true;
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                success = false;
                msg = "System Problem.";
                status = "danger";
            }

            return Json(new { success = success, id = linkfile, msg = msg, status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRejectWorkerApprovalDetail(long id)
        {
            ViewBag.Approval = "class = active";
            ViewBag.fileid = id;
            var getnulldata = db.tblPkjmastApp.Where(x => x.fldNoPkj == null).ToList();
            if (getnulldata.Count() > 0)
            {
                db.tblPkjmastApp.RemoveRange(getnulldata);
                db.SaveChanges();
            }

            var getapprovaldetail = db.tblPkjmastApp.Where(x => x.fldFileID == id).ToList();
            ViewBag.getdata = db.tblPkjmastApp.Where(x => x.fldFileID == id && x.fldStatus == 1).Count();
            return View(getapprovaldetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRejectWorkerApprovalDetail(long[] approval, int fileid, string Tolak)
        {
            ViewBag.Approval = "class = active";
            ViewBag.fileid = fileid;
            int? getuserid = getidentity.ID(User.Identity.Name);
            try
            {
                if (Tolak == null)
                {
                    if(approval != null)
                    {
                        var approvalupdateaccept = db.tblPkjmastApp.Where(x => approval.Contains(x.fldID)).ToList();
                        approvalupdateaccept.ForEach(x => x.fldStatus = 0);
                        approvalupdateaccept.ForEach(x => x.fldDateTimeApprove = timezone.gettimezone());
                        approvalupdateaccept.ForEach(x => x.fldActionBy = getuserid);
                        db.SaveChanges();
                    }

                    ViewBag.Status = "Status berjaya dikemaskini";
                    ViewBag.ClassStatus = "success";
                }
                else
                {
                    var approvalupdatenotaccept = db.tblPkjmastApp.Where(x => x.fldFileID == fileid).ToList();
                    approvalupdatenotaccept.ForEach(x => x.fldStatus = 0);
                    approvalupdatenotaccept.ForEach(x => x.fldDateTimeApprove = timezone.gettimezone());
                    approvalupdatenotaccept.ForEach(x => x.fldActionBy = getuserid);
                    db.SaveChanges();

                    ViewBag.Status = "Status telah berjaya dikemaskini";
                    ViewBag.ClassStatus = "success";

                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                ViewBag.Status = "Masalah Sistem";
                ViewBag.ClassStatus = "warning";
            }
            var getapprovaldetail = db.tblPkjmastApp.Where(x => x.fldFileID == fileid).ToList();
            ViewBag.getdata = db.tblPkjmastApp.Where(x => x.fldFileID == fileid && x.fldStatus == 1).Count();
            return View(getapprovaldetail);
        }

        public ActionResult ApprovalNewWorkerOPMS(int SyarikatList = 0, int WilayahIDList = 0, int LadangIDList = 0) //fatin modified add SyarikatList - 17/07/2023
        {
            ViewBag.Approval = "class = active";
            int[] wlyhid = new int[] { };
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? roleIDUser = getidentity.RoleID(getuserid);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            // fatin added - 17/07/2023
            List<SelectListItem> SyarikatList2 = new List<SelectListItem>();
            var CompCode = dbC.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_CostCentre).FirstOrDefault();
            //end

            int? getWilayahID = 0;
            int getLadangID = 0;
            //11 OGOS 2021 SEPUL TAMBAH ROLE
            //if (roleIDUser != 4 && roleIDUser != 5)  //fatin comment - 07/08/2023
            if (roleIDUser != 4 && roleIDUser != 5 && roleIDUser != 10) // fatin added - 07/08/2023
            {
                if (WilayahIDList == 0 && LadangIDList == 0)
                {
                    getWilayahID = dbC.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID).Select(s => s.fld_ID).Take(1).FirstOrDefault();
                }

                else if (WilayahIDList != 0 && LadangIDList != 0)
                {
                    getWilayahID = WilayahIDList;
                }

                if (WilayahIDList == 0 && LadangIDList == 0)
                {
                    wlyhid = getwilyah.GetWilayahID(SyarikatID);
                    WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();
                    //fatin modified add costcentre - 02/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_WlyhID == getWilayahID && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                    //Fatin added - 17/07/2023
                    WilayahIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                    LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                    if (SyarikatID == 1)
                    {
                        SyarikatList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                    }

                    if (SyarikatID == 2)
                    {
                        SyarikatList2 = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                    }
                    SyarikatList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                    ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_LdgName).FirstOrDefault();
                    //end
                }

                else if (WilayahIDList != 0 && LadangIDList != 0)
                {
                    wlyhid = getwilyah.GetWilayahID(SyarikatID);
                    WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();
                    //fatin modified add costcentre - 02/08/2023
                    LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                    //Fatin added - 17/07/2023
                    WilayahIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                    LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                    if (SyarikatID == 1)
                    {
                        SyarikatList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                    }

                    if (SyarikatID == 2)
                    {
                        SyarikatList2 = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                    }
                    SyarikatList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                    ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_LdgName).FirstOrDefault();
                    //end
                }

                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                ViewBag.SyarikatList = SyarikatList2; //fatin added - 17/07/2023


                if (WilayahIDList == 0 && LadangIDList == 0)
                {
                    ViewBag.resultcount = 0;
                    return View("ApprovalNewWorkerOPMS");
                }
                else if (WilayahID != 0 && LadangID == 0)
                {
                    var resultreport = dbhq.tblASCApprovalFileDetails.Where(x => x.fldWilayahID == WilayahIDList && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldASCFileStatus == 1 && x.fldPurpose == 2).OrderBy(o => o.fldWilayahID);
                    ViewBag.resultcount = resultreport.Count();
                    return View("ApprovalNewWorkerOPMS", resultreport);
                }
                else
                {
                    var resultreport = dbhq.tblASCApprovalFileDetails.Where(x => x.fldWilayahID == WilayahIDList && x.fldLadangID == LadangIDList && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldASCFileStatus == 1 && x.fldPurpose == 2).OrderBy(o => o.fldWilayahID);
                    ViewBag.resultcount = resultreport.Count();
                    return View("ApprovalNewWorkerOPMS", resultreport);
                }

            }
            else
            {
                //11 ogos 2021 SEPUL TAMBAH
                WilayahIDList2 = new SelectList(dbC.tbl_Wilayah.Where(x => x.fld_ID == WilayahID).OrderBy(o => o.fld_WlyhName), "fld_ID", "fld_WlyhName").ToList();
                //fatin modified add costcentre - 02/08/2023
                LadangIDList2 = new SelectList(dbC.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false && x.fld_CostCentre == CompCode).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                //Fatin added - 17/07/2023
                WilayahIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                if (SyarikatID == 1)
                {
                    SyarikatList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                }

                if (SyarikatID == 2)
                {
                    SyarikatList2 = new SelectList(dbC.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => new SelectListItem { Value = s.fld_SyarikatID.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text").ToList();
                }
                SyarikatList2.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                //end
                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                //fatin added - 17/07/2023
                ViewBag.SyarikatList = SyarikatList2;
                ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).Select(s => s.fld_LdgName).FirstOrDefault();
                //end
                //fatin comment - 07/08/2023
                //var resultreport = dbhq.tblASCApprovalFileDetails.Where(x => x.fldWilayahID == WilayahIDList && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldASCFileStatus == 1 && x.fldPurpose == 2).OrderBy(o => o.fldWilayahID);
                //fatin added - 07/08/2023
                var resultreport = dbhq.tblASCApprovalFileDetails.Where(x => x.fldWilayahID == WilayahIDList && x.fldLadangID == LadangIDList && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldASCFileStatus == 1 && x.fldPurpose == 2).OrderBy(o => o.fldWilayahID);
                //end
                ViewBag.resultcount = resultreport.Count();
                return View("ApprovalNewWorkerOPMS", resultreport);
            }
        }
        public PartialViewResult ApprovalNewWorkerOPMSDetail(int fileID)
        {
            MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
            var result = dbhq.tblPkjmastApp.Where(x => x.fldFileID == fileID && x.fldStatus == 2);
            //var result = db.tblPkjmastApps.Where(x => x.fldFileID == fileID);
            ViewBag.Datacount = result.Count();
            return PartialView("ApprovalNewWorkerOPMSDetail", result);
        }

        public JsonResult ActionApprove(int act, int id, string sbbTolak)
        {
            ViewBag.Approval = "class = active";
            Connection Connection = new Connection();
            Boolean status = false;
            int year = DateTime.Now.Year;
            DateTime lastDay = new DateTime(year, 12, 31);

            if (act == 1)
            {
                //approve(1)
                MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
                var app1 = dbhq.tblPkjmastApp.Where(x => x.fldID == id).FirstOrDefault();
                if (app1 != null && app1.fldSbbMsk.Trim() == "PL")
                {
                    string host2, catalog2, user2, pass2 = "";
                    Connection.GetConnection(out host2, out catalog2, out user2, out pass2, app1.fldWilayahAsal, app1.fldSyarikatAsal, app1.fldNegaraAsal);
                    MVC_SYSTEM_ModelsEstate dbAsal = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host2, catalog2, user2, pass2);

                    //using (TransactionScope scope = new TransactionScope())
                    //{
                    try
                    {
                        //update pkjmastapp
                        var GetOldDataPkj = dbAsal.tbl_Pkjmast.Where(x => x.fld_Nopkj == app1.fldNoPkjAsal && x.fld_NegaraID == app1.fldNegaraAsal && x.fld_SyarikatID == app1.fldSyarikatAsal && x.fld_WilayahID == app1.fldWilayahAsal && x.fld_LadangID == app1.fldLadangAsal).FirstOrDefault();
                        if (app1 != null)
                        {
                            app1.fldStatus = 1;
                            app1.fldDateTimeApprove = DateTime.Now;
                            app1.fldActionBy = getidentity.ID(User.Identity.Name);
                            dbhq.SaveChanges();
                        }

                        string nopkj = app1.fldNoPkj;
                        int ldgID = app1.fldLadangID.Value;
                        int wlyhID = app1.fldWilayahID.Value;
                        int syrktID = app1.fldSyarikatID.Value;
                        int ngraID = app1.fldNegaraID.Value;
                        string host, catalog, user, pass = "";

                        //update pkjmast
                        Connection.GetConnection(out host, out catalog, out user, out pass, wlyhID, syrktID, ngraID);
                        MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
                        var app2 = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == nopkj && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_WilayahID == wlyhID && x.fld_LadangID == ldgID).FirstOrDefault();
                        if (app2 != null)
                        {
                            app2.fld_Kdaktf = "1";
                            app2.fld_StatusAkaun = "1";
                            app2.fld_StatusApproved = 1;
                            app2.fld_KodKWSP = GetOldDataPkj.fld_KodKWSP;
                            app2.fld_KodSocso = GetOldDataPkj.fld_KodSocso;
                            app2.fld_Noperkeso = GetOldDataPkj.fld_Noperkeso;
                            app2.fld_Nokwsp = GetOldDataPkj.fld_Nokwsp;
                            app2.fld_ActionBy = User.Identity.Name;
                            app2.fld_ActionDate = DateTime.Now;
                            dbr.SaveChanges();
                        }

                        //update cuti
                        var CutiTransfer = dbAsal.tbl_CutiPeruntukan.Where(x => x.fld_NoPkj == app1.fldNoPkjAsal && x.fld_Tahun == year && x.fld_NegaraID == app1.fldNegaraAsal && x.fld_SyarikatID == app1.fldSyarikatAsal && x.fld_WilayahID == app1.fldWilayahAsal && x.fld_LadangID == app1.fldLadangAsal && x.fld_Deleted == false).ToList();
                        if (CutiTransfer.Count() != 0)
                        {
                            foreach (var item in CutiTransfer)
                            {
                                ModelsEstate.tbl_CutiPeruntukan tblLeaves = new tbl_CutiPeruntukan();
                                tblLeaves.fld_KodCuti = item.fld_KodCuti;
                                tblLeaves.fld_NoPkj = nopkj;
                                tblLeaves.fld_Tahun = item.fld_Tahun;
                                tblLeaves.fld_JumlahCuti = item.fld_JumlahCuti;
                                tblLeaves.fld_JumlahCutiDiambil = item.fld_JumlahCutiDiambil;
                                tblLeaves.fld_NegaraID = ngraID;
                                tblLeaves.fld_SyarikatID = syrktID;
                                tblLeaves.fld_WilayahID = wlyhID;
                                tblLeaves.fld_LadangID = ldgID;
                                tblLeaves.fld_Deleted = false;
                                dbr.tbl_CutiPeruntukan.Add(tblLeaves);
                                dbr.SaveChanges();
                            }
                        }

                        //update caruman tambahan
                        //if (app2.fld_Kdrkyt == "MA") //fatin comment - 16/05/2024
                        //{
                            var CarumanTransfer = dbAsal.tbl_PkjCarumanTambahan.Where(x => x.fld_Nopkj == app1.fldNoPkjAsal && x.fld_NegaraID == app1.fldNegaraAsal && x.fld_SyarikatID == app1.fldSyarikatAsal && x.fld_WilayahID == app1.fldWilayahAsal && x.fld_LadangID == app1.fldLadangAsal && x.fld_Deleted == false).ToList();
                            if (CarumanTransfer.Count() != 0)
                            {
                                foreach (var item in CarumanTransfer)
                                {
                                    tbl_PkjCarumanTambahan pkjCarumanTambahan = new tbl_PkjCarumanTambahan();
                                    pkjCarumanTambahan.fld_Nopkj = nopkj;
                                    pkjCarumanTambahan.fld_KodCaruman = item.fld_KodCaruman;
                                    pkjCarumanTambahan.fld_KodSubCaruman = item.fld_KodSubCaruman;
                                    pkjCarumanTambahan.fld_NegaraID = ngraID;
                                    pkjCarumanTambahan.fld_SyarikatID = syrktID;
                                    pkjCarumanTambahan.fld_WilayahID = wlyhID;
                                    pkjCarumanTambahan.fld_LadangID = ldgID;
                                    pkjCarumanTambahan.fld_Deleted = false;
                                    dbr.tbl_PkjCarumanTambahan.Add(pkjCarumanTambahan);
                                    dbr.SaveChanges();
                                }
                            }
                        //}
                        status = true;
                        //scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        //scope.Dispose();
                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    }
                    //}
                }
                else if (app1 != null && app1.fldSbbMsk.Trim() == "PB")
                {
                    //using (TransactionScope scope = new TransactionScope())
                    //{
                    try
                    {
                        //update pkjmastapp
                        if (app1 != null)
                        {
                            app1.fldStatus = 1;
                            app1.fldDateTimeApprove = DateTime.Now;
                            app1.fldActionBy = getidentity.ID(User.Identity.Name);
                            dbhq.SaveChanges();
                        }

                        string nopkj = app1.fldNoPkj;
                        int ldgID = app1.fldLadangID.Value;
                        int wlyhID = app1.fldWilayahID.Value;
                        int syrktID = app1.fldSyarikatID.Value;
                        int ngraID = app1.fldNegaraID.Value;
                        string host, catalog, user, pass = "";

                        //update pkjmast
                        Connection.GetConnection(out host, out catalog, out user, out pass, wlyhID, syrktID, ngraID);
                        MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
                        var app2 = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == nopkj && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_WilayahID == wlyhID && x.fld_LadangID == ldgID).FirstOrDefault();
                        if (app2 != null)
                        {
                            app2.fld_Kdaktf = "1";
                            app2.fld_StatusAkaun = "1";
                            app2.fld_StatusApproved = 1;
                            app2.fld_ActionBy = User.Identity.Name;
                            app2.fld_ActionDate = DateTime.Now;
                            dbr.SaveChanges();
                        }

                        //calculate annual leave
                        DateDiff dateDiff = new DateDiff(Convert.ToDateTime(app2.fld_Trmlkj).AddDays(-1), lastDay);
                        var kodCutiTahunan = dbhq.tblOptionConfigsWebs.SingleOrDefault(x => x.fldOptConfFlag1 == "kodCutiTahunan" && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fldDeleted == false);
                        var cutiTahunanPkj = dbhq.tbl_CutiMaintenance.Where(x => x.fld_JenisCuti == kodCutiTahunan.fldOptConfValue && x.fld_Deleted == false && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_LowerLimit <= dateDiff.Months && x.fld_UpperLimit >= dateDiff.Months).Select(s => s.fld_PeruntukkanCuti).FirstOrDefault();

                        if (!dbr.tbl_CutiPeruntukan.Any(x => x.fld_NoPkj == nopkj && x.fld_KodCuti == kodCutiTahunan.fldOptConfValue && x.fld_Tahun == year && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_WilayahID == wlyhID && x.fld_LadangID == ldgID && x.fld_Deleted == false))
                        {
                            ModelsEstate.tbl_CutiPeruntukan CutiPeruntukanTahunan = new tbl_CutiPeruntukan();
                            CutiPeruntukanTahunan.fld_NoPkj = nopkj;
                            CutiPeruntukanTahunan.fld_JumlahCuti = cutiTahunanPkj;
                            CutiPeruntukanTahunan.fld_KodCuti = kodCutiTahunan.fldOptConfValue;
                            CutiPeruntukanTahunan.fld_JumlahCutiDiambil = 0;
                            CutiPeruntukanTahunan.fld_Tahun = Convert.ToInt16(year);
                            CutiPeruntukanTahunan.fld_NegaraID = ngraID;
                            CutiPeruntukanTahunan.fld_SyarikatID = syrktID;
                            CutiPeruntukanTahunan.fld_WilayahID = wlyhID;
                            CutiPeruntukanTahunan.fld_LadangID = ldgID;
                            CutiPeruntukanTahunan.fld_Deleted = false;
                            dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanTahunan);
                            dbr.SaveChanges();
                        }

                        //calculate sick leave
                        var kodCutiSakit = dbhq.tblOptionConfigsWebs.SingleOrDefault(x => x.fldOptConfFlag1 == "kodCutiSakit" && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fldDeleted == false);
                        var cutiSakitPkj = dbhq.tbl_CutiMaintenance.Where(x => x.fld_JenisCuti == kodCutiSakit.fldOptConfValue && x.fld_Deleted == false && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_LowerLimit <= dateDiff.Years && x.fld_UpperLimit >= dateDiff.Years).Select(s => s.fld_PeruntukkanCuti).Single();

                        if (!dbr.tbl_CutiPeruntukan.Any(x => x.fld_NoPkj == nopkj && x.fld_KodCuti == kodCutiSakit.fldOptConfValue && x.fld_Tahun == year && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_WilayahID == wlyhID && x.fld_LadangID == ldgID && x.fld_Deleted == false))
                        {
                            ModelsEstate.tbl_CutiPeruntukan CutiPeruntukanSakit = new tbl_CutiPeruntukan();
                            CutiPeruntukanSakit.fld_NoPkj = nopkj;
                            CutiPeruntukanSakit.fld_JumlahCuti = cutiSakitPkj;
                            CutiPeruntukanSakit.fld_KodCuti = kodCutiSakit.fldOptConfValue;
                            CutiPeruntukanSakit.fld_JumlahCutiDiambil = 0;
                            CutiPeruntukanSakit.fld_Tahun = Convert.ToInt16(year);
                            CutiPeruntukanSakit.fld_NegaraID = ngraID;
                            CutiPeruntukanSakit.fld_SyarikatID = syrktID;
                            CutiPeruntukanSakit.fld_WilayahID = wlyhID;
                            CutiPeruntukanSakit.fld_LadangID = ldgID;
                            CutiPeruntukanSakit.fld_Deleted = false;
                            dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanSakit);
                            dbr.SaveChanges();
                        }

                        var kodCutiUmum = dbhq.tblOptionConfigsWebs.SingleOrDefault(x => x.fldOptConfFlag1 == "kodCutiAm" && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fldDeleted == false);

                        if (!dbr.tbl_CutiPeruntukan.Any(x => x.fld_NoPkj == nopkj && x.fld_KodCuti == kodCutiUmum.fldOptConfValue && x.fld_Tahun == year && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_WilayahID == wlyhID && x.fld_LadangID == ldgID && x.fld_Deleted == false))
                        {
                            ModelsEstate.tbl_CutiPeruntukan CutiPeruntukanUmum = new tbl_CutiPeruntukan();
                            CutiPeruntukanUmum.fld_NoPkj = nopkj;
                            CutiPeruntukanUmum.fld_JumlahCuti = 11;
                            CutiPeruntukanUmum.fld_KodCuti = kodCutiUmum.fldOptConfValue;
                            CutiPeruntukanUmum.fld_JumlahCutiDiambil = 0;
                            CutiPeruntukanUmum.fld_Tahun = Convert.ToInt16(year);
                            CutiPeruntukanUmum.fld_NegaraID = ngraID;
                            CutiPeruntukanUmum.fld_SyarikatID = syrktID;
                            CutiPeruntukanUmum.fld_WilayahID = wlyhID;
                            CutiPeruntukanUmum.fld_LadangID = ldgID;
                            CutiPeruntukanUmum.fld_Deleted = false;
                            dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanUmum);
                            dbr.SaveChanges();
                        }

                        DateDiff umurPekerja = new DateDiff(Convert.ToDateTime(app2.fld_Trlhr).AddDays(-1), lastDay);
                        var jnsSocso = dbhq.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodCarumanSocso" && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
                        var kodSocso = dbhq.tbl_JenisCaruman.Where(x => x.fld_UmurLower <= umurPekerja.Years && x.fld_UmurUpper >= umurPekerja.Years && x.fld_JenisCaruman == jnsSocso && x.fld_Default == true).Select(s => s.fld_KodCaruman).FirstOrDefault();

                        //kwsp & socso
                        if (app2.fld_Kdrkyt != "MA")
                        {
                            if (app2 != null)
                            {
                                app2.fld_KodSocso = kodSocso;
                                app2.fld_StatusKwspSocso = "1";
                                dbr.SaveChanges();
                            }

                            var activeContributionCategoryData = dbhq.tbl_CarumanTambahan.Where(x => x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_Deleted == false && x.fld_Default == true && x.fld_Warganegara == 2).ToList();

                            foreach (var activeContribution in activeContributionCategoryData)
                            {
                                var activeSubContributionCategoryData = db.tbl_SubCarumanTambahan.Where(x => x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_Deleted == false && x.fld_KodCaruman == activeContribution.fld_KodCaruman);
                                foreach (var activeSubContribution in activeSubContributionCategoryData)
                                {
                                    if (activeSubContribution.fld_UmurLower <= umurPekerja.Years && activeSubContribution.fld_UmurUpper >= umurPekerja.Years)
                                    {
                                        if (!dbr.tbl_PkjCarumanTambahan.Any(x => x.fld_Nopkj == nopkj && x.fld_KodCaruman == activeContribution.fld_KodCaruman && x.fld_KodSubCaruman == activeSubContribution.fld_KodSubCaruman && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_WilayahID == wlyhID && x.fld_LadangID == ldgID && x.fld_Deleted == false))
                                        {
                                            tbl_PkjCarumanTambahan pkjCarumanTambahan = new tbl_PkjCarumanTambahan();
                                            pkjCarumanTambahan.fld_Nopkj = nopkj;
                                            pkjCarumanTambahan.fld_KodCaruman = activeContribution.fld_KodCaruman;
                                            pkjCarumanTambahan.fld_KodSubCaruman = activeSubContribution.fld_KodSubCaruman;
                                            pkjCarumanTambahan.fld_NegaraID = ngraID;
                                            pkjCarumanTambahan.fld_SyarikatID = syrktID;
                                            pkjCarumanTambahan.fld_WilayahID = wlyhID;
                                            pkjCarumanTambahan.fld_LadangID = ldgID;
                                            pkjCarumanTambahan.fld_Deleted = false;
                                            dbr.tbl_PkjCarumanTambahan.Add(pkjCarumanTambahan);
                                            dbr.SaveChanges();
                                        }
                                    }
                                }
                            }

                        }
                        else if (app2.fld_Kdrkyt == "MA")
                        {
                            var jnsKwsp = dbhq.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodCarumanKwsp" && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
                            var kodKwsp = dbhq.tbl_JenisCaruman.Where(x => x.fld_UmurLower <= umurPekerja.Years && x.fld_UmurUpper >= umurPekerja.Years && x.fld_JenisCaruman == jnsKwsp && x.fld_Default == true).Select(s => s.fld_KodCaruman).FirstOrDefault();

                            if (app2 != null)
                            {
                                app2.fld_KodSocso = kodSocso;
                                app2.fld_KodKWSP = kodKwsp;
                                app2.fld_StatusKwspSocso = "1";
                                dbr.SaveChanges();
                            }

                            //sip
                            var activeContributionCategoryData = dbhq.tbl_CarumanTambahan.Where(x => x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_Deleted == false && x.fld_Default == true && x.fld_Warganegara == 1).ToList();

                            foreach (var activeContribution in activeContributionCategoryData)
                            {
                                var activeSubContributionCategoryData = db.tbl_SubCarumanTambahan.Where(x => x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_Deleted == false && x.fld_KodCaruman == activeContribution.fld_KodCaruman);
                                foreach (var activeSubContribution in activeSubContributionCategoryData)
                                {
                                    if (activeSubContribution.fld_UmurLower <= umurPekerja.Years && activeSubContribution.fld_UmurUpper >= umurPekerja.Years)
                                    {
                                        if (!dbr.tbl_PkjCarumanTambahan.Any(x => x.fld_Nopkj == nopkj && x.fld_KodCaruman == activeContribution.fld_KodCaruman && x.fld_KodSubCaruman == activeSubContribution.fld_KodSubCaruman && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_WilayahID == wlyhID && x.fld_LadangID == ldgID && x.fld_Deleted == false))
                                        {
                                            tbl_PkjCarumanTambahan pkjCarumanTambahan = new tbl_PkjCarumanTambahan();
                                            pkjCarumanTambahan.fld_Nopkj = nopkj;
                                            pkjCarumanTambahan.fld_KodCaruman = activeContribution.fld_KodCaruman;
                                            pkjCarumanTambahan.fld_KodSubCaruman = activeSubContribution.fld_KodSubCaruman;
                                            pkjCarumanTambahan.fld_NegaraID = ngraID;
                                            pkjCarumanTambahan.fld_SyarikatID = syrktID;
                                            pkjCarumanTambahan.fld_WilayahID = wlyhID;
                                            pkjCarumanTambahan.fld_LadangID = ldgID;
                                            pkjCarumanTambahan.fld_Deleted = false;
                                            dbr.tbl_PkjCarumanTambahan.Add(pkjCarumanTambahan);
                                            dbr.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        status = true;
                        //scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        //scope.Dispose();
                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    }
                    //}

                }
            }
            return Json(new { msg = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Rejectreason(int id)
        {
            ViewBag.Approval = "class = active";
            MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
            //ammended by mas on 29.05.2020 : tambah --> && x.fld_SyarikatID == 1
            var viewresult = dbhq.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "sbbTolak" && x.fldDeleted == false && x.fld_SyarikatID == 1);
            var result = dbhq.tblPkjmastApp.Where(x => x.fldID == id).FirstOrDefault();
            ViewBag.Nopkj = result.fldNoPkj;
            ViewBag.Nama = result.fldNama1;
            ViewBag.ID = result.fldID;
            //ViewBag.fldSbbTolak = sbbtolak;
            return PartialView(viewresult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rejectreason()
        {
            ViewBag.Approval = "class = active";
            MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
            string chksbbtolak = "";

            /*commented by kamalia on 13.11.2020

             chksbbtolak = Request.Form["ChkVal"];
             int id = int.Parse(Request.Form["idval"]);
             var pkjreject = dbhq.tblPkjmastApp.Where(x => x.fldID.Equals(id)).FirstOrDefault();
             pkjreject.fldSbbTolak = chksbbtolak;
             pkjreject.fldStatus = 0;
             //amended by mas on 29.05.2020
             //db kpd dbhq
             dbhq.SaveChanges();

              //commented by mas on 29.05.2020
             //return Json(new { success = true, msg = "Data successfully rejected.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "", data3 = "" });

             // return RedirectToAction("ApprovalNewWorkerOPMS"); */


            //added by kamalia on 13.11.2020
            if (ModelState.IsValid)
            {
                chksbbtolak = Request.Form["ChkVal"];
                int id = int.Parse(Request.Form["idval"]);
                var pkjreject = dbhq.tblPkjmastApp.Where(x => x.fldID.Equals(id)).FirstOrDefault();
                pkjreject.fldSbbTolak = chksbbtolak;
                pkjreject.fldStatus = 0;
                dbhq.SaveChanges();

                return Json(new { redirectTo = Url.Action("ApprovalNewWorkerOPMS", "Approval") }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        public ActionResult ApprovalTransferWorkerOPMS(int WilayahIDList = 0, int LadangIDList = 0)
        {
            ViewBag.Approval = "class = active";
            MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                WilayahIDList2 = new SelectList(dbhq.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                LadangIDList2 = new SelectList(dbhq.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                var resultreport = dbhq.tblASCApprovalFileDetails.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).OrderBy(o => o.fldWilayahID);
                return View("TransferWorker", resultreport);
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                WilayahIDList2 = new SelectList(dbhq.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList2 = new SelectList(dbhq.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                var resultreport = dbhq.tblASCApprovalFileDetails.Where(x => x.fldWilayahID == WilayahID && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).OrderBy(o => o.fldWilayahID);
                return View("TransferWorker", resultreport);
            }
            else
            {
                WilayahIDList2 = new SelectList(dbhq.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList2 = new SelectList(dbhq.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                var resultreport = dbhq.tblASCApprovalFileDetails.Where(x => x.fldWilayahID == WilayahID && x.fldLadangID == LadangID && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).OrderBy(o => o.fldWilayahID);
                ViewBag.resultcount = resultreport.Count();
                return View("TransferWorker", resultreport);
            }
        }

        public PartialViewResult ApprovalTransferWorkerOPMSDetail(int fileID)
        {
            ViewBag.Approval = "class = active";
            MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
            var result = dbhq.tblPkjmastApp.Where(x => x.fldFileID == fileID && x.fldStatus == 2 && x.fldSbbMsk == "PL");
            //var result = db.tblPkjmastApps.Where(x => x.fldFileID == fileID);
            ViewBag.Datacount = result.Count();
            return PartialView("NewWorkerDetail", result);
        }

        public JsonResult GetLadang(int WilayahID)
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
                    ladanglist = new SelectList(dbC.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(dbC.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        //fatin added - 11/10/2023
        public JsonResult GetWilayah(string SyarikatID)
        {
            List<SelectListItem> wilayahlist = new List<SelectListItem>();
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

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
                        wilayahlist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                        ladanglist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

                    }
                    else
                    {
                        wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID2 && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                        wilayahlist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); //fatin added - 16/10/2023
                        ladanglist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                    }
                }
            }

            if (SyarikatID2 == 2)
            {
                if (WilayahID == 0)
                {
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID.ToString() == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    wilayahlist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                    ladanglist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

                }
                else
                {
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID.ToString() == SyarikatID && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    wilayahlist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" })); //fatin added - 16/10/2023
                    ladanglist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                }
            }

            return Json(wilayahlist);
        }
        //end

        //fatin added - 03/08/2023
        public JsonResult GetLadang2(int WilayahID, string SyarikatList)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if (SyarikatID == 1)
            {
                if (getwilyah.GetAvailableWilayah(SyarikatID))
                {

                    if (WilayahID == 0)
                    {
                        ladanglist = new SelectList(dbC.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                        ladanglist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                    }
                    else
                    {
                        ladanglist = new SelectList(dbC.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false && x.fld_CostCentre == SyarikatList).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                        ladanglist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                    }
                }
            }

            if (SyarikatID == 2)
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID.ToString() == SyarikatList && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                    ladanglist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID.ToString() == SyarikatList && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                    ladanglist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
                }
            }

            return Json(ladanglist);
        }
        //end

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db2.Dispose();
                //db3.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}