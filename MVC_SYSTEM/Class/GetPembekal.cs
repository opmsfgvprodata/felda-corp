using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetPembekal
    {
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        public string GetNamaPembekal(string codebkl)
        {
            string pembekal = db.tbl_Pembekal.Where(x => x.fld_KodPbkl == codebkl).Select(s => s.fld_NamaPbkl).FirstOrDefault();
            return pembekal;
        }
    }
}