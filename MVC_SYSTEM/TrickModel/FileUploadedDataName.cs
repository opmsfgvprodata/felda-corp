namespace MVC_SYSTEM.TrickModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FileUploadedDataName
    {
        [Key]
        public int ID { get; set; }

        public string FileName { get; set; }

        public string KodLadang { get; set; }

        public string NamaLadang { get; set; }
    }
}
