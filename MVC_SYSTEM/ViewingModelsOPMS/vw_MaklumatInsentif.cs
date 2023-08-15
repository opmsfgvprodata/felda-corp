namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_MaklumatInsentif
    {
        [Key]
        public Guid fld_InsentifID { get; set; }

        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(5)]
        public string fld_KodInsentif { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        public decimal? fld_NilaiInsentif { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }
    }
}
