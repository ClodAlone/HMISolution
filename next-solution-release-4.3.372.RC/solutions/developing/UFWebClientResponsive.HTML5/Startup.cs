using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(UFWebClientResponsive_HTML5.Startup))]

// Files related to ASP.NET Identity duplicate the Microsoft ASP.NET Identity file structure and contain initial Microsoft comments.

namespace UFWebClientResponsive_HTML5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            DevExpress.Xpf.Bars.ElementRegistrator.GlobalSkipUniquenessCheck = true;
            //GlobalHost.Configuration.DefaultMessageBufferSize = 100;
            app.MapSignalR();
        }
    }
}