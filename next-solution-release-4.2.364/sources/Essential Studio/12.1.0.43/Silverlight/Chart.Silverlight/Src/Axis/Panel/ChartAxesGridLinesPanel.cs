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
using System.Linq;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartAxesgridLinesPanel
    /// </summary>
    public class ChartAxesGridLinesPanel : Panel
    {
        Grid childGrid = new Grid();
        ObservableCollection<Line> lns = new ObservableCollection<Line>();
        int alt_cnt = 0;
        /// <summary>
        /// Called when instance created for ChartAxesGridLinesPanel
        /// </summary>
        public ChartAxesGridLinesPanel()
        {
            this.Children.Add(childGrid);
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
        /// Identifies the Axes dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesProperty =
DependencyProperty.Register("Axes", typeof(AxesCollection), typeof(ChartAxesGridLinesPanel), new PropertyMetadata(null));

        private UIElement GetGridChildByName(Grid gr, string childname)
        {
            if (gr != null)
            {
                foreach (UIElement child in gr.Children)
                {
                    if (child.GetValue(NameProperty).ToString() == childname)
                        return child;
                }
            }
            return null;
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
            Grid grid = GetParentGrid();
            Grid sergrid =this.GetGridChildByName(grid, "InternalCanvas") as Grid;

            Canvas secaxis = this.GetGridChildByName(sergrid, "secondaryaxis") as Canvas;
            int actualChildCount = (from d in this.Axes select d.Items.Count).Sum();
            if (this.IsRefreshLines || this.childGrid.Children.Count == 0 || this.childGrid.Children.Count != actualChildCount)
            {
                GenerateLines();
                UpdateLines();
                this.IsRefreshLines = false;
            }
            else
            {
                UpdateLines();
                //GenerateLines();
            }
            childGrid.Measure(availableSize);
            UpdateLines();
            foreach (ChartAxis axis in this.Axes)
            {
                if (lns.Count > 0 && (axis.ChartAxesProvider is IChartPolarAxes || axis.ChartAxesProvider is IChartRadarAxes) && axis.Orientation == Orientation.Horizontal)
                {

                    if (axis.Area != null && ((axis.ChartAxesProvider is IChartRadarAxes && ChartRadarType.GetDrawType(axis.Area) == ChartPolarDrawType.Area) || (axis.ChartAxesProvider is IChartPolarAxes && ChartPolarType.GetDrawType(axis.Area) == ChartPolarDrawType.Area)))
                    {
                        //Modified for Manual Testing issue MT1332
                        var lastLine = lns[lns.Count - 1];
                        
                       
                        Line line = new Line() { X1 = lastLine.X1, X2 = lastLine.X2, Y1 = lastLine.Y1, Y2 = lastLine.Y2, Stroke = lastLine.Stroke, StrokeThickness = lastLine.StrokeThickness, Visibility = System.Windows.Visibility.Visible };
                        lns[lns.Count - 1].Visibility = System.Windows.Visibility.Collapsed;
                        //childGrid.Children.Remove(lns[lns.Count-1]); 
                        lns[lns.Count - 1].Visibility = System.Windows.Visibility.Visible;
                        if (secaxis != null)
                        {
                            secaxis.Children.Clear();
                            secaxis.Children.Add(line);//new Line() {X1= secaxisline.X1, X2= secaxisline.X2, Y1= secaxisline.Y1, Y2= secaxisline.Y2, Visibility= System.Windows.Visibility.Visible, Stroke= secaxisline.Stroke, StrokeThickness= secaxisline.StrokeThickness });//new Line() { X1 = lns[0].X1, X2 = lns[0].X2, Y1 = lns[0].Y1, Y2 = lns[0].Y2 , Visibility= System.Windows.Visibility.Visible, HorizontalAlignment = System.Windows.HorizontalAlignment.Left, VerticalAlignment= System.Windows.VerticalAlignment.Top});
                            secaxis.InvalidateMeasure();
                            secaxis.InvalidateArrange();
                        }
                    }
                }
            }
           
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Identify the Parent Grid of chart axis
        /// </summary>
        /// <returns>Parent Grid Value</returns>
        internal Grid GetParentGrid()
        {
            DependencyObject element = this;
            while (!(element is Grid))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element is Grid)
            {
                return element as Grid;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Method implementation for update the f=grid lines when any property changed
        /// </summary>
        public void UpdateLines()
        {
            int index = this.alt_cnt;
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.ChartAxesProvider == null)
                    continue;
                int count = 0;
                foreach (DataAxis axislabel in axis.Items)
                {
                    if (index < this.childGrid.Children.Count)
                    {
                        Line ln = this.childGrid.Children[index] as Line;
                        Polyline polyline = this.childGrid.Children[index] as Polyline;
                        Ellipse ellipse = this.childGrid.Children[index] as Ellipse;
                        index++;

                        if (ln != null)
                        {
                            ln.X1 = axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes ? 0 : axislabel.X1;
                            ln.Y1 = axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes ? 0 : axislabel.Y1;
                            ln.X2 = axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes ? 0 : axislabel.X2;
                            ln.Y2 = axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes ? 5 : axislabel.Y2;

                            ln.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                            ln.Stroke = axis.GridLineStroke;
                            ln.StrokeThickness = count == 0 ? 1 : axis.GridLineStrokeThickness;
                            ln.StrokeDashArray = axis.GridLineStyle;
                            if (axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes)
                            {
                                ln.Margin = new Thickness(axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? axislabel.PointInfo.X1 : 0, axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? 0 : axislabel.PointInfo.Y1, 0, 0);
                                if (this.Axes.Count > 2)
                                {
                                    if (this.Axes[2].OpposedPosition == false)
                                    {
                                        if (axislabel == axis.Items.LastOrDefault() && axislabel.RelatedAxis.Orientation == Orientation.Horizontal)
                                            ln.Margin = new Thickness(ln.Margin.Left - ln.StrokeThickness, ln.Margin.Top, ln.Margin.Right, ln.Margin.Bottom);
                                        if (axislabel == axis.Items.FirstOrDefault() && axislabel.RelatedAxis.Orientation == Orientation.Vertical)
                                            ln.Margin = new Thickness(ln.Margin.Left, ln.Margin.Top - ln.StrokeThickness, ln.Margin.Right, ln.Margin.Bottom);
                                    }
                                }
                                else
                                {
                                    if (axislabel == axis.Items.LastOrDefault() && axislabel.RelatedAxis.Orientation == Orientation.Horizontal)
                                        ln.Margin = new Thickness(ln.Margin.Left - ln.StrokeThickness, ln.Margin.Top, ln.Margin.Right, ln.Margin.Bottom);
                                    if (axislabel == axis.Items.FirstOrDefault() && axislabel.RelatedAxis.Orientation == Orientation.Vertical)
                                        ln.Margin = new Thickness(ln.Margin.Left, ln.Margin.Top - ln.StrokeThickness, ln.Margin.Right, ln.Margin.Bottom);
                                }
                                if (axislabel.RelatedAxis.Orientation == Orientation.Vertical)
                                {
                                    ln.X2 = 5;
                                    ln.Y2 = 0;
                                }
                            }
                            //if (count == 0 && axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes)
                            //{
                            //    ln.Visibility = System.Windows.Visibility.Collapsed;
                            //}

                            count++;
                        }
                        else if (polyline != null)
                        {
                            polyline.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                            polyline.Points = axislabel.PolyPoints;
                            polyline.Stroke = axis.GridLineStroke;
                            polyline.StrokeThickness = axis.GridLineStrokeThickness;
                        }
                        else if (ellipse != null)
                        {
                            ellipse.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                            ellipse.Stroke = axis.GridLineStroke;
                            ellipse.StrokeThickness = axis.GridLineStrokeThickness;
                            ellipse.Width = axislabel.Radius;
                            ellipse.Height = axislabel.Radius;
                        }
                    }
                }
                if (axis.ShowOriginLine && (axis.ChartAxesProvider is ChartCartesianAxesGenerator))
                {
                    Line ln = new Line();
                    ln.Stroke = axis.OriginLineStroke;
                    ln.StrokeThickness = axis.OriginLineStrokeThickness;
                    ln.HorizontalAlignment = HorizontalAlignment.Left;
                    ln.VerticalAlignment = VerticalAlignment.Top;
                    ln.Stretch = Stretch.Fill;
                    Orientation orientation = axis.Orientation;
                    foreach (ChartSeries series in axis.axisBindedSeriesList)
                    {
                        if (series.Type == ChartTypes.Bar || series.Type == ChartTypes.Gantt || series.Type == ChartTypes.RotatedSpline || series.Type == ChartTypes.Tornado || series.Type == ChartTypes.StackingBar || series.Type == ChartTypes.StackingBar100)
                        {
                            axis.isRotatedAxis = true;
                        }
                        else
                        {
                            axis.isRotatedAxis = false;
                        }
                    }
                    if (axis.isRotatedAxis)
                    {
                        orientation = ((axis.Orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal);
                    }
                    if (axis.isRotatedAxis)
                    {
                        if (orientation == Orientation.Horizontal)
                        {
                            ln.X1 = 0;
                            ln.X2 = 0;
                            ln.Y1 = 5;
                            ln.Y2 = 0;
                            double originvalue = axis.Area.ValueToPoint(axis.Area.SecondaryAxis, axis.Origin);
                            if (!double.IsNaN(originvalue))
                            {
                                ln.Margin = new Thickness(originvalue, 0, 0, 0);
                                childGrid.Children.Add(ln);
                            }
                        }
                        else if (axis.Items.Count > 0 && axis.Area != null)
                        {

                            ln.X1 = 0;
                            ln.X2 = 5;
                            ln.Y1 = 0;
                            ln.Y2 = 0;// axis.Area.ValueToPoint(axis.Area.SecondaryAxis, axis.Origin) - axis.Area.AxesThickness.Bottom;
                            double totalSize = (axis.Items[axis.Items.Count - 1] as DataAxis).Y2 - axis.Area.AxesThickness.Bottom - ln.StrokeThickness;
                            double originvalue = totalSize * (1 - ((1 / (axis.Area.PrimaryAxis.VisibleRange.End - axis.Area.PrimaryAxis.VisibleRange.Start)) * (axis.Origin + (axis.Area.PrimaryAxis.VisibleRange.Start * (-1)))));
                            if (!double.IsNaN(originvalue))
                            {
                                ln.Margin = new Thickness(0, originvalue, 0, 0);
                                childGrid.Children.Add(ln);
                            }
                        }
                    }
                    else
                    {
                        if (orientation == Orientation.Horizontal && axis.Items.Count > 0 && axis.Area != null)
                        {
                            ln.X1 = 0;
                            ln.X2 = 5;
                            ln.Y1 = 0;
                            ln.Y2 = 0;// axis.Area.ValueToPoint(axis.Area.SecondaryAxis, axis.Origin) - axis.Area.AxesThickness.Bottom;
                            double totalSize = (axis.Items[axis.Items.Count - 1] as DataAxis).Y2 - axis.Area.AxesThickness.Bottom - ln.StrokeThickness;
                            double originvalue = totalSize * (1 - ((1 / (axis.Area.SecondaryAxis.VisibleRange.End - axis.Area.SecondaryAxis.VisibleRange.Start)) * (axis.Origin + (axis.Area.SecondaryAxis.VisibleRange.Start * (-1)))));
                            if (axis.Origin == 0)
                                originvalue -= axis.OriginLineStrokeThickness;
                            if (!double.IsNaN(originvalue))
                            {
                                ln.Margin = new Thickness(0, originvalue, 0, 0);
                                childGrid.Children.Add(ln);
                            }
                        }
                        else if (axis.Area != null && axis.Origin >= axis.Area.PrimaryAxis.VisibleRange.Start && axis.Origin <= axis.Area.PrimaryAxis.VisibleRange.End)
                        {
                            ln.X1 = 0;
                            ln.X2 = 0;
                            ln.Y1 = 5;
                            ln.Y2 = 0;
                            double originvalue = axis.Area.ValueToPoint(axis.Area.PrimaryAxis, axis.Origin);
                            if (!double.IsNaN(originvalue))
                            {
                                ln.Margin = new Thickness(originvalue, 0, 0, 0);
                                childGrid.Children.Add(ln);
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Method implementation for Draw lines for ChartAxes grid lines
        /// </summary>
        public void GenerateLines()
        {
            childGrid.Children.Clear();
            #region alternative grid background
            int index1 = 0;
            double Prevwidth = 0d, Prevheight = 0d;
            bool isFisrtUpdate = false;
            foreach (ChartAxis axis in this.Axes)
            {
                foreach (DataAxis axislabel in axis.Items)
                {
                    if (axis.Area != null)
                    {
                        if (((System.Windows.Media.SolidColorBrush)(axis.Area.AlternatingGridBackground)).Color != Colors.Transparent)
                        {
                            if (axis.Area.AlternatingFillDirection == Orientation.Horizontal && axis.Orientation == Orientation.Horizontal)
                            {
                                if (axis.Area.AlternatingFillMode == AlternatingFillMode.Even && index1 % 2 != 0)
                                {
                                    Rectangle rect = new Rectangle();
                                    rect.Fill = axis.Area.AlternatingGridBackground;
                                    double width = (axislabel.X1 - Prevwidth);
                                    rect.Width = width < 0 ? 0 : width;
                                    rect.Height = axislabel.Y2;
                                    rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    rect.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                    rect.Margin = new Thickness(axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? axislabel.PointInfo.X1 + (axis.GridLineStrokeThickness) : 0, axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? axis.Area.SecondaryAxis.GridLineStrokeThickness : axislabel.PointInfo.Y1, 0, 0);
                                    childGrid.Children.Add(rect);
                                }
                                else if (axis.Area.AlternatingFillMode == AlternatingFillMode.Odd && index1 % 2 == 0)
                                {
                                    Rectangle rect = new Rectangle();
                                    rect.Fill = axis.Area.AlternatingGridBackground;
                                    double width = (axislabel.X1 - Prevwidth);
                                    rect.Width = width < 0 ? 0 : width;
                                    rect.Height = axislabel.Y2;
                                    rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    rect.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                    rect.Margin = new Thickness(axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? axislabel.PointInfo.X1 + (axis.GridLineStrokeThickness) : 0, axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? axis.Area.SecondaryAxis.GridLineStrokeThickness : axislabel.PointInfo.Y1, 0, 0);
                                    childGrid.Children.Add(rect);
                                    if (Prevwidth == 0)
                                        isFisrtUpdate = true;
                                    if (Prevwidth != 0 && isFisrtUpdate)
                                    {
                                        rect = new Rectangle() { Fill = axis.Area.AlternatingGridBackground, Width = (axislabel.X1 - Prevwidth), Height = axislabel.Y2, HorizontalAlignment = System.Windows.HorizontalAlignment.Left, VerticalAlignment = System.Windows.VerticalAlignment.Top };
                                        childGrid.Children.Add(rect);
                                        isFisrtUpdate = false;
                                    }
                                }
                                Prevwidth = axislabel.X1;
                                index1++;
                            }
                            else if (axis.Area.AlternatingFillDirection == Orientation.Vertical && axis.Orientation == Orientation.Vertical)
                            {
                                if (axis.Area.AlternatingFillMode == AlternatingFillMode.Odd && index1 % 2 != 0)
                                {
                                    Rectangle rect = new Rectangle();
                                    rect.Fill = axis.Area.AlternatingGridBackground;
                                    rect.Width = axislabel.X2;
                                    double height = (Prevheight - axislabel.Y1);
                                    rect.Height = height < 0 ? 0 : height;
                                    rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    rect.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                    rect.Margin = new Thickness(axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? axislabel.PointInfo.X1 : 0, axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? 0 : axislabel.PointInfo.Y1 + (axis.GridLineStrokeThickness), 0, 0);
                                    childGrid.Children.Add(rect);
                                }
                                else if (axis.Area.AlternatingFillMode == AlternatingFillMode.Even && index1 % 2 == 0 && Prevheight != 0)
                                {
                                    Rectangle rect = new Rectangle();
                                    rect.Fill = axis.Area.AlternatingGridBackground;
                                    rect.Width = axislabel.X2;
                                    double height = (Prevheight - axislabel.Y1);
                                    rect.Height = height < 0 ? 0 : height;
                                    rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    rect.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                    rect.Margin = new Thickness(axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? axislabel.PointInfo.X1 : 0, axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? 0 : axislabel.PointInfo.Y1 + (axis.GridLineStrokeThickness), 0, 0);
                                    childGrid.Children.Add(rect);
                                }
                                Prevheight = axislabel.Y1;
                                index1++;
                            }
                        }

                    }
                }
            }

            this.alt_cnt = this.childGrid.Children.Count;
            #endregion

            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.ChartAxesProvider == null)
                    continue;
                int count = 0;
                foreach (DataAxis axislabel in axis.Items)
                {
                    //if (axis.Items.IndexOf(axislabel) == 0)
                    //    continue;
                    Line ln = new Line();
                    ln.Stroke = axis.GridLineStroke;
                    ln.StrokeThickness = axis.GridLineStrokeThickness;
                    ln.StrokeDashArray = axis.GridLineStyle;
                    ln.X1 = axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes ? 0 : axislabel.X1;
                    ln.Y1 = axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes ? 0 : axislabel.Y1;
                    ln.X2 = axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes ? 0 : axislabel.X2;
                    ln.Y2 = axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes ? 5 : axislabel.Y2;
                    ln.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                    ln.Visibility = axislabel.Label.Equals("") && axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes ? Visibility.Collapsed : ln.Visibility;
                    ln.HorizontalAlignment = HorizontalAlignment.Left;
                    ln.VerticalAlignment = VerticalAlignment.Top;
                    if (axislabel.RelatedAxis.ChartAxesProvider is IChartCartesianAxes)
                    {
                        ln.Stretch = Stretch.Fill;
                        ln.Margin = new Thickness(axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? axislabel.PointInfo.X1 : 0, axislabel.RelatedAxis.Orientation == Orientation.Horizontal ? 0 : axislabel.PointInfo.Y1, 0, 0);

                        //fix for cropping of Last GridLine in the Panel - The last grid lines are omitted by the panel, as it comes outside the panel.                        
                        if (axislabel == axis.Items.LastOrDefault() && axislabel.RelatedAxis.Orientation == Orientation.Horizontal)
                            ln.Margin = new Thickness(ln.Margin.Left - ln.StrokeThickness, ln.Margin.Top, ln.Margin.Right, ln.Margin.Bottom);
                        if (axislabel == axis.Items.FirstOrDefault() && axislabel.RelatedAxis.Orientation == Orientation.Vertical)
                            ln.Margin = new Thickness(ln.Margin.Left, ln.Margin.Top - ln.StrokeThickness, ln.Margin.Right, ln.Margin.Bottom);

                        if (axislabel.RelatedAxis.Orientation == Orientation.Vertical)
                        {
                            ln.X2 = 5;
                            ln.Y2 = 0;
                        }
                    }

                    if (axis != null)
                    {
                        if (axis.Orientation == Orientation.Vertical && !(axis.ChartAxesProvider is IChartCartesianAxes))
                        {
                            if (axis.ChartAxesProvider is IChartRadarAxes)
                            {
                                Polyline polyline = new Polyline();
                                polyline.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                                polyline.Points = axislabel.PolyPoints;
                                //This condition and flowdirection of polyline is included to fix netgrid line is not drawn correctly. 
                                if (ChartRadarType.GetIsClockWise(axis.Area))
                                {
                                    polyline.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                                }
                                else
                                {
                                    polyline.FlowDirection = System.Windows.FlowDirection.RightToLeft;
                                }
                                polyline.Stroke = axis.GridLineStroke;
                                polyline.StrokeThickness = axis.GridLineStrokeThickness;
                                childGrid.Children.Add(polyline);
                            }
                            else if (axis.ChartAxesProvider is IChartPolarAxes)
                            {
                                Ellipse ellipse = new Ellipse();
                                ellipse.Visibility = axis.ShowGridLines ? Visibility.Visible : Visibility.Collapsed;
                                ellipse.Fill = new SolidColorBrush(Colors.Transparent);
                                ellipse.Stroke = axis.GridLineStroke;
                                ellipse.StrokeThickness = axis.GridLineStrokeThickness;
                                ellipse.Width = axislabel.Radius;
                                ellipse.Height = axislabel.Radius;
                                childGrid.Children.Add(ellipse);
                            }
                        }
                        else if (axis.ChartAxesProvider is IChartCartesianAxes || ((axis.ChartAxesProvider is IChartPolarAxes || axis.ChartAxesProvider is IChartRadarAxes) && axis.Orientation == Orientation.Horizontal))
                        {
                            if (count == 0)
                            {
                                ln.Visibility = Visibility.Collapsed;
                                lns.Add(ln);
                            }
                            childGrid.Children.Add(ln);
                            count++;
                        }
                    }
                }
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
            childGrid.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }
    }
}
