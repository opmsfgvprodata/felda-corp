namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_TutupUrusNiaga
    {
        [Key]
        public int fld_ID { get; set; }

        public bool? fld_StsTtpUrsNiaga { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public decimal? fld_Credit { get; set; }

        public decimal? fld_Debit { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }
    }
}
