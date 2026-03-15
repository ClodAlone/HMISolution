#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;


#if !WinRT
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
namespace Syncfusion.Windows.Controls
#else
using Syncfusion.WinRT;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.System;

namespace Syncfusion.WinRT.Controls
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    [TemplatePart(Name = "ScrollContentPresenter", Type = typeof(ScrollableContentPresenter))]
    [TemplatePart(Name = "HorizontalScrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "VerticalScrollBar", Type = typeof(ScrollBar))]
    public class ScrollableContentViewer : ContentControl, IDisposable
    {
        #region Fields
        private bool _inMeasure;
        private IScrollableInfo _scrollInfo;
        internal const double _scrollLineDelta = 16.0;
        private Visibility _scrollVisibilityX;
        private Visibility _scrollVisibilityY;
        private bool _templatedParentHandlesScrolling;
        private double _xExtent;
        private double _xOffset;
        private double _xViewport;
        private double _yExtent;
        private double _yOffset;
        private double _yViewport;

        private const string ElementHorizontalScrollBarName = "HorizontalScrollBar";
        private const string ElementScrollContentPresenterName = "ScrollContentPresenter";
        private const string ElementVerticalScrollBarName = "VerticalScrollBar";

        internal event ScrollChangedDelegate ScrollChanged;
        #endregion

        #region Local Dependency Properties
#if !WinRT
        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty = DependencyProperty.Register("ComputedHorizontalScrollBarVisibility", typeof(Visibility), typeof(ScrollableContentViewer), null);
        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty = DependencyProperty.Register("ComputedVerticalScrollBarVisibility", typeof(Visibility), typeof(ScrollableContentViewer), null);
#else
        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty = DependencyProperty.Register("ComputedHorizontalScrollBarVisibility", typeof(Visibility), typeof(ScrollableContentViewer), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty = DependencyProperty.Register("ComputedVerticalScrollBarVisibility", typeof(Visibility), typeof(ScrollableContentViewer), new PropertyMetadata(Visibility.Visible));
#endif
        public static readonly DependencyProperty ExtentHeightProperty = DependencyProperty.Register("ExtentHeight", typeof(double), typeof(ScrollableContentViewer), null);
        public static readonly DependencyProperty ExtentWidthProperty = DependencyProperty.Register("ExtentWidth", typeof(double), typeof(ScrollableContentViewer), null);
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ScrollableContentViewer), null);
        public static readonly DependencyProperty ScrollableHeightProperty = DependencyProperty.Register("ScrollableHeight", typeof(double), typeof(ScrollableContentViewer), null);
        public static readonly DependencyProperty ScrollableWidthProperty = DependencyProperty.Register("ScrollableWidth", typeof(double), typeof(ScrollableContentViewer), null);
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ScrollableContentViewer), null);
        public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register("ViewportHeight", typeof(double), typeof(ScrollableContentViewer), null);
        public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.Register("ViewportWidth", typeof(double), typeof(ScrollableContentViewer), null);
        #endregion

        #region Attached Dependency Properties
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.RegisterAttached("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ScrollableContentViewer), new PropertyMetadata(new PropertyChangedCallback(ScrollableContentViewer.OnScrollBarVisibilityChanged)));
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.RegisterAttached("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ScrollableContentViewer), new PropertyMetadata(new PropertyChangedCallback(ScrollableContentViewer.OnScrollBarVisibilityChanged)));
        public static readonly DependencyProperty CanContentScrollProperty = DependencyProperty.RegisterAttached("CanContentScroll", typeof(bool), typeof(ScrollableContentViewer), new PropertyMetadata(false));

        public static ScrollBarVisibility GetHorizontalScrollBarVisibility(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ScrollBarVisibility)element.GetValue(HorizontalScrollBarVisibilityProperty);
        }

        public static void SetHorizontalScrollBarVisibility(DependencyObject element, ScrollBarVisibility horizontalScrollBarVisibility)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(HorizontalScrollBarVisibilityProperty, (Enum)horizontalScrollBarVisibility);
        }

        public static ScrollBarVisibility GetVerticalScrollBarVisibility(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ScrollBarVisibility)element.GetValue(VerticalScrollBarVisibilityProperty);
        }

        public static void SetVerticalScrollBarVisibility(DependencyObject element, ScrollBarVisibility verticalScrollBarVisibility)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(VerticalScrollBarVisibilityProperty, (Enum)verticalScrollBarVisibility);
        }

        /// <summary> 
        /// Helper for setting CanContentScroll property.
        /// </summary>
        public static void SetCanContentScroll(DependencyObject element, bool canContentScroll)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CanContentScrollProperty, canContentScroll);
        }

        /// <summary> 
        /// Helper for reading CanContentScroll property.
        /// </summary> 
        public static bool GetCanContentScroll(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return ((bool)element.GetValue(CanContentScrollProperty));
        }
        #endregion

        public ScrollableContentViewer()
        {
            this.DefaultStyleKey = typeof(ScrollableContentViewer);
        }

        internal bool CanForwardPropertyValue(DependencyProperty property)
        {
            return (base.ReadLocalValue(property) == DependencyProperty.UnsetValue);
        }

        private void HandleScroll(Orientation orientation, ScrollEventArgs e)
        {
            if (this.ElementScrollContentPresenter != null)
            {
                IScrollableInfo scrollableChild = ScrollableChild;
                double offset = (Orientation.Horizontal == orientation) ? this.ElementScrollContentPresenter.HorizontalOffset : this.ElementScrollContentPresenter.VerticalOffset;
                double extent = (Orientation.Horizontal == orientation) ? this.ElementScrollContentPresenter.ViewportWidth : this.ElementScrollContentPresenter.ViewportHeight;
                double newValue = offset;
                switch (e.ScrollEventType)
                {
                    case ScrollEventType.SmallDecrement:
                        if (scrollableChild != null)
                        {
                            if (orientation == Orientation.Horizontal && ComputedHorizontalScrollBarVisibility == Visibility.Visible)
                                scrollableChild.LineLeft();
                            else
                                scrollableChild.LineUp();
                        }
                        else
                            newValue -= 16.0;
                        break;

                    case ScrollEventType.SmallIncrement:
                        if (scrollableChild != null)
                        {
                            if (orientation == Orientation.Horizontal && ComputedHorizontalScrollBarVisibility == Visibility.Visible)
                                scrollableChild.LineRight();
                            else
                                scrollableChild.LineDown();
                        }
                        else
                            newValue += 16.0;
                        break;

                    case ScrollEventType.LargeDecrement:
                        if (scrollableChild != null)
                        {
                            if (orientation == Orientation.Horizontal && ComputedHorizontalScrollBarVisibility == Visibility.Visible)
                                scrollableChild.PageLeft();
                            else
                                scrollableChild.PageUp();
                        }
                        else
                            newValue -= extent;
                        break;

                    case ScrollEventType.LargeIncrement:
                        if (scrollableChild != null)
                        {
                            if (orientation == Orientation.Horizontal && ComputedHorizontalScrollBarVisibility == Visibility.Visible)
                                scrollableChild.PageRight();
                            else
                                scrollableChild.PageDown();
                        }
                        else
                            newValue += extent;
                        break;


                    case ScrollEventType.ThumbPosition:
                    case ScrollEventType.ThumbTrack:
                        newValue = e.NewValue;
                        break;

                    case ScrollEventType.First:
                        newValue = double.MinValue;
                        break;

                    case ScrollEventType.Last:
                        newValue = double.MaxValue;
                        break;
                }
                newValue = Math.Max(newValue, 0.0);
                if (Orientation.Horizontal == orientation)
                {
                    newValue = Math.Min(this.ScrollableWidth, newValue);
                }
                else
                {
                    newValue = Math.Min(this.ScrollableHeight, newValue);
                }
                if (!DoubleUtil.AreClose(offset, newValue))
                {
                    if (Orientation.Horizontal == orientation)
                    {
                        this.ElementScrollContentPresenter.SetHorizontalOffset(newValue);
                    }
                    else
                    {
                        this.ElementScrollContentPresenter.SetVerticalOffset(newValue);
                    }
                    if (Orientation.Horizontal == orientation)
                    {
                        this._xOffset = this.HorizontalOffset;
                        this.HorizontalOffset = newValue;
                        if (this.ElementHorizontalScrollBar != null)
                        {
                            this.ElementHorizontalScrollBar.Value = newValue;
                        }
                    }
                    else if (orientation == Orientation.Vertical)
                    {
                        this._yOffset = this.VerticalOffset;
                        this.VerticalOffset = newValue;
                        if (this.ElementVerticalScrollBar != null)
                        {
                            this.ElementVerticalScrollBar.Value = newValue;
                        }
                    }
                    if (this.ScrollChanged != null)
                    {
                        this.ScrollChanged(this.HorizontalOffset, this.VerticalOffset);
                    }
                }
            }
        }

        public void InvalidateScrollInfo()
        {
            if (this.ScrollInfo != null)
            {
                if (!this._inMeasure)
                {
                    double extentWidth = this.ScrollInfo.ExtentWidth;
                    double viewportWidth = this.ScrollInfo.ViewportWidth;
                    if ((this.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto) && (((this._scrollVisibilityX == Visibility.Collapsed) && (extentWidth > viewportWidth)) || ((this._scrollVisibilityX == Visibility.Visible) && (extentWidth < viewportWidth))))
                    {
                        base.InvalidateMeasure();
                    }
                    else
                    {
                        extentWidth = this.ScrollInfo.ExtentHeight;
                        viewportWidth = this.ScrollInfo.ViewportHeight;
                        if ((this.VerticalScrollBarVisibility == ScrollBarVisibility.Auto) && (((this._scrollVisibilityY == Visibility.Collapsed) && (extentWidth > viewportWidth)) || ((this._scrollVisibilityY == Visibility.Visible) && (extentWidth < viewportWidth))))
                        {
                            base.InvalidateMeasure();
                        }
                    }
                }
                if ((((this.HorizontalOffset != this.ScrollInfo.HorizontalOffset) || (this.VerticalOffset != this.ScrollInfo.VerticalOffset)) || ((this.ViewportWidth != this.ScrollInfo.ViewportWidth) || (this.ViewportHeight != this.ScrollInfo.ViewportHeight))) || ((this.ExtentWidth != this.ScrollInfo.ExtentWidth) || (this.ExtentHeight != this.ScrollInfo.ExtentHeight)))
                {
                    double horizontalOffset = this.HorizontalOffset;
                    double verticalOffset = this.VerticalOffset;
                    double viewportX = this.ViewportWidth;
                    double viewportHeight = this.ViewportHeight;
                    double extentX = this.ExtentWidth;
                    double extentHeight = this.ExtentHeight;
                    double scrollableWidth = this.ScrollableWidth;
                    double scrollableHeight = this.ScrollableHeight;
                    bool modified = false;
                    try
                    {
                        if (horizontalOffset != this.ScrollInfo.HorizontalOffset)
                        {
                            this._xOffset = this.ScrollInfo.HorizontalOffset;
                            this.HorizontalOffset = this._xOffset;
                            modified = true;
                        }
                        if (verticalOffset != this.ScrollInfo.VerticalOffset)
                        {
                            this._yOffset = this.ScrollInfo.VerticalOffset;
                            this.VerticalOffset = this._yOffset;
                            modified = true;
                        }
                        if (viewportX != this.ScrollInfo.ViewportWidth)
                        {
                            this._xViewport = this.ScrollInfo.ViewportWidth;
                            this.ViewportWidth = this._xViewport;
                            modified = true;
                        }
                        if (viewportHeight != this.ScrollInfo.ViewportHeight)
                        {
                            this._yViewport = this.ScrollInfo.ViewportHeight;
                            this.ViewportHeight = this._yViewport;
                            modified = true;
                        }
                        if (extentX != this.ScrollInfo.ExtentWidth)
                        {
                            this._xExtent = this.ScrollInfo.ExtentWidth;
                            this.ExtentWidth = this._xExtent;
                            modified = true;
                        }
                        if (extentHeight != this.ScrollInfo.ExtentHeight)
                        {
                            this._yExtent = this.ScrollInfo.ExtentHeight;
                            this.ExtentHeight = this._yExtent;
                            modified = true;
                        }
                        double num11 = this.ScrollableWidth;
                        if (scrollableWidth != this.ScrollableWidth)
                        {
                            this.ScrollableWidth = num11;
                            modified = true;
                        }
                        double num12 = this.ScrollableHeight;
                        if (scrollableHeight != this.ScrollableHeight)
                        {
                            this.ScrollableHeight = num12;
                            modified = true;
                        }
                    }
                    finally
                    {
                        if (modified)
                        {
                            if (horizontalOffset != this._xOffset)
                            {
                                if (ScrollableChild != null && this.ElementHorizontalScrollBar != null)
                                {
                                    this.ElementHorizontalScrollBar.Value = HorizontalOffset;
                                }
                                else
#if !WinRT
                                    this.HandleScroll(Orientation.Horizontal, new ScrollEventArgs(ScrollEventType.ThumbPosition, this._xOffset));
#else
                                    this.HandleScroll(Orientation.Horizontal, new ScrollEventArgs());
#endif
                            }
                            if (verticalOffset != this._yOffset)
                            {
                                if (ScrollableChild != null && this.ElementVerticalScrollBar != null)
                                {
                                    this.ElementVerticalScrollBar.Value = VerticalOffset;
                                }
                                else
#if !WinRT
                                    this.HandleScroll(Orientation.Vertical, new ScrollEventArgs(ScrollEventType.ThumbPosition, this._yOffset));
#else
                                    this.HandleScroll(Orientation.Vertical, new ScrollEventArgs());
#endif
                            }
                            if (this.ScrollChanged != null)
                            {
                                this.ScrollChanged(this.HorizontalOffset, this.VerticalOffset);
                            }
                            if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
                            {
                                //ScrollContentViewerAutomationPeer orCreateAutomationPeer = base.GetOrCreateAutomationPeer() as ScrollContentViewerAutomationPeer;
                                //if (orCreateAutomationPeer != null)
                                //{
                                //    orCreateAutomationPeer.RaiseAutomationEvents(extentX, extentHeight, viewportX, viewportHeight, horizontalOffset, verticalOffset);
                                //}
                            }
                        }
                    }
                }
            }
        }

        internal void LineDown()
        {
#if !WinRT
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs(ScrollEventType.SmallIncrement, 0.0));
#else
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs());
#endif
        }

        internal void LineLeft()
        {
#if !WinRT
            this.HandleScroll(Orientation.Horizontal, new ScrollEventArgs(ScrollEventType.SmallDecrement, 0.0));
#else
            this.HandleScroll(Orientation.Horizontal, new ScrollEventArgs());
#endif
        }

        internal void LineRight()
        {
#if !WinRT
            this.HandleScroll(Orientation.Horizontal, new ScrollEventArgs(ScrollEventType.SmallIncrement, 0.0));
#else
            this.HandleScroll(Orientation.Horizontal, new ScrollEventArgs());
#endif
        }

        internal void LineUp()
        {
#if !WinRT
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs(ScrollEventType.SmallDecrement, 0.0));
#else
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs());
#endif

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
        protected override Size MeasureOverride(Size constraint)
        {
            UIElement element = (VisualTreeHelper.GetChildrenCount(this) == 0) ? null : (VisualTreeHelper.GetChild(this, 0) as UIElement);
            if (element == null)
            {
                return new Size();
            }
            IScrollableInfo scrollInfo = this.ScrollInfo;
            ScrollBarVisibility verticalScrollBarVisibility = this.VerticalScrollBarVisibility;
            ScrollBarVisibility horizontalScrollBarVisibility = this.HorizontalScrollBarVisibility;
            bool showVerticalScroll = verticalScrollBarVisibility == ScrollBarVisibility.Auto;
            bool showHorizontalScroll = horizontalScrollBarVisibility == ScrollBarVisibility.Auto;
            Visibility newComputedVerticalVisibility = (verticalScrollBarVisibility == ScrollBarVisibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
            Visibility newComputedHorizonalVisibility = (horizontalScrollBarVisibility == ScrollBarVisibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
            try
            {
                this._inMeasure = true;
                if (this._scrollVisibilityY != newComputedVerticalVisibility)
                {
                    this._scrollVisibilityY = newComputedVerticalVisibility;
                    this.ComputedVerticalScrollBarVisibility = this._scrollVisibilityY;
                }
                if (this._scrollVisibilityX != newComputedHorizonalVisibility)
                {
                    this._scrollVisibilityX = newComputedHorizonalVisibility;
                    this.ComputedHorizontalScrollBarVisibility = this._scrollVisibilityX;
                }
                if (scrollInfo != null)
                {
                    scrollInfo.CanHorizontallyScroll = horizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                    scrollInfo.CanVerticallyScroll = verticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                }
                element.Measure(constraint);
                if ((scrollInfo != null) && (showVerticalScroll || showHorizontalScroll))
                {
                    bool horizontalChanged = showHorizontalScroll && (scrollInfo.ExtentWidth > scrollInfo.ViewportWidth);
                    bool verticalChanged = showVerticalScroll && (scrollInfo.ExtentHeight > scrollInfo.ViewportHeight);
                    if (horizontalChanged && (this._scrollVisibilityX != Visibility.Visible))
                    {
                        this._scrollVisibilityX = Visibility.Visible;
                        this.ComputedHorizontalScrollBarVisibility = this._scrollVisibilityX;
                    }
                    if (verticalChanged && (this._scrollVisibilityY != Visibility.Visible))
                    {
                        this._scrollVisibilityY = Visibility.Visible;
                        this.ComputedVerticalScrollBarVisibility = this._scrollVisibilityY;
                    }
                    if (horizontalChanged || verticalChanged)
                    {
                        element.InvalidateMeasure();
                        element.Measure(constraint);
                    }
                    if ((showHorizontalScroll && showVerticalScroll) && (horizontalChanged != verticalChanged))
                    {
                        bool verifyHorizontal = !horizontalChanged && (scrollInfo.ExtentWidth > scrollInfo.ViewportWidth);
                        bool verifyVertical = !verticalChanged && (scrollInfo.ExtentHeight > scrollInfo.ViewportHeight);
                        if (verifyHorizontal)
                        {
                            if (this._scrollVisibilityX != Visibility.Visible)
                            {
                                this._scrollVisibilityX = Visibility.Visible;
                                this.ComputedHorizontalScrollBarVisibility = this._scrollVisibilityX;
                            }
                        }
                        else if (verifyVertical && (this._scrollVisibilityY != Visibility.Visible))
                        {
                            this._scrollVisibilityY = Visibility.Visible;
                            this.ComputedVerticalScrollBarVisibility = this._scrollVisibilityY;
                        }
                        if (verifyHorizontal || verifyVertical)
                        {
                            element.InvalidateMeasure();
                            element.Measure(constraint);
                        }
                    }
                }
            }
            finally
            {
                this._inMeasure = false;
            }
            return element.DesiredSize;
        }

#if !WinRT

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ScrollableContentViewerAutomationPeer(this);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled && !this.TemplatedParentHandlesScrolling)
            {
                switch (e.Key)
                {
                    case Key.PageUp:
                    case Key.PageDown:
                    case Key.End:
                    case Key.Home:
                    case Key.Left:
                    case Key.Up:
                    case Key.Right:
                    case Key.Down:
                        ScrollInDirection(e.Key);
                        e.Handled = true;
                        return;
                }
            }
#else
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled && !this.TemplatedParentHandlesScrolling)
            {
                switch (e.Key)
                {
                    case VirtualKey.PageUp:
                    case VirtualKey.PageDown:
                    case VirtualKey.End:
                    case VirtualKey.Home:
                    case VirtualKey.Left:
                    case VirtualKey.Up:
                    case VirtualKey.Right:
                    case VirtualKey.Down:
                        return;
                }
            }
