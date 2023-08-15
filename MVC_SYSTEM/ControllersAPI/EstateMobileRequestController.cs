using MVC_SYSTEM.ModelsAPI;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace MVC_SYSTEM.Controllers
{
    public class EstateMobileRequestController : ApiController
    {
        private EncryptDecrypt EncryptDecrypt = new EncryptDecrypt();
        MVC_SYSTEM_Models_API db = new MVC_SYSTEM_Models_API();
        [HttpGet]
        public IEnumerable<tblUser> Get(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            List<tblUser> users = new List<tblUser>();

            var datatblUsers = db.tblUsers.Where(x => x.fld_KmplnSyrktID == fld_KmplnSyrktID && x.fldNegaraID == fldNegaraID && x.fldSyarikatID == fldSyarikatID && x.fldWilayahID == fldWilayahID && x.fldLadangID == fldLadangID && x.fldDeleted == false).ToList();

            foreach(var datatblUser in datatblUsers)
            {
                datatblUser.fldUserPassword = EncryptDecrypt.Decrypt(datatblUser.fldUserPassword);
                users.Add(datatblUser);
            }

            return users;
        }

        //public string Get(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        //{
        //    var JSONString = new StringBuilder();
        //    int count = 1;
        //    var alltbluserdata = db.tblUsers.Where(x => x.fld_KmplnSyrktID == fld_KmplnSyrktID && x.fldNegaraID == fldNegaraID && x.fldSyarikatID == fldSyarikatID && x.fldWilayahID == fldWilayahID && x.fldLadangID == fldLadangID).ToList();
        //    JSONString.Append("[");
        //    JSONString.Append("{");
        //    JSONString.Append("tblUser:");
        //    JSONString.Append("[");
        //    foreach (var tbluserdata in alltbluserdata)
        //    {
        //        JSONString.Append("{");
        //        JSONString.Append("fldUserName:" + "" + tbluserdata.fldUserName + ",");
        //        JSONString.Append("fldUserFullName:" + "" + tbluserdata.fldUserFullName + ",");
        //        JSONString.Append("fldUserPassword:" + "" + EncryptDecrypt.Decrypt(tbluserdata.fldUserPassword));

        //        if (count == alltbluserdata.Count())
        //        {
        //            JSONString.Append("}");
        //        }
        //        else
        //        {
        //            JSONString.Append("},");
        //        }
        //        count++;
        //    }
        //    JSONString.Append("]");
        //    JSONString.Append("}");
        //    JSONString.Append("]");

        //    return JSONString.ToString();
        //}
    }
}
