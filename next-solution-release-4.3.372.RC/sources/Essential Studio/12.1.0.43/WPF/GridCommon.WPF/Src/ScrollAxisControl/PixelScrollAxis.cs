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
using Syncfusion.Windows.GridCommon;

// TODO: Test setting DefaultLineSize. Some changes might be needed for setting DefaultLineSize in nested or shared layout scenarios.

namespace Syncfusion.Windows.Controls.Scroll
{
    /// <summary>
    /// PixelScrollAxis implements scrolling logic for both horizontal and vertical 
    /// scrolling in a <see cref="ScrollAxisControl"/>.
    /// Logical units in the ScrollAxisBase are called "Lines". With the 
    /// <see cref="ScrollAxisControl.ScrollRows"/> a line representes rows in a grid 
    /// and with <see cref="ScrollAxisControl.ScrollRows"/> a line represents columns in a grid.
    /// <para/>
    /// PixelScrollAxis supports pixel scrolling and calculates the total height or
    /// width of all lines.
    /// </summary>
    public class PixelScrollAxis : ScrollAxisBase
    {
        /// <summary>
        /// Distances holds the line sizes. Hidden lines
        /// have a distance of 0.0. 
        /// </summary>        
        IDistanceCounterCollection distances;
        IDistancesHost distancesHost;
        double headerExtent;
        double footerExtent;
        ScrollAxisBase parentScrollAxis;

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="PixelScrollAxis"/> class which 
        /// is nested as a single line in a parent scroll axis.
        /// </summary>
        /// <param name="parentScrollAxis">The parent scroll axis.</param>
        /// <param name="sb">The scrollbar state.</param>
        /// <param name="scrollLinesHost">The scroll lines host.</param>
        /// <param name="distancesHost">The distances host.</param>
        public PixelScrollAxis(ScrollAxisBase parentScrollAxis, IScrollBar sb, ILineSizeHost scrollLinesHost, IDistancesHost distancesHost)
            : base(sb, scrollLinesHost)
        {
            // GridCellGridRenderer passes in Distances. LineSizeCollection holds them. 
            // This allows faster construction of grids when they were scrolled out of view
            // and unloaded.
            this.parentScrollAxis = parentScrollAxis;
            this.distancesHost = distancesHost;
            if (Distances == null)
                throw new ArgumentNullException("Distances");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelScrollAxis"/> class.
        /// </summary>
        /// <param name="sb">The scrollbar state.</param>
        /// <param name="scrollLinesHost">The scroll lines host.</param>
        /// <param name="distancesHost">The distances host.</param>
        public PixelScrollAxis(IScrollBar sb, ILineSizeHost scrollLinesHost, IDistancesHost distancesHost)
            : base(sb, scrollLinesHost)
        {
            if (distancesHost != null)
            {
                this.distancesHost = distancesHost;
            }
            else if (scrollLinesHost != null)
            {
                this.distances = new DistanceRangeCounterCollection();
                this.distances.DefaultDistance = scrollLinesHost.GetDefaultLineSize();
                scrollLinesHost.InitializeScrollAxis(this);
            }
            if (Distances == null)
                throw new ArgumentNullException("Distances");
        }
        #endregion

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
                return true;
            }
        }

        /// <summary>
        /// Gets the distances collection which is used internally
        /// for mapping from a point position to
        /// a line index and vice versa.
        /// </summary>
        /// <value>The distances collection.</value>
        public IDistanceCounterCollection Distances
        {
            get
            {
                if (distancesHost != null)
                    return distancesHost.Distances;
                return distances;
            }
        }

        #region LineCount, DefaultLineSize, HiddenState and LineSizes

        /// <summary>
        /// Gets the total extent of all line sizes.
        /// </summary>
        /// <value>The total extent.</value>
        public double TotalExtent
        {
            get
            {
                return Distances.TotalDistance;
            }
        }

