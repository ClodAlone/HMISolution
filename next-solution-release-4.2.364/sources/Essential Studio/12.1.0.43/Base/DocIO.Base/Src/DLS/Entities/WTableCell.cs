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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.Rendering;
using Syncfusion.Layouting;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a table cell.
    /// </summary>
    public class WTableCell
      : WTextBody
      , ICompositeEntity
#if !SILVERLIGHT && !WP
      , IWidget
#endif
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private CellFormat m_cellFormat;
        private WCharacterFormat m_charFormat;
        internal TextureStyle m_textureStyle = TextureStyle.TextureNone;
        internal Color m_foreColor = Color.Empty;
        internal CellFormat m_trackCellFormat = null;
        private int m_colspan = 1;
        private StructureDocumentTagCell m_SDTCell;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the grid Span
        /// </summary>
        public short GridSpan
        {
            get
            {
                if (OwnerRow != null
                    && OwnerRow.OwnerTable != null)
                    return OwnerRow.RowFormat.GetGridCount(GetIndexInOwnerCollection());
                return 1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal StructureDocumentTagCell SDTCell
        {
            get
            {
                return m_SDTCell;
            }
            set
            {
                m_SDTCell = value;
            }
        }
        /// <summary>
        /// Gets and sets the Column span
        /// </summary>
        internal int Colspan
        {
            get
            {
                return m_colspan;
            }
            set
            {
                m_colspan = value;
            }
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.TableCell;
            }
        }
        /// <summary>
        /// Gets owner row of the cell.
        /// </summary>
        public WTableRow OwnerRow
        {
            get
            {
                return Owner as WTableRow;
            }
        }
        /// <summary>
        /// Gets cell format.
        /// </summary>
        public CellFormat CellFormat
        {
            get
            {
                return m_cellFormat;
            }
        }
        /// <summary>
        /// Gets / sets cell width.
        /// </summary>
        public float Width
        {
            get
            {
                return CellFormat.CellWidth;
            }
            set
            {
                float prevWidth = CellFormat.CellWidth;
                CellFormat.CellWidth = value;
                if (!Document.IsOpening
                    && OwnerRow != null
                    && OwnerRow.OwnerTable != null)
                {
                    OwnerRow.OwnerTable.m_bIsTableGridUpdated = false;
                    UpdateTablePreferredWidth(prevWidth, value);
                }
            }
        }
        /// <summary>
        /// Gets or sets the color of the fore.
        /// </summary>
        /// <value>The color of the fore.</value>
        internal Color ForeColor
        {
            get
            {
                return CellFormat.ForeColor;
            }
            set
            {
                CellFormat.ForeColor = value;
            }
        }
        /// <summary>
        /// Gets or sets the texture style.
        /// </summary>
        /// <value>The texture style.</value>
        internal TextureStyle TextureStyle
        {
            get
            {
                return CellFormat.TextureStyle;
            }
            set
            {
                CellFormat.TextureStyle = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal WCharacterFormat CharacterFormat
        {
            get
            {
                return m_charFormat;
            }
        }
        /// <summary>
        /// Gets if size of cell is fixed.
        /// </summary>
        internal bool IsFixedWidth
        {
            get
            {
                return (Width > -1);
            }
        }
        /// <summary>
        /// Gets the old cell format.
        /// </summary>
        /// <value>The old cell format.</value>
        internal CellFormat TrackCellFormat
        {
            get
            {
                if (m_trackCellFormat == null)
                {
                    m_trackCellFormat = new CellFormat();
                    m_trackCellFormat.SetOwner(this);
                }
                return m_trackCellFormat;
            }
        }
        /// <summary>
        /// Gets the preferred width of the cell.
        /// </summary>
        /// <value>The preferred cell width.</value>
        internal PreferredWidthInfo PreferredWidth
        {
            get
            {
                return CellFormat.PreferredWidth;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WTableCell"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public WTableCell(IWordDocument document)
            : base((WordDocument)document, null)
        {
            m_cellFormat = new CellFormat();
            m_cellFormat.SetOwner(this);
            m_charFormat = new WCharacterFormat(Document);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Entity Clone()
        {
            return (Entity)CloneImpl();
        }
        /// <summary>
        /// Get cell index in the table row.
        /// </summary>
        /// <returns></returns>
        public int GetCellIndex()
        {
            return GetIndexInOwnerCollection();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the width of the table preferred.
        /// </summary>
        /// <param name="prevWidth">Width of the prev.</param>
        /// <param name="newValue">The new value.</param>
        private void UpdateTablePreferredWidth(float prevWidth, float newValue)
        {
            if ((byte)OwnerRow.OwnerTable.PreferredTableWidth.WidthType > 1)
            {
                float rowWidth = OwnerRow.GetRowWidth();
                //Updates the preferred width of table.
                if (OwnerRow.OwnerTable.PreferredTableWidth.WidthType == FtsWidth.Point
                    && OwnerRow.OwnerTable.PreferredTableWidth.Width < rowWidth)
                    OwnerRow.OwnerTable.PreferredTableWidth.Width += newValue - prevWidth;
                else if (OwnerRow.OwnerTable.PreferredTableWidth.WidthType == FtsWidth.Percentage)
                {
                    float ownerWidth = OwnerRow.OwnerTable.GetOwnerWidth();
                    float tableWidth = ownerWidth * OwnerRow.OwnerTable.PreferredTableWidth.Width / DLSConstants.HundredthsUnit;
                    if (tableWidth < rowWidth)
                        OwnerRow.OwnerTable.PreferredTableWidth.Width = ((tableWidth + newValue - prevWidth) / ownerWidth) * DLSConstants.HundredthsUnit;
                }
            }
            //Updates the preferred width of current cell.
            if (CellFormat.HorizontalMerge == CellMerge.Start)
            {
                PreferredWidth.WidthType = FtsWidth.None;
                PreferredWidth.Width = 0;
            }
            else
            {
                if (PreferredWidth.WidthType == FtsWidth.Percentage)
                {
                    float curTableWidth = OwnerRow.OwnerTable.GetTableClientWidth();
                    PreferredWidth.Width = (newValue / curTableWidth) * DLSConstants.HundredthsUnit;
                }
                else
                {
                    PreferredWidth.WidthType = FtsWidth.Point;
                    PreferredWidth.Width = newValue;
                }
            }
        }
        /// <summary>
        /// Applies the table style base formats.
        /// </summary>
        /// <param name="cellFormat">The cell format.</param>
        /// <param name="paraFormat">The para format.</param>
        /// <param name="charFormat">The char format.</param>
        internal void ApplyTableStyleBaseFormats(CellFormat cellFormat, WParagraphFormat paraFormat, WCharacterFormat charFormat)
        {
            CellFormat.ApplyBase(cellFormat);
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] is WParagraph)
                {
                    WParagraph para = Items[i] as WParagraph;
                    para.ParagraphFormat.TableStyleParagraphFormat = paraFormat;
                    para.BreakCharacterFormat.TableStyleCharacterFormat = charFormat;
                    for (int j = 0; j < para.Items.Count; j++)
                    {
                        if(para.Items[j] is ParagraphItem )
                            para.Items[j].ParaItemCharFormat.TableStyleCharacterFormat = charFormat;
                    }
                }
            }
        }
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            WTableCell tc = (WTableCell)base.CloneImpl();

            tc.m_cellFormat = new CellFormat();
            tc.m_cellFormat.SetOwner(tc);
            tc.m_cellFormat.ImportContainer(m_cellFormat);
            tc.m_cellFormat.SetCellDescriptor(m_cellFormat);
            //Import cell paddings
            tc.m_cellFormat.ImportPaddings(m_cellFormat.Paddings);
            tc.m_charFormat = new WCharacterFormat(Document);
            tc.m_charFormat.ImportContainer(CharacterFormat);
            return tc;
        }
        /// <summary>
        /// Clones the cell.
        /// </summary>
        /// <returns></returns>
        internal Entity CloneCell()
        {
            WTableCell tc = (WTableCell)base.CloneImpl();

            tc.m_cellFormat = new CellFormat();
            tc.m_cellFormat.SetOwner(tc);
            tc.m_cellFormat.ImportContainer(m_cellFormat);
            //Import cell paddings
            tc.m_cellFormat.ImportPaddings(m_cellFormat.Paddings);
            tc.m_charFormat = new WCharacterFormat(Document);
            tc.m_charFormat.ImportContainer(CharacterFormat);
            return tc;
        }
        /// <summary>
        /// Clones the relations for TableCell.
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            Entity ent = null;
            for (int i = 0, cnt = ChildEntities.Count; i < cnt; i++)
            {
                ent = ChildEntities[i];
                ent.CloneRelationsTo(doc, nextOwner);
            }
        }
        /// <summary>
        /// Gets the next TextBodyItem in the document 
        /// </summary>
        /// <returns></returns>
        internal TextBodyItem GetNextTextBodyItem()
        {
            if (this.NextSibling == null)
            {
                if (this.OwnerRow == null)
                    return null;

                if (this.OwnerRow.NextSibling == null)
                {
                    if (this.OwnerRow.OwnerTable != null)
                        return this.OwnerRow.OwnerTable.GetNextTextBodyItem();
                    else
                        return null;
                }
                else
                {
                    WTableRow row = this.OwnerRow;
                    while (row.NextSibling != null)
                    {
                        row = row.NextSibling as WTableRow;
                        foreach (WTableCell cell in row.Cells)
                        {
                            if (cell.Items.Count > 0)
                            {
                                return cell.Items[0] as TextBodyItem;
                            }
                        }
                    }

                    return this.OwnerRow.OwnerTable.GetNextTextBodyItem();
                }
            }
            else
            {
                WTableCell tblCell = this.NextSibling as WTableCell;
                if (tblCell.Items.Count > 0)
                    return tblCell.Items[0] as TextBodyItem;
                else
                    return tblCell.GetNextTextBodyItem();
            }
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            base.Close();
            if (m_charFormat != null)
            {
                m_charFormat.Close();
                m_charFormat = null;
            }

            if (m_cellFormat != null)
            {
                m_cellFormat.Close();
                m_cellFormat = null;
            }
        }
        #endregion

        #region Implementation / xml
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();
            XDLSHolder.AddElement(XDLSConstants.CellFormatTag, CellFormat);
            XDLSHolder.AddElement(XDLSConstants.CharacterFormatTag, CharacterFormat);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (m_cellFormat.OwnerRowFormat.HasSprms())
                return;

            if (IsFixedWidth)
            {
                writer.WriteValue(XDLSConstants.TableCellWidthAttr, Width);
            }

            if (ForeColor != Color.Empty)
            {
                writer.WriteValue(XDLSConstants.TableCellForeColorAttr, ForeColor);
            }

            writer.WriteValue(XDLSConstants.TableCellTextureAttr, TextureStyle);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.TableCellWidthAttr))
            {
                Width = reader.ReadFloat(XDLSConstants.TableCellWidthAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TableCellForeColorAttr))
            {
                ForeColor = reader.ReadColor(XDLSConstants.TableCellForeColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TableCellTextureAttr))
            {
                TextureStyle = (TextureStyle)reader.ReadEnum(XDLSConstants.TableCellTextureAttr, typeof(TextureStyle));
            }
        }
