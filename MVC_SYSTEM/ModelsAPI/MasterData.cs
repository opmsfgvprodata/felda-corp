using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsAPI
{
    public class MasterData
    {
        
    }

    public class ZOPMSGL
    {
        public string bukrs { get; set; }
        public string saknr { get; set; }
        public string txt50 { get; set; }
        public string xloeb { get; set; }

    }

    public class ZOPMSCC
    {
        public string kokrs { get; set; }
        public string kostl { get; set; }
        public string ltext { get; set; }
        public string bkzkp { get; set; }
    }
    
    public class ZOPMSCS
    {
        public string bukrs { get; set; }
        public string kunnr { get; set; }
        public string name1 { get; set; }
        public string loevm { get; set; }
    }

    public class ZOPMSVD
    {
        public string bukrs { get; set; }
        public string lifnr { get; set; }
        public string name1 { get; set; }
        public string loevm { get; set; }
    }

    public class ZOPMSSLP
    {
        public string zbukrs { get; set; }

        public string zkdrgi { get; set; }

        public string zkdrgn { get; set; }

        public string zkdpkt { get; set; }

        public string zkdpk2 { get; set; }

        public string zthnpb { get; set; }

        public string zthpts { get; set; }

        public string zgpcos { get; set; }

        public string zpsnd1 { get; set; }

        public string zpsnd2 { get; set; }

        public string zpsnd3 { get; set; }

        public string zpsnd4 { get; set; }

        public string zpsnd5 { get; set; }

        public string zpsnd6 { get; set; }

        public string zthtmb { get; set; }

        public decimal zblpn2 { get; set; }

        public decimal zblot2 { get; set; }

        public string zpkpbg { get; set; }

        public string zththp { get; set; }

        public string zthmtm { get; set; }

        public decimal zjmltf { get; set; }

        public decimal zlkwtn { get; set; }

        public decimal zlskbh { get; set; }

        public decimal zlskbf { get; set; }

        public decimal zlskbp { get; set; }

        public string zjstnm { get; set; }

        public string zkdblk { get; set; }

        public string zjenki { get; set; }

        public decimal zldltf { get; set; }

        public decimal zldltp { get; set; }

        public string zjsblk { get; set; }

        public string zjskws { get; set; }

        public string zblpr3 { get; set; }

        public decimal zblkwu { get; set; }

        public decimal zblkwr { get; set; }
    }

    public class RETURN
    {
        public string type { get; set; }
        public string id { get; set; }
        public string number { get; set; }
        public string message { get; set; }
        public string logno { get; set; }
        public string logmsgno { get; set; }
        public string message1 { get; set; }
        public string message2 { get; set; }
        public string message3 { get; set; }
        public string message4 { get; set; }
        public string parameter { get; set; }
        public string row { get; set; }
        public string field { get; set; }
        public string system { get; set; }
    }

    
}