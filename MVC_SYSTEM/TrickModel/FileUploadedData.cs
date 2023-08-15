namespace MVC_SYSTEM.TrickModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FileUploadedData
    {
        [Key]
        public int ID { get; set; }

        public string FileName { get; set; }

        public DateTime DateTimeCreated { get; set; }
        
        public long SizeFile { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }
    }
}
