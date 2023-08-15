using System.Web.Mvc;
using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_HargaSawitSemasa
    {
        [Key]
        public int fld_ID { get; set; }

        public int? fld_Bulan { get; set; }

        [StringLength(1)]
        public string fld_JnsTnmn { get; set; }

        public decimal? fld_HargaSemasa { get; set; }

        public decimal? fld_Insentif { get; set; }

        public int? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_HargaSawitSemasaModelViewCreate
    {
        [Key]
        public int fld_ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_Bulan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(1)]
        public string fld_JnsTnmn { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        [Remote("IsPalmPriceExceedValue", "DataEntry", AdditionalFields = "fldOptConfFlag2", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "MsgModelValidationPalmPriceExcceedLimit")]
        public decimal? fld_HargaSemasa { get; set; }

        public decimal? fld_Insentif { get; set; }

        public int? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_HargaSawitSemasaModelViewEdit
    {
        [Key]
        public int fld_ID { get; set; }

        public int? fld_Bulan { get; set; }

        [StringLength(1)]
        public string fld_JnsTnmn { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        [Remote("IsPalmPriceExceedValue", "DataEntry", AdditionalFields = "fldOptConfFlag2", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "MsgModelValidationPalmPriceExcceedLimit")]
        public decimal? fld_HargaSemasa { get; set; }

        public decimal? fld_Insentif { get; set; }

        public int? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
