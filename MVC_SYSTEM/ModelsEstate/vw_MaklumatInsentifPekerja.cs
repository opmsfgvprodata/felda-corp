using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsEstate
{
    public class vw_MaklumatInsentifPekerja
    {
        public tbl_Pkjmast Pkjmast { get; set; }
        public vw_MaklumatInsentif insentif { get; set; }
        public List<vw_MaklumatInsentif> Pendapatan { get; set; }
        public List<vw_MaklumatInsentif> Potongan { get; set; }

        public ModelsCorporate.tbl_Wilayah wilayah { get; set; }
        public Models.tbl_Ladang ladang { get; set; }
    }
}


