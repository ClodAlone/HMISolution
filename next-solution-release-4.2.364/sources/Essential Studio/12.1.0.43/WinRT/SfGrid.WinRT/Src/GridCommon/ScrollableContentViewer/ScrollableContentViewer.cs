#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
#if !WP
using Syncfusion.UI.Xaml.Controls.DataPager;
#endif
using Syncfusion.UI.Xaml.Grid;
#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
#endif
using Syncfusion.UI.Xaml.Utility;
#if WinRT
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

#else
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [TemplatePart(Name = "PART_ScrollContentPresenter", Type = typeof(ScrollableContentPresenter))]
    [TemplatePart(Name = "PART_HorizontalScrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "PART_VerticalScrollBar", Type = typeof(ScrollBar))]
    [ClassReference(IsReviewed = false)]
    public class ScrollableContentViewer : ContentControl, IDisposable
    {
        #region Properties

        private bool _inChildInvalidateMeasure;
        private bool _inMeasure;
        private const double PreFeedbackTranslationX = 50d;
        private const double PreFeedbackTranslationY = 50d;
        internal const double _mouseWheelDelta = 48.0;
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

        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("ComputedHorizontalScrollBarVisibility", typeof (Visibility),
                                        typeof (ScrollableContentViewer), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("ComputedVerticalScrollBarVisibility", typeof (Visibility),
                                        typeof (ScrollableContentViewer), new PropertyMetadata(Visibility.Collapsed));

        private const string ElementHorizontalScrollBarName = "PART_HorizontalScrollBar";
        private const string ElementScrollContentPresenterName = "PART_ScrollContentPresenter";
        private const string ElementVerticalScrollBarName = "PART_VerticalScrollBar";
        private DispatcherTimer _timer = new DispatcherTimer();
        public TimeSpan TimeOut { get; private set; }
        public DateTime LastMove { get; private set; }

        public static readonly DependencyProperty ExtentHeightProperty = DependencyProperty.Register("ExtentHeight",
                                                                                                     typeof (double),
                                                                                                     typeof (
                                                                                                         ScrollableContentViewer
                                                                                                         ), null);

        public static readonly DependencyProperty ExtentWidthProperty = DependencyProperty.Register("ExtentWidth",
                                                                                                    typeof (double),
                                                                                                    typeof (
                                                                                                        ScrollableContentViewer
                                                                                                        ), null);

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof (double), typeof (ScrollableContentViewer), null);

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.RegisterAttached("HorizontalScrollBarVisibility", typeof (ScrollBarVisibility),
                                                typeof (ScrollableContentViewer),
                                                new PropertyMetadata(ScrollBarVisibility.Auto,
                                                                     new PropertyChangedCallback(
                                                                         ScrollableContentViewer
                                                                             .OnScrollBarVisibilityChanged)));

        public static readonly DependencyProperty ScrollableHeightProperty =
            DependencyProperty.Register("ScrollableHeight", typeof (double), typeof (ScrollableContentViewer), null);

        public static readonly DependencyProperty ScrollableWidthProperty =
            DependencyProperty.Register("ScrollableWidth", typeof (double), typeof (ScrollableContentViewer), null);

        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
            "VerticalOffset", typeof (double), typeof (ScrollableContentViewer), null);

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBarVisibility", typeof (ScrollBarVisibility),
                                                typeof (ScrollableContentViewer),
                                                new PropertyMetadata(ScrollBarVisibility.Auto,
                                                                     new PropertyChangedCallback(
                                                                         ScrollableContentViewer
                                                                             .OnScrollBarVisibilityChanged)));

        public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register(
            "ViewportHeight", typeof (double), typeof (ScrollableContentViewer), null);

        public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.Register("ViewportWidth",
                                                                                                      typeof (double),
                                                                                                      typeof (
                                                                                                          ScrollableContentViewer
                                                                                                          ), null);


        public DependencyObject Owner
        {
            get { return (DependencyObject) GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }

        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof (DependencyObject), typeof (ScrollableContentViewer),
                                        new PropertyMetadata(null));


        internal event ScrollChangedDelegate ScrollChanged;

        public static void SetHorizontalScrollBarVisibility(DependencyObject element,
                                                            ScrollBarVisibility horizontalScrollBarVisibility)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(HorizontalScrollBarVisibilityProperty, (Enum) horizontalScrollBarVisibility);
        }

        public static void SetVerticalScrollBarVisibility(DependencyObject element,
                                                          ScrollBarVisibility verticalScrollBarVisibility)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(VerticalScrollBarVisibilityProperty, (Enum) verticalScrollBarVisibility);
        }

        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get { return (Visibility) base.GetValue(ComputedHorizontalScrollBarVisibilityProperty); }
            internal set { base.SetValue(ComputedHorizontalScrollBarVisibilityProperty, value); }
        }

        public Visibility ComputedVerticalScrollBarVisibility
        {
            get { return (Visibility) base.GetValue(ComputedVerticalScrollBarVisibilityProperty); }
            internal set { base.SetValue(ComputedVerticalScrollBarVisibilityProperty, value); }
        }

        internal ScrollBar ElementHorizontalScrollBar { get; set; }

        internal ScrollableContentPresenter ElementScrollContentPresenter { get; set; }

        internal ScrollBar ElementVerticalScrollBar { get; set; }

        public double ExtentHeight
        {
            get { return this._yExtent; }
            internal set
            {
                this._yExtent = value;
                base.SetValue(ExtentHeightProperty, value);
            }
        }

        public double ExtentWidth
        {
            get { return this._xExtent; }
            internal set
            {
                this._xExtent = value;
                base.SetValue(ExtentWidthProperty, value);
            }
        }

        public double HorizontalOffset
        {
            get { return this._xOffset; }
            internal set
            {
                this._xOffset = value;
                base.SetValue(HorizontalOffsetProperty, value);
            }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility) base.GetValue(HorizontalScrollBarVisibilityProperty); }
            set { base.SetValue(HorizontalScrollBarVisibilityProperty, (Enum) value); }
        }

        internal bool InChildInvalidateMeasure
        {
            get { return this._inChildInvalidateMeasure; }
            set { this._inChildInvalidateMeasure = value; }
        }

        public double ScrollableHeight
        {
            get { return Math.Max((double) 0.0, (double) (this.ExtentHeight - this.ViewportHeight)); }
            internal set { base.SetValue(ScrollableHeightProperty, value); }
        }

        public double ScrollableWidth
        {
            get { return Math.Max((double) 0.0, (double) (this.ExtentWidth - this.ViewportWidth)); }
            internal set { base.SetValue(ScrollableWidthProperty, value); }
        }

        internal IScrollableInfo ScrollInfo
        {
            get { return this._scrollInfo; }
            set
            {
                this._scrollInfo = value;
                if (this._scrollInfo != null)
                {
                    this._scrollInfo.CanHorizontallyScroll = this.HorizontalScrollBarVisibility !=
                                                             ScrollBarVisibility.Disabled;
                    this._scrollInfo.CanVerticallyScroll = this.VerticalScrollBarVisibility !=
                                                           ScrollBarVisibility.Disabled;
                }
            }
        }

        internal bool TemplatedParentHandlesMouseButton { get; set; }

        internal bool TemplatedParentHandlesScrolling
        {
            get { return this._templatedParentHandlesScrolling; }
            set
            {
                this._templatedParentHandlesScrolling = value;
                base.IsTabStop = !this._templatedParentHandlesScrolling;
            }
        }

        public double VerticalOffset
        {
            get { return this._yOffset; }
            internal set
            {
                this._yOffset = value;
                base.SetValue(VerticalOffsetProperty, value);
            }
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility) base.GetValue(VerticalScrollBarVisibilityProperty); }
            set { base.SetValue(VerticalScrollBarVisibilityProperty, (Enum) value); }
        }

        public double ViewportHeight
        {
            get { return this._yViewport; }
            internal set
            {
                this._yViewport = value;
                base.SetValue(ViewportHeightProperty, value);
            }
        }

        public double ViewportWidth
        {
            get { return this._xViewport; }
            internal set
            {
                this._xViewport = value;
                base.SetValue(ViewportWidthProperty, value);
            }
        }

        internal delegate void ScrollChangedDelegate(double xOffset, double yOffset);

        #endregion

        #region Ctor

        public ScrollableContentViewer()
        {
            WireEvents();
            base.DefaultStyleKey = typeof (ScrollableContentViewer);
            this.Owner = this;
        }

        #endregion

        #region public methods

        public static ScrollBarVisibility GetHorizontalScrollBarVisibility(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ScrollBarVisibility) element.GetValue(HorizontalScrollBarVisibilityProperty);
        }

        public static ScrollBarVisibility GetVerticalScrollBarVisibility(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ScrollBarVisibility) element.GetValue(VerticalScrollBarVisibilityProperty);
        }

        public void InvalidateScrollInfo()
        {
            if (this.ScrollInfo != null)
            {
                if (!this._inMeasure)
                {
                    double num = this.ScrollInfo.ExtentWidth;
                    double num2 = this.ScrollInfo.ViewportWidth;
                    if ((this.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto) &&
                        (((this._scrollVisibilityX == Visibility.Collapsed) && (num > num2)) ||
                         ((this._scrollVisibilityX == Visibility.Visible) && (num < num2))))
                    {
                        base.InvalidateMeasure();
                    }
                    else
                    {
                        num = this.ScrollInfo.ExtentHeight;
                        num2 = this.ScrollInfo.ViewportHeight;
                        if ((this.VerticalScrollBarVisibility == ScrollBarVisibility.Auto) &&
                            (((this._scrollVisibilityY == Visibility.Collapsed) && (num > num2)) ||
                             ((this._scrollVisibilityY == Visibility.Visible) && (num < num2))))
                        {
                            base.InvalidateMeasure();
                        }
                    }
                }
                double horizontalOffset = this.ScrollInfo.HorizontalOffset;
                double verticalOffset = this.ScrollInfo.VerticalOffset;
                double viewportWidth = this.ScrollInfo.ViewportWidth;
                double viewportHeight = this.ScrollInfo.ViewportHeight;
                double extentWidth = this.ScrollInfo.ExtentWidth;
                double extentHeight = this.ScrollInfo.ExtentHeight;
                if (((!DoubleUtil.AreClose(this._xOffset, horizontalOffset) ||
                      !DoubleUtil.AreClose(this._yOffset, verticalOffset)) ||
                     (!DoubleUtil.AreClose(this._xViewport, viewportWidth) ||
                      !DoubleUtil.AreClose(this._yViewport, viewportHeight))) ||
                    (!DoubleUtil.AreClose(this._xExtent, extentWidth) ||
                     !DoubleUtil.AreClose(this._yExtent, extentHeight)))
                {
                    double num9 = this._xOffset;
                    double num10 = this._yOffset;
                    double num11 = this._xViewport;
                    double num12 = this._yViewport;
                    double num13 = this._xExtent;
                    double num14 = this._yExtent;
                    double scrollableWidth = this.ScrollableWidth;
                    double scrollableHeight = this.ScrollableHeight;
                    bool flag = false;
                    try
                    {
                        if (!DoubleUtil.AreClose(num9, horizontalOffset))
                        {
                            this.HorizontalOffset = horizontalOffset;
                            flag = true;
                        }
                        if (!DoubleUtil.AreClose(num10, verticalOffset))
                        {
                            this.VerticalOffset = verticalOffset;
                            flag = true;
                        }
                        if (!DoubleUtil.AreClose(num11, viewportWidth))
                        {
                            this.ViewportWidth = viewportWidth;
                            flag = true;
                        }
                        if (!DoubleUtil.AreClose(num12, viewportHeight))
                        {
                            this.ViewportHeight = viewportHeight;
                            flag = true;
                        }
                        if (!DoubleUtil.AreClose(num13, extentWidth))
                        {
                            this.ExtentWidth = extentWidth;
                            flag = true;
                        }
                        if (!DoubleUtil.AreClose(num14, extentHeight))
                        {
                            this.ExtentHeight = extentHeight;
                            flag = true;
                        }
                        double num17 = this.ScrollableWidth;
                        if (!DoubleUtil.AreClose(scrollableWidth, num17))
                        {
                            this.ScrollableWidth = num17;
                            flag = true;
                        }
                        double num18 = this.ScrollableHeight;
                        if (!DoubleUtil.AreClose(scrollableHeight, num18))
                        {
                            this.ScrollableHeight = num18;
                            flag = true;
                        }
                    }
                    finally
                    {
                        if (flag)
                        {
                            if ((!DoubleUtil.AreClose(num9, this._xOffset) && (this.ElementHorizontalScrollBar != null)))
                            {
                                this.ElementHorizontalScrollBar.Value = this._xOffset;
                            }
                            if ((!DoubleUtil.AreClose(num10, this._yOffset) && (this.ElementVerticalScrollBar != null)))
                            {
                                this.ElementVerticalScrollBar.Value = this._yOffset;
                            }
                            if (this.ScrollChanged != null)
                            {
                                this.ScrollChanged(this.HorizontalOffset, this.VerticalOffset);
                            }
                        }
                    }
                }
            }
        }

        public void ScrollToHorizontalOffset(double offset)
        {
            this.HandleHorizontalScroll(new ScrollableEventArgs(ScrollEventType.ThumbPosition, offset));
        }

        public void ScrollToVerticalOffset(double offset)
        {
            this.HandleVerticalScroll(new ScrollableEventArgs(ScrollEventType.ThumbPosition, offset));
        }

        #endregion

        #region internal methods

