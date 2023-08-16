using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsAPI;
using MVC_SYSTEM.ModelsCorporate;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    public class SAPMasterDataController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Models db3 = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetConfig GetConfig = new GetConfig();
        private GetNSWL GetNSWL = new GetNSWL();
        GetWilayah getwilyah = new GetWilayah();
        Connection Connection = new Connection();

        // GET: SAPMasterData
        public ActionResult Index()
        {
            //ViewBag.Maintenance = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.SubList = sublist;
            //ViewBag.Maintenance = "class = active";
            ViewBag.MainList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "sapmaster" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            db.Dispose();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string MainList, string SubList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (SubList != null)
            {
                return RedirectToAction(SubList, "SAPMasterData");
            }
            else
            {
                int MainLists = int.Parse(MainList);
                var action = db.tblMenuLists.Where(x => x.fld_ID == MainLists && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "SAPMasterData");
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

        public JsonResult GetLadang(int? WilayahID)
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
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LdgCode.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LdgCode.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }
        public ActionResult glList(string GLCode, string GLDesc) //farahin ubah - 28/4/2023
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //farahin tambah - 28/4/2023
            string CompCode = "";
            var result = new List<tbl_GLSAP>();
            //GLCode = 0;
            //GLDesc = "";

            //farahin tambah - 28/4/2023
            List<SelectListItem> CompanyCode = new List<SelectListItem>();
            CompanyCode = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            CompanyCode.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblAll, Value = "0" }));

            ViewBag.CompanyCode = CompanyCode;

            if (Request["CompanyCode"] == null)
            {
                CompCode = "";
            }
            else
            {
                CompCode = Request["CompanyCode"].ToString();
            }
            //sampai sini

            //farahin modified - 28/04/2023

            if (CompCode == "0" || CompCode == "")
            {
                if ((GLCode == null || GLCode == "") && (GLDesc == null || GLDesc == ""))
                {
                    result = db.tbl_GLSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (GLCode != null && GLCode != "")
                {
                    result = db.tbl_GLSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_GLcode.Contains(GLCode)).OrderByDescending(o => o.fld_DTModified).ToList();
                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (GLDesc != null && GLDesc != "")
                {
                    result = db.tbl_GLSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_Desc.Contains(GLDesc)).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    // return View(result);
                }
                return View(result);
            }
            else if (CompCode != "0" || CompCode != "")
            {
                if ((GLCode == null || GLCode == "") && (GLDesc == null || GLDesc == ""))
                {
                    result = db.tbl_GLSAP.Where(w => w.fld_CompanyCode == CompCode).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (GLCode != null && GLCode != "")
                {
                    result = db.tbl_GLSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_GLcode.Contains(GLCode) && w.fld_CompanyCode == CompCode).OrderByDescending(o => o.fld_DTModified).ToList();
                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (GLDesc != null && GLDesc != "")
                {
                    result = db.tbl_GLSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_Desc.Contains(GLDesc) && w.fld_CompanyCode == CompCode).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }
                return View(result);
            }
            else
            {
                result = new List<tbl_GLSAP>();
                return View(result);
            }

            //sampai sini

        }

        public ActionResult glRequest()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //farahin tukar linq
            List<SelectListItem> SyarikatList = new List<SelectListItem>();
            SyarikatList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.fld_CompanyCode = SyarikatList;

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            return PartialView("glRequest");
        }

        [HttpPost]
        public ActionResult _glRequest(tbl_SAPLog tbl_SAPLog, tbl_GLSAP _glSAP, tbl_GLSAPCreate _GLSAPCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);




            if (_GLSAPCreate.fld_GLcode == "")
            {
                _GLSAPCreate.fld_GLcode = "ALL";
            }
            else
            {
                _GLSAPCreate.fld_GLcode = _GLSAPCreate.fld_GLcode;
            }


            if (_GLSAPCreate.fld_GLcode2 == "")
            {
                if (_GLSAPCreate.fld_GLcode == "ALL")
                {
                    _GLSAPCreate.fld_GLcode2 = "";
                }
                else
                {
                    _GLSAPCreate.fld_GLcode2 = _GLSAPCreate.fld_GLcode;
                }
            }
            else
            {
                _GLSAPCreate.fld_GLcode2 = _GLSAPCreate.fld_GLcode2;
            }

            string bukrs = "", saknr = "", txt50 = "", xloeb = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";
            string exception = "";

            //FELDA
            var oClient = new SAPMD_FLP.ZWS_OPMS_MASTERClient();
            var request = new SAPMD_FLP.ZfmOpmsMaster();

            SAPMD_FLP.ZfmOpmsMasterResponse iresponse = new SAPMD_FLP.ZfmOpmsMasterResponse();

            SAPMD_FLP.Zopmsgl[] zopmsgl = new SAPMD_FLP.Zopmsgl[1];
            SAPMD_FLP.Zopmsgl zopmsgls = new SAPMD_FLP.Zopmsgl();

            SAPMD_FLP.Bapiret2[] bapirtn = new SAPMD_FLP.Bapiret2[1];
            SAPMD_FLP.Bapiret2 bapiret2_return = new SAPMD_FLP.Bapiret2();

            List<ZOPMSGL> glAmount = new List<ZOPMSGL>();

            oClient.ClientCredentials.UserName.UserName = "WF-BATCH";
            oClient.ClientCredentials.UserName.Password = "@12345bnm";


            //FPM
            var FPMClient = new FPMMD_FTQ.ZWS_OPMS_MASTERDATAClient();
            var FPMReq = new FPMMD_FTQ.ZFM_OPMS_MASTER();
            FPMMD_FTQ.ZFM_OPMS_MASTERResponse FPMRespond = new FPMMD_FTQ.ZFM_OPMS_MASTERResponse();

            FPMMD_FTQ.ZOPMSGL[] zopmsGL = new FPMMD_FTQ.ZOPMSGL[1];
            FPMMD_FTQ.ZOPMSGL zopmsGLs = new FPMMD_FTQ.ZOPMSGL();

            FPMMD_FTQ.BAPIRET2[] bAPIRET = new FPMMD_FTQ.BAPIRET2[1];
            FPMMD_FTQ.BAPIRET2 bAPIRETs = new FPMMD_FTQ.BAPIRET2();

            FPMClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            FPMClient.ClientCredentials.UserName.Password = "@12345bnm";



            oClient.Open(); FPMClient.Open();

            if (_GLSAPCreate.fld_CompanyCode == "1000")
            {
                try
                {

                    request = new SAPMD_FLP.ZfmOpmsMaster();

                    //request.DateBegin = tarikhmula;
                    //request.DateEnd = tarikhAkhir;
                    request.DateBegin = "";
                    request.DateEnd = "";
                    request.GlBegin = _GLSAPCreate.fld_GLcode;
                    request.GlEnd = _GLSAPCreate.fld_GLcode2;
                    request.GlComp = _GLSAPCreate.fld_CompanyCode;
                    request.ItGl = zopmsgl;

                    iresponse = oClient.ZfmOpmsMaster(request);

                    zopmsgl = iresponse.ItGl;
                    bapirtn = iresponse.Return;

                    if (iresponse.ItGl.Count() - 1 >= 0)
                    {
                        foreach (SAPMD_FLP.Zopmsgl a in zopmsgl)
                        {

                            bukrs = a.Bukrs;
                            saknr = a.Saknr;
                            txt50 = a.Txt50;
                            xloeb = a.Xloeb;

                            //save dlm db
                            //if glcode dah ade dlm db, update desc/deleted je.. kalau tak de baru save.
                            var getGLDetails = db.tbl_GLSAP.Where(x => x.fld_GLcode == saknr && x.fld_Desc == txt50 && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
                            var glCode = db.tbl_GLSAP.Where(x => x.fld_GLcode == saknr && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_GLcode).FirstOrDefault();
                            var gldesc = db.tbl_GLSAP.Where(x => x.fld_GLcode == saknr && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Desc).FirstOrDefault();


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
                                    _glSAP.fld_CreatedBy = "SAP";
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
                                else if (glCode != null && gldesc != txt50)
                                {

                                    ModelsCorporate.tbl_GLSAP getGL = db.tbl_GLSAP
                                                .Where(x => x.fld_GLcode == saknr && x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                                    getGL.fld_GLcode = saknr;
                                    getGL.fld_Desc = txt50;
                                    getGL.fld_CompanyCode = bukrs;
                                    getGL.fld_DTModified = DateTime.Today;
                                    getGL.fld_ModifiedBy = "SAP";

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
                                else
                                {
                                    ModelsCorporate.tbl_GLSAP getGL = db.tbl_GLSAP
                                                .Where(x => x.fld_GLcode == saknr && x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                                    getGL.fld_GLcode = saknr;
                                    getGL.fld_Desc = txt50;
                                    getGL.fld_CompanyCode = bukrs;
                                    getGL.fld_DTModified = DateTime.Today;
                                    getGL.fld_ModifiedBy = "SAP";

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
                                //tbl_SAPLog.fld_id = "";
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

                    if (iresponse.ItGl.Count() - 1 == 0)
                    {
                        if (iresponse.Return.Count() - 1 >= 0)
                        {
                            foreach (SAPMD_FLP.Bapiret2 a in bapirtn)
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

                }
                catch (Exception ex)
                {
                    throw (ex);
                    ViewBag.Message = ex;
                }
                finally
                {
                    oClient.Close();

                }
            }
            else if (_GLSAPCreate.fld_CompanyCode == "8800")
            {
                try
                {
                    FPMReq = new FPMMD_FTQ.ZFM_OPMS_MASTER();

                    FPMReq.GL_COMP = _GLSAPCreate.fld_CompanyCode;
                    FPMReq.GL_BEGIN = _GLSAPCreate.fld_GLcode;
                    FPMReq.GL_END = _GLSAPCreate.fld_GLcode2;
                    FPMReq.DATE_BEGIN = "";
                    FPMReq.DATE_END = "";
                    FPMReq.IT_GL = zopmsGL;

                    FPMRespond = FPMClient.ZFM_OPMS_MASTER(FPMReq);

                    zopmsGL = FPMRespond.IT_GL;
                    bAPIRET = FPMRespond.RETURN;

                    if (FPMRespond.IT_GL.Count() - 1 >= 0)
                    {
                        foreach (FPMMD_FTQ.ZOPMSGL a in zopmsGL)
                        {
                            //save dlm db
                            //if glcode dah ade dlm db, update desc/deleted je.. kalau tak de baru save.
                            var getGLDetails = db.tbl_GLSAP.Where(x => x.fld_GLcode == a.SAKNR && x.fld_Desc == a.TXT50 && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
                            var glCode = db.tbl_GLSAP.Where(x => x.fld_GLcode == a.SAKNR && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_GLcode).FirstOrDefault();
                            var gldesc = db.tbl_GLSAP.Where(x => x.fld_GLcode == a.SAKNR && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Desc).FirstOrDefault();


                            if (getGLDetails == null)
                            {
                                if (glCode == null)
                                {

                                    _glSAP = new tbl_GLSAP();

                                    _glSAP.fld_GLcode = a.SAKNR;
                                    _glSAP.fld_Desc = a.TXT50;
                                    _glSAP.fld_NegaraID = NegaraID;
                                    _glSAP.fld_SyarikatID = SyarikatID;
                                    _glSAP.fld_DTCreated = DateTime.Today;
                                    _glSAP.fld_DTModified = DateTime.Today;
                                    _glSAP.fld_CreatedBy = "SAP";
                                    _glSAP.fld_CompanyCode = a.BUKRS;

                                    if (a.XLOEB == "")
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
                                else if (glCode != null && gldesc != a.TXT50)
                                {

                                    ModelsCorporate.tbl_GLSAP getGL = db.tbl_GLSAP
                                                .Where(x => x.fld_GLcode == a.SAKNR && x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                                    getGL.fld_GLcode = a.SAKNR;
                                    getGL.fld_Desc = a.TXT50;
                                    getGL.fld_CompanyCode = a.BUKRS;
                                    getGL.fld_DTModified = DateTime.Today;
                                    getGL.fld_ModifiedBy = "SAP";

                                    if (a.XLOEB == "")
                                    {
                                        getGL.fld_Deleted = false;
                                    }
                                    else
                                    {
                                        getGL.fld_Deleted = true;
                                    };


                                    db.SaveChanges();

                                }
                                else
                                {
                                    ModelsCorporate.tbl_GLSAP getGL = db.tbl_GLSAP
                                                .Where(x => x.fld_GLcode == a.SAKNR && x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                                    getGL.fld_GLcode = a.SAKNR;
                                    getGL.fld_Desc = a.TXT50;
                                    getGL.fld_CompanyCode = a.BUKRS;
                                    getGL.fld_DTModified = DateTime.Today;
                                    getGL.fld_ModifiedBy = "SAP";

                                    if (a.XLOEB == "")
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
                                //tbl_SAPLog.fld_id = "";
                                tbl_SAPLog.fld_type = "S";
                                tbl_SAPLog.fld_message = "GL inbound success";
                                tbl_SAPLog.fld_msg1 = saknr;
                                tbl_SAPLog.fld_row = Convert.ToString(FPMRespond.IT_GL.Count());
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

                    if (FPMRespond.IT_GL.Count() - 1 == 0)
                    {
                        if (FPMRespond.RETURN.Count() - 1 >= 0)
                        {
                            foreach (FPMMD_FTQ.BAPIRET2 a in bAPIRET)
                            {

                                //save dlm db

                                tbl_SAPLog.fld_type = a.TYPE;
                                tbl_SAPLog.fld_number = a.NUMBER;
                                tbl_SAPLog.fld_id = a.ID;
                                tbl_SAPLog.fld_logno = a.LOG_NO;
                                tbl_SAPLog.fld_logmsgno = a.LOG_MSG_NO;
                                tbl_SAPLog.fld_message = a.MESSAGE;
                                tbl_SAPLog.fld_msg1 = a.MESSAGE_V1;
                                tbl_SAPLog.fld_msg2 = a.MESSAGE_V2;
                                tbl_SAPLog.fld_msg3 = a.MESSAGE_V3;
                                tbl_SAPLog.fld_msg4 = a.MESSAGE_V4;
                                tbl_SAPLog.fld_parameter = a.PARAMETER;
                                tbl_SAPLog.fld_row = a.ROW.ToString();
                                tbl_SAPLog.fld_field = a.FIELD;
                                tbl_SAPLog.fld_system = "SAP GL";

                                tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                                tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                                tbl_SAPLog.fld_logDate = DateTime.Now;

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    throw (ex);
                    ViewBag.Message = ex;
                }
                finally
                {


                }

            }

            oClient.Close(); FPMClient.Close();

            //return RedirectToAction("glList");
            return RedirectToAction("glTodayDate");
        }


        public ActionResult glTodayDate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            DateTime today = DateTime.Today;
            var result = db.tbl_GLSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && (w.fld_DTCreated == today || w.fld_DTModified == today)).OrderByDescending(o => o.fld_DTModified);

            if (!result.Any())
            {
                ViewBag.Message = "Tiada Record";
                return View();

            }

            ViewBag.Date = today.ToString("dd/MM/yyyy");
            return View(result);
        }
        public ViewResult ccList(string ccCode, string ccDesc)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            string CompCode = "";
            var result = new List<tbl_CCSAP>();

            //GLCode = 0;
            //GLDesc = "";

            List<SelectListItem> CompanyCode = new List<SelectListItem>();
            CompanyCode = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            CompanyCode.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblAll, Value = "0" }));

            ViewBag.CompanyCode = CompanyCode;

            if (Request["CompanyCode"] == null)
            {
                CompCode = "";
            }
            else
            {
                CompCode = Request["CompanyCode"].ToString();
            }


            if (CompCode == "0" || CompCode == "")
            {

                if ((ccCode == null || ccCode == "") && (ccDesc == null || ccDesc == ""))
                {
                    result = db.tbl_CCSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (ccCode != null && ccCode != "")
                {
                    result = db.tbl_CCSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_CstCnter.Contains(ccCode)).OrderByDescending(o => o.fld_DTModified).ToList();
                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    return View(result);
                }

                else if (ccDesc != null && ccDesc != "")
                {
                    result = db.tbl_CCSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_Desc.Contains(ccDesc)).OrderByDescending(o => o.fld_DTModified).ToList();



                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }


                }
                return View(result);
            }

            else if (CompCode != "0" || CompCode != "")
            {
                if ((ccCode == null || ccCode == "") && (ccDesc == null || ccDesc == ""))
                {
                    result = db.tbl_CCSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_CompanyCode == CompCode).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (ccCode != null && ccCode != "")
                {
                    result = db.tbl_CCSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_CstCnter.Contains(ccCode) && w.fld_CompanyCode == CompCode).OrderByDescending(o => o.fld_DTModified).ToList();
                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    return View(result);
                }

                else if (ccDesc != null && ccDesc != "")
                {
                    result = db.tbl_CCSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_Desc.Contains(ccDesc) && w.fld_CompanyCode == CompCode).OrderByDescending(o => o.fld_DTModified).ToList();



                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }


                }
                return View(result);
            }


            else
            {
                result = new List<tbl_CCSAP>();
                return View(result);
            }

        }

        public ActionResult ccRequest()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            //farahin tukar linq
            List<SelectListItem> SyarikatList = new List<SelectListItem>();
            SyarikatList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.fld_CompanyCode = SyarikatList;

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            return View();
        }

        [HttpPost]
        public ActionResult _ccRequest(tbl_SAPLog tbl_SAPLog, tbl_CCSAP _ccSAP, tbl_CCSAPCreate _CCSAPCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (_CCSAPCreate.fld_CstCnter == "")
            {
                _CCSAPCreate.fld_CstCnter = "ALL";
            }
            else
            {
                _CCSAPCreate.fld_CstCnter = _CCSAPCreate.fld_CstCnter;
            }


            if (_CCSAPCreate.fld_CstCnter2 == "")
            {
                if (_CCSAPCreate.fld_CstCnter == "ALL")
                {
                    _CCSAPCreate.fld_CstCnter2 = "";
                }
                else
                {
                    _CCSAPCreate.fld_CstCnter2 = _CCSAPCreate.fld_CstCnter;
                }
            }
            else
            {
                _CCSAPCreate.fld_CstCnter2 = _CCSAPCreate.fld_CstCnter2;
            }

            string today = DateTime.Now.ToString("yyyyMMdd");
            string KOKRS = "", KOSTL = "", LTEXT = "", BKZKP = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";

            //FELDA
            var oClient = new SAPMD_FLP.ZWS_OPMS_MASTERClient();
            var request = new SAPMD_FLP.ZfmOpmsMaster();
            SAPMD_FLP.ZfmOpmsMasterResponse iresponse = new SAPMD_FLP.ZfmOpmsMasterResponse();

            SAPMD_FLP.Zopmscc[] zopmscc = new SAPMD_FLP.Zopmscc[1];
            SAPMD_FLP.Zopmscc zopmsccs = new SAPMD_FLP.Zopmscc();

            SAPMD_FLP.Bapiret2[] bapirtn = new SAPMD_FLP.Bapiret2[1];
            SAPMD_FLP.Bapiret2 bapiret2_return = new SAPMD_FLP.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "WF-BATCH";
            oClient.ClientCredentials.UserName.Password = "@12345bnm";


            //FPM
            var FPMClient = new FPMMD_FTQ.ZWS_OPMS_MASTERDATAClient();
            var FPMReq = new FPMMD_FTQ.ZFM_OPMS_MASTER();
            FPMMD_FTQ.ZFM_OPMS_MASTERResponse FPMRespond = new FPMMD_FTQ.ZFM_OPMS_MASTERResponse();

            FPMMD_FTQ.ZOPMSCC[] zopmsCC = new FPMMD_FTQ.ZOPMSCC[1];
            FPMMD_FTQ.ZOPMSCC zopmsCCs = new FPMMD_FTQ.ZOPMSCC();

            FPMMD_FTQ.BAPIRET2[] bAPIRET = new FPMMD_FTQ.BAPIRET2[1];
            FPMMD_FTQ.BAPIRET2 bAPIRETs = new FPMMD_FTQ.BAPIRET2();

            FPMClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            FPMClient.ClientCredentials.UserName.Password = "@12345bnm";

            oClient.Open(); FPMClient.Open();


            if (_CCSAPCreate.fld_CompanyCode == "1000")
            {
                try
                {
                    request = new SAPMD_FLP.ZfmOpmsMaster();

                    //request.DateBegin = tarikhmula;
                    //request.DateEnd = tarikhAkhir;
                    request.DateBegin = "";
                    request.DateEnd = "";
                    request.CcBegin = _CCSAPCreate.fld_CstCnter;
                    request.CcEnd = _CCSAPCreate.fld_CstCnter2;
                    request.CcComp = _CCSAPCreate.fld_CompanyCode;
                    request.ItCc = zopmscc;

                    iresponse = oClient.ZfmOpmsMaster(request);

                    zopmscc = iresponse.ItCc;
                    bapirtn = iresponse.Return;

                    if (iresponse.ItCc.Count() - 1 >= 0)
                    {

                        foreach (SAPMD_FLP.Zopmscc a in zopmscc)
                        {
                            KOKRS = a.Kokrs;
                            KOSTL = a.Kostl;
                            LTEXT = a.Ltext;
                            BKZKP = a.Bkzkp;

                            //save dlm db

                            //if glcode dah ade dlm db, tak yah save. kalau tak de baru save.


                            var getCCDetails = db.tbl_CCSAP.Where(x => x.fld_CstCnter == KOSTL && x.fld_Desc == LTEXT && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
                            var Code = db.tbl_CCSAP.Where(x => x.fld_CstCnter == KOSTL && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_CstCnter).FirstOrDefault();
                            var desc = db.tbl_CCSAP.Where(x => x.fld_CstCnter == KOSTL && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Desc).FirstOrDefault();



                            if (getCCDetails == null)
                            {
                                if (Code == null)
                                {

                                    _ccSAP = new tbl_CCSAP();

                                    _ccSAP.fld_CstCnter = KOSTL;
                                    _ccSAP.fld_Desc = LTEXT;
                                    _ccSAP.fld_NegaraID = NegaraID;
                                    _ccSAP.fld_SyarikatID = SyarikatID;
                                    _ccSAP.fld_DTCreated = DateTime.Today;
                                    _ccSAP.fld_DTModified = DateTime.Today;
                                    _ccSAP.fld_CreatedBy = "SAP";
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

                                else if (Code != null && desc != LTEXT)
                                {
                                    ModelsCorporate.tbl_CCSAP getCC = db.tbl_CCSAP
                                                .Where(x => x.fld_CstCnter == KOSTL).FirstOrDefault();

                                    getCC.fld_CstCnter = KOSTL;
                                    getCC.fld_Desc = LTEXT;
                                    getCC.fld_DTModified = DateTime.Today;
                                    getCC.fld_ModifiedBy = "SAP";
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
                                tbl_SAPLog.fld_logDate = DateTime.Today;
                                tbl_SAPLog.fld_msg4 = "SAP";
                                tbl_SAPLog.fld_negaraID = "1";
                                tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }
                            else
                            {


                                ModelsCorporate.tbl_CCSAP getCC = db.tbl_CCSAP
                                                        .Where(x => x.fld_CstCnter == KOSTL).FirstOrDefault();

                                getCC.fld_CstCnter = KOSTL;
                                getCC.fld_Desc = LTEXT;
                                getCC.fld_DTModified = DateTime.Today;
                                getCC.fld_ModifiedBy = "SAP";
                                if (BKZKP == "") { getCC.fld_Deleted = false; }
                                else { getCC.fld_Deleted = true; };

                                db.SaveChanges();

                                //return if success since sap will return null to any success inbound
                                //save dlm db

                                tbl_SAPLog.fld_type = "S";
                                tbl_SAPLog.fld_message = "CC inbound success";
                                tbl_SAPLog.fld_msg1 = KOSTL;
                                tbl_SAPLog.fld_row = Convert.ToString(iresponse.ItCc.Count());
                                tbl_SAPLog.fld_system = "SAP CC";
                                tbl_SAPLog.fld_logDate = DateTime.Today;
                                tbl_SAPLog.fld_msg4 = "SAP";
                                tbl_SAPLog.fld_negaraID = "1";
                                tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }


                        }


                    }


                    if (iresponse.ItCc.Count() - 1 == 0)
                    {
                        if (iresponse.Return.Count() - 1 >= 0)
                        {
                            foreach (SAPMD_FLP.Bapiret2 a in bapirtn)
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

                }

                catch (Exception ex)
                {
                    throw (ex);
                }

                finally
                {

                }
            }
            else if (_CCSAPCreate.fld_CompanyCode == "8800")
            {
                try
                {
                    FPMReq = new FPMMD_FTQ.ZFM_OPMS_MASTER();

                    FPMReq.CC_COMP = _CCSAPCreate.fld_CompanyCode;
                    FPMReq.CC_BEGIN = _CCSAPCreate.fld_CstCnter;
                    FPMReq.CC_END = _CCSAPCreate.fld_CstCnter2;
                    FPMReq.DATE_BEGIN = "";
                    FPMReq.DATE_END = "";
                    FPMReq.IT_CC = zopmsCC;

                    FPMRespond = FPMClient.ZFM_OPMS_MASTER(FPMReq);

                    zopmsCC = FPMRespond.IT_CC;
                    bAPIRET = FPMRespond.RETURN;

                    if (FPMRespond.IT_CC.Count() - 1 >= 0)
                    {
                        foreach (FPMMD_FTQ.ZOPMSCC a in zopmsCC)
                        {
                            //save dlm db
                            //if glcode dah ade dlm db, update desc/deleted je.. kalau tak de baru save.
                            var getCCDetails = db.tbl_CCSAP.Where(x => x.fld_CstCnter == a.KOSTL && x.fld_Desc == a.LTEXT && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
                            var ccCode = db.tbl_CCSAP.Where(x => x.fld_CstCnter == a.KOSTL && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_CstCnter).FirstOrDefault();
                            var ccdesc = db.tbl_CCSAP.Where(x => x.fld_CstCnter == a.KOSTL && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Desc).FirstOrDefault();


                            if (getCCDetails == null)
                            {
                                if (ccCode == null)
                                {

                                    _ccSAP = new tbl_CCSAP();

                                    _ccSAP.fld_CstCnter = a.KOSTL;
                                    _ccSAP.fld_Desc = a.LTEXT;
                                    _ccSAP.fld_NegaraID = NegaraID;
                                    _ccSAP.fld_SyarikatID = SyarikatID;
                                    _ccSAP.fld_DTCreated = DateTime.Today;
                                    _ccSAP.fld_DTModified = DateTime.Today;
                                    _ccSAP.fld_CreatedBy = "SAP";
                                    _ccSAP.fld_CompanyCode = a.KOKRS;

                                    if (a.BKZKP == "")
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
                                else if (ccCode != null && ccdesc != a.LTEXT)
                                {

                                    ModelsCorporate.tbl_CCSAP getCC = db.tbl_CCSAP
                                                .Where(x => x.fld_CstCnter == a.KOSTL && x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                                    getCC.fld_CstCnter = a.KOSTL;
                                    getCC.fld_Desc = a.LTEXT;
                                    getCC.fld_CompanyCode = a.KOKRS;
                                    getCC.fld_DTModified = DateTime.Today;
                                    getCC.fld_ModifiedBy = "SAP";

                                    if (a.BKZKP == "")
                                    {
                                        getCC.fld_Deleted = false;
                                    }
                                    else
                                    {
                                        getCC.fld_Deleted = true;
                                    };


                                    db.SaveChanges();

                                }
                                else
                                {
                                    ModelsCorporate.tbl_CCSAP getCC = db.tbl_CCSAP
                                                .Where(x => x.fld_CstCnter == a.KOSTL && x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                                    getCC.fld_CstCnter = a.KOSTL;
                                    getCC.fld_Desc = a.LTEXT;
                                    getCC.fld_CompanyCode = a.KOKRS;
                                    getCC.fld_DTModified = DateTime.Today;
                                    getCC.fld_ModifiedBy = "SAP";

                                    if (a.BKZKP == "")
                                    {
                                        getCC.fld_Deleted = false;
                                    }
                                    else
                                    {
                                        getCC.fld_Deleted = true;
                                    };


                                    db.SaveChanges();
                                }


                                //return if success since sap will return null to any success inbound
                                //save dlm db
                                //tbl_SAPLog.fld_id = "";
                                tbl_SAPLog.fld_type = "S";
                                tbl_SAPLog.fld_message = "CC inbound success";
                                tbl_SAPLog.fld_msg1 = a.KOSTL;
                                tbl_SAPLog.fld_row = Convert.ToString(FPMRespond.IT_CC.Count());
                                tbl_SAPLog.fld_system = "SAP CC";
                                tbl_SAPLog.fld_logDate = DateTime.Now;
                                tbl_SAPLog.fld_msg4 = User.Identity.Name;
                                tbl_SAPLog.fld_negaraID = "1";
                                tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }


                        }


                    }

                    if (FPMRespond.IT_CC.Count() - 1 == 0)
                    {
                        if (FPMRespond.RETURN.Count() - 1 >= 0)
                        {
                            foreach (FPMMD_FTQ.BAPIRET2 a in bAPIRET)
                            {

                                //save dlm db

                                tbl_SAPLog.fld_type = a.TYPE;
                                tbl_SAPLog.fld_number = a.NUMBER;
                                tbl_SAPLog.fld_id = a.ID;
                                tbl_SAPLog.fld_logno = a.LOG_NO;
                                tbl_SAPLog.fld_logmsgno = a.LOG_MSG_NO;
                                tbl_SAPLog.fld_message = a.MESSAGE;
                                tbl_SAPLog.fld_msg1 = a.MESSAGE_V1;
                                tbl_SAPLog.fld_msg2 = a.MESSAGE_V2;
                                tbl_SAPLog.fld_msg3 = a.MESSAGE_V3;
                                tbl_SAPLog.fld_msg4 = a.MESSAGE_V4;
                                tbl_SAPLog.fld_parameter = a.PARAMETER;
                                tbl_SAPLog.fld_row = a.ROW.ToString();
                                tbl_SAPLog.fld_field = a.FIELD;
                                tbl_SAPLog.fld_system = "SAP CC";

                                tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                                tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                                tbl_SAPLog.fld_logDate = DateTime.Now;

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    throw (ex);
                    ViewBag.Message = ex;
                }
                finally
                {


                }

            }
            oClient.Close(); FPMClient.Close();
            //return RedirectToAction("ccList");
            return RedirectToAction("ccTodayList");
        }

        public ActionResult ccTodayList()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            DateTime today = DateTime.UtcNow.Date;

            var result = db.tbl_CCSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && (w.fld_DTCreated == today || w.fld_DTModified == today)).OrderByDescending(o => o.fld_DTModified);

            if (!result.Any())
            {
                ViewBag.Message = "Tiada Record";
                return View();

            }

            ViewBag.Date = today.ToString("dd/MM/yyyy");
            return View(result);

        }

        public ViewResult vdList(string vdCode, string vdDesc)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //GLCode = 0;
            //GLDesc = "";
            string CompCode = "";
            var result = new List<tbl_VDSAP>();

            List<SelectListItem> CompanyCode = new List<SelectListItem>();
            CompanyCode = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            CompanyCode.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblAll, Value = "0" }));

            ViewBag.CompanyCode = CompanyCode;

            if (Request["CompanyCode"] == null)
            {
                CompCode = "";
            }
            else
            {
                CompCode = Request["CompanyCode"].ToString();
            }

            if (CompCode == "0" || CompCode == "")
            {
                if ((vdCode == null || vdCode == "") && (vdDesc == null || vdDesc == ""))
                {
                    result = db.tbl_VDSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (vdCode != null && vdCode != "")
                {
                    result = db.tbl_VDSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_VendorNo.Contains(vdCode)).OrderByDescending(o => o.fld_DTModified).ToList();
                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (vdDesc != null && vdDesc != "")
                {
                    result = db.tbl_VDSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_Desc.Contains(vdDesc)).OrderByDescending(o => o.fld_DTModified).ToList();



                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }


                }
                return View(result);
            }

            else if (CompCode != "0" || CompCode != "")
            {
                if ((vdCode == null || vdCode == "") && (vdDesc == null || vdDesc == ""))
                {
                    result = db.tbl_VDSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_CompanyCode == CompCode).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (vdCode != null && vdCode != "")
                {
                    result = db.tbl_VDSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_VendorNo.Contains(vdCode) && w.fld_CompanyCode == CompCode).OrderByDescending(o => o.fld_DTModified).ToList();
                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (vdDesc != null && vdDesc != "")
                {
                    result = db.tbl_VDSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_Desc.Contains(vdDesc) && w.fld_CompanyCode == CompCode).OrderByDescending(o => o.fld_DTModified).ToList();



                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }


                }
                return View(result);
            }

            else
            {
                result = new List<tbl_VDSAP>();
                return View(result);
            }

        }

        public ActionResult vdRequest()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            //farahin tukar linq
            List<SelectListItem> SyarikatList = new List<SelectListItem>();
            SyarikatList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.fld_CompanyCode = SyarikatList;

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            return View();
        }

        [HttpPost]
        public ActionResult _vdRequest(tbl_SAPLog tbl_SAPLog, tbl_VDSAP _vdSAP, tbl_VDSAPCreate _VDSAPCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (_VDSAPCreate.fld_VendorNo == "")
            {
                _VDSAPCreate.fld_VendorNo = "ALL";
            }
            else
            {
                _VDSAPCreate.fld_VendorNo = _VDSAPCreate.fld_VendorNo;
            }


            if (_VDSAPCreate.fld_VendorNo2 == "")
            {
                if (_VDSAPCreate.fld_VendorNo == "ALL")
                {
                    _VDSAPCreate.fld_VendorNo2 = "";
                }
                else
                {
                    _VDSAPCreate.fld_VendorNo2 = _VDSAPCreate.fld_VendorNo;
                }
            }
            else
            {
                _VDSAPCreate.fld_VendorNo2 = _VDSAPCreate.fld_VendorNo2;
            }

            string today = DateTime.Now.ToString("yyyyMMdd");
            string bukrs = "", lifnr = "", name1 = "", loevm = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";

            //FELDA
            var oClient = new SAPMD_FLP.ZWS_OPMS_MASTERClient();
            var request = new SAPMD_FLP.ZfmOpmsMaster();
            SAPMD_FLP.ZfmOpmsMasterResponse iresponse = new SAPMD_FLP.ZfmOpmsMasterResponse();

            SAPMD_FLP.Zopmsvd[] zopmsvd = new SAPMD_FLP.Zopmsvd[1];
            SAPMD_FLP.Zopmsvd zopmsvds = new SAPMD_FLP.Zopmsvd();

            SAPMD_FLP.Bapiret2[] bapirtn = new SAPMD_FLP.Bapiret2[1];
            SAPMD_FLP.Bapiret2 bapiret2_return = new SAPMD_FLP.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "WF-BATCH";
            oClient.ClientCredentials.UserName.Password = "@12345bnm";

            //FPM
            var FPMClient = new FPMMD_FTQ.ZWS_OPMS_MASTERDATAClient();
            var FPMReq = new FPMMD_FTQ.ZFM_OPMS_MASTER();
            FPMMD_FTQ.ZFM_OPMS_MASTERResponse FPMRespond = new FPMMD_FTQ.ZFM_OPMS_MASTERResponse();

            FPMMD_FTQ.ZOPMSVD[] zopmsVD = new FPMMD_FTQ.ZOPMSVD[1];
            FPMMD_FTQ.ZOPMSVD zopmsVDs = new FPMMD_FTQ.ZOPMSVD();

            FPMMD_FTQ.BAPIRET2[] bAPIRET = new FPMMD_FTQ.BAPIRET2[1];
            FPMMD_FTQ.BAPIRET2 bAPIRETs = new FPMMD_FTQ.BAPIRET2();

            FPMClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            FPMClient.ClientCredentials.UserName.Password = "@12345bnm";

            oClient.Open(); FPMClient.Open();


            if (_VDSAPCreate.fld_CompanyCode == "1000")
            {

                try
                {
                    request = new SAPMD_FLP.ZfmOpmsMaster();

                    //request.DateBegin = tarikhmula;
                    //request.DateEnd = tarikhAkhir;
                    request.DateBegin = "";
                    request.DateEnd = "";
                    request.VdBegin = _VDSAPCreate.fld_VendorNo;
                    request.VdEnd = _VDSAPCreate.fld_VendorNo2;
                    request.VdComp = _VDSAPCreate.fld_CompanyCode;
                    request.ItVend = zopmsvd;

                    iresponse = oClient.ZfmOpmsMaster(request);

                    zopmsvd = iresponse.ItVend;
                    bapirtn = iresponse.Return;

                    if (iresponse.ItVend.Count() - 1 >= 0)
                    {

                        foreach (SAPMD_FLP.Zopmsvd a in zopmsvd)
                        {
                            bukrs = a.Bukrs;
                            lifnr = a.Lifnr;
                            name1 = a.Name1;
                            loevm = a.Loevm;

                            //save dlm db

                            //if glcode dah ade dlm db, tak yah save. kalau tak de baru save.


                            var getVDDetails = db.tbl_VDSAP.Where(x => x.fld_VendorNo == lifnr && x.fld_Desc == name1).FirstOrDefault();
                            var Code = db.tbl_VDSAP.Where(x => x.fld_VendorNo == lifnr).Select(s => s.fld_VendorNo).FirstOrDefault();
                            var desc = db.tbl_VDSAP.Where(x => x.fld_VendorNo == lifnr).Select(s => s.fld_Desc).FirstOrDefault();


                            if (getVDDetails == null)
                            {
                                if (Code == null)
                                {

                                    _vdSAP = new tbl_VDSAP();

                                    _vdSAP.fld_VendorNo = lifnr;
                                    _vdSAP.fld_Desc = name1;
                                    _vdSAP.fld_NegaraID = NegaraID;
                                    _vdSAP.fld_SyarikatID = SyarikatID;
                                    _vdSAP.fld_DTCreated = DateTime.Today;
                                    _vdSAP.fld_DTModified = DateTime.Today;
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

                                else if (Code != null && desc == null)
                                {
                                    ModelsCorporate.tbl_VDSAP getVD = db.tbl_VDSAP
                                                .Single(x => x.fld_VendorNo == lifnr);

                                    getVD.fld_Desc = name1;
                                    getVD.fld_DTModified = DateTime.Today;
                                    getVD.fld_ModifiedBy = User.Identity.Name;

                                    if (loevm == "")
                                    {
                                        getVD.fld_Deleted = false;
                                    }
                                    else
                                    {
                                        getVD.fld_Deleted = true;
                                    };

                                    db.SaveChanges();
                                }
                                //return if success since sap will return null to any success inbound
                                //save dlm db

                                tbl_SAPLog.fld_type = "S";
                                tbl_SAPLog.fld_message = "Vendor inbound success";
                                tbl_SAPLog.fld_msg1 = lifnr;
                                tbl_SAPLog.fld_row = Convert.ToString(iresponse.ItVend.Count());
                                tbl_SAPLog.fld_system = "SAP VD";
                                tbl_SAPLog.fld_logDate = DateTime.Today;
                                tbl_SAPLog.fld_msg4 = "SAP";
                                tbl_SAPLog.fld_negaraID = "1";
                                tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }




                        }


                    }
                    if (iresponse.ItVend.Count() - 1 == 0)
                    {
                        if (iresponse.Return.Count() - 1 >= 0)
                        {
                            foreach (SAPMD_FLP.Bapiret2 a in bapirtn)
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

                                // ViewBag.Message = "No Data for Date : " tarikhmula + "-" + tarikhAkhir + "| Vendor Code: " + VDStart + "|" + VDEnd;
                            }

                        }
                    }

                }

                catch (Exception ex)
                {
                    throw (ex);
                }

                finally
                {

                }
            }
            else if (_VDSAPCreate.fld_CompanyCode == "8800")
            {
                try
                {
                    FPMReq = new FPMMD_FTQ.ZFM_OPMS_MASTER();

                    FPMReq.VD_COMP = _VDSAPCreate.fld_CompanyCode;
                    FPMReq.VD_BEGIN = _VDSAPCreate.fld_VendorNo;
                    FPMReq.VD_END = _VDSAPCreate.fld_VendorNo2;
                    FPMReq.DATE_BEGIN = "";
                    FPMReq.DATE_END = "";
                    FPMReq.IT_VEND = zopmsVD;

                    FPMRespond = FPMClient.ZFM_OPMS_MASTER(FPMReq);

                    zopmsVD = FPMRespond.IT_VEND;
                    bAPIRET = FPMRespond.RETURN;

                    if (FPMRespond.IT_VEND.Count() - 1 >= 0)
                    {
                        foreach (FPMMD_FTQ.ZOPMSVD a in zopmsVD)
                        {
                            //save dlm db
                            //if glcode dah ade dlm db, update desc/deleted je.. kalau tak de baru save.
                            var getVDDetails = db.tbl_VDSAP.Where(x => x.fld_VendorNo == a.LIFNR && x.fld_Desc == a.NAME1 && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
                            var VDCode = db.tbl_VDSAP.Where(x => x.fld_VendorNo == a.LIFNR && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_VendorNo).FirstOrDefault();
                            var VDdesc = db.tbl_VDSAP.Where(x => x.fld_VendorNo == a.LIFNR && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Desc).FirstOrDefault();


                            if (getVDDetails == null)
                            {
                                if (VDCode == null)
                                {

                                    _vdSAP = new tbl_VDSAP();

                                    _vdSAP.fld_VendorNo = a.LIFNR;
                                    _vdSAP.fld_Desc = a.NAME1;
                                    _vdSAP.fld_NegaraID = NegaraID;
                                    _vdSAP.fld_SyarikatID = SyarikatID;
                                    _vdSAP.fld_DTCreated = DateTime.Today;
                                    _vdSAP.fld_DTModified = DateTime.Today;
                                    _vdSAP.fld_CreatedBy = "SAP";
                                    _vdSAP.fld_CompanyCode = a.BUKRS;

                                    if (a.LOEVM == "")
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
                                else if (VDCode != null && VDdesc != a.NAME1)
                                {

                                    ModelsCorporate.tbl_VDSAP getVD = db.tbl_VDSAP
                                                .Where(x => x.fld_VendorNo == a.LIFNR && x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                                    getVD.fld_VendorNo = a.LIFNR;
                                    getVD.fld_Desc = a.NAME1;
                                    getVD.fld_CompanyCode = a.BUKRS;
                                    getVD.fld_DTModified = DateTime.Today;
                                    getVD.fld_ModifiedBy = "SAP";

                                    if (a.LOEVM == "")
                                    {
                                        getVD.fld_Deleted = false;
                                    }
                                    else
                                    {
                                        getVD.fld_Deleted = true;
                                    };


                                    db.SaveChanges();

                                }
                                else
                                {
                                    ModelsCorporate.tbl_VDSAP getVD = db.tbl_VDSAP
                                                .Where(x => x.fld_VendorNo == a.LIFNR && x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                                    getVD.fld_VendorNo = a.LIFNR;
                                    getVD.fld_Desc = a.NAME1;
                                    getVD.fld_CompanyCode = a.BUKRS;
                                    getVD.fld_DTModified = DateTime.Today;
                                    getVD.fld_ModifiedBy = "SAP";

                                    if (a.LOEVM == "")
                                    {
                                        getVD.fld_Deleted = false;
                                    }
                                    else
                                    {
                                        getVD.fld_Deleted = true;
                                    };


                                    db.SaveChanges();
                                }


                                //return if success since sap will return null to any success inbound
                                //save dlm db
                                //tbl_SAPLog.fld_id = "";
                                tbl_SAPLog.fld_type = "S";
                                tbl_SAPLog.fld_message = "CC inbound success";
                                tbl_SAPLog.fld_msg1 = a.LIFNR;
                                tbl_SAPLog.fld_row = Convert.ToString(FPMRespond.IT_CC.Count());
                                tbl_SAPLog.fld_system = "SAP CC";
                                tbl_SAPLog.fld_logDate = DateTime.Now;
                                tbl_SAPLog.fld_msg4 = User.Identity.Name;
                                tbl_SAPLog.fld_negaraID = "1";
                                tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }


                        }


                    }

                    if (FPMRespond.IT_CC.Count() - 1 == 0)
                    {
                        if (FPMRespond.RETURN.Count() - 1 >= 0)
                        {
                            foreach (FPMMD_FTQ.BAPIRET2 a in bAPIRET)
                            {

                                //save dlm db

                                tbl_SAPLog.fld_type = a.TYPE;
                                tbl_SAPLog.fld_number = a.NUMBER;
                                tbl_SAPLog.fld_id = a.ID;
                                tbl_SAPLog.fld_logno = a.LOG_NO;
                                tbl_SAPLog.fld_logmsgno = a.LOG_MSG_NO;
                                tbl_SAPLog.fld_message = a.MESSAGE;
                                tbl_SAPLog.fld_msg1 = a.MESSAGE_V1;
                                tbl_SAPLog.fld_msg2 = a.MESSAGE_V2;
                                tbl_SAPLog.fld_msg3 = a.MESSAGE_V3;
                                tbl_SAPLog.fld_msg4 = a.MESSAGE_V4;
                                tbl_SAPLog.fld_parameter = a.PARAMETER;
                                tbl_SAPLog.fld_row = a.ROW.ToString();
                                tbl_SAPLog.fld_field = a.FIELD;
                                tbl_SAPLog.fld_system = "SAP VD";

                                tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                                tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                                tbl_SAPLog.fld_logDate = DateTime.Now;

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    throw (ex);
                    ViewBag.Message = ex;
                }
                finally
                {


                }

            }
            oClient.Close(); FPMClient.Close();

            return RedirectToAction("vdTodayList");
        }

        public ActionResult vdTodayList()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            DateTime today = DateTime.UtcNow.Date;

            var result = db.tbl_VDSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && (w.fld_DTCreated == today || w.fld_DTModified == today)).OrderByDescending(o => o.fld_DTModified);

            if (!result.Any())
            {
                ViewBag.Message = "Tiada Record";
                return View();

            }

            ViewBag.Date = today.ToString("dd/MM/yyyy");

            return View(result);

        }

        public ViewResult cmList(string cmCode, string cmDesc)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //GLCode = 0;
            //GLDesc = "";

            //farahin tambah - 28/4/2023
            string CompCode = "";
            var result = new List<tbl_CMSAP>();
            //GLCode = 0;
            //GLDesc = "";

            //farahin tambah - 28/4/2023
            List<SelectListItem> CompanyCode = new List<SelectListItem>();
            CompanyCode = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            CompanyCode.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblAll, Value = "0" }));

            ViewBag.CompanyCode = CompanyCode;

            if (Request["CompanyCode"] == null)
            {
                CompCode = "";
            }
            else
            {
                CompCode = Request["CompanyCode"].ToString();
            }
            //sampai sini
            if (CompCode == "0" || CompCode == "")
            {
                if ((cmCode == null || cmCode == "") && (cmDesc == null || cmDesc == ""))
                {
                    result = db.tbl_CMSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (cmCode != null && cmCode != "")
                {
                    result = db.tbl_CMSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_CustomerNo.Contains(cmCode)).OrderByDescending(o => o.fld_DTModified).ToList();
                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (cmDesc != null && cmDesc != "")
                {
                    result = db.tbl_CMSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_Desc.Contains(cmDesc)).OrderByDescending(o => o.fld_DTModified).ToList();



                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }
                return View(result);
            }
            else if (CompCode != "0" || CompCode != "")
            {
                if ((cmCode == null || cmCode == "") && (cmDesc == null || cmDesc == ""))
                {
                    result = db.tbl_CMSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (cmCode != null && cmCode != "")
                {
                    result = db.tbl_CMSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_CustomerNo.Contains(cmCode)).OrderByDescending(o => o.fld_DTModified).ToList();
                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }

                else if (cmDesc != null && cmDesc != "")
                {
                    result = db.tbl_CMSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_Desc.Contains(cmDesc)).OrderByDescending(o => o.fld_DTModified).ToList();



                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }

                    //return View(result);
                }
                return View(result);
            }

            else
            {
                result = new List<tbl_CMSAP>();
                return View(result);
            }

        }

        public ActionResult cmRequest()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> SyarikatList = new List<SelectListItem>();
            SyarikatList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.fld_CompanyCode = SyarikatList;

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            return View();
        }

        [HttpPost]
        public ActionResult _cmRequest(tbl_SAPLog tbl_SAPLog, tbl_CMSAP _cmSAP, tbl_CMSAPCreate _CMSAPCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (_CMSAPCreate.fld_CustomerNo == "")
            {
                _CMSAPCreate.fld_CustomerNo = "ALL";
            }
            else
            {
                _CMSAPCreate.fld_CustomerNo = _CMSAPCreate.fld_CustomerNo;
            }


            if (_CMSAPCreate.fld_CustomerNo2 == "")
            {
                if (_CMSAPCreate.fld_CustomerNo == "ALL")
                {
                    _CMSAPCreate.fld_CustomerNo2 = "";
                }
                else
                {
                    _CMSAPCreate.fld_CustomerNo2 = _CMSAPCreate.fld_CustomerNo;
                }
            }
            else
            {
                _CMSAPCreate.fld_CustomerNo2 = _CMSAPCreate.fld_CustomerNo2;
            }

            string today = DateTime.Now.ToString("yyyyMMdd");
            string bukrs = "", kunnr = "", name1 = "", loevm = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";

            //FELDA
            var oClient = new SAPMD_FLP.ZWS_OPMS_MASTERClient();
            var request = new SAPMD_FLP.ZfmOpmsMaster();
            SAPMD_FLP.ZfmOpmsMasterResponse iresponse = new SAPMD_FLP.ZfmOpmsMasterResponse();

            SAPMD_FLP.Zopmscs[] zopmscs = new SAPMD_FLP.Zopmscs[1];
            SAPMD_FLP.Zopmscs zopmscss = new SAPMD_FLP.Zopmscs();

            SAPMD_FLP.Bapiret2[] bapirtn = new SAPMD_FLP.Bapiret2[1];
            SAPMD_FLP.Bapiret2 bapiret2_return = new SAPMD_FLP.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "WF-BATCH";
            oClient.ClientCredentials.UserName.Password = "@12345bnm";

            //FPM
            var FPMClient = new FPMMD_FTQ.ZWS_OPMS_MASTERDATAClient();
            var FPMReq = new FPMMD_FTQ.ZFM_OPMS_MASTER();
            FPMMD_FTQ.ZFM_OPMS_MASTERResponse FPMRespond = new FPMMD_FTQ.ZFM_OPMS_MASTERResponse();

            FPMMD_FTQ.ZOPMSCS[] zopmsCS = new FPMMD_FTQ.ZOPMSCS[1];
            FPMMD_FTQ.ZOPMSCS zopmsCSs = new FPMMD_FTQ.ZOPMSCS();

            FPMMD_FTQ.BAPIRET2[] bAPIRET = new FPMMD_FTQ.BAPIRET2[1];
            FPMMD_FTQ.BAPIRET2 bAPIRETs = new FPMMD_FTQ.BAPIRET2();

            FPMClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            FPMClient.ClientCredentials.UserName.Password = "@12345bnm";

            oClient.Open(); FPMClient.Open();


            if (_CMSAPCreate.fld_CompanyCode == "1000")
            {

                try
                {
                    request = new SAPMD_FLP.ZfmOpmsMaster();

                    //request.DateBegin = tarikhmula;
                    //request.DateEnd = tarikhAkhir;
                    request.DateBegin = "";
                    request.DateEnd = "";
                    request.CsBegin = _CMSAPCreate.fld_CustomerNo;
                    request.CsEnd = _CMSAPCreate.fld_CustomerNo2;
                    request.CsComp = _CMSAPCreate.fld_CompanyCode;
                    request.ItCust = zopmscs;

                    iresponse = oClient.ZfmOpmsMaster(request);

                    zopmscs = iresponse.ItCust;
                    bapirtn = iresponse.Return;

                    if (iresponse.ItCust.Count() - 1 >= 0)
                    {

                        foreach (SAPMD_FLP.Zopmscs a in zopmscs)
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

                                    _cmSAP = new tbl_CMSAP();

                                    _cmSAP.fld_CustomerNo = kunnr;
                                    _cmSAP.fld_Desc = name1;
                                    _cmSAP.fld_NegaraID = NegaraID;
                                    _cmSAP.fld_SyarikatID = SyarikatID;
                                    _cmSAP.fld_DTCreated = DateTime.Today;
                                    _cmSAP.fld_DTModified = DateTime.Today;
                                    _cmSAP.fld_CompanyCode = bukrs;
                                    _cmSAP.fld_CreatedBy = "SAP";

                                    if (loevm == "")
                                    {
                                        _cmSAP.fld_Deleted = false;
                                    }
                                    else
                                    {
                                        _cmSAP.fld_Deleted = true;
                                    };

                                    db.tbl_CMSAP.Add(_cmSAP);
                                    db.SaveChanges();
                                    db.Entry(_cmSAP).State = EntityState.Detached;
                                }

                                else if (cmCode != null && cmDesc == null)
                                {
                                    ModelsCorporate.tbl_CMSAP getCM = db.tbl_CMSAP
                                                .Single(x => x.fld_CustomerNo == kunnr);

                                    getCM.fld_Desc = name1;
                                    getCM.fld_DTModified = DateTime.Today;
                                    getCM.fld_ModifiedBy = "SAP";
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


                    if (iresponse.ItCust.Count() - 1 == 0)
                    {
                        if (iresponse.Return.Count() - 1 >= 0)
                        {
                            foreach (SAPMD_FLP.Bapiret2 a in bapirtn)
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

                }

                catch (Exception ex)
                {
                    throw (ex);
                }

                finally
                {

                }
            }
            else if (_CMSAPCreate.fld_CompanyCode == "8800")
            {
                try
                {
                    FPMReq = new FPMMD_FTQ.ZFM_OPMS_MASTER();

                    FPMReq.CS_COMP = _CMSAPCreate.fld_CompanyCode;
                    FPMReq.CS_BEGIN = _CMSAPCreate.fld_CustomerNo;
                    FPMReq.CS_END = _CMSAPCreate.fld_CustomerNo2;
                    FPMReq.DATE_BEGIN = "";
                    FPMReq.DATE_END = "";
                    FPMReq.IT_CUST = zopmsCS;

                    FPMRespond = FPMClient.ZFM_OPMS_MASTER(FPMReq);

                    zopmsCS = FPMRespond.IT_CUST;
                    bAPIRET = FPMRespond.RETURN;

                    if (FPMRespond.IT_CUST.Count() - 1 >= 0)
                    {
                        foreach (FPMMD_FTQ.ZOPMSCS a in zopmsCS)
                        {
                            //save dlm db
                            //if glcode dah ade dlm db, update desc/deleted je.. kalau tak de baru save.
                            var getCSDetails = db.tbl_CMSAP.Where(x => x.fld_CustomerNo == a.KUNNR && x.fld_Desc == a.NAME1 && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
                            var CSCode = db.tbl_CMSAP.Where(x => x.fld_CustomerNo == a.KUNNR && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_CustomerNo).FirstOrDefault();
                            var CSdesc = db.tbl_CMSAP.Where(x => x.fld_CustomerNo == a.KUNNR && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Desc).FirstOrDefault();


                            if (getCSDetails == null)
                            {
                                if (CSCode == null)
                                {

                                    _cmSAP = new tbl_CMSAP();

                                    _cmSAP.fld_CustomerNo = a.KUNNR;
                                    _cmSAP.fld_Desc = a.NAME1;
                                    _cmSAP.fld_NegaraID = NegaraID;
                                    _cmSAP.fld_SyarikatID = SyarikatID;
                                    _cmSAP.fld_DTCreated = DateTime.Today;
                                    _cmSAP.fld_DTModified = DateTime.Today;
                                    _cmSAP.fld_CreatedBy = "SAP";
                                    _cmSAP.fld_CompanyCode = a.BUKRS;

                                    if (a.LOEVM == "")
                                    {
                                        _cmSAP.fld_Deleted = false;
                                    }
                                    else
                                    {
                                        _cmSAP.fld_Deleted = true;
                                    };

                                    db.tbl_CMSAP.Add(_cmSAP);
                                    db.SaveChanges();
                                    db.Entry(_cmSAP).State = EntityState.Detached;
                                }
                                else if (CSCode != null && CSdesc != a.NAME1)
                                {

                                    ModelsCorporate.tbl_CMSAP getCS = db.tbl_CMSAP
                                                .Where(x => x.fld_CustomerNo == a.KUNNR && x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                                    getCS.fld_CustomerNo = a.KUNNR;
                                    getCS.fld_Desc = a.NAME1;
                                    getCS.fld_CompanyCode = a.BUKRS;
                                    getCS.fld_DTModified = DateTime.Today;
                                    getCS.fld_ModifiedBy = "SAP";

                                    if (a.LOEVM == "")
                                    {
                                        getCS.fld_Deleted = false;
                                    }
                                    else
                                    {
                                        getCS.fld_Deleted = true;
                                    };


                                    db.SaveChanges();

                                }
                                else
                                {
                                    ModelsCorporate.tbl_CMSAP getCS = db.tbl_CMSAP
                                                .Where(x => x.fld_CustomerNo == a.KUNNR && x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                                    getCS.fld_CustomerNo = a.KUNNR;
                                    getCS.fld_Desc = a.NAME1;
                                    getCS.fld_CompanyCode = a.BUKRS;
                                    getCS.fld_DTModified = DateTime.Today;
                                    getCS.fld_ModifiedBy = "SAP";

                                    if (a.LOEVM == "")
                                    {
                                        getCS.fld_Deleted = false;
                                    }
                                    else
                                    {
                                        getCS.fld_Deleted = true;
                                    };


                                    db.SaveChanges();
                                }


                                //return if success since sap will return null to any success inbound
                                //save dlm db
                                //tbl_SAPLog.fld_id = "";
                                tbl_SAPLog.fld_type = "S";
                                tbl_SAPLog.fld_message = "CC inbound success";
                                tbl_SAPLog.fld_msg1 = a.KUNNR;
                                tbl_SAPLog.fld_row = Convert.ToString(FPMRespond.IT_CC.Count());
                                tbl_SAPLog.fld_system = "SAP CC";
                                tbl_SAPLog.fld_logDate = DateTime.Now;
                                tbl_SAPLog.fld_msg4 = User.Identity.Name;
                                tbl_SAPLog.fld_negaraID = "1";
                                tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }


                        }


                    }

                    if (FPMRespond.IT_CC.Count() - 1 == 0)
                    {
                        if (FPMRespond.RETURN.Count() - 1 >= 0)
                        {
                            foreach (FPMMD_FTQ.BAPIRET2 a in bAPIRET)
                            {

                                //save dlm db

                                tbl_SAPLog.fld_type = a.TYPE;
                                tbl_SAPLog.fld_number = a.NUMBER;
                                tbl_SAPLog.fld_id = a.ID;
                                tbl_SAPLog.fld_logno = a.LOG_NO;
                                tbl_SAPLog.fld_logmsgno = a.LOG_MSG_NO;
                                tbl_SAPLog.fld_message = a.MESSAGE;
                                tbl_SAPLog.fld_msg1 = a.MESSAGE_V1;
                                tbl_SAPLog.fld_msg2 = a.MESSAGE_V2;
                                tbl_SAPLog.fld_msg3 = a.MESSAGE_V3;
                                tbl_SAPLog.fld_msg4 = a.MESSAGE_V4;
                                tbl_SAPLog.fld_parameter = a.PARAMETER;
                                tbl_SAPLog.fld_row = a.ROW.ToString();
                                tbl_SAPLog.fld_field = a.FIELD;
                                tbl_SAPLog.fld_system = "SAP CS";

                                tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                                tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                                tbl_SAPLog.fld_logDate = DateTime.Now;

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    throw (ex);
                    ViewBag.Message = ex;
                }
                finally
                {


                }

            }
            oClient.Close(); FPMClient.Close();

            //ViewBag.Message = tarikhmula + "|" + tarikhAkhir + "|" + CMStart + "|" + CMEnd;
            //return View("Index");

            return RedirectToAction("cmTodayList");

        }

        public ActionResult cmTodayList()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            DateTime today = DateTime.Today;

            ViewBag.Date = today;

            var result = db.tbl_CMSAP.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && (w.fld_DTCreated == today || w.fld_DTModified == today)).OrderByDescending(o => o.fld_DTModified);

            if (!result.Any())
            {
                ViewBag.Message = "Tiada Record";
                return View();

            }

            return View(result);

        }


        //farahin ubah whole function - 20 May 2023
        public ActionResult IOList(string IOCode, int? WilayahList, int? LadangList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            int[] wlyhid = new int[] { };

            List<SelectListItem> wilayahList = new List<SelectListItem>();

            wilayahList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_WlyhName)
                    .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }),
                "Value", "Text").ToList();
            wilayahList.Insert(0, (new SelectListItem { Text = "SEMUA", Value = "0" }));

            ViewBag.WilayahList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();

            ladangList = new SelectList(
                db.tbl_Ladang
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_LdgName)
                    .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }),
                "Value", "Text").ToList();
            ladangList.Insert(0, (new SelectListItem { Text = "SEMUA", Value = "0" }));

            ViewBag.LadangList = ladangList;

            List<vw_SAPIODetails> result = new List<vw_SAPIODetails>();

            if (String.IsNullOrEmpty(WilayahList.ToString()) || String.IsNullOrEmpty(LadangList.ToString()))
            {
                //All list
                if ((IOCode == null || IOCode == "") && (WilayahList == null) && (LadangList == null))
                {
                    result = db.vw_SAPIODetails.OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }


                }

                //search by IO Code
                else if ((IOCode != null && IOCode != "") && (WilayahList == 0) && (LadangList == 0))
                {
                    result = db.vw_SAPIODetails.Where(w => (w.fld_IOcode.Contains(IOCode))).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }


                }
            }


            //search by wilayah only
            else if ((IOCode == null || IOCode == "") && WilayahList != 0 && (LadangList == 0))
            {
                result = db.vw_SAPIODetails.Where(w => w.fld_WilayahID == WilayahList).OrderByDescending(o => o.fld_DTModified).ToList();
                if (!result.Any())
                {
                    ViewBag.Message = "Tiada Record";
                    return View();

                }
            }

            //search by wilayah & ladang
            else if ((IOCode == null || IOCode == "") && WilayahList != 0 && LadangList != 0)
            {
                result = db.vw_SAPIODetails.Where(w => w.fld_WilayahID == WilayahList && w.fld_LadangID == LadangList).OrderByDescending(o => o.fld_DTModified).ToList();

                if (!result.Any())
                {
                    ViewBag.Message = "Tiada Record";
                    return View();

                }

            }
            else if ((IOCode != null && IOCode != "") && (WilayahList != 0) && (LadangList != 0))
            {
                result = db.vw_SAPIODetails.Where(w => (w.fld_IOcode == IOCode) && w.fld_WilayahID == WilayahList && w.fld_LadangID == LadangList).OrderByDescending(o => o.fld_DTModified).ToList();

                if (!result.Any())
                {
                    ViewBag.Message = "Tiada Record";
                    return View();

                }
            }

            return View();
        }

        //farahin ubah whole function - 20 May 2023
        public ActionResult _IOList(string IOCode, int? WilayahList, int? LadangList)
        {
            List<vw_SAPIODetails> result = new List<vw_SAPIODetails>();

            if (String.IsNullOrEmpty(WilayahList.ToString()) || String.IsNullOrEmpty(LadangList.ToString()))
            {
                //All list
                if ((IOCode == null || IOCode == "") && (WilayahList == null) && (LadangList == null))
                {
                    result = db.vw_SAPIODetails.OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }


                }

                //search by IO Code
                else if ((IOCode != null || IOCode != "") && (WilayahList == 0) && (LadangList == 0))
                {
                    result = db.vw_SAPIODetails.Where(w => (w.fld_IOcode.Contains(IOCode))).OrderByDescending(o => o.fld_DTModified).ToList();

                    if (!result.Any())
                    {
                        ViewBag.Message = "Tiada Record";
                        return View();

                    }


                }
            }


            //search by wilayah only
            else if ((IOCode == null || IOCode == "") && WilayahList != 0 && (LadangList == 0))
            {
                result = db.vw_SAPIODetails.Where(w => w.fld_WilayahID == WilayahList).OrderByDescending(o => o.fld_DTModified).ToList();
                if (!result.Any())
                {
                    ViewBag.Message = "Tiada Record";
                    return View();

                }
            }

            //search by wilayah & ladang
            else if ((IOCode == null || IOCode == "") && WilayahList != 0 && LadangList != 0)
            {
                result = db.vw_SAPIODetails.Where(w => w.fld_WilayahID == WilayahList && w.fld_LadangID == LadangList).OrderByDescending(o => o.fld_DTModified).ToList();

                if (!result.Any())
                {
                    ViewBag.Message = "Tiada Record";
                    return View();

                }

            }
            else if ((IOCode != null || IOCode != "") && (WilayahList != 0) && (LadangList != 0))
            {
                result = db.vw_SAPIODetails.Where(w => (w.fld_IOcode == IOCode) && w.fld_WilayahID == WilayahList && w.fld_LadangID == LadangList).OrderByDescending(o => o.fld_DTModified).ToList();

                if (!result.Any())
                {
                    ViewBag.Message = "Tiada Record";
                    return View();

                }
            }

            return PartialView(result);

        }

        //farahin ubah whole function - 20 May 2023
        public ActionResult ioRequest()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            int[] wlyhid = new int[] { };
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;

            List<SelectListItem> SyarikatList = new List<SelectListItem>();
            SyarikatList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            SyarikatList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.fld_CompanyCode = SyarikatList;

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = "SEMUA", Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "SEMUA", Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                incldg = 1;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text").ToList();

                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 1;
            }

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.fld_WilayahID = WilayahIDList;
            ViewBag.fld_LadangID = LadangIDList;

            return View();
        }

        //farahin ubah whole function - 20 May 2023
        [HttpPost]
        public ActionResult _ioRequest(tbl_SAPLog tbl_SAPLog, tbl_IOSAP _ioSAP, vw_SAPIODetailsCreate _IOSAPCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            string Action;
            string error;
            string CompanyCode = ""; string IOFPM = "";


            string selectLadang = _IOSAPCreate.fld_LadangID.ToString().PadLeft(3, '0');

            string LadangCode = "";
            int? LdgID = 0;

            var GetLadangDetails = db.tbl_Ladang.Where(x => (x.fld_LdgCode == selectLadang) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();

            if (GetLadangDetails != null)
            {
                LadangCode = GetLadangDetails.fld_LdgCode;
                LdgID = GetLadangDetails.fld_ID;
            }
            string today = DateTime.Today.ToString("yyyyMMdd");
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";
            string kodComp = "", IndRanc = "", kodRanc = "", kodPkt = "", kodSubPkt = "", thnPembangunan = "", thnPembangunantanamsemula = "", busArea = "", IO1 = "", IO2 = "", IO3 = "", IO4 = "", IO5 = "", IO6 = "", tkhTanamMulaBhsl = "", PktPembgnn = "", tkhTahapPmbgnn = "", tkhMulaTanam = "", jnsTanaman = "", kodBlok = "", indJnsKiraan = "", jnsBlok = "", jnsKawasan = "", bilPenerokadlmBlok = "", ioFelda = "", ioFPM = "", wbsNo = "";
            decimal bilPeneroka = 0M, bilPenerokaPkt = 0M, jumLuasKeseluruhan = 0M, luasKwsnTanaman = 0M, luasKwsnBhasil = 0M, luasKwsnBhasilFelda = 0M, LuasKwsnBhasilPeneroka = 0M, jumLuasLotLdgFelda = 0M, jumLuasLotLdgPeneroka = 0M, bilKwsnUtama = 0M, bilKwsnRezab = 0M;


            //declaration FELDA
            var oClient = new SAPMD_FLP.ZWS_OPMS_MASTERClient();
            var request = new SAPMD_FLP.ZfmOpmsMaster();
            SAPMD_FLP.ZfmOpmsMasterResponse iresponse = new SAPMD_FLP.ZfmOpmsMasterResponse();

            SAPMD_FLP.Zopmsslp[] zopmsslp = new SAPMD_FLP.Zopmsslp[1];
            SAPMD_FLP.Zopmsslp zopmsslps = new SAPMD_FLP.Zopmsslp();

            SAPMD_FLP.Bapiret2[] bapirtn = new SAPMD_FLP.Bapiret2[1];
            SAPMD_FLP.Bapiret2 bapiret2_return = new SAPMD_FLP.Bapiret2();

            var credential = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "sapCredential");
            string username = "", password = "";

            username = credential.Where(x => x.fldOptConfDesc == "masterdataID").Select(s => s.fldOptConfValue).ToString();
            password = credential.Where(x => x.fldOptConfDesc == "masterdataPWD").Select(s => s.fldOptConfValue).ToString();

            oClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            oClient.ClientCredentials.UserName.Password = "@12345bnm";

            //declaration FPM
            var FPMClient = new FPMMD_FTQ.ZWS_OPMS_MASTERDATAClient();
            var FPMReq = new FPMMD_FTQ.ZFM_OPMS_MASTER();
            FPMMD_FTQ.ZFM_OPMS_MASTERResponse FPMRespond = new FPMMD_FTQ.ZFM_OPMS_MASTERResponse();

            FPMMD_FTQ.ZOPMSIO[] zopmsio = new FPMMD_FTQ.ZOPMSIO[1];
            FPMMD_FTQ.ZOPMSIO zopmsios = new FPMMD_FTQ.ZOPMSIO();

            FPMMD_FTQ.BAPIRET2[] bAPIRET = new FPMMD_FTQ.BAPIRET2[1];
            FPMMD_FTQ.BAPIRET2 bAPIRETs = new FPMMD_FTQ.BAPIRET2();

            FPMClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            FPMClient.ClientCredentials.UserName.Password = "@12345bnm";



            oClient.Open(); FPMClient.Open();

            try
            {

                if (_IOSAPCreate.fld_CompanyCode == "1000")
                {
                    request = new SAPMD_FLP.ZfmOpmsMaster();

                    request.DateBegin = "";
                    request.DateEnd = "";
                    request.SlpComp = "1000";
                    request.SlpIndBegin = "1";
                    request.SlpIndEnd = "9";
                    request.SlpPktBegin = "001";
                    request.SlpPktEnd = "999";
                    request.SlpRanBegin = LadangCode;
                    request.SlpRanEnd = LadangCode;
                    request.ItSlp = zopmsslp;

                    iresponse = oClient.ZfmOpmsMaster(request);

                    bapirtn = iresponse.Return;
                    zopmsslp = iresponse.ItSlp;

                    if (zopmsslp.Count() - 1 >= 0)
                    {
                        foreach (SAPMD_FLP.Zopmsslp a in zopmsslp)
                        {

                            kodComp = a.Zbukrs;
                            IndRanc = a.Zkdrgi;
                            kodRanc = a.Zkdrgn;
                            kodPkt = a.Zkdpkt;
                            kodSubPkt = a.Zkdpk2;
                            thnPembangunan = a.Zthnpb;
                            thnPembangunantanamsemula = a.Zthpts;
                            busArea = a.Zgpcos;
                            IO1 = a.Zpsnd1;
                            IO2 = a.Zpsnd2;
                            IO3 = a.Zpsnd3;
                            IO4 = a.Zpsnd4;
                            IO5 = a.Zpsnd5;
                            IO6 = a.Zpsnd6;
                            tkhTanamMulaBhsl = a.Zthtmb;
                            PktPembgnn = a.Zpkpbg;
                            tkhTahapPmbgnn = a.Zththp;
                            tkhMulaTanam = a.Zthmtm;
                            jnsTanaman = a.Zjstnm;
                            kodBlok = a.Zkdblk;
                            indJnsKiraan = a.Zjenki;
                            jnsBlok = a.Zjsblk;
                            jnsKawasan = a.Zjskws;

                            bilPenerokadlmBlok = a.Zblpr3;
                            bilPeneroka = a.Zblpn2;
                            bilPenerokaPkt = a.Zblot2;
                            jumLuasKeseluruhan = a.Zjmltf;
                            luasKwsnTanaman = a.Zlkwtn;
                            luasKwsnBhasil = a.Zlskbh;
                            luasKwsnBhasilFelda = a.Zlskbf;
                            LuasKwsnBhasilPeneroka = a.Zlskbp;
                            jumLuasLotLdgFelda = a.Zldltf;
                            jumLuasLotLdgPeneroka = a.Zldltp;
                            bilKwsnUtama = a.Zblkwu;
                            bilKwsnRezab = a.Zblkwr;
                            ioFelda = a.Ziofld;
                            ioFPM = a.Ziofpm;
                            //wbsNo = a.ZprpsPosid1;

                            //save db
                            if (a.Zpsnd1 != null && a.Zpsnd1 != "")
                            {
                                var getIODetails = db.tbl_IOSAP.Where(x => x.fld_IOcode == IO1 && x.fld_LadangID == LdgID && x.fld_WilayahID == _IOSAPCreate.fld_WilayahID).FirstOrDefault();

                                if (getIODetails == null)
                                {
                                    _ioSAP = new tbl_IOSAP();

                                    _ioSAP.fld_IOcode = IO1;
                                    _ioSAP.fld_PktCode = kodPkt;
                                    _ioSAP.fld_SubPktCode = kodSubPkt;
                                    _ioSAP.fld_LuasPkt = jumLuasKeseluruhan;
                                    _ioSAP.fld_LuasKawTnmn = luasKwsnTanaman;
                                    _ioSAP.fld_LuasKawBerhasil = luasKwsnBhasil;
                                    _ioSAP.fld_LdgIndicator = IndRanc;
                                    _ioSAP.fld_LdgKod = kodRanc;
                                    //tbl_IOSAP.fld_StatusUsed = "NULL";
                                    _ioSAP.fld_JnsLot = "";
                                    _ioSAP.fld_NegaraID = Convert.ToInt32(NegaraID);
                                    _ioSAP.fld_SyarikatID = Convert.ToInt32(SyarikatID);
                                    _ioSAP.fld_WilayahID = _IOSAPCreate.fld_WilayahID;
                                    _ioSAP.fld_LadangID = LdgID;
                                    _ioSAP.fld_Deleted = false;
                                    _ioSAP.fld_DTCreated = DateTime.Today;
                                    _ioSAP.fld_DTModified = DateTime.Today;
                                    _ioSAP.fld_thnPembangunan = thnPembangunan;
                                    _ioSAP.fld_thnPembangunantanamsemula = thnPembangunantanamsemula;
                                    _ioSAP.fld_busArea = busArea;
                                    _ioSAP.fld_IO2 = IO2;
                                    _ioSAP.fld_IO3 = IO3;
                                    _ioSAP.fld_IO4 = IO4;
                                    _ioSAP.fld_IO5 = IO5;
                                    _ioSAP.fld_IO6 = IO6;
                                    _ioSAP.fld_PktPembgnn = PktPembgnn;
                                    if (tkhTanamMulaBhsl != "0000-00-00")
                                    {


                                    }
                                    else
                                    {

                                    }


                                    if (tkhTahapPmbgnn != "0000-00-00")
                                    {


                                    }
                                    else
                                    {

                                    }
                                    if (tkhMulaTanam != "0000-00-00")
                                    {

                                    }
                                    else
                                    {

                                    }

                                    _ioSAP.fld_jnsTanaman = jnsTanaman;
                                    _ioSAP.fld_kodBlok = kodBlok;
                                    _ioSAP.fld_indJnsKiraan = indJnsKiraan;
                                    _ioSAP.fld_jnsBlok = jnsBlok;
                                    _ioSAP.fld_jnsKawasan = jnsKawasan;
                                    _ioSAP.fld_bilPenerokadlmBlok = Convert.ToInt32(bilPenerokadlmBlok);
                                    _ioSAP.fld_bilPeneroka = Convert.ToInt32(bilPeneroka);
                                    _ioSAP.fld_bilPenerokaPkt = Convert.ToInt32(bilPenerokaPkt);
                                    _ioSAP.fld_luasKwsnBhasilFelda = Convert.ToDecimal(luasKwsnBhasilFelda);
                                    _ioSAP.fld_LuasKwsnBhasilPeneroka = Convert.ToDecimal(LuasKwsnBhasilPeneroka);
                                    _ioSAP.fld_jumLuasLotLdgFelda = Convert.ToDecimal(jumLuasLotLdgFelda);
                                    _ioSAP.fld_jumLuasLotLdgPeneroka = Convert.ToDecimal(jumLuasLotLdgPeneroka);
                                    _ioSAP.fld_bilKwsnUtama = Convert.ToInt32(bilKwsnUtama);
                                    _ioSAP.fld_bilKwsnRezab = Convert.ToInt32(bilKwsnRezab);
                                    _ioSAP.fld_CreatedBy = "SAP";
                                    _ioSAP.fld_ZIOFLD = ioFelda;
                                    _ioSAP.fld_ZIOFPM = ioFPM;
                                    //_ioSAP.fld_WBS = wbsNo;

                                    if (IndRanc == "2" || IndRanc == "5" || IndRanc == "6")
                                    {
                                        _ioSAP.fld_CompanyCode = "8800";
                                    }
                                    else
                                    {
                                        _ioSAP.fld_CompanyCode = "1000";
                                    }

                                    db.tbl_IOSAP.Add(_ioSAP);
                                    db.SaveChanges();
                                    db.Entry(_ioSAP).State = EntityState.Detached;

                                }
                                else
                                {
                                    _ioSAP = new tbl_IOSAP();

                                    _ioSAP.fld_PktCode = kodPkt;
                                    _ioSAP.fld_SubPktCode = kodSubPkt;
                                    _ioSAP.fld_LuasPkt = jumLuasKeseluruhan;
                                    _ioSAP.fld_LuasKawTnmn = luasKwsnTanaman;
                                    _ioSAP.fld_LuasKawBerhasil = luasKwsnBhasil;
                                    _ioSAP.fld_LdgIndicator = IndRanc;
                                    _ioSAP.fld_LdgKod = kodRanc;
                                    //tbl_IOSAP.fld_StatusUsed = "NULL";
                                    _ioSAP.fld_JnsLot = "";
                                    _ioSAP.fld_NegaraID = Convert.ToInt32(NegaraID);
                                    _ioSAP.fld_SyarikatID = Convert.ToInt32(SyarikatID);
                                    _ioSAP.fld_WilayahID = _IOSAPCreate.fld_WilayahID;
                                    _ioSAP.fld_LadangID = LdgID;
                                    _ioSAP.fld_Deleted = false;
                                    _ioSAP.fld_DTModified = DateTime.Today;
                                    _ioSAP.fld_thnPembangunan = thnPembangunan;
                                    _ioSAP.fld_thnPembangunantanamsemula = thnPembangunantanamsemula;
                                    _ioSAP.fld_busArea = busArea;
                                    _ioSAP.fld_IO2 = IO2;
                                    _ioSAP.fld_IO3 = IO3;
                                    _ioSAP.fld_IO4 = IO4;
                                    _ioSAP.fld_IO5 = IO5;
                                    _ioSAP.fld_IO6 = IO6;
                                    _ioSAP.fld_PktPembgnn = PktPembgnn;
                                    if (tkhTanamMulaBhsl != "0000-00-00")
                                    {
                                        if (tkhTanamMulaBhsl == "0 - - ")
                                        {

                                        }
                                        else
                                        {
                                            //_ioSAP.fld_tkhTanamMulaBhsl = Convert.ToDateTime(tkhTanamMulaBhsl);
                                        }

                                    }
                                    else
                                    {
                                        //_ioSAP.fld_tkhTanamMulaBhsl = Convert.ToDateTime(tkhTanamMulaBhsl);
                                    }


                                    if (tkhTahapPmbgnn != "0000-00-00")
                                    {
                                        if (tkhTahapPmbgnn == "0 - - ")
                                        {

                                        }
                                        else
                                        {
                                            //_ioSAP.fld_tkhTahapPmbgnn = Convert.ToDateTime(tkhTahapPmbgnn);
                                        }

                                    }
                                    else
                                    {
                                        //_ioSAP.fld_tkhTahapPmbgnn = Convert.ToDateTime(tkhTahapPmbgnn);
                                    }
                                    if (tkhMulaTanam != "0000-00-00")
                                    {
                                        if (tkhMulaTanam == "0 - - ")
                                        {

                                        }
                                        else
                                        {
                                            //tbl_IOSAP.fld_tkhMulaTanam = Convert.ToDateTime(tkhMulaTanam);
                                        }

                                    }
                                    else
                                    {
                                        //_ioSAP.fld_tkhMulaTanam = Convert.ToDateTime(tkhMulaTanam);
                                    }
                                    _ioSAP.fld_jnsTanaman = jnsTanaman;
                                    _ioSAP.fld_kodBlok = kodBlok;
                                    _ioSAP.fld_indJnsKiraan = indJnsKiraan;
                                    _ioSAP.fld_jnsBlok = jnsBlok;
                                    _ioSAP.fld_jnsKawasan = jnsKawasan;
                                    _ioSAP.fld_bilPenerokadlmBlok = Convert.ToInt32(bilPenerokadlmBlok);
                                    _ioSAP.fld_bilPeneroka = Convert.ToInt32(bilPeneroka);
                                    _ioSAP.fld_bilPenerokaPkt = Convert.ToInt32(bilPenerokaPkt);
                                    _ioSAP.fld_luasKwsnBhasilFelda = Convert.ToDecimal(luasKwsnBhasilFelda);
                                    _ioSAP.fld_LuasKwsnBhasilPeneroka = Convert.ToDecimal(LuasKwsnBhasilPeneroka);
                                    _ioSAP.fld_jumLuasLotLdgFelda = Convert.ToDecimal(jumLuasLotLdgFelda);
                                    _ioSAP.fld_jumLuasLotLdgPeneroka = Convert.ToDecimal(jumLuasLotLdgPeneroka);
                                    _ioSAP.fld_bilKwsnUtama = Convert.ToInt32(bilKwsnUtama);
                                    _ioSAP.fld_bilKwsnRezab = Convert.ToInt32(bilKwsnRezab);

                                    _ioSAP.fld_CreatedBy = "SAP";
                                    _ioSAP.fld_ZIOFLD = ioFelda;
                                    _ioSAP.fld_ZIOFPM = ioFPM;
                                    //_ioSAP.fld_WBS = wbsNo;

                                    if (IndRanc == "2" || IndRanc == "5" || IndRanc == "6")
                                    {
                                        _ioSAP.fld_CompanyCode = "8800";
                                    }
                                    else
                                    {
                                        _ioSAP.fld_CompanyCode = "1000";
                                    }

                                    db.SaveChanges();
                                    db.Entry(_ioSAP).State = EntityState.Detached;
                                }

                                tbl_SAPLog.fld_type = "S";
                                tbl_SAPLog.fld_message = "IO inbound success";
                                tbl_SAPLog.fld_msg1 = IO1;
                                tbl_SAPLog.fld_system = "SLP IO";
                                tbl_SAPLog.fld_logDate = DateTime.Today;
                                tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                                tbl_SAPLog.fld_negaraID = "1";
                                tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                                db.tbl_SAPLog.Add(tbl_SAPLog);
                                db.SaveChanges();
                            }
                        }

                        tbl_SAPLog.fld_type = "S";
                        tbl_SAPLog.fld_message = "IO inbound success";
                        tbl_SAPLog.fld_row = Convert.ToString(iresponse.ItSlp.Count());
                        tbl_SAPLog.fld_system = "SLP IO";
                        tbl_SAPLog.fld_logDate = DateTime.Today;
                        tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                        tbl_SAPLog.fld_negaraID = "1";
                        tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);


                        db.tbl_SAPLog.Add(tbl_SAPLog);
                        db.SaveChanges();

                        //string appname = Request.ApplicationPath;
                        //string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                        //var lang = Request.RequestContext.RouteData.Values["lang"];

                        //if (appname != "/")
                        //{
                        //    domain = domain + appname;
                        //}

                        //return Json(new
                        //{
                        //    success = true,
                        //    msg = "Data successfully import from SAP.",
                        //    status = "success",
                        //    checkingdata = "0",
                        //    method = "1",
                        //    div = "IOList",
                        //    rootUrl = domain,
                        //    action = "IOList",
                        //    controller = "SAPMasterData"
                        //});
                    }

                    if (iresponse.Return.Count() - 1 >= 1)
                    {
                        foreach (SAPMD_FLP.Bapiret2 a in bapirtn)
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
                            tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                            tbl_SAPLog.fld_parameter = parameter;
                            tbl_SAPLog.fld_row = row;
                            tbl_SAPLog.fld_field = field;
                            tbl_SAPLog.fld_system = "SLP IO";

                            tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                            tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                            tbl_SAPLog.fld_logDate = DateTime.Today;

                            db.tbl_SAPLog.Add(tbl_SAPLog);
                            db.SaveChanges();
                        }

                    }
                }
                else if (_IOSAPCreate.fld_CompanyCode == "8800")
                {
                    //FPM

                    FPMReq = new FPMMD_FTQ.ZFM_OPMS_MASTER();
                    FPMReq.DATE_BEGIN = "";
                    FPMReq.DATE_END = "";
                    FPMReq.ORDERID_BEGIN = _IOSAPCreate.fld_IOCodeBegin;
                    FPMReq.ORDERID_END = _IOSAPCreate.fld_IOCodeEnd;
                    FPMReq.IT_IO = zopmsio;

                    FPMRespond = FPMClient.ZFM_OPMS_MASTER(FPMReq);

                    bAPIRET = FPMRespond.RETURN;
                    zopmsio = FPMRespond.IT_IO;

                    if (zopmsio.Count() - 1 >= 0)
                    {
                        foreach (FPMMD_FTQ.ZOPMSIO a in zopmsio)
                        {
                            var getIODetails = db.tbl_IOSAP.Where(x => x.fld_ZIOFPM == a.AUFNR && x.fld_LadangID == LdgID && x.fld_WilayahID == _IOSAPCreate.fld_WilayahID).FirstOrDefault();

                            if (getIODetails == null)
                            {
                                _ioSAP = new tbl_IOSAP();

                                _ioSAP.fld_CompanyCode = a.BUKRS;
                                _ioSAP.fld_ZIOFPM = a.AUFNR;
                                _ioSAP.fld_LdgKod = LadangCode;
                                _ioSAP.fld_NegaraID = Convert.ToInt32(NegaraID);
                                _ioSAP.fld_SyarikatID = Convert.ToInt32(SyarikatID);
                                _ioSAP.fld_WilayahID = _IOSAPCreate.fld_WilayahID;
                                _ioSAP.fld_LadangID = LdgID;
                                _ioSAP.fld_Deleted = false;
                                _ioSAP.fld_DTCreated = DateTime.Today;
                                _ioSAP.fld_DTModified = DateTime.Today;


                                db.tbl_IOSAP.Add(_ioSAP);
                                db.SaveChanges();
                                db.Entry(_ioSAP).State = EntityState.Detached;

                            }
                            else
                            {
                                _ioSAP = new tbl_IOSAP();

                                _ioSAP.fld_CompanyCode = a.BUKRS;
                                _ioSAP.fld_ZIOFPM = a.AUFNR;
                                _ioSAP.fld_LdgKod = LadangCode;
                                _ioSAP.fld_NegaraID = Convert.ToInt32(NegaraID);
                                _ioSAP.fld_SyarikatID = Convert.ToInt32(SyarikatID);
                                _ioSAP.fld_WilayahID = _IOSAPCreate.fld_WilayahID;
                                _ioSAP.fld_LadangID = LdgID;
                                _ioSAP.fld_Deleted = false;
                                _ioSAP.fld_DTCreated = DateTime.Today;
                                _ioSAP.fld_DTModified = DateTime.Today;

                                db.SaveChanges();
                                db.Entry(_ioSAP).State = EntityState.Detached;
                            }

                            tbl_SAPLog.fld_type = "S";
                            tbl_SAPLog.fld_message = "IO inbound success";
                            tbl_SAPLog.fld_msg1 = _ioSAP.fld_ZIOFPM;
                            tbl_SAPLog.fld_system = "SAP IO";
                            tbl_SAPLog.fld_logDate = DateTime.Today;
                            tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                            tbl_SAPLog.fld_negaraID = "1";
                            tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                            db.tbl_SAPLog.Add(tbl_SAPLog);
                            db.SaveChanges();
                        }
                    }

                    tbl_SAPLog.fld_type = "S";
                    tbl_SAPLog.fld_message = "IO inbound success";
                    tbl_SAPLog.fld_row = Convert.ToString(FPMRespond.IT_IO.Count());
                    tbl_SAPLog.fld_system = "SAP IO";
                    tbl_SAPLog.fld_logDate = DateTime.Today;
                    tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                    tbl_SAPLog.fld_negaraID = "1";
                    tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);


                    db.tbl_SAPLog.Add(tbl_SAPLog);
                    db.SaveChanges();


                }

            }
            catch (Exception ex)
            {
                tbl_SAPLog.fld_type = type;
                tbl_SAPLog.fld_number = number;
                tbl_SAPLog.fld_id = id;
                tbl_SAPLog.fld_logno = logno;
                tbl_SAPLog.fld_logmsgno = logmsgno;
                tbl_SAPLog.fld_message = Convert.ToString(ex);
                tbl_SAPLog.fld_msg1 = LadangCode;
                tbl_SAPLog.fld_msg2 = message2;
                tbl_SAPLog.fld_msg3 = message3;
                tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                tbl_SAPLog.fld_parameter = parameter;
                tbl_SAPLog.fld_row = row;
                tbl_SAPLog.fld_field = field;
                tbl_SAPLog.fld_system = "SLP IO";

                tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                tbl_SAPLog.fld_logDate = DateTime.Today;


                db.tbl_SAPLog.Add(tbl_SAPLog);
                db.SaveChanges();


                //farahin tambah - 10/01/2023

                //ViewBag.error = tbl_SAPLog.fld_message;
                string msg = ex.InnerException.ToString().Substring(63, 3);

                if (msg == "401")
                {
                    Action = "errorView";
                    ViewBag.msg = "Error 401: System Credential blocked. Kindly contact SAP basis team to unblock.";
                }
            }
            finally
            {
                oClient.Close(); FPMClient.Close();

                if (tbl_SAPLog.fld_type == "S")
                {
                    Action = "ioTodayList";
                }
                else
                {

                    Action = "errorView";

                }
            }

            return RedirectToAction(Action);
        }

        //farahin ubah whole function - 20 May 2023
        public ActionResult ioTodayList()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            DateTime today = DateTime.Today;

            List<vw_SAPIODetails> result = new List<vw_SAPIODetails>();

            result = db.vw_SAPIODetails.Where(w => w.fld_DTCreated == today || w.fld_DTModified == today).OrderByDescending(o => o.fld_DTModified).ToList();

            if (!result.Any())
            {
                ViewBag.Message = "Tiada Record";
                return View();

            }


            return View(result);
        }

        public ActionResult errorView()
        {

            //farahin modify - 10/01/2023

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            DateTime today = DateTime.Today;
            string type = "S";
            List<tbl_SAPLog> result = new List<tbl_SAPLog>();

            result = db.tbl_SAPLog.Where(w => w.fld_logDate == today && w.fld_type != type && w.fld_system == "SLP IO").OrderByDescending(o => o.fld_logDate).ToList();

            //if (!result.Any())
            //{

            //    return View();

            //}


            return View(result);
        }
    }
}