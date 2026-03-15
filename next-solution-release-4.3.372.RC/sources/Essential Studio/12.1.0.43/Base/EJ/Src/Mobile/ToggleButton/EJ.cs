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
        /// Toggles the button.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MobileToggleButtonPropertiesBuilder ToggleButton(String id)
        {
            var model = new MobileToggleButtonProperties();
            var mToggleButton = new ToggleButton(id, model);
            return new MobileToggleButtonPropertiesBuilder(mToggleButton);
        }
        public MobileToggleButtonPropertiesBuilder ToggleButton(String id, MobileToggleButtonProperties model)
        { 
            var mToggleButton = new ToggleButton(id, model);
            return new MobileToggleButtonPropertiesBuilder(mToggleButton);
        }
    }
}
