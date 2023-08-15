namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_skb
    {
        [Key]
        public Guid fld_UniqueID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Amt { get; set; }

        [StringLength(15)]
        public string fld_skb { get; set; }

        public int? fld_Tahun { get; set; }

        [StringLength(2)]
        public string fld_Bulan { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        [StringLength(3)]
        public string fld_Lejar { get; set; }
    }
}
