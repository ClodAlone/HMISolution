#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartAxisHeaderPanel
    /// </summary>
    public class ChartAxisHeaderPanel : Panel
    {
        /// <summary>
        ///  Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
    DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartAxisHeaderPanel), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Get or Set OrientationProperty
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        ///  Identifies the ChartAxes dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartAxesProperty =
   DependencyProperty.Register("ChartAxes", typeof(ChartAxis), typeof(ChartAxisHeaderPanel), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set ChartAxesProperty
        /// </summary>
        public ChartAxis ChartAxes
        {
            get { return (ChartAxis)GetValue(ChartAxesProperty); }
            set { SetValue(ChartAxesProperty, value); }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxisHeaderPanel panel = d as ChartAxisHeaderPanel;
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            UIElement element;
            if (this.Children.Count > 0)
            {
                element = this.Children[0];
                element.Measure(availableSize);

                Canvas canvas = element as Canvas;
                double width = this.Orientation == Orientation.Horizontal ? element.DesiredSize.Width : element.DesiredSize.Height;
                double height = this.Orientation == Orientation.Horizontal ? element.DesiredSize.Height : element.DesiredSize.Width;
                if (canvas != null && canvas.Children.Count > 0)
                {
                    width = this.Orientation == Orientation.Vertical ? canvas.Children[0].DesiredSize.Height : element.DesiredSize.Width;
                    height = this.Orientation == Orientation.Horizontal ? canvas.Children[0].DesiredSize.Height : element.DesiredSize.Width;
                    //This condition has been commented because when changing the chart types from column to bar, axis header moves away.
                    bool isrotated = false;
                    foreach (ChartSeries series in this.ChartAxes.axisBindedSeriesList)
                    {
                        if (series.Type == ChartTypes.Bar)
                        {
                            isrotated = true;
                        }
                    }
                    if (this.ChartAxes != null && !isrotated)
                    {
                        Canvas.SetTop(canvas.Children[0], 0);
                        Canvas.SetLeft(canvas.Children[0], 0);
                        if (this.Orientation == Orientation.Vertical)
                            Canvas.SetTop(canvas.Children[0], (canvas.Children[0].DesiredSize.Width / 2d));
                        else
                            Canvas.SetLeft(canvas.Children[0], (canvas.Children[0].DesiredSize.Width / 2d * -1));
                    }
                    else
                    {
                        Canvas.SetTop(canvas.Children[0], 0);
                        Canvas.SetLeft(canvas.Children[0], 0);
                        if (this.Orientation == Orientation.Vertical)
                            Canvas.SetTop(canvas.Children[0], (canvas.Children[0].DesiredSize.Width / 2d));
                        else
                            Canvas.SetLeft(canvas.Children[0], (canvas.Children[0].DesiredSize.Width / 2d * -1));
                    }
                }

                if (this.Orientation == Orientation.Horizontal)
                {
                    return new Size(width, height);
                }
                else
                {
                    return new Size(width, height);
                }
            }

            return availableSize;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            UIElement element;
            if (this.Children.Count > 0)
            {
                element = this.Children[0];

                if (this.Orientation == Orientation.Horizontal)
                    element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));
                else
                    element.Arrange(new Rect(0, element.DesiredSize.Width, element.DesiredSize.Width, element.DesiredSize.Height));
            }

            return finalSize;
        }
    }


    //public class ChartAxisLabelRotatePanel : Panel
    //{
    //    public static readonly DependencyProperty RotateProperty =
    //DependencyProperty.Register("Rotate", typeof(double), typeof(ChartAxisLabelRotatePanel), new PropertyMetadata(0d, new PropertyChangedCallback(OnRotateAngleChanged)));

    //    public double Rotate
    //    {
    //        get { return (double)GetValue(RotateProperty); }
    //        set { SetValue(RotateProperty, value); }
    //    }

    //    private static void OnRotateAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        ChartAxisLabelRotatePanel panel = d as ChartAxisLabelRotatePanel;
    //        if (panel != null)
    //        {
    //            panel.InvalidateMeasure();
    //        }
    //    }

    //    protected override Size MeasureOverride(Size availableSize)
    //    {
    //        base.MeasureOverride(availableSize);
    //        UIElement element;
    //        if (this.Children.Count > 0)
    //        {
    //            element = this.Children[0];
    //            element.Measure(availableSize);

    //            Point rightTop = GeneralPointRotation(new Point(0, 0), new Point(element.DesiredSize.Width, 0), this.Rotate);
    //            Point rightBottom = GeneralPointRotation(new Point(0, 0), new Point(0, element.DesiredSize.Height), this.Rotate);
    //            //if (this.Orientation == Orientation.Horizontal)
    //            //    return new Size(element.DesiredSize.Width, element.DesiredSize.Height);
    //            //else
    //            //    return new Size(element.DesiredSize.Height, element.DesiredSize.Width);
    //            return new Size(rightTop.X, rightTop.Y);
    //        }

    //        return availableSize;
    //    }

    //    protected override Size ArrangeOverride(Size finalSize)
    //    {
    //        base.ArrangeOverride(finalSize);
    //        UIElement element;
    //        if (this.Children.Count > 0)
    //        {
    //            element = this.Children[0];

    //        }

    //        return finalSize;
    //    }

    //    Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
    //    {
    //        double ang = angle * Math.PI / 180;
    //        Point displacement = new Point(endpoint.X - originpoint.X, endpoint.Y - originpoint.Y);
    //        endpoint.X = (displacement.X * Math.Cos(ang)) - (displacement.Y * Math.Sin(ang));
    //        endpoint.Y = (displacement.Y * Math.Cos(ang)) + (displacement.X * Math.Sin(ang));
    //        endpoint.X += originpoint.X;
    //        endpoint.Y += originpoint.Y;
    //        return endpoint;
    //    }
    //}
}
