namespace MVC_SYSTEM.ConfigModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_Config : DbContext
    {
        public MVC_SYSTEM_Config()
            : base(nameOrConnectionString: "MVC_SYSTEM_HQ_CONN")
        {
            //string dtsrc, incat, usid, pssd;

            //dtsrc = "172.16.23.177";
            //incat = "CheckRollCorp";
            //usid = "sa";
            //pssd = "sa@2010";

            //base.Database.Connection.ConnectionString = "data source=" + dtsrc + ";initial catalog=" + incat + ";user id=" + usid + ";password=" + pssd + ";MultipleActiveResultSets=True;App=EntityFramework";
        }

        public virtual DbSet<tblSystemConfig> tblSystemConfigs { get; set; }
    }
}
