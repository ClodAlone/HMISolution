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

#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.Layouting;
using System.Drawing;
using Syncfusion.DocIO.Rendering;
#endif

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represent a Absolute Tab.
    /// </summary>
    internal class WAbsoluteTab : ParagraphItem
#if !SILVERLIGHT && !WP
, ILeafWidget
#endif
    {
        #region Fields
        private AbsoluteTabAlignment m_alignment;
        private AbsoluteTabRelation m_relation;
        private TabLeader m_tabLeader;
        private WCharacterFormat m_characterFormat;
# if !SILVERLIGHT && !WP
        internal HtmlToDocLayoutInfo m_htmlToDocLayoutInfo = new HtmlToDocLayoutInfo();
#endif
        #endregion

        #region Properties
# if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets Html to Doc layout info
        /// </summary>
        internal HtmlToDocLayoutInfo HtmlToDocLayoutInfo
        {
            get
            {
                return m_htmlToDocLayoutInfo;
            }
        }
#endif
        /// <summary>
        /// Gets/sets Character format.
        /// </summary>
        internal string Text
        {
            get
            {
                if (Alignment == AbsoluteTabAlignment.Left)
                    return '\v'.ToString();
                else
                    return '\t'.ToString();
            }
        }
        /// <summary>
        /// Gets the Absolute Tab position.
        /// </summary>
        /// <value>The Absolute Tab position.</value>
        internal float Position
        {
            get
            {
                return GetTabPostion();
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
                return EntityType.AbsoluteTab;
            }
        }
        /// <summary>
        /// Gets or Sets the Absolute Tab alignment.
        /// </summary>
        /// <value>The Absolute Tab alignment.</value>
        internal AbsoluteTabAlignment Alignment
        {
            get
            {
                return m_alignment;
            }
            set
            {
                m_alignment = value;
            }
        }
        /// <summary>
        /// Gets or Sets the Absolute Tab relation.
        /// </summary>
        /// <value>The Absolute Tab relation.</value>
        internal AbsoluteTabRelation Relation
        {
            get
            {
                return m_relation;
            }
            set
            {
                m_relation = value;
            }
        }
        /// <summary>
        /// Gets or Sets the Absolute Tab Leader.
        /// </summary>
        /// <value>The Absolute Tab Leader.</value>
        internal TabLeader TabLeader
        {
            get
            {
                return m_tabLeader;
            }
            set
            {
                m_tabLeader = value;
            }
        }
        /// <summary>
        /// Gets/sets Character format.
        /// </summary>
        internal WCharacterFormat CharacterFormat
        {
            get
            {
                if (m_characterFormat == null)
                    m_characterFormat = new WCharacterFormat(Document);
                return m_characterFormat;
            }
            set
            {
                m_characterFormat = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WAbsoluteTab"/> class.
        /// </summary>
        /// <param name="doc">Document</param>
        internal WAbsoluteTab(IWordDocument doc)
            : base((WordDocument)doc)
        {

        }
        #endregion

        #region Implementation
        /// <summary>
        /// Get the tab position.
        /// </summary>
        private float GetTabPostion()
        {
            if (m_relation == AbsoluteTabRelation.Margin)
                return GetTabPostionRelativeToMargin();
            else
                return GetTabPostionRelativeToIndent();
        }
        /// <summary>
        /// Get the tab position relative to margin.
        /// </summary>
        private float GetTabPostionRelativeToMargin()
        {
            float position = 0;
            WParagraph ownerParagraph = this.OwnerParagraph;
            if (ownerParagraph == null)
                ownerParagraph = this.GetOwnerParagraph();
            switch (m_alignment)
            {
                case AbsoluteTabAlignment.Right:
                    if (ownerParagraph.Owner.Owner is WSection)
                    {
                        WSection section = (ownerParagraph.Owner.Owner as WSection);
                        position = section.PageSetup.ClientWidth;
                    }
                    else if (ownerParagraph.Owner is WTableCell)
                    {
                        WTableCell cell = (ownerParagraph.Owner as WTableCell);
                        position = GetCellWidth(cell);
                    }
                    else if (ownerParagraph.Owner.Owner is WTextBox || ownerParagraph.Owner.Owner is Shape)
                    {
                        WSection section = (GetBaseEntity(this) as WSection);
                        position = section.PageSetup.ClientWidth;
                    }
                    break;
                case AbsoluteTabAlignment.Center:
                    if (ownerParagraph.Owner.Owner is WSection)
                    {
                        WSection section = (ownerParagraph.Owner.Owner as WSection);
                        position = section.PageSetup.ClientWidth / 2;
                    }
                    else if (ownerParagraph.Owner is WTableCell)
                    {
                        WTableCell cell = (ownerParagraph.Owner as WTableCell);
                        position = (GetCellWidth(cell) / 2);
                    }
                    else if (ownerParagraph.Owner.Owner is WTextBox || ownerParagraph.Owner.Owner is Shape)
                    {
                        WSection section = (GetBaseEntity(this) as WSection);
                        position = section.PageSetup.ClientWidth / 2;
                    }
                    break;
            }
            return position;
        }
        /// <summary>
        /// Get the tab position relative to indent.
        /// </summary>
        private float GetTabPostionRelativeToIndent()
        {
            float position = 0;
            WParagraph ownerParagraph = this.OwnerParagraph;
            if (ownerParagraph == null)
                ownerParagraph = this.GetOwnerParagraph();
            switch (m_alignment)
            {
                case AbsoluteTabAlignment.Right:
                    if (ownerParagraph.Owner.Owner is WSection)
                    {
                        WSection section = (ownerParagraph.Owner.Owner as WSection);
                        position = section.PageSetup.ClientWidth - ownerParagraph.ParagraphFormat.RightIndent;
                    }
                    else if (ownerParagraph.Owner is WTableCell)
                    {
                        WTableCell cell = (ownerParagraph.Owner as WTableCell);
                        position = GetCellWidth(cell) - ownerParagraph.ParagraphFormat.RightIndent;
                    }
                    else if (ownerParagraph.Owner.Owner is WTextBox || ownerParagraph.Owner.Owner is Shape)
                    {
                        WSection section = (GetBaseEntity(this) as WSection);
                        position = section.PageSetup.ClientWidth - ownerParagraph.ParagraphFormat.RightIndent;
                    }
                    break;
                case AbsoluteTabAlignment.Center:
                    if (ownerParagraph.Owner.Owner is WSection)
                    {
                        WSection section = (ownerParagraph.Owner.Owner as WSection);
                        position = (section.PageSetup.ClientWidth + ownerParagraph.ParagraphFormat.LeftIndent) / 2;
                    }
                    else if (ownerParagraph.Owner is WTableCell)
                    {
                        WTableCell cell = (ownerParagraph.Owner as WTableCell);
                        position = (GetCellWidth(cell) + ownerParagraph.ParagraphFormat.LeftIndent) / 2;
                    }
                    else if (ownerParagraph.Owner.Owner is WTextBox || ownerParagraph.Owner.Owner is Shape)
                    {
                        WSection section = (GetBaseEntity(this) as WSection);
                        position = (section.PageSetup.ClientWidth + ownerParagraph.ParagraphFormat.LeftIndent) / 2;
                    }
                    break;
            }
            return position;
        }
        /// <summary>
        /// Get Base Entity
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        private Entity GetBaseEntity(Entity ent)
        {
            while (!(ent is WSection))
            {
                if (ent.Owner == null)
                    break;
                else
                    ent = ent.Owner as Entity;
            }
            return ent;
        }
        /// <summary>
        /// Get cell width
        /// </summary>
        /// <param name="tableCell"></param>
        /// <returns>the cell width</returns>
        private float GetCellWidth(WTableCell tableCell)
        {            
            float cellSpacing = 0;
            if (tableCell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                cellSpacing = (float)Math.Round(tableCell.OwnerRow.OwnerTable.TableFormat.CellSpacing, 2) * 2;
            return tableCell.Width - GetLeftPadding(tableCell) - GetRightPadding(tableCell) - cellSpacing;
        }
        /// <summary>
        /// Get left padding of table cell
        /// </summary>
        /// <param name="tableCell"></param>
        /// <returns>the left padding</returns>
        private float GetLeftPadding(WTableCell tableCell)
        {
            if (tableCell.CellFormat.SamePaddingsAsTable)
            {
                if (tableCell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.LeftKey))
                {
                    return tableCell.OwnerRow.RowFormat.Paddings.Left;
                }
                else if (tableCell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.LeftKey))
                {
                    return tableCell.OwnerRow.OwnerTable.TableFormat.Paddings.Left;
                }
                else
                    return 5.4f;
            }
            else
               return tableCell.CellFormat.Paddings.Left;
        }
        /// <summary>
        /// Get right padding of table cell
        /// </summary>
        /// <param name="tableCell"></param>
        /// <returns>the right padding</returns>
        private float GetRightPadding(WTableCell tableCell)
        {
            if (tableCell.CellFormat.SamePaddingsAsTable)
            {   
                if (tableCell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
                {
                    return tableCell.OwnerRow.RowFormat.Paddings.Right;
                }
                else if (tableCell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
                {
                    return tableCell.OwnerRow.OwnerTable.TableFormat.Paddings.Right;
                }
                else
                    return 5.4f;
            }
            else
              return  tableCell.CellFormat.Paddings.Right;
        }
        /// <summary>
        /// Attaches to paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="itemPos">The item pos.</param>
        internal override void Attach(WParagraph paragraph, int itemPos)
        {
            base.Attach(paragraph, itemPos);
            if (paragraph.ParagraphFormat.AbsoluteTab == null)
                paragraph.ParagraphFormat.AbsoluteTab = this;
        }
        /// <summary>
        /// Detaches from owner.
        /// </summary>
        internal override void Detach()
        {
            WParagraph ownerPara = this.OwnerParagraph;
            if (ownerPara == null)
                ownerPara = this.GetOwnerParagraph();
            ownerPara.ParagraphFormat.AbsoluteTab = null;
            //Set next absolute tab as AbsoluteTab for OwnerParagraph
            foreach (Entity entity in ownerPara.ChildEntities)
            {
                if (entity is WAbsoluteTab)
                {
                    ownerPara.ParagraphFormat.AbsoluteTab = (entity as WAbsoluteTab);
                    break;
                }
            }
            base.Detach();
        }
        #endregion

        #region Implementation / xml
        #if !SILVERLIGHT && !WP
        /// <summary>
        /// Writes object data as xml attributes.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            //To Do
        }
        /// <summary>
        /// Reads object data from xml attributes.
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            //To Do
        }
        /// <summary>
        /// Registers child objects in XDSL holder.
        /// </summary>
        protected override void InitXDLSHolder()
        {
            //To Do
        }
        #endif
        #endregion

        #region Implementation / layout
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Create LayoutInfo
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new TabsLayoutInfo(ChildrenLayoutDirection.Horizontal);
            (m_layoutInfo as TabsLayoutInfo).AddTab(GetLayoutTabPostion(), (Layouting.TabJustification)Alignment, (Layouting.TabLeader)TabLeader);
        }
        /// <summary>
        /// Measures self size.
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns></returns>
        SizeF ILeafWidget.Measure(DrawingContext dc)
        {
            SizeF size = dc.MeasureString(" ", this.CharacterFormat.Font, null);
            size.Width = 0.0f;
            return size;
        }
        /// <summary>
        /// Get Absolute Tab position to layout in Doc to PDF conversion
        /// </summary>
        /// <returns></returns>
        private float GetLayoutTabPostion()
        {
            float position = 0;
            //Get Owner Paragraph
            WParagraph ownerParagraph = this.GetOwnerParagraph();
            //Get Base Entity
            Entity ent = GetBaseEntity(this.GetOwnerParagraph());
            switch (m_alignment)
            {
                case AbsoluteTabAlignment.Left:
                case AbsoluteTabAlignment.Right:
                    switch (Relation)
                    {
                        case AbsoluteTabRelation.Margin:
                            if (ownerParagraph.IsInCell)
                            {
                                position = GetLayoutCellWidth(ownerParagraph.OwnerTextBody as WTableCell);
                            }
                            else if (ent is WSection)
                            {
                                position = (ent as WSection).PageSetup.ClientWidth;
                            }
                            break;
                        case AbsoluteTabRelation.Indent:
                            if (ownerParagraph.IsInCell)
                            {
                                position = GetLayoutCellWidth(ownerParagraph.OwnerTextBody as WTableCell) - ownerParagraph.ParagraphFormat.RightIndent;
                            }
                            else if (ent is WSection)
                            {
                                position = (ent as WSection).PageSetup.ClientWidth - ownerParagraph.ParagraphFormat.RightIndent;
                            }
                            break;
                    }
                    return position;
                case AbsoluteTabAlignment.Center:
                    switch (Relation)
                    {
                        case AbsoluteTabRelation.Margin:
                            if (ownerParagraph.IsInCell)
                            {
                                position = (GetLayoutCellWidth(ownerParagraph.OwnerTextBody as WTableCell) / 2);
                            }
                            else if (ent is WSection)
                            {
                                position = (ent as WSection).PageSetup.ClientWidth / 2;
                            }
                            break;
                        case AbsoluteTabRelation.Indent:
                            if (ownerParagraph.IsInCell)
                            {
                                position = ((GetLayoutCellWidth(ownerParagraph.OwnerTextBody as WTableCell) + ownerParagraph.ParagraphFormat.LeftIndent) / 2);
                            }
                            else if (ent is WSection)
                            {
                                position = ((ent as WSection).PageSetup.ClientWidth + ownerParagraph.ParagraphFormat.LeftIndent) / 2;
                            }
                            break;
                    }
                    return position;
            }
            return position;
        }
        /// <summary>
        /// Get Table CellWidth
        /// </summary>
        /// <param name="tableCell"></param>
        /// <returns></returns>
        private float GetLayoutCellWidth(WTableCell tableCell)
        {
            float cellSpacing = 0;
            if (tableCell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                cellSpacing = (float)Math.Round(tableCell.OwnerRow.OwnerTable.TableFormat.CellSpacing, 2) * 2;
            ILayoutSpacingsInfo spacingInfo = (tableCell as IWidget).LayoutInfo as ILayoutSpacingsInfo;
            float leftPadding = (float)Math.Round((spacingInfo.Paddings.Left + spacingInfo.Margins.Left - cellSpacing), 2);
            float rightPadding = (float)Math.Round((spacingInfo.Paddings.Right + spacingInfo.Margins.Right - cellSpacing), 2);
            float cellWidth = (float)((tableCell as IWidget).LayoutInfo as TableLayoutInfo).CellWidth - leftPadding - rightPadding - cellSpacing;
            return cellWidth;
        }
        /// <summary>
        /// Draw AbsoluteTab
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            dc.DrawAbsoluteTab(this, ltWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
#endif
        #endregion
    }
}
