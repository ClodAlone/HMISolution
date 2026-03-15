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
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public partial class EssentialJavaScriptMobile
    {
        /// <summary>
        /// Extension for Tab control.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder Tab(String id)
        {
            var model = new MobileTabProperties();
            var mTab = new Tab(id, model);
            return new MobileTabPropertiesBuilder(mTab);
        }
        public MobileTabPropertiesBuilder Tab(String id, MobileTabProperties model)
        { 
            var mTab = new Tab(id, model);
            return new MobileTabPropertiesBuilder(mTab);
        }
    }
}
