namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_GajiBulanan
    {
        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama1 { get; set; }

        [StringLength(12)]
        public string fld_Nokp { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Gaji_Kasar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Epf_Mjk { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Socso_Mjk { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Epf_Pkj { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Socso_Pkj { get; set; }

        [Key]
        public Guid fld_UniqueID { get; set; }
    }
}
