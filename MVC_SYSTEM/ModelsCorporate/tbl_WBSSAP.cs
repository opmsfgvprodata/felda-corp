using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCorporate
{
    public class tbl_WBSSAP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fld_ID { get; set; }
       public string fld_wbsElement { get; set; }
        public string fld_wbsDescription { get; set; }
        public string fld_createdby { get; set; }
        public DateTime? fld_createdDate { get; set; }
        public string fld_updatedby { get; set; }
        public DateTime? fld_updatedDate { get; set; }
        public bool? fld_Deleted { get; set; }

    }

    [Table("tbl_WBSSAP")]
    public class tbl_WBSSAPCreate
    {

        public string fld_CompanyCode { get; set; }

        [Required(ErrorMessage = "WBS Code is required.")]
        [Display(Name = "WBS Element")]
        public string fld_wbsElement { get; set; }
       
    }

   
}