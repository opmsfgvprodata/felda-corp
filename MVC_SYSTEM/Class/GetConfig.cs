using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ConfigModels;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsEstate;

namespace MVC_SYSTEM.Class
{
    public class GetConfig
    {
        MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        MVC_SYSTEM_Config config = new MVC_SYSTEM_Config();
        ChangeTimeZone changeTimeZone = new ChangeTimeZone();
        Connection Connection = new Connection();

        //new Class
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();

        public string GetData(string data)
        {
            
            ConfigModels.tblSystemConfig Config;
            string value = "";
            Config = config.tblSystemConfigs.Where(u => u.fldFlag1.Equals(data)).FirstOrDefault();
            if (Config != null)
            {
                value = Config.fldConfigValue.ToString();
            }

            return value;
        }

        public string GetData2(string data, string flag1)
        {
            var getvalue = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();

            return getvalue;
        }

        public int GetRole(string flag1)
        {
            string getvalue = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == flag1 && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();

            return int.Parse(getvalue);
        }

        public int GetConfigValueParseIntData(string flag1, int? NegaraID, int? SyarikatID)
        {
            string getvalue = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == flag1 && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfValue).FirstOrDefault();

            return int.Parse(getvalue);
        }

        public string GetDescData(string value, string flag1, int? NegaraID, int? SyarikatID)
        {
            string getdesc = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == value && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfDesc).FirstOrDefault();

