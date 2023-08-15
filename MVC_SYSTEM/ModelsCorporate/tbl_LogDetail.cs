namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_LogDetail
    {
        [Key]
        public long fld_ID { get; set; }

        public string fld_LogDetail { get; set; }

        public string fld_ErrorDetail { get; set; }

        public int? fld_ClientID { get; set; }

        public int? fld_Status { get; set; }

        public DateTime? fld_DTProcess { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        [StringLength(50)]
        public string fld_ASCFileName { get; set; }
    }
}
