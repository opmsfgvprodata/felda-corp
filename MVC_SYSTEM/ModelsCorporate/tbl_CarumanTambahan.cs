using System.Web.Mvc;
using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CarumanTambahan
    {
        [Key]
        public int fld_JenisCarumanID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(30)]
        public string fld_NamaCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public bool fld_Berjadual { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int fld_CarumanOleh { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_Warganegara { get; set; }

        public bool fld_Default { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public bool fld_Deleted { get; set; }
    }

    public partial class tbl_CarumanTambahanViewModelCreate
    {
        [Key]
        public int fld_JenisCarumanID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Remote("IsContributionCodeExist", "Maintenance", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelDuplicateCode")]
        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(30)]
        public string fld_NamaCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public bool fld_Berjadual { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int fld_CarumanOleh { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_Warganegara { get; set; }

        public bool fld_Default { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public bool fld_Deleted { get; set; }
    }
}
