using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCustom
{
    public class CustMod_DataEntryReportResult
    {
        public int? ladangID { get; set; }

        public List<CustMod_PkjDataEntryResult> pkjList { get; set; }
    }

    public class CustMod_PkjDataEntryResult
    {
        public string noPkj { get; set; }

        public string namaPkj { get; set; }

        public List<CustMod_ReportPerDayResult> dataEntryList { get; set; }
    }

    public class CustMod_ReportPerDayResult
    {
        public int Day { get; set; }

        public bool? Task { get; set; }

        public bool? Attendance { get; set; }

    }
}