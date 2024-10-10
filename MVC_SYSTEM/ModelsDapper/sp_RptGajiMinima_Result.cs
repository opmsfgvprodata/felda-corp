using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsDapper;

namespace MVC_SYSTEM.ModelsDapper
{
    public class sp_RptGajiMinima_Result
    {
        public System.Guid fld_ID { get; set; }
        public string fld_NamaWilayah { get; set; }
        public string fld_KodLadang { get; set; }
        public string fld_NoPkj { get; set; }
        public string fld_Nama { get; set; }
        public string fld_NoKP { get; set; }
        public string fld_KodWarganegara { get; set; }
        public string fld_KodKategoriPekerja { get; set; }
        public Nullable<int> fld_BilanganTawaranHariBekerja { get; set; }
        public Nullable<int> fld_BilanganHariBekerjaSebenar { get; set; }
        public Nullable<decimal> fld_GajiBulanan { get; set; }
        public string fld_SebabGajiMinima { get; set; }
        public string fld_TindakanGajiMinima { get; set; }
        public Nullable<int> fld_Bulan { get; set; }
        public Nullable<int> fld_Tahun { get; set; }
        public Nullable<int> fld_NegaraID { get; set; }
        public Nullable<int> fld_SyarikatID { get; set; }
        public Nullable<int> fld_WIlayahID { get; set; }
        public Nullable<int> fld_LadangID { get; set; }
        public Nullable<int> fld_CreatedBy { get; set; }
        public string fld_CostCentre { get; set; }
    }
}