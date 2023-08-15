using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Models;
using Microsoft.Ajax.Utilities;
using MVC_SYSTEM.ModelsCustom;
using Itenso.TimePeriod;
using MVC_SYSTEM.ModelsEstate;
using static MVC_SYSTEM.Class.GlobalFunction;
using tbl_CutiPeruntukan = MVC_SYSTEM.ModelsEstate.tbl_CutiPeruntukan;
using tbl_Ladang = MVC_SYSTEM.ModelsCorporate.tbl_Ladang;
using tbl_Pkjmast = MVC_SYSTEM.ModelsCorporate.tbl_Pkjmast;
using vw_CutiUmumLdg = MVC_SYSTEM.ModelsCorporate.vw_CutiUmumLdg;
using System.Data.Entity.Validation;
using System.Globalization;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.ModelsAPI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using MVC_SYSTEM.AuthModels;

//by farahin 
//integration SAP

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
    public class SAPIntegrationController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Models db3 = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
       
        private GetIdentity GetIdentity = new GetIdentity();
        private GetConfig GetConfig = new GetConfig();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        errorlog geterror = new errorlog();
        private GlobalFunction GlobalFunction = new GlobalFunction();
        GetWilayah getwilyah = new GetWilayah();

        // GET: SAPIntegration
        public ActionResult Index()
        {
            ViewBag.Maintenance = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.MaintenanceSubList = sublist;
            ViewBag.Maintenance = "class = active";
            ViewBag.MaintenanceList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "sap" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            db.Dispose();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string MaintenanceList, string MaintenanceSubList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (MaintenanceSubList != null)
            {
                return RedirectToAction(MaintenanceSubList, "SAPIntegration");
            }
            else
            {
                int maintenancelist = int.Parse(MaintenanceList);
                var action = db.tblMenuLists.Where(x => x.fld_ID == maintenancelist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "SAPIntegration");
            }
        }

        public JsonResult GetSubList(int ListID)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var findsub = db.tblMenuLists.Where(x => x.fld_ID == ListID).Select(s => s.fld_Sub).FirstOrDefault();
            List<SelectListItem> sublist = new List<SelectListItem>();
            if (findsub != null)
            {
                sublist = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == findsub && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
            }
            db.Dispose();
            return Json(sublist);
        }
        public JsonResult GetAktvt(string JnisAktvt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> PilihAktiviti = new List<SelectListItem>();

            var tbl_UpahAktiviti = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == JnisAktvt && x.fld_Deleted == false).ToList();

            PilihAktiviti = new SelectList(tbl_UpahAktiviti.OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt.Trim() + " - " + s.fld_Desc }), "Value", "Text").ToList();
            PilihAktiviti.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" }));

            return Json(new { PilihAktiviti });
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
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_LdgCode.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_LdgCode.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }
        public ActionResult _GLSAPIntegration(ModelsCorporate.tbl_SAPLog tbl_SAPLog, ModelsCorporate.tbl_GLSAP _glSAP)
        {

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            RETURN rtn_details = new RETURN();
            ZOPMSGL gl = new ZOPMSGL();

            string today = DateTime.Now.ToString("yyyyMMdd");
            string bukrs = "", saknr = "", txt50 = "", xloeb = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";
            string exception = "";

            var oClient = new SAPMD.ZWS_OPMS_MASTERClient();
            //var oResponse = new SAPMD.ZfmOpmsMasterResponse();
            //var oRequest = new SAPMD.ZfmOpmsMasterRequest();
            var request = new SAPMD.ZfmOpmsMaster();

            //SAPMD.ZfmOpmsMasterResponse[] response = new SAPMD.ZfmOpmsMasterResponse[1];
            SAPMD.ZfmOpmsMasterResponse iresponse = new SAPMD.ZfmOpmsMasterResponse();

            SAPMD.Zopmsgl[] zopmsgl = new SAPMD.Zopmsgl[1];
            SAPMD.Zopmsgl zopmsgls = new SAPMD.Zopmsgl();

            SAPMD.Bapiret2[] bapirtn = new SAPMD.Bapiret2[1];
            SAPMD.Bapiret2 bapiret2_return = new SAPMD.Bapiret2();

            List<ZOPMSGL> glAmount = new List<ZOPMSGL>();

            oClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            oClient.ClientCredentials.UserName.Password = "@12345bnm";

            oClient.Open();

            try
            {

                request = new SAPMD.ZfmOpmsMaster();

                request.DateBegin = today;
                request.DateEnd = today;
                request.GlBegin = "ALL";
                request.GlEnd = "";
                request.GlComp = "1000";
                request.ItGl = zopmsgl;

                //request.DateBegin = "";
                //request.DateEnd = "";
                //request.GlBegin = "0070000010";
                //request.GlEnd = "0070000013";
                //request.GlComp = "1000";
                //request.ItGl = zopmsgl;

                iresponse = oClient.ZfmOpmsMaster(request);

                zopmsgl = iresponse.ItGl;
                bapirtn = iresponse.Return;

                if (iresponse.ItGl.Count() - 1 >= 0)
                {

                    foreach (SAPMD.Zopmsgl a in zopmsgl)
                    {
                        bukrs = a.Bukrs;
                        saknr = a.Saknr;
                        txt50 = a.Txt50;
                        xloeb = a.Xloeb;

                        var getGLDetails = db.tbl_GLSAP.Where(x => x.fld_GLcode == saknr && x.fld_Desc == txt50).FirstOrDefault();
                        var glCode = db.tbl_GLSAP.Where(x => x.fld_GLcode == saknr).Select(s => s.fld_GLcode).FirstOrDefault();
                        var gldesc = db.tbl_GLSAP.Where(x => x.fld_GLcode == saknr).Select(s => s.fld_Desc).FirstOrDefault();


                        if (getGLDetails == null)
                        {
                            if (glCode == null)
                            {
                                _glSAP = new tbl_GLSAP();

                                _glSAP.fld_GLcode = saknr;
                                _glSAP.fld_Desc = txt50;
                                _glSAP.fld_NegaraID = NegaraID;
                                _glSAP.fld_SyarikatID = SyarikatID;
                                _glSAP.fld_DTCreated = DateTime.Today;
                                _glSAP.fld_DTModified = DateTime.Today;
                                _glSAP.fld_CreatedBy = User.Identity.Name;
                                _glSAP.fld_CompanyCode = bukrs;

                                if (xloeb == "")
                                {
                                    _glSAP.fld_Deleted = false;
                                }
                                else
                                {
                                    _glSAP.fld_Deleted = true;
                                };

                                db.tbl_GLSAP.Add(_glSAP);
                                db.SaveChanges();
                                db.Entry(_glSAP).State = EntityState.Detached;
                            }
                            else if (glCode != null && gldesc == null)
                            {

                                ModelsCorporate.tbl_GLSAP getGL = db.tbl_GLSAP
                                            .Single(x => x.fld_GLcode == saknr);

                                getGL.fld_Desc = txt50;
                                getGL.fld_DTModified = DateTime.Today;
                                getGL.fld_ModifiedBy = User.Identity.Name;

                                if (xloeb == "")
                                {
                                    getGL.fld_Deleted = false;
                                }
                                else
                                {
                                    getGL.fld_Deleted = true;
                                };


                                db.SaveChanges();

                            }

                            //return if success since sap will return null to any success inbound
                            //save dlm db
                            tbl_SAPLog.fld_id = "";
                            tbl_SAPLog.fld_type = "S";
                            tbl_SAPLog.fld_message = "GL inbound success";
                            tbl_SAPLog.fld_msg1 = saknr;
                            tbl_SAPLog.fld_row = Convert.ToString(iresponse.ItGl.Count());
                            tbl_SAPLog.fld_system = "SAP GL";
                            tbl_SAPLog.fld_logDate = DateTime.Now;
                            tbl_SAPLog.fld_msg4 = User.Identity.Name;
                            tbl_SAPLog.fld_negaraID = "1";
                            tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                            db.tbl_SAPLog.Add(tbl_SAPLog);
                            db.SaveChanges();
                        }


                    }


                }

                if (iresponse.Return.Count() - 1 >= 0)
                {
                    foreach (SAPMD.Bapiret2 a in bapirtn)
                    {
                        type = a.Type;
                        id = a.Id;
                        number = a.Number;
                        logno = a.LogNo;
                        logmsgno = a.LogMsgNo;
                        message = a.Message;
                        message1 = a.MessageV1;
                        message2 = a.MessageV2;
                        message3 = a.MessageV3;
                        message4 = a.MessageV4;
                        parameter = a.Parameter;
                        row = a.Row.ToString();
                        field = a.Field;
                        system = a.System;

                        //save dlm db

                        tbl_SAPLog.fld_type = type;
                        tbl_SAPLog.fld_number = number;
                        tbl_SAPLog.fld_id = id;
                        tbl_SAPLog.fld_logno = logno;
                        tbl_SAPLog.fld_logmsgno = logmsgno;
                        tbl_SAPLog.fld_message = message;
                        tbl_SAPLog.fld_msg1 = message1;
                        tbl_SAPLog.fld_msg2 = message2;
                        tbl_SAPLog.fld_msg3 = message3;
                        tbl_SAPLog.fld_msg4 = message4;
                        tbl_SAPLog.fld_parameter = parameter;
                        tbl_SAPLog.fld_row = row;
                        tbl_SAPLog.fld_field = field;
                        tbl_SAPLog.fld_system = "SAP GL";

                        tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                        tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                        tbl_SAPLog.fld_logDate = DateTime.Now;

                        db.tbl_SAPLog.Add(tbl_SAPLog);
                        db.SaveChanges();
                    }

                }

            }
            catch (Exception ex)
            {
                throw (ex);

            }
            finally
            {
                oClient.Close();

            }

            return RedirectToAction("GLMaintenance", "SAPIntegration");


        }

        public ActionResult _CostCenterSAPIntegration(ModelsCorporate.tbl_SAPLog tbl_SAPLog, ModelsCorporate.tbl_CCSAP _ccSAP)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            string today = DateTime.Now.ToString("yyyyMMdd");
            string KOKRS = "", KOSTL = "", LTEXT = "", BKZKP = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";

            var oClient = new SAPMD.ZWS_OPMS_MASTERClient();
            var request = new SAPMD.ZfmOpmsMaster();
            SAPMD.ZfmOpmsMasterResponse iresponse = new SAPMD.ZfmOpmsMasterResponse();

            SAPMD.Zopmscc[] zopmscc = new SAPMD.Zopmscc[1];
            SAPMD.Zopmscc zopmsccs = new SAPMD.Zopmscc();

            SAPMD.Bapiret2[] bapirtn = new SAPMD.Bapiret2[1];
            SAPMD.Bapiret2 bapiret2_return = new SAPMD.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            oClient.ClientCredentials.UserName.Password = "@12345bnm";

            oClient.Open();

            try
            {
                request = new SAPMD.ZfmOpmsMaster();

                request.DateBegin = today;
                request.DateEnd = today;
                request.CcBegin = "ALL";
                request.CcEnd = "";
                request.CcComp = "1000";
                request.ItCc = zopmscc;

                iresponse = oClient.ZfmOpmsMaster(request);

                zopmscc = iresponse.ItCc;
                bapirtn = iresponse.Return;

                if (iresponse.ItCc.Count() - 1 >= 0)
                {

                    foreach (SAPMD.Zopmscc a in zopmscc)
                    {
                        KOKRS = a.Kokrs;
                        KOSTL = a.Kostl;
                        LTEXT = a.Ltext;
                        BKZKP = a.Bkzkp;

                        //save dlm db

                        //if glcode dah ade dlm db, tak yah save. kalau tak de baru save.
                        var getCCDetails = db.tbl_CCSAP.Where(x => x.fld_CstCnter == KOSTL && x.fld_Desc == LTEXT).FirstOrDefault();
                        var Code = db.tbl_CCSAP.Where(x => x.fld_CstCnter == KOSTL).Select(s => s.fld_CstCnter).FirstOrDefault();
                        var desc = db.tbl_CCSAP.Where(x => x.fld_CstCnter == KOSTL).Select(s => s.fld_Desc).FirstOrDefault();


                        if (getCCDetails == null)
                        {
                            if (Code == null)
                            {

                                _ccSAP = new ModelsCorporate.tbl_CCSAP();

                                _ccSAP.fld_CstCnter = KOSTL;
                                _ccSAP.fld_Desc = LTEXT;
                                _ccSAP.fld_NegaraID = NegaraID;
                                _ccSAP.fld_SyarikatID = SyarikatID;
                                _ccSAP.fld_DTCreated = DateTime.Today;
                                _ccSAP.fld_DTModified = DateTime.Today;
                                _ccSAP.fld_CreatedBy = User.Identity.Name;
                                _ccSAP.fld_CompanyCode = KOKRS;

                                if (BKZKP == "")
                                {
                                    _ccSAP.fld_Deleted = false;
                                }
                                else
                                {
                                    _ccSAP.fld_Deleted = true;
                                };

                                db.tbl_CCSAP.Add(_ccSAP);
                                db.SaveChanges();
                                db.Entry(_ccSAP).State = EntityState.Detached;
                            }

                            else if (Code != null && desc == null)
                            {
                                ModelsCorporate.tbl_CCSAP getCC = db.tbl_CCSAP
                                            .Single(x => x.fld_CstCnter == KOSTL);

                                getCC.fld_Desc = LTEXT;
                                getCC.fld_DTModified = DateTime.Today;
                                getCC.fld_ModifiedBy = User.Identity.Name;
                                if (BKZKP == "") { getCC.fld_Deleted = false; }
                                else { getCC.fld_Deleted = true; };

                                db.SaveChanges();
                            }
                            //return if success since sap will return null to any success inbound
                            //save dlm db

                            tbl_SAPLog.fld_type = "S";
                            tbl_SAPLog.fld_message = "CC inbound success";
                            tbl_SAPLog.fld_msg1 = KOSTL;
                            tbl_SAPLog.fld_row = Convert.ToString(iresponse.ItCc.Count());
                            tbl_SAPLog.fld_system = "SAP CC";
                            tbl_SAPLog.fld_logDate = DateTime.Now;
                            tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                            tbl_SAPLog.fld_negaraID = "1";
                            tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                            db.tbl_SAPLog.Add(tbl_SAPLog);
                            db.SaveChanges();
                        }
                    }


                }

                if (iresponse.Return.Count() - 1 >= 0)
                {
                    foreach (SAPMD.Bapiret2 a in bapirtn)
                    {
                        type = a.Type;
                        id = a.Id;
                        number = a.Number;
                        logno = a.LogNo;
                        logmsgno = a.LogMsgNo;
                        message = a.Message;
                        message1 = a.MessageV1;
                        message2 = a.MessageV2;
                        message3 = a.MessageV3;
                        message4 = a.MessageV4;
                        parameter = a.Parameter;
                        row = a.Row.ToString();
                        field = a.Field;
                        system = a.System;

                        //save dlm db

                        tbl_SAPLog.fld_type = type;
                        tbl_SAPLog.fld_number = number;
                        tbl_SAPLog.fld_id = id;
                        tbl_SAPLog.fld_logno = logno;
                        tbl_SAPLog.fld_logmsgno = logmsgno;
                        tbl_SAPLog.fld_message = message;
                        tbl_SAPLog.fld_msg1 = message1;
                        tbl_SAPLog.fld_msg2 = message2;
                        tbl_SAPLog.fld_msg3 = message3;
                        tbl_SAPLog.fld_msg4 = message4;
                        tbl_SAPLog.fld_parameter = parameter;
                        tbl_SAPLog.fld_row = row;
                        tbl_SAPLog.fld_field = field;
                        tbl_SAPLog.fld_system = "SAP CC";

                        tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                        tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                        tbl_SAPLog.fld_logDate = DateTime.Now;

                        db.tbl_SAPLog.Add(tbl_SAPLog);
                        db.SaveChanges();
                    }

                }

            }

            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                oClient.Close();
            }

            return RedirectToAction("CostCenterMaintenance", "SAPIntegration");
        }

        public ActionResult _CustSAPIntegration(ModelsCorporate.tbl_SAPLog tbl_SAPLog, ModelsCorporate.tbl_CMSAP _custSAP)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            string today = DateTime.Now.ToString("yyyyMMdd");
            string bukrs = "", kunnr = "", name1 = "", loevm = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";

            var oClient = new SAPMD.ZWS_OPMS_MASTERClient();
            var request = new SAPMD.ZfmOpmsMaster();
            SAPMD.ZfmOpmsMasterResponse iresponse = new SAPMD.ZfmOpmsMasterResponse();

            SAPMD.Zopmscs[] zopmscs = new SAPMD.Zopmscs[1];
            SAPMD.Zopmscs zopmscss = new SAPMD.Zopmscs();

            SAPMD.Bapiret2[] bapirtn = new SAPMD.Bapiret2[1];
            SAPMD.Bapiret2 bapiret2_return = new SAPMD.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            oClient.ClientCredentials.UserName.Password = "@12345bnm";

            oClient.Open();

            try
            {
                request = new SAPMD.ZfmOpmsMaster();

                request.DateBegin = today;
                request.DateEnd = today;
                request.CsBegin = "ALL";
                request.CsEnd = "";
                request.CsComp = "1000";
                request.ItCust = zopmscs;

                iresponse = oClient.ZfmOpmsMaster(request);

                zopmscs = iresponse.ItCust;
                bapirtn = iresponse.Return;

                if (iresponse.ItCust.Count() - 1 > 0)
                {

                    foreach (SAPMD.Zopmscs a in zopmscs)
                    {
                        bukrs = a.Bukrs;
                        kunnr = a.Kunnr;
                        name1 = a.Name1;
                        loevm = a.Loevm;

                        //save dlm db

                        //if glcode dah ade dlm db, tak yah save. kalau tak de baru save.
                        var getCMDetails = db.tbl_CMSAP.Where(x => x.fld_CustomerNo == kunnr && x.fld_Desc == name1).FirstOrDefault();
                        var cmCode = db.tbl_CMSAP.Where(x => x.fld_CustomerNo == kunnr).Select(s => s.fld_CustomerNo).FirstOrDefault();
                        var cmDesc = db.tbl_CMSAP.Where(x => x.fld_CustomerNo == kunnr).Select(s => s.fld_Desc).FirstOrDefault();


                        if (getCMDetails == null)
                        {
                            if (cmCode == null)
                            {

                                _custSAP = new ModelsCorporate.tbl_CMSAP();

                                _custSAP.fld_CustomerNo = kunnr;
                                _custSAP.fld_Desc = name1;
                                _custSAP.fld_NegaraID = NegaraID;
                                _custSAP.fld_SyarikatID = SyarikatID;
                                _custSAP.fld_DTCreated = DateTime.Now;
                                _custSAP.fld_DTModified = DateTime.Now;
                                _custSAP.fld_CompanyCode = bukrs;
                                _custSAP.fld_CreatedBy = User.Identity.Name;

                                if (loevm == "")
                                {
                                    _custSAP.fld_Deleted = false;
                                }
                                else
                                {
                                    _custSAP.fld_Deleted = true;
                                };

                                db.tbl_CMSAP.Add(_custSAP);
                                db.SaveChanges();
                                db.Entry(_custSAP).State = EntityState.Detached;
                            }

                            else if (cmCode != null && cmDesc == null)
                            {
                                ModelsCorporate.tbl_CMSAP getCM = db.tbl_CMSAP
                                            .Single(x => x.fld_CustomerNo == kunnr);

                                getCM.fld_Desc = name1;
                                getCM.fld_DTModified = DateTime.Now;
                                getCM.fld_ModifiedBy = User.Identity.Name;
                                if (loevm == "")
                                { getCM.fld_Deleted = false; }
                                else
                                { getCM.fld_Deleted = true; };

                                db.SaveChanges();
                            }
                            //return if success since sap will return null to any success inbound
                            //save dlm db

                            tbl_SAPLog.fld_type = "S";
                            tbl_SAPLog.fld_message = "Customer inbound success";
                            tbl_SAPLog.fld_msg1 = kunnr;
                            tbl_SAPLog.fld_row = Convert.ToString(iresponse.ItCust.Count());
                            tbl_SAPLog.fld_system = "SAP CM";
                            tbl_SAPLog.fld_logDate = DateTime.Now;
                            tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                            tbl_SAPLog.fld_negaraID = "1";
                            tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                            db.tbl_SAPLog.Add(tbl_SAPLog);
                            db.SaveChanges();
                        }
                    }

                }

                if (iresponse.Return.Count() - 1 >= 0)
                {
                    foreach (SAPMD.Bapiret2 a in bapirtn)
                    {
                        type = a.Type;
                        id = a.Id;
                        number = a.Number;
                        logno = a.LogNo;
                        logmsgno = a.LogMsgNo;
                        message = a.Message;
                        message1 = a.MessageV1;
                        message2 = a.MessageV2;
                        message3 = a.MessageV3;
                        message4 = a.MessageV4;
                        parameter = a.Parameter;
                        row = a.Row.ToString();
                        field = a.Field;
                        system = a.System;

                        //save dlm db

                        tbl_SAPLog.fld_type = type;
                        tbl_SAPLog.fld_number = number;
                        tbl_SAPLog.fld_id = id;
                        tbl_SAPLog.fld_logno = logno;
                        tbl_SAPLog.fld_logmsgno = logmsgno;
                        tbl_SAPLog.fld_message = message;
                        tbl_SAPLog.fld_msg1 = message1;
                        tbl_SAPLog.fld_msg2 = message2;
                        tbl_SAPLog.fld_msg3 = message3;
                        tbl_SAPLog.fld_msg4 = message4;
                        tbl_SAPLog.fld_parameter = parameter;
                        tbl_SAPLog.fld_row = row;
                        tbl_SAPLog.fld_field = field;
                        tbl_SAPLog.fld_system = "SAP CM";

                        tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                        tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                        tbl_SAPLog.fld_logDate = DateTime.Now;

                        db.tbl_SAPLog.Add(tbl_SAPLog);
                        db.SaveChanges();
                    }

                }

            }

            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                oClient.Close();
            }

            return RedirectToAction("CustomerDetail", "SAPIntegration");
        }


        public ActionResult _VendorSAPIntegration(ModelsCorporate.tbl_SAPLog tbl_SAPLog, ModelsCorporate.tbl_VDSAP _vdSAP)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            string today = DateTime.Now.ToString("yyyyMMdd");
            string bukrs = "", lifnr = "", name1 = "", loevm = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";

            var oClient = new SAPMD.ZWS_OPMS_MASTERClient();
            var request = new SAPMD.ZfmOpmsMaster();
            SAPMD.ZfmOpmsMasterResponse iresponse = new SAPMD.ZfmOpmsMasterResponse();

            SAPMD.Zopmsvd[] zopmsvd = new SAPMD.Zopmsvd[1];
            SAPMD.Zopmsvd zopmsvds = new SAPMD.Zopmsvd();

            SAPMD.Bapiret2[] bapirtn = new SAPMD.Bapiret2[1];
            SAPMD.Bapiret2 bapiret2_return = new SAPMD.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            oClient.ClientCredentials.UserName.Password = "@12345bnm";

            oClient.Open();

            try
            {
                request = new SAPMD.ZfmOpmsMaster();

                request.DateBegin = today;
                request.DateEnd = today;
                request.VdBegin = "ALL";
                request.VdEnd = "";
                request.VdComp = "1000";
                request.ItVend = zopmsvd;

                iresponse = oClient.ZfmOpmsMaster(request);

                zopmsvd = iresponse.ItVend;
                bapirtn = iresponse.Return;

                if (iresponse.ItVend.Count() - 1 >= 0)
                {

                    foreach (SAPMD.Zopmsvd a in zopmsvd)
                    {
                        bukrs = a.Bukrs;
                        lifnr = a.Lifnr;
                        name1 = a.Name1;
                        loevm = a.Loevm;

                        //save dlm db
                        //if glcode dah ade dlm db, tak yah save. kalau tak de baru save.
                        var getVDDetails = db.tbl_VDSAP.Where(x => x.fld_VendorNo == lifnr && x.fld_Desc == name1).FirstOrDefault();
                        var vdCode = db.tbl_VDSAP.Where(x => x.fld_VendorNo == lifnr).Select(s => s.fld_VendorNo).FirstOrDefault();
                        var vdDesc = db.tbl_VDSAP.Where(x => x.fld_VendorNo == lifnr).Select(s => s.fld_Desc).FirstOrDefault();

                        if (getVDDetails == null)
                        {
                            if (vdCode == null)
                            {

                                _vdSAP = new ModelsCorporate.tbl_VDSAP();

                                _vdSAP.fld_VendorNo = lifnr;
                                _vdSAP.fld_Desc = name1;
                                _vdSAP.fld_NegaraID = NegaraID;
                                _vdSAP.fld_SyarikatID = SyarikatID;
                                _vdSAP.fld_DTCreated = DateTime.Now;
                                _vdSAP.fld_DTModified = DateTime.Now;
                                _vdSAP.fld_CreatedBy = User.Identity.Name;
                                _vdSAP.fld_CompanyCode = bukrs;

                                if (loevm == "")
                                {
                                    _vdSAP.fld_Deleted = false;
                                }
                                else
                                {
                                    _vdSAP.fld_Deleted = true;
                                };

                                db.tbl_VDSAP.Add(_vdSAP);
                                db.SaveChanges();
                                db.Entry(_vdSAP).State = EntityState.Detached;
                            }

                            else if (vdCode != null && vdDesc == null)
                            {
                                ModelsCorporate.tbl_VDSAP getVD = db.tbl_VDSAP
                                            .Single(x => x.fld_VendorNo == lifnr);

                                getVD.fld_Desc = name1;
                                getVD.fld_DTModified = DateTime.Now;
                                getVD.fld_ModifiedBy = User.Identity.Name;

                                if (loevm == "")
                                { getVD.fld_Deleted = false; }
                                else
                                { getVD.fld_Deleted = true; };

                                db.SaveChanges();
                            }

                            //return if success since sap will return null to any success inbound
                            //save dlm db

                            tbl_SAPLog.fld_type = "S";
                            tbl_SAPLog.fld_message = "Vendor inbound success";
                            tbl_SAPLog.fld_msg1 = lifnr;
                            tbl_SAPLog.fld_row = Convert.ToString(iresponse.ItVend.Count());
                            tbl_SAPLog.fld_system = "SAP VD";
                            tbl_SAPLog.fld_logDate = DateTime.Now;
                            tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                            tbl_SAPLog.fld_negaraID = "1";
                            tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                            db.tbl_SAPLog.Add(tbl_SAPLog);
                            db.SaveChanges();
                        }
                    }

                }


                if (iresponse.Return.Count() - 1 >= 0)
                {
                    foreach (SAPMD.Bapiret2 a in bapirtn)
                    {
                        type = a.Type;
                        id = a.Id;
                        number = a.Number;
                        logno = a.LogNo;
                        logmsgno = a.LogMsgNo;
                        message = a.Message;
                        message1 = a.MessageV1;
                        message2 = a.MessageV2;
                        message3 = a.MessageV3;
                        message4 = a.MessageV4;
                        parameter = a.Parameter;
                        row = a.Row.ToString();
                        field = a.Field;
                        system = a.System;

                        //save dlm db

                        tbl_SAPLog.fld_type = type;
                        tbl_SAPLog.fld_number = number;
                        tbl_SAPLog.fld_id = id;
                        tbl_SAPLog.fld_logno = logno;
                        tbl_SAPLog.fld_logmsgno = logmsgno;
                        tbl_SAPLog.fld_message = message;
                        tbl_SAPLog.fld_msg1 = message1;
                        tbl_SAPLog.fld_msg2 = message2;
                        tbl_SAPLog.fld_msg3 = message3;
                        tbl_SAPLog.fld_msg4 = message4;
                        tbl_SAPLog.fld_parameter = parameter;
                        tbl_SAPLog.fld_row = row;
                        tbl_SAPLog.fld_field = field;
                        tbl_SAPLog.fld_system = "SAP VD";

                        tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                        tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                        tbl_SAPLog.fld_logDate = DateTime.Now;

                        db.tbl_SAPLog.Add(tbl_SAPLog);
                        db.SaveChanges();
                    }

                }

            }

            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                oClient.Close();
            }

            return RedirectToAction("SupplierDetail", "SAPIntegration");
        }

      
        //GL Mapping
        public ActionResult GLMaintenance(string filter, int page = 1, string sort = "fld_KodGL",
             string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.Maintenance = "class = active";

            return View();
        }

        public ActionResult _GLMaintenance(string filter, int page = 1,
            string sort = "fld_KodGL",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsCorporate.tbl_MapGL>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var glData = db.tbl_MapGL
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = glData
                    .Where(x => x.fld_KodGL.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodAktvt.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = glData
                    .Count(x => x.fld_KodGL.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodAktvt.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = glData.OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = glData
                    .Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;

            return View(records);
        }


       
        public ActionResult _GLMaintenanceCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            try
            {
                var jnsxtivlist = db.tbl_JenisAktiviti
                    .Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodJnsAktvt).ToList();

                ViewBag.JnisAktvt = new SelectList(
                    jnsxtivlist
                    .Select(s => new SelectListItem { Value = s.fld_KodJnsAktvt, Text = s.fld_Desc }), "Value", "Text").ToList();

                var fistxtivit = jnsxtivlist.Select(s => s.fld_KodJnsAktvt).Take(1).FirstOrDefault();
                ViewBag.KodAktvtList = new SelectList(
                    db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == fistxtivit && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt)
                    .Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt.Trim() + " - " + s.fld_Desc }), "Value", "Text").ToList();

                ViewBag.JnsLotList = new SelectList(
                    db.tblOptionConfigsWebs
                        .Where(x => x.fldOptConfFlag1 == "jnsLot" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
                        .Select(
                            s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                ViewBag.PaysheetList = new SelectList(
                    db.tblOptionConfigsWebs
                        .Where(x => x.fldOptConfFlag1 == "jnsPaysheet" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
                        .Select(
                            s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                ViewBag.KodGLList = new SelectList(
                    db.tbl_GLSAP
                        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_GLcode)
                        .Select(
                            s => new SelectListItem { Value = s.fld_GLcode, Text = s.fld_GLcode }), "Value", "Text").ToList();

                return PartialView();
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


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _GLMaintenanceCreate(ModelsCorporate.tbl_MapGL GlMaintenance, string KodAktvtList, string PaysheetList, string KodGLList, string JnsLotList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            try
            {
                var checkdata = db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                    x.fld_KodAktvt == KodAktvtList && x.fld_KodGL == KodGLList && x.fld_JnsLot == JnsLotList &&
                                                    x.fld_Paysheet == PaysheetList && x.fld_Deleted == false).FirstOrDefault();
                if (checkdata == null)
                {
                    GlMaintenance.fld_KodAktvt = KodAktvtList;
                    GlMaintenance.fld_KodGL = KodGLList;
                    GlMaintenance.fld_Paysheet = PaysheetList;
                    GlMaintenance.fld_JnsLot = JnsLotList;
                    GlMaintenance.fld_Deleted = false;
                    GlMaintenance.fld_NegaraID = NegaraID;
                    GlMaintenance.fld_SyarikatID = SyarikatID;
                    GlMaintenance.fld_WilayahID = WilayahID;
                    GlMaintenance.fld_LadangID = LadangID;

                    db.tbl_MapGL.Add(GlMaintenance);
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
                        msg = GlobalResCorp.msgAdd,
                        status = "success",
                        checkingdata = "0",
                        method = "1",
                        div = "GLMaintenanceDetails",
                        rootUrl = domain,
                        action = "_GLMaintenance",
                        controller = "SAPIntegration"
                    });
                }
                else
                {
                    return Json(new { success = true, msg = GlobalResCorp.msgDataExist, status = "warning", checkingdata = "1" });
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

        public ActionResult _GLMaintenanceEdit(int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);



            var glData = db.tbl_MapGL
                .SingleOrDefault(x => x.fld_ID == id &&
                                    x.fld_Deleted == false);

            var JnsAktivitiData = db.tbl_UpahAktiviti
                .SingleOrDefault(x => x.fld_KodAktvt == glData.fld_KodAktvt.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);


            List<SelectListItem> JnisAktvt = new List<SelectListItem>();
            JnisAktvt = new SelectList(
                db.tbl_JenisAktiviti
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodJnsAktvt)
                    .Select(
                        s => new SelectListItem { Value = s.fld_KodJnsAktvt.ToString(), Text = s.fld_Desc }), "Value", "Text", JnsAktivitiData.fld_KodJenisAktvt).ToList();
            JnisAktvt.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.fld_KodJnsAktvt = JnisAktvt;


            List<SelectListItem> kodAktvtList = new List<SelectListItem>();
            kodAktvtList = new SelectList(
                db.tbl_UpahAktiviti
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt)
                    .Select(
                        s => new SelectListItem { Value = s.fld_KodAktvt.ToString(), Text = s.fld_KodAktvt.Trim() + " - " + s.fld_Desc }), "Value", "Text", glData.fld_KodAktvt.Trim()).ToList();
            kodAktvtList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.fld_KodAktvt = kodAktvtList;



            List<SelectListItem> JnsLot = new List<SelectListItem>();
            JnsLot = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jnsLot" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
                    .Select(
                           s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", glData.fld_JnsLot.Trim()).ToList();
            JnsLot.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.fld_JnsLot = JnsLot;

            List<SelectListItem> paysheetList = new List<SelectListItem>();
            paysheetList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jnsPaysheet" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
                    .Select(
                           s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", glData.fld_Paysheet.Trim()).ToList();
            paysheetList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.fld_Paysheet = paysheetList;


            List<SelectListItem> kodGLList = new List<SelectListItem>();
            kodGLList = new SelectList(
                db.tbl_GLSAP
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_GLcode)
                    .Select(
                        s => new SelectListItem { Value = s.fld_GLcode, Text = s.fld_GLcode }), "Value", "Text", glData.fld_KodGL.Trim()).ToList();
            kodGLList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.fld_KodGL = kodGLList;


            return PartialView(glData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _GLMaintenanceEdit(ModelsCorporate.tbl_MapGL gl)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            try
            {
                var checkdata = db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                    x.fld_KodAktvt == gl.fld_KodAktvt && x.fld_KodGL == gl.fld_KodGL && x.fld_JnsLot == gl.fld_JnsLot &&
                                                    x.fld_Paysheet == gl.fld_Paysheet && x.fld_Deleted == false).FirstOrDefault();
                if (checkdata == null)
                {
                    ModelsCorporate.tbl_MapGL getGL = db.tbl_MapGL
                        .Single(x => x.fld_ID == gl.fld_ID &&
                                     x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

                    getGL.fld_KodAktvt = gl.fld_KodAktvt;
                    getGL.fld_JnsLot = gl.fld_JnsLot;
                    getGL.fld_KodGL = gl.fld_KodGL;
                    getGL.fld_Paysheet = gl.fld_Paysheet;


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
                        div = "GLMaintenanceDetails",
                        rootUrl = domain,
                        action = "_GLMaintenance",
                        controller = "SAPIntegration"
                    });
                }
                else
                {
                    return Json(new { success = true, msg = GlobalResCorp.msgDataExist, status = "warning", checkingdata = "1" });
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

        public ActionResult _GLMaintenanceDelete(int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            var GlData = db.tbl_MapGL
                .SingleOrDefault(x => x.fld_ID == id && x.fld_NegaraID == NegaraID &&
                                      x.fld_SyarikatID == SyarikatID);

            return PartialView(GlData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _GLMaintenanceDelete(ModelsCorporate.tbl_MapGL gL)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            try
            {
                var GlData = db.tbl_MapGL.SingleOrDefault(
                    x => x.fld_ID == gL.fld_ID &&
                         x.fld_NegaraID == NegaraID &&
                         x.fld_SyarikatID == SyarikatID);

                bool status = true;

                var message = "";
                if (GlData.fld_Deleted == false)
                {
                    status = true;
                    message = GlobalResCorp.msgDelete2;
                }

                else
                {
                    status = false;
                    message = GlobalResCorp.msgUndelete;
                }

                GlData.fld_Deleted = status;

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
                    div = "GLMaintenanceDetails",
                    rootUrl = domain,
                    action = "_GLMaintenance",
                    controller = "SAPIntegration"
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

        //Cost Centre Mapping


        public ActionResult CostCenterMaintenance(string filter = "", int fldUserID = 0, int page = 1, string sort = "fld_DTModified", string sortdir = "DESC", int id = 0)
        {
            //load page pembekal
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            ViewBag.User = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_CCSAP>();
            ViewBag.filter = filter;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (filter == "")
            {
                if (GetIdentity.MySuperAdmin(User.Identity.Name))
                {
                    records.Content = dbview.tbl_CCSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_CCSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_CCSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_CCSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            else
            {
                if (GetIdentity.MySuperAdmin(User.Identity.Name))
                {
                    records.Content = dbview.tbl_CCSAP.Where(x => (x.fld_CstCnter.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_CCSAP.Where(x => (x.fld_CstCnter.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_CCSAP.Where(x => (x.fld_CstCnter.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_CCSAP.Where(x => (x.fld_CstCnter.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            return View(records);
        }

        public ActionResult _CostCenterMaintenance(string filter/*, int page = 1, string sort = "fld_DTModified", string sortdir = "Desc"*/)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
            //    NegaraID.Value);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            
            int role = GetIdentity.RoleID(getuserid).Value;

            var Data = db.tbl_CCSAP
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            if (!String.IsNullOrEmpty(filter))
            {
               Data.Where(x => x.fld_CstCnter.Contains(filter.ToUpper())).ToList();
            }
            else
            {
                Data.OrderByDescending(o => o.fld_DTModified);
            }
                //    records = Data
                //        .Where(x => x.fld_CstCnter.ToUpper().Contains(filter.ToUpper()))
                //        .OrderBy(sort + " " + sortdir)
                //        .Skip((page - 1) * pageSize)
                //        .Take(pageSize)
                //        .ToList();

                //    records.TotalRecords = Data
                //        .Count(x => x.fld_CstCnter.ToUpper().Contains(filter.ToUpper()));
                //}

                //else
                //{
                //    records.Content = Data.OrderBy(sort + " " + sortdir)
                //        .Skip((page - 1) * pageSize)
                //        .Take(pageSize)
                //        .ToList();

                //    records.TotalRecords = Data
                //        .Count();
                //}

                //records.CurrentPage = page;
                //records.PageSize = pageSize;
                ViewBag.RoleID = role;
            ViewBag.pageSize = 1;

            return View(Data);
        }

        //public ActionResult _CostCenterMaintenanceCreate()
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

        //    List<SelectListItem> CostCentre = new List<SelectListItem>();

        //    CostCentre = new SelectList(
        //        db.tbl_CCSAP
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_CstCnter)
        //            .Select(
        //                s => new SelectListItem { Value = s.fld_CstCnter.ToString(), Text = s.fld_CstCnter.ToString() + " - " + s.fld_Desc }), "Value", "Text").ToList();
        //    //CostCentre.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

        //    ViewBag.CostCentre = CostCentre;

        //    List<SelectListItem> Wilayah = new List<SelectListItem>();

        //    Wilayah = new SelectList(
        //        db.tbl_Wilayah
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID)
        //            .Select(
        //                s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_ID.ToString() + " - " + s.fld_WlyhName }), "Value", "Text").ToList();
        //    //CostCentre.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

        //    ViewBag.Wilayah = Wilayah;

        //    List<SelectListItem> Ladang = new List<SelectListItem>();

        //    Ladang = new SelectList(
        //        db.tbl_Ladang
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID).OrderBy(o => o.fld_ID)
        //            .Select(
        //                s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_ID.ToString() + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //    //CostCentre.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });

        //    ViewBag.Ladang = Ladang;

        //    return PartialView();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult _CostCenterMaintenanceCreate(ModelsCorporate.tbl_CostCentre ccMaintenance)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

        //    try
        //    {
               
        //        var checkdata = db.tbl_CostCentre.Where(x => x.fld_CostCentre == ccMaintenance.fld_CostCentre && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();
        //        if (checkdata == null)
        //        {
        //            ccMaintenance.fld_CostCentre = ccMaintenance.fld_CostCentre;
        //            ccMaintenance.fld_Deleted = false;
        //            ccMaintenance.fld_NegaraID = NegaraID;
        //            ccMaintenance.fld_SyarikatID = SyarikatID;
        //            ccMaintenance.fld_WilayahID = WilayahID;
        //            ccMaintenance.fld_LadangID = LadangID;
        //            ccMaintenance.fld_DTCreated = timezone.gettimezone();

        //            db.tbl_CostCentre.Add(ccMaintenance);
        //            db.SaveChanges();

        //            string appname = Request.ApplicationPath;
        //            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
        //            var lang = Request.RequestContext.RouteData.Values["lang"];

        //            if (appname != "/")
        //            {
        //                domain = domain + appname;
        //            }

        //            return Json(new
        //            {
        //                success = true,
        //                msg = GlobalResCorp.msgAdd,
        //                status = "success",
        //                checkingdata = "0",
        //                method = "1",
        //                div = "CostCentMaintenanceDetails",
        //                rootUrl = domain,
        //                action = "_CostCenterMaintenance",
        //                controller = "SAPIntegration"
        //            });
        //        }
        //        else
        //        {
        //            return Json(new { success = true, msg = GlobalResCorp.msgDataExist, status = "warning", checkingdata = "1" });
        //        }


        //    }

        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        return Json(new
        //        {
        //            success = false,
        //            msg = GlobalResCorp.msgError,
        //            status = "danger",
        //            checkingdata = "0"
        //        });
        //    }

        //    finally
        //    {
        //        db.Dispose();
        //    }
        //}

        //public ActionResult _CostCenterMaintenanceEdit(int id)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

        //    var ccData = db.tbl_CostCentre
        //        .SingleOrDefault(x => x.fld_ID == id && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID);

        //    var CostCentreDetails = db.tbl_CCSAP.SingleOrDefault(x => x.fld_CstCnter == ccData.fld_CostCentre.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID);
            
        //    var WilayahDetail = db.tbl_Wilayah.SingleOrDefault(x => x.fld_ID == ccData.fld_WilayahID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID);

        //    var LadangDetail = db.tbl_Ladang.SingleOrDefault(x => x.fld_ID == ccData.fld_LadangID && x.fld_WlyhID == ccData.fld_WilayahID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID);

        //    List<SelectListItem> CostCentre = new List<SelectListItem>();
        //    CostCentre = new SelectList(
        //        db.tbl_CCSAP
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID)
        //            .Select(
        //                s => new SelectListItem { Value = s.fld_CstCnter.ToString(), Text = s.fld_CstCnter+" - " + s.fld_Desc }), "Value", "Text", CostCentreDetails.fld_CstCnter).ToList();
        //    CostCentre.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
        //    ViewBag.fld_CostCentre = CostCentre;

        //    List<SelectListItem> Wilayah = new List<SelectListItem>();
        //    Wilayah = new SelectList(
        //        db.tbl_Wilayah
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID)
        //            .Select(
        //                s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_ID + " - " + s.fld_WlyhName }), "Value", "Text", WilayahDetail.fld_ID).ToList();
        //    Wilayah.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
        //    ViewBag.fld_WilayahID = Wilayah;

        //    List<SelectListItem> Ladang = new List<SelectListItem>();
        //    Ladang = new SelectList(
        //        db.tbl_Ladang
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahDetail.fld_ID).OrderBy(o => o.fld_ID)
        //            .Select(
        //                s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_ID + " - " + s.fld_LdgName }), "Value", "Text", LadangDetail.fld_ID).ToList();
        //    Wilayah.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
        //    ViewBag.fld_LadangID = Ladang;

        //    return PartialView(ccData);

        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult _CostCenterMaintenanceEdit(ModelsCorporate.tbl_CCSAP cc)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

        //    try
        //    {
        //        var checkdata = db.tbl_CCSAP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
        //                                            x.fld_CstCnter == cc.fld_CstCnter && x.fld_Desc == cc.fld_Desc && x.fld_Deleted == false).FirstOrDefault();
        //        if (checkdata == null)
        //        {
        //            ModelsCorporate.tbl_CCSAP getCC = db.tbl_CCSAP
        //                .Single(x => x.fld_ID == cc.fld_ID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

        //            getCC.fld_CstCnter = cc.fld_CstCnter;
        //            getCC.fld_Desc = cc.fld_Desc;
        //            getCC.fld_DTModified = timezone.gettimezone();

        //            db.SaveChanges();

        //            string appname = Request.ApplicationPath;
        //            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
        //            var lang = Request.RequestContext.RouteData.Values["lang"];

        //            if (appname != "/")
        //            {
        //                domain = domain + appname;
        //            }

        //            return Json(new
        //            {
        //                success = true,
        //                msg = GlobalResCorp.msgUpdate,
        //                status = "success",
        //                checkingdata = "0",
        //                method = "1",
        //                div = "CostCentMaintenanceDetails",
        //                rootUrl = domain,
        //                action = "_CostCenterMaintenance",
        //                controller = "SAPIntegration"
        //            });
        //        }
        //        else
        //        {
        //            return Json(new { success = true, msg = GlobalResCorp.msgDataExist, status = "warning", checkingdata = "1" });
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        return Json(new
        //        {
        //            success = false,
        //            msg = GlobalResCorp.msgError,
        //            status = "danger",
        //            checkingdata = "0"
        //        });
        //    }

        //    finally
        //    {
        //        db.Dispose();
        //    }
        //}

        //public ActionResult _CostCenterMaintenanceDelete(int id)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

        //    var CcData = db.tbl_CCSAP
        //        .SingleOrDefault(x => x.fld_ID == id && x.fld_NegaraID == NegaraID &&
        //                              x.fld_SyarikatID == SyarikatID);

        //    return PartialView(CcData);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult _CostCenterMaintenanceDelete(ModelsCorporate.tbl_CCSAP cc)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

        //    try
        //    {
        //        var CcData = db.tbl_CCSAP.SingleOrDefault(
        //            x => x.fld_ID == cc.fld_ID &&
        //                 x.fld_NegaraID == NegaraID &&
        //                 x.fld_SyarikatID == SyarikatID);

        //        bool status = true;

        //        var message = "";
        //        if (CcData.fld_Deleted == false)
        //        {
        //            status = true;
        //            message = GlobalResCorp.msgDelete2;
        //        }

        //        else
        //        {
        //            status = false;
        //            message = GlobalResCorp.msgUndelete;
        //        }

        //        CcData.fld_Deleted = status;

        //        db.SaveChanges();

        //        string appname = Request.ApplicationPath;
        //        string domain = Request.Url.GetLeftPart(UriPartial.Authority);
        //        var lang = Request.RequestContext.RouteData.Values["lang"];

        //        if (appname != "/")
        //        {
        //            domain = domain + appname;
        //        }

        //        return Json(new
        //        {
        //            success = true,
        //            msg = GlobalResCorp.msgUpdate,
        //            status = "success",
        //            checkingdata = "0",
        //            method = "1",
        //            div = "CostCentMaintenanceDetails",
        //            rootUrl = domain,
        //            action = "_CostCenterMaintenance",
        //            controller = "SAPIntegration"
        //        });
        //    }

        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        return Json(new
        //        {
        //            success = false,
        //            msg = GlobalResCorp.msgError,
        //            status = "danger",
        //            checkingdata = "0"
        //        });
        //    }

        //    finally
        //    {
        //        db.Dispose();
        //    }
        //}

        public ActionResult SupplierDetail(string filter = "", int fldUserID = 0, int page = 1, string sort = "fld_DTModified", string sortdir = "ASC", int id = 0)
        {
            //load page pembekal
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            ViewBag.User = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_VDSAP>();
            ViewBag.filter = filter;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (filter == "")
            {
                if (GetIdentity.MySuperAdmin(User.Identity.Name))
                {
                    records.Content = dbview.tbl_VDSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_VDSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_VDSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_VDSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            else
            {
                if (GetIdentity.MySuperAdmin(User.Identity.Name))
                {
                    records.Content = dbview.tbl_VDSAP.Where(x => (x.fld_VendorNo.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_VDSAP.Where(x => (x.fld_VendorNo.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_VDSAP.Where(x => (x.fld_VendorNo.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_VDSAP.Where(x => (x.fld_VendorNo.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            return View(records);
        }

        //public ActionResult SupplierDetailInsert()
        //{
        //    //tmbh button

        //    int negaraid = 0;

        //    ViewBag.fld_NegaraID = new SelectList(db.tbl_Negara.Where(x => x.fld_Deleted == false), "fld_NegaraID", "fld_NamaNegara");
        //    negaraid = db.tbl_Negara.Where(x => x.fld_Deleted == false).Select(s => s.fld_NegaraID).Take(1).FirstOrDefault();
        //    ViewBag.fld_SyarikatID = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == negaraid && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");


        //    ViewBag.PembekalList = new SelectList(db.tbl_VDSAP.Where(x => x.fld_NegaraID == negaraid && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_VendorNo.ToString(), Text = s.fld_VendorNo + " - " + s.fld_Desc }), "Value", "Text").ToList();



        //    return PartialView("SupplierDetailInsert");
        //}


        public JsonResult GetPembekalList(int? fld_SyarikatID, int NegaraID)
        {

            List<SelectListItem> pembekallist = new List<SelectListItem>();


            pembekallist = new SelectList(db.tbl_VDSAP.Where(x => x.fld_SyarikatID == fld_SyarikatID).Select(s => new SelectListItem { Value = s.fld_VendorNo.ToString(), Text = s.fld_VendorNo + " - " + s.fld_Desc }), "Value", "Text").ToList();


            return Json(pembekallist, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SupplierDetailInsert(ModelsCorporate.tbl_Pembekal tbl_Pembekal, ModelsCorporate.tbl_VDSAP tbl_VDSAP)
        //{


        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var checkdata = db.tbl_Pembekal.Where(x => x.fld_KodPbkl == tbl_VDSAP.fld_VendorNo).FirstOrDefault();
        //            if (checkdata == null)
        //            {
        //                var namapmbkl = db.tbl_VDSAP.Where(x => x.fld_VendorNo == tbl_VDSAP.fld_VendorNo).Select(s => s.fld_Desc).FirstOrDefault();

        //                tbl_Pembekal.fld_KodPbkl = tbl_VDSAP.fld_VendorNo;
        //                tbl_Pembekal.fld_NamaPbkl = namapmbkl;
        //                tbl_Pembekal.fld_NegaraID = tbl_VDSAP.fld_NegaraID;
        //                tbl_Pembekal.fld_SyarikatID = tbl_VDSAP.fld_SyarikatID;
        //                tbl_Pembekal.fld_Deleted = false;
        //                db.tbl_Pembekal.Add(tbl_Pembekal);
        //                db.SaveChanges();
        //                var getid = db.tbl_Pembekal.Where(w => w.fld_KodPbkl == tbl_Pembekal.fld_KodPbkl).FirstOrDefault();
        //                return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tbl_Pembekal.fld_KodPbkl, data2 = tbl_Pembekal.fld_KodPbkl, data3 = "" });
        //            }
        //            else
        //            {
        //                return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
        //    }
        //}

        //public ActionResult SupplierDetailUpdate(int? id)
        //{


        //    if (id == null)
        //    {
        //        return RedirectToAction("SupplierDetail");
        //    }
        //    ModelsCorporate.tbl_Pembekal tbl_Pembekal = db.tbl_Pembekal.Find(id);
        //    if (tbl_Pembekal == null)
        //    {
        //        return RedirectToAction("SupplierDetail");
        //    }

        //    ViewBag.NegaraList = new SelectList(db.tbl_Negara.Where(x => x.fld_Deleted == false && x.fld_NegaraID == tbl_Pembekal.fld_NegaraID), "fld_NegaraID", "fld_NamaNegara");
        //    ViewBag.SyarikatList = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == tbl_Pembekal.fld_NegaraID && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");
        //    return PartialView("SupplierDetailUpdate", tbl_Pembekal);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SupplierDetailUpdate(int id, Models.tbl_Pembekal tbl_Pembekal)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var getdata = db.tbl_Pembekal.Find(id);
        //            //getdata.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
        //            getdata.fld_NamaPbkl = tbl_Pembekal.fld_NamaPbkl;

        //            db.Entry(getdata).State = EntityState.Modified;
        //            db.SaveChanges();
        //            var getid = id;
        //            return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
        //    }
        //}

        //public ActionResult SupplierDetailDelete(int? id)
        //{
        //    string negara = "";
        //    string syarikat = "";
        //    string pembekal = "";


        //    if (id == null)
        //    {
        //        return RedirectToAction("SupplierDetail");
        //    }
        //    ModelsCorporate.tbl_Pembekal tbl_Pembekal = db.tbl_Pembekal.Find(id);
        //    if (tbl_Pembekal == null)
        //    {
        //        return RedirectToAction("SupplierDetail");
        //    }
        //    else
        //    {
        //        negara = db.tbl_Negara.Where(x => x.fld_NegaraID == tbl_Pembekal.fld_NegaraID).Select(s => s.fld_NamaNegara).FirstOrDefault();
        //        syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == tbl_Pembekal.fld_SyarikatID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();

        //    }
        //    ViewBag.CountryName = negara;
        //    ViewBag.CompanyName = syarikat;
        //    return PartialView("SupplierDetailDelete", tbl_Pembekal);
        //}

        //[HttpPost, ActionName("SupplierDetailDelete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult SupplierDetailDeleteConfirmed(int id)
        //{
        //    try
        //    {
        //        ModelsCorporate.tbl_Pembekal tbl_Pembekal = db.tbl_Pembekal.Find(id);
        //        if (tbl_Pembekal == null)
        //        {
        //            return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
        //        }
        //        else
        //        {
        //            db.tbl_Pembekal.Remove(tbl_Pembekal);
        //            db.SaveChanges();
        //            return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
        //    }
        //}



        public ActionResult CustomerDetail(string filter = "", int fldUserID = 0, int page = 1, string sort = "fld_DTModified", string sortdir = "DESC", int id = 0)
        {
            //load page pembekal
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            ViewBag.User = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_CMSAP>();
            ViewBag.filter = filter;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (filter == "")
            {
                if (GetIdentity.MySuperAdmin(User.Identity.Name))
                {
                    records.Content = dbview.tbl_CMSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_CMSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_CMSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_CMSAP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            else
            {
                if (GetIdentity.MySuperAdmin(User.Identity.Name))
                {
                    records.Content = dbview.tbl_CMSAP.Where(x => (x.fld_CustomerNo.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_CMSAP.Where(x => (x.fld_CustomerNo.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_CMSAP.Where(x => (x.fld_CustomerNo.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_CMSAP.Where(x => (x.fld_CustomerNo.Contains(filter) || x.fld_Desc.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            return View(records);
        }

        //public ActionResult CustomerDetailInsert()
        //{
        //    //tmbh button
        //    int negaraid = 0;

        //    ViewBag.fld_NegaraID = new SelectList(db.tbl_Negara.Where(x => x.fld_Deleted == false), "fld_NegaraID", "fld_NamaNegara");
        //    negaraid = db.tbl_Negara.Where(x => x.fld_Deleted == false).Select(s => s.fld_NegaraID).Take(1).FirstOrDefault();
        //    ViewBag.fld_SyarikatID = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == negaraid && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");
        //    //     ViewBag.PembekalList = new SelectList(db.tbl_Pembekal.Where(x => x.fld_SyarikatIDx.fld_Deleted == false), "fld_KodPbkl", "fld_NamaPbkl").ToList();
        //    ViewBag.CustomerList = new SelectList(db.tbl_CMSAP.Where(x => x.fld_NegaraID == negaraid).Select(s => new SelectListItem { Value = s.fld_CustomerNo.ToString(), Text = s.fld_CustomerNo + " - " + s.fld_Desc }), "Value", "Text").ToList();

        //    return PartialView("CustomerDetailInsert");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CustomerDetailInsert(ModelsCorporate.tbl_Customer tbl_Customer, ModelsCorporate.tbl_CMSAP tbl_CMSAP)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        try
        //        {
        //            var checkdata = db.tbl_Customer.Where(x => x.fld_KodCstmr == tbl_CMSAP.fld_CustomerNo).FirstOrDefault();
        //            if (checkdata == null)
        //            {
        //                var namacstmr = db.tbl_CMSAP.Where(x => x.fld_CustomerNo == tbl_CMSAP.fld_CustomerNo).Select(s => s.fld_Desc).FirstOrDefault();

        //                tbl_Customer.fld_KodCstmr = tbl_CMSAP.fld_CustomerNo;
        //                tbl_Customer.fld_NamaCstmr = namacstmr;
        //                tbl_Customer.fld_NegaraID = tbl_CMSAP.fld_NegaraID;
        //                tbl_Customer.fld_SyarikatID = tbl_CMSAP.fld_SyarikatID;
        //                tbl_Customer.fld_Deleted = false;
        //                db.tbl_Customer.Add(tbl_Customer);
        //                db.SaveChanges();
        //                var getid = db.tbl_Customer.Where(w => w.fld_KodCstmr == tbl_Customer.fld_KodCstmr).FirstOrDefault();
        //                return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tbl_Customer.fld_KodCstmr, data2 = tbl_Customer.fld_KodCstmr, data3 = "" });
        //            }
        //            else
        //            {
        //                return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
        //    }
        //}

        //public ActionResult CustomerDetailUpdate(int? id)
        //{


        //    if (id == null)
        //    {
        //        return RedirectToAction("CustomerDetail");
        //    }
        //    ModelsCorporate.tbl_Customer tbl_Customer = db.tbl_Customer.Find(id);
        //    if (tbl_Customer == null)
        //    {
        //        return RedirectToAction("CustomerDetail");
        //    }

        //    ViewBag.NegaraList = new SelectList(db.tbl_Negara.Where(x => x.fld_Deleted == false && x.fld_NegaraID == tbl_Customer.fld_NegaraID), "fld_NegaraID", "fld_NamaNegara");
        //    ViewBag.SyarikatList = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == tbl_Customer.fld_NegaraID && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");
        //    return PartialView("CustomerDetailUpdate", tbl_Customer);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CustomerDetailUpdate(int id, ModelsCorporate.tbl_Customer tbl_Customer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var getdata = db.tbl_Customer.Find(id);
        //            //getdata.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
        //            getdata.fld_NamaCstmr = tbl_Customer.fld_NamaCstmr;

        //            db.Entry(getdata).State = EntityState.Modified;
        //            db.SaveChanges();
        //            var getid = id;
        //            return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
        //    }
        //}

        //public ActionResult CustomerDetailDelete(int? id)
        //{
        //    string negara = "";
        //    string syarikat = "";
        //    string pembekal = "";


        //    if (id == null)
        //    {
        //        return RedirectToAction("CustomerDetail");
        //    }
        //    ModelsCorporate.tbl_Customer tbl_Customer = db.tbl_Customer.Find(id);
        //    if (tbl_Customer == null)
        //    {
        //        return RedirectToAction("CustomerDetail");
        //    }
        //    else
        //    {
        //        negara = db.tbl_Negara.Where(x => x.fld_NegaraID == tbl_Customer.fld_NegaraID).Select(s => s.fld_NamaNegara).FirstOrDefault();
        //        syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == tbl_Customer.fld_SyarikatID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();

        //    }
        //    ViewBag.CountryName = negara;
        //    ViewBag.CompanyName = syarikat;
        //    return PartialView("CustomerDetailDelete", tbl_Customer);
        //}

        //[HttpPost, ActionName("CustomerDetailDelete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult CustomerDetailDeleteConfirmed(int id)
        //{
        //    try
        //    {
        //        ModelsCorporate.tbl_Customer tbl_Customer = db.tbl_Customer.Find(id);
        //        if (tbl_Customer == null)
        //        {
        //            return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
        //        }
        //        else
        //        {
        //            db.tbl_Customer.Remove(tbl_Customer);
        //            db.SaveChanges();
        //            return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
        //    }
        //}


        public ActionResult ConvertPDF2(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

            tblHtmlReport.fldHtlmCode = myHtml;
            tblHtmlReport.fldFileName = filename;
            tblHtmlReport.fldReportName = reportname;

            db3.tblHtmlReports.Add(tblHtmlReport);
            db3.SaveChanges();

            success = true;
            status = "success";

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF2", "SAPIntegration", null, "http") + "/" + tblHtmlReport.fldID });
        }

        public ActionResult GetPDF2(int id)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string width = "1700", height = "1190";
            string imagepath = Server.MapPath("~/Asset/Images/");

            var gethtml = db3.tblHtmlReports.Find(id);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();


            Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
            {
                using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
                {
                    htmlWorker.Open();
                    htmlWorker.Parse(sr);
                }
            }
            pdfDoc.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            db.Entry(gethtml).State = EntityState.Deleted;
            db.SaveChanges();
            return View();
        }

    }
}
