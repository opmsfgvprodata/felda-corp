using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class CheckSharedFolder
    {
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        errorlog geterror = new errorlog();
        public bool getsharedfolderfile(string file)
        {
            bool result = false;

            try
            {
                FileInfo dirInfo = new FileInfo(file);

                if (dirInfo.Exists)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }
            
            return result;
        }
        public string createfolderfile(string month, string year, string kodcode, string filesource)
        {
            string folderfile = "";

            folderfile = filesource + month + year + "\\" + kodcode + month + year + ".zip";

            return folderfile;
        }

        public string GetSourceTargetPath(string Flag1, int? NegaraID, int? SyarikatID)
        {
            string result = "";

            result = db.tbl_OptionConfig.Where(x => x.fldOptConfFlag1 == Flag1 && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();

            return result;
        }
    }
}