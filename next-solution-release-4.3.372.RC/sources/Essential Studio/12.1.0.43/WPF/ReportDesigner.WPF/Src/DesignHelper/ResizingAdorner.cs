#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;

namespace Syncfusion.Windows.Reports.Designer.Controls
{
    internal class ResizingAdorner : Adorner
    {
        Thumb topLeft, topRight, bottomLeft, bottomRight,topMidddle,bottomMiddle,leftMiddle,rightMiddle,movingThumb;

        Canvas designCanvas;
        DesignPanel designPanel;
        IReportItemControl reportControl;
        Point startMousePoistion;
        VisualCollection visualChildren;
        
        List<ReportItemLocationInfo> designerItemsInfo = new List<ReportItemLocationInfo>();
        Dictionary<string, ReportItemLocationInfo> selectedReportItemsParentInfo = new Dictionary<string, ReportItemLocationInfo>();
        Dictionary<string, ReportItemImageControl> reportItemImages = new Dictionary<string, ReportItemImageControl>();

        Dictionary<string, MovingHelpingLine> topRelativeLocationInfo = new Dictionary<string, MovingHelpingLine>();
        Dictionary<string, MovingHelpingLine> bottomRelativeLocationInfo = new Dictionary<string, MovingHelpingLine>();
        Dictionary<string, MovingHelpingLine> leftRelativeLocationInfo = new Dictionary<string, MovingHelpingLine>();
        Dictionary<string, MovingHelpingLine> rightRelativeLocationInfo = new Dictionary<string, MovingHelpingLine>();

        Dictionary<string, MovingHelpingLine> topMovingSizes = new Dictionary<string, MovingHelpingLine>();
        Dictionary<string, MovingHelpingLine> leftMovingSizes = new Dictionary<string, MovingHelpingLine>();
        Dictionary<string, MovingHelpingLine> rightMovingSizes = new Dictionary<string, MovingHelpingLine>();
        Dictionary<string, MovingHelpingLine> bottomMovingSizes = new Dictionary<string, MovingHelpingLine>();

        Dictionary<string, ResizingHelpingLine> resizingWidthSizes = new Dictionary<string, ResizingHelpingLine>();
        Dictionary<string, ResizingHelpingLine> resizingHeightSizes = new Dictionary<string, ResizingHelpingLine>();

        // Initialize the ResizingAdorner.
        public ResizingAdorner(UIElement adornedElement,DesignPanel panel)
            : base(adornedElement)
        {
            this.designPanel = panel;
            this.visualChildren = new VisualCollection(this);

            this.reportControl = adornedElement as IReportItemControl;
            this.BuildMovingThumb(ref movingThumb, Cursors.SizeAll);
            
            if (reportControl.ItemType == DrawingReportItem.Line)
            {
                this.BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
                this.BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);

                this.topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeftDrag);
                this.topLeft.DragCompleted += new DragCompletedEventHandler(ReportItemResizingCompleted);
                this.topLeft.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

                this.bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
                this.bottomRight.DragCompleted += new DragCompletedEventHandler(ReportItemResizingCompleted);
                this.bottomRight.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
            }
            else
            {
                this.BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
                this.BuildAdornerCorner(ref topRight, Cursors.SizeNESW);

                this.BuildAdornerCorner(ref bottomLeft, Cursors.SizeNESW);
                this.BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);

                this.BuildAdornerCorner(ref topMidddle, Cursors.SizeNS);
                this.BuildAdornerCorner(ref bottomMiddle, Cursors.SizeNS);
                
                this.BuildAdornerCorner(ref leftMiddle, Cursors.SizeWE);
                this.BuildAdornerCorner(ref rightMiddle, Cursors.SizeWE);
             
