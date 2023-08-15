namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_HargaSawitSemasa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }

        [StringLength(1)]
        public string fld_JnsTnmn { get; set; }

        public decimal fld_HargaSemasa { get; set; }

        public int? fld_Bulan { get; set; }

        public int? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }

        public decimal? fld_RangeHargaLower { get; set; }

        public decimal? fld_RangeHargaUpper { get; set; }

        public decimal? fld_Insentif { get; set; }
    }
}
