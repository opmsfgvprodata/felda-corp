using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Kerja
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(20)]
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

        [Column(TypeName = "numeric")]
        public decimal? fld_HrgaKwsnSkar { get; set; }

        [StringLength(10)]
        public string fld_KodKwsnSkar { get; set; }

        public int? fld_ApprovalKwsnSkarLainBy { get; set; }

        public DateTime? fld_ApprovalKwsnSkarDT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_OverallAmount { get; set; }

        public bool? fld_PinjamStatus { get; set; }
    }
}
