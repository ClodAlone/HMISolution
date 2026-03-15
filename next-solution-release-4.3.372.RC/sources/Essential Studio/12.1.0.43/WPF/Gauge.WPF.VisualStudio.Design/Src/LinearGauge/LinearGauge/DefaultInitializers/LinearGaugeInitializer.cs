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

namespace Syncfusion.Gauge.WPF.VisualStudio.Design
{
    internal class LinearGaugeInitializer : DefaultInitializer
    {
        public LinearGaugeInitializer()
        {
        }

        public override void InitializeDefaults( ModelItem item )
        {
            using( ModelEditingScope scope = item.BeginEdit( ) )
            {
               LinearScale  m_scale = new LinearScale();
               m_scale.ShadowOffset = 0;
               m_scale.Minimum = 0;
               m_scale.Maximum = 100;
               m_scale.MinorIntervalValue = 2;
               m_scale.MajorIntervalValue = 10;
               m_scale.ScaleBarSize = 15;
               m_scale.ScaleBarLength = 220;
               m_scale.BorderWidth = 3;
               m_scale.ScaleStyle = LinearScaleStyle.Rectangle;
               m_scale.Name = "linearScale1";
               //m_scale.BorderBrush = Brushes.PeachPuff;
               //m_scale.BackgroundBrush = Brushes.Orange;

               LinearMarkTick majorTick = new LinearMarkTick();
               majorTick.TickWidth = 2;
               majorTick.TickHeight = 6;
               majorTick.TickPlacement = ScalePlacement.Inside;
               majorTick.TickStyle = TickStyle.MajorTick;
               //majorTick.BackgroundBrush = Brushes.Green;
               majorTick.TickShape = TickShape.Rectangle;
               majorTick.Name = "linearTick2";

               LinearMarkTick minorTick = new LinearMarkTick();
               minorTick.TickWidth = 2;
               minorTick.TickHeight = 2;
               minorTick.TickStyle = TickStyle.MinorTick;
               minorTick.TickPlacement = ScalePlacement.Inside;
               minorTick.TickShape = TickShape.RoundedRectangle;
               minorTick.Name = "linearTick1";
               //minorTick.BackgroundBrush = new SolidColorBrush(Color.FromRgb(0, 59, 137));

               LinearLabelTick majorLabelTick = new LinearLabelTick();
               majorLabelTick.FontSize = 11;
               majorLabelTick.TickStyle = TickStyle.MajorTick;
               //majorLabelTick.BackgroundBrush = Brushes.Wheat;
               majorLabelTick.TickPlacement = ScalePlacement.Inside;
               majorLabelTick.DistanceFromScale = 5;
               majorLabelTick.Name = "linearTick3";
            
               m_scale.Ticks.Add(minorTick);
               m_scale.Ticks.Add(majorTick);
               m_scale.Ticks.Add(majorLabelTick);

               LinearRange range = new LinearRange();
               range.StartValue = 65;
               range.EndValue = 100;
               range.StartWidth = 2;
               range.EndWidth = 10;
               range.RangePosition = ScalePlacement.Inside;
               //range.BackgroundBrush = Brushes.Yellow;
               //range.BorderBrush = new LinearGradientBrush(Color.FromRgb(46, 94, 160), Colors.DarkRed, 90d);
               range.BorderWidth = 0.5;
               range.Name = "linearRange1";
               m_scale.Ranges.Add(range);


               LinearBarPointer pointer1 = new LinearBarPointer();
               pointer1.PointerWidth = 20;
               //  pointer1.BackgroundBrush = Brushes.LemonChiffon;
               pointer1.Name = "linearPointer1";


               LinearMarkerPointer pointer2 = new LinearMarkerPointer();
               pointer2.MarkerStyle = MarkerStyle.Triangle;
               pointer2.PointerLength = 30;
               pointer2.PointerWidth = 20;
               pointer2.BorderWidth = 1;
               pointer2.Name = "linearPointer2";

               
               m_scale.Pointers.Add(pointer1);
               m_scale.Pointers.Add(pointer2);

               StateIndicator m_indicator = new StateIndicator();
               m_indicator.IndicatorStyle = IndicatorStyle.RectangularLED;
               m_indicator.StateRanges.Add(new StateRange(10, 20,m_indicator.ActiveBackgroundBrush));
               m_indicator.StateRanges.Add(new StateRange(70, 100,m_indicator.ActiveBackgroundBrush));
               //   m_indicator.BackgroundBrush = Brushes.Lavender;
               //  m_indicator.ActiveBackgroundBrush = Brushes.DarkGreen;
               //m_indicator.ActiveBorderBrush = Brushes.Red;
              
               m_indicator.IndicatorWidth = 20;
               m_indicator.IndicatorHeight = 20;
               m_indicator.Location = new Point(50, 93);
                m_indicator.Name="stateIndicator1";

               item.Properties["StateIndicators"].Collection.Add(m_indicator);
               item.Properties["Scales"].Collection.Add(m_scale);

               scope.Complete();
            }
        }
    }
}
