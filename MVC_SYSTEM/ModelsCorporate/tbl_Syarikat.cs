using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Syarikat
    {
        [Key]
        public int fld_SyarikatID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_NamaSyarikat { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(20)]
        public string fld_NamaPndkSyarikat { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(50)]
        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public string fld_NoSyarikat { get; set; }

        [StringLength(50)]
        public string fld_LogoName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_SyarikatEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_FrstNmeUsrNme { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_RequestCode { get; set; }

        //farahin tambah 22/09/2021
        public string fld_SAPComCode { get; set; }

        //add by faeza 28.08.2023
        public string fld_CorporateID { get; set; }
        public string fld_ClientBatchID { get; set; }
        public string fld_AccountNo { get; set; }
    }
}
