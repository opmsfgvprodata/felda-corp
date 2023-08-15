using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsCustom;

namespace MVC_SYSTEM.ModelsCustom
{
    public class CustMod_DesignationIncentiveEligibility
    {
        public tbl_KelayakanInsentifLdg KelayakanInsentif { get; set; }
        public List<tbl_KelayakanInsentifPkjLdg> KelayakanInsentifPkjLdg { get; set; }
    }
}