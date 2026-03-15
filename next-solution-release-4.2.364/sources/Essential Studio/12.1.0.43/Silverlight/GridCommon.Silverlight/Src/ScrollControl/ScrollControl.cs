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

#if !WinRT
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
namespace Syncfusion.Windows.Controls.Scroll
#else
using Syncfusion.WinRT.GridCommon;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{
    /// <summary>
    /// ScrollControl maintains a collection of child frames at the top, bottom, left and right of the control 
    /// so that they do not scroll similiar to Internet Explorer frames concept. It maintains clipping for 
    /// child elements inside these frames. The height and width of the frames is specified through the 
    /// <see cref="TopLeftFrameExtent"/> and <see cref="BottomRightFrameExtent"/> methods. The inner frame 
    /// is scrollable both horizontally and vertically.
    /// <para/>
    /// ScrollControl implements IScrollInfo and provides <see cref="VScrollBar"/> and 
    /// <see cref="HScrollBar"/> properties to simplify management of scrollbars. 
    /// <para/>
    /// Adding and removing Visual elements from the <see cref="VisualContainer.Children"/> collection does not trigger
    /// calls to InvalidateMeasure. This allows adding and removing elements
    /// on the fly. A derived control is responsible to call Measure and Arrange
    /// on child elements since this base class will not do this by itsself.
    /// <para/>
    /// ScrollControl also has support for automatic scrolling of content when the user drags the pressed
    /// mouse to an edge of the control. 
    /// <para/>
    /// ScrollControl is best embedded inside a ScrollViewer container, but can also be placed standalone on a form.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class ScrollControl : ContentControl, IScrollBarProvider
    {
        #region Fields
        // Fields
        ScrollableContentViewer _owner;
        ScrollInfo _hScrollBar;
        ScrollInfo _vScrollBar;
        ScrollInfo _hScrollBarCopy;
        ScrollInfo _vScrollBarCopy;
        bool ignoreHScrollBarEvents = false;
        bool ignoreVScrollBarEvents = false;
        bool isInArrangeContent = false;
        Size lastArrangeSize;

        public new Size RenderSize
        {
            get { return lastArrangeSize; }
        }

        int suspendInvalidate = 0;
        //bool isInvalidateDirty = false;
        bool isArrangeDirty = true;
        bool invalidateArrangeOnLoaded = false;

        Panel backgroundFrame;
        Panel foregroundFrame;
        Panel innerFrame;
        Panel elementFrame;
        Panel graphicFrame;

        Size topLeftFrameExtent = new Size(0, 0);
        Size bottomRightFrameExtent = new Size(0, 0);
        AutoScroller autoScroller;

        //MouseEventTargetCollection mouseEventsListeners = new MouseEventTargetCollection();
        MouseControllerDispatcher mouseControllerDispatcher;

        public MouseControllerDispatcher MouseControllerDispatcher
        {
            get { return mouseControllerDispatcher; }
        }

        #endregion
        #region Zoom


        public double ZoomScale
        {
            get
            {
                return (double)GetValue(ZoomScaleProperty);
            }
            set
            {
                SetValue(ZoomScaleProperty, value);
            }
        }

        bool ZoomScaleChangedBeforeLoaded = false;

        public static readonly DependencyProperty ZoomScaleProperty =
               DependencyProperty.Register("ZoomScale", typeof(double), typeof(ScrollControl), new PropertyMetadata(1.0, ZoomScale_Changed));

        private static void ZoomScale_Changed(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScrollControl c = (ScrollControl)o;
            if (c.ScrollOwner != null)
            {
                c.ScrollOwner.ApplyLayoutTransform();
                c.InvalidateVisual();
            }
            else
            {
                c.ZoomScaleChangedBeforeLoaded = true;
            }
        }

        #endregion
        #region Ctor, Unload
        static ScrollControl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollControl"/> class.
        /// </summary>
        public ScrollControl()
        {
            Content = new VisualContainer("ScrollControl.Content");
            InitializeChildFrames();
            this.Loaded += new RoutedEventHandler(this_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(ScrollControl_SizeChanged);
            this.Unloaded += new RoutedEventHandler(ScrollControl_Unloaded);
            mouseControllerDispatcher = new MouseControllerDispatcher(this);
            autoScroller = new AutoScroller(this);
            //InitializeMouseEventListeners();
        }

        void ScrollControl_Unloaded(object sender, RoutedEventArgs e)
        {
            OnUnloaded(e);
            invalidateArrangeOnLoaded = true;
        }

        void ScrollControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnSizeChanged(e);
            InvalidateVisual(true);
        }

        /// <summary>
        /// This virtual method is called from the <see cref="FrameworkElement.SizeChanged"/> event handler.<para/>
        /// Disable the render optimization when the size changed
        /// </summary>
        /// <param name="e">SizeChangedEventArgs</param>
        protected virtual void OnSizeChanged(SizeChangedEventArgs e)
        {

        }

        /// <summary>
        /// Gets the auto scroller which provides automatic scrolling of content when the user drags the pressed
        /// mouse to an edge of the control.
        /// </summary>
        /// <value>The auto scroller.</value>
        public AutoScroller AutoScroller
        {
            get
            {
                return this.autoScroller;
            }
        }

        Panel Panel { get { return (Panel)Content; } }

        public UIElementCollection Children
        {
            get
            {
                return Panel.Children;
            }
        }

        void this_Loaded(object sender, RoutedEventArgs e)
        {
#if !WinRT
            ZoomScaleChangedBeforeLoaded = false;
#endif
            OnLoaded(e);
            if (invalidateArrangeOnLoaded)
                InvalidateVisual(true);
        }

        /// <summary>
        /// This virtual method is called from the <see cref="FrameworkElement.Loaded"/> event handler.<para/>
        /// Do not override this method to wire events since a control can be unloaded and loaded 
        /// multiple times during its lifetime.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnLoaded(RoutedEventArgs e)
        {
#if SILVERLIGHT
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    AutoMeasureRecursive(Content as Panel);
            //    AutoArrangeRecursive(Content as Panel);
            //}));
#endif
        }

        /// <summary>
        /// This virtual method is called from the <see cref="FrameworkElement.Unloaded"/> event handler.<para/>
        /// Override this method to clear cached settings (e.g. rendered styles, visibility of rows) when the 
        /// control was unloaded. Do not unwire events here since a control can be unloaded and loaded 
        /// multiple times during its lifetime.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnloaded(RoutedEventArgs e)
        {
        }

        void InitializeChildFrames()
        {
            backgroundFrame = new VisualContainer("BackgroundFrame");
            innerFrame = new VisualContainer("InnerFrame");
            foregroundFrame = new VisualContainer("ForegroundFrame");
            elementFrame = new VisualContainer("ElementFrame");
            graphicFrame = new VisualContainer("GraphicFrame");

            backgroundFrame.IsHitTestVisible = false;
            foregroundFrame.IsHitTestVisible = false;
            elementFrame.IsHitTestVisible = false;
            graphicFrame.IsHitTestVisible = false;

            //innerFrame.SetValue(Control.BackgroundProperty, Colors.Transparent);
            //innerFrame.Opacity = 0.20;
            //foregroundFrame.Opacity = 0.20;

            Children.Add(backgroundFrame);
            Children.Add(innerFrame);
            Children.Add(foregroundFrame);
            Children.Add(elementFrame);
            Children.Add(graphicFrame);

            // Do not set SetWantsMouseInput for backgroundFrame. See
            // also InitalizeNested in GridNestedSharedLayoutGrid and
            // VisualContainer.GetWantsMouseInput for another
            // solution. The problem this fixes is that clicking inside
            // a textbox inside nested grid would not select the text inside
            // that cell. Same goes with static text inside a cell in a nested grid.
            // Review this again later!
            // Update 7/28/08: This seems to be fine now together with 
            // using NonHitTestDrawingVisuals for drawing-only frames
            // and HitTestCore overrides.
            VisualContainer.SetWantsMouseInput(backgroundFrame, false);
            VisualContainer.SetWantsMouseInput(graphicFrame, true);
            VisualContainer.SetWantsMouseInput(innerFrame, false);
            VisualContainer.SetWantsMouseInput(foregroundFrame, false);
            VisualContainer.SetWantsMouseInput(elementFrame, false);

            // ScrollBody
            AddChildFrame(false, false, false, false, ElementFrame);
            AddChildFrame(false, false, false, false, InnerFrame);
            GetChildFrame(false, false, false, false, this.GraphicFrame);
            GetChildFrame(false, true, false, false, this.GraphicFrame);
        }
        #endregion
        #region Measure, ArrangeOverride and Render

        public override string ToString()
        {
            return String.Format("{2}: {0} with {1} elements.", Name, Children.Count, GetType().Name);
        }

        /// <summary>
        /// Arranges all child frames. Each frames <see cref="Canvas.LeftProperty"/>, <see cref="Canvas.TopProperty"/>, <see cref="Canvas.RightProperty"/> and <see cref="Canvas.BottomProperty"/> properties are initialized. <see cref="UIElement.Arrange"/> is called and a the <see cref="UIElement.Clip"/> property is set. After all child frames were arranged the virtual <see cref="OnArrangeContent"/> method is called.
        /// </summary>
        /// <param name="arrangeSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>Same size as given in arrangeSize.</returns>
        protected sealed override Size ArrangeOverride(Size arrangeSize)
        {
#if !WinRT
#if DEBUG
            Trace.Write("ArrangeOverride: " + ToString());
#endif
#endif
            if (invalidateArrangeOnLoaded)
            {
                isArrangeDirty = true;
                invalidateArrangeOnLoaded = false;
            }

            // REVIEW: OnArrangeOverride
            isInArrangeOverride = true;
            try
            {
                arrangeSize = OnArrangeOverride(arrangeSize, ref isArrangeDirty);
#if !WinRT
                if (!isArrangeDirty && arrangeSize == lastArrangeSize && (ZoomScale == 1.0 || ZoomScaleChangedBeforeLoaded))
#else
                if (!isArrangeDirty && arrangeSize == lastArrangeSize)
#endif
                {
#if (SILVERLIGHT || WinRT)
                    // need to avoid that grid lines and cell backgrounds disappear in silverlight.
                    // Also, force Measure and Arrange being called on all hosted elements in separate
                    // pass. One example where this is needed is a Charts hosted inside a DataTemplate where
                    // the grids CanContentScroll is false and the chart cell is outside current viewing area.
                    // Measure and Arrange needs to be called when cell becomes visible.
                    AutoArrangeRecursive(Content as Panel);
                    //Dispatcher.BeginInvoke(new Action(() => {
                    //    AutoMeasureRecursive(Content as Panel);
                    //    AutoArrangeRecursive(Content as Panel);
                    //}));
#endif
                    return lastArrangeSize;
                }

                HScrollBar.CopyTo(HScrollBarShadow);
                VScrollBar.CopyTo(VScrollBarShadow);

                ArrangeContent(arrangeSize);

                ArrangeFrames(arrangeSize);

                return base.ArrangeOverride(arrangeSize);
            }
            finally
            {
                isInArrangeOverride = false;
            }
        }

#if (SILVERLIGHT || WinRT)
        protected void AutoArrangeRecursive(Panel panel)
        {
            foreach (UIElement el in panel.Children)
            {
                if (el is Panel)
                    AutoArrangeRecursive((Panel)el);
                else
                {
                    Rect r = VisualContainer.GetRenderBounds(el);
                    if (!r.IsEmpty)
                        el.Arrange(r);
                }
            }
        }

        protected void AutoMeasureRecursive(Panel panel)
        {
            foreach (UIElement el in panel.Children)
            {
                if (el is Panel)
                    AutoMeasureRecursive((Panel)el);
                else
                {
                    Rect r = VisualContainer.GetRenderBounds(el);
                    if (!r.IsEmpty)
                    {
                        el.Measure(GridUtil.GetSize(r));
                    }
                }
            }
        }
#endif

        protected bool IsArrangeDirty
        {
            get { return isArrangeDirty; }
        }

        bool isInArrangeOverride = false;

        /// <summary>
        /// Gets a value indicating whether this instance is currently handling <see cref="ArrangeOverride"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in arrange override; otherwise, <c>false</c>.
        /// </value>
        public bool IsInArrangeOverride
        {
            get { return isInArrangeOverride; }
        }

        /// <summary>
        /// Called from <see cref="ArrangeOverride"/> first before any other code
        /// is executed in the method. You can change/set the isArrangeDirty flag
        /// to force <see cref="OnArrangeContent"/> to be called. By default
        /// content is only rearranged when this was previously indicated to the
        /// control with a <see cref="InvalidateVisual(bool)"/> call or when the
        /// 
        /// </summary>
        /// <param name="arrangeSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <param name="isArrangeDirty">You can change/set the isArrangeDirty flag
        /// to force <see cref="OnArrangeContent"/> to be called.</param>
        /// <returns>Same size as given in arrangeSize.</returns>
        protected virtual Size OnArrangeOverride(Size arrangeSize, ref bool isArrangeDirty)
        {
            return arrangeSize;
        }

        private void ArrangeFrames(Size arrangeSize)
        {
            Rect arrangeRect = new Rect(new Point(0, 0), arrangeSize);

            if (topLeftFrameExtent.IsEmpty)
                topLeftFrameExtent = new Size(0, 0);

            if (bottomRightFrameExtent.IsEmpty)
                bottomRightFrameExtent = new Size(0, 0);

            Rect r = new Rect(topLeftFrameExtent.Width, topLeftFrameExtent.Height,
                  Math.Max(0, arrangeSize.Width - topLeftFrameExtent.Width - bottomRightFrameExtent.Width),
                  Math.Max(0, arrangeSize.Height - topLeftFrameExtent.Height - bottomRightFrameExtent.Height));

            if(InnerFrame!=null)
            {
                foreach (UIElement visual in innerFrame.Children)
                {
                    ScrollControlChildFrame frame = visual as ScrollControlChildFrame;
                    if (frame != null)
                        ArrangeFrame(frame, arrangeRect, r);
                }
            }

            if (ElementFrame != null)
            {
                foreach (UIElement visual in elementFrame.Children)
                {
                    ScrollControlChildFrame frame = visual as ScrollControlChildFrame;
                    if (frame != null)
                        ArrangeFrame(frame, arrangeRect, r);
                }
            }

            if (GraphicFrame != null)
            {
                foreach (UIElement visual in graphicFrame.Children)
                {
                    ScrollControlChildFrame frame = visual as ScrollControlChildFrame;
                    if (frame != null)
                        ArrangeFrame(frame, arrangeRect, r);
                }
            }
        }

        private void ArrangeContent(Size arrangeSize)
        {
            lastArrangeSize = arrangeSize;
            isInArrangeContent = true;
            try
            {
                OnArrangeContent(arrangeSize);
            }
            finally
            {
                isInArrangeContent = false;
            }
            isArrangeDirty = false;
        }


        #endregion
        #region Invalidate

        /// <summary>
        /// Invalidates the rendering of the element, and forces a complete new layout pass. <see cref="System.Windows.UIElement.OnRender(System.Windows.Media.DrawingContext)"/> is called after the layout cycle is completed.
        /// The method is overloaded in <see cref="ScrollControl"/> to redirect its call to 
        /// <see cref="InvalidateVisual(System.Boolean)"/> with setArrangeDirty being true. 
        /// </summary>
#if (!SILVERLIGHT && !WinRT)
        new 
#endif
        public void InvalidateVisual()
        {
            InvalidateVisual(true);
        }

        /// <summary>
        /// Invalidates the rendering of the element, and forces a complete new layout pass when 
        /// setArrangeDirty is true. <see cref="System.Windows.UIElement.OnRender(System.Windows.Media.DrawingContext)"/> is called after the layout cycle is completed.
        /// </summary>
        /// <param name="setArrangeDirty">if set to <c>true</c> indicates that <see cref="OnArrangeContent"/>
        /// will be called when layout cycle occurs. Otherwise the OnArrangeContent will be skipped
        /// and only OnRender will be called.</param>
        public virtual void InvalidateVisual(bool setArrangeDirty)
        {
            if (this.isInArrangeContent)
                return;

            isArrangeDirty |= setArrangeDirty;

            if (IsSuspendedInvalidate)
            {
                //isInvalidateDirty = true;
                return;
            }

            //isInvalidateDirty = false;

            // Needed for SILVERLIGHT since UIElements
            // should be arranged with ScrollControlChildFrame.ArraneOverride
            //foreach (UIElement el in InnerFrame.Children)
            //    el.InvalidateArrange();

            base.InvalidateArrange();
            OnInvalidated(isArrangeDirty);
        }

        #region possible Suspend/ResumeInvalidate logic - no need found for now.
        ///// <summary>
        ///// Suspends the invalidatation of the controls contents with <see cref="Invalidate"/> calls 
        ///// until <see cref="ResumeInvalidate"/> is called. When you call SuspendInvalidate
        ///// multiple times you need to call ResumeInvalidate also the same number of times.
        ///// </summary>
        //private void SuspendInvalidate()
        //{
        //    suspendInvalidate++;
        //}

        ///// <summary>
        ///// Resumes the invalidation of the controls contents. If there are pending <see cref="Invalidate"/>
        ///// calls then they will be executed at this time and <see cref="OnInvalidated"/> 
        ///// is called. When you call SuspendInvalidate
        ///// multiple times you need to call ResumeInvalidate also the same number of times.
        ///// </summary>
        ///// <param name="forceResumeNow">if set to <c>true</c> this will let you resume operation when
        ///// SuspendInvalidate was called multiple times and you only want to call ResumeInvalidate once.</param>
        //private void ResumeInvalidate(bool forceResumeNow)
        //{
        //    if (suspendInvalidate > 0)
        //        suspendInvalidate--;
        //    if (!IsSuspendedInvalidate || forceResumeNow)
        //    {
        //        if (isInvalidateDirty)
        //            Invalidate(isArrangeDirty);
        //        suspendInvalidate = 0;
        //    }
        //}

        //private void CancelInvalidate()
        //{
        //    suspendInvalidate = 0;
        //    isInvalidateDirty = false;
        //    isArrangeDirty = true;
        //}
        ///// <summary>
        ///// Resumes the invalidation of the controls contents. If there are pending <see cref="Invalidate"/>
        ///// calls then they will be executed at this time and <see cref="OnInvalidated"/> is called. 
        ///// When you call SuspendInvalidate
        ///// multiple times you need to call ResumeInvalidate also the same number of times.
        ///// </summary>
        //private void ResumeInvalidate()
        //{
        //    ResumeInvalidate(false);
        //}

        // <summary>
        // Determines whether <see cref="SuspendInvalidate"/> was called.
        // </summary>
        private bool IsSuspendedInvalidate
        {
            get { return suspendInvalidate > 0; }
        }
        #endregion

        /// <summary>
        /// Called when the <see cref="ScrollControl.InvalidateVisual(System.Boolean)"/> method was called.
        /// </summary>
        /// <param name="isArrangeDirty">if set to <c>true</c> indicates that <see cref="ScrollControl.OnArrangeContent"/>
        /// will be called when control gets updated. Otherwise the OnArrangeContent will be skipped
        /// and only OnRender will be called.</param>
        protected virtual void OnInvalidated(bool isArrangeDirty)
        {
        }
        #endregion
        #region OnArrangeContent
        /// <summary>
        /// Gets a value indicating whether this instance is currently processing <see cref="OnArrangeContent"/> method.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is processing <see cref="OnArrangeContent"/>; otherwise, <c>false</c>.
        /// </value>
        public bool IsInArrangeContent
        {
            get
            {
                return isInArrangeContent;
            }
        }

        /// <summary>
        /// Called after <see cref="ArrangeOverride"/> arranged all child frames.
        /// </summary>
        /// <param name="arrangeSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        protected virtual void OnArrangeContent(Size arrangeSize)
        {
        }

        #endregion
        #region TopLeftFrameExtent, BottomRightFrameExtent
        /// <summary>
        /// Gets or sets the top and left frame extent specifiying the height of the
        /// top frame and the width of the left frame.
        /// </summary>
        /// <value>The top and left frame extent.</value>
        protected Size TopLeftFrameExtent
        {
            get
            {
                return topLeftFrameExtent;
            }
            set
            {
                if (topLeftFrameExtent != value)
                {
                    topLeftFrameExtent = value;
                    InvalidateArrange();
                    OnTopLeftFrameExtentChanged();
                }
            }
        }

        /// <summary>
        /// Called when <see cref="TopLeftFrameExtent"/> was changed.
        /// </summary>
        protected virtual void OnTopLeftFrameExtentChanged()
        {
            ArrangeFrames(RenderSize);
        }

        /// <summary>
        /// Gets or sets the bottom and right frame extent specifiying the height of the
        /// bottom frame and the width of the right frame.
        /// </summary>
        /// <value>The bottom and right frame extent.</value>
        protected Size BottomRightFrameExtent
        {
            get
            {
                return bottomRightFrameExtent;
            }
            set
            {
                if (bottomRightFrameExtent != value)
                {
                    bottomRightFrameExtent = value;
                    if (!isInArrangeContent)
                    {
                        InvalidateArrange();
                        OnBottomRightFrameExtentChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Called when <see cref="BottomRightFrameExtent"/> was changed.
        /// </summary>
        protected virtual void OnBottomRightFrameExtentChanged()
        {
            ArrangeFrames(RenderSize);
        }
        #endregion
        #region Frames
        /// <summary>
        /// Gets the background frame which is placed behind the inner frame. Use this frame
        /// to draw behind the default content. <see cref="Syncfusion.Windows.Controls.Cells.VirtualizingCellsControl"/> uses it to draw
        /// the background of cells in this frame. The frame spans the whole visible area
        /// of the control.
        /// </summary>
        /// <value>The background frame.</value>
        public Panel BackgroundFrame
        {
            get
            {
                return backgroundFrame;
            }
        }

        /// <summary>
        /// Gets the foreground frame which is placed in front of the inner frame. Use this frame
        /// to draw objects in front of the default content. The GridControl uses it to draw
        /// cell selection and current cell border in front of cells using a semi-transparent
        /// color.
        /// </summary>
        /// <value>The background frame.</value>
        public Panel ForegroundFrame
        {
            get
            {
                return foregroundFrame;
            }
        }

        /// <summary>
        /// Gets the inner frame. The children of the innerframe are the
        /// child frames (<see cref="ScrollControlChildFrame"/>) at the top, bottom, left and right of the control.
        /// </summary>
        public Panel InnerFrame
        {
            get { return innerFrame; }
        }

        public Panel ElementFrame
        {
            get { return elementFrame; }
        }

        public Panel GraphicFrame
        {
            get { return graphicFrame; }
        }

        /// <summary>
        /// Gets the specific child frame that matches the given parameters.
        /// </summary>
        /// <param name="isAtTop">if set to <c>true</c> frame is at top.</param>
        /// <param name="isAtLeftSide">if set to <c>true</c> frame is at left side.</param>
        /// <param name="isAtBottom">if set to <c>true</c> frame is at bottom.</param>
        /// <param name="isAtRightSide">if set to <c>true</c> is at right side.</param>
        /// <returns></returns>
        public ScrollControlChildFrame GetChildFrame(bool isAtLeftSide, bool isAtTop, bool isAtRightSide, bool isAtBottom, Panel frame)
        {
            foreach (UIElement visual in frame.Children)
            {
                ScrollControlChildFrame cv = visual as ScrollControlChildFrame;
                if (cv != null && cv.CompareState(isAtLeftSide, isAtTop, isAtRightSide, isAtBottom))
                    return cv;
            }

            return AddChildFrame(isAtLeftSide, isAtTop, isAtRightSide, isAtBottom, frame);
        }

        internal ScrollControlChildFrame AddChildFrame(bool isAtLeftSide, bool isAtTop, bool isAtRightSide, bool isAtBottom, Panel panel)
        {
            ScrollControlChildFrame frame = CreateScrollControlChildFrame();
            frame.SetState(isAtLeftSide, isAtTop, isAtRightSide, isAtBottom);
            //?VisualContainer.SetWantsMouseInput(frame, true);

            //frame.Name = frame.ToString();
            panel.Children.Add(frame);

            return frame;
        }

        /// <summary>
        /// Creates the scroll control child frame object.
        /// </summary>
        /// <returns></returns>
        protected virtual ScrollControlChildFrame CreateScrollControlChildFrame()
        {
            return new ScrollControlChildFrame();
        }
        #endregion
        #region ArrangeFrame
        /// <summary>
        /// Arranges the frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="arrangeRect">The arrange rect.</param>
        /// <param name="r">The r.</param>
        void ArrangeFrame(ScrollControlChildFrame frame, Rect arrangeRect, Rect r)
        {
            ArrangeBounds(frame, arrangeRect);
            Rect clipRect = GetClipRect(arrangeRect, r, frame.IsAtLeftSide, frame.IsAtTop, frame.IsAtRightSide, frame.IsAtBottom);
            ClipRect(frame, clipRect);
        }

        private void ArrangeBounds(ScrollControlChildFrame canvas, Rect arrangeRect)
        {
#if !WinRT
#if DEBUG
            Trace.Write(string.Format("ArrangeBounds: {0} = {1}" , canvas.ToString(), arrangeRect));
#endif
#endif
            canvas.Arrange(arrangeRect);
            canvas.IsArrangeDirty = true;
        }

        private Rect GetClipRect(Rect arrangeRect, Rect innerRect, bool isHeaderColumn, bool isHeaderRow, bool isFooterColumn, bool isFooterRow)
        {
            double top, left, bottom, right;

            if (isHeaderRow)
                top = 0;
            else if (isFooterRow)
                top = innerRect.Bottom;
            else
                top = innerRect.Top;

            if (isFooterRow)
                bottom = arrangeRect.Bottom;
            else if (isHeaderRow)
                bottom = innerRect.Top;
            else
                bottom = innerRect.Bottom;

            if (isHeaderColumn)
                left = 0;
            else if (isFooterColumn)
                left = innerRect.Right;
            else
                left = innerRect.Left;

            if (isFooterColumn)
                right = arrangeRect.Right;
            else if (isHeaderColumn)
                right = innerRect.Left;
            else
                right = innerRect.Right;

            return new Rect(left, top, Math.Max(0, right - left), Math.Max(0, bottom - top));
        }

        public static RectangleGeometry ClipRect(FrameworkElement el, Rect r)
        {
            return GridUtil.SetClipRect(el, r);
        }
        #endregion
        #region Scrollbars

        /// <summary>
        /// Lets you force showing the scrollbar. Will throw exception if not yet hooked up with ScrollOwner. Therefore don't use it in 
        /// derived ctor and instead only use it after instance was fully constructed.
        /// </summary>
        public bool VScroll
        {
            get
            {
                return this.ScrollOwner.VerticalScrollBarVisibility == ScrollBarVisibility.Visible;
            }
            set
            {
                this.ScrollOwner.VerticalScrollBarVisibility = value ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            }
        }

        /// <summary>
        /// Lets you force showing the scrollbar. Will throw exception if not yet hooked up with ScrollOwner. Therefore don't use in 
        /// derived ctor and instead only use it after instance was fully constructed.
        /// </summary>
        public bool HScroll
        {
            get
            {
                return this.ScrollOwner.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible;
            }
            set
            {
                this.ScrollOwner.HorizontalScrollBarVisibility = value ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            }
        }

        #region IScrollBarHost implementation
        FrameworkElement IScrollBarProvider.Element
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the state describing for the horizontal scroll bar.
        /// </summary>
        /// <value>The horizontal scroll bar state.</value>
        public ScrollInfo HScrollBar
        {
            get
            {
                if (_hScrollBar == null)
                {
                    _hScrollBar = new ScrollInfo();
                    WireHScrollBar();
                }
                return _hScrollBar;
            }
        }

        /// <summary>
        /// Gets the state describing for the vertical scroll bar.
        /// </summary>
        /// <value>The vertical scroll bar state.</value>
        public ScrollInfo VScrollBar
        {
            get
            {
                if (_vScrollBar == null)
                {
                    _vScrollBar = new ScrollInfo();
                    WireVScrollBar();
                }
                return _vScrollBar;
            }
        }
        #endregion
        #region Scrollbar events
        private void WireVScrollBar()
        {
            if (!ignoreVScrollBarEvents && _vScrollBar != null)
            {
                _vScrollBar.PropertyChanged += new PropertyChangedEventHandler(_vScrollBar_PropertyChanged);
                _vScrollBar.ValueChanged += new EventHandler(OnVScrollBarValueChanged);
                _vScrollBar.ValueChanging += new ValueChangingEventHandler(OnVScrollBarValueChanging);
            }
        }

        private void UnwireVScrollBar()
        {
            if (_vScrollBar != null)
            {
                _vScrollBar.PropertyChanged -= new PropertyChangedEventHandler(_vScrollBar_PropertyChanged);
                _vScrollBar.ValueChanged -= new EventHandler(OnVScrollBarValueChanged);
                _vScrollBar.ValueChanging -= new ValueChangingEventHandler(OnVScrollBarValueChanging);
            }
        }

        private void WireHScrollBar()
        {
            if (!ignoreHScrollBarEvents && _hScrollBar != null)
            {
                _hScrollBar.PropertyChanged += new PropertyChangedEventHandler(_hScrollBar_PropertyChanged);
                _hScrollBar.ValueChanged += new EventHandler(OnHScrollBarValueChanged);
                _hScrollBar.ValueChanging += new ValueChangingEventHandler(OnHScrollBarValueChanging);
            }
        }

        private void UnwireHScrollBar()
        {
            if (_hScrollBar != null)
            {
                _hScrollBar.PropertyChanged -= new PropertyChangedEventHandler(_hScrollBar_PropertyChanged);
                _hScrollBar.ValueChanged -= new EventHandler(OnHScrollBarValueChanged);
                _hScrollBar.ValueChanging -= new ValueChangingEventHandler(OnHScrollBarValueChanging);
            }
        }

        void _hScrollBar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_owner != null)
                _owner.InvalidateScrollInfo();
        }

        void _vScrollBar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_owner != null)
                _owner.InvalidateScrollInfo();
        }

        /// <summary>
        /// Called before Value property in Vertical scrollbar is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnVScrollBarValueChanging(object sender, ValueChangingEventArgs e)
        {
        }

        /// <summary>
        /// Value property in Vertical scrollbar has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnVScrollBarValueChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("VScrollBar.Value = {0}", VScrollBar.Value);
            InvalidateVisual(true); // Triggers calls to ArrangeOverride and OnRender
            //if (_owner != null)
            //    _owner.InvalidateScrollInfo();
        }

        /// <summary>
        /// Called before Value property in horizontal scrollbar is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHScrollBarValueChanging(object sender, ValueChangingEventArgs e)
        {
        }

        /// <summary>
        /// Value property in Horizontal scrollbar has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHScrollBarValueChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("HScrollBar.Value = {0} {1}", HScrollBar.Value, HScrollBar.Value + HScrollBar.LargeChange);
            InvalidateVisual(true); // Triggers calls to ArrangeOverride and OnRender
            //if (_owner != null)
            //    _owner.InvalidateScrollInfo();
        }

        #region Commented out IgnoreScrollBarEvents
        //public bool IgnoreHScrollBarEvents
        //{
        //    get { return ignoreHScrollBarEvents; }
        //    set
        //    {
        //        if (ignoreHScrollBarEvents != value)
        //        {
        //            UnwireHScrollBar();
        //            ignoreHScrollBarEvents = value;
        //            WireHScrollBar();
        //        }
        //    }
        //}

        //public bool IgnoreVScrollBarEvents
        //{
        //    get { return ignoreVScrollBarEvents; }
        //    set
        //    {
        //        if (ignoreVScrollBarEvents != value)
        //        {
        //            UnwireVScrollBar();
        //            ignoreVScrollBarEvents = value;
        //            WireVScrollBar();
        //        }
        //    }
        //}
        #endregion
        #endregion
        #region IScrollInfo Members

        /// <summary>
        /// Gets or sets a <see cref="System.Windows.Controls.ScrollViewer"/> element that controls scrolling behavior.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="System.Windows.Controls.ScrollViewer"/> element that controls scrolling behavior. This property has no default value.</returns>
        public virtual ScrollableContentViewer ScrollOwner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the horizontal axis is possible.
        /// </summary>
        /// <value></value>
        /// <returns>true if scrolling is possible; otherwise, false. This property has no default value.</returns>
        public virtual bool CanHorizontallyScroll
        {
            get { return ScrollOwner != null && ScrollOwner.CanContentScroll && HScrollBar.Enabled; }
            set { HScrollBar.Enabled = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the vertical axis is possible.
        /// </summary>
        /// <value></value>
        /// <returns>true if scrolling is possible; otherwise, false. This property has no default value.</returns>
        public virtual bool CanVerticallyScroll
        {
            get { return ScrollOwner != null && ScrollOwner.CanContentScroll && VScrollBar.Enabled; }
            set { VScrollBar.Enabled = value; }
        }

        /// <summary>
        /// Gets the horizontal offset of the scrolled content.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="System.Double"/> that represents, in device independent pixels, the horizontal offset. This property has no default value.</returns>
        public virtual double HorizontalOffset
        {
            get { return HScrollBar.Value - HScrollBar.Minimum; }
        }

        /// <summary>
        /// Gets the vertical offset of the scrolled content.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="System.Double"/> that represents, in device independent pixels, the vertical offset of the scrolled content. Valid values are between zero and the <see cref="System.Windows.Controls.Primitives.IScrollInfo.ExtentHeight"/> minus the <see cref="System.Windows.Controls.Primitives.IScrollInfo.ViewportHeight"/>. This property has no default value.</returns>
        public virtual double VerticalOffset
        {
            get { return VScrollBar.Value - VScrollBar.Minimum; }
        }

        /// <summary>
        /// Gets the horizontal size of the extent.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="System.Double"/> that represents, in device independent pixels, the horizontal size of the extent. This property has no default value.</returns>
        public virtual double ExtentWidth
        {
            get { return HScrollBar.Maximum - HScrollBar.Minimum + 1; }
        }

        /// <summary>
        /// Gets the vertical size of the extent.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="System.Double"/> that represents, in device independent pixels, the vertical size of the extent.This property has no default value.</returns>
        public virtual double ExtentHeight
        {
            get { return VScrollBar.Maximum - VScrollBar.Minimum + 1; }
        }

        /// <summary>
        /// Gets the horizontal size of the viewport for this content.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="System.Double"/> that represents, in device independent pixels, the vertical size of the viewport for this content. This property has no default value.</returns>
        public virtual double ViewportWidth
        {
            get { return HScrollBar.LargeChange; }
        }

        /// <summary>
        /// Gets the vertical size of the viewport for this content.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="System.Double"/> that represents, in device independent pixels, the vertical size of the viewport for this content. This property has no default value.</returns>
        public virtual double ViewportHeight
        {
            get { return VScrollBar.LargeChange; }
        }

        /// <summary>
        /// Scrolls up within content by one logical unit.
        /// </summary>
        public virtual void LineUp()
        {
            VScrollBar.Value -= VScrollBar.SmallChange;
        }

        /// <summary>
        /// Scrolls down within content by one logical unit.
        /// </summary>
        public virtual void LineDown()
        {
            VScrollBar.Value += VScrollBar.SmallChange;
        }

        /// <summary>
        /// Scrolls left within content by one logical unit.
        /// </summary>
        public virtual void LineLeft()
        {
            HScrollBar.Value -= HScrollBar.SmallChange;
        }

        /// <summary>
        /// Scrolls right within content by one logical unit.
        /// </summary>
        public virtual void LineRight()
        {
            HScrollBar.Value += HScrollBar.SmallChange;
        }

        /// <summary>
        /// Scrolls up within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public virtual void MouseWheelUp()
        {
            LineUp();
        }

        /// <summary>
        /// Scrolls down within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public virtual void MouseWheelDown()
        {
            LineDown();
        }

        /// <summary>
        /// Scrolls left within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public virtual void MouseWheelLeft()
        {
            LineLeft();
        }

        /// <summary>
        /// Scrolls right within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public virtual void MouseWheelRight()
        {
            LineRight();
        }

        /// <summary>
        /// Scrolls up within content by one page.
        /// </summary>
        public virtual void PageUp()
        {
            VScrollBar.Value -= Math.Max(VScrollBar.SmallChange, VScrollBar.LargeChange - VScrollBar.SmallChange);
        }

        /// <summary>
        /// Scrolls down within content by one page.
        /// </summary>
        public virtual void PageDown()
        {
            VScrollBar.Value += Math.Max(VScrollBar.SmallChange, VScrollBar.LargeChange - VScrollBar.SmallChange);
        }

        /// <summary>
        /// Scrolls left within content by one page.
        /// </summary>
        public virtual void PageLeft()
        {
            HScrollBar.Value -= Math.Max(HScrollBar.SmallChange, HScrollBar.LargeChange - HScrollBar.SmallChange);
        }

        /// <summary>
        /// Scrolls right within content by one page.
        /// </summary>
        public virtual void PageRight()
        {
            HScrollBar.Value += Math.Max(HScrollBar.SmallChange, HScrollBar.LargeChange - HScrollBar.SmallChange);
        }

        /// <summary>
        /// Sets the amount of vertical offset.
        /// </summary>
        /// <param name="offset">The degree to which content is vertically offset from the containing viewport.</param>
        public virtual void SetVerticalOffset(double offset)
        {
            VScrollBar.Value = offset + VScrollBar.Minimum;
        }

        /// <summary>
        /// Sets the amount of horizontal offset.
        /// </summary>
        /// <param name="offset">The degree to which content is horizontally offset from the containing viewport.</param>
        public virtual void SetHorizontalOffset(double offset)
        {
            HScrollBar.Value = offset + HScrollBar.Minimum;
        }

        #endregion
        #region Scrollbar Shadow
        /// <summary>
        /// Gets a shadowed copy of the <see cref="HScrollBar"/> property. The object is created first thing
        /// in the ArrangeOverride method.
        /// </summary>
        /// <value>The H scroll bar shadow.</value>
        public ScrollInfo HScrollBarShadow
        {
            get
            {
                if (_hScrollBarCopy == null)
                    _hScrollBarCopy = HScrollBar.Clone();
                return _hScrollBarCopy;
            }
        }

        /// <summary>
        /// Gets a shadowed copy of the <see cref="VScrollBar"/> property. The object is created first thing
        /// in the ArrangeOverride method.
        /// </summary>
        /// <value>The H scroll bar shadow.</value>
        public ScrollInfo VScrollBarShadow
        {
            get
            {
                if (_vScrollBarCopy == null)
                    _vScrollBarCopy = VScrollBar.Clone();
                return _vScrollBarCopy;
            }
        }

        #endregion
        #endregion

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
        }

#if !WinRT
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (mouseControllerDispatcher != null)
            {
                MouseControllerDispatcher.OnMouseEnter(e);
                base.OnMouseEnter(e);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (mouseControllerDispatcher != null)
            {
                MouseControllerDispatcher.OnMouseLeave(e);
                base.OnMouseLeave(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseControllerDispatcher != null)
            {
                MouseControllerDispatcher.OnMouseMove(e);
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (mouseControllerDispatcher != null)
            {
                MouseControllerDispatcher.OnMouseDown(e);
                base.OnMouseLeftButtonDown(e);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (mouseControllerDispatcher != null)
            {
                base.OnMouseLeftButtonUp(e);
                MouseControllerDispatcher.OnMouseUp(e);
            }
        }
#else
        #region WinRT Code

        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            mouseControllerDispatcher.OnPointerPressed(e);
            base.OnPointerPressed(e);
        }
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            mouseControllerDispatcher.OnPointerReleased(e);
        }
        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            mouseControllerDispatcher.PointerMove(e);
        }

        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
        }

        #endregion
