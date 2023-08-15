using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.LoginModels
{
    public class MVC_SYSTEM_Login : DbContext
    {
        public MVC_SYSTEM_Login()
            : base(nameOrConnectionString: "MVC_SYSTEM_HQ_CONN")
        {
            //string dtsrc, incat, usid, pssd;

            //dtsrc = "172.16.23.177";
            //incat = "CheckRollCorp";
            //usid = "sa";
            //pssd = "sa@2010";

            //base.Database.Connection.ConnectionString = "data source=" + dtsrc + ";initial catalog=" + incat + ";user id=" + usid + ";password=" + pssd + ";MultipleActiveResultSets=True;App=EntityFramework";
        }

        public virtual DbSet<tblUser> tblUsers { get; set; }

    }
}