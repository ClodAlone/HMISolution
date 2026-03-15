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
        /// Extension for the Rotator Control.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder Rotator(String id)
        {
            var model = new MobileRotatorProperties();
            var rotator = new Rotator(id, model);
            return new MobileRotatorPropertiesBuilder(rotator);
        }
        public MobileRotatorPropertiesBuilder Rotator(String id, MobileRotatorProperties model)
        { 
            var rotator = new Rotator(id, model);
            return new MobileRotatorPropertiesBuilder(rotator);
        }
    }
}
