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
using Microsoft.Windows.Design.Model;
using System.Windows.Media;

namespace Syncfusion.Gauge.WPF.VisualStudio.Design
{
    public class RollingGaugeInitializer : DefaultInitializer
    {
        public RollingGaugeInitializer()
        {
        }

        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                item.Properties["SegmentCount"].SetValue(4);
                item.Properties["SegmentForeground"].SetValue(new SolidColorBrush(Colors.Blue));
                item.Properties["Value"].SetValue("1000");
                item.Properties["Unit"].SetValue("KM");
                scope.Complete();
            }
        }
    }
}