#if WinRT
        internal void HandleKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled && !this.TemplatedParentHandlesScrolling)
            {
                bool flag =
                    Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
                var vertical = Orientation.Vertical;
                var thumbTrack = ScrollEventType.ThumbTrack;
                bool flag2 = base.FlowDirection == FlowDirection.RightToLeft;
                switch (e.Key)
                {
                    case VirtualKey.PageUp:
                        thumbTrack = ScrollEventType.LargeDecrement;
                        break;

                    case VirtualKey.PageDown:
                        thumbTrack = ScrollEventType.LargeIncrement;
                        break;

                    case VirtualKey.End:
                        if (!flag)
                        {
                            vertical = Orientation.Horizontal;
                        }
                        thumbTrack = ScrollEventType.Last;
                        break;

                    case VirtualKey.Home:
                        if (!flag)
                        {
                            vertical = Orientation.Horizontal;
                        }
                        thumbTrack = ScrollEventType.First;
                        break;

                    case VirtualKey.Left:
                        vertical = Orientation.Horizontal;
                        if (!flag2)
                        {
                            thumbTrack = ScrollEventType.SmallDecrement;
                            break;
                        }
                        thumbTrack = ScrollEventType.SmallIncrement;
                        break;

                    case VirtualKey.Up:
                        thumbTrack = ScrollEventType.SmallDecrement;
                        break;

                    case VirtualKey.Right:
                        vertical = Orientation.Horizontal;
                        if (!flag2)
                        {
                            thumbTrack = ScrollEventType.SmallIncrement;
                            break;
                        }
                        thumbTrack = ScrollEventType.SmallDecrement;
                        break;

                    case VirtualKey.Down:
                        thumbTrack = ScrollEventType.SmallIncrement;
                        break;
                }
                if (ScrollEventType.ThumbTrack != thumbTrack)
                {
                    this.HandleScroll(vertical, new ScrollableEventArgs(thumbTrack, 0.0));
                    e.Handled = true;
                }
            }
        }
