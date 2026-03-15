#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Collections;

// TODO: See if DistanceCollection can be shared once implementing splitter view.
// TODO: Expect issues when hiding rows or columns that are in header or footer area.
// TODO: Support for hiding lines in header or footer area ... Needed: VisibleFooterLineCount
#if !WinRT
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Scroll
#else
using Syncfusion.WinRT.GridCommon;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{
    /// <summary>
    /// The LineScrollAxis implements scrolling only for whole lines. You can
    /// hide lines and LineScrollAxis provides a mapping mechanism between the
    /// index of the line and the scroll index and vice versa. Hidden lines
    /// are not be counted when the scroll index is determined for a line.
    /// <para/>
    /// The LineScrollAxis does not support scrolling in between lines (pixel scrolling).
    /// This can be of advantage if you have a large number of lines with varying
    /// line sizes. In such case the LineScrollAxis does not need to maintain
    /// a collection that tracks line sizes whereas the <see cref="PixelScrollAxis"/> does need to.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class LineScrollAxis : ScrollAxisBase
    {
        /// <summary>
        /// distances holds the visible lines. Each visible line
        /// has a distance of 1.0. Hidden lines have a distance of 0.0.
        /// </summary>
        IDistanceCounterCollection distances = new DistanceRangeCounterCollection();
        double headerExtent;
        int headerLineCount;
        double footerExtent;
        int footerLineCount;
        double defaultLineSize;
        double viewSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineScrollAxis"/> class.
        /// </summary>
        /// <param name="sb">The state of the scrollbar.</param>
        /// <param name="scrollLinesHost">The scroll lines host.</param>
        public LineScrollAxis(IScrollBar sb, ILineSizeHost scrollLinesHost)
            : base(sb, scrollLinesHost)
        {
            var distancesHost = scrollLinesHost as IDistancesHost;
            if (distancesHost != null)
            {
                distances = distancesHost.Distances;
            }
            else
            {
                distances = new DistanceRangeCounterCollection();
            }
            if (scrollLinesHost != null)
                scrollLinesHost.InitializeScrollAxis(this);
            distances.DefaultDistance = 1.0;
        }

        /// <summary>
        /// Gets or sets the line count.
        /// </summary>
        /// <value>The line count.</value>
        public override int LineCount
        {
            get
            {
                return distances.Count;
            }
            set
            {
                if (LineCount != value)
                {
                    distances.Count = value;
                    UpdateScrollBar();
                }
            }
        }

        /// <summary>
        /// Gets or sets the default size of lines.
        /// </summary>
        /// <value>The default size of lines.</value>
        public override double DefaultLineSize
        {
            get
            {
                return defaultLineSize;
            }
            set
            {
                if (DefaultLineSize != value)
                {
                    defaultLineSize = value;

                    UpdateDistances();

                    UpdateScrollBar();
                }
            }
        }


        /// <summary>
        /// Updates the linesize for visible lines to be "1" for LineScrollAxis
        /// </summary>
        private void UpdateDistances()
        {
            int repeatSizeCount;

            for (int index = 0; index < LineCount; index++)
            {
                bool hide = ScrollLinesHost.GetHidden(index, out repeatSizeCount);
                var value = hide == true ? 0 : 1.0;
                int rangeTo = GetRangeToHelper(index, LineCount - 1, repeatSizeCount);
                distances.SetRange(index, rangeTo, value);
                index = rangeTo;
            }
        }

        /// <summary>
        /// Gets the header extent. This is total height (or width) of the header lines.
        /// </summary>
        /// <value>The header extent.</value>
        public override double HeaderExtent
        {
            get
            {
                return this.headerExtent;
            }
        }

        /// <summary>
        /// Sets the header line count.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override void SetHeaderLineCount(int value)
        {
            double size = 0.0;
            int lines = 0;
            while (lines < value)
                size += GetLineSize(lines++);

            this.headerLineCount = value;
            this.headerExtent = size;
        }

        /// <summary>
        /// Gets the footer extent. This is total height (or width) of the footer lines.
        /// </summary>
        /// <value>The footer extent.</value>
        public override double FooterExtent
        {
            get
            {
                return this.footerExtent;
            }
        }

        /// <summary>
        /// Sets the footer line count.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override void SetFooterLineCount(int value)
        {
            double size = 0.0;
            int lines = 0;
            int index = LineCount - 1;
            while (lines < value)
            {
                size += GetLineSize(index--);
                lines++;
            }
            this.footerLineCount = lines;
            this.footerExtent = size;
        }

        // Scroll = First Visible Body Line
        /// <summary>
        /// Gets or sets the index of the first visible Line in the Body region.
        /// </summary>
        /// <value>The index of the scroll line.</value>
        public override int ScrollLineIndex
        {
            get
            {
                return ScrollBarValueToLineIndex(ScrollBar.Value);
            }
            set
            {
                SetScrollLineIndex(value, 0.0);
            }
        }

        /// <summary>
        /// Gets the index of the scroll line.
        /// </summary>
        /// <param name="scrollLindeIndex">Index of the scroll linde.</param>
        /// <param name="scrollLineDelta">The scroll line delta.</param>
        public override void GetScrollLineIndex(out int scrollLindeIndex, out double scrollLineDelta)
        {
            scrollLindeIndex = ScrollBarValueToLineIndex(ScrollBar.Value);
            scrollLineDelta = 0.0;
        }

        /// <summary>
        /// Sets the index of the scroll line.
        /// </summary>
        /// <param name="scrollLindeIndex">Index of the scroll linde.</param>
        /// <param name="scrollLineDelta">The scroll line delta.</param>
        public override void SetScrollLineIndex(int scrollLindeIndex, double scrollLineDelta)
        {
            scrollLindeIndex = Math.Min(distances.Count - 1, Math.Max(0, scrollLindeIndex));
            ScrollBar.Value = LineIndexToScrollBarValue(scrollLindeIndex);
            ResetVisibleLines();
        }

        /// <summary>
        /// Sets the hidden state of the lines.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="hide">if set to <c>true</c> [hide].</param>
        public override void SetLineHiddenState(int from, int to, bool hide)
        {
            if (hide)
                distances.SetRange(from, to, 0.0);
            else
                distances.SetRange(from, to, 1.0);
        }

        /// <summary>
        /// Sets the size of the lines. Will do nothing for a <see cref="LineScrollAxis"/>
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="size">The size.</param>
        public override void SetLineSize(int from, int to, double size)
        {
        }

        /// <summary>
        /// Gets the view size of the (either height or width) of the parent control. Normally
        /// the ViewSize is the same as <see cref="ScrollAxisBase.RenderSize"/>. Only if the parent control
        /// has more space then needed to display all lines, the ViewSize will be less. In
        /// such case the ViewSize is the total height for all lines.
        /// </summary>
        /// <value>The size of the view.</value>
        public override double ViewSize
        {
            get
            {
                if (ScrollBar.Value + ScrollBar.LargeChange > ScrollBar.Maximum)
                    return viewSize;
                else
                    return RenderSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this axis supports pixel scrolling.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance supports pixel scrolling; otherwise, <c>false</c>.
        /// </value>
        public override bool IsPixelScroll
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Initialize scrollbar properties from line count in header, footer and body.
        /// </summary>
        public override void UpdateScrollBar()
        {
            SetHeaderLineCount(HeaderLineCount);
            SetFooterLineCount(FooterLineCount);

            IScrollBar sb = ScrollBar;
            bool isMinimum = sb.Minimum == sb.Value;
            sb.Minimum = HeaderLineCount;
            sb.Maximum = distances.TotalDistance - FooterLineCount - 1;
            sb.SmallChange = 1;

            sb.LargeChange = DetermineLargeChange();
            sb.Value = isMinimum ? sb.Minimum : Math.Max(sb.Minimum, Math.Min(sb.Maximum, sb.Value));
        }

        int DetermineLargeChange()
        {
            double sbValue = ScrollBar.Maximum;
            double abortSize = ScrollPageSize;
            int count = 0;
            viewSize = 0;

            while (sbValue >= ScrollBar.Minimum)
            {
                int lineIndex = ScrollBarValueToLineIndex(sbValue);
                double size = GetLineSize(lineIndex);
                if (viewSize + size > abortSize)
                    break;

                count++;
                sbValue--;
                viewSize += size;
            }

            viewSize += FooterExtent + HeaderExtent;

            return count;
        }

        int ScrollBarValueToLineIndex(double sbValue)
        {
            return distances.IndexOfCumulatedDistance(sbValue);
        }

        double LineIndexToScrollBarValue(int lineIndex)
        {
            return distances.GetCumulatedDistanceAt(lineIndex);
        }

        /// <summary>
        /// Gets the index of the previous scroll line.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override int GetPreviousScrollLineIndex(int index)
        {
#if WinRT
            if (distances.Count > index)
                return distances.GetPreviousVisibleIndex(index);
            else
                return 0;
#else
            return distances.GetPreviousVisibleIndex(index);
#endif
        }

        /// <summary>
        /// Gets the index of the next scroll line.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override int GetNextScrollLineIndex(int index)
        {
#if WinRT
            if (distances.Count > index)
                return distances.GetNextVisibleIndex(index);
            else
                return 0;
#else
            return distances.GetNextVisibleIndex(index);
#endif
        }

        /// <summary>
        /// Scrolls to next page.
        /// </summary>
        public override void ScrollToNextPage()
        {
            int index = VisiblePointToLineIndex(ScrollPageSize, true);
            if (index == ScrollLineIndex)
                index = GetNextScrollLineIndex(index);
            ScrollLineIndex = index;
        }

        /// <summary>
        /// Scrolls to previous page.
        /// </summary>
        public override void ScrollToPreviousPage()
        {
            int index = IntPreviousPageLineIndex(ScrollPageSize, ScrollLineIndex);
            if (index == ScrollLineIndex)
                index = GetPreviousScrollLineIndex(index);
            ScrollLineIndex = index;
        }

        private int IntPreviousPageLineIndex(double p, int index)
        {
            int result = index;
            while (p > 0)
            {
                result = index;
                index = GetPreviousScrollLineIndex(index);
                if (index == -1)
                    return -1;
                p -= GetLineSize(index);
            }
            return result;
        }

        /// <summary>
        /// Scrolls to next line.
        /// </summary>
        public override void ScrollToNextLine()
        {
            ScrollLineIndex = GetNextScrollLineIndex(ScrollLineIndex);
        }

        /// <summary>
        /// Scrolls to previous line.
        /// </summary>
        public override void ScrollToPreviousLine()
        {
            ScrollLineIndex = GetPreviousScrollLineIndex(ScrollLineIndex);
        }

        /// <summary>
        /// Aligns the scroll line.
        /// </summary>
        public override void AlignScrollLine()
        {
        }

        /// <summary>
        /// Returns an array with 3 ranges indicating the first and last point for the given lines in each region.
        /// </summary>
        /// <param name="first">The index of the first line.</param>
        /// <param name="last">The index of the last line.</param>
        /// <param name="allowEstimatesForOutOfViewLines">if set to <c>true</c> allow estimates for out of view lines.</param>
        /// <returns></returns>
        public override DoubleSpan[] RangeToRegionPoints(int first, int last, bool allowEstimatesForOutOfViewLines)
        {
            DoubleSpan[] result = new DoubleSpan[3];
            for (int n = 0; n < 3; n++)
                result[n] = RangeToPoints((ScrollAxisRegion)n, first, last, allowEstimatesForOutOfViewLines);
            return result;
            //new DoubleSpan[3] { DoubleSpan.Empty, DoubleSpan.Empty, DoubleSpan.Empty };
        }

        /// <summary>
        /// Returns the first and last point for the given lines in a region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="first">The index of the first line.</param>
        /// <param name="last">The index of the last line.</param>
        /// <param name="allowEstimatesForOutOfViewLines">if set to <c>true</c> allow estimates for out of view lines.</param>
        /// <returns></returns>
        public override DoubleSpan RangeToPoints(ScrollAxisRegion region, int first, int last, bool allowEstimatesForOutOfViewLines)
        {
            VisibleLinesCollection visibleLines = this.GetVisibleLines();
            bool firstVisible, lastVisible;
            VisibleLineInfo firstLine, lastLine;

            GetLinesAndVisibility(first, last, true, out firstVisible, out lastVisible, out firstLine, out lastLine);

            if (firstLine == null || lastLine == null)
                return DoubleSpan.Empty;

            if (allowEstimatesForOutOfViewLines)
            {
                switch (region)
                {
                    case ScrollAxisRegion.Header:
                        if (!firstLine.IsHeader)
                            return DoubleSpan.Empty;
                        break;
                    case ScrollAxisRegion.Footer:
                        if (!lastLine.IsFooter)
                            return DoubleSpan.Empty;
                        break;
                    case ScrollAxisRegion.Body:
                        if (firstLine.IsFooter || lastLine.IsHeader)
                            return DoubleSpan.Empty;
                        break;
                } // switch

                return new DoubleSpan(firstLine.Origin, lastLine.Corner);
            }
            else
            {
                switch (region)
                {
                    case ScrollAxisRegion.Header:
                        {
                            if (!firstLine.IsHeader)
                                return DoubleSpan.Empty;

                            if (!lastVisible || !lastLine.IsHeader)
                            {
                                double corner = firstLine.Corner;
                                for (int n = firstLine.LineIndex + 1; n <= last; n++)
                                {
                                    corner += GetLineSize(n);
                                }

                                return new DoubleSpan(firstLine.Origin, corner);
                            }

                            return new DoubleSpan(firstLine.Origin, lastLine.Corner);
                        }

                    case ScrollAxisRegion.Footer:
                        {
                            if (!lastLine.IsFooter)
                                return DoubleSpan.Empty;

                            if (!firstVisible || !firstLine.IsFooter)
                            {
                                double origin = lastLine.Origin;
                                for (int n = lastLine.LineIndex - 1; n >= first; n--)
                                {
                                    origin -= GetLineSize(n);
                                }

                                return new DoubleSpan(origin, lastLine.Corner);
                            }

                            return new DoubleSpan(firstLine.Origin, lastLine.Corner);
                        }

                    case ScrollAxisRegion.Body:
                        {
                            if (firstLine.IsFooter || lastLine.IsHeader)
                                return DoubleSpan.Empty;

                            double origin = firstLine.Origin;
                            if (!firstVisible || firstLine.Region != ScrollAxisRegion.Body)
                            {
                                origin = HeaderExtent;
                                for (int n = ScrollLineIndex - 1; n >= first; n--)
                                {
                                    origin -= GetLineSize(n);
                                }
                            }

                            double corner = lastLine.Corner;
                            if (!lastVisible || lastLine.Region != ScrollAxisRegion.Body)
                            {
                                corner = LastBodyVisibleLine.Corner;
                                for (int n = LastBodyVisibleLine.LineIndex + 1; n <= last; n++)
                                {
                                    corner += GetLineSize(n);
                                }
                            }

                            return new DoubleSpan(origin, corner);
                        }
                } // switch

                return new DoubleSpan(firstLine.Origin, lastLine.Corner);
            }
        }

        /// <summary>
        /// This method is called in response to a MouseWheel event.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public override void MouseWheel(int delta)
        {
            if (delta > 0)
                ScrollToPreviousLine();
            else
                ScrollToNextLine();
        }

        /// <summary>
        /// Scrolls the line into viewable area.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <param name="lineSize"></param>
        public override void ScrollInView(int lineIndex, double lineSize)
        {
            VisibleLinesCollection lines = GetVisibleLines();
            VisibleLineInfo line = lines.GetVisibleLineAtLineIndex(lineIndex);
            double delta = 0;

            if (line != null)
            {
                if (!line.IsClipped || line.IsFooter || line.IsHeader)
                    return;

                if (line.IsClippedOrigin && !line.IsClippedCorner)
                    delta = -1;

                else if (!line.IsClippedOrigin && line.IsClippedCorner)
                {
                    double y = line.Size - line.ClippedSize;
                    int scrollIndex = this.ScrollLineIndex;
                    double visibleScrollIndex = ScrollBar.Value;
                    while (y > 0 && visibleScrollIndex < ScrollBar.Maximum)
                    {
                        delta++;
                        visibleScrollIndex++;
                        y -= GetLineSize(ScrollBarValueToLineIndex(visibleScrollIndex));
                    }
                }
            }
            else
            {
                double visibleScrollIndex = LineIndexToScrollBarValue(lineIndex);

                if (visibleScrollIndex > ScrollBar.Value)
                {
                    int scrollIndexLinex = this.IntPreviousPageLineIndex(ScrollPageSize - GetLineSize(lineIndex), lineIndex);
                    visibleScrollIndex = LineIndexToScrollBarValue(scrollIndexLinex);
                }
                delta = visibleScrollIndex - ScrollBar.Value;
            }

            if (delta != 0)
            {
                ScrollBar.Value += delta;
            }

            base.ScrollInView(lineIndex, lineSize);
        }

        /// <summary>
        /// Called when lines were removed in ScrollLinesHost.
        /// </summary>
        /// <param name="removeAt">Index of the first removed line.</param>
        /// <param name="count">The count.</param>
        protected override void OnLinesRemoved(int removeAt, int count)
        {
            distances.Remove(removeAt, count);
        }

        /// <summary>
        /// Called when lines were inserted in ScrollLinesHost.
        /// </summary>
        /// <param name="insertAt">Index of the first inserted line.</param>
        /// <param name="count">The count.</param>
        protected override void OnLinesInserted(int insertAt, int count)
        {
            distances.Insert(insertAt, count);

            int to = insertAt + count - 1;
            int repeatSizeCount;

            for (int index = insertAt; index <= to; index++)
            {
                bool hide = ScrollLinesHost.GetHidden(index, out repeatSizeCount);
#if WinRT
                var value = hide == true ? 0 : 1.0;
                int rangeTo = GetRangeToHelper(index, to, repeatSizeCount);
                distances.SetRange(index, rangeTo, value);
                index = rangeTo;
#else
                if (hide)
                {
                    int rangeTo = GetRangeToHelper(index, to, repeatSizeCount);
                    distances.SetRange(index, rangeTo, 0.0);
                    index = rangeTo;
                }
#endif
            }

            //if (distances != null)
            //    DistancesUtil.OnInserted(distances, ScrollLinesHost, insertAt, count);

        }

    }

}
