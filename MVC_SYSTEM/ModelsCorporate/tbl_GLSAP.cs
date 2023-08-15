namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_GLSAP
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(20)]
        public string fld_GLcode { get; set; }

        [StringLength(50)]
        public string fld_Desc { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }

        public DateTime fld_DTCreated { get; set; }

        public DateTime fld_DTModified { get; set; }
        //farahin tambah - 3/8/2021
        public string fld_CreatedBy { get; set; }
        public string fld_ModifiedBy { get; set; }
        public string fld_CompanyCode { get; set; }
    }

    [Table("tbl_GLSAP")]
    public partial class tbl_GLSAPCreate
    {
        [Required(ErrorMessage = "GL Number is required.")]
        [Display(Name = "GLNumber")]
        [StringLength(20)]
        public string fld_GLcode { get; set; }

        [Display(Name = "GLNumber2")]
        [StringLength(20)]
        public string fld_GLcode2 { get; set; }

        public string fld_CompanyCode { get; set; }
    }
}
