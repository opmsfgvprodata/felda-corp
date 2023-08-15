namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_PaySlipPekerja
    {
        public vw_GajiPekerja Pkjmast { get; set; }
        public ViewingModelsOPMS.tbl_GajiBulanan GajiBulanan { get; set; }
        public List<vw_MaklumatInsentif> InsentifPekerja { get; set; }
        public List<KerjaPekerjaCustomModel> KerjaPekerja { get; set; }
        public List<OTPekerjaCustomModel> OTPekerja { get; set; }
        public List<BonusPekerjaCustomModel> BonusPekerja { get; set; }
        public List<CutiPekerjaCustomModel> CutiPekerja { get; set; }
        public List<FootNoteCustomModel> FootNote { get; set; }
    }
}
