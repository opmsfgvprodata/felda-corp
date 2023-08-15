namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CutiUmum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fld_CutiUmumID { get; set; }

        [StringLength(100)]
        public string fld_KeteranganCuti { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime? fld_TarikhCuti { get; set; }

        public int? fld_Negeri { get; set; }

        public short? fld_Tahun { get; set; }
    }
}
