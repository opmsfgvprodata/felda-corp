namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public partial class tbl_GajiMinimaLdg
    {
        [Key]
        public int fld_ID { get; set; }
        public decimal? fld_NilaiGajiMinima { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }
        public int? fld_OptConfigID { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        //added by kamalia 31/3/2021
        public DateTime? fld_ModifiedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }


    }
    [Table("tbl_GajiMinimaLdg")]
    public partial class LadangWithGajiMinima
    {
        public int? ladangid { get; set; }

        public int? wilayahid { get; set; }

        public string NamaWilayah { get; set; }
        public string NamaLadang { get; set; }
    }
}