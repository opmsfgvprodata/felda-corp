using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetSyarikat
    {
        private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        private MVC_SYSTEM_Models db2 = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_ModelsCorporate dbCorp = new MVC_SYSTEM_ModelsCorporate();
        public int[] GetSyarikatID(int? NegaraID)
        {
            IEnumerable<int> enumerable = db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID).OrderBy(o => o.fld_SyarikatID).Select(s => s.fld_SyarikatID).ToArray();
            int[] syrktid = enumerable.ToArray();

            return syrktid;
        }
    }
}