#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media.Animation;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Data;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class SfColumnSparkline : ColumnBase
    {
        #region fields

        Rectangle rectSegment;

        Line axisLine;

        #endregion

        #region properties

        /// <summary>
        /// Gets or Sets the property path of the x data in ItemsSource.
        /// </summary>
        public string XBindingPath
        {
            get { return (string)GetValue(XBindingPathProperty); }
            set { SetValue(XBindingPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XBindingPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XBindingPathProperty =
            DependencyProperty.Register("XBindingPath", typeof(string), typeof(SfColumnSparkline), new PropertyMetadata(String.Empty));

        /// <summary>
        /// Gets or sets the axis line style.
        /// </summary>
        public Style AxisStyle
        {
            get { return (Style)GetValue(AxisStyleProperty); }
            set { SetValue(AxisStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisStyleProperty =
            DependencyProperty.Register("AxisStyle", typeof(Style), typeof(SfColumnSparkline), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets show axis
        /// </summary>
        public bool ShowAxis
        {
            get { return (bool)GetValue(ShowAxisProperty); }
            set { SetValue(ShowAxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowAxisProperty =
            DependencyProperty.Register("ShowAxis", typeof(bool), typeof(SfColumnSparkline), new PropertyMetadata(false, OnShowAxisChanged));

        /// <summary>
        /// Gets or Sets axis origin
        /// </summary>
        public double AxisOrigin
        {
            get { return (double)GetValue(AxisOriginProperty); }
            set { SetValue(AxisOriginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisOrigin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisOriginProperty =
            DependencyProperty.Register("AxisOrigin", typeof(double), typeof(SfColumnSparkline), new PropertyMetadata(0d, OnAxisOriginChanged));

        /// <summary>
        /// Gets or sets the segment template selector to customize the each segments.
        /// </summary>
        public TemplateSelector SegmentTemplateSelector
        {
            get { return (TemplateSelector)GetValue(SegmentTemplateSelectorProperty); }
            set { SetValue(SegmentTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SegmentTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SegmentTemplateSelectorProperty =
            DependencyProperty.Register("SegmentTemplateSelector", typeof(TemplateSelector), typeof(SfColumnSparkline), new PropertyMetadata(null));

        #endregion

        #region ctor

        public SfColumnSparkline()
        {
            this.DefaultStyleKey = typeof(SfColumnSparkline);
        }

        #endregion

        #region methods

        protected override void GeneratePoints(string xPath)
        {
            base.GeneratePoints(XBindingPath);
        }

        protected override void SetIndividualPoints(int index, object obj, bool replace, string xPath)
        {
            base.SetIndividualPoints(index, obj, replace, XBindingPath);
        }

        protected void RemoveAxis()
        {
            if (RootPanel.Children.Contains(axisLine))
                RootPanel.Children.Remove(axisLine);
            axisLine = null;
        }

        protected virtual void UpdateHorizontalAxis()
        {
            if (RootPanel != null && ShowAxis)
            {
                if (axisLine == null)
                {
                    axisLine = new Line();
                    StyleBinding(axisLine, AxisStyle, "AxisStyle");
                    RootPanel.Children.Add(axisLine);
                }
                Point pos = TransformToVisible(0, AxisOrigin);
                axisLine.X1 = 0;
                axisLine.X2 = availableWidth;
                axisLine.Y1 = pos.Y;
                axisLine.Y2 = pos.Y;
            }
        }

        private static void OnShowAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
                (d as SfColumnSparkline).RemoveAxis();
            else
                (d as SfColumnSparkline).UpdateHorizontalAxis();
        }

        private static void OnAxisOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfColumnSparkline).UpdateHorizontalAxis();
        }

        protected override void UpdateMinMaxValues()
        {
            base.UpdateMinMaxValues();
            if (IsIndexed)
            {
                maxXValue += 0.8;
                deltaX = maxXValue - minXValue;
            }
        }

        protected override void RenderSegments()
        {
            if (yValues != null)
            {
                double sbsInfoStart, sbsInfoEnd;
                if (!IsIndexed)
                {
                    sbsInfoEnd = 0.4;
                    sbsInfoStart = -0.4;
                }
                else
                {
                    sbsInfoStart = 0;
                    sbsInfoEnd = 0.8;
                }

                if (ShowAxis)
                    UpdateHorizontalAxis();
                base.RenderSegments();
                if (SegmentTemplateSelector != null)
                    SegmentTemplateSelector.SetData(this, DataCount);
                Point point1, point2;
                Rect rect;
                int count = yValues.Count;
                double yVal = 0, xVal = 0;
                for (int i = 0; i < count; i++)
                {
                    xVal = xValues[i];
                    yVal = yValues[i];
                    if (!double.IsNaN(yVal))
                    {
                        point1 = TransformToVisible(xVal + sbsInfoStart, yVal);
                        point2 = TransformToVisible((xVal + sbsInfoEnd), 0);
                        rect = new Rect(point1, point2);
                        if (SegmentPresenter.Children.Count > i)
                        {
                            rectSegment = SegmentPresenter.Children[i] as Rectangle;
                        }
                        else
                        {
                            rectSegment = new Rectangle();
                            BindFillProperty(rectSegment, "Interior");
                            SegmentPresenter.Children.Add(rectSegment);
                            rectSegment.Tag = new object[] { "Selectable", xVal,yVal };
                        }
                        if (SegmentTemplateSelector != null)
                            (SegmentTemplateSelector as SegmentTemplateSelector).BindVisual(xVal, yVal, rectSegment);
                        rectSegment.Width = rect.Width;
                        rectSegment.SetValue(Canvas.LeftProperty, rect.X);
                        rectSegment.Height = rect.Height;
                        rectSegment.SetValue(Canvas.TopProperty, rect.Y);
                    }
                }
            }
        }

        #endregion
    }
}