#else
        internal void HandleKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled && !this.TemplatedParentHandlesScrolling)
            {
#if WPF
                bool flag = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
#else
                bool flag = ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
#endif
                var vertical = Orientation.Vertical;
                var thumbTrack = ScrollEventType.ThumbTrack;
                bool flag2 = base.FlowDirection == FlowDirection.RightToLeft;
                switch (e.Key)
                {
                    case Key.PageUp:
                        thumbTrack = ScrollEventType.LargeDecrement;
                        break;

                    case Key.PageDown:
                        thumbTrack = ScrollEventType.LargeIncrement;
                        break;

                    case Key.End:
                        if (!flag)
                        {
                            vertical = Orientation.Horizontal;
                        }
                        thumbTrack = ScrollEventType.Last;
                        break;

                    case Key.Home:
                        if (!flag)
                        {
                            vertical = Orientation.Horizontal;
                        }
                        thumbTrack = ScrollEventType.First;
                        break;

                    case Key.Left:
                        vertical = Orientation.Horizontal;
                        if (!flag2)
                        {
                            thumbTrack = ScrollEventType.SmallDecrement;
                            break;
                        }
                        thumbTrack = ScrollEventType.SmallIncrement;
                        break;

                    case Key.Up:
                        thumbTrack = ScrollEventType.SmallDecrement;
                        break;

                    case Key.Right:
                        vertical = Orientation.Horizontal;
                        if (!flag2)
                        {
                            thumbTrack = ScrollEventType.SmallIncrement;
                            break;
                        }
                        thumbTrack = ScrollEventType.SmallDecrement;
                        break;

                    case Key.Down:
                        thumbTrack = ScrollEventType.SmallIncrement;
                        break;
                }
                if (ScrollEventType.ThumbTrack != thumbTrack)
                {
                    this.HandleScroll(vertical, new ScrollableEventArgs(thumbTrack, 0.0));
                    e.Handled = true;
                }
            }
        }
