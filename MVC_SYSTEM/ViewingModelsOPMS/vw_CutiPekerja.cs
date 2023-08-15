namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_CutiPekerja
    {
        [Key]
        [Column(Order = 0)]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(50)]
        public string fld_Kum { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Tarikh { get; set; }

        [StringLength(3)]
        public string fld_Kdhdct { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kadar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jumlah { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string fldOptConfDesc { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string fldOptConfFlag1 { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag2 { get; set; }
    }
}