        /// <summary>
        /// Gets or sets the line count.
        /// </summary>
        /// <value>The line count.</value>
        public override int LineCount
        {
            get
            {
                return Distances.Count;
            }
            set
            {
                if (LineCount != value)
                {
                    if (distances != null)
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
                return Distances.DefaultDistance;
            }
            set
            {
                if (DefaultLineSize != value)
                {
                    if (distances != null)
                    {
                        distances.DefaultDistance = value;
                        distances.Clear();

                        if (ScrollLinesHost != null)
                            ScrollLinesHost.InitializeScrollAxis(this);
                    }

                    UpdateScrollBar();

                    if (parentScrollAxis != null)
                        parentScrollAxis.SetLineSize(StartLineIndex, StartLineIndex, Distances.TotalDistance);
                }
            }
        }


        /// <summary>
        /// Sets the hidden state of the lines.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="hide">if set to <c>true</c> hide lines.</param>
        public override void SetLineHiddenState(int from, int to, bool hide)
        {
            if (hide)
                this.SetLineSize(from, to, 0.0);
            else
            {
                for (int n = from; n <= to; n++)
                {
                    int repeatSizeCount;
                    double size = GetLineSize(n, out repeatSizeCount);
                    int rangeTo = GetRangeToHelper(n, to, repeatSizeCount);
                    this.SetLineSize(n, rangeTo, size);
                    n = rangeTo;
                }
            }
        }

        /// <summary>
        /// Sets the size of the lines.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="size">The size.</param>
        public override void SetLineSize(int from, int to, double size)
        {
            if (distances != null)
                distances.SetRange(from, to, size);
                
                // special case for SetLineResize when axis is nested. Parent Scroll Axis relies on Distances.TotalDistance
                // and this only gets updated if we temporarily set the value in the collection.
            else if (distancesHost != null && inLineResize)	  
                distancesHost.Distances.SetRange(from, to, size);

            //if (from < HeaderLineCount)
            //    SetHeaderLineCount(HeaderLineCount);
        }

        #endregion
        
        #region LineResize
        bool inLineResize = false;
        /// <summary>
        /// Set temporary value for a line size during a resize operation without commiting
        /// value to SrollLinesHost.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="size">The size.</param>
        public override void SetLineResize(int index, double size)
        {
            inLineResize = true;
            if (Distances.GetNestedDistances(index) == null)
                base.SetLineResize(index, size);
            else
            {
                MarkDirty();
                RaiseChanged();
            }
            if (parentScrollAxis != null)
                parentScrollAxis.SetLineResize(StartLineIndex, Distances.TotalDistance);
            inLineResize = false;
        }

        /// <summary>
        /// Resets temporary value for line size after a resize operation
        /// </summary>
        public override void ResetLineResize()
        {
            base.ResetLineResize();
            if (parentScrollAxis != null)
                parentScrollAxis.ResetLineResize();
        }

        #endregion

        #region Header and Footer

        /// <summary>
        /// Gets the header extent. This is total height (or width) of the header lines.
        /// </summary>
        /// <value>The header extent.</value>
        public override double HeaderExtent
        {
            get
            {
                GetVisibleLines();
                return this.headerExtent;
            }
        }

        /// <summary>
        /// Sets the header line count.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override void SetHeaderLineCount(int value)
        {
            headerExtent = Distances.GetCumulatedDistanceAt(Math.Min(value, Distances.Count));
        }

        /// <summary>
        /// Gets the footer extent. This is total height (or width) of the footer lines.
        /// </summary>
        /// <value>The footer extent.</value>
        public override double FooterExtent
        {
            get
            {
                GetVisibleLines();
                return this.footerExtent;
            }
        }

        /// <summary>
        /// Sets the footer line count.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override void SetFooterLineCount(int value)
        {
            if (value == 0)
                footerExtent = 0;
            else
            {
                int n = Distances.Count - value;
                // The Total distance must be reduced by the padding size of the Distance total size. Then it should be calculated. This issue is occured in Nested Grid. SD 9312.
                // this will give the exact size of the footer Extent when padding distance is reduced from total distance. 

                if (!(Distances is DistanceCounterSubset)) // Nested Grid cells in GridControl is not DistanceRangeCounterCollection type.
                footerExtent = Distances.TotalDistance - ((Syncfusion.Windows.Controls.Scroll.DistanceRangeCounterCollection)(Distances)).paddingDistance - Distances.GetCumulatedDistanceAt(n);
                else
                footerExtent = Distances.TotalDistance  - Distances.GetCumulatedDistanceAt(n);
            }
        }

        #endregion

        #region Scrolling

        /// <summary>
        /// Initialize scrollbar properties from header and footer size and total size of lines in body.
        /// </summary>
        public override void UpdateScrollBar()
        {
            IScrollBar sb = ScrollBar;

            //sb.BeginUpdate();

            // Adjust Scroll Position when header row or column is resized.
            SetHeaderLineCount(HeaderLineCount);
            SetFooterLineCount(FooterLineCount); 

            double delta = HeaderExtent - sb.Minimum;
            double oldValue = sb.Value;

            sb.Minimum = HeaderExtent;
            sb.Maximum = Distances.TotalDistance - FooterExtent;
            sb.SmallChange = Distances.DefaultDistance;
            double proposeLargeChange = ScrollPageSize;
            sb.LargeChange = proposeLargeChange;
            // SH - Added check for delta != 0 to avoid value being reset when 
            // you resize grid such that only header columns are visible and then
            // resize back to larger size.
            // SH 6/22 - Commented out change to sb.Value and also modified ScrollInfo.Value to 
            // return Math.Max(minimum, Math.Min(maximum - largeChange, value));
            // instead.
            //if (proposeLargeChange >= 0) 
            //    sb.Value = Math.Max(sb.Minimum, Math.Min(sb.Maximum - sb.LargeChange, sb.Value + delta));
            if (proposeLargeChange >= 0 && delta != 0)
                sb.Value = oldValue + delta;

            //sb.EndUpdate();

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
                return Distances.IndexOfCumulatedDistance(ScrollBar.Value);
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
            scrollLindeIndex = Math.Max(0, Distances.IndexOfCumulatedDistance(ScrollBar.Value));
            if (scrollLindeIndex >= LineCount)
                scrollLineDelta = 0;
            else
                scrollLineDelta = ScrollBar.Value - Distances.GetCumulatedDistanceAt(scrollLindeIndex);
        }

        /// <summary>
        /// Sets the index of the scroll line.
        /// </summary>
        /// <param name="scrollLindeIndex">Index of the scroll linde.</param>
        /// <param name="scrollLineDelta">The scroll line delta.</param>
        public override void SetScrollLineIndex(int scrollLindeIndex, double scrollLineDelta)
        {
            scrollLindeIndex = Math.Min(LineCount, Math.Max(0, scrollLindeIndex));
            ScrollBar.Value = Distances.GetCumulatedDistanceAt(scrollLindeIndex) + scrollLineDelta;
            ResetVisibleLines();
        }

        /// <summary>
        /// Gets the index of the previous scroll line.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override int GetPreviousScrollLineIndex(int index)
        {
            double point = Distances.GetCumulatedDistanceAt(index);
            point--;
            if (point > -1)
            {
                return Distances.IndexOfCumulatedDistance(point);
            }

            return -1;
        }

        /// <summary>
        /// Gets the index of the next scroll line.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override int GetNextScrollLineIndex(int index)
        {
            return Distances.GetNextVisibleIndex(index);
        }

        /// <summary>
        /// Scrolls to next page.
        /// </summary>
        public override void ScrollToNextPage()
        {
            ScrollBar.Value += Math.Max(ScrollBar.SmallChange, ScrollBar.LargeChange - ScrollBar.SmallChange);
            ScrollToNextLine();
        }

        /// <summary>
        /// Scrolls to previous page.
        /// </summary>
        public override void ScrollToPreviousPage()
        {
            ScrollBar.Value -= Math.Max(ScrollBar.SmallChange, ScrollBar.LargeChange - ScrollBar.SmallChange);
            AlignScrollLine();
        }

        /// <summary>
        /// Scrolls to next line.
        /// </summary>
        public override void ScrollToNextLine()
        {
            double d = Distances.GetNextScrollValue(ScrollBar.Value);
            if (!double.IsNaN(d))
            {
                if (d <= ScrollBar.Value)
                {
                    ScrollBar.Value = Distances.GetNextScrollValue(ScrollBar.Value + 1);
                }
                else
                {
                    ScrollBar.Value = d;
                }
            }
            else
                ScrollBar.Value += ScrollBar.SmallChange;
        }

        /// <summary>
        /// Scrolls to previous line.
        /// </summary>
        public override void ScrollToPreviousLine()
        {
            if (Distances != null && ScrollBar != null)
            {
                double d = Distances.GetPreviousScrollValue(ScrollBar.Value);
                if (!double.IsNaN(d))
                    ScrollBar.Value = d;
            }
        }

        /// <summary>
        /// Aligns the scroll line.
        /// </summary>
        public override void AlignScrollLine()
        {
            double d = Distances.GetAlignedScrollValue(ScrollBar.Value);
            if (!double.IsNaN(d))
                ScrollBar.Value = d;
        }
        #endregion

        #region Nested Lines
        /// <summary>
        /// Associates a collection of nested lines with a line in this axis.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="nestedLines">The nested lines.</param>
        public void SetNestedLines(int index, IDistanceCounterCollection nestedLines)
        {
            if (distances != null)
                distances.SetNestedDistances(index, nestedLines);
        }

        #endregion

        #region RenderSize, ViewSize and Clip

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
                return Math.Min(RenderSize, Distances.TotalDistance);
            }
        }

        #endregion

        #region Layout, HitTest and Clip Calculations

        /// <summary>
        /// Returns an array with 3 ranges indicating the first and last point for the given lines in each region.
        /// </summary>
        /// <param name="first">The index of the first line.</param>
        /// <param name="last">The index of the last line.</param>
        /// <param name="allowEstimatesForOutOfViewLines">if set to <c>true</c> allow estimates for out of view lines.</param>
        /// <returns></returns>
        public override DoubleSpan[] RangeToRegionPoints(int first, int last, bool allowEstimatesForOutOfViewLines)
        {
            double p1, p2;
            p1 = Distances.GetCumulatedDistanceAt(first);
            p2 = last >= Distances.Count - 1 ? Distances.TotalDistance : Distances.GetCumulatedDistanceAt(last + 1);

            DoubleSpan[] result = new DoubleSpan[3];
            for (int n = 0; n < 3; n++)
                result[n] = RangeToPointsHelper((ScrollAxisRegion)n, p1, p2);

            return result;
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
            VisibleLinesCollection lines = GetVisibleLines();

            // If line is visible use already calculated values,
            // otherwise get value from Distances
            VisibleLineInfo line1 = lines.GetVisibleLineAtLineIndex(first);
            VisibleLineInfo line2 = lines.GetVisibleLineAtLineIndex(last);
 
            double p1, p2;
            if (line1 == null)
                p1 = Distances.GetCumulatedDistanceAt(first);
            else
                p1 = GetCumulatedOrigin(line1);
       
            if (line2 == null)
                p2 = Distances.GetCumulatedDistanceAt(last + 1);
            else
                p2 = GetCumulatedCorner(line2);

            return RangeToPointsHelper(region, p1, p2);

        }

        /// <summary>
        /// Gets the cumulated origin taking scroll position into account. The
        /// returned value is between ScrollBar.Minimum and ScrollBar.Maximum.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private double GetCumulatedOrigin(VisibleLineInfo line)
        {
            VisibleLinesCollection lines = GetVisibleLines();
            if (line.IsHeader)
                return line.Origin;
            else if (line.IsFooter)
                return ScrollBar.Maximum - lines[lines.firstFooterVisibleIndex].Origin + line.Origin;

            return line.Origin - ScrollBar.Minimum + ScrollBar.Value;
        }

        /// <summary>
        /// Gets the cumulated corner taking scroll position into account. The
        /// returned value is between ScrollBar.Minimum and ScrollBar.Maximum.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private double GetCumulatedCorner(VisibleLineInfo line)
        {
            VisibleLinesCollection lines = GetVisibleLines();
            if (line.IsHeader)
                return line.Corner;
            else if (line.IsFooter)
                return ScrollBar.Maximum - lines[lines.firstFooterVisibleIndex].Origin + line.Corner;

            return line.Corner - ScrollBar.Minimum + ScrollBar.Value;
        }

        private DoubleSpan RangeToPointsHelper(ScrollAxisRegion region, double p1, double p2)
        {
            VisibleLinesCollection lines = GetVisibleLines();
            switch (region)
            {
                case ScrollAxisRegion.Header:
                    if (HeaderLineCount > 0)
                        return new DoubleSpan(p1, p2);
                    else
                        return DoubleSpan.Empty;

                case ScrollAxisRegion.Footer:
                    if (IsFooterVisible)
                    {
                        VisibleLineInfo l = lines[lines.FirstFooterVisibleIndex];
                        double p3 = Distances.TotalDistance - this.FooterExtent;
                        p1 += l.Origin - p3;
                        p2 += l.Origin - p3;
                        return new DoubleSpan(p1, p2);
                    }
                    else
                        return DoubleSpan.Empty;

                case ScrollAxisRegion.Body:
                    p1 += HeaderExtent - ScrollBar.Value;
                    p2 += HeaderExtent - ScrollBar.Value;
                    return new DoubleSpan(p1, p2);
            }

            return DoubleSpan.Empty;
        }

        #endregion

        /// <summary>
        /// This method is called in response to a MouseWheel event.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public override void MouseWheel(int delta)
        {
            ScrollBar.Value -= delta;
        }

        /// <summary>
        /// Scrolls the line into viewable area.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <param name="lineSize">Size of the line.</param>
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
                    delta = -(line.Size - line.ClippedSize);

                else if (!line.IsClippedOrigin && line.IsClippedCorner)
                {
                    ///Following code prevent the horizontal auto scrolling when column size is bigger than viewPort size.
                    if (line.ClippedOrigin < line.ClippedSize)
                        delta = 0;
                    else
                        delta = line.Size - line.ClippedSize;
                }
            }
            else
            {
                double d = Distances.GetCumulatedDistanceAt(lineIndex);

                if (d > ScrollBar.Value)
                    d = d + lineSize - ScrollBar.LargeChange;

                delta = d - ScrollBar.Value;
            }

            if (delta != 0)
            {
                ScrollBar.Value += delta;
            }
        }

        /// <summary>
        /// Called when lines were removed in ScrollLinesHost.
        /// </summary>
        /// <param name="removeAt">Index of the first removed line.</param>
        /// <param name="count">The count.</param>
        protected override void OnLinesRemoved(int removeAt, int count)
        {
            if (distances != null)
                distances.Remove(removeAt, count);
        }

        /// <summary>
        /// Called when lines were inserted in ScrollLinesHost.
        /// </summary>
        /// <param name="insertAt">Index of the first inserted line.</param>
        /// <param name="count">The count.</param>
        protected override void OnLinesInserted(int insertAt, int count)
        {
            if (distances != null)
                DistancesUtil.OnInserted(distances, ScrollLinesHost, insertAt, count);
        }

        public override void Dispose(bool disposing)
        {
            if (this.distances != null)
            {
                this.distances.Clear();
            }
            if (this.distancesHost != null)
            {
                this.distancesHost.Dispose();
            }
            if (this.parentScrollAxis != null)
            {
                this.parentScrollAxis.Dispose(disposing);
                this.parentScrollAxis = null;
            }
            base.Dispose(disposing);
        }
    }

}
