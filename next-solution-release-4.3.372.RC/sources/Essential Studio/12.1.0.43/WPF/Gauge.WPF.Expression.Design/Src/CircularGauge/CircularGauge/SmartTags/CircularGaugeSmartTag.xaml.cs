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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Design;
using Syncfusion.Windows.Gauge;

namespace Syncfusion.Gauge.WPF.Expression.Design
{
	/// <summary>
	/// Interaction logic for GroupBarSmartTag.xaml
	/// </summary>
	public partial class CircularGaugeSmartTag : SmartTagBase
	{
        GetChildCount child = new GetChildCount();
		public CircularGaugeSmartTag()
		{
			InitializeComponent();

		}

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(txtName, "Name");
            BindSelectorWithBrushes(comboFirstFrameFillColor, "FirstFrameFillColor");
            BindThickness(txtFirstFrameThickness, "FirstFrameThickness");
            BindSelectorWithBrushes(comboSecondFrameFillColor, "SecondFrameFillColor");
            BindThickness(txtSecondFrameThickness, "SecondFrameThickness");
            BindSelectorWithBrushes(comboCenterFrameColorSelector, "CenterFrameFillColor");
            BindNumeric(txtRadius, "Radius");            
            BindNumeric(txtHalfCircleInnerRadius, "HalfCircleInnerRadius");
            BindSelectorWithEnum(comboHalfCircleInnerSweepDirection, "HalfCircleInnerSweepDirection", typeof(SweepDirection));
            BindSelectorWithEnum(comboHalfCircleSweepDirection, "HalfCircleSweepDirection", typeof(SweepDirection));
            BindSelectorWithEnum(comboFrameType, "FrameType", typeof(GaugeFrameType));
        }

        private new void AddChildControl(object sender, RoutedEventArgs e)
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
            m_scale.BorderBrush = Brushes.PeachPuff;

            m_scale.BackgroundBrush = Brushes.Orange;
            m_scale.ShadowOffset = 2.5;
          
            m_scale.Name = "circularScale" + child.GetSuffix("circularScale","Scales",this.ModelItem);
            this.ModelItem.Properties["Scales"].Collection.Add(m_scale);
        }

        

        private void AddStateIndicators(object sender, RoutedEventArgs e)
        {
            StateIndicator si = new StateIndicator();
            si.Name = "stateIndicator" + child.GetSuffix("stateIndicator", "StateIndicators", this.ModelItem);
            this.ModelItem.Properties["StateIndicators"].Collection.Add(si);
        }
	}
}