#endif


        }
#if !WinRT
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!e.Handled && base.Focus())
            {
                e.Handled = true;
            }
        }
#endif
#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.ElementScrollContentPresenter = base.GetTemplateChild("ScrollContentPresenter") as ScrollableContentPresenter;
            if (this.ElementScrollContentPresenter != null)
            {
                this.ElementScrollContentPresenter.ScrollOwner = this;
                if (this.ElementScrollContentPresenter != null)
                {
                    this.ScrollInfo = this.ElementScrollContentPresenter;
                }
            }
            this.ElementHorizontalScrollBar = base.GetTemplateChild("HorizontalScrollBar") as ScrollBar;
            if (this.ElementHorizontalScrollBar != null)
            {
                this.ElementHorizontalScrollBar.Scroll += delegate(object sender, ScrollEventArgs e)
                {
                    this.HandleScroll(Orientation.Horizontal, e);
                };
            }
            this.ElementVerticalScrollBar = base.GetTemplateChild("VerticalScrollBar") as ScrollBar;
            if (this.ElementVerticalScrollBar != null)
            {
                this.ElementVerticalScrollBar.Scroll += delegate(object sender, ScrollEventArgs e)
                {
                    this.HandleScroll(Orientation.Vertical, e);
                };
            }
        }
        #region Manipulation Events
