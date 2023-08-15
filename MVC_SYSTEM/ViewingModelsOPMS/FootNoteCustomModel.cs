namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FootNoteCustomModel
    {
        [StringLength(100)]
        public string fld_Desc { get; set; }

        public decimal? fld_Bilangan { get; set; }
    }
}
