using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Class
{
    public class GetLadang
    {
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_ModelsCorporate dbCorp = new MVC_SYSTEM_ModelsCorporate();
        public List<int?> GetCodeLadang(int wlyhid, string flag, int year)
        {
            var CodeLadang = new List<int?>();

            switch(flag)
            {
                case "Permit":
                    var CodeLadang1 = db.vw_PermitPassportDetail.Where(x => x.fld_BilBlnTmtPrmnt <= 3 && x.fld_WlyhID == wlyhid && x.fld_Kdaktf == "1").Select(s => new { s.fld_LadangID, s.fld_LdgName }).Distinct().OrderBy(o => o.fld_LdgName).ToList();
                    CodeLadang = CodeLadang1.Select(s => s.fld_LadangID).ToList();
                    break;

                case "Passport":
                    var CodeLadang2 = db.vw_PermitPassportDetail.Where(x => x.fld_BilBlnTmtPsprt <= 3 && x.fld_WlyhID == wlyhid && x.fld_Kdaktf == "1").Select(s => new { s.fld_LadangID, s.fld_LdgName }).Distinct().OrderBy(o => o.fld_LdgName).ToList();
                    CodeLadang = CodeLadang2.Select(s => s.fld_LadangID).ToList();
                    break;

                case "AuditTrail":
                    var CodeLadang3 = db.vw_AuditTrail.Where(x => x.fld_Thn == year && x.fld_WilayahID == wlyhid).Distinct().OrderBy(o => o.fld_LdgName).ToList();
                    CodeLadang = CodeLadang3.Select(s => s.fld_LadangID).ToList();
                    break;

                case "WorkerTransac":
                    var CodeLadang4 = db.vw_AuditTrail.Where(x => x.fld_Thn == year && x.fld_WilayahID == wlyhid).Select(s => new { s.fld_LadangID, s.fld_LdgName }).Distinct().OrderBy(o => o.fld_LdgName).ToList();
                    CodeLadang = CodeLadang4.Select(s => s.fld_LadangID).ToList();
                    break;

                case "NewWorkerApp":
                    var CodeLadang5 = db.tblTaskRemainders.Join(db.vw_NSWL, j=>j.fldLadangID, k=>k.fld_LadangID, (j,k) => new { j.fldWilayahID,j.fldPurpose,j.fldStatus,j.fldLadangID,k.fld_NamaLadang}).Where(x => x.fldWilayahID == wlyhid && x.fldPurpose == "01" && x.fldStatus == 0).Distinct().OrderBy(o => o.fld_NamaLadang).ToList();
                    CodeLadang = CodeLadang5.Select(s => s.fldLadangID).ToList();
                    break;

                case "NewUserIDApp":
                    var CodeLadang6 = db.tblTaskRemainders.Join(db.vw_NSWL, j => j.fldLadangID, k => k.fld_LadangID, (j, k) => new { j.fldWilayahID, j.fldPurpose, j.fldStatus, j.fldLadangID, k.fld_NamaLadang }).Where(x => x.fldWilayahID == wlyhid && x.fldPurpose == "02" && x.fldStatus == 0).Distinct().OrderBy(o => o.fld_NamaLadang).ToList();
                    CodeLadang = CodeLadang6.Select(s => s.fldLadangID).ToList();
                    break;
            }
            return CodeLadang;
        }

        public List<int?> GetCodeLadang2(int wlyhid, string flag, int year, int month, string status)
        {
            var CodeLadang = new List<int?>();

            int status2 = int.Parse(status);
            switch (flag)
            {
                case "NewWorkerAppReport":
                    if (status2 == 0)
                    {
                        var CodeLadang1 = db.tblPkjmastApps.Join(db.vw_NSWL, j => j.fldLadangID, k => k.fld_LadangID, (j, k) => new { j.fldLadangID, k.fld_LadangID, j.fldWilayahID, j.fldDateTimeApprove, k.fld_NamaLadang }).Where(x => x.fldWilayahID == wlyhid && x.fldDateTimeApprove.Value.Month == month && x.fldDateTimeApprove.Value.Year == year).Distinct().OrderBy(o => o.fld_NamaLadang).ToList();
                        CodeLadang = CodeLadang1.Select(s => s.fldLadangID).ToList();
                    }
                    else
                    {
                        var CodeLadang2 = db.tblPkjmastApps.Join(db.vw_NSWL, j => j.fldLadangID, k => k.fld_LadangID, (j, k) => new { j.fldLadangID, k.fld_LadangID, j.fldWilayahID, j.fldDateTimeApprove, k.fld_NamaLadang, j.fldStatus }).Where(x => x.fldWilayahID == wlyhid && x.fldDateTimeApprove.Value.Month == month && x.fldDateTimeApprove.Value.Year == year && x.fldStatus == status2).Distinct().OrderBy(o => o.fld_NamaLadang).ToList();
                        CodeLadang = CodeLadang2.Select(s => s.fldLadangID).ToList();
                    }
                    break;
            }
            return CodeLadang;
        }

        public List<int?> GetCodeLadang3(int wlyhid, string flag, int year, string Comcode)
        {
            var CodeLadang = new List<int?>();

            switch (flag)
            {
                case "Permit":
                    var CodeLadang1 = db.vw_PermitPassportDetail.Where(x => x.fld_BilBlnTmtPrmnt <= 3 && x.fld_WlyhID == wlyhid && x.fld_Kdaktf == "1").Select(s => new { s.fld_LadangID, s.fld_LdgName }).Distinct().OrderBy(o => o.fld_LdgName).ToList();
                    CodeLadang = CodeLadang1.Select(s => s.fld_LadangID).ToList();
                    break;

                case "Passport":
                    var CodeLadang2 = db.vw_PermitPassportDetail.Where(x => x.fld_BilBlnTmtPsprt <= 3 && x.fld_WlyhID == wlyhid && x.fld_Kdaktf == "1").Select(s => new { s.fld_LadangID, s.fld_LdgName }).Distinct().OrderBy(o => o.fld_LdgName).ToList();
                    CodeLadang = CodeLadang2.Select(s => s.fld_LadangID).ToList();
                    break;

                case "AuditTrail":
                    var CodeLadang3 = db.vw_AuditTrail.Where(x => x.fld_Thn == year && x.fld_WilayahID == wlyhid && x.fld_CostCentre == Comcode).Distinct().OrderBy(o => o.fld_LdgName).ToList();
                    CodeLadang = CodeLadang3.Select(s => s.fld_LadangID).ToList();
                    break;

                case "WorkerTransac":
                    var CodeLadang4 = db.vw_AuditTrail.Where(x => x.fld_Thn == year && x.fld_WilayahID == wlyhid).Select(s => new { s.fld_LadangID, s.fld_LdgName }).Distinct().OrderBy(o => o.fld_LdgName).ToList();
                    CodeLadang = CodeLadang4.Select(s => s.fld_LadangID).ToList();
                    break;

                case "NewWorkerApp":
                    var CodeLadang5 = db.tblTaskRemainders.Join(db.vw_NSWL, j => j.fldLadangID, k => k.fld_LadangID, (j, k) => new { j.fldWilayahID, j.fldPurpose, j.fldStatus, j.fldLadangID, k.fld_NamaLadang }).Where(x => x.fldWilayahID == wlyhid && x.fldPurpose == "01" && x.fldStatus == 0).Distinct().OrderBy(o => o.fld_NamaLadang).ToList();
                    CodeLadang = CodeLadang5.Select(s => s.fldLadangID).ToList();
                    break;

                case "NewUserIDApp":
                    var CodeLadang6 = db.tblTaskRemainders.Join(db.vw_NSWL, j => j.fldLadangID, k => k.fld_LadangID, (j, k) => new { j.fldWilayahID, j.fldPurpose, j.fldStatus, j.fldLadangID, k.fld_NamaLadang }).Where(x => x.fldWilayahID == wlyhid && x.fldPurpose == "02" && x.fldStatus == 0).Distinct().OrderBy(o => o.fld_NamaLadang).ToList();
                    CodeLadang = CodeLadang6.Select(s => s.fldLadangID).ToList();
                    break;
            }
            return CodeLadang;
        }

        public int GetCodeLadangFromID(int ladangid)
        {
            int CodeLadang = 0;

            CodeLadang = db.tbl_Ladang.Where(x => x.fld_ID == ladangid).Select(s => s.fld_ID).FirstOrDefault();
            
            return CodeLadang;
        }
        public string GetCodeLadangFromID2(int ladangid)
        {
            string CodeLadang = "";

            CodeLadang = db.tbl_Ladang.Where(x => x.fld_ID == ladangid).Select(s => s.fld_LdgCode).FirstOrDefault();

            return CodeLadang;
        }
        public int JumlahLadang(int wlyhID)
        {
            MVC_SYSTEM_ModelsCorporate corporateConnection = new MVC_SYSTEM_ModelsCorporate();
            int getjmlhldg = corporateConnection.tbl_Ladang.Count(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false);

            corporateConnection.Dispose();

            return getjmlhldg;
        }

        public int JumlahLadang2(int wlyhID, string Comcode)
        {
            MVC_SYSTEM_ModelsCorporate corporateConnection = new MVC_SYSTEM_ModelsCorporate();
            int getjmlhldg = corporateConnection.tbl_Ladang.Count(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false && x.fld_CostCentre == Comcode);

            corporateConnection.Dispose();

            return getjmlhldg;
        }

        public string GetLadangName(int ladangid, int wlyhID)
        {
            string LadangName = db.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wlyhID).Select(s => s.fld_LdgName).FirstOrDefault();
            return LadangName;
        }

        public string GetLadangCode(int ladangid)
        {
            string LadangCode = db.tbl_Ladang.Where(x => x.fld_ID == ladangid).Select(s => s.fld_LdgCode).FirstOrDefault();
            return LadangCode;
        }

        public void GetLadangAcc(out string NoAcc, out string NoGL, out string NoCIT, int? ldgid, int? wlyhid)
        {
            var account = dbCorp.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhid && x.fld_ID == ldgid).FirstOrDefault();
            NoAcc = account.fld_NoAcc;
            NoGL = account.fld_NoGL;
            NoCIT = account.fld_NoCIT;
        }
    }
}