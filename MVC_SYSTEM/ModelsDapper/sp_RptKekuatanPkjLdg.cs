using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class sp_RptKekuatanPkjLdg_Result
    {
        public System.Guid fld_ID { get; set; }
        public string fld_NamaWilayah { get; set; }
        public string fld_KodLadang { get; set; }
        public string fld_NamaLadang { get; set; }
        public Nullable<int> fld_KeperluanSebenar { get; set; }
        public Nullable<int> fld_LuasKeseluruhan { get; set; }
        public Nullable<int> fld_PekerjaTempatan { get; set; }
        public Nullable<int> fld_PekerjaAsing { get; set; }
        public Nullable<int> fld_PekerjaKontraktor { get; set; }
        public Nullable<int> fld_NegaraID { get; set; }
        public Nullable<int> fld_SyarikatID { get; set; }
        public Nullable<int> fld_WilayahID { get; set; }
        public Nullable<int> fld_LadangID { get; set; }
        public Nullable<int> fld_CreatedBy { get; set; }
        public string fld_CompCode { get; set; }

    }
}