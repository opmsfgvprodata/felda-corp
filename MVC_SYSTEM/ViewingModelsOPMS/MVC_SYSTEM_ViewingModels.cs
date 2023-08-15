namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_ViewingModels : DbContext
    {
        public static string host1 = "";
        public static string catalog1 = "";
        public static string user1 = "";
        public static string pass1 = "";

        public MVC_SYSTEM_ViewingModels()
            : base(nameOrConnectionString: "MVC_SYSTEM_HQ_CONN")
        {
            if (host1 != "" && catalog1 != "" && user1 != "" && pass1 != "")
            {
                base.Database.Connection.ConnectionString = "data source=" + host1 + ";initial catalog=" + catalog1 + ";user id=" + user1 + ";password=" + pass1 + ";MultipleActiveResultSets=True;App=EntityFramework";
            }

        }

        public static MVC_SYSTEM_ViewingModels ConnectToSqlServer(string host, string catalog, string user, string pass)
        {
            host1 = host;
            catalog1 = catalog;
            user1 = user;
            pass1 = pass;

            return new MVC_SYSTEM_ViewingModels();

        }

        public virtual DbSet<vw_RptSctran> vw_RptSctran { get; set; }
        public virtual DbSet<tbl_Ladang> tbl_Ladang { get; set; }
        public virtual DbSet<tblUser> tblUser { get; set; }
        public virtual DbSet<tbl_GajiBulanan> tbl_GajiBulanan { get; set; }
        public virtual DbSet<vw_GajiPekerja> vw_GajiPekerja { get; set; }
        public virtual DbSet<vw_MaklumatInsentif> vw_MaklumatInsentif { get; set; }
        public virtual DbSet<vw_KerjaPekerja> vw_KerjaPekerja { get; set; }
        public virtual DbSet<vw_OTPekerja> vw_OTPekerja { get; set; }
        public virtual DbSet<vw_BonusPekerja> vw_BonusPekerja { get; set; }
        public virtual DbSet<vw_CutiPekerja> vw_CutiPekerja { get; set; }
        public virtual DbSet<vw_KehadiranPekerja> vw_KehadiranPekerja { get; set; }
        public virtual DbSet<tbl_KumpulanKerja> tbl_KumpulanKerja { get; set; }
        public virtual DbSet<vw_PaySheetPekerja> vw_PaySheetPekerja { get; set; }
        //added by kamalia 24/11/21
        public virtual DbSet<tbl_ByrCarumanTambahan> tbl_ByrCarumanTambahan { get; set; }


    }
}
