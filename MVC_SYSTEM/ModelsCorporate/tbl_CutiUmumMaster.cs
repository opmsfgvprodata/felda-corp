using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CutiUmumMaster
    {
        [Key]
        public int fld_CutiUmumMasterID { get; set; }

        [StringLength(100)]
        public string fld_KeteranganCuti { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhCuti { get; set; }

        public short? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_CutiUmumMasterViewModelCreate
    {
        [Key]
        public int fld_CutiUmumMasterID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_KeteranganCuti { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Column(TypeName = "date")]
        public DateTime? fld_TarikhCuti { get; set; }

        public short? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_CutiUmumMasterViewModelEdit
    {
        [Key]
        public int fld_CutiUmumMasterID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_KeteranganCuti { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Column(TypeName = "date")]
        public DateTime? fld_TarikhCuti { get; set; }

        public short? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_CutiUmumMasterViewModelGenerate
    {
        [Key]
        public int fld_CutiUmumMasterID { get; set; }

        public short? fld_Tahun { get; set; }
    }
}
