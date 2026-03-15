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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for TickLinesPanel
    /// </summary>
    public class TickLinesPanel : Panel
    {
        Grid childGrid = null;
        /// <summary>
        /// Called when instance created for  TickLinesPanel 
        /// </summary>
        public TickLinesPanel()
        {
            childGrid = new Grid();
            this.Children.Add(childGrid);
        }

        /// <summary>
        ///  Identifies the Axis dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisProperty =
DependencyProperty.Register("Axis", typeof(ChartAxis), typeof(TickLinesPanel), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set AxisProperty
        /// </summary>
        public ChartAxis Axis
        {
            get { return (ChartAxis)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }
        /// <summary>
        ///  Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
DependencyProperty.Register("Orientation", typeof(Orientation), typeof(TickLinesPanel), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));
        /// <summary>
        /// Get or Set Orientation property
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickLinesPanel panel = d as TickLinesPanel;
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
            Size TotalSize = new Size();
            TotalSize = new Size(double.IsNaN(availableSize.Width) ? 0 : availableSize.Width, double.IsNaN(availableSize.Height) ? 0 : availableSize.Height);

            base.MeasureOverride(availableSize);
            if (this.Axis != null)
            {
                childGrid.Children.Clear();
                double interval = (Axis.ActualVisibleInterval / ((Axis.SmallTicksRequired ? Axis.SmallTicksPerInterval : 0) + 1));

                bool isOpposedPosition = Axis.OpposedPosition;
                if (Axis.ChartAxesProvider is IChartRadarAxes && Axis.Area != null)
                {
                    isOpposedPosition = ChartRadarType.GetIsClockWise(Axis.Area);
                }
                else if (Axis.Area != null && Axis.ChartAxesProvider is IChartPolarAxes)
                {
                    isOpposedPosition = ChartPolarType.GetIsClockWise(Axis.Area);
                }
                if (Axis.IsLogarithmic && Axis.LogarithmicBase == 10)
                {
                    foreach (ChartAxisPoints axislabel in Axis.ChartAxesProvider.GetPoints(Axis.LogarithmicRange.Start, Axis.LogarithmicRange.End, interval, Axis.Orientation, isOpposedPosition, TotalSize, new List<double>(), Axis.IsLogarithmic, Axis.LogarithmicBase, Axis.firstinterval, Axis))
                    {
                        Line ln = new Line();
                        Binding bindingStroke = new Binding() { Source = Axis, Path = new PropertyPath("SmallTicksStroke"), Mode = BindingMode.OneWay };
                        BindingOperations.SetBinding(ln, Line.StrokeProperty, bindingStroke);
                        Binding bindingThickness = new Binding() { Source = Axis, Path = new PropertyPath("SmallTicksStrokeThickness"), Mode = BindingMode.OneWay };
                        BindingOperations.SetBinding(ln, Line.StrokeThicknessProperty, bindingThickness);
                        ln.X1 = 0;
                        ln.Y1 = 0;
                        ln.X2 = this.Orientation == Orientation.Horizontal ? 0 : Axis.TickSize;
                        ln.Y2 = this.Orientation == Orientation.Horizontal ? Axis.TickSize : 0;

                        ln.Margin = new Thickness(this.Orientation == Orientation.Horizontal ? axislabel.X1 : 0, this.Orientation == Orientation.Horizontal ? 0 : axislabel.Y1, 0, 0);
                        childGrid.Children.Add(ln);
                    }
                }
                else
                {
                    double smallint = 0;
                    foreach (ChartAxisPoints axislabel in Axis.ChartAxesProvider.GetPoints(Axis.ActualVisibleRange.Start, Axis.ActualVisibleRange.End, interval, Axis.Orientation, isOpposedPosition, TotalSize, new List<double>(), Axis.IsLogarithmic, Axis.LogarithmicBase, Axis.firstinterval, Axis.m_enableBreaks, Axis))
                    {
                        Line ln = new Line();
                        if (smallint == 0)
                        {
                            Binding bindingStroke = new Binding() { Source = Axis, Path = new PropertyPath("TickLineStroke"), Mode = BindingMode.OneWay };
                            BindingOperations.SetBinding(ln, Line.StrokeProperty, bindingStroke);
                            Binding bindingThickness = new Binding() { Source = Axis, Path = new PropertyPath("TickLineStrokeThickness"), Mode = BindingMode.OneWay };
                            BindingOperations.SetBinding(ln, Line.StrokeThicknessProperty, bindingThickness);
                            ln.X1 = 0;
                            ln.Y1 = 0;
                            ln.X2 = this.Orientation == Orientation.Horizontal ? 0 : Axis.TickSize;
                            ln.Y2 = this.Orientation == Orientation.Horizontal ? Axis.TickSize : 0;
                            if (this.Axis.SmallTicksRequired && this.Axis.SmallTicksPerInterval > 0)
                            {
                                smallint = 1;
                            }
                            double leftCanvas = Axis.SmallTickSize > Axis.TickSize ? Math.Abs(Axis.TickSize - Axis.SmallTickSize) : 0;
                            ln.Margin = new Thickness(this.Orientation == Orientation.Horizontal ? axislabel.X1 : leftCanvas, this.Orientation == Orientation.Horizontal ? 0 : axislabel.Y1, 0, 0);
                        }
                        else
                        {
                            Binding bindingStroke1 = new Binding() { Source = Axis, Path = new PropertyPath("SmallTicksStroke"), Mode = BindingMode.OneWay };
                            BindingOperations.SetBinding(ln, Line.StrokeProperty, bindingStroke1);
                            Binding bindingThickness1 = new Binding() { Source = Axis, Path = new PropertyPath("SmallTicksStrokeThickness"), Mode = BindingMode.OneWay };
                            BindingOperations.SetBinding(ln, Line.StrokeThicknessProperty, bindingThickness1);
                            ln.X1 = 0;
                            ln.Y1 = 0;
                            ln.X2 = this.Orientation == Orientation.Horizontal ? 0 : Axis.SmallTickSize;
                            ln.Y2 = this.Orientation == Orientation.Horizontal ? Axis.SmallTickSize : 0;
                            if (Axis.Orientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                double leftCanvas_smallTick = Axis.TickSize > Axis.SmallTickSize ? Math.Abs(Axis.TickSize - Axis.SmallTickSize) : 0;
                                ln.Margin = new Thickness(this.Orientation == Orientation.Horizontal ? axislabel.X1 : 0, 0, 0, leftCanvas_smallTick);
                            }
                            else
                            {
                                double leftCanvas_smallTick = Axis.TickSize > Axis.SmallTickSize ? Math.Abs(Axis.TickSize - Axis.SmallTickSize) : 0;
                                ln.Margin = new Thickness(leftCanvas_smallTick, this.Orientation == Orientation.Horizontal ? 0 : axislabel.Y1, 0, 0);
                            }
                            smallint++;
                            if (smallint > this.Axis.SmallTicksPerInterval)
                            {
                                smallint = 0;
                            }

                        }
                        childGrid.Children.Add(ln);
                    }
                }
                //fix for cropping of Last GridLine in the Panel - The last stroke lines are omitted by the panel, as it comes outside the panel.      
                DataAxis firstlabel = this.Axis.Items.Count > 0 ? this.Axis.Items[0] as DataAxis : new DataAxis();
                DataAxis lastlabel = this.Axis.Items.Count > 0 ? this.Axis.Items[this.Axis.Items.Count - 1] as DataAxis : new DataAxis();
                if (this.Orientation == Orientation.Horizontal && childGrid.Children.Count > 0)
                {
                    Line lastLn = childGrid.Children[childGrid.Children.Count - 1] as Line;
                    lastLn.Margin = new Thickness(lastLn.Margin.Left - lastLn.StrokeThickness, lastLn.Margin.Top, lastLn.Margin.Right, lastLn.Margin.Bottom);
                    if (firstlabel.Label.ToString() == "")
                        childGrid.Children[0].Visibility = Visibility.Collapsed;
                    if (lastlabel.Label.ToString() == "")
                        childGrid.Children[childGrid.Children.Count - 1].Visibility = Visibility.Collapsed;
                }
                if (this.Orientation == Orientation.Vertical && childGrid.Children.Count > 0)
                {
                    Line firstLn = childGrid.Children[0] as Line;
                    firstLn.Margin = new Thickness(firstLn.Margin.Left, firstLn.Margin.Top - firstLn.StrokeThickness, firstLn.Margin.Right, firstLn.Margin.Bottom);
                    if (firstlabel.Label.ToString() == "")
                        childGrid.Children[0].Visibility = Visibility.Collapsed;
                    if (lastlabel.Label.ToString() == "")
                        childGrid.Children[childGrid.Children.Count - 1].Visibility = Visibility.Collapsed;
                }
            }

            if (this.Orientation == Orientation.Horizontal)
            {
                return new Size(availableSize.Width, Axis.TickSize > Axis.SmallTickSize ? Axis.TickSize : Axis.SmallTickSize);
            }
            else
            {
                return new Size(Axis.TickSize > Axis.SmallTickSize ? Axis.TickSize : Axis.SmallTickSize, availableSize.Height);
            }
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
            childGrid.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }
    }
}
