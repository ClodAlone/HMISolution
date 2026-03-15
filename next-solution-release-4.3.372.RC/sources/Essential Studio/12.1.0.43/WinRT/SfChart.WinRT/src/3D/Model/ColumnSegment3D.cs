#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Collections.Generic;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Data;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart column segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WinRT Chart building system.</remarks>
    [ClassReference(IsReviewed = false)]
    public class ColumnSegment3D : ChartSegment3D
    {
        protected double StartDepth, EndDepth;

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Top.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(ColumnSegment3D), new PropertyMetadata(0d, OnValueChanged));

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>
        /// The bottom.
        /// </value>
        public double Bottom
        {
            get { return (double)GetValue(BottomProperty); }
            set { SetValue(BottomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bottom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomProperty =
            DependencyProperty.Register("Bottom", typeof(double), typeof(ColumnSegment3D), new PropertyMetadata(0d, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var columnSegment3D = d as ColumnSegment3D;
            if (columnSegment3D != null) columnSegment3D.OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (Series != null && Series.GetAnimationIsActive())
                Update(Series.CreateTransformer(new Size(), false));
        }

        private double internaltop;

        internal double InternalTop
        {
            get { return internaltop; }
            set { internaltop = value; }
        }

        private Polygon3D[] plans;

        private double internalBottom;
        public double InternalBottom
        {
            get { return internalBottom; }
            set { internalBottom = value; }
        }

        internal Polygon3D[] Plans
        {
            get { return plans; }
            set { plans = value; }
        }

        #region fields
        /// <summary>
        /// Variables declarations
        /// </summary>
        protected double Left = 0d, Right = 0d;

        private double rectX, rectY, width, height;

        /// <summary>
        /// Get or Set XData property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double XData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get or Set YData property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double YData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get or Set Width property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Width
        {

            get
            {

                return width;
            }
            set
            {
                width = value;
                OnPropertyChanged("Width");
            }
        }

        /// <summary>
        /// Get or Set Height property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Height
        {

            get
            {

                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }

        /// <summary>
        /// Get or Set RectX property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double RectX
        {

            get
            {

                return rectX;
            }
            set
            {
                rectX = value;
                OnPropertyChanged("RectX");
            }
        }

        /// <summary>
        /// Get or Set RectY property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double RectY
        {

            get
            {

                return rectY;
            }
            set
            {
                rectY = value;
                OnPropertyChanged("RectY");
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Defines the Column Rectangle
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="endDepth"></param>
        /// <param name="series"></param>
        /// <param name="startDepth"></param>
        public ColumnSegment3D(double x1, double y1, double x2, double y2, double startDepth, double endDepth, ChartSeriesBase series)
        {
            Series = series;
            SetData(x1, y1, x2, y2,startDepth, endDepth);
        }

        /// <summary>
        /// Called when instance created for ColumnSegment
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public ColumnSegment3D(double x1, double y1, double x2, double y2, double startDepth, double endDepth)
        {
            SetData(x1, y1, x2, y2, startDepth, endDepth);
        }


        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="values"></param>
        public override void SetData(params double[] values)
        {
            Plans = null;
            Left = values[0];
            internalBottom = Bottom = values[3];
            internaltop = Top = values[1];
            Right = values[2];
            StartDepth = values[4];
            EndDepth = values[5];
            XRange = new DoubleRange(Left, Right);
            YRange = new DoubleRange(Top, Bottom);
        }

        #endregion

        #region methods
        /// <summary>
        /// method implementation for Set Bindings to properties in ColumnSegment
        /// </summary>
        /// <param name="element"></param>
        [ClassReference(IsReviewed = false)]
        protected override void SetVisualBindings(Shape element)
        {
            base.SetVisualBindings(element);
            var binding = new Binding { Source = this, Path = new PropertyPath("Stroke") };
            element.SetBinding(Shape.StrokeProperty, binding);
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// returns UI Element
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement CreateVisual(Size size)
        {
            return null;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>returns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            if (transformer == null) return;
            var cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
            if (cartesianTransformer == null) return;
            var xBase = cartesianTransformer.XAxis.IsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.XAxis).LogarithmicBase : 1;
            var xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
            var left = xIsLogarithmic ? Math.Log(Left, xBase) : Left;
            var right = xIsLogarithmic ? Math.Log(Right, xBase) : Right;
            var bottom = cartesianTransformer.YAxis.VisibleRange.Start;
            var top = cartesianTransformer.YAxis.VisibleRange.End;
            var xStart = cartesianTransformer.XAxis.VisibleRange.Start;
            var xEnd = cartesianTransformer.XAxis.VisibleRange.End;
            if ((!(left >= xStart) || !(left <= xEnd)) && (!(right >= xStart) || !(right <= xEnd))) return;

            var tlpoint = transformer.TransformToVisible(Left > xStart ? Left : xStart, Top < top ? Top : top);
            var rbpoint = transformer.TransformToVisible(xEnd > Right ? Right : xEnd, bottom > Bottom ? bottom : Bottom);
            var rect = new Rect(tlpoint, rbpoint);
            var area = Series.ActualArea as SfChart3D;
            var tlfVector = new Vector3D(rect.Left, rect.Top, StartDepth);
            var brbVector = new Vector3D(rect.Right, rect.Bottom, EndDepth);
           
            if (plans == null)
                plans = Polygon3D.CreateBox(tlfVector, brbVector, this, Series.Segments.IndexOf(this), area.Graphics3D,
                    Stroke, Interior, StrokeThickness, Series.IsActualTransposed);
            else
                Polygon3D.UpdateBox(plans, tlfVector, brbVector, Interior, tlpoint.Y == rbpoint.Y ? Visibility.Collapsed : Visibility.Visible);
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        [ClassReference(IsReviewed = false)]
        public override void OnSizeChanged(Size size)
        {

        }

        #endregion
    }
}
