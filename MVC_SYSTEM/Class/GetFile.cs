using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.Class
{
    public class GetFile
    {
        public string GetFileName(int id)
        {
            MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();

            string filename = db.tblASCApprovalFileDetails.Where(x => x.fldID == id).Select(s => s.fldFileName).FirstOrDefault();

            return filename;
        }
    }
}