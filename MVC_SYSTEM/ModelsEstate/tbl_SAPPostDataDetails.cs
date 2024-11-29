namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPPostDataDetails
    {
        [Key]
        public Guid fld_ID { get; set; }

        public int? fld_ItemNo { get; set; }

        [StringLength(20)]
        public string fld_VendorCode { get; set; }

        [StringLength(100)]
        public string fld_Desc { get; set; }

        public decimal? fld_Amount { get; set; }

        [StringLength(10)]
        public string fld_Currency { get; set; }

        public Guid? fld_SAPPostRefID { get; set; }
        public int? fld_flag { get; set; }
        public string fld_DocNoSAP { get; set; }       
    }
}
