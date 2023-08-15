using System.Web.Mvc;
using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CutiUmumLdg
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_CutiUmumLdgID { get; set; }

        public int? fld_CutiMasterID { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }

        [StringLength(5)]
        public string fld_CreatedInd { get; set; }
    }

    public partial class tbl_CutiUmumLdgViewModelCreateHQ
    { 
        [Key]
        public Guid fld_CutiUmumLdgID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_WilayahID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public IEnumerable<int> fld_CutiMasterID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_Year { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public IEnumerable<int> fld_LadangID { get; set; }
    }
}
