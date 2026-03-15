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
    internal class LinearGaugeInitializer : DefaultInitializer
    {
        public LinearGaugeInitializer()
            : base() 
        {
        }

        public override void InitializeDefaults(ModelItem item, EditingContext context) 
        {
            Utils.SparseSetValue(item.Properties["Width"], 150d);
            Utils.SparseSetValue(item.Properties["Height"], 300d);
            
            LinearScale Linearscale = new LinearScale();
            Linearscale.Minimum = 0;
            Linearscale.Maximum = 100;
            Linearscale.MinorIntervalValue = 2;
            Linearscale.ScaleBarLength = 220;
            Linearscale.ScaleBarSize = 15;
            Linearscale.MajorIntervalValue = 10;
            Linearscale.MidIntervalValue = 5;
            Linearscale.RadiusX = 0;
            Linearscale.RadiusY = 0;

            LabelTickSet labeltick = new LabelTickSet();
            labeltick.TickPlacement = ScalePlacement.Inside;
            labeltick.TickStyle = TickStyle.MajorTick;
            labeltick.DistanceFromScale = 1;
            labeltick.FontSize = 12;
            Linearscale.Ticks.Add(labeltick);

            // Minor Tick properties 
            MarkTickSet minorTick = new MarkTickSet();
            minorTick.TickWidth = 1;
            minorTick.TickHeight = 3;
            minorTick.TickStyle = TickStyle.MinorTick;
            minorTick.TickPlacement = ScalePlacement.Outside;
            Linearscale.Ticks.Add(minorTick);

            // Major Ticks properties
            MarkTickSet majorTick = new MarkTickSet();
            majorTick.TickWidth = 3;
            majorTick.TickHeight = 7;
            majorTick.TickStyle = TickStyle.MajorTick;
            majorTick.TickPlacement = ScalePlacement.Outside;
            Linearscale.Ticks.Add(majorTick);

            // Major Ticks properties
            MarkTickSet midTick = new MarkTickSet();
            midTick.TickWidth = 1;
            midTick.TickHeight = 5;
            midTick.TickStyle = TickStyle.MidTick;
            midTick.TickPlacement = ScalePlacement.Outside;
            Linearscale.Ticks.Add(midTick);

            
            // Pointer for  Scale
            LinearMarkerPointer linearpointer = new LinearMarkerPointer();
            linearpointer.PointerWidth = 20;
            linearpointer.PointerLength = 20;
            linearpointer.Value = 0;
            linearpointer.PointerPlacement = ScalePlacement.Inside;
            linearpointer.MarkerStyle = MarkerStyle.Triangle;
            Linearscale.Pointers.Add(linearpointer);

            item.Properties["Scales"].Collection.Add(Linearscale); 
            

        }
    }
}
