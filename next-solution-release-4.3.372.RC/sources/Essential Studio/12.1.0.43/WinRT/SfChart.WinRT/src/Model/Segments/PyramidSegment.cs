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
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Linesegment = System.Windows.Media.LineSegment;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using Linesegment = Windows.UI.Xaml.Media.LineSegment;
using Windows.UI.Xaml.Controls;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart pyramid segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="PyramidSeries"/>
    /// <seealso cref="FunnelSeries"/>
    /// <seealso cref="FunnelSegment"/>
    [ClassReference(IsReviewed = false)]
    public class PyramidSegment : ChartSegment
    {
        #region fields

        private double y = 0d, explodedOffset = 0d, height = 0d;

        private Path segmentPath;

        private PathGeometry segmentGeometry;

        private bool isExploded;
        private double xData;
        private double yData;

        #endregion

        #region properties
        /// <summary>
        /// Gets the X-Value of the <see cref="PieSegment"/>
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double XData
        {
            get
            {
                return xData;
            }
            internal set
            {
                xData = value;
                OnPropertyChanged("XData");
            }
        }

        /// <summary>
        /// Gets the Y-Value of the <see cref="PieSegment"/>
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double YData
        {
            get
            {
                return yData;
            }
            internal set
            {
                yData = value;
                OnPropertyChanged("YData");
            }
        }
        #endregion
        #region ctor

        /// <summary>
        /// Defines the pyramid path
        /// </summary>
        /// <param name="y"></param>
        /// <param name="height"></param>
        /// <param name="explodedOffset"></param>
        /// <param name="series"></param>
        /// <param name="isExploded"></param>
        [ClassReference(IsReviewed = false)]
        public PyramidSegment(double y, double height, double explodedOffset, PyramidSeries series, bool isExploded)
        {
            base.Series = series;
            this.y = y;
            this.height = height;
            this.isExploded = isExploded;
            this.explodedOffset = explodedOffset;
        }

        #endregion

        #region methods

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
            segmentPath = new Path();
            SetVisualBindings(segmentPath);
            segmentPath.Tag = this;
            return segmentPath;
        }

        /// <summary>
        /// Method Implementation for set  Binding to PyramidSegments properties
        /// </summary>
        /// <param name="element"></param>
        [ClassReference(IsReviewed = false)]
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Interior");
            element.SetBinding(Shape.FillProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);

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
            Rect rect = new Rect(0, 0, transformer.Viewport.Width, transformer.Viewport.Height);
            if (rect.IsEmpty)
                this.segmentPath.Data = null;
            else
            {
                if (this.isExploded)
                {
                    rect.X += explodedOffset;
                }
                double top = y;
                double bottom = y + height;
                double topRadius = 0.5d * (1d - y);
                double bottomRadius = 0.5d * (1d - bottom);
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(rect.X + topRadius * rect.Width, rect.Y + top * rect.Height);
                Linesegment lineSeg1 = new Linesegment();
                lineSeg1.Point = new Point(rect.X + (1 - topRadius) * rect.Width, rect.Y + top * rect.Height);
                figure.Segments.Add(lineSeg1);
                Linesegment lineSeg3 = new Linesegment();
                lineSeg3.Point = new Point(rect.X + (1 - bottomRadius) * rect.Width, rect.Y + bottom * rect.Height - Series.StrokeThickness / 2);
                figure.Segments.Add(lineSeg3);
                Linesegment lineSeg4 = new Linesegment();
                lineSeg4.Point = new Point(rect.X + bottomRadius * rect.Width, rect.Y + bottom * rect.Height - Series.StrokeThickness / 2);
                figure.Segments.Add(lineSeg4);
                figure.IsClosed = true;
                this.segmentGeometry = new PathGeometry();
                this.segmentGeometry.Figures = new PathFigureCollection() { figure };
                segmentPath.Data = segmentGeometry;
            }
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return segmentPath;
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

        #endregion
    }
}
