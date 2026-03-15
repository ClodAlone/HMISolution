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
using Syncfusion.Gauge.Silverlight.VisualStudio.Design.Infrastructure;
using Syncfusion.Windows.Gauge;

namespace Syncfusion.Gauge.Silverlight.VisualStudio.Design
{
    internal class RollingGaugeInitializer:DefaultInitializer
    {
        public RollingGaugeInitializer()
            : base()
        {
            
        }

        public override void InitializeDefaults(Microsoft.Windows.Design.Model.ModelItem item, Microsoft.Windows.Design.EditingContext context)
        {
            Utils.SparseSetValue(item.Properties["SegmentCount"], 4);
            Utils.SparseSetValue(item.Properties["Unit"], "KM");
            Utils.SparseSetValue(item.Properties["Value"], "1000");
            Utils.SparseSetValue(item.Properties["Height"], "50");
            item.Properties["UnitPosition"].SetValue(UnitPosition.End);
        }
    }
}
