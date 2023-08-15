namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_KerjaHarian
    {
        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(4)]
        public string fld_Kum { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fld_Tarikh { get; set; }

        [StringLength(1)]
        public string fld_Hujan { get; set; }

        [StringLength(2)]
        public string fld_Kdhdct { get; set; }

        [StringLength(3)]
        public string fld_Glcd { get; set; }

        [StringLength(5)]
        public string fld_Pkt { get; set; }

        [StringLength(4)]
        public string fld_Aktvt { get; set; }

        [StringLength(10)]
        public string fld_No_Kontrak { get; set; }

        [StringLength(1)]
        public string fld_Kdhbyr { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kgk { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kti1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kti3 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kong { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kdrot { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jamot { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Qty { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Amt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jumlah { get; set; }

        [StringLength(12)]
        public string fld_UserId { get; set; }

        [StringLength(40)]
        public string fld_UserName { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(40)]
        public string fld_Nama1 { get; set; }

        [Key]
        public Guid fld_UniqueID { get; set; }
    }
}
