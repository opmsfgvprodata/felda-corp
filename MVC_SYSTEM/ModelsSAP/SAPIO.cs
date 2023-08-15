using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsSAP
{
    public class SAPIO
    {
        //Comp_Code
        public string ZBUKRS { get; set; }

        //Ind_Rncgn
        public string ZKDRGI { get; set; }

        //Kod_Rncgn
        public string ZKDRGN { get; set; }

        //Kod_Prgkt
        public string ZKDPKT { get; set; }

        //Kod_Sub_Prgkt 
        public string ZKDPK2 { get; set; }

        //IO_NO1
        public string ZPSND1 { get; set; }

        //IO_NO2
        public string ZPSND2 { get; set; }

        //IO_NO3
        public string ZPSND3 { get; set; }

        //IO_NO4
        public string ZPSND4 { get; set; }

        //IO_NO5
        public string ZPSND5 { get; set; }

        //IO_NO6
        public string ZPSND6 { get; set; }

        //Luas_Kslrhan
        public decimal ZJMLTF { get; set; }

        //Luas_Tnman
        public decimal ZLKWTN { get; set; }

        //Luas_Bhasil
        public decimal ZLSKBH { get; set; }
    }
}