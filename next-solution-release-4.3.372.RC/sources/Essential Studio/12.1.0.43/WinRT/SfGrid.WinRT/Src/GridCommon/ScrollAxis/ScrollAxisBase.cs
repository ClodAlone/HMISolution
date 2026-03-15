#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using System;
using System.ComponentModel;
#if !WinRT
using System.Windows.Controls;
#endif

namespace Syncfusion.UI.Xaml.ScrollAxis
{
    /// <summary>
    /// ScrollAxisBase is an abstract base class and implements scrolling
    /// logic for both horizontal and vertical scrolling in a <see cref="ScrollAxisControl"/>.
    /// Logical units in the ScrollAxisBase are called "Lines". With the 
    /// <see cref="ScrollAxisControl.ScrollRows"/> a line representes rows in a grid 
    /// and with <see cref="ScrollAxisControl.ScrollRows"/> a line represents columns in a grid.
    /// <para/>
    /// ScrollAxisBase has support for frozen header and footer lines, maintaining a
    /// scroll position and updating and listening to scrollbars. It also maintains
    /// a collection of <see cref="VisibleLineInfo"/> items for all the lines that are
    /// visible in the viewing area. ScrollAxisBase wires itself with a 
    /// <see cref="ScrollLinesHost"/> and reacts to changes in line count,
    /// line sizes, hidden state and default line size.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public abstract class ScrollAxisBase : IDisposable
    {
        #region Fields
        double renderSize;
        IScrollBar scrollBar;
        ILineSizeHost scrollLinesHost;
        bool layoutDirty = false;
        int lineResizeIndex = -1;
        double lineResizeSize = 0;
        VisibleLinesCollection visibleLines = new VisibleLinesCollection();
        double lastScrollValue = -1;
        DoubleSpan clip = DoubleSpan.Empty;
        bool ignoreScrollBarPropertyChange = false;
        bool inGetVisibleLines = false;
        bool allBodyLinesShown = false;
        int lastBodyLineIndex = -1;
        internal string Name { get; set; }

        #endregion

        #region Events
        /// <summary>
        /// Occurs when a property was changed.
        /// </summary>
        public event ScrollChangedEventHandler Changed;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollAxisBase"/> class.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="scrollLinesHost">The scroll lines host.</param>
        public ScrollAxisBase(IScrollBar sb, ILineSizeHost scrollLinesHost)
        {
            if (sb == null)
                throw new ArgumentNullException();
            this.scrollBar = sb;
            this.scrollBar.PropertyChanged += new PropertyChangedEventHandler(scrollBar_PropertyChanged);
            this.scrollLinesHost = scrollLinesHost;
            this.WireScrollLinesHost();
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.scrollBar.PropertyChanged -= new PropertyChangedEventHandler(scrollBar_PropertyChanged);
            UnwireScrollLinesHost();
        }

        #endregion

        #region IsPixelScroll, StartLineIndex

        /// <summary>
        /// Gets a value indicating whether this axis supports pixel scrolling.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance supports pixel scrolling; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsPixelScroll
        {
            get;
        }

        // Support for sharing axis with a parent axis

        /// <summary>
        /// Gets or sets the index of the first line in a parent axis. This is used for shared 
        /// or nested scroll axis (e.g. a nested grid with shared axis in a covered cell).
        /// </summary>
        /// <value>The index of the first line..</value>
        public virtual int StartLineIndex { get; set; }

        #endregion

        #region ScrollBar
        /// <summary>
        /// Gets the scroll bar state.
        /// </summary>
        /// <value>The scroll bar state.</value>
        public IScrollBar ScrollBar
        {
            get
            {
                return this.scrollBar;
            }
        }

        void scrollBar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ignoreScrollBarPropertyChange) return;
            this.visibleLines.Clear();
            RaiseChanged(ScrollChangedAction.ScrollBarValueChanged);
        }

        public void UpdateScrollBar(bool ignorePropertyChange)
        {
            bool b = ignoreScrollBarPropertyChange;
            ignoreScrollBarPropertyChange |= ignorePropertyChange;
            try
            {
                UpdateScrollBar();
            }
            finally
            {
                ignoreScrollBarPropertyChange = b;
            }
        }

        /// <summary>
        /// Updates the scroll bar.
        /// </summary>
        public abstract void UpdateScrollBar();

        #endregion

        #region LineCount, DefaultLineSize, HiddenState and LineSizes
        /// <summary>
        /// Gets or sets the line count.
        /// </summary>
        /// <value>The line count.</value>
        public abstract int LineCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default size of lines.
        /// </summary>
        /// <value>The default size of lines.</value>
        public abstract double DefaultLineSize
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the hidden state of the lines.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="hide">if set to <c>true</c> [hide].</param>
        public abstract void SetLineHiddenState(int from, int to, bool hide);

        /// <summary>
        /// Sets the size of the lines.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="size">The size.</param>
        public abstract void SetLineSize(int from, int to, double size);

        /// <summary>
        /// Gets size from ScrollLinesHost or if the line is being resized then get temporary value
        /// previously set with <see cref="SetLineResize"/>
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="repeatSizeCount">The number of subsequent values with same size.</param>
        /// <returns></returns>
        public virtual double GetLineSize(int index, out int repeatSizeCount)
        {
            repeatSizeCount = 1;
            if (index == lineResizeIndex)
                return lineResizeSize;

            if (ScrollLinesHost == null)
            {
                repeatSizeCount = int.MaxValue;
                return DefaultLineSize;
            }

            return GetScrollLinesHostSize(index, out repeatSizeCount);
        }

        /// <summary>
        /// Gets the size of the line.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public double GetLineSize(int index)
        {
            int repeatSizeCount;
            return GetLineSize(index, out repeatSizeCount);
        }

        internal int GetRangeToHelper(int n, int to, int repeatSizeCount)
        {
            if (repeatSizeCount == int.MaxValue)
                return to;
            return Math.Min(to, n + repeatSizeCount - 1);
        }

        #endregion

        #region LineResize
        /// <summary>
        /// Set temporary value for a line size during a resize operation without commiting
        /// value to SrollLinesHost.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="size">The size.</param>
        public virtual void SetLineResize(int index, double size)
        {
            this.lineResizeIndex = index;
            this.lineResizeSize = size;
            SetLineSize(index, index, size);
            MarkDirty();
            RaiseChanged(ScrollChangedAction.LineResized);
        }

        /// <summary>
        /// Resets temporary value for line size after a resize operation 
        /// </summary>
        public virtual void ResetLineResize()
        {
            int repeatSizeCount;
            if (lineResizeIndex >= 0 && ScrollLinesHost != null)
                SetLineSize(lineResizeIndex, lineResizeIndex, GetScrollLinesHostSize(lineResizeIndex, out repeatSizeCount));
            this.lineResizeIndex = -1;
            this.lineResizeSize = 0;
            MarkDirty();
            RaiseChanged(ScrollChangedAction.LineResized);
        }

        /// <summary>
        /// Gets size from ScrollLinesHost or if the line is being resized then get temporary value
        /// previously set with <see cref="SetLineResize"/>. If size is negative then <see cref="DefaultLineSize"/> is returned.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="repeatSizeCount">The number of subsequent values with same size.</param>
        /// <returns></returns>
        public double GetScrollLinesHostSize(int index, out int repeatSizeCount)
        {
            double size = ScrollLinesHost.GetSize(index, out repeatSizeCount);
            if (size < 0)
                size = DefaultLineSize;
            return size;
        }


        #endregion

        #region ScrollLinesHost

        /// <summary>
        /// Gets the scroll lines host.
        /// </summary>
        /// <value>The scroll lines host.</value>
        public ILineSizeHost ScrollLinesHost
        {
            get
            {
                return this.scrollLinesHost;
            }
        }

        #region ScrollLinesHost Event Handlers
        private void WireScrollLinesHost()
        {
            if (ScrollLinesHost == null)
                return;

            ScrollLinesHost.DefaultLineSizeChanged +=new DefaultLineSizeChangedEventHandler(ScrollLinesHost_DefaultLineSizeChanged);
            ScrollLinesHost.LineCountChanged += new EventHandler(ScrollLinesHost_LineCountChanged);
            ScrollLinesHost.LineHiddenChanged += new HiddenRangeChangedEventHandler(ScrollLinesHost_LineHiddenChanged);
            ScrollLinesHost.LineSizeChanged += new RangeChangedEventHandler(ScrollLinesHost_LineSizeChanged);
            ScrollLinesHost.HeaderLineCountChanged += new EventHandler(ScrollLinesHost_HeaderLineCountChanged);
            ScrollLinesHost.FooterLineCountChanged += new EventHandler(ScrollLinesHost_FooterLineCountChanged);
            ScrollLinesHost.LinesInserted += new LinesInsertedEventHandler(ScrollLinesHost_LinesInserted);
            ScrollLinesHost.LinesRemoved += new LinesRemovedEventHandler(ScrollLinesHost_LinesRemoved);
        }

        private void UnwireScrollLinesHost()
        {
            if (ScrollLinesHost == null)
                return;

            ScrollLinesHost.DefaultLineSizeChanged -= new DefaultLineSizeChangedEventHandler(ScrollLinesHost_DefaultLineSizeChanged);
            ScrollLinesHost.LineCountChanged -= new EventHandler(ScrollLinesHost_LineCountChanged);
            ScrollLinesHost.LineHiddenChanged -= new HiddenRangeChangedEventHandler(ScrollLinesHost_LineHiddenChanged);
            ScrollLinesHost.LineSizeChanged -= new RangeChangedEventHandler(ScrollLinesHost_LineSizeChanged);
            ScrollLinesHost.HeaderLineCountChanged -= new EventHandler(ScrollLinesHost_HeaderLineCountChanged);
            ScrollLinesHost.FooterLineCountChanged -= new EventHandler(ScrollLinesHost_FooterLineCountChanged);
            ScrollLinesHost.LinesInserted -= new LinesInsertedEventHandler(ScrollLinesHost_LinesInserted);
            ScrollLinesHost.LinesRemoved -= new LinesRemovedEventHandler(ScrollLinesHost_LinesRemoved);
        }

        void ScrollLinesHost_LineSizeChanged(object sender, RangeChangedEventArgs e)
        {
            // todo: not needed when distances == null in PixelScrollAxis.
            for (int n = e.From; n <= e.To; n++)
            {
                int repeatSizeCount;
                double size = GetScrollLinesHostSize(n, out repeatSizeCount);
                int rangeTo = GetRangeToHelper(n, e.To, repeatSizeCount);
                this.SetLineSize(n, rangeTo, size);
                n = rangeTo;
            }

            // Also check whether I need to re-hide any of the rows.
            ScrollLinesHost_LineHiddenChanged(sender, new HiddenRangeChangedEventArgs(e.From, e.To, false));
            MarkDirty();
            RaiseChanged(ScrollChangedAction.LineResized);
        }

        void ScrollLinesHost_LineHiddenChanged(object sender, HiddenRangeChangedEventArgs e)
        {
            for (int n = e.From; n <= e.To; n++)
            {
                int repeatSizeCount;
                bool hide = ScrollLinesHost.GetHidden(n, out repeatSizeCount);
                int rangeTo = GetRangeToHelper(n, e.To, repeatSizeCount);
                this.SetLineHiddenState(n, rangeTo, hide);
                n = rangeTo;
            }
            MarkDirty();
            this.UpdateScrollBar(true);
            RaiseChanged(ScrollChangedAction.HiddenLineChanged);
        }

        void ScrollLinesHost_LineCountChanged(object sender, EventArgs e)
        {
            LineCount = ScrollLinesHost.GetLineCount();
            MarkDirty();
            UpdateScrollBar(true);
            RaiseChanged(ScrollChangedAction.LineCountChanged);
        }

        void ScrollLinesHost_DefaultLineSizeChanged(object sender, DefaultLineSizeChangedEventArgs e)
        {
            DefaultLineSize = ScrollLinesHost.GetDefaultLineSize();
            MarkDirty();
            UpdateScrollBar(true);
            RaiseChanged(ScrollChangedAction.DefaultLineSizeChanged);
        }

        void ScrollLinesHost_FooterLineCountChanged(object sender, EventArgs e)
        {
            SetFooterLineCount(ScrollLinesHost.GetFooterLineCount());
            MarkDirty();
            RaiseChanged(ScrollChangedAction.FooterLineCountChanged);
        }

        void ScrollLinesHost_HeaderLineCountChanged(object sender, EventArgs e)
        {
            SetHeaderLineCount(ScrollLinesHost.GetHeaderLineCount());
            MarkDirty();
            RaiseChanged(ScrollChangedAction.HeaderLineCountChanged);
        }

        void ScrollLinesHost_LinesRemoved(object sender, LinesRemovedEventArgs e)
        {
            OnLinesRemoved(e.RemoveAt, e.Count);
            RaiseChanged(ScrollChangedAction.LinesRemoved);
        }

        void ScrollLinesHost_LinesInserted(object sender, LinesInsertedEventArgs e)
        {
            OnLinesInserted(e.InsertAt, e.Count);
            RaiseChanged(ScrollChangedAction.LinesInserted);
        }

        /// <summary>
        /// Called when lines were removed in ScrollLinesHost.
        /// </summary>
        /// <param name="removeAt">Index of the first removed line.</param>
        /// <param name="count">The count.</param>
        protected virtual void OnLinesRemoved(int removeAt, int count)
        {
        }

        /// <summary>
        /// Called when lines were inserted in ScrollLinesHost.
        /// </summary>
        /// <param name="insertAt">Index of the first inserted line.</param>
        /// <param name="count">The count.</param>
        protected virtual void OnLinesInserted(int insertAt, int count)
        {
        }

        #endregion

        #endregion

        #region RenderSize, ViewSize and Clip

        /// <summary>
        /// Gets or sets the size (either height or width) of the parent control.
        /// </summary>
        /// <value>The size of the the parent control.</value>
        public double RenderSize
        {
            get
            {
                return this.renderSize;
            }
            set
            {
                if (this.renderSize != value)
                {
                    this.renderSize = value;
                    MarkDirty();
                    UpdateScrollBar(true);
                }
            }
        }

        /// <summary>
        /// Gets the size (either height or width) of the parent control excluding the 
        /// area occupied by Header and Footer. This size is used for scrolling down
        /// or up one page.
        /// </summary>
        /// <value>The size of the the parent control.</value>
        public double ScrollPageSize
        {
            get
            {
                return RenderSize - HeaderExtent - FooterExtent;
            }
        }


        /// <summary>
        /// Gets or sets the clipping region. Depending on the orientation of
        /// the axis, this is either the left and right or top and bottom
        /// values of the clipping rectangle in the parent control.
        /// </summary>
        /// <value>The clip.</value>
        public DoubleSpan Clip
        {
            get
            {
                return clip;
            }
            set
            {
                clip = value;
            }
        }

        /// <summary>
        /// Gets the view size of the (either height or width) of the parent control. Normally
        /// the ViewSize is the same as <see cref="RenderSize"/>. Only if the parent control
        /// has more space then needed to display all lines, the ViewSize will be less. In 
        /// such case the ViewSize is the total height for all lines.
        /// </summary>
        /// <value>The size of the view.</value>
        public abstract double ViewSize
        {
            get;
        }

        #endregion

        #region Header and Footer

        /// <summary>
        /// Gets the header extent. This is total height (or width) of the header lines.
        /// </summary>
        /// <value>The header extent.</value>
        public abstract double HeaderExtent
        {
            get;
        }

        /// <summary>
        /// Gets the header line count.
        /// </summary>
        /// <value>The header line count.</value>
        public virtual int HeaderLineCount
        {
            get
            {
                if (ScrollLinesHost == null)
                    return 0;
                return ScrollLinesHost.GetHeaderLineCount();
            }
        }

        /// <summary>
        /// Sets the header line count.
        /// </summary>
        /// <param name="value">The value.</param>
        protected abstract void SetHeaderLineCount(int value);

        /// <summary>
        /// Gets the footer extent. This is total height (or width) of the footer lines.
        /// </summary>
        /// <value>The footer extent.</value>
        public abstract double FooterExtent
        {
            get;
        }

        /// <summary>
        /// Gets the footer line count.
        /// </summary>
        /// <value>The footer line count.</value>
        public virtual int FooterLineCount
        {
            get
            {
                if (ScrollLinesHost == null)
                    return 0;
                return ScrollLinesHost.GetFooterLineCount();
            }
        }

        /// <summary>
        /// Sets the footer line count.
        /// </summary>
        /// <param name="value">The value.</param>
        protected abstract void SetFooterLineCount(int value);

        #endregion

        #region Scrolling

        // Scroll = First Visible Body Line

        /// <summary>
        /// Gets or sets the index of the first visible Line in the Body region.
        /// </summary>
        /// <value>The index of the scroll line.</value>
        public abstract int ScrollLineIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the index of the scroll line.
        /// </summary>
        /// <param name="scrollLindeIndex">Index of the scroll linde.</param>
        /// <param name="scrollLineOffset">The scroll line offset.</param>
        public abstract void GetScrollLineIndex(out int scrollLindeIndex, out double scrollLineOffset);

        /// <summary>
        /// Sets the index of the scroll line.
        /// </summary>
        /// <param name="scrollLindeIndex">Index of the scroll linde.</param>
        /// <param name="scrollLineOffset">The scroll line offset.</param>
        public abstract void SetScrollLineIndex(int scrollLindeIndex, double scrollLineOffset);

        /// <summary>
        /// Gets the index of the previous scroll line.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <returns></returns>
        public abstract int GetPreviousScrollLineIndex(int lineIndex);

        /// <summary>
        /// Gets the index of the next scroll line.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <returns></returns>
        public abstract int GetNextScrollLineIndex(int lineIndex);

        /// <summary>
        /// Scrolls to next page.
        /// </summary>
        public abstract void ScrollToNextPage();

        /// <summary>
        /// Scrolls to previous page.
        /// </summary>
        public abstract void ScrollToPreviousPage();

        /// <summary>
        /// Scrolls to next line.
        /// </summary>
        public abstract void ScrollToNextLine();

        /// <summary>
        /// Scrolls to previous line.
        /// </summary>
        public abstract void ScrollToPreviousLine();

        /// <summary>
        /// Aligns the scroll line.
        /// </summary>
        public abstract void AlignScrollLine();

        /// <summary>
        /// Scrolls the line into viewable area.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <param name="lineSize">Size of the line.</param>
        public virtual void ScrollInView(int lineIndex, double lineSize)
        {
        }

        /// <summary>
        /// Scrolls the line into viewable area.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        public void ScrollInView(int lineIndex)
        {
            ScrollInView(lineIndex, GetLineSize(lineIndex));
        }



        /// <summary>
        /// This method is called in response to a MouseWheel event.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public abstract void MouseWheel(int delta);

        #endregion

        #region Visible Lines

        /// <summary>
        /// Force recalculation of visible lines and scrollbar properties
        /// next time GetVisibleLines is called.
        /// </summary>
        public void MarkDirty()
        {
            layoutDirty = true;
        }

        //code to handle issue in DT 77714
        private const double EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+EPSILON != 1.0 */
        private static bool StrictlyLessThan(double d1, double d2)
        {
            return !(d1 > d2 || AreClose(d1, d2));
        }
        private static bool AreClose(double d1, double d2)
        {
            if (d1 == d2)
            {
                return true;
            }
            double eps = (Math.Abs(d1) + Math.Abs(d2) + 10.0) * EPSILON;
            return Math.Abs(d1 - d2) < eps;
        }

        public void FreezeVisibleLines()
        {
            inGetVisibleLines = true;
        }

        public void UnfreezeVisibleLines()
        {
            inGetVisibleLines = false;
        }

        /// <summary>
        /// Gets the visible lines collection
        /// </summary>
        /// <returns></returns>
        public VisibleLinesCollection GetVisibleLines()
        {
            if (inGetVisibleLines)
                return visibleLines;
            inGetVisibleLines = true;
            try
            {
                if (layoutDirty)
                {
                    SetHeaderLineCount(HeaderLineCount);
                    SetFooterLineCount(FooterLineCount);

                    lastScrollValue = -1;
                    layoutDirty = false;
                    UpdateScrollBar(true);
                }
                if (visibleLines.Count == 0 || lastScrollValue != scrollBar.Value)
                {
                    visibleLines.Clear();

                    int visibleIndex = 0;
                    int scrollLineIndex;
                    double scrollOffset;
                    int headerLineCount = HeaderLineCount;
                    GetScrollLineIndex(out scrollLineIndex, out scrollOffset);
                    int firstFooterLine = LineCount - FooterLineCount;
                    double footerStartPoint = RenderSize - FooterExtent;
                    int index;

                    // Header
                    double point = 0;
                    int lastHeaderLineIndex = -1;
                    for (index = 0;
                        //  index != -1 && point < HeaderExtent && index < firstFooterLine;
                        index != -1 && StrictlyLessThan(point, HeaderExtent) && index < firstFooterLine && index < headerLineCount;
                        index = GetNextScrollLineIndex(index))
                    {
                        double size = this.GetLineSize(index);
                        visibleLines.Add(new VisibleLineInfo(visibleIndex++, index, size, point, 0, true, false));
                        point += size;
                        lastHeaderLineIndex = index;
                    }
                    visibleLines.firstBodyVisibleIndex = visibleLines.Count;

                    VisibleLineInfo lastScrollableLine = null;

                    // Body
                    point = HeaderExtent;
                    int firstBodyLineIndex = Math.Max(scrollLineIndex, lastHeaderLineIndex + 1);

                    for (index = firstBodyLineIndex;
                        // index != -1 && point < footerStartPoint && index < firstFooterLine;
                        index != -1 && StrictlyLessThan(point, footerStartPoint) && index < firstFooterLine;
                        index = GetNextScrollLineIndex(index))
                    {
                        double size = this.GetLineSize(index);
                        visibleLines.Add(lastScrollableLine = new VisibleLineInfo(visibleIndex++, index, size, point, scrollOffset, false, false));
                        point += size - scrollOffset;
                        scrollOffset = 0;  // reset scrollOffset after first line. Subsequent lines will start at given point.
                    }

                    if (lastScrollableLine == null)
                    {
                        allBodyLinesShown = true;
                        lastBodyLineIndex = -1;
                    }
                    else
                    {
                        allBodyLinesShown = index >= firstFooterLine;
                        lastBodyLineIndex = lastScrollableLine.LineIndex;
                    }

                    visibleLines.firstFooterVisibleIndex = visibleLines.Count;

                    // Footer
                    point = Math.Max(HeaderExtent, ViewSize - FooterExtent);
                    for (index = firstFooterLine;
                        // index != -1 && point < RenderSize && index < LineCount;
                        index != -1 && StrictlyLessThan(point, RenderSize) && index < LineCount;
                        index = GetNextScrollLineIndex(index))
                    {
                        if (lastScrollableLine != null)
                        {
                            lastScrollableLine.clippedCornerExtent = lastScrollableLine.Corner - point;
                            lastScrollableLine = null;
                        }
                        double size = this.GetLineSize(index);
                        visibleLines.Add(new VisibleLineInfo(visibleIndex++, index, size, point, 0, false, true));
                        point += size;
                    }

                    if (lastScrollableLine != null)
                    {
                        lastScrollableLine.clippedCornerExtent = lastScrollableLine.Corner - point;
                        lastScrollableLine = null;
                    }

                    lastScrollValue = scrollBar.Value;

                    if (visibleLines.Count > 0)
                        visibleLines[visibleLines.Count - 1].isLastLine = true;

#if DEBUG
                    try
                    {
                        // throws exception when a line is duplicate
                        visibleLines.GetVisibleLineAtLineIndex(0);
                    }
                    catch (Exception)
                    {
                        if (!indebug)
                        {
                            indebug = true;
                            visibleLines.Clear();
                            try
                            {
                                GetVisibleLines();
                            }
                            finally
                            {
                                indebug = false;
                            }
                        }
                    }
#endif
                }

            }
            finally
            {
                inGetVisibleLines = false;
            }
            return visibleLines;
        }
  
