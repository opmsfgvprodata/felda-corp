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
    public class SAPIOController : ApiController
    {
        [HttpPost]
        public bool Post(HttpRequestMessage request)
        {
            List<SAPIO> SAPIO = new List<SAPIO>();
            bool Result = false;
            var doc = new XmlDocument();
            try
            {
                doc.Load(request.Content.ReadAsStreamAsync().Result);

                foreach (XmlNode node in doc.SelectNodes("/SAPInter/SAPIO"))
                {
                    SAPIO.Add(new SAPIO
                    {
                        ZBUKRS = node["ZBUKRS"].InnerText,
                        ZKDRGI = node["ZKDRGI"].InnerText,
                        ZKDRGN = node["ZKDRGN"].InnerText,
                        ZKDPKT = node["ZKDPKT"].InnerText,
                        ZKDPK2 = node["ZKDPK2"].InnerText,
                        ZPSND1 = node["ZPSND1"].InnerText,
                        ZPSND2 = node["ZPSND2"].InnerText,
                        ZPSND3 = node["ZPSND3"].InnerText,
                        ZPSND4 = node["ZPSND4"].InnerText,
                        ZPSND5 = node["ZPSND5"].InnerText,
                        ZPSND6 = node["ZPSND6"].InnerText,
                        ZJMLTF = decimal.Parse(node["ZJMLTF"].InnerText),
                        ZLKWTN = decimal.Parse(node["ZLKWTN"].InnerText),
                        ZLSKBH = decimal.Parse(node["ZLSKBH"].InnerText),
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
