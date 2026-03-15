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
using System.Collections.Generic;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    ///Class implementation for ChartAxisElementPanel
    /// </summary>
    public class ChartAxisElementPanel : Panel
    {
        /// <summary>
        /// Called when instance created for ChartAxisElementPanel
        /// </summary>
        public ChartAxisElementPanel()
        {
            this.sumWidth = this.sumHeight = 0d;
            this.Orientation = Orientation.Horizontal;
            this.OpposedPosition = false;
        }

        internal Size MaxSize
        {
            get;
            set;
        }
        

        internal Orientation Orientation
        {
            get;
            set;
        }

        internal bool OpposedPosition
        {
            get;
            set;
        }

        internal double sumWidth
        {
            get;
            set;
        }

        internal double sumHeight
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
            DependencyObject obj = VisualTreeHelper.GetParent(this);
            Grid obj1 = VisualTreeHelper.GetParent(obj) as Grid;
            StackPanel panel = VisualTreeHelper.GetParent(obj1) as StackPanel;

            Size totalSize = new Size(double.IsInfinity(availableSize.Width) ? 300 : availableSize.Width, double.IsInfinity(availableSize.Height) ? 300 : availableSize.Height);

            this.sumWidth = this.sumHeight = 0d;


            foreach (UIElement element in this.Children)
            {
                element.Measure(totalSize);

                DataAxis data = element as DataAxis;
                if (data != null && data.RelatedAxis != null)
                {
                    if (data.RelatedAxis.Orientation == Orientation.Horizontal)
                    {
                        this.sumWidth += element.DesiredSize.Width;
                        this.sumHeight = this.sumHeight < element.DesiredSize.Height ? element.DesiredSize.Height : this.sumHeight;
                    }
                    else
                    {
                        this.sumWidth = this.sumWidth < element.DesiredSize.Width ? element.DesiredSize.Width : this.sumWidth;
                        this.sumHeight += element.DesiredSize.Height;
                    }

                    data.LabelWidth = element.DesiredSize.Width;
                    data.LabelHeight = element.DesiredSize.Height;

                    if (data.RelatedAxis.ChartAxesProvider is IChartCartesianAxes)
                    {
                        data.Left = data.LabelPosition.Left;
                        data.Top = data.LabelPosition.Top;
                    }
                    else
                    {
                        data.Left = data.Orientation == Orientation.Vertical ? data.PointInfo.LabelXPosition : this.IsInside(data, "Left");
                        data.Top = data.Orientation == Orientation.Vertical ? data.PointInfo.LabelYPosition : this.IsInside(data, "Top");

                    }
                }
            }

            if (this.Children.Count > 0 && this.Children[0] is DataAxis)
            {
                this.MaxSize = new Size();
                DataAxis data = this.Children[0] as DataAxis;
                if (data != null && data.RelatedAxis != null && data.RelatedAxis.ChartAxesProvider is IChartCartesianAxes)
                {
                    this.Orientation = data.RelatedAxis.Orientation;
                    this.OpposedPosition = data.RelatedAxis.OpposedPosition;

                    data.RelatedAxis.AxisLabelMaxSize = (data.RelatedAxis.Orientation == Orientation.Horizontal) ? (from label in this.Children.OfType<DataAxis>() select (label.DesiredSize.Height - label.IntersectActionMargin.Top - label.IntersectActionMargin.Bottom)).Max() : (from label in this.Children.OfType<DataAxis>() select label.DesiredSize.Width - label.IntersectActionMargin.Right - label.IntersectActionMargin.Left).Max();

                    if (data.RelatedAxis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Shift)
                    {
                        EdgeLabelDrawingMode(availableSize);
                    }

                    if (data.RelatedAxis.Orientation == Orientation.Horizontal)
                    {
                        switch (data.RelatedAxis.IntersectAction)
                        {
                            case ChartLabelIntersectAction.Hide:
                                HideAction();
                                break;
                            case ChartLabelIntersectAction.MultipleRows:
                                MultipleRowsAction();
                                break;
                            case ChartLabelIntersectAction.Rotate:
                                RotateAction();
                                break;
                            case ChartLabelIntersectAction.Wrap:
                                WrapAction();
                                break;

                        }
                    }
                    else
                    {
                        switch (data.RelatedAxis.IntersectAction)
                        {
                            case ChartLabelIntersectAction.MultipleRows:
                                MultipleRowsAction();
                                break;
                            case ChartLabelIntersectAction.Hide:
                                HideAction();
                                break;
                        }
                    }

                    if ((data.RelatedAxis.IntersectAction==ChartLabelIntersectAction.Rotate || data.RelatedAxis.IntersectAction ==ChartLabelIntersectAction.None) && data.RelatedAxis.LabelRotateAngle != 0)
                    {
                        foreach (DataAxis dataaxis in this.Children.OfType<DataAxis>())
                        {
                            getLabelSize(dataaxis, data.RelatedAxis.LabelRotateAngle);
                            if (dataaxis.LabelRotateTransform.Children.Count == 2)
                            {
                                dataaxis.LabelRotateTransform.Children.RemoveAt(1);
                            }
                            dataaxis.LabelRotateTransform.Children.Add(new TranslateTransform() { X = dataaxis.RelatedAxis.Orientation == Orientation.Horizontal ? 0 : dataaxis.transformx / (dataaxis.OpposedPosition ? 2d : -2d), Y = dataaxis.RelatedAxis.Orientation == Orientation.Horizontal ? (dataaxis.transformy / 2d) - 5  : 0d });
                        }

                        if (data.RelatedAxis.Orientation == Orientation.Horizontal)
                            sumHeight = Math.Max(sumHeight, this.MaxSize.Height);
                        else
                            sumWidth = Math.Max(sumWidth, this.MaxSize.Width);
                    }

                    totalSize = data.RelatedAxis.Orientation == Orientation.Horizontal ? new Size(totalSize.Width, sumHeight) : new Size(sumWidth, totalSize.Height);

                    if (data.RelatedAxis.IntersectAction == ChartLabelIntersectAction.MultipleRows)
                    {
                        double maxSize = data.RelatedAxis.Orientation == Orientation.Horizontal ? totalSize.Height : totalSize.Width;
                        if (data.RelatedAxis.Orientation == Orientation.Horizontal)
                        {
                            var result = (from axislabel in this.Children.OfType<DataAxis>() where axislabel.Row != 0 select axislabel.LabelPosition.Top + (axislabel.LabelHeight * axislabel.Row));
                            maxSize += result != null && result.Count<double>() != 0 ? result.Max() : 0d;
                            totalSize = new Size(totalSize.Width, maxSize);
                        }
                        else
                        {
                            var result1 = (from axislabel in this.Children.OfType<DataAxis>() where axislabel.Row != 0 select axislabel.LabelPosition.Left + (axislabel.LabelWidth * axislabel.Row));
                            maxSize += result1 != null && result1.Count<double>() != 0 ? result1.Max() : 0d;
                            totalSize = new Size(maxSize, totalSize.Height);
                        }
                    }

                    return totalSize;
                }
                return base.MeasureOverride(totalSize);
            }



            return base.MeasureOverride(totalSize);
        }

        private double IsInside(DataAxis data, string align)
        {
            double top = 0;
            double left = 0;
            double centerX = data.PointInfo.CenterPoint.X;
            double centerY = data.PointInfo.CenterPoint.Y;
            double offsetX = data.LabelWidth / 3;
            double offsetY = data.LabelHeight / 2;
            double X1 = data.PointInfo.LabelXPosition;
            double Y1 = data.PointInfo.LabelYPosition;
            if (align == "Left")
            {
                if (X1 == centerX)
                {
                    return left = X1;
                }
                else
                {
                    return left = X1 < centerX ? X1 - (offsetX) : X1 + (offsetX);
                }
            }
            else
            {
               
                    if (X1 == centerX)
                    {
                        return top = Y1;
                    }
                    else
                    {
                        return top = Y1 < centerY ? Y1 - offsetY : Y1 - offsetY;
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
            foreach (DataAxis element in this.Children)
            {
                Rect rect;

                if (element.RelatedAxis.ChartAxesProvider is IChartCartesianAxes)
                {
                    rect = new Rect((this.Orientation == Orientation.Vertical && this.OpposedPosition == false ? finalSize.Width - element.DesiredSize.Width : element.LabelPosition.Left) + element.IntersectActionMargin.Left, element.LabelPosition.Top + element.IntersectActionMargin.Top, element.DesiredSize.Width, element.DesiredSize.Height);
                }
                else if (element.RelatedAxis.ChartAxesProvider is IChartRadarAxes || element.RelatedAxis.ChartAxesProvider is IChartPolarAxes)
                {
                    rect = new Rect((element.Left - (element.RelatedAxis.Orientation == Orientation.Vertical ? element.DesiredSize.Width : 0)), element.Top, element.DesiredSize.Width, element.DesiredSize.Height);
                }
                else
                {
                    rect = new Rect(element.PointInfo.X1, element.PointInfo.Y1, element.DesiredSize.Width, element.DesiredSize.Height);
                }


                #region Hide Partial Label
                if (element.RelatedAxis != null && element.RelatedAxis.HidePartialLabel == true)
                {
                    bool result = element.RelatedAxis.Orientation == Orientation.Horizontal ? rect.Left <= 0 || rect.Left + rect.Width - 5 >= finalSize.Width : rect.Top <= 0 || rect.Top + rect.Height - 8 >= finalSize.Height;
                    rect = result ? new Rect(0, 0, 0, 0) : rect;
                }
                #endregion

                rect = element.IsHideLabel ? new Rect(0, 0, 0, 0) : rect;

                if (element.RelatedAxis.IsInversed)
                {
                    rect = new Rect(element.RelatedAxis.Orientation == Orientation.Horizontal ? Math.Abs(finalSize.Width - rect.X) : rect.X,
                        element.RelatedAxis.Orientation == Orientation.Vertical ? Math.Abs(finalSize.Height - rect.Y) : rect.Y, rect.Width, rect.Height);
                }
                
                element.Arrange(rect);
                
            }

            return finalSize;
        }

        #region EdgeLabel Drawing Mode
        internal void EdgeLabelDrawingMode(Size avilableSize)
        {
            foreach (DataAxis data in this.Children.OfType<DataAxis>())
            {
                bool result = data.RelatedAxis.Orientation == Orientation.Horizontal ? data.Left <= 0 || data.Left + data.LabelWidth >= avilableSize.Width : data.Top <= 0 || data.Top + data.LabelHeight >= avilableSize.Height;
                if (result)
                {
                    if (data.Orientation == Orientation.Horizontal)
                    {
                        double Left = data.Left <= 0d ? 0d : ((data.Left + data.DesiredSize.Width >= avilableSize.Width) ? data.Left - (data.Left + data.DesiredSize.Width - avilableSize.Width) : data.Left);
                        data.Left = Left + data.LabelWidth / 2d;
                        data.LabelPosition = new Thickness(data.Left, data.LabelPosition.Top, data.LabelPosition.Right, data.LabelPosition.Bottom);
                    }
                    else
                    {
                        double Top = data.Top <= 0 ? 0d : ((data.Top + data.DesiredSize.Height >= avilableSize.Height) ? data.Top - (data.Top + data.DesiredSize.Height - avilableSize.Height) : data.Top);
                        data.Top = Top + data.LabelHeight / 2d;
                        data.LabelPosition = new Thickness(data.LabelPosition.Left, data.Top, data.LabelPosition.Right, data.LabelPosition.Bottom);
                    }
                }
            }
        }
        #endregion

        #region Rotate Intersect Action
        void getLabelSize(DataAxis axis, double angle)
        {
            Point origin = new Point(0, 0);
            Point EndPoint = new Point(axis.DesiredSize.Width, axis.DesiredSize.Height);
            Point result = ChartMath.GeneralPointRotation(origin, EndPoint, angle);

            double width=Math.Abs(result.X);
            double height=Math.Abs(result.Y);
            axis.transformx = width;
            axis.transformy = height;

            this.MaxSize = new Size(this.MaxSize.Width < width ? width : this.MaxSize.Width, this.MaxSize.Height < height ? height : this.MaxSize.Height);
            
        }
        /// <summary>
        /// Method implementation for set RotateAngle for ChartAxis labels
        /// </summary>
        public void RotateAction()
        {
            List<DataAxis> newdata = new List<DataAxis>();
            double maxH = 0;
            double minW = double.MaxValue; ////min label dist 
            double maxW = 0;
            double lastX = 0;

            foreach (DataAxis data in this.Children.OfType<DataAxis>())
            {
                newdata.Add(data);
                if (newdata[newdata.Count - 1].Left > lastX && newdata.Count > 1)
                {
                    minW = Math.Min(minW, data.LabelWidth);
                }

                lastX = newdata[newdata.Count - 1].Left;
                maxH = Math.Max(maxH, data.LabelHeight);
                maxW = Math.Max(maxW, data.LabelWidth);
            }

            double angle = minW != double.MaxValue ? 180 * Math.Atan(maxH / minW) / Math.PI : 0d;
            angle = Math.Abs(angle < 0 ? 90 : angle);
            if (IsLabelIntersect() == false)
            {
                angle = 0;
                newdata[0].RelatedAxis.LabelRotateAngle = angle;
            }
            else
            {
                newdata[0].RelatedAxis.LabelRotateAngle = angle;
            }

            //foreach (DataAxis data in this.Children.OfType<DataAxis>())
            //{
            //    getLabelSize(data, angle);
            //}
        }

        internal bool IsLabelIntersect()
        {
            List<DataAxis> newdata = new List<DataAxis>();

            foreach (DataAxis data in this.Children.OfType<DataAxis>())
            {
                newdata.Add(data);
            }

            if (this.Orientation == Orientation.Horizontal)
            {
                for (int i = 0; i < newdata.Count; i++)
                {
                    for (int j = i + 1; j < newdata.Count; j++)
                    {
                        if (newdata[i].Left + newdata[i].LabelWidth >= newdata[j].Left)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < newdata.Count; i++)
                {
                    for (int j = i + 1; j < newdata.Count; j++)
                    {
                        if (newdata[j].Top + newdata[j].LabelHeight >= newdata[i].Top)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        #region Wrap Intersect Action
        internal void WrapAction()
        {
            List<DataAxis> newdata = new List<DataAxis>();

            foreach (DataAxis data in this.Children.OfType<DataAxis>())
            {
                newdata.Add(data);
            }

            if (this.Orientation == Orientation.Horizontal)
            {
                if (newdata.Count > 1)
                {
                    double width = Math.Abs(newdata[0].PointInfo.X1 - newdata[1].PointInfo.X1) - 3;
                    foreach (DataAxis data in newdata)
                    {
                        data.Width = width;
                    }
                }
            }
            else
            {
                if (newdata.Count > 1)
                {
                    double height = Math.Abs(newdata[0].PointInfo.Y1 - newdata[1].PointInfo.Y1) - 3;
                    foreach (DataAxis data in newdata)
                    {
                        data.Height = height;
                    }
                }
            }
        }
        #endregion

        #region Hide Intersect Action
        internal void HideAction()
        {
            List<DataAxis> newdata = new List<DataAxis>();

            foreach (DataAxis data in this.Children.OfType<DataAxis>())
            {
                newdata.Add(data);
            }

            if (this.Orientation == Orientation.Horizontal)
            {
                for (int i = 0; i < newdata.Count; i++)
                {
                    for (int j = i + 1; j < newdata.Count; j++)
                    {
                        if (newdata[i].IsHideLabel == true)
                        {
                            break;
                        }

                        if (newdata[i].Left + newdata[i].LabelWidth + 2 >= newdata[j].Left && newdata[i].IsHideLabel == false)
                        {
                            newdata[j].IsHideLabel = true;
                        }
                        else
                        {
                            newdata[j].IsHideLabel = false;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < newdata.Count; i++)
                {
                    for (int j = i + 1; j < newdata.Count; j++)
                    {
                        if (newdata[i].IsHideLabel == true)
                        {
                            break;
                        }

                        if (newdata[j].Top + newdata[j].LabelHeight >= newdata[i].Top && newdata[i].IsHideLabel == false)
                        {
                            newdata[j].IsHideLabel = true;
                        }
                        else
                        {
                            newdata[j].IsHideLabel = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region Multiple Row Action
        internal void SetRows(List<DataAxis> data, int row)
        {
            if (data.Count == 0)
            {
                return;
            }

            List<DataAxis> newdata = new List<DataAxis>();
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = i + 1; j < data.Count; j++)
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        if (data[i].Left + data[i].LabelWidth >= data[j].Left && data[i].Row == data[j].Row && data[j].Row != row)
                        {
                            data[j].Row = row;
                            newdata.Add(data[j]);
                        }
                    }
                    else
                    {
                        if (data[j].Top + data[j].LabelHeight >= data[i].Top && data[i].Row == data[j].Row && data[j].Row != row)
                        {
                            data[j].Row = row;
                            newdata.Add(data[j]);
                        }
                    }
                }
            }

            SetRows(newdata, ++row);
        }

        internal double MaxRow
        {
            get;
            set;
        }


        internal void MultipleRowsAction()
        {
            List<DataAxis> newdata = new List<DataAxis>();

            foreach (DataAxis data in this.Children.OfType<DataAxis>())
            {
                data.Row = 0d;
                newdata.Add(data);
            }

            SetRows(newdata, 1);
            this.MaxRow = (from label in newdata select label.Row).Max();


            foreach (DataAxis data in this.Children.OfType<DataAxis>())
            {
                if (data.Orientation == Orientation.Horizontal && data.OpposedPosition == false)
                {
                    data.IntersectActionMargin = new Thickness(0, (data.Row * data.RelatedAxis.AxisLabelMaxSize), 0, 0);
                }
                else if (data.Orientation == Orientation.Horizontal && data.OpposedPosition == true)
                {
                    data.IntersectActionMargin = new Thickness(0, ((this.MaxRow - data.Row) * data.RelatedAxis.AxisLabelMaxSize), 0, (data.Row * data.RelatedAxis.AxisLabelMaxSize));
                }
                else if (data.Orientation == Orientation.Vertical && data.OpposedPosition == false)
                {
                    if (data.RelatedAxis.ChartLabelPosition == LabelPositions.Outside)
                    {
                        data.IntersectActionMargin = new Thickness((-(data.Row) * data.RelatedAxis.AxisLabelMaxSize), 0, (data.Row * data.RelatedAxis.AxisLabelMaxSize), 0);

                    }
                    else
                    {
                        data.IntersectActionMargin = new Thickness(((this.MaxRow - data.Row) * data.RelatedAxis.AxisLabelMaxSize), 0, (data.Row * data.RelatedAxis.AxisLabelMaxSize), 0);
                    }
                }
                else
                {
                    data.IntersectActionMargin = new Thickness((data.Row * data.RelatedAxis.AxisLabelMaxSize), 0, 0, 0);
                }

            }

        }
        #endregion
    }
}
