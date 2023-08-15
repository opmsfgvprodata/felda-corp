using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsEstate;

namespace MVC_SYSTEM.ModelsCustom
{
    public class CustMod_WorkerIncentiveList
    {
        public tbl_Pkjmast Pkjmast { get; set; }
        public List<tbl_Insentif> Insentif { get; set; }
    }
}