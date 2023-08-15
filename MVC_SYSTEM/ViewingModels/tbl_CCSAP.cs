
namespace MVC_SYSTEM.ViewingModels
{
    using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_CCSAP")]
    public class tbl_CCSAP
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(12)]
        public string fld_CstCnter { get; set; }

        [StringLength(50)]
        public string fld_Desc { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public DateTime? fld_DTCreated { get; set; }

        public DateTime? fld_DTModified { get; set; }
        //farahin tambah - 3/8/2021
        public string fld_CreatedBy { get; set; }
        public string fld_ModifiedBy { get; set; }
        public string fld_CompanyCode { get; set; }
    }
}