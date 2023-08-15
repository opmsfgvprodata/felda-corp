using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Class
{
    public class GetTriager
    {
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        public string BlnTriger(int month)
        {
            string result = "NO";

            if (month <= timezone.gettimezone().Month || timezone.gettimezone().Month == 12)
            {
                result = "YES";
            }

            return result;
        }

        public string GetDashForInt (int? no)
        {
            string result = "";
            
            if (no == 0 || !no.HasValue)
            {
                result = "-";
            }
            else
            {
                result = no.ToString();
            }

            return result;
        }

        public string GetDashForDec(double? no)
        {
            string result = "";

            if (no == 0 || !no.HasValue)
            {
                result = "-";
            }
            else
            {
                result = no.ToString();
            }

            return result;
        }

        public string GetDashForDec2(decimal? no)
        {
            string result = "";

            if (no == 0 || !no.HasValue)
            {
                result = "-";
            }
            else
            {
                result = Math.Round((Double)no, 2).ToString("N");
            }

            return result;
        }

        public string GetTotalForMoney(decimal? no)
        {
            string result = "";

            if (no == 0 || !no.HasValue)
            {
                result = "0.00";
            }
            else
            {
                result = Math.Round((Double)no, 2).ToString("N");
            }

            return result;
        }

        public decimal? GetTotalForMoney2(decimal? no)
        {
            decimal? result = 0;

            if (no == 0 || !no.HasValue)
            {
                result = 0;
            }
            else
            {
                result = no;
            }

            return result;
        }

        public string GetMonthName(int month)
        {
            string result = "";

            switch (month)
            {
                case 1:
                    result = GlobalResGeneral.hdrM1;
                    break;
                case 2:
                    result = GlobalResGeneral.hdrM2;
                    break;
                case 3:
                    result = GlobalResGeneral.hdrM3;
                    break;
                case 4:
                    result = GlobalResGeneral.hdrM4;
                    break;
                case 5:
                    result = GlobalResGeneral.hdrM5;
                    break;
                case 6:
                    result = GlobalResGeneral.hdrM6;
                    break;
                case 7:
                    result = GlobalResGeneral.hdrM7;
                    break;
                case 8:
                    result = GlobalResGeneral.hdrM8;
                    break;
                case 9:
                    result = GlobalResGeneral.hdrM9;
                    break;
                case 10:
                    result = GlobalResGeneral.hdrM10;
                    break;
                case 11:
                    result = GlobalResGeneral.hdrM11;
                    break;
                case 12:
                    result = GlobalResGeneral.hdrM12;
                    break;
            }

            return result;
        }

        public string getEmailStatusIDApplicant(int batchno, int Purpose, int NegaraID, int SyarikatID, int WilayahID, int LadangID, string EmailSource)
        {
            string status = "";
            var getemailsendstatus = db.vw_ApplicationInfo.Where(x => x.fldID == batchno && x.fldASCFileStatus == 1 && x.tblASCApprovalFileDetailfldPurpose == Purpose && x.fldEmailNotiSource == EmailSource && x.fldEmailNotiStatus == 1).Count();
            if (getemailsendstatus > 0)
            {
                status = "Email telah berjaya dihantar kepada pihak bertanggungjawab";
            }
            return status;
        }

        public static string GetPercentageDivision(decimal? no)
        {
            decimal? result = 0;

            if (no == 0 || !no.HasValue)
            {
                result = 0;
            }
            else
            {
                result = no / 100;
            }

            return result.ToString();
        }

        public string GetGender(string gender, int NegaraID, int SyarikatID)
        {
            MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
            var result = "";
            var genderDesc = dbhq.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == gender && x.fldOptConfFlag1 == "jantina" && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                .Select(s => s.fldOptConfDesc).SingleOrDefault();

            if (string.IsNullOrEmpty(genderDesc))
            {
                result = "";
            }
            else
            {
                result = genderDesc;
            }

            return result;
        }

        public string GetWorkerRole(string role, int NegaraID, int SyarikatID)
        {
            MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
            var result = "";
            var roleDesc = dbhq.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == role.ToString() && x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                .Select(s => s.fldOptConfDesc).SingleOrDefault();

            if (string.IsNullOrEmpty(roleDesc))
            {
                result = "";
            }
            else
            {
                result = roleDesc;
            }

            return result;
        }
    }
}