                // Add handlers for resizing.
                this.topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeftDrag);
                this.topLeft.DragCompleted += new DragCompletedEventHandler(ReportItemResizingCompleted);
                this.topLeft.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

                this.topMidddle.DragDelta += new DragDeltaEventHandler(HandleTopMiddleDrag);
                this.topMidddle.DragCompleted += new DragCompletedEventHandler(ReportItemResizingCompleted);
                this.topMidddle.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

                this.topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
                this.topRight.DragCompleted += new DragCompletedEventHandler(ReportItemResizingCompleted);
                this.topRight.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

                this.bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeftDrag);
                this.bottomLeft.DragCompleted += new DragCompletedEventHandler(ReportItemResizingCompleted);
                this.bottomLeft.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

                this.bottomMiddle.DragDelta += new DragDeltaEventHandler(HandleBottomMiddleDrag);
                this.bottomMiddle.DragCompleted += new DragCompletedEventHandler(ReportItemResizingCompleted);
                this.bottomMiddle.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

                this.bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
                this.bottomRight.DragCompleted += new DragCompletedEventHandler(ReportItemResizingCompleted);
                this.bottomRight.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
                
                this.leftMiddle.DragDelta += new DragDeltaEventHandler(HandleLeftMiddleDrag);
                this.leftMiddle.DragCompleted += new DragCompletedEventHandler(ReportItemResizingCompleted);
                this.leftMiddle.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

                this.rightMiddle.DragDelta += new DragDeltaEventHandler(HandleRightMiddle);
                this.rightMiddle.DragCompleted += new DragCompletedEventHandler(ReportItemResizingCompleted);
                this.rightMiddle.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
            }

            this.movingThumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
            this.movingThumb.DragDelta += new DragDeltaEventHandler(movingThumb_Drag);
            this.movingThumb.DragCompleted += new DragCompletedEventHandler(movingThumb_DragCompleted);
        }

        private void UpdateLine(string reportItem,ReportItemLocationInfo reportItemLocation,Direction direction)
        {
            MovingHelpingLine rightLine = this.rightRelativeLocationInfo[reportItem];
            MovingHelpingLine bottomLine = this.bottomRelativeLocationInfo[reportItem];
            MovingHelpingLine leftLine = this.leftRelativeLocationInfo[reportItem];
            MovingHelpingLine topLine = this.topRelativeLocationInfo[reportItem];

            if (direction == Direction.TopLeft || direction == Direction.TopMidddle || direction == Direction.TopRight || direction == Direction.Top)
            {
                var topReportItems = (from reportitem in this.designerItemsInfo
                                      where Math.Floor(reportitem.ItemTop) == Math.Floor(reportItemLocation.ItemTop)
                                      select reportitem).ToList();

                if (direction == Direction.TopLeft)
                {
                    this.designPanel.RulerHorizontal.Chip = this.designPanel.RulerHorizontal.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemLeft) : DipHelper.DipToInch(reportItemLocation.ItemLeft);
                }
                else if(direction == Direction.TopRight)
                {
                    this.designPanel.RulerHorizontal.Chip = this.designPanel.RulerHorizontal.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemRight) : DipHelper.DipToInch(reportItemLocation.ItemRight);
                }
                this.designPanel.RulerVerticalBody.Chip = this.designPanel.RulerVerticalBody.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemTop) : DipHelper.DipToInch(reportItemLocation.ItemTop);

                if (topReportItems.Count > 0)
                {
                    topReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                    {
                        return first.ItemLeft.CompareTo(second.ItemLeft);
                    });

                    var leftReportItem = topReportItems.First();


                    double leftValue = leftReportItem.ItemLeft < reportItemLocation.ItemLeft ? leftReportItem.ItemLeft : reportItemLocation.ItemLeft;

                    topReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                    {
                        return (first.ItemRight).CompareTo(second.ItemRight);
                    });

                    var rightReportItem = topReportItems.Last();
                    double rightValue = rightReportItem.ItemRight < reportItemLocation.ItemRight ? reportItemLocation.ItemRight : rightReportItem.ItemRight;

                    double leftVal = 0;

                    if (leftReportItem.ReportItem!=null && leftReportItem.ReportItem.Parent is RectangleControl && leftReportItem.ItemLeft > reportItemLocation.ItemLeft)
                    {
                        leftVal = leftReportItem.ReportItem.ItemLeft;
                    }

                    topLine.Visibility = System.Windows.Visibility.Visible;
                    topLine.Width = rightValue - leftValue;
                    topLine.X2 = topLine.Width - leftVal;

                    if (direction != Direction.Top)
                    {
                        if (reportItemLocation.ItemLeft > leftReportItem.ItemRight)
                        {
                            topLine.Text = ((Math.Abs(reportItemLocation.ItemLeft - leftReportItem.ItemRight)) / 96).ToString("#0.0") + "in";
                            Canvas.SetLeft(topLine.InnerTextblock, (leftReportItem.ItemRight + reportItemLocation.ItemLeft) / 2 - leftValue);
                        }
                        else
                        {
                            topLine.Text = ((Math.Abs(rightReportItem.ItemLeft - reportItemLocation.ItemRight)) / 96).ToString("#0.0") + "in";
                            Canvas.SetLeft(topLine.InnerTextblock, (rightReportItem.ItemLeft + reportItemLocation.ItemRight) / 2 - leftValue);
                        }
                    }
                    Canvas.SetLeft(topLine, leftValue);
                    Canvas.SetTop(topLine, reportItemLocation.ItemTop);
                }
            }

            if (direction == Direction.TopLeft || direction == Direction.LeftMiddle || direction == Direction.BottomLeft || direction == Direction.Left)
            {
                var leftReportItems = (from reportitem in this.designerItemsInfo
                                       where Math.Floor(reportitem.ItemLeft) ==Math.Floor (reportItemLocation.ItemLeft)
                                       select reportitem).ToList();

                if (direction == Direction.TopLeft)
                {
                    this.designPanel.RulerVerticalBody.Chip = this.designPanel.RulerVerticalBody.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemTop) : DipHelper.DipToInch(reportItemLocation.ItemTop);
                }
                else if(direction == Direction.BottomLeft)
                {
                    this.designPanel.RulerVerticalBody.Chip = this.designPanel.RulerVerticalBody.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemBottom) : DipHelper.DipToInch(reportItemLocation.ItemBottom);
                }
                this.designPanel.RulerHorizontal.Chip = this.designPanel.RulerHorizontal.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemLeft) : DipHelper.DipToInch(reportItemLocation.ItemLeft);

                if (leftReportItems.Count > 0)
                {
                    leftReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                    {
                        return first.ItemTop.CompareTo(second.ItemTop);
                    });

                    var topReportItem = leftReportItems.First();
                    double topValue = topReportItem.ItemTop < reportItemLocation.ItemTop ? topReportItem.ItemTop : reportItemLocation.ItemTop;

                    leftReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                    {
                        return first.ItemBottom.CompareTo(first.ItemBottom);
                    });

                    var bottomReportItem = leftReportItems.Last();
                    double bottomValue = bottomReportItem.ItemBottom < reportItemLocation.ItemBottom ? reportItemLocation.ItemBottom : bottomReportItem.ItemBottom;
                    leftLine.Visibility = System.Windows.Visibility.Visible;
                    leftLine.Height = bottomValue - topValue;
                    leftLine.Y2 = leftLine.Height;
                    if (direction != Direction.Left)
                    {
                        if (reportItemLocation.ItemTop > topReportItem.ItemBottom)
                        {
                            leftLine.Text = ((Math.Abs(reportItemLocation.ItemTop - topReportItem.ItemBottom)) / 96).ToString("#0.0") + "in";
                            Canvas.SetTop(leftLine.InnerTextblock, (topReportItem.ItemBottom + reportItemLocation.ItemTop) / 2 - topValue);
                        }
                        else
                        {
                            leftLine.Text = ((Math.Abs(bottomReportItem.ItemTop - reportItemLocation.ItemBottom)) / 96).ToString("#0.0") + "in";
                            Canvas.SetTop(leftLine.InnerTextblock, (bottomReportItem.ItemTop + reportItemLocation.ItemBottom) / 2 - topValue);
                        }
                    }
                    Canvas.SetLeft(leftLine, reportItemLocation.ItemLeft);
                    Canvas.SetTop(leftLine, topValue);
                }
            }

            if (direction == Direction.BottomLeft || direction == Direction.BottomMiddle || direction == Direction.BottomRight || direction == Direction.Bottom)
            {
                var bottomReportItems = (from reportitem in this.designerItemsInfo
                                         where Math.Floor(reportitem.ItemBottom) == Math.Floor(reportItemLocation.ItemBottom)
                                         select reportitem).ToList();

                if (direction == Direction.BottomLeft)
                {
                    this.designPanel.RulerHorizontal.Chip = this.designPanel.RulerHorizontal.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemLeft) : DipHelper.DipToInch(reportItemLocation.ItemLeft);
                }
                else if(direction == Direction.BottomRight)
                {
                    this.designPanel.RulerHorizontal.Chip = this.designPanel.RulerHorizontal.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemRight) : DipHelper.DipToInch(reportItemLocation.ItemRight);
                }
                this.designPanel.RulerVerticalBody.Chip = this.designPanel.RulerVerticalBody.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemBottom) : DipHelper.DipToInch(reportItemLocation.ItemBottom);

                if (bottomReportItems.Count > 0)
                {
                    bottomReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                    {
                        return first.ItemLeft.CompareTo(second.ItemLeft);
                    });

                    var leftReportItem = bottomReportItems.First();
                    double leftValue = leftReportItem.ItemLeft < reportItemLocation.ItemLeft ? leftReportItem.ItemLeft : reportItemLocation.ItemLeft;

                    bottomReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                    {
                        return first.ItemRight.CompareTo(second.ItemRight);
                    });

                    double leftVal = 0;

                    if (leftReportItem.ReportItem != null && leftReportItem.ReportItem.Parent is RectangleControl && leftReportItem.ItemLeft > reportItemLocation.ItemLeft)
                    {
                        leftVal = leftReportItem.ReportItem.ItemLeft;
                    }

                    var rightReportItem = bottomReportItems.Last();
                    double rightValue = rightReportItem.ItemRight < reportItemLocation.ItemRight ? reportItemLocation.ItemRight : rightReportItem.ItemRight;
                    bottomLine.Visibility = System.Windows.Visibility.Visible;
                    bottomLine.Width = rightValue - leftValue;
                    bottomLine.X2 = bottomLine.Width-leftVal;
                    bottomLine.X1 = 0;
                    bottomLine.Y1 = 0;
                    bottomLine.Y2 = 0;
                    if (direction != Direction.Bottom)
                    {
                        if (reportItemLocation.ItemLeft > leftReportItem.ItemRight)
                        {
                            bottomLine.Text = ((Math.Abs(reportItemLocation.ItemLeft - leftReportItem.ItemRight)) / 96).ToString("#0.0") + "in";
                            Canvas.SetLeft(bottomLine.InnerTextblock, (leftReportItem.ItemRight + reportItemLocation.ItemLeft) / 2 - leftValue);
                        }
                        else
                        {
                            bottomLine.Text = ((Math.Abs(rightReportItem.ItemLeft - reportItemLocation.ItemRight)) / 96).ToString("#0.0") + "in";
                            Canvas.SetLeft(bottomLine.InnerTextblock, (rightReportItem.ItemLeft + reportItemLocation.ItemRight) / 2 - leftValue);
                        }
                    }
                    Canvas.SetLeft(bottomLine, leftValue);
                    Canvas.SetTop(bottomLine, reportItemLocation.ItemBottom);
                }
            }

            if (direction == Direction.TopRight || direction == Direction.RightMiddle || direction == Direction.BottomRight || direction == Direction.Right)
            {
                var rightReportItems = (from reportitem in this.designerItemsInfo
                                        where Math.Floor(reportitem.ItemRight) == Math.Floor(reportItemLocation.ItemRight)
                                        select reportitem).ToList();

                if (direction == Direction.TopRight)
                {
                    this.designPanel.RulerVerticalBody.Chip = this.designPanel.RulerVerticalBody.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemTop) : DipHelper.DipToInch(reportItemLocation.ItemTop);
                }
                else if(direction == Direction.BottomRight)
                {
                    this.designPanel.RulerVerticalBody.Chip = this.designPanel.RulerVerticalBody.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemBottom) : DipHelper.DipToInch(reportItemLocation.ItemBottom);
                }
                this.designPanel.RulerHorizontal.Chip = this.designPanel.RulerHorizontal.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemRight) : DipHelper.DipToInch(reportItemLocation.ItemRight);

                if (rightReportItems.Count > 0)
                {
                    rightReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                    {
                        return first.ItemTop.CompareTo(second.ItemTop);
                    });

                    var topReportItem = rightReportItems.First();
                    double topValue = topReportItem.ItemTop < reportItemLocation.ItemTop ? topReportItem.ItemTop : reportItemLocation.ItemTop;

                    rightReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                    {
                        return first.ItemBottom.CompareTo(second.ItemBottom);
                    });

                    var bottomReportItem = rightReportItems.Last();
                    double bottomValue = bottomReportItem.ItemBottom < reportItemLocation.ItemBottom ? reportItemLocation.ItemBottom : bottomReportItem.ItemBottom;
                    rightLine.Visibility = System.Windows.Visibility.Visible;
                    rightLine.Width = bottomValue - topValue;
                    rightLine.Y2 = rightLine.Width;
                    rightLine.X1 = 0;
                    rightLine.X2 = 0;
                    rightLine.Y1 = 0;
                    if (direction != Direction.Right)
                    {
                        if (reportItemLocation.ItemTop > topReportItem.ItemBottom)
                        {
                            rightLine.Text = ((Math.Abs(reportItemLocation.ItemTop - topReportItem.ItemBottom)) / 96).ToString("#0.0") + "in";
                            Canvas.SetTop(rightLine.InnerTextblock, (topReportItem.ItemBottom + reportItemLocation.ItemTop) / 2 - topValue);
                        }
                        else
                        {
                            rightLine.Text = ((Math.Abs(bottomReportItem.ItemTop - reportItemLocation.ItemBottom)) / 96).ToString("#0.0") + "in";
                            Canvas.SetTop(rightLine.InnerTextblock, (bottomReportItem.ItemTop + reportItemLocation.ItemBottom) / 2 - topValue);
                        }
                    }
                    Canvas.SetLeft(rightLine, reportItemLocation.ItemRight);
                    Canvas.SetTop(rightLine, topValue);
                }
            }
        }

        void UpdateMovingHelpingLines(ReportItemImageControl moveControl, ReportItemLocationInfo reportItemLocation,Canvas drawingArea)
        {
            MovingHelpingLine topLine = this.topMovingSizes[moveControl.ReportItemName];
            MovingHelpingLine leftLine = this.leftMovingSizes[moveControl.ReportItemName];
            MovingHelpingLine rightLine = this.rightMovingSizes[moveControl.ReportItemName];
            MovingHelpingLine bottomLine = this.bottomMovingSizes[moveControl.ReportItemName];

            double leftval = 0;
            double topval = 0;

            this.designPanel.RulerHorizontal.Chip = this.designPanel.RulerHorizontal.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemLeft) : DipHelper.DipToInch(reportItemLocation.ItemLeft);
            this.designPanel.RulerVerticalBody.Chip = this.designPanel.RulerVerticalBody.Unit == Unit.Cm ? DipHelper.DipToCm(reportItemLocation.ItemTop) : DipHelper.DipToInch(reportItemLocation.ItemTop);

            if (drawingArea is IReportItemControl)
            {
                leftval = (drawingArea as IReportItemControl).ItemLeft;
                topval = (drawingArea as IReportItemControl).ItemTop;
            }

            var topReportItems = (from reportitem in this.designerItemsInfo
                                  where reportitem.ItemBottom < reportItemLocation.ItemTop && (topLine.X1 > reportitem.ItemLeft  && topLine.X1 < reportitem.ItemRight)
                                  select reportitem).ToList();
            var bottomReportItems = (from reportitem in this.designerItemsInfo
                                     where reportitem.ItemTop > reportItemLocation.ItemBottom && (bottomLine.X1 > reportitem.ItemLeft  && bottomLine.X1 < reportitem.ItemRight)
                                     select reportitem).ToList();
            var leftReportItems = (from reportitem in this.designerItemsInfo
                                   where reportitem.ItemRight  < reportItemLocation.ItemLeft && (leftLine.Y1 > reportitem.ItemTop && leftLine.Y1 < reportitem.ItemBottom)
                                   select reportitem).ToList();
            var rightReportItems = (from reportitem in this.designerItemsInfo
                                    where reportitem.ItemLeft > reportItemLocation.ItemRight && (rightLine.Y1 > reportitem.ItemTop && rightLine.Y1 < reportitem.ItemBottom)
                                    select reportitem).ToList();


            if (topReportItems.Count > 0 && !(drawingArea is IReportItemControl))
            {
                topReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                {
                    return first.ItemBottom.CompareTo(second.ItemBottom);
                });
                var leftReportItem = topReportItems.Last();

                topLine.X2 = (reportItemLocation.ItemLeft + reportItemLocation.ItemRight) / 2;
                topLine.Y2 = reportItemLocation.ItemTop;
                topLine.X1 = topLine.X2;
                topLine.Y1 = leftReportItem.ItemBottom;
                topLine.Text = ((Math.Abs(reportItemLocation.ItemTop - leftReportItem.ItemBottom)) / 96).ToString("#0.0") + "in";
                topLine.Visibility = System.Windows.Visibility.Visible;
                Canvas.SetLeft(topLine.InnerTextblock, topLine.X2);
                Canvas.SetTop(topLine.InnerTextblock, (topLine.Y1 + topLine.Y2) / 2);

            }

            else
            {
                if (reportItemLocation.ItemTop > topval)
                {
                    topLine.X2 = (reportItemLocation.ItemLeft + reportItemLocation.ItemRight) / 2;
                    topLine.Y2 = reportItemLocation.ItemTop;
                    topLine.X1 = topLine.X2;
                    topLine.Y1 = topval;
                    topLine.Text = (reportItemLocation.ItemTop / 96).ToString("#0.0") + "in";
                    topLine.Visibility = System.Windows.Visibility.Visible;
                    Canvas.SetLeft(topLine.InnerTextblock, topLine.X2);
                    Canvas.SetTop(topLine.InnerTextblock, (topLine.Y1 + topLine.Y2) / 2);
                }
                else
                {
                    topLine.Visibility = System.Windows.Visibility.Hidden;
                }
            }

            if (leftReportItems.Count > 0 && !(drawingArea is IReportItemControl))
            {
                leftReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                {
                    return first.ItemLeft.CompareTo(second.ItemLeft);
                });
                var topReportItem = leftReportItems.Last();
                leftLine.Y2 = (reportItemLocation.ItemTop + reportItemLocation.ItemBottom) / 2;
                leftLine.X2 = reportItemLocation.ItemLeft;
                leftLine.Y1 = leftLine.Y2;
                leftLine.X1 = topReportItem.ItemRight;
                leftLine.Text = (Math.Abs(reportItemLocation.ItemLeft- topReportItem.ItemRight) / 96).ToString("#0.0") + "in";
                leftLine.Visibility = System.Windows.Visibility.Visible;
                Canvas.SetLeft(leftLine.InnerTextblock, (leftLine.X1 + leftLine.X2) / 2);
                Canvas.SetTop(leftLine.InnerTextblock, leftLine.Y2);

            }
            else
            {
                if (reportItemLocation.ItemLeft > leftval)
                {
                    leftLine.Y2 = (reportItemLocation.ItemTop + reportItemLocation.ItemBottom) / 2;
                    leftLine.X2 = reportItemLocation.ItemLeft;
                    leftLine.Y1 = leftLine.Y2;
                    leftLine.X1 = leftval;
                    leftLine.Text = (reportItemLocation.ItemLeft / 96).ToString("#0.0") + "in";
                    leftLine.Visibility = System.Windows.Visibility.Visible;
                    Canvas.SetLeft(leftLine.InnerTextblock, (leftLine.X1 + leftLine.X2) / 2);
                    Canvas.SetTop(leftLine.InnerTextblock, leftLine.Y2);
                }
                else
                {
                    leftLine.Visibility = System.Windows.Visibility.Hidden;
                }
            }

            if (rightReportItems.Count > 0 && !(drawingArea is IReportItemControl))
            {

                rightReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                {
                    return first.ItemLeft.CompareTo(second.ItemLeft);
                });
                var topReportItem = rightReportItems.First();
                rightLine.Y1 = (reportItemLocation.ItemTop + reportItemLocation.ItemBottom) / 2;
                rightLine.X1 = reportItemLocation.ItemRight;
                rightLine.X2 = topReportItem.ItemLeft;
                rightLine.Y2 = rightLine.Y1;
                rightLine.Visibility = System.Windows.Visibility.Visible;
                rightLine.Text = ((topReportItem.ItemLeft-reportItemLocation.ItemRight) / 96).ToString("#0.0") + "in";
                Canvas.SetLeft(rightLine.InnerTextblock, (rightLine.X1 + rightLine.X2) / 2);
                Canvas.SetTop(rightLine.InnerTextblock, rightLine.Y1);
            }
            else
            {
                if (reportItemLocation.ItemRight < drawingArea.ActualWidth + leftval)
                {
                    rightLine.Y1 = (reportItemLocation.ItemTop + reportItemLocation.ItemBottom) / 2;
                    rightLine.X1 = reportItemLocation.ItemRight;
                    rightLine.X2 = drawingArea.ActualWidth + leftval;
                    rightLine.Y2 = rightLine.Y1;
                    rightLine.Visibility = System.Windows.Visibility.Visible;
                    rightLine.Text = ((drawingArea.ActualWidth + leftval - reportItemLocation.ItemRight) / 96).ToString("#0.0") + "in";
                    Canvas.SetLeft(rightLine.InnerTextblock, (rightLine.X1 + rightLine.X2) / 2);
                    Canvas.SetTop(rightLine.InnerTextblock, rightLine.Y1);
                }
                else
                {
                    rightLine.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            if (bottomReportItems.Count > 1 && !(drawingArea is IReportItemControl))
            {
                bottomReportItems.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                {
                    return first.ItemTop.CompareTo(second.ItemTop);
                });
                var leftReportItem = bottomReportItems.First();
                bottomLine.X1 = (reportItemLocation.ItemLeft + reportItemLocation.ItemRight) / 2;
                bottomLine.Y1 = reportItemLocation.ItemBottom;
                bottomLine.X2 = bottomLine.X1;
                bottomLine.Y2 = leftReportItem.ItemTop;
                bottomLine.Visibility = System.Windows.Visibility.Visible;
                bottomLine.Text = ((leftReportItem.ItemTop - reportItemLocation.ItemBottom) / 96).ToString("#0.0") + "in";
                Canvas.SetLeft(bottomLine.InnerTextblock, bottomLine.X1);
                Canvas.SetTop(bottomLine.InnerTextblock, (bottomLine.Y1 + bottomLine.Y2) / 2);

            }

            else
            {
                if (reportItemLocation.ItemBottom < drawingArea.ActualHeight + topval)
                {
                    bottomLine.X1 = (reportItemLocation.ItemLeft + reportItemLocation.ItemRight) / 2;
                    bottomLine.Y1 = reportItemLocation.ItemBottom;
                    bottomLine.X2 = bottomLine.X1;
                    bottomLine.Y2 = drawingArea.ActualHeight + topval;
                    bottomLine.Visibility = System.Windows.Visibility.Visible;
                    bottomLine.Text = ((drawingArea.ActualHeight + topval - reportItemLocation.ItemBottom) / 96).ToString("#0.0") + "in";
                    Canvas.SetLeft(bottomLine.InnerTextblock, bottomLine.X1);
                    Canvas.SetTop(bottomLine.InnerTextblock, (bottomLine.Y1 + bottomLine.Y2) / 2);
                }
                else
                {
                    bottomLine.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }


        MovingHelpingLine GetDefaultLine()
        {
            MovingHelpingLine line = new MovingHelpingLine();
            line.Visibility = System.Windows.Visibility.Hidden;
            line.InnerLine.StrokeThickness = 1;
            line.InnerLine.Stroke = Brushes.Blue;
            this.designCanvas.Children.Add(line);
            return line;
        }

        void UpdateLineVisiblity()
        {
            if (this.designCanvas != null)
            {
                var lineControls = this.designCanvas.Children.OfType<MovingHelpingLine>().ToList();

                foreach (var lineControl in lineControls)
                {
                    lineControl.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        void ClearLines()
        {
            if (this.designCanvas != null)
            {
                var lineControls = this.designCanvas.Children.OfType<System.Windows.Shapes.Line>().ToList();
                var movingHelpingLines = this.designCanvas.Children.OfType<MovingHelpingLine>().ToList();
                var resizingHelpingLines = this.designCanvas.Children.OfType<ResizingHelpingLine>().ToList();
                foreach (var lineControl in lineControls)
                {
                    this.designCanvas.Children.Remove(lineControl);
                }

                foreach (var movingline in movingHelpingLines)
                {
                    movingline.Visibility = System.Windows.Visibility.Hidden;
                    this.designCanvas.Children.Remove(movingline);
                }

                foreach (var resizeLine in resizingHelpingLines)
                {
                    resizeLine.Visibility = System.Windows.Visibility.Hidden;
                    this.designCanvas.Children.Remove(resizeLine);
                }

                this.resizingHeightSizes.Clear();
                this.resizingWidthSizes.Clear();
                this.topMovingSizes.Clear();
                this.leftMovingSizes.Clear();
                this.rightMovingSizes.Clear();
                this.bottomMovingSizes.Clear();
                this.topRelativeLocationInfo.Clear();
                this.leftRelativeLocationInfo.Clear();
                this.bottomRelativeLocationInfo.Clear();
                this.rightRelativeLocationInfo.Clear();
            }
        }

        void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            bool isResize = sender != movingThumb;
            reportControl.IsFocusedItem = false;
            this.designPanel.SetZindex();
            this.reportItemImages.Clear();
            this.selectedReportItemsParentInfo.Clear();
            this.startMousePoistion = Mouse.GetPosition(null);

            var removeSelectionItems = (from reportItem in this.designPanel.SelectedReportItems
                                        where reportItem.Parent != this.reportControl.Parent
                                        select reportItem).ToList();

            foreach (var reportItem in removeSelectionItems)
            {
                this.designPanel.SelectedReportItems.Remove(reportItem);
                reportItem.IsFocusedItem = false;
                reportItem.IsItemSelected = false;
            }

            this.designCanvas = this.designPanel.GetDrawingCanvas();

            if (this.designCanvas == null)
            {
                this.designCanvas = reportControl.Parent;
            }

            this.designerItemsInfo = this.designPanel.GetReportItemLocationInfo(this.designCanvas, isResize);
            if (designCanvas != null)
            {
                foreach (var reportItem in this.designPanel.SelectedReportItems)
                {
                    ReportItemImageControl imageControl = new ReportItemImageControl();
                    reportItem.IsFocusedItem = false;
                    imageControl.Source = reportItem.GetImageSource();
                    imageControl.Stretch = Stretch.Fill;
                    imageControl.ReportItem = reportItem;
                    imageControl.ReportItemName = reportItem.ItemName;
                    imageControl.ItemHeight = reportItem.ItemHeight;
                    imageControl.ItemWidth = reportItem.ItemWidth;
                    imageControl.ItemLeft = reportItem.ItemLeft;
                    imageControl.ItemTop = reportItem.ItemTop;
                    double imageHeight = reportItem.ItemHeight;
                    double imageWidth = reportItem.ItemWidth;
                    double imageLeft = reportItem.ItemLeft;
                    double imageTop = reportItem.ItemTop;

                    if (reportItem.ItemType == DrawingReportItem.Line)
                    {
                        imageHeight = imageHeight != 0 ? imageHeight : (reportItem as LineControl).InnerLine.StrokeThickness;
                        imageWidth = imageWidth != 0 ? imageWidth : (reportItem as LineControl).InnerLine.StrokeThickness;

                        if (imageHeight < 0)
                        {
                            imageTop = reportItem.ItemTop + reportItem.ItemHeight;
                            imageHeight = Math.Abs(imageTop - reportItem.ItemTop);
                        }

                        if (imageWidth < 0)
                        {
                            imageLeft = reportItem.ItemLeft + reportItem.ItemWidth;
                            imageWidth = Math.Abs(imageLeft - reportItem.ItemLeft);
                        }

                        imageHeight = imageHeight < 1 ? 1 : imageHeight;
                        imageWidth = imageWidth < 1 ? 1 : imageWidth;
                    }

                    imageControl.Height = imageHeight;
                    imageControl.Width = imageWidth;
                    ReportItemLocationInfo parentInfo = new ReportItemLocationInfo();

                    if (reportItem.Parent is IReportItemControl || reportItem.Parent == null)
                    {
                        parentInfo = this.designPanel.GetParentInfo(reportItem.Parent as IReportItemControl);

                        if (!isResize)
                        {
                            this.reportControl.Parent.Children.Add(imageControl);
                        }
                    }

                    if (imageControl.ReportItemName!=null && !this.selectedReportItemsParentInfo.ContainsKey(imageControl.ReportItemName))
                    {
                        this.selectedReportItemsParentInfo.Add(imageControl.ReportItemName, parentInfo);
                    }
                    if (!this.reportItemImages.ContainsKey(imageControl.ReportItemName))
                    {
                        reportItemImages.Add(imageControl.ReportItemName, imageControl);
                    }

                    if (isResize)
                    {
                        Canvas.SetZIndex(reportItem as UIElement, 1);
                        this.reportControl.Parent.Children.Add(imageControl);
                        ResizingHelpingLine resizeLineWidth = new ResizingHelpingLine();
                        ResizingHelpingLine resizeLineHeight = new ResizingHelpingLine();
                        this.resizingWidthSizes.Add(imageControl.ReportItemName, resizeLineWidth);
                        this.resizingHeightSizes.Add(imageControl.ReportItemName, resizeLineHeight);
                        this.designCanvas.Children.Add(resizeLineHeight);
                        this.designCanvas.Children.Add(resizeLineWidth);
                    }
                    else
                    {
                        imageTop += parentInfo.ItemTop;
                        imageLeft += parentInfo.ItemLeft;
                        imageControl.ItemLeft = imageLeft;
                        imageControl.ItemTop = imageTop;
                        if (reportItem.Parent is IReportItemControl)
                        {
                            this.reportControl.Parent.Children.Remove(imageControl);
                        }
                        this.designCanvas.Children.Add(imageControl);
                        this.AddingMovingHelpingLines(imageControl);
                    }

                    Canvas.SetTop(imageControl, imageTop);
                    Canvas.SetLeft(imageControl, imageLeft);

                    MovingHelpingLine left = this.GetDefaultLine();
                    MovingHelpingLine right = this.GetDefaultLine();
                    MovingHelpingLine top = this.GetDefaultLine();
                    MovingHelpingLine bottom = this.GetDefaultLine();
                    this.leftRelativeLocationInfo.Add(imageControl.ReportItemName, left);
                    this.rightRelativeLocationInfo.Add(imageControl.ReportItemName, right);
                    this.topRelativeLocationInfo.Add(imageControl.ReportItemName, top);
                    this.bottomRelativeLocationInfo.Add(imageControl.ReportItemName, bottom);
                }
            }
        }

        void AddingMovingHelpingLines(ReportItemImageControl imageControl)
        {
            MovingHelpingLine topLine = new MovingHelpingLine();
            MovingHelpingLine leftLine = new MovingHelpingLine();
            MovingHelpingLine rightLine = new MovingHelpingLine();
            MovingHelpingLine bottomLine = new MovingHelpingLine();
            this.topMovingSizes.Add(imageControl.ReportItemName, topLine);
            this.leftMovingSizes.Add(imageControl.ReportItemName, leftLine);
            this.rightMovingSizes.Add(imageControl.ReportItemName, rightLine);
            this.bottomMovingSizes.Add(imageControl.ReportItemName, bottomLine);
            this.designCanvas.Children.Add(topLine);
            this.designCanvas.Children.Add(leftLine);
            this.designCanvas.Children.Add(bottomLine);
            this.designCanvas.Children.Add(rightLine);
        }

        void movingThumb_Drag(object sender, DragDeltaEventArgs e)
        {
            this.UpdateLineVisiblity();

            foreach (var imageControl in this.reportItemImages)
            {
                var moveControl = imageControl.Value;
                
                double left = moveControl.ItemLeft + e.HorizontalChange;
                double top = moveControl.ItemTop + e.VerticalChange;
                double bottom =  top + moveControl.ItemHeight;
                double right = left + moveControl.ItemWidth;

                Canvas.SetLeft(moveControl, left);
                Canvas.SetTop(moveControl, top);
               
                ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
                locationInfo.ItemTop = top;
                locationInfo.ItemLeft = left;
                locationInfo.ItemRight = locationInfo.ItemLeft + imageControl.Value.ActualWidth;
                locationInfo.ItemBottom = locationInfo.ItemTop + imageControl.Value.ActualHeight;

                this.UpdateLine(moveControl.ReportItemName, locationInfo, Direction.Top);
                this.UpdateLine(moveControl.ReportItemName, locationInfo, Direction.Left);
                this.UpdateLine(moveControl.ReportItemName, locationInfo, Direction.Right);
                this.UpdateLine(moveControl.ReportItemName, locationInfo, Direction.Bottom);
                Point dropLocation = Mouse.GetPosition(this.designCanvas);
                Rect dropRect = new Rect(dropLocation.X, dropLocation.Y, 1, 1);
                Canvas dropCanvas = (this.AdornedElement as IReportItemControl).Parent as Canvas;

                var dropContainerLocation = (from container in this.designerItemsInfo
                                             where container.Bounds != Rect.Empty && container.Bounds.IntersectsWith(dropRect)
                                             select container).ToList();

                var dropContainers = (from container in dropContainerLocation
                                      where (container.ReportItemArea == this.designPanel.headerCanvas || container.ReportItemArea == this.designPanel.footerCanvas) && container.Bounds.IntersectsWith(dropRect) 
                                     select container).ToList();

                if (dropContainerLocation.Count > 0)
                {
                    dropContainerLocation.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                    {
                        return (first.ItemLeft).CompareTo(second.ItemLeft);
                    });

                    dropCanvas = dropContainerLocation.Last().ReportItemArea;
                }

                if (moveControl.ReportItem == reportControl)
                {
                    this.UpdateMovingHelpingLines(moveControl, locationInfo,dropCanvas);
                }


                var reportItems = (from reportItem in this.designPanel.SelectedReportItems.OfType<IReportItemControl>()
                                   where !this.CanDrop(reportItem)
                                   select reportItem).ToList();

                bool canDrop = true;

                if (dropContainers.Count > 0 && reportItems.Count > 0)
                {
                    canDrop = false;
                }

                if (dropCanvas != reportControl.Parent)
                {
                    System.Windows.Resources.StreamResourceInfo info = null;

                    if (canDrop)
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/DropCursor.cur", UriKind.RelativeOrAbsolute));
                        movingThumb.Cursor = new System.Windows.Input.Cursor(info.Stream);
                    }
                    else
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/Invalid.cur", UriKind.RelativeOrAbsolute));
                        movingThumb.Cursor = new System.Windows.Input.Cursor(info.Stream);
                        canDrop = true;
                    }
                }

                else
                {
                    movingThumb.Cursor = Cursors.SizeAll;
                }

            }
        }

        bool CanDrop(IReportItemControl reportItem)
        {
            if (reportItem.ItemType == DrawingReportItem.Rectangle)
            {
                RectangleControl rectangle = reportItem as RectangleControl;

                foreach (IReportItemControl rectchildren in rectangle.Children.OfType<IReportItemControl>())
                {
                    if (rectchildren.ItemType == DrawingReportItem.Tablix || rectchildren.ItemType == DrawingReportItem.Gauge || rectchildren.ItemType == DrawingReportItem.List
                        || rectchildren.ItemType == DrawingReportItem.SubReport || rectchildren.ItemType == DrawingReportItem.Chart)
                    {
                        return false;
                    }
                    else if (rectchildren.ItemType == DrawingReportItem.Rectangle)
                    {
                        return CanDrop(rectchildren);
                    }
                }
            }
            else if (reportItem.ItemType == DrawingReportItem.Tablix || reportItem.ItemType == DrawingReportItem.Gauge || reportItem.ItemType == DrawingReportItem.List
                        || reportItem.ItemType == DrawingReportItem.SubReport || reportItem.ItemType == DrawingReportItem.Chart)
            {
                return false;
            }

            return true;
        }

        void UpdateResizingHelpingLines(IReportItemControl reportItem,Direction direction)
        {

            double leftval = 0;
            double topval = 0;

            if (reportItem.Parent is IReportItemControl && !(reportItem.Parent.Parent is CellContentsControl))
            {
                leftval = (reportItem.Parent as IReportItemControl).ItemLeft;
                topval = (reportItem.Parent as IReportItemControl).ItemTop;
            }

            else if (reportItem.Parent.Parent is CellContentsControl)
            {
                FrameworkElement contentControl = (FrameworkElement)(reportItem.Parent.Parent as CellContentsControl);
                TablixControl parentElement = contentControl.Parent as TablixControl;
                ReportItemLocationInfo reportItemLocation = this.designPanel.GetTablixItemLocationInfo(contentControl, parentElement);
                leftval = reportItemLocation.ItemLeft + parentElement.ItemLeft;
                topval = reportItemLocation.ItemTop + parentElement.ItemTop;
            }

            ResizingHelpingLine resizeLineWidth = this.resizingWidthSizes[reportItem.ItemName];
            resizeLineWidth.left.X1 = resizeLineWidth.X1 = resizeLineWidth.left.X2 = reportItem.ItemLeft + leftval; ;
            resizeLineWidth.left.Y1 =resizeLineWidth.right.Y1= reportItem.ItemTop + reportItem.ItemHeight+topval;
            resizeLineWidth.left.Y2 = resizeLineWidth.right.Y2= resizeLineWidth.left.Y1 + 25;
            resizeLineWidth.right.X1 =resizeLineWidth.X2= resizeLineWidth.right.X2 = reportItem.ItemLeft + reportItem.ItemWidth+leftval;
            resizeLineWidth.Y1 =resizeLineWidth.Y2= resizeLineWidth.left.Y2-5;
            resizeLineWidth.Text = (reportItem.ItemWidth / 96).ToString("#0.0") + "in";
            Canvas.SetLeft(resizeLineWidth.InnerTextblock, (resizeLineWidth.X1+resizeLineWidth.X2) / 2);
            Canvas.SetTop(resizeLineWidth.InnerTextblock, resizeLineWidth.Y1);

            ResizingHelpingLine resizeLineHeight = this.resizingHeightSizes[reportItem.ItemName];
            resizeLineHeight.left.X1 =resizeLineHeight.right.X1= reportItem.ItemLeft + reportItem.ItemWidth+leftval;
            resizeLineHeight.left.Y1 =resizeLineHeight.Y1= resizeLineHeight.left.Y2 = reportItem.ItemTop+topval;
            resizeLineHeight.left.X2 =resizeLineHeight.right.X2= resizeLineHeight.left.X1 + 25;
            resizeLineHeight.right.Y1 = resizeLineHeight.left.Y1 + reportItem.ItemHeight;
            resizeLineHeight.right.Y2 =resizeLineHeight.Y2= resizeLineHeight.left.Y2 + reportItem.ItemHeight;
            resizeLineHeight.X1 = resizeLineHeight.left.X2-5;
            resizeLineHeight.X2 = resizeLineHeight.right.X2-5;
            resizeLineHeight.Text = (reportItem.ItemHeight / 96).ToString("#0.0") + "in";
            Canvas.SetLeft(resizeLineHeight.InnerTextblock, resizeLineHeight.X1);
            Canvas.SetTop(resizeLineHeight.InnerTextblock, (resizeLineHeight.Y1 + resizeLineHeight.Y2) / 2);

        }

        void movingThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.designPanel.drawingGrid != null)
            {
                Grid.SetZIndex(this.designPanel.drawingGrid, 0);
                this.designPanel.drawingGrid = null;
            }

            if (this.designCanvas != null)
            {
                Point dropLocation = Mouse.GetPosition(this.designCanvas);
                Rect dropRect = new Rect(dropLocation.X, dropLocation.Y, 1, 1);
                Canvas dropCanvas = (this.AdornedElement as IReportItemControl).Parent as Canvas;

               

                var dropContainerLocation = (from container in this.designerItemsInfo
                                            where container.Bounds != Rect.Empty && container.Bounds.IntersectsWith(dropRect)
                                            select container).ToList();

                if (dropContainerLocation.Count > 0)
                {
                    dropContainerLocation.Sort(delegate(ReportItemLocationInfo first, ReportItemLocationInfo second)
                    {
                        return (first.ItemLeft).CompareTo(second.ItemLeft);
                    });

                    dropCanvas = dropContainerLocation.Last().ReportItemArea;
                }


                var dropContainers = (from container in dropContainerLocation
                                      where (container.ReportItemArea == this.designPanel.headerCanvas || container.ReportItemArea == this.designPanel.footerCanvas) && container.Bounds.IntersectsWith(dropRect)
                                      select container).ToList();

                var reportItems = (from reportItem in this.designPanel.SelectedReportItems.OfType<IReportItemControl>()
                                   where !this.CanDrop(reportItem)
                                   select reportItem).ToList();

                bool canDrop = true;

                if (dropContainers.Count > 0 && reportItems.Count > 0)
                {
                    canDrop = false;
                }

                if (canDrop)
                {
                    EditAction editAction = new EditAction();
                    editAction.MovedReportItems = new List<MovingChange>();
                    editAction.EditingType = EditActionType.ItemMove;
                    this.designPanel.EditingManager.AddAction(editAction);
                    this.designPanel.EditingManager.IsMergeAction = true;

                    foreach (var imageControl in this.reportItemImages)
                    {
                        var moveControl = imageControl.Value; 
                        FrameworkElement element = moveControl as FrameworkElement;
                        double topVal = Canvas.GetTop(element);
                        double leftVal = Canvas.GetLeft(element);

                        Point screenPoint = this.designCanvas.PointToScreen(new Point(leftVal, topVal));
                        Point controlPoint = dropCanvas.PointFromScreen(screenPoint);
                        topVal = controlPoint.Y;
                        leftVal = controlPoint.X;
                        movingThumb.Cursor = Cursors.SizeAll;

                        MovingChange change = new MovingChange();
                        change.ReportItem = moveControl.ReportItem;
                        change.OldLeft = moveControl.ReportItem.ItemLeft;
                        change.OldTop = moveControl.ReportItem.ItemTop;
                        change.OldHeight = moveControl.ReportItem.ItemHeight;
                        change.OldWidth = moveControl.ReportItem.ItemWidth;
                        change.NewParent = dropCanvas;
                        change.OldParent = moveControl.ReportItem.Parent;
                        this.ArrangeAdorner(change.ReportItem);

                        if (dropCanvas != change.OldParent)
                        {
                            moveControl.ReportItem.ReportItem = moveControl.ReportItem.GetReportItem();

                            if (moveControl.ReportItem.ItemType == DrawingReportItem.Tablix)
                            {
                                this.designPanel.UpdateTablixReportItems(moveControl.ReportItem as TablixControl);
                            }
                            else if (moveControl.ReportItem.ItemType == DrawingReportItem.Rectangle)
                            {
                                this.designPanel.UpdateRectangleReportItems(moveControl.ReportItem as RectangleControl);
                            }

                            change.OldParent.Children.Remove(moveControl.ReportItem as UIElement);
                            moveControl.ReportItem.RaiseReportItemSizeChangedEvent();
                            change.NewParent.Children.Add(moveControl.ReportItem as UIElement);
                            moveControl.ReportItem.Parent = change.NewParent;
                        }

                        if (moveControl.ReportItem.ItemType == DrawingReportItem.Line)
                        {
                            if (moveControl.ItemHeight < 0)
                            {
                                topVal += moveControl.Height;
                            }

                            if (moveControl.ItemWidth < 0)
                            {
                                leftVal += moveControl.Width;
                            }

                            moveControl.ReportItem.ItemTop = topVal;
                            moveControl.ReportItem.ItemLeft = leftVal;
                            moveControl.ReportItem.ItemWidth = moveControl.ItemWidth;
                            moveControl.ReportItem.ItemHeight = moveControl.ItemHeight;
                            this.ArrangeAdorner(moveControl.ReportItem);
                        }
                        else
                        {
                            moveControl.ReportItem.ItemTop = topVal;
                            moveControl.ReportItem.ItemLeft = leftVal;
                        }

                        leftVal = (leftVal < 0) ? 0 : leftVal;
                        topVal = (topVal < 0) ? 0 : topVal;

                        change.NewLeft = leftVal;
                        change.NewTop = topVal;
                        change.NewHeight = change.ReportItem.ItemHeight;
                        change.NewWidth = change.ReportItem.ItemWidth;
                        editAction.MovedReportItems.Add(change);
                        this.ArrangeAdorner(change.ReportItem);
                    }

                    this.designPanel.EditingManager.IsMergeAction = false;
                }
            }

            foreach (var imageControl in this.reportItemImages)
            {
                var moveControl = imageControl.Value;   
                FrameworkElement element = moveControl as FrameworkElement;
                FrameworkElement imageControlParent = element.Parent as FrameworkElement;
                ((Canvas)imageControlParent).Children.Remove(element);
            }

            this.designerItemsInfo.Clear();

            this.selectedReportItemsParentInfo.Clear();
            this.reportItemImages.Clear();
            this.ClearLines();
            
            this.designPanel.EditingManager.IsMergeAction = true;

            foreach (var selectedItem in this.designPanel.SelectedReportItems)
            {
                selectedItem.UpdateItemSizeProperties();
                selectedItem.RaiseReportItemSizeChangedEvent();
            }

            movingThumb.Cursor = Cursors.SizeAll;
            this.designPanel.EditingManager.IsMergeAction = false;
        }

        void ReportItemResizingCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.designPanel.drawingGrid != null)
            {
                Grid.SetZIndex(this.designPanel.drawingGrid, 0);
                this.designPanel.drawingGrid = null;
            }

            if (this.designCanvas != null)
            {
                EditAction editAction = new EditAction();
                editAction.ResizedReportItems = new List<SizingChange>();

                foreach (var moveControl in this.reportItemImages)
                {
                    FrameworkElement element = moveControl.Value as FrameworkElement;
                    (element.Parent as Canvas).Children.Remove(element);

                    SizingChange change = new SizingChange();
                    change.ReportItem = moveControl.Value.ReportItem;
                    change.OldWidth = moveControl.Value.ItemWidth;
                    change.OldHeight = moveControl.Value.ItemHeight;
                    change.OldLeft = moveControl.Value.ItemLeft;
                    change.OldTop = moveControl.Value.ItemTop;
                    change.NewHeight = change.ReportItem.ItemHeight;
                    change.NewWidth = change.ReportItem.ItemWidth;
                    change.NewLeft = change.ReportItem.ItemLeft;
                    change.NewTop = change.ReportItem.ItemTop;
                    editAction.ResizedReportItems.Add(change);
                }

                editAction.EditingType = EditActionType.ItemResize;
                this.designPanel.EditingManager.AddAction(editAction);

                this.selectedReportItemsParentInfo.Clear();
                this.reportItemImages.Clear();
                this.ClearLines();
            }

            this.designPanel.EditingManager.IsMergeAction = true;

            foreach (var selectedItem in this.designPanel.SelectedReportItems)
            {
                Canvas.SetZIndex(selectedItem as UIElement, 0);
                (selectedItem as FrameworkElement).MaxHeight = double.PositiveInfinity;
                (selectedItem as FrameworkElement).MaxWidth = double.PositiveInfinity;
                selectedItem.UpdateItemSizeProperties();
                selectedItem.RaiseReportItemSizeChangedEvent();

                if (selectedItem.ItemType == DrawingReportItem.Image)
                {
                    var imgControl = (Syncfusion.Windows.Reports.Designer.Controls.ImageControl)selectedItem;
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.ItemChanged;
                    ItemChange change = new ItemChange();
                    change.ReportItem = imgControl;
                    change.OldValue = imgControl.GetReportItem();
                    imgControl.UpdateImageSource();
                    change.NewValue = imgControl.GetReportItem();
                    action.ItemChange = change;
                    this.designPanel.EditingManager.AddAction(action);
                }
            }

            this.designPanel.EditingManager.IsMergeAction = false;
        }

        // Handler for resizing from the bottom-right.
        void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            Thumb hitThumb = sender as Thumb;

            if (hitThumb == null)
                return;

            this.UpdateLineVisiblity();

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(this.AdornedElement as FrameworkElement);

            double diffHeight = Mouse.GetPosition(null).Y - this.startMousePoistion.Y;
            double diffWidth = Mouse.GetPosition(null).X - this.startMousePoistion.X;

            foreach (var reportItem in this.designPanel.SelectedReportItems)
            {
                FrameworkElement uiElement = reportItem as FrameworkElement;
                EnforceSize(uiElement);

                var reportItemImage = this.reportItemImages[reportItem.ItemName];

                if (reportItem is LineControl)
                {
                    reportItem.ItemHeight = reportItemImage.ItemHeight + diffHeight;
                    uiElement.Height = reportItem.ItemHeight > 0 ? reportItem.ItemHeight : reportItem.ItemTop;
                    reportItem.ItemWidth = reportItemImage.ItemWidth + diffWidth;
                    uiElement.Width = reportItem.ItemWidth > 0 ? reportItem.ItemWidth : reportItem.ItemLeft;
                    this.ArrangeAdorner(reportItem);
                }
                else
                {
                    uiElement.Width = Math.Max(reportItemImage.ItemWidth + diffWidth, hitThumb.DesiredSize.Width);
                    uiElement.Height = Math.Max( reportItemImage.ItemHeight + diffHeight, hitThumb.DesiredSize.Height);
                }

                double left = reportItem.ItemLeft;
                double top = reportItem.ItemTop;

                ReportItemLocationInfo parentInfo = this.selectedReportItemsParentInfo[reportItem.ItemName];
                ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
                locationInfo.ItemTop = top + parentInfo.ItemTop;
                locationInfo.ItemLeft = left + parentInfo.ItemLeft;
                locationInfo.ItemRight = locationInfo.ItemLeft + reportItem.ItemWidth;
                locationInfo.ItemBottom = locationInfo.ItemTop + reportItem.ItemHeight;                
                this.UpdateLine(reportItem.ItemName, locationInfo, Direction.BottomRight);
                this.UpdateResizingHelpingLines(reportItem,Direction.BottomRight);
                this.UppdateMapSize(uiElement);
            }
        }

        void HandleRightMiddle(object sender, DragDeltaEventArgs args)
        {
            this.UpdateLineVisiblity();

            EnforceSize(this.AdornedElement as FrameworkElement);
            double diffWidth = Mouse.GetPosition(null).X - this.startMousePoistion.X;

            foreach (var reportItem in this.designPanel.SelectedReportItems)
            {
                var children = reportItem as UIElement;
                this.reportControl.IsFocusedItem = false;
                FrameworkElement uiElement = children as FrameworkElement;
                Thumb hitThumb = sender as Thumb;

                var reportItemImage = this.reportItemImages[reportItem.ItemName];
                uiElement.Width = Math.Max(reportItemImage.ItemWidth + diffWidth, hitThumb.DesiredSize.Width);

                if (reportItem.ItemType == DrawingReportItem.Line)
                {
                    this.ArrangeAdorner(reportItem);
                }

                double left = reportItem.ItemLeft + args.HorizontalChange;
                double top = reportItem.ItemTop + args.VerticalChange;

                ReportItemLocationInfo parentInfo = this.selectedReportItemsParentInfo[reportItem.ItemName];
                ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
                locationInfo.ItemTop = top + parentInfo.ItemTop;
                locationInfo.ItemLeft = left + parentInfo.ItemLeft;
                locationInfo.ItemRight = locationInfo.ItemLeft + reportItem.ItemWidth;
                locationInfo.ItemBottom = locationInfo.ItemTop + reportItem.ItemHeight;
                this.UpdateLine(reportItem.ItemName, locationInfo, Direction.RightMiddle);
                this.UpdateResizingHelpingLines(reportItem,Direction.RightMiddle);
                this.UppdateMapSize(uiElement);
            }
        }

        // Handler for resizing from the top-right.
        void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            Thumb hitThumb = sender as Thumb;

            if (hitThumb == null)
                return;

            this.UpdateLineVisiblity();

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(this.AdornedElement as FrameworkElement);

            double diffHeight = Mouse.GetPosition(null).Y - this.startMousePoistion.Y;
            double diffWidth = Mouse.GetPosition(null).X - this.startMousePoistion.X;

            foreach (var reportItem in this.designPanel.SelectedReportItems)
            {
                var uiElement = reportItem as UIElement;
                this.reportControl.IsFocusedItem = false;

                FrameworkElement frameWorkElement = uiElement as FrameworkElement;

                EnforceSize(frameWorkElement);

                var reportItemImage = this.reportItemImages[reportItem.ItemName];

                double reportItemTop = reportItemImage.ItemTop + diffHeight;

                double widthVal = reportItemImage.ItemWidth + diffWidth;
                double heightVal = reportItemImage.ItemHeight - diffHeight;

                if ( widthVal > hitThumb.DesiredSize.Width)
                {
                    reportItem.ItemWidth = widthVal;
                }

                if (reportItemTop != reportItem.ItemTop && reportItemTop > 0 && heightVal > hitThumb.DesiredSize.Height)
                {
                    reportItem.ItemTop = reportItemTop;
                    reportItem.ItemHeight = heightVal;
                }

                if (reportItem.ItemType == DrawingReportItem.Line)
                {
                    this.ArrangeAdorner(reportItem);
                }
               
                double left = reportItem.ItemLeft;
                double top = reportItem.ItemTop;
                ReportItemLocationInfo parentInfo = this.selectedReportItemsParentInfo[reportItem.ItemName];
                ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
                locationInfo.ItemTop = top + parentInfo.ItemTop;
                locationInfo.ItemLeft = left + parentInfo.ItemLeft;
                locationInfo.ItemRight = locationInfo.ItemLeft + reportItem.ItemWidth;
                locationInfo.ItemBottom = locationInfo.ItemTop + reportItem.ItemHeight;
                this.UpdateLine(reportItem.ItemName, locationInfo, Direction.TopRight);
                this.UpdateResizingHelpingLines(reportItem, Direction.TopRight);
                this.UppdateMapSize(frameWorkElement);
            }

        }

        // Handler for resizing from the top-left.
        void HandleTopLeftDrag(object sender, DragDeltaEventArgs args)
        {
            Thumb hitThumb = sender as Thumb;

            if (hitThumb == null)
                return;

            this.UpdateLineVisiblity();

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(this.AdornedElement as FrameworkElement);

            double diffHeight = Mouse.GetPosition(null).Y - this.startMousePoistion.Y;
            double diffWidth = Mouse.GetPosition(null).X - this.startMousePoistion.X;          

            foreach (var reportItem in this.designPanel.SelectedReportItems)
            {
                FrameworkElement frameWorkElement = reportItem as FrameworkElement;

                EnforceSize(frameWorkElement);

                var reportItemImage = this.reportItemImages[reportItem.ItemName];

                double reportItemLeft = reportItemImage.ItemLeft + diffWidth;
                double reportItemTop = reportItemImage.ItemTop + diffHeight;

                if (reportItem.ItemType == DrawingReportItem.Line)
                {
                    if (reportItemTop >= 0)
                    {
                        reportItem.ItemLeft = reportItemLeft;
                    }
                    if (reportItemLeft >= 0)
                    {
                        reportItem.ItemTop = reportItemTop;
                    }

                    frameWorkElement.Height = frameWorkElement.ActualHeight;
                    frameWorkElement.Width = frameWorkElement.ActualWidth;
                    this.ArrangeAdorner(reportItem);
                }
                else
                {
                    double widthVal = reportItemImage.ItemWidth - diffWidth;
                    double heightVal = reportItemImage.ItemHeight - diffHeight;

                    if (reportItemLeft != reportItem.ItemLeft && reportItemLeft > 0 && widthVal > hitThumb.DesiredSize.Width)
                    {
                        reportItem.ItemLeft = reportItemLeft;
                        reportItem.ItemWidth = widthVal;
                    }

                    if (reportItemTop != reportItem.ItemTop && reportItemTop > 0 && heightVal > hitThumb.DesiredSize.Height) 
                    {
                        reportItem.ItemTop = reportItemTop;
                        reportItem.ItemHeight = heightVal;
                    }
                }

                double left = reportItem.ItemLeft;
                double top = reportItem.ItemTop;
                ReportItemLocationInfo parentInfo = this.selectedReportItemsParentInfo[reportItem.ItemName];
                ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
                locationInfo.ItemTop = top + parentInfo.ItemTop;
                locationInfo.ItemLeft = left + parentInfo.ItemLeft;
                locationInfo.ItemRight = locationInfo.ItemLeft + reportItem.ItemWidth;
                locationInfo.ItemBottom = locationInfo.ItemTop + reportItem.ItemHeight;
                this.UpdateLine(reportItem.ItemName, locationInfo, Direction.TopLeft);
                this.UpdateResizingHelpingLines(reportItem, Direction.TopLeft);
                this.UppdateMapSize(frameWorkElement);
            }
        }

        // Handler for resizing from the bottom-left.
        void HandleLeftMiddleDrag(object sender, DragDeltaEventArgs args)
        {
            Thumb hitThumb = sender as Thumb;

            if (hitThumb == null)
                return;

            this.UpdateLineVisiblity();

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(this.AdornedElement as FrameworkElement);

            double diffWidth = Mouse.GetPosition(null).X - this.startMousePoistion.X;

            foreach (var reportItem in this.designPanel.SelectedReportItems)
            {
                var uiElement = reportItem as UIElement;
                this.reportControl.IsFocusedItem = false;
                FrameworkElement frameWorkElement = uiElement as FrameworkElement;
                EnforceSize(frameWorkElement);
                var reportItemImage = this.reportItemImages[reportItem.ItemName];
                double reportItemLeft = reportItemImage.ItemLeft + diffWidth;
                double widthVal = reportItemImage.ItemWidth - diffWidth;

                if (reportItemLeft != reportItem.ItemLeft && reportItemLeft > 0 && widthVal > hitThumb.DesiredSize.Width)
                {
                    reportItem.ItemLeft = reportItemLeft;
                    reportItem.ItemWidth = widthVal;
                }

                if (reportItem.ItemType == DrawingReportItem.Line)
                {
                    this.ArrangeAdorner(reportItem);
                }

                double left = reportItem.ItemLeft;
                double top = reportItem.ItemTop;
                ReportItemLocationInfo parentInfo = this.selectedReportItemsParentInfo[reportItem.ItemName];
                ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
                locationInfo.ItemTop = top + parentInfo.ItemTop;
                locationInfo.ItemLeft = left + parentInfo.ItemLeft;
                locationInfo.ItemRight = locationInfo.ItemLeft + reportItem.ItemWidth;
                locationInfo.ItemBottom = locationInfo.ItemTop + reportItem.ItemHeight;
                this.UpdateLine(reportItem.ItemName, locationInfo, Direction.LeftMiddle);
                this.UpdateResizingHelpingLines(reportItem,Direction.LeftMiddle);
                this.UppdateMapSize(frameWorkElement);
            }
        }

        // Handler for resizing from the bottom-left.
        void HandleBottomLeftDrag(object sender, DragDeltaEventArgs args)
        {
            Thumb hitThumb = sender as Thumb;

            if (hitThumb == null)
                return;

            this.UpdateLineVisiblity();

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(this.AdornedElement as FrameworkElement);

            double diffHeight = Mouse.GetPosition(null).Y - this.startMousePoistion.Y;
            double diffWidth = Mouse.GetPosition(null).X - this.startMousePoistion.X;

            foreach (var reportItem in this.designPanel.SelectedReportItems)
            {
                var uiElement = reportItem as UIElement;
                this.reportControl.IsFocusedItem = false;

                FrameworkElement frameWorkElement = uiElement as FrameworkElement;

                EnforceSize(frameWorkElement);

                var reportItemImage = this.reportItemImages[reportItem.ItemName];

                double reportItemLeft = reportItemImage.ItemLeft + diffWidth;
                double reportItemTop = reportItemImage.ItemTop + diffHeight;

                double widthVal = reportItemImage.ItemWidth - diffWidth;
                double heightVal = reportItemImage.ItemHeight + diffHeight;

                if (reportItemLeft != reportItem.ItemLeft && reportItemLeft > 0 && widthVal > hitThumb.DesiredSize.Width)
                {
                    reportItem.ItemLeft = reportItemLeft;
                    reportItem.ItemWidth = widthVal;
                }

                if (heightVal > hitThumb.DesiredSize.Height)
                {
                    reportItem.ItemHeight = heightVal;
                }

                if (reportItem.ItemType == DrawingReportItem.Line)
                {
                    this.ArrangeAdorner(reportItem);
                }

                double left = reportItem.ItemLeft;
                double top = reportItem.ItemTop;
                ReportItemLocationInfo parentInfo = this.selectedReportItemsParentInfo[reportItem.ItemName];
                ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
                locationInfo.ItemTop = top + parentInfo.ItemTop;
                locationInfo.ItemLeft = left + parentInfo.ItemLeft;
                locationInfo.ItemRight = locationInfo.ItemLeft + reportItem.ItemWidth;
                locationInfo.ItemBottom = locationInfo.ItemTop + reportItem.ItemHeight;
                this.UpdateLine(reportItem.ItemName, locationInfo, Direction.BottomLeft);
                this.UpdateResizingHelpingLines(reportItem, Direction.BottomLeft);
                this.UppdateMapSize(frameWorkElement);
            }
        }

        void HandleTopMiddleDrag(object sender, DragDeltaEventArgs args)
        {
            this.UpdateLineVisiblity();

            EnforceSize(this.AdornedElement as FrameworkElement);

            double diffHeight = Mouse.GetPosition(null).Y - this.startMousePoistion.Y;

            foreach (var reportItem in this.designPanel.SelectedReportItems)
            {
                var uiElement = reportItem as UIElement;
                this.reportControl.IsFocusedItem = false;

                FrameworkElement frameWorkElement = uiElement as FrameworkElement;

                EnforceSize(frameWorkElement);

                var reportItemImage = this.reportItemImages[reportItem.ItemName];

                double reportItemTop = reportItemImage.ItemTop + diffHeight;
                FrameworkElement adornedElement = uiElement as FrameworkElement;
                Thumb hitThumb = sender as Thumb;
                if (adornedElement == null || hitThumb == null) return;

                double heightVal = reportItemImage.ItemHeight - diffHeight;

                if (reportItemTop != reportItem.ItemTop && reportItemTop > 0 && heightVal > hitThumb.DesiredSize.Height)
                {
                    reportItem.ItemTop = reportItemTop;
                    reportItem.ItemHeight = heightVal;
                }

                if (reportItem.ItemType == DrawingReportItem.Line)
                {
                    this.ArrangeAdorner(reportItem);
                }

                double left = reportItem.ItemLeft + args.HorizontalChange;
                double top = reportItem.ItemTop + args.VerticalChange;
                ReportItemLocationInfo parentInfo = this.selectedReportItemsParentInfo[reportItem.ItemName];
                ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
                locationInfo.ItemTop = top + parentInfo.ItemTop;
                locationInfo.ItemLeft = left + parentInfo.ItemLeft;
                locationInfo.ItemRight = locationInfo.ItemLeft + reportItem.ItemWidth;
                locationInfo.ItemBottom = locationInfo.ItemTop + reportItem.ItemHeight;
                this.UpdateLine(reportItem.ItemName, locationInfo, Direction.TopMidddle);
                this.UpdateResizingHelpingLines(reportItem, Direction.TopMidddle);
                this.UppdateMapSize(frameWorkElement);
            }
        }

        void HandleBottomMiddleDrag(object sender, DragDeltaEventArgs args)
        {
            this.UpdateLineVisiblity();

            EnforceSize(this.AdornedElement as FrameworkElement);
            double diffHeight = Mouse.GetPosition(null).Y - this.startMousePoistion.Y;

            foreach (var reportItem in this.designPanel.SelectedReportItems)
            {
                var children = reportItem as UIElement;
                this.reportControl.IsFocusedItem = false;
                FrameworkElement uiElement = children as FrameworkElement;
                Thumb hitThumb = sender as Thumb;

                var reportItemImage = this.reportItemImages[reportItem.ItemName];
                uiElement.Height = Math.Max(reportItemImage.ItemHeight + diffHeight, hitThumb.DesiredSize.Height);

                if (reportItem.ItemType == DrawingReportItem.Line)
                {
                    this.ArrangeAdorner(reportItem);
                }

                double left = reportItem.ItemLeft + args.HorizontalChange;
                double top = reportItem.ItemTop + args.VerticalChange;
                ReportItemLocationInfo parentInfo = this.selectedReportItemsParentInfo[reportItem.ItemName];
                ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
                locationInfo.ItemTop = top + parentInfo.ItemTop;
                locationInfo.ItemLeft = left + parentInfo.ItemLeft;
                locationInfo.ItemRight = locationInfo.ItemLeft + reportItem.ItemWidth;
                locationInfo.ItemBottom = locationInfo.ItemTop + reportItem.ItemHeight;
                this.UpdateLine(reportItem.ItemName, locationInfo, Direction.BottomMiddle);
                this.UpdateResizingHelpingLines(reportItem, Direction.BottomMiddle);
                this.UppdateMapSize(uiElement);
            }
        }

        void UppdateMapSize(FrameworkElement uiElement)
        {
#if !SyncfusionFramework3_5
            if (uiElement is MapControl)
            {
                (uiElement as MapControl).InternalMap.Height = uiElement.Height;
                (uiElement as MapControl).InternalMap.Width = uiElement.Width;
                foreach (var layer in (uiElement as MapControl).InternalMap.Layers)
                {
                    if (ActualHeight > 5)
                        layer.Height = ActualHeight - 5;
                    if (ActualWidth > 5)
                        layer.Width = ActualWidth - 5;
                }

            }
#endif
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.
            // These will be used to place the ResizingAdorner at the corners of the adorned element.
           
            double desiredWidth = AdornedElement.DesiredSize.Width;
            double desiredHeight = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well.
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            IReportItemControl reportControl = this.AdornedElement as IReportItemControl;

            if (reportControl.ItemType == DrawingReportItem.Line)
            {
                topLeft.Arrange(new Rect((reportControl as LineControl).InnerLine.X1-8, (reportControl as LineControl).InnerLine.Y1-4, 10, 10));
                bottomRight.Arrange(new Rect((reportControl as LineControl).InnerLine.X2-8, (reportControl as LineControl).InnerLine.Y2-4, 10, 10));
                movingThumb.Arrange(new Rect(reportControl.ItemLeft + reportControl.ItemWidth / 15,reportControl.ItemTop + reportControl.ItemHeight / 15, 20, 20));

            }
            else
            {
                topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
                leftMiddle.Arrange(new Rect(-adornerWidth / 2, 0, adornerWidth, adornerHeight));
                bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
                topMidddle.Arrange(new Rect(0, -adornerHeight / 2, adornerWidth, adornerHeight));
                topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
                rightMiddle.Arrange(new Rect(desiredWidth - adornerWidth / 2, 0, adornerWidth, adornerHeight));
                bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
                bottomMiddle.Arrange(new Rect(0, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
                movingThumb.Arrange(new Rect(-adornerWidth / 2.3, -adornerHeight / 2, adornerWidth, adornerHeight));
            }


            // Return the final size.
            return finalSize;
        }

        void ArrangeAdorner(IReportItemControl reportItem)
        {
            Adorner layer = AdornerLayer.GetAdornerLayer(reportItem as UIElement).GetAdorners(reportItem as UIElement).First();
            layer.InvalidateArrange();
        }

        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();
            cornerThumb.Style = (Style)this.designPanel.FindResource("ThumbStyle");
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 8;
            visualChildren.Add(cornerThumb);
        }

        void BuildMovingThumb(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 18;
            cornerThumb.Style = (Style)this.designPanel.FindResource("MovingThumbStyle");
            MenuItem delete = new MenuItem { Header = "Delete" };
            cornerThumb.ContextMenu = new ContextMenu();

            delete.Click += new RoutedEventHandler(delete_Click);
            cornerThumb.ContextMenu.Items.Add(delete);
            visualChildren.Add(cornerThumb);
        }

        void delete_Click(object sender, RoutedEventArgs e)
        {
            this.designPanel.DeleteSelectedReportItems();
        }

        void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            FrameworkElement parent = adornedElement.Parent as FrameworkElement;

            adornedElement.MaxHeight = double.PositiveInfinity;
            adornedElement.MaxWidth = double.PositiveInfinity;
        }

        protected override int VisualChildrenCount { get { return visualChildren.Count; } }

        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

        private enum Direction
        {
            Top,
            Left,
            Bottom,
            Right,
            TopLeft, 
            TopRight,
            TopMidddle,
            BottomLeft, 
            BottomRight, 
            BottomMiddle, 
            LeftMiddle, 
            RightMiddle
        }
    }
}
