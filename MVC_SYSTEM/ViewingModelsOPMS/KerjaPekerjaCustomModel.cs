namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class KerjaPekerjaCustomModel
    {
        [Key]
        public Guid? fld_ID { get; set; }

        [StringLength(10)]
        public string fld_KodPkt { get; set; }

        [StringLength(10)]
        public string fld_Unit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KadarByr { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahHasil { get; set; }

        [StringLength(100)]
        public string fld_Desc { get; set; }

        public decimal? fld_TotalAmount { get; set; }

        public string fld_Gandaan { get; set; }
    }
}
