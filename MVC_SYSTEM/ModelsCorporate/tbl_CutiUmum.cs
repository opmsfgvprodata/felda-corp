using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CutiUmum
    {
        [Key]
        public int fld_CutiUmumID { get; set; }

        [StringLength(100)]
        public string fld_KeteranganCuti { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhCuti { get; set; }

        [StringLength(10)]
        public string fld_Negeri { get; set; }

        public short? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_IsSelected { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CutiUmumMasterID { get; set; }
    }

    public partial class tbl_CutiUmumViewModelCreate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_CutiUmumID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_KeteranganCuti { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Column(TypeName = "date")]
        public DateTime? fld_TarikhCuti { get; set; }

        public List<tblOptionConfigsWebAnnualLeaveViewModelCreate> fld_IsSelected { get; set; }
    }

    public partial class tbl_CutiUmumViewModelEdit
    {
        [Key]
        public int fld_CutiUmumID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_KeteranganCuti { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Column(TypeName = "date")]
        public DateTime? fld_TarikhCuti { get; set; }

        public int? fld_Negeri { get; set; }

        public short? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_IsSelected { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_CutiUmumViewModelGenerate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_CutiUmumID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Column(TypeName = "date")]
        public int? fld_Year { get; set; }
    }
}
