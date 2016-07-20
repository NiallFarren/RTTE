using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace RealTimeTextEditor
{
    public class HtmlRtfConvertor
    {
        public void ThreadConvertor(string inputpath, string inputhtml)
        {
            var t = new Thread(ConvertHtmltoRtf);
            var dataForConversion = new ConvertorData { path = inputpath, html = inputhtml };
            t.SetApartmentState(ApartmentState.STA);
            t.Start(dataForConversion);
            t.Join();
            
        }

        public static void ConvertHtmltoRtf (object obj)
        {
            var data = obj as ConvertorData;
            using (WebBrowser tempBrowser = new WebBrowser())
            {   
                tempBrowser.CreateControl();
                tempBrowser.DocumentText = data.html;
                while (tempBrowser.DocumentText != data.html)
                {
                    Application.DoEvents();
                }
                tempBrowser.Document.ExecCommand("SelectAll", false, null);
                tempBrowser.Document.ExecCommand("Copy", false, null);
                using (RichTextBox rtb = new RichTextBox())
                {
                    rtb.Paste();
                    rtb.SaveFile(data.path);

                }
            }

        }

        public class ConvertorData
        {
            public string path { get; set; }
            public string html { get; set; }
        }

    }
}