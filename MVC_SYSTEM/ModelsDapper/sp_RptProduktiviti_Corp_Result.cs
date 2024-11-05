using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class sp_RptProduktiviti_Corp_Result
    {
        public int fldID { get; set; }
        public string fld_Nopkj { get; set; }
        public string fld_Nama { get; set; }
        public Nullable<decimal> fld_TargetHarian { get; set; }
        public string fld_UnitTargetHarian { get; set; }
        public Nullable<int> fld_HariBekerjaSebenar { get; set; }
        public Nullable<decimal> fld_Hasil { get; set; }
        public string fld_Unit { get; set; }
        public string fld_KodPkt { get; set; }
        public Nullable<decimal> fld_AmaunBayar { get; set; }
        public int? fld_LadangID { get; set; }
        public int? fld_WilayahID { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public int? fld_CreatedBy { get; set; }
        public string fld_LdgName { get; set; }
        public string fld_CostCentre { get; set; }

    }
}