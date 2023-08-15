using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WriteFixedLength;

namespace MVC_SYSTEM.Class
{
    public class CreateASC
    {
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_ModelsCorporate db2 = new MVC_SYSTEM_ModelsCorporate();

        CheckSharedFolder CheckSharedFolder = new CheckSharedFolder();

        public void CreateASCFileApprovalWorker(long fileid, int? NegaraID, int? SyarikatID)
        {
            //int[] widths = new int[] { 0, 10, 40, 12, 1, 2, 8, 8, 3, 1, 30, 30, 30, 7 };
            string getttsplpday = "";
            string getttsplpmonth = "";
            string getttsplpyear = "";
            string fldTtsplp = "";
            string getttplksday = "";
            string getttplksmonth = "";
            string getttplksyear = "";
            string fldTtplks = "";
            int fldJumPjm = 0;
            string fldJumPjm2 = "";
            string fldKdLdg = "";
            string targetpathmix = "";

            var getapprovalworker = db.tblPkjmastApps.Where(x => x.fldFileID == fileid && x.fldStatus == 1).ToList();
            var getfilename = db.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).Select(s => s.fldFileName).FirstOrDefault();
            var getkdldg = db.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).Select(s => s.fldCodeLadang).FirstOrDefault();
            string targetpath = CheckSharedFolder.GetSourceTargetPath("filetargetpathworkerapp", NegaraID, SyarikatID); //db.tbl_OptionConfig.Where(x => x.fldOptConfFlag1 == "filetargetpathworkerapp" && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
            targetpathmix = targetpath + getkdldg + "\\" + getfilename;
            checkcreatefolder(targetpath + getkdldg);
            TryToDelete(targetpathmix);
            using (StreamWriter writer = new StreamWriter(targetpathmix, true))
            {
                foreach (var workerdetail in getapprovalworker)
                {
                    getttsplpday = string.Format("{0:dd}", workerdetail.fldTtsplp);
                    getttsplpmonth = string.Format("{0:MM}", workerdetail.fldTtsplp);
                    getttsplpyear = string.Format("{0:yyyy}", workerdetail.fldTtsplp);
                    getttplksday = string.Format("{0:dd}", workerdetail.fldTtsplp);
                    getttplksmonth = string.Format("{0:MM}", workerdetail.fldTtsplp);
                    getttplksyear = string.Format("{0:yyyy}", workerdetail.fldTtsplp);
                    fldTtsplp = getttsplpday + getttsplpmonth + getttsplpyear;
                    fldTtplks = getttplksday + getttplksmonth + getttplksyear;
                    fldJumPjm = (int)workerdetail.fldJumPjm * 100;
                    fldJumPjm2 = fldJumPjm.ToString().PadLeft(7, '0');
                    fldKdLdg = workerdetail.fldKdLdg.PadLeft(3, '0').Trim();
                    getfilename = getfilename.PadRight(30, ' ');

                    writer.Write(workerdetail.fldNoPkj.PadRight(10, ' '));
                    writer.Write(workerdetail.fldNama1.PadRight(40, ' '));
                    writer.Write(workerdetail.fldNoKP.PadRight(12, ' '));
                    writer.Write(workerdetail.fldKdJnsPkj.PadRight(1, ' '));
                    writer.Write(workerdetail.fldKdRkyt.PadRight(2, ' '));
                    writer.Write(fldTtsplp);
                    writer.Write(fldTtplks);
                    writer.Write(fldKdLdg);
                    writer.Write(workerdetail.fldStatus.ToString().PadRight(1, ' '));
                    writer.Write(getfilename);
                    writer.Write(workerdetail.fldSbbMsk.PadRight(30, ' '));
                    writer.Write(workerdetail.fldAlsnMsk.PadRight(30, ' '));
                    writer.WriteLine(fldJumPjm2);
                    //writer.Write(workerdetail.)
                }
            }
        }
        
        public void CreateASCFileApprovalUserID(long fileid, int? NegaraID, int? SyarikatID)
        {
            //int[] widths = new int[] { 0, 10, 40, 12, 1, 2, 8, 8, 3, 1, 30, 30, 30, 7 };
            string gettrkhday = "";
            string gettrkhmonth = "";
            string gettrkhyear = "";
            string fldTarikh = "";
            string fldKdLdg = "";
            string fldNama = "";
            string fldNoKP = "";
            string fldJawatan = "";
            string fldNamaLdg = "";
            string fldUserid = "";
            string fldPassword = "";
            string fldStatus = "";
            string targetpathmix = "";

            var getapprovaluserid = db2.tblUserIDApps.Where(x => x.fldFileID == fileid && x.fldStatus == "1").ToList();
            var getfilename = db.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).Select(s => s.fldFileName).FirstOrDefault();
            var getkdldg = db.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).Select(s => s.fldCodeLadang).FirstOrDefault();
            string targetpath = CheckSharedFolder.GetSourceTargetPath("filetargetpathuseridapp", NegaraID, SyarikatID); //db.tbl_OptionConfig.Where(x => x.fldOptConfFlag1 == "filetargetpathuseridapp" && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
            targetpathmix = targetpath + getkdldg + "\\" + getfilename;
            checkcreatefolder(targetpath + getkdldg);
            TryToDelete(targetpathmix);
            using (StreamWriter writer = new StreamWriter(targetpathmix, true))
            {
                foreach (var useriddetail in getapprovaluserid)
                {
                    gettrkhday = string.Format("{0:dd}", useriddetail.fldTarikh);
                    gettrkhmonth = string.Format("{0:MM}", useriddetail.fldTarikh);
                    gettrkhyear = string.Format("{0:yyyy}", useriddetail.fldTarikh);
                    
                    fldNama = useriddetail.fldNama.Trim();
                    fldNoKP = useriddetail.fldNoKP.Trim();
                    fldJawatan = useriddetail.fldJawatan.Trim();
                    fldKdLdg = useriddetail.fldKdLdg.Trim();
                    fldNamaLdg = useriddetail.fldNamaLdg.Trim();
                    fldTarikh = gettrkhday + gettrkhmonth + gettrkhyear;
                    fldUserid = useriddetail.fldUserid.Trim();
                    fldPassword = useriddetail.fldPassword.Trim();
                    fldStatus = useriddetail.fldStatus.Trim();
                    getfilename = getfilename.Trim();

                    writer.Write(fldNama.PadRight(40, ' '));
                    writer.Write(fldNoKP.PadRight(17, ' '));
                    writer.Write(fldJawatan.PadRight(35, ' '));
                    writer.Write(fldKdLdg.PadRight(4, ' '));
                    writer.Write(fldNamaLdg.PadRight(40, ' '));
                    writer.Write(fldTarikh.PadRight(8, ' '));
                    writer.Write(fldUserid.PadRight(17, ' '));
                    writer.Write(fldPassword.PadRight(7, ' '));
                    writer.Write(fldStatus.PadRight(1, ' '));
                    writer.WriteLine(getfilename.PadRight(30, ' '));
                    //writer.Write(workerdetail.)
                }
            }
        }

        public void checkcreatefolder(string folderpath)
        {
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
        }

        static bool TryToDelete(string f)
        {
            try
            {
                // A.
                // Try to delete the file.
                File.Delete(f);
                return true;
            }
            catch (IOException)
            {
                // B.
                // We could not delete the file.
                return false;
            }
        }
    }
}

   