#endif


        internal void LineDown()
        {
            this.HandleVerticalScroll(new ScrollableEventArgs(ScrollEventType.SmallIncrement, 0.0));
        }

        internal void LineLeft()
        {
            this.HandleHorizontalScroll(new ScrollableEventArgs(ScrollEventType.SmallDecrement, 0.0));
        }

        internal void LineRight()
        {
            this.HandleHorizontalScroll(new ScrollableEventArgs(ScrollEventType.SmallIncrement, 0.0));
        }

        internal void LineUp()
        {
            this.HandleVerticalScroll(new ScrollableEventArgs(ScrollEventType.SmallDecrement, 0.0));
        }

        internal void PageDown()
        {
            this.HandleVerticalScroll(new ScrollableEventArgs(ScrollEventType.LargeIncrement, 0.0));
        }

        internal void PageEnd()
        {
            this.HandleHorizontalScroll(new ScrollableEventArgs(ScrollEventType.Last, 0.0));
        }

        internal void PageHome()
        {
            this.HandleHorizontalScroll(new ScrollableEventArgs(ScrollEventType.First, 0.0));
        }

        internal void PageLeft()
        {
            this.HandleHorizontalScroll(new ScrollableEventArgs(ScrollEventType.LargeDecrement, 0.0));
        }

        internal void PageRight()
        {
            this.HandleHorizontalScroll(new ScrollableEventArgs(ScrollEventType.LargeIncrement, 0.0));
        }

        internal void PageUp()
        {
            this.HandleVerticalScroll(new ScrollableEventArgs(ScrollEventType.LargeDecrement, 0.0));
        }

#if WinRT
        internal void ScrollInDirection(VirtualKey key)
        {
            bool flag = base.FlowDirection == FlowDirection.RightToLeft;
            switch (key)
            {
                case VirtualKey.PageUp:
                    this.PageUp();
                    return;

                case VirtualKey.PageDown:
                    this.PageDown();
                    return;

                case VirtualKey.End:
                    this.PageEnd();
                    return;

                case VirtualKey.Home:
                    this.PageHome();
                    return;

                case VirtualKey.Left:
                    if (!flag)
                    {
                        this.LineLeft();
                        return;
                    }
                    this.LineRight();
                    return;

                case VirtualKey.Up:
                    this.LineUp();
                    return;

                case VirtualKey.Right:
                    if (!flag)
                    {
                        this.LineRight();
                        return;
                    }
                    this.LineLeft();
                    return;

                case VirtualKey.Down:
                    this.LineDown();
                    return;
            }
        }
#else
        internal void ScrollInDirection(Key key)
        {
            bool flag = base.FlowDirection == FlowDirection.RightToLeft;
            switch (key)
            {
                case Key.PageUp:
                    this.PageUp();
                    return;

                case Key.PageDown:
                    this.PageDown();
                    return;

                case Key.End:
                    this.PageEnd();
                    return;

                case Key.Home:
                    this.PageHome();
                    return;

                case Key.Left:
                    if (!flag)
                    {
                        this.LineLeft();
                        return;
                    }
                    this.LineRight();
                    return;

                case Key.Up:
                    this.LineUp();
                    return;

                case Key.Right:
                    if (!flag)
                    {
                        this.LineRight();
                        return;
                    }
                    this.LineLeft();
                    return;

                case Key.Down:
                    this.LineDown();
                    return;
            }
        }
