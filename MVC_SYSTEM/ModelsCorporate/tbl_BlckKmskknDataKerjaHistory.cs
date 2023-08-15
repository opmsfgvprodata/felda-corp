namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_BlckKmskknDataKerjaHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        public bool? fld_BlokStatus { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        [StringLength(300)]
        public string fld_Reason { get; set; }

        public short? fld_BilHariXKyIn { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_UnBlockAppBy { get; set; }

        public DateTime? fld_UnBlockAppDT { get; set; }

        [StringLength(300)]
        public string fld_Remark { get; set; }
    }
}
