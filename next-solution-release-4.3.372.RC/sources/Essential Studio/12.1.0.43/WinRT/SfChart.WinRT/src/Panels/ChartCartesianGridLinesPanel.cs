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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents ChartCartesianGridLinesPanel.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartCartesianGridLinesPanel:ILayoutCalculator
    {
        #region fields

        private Panel panel;

        internal ChartBase Area;

        private Size desiredSize;

        private readonly UIElementsRecycler<Border> stripLines;

        #endregion

        #region properties

        /// <summary>
        /// Gets the desired size of the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Size DesiredSize
        {
            get { return desiredSize; }
        }

        public Panel Panel
        {
            get { return panel; }
        }

        /// <summary>
        /// Gets the children count in the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public List<UIElement> Children
        {
            get
            {
                if (panel != null)
                {
                    return panel.Children.Cast<UIElement>().ToList();
                }

                return null;
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartCartesianGridLinesPanel
        /// </summary>
        /// <param name="panel"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ChartCartesianGridLinesPanel(Panel panel)
        {
            //if (panel == null)
            //    throw new ArgumentNullException();

            this.panel = panel;

            stripLines = new UIElementsRecycler<Border>(this.panel);
        }

        #endregion

        #region methods
        /// <summary>
        /// Measures the elements in the panel.
        /// </summary>
        /// <param name="availableSize">available size of the panel.</param>
        /// <returns>returns Size</returns>
        [ClassReference(IsReviewed = false)]
        public Size Measure(Size availableSize)
        {
            desiredSize = availableSize;
            return availableSize;
        }
        
        /// <summary>
        /// Arrranges the elements inside a panel.
        /// </summary>
        /// <param name="finalSize">final size of the panel.</param>
        /// <returns>returns Size</returns>
        [ClassReference(IsReviewed = false)]
        public Size Arrange(Size finalSize)
        {
            foreach (ChartAxis axis in Area.Axes)
            {
                if(axis.ShowGridLines)
                    DrawGridLines(axis);
            }
            return finalSize;
        }

        /// <summary>
        /// Arrranges the elements inside a panel.
        /// </summary>
        /// <param name="finalSize">final size of the panel.</param>
        /// <returns>returns Size</returns>
        [ClassReference(IsReviewed = false)]
        public Size Arrange3D(Size finalSize)
        {
            foreach (var axis in Area.Axes.Where(axis => axis.ShowGridLines))
            {
                DrawGridLines3D(axis);
            }
            return finalSize;
        }

        public void DetachElements()
        {
            panel.Children.Clear();
            panel = null;

            if(stripLines != null)
                stripLines.Clear();
        }

        /// <summary>
        /// Adds the elements in the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public void UpdateElements()
        {
            foreach (ChartAxis axis in Area.Axes)
            {
                UpdateGridLines(axis);
            }

            UpdateStripLines();
        }

        /// <summary>
        /// Adds the Gridlines for the axis.
        /// </summary>
        /// <param name="axis"></param>
        [ClassReference(IsReviewed = false)]
        public void UpdateGridLines(ChartAxis axis)
        {
            if (axis == null)
                return;
            if (axis.GridLinesRecycler == null)
                axis.CreateLineRecycler();
            int axesCount = 1;
            if (axis.RegisteredSeries.Count > 0)
            {
                axesCount = axis.Orientation == Orientation.Horizontal
                    ? (axis.AssociatedAxes.DistinctBy(Area.GetActualRow)).Count()
                    : (axis.AssociatedAxes.DistinctBy(Area.GetActualColumn)).Count();
            }
            int tickCount = axis.SmallTickPoints.Count * axesCount;
            if (!(axis is CategoryAxis && (axis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks))
                tickCount = axis.VisibleLabels.Count * axesCount;

            UpdateGridlines(axis, axis.GridLinesRecycler, tickCount, true, true);

            if (axis.smallTicksRequired)
                UpdateGridlines(axis, axis.MinorGridLinesRecycler, axis.SmallTickPoints.Count * axesCount, false, false);

        }

        private void UpdateGridlines(ChartAxis axis, UIElementsRecycler<Line> linesRecycler, int requiredLinescount, bool isMajor, bool checkOrginFlag)
        {
            if (linesRecycler == null || axis == null)
                return;

            int totalLinesCount = requiredLinescount;

            if (!axis.ShowGridLines)
                totalLinesCount = 0;
            else if (axis.ShowOrigin && checkOrginFlag)
                totalLinesCount += 1;

            if (!linesRecycler.BindingProvider.Keys.Contains(FrameworkElement.StyleProperty))
            {
                Binding binding = new Binding();
                binding.Path = isMajor ? new PropertyPath("MajorGridLineStyle") :
                    new PropertyPath("MinorGridLineStyle");
                binding.Source = axis;
                linesRecycler.BindingProvider.Add(FrameworkElement.StyleProperty, binding);
            }

            linesRecycler.GenerateElements(totalLinesCount);
#if!WPF
            //StrokeDashArray applied only for the first line element when it is applied through style. 
            //It is bug in the framework.
            //And hence manually setting stroke dash array for each and every grid line.
            if (linesRecycler.Count > 0)
            {
                DoubleCollection collection = linesRecycler[0].StrokeDashArray;
                if (collection != null && collection.Count > 0)
                {
                    foreach (Line line in linesRecycler)
                    {
                        DoubleCollection doubleCollection = new DoubleCollection();
                        foreach (double value in collection)
                        {
                            doubleCollection.Add(value);
                        }
                        line.StrokeDashArray = doubleCollection;
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Draws the Gridlines at definite intervals in <see cref="ChartAxis"/>
        /// </summary>
        /// <param name="axis">Relevant ChartAxis</param>
        [ClassReference(IsReviewed = false)]
        public void DrawGridLines(ChartAxis axis)
        {
            if (axis == null)
                return;

            double left = axis.RenderedRect.Left - Area.AxisThickness.Left;
            double right = axis.RenderedRect.Right - Area.AxisThickness.Left;
            double top = axis.RenderedRect.Top - Area.AxisThickness.Top;
            double bottom = axis.RenderedRect.Bottom - Area.AxisThickness.Top;

            double width = 0d;
            double height = 0d;


            var values = (from label in axis.VisibleLabels
                select label.Position).ToArray();

            if ((axis is CategoryAxis) && (axis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks)
                values = (from pointValues in axis.SmallTickPoints select pointValues).ToArray();

            if (axis.Orientation == Orientation.Horizontal)
            {
                width = right - left;
                IEnumerable<ChartAxis> selectedAxes = null;
                if (axis.RegisteredSeries.Count > 0)
                {
                    selectedAxes = axis.AssociatedAxes.OrderByDescending(chartAxis => chartAxis.Area.GetActualRowSpan(chartAxis)).DistinctBy(Area.GetActualRow);
                }
                else
                {
                    if (Area.InternalPrimaryAxis != null)
                        selectedAxes = new List<ChartAxis> {Area.InternalSecondaryAxis};
                }
                int index = 0;
                int smallTickIndex = 0;
                foreach (ChartAxis supportAxis in selectedAxes)
                {
                    top = supportAxis.ArrangeRect.Top - Area.AxisThickness.Top;
                    height = top + supportAxis.ArrangeRect.Height;
                    if (values.Length > 0)
                        DrawGridLines(axis, axis.GridLinesRecycler, left, top, width, height, values, true, index);
                    if (axis.smallTicksRequired)
                    {
                        var smallTickvalues = (from pointValues in axis.SmallTickPoints select pointValues).ToArray();
                        if (smallTickvalues.Length > 0)
                            DrawGridLines(axis, axis.MinorGridLinesRecycler, left, top, width, height, smallTickvalues, false, smallTickIndex);
                        smallTickIndex += smallTickvalues.Length;
                    }
                    index += values.Length;
                }
            }
            else
            {
                height = bottom - top;

                IEnumerable<ChartAxis> selectedAxes = null;
                if (axis.RegisteredSeries.Count > 0)
                {
                    selectedAxes = axis.AssociatedAxes.OrderByDescending(chartAxis => chartAxis.Area.GetActualColumnSpan(chartAxis)).DistinctBy(Area.GetActualColumn);
                }
                else
                {
                    if (Area.InternalPrimaryAxis != null)
                        selectedAxes = new List<ChartAxis> {Area.InternalPrimaryAxis};
                }
                int index = 0;
                int smallTickIndex = 0;
                foreach (ChartAxis supportAxis in selectedAxes)
                {
                    left = supportAxis.ArrangeRect.Left - Area.AxisThickness.Left;
                    width = left + supportAxis.ArrangeRect.Width;
                    if (values.Length > 0)
                        DrawGridLines(axis, axis.GridLinesRecycler, left, top, width, height, values, true, index);
                    if (axis.smallTicksRequired)
                    {
                        var smallTickvalues = (from pointValues in axis.SmallTickPoints select pointValues).ToArray();
                        if (smallTickvalues.Length > 0)
                            DrawGridLines(axis, axis.MinorGridLinesRecycler, left, top, width, height, smallTickvalues, false, smallTickIndex);
                        smallTickIndex += smallTickvalues.Length;
                    }
                    index += values.Length;
                }
            }
        }

        private void DrawGridLines(ChartAxis axis, UIElementsRecycler<Line> lines,double left,double top,double width, double height, double[] values, bool drawOrigin, int index)
        {
            if (lines == null || axis == null)
                return;

            int labelsCount = values.Length;
            int linesCount = lines.Count;
            Orientation orienatation = axis.Orientation;
            if (orienatation == Orientation.Horizontal)
            {
                int i;
                for (i = 0; i < labelsCount; i++)
                {
                    if (i < linesCount)
                    {
                        Line line = lines[index];
                        double value = axis.ValueToCoefficientCalc(values[i]);
                        value = double.IsNaN(value) ? 0 : value;
                        //line.X1 = (value=Math.Round(width * value) + 0.5)>width ? value-0.5:value;
                        line.X1 = (Math.Round(width * value)) + left;
                        line.Y1 = top;
                        line.X2 = line.X1;
                        line.Y2 = height;
                    }
                    index++;
                }

                if (axis.ShowOrigin && drawOrigin)
                {
                    Line originLine = lines[i];
                    double value = Area.InternalSecondaryAxis.ValueToCoefficientCalc(axis.Origin);
                    value = double.IsNaN(value) ? 0 : value;
                    originLine.X1 = 0;
                    originLine.Y1 = originLine.Y2 = Math.Round(height * (1 - value));
                    originLine.X2 = width;
                }
            }
            else
            {
                int i;
                for (i = 0; i < labelsCount; i++)
                {
                    if (i < linesCount)
                    {
                        Line line = lines[index];
                        double value = axis.ValueToCoefficientCalc(values[i]);
                        value = double.IsNaN(value) ? 0 : value;
                        line.X1 = left;
                        line.Y1 = Math.Round(height * (1 - value)) + 0.5 + top;
                        line.X2 = width;
                        line.Y2 = line.Y1;
                    }
                    index++;
                }

                if (axis.ShowOrigin && axis.VisibleRange.Delta > 0 && drawOrigin)
                {
                    Line originLine = lines[i];
                    double value = Area.InternalPrimaryAxis.ValueToCoefficientCalc(axis.Origin);
                    value = double.IsNaN(value) ? 0 : value;
                    originLine.X1 = originLine.X2 = Math.Round(width * (value));
                    originLine.Y1 = 0;
                    originLine.Y2 = height;
                }

            }
        }


        /// <summary>
        /// Draws the Gridlines at definite intervals in <see cref="ChartAxis"/>
        /// </summary>
        /// <param name="axis">Relevant ChartAxis</param>
        [ClassReference(IsReviewed = false)]
        public void DrawGridLines3D(ChartAxis axis)
        {

            if (axis == null)
                return;
            double left = axis.RenderedRect.Left;
            var right = axis.RenderedRect.Right;
            var top = axis.RenderedRect.Top;
            var bottom = axis.RenderedRect.Bottom;

            double width;
            double height;


            var values = (from label in axis.VisibleLabels
                          select label.Position).ToArray();

            if ((axis is CategoryAxis) && (axis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks)
                values = (from pointValues in axis.SmallTickPoints select pointValues).ToArray();

            if (axis.Orientation == Orientation.Horizontal)
            {
                width = right - left;
                IEnumerable<ChartAxis> selectedAxes = null;
                if (axis.RegisteredSeries.Count > 0)
                {
                    selectedAxes = axis.AssociatedAxes.DistinctBy(Area.GetActualRow);
                }
                else
                {
                    if (Area.InternalPrimaryAxis != null)
                        selectedAxes = new List<ChartAxis> { Area.InternalSecondaryAxis };
                }
                var index = 0;
                var smallTickIndex = 0;
                foreach (var supportAxis in selectedAxes)
                {
                    top = supportAxis.ArrangeRect.Top;
                    height = top + supportAxis.ArrangeRect.Height;
                    if (values.Length > 0)
                        DrawGridLines3D(axis, axis.GridLinesRecycler, left, top, width, height, values, true, index);
                    if (axis.smallTicksRequired)
                    {
                        var smallTickvalues = (from pointValues in axis.SmallTickPoints select pointValues).ToArray();
                        if (smallTickvalues.Length > 0)
                            DrawGridLines3D(axis, axis.MinorGridLinesRecycler, left, top, width, height, smallTickvalues, false, smallTickIndex);
                        smallTickIndex += smallTickvalues.Length;
                    }
                    index += values.Length;
                }
            }
            else
            {
                height = bottom - top;

                IEnumerable<ChartAxis> selectedAxes = null;
                if (axis.RegisteredSeries.Count > 0)
                {
                    selectedAxes = axis.AssociatedAxes.DistinctBy(Area.GetActualColumn);
                }
                else
                {
                    if (Area.InternalPrimaryAxis != null)
                        selectedAxes = new List<ChartAxis> { Area.InternalPrimaryAxis };
                }
                var index = 0;
                var smallTickIndex = 0;
                foreach (var supportAxis in selectedAxes)
                {
                    left = supportAxis.ArrangeRect.Left;
                    width = left + supportAxis.ArrangeRect.Width;
                    if (values.Length > 0)
                        DrawGridLines3D(axis, axis.GridLinesRecycler, left, top, width, height, values, true, index);
                    if (axis.smallTicksRequired)
                    {
                        var smallTickvalues = (from pointValues in axis.SmallTickPoints select pointValues).ToArray();
                        if (smallTickvalues.Length > 0)
                            DrawGridLines3D(axis, axis.MinorGridLinesRecycler, left, top, width, height, smallTickvalues, false, smallTickIndex);
                        smallTickIndex += smallTickvalues.Length;
                    }
                    index += values.Length;
                }
            }
        }

        private void DrawGridLines3D(ChartAxis axis, UIElementsRecycler<Line> lines, double left, double top, double width, double height, double[] values, bool drawOrigin, int index)
        {
            if (lines == null || axis == null)
                return;

            var labelsCount = values.Length;
            var linesCount = lines.Count;
            var orientation = axis.Orientation;
            double x1, x2, y1, y2;
            if (orientation == Orientation.Horizontal)
            {
                int i;
                for (i = 0; i < labelsCount; i++)
                {
                    if (i < linesCount)
                    {
                        var line = lines[index];
                        var value = axis.ValueToCoefficientCalc(values[i]);
                        value = double.IsNaN(value) ? 0 : value;
                        x2 = x1 = (Math.Round(width * value) + left);
                        y1 = top;
                        y2 = height;
                        var area = Area as SfChart3D;
                        if (area != null)
                        {
                            var depth = area.Depth > 2 ? area.Depth - 2 : 1;
                            var g3 = ((SfChart3D) Area).Graphics3D;
                       
                            g3.AddVisual(Polygon3D.CreateLine(line, x1, y1,x2,y2, depth));
                            var bottom = axis.OpposedPosition ? axis.Area.SeriesClipRect.Top : axis.Area.SeriesClipRect.Height + axis.Area.SeriesClipRect.Top;
                            var line3D = Polygon3D.CreateLine(new Line { Opacity = line.Opacity, StrokeThickness = line.StrokeThickness, Stroke = line.Stroke }, x2, 0, x2, -depth, bottom);
                            line3D.Transform(Matrix3D.Tilt((float)(Math.PI / 2)));
                        
                            g3.AddVisual(line3D);
                        }
                    }
                    index++;
                }
            }
            else
            {
                int i;
                for (i = 0; i < labelsCount; i++)
                {
                    if (i < linesCount)
                    {
                        var line = lines[index];
                        var value = axis.ValueToCoefficientCalc(values[i]);
                        value = double.IsNaN(value) ? 0 : value;
                        x1 = left;
                        y1 = Math.Round(height * (1 - value)) + 0.5 + top;
                        x2 = width;
                        y2 = y1;

                        var area = Area as SfChart3D;
                        var depth = area.Depth > 2 ? area.Depth - 2 : 1;
                        
                        area.Graphics3D.AddVisual(Polygon3D.CreateLine(line, x1, y1, x2, y2, depth));

                        var depthD = axis.OpposedPosition ? axis.Area.SeriesClipRect.Width + axis.Area.SeriesClipRect.Left + 1 : axis.Area.SeriesClipRect.Left;
                        var line3D = Polygon3D.CreateLine(new Line { StrokeThickness = line.StrokeThickness, Stroke = line.Stroke, Opacity = line.Opacity }, -depth, y2, 0, y2, depthD);
                        line3D.Transform(Matrix3D.Turn((float)(-Math.PI / 2)));
                        area.Graphics3D.AddVisual(line3D);

                    }
                    index++;
                }
            }
        }

        private Binding CreateBinding(string path, object source)
        {
            var bindingProvider = new Binding
            {
                Path = new PropertyPath(path),
                Source = source,
                Mode = BindingMode.OneWay
            };
            return bindingProvider;
        }

        private void RenderStripLine(Rect stripRect, ChartStripLine stripLine)
        {
            if (stripRect.IsEmpty) return;
            var border = stripLines.CreateNewInstance();
            var control = new ContentControl();

            border.SetBinding(Border.BackgroundProperty, CreateBinding("Background", stripLine));
            border.SetBinding(Border.BorderBrushProperty, CreateBinding("BorderBrush", stripLine));
            border.SetBinding(Border.BorderThicknessProperty, CreateBinding("BorderThickness", stripLine));
            border.SetBinding(Border.OpacityProperty, CreateBinding("Opacity", stripLine));
            control.SetBinding(ContentControl.ContentProperty, CreateBinding("Label", stripLine));
            control.SetBinding(ContentControl.ContentTemplateProperty, CreateBinding("LabelTemplate", stripLine));
            control.SetBinding(ContentControl.OpacityProperty, CreateBinding("Opacity", stripLine));
            control.RenderTransformOrigin = new Point(0.5, 0.5);
            control.RenderTransform = new RotateTransform
            {
                Angle = stripLine.LabelAngle
            };
            control.SetBinding(FrameworkElement.HorizontalAlignmentProperty, CreateBinding("LabelHorizontalAlignment", stripLine));
            control.SetBinding(FrameworkElement.VerticalAlignmentProperty, CreateBinding("LabelVerticalAlignment", stripLine));

            border.Child = control;

            border.Width = stripRect.Width;
            border.Height = stripRect.Height;
            Canvas.SetLeft(border, stripRect.Left);
            Canvas.SetTop(border, stripRect.Top);
        }

        private void UpdateHorizontalStripLine(ChartAxisBase2D axis)
        {
            var seriesRect = Area.SeriesClipRect;
            var visibleRange = axis.VisibleRange;
            foreach (var stripLine in axis.StripLines)
            {
                IEnumerable<ChartAxis> yAxes;
                ChartAxis cusAxis = Area.Axes[stripLine.SegmentAxisName];
                if (cusAxis != null)
                {
                    yAxes = new List<ChartAxis> { cusAxis };
                }
                else if (axis.RegisteredSeries.Count > 0)
                {
                    yAxes = axis.AssociatedAxes.DistinctBy(Area.GetActualRow);
                }
                else
                {
                    yAxes = new List<ChartAxis> { Area.InternalSecondaryAxis };
                }

                foreach (var currAxis in yAxes)
                {
                    var yAxisRect = currAxis.ArrangeRect;
                    Rect stripRect;
                    if (!stripLine.IsSegmented)
                    {
                        double startStrip = stripLine.Start;
                        double endStrip = stripLine.RepeatUntil == 0
                            ? visibleRange.End
                            : stripLine.RepeatUntil;
                        double periodStrip = stripLine.RepeatEvery;

                        do
                        {
                            double stripStart = startStrip;
                            double stripEnd = stripStart + stripLine.Width;
                            stripEnd = stripEnd > visibleRange.End ? visibleRange.End : stripEnd;
                            stripStart = stripStart < visibleRange.Start
                                ? visibleRange.Start
                                : stripStart;
                            if (!(stripEnd < visibleRange.Start) && !(stripStart > visibleRange.End))
                            {
                                double x1 = Area.ValueToPoint(axis, stripStart);

                                double x2;
                                if (stripLine.IsPixelWidth)
                                {
                                    x2 = x1 + stripLine.Width;
                                }
                                else
                                {
                                    x2 = Area.ValueToPoint(axis, stripEnd);
                                }
                                stripRect = new Rect(new Point(x1, yAxisRect.Top - seriesRect.Top),
                                    new Point(x2, yAxisRect.Height + yAxisRect.Top - seriesRect.Top));
                                RenderStripLine(stripRect, stripLine);
                            }

                            startStrip += periodStrip;
                        } while ((periodStrip != 0) && (startStrip < endStrip));
                    }

                    else
                    {
                        double startStrip = stripLine.Start;
                        double endStrip = stripLine.RepeatUntil == 0
                            ? visibleRange.End
                            : stripLine.RepeatUntil;
                        double periodStrip = stripLine.RepeatEvery;

                        if (!double.IsNaN(stripLine.SegmentStartValue) &&
                            !double.IsNaN(stripLine.SegmentEndValue))
                        {
                            do
                            {
                                if (visibleRange.Inside(startStrip) ||
                                    visibleRange.Inside(startStrip + stripLine.Width))
                                {

                                    double x1 = Area.ValueToPoint(axis, startStrip);
                                    double x2 = x1 + stripLine.Width;

                                    if (axis.Area != null)
                                    {
                                        double startVal = Area.ValueToPoint(currAxis,
                                            stripLine.SegmentStartValue);
                                        double endVal = Area.ValueToPoint(currAxis,
                                            stripLine.SegmentEndValue);

                                        stripRect = new Rect(new Point(x1, startVal),
                                            new Point(x2, endVal));
                                        RenderStripLine(stripRect, stripLine);
                                    }
                                }
                                startStrip += periodStrip;
                            } while ((periodStrip != 0) && (startStrip < endStrip));
                        }
                    }
                }
            }
        }

        private void UpdateVerticalStripLine(ChartAxisBase2D axis)
        {
            var visibleRange = axis.VisibleRange;
            var seriesRect = Area.SeriesClipRect;
            foreach (var stripLine in axis.StripLines)
            {
                IEnumerable<ChartAxis> xAxes;
                ChartAxis cusAxis = Area.Axes[stripLine.SegmentAxisName];
                if (cusAxis != null)
                {
                    xAxes = new List<ChartAxis> { cusAxis };
                }
                else if (axis.RegisteredSeries.Count > 0)
                {
                    xAxes = axis.AssociatedAxes.DistinctBy(Area.GetActualColumn);
                }
                else
                {
                    xAxes = new List<ChartAxis> { Area.InternalPrimaryAxis };
                }

                foreach (var xAxis in xAxes)
                {
                    var xAxisRect = xAxis.ArrangeRect;
                    var yAxisRect = axis.ArrangeRect;

                    Rect stripRect;
                    if (!stripLine.IsSegmented)
                    {
                        double startStrip = stripLine.Start;
                        double endStrip = stripLine.RepeatUntil == 0
                            ? visibleRange.End
                            : stripLine.RepeatUntil;
                        double periodStrip = stripLine.RepeatEvery;

                        do
                        {
                            if (visibleRange.Inside(startStrip) ||
                                visibleRange.Inside(startStrip + stripLine.Width))
                            {
                                double y1 = Area.ValueToPoint(axis, startStrip);
                                double y2;
                                if (stripLine.IsPixelWidth)
                                {
                                    y2 = stripLine.Width < yAxisRect.Height
                                        ? y1 - stripLine.Width
                                        : y1 - yAxisRect.Height;
                                }
                                else
                                {
                                    double stripEnd = startStrip + stripLine.Width;
                                    stripEnd = (stripEnd > visibleRange.End)
                                        ? visibleRange.End
                                        : stripEnd;
                                    y2 = Area.ValueToPoint(axis, stripEnd);
                                }

                                stripRect = new Rect(new Point(xAxisRect.Left - seriesRect.Left, y1),
                                    new Point(xAxisRect.Width + xAxisRect.Left - seriesRect.Left, y2));
                                RenderStripLine(stripRect, stripLine);
                            }

                            startStrip += periodStrip;
                        } while (periodStrip != 0 && (startStrip < endStrip));
                    }
                    else
                    {
                        double startStrip = stripLine.Start;
                        double endStrip = stripLine.RepeatUntil == 0
                            ? visibleRange.End
                            : stripLine.RepeatUntil;
                        double periodStrip = stripLine.RepeatEvery;

                        if (!double.IsNaN(stripLine.SegmentStartValue) &&
                            !double.IsNaN(stripLine.SegmentEndValue))
                        {
                            do
                            {
                                if (visibleRange.Inside(startStrip) ||
                                    visibleRange.Inside(startStrip + stripLine.Width))
                                {
                                    double y1 = Area.ValueToPoint(axis, startStrip);
                                    double y2 = y1 - stripLine.Width;

                                    if (axis.Area != null && axis.Area.InternalPrimaryAxis != null)
                                    {
                                        double startVal = Area.ValueToPoint(xAxis,
                                            stripLine.SegmentStartValue);
                                        double endVal = Area.ValueToPoint(xAxis,
                                            stripLine.SegmentEndValue);

                                        stripRect = new Rect(new Point(startVal, y1),
                                            new Point(endVal, y2));
                                        RenderStripLine(stripRect, stripLine);
                                    }
                                }

                                startStrip += periodStrip;
                            } while (periodStrip != 0 && (startStrip < endStrip));
                        }
                    }
                }
            }
        }

        internal void UpdateStripLines()
        {
            if (Area is SfChart3D) return;
            stripLines.Clear();
            foreach (
                var axis in
                    Area.Axes.Where(
                        axis =>
                            axis != null && !axis.VisibleRange.IsEmpty && (axis as ChartAxisBase2D).StripLines != null))
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    UpdateHorizontalStripLine(axis as ChartAxisBase2D);
                }
                else
                {
                    UpdateVerticalStripLine(axis as ChartAxisBase2D);
                }
            }
        }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public double Left
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public double Top
        {
            get;
            set;
        }

        #endregion
    }
}
