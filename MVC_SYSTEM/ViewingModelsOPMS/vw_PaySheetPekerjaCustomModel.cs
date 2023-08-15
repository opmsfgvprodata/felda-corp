namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class vw_PaySheetPekerjaCustomModel
    {
        public vw_PaySheetPekerja PaySheetPekerja { get; set; }
        //add by kamalia 24/11/21
        public List<CarumanTambahanCustomModel> CarumanTambahan { get; set; }
    }
}
