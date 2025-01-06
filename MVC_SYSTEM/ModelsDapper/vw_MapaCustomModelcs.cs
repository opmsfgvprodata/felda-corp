using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsDapper;

namespace MVC_SYSTEM.ModelsDapper
{
    public class vw_MapaCustomModelcs
    {

        public sp_MapaReport_FPM_Result sp_RptMAPA { get; set; }
        public List<CarumanTambahanCustomModel> CarumanTambahan { get; set; }
    }
}