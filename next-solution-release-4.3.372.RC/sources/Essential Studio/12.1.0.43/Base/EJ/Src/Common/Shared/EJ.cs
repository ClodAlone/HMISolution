#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Syncfusion.JavaScript
{
    public partial class EssentialJavaScript
    {
        private static bool _unobtrusive = Utils.IsUnObtrusive();
        internal static bool UnObtrusive
        {
            get { return _unobtrusive; }
        }

        public HtmlString ScriptManager()
        {
            String content = "";
            if (!EssentialJavaScript.UnObtrusive)
            {
                 content = Syncfusion.JavaScript.ScriptManager.RenderScriptElement();
                Utils.IdJsonPair = new Dictionary<string, string>();              
            }
            return new HtmlString(content);
        }

        public OlapControls Olap()
        {
            return new OlapControls();
        }
    }
}
