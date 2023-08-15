namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OTPekerjaCustomModel
    {
        [Key]
        public Guid? fld_ID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahJamOT { get; set; }

        [StringLength(10)]
        public string fld_Unit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KadarByr { get; set; }

        [StringLength(100)]
        public string fld_Desc { get; set; }

        public decimal? fld_TotalAmount { get; set; }

        public string fld_Gandaan { get; set; }
    }
}