#if DEBUG
        bool indebug = false;
#endif

        /// <summary>
        /// Resets the visible lines collection.
        /// </summary>
        public void ResetVisibleLines()
        {
            visibleLines.Clear();
        }

        /// <summary>
        /// Gets the visible line index for a point in the display.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="allowOutsideLines">Set this true if point can be below corner of last line.</param>
        /// <returns></returns>
        public int VisiblePointToLineIndex(double point, bool allowOutsideLines)
        {
            if (allowOutsideLines)
                point = Math.Max(point, 0);
            VisibleLineInfo line = GetVisibleLines().GetVisibleLineAtPoint(point);
            if (line != null && (allowOutsideLines || point <= line.Corner))
                return line.LineIndex;
            return -1;
        }

        /// <summary>
        /// Gets the visible line index for a point in the display.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public int VisiblePointToLineIndex(double point)
        {
            return VisiblePointToLineIndex(point, true);
        }

        /// <summary>
        /// Gets the visibles line for a point in the display.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public VisibleLineInfo GetVisibleLineAtPoint(double point)
        {
            return GetVisibleLines().GetVisibleLineAtPoint(point);
        }

        /// <summary>
        /// Gets the visibles line that displays the line with the given absolut line index.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <returns></returns>
        public VisibleLineInfo GetVisibleLineAtLineIndex(int lineIndex)
        {
            return GetVisibleLines().GetVisibleLineAtLineIndex(lineIndex);
        }

        /// <summary>
        /// Gets the visibles line that displays the line with the given absolut line index. If the
        /// line is outside the view and you specify allowCreateEmptyLineIfNotVisible then
        /// the method will create an empty line and initializes its LineIndex and LineSize.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <param name="allowCreateEmptyLineIfNotVisible">if set to <c>true</c> and if the
        /// line is outside the view then
        /// the method will create an empty line and initializes its LineIndex and LineSize.</param>
        /// <returns></returns>
        public VisibleLineInfo GetVisibleLineAtLineIndex(int lineIndex, bool allowCreateEmptyLineIfNotVisible)
        {
            VisibleLineInfo line = GetVisibleLineAtLineIndex(lineIndex);
            if (line == null && allowCreateEmptyLineIfNotVisible)
            {
                double size = GetLineSize(lineIndex);
                line = new VisibleLineInfo(int.MaxValue, lineIndex, size, RenderSize + 1, size, false, false);
            }

            return line;
        }


        /// <summary>
        /// Determines if the line with the given absolut line index is visible.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <returns></returns>
        public bool IsLineVisible(int lineIndex)
        {
            return GetVisibleLines().GetVisibleLineAtLineIndex(lineIndex) != null || lineIndex > lastBodyLineIndex && allBodyLinesShown;
        }

        /// <summary>
        /// Determines if any of the lines with the given absolut line index are visible.
        /// </summary>
        public bool AnyVisibleLines(int lineIndex1, int lineIndex2)
        {
            return visibleLines.AnyVisibleLines(lineIndex1, lineIndex2);
        }

        #endregion

        #region Layout, HitTest and Clip Calculations

        /// <summary>
        /// Gets the origin and corner points of body region.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="corner">The corner.</param>
        public void GetOriginAndCornerOfBodyRegion(out double origin, out double corner)
        {
            int scrollLineIndex;
            double scrollOffset;
            GetScrollLineIndex(out scrollLineIndex, out scrollOffset);

            double arrangeSize = RenderSize;
            double adjustedFooterExtent = AdjustFooterExtentToAvoidGap(FooterExtent, arrangeSize);

            origin = HeaderExtent - scrollOffset;
            corner = arrangeSize - adjustedFooterExtent;
        }

        /// <summary>
        /// Adjusts the footer extent to avoid gap between last visible line of body region
        /// and first line of footer in case the view is larger than the height/width of all
        /// lines.
        /// </summary>
        /// <param name="footerSize">Size of the footer.</param>
        /// <param name="arrangeSize">Size of the arrange.</param>
        /// <returns></returns>
        private double AdjustFooterExtentToAvoidGap(double footerSize, double arrangeSize)
        {
            // Adjust start of footer to avoid gap after last row.
            if (ViewSize < arrangeSize)
                footerSize += arrangeSize - ViewSize;

            if (footerSize + HeaderExtent > arrangeSize)
                footerSize = Math.Max(0, arrangeSize - HeaderExtent);

            return footerSize;
        }

        /// <summary>
        /// Gets the view corner which is the point after the last visible line
        /// of the body region.
        /// </summary>
        /// <value>The view corner.</value>
        public double ViewCorner
        {
            get
            {
                int scrollLineIndex;
                double scrollOffset;
                GetScrollLineIndex(out scrollLineIndex, out scrollOffset);

                double arrangeSize = RenderSize;
                double adjustedFooterExtent = AdjustFooterExtentToAvoidGap(FooterExtent, arrangeSize);

                return arrangeSize - adjustedFooterExtent;
            }
        }

        /// <summary>
        /// Return indexes for VisibleLinesCollection for area identified by section.
        /// </summary>
        /// <param name="section">0 - Header, 1 - Body, 2 - Footer</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public void GetVisibleSection(int section, out int start, out int end)
        {
            VisibleLinesCollection visibleLines = this.GetVisibleLines();
            switch (section)
            {
                case 0:
                    start = 0;
                    end = visibleLines.FirstBodyVisibleIndex - 1;
                    break;
                case 1:
                    start = visibleLines.FirstBodyVisibleIndex;
                    end = visibleLines.FirstFooterVisibleIndex - 1;
                    break;
                case 2:
                    start = visibleLines.FirstFooterVisibleIndex;
                    end = visibleLines.Count - 1;
                    break;
                default:
                    start = end = -1;
                    break;
            }
        }

        /// <summary>
        /// Returns the first and last VisibleLine.LineIndex for area identified by section.
        /// </summary>
        /// <param name="section">0 - Header, 1 - Body, 2 - Footer</param>
        public Int32Span GetVisibleLinesRange(int section)
        {
            VisibleLinesCollection visibleLines = this.GetVisibleLines();
            int start, end;
            GetVisibleSection(section, out start, out end);
            return new Int32Span(visibleLines[start].LineIndex, visibleLines[end].LineIndex);
        }

        /// <summary>
        /// Return indexes for VisibleLinesCollection for area identified by section.
        /// </summary>
        /// <param name="section">0 - Header, 1 - Body, 2 - Footer</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public void GetVisibleSection(ScrollAxisRegion section, out int start, out int end)
        {
            GetVisibleSection((int)section, out start, out end);
        }

        /// <summary>
        /// Returns the clipping area for the specified visible lines. Only if <see cref="VisibleLineInfo.IsClippedOrigin"/> is true for
        /// first line or if <see cref="VisibleLineInfo.IsClippedCorner"/> is true for last line then the area will be clipped. Otherwise
        /// the whole area from 0 to <see cref="RenderSize"/> is returned.
        /// </summary>
        /// <param name="firstLine">The first line.</param>
        /// <param name="lastLine">The last line.</param>
        /// <returns></returns>
        public DoubleSpan GetBorderRangeClipPoints(VisibleLineInfo firstLine, VisibleLineInfo lastLine)
        {
            if (!firstLine.IsClippedOrigin && !lastLine.IsClippedCorner)
                return new DoubleSpan(0, RenderSize);

            if (firstLine.IsClippedOrigin && !lastLine.IsClippedCorner)
                return firstLine.ClippedOrigin < RenderSize ? new DoubleSpan(firstLine.ClippedOrigin, RenderSize) : new DoubleSpan(RenderSize, firstLine.ClippedOrigin);

            if (!firstLine.IsClippedOrigin && lastLine.IsClippedCorner)
                return new DoubleSpan(0, lastLine.ClippedCorner);

            return new DoubleSpan(firstLine.ClippedOrigin, lastLine.ClippedCorner);
        }


        /// <summary>
        /// Gets the line near the given corner point. Use this method for hit-testing row or 
        /// column lines for resizing cells.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="hitTestPrecision">The hit test precision in points.</param>
        /// <returns></returns>
        public VisibleLineInfo GetLineNearCorner(double point, double hitTestPrecision)
        {
            return GetLineNearCorner(point, hitTestPrecision, CornerSide.Both);
        }

        /// <summary>
        /// Gets the line near the given corner point. Use this method for hit-testing row or 
        /// column lines for resizing cells.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="hitTestPrecision">The hit test precision in points.</param>
        /// <param name="side">The hit test corner.</param>
        /// <returns></returns>
        public VisibleLineInfo GetLineNearCorner(double point, double hitTestPrecision, CornerSide side)
        {
            VisibleLinesCollection lines = GetVisibleLines();
            VisibleLineInfo visibleLine = lines.GetVisibleLineAtPoint(point);

            if (visibleLine != null)
            {
                double d;
                // Close to 
                for (int n = Math.Max(1, visibleLine.VisibleIndex); n < lines.Count; n++)
                {
                    visibleLine = lines[n];

                    d = visibleLine.ClippedOrigin - point;

                    if ((d > hitTestPrecision) || (d > 0 && (side == CornerSide.Right || side == CornerSide.Bottom))
                        || (d < 0 && (side == CornerSide.Left)))
                        return null;

                    if (Math.Abs(d) <= hitTestPrecision)
                        return lines[visibleLine.VisibleIndex - 1];
                }

                // last line - check corner instead of origin.
                d = visibleLine.ClippedCorner - point;
                if (Math.Abs(d) <= hitTestPrecision)
                    return lines[visibleLine.VisibleIndex];
            }

            return null;
        }

        /// <summary>
        /// Returns points for given absolut line indexes
        /// </summary>
        /// <param name="firstIndex">The first index.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <param name="allowAdjust">if set to <c>true</c> return the first visible line if firstIndex
        /// is above viewable area or return last visible line if lastIndex is after viewable area
        /// (works also for header and footer).
        /// </param>
        /// <param name="firstVisible">if set to <c>true</c> indicates the line with index 
        /// firstIndex is visible in viewable area.</param>
        /// <param name="lastVisible">if set to <c>true</c> indicates the line with index 
        /// lastIndex is visible in viewable area..</param>
        /// <param name="firstLine">The first line or null if allowAdjust is false and line 
        /// is not in viewable area.</param>
        /// <param name="lastLine">The last line or null if allowAdjust is false and line 
        /// is not in viewable area.</param>
        public void GetLinesAndVisibility(int firstIndex, int lastIndex, bool allowAdjust, out bool firstVisible, out bool lastVisible, out VisibleLineInfo firstLine, out VisibleLineInfo lastLine)
        {
            VisibleLinesCollection visibleLines = this.GetVisibleLines();

            //if (GetLineSize(firstIndex == 0))
            //    firstIndex = GetPreviousScrollLineIndex(firstIndex);
            if (firstIndex < 0)
                firstIndex = 0;

            // Invalid Line
            if (firstIndex < 0 || firstIndex >= LineCount)
            {
                firstVisible = false;
                firstLine = null;
            }
            // Header
            else if (firstIndex < HeaderLineCount)
            {
                firstVisible = true;
                firstLine = visibleLines.GetVisibleLineNearLineIndex(firstIndex);
            }
            // Footer
            else if (firstIndex >= FirstFooterLineIndex)
            {
                firstVisible = true;
                firstLine = visibleLines.GetVisibleLineNearLineIndex(firstIndex);
            }
            // After Header and Before Scroll Position
            else if (firstIndex < ScrollLineIndex)
            {
                firstVisible = false;
                firstLine = allowAdjust ? GetVisibleLineAtLineIndex(ScrollLineIndex) : null;
            }
                // After Scroll Position and Before Footer
            else if (firstIndex > LastBodyVisibleLineIndex)
            {
                firstVisible = false;
                if (allowAdjust && IsFooterVisible)
                    firstLine = visibleLines[visibleLines.FirstFooterVisibleIndex];
                else
                    firstLine = null;
            }
            // Regular line (Body) - Visible and not a Header or Footer.
            else
            {
                firstVisible = true;
                firstLine = visibleLines.GetVisibleLineNearLineIndex(firstIndex);
            }

            if (lastIndex >= LineCount)
                lastIndex = LineCount - 1;

            // Invalid Line
            if (lastIndex < 0 || lastIndex >= LineCount)
            {
                lastVisible = false;
                lastLine = null;
            }
            // Header
            else if (lastIndex < HeaderLineCount)
            {
                lastVisible = true;
                lastLine = visibleLines.GetVisibleLineNearLineIndex(lastIndex);
            }
            // Footer
            else if (lastIndex >= FirstFooterLineIndex)
            {
                lastVisible = true;
                lastLine = visibleLines.GetVisibleLineNearLineIndex(lastIndex);
            }
            // After Header and Before Scroll Position
            else if (lastIndex < ScrollLineIndex)
            {
                lastVisible = false;
                if (!firstVisible && firstIndex < ScrollLineIndex) // maybe - in case you want right border to look through ...: && lastIndex+1 < ScrollLineIndex)
                {
                    firstLine = null;
                    lastLine = null;
                }
                else
                {
                    if (allowAdjust && HeaderLineCount > 0)
                        lastLine = visibleLines[visibleLines.FirstBodyVisibleIndex - 1];
                    else
                        lastLine = null;
                }
            }
            // After Scroll Position and Before Footer
            else if (lastIndex > LastBodyVisibleLineIndex)
            {
                lastVisible = false;
                if (!firstVisible && firstIndex > LastBodyVisibleLineIndex)
                {
                    firstLine = null;
                    lastLine = null;
                }
                else
                {
                    lastLine = allowAdjust ? LastBodyVisibleLine : null;
                }
            }
            // Regular line (Body) - Visible and not a Header or Footer.
            else
            {
                lastVisible = true;
                lastLine = visibleLines.GetVisibleLineNearLineIndex(lastIndex);
            }
        }

        /// <summary>
        /// Gets the visible lines clip points (clipped origin of first line and clipped 
        /// corner of last line). If both lines are above or below viewable area an empty 
        /// span is returned. If lines are both above and below viewable are then the 
        /// range for all viewable lines is returned.
        /// </summary>
        /// <param name="firstIndex">The first index.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <returns></returns>
        public DoubleSpan GetVisibleLinesClipPoints(int firstIndex, int lastIndex)
        {
            bool firstVisible, lastVisible;
            VisibleLineInfo firstLine, lastLine;

            GetLinesAndVisibility(firstIndex, lastIndex, true, out firstVisible, out lastVisible, out firstLine, out lastLine);
            if (firstLine == null || lastLine == null)
                return DoubleSpan.Empty;

            return new DoubleSpan(firstLine.ClippedOrigin, lastLine.ClippedCorner);
        }

        /// <summary>
        /// Gets a value indicating whether footer lines are visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if footer lines are visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsFooterVisible
        {
            get
            {
                VisibleLinesCollection visibleLines = this.GetVisibleLines();
                return visibleLines.FirstFooterVisibleIndex < visibleLines.Count;
            }
        }

        /// <summary>
        /// Gets the index of the first footer line.
        /// </summary>
        /// <value>The index of the first footer line.</value>
        public int FirstFooterLineIndex
        {
            get
            {
                return LineCount - FooterLineCount;
            }
        }

        /// <summary>
        /// Gets the last visible line.
        /// </summary>
        /// <value>The last visible line.</value>
        public VisibleLineInfo LastBodyVisibleLine
        {
            get
            {
                VisibleLinesCollection visibleLines = this.GetVisibleLines();
                if (visibleLines.Count == 0 || visibleLines.LastBodyVisibleIndex > visibleLines.Count)
                    return null;
                return visibleLines[visibleLines.LastBodyVisibleIndex];
            }
        }

        /// <summary>
        /// Gets the index of the last visible line.
        /// </summary>
        /// <value>The index of the last visible line.</value>
        public int LastBodyVisibleLineIndex
        {
            get
            {
                VisibleLinesCollection visibleLines = this.GetVisibleLines();
                if (visibleLines.Count == 0 || visibleLines.LastBodyVisibleIndex > visibleLines.Count)
                    return -1;
                return visibleLines[visibleLines.LastBodyVisibleIndex].LineIndex;
            }
        }

        /// <summary>
        /// Gets the clip points for a region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <returns></returns>
        public DoubleSpan GetClipPoints(ScrollAxisRegion region)
        {
            VisibleLinesCollection lines = GetVisibleLines();
            int start, end;
            this.GetVisibleSection(region, out start, out end);
            if (start == end && region == ScrollAxisRegion.Body && lines[end].ClippedOrigin > 0 && lines[start].ClippedCorner > 0)
                return new DoubleSpan(lines[start].ClippedOrigin, lines[end].ClippedCorner);
            //if (start == end && (region == ScrollAxisRegion.Header || region == ScrollAxisRegion.Footer) && lines[start].ClippedCorner > 0)
            //       return new DoubleSpan(lines[start].ClippedOrigin, lines[end].ClippedCorner);
            if (end < start)
                return DoubleSpan.Empty;

            //            if (start == 0 && end == lines.Count-1)
            //                return new DoubleSpan(int.MinValue, int.MaxValue);
            //else if (start == 0)
            //                return new DoubleSpan(int.MinValue, lines[end].ClippedCorner);
            //else if (end == lines.Count-1)
            //                return new DoubleSpan(lines[start].ClippedOrigin, int.MaxValue);
            //            else
            return new DoubleSpan(lines[start].ClippedOrigin, lines[end].ClippedCorner);
        }

        /// <summary>
        /// Returns an array with 3 ranges indicating the first and last point for the given lines in each region.
        /// </summary>
        /// <param name="first">The index of the first line.</param>
        /// <param name="last">The index of the last line.</param>
        /// <param name="allowEstimatesForOutOfViewLines">if set to <c>true</c> allow estimates for out of view lines.</param>
        /// <returns></returns>
        public abstract DoubleSpan[] RangeToRegionPoints(int first, int last, bool allowEstimatesForOutOfViewLines);

        /// <summary>
        /// Returns the first and last point for the given lines in a region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="first">The index of the first line.</param>
        /// <param name="last">The index of the last line.</param>
        /// <param name="allowEstimatesForOutOfViewLines">if set to <c>true</c> allow estimates for out of view lines.</param>
        /// <returns></returns>
        public abstract DoubleSpan RangeToPoints(ScrollAxisRegion region, int first, int last, bool allowEstimatesForOutOfViewLines);

        #endregion

        /// <summary>
        /// Raises the changed event.
        /// </summary>
        public void RaiseChanged(ScrollChangedAction action)
        {
            if (Changed != null)
                Changed(this, new ScrollChangedEventArgs(action));
        }

        /// <summary>
        /// Determines the line one page down from the given line.
        /// </summary>
        /// <param name="lineIndex">The current line.</param>
        /// <returns>The line index of the line one page down</returns>
        public int GetNextPage(int lineIndex)
        {
            double extent = 0;
            double pageExtent = ScrollPageSize - GetLineSize(lineIndex);
            int count = LineCount;
            while (extent < pageExtent && lineIndex < count && lineIndex != -1)
            {
                var index = GetNextScrollLineIndex(lineIndex);
                if (index < 0)
                {
                    break;
                }

                lineIndex = index;
                extent += GetLineSize(index);
            }

            return lineIndex;
        }

        /// <summary>
        /// Determines the line one page up from the given line.
        /// </summary>
        /// <param name="lineIndex">The current line.</param>
        /// <returns>The line index of the line one page up</returns>
        public int GetPreviousPage(int lineIndex)
        {
            double extent = 0;
            double pageExtent = ScrollPageSize - GetLineSize(lineIndex);
            while (extent < pageExtent && lineIndex > 0)
            {
                lineIndex = GetPreviousScrollLineIndex(lineIndex);
                extent += GetLineSize(lineIndex);
            }

            return lineIndex;
        }
    }

    /// <summary>
    /// Corner side enumeration.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public enum CornerSide
    {
        /// <summary>
        /// Includes both Left and right side or Top and Bottom side.
        /// </summary>
        Both = 0,
        /// <summary>
        /// Left side alone.
        /// </summary>
        Left = 1,
        /// <summary>
        /// Right side alone.
        /// </summary>
        Right = 2,
        /// <summary>
        /// Top side alone.
        /// </summary>
        Top,
        /// <summary>
        /// Bottom side alone.
        /// </summary>
        Bottom
    }
}
