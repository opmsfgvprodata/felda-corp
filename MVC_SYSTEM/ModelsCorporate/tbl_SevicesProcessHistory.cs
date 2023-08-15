namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SevicesProcessHistory
    {
        [Key]
        public long fld_ID { get; set; }

        [StringLength(50)]
        public string fld_ProcessName { get; set; }

        [StringLength(50)]
        public string fld_ServicesName { get; set; }

        public int? fld_Flag { get; set; }

        public int? fld_UserID { get; set; }

        public DateTime? fld_DTProcess { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_UplSelCat { get; set; }

        public int? fld_SelCatVal { get; set; }

        public int? fld_ClientID { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public DateTime? fld_DTEndProcess { get; set; }

        public long? fld_ProcessID { get; set; }
    }
}
