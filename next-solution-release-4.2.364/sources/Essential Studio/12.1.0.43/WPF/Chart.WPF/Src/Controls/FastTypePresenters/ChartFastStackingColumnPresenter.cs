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
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;


namespace Syncfusion.Windows.Chart
{

    /// <summary>
    /// Class implementation for ChartFastStackingColumnPresenter
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartFastStackingColumnPresenter : ChartFastSeriesPresenter
    {

        int noOfPoint = 0;

/*
     private double viewPortwidth;
*/

/*
        private double viewPortHeight;
*/
        /// <summary>
        /// Called when instance created for ChartFastStackingColumnPresenter
        /// </summary>
        public ChartFastStackingColumnPresenter()
        {
            this.SizeChanged += new SizeChangedEventHandler(ChartFastStackingColumnPresenter_SizeChanged);
        }

        void ChartFastStackingColumnPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.AffectRender = true;
        }

            /// <summary>
            /// Virtual method created for OnDrawingPointsChanged
            /// </summary>
            /// <param name="args"></param>
            protected override void OnDrawingPointsChanged(DependencyPropertyChangedEventArgs args)
        {
            // this.AffectRender = true;
            base.OnDrawingPointsChanged(args);
            if (this.PointView != null)
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
                StackingColumnChartValues values = this.PointView.GetItemAt(i) as StackingColumnChartValues;
                ChartFastSeriesPresenter.SetIndex(visual, i);
                ChartSegment segment = this.DataContext as ChartSegment;
                using (DrawingContext context = visual.RenderOpen())
                {

                    double strokeThickness = this.StrokeThickness;
                    if (this.StrokeThickness >= values.Width / 2)
                    {
                        strokeThickness = this.StrokeThickness / 4;
                    }

                    if (this.SegmentInteriorList.IsUpdated == true && this.SegmentInteriorList.DataCount == 0)// || (this.SegmentInteriorList != null && this.SegmentInteriorList.Count == 0))
                    {
                        context.DrawRectangle(this.Interior, new Pen(this.Stroke, strokeThickness), new Rect(values.X, values.Y, values.Width, values.Height));
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
                        else
                        {
                            strokeThickness = this.SegmentInteriorList[i].StrokeThickness;
                            if (this.SegmentInteriorList[i].StrokeThickness >= values.Width / 2)
                            {
                                strokeThickness = this.SegmentInteriorList[i].StrokeThickness / 4;
                            }
                        }

                        context.DrawRectangle(this.SegmentInteriorList[i].Interior, new Pen(this.SegmentInteriorList[i].Stroke, strokeThickness), new Rect(values.X, values.Y, values.Width, values.Height));

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
                ChartFastStackingColumnSegment segment = null;
                if (this.DataContext != null)
                {
                    segment = this.DataContext as ChartFastStackingColumnSegment;
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
    /// Class implementation for StackingColumnChartValues
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class StackingColumnChartValues : DependencyObject
    {
        #region dependency properties
        /// <summary>
        /// Identifies the IsLower dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLowerProperty =
            DependencyProperty.Register("IsLower", typeof(bool), typeof(StackingColumnChartValues), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies the IsUpper dependency property.
        /// </summary>
        public static readonly DependencyProperty IsUpperProperty =
            DependencyProperty.Register("IsUpper", typeof(bool), typeof(StackingColumnChartValues), new FrameworkPropertyMetadata(false));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this segment is lower part of stack.
        /// </summary>
        /// <value><c>true</c> if this instance is lower; otherwise, <c>false</c>.</value>
        public bool IsLower
        {
            get { return (bool)GetValue(IsLowerProperty); }
            set { SetValue(IsLowerProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is upper part of stack.
        /// </summary>
        /// <value><c>true</c> if this instance is upper; otherwise, <c>false</c>.</value>
        public bool IsUpper
        {
            get { return (bool)GetValue(IsUpperProperty); }
            set { SetValue(IsUpperProperty, value); }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(StackingColumnChartValues), new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(StackingColumnChartValues), new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(StackingColumnChartValues), new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(StackingColumnChartValues), new FrameworkPropertyMetadata(0d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X co-ordinate of segment. This is a dependency property.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get
            {
                return (double)GetValue(XProperty);
            }

            set
            {
                SetValue(XProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate of segment. This is a dependency property.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            get
            {
                return (double)GetValue(YProperty);
            }

            set
            {
                SetValue(YProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of segment. This is a dependency property.
        /// </summary>
        /// <value>The width value.</value>
        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }

            set
            {
                SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of segment. This is a dependency property.
        /// </summary>
        /// <value>The height value.</value>
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }

            set
            {
                SetValue(HeightProperty, value);
            }
        }
        #endregion

    }

    /// <summary>
        /// Class implementation for StackingColumnChartValuesCollection
    /// </summary>
    public class StackingColumnChartValuesCollection : ObservableCollection<StackingColumnChartValues>
    {
        /// <summary>
        /// Empty constructor for StackingColumnChartValuesCollection
        /// </summary>
        public StackingColumnChartValuesCollection()
        {
        }

    }
}
