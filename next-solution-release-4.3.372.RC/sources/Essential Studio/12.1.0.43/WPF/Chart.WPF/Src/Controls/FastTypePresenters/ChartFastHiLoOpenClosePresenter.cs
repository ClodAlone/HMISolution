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
using System.Collections.ObjectModel;
using System.Windows.Media;
namespace Syncfusion.Windows.Chart
{
    
    /// <summary>
    /// Class implementation for ChartFastHiLoOpenClosePresenter
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartFastHiLoOpenClosePresenter : ChartFastSeriesPresenter
    {
        int noOfPoint = 0;

        /// <summary>
        /// Called when instance created for ChartFastHiLoOpenClosePresenter
        /// </summary>
        public ChartFastHiLoOpenClosePresenter()
        {
            this.SizeChanged += new SizeChangedEventHandler(ChartFastHiLoOpenClosePresenter_SizeChanged);
        }

        void ChartFastHiLoOpenClosePresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (this.PointView.Count != noOfPoint)
            //{
            //    this.RenderPoints(true);
            //}

            this.AffectRender = true;
        }

            /// <summary>
            /// Virtual method created for OnDrawingPointsChanged
            /// </summary>
            /// <param name="args"></param>
            protected override void OnDrawingPointsChanged(DependencyPropertyChangedEventArgs args)
        {
            base.OnDrawingPointsChanged(args);
            if (this.PointView != null && this.VisualCollection.Count != 0)
            {
                this.RenderPoints(false);
            }
        }

        internal void RenderPoints(bool redraw)
        {
            if (this.AffectRender == false)
                return;
  if (this.SegmentInteriorList == null || (this.SegmentInteriorList != null && this.SegmentInteriorList.IsUpdated == false))
            {
                return;
            }


            if (redraw == false && this.VisualChildrenCount != this.PointView.Count)
            {
                for (int i = 0; i < this.PointView.Count; i++)
                {
                    this.VisualCollection.Add(new DrawingVisual());
                    //   redraw = false;
                }

            }

            if (this.Interior.CanFreeze)
            {
                this.Interior.Freeze();
            }

            if (this.Stroke.CanFreeze)
            {
                this.Stroke.Freeze();
            }

            for (int i = 0; i < this.PointView.Count; i++)
            {
                DrawingVisual visual;
                if (redraw)
                {
                    visual = new DrawingVisual();
                }
                else
                {
                    visual = this.VisualCollection[i] as DrawingVisual;
                }
                HiLoOpenCloseDrawingValues values = this.PointView.GetItemAt(i) as HiLoOpenCloseDrawingValues;
                ChartFastSeriesPresenter.SetIndex(visual, i);
                ChartSegment segment = this.DataContext as ChartSegment;
                using (DrawingContext context = visual.RenderOpen())
                {
                    if (this.SegmentInteriorList.IsUpdated == true && this.SegmentInteriorList.DataCount == 0)// || (this.SegmentInteriorList != null && this.SegmentInteriorList.Count == 0))
                    {
                        context.DrawLine(new Pen(this.Interior, this.StrokeThickness), values.StartOpenPoint, values.EndOpenPoint);
                        context.DrawLine(new Pen(this.Interior, this.StrokeThickness), values.StartClosePoint, values.EndClosePoint);
                        context.DrawLine(new Pen(this.Interior, this.StrokeThickness), values.HighPoint, values.LowPoint);
                    }
                    else if (this.SegmentInteriorList.Count != 0)
                    {
                        if (this.SegmentInteriorList[i].Interior == null)
                        {
                            this.SegmentInteriorList[i].Interior = this.Interior;
                        }
                        if (this.SegmentInteriorList[i].Stroke == null)
                        {
                            this.SegmentInteriorList[i].Stroke = this.Stroke;
                        }
                        if (Double.IsNaN(this.SegmentInteriorList[i].StrokeThickness))
                        {
                            this.SegmentInteriorList[i].StrokeThickness = this.StrokeThickness;
                        }

                        context.DrawLine(new Pen(this.SegmentInteriorList[i].Interior, this.SegmentInteriorList[i].StrokeThickness), values.StartOpenPoint, values.EndOpenPoint);
                        context.DrawLine(new Pen(this.SegmentInteriorList[i].Interior, this.SegmentInteriorList[i].StrokeThickness), values.StartClosePoint, values.EndClosePoint);
                        context.DrawLine(new Pen(this.SegmentInteriorList[i].Interior, this.SegmentInteriorList[i].StrokeThickness), values.HighPoint, values.LowPoint);
                    }

                }
                if (redraw)
                {
                    VisualCollection.Add(visual);
                }
            }

            if (redraw)
            {
                noOfPoint = this.PointView.Count;
            }
        }


