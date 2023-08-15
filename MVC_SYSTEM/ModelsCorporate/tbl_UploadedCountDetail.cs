namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_UploadedCountDetail
    {
        [Key]
        public long fld_ID { get; set; }

        [StringLength(50)]
        public string fld_FileName { get; set; }

        public int? fld_InFileCount { get; set; }

        public int? fld_DwnloadCount { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public long? fld_ProcessID { get; set; }

        public int? fld_UploadBy { get; set; }
    }
}
