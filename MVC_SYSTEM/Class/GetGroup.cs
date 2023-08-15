using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsEstate;


namespace MVC_SYSTEM.Class
{
    public class GetGroup
    {
        Connection Connection = new Connection();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();

        public string GroupName(int groupID, int? getuserid, string getusername)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, getusername);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
            string groupname = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KumpulanID == groupID).Select(s => s.fld_Keterangan).FirstOrDefault();
            return groupname;
        }

        public string GroupCode(int? groupID, int? getuserid, string getusername, int wlyh, int ldg)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, getusername);
            Connection.GetConnection(out host, out catalog, out user, out pass, wlyh, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
            string groupcode = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg && x.fld_deleted == false && x.fld_KumpulanID == groupID).Select(s => s.fld_KodKumpulan).FirstOrDefault();
            return groupcode;
        }
    }
}