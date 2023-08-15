namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_HargaSawitRange
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(3)]
        public string fld_KodHarga { get; set; }

        public decimal? fld_RangeHargaLower { get; set; }

        public decimal? fld_RangeHargaUpper { get; set; }

        public decimal? fld_Insentif { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
