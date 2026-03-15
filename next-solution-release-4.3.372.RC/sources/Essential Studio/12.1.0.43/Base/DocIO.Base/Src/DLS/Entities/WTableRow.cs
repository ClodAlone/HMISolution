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

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a table row.
    /// </summary>
    public class WTableRow
      : WidgetBase
      , ICompositeEntity
#if !SILVERLIGHT && !WP
      , IWidget
#endif
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private WCellCollection m_cells;
        private RowFormat m_tableFormat;
        private WCharacterFormat m_charFormat;
        private TableRowHeightType m_heightType;
        private byte[] m_internalData = null;
        internal RowFormat m_trackRowFormat = null;
        private bool m_isDeleteRevision = false;
        private bool m_isInsertRevision = false;
        private bool m_hasTblPrEx = false;
        private StructureDocumentTagRow m_SDTRow;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the child entities.
        /// </summary>
        /// <value>The child entities.</value>
        public EntityCollection ChildEntities
        {
            get
            {
                return m_cells;
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
                return EntityType.TableRow;
            }
        }
        /// <summary>
        /// Gets / sets cell collection.
        /// </summary>
        public WCellCollection Cells
        {
            get
            {
                return m_cells;
            }
            set
            {
                m_cells = value;
            }
        }
        /// <summary>
        /// Get / set table row height type
        /// </summary>
        public TableRowHeightType HeightType
        {
            get
            {
                if (m_heightType == TableRowHeightType.AtLeast)
                {
                    return (Height > 0) ? TableRowHeightType.AtLeast : TableRowHeightType.Exactly;
                }
                return m_heightType;
            }
            set
            {
                if (HeightType != value)
                {
                    if (m_tableFormat.Sprms != null)
                    {
                        m_tableFormat.HasInvalidSprms = true;
                        Height *= -1;
                    }
                    m_heightType = value;
                }
            }
        }
        /// <summary>
        /// Gets table format
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public RowFormat RowFormat
        {
            get
            {
                return m_tableFormat;
            }
        }
        /// <summary>
        /// Gets / sets height of the row.
        /// </summary>
        public float Height
        {
            get
            {
                return m_tableFormat.Height;
            }
            set
            {
                m_tableFormat.Height = value;
            }
        }
        /// <summary>
        /// Gets / sets whether the row is a table header.
        /// </summary>
        public bool IsHeader
        {
            get
            {
                return m_tableFormat.IsHeaderRow;
            }
            set
            {
                m_tableFormat.IsHeaderRow = value;
            }
        }
        /// <summary>
        /// Gets the owner table.
        /// </summary>
        /// <value>The owner table.</value>
        internal WTable OwnerTable
        {
            get
            {
                return Owner as WTable;
            }
        }
        /// <summary>
        /// Gets or sets the data array.
        /// </summary>
        /// <value>The data array.</value>
        internal byte[] DataArray
        {
            get
            {
                return m_internalData;
            }
            set
            {
                m_internalData = value;
            }
        }
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <value>The character format.</value>
        internal WCharacterFormat CharacterFormat
        {
            get
            {
                return m_charFormat;
            }
        }
        /// <summary>
        /// Gets the old row format.
        /// </summary>
        /// <value>The old row format.</value>
        internal RowFormat TrackRowFormat
        {
            get
            {
                if (m_trackRowFormat == null)
                {
                    m_trackRowFormat = new RowFormat();
                }
                return m_trackRowFormat;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is delete revision.
        /// </summary>
        /// <value>
        /// 	if this instance is delete revision, set to <c>true</c>.
        /// </value>
        internal bool IsDeleteRevision
        {
            get
            {
                return (m_isDeleteRevision || CharacterFormat.IsDeleteRevision);
            }
            set
            {
                m_isDeleteRevision = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is insert revision.
        /// </summary>
        /// <value>
        /// 	if this instance is insert revision, set to <c>true</c>.
        /// </value>
        internal bool IsInsertRevision
        {
            get
            {
                return (m_isInsertRevision || CharacterFormat.IsInsertRevision);
            }
            set
            {
                m_isInsertRevision = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance has TblPrEx.
        /// </summary>
        internal bool HasTblPrEx
        {
          get
          {
            return m_hasTblPrEx;
          }
          set
          {
            m_hasTblPrEx = value;
          }
        }
        internal StructureDocumentTagRow SDTRow
        {
            get
            {
                return m_SDTRow;
            }
            set
            {
                m_SDTRow = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WTableRow"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public WTableRow(IWordDocument document)
            : base((WordDocument)document, null)
        {
            m_cells = new WCellCollection(this);
            m_charFormat = new WCharacterFormat(Document);
            m_tableFormat = new RowFormat();
            m_tableFormat.SetOwner(this);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        new public WTableRow Clone()
        {
            return (WTableRow)CloneImpl();
        }
        /// <summary>
        /// Adds the cell.
        /// </summary>
        public WTableCell AddCell()
        {
            return AddCell(true);
        }
        /// <summary>
        /// Adds the cell.
        /// </summary>
        /// <param name="isCopyFormat">Specifies whether to apply the parent row format.</param>
        /// <returns></returns>
        public WTableCell AddCell(bool isCopyFormat)
        {
            WTableCell cell = new WTableCell(Document);
            WTableRow topRow = PreviousSibling as WTableRow;
            WTableCell topCell = null;

            if (topRow != null && topRow.Cells.Count > Cells.Count)
            {
                topCell = topRow.Cells[Cells.Count];
            }

            if (isCopyFormat && topCell != null)
            {
                cell.CellFormat.ImportContainer(topCell.CellFormat);
                cell.Width = topCell.Width;
            }
            else if (isCopyFormat && topCell == null)
            {
                cell.CellFormat.ImportContainer(m_tableFormat);
            }
            Cells.Add(cell);
            return cell;
        }
        /// <summary>
        /// Returns index of the row in owner table.
        /// </summary>
        /// <returns></returns>
        public int GetRowIndex()
        {
            return GetIndexInOwnerCollection();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            foreach (WTableCell cell in Cells)
            {
                cell.AddSelf();
            }
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WTableRow row = (WTableRow)base.CloneImpl();
            row.m_charFormat = new WCharacterFormat(Document);
            row.m_charFormat.ImportContainer(CharacterFormat);
            row.m_tableFormat = new RowFormat(Document);

            row.m_tableFormat.ImportContainer(RowFormat);
            row.m_tableFormat.SetOwner(row);

            if (DataArray != null)
            {
                row.m_internalData = new byte[DataArray.Length];
                DataArray.CopyTo(row.m_internalData, 0);
            }

            row.m_cells = new WCellCollection(row);
            Cells.CloneTo(row.m_cells);

            return row;
        }
        /// <summary>
        /// Clones the relations for TableRow.
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            Entity ent = null;
            for (int i = 0, cnt = ChildEntities.Count; i < cnt; i++)
            {
                ent = ChildEntities[i];
                ent.CloneRelationsTo(doc, nextOwner );
            }
        }
        /// <summary>
        /// Checks the format owner.
        /// </summary>
        private void CheckFormatOwner()
        {
            if (this.RowFormat.OwnerBase != this)
            {
                this.RowFormat.SetOwner(this);
            }
        }
        /// <summary>
        /// Called when cells are inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="cellFormat">The cell format.</param>
        internal void OnInsertCell(int index, CellFormat cellFormat)
        {
            if (this.RowFormat.RowDescriptor != null)
            {
                this.RowFormat.HasInvalidSprms = true;
                this.RowFormat.RowDescriptor.InsertCellDescriptor(index, cellFormat);
            }
            if (OwnerTable != null)
                OwnerTable.m_bIsTableGridUpdated = false;
        }
        /// <summary>
        /// Called when cells are removed.
        /// </summary>
        /// <param name="index">The index.</param>
        internal void OnRemoveCell(int index)
        {
            if (this.RowFormat.RowDescriptor != null)
            {
                this.RowFormat.HasInvalidSprms = true;
                this.RowFormat.RowDescriptor.RemoveCellDescriptor(index);
            }
            if (OwnerTable != null)
                OwnerTable.m_bIsTableGridUpdated = false;
        }
        /// <summary>
        /// Gets the width to resize cells.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="clientWidth">Width of the client.</param>
        /// <returns></returns>
        internal float GetWidthToResizeCells(WTableRow row, float clientWidth)
        {
            float rowWidth = 0;
            bool useRowWidth = false;
            for (int i = 0; i < Cells.Count; i++)
            {
                rowWidth += Cells[i].Width;
                foreach (WTable table in Cells[i].Tables)
                {
                    if (table.PreferredTableWidth.WidthType == FtsWidth.Point
                        && table.PreferredTableWidth.Width > 0)
                    {
                        //Handled to preserve the existing table grid width, if nested table's preferred width exceeds the parent table width.
                        useRowWidth = true;
                        break;
                    }
                }
            }
            if (useRowWidth)
                return rowWidth;
            return clientWidth;
        }
        /// <summary>
        /// Gets the width of the row.
        /// </summary>
        /// <returns></returns>
        internal float GetRowWidth()
        {
            float rowWidth = 0;
            for (int i = 0; i < Cells.Count; i++)
            {
                rowWidth += Cells[i].Width;
            }
            return rowWidth;
        }
        /// <summary>
        /// Gets the preferred width of the row.
        /// </summary>
        /// <param name="tableWidth">Width of the table.</param>
        /// <returns></returns>
        internal float GetRowPreferredWidth(float tableWidth)
        {
            float rowWidth = 0;
            foreach (WTableCell cell in Cells)
            {
                if (cell.PreferredWidth.WidthType == FtsWidth.Point)
                    rowWidth += (float)Math.Round(cell.PreferredWidth.Width * DLSConstants.TwipsInOnePoint);
                else if (cell.PreferredWidth.WidthType == FtsWidth.Percentage)
                    rowWidth += (float)Math.Round(tableWidth * cell.PreferredWidth.Width / 5);
            }
            return rowWidth;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            if (m_cells != null && m_cells.Count > 0)
            {
                int cnt = m_cells.Count;
                WTableCell cell = null;
                for (int i = 0; i < cnt; i++)
                {
                    cell = m_cells[i];
                    cell.Close();
                    cell = null;
                }

                m_cells.Clear();
                m_cells = null;
            }

            if (m_tableFormat != null)
            {
                m_tableFormat.Close();
                m_tableFormat = null;
            }

            if (m_charFormat != null)
            {
                m_charFormat.Close();
                m_charFormat = null;
            }
        }
        #endregion

        #region Implementation / xml
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.CellsItemTag, Cells);
            XDLSHolder.AddElement(XDLSConstants.CharacterFormatTag, CharacterFormat);
            XDLSHolder.AddElement(XDLSConstants.TableFormatTag, RowFormat);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlContent(Syncfusion.DocIO.DLS.XML.IXDLSContentWriter writer)
        {
            if (DataArray != null)
            {
                writer.WriteChildBinaryElement(XDLSConstants.InternalDataTag, DataArray);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override bool ReadXmlContent(Syncfusion.DocIO.DLS.XML.IXDLSContentReader reader)
        {
            bool retValue = base.ReadXmlContent(reader);

            if (reader.TagName == XDLSConstants.InternalDataTag)
            {
                DataArray = reader.ReadChildBinaryElement();
                retValue = true;
            }

            return retValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            //base.WriteXmlAttributes (writer);
            if (IsHeader)
            {
                writer.WriteValue(XDLSConstants.TableRowHeaderAttr, IsHeader);
            }

            if (m_tableFormat.HasSprms())
                return;

            if (Height > 0)
            {
                writer.WriteValue(XDLSConstants.TableRowHeigthAttr, Height);
            }
            writer.WriteValue(XDLSConstants.TableRowHeighTypeAttr, HeightType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            //base.ReadXmlAttributes (reader);
            if (reader.HasAttribute(XDLSConstants.TableRowHeigthAttr))
            {
                Height = reader.ReadFloat(XDLSConstants.TableRowHeigthAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TableRowHeaderAttr))
            {
                IsHeader = reader.ReadBoolean(XDLSConstants.TableRowHeaderAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TableRowHeighTypeAttr))
            {
                HeightType = (TableRowHeightType)reader.ReadEnum(XDLSConstants.TableRowHeighTypeAttr, typeof(TableRowHeightType));
            }
        }
//#endif
        #endregion

        #region Implementation / layout
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new TableLayoutInfo((HeightType == TableRowHeightType.Exactly
                && (Height >= 0 ? Height : -1 * Height) > 1),
                Height);
            m_layoutInfo.IsVerticalText = true;
            for (int i = 0; i < this.Cells.Count; i++)
            {
                if (this.Cells[i].CellFormat.TextDirection == TextDirection.Horizontal)
                {
                    m_layoutInfo.IsVerticalText = false;
                    break;
                }
            }
            if (!((this.OwnerTable.Owner is WTableCell) || (this.OwnerTable.TableFormat.WrapTextAround)))
                m_layoutInfo.IsKeepWithNext = IsKeepWithNext;
            DetermineTableFormat();
        }
        /// <summary>
        /// Get's whether the first paragraph of the row is having KeepWithNext property set
        /// </summary>
        internal bool IsKeepWithNext
        {
            get
            {
                WParagraph paragraph = null;
                GetFirstParagraphOfRow(ref paragraph);
                return (paragraph!=null && paragraph.ParagraphFormat.KeepFollow);
            }
        }
        /// <summary>
        /// Get First Paragraph of the Row
        /// </summary>
        /// <returns></returns>
        private void GetFirstParagraphOfRow(ref WParagraph paragraph)
        {
            if (this.Cells[0].ChildEntities.Count > 0)
            {
                if (this.Cells[0].ChildEntities[0] is WTable)
                {
                    (this.Cells[0].ChildEntities[0] as WTable).Rows[0].GetFirstParagraphOfRow(ref paragraph);
                }
                if (this.Cells[0].ChildEntities[0] is WParagraph)
                    paragraph = this.Cells[0].ChildEntities[0] as WParagraph;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            dc.DrawTableRow(this, ltWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
        /// <summary>
        /// Determines the table format.
        /// </summary>
        private void DetermineTableFormat()
        {
            float cellSpacing = RowFormat.CellSpacing / 4;
            float leftIndent = RowFormat.LeftIndent;

            if ( /*cellSpacing*/RowFormat.CellSpacing > -1)
            {
                Borders borders = RowFormat.Borders;
                float leftHalfWidth = borders.Left.LineWidth / 2;
                float topHalfWidth = borders.Top.LineWidth / 2;
                float rightHalfWidth = borders.Right.LineWidth / 2;
                float bottomHalfWidth = borders.Bottom.LineWidth / 2;

                Spacings paddings = m_layoutInfo.Paddings;
                paddings.Left = leftHalfWidth + cellSpacing;
                paddings.Top = topHalfWidth + cellSpacing;
                paddings.Right = rightHalfWidth + cellSpacing;
                paddings.Bottom = bottomHalfWidth + cellSpacing;

                Spacings margins = m_layoutInfo.Margins;
                margins.Left = leftHalfWidth;
                margins.Top = topHalfWidth;
                margins.Right = rightHalfWidth;
                margins.Bottom = bottomHalfWidth;
            }

            m_layoutInfo.Margins.Left += leftIndent;
        }
#endif
        #endregion
    }
}
