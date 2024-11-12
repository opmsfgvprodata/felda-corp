namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_ModelsEstate : DbContext
    {
        public static string host1 = "";
        public static string catalog1 = "";
        public static string user1 = "";
        public static string pass1 = "";

        public MVC_SYSTEM_ModelsEstate()
            : base(nameOrConnectionString: "BYOWN")
        {
            base.Database.Connection.ConnectionString = "data source=" + host1 + ";initial catalog=" + catalog1 + ";user id=" + user1 + ";password=" + pass1 + ";MultipleActiveResultSets=True;App=EntityFramework";
        }

        public static MVC_SYSTEM_ModelsEstate ConnectToSqlServer(string host, string catalog, string user, string pass)
        {
            host1 = host;
            catalog1 = catalog;
            user1 = user;
            pass1 = pass;

            return new MVC_SYSTEM_ModelsEstate();
        }

        public virtual DbSet<tbl_Pkjmast> tbl_Pkjmast { get; set; }
        public virtual DbSet<tbl_CutiPeruntukan> tbl_CutiPeruntukan { get; set; }
        public virtual DbSet<vw_KumpulanKerja> vw_KumpulanKerja { get; set; }
        public virtual DbSet<vw_PermitPassportDetail> vw_PermitPassportDetail { get; set; }
        public virtual DbSet<tbl_PkjCarumanTambahan> tbl_PkjCarumanTambahan { get; set; }
        public virtual DbSet<tbl_Kepuasan> tbl_Kepuasan { get; set; }
        public virtual DbSet<tbl_Produktiviti> tbl_Produktiviti { get; set; }
        public virtual DbSet<tbl_PkjIncrmntSalary> tbl_PkjIncrmntSalary { get; set; }
        public virtual DbSet<tbl_PkjIncrmntSalaryHistory> tbl_PkjIncrmntSalaryHistory { get; set; }
        public virtual DbSet<tbl_Insentif> tbl_Insentif { get; set; }
        public virtual DbSet<tbl_TutupUrusNiaga> tbl_TutupUrusNiaga { get; set; }
        public virtual DbSet<vw_MaklumatInsentif> vw_MaklumatInsentif { get; set; }
        public virtual DbSet<vw_RptSctran> vw_RptSctran { get; set; }

        //fatin added - 25/04/2023
        public virtual DbSet<tbl_PktPinjam> tbl_PktPinjam { get; set; }
        public virtual DbSet<tbl_PktUtama> tbl_PktUtama { get; set; }
        public virtual DbSet<tbl_Blok> tbl_Blok { get; set; }
        public virtual DbSet<tbl_SubPkt> tbl_SubPkt { get; set; }
        public virtual DbSet<tbl_BuruhKontrak> tbl_BuruhKontrak { get; set; } //fatin added - 17/04/2024
        //end
        public virtual DbSet<tbl_ByrCarumanTambahan> tbl_ByrCarumanTambahan { get; set; }//Added by Shazana 15/8/2023
        public virtual DbSet<vw_PaySheetPekerja> vw_PaySheetPekerja { get; set; } //Added by Shazana 15/8/2023

        public virtual DbSet<tbl_Kerja> tbl_Kerja { get; set; }
        public virtual DbSet<tbl_Kerjahdr> tbl_Kerjahdr { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
    }
}
