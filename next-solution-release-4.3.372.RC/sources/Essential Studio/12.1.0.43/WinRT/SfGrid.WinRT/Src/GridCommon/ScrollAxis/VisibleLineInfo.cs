#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.UI.Xaml.ScrollAxis
{
    /// <summary>
    /// Contains information about a visible line (can also be a row or column).
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public sealed class VisibleLineInfo : IComparable<VisibleLineInfo>
    {
        int visibleIndex;
        int lineIndex;
        double size;
        double clippedOrigin;
        double scrollOffset;
        bool isHeader;
        bool isFooter;
        internal double clippedCornerExtent;
        internal bool isLastLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleLineInfo"/> class.
        /// </summary>
        /// <param name="visibleIndex">Visible index of the line.</param>
        /// <param name="lineIndex">Absolute index of the line.</param>
        /// <param name="size">The size.</param>
        /// <param name="clippedOrigin">The clipped origin.</param>
        /// <param name="scrollOffset">The scroll offset.</param>
        /// <param name="isHeader">if set to <c>true</c> line is a header.</param>
        /// <param name="isFooter">if set to <c>true</c> line is a footer.</param>
        public VisibleLineInfo(int visibleIndex, int lineIndex, double size, double clippedOrigin, double scrollOffset, bool isHeader, bool isFooter)
        {
            this.visibleIndex = visibleIndex;
            this.lineIndex = lineIndex;
            this.size = size;
            this.clippedOrigin = clippedOrigin;
            this.scrollOffset = scrollOffset;
            this.isHeader = isHeader;
            this.isFooter = isFooter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleLineInfo"/> class. Used for BinarySearch.
        /// </summary>
        /// <param name="clippedOrigin">The clipped origin.</param>
        internal VisibleLineInfo(double clippedOrigin)
        {
            this.clippedOrigin = clippedOrigin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleLineInfo"/> class. Used for BinarySearch.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        internal VisibleLineInfo(int lineIndex)
        {
            this.lineIndex = lineIndex;
        }


        /// <summary>
        /// Gets the visible index of the line.
        /// </summary>
        /// <value>The visible index of the line.</value>
        public int VisibleIndex
        {
            get
            {
                return visibleIndex;
            }
        }

        /// <summary>
        /// Determines if the line is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return visibleIndex != int.MaxValue;
            }
        }

        /// <summary>
        /// Gets the index of the line.
        /// </summary>
        /// <value>The index of the line.</value>
        public int LineIndex
        {
            get
            {
                return lineIndex;
            }
            internal set
            {
                lineIndex = value;
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public double Size
        {
            get
            {
                return size;
            }
        }

        /// <summary>
        /// Gets the size of the clipped area.
        /// </summary>
        /// <value>The size of the clipped area.</value>
        public double ClippedSize
        {
            get
            {
                return Math.Max(0.0, size - scrollOffset - clippedCornerExtent);
            }
        }

        /// <summary>
        /// Gets the corner.
        /// </summary>
        /// <value>The corner.</value>
        public double Corner
        {
            get
            {
                return Origin + size;
            }
        }

        /// <summary>
        /// Gets the clipped corner.
        /// </summary>
        /// <value>The clipped corner.</value>
        public double ClippedCorner
        {
            get
            {
                return Origin + size - clippedCornerExtent;
            }
        }

        /// <summary>
        /// Gets the clipped origin.
        /// </summary>
        /// <value>The clipped origin.</value>
        public double ClippedOrigin
        {
            get
            {
                return clippedOrigin;
            }
        }

        /// <summary>
        /// Gets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public double Origin
        {
            get
            {
                return clippedOrigin - scrollOffset;
            }
        }

        /// <summary>
        /// Gets the scroll offset.
        /// </summary>
        /// <value>The scroll offset.</value>
        public double ScrollOffset
        {
            get
            {
                return scrollOffset;
            }
        }

        /// <summary>
        /// Gets the clipped corner extent.
        /// </summary>
        /// <value>The clipped corner extent.</value>
        public double ClippedCornerExtent
        {
            get
            {
                return clippedCornerExtent;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is clipped.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is clipped; otherwise, <c>false</c>.
        /// </value>
        public bool IsClipped
        {
            get
            {
                return scrollOffset + clippedCornerExtent > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance corner is clipped.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance corner is clipped; otherwise, <c>false</c>.
        /// </value>
        public bool IsClippedCorner
        {
            get
            {
                return clippedCornerExtent > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance origin is clipped.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance origin is clipped; otherwise, <c>false</c>.
        /// </value>
        public bool IsClippedOrigin
        {
            get
            {
                return scrollOffset > 0;
            }
        }

        /// <summary>
        /// Determines whether this instance is clipped taking into consideration whether it is the the first or last visible line 
        /// and no clipping is needed for these cases.
        /// </summary>
        /// <param name="isFirstRow"></param>
        /// <param name="isLastRow"></param>
        /// <returns></returns>
        public bool IsClippedBody
        {
            get
            {
                return VisibleIndex > 0 && IsClippedOrigin
                    || !isLastLine && IsClippedCorner;
            }
        }

        internal bool IsClippedBodyAny(bool hasOriginMargin, bool hasCornerMargin)
        {
            return (hasOriginMargin || VisibleIndex > 0) && IsClippedOrigin
                   || (hasCornerMargin || !isLastLine) && IsClippedCorner;
        }

        internal bool IsClippedBodyCorner(bool hasCornerMargin)
        {
            return (hasCornerMargin || !isLastLine) && IsClippedCorner;
        }

        internal bool IsClippedBodyOrigin(bool hasOriginMargin)
        {
            return (hasOriginMargin || VisibleIndex > 0) && IsClippedOrigin;
        }

        /// <summary>
        /// Gets the axis region this line belongs to.
        /// </summary>
        /// <value>The axis region.</value>
        public ScrollAxisRegion Region
        {
            get
            {
                if (IsHeader)
                    return ScrollAxisRegion.Header;
                else if (IsFooter)
                    return ScrollAxisRegion.Footer;
                
                return ScrollAxisRegion.Body;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a header.
        /// </summary>
        /// <value><c>true</c> if this instance is a header; otherwise, <c>false</c>.</value>
        public bool IsHeader
        {
            get
            {
                return isHeader;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a footer.
        /// </summary>
        /// <value><c>true</c> if this instance is a footer; otherwise, <c>false</c>.</value>
        public bool IsFooter
        {
            get
            {
                return isFooter;
            }
        }

             /// <summary>
        /// Returns the type name with state of this instance.
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("VisibleLineInfo { ");
            sb.Append("visibleIndex = " + visibleIndex.ToString());
            sb.Append(", lineIndex = " + lineIndex.ToString());
            sb.Append(", size = " + size.ToString());
            sb.Append(", origin = " + Origin.ToString());
            sb.Append(", clippedOrigin = " + clippedOrigin.ToString());
            sb.Append(", scrollOffset = " + scrollOffset.ToString());
            sb.Append(", region = " + Region.ToString());
            sb.Append("} ");
            return sb.ToString();
        }

        #region IComparable<VisibleLineInfo> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(VisibleLineInfo other)
        {
            return Math.Sign(clippedOrigin - other.clippedOrigin);
        }

        #endregion
    }

    /// <summary>
    /// A strong-typed collection of <see cref="VisibleLineInfo"/> items.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class VisibleLinesCollection : List<VisibleLineInfo>
    {
        internal int firstBodyVisibleIndex;
        internal int firstFooterVisibleIndex;
        VisibleLineInfoLineIndexComparer lineIndexComparer = new VisibleLineInfoLineIndexComparer();
        Dictionary<int, VisibleLineInfo> lineIndexes = new Dictionary<int,VisibleLineInfo>();
        Dictionary<int, VisibleLineInfo> shadowedLineIndexes = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleLinesCollection"/> class.
        /// </summary>
        public VisibleLinesCollection()
        {
            shadowedLineIndexes = lineIndexes;
        }

        /// <summary>
        /// Gets the visible line indexes.
        /// </summary>
        /// <value>The visible line indexes.</value>
        public Dictionary<int, VisibleLineInfo> VisibleLineIndexes
        {
            get { return shadowedLineIndexes; }
        }

        /// <summary>
        /// Gets the index of the first visible line in the body region.
        /// </summary>
        /// <value>The index of the first visible line in the body region.</value>
        public int FirstBodyVisibleIndex
        {
            get
            {
                return this.firstBodyVisibleIndex;
            }
        }

        /// <summary>
        /// Gets the index of the first visible line in the footer region.
        /// </summary>
        /// <value>The index of the first visible line in the footer region.</value>
        public int FirstFooterVisibleIndex
        {
            get
            {
                return this.firstFooterVisibleIndex;
            }
        }

        /// <summary>
        /// Gets the index of the last visible line in the body region.
        /// </summary>
        /// <value>The index of the last visible line in the body region.</value>
        public int LastBodyVisibleIndex
        {
            get
            {
                return this.firstFooterVisibleIndex - 1;
            }
        }

        /// <summary>
        /// Gets the visible line at point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>visible line at point.</returns>
        public VisibleLineInfo GetVisibleLineAtPoint(double point)
        {
            int index = BinarySearch(new VisibleLineInfo(point));
            index = (index < 0) ? (~index) - 1 : index;
            if (index >= 0)
                return this[index];
            return null;
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            lineIndexes = new Dictionary<int, VisibleLineInfo>(); ;
        }

        /// <summary>
        /// Gets the the visible line at line index.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <returns>visible line at line index.</returns>
        public VisibleLineInfo GetVisibleLineAtLineIndex(int lineIndex)
        {
#if true
            if (lineIndexes.Count == 0)
            {
                foreach (VisibleLineInfo line in this)
                    lineIndexes.Add(line.LineIndex, line);
                shadowedLineIndexes = lineIndexes;
            }

            VisibleLineInfo lineInfo;
            lineIndexes.TryGetValue(lineIndex, out lineInfo);
            return lineInfo;
#else
            int index = BinarySearch(new VisibleLineInfo(lineIndex), lineIndexComparer);
            if (index < 0)
                return null;
            return this[index];
#endif
        }

        /// <summary>
        /// Gets the visible line for a line index. If the line specified
        /// line is hidden the next visible line is returned.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <returns>The first visible line for a line index that is not hidden.</returns>
        public VisibleLineInfo GetVisibleLineNearLineIndex(int lineIndex)
        {
            int index = BinarySearch(new VisibleLineInfo(lineIndex), lineIndexComparer);
            index = (index < 0) ? (~index) - 1 : index;
            if (index >= 0)
                return this[index];
            return null;
        }

        class VisibleLineInfoLineIndexComparer : IComparer<VisibleLineInfo>
        {
            #region IComparer<VisibleLineInfo> Members

            public int Compare(VisibleLineInfo x, VisibleLineInfo y)
            {
                return Math.Sign(x.LineIndex - y.LineIndex);
            }

            #endregion
        }

        public bool AnyVisibleLines(int lineIndex1, int lineIndex2)
        {
            if (lineIndex1 == lineIndex2)
                return GetVisibleLineAtLineIndex(lineIndex1) != null;

            for (int n = 0;n < this.Count;n++)
            {
                VisibleLineInfo line = this[n];
                if (line.LineIndex >= lineIndex1 && line.LineIndex <= lineIndex2)
                    return true;
            }
            return false;
        }

        internal bool RemoveLinesInternal(int lineIndex, int count)
        {
            bool visibleLinesAffected = false;
            for (int n = 0;n < this.Count;n++)
            {
                VisibleLineInfo line = this[n];
                if (line.LineIndex >= lineIndex)
                {
                    if (line.LineIndex < lineIndex + count)
                        visibleLinesAffected = true;
                    else
                        line.LineIndex -= count;
                }
            }
            return visibleLinesAffected;
        }

        internal bool InsertLinesInternal(int lineIndex, int count)
        {
            bool visibleLinesAffected = false;
            for (int n = 0;n < this.Count;n++)
            {
                VisibleLineInfo line = this[n];
                if (line.LineIndex >= lineIndex)
                {
                    if (line.LineIndex == lineIndex)
                        visibleLinesAffected = true;
                    line.LineIndex += count;
                }
            }
            return visibleLinesAffected;
        }
    }

 
}
