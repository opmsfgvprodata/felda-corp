using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class sp_RptMasterdataPekerja_Result
    {
        public System.Guid fld_ID { get; set; }
        public string fld_NamaWilayah { get; set; }
        public string fld_KodLadang { get; set; }
        public string fld_NoPkj { get; set; }
        public string fld_Nama { get; set; }
        public string fld_NoKP { get; set; }
        public string fld_KodWarganegara { get; set; }
        public string fld_KodKategoriPekerja { get; set; }
        public string fld_KodPembekal { get; set; }
        public Nullable<System.DateTime> fld_TarikhMulaKhidmat { get; set; }
        public string fld_KodStatusAktif { get; set; }
        public string fld_KodSebabTakAktif { get; set; }
        public Nullable<System.DateTime> fld_TarikhTamatPermit { get; set; }
        public Nullable<System.DateTime> fld_TarikhTamatPassport { get; set; }
        public Nullable<int> fld_NegaraID { get; set; }
        public Nullable<int> fld_SyarikatID { get; set; }
        public Nullable<int> fld_WilayahID { get; set; }
        public Nullable<int> fld_LadangID { get; set; }
        public Nullable<int> fld_CreatedBy { get; set; }
        public string fld_Prmtno { get; set; }
        public string fld_Noperkeso { get; set; }
        public string fld_Kdjnt { get; set; }
        public string fld_CompCode { get; set; }
        public Nullable<System.DateTime> fld_Trlhr { get; set; }
        public Nullable<int> fld_Umur { get; set; }
        public string fld_statuskhwn { get; set; }
        public string fld_Kdbangsa { get; set; }
        public string fld_Kdagama { get; set; }
        public string fld_Alamat1 { get; set; }
        public string fld_Poskod { get; set; }
        public string fld_Daerah { get; set; }
        public string fld_Negeri { get; set; }
        public string fld_Negara { get; set; }
        public string fld_NoTel { get; set; }
        public Nullable<System.DateTime> fld_TrkCopImigLantikan { get; set; }
        public Nullable<System.DateTime> fld_Trkmulakhidmat { get; set; }
        public string fld_Modbayaran { get; set; }
        public string fld_Namawaris { get; set; }
        public string fld_Hubwaris { get; set; }
        public string fld_Notelwaris { get; set; }
        public string fld_Generasi { get; set; }
        public Nullable<System.DateTime> fld_Trkhtakaktif { get; set; }
        public Nullable<System.DateTime> fld_TarikhSahJawatan { get; set; }
    }
}