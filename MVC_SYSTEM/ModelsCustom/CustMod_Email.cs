using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCustom
{
    public class CustMod_Email
    {
        public int fldID { get; set; }

        public string fldEmail { get; set; }

        public string fldName { get; set; }

        public string fldCategory { get; set; }

        public string fldDepartment { get; set; }

        public bool? fldDeleted { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }
        //added by kamali 24/11/21
        public int? fldLadangID { get; set; }

        public string fldCostCenter { get; set; }

        public string fld_CreatedBy { get; set; }
        public string fld_ModifiedBy { get; set; }
        public DateTime? fld_DTCreated { get; set; }
        public DateTime? fld_DTModified { get; set; }

    }
}