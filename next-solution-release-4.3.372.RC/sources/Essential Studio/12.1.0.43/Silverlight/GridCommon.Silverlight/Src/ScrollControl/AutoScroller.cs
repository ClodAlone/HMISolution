#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

#if !WinRT
using System.Windows.Threading;
using Syncfusion.Windows.GridCommon;
namespace Syncfusion.Windows.Controls.Scroll
#else
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{
    /// <summary>
    /// Provides automatic scrolling of content when the user drags the pressed
    /// mouse to an edge of the control.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class AutoScroller : IDisposable
    {
        // Mousedrag to corner (auto) scrolling
        private AutoScrollOrientation autoScrolling = AutoScrollOrientation.None;
        private Rect autoScrollBounds = Rect.Empty;
        Size insideScrollMargins = new Size(20, 20);
        bool inMouseDragScroll;
        DispatcherTimer autoScrollTimer = null;
        TimeSpan intervalTime = new TimeSpan(0, 0, 0, 0, 20);
        IScrollBarProvider host;  // uses VScrollBar, HScrollBar properties.
        bool enabled = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoScroller"/> class.
        /// </summary>
        /// <param name="scrollControl">The scroll control.</param>
        public AutoScroller(IScrollBarProvider scrollControl)
        {
            this.host = scrollControl;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AutoScroller"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; if (!enabled) AutoScrolling = AutoScrollOrientation.None; }
        }

        /// <summary>
        /// Gets a value indicating whether a scroll operation is triggered by this object
        /// </summary>
        /// <value><c>true</c> if mouse drag scrolling; otherwise, <c>false</c>.</value>
        public bool InMouseDragScroll
        {
            get
            {
                return this.inMouseDragScroll;
            }
        }

        /// <summary>
        /// Gets the state describing for the horizontal scroll bar.
        /// </summary>
        /// <value>The horizontal scroll bar state.</value>
        public IScrollBar HScrollBar
        {
            get
            {
                return host.HScrollBar;
            }
        }

        /// <summary>
        /// Gets the state describing for the vertical scroll bar.
        /// </summary>
        /// <value>The vertical scroll bar state.</value>
        public IScrollBar VScrollBar
        {
            get
            {
                return host.VScrollBar;
            }
        }
        /// <summary>
        /// Disables or specifies the direction for automatic scrolling when the user drags
        /// the mouse cursor out of the scrolling area.
        /// </summary>
        /// <remarks>
        /// <list type="">
        /// <item>ScrollBars.None will disable scrolling.</item>
        /// <item>ScrollBars.Horizontal will enable horizontal scrolling.</item>
        /// <item>ScrollBars.Vertical will enable vertical scrolling.</item>
        /// <item>ScrollBars.Horizontal|ScrollBars.Vertical will enable both horizontal and vertical scrolling.</item>
        /// </list>
        /// </remarks>
#if !WinRT
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public AutoScrollOrientation AutoScrolling
        {
            get
            {
                return autoScrolling;
            }
            set
            {
                if (!Enabled)
                    value = AutoScrollOrientation.None;

                if (value != autoScrolling)
                {
                    autoScrolling = value;
                    if (autoScrolling == AutoScrollOrientation.None)
                        StopAutoScrollTimer();
                    else
                        StartAutoScrollTimer();
                    OnAutoScrollingChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the AutoScrolling property is changed.
        /// </summary>
        /// <remarks>
        /// If you want to prevent autoscrolling, you should handle this event
        /// and reset the AutoScrolling property to ScrollBars.None.
        /// </remarks>
#if !WinRT
        [
        Description("Occurs when AutoScrolling property is changed."),
        Category("Behavior")
        ]
#endif
        public event EventHandler AutoScrollingChanged;


        /// <summary>
        /// Raises the <see cref="AutoScrollingChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnAutoScrollingChanged(EventArgs e)
        {
            if (AutoScrollingChanged != null)
                AutoScrollingChanged(this, e);
        }

        /// <summary>
        /// Gets or sets the outer scrolling area. Typically the client area of the control.
        /// </summary>
#if !WinRT
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public Rect AutoScrollBounds
        {
            get
            {
                if (autoScrollBounds.IsEmpty)
                    return new Rect(new Point(0, 0), host.Element.RenderSize);
                return autoScrollBounds;
            }
            set
            {
                autoScrollBounds = value;
            }
        }

        /// <summary>
        /// Gets the inside scrolling area. The control will scroll if the user drags
        /// the mouse outside this area.
        /// </summary>
#if !WinRT
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),]
#endif
        public virtual Rect InsideScrollBounds
        {
            get
            {
                Rect r = AutoScrollBounds;
                if (!r.IsEmpty)
                {
#if (SILVERLIGHT || WinRT)
                    r.X += InsideScrollMargins.Width;
                    var scrollWidth = InsideScrollMargins.Width * 2;
                    if (r.Width > scrollWidth)
                    {
                        r.Width -= scrollWidth;
                    }
                    r.Y += InsideScrollMargins.Height;
                    var scrollHeight = InsideScrollMargins.Height * 2;
                    if (r.Height > scrollHeight)
                    {
                        r.Height -= scrollHeight;
                    }
#else
                    r.Inflate(-InsideScrollMargins.Width, -InsideScrollMargins.Height);
#endif
                }
                return r;
            }
        }

        /// <summary>
        /// Gets or sets the default margins for the scrolling area when the user moves the mouse to the
        /// margin between InsideScrollBounds and AutoScrollBounds.
        /// </summary>
#if !WinRT
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),]
#endif
        public Size InsideScrollMargins
        {
            get
            {
                return insideScrollMargins;
            }
            set
            {
                insideScrollMargins = value;
            }
        }

        bool ShouldSerializeInsideScrollMargins()
        {
            return insideScrollMargins.Width != 10 || insideScrollMargins.Height != 10;
        }
        /// <summary>
        /// Resets the <see cref="InsideScrollMargins"/> property to its default value.
        /// </summary>
        public void ResetInsideScrollMargins()
        {
            insideScrollMargins = new Size(10, 10);
        }

        /// <summary>
        /// sets the timer interval for auto scrolling.
        /// </summary>
        public TimeSpan IntervalTime
        {
            get
            {
                return intervalTime;
            }
            set
            {
                intervalTime = value;
            }
        }

        void StopAutoScrollTimer()
        {
            if (autoScrollTimer != null)
            {
                autoScrollTimer.Tick -= (autoScrollTimer_Tick);
                autoScrollTimer.Stop();
                autoScrollTimer = null;
#if SILVERLIGHT
                host.Element.MouseMove -= new MouseEventHandler(Element_MouseMove);
#endif
            }
        }

        void StartAutoScrollTimer()
        {

            if (autoScrollTimer == null)
            {
                autoScrollTimer = new DispatcherTimer();
                autoScrollTimer.Interval = IntervalTime; ;
                autoScrollTimer.Tick += (autoScrollTimer_Tick);
                autoScrollTimer.Start();
#if (SILVERLIGHT && !WinRT)
                host.Element.MouseMove += new MouseEventHandler(Element_MouseMove);
#endif
#if WinRT
                host.Element.PointerMoved += Element_PointerMoved;
#endif
                lastMouseMovePosition = new Point(0, 0);

            }
        }
#if WinRT
        void Element_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            lastMouseMovePosition = e.GetCurrentPoint(host.Element).Position;
        }
