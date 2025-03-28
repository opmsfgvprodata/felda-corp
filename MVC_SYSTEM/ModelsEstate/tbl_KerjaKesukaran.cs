namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_KerjaKesukaran
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(50)]
        public string fld_Kum { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Tarikh { get; set; }

        [StringLength(20)]
        public string fld_KodKesukaran { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kuantiti { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kadar { get; set; }

        public short? fld_Gandaan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jumlah { get; set; }

        public Guid? fld_KerjaID { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }
}
