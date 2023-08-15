using MVC_SYSTEM.Models;
using MVC_SYSTEM.AuthModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Class
{
    public class DatabaseAction
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_ModelsCorporate db3 = new MVC_SYSTEM_ModelsCorporate();
        private ChangeTimeZone timezone = new ChangeTimeZone();

        //new Class
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth dbA = new MVC_SYSTEM_Auth();

        public void InsertDataTotbltblTaskRemainder(string filename, string kdldg, int NegaraID, int? SyarikatID, int? WilayahID, int LadangID, string kdpurpose)
        {
            var getTaskRemainder = dbC.tblTaskRemainders.Where(x => x.fldFileName.Trim() == filename.Trim() && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID && x.fldPurpose == kdpurpose).FirstOrDefault();

            if(getTaskRemainder == null)
            {
                ModelsCorporate.tblTaskRemainder tblTaskRemainder = new ModelsCorporate.tblTaskRemainder();
                tblTaskRemainder.fldFileName = filename;
                tblTaskRemainder.fldCodeLadang = kdldg;
                tblTaskRemainder.fldNegaraID = NegaraID;
                tblTaskRemainder.fldSyarikatID = SyarikatID;
                tblTaskRemainder.fldWilayahID = WilayahID;
                tblTaskRemainder.fldLadangID = LadangID;
                tblTaskRemainder.fldPurpose = kdpurpose;
                tblTaskRemainder.fldStatus = 0;
                tblTaskRemainder.fldDateTimeStamp = timezone.gettimezone();

                dbC.tblTaskRemainders.Add(tblTaskRemainder);
                dbC.SaveChanges();
            }
        }

        public void UpdateDataTotbltblTaskRemainder(string filename, int NegaraID, int? SyarikatID, int? WilayahID, int LadangID, string kdpurpose)
        {
            var getTaskRemainder = dbC.tblTaskRemainders.Where(x => x.fldFileName.Trim() == filename.Trim() && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID && x.fldPurpose == kdpurpose).FirstOrDefault();
            if (getTaskRemainder != null)
            {
                getTaskRemainder.fldStatus = 1;

                dbC.Entry(getTaskRemainder).State = EntityState.Modified;
                dbC.SaveChanges();
            }
        }

        public void UpdateDataTotblASCApprovalFileDetail(long fileid)
        {
            var getASCFileDetail = dbC.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).FirstOrDefault();

            getASCFileDetail.fldGenStatus = 1;

            dbC.Entry(getASCFileDetail).State = EntityState.Modified;
            dbC.SaveChanges();
        }

        public void UpdateDataTotblSokPermhnWang(long ID, int semakwil, int tolakwil, int sokongwilgm, int tolakwilgm, int terimahq, int tolakhq, string flag, int userid, DateTime getdatetime, decimal PDP, decimal CIT, string NoAcc, string NoGL, string NoCIT, decimal Manual)
        {
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_ID == ID).FirstOrDefault();

            switch(flag)
            {
                case "SemakWil":
                    getdata.fld_SemakWil_Status = semakwil;
                    getdata.fld_SemakWil_By = userid;
                    getdata.fld_SemakWil_DT = getdatetime;
                    getdata.fld_TolakWil_Status = tolakwil;
                    getdata.fld_TolakWil_By = 0;
                    getdata.fld_TolakWil_DT = null;
                    getdata.fld_JumlahPDP = PDP;
                    //getdata.fld_JumlahTT = TT;
                    getdata.fld_JumlahCIT = CIT;
                    getdata.fld_NoAcc = NoAcc;
                    getdata.fld_NoGL = NoGL;
                    getdata.fld_NoCIT = NoCIT;
                    getdata.fld_JumlahManual = Manual;
                    break;

                case "TolakWil":
                    getdata.fld_SemakWil_Status = semakwil;
                    getdata.fld_SemakWil_By = 0;
                    getdata.fld_SemakWil_DT = null;
                    getdata.fld_TolakWil_Status = tolakwil;
                    getdata.fld_TolakWil_By = userid;
                    getdata.fld_TolakWil_DT = getdatetime;
                    break;
            }

            db.Entry(getdata).State = EntityState.Modified;
            db.SaveChanges();
        }
      
        //added by kamalia 24/11/21
        public void UpdateDataTotblSokPermhnWang(long ID, int semakwil, int tolakwil, int sokongwilgm, int tolakwilgm, int terimahq, int tolakhq, string flag, int userid, DateTime getdatetime, string NoAcc, string NoCIT)
        {
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_ID == ID).FirstOrDefault();

            switch (flag)
            {
                case "SemakWil":
                    getdata.fld_SemakWil_Status = semakwil;
                    getdata.fld_SemakWil_By = userid;
                    getdata.fld_SemakWil_DT = getdatetime;
                    getdata.fld_TolakWil_Status = tolakwil;
                    getdata.fld_TolakWil_By = 0;
                    getdata.fld_TolakWil_DT = null;
                    //getdata.fld_JumlahPDP = PDP;
                    //getdata.fld_JumlahTT = TT;
                    //getdata.fld_JumlahCIT = CIT;
                    getdata.fld_NoAcc = NoAcc;
                    //getdata.fld_NoGL = NoGL;
                    getdata.fld_NoCIT = NoCIT;
                    //getdata.fld_JumlahManual = Manual;
                    break;

                case "TolakWil":
                    getdata.fld_SemakWil_Status = semakwil;
                    getdata.fld_SemakWil_By = 0;
                    getdata.fld_SemakWil_DT = null;
                    getdata.fld_TolakWil_Status = tolakwil;
                    getdata.fld_TolakWil_By = userid;
                    getdata.fld_TolakWil_DT = getdatetime;
                    break;
            }

            db.Entry(getdata).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void InsertDataTotblSokPermhnWangHisAction(string hisdesc, int hisuserid, DateTime hisDT, long SPWID, int HisAppLevel, string SebabTolak)
        {
            ModelsCorporate.tblSokPermhnWangHisAction tblSokPermhnWangHisAction = new ModelsCorporate.tblSokPermhnWangHisAction();

            tblSokPermhnWangHisAction.fldHisDesc = hisdesc;
            tblSokPermhnWangHisAction.fldHisUserID = hisuserid;
            tblSokPermhnWangHisAction.fldHisDT = hisDT;
            tblSokPermhnWangHisAction.fldHisSPWID = SPWID;
            tblSokPermhnWangHisAction.fldHisAppLevel = HisAppLevel;
            tblSokPermhnWangHisAction.fldHisReason = SebabTolak;

            db.tblSokPermhnWangHisActions.Add(tblSokPermhnWangHisAction);
            db.SaveChanges();
        }
        //end
        //add by kamalia 2/12/21
        public void UpdateDataTotblSokPermhnWangGM2(int ID, string flag, int Month, int Year, int userid, DateTime getdatetime)
        {
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == ID && x.fld_Month == Month && x.fld_Year == Year).ToList();

            switch (flag)
            {
                case "SokongGMWil":
                    getdata.ForEach(x => x.fld_SokongWilGM_Status = 1);
                    getdata.ForEach(x => x.fld_SokongWilGM_By = userid);
                    getdata.ForEach(x => x.fld_SokongWilGM_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TolakWilGM_Status = 0);
                    getdata.ForEach(x => x.fld_TolakWilGM_By = 0);
                    getdata.ForEach(x => x.fld_TolakWilGM_DT = null);
                    break;

                case "TolakGMWil":
                    getdata.ForEach(x => x.fld_TolakWilGM_Status = 1);
                    getdata.ForEach(x => x.fld_TolakWilGM_By = userid);
                    getdata.ForEach(x => x.fld_TolakWilGM_DT = getdatetime);
                    getdata.ForEach(x => x.fld_SokongWilGM_Status = 0);
                    getdata.ForEach(x => x.fld_SokongWilGM_By = userid);
                    getdata.ForEach(x => x.fld_SokongWilGM_DT = null);
                    break;
            }
            db.SaveChanges();
        }


        public void UpdateDataTotblSokPermhnWangHQ2(int ID, string flag, int Month, int Year, int userid, DateTime getdatetime)
        {
            //var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == ID && x.fld_Month == Month && x.fld_Year == Year).ToList();
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == ID && x.fld_Month == Month && x.fld_Year == Year).ToList();

            switch (flag)
            {
                case "TerimaHQ":
                    getdata.ForEach(x => x.fld_TerimaHQ_Status = 1);
                    getdata.ForEach(x => x.fld_TerimaHQ_By = userid);
                    getdata.ForEach(x => x.fld_TerimaHQ_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TolakHQ_Status = 0);
                    getdata.ForEach(x => x.fld_TolakHQ_By = 0);
                    getdata.ForEach(x => x.fld_TolakHQ_DT = null);
                    break;

                case "TolakHQ":
                    getdata.ForEach(x => x.fld_TolakHQ_Status = 1);
                    getdata.ForEach(x => x.fld_TolakHQ_By = userid);
                    getdata.ForEach(x => x.fld_TolakHQ_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TerimaHQ_Status = 0);
                    getdata.ForEach(x => x.fld_TerimaHQ_By = userid);
                    getdata.ForEach(x => x.fld_TerimaHQ_DT = null);
                    break;
            }
            db.SaveChanges();
        } //end   by kamalia 2/12/21

        public void UpdateDataTotblSokPermhnWangGM(int ID, string flag, int Month, int Year, int userid, DateTime getdatetime)
        {
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == ID && x.fld_Month == Month && x.fld_Year == Year).ToList();

            switch (flag)
            {
                case "SokongGMWil":
                    getdata.ForEach(x=>x.fld_SokongWilGM_Status = 1);
                    getdata.ForEach(x => x.fld_SokongWilGM_By = userid);
                    getdata.ForEach(x => x.fld_SokongWilGM_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TolakWilGM_Status = 0);
                    getdata.ForEach(x => x.fld_TolakWilGM_By = 0);
                    getdata.ForEach(x => x.fld_TolakWilGM_DT = null);
                    break;

                case "TolakGMWil":
                    getdata.ForEach(x => x.fld_TolakWilGM_Status = 1);
                    getdata.ForEach(x => x.fld_TolakWilGM_By = userid);
                    getdata.ForEach(x => x.fld_TolakWilGM_DT = getdatetime);
                    getdata.ForEach(x => x.fld_SokongWilGM_Status = 0);
                    getdata.ForEach(x => x.fld_SokongWilGM_By = userid);
                    getdata.ForEach(x => x.fld_SokongWilGM_DT = null);
                    break;
            }
            db.SaveChanges();
        }

        public void UpdateDataTotblSokPermhnWangHQ(int ID, string flag, int Month, int Year, int userid, DateTime getdatetime)
        {
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == ID && x.fld_Month == Month && x.fld_Year == Year).ToList();

            switch (flag)
            {
                case "TerimaHQ":
                    getdata.ForEach(x => x.fld_TerimaHQ_Status = 1);
                    getdata.ForEach(x => x.fld_TerimaHQ_By = userid);
                    getdata.ForEach(x => x.fld_TerimaHQ_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TolakHQ_Status = 0);
                    getdata.ForEach(x => x.fld_TolakHQ_By = 0);
                    getdata.ForEach(x => x.fld_TolakHQ_DT = null);
                    break;

                case "TolakHQ":
                    getdata.ForEach(x => x.fld_TolakHQ_Status = 1);
                    getdata.ForEach(x => x.fld_TolakHQ_By = userid);
                    getdata.ForEach(x => x.fld_TolakHQ_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TerimaHQ_Status = 0);
                    getdata.ForEach(x => x.fld_TerimaHQ_By = userid);
                    getdata.ForEach(x => x.fld_TerimaHQ_DT = null);
                    break;
            }
            db.SaveChanges();
        }
        public void InsertDataTotblSokPermhnWangHisAction(string hisdesc, int hisuserid, DateTime hisDT, long SPWID, int HisAppLevel)
        {
            ModelsCorporate.tblSokPermhnWangHisAction tblSokPermhnWangHisAction = new ModelsCorporate.tblSokPermhnWangHisAction();

            tblSokPermhnWangHisAction.fldHisDesc = hisdesc;
            tblSokPermhnWangHisAction.fldHisUserID = hisuserid;
            tblSokPermhnWangHisAction.fldHisDT = hisDT;
            tblSokPermhnWangHisAction.fldHisSPWID = SPWID;
            tblSokPermhnWangHisAction.fldHisAppLevel = HisAppLevel;

            db.tblSokPermhnWangHisActions.Add(tblSokPermhnWangHisAction);
            db.SaveChanges();
        }

        public int InsertDataTotblASCApprovalFileDetail(string fldFileName, string fldCodeLadang, int fldNegaraID,
            int? fldSyarikatID, int? fldWilayahID, int fldLadangID, int fldGenStatus, int fldASCFileStatus,
            int fldPurpose, DateTime fldDateTimeCreated)
        {
            ModelsCorporate.tblASCApprovalFileDetail tblASCApprovalFileDetail = new ModelsCorporate.tblASCApprovalFileDetail();

            tblASCApprovalFileDetail.fldFileName = fldFileName;
            tblASCApprovalFileDetail.fldCodeLadang = fldCodeLadang;
            tblASCApprovalFileDetail.fldNegaraID = fldNegaraID;
            tblASCApprovalFileDetail.fldSyarikatID = fldSyarikatID;
            tblASCApprovalFileDetail.fldWilayahID = fldWilayahID;
            tblASCApprovalFileDetail.fldLadangID = fldLadangID;
            tblASCApprovalFileDetail.fldGenStatus = fldGenStatus;
            tblASCApprovalFileDetail.fldASCFileStatus = fldASCFileStatus;
            tblASCApprovalFileDetail.fldPurpose = fldPurpose;
            tblASCApprovalFileDetail.fldDateApplied = fldDateTimeCreated;

            dbC.tblASCApprovalFileDetails.Add(tblASCApprovalFileDetail);
            dbC.SaveChanges();

            return tblASCApprovalFileDetail.fldID;
        }

        public void InsertDataTotblUserIDApp(string fldUserid, string fldNama, string fldNoKP, string fldKdLdg, string fldNamaLdg, 
            DateTime fldTarikh, string fldJawatan, string fldPassword, string fldStatus, DateTime fldTrkdload, long fldFileID, 
            int fldNegaraID,int? fldSyarikatID, int? fldWilayahID, int fldLadangID, int? fldActionBy, DateTime? fldDateTimeApprove)
        {
            ModelsCorporate.tblUserIDApp tblUserIDApp = new ModelsCorporate.tblUserIDApp();

            tblUserIDApp.fldUserid = fldUserid;
            tblUserIDApp.fldNama = fldNama;
            tblUserIDApp.fldNoKP = fldNoKP;
            tblUserIDApp.fldKdLdg = fldKdLdg;
            tblUserIDApp.fldNamaLdg = fldNamaLdg;
            tblUserIDApp.fldTarikh = fldTarikh;
            tblUserIDApp.fldJawatan = fldJawatan;
            tblUserIDApp.fldPassword = fldPassword;
            tblUserIDApp.fldStatus = fldStatus;
            tblUserIDApp.fldTrkdload = fldTrkdload;
            tblUserIDApp.fldFileID = fldFileID;
            tblUserIDApp.fldNegaraID = fldNegaraID;
            tblUserIDApp.fldSyarikatID = fldSyarikatID;
            tblUserIDApp.fldWilayahID = fldWilayahID;
            tblUserIDApp.fldLadangID = fldLadangID;
            tblUserIDApp.fldActionBy = fldActionBy;
            tblUserIDApp.fldDateTimeApprove = fldDateTimeApprove;

            dbC.tblUserIDApps.Add(tblUserIDApp);
            dbC.SaveChanges();
        }

        public void InsertDataTotblUser(string fldUserName, string fldUserFullName, string fldUserShortName,
            string fldUserEmail, string fldUserPassword, int? fldRoleID, int? fld_KmplnSyrktID, int? fldNegaraID,
            int? fldSyarikatID, int? fldWilayahID, int? fldLadangID, int? fldFirstTimeLogin, int? fldClientID,
            bool? fldDeleted, int? fld_CreatedBy, DateTime? fld_CreatedDT, string fldUserCategory)
        {
            AuthModels.tblUser tblUser = new AuthModels.tblUser();

            tblUser.fldUserName = fldUserName;
            tblUser.fldUserFullName = fldUserFullName;
            tblUser.fldUserShortName = fldUserShortName;
            tblUser.fldUserEmail = fldUserEmail;
            tblUser.fldUserPassword = fldUserPassword;
            tblUser.fldRoleID = fldRoleID;
            tblUser.fld_KmplnSyrktID = fld_KmplnSyrktID;
            tblUser.fldNegaraID = fldNegaraID;
            tblUser.fldSyarikatID = fldSyarikatID;
            tblUser.fldWilayahID = fldWilayahID;
            tblUser.fldLadangID = fldLadangID;
            tblUser.fldFirstTimeLogin = fldFirstTimeLogin;
            tblUser.fldClientID = fldClientID;
            tblUser.fldDeleted = fldDeleted;
            tblUser.fld_CreatedBy = fld_CreatedBy;
            tblUser.fld_CreatedDT = fld_CreatedDT;
            tblUser.fldUserCategory = fldUserCategory;

            dbA.tblUsers.Add(tblUser);
            dbA.SaveChanges();
        }

        public void UpdateDataTotblUserIDApp(string fldUserid, string fldNama, string fldNoKP, string fldKdLdg, string fldNamaLdg,
            DateTime fldTarikh, string fldJawatan, string fldPassword, string fldStatus, DateTime fldTrkdload, int fldFileID,
            int fldNegaraID, int? fldSyarikatID, int? fldWilayahID, int fldLadangID, int? fldActionBy, DateTime? fldDateTimeApprove)
        {
            var gettblUserIDApp = db3.tblUserIDApps.Where(x => x.fldUserid == fldUserid).FirstOrDefault();

            gettblUserIDApp.fldNama = fldNama;
            gettblUserIDApp.fldNoKP = fldNoKP;
            gettblUserIDApp.fldJawatan = fldJawatan;

            dbC.Entry(gettblUserIDApp).State = EntityState.Modified;
            dbC.SaveChanges();
        }

        public void UpdateDataTotblUser(string fldUserName, string fldUserFullName, string fldUserShortName,
            string fldUserEmail, string fldUserPassword, int? fldRoleID, int? fld_KmplnSyrktID, int? fldNegaraID,
            int? fldSyarikatID, int? fldWilayahID, int? fldLadangID, int? fldFirstTimeLogin, int? fldClientID,
            bool? fldDeleted, int? fld_CreatedBy, DateTime? fld_CreatedDT, string fldUserCategory)
        {
            var gettblUsersdata = dbA.tblUsers.Where(x => x.fldUserName == fldUserName).FirstOrDefault();

            gettblUsersdata.fldUserFullName = fldUserFullName;
            gettblUsersdata.fldUserShortName = fldUserShortName;
            gettblUsersdata.fldUserEmail = fldUserEmail;
            gettblUsersdata.fldRoleID = fldRoleID;

            dbA.Entry(gettblUsersdata).State = EntityState.Modified;
            dbA.SaveChanges();
        }

        //Added by Shazana on 15/12/2022
        public void UpdateDataTotblSokPermhnWangGMSecond(int ID, string flag, int Month, int Year, int userid, DateTime getdatetime)
        {
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == ID && x.fld_Month == Month && x.fld_Year == Year).ToList();

            switch (flag)
            {
                case "SokongGMWil":
                    getdata.ForEach(x => x.fld_TerimaHQ_Status = 1);
                    getdata.ForEach(x => x.fld_TerimaHQ_By = userid);
                    getdata.ForEach(x => x.fld_TerimaHQ_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TolakHQ_Status = 0);
                    getdata.ForEach(x => x.fld_TolakHQ_By = 0);
                    getdata.ForEach(x => x.fld_TolakHQ_DT = null);
                    break;

                case "TolakGMWil":
                    getdata.ForEach(x => x.fld_TolakHQ_Status = 1);
                    getdata.ForEach(x => x.fld_TolakHQ_By = userid);
                    getdata.ForEach(x => x.fld_TolakHQ_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TerimaHQ_Status = 0);
                    getdata.ForEach(x => x.fld_TerimaHQ_By = userid);
                    getdata.ForEach(x => x.fld_TerimaHQ_DT = null);
                    break;
            }
            db.SaveChanges();
        }
    }
}