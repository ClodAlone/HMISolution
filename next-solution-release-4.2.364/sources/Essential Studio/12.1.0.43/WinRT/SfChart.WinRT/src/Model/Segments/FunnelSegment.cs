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
    /// Represents chart funnel segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FunnelSeries"/>
    /// <seealso cref="PyramidSegment"/>
    /// <seealso cref="PyramidSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FunnelSegment : ChartSegment
    {
        #region fields

        private double top, bottom, explodedOffset, minimumWidth, topRadius, bottomRadius;
        /// <summary>
        /// get or Set Isexploded property
        /// </summary>
        public bool IsExploded { get; set; }

        private Path segmentPath;

        private PathGeometry segmentGeometry;

        #endregion

        #region properties

        public double XData { get; set; }

        public double YData { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// Defines the funnel path
        /// </summary>
        /// <param name="y"></param>
        /// <param name="height"></param>
        /// <param name="funnelSeries"></param>
        /// <param name="isExploded"></param>
        public FunnelSegment(double y, double height, FunnelSeries funnelSeries, bool isExploded)
        {
            base.Series = funnelSeries;
            top = y;
            bottom = y + height;
            topRadius = y / 2;
            bottomRadius = (y + height) / 2;
            minimumWidth = funnelSeries.MinWidth;
            explodedOffset = funnelSeries.ExplodeOffset;
            this.IsExploded = isExploded;
        }

        /// <summary>
        /// Defines the funnel path
        /// </summary>
        /// <param name="y"></param>
        /// <param name="height"></param>
        /// <param name="widthTop"></param>
        /// <param name="widthBottom"></param>
        /// <param name="funnelSeries"></param>
        /// <param name="isExploded"></param>
        public FunnelSegment(double y, double height, double widthTop, double widthBottom, FunnelSeries funnelSeries, bool isExploded)
        {

            base.Series = funnelSeries;
            top = y;
            bottom = y + height;
            topRadius = (1 - widthTop) / 2;
            bottomRadius = (1 - widthBottom) / 2;
            minimumWidth = funnelSeries.MinWidth;
            explodedOffset = funnelSeries.ExplodeOffset;
            this.IsExploded = isExploded;
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
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
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
            base.SetVisualBindings(element);
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
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            Rect rect = new Rect(this.explodedOffset, 0, transformer.Viewport.Width - (2 * this.explodedOffset)>=0 ?transformer.Viewport.Width - (2 * this.explodedOffset): 0, transformer.Viewport.Height>=0?transformer.Viewport.Height:0);
            if (rect.IsEmpty)
                segmentPath.Data = null;
            else
            {
                if (this.IsExploded)
                    rect.X = this.explodedOffset; ;
                PathFigure figure = new PathFigure();
                double minRadius = 0.5 * (1d - minimumWidth / rect.Width);
                bool isBroken = (topRadius >= minRadius) ^ (bottomRadius > minRadius);
                double bottomY = minRadius * (bottom - top) / (bottomRadius - topRadius);
                topRadius = Math.Min(topRadius, minRadius);
                bottomRadius = Math.Min(bottomRadius, minRadius);
                figure.StartPoint = new Point(rect.X + topRadius * rect.Width, rect.Y + top * rect.Height);
                Linesegment lineSeg1 = new Linesegment();
                lineSeg1.Point = new Point(rect.X + (1 - topRadius) * rect.Width, rect.Y + top * rect.Height);
                figure.Segments.Add(lineSeg1);
                if (isBroken)
                {
                    Linesegment lineSeg2 = new Linesegment();
                    lineSeg2.Point = new Point(rect.X + (1 - minRadius) * rect.Width, rect.Y + bottomY * rect.Height);
                    figure.Segments.Add(lineSeg2);
                }
                Linesegment lineSeg3 = new Linesegment();
                lineSeg3.Point = new Point(rect.X + (1 - bottomRadius) * rect.Width, rect.Y + bottom * rect.Height - Series.StrokeThickness / 2);
                figure.Segments.Add(lineSeg3);
                Linesegment lineSeg4 = new Linesegment();
                lineSeg4.Point = new Point(rect.X + bottomRadius * rect.Width, rect.Y + bottom * rect.Height - Series.StrokeThickness / 2);
                figure.Segments.Add(lineSeg4);

                if (isBroken)
                {
                    Linesegment lineSeg5 = new Linesegment();
                    lineSeg5.Point = new Point(rect.X + minRadius * rect.Width, rect.Y + bottomY * rect.Height);
                    figure.Segments.Add(lineSeg5);
                }

                figure.IsClosed = true;
                this.segmentGeometry = new PathGeometry();
                this.segmentGeometry.Figures = new PathFigureCollection() { figure };
                segmentPath.Data = segmentGeometry;
            }
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        public override void OnSizeChanged(Size size)
        {

        }
        #endregion

    }
}
