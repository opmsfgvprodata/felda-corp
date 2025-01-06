namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public partial class tbl_RptMasterJumPkj
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(50)]
        public string fld_NamaSyarikat { get; set; }

        [StringLength(50)]
        public string fld_NamaWilayah { get; set; }
        
        [StringLength(5)]
        public string fld_KodLadang { get; set; }

        [StringLength(50)]
        public string fld_NamaLadang { get; set; }
        
        public int? fld_Perlu { get; set; }
        public int? fld_JumPkjSediaAda { get; set; }
        public int? fld_BilPkjJan { get; set; }
        public int? fld_BilPkjFeb { get; set; }
        public int? fld_BilPkjMac { get; set; }
        public int? fld_BilPkjApr { get; set; }
        public int? fld_BilPkjMei { get; set; }
        public int? fld_BilPkjJun { get; set; }
        public int? fld_BilPkjJul { get; set; }
        public int? fld_BilPkjOgos { get; set; }
        public int? fld_BilPkjSept { get; set; }
        public int? fld_BilPkjOkt { get; set; }
        public int? fld_BilPkjNov { get; set; }
        public int? fld_BilPkjDis { get; set; }
        public int? fld_JumPkjSmsa { get; set; }
        public int? fld_JumPkj { get; set; }
        public int? fld_BilPkjCom { get; set; }
        public int? fld_BilPkjUnfit { get; set; }
        public int? fld_BilPkjPindah { get; set; }
        public int? fld_BilPkjTngkpUsir { get; set; }
        public int? fld_BilPkjMeninggal { get; set; }
        public int? fld_BilPkjLari { get; set; }
        public int? fld_JumPkjKeluarSmsa { get; set; }
        public int? fld_JumTKT { get; set; }
        public int? fld_JumTKA { get; set; }
        public int? fld_JumTKTPrdktvt { get; set; }
        public int? fld_JumTKTArtisan { get; set; }
        public int? fld_BilPkjKontraktor { get; set; }
        public int? fld_PrcntPkjKontraktor { get; set; }
        public int? fld_JumBesar { get; set; }
        public int? fld_PrcntJumBesar { get; set; }
        public int? fld_KekurangSmsa { get; set; }
        public int? fld_PrcntKekurangSmsa { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public int? fld_WilayahID { get; set; }
        public int? fld_LadangID { get; set; }
        public int? fld_CreatedBy { get; set; }
        [StringLength(10)]
        public string fld_CompCode { get; set; }
    }
}