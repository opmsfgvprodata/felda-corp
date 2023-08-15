namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public class tbl_VDSAP
    {
          [Key]
          public string fld_VendorNo { get; set; }
          public string fld_Desc { get; set; }
          public int? fld_NegaraID { get; set; }
          public int? fld_SyarikatID { get; set; }
          public DateTime? fld_DTCreated { get; set; }
          public DateTime? fld_DTModified { get; set; }
          public bool? fld_Deleted { get; set; }
        //farahin tambah - 3/8/2021
        public string fld_CreatedBy { get; set; }
        public string fld_ModifiedBy { get; set; }
        public string fld_CompanyCode { get; set; }
    }

    public class tbl_VDSAPCreate
    {
        [Required(ErrorMessage = "Vendor Code is required.")]
        [Display(Name = "VendorCode")]
        [StringLength(20)]
        public string fld_VendorNo { get; set; }

        public string fld_VendorNo2 { get; set; }
        public string fld_CompanyCode { get; set; }
    }
}