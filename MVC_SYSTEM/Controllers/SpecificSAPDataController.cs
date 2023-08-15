using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
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
using iTextSharp.text;
using iTextSharp.text.pdf;
using MVC_SYSTEM.AuthModels;
using iTextSharp.tool.xml;
using System.IO;





namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin, Super Admin, Admin 1")]

    public class SpecificSAPDataController : Controller
    {

        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Models db3 = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetConfig GetConfig = new GetConfig();
        private GetNSWL GetNSWL = new GetNSWL();
        GetWilayah getwilyah = new GetWilayah();
        Connection Connection = new Connection();

        string tarikhmula, tarikhAkhir, CMStart, CMEnd, GLStart, GLEnd, VDStart, VDEnd, CCStart, CCEnd, compCode;

        //structure request
        /* Pull data by tarikh
         * DateBegin = "20201001"
         * DateEnd = "20201031"
         * GLBegin = "ALL"
         * GLEnd = ""
         * Glcomp = "1000"
         * 
         * Pull data by GL
         * DateBegin = ""
         * DateEnd = ""
         * GLBegin = "0700000000" / GLBegin = "0700000010"
         * GLEnd = "07999999999" / GLEnd = "0700000010"
         * GlComp = "1000"
         */

        // GET: SpecificSAPData
        public ActionResult Index()
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
            wilayahList.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "0" });
            ViewBag.WilayahIDList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();
            //    ladangList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            ViewBag.LadangIDList = ladangList;


            List<SelectListItem> SyarikatList = new List<SelectListItem>();
            SyarikatList = new SelectList(
                db.tbl_Syarikat
                    .Where(x => x.fld_SAPComCode != null)
                    .Select(
                            s => new SelectListItem { Value = s.fld_SAPComCode.ToString(), Text = s.fld_SAPComCode }), "Value", "Text").ToList();

            ViewBag.glsyarikatList = SyarikatList;
            ViewBag.ccsyarikatList = SyarikatList;
            ViewBag.cmsyarikatList = SyarikatList;
            ViewBag.vdsyarikatList = SyarikatList;
            ViewBag.slpsyarikatList = SyarikatList;

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;


            return View();
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
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LdgCode.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LdgCode.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }


        public ActionResult _GLListing(string print)
        {
            var result = db.tbl_GLSAP.OrderByDescending(o => o.fld_DTModified);
            return View(result);
        }

        [HttpPost]
        public ActionResult GLRequest(tbl_SAPLog tbl_SAPLog, tbl_GLSAP _glSAP)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //string tarikhmula, tarikhAkhir, GLStart, GLEnd, compCode;

            compCode = Request["glsyarikatList"];


            //if (Request["dateStart"].ToString() == "")
            //{
            //    tarikhmula = "";
            //}
            //else
            //{
            //    tarikhmula = Request["dateStart"].ToString();
            //}

            //if (Request["dateEnd"].ToString() == "")
            //{
            //    tarikhAkhir = tarikhmula;
            //}
            //else
            //{
            //    tarikhAkhir = Request["dateEnd"].ToString();
            //}


            if (Request["glStart"].ToString() == "")
            {
                GLStart = "ALL";
            }
            else
            {
                GLStart = Request["glStart"].ToString();
            }


            if (Request["glEnd"].ToString() == "")
            {
                if (GLStart == "ALL")
                {
                    GLEnd = "";
                }
                else
                {
                    GLEnd = GLStart;
                }
            }
            else
            {
                GLEnd = Request["glEnd"].ToString();
            }



            // ViewBag.Message = tarikhmula + "|" + tarikhAkhir + "|" + GLStart + "|" + GLEnd;
            string bukrs = "", saknr = "", txt50 = "", xloeb = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";
            string exception = "";

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

            oClient.Open();
            try
            {

                request = new SAPMD_FLP.ZfmOpmsMaster();

                //request.DateBegin = tarikhmula;
                //request.DateEnd = tarikhAkhir;
                request.DateBegin = "";
                request.DateEnd = "";
                request.GlBegin = GLStart;
                request.GlEnd = GLEnd;
                request.GlComp = compCode;
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
                            else if (glCode != null && gldesc != txt50)
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

            return View("Index");

        }

        public ActionResult _CCListing(string print)
        {
            var result = db.tbl_CCSAP.OrderByDescending(o => o.fld_DTModified);
            return View(result);
        }

        [HttpPost]
        public ActionResult ccRequest(ModelsCorporate.tbl_SAPLog tbl_SAPLog, ModelsCorporate.tbl_CCSAP _ccSAP)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            compCode = Request["ccsyarikatList"];

            //if (Request["dateStart"].ToString() == null)
            //{
            //    tarikhmula = "";
            //}
            //else
            //{
            //    tarikhmula = Request["dateStart"].ToString();
            //}

            //if (Request["dateEnd"].ToString() == "")
            //{
            //    tarikhAkhir = tarikhmula;
            //}
            //else
            //{
            //    tarikhAkhir = Request["dateEnd"].ToString();
            //}

            if (Request["ccStart"].ToString() == "")
            {
                CCStart = "ALL";
            }
            else
            {
                CCStart = Request["ccStart"].ToString();
            }


            if (Request["ccEnd"].ToString() == "")
            {
                if (CCStart == "ALL")
                {
                    CCEnd = "";
                }
                else
                {
                    CCEnd = CCStart;
                }
            }
            else
            {
                CCEnd = Request["ccEnd"].ToString();
            }


            //ViewBag.Message = tarikhmula + "|" + tarikhAkhir + "|" + CCStart + "|" + CCEnd;

            string today = DateTime.Now.ToString("yyyyMMdd");
            string KOKRS = "", KOSTL = "", LTEXT = "", BKZKP = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";

            var oClient = new SAPMD_FLP.ZWS_OPMS_MASTERClient();
            var request = new SAPMD_FLP.ZfmOpmsMaster();
            SAPMD_FLP.ZfmOpmsMasterResponse iresponse = new SAPMD_FLP.ZfmOpmsMasterResponse();

            SAPMD_FLP.Zopmscc[] zopmscc = new SAPMD_FLP.Zopmscc[1];
            SAPMD_FLP.Zopmscc zopmsccs = new SAPMD_FLP.Zopmscc();

            SAPMD_FLP.Bapiret2[] bapirtn = new SAPMD_FLP.Bapiret2[1];
            SAPMD_FLP.Bapiret2 bapiret2_return = new SAPMD_FLP.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "WF-BATCH";
            oClient.ClientCredentials.UserName.Password = "init1234";

            oClient.Open();

            try
            {
                request = new SAPMD_FLP.ZfmOpmsMaster();

                //request.DateBegin = tarikhmula;
                //request.DateEnd = tarikhAkhir;
                request.DateBegin = "";
                request.DateEnd = "";
                request.CcBegin = CCStart;
                request.CcEnd = CCEnd;
                request.CcComp = compCode;
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
                        var getCCDetails = db.tbl_CCSAP.Where(x => x.fld_CstCnter == KOSTL && x.fld_Desc == LTEXT).FirstOrDefault();
                        var Code = db.tbl_CCSAP.Where(x => x.fld_CstCnter == KOSTL).Select(s => s.fld_CstCnter).FirstOrDefault();
                        var desc = db.tbl_CCSAP.Where(x => x.fld_CstCnter == KOSTL).Select(s => s.fld_Desc).FirstOrDefault();


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
                oClient.Close();
            }
            return View("Index");
        }

        public ActionResult _CMListing(string print)
        {
            var result = db.tbl_CMSAP.OrderByDescending(o => o.fld_DTModified);
            return View(result);
        }

        [HttpPost]
        public ActionResult cmRequest(ModelsCorporate.tbl_SAPLog tbl_SAPLog, ModelsCorporate.tbl_CMSAP _custSAP)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            compCode = Request["cmsyarikatList"];

            //if (Request["dateStart"].ToString() == "")
            //{
            //    tarikhmula = "";
            //}
            //else
            //{
            //    tarikhmula = Request["dateStart"].ToString();
            //}

            //if (Request["dateEnd"].ToString() == "")
            //{
            //    tarikhAkhir = tarikhmula;
            //}
            //else
            //{
            //    tarikhAkhir = Request["dateEnd"].ToString();
            //}


            if (Request["cmStart"].ToString() == "")
            {
                CMStart = "ALL";
            }
            else
            {
                CMStart = Request["cmStart"].ToString();
            }


            if (Request["cmEnd"].ToString() == "")
            {
                if (CMStart == "ALL")
                {
                    CMEnd = "";
                }
                else
                {
                    CMEnd = CMStart;
                }
            }
            else
            {
                CMEnd = Request["cmEnd"].ToString();
            }
            string bukrs = "", kunnr = "", name1 = "", loevm = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";

            var oClient = new SAPMD_FLP.ZWS_OPMS_MASTERClient();
            var request = new SAPMD_FLP.ZfmOpmsMaster();
            SAPMD_FLP.ZfmOpmsMasterResponse iresponse = new SAPMD_FLP.ZfmOpmsMasterResponse();

            SAPMD_FLP.Zopmscs[] zopmscs = new SAPMD_FLP.Zopmscs[1];
            SAPMD_FLP.Zopmscs zopmscss = new SAPMD_FLP.Zopmscs();

            SAPMD_FLP.Bapiret2[] bapirtn = new SAPMD_FLP.Bapiret2[1];
            SAPMD_FLP.Bapiret2 bapiret2_return = new SAPMD_FLP.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "WF-BATCH";
            oClient.ClientCredentials.UserName.Password = "init1234";

            oClient.Open();

            try
            {
                request = new SAPMD_FLP.ZfmOpmsMaster();

                //request.DateBegin = tarikhmula;
                //request.DateEnd = tarikhAkhir;
                request.DateBegin = "";
                request.DateEnd = "";
                request.CsBegin = CMStart;
                request.CsEnd = CMEnd;
                request.CsComp = compCode;
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

                                _custSAP = new tbl_CMSAP();

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
                oClient.Close();
            }

            //ViewBag.Message = tarikhmula + "|" + tarikhAkhir + "|" + CMStart + "|" + CMEnd;
            return View("Index");
        }


        public ActionResult _VDListing(string print)
        {
            var result = db.tbl_VDSAP.OrderByDescending(o => o.fld_DTModified);
            return View(result);
        }


        [HttpPost]
        public ActionResult vdRequest(ModelsCorporate.tbl_SAPLog tbl_SAPLog, ModelsCorporate.tbl_VDSAP _vdSAP)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            compCode = Request["vdsyarikatList"];

            //if (Request["dateStart"].ToString() == null)
            //{
            //    tarikhmula = "";
            //}
            //else
            //{
            //    tarikhmula = Request["dateStart"].ToString();
            //}

            //if (Request["dateEnd"].ToString() == "")
            //{
            //    tarikhAkhir = tarikhmula;
            //}
            //else
            //{
            //    tarikhAkhir = Request["dateEnd"].ToString();
            //}


            if (Request["vdStart"].ToString() == "")
            {
                VDStart = "ALL";
            }
            else
            {
                VDStart = Request["vdStart"].ToString();
            }


            if (Request["vdEnd"].ToString() == "")
            {
                if (VDStart == "ALL")
                {
                    VDEnd = "";
                }
                else
                {
                    VDEnd = VDStart;
                }
            }
            else
            {
                VDEnd = Request["vdEnd"].ToString();
            }

            string today = DateTime.Now.ToString("yyyyMMdd");
            string bukrs = "", lifnr = "", name1 = "", loevm = "";
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";

            var oClient = new SAPMD_FLP.ZWS_OPMS_MASTERClient();
            var request = new SAPMD_FLP.ZfmOpmsMaster();
            SAPMD_FLP.ZfmOpmsMasterResponse iresponse = new SAPMD_FLP.ZfmOpmsMasterResponse();

            SAPMD_FLP.Zopmsvd[] zopmsvd = new SAPMD_FLP.Zopmsvd[1];
            SAPMD_FLP.Zopmsvd zopmsvds = new SAPMD_FLP.Zopmsvd();

            SAPMD_FLP.Bapiret2[] bapirtn = new SAPMD_FLP.Bapiret2[1];
            SAPMD_FLP.Bapiret2 bapiret2_return = new SAPMD_FLP.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "WF-BATCH";
            oClient.ClientCredentials.UserName.Password = "init1234";

            oClient.Open();

            try
            {
                request = new SAPMD_FLP.ZfmOpmsMaster();

                //request.DateBegin = tarikhmula;
                //request.DateEnd = tarikhAkhir;
                request.DateBegin = "";
                request.DateEnd = "";
                request.VdBegin = VDStart;
                request.VdEnd = VDStart;
                request.VdComp = compCode;
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
                        var vdCode = db.tbl_VDSAP.Where(x => x.fld_VendorNo == lifnr).Select(s => s.fld_VendorNo).FirstOrDefault();
                        var vdDesc = db.tbl_VDSAP.Where(x => x.fld_VendorNo == lifnr).Select(s => s.fld_Desc).FirstOrDefault();

                        if (getVDDetails == null)
                        {
                            if (vdCode == null)
                            {

                                _vdSAP = new tbl_VDSAP();

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
                oClient.Close();
            }


            return View("Index");
        }

        public ActionResult _IOListing(string print)
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
            //   wilayahList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            ViewBag.WilayahIDList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();
            //    ladangList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            ViewBag.LadangIDList = ladangList;

            var result = db.vw_SAPIODetails.OrderByDescending(o => o.fld_DTModified);
            return View(result);
        }
        [HttpPost]
        public ActionResult SLPRequest(ModelsCorporate.tbl_SAPLog tbl_SAPLog, ModelsCorporate.tbl_IOSAP tbl_IOSAP)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            compCode = Request["slpsyarikatList"];

            string selectLadang = Request["LadangIDList"].ToString();

            string LadangCode = "";

            var GetLadangDetails = db.tbl_Ladang.Where(x => x.fld_LdgCode == selectLadang && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();

            LadangCode = GetLadangDetails.fld_LdgCode;


            string today = DateTime.Now.ToString("yyyyMMdd");
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";
            string kodComp = "", IndRanc = "", kodRanc = "", kodPkt = "", kodSubPkt = "", thnPembangunan = "", thnPembangunantanamsemula = "", busArea = "", IO1 = "", IO2 = "", IO3 = "", IO4 = "", IO5 = "", IO6 = "", tkhTanamMulaBhsl = "", PktPembgnn = "", tkhTahapPmbgnn = "", tkhMulaTanam = "", jnsTanaman = "", kodBlok = "", indJnsKiraan = "", jnsBlok = "", jnsKawasan = "", bilPenerokadlmBlok = "";
            decimal bilPeneroka = 0M, bilPenerokaPkt = 0M, jumLuasKeseluruhan = 0M, luasKwsnTanaman = 0M, luasKwsnBhasil = 0M, luasKwsnBhasilFelda = 0M, LuasKwsnBhasilPeneroka = 0M, jumLuasLotLdgFelda = 0M, jumLuasLotLdgPeneroka = 0M, bilKwsnUtama = 0M, bilKwsnRezab = 0M;



            var oClient = new SAPMD_FLP.ZWS_OPMS_MASTERClient();
            var request = new SAPMD_FLP.ZfmOpmsMaster();
            SAPMD_FLP.ZfmOpmsMasterResponse iresponse = new SAPMD_FLP.ZfmOpmsMasterResponse();

            SAPMD_FLP.Zopmsslp[] zopmsslp = new SAPMD_FLP.Zopmsslp[1];
            SAPMD_FLP.Zopmsslp zopmsslps = new SAPMD_FLP.Zopmsslp();

            SAPMD_FLP.Bapiret2[] bapirtn = new SAPMD_FLP.Bapiret2[1];
            SAPMD_FLP.Bapiret2 bapiret2_return = new SAPMD_FLP.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "WF-BATCH";
            oClient.ClientCredentials.UserName.Password = "init1234";

            oClient.Open();

            try
            {
                request = new SAPMD_FLP.ZfmOpmsMaster();

                request.DateBegin = "";
                request.DateEnd = "";
                request.SlpComp = compCode;
                request.SlpIndBegin = "3";
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

                        //save db
                        var getIODetails = db.tbl_IOSAP.Where(x => x.fld_IOcode == IO1).FirstOrDefault();

                        if (getIODetails == null)
                        {
                            tbl_IOSAP = new tbl_IOSAP();

                            tbl_IOSAP.fld_IOcode = IO1;
                            tbl_IOSAP.fld_PktCode = kodPkt;
                            tbl_IOSAP.fld_SubPktCode = kodSubPkt;
                            tbl_IOSAP.fld_LuasPkt = jumLuasKeseluruhan;
                            tbl_IOSAP.fld_LuasKawTnmn = luasKwsnTanaman;
                            tbl_IOSAP.fld_LuasKawBerhasil = luasKwsnBhasil;
                            tbl_IOSAP.fld_LdgIndicator = IndRanc;
                            tbl_IOSAP.fld_LdgKod = kodRanc;
                            //tbl_IOSAP.fld_StatusUsed = "NULL";
                            tbl_IOSAP.fld_JnsLot = "";
                            tbl_IOSAP.fld_NegaraID = Convert.ToInt32(NegaraID);
                            tbl_IOSAP.fld_SyarikatID = Convert.ToInt32(SyarikatID);
                            tbl_IOSAP.fld_WilayahID = Convert.ToInt32(WilayahID);
                            tbl_IOSAP.fld_LadangID = Convert.ToInt32(LadangID);
                            tbl_IOSAP.fld_Deleted = false;
                            tbl_IOSAP.fld_DTCreated = DateTime.Now;
                            tbl_IOSAP.fld_DTModified = DateTime.Now;
                            tbl_IOSAP.fld_thnPembangunan = thnPembangunan;
                            tbl_IOSAP.fld_thnPembangunantanamsemula = thnPembangunantanamsemula;
                            tbl_IOSAP.fld_busArea = busArea;
                            tbl_IOSAP.fld_IO2 = IO2;
                            tbl_IOSAP.fld_IO3 = IO3;
                            tbl_IOSAP.fld_IO4 = IO4;
                            tbl_IOSAP.fld_IO5 = IO5;
                            tbl_IOSAP.fld_IO6 = IO6;
                            if (tkhTanamMulaBhsl != "0000-00-00")
                            {
                                tbl_IOSAP.fld_tkhTanamMulaBhsl = Convert.ToDateTime(tkhTanamMulaBhsl);
                            }

                            tbl_IOSAP.fld_PktPembgnn = PktPembgnn;
                            if (tkhTahapPmbgnn != "0000-00-00")
                            {
                                tbl_IOSAP.fld_tkhTahapPmbgnn = Convert.ToDateTime(tkhTahapPmbgnn);
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



                            tbl_IOSAP.fld_jnsTanaman = jnsTanaman;
                            tbl_IOSAP.fld_kodBlok = kodBlok;
                            tbl_IOSAP.fld_indJnsKiraan = indJnsKiraan;
                            tbl_IOSAP.fld_jnsBlok = jnsBlok;
                            tbl_IOSAP.fld_jnsKawasan = jnsKawasan;
                            tbl_IOSAP.fld_bilPenerokadlmBlok = Convert.ToInt32(bilPenerokadlmBlok);
                            tbl_IOSAP.fld_bilPeneroka = Convert.ToInt32(bilPeneroka);
                            tbl_IOSAP.fld_bilPenerokaPkt = Convert.ToInt32(bilPenerokaPkt);
                            tbl_IOSAP.fld_luasKwsnBhasilFelda = Convert.ToDecimal(luasKwsnBhasilFelda);
                            tbl_IOSAP.fld_LuasKwsnBhasilPeneroka = Convert.ToDecimal(LuasKwsnBhasilPeneroka);
                            tbl_IOSAP.fld_jumLuasLotLdgFelda = Convert.ToDecimal(jumLuasLotLdgFelda);
                            tbl_IOSAP.fld_jumLuasLotLdgPeneroka = Convert.ToDecimal(jumLuasLotLdgPeneroka);
                            tbl_IOSAP.fld_bilKwsnUtama = Convert.ToInt32(bilKwsnUtama);
                            tbl_IOSAP.fld_bilKwsnRezab = Convert.ToInt32(bilKwsnRezab);

                            db.tbl_IOSAP.Add(tbl_IOSAP);
                            db.SaveChanges();
                            db.Entry(tbl_IOSAP).State = EntityState.Detached;


                            tbl_SAPLog.fld_type = "S";
                            tbl_SAPLog.fld_message = "IO inbound success";
                            tbl_SAPLog.fld_msg1 = IO1;
                            tbl_SAPLog.fld_system = "SLP IO";
                            tbl_SAPLog.fld_logDate = DateTime.Now;
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
                    tbl_SAPLog.fld_logDate = DateTime.Now;
                    tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                    tbl_SAPLog.fld_negaraID = "1";
                    tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                    db.tbl_SAPLog.Add(tbl_SAPLog);
                    db.SaveChanges();
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
                        tbl_SAPLog.fld_msg4 = message4;
                        tbl_SAPLog.fld_parameter = parameter;
                        tbl_SAPLog.fld_row = row;
                        tbl_SAPLog.fld_field = field;
                        tbl_SAPLog.fld_system = "SLP IO";

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
                tbl_SAPLog.fld_type = type;
                tbl_SAPLog.fld_number = number;
                tbl_SAPLog.fld_id = id;
                tbl_SAPLog.fld_logno = logno;
                tbl_SAPLog.fld_logmsgno = logmsgno;
                tbl_SAPLog.fld_message = Convert.ToString(ex);
                tbl_SAPLog.fld_msg1 = LadangCode;
                tbl_SAPLog.fld_msg2 = message2;
                tbl_SAPLog.fld_msg3 = message3;
                tbl_SAPLog.fld_msg4 = message4;
                tbl_SAPLog.fld_parameter = parameter;
                tbl_SAPLog.fld_row = row;
                tbl_SAPLog.fld_field = field;
                tbl_SAPLog.fld_system = "SLP IO";

                tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                tbl_SAPLog.fld_logDate = DateTime.Now;

                db.tbl_SAPLog.Add(tbl_SAPLog);
                db.SaveChanges();

            }
            finally
            {
                oClient.Close();
            }

            return View("Index");

            //List<SelectListItem> wilayahList = new List<SelectListItem>();
            //wilayahList = new SelectList(
            //    db.tbl_Wilayah
            //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
            //        .Select(
            //            s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            ////   wilayahList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            //ViewBag.WilayahIDList = wilayahList;

            //List<SelectListItem> ladangList = new List<SelectListItem>();
            ////    ladangList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            //ViewBag.LadangIDList = ladangList;

            //return RedirectToAction("_IOListing");

        }


        //function pdf GL
        [HttpPost]
        public ActionResult ConvertPDF(string myHtml, string filename, string reportname)
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

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF", "SpecificSAPData", null, "http") + "/" + tblHtmlReport.fldID });
        }

        public ActionResult GetPDF(int id)
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

        //function pdf CC
        //[HttpPost]
        //public ActionResult ConvertPDF2(string myHtml, string filename, string reportname)
        //{
        //    bool success = false;
        //    string msg = "";
        //    string status = "";
        //    Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

        //    tblHtmlReport.fldHtlmCode = myHtml;
        //    tblHtmlReport.fldFileName = filename;
        //    tblHtmlReport.fldReportName = reportname;

        //    db3.tblHtmlReports.Add(tblHtmlReport);
        //    db3.SaveChanges();

        //    success = true;
        //    status = "success";

        //    return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF2", "SpecificSAPData", null, "http") + "/" + tblHtmlReport.fldID });
        //}

        //public ActionResult GetPDF2(int id)
        //{
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string width = "1700", height = "1190";
        //    string imagepath = Server.MapPath("~/Asset/Images/");

        //    var gethtml = db3.tblHtmlReports.Find(id);

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();


        //    Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

        //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //    pdfDoc.Open();

        //    using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
        //    {
        //        using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
        //        {
        //            htmlWorker.Open();
        //            htmlWorker.Parse(sr);
        //        }
        //    }
        //    pdfDoc.Close();
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdfDoc);
        //    Response.End();

        //    db.Entry(gethtml).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    return View();
        //}

        ////function pdf CM
        //[HttpPost]
        //public ActionResult ConvertPDF3(string myHtml, string filename, string reportname)
        //{
        //    bool success = false;
        //    string msg = "";
        //    string status = "";
        //    Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

        //    tblHtmlReport.fldHtlmCode = myHtml;
        //    tblHtmlReport.fldFileName = filename;
        //    tblHtmlReport.fldReportName = reportname;

        //    db3.tblHtmlReports.Add(tblHtmlReport);
        //    db3.SaveChanges();

        //    success = true;
        //    status = "success";

        //    return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF3", "SpecificSAPData", null, "http") + "/" + tblHtmlReport.fldID });
        //}

        //public ActionResult GetPDF3(int id)
        //{
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string width = "1700", height = "1190";
        //    string imagepath = Server.MapPath("~/Asset/Images/");

        //    var gethtml = db3.tblHtmlReports.Find(id);

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();


        //    Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

        //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //    pdfDoc.Open();

        //    using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
        //    {
        //        using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
        //        {
        //            htmlWorker.Open();
        //            htmlWorker.Parse(sr);
        //        }
        //    }
        //    pdfDoc.Close();
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdfDoc);
        //    Response.End();

        //    db.Entry(gethtml).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    return View();
        //}

        ////function pdf VD
        //[HttpPost]
        //public ActionResult ConvertPDF4(string myHtml, string filename, string reportname)
        //{
        //    bool success = false;
        //    string msg = "";
        //    string status = "";
        //    Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

        //    tblHtmlReport.fldHtlmCode = myHtml;
        //    tblHtmlReport.fldFileName = filename;
        //    tblHtmlReport.fldReportName = reportname;

        //    db3.tblHtmlReports.Add(tblHtmlReport);
        //    db3.SaveChanges();

        //    success = true;
        //    status = "success";

        //    return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF4", "SpecificSAPData", null, "http") + "/" + tblHtmlReport.fldID });
        //}

        //public ActionResult GetPDF4(int id)
        //{
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string width = "1700", height = "1190";
        //    string imagepath = Server.MapPath("~/Asset/Images/");

        //    var gethtml = db3.tblHtmlReports.Find(id);

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();


        //    Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

        //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //    pdfDoc.Open();

        //    using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
        //    {
        //        using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
        //        {
        //            htmlWorker.Open();
        //            htmlWorker.Parse(sr);
        //        }
        //    }
        //    pdfDoc.Close();
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdfDoc);
        //    Response.End();

        //    db.Entry(gethtml).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    return View();
        //}

        ////function pdf IO
        //[HttpPost]
        //public ActionResult ConvertPDF5(string myHtml, string filename, string reportname)
        //{
        //    bool success = false;
        //    string msg = "";
        //    string status = "";
        //    Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

        //    tblHtmlReport.fldHtlmCode = myHtml;
        //    tblHtmlReport.fldFileName = filename;
        //    tblHtmlReport.fldReportName = reportname;

        //    db3.tblHtmlReports.Add(tblHtmlReport);
        //    db3.SaveChanges();

        //    success = true;
        //    status = "success";

        //    return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF5", "SpecificSAPData", null, "http") + "/" + tblHtmlReport.fldID });
        //}

        //public ActionResult GetPDF5(int id)
        //{
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string width = "1700", height = "1190";
        //    string imagepath = Server.MapPath("~/Asset/Images/");

        //    var gethtml = db3.tblHtmlReports.Find(id);

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();


        //    Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

        //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //    pdfDoc.Open();

        //    using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
        //    {
        //        using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
        //        {
        //            htmlWorker.Open();
        //            htmlWorker.Parse(sr);
        //        }
        //    }
        //    pdfDoc.Close();
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdfDoc);
        //    Response.End();

        //    db.Entry(gethtml).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    return View();
        //}
    }
}