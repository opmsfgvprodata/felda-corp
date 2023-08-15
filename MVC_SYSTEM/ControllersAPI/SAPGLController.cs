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
    public class SAPGLController : ApiController
    {
        [HttpPost]
        public bool Post(HttpRequestMessage request)
        {
            List<SAPGL> SAPGL = new List<SAPGL>();
            bool Result = false;
            var doc = new XmlDocument();
            try
            {
                doc.Load(request.Content.ReadAsStreamAsync().Result);

                foreach (XmlNode node in doc.SelectNodes("/SAPInter/SAPGL"))
                {
                    SAPGL.Add(new SAPGL
                    {
                        BUKRS = node["BUKRS"].InnerText,
                        SAKNR = node["SAKNR"].InnerText,
                        SKA1TXT50 = node["SKA1-TXT50"].InnerText
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
