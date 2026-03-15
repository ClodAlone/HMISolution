// <copyright file="ChartCartesianAreaGrid.cs" company="Syncfusion">
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
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls;

    /// <summary>
    /// Renders the chart grid.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartCartesianAreaGrid : FrameworkElement, IDisposable
    {
        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for XAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
          DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(ChartCartesianAreaGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxeschanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for YAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
          DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(ChartCartesianAreaGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxeschanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ChartCartesianAreaGrid), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Axes.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AxesProperty =
            DependencyProperty.Register("Axes", typeof(ChartAxesCollection), typeof(ChartCartesianAreaGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// DependencyProperty as the backing store for AlternatingLinesBrush.
        /// </summary>
        public static readonly DependencyProperty AlternatingLinesBrushProperty =
            DependencyProperty.Register("AlternatingLinesBrush", typeof(Brush), typeof(ChartCartesianAreaGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// DependencyProperty as the backing store for FillMode.
        /// </summary>
        public static readonly DependencyProperty FillRuleProperty =
            DependencyProperty.Register("FillRule", typeof(AlternatingFillMode), typeof(ChartCartesianAreaGrid), new FrameworkPropertyMetadata(AlternatingFillMode.Even, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// DependencyProperty as the backing store for AlternatingLinesDirection.
        /// </summary>
        public static readonly DependencyProperty AlternatingLinesDirectionProperty =
            DependencyProperty.Register("AlternatingLinesDirection", typeof(Orientation), typeof(ChartCartesianAreaGrid), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets alt. lines direction.
        /// </summary>
        public Orientation AlternatingLinesDirection
        {
            get { return (Orientation)GetValue(AlternatingLinesDirectionProperty); }
            set { SetValue(AlternatingLinesDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets fill rule for alt. grid lines.
        /// </summary>
        public AlternatingFillMode FillRule
        {
            get { return (AlternatingFillMode)GetValue(FillRuleProperty); }
            set { SetValue(FillRuleProperty, value); }
        }

        /// <summary>
        /// Gets or sets AlternatingLinesBrush.
        /// </summary>
        public Brush AlternatingLinesBrush
        {
            get { return (Brush)GetValue(AlternatingLinesBrushProperty); }
            set { SetValue(AlternatingLinesBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the axes.
        /// </summary>
        /// <value>The axes value.</value>
        public ChartAxesCollection Axes
        {
            get
            {
                return (ChartAxesCollection)GetValue(AxesProperty);
            }

            set
            {
                SetValue(AxesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the X axis.
        /// </summary>
        /// <value>The X axis.</value>
        public ChartAxis XAxis
        {
            get
            {
                return (ChartAxis)GetValue(XAxisProperty);
            }

            set
            {
                SetValue(XAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y axis.
        /// </summary>
        /// <value>The Y axis.</value>
        public ChartAxis YAxis
        {
            get
            {
                return (ChartAxis)GetValue(YAxisProperty);
            }

            set
            {
                SetValue(YAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }

            set
            {
                SetValue(BackgroundProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            ChartAxis xAxis = this.XAxis;
            ChartAxis yAxis = this.YAxis;

            if (xAxis != null && yAxis != null)
            {
                Rect clientRect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

                #region Render background
                drawingContext.DrawRectangle(this.Background, null, clientRect);
                #endregion

                #region Render alternating gridlines
                if (this.AlternatingLinesBrush != null)
                {
                    double startCoord;
                    double endCoord;
                    if (this.AlternatingLinesDirection == Orientation.Horizontal)
                    {
                        for (double i = xAxis.VisibleRange.Start + xAxis.VisibleInterval * (int)this.FillRule + xAxis.VisibleIntervalOffset; i < xAxis.VisibleRange.End + xAxis.VisibleIntervalOffset; i += 2 * xAxis.VisibleInterval)
                        {
                            startCoord = clientRect.Width * xAxis.ValueToCoefficient(i);
                            endCoord = clientRect.Width * xAxis.ValueToCoefficient(i + xAxis.VisibleInterval);
                            endCoord = endCoord <= clientRect.Width ? endCoord : clientRect.Width;
                            drawingContext.DrawRectangle(this.AlternatingLinesBrush, null, new Rect(new Point(startCoord, 0), new Point(endCoord, clientRect.Height)));
                        }
                    }
                    else
                    {
                        for (double i = yAxis.VisibleRange.Start + yAxis.VisibleInterval * (int)this.FillRule + yAxis.VisibleIntervalOffset; i < yAxis.VisibleRange.End + yAxis.VisibleIntervalOffset; i += 2 * yAxis.VisibleInterval)
                        {
                            startCoord = clientRect.Height * (1 - yAxis.ValueToCoefficient(i));
                            endCoord = clientRect.Height * (1 - yAxis.ValueToCoefficient(i + yAxis.VisibleInterval));
                            endCoord = endCoord >= 0 ? endCoord : 0;
                            drawingContext.DrawRectangle(this.AlternatingLinesBrush, null, new Rect(new Point(0, startCoord), new Point(clientRect.Width, endCoord)));
                        }
                    }
                }
                #endregion

                DrawStripLines(drawingContext, clientRect);
                DrawGridLines(drawingContext, clientRect);

                #region Render origin lines
                if (xAxis != null && ChartArea.GetShowOriginLine(xAxis))
                {
                    double origin = Math.Round(clientRect.Width *  xAxis.ValueToCoefficient(xAxis.Origin));
                    origin = Math.Round(clientRect.Height * (1 - yAxis.ValueToCoefficient(xAxis.Origin)));
                    xAxis.XLabelOffset = Math.Round(clientRect.Height * (yAxis.ValueToCoefficient(xAxis.Origin)));
                    xAxis.gridheight = this.ActualHeight;
                    drawingContext.DrawLine(ChartArea.GetOriginLineStroke(xAxis), new Point(clientRect.Left, origin), new Point(clientRect.Right, origin));
                }

                if (yAxis != null && ChartArea.GetShowOriginLine(yAxis))
                {
                    double origin = Math.Round(clientRect.Height * (1 - yAxis.ValueToCoefficient(yAxis.Origin)));
                    origin = Math.Round(clientRect.Width * xAxis.ValueToCoefficient(yAxis.Origin));
                    yAxis.YLabelOffset = origin;
                    yAxis.gridWidth = this.ActualWidth;
                    drawingContext.DrawLine(ChartArea.GetOriginLineStroke(yAxis), new Point(origin, clientRect.Top), new Point(origin, clientRect.Bottom));
                }
                #endregion
            }

            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Draws the strip lines.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <param name="clientRect">The client rect.</param>
        private void DrawStripLines(DrawingContext drawingContext, Rect clientRect)
        {
            if (this.Axes != null)
            {
                if (this.Axes.Count > 0)
                {
                    foreach (ChartAxis axis in this.Axes)
                    {
                        if (axis != null)
                        {
                            if (axis.Orientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                #region Render primary strip lines
                                if (axis != null && axis.StripLines != null)
                                {
                                    DoubleRange visibleRange = axis.VisibleRange;
                                    DoubleRange axisRange = axis.ActualRange;

                                    foreach (ChartStripLine stripLine in axis.StripLines)
                                    {
                                        #region Default StripLine
                                        if (stripLine.IsSegmented == false && stripLine.Visibility==Visibility.Visible)
                                        {
                                            //SD13451 Stripline background is drawn beyond the chart plotting area while zooming.
                                            double startStrip = stripLine.StartFromAxis ? axisRange.Start + stripLine.Offset : stripLine.Start;
                                            double endStrip = stripLine.StartFromAxis ? (stripLine.RepeatUntil == 0 ? axisRange.End : stripLine.RepeatUntil) : stripLine.RepeatUntil;
                                            double periodStrip = stripLine.RepeatEvery;

                                            do
                                            {
                                                double x1 = 0d;
                                                double x2 = 0d;
                                                double stripStart = startStrip;
                                                double stripEnd = stripStart + stripLine.Width;
                                                //SD13451 Stripline disappears while sector zooming.
                                                stripEnd = stripEnd > visibleRange.End ? visibleRange.End : stripEnd;
                                                stripStart = stripStart < visibleRange.Start ? visibleRange.Start : stripStart;
                                                if(!(stripEnd<visibleRange.Start) && !(stripStart>visibleRange.End))
                                                {
                                                    x1 = clientRect.Width * axis.ValueToCoefficient(stripStart);
                                                    
                                                    if (stripLine.IsPixelWidth == true)
                                                    {
                                                        x2 = x1 + stripLine.Width;
                                                    }
                                                    else
                                                    {
                                                        x2 = clientRect.Width * axis.ValueToCoefficient(stripEnd);
                                                    }
                                                    // clientRect.Width * axis.ValueToCoefficient(startStrip + stripLine.Width);
                                                    Rect stripRect = new Rect(new Point(x1, 0), new Point(x2, clientRect.Height));

                                                    drawingContext.DrawRectangle(stripLine.Interior, stripLine.Stroke, stripRect);

                                                    if (stripLine.Text != null)
                                                    {
                                                        Size textSize = new Size(stripLine.Text.Width, stripLine.Text.Height);
                                                        Point textLoc = ChartLayoutUtils.GetStartPointBy(textSize, stripRect, ChartAlignment.Center, stripLine.TextAlignment);

                                                        RotateTransform rt = new RotateTransform();
                                                        if (stripLine.VerticalText)
                                                        {
                                                            rt.Angle = -90;
                                                        }
                                                        else
                                                        {
                                                            rt.Angle = 0;
                                                        }

                                                        rt.Angle = stripLine.TextRotationAngle;
                                                        TranslateTransform tr = new TranslateTransform(stripLine.TextOffsetX, stripLine.TextOffsetY);

                                                        TransformGroup group = new TransformGroup();
                                                        group.Children.Add(rt);
                                                        group.Children.Add(tr);

                                                        rt.CenterX = textLoc.X + textSize.Width / 2;
                                                        rt.CenterY = textLoc.Y + textSize.Height / 2;
                                                        drawingContext.PushTransform(group);

                                                        if (stripLine.TextBackground != Brushes.Transparent)
                                                        {
                                                            if (stripLine.Text.FlowDirection == FlowDirection.RightToLeft)
                                                            {
                                                                drawingContext.DrawRectangle(stripLine.TextBackground, null, new Rect(new Point(textLoc.X - textSize.Width, textLoc.Y), textSize));
                                                            }
                                                            else
                                                            {
                                                                drawingContext.DrawRectangle(stripLine.TextBackground, null, new Rect(textLoc, textSize));
                                                            }
                                                        }
                                                        //SD13451 Stripline labels are drawn behind the gridlines.
                                                        if (textSize.Height <= stripRect.Width) 
                                                        {
                                                            drawingContext.DrawText(stripLine.Text, textLoc);
                                                        }
                                                        else if (stripLine.IsPixelWidth == true)
                                                        {
                                                            drawingContext.DrawText(stripLine.Text, textLoc);
                                                        }
                                                            drawingContext.Pop();
                                                        
                                                    }
                                                }

                                                startStrip += periodStrip;
                                            }
                                            while ((periodStrip != 0) && (startStrip < endStrip));
                                        }
                                        #endregion
                                        #region Segmented Stripline
                                        else if(stripLine.Visibility==Visibility.Visible)
                                        {
                                            double startStrip = stripLine.StartFromAxis ? visibleRange.Start + stripLine.Offset : stripLine.Start;
                                            double endStrip = stripLine.StartFromAxis ? (stripLine.RepeatUntil == 0 ? visibleRange.End : stripLine.RepeatUntil) : stripLine.RepeatUntil;
                                            double periodStrip = stripLine.RepeatEvery;

                                            do
                                            {
                                                if (visibleRange.Inside(startStrip) || visibleRange.Inside(startStrip + stripLine.Width))
                                                {

                                                    double x1 = clientRect.Width * axis.ValueToCoefficient(startStrip);
                                                    double x2 = 0d;
                                                    if (stripLine.IsPixelWidth == true)
                                                    {
                                                        x2 = stripLine.Width < clientRect.Width ? x1+ stripLine.Width :x1+clientRect.Width;
                                                    }
                                                    else
                                                    {
                                                        double stripEnd = startStrip + stripLine.Width;
                                                        stripEnd = (stripEnd > visibleRange.End) ? visibleRange.End : stripEnd;
                                                        x2 = clientRect.Width * (axis.ValueToCoefficient(stripEnd));
                                                    }

                                                    if (axis.Area != null && axis.Area.SecondaryAxis != null)
                                                    {

                                                        ChartAxis secondaryAxis = axis.Area.SecondaryAxis;
                                                        double startVal = clientRect.Height * (1 - secondaryAxis.ValueToCoefficient(stripLine.SegmentStartValue));
                                                        double endVal = clientRect.Height * (1 - secondaryAxis.ValueToCoefficient(stripLine.SegmentEndValue));


                                                        Rect stripRect = new Rect(new Point(x1, startVal), new Point(x2, endVal));

                                                        drawingContext.DrawRectangle(stripLine.Interior, stripLine.Stroke, stripRect);

                                                        if (stripLine.Text != null)
                                                        {
                                                            Size textSize = new Size(stripLine.Text.Width, stripLine.Text.Height);
                                                            Point textLoc = ChartLayoutUtils.GetStartPointBy(textSize, stripRect, ChartAlignment.Center, stripLine.TextAlignment);

                                                            RotateTransform rt = new RotateTransform();
                                                            if (stripLine.VerticalText)
                                                            {
                                                                rt.Angle = -90;
                                                            }
                                                            else
                                                            {
                                                                rt.Angle = 0;
                                                            }
                                                            rt.Angle = stripLine.TextRotationAngle;
                                                            TranslateTransform tr = new TranslateTransform(stripLine.TextOffsetX, stripLine.TextOffsetY);

                                                            TransformGroup group = new TransformGroup();
                                                            group.Children.Add(rt);
                                                            group.Children.Add(tr);

                                                            rt.CenterX = textLoc.X + textSize.Width / 2;
                                                            rt.CenterY = textLoc.Y + textSize.Height / 2;
                                                            drawingContext.PushTransform(group);

                                                            //if (stripLine.TextBackground != Brushes.Transparent)
                                                            //{
                                                            if (stripLine.Text.FlowDirection == FlowDirection.RightToLeft)
                                                            {
                                                                drawingContext.DrawRectangle(stripLine.TextBackground, null, new Rect(new Point(textLoc.X - textSize.Width, textLoc.Y), textSize));
                                                            }
                                                            else
                                                            {
                                                                drawingContext.DrawRectangle(stripLine.TextBackground, null, new Rect(textLoc, textSize));
                                                            }
                                                            //}

                                                            drawingContext.DrawText(stripLine.Text, textLoc);
                                                            drawingContext.Pop();
                                                        }
                                                    }
                                                }
                                                startStrip += periodStrip;
                                            }
                                            while ((periodStrip != 0) && (startStrip < endStrip));
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region Render secondary strip lines
                                if (axis != null && axis.StripLines != null)
                                {
                                    DoubleRange visibleRange = axis.VisibleRange;

                                    foreach (ChartStripLine stripLine in axis.StripLines)
                                    {
                                        #region Default Strip line
                                        if (stripLine.IsSegmented == false && stripLine.Visibility == Visibility.Visible)
                                        {
                                            double startStrip = stripLine.StartFromAxis ? visibleRange.Start + stripLine.Offset : stripLine.Start;
                                            double endStrip = stripLine.StartFromAxis ? (stripLine.RepeatUntil == 0 ? visibleRange.End : stripLine.RepeatUntil) : stripLine.RepeatUntil;
                                            double periodStrip = stripLine.RepeatEvery;

                                            do
                                            {
                                                if (visibleRange.Inside(startStrip) || visibleRange.Inside(startStrip + stripLine.Width))
                                                {
                                                    double y1 = clientRect.Height * (1 - axis.ValueToCoefficient(startStrip));
                                                    double y2 = 0d;
                                                    if (stripLine.IsPixelWidth == true)
                                                    {
                                                        y2 = stripLine.Width < clientRect.Height ? y1 - stripLine.Width : y1 - clientRect.Height;
                                                    }
                                                    else
                                                    {
                                                        double stripEnd = startStrip + stripLine.Width;                                                        
                                                        stripEnd = (stripEnd > visibleRange.End) ? visibleRange.End : stripEnd;
                                                        y2 = clientRect.Height * (1 - axis.ValueToCoefficient(stripEnd));
                                                    }

                                                    // <= visibleRange.End - visibleRange.Start ?
                                                    //clientRect.Height * (1 - axis.ValueToCoefficient(startStrip + stripLine.Width)) :
                                                    //clientRect.Height * (1 - axis.ValueToCoefficient(startStrip + visibleRange.End - visibleRange.Start));

                                                    Rect stripRect = new Rect(new Point(0, y1), new Point(clientRect.Width, y2));

                                                    drawingContext.DrawRectangle(stripLine.Interior, stripLine.Stroke, stripRect);

                                                    if (stripLine.Text != null)
                                                    {
                                                        Size textSize = new Size(stripLine.Text.Width, stripLine.Text.Height);
                                                        Point textLoc = ChartLayoutUtils.GetStartPointBy(textSize, stripRect, ChartAlignment.Center, stripLine.TextAlignment);

                                                        RotateTransform rt = new RotateTransform();
                                                        if (stripLine.VerticalText)
                                                        {
                                                            rt.Angle = -90;
                                                        }
                                                        else
                                                        {
                                                            rt.Angle = 0;
                                                        }
                                                        rt.Angle = stripLine.TextRotationAngle;
                                                        TranslateTransform tr = new TranslateTransform(stripLine.TextOffsetX, stripLine.TextOffsetY);

                                                        TransformGroup group = new TransformGroup();
                                                        group.Children.Add(rt);
                                                        group.Children.Add(tr);

                                                        rt.CenterX = textLoc.X + textSize.Width / 2;
                                                        rt.CenterY = textLoc.Y + textSize.Height / 2;
                                                        drawingContext.PushTransform(group);

                                                        if (stripLine.TextBackground != Brushes.Transparent)
                                                        {
                                                            if (stripLine.Text.FlowDirection == FlowDirection.RightToLeft)
                                                            {
                                                                drawingContext.DrawRectangle(stripLine.TextBackground, null, new Rect(new Point(textLoc.X - textSize.Width, textLoc.Y), textSize));
                                                            }
                                                            else
                                                            {
                                                                drawingContext.DrawRectangle(stripLine.TextBackground, null, new Rect(textLoc, textSize));
                                                            }
                                                        }

                                                        drawingContext.DrawText(stripLine.Text, textLoc);
                                                        drawingContext.Pop();
                                                    }
                                                }

                                                startStrip += periodStrip;
                                            }
                                            while ((periodStrip != 0) && (startStrip < endStrip));
                                        }
                                        #endregion
                                        #region Segmented Strip line
                                        else if(stripLine.Visibility==Visibility.Visible)
                                        {
                                            double startStrip = stripLine.StartFromAxis ? visibleRange.Start + stripLine.Offset : stripLine.Start;
                                            double endStrip = stripLine.StartFromAxis ? (stripLine.RepeatUntil == 0 ? visibleRange.End : stripLine.RepeatUntil) : stripLine.RepeatUntil;
                                            double periodStrip = stripLine.RepeatEvery;

                                            do
                                            {
                                                if (visibleRange.Inside(startStrip) || visibleRange.Inside(startStrip + stripLine.Width))
                                                {
                                                    double y1 = clientRect.Height * (1 - axis.ValueToCoefficient(startStrip));
                                                    double y2 = 0d;
                                                    if (stripLine.IsPixelWidth == true)
                                                    {
                                                        y2 = stripLine.Width < clientRect.Height ? y1 - stripLine.Width : y1 - clientRect.Height;
                                                    }
                                                    else
                                                    {
                                                        double stripEnd = startStrip + stripLine.Width;
                                                        stripEnd = (stripEnd > visibleRange.End) ? visibleRange.End : stripEnd;
                                                        y2 = clientRect.Height * (1 - axis.ValueToCoefficient(stripEnd));
                                                    }
                                                    if (axis.Area != null && axis.Area.PrimaryAxis != null)
                                                    {
                                                        ChartAxis primaryAxis = axis.Area.PrimaryAxis;
                                                        double startVal = clientRect.Width * (primaryAxis.ValueToCoefficient(stripLine.SegmentStartValue));
                                                        double endVal = clientRect.Width * (primaryAxis.ValueToCoefficient(stripLine.SegmentEndValue));

                                                        Rect stripRect = new Rect(new Point(startVal, y1), new Point(endVal, y2));

                                                        drawingContext.DrawRectangle(stripLine.Interior, stripLine.Stroke, stripRect);

                                                        if (stripLine.Text != null)
                                                        {
                                                            Size textSize = new Size(stripLine.Text.Width, stripLine.Text.Height);
                                                            Point textLoc = ChartLayoutUtils.GetStartPointBy(textSize, stripRect, ChartAlignment.Center, stripLine.TextAlignment);

                                                            RotateTransform rt = new RotateTransform();
                                                            if (stripLine.VerticalText)
                                                            {
                                                                rt.Angle = -90;
                                                            }
                                                            else
                                                            {
                                                                rt.Angle = 0;
                                                            }
                                                            rt.Angle = stripLine.TextRotationAngle;
                                                            TranslateTransform tr = new TranslateTransform(stripLine.TextOffsetX, stripLine.TextOffsetY);
                                                            TransformGroup group = new TransformGroup();
                                                            group.Children.Add(rt);
                                                            group.Children.Add(tr);

                                                            rt.CenterX = textLoc.X + textSize.Width / 2;
                                                            rt.CenterY = textLoc.Y + textSize.Height / 2;
                                                            drawingContext.PushTransform(group);

                                                            if (stripLine.Text.FlowDirection == FlowDirection.RightToLeft)
                                                            {
                                                                drawingContext.DrawRectangle(stripLine.TextBackground, null, new Rect(new Point(textLoc.X - textSize.Width, textLoc.Y), textSize));
                                                            }
                                                            else
                                                            {
                                                                drawingContext.DrawRectangle(stripLine.TextBackground, null, new Rect(textLoc, textSize));
                                                            }


                                                            drawingContext.DrawText(stripLine.Text, textLoc);
                                                            drawingContext.Pop();
                                                        }
                                                    }
                                                }

                                                startStrip += periodStrip;
                                            }
                                            while ((periodStrip != 0) && (startStrip < endStrip));
                                        }

                                        #endregion
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Draws the grid lines.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <param name="clientRect">The client rect.</param>
        private void DrawGridLines(DrawingContext drawingContext, Rect clientRect)
        {
            this.ClipToBounds = false;
            GuidelineSet guidelines;
            if (this.Axes != null)
            {
                if (this.Axes.Count > 0)
                {
                    foreach (ChartAxis axis in this.Axes)
                    {
                        if (axis != null)
                        {
                            if (ChartArea.GetShowGridLines(axis))
                            {
                                bool ticksAllowed = (axis.Area.PrimarySeries == null) ? true : axis.Area.PrimarySeries.Type != ChartTypes.PointAndFigure;
                                if (axis.Orientation == System.Windows.Controls.Orientation.Horizontal)
                                {
                                    ChartAxis xAxis = axis;
                                    if (axis.Area.StyleGridLine == false)
                                    {
                                        #region Render primary ticks grid lines
                                        if (xAxis != null && ChartArea.GetShowGridLines(xAxis) && ticksAllowed && xAxis.SmallTicksPerInterval!=0)
                                        {
                                            Pen gridLinePen = ChartArea.GetSmallGridLineStroke(xAxis);
                                            //gridLinePen.Brush.Opacity = 0.2d;
                                            double halfPenWidth = gridLinePen.Thickness / 2;
                                            // Create a guidelines set
                                            guidelines = new GuidelineSet();
                                            for (int i = 0, ci = xAxis.TicksPoint.Count; i < ci; i++)
                                            {                                                
                                                double x = clientRect.Width * xAxis.ValueToCoefficient((double)xAxis.TicksPoint[i]);
                                                guidelines.GuidelinesX.Add(x + halfPenWidth);
                                                guidelines.GuidelinesY.Add(clientRect.Height + halfPenWidth);
                                                guidelines.GuidelinesY.Add(clientRect.Height + halfPenWidth);
                                                drawingContext.PushGuidelineSet(guidelines);
                                                //SD13451 Gridline are drawn beyond the chart plotting area while zooming.
                                                if (x > 0)
                                                {
                                                    drawingContext.DrawLine(gridLinePen, new Point(x, 0), new Point(x, clientRect.Height));
                                                }
                                                drawingContext.Pop();
                                            }
                                        }
                                        #endregion

                                        #region Render primary grid lines
                                        if (xAxis != null)
                                        {
                                            Pen gridLinePen = ChartArea.GetSmallGridLineStroke(xAxis);
                                            double halfPenWidth = gridLinePen.Thickness / 2;
                                            if (ChartArea.GetShowMajorGridLines(xAxis))
                                            {
                                                axis.SegmentPosition = axis.Indexed?(axis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes ? SegmentPositions.BetweenTicks : SegmentPositions.OnTicks):SegmentPositions.OnTicks;
                                                //feature for between segment and on segment
                                                bool isDefault = xAxis.Indexed ? (xAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes ? (!xAxis.m_showAllLabelsChanged ? true : false) : false) : false;
                                                #region BetweenTicks
                                                if (axis.SegmentPosition == SegmentPositions.BetweenTicks)
                                                {
                                                    gridLinePen = ChartArea.GetGridLineStroke(xAxis);
                                                    halfPenWidth = gridLinePen.Thickness / 2;
                                                    // Create a guidelines set
                                                    guidelines = new GuidelineSet();
                                                    double interval=0d,x;
                                                    int pos=0,point=0;
                                                    for (int i = 0, ci = xAxis.ShowAllLabels?xAxis.m_ticksCount : (isDefault?xAxis.m_ticksCount:xAxis.VisibleLabels.Count); i < ci; i++)
                                                    {
                                                        if(xAxis.ShowAllLabels || isDefault)
                                                            point = i == 0 ? (double.IsNaN(xAxis.BaseInterval) ? ((xAxis.VisibleLabels.Count / xAxis.m_ticksCount)!=0?(xAxis.VisibleLabels.Count / xAxis.m_ticksCount):1) : ((xAxis.ShowAllLabels || isDefault) ? (i + (int)xAxis.BaseInterval) : i)) : point;
                                                        pos=pos+point;
                                                        if ((i == 0 && ci > 1) && xAxis.VisibleLabels.Count >1)
                                                            interval = clientRect.Width * xAxis.ValueToCoefficient(xAxis.VisibleLabels[i + 1].Position) - clientRect.Width * xAxis.ValueToCoefficient(xAxis.VisibleLabels[i].Position);
                                                        if (xAxis.VisibleLabels.Count > pos)
                                                        {
                                                            x = clientRect.Width * xAxis.ValueToCoefficient(xAxis.VisibleLabels[xAxis.ShowAllLabels?pos : (isDefault?pos:i)].Position);
                                                            guidelines.GuidelinesX.Add(x - (interval * (1 / (xAxis.VisibleInterval * 2))) + halfPenWidth);
                                                            guidelines.GuidelinesY.Add(clientRect.Height + halfPenWidth);
                                                            guidelines.GuidelinesY.Add(clientRect.Height + halfPenWidth);
                                                            drawingContext.PushGuidelineSet(guidelines);
                                                            //SD13451 Gridline are drawn beyond the chart plotting area while zooming.
                                                            if (x > 0)
                                                            {
                                                                drawingContext.DrawLine(gridLinePen, new Point(x - (interval * (1 / (xAxis.VisibleInterval * 2))), 0), new Point(x - (interval * (1 / (xAxis.VisibleInterval * 2))), clientRect.Height));
                                                            }
                                                            drawingContext.Pop();
                                                        }
                                                    }
                                                }
                                                #endregion
                                                else
                                                #region OnTicks
                                                {
                                                    gridLinePen = ChartArea.GetGridLineStroke(xAxis);
                                                    halfPenWidth = gridLinePen.Thickness / 2;
                                                    // Create a guidelines set
                                                    guidelines = new GuidelineSet();
                                                    int pos = 0,point=0;
                                                    double x;
                                                    for (int i = 0, ci = xAxis.ShowAllLabels?xAxis.m_ticksCount:xAxis.VisibleLabels.Count; i < ci; i++)
                                                    {
                                                        if(xAxis.ShowAllLabels)
                                                        point = i == 0 ? (double.IsNaN(xAxis.BaseInterval) ? (xAxis.VisibleLabels.Count / xAxis.m_ticksCount) : (xAxis.ShowAllLabels ? (i + (int)xAxis.BaseInterval) : i)) : point;

                                                        pos = pos + point;
                                                        if (xAxis.VisibleLabels.Count > pos)
                                                        {
                                                            x = clientRect.Width * xAxis.ValueToCoefficient(xAxis.VisibleLabels[xAxis.ShowAllLabels?pos:i].Position);
                                                            guidelines.GuidelinesX.Add(x + halfPenWidth);
                                                            guidelines.GuidelinesY.Add(clientRect.Height + halfPenWidth);
                                                            guidelines.GuidelinesY.Add(clientRect.Height + halfPenWidth);
                                                            drawingContext.PushGuidelineSet(guidelines);
                                                            //SD13451 Gridline are drawn beyond the chart plotting area while zooming.
                                                            if (x > 0)
                                                            {
                                                                drawingContext.DrawLine(gridLinePen, new Point(x, 0), new Point(x, clientRect.Height));
                                                            }
                                                            drawingContext.Pop();
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                if (xAxis.TicksPoint.Count <= xAxis.VisibleLabels.Count)
                                                {
                                                    gridLinePen = null;
                                                }
                                            }
                                            
                                            gridLinePen = ChartArea.GetGridLineStroke(xAxis);
                                            guidelines = new GuidelineSet();
                                            guidelines.GuidelinesX.Add(clientRect.Left + halfPenWidth);
                                            guidelines.GuidelinesX.Add(clientRect.Right + halfPenWidth);
                                            guidelines.GuidelinesY.Add(clientRect.Top + halfPenWidth);
                                            guidelines.GuidelinesY.Add(clientRect.Bottom + halfPenWidth);
                                            drawingContext.PushGuidelineSet(guidelines);
                                            drawingContext.DrawLine(gridLinePen, clientRect.TopLeft, clientRect.BottomLeft);
                                            drawingContext.DrawLine(gridLinePen, clientRect.TopRight, clientRect.BottomRight);
                                            drawingContext.Pop();
                                        }

                                        #endregion
                                    }
                                }
                                else
                                {
                                    ChartAxis yAxis = axis;
                                    #region Render secondary ticks grid lines
                                    if (yAxis != null && ChartArea.GetShowGridLines(yAxis) && ticksAllowed && yAxis.SmallTicksPerInterval != 0)
                                    {
                                        Pen gridLinePen = ChartArea.GetSmallGridLineStroke(yAxis);
                                     //  gridLinePen.Brush.Opacity = 0.2d;
                                        GuidelineSet guidelines2 = new GuidelineSet();
                                        double halfPenWidth = gridLinePen.Thickness / 2;

                                        for (int i = 0, ci = yAxis.TicksPoint.Count; i < ci; i++)
                                        {                           
                                            double y = clientRect.Height * (1 - yAxis.ValueToCoefficient((double)yAxis.TicksPoint[i]));
                                            guidelines2.GuidelinesX.Add(0+halfPenWidth);
                                            guidelines2.GuidelinesX.Add(clientRect.Width + halfPenWidth);
                                            guidelines2.GuidelinesY.Add(y + halfPenWidth);
                                            drawingContext.PushGuidelineSet(guidelines2);
                                            drawingContext.DrawLine(gridLinePen, new Point(0, y), new Point(clientRect.Width, y));
                                            drawingContext.Pop();
                                        }
                                    }
                                    #endregion

                                    #region Render secondary grid lines
                                    if (yAxis != null && ChartArea.GetShowGridLines(yAxis))
                                    {
                                        Pen gridLinePen = ChartArea.GetSmallGridLineStroke(yAxis);
                                        double halfPenWidth = gridLinePen.Thickness / 2;
                                        if (ChartArea.GetShowMajorGridLines(yAxis))
                                        {
                                            axis.SegmentPosition = axis.Indexed?(axis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes ? SegmentPositions.BetweenTicks : SegmentPositions.OnTicks):SegmentPositions.OnTicks;
                                            gridLinePen = ChartArea.GetGridLineStroke(yAxis);
                                            GuidelineSet guidelines2 = new GuidelineSet();
                                            halfPenWidth = gridLinePen.Thickness/2;
                                            #region BetweenTicks
                                            bool isDefault = yAxis.Indexed ? (yAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes ? (!yAxis.m_showAllLabelsChanged ? true : false) : false) : false;
                                            if (axis.SegmentPosition == SegmentPositions.BetweenTicks)
                                            {
                                                double interval = 0d;
                                                int pos = 0, point = 0;
                                                for (int i = 0, ci = yAxis.ShowAllLabels ? yAxis.m_ticksCount : isDefault?yAxis.m_ticksCount: yAxis.VisibleLabels.Count; i < ci; i++)
                                                {
                                                    point = i == 0 ? (double.IsNaN(yAxis.BaseInterval) ? (yAxis.VisibleLabels.Count / yAxis.m_ticksCount) : (yAxis.ShowAllLabels ? (i + (int)yAxis.BaseInterval - 1) :(isDefault?(i + (int)yAxis.Interval - 1):i))) : (i == 1 ? (double.IsNaN(yAxis.BaseInterval) ? point : point + 1) : point);
                                                    pos = pos + point;
                                                    if (i == 0 && ci > 1 && yAxis.VisibleLabels.Count >i+1)
                                                    interval = (clientRect.Height * (1 - yAxis.ValueToCoefficient(yAxis.VisibleLabels[i + 1].Position)) - clientRect.Height * (1 - yAxis.ValueToCoefficient(yAxis.VisibleLabels[i].Position)));
                                                    if (yAxis.VisibleLabels.Count > pos)
                                                    {
                                                        double y = clientRect.Height * (1 - yAxis.ValueToCoefficient(yAxis.VisibleLabels[yAxis.ShowAllLabels ? pos : isDefault?pos: i].Position));
                                                        guidelines2.GuidelinesX.Add(0 + halfPenWidth);
                                                        guidelines2.GuidelinesX.Add(clientRect.Width + halfPenWidth);
                                                        guidelines2.GuidelinesY.Add(y-(interval * (1 / (yAxis.VisibleInterval * 2))) + halfPenWidth);
                                                        drawingContext.PushGuidelineSet(guidelines2);
                                                        drawingContext.DrawLine(gridLinePen, new Point(0, y-(interval * (1 / (yAxis.VisibleInterval * 2)))), new Point(clientRect.Width, y-(interval * (1 / (yAxis.VisibleInterval * 2)))));
                                                        drawingContext.Pop();
                                                    }
                                                }
                                            }
                                            #endregion
                                            #region OnTicks
                                            else
                                            {
                                                for (int i = 0, ci = yAxis.VisibleLabels.Count; i < ci; i++)
                                                {
                                                    double y = clientRect.Height * (1 - yAxis.ValueToCoefficient(yAxis.VisibleLabels[i].Position));
                                                    guidelines2.GuidelinesX.Add(0 + halfPenWidth);
                                                    guidelines2.GuidelinesX.Add(clientRect.Width + halfPenWidth);
                                                    guidelines2.GuidelinesY.Add(y + halfPenWidth);
                                                    drawingContext.PushGuidelineSet(guidelines2);

                                                    drawingContext.DrawLine(gridLinePen, new Point(0, y), new Point(clientRect.Width, y));
                                                    drawingContext.Pop();
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            if (yAxis.TicksPoint.Count <= yAxis.VisibleLabels.Count)
                                            {
                                                gridLinePen = null;
                                            }
                                        }

                                        gridLinePen = ChartArea.GetGridLineStroke(yAxis);
                                        guidelines = new GuidelineSet();
                                        guidelines.GuidelinesX.Add(clientRect.Left + halfPenWidth);
                                        guidelines.GuidelinesX.Add(clientRect.Right + halfPenWidth);
                                        guidelines.GuidelinesY.Add(clientRect.Top + halfPenWidth);
                                        guidelines.GuidelinesY.Add(clientRect.Bottom + halfPenWidth);
                                        drawingContext.PushGuidelineSet(guidelines);
                                        drawingContext.DrawLine(gridLinePen, clientRect.TopLeft, clientRect.TopRight);
                                        drawingContext.DrawLine(gridLinePen, clientRect.BottomLeft, clientRect.BottomRight);
                                        drawingContext.Pop();
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when axes is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAxeschanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        /// <summary>
        /// Called when axes is changed.
        /// </summary>
        /// <param name="dObj">The d obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAxeschanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            ChartCartesianAreaGrid grid = dObj as ChartCartesianAreaGrid;

            if (grid != null)
            {
                if (args.OldValue != null)
                {
                    (args.OldValue as ChartAxis).Changed -= new EventHandler(grid.OnAxeschanged);
                }

                if (args.NewValue != null)
                {
                    (args.NewValue as ChartAxis).Changed += new EventHandler(grid.OnAxeschanged);
                }
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (this.Axes != null)
            {
                this.Axes.Clear();
                this.Axes = null;
            }
        }

        #endregion
    }
}
