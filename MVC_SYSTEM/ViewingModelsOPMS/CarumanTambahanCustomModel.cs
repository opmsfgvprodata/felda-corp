using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModelsOPMS
{
    public class CarumanTambahanCustomModel
    {
        [Key]
        public Guid? fld_ID { get; set; }

        public string fld_KodCarumanTambahan { get; set; }

        public decimal? fld_CarumanPekerja { get; set; }

        public decimal? fld_CarumanMajikan { get; set; }
    }
}