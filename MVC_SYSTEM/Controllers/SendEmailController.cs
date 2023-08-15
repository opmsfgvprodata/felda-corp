using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    public class SendEmailController : Controller
    {
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        SendEmailNotification SendEmailNotification = new SendEmailNotification();
        errorlog geterror = new errorlog();
        DatabaseAction DatabaseAction = new DatabaseAction();

        //new Class
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        private GetNSWL GetNSWL = new GetNSWL();

        // GET: SendEmail
        //public ActionResult SendEmailForAppNewWorker(string kdldg, string filename)
        //{
        //    string status = "Nothing Happen";
        //    if (kdldg != null && filename != null)
        //    {
        //        try
        //        {
        //            string subject = "Permohonan Kelulusan Pekerja Baru";
        //            string msg = "";
        //            string[] cc = new string[] { };
        //            List<string> cclist = new List<string>();
        //            string[] bcc = new string[] { };
        //            List<string> bcclist = new List<string>();

        //            var getreceiverdetail = db.vw_NSWL.Where(x => x.fld_LdgCode.Trim() == kdldg.Trim()).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID }).FirstOrDefault();

        //            msg = "<html>";
        //            msg += "<body>";
        //            msg += "<p>Assalamualaikum,</p>";
        //            msg += "<p>Mohon kerjasama pihak Wilayah untuk meluluskan permohonan pekerja baru. Keterangan seperti dibawah:-</p>";
        //            msg += "<table border=\"1\">";
        //            msg += "<thead>";
        //            msg += "<tr>";
        //            msg += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Nama File</th><th>Pautan Untuk HQ</th>";
        //            msg += "</tr>";
        //            msg += "</thead>";
        //            msg += "<tbody>";
        //            msg += "<tr>";
        //            msg += "<td align=\"center\">" + getreceiverdetail.fld_NamaWilayah + "</td><td align=\"center\">" + getreceiverdetail.fld_LdgCode + "</td><td align=\"center\">" + getreceiverdetail.fld_NamaLadang + "</td><td align=\"center\">" + filename + "</td><td align=\"center\"><a href=\"" + Url.Action("ApprovalNewWorker", "Approval", new { kdldg = kdldg, ascfilename = filename }, "http") + "\">Klik ke pautan kelulusan</a></td>";
        //            msg += "</tr>";
        //            msg += "</tbody>";
        //            msg += "</table>";
        //            msg += "<p>Terima Kasih.</p>";
        //            msg += "</body>";
        //            msg += "</html>";

        //            cclist.Add(getreceiverdetail.fld_SyarikatEmail);
        //            cclist.Add(getreceiverdetail.fld_LdgEmail);
        //            var emailcclist = db.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldCategory == "CC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
        //            if (emailcclist != null)
        //            {
        //                foreach (var ccemail in emailcclist)
        //                {
        //                    cclist.Add(ccemail.fldEmail);
        //                }
        //            }
        //            cc = cclist.ToArray();

        //            var emailbcclist = db.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
        //            if (emailbcclist != null)
        //            {
        //                foreach (var bccemail in emailbcclist)
        //                {
        //                    bcclist.Add(bccemail.fldEmail);
        //                }
        //                bcc = bcclist.ToArray();
        //            }

        //            if (SendEmailNotification.CheckEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, filename, "Ladang"))
        //            {
        //                if (SendEmailNotification.SendEmail(subject, msg, getreceiverdetail.fld_WlyhEmail, cc, bcc))
        //                {
        //                    SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, filename, "Email From Ladang To HQ - New Worker Approval", "Ladang", 1);
        //                    status = "Email telah dihantar";
        //                }
        //                else
        //                {
        //                    SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, filename, "Email From Ladang To HQ - New Worker Approval", "Ladang", 0);
        //                    status = "Email gagal dihantar";
        //                }
        //                DatabaseAction.InsertDataTotbltblTaskRemainder(filename, kdldg, getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, "01");
        //            }
        //            else
        //            {
        //                status = "Email telah dihantar kepada HQ sebelum ini";
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            status = "Maaf masalah penghantaran email";
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        }
        //    }
            
        //    ViewBag.status = status;
        //    return View();
        //    //return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult SendEmailForAppNewUserID(string kdldg, string filename)
        //{
        //    string status = "Nothing Happen";
        //    if (kdldg != null && filename != null)
        //    {
        //        try
        //        {
        //            string subject = "Permohonan Kelulusan ID Pengguna Baru";
        //            string msg = "";
        //            string[] cc = new string[] { };
        //            List<string> cclist = new List<string>();
        //            string[] bcc = new string[] { };
        //            List<string> bcclist = new List<string>();

        //            var getreceiverdetail = db.vw_NSWL.Where(x => x.fld_LdgCode.Trim() == kdldg.Trim()).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID }).FirstOrDefault();

        //            msg = "<html>";
        //            msg += "<body>";
        //            msg += "<p>Assalamualaikum,</p>";
        //            msg += "<p>Mohon kerjasama pihak Wilayah untuk meluluskan permohonan ID pengguna baru. Keterangan seperti dibawah:-</p>";
        //            msg += "<table border=\"1\">";
        //            msg += "<thead>";
        //            msg += "<tr>";
        //            msg += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Nama File</th><th>Pautan Untuk HQ</th>";
        //            msg += "</tr>";
        //            msg += "</thead>";
        //            msg += "<tbody>";
        //            msg += "<tr>";
        //            msg += "<td align=\"center\">" + getreceiverdetail.fld_NamaWilayah + "</td><td align=\"center\">" + getreceiverdetail.fld_LdgCode + "</td><td align=\"center\">" + getreceiverdetail.fld_NamaLadang + "</td><td align=\"center\">" + filename + "</td><td align=\"center\"><a href=\"" + Url.Action("ApprovalNewUserID", "Approval", new { kdldg = kdldg, ascfilename = filename }, "http") + "\">Klik ke pautan kelulusan</a></td>";
        //            msg += "</tr>";
        //            msg += "</tbody>";
        //            msg += "</table>";
        //            msg += "<p>Terima Kasih.</p>";
        //            msg += "</body>";
        //            msg += "</html>";

        //            cclist.Add(getreceiverdetail.fld_SyarikatEmail);
        //            cclist.Add(getreceiverdetail.fld_LdgEmail);
        //            var emailcclist = db.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldCategory == "CC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
        //            if (emailcclist != null)
        //            {
        //                foreach (var ccemail in emailcclist)
        //                {
        //                    cclist.Add(ccemail.fldEmail);
        //                }
        //            }
        //            cc = cclist.ToArray();

        //            var emailbcclist = db.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
        //            if (emailbcclist != null)
        //            {
        //                foreach (var bccemail in emailbcclist)
        //                {
        //                    bcclist.Add(bccemail.fldEmail);
        //                }
        //                bcc = bcclist.ToArray();
        //            }
                    
        //            if (SendEmailNotification.CheckEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, filename, "Ladang"))
        //            {
        //                if (SendEmailNotification.SendEmail(subject, msg, getreceiverdetail.fld_WlyhEmail, cc, bcc))
        //                {
        //                    SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, filename, "Email From Ladang To HQ - New User ID Approval", "Ladang", 1);
        //                    status = "Email telah dihantar kepada HQ";
        //                }
        //                else
        //                {
        //                    SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, filename, "Email From Ladang To HQ - New User ID Approval", "Ladang", 0);
        //                    status = "Email gagal dihantar kepada HQ";
        //                }
        //                DatabaseAction.InsertDataTotbltblTaskRemainder(filename, kdldg, getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, "02");

        //            }
        //            else
        //            {
        //                status = "Email telah dihantar kepada HQ sebelum ini";
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            status = "Maaf masalah penghantaran email";
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        }
        //    }
        //    ViewBag.status = status;
        //    return View();
        //    //return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SendEmailForAppNewUserID2(string kdldg, string filename, string kdprmhnan)
        {
            string msg1 = "";
            string statusmsg = "";
            bool status = false;
            string departmentto = "";
            string departmentcc = "";
            //var filename = db.tblASCApprovalFileDetails.Where(x => x.fldID == batchid).Select(s => s.fldFileName).FirstOrDefault();
            if (kdldg != null && filename != null)
            {
                try
                {
                    string subject = "Permohonan Kelulusan ID Pengguna Baru";
                    string msg = "";
                    string[] to = new string[] { };
                    List<string> tolist = new List<string>();
                    string[] cc = new string[] { };
                    List<string> cclist = new List<string>();
                    string[] bcc = new string[] { };
                    List<string> bcclist = new List<string>();

                    var getreceiverdetail = GetNSWL.GetLadangDetail(kdprmhnan, kdldg);

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum,</p>";
                    //msg += "<p><font color=\"red\">INI ADALAH CUBAAN SEMATA - MATA </font></p>";
                    msg += "<p>Mohon kerjasama pihak Wilayah untuk meluluskan permohonan ID pengguna baru. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Nama File</th><th>Pautan Untuk HQ</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + getreceiverdetail.fld_NamaWilayah + "</td><td align=\"center\">" + getreceiverdetail.fld_LdgCode + "</td><td align=\"center\">" + getreceiverdetail.fld_NamaLadang + "</td><td align=\"center\">" + filename + "</td><td align=\"center\"><a href=\"" + Url.Action("ApprovalNewUserIDOPMS", "Approval", new { kdldg = kdldg, ascfilename = filename }, this.Request.Url.Scheme) + "\">Klik ke pautan kelulusan</a></td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    if(getreceiverdetail.fld_CostCentre == "1000")
                    {
                        departmentto = "REGION_USERID_APPROVAL_FELDA";
                        departmentcc = "HQ_USERID_APPROVAL_FELDA";
                    }

                    if (getreceiverdetail.fld_CostCentre == "8800")
                    {
                        departmentto = "REGION_USERID_APPROVAL_FPM";
                        departmentcc = "HQ_USERID_APPROVAL_FPM";
                    }

                    //commented by faeza 19.06.2023
                    //var emaillist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldDeleted == false).ToList();
                    //var emailtolist = emaillist.Where(x => x.fldCategory == "TO" && x.fldDepartment == "REGION_USERID_APPROVAL" && x.fldWilayahID == getreceiverdetail.fld_WilayahID).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    //added by faeza 19.06.2023
                    var emailtolist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldWilayahID == getreceiverdetail.fld_WilayahID && x.fldCategory == "TO" && x.fldDepartment == departmentto && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    if (emailtolist.Count() > 0)
                    {
                        foreach (var toemail in emailtolist)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();

                        //commented by faeza 19.06.2023
                        //var emailcclist = emaillist.Where(x => (x.fldCategory == "CC" && x.fldDepartment == "HQ_USERID_APPROVAL") || (x.fldCategory == "CC" && x.fldDepartment == "HQ_USERID_APPROVAL")).Select(s => new { s.fldEmail, s.fldName }).ToList();

                        //added by faeza faeza 19.06.2023
                        var emailcclist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldCategory == "CC" && x.fldDepartment == departmentcc && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                        if (emailcclist.Count() > 0)
                        {
                            foreach (var ccemail in emailcclist)
                            {
                                cclist.Add(ccemail.fldEmail);
                            }
                        }
                        cc = cclist.ToArray();

                        //commented by faeza 19.06.2023
                        //var emailbcclist = emaillist.Where(x => x.fldCategory == "BCC" && x.fldDepartment == "DEVELOPER").Select(s => new { s.fldEmail, s.fldName }).ToList();

                        //added by faeza faeza 19.06.2023
                        var emailbcclist = dbC.tblEmailLists.Where(x => x.fldNegaraID == getreceiverdetail.fld_NegaraID && x.fldSyarikatID == getreceiverdetail.fld_SyarikatID && x.fldCategory == "BCC" && x.fldDepartment == "DEVELOPER" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                        if (emailbcclist.Count() > 0)
                        {
                            foreach (var bccemail in emailbcclist)
                            {
                                bcclist.Add(bccemail.fldEmail);
                            }
                        }
                        bcc = bcclist.ToArray();

                        if (SendEmailNotification.CheckEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, filename, "Ladang"))
                        {
                            //if (SendEmailNotification.SendEmail(subject, msg, getreceiverdetail.fld_WlyhEmail, cc, bcc))
                            if (SendEmailNotification.SendEmail(subject, msg, to, cc, bcc))
                            {
                                SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, filename, "Email From Ladang To HQ - New User ID Approval", "Ladang", 1);
                                msg1 = "Email telah dihantar kepada HQ";
                                statusmsg = "success";
                                status = true;
                            }
                            else
                            {
                                SendEmailNotification.InsertIntotblEmailNotiStatus(getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, filename, "Email From Ladang To HQ - New User ID Approval", "Ladang", 0);
                                msg1 = "Email gagal dihantar kepada HQ";
                                statusmsg = "warning";
                                status = false;
                            }
                            DatabaseAction.InsertDataTotbltblTaskRemainder(filename, kdldg, getreceiverdetail.fld_NegaraID, getreceiverdetail.fld_SyarikatID, getreceiverdetail.fld_WilayahID, getreceiverdetail.fld_LadangID, "02");

                        }
                        else
                        {
                            msg1 = "Email telah dihantar kepada HQ sebelum ini";
                            statusmsg = "warning";
                            status = false;
                        }
                    }
                    else
                    {
                        msg1 = "Email penerima tiada sila mohon pihak HQ memasukkan email berkenaan";
                        statusmsg = "warning";
                        status = false;
                    }
                }
                catch (Exception ex)
                {
                    msg1 = "Maaf masalah penghantaran email";
                    statusmsg = "danger";
                    status = false;
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                }
            }
            return Json(new { msg = msg1, statusmsg = statusmsg,status = status });
        }
    }
}