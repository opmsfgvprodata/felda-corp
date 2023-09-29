namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_ModelsCorporate : DbContext
    {
        public MVC_SYSTEM_ModelsCorporate()
            : base(nameOrConnectionString: "MVC_SYSTEM_HQ_CONN")
        {
        }

        public virtual DbSet<tbl_Ladang> tbl_Ladang { get; set; }
        public virtual DbSet<tblPkjmastApp> tblPkjmastApp { get; set; }
        public virtual DbSet<vw_AuditTrail> vw_AuditTrail { get; set; }
        public virtual DbSet<vw_UserIDDetail> vw_UserIDDetail { get; set; }
        public virtual DbSet<tbl_ASCRawData> tbl_ASCRawData { get; set; }
        public virtual DbSet<tbl_ASCRawDataDetail> tbl_ASCRawDataDetail { get; set; }
        public virtual DbSet<tbl_AuditTrail> tbl_AuditTrail { get; set; }
        public virtual DbSet<tbl_Bank> tbl_Bank { get; set; }
        public virtual DbSet<tbl_EstateSelection> tbl_EstateSelection { get; set; }
        public virtual DbSet<tbl_KumpulanSyarikat> tbl_KumpulanSyarikat { get; set; }
        public virtual DbSet<tbl_ListASCFile> tbl_ListASCFile { get; set; }
        public virtual DbSet<tbl_LogDetail> tbl_LogDetail { get; set; }
        public virtual DbSet<tbl_Negara> tbl_Negara { get; set; }
        public virtual DbSet<tbl_OptionConfig> tbl_OptionConfig { get; set; }
        public virtual DbSet<tbl_Pembekal> tbl_Pembekal { get; set; }
        public virtual DbSet<tbl_PerluLadang> tbl_PerluLadang { get; set; }
        public virtual DbSet<tbl_PerluLadangHistory> tbl_PerluLadangHistory { get; set; }
       

        public virtual DbSet<tbl_Poskod> tbl_Poskod { get; set; }
        public virtual DbSet<tbl_QuotaPerluLadang> tbl_QuotaPerluLadang { get; set; }
        public virtual DbSet<tbl_QuotaPerluLadangHistory> tbl_QuotaPerluLadangHistory { get; set; }
        public virtual DbSet<tbl_ServicesList> tbl_ServicesList { get; set; }
        public virtual DbSet<tbl_SevicesProcess> tbl_SevicesProcess { get; set; }
        public virtual DbSet<tbl_SevicesProcessHistory> tbl_SevicesProcessHistory { get; set; }
        public virtual DbSet<tbl_SokPermhnWang> tbl_SokPermhnWang { get; set; }
        public virtual DbSet<tbl_SuperAdminSelection> tbl_SuperAdminSelection { get; set; }
        public virtual DbSet<tbl_Syarikat> tbl_Syarikat { get; set; }
        public virtual DbSet<tbl_UploadedCountDetail> tbl_UploadedCountDetail { get; set; }
        public virtual DbSet<tbl_Wilayah> tbl_Wilayah { get; set; }
        public virtual DbSet<tblAktiviti> tblAktivitis { get; set; }
        public virtual DbSet<tblASCApprovalFileDetail> tblASCApprovalFileDetails { get; set; }
        public virtual DbSet<tblASCApprovalRawData> tblASCApprovalRawDatas { get; set; }
        public virtual DbSet<tblClient> tblClients { get; set; }
        public virtual DbSet<tblConnection> tblConnections { get; set; }
        public virtual DbSet<tblDataEntryList> tblDataEntryLists { get; set; }
        public virtual DbSet<tblEmailList> tblEmailLists { get; set; }
        public virtual DbSet<tblEmailNotiStatu> tblEmailNotiStatus { get; set; }
        public virtual DbSet<tblHtmlReport> tblHtmlReport { get; set; }
        public virtual DbSet<tblMaintenanceList> tblMaintenanceLists { get; set; }
        public virtual DbSet<tblNgrSmbrSyrkt> tblNgrSmbrSyrkts { get; set; }
        public virtual DbSet<tblOptionConfigsWeb> tblOptionConfigsWebs { get; set; }
        public virtual DbSet<tblReportExport> tblReportExports { get; set; }
        public virtual DbSet<tblReportList> tblReportLists { get; set; }
        public virtual DbSet<tblRoleReport> tblRoleReports { get; set; }
        public virtual DbSet<tblRole> tblRoles { get; set; }
        public virtual DbSet<tblSokPermhnWangHisAction> tblSokPermhnWangHisActions { get; set; }
        public virtual DbSet<tblStatusPkj> tblStatusPkjs { get; set; }
        public virtual DbSet<tblSubReportList> tblSubReportLists { get; set; }
        public virtual DbSet<tblSystemConfig> tblSystemConfigs { get; set; }
        public virtual DbSet<tblTaskRemainder> tblTaskRemainders { get; set; }
        public virtual DbSet<tblTKABatch> tblTKABatches { get; set; }
        public virtual DbSet<tblTKADetail> tblTKADetails { get; set; }
        public virtual DbSet<tblUserIDApp> tblUserIDApps { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tbl_Upah> tbl_Upah { get; set; }
        public virtual DbSet<tbl_UpahMenuai> tbl_UpahMenuai { get; set; }

        //fatin added - 3/5/2023
        public virtual DbSet<tbl_UpahGetah> tbl_UpahGetah { get; set; }
        public virtual DbSet<tbl_HargaGetahSMR> tbl_HargaGetahSMR { get; set; }
        public virtual DbSet<tbl_ProduktivitiGetah> tbl_ProduktivitiGetah { get; set; }
        public virtual DbSet<tbl_InsentifKGK> tbl_InsentifKGK { get; set; }
        //end

        public virtual DbSet<tbl_CutiKategori> tbl_CutiKategori { get; set; }
        public virtual DbSet<tbl_CutiUmum> tbl_CutiUmum { get; set; }
        public virtual DbSet<tbl_CutiMaintenance> tbl_CutiMaintenance { get; set; }
        public virtual DbSet<tbl_JenisCaruman> tbl_JenisCaruman { get; set; }
        public virtual DbSet<tbl_Kwsp> tbl_Kwsp { get; set; }
        public virtual DbSet<tbl_Socso> tbl_Socso { get; set; }
        public virtual DbSet<vw_Socso> vw_Socso { get; set; }
        public virtual DbSet<tblMenuList> tblMenuLists { get; set; }
        public virtual DbSet<tbl_JenisInsentif> tbl_JenisInsentif { get; set; }
        public virtual DbSet<tbl_UpahAktiviti> tbl_UpahAktiviti { get; set; }
        public virtual DbSet<tbl_MingguNegeri> tbl_MingguNegeri { get; set; }
        public virtual DbSet<tbl_HariBekerja> tbl_HariBekerja { get; set; }
        public virtual DbSet<tbl_JenisAktiviti> tbl_JenisAktiviti { get; set; }
        public virtual DbSet<tbl_HargaSawitRange> tbl_HargaSawitRange { get; set; }
        public virtual DbSet<tbl_HargaSawitSemasa> tbl_HargaSawitSemasa { get; set; }
        public virtual DbSet<vw_ServicesProcess> vw_ServicesProcess { get; set; }
        public virtual DbSet<tbl_Lejar> tbl_Lejar { get; set; }
        public virtual DbSet<tbl_CarumanTambahan> tbl_CarumanTambahan { get; set; }
        public virtual DbSet<tbl_SubCarumanTambahan> tbl_SubCarumanTambahan { get; set; }
        public virtual DbSet<tbl_JadualCarumanTambahan> tbl_JadualCarumanTambahan { get; set; }
        public virtual DbSet<vw_MingguNegeri> vw_MingguNegeri { get; set; }
        public virtual DbSet<vw_HariBekerja> vw_HariBekerja { get; set; }
        public virtual DbSet<vw_NSWL> vw_NSWL { get; set; }
        public virtual DbSet<tbl_MapGL> tbl_MapGL { get; set; }
        public virtual DbSet<tbl_BlckKmskknDataKerja> tbl_BlckKmskknDataKerja { get; set; }
        
        public virtual DbSet<tbl_Sctran> tbl_Sctran { get; set; }
        public virtual DbSet<vw_DetailPekerja> vw_DetailPekerja { get; set; }
        public virtual DbSet<tbl_Pkjmast> tbl_Pkjmast { get; set; }
        public virtual DbSet<vw_GajiBulanan> vw_GajiBulanan { get; set; }
        public virtual DbSet<vw_NeragaSumberDetail> vw_NeragaSumberDetail { get; set; }
        public virtual DbSet<vw_TKADetail> vw_TKADetail { get; set; }
        public virtual DbSet<tbl_Version> tbl_Version { get; set; }
        public virtual DbSet<vw_PermohonanWang> vw_PermohonanWang { get; set; }
        public virtual DbSet<vw_PermitPassportDetail> vw_PermitPassportDetail { get; set; }
        public virtual DbSet<tbl_KategoriAktiviti> tbl_KategoriAktiviti { get; set; }
        public virtual DbSet<tbl_BlckKmskknDataKerjaHistory> tbl_BlckKmskknDataKerjaHistory { get; set; }
        public virtual DbSet<vw_PermohonanKewangan> vw_PermohonanKewangan { get; set; }
        public virtual DbSet<tbl_CostCentre> tbl_CostCentre { get; set; }
        
        public virtual DbSet<tblUserAuditTrail> tblUserAuditTrails { get; set; }
        public virtual DbSet<vw_HargaSemasa> vw_HargaSemasa { get; set; }
        public virtual DbSet<tbl_CutiUmumMaster> tbl_CutiUmumMaster { get; set; }
        public virtual DbSet<tbl_CutiUmumLdg> tbl_CutiUmumLdg { get; set; }
        public virtual DbSet<vw_CutiUmumLdg> vw_CutiUmumLdg { get; set; }
        public virtual DbSet<vw_CutiUmumLdgDetails> vw_CutiUmumLdgDetails { get; set; }
        public virtual DbSet<tbl_CutiUmumKelayakan> tbl_CutiUmumKelayakan { get; set; }
        public virtual DbSet<tbl_KelayakanInsentifLdg> tbl_KelayakanInsentifLdg { get; set; }
        public virtual DbSet<tbl_KelayakanInsentifPkjLdg> tbl_KelayakanInsentifPkjLdg { get; set; }
        public virtual DbSet<vw_KelayakanInsentifLdg> vw_KelayakanInsentifLdg { get; set; }
        public virtual DbSet<vw_JenisMingguLadang> vw_JenisMingguLadang { get; set; }
        public virtual DbSet<tbl_JenisMingguLadang> tbl_JenisMingguLadang { get; set; }
        public virtual DbSet<tbl_HariBekerjaLadang> tbl_HariBekerjaLadang { get; set; }
        public virtual DbSet<vw_HariBekerjaLadang> vw_HariBekerjaLadang { get; set; }
        public virtual DbSet<vw_CutiUmumKelayakan> vw_CutiUmumKelayakan { get; set; }
        public virtual DbSet<vw_CutiUmumKelayakanDetails> vw_CutiUmumKelayakanDetails { get; set; }
      
        public virtual DbSet<tbl_BatchRunNo> tbl_BatchRunNo { get; set; }
        public virtual DbSet<tbl_PkjIncrmntApp> tbl_PkjIncrmntApp { get; set; }
        public virtual DbSet<tbl_Messaging> tbl_Messaging { get; set; }
        //Kamalia - 18/02/2021
        public virtual DbSet<tbl_GajiMinimaLdg> tbl_GajiMinimaLdg { get; set; }
        //fatin added - 31/03/2023
        public virtual DbSet<tbl_CustomerVendorGLMap> tbl_CustomerVendorGLMap { get; set; }
        //end

        //farahin punya kerjaa..hehehe..
        public virtual DbSet<tbl_SAPLog> tbl_SAPLog { get; set; }
        public virtual DbSet<tbl_IOSAP> tbl_IOSAP { get; set; }
        public virtual DbSet<tbl_CCSAP> tbl_CCSAP { get; set; }
        public virtual DbSet<tbl_GLSAP> tbl_GLSAP{ get; set; }
        public virtual DbSet<tbl_VDSAP> tbl_VDSAP { get; set; }
        public virtual DbSet<tbl_CMSAP> tbl_CMSAP { get; set; }
        public virtual DbSet<tbl_Customer> tbl_Customer { get; set; }
        public virtual DbSet<vw_SAPIODetails> vw_SAPIODetails { get; set; }
        public virtual DbSet<tbl_WBSSAP> tbl_WBSSAP { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbl_SubCarumanTambahan>()
                .Property(e => e.fld_KadarMajikan)
                .HasPrecision(18, 4);

            modelBuilder.Entity<tbl_SubCarumanTambahan>()
                .Property(e => e.fld_KadarPekerja)
                .HasPrecision(18, 4);

            //added by kamalia 7/10/2021
            modelBuilder.Entity<tbl_UpahAktiviti>()
              .Property(e => e.fld_Harga)
              .HasPrecision(18, 3);

            modelBuilder.Entity<tbl_UpahAktiviti>()
                .Property(e => e.fld_MaxProduktiviti)
                .HasPrecision(18, 3);
            //
            //modelBuilder.Entity<tblTaskRemainder>()
            //    .Property(e => e.fldPurpose)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_UserIDDetail>()
            //    .Property(e => e.fldNama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_UserIDDetail>()
            //    .Property(e => e.fldNoKP)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_UserIDDetail>()
            //    .Property(e => e.fldKdLdg)
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_UserIDDetail>()
            //    .Property(e => e.fldNamaLdg)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_UserIDDetail>()
            //    .Property(e => e.fldJawatan)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_UserIDDetail>()
            //    .Property(e => e.fldPassword)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_UserIDDetail>()
            //    .Property(e => e.fldStatus)
            //    .IsFixedLength()
            //    .IsUnicode(false);


            //modelBuilder.Entity<tbl_JenisCaruman>()
            //    .Property(e => e.fld_PeratusCarumanPekerja)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<tbl_JenisCaruman>()
            //    .Property(e => e.fld_PeratusCarumanMajikanBawah5000)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<tbl_JenisCaruman>()
            //    .Property(e => e.fld_PeratusCarumanMajikanAtas5000)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<tbl_MapGL>()
            //    .Property(e => e.fld_KodAktvt)
            //    .IsFixedLength();

            //modelBuilder.Entity<tbl_MapGL>()
            //    .Property(e => e.fld_KodGL)
            //    .IsFixedLength();

            //modelBuilder.Entity<tbl_MapGL>()
            //    .Property(e => e.fld_Paysheet)
            //    .IsFixedLength();

            //modelBuilder.Entity<tbl_Lejar>()
            //    .Property(e => e.fld_KodCaj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Poskod>()
            //    .Property(e => e.fld_Postcode)
            //    .IsFixedLength();

            //modelBuilder.Entity<tbl_Poskod>()
            //    .Property(e => e.fld_DistrictArea)
            //    .IsFixedLength();

            //modelBuilder.Entity<tbl_Poskod>()
            //    .Property(e => e.fld_State)
            //    .IsFixedLength();

            //modelBuilder.Entity<tbl_Poskod>()
            //    .Property(e => e.fld_Region)
            //    .IsFixedLength();

            //modelBuilder.Entity<tbl_SokPermhnWang>()
            //    .Property(e => e.fld_JumlahPermohonan)
            //    .HasPrecision(13, 2);

            //modelBuilder.Entity<tbl_SokPermhnWang>()
            //    .Property(e => e.fld_JumlahPDP)
            //    .HasPrecision(13, 2);

            //modelBuilder.Entity<tbl_SokPermhnWang>()
            //    .Property(e => e.fld_JumlahTT)
            //    .HasPrecision(13, 2);

            //modelBuilder.Entity<tbl_SokPermhnWang>()
            //    .Property(e => e.fld_JumlahCIT)
            //    .HasPrecision(13, 2);

            //modelBuilder.Entity<tbl_SokPermhnWang>()
            //    .Property(e => e.fld_JumlahManual)
            //    .HasPrecision(13, 2);

            //modelBuilder.Entity<tblPkjmastApp>()
            //    .Property(e => e.fldNoPkj)
            //    .IsFixedLength();

            //modelBuilder.Entity<tblPkjmastApp>()
            //    .Property(e => e.fldNama1)
            //    .IsFixedLength();

            //modelBuilder.Entity<tblPkjmastApp>()
            //    .Property(e => e.fldNoKP)
            //    .IsFixedLength();

            //modelBuilder.Entity<tblPkjmastApp>()
            //    .Property(e => e.fldKdJnsPkj)
            //    .IsFixedLength();

            //modelBuilder.Entity<tblPkjmastApp>()
            //    .Property(e => e.fldKdRkyt)
            //    .IsFixedLength();

            //modelBuilder.Entity<tblPkjmastApp>()
            //    .Property(e => e.fldKdLdg)
            //    .IsFixedLength();

            //modelBuilder.Entity<tblPkjmastApp>()
            //    .Property(e => e.fldJumPjm)
            //    .HasPrecision(7, 2);

            //modelBuilder.Entity<tblPkjmastApp>()
            //    .Property(e => e.fldSbbMsk)
            //    .IsFixedLength();

            //modelBuilder.Entity<tblPkjmastApp>()
            //    .Property(e => e.fldAlsnMsk)
            //    .IsFixedLength();

            //modelBuilder.Entity<tblStatusPkj>()
            //    .Property(e => e.fldNoPkjLama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblStatusPkj>()
            //    .Property(e => e.fldNoKP)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblStatusPkj>()
            //    .Property(e => e.fldNoPkjBaru)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblTaskRemainder>()
            //    .Property(e => e.fldPurpose)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblUserIDApp>()
            //    .Property(e => e.fldUserid)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblUserIDApp>()
            //    .Property(e => e.fldNama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblUserIDApp>()
            //    .Property(e => e.fldNoKP)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblUserIDApp>()
            //    .Property(e => e.fldKdLdg)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblUserIDApp>()
            //    .Property(e => e.fldNamaLdg)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblUserIDApp>()
            //    .Property(e => e.fldJawatan)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblUserIDApp>()
            //    .Property(e => e.fldPassword)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblUserIDApp>()
            //    .Property(e => e.fldStatus)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblStatusPkj>()
            //    .Property(e => e.fldNoPkjLama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblStatusPkj>()
            //    .Property(e => e.fldNoKP)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblStatusPkj>()
            //    .Property(e => e.fldNoPkjBaru)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Upah>()
            //   .Property(e => e.fld_Harga)
            //   .HasPrecision(18, 3);

            //modelBuilder.Entity<tbl_UpahAktiviti>()
            //    .Property(e => e.fld_KdhByr)
            //    .IsFixedLength()
            //    .IsUnicode(false);
        }
    }
}
