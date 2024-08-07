using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class sp_MapaReport_Result
    {
        public System.Guid fld_Id { get; set; }
        public string fld_LdgCode { get; set; }
        public string fld_LdgName { get; set; }
        public Nullable<decimal> fld_Cash { get; set; }
        public Nullable<decimal> fld_Cheque { get; set; }
        public Nullable<decimal> fld_Cdmas { get; set; }
        public Nullable<decimal> fld_Ewallet { get; set; }
        public Nullable<int> fld_Month { get; set; }
        public Nullable<int> fld_Year { get; set; }
        public Nullable<int> fld_NegaraID { get; set; }
        public Nullable<int> fld_SyarikatID { get; set; }
        public Nullable<int> fld_WilayahID { get; set; }
        public Nullable<int> fld_LadangID { get; set; }
        public Nullable<int> fld_CreatedBy { get; set; }
        public Nullable<decimal> fld_Socso { get; set; }
        public Nullable<decimal> fld_Kwsp { get; set; }
        public Nullable<decimal> fld_Sip { get; set; }
        public Nullable<decimal> fld_Sbkp { get; set; }
        public Nullable<decimal> fld_LainPotongan { get; set; }
        public string Email { get; set; }
        public string fld_BranchName { get; set; }
        public string fld_NoAcc { get; set; }
        public Nullable<int> fld_CashPax { get; set; }
        public Nullable<int> fld_ChequePax { get; set; }
        public Nullable<int> fld_EwalletPax { get; set; }
        public Nullable<int> fld_CdmasPax { get; set; }
        public Nullable<System.DateTime> fld_PostingDate { get; set; }
        public string fld_NoDocSAP { get; set; }
        public Nullable<int> fld_Verify_By { get; set; }
        public Nullable<System.DateTime> fld_Verify_DT { get; set; }
        public Nullable<int> fld_SemakWil_By { get; set; }
        public Nullable<int> fld_SemakWil_Status { get; set; }
        public Nullable<int> fld_SokongWilGM_By { get; set; }
        public Nullable<int> fld_SokongWilGM_Status { get; set; }
        public Nullable<int> fld_TerimaHQ_By { get; set; }
        public Nullable<int> fld_TerimaHQ_Status { get; set; }
        public string fld_Verify_Name { get; set; }
        public string fld_SemakWil_Name { get; set; }
        public string fld_SokongWilGM_Name { get; set; }
        public string fld_TerimaHQ_Name { get; set; }
        public Nullable<decimal> fld_M2U { get; set; }
        public Nullable<decimal> fld_M2E { get; set; }
        public Nullable<int> fld_M2UPax { get; set; }
        public Nullable<int> fld_M2EPax { get; set; }
        public string fld_CompCode { get; set; }
        public Nullable<decimal> fld_SocsoMjkn { get; set; }
        public Nullable<decimal> fld_KwspMjkn { get; set; }
        public Nullable<decimal> fld_SipMjkn { get; set; }
        public Nullable<decimal> fld_SbkpMjkn { get; set; }
        public string fld_AkaunMerchProj { get; set; }
        public Nullable<int> fld_JumlahPax { get; set; }
        public Nullable<decimal> fld_CheckRollManual { get; set; }
        public Nullable<decimal> fld_MerchEwallet { get; set; }
        public string fld_NoDocSAPEwallet { get; set; }
        public Nullable<decimal> fld_CheckRollRML { get; set; }
        public Nullable<decimal> fld_EwalletTnG { get; set; }
        public Nullable<int> fld_EwalletTnGPax { get; set; }
        public Nullable<int> fld_TutupUrusniaga { get; set; }
    }
}