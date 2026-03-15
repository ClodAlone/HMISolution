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
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a row definition.
    /// </summary>
    /// <remarks>
    /// The height of the row can be defined either in terms of fixed pixels units mode or auto adjust mode, by using <see cref="ChartRowDefinition.Unit"/> property.
    /// </remarks>
    public class ChartRowDefinition : DependencyObject,ICloneable
    {
        private List<ChartAxis> axis;

        private double computedHeight = 0;

        internal double ComputedHeight
        {
            get { return computedHeight; }
            set { computedHeight = value; }
        }

        private double computedTop = 0;

        internal double ComputedTop
        {
            get { return computedTop; }
            set { computedTop = value; }
        }

        /// <summary>
        /// Called when instance created for ChartRowdefinitions
        /// </summary>
        public ChartRowDefinition()
        {
            axis = new List<ChartAxis>();
        }

        /// <summary>
        /// Get or Set RowTap property
        /// </summary>
        public double RowTop
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or Sets height of this row.
        /// </summary>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ChartRowDefinition), new PropertyMetadata(1d));

        /// <summary>
        /// Gets or Sets unit of the value specified in Height.
        /// </summary>
        public ChartUnitType Unit
        {
            get { return (ChartUnitType)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(ChartUnitType), typeof(ChartRowDefinition), new PropertyMetadata(ChartUnitType.Star));

        /// <summary>
        /// Gets or Sets thickness of the border.
        /// </summary>
        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SplitterLineThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(double), typeof(ChartRowDefinition), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or Sets border stroke.
        /// </summary>
        public Brush BorderStroke
        {
            get { return (Brush)GetValue(BorderStrokeProperty); }
            set { SetValue(BorderStrokeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SplitterStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BorderStrokeProperty =
            DependencyProperty.Register("BorderStroke", typeof(Brush), typeof(ChartRowDefinition), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        internal Line BorderLine;

        internal List<ChartAxis> Axis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = value;
            }
        }

        private List<ChartLegend> legends = new List<ChartLegend>();

        internal List<ChartLegend> Legends
        {
            get
            {
                return legends;
            }
            set
            {
                legends = value;
            }
        }

        internal void MeasureLegends(Size size, List<double> nearSizes, List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;
            foreach (ChartLegend content in Legends)
            {
                if (content != null && content.DockPosition != ChartDock.Floating && content.LegendPosition == LegendPosition.Outside)
                {
                    if (content.DesiredSize.Width == 0d)
                        content.Measure(size);
                    if (content.DockPosition == ChartDock.Left)
                    {
                        if (nearSizes.Count <= nearIndex)
                        {
                            nearSizes.Add(content.DesiredSize.Width);
                        }
                        else if (nearSizes[nearIndex] < (content.DesiredSize.Width))
                        {
                            nearSizes[nearIndex] = content.DesiredSize.Width;
                        }
                        nearIndex++;
                    }
                    else
                    {
                        if (farSizes.Count <= farIndex)
                        {
                            farSizes.Add(content.DesiredSize.Width);
                        }
                        else if (farSizes[farIndex] < (content.DesiredSize.Width))
                        {
                            farSizes[farIndex] = content.DesiredSize.Width;
                        }
                        farIndex++;
                    }
                }
            }
        }

        internal void Measure(Size size, List<double> nearSizes, List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;
            bool isOpposedFirstElement = true;
            bool isFirstElement = true;
            double innerPadding = 0;
            double axisHeight = 0,top=0;

            foreach (ChartAxis content in axis)
            {
                if (content != null)
                {

                    if (content.Area != null)
                    {
                        CalcRowSpanAxisWidth_and_Top(top, content.Area.GetActualRowSpan(content), size.Height, content, out top, out axisHeight);
                        if (content.Area.GetActualRow(content) == content.Area.RowDefinitions.IndexOf(this))
                            content.ComputeDesiredSize(new Size(size.Width, axisHeight));
                    }
                    if (content.Area != null && content.Area.InternalSecondaryAxis == content
                        && content.ShowAxisNextToOrigin && content.Area.ColumnDefinitions.Count <= 1)
                    {
                        double value = content.Area.InternalPrimaryAxis.ValueToCoefficientCalc(content.Origin);
                        if (0 < value && 1 > value)
                        {
                            continue;
                        }
                    }
                    if (content.OpposedPosition)
                    {
                        innerPadding = isOpposedFirstElement ? content.InsidePadding : 0;
                        if (farSizes.Count <= farIndex)
                        {
                            farSizes.Add(content.ComputedDesiredSize.Width - innerPadding);
                        }
                        else if (farSizes[farIndex] < (content.ComputedDesiredSize.Width - innerPadding))
                        {
                            farSizes[farIndex] = content.ComputedDesiredSize.Width - innerPadding;
                        }
                        farIndex++;
                        isOpposedFirstElement = false;
                    }
                    else
                    {
                        innerPadding = isFirstElement ? content.InsidePadding : 0;
                        if (nearSizes.Count <= nearIndex)
                        {
                            nearSizes.Add(content.ComputedDesiredSize.Width - innerPadding);
                        }
                        else if (nearSizes[nearIndex] < (content.ComputedDesiredSize.Width - innerPadding))
                        {
                            nearSizes[nearIndex] = content.ComputedDesiredSize.Width - innerPadding;
                        }
                        nearIndex++;
                        isFirstElement = false;
                    }
                }
            }
        }

        internal void UpdateLegendArrangeRect(double top, double height, double areaWidth, List<double> nearSizes, List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;
            double nearTotalSize = nearSizes.Sum();
            double farTotalSize = farSizes.Sum();
            double sum = farTotalSize;
            double axisheight = height;
            double axistop = top;
            for (int i = 0; i < Legends.Count; i++)
            {
                ChartLegend element = Legends[i];
                if (element != null && element.DockPosition != ChartDock.Floating && element.LegendPosition == LegendPosition.Outside)
                {
                    //Set RowSpan height and top value
                    if(element.ChartArea!=null)
                    CalcRowSpanAxisWidth_and_Top(top, element.ChartArea.GetActualRowSpan(element), height, element.YAxis, out axistop, out axisheight);
                    var desiredSize = element.DesiredSize;
                    if (element.DockPosition == ChartDock.Left)
                    {
                        element.ArrangeRect = new Rect((nearTotalSize - desiredSize.Width), axistop,
                                                       desiredSize.Width,
                                                       axisheight);
                        nearTotalSize -= nearSizes[nearIndex];
                        nearIndex++;
                    }
                    else
                    {
                        element.ArrangeRect = new Rect(((areaWidth + sum) - farTotalSize), axistop,
                                                       desiredSize.Width, axisheight);
                        farTotalSize -= farSizes[farIndex];
                        farIndex++;
                    }
                }
            }
        }

        //Calculate rowspan height and top value
        private void CalcRowSpanAxisWidth_and_Top(double oldTop,int rowSpan, double oldHeight, ChartAxis axis, out double newTop, out double newHeight)
        {
            int row = axis.Area.GetActualRow(axis);
            if (axis.Area != null && rowSpan > 1 && row == axis.Area.RowDefinitions.IndexOf(this))
            {
                var rows = axis.Area.RowDefinitions;
                int j = rows.IndexOf(this), i = 0;
                newTop = 0;
                newHeight = 0;
                for (; j < rows.Count; j++)
                {
                    if (i < rowSpan)
                    {
                        newHeight += rows[j].computedHeight;
                        newTop = rows[j].ComputedTop;
                       i++;
                    }
                    
                }
            }
            else
            {
                newTop = oldTop;
                newHeight = oldHeight;
            }
        }
      
        internal void UpdateArrangeRect(double top, double height, double areaWidth, List<double> nearSizes, List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;
            bool isOpposedFirstElement = true;
            bool isFirstElement = true;
            double nearTotalSize = nearSizes.Sum();
            double farTotalSize = farSizes.Sum();
            double innerPadding = 0;
            double axisHeight = 0;
            double axisTop = 0;
            int actualRowIndex = 0,elementRowIndex = 0;
            for (int i = 0; i < Axis.Count; i++)
            {
                ChartAxis element = Axis[i];
                if (element != null)
                {
                    //Set RowSpan height and top value
                    if (element.Area!=null)
                    {
                        elementRowIndex =element.Area.GetActualRow(element);
                        actualRowIndex = element.Area.RowDefinitions.IndexOf(this);
                        CalcRowSpanAxisWidth_and_Top(top, element.Area.GetActualRowSpan(element), height, element, out axisTop, out axisHeight);
                    }
                   
                    Size desiredSize = element.ComputedDesiredSize;
                    try
                    {
                        if (element.Area != null && element.Area.InternalSecondaryAxis == element
                            && element.ShowAxisNextToOrigin && element.Area.ColumnDefinitions.Count <= 1)
                        {
                            double value = element.Area.InternalPrimaryAxis.ValueToCoefficientCalc(element.Origin);
                            double plotOffset = element.Area.InternalPrimaryAxis.ActualPlotOffset*2;
                            if (0 < value && 1 > value)
                            {
                                if (element.OpposedPosition && elementRowIndex == actualRowIndex)
                                {
                                    element.ArrangeRect = new Rect(((areaWidth - plotOffset) * value) - innerPadding, axisTop,
                                                                   desiredSize.Width, axisHeight);
                                }
                                else if (elementRowIndex == actualRowIndex)
                                {
                                    element.ArrangeRect =
                                        new Rect(((areaWidth - plotOffset)*value) - desiredSize.Width + innerPadding,
                                                 axisTop,
                                                 desiredSize.Width,
                                                 axisHeight);
                                }
                                element.Measure(new Size(element.ArrangeRect.Width, element.ArrangeRect.Height));
                                continue;
                            }
                        }

                        if (element.OpposedPosition)
                        {
                            innerPadding = isOpposedFirstElement ? element.InsidePadding : 0;
                            if (elementRowIndex == actualRowIndex)
                                element.ArrangeRect = new Rect((areaWidth - farTotalSize) - innerPadding, axisTop,
                                                           desiredSize.Width, axisHeight);
                            element.Measure(new Size(element.ArrangeRect.Width, element.ArrangeRect.Height));
                            farTotalSize -= farSizes[farIndex];
                            farIndex++;
                            isOpposedFirstElement = false;
                        }
                        else
                        {
                            innerPadding = isFirstElement ? element.InsidePadding : 0;
                            if (elementRowIndex == actualRowIndex)
                                element.ArrangeRect = new Rect((nearTotalSize - desiredSize.Width) + innerPadding, axisTop,
                                                           desiredSize.Width,
                                                           axisHeight);
                            element.Measure(new Size(element.ArrangeRect.Width, element.ArrangeRect.Height));
                            nearTotalSize -= nearSizes[nearIndex];
                            nearIndex++;
                            isFirstElement = false;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        internal void Arrange()
        {
            foreach (ChartAxis chartAxis in Axis)
            {
                Canvas.SetLeft(chartAxis, chartAxis.ArrangeRect.Left);
                Canvas.SetTop(chartAxis, chartAxis.ArrangeRect.Top);
            }

            RenderBorderLine();
        }

        private void RenderBorderLine()
        {
            if (BorderLine == null)
            {
                BorderLine = new Line();
                BindBorder(BorderLine);
            }
            if (Axis != null && this.Axis.Count > 0)
            {
                ChartAxis element = this.Axis.FirstOrDefault();
                if (element.Area != null)
                {
                    BorderLine.X1 = 0;
                    BorderLine.X2 = element.Area.SeriesClipRect.Width;
                    BorderLine.Y1 = BorderLine.Y2 = element.ArrangeRect.Top - element.Area.SeriesClipRect.Top;
                }
            }
        }

        private void BindBorder(UIElement element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("BorderStroke");
            BindingOperations.SetBinding(element, Line.StrokeProperty, binding);

            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("BorderThickness");
            BindingOperations.SetBinding(element, Line.StrokeThicknessProperty, binding);
        }

        public DependencyObject Clone()
        {
            return new ChartRowDefinition()
                {
                    BorderStroke = this.BorderStroke,
                    BorderThickness = this.BorderThickness,
                    Height = this.Height,
                    Unit = this.Unit
                };

        }
    }
}
