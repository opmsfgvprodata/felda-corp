using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class HTMLWorkerExtended : HTMLWorker
    {
        string logopath1 = "";
        public HTMLWorkerExtended(IDocListener document, string logopath) : base(document)
        {
            logopath1 = logopath;
        }
        public override void StartElement(string tag, IDictionary<string, string> str)
        {
            if (tag.Equals("newpage"))
            {
                document.Add(Chunk.NEXTPAGE);
            }
            else if (tag.Equals("logo"))
            {
                Image logo = Image.GetInstance(logopath1);
                //Image alignment
                logo.ScaleToFit(50f, 50f);
                logo.Alignment = Image.TEXTWRAP | Image.ALIGN_CENTER;
                document.Add(logo);
            }
            else
            {
                base.StartElement(tag, str);
            } 
        }
    }
}