#endif

        #endregion

        #region override methods

        #region OnApplyTemplate

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            ScrollEventHandler handler = null;
            ScrollEventHandler handler2 = null;
            base.OnApplyTemplate();
            this.ElementScrollContentPresenter =
                base.GetTemplateChild("PART_ScrollContentPresenter") as ScrollableContentPresenter;
            this.ElementHorizontalScrollBar = base.GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;
            if (this.ElementHorizontalScrollBar != null)
            {
                if (handler == null)
                {
                    handler = (sender, e) => this.HandleHorizontalScroll(e);
                }
                this.ElementHorizontalScrollBar.Scroll += handler;
            }
            this.ElementVerticalScrollBar = base.GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
            if (this.ElementVerticalScrollBar != null)
            {
                if (handler2 == null)
                {
                    handler2 = (sender, e) => this.HandleVerticalScroll(e);
                }
                this.ElementVerticalScrollBar.Scroll += handler2;
            }
        }

        #endregion

        #region MeasureOverride

        protected override Size MeasureOverride(Size availableSize)
        {
            this._inChildInvalidateMeasure = false;
            UIElement element = (VisualTreeHelper.GetChildrenCount(this) == 0)
                                    ? null
                                    : (VisualTreeHelper.GetChild(this, 0) as UIElement);
            if (element == null)
            {
                return new Size();
            }
            IScrollableInfo scrollInfo = this.ScrollInfo;
            ScrollBarVisibility verticalScrollBarVisibility = this.VerticalScrollBarVisibility;
            ScrollBarVisibility horizontalScrollBarVisibility = this.HorizontalScrollBarVisibility;
            bool flag = verticalScrollBarVisibility == ScrollBarVisibility.Auto;
            bool flag2 = horizontalScrollBarVisibility == ScrollBarVisibility.Auto;
            Visibility visibility3 = (verticalScrollBarVisibility == ScrollBarVisibility.Visible)
                                         ? Visibility.Visible
                                         : Visibility.Collapsed;
            Visibility visibility4 = (horizontalScrollBarVisibility == ScrollBarVisibility.Visible)
                                         ? Visibility.Visible
                                         : Visibility.Collapsed;
            try
            {
                this._inMeasure = true;
                if (this._scrollVisibilityY != visibility3)
                {
                    this._scrollVisibilityY = visibility3;
                    this.ComputedVerticalScrollBarVisibility = this._scrollVisibilityY;
                }
                if (this._scrollVisibilityX != visibility4)
                {
                    this._scrollVisibilityX = visibility4;
                    this.ComputedHorizontalScrollBarVisibility = this._scrollVisibilityX;
                }
                if (scrollInfo != null)
                {
                    scrollInfo.CanHorizontallyScroll = horizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                    scrollInfo.CanVerticallyScroll = verticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                }
                element.Measure(availableSize);
                scrollInfo = this.ScrollInfo;
                if ((scrollInfo != null) && (flag || flag2))
                {
                    bool flag3 = flag2 && (scrollInfo.ExtentWidth > scrollInfo.ViewportWidth);
                    bool flag4 = flag && (scrollInfo.ExtentHeight > scrollInfo.ViewportHeight);
                    if (flag3 && (this._scrollVisibilityX != Visibility.Visible))
                    {
                        this._scrollVisibilityX = Visibility.Visible;
                        this.ComputedHorizontalScrollBarVisibility = this._scrollVisibilityX;
                    }
                    if (flag4 && (this._scrollVisibilityY != Visibility.Visible))
                    {
                        this._scrollVisibilityY = Visibility.Visible;
                        this.ComputedVerticalScrollBarVisibility = this._scrollVisibilityY;
                    }
                    if (flag3 || flag4)
                    {
                        this._inChildInvalidateMeasure = true;
                        element.InvalidateMeasure();
                        element.Measure(availableSize);
                    }
                    if ((flag2 && flag) && (flag3 != flag4))
                    {
                        bool flag5 = !flag3 && (scrollInfo.ExtentWidth > scrollInfo.ViewportWidth);
                        bool flag6 = !flag4 && (scrollInfo.ExtentHeight > scrollInfo.ViewportHeight);
                        if (flag5)
                        {
                            if (this._scrollVisibilityX != Visibility.Visible)
                            {
                                this._scrollVisibilityX = Visibility.Visible;
                                this.ComputedHorizontalScrollBarVisibility = this._scrollVisibilityX;
                            }
                        }
                        else if (flag6 && (this._scrollVisibilityY != Visibility.Visible))
                        {
                            this._scrollVisibilityY = Visibility.Visible;
                            this.ComputedVerticalScrollBarVisibility = this._scrollVisibilityY;
                        }
                        if (flag5 || flag6)
                        {
                            this._inChildInvalidateMeasure = true;
                            element.InvalidateMeasure();
                            element.Measure(availableSize);
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

        #endregion

#if WinRT

        #region Key/Pointer Events

        #region OnKeyDown

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            LastMove = DateTime.Now;
            //this.HandleKeyDown(e);
        }

        #endregion

        #region OnPointerPressed

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if ((!e.Handled && !this.TemplatedParentHandlesMouseButton) && base.Focus(FocusState.Pointer))
            {
                e.Handled = true;
            }
        }

        #endregion

        #region OnPointerMoved

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                VisualStateManager.GoToState(this, "MouseIndicator", true);
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
                VisualStateManager.GoToState(this, "TouchIndicator", true);
            LastMove = DateTime.Now;
        }

        #endregion

        #region OnPointerReleased

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            e.Handled = true;
        }

        #endregion

        #endregion

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
                double y = this._panningInfo.UnusedTranslation.Y;
                if (!this._panningInfo.InVerticalFeedback && DoubleUtil.LessThan(Math.Abs(y), PreFeedbackTranslationY))
                {
                    y = 0.0;
                }
                this._panningInfo.InVerticalFeedback = !DoubleUtil.AreClose(y, 0.0);
                this.ScrollInfo.VerticalPadding = y;
                double x = this._panningInfo.UnusedTranslation.X;
                if (!this._panningInfo.InHorizontalFeedback && DoubleUtil.LessThan(Math.Abs(x), PreFeedbackTranslationX))
                {
                    x = 0.0;
                }
                this._panningInfo.InHorizontalFeedback = !DoubleUtil.AreClose(x, 0.0);
                this.ScrollInfo.HorizontalPadding = x;
                if (x != 0 || y != 0)
                {
                    this.ScrollInfo.SetHorizontalOffset(this.HorizontalOffset);
                }
            }
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            LastMove = DateTime.Now;
            if (this._panningInfo != null)
            {
                if (!e.IsInertial && !this._panningInfo.IsPanning)
                {
                    e.Handled = true;
                    return;
                }
                if (this.ElementScrollContentPresenter.Content is NumericButtonPanel)
                {
                    var isHorizontal = (ElementScrollContentPresenter.Content as NumericButtonPanel).DataPager.Orientation == Orientation.Horizontal;
                    ManipulationPagerAnimation(isHorizontal);
                }
                else
                    ManipulationAnimation();
                this._panningInfo = null;
                e.Handled = true;
            }
        }

        /// <summary>
        /// This Method is used for Translation When the User Pulls the Grid along the Edges
        /// </summary>
        private async void ManipulationAnimation()
        {
            double animationRatio = Math.Abs(this.ScrollInfo.VerticalPadding)/13;
            animationRatio = animationRatio == 0 ? Math.Abs(this.ScrollInfo.HorizontalPadding)/13 : animationRatio;
            if ((this.ScrollInfo.VerticalPadding >= 0) && (this.ScrollInfo.HorizontalPadding >= 0))
            {
                while ((this.ScrollInfo.VerticalPadding > 0) || (this.ScrollInfo.HorizontalPadding > 0))
                {
                    await Task.Delay(1);
                    this.ScrollInfo.VerticalPadding = Math.Round(this.ScrollInfo.VerticalPadding) < 0
                                                          ? 0
                                                          : this.ScrollInfo.VerticalPadding - animationRatio;
                    this.ScrollInfo.HorizontalPadding = Math.Round(this.ScrollInfo.HorizontalPadding) < 0
                                                            ? 0
                                                            : this.ScrollInfo.HorizontalPadding - animationRatio;
                    this.ScrollInfo.VerticalPadding = Math.Max(0.0, this.ScrollInfo.VerticalPadding);
                    this.ScrollInfo.HorizontalPadding = Math.Max(0.0, this.ScrollInfo.HorizontalPadding);
                    this.ScrollInfo.SetHorizontalOffset(this.HorizontalOffset);
                }
            }
            else if ((this.ScrollInfo.VerticalPadding <= 0) && (this.ScrollInfo.HorizontalPadding <= 0))
            {
                while ((this.ScrollInfo.VerticalPadding < 0) || (this.ScrollInfo.HorizontalPadding < 0))
                {
                    await Task.Delay(1);
                    this.ScrollInfo.VerticalPadding = Math.Round(this.ScrollInfo.VerticalPadding) > 0
                                                          ? 0
                                                          : this.ScrollInfo.VerticalPadding + animationRatio;
                    this.ScrollInfo.HorizontalPadding = Math.Round(this.ScrollInfo.HorizontalPadding) > 0
                                                            ? 0
                                                            : this.ScrollInfo.HorizontalPadding + animationRatio;
                    this.ScrollInfo.VerticalPadding = Math.Min(0.0, this.ScrollInfo.VerticalPadding);
                    this.ScrollInfo.HorizontalPadding = Math.Min(0.0, this.ScrollInfo.HorizontalPadding);
                    this.ScrollInfo.SetHorizontalOffset(this.HorizontalOffset);
                }
            }
            else if ((this.ScrollInfo.VerticalPadding <= 0) && (this.ScrollInfo.HorizontalPadding >= 0))
            {
                while ((this.ScrollInfo.VerticalPadding < 0) || (this.ScrollInfo.HorizontalPadding > 0))
                {
                    await Task.Delay(1);
                    this.ScrollInfo.VerticalPadding = Math.Round(this.ScrollInfo.VerticalPadding) > 0
                                                          ? 0
                                                          : this.ScrollInfo.VerticalPadding + animationRatio;
                    this.ScrollInfo.HorizontalPadding = Math.Round(this.ScrollInfo.HorizontalPadding) < 0
                                                            ? 0
                                                            : this.ScrollInfo.HorizontalPadding - animationRatio;
                    this.ScrollInfo.VerticalPadding = Math.Min(0.0, this.ScrollInfo.VerticalPadding);
                    this.ScrollInfo.HorizontalPadding = Math.Max(0.0, this.ScrollInfo.HorizontalPadding);
                    this.ScrollInfo.SetHorizontalOffset(this.HorizontalOffset);
                }
            }
            else
            {
                while ((this.ScrollInfo.VerticalPadding > 0) || (this.ScrollInfo.HorizontalPadding < 0))
                {
                    await Task.Delay(1);
                    this.ScrollInfo.VerticalPadding = Math.Round(this.ScrollInfo.VerticalPadding) < 0
                                                          ? 0
                                                          : this.ScrollInfo.VerticalPadding - animationRatio;
                    this.ScrollInfo.HorizontalPadding = Math.Round(this.ScrollInfo.HorizontalPadding) > 0
                                                            ? 0
                                                            : this.ScrollInfo.HorizontalPadding + animationRatio;
                    this.ScrollInfo.VerticalPadding = Math.Max(0.0, this.ScrollInfo.VerticalPadding);
                    this.ScrollInfo.HorizontalPadding = Math.Min(0.0, this.ScrollInfo.HorizontalPadding);
                    this.ScrollInfo.SetHorizontalOffset(this.HorizontalOffset);
                }
            }
        }

        private async void ManipulationPagerAnimation(bool isHorizontal)
        {
            double increasedValue = isHorizontal ? this.HorizontalOffset % 50 : this.VerticalOffset % 50;
            if (increasedValue > 0)
            {
                double animationRatio = 3;
                while (increasedValue > 0)
                {
                    await Task.Delay(1);
                    var value = increasedValue - animationRatio;
                    animationRatio = value > 0 ? animationRatio : increasedValue;
                    increasedValue = value > 0 ? value : 0;
                    if (isHorizontal)
                        this.ScrollInfo.SetHorizontalOffset(this.HorizontalOffset - animationRatio);
                    else
                        this.ScrollInfo.SetVerticalOffset(this.VerticalOffset - animationRatio);
                }
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Mouse)
                return;

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
                //   e.Handled = true;
            }
            base.OnManipulationDelta(e);
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Mouse)
                VisualStateManager.GoToState(this, "MouseIndicator", true);
            else if (e.PointerDeviceType == PointerDeviceType.Touch)
                VisualStateManager.GoToState(this, "TouchIndicator", true);

            if (this._panningInfo != null)
            {
                if (!this._panningInfo.IsPanning)
                {
                    this._panningInfo = null;
                    e.Handled = true;
                    return;
                }
                else
                {
                    e.TranslationBehavior.DesiredDeceleration = 0.003; // this.PanningDeceleration;
                }
                e.Handled = true;
            }
            base.OnManipulationInertiaStarting(e);
        }

        protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
        {
            this._panningInfo = null;
            e.Mode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateRailsX |
                     ManipulationModes.TranslateRailsY | ManipulationModes.TranslateInertia;
            e.Container = this;
            var info = new PanningInfo
                {
                    OriginalHorizontalOffset = this.HorizontalOffset,
                    OriginalVerticalOffset = this.VerticalOffset
                };
            this._panningInfo = info;
            double num = this.ViewportWidth + 1.0;
            double num2 = this.ViewportHeight + 1.0;
            if (this.ElementScrollContentPresenter != null)
            {
                this._panningInfo.DeltaPerHorizontalOffet = DoubleUtil.AreClose(num, 0.0)
                                                                ? 0.0
                                                                : (this.ElementScrollContentPresenter.ActualWidth/num);
                this._panningInfo.DeltaPerVerticalOffset = DoubleUtil.AreClose(num2, 0.0)
                                                               ? 0.0
                                                               : (this.ElementScrollContentPresenter.ActualHeight/num2);
            }
            else
            {
                this._panningInfo.DeltaPerHorizontalOffet = DoubleUtil.AreClose(num, 0.0) ? 0.0 : (base.ActualWidth/num);
                this._panningInfo.DeltaPerVerticalOffset = DoubleUtil.AreClose(num2, 0.0)
                                                               ? 0.0
                                                               : (base.ActualHeight/num2);
            }
            e.Handled = true;
        }

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
#endif

