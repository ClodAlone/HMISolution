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
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a column definition.
    /// </summary>
    /// <remarks>
    /// The width of the row can be defined either in terms of fixed pixels units mode or in auto adjust mode, by using <see cref="ChartColumnDefinition.Unit"/> property.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ChartColumnDefinition: DependencyObject, ICloneable
    {
        private List<ChartAxis> axis;

        private double computedWidth = 0;

        internal double ComputedWidth
        {
            get { return computedWidth; }
            set { computedWidth = value; }
        }

        private double computedLeft = 0;

        internal double ComputedLeft
        {
            get { return computedLeft; }
            set { computedLeft = value; }
        }

        /// <summary>
        /// Called when instance created for ChartColumnDefinition
        /// </summary>
        public ChartColumnDefinition()
        {
            axis = new List<ChartAxis>();
        }

        /// <summary>
        /// Gets or Sets width of this column.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartColumnDefinition), new PropertyMetadata(1d));

        /// <summary>
        /// Gets or Sets unit of the value specified in Width.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartUnitType Unit
        {
            get { return (ChartUnitType)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(ChartUnitType), typeof(ChartColumnDefinition), new PropertyMetadata(ChartUnitType.Star));

        /// <summary>
        /// Gets or Sets thickness of the border
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SplitterLineThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(double), typeof(ChartColumnDefinition), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or Sets border stroke.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush BorderStroke
        {
            get { return (Brush)GetValue(BorderStrokeProperty); }
            set { SetValue(BorderStrokeProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for SplitterStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BorderStrokeProperty =
            DependencyProperty.Register("BorderStroke", typeof(Brush), typeof(ChartColumnDefinition), new PropertyMetadata(new SolidColorBrush(Colors.Red)));


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

        internal void Measure(Size size, List<double> nearSizes, List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;

            bool isOpposedFirstElement = true;
            bool isFirstElement = true;
            double innerPadding = 0;
            double axisWidth = 0;

            foreach (ChartAxis content in axis)
            {
                if (content != null)
                {
                    int columnSpan = content.Area!=null ? content.Area.GetActualColumnSpan(content):0;
                        axisWidth = CalcColumnSpanAxisWidth(size.Width, content, columnSpan);

                    if(content.Area!=null  && content.Area.GetActualColumn(content) == content.Area.ColumnDefinitions.IndexOf(this))
                        content.ComputeDesiredSize(new Size(axisWidth,size.Height));

                    if (content.Area != null && content.Area.InternalPrimaryAxis == content
                        && content.ShowAxisNextToOrigin && content.Area.RowDefinitions.Count <= 1)
                    {
                        double value = content.Area.InternalSecondaryAxis.ValueToCoefficientCalc(content.Origin);
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
                            farSizes.Add(content.ComputedDesiredSize.Height - innerPadding);
                        }
                        else if (farSizes[farIndex] < (content.ComputedDesiredSize.Height - innerPadding))
                        {
                            farSizes[farIndex] = content.ComputedDesiredSize.Height - innerPadding;
                        }
                        farIndex++;
                        isOpposedFirstElement = false;
                    }
                    else
                    {
                        innerPadding = isFirstElement ? content.InsidePadding : 0;
                        if (nearSizes.Count <= nearIndex)
                        {
                            nearSizes.Add(content.ComputedDesiredSize.Height - innerPadding);
                        }
                        else if (nearSizes[nearIndex] < (content.ComputedDesiredSize.Height - innerPadding))
                        {
                            nearSizes[nearIndex] = content.ComputedDesiredSize.Height - innerPadding;
                        }
                        nearIndex++;
                        isFirstElement = false;
                    }
                }
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
                    if (content.DockPosition == ChartDock.Top)
                    {
                        if (farSizes.Count <= farIndex)
                        {
                            farSizes.Add(content.DesiredSize.Height);
                        }
                        else if (farSizes[farIndex] < (content.DesiredSize.Height))
                        {
                            farSizes[farIndex] = content.DesiredSize.Height;
                        }
                        farIndex++;
                    }
                    else
                    {
                        if (nearSizes.Count <= nearIndex)
                        {
                            nearSizes.Add(content.DesiredSize.Height);
                        }
                        else if (nearSizes[nearIndex] < (content.DesiredSize.Height))
                        {
                            nearSizes[nearIndex] = content.DesiredSize.Height;
                        }
                        nearIndex++;
                    }
                }
            }
        }


        internal void UpdateLegendsArrangeRect(double left, double width, double areaHeight, List<double> nearSizes,
                                      List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;
            double nearTotalSize = nearSizes.Sum();
            double farTotalSize = farSizes.Sum();
            double sum = nearTotalSize;
            double axisWidth = 0;
            for (int i = 0; i < Legends.Count; i++)
            {
                var element = this.Legends[i];
                if (element != null && element.DockPosition != ChartDock.Floating && element.LegendPosition == LegendPosition.Outside)
                {
                    //Set ColumnSpan width value
                    axisWidth = element.XAxis != null && element.ChartArea!=null ? CalcColumnSpanAxisWidth(width, element.XAxis, element.ChartArea.GetActualColumnSpan(element)) : width;
                    var desiredSize = element.DesiredSize;
                    if (element.DockPosition == ChartDock.Bottom)
                    {
                        element.ArrangeRect = new Rect(left, (areaHeight + sum - nearTotalSize), axisWidth,
                                                     desiredSize.Height);
                        if (nearIndex < nearSizes.Count)
                        nearTotalSize -= nearSizes[nearIndex];
                        nearIndex++;
                    }
                    else
                    {
                        element.ArrangeRect = new Rect(left, (farTotalSize - desiredSize.Height),
                                                      axisWidth, desiredSize.Height);
                        if (farIndex < farSizes.Count)
                            farTotalSize -= farSizes[farIndex];

                        farIndex++;
                    }
                }
            }
        }

        //Calculate ColumnSpan width value
        private double CalcColumnSpanAxisWidth(double width,ChartAxis axis,int columnSpan)
        {
            int column = axis.Area != null ? axis.Area.GetActualColumn(axis) : 0;
            if (axis.Area != null && columnSpan > 1 && column == axis.Area.ColumnDefinitions.IndexOf(this))
            {
                var cols = axis.Area.ColumnDefinitions;
                int j = cols.IndexOf(this),i=0;
                width = 0;
                for (; j < cols.Count; j++)
                {
                    if (i < columnSpan)
                    {
                        width += cols[j].ComputedWidth;
                        i++;
                    }
                }
            }
            return width;
        }
       
        internal void UpdateArrangeRect(double left, double width, double areaHeight, List<double> nearSizes,
                                        List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;
            bool isOpposedFirstElement = true;
            bool isFirstElement = true;
            double nearTotalSize = nearSizes.Sum();
            double farTotalSize = farSizes.Sum();
            double innerPadding = 0;
            double axisWidth = 0;
            int actualColumnIndex = 0;
            int elementColumnIndex = 0;

            for (int i = 0; i < Axis.Count; i++)
            {
                ChartAxis element = this.Axis[i];
                if (element != null)
                {
                    //Set ColumnSpan width value
                    if (element.Area!=null)
                    {
                    axisWidth =CalcColumnSpanAxisWidth(width, element, element.Area.GetActualColumnSpan(element));
                    actualColumnIndex =element.Area.ColumnDefinitions.IndexOf(this);
                    elementColumnIndex =element.Area.GetActualColumn(element);
                    }
                    Size desiredSize = element.ComputedDesiredSize;
                    try
                    {
                        if (element.Area != null && element.Area.InternalPrimaryAxis == element
                            && element.ShowAxisNextToOrigin && element.Area.RowDefinitions.Count <= 1)
                        {
                            ChartAxis secondaryAxis = element.Area.InternalSecondaryAxis;
                            double value = secondaryAxis.ValueToCoefficientCalc(element.Origin);
                            double plotOffset = secondaryAxis.ActualPlotOffset * 2;
                            if (0 < value && 1 > value)
                            {
                                if (element.OpposedPosition && elementColumnIndex == actualColumnIndex)
                                {
                                    element.ArrangeRect = new Rect(left,
                                                                   (((areaHeight - plotOffset)*(1 - value)) -
                                                                    desiredSize.Height) +
                                                                   element.InsidePadding,
                                                                   axisWidth, desiredSize.Height);
                                }
                                else if (elementColumnIndex == actualColumnIndex)
                                {
                                    element.ArrangeRect = new Rect(left,
                                                                   ((areaHeight - plotOffset)*(1 - value)) -
                                                                   element.InsidePadding,
                                                                   axisWidth,
                                                                   desiredSize.Height);
                                }
                                element.Measure(new Size(element.ArrangeRect.Width, element.ArrangeRect.Height));
                                continue;
                            }
                        }

                        if (element.OpposedPosition)
                        {
                            innerPadding = isOpposedFirstElement ? element.InsidePadding : 0;
                            if (elementColumnIndex == actualColumnIndex)
                            element.ArrangeRect = new Rect(left, (farTotalSize - desiredSize.Height) + innerPadding,
                                                           axisWidth, desiredSize.Height);
                            element.Measure(new Size(element.ArrangeRect.Width, element.ArrangeRect.Height));
                            farTotalSize -= farSizes[farIndex];
                            farIndex++;
                            isOpposedFirstElement = false;
                        }
                        else
                        {
                            innerPadding = isFirstElement ? element.InsidePadding : 0;
                            if (elementColumnIndex == actualColumnIndex)
                                element.ArrangeRect = new Rect(left, (areaHeight - nearTotalSize) - innerPadding, axisWidth,
                                                           desiredSize.Height);
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
            if (Axis != null && Axis.Count > 0)
            {
                ChartAxis element = this.Axis.FirstOrDefault();
                if (element.Area!=null)
                {
                BorderLine.X1 = BorderLine.X2 = element.ArrangeRect.Left - element.Area.SeriesClipRect.Left;
                BorderLine.Y1 = 0;
                BorderLine.Y2 = element.Area.SeriesClipRect.Height;
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
            return new ChartColumnDefinition()
            {
                BorderStroke = this.BorderStroke,
                BorderThickness = this.BorderThickness,
                Width = this.Width,
                Unit = this.Unit
            };
        }
    }
}
