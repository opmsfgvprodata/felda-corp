using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using MVC_SYSTEM.ModelsSAP;

namespace MVC_SYSTEM.ControllersAPI
{
    public class SAPCCController : ApiController
    {
        [HttpPost]
        public bool Post(HttpRequestMessage request)
        {
            List<SAPCC> SAPCC = new List<SAPCC>();
            bool Result = false;
            var doc = new XmlDocument();
            try
            {
                doc.Load(request.Content.ReadAsStreamAsync().Result);

                XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(doc.NameTable);

                xmlnsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                xmlnsManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xmlnsManager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
                xmlnsManager.AddNamespace("si", "http://example.com/SystemIntegration");


                foreach (XmlNode node in doc.SelectNodes("/SAPInter/SAPCC"))
                {
                    SAPCC.Add(new SAPCC
                    {
                        BUKRS = node["BUKRS"].InnerText,
                        KOSTL = node["KOSTL"].InnerText,
                        LTEXT = node["LTEXT"].InnerText
                    });
                }

                Result = true;
            }
            catch (Exception ex)
            {
                Result = false;
            }

            return Result;
        }
    }
}