#if !WinRT
        #region OnKeyDown
        protected override void OnKeyDown(KeyEventArgs e)
        {
            LastMove = DateTime.Now;
        }
        #endregion
#endif

#if  WPF

        #region OnPreviewMouseDown

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            //if ((!e.Handled && !this.TemplatedParentHandlesMouseButton) && base.Focus(FocusState.Pointer))
            //{
            //    e.Handled = true;
            //}
        }
        
        #endregion

        #region OnPreviewMouseMove

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            //if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            //    VisualStateManager.GoToState(this, "MouseIndicator", true);
            //if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            //    VisualStateManager.GoToState(this, "TouchIndicator", true);
            LastMove = DateTime.Now;
        }
        
        #endregion

        #region OnPointerReleased

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            //e.Handled = true;
        }

        #endregion

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
#if WPF
            if (!e.OriginalSource.Equals(e.Source))
            {
                var el = e.Source as DependencyObject;
                if (el != null)
                {
                    if (VisualContainer.GetWantsMouseInput(el, this) == true)
                        return;
                }
            }
#endif

            base.OnPreviewMouseWheel(e);
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
#if SILVERLIGHT

        #region OnMouseLeftButtonDown

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            //if ((!e.Handled && !this.TemplatedParentHandlesMouseButton) && base.Focus(FocusState.Pointer))
            //{
            //    e.Handled = true;
            //}
        }
        #endregion

        #region OnMouseMove

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            //    VisualStateManager.GoToState(this, "MouseIndicator", true);
            //if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            //    VisualStateManager.GoToState(this, "TouchIndicator", true);
            LastMove = DateTime.Now;
        }
        #endregion

        #region OnPointerReleased

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            //We have not handled OnMouseLeftButtonUp, cos MultiColumnDropDown Control requires this Event for closing the Popup after selection.
            //e.Handled = true;
        }

        #endregion

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
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
        #region Manipulation Events

        private PanningInfo _panningInfo;

        private bool IsPastInertialLimit()
        {
            if (!DoubleUtil.GreaterThanOrClose(Math.Abs(this._panningInfo.UnusedTranslation.X), 50.0))
            {
                return DoubleUtil.GreaterThanOrClose(Math.Abs(this._panningInfo.UnusedTranslation.Y), 50.0);
            }
            return true;
        }


        private void ManipulateScroll(double delta, double cumulativeTranslation, bool isHorizontal)
        {
            double num = isHorizontal ? this._panningInfo.UnusedTranslation.X : this._panningInfo.UnusedTranslation.Y;
            double num2 = isHorizontal ? this.HorizontalOffset : this.VerticalOffset;
            double num3 = isHorizontal ? this.ScrollableWidth : this.ScrollableHeight;
            if (DoubleUtil.AreClose(num3, 0.0))
            {
                // If the Scrollable length in this direction is 0,
                // then we should neither scroll nor report the boundary feedback
                num = 0.0;
                delta = 0.0;
            }
            else if ((DoubleUtil.GreaterThan(delta, 0.0) && DoubleUtil.AreClose(num2, 0.0)) ||
                     (DoubleUtil.LessThan(delta, 0.0) && DoubleUtil.AreClose(num2, num3)))
            {
                // If we are past the boundary and the delta is in the same direction,
                // then add the delta to the unused vector
                num += delta;
                delta = 0.0;
            }
            else if (DoubleUtil.LessThan(delta, 0.0) && DoubleUtil.GreaterThan(num, 0.0))
            {
                // If we are past the boundary in positive direction
                // and the delta is in negative direction,
                // then compensate the delta from unused vector.
                double num4 = Math.Max((double) (num + delta), (double) 0.0);
                delta += num - num4;
                num = num4;
            }
            else if (DoubleUtil.GreaterThan(delta, 0.0) && DoubleUtil.LessThan(num, 0.0))
            {
                // If we are past the boundary in negative direction
                // and the delta is in positive direction,
                // then compensate the delta from unused vector.
                double num5 = Math.Min((double) (num + delta), (double) 0.0);
                delta += num - num5;
                num = num5;
            }
            if (isHorizontal)
            {
                if (!DoubleUtil.AreClose(delta, 0.0))
                {
                    this.ScrollToHorizontalOffset(this._panningInfo.OriginalHorizontalOffset -
                                                  Math.Round(
                                                      (double)
                                                      (cumulativeTranslation/this._panningInfo.DeltaPerHorizontalOffet)));
                }
                this._panningInfo.UnusedTranslation = new Point(num, this._panningInfo.UnusedTranslation.Y);
            }
            else
            {
                if (!DoubleUtil.AreClose(delta, 0.0))
                {
                    this.ScrollToVerticalOffset(this._panningInfo.OriginalVerticalOffset -
                                                Math.Round(
                                                    (double)
                                                    (cumulativeTranslation/this._panningInfo.DeltaPerVerticalOffset)));
                }
                this._panningInfo.UnusedTranslation = new Point(this._panningInfo.UnusedTranslation.X, num);
            }
        }

        private bool CanStartScrollManipulation(Point translation, out bool cancelManipulation)
        {
            cancelManipulation = false;
            bool flag = DoubleUtil.GreaterThan(Math.Abs(translation.X), 3.0);
            bool flag2 = DoubleUtil.GreaterThan(Math.Abs(translation.Y), 3.0);
            if (flag || flag2)
            {
                return true;
            }
            return false;
        }

        #endregion
