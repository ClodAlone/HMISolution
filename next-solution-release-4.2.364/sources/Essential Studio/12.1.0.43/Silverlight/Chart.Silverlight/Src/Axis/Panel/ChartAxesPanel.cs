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
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartAxesPanel
    /// </summary>
    public class ChartAxesPanel : Panel
    {

        #region for Merge
        ObservableCollection<DataAxis> m_dataAxes = new ObservableCollection<DataAxis>();
        /// <summary>
        /// Get or Set DataAxes property
        /// </summary>
        protected internal ObservableCollection<DataAxis> DataAxes
        {
            get
            {
                m_dataAxes.Clear();
                foreach (ChartAxis axis in this.Children.OfType<ChartAxis>())
                {
                    foreach (DataAxis label in axis.Items.OfType<DataAxis>())
                    {
                        m_dataAxes.Add(label);
                    }
                }

                return m_dataAxes;
            }
        }
        #endregion

        /// <summary>
        /// Called when instance created for ChartAxesPanel
        /// </summary>
        public ChartAxesPanel()
        {
            this.Right = this.Left = this.Top = this.Bottom = 0d;
            this.Loaded += new RoutedEventHandler(ChartAxesPanel_Loaded);
        }

        void ChartAxesPanel_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (ChartAxis axis in this.Children.OfType<ChartAxis>())
            {
                axis.GridLinesChanged += new PropertyChangedCallback(axis_GridLinesChanged);
            }
        }

        void axis_GridLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                this.InvalidateMeasure();
            }
        }

        internal ChartArea GetParentArea()
        {
            DependencyObject element = this;
            while (!(element is ChartArea))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as ChartArea;
            }

            return null;
        }

        ChartArea m_area = null;
        /// <summary>
        /// Get or Set ParentArea property
        /// </summary>
        public ChartArea ParentArea
        {
            get
            {
                if (m_area == null)
                {
                    m_area = this.GetParentArea();
                }

                return m_area;
            }
            internal set { m_area = value; }
        }
        /// <summary>
        /// Identifies the AxesThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesThicknessProperty =
