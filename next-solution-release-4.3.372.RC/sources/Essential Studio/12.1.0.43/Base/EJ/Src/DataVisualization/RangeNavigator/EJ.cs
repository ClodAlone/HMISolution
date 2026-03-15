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
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript
{
    public partial class EssentialJavaScript
    {
        public RangeNavigatorPropertiesBuilder RangeNavigator(string id)
        {
            var model = new RangeNavigatorProperties();
            var rangeNavigator = new RangeNavigator(id, model);
            return new RangeNavigatorPropertiesBuilder(rangeNavigator);
        }
        public RangeNavigator RangeNavigator(String id, RangeNavigatorProperties model)
        {
            var rangeNavigator = new RangeNavigator(id, model);
            return rangeNavigator;
        }
    }
}
