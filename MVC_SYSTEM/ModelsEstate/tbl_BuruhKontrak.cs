using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_BuruhKontrak
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fld_ID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(5)]
        public string fld_LadangCode { get; set; }

        [StringLength(50)]
        public string fld_LadangName { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        [StringLength(50)]
        public string fld_Designation { get; set; }

        public int? fld_JumlahBuruh { get; set; }
        public int? fld_Tahun { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_BuruhKontrakModelViewCreate
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fld_ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_LadangID { get; set; }

        [StringLength(5)]
        public string fld_LadangCode { get; set; }

        [StringLength(50)]
        public string fld_LadangName { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_WilayahID { get; set; }
        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        [StringLength(50)]
        public string fld_Designation { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxSmallIntegerModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_JumlahBuruh { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_Tahun { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_BuruhKontrakModelViewEdit
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fld_ID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(5)]
        public string fld_LadangCode { get; set; }

        [StringLength(50)]
        public string fld_LadangName { get; set; }

        public int? fld_WilayahID { get; set; }
        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        [StringLength(50)]
        public string fld_Designation { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxSmallIntegerModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int? fld_JumlahBuruh { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_Tahun { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
