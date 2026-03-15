#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;



namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// TreeNodesScrollAxis provides support for vertical pixel scrolling through
    /// nodes in a <see cref="TreeModel"/> of a <see cref="VirtualTreeView"/> with its
    /// <see cref="ScrollAxisControl.ScrollRows"/> property.
    /// <para/>
    /// The axis provides the mapping between a row index or pixel position in the VirtualTreeView
    /// and the TreeNodes and vice versa.
    /// <para/>
    /// The TreeNodes maintain their IsExpanded state and also keep track of the height
    /// of themselves and their child nodes in a TreeTable that counts the number of 
    /// nested child nodes and their height.
    /// <para/>
    /// The axis listens to scrollbar events, resizing and hiding of nodes and updates
    /// scrollbar properties with setting calculated from header and footer size and 
    /// total size of lines in body.
    /// </summary>
    public class TreeNodesScrollAxis : ScrollAxisBase
    {
        double headerExtent;
        double footerExtent;
        TreeModel treeModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodesScrollAxis"/> class.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="treeModel">The tree model.</param>
        public TreeNodesScrollAxis(IScrollBar sb, TreeModel treeModel)
            : base(sb, treeModel.VisibleNodes)
        {
            this.treeModel = treeModel;
        }

        /// <summary>
        /// Gets the visible nodes.
        /// </summary>
        /// <value>The visible nodes.</value>
        public TreeNodesFlattenedList VisibleNodes
        {
            get
            {
                return treeModel.VisibleNodes;
            }
        }

        /// <summary>
        /// Gets the total extent.
        /// </summary>
        /// <value>The total extent.</value>
        public double TotalExtent
        {
            get
            {
                return VisibleNodes.TotalHeight;
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
                return VisibleNodes.Count;
            }
            set
            {
                treeModel.RaiseLineCountChanged();
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
                return treeModel.DefaultHeight;
            }
            set
            {
                treeModel.DefaultHeight = value;
                //throw new NotSupportedException();
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
            headerExtent = 0;
            for (int n = 0; n < value; n++)
                headerExtent += this.GetLineSize(n);
            //headerExtent = 20;// value* DefaultLineSize;
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
            footerExtent = 0;
            for (int n = Math.Max(0, VisibleNodes.Count - value); n < VisibleNodes.Count; n++)
                footerExtent += this.GetLineSize(n);
            //footerExtent = value * DefaultLineSize;
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
                double pos = ScrollBar.Value;
                TreeNode node = VisibleNodes.GetTreeTreeNodeAtCumulatedHeight(pos);
                return VisibleNodes.IndexOf(node);
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
            double pos = ScrollBar.Value;
            TreeNode node = VisibleNodes.GetTreeTreeNodeAtCumulatedHeight(pos);
            scrollLindeIndex = VisibleNodes.IndexOf(node);
            if (scrollLindeIndex >= LineCount)
                scrollLineDelta = 0;
            else
                scrollLineDelta = pos - VisibleNodes.GetCumulatedHeight(node);
        }

        /// <summary>
        /// Sets the index of the scroll line.
        /// </summary>
        /// <param name="scrollLindeIndex">Index of the scroll linde.</param>
        /// <param name="scrollLineDelta">The scroll line delta.</param>
        public override void SetScrollLineIndex(int scrollLindeIndex, double scrollLineDelta)
        {
            scrollLindeIndex = Math.Min(LineCount, Math.Max(0, scrollLindeIndex));
            TreeNode node = VisibleNodes[scrollLindeIndex];
            ScrollBar.Value = VisibleNodes.GetCumulatedHeight(node) + scrollLineDelta;
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
                this.SetLineSize(from, to, 0.0);
            else
            {
                for (int n = from; n <= to; n++)
                    this.SetLineSize(n, n, GetLineSize(n));
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
            for (int n = from; n <= to; n++)
            {
                TreeNode node = this.treeModel.VisibleNodes[n];
                node.SetItemHeight(size, false);
            }
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
                return Math.Min(RenderSize, VisibleNodes.TotalHeight);
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
                return true;
            }
        }

        /// <summary>
        /// Initialize scrollbar properties from header and footer size and total size of lines in body.
        /// </summary>
        public override void UpdateScrollBar()
        {
            IScrollBar sb = ScrollBar;
            bool isMinimum = sb.Minimum == sb.Value;
            sb.Minimum = HeaderExtent;
            sb.Maximum = VisibleNodes.TotalHeight - FooterExtent;
            sb.SmallChange = treeModel.DefaultHeight;
            sb.LargeChange = RenderSize - HeaderExtent - FooterExtent;
            sb.Value = isMinimum ? sb.Minimum : Math.Max(sb.Minimum, Math.Min(sb.Maximum - sb.LargeChange, sb.Value));
        }

        /// <summary>
        /// Gets the index of the previous scroll line.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override int GetPreviousScrollLineIndex(int index)
        {
            return index - 1;
        }

        /// <summary>
        /// Gets the index of the next scroll line.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override int GetNextScrollLineIndex(int index)
        {
            return index + 1;
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
            ScrollLineIndex++;
        }

        /// <summary>
        /// Scrolls to previous line.
        /// </summary>
        public override void ScrollToPreviousLine()
        {
            int scrollLindeIndex;
            double scrollLineDelta;
            GetScrollLineIndex(out scrollLindeIndex, out scrollLineDelta);
            if (scrollLineDelta > 0)
                ScrollLineIndex = scrollLindeIndex;
            else
                ScrollLineIndex = scrollLindeIndex - 1;
        }

        /// <summary>
        /// Aligns the scroll line.
        /// </summary>
        public override void AlignScrollLine()
        {
            ScrollLineIndex = ScrollLineIndex;
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
            double p1, p2;
            p1 = VisibleNodes.GetCumulatedHeight(first);
            p2 = last >= VisibleNodes.Count - 1 ? VisibleNodes.TotalHeight : VisibleNodes.GetCumulatedHeight(last + 1);

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
                p1 = VisibleNodes.GetCumulatedHeight(first);
            else
                p1 = GetCumulatedOrigin(line1);
       
            if (line2 == null)
                p2 = VisibleNodes.GetCumulatedHeight(last + 1);
            else
                p2 = GetCumulatedCorner(line2);

            return RangeToPointsHelper(region, p1, p2);

        }

        private double GetCumulatedOrigin(VisibleLineInfo line)
        {
            VisibleLinesCollection lines = GetVisibleLines();
            if (line.IsHeader)
                return line.Origin;
            else if (line.IsFooter)
                return ScrollBar.Maximum - lines[lines.FirstFooterVisibleIndex].Origin + line.Origin;

            return line.Origin - ScrollBar.Minimum + ScrollBar.Value;
        }

        private double GetCumulatedCorner(VisibleLineInfo line)
        {
            VisibleLinesCollection lines = GetVisibleLines();
            if (line.IsHeader)
                return line.Corner;
            else if (line.IsFooter)
                return ScrollBar.Maximum - lines[lines.FirstFooterVisibleIndex].Corner + line.Corner;

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
                        double p3 = VisibleNodes.TotalHeight - this.FooterExtent;
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

        /// <summary>
        /// This method is called in response to a MouseWheel event.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public override void MouseWheel(int delta)
        {
            ScrollBar.Value -= delta;
        }
    }

}
