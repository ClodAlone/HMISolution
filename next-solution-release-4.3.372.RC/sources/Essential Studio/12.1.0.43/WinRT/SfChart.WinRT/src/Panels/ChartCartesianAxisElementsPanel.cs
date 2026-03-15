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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.UI;
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
    /// Represents ChartCartesianAxisElementsPanel.
    /// </summary>
    /// <remarks>
    /// The elements inside the panel comprises of <see cref="ChartAxis"/> axis line,major ticklines and minor ticklines.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ChartCartesianAxisElementsPanel : ILayoutCalculator
    {
        #region fields

        Line mainAxisLine;

        UIElementsRecycler<Line> majorTicksRecycler;

        UIElementsRecycler<Line> minorTicksRecycler;

        private Size desiredSize;

        private Panel labelsPanels;

        #endregion

        #region properties


        public double Left
        {
            get;
            set;
        }

        public double Top
        {
            get;
            set;
        }

        public Panel Panel
        {
            get { return labelsPanels; }
        }

        ChartAxis axis;
        internal ChartAxis Axis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = value;
                SetAxisLineBinding();
            }
        }

        private void SetAxisLineBinding()
        {
            Binding binding = new Binding();
            binding.Source = axis;
            binding.Path = new PropertyPath("AxisLineStyle");
            mainAxisLine.SetBinding(Line.StyleProperty, binding);
        }

        /// <summary>
        /// Gets the desired size of the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Size DesiredSize
        {
            get
            {
                return desiredSize;
            }
        }

        /// <summary>
        /// Gets the Children count in the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public List<UIElement> Children
        {
            get
            {
                if (labelsPanels != null)
                {
                    return labelsPanels.Children.Cast<UIElement>().ToList();
                }

                return null;
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartCartesianAxisElementsPanel
        /// </summary>
        /// <param name="panel"></param>
        public ChartCartesianAxisElementsPanel(Panel panel)
        {
            this.labelsPanels = panel;
            mainAxisLine = new Line();
            if (panel != null)
                panel.Children.Add(mainAxisLine);
            majorTicksRecycler = new UIElementsRecycler<Line>(panel);
            minorTicksRecycler = new UIElementsRecycler<Line>(panel);
        }

        #endregion

        #region methods

        /// <summary>
        /// Method declaration for Measure
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public Size Measure(Size availableSize)
        {
            Size size = Size.Empty;
            double smallTickLineSize = Axis.Area is SfChart3D ? Axis is CategoryAxis3D ? 5 : (Axis as RangeAxisBase3D).SmallTickLineSize : (Axis is CategoryAxis|| Axis is DateTimeCategoryAxis)  ? 5 : (Axis as RangeAxisBase).SmallTickLineSize;
            if (Axis.Orientation == Orientation.Horizontal)
            {
                size = new Size(availableSize.Width, Math.Max(Math.Max(Axis.TickLineSize, smallTickLineSize), 0) + mainAxisLine.StrokeThickness);
            }
            else
            {
                size = new Size(Math.Max(Math.Max(Axis.TickLineSize, smallTickLineSize), 0) + mainAxisLine.StrokeThickness, availableSize.Height);
            }

            desiredSize = size;

            return size;
        }

        public void DetachElements()
        {
            if (mainAxisLine != null && Children != null
                && Children.Contains(mainAxisLine))
                Children.Remove(mainAxisLine);
            if(majorTicksRecycler != null)
                majorTicksRecycler.Clear();

            if(minorTicksRecycler != null)
                minorTicksRecycler.Clear();

            labelsPanels = null;
        }

        /// <summary>
        /// Method declaration for Arrange
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public Size Arrange(Size finalSize)
        {
            double[] values = (from val in Axis.VisibleLabels
                               select val.Position).ToArray();
           
            if (Axis.Area is SfChart)
            {
                RenderAxisLine(finalSize);
                if (Axis is CategoryAxis && (Axis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks)
                    values = (from val in Axis.SmallTickPoints
                              select val).ToArray();
                RenderTicks(finalSize, majorTicksRecycler, Axis.Orientation, Axis.TickLineSize, Axis.TickLinesPosition, values);

                if (Axis.smallTicksRequired)
                {
                    values = (from val in Axis.SmallTickPoints
                              select val).ToArray();
                    RenderTicks(finalSize, minorTicksRecycler, Axis.Orientation, (Axis as RangeAxisBase).SmallTickLineSize, (Axis as RangeAxisBase).SmallTickLinesPosition, values);
                }
            }
            else
            {
                RenderAxisLine3D(finalSize);
                if (Axis is CategoryAxis3D && (Axis as CategoryAxis3D).LabelPlacement == LabelPlacement.BetweenTicks)
                    values = (from val in Axis.SmallTickPoints
                              select val).ToArray();

                RenderTicks3D(majorTicksRecycler, Axis.Orientation, Axis.TickLineSize, Axis.TickLinesPosition, values);

                if (Axis.smallTicksRequired)
                {
                    values = (from val in Axis.SmallTickPoints
                              select val).ToArray();
                    RenderTicks3D(minorTicksRecycler, Axis.Orientation, (Axis as RangeAxisBase3D).SmallTickLineSize, (Axis as RangeAxisBase3D).SmallTickLinesPosition, values);
                }
            }
            return finalSize;
        }


        internal void UpdateTicks()
        {
            int tickCount = Axis.SmallTickPoints.Count;
            if (!(Axis is CategoryAxis && (Axis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks))
                tickCount = Axis.VisibleLabels.Count;

            UpdateTicks(tickCount, majorTicksRecycler, "MajorTickLineStyle");

            if (Axis.smallTicksRequired)
                UpdateTicks(Axis.SmallTickPoints.Count, minorTicksRecycler, "MinorTickLineStyle");
        }

        private void UpdateTicks(int linescount, UIElementsRecycler<Line> lineRecycler, String lineStylePath)
        {
            int linesCount = linescount;

            if (!lineRecycler.BindingProvider.Keys.Contains(Line.StyleProperty))
            {
                Binding binding = new Binding();
                binding.Source = Axis;
                binding.Path = new PropertyPath(lineStylePath);
                lineRecycler.BindingProvider.Add(Line.StyleProperty, binding);
            }

            lineRecycler.GenerateElements(linesCount);
        }

        private void RenderAxisLine(Size finalSize)
        {
            double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            double width = finalSize.Width;
            double height = finalSize.Height;

            Orientation orientaion = Axis.Orientation;
            bool isOpposed = Axis.OpposedPosition ^ Axis.TickLinesPosition == AxisElementPosition.Inside;

            if (orientaion == Orientation.Horizontal)
            {
                x1 = this.Axis.AxisLineOffset;
                x2 = width - this.Axis.AxisLineOffset;
                y1 = y2 = isOpposed ? height : 0;
            }
            else
            {
                x1 = x2 = isOpposed ? 0 : width;
                y1 = this.Axis.AxisLineOffset;
                y2 = height - this.Axis.AxisLineOffset;
            }

            if (mainAxisLine != null)
            {
                mainAxisLine.X1 = x1;
                mainAxisLine.Y1 = y1;
                mainAxisLine.X2 = x2;
                mainAxisLine.Y2 = y2;
            }
        }

        private void RenderAxisLine3D(Size finalSize)
        {
            double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            double width = axis.ArrangeRect.Width;
            double height = axis.ArrangeRect.Height;

            Orientation orientaion = Axis.Orientation;
            bool isOpposed = Axis.OpposedPosition;
          
            if (orientaion == Orientation.Horizontal)
            {
                var pos = 0d;
                if (Axis.TickLinesPosition == AxisElementPosition.Inside)
                    pos = Axis.TickLineSize;
                if (Axis.LabelsPosition == AxisElementPosition.Inside)
                    pos += axis.axisLabelsPanel.DesiredSize.Height;
                x1 = Axis.AxisLineOffset + Axis.ArrangeRect.Left;
                x2 = width - Axis.AxisLineOffset + Axis.ArrangeRect.Left;
                y1 = y2 = isOpposed ? height + axis.ArrangeRect.Top - pos : axis.ArrangeRect.Top + pos;
            }
            else
            {
                var pos = 0d;
                if (Axis.TickLinesPosition == AxisElementPosition.Inside)
                    pos = Axis.TickLineSize;
                if (Axis.LabelsPosition == AxisElementPosition.Inside)
                    pos += axis.axisLabelsPanel.DesiredSize.Width;
                x1 = x2 = isOpposed ? axis.ArrangeRect.Left + pos: width + axis.ArrangeRect.Left - pos;
                y1 = Axis.AxisLineOffset + axis.ArrangeRect.Top;
                y2 = height - Axis.AxisLineOffset + axis.ArrangeRect.Top;
            }

            if (mainAxisLine != null)
            {
                var area = (SfChart3D)Axis.Area;
                ((SfChart3D)Axis.Area).Graphics3D.AddVisual(Polygon3D.CreateLine(mainAxisLine, x1, y1, x2, y2, 0));
            }
        }

        private void RenderTicks(Size finalSize, UIElementsRecycler<Line> linesRecycler, Orientation orientation
             , double tickSize, AxisElementPosition tickPosition, double[] Values)
        {

            int labelsCount = Values.Length;
            int linesCount = linesRecycler.Count;
            double width = finalSize.Width;
            double height = finalSize.Height;

            for (int i = 0; i < labelsCount; i++)
            {
                if (i < linesCount)
                {
                    double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                    Line line = linesRecycler[i];
                    double value = this.Axis.ValueToCoefficientCalc(Values[i]);
                    value = double.IsNaN(value) ? 0 : value;

                    if (orientation == Orientation.Horizontal)
                    {
                        x1 = x2 = Math.Round(Axis.ActualPlotOffset + (this.Axis.RenderedRect.Width * value));
                    }
                    else
                    {
                        y1 = y2 = Math.Round(Axis.ActualPlotOffset + (this.Axis.RenderedRect.Height * (1 - value)));
                    }

                    CalculatePosition(tickPosition, tickSize, width, height, ref x1, ref y1, ref x2, ref y2);

                    line.X1 = x1;
                    line.X2 = x2;
                    line.Y1 = y1;
                    line.Y2 = y2;
                }
            }
        }

        private void CalculatePosition(AxisElementPosition ticksPosition, double tickSize, double width, double height
                                   , ref double x1, ref double y1, ref double x2, ref double y2)
        {
            Orientation orientaion = Axis.Orientation;
            bool isOpposed = Axis.OpposedPosition;

            if (orientaion == Orientation.Horizontal)
            {
                switch (ticksPosition)
                {
                    case AxisElementPosition.Inside:
                        y1 = isOpposed ? mainAxisLine.StrokeThickness : 0;
                        y2 = isOpposed ? y1 + tickSize : tickSize;
                        break;
                    case AxisElementPosition.Outside:
                        y1 = isOpposed ? 0 : mainAxisLine.StrokeThickness;
                        y2 = isOpposed ? tickSize : y1 + tickSize;
                        break;
                }
            }
            else
            {
                switch (ticksPosition)
                {
                    case AxisElementPosition.Inside:
                        x1 = isOpposed ? 0 : mainAxisLine.StrokeThickness;
                        x2 = isOpposed ? tickSize : x1 + tickSize;
                        break;
                    case AxisElementPosition.Outside:
                        x1 = isOpposed ? mainAxisLine.StrokeThickness : 0;
                        x2 = isOpposed ? x1 + tickSize : tickSize;
                        break;
                }
            }
        }

        private void RenderTicks3D(UIElementsRecycler<Line> linesRecycler, Orientation orientation
              , double tickSize, AxisElementPosition tickPosition, IList<double> values)
        {
            var labelsCount = values.Count;
            var linesCount = linesRecycler.Count;

            for (var i = 0; i < labelsCount; i++)
            {
                if (i >= linesCount) continue;
                double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                var line = linesRecycler[i];
                var value = Axis.ValueToCoefficientCalc(values[i]);
                value = double.IsNaN(value) ? 0 : value;

                if (orientation == Orientation.Horizontal)
                {
                    x1 = x2 = Math.Round(Axis.ActualPlotOffset + (Axis.RenderedRect.Width * value));
                }
                else
                {
                    y1 = y2 = Math.Round(Axis.ActualPlotOffset + (Axis.RenderedRect.Height * (1 - value)));
                }

                CalculatePosition3D(tickPosition, tickSize, ref x1, ref y1, ref x2, ref y2);

                ((SfChart3D)Axis.Area).Graphics3D.AddVisual(Polygon3D.CreateLine(line, x1, y1, x2, y2, 0));
            }
        }

        private void CalculatePosition3D(AxisElementPosition ticksPosition, double tickSize,ref double x1, ref double y1, ref double x2, ref double y2)
        {
            var orientation = Axis.Orientation;
            var isOpposed = Axis.OpposedPosition;

            if (orientation == Orientation.Horizontal)
            {
                switch (ticksPosition)
                {
                    case AxisElementPosition.Inside:
                        y1 = isOpposed ? mainAxisLine.StrokeThickness : 0;
                        y2 = isOpposed ? y1 + tickSize : tickSize;
                        break;
                    case AxisElementPosition.Outside:
                        y1 = isOpposed ? 0 : mainAxisLine.StrokeThickness;
                        y2 = isOpposed ? tickSize : y1 + tickSize;
                        break;
                }
                var screenPositionTop = axis.ArrangeRect.Top + Top;
                var screenPositionLeft = axis.ArrangeRect.Left;
                y1 += screenPositionTop;
                y2 += screenPositionTop;

                x1 += screenPositionLeft;
                x2 += screenPositionLeft;
            }
            else
            {
                switch (ticksPosition)
                {
                    case AxisElementPosition.Inside:
                        x1 = isOpposed ? 0 : mainAxisLine.StrokeThickness;
                        x2 = isOpposed ? tickSize : x1 + tickSize;                       
                        break;
                    case AxisElementPosition.Outside:
                        x1 = isOpposed ? mainAxisLine.StrokeThickness : 0;
                        x2 = isOpposed ? x1 + tickSize : tickSize;
                        break;
                }
                var screenPositionLeft = axis.ArrangeRect.Left + Left;
                var screenPositionTop = axis.ArrangeRect.Top;
                x1 += screenPositionLeft;
                x2 += screenPositionLeft;

                y1 += screenPositionTop;
                y2 += screenPositionTop;
            }
        }

        /// <summary>
        /// Method declaration for UpdateElements
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public void UpdateElements()
        {
            UpdateTicks();
        }

        #endregion
    }
}
