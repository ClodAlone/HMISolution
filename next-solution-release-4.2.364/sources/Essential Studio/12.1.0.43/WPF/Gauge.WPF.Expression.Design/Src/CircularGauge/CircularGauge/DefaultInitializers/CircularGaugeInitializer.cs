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
    internal class CircularGaugeInitializer : DefaultInitializer
    {
        public CircularGaugeInitializer()
        {
        }

        public override void InitializeDefaults( ModelItem item )
        {
            using( ModelEditingScope scope = item.BeginEdit( ) )
            {
                CircularScale m_scale = new CircularScale();
                m_scale.ShadowOffset = 1;
                m_scale.Minimum = 0;
                m_scale.Maximum = 100;
                m_scale.MinorIntervalValue = 2;
                m_scale.MajorIntervalValue = 10;
                m_scale.StartAngle = 120;
                m_scale.GapSweepAngle = 300;
                m_scale.ScaleBarSize = 10;
                m_scale.Radius = 130;
                m_scale.BorderWidth = 3;
                m_scale.Name = "circularScale1";
                              
                CircularLabelTick majorLabelTick = new CircularLabelTick();
                majorLabelTick.FontSize = 11;
                majorLabelTick.TickStyle = TickStyle.MajorTick;
                majorLabelTick.TickPlacement = ScalePlacement.Inside;
                majorLabelTick.DistanceFromScale = 5;
                majorLabelTick.Name = "circularTick3";

                CircularMarkTick majorTick = new CircularMarkTick();
                majorTick.TickWidth = 5;
                majorTick.TickHeight = 10;
                majorTick.TickStyle = TickStyle.MajorTick;
                majorTick.TickShape = TickShape.RoundedRectangle;
                majorTick.Name = "circularTick2";

                CircularMarkTick minorTick = new CircularMarkTick();
                minorTick.TickWidth = 1;
                minorTick.TickHeight = 4;
                minorTick.TickStyle = TickStyle.MinorTick;
                minorTick.Name = "circularTick1";


                m_scale.Ticks.Add(minorTick);
                m_scale.Ticks.Add(majorTick);
                m_scale.Ticks.Add(majorLabelTick);

                CircularRange range = new CircularRange();
                range.StartValue = 70;
                range.EndValue = 100;
                range.StartWidth = 2;
                range.EndWidth = 20;
                range.RangePosition = ScalePlacement.Inside;
                range.DistanceFromScale = 23;
                range.BorderWidth = 1;
                range.Name = "circularRange1";

                m_scale.Ranges.Add(range);


                m_scale.PointerCap.CapOnTop = true;
                m_scale.PointerCap.PointerCapRadius = 10;
                m_scale.PointerCap.PointerCapType = PointerCapType.Default;
                m_scale.PointerCap.Width = 20;
                m_scale.PointerCap.Name = "pointerCap1";

                CircularPointer pointer1 = new CircularPointer();

                pointer1.BorderWidth = 0.3;
                pointer1.PointerWidth = 20;

               // double dd = double.Parse(item.Properties["Radius"].Value.ToString());
                pointer1.PointerLength = 150 / 1.5;
                pointer1.HorizontalAlignment = HorizontalAlignment.Right;
                pointer1.PointerPlacement = ScalePlacement.Outside;
                pointer1.Name = "circularPointer1";

                m_scale.Pointers.Add(pointer1);
                StateIndicator m_indicator = new StateIndicator();
                m_indicator.IndicatorStyle = IndicatorStyle.Text;
                m_indicator.StateRanges.Add(new StateRange(10, 20,m_indicator.ActiveBackgroundBrush));
                m_indicator.StateRanges.Add(new StateRange(70, 100,m_indicator.ActiveBackgroundBrush));
                m_indicator.FontSize = 12;
                m_indicator.FontFamily = new FontFamily("Verdana");
                m_indicator.Text = "Off";
                m_indicator.ActiveText = "On";
                m_indicator.IndicatorWidth = 20;
                m_indicator.IndicatorHeight = 20;
                m_indicator.Location = new Point(50, 80);
                m_indicator.Name = "stateIndicator1";
                item.Properties["StateIndicators"].Collection.Add(m_indicator);

                item.Properties["Scales"].Collection.Add(m_scale);

                item.Properties["Height"].SetValue((double)300);
                item.Properties["Width"].SetValue((double)300);
                scope.Complete();
            }
        }
    }
}
