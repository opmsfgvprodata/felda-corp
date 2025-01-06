namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPHistory
    {
        [Key]
        public int? fld_ID  { get; set; }
        public Guid fld_ID_ref { get; set; }
        public string fld_HeaderText { get; set; }
        public int? fld_Month { get; set; }
        public int? fld_Year { get; set; }
        public string fld_CompCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_DocDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_PostingDate { get; set; }
        public string fld_DocType { get; set; }
        public string fld_RefNo { get; set; }
        public string fld_NoDocSAP { get; set; }
        public short? fld_Purpose { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public int? fld_WilayahID { get; set; }
        public int? fld_LadangID { get; set; }
        public int? fld_CreatedBy { get; set; }
        public DateTime? fld_CreatedDT { get; set; }
        public decimal? fld_Amount { get; set; }
        public string fld_VendorCode { get; set; }
        public string fld_DocNoSAP { get; set; }
        public int? fld_flag { get; set; }
    }
}
