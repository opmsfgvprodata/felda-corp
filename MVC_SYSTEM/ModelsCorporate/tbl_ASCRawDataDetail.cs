namespace MVC_SYSTEM.ModelsCorporate

{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ASCRawDataDetail
    {
        [Key]
        public long fld_ID { get; set; }

        [StringLength(50)]
        public string fld_ASCFileName { get; set; }

        public int? fld_CountData { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_ServicesName { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
    }
}
