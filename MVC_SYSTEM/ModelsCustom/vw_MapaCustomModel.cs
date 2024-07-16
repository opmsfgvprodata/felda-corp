using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsSP;
using MVC_SYSTEM.ModelsCustom;

namespace MVC_SYSTEM.ModelsCustom
{
    public class vw_MapaCustomModel
    {

        public sp_MapaReport_Result sp_RptMAPA { get; set; }
        public List<CarumanTambahanCustomModel> CarumanTambahan { get; set; }
    }

}

