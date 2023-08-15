namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_LBPPL
    {
        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jumlah { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Tarikh { get; set; }

        [StringLength(5)]
        public string fld_UploadCdLdg { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }

        [Key]
        public Guid fld_UniqueID { get; set; }
    }
}
