using MVC_SYSTEM.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.log
{
    public class errorlog
    {
        ChangeTimeZone timezone = new ChangeTimeZone();
        public void catcherro(string data, string data2, string data3, string data4)
        {
            string message = string.Format("Time: {0}", timezone.gettimezone().ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", data);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", data2);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", data3);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", data4);
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = HttpContext.Current.Server.MapPath("~/ErrorLog/ErrorLog-" + timezone.gettimezone().ToString("dd-MM-yyyy") + ".txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }

        public void testlog(string data1, string data2, string data3)
        {
            string path = HttpContext.Current.Server.MapPath("~/ErrorLog/testlog-" + timezone.gettimezone().ToString("dd-MM-yyyy") + ".txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine("Data 1 : " + data1 + ", Data 2 : " + data2 + ", Data 3 : " + data3 + ", DateTime : " + timezone.gettimezone());
                writer.Close();
            }
        }
    }
}