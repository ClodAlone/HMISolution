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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public partial class EssentialJavaScript
    {
        public ToolbarPropertiesBuilder Toolbar(string id)
        {
            var model = new ToolbarProperties();
            var toolbar = new Toolbar(id, model);
            return new ToolbarPropertiesBuilder(toolbar);
        }
        public Toolbar Toolbar(String id, ToolbarProperties model)
        {
            var toolbar = new Toolbar(id, model);
            return toolbar;
        }
    }
}
