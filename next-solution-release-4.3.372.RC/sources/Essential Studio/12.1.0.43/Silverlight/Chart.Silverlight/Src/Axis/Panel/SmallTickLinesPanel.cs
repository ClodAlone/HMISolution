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
using System.Collections.Generic;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Public Class implementation for SmallTickLinesPanel
    /// </summary>
    public class SmallTickLinesPanel : Panel
    {
        Grid childGrid = new Grid();
        //Grid smallchildGrid = new Grid();
        /// <summary>
        /// Called when instance created for SmallTickLinesPanel
        /// </summary>
        public SmallTickLinesPanel()
        {
            this.Children.Add(childGrid);
            //this.Children.Add(smallchildGrid);
            this.IsRefreshLines = true;
            this.Axes = new AxesCollection();
            this.Loaded += new RoutedEventHandler(ChartAxesGridLinesPanel_Loaded);
        }

        void ChartAxesGridLinesPanel_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (ChartAxis axis in this.Axes)
            {
                axis.GridLinesChanged += new PropertyChangedCallback(axis_GridLinesChanged);
            }
        }

        void axis_GridLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (axis.IsRefreshItems == true)
                {
                    this.IsRefreshLines = true;
                }

                this.InvalidateMeasure();
            }
        }

        internal bool IsRefreshLines
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Axes property
        /// </summary>
        public AxesCollection Axes
        {
            get
            {
                return (AxesCollection)GetValue(AxesProperty);
            }

            set
            {
                SetValue(AxesProperty, value);
            }
        }
        /// <summary>
        ///  Identifies the Axes dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesProperty =
DependencyProperty.Register("Axes", typeof(AxesCollection), typeof(SmallTickLinesPanel), new PropertyMetadata(null));

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.IsRefreshLines || this.childGrid.Children.Count == 0 )//|| this.smallchildGrid.Children.Count==0)
            {
                GenerateLines(availableSize);
                this.IsRefreshLines = false;
            }
            else
            {
                GenerateLines(availableSize);
                UpdateLines(availableSize);
            }

            childGrid.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        internal void RefreshPanel()
        {
            this.IsRefreshLines = true;
            this.InvalidateMeasure();
        }
        /// <summary>
        /// Method implementation for UpdateLines in SmallTickLines
        /// </summary>
        /// <param name="avilabelSize"></param>
        public void UpdateLines(Size avilabelSize)
        {
            int index = 0;
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.ChartAxesProvider == null || axis.SmallTicksRequired == false)
                    continue;
                else if (!(axis.ChartAxesProvider is IChartCartesianAxes))
                    continue;

                Size TotalSize = new Size();
                TotalSize = new Size(double.IsNaN(avilabelSize.Width) ? 0 : avilabelSize.Width, double.IsNaN(avilabelSize.Height) ? 0 : avilabelSize.Height);

                double interval = (axis.ActualVisibleInterval / (axis.SmallTicksPerInterval + 1));

                bool isOpposedPosition = axis.OpposedPosition;
                if (axis.ChartAxesProvider is IChartRadarAxes && axis.Area != null)
                {
                    isOpposedPosition = ChartRadarType.GetIsClockWise(axis.Area);
                }
                else if (axis.Area != null && axis.ChartAxesProvider is IChartPolarAxes)
                {
                    isOpposedPosition = ChartPolarType.GetIsClockWise(axis.Area);
                }
                double axisIndex = 0d;
                if (axis.IsLogarithmic && axis.LogarithmicBase == 10)
                {

                    foreach (ChartAxisPoints point in axis.ChartAxesProvider.GetPoints(axis.LogarithmicRange.Start, axis.LogarithmicRange.End, interval, axis.Orientation, isOpposedPosition, TotalSize, new List<double>(), axis.IsLogarithmic, axis.LogarithmicBase, axis.firstinterval, axis))
                    {
                        if (index < this.childGrid.Children.Count)
                        {
                            Line ln = this.childGrid.Children[index] as Line;
                            index++;

                            if (ln != null)
                            {
                                ln.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                                ln.Visibility = ln.Visibility == Visibility.Visible && axis.SmallTicksRequired ? Visibility.Visible : Visibility.Collapsed;
                                ln.Stroke = axis.MinorGridLineStroke;
                                ln.StrokeThickness = axis.MinorGridLineStrokeThickness;
                                ln.StrokeDashArray = axis.GridLineStyle;
                                ln.Margin = new Thickness(axis.Orientation == Orientation.Horizontal ? point.X1 : 0, axis.Orientation == Orientation.Horizontal ? 0 : point.Y1, 0, 0);
                            }
                            interval = interval + 1;
                        }
                    }
                }
                else
                {
                    foreach (ChartAxisPoints point in axis.ChartAxesProvider.GetPoints(axis.ActualVisibleRange.Start, axis.ActualVisibleRange.End, interval, axis.Orientation, isOpposedPosition, TotalSize, new List<double>(), axis.IsLogarithmic, axis.LogarithmicBase, axis.firstinterval, axis.m_enableBreaks, axis))
                    {
                        if (index < this.childGrid.Children.Count)
                        {
                            Line ln = this.childGrid.Children[index] as Line;
                            //Line smallln = this.smallchildGrid.Children[index] as Line;
                            index++;

                            if (ln != null)
                            {
                                ln.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                                ln.Visibility = ln.Visibility == Visibility.Visible && axis.SmallTicksRequired ? Visibility.Visible : Visibility.Collapsed;
                                ln.Stroke = axis.MinorGridLineStroke;
                                ln.StrokeThickness = axis.MinorGridLineStrokeThickness;
                                ln.StrokeDashArray = axis.GridLineStyle;
                                ln.Margin = new Thickness(axis.Orientation == Orientation.Horizontal ? point.X1 : 0, axis.Orientation == Orientation.Horizontal ? 0 : point.Y1, 0, 0);
                                if (axisIndex == 0 || axisIndex > (axis.SmallTicksPerInterval))
                                {
                                    ln.Visibility = System.Windows.Visibility.Collapsed;
                                    axisIndex = 0;
                                }
                                axisIndex++;
                            }

                            //if (smallln != null)
                            //{
                            //    smallln.X1 = axis.OpposedPosition == true && axis.Orientation == Orientation.Vertical ? avilabelSize.Width : 0;
                            //    smallln.Y1 = axis.OpposedPosition == false && axis.Orientation == Orientation.Horizontal ? avilabelSize.Height : 0;
                            //    smallln.X2 = axis.Orientation == Orientation.Horizontal ? 0 : smallln.X1 + (!axis.OpposedPosition ? 5 : -5);
                            //    smallln.Y2 = axis.Orientation == Orientation.Horizontal ? smallln.Y1 + (axis.OpposedPosition ? 5 : -5) : 0;

                            //    smallln.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                            //    smallln.Visibility = ln.Visibility == Visibility.Visible && axis.SmallTicksRequired ? Visibility.Visible : Visibility.Collapsed;
                            //    smallln.Stroke = axis.SmallTicksStroke;
                            //    smallln.StrokeThickness = axis.SmallTicksStrokeThickness;
                            //    smallln.Margin = new Thickness(point.X1 + (axis.Orientation == Orientation.Vertical ? (axis.OpposedPosition ? left * -1 : right) : left), point.Y1 + (axis.Orientation == Orientation.Horizontal ? (!axis.OpposedPosition ? bottom : top * -1) : 0), 0, 0);
                            //}
                        }
                    }
                }
            }
        }

        //double left = 0, right = 0, top = 0, bottom = 0;

        /// <summary>
        /// Method implementation for Create SmallTickLines
        /// </summary>
        /// <param name="avilabelSize"></param>
        public void GenerateLines(Size avilabelSize)
        {
            childGrid.Children.Clear();
            //smallchildGrid.Children.Clear();
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.ChartAxesProvider == null || axis.SmallTicksRequired==false)
                    continue;
                else if (!(axis.ChartAxesProvider is IChartCartesianAxes) && !axis.SmallTicksRequired)
                    continue;

                Size TotalSize = new Size();
                TotalSize = new Size(double.IsNaN(avilabelSize.Width) ? 0 : avilabelSize.Width, double.IsNaN(avilabelSize.Height) ? 0 : avilabelSize.Height);

                double interval = 0;
                    interval = (axis.ActualVisibleInterval / (axis.SmallTicksPerInterval + 1));

                bool isOpposedPosition = axis.OpposedPosition;
                if (axis.ChartAxesProvider is IChartRadarAxes && axis.Area != null)
                {
                    isOpposedPosition = ChartRadarType.GetIsClockWise(axis.Area);
                }
                else if (axis.Area != null && axis.ChartAxesProvider is IChartPolarAxes)
                {
                    isOpposedPosition = ChartPolarType.GetIsClockWise(axis.Area);
                }
                double axisIndex = 0d;
                if (axis.IsLogarithmic && axis.LogarithmicBase == 10)
                {
                    foreach (ChartAxisPoints point in axis.ChartAxesProvider.GetPoints(axis.LogarithmicRange.Start, axis.LogarithmicRange.End, interval, axis.Orientation, isOpposedPosition, TotalSize, new List<double>(), axis.IsLogarithmic, axis.LogarithmicBase, axis.firstinterval, axis))
                    {
                        Line ln = new Line();
                        ln.Stroke = axis.MinorGridLineStroke;
                        ln.StrokeThickness = axis.MinorGridLineStrokeThickness;
                        ln.StrokeDashArray = axis.GridLineStyle;
                        ln.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                        ln.X1 = point.X1;
                        ln.Y1 = point.Y1;
                        ln.X2 = point.X2;
                        ln.Y2 = point.Y2;
                        ln.HorizontalAlignment = HorizontalAlignment.Left;
                        ln.VerticalAlignment = VerticalAlignment.Top;
                        ln.Stretch = Stretch.Fill;
                        ln.Margin = new Thickness(axis.Orientation == Orientation.Horizontal ? point.X1 : 0, axis.Orientation == Orientation.Horizontal ? 0 : point.Y1, 0, 0);
                        childGrid.Children.Add(ln);
                    }
                }
                else
                {
                    foreach (ChartAxisPoints point in axis.ChartAxesProvider.GetPoints(axis.ActualVisibleRange.Start, axis.ActualVisibleRange.End, interval, axis.Orientation, isOpposedPosition, TotalSize, new List<double>(), axis.IsLogarithmic, axis.LogarithmicBase, axis.firstinterval, axis.m_enableBreaks, axis))
                    {
                        Line ln = new Line();
                        ln.Stroke = axis.MinorGridLineStroke;
                        ln.StrokeThickness = axis.MinorGridLineStrokeThickness;
                        ln.StrokeDashArray = axis.GridLineStyle;
                        ln.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                        ln.X1 = point.X1;
                        ln.Y1 = point.Y1;
                        ln.X2 = point.X2;// axis.Orientation == Orientation.Horizontal ? 0 : 5;
                        ln.Y2 = point.Y2;// axis.Orientation == Orientation.Horizontal ? 5 : 0;
                        ln.HorizontalAlignment = HorizontalAlignment.Left;
                        ln.VerticalAlignment = VerticalAlignment.Top;
                        ln.Stretch = Stretch.Fill;
                        //ln.Margin = new Thickness(axis.Orientation == Orientation.Horizontal ? point.X1 : 0, axis.Orientation==Orientation.Horizontal ? 0 : point.Y1, 0, 0);
                        if (axisIndex == 0 || axisIndex > (axis.SmallTicksPerInterval))
                        {
                            ln.Visibility = System.Windows.Visibility.Collapsed;
                            axisIndex = 0;
                        }
                        axisIndex++;
                        childGrid.Children.Add(ln);

                        //    Line smallindicatorln = new Line();
                        //    smallindicatorln.Stroke = axis.SmallTicksStroke;
                        //    smallindicatorln.StrokeThickness = axis.SmallTicksStrokeThickness;
                        //    smallindicatorln.X1 = axis.OpposedPosition == true && axis.Orientation == Orientation.Vertical ? avilabelSize.Width : 0;
                        //    smallindicatorln.Y1 = axis.OpposedPosition == false && axis.Orientation == Orientation.Horizontal ? avilabelSize.Height : 0;
                        //    smallindicatorln.X2 = axis.Orientation == Orientation.Horizontal ? 0 : smallindicatorln.X1 + (!axis.OpposedPosition ? 5 : -5);
                        //    smallindicatorln.Y2 = axis.Orientation == Orientation.Horizontal ? smallindicatorln.Y1 + (axis.OpposedPosition ? 5 : -5) : 0;
                        //    smallindicatorln.HorizontalAlignment = HorizontalAlignment.Left;
                        //    smallindicatorln.VerticalAlignment = VerticalAlignment.Top;
                        //    smallindicatorln.Margin = new Thickness(point.X1, point.Y1, 0, 0);
                        //    smallchildGrid.Children.Add(smallindicatorln);
                    }
                }
                //if (axis.SmallTicksRequired)
                //{
                //    this.bottom = axis.Orientation == Orientation.Horizontal && axis.OpposedPosition == false ? 5 : this.bottom;
                //    this.top = axis.Orientation == Orientation.Horizontal && axis.OpposedPosition == true ? 5 : this.top;
                //    this.left = axis.Orientation == Orientation.Vertical && axis.OpposedPosition == false ? 5 : this.left;
                //    this.right = axis.Orientation == Orientation.Vertical && axis.OpposedPosition == true ? 5 : this.right;
                //}
            }

            //smallchildGrid.Margin = new Thickness(-left, -top, -right, -bottom);
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
            childGrid.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            //smallchildGrid.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }
    }
}