//#endif
        #endregion

        #region Implementation / layout
//#if !SILVERLIGHT
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            if (Paragraphs.Count == 0)
            {
                // NOTE: if paragraphs count = 0 ???
                AddParagraph();
            }

            m_layoutInfo = new LayoutCellInfo(this);
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override IEntityCollectionBase WidgetCollection
        {
            get
            {
                return m_bodyItems;
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            dc.DrawTableCell(this, ltWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
#endif
//#endif
        #endregion

        #region Class Internal declarations
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        internal class LayoutCellInfo
          : TableLayoutInfo
        {
            #region Class Members
            /// <summary>
            /// 
            /// </summary>
            WTableCell m_cell;
            private bool m_bSkipTopBorder;
            private bool m_bSkipBottomBorder;
            private bool m_bSkipLeftBorder;
            private bool m_bSkipRightBorder;
            private Borders m_borders;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets a value indicating whether to skip top border.
            /// </summary>
            /// <value><c>true</c> if skip top border; otherwise, <c>false</c>.</value>
            internal bool SkipTopBorder
            {
                get
                {
                    return m_bSkipTopBorder;
                }
                set
                {
                    m_bSkipTopBorder = value;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether to skip bottom border.
            /// </summary>
            /// <value><c>true</c> if skip bottom border; otherwise, <c>false</c>.</value>
            internal bool SkipBottomBorder
            {
                get
                {
                    return m_bSkipBottomBorder;
                }
                set
                {
                    m_bSkipBottomBorder = value;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether to skip left border.
            /// </summary>
            /// <value><c>true</c> if skip left border; otherwise, <c>false</c>.</value>
            internal bool SkipLeftBorder
            {
                get
                {
                    return m_bSkipLeftBorder;
                }
                set
                {
                    m_bSkipLeftBorder = value;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether to skip right border.
            /// </summary>
            /// <value><c>true</c> if skip right border; otherwise, <c>false</c>.</value>
            internal bool SkipRightBorder
            {
                get
                {
                    return m_bSkipRightBorder;
                }
                set
                {
                    m_bSkipRightBorder = value;
                }
            }
            /// <summary>
            /// Get's the cell borders
            /// </summary>
            internal Borders CellBorders
            {
                get
                {
                    if (m_borders == null)
                    {
                        m_borders = new Borders();
                    }
                    return m_borders;
                }
            }
            #endregion

            #region Class initialize/finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="LayoutCellInfo"/> class.
            /// </summary>
            /// <param name="cell">The cell.</param>
            internal LayoutCellInfo(WTableCell cell)
                : base(ChildrenLayoutDirection.Vertical)
            {
                m_cell = cell;
                InitSpacings();
                InitMerges();
                CellFormat cellFormat = m_cell.CellFormat;
                if (cellFormat.TextDirection != TextDirection.Horizontal)
                    IsVerticalText = true;
                VerticalAlignment = (byte)cellFormat.VerticalAlignment;
                TextWrap = cellFormat.TextWrap;
            }
            #endregion

            #region Class helper methods
            /// <summary>
            /// Determines the formats.
            /// </summary>
            private void InitMerges()
            {
                CellFormat cellFormat = m_cell.CellFormat;
                int cellIndex = m_cell.OwnerRow.Cells.IndexOf(m_cell);
                IsColumnMergeStart = (cellFormat.HorizontalMerge == CellMerge.Start && cellIndex < m_cell.OwnerRow.Cells.Count - 1);
                IsColumnMergeContinue = (cellFormat.HorizontalMerge == CellMerge.Continue);
                IsRowMergeStart = (cellFormat.VerticalMerge == CellMerge.Start);
                IsRowMergeContinue = (cellFormat.VerticalMerge == CellMerge.Continue);
            }
            /// <summary>
            /// Determines the cell spacing.
            /// </summary>
            private void InitSpacings()
            {
                Paddings paddings = m_cell.CellFormat.Paddings;
                float left = m_cell.CellFormat.Paddings.Left;
                float right = m_cell.CellFormat.Paddings.Right;
                float top = m_cell.CellFormat.Paddings.Top;
                float bottom = m_cell.CellFormat.Paddings.Bottom;

                int cellIndex = m_cell.GetCellIndex();
                int rowIndex = m_cell.OwnerRow.GetRowIndex();
                int cellLast = m_cell.OwnerRow.Cells.Count - 1;
                int rowLast = m_cell.OwnerRow.OwnerTable.Rows.Count - 1;

                Border leftBorder = m_cell.CellFormat.Borders.Left;
                Border rightBorder = m_cell.CellFormat.Borders.Right;
                Border topBorder = m_cell.CellFormat.Borders.Top;
                Border bottomBorder = m_cell.CellFormat.Borders.Bottom;

                float leftHalfWidth = GetLeftHalfWidth(cellIndex,ref leftBorder);
                float topHalfWidth = GetTopHalfWidth(cellIndex, rowIndex, ref topBorder);
                float rightHalfWidth = GetRightHalfWidth(cellIndex, cellLast,ref rightBorder);
                float bottomHalfWidth = GetBottomHalfWidth(cellIndex, cellLast, rowIndex, rowLast,ref bottomBorder);
                ImportBorders(leftBorder, rightBorder, topBorder, bottomBorder);
                float cellSpacing = 0;
                if (m_cell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                    cellSpacing = m_cell.OwnerRow.OwnerTable.TableFormat.CellSpacing * 2;
#region Paddings
                if (m_cell.CellFormat.SamePaddingsAsTable)
                {
                    WTable table= m_cell.OwnerRow.OwnerTable;
                    WTableStyle tableStyle = null;
                    if (table.StyleName != null && table.StyleName != string.Empty && table.Document.StyleNameIds.ContainsValue(table.StyleName))
                    {
                        tableStyle = table.Document.Styles.FindByName(table.StyleName) as WTableStyle;
                    }
                    //Left
                    if (m_cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.LeftKey))
                    {
                        left = m_cell.OwnerRow.RowFormat.Paddings.Left;
                    }
                    else if (table.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.LeftKey))
                    {
                        left = table.TableFormat.Paddings.Left;
                    }
                    else
                    {
                        //Doc format document can have default cell margin value as zero.
                        if (m_cell.Document.ActualFormatType == FormatType.Doc)
                            left = 0.0f;
                        else if (tableStyle != null && tableStyle.TableProperties.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.LeftKey))
                            left = tableStyle.TableProperties.Paddings.Left;
                        else
                            left = 5.4f;
                    }
                    //Right
                    if (m_cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
                    {
                        right = m_cell.OwnerRow.RowFormat.Paddings.Right;
                    }
                    else if (table.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
                    {
                        right = table.TableFormat.Paddings.Right;
                    }
                    //Doc format document can have default cell margin value as zero.
                    else
                    {
                        if (m_cell.Document.ActualFormatType == FormatType.Doc)
                            right = 0.0f;
                        else if (tableStyle != null && tableStyle.TableProperties.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
                            right = tableStyle.TableProperties.Paddings.Right;
                        else
                            right = 5.4f;
                    }
                    //Top
                    if (m_cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.TopKey))
                    {
                        top = m_cell.OwnerRow.RowFormat.Paddings.Top;
                    }
                    else if (table.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.TopKey))
                    {
                        top = table.TableFormat.Paddings.Top;
                    }
                    else
                    {
                        if (tableStyle != null && tableStyle.TableProperties.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.TopKey))
                            top = tableStyle.TableProperties.Paddings.Top;
                        else
                            top = 0.0f;
                    }
                    for (int i = 0; i < m_cell.OwnerRow.Cells.Count; i++)
                    {
                        if (!m_cell.OwnerRow.Cells[i].CellFormat.SamePaddingsAsTable)
                        {
                            GetMaxTopPadding(ref top, i);
                        }
                    }
                    //Bottom
                    if (m_cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey))
                    {
                        bottom = m_cell.OwnerRow.RowFormat.Paddings.Bottom;
                    }
                    else if (table.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey))
                    {
                        bottom = table.TableFormat.Paddings.Bottom;
                    }
                    else
                    {
                        if (tableStyle != null && tableStyle.TableProperties.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey))
                            bottom = tableStyle.TableProperties.Paddings.Bottom;
                        else
                            bottom = 0.0f;
                    }
                    for (int i = 0; i < m_cell.OwnerRow.Cells.Count; i++)
                    {
                        if (!m_cell.OwnerRow.Cells[i].CellFormat.SamePaddingsAsTable)
                        {
                            GetMaxBottomPadding(ref bottom, i);
                        }
                    }
                }
                else
                {
                    left = GetLeftPadding();
                    right = GetRightPadding();
                    top = GetTopPadding();
                    bottom = GetBottomPadding();
                }
#endregion

                Paddings.Left = left != 0 ? left - leftHalfWidth : 0;
                Paddings.Top = (top >= 0) ? top : m_cell.OwnerRow.RowFormat.Paddings.Top > 0 ? m_cell.OwnerRow.RowFormat.Paddings.Top : 0;
                Paddings.Right = right != 0 ? right - rightHalfWidth : 0;
                Paddings.Bottom = (bottom >= 0) ? bottom : m_cell.OwnerRow.RowFormat.Paddings.Bottom > 0 ? m_cell.OwnerRow.RowFormat.Paddings.Bottom : 0;

                float lineGap = 3.0f;//Line space between the center line and outerline.
                Margins.Left = cellSpacing + leftHalfWidth;
                if (leftBorder.BorderType == BorderStyle.ThinThickThinSmallGap)
                    Margins.Left += lineGap+leftHalfWidth;
                Margins.Top = cellSpacing + topHalfWidth;
                if (topBorder.BorderType == BorderStyle.ThinThickThinSmallGap)
                    Margins.Top += lineGap + topHalfWidth;
                Margins.Right = cellSpacing + rightHalfWidth;
                Margins.Bottom = cellSpacing + bottomHalfWidth;
            }
            /// <summary>
            /// Import cell Borders
            /// </summary>
            /// <param name="leftBorder"></param>
            /// <param name="rightBorder"></param>
            /// <param name="topBorder"></param>
            /// <param name="bottomBorder"></param>
            private void ImportBorders(Border leftBorder, Border rightBorder, Border topBorder, Border bottomBorder)
            {
                //Import left border
                ImportBorder(CellBorders.Left, leftBorder);
                //Import right border
                ImportBorder(CellBorders.Right, rightBorder);
                //Import top border
                ImportBorder(CellBorders.Top, topBorder);
                //Import bottom border
                ImportBorder(CellBorders.Bottom, bottomBorder);
            }
            /// <summary>
            /// Import Border
            /// </summary>
            /// <param name="destBorder"></param>
            /// <param name="srcBorder"></param>
            private void ImportBorder(Border destBorder, Border srcBorder)
            {
                if (!srcBorder.IsDefault)
                {
                    destBorder.Color = srcBorder.Color;
                    destBorder.BorderType = srcBorder.BorderType;
                    destBorder.LineWidth = srcBorder.LineWidth;
                    destBorder.Space = srcBorder.Space;
                    destBorder.Shadow = srcBorder.Shadow;
                }
                else
                {
                    destBorder.Color = Color.Empty;
                    destBorder.BorderType = 0;
                    destBorder.LineWidth = 0;
                    destBorder.Space = 0;
                    destBorder.Shadow = false;
                }
            }
            /// <summary>
            /// Gets the Left HalfWidth.
            /// </summary>
            /// <param name="cellIndex">The cellIndex.</param>
            /// <returns></returns>
            private float GetLeftHalfWidth( int cellIndex,ref Border leftBorder)
            {
                Borders borders = m_cell.CellFormat.Borders;
                Borders tableBorders = m_cell.OwnerRow.OwnerTable.TableFormat.Borders;
                Borders rowBorders = m_cell.OwnerRow.RowFormat.Borders;
                float leftHalfWidth = borders.Left.LineWidth / 2;
                if (m_cell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                {
                    leftHalfWidth = ((borders.Left.BorderType != BorderStyle.None) ?
                        borders.Left.LineWidth : tableBorders.Vertical.LineWidth) / 2;
                    if (borders.Left.BorderType == BorderStyle.None)
                        leftBorder = tableBorders.Vertical;
                }
                else
                {
                    if (cellIndex > 0)
                    {
                        WTableCell prevCell = GetAdjacentCell(cellIndex - 1);
                        if (borders.Left.BorderType != BorderStyle.None && prevCell.CellFormat.Borders.Right.IsBorderDefined)
                        {
                            SkipLeftBorder = IsSkipBorder(borders.Left.LineWidth, prevCell.CellFormat.Borders.Right.LineWidth, true, ref leftHalfWidth);
                            if (SkipLeftBorder)
                                leftBorder = prevCell.CellFormat.Borders.Right;
                        }
                        else if (borders.Left.BorderType != BorderStyle.None)
                        {
                            SkipLeftBorder = IsSkipBorder(borders.Left.LineWidth, tableBorders.Vertical.LineWidth, true, ref leftHalfWidth);
                            if (SkipLeftBorder)
                                leftBorder = tableBorders.Vertical;
                        }
                        else if (prevCell.CellFormat.Borders.Right.IsBorderDefined)
                        {
                            if (tableBorders.Vertical.LineWidth < prevCell.CellFormat.Borders.Right.LineWidth)
                            {
                                leftHalfWidth = prevCell.CellFormat.Borders.Right.LineWidth / 2;
                                leftBorder = prevCell.CellFormat.Borders.Right;
                                SkipLeftBorder = true;
                            }
                            else if (borders.Left.BorderType == BorderStyle.None && borders.Left.HasNoneStyle)
                            {
                                SkipLeftBorder = true;
                                leftHalfWidth = 0;
                                leftBorder = borders.Left;
                            }
                            else
                            {
                                leftHalfWidth = tableBorders.Vertical.LineWidth / 2;
                                leftBorder = tableBorders.Vertical;
                                SkipLeftBorder = (rowBorders.Vertical.BorderType == BorderStyle.None) ? (tableBorders.Vertical.BorderType == BorderStyle.None) : false;
                            }
                        }
                        else
                        {
                            leftHalfWidth = tableBorders.Vertical.LineWidth / 2;
                            leftBorder = tableBorders.Vertical;
                        }
                    }
                    else
                    {
                        leftHalfWidth = ((borders.Left.BorderType != BorderStyle.None) ?
                            borders.Left.LineWidth : tableBorders.Left.LineWidth) / 2;
                        if (borders.Left.BorderType == BorderStyle.None)
                            leftBorder = tableBorders.Left;
                    }
                }
                return leftHalfWidth;
            }
            /// <summary>
            /// Gets the Top HalfWidth.
            /// </summary>
            /// <param name="cellIndex">The cellIndex.</param>
            /// <param name="rowIndex">The rowIndex.</param>
            /// <returns></returns>
            private float GetTopHalfWidth(int cellIndex, int rowIndex, ref Border topBorder)
            {
                Borders borders = m_cell.CellFormat.Borders;
                Borders tableBorders = m_cell.OwnerRow.OwnerTable.TableFormat.Borders;
                Borders rowBorders = m_cell.OwnerRow.RowFormat.Borders;
                float topHalfWidth = borders.Top.LineWidth / 2;
                if (m_cell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                {
                    topHalfWidth = ((borders.Top.BorderType != BorderStyle.None) ?
                        borders.Top.LineWidth : tableBorders.Horizontal.LineWidth) / 2;
                    if (borders.Top.BorderType == BorderStyle.None)
                        topBorder = tableBorders.Horizontal;
                }
                else
                {
                    if (rowIndex > 0)
                    {
                        WTableCell prevRowCell = GetAdjacentRowCell(rowIndex - 1);
                        if (borders.Top.BorderType != BorderStyle.None && prevRowCell.CellFormat.Borders.Bottom.IsBorderDefined)
                        {
                            SkipTopBorder = IsSkipBorder(borders.Top.LineWidth, prevRowCell.CellFormat.Borders.Bottom.LineWidth, true, ref topHalfWidth);
                            if (SkipTopBorder)
                                topBorder = prevRowCell.CellFormat.Borders.Bottom;
                            else if (prevRowCell.CellFormat.Borders.Bottom.BorderType == BorderStyle.Cleared && borders.Top.BorderType == BorderStyle.Cleared)
                                SkipTopBorder = true;
                        }
                        else if (borders.Top.BorderType != BorderStyle.None)
                        {
                            if (borders.Top.BorderType == BorderStyle.Cleared && rowIndex != 0 && (prevRowCell.OwnerRow.RowFormat.Borders.Horizontal.IsBorderDefined||tableBorders.Horizontal.IsBorderDefined))
                            {
                                SkipTopBorder = false;
                                topBorder = (prevRowCell.OwnerRow.RowFormat.Borders.Horizontal.IsBorderDefined) ? prevRowCell.OwnerRow.RowFormat.Borders.Horizontal : tableBorders.Horizontal;
                            }
                            else
                                SkipTopBorder = IsSkipBorder(borders.Top.LineWidth, tableBorders.Horizontal.LineWidth, true, ref topHalfWidth);
                            if (SkipTopBorder)
                                topBorder = tableBorders.Horizontal;
                        }
                        else if (prevRowCell.CellFormat.Borders.Bottom.IsBorderDefined && !prevRowCell.CellFormat.Borders.Bottom.HasNoneStyle)
                        {
                            if (rowBorders.Horizontal.IsBorderDefined)
                            {
                                if (rowBorders.Horizontal.LineWidth < prevRowCell.CellFormat.Borders.Bottom.LineWidth)
                                {
                                    topHalfWidth = prevRowCell.CellFormat.Borders.Bottom.LineWidth / 2;
                                    topBorder = prevRowCell.CellFormat.Borders.Bottom;
                                    SkipTopBorder = true;
                                }
                                else
                                {
                                    topHalfWidth = rowBorders.Horizontal.LineWidth / 2;
                                    SkipTopBorder = false;
                                    topBorder = rowBorders.Horizontal;
                                }
                            }
                            else
                            {
                                if (tableBorders.Horizontal.LineWidth < prevRowCell.CellFormat.Borders.Bottom.LineWidth)
                                {
                                    topHalfWidth = prevRowCell.CellFormat.Borders.Bottom.LineWidth / 2;
                                    SkipTopBorder = true;
                                    topBorder = prevRowCell.CellFormat.Borders.Bottom;
                                }
                                else
                                {
                                    topHalfWidth = tableBorders.Horizontal.LineWidth / 2;
                                    SkipTopBorder = (rowBorders.Horizontal.BorderType == BorderStyle.None) ? (tableBorders.Horizontal.BorderType == BorderStyle.None) : false;
                                    topBorder = tableBorders.Horizontal;
                                }
                            }
                        }
                        else if (borders.Top.BorderType == BorderStyle.None && borders.Top.HasNoneStyle)
                        {
                            SkipTopBorder = true;
                        }
                        else
                        {
                            topHalfWidth = tableBorders.Horizontal.LineWidth / 2;
                            topBorder = tableBorders.Horizontal;
                        }
                    }
                    else
                    {
                        topHalfWidth = ((borders.Top.BorderType != BorderStyle.None) ?
                            borders.Top.LineWidth : tableBorders.Top.LineWidth) / 2;
                        if (borders.Top.BorderType == BorderStyle.None)
                            topBorder = tableBorders.Top;
                    }
                }
                return topHalfWidth;
            }
            /// <summary>
            /// Gets the Right HalfWidth.
            /// </summary>
            /// <param name="cellIndex">The cellIndex.</param>
            /// <param name="cellLast">The cellLast.</param>
            /// <returns></returns>
            private float GetRightHalfWidth(int cellIndex, int cellLast, ref Border rightBorder)
            {
                Borders borders = m_cell.CellFormat.Borders;
                Borders tableBorders = m_cell.OwnerRow.OwnerTable.TableFormat.Borders;
                int nextCellIndex = GetNextCellIndex(cellIndex, cellLast);
                if (nextCellIndex - 1 > cellIndex)
                    borders = m_cell.OwnerRow.Cells[nextCellIndex - 1].CellFormat.Borders;
                float rightHalfWidth = borders.Right.LineWidth / 2;
                if (m_cell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                {
                    rightHalfWidth = ((borders.Right.BorderType != BorderStyle.None) ?
                        borders.Right.LineWidth : tableBorders.Vertical.LineWidth) / 2;
                    if (borders.Right.BorderType == BorderStyle.None)
                        rightBorder = tableBorders.Vertical;
                }
                else
                {
                    if (cellIndex < cellLast)
                    {
                        WTableCell nextCell = GetAdjacentCell(nextCellIndex);
                        if ((borders.Right.BorderType != BorderStyle.None || borders.Right.HasNoneStyle) && nextCell.CellFormat.Borders.Left.IsBorderDefined)
                        {
                            SkipRightBorder = IsSkipBorder(borders.Right.LineWidth, nextCell.CellFormat.Borders.Left.LineWidth, false, ref rightHalfWidth);
                            if (SkipRightBorder)
                                rightBorder = nextCell.CellFormat.Borders.Left;
                        }
                        else if (borders.Right.BorderType != BorderStyle.None)
                        {
                            SkipRightBorder = IsSkipBorder(borders.Right.LineWidth, tableBorders.Vertical.LineWidth, false, ref rightHalfWidth);
                            if (SkipRightBorder)
                                rightBorder = tableBorders.Vertical;
                        }
                        else if (nextCell.CellFormat.Borders.Left.IsBorderDefined)
                        {
                            SkipRightBorder = IsSkipBorder(tableBorders.Vertical.LineWidth, nextCell.CellFormat.Borders.Left.LineWidth, false, ref rightHalfWidth);
                            if (SkipRightBorder)
                                rightBorder = nextCell.CellFormat.Borders.Left;
                        }
                        else
                        {
                            rightHalfWidth = tableBorders.Vertical.LineWidth / 2;
                            SkipRightBorder = true;
                            rightBorder = tableBorders.Vertical;
                        }
                        if (borders.Right.BorderType == BorderStyle.Cleared)
                        {
                            int start = nextCell.OwnerRow.GetRowIndex();
                            int rowIndex = m_cell.OwnerRow.GetRowIndex();
                            if (start < rowIndex)
                            {
                                ((nextCell as IWidget).LayoutInfo as WTableCell.LayoutCellInfo).SkipLeftBorder = true;
                                int index = nextCell.GetCellIndex();
                                if (index > 0)
                                {
                                    for (int i = start; i < rowIndex; i++)
                                    {
                                        int adjCellIndex = GetAdjacentCellIndex(nextCell, index, i);
                                        if (adjCellIndex > 0)
                                            ((m_cell.OwnerRow.OwnerTable.Rows[i].Cells[adjCellIndex - 1] as IWidget).LayoutInfo as WTableCell.LayoutCellInfo).SkipRightBorder = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        rightHalfWidth = ((borders.Right.BorderType != BorderStyle.None) ?
                        borders.Right.LineWidth : tableBorders.Right.LineWidth) / 2;
                        if (borders.Right.BorderType == BorderStyle.None)
                            rightBorder = tableBorders.Right;
                    }
                }
                return rightHalfWidth;
            }
            /// <summary>
            /// Gets the Bottom HalfWidth.
            /// </summary>
            /// <param name="cellIndex">The cellIndex.</param>
            /// <param name="cellLast">The cellLast.</param>
            /// <param name="rowIndex">The rowIndex.</param>
            /// <param name="rowLast">The rowLast.</param>
            /// <returns></returns>
            private float GetBottomHalfWidth(int cellIndex, int cellLast, int rowIndex, int rowLast, ref Border bottomBorder)
            {
                Borders borders = m_cell.CellFormat.Borders;
                Borders tableBorders = m_cell.OwnerRow.OwnerTable.TableFormat.Borders;
                Borders rowBorders = m_cell.OwnerRow.RowFormat.Borders; 
                float bottomHalfWidth = borders.Bottom.GetLineWidth() / 2;
                if (m_cell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                {
                    bottomHalfWidth = ((borders.Bottom.BorderType != BorderStyle.None) ?
                        borders.Bottom.GetLineWidth () : tableBorders.Horizontal.GetLineWidth ()) / 2;
                    if (borders.Bottom.BorderType == BorderStyle.None)
                        bottomBorder = tableBorders.Horizontal;
                }
                else
                {
                    int nextRowIndex = GetNextRowIndex(cellIndex, rowIndex, rowLast);
                    if (rowIndex < rowLast)
                    {
                        int nextCellIndex = GetAdjacentCellIndex(m_cell, cellIndex, nextRowIndex);
                        Borders nextRowBorders = m_cell.OwnerRow.OwnerTable.Rows[nextRowIndex].RowFormat.Borders;
                        WTableCell nextRowCell = GetAdjacentRowCell(nextRowIndex);
                        if (borders.Bottom.BorderType != BorderStyle.None && nextRowCell.CellFormat.Borders.Top.IsBorderDefined)
                        {
                            SkipBottomBorder = IsSkipBorder(borders.Bottom.LineWidth, nextRowCell.CellFormat.Borders.Top.LineWidth, false, ref bottomHalfWidth);
                            if (SkipBottomBorder)
                                bottomBorder = nextRowCell.CellFormat.Borders.Top;
                        }
                        else if (borders.Bottom.BorderType != BorderStyle.None)
                        {
                            if (rowBorders.Horizontal.IsBorderDefined)
                            {
                                SkipBottomBorder = IsSkipBorder(borders.Bottom.LineWidth, rowBorders.Horizontal.LineWidth, false, ref bottomHalfWidth);
                                if (SkipBottomBorder)
                                    bottomBorder = rowBorders.Horizontal;
                            }
                            else
                            {
                                SkipBottomBorder = IsSkipBorder(borders.Bottom.LineWidth, tableBorders.Horizontal.LineWidth, false, ref bottomHalfWidth);
                                if (SkipBottomBorder)
                                    bottomBorder = tableBorders.Horizontal;
                            }
                        }
                        else if (nextRowCell.CellFormat.Borders.Top.IsBorderDefined && !nextRowCell.CellFormat.Borders.Top.HasNoneStyle)
                        {
                            if (nextRowBorders.Horizontal.IsBorderDefined)
                            {
                                SkipBottomBorder = IsSkipBorder(nextRowBorders.Horizontal.LineWidth, nextRowCell.CellFormat.Borders.Top.LineWidth, false, ref bottomHalfWidth);
                                if (SkipBottomBorder)
                                    bottomBorder = nextRowCell.CellFormat.Borders.Top;
                                else
                                    bottomBorder = nextRowBorders.Horizontal;
                            }
                            else
                            {
                                SkipBottomBorder = IsSkipBorder(tableBorders.Horizontal.LineWidth, nextRowCell.CellFormat.Borders.Top.LineWidth, false, ref bottomHalfWidth);
                                if (SkipBottomBorder)
                                    bottomBorder = nextRowCell.CellFormat.Borders.Top;
                                else
                                    bottomBorder = tableBorders.Horizontal;
                            }
                        }
                        else if (borders.Bottom.BorderType == BorderStyle.None && borders.Bottom.HasNoneStyle)
                        {
                            SkipBottomBorder = true;
                            bottomHalfWidth = 0;
                            bottomBorder = borders.Bottom;
                        }
                        else
                        {
                            bottomHalfWidth = tableBorders.Horizontal.LineWidth / 2;
                            bottomBorder = tableBorders.Horizontal;
                            SkipBottomBorder = (nextRowBorders.Horizontal.IsBorderDefined) ? (nextRowBorders.Horizontal.BorderType != BorderStyle.None) : true;
                        }
                        if (borders.Bottom.BorderType == BorderStyle.Cleared)
                            SkipBottomBorder = true;
                        if (SkipBottomBorder
                            && m_cell.OwnerRow.OwnerTable.Rows[nextRowIndex].Cells[nextCellIndex].Width < m_cell.Width
                            && borders.Bottom.BorderType != BorderStyle.None)
                            SkipBottomBorder = false;
                        //Check the current cell end position greater position next adjust cell index means false the skipbottomboreder property.
                        if (SkipBottomBorder && nextCellIndex == m_cell.OwnerRow.OwnerTable.Rows[nextRowIndex].Cells.Count - 1
                            && GetCellEndPosition(nextRowIndex, nextCellIndex) < GetCellEndPosition(rowIndex, cellIndex))
                            SkipBottomBorder = false;
                    }
                    else
                    {
                        //if the border style is thin thic thin then getLineWidth() methode will retuns the total width of the thin thic thin border, else it will returns the single line width.
                        bottomHalfWidth = ((borders.Bottom.BorderType != BorderStyle.None) ?
                        borders.Bottom.GetLineWidth () : tableBorders.Bottom.GetLineWidth ()) / 2;
                        if (borders.Bottom.BorderType == BorderStyle.None)
                            bottomBorder = tableBorders.Bottom;
                    }
                }
                return bottomHalfWidth;
            }
            /// <summary>
            /// Gets the adjacent cell.
            /// Returns the vertical merge start cell for merged cells.
            /// </summary>
            /// <returns>Returns the adjacent cell, ie., left and right cell</returns>
            internal WTableCell GetAdjacentCell(int cellIndex)
            {
                int rowIndex = m_cell.OwnerRow.GetRowIndex();
                if (rowIndex > 0 && m_cell.CellFormat.VerticalMerge == CellMerge.Continue)
                {
                    int prevCellIndex = GetAdjacentCellIndex(m_cell, cellIndex, rowIndex - 1);
                    if (m_cell.OwnerRow.OwnerTable.Rows[rowIndex - 1].Cells[prevCellIndex].CellFormat.VerticalMerge == CellMerge.Continue)
                    {
                        for (int i = rowIndex - 1; i >= 0; i--)
                        {
                            prevCellIndex = GetAdjacentCellIndex(m_cell, cellIndex, i);
                            if (m_cell.OwnerRow.OwnerTable.Rows[i].Cells[prevCellIndex].CellFormat.VerticalMerge == CellMerge.Start)
                            {
                                return m_cell.OwnerRow.OwnerTable.Rows[i].Cells[prevCellIndex];
                            }
                        }
                    }
                    else if (m_cell.OwnerRow.OwnerTable.Rows[rowIndex - 1].Cells[prevCellIndex].CellFormat.VerticalMerge == CellMerge.Start)
                    {
                        return m_cell.OwnerRow.OwnerTable.Rows[rowIndex - 1].Cells[prevCellIndex];
                    }
                }
                return m_cell.OwnerRow.Cells[cellIndex];
            }
            /// <summary>
            /// Gets the adjacent row cell.
            /// Returns the horizontal merge start cell for merged cells.
            /// </summary>
            /// <param name="rowIndex">Index of the row.</param>
            /// <returns>Returns the adjacent row cell, ie., top and bottom cell</returns>
            internal WTableCell GetAdjacentRowCell(int rowIndex)
            {
                int cellIndex = m_cell.GetCellIndex();
                int adjCellIndex = GetAdjacentCellIndex(m_cell, cellIndex, rowIndex);
                if (adjCellIndex > 0)
                {
                    if (m_cell.OwnerRow.OwnerTable.Rows[rowIndex].Cells[adjCellIndex - 1].CellFormat.HorizontalMerge == CellMerge.Continue)
                    {
                        for (int i = adjCellIndex - 1; i >= 0; i--)
                        {
                            if (m_cell.OwnerRow.OwnerTable.Rows[rowIndex].Cells[i].CellFormat.HorizontalMerge == CellMerge.Start)
                            {
                                return m_cell.OwnerRow.OwnerTable.Rows[rowIndex].Cells[i];
                            }
                        }
                    }
                    else if (m_cell.OwnerRow.OwnerTable.Rows[rowIndex].Cells[adjCellIndex - 1].CellFormat.HorizontalMerge == CellMerge.Start
                        && m_cell.OwnerRow.OwnerTable.Rows[rowIndex].Cells[adjCellIndex].CellFormat.HorizontalMerge != CellMerge.Start)
                    {
                        return m_cell.OwnerRow.OwnerTable.Rows[rowIndex].Cells[adjCellIndex - 1];
                    }
                }
                return m_cell.OwnerRow.OwnerTable.Rows[rowIndex].Cells[adjCellIndex];
            }
            /// <summary>
            /// Determines whether to skip border or not.
            /// </summary>
            /// <param name="value1">The value1.</param>
            /// <param name="value2">The value2.</param>
            /// <param name="isTopOrLeftBorder">True for top and left border.</param>
            /// <param name="lineWidth">The lineWidth.</param>
            /// <returns>
            /// 	<c>true</c> if skip border; otherwise, <c>false</c>.
            /// </returns>
            private bool IsSkipBorder(float value1, float value2, bool isTopOrLeftBorder, ref float lineWidth)
            {
                bool skip = false;
                if (isTopOrLeftBorder ? 
                    (value1 < value2)
                    : (value1 <= value2))
                {
                    lineWidth = value2 / 2;
                    skip = true;
                }
                else
                    lineWidth = value1 / 2;
                return skip;
            }
            /// <summary>
            /// Gets the index of the adjacent cell.
            /// </summary>
            /// <param name="cell">The cell.</param>
            /// <param name="cellIndex">Index of the cell.</param>
            /// <param name="adjRowIndex">Index of the adj row.</param>
            /// <returns></returns>
            private int GetAdjacentCellIndex(WTableCell cell, int cellIndex, int adjRowIndex)
            {
                int adjCellIndex = 0;
                float cellStartPos = 0;
                for (int i = 0; i < cellIndex; i++)
                {
                    cellStartPos += cell.OwnerRow.Cells[i].Width;
                }
                float adjCellStartPos = 0;
                for (int i = 0; i < cell.OwnerRow.OwnerTable.Rows[adjRowIndex].Cells.Count; i++)
                {
                    adjCellStartPos += cell.OwnerRow.OwnerTable.Rows[adjRowIndex].Cells[i].Width;
                    if (Math.Round(cellStartPos, 2) == Math.Round(adjCellStartPos, 2))
                    {
                        if (i == cell.OwnerRow.OwnerTable.Rows[adjRowIndex].Cells.Count - 1)
                            return i;
                        adjCellIndex = i + 1;
                        break;
                    }
                    else if (cellStartPos < adjCellStartPos)
                    {
                        adjCellIndex = i;
                        break;
                    }
                }
                return adjCellIndex;
            }
            /// <summary>
            /// Gets the index of the next cell.
            /// </summary>
            /// <param name="cellIndex">The cellIndex.</param>
            /// <param name="cellLast">The cellLast.</param>
            /// <returns></returns>
            private int GetNextCellIndex(int cellIndex, int cellLast)
            {
                for (int i = cellIndex + 1; i < cellLast + 1; i++)
                {
                    if (m_cell.OwnerRow.Cells[i].CellFormat.HorizontalMerge != CellMerge.Continue)
                        return i;
                }
                return cellIndex;
            }
            /// <summary>
            /// Gets the index of the next row.
            /// </summary>
            /// <param name="cellIndex">The cellIndex.</param>
            /// <param name="rowIndex">The rowIndex.</param>
            /// <param name="rowLast">The rowLast.</param>
            /// <returns></returns>
            private int GetNextRowIndex(int cellIndex, int rowIndex, int rowLast)
            {
                for (int i = rowIndex + 1; i < rowLast + 1; i++)
                {
                    if (cellIndex < m_cell.OwnerRow.OwnerTable.Rows[i].Cells.Count)
                    {
                        if (m_cell.OwnerRow.OwnerTable.Rows[i].Cells[cellIndex].CellFormat.VerticalMerge != CellMerge.Continue)
                            return i;
                    }
                    else
                        return i;
                }
                return rowIndex;
            }
            /// <summary>
            /// Get currnt cell End position
            /// </summary>
            /// <param name="rowIndex">The row index.</param>
            /// <param name="cellIndex">The cell index.</param>
            /// <returns> currnt cell End postion</returns>
            private float GetCellEndPosition(int rowIndex, int cellIndex)
            {
                float cellEndPos = 0;
                for (int i = 0; i < cellIndex; i++)
                {
                    cellEndPos += m_cell.OwnerRow.OwnerTable.Rows[rowIndex].Cells[i].Width;
                }
                return cellEndPos;
            }
            /// <summary>
            /// Returns Left Padding
            /// </summary>
            /// <returns></returns>
            private float GetLeftPadding()
            {
                //Left
                float left = m_cell.CellFormat.Paddings.Left;
                if (m_cell.CellFormat.Paddings.Left == -0.05f)
                {
                    if (m_cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.LeftKey))
                    {
                        left = m_cell.OwnerRow.RowFormat.Paddings.Left;
                    }
                    else if (m_cell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.LeftKey))
                    {
                        left = m_cell.OwnerRow.OwnerTable.TableFormat.Paddings.Left;
                    }
                    //Doc format document can have default cell margin value as zero.
                    else if (m_cell.Document.ActualFormatType == FormatType.Doc)
                        left = 0.0f;
                    else
                        left = 5.4f;
                }
                return left;
            }
            /// <summary>
            /// Returns Right Padding
            /// </summary>
            /// <returns></returns>
            private float GetRightPadding()
            {
                //Right
                float right = m_cell.CellFormat.Paddings.Right;
                if (m_cell.CellFormat.Paddings.Right == -0.05f)
                {
                    if (m_cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
                    {
                        right = m_cell.OwnerRow.RowFormat.Paddings.Right;
                    }
                    else if (m_cell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
                    {
                        right = m_cell.OwnerRow.OwnerTable.TableFormat.Paddings.Right;
                    }
                    //Doc format document can have default cell margin value as zero.
                    else if (m_cell.Document.ActualFormatType == FormatType.Doc)
                        right = 0.0f;
                    else
                        right = 5.4f;
                }
                return right;
            }
            /// <summary>
            /// Returns Top Padding
            /// </summary>
            /// <returns></returns>
            private float GetTopPadding()
            {
                //Top
                float top = m_cell.CellFormat.Paddings.Top;
                if (m_cell.CellFormat.Paddings.Top == -0.05f || (m_cell.CellFormat.Paddings.Top == 0.0f && !m_cell.CellFormat.Paddings.HasValue(Syncfusion.DocIO.DLS.Paddings.TopKey)))
                {
                    if (m_cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.TopKey))
                    {
                        top = m_cell.OwnerRow.RowFormat.Paddings.Top;
                    }
                    else if (m_cell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.TopKey))
                    {
                        top = m_cell.OwnerRow.OwnerTable.TableFormat.Paddings.Top;
                    }
                    else
                        top = 0.0f;
                }
                for (int i = 0; i < m_cell.OwnerRow.Cells.Count; i++)
                {
                    if (m_cell.GetCellIndex() != i)
                        GetMaxTopPadding(ref top, i);
                }
                return top;
            }
            /// <summary>
            /// Get Maximum Top Padding
            /// </summary>
            /// <param name="top"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            private void GetMaxTopPadding(ref float top, int index)
            {
                WTableCell cell = m_cell.OwnerRow.Cells[index];
                float adjacentCellTop = cell.CellFormat.Paddings.Top;
                if (cell.CellFormat.Paddings.Top == -0.05f || (cell.CellFormat.Paddings.Top == 0.0f && !cell.CellFormat.Paddings.HasValue(Syncfusion.DocIO.DLS.Paddings.TopKey)))
                {
                    if (cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.TopKey))
                    {
                        adjacentCellTop = cell.OwnerRow.RowFormat.Paddings.Top;
                    }
                    else if (cell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.TopKey))
                    {
                        adjacentCellTop = cell.OwnerRow.OwnerTable.TableFormat.Paddings.Top;
                    }
                    else
                        adjacentCellTop = 0.0f;
                }
                if (adjacentCellTop > top)
                {
                    top = adjacentCellTop;
                }
            }
            /// <summary>
            /// Returns Bottom Padding
            /// </summary>
            /// <returns></returns>
            private float GetBottomPadding()
            {
                //Bottom
                float bottom = m_cell.CellFormat.Paddings.Bottom;
                if (m_cell.CellFormat.Paddings.Bottom == -0.05f || (m_cell.CellFormat.Paddings.Bottom == 0.0f && !m_cell.CellFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey)))
                {
                    if (m_cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey))
                    {
                        bottom = m_cell.OwnerRow.RowFormat.Paddings.Bottom;
                    }
                    else if (m_cell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey))
                    {
                        bottom = m_cell.OwnerRow.OwnerTable.TableFormat.Paddings.Bottom;
                    }
                    else
                        bottom = 0.0f;
                }
                for (int i = 0; i < m_cell.OwnerRow.Cells.Count; i++)
                {
                    if (m_cell.GetCellIndex() != i)
                        GetMaxBottomPadding(ref bottom, i);
                }
                return bottom;
            }
            /// <summary>
            /// Get Maximum Bottom padding
            /// </summary>
            /// <param name="bottom"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            private void GetMaxBottomPadding(ref float bottom, int index)
            {
                WTableCell cell = m_cell.OwnerRow.Cells[index];
                float adjacentCellBottom = cell.CellFormat.Paddings.Bottom;
                if (cell.CellFormat.Paddings.Bottom == -0.05f || (cell.CellFormat.Paddings.Bottom == 0.0f && !cell.CellFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey)))
                {
                    if (cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey))
                    {
                        adjacentCellBottom = cell.OwnerRow.RowFormat.Paddings.Bottom;
                    }
                    else if (cell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey))
                    {
                        adjacentCellBottom = cell.OwnerRow.OwnerTable.TableFormat.Paddings.Bottom;
                    }
                    else
                        adjacentCellBottom = 0.0f;
                }
                if (adjacentCellBottom > bottom)
                {
                    bottom = adjacentCellBottom;
                }
            }
            #endregion
        }
#endif
        #endregion
    }
}
