using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_UpahAktiviti
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(150)]
        public string fld_Desc { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        //added by kamalia 6/10/2021
        [Range(0, 9999999.9999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,4})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public short? fld_DisabledFlag { get; set; }

        [StringLength(1)]
        public string fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(1)]
        public string fld_Kategori { get; set; }

        //added by kamalia 6/10/2021
        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MaxProduktiviti { get; set; }

        [StringLength(2)]
        public string fld_KategoriAktvt { get; set; }

        //fatin added - 8/11/2023
        [StringLength(5)]
        public string fld_compcode { get; set; }
    }

    [Table("tbl_UpahAktiviti")]
    public partial class tbl_UpahAktivitiViewModel
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_Desc { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        //added by kamalia 6/10/2021
        [Range(0, 9999999.9999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,4})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public short? fld_DisabledFlag { get; set; }

        [StringLength(1)]
        public string fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        //added by kamalia 6/10/2021
        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MaxProduktiviti { get; set; }

        [StringLength(1)]
        public string fld_Kategori { get; set; }

        //fatin added - 8/11/2023
        [StringLength(5)]
        public string fld_compcode { get; set; }

    }

    [Table("tbl_UpahAktiviti")]
    public partial class tbl_UpahAktivitiViewModelCreate
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_Desc { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        //added by kamalia 6/10/2021
        [Range(0, 9999999.9999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,4})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public short? fld_DisabledFlag { get; set; }

        [StringLength(1)]
        public string fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        //added by kamalia 6/10/2021
        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MaxProduktiviti { get; set; }

        [StringLength(1)]
        public string fld_Kategori { get; set; }

        //fatin added - 8/11/2023
        [StringLength(5)]
        public string fld_compcode { get; set; }

    }

    [Table("tbl_UpahAktiviti")]
    public partial class tbl_UpahAktivitiViewModelGMN
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_Desc { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        //added by kamalia 6/10/2021
        [Range(0, 9999999.9999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,4})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public short? fld_DisabledFlag { get; set; }

        [StringLength(1)]
        public string fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        //added by kamalia 6/10/2021
        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MaxProduktiviti { get; set; }

        [StringLength(1)]
        public string fld_Kategori { get; set; }

        //fatin added - 8/11/2023
        [StringLength(5)]
        public string fld_compcode { get; set; }

    }
}
