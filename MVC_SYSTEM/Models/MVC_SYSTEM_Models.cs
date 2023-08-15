namespace MVC_SYSTEM.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Configuration;
    using AuthModels;

    public partial class MVC_SYSTEM_Models : DbContext
    {
        public MVC_SYSTEM_Models()
            : base(nameOrConnectionString: "MVC_SYSTEM_HQ_CONN")
        {
        }
        //public static string host1 = "";
        //public static string catalog1 = "";
        //public static string user1 = "";
        //public static string pass1 = "";
        //public MVC_SYSTEM_Models()
        //    : base(nameOrConnectionString: "BYOWN")
        //{
        //    base.Database.Connection.ConnectionString = "data source=" + host1 + ";initial catalog=" + catalog1 + ";user id=" + user1 + ";password=" + pass1 + ";MultipleActiveResultSets=True;App=EntityFramework";
        //}

        //public static MVC_SYSTEM_Models ConnectToSqlServer(string host, string catalog, string user, string pass)
        //{
        //    //SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
        //    //sqlBuilder.DataSource = host;
        //    //sqlBuilder.InitialCatalog = catalog;
        //    //sqlBuilder.MultipleActiveResultSets = true;
        //    //sqlBuilder.UserID = user;
        //    //sqlBuilder.Password = pass;
        //    //sqlBuilder.ConnectTimeout = 100;
        //    //sqlBuilder.PersistSecurityInfo = true;
        //    //sqlBuilder.IntegratedSecurity = true;

        //    //var entityConnectionStringBuilder = new EntityConnectionStringBuilder();
        //    //entityConnectionStringBuilder.Provider = "System.Data.SqlClient";
        //    //entityConnectionStringBuilder.ProviderConnectionString = sqlBuilder.ConnectionString;
        //    //entityConnectionStringBuilder.Metadata = "res://*/";
        //    host1 = host;
        //    catalog1 = catalog;
        //    user1 = user;
        //    pass1 = pass;

        //    return new MVC_SYSTEM_Models();

        //}

        public virtual DbSet<tbl_ASCRawData> tbl_ASCRawData { get; set; }
        public virtual DbSet<tbl_ASCRawDataDetail> tbl_ASCRawDataDetail { get; set; }
        public virtual DbSet<tbl_Ladang> tbl_Ladang { get; set; }
        public virtual DbSet<tbl_ListASCFile> tbl_ListASCFile { get; set; }
        public virtual DbSet<tbl_OptionConfig> tbl_OptionConfig { get; set; }
        public virtual DbSet<tbl_SevicesProcess> tbl_SevicesProcess { get; set; }
        public virtual DbSet<tbl_SevicesProcessHistory> tbl_SevicesProcessHistory { get; set; }
        public virtual DbSet<tbl_Wilayah> tbl_Wilayah { get; set; }
        public virtual DbSet<vw_PermitPassportDetail> vw_PermitPassportDetail { get; set; }
        public virtual DbSet<tbl_WilCli> tbl_WilCli { get; set; }
        public virtual DbSet<vw_AuditTrail> vw_AuditTrail { get; set; }
        public virtual DbSet<vw_ClientWilayah> vw_ClientWilayah { get; set; }
        public virtual DbSet<tbl_ServicesList> tbl_ServicesList { get; set; }
        public virtual DbSet<tblReportList> tblReportLists { get; set; }
        public virtual DbSet<tblRoleReport> tblRoleReports { get; set; }
        public virtual DbSet<tblReportExport> tblReportExports { get; set; }
        public virtual DbSet<tblHtmlReport> tblHtmlReports { get; set; }
        public virtual DbSet<vw_LBPPL> vw_LBPPL { get; set; }
        public virtual DbSet<tblASCApprovalRawData> tblASCApprovalRawDatas { get; set; }
        public virtual DbSet<tblASCApprovalFileDetail> tblASCApprovalFileDetails { get; set; }
        public virtual DbSet<tblPkjmastApp> tblPkjmastApps { get; set; }
        public virtual DbSet<tblSubReportList> tblSubReportLists { get; set; }
        public virtual DbSet<tblAktiviti> tblAktivitis { get; set; }
        public virtual DbSet<tblAktiviti2> tblAktiviti2 { get; set; }
        public virtual DbSet<vw_DetailPekerja> vw_DetailPekerja { get; set; }
        public virtual DbSet<vw_NSWL> vw_NSWL { get; set; }
        public virtual DbSet<vw_skb> vw_skb { get; set; }
        public virtual DbSet<tblEmailNotiStatu> tblEmailNotiStatus { get; set; }
        public virtual DbSet<tblTaskRemainder> tblTaskRemainders { get; set; }
        public virtual DbSet<tbl_Sctran> tbl_Sctran { get; set; }
        public virtual DbSet<tblOptionGeneralConfigsWeb> tblOptionGeneralConfigsWebs { get; set; }
        public virtual DbSet<vw_GajiBulanan> vw_GajiBulanan { get; set; }
        public virtual DbSet<vw_KerjaHarian> vw_KerjaHarian { get; set; }
        public virtual DbSet<tbl_Pkjmast> tbl_Pkjmast { get; set; }
        public virtual DbSet<tblEmailList> tblEmailLists { get; set; }
        public virtual DbSet<tbl_SokPermhnWang> tbl_SokPermhnWang { get; set; }
        public virtual DbSet<vw_PermohonanWang> vw_PermohonanWang { get; set; }
        public virtual DbSet<tblSokPermhnWangHisAction> tblSokPermhnWangHisActions { get; set; }
        public virtual DbSet<vw_ServicesProcess> vw_ServicesProcess { get; set; }
        public virtual DbSet<vw_NeragaSumberDetail> vw_NeragaSumberDetail { get; set; }
        public virtual DbSet<tblTKABatch> tblTKABatches { get; set; }
        public virtual DbSet<tblTKADetail> tblTKADetails { get; set; }
        public virtual DbSet<vw_TKADetail> vw_TKADetail { get; set; }
        public virtual DbSet<tbl_PerluLadang> tbl_PerluLadang { get; set; }
        public virtual DbSet<tbl_PerluLadangHistory> tbl_PerluLadangHistory { get; set; }
        public virtual DbSet<tbl_QuotaPerluLadang> tbl_QuotaPerluLadang { get; set; }
        public virtual DbSet<tbl_QuotaPerluLadangHistory> tbl_QuotaPerluLadangHistory { get; set; }
        public virtual DbSet<tblDataEntryList> tblDataEntryLists { get; set; }
        public virtual DbSet<tbl_Pembekal> tbl_Pembekal { get; set; }
        public virtual DbSet<tblMaintenanceList> tblMaintenanceLists { get; set; }
        public virtual DbSet<tbl_Syarikat> tbl_Syarikat { get; set; }
        public virtual DbSet<vw_ApplicationInfo> vw_ApplicationInfo { get; set; }
        public virtual DbSet<tbl_Version> tbl_Version { get; set; }
        public virtual DbSet<tbl_CutiUmum> tbl_CutiUmum { get; set; }
        public virtual DbSet<tblOptionConfigsWeb> tblOptionConfigsWeb { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<vw_PermitPassportDetail>()
               .Property(e => e.fld_Nopkj)
               .IsFixedLength()
               .IsUnicode(false);

            modelBuilder.Entity<vw_PermitPassportDetail>()
                .Property(e => e.fld_Nama1)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_PermitPassportDetail>()
                .Property(e => e.fld_Prmtno)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_PermitPassportDetail>()
                .Property(e => e.fld_Nokp)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_LBPPL>()
                .Property(e => e.fld_Nopkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_LBPPL>()
                .Property(e => e.fld_Nama1)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_LBPPL>()
                .Property(e => e.fld_Jumlah)
                .HasPrecision(8, 2);

            modelBuilder.Entity<vw_LBPPL>()
                .Property(e => e.fld_Kdrkyt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldNoPkj)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldNama1)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldNoKP)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldKdJnsPkj)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldKdRkyt)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldKdLdg)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldJumPjm)
                .HasPrecision(7, 2);

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldSbbMsk)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldAlsnMsk)
                .IsFixedLength();

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

            modelBuilder.Entity<vw_DetailPekerja>()
                .Property(e => e.fld_Nopkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_DetailPekerja>()
                .Property(e => e.fld_Nama1)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_DetailPekerja>()
                .Property(e => e.fld_Nokp)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_DetailPekerja>()
                .Property(e => e.fld_Kdaktf)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_DetailPekerja>()
                .Property(e => e.fld_Kdrkyt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_DetailPekerja>()
                .Property(e => e.fld_Kdjnt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_DetailPekerja>()
                .Property(e => e.fld_Jenispekerja)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_DetailPekerja>()
                .Property(e => e.fld_Ktgpkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_skb>()
                .Property(e => e.fld_Amt)
                .HasPrecision(13, 2);

            modelBuilder.Entity<vw_skb>()
                .Property(e => e.fld_Bulan)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_skb>()
                .Property(e => e.fld_Lejar)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblTaskRemainder>()
                .Property(e => e.fldPurpose)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_KdCaj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_Bulan)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_KdTran)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_Lejar)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_KdLdg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_Pkt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_Akt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_Amt)
                .HasPrecision(13, 2);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_NoDkmn)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_NoCtr)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_NoKP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_Nop23i)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_NoAset)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_KdStok)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_Qty)
                .HasPrecision(10, 2);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_NoCek)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Sctran>()
                .Property(e => e.fld_NoSkb)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Nopkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Kum)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Hujan)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Kdhdct)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Glcd)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Pkt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Aktvt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_No_Kontrak)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Kdhbyr)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Kgk)
                .HasPrecision(6, 2);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Kti1)
                .HasPrecision(6, 2);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Kti3)
                .HasPrecision(6, 2);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Kong)
                .HasPrecision(6, 2);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Kdrot)
                .HasPrecision(6, 2);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Jamot)
                .HasPrecision(4, 2);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Qty)
                .HasPrecision(6, 2);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Amt)
                .HasPrecision(6, 2);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Jumlah)
                .HasPrecision(8, 2);

            modelBuilder.Entity<vw_KerjaHarian>()
                .Property(e => e.fld_Nama1)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiBulanan>()
                .Property(e => e.fld_Nopkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiBulanan>()
                .Property(e => e.fld_Nama1)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiBulanan>()
                .Property(e => e.fld_Nokp)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiBulanan>()
                .Property(e => e.fld_Gaji_Kasar)
                .HasPrecision(8, 2);

            modelBuilder.Entity<vw_GajiBulanan>()
                .Property(e => e.fld_Epf_Mjk)
                .HasPrecision(8, 2);

            modelBuilder.Entity<vw_GajiBulanan>()
                .Property(e => e.fld_Socso_Mjk)
                .HasPrecision(6, 2);

            modelBuilder.Entity<vw_GajiBulanan>()
                .Property(e => e.fld_Epf_Pkj)
                .HasPrecision(8, 2);

            modelBuilder.Entity<vw_GajiBulanan>()
                .Property(e => e.fld_Socso_Pkj)
                .HasPrecision(6, 2);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Nopkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Nokp)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Nama)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Almt1)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Daerah)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Neg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Negara)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Poskod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Notel)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Nofax)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Kdjnt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Kdbgsa)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Kdagma)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Kdrkyt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Kdkwn)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Kpenrka)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Kdaktf)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Sbtakf)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Ktgpkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Jenispekerja)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Kodbkl)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_KodSocso)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Noperkeso)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_KodKWSP)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Nokwsp)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Visano)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Nogilr)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Prmtno)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Psptno)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_Kdldg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Pkjmast>()
                .Property(e => e.fld_IDpkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_SokPermhnWang>()
                .Property(e => e.fld_JumlahPermohonan)
                .HasPrecision(13, 2);

            modelBuilder.Entity<tbl_SokPermhnWang>()
                .Property(e => e.fld_JumlahPDP)
                .HasPrecision(13, 2);

            modelBuilder.Entity<tbl_SokPermhnWang>()
                .Property(e => e.fld_JumlahTT)
                .HasPrecision(13, 2);

            modelBuilder.Entity<tbl_SokPermhnWang>()
                .Property(e => e.fld_JumlahCIT)
                .HasPrecision(13, 2);

            modelBuilder.Entity<tbl_SokPermhnWang>()
                .Property(e => e.fld_JumlahManual)
                .HasPrecision(13, 2);

            modelBuilder.Entity<vw_PermohonanWang>()
                .Property(e => e.fld_NoSkb)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_ApplicationInfo>()
                .Property(e => e.tblTaskRemainderfldPurpose)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Version>()
               .Property(e => e.fld_prog_date)
               .IsUnicode(false);

            modelBuilder.Entity<tbl_Version>()
                .Property(e => e.fld_progr_ver)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Version>()
                .Property(e => e.fld_Nmpgrs)
                .IsUnicode(false);
            
        }

        public System.Data.Entity.DbSet<MVC_SYSTEM.AuthModels.tblUser> tblUsers { get; set; }
    }
}
