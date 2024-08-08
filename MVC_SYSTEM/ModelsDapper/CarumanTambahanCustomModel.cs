using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class CarumanTambahanCustomModel
    {
      
        public int? fld_ID { get; set; }

        public string fld_KodCarumanTambahan { get; set; }

        public decimal? fld_CarumanPekerja { get; set; }

        public decimal? fld_CarumanMajikan { get; set; }

        public decimal? fld_CarumanMajikanKwsp { get; set; }

        public int? ladangid { get; set; }
        public string fld_accountNo { get; set; }
        public string fld_NoGL { get; set; }
        public string fld_zonCIT { get; set; }

    }
}