DependencyProperty.Register("AxesThickness", typeof(Thickness), typeof(ChartAxesPanel), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Get or Set AxesThicknessProperty
        /// </summary>
        public Thickness AxesThickness
        {
            get
            {
                return (Thickness)GetValue(AxesThicknessProperty);
            }

            set
            {
                SetValue(AxesThicknessProperty, value);
            }
        }

        internal double Right
        {
            get;
            set;
        }

        internal double Left
        {
            get;
            set;
        }

        internal double Top
        {
            get;
            set;
        }

        internal double Bottom
        {
            get;
            set;
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
            availableSize = new Size(double.IsInfinity(availableSize.Width) ? 400 : availableSize.Width, double.IsInfinity(availableSize.Height) ? 400 : availableSize.Height);
            this.Right = this.Left = this.Top = this.Bottom = 0d;

            foreach (ChartAxis axis in this.Children.OfType<ChartAxis>())
            {
                axis.Measure(availableSize);

                if (axis.Orientation == Orientation.Horizontal && axis.OpposedPosition==false)
                {
                    this.Bottom += axis.DesiredSize.Height;
                }
                else if (axis.Orientation == Orientation.Horizontal && axis.OpposedPosition == true)
                {
                    this.Top += axis.DesiredSize.Height;
                }
                else if (axis.Orientation == Orientation.Vertical && axis.OpposedPosition == false)
                {
                    this.Left += axis.DesiredSize.Width;
                }
                else
                {
                    this.Right += axis.DesiredSize.Width;
                }
            }

            foreach (ChartAxis axis in this.Children.OfType<ChartAxis>())
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    axis.Margin = new Thickness(this.Left, 0, this.Right + (axis.Area != null && axis.Area.VerticalBar.Visibility == Visibility.Visible ? 17 : 0), 0);
                }
                else
                {
                    axis.Margin = new Thickness(0, this.Top, 0, this.Bottom + (axis.Area != null && axis.Area.HorizontalBar.Visibility == Visibility.Visible ? 17 : 0));
                }
               
            }

            if (this.ParentArea != null)
            {
                this.ParentArea.AxesThickness = new Thickness(this.Left, this.Top, this.Right, this.Bottom);
                //if (this.ParentArea.ChartAreaParent != null)
                //{
                //    double maxleft=0, maxright=0;
                //    foreach (ChartArea area in this.ParentArea.ChartAreaParent.Areas)
                //    {
                //        maxleft = Math.Max(maxleft, area.AxesThickness.Left);
                //        maxright = Math.Max(maxright, area.AxesThickness.Right);
                //    }
                //    foreach (ChartArea area in this.ParentArea.ChartAreaParent.Areas)
                //    {
                //        area.AxesThickness = new Thickness(maxleft, area.AxesThickness.Top, maxright, area.AxesThickness.Bottom);
                //    }
                //}
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
            double bottom = this.Bottom;
            double top = this.Top;
            double left = this.Left;
            double right = this.Right;
            
            foreach (ChartAxis axis in this.Children.OfType<ChartAxis>())
            {
                
                axis.LineStrokeThickness = axis.LineStrokeThickness == 0.5 ? 1 : axis.LineStrokeThickness; 
                if (!(axis.ChartAxesProvider is IChartCartesianAxes))
                {
                    axis.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }
                else if (axis.Orientation == Orientation.Horizontal && axis.OpposedPosition == false)
                {
                    axis.Arrange(new Rect(0, finalSize.Height - bottom - axis.LineStrokeThickness, axis.DesiredSize.Width, axis.DesiredSize.Height));
                    bottom -= axis.DesiredSize.Height;
                }
                else if (axis.Orientation == Orientation.Horizontal && axis.OpposedPosition == true)
                {
                    axis.Arrange(new Rect(0, top - axis.DesiredSize.Height, axis.DesiredSize.Width, axis.DesiredSize.Height));
                    top -= axis.DesiredSize.Height;
                }
                else if (axis.Orientation == Orientation.Vertical && axis.OpposedPosition == false)
                {
                    axis.Arrange(new Rect(left + axis.LineStrokeThickness - axis.DesiredSize.Width, 0, axis.DesiredSize.Width, axis.DesiredSize.Height));
                    left -= axis.DesiredSize.Width;
                }
                else
                {
                    axis.Arrange(new Rect(finalSize.Width - right, 0, axis.DesiredSize.Width, axis.DesiredSize.Height));
                    right -= axis.DesiredSize.Width;
                }
                DoLayout(axis);
            }

            return finalSize;
        }

        private void DoLayout(ChartAxis axis)
        {

            if (axis.Orientation == Orientation.Vertical)
            {
                switch (axis.ChartLabelPosition)
                {                    
                    case LabelPositions.Inside:
                        axis.LabelTransformX = (axis.DesiredSize.Width) -( ((axis.ChartTickLinesPosition == AxisPositions.Outside) ? axis.TickSize : 0) +( axis.ChartTickLinesPosition == AxisPositions.Cross ? (axis.TickSize * axis.ChartTickLinesRange) : 0));
                        if (axis.OpposedPosition)
                            axis.LabelTransformX = -axis.LabelTransformX;
                        break;
                    case LabelPositions.Outside:
                        axis.LabelTransformX = 0d;
                        break;
                }

                switch (axis.ChartTickLinesPosition)
                {
                    case AxisPositions.Inside:
                        axis.TickTransformX = axis.TickSize;
                        if (axis.OpposedPosition)
                            axis.TickTransformX = -axis.TickTransformX;
                        break;
                    case AxisPositions.Outside:
                        axis.TickTransformX = 0d;
                        break;
                    case AxisPositions.Cross:
                        axis.TickTransformX = axis.TickSize * axis.ChartTickLinesRange;
                        if (axis.OpposedPosition)
                            axis.TickTransformX = -axis.TickTransformX;
                        break;

                }

                switch (axis.HeaderPosition)
                {
                    case AxisPositions.Inside:
                        axis.HeaderTransformX = axis.DesiredSize.Width + (axis.ChartTickLinesPosition == AxisPositions.Inside ? axis.TickSize : 0) + (axis.ChartLabelPosition == LabelPositions.Inside ? axis.axisElementPanel.ActualWidth : 0);
                        if (axis.OpposedPosition)
                            axis.HeaderTransformX = -axis.HeaderTransformX;
                        break;
                    case AxisPositions.Outside:
                        axis.HeaderTransformX = 0d;
                        break;
                    case AxisPositions.Cross:                        
                        TextBlock block = new TextBlock();
                        block.Text = axis.Header.ToString();
                        axis.HeaderTransformX = axis.DesiredSize.Width- (block.ActualHeight/2);
                        if (axis.OpposedPosition)
                            axis.HeaderTransformX = -axis.HeaderTransformX;
                        break;
                }

                switch (axis.AxisLabels)
                {
                    case AxisLabels.High:
                        //axis.OpposedPosition = false; Commented for Automation Break MT1332
                        axis.LabelTransformX = axis.Area.seriesGrid.ActualWidth + axis.LabelTransformX;
                        axis.TickTransformX = axis.Area.seriesGrid.ActualWidth + axis.TickTransformX;
                        break;
                    case AxisLabels.NextToAxis:
                        //axis.OpposedPosition = false;
                        axis.LabelTransformX = axis.LabelTransformX + axis.Area.ValueToPoint(axis.Area.PrimaryAxis, axis.Origin);
                        axis.TickTransformX = axis.TickTransformX + axis.Area.ValueToPoint(axis.Area.PrimaryAxis, axis.Origin);
                        break;
                    case AxisLabels.Low:
                        //axis.OpposedPosition = false;
                        break;
                }
            }
            else if (axis.Orientation == Orientation.Horizontal)
            {
                #region HorizontalAxis
                switch (axis.ChartLabelPosition)
                {
                    case LabelPositions.Inside:
                        if (axis.axisElementPanel != null)
                        {
                            axis.LabelTransformY = -((axis.DesiredSize.Height -( ((axis.ChartTickLinesPosition == AxisPositions.Outside) ? axis.TickSize : 0) + ((axis.ChartTickLinesPosition == AxisPositions.Cross) ? (axis.TickSize * axis.ChartTickLinesRange) : 0))) - axis.TickSize) -(axis.axisElementPanel.ActualHeight/2);
                            if (axis.OpposedPosition)
                                axis.LabelTransformY = -axis.LabelTransformY;
                        }
                        break;
                    case LabelPositions.Outside:
                        axis.LabelTransformY = 0d;
                        break;                        
                }
                
                switch (axis.ChartTickLinesPosition)
                {
                    case AxisPositions.Inside:
                        axis.TickTransformY = -axis.TickSize;
                        if (axis.OpposedPosition)
                            axis.TickTransformY = -axis.TickTransformY;
                        break;
                    case AxisPositions.Outside:
                        axis.TickTransformY = 0d;
                        break;
                    case AxisPositions.Cross:
                        axis.TickTransformY =-( axis.TickSize * axis.ChartTickLinesRange);
                        if (axis.OpposedPosition)
                            axis.TickTransformY = -axis.TickTransformY;
                        break;
                }

                switch (axis.HeaderPosition)
                {
                    case AxisPositions.Inside:
                        axis.HeaderTransformY = -(axis.DesiredSize.Height + (axis.ChartTickLinesPosition == AxisPositions.Inside ? axis.TickSize : 0) + (axis.ChartLabelPosition == LabelPositions.Inside ? axis.axisElementPanel.ActualHeight : 0));
                        if (axis.OpposedPosition)
                            axis.HeaderTransformY = -axis.HeaderTransformY;
                        break;
                    case AxisPositions.Outside:
                        axis.HeaderTransformY = 0d;
                        break;
                    case AxisPositions.Cross:
                        TextBlock block = new TextBlock();
                        block.Text = axis.Header.ToString();
                        axis.HeaderTransformY = -(axis.DesiredSize.Height - (block.ActualHeight / 2));
                        if (axis.OpposedPosition)
                            axis.HeaderTransformY = -axis.HeaderTransformY;
                        break;
                }

                switch (axis.AxisLabels)
                {
                    case AxisLabels.High:
                        //axis.OpposedPosition = false;
                        axis.LabelTransformY = axis.LabelTransformY - axis.Area.seriesGrid.ActualHeight;
                        axis.TickTransformY = axis.TickTransformY - axis.Area.seriesGrid.ActualHeight;
                        break;
                    case AxisLabels.NextToAxis:
                        //axis.OpposedPosition = false;
                        axis.LabelTransformY = axis.LabelTransformY - (axis.Area.seriesGrid.ActualHeight - axis.Area.ValueToPoint(axis.Area.SecondaryAxis, axis.Origin));
                        axis.TickTransformY = axis.TickTransformY - (axis.Area.seriesGrid.ActualHeight - axis.Area.ValueToPoint(axis.Area.SecondaryAxis, axis.Origin));
                        break;
                    case AxisLabels.Low:
                        //axis.OpposedPosition = false;
                        break;
                }
                #endregion

            }
        }
    }
}
