// <copyright file="ChartCartesianAxisLabelsPanel.cs" company="Syncfusion">
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
    using System.Windows.Controls;
    using System.Media;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Globalization;

  
    /// <summary>
    /// Represents layout panel for <see cref="ChartAxisLabel">chart axis labels</see>.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartCartesianAxisLabelsPanel : Panel, IDisposable
    {
        // Fixes for SD14648-On resizing the chart "ChartArea.AxesThickness" is changing  frequently and ID2196-chartarea cropped automation break
        private List<double> fitpaddingrightCollection;
        private List<double> fitpaddingleftCollection;
        #region Constants
        /// <summary>
        /// Initializes C_rowsCount
        /// </summary>
        private const int C_rowsCount = 4;

        /// <summary>
        /// Initializes C_resizeAffect
        /// </summary>
        private const FrameworkPropertyMetadataOptions C_resizeAffect = FrameworkPropertyMetadataOptions.AffectsArrange
          | FrameworkPropertyMetadataOptions.AffectsMeasure
          | FrameworkPropertyMetadataOptions.AffectsParentArrange
          | FrameworkPropertyMetadataOptions.AffectsParentMeasure;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the Axis dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register("Axis", typeof(ChartAxis), typeof(ChartCartesianAxisLabelsPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAxisChanged)));

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
          DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartCartesianAxisLabelsPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, C_resizeAffect));

        /// <summary>
        /// Identifies the LabelRotateAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelRotateAngleProperty =
          DependencyProperty.Register("LabelRotateAngle", typeof(double), typeof(ChartCartesianAxisLabelsPanel), new FrameworkPropertyMetadata(0d, C_resizeAffect));

        /// <summary>
        /// Identifies the OpposedPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty OpposedPositionProperty =
          DependencyProperty.Register("OpposedPosition", typeof(bool), typeof(ChartCartesianAxisLabelsPanel), new FrameworkPropertyMetadata(false, C_resizeAffect));

        /// <summary>
        /// Identifies the Label alignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelAlignmentProperty =
          DependencyProperty.Register("LabelAlignment", typeof(ChartAlignment), typeof(ChartCartesianAxisLabelsPanel), new FrameworkPropertyMetadata(ChartAlignment.Center, C_resizeAffect));

        /// <summary>
        /// Identifies the Position dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
          DependencyProperty.RegisterAttached("Position", typeof(double), typeof(ChartCartesianAxisLabelsPanel), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the IntersectAction dependency property.
        /// </summary>
        public static readonly DependencyProperty IntersectActionProperty =
          DependencyProperty.Register("IntersectAction", typeof(ChartLabelIntersectAction), typeof(ChartCartesianAxisLabelsPanel), new FrameworkPropertyMetadata(ChartLabelIntersectAction.None, C_resizeAffect));

        /// <summary>
        /// Identifies the HidePartialLabel dependency property.
        /// </summary>
        public static readonly DependencyProperty HidePartialLabelProperty =
            DependencyProperty.Register("HidePartialLabel", typeof(bool), typeof(ChartCartesianAxisLabelsPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Identifies the EdgeLabelsDrawingMode dependency property.
        /// </summary>
        public static readonly DependencyProperty EdgeLabelsDrawingModeProperty =
            DependencyProperty.Register("EdgeLabelsDrawingMode", typeof(EdgeLabelsDrawingMode), typeof(ChartCartesianAxisLabelsPanel), new FrameworkPropertyMetadata(EdgeLabelsDrawingMode.Center, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Axis property is changed.
        /// </summary>
        public event PropertyChangedCallback AxisChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating mode that controls partially visible labels behaviour.
        /// </summary>
        /// <value><c>true</c> if partial labels should be hidden; otherwise, <c>false</c>.</value>
        public EdgeLabelsDrawingMode EdgeLabelsDrawingMode
        {
            get { return (EdgeLabelsDrawingMode)GetValue(EdgeLabelsDrawingModeProperty); }
            set { SetValue(EdgeLabelsDrawingModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether partial labels should be hidden.
        /// </summary>
        /// <value><c>true</c> if partial labels are hidden; otherwise, <c>false</c>.</value>
        public bool HidePartialLabel
        {
            get { return (bool)GetValue(HidePartialLabelProperty); }
            set { SetValue(HidePartialLabelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value of the Axis. This is a dependency property.
        /// </summary>
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
        /// Gets or sets the labels rotate angle. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Property represents angle that labels on axis should be rotated by.
        /// </remarks>
        /// <value>The label rotate angle in degrees.</value>
        /// <seealso cref="ChartAxisLabel"/>
        /// <seealso cref="ChartAxis"/>
        public double LabelRotateAngle
        {
            get
            {
                return (double)GetValue(LabelRotateAngleProperty);
            }

            set
            {
                SetValue(LabelRotateAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label alignment. This is a dependency property.
        /// </summary>
        /// <value>The label alignment.</value>
        public ChartAlignment LabelAlignment
        {
            get
            {
                return (ChartAlignment)GetValue(LabelAlignmentProperty);
            }

            set
            {
                SetValue(LabelAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation. This is a dependency property.
        /// </summary>
        /// <value>The labels panel orientation.</value>
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
        /// Gets or sets a value indicating whether axis labels panel should be positioned opposed to to regular axis position.
        /// This is a dependency property.
        /// </summary>
        /// <value><c>true</c> if panel should be opposed positioned; otherwise, <c>false</c>.</value>
        public bool OpposedPosition
        {
            get
            {
                return (bool)GetValue(OpposedPositionProperty);
            }

            set
            {
                SetValue(OpposedPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets type of action that should be taken to prevent labels intersection. This is a dependency property.
        /// </summary>
        /// <value>The intersect action.</value>
        /// <seealso cref="ChartLabelIntersectAction"/>
        public ChartLabelIntersectAction IntersectAction
        {
            get { return (ChartLabelIntersectAction)GetValue(IntersectActionProperty); }
            set { SetValue(IntersectActionProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCartesianAxisLabelsPanel"/> class.
        /// </summary>
        public ChartCartesianAxisLabelsPanel()
        {
            base.ClipToBounds = false;
            // Fixes for SD14648-On resizing the chart "ChartArea.AxesThickness" is changing  frequently and ID2196-chartarea cropped automation break
            fitpaddingrightCollection = new List<double>();
            fitpaddingrightCollection.Add(0);
            fitpaddingleftCollection = new List<double>();
            fitpaddingleftCollection.Add(0);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the position of passed object on panel.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Object's position.</returns>
        public static double GetPosition(DependencyObject obj)
        {
            return (double)obj.GetValue(ChartCartesianAxisLabelsPanel.PositionProperty);
        }

        /// <summary>
        /// Sets the position of passed object on panel.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="position">The position.</param>
        public static void SetPosition(DependencyObject obj, double position)
        {
            obj.SetValue(ChartCartesianAxisLabelsPanel.PositionProperty, position);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <param name="dObj">The d obj.</param>
        /// <param name="length">The length.</param>
        /// <returns>The double position</returns>
        private double GetPosition(DependencyObject dObj, double length)
        {
            if (this.Axis != null)
            {
                return length * this.Axis.ValueToCoefficient((double)dObj.GetValue(PositionProperty));
            }

            return 0d;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            DoLayout(finalSize, false);

            return finalSize;
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
            return ChartLayoutUtils.CheckSize(this.DoLayout(availableSize, true));
        }

        /// <summary>
        /// Updates property value cache and raises AxisChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public void OnAxisChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AxisChanged != null)
            {
                AxisChanged(this, e);
            }
        }

        /// <summary>
        /// Does the layout of panel's items.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <param name="isMeasuring">if set to <c>true</c> [is measuring].</param>
        /// <returns>New element size.</returns>
        private Size DoLayout(Size size, bool isMeasuring)
        {
            bool isOpposed = this.OpposedPosition;
            double oppcoef = isOpposed ? -1 : 1;
            double dimention = 0;
            double offsetLables = 0d;

            switch (Axis.AxisLabelsPosition)
            {
                case AxisLabels.Low:
                    offsetLables = 0;
                    break;
                case AxisLabels.High:
                    if (Axis.Orientation == Orientation.Vertical)
                    {
                        offsetLables = Axis.gridWidth - Math.Max(Math.Max(this.Axis.TickSize, this.Axis.SmallTickSize), 0);
                    }
                    else
                    {
                        offsetLables = Axis.gridheight - Math.Max(Math.Max(this.Axis.TickSize, this.Axis.SmallTickSize), 0);
                    }
                    break;
                case AxisLabels.NextToAxis:
                    if (Axis.Orientation == Orientation.Horizontal)
                        offsetLables = Axis.XLabelOffset;
                    else
                        offsetLables = Axis.YLabelOffset;
                    break;
            }
            if (Axis.AxisLabelsPosition == AxisLabels.NextToAxis)
            {
                switch (Axis.LabelPosition)
                {
                    case LabelPositions.Inside:
                        if (this.Axis.TickLinesPosition == AxisPositions.Inside)
                        {
                            double ticksize = Math.Max(Math.Max(this.Axis.TickSize, this.Axis.SmallTickSize), 0);
                            if (ticksize < 0)
                            {
                                ticksize = -ticksize;
                            }
                            offsetLables = offsetLables + Axis.LabelFontSize + 10 + ticksize;
                        }
                        else if (this.Axis.TickLinesPosition == AxisPositions.Cross)
                        {
                            double plusValueY = this.Axis.TickSize / (1 / this.Axis.TickLinesRange);
                            double plusValueSmallTick = this.Axis.SmallTickSize / (1 / this.Axis.SmallTickLinesRange);
                            double ticksize = Math.Max(Math.Max(plusValueSmallTick, plusValueY), 0);
                            if (ticksize < 0)
                            {
                                ticksize = -ticksize;
                            }
                            offsetLables = offsetLables + Axis.LabelFontSize + 10 + ticksize;
                        }
                        else
                        {
                            offsetLables = offsetLables + Axis.LabelFontSize + 10;
                        }
                        break;                    
                    case LabelPositions.Outside:
                        if (this.Axis.TickLinesPosition == AxisPositions.Outside)
                        {
                            offsetLables = offsetLables - Math.Max(Math.Max(this.Axis.TickSize, this.Axis.SmallTickSize), 0);
                        }
                        else if (this.Axis.TickLinesPosition == AxisPositions.Cross)
                        {
                            double plusValueY = this.Axis.TickSize / (1 / (1 - this.Axis.TickLinesRange));
                            double plusValueSmallTick = this.Axis.SmallTickSize / (1 / (1 - this.Axis.SmallTickLinesRange));
                            offsetLables = offsetLables - Math.Max(Math.Max(plusValueY, plusValueSmallTick), 0);
                        }
                        break;
                }
            }
            else
            {
                if (isOpposed)
                   {
                    switch (Axis.LabelPosition)
                    {
                        case LabelPositions.Inside:
                            double ticksize = Math.Max(Math.Max(this.Axis.TickSize, this.Axis.SmallTickSize), 0);
                            if (ticksize < 0)
                            {
                                ticksize = -ticksize;
                            }
                            offsetLables = offsetLables - Axis.LabelFontSize - ticksize - 10;

                            break;
                        case LabelPositions.Outside:
                            break;
                    }
                }
                else
                {
                    switch (Axis.LabelPosition)
                    {
                        case LabelPositions.Inside:

                            double ticksize = Math.Max(Math.Max(this.Axis.TickSize, this.Axis.SmallTickSize), 0);
                            if (ticksize < 0)
                            {
                                ticksize = -ticksize;
                            }
                            offsetLables = offsetLables + Axis.LabelFontSize + ticksize + 10;
                            break;
                      
                        case LabelPositions.Outside:
                            break;
                    }  
                }
                
            }
            if (Axis != null)
            {
                if (Axis.Orientation == Orientation.Horizontal)
                {
                    ChartAlignment alignmentX = this.LabelAlignment;
                    ChartAlignment alignmentY = isOpposed ? (Axis.LabelPosition == LabelPositions.Inside ? ChartAlignment.Far : ChartAlignment.Near) : ChartAlignment.Far;
                    Point startPt = new Point(0, isOpposed ? size.Height : 0);
                    Rect clientRect = new Rect(0, 0, size.Width, size.Height);

                    #region EdgeLabelsDrawingMode Fit
 
                    if (this.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Fit && this.InternalChildren.Count > 0)
                    {
                        bool fitFlag = true;
                        int index = 0;
                        Border areaBorder = VisualTreeHelper.GetChild(Axis.Area, 0) as Border;
                        Thickness fitPadding = areaBorder.Padding;// new Thickness(0);
                        do
                        {
                            if (index >= this.InternalChildren.Count - 1)
                                fitFlag = false;
                            FrameworkElement element = this.InternalChildren[index] as FrameworkElement;
 
                            element.Visibility = Visibility.Visible;
                            element.LayoutTransform = null;
 
                            if (isMeasuring)
                            {
                                element.Measure(size);
                            }
 
                            Point connectPoint = new Point(startPt.X + GetPosition(element, size.Width), startPt.Y);
                            Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);
                            Rect parentRect = new Rect(new Point(0, 0), size);

                            ChartAxis secondaryAxis = this.Axis.Area.SecondaryAxis;
                            double secondaryAxisLeft = secondaryAxis.Area.AxesThickness.Left;
                            double secondaryAxisRight = secondaryAxis.Area.AxesThickness.Right;
                            if (index == 0)
                            {
                                double diff = parentRect.Left - bounds.Left;
                                if (bounds.Left < parentRect.Left && ((parentRect.Left - bounds.Left) > secondaryAxisLeft))
                                    fitPadding.Left = parentRect.Left - bounds.Left;
                                else
                                    fitPadding.Left = 0;
                            }
                            if (index == this.InternalChildren.Count - 1)
                            {
                                double lblPosition = bounds.Left +  bounds.Width;
                                double panelSize = parentRect.Left +  parentRect.Width;
                                if (lblPosition > panelSize && (lblPosition - panelSize) > secondaryAxisRight)
                                    fitPadding.Right = lblPosition - panelSize; 
                                else
                                    fitPadding.Right = 0;
                            }
                            index = this.InternalChildren.Count - 1;
                        } while (fitFlag);

                        // Fixes for SD14648-On resizing the chart "ChartArea.AxesThickness" is changing  frequently and ID2196-chartarea cropped automation break
                        double fitpaddingright = 0, fitpaddingleft = 0;
                        bool result = false;
                        fitpaddingright = Math.Round((double)fitPadding.Right, 1);
                        fitpaddingleft = Math.Round((double)fitPadding.Left, 1);
                        for (int i = 0; i < fitpaddingrightCollection.Count; i++)
                        {
                            if (fitpaddingrightCollection[i] == fitpaddingright && fitpaddingleftCollection[i] == fitpaddingleft)
                            {
                                result = false;
                                break;
                            }
                            else { result = true; }
                        }
                        if (areaBorder != null && result)
                        {
                            areaBorder.Padding = fitPadding;
                            fitpaddingrightCollection.Add(fitpaddingright);
                            fitpaddingleftCollection.Add(fitpaddingleft);
                        }
                    }
                    else if (this.EdgeLabelsDrawingMode != EdgeLabelsDrawingMode.Fit && Axis.Area != null)
                    {
                        Border areaBorder = null;
                        if (VisualTreeHelper.GetChildrenCount(Axis.Area) > 0)
                        	areaBorder = VisualTreeHelper.GetChild(Axis.Area, 0) as Border;
                        if (areaBorder != null)
                        {
                            Thickness padding = areaBorder.Padding;
                            padding.Left = 0;
                            padding.Right = 0;
                            areaBorder.Padding = padding;
                        }
                    }
 
                    #endregion 
                    switch (this.IntersectAction)
                    {
                        case ChartLabelIntersectAction.None:
                            #region ChartLabelIntersectAction.None layout
                            {
                                for (int i = 0; i < this.InternalChildren.Count; i++)
                                {
                                    FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                    element.Visibility = Visibility.Visible;

                                    Point connectPoint = new Point(startPt.X + GetPosition(element, size.Width), startPt.Y);

                                    if (isMeasuring)
                                    {
                                        element.LayoutTransform = null;
                                        element.Measure(size);
                                    }


                                    alignmentX = GetLabelAlignment(this.LabelRotateAngle, Orientation.Horizontal);

                                    Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, this.Axis);

                                    Rect rect = new Rect(GetContentSize(element, Axis));
                                    switch (alignmentX)
                                    {
                                        case ChartAlignment.Far:
                                            bounds.X = bounds.X - (rect.Width / 2);
                                            break;
                                        case ChartAlignment.Near:
                                            bounds.X = bounds.X + (rect.Width / 2);
                                            break;
                                    }

                                    if (!isMeasuring)
                                    {
                                        ArrangeElement(element, bounds, this.LabelRotateAngle, HidePartialLabel, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                    }

                                    dimention = Math.Max(dimention, bounds.Height + 1);
                                }
                            }
                            #endregion
                            break;

                        case ChartLabelIntersectAction.Wrap:
                            #region ChartLabelIntersectAction.Wrap layout
                            {
                                double maxH = 0;
                                Point[] wrapPoss = new Point[this.InternalChildren.Count];

                                for (int i = 0; i < this.InternalChildren.Count; i++)
                                {
                                    wrapPoss[i] = new Point(startPt.X + GetPosition(this.InternalChildren[i], size.Width), startPt.Y);
                                }

                                for (int i = 0, c = this.InternalChildren.Count; i < c; i++)
                                {
                                    double width = double.MaxValue;
                                    FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                    element.Visibility = Visibility.Visible;

                                    Point connectPoint = new Point(GetPosition(element, size.Width), 0);

                                    if (i > 0)
                                    {
                                        width = Math.Min(width, Math.Abs(wrapPoss[i].X - wrapPoss[i - 1].X));
                                    }

                                    if (i < c - 1)
                                    {
                                        width = Math.Min(width, Math.Abs(wrapPoss[i + 1].X - wrapPoss[i].X));
                                    }
                                    if (this.InternalChildren.IndexOf(element) == 0 || this.InternalChildren.IndexOf(element) == this.InternalChildren.Count - 1)
                                    {
                                        if (i > 0)
                                        {
                                            width = width - (Math.Min(width, Math.Abs(wrapPoss[i].X - wrapPoss[i - 1].X)) / 2);
                                        }

                                        if (i < c - 1)
                                        {
                                            width = width - (Math.Min(width, Math.Abs(wrapPoss[i + 1].X - wrapPoss[i].X)) / 2);
                                        }
                                    }
                                    if (isMeasuring)
                                    {
                                        element.LayoutTransform = null;
                                        element.Measure(new Size(width, size.Height));
                                    }

                                    alignmentX = GetLabelAlignment(0, Orientation.Horizontal);

                                    Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, 0, offsetLables, Axis);

                                    if (!isMeasuring)
                                    {
                                        bounds.Y = 0;
                                        ArrangeElement(element, bounds, 0, this.HidePartialLabel, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                    }

                                    //SD16967-Chart Re-render when set intersect action as wrap
                                    //if (this.InternalChildren.IndexOf(element) == 0)
                                    //{
                                    //    element.Margin = new Thickness(element.Margin.Left - 0.001, element.Margin.Top, element.Margin.Right - 0.003, element.Margin.Bottom);
                                    //}
                                    maxH = Math.Max(maxH, bounds.Height + 1);
                                }

                                dimention = maxH;
                            }
                            #endregion
                            break;

                        case ChartLabelIntersectAction.MultipleRows:
                            #region ChartLabelIntersectAction.MultipleRows layout
                            {
                                ////rows to layout
                                int[] rows = new int[this.InternalChildren.Count]; ////index of the row for each label
                                ////last x - coordinates for each row
                                double[] lastX = new double[C_rowsCount] { double.MinValue, double.MinValue, double.MinValue, double.MinValue };
                                ////max y - coordinate for each row
                                double[] maxY = new double[C_rowsCount + 1] { 0, 0, 0, 0, 0 };

                                for (int i = 0; i < this.InternalChildren.Count; i++)
                                {
                                    int row = 0;
                                    double ms = double.MaxValue;

                                    FrameworkElement element = this.InternalChildren[i] as FrameworkElement;

                                    element.Visibility = Visibility.Visible;
                                    element.LayoutTransform = null;

                                    if (isMeasuring)
                                    {
                                        element.Measure(size);
                                    }

                                    Point connectPoint = new Point(startPt.X + GetPosition(element, size.Width), startPt.Y);
                                    Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);
                                    Rect parentRect = new Rect(new Point(0, 0), size);
                                    if (this.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Shift)
                                    {
                                        if (bounds.Left < 0)
                                        {
                                            bounds.Offset(-bounds.Left, 0);
                                        }
                                        if (bounds.Right > parentRect.Right)
                                        {
                                            bounds.Offset(parentRect.Right - bounds.Right, 0);
                                        }
                                    }
                                    else if (HidePartialLabel)
                                    {
                                        if (!parentRect.Contains(bounds))
                                        {
                                            element.Visibility = Visibility.Hidden;
                                        }
                                    }
                                    for (int j = 0; j < C_rowsCount; j++)
                                    {
                                        if (lastX[j] < bounds.Left)
                                        {
                                            row = j;
                                            break;
                                        }
                                        else if (lastX[j] - bounds.Left < ms)
                                        {
                                            ms = lastX[row] - bounds.Left;
                                            row = j;
                                        }
                                    }

                                    lastX[row] = bounds.Right;
                                    maxY[row + 1] = Math.Max(maxY[row + 1], bounds.Height);
                                    rows[i] = row;
                                }

                                for (int i = 1; i < maxY.Length; i++)
                                {
                                    maxY[i] += maxY[i - 1];
                                }

                                if (!isMeasuring)
                                {
                                    for (int i = 0; i < this.InternalChildren.Count; i++)
                                    {
                                        FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                        Point connectPoint = new Point(startPt.X + GetPosition(element, size.Width), startPt.Y);

                                        if (isOpposed)
                                        {
                                            connectPoint.Y -= maxY[rows[i]];
                                        }
                                        else
                                        {
                                            connectPoint.Y += maxY[rows[i]];
                                        }

                                        Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);
                                        ArrangeElement(element, bounds, this.LabelRotateAngle, HidePartialLabel, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                    }
                                }

                                dimention = maxY[C_rowsCount];
                            }
                            #endregion
                            break;

                        case ChartLabelIntersectAction.Hide:
                            #region ChartLabelIntersectAction.Hide layout
                            {
                                double maxHeight = 0;
                                Rect previosRect = Rect.Empty;

                                for (int i = 0; i < this.InternalChildren.Count; i++)
                                {
                                    FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                    element.Visibility = Visibility.Visible;

                                    Point connectPoint = new Point(startPt.X + GetPosition(element, size.Width), startPt.Y);

                                    if (isMeasuring)
                                    {
                                        element.LayoutTransform = null;
                                        element.Measure(new Size(2000, 2000));
                                    }

                                    Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);

                                    Rect parentRect = new Rect(new Point(0, 0), size);
                                    if (this.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Shift)
                                    {
                                        if (bounds.Left < 0)
                                        {
                                            bounds.Offset(-bounds.Left, 0);
                                        }
                                        if (bounds.Right > parentRect.Right)
                                        {
                                            bounds.Offset(parentRect.Right - bounds.Right, 0);
                                        }

                                    }
                                    if (!previosRect.IsEmpty && previosRect.IntersectsWith(bounds))
                                    {
                                        if (!isMeasuring)
                                        {
                                            element.Visibility = Visibility.Hidden;
                                        }
                                    }
                                    else
                                    {
                                        if (!isMeasuring)
                                        {
                                            element.Visibility = Visibility.Visible;
                                            ArrangeElement(element, bounds, this.LabelRotateAngle, HidePartialLabel, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                        }

                                        previosRect = bounds;
                                    }

                                    maxHeight = Math.Max(maxHeight, bounds.Height + 1);
                                }

                                dimention = maxHeight;
                            }
                            #endregion
                            break;

                        case ChartLabelIntersectAction.Rotate:
                            #region ChartLabelIntersectAction.Rotate layout
                            {
                                double maxH = 0;
                                double minW = double.MaxValue; ////min label dist 
                                double maxW = 0;
                                double lastX = 0;

                                for (int i = 0; i < this.InternalChildren.Count; i++)
                                {
                                    FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                    element.Visibility = Visibility.Visible;

                                    Point connectPoint = new Point(startPt.X + GetPosition(element, size.Width), startPt.Y);

                                    if (isMeasuring)
                                    {
                                        element.LayoutTransform = null;
                                        element.Measure(size);
                                    }

                                    Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY,this.LabelRotateAngle,offsetLables,Axis);

                                    if ((i != 0) && (bounds.Left < lastX))
                                    {
                                        minW = Math.Min(minW, bounds.Right - lastX);
                                    }

                                    lastX = bounds.Right;
                                    maxH = Math.Max(maxH, bounds.Height);
                                    maxW = Math.Max(maxW, bounds.Width);

                                }

                                double angle = minW != double.MaxValue ? 180 * Math.Atan(maxH / minW) / Math.PI : 0d;
                                angle = Math.Abs(oppcoef * (angle < 0 ? 90 : angle));
                                alignmentX = GetLabelAlignment(angle, Orientation.Horizontal);
                                //if (this.LabelRotateAngle == 0)
                                //{
                                    if (angle != 0)
                                    {
                                        for (int i = 0; i < this.InternalChildren.Count; i++)
                                        {
                                            FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                            Point connectPoint = new Point(startPt.X + GetPosition(element, size.Width), startPt.Y);

                                            Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, angle, offsetLables, Axis);
                                            maxH = Math.Max(maxH, bounds.Height);
                                            ////if (OpposedPosition)
                                            ////  bounds.Y += maxH;        
                                            Rect rect = new Rect(GetContentSize(element, Axis));
                                            switch (alignmentX)
                                            {
                                                case ChartAlignment.Far:
                                                    bounds.X = bounds.X - (rect.Width / 2);
                                                    break;
                                                case ChartAlignment.Near:
                                                    bounds.X = bounds.X + (rect.Width / 2);
                                                    break;
                                            }
                                            if (!isMeasuring)
                                            {
                                                ArrangeElement(element, bounds, angle, HidePartialLabel, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < this.InternalChildren.Count; i++)
                                        {
                                            FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                            Point connectPoint = new Point(startPt.X + GetPosition(element, size.Width), startPt.Y);

                                            Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);
                                            maxH = Math.Max(maxH, bounds.Height);
                                            ////if (OpposedPosition)
                                            ////  bounds.Y += maxH;

                                            if (!isMeasuring)
                                            {
                                                ArrangeElement(element, bounds, this.LabelRotateAngle, HidePartialLabel, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                            }
                                        }
                                    }
                                    dimention = maxH;
                               // }
                                //else
                                //{
                                //    for (int i = 0; i < this.InternalChildren.Count; i++)
                                //    {
                                //        FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                //        element.Visibility = Visibility.Visible;

                                //        Point connectPoint = new Point(startPt.X + GetPosition(element, size.Width), startPt.Y);

                                //        if (isMeasuring)
                                //        {
                                //            element.LayoutTransform = null;
                                //            element.Measure(size);
                                //        }

                                //        alignmentX = GetLabelAlignment(this.LabelRotateAngle, Orientation.Horizontal);

                                //        Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);

                                //        Rect rect = new Rect(GetContentSize(element, Axis));
                                //        switch (alignmentX)
                                //        {
                                //            case ChartAlignment.Far:
                                //                bounds.X = bounds.X - (rect.Width / 2);
                                //                break;
                                //            case ChartAlignment.Near:
                                //                bounds.X = bounds.X + (rect.Width / 2);
                                //                break;
                                //        }
                                //        if (!isMeasuring)
                                //        {
                                //            ArrangeElement(element, bounds, this.LabelRotateAngle, HidePartialLabel, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                //        }

                                //        dimention = Math.Max(dimention, bounds.Height + 1);
                                //    }
                                //}


                            }
                            #endregion
                            break;
                    }
                }
                else
                {
                    ChartAlignment alignmentY = this.LabelAlignment;
                    ChartAlignment alignmentX = isOpposed ? (Axis.LabelPosition == LabelPositions.Inside ? ChartAlignment.Near : ChartAlignment.Far) : ChartAlignment.Near;
                    Point startPt = new Point(this.OpposedPosition ? 0 : size.Width, 0);
                    double labelHeight = 0d;

                    #region EdgeLabelsDrawingMode Fit
 
                    if (this.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Fit && this.InternalChildren.Count > 0)
                    {
                        bool fitFlag = true;
                        int index = 0;
                        Border areaBorder = VisualTreeHelper.GetChild(Axis.Area, 0) as Border;
                        Thickness fitPadding = areaBorder.Padding;
                        do
                        {
                            if (index >= this.InternalChildren.Count - 1)
                                fitFlag = false;
                            FrameworkElement element = this.InternalChildren[index] as FrameworkElement;
 
                            element.Visibility = Visibility.Visible;
                            element.LayoutTransform = null;
 
                            if (isMeasuring)
                            {
                                element.Measure(size);
                            }
 
                            Point connectPoint = new Point(startPt.X, size.Height - GetPosition(element, size.Height));
                            Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);
                            Rect parentRect = new Rect(new Point(0, 0), size);

                            ChartAxis primaryAxis = this.Axis.Area.PrimaryAxis;
                            double primaryAxisTop = primaryAxis.Area.AxesThickness.Top;
                            double primaryAxisBottom = primaryAxis.Area.AxesThickness.Bottom;
                            
                            if (index == 0)
                            {
                                if (bounds.Bottom > parentRect.Bottom && ((bounds.Bottom - parentRect.Bottom) > primaryAxisBottom))
                                    fitPadding.Bottom = bounds.Bottom - parentRect.Bottom; 
                                else
                                    fitPadding.Bottom = 0;
                            }
                            if (index == this.InternalChildren.Count - 1)
                            {
                               if (bounds.Top < 0 && ((bounds.Top * -1) > primaryAxisTop))
                                    fitPadding.Top = bounds.Top * -1; 
                                else
                                    fitPadding.Top = 0;
                            }
                            index = this.InternalChildren.Count - 1;
                            
                       } while (fitFlag);
 
 
                        if (areaBorder != null)
                        {
                            areaBorder.Padding = fitPadding;
                        }
                    }
                    else if (this.EdgeLabelsDrawingMode != EdgeLabelsDrawingMode.Fit && Axis.Area != null)
                    {
                        Border areaBorder = null;
                        if (VisualTreeHelper.GetChildrenCount(Axis.Area) > 0)
                            areaBorder = VisualTreeHelper.GetChild(Axis.Area, 0) as Border;
                        if (areaBorder != null)
                        {
                            Thickness padding = areaBorder.Padding;
                            padding.Top = 0;
                            padding.Bottom = 0;
                            areaBorder.Padding = padding;
                        }
                    }
 
                    #endregion
 
                    switch (this.IntersectAction)
                    {
                        case ChartLabelIntersectAction.Wrap:
                        case ChartLabelIntersectAction.Rotate:
                        case ChartLabelIntersectAction.None:
                            #region ChartLabelIntersectAction.None layout
                            {
                                double maxWidth = 0;

                                for (int i = 0; i < this.InternalChildren.Count; i++)
                                {
                                    FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                    element.Visibility = Visibility.Visible;

                                    Point connectPoint = new Point(startPt.X, size.Height - startPt.Y - GetPosition(element, size.Height));

                                    if (isMeasuring)
                                    {
                                        element.LayoutTransform = null;
                                        element.Measure(size);
                                    }

                                    alignmentY = GetLabelAlignment(this.LabelRotateAngle, Orientation.Vertical);
                                    ChartAxisLabel label = element.DataContext as ChartAxisLabel;
                                    if (this.Axis.EnableBreaks && this.Axis.BreakRange.m_breaksMode == ChartBreaksModes.Manual)
                                    {
                                        foreach (KeyValuePair<DoubleRange, ChartBreakRangeInfo> brk in this.Axis.BreakRange.Breaks)
                                        {
                                            if (brk.Key.Inside(label.Position) && label.Position != brk.Key.End && label.Position != brk.Key.Start)
                                                element.Visibility = Visibility.Hidden;
                                            if (label.Position == brk.Key.Start)
                                            {
                                                element.Margin = new Thickness(0, 5, 0, -5);
                                            }
                                            if (label.Position == brk.Key.End)
                                            {
                                                element.Margin = new Thickness(0,-5,0,0);
                                            }
                                        }
                                    }
            
                                    Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);

                                    if (!isMeasuring)
                                    {
                                        if (this.Axis.m_visibleRange == new DoubleRange(0, 1))
                                            ArrangeElement(element, bounds, this.LabelRotateAngle, false, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                        else
                                            ArrangeElement(element, bounds, this.LabelRotateAngle, HidePartialLabel, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                    }
                                    labelHeight = bounds.Height;
                                    //if (Axis.Area != null && Axis.Area.IsSync == true)
                                    //{
                                    //    if (this.InternalChildren.IndexOf(element) == 0)
                                    //    {
                                    //        element.Margin = new Thickness(element.Margin.Left, element.Margin.Top - 0.001, element.Margin.Right, element.Margin.Bottom - 0.003);
                                    //    }
                                    //    if (this.InternalChildren.IndexOf(element) == this.InternalChildren.Count - 1)
                                    //    {
                                    //        element.Margin = new Thickness(element.Margin.Left, element.Margin.Top + 0.0005, element.Margin.Right, element.Margin.Bottom - 0.003);
                                    //    }
                                    //}

                                    maxWidth = Math.Max(maxWidth, bounds.Width);
                                }


                                dimention = maxWidth;
                            }
                            #endregion
                            break;

                        case ChartLabelIntersectAction.MultipleRows:
                            #region ChartLabelIntersectAction.MultipleRows layout
                            {
                                int[] rows = new int[this.InternalChildren.Count];
                                double[] lastY = new double[C_rowsCount] { double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue };
                                double[] maxX = new double[C_rowsCount + 1] { 0, 0, 0, 0, 0 };

                                for (int i = 0; i < this.InternalChildren.Count; i++)
                                {
                                    int row = 0;
                                    double ms = double.MaxValue;

                                    FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                    element.Visibility = Visibility.Visible;

                                    if (isMeasuring)
                                    {
                                        element.LayoutTransform = null;
                                        element.Measure(size);
                                    }

                                    Point connectPoint = new Point(startPt.X, size.Height - GetPosition(element, size.Height));

                                    Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);

                                    for (int j = 0; j < C_rowsCount; j++)
                                    {
                                        if (lastY[j] > bounds.Bottom)
                                        {
                                            row = j;
                                            break;
                                        }
                                        else if (lastY[j] - bounds.Top < ms)
                                        {
                                            ms = lastY[row] - bounds.Top;
                                            row = j;
                                        }
                                    }

                                    lastY[row] = bounds.Top;
                                    maxX[row + 1] = Math.Max(maxX[row + 1], bounds.Width);
                                    rows[i] = row;
                                }

                                for (int i = 1; i < maxX.Length; i++)
                                {
                                    maxX[i] += maxX[i - 1];
                                }

                                if (!isMeasuring)
                                {
                                    for (int i = 0; i < this.InternalChildren.Count; i++)
                                    {
                                        FrameworkElement element = this.InternalChildren[i] as FrameworkElement;
                                        Point connectPoint = new Point(startPt.X, size.Height - GetPosition(element, size.Height));

                                        if (OpposedPosition)
                                        {
                                            connectPoint.X += maxX[rows[i]];
                                        }
                                        else
                                        {
                                            connectPoint.X -= maxX[rows[i]];
                                        }

                                        Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);

                                        ArrangeElement(element, bounds, this.LabelRotateAngle, HidePartialLabel, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                        labelHeight = bounds.Height;
                                    }
                                }

                                dimention = maxX[C_rowsCount];
                            }
                            #endregion
                            break;

                        case ChartLabelIntersectAction.Hide:
                            #region ChartLabelIntersectAction.Hide layout
                            {
                                Rect previosRect = Rect.Empty;

                                foreach (FrameworkElement element in this.InternalChildren)
                                {
                                    element.Visibility = Visibility.Visible;

                                    Point connectPoint = new Point(startPt.X, size.Height - startPt.Y - GetPosition(element, size.Height));

                                    if (isMeasuring)
                                    {
                                        element.LayoutTransform = null;
                                        element.Measure(size);
                                    }

                                    Rect bounds = GetElementBounds(element, connectPoint, alignmentX, alignmentY, this.LabelRotateAngle, offsetLables, Axis);
                                    //Rect parentRect = new Rect(new Point(0, 0), size);
                                    //if (this.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Shift)
                                    //{
                                    //    if (bounds.Top < 0)
                                    //    {
                                    //        bounds.Offset(0, -bounds.Top);
                                    //    }                                        
                                    //    if (bounds.Bottom > parentRect.Bottom)
                                    //    {
                                    //        bounds.Offset(0, parentRect.Bottom - bounds.Bottom);
                                    //    }                            
                                    //}
                                    if (!isMeasuring)
                                    {
                                        element.Arrange(bounds);
                                    }

                                    if (!previosRect.IsEmpty && previosRect.IntersectsWith(bounds))
                                    {
                                        if (!isMeasuring)
                                        {
                                            element.Visibility = Visibility.Hidden;
                                        }
                                    }
                                    else
                                    {
                                        if (!isMeasuring)
                                        {
                                            element.Visibility = Visibility.Visible;
                                            ArrangeElement(element, bounds, this.LabelRotateAngle, HidePartialLabel, this.EdgeLabelsDrawingMode, size, Axis.LabelPosition);
                                        }

                                        previosRect = bounds;
                                    }
                                    labelHeight = bounds.Height;
                                    dimention = Math.Max(dimention, bounds.Width);
                                }
                            }
                            #endregion
                            break;
                    }

                    if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Center && Axis.Area != null && Axis.Area.IsSync == true)
                    {
                        SyncChartAreas sArea = Axis.Area.ChartAreaParent as SyncChartAreas;
                        if (Axis.Area.index == 0)
                        {
                            if (labelHeight / 2 > Axis.Area.Padding.Bottom)
                            {
                                if (Axis.Area.Padding.Bottom == 0d)
                                {
                                    Axis.Area.AreaPaddingBottom = 0d;
                                    if (sArea.Areas.Count > 0)
                                    {
                                        //This is loop is added to avoid argument out of index exception when using one area in SyncChart.
                                        if (sArea.Areas.Count > 1)
                                        {
                                            sArea.Areas[Axis.Area.index + 1].AreaPaddingBottom = 0d;
                                        }
                                        else
                                        {
                                            sArea.Areas[0].AreaPaddingBottom = 0d;
                                        }
                                    }
                                }
                                //This condition is added to show the primary axis header when the areas count is one.
                                if (sArea.Areas.Count > 1)
                                {
                                    Axis.Area.Padding = new Thickness(0, 0, 0, (labelHeight / 2));
                                    Axis.Area.AreaPaddingBottom = (labelHeight / 2);
                                }
                            }
                        }
                        else
                        {
                            if (labelHeight / 2 > Axis.Area.AreaPaddingBottom)
                            {
                                int index = sArea.Areas.IndexOf(Axis.Area);
                                sArea.Areas[index - 1].Padding = new Thickness(0, 0, 0, sArea.Areas[index - 1].Padding.Bottom + labelHeight / 2);
                                Axis.Area.AreaPaddingBottom = sArea.Areas[index - 1].Padding.Bottom;
                            }
                        }
                    }

                }
            }

            dimention = Math.Abs(dimention);

            if (Axis != null)
            {
                return new Size(Axis.Orientation == Orientation.Vertical ? dimention : 0d, Axis.Orientation == Orientation.Horizontal ? dimention : 0d);
            }
            else
            {
                return new Size(0, 0);
            }
        }

        /// <summary>
        /// Calls OnAxisChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartCartesianAxisLabelsPanel instance = (ChartCartesianAxisLabelsPanel)d;
            instance.OnAxisChanged(e);
        }
        #endregion

        #region Helper methods

        /// <summary>
        /// Gets the size of the content.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="axis"></param>
        /// <returns>Size of content.</returns>
        private static Size GetContentSize(UIElement element, ChartAxis axis)
        {
            if (VisualTreeHelper.GetChildrenCount(element) == 1)
            {
                Size contentSize = (VisualTreeHelper.GetChild(element, 0) as UIElement).DesiredSize;

                if (axis.LabelHorizontalAlignment == HorizontalAlignment.Stretch && axis.LabelVerticalAlignment == VerticalAlignment.Stretch)
                {
                    return contentSize;
                }
                else
                {
                    if (axis.LabelHorizontalAlignment != HorizontalAlignment.Stretch)
                    {
                        contentSize.Width = (double.IsNaN(axis.LabelWidth) || axis.LabelWidth == 0) ? contentSize.Width : axis.LabelWidth;
                    }
                    if (axis.LabelVerticalAlignment != VerticalAlignment.Stretch)
                    {
                        contentSize.Height = (double.IsNaN(axis.LabelHeight) || axis.LabelHeight == 0) ? contentSize.Height : axis.LabelHeight;
                    }
                    return contentSize;
                }
            }

            return Size.Empty;
        }

        /// <summary>
        /// Gets the element bounds.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="connectPoint">The connect point.</param>
        /// <param name="alignmentX">The alignment X.</param>
        /// <param name="alignmentY">The alignment Y.</param>
        /// <param name="axis"></param>
        /// <returns>Bounds of element.</returns>
        private static Rect GetElementBounds(FrameworkElement element, Point connectPoint, ChartAlignment alignmentX, ChartAlignment alignmentY, ChartAxis axis)
        {
            Rect rect = new Rect(GetContentSize(element, axis));

            switch (alignmentX)
            {
                case ChartAlignment.Center:
                    rect.X = connectPoint.X - rect.Width / 2;
                    break;
                case ChartAlignment.Near:
                    rect.X = connectPoint.X - rect.Width;
                    break;
                case ChartAlignment.Far:
                    rect.X = connectPoint.X;
                    break;
            }

            switch (alignmentY)
            {
                case ChartAlignment.Center:
                    rect.Y = connectPoint.Y - rect.Height / 2;
                    break;
                case ChartAlignment.Near:
                    rect.Y = connectPoint.Y - rect.Height;
                    break;
                case ChartAlignment.Far:
                    rect.Y = connectPoint.Y;
                    break;
            }

            return rect;
        }

        /// <summary>
        /// Gets the element bounds.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="connectPoint">The connect point.</param>
        /// <param name="alignmentX">The alignment X.</param>
        /// <param name="alignmentY">The alignment Y.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="offsetLables"></param>
        /// <param name="axis"></param>
        /// <returns>Bounds of element.</returns>
        private static Rect GetElementBounds(FrameworkElement element, Point connectPoint, ChartAlignment alignmentX, ChartAlignment alignmentY, double angle, double offsetLables, ChartAxis axis)
        {
            Rect rect = new Rect(GetContentSize(element, axis));



            double width = rect.Width;
            double height = rect.Height;

            double absCos = Math.Abs(Math.Cos(angle * ChartMath.ToRadial));
            double absSin = Math.Abs(Math.Sin(angle * ChartMath.ToRadial));

            rect.Width = absCos * width + absSin * height;
            rect.Height = absSin * width + absCos * height;
            switch (alignmentX)
            {
                case ChartAlignment.Center:
                    rect.X = (connectPoint.X - rect.Width / 2);
                    break;
                case ChartAlignment.Near:
                    rect.X = (connectPoint.X - rect.Width) + offsetLables;
                    break;
                case ChartAlignment.Far:
                    rect.X = connectPoint.X;
                    break;
            }

            switch (alignmentY)
            {
                case ChartAlignment.Center:
                    rect.Y = connectPoint.Y - rect.Height / 2;
                    break;
                case ChartAlignment.Near:
                    rect.Y = connectPoint.Y - rect.Height;
                    break;
                case ChartAlignment.Far:
                    rect.Y = connectPoint.Y - offsetLables;
                    break;
            }

            return rect;
        }

        /// <summary>
        /// Arranges the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="partialHidingRequired">if set to <c>true</c> partial hiding is required.</param>
        /// <param name="drawingMode">The drawing mode.</param>
        /// <param name="parentSize">Size of the parent.</param>
        /// <param name="LabelPotition"></param>
        private static void ArrangeElement(FrameworkElement element, Rect bounds, double angle, bool partialHidingRequired, EdgeLabelsDrawingMode drawingMode, Size parentSize, LabelPositions LabelPotition)
        {
            element.LayoutTransform = new RotateTransform(angle, bounds.Width / 2, bounds.Height / 2);
            Rect parentRect = new Rect(new Point(0, 0), parentSize);
            if (drawingMode == EdgeLabelsDrawingMode.Shift)
            {
                switch (LabelPotition)
                {
                    case LabelPositions.Outside:
                        if (bounds.Left < 0)
                        {
                            bounds.Offset(-bounds.Left, 0);
                        }

                        if (bounds.Top < 0)
                        {
                            bounds.Offset(0, -bounds.Top);
                        }

                        if (bounds.Right > parentRect.Right)
                        {
                            bounds.Offset(parentRect.Right - bounds.Right, 0);
                        }

                        if (bounds.Bottom > parentRect.Bottom)
                        {
                            bounds.Offset(0, parentRect.Bottom - bounds.Bottom);
                        }
                        break;
                    case LabelPositions.Inside:
                        if (bounds.Left < bounds.Y)
                        {
                            bounds.Offset(-bounds.Left, 0);
                        }
                        if (bounds.Right > parentRect.Right)
                        {
                            bounds.Offset(parentRect.Right - bounds.Right, 0);
                        }
                        break;
                }
                if (bounds.Top < 0)
                {
                    bounds.Offset(0, -bounds.Top);
                }
            }
            if (partialHidingRequired)
            {
                if (drawingMode == EdgeLabelsDrawingMode.Center)
                {
                    if (bounds.X != 0)
                    parentRect.Y = parentRect.Y + (+bounds.Y);
                    if (!parentRect.Contains(bounds))
                    {

                        element.Visibility = Visibility.Hidden;
                    }
                }
            }

            element.Arrange(bounds);
        }

        /// <summary>
        /// Gets the label alignment X.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="parentAxisOrientation">The Parent Axis orientation.</param>
        /// <returns>Returns the ChartAlignment</returns>
        private static ChartAlignment GetLabelAlignment(double angle, Orientation parentAxisOrientation)
        {
            double realAngle = angle % 180;
            if (realAngle == 0 || realAngle == 1 || realAngle == 90 || realAngle==-90)
            {
                return ChartAlignment.Center;
            }

            if (realAngle < 90)
            {
                return (parentAxisOrientation == Orientation.Horizontal) ? ChartAlignment.Far : ChartAlignment.Near;
            }
            else
            {
                return (parentAxisOrientation == Orientation.Horizontal) ? ChartAlignment.Near : ChartAlignment.Far;
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Axis = null;
            this.AxisChanged = null;
            // Fixes for SD14648-On resizing the chart "ChartArea.AxesThickness" is changing  frequently and ID2196-chartarea cropped automation break
            fitpaddingleftCollection = null;
            fitpaddingrightCollection = null;
        }

        #endregion
    }
}