#endif

#if (SILVERLIGHT || WinRT)
        Point lastMouseMovePosition = new Point(0, 0);

#if !WinRT
        void Element_MouseMove(object sender, MouseEventArgs e)
        {
            lastMouseMovePosition = e.GetPosition(host.Element);
        }
#endif
#endif

#if WinRT
        void autoScrollTimer_Tick(object sender, object e)
#else
        void autoScrollTimer_Tick(object sender, EventArgs e)
#endif
        {
#if (SILVERLIGHT || WinRT)
            if (lastMouseMovePosition.X > 0
                || lastMouseMovePosition.Y > 0)
                CheckAutoScroll(lastMouseMovePosition);

#else
            CheckAutoScroll(Mouse.GetPosition(host.Element));
#endif
        }

        bool allowScrollOutsideBounds = true;

        /// <summary>
        /// Gets or sets a value indicating whether the parent control should
        /// scroll when the user drags the mouse outside the parent controls 
        /// client area.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if allow to scroll outside bounds; otherwise, <c>false</c>.
        /// </value>
        public bool AllowScrollOutsideBounds
        {
            get { return allowScrollOutsideBounds; }
            set { allowScrollOutsideBounds = value; }
        }

        //Point lastPoint;

        protected virtual void CheckAutoScroll(Point mousePoint)
        {
            int thisTick = Environment.TickCount;
            bool isLineLeft = false, isLineRight = false, isLineUp = false, isLineDown = false;
            Rect autoScrollBounds = AutoScrollBounds;
            Rect insideScrollBounds = InsideScrollBounds;

            if (AutoScrolling != AutoScrollOrientation.None &&
                !autoScrollBounds.IsEmpty &&
                !insideScrollBounds.IsEmpty
                /* comment this out if you want to avoid scrolling when
                 * the user drags the mouse outside of the AutoScrollBounds area*/
                 && AllowScrollOutsideBounds || autoScrollBounds.Contains(mousePoint)
                )
            {
                if ((AutoScrolling & AutoScrollOrientation.Horizontal) != 0)
                {
                    bool rightToLeft = false;//HScrollBar.InnerScrollBar != null && HScrollBar.InnerScrollBar.RightToLeft == RightToLeft.Yes;

                    if (mousePoint.X < insideScrollBounds.Left &&
                        (!rightToLeft && HScrollBar.Value > HScrollBar.Minimum
                        || rightToLeft && HScrollBar.Value + HScrollBar.LargeChange <= HScrollBar.Maximum)
                        )
                    {
                        inMouseDragScroll = true;
                        isLineLeft = true;
                        host.LineLeft();
                    }
                    else if (mousePoint.X > insideScrollBounds.Right &&
                        (!rightToLeft && HScrollBar.Value + HScrollBar.LargeChange <= HScrollBar.Maximum
                        || rightToLeft && HScrollBar.Value > HScrollBar.Minimum)
                        )
                    {
                        inMouseDragScroll = true;
                        isLineRight = true;
                        host.LineRight();
                    }
                }

                if ((AutoScrolling & AutoScrollOrientation.Vertical) != 0)
                {
                    if (mousePoint.Y < insideScrollBounds.Top &&
                        VScrollBar.Value > VScrollBar.Minimum)
                    {
                        inMouseDragScroll = true;
                        isLineUp = true;
                        host.LineUp();
                    }
                    else if (mousePoint.Y > insideScrollBounds.Bottom &&
                        VScrollBar.Value + VScrollBar.LargeChange <= VScrollBar.Maximum)
                    {
                        inMouseDragScroll = true;
                        isLineDown = true;
                        host.LineDown();
                    }
                }

                if (isLineDown || isLineUp || isLineLeft || isLineRight)
                {
                    this.RaiseAutoScrollerValueChanged(isLineUp, isLineDown, isLineLeft, isLineRight);
                }

                inMouseDragScroll = false;
            }
        }

        protected virtual void RaiseAutoScrollerValueChanged(bool isLineUp, bool isLineDown, bool isLineLeft, bool isLineRight)
        {
            if (this.AutoScrollerValueChanged != null)
            {
                this.AutoScrollerValueChanged(this, new AutoScrollerValueChangedEventArgs(isLineUp, isLineDown, isLineLeft, isLineRight));
            }
        }

        public event AutoScrollerValueChangedEventHandler AutoScrollerValueChanged;

        public delegate void AutoScrollerValueChangedEventHandler(object sender, AutoScrollerValueChangedEventArgs args);

        public void Dispose()
        {
            StopAutoScrollTimer();
            this.host = null;
        }
    }

    /// <summary>
    /// Provides data about AutoScroller update
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class AutoScrollerValueChangedEventArgs : EventArgs
    {
        public AutoScrollerValueChangedEventArgs(bool isLineUp, bool isLineDown, bool isLineLeft, bool isLineRight)
        {
            this.isLineUp = isLineUp;
            this.isLineDown = isLineDown;
            this.isLineLeft = isLineLeft;
            this.isLineRight = isLineRight;
        }

        private bool isLineLeft;

        /// <summary>
        /// Indicates Scrolling Left
        /// </summary>
        public bool IsLineLeft
        {
            get { return isLineLeft; }
            private set { isLineLeft = value; }
        }

        private bool isLineRight;

        /// <summary>
        /// Indicates Scrolling Right
        /// </summary>
        public bool IsLineRight
        {
            get { return isLineRight; }
            private set { isLineRight = value; }
        }

        private bool isLineUp;

        /// <summary>
        /// Indicates Scrolling Up
        /// </summary>
        public bool IsLineUp
        {
            get { return isLineUp; }
            private set { isLineUp = value; }
        }


        private bool isLineDown;

        /// <summary>
        /// Indicates Scrolling down
        /// </summary>
        public bool IsLineDown
        {
            get { return isLineDown; }
            private set { isLineDown = value; }
        }
    }
}
