#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using System.Linq;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Shapes;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Media;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for ChartSeries3D
    /// </summary>
    public abstract class ChartSeries3D : ChartSeriesBase
    {

        #region filesds

        internal int PrevSelectedIndex = -1;

        Brush prevSegmentBrush;

        bool dragged;

        object mouseUnderObject;

#endregion

        #region Properties

        /// <summary>
        /// Gets or sets the segment selection brush.
        /// </summary>
        /// <value>
        /// The segment selection brush.
        /// </value>
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { SetValue(SegmentSelectionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SegmentSelectionBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(ChartSeries3D), new PropertyMetadata(null,OnSegmentSelectionBrush));

        

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs args)
        { 
        
        }

        /// <summary>
        /// Get or Set Area property 
        /// </summary>
        internal SfChart3D Area
        {
            get { return ActualArea as SfChart3D; }
            set { ActualArea = value; }
        }

        /// <summary>
        /// Gets or Sets ChartAdornmentInfo. This allows us to customize the appearance of a data point by displaying labels, shapes and connector lines.
        /// </summary>
        /// <value>
        /// The <see cref="ChartAdornmentInfo" /> value.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public ChartAdornmentInfo3D AdornmentsInfo
        {
            get
            {
                return (ChartAdornmentInfo3D)GetValue(AdornmentsInfoProperty);
            }

            set
            {
                SetValue(AdornmentsInfoProperty, value);
            }
        }

        /// <summary>
        /// Identifies the AdornmentsInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty AdornmentsInfoProperty =
          DependencyProperty.Register("AdornmentsInfo", typeof(ChartAdornmentInfo3D), typeof(ChartSeries3D), new PropertyMetadata(null, OnAdornmentsInfoChanged));

        private static void OnAdornmentsInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as ChartSeries3D;

            if (e.OldValue != null)
            {
                var adornmentInfo = e.OldValue as ChartAdornmentInfoBase;
                if (series != null) series.Adornments.Clear();

                if (adornmentInfo != null)
                {
                    adornmentInfo.ClearChildren();
                    adornmentInfo.Series = null;
                }
            }

            if (e.NewValue == null) return;
            if (series == null) return;
            series.adornmentInfo = e.NewValue as ChartAdornmentInfoBase;
            series.AdornmentsInfo.Series = series;
            if (series.Area == null || series.AdornmentsInfo == null) return;
            Panel panel = series.AdornmentPresenter;
            if (panel == null) return;
            series.AdornmentsInfo.PanelChanged(panel);
            series.Area.ScheduleUpdate();
        }

        #endregion

        #region Methods

        #if WINDOWS_PHONE
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
#else
        /// <summary>
        /// Invoke to render 3D series
        /// </summary>
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            if (Area != null && AdornmentsInfo != null)
            {
                AdornmentsInfo.PanelChanged(null);
            }

        }

        /// <summary>
        /// Called when [series mouse down].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pos">The position.</param>
        protected internal virtual void OnSeriesMouseDown(object source, Point pos)
        {
            dragged = false;
            mouseUnderObject = source;
        }
        /// <summary>
        /// Called when [series mouse up].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pos">The position.</param>
        protected internal virtual void OnSeriesMouseUp(object source, Point pos)
        {
            ApplySelection(source as FrameworkElement);
        }

        internal void ApplySelection(FrameworkElement element)
        {
            if (element == null || !(element.Tag is ChartSegment3D)) return;
            var segment = element.Tag as ChartSegment3D;
            if (SegmentSelectionBrush != null && element == mouseUnderObject && !dragged)
            {
                var segmentIndex = Segments.IndexOf(segment);
                segment.Interior = SegmentSelectionBrush;
                
              
                if (PrevSelectedIndex >= 0 && prevSegmentBrush != null)
                {
                    var prevSegment = Segments[PrevSelectedIndex] as ChartSegment3D;
                   
                    prevSegment.Interior = prevSegmentBrush;
                    foreach (var item in prevSegment.Polygons)
                    {
                        item.Fill = prevSegmentBrush;
                        item.ReDraw();
                    }
                    if (prevSegment == segment)
                    {
                        PrevSelectedIndex = -1;
                        return;
                    }
                }
                prevSegmentBrush = segment.Polygons[0].Fill;
                foreach (var item in segment.Polygons)
                {
                    item.Fill = SegmentSelectionBrush;
                    item.ReDraw();
                }
                PrevSelectedIndex = segmentIndex;
            }
              Area.OnSelectionChanged(new ChartSelectionChangedEventArgs { SelectedSegment = segment, SelectedSeries = this });
        }

        /// <summary>
        /// Called when [series mouse move].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pos">The position.</param>
        protected internal virtual void OnSeriesMouseMove(object source, Point pos)
        {
            dragged = true;
            UpdateTooltip(source, pos);
        }

        /// <summary>
        /// Method implementation for Create Adornments
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="xVal">The x value.</param>
        /// <param name="yVal">The y value.</param>
        /// <param name="xPos">The x position.</param>
        /// <param name="yPos">The y position.</param>
        /// <param name="startDepth">The start depth.</param>
        /// <returns></returns>
        protected virtual ChartAdornment CreateAdornment(ChartSeriesBase series, double xVal, double yVal, double xPos, double yPos, double startDepth)
        {
            return new ChartAdornment3D(xVal, yVal, xPos, yPos,startDepth, series);
        }
        /// <summary>
        /// Method implementation for Add ColumnAdornments in Chart
        /// </summary>
        /// <param name="values"></param>
        protected virtual void AddColumnAdornments(params double[] values)
        {
            //values[0] -->   xData
            //values[1] -->   yData
            //values[2] -->   xPos
            //values[3] -->   yPos
            //values[4] -->   data point index
            //values[5] -->   Median value.

            double adornposX = values[2] + values[5], adornposY = values[3];
            var pointIndex = (int)values[4];

            if (pointIndex < Adornments.Count)
            {
                Adornments[pointIndex].SetData(values[0], values[1], adornposX, adornposY);
            }
            else
            {
                Adornments.Add(CreateAdornment(this, values[0], values[1], adornposX, adornposY, values[6]));
            }
            Adornments[pointIndex].Item = ActualData[pointIndex];
        }

        /// <summary>
        /// Method implementation for Add Adornments at XY
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pointindex"></param>
        /// <param name="startDepth"></param>
        protected virtual void AddAdornmentAtXY(double x, double y, int pointindex, double startDepth)
        {
            double adornposX = x, adornposY = y;

            if (pointindex < Adornments.Count)
            {
                Adornments[pointindex].SetData(x, y, adornposX, adornposY);
            }
            else
            {
                Adornments.Add(CreateAdornment(this, x, y, adornposX, adornposY, startDepth));
            }
        }

        /// <summary>
        /// Updates the on series bound changed.
        /// </summary>
        /// <param name="size">The size.</param>
        internal override void UpdateOnSeriesBoundChanged(Size size)
        {
            if (AdornmentsInfo != null)
            {
                AdornmentsInfo.UpdateElements();
                AdornmentsInfo.Measure(size, null);
            }

            var canUpdate = !(this is ISupportAxes) || this is ISupportAxes && ActualXAxis != null && ActualYAxis != null;

            if (!canUpdate) return;
            var chartTransformer = CreateTransformer(size, true);
            foreach (var segment in Segments)
            {
                segment.CreateSegmentVisual(size);
                segment.Update(chartTransformer);
            }
        }

        /// <summary>
        /// Called when DataSource property changed 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (AdornmentsInfo != null)
            {
                Adornments.Clear();
                AdornmentsInfo.UpdateElements();
            }
            base.OnDataSourceChanged(oldValue, newValue);
        }
        /// <summary>
        /// Method implementation for Clear Unused Adornments
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        protected void ClearUnUsedAdornments(int startIndex)
        {
            if (Adornments.Count <= startIndex) return;
            var count = Adornments.Count;

            for (var i = startIndex; i < count; i++)
            {
                Adornments.RemoveAt(startIndex);
            }
        }

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            if (AdornmentsInfo != null)
                ((ChartSeries3D)obj).AdornmentsInfo = (ChartAdornmentInfo3D)AdornmentsInfo.Clone();
            return base.CloneSeries(obj);
        }

        #endregion
    }
}
