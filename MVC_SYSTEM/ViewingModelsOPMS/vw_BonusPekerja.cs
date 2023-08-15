namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_BonusPekerja
    {
        public Guid? fld_ID { get; set; }

        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(50)]
        public string fld_Kum { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Tarikh { get; set; }

        public byte? fld_JnsPkt { get; set; }

        [StringLength(10)]
        public string fld_KodPkt { get; set; }

        [StringLength(2)]
        public string fld_JnisAktvt { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [StringLength(10)]
        public string fld_Unit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KadarByr { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahHasil { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_BrtGth { get; set; }

        public byte? fld_PerBrshGth { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kong { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Amount { get; set; }

        [StringLength(5)]
        public string fld_KodGL { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JamOT { get; set; }

        public byte? fld_Bonus { get; set; }

        public short? fld_Quality { get; set; }

        [StringLength(1)]
        public string fld_KdhMenuai { get; set; }

        [StringLength(1)]
        public string fld_DataSource { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        [StringLength(3)]
        public string fld_Kdhdct { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string fldOptConfDesc { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string fldOptConfFlag1 { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag2 { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag3 { get; set; }

        [StringLength(100)]
        public string fld_Desc { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kadar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jumlah { get; set; }
    }
}