            return getdesc;
        }

        public string GetWebConfigDesc(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfDesc)
                .FirstOrDefault();

            return getvalue;
        }

        public string GetWebConfigDescFromFlag2(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag2 == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfDesc)
                .FirstOrDefault();

            return getvalue;
        }

        public string GetWebConfigDescFromFlag2a(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == data && x.fldOptConfFlag2 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfDesc)
                .FirstOrDefault();

            return getvalue;
        }

        public string GetWebConfigDescFromFlag3(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag3 == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfDesc)
                .FirstOrDefault();

            return getvalue;
        }

        public string GetWebConfigValue(string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == flag1 && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
                && x.fldDeleted == false).Select(s => s.fldOptConfValue)
                .FirstOrDefault();
            return getvalue;
        }

        public string GetWebConfigFlag2FromValue(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfFlag2)
                .FirstOrDefault();

            return getvalue;
        }

        public string GetBank(string kod, int negara, int syrkt)
        {
            string bankname = db.tbl_Bank.Where(x => x.fld_KodBank == kod && x.fld_NegaraID == negara && x.fld_SyarikatID == syrkt && x.fld_Deleted == false).Select(s => s.fld_NamaBank).FirstOrDefault();
            return bankname;
        }

        public string GetKwspSocso(string kod, int syrkt, int negara)
        {
            string name = db.tbl_JenisCaruman.Where(x => x.fld_KodCaruman == kod && x.fldNegaraID == negara && x.fldSyarikatID == syrkt && x.fld_Deleted == false).Select(s => s.fld_Keterangan).FirstOrDefault();
            return name;
        }

        public string GetAktvt(string code, int negara, int syarikat)
        {
            var aktvt = db.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_Desc).FirstOrDefault();
            return aktvt;
        }

        public void GetCutiDesc(string data, string flag1, out string keterangan, out string status, out short KadarByrn, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).FirstOrDefault();
            keterangan = getvalue.fldOptConfDesc;
            status = getvalue.fldOptConfFlag2;
            KadarByrn = short.Parse(getvalue.fldOptConfFlag3);
        }

        public string GetPembekal(string kodbkl, int ngra, int syrkt)
        {
            string pembekal = db.tbl_Pembekal.Where(x => x.fld_KodPbkl == kodbkl && x.fld_NegaraID == ngra && x.fld_SyarikatID == syrkt && x.fld_Deleted == false).Select(s => s.fld_NamaPbkl).FirstOrDefault();
            return pembekal;
        }

        public decimal? UpahManual(decimal Hasil, int? NegaraID, int? SyarikatID)
        {
            decimal? upah = db.tbl_UpahMenuai.Where(x => x.fld_Jadual == "A" && x.fld_HasilLower <= Hasil && x.fld_HasilUpper >= Hasil && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Kadar).FirstOrDefault();
            return upah;
        }

        public decimal? UpahMesin(decimal Hasil, int? NegaraID, int? SyarikatID)
        {
            decimal? upah = db.tbl_UpahMenuai.Where(x => x.fld_Jadual == "B" && x.fld_HasilLower <= Hasil && x.fld_HasilUpper >= Hasil && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Kadar).FirstOrDefault();
            return upah;
        }

        public string Insentif(int kod, int? NegaraID, int? SyarikatID)
        {
            string insentif = "";
            string code = kod.ToString();
            insentif = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "insentif" && x.fldOptConfValue == code && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfDesc).FirstOrDefault();
            return insentif;
        }

        public string GetStatusAktif(string kod, int? NegaraID, int? SyarikatID)
        {
            var status = "";
            if (kod != null)
            {
                status = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldOptConfValue == kod && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfDesc).FirstOrDefault();
            }
            return status;
        }

        public string GetAdditionalContributionDesc(string kod, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tbl_SubCarumanTambahan
                .Where(x => x.fld_KodSubCaruman == kod && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
                            && x.fld_Deleted == false).Select(s => s.fld_KeteranganSubCaruman)
                .FirstOrDefault();
            return getvalue;
        }

        public string GetAdditionalMainContributionDesc(string kod, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tbl_CarumanTambahan
                .Where(x => x.fld_KodCaruman == kod && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
                            && x.fld_Deleted == false).Select(s => s.fld_NamaCaruman)
                .FirstOrDefault();
            return getvalue;
        }

        public string GetCompanyGroupDesc(int id)
        {
            var getvalue = db.tbl_KumpulanSyarikat
                .Where(x => x.fld_KmplnSyrktID == id)
                .Select(s => s.fld_NamaKmplnSyrkt)
                .FirstOrDefault();
            return getvalue;
        }

        public string GetGLDesc(string kodLejar, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tbl_Lejar
                .Where(x => x.fld_KodLejar == kodLejar
                            && x.fld_Deleted == false)
                .Select(s => s.fld_Desc)
                .FirstOrDefault();

            var GLCodeDesc = kodLejar + " - " + getvalue;

            return GLCodeDesc;
        }

        public string GetCompanyCountryDesc(int id)
        {

            var getData = from tbl_Negara in db.tbl_Negara
                          join tbl_KumpulanSyarikat in db.tbl_KumpulanSyarikat on tbl_Negara.fld_KmplnSyrktID equals
                          tbl_KumpulanSyarikat.fld_KmplnSyrktID
                          where tbl_Negara.fld_NegaraID == id
                          select new { Negara = tbl_Negara.fld_NamaNegara, KumpulanSyarikat = tbl_KumpulanSyarikat.fld_NamaKmplnSyrkt };

            var getvalue = getData.Select(x => x.KumpulanSyarikat).FirstOrDefault() + " (" + getData.Select(x => x.Negara).FirstOrDefault() + ")";

            return getvalue;
        }

        public string GetIncentiveCodeFromID(int id)
        {
            var getvalue = db.tbl_JenisInsentif
                .Where(x => x.fld_JenisInsentifID == id)
                .Select(s => s.fld_KodInsentif).SingleOrDefault();

            return getvalue;
        }

        public string GetIncentiveDescFromCode(string incentiveCode, int? NegaraID, int? SyarikatID)
        {

            var getvalue = db.tbl_JenisInsentif
                .Where(x => x.fld_KodInsentif == incentiveCode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fld_Keterangan).SingleOrDefault();

            return getvalue;
        }

        public bool GetIncentiveIsValidRange(string incentiveCode, decimal incentiveValue, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tbl_JenisInsentif
                .SingleOrDefault(x => x.fld_KodInsentif == incentiveCode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID);

            var result = getvalue.fld_MinValue <= incentiveValue && getvalue.fld_MaxValue >= incentiveValue;

            return result;
        }

        public string GetPaidLeaveDescFromCode(string paidLeaveCode, int? NegaraID, int? SyarikatID)
        {

            var getvalue = db.tbl_CutiKategori
                .Where(x => x.fld_KodCuti == paidLeaveCode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fld_KeteranganCuti).SingleOrDefault();

            return getvalue;
        }

        public string GetLadangNegeriFromID(int? id)
        {
            var getvalue = db.tbl_Ladang
                .SingleOrDefault(x => x.fld_ID == id).fld_KodNegeri.ToString();

            return getvalue;
        }

        //atun tmbah 19/4
        public string GetAktvtCode(string code, int negara, int syarikat)
        {
            var aktvtCode = db.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_KodAktvt).FirstOrDefault();
            return aktvtCode;
        }

        //atun tmbah 25/4
        public string GetKodAktvt(string code, int negara, int syarikat)
        {
            var GLCode = db.tbl_MapGL.Where(x => x.fld_KodAktvt == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_KodAktvt).FirstOrDefault();
            return GLCode;
        }

        public string GetKodGL(string code, int negara, int syarikat)
        {
            var GLCode = db.tbl_MapGL.Where(x => x.fld_KodGL == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_KodGL).FirstOrDefault();
            return GLCode;
        }

        //atun tmbah 15/5
        public string GetjnsAktvtCode(string code, int negara, int syarikat)
        {
            var aktvtCode = db.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_KodAktvt).FirstOrDefault();
            return aktvtCode;
        }

        public string GetjnsAktvt(string code, int negara, int syarikat)
        {
            var aktvt = db.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_Desc).FirstOrDefault();
            return aktvt;
        }

        public string GetLadangKodFromID(int? id)
        {
            var getvalue = db.tbl_Ladang
                .SingleOrDefault(x => x.fld_ID == id).fld_LdgCode;

            return getvalue;
        }

        public string GetLadangNameFromID(int? id)
        {
            var getvalue = db.tbl_Ladang
                .SingleOrDefault(x => x.fld_ID == id).fld_LdgName;

            return getvalue;
        }

        public string GetWilayahNameFromID(int? id)
        {
            var getvalue = db.tbl_Wilayah
                .SingleOrDefault(x => x.fld_ID == id).fld_WlyhName;

            return getvalue;
        }

        public short? getTotalOfferedWorkingDaysInAMonth(int? NegaraID, int? SyarikatID, int? LadangID, int? Month, int? Year)
        {
            var getKodNegeriLadang = Convert.ToInt32(db.tbl_Ladang.SingleOrDefault(x => x.fld_ID == LadangID).fld_KodNegeri);

            //farahin comment yg asal -21012021
            //var totalOfferedHariBekerja = db.tbl_HariBekerja.SingleOrDefault(x =>
            //    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Month == Month &&
            //    x.fld_Year == Year && x.fld_NegeriID == getKodNegeriLadang && x.fld_Deleted == false).fld_BilanganHariBekerja;

            //return totalOfferedHariBekerja;

            //farahin edit-21012021
            var totalOfferedHariBekerja = db.tbl_HariBekerjaLadang.SingleOrDefault(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Month == Month &&
                x.fld_Year == Year && x.fld_Deleted == false).fld_BilHariBekerja;

            return (short)totalOfferedHariBekerja;

        }

        //table tbl_CCSAP
        public string GetCostCenterMainCode(string code, int negara, int syarikat)
        {
            var ccCodecode = db.tbl_CCSAP.Where(x => x.fld_CstCnter == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false)
                .Select(s => s.fld_CstCnter).FirstOrDefault();
            return ccCodecode;
        }

        public string GetCostCenterMainDesc(string code, int negara, int syarikat)
        {
            var ccDesc = db.tbl_CCSAP.Where(x => x.fld_CstCnter == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false)
                .Select(s => s.fld_Desc).FirstOrDefault();
            return ccDesc;
        }

        //table tbl_CostCentre
        public string Getkatxtiviti(string code, int negara, int syarikat)
        {
            var katxtivitiCode = db.tbl_KategoriAktiviti.Where(x => x.fld_KodKategori == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false)
                .Select(s => s.fld_KodKategori).FirstOrDefault();
            return katxtivitiCode;
        }

        public string GetCostCent(string code, int negara, int syarikat)
        {
            var ccCode = db.tbl_CostCentre.Where(x => x.fld_CostCentre == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false)
                .Select(s => s.fld_CostCentre).FirstOrDefault();
            return ccCode;
        }

        public string GetkatxtivitiDesc(string code, int negara, int syarikat)
        {
            var katxtivitiCodeDesc = db.tbl_KategoriAktiviti.Where(x => x.fld_KodKategori == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false)
                .Select(s => s.fld_Kategori).FirstOrDefault();
            return katxtivitiCodeDesc;
        }

        public string UppercaseFirst(string s)
        {
            string toLower = s.ToLower();

            char[] array = toLower.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

        public void AddUserAuditTrail(int? userID, string userAction)
        {
            tblUserAuditTrail userAuditTrail = new tblUserAuditTrail();

            userAuditTrail.fld_UserActivity = userAction;
            userAuditTrail.fld_CreatedBy = userID;
            userAuditTrail.fld_CreatedDT = changeTimeZone.gettimezone();

            db.tblUserAuditTrails.Add(userAuditTrail);
            db.SaveChanges();
        }

        public string DateToString(DateTime? dateTime)
        {
            var dateString = "";

            dateString = String.Format("{0:dd/MM/yyyy}", dateTime);

            return dateString;
        }

        public string GetPkjNameFromNoPkj(string noPkj, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangiD)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            var pkjName = estateConnection.tbl_Pkjmast.SingleOrDefault(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                x.fld_LadangID == LadangiD && x.fld_Nopkj == noPkj).fld_Nama;

            return pkjName;
        }

        //Added by Shazana 1/8/2023
        public string GetSyarikatFullName(string costcentre)
        {
            tblOptionConfigsWeb OptionConfigsWeb = new tblOptionConfigsWeb();

            var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fldOptConfValue == costcentre).FirstOrDefault();
            var SyarikatFullName = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatInfo.fld_SyarikatID && x.fld_NoSyarikat.Contains(syarikatInfo.fldOptConfDesc) && x.fld_NegaraID == syarikatInfo.fld_NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();

            return SyarikatFullName;
        }

        //Added by Shazana 1/8/2023
        public string GetSyarikatName(string costcentre)
        {
            var SyarikatName = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fldOptConfValue == costcentre).Select(s => s.fldOptConfDesc).FirstOrDefault();
            return SyarikatName;
        }
    }
}