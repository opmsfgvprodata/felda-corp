using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Class
{
    public class GlobalFunction
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();

        public static class PropertyCopy
        {
            public static void Copy<TDest, TSource>(TDest destination, TSource source)
                where TSource : class
                where TDest : class
            {
                var destProperties = destination.GetType()
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite && !x.GetGetMethod().IsVirtual);
                var sourceProperties = source.GetType()
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite && !x.GetGetMethod().IsVirtual);
                var copyProperties = sourceProperties.Join(destProperties, x => x.Name, y => y.Name, (x, y) => x);
                foreach (var sourceProperty in copyProperties)
                {
                    var prop = destProperties.FirstOrDefault(x => x.Name == sourceProperty.Name);
                    prop.SetValue(destination, sourceProperty.GetValue(source));
                }
            }
        }

        //Added by Shazana 1/8/2023
        public string getJawatanName(int? userid, int? NegaraID, int? SyarikatID)
        {

            var usernameInfo = db.tblUsers.Where(x => x.fldUserID == userid).FirstOrDefault();
            string jawatancode = db.tblUserIDApps.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldUserid == usernameInfo.fldUserName).Select(s => s.fldJawatan).FirstOrDefault();

            string jawatanname = "";
            if (jawatancode == null)
            {
                jawatanname = db.tblRoles.Where(x => x.fldRoleID == usernameInfo.fldRoleID && x.fldDeleted == false).Select(x => x.fldDescriptionRole).FirstOrDefault();
            }
            else
            { jawatanname = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == jawatancode && x.fldOptConfFlag1 == "position").Select(s => s.fldOptConfDesc).FirstOrDefault(); }

            return jawatanname.ToUpper();
        }

    }
}