using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CutiKategori
    {
        [Key]
        public int fld_CutiKategoriID { get; set; }

        [StringLength(50)]
        public string fld_KodCuti { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_KeteranganCuti { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public byte fld_WaktuBayaranCuti { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
