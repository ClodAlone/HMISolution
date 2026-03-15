#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Syncfusion.Gauge.Silverlight.VisualStudio.Design.Infrastructure;
using Syncfusion.Windows.Gauge;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Gauge.Silverlight.VisualStudio.Design
{
    internal class DigitalGaugeInitializer : DefaultInitializer
    {
        public DigitalGaugeInitializer()
            : base() 
        {
        }

        public override void InitializeDefaults(ModelItem item, EditingContext context) 
        {
            Utils.SparseSetValue(item.Properties["Width"], 350d);
            Utils.SparseSetValue(item.Properties["Height"], 150d);
            Utils.SparseSetValue(item.Properties["Value"], "Syncfusion");
            Utils.SparseSetValue(item.Properties["CharacterHeight"], 30d);
            Utils.SparseSetValue(item.Properties["CharacterCount"], 10);
            Utils.SparseSetValue(item.Properties["CharacterSpacing"], 2d);

            item.Properties["CharacterType"].SetValue(CharacterType.SegmentFourteen);
           

        }
    }
}
