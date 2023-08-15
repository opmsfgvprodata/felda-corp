using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetActivityDetail
    {
        MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();

        public string getKeteranganAktiviti(string lejar, string kodaktiviti)
        {
            string returnresult = "";

            returnresult = db.tblAktivitis.Where(x => x.fldLejar == lejar && x.fldKodAktvti == kodaktiviti && x.fldDeleted == false).Select(s => s.fldKtrangan).FirstOrDefault();

            return returnresult;
        }
    }
}