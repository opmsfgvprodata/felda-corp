using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class sp_MaybankRcmsOthers_Result
    {
        public Nullable<System.Guid> fld_ID { get; set; }
        public string fld_Nokp { get; set; }
        public string fld_Nopkj { get; set; }
        public string fld_Nama { get; set; }
        public string fld_RcmsBankCode { get; set; }
        public string fld_NamaBank { get; set; }
        public string fld_NoAkaun { get; set; }
        public Nullable<decimal> fld_GajiBersih { get; set; }
        public Nullable<int> fld_Year { get; set; }
        public Nullable<int> fld_Month { get; set; }
        public Nullable<int> fld_NegaraID { get; set; }
        public Nullable<int> fld_SyarikatID { get; set; }
        public Nullable<int> fld_WilayahID { get; set; }
        public Nullable<int> fld_LadangID { get; set; }
        public string fld_LdgCode { get; set; }
        public string fld_LdgName { get; set; }
        public string fld_Kdrkyt { get; set; }
        public string fld_PaymentMode { get; set; }
        public string fld_CostCentre { get; set; }
        public string fld_LdgShortName { get; set; }
        public string fld_LdgIndicator { get; set; }
    }
}