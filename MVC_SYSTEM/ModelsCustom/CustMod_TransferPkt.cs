using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCustom
{
    public class CustMod_TransferPkt
    {
        public int wilayahList1 { get; set; }

        public int ladangList1 { get; set; }

        public string JnisAktvt { get; set; }

        public short JnisPkt { get; set; }

        public int PilihanPkt { get; set; }

        public int wilayahList2 { get; set; }

        public int ladangList2 { get; set; }

        public DateTime DateEnd { get; set; }
    }
}