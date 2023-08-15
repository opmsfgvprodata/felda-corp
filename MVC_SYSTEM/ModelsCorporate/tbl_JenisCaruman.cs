using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_JenisCaruman
    {
        [Key]
        public int fld_JenisCarumanID { get; set; }

        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        [StringLength(30)]
        public string fld_JenisCaruman { get; set; }

        public int? fld_UmurLower { get; set; }

        public int? fld_UmurUpper { get; set; }

        public decimal? fld_PeratusCarumanPekerja { get; set; }

        public decimal? fld_PeratusCarumanMajikanBawah5000 { get; set; }

        public decimal? fld_PeratusCarumanMajikanAtas5000 { get; set; }

        [StringLength(70)]
        public string fld_Keterangan { get; set; }

        public bool? fld_Deleted { get; set; }

        public int fldSyarikatID { get; set; }

        public int fldNegaraID { get; set; }

        public bool? fld_Default { get; set; }
    }

    public partial class tbl_JenisCarumanKWSPViewModelCreate
    {
        [Key]
        public int fld_JenisCarumanID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(30)]
        public string fld_JenisCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxAgeModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_UmurLower { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxAgeModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_UmurUpper { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanPekerja { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanMajikanBawah5000 { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanMajikanAtas5000 { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(70)]
        public string fld_Keterangan { get; set; }

        public bool? fld_Deleted { get; set; }

        public int fldSyarikatID { get; set; }

        public int fldNegaraID { get; set; }

        public bool? fld_Default { get; set; }
    }

    public partial class tbl_JenisCarumanKWSPViewModelEdit
    {
        [Key]
        public int fld_JenisCarumanID { get; set; }

        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        [StringLength(30)]
        public string fld_JenisCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxAgeModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_UmurLower { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxAgeModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_UmurUpper { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanPekerja { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanMajikanBawah5000 { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanMajikanAtas5000 { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(70)]
        public string fld_Keterangan { get; set; }

        public bool? fld_Deleted { get; set; }

        public int fldSyarikatID { get; set; }

        public int fldNegaraID { get; set; }

        public bool? fld_Default { get; set; }
    }

    public partial class tbl_JenisCarumanSOCSOViewModelCreate
    {
        [Key]
        public int fld_JenisCarumanID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(30)]
        public string fld_JenisCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxAgeModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_UmurLower { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxAgeModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_UmurUpper { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanPekerja { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanMajikanBawah5000 { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanMajikanAtas5000 { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(70)]
        public string fld_Keterangan { get; set; }

        public bool? fld_Deleted { get; set; }

        public int fldSyarikatID { get; set; }

        public int fldNegaraID { get; set; }

        public bool? fld_Default { get; set; }
    }

    public partial class tbl_JenisCarumanSOCSOViewModelEdit
    {
        [Key]
        public int fld_JenisCarumanID { get; set; }

        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        [StringLength(30)]
        public string fld_JenisCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxAgeModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_UmurLower { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxAgeModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_UmurUpper { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanPekerja { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanMajikanBawah5000 { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_PeratusCarumanMajikanAtas5000 { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(70)]
        public string fld_Keterangan { get; set; }

        public bool? fld_Deleted { get; set; }

        public int fldSyarikatID { get; set; }

        public int fldNegaraID { get; set; }

        public bool? fld_Default { get; set; }
    }
}
