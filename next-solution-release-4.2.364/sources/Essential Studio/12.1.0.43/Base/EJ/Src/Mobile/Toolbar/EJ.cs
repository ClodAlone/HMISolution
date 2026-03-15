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
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public partial class EssentialJavaScriptMobile
    {
        /// <summary>
        /// Toolbars the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MobileToolbarPropertiesBuilder Toolbar(String id)
        {
            var model = new MobileToolbarProperties();
            var mToolbar = new Toolbar(id, model);
            return new MobileToolbarPropertiesBuilder(mToolbar);
        }
        public MobileToolbarPropertiesBuilder Toolbar(String id, MobileToolbarProperties model)
        { 
            var mToolbar = new Toolbar(id, model);
            return new MobileToolbarPropertiesBuilder(mToolbar);
        }
    }
}
