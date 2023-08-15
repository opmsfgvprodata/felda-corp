namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Pkjmast
    {
        [Key]
        public Guid fld_UniqueID { get; set; }

        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(9)]
        public string fld_Nokpl { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Ttsplp { get; set; }

        [StringLength(10)]
        public string fld_Noplks { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Ttplks { get; set; }

        [StringLength(40)]
        public string fld_Nama1 { get; set; }

        [StringLength(40)]
        public string fld_Nama2 { get; set; }

        [StringLength(30)]
        public string fld_Almt1 { get; set; }

        [StringLength(30)]
        public string fld_Almt2 { get; set; }

        [StringLength(30)]
        public string fld_Almt3 { get; set; }

        [StringLength(2)]
        public string fld_Neg { get; set; }

        [StringLength(5)]
        public string fld_Poskod { get; set; }

        [StringLength(10)]
        public string fld_Notel { get; set; }

        [StringLength(10)]
        public string fld_Nofax { get; set; }

        [StringLength(30)]
        public string fld_Almtt1 { get; set; }

        [StringLength(30)]
        public string fld_Almtt2 { get; set; }

        [StringLength(30)]
        public string fld_Almtt3 { get; set; }

        [StringLength(2)]
        public string fld_Negt { get; set; }

        [StringLength(5)]
        public string fld_Pskdt { get; set; }

        [StringLength(10)]
        public string fld_Notelt { get; set; }

        [StringLength(10)]
        public string fld_Nofaxt { get; set; }

        [StringLength(1)]
        public string fld_Kdjnt { get; set; }

        [StringLength(2)]
        public string fld_Kdbgsa { get; set; }

        [StringLength(1)]
        public string fld_Kdagma { get; set; }

        [StringLength(1)]
        public string fld_Btutur1 { get; set; }

        [StringLength(1)]
        public string fld_Btutur2 { get; set; }

        [StringLength(1)]
        public string fld_Btutur3 { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trlhr { get; set; }

        [StringLength(30)]
        public string fld_Tptlhr { get; set; }

        [StringLength(2)]
        public string fld_Neglhr { get; set; }

        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }

        [StringLength(1)]
        public string fld_Kdtkwn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Bilank { get; set; }

        [StringLength(1)]
        public string fld_Kpenrka { get; set; }

        [Required]
        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trtakf { get; set; }

        [StringLength(60)]
        public string fld_Sbtakf { get; set; }

        [StringLength(2)]
        public string fld_Kdzon { get; set; }

        [StringLength(2)]
        public string fld_Kdkaw { get; set; }

        [Required]
        [StringLength(3)]
        public string fld_Kdldg { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trmlkj { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trshjw { get; set; }

        [StringLength(2)]
        public string fld_Kdjwtm { get; set; }

        [Required]
        [StringLength(2)]
        public string fld_Ktgpkj { get; set; }

        [StringLength(12)]
        public string fld_Nokp { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Gajisehari { get; set; }

        [Required]
        [StringLength(2)]
        public string fld_Jenispekerja { get; set; }

        [StringLength(3)]
        public string fld_Jenisinsuran { get; set; }

        [StringLength(12)]
        public string fld_Noperkeso { get; set; }

        [StringLength(12)]
        public string fld_Nokwsp { get; set; }

        [StringLength(12)]
        public string fld_Nocpendptn { get; set; }

        [StringLength(3)]
        public string fld_Namasingkat { get; set; }

        [StringLength(2)]
        public string fld_Negra { get; set; }

        [StringLength(2)]
        public string fld_Negrat { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Thvisa { get; set; }

        [StringLength(13)]
        public string fld_Batcno { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jumpjm { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Penlev { get; set; }

        [StringLength(1)]
        public string fld_Sthadr { get; set; }

        [StringLength(3)]
        public string fld_Kodbkl { get; set; }

        [StringLength(13)]
        public string fld_Clvisa { get; set; }

        [StringLength(13)]
        public string fld_Nogilr { get; set; }

        [StringLength(2)]
        public string fld_Kodtakf { get; set; }

        [StringLength(17)]
        public string fld_Granteno { get; set; }

        [StringLength(18)]
        public string fld_Insurano { get; set; }

        [StringLength(20)]
        public string fld_Prmtno { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1prmt { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T2prmt { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1insr { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T2insr { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1grnt { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T2grnt { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trkdload { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trdload { get; set; }

        [StringLength(2)]
        public string fld_Jnspmt { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        [StringLength(50)]
        public string fld_ServicesName { get; set; }

        public int? fld_UploadBy { get; set; }

        public DateTime? fld_UploadDate { get; set; }

        [StringLength(5)]
        public string fld_UploadCdLdg { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public long? fld_ProcessID { get; set; }

        [StringLength(50)]
        public string fld_ASCFileName { get; set; }
    }
}
