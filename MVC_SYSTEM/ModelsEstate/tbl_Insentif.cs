using System.Web.Mvc;
using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Insentif
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_InsentifID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(5)]
        public string fld_KodInsentif { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_NilaiInsentif { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }

    public partial class tbl_InsentifViewModelCreate
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_InsentifID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_JenisInsentif { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int fld_TetapanNilai { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(5)]
        public string fld_KodInsentif { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_NilaiTetap { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_NilaiHarian { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        [Remote("IsIncentiveWithinRange", "Maintenance", AdditionalFields = "fld_KodInsentif", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "MsgModelValidationIncentiveExceedRange")]
        public decimal? fld_NilaiTidakTetap { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_NilaiInsentif { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }

    public partial class tbl_InsentifViewModelEdit
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_InsentifID { get; set; }

        public string fld_KodInsentif { get; set; }

        public string fld_Keterangan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        [Remote("IsIncentiveWithinRange", "Maintenance", AdditionalFields = "fld_KodInsentif", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "MsgModelValidationIncentiveExceedRange")]
        public decimal? fld_NilaiTidakTetap { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }
    }

    public partial class tbl_GroupInsentifViewModelCreate
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_InsentifID { get; set; }

        public int fld_KumpulanID { get; set; }

        [StringLength(50)]
        public string fld_KodKumpulan { get; set; }

        [StringLength(50)]
        public string fld_KodKerja { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(5)]
        public string fld_KodInsentif { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_NilaiTetap { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_NilaiHarian { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        [Remote("IsIncentiveWithinRange", "Maintenance", AdditionalFields = "fld_KodInsentif", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "MsgModelValidationIncentiveExceedRange")]
        public decimal? fld_NilaiTidakTetap { get; set; }
    }

    public partial class tbl_InsentifViewModelCopy
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_InsentifID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_YearFrom { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_MonthFrom { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_YearTo { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_MonthTo { get; set; }

        public string fld_KodInsentif { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_WilayahID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }
}
