//kamalia - 18.02.2021
using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCustom
{
     public class CustMod_HargaKesukaran
    {
   
        public int fldOptConfID { get; set; }

        [StringLength(50)]
        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public string fldOptConfValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fldOptConfDesc { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag1 { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag2 { get; set; }

        public bool? fldDeleted { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag3 { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        [StringLength(100)]
        public string JenisHargaKesukaran { get; set; }
    }


}