#if WinRT

        private PanningInfo _panningInfo;

        private bool IsPastInertialLimit()
        {
            if (Math.Abs((int)(Environment.TickCount - this._panningInfo.InertiaBoundaryBeginTimestamp)) < 100)
            {
                return false;
            }
            if (!DoubleUtil.GreaterThanOrClose(Math.Abs(this._panningInfo.UnusedTranslation.X), 50.0))
            {
                return DoubleUtil.GreaterThanOrClose(Math.Abs(this._panningInfo.UnusedTranslation.Y), 50.0);
            }
            return true;
        }

        private void ManipulateScroll(ManipulationDeltaRoutedEventArgs e)
        {
            this.ManipulateScroll(e.Delta.Translation.X, e.Cumulative.Translation.X, true);
            this.ManipulateScroll(e.Delta.Translation.Y, e.Cumulative.Translation.Y, false);

            if (e.IsInertial && this.IsPastInertialLimit())
            {
                e.Complete();
            }
            else
            {
                double x = this._panningInfo.UnusedTranslation.X;
                if (!this._panningInfo.InHorizontalFeedback && DoubleUtil.LessThan(Math.Abs(x), 8.0))
                {
                    x = 0.0;
                }
                this._panningInfo.InHorizontalFeedback = !DoubleUtil.AreClose(x, 0.0);
                double y = this._panningInfo.UnusedTranslation.Y;
                if (!this._panningInfo.InVerticalFeedback && DoubleUtil.LessThan(Math.Abs(y), 5.0))
                {
                    y = 0.0;
                }
                this._panningInfo.InVerticalFeedback = !DoubleUtil.AreClose(y, 0.0);
                if (this._panningInfo.InHorizontalFeedback || this._panningInfo.InVerticalFeedback)
                {
                    if (e.IsInertial && (this._panningInfo.InertiaBoundaryBeginTimestamp == 0))
                    {
                        this._panningInfo.InertiaBoundaryBeginTimestamp = Environment.TickCount;
                    }
                }
            }
        }

        private void ManipulateScroll(double delta, double cumulativeTranslation, bool isHorizontal)
        {
            double num = isHorizontal ? this._panningInfo.UnusedTranslation.X : this._panningInfo.UnusedTranslation.Y;
            double num2 = isHorizontal ? this.HorizontalOffset : this.VerticalOffset;
            double num3 = isHorizontal ? this.ScrollableWidth : this.ScrollableHeight;
            if (DoubleUtil.AreClose(num3, 0.0))
            {
                num = 0.0;
                delta = 0.0;
            }
            else if ((DoubleUtil.GreaterThan(delta, 0.0) && DoubleUtil.AreClose(num2, 0.0)) || (DoubleUtil.LessThan(delta, 0.0) && DoubleUtil.AreClose(num2, num3)))
            {
                num += delta;
                delta = 0.0;
            }
            else if (DoubleUtil.LessThan(delta, 0.0) && DoubleUtil.GreaterThan(num, 0.0))
            {
                double num4 = Math.Max((double)(num + delta), (double)0.0);
                delta += num - num4;
                num = num4;
            }
            else if (DoubleUtil.GreaterThan(delta, 0.0) && DoubleUtil.LessThan(num, 0.0))
            {
                double num5 = Math.Min((double)(num + delta), (double)0.0);
                delta += num - num5;
                num = num5;
            }
            if (isHorizontal)
            {
                if (!DoubleUtil.AreClose(delta, 0.0))
                {
                    this.ScrollToHorizontalOffset(this._panningInfo.OriginalHorizontalOffset - Math.Round((double)((cumulativeTranslation) / this._panningInfo.DeltaPerHorizontalOffet)));
                }
                this._panningInfo.UnusedTranslation = new Point(num, this._panningInfo.UnusedTranslation.Y);
            }
            else
            {
                if (!DoubleUtil.AreClose(delta, 0.0))
                {
                    this.ScrollToVerticalOffset(this._panningInfo.OriginalVerticalOffset - Math.Round((double)((cumulativeTranslation) / this._panningInfo.DeltaPerVerticalOffset)));
                }
                this._panningInfo.UnusedTranslation = new Point(this._panningInfo.UnusedTranslation.X, num);
            }
        }
