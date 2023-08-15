using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CutiPeruntukan
    {
        [Key]
        public int fld_CutiPeruntukanID { get; set; }

        [StringLength(50)]
        public string fld_KodCuti { get; set; }

        [StringLength(50)]
        public string fld_NoPkj { get; set; }

        public short? fld_Tahun { get; set; }

        public int? fld_JumlahCuti { get; set; }

        public int? fld_JumlahCutiDiambil { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_CutiPeruntukanModelViewCreate
    {
        [Key]
        public int fld_CutiPeruntukanID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_KodCuti { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_NoPkj { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public short? fld_Tahun { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxSmallIntegerModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_JumlahCuti { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxSmallIntegerModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_JumlahCutiDiambil { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_WilayahID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_CutiPeruntukanModelViewEdit
    {
        [Key]
        public int fld_CutiPeruntukanID { get; set; }

        [StringLength(50)]
        public string fld_KodCuti { get; set; }

        [StringLength(50)]
        public string fld_NoPkj { get; set; }

        public short? fld_Tahun { get; set; }

        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxSmallIntegerModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_JumlahCuti { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxSmallIntegerModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_JumlahCutiDiambil { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
