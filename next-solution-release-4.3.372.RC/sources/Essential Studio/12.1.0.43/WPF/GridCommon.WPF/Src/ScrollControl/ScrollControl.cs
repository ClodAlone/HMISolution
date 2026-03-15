#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Scroll
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
    public class ScrollControl : VisualContainer, IScrollBarProvider, IDisposable
    {
        #region Fields
        // Fields
        ScrollViewer _owner;
        ScrollInfo _hScrollBar;
        ScrollInfo _vScrollBar;
        ScrollInfo _hScrollBarCopy;
        ScrollInfo _vScrollBarCopy;
        internal bool ignoreHScrollBarEvents = false;
        internal bool ignoreVScrollBarEvents = false;
        bool isInArrangeContent = false;
        Size lastArrangeSize;

        int suspendInvalidate = 0;
        //bool isInvalidateDirty = false;
        bool isArrangeDirty = true;
        bool invalidateArrangeOnLoaded = false;

        VisualContainer backgroundFrame;
        VisualContainer foregroundFrame;
        VisualContainer innerFrame;
        VisualContainer elementsFrame;
        VisualContainer graphicFrame;

        Size topLeftFrameExtent = new Size(0, 0);
        Size bottomRightFrameExtent = new Size(0, 0);
        AutoScroller autoScroller;

        VisualCollection _children;

        MouseEventTargetCollection mouseEventsListeners = new MouseEventTargetCollection();

        #region ZoomScale

        private ScaleTransform ScaleTransform = null;

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

        public static readonly DependencyProperty ZoomScaleProperty =
               DependencyProperty.Register("ZoomScale", typeof(double), typeof(ScrollControl), new PropertyMetadata(1.0, ZoomScale_Changed));

        private static void ZoomScale_Changed(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScrollControl c = (ScrollControl)o;

            //ScaleTransform
            if (c.ScaleTransform != null)
            {
                c.ScaleTransform.ScaleX = c.ZoomScale;
                c.ScaleTransform.ScaleY = c.ZoomScale;
            }
        }

        #endregion
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
            RegisterEvents();
            _children = new VisualCollection(this);
            InitializeChildFrames();

            //ScaleTransform
            this.ScaleTransform = new ScaleTransform() { ScaleX = ZoomScale, ScaleY = ZoomScale };
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(this.ScaleTransform);
            this.LayoutTransform = transformGroup;

            this.Loaded += new RoutedEventHandler(this_Loaded);
            this.Unloaded += new RoutedEventHandler(this_Unloaded);
            InitializeMouseEventListeners();
        }

        void this_Loaded(object sender, RoutedEventArgs e)
        {
            OnLoaded(e);
            if (invalidateArrangeOnLoaded)
                InvalidateVisual(true);
        }

        void this_Unloaded(object sender, RoutedEventArgs e)
        {
            OnUnloaded(e);
            invalidateArrangeOnLoaded = true;
        }

        /// <summary>
        /// This virtual method is called from the <see cref="FrameworkElement.Loaded"/> event handler.<para/>
        /// Do not override this method to wire events since a control can be unloaded and loaded 
        /// multiple times during its lifetime.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnLoaded(RoutedEventArgs e)
        {
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
            elementsFrame = new VisualContainer("ElementsFrame");
            foregroundFrame = new VisualContainer("ForegroundFrame");
            graphicFrame = new VisualContainer("GraphicFrame");

            Children.Add(backgroundFrame);
            Children.Add(innerFrame);
            Children.Add(elementsFrame);
            Children.Add(foregroundFrame);
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
            VisualContainer.SetWantsMouseInput(elementsFrame, false);
            VisualContainer.SetWantsMouseInput(foregroundFrame, false);

            // ScrollBody
            AddChildFrame(false, false, false, false, this.ElementsFrame);
            AddChildFrame(false, false, false, false, this.InnerFrame);
        }
        #endregion
        #region Measure, ArrangeOverride and Render
        /// <summary>
        /// Draws a transparent rectangle across the whole area.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            //this.isInvalidateDirty = false;

            // I originally need to do this in order to receive mouse preview messages, but
            // overriding HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
            // below also took care of it and this seems the better solution.
            //drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
        }

        //protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        //{
        //    // Not sure if I should override/modify this like HitTestCore(PointHitTestParameters hitTestParameters)
        //    // below.
        //    return base.HitTestCore(hitTestParameters);
        //}

        /// <summary>
        /// Implements <see cref="System.Windows.Media.Visual.HitTestCore(System.Windows.Media.PointHitTestParameters)"/> to supply base element hit testing behavior (returning <see cref="T:System.Windows.Media.HitTestResult"/>).
        /// </summary>
        /// <param name="hitTestParameters">Describes the hit test to perform, including the initial hit point.</param>
        /// <returns>
        /// Results of the test, including the evaluated point.
        /// </returns>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            // base class checks for _drawingContent != null (VisualTreeHelper.GetDrawing). We do not want this, we
            // always want the control to receive mouse preview messages.
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        /// <summary>
        /// Arranges all child frames. Each frames <see cref="Canvas.LeftProperty"/>, <see cref="Canvas.TopProperty"/>, <see cref="Canvas.RightProperty"/> and <see cref="Canvas.BottomProperty"/> properties are initialized. <see cref="UIElement.Arrange"/> is called and a the <see cref="UIElement.Clip"/> property is set. After all child frames were arranged the virtual <see cref="OnArrangeContent"/> method is called.
        /// </summary>
        /// <param name="arrangeSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>Same size as given in arrangeSize.</returns>
        protected sealed override Size ArrangeOverride(Size arrangeSize)
        {
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

                if (!isArrangeDirty && arrangeSize == lastArrangeSize)
                {
#if SILVERLIGHT
                    // need to avoid that grid lines and cell backgrounds disappear in silverlight.
                    AutoArrangeRecursive(Content as Panel);
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

#if SILVERLIGHT
        private void AutoArrangeRecursive(Panel panel)
        {
            foreach (UIElement el in panel.Children)
            {
                if (el is Panel)
                    AutoArrangeRecursive((Panel) el);
                else
                {
                    Rect r = VisualContainer.GetRenderBounds(el);
                    if (!r.IsEmpty)
                        el.Arrange(r);
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
            if (delayInvalidateScrollInfo)
            {
                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
                delayInvalidateScrollInfo = false;
            }
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

            foreach (UIElement visual in innerFrame.Children)
            {
                ScrollControlChildFrame frame = visual as ScrollControlChildFrame;
                if (frame != null)
                    ArrangeFrame(frame, arrangeRect, r);
            }

            foreach (UIElement visual in elementsFrame.Children)
            {
                ScrollControlChildFrame frame = visual as ScrollControlChildFrame;
                if (frame != null)
                    ArrangeFrame(frame, arrangeRect, r);
            }

            foreach (UIElement visual in graphicFrame.Children)
            {
                ScrollControlChildFrame frame = visual as ScrollControlChildFrame;
                if (frame != null)
                    ArrangeFrame(frame, arrangeRect, r);
            }
        }

        internal void ArrangeFrames(Size arrangeSize, IList<Visual> uiElements)
        {
            Rect arrangeRect = new Rect(new Point(0, 0), arrangeSize);

            if (topLeftFrameExtent.IsEmpty)
                topLeftFrameExtent = new Size(0, 0);

            if (bottomRightFrameExtent.IsEmpty)
                bottomRightFrameExtent = new Size(0, 0);

            Rect r = new Rect(topLeftFrameExtent.Width, topLeftFrameExtent.Height,
                  Math.Max(0, arrangeSize.Width - topLeftFrameExtent.Width - bottomRightFrameExtent.Width),
                  Math.Max(0, arrangeSize.Height - topLeftFrameExtent.Height - bottomRightFrameExtent.Height));

            foreach (UIElement visual in uiElements)
            {
                ScrollControlChildFrame frame = visual as ScrollControlChildFrame;
                if (frame != null)
                    ArrangeFrame(frame, arrangeRect, r);
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

        /// <summary>
        /// Calls the virtuals <see cref="OnArrangeContent"/> method and raises the <see cref="System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data 
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            //OnArrangeContent(sizeInfo.NewSize);
            InvalidateVisual(true);

            // NOTE: OnRenderSizeChanged is called immeditately after a ArrangeOverride call but the
            // size passed to ArrangeOverride is sometimes not correct. Explicitly handling
            // OnRenderSizeChanged in such case seems the better option.

            base.OnRenderSizeChanged(sizeInfo);
        }
        #endregion
        #region Invalidate

        /// <summary>
        /// Invalidates the rendering of the element, and forces a complete new layout pass. <see cref="System.Windows.UIElement.OnRender(System.Windows.Media.DrawingContext)"/> is called after the layout cycle is completed.
        /// The method is overloaded in <see cref="ScrollControl"/> to redirect its call to 
        /// <see cref="InvalidateVisual(System.Boolean)"/> with setArrangeDirty being true. 
        /// </summary>
        public new void InvalidateVisual()
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
            base.InvalidateVisual();
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
        public VisualContainer BackgroundFrame
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
        public VisualContainer ForegroundFrame
        {
            get
            {
                return foregroundFrame;
            }
        }

        public VisualContainer ElementsFrame
        {
            get { return elementsFrame; }
        }

        /// <summary>
        /// Gets the inner frame. The children of the innerframe are the
        /// child frames (<see cref="ScrollControlChildFrame"/>) at the top, bottom, left and right of the control.
        /// </summary>
        public VisualContainer InnerFrame
        {
            get { return innerFrame; }
        }

        public VisualContainer GraphicFrame
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
        public ScrollControlChildFrame GetChildFrame(bool isAtLeftSide, bool isAtTop, bool isAtRightSide, bool isAtBottom, VisualContainer frame)
        {
            foreach (UIElement visual in frame.Children)
            {
                ScrollControlChildFrame cv = visual as ScrollControlChildFrame;
                if (cv != null && cv.CompareState(isAtLeftSide, isAtTop, isAtRightSide, isAtBottom))
                    return cv;
            }



            return AddChildFrame(isAtLeftSide, isAtTop, isAtRightSide, isAtBottom, frame);
        }

        ScrollControlChildFrame AddChildFrame(bool isAtLeftSide, bool isAtTop, bool isAtRightSide, bool isAtBottom, VisualContainer frame)
        {
            ScrollControlChildFrame scrollControlChildframe = CreateScrollControlChildFrame();
            scrollControlChildframe.SetState(isAtLeftSide, isAtTop, isAtRightSide, isAtBottom);
            //?VisualContainer.SetWantsMouseInput(frame, true);

            this.SetFrameProperties(frame, scrollControlChildframe);

            frame.Children.Add(scrollControlChildframe);
            //innerFrame.Children.Add(frame);

            return scrollControlChildframe;
        }

        protected virtual void SetFrameProperties(VisualContainer parent, ScrollControlChildFrame child)
        {

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
            SetClipRect(frame, clipRect);
        }

        private void ArrangeBounds(ScrollControlChildFrame canvas, Rect arrangeRect)
        {
            canvas.Arrange(arrangeRect);
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

        static RectangleGeometry SetClipRect(UIElement el, Rect r)
        {
            return GridUtil.SetClipRect(el, r);
        }
        #endregion
        #region Extra VisualChildren

        /// <summary>
        /// Gets a separate collection of visual children that is maintained
        /// independently from <see cref="VisualContainer.Children"/>. You can add and remove Visuals 
        /// through this collection without affecting the order of the <see cref="VisualContainer.Children"/>
        /// collection.  Adding and removing elements through the <see cref="VisualChildren"/>
        /// collection does not trigger calls to InvalidateMeasure. 
        /// </summary>
        /// <value>The extra visual children.</value>
        public VisualCollection VisualChildren
        {
            get
            {
                return this._children;
            }
        }

        // Provide a required override for the VisualChildrenCount property.
        /// <summary>
        /// Gets the visual children count.
        /// </summary>
        /// <value>The visual children count.</value>
        protected override int VisualChildrenCount
        {
            get { return base.VisualChildrenCount + _children.Count; }
        }

        // Provide a required override for the GetVisualChild method.
        /// <summary>
        /// Gets the visual child at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            int baseVisualChildrenCount = base.VisualChildrenCount;
            if (index < baseVisualChildrenCount)
                return base.GetVisualChild(index);

            index -= baseVisualChildrenCount;
            if (index < 0 || index > _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
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
            //if (_owner != null)
            //    _owner.InvalidateScrollInfo();
            delayInvalidateScrollInfo = true;
        }

        bool _delayInvalidateScrollInfo;

        internal bool delayInvalidateScrollInfo
        {
            get { return _delayInvalidateScrollInfo; }
            set
            {
                if (_delayInvalidateScrollInfo != value)
                {
                    _delayInvalidateScrollInfo = value;
                    if (value)
                        Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if (_delayInvalidateScrollInfo && _owner != null)
                                    _owner.InvalidateScrollInfo();
                            }));
                }
            }
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
        public virtual ScrollViewer ScrollOwner
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
            // conversion to float helps prevent rounding errors later.
            VScrollBar.Value = (float)offset + VScrollBar.Minimum;
        }

        /// <summary>
        /// Sets the amount of horizontal offset.
        /// </summary>
        /// <param name="offset">The degree to which content is horizontally offset from the containing viewport.</param>
        public virtual void SetHorizontalOffset(double offset)
        {
            // conversion to float helps prevent rounding errors later.
            HScrollBar.Value = (float)offset + HScrollBar.Minimum;
        }

        /// <summary>
        /// Override this method to scroll until the coordinate space of a <see cref="System.Windows.Media.Visual"/> object is visible.
        /// The default implementation of this method does not perform any scrolling.
        /// </summary>
        /// <param name="visual">A <see cref="System.Windows.Media.Visual"/> that becomes visible.</param>
        /// <param name="rectangle">A bounding rectangle that identifies the coordinate space to make visible.</param>
        /// <returns>
        /// A <see cref="System.Windows.Rect"/> that is visible.
        /// </returns>
        public virtual Rect MakeVisible(Visual visual, Rect rectangle)
        {
            ////Vector bounds = VisualTreeHelper.GetOffset(visual);
            //DependencyObject dpo = VisualTreeHelper.GetParent(visual);
            //while (dpo != null)
            //{
            //    visual = (Visual)dpo;
            //    Console.WriteLine(VisualTreeHelper.GetOffset(visual));
            //    dpo = VisualTreeHelper.GetParent(visual);
            //}
            // TODO: Implementation of this method will be good
            // for large covered cell scenarios.
            return rectangle;
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
        #region MouseEventListeners
        private void InitializeMouseEventListeners()
        {
            autoScroller = new AutoScroller(this);
            this.mouseEventsListeners.SetHost(this);
            //this.MouseEventListeners.Add(autoScroller);
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

        /// <summary>
        /// Gets the collection of mouse event listeners. Mouse events will be forwarded to the 
        /// objects to this collection.
        /// </summary>
        /// <value>The mouse event listeners.</value>
        public List<IMouseEventsTarget> MouseEventListeners
        {
            get
            {
                return this.mouseEventsListeners;
            }
        }

        #region Mouse Event Forwarders
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseEnterEvent"/>�attached event is raised on this element. 
        /// The implementation of this method forwards the event to all objects in the <see cref="MouseEventListeners"/> collection.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (mouseEventsListeners != null)
                mouseEventsListeners.OnMouseEnter(e);
            if (e.Handled)
                return;
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseLeaveEvent"/>�attached event is raised on this element.
        /// The implementation of this method forwards the event to all objects in the <see cref="MouseEventListeners"/> collection.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (mouseEventsListeners != null)
                mouseEventsListeners.OnMouseLeave(e);
            if (e.Handled)
                return;
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseDownEvent"/>�attached event reaches an element in its route that is derived from this class.
        /// The implementation of this method forwards the event to all objects in the <see cref="MouseEventListeners"/> collection.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            ScrollControlMouseButtonEventArgs newEventArgs = new ScrollControlMouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice);
            newEventArgs.RoutedEvent = ScrollControlMouseDownEvent;
            RaiseEvent(newEventArgs);
            if (newEventArgs.Handled)
            {
                e.Handled = true;
                return;
            }
            if (newEventArgs.SkipListeners) return;

            if (e.MiddleButton != MouseButtonState.Pressed)
            {
                if (mouseEventsListeners != null)
                    mouseEventsListeners.OnMouseDown(e);
                if (e.Handled)
                    return;
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseMoveEvent"/>�attached event reaches an element in its route that is derived from this class.
        /// The implementation of this method forwards the event to all objects in the <see cref="MouseEventListeners"/> collection.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            ScrollControlMouseEventArgs newEventArgs = new ScrollControlMouseEventArgs(e.MouseDevice, e.Timestamp, e.StylusDevice);
            newEventArgs.RoutedEvent = ScrollControlMouseMoveEvent;
            RaiseEvent(newEventArgs);
            if (newEventArgs.Handled)
            {
                e.Handled = true;
                return;
            }
            if (newEventArgs.SkipListeners) return;

            if (e.MiddleButton != MouseButtonState.Pressed)
            {
                if (mouseEventsListeners != null)
                    mouseEventsListeners.OnMouseMove(e);
                if (e.Handled)
                    return;
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseUpEvent"/>�routed event reaches an element in its route that is derived from this class.
        /// The implementation of this method forwards the event to all objects in the <see cref="MouseEventListeners"/> collection.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            ScrollControlMouseButtonEventArgs newEventArgs = new ScrollControlMouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice);
            newEventArgs.RoutedEvent = ScrollControlMouseUpEvent;
            RaiseEvent(newEventArgs);
            if (newEventArgs.Handled)
            {
                e.Handled = true;
                return;
            }
            if (newEventArgs.SkipListeners) return;

            if (e.MiddleButton != MouseButtonState.Pressed)
            {
                if (mouseEventsListeners != null)
                    mouseEventsListeners.OnMouseUp(e);

                if (e.Handled)
                    return;
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseWheelEvent"/>�attached event reaches an element in its route that is derived from this class. 
        /// The implementation of this method forwards the event to all objects in the <see cref="MouseEventListeners"/> collection.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (mouseEventsListeners != null)
                mouseEventsListeners.OnMouseWheel(e);
            if (e.Handled)
                return;
            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.PreviewMouseWheelEvent"/>�attached event reaches an element in its route that is derived from this class.
        /// 
        /// The implementation of this method forwards the event to all objects in the <see cref="MouseEventListeners"/> collection.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);
            if (e.Handled)
                return;
            if (mouseEventsListeners != null)
                mouseEventsListeners.OnPreviewMouseWheel(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.PreviewMouseDownEvent"/> attached�routed event reaches an element in its route that is derived from this class.
        /// The implementation of this method forwards the event to all objects in the <see cref="MouseEventListeners"/> collection.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that one or more mouse buttons were pressed.</param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            ScrollControlMouseButtonEventArgs newEventArgs = new ScrollControlMouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice);
            newEventArgs.RoutedEvent = ScrollControlPreviewMouseDownEvent;
            RaiseEvent(newEventArgs);
            if (newEventArgs.Handled)
            {
                e.Handled = true;
                return;
            }
            if (newEventArgs.SkipListeners) return;

            base.OnPreviewMouseDown(e);

            if (e.MiddleButton != MouseButtonState.Pressed)
            {
                if (e.Handled)
                    return;

                // Take care of such cases when a combobox is dropped down and you click
                // inside another cell. In such case the combobox should handle the click
                // (and in that case close the dropped list.)
                if (e.MouseDevice.Captured != null)
                    return;

                // Take care of such cases when moving mouse over a popup window. In such
                // we should not intercept messages. The popup should get them.
                if (!GridUtil.IsObjectDescendantOfParent(this, e.OriginalSource as DependencyObject))
                    return;

                if (mouseEventsListeners != null)
                    mouseEventsListeners.OnPreviewMouseDown(e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.PreviewMouseMoveEvent"/>�attached event reaches an element in its route that is derived from this class. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            ScrollControlMouseEventArgs newEventArgs = new ScrollControlMouseEventArgs(e.MouseDevice, e.Timestamp, e.StylusDevice);
            newEventArgs.RoutedEvent = ScrollControlPreviewMouseMoveEvent;
            RaiseEvent(newEventArgs);
            if (newEventArgs.Handled)
            {
                e.Handled = true;
                return;
            }
            if (newEventArgs.SkipListeners) return;

            base.OnPreviewMouseMove(e);
            if (e.MiddleButton != MouseButtonState.Pressed)
            {
                if (e.Handled)
                    return;

                // Results of testing if the two conditions below
                // should only be checked for MouseDown scenario, or for MouseMove also 
                // are:
                // 1) Checking for Captured is wrong. This should only be checked on MouseDown.
                // With MouseMove it will interfere with clicking inside textbox
                // and then trying to select cells by dragging mouse outside textbox. 
                // 2) Checking for !IsObjectDescendantOfParent is necessary. Otherwise MouseMove messages
                // get forwarded when you move mouse over popup list in a dropped down
                // combobox.

                // Take care of such cases when a combobox is dropped down and you click
                // inside another cell. In such case the combobox should handle the click
                // (and in that case close the dropped list.)
                //if (e.MouseDevice.Captured != null)
                //    return;

                // Take care of such cases when moving mouse over a popup window. In such
                // we should not intercept messages. The popup should get them.
                if (!GridUtil.IsObjectDescendantOfParent(this, e.OriginalSource as DependencyObject))
                    return;

                if (mouseEventsListeners != null)
                    mouseEventsListeners.OnPreviewMouseMove(e);
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (mouseEventsListeners != null)
            {
                mouseEventsListeners.OnDrop(e);
            }
        }
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.PreviewMouseUpEvent"/>�attached event reaches an element in its route that is derived from this class. 
        /// The implementation of this method forwards the event to all objects in the <see cref="MouseEventListeners"/> collection.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that one or more mouse buttons were released.</param>
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            ScrollControlMouseButtonEventArgs newEventArgs = new ScrollControlMouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice);
            newEventArgs.RoutedEvent = ScrollControlPreviewMouseUpEvent;
            RaiseEvent(newEventArgs);
            if (newEventArgs.Handled)
            {
                e.Handled = true;
                return;
            }
            if (newEventArgs.SkipListeners) return;

            base.OnPreviewMouseUp(e);
            if (e.MiddleButton != MouseButtonState.Pressed)
            {
                if (e.Handled)
                    return;

                // Take care of such cases when releasing mouse button over a popup window. In such
                // we should not intercept messages. The popup should get them.
                if (!GridUtil.IsObjectDescendantOfParent(this, e.OriginalSource as DependencyObject))
                    return;

                if (mouseEventsListeners != null)
                    mouseEventsListeners.OnPreviewMouseUp(e);
            }
        }
        #endregion
        #region Custom ScrollControl Mouse Events
        void RegisterEvents()
        {
            this.ScrollControlPreviewMouseDown += new ScrollControlMouseButtonEventHandler(OnScrollControlPreviewMouseDownThunk);
            this.ScrollControlPreviewMouseUp += new ScrollControlMouseButtonEventHandler(OnScrollControlPreviewMouseUpThunk);
            this.ScrollControlPreviewMouseMove += new ScrollControlMouseEventHandler(OnScrollControlPreviewMouseMoveThunk);
            this.ScrollControlMouseDown += new ScrollControlMouseButtonEventHandler(OnScrollControlMouseDownThunk);
            this.ScrollControlMouseUp += new ScrollControlMouseButtonEventHandler(OnScrollControlMouseUpThunk);
            this.ScrollControlMouseMove += new ScrollControlMouseEventHandler(OnScrollControlMouseMoveThunk);
            //EventManager.RegisterClassHandler(typeof(ScrollControl), ScrollControlPreviewMouseDownEvent,
            //    new ScrollControlMouseButtonEventHandler(OnScrollControlPreviewMouseDownThunk), false);
            //EventManager.RegisterClassHandler(typeof(ScrollControl), ScrollControlPreviewMouseUpEvent,
            //    new ScrollControlMouseButtonEventHandler(OnScrollControlPreviewMouseUpThunk), false);
            //EventManager.RegisterClassHandler(typeof(ScrollControl), ScrollControlPreviewMouseMoveEvent,
            //    new ScrollControlMouseEventHandler(OnScrollControlPreviewMouseMoveThunk), false);
            //EventManager.RegisterClassHandler(typeof(ScrollControl), ScrollControlMouseDownEvent,
            //    new ScrollControlMouseButtonEventHandler(OnScrollControlMouseDownThunk), false);
            //EventManager.RegisterClassHandler(typeof(ScrollControl), ScrollControlMouseUpEvent,
            //    new ScrollControlMouseButtonEventHandler(OnScrollControlMouseUpThunk), false);
            //EventManager.RegisterClassHandler(typeof(ScrollControl), ScrollControlMouseMoveEvent,
            //    new ScrollControlMouseEventHandler(OnScrollControlMouseMoveThunk), false);

            // TODO: xml comments
        }

        #region ScrollControlMouseMoveEvent
        private static void OnScrollControlMouseMoveThunk(object sender, ScrollControlMouseEventArgs e)
        {
            ((ScrollControl)sender).OnScrollControlMouseMove(e);
        }

        /// <summary> 
        ///     Declaration of the routed event reporting the mouse was pressed
        /// </summary> 
        public static readonly RoutedEvent ScrollControlMouseMoveEvent = EventManager.RegisterRoutedEvent("ScrollControlMouseMove",
            RoutingStrategy.Direct, typeof(ScrollControlMouseEventHandler), typeof(ScrollControl));

        /// <summary> 
        ///     Event reporting the mouse  was pressed
        /// </summary>
        public event ScrollControlMouseEventHandler ScrollControlMouseMove
        {
            add { AddHandler(ScrollControl.ScrollControlMouseMoveEvent, value, false); }
            remove { RemoveHandler(ScrollControl.ScrollControlMouseMoveEvent, value); }
        }

        /// <summary> 
        ///     Virtual method reporting the mouse  was pressed
        /// </summary>
        protected virtual void OnScrollControlMouseMove(ScrollControlMouseEventArgs e)
        {
        }
        #endregion
        #region ScrollControlMouseUp
        private static void OnScrollControlMouseUpThunk(object sender, ScrollControlMouseButtonEventArgs e)
        {
            ((ScrollControl)sender).OnScrollControlMouseUp(e);
        }

        /// <summary> 
        ///     Declaration of the routed event reporting the mouse button was pressed
        /// </summary> 
        public static readonly RoutedEvent ScrollControlMouseUpEvent = EventManager.RegisterRoutedEvent("ScrollControlMouseUp",
            RoutingStrategy.Direct, typeof(ScrollControlMouseButtonEventHandler), typeof(ScrollControl));

        /// <summary> 
        ///     Event reporting the mouse button was pressed
        /// </summary>
        public event ScrollControlMouseButtonEventHandler ScrollControlMouseUp
        {
            add { AddHandler(ScrollControl.ScrollControlMouseUpEvent, value, false); }
            remove { RemoveHandler(ScrollControl.ScrollControlMouseUpEvent, value); }
        }

        /// <summary> 
        ///     Virtual method reporting the mouse button was pressed
        /// </summary>
        protected virtual void OnScrollControlMouseUp(ScrollControlMouseButtonEventArgs e)
        {
        }
        #endregion
        #region ScrollControlMouseDown

        private static void OnScrollControlMouseDownThunk(object sender, ScrollControlMouseButtonEventArgs e)
        {
            ((ScrollControl)sender).OnScrollControlMouseDown(e);
        }

        /// <summary> 
        ///     Declaration of the routed event reporting the mouse button was pressed
        /// </summary> 
        public static readonly RoutedEvent ScrollControlMouseDownEvent = EventManager.RegisterRoutedEvent("ScrollControlMouseDown",
            RoutingStrategy.Direct, typeof(ScrollControlMouseButtonEventHandler), typeof(ScrollControl));

        /// <summary> 
        ///     Event reporting the mouse button was pressed
        /// </summary>
        public event ScrollControlMouseButtonEventHandler ScrollControlMouseDown
        {
            add { AddHandler(ScrollControl.ScrollControlMouseDownEvent, value, false); }
            remove { RemoveHandler(ScrollControl.ScrollControlMouseDownEvent, value); }
        }

        /// <summary> 
        ///     Virtual method reporting the mouse button was pressed
        /// </summary>
        protected virtual void OnScrollControlMouseDown(ScrollControlMouseButtonEventArgs e)
        {
        }
        #endregion
        #region ScrollControlPreviewMouseMove

        private static void OnScrollControlPreviewMouseMoveThunk(object sender, ScrollControlMouseEventArgs e)
        {
            ((ScrollControl)sender).OnScrollControlPreviewMouseMove(e);
        }

        /// <summary> 
        ///     Declaration of the routed event reporting the mouse  was pressed
        /// </summary> 
        public static readonly RoutedEvent ScrollControlPreviewMouseMoveEvent = EventManager.RegisterRoutedEvent("ScrollControlPreviewMouseMove",
            RoutingStrategy.Direct, typeof(ScrollControlMouseEventHandler), typeof(ScrollControl));

        /// <summary> 
        ///     Event reporting the mouse  was pressed
        /// </summary>
        public event ScrollControlMouseEventHandler ScrollControlPreviewMouseMove
        {
            add { AddHandler(ScrollControl.ScrollControlPreviewMouseMoveEvent, value, false); }
            remove { RemoveHandler(ScrollControl.ScrollControlPreviewMouseMoveEvent, value); }
        }

        /// <summary> 
        ///     Virtual method reporting the mouse  was pressed
        /// </summary>
        protected virtual void OnScrollControlPreviewMouseMove(ScrollControlMouseEventArgs e)
        {
        }
        #endregion
        #region ScrollControlPreviewMouseUp

        private static void OnScrollControlPreviewMouseUpThunk(object sender, ScrollControlMouseButtonEventArgs e)
        {
            ((ScrollControl)sender).OnScrollControlPreviewMouseUp(e);
        }

        /// <summary> 
        ///     Declaration of the routed event reporting the mouse button was pressed
        /// </summary> 
        public static readonly RoutedEvent ScrollControlPreviewMouseUpEvent = EventManager.RegisterRoutedEvent("ScrollControlPreviewMouseUp",
            RoutingStrategy.Direct, typeof(ScrollControlMouseButtonEventHandler), typeof(ScrollControl));

        /// <summary> 
        ///     Event reporting the mouse button was pressed
        /// </summary>
        public event ScrollControlMouseButtonEventHandler ScrollControlPreviewMouseUp
        {
            add { AddHandler(ScrollControl.ScrollControlPreviewMouseUpEvent, value, false); }
            remove { RemoveHandler(ScrollControl.ScrollControlPreviewMouseUpEvent, value); }
        }

        /// <summary> 
        ///     Virtual method reporting the mouse button was pressed
        /// </summary>
        protected virtual void OnScrollControlPreviewMouseUp(ScrollControlMouseButtonEventArgs e)
        {
        }
        #endregion
        #region ScrollControlPreviewMouseDown
        private static void OnScrollControlPreviewMouseDownThunk(object sender, ScrollControlMouseButtonEventArgs e)
        {
            ((ScrollControl)sender).OnScrollControlPreviewMouseDown(e);
        }

        /// <summary> 
        ///     Declaration of the routed event reporting the mouse button was pressed
        /// </summary> 
        public static readonly RoutedEvent ScrollControlPreviewMouseDownEvent = EventManager.RegisterRoutedEvent("ScrollControlPreviewMouseDown",
            RoutingStrategy.Direct, typeof(ScrollControlMouseButtonEventHandler), typeof(ScrollControl));

        /// <summary> 
        ///     Event reporting the mouse button was pressed
        /// </summary>
        public event ScrollControlMouseButtonEventHandler ScrollControlPreviewMouseDown
        {
            add { AddHandler(ScrollControl.ScrollControlPreviewMouseDownEvent, value, false); }
            remove { RemoveHandler(ScrollControl.ScrollControlPreviewMouseDownEvent, value); }
        }

        /// <summary> 
        ///     Virtual method reporting the mouse button was pressed
        /// </summary>
        protected virtual void OnScrollControlPreviewMouseDown(ScrollControlMouseButtonEventArgs e)
        {
        }
        #endregion
        #endregion
        #endregion

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.backgroundFrame != null)
                {
                    this.backgroundFrame.Children.Clear();
                    this.backgroundFrame = null;
                }
                if (this.foregroundFrame != null)
                {
                    this.foregroundFrame.Children.Clear();
                    this.foregroundFrame = null;
                }
                if (this.innerFrame != null)
                {
                    this.innerFrame.Children.Clear();
                    this.innerFrame = null;
                }
                if (this.elementsFrame != null)
                {
                    this.elementsFrame.Children.Clear();
                    this.elementsFrame = null;
                }
                if (this.graphicFrame != null)
                {
                    this.graphicFrame.Children.Clear();
                    this.graphicFrame = null;
                }
            }
            this.UnwireHScrollBar();
            this.UnwireVScrollBar();
            this.Children.Clear();
            if (this._children != null)
            {
                this._children.Dispose();             
            }
            if (this.VisualChildren != null)
            {
                this.VisualChildren.Clear();             
            }
            _owner = null;
            //_hScrollBar = null;
            //_vScrollBar = null;
            _hScrollBarCopy = null;
            _vScrollBarCopy = null;
            autoScroller = null;
            this.ScrollControlPreviewMouseDown -= new ScrollControlMouseButtonEventHandler(OnScrollControlPreviewMouseDownThunk);
            this.ScrollControlPreviewMouseUp -= new ScrollControlMouseButtonEventHandler(OnScrollControlPreviewMouseUpThunk);
            this.ScrollControlPreviewMouseMove -= new ScrollControlMouseEventHandler(OnScrollControlPreviewMouseMoveThunk);
            this.ScrollControlMouseDown -= new ScrollControlMouseButtonEventHandler(OnScrollControlMouseDownThunk);
            this.ScrollControlMouseUp -= new ScrollControlMouseButtonEventHandler(OnScrollControlMouseUpThunk);
            this.ScrollControlMouseMove -= new ScrollControlMouseEventHandler(OnScrollControlMouseMoveThunk);
            GC.SuppressFinalize(this);
        }
    }


    /// <summary>
    /// Represents the method that will handle mouse button related routed events,
    /// <see cref="ScrollControl.ScrollControlPreviewMouseDown"/>.
    /// </summary>
    public delegate void ScrollControlMouseButtonEventHandler(object sender, ScrollControlMouseButtonEventArgs e);

    /// <summary>
    /// Provides data for mouse button related events in the ScrollControl and
    /// allows you to specify whether the ScrollControls mouse event listeners
    /// should be prevented from handling the event with the <see cref="SkipListeners"/>
    /// property.
    /// </summary>
    public class ScrollControlMouseButtonEventArgs : MouseButtonEventArgs
    {
        bool skipListeners = false;

        /// <summary>
        /// Initializes a new instance of the System.Windows.Input.MouseButtonEventArgs
        /// class by using the specified System.Windows.Input.MouseDevice, timestamp,
        /// and System.Windows.Input.MouseButton.
        /// </summary>
        /// <param name="mouse">The logical mouse device associated with this event.</param>
        /// <param name="timestamp">The time the event occurred.</param>
        /// <param name="button">The button associated with this event.</param>
        /// <param name="stylusDevice">The stylus device associated with this event.</param>
        public ScrollControlMouseButtonEventArgs(MouseDevice mouse, int timestamp, MouseButton button, StylusDevice stylusDevice)
            : base(mouse, timestamp, button, stylusDevice)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ScrollControl"/> should
        /// forward this event to registered <see cref="ScrollControl.MouseEventListeners"/>.
        /// Set this property true if you do want to prevent the MouseControllerDispatcher and
        /// other listeners to handle this event. The property does not affect the <see cref="RoutedEventArgs.Handled"/>
        /// state of the underlying mouse event. You can however optionally set the <see cref="RoutedEventArgs.Handled"/>
        /// property to mark the underlying mouse event as handled.
        /// </summary>
        /// <value><c>true</c> if ScrollControl shoull prevent listeners from handling the event; otherwise, <c>false</c>.</value>
        public bool SkipListeners
        {
            get { return skipListeners; }
            set { skipListeners = value; }
        }

        /// <summary>
        /// Invokes event handlers in a type-specific way, which can increase event system efficiency.
        /// </summary>
        /// <param name="genericHandler">The generic handler to call in a type-specific way.</param>
        /// <param name="genericTarget">The target to call the handler on.</param>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            ScrollControlMouseButtonEventHandler handler = (ScrollControlMouseButtonEventHandler)genericHandler;
            handler(genericTarget, this);
        }
    }


    /// <summary>
    /// Represents the method that will handle mouse related routed events,
    /// for example <see cref="ScrollControl.ScrollControlPreviewMouseMove"/>.
    /// </summary>
    public delegate void ScrollControlMouseEventHandler(object sender, ScrollControlMouseEventArgs e);

    /// <summary>
    /// Provides data for mouse  related events in the ScrollControl and
    /// allows you to specify whether the ScrollControls mouse event listeners
    /// should be prevented from handling the event with the <see cref="SkipListeners"/>
    /// property.
    /// </summary>
    public class ScrollControlMouseEventArgs : MouseEventArgs
    {
        bool skipListeners = false;

        /// <summary>
        /// Initializes a new instance of the System.Windows.Input.MouseEventArgs
        /// class by using the specified System.Windows.Input.MouseDevice, timestamp,
        /// and System.Windows.Input.Mouse.
        /// </summary>
        /// <param name="mouse">The logical mouse device associated with this event.</param>
        /// <param name="timestamp">The time the event occurred.</param>
        /// <param name="stylusDevice">The stylus device associated with this event.</param>
        public ScrollControlMouseEventArgs(MouseDevice mouse, int timestamp, StylusDevice stylusDevice)
            : base(mouse, timestamp, stylusDevice)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ScrollControl"/> should
        /// forward this event to registered <see cref="ScrollControl.MouseEventListeners"/>.
        /// Set this property true if you do want to prevent the MouseControllerDispatcher and
        /// other listeners to handle this event. The property does not affect the <see cref="RoutedEventArgs.Handled"/>
        /// state of the underlying mouse event. You can however optionally set the <see cref="RoutedEventArgs.Handled"/>
        /// property to mark the underlying mouse event as handled.
        /// </summary>
        /// <value><c>true</c> if ScrollControl shoull prevent listeners from handling the event; otherwise, <c>false</c>.</value>
        public bool SkipListeners
        {
            get { return skipListeners; }
            set { skipListeners = value; }
        }

        /// <summary>
        /// Invokes event handlers in a type-specific way, which can increase event system efficiency.
        /// </summary>
        /// <param name="genericHandler">The generic handler to call in a type-specific way.</param>
        /// <param name="genericTarget">The target to call the handler on.</param>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            ScrollControlMouseEventHandler handler = (ScrollControlMouseEventHandler)genericHandler;
            handler(genericTarget, this);
        }
    }

}