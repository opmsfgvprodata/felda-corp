namespace MVC_SYSTEM.ViewingModels
{
    using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;
    

    public partial class FileList
    {
        public string Name { get; set; }

        public DateTime CreationTime { get; set; }

        public float Length { get; set; }

    }
}