#endif
        public void ScrollToHorizontalOffset(double offset)
        {
            this.ElementScrollContentPresenter.SetHorizontalOffset(offset);
            this._xOffset = this.HorizontalOffset;
            this.HorizontalOffset = offset;
            if (this.ElementHorizontalScrollBar != null)
            {
                this.ElementHorizontalScrollBar.Value = offset;
            }
        }

        public void ScrollToVerticalOffset(double offset)
        {
            this.ElementScrollContentPresenter.SetVerticalOffset(offset);
            this._yOffset = this.VerticalOffset;
            this.VerticalOffset = offset;
            if (this.ElementVerticalScrollBar != null)
            {
                this.ElementVerticalScrollBar.Value = offset;
            }
        }
#if WinRT
        private bool CanStartScrollManipulation(Point translation, out bool cancelManipulation)
        {
            cancelManipulation = false;

            bool flag = DoubleUtil.GreaterThan(Math.Abs(translation.X), 3.0);
            bool flag2 = DoubleUtil.GreaterThan(Math.Abs(translation.Y), 3.0);
            if ((flag || flag2))
            {
                return true;
            }
            return false;
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            if (this._panningInfo != null)
            {
                if (!e.IsInertial && !this._panningInfo.IsPanning)
                {
                    e.Handled = true;
                    return;
                }
                this._panningInfo = null;
                e.Handled = true;
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            if (this._panningInfo != null)
            {
                bool cancelManipulation = false;
                if (this._panningInfo.IsPanning)
                {
                    this.ManipulateScroll(e);
                }
                else if (this.CanStartScrollManipulation(e.Cumulative.Translation, out cancelManipulation))
                {
                    this._panningInfo.IsPanning = true;
                    this.ManipulateScroll(e);
                }
                else if (cancelManipulation)
                {
                    this._panningInfo = null;
                    e.Handled = true;
                    return;
                }
                e.Handled = true;
            }
        }

        protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
        {
            this._panningInfo = null;
            e.Mode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            e.Container = this;
            PanningInfo info = new PanningInfo
            {
                OriginalHorizontalOffset = this.HorizontalOffset,
                OriginalVerticalOffset = this.VerticalOffset
            };
            this._panningInfo = info;
            double num = this.ViewportWidth + 1.0;
            double num2 = this.ViewportHeight + 1.0;
            if (this.ElementScrollContentPresenter != null)
            {
                this._panningInfo.DeltaPerHorizontalOffet = DoubleUtil.AreClose(num, 0.0) ? 0.0 : (this.ElementScrollContentPresenter.ActualWidth / num);
                this._panningInfo.DeltaPerVerticalOffset = DoubleUtil.AreClose(num2, 0.0) ? 0.0 : (this.ElementScrollContentPresenter.ActualHeight / num2);
            }
            else
            {
                this._panningInfo.DeltaPerHorizontalOffet = DoubleUtil.AreClose(num, 0.0) ? 0.0 : (base.ActualWidth / num);
                this._panningInfo.DeltaPerVerticalOffset = DoubleUtil.AreClose(num2, 0.0) ? 0.0 : (base.ActualHeight / num2);
            }
            e.Handled = true;
        }
#endif
        #endregion

#if SILVERLIGHT && SyncfusionFramework4_0
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                if (this.ScrollInfo != null)
                {
                    if (e.Delta < 0)
                    {
                        this.ScrollInfo.MouseWheelDown();
                    }
                    else
                    {
                        this.ScrollInfo.MouseWheelUp();
                    }
                }
                e.Handled = true;
            }
        }
