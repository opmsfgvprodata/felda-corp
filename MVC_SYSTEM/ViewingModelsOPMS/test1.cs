namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class test1 : DbContext
    {
        public test1()
            : base("name=test1")
        {
        }

        public virtual DbSet<tbl_GajiBulanan> tbl_GajiBulanan { get; set; }
        public virtual DbSet<vw_GajiPekerja> vw_GajiPekerja { get; set; }
        public virtual DbSet<vw_MaklumatInsentif> vw_MaklumatInsentif { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_Nopkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_ByrKerja)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_KWSPPkj)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_KWSPMjk)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_SocsoPkj)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_SocsoMjk)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_LainInsentif)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_OT)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_ByrCuti)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_BonusHarian)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_LainPotongan)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_TargetProd)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_CapaiProd)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_ProdInsentif)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_KuaInsentif)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_HdrInsentif)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_AIPS)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_GajiKasar)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_GajiBersih)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_PurataGaji)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_PurataGaji12Bln)
                .HasPrecision(8, 2);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Nopkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Nokp)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Nama)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Almt1)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Daerah)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Neg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Negara)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Poskod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Notel)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Nofax)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Kdjnt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Kdbgsa)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Kdagma)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Kdrkyt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Kdkwn)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Kpenrka)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Kdaktf)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Sbtakf)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Ktgpkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Jenispekerja)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Kodbkl)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_KodSocso)
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Noperkeso)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_KodKWSP)
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Nokwsp)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Visano)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Nogilr)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Prmtno)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Psptno)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_Kdldg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_IDpkj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_StatusKwspSocso)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_StatusAkaun)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_GajiPekerja>()
                .Property(e => e.fld_ByrKerja)
                .HasPrecision(8, 2);

            modelBuilder.Entity<vw_MaklumatInsentif>()
                .Property(e => e.fld_Nopkj)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}
