using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace UFWebClient.HTML5
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            //GlobalHost.Configuration.DefaultMessageBufferSize = 100;
            app.MapSignalR();
        }
    }
}