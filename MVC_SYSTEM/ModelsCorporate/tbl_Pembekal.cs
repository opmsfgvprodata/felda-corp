namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Pembekal
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodPbkl { get; set; }

        [StringLength(50)]
        public string fld_NamaPbkl { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    [Table ("tbl_Pembekal")]
    public partial class tbl_PembekalCreate
    {
        [Key]
        public int fld_ID { get; set; }

        [Required]
        [StringLength(5)]
        public string fld_KodPbkl { get; set; }

        [Required]
        [StringLength(50)]
        public string fld_NamaPbkl { get; set; }

        [Required]
        public int? fld_SyarikatID { get; set; }

        [Required]
        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
