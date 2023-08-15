namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Version
    {
        [Key]
        public Guid fld_UniqueID { get; set; }

        [StringLength(12)]
        public string fld_prog_date { get; set; }

        [StringLength(20)]
        public string fld_progr_ver { get; set; }

        [StringLength(5)]
        public string fld_kdldg { get; set; }

        public int? fld_prog_year { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_trkdload { get; set; }

        [StringLength(40)]
        public string fld_Nmpgrs { get; set; }

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