            /// <summary>
            /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
            /// </summary>
            /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
            protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.PointView != null && this.PointView.Count > 0)
            {
                ChartFastHiLoOpenCloseSegment segment = null;
                if (this.DataContext != null)
                {
                    segment = this.DataContext as ChartFastHiLoOpenCloseSegment;
                    IChartTransformer transform = ChartTransform.CreateTransformer(ChartAxesType.CartesianAxes, new Rect(this.RenderSize), segment.Series);
                    segment.Update(transform);
                    this.AffectRender = segment.affectRender;
                }
                if (this.PointView.Count != noOfPoint)
                {
                    this.RenderPoints(true);
                   // noOfPoint = this.PointView.Count;
                }
                else
                {
                    this.RenderPoints(false);
                }
                if (segment != null)
                {
                    segment.affectRender = false;
                }

            }
        }
    }
        /// <summary>
        /// Class implementation for HiLoOpenCloseDrawingValues
        /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class HiLoOpenCloseDrawingValues : DependencyObject
    {
        /// <summary>
        /// Identifies the StartOpenPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty StartOpenPointProperty =
          DependencyProperty.Register("StartOpenPoint", typeof(Point), typeof(HiLoOpenCloseDrawingValues), new PropertyMetadata(null));

        /// <summary>
        /// Get or Sets the StartOpenPointProperty
        /// </summary>
        public Point StartOpenPoint
        {
            set { SetValue(StartOpenPointProperty, value); }
            get { return (Point)GetValue(StartOpenPointProperty); }
        }

        /// <summary>
        /// Identifies the EndOpenPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty EndOpenPointProperty =
            DependencyProperty.Register("EndOpenPoint", typeof(Point), typeof(HiLoOpenCloseDrawingValues), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the EndOpenPointProperty
        /// </summary>
        public Point EndOpenPoint
        {
            set { SetValue(EndOpenPointProperty, value); }
            get { return (Point)GetValue(EndOpenPointProperty); }
        }

        /// <summary>
        /// Identifies the StartClosePoint dependency property.
        /// </summary>
        public static readonly DependencyProperty StartClosePointProperty =
            DependencyProperty.Register("StartClosePoint", typeof(Point), typeof(HiLoOpenCloseDrawingValues), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the StartClosePointProperty
        /// </summary>
        public Point StartClosePoint
        {
            set { SetValue(StartClosePointProperty, value); }
            get { return (Point)GetValue(StartClosePointProperty); }
        }

        /// <summary>
        ///  Identifies the EndClosepoint dependency property.
        /// </summary>
        public static readonly DependencyProperty EndClosePointProperty =
            DependencyProperty.Register("EndClosePoint", typeof(Point), typeof(HiLoOpenCloseDrawingValues), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set EndClosePointProperty
        /// </summary>
        public Point EndClosePoint
        {
            set { SetValue(EndClosePointProperty, value); }
            get { return (Point)GetValue(EndClosePointProperty); }
        }

        /// <summary>
        /// Identifies the HighPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty HighPointProperty =
            DependencyProperty.Register("HighPoint", typeof(Point), typeof(HiLoOpenCloseDrawingValues), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the HighPointProperty
        /// </summary>
        public Point HighPoint
        {
            set { SetValue(HighPointProperty, value); }
            get { return (Point)GetValue(HighPointProperty); }
        }

        /// <summary>
        /// Identifies the LowPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty LowPointProperty =
            DependencyProperty.Register("LowPoint", typeof(Point), typeof(HiLoOpenCloseDrawingValues), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the LowPointProperty
        /// </summary>
        public Point LowPoint
        {
            set { SetValue(LowPointProperty, value); }
            get { return (Point)GetValue(LowPointProperty); }
        }

    }

    /// <summary>
        /// Class implementation for HiLoOpenCloseChartDrawingValuesCollection
    /// </summary>
    public class HiLoOpenCloseChartDrawingValuesCollection : ObservableCollection<HiLoOpenCloseDrawingValues>
    {
    }

}
