#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;TD&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Td)]
    public class TDElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Td;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class members
        /// <summary>
        /// Number of colspan of the element.
        /// </summary>
        private int m_colspan = 0;

        /// <summary>
        /// Number of rowspan of the element.
        /// </summary>
        private int m_rowspan = 0;

        /// <summary>
        /// Horizontal order of the cell in the row.
        /// </summary>
        private int m_hOrder;

        /// <summary>
        /// Vertical order of the cell in the column.
        /// </summary>
        private int m_vOrder;

        /// <summary>
        /// Parent table for the cell element.
        /// </summary>
        private TABLEElementImpl m_table;
        #endregion

        #region Class Properties
        /// <summary>
        /// Overridden. Returns an array of supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return DEF_SUPP_EVENTS;
            }
        }

        /// <summary>
        /// Gets the order of the cell in the row.
        /// </summary>
        [Browsable(false)]
        public int ColumnIndex
        {
            get
            {
                return m_hOrder;
            }
        }

        /// <summary>
        /// Gets the order of the cell in the column.
        /// </summary>
        [Browsable(false)]
        public int RowIndex
        {
            get
            {
                return m_vOrder;
            }
        }

        /// <summary>
        /// Gets number of colspan in the element.
        /// </summary>
        [Browsable(false)]
        public int Colspan
        {
            get
            {
                if (m_colspan == 0)
                {
                    SetDimensionAttributes();
                }

                return m_colspan;
            }
        }

        /// <summary>
        /// Gets the number of rowspan in the element.
        /// </summary>
        [Browsable(false)]
        public int Rowspan
        {
            get
            {
                if (m_rowspan == 0)
                {
                    SetDimensionAttributes();
                }

                return m_rowspan;
            }
        }

        /// <summary>
        /// Gets or sets the parent table for this cell element.
        /// </summary>
        public TABLEElementImpl Table
        {
            get
            {
                return m_table;
            }
            set
            {
                if (m_table != value)
                {
                    m_table = value;
                }
            }
        }

        /// <summary>
        /// Overridden. Gets or sets the format which is special for tag A (Hyperlink).
        /// </summary>
        protected internal override HTMLFormat OwnFormat
        {
            get
            {
                if (m_ownFormat == null)
                {
                    SetOwnFormat();
                }

                return m_ownFormat;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the TDElementImpl class 
        /// </summary>
        static TDElementImpl()
        {
            Type type = typeof(TDElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the TDElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public TDElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TDElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="tagName">Name of the tag.</param>
        protected TDElementImpl(IHTMLElement parent, string tagName)
            : base(parent, tagName)
        {
        }

        /// <summary>
        /// Overridden. Disposes all resources.
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            if (m_table != null)
            {
                m_table = null;
            }
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns an instance of the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>Event object.</returns>
        protected override IHTMLEvent CreateEventInternal(string name)
        {
            return new HashElementEvents(this, m_eventHash, name);
        }

        /// <summary>
        /// Overridden. Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size instance</returns>
        protected override Size CalculateSizeInternal()
        {
            if (!this.IsVisible) return this.Size;

            this.Type = ElementType.BlockSimple;
            this.Size = DefaultCalculateSizeInternal();

            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            if (this.IsVisible)
            {
                BaseElement parent = (BaseElement)this.Parent;
                this.CurrentPosition = parent.CurrentPosition;

                CalculateChildPositions(this.CurrentPosition, parent.Bounds);
            }
        }

        /// <summary>
        /// Overridden. Calculates the format to the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();
            this.InsideTable = true;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element.
        /// </summary>
        /// <param name="curPosition">Current position.</param>
        /// <param name="bounds">Bounds for the element.</param>
        /// <returns>Array of the blocks.</returns>
        protected override BlocksCollection CalculateChildPositions(Point curPosition, Rectangle bounds)
        {
            // If element is invisible, skip position calculating method.
            this.IsVisible = (this.Table == null) ? false : this.IsVisible;
            if (!this.IsVisible) return new BlocksCollection();

            BlocksCollection result = null;

            if (this.Table.FirstRender || this.RowIndex == 0)
            {
                result = base.CalculateChildPositions(curPosition, bounds);
            }
            else
            {
                this.QuietMode = true;

                int x = GetXFromLeftCells();
                if (x != -1)
                {
                    curPosition.X = x;
                    bounds.X = x;
                }

                BaseElement upperCell = GetUpperCell();
                if (upperCell != null)
                {
                    curPosition.Y = upperCell.Y + upperCell.Height;

                    bounds.Y = curPosition.Y;
                }

                this.QuietMode = false;
                result = base.CalculateChildPositions(curPosition, bounds);
            }

            return result;
        }

        /// <summary>
        /// Overridden. Sets the value indicating if trailing whitespace must be allowed after element.
        /// </summary>
        protected override void ProhibitSpaceAfter()
        {
            if (this.Parent != null)
            {
                (this.Parent as BaseElement).SpaceProhibited = true;
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Searches for colspan and rowspan attributes and assigns them to variables.
        /// </summary>
        protected internal void SetDimensionAttributes()
        {
            m_colspan = 1;
            m_rowspan = 1;

            IHTMLAttribute colspan = this.Attributes[AttributeName.Colspan];
            IHTMLAttribute rowspan = this.Attributes[AttributeName.Rowspan];

            if (colspan != null)
            {
                double result;
                if (Double.TryParse(colspan.Value, NumberStyles.Number, null, out result))
                {
                    if (result > 1)
                    {
                        m_colspan = (int)result;
                    }
                }
            }

            if (rowspan != null)
            {
                double result;
                if (Double.TryParse(rowspan.Value, NumberStyles.Number, null, out result))
                {
                    if (result > 1)
                    {
                        m_rowspan = (int)result;
                    }
                }
            }
        }

        /// <summary>
        /// Inherits from table, parent element's border and background color.
        /// </summary>
        /// <param name="format">HTMLFormat instance</param>
        protected internal void GetStyleFromParents(HTMLFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            TABLEElementImpl table = GetParentTable() as TABLEElementImpl;

            // Attach border from table.
            if (table.Attributes.Contains(AttributeName.Border))
            {
                if (format.Left.Width == 0 && table.Format.Left.Width > 0 && this.ColumnIndex != 0)
                {
                    format.Left.Width = table.Format.Left.Width;
                    format.Left.Color = table.Format.Left.Color;
                    format.Merge |= MergeMask.BorderLeft;
                    format.Merge |= MergeMask.BorderWidth;
                    format.Merge |= MergeMask.BorderColor;
                }

                if (format.Top.Width == 0 && table.Format.Top.Width > 0 && this.RowIndex != 0)
                {
                    format.Top.Width = table.Format.Top.Width;
                    format.Top.Color = table.Format.Top.Color;
                    format.Merge |= MergeMask.BorderTop;
                    format.Merge |= MergeMask.BorderWidth;
                    format.Merge |= MergeMask.BorderColor;
                }

                int maxHOrder = this.ColumnIndex + this.Colspan;

                if (format.Right.Width == 0 && table.Format.Right.Width > 0 &&
                  maxHOrder != table.ColsCount)
                {
                    format.Right.Width = table.Format.Right.Width;
                    format.Right.Color = table.Format.Right.Color;
                    format.Merge |= MergeMask.BorderRight;
                    format.Merge |= MergeMask.BorderWidth;
                    format.Merge |= MergeMask.BorderColor;
                }

                int maxVOrder = this.RowIndex + this.Rowspan;

                if (format.Bottom.Width == 0 && table.Format.Bottom.Width > 0 &&
                  maxVOrder != table.RowsCount)
                {
                    format.Bottom.Width = table.Format.Bottom.Width;
                    format.Bottom.Color = table.Format.Bottom.Color;
                    format.Merge |= MergeMask.BorderBottom;
                    format.Merge |= MergeMask.BorderWidth;
                    format.Merge |= MergeMask.BorderColor;
                }
            }
        }

        /// <summary>
        /// Returns parent table for this cell element.
        /// </summary>
        /// <returns>Parent table for this cell element.</returns>
        protected internal IHTMLElement GetParentTable()
        {
            IHTMLElement parent = this.Parent;
            if (parent == null)
                throw new ParseException("TD element hasn't parent table tag.");

            while (true)
            {
                if (parent is TABLEElementImpl) return parent;
                if (parent == null) break;
                if (parent == this.Document.RenderRoot) break;

                parent = parent.Parent;
            }

            throw new ParseException("TD element hasn't parent table tag.");
        }

        /// <summary>
        /// Returns parent row for this cell element.
        /// </summary>
        /// <returns>Parent row for this cell element.</returns>
        protected internal IHTMLElement GetParentRow()
        {
            IHTMLElement parent = this.Parent;
            if (parent == null)
                throw new ParseException("TD element hasn't parent TR tag.");

            while (true)
            {
                if (parent is TRElementImpl) return parent;
                if (parent == null) break;
                if (parent == this.Document.RenderRoot) break;

                parent = parent.Parent;
            }

            throw new ParseException("TD element hasn't parent TR tag.");
        }

        /// <summary>
        /// Sets own format (inherits background color and border from table and tr parent elements).
        /// </summary>
        protected internal virtual void SetOwnFormat()
        {
            m_ownFormat = base.OwnFormat;
            m_ownFormat.VerticalAlign = StringAlignment.Center;
            m_ownFormat.Merge |= MergeMask.VAlignmnet;

            GetStyleFromParents(m_ownFormat);
        }

        /// <summary>
        /// Returns cell which is above current if it exists; Null otherwise.
        /// </summary>
        /// <returns>Cell which is above current if it exists; Null otherwise.</returns>
        protected internal BaseElement GetUpperCell()
        {
            if (this.RowIndex <= 0) return null;

            TDElementImpl upperCell = this.Table.Matrix[this.RowIndex - 1, this.ColumnIndex];

            if (upperCell != null && upperCell != this) return upperCell;

            return null;
        }

        /// <summary>
        /// Returns the X coordinate corresponding to cells which are at the left of the current cell.
        /// If there are no cells at the left, it returns -1.
        /// </summary>
        /// <returns>X coordinate corresponding to cells which are at the left from current cell.
        /// If there are no cells at the left; -1 otherwise.</returns>
        protected internal int GetXFromLeftCells()
        {
            if (this.RowIndex <= 0) return -1;

            TDElementImpl cell = this.Table.Matrix[this.RowIndex, 0];
            int result = -1;

            if (cell != null)
            {
                result = cell.X;
                int colIndex = 0;

                while (true)
                {
                    if (colIndex >= this.ColumnIndex) break;

                    cell = this.Table.Matrix[this.RowIndex, colIndex];

                    if (cell != null)
                    {
                        result += cell.Width;
                        colIndex += cell.Colspan;
                    }
                    else
                    {
                        colIndex++;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Sets value of cell's index.
        /// </summary>
        /// <param name="index">Index value.</param>
        /// <param name="bColumn">If true - sets ColumnIndex, otherwise - RowIndex.</param>
        internal void SetIndex(int index, bool bColumn)
        {
            if (bColumn)
            {
                m_hOrder = index;
            }
            else
            {
                m_vOrder = index;
            }
        }
        #endregion
    }
}
