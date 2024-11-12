namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ByrCarumanTambahan
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        [StringLength(10)]
        public string fld_KodSubCaruman { get; set; }

        public decimal? fld_CarumanPekerja { get; set; }

        public decimal? fld_CarumanMajikan { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public Guid? fld_GajiID { get; set; }
    }
}
