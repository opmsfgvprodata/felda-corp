//kamalia - 18.02.2021
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCustom
{
    public class CustMod_GajiMinima
    {
        public List<TiadaGajiMinima> TiadaGajiMinima { get; set; }
        public List<AdaGajiMinima> AdaGajiMinima { get; set; }

        //added by kamalia 24/02/2021
        public List<ListLadangAdaGajiMinima> ListLadangAdaGajiMinima { get; set; }

        public int fld_WilayahID { get; set; }

        public int fld_LadangID { get; set; }

        //added by kamalia 22/02/2021
        public int? fld_OptConfigID { get; set; }

    }
    public class TiadaGajiMinima
    {
        public int LadangID { get; set; }
        public string NamaLadang { get; set; }
    }
    public class AdaGajiMinima
    {
        public int LadangID { get; set; }

        public string NamaLadang { get; set; }

    }

    //added by kamalia 24/02/2021
    public class ListLadangAdaGajiMinima
    {
        public int? LadangID { get; set; }
        public int? WilayahID { get; set; }
        public string NamaLadang { get; set; }
        public string Namawilayah { get; set; }

    }
}