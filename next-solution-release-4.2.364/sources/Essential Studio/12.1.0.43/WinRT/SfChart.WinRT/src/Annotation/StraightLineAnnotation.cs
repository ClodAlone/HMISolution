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
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class StraightLineAnnotation : LineAnnotation
    {
        internal AxisMarker AxisMarkerObject { get; set; }

        SfChart chart;
        
        internal override SfChart Chart
        {
            get { return chart; }
            set
            {
                if (chart != null)
                    chart.SizeChanged -= OnChartSizeChanged;
                chart = value;
                if (chart != null)
                    chart.SizeChanged += OnChartSizeChanged;
                SetAxisFromName();
            }
        }

        internal  override void OnVisibilityChanged(object visibility)
        {
            if (this.chart != null && this.chart.AnnotationManager != null)
            {
                this.IsVisbilityChanged = true;
                if (visibility.Equals(Visibility.Collapsed))
                    this.chart.AnnotationManager.AddOrRemoveAnnotations(this, true);
                else
                    this.chart.AnnotationManager.AddOrRemoveAnnotations(this, false);
            }
        }

        void OnChartSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.CoordinateUnit == Charts.CoordinateUnit.Pixel)
                UpdateAnnotation();
        }
        /// <summary>
        /// Gets or Sets Template For Axis Label View.
        /// </summary>
        public DataTemplate AxisLabelTemplate
        {
            get { return (DataTemplate)GetValue(AxisLabelTemplateProperty); }
            set { SetValue(AxisLabelTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Template. 
        public static readonly DependencyProperty AxisLabelTemplateProperty =
            DependencyProperty.Register("AxisLabelTemplate", typeof(DataTemplate), typeof(StraightLineAnnotation), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the ShowAxisLabel
        /// </summary>
        public bool ShowAxisLabel
        {
            get { return (bool)GetValue(ShowAxisLabelProperty); }
            set { SetValue(ShowAxisLabelProperty, value); }
        }

        /// <summary>
        /// The ShowAxisLabel property
        /// </summary>
        public static readonly DependencyProperty ShowAxisLabelProperty =
            DependencyProperty.Register("ShowAxisLabel", typeof(bool), typeof(StraightLineAnnotation), new PropertyMetadata(false,OnShowAxisLabelChanged));

        private static void OnShowAxisLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StraightLineAnnotation).OnShowAxisLabelChanged(Convert.ToBoolean(e.NewValue));
        }

        private void OnShowAxisLabelChanged(bool isShow)
        {
            if (isShow)
            {
                AxisMarkerObject = new AxisMarker();
                AxisMarkerObject.ParentAnnotation = this;
                UpdateAnnotation();
            }
            else if(AxisMarkerObject!=null)
            {
                Chart.AnnotationManager.AddOrRemoveAnnotations(AxisMarkerObject, true);
            }
        }
        
        protected void SetAxisMarkerValue(object X1, object X2, object Y1, object Y2, AxisMode axisMode)
        {
            AxisMarkerObject.X1 = X1;
            AxisMarkerObject.X2 = X2;
            AxisMarkerObject.Y1 = Y1;
            AxisMarkerObject.Y2 = Y2;
            AxisMarkerObject.XAxisName = this.XAxisName;
            AxisMarkerObject.YAxisName = this.YAxisName;
            AxisMarkerObject.YAxis = this.YAxis;
            AxisMarkerObject.XAxis = this.XAxis;
            AxisMarkerObject.DraggingMode = axisMode;
        }
    }
}
