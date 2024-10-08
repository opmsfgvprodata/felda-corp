namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class vw_SAPPostData
    {
        public Guid fld_PostRefID { get; set; }
        public int fld_Month { get; set; }
        public int fld_Year { get; set; }
        public string fld_HeaderText { get; set; }
        public string fld_RefNo { get; set; }
        public string fld_CompCode { get; set; }
        public DateTime fld_DocDate { get; set; }
        public DateTime fld_PostingDate { get; set; }
        public string fld_DocType { get; set; }
        public string fld_NoDocSAP { get; set; }
        public short fld_Purpose { get; set; }
        public int fld_NegaraID { get; set; }
        public int fld_SyarikatID { get; set; }
        public int fld_WilayahID { get; set; }
        public int fld_LadangID { get; set; }
        public bool fld_StatusProceed { get; set; }
        public int fld_ItemNo { get; set; }
        public string fld_GL { get; set; }
        public string fld_IO { get; set; }
        public string fld_SAPActivityCode { get; set; }
        public Decimal? fld_Amount { get; set; }
        public string fld_Desc { get; set; }
        public string fld_Currency { get; set; }
        public Guid fld_SAPPostRefID { get; set; }

        [Key]
        public Guid fld_PostDataID { get; set; }

        public string fld_CustCPD { get; set; }

        public string fld_Poskod { get; set; }

        public string fld_DistrictArea { get; set; }

        public string fld_CustDesc { get; set; }

        public string fld_VendorCode { get; set; }

        public int fld_flag { get; set; }

        public string fld_DocNoSAP { get; set; }

        public string fld_LdgCode { get; set; }

        public string fld_SAPType { get; set; }
    }
}