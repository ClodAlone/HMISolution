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
using Syncfusion.Gauge.Silverlight.Expression.Design.Infrastructure;
using Syncfusion.Windows.Gauge;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Gauge.Silverlight.Expression.Design
{
    internal class CircularGaugeInitializer : DefaultInitializer
    {
        public CircularGaugeInitializer()
            : base() 
        {
        }

        public override void InitializeDefaults(ModelItem item, EditingContext context) 
        {
            Utils.SparseSetValue(item.Properties["Width"], 300d);
            Utils.SparseSetValue(item.Properties["Height"], 300d);
            Utils.SparseSetValue(item.Properties["Radius"], 150d);
           
            CircularScale circularscale = new CircularScale();
            circularscale.ShadowOffset = 1;
            circularscale.Minimum = 0;
            circularscale.Maximum = 100;
            circularscale.MinorIntervalValue = 2;
            circularscale.MajorIntervalValue = 10;
            circularscale.StartAngle = 120;
            circularscale.GapSweepAngle = 300;
            circularscale.MidIntervalValue = 5;
            circularscale.ScaleBarSize = 5;
            LabelTickSet labeltick = new LabelTickSet();
            labeltick.TickPlacement = ScalePlacement.Inside;
            labeltick.TickStyle = TickStyle.MajorTick;
            labeltick.DistanceFromScale = 1;
            labeltick.FontSize = 12;
            circularscale.Ticks.Add(labeltick);

            // Minor Tick properties 
            MarkTickSet minorTick = new MarkTickSet();
            minorTick.TickWidth = 2;
            minorTick.TickHeight = 5;
            minorTick.TickStyle = TickStyle.MinorTick;
            minorTick.TickPlacement = ScalePlacement.Cross;
            circularscale.Ticks.Add(minorTick);

            // Major Ticks properties
            MarkTickSet majorTick = new MarkTickSet();
            majorTick.TickWidth = 5;
            majorTick.TickHeight = 7;
            majorTick.TickStyle = TickStyle.MajorTick;
            circularscale.Ticks.Add(majorTick);

            // Major Ticks properties
            MarkTickSet midTick = new MarkTickSet();
            midTick.TickWidth = 3;
            midTick.TickHeight = 6;
            midTick.TickStyle = TickStyle.MidTick;
            circularscale.Ticks.Add(midTick);

            // PointerCap properties
            circularscale.PointerCap.PointerCapRadius = 15;
            

            // Pointer for  Scale
            CircularPointer circularpointer = new CircularPointer();
            circularpointer.PointerNeedleType = PointerNeedleType.Needle;
            circularpointer.PointerWidth = 10;
            circularpointer.PointerPlacement = ScalePlacement.Inside;
            circularpointer.NeedleStyle = NeedleStyle.Needle;
            circularscale.Pointers.Add(circularpointer);
            item.Properties["Scales"].Collection.Add(circularscale);            
            
            

        }
    }
}