#endif
        public void DisposePanel(Panel frame)
        {
            foreach (var item in frame.Children)
            {
                if (item is ScrollControlChildFrame)
                {
                    (item as ScrollControlChildFrame).Dispose();
                }
            }
        }

        public virtual void Dispose()
        {
            dispose();
            this.Loaded -= new RoutedEventHandler(this_Loaded);
            this.Unloaded -= new RoutedEventHandler(ScrollControl_Unloaded);
            this.SizeChanged -= new SizeChangedEventHandler(ScrollControl_SizeChanged);
            if (mouseControllerDispatcher != null)
            {
                mouseControllerDispatcher.Dispose();
                mouseControllerDispatcher = null;
            }
            if (elementFrame != null)
            {
                DisposePanel(elementFrame);
                elementFrame = null;
            }
            if (innerFrame != null)
            {
                DisposePanel(innerFrame);
                innerFrame = null;
            }
            if (backgroundFrame != null)
            {
                DisposePanel(backgroundFrame);
                backgroundFrame = null;
            }
            if (foregroundFrame != null)
            {
                DisposePanel(foregroundFrame);
                foregroundFrame = null;
            }
            if (this._hScrollBar != null)
                this._hScrollBar = null;
            if (this._vScrollBar != null)
                this._vScrollBar = null;
            if (this._hScrollBarCopy != null)
                this._hScrollBarCopy = null;
            if (this._vScrollBarCopy != null)
                this._vScrollBarCopy = null;
            if (this.autoScroller != null)
            {
                this.autoScroller.Dispose();
                this.autoScroller = null;
            }
            if (_owner != null)
            {
                this._owner.Dispose();
                this._owner = null;
            }
            this.Children.Clear();
        }

        // dispose differing only in case is not CLS-compliant
        protected virtual void dispose()
        {

        }

    }
}