#endif
#if WinRT
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                if (this.ScrollInfo != null)
                {
                    if (e.GetCurrentPoint(null).Properties.MouseWheelDelta < 0)
                    {
                        this.ScrollInfo.MouseWheelDown();
                    }
                    else
                    {
                        this.ScrollInfo.MouseWheelUp();
                    }
                }
                e.Handled = true;
            }
        }
        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
            if (!e.Handled)
            {
                if (this.ScrollInfo != null)
                {
                    if (e.Delta.Scale < 0)
                    {
                        this.ScrollInfo.MouseWheelDown();
                    }
                    else
                    {
                        this.ScrollInfo.MouseWheelUp();
                    }
                }
                e.Handled = true;
            }
        }
#endif
        private static void OnScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollableContentViewer viewer = d as ScrollableContentViewer;
            if (viewer != null)
            {
                if (viewer._scrollInfo != null)
                {
                    viewer._scrollInfo.CanHorizontallyScroll = viewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                    viewer._scrollInfo.CanVerticallyScroll = viewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                }
                viewer.InvalidateMeasure();
            }
            else
            {
            }
        }

        internal void PageDown()
        {
#if!WinRT
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs(ScrollEventType.LargeIncrement, 0.0));
#else
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs());
#endif
        }

        internal void PageEnd()
        {
#if!WinRT
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs(ScrollEventType.Last, 0.0));
#else
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs());
#endif
        }

        internal void PageHome()
        {
#if!WinRT
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs(ScrollEventType.First, 0.0));
#else
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs());
#endif
        }

        internal void PageLeft()
        {
#if!WinRT
            this.HandleScroll(Orientation.Horizontal, new ScrollEventArgs(ScrollEventType.LargeDecrement, 0.0));
#else
            this.HandleScroll(Orientation.Horizontal, new ScrollEventArgs());
#endif
        }

        internal void PageRight()
        {
#if!WinRT
            this.HandleScroll(Orientation.Horizontal, new ScrollEventArgs(ScrollEventType.LargeIncrement, 0.0));
#else
            this.HandleScroll(Orientation.Horizontal, new ScrollEventArgs());
#endif
        }

        internal void PageUp()
        {
#if!WinRT
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs(ScrollEventType.LargeDecrement, 0.0));
#else
            this.HandleScroll(Orientation.Vertical, new ScrollEventArgs());
#endif
        }
#if!WinRT
        internal void ScrollInDirection(Key key)
#else
        internal void ScrollInDirection(VirtualKey key)
#endif
        {
            bool ctrl = false;
#if !WinRT
            ctrl = (Keyboard.Modifiers & ModifierKeys.Control) != 0;
#else
            ctrl = ((Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control)) == CoreVirtualKeyStates.Down);
#endif
            switch (key)
            {
#if !WinRT
                case Key.PageUp:
#else
                case VirtualKey.PageUp:
#endif
                    if (ctrl)
                    {
                        if (ElementScrollContentPresenter != null)
                            ElementScrollContentPresenter.PageLeft();
                        else
                            this.PageLeft();
                    }
                    else
                    {
                        if (ElementScrollContentPresenter != null)
                            ElementScrollContentPresenter.PageUp();
                        else
                            this.PageUp();
                    }
                    return;
#if !WinRT
                case Key.PageDown:
#else
                case VirtualKey.PageDown:
#endif
                    if (ctrl)
                    {
                        if (ElementScrollContentPresenter != null)
                            ElementScrollContentPresenter.PageRight();
                        else
                            this.PageRight();
                    }
                    else
                    {
                        if (ElementScrollContentPresenter != null)
                            ElementScrollContentPresenter.PageDown();
                        else
                            this.PageDown();
                    }
                    return;
#if !WinRT
                case Key.End:
#else
                case VirtualKey.End:
#endif
                    this.PageEnd();
                    return;
#if !WinRT
                case Key.Home:
#else
                case VirtualKey.Home:
#endif
                    this.PageHome();
                    return;
#if !WinRT
                case Key.Left:
#else
                case VirtualKey.Left:
#endif
                    if (ElementScrollContentPresenter != null)
                        ElementScrollContentPresenter.LineLeft();
                    else
                        this.LineLeft();
                    return;
#if !WinRT
                case Key.Up:
#else
                case VirtualKey.Up:
#endif
                    if (ElementScrollContentPresenter != null)
                        ElementScrollContentPresenter.LineUp();
                    else
                        this.LineUp();
                    return;
#if !WinRT
                case Key.Right:
#else
                case VirtualKey.Right:
#endif
                    if (ElementScrollContentPresenter != null)
                        ElementScrollContentPresenter.LineRight();
                    else
                        this.LineRight();
                    return;
#if !WinRT
                case Key.Down:
#else
                case VirtualKey.Down:
#endif
                    if (ElementScrollContentPresenter != null)
                        ElementScrollContentPresenter.LineDown();
                    else
                        this.LineDown();
                    return;
            }
        }

        internal void ApplyLayoutTransform()
        {
#if !WinRT
            this.ElementScrollContentPresenter.ApplyLayoutTransform();
#endif
        }
        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get
            {
                return (Visibility)base.GetValue(ComputedHorizontalScrollBarVisibilityProperty);
            }
            internal set
            {
                base.SetValue(ComputedHorizontalScrollBarVisibilityProperty, value);
            }
        }

        public Visibility ComputedVerticalScrollBarVisibility
        {
            get
            {
                return (Visibility)base.GetValue(ComputedVerticalScrollBarVisibilityProperty);
            }
            internal set
            {
                base.SetValue(ComputedVerticalScrollBarVisibilityProperty, value);
            }
        }

        internal ScrollBar ElementHorizontalScrollBar { get; set; }
        internal ScrollableContentPresenter ElementScrollContentPresenter { get; set; }
        internal ScrollBar ElementVerticalScrollBar { get; set; }
        internal IScrollableInfo ScrollableChild
        {
            get
            {
                if (!GetCanContentScroll(this) || ElementScrollContentPresenter == null)
                    return null;
                return ElementScrollContentPresenter.ScrollableChild;
            }
        }

        public double ExtentHeight
        {
            get
            {
                return this._yExtent;
            }
            internal set
            {
                base.SetValue(ExtentHeightProperty, value);
            }
        }

        public double ExtentWidth
        {
            get
            {
                return this._xExtent;
            }
            internal set
            {
                base.SetValue(ExtentWidthProperty, value);
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return this._xOffset;
            }
            internal set
            {
                base.SetValue(HorizontalOffsetProperty, value);
            }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)base.GetValue(HorizontalScrollBarVisibilityProperty);
            }
            set
            {
                base.SetValue(HorizontalScrollBarVisibilityProperty, (Enum)value);
            }
        }

        public double ScrollableHeight
        {
            get
            {
                return Math.Max((double)0.0, (double)(this.ExtentHeight - this.ViewportHeight));
            }
            internal set
            {
                base.SetValue(ScrollableHeightProperty, value);
            }
        }

        public double ScrollableWidth
        {
            get
            {
                return Math.Max((double)0.0, (double)(this.ExtentWidth - this.ViewportWidth));
            }
            internal set
            {
                base.SetValue(ScrollableWidthProperty, value);
            }
        }

        internal IScrollableInfo ScrollInfo
        {
            get
            {
                return this._scrollInfo;
            }
            set
            {
                this._scrollInfo = value;
                if (this._scrollInfo != null)
                {
                    this._scrollInfo.CanHorizontallyScroll = this.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                    this._scrollInfo.CanVerticallyScroll = this.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                }
            }
        }

        internal bool TemplatedParentHandlesScrolling
        {
            get
            {
                return this._templatedParentHandlesScrolling;
            }
            set
            {
                this._templatedParentHandlesScrolling = value;
                base.IsTabStop = !this._templatedParentHandlesScrolling;
            }
        }

        /// <summary>
        /// This property indicates whether the Content should handle scrolling if it can.
        /// A true value indicates Content should be allowed to scroll if it supports IScrollInfo. 
        /// A false value will always use the default physically scrolling handler.
        /// </summary> 
        public bool CanContentScroll
        {
            get { return (bool)GetValue(CanContentScrollProperty); }
            set { SetValue(CanContentScrollProperty, value); }
        }

        public double VerticalOffset
        {
            get
            {
                return this._yOffset;
            }
            internal set
            {
                base.SetValue(VerticalOffsetProperty, value);
            }
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)base.GetValue(VerticalScrollBarVisibilityProperty);
            }
            set
            {
                base.SetValue(VerticalScrollBarVisibilityProperty, (Enum)value);
            }
        }

        public double ViewportHeight
        {
            get
            {
                return this._yViewport;
            }
            internal set
            {
                base.SetValue(ViewportHeightProperty, value);
            }
        }

        public double ViewportWidth
        {
            get
            {
                return this._xViewport;
            }
            internal set
            {
                base.SetValue(ViewportWidthProperty, value);
            }
        }

        internal delegate void ScrollChangedDelegate(double xOffset, double yOffset);

        public void Dispose()
        {
            this._scrollInfo = null;
            this.ElementHorizontalScrollBar = null;
            if (this.ElementScrollContentPresenter != null)
            {
                this.ElementScrollContentPresenter.Dispose();
                this.ElementScrollContentPresenter = null;
            }
            this.ElementVerticalScrollBar = null;
        }
    }
}

