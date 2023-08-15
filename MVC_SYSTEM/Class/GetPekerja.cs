using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ConfigModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsEstate;

namespace MVC_SYSTEM.Class
{
    public class GetPekerja
    {
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();

        public List<string> NoPekerja(int mont, int year, int wlyhid, int ldgid)
        {
            var nopkj = new List<string>();
            nopkj = db.vw_KerjaHarian.Where(x => x.fld_WilayahID == wlyhid && x.fld_LadangID == ldgid && x.fld_Tarikh.Value.Month == mont && x.fld_Tarikh.Value.Year == year).Select(s => s.fld_Nopkj.Trim()).Distinct().ToList();
            return nopkj;
        }

        public bool getExistingWorkerApp(string NoPjk, string NoPassIC, long? fileid, int? ldgid, int? wlyhid, int? syrktid, int? ngraid)
        {
            bool result = false;

            var getworker = db.tblPkjmastApps.Where(x => x.fldNoPkj == NoPjk && x.fldNoKP == NoPassIC && x.fldLadangID == ldgid && x.fldWilayahID == wlyhid && x.fldSyarikatID == syrktid && x.fldNegaraID == ngraid && x.fldStatus == 1).Count();

            if (getworker >= 1)
            {
                result = true;
            }

            return result;
        }

        public string getWorkerName(string NoPkj, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            Connection Connection = new Connection();
            string host, catalog, user, pass = "";
            string WorkerName = "";

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            WorkerName = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nama).FirstOrDefault();

            return WorkerName;
        }
    }
}