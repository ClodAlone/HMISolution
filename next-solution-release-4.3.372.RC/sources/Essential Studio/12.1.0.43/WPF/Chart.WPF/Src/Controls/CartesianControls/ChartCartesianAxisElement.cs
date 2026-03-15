// <copyright file="ChartCartesianAxisElement.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Media;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Collections.Specialized;

    /// <summary>
    /// This element renders the axis line and ticks.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartCartesianAxisElement : FrameworkElement
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Axis dependency property.
        /// </summary>
        /// <summary>
        /// Using a DependencyProperty as the backing store for Axis. This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register("Axis", typeof(ChartAxis), typeof(ChartCartesianAxisElement));

        /// <summary>
        /// Identifies the LineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeProperty =
            ChartAxis.LineStrokeProperty.AddOwner(typeof(ChartCartesianAxisElement));

        /// <summary>
        /// Identifies the TickSize dependency property.
        /// </summary>
        public static readonly DependencyProperty TickSizeProperty =
            ChartAxis.TickSizeProperty.AddOwner(typeof(ChartCartesianAxisElement));

        /// <summary>
        /// Identifies the Labels dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(ChartAxisLabelsCollection), typeof(ChartCartesianAxisElement), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelsChanged)));

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            ChartAxis.OrientationProperty.AddOwner(typeof(ChartCartesianAxisElement));

        /// <summary>
        /// Identifies the Caps dependency property.
        /// </summary>
        public static readonly DependencyProperty CapsProperty =
                DependencyProperty.Register("Caps", typeof(ChartAxisCap), typeof(ChartCartesianAxisElement), new FrameworkPropertyMetadata(ChartAxisCap.None, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the CapsSize dependency property.
        /// </summary>
        public static readonly DependencyProperty CapsSizeProperty =
                DependencyProperty.Register("CapsSize", typeof(Size), typeof(ChartCartesianAxisElement), new FrameworkPropertyMetadata(new Size(10, 3), FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the axis. This is a dependency property.
        /// </summary>
        /// <value>The axis value.</value>
        public ChartAxis Axis
        {
            get
            {
                return (ChartAxis)GetValue(AxisProperty);
            }

            set
            {
                SetValue(AxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation. This is a dependency property.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the labels. This is a dependency property.
        /// </summary>
        /// <value>The labels.</value>
        public ChartAxisLabelsCollection Labels
        {
            get
            {
                return (ChartAxisLabelsCollection)GetValue(LabelsProperty);
            }

            set
            {
                SetValue(LabelsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the line stroke. This is a dependency property.
        /// </summary>
        /// <value>The line stroke.</value>
        public Pen LineStroke
        {
            get
            {
                return (Pen)GetValue(LineStrokeProperty);
            }

            set
            {
                SetValue(LineStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the tick. This is a dependency property.
        /// </summary>
        /// <value>The size of the tick.</value>
        public double TickSize
        {
            get
            {
                return (double)GetValue(TickSizeProperty);
            }

            set
            {
                SetValue(TickSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the axis caps. This is a dependency property.
        /// </summary>
        /// <value>The axis caps.</value>
        public ChartAxisCap Caps
        {
            get
            {
                return (ChartAxisCap)GetValue(CapsProperty);
            }

            set
            {
                SetValue(CapsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the caps. This is a dependency property.
        /// </summary>
        /// <value>The size of the caps.</value>
        public Size CapsSize
        {
            get
            {
                return (Size)GetValue(CapsSizeProperty);
            }

            set
            {
                SetValue(CapsSizeProperty, value);
            }
        }
        #endregion

        #region Implamentation
        double plusValueY = 1d;
        double plussmallTickValye = 1d;
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            //if (Axis.LineStroke.Thickness >= 1)
            //this.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            this.ClipToBounds = false;
            if (this.Axis != null)
            {
                Rect clientRect = new Rect(this.RenderSize);
                Pen lineStroke = this.Axis.LineStroke;
                Pen ticksStroke = this.Axis.TickLineStroke;
                Pen smallTicklineStroke = this.Axis.SmallTickLineStroke;
                double tickSize = this.TickSize;
                DoubleRange visibleRange = this.Axis.VisibleRange;

                #region Draw line
                double halfPenWidth ;
                GuidelineSet guidelines;
                if (this.Orientation == Orientation.Horizontal)
                {
                    halfPenWidth = lineStroke.Thickness / 2;
                    guidelines= new GuidelineSet(); 
                    if (this.Axis.OpposedPosition)
                    {
                        guidelines.GuidelinesX.Add(clientRect.BottomLeft.X + halfPenWidth);
                        guidelines.GuidelinesY.Add(clientRect.BottomRight.Y + halfPenWidth);
                        drawingContext.PushGuidelineSet(guidelines);
                        DrawAxisLine(drawingContext, lineStroke, clientRect.BottomLeft, clientRect.BottomRight, this.Caps, this.CapsSize, this.CapsSize);
                        drawingContext.Pop();
                    }
                    else 
                    {
                        guidelines.GuidelinesX.Add(clientRect.TopLeft.X + halfPenWidth);
                        guidelines.GuidelinesY.Add(clientRect.TopRight.Y + halfPenWidth);
                        drawingContext.PushGuidelineSet(guidelines);
                        DrawAxisLine(drawingContext, lineStroke, clientRect.TopLeft, clientRect.TopRight, this.Caps, this.CapsSize, this.CapsSize);
                        drawingContext.Pop();
                    }
                    
                }
                else
                {
                    halfPenWidth = lineStroke.Thickness / 2;
                    guidelines = new GuidelineSet();
                    if (this.Axis.OpposedPosition)
                    {
                        guidelines.GuidelinesX.Add(clientRect.TopLeft.X + halfPenWidth);
                        guidelines.GuidelinesY.Add(clientRect.BottomLeft.Y + halfPenWidth);
                        drawingContext.PushGuidelineSet(guidelines);
                        DrawAxisLine(drawingContext, lineStroke, clientRect.TopLeft, clientRect.BottomLeft, this.Caps, this.CapsSize, this.CapsSize);
                        drawingContext.Pop();
                    }
                    else 
                    {
                        guidelines.GuidelinesX.Add(clientRect.TopRight.X + halfPenWidth);
                        guidelines.GuidelinesY.Add(clientRect.BottomRight.Y + halfPenWidth);
                        drawingContext.PushGuidelineSet(guidelines);
                        DrawAxisLine(drawingContext, lineStroke, clientRect.TopRight, clientRect.BottomRight, this.Caps, this.CapsSize, this.CapsSize);
                        drawingContext.Pop();
                    }
                    
                }
                #endregion
                
                #region Draw ticks
                GuidelineSet tickguidelines; 
                if (this.Axis.TicksPoint != null)
                {
                    double offsetLables = 0d;
                    switch (Axis.AxisLabelsPosition)
                    {
                        case AxisLabels.Low:
                            offsetLables = 0;
                            break;
                        case AxisLabels.High:
                            if (Axis.Orientation == Orientation.Vertical)
                            {
                                offsetLables = Axis.gridWidth;
                            }
                            else
                            {
                                offsetLables = Axis.gridheight;
                            }
                            break;
                        case AxisLabels.NextToAxis:
                            if (Axis.Orientation == Orientation.Horizontal)
                                offsetLables = Axis.XLabelOffset;
                            else
                                offsetLables = Axis.YLabelOffset;
                            break;
                    }
                    switch (Axis.TickLinesPosition)
                    {
                        case AxisPositions.Inside:
                            double ticksize = Axis.SmallTickSize;
                            if (ticksize < 0)
                            {
                                ticksize = -ticksize;
                            }
                            offsetLables = offsetLables + ticksize;
                            break;
                    }
                    foreach (object label in this.Axis.TicksPoint)
                    {
                        double pos = (double)label;
                        if (visibleRange.Inside(pos))
                        {
                            double smallTickSize = this.Axis.SmallTickSize;

                            if (Orientation == Orientation.Horizontal)
                            {
                                double x =  clientRect.Width * this.Axis.ValueToCoefficient(pos);
                                halfPenWidth = smallTicklineStroke.Thickness / 2;
                                tickguidelines = new GuidelineSet();
                                if (Axis.OpposedPosition)
                                {
                                    tickguidelines.GuidelinesX.Add(x + halfPenWidth);
                                    tickguidelines.GuidelinesY.Add(this.ActualHeight + halfPenWidth);
                                    tickguidelines.GuidelinesY.Add((this.ActualHeight - smallTickSize) + halfPenWidth);
                                    drawingContext.PushGuidelineSet(tickguidelines);
                                    drawingContext.DrawLine(smallTicklineStroke, new Point(x, this.ActualHeight), new Point(x, this.ActualHeight - smallTickSize));
                                    drawingContext.Pop();
                                }
                                else
                                {
                                    if (Axis.TickLinesPosition == AxisPositions.Cross)
                                    {
                                        double plusValueX = smallTickSize / (1 / this.Axis.SmallTickLinesRange);
                                        plussmallTickValye = smallTickSize / (1 / (1- this.Axis.SmallTickLinesRange));
                                        tickguidelines.GuidelinesX.Add(x + halfPenWidth);
                                        tickguidelines.GuidelinesY.Add((-offsetLables - plusValueX) + halfPenWidth);
                                        tickguidelines.GuidelinesY.Add(((-offsetLables) + plussmallTickValye) + halfPenWidth);
                                        drawingContext.PushGuidelineSet(tickguidelines);
                                        drawingContext.DrawLine(smallTicklineStroke, new Point(x, -offsetLables - plusValueX), new Point(x, (-offsetLables) + plussmallTickValye));
                                        drawingContext.Pop();
                                    }
                                    else
                                    {
                                        tickguidelines.GuidelinesX.Add(x + halfPenWidth);
                                        tickguidelines.GuidelinesY.Add(-offsetLables + halfPenWidth);
                                        tickguidelines.GuidelinesY.Add((smallTickSize - offsetLables) + halfPenWidth);
                                        drawingContext.PushGuidelineSet(tickguidelines);
                                        drawingContext.DrawLine(smallTicklineStroke, new Point(x, -offsetLables), new Point(x, smallTickSize - offsetLables));
                                        drawingContext.Pop();
                                    }
                                }
                                
                            }
                            else
                            {
                                double y = clientRect.Height * (1 - this.Axis.ValueToCoefficient(pos));
                                halfPenWidth = smallTicklineStroke.Thickness / 2;
                                tickguidelines = new GuidelineSet();
                                if (Axis.OpposedPosition)
                                {
                                    tickguidelines.GuidelinesX.Add(halfPenWidth);
                                    tickguidelines.GuidelinesX.Add(smallTickSize+halfPenWidth);
                                    tickguidelines.GuidelinesY.Add(y + halfPenWidth);
                                    drawingContext.PushGuidelineSet(tickguidelines);
                                    drawingContext.DrawLine(smallTicklineStroke, new Point(0, y), new Point(smallTickSize, y));
                                    drawingContext.Pop();
                                }
                                else
                                {
                                    if (Axis.TickLinesPosition == AxisPositions.Cross)
                                    {
                                        double plusValueX = smallTickSize / (1 / this.Axis.SmallTickLinesRange);
                                        plussmallTickValye = smallTickSize / (1 / (1- this.Axis.SmallTickLinesRange));
                                        tickguidelines.GuidelinesX.Add(offsetLables+ halfPenWidth);
                                        tickguidelines.GuidelinesX.Add((this.ActualWidth + offsetLables + plusValueX) + halfPenWidth);
                                        tickguidelines.GuidelinesY.Add(y + halfPenWidth);
                                        drawingContext.PushGuidelineSet(tickguidelines);
                                        drawingContext.DrawLine(smallTicklineStroke, new Point(this.ActualWidth + offsetLables + plusValueX, y), new Point(offsetLables + (this.ActualWidth - plussmallTickValye), y));
                                        drawingContext.Pop();
                                    }
                                    else
                                    {
                                        tickguidelines.GuidelinesX.Add(offsetLables + halfPenWidth);
                                        tickguidelines.GuidelinesX.Add(this.ActualWidth + offsetLables + halfPenWidth);
                                        tickguidelines.GuidelinesY.Add(y + halfPenWidth);
                                        drawingContext.PushGuidelineSet(tickguidelines);
                                        drawingContext.DrawLine(smallTicklineStroke, new Point(this.ActualWidth + offsetLables, y), new Point(offsetLables + (this.ActualWidth - smallTickSize), y));
                                        drawingContext.Pop();
                                    }
                                }
                                
                            }
                        }
                    }
                }
                double offsetLables1 = 0d;
                switch (Axis.AxisLabelsPosition)
                {
                    case AxisLabels.Low:
                        offsetLables1 = 0;
                        break;
                    case AxisLabels.High:
                        if (Axis.Orientation == Orientation.Vertical)
                        {
                            offsetLables1 = Axis.gridWidth;
                        }
                        else
                        {
                            offsetLables1 = Axis.gridheight;
                        }
                        break;
                    case AxisLabels.NextToAxis:
                        if (Axis.Orientation == Orientation.Horizontal)
                            offsetLables1 = Axis.XLabelOffset;
                        else
                            offsetLables1 = Axis.YLabelOffset;
                        break;
                }


                
                    switch (Axis.TickLinesPosition)
                    {
                        case AxisPositions.Inside:
                            double ticksize = Axis.TickSize;
                            if (ticksize < 0)
                            {
                                ticksize = -ticksize;
                            }
                            offsetLables1 = offsetLables1 + ticksize;
                            break;                    
                    }

                if(this.Labels!=null)
                foreach (ChartAxisLabel label in this.Labels)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        double x =  clientRect.Width * this.Axis.ValueToCoefficient(label.Position);
                        halfPenWidth=(ticksStroke.Thickness/2);
                        tickguidelines = new GuidelineSet();
                        if (Axis.OpposedPosition)
                        {
                            tickguidelines.GuidelinesX.Add(x + halfPenWidth);
                            tickguidelines.GuidelinesY.Add(this.ActualHeight + halfPenWidth);
                            tickguidelines.GuidelinesY.Add(this.ActualHeight - tickSize + halfPenWidth);
                            drawingContext.PushGuidelineSet(tickguidelines);
                            drawingContext.DrawLine(ticksStroke, new Point(x, this.ActualHeight), new Point(x, this.ActualHeight - tickSize));
                            drawingContext.Pop();
                        }
                        else
                        {
                            if (Axis.TickLinesPosition == AxisPositions.Cross)
                            {
                                double plusValueX = tickSize / (1 / this.Axis.TickLinesRange);
                                plusValueY = tickSize / (1 / (1- this.Axis.TickLinesRange));
                                tickguidelines.GuidelinesX.Add(x + halfPenWidth);
                                tickguidelines.GuidelinesY.Add((-offsetLables1 - plusValueX) + halfPenWidth);
                                tickguidelines.GuidelinesY.Add(((-offsetLables1) + plusValueY) + halfPenWidth);
                                drawingContext.PushGuidelineSet(tickguidelines);
                                drawingContext.DrawLine(ticksStroke, new Point(x, -offsetLables1 - plusValueX), new Point(x, (-offsetLables1) + plusValueY));
                                drawingContext.Pop();
                            }
                            else
                            {
                                tickguidelines.GuidelinesX.Add(x + halfPenWidth);
                                tickguidelines.GuidelinesY.Add((-offsetLables1) + halfPenWidth);
                                tickguidelines.GuidelinesY.Add((tickSize - offsetLables1) + halfPenWidth);
                                drawingContext.PushGuidelineSet(tickguidelines);
                                drawingContext.DrawLine(ticksStroke, new Point(x, -offsetLables1), new Point(x, tickSize - offsetLables1));
                                drawingContext.Pop();
                            }
                        }
                       
                    }
                    else
                    {
                        double y =clientRect.Height * (1 - this.Axis.ValueToCoefficient(label.Position));
                        tickguidelines = new GuidelineSet();
                        if (Axis.OpposedPosition)
                        {
                            //if (this.Axis != null && this.Axis.Area != null)
                            //{
                            //    if (this.Axis.Area.index > 0 && this.Axis.Area.IsSync == true && this.Labels.IndexOf(label) == this.Labels.Count - 1)
                            //    {
                            //        continue;
                            //    }
                            //}
                            tickguidelines.GuidelinesX.Add(halfPenWidth);
                            tickguidelines.GuidelinesX.Add(tickSize + halfPenWidth);
                            tickguidelines.GuidelinesY.Add(y + halfPenWidth);
                            drawingContext.PushGuidelineSet(tickguidelines);
                            drawingContext.DrawLine(ticksStroke, new Point(0, y), new Point(tickSize, y));
                            drawingContext.Pop();
                        }
                        else
                        {
                            //if (this.Axis != null && this.Axis.Area != null)
                            //{
                            //    if (this.Axis.Area.index > 0 && this.Axis.Area.IsSync == true && this.Labels.IndexOf(label) == this.Labels.Count - 1)
                            //    {
                            //        continue;
                            //    }
                            //}

                            if (Axis.TickLinesPosition == AxisPositions.Cross)
                            {
                                double plusValueX = tickSize / (1 / this.Axis.TickLinesRange);
                                plusValueY = tickSize / (1 / (1-this.Axis.TickLinesRange));
                                tickguidelines.GuidelinesX.Add(this.ActualWidth + offsetLables1 + plusValueX + halfPenWidth);
                                tickguidelines.GuidelinesX.Add((offsetLables1 + (this.ActualWidth - plusValueY)) + halfPenWidth);
                                tickguidelines.GuidelinesY.Add(y + halfPenWidth);
                                drawingContext.PushGuidelineSet(tickguidelines);
                                drawingContext.DrawLine(ticksStroke, new Point(this.ActualWidth + offsetLables1 + plusValueX, y), new Point(offsetLables1 + (this.ActualWidth - plusValueY), y));
                                drawingContext.Pop();
                            }
                            else
                            {
                                tickguidelines.GuidelinesX.Add(this.ActualWidth + offsetLables1 + halfPenWidth);
                                tickguidelines.GuidelinesX.Add((offsetLables1 + (this.ActualWidth - tickSize)) + halfPenWidth);
                                tickguidelines.GuidelinesY.Add(y + halfPenWidth);
                                drawingContext.PushGuidelineSet(tickguidelines);
                                drawingContext.DrawLine(ticksStroke, new Point(this.ActualWidth + offsetLables1, y), new Point(offsetLables1 + (this.ActualWidth - tickSize), y));
                                drawingContext.Pop();
                            }
                        }
                    } 
                   
                }
                #endregion

                base.OnRender(drawingContext);
            }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"></see>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Axis != null)
            {
                if (Orientation == Orientation.Vertical)
                {
                    if ((this.Axis.AxisLabelsPosition == AxisLabels.Low || this.Axis.Origin == this.Axis.Area.PrimaryAxis.m_visibleRange.Start) && this.Axis.TickLinesPosition == AxisPositions.Outside)
                    {
                        availableSize.Width = Math.Max(Math.Max(this.Axis.TickSize, this.Axis.SmallTickSize), 0);
                        availableSize.Height = 0d;
                    }
                    else if ((this.Axis.AxisLabelsPosition != AxisLabels.NextToAxis || this.Axis.Origin == this.Axis.Area.PrimaryAxis.m_visibleRange.Start) && this.Axis.TickLinesPosition == AxisPositions.Cross)
                    {
                        availableSize.Width = Math.Max(Math.Max(this.Axis.TickSize / (1 / (1-this.Axis.TickLinesRange)), this.Axis.SmallTickSize / (1 / (1-this.Axis.SmallTickLinesRange))), 0);
                        availableSize.Height = 0d;
                    }
                    else
                    {
                        availableSize.Width = 1d;
                        availableSize.Height = 0d;
                    }

                }
                else
                {
                    if ((this.Axis.AxisLabelsPosition == AxisLabels.Low || this.Axis.Origin == this.Axis.Area.SecondaryAxis.m_visibleRange.Start) && this.Axis.TickLinesPosition == AxisPositions.Outside)
                    {
                        availableSize.Width = 0d;
                        availableSize.Height = Math.Max(Math.Max(this.Axis.TickSize, this.Axis.SmallTickSize), 0);
                    }
                    else if ((this.Axis.AxisLabelsPosition != AxisLabels.NextToAxis || this.Axis.Origin == this.Axis.Area.SecondaryAxis.m_visibleRange.Start) && this.Axis.TickLinesPosition == AxisPositions.Cross)
                    {
                        availableSize.Width = 0d;
                        availableSize.Height = Math.Max(Math.Max(this.Axis.TickSize / (1 / (1-this.Axis.TickLinesRange)), this.Axis.SmallTickSize / (1 / (1-this.Axis.SmallTickLinesRange))), 0);
                    }
                    else
                    {
                        availableSize.Width = 0d;
                        availableSize.Height = 1d;
                    }

                }
            }

            return availableSize;
        }

        /// <summary>
        /// Called when labels is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnLabelsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateMeasure();
            this.InvalidateVisual();
        }

        /// <summary>
        /// Called when labels is changed.
        /// </summary>
        /// <param name="dObj">The d obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelsChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            ChartCartesianAxisElement element = dObj as ChartCartesianAxisElement;

            if (element != null)
            {
                if (args.OldValue != null)
                {
                    (args.OldValue as ChartAxisLabelsCollection).CollectionChanged -= new NotifyCollectionChangedEventHandler(element.OnLabelsChanged);
                }

                if (args.NewValue != null)
                {
                    (args.NewValue as ChartAxisLabelsCollection).CollectionChanged += new NotifyCollectionChangedEventHandler(element.OnLabelsChanged);
                }
            }
        }

        /// <summary>
        /// Draws the axis line.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pen">The pen value.</param>
        /// <param name="point1">The starting point.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="caps">The caps value.</param>
        /// <param name="startArrowOffset">The start arrow offset.</param>
        /// <param name="endArrowOffset">The end arrow offset.</param>
        protected static void DrawAxisLine(DrawingContext context, Pen pen, Point point1, Point point2, ChartAxisCap caps, Size startArrowOffset, Size endArrowOffset)
        {
            PathFigure pathFigure = new PathFigure();

            pathFigure.StartPoint = point1;

            Vector offsetVector = new Vector(point2.X - point1.X, point2.Y - point1.Y);
            offsetVector.Normalize();
            Vector normalVector = new Vector(offsetVector.Y, offsetVector.X);

            if (!startArrowOffset.IsEmpty && (caps & ChartAxisCap.StartArrow) == ChartAxisCap.StartArrow)
            {
                Vector v1 = startArrowOffset.Width * offsetVector + startArrowOffset.Height * normalVector;
                Vector v2 = startArrowOffset.Width * offsetVector - startArrowOffset.Height * normalVector;

                pathFigure.Segments.Add(new LineSegment(point1 + v1, true));
                pathFigure.Segments.Add(new LineSegment(point1 + v2, true));
                pathFigure.Segments.Add(new LineSegment(point1, true));
            }

            pathFigure.Segments.Add(new LineSegment(point2, true));

            if (!endArrowOffset.IsEmpty && (caps & ChartAxisCap.EndArrow) == ChartAxisCap.EndArrow)
            {
                Vector v1 = endArrowOffset.Width * offsetVector + endArrowOffset.Height * normalVector;
                Vector v2 = endArrowOffset.Width * offsetVector - endArrowOffset.Height * normalVector;

                pathFigure.Segments.Add(new LineSegment(point2 - v1, true));
                pathFigure.Segments.Add(new LineSegment(point2 - v2, true));
                pathFigure.Segments.Add(new LineSegment(point2, true));
            }

            context.DrawGeometry(null, pen, new PathGeometry(new PathFigure[] { pathFigure }));
        }
        #endregion
    }
}
