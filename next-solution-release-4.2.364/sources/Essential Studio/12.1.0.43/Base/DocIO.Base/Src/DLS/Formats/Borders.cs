#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using Syncfusion.DocIO.DLS.XML;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents border formatting.
    /// </summary>
    public class Border : FormatBase
    {
        #region Class enums
        /// <summary>
        /// Specifies the border position.
        /// </summary>
        internal enum BorderPositions
        {
            /// <summary>
            /// Left border
            /// </summary>
            Left,
            /// <summary>
            /// Top border
            /// </summary>
            Top,
            /// <summary>
            /// Right border
            /// </summary>
            Right,
            /// <summary>
            /// Bottom border
            /// </summary>
            Bottom,
            /// <summary>
            /// Vertical border
            /// </summary>
            Vertical = 5,
            /// <summary>
            /// Horizontal border
            /// </summary>    
            Horizontal = 6,
            /// <summary>
            /// Diagonal Down border
            /// </summary>
            DiagonalDown = 7,
            /// <summary>
            /// Diagonal Up border
            /// </summary>
            DiagonalUp = 8
        }
        #endregion

        #region Class constants
        /// <summary>
        /// Variable used to hold the ColorKey.
        /// </summary>
        public const int ColorKey = 1;
        /// <summary>
        /// Variable used to hold the BorderTypeKey.
        /// </summary>
        internal const int BorderTypeKey = 2;
        /// <summary>
        /// Variable used to hold the LineWidthKey.
        /// </summary>
        internal const int LineWidthKey = 3;
        /// <summary>
        /// Variable used to hold the SpaceKey.
        /// </summary>
        protected const int SpaceKey = 4;
        /// <summary>
        /// Variable used to hold the ShadowKey.
        /// </summary>
        protected const int ShadowKey = 5;
        /// <summary>
        /// Variable used to hold the HasNoneStyleKey.
        /// </summary>
        protected const int HasNoneStyleKey = 6;
        #endregion

        #region Class members
        /// <summary>
        /// Indicate border position
        /// </summary>
        private BorderPositions m_borderPosition;
        /// <summary>
        /// Document is presently read
        /// </summary>
        private bool m_read;
        #endregion

        #region Class properties
        /// <summary>
        /// Document is presently read
        /// </summary>
        internal bool IsRead
        {
            get
            {
                return m_read;
            }
            set
            {
                m_read = value;
            }
        }
        /// <summary>
        /// Get/set border position value
        /// </summary>
        internal BorderPositions BorderPosition
        {
            get
            {
                return m_borderPosition;
            }
            set
            {
                m_borderPosition = value;
            }
        }
        /// <summary>
        /// Gets/sets color of the border.
        /// </summary>
        public Color Color
        {
            get
            {
                return (Color)this[ColorKey];
            }
            set
            {
                this[ColorKey] = value;
                //UpdateTableCells();
            }
        }
        /// <summary>
        /// Gets/sets width of the border.
        /// </summary>
        public float LineWidth
        {
            get
            {
                return (float)this[LineWidthKey];
            }
            set
            {
                this[LineWidthKey] = value;

                if (value == 0f)
                {
                    if ((BorderType != BorderStyle.None) && (BorderType != BorderStyle.Cleared))
                        BorderType = BorderStyle.None;
                }
                else
                {
                    if (BorderType == BorderStyle.None)
                        BorderType = BorderStyle.Single;
                }
                if (!IsRead)
                    UpdateTableCells();
            }
        }
        /// <summary>
        /// Gets/sets  style of the border.
        /// </summary>
        public BorderStyle BorderType
        {
            get
            {
                return (BorderStyle)this[BorderTypeKey];
            }
            set
            {
                SetBorderStyle(value);
            }
        }
        /// <summary>
        /// Gets / Sets width of space to maintain between border and text within border.
        /// </summary>
        public float Space
        {
            get
            {
                return (float)this[SpaceKey];
            }
            set
            {
                this[SpaceKey] = value;
            }
        }
        /// <summary>
        /// Setting to define if border should be drawn with shadow.
        /// </summary>
        public bool Shadow
        {
            get
            {
                return (bool)this[ShadowKey];
            }
            set
            {
                this[ShadowKey] = value;
                if (!IsRead)
                    UpdateTableCells();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool HasNoneStyle
        {
            get
            {
                return (bool)this[HasNoneStyleKey];
            }
            set
            {
                this[HasNoneStyleKey] = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether border is defined.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if border is defined; otherwise, <c>false</c>.
        /// </value>
        internal bool IsBorderDefined
        {
            get
            {
                return (BorderType != BorderStyle.None
                    || (HasNoneStyle && HasKey(HasNoneStyleKey)));
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializing constructor.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="baseKey">The base key.</param>
        public Border(FormatBase parent, int baseKey)
            : base(parent, baseKey)
        {
        }
        #endregion

        #region Class helper methods
        private Border OwnerBorder()
        {
            Borders borders = this.OwnerBase as Borders;
            if ( borders != null && borders.CurrentRow != null)
            {
                RowFormat rFormat = borders.CurrentRow.RowFormat;
                Borders rowBorders = rFormat.Borders;
                switch (BorderPosition)
                {
                    case BorderPositions.Left:
                        return rowBorders.Left;
                    case BorderPositions.Right:
                        return rowBorders.Right;
                    case BorderPositions.Top:
                        return rowBorders.Top;
                    case BorderPositions.Bottom:
                        return rowBorders.Bottom;
                }
            }
            return null;
        }
        /// <summary>
        /// Clones the Border formatting.
        /// </summary>
        /// <param name="sourceBorder"></param>
        internal void CopyBorderFormatting(Border sourceBorder)
        {
            this[BorderTypeKey] = sourceBorder.BorderType;
            this[LineWidthKey] = sourceBorder.LineWidth;
            this[ColorKey] = sourceBorder.Color;
            this[ShadowKey] = sourceBorder.Shadow;
            if (sourceBorder.BorderType != BorderStyle.Cleared)
                this[HasNoneStyleKey] = sourceBorder.HasNoneStyle;
        }
        /// <summary>
        /// Update table calls border
        /// </summary>
        private void UpdateTableCells()
        {
            if (this.OwnerBase != null && this.OwnerBase is Borders)
            {
                Borders borders = this.OwnerBase as Borders;
                if (borders.CurrentRow == null)
                    return;

                WTable currTable = borders.CurrentRow.OwnerTable;
                if (currTable == null)
                    return;

                int currRowIndex = borders.CurrentRow.GetRowIndex();

                if (borders.CurrentCell == null)
                {
                    UpdateAruondRow(currTable, currRowIndex);
                }
                else
                {
                    int currCellIndex = borders.CurrentCell.CellFormat.CurCellIndex;
                    if (this.HasNoneStyle || this.BorderType != BorderStyle.None)
                        UpdateAroundCell(currTable, borders, currRowIndex, currCellIndex);
                }
            }
        }

        internal float GetLineWidth()
        {
            if (BorderType != BorderStyle.ThinThickThinSmallGap)
                return LineWidth;
            else
                return LineWidth + 9.0f;// the total line width= outerline width + outerline gap+ center line width(line width)+innerline gap|innerline width
        }
        /// <summary>
        /// Update table cell borders
        /// </summary>
        /// <param name="table">Table</param>
        /// <param name="row">Row index</param>
        /// <param name="cell">Cell index</param>
        private void UpdateAroundCell(WTable table, Borders borders, int rowIndex, int cellIndex)
        {
            if (rowIndex == -1)
                return;

            switch (BorderPosition)
            {
                case BorderPositions.Left:
                    if (cellIndex > 0 && cellIndex - 1 < table.Rows[rowIndex].Cells.Count )
                    {
                        WTableCell cell = table[rowIndex, cellIndex - 1];
                        if (cell.CellFormat.Borders.Right.HasNoneStyle)
                            cell.CellFormat.Borders.Right.CopyBorderFormatting(this);
                    }
                    break;
                case BorderPositions.Top:
                    if (rowIndex > 0 && table.Rows[rowIndex  - 1].Cells.Count > cellIndex)
                    {
                        WTableCell cell = table[rowIndex - 1, cellIndex];
                        if (cell.CellFormat.Borders.Bottom.HasNoneStyle)
                            cell.CellFormat.Borders.Bottom.CopyBorderFormatting(this);
                    }
                    break;
                case BorderPositions.Right:
                    if ( cellIndex + 1 < table.Rows[rowIndex].Cells.Count)
                    {
                        WTableCell cell = table[rowIndex, cellIndex + 1];
                        if (cell.CellFormat.Borders.Left.HasNoneStyle)
                            cell.CellFormat.Borders.Left.CopyBorderFormatting(this);
                    }
                    break;
                case BorderPositions.Bottom:
                    if (rowIndex + 1 < table.Rows.Count && table.Rows[rowIndex + 1].Cells.Count > cellIndex)
                    {
                        WTableCell cell = table[rowIndex + 1, cellIndex];
                        if (cell.CellFormat.Borders.Top.HasNoneStyle)
                            cell.CellFormat.Borders.Top.CopyBorderFormatting(this);
                    }
                    break;
            }
        }
        /// <summary>
        /// Update table row borders
        /// </summary>
        /// <param name="table">Table</param>
        /// <param name="row">Row index</param>
        private void UpdateAruondRow(WTable table, int rowIndex)
        {
            switch (BorderPosition)
            {
                case BorderPositions.Top:
                    if (rowIndex != 0)
                    {
                        // Need implementation
                    }
                    break;
                case BorderPositions.Bottom:
                    if (rowIndex != table.Rows.Count - 1)
                    {
                        // Need implementation
                    }
                    break;
            }
        }
        /// <summary>
        /// Set the style of the border
        /// </summary>
        /// <param name="value"></param>
        private void SetBorderStyle(BorderStyle value)
        {
            if (value == BorderStyle.None || value == BorderStyle.Cleared)
            {
                if (LineWidth != 0f)
                {
                    LineWidth = 0f;
                }

                if (value == BorderStyle.None)
                    HasNoneStyle = true;
                else
                    HasNoneStyle = false;
            }
            else if ((BorderStyle)this[BorderTypeKey] == BorderStyle.None && value != BorderStyle.Cleared)
            {
                if (LineWidth == 0f)
                {
                    LineWidth = 0.5f;
                }

                if (Color == Color.Empty)
                {
                    Color = Color.Black;
                }
                HasNoneStyle = false;
            }
            if (((BorderStyle)this[BorderTypeKey] == BorderStyle.None || (BorderStyle)this[BorderTypeKey] == BorderStyle.Cleared) && (value != BorderStyle.Cleared && value != BorderStyle.None))
            {
                if (LineWidth == 0f)
                {
                    LineWidth = 0.5f;
                }

                if (Color == Color.Empty || Color == Color.White || Color.Name.ToLower() == "ffffff")
                {
                    Color = Color.Black;
                }
            }
            this[BorderTypeKey] = value;
            //if (!IsRead)
            //    UpdateTableCells();
        }
        /// <summary>
        /// Sets the default properties.
        /// </summary>
        internal void SetDefaultProperties()
        {
            PropertiesHash.Add(GetFullKey(LineWidthKey), 0f);
            PropertiesHash.Add(GetFullKey(BorderTypeKey), BorderStyle.None);
            PropertiesHash.Add(GetFullKey(ShadowKey), false);
            PropertiesHash.Add(GetFullKey(ColorKey), Color.Empty);
            PropertiesHash.Add(GetFullKey(SpaceKey), 0f);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Initialize Border style.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="lineWidth">Width of the line.</param>
        /// <param name="borderType">Type of the border.</param>
        /// <param name="shadow">if it specifies shadow, set to <c>true</c>.</param>
        public void InitFormatting(Color color, float lineWidth, BorderStyle borderType, bool shadow)
        {
            this[ColorKey] = color;
            this[LineWidthKey] = lineWidth;
            this[BorderTypeKey] = borderType;
            this[ShadowKey] = shadow;

            if (borderType == BorderStyle.None)
                this[HasNoneStyleKey] = true;
            else
                this[HasNoneStyleKey] = false;

        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override object GetDefValue(int key)
        {
            switch (key)
            {
                case ColorKey:
                    return Color.Empty;
                case BorderTypeKey:
                    return BorderStyle.None;
                case LineWidthKey:
                    return 0f;
                case ShadowKey:
                    return false;
                case SpaceKey:
                    return (float)0;
                case HasNoneStyleKey:
                    return false;
            }

            throw new ArgumentException("key has invalid value");
        }
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (HasKey(ColorKey) && !Color.IsEmpty)
            {
                writer.WriteValue(XDLSConstants.BorderColorAttr, Color);
            }
            if (HasKey(LineWidthKey))
            {
                writer.WriteValue(XDLSConstants.BorderWidthAttr, LineWidth);
            }
            if (HasKey(BorderTypeKey))
            {
                writer.WriteValue(XDLSConstants.BorderTypeAttr, BorderType);
            }
            if (HasKey(SpaceKey))
            {
                writer.WriteValue(XDLSConstants.BorderSpaceAttr, Space);
            }
            if (HasKey(ShadowKey))
            {
                writer.WriteValue(XDLSConstants.BorderShadowAttr, Shadow);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.BorderColorAttr))
            {
                Color = reader.ReadColor(XDLSConstants.BorderColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.BorderWidthAttr))
            {
                LineWidth = reader.ReadFloat(XDLSConstants.BorderWidthAttr);
            }
            if (reader.HasAttribute(XDLSConstants.BorderTypeAttr))
            {
                BorderType = (BorderStyle)reader.ReadEnum(XDLSConstants.BorderTypeAttr, typeof(BorderStyle));
            }
            if (reader.HasAttribute(XDLSConstants.BorderSpaceAttr))
            {
                Space = reader.ReadFloat(XDLSConstants.BorderSpaceAttr);
            }
            if (reader.HasAttribute(XDLSConstants.BorderShadowAttr))
            {
                Shadow = reader.ReadBoolean(XDLSConstants.BorderShadowAttr);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            if (IsDefault)
            {
                XDLSHolder.SkipMe = true;
            }
        }
//#endif

        /// <summary>
        /// Action on format change.
        /// </summary>
        /// <param name="format"></param>
        protected override void OnChange(FormatBase format, int propertyKey)
        {
            base.OnChange(format, propertyKey);
        }
        /// <summary>
        /// Apply base style
        /// </summary>
        /// <param name="baseFormat"></param>
        internal override void ApplyBase(FormatBase baseFormat)
        {
            base.ApplyBase(baseFormat);
        }
        #endregion

        #region Implementation / Import contents
        /// <summary>
        /// Updates the source formatting.
        /// </summary>
        /// <param name="border">The border.</param>
        internal void UpdateSourceFormatting(Border border)
        {
            if (border.BorderPosition != BorderPosition)
                BorderPosition = border.BorderPosition;
            if (border.BorderType != BorderType)
                BorderType = border.BorderType;
            if (border.Color != Color)
                Color = border.Color;
            if (border.HasNoneStyle != HasNoneStyle)
                HasNoneStyle = border.HasNoneStyle;
            if (border.IsRead != IsRead)
                IsRead = border.IsRead;
            if (border.LineWidth != LineWidth)
                LineWidth = border.LineWidth;
            if (border.Shadow != Shadow)
                Shadow = border.Shadow;
            if (border.Space != Space)
                Space = border.Space;
        }
        #endregion
    }

    /// <summary>
    /// Represents a collection of four borders. <see cref="Syncfusion.DocIO.DLS.Border"/>
    /// </summary>
    public class Borders : FormatBase
    {
        #region Class constants
        /// <summary>
        /// Constant value for left key.
        /// </summary>
        public const int LeftKey = 1;
        /// <summary>
        /// Constant value for top key.
        /// </summary>
        public const int TopKey = 2;
        /// <summary>
        /// Constant value for bottom key.
        /// </summary>
        public const int BottomKey = 3;
        /// <summary>
        /// Constant value for right key.
        /// </summary>
        public const int RightKey = 4;
        /// <summary>
        /// constant value for vertical key.
        /// </summary>
        public const int VerticalKey = 5;
        /// <summary>
        /// constant value for horizontal key.
        /// </summary>
        public const int HorizontalKey = 6;
        /// <summary>
        /// constant value for diagonal down key.
        /// </summary>
        public const int DiagonalDownKey = 7;
        /// <summary>
        /// constant value for diagonal up key.
        /// </summary>
        public const int DiagonalUpKey = 8;
        #endregion

        #region Class members
        /// <summary>
        /// Current table cell
        /// </summary>
        private WTableCell m_currTableCell;
        /// <summary>
        /// Current table row
        /// </summary>
        private WTableRow m_currTableRow;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets whether the border exists
        /// </summary>
        public bool NoBorder
        {
            get
            {
                return (Left.BorderType == BorderStyle.None
                         && Right.BorderType == BorderStyle.None
                         && Top.BorderType == BorderStyle.None
                         && Bottom.BorderType == BorderStyle.None);
            }
        }
        /// <summary>
        /// Gets left border.
        /// </summary>
        public Border Left
        {
            get
            {
                return this[LeftKey] as Border;
            }
        }
        /// <summary>
        /// Gets top border.
        /// </summary>
        public Border Top
        {
            get
            {
                return this[TopKey] as Border;
            }
        }
        /// <summary>
        /// Gets right border.
        /// </summary>
        public Border Right
        {
            get
            {
                return this[RightKey] as Border;
            }
        }
        /// <summary>
        /// Gets bottom border.
        /// </summary>
        public Border Bottom
        {
            get
            {
                return this[BottomKey] as Border;
            }
        }
        /// <summary>
        /// Gets vertical border.
        /// </summary>
        public Border Vertical
        {
            get
            {
                return this[VerticalKey] as Border;
            }
        }
        /// <summary>
        /// Gets horizontal border.
        /// </summary>
        public Border Horizontal
        {
            get
            {
                return this[HorizontalKey] as Border;
            }
        }
        /// <summary>
        /// Gets diagonal border from top left corner to bottom right corner.
        /// </summary>
        internal Border DiagonalDown
        {
            get
            {
                return this[DiagonalDownKey] as Border;
            }
        }
        /// <summary>
        /// Gets diagonal border from bottom left corner to top right corner.
        /// </summary>
        internal Border DiagonalUp
        {
            get
            {
                return this[DiagonalUpKey] as Border;
            }
        }
        /// <summary>
        /// Sets color of the borders.
        /// </summary>
        public Color Color
        {
            set
            {
                Left.Color = Right.Color = Top.Color = Bottom.Color = value;
            }
        }
        /// <summary>
        /// Sets width of the borders.
        /// </summary>
        public float LineWidth
        {
            set
            {
                Left.LineWidth = Right.LineWidth = Top.LineWidth = Bottom.LineWidth = value;
            }
        }
        /// <summary>
        /// Sets style of the borders.
        /// </summary>
        public BorderStyle BorderType
        {
            set
            {
                Left.BorderType = Right.BorderType = Top.BorderType = Bottom.BorderType = value;
                Vertical.BorderType = Horizontal.BorderType = value;
            }
        }
        /// <summary>
        /// Sets width of space to maintain between borders and text within borders.
        /// </summary>
        public float Space
        {
            set
            {
                SetSpacing(value);
            }
        }
        /// <summary>
        /// Sets whether borders are drawn with shadow.
        /// </summary>
        public bool Shadow
        {
            set
            {
                Left.Shadow = Right.Shadow = Top.Shadow = Bottom.Shadow = value;
            }
        }
        /// <summary>
        /// Gets owner borders cell
        /// </summary>
        internal WTableCell CurrentCell
        {
            get
            {
                if (m_currTableCell == null)
                {
                    if (this.OwnerBase != null && this.OwnerBase is CellFormat)
                    {
                        CellFormat cellFormat = this.OwnerBase as CellFormat;
                        if (cellFormat.OwnerBase != null)
                        {
                            m_currTableCell = cellFormat.OwnerBase as WTableCell;
                        }
                    }
                }
                return m_currTableCell;
            }
        }
        /// <summary>
        /// Gets owner borders row
        /// </summary>
        internal WTableRow CurrentRow
        {
            get
            {
                if (m_currTableRow == null)
                {
                    if (CurrentCell != null)
                    {
                        m_currTableRow = CurrentCell.OwnerRow;
                    }
                    else
                    {
                        if (this.OwnerBase != null && this.OwnerBase is WTableRow)
                        {
                            // Need implementation
                            return null;
                        }
                    }
                }
                return m_currTableRow;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializing constructor.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="baseKey">The base key.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal Borders(FormatBase parent, int baseKey)
            : base(parent, baseKey)
        {
            InitBorders();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Borders"/> class.
        /// </summary>
        public Borders()
            : base()
        {
            InitBorders();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Borders"/> class.
        /// </summary>
        /// <param name="borders">The borders.</param>
        internal Borders(Borders borders)
            : base()
        {
            this.ImportContainer(borders);
            InitBorders();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal override void EnsureComposites()
        {
            EnsureComposites(LeftKey, RightKey, TopKey, BottomKey, VerticalKey, HorizontalKey, DiagonalDownKey, DiagonalUpKey);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override object GetDefValue(int key)
        {
            throw new ArgumentException("key has invalid value");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override FormatBase GetDefComposite(int key)
        {
            switch (key)
            {
                case LeftKey:
                    return GetDefComposite(LeftKey, new Border(this, LeftKey));
                case TopKey:
                    return GetDefComposite(TopKey, new Border(this, TopKey));
                case RightKey:
                    return GetDefComposite(RightKey, new Border(this, RightKey));
                case BottomKey:
                    return GetDefComposite(BottomKey, new Border(this, BottomKey));
                case VerticalKey:
                    return GetDefComposite(VerticalKey, new Border(this, VerticalKey));
                case HorizontalKey:
                    return GetDefComposite(HorizontalKey, new Border(this, HorizontalKey));
                case DiagonalDownKey:
                    return GetDefComposite(DiagonalDownKey, new Border(this, DiagonalDownKey));
                case DiagonalUpKey:
                    return GetDefComposite(DiagonalUpKey, new Border(this, DiagonalUpKey));
            }

            return null;
        }
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            if (IsDefault)
            {
                XDLSHolder.SkipMe = true;
            }
            XDLSHolder.AddElement(XDLSConstants.BorderBottomTag, Bottom);
            XDLSHolder.AddElement(XDLSConstants.BorderTopTag, Top);
            XDLSHolder.AddElement(XDLSConstants.BorderLeftTag, Left);
            XDLSHolder.AddElement(XDLSConstants.BorderRightTag, Right);
            XDLSHolder.AddElement(XDLSConstants.BorderHorizontalTag, Horizontal);
            XDLSHolder.AddElement(XDLSConstants.BorderVerticalTag, Vertical);
        }
//#endif
        /// <summary>
        /// Clones self.
        /// </summary>
        /// <returns></returns>
        public Borders Clone()
        {
            return (Borders)CloneImpl();
        }
        /// <summary>
        /// Clone method implementation.
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            return new Borders(this);
        }
        /// <summary>
        /// Action on format change.
        /// </summary>
        /// <param name="format"></param>
        protected override void OnChange(FormatBase format, int propertyKey)
        {
            base.OnChange(format, propertyKey);
        }
        /// <summary>
        /// Apply base style
        /// </summary>
        /// <param name="baseFormat"></param>
        internal override void ApplyBase(FormatBase baseFormat)
        {
            base.ApplyBase(baseFormat);

            Left.ApplyBase((baseFormat as Borders).Left);
            Right.ApplyBase((baseFormat as Borders).Right);
            Top.ApplyBase((baseFormat as Borders).Top);
            Bottom.ApplyBase((baseFormat as Borders).Bottom);
            Horizontal.ApplyBase((baseFormat as Borders).Horizontal);
            Vertical.ApplyBase((baseFormat as Borders).Vertical);
            DiagonalDown.ApplyBase((baseFormat as Borders).DiagonalDown);
            DiagonalUp.ApplyBase((baseFormat as Borders).DiagonalUp);
        }
        /// <summary>
        /// Sets the default properties.
        /// </summary>
        internal void SetDefaultProperties()
        {
            Top.SetDefaultProperties();
            Left.SetDefaultProperties();
            Bottom.SetDefaultProperties();
            Right.SetDefaultProperties();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets spacing for all 
        /// </summary>
        /// <param name="value"></param>
        private void SetSpacing(float value)
        {
            if (ParentFormat is RowFormat || ParentFormat is CellFormat)
            {
                if (ParentFormat is RowFormat)
                {
                    (ParentFormat as RowFormat).Paddings.All = value;
                }
                else
                {
                    (ParentFormat as CellFormat).Paddings.All = value;
                }
            }
            else if (ParentFormat is WParagraphFormat)
            {
                Left.Space = value;
                Right.Space = value;
                Top.Space = value;
                Bottom.Space = value;
            }
        }
        #endregion

        #region Implementation / helper methods
        /// <summary>
        /// Init borders owner
        /// </summary>
        private void InitBorders()
        {
            Left.SetOwner(this);
            Left.BorderPosition = Border.BorderPositions.Left;
            Top.SetOwner(this);
            Top.BorderPosition = Border.BorderPositions.Top;
            Right.SetOwner(this);
            Right.BorderPosition = Border.BorderPositions.Right;
            Bottom.SetOwner(this);
            Bottom.BorderPosition = Border.BorderPositions.Bottom;
            Vertical.SetOwner(this);
            Vertical.BorderPosition = Border.BorderPositions.Vertical;
            Horizontal.SetOwner(this);
            Horizontal.BorderPosition = Border.BorderPositions.Horizontal;
            DiagonalDown.SetOwner(this);
            DiagonalDown.BorderPosition = Border.BorderPositions.DiagonalDown;
            DiagonalUp.SetOwner(this);
            DiagonalUp.BorderPosition = Border.BorderPositions.DiagonalUp;
        }
        #endregion

        #region Implementation / Import contents
        /// <summary>
        /// Updates the source formatting.
        /// </summary>
        /// <param name="borders">The borders.</param>
        internal void UpdateSourceFormatting(Borders borders)
        {
            Left.UpdateSourceFormatting(borders.Left);
            Right.UpdateSourceFormatting(borders.Right);
            Top.UpdateSourceFormatting(borders.Top);
            Bottom.UpdateSourceFormatting(borders.Bottom);
            Horizontal.UpdateSourceFormatting(borders.Horizontal);
            Vertical.UpdateSourceFormatting(borders.Vertical);
            DiagonalDown.UpdateSourceFormatting(borders.DiagonalDown);
            DiagonalUp.UpdateSourceFormatting(borders.DiagonalUp);
        }
        #endregion
    }
}
