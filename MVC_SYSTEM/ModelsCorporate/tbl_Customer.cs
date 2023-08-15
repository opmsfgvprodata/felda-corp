namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Customer
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(10)]
        public string fld_KodCstmr { get; set; }

        [StringLength(50)]
        public string fld_NamaCstmr { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }

        public DateTime? fld_DtCreated { get; set; }
    }


    [Table("tbl_Customer")]
    public partial class tbl_CustomerCreate
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodCstmr { get; set; }

        [StringLength(50)]
        public string fld_NamaCstmr { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }

        public DateTime? fld_DtCreated { get; set; }
    }
}
