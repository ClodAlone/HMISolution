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
using Syncfusion.Windows.Gauge;
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Gauge.WPF.Expression.Design
{
    internal class DigitalGaugeInitializer : DefaultInitializer
    {
        public DigitalGaugeInitializer()
        {
        }

        public override void InitializeDefaults( ModelItem item )
        {
            using( ModelEditingScope scope = item.BeginEdit( ) )
            {
                item.Properties["CharacterCount"].SetValue(10);
                item.Properties["CharacterType"].SetValue(CharacterType.SegmentFourteen);
                item.Properties["Value"].SetValue("Syncfusion");
                item.Properties["CharacterHeight"].SetValue(50d);
                item.Properties["CharacterSpacing"].SetValue(6d);
                item.Properties["SegmentSpacing"].SetValue(1.5d);
                item.Properties["SegmentWidth"].SetValue(2.5d);
                item.Properties["Foreground"].SetValue(Brushes.Black);
                item.Properties["Height"].SetValue(75d);
                item.Properties["Width"].SetValue(450d);
                DigitalGauge dd = new DigitalGauge();
       
                scope.Complete();

            }
        }
    }
}
