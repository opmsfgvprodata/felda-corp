using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CutiMaintenance
    {
        [Key]
        public int fld_CutiMaintenanceID { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        [StringLength(30)]
        public string fld_JenisCuti { get; set; }

        public int? fld_LowerLimit { get; set; }

        public int? fld_UpperLimit { get; set; }

        public int? fld_PeruntukkanCuti { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    [Table("tbl_CutiMaintenance")]
    public partial class tbl_CutiMaintenanceViewModelCreate
    {
        [Key]
        public int fld_CutiMaintenanceID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(30)]
        public string fld_JenisCuti { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxLargeIntegerModelValidation")]
        [RegularExpression("^\\d+(?:)?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_LowerLimit { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxLargeIntegerModelValidation")]
        [RegularExpression("^\\d+(?:)?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_UpperLimit { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxLargeIntegerModelValidation")]
        [RegularExpression("^\\d+(?:)?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_PeruntukkanCuti { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    [Table("tbl_CutiMaintenance")]
    public partial class tbl_CutiMaintenanceViewModelEdit
    {
        [Key]
        public int fld_CutiMaintenanceID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        [StringLength(30)]
        public string fld_JenisCuti { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxLargeIntegerModelValidation")]
        [RegularExpression("^\\d+(?:)?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_LowerLimit { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxLargeIntegerModelValidation")]
        [RegularExpression("^\\d+(?:)?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_UpperLimit { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxLargeIntegerModelValidation")]
        [RegularExpression("^\\d+(?:)?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_PeruntukkanCuti { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
