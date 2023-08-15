using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.ViewingModels
{
    public class MVC_SYSTEM_Viewing : DbContext
    {
        public static string host1 = "";
        public static string catalog1 = "";
        public static string user1 = "";
        public static string pass1 = "";
        public MVC_SYSTEM_Viewing()
            : base(nameOrConnectionString: "MVC_SYSTEM_HQ_CONN")
        {
            if (host1 != "" && catalog1 != "" && user1 != "" && pass1 != "")
            {
                base.Database.Connection.ConnectionString = "data source=" + host1 + ";initial catalog=" + catalog1 + ";user id=" + user1 + ";password=" + pass1 + ";MultipleActiveResultSets=True;App=EntityFramework";
            }

            host1 = "";
            catalog1 = "";
            user1 = "";
            pass1 = "";
        }

        public static MVC_SYSTEM_Viewing ConnectToSqlServer(string host, string catalog, string user, string pass)
        {
            host1 = host;
            catalog1 = catalog;
            user1 = user;
            pass1 = pass;

            return new MVC_SYSTEM_Viewing();

        }

        public virtual DbSet<tblSystemConfig> tblSystemConfigs { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblOptionGeneralConfigsWeb> tblOptionGeneralConfigsWeb { get; set; }
        public virtual DbSet<tbl_PerluLadang> tbl_PerluLadang { get; set; }
        public virtual DbSet<tbl_QuotaPerluLadang> tbl_QuotaPerluLadang { get; set; }
        public virtual DbSet<tblOptionConfigsWeb> tblOptionConfigsWeb { get; set; }
        public virtual DbSet<tblEmailList> tblEmailList { get; set; }
        public virtual DbSet<tbl_Ladang> tbl_Ladang { get; set; }
        public virtual DbSet<tbl_Pembekal> tbl_Pembekal { get; set; }
        public virtual DbSet<tbl_Wilayah> tbl_Wilayah { get; set; }
        //public virtual DbSet<tbl_Asaspkt> tbl_Asaspkt { get; set; }
        //public virtual DbSet<tbl_Pkjmast> tbl_Pkjmast { get; set; }
        //public virtual DbSet<tblPkjmastApp> tblPkjmastApp { get; set; }
        //public virtual DbSet<tbl_KumpulanKerja> tbl_KumpulanKerja { get; set; }
        //public virtual DbSet<vw_KumpulanPekerja> vw_KumpulanPekerja { get; set; }
        //public virtual DbSet<vw_KumpulanKerja> vw_KumpulanKerja { get; set; }
        public virtual DbSet<tbl_UpahMenuai> tbl_UpahMenuai { get; set; }

        //fatin added - 3/5/2023
        public virtual DbSet<tbl_UpahGetah> tbl_UpahGetah { get; set; }
        //end

        //fatin added - 15/05/2023
        public virtual DbSet<tbl_HargaGetahSMR> tbl_HargaGetahSMR { get; set; }
        public virtual DbSet<tbl_ProduktivitiGetah> tbl_ProduktivitiGetah { get; set; }
        public virtual DbSet<tbl_InsentifKGK> tbl_InsentifKGK { get; set; }
        //end

        //public virtual DbSet<vw_GajiBulananPekerja> vw_GajiBulananPekerja { get; set; }
        //public virtual DbSet<tbl_Upah> tbl_Upah { get; set; }
        public virtual DbSet<tbl_HasilSawit> tbl_HasilSawit { get; set; }
        //public virtual DbSet<tbl_Insentif> tbl_Insentif { get; set; }
        //public virtual DbSet<vw_MaklumatInsentif> vw_MaklumatInsentif { get; set; }
        //public virtual DbSet<vw_InsentifPekerja> vw_InsentifPekerja { get; set; }
        //public virtual DbSet<vw_MaklumatInsentifPekerja> vw_MaklumatInsentifPekerja { get; set; }
        //public virtual DbSet<Checkboxes> Checkboxes { get; set; }
        //public virtual DbSet<vw_MaklumatProduktiviti> vw_MaklumatProduktiviti { get; set; }
        //public virtual DbSet<vw_HasilSawitBlok> vw_HasilSawitBlok { get; set; }
        //public virtual DbSet<vw_HasilSawitSubPkt> vw_HasilSawitSubPkt { get; set; }
        //public virtual DbSet<vw_HasilSawitPkt> vw_HasilSawitPkt { get; set; }
        //public virtual DbSet<vw_MaklumatCuti> vw_MaklumatCuti { get; set; }
        public virtual DbSet<tbl_CutiPeruntukan> tbl_CutiPeruntukan { get; set; }
        //public virtual DbSet<tbl_CutiDiambil> tbl_CutiDiambil { get; set; }
        //public virtual DbSet<tbl_Kerjahdr> tbl_Kerjahdr { get; set; }
        public virtual DbSet<tbl_UpahAktiviti> tbl_UpahAktiviti { get; set; }
        public virtual DbSet<tbl_Kwsp> tbl_Kwsp { get; set; }
        public virtual DbSet<tbl_Socso> tbl_Socso { get; set; }
        //public virtual DbSet<tbl_Skb> tbl_Skb { get; set; }
        //public virtual DbSet<tbl_CutiKategori> tbl_CutiKategori { get; set; }
        public virtual DbSet<tbl_CutiMaintenance> tbl_CutiMaintenance { get; set; }
        //public virtual DbSet<tbl_CutiUmum> tbl_CutiUmum { get; set; }
        public virtual DbSet<vw_CutiUmumNegeri> vw_CutiUmumNegeri { get; set; }
        //public virtual DbSet<tbl_Sctran> tbl_Sctran { get; set; }
        //public virtual DbSet<vw_RptSctran> vw_RptSctran { get; set; }
        //public virtual DbSet<tbl_GajiBulanan> tbl_GajiBulanan { get; set; }
        //public virtual DbSet<vw_KerjaPekerja> vw_KerjaPekerja { get; set; }
        //public virtual DbSet<KerjaPekerjaCustomModel> KerjaPekerjaCustomModel { get; set; }
        //public virtual DbSet<vw_OTPekerja> vw_OTPekerja { get; set; }
        //public virtual DbSet<OTPekerjaCustomModel> OTPekerjaCustomModel { get; set; }
        //public virtual DbSet<tbl_KerjaBonus> tbl_KerjaBonus { get; set; }
        //public virtual DbSet<vw_BonusPekerja> vw_BonusPekerja { get; set; }
        //public virtual DbSet<vw_CutiPekerja> vw_CutiPekerja { get; set; }
        //public virtual DbSet<CutiPekerjaCustomModel> CutiPekerjaCustomModel { get; set; }
        //public virtual DbSet<vw_GajiPekerja> vw_GajiPekerja { get; set; }
        //public virtual DbSet<vw_KehadiranPekerja> vw_KehadiranPekerja { get; set; }
        public virtual DbSet<vw_MingguNegeri> vw_MingguNegeri { get; set; }
        //public virtual DbSet<vw_PaySheetPekerja> vw_PaySheetPekerja { get; set; }
        //public virtual DbSet<tbl_GajiMinima> tbl_GajiMinima { get; set; }
        //public virtual DbSet<vw_GajiMinima> vw_GajiMinima { get; set; }
        //public virtual DbSet<tbl_Produktiviti> tbl_Produktiviti { get; set; }
        //public virtual DbSet<vw_HargaSemasa> vw_HargaSemasa { get; set; }
        public virtual DbSet<tbl_CarumanTambahan> tbl_CarumanTambahan { get; set; }
        public virtual DbSet<vw_HariBekerja> vw_HariBekerja { get; set; }
        //public virtual DbSet<tbl_HargaSawitRange> tbl_HargaSawitRange { get; set; }
        public virtual DbSet<tbl_MapGL> tbl_MapGL { get; set; }
        public virtual DbSet<tbl_KategoriAktiviti> tbl_KategoriAktiviti { get; set; }
        public virtual DbSet<vw_CutiUmumLdg> vw_CutiUmumLdg { get; set; }
        public virtual DbSet<vw_CutiUmumLdgDetails> vw_CutiUmumLdgDetails { get; set; }
        public virtual DbSet<vw_KelayakanInsentifLdg> vw_KelayakanInsentifLdg { get; set; }

        public virtual DbSet<tbl_Customer> tbl_Customer { get; set; }

        //farahin - 6/8/2021
        public virtual DbSet<tbl_CMSAP> tbl_CMSAP { get; set; }
        public virtual DbSet<tbl_CCSAP> tbl_CCSAP { get; set; }
        public virtual DbSet<tbl_VDSAP> tbl_VDSAP { get; set; }
        //sepul - 31/03/2023
        public virtual DbSet<vw_LadangDetails> vw_LadangDetails { get; set; }

    }
}