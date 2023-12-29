using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_JenisInsentif
    {
        [Key]
        public int fld_JenisInsentifID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_JenisInsentif { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(5)]
        public string fld_KodInsentif { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0.00, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MinValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0.00, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MaxValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0.00, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_FixedValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0.00, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_DailyFixedValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public bool? fld_AdaCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_TetapanNilai { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_KelayakanKepada { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(5)]
        public string fld_KodGL { get; set; }

        public bool? fld_Deleted { get; set; }

        //fatin added - 19/10/2023
        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public bool? fld_AdaORP { get; set; }
        
    }
}