#endif
        #endregion

        #region private method

        /// <summary>
        /// Dispatch Timer is a DispatcherTimer Method, Checks for User Actions to hide the Scrollbars if the Control ain't active
        /// </summary>
        public void WireEvents()
        {
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, object e)
        {
            TimeSpan elaped = DateTime.Now - LastMove;
            if (elaped.Seconds >= 1)
            {
                VisualStateManager.GoToState(this, "NoIndicator", true);
                elaped = TimeSpan.Zero;
            }
        }

        private void HandleHorizontalScroll(ScrollEventArgs e)
        {
            LastMove = DateTime.Now;
            HandleHorizontalScroll(new ScrollableEventArgs(e.ScrollEventType, e.NewValue));
        }

        private void HandleHorizontalScroll(ScrollableEventArgs e)
        {
            if (this.ScrollInfo != null)
            {
                double horizontalOffset = this.ScrollInfo.HorizontalOffset;
                double newValue = horizontalOffset;
                switch (e.ScrollEventType)
                {
                    case ScrollEventType.SmallDecrement:
                        this.ScrollInfo.LineLeft();
                        break;

                    case ScrollEventType.SmallIncrement:
                        this.ScrollInfo.LineRight();
                        break;

                    case ScrollEventType.LargeDecrement:
                        this.ScrollInfo.PageLeft();
                        break;

                    case ScrollEventType.LargeIncrement:
                        this.ScrollInfo.PageRight();
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
                if (!DoubleUtil.AreClose(horizontalOffset, newValue))
                {
                    this.ScrollInfo.SetHorizontalOffset(newValue);
                }
            }
        }

        private void HandleScroll(Orientation orientation, ScrollableEventArgs e)
        {
            if (orientation == Orientation.Horizontal)
            {
                this.HandleHorizontalScroll(e);
            }
            else
            {
                this.HandleVerticalScroll(e);
            }
        }


        private void HandleVerticalScroll(ScrollEventArgs e)
        {
            LastMove = DateTime.Now;
            HandleVerticalScroll(new ScrollableEventArgs(e.ScrollEventType, e.NewValue));
        }

        private void HandleVerticalScroll(ScrollableEventArgs e)
        {
            if (this.ScrollInfo != null)
            {
                double verticalOffset = this.ScrollInfo.VerticalOffset;
                double newValue = verticalOffset;
                switch (e.ScrollEventType)
                {
                    case ScrollEventType.SmallDecrement:
                        this.ScrollInfo.LineUp();
                        break;

                    case ScrollEventType.SmallIncrement:
                        this.ScrollInfo.LineDown();
                        break;

                    case ScrollEventType.LargeDecrement:
                        this.ScrollInfo.PageUp();
                        break;

                    case ScrollEventType.LargeIncrement:
                        this.ScrollInfo.PageDown();
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
                if (!DoubleUtil.AreClose(verticalOffset, newValue))
                {
                    this.ScrollInfo.SetVerticalOffset(newValue);
                }
            }
        }

        private void MakeVisible(UIElement element, Rect targetRect)
        {
            UIElement element2 = element;
            var elementScrollContentPresenter = this.ElementScrollContentPresenter as IScrollableInfo;
            UIElement element3 = this.ElementScrollContentPresenter;
            if ((((element2 != null) && (element3 != null)) && ((element3 == element2))))
            {
                if (targetRect.IsEmpty)
                {
                    targetRect = new Rect(0.0, 0.0, element2.RenderSize.Width, element2.RenderSize.Height);
                }
                Rect targetRectangle = elementScrollContentPresenter.MakeVisible(element2, targetRect);
                if (!targetRectangle.IsEmpty)
                {
#if WinRT
                    Point point =
                        element3.TransformToVisual(this).TransformPoint(new Point(targetRectangle.X, targetRectangle.Y));
                    targetRectangle.X = point.X;
                    targetRectangle.Y = point.Y;
#endif
                }
                //base.BringIntoView(targetRectangle);
            }
        }

        private static void OnScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = d as ScrollableContentViewer;
            if (viewer != null)
            {
                if (viewer.ScrollInfo != null)
                {
                    viewer.ScrollInfo.CanHorizontallyScroll = viewer.HorizontalScrollBarVisibility !=
                                                              ScrollBarVisibility.Disabled;
                    viewer.ScrollInfo.CanVerticallyScroll = viewer.VerticalScrollBarVisibility !=
                                                            ScrollBarVisibility.Disabled;
                }
                viewer.InvalidateMeasure();
            }
        }

        #endregion

        #region Dispose

        public void UnWireEvents()
        {
            _timer.Stop();
            _timer.Tick -= _timer_Tick;
        }

        public void Dispose()
        {
            this.UnWireEvents();
        }

        #endregion
    }

    #region ScrollableEventArgs

    public class ScrollableEventArgs : RoutedEventArgs
    {
        private double _newValue;

        public double NewValue
        {
            get { return _newValue; }
        }

        private ScrollEventType _scrollEventType;

        public ScrollEventType ScrollEventType
        {
            get { return _scrollEventType; }
        }

        public ScrollableEventArgs(ScrollEventType scrollEventType, double newValue)
        {
            this._newValue = newValue;
            this._scrollEventType = scrollEventType;
        }
    }

    #endregion
}