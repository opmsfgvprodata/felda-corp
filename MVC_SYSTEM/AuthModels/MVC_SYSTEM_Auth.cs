namespace MVC_SYSTEM.AuthModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using MVC_SYSTEM.ModelsCorporate;

    public partial class MVC_SYSTEM_Auth : DbContext
    {
        public MVC_SYSTEM_Auth()
            : base(nameOrConnectionString: "MVC_SYSTEM_HQ_CONN")
        {
            //string dtsrc, incat, usid, pssd;

            //dtsrc = "172.16.23.177";
            //incat = "CheckRollCorp";
            //usid = "sa";
            //pssd = "sa@2010";

            //base.Database.Connection.ConnectionString = "data source=" + dtsrc + ";initial catalog=" + incat + ";user id=" + usid + ";password=" + pssd + ";MultipleActiveResultSets=True;App=EntityFramework";
        }

        public virtual DbSet<tblRole> tblRoles { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblClient> tblClients { get; set; }
        public virtual DbSet<tbl_Ladang> tbl_Ladang { get; set; }
        public virtual DbSet<tbl_Negara> tbl_Negara { get; set; }
        public virtual DbSet<tbl_Syarikat> tbl_Syarikat { get; set; }
        public virtual DbSet<tbl_Wilayah> tbl_Wilayah { get; set; }
        public virtual DbSet<tbl_KumpulanSyarikat> tbl_KumpulanSyarikat { get; set; }
        public virtual DbSet<tbl_SuperAdminSelection> tbl_SuperAdminSelection { get; set; }
        public virtual DbSet<tblUserIDApp> tblUserIDApps { get; set; }
        public virtual DbSet<tblUserAuditTrail> tblUserAuditTrail { get; set; }
        public virtual DbSet<tbl_EstateSelection> tbl_EstateSelection { get; set; }
        public virtual DbSet<vw_NSWL> vw_NSWL { get; set; }
        public virtual DbSet<tblOptionConfigsWeb> tblOptionConfigsWeb { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldUserid)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldNama)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldNoKP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldKdLdg)
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldNamaLdg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldJawatan)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldPassword)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldStatus)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}
