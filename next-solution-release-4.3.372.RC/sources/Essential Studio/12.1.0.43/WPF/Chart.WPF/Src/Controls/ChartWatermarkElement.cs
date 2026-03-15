// <copyright file="ChartWatermarkElement.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls;

    /// <summary>
    /// Renders the chart watermark.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartWatermarkElement : FrameworkElement, IDisposable
    {
        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for XAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
          DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(ChartWatermarkElement), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxisChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for YAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
          DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(ChartWatermarkElement), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxisChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for watermark.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(Brush), typeof(ChartWatermarkElement), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Properties
    
        /// <summary>
        /// Gets or sets the X axis.
        /// </summary>
        /// <value>The X axis.</value>
        public ChartAxis XAxis
        {
            get
            {
                return (ChartAxis)GetValue(XAxisProperty);
            }

            set
            {
                SetValue(XAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y axis.
        /// </summary>
        /// <value>The Y axis.</value>
        public ChartAxis YAxis
        {
            get
            {
                return (ChartAxis)GetValue(YAxisProperty);
            }

            set
            {
                SetValue(YAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the watermark.
        /// </summary>
        /// <value>The Watermark.</value>
        public Brush Watermark
        {
            get
            {
                return (Brush)GetValue(WatermarkProperty);
            }

            set
            {
                SetValue(WatermarkProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            ChartAxis xAxis = this.XAxis;
            ChartAxis yAxis = this.YAxis;

            if (xAxis != null && yAxis != null)
            {
                Rect clientRect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

                #region Render watermark
                drawingContext.DrawRectangle(this.Watermark, null, clientRect);
                #endregion             
            }

            base.OnRender(drawingContext);
        }
     

        /// <summary>
        /// Called when axes is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAxisChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        /// <summary>
        /// Called when axes is changed.
        /// </summary>
        /// <param name="dObj">The d obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAxisChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            ChartWatermarkElement watermark = dObj as ChartWatermarkElement;

            if (watermark != null)
            {
                if (args.OldValue != null)
                {
                    (args.OldValue as ChartAxis).Changed -= new EventHandler(watermark.OnAxisChanged);
                }

                if (args.NewValue != null)
                {
                    (args.NewValue as ChartAxis).Changed += new EventHandler(watermark.OnAxisChanged);
                }
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (this.XAxis != null && this.YAxis != null)
            {
                this.XAxis.Dispose();
                this.YAxis.Dispose();
            }
            this.Watermark = null;
        }

        #endregion
    }
}
