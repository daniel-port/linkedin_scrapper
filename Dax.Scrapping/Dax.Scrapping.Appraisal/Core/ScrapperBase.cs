using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dax.Scrapping.Appraisal.Core
{
    public class ScrapperBase
    {
        protected ChromiumWebBrowser _brouserComponent;
        public ChromiumWebBrowser BrouserComponent { get { return _brouserComponent; } }

        public ScrapperBase()
        {
            
        }


        protected void Click(string id)
        {
            var scriptTmpl = @"(function() {
                                    document.getElementById('{0}').click();
                               })();";

            var script = scriptTmpl.Replace("{0}", id);
            _brouserComponent.ExecuteScriptAsync(script);
        }

        protected void ScrollDown()
        {
            var scroolScript = @"(function() {                                        
                                    window.scrollTo(0, document.body.scrollHeight || document.documentElement.scrollHeight);
                               })();";


            _brouserComponent.ExecuteScriptAsync(scroolScript);
            Thread.Sleep(2000);
            return;
        }
    }
}
