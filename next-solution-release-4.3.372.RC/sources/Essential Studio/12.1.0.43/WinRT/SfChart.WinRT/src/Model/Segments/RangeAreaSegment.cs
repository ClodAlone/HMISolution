#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using WindowsLinesegment = System.Windows.Media.LineSegment;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLinesegment = Windows.UI.Xaml.Media.LineSegment;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart range area segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="RangeAreaSeries"/>
    [ClassReference(IsReviewed = false)]
    public class RangeAreaSegment:ChartSegment
    {

        #region fields

        private List<Point> AreaPoints;

        private RangeAreaSeries containerSeries;

        private bool isHighLow;

        private Path segPath;

        private Brush hiValueInterior, loValueInterior;

        #endregion

        #region properties

        /// <summary>
        /// Gets the actual color used to paint the interior of the segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush ActualInterior
        {
            get
            {
                return isHighLow 
                    ? HighValueInterior : LowValueInterior;
            }
        }

        /// <summary>
        /// Gets the high value interior
        /// </summary>
       [ClassReference(IsReviewed = false)]
        public Brush HighValueInterior
        {
            get
            {
                return hiValueInterior == null 
                    ? this.Interior : hiValueInterior;
            }
            set
            {
                if (hiValueInterior != value)
                {
                    hiValueInterior = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        /// <summary>
        /// Gets the low value interior
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush LowValueInterior
        {
            get
            {
                return loValueInterior == null
                    ? this.Interior: loValueInterior;
            }
            set
            {
                if (loValueInterior != value)
                {
                    loValueInterior = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        private double _high;
        public double High
        {
            get { return _high; }
            set
            {
                _high = value;
                OnPropertyChanged("High");
            }
        }

        private double _low;
        public double Low
        {
            get { return _low; }
            set
            {
                _low = value;
                OnPropertyChanged("Low");
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// Called when instance created for rangeAreaSegments
        /// </summary>
        /// <param name="AreaPoints"></param>
        /// <param name="isHighLow"></param>
        /// <param name="series"></param>
        public RangeAreaSegment(List<Point> AreaPoints, bool isHighLow, RangeAreaSeries series)
        {
            base.Series = series;
            this.containerSeries = series;
            this.isHighLow = isHighLow;
            HighValueInterior = series.HighValueInterior;
            LowValueInterior = series.LowValueInterior;
            SetData(AreaPoints);
        }


        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="AreaPoints"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(List<Point> AreaPoints)
        {
            this.AreaPoints = AreaPoints;
            double X_MAX = AreaPoints.Max(x => x.X);
            double Y_MAX = AreaPoints.Max(y => y.Y);
            double X_MIN = AreaPoints.Min(x => x.X);
            double Y_MIN = AreaPoints.Min(y => y.Y);
            XRange = new DoubleRange(X_MIN, X_MAX);
            YRange = new DoubleRange(Y_MIN, Y_MAX);
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// retuns UIElement
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement CreateVisual(Size size)
        {
            segPath = new Path();
            segPath.Tag = this;
            SetVisualBindings(segPath);
            return segPath;
        }

        /// <summary>
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualInterior");
            element.SetBinding(Shape.FillProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualInterior");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return segPath;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            PathFigure figure = new PathFigure();

            int startIndex = 1;
            int endIndex = AreaPoints.Count - 2;

            if (this.containerSeries.Segments.IndexOf(this) == 0)
            {
                startIndex = 2;
            }

            if (this.containerSeries.Segments.IndexOf(this) == this.containerSeries.Segments.Count - 1)
            {
                endIndex = AreaPoints.Count - 1;
            }

            if (AreaPoints.Count > 0)
            {
                figure.StartPoint = transformer.TransformToVisible(AreaPoints[0].X, AreaPoints[0].Y);

                for (int i = startIndex; i < AreaPoints.Count; i += 2)
                {
                    WindowsLinesegment lineSeg = new WindowsLinesegment();
                    lineSeg.Point=transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y);
                    figure.Segments.Add(lineSeg);
                }

                for (int i = endIndex; i >= 1; i -= 2)
                {
                    WindowsLinesegment lineSeg = new WindowsLinesegment();
                    lineSeg.Point = transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y);
                    figure.Segments.Add(lineSeg);

                }
                figure.IsClosed = true;
            }
            PathGeometry segmentGeometry = new PathGeometry();
            segmentGeometry.Figures.Add(figure);
            segPath.Data = segmentGeometry;
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        [ClassReference(IsReviewed = false)]
        public override void OnSizeChanged(Size size)
        {

        }

        /// <summary>
        /// Called when Property changed 
        /// </summary>
        /// <param name="name"></param>
        protected override void OnPropertyChanged(string name)
        {
            if (name == "Interior")
                name = "ActualInterior";
            base.OnPropertyChanged(name);
        }

        #endregion
    }
}
