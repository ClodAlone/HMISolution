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

#if !SILVERLIGHT

using System;
using System.Drawing;
using System.Collections;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS.Rendering;

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Represents the layout context for tables.
    /// </summary>
    internal class LCTable : LayoutContext
    {
        #region Constants
        /// <summary>
        /// The minimum width.
        /// </summary>
        private const float DEF_MIN_WIDTH = 16f;
        #endregion

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private bool m_bHeaderRepeat = false;
        /// <summary>
        /// 
        /// </summary>
        private int m_currHeaderRowIndex = -1;
        /// <summary>
        /// 
        /// </summary>
        private int m_currRowIndex = -1;
        /// <summary>
        /// 
        /// </summary>
        private int m_currColIndex = -1;
        /// <summary>
        /// 
        /// </summary>
        private LayoutedWidget m_currRowLW;
        /// <summary>
        /// 
        /// </summary>
        private LayoutedWidget m_currCellLW;
        /// <summary>
        /// 
        /// </summary>
        private double[] m_rowsHeight;
        /// <summary>
        /// Handled for updating vertical merge start cell index.
        /// </summary>
        private int[] m_mergedRowIndex;
        /// <summary>
        /// 
        /// </summary>
        protected bool m_bAtLastOneCellFitted = false;
        /// <summary>
        /// 
        /// </summary>
        private SplitWidgetContainer[] m_splitedCells;
        /// <summary>
        /// 
        /// </summary>
        private LayoutState m_blastRowState = LayoutState.Unknown;
        /// <summary>
        /// 
        /// </summary>
        private SplitTableWidget m_spitTableWidget = null;
        /// <summary>
        /// 
        /// </summary>
        private WTable m_table = null;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isTableSplitted = false;
        /// <summary>
        /// 
        /// </summary>
        private int m_splittedTableRowIndex = 0;
        /// <summary>
        ///
        /// </summary>
        private bool m_isSplitTableCurrentCellMergeStart = false;
        /// <summary>
        /// 
        /// </summary>
        private float m_headerRowHeight;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the table layout info.
        /// </summary>
        /// <value>The table layout info.</value>
        public ITableLayoutInfo TableLayoutInfo
        {
            get
            {
                return TableWidget.TableLayoutInfo;
            }
        }

        /// <summary>
        /// Gets the table widget.
        /// </summary>
        /// <value>The table widget.</value>
        public ITableWidget TableWidget
        {
            get
            {
                return m_widget as ITableWidget;
            }
        }

        /// <summary>
        /// Gets the index of the curr row.
        /// </summary>
        /// <value>The index of the curr row.</value>
        public int CurrRowIndex
        {
            get
            {
                if (m_bHeaderRepeat)
                {
                    return m_currHeaderRowIndex;
                }
                return m_currRowIndex;
            }
        }
        /// <summary>
        /// Gets the left pad for table.
        /// </summary>
        /// <value>The left pad.</value>
        internal float LeftPad
        {
            get
            {
                float leftPad = (float)((m_table.Rows[0].Cells[0] as IWidget).LayoutInfo.Paddings.Left
                    + (m_table.Rows[0].Cells[0] as IWidget).LayoutInfo.Margins.Left);
                if (m_table.TableFormat.CellSpacing > 0)
                    leftPad += m_table.TableFormat.Borders.Left.LineWidth;
                return leftPad;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LCTable"/> class.
        /// </summary>
        /// <param name="splitWidget">The split widget.</param>
        /// <param name="lcOperator">The lc operator.</param>
        public LCTable(SplitTableWidget splitWidget, ILCOperator lcOperator)
            : base(splitWidget.TableWidget, lcOperator)
        {
            m_bHeaderRepeat = true;
            m_currRowIndex = splitWidget.StartRowNumber - 1;
            m_spitTableWidget = splitWidget;
            m_isTableSplitted = true;
            //m_currColIndex = splitWidget.StartColumnNumber - 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LCTable"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="lcOperator">The lc operator.</param>
        public LCTable(ITableWidget table, ILCOperator lcOperator)
            : base(table, lcOperator)
        {

        }
        #endregion

        #region Public methods
        /// <summary>
        /// Get the base entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Entity GetBaseEntity(Entity entity)
        {
            Entity baseEntity = entity;
            do
            {
                if (baseEntity.Owner == null)
                    return baseEntity;
                baseEntity = baseEntity.Owner;
            }
            while (!(baseEntity is WSection || baseEntity is HeaderFooter));

            return baseEntity;
        }
        /// <summary>
        /// Layouts the specified widget.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public override LayoutedWidget Layout(RectangleF rect)
        {
            m_table = m_widget as WTable;
            m_table.ApplyBaseStyleFormats();

            MarginsF pageMargins = InitializePageMargins();
            if (m_table.TableFormat.WrapTextAround)
            {
                //Get Client area height
                Entity ent = GetBaseEntity(m_table);
                float pageHeight = (m_lcOperator as Layouter).ClientLayoutArea.Height;
                if (ent is HeaderFooter)
                {
                    pageHeight = (ent.Owner as WSection).PageSetup.PageSize.Height;
                    rect.Height = pageHeight;
                }
                float currRectX = rect.X;
                float currRectY = rect.Y;
                //check whether the table is absoulte table and vertical position relative to the page or margin 
                if (m_table.Document.Settings.CompatibilityMode != CompatibilityMode.Word2013
                    && !(m_table.OwnerTextBody is WTableCell) 
                    && !m_table.m_isTextBox && !TableLayoutInfo.IsSplittedTable && m_table.TableFormat.WrapTextAround
                    && (m_table.TableFormat.Positioning.VertRelationTo == VerticalRelation.Margin
                    || m_table.TableFormat.Positioning.VertRelationTo == VerticalRelation.Page))
                {
                    //Extend the height of the table with including that vertical position
                    rect.Height += m_table.TableFormat.Positioning.VertPosition;
                }

                if (!TableLayoutInfo.IsSplittedTable)
                {
                    if ((m_table.OwnerTextBody is WTableCell))
                    {
                        if (rect.Height <= Math.Abs(m_table.Rows[0].Height) && (m_table.OwnerTextBody as WTableCell).OwnerRow.HeightType == TableRowHeightType.Exactly)
                        {
                            rect.Y = currRectY;
                        }
                        else if (m_table.TableFormat.Positioning.VertPositionAbs == VerticalPosition.None)
                        {
                            rect.Y += m_table.TableFormat.Positioning.VertPosition;
                        }
                        if (rect.Y < ((m_table.OwnerTextBody as WTableCell).m_layoutInfo as TableLayoutInfo).TableCellTopMargin
                            && !m_table.m_isTextBox)
                            rect.Y = ((m_table.OwnerTextBody as WTableCell).m_layoutInfo as TableLayoutInfo).TableCellTopMargin;
                    }
                    else if (m_table.TableFormat.Positioning.VertRelationTo == Syncfusion.DocIO.VerticalRelation.Paragraph)
                    {
                        if (m_table.m_isTextBox)
                        {
                            WParagraph ownerPara = m_table.m_textBoxFormat.OwnerBase.OwnerBase as WParagraph;
                            if (m_table.m_textBoxFormat.VerticalOrigin == VerticalOrigin.Line && !((ownerPara as IWidget).LayoutInfo as ParagraphLayoutInfo).IsFirstLine)
                                rect.Y += m_table.TableFormat.Positioning.VertPosition;
                            else
                            {
                                float topMargin = 0;
                                if (ownerPara.IsInCell && !m_table.m_textBoxFormat.AllowInCell)
                                {
                                    topMargin = (float)(((ownerPara.OwnerTextBody as WTableCell).m_layoutInfo as TableLayoutInfo).TableCellTopMargin
                                                 - (ownerPara.OwnerTextBody as WTableCell).m_layoutInfo.Margins.Top
                                                 - (ownerPara.OwnerTextBody as WTableCell).m_layoutInfo.Paddings.Top);
                                    rect.Y = topMargin + m_table.TableFormat.Positioning.VertPosition;
                                }
                                else
                                    rect.Y = ((ownerPara as IWidget).LayoutInfo as ParagraphLayoutInfo).YPosition + m_table.TableFormat.Positioning.VertPosition;
                            }
                        }
                        else if (m_table.TableFormat.Positioning.VertPositionAbs == VerticalPosition.None)
                            rect.Y += m_table.TableFormat.Positioning.VertPosition - ((m_table.Document.Settings.CompatibilityMode == CompatibilityMode.Word2013 || (m_lcOperator as Layouter).WrappingDifference == float.MinValue) ? 0 : (m_lcOperator as Layouter).WrappingDifference);
                    }
                    else if (!m_table.m_isTextBox && m_table.TableFormat.Positioning.VertPositionAbs != VerticalPosition.None)
                    {
                        if (m_table.TableFormat.Positioning.VertRelationTo == Syncfusion.DocIO.VerticalRelation.Page)
                            rect.Y = 0;
                        else if ((m_table.TableFormat.Positioning.VertPositionAbs == VerticalPosition.Top
                            || m_table.TableFormat.Positioning.VertPositionAbs == VerticalPosition.Inside)
                            && m_table.TableFormat.Positioning.VertRelationTo == Syncfusion.DocIO.VerticalRelation.Margin
                            && pageMargins != null)
                        {
                            rect.Y = pageMargins.Top;
                        }
                    }
                    else if (m_table.TableFormat.Positioning.VertPosition != 0.0f)
                    {
                        if (!m_table.m_isTextBox
                            && !(m_table.OwnerTextBody is WTableCell)
                            && m_table.TableFormat.Positioning.VertRelationTo == Syncfusion.DocIO.VerticalRelation.Margin
                            && pageMargins != null)
                            rect.Y = pageMargins.Top + m_table.TableFormat.Positioning.VertPosition;
                        else
                            rect.Y = m_table.TableFormat.Positioning.VertPosition;
                    }
                    if (rect.Y < 0 && !(m_lcOperator as Layouter).IsLayoutingHeaderFooter && !m_table.m_isTextBox)
                        rect.Y = currRectY;
                    rect.Height = rect.Height + (currRectY - rect.Y);

                    if (m_table.m_isTextBox && m_table.m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.Inline && rect.Height < m_table.Rows[0].Height)
                    {
                        rect.Height = m_table.Rows[0].Height + m_table.TableFormat.Paddings.Top + m_table.TableFormat.Paddings.Bottom;
                    }
                    if (!m_table.m_isTextBox
                        && m_table.TableFormat.Positioning.VertPositionAbs != VerticalPosition.None
                        && m_table.TableFormat.Positioning.VertRelationTo == Syncfusion.DocIO.VerticalRelation.Page
                        && !(m_table.OwnerTextBody is WTableCell))
                        rect.Height = pageHeight;
                }
                if (m_table.m_isTextBox && m_table.m_textBoxFormat.HorizontalOrigin == HorizontalOrigin.Character)
                    rect.X = m_table.TableFormat.Positioning.HorizPosition;
                else if (m_table.m_isTextBox && m_table.m_textBoxFormat.HorizontalOrigin == HorizontalOrigin.Column)
                {
                    if (!(m_table.m_textBoxFormat.OwnerBase.OwnerBase as WParagraph).IsXpositionUpated || !m_table.m_textBoxFormat.IsWrappingBoundsAdded)
                    {
                        WParagraph ownerPara = m_table.m_textBoxFormat.OwnerBase.OwnerBase as WParagraph;//Update the X position while horrizontal alighnment is column
                        rect.X = ((ownerPara as IWidget).LayoutInfo as ParagraphLayoutInfo).XPosition + m_table.TableFormat.Positioning.HorizPosition;
                    }
                    else
                        rect.X = (m_lcOperator as Layouter).ClientLayoutArea.X + m_table.TableFormat.Positioning.HorizPosition;
                }
                else if (m_table.TableFormat.Positioning.HorizRelationTo == Syncfusion.DocIO.HorizontalRelation.Column
                    && m_table.TableFormat.Positioning.HorizPositionAbs == HorizontalPosition.Left)
                    rect.X += m_table.TableFormat.Positioning.HorizPosition;
                if (((m_table.TableFormat.Positioning.HorizPositionAbs == HorizontalPosition.Left && !m_table.m_isTextBox)
                    || (m_table.m_isTextBox && m_table.TableFormat.Positioning.HorizPosition != 0))
                    && m_table.TableFormat.Positioning.HorizRelationTo != Syncfusion.DocIO.HorizontalRelation.Column)
                {
                    if (m_table.m_isTextBoxInTable)
                        rect.X += m_table.TableFormat.Positioning.HorizPosition;
                    else if (m_table.OwnerTextBody is WTableCell)
                    {
                        if (m_table.Width > rect.Width)
                            rect.X = currRectX;
                        else
                            rect.X += m_table.TableFormat.Positioning.HorizPosition;
                    }
                    else if (m_table.TableFormat.Positioning.HorizRelationTo == HorizontalRelation.Margin && pageMargins != null)
                        rect.X = pageMargins.Left + m_table.TableFormat.Positioning.HorizPosition;
                    else
                        rect.X = m_table.TableFormat.Positioning.HorizPosition;
                }
            }
            CreateTableClientArea(ref rect);
            if ((m_table.OwnerTextBody is WTableCell))
            {
                float xPosition = ((m_table.OwnerTextBody as WTableCell).m_layoutInfo as TableLayoutInfo).TableCellLeftMargin - (float)(m_table.OwnerTextBody as WTableCell).m_layoutInfo.Paddings.Left;
                if (rect.X < xPosition && !m_table.m_isTextBox)
                    rect.X = xPosition;
                CreateLayoutArea(rect);
            }
            //Update Layout area based on text wrap
            Font font = (m_table.Rows[0].Cells[0] as WTextBody).LastParagraph != null ? (m_table.Rows[0].Cells[0] as WTextBody).LastParagraph.BreakCharacterFormat.Font : m_table.Rows[0].Cells[0].CharacterFormat.Font;
            SizeF size = this.DrawingContext.MeasureString(" ", font, null);
            size.Width = m_table.Width;
            AdjustClientAreaBasedOnTextWrap(size, ref rect);

            CreateLayoutedWidget(rect.Location);
            m_rowsHeight = new double[m_table.Rows[TableWidget.MaxRowIndex].Cells.Count];
            m_mergedRowIndex = new int[m_table.Rows[TableWidget.MaxRowIndex].Cells.Count];

            do
            {
                if (!CreateRowLayoutedWidget())
                {
                    if (m_bAtLastOneCellFitted)
                    {
                        m_ltState = LayoutState.Fitted;
                    }
                    break;
                }

                DoLayoutRow();
                CommitRow();
            }
            while (m_ltState == LayoutState.Unknown);

            //Update Vertical Merge cell widget with text direction as vertical
            UpdateVerticalMergeCellWidget();

            DeleteContinuousCells();

#if DEBUG_LAYOUTING
      /* Debug code */ DBG_CommitChildContext( this );
#endif
            UpdateTableLWBounds();
            return m_ltWidget;
        }
        /// <summary>
        /// Update Vertical Merge cell widget with text direction as vertical
        /// </summary>
        private void UpdateVerticalMergeCellWidget()
        {
            for (int i = 0, cnt = m_ltWidget.ChildWidgets.Count; i < cnt; i++)
            {
                LayoutedWidget lw = m_ltWidget.ChildWidgets[i];
                TableLayoutInfo rowTableInfo = lw.Widget.LayoutInfo as TableLayoutInfo;
                if (!rowTableInfo.IsVerticalText)
                {
                    for (int j = 0; j < lw.ChildWidgets.Count; j++)
                    {
                        LayoutedWidget cellLW = lw.ChildWidgets[j];
                        if ((cellLW.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart)
                        {
                            int rowMergeStartIndex = i;
                            int columnIndex = j;
                            LayoutInfo layoutInfo = m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex].Widget.LayoutInfo as LayoutInfo;
                            if (layoutInfo.IsVerticalText)
                            {
                                int rowIndex = (m_ltWidget.ChildWidgets[rowMergeStartIndex].Widget as WTableRow).GetRowIndex();
                                //Get the cell widget with text direction as vertical
                                LayoutedWidget cellLtWidget = GetCellWidget(m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex].Bounds.Height, rowIndex, columnIndex);
                                if (m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex].ChildWidgets.Count > 0)
                                {
                                    m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex].ChildWidgets[0] = cellLtWidget;
                                }
                                else
                                {
                                    m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex].ChildWidgets.Add(cellLtWidget);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Get Cell Widget with text direction as vertical
        /// </summary>
        /// <param name="rowHeight"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        private LayoutedWidget GetCellWidget(float rowHeight, int rowIndex, int cellIndex)
        {
            TableLayoutInfo tableLayoutInfo = (TableWidget.GetCellWidget(rowIndex, cellIndex)).LayoutInfo as TableLayoutInfo;
            RectangleF cellBounds = new RectangleF();
            cellBounds.X = (float)(tableLayoutInfo.TableCellLeftMargin - tableLayoutInfo.Margins.Left - tableLayoutInfo.Paddings.Left);
            cellBounds.Y = (float)(tableLayoutInfo.TableCellTopMargin - tableLayoutInfo.Margins.Top - tableLayoutInfo.Paddings.Top);
            cellBounds.Width = rowHeight;
            cellBounds.Height = GetCellClientArea((tableLayoutInfo != null) ? tableLayoutInfo.IsColumnMergeStart : false, rowIndex, cellIndex).ClientArea.Height;
            tableLayoutInfo.VerticalCellWidth = cellBounds.Width;
            //Layout the cell widget with text direction as vertical
            LayoutedWidget ltWidget = new LayoutedWidget(TableWidget.GetCellWidget(rowIndex, cellIndex));
            LayoutContext lc = LayoutContext.Create(ltWidget.Widget, m_lcOperator, (float)m_layoutArea.Width);
            LayoutedWidget cellLtWidget = lc.Layout(cellBounds);
            cellLtWidget.TopMargin = tableLayoutInfo.TableCellTopMargin;
            tableLayoutInfo.CellHeight = cellLtWidget.Bounds.Height;
            return cellLtWidget;
        }
        /// <summary>
        /// Updates the table LW bounds.
        /// </summary>
        private void UpdateTableLWBounds()
        {
            RectangleF bounds = m_ltWidget.Bounds;
            float cellSpacing = 0, left = 0, right = 0, top = 0, bottom = 0;
            if (m_table.TableFormat.CellSpacing > 0)
            {
                cellSpacing = m_table.TableFormat.CellSpacing * 2;
                left = m_table.TableFormat.Borders.Left.LineWidth / 2;
                right = m_table.TableFormat.Borders.Right.LineWidth / 2;
                top = m_table.TableFormat.Borders.Top.LineWidth / 2;
            }
            bounds.X -= left;
            bounds.Y -= top;
            bounds.Width += (float)(cellSpacing + left + right);
            bounds.Height += (float)(cellSpacing + top + GetMaxBottomMargin(cellSpacing));
            //Update KeepWithNext property set for the Table
            UpdateTableKeepWithNext();
            m_ltWidget.Bounds = bounds;
            if (!TableLayoutInfo.IsSplittedTable && m_table.TableFormat.WrapTextAround)
                UpdateAbsoluteTablePosition();
            m_ltWidget.Widget.LayoutInfo.IsFirstItemInPage = false;
            if (m_ltWidget.Bounds.Y == (m_lcOperator as Layouter).PageTopMargin)
                m_ltWidget.Widget.LayoutInfo.IsFirstItemInPage = true;
            if (CurrRowIndex == m_table.Rows.Count - 1 && DocumentLayouter.IsFirstLayouting)
                TableLayoutInfo.IsSplittedTable = false;
        }
        /// <summary>
        /// Get Max Bottom Margin of the last row
        /// </summary>
        /// <returns></returns>
        private float GetMaxBottomMargin(float cellSpacing)
        {
            float maxBottomMargin = 0;
            if (!m_table.m_isTextBox)
            {
                for (int i = 0; i < m_currRowLW.ChildWidgets.Count; i++)
                {
                    if (m_currRowLW.ChildWidgets[i].Widget.LayoutInfo.Margins.Bottom > maxBottomMargin
                        && !(m_currRowLW.ChildWidgets[i].Widget.LayoutInfo as WTableCell.LayoutCellInfo).SkipBottomBorder)
                        maxBottomMargin = (float)m_currRowLW.ChildWidgets[i].Widget.LayoutInfo.Margins.Bottom - cellSpacing;
                }
            }
            return maxBottomMargin;
        }
        /// <summary>
        /// Update KeepWithNext property set for the Table
        /// </summary>
        private void UpdateTableKeepWithNext()
        {
            if (!(m_table.TableFormat.WrapTextAround || m_table.OwnerTextBody is WTableCell) && m_ltWidget.ChildWidgets.Count > 0)
            {
                m_ltWidget.Widget.LayoutInfo.IsKeepWithNext = true;
                for (int i = 0; i < m_ltWidget.ChildWidgets.Count; i++)
                {
                    if (!m_ltWidget.ChildWidgets[i].Widget.LayoutInfo.IsKeepWithNext)
                    {
                        m_ltWidget.Widget.LayoutInfo.IsKeepWithNext = false;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Update absolute table position
        /// </summary>
        private void UpdateAbsoluteTablePosition()
        {
            float xOffSet = 0;
            float yOffSet = 0;
            MarginsF pageMargins = InitializePageMargins();
            float totalHeight = 0;
            if (pageMargins != null)
            {
                Entity ent = GetBaseEntity(m_table);
                if (ent is HeaderFooter)
                    totalHeight = (ent.Owner as WSection).PageSetup.PageSize.Height;
                else
                    totalHeight = (m_lcOperator as Layouter).ClientLayoutArea.Height + pageMargins.Top + pageMargins.Bottom;
            }
            if (m_ltWidget.Bounds.Height < (m_lcOperator as Layouter).ClientLayoutArea.Height
                && m_table.TableFormat.Positioning.VertRelationTo != VerticalRelation.Page)
                yOffSet = (m_lcOperator as Layouter).ClientLayoutArea.Height - m_ltWidget.Bounds.Height;
            else if (m_ltWidget.Bounds.Height < totalHeight)
                yOffSet = totalHeight - m_ltWidget.Bounds.Height;
            if (!m_table.m_isTextBox
                && !(m_table.OwnerTextBody is WTableCell)
                && yOffSet != 0)
            {
                Entity ent = GetBaseEntity(m_table);//Get the base entity for the current table
                if (m_table.TableFormat.Positioning.VertPositionAbs == VerticalPosition.Bottom
                    || m_table.TableFormat.Positioning.VertPositionAbs == VerticalPosition.Outside)
                    m_ltWidget.ShiftLocation(xOffSet, yOffSet, false);
                else if (m_table.TableFormat.Positioning.VertPositionAbs == VerticalPosition.Center)
                    m_ltWidget.ShiftLocation(xOffSet, yOffSet / 2, false);
            }
        }

        private MarginsF InitializePageMargins()
        {
            IWSection sec = null;
            MarginsF pageMargins = null;
            IEntity ent = m_table.Owner;
            if (ent != null)
            {
                while (ent.EntityType != EntityType.Section)
                {
                    if (ent.Owner != null)
                        ent = ent.Owner;
                    else
                        break;
                }
                if (ent.EntityType == EntityType.Section)
                {
                    sec = ent as WSection;
                    if (sec != null)
                        pageMargins = sec.PageSetup.Margins;
                }
            }
            return pageMargins;
        }
        /// <summary>
        /// Adjust Client Area for Text Wrapping
        /// </summary>
        /// <param name="size">Size</param>
        /// <param name="rect">The rect</param>
        private void AdjustClientAreaBasedOnTextWrap(SizeF size, ref RectangleF rect)
        {
            //Get the first row width
            float firstRowWidth = GetFirstRowWidth();
            #region textwrap
            //Update Layout area based on text wrap and ignore the yposition update while 
            //wrapping bounds already added to the collection
            if (!(m_lcOperator as Layouter).IsLayoutingHeaderFooter
                && !(m_table.m_isTextBox
                && (m_table.m_textBoxFormat.TextWrappingStyle == TextWrappingStyle.InFrontOfText
                || m_table.m_textBoxFormat.TextWrappingStyle == TextWrappingStyle.Behind)))
            {
                RectangleF clientLayoutArea = (m_lcOperator as Layouter).ClientLayoutArea;
                int wrapItemIndex = GetFloattingItemIndex(GetBaseEntity(m_table));
                for (int i = 0; i < (m_lcOperator as Layouter).FloatingItems.Count; i++)
                {
                    RectangleF textWrappingBounds = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingBounds;
                    TextWrappingStyle textWrappingStyle = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingStyle;
                    bool allowOverlap = (m_lcOperator as Layouter).FloatingItems[i].AllowOverlap;
                    if (!(clientLayoutArea.X > textWrappingBounds.Right + DEF_MIN_WIDTH || clientLayoutArea.Right < textWrappingBounds.X - DEF_MIN_WIDTH))
                    {
                        if ((m_lcOperator as Layouter).FloatingItems.Count > 0
                            && wrapItemIndex != i
                            && Math.Round(rect.Y + size.Height, 2) >= Math.Round(textWrappingBounds.Y, 2)
                            && Math.Round(rect.Y, 2) < Math.Round(textWrappingBounds.Bottom, 2)
                            && textWrappingStyle != TextWrappingStyle.Inline
                            && textWrappingStyle != TextWrappingStyle.TopAndBottom
                            && textWrappingStyle != TextWrappingStyle.InFrontOfText
                            && textWrappingStyle != TextWrappingStyle.Behind
                            && !(allowOverlap && (m_table.m_isTextBox && m_table.m_textBoxFormat.TextWrappingStyle !=TextWrappingStyle.Inline && m_table.m_textBoxFormat.AllowOverlap)))
                        {
                            if (rect.X >= textWrappingBounds.X && rect.X < textWrappingBounds.Right)
                            {
                                rect.Width = rect.Width - (textWrappingBounds.Right - rect.X);
                                //checks minimum width
                                if (rect.Width < DEF_MIN_WIDTH || (rect.Width < firstRowWidth && firstRowWidth > 0))
                                {
                                    rect.Width = m_layoutArea.ClientActiveArea.Right - textWrappingBounds.Right;
                                    if (rect.Width < DEF_MIN_WIDTH || (rect.Width < firstRowWidth && firstRowWidth > 0))
                                    {
                                        rect.Y = textWrappingBounds.Bottom;
                                        rect.Width = m_layoutArea.ClientArea.Width;
                                        rect.Height = rect.Height - textWrappingBounds.Height;
                                        CreateLayoutArea(rect);
                                    }
                                    else
                                    {
                                        rect.X = textWrappingBounds.Right;
                                        CreateLayoutArea(rect);
                                    }
                                }
                                else
                                {
                                    rect.X = textWrappingBounds.Right;
                                    CreateLayoutArea(rect);
                                }
                            }
                            else if ((rect.Right - textWrappingBounds.Right) > 0
                                && (rect.Right - textWrappingBounds.Right) < rect.Width
                                && (rect.Y >= textWrappingBounds.Y
                                || (rect.Y + size.Height) >= textWrappingBounds.Y))
                            {
                                rect.Y = textWrappingBounds.Bottom;
                                rect.Height = rect.Height - textWrappingBounds.Height;
                                CreateLayoutArea(rect);
                            }
                            else if (textWrappingBounds.X > rect.X && rect.Right > textWrappingBounds.X)
                            {
                                rect.Width = textWrappingBounds.X - rect.X;
                                //checks minimum width
                                if (rect.Width < DEF_MIN_WIDTH || (rect.Width < firstRowWidth && firstRowWidth > 0))
                                {
                                    rect.Width = m_layoutArea.ClientActiveArea.Right - textWrappingBounds.Right;
                                    if (rect.Width < DEF_MIN_WIDTH || (rect.Width < firstRowWidth && firstRowWidth > 0))
                                    {
                                        if (m_layoutArea.ClientArea.Right < (m_lcOperator as Layouter).ClientLayoutArea.Right
                                            && textWrappingBounds.Right < (m_lcOperator as Layouter).ClientLayoutArea.Right)
                                        {
                                            rect.Width = (m_lcOperator as Layouter).ClientLayoutArea.Right - textWrappingBounds.Right;
                                            rect.X = textWrappingBounds.Right;
                                        }
                                        else
                                        {
                                            rect.Y = textWrappingBounds.Bottom;
                                            rect.Height = rect.Height - textWrappingBounds.Height;
                                        }
                                        CreateLayoutArea(rect);
                                    }
                                }
                                else
                                    CreateLayoutArea(rect);
                            }
                            else if (rect.X > textWrappingBounds.X && rect.X > textWrappingBounds.Right)
                            {
                                rect.Width = m_layoutArea.ClientArea.Width;
                                CreateLayoutArea(rect);
                            }
                            else if (rect.X > textWrappingBounds.X && rect.X < textWrappingBounds.Right)
                            {
                                rect.Width = rect.Width - (textWrappingBounds.Right - rect.X);
                                rect.X = textWrappingBounds.Right;
                                CreateLayoutArea(rect);
                            }
                        }
                        else if ((m_lcOperator as Layouter).FloatingItems.Count > 0
                            && wrapItemIndex != i
                            && ((rect.Y >= textWrappingBounds.Y
                            && rect.Y < textWrappingBounds.Bottom)
                            || ((rect.Y + GetMaxCellHeight(size.Height)) >= textWrappingBounds.Y
                            && (rect.Y + size.Height) < textWrappingBounds.Bottom))
                            && textWrappingStyle == TextWrappingStyle.TopAndBottom
                            && !(allowOverlap && (m_table.m_isTextBox && m_table.m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.Inline && m_table.m_textBoxFormat.AllowOverlap)))
                        {
                            rect.Y = textWrappingBounds.Bottom;
                            rect.Height = rect.Height - textWrappingBounds.Height;
                            CreateLayoutArea(rect);
                        }
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// Get the First row width
        /// </summary>
        /// <returns></returns>
        private float GetFirstRowWidth()
        {
            float firstRowWidth = 0;
            int rowIndex = CurrRowIndex == -1 ? 0 : CurrRowIndex;
            if (m_table.Rows.Count > rowIndex)
            {
                for (int i = 0; i < m_table.Rows[rowIndex].Cells.Count; i++)
                    firstRowWidth += GetCellWidth(rowIndex, i);
            }
            return firstRowWidth;
        }
        #endregion



        /// <summary>
        /// Get the First row maximum cell width
        /// </summary>
        /// <returns></returns>
        private float GetMaxCellHeight(float cellMinHeight)
        {
            float firstMaxCellHeight = 0;
            for (int i = 0; i < m_table.Rows[0].Cells.Count; i++)
                firstMaxCellHeight = Math.Max(firstMaxCellHeight, GetCellHeight(0, i, cellMinHeight));
            return firstMaxCellHeight;
        }


        #region Implementation ( row )
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CreateRowLayoutedWidget()
        {
            if (CurrRowIndex + 1 < TableWidget.RowsCount)
            {
                m_currColIndex = -1;
                NextRowIndex();
                m_currRowLW = new LayoutedWidget(
                  TableWidget.GetRowWidget(CurrRowIndex)
                  );

                m_currRowLW.Bounds = new RectangleF(m_layoutArea.ClientActiveArea.Location, new SizeF());
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void DoLayoutRow()
        {
#if DEBUG_LAYOUTING
      DBG_WriteSpec( m_currRowIndex.ToString(), "NEW ROW" );
#endif
            if (!m_table.IsHiddenRow(CurrRowIndex, m_table))
            {
                m_splitedCells = new SplitWidgetContainer[m_table.Rows[CurrRowIndex].Cells.Count];
                TableLayoutInfo tableInfo;
                WTableCell cell = null;
                ArrayList al = new ArrayList();
                do
                {
                    // Creates next child context 
                    LayoutContext childContext = CreateNextCellContext();
                    LayoutArea cellArea = null;
                    if (childContext == null)
                    {
                        break;
                    }

                    tableInfo = childContext.Widget.LayoutInfo as TableLayoutInfo;
                    cell = m_table.Rows[CurrRowIndex].Cells[m_currColIndex] as WTableCell;
                    cellArea = GetCellClientArea((tableInfo != null) ? tableInfo.IsColumnMergeStart : false, CurrRowIndex, m_currColIndex);
                    //Sets the client layout area right margin
                    childContext.ClientLayoutAreaRight = (float)cellArea.Width;

                    if (cellArea.Height > m_layoutArea.ClientActiveArea.Height)
                    {
                        // To preserve top border, if the row is not fitted and moved to next page.
                        (childContext.Widget.LayoutInfo as WTableCell.LayoutCellInfo).SkipTopBorder = false;
                        m_ltState = LayoutState.NotFitted;
                        //To apply KeepWithNext funtionality, if the row is not fitted and moved to next page
                        CommitKeepWithNext();
                    }
                    else
                    {
#if DEBUG_LAYOUTING
          DBG_WriteSpec( m_currRowIndex.ToString() + " / " + m_currColIndex.ToString(), "ROW / CELL" );
#endif
                        DoLayoutCell(childContext, cellArea.ClientArea); //??
                        SaveChildContextState(childContext);
                    }
                }
                while (State == LayoutState.Unknown);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void CommitRow()
        {
            bool isRowBreak = true;
            if (m_ltState == LayoutState.Unknown && m_bAtLastOneCellFitted)
            {
                UpdateRowSize();

                //Retreive first paragraph persent in first cell of the current row
                IWidget widget = GetChildParaWidget(m_currRowLW);
                //check whether the paragraph has pagebreakbefore property set and
                //verify whether it is the first item in the page
                //if it is not the first item then split the table row to next page
                if (widget != null && (widget is WParagraph)
                    && (widget as WParagraph).ParagraphFormat.PageBreakBefore
                    && (Math.Round((m_currRowLW.Bounds.Y - m_headerRowHeight), 2) != Math.Round((m_lcOperator as Layouter).ClientLayoutArea.Y, 2)))
                {
                    //split the row into next page
                    m_sptWidget = new SplitTableWidget(TableWidget, CurrRowIndex + 1);
                    m_ltState = LayoutState.Splitted;
                    return;
                }

                for (int i = 0; i < m_table.Rows[CurrRowIndex].Cells.Count; i++)
                {
                    // Add Splitted cells into the SplittedWidget collection
                    if ((m_table.Rows[CurrRowIndex].Cells[i].m_layoutInfo as TableLayoutInfo).IsRowMergeStart && !m_splittedWidget.ContainsKey(m_ltWidget.ChildWidgets.Count)
                        && m_splitedCells[i] != null && (m_splitedCells[i] as SplitWidgetContainer).m_currentChild != null
                        && (m_currRowLW.ChildWidgets[i].Widget.LayoutInfo as TableLayoutInfo).IsRowSplitted 
                        && m_currRowLW.ChildWidgets[i].ChildWidgets[0].TextTag == "Splitted")
                        m_splittedWidget.Add(m_ltWidget.ChildWidgets.Count, m_splitedCells);
                    UpdateCellSize(i);
                }

                if (m_blastRowState == LayoutState.Splitted)
                {
                    if (IsRowNeedToBeSplitted())
                    {
                        if (!(!(m_currRowLW.Widget as WTableRow).RowFormat.IsBreakAcrossPages
                            && Math.Round(m_currRowLW.Bounds.Y - m_headerRowHeight, 2) == Math.Round((m_lcOperator as Layouter).PageTopMargin, 2)))
                        {
                            //Update Splitted VerticalMergeStart cell
                            UpdateSplittedVerticalMergeStartCell();
                            m_sptWidget = new SplitTableWidget(TableWidget, CurrRowIndex + 1, m_splitedCells);
                            m_ltState = LayoutState.Splitted;
                        }
                    }
                    else
                    {
                        isRowBreak = false;
                        if (m_ltWidget.ChildWidgets.Count > 0)
                        {
                            m_sptWidget = new SplitTableWidget(TableWidget, CurrRowIndex + 1);
                            m_ltState = LayoutState.Splitted;
                        }
                        else
                            m_ltState = LayoutState.NotFitted;
                    }
                }
                if (isRowBreak)
                {
                    //Update Row Width
                    UpdateRowWidth();
                    // Adds row to table
                    m_ltWidget.ChildWidgets.Add(m_currRowLW);
                    m_currRowLW.Owner = m_ltWidget;
                    UpdateLWBounds();
                    UpdateClientArea();
                }
            }
            else if (m_ltWidget.ChildWidgets.Count > 0)
            {
                //Update Splitted VerticalMergeStart cell
                UpdateSplittedVerticalMergeStartCell();
                if (IsNotEmptySplittedCell())
                {
                    //Update Splitted cells
                    UpdateSplittedCell();
                    m_sptWidget = new SplitTableWidget(TableWidget, CurrRowIndex + 1, m_splitedCells);
                    m_ltState = LayoutState.Splitted;
                }
                else
                {
                    m_sptWidget = new SplitTableWidget(TableWidget, CurrRowIndex + 1);
                    m_ltState = LayoutState.Splitted;
                }
                TableLayoutInfo.IsSplittedTable = true;
            }

        }
        /// <summary>
        /// Retreive first pagragraph present in the first cell of the current row
        /// </summary>
        /// <returns>paragraph widget of first cell</returns>
        private IWidget GetChildParaWidget(LayoutedWidget layoutedWidget)
        {
            if (layoutedWidget.ChildWidgets != null && layoutedWidget.ChildWidgets.Count > 0)
            {
                if ((layoutedWidget.ChildWidgets[0].Widget is WTableCell) ||
                    (layoutedWidget.ChildWidgets[0].Widget is WTableRow) ||
                    (layoutedWidget.ChildWidgets[0].Widget is WTable))
                {
                    return GetChildParaWidget(layoutedWidget.ChildWidgets[0]);
                }
                else if (layoutedWidget.ChildWidgets[0].Widget is WParagraph)
                {
                    return layoutedWidget.ChildWidgets[0].Widget;
                }
            }

            return null;

        }
        /// <summary>
        /// Determine whether the splitted cells are not empty
        /// </summary>
        /// <returns></returns>
        private bool IsNotEmptySplittedCell()
        {
            for (int i = 0; i < m_splitedCells.Length; i++)
            {
                if (m_splitedCells[i] != null && (m_splitedCells[i] as SplitWidgetContainer).m_currentChild != null)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Update Splitted VerticalMergeStart cell
        /// </summary>
        private void UpdateSplittedVerticalMergeStartCell()
        {
            if (m_ltWidget.ChildWidgets.Count > 0)
            {
                LayoutedWidget lw = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1];

                for (int j = lw.ChildWidgets.Count - 1; j > -1; j--)
                {
                    LayoutedWidget cellLW = lw.ChildWidgets[j];
                    TableLayoutInfo tableInfo = cellLW.Widget.LayoutInfo as TableLayoutInfo;
                    bool colMergeCont = tableInfo.IsColumnMergeContinue;
                    if (tableInfo.IsRowMergeContinue || tableInfo.IsRowMergeStart)
                    {
                        int rowMergeStartIndex = m_ltWidget.ChildWidgets.Count - 1;
                        int columnIndex = j;
                        if (tableInfo.IsRowMergeStart || SearchedMergeStart(ref rowMergeStartIndex, ref columnIndex))
                        {
                            //Update Vertical merge splitted cell
                            if (m_splittedWidget.ContainsKey(rowMergeStartIndex)
                                && columnIndex < lw.ChildWidgets.Count
                                && (m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex].Widget.LayoutInfo as TableLayoutInfo).IsRowSplitted)
                            {
                                if (m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex].ChildWidgets[0].Bounds.Bottom > m_currRowLW.Bounds.Bottom)
                                    m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex].ChildWidgets[0] = UpdateSplittedLtWidget(rowMergeStartIndex, columnIndex);
                                SplitWidgetContainer[] splitedCells = m_splittedWidget[rowMergeStartIndex];
                                m_splitedCells[(columnIndex<m_splitedCells.Length)?columnIndex :m_splitedCells .Length -1] = splitedCells[columnIndex];
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Update Splited cell
        /// </summary>
        private void UpdateSplittedCell()
        {
            bool isNeedToUpdateRowIndex = true;
            for (int index = 0; index < m_splitedCells.Length; index++)
            {
                if (index < m_table.Rows[CurrRowIndex].Cells.Count
                    && m_table.Rows[CurrRowIndex].Cells[index].CellFormat.VerticalMerge == CellMerge.Continue)
                {
                    isNeedToUpdateRowIndex = false;
                    break;
                }
            }
            if (!isNeedToUpdateRowIndex)
            {
                for (int i = 0; i < m_splitedCells.Length; i++)
                {
                    if (m_splitedCells[i] == null)
                    {
                        m_splitedCells[i] = new SplitWidgetContainer((TableWidget.GetCellWidget(CurrRowIndex, i) as IWidgetContainer), m_table.Rows[CurrRowIndex].Cells[i].ChildEntities.FirstItem as IWidget, 0);
                    }
                }
            }
            else
            {
                //Reduce the row index when the next row doesn't continue with current row cells
                --m_currRowIndex;
            }
        }
        /// <summary>
        /// Update splitted layouted widget for the VerticalMergeStart cell
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        private LayoutedWidget UpdateSplittedLtWidget(int rowIndex, int colIndex)
        {
            TableLayoutInfo tableLayoutInfo = (TableWidget.GetCellWidget(rowIndex, colIndex)).LayoutInfo as TableLayoutInfo;
            RectangleF cellArea = m_ltWidget.ChildWidgets[rowIndex].ChildWidgets[colIndex].Bounds;
            cellArea.Height = m_currRowLW.Bounds.Bottom - cellArea.Y;
            //Layout Vertical Merge start cell
            LayoutedWidget mergeStartLtWidget = new LayoutedWidget(TableWidget.GetCellWidget(rowIndex, colIndex));
            LayoutContext lc = LayoutContext.Create(mergeStartLtWidget.Widget, m_lcOperator, (float)m_layoutArea.Width);
            LayoutedWidget cellLtWidget = lc.Layout(cellArea);
            cellLtWidget.TopMargin = tableLayoutInfo.TableCellTopMargin;
            if (lc.State == LayoutState.Splitted)
            {
                SplitWidgetContainer[] splitedCells = m_splittedWidget[rowIndex];
                splitedCells[m_currColIndex] = ((lc.SplittedWidget as SplitWidgetContainer) != null)
                                                   ? lc.SplittedWidget as SplitWidgetContainer
                                                   : new SplitWidgetContainer(lc.Widget as IWidgetContainer);
                m_splittedWidget[rowIndex] = splitedCells;
            }
            return cellLtWidget;
        }
        /// <summary>
        /// Determine Whether the row need to be splitted across pages.
        /// </summary>
        /// <returns></returns>
        private bool IsRowNeedToBeSplitted()
        {
            WTableRow tableRow = TableWidget.GetRowWidget(CurrRowIndex) as WTableRow;
            if ((tableRow.RowFormat.IsBreakAcrossPages
                || Math.Round(m_currRowLW.Bounds.Y - m_headerRowHeight, 2) == Math.Round((m_lcOperator as Layouter).PageTopMargin, 2))
                && !(m_currRowLW.Widget.LayoutInfo.IsKeepWithNext
                && Math.Round(m_currRowLW.Bounds.Y - m_headerRowHeight, 2) != Math.Round((m_lcOperator as Layouter).PageTopMargin, 2))
                && !tableRow.IsHeader)
                return true;
            else
            {
                if (m_currRowLW.Widget.LayoutInfo.IsKeepWithNext)
                    CommitKeepWithNext();
                return false;
            }

        }
        /// <summary>
        /// Update Row Width
        /// </summary>
        private void UpdateRowWidth()
        {
            RectangleF bounds = m_currRowLW.Bounds;
            float width = 0;
            for (int i = 0; i < m_currRowLW.ChildWidgets.Count; i++)
            {
                width += m_currRowLW.ChildWidgets[i].Bounds.Width;
            }
            if (width > bounds.Width)
                bounds.Width = width;
            m_currRowLW.Bounds = bounds;
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateRowLWBounds(LayoutContext childContext)
        {
            RectangleF bounds = m_currRowLW.Bounds;
            RectangleF childBounds = m_currCellLW.Bounds;

            double rightPad = (m_bSkipAreaSpacing) ?
              0f : childContext.BoundsPaddingRight;
            double bottomPad = (m_bSkipAreaSpacing) ?
              0f : childContext.BoundsPaddingBottom;

            //ChangeChildsAlignment();

            double right = Math.Max(childBounds.Right + rightPad, bounds.Right);
            double bottom = Math.Max(childBounds.Bottom + bottomPad, bounds.Bottom);
            SizeF size = new SizeF((float)(right - bounds.Left), (float)(bottom - bounds.Top));
            m_currRowLW.Bounds = new RectangleF(bounds.Location, size);
        }
        /// <summary>
        /// 
        /// </summary>
        private void NextRowIndex()
        {
            if (m_bHeaderRepeat)
            {
                WTable table = TableWidget as WTable;
                WTableRow row = table.Rows[0];
                if (row.IsHeader && !table.TableFormat.WrapTextAround && !(m_currRowIndex > 0 && table.Rows[m_currRowIndex].IsHeader))
                    LayoutHeaderRow(row);
                else
                    m_bHeaderRepeat = false;
            }
            else
            {
                //Trace.WriteLine( "Row:" + m_currRowIndex );
                m_currRowIndex++;
            }
        }
        /// <summary>
        /// Layouts the header rows
        /// </summary>
        /// <param name="row">header row</param>
        private void LayoutHeaderRow(WTableRow row)
        {
            if (row.IsHeader)
            {
                m_headerRowHeight = GetHeaderRowHeight(row);
                if (m_headerRowHeight <= (m_lcOperator as Layouter).ClientLayoutArea.Height)
                {
                    while (row.IsHeader)
                    {
                        m_currHeaderRowIndex++;
                        if (m_currRowIndex != m_currHeaderRowIndex)
                        {
                            UpdateHeaderRowWidget();
                            CommitRow();
                            m_currColIndex = -1;
                            //Checks whether next row is an header row
                            if ((row.NextSibling != null && (row.NextSibling is WTableRow) && (row.NextSibling as WTableRow).IsHeader))
                                row = row.NextSibling as WTableRow;
                            else
                            {
                                m_bHeaderRepeat = false;
                                break;
                            }
                        }
                        else
                        {
                            m_bHeaderRepeat = false;
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Get Header Row's Height
        /// </summary>
        /// <param name="headerRow"></param>
        /// <returns></returns>
        private float GetHeaderRowHeight(WTableRow headerRow)
        {
            float headerRowHeight = 0;
            while (headerRow.IsHeader)
            {
                m_currHeaderRowIndex++;
                UpdateHeaderRowWidget();
                headerRowHeight += m_currRowLW.Bounds.Height;
                m_currColIndex = -1;
                //Checks whether next row is an header row
                if ((headerRow.NextSibling != null && (headerRow.NextSibling is WTableRow) && (headerRow.NextSibling as WTableRow).IsHeader))
                    headerRow = headerRow.NextSibling as WTableRow;
                else if (headerRowHeight >= (m_lcOperator as Layouter).ClientLayoutArea.Height)
                {
                    m_bHeaderRepeat = false;
                    break;
                }
                else
                    break;
            }
            m_currHeaderRowIndex = -1;
            return headerRowHeight;
        }
        /// <summary>
        /// Update Header Row Widget
        /// </summary>
        private void UpdateHeaderRowWidget()
        {
            m_currRowLW = new LayoutedWidget(TableWidget.GetRowWidget(CurrRowIndex));

            m_currRowLW.Bounds = new RectangleF(m_layoutArea.ClientActiveArea.Location, new SizeF());
            for (int i = 0; i < (m_currRowLW.Widget as WTableRow).Cells.Count; i++)
            {
                for (int j = 0; j < (m_currRowLW.Widget as WTableRow).Cells[i].Items.Count; j++)
                {
                    if ((m_currRowLW.Widget as WTableRow).Cells[i].Items[j] is WParagraph)
                    {
                        WParagraph para = (m_currRowLW.Widget as WTableRow).Cells[i].Items[j] as WParagraph;
                        if (para != null)
                        {
                            for (int k = 0; k < para.Items.Count; k++)
                            {
                                if (para.Items[k] is WTextRange)
                                {
                                    (para.Items[k] as WTextRange).TextToSplit = (para.Items[k] as WTextRange).Text;
                                }
                                else if (para.Items[k] is WFootnote)
                                    ((para.Items[k] as WFootnote) as IWidget).LayoutInfo.IsSkip = true;
                            }
                        }
                    }

                }
            }
            DoLayoutRow();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the height of the row.
        /// </summary>
        private void UpdateRowSize()
        {
            RectangleF rowBounds = m_currRowLW.Bounds;
            WTableRow row = m_currRowLW.Widget as WTableRow;
            double maxPadding = 0;

            for (int i = 0; i < m_currRowLW.ChildWidgets.Count; i++)
            {
                LayoutedWidget cellLtWidget = m_currRowLW.ChildWidgets[i];

                TableLayoutInfo cellTableInfo = cellLtWidget.Widget.LayoutInfo as TableLayoutInfo;

                bool currCellContinue = cellTableInfo.IsRowMergeContinue;
                bool nextCellContinue = false;

                maxPadding =
                  Math.Max(maxPadding,
                            cellLtWidget.Widget.LayoutInfo.Paddings.Bottom +
                            cellLtWidget.Widget.LayoutInfo.Paddings.Top);

                // Checks if next cell not "IsRowMergeContinue"
                if (CurrRowIndex != TableWidget.RowsCount - 1)
                {
                    int index = GetNextRowCellIndex(CurrRowIndex, i);
                    IWidgetContainer wc = TableWidget.GetCellWidget(CurrRowIndex + 1, index);
                    TableLayoutInfo wcTableInfo = wc.LayoutInfo as TableLayoutInfo;
                    nextCellContinue = wcTableInfo.IsRowMergeContinue;
                }

                if ((currCellContinue || cellTableInfo.IsRowMergeStart)
                    && !nextCellContinue)
                {
                    int index = i;
                    if (CurrRowIndex > 0 && m_ltWidget.ChildWidgets.Count > 0)
                        index = GetVerticalMergeStartCellIndex(cellLtWidget.Bounds.X, i);
                    rowBounds.Height = (float)Math.Max(m_rowsHeight[index], rowBounds.Height);
                }
            }
            TableLayoutInfo rowTableInfo = m_currRowLW.Widget.LayoutInfo as TableLayoutInfo;

            double rowHeight = rowTableInfo.RowHeight;
            if (!rowTableInfo.IsExactlyRowHeight && !m_table.m_isTextBox)
                rowHeight += (float)maxPadding;
            //rowBounds.Width = m_clientArea.Width;
            if (m_layoutArea.ClientActiveArea.Width > 0)
                rowBounds.Width = m_layoutArea.ClientActiveArea.Width;
            // expansion row height to "fixed row height"
            if ((rowTableInfo.IsExactlyRowHeight && rowHeight > 0) || m_table.m_isTextBox)
            {
                rowBounds.Height = (float)rowHeight;
            }
            else
            {
                if (rowTableInfo.IsVerticalText)
                {
                    if (rowTableInfo.RowHeight == 0)
                    {
                        float height = (m_table.Rows[CurrRowIndex].Cells[0].LastParagraph as IWidget).LayoutInfo.Size.Height;
                        rowBounds.Height = (float)rowHeight + height;
                    }
                    else
                        rowBounds.Height = (float)rowHeight;
                }
                else
                {
                    bool isVerticalText = false;
                    for (int i = 0; i < m_currRowLW.ChildWidgets.Count; i++)
                    {
                        if (m_currRowLW.ChildWidgets[i].Widget.LayoutInfo.IsVerticalText)
                        {
                            isVerticalText = true;
                            break;
                        }
                    }
                    if (isVerticalText)
                    {
                        float minHeight = 0;
                        for (int i = 0; i < m_currRowLW.ChildWidgets.Count; i++)
                        {
                            float maxHeight = m_currRowLW.ChildWidgets[i].Bounds.Height;
                            if (!m_currRowLW.ChildWidgets[i].Widget.LayoutInfo.IsVerticalText && minHeight < maxHeight)
                            {
                                minHeight = maxHeight;
                            }
                        }
                        rowBounds.Height = (float)Math.Max(minHeight, rowHeight);
                    }
                    else
                        rowBounds.Height = (float)Math.Max(rowBounds.Height, rowHeight);
                }
            }
            if (m_rowsHeight.Length > 0)
            {
                double height = m_rowsHeight[0];
                for (int i = 1; i < m_currRowLW.ChildWidgets.Count; i++)
                {
                    if (height < m_rowsHeight[i])
                    {
                        height = m_rowsHeight[i];
                    }
                }
                /*  if (rowBounds.Height < height)
                      rowBounds.Height = (float)height;*/
            }
            float cellSpacing = 0;
            if (m_table.TableFormat.CellSpacing > 0)
                cellSpacing = m_table.TableFormat.CellSpacing;
            if (row.GetRowIndex() > 0 && rowTableInfo.IsExactlyRowHeight)
            {
                rowBounds.Y += cellSpacing;
            }
            m_currRowLW.Bounds = rowBounds;
            if (!rowTableInfo.IsExactlyRowHeight && !rowTableInfo.IsVerticalText)
            {
                for (int i = 0; i < m_currRowLW.ChildWidgets.Count; i++)
                {
                    if (m_currRowLW.ChildWidgets[i].Widget.LayoutInfo.IsVerticalText)
                    {
                        //Get the cell widget with text direction as vertical
                        LayoutedWidget cellLtWidget = GetCellWidget(rowBounds.Height, CurrRowIndex, i);
                        if (m_currRowLW.ChildWidgets[i].ChildWidgets.Count > 0)
                        {
                            m_currRowLW.ChildWidgets[i].ChildWidgets[0] = cellLtWidget;
                        }
                        else
                        {
                            m_currRowLW.ChildWidgets[i].ChildWidgets.Add(cellLtWidget);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets the index of the next row cell.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <returns></returns>
        private int GetNextRowCellIndex(int rowIndex, int cellIndex)
        {
            int nextRowCellIndex = 0;
            float cellStartPos = 0;
            for (int i = 0; i < cellIndex; i++)
            {
                cellStartPos += (TableWidget as WTable).Rows[rowIndex].Cells[i].Width;
            }
            float nextRowCellStartPos = 0;
            for (int i = 0; i < (TableWidget as WTable).Rows[rowIndex + 1].Cells.Count; i++)
            {
                nextRowCellStartPos += (TableWidget as WTable).Rows[rowIndex + 1].Cells[i].Width;
                if (cellStartPos < nextRowCellStartPos)
                {
                    nextRowCellIndex = i;
                    break;
                }
                else if (cellStartPos == nextRowCellStartPos)
                {
                    if (i == (TableWidget as WTable).Rows[rowIndex + 1].Cells.Count - 1)
                        return i;
                    nextRowCellIndex = i + 1;
                    break;
                }
            }
            return nextRowCellIndex;
        }

        /// <summary>
        /// Updates the size of the cell.
        /// </summary>
        /// <param name="column">The column.</param>
        private void UpdateCellSize(int column)
        {
            TableLayoutInfo cellLI = TableWidget.GetCellWidget(CurrRowIndex, column).LayoutInfo as TableLayoutInfo;
            UpdateCellWidth(column);
            UpdateCellHeight(column);
        }

        /// <summary>
        /// Updates the width of the cell.
        /// </summary>
        /// <param name="column">The column.</param>
        private void UpdateCellWidth(int column)
        {
            LayoutedWidget cellLtWidget = m_currRowLW.ChildWidgets[column];
            RectangleF bounds = cellLtWidget.Bounds;
            m_currColIndex = column;
            bounds.Width = GetCellMergedWidth(CurrRowIndex, m_currColIndex);
            // Updates the spacing as cell width according to MS Word behavior, if cell width is "0".
            if (bounds.Width == 0)
                bounds.Width = (float)(cellLtWidget.Widget.LayoutInfo.Paddings.Left
                    + cellLtWidget.Widget.LayoutInfo.Paddings.Right
                    + cellLtWidget.Widget.LayoutInfo.Margins.Left
                    + cellLtWidget.Widget.LayoutInfo.Margins.Right);
            if (m_table.TableFormat.CellSpacing > 0)
                bounds.Width -= (float)(m_table.TableFormat.CellSpacing * 2);
            cellLtWidget.Bounds = bounds;
            if (cellLtWidget.TextTag == "SkipRight")
            {
                UpdateSkipRightPosition(cellLtWidget, (float)(cellLtWidget.Bounds.Right - (cellLtWidget.Widget.LayoutInfo.Margins.Left + cellLtWidget.Widget.LayoutInfo.Margins.Right)));
            }
        }
        /// <summary>
        /// Get Column Index.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        private int GetColumnIndex(WSection section)
        {
            ColumnCollection columns = section.Columns;
            int columnIndex = 0;
            float prevColWidth = 0f;
            for (int i = 0; i < columns.Count; i++)
            {
                if ((m_layoutArea.ClientArea.X - section.PageSetup.Margins.Left - prevColWidth) <= columns[i].Width)
                {
                    columnIndex = i;
                    break;
                }
                prevColWidth += columns[i].Width + columns[i].Space;
            }
            return columnIndex;
        }

        /// <summary>
        /// Updates the height of the cell.
        /// </summary>
        /// <param name="column">The column.</param>
        private void UpdateCellHeight(int column)
        {
            LayoutedWidget cellLtWidget = m_currRowLW.ChildWidgets[column];

            RectangleF bounds = cellLtWidget.Bounds;
            RectangleF rowBounds = m_currRowLW.Bounds;

            bool currCellMergeContinue =
              (cellLtWidget.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeContinue;
            bool nextCellMergeContinue = false;

            if (CurrRowIndex != TableWidget.RowsCount - 1)
            {
                IWidgetContainer widget = TableWidget.GetCellWidget(CurrRowIndex, column);
                nextCellMergeContinue = (widget.LayoutInfo as TableLayoutInfo).IsRowMergeContinue;
            }
            if (m_isTableSplitted && !m_isSplitTableCurrentCellMergeStart)
            {
                currCellMergeContinue = false;
                m_isSplitTableCurrentCellMergeStart = true;
            }
            if (currCellMergeContinue)// && !nextCellMergeContinue)
            {
                UpdateContinueCellHeight(column);
            }
            float cellSpacing = 0;
            if (m_table.TableFormat.CellSpacing > 0)
                cellSpacing = m_table.TableFormat.CellSpacing * 2;
            // Correct cell height margins
            bounds.Height = rowBounds.Height - cellSpacing;
            if (CurrRowIndex > 0 && (cellLtWidget.Widget.LayoutInfo as TableLayoutInfo).IsExactlyRowHeight)
                bounds.Height += (cellSpacing / 2);
            if ((cellLtWidget.Widget.LayoutInfo as WTableCell.LayoutCellInfo).SkipBottomBorder) //Update the cell height based on bottom border width when the cell doesn't have bottom border.
                bounds.Height -= (float)(cellLtWidget.Widget.LayoutInfo.Margins.Bottom - cellSpacing);
            cellLtWidget.Bounds = bounds;
            if (CurrRowIndex > 0
                && m_ltWidget.ChildWidgets.Count > 0
                && !(cellLtWidget.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart)
                UpdateMergedCellHeight(cellLtWidget.Bounds.X, rowBounds.Height);
            else if ((cellLtWidget.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart)
            {
                double rowHeight = m_rowsHeight[m_currColIndex] - rowBounds.Height;
                m_rowsHeight[m_currColIndex] = (rowHeight > 0) ? rowHeight : 0;
            }
            if (cellLtWidget.ChildWidgets[0].ChildWidgets.Count > 0 && (m_currRowLW.Widget.LayoutInfo as TableLayoutInfo).IsExactlyRowHeight)
                UpdateSkipBottomBorder(cellLtWidget);
        }
        /// <summary>
        /// Gets the index of the vertical merge start cell.
        /// </summary>
        /// <param name="cellStartPos">The cellStartPos.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <returns></returns>
        private int GetVerticalMergeStartCellIndex(float cellStartPos, int cellIndex)
        {
            int mergeCellIndex = cellIndex;
            for (int i = m_ltWidget.ChildWidgets.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < m_ltWidget.ChildWidgets[i].ChildWidgets.Count; j++)
                {
                    float adjCellStartPos = m_ltWidget.ChildWidgets[i].ChildWidgets[j].Bounds.X;
                    if (cellStartPos <= adjCellStartPos)
                    {
                        if ((m_ltWidget.ChildWidgets[i].ChildWidgets[j].Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart)
                        {
                            if (m_mergedRowIndex[j] == i)
                                return j;
                            else
                                return mergeCellIndex;
                        }
                        else if (!(m_ltWidget.ChildWidgets[i].ChildWidgets[j].Widget.LayoutInfo as TableLayoutInfo).IsRowMergeContinue)
                            return mergeCellIndex;
                        break;
                    }
                }
            }
            return mergeCellIndex;
        }
        /// <summary>
        /// Gets the index of the vertical merge start cell.
        /// </summary>
        /// <param name="cellStartPos">The cellStartPos.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <returns></returns>
        private int GetVerticalMergeCellStartIndex(float cellStartPos, int cellIndex)
        {
            int mergeCellIndex = cellIndex;
            for (int i = CurrRowIndex - 1; i >= 0; i--)
            {
                for (int j = 0; j < m_table.Rows[i].Cells.Count; j++)
                {
                    float adjCellStartPos = (m_table.Rows[i].Cells[j].m_layoutInfo as TableLayoutInfo).TableCellLeftMargin;
                    if (cellStartPos == adjCellStartPos)
                    {
                        if ((m_table.Rows[i].Cells[j].m_layoutInfo as TableLayoutInfo).IsRowMergeStart)
                        {
                            return j;
                            break;
                        }
                    }
                }
            }
            return mergeCellIndex;
        }
        /// <summary>
        /// Update Row Height for the vertical Merged cells
        /// </summary>
        /// <param name="cellStartPos">The Cell Start position</param>
        /// <param name="height">Height of the row</param>
        private void UpdateMergedCellHeight(float cellStartPos, float height)
        {
            for (int i = m_ltWidget.ChildWidgets.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < m_ltWidget.ChildWidgets[i].ChildWidgets.Count; j++)
                {
                    float adjCellStartPos = m_ltWidget.ChildWidgets[i].ChildWidgets[j].Bounds.X;
                    if (cellStartPos <= adjCellStartPos)
                    {
                        if ((m_ltWidget.ChildWidgets[i].ChildWidgets[j].Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart)
                        {
                            if (m_mergedRowIndex[j] == i)
                            {
                                double rowHeight = m_rowsHeight[j] - height;
                                m_rowsHeight[j] = (rowHeight > 0) ? rowHeight : 0;
                                return;
                            }
                        }
                        else if (!(m_ltWidget.ChildWidgets[i].ChildWidgets[j].Widget.LayoutInfo as TableLayoutInfo).IsRowMergeContinue)
                            return;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Updates whether to skip bottom border.
        /// This method updates skip bottom border in nested tables.
        /// </summary>
        /// <param name="cellWidget">The cell widget.</param>
        private void UpdateSkipBottomBorder(LayoutedWidget cellWidget)
        {
            if (cellWidget.ChildWidgets[0].ChildWidgets[cellWidget.ChildWidgets.Count - 1].Widget is WTable)
            {
                LayoutedWidget tableWidget = cellWidget.ChildWidgets[0].ChildWidgets[cellWidget.ChildWidgets.Count - 1];
                if (tableWidget.Bounds.Bottom >= cellWidget.Bounds.Bottom)
                {
                    for (int i = 0; i < tableWidget.ChildWidgets.Count; i++)
                    {
                        LayoutedWidget lastRowWidget = tableWidget.ChildWidgets[i];
                        for (int j = 0; j < lastRowWidget.ChildWidgets.Count && lastRowWidget.Bounds.Bottom >= cellWidget.Bounds.Bottom; j++)
                        {
                            if (lastRowWidget.Bounds.Top >= cellWidget.Bounds.Bottom)
                                lastRowWidget.ChildWidgets.RemoveAt(j);
                            else
                            {
                                (lastRowWidget.ChildWidgets[j].Widget.LayoutInfo as WTableCell.LayoutCellInfo).SkipBottomBorder = true;
                                float bottom = (float)(cellWidget.Bounds.Bottom - (cellWidget.Widget.LayoutInfo.Margins.Bottom + cellWidget.Widget.LayoutInfo.Margins.Top));
                                lastRowWidget.UpdateLtWidgetBounds(0, bottom - lastRowWidget.ChildWidgets[j].Bounds.Y, 0, bottom - lastRowWidget.ChildWidgets[j].Bounds.Y);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private int UpdateColumnIndex(int rowIndex, int column)
        {
            float count_prev = 0.0f;
            float count_curr = 0.0f;
            int x = 0;
            if (column == 0)
                return column;
            for (int i = 0; i < column; i++)
            {
                count_curr += m_table.Rows[CurrRowIndex].Cells[i].Width;
            }
            for (int i = 0; i < m_table.Rows[rowIndex].Cells.Count; i++)
            {
                count_prev += m_table.Rows[rowIndex].Cells[i].Width;
                if (Math.Round(count_prev, 1) == Math.Round(count_curr, 1))
                {
                    column = ++i;
                    break;
                }
                else if (Math.Round(count_prev, 1) > Math.Round(count_curr, 1))
                {
                    if (Math.Ceiling(count_prev) == Math.Ceiling(count_curr))
                        column = ++i;
                    else
                        column = i;
                    break;
                }
            }
            if (m_table.Rows[rowIndex].Cells.Count <= column)
                column = m_table.Rows[rowIndex].Cells.Count - 1;
            return column;
        }

        /// <summary>
        /// Updates the height of the continue cell.
        /// </summary>
        /// <param name="column">The column.</param>
        private void UpdateContinueCellHeight(int column)
        {
            int columnIndex = column;
            int rowIndex = CurrRowIndex - 1;
            int splitTableCurrRowIndex = 0;
            RectangleF rowBounds = m_currRowLW.Bounds;
            WTableRow lastRow = null;
            int lastRowIndex = -1;
            bool isRowMergeStart = false;
            while (rowIndex > -1)
            {
                column = UpdateColumnIndex(rowIndex, columnIndex);//column);
                TableLayoutInfo last_widgetTableInfo = TableWidget.GetCellWidget(rowIndex, column).LayoutInfo as TableLayoutInfo;
                //Get the IsRowMergeStart Property
                isRowMergeStart = last_widgetTableInfo.IsRowMergeStart;
                if (m_isSplitTableCurrentCellMergeStart || (m_splittedTableRowIndex == rowIndex && m_isTableSplitted))
                {
                    if (last_widgetTableInfo.IsRowMergeContinue)
                        isRowMergeStart = (m_ltWidget.ChildWidgets.Count == (CurrRowIndex - rowIndex));
                    splitTableCurrRowIndex = CurrRowIndex;
                    m_splittedTableRowIndex = rowIndex;
                }
                if (isRowMergeStart && m_ltWidget.ChildWidgets.Count + splitTableCurrRowIndex > rowIndex)
                {
                    if (m_ltWidget.ChildWidgets.Count == 0)
                    {
                        break;
                    }
                    LayoutedWidget last_cellLtWidget;
                    //Get Last row and it's index
                    if (splitTableCurrRowIndex != 0)
                    {
                        last_cellLtWidget = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - (CurrRowIndex - rowIndex)].ChildWidgets[column];
                        (last_cellLtWidget.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart = isRowMergeStart;
                        lastRow = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - (CurrRowIndex - rowIndex)].Widget as WTableRow;
                        lastRowIndex = lastRow.GetRowIndex();
                    }
                    else
                    {
                        last_cellLtWidget = m_ltWidget.ChildWidgets[rowIndex].ChildWidgets[column];
                        lastRow = m_ltWidget.ChildWidgets[rowIndex].Widget as WTableRow;
                        lastRowIndex = lastRow.GetRowIndex();
                    }

                    RectangleF cell_bounds = last_cellLtWidget.Bounds;
                    //Check previous row index and last row index is same or not
                    if (lastRowIndex == rowIndex)
                        cell_bounds.Height = rowBounds.Bottom - last_cellLtWidget.Bounds.Top;
                    last_cellLtWidget.Bounds = cell_bounds;
                    if (last_widgetTableInfo.IsExactlyRowHeight
                        && (m_currRowLW.ChildWidgets[columnIndex].Widget.LayoutInfo as TableLayoutInfo).IsRowMergeContinue
                        && (CurrRowIndex == m_table.Rows.Count - 1
                        || (CurrRowIndex < m_table.Rows.Count - 1
                        && m_table.Rows[CurrRowIndex + 1].Cells[GetNextRowCellIndex(CurrRowIndex, columnIndex)].CellFormat.VerticalMerge != CellMerge.Continue)))
                    {
                        UpdateChildWidget(last_cellLtWidget);
                    }
                    break;
                }
                else if (isRowMergeStart && m_ltWidget.ChildWidgets.Count <= rowIndex)
                {
                    LayoutedWidget last_cellLtWidget = new LayoutedWidget(TableWidget.GetCellWidget(rowIndex, column));
                    RectangleF cell_bounds = last_cellLtWidget.Bounds;
                    cell_bounds.Height = rowBounds.Bottom - last_cellLtWidget.Bounds.Top;
                    // Correct cell mergins          
                    cell_bounds.Height -= (float)last_cellLtWidget.Widget.LayoutInfo.Margins.Bottom;
                    last_cellLtWidget.Bounds = cell_bounds;
                    break;
                }
                rowIndex--;
            }
        }
        /// <summary>
        /// Layouts the cell, based on the updated cell area.
        /// </summary>
        /// <param name="ltWidget">The ltWidget.</param>
        private void UpdateChildWidget(LayoutedWidget ltWidget)
        {
            // Layouts the cell, based on the updated cell client area.
            LayoutContext lc = LayoutContext.Create(ltWidget.Widget, m_lcOperator, (float)m_layoutArea.Width);
            LayoutedWidget cellLtWidget = lc.Layout(ltWidget.Bounds);
            cellLtWidget.TopMargin = cellLtWidget.Bounds.Y;
            ltWidget.ChildWidgets[0] = cellLtWidget;
        }
        /// <summary>
        /// Deletes the continuous cells.
        /// </summary>
        private void DeleteContinuousCells()
        {
            for (int i = 0, cnt = m_ltWidget.ChildWidgets.Count; i < cnt; i++)
            {
                LayoutedWidget lw = m_ltWidget.ChildWidgets[i];

                for (int j = lw.ChildWidgets.Count - 1; j > -1; j--)
                {
                    LayoutedWidget cellLW = lw.ChildWidgets[j];

                    TableLayoutInfo tableInfo = cellLW.Widget.LayoutInfo as TableLayoutInfo;
                    bool colMergeCont = tableInfo.IsColumnMergeContinue;

                    if (tableInfo.IsRowMergeContinue)
                    {
                        int rowMergeStartIndex = i;
                        int columnIndex = j;
                        if (SearchedMergeStart(ref rowMergeStartIndex, ref columnIndex))
                        {
                            //Remove all the Child items from a cell Widget
                            for (int itemIndex = lw.ChildWidgets[j].ChildWidgets.Count - 1; itemIndex >= 0; itemIndex--)
                            {
                                m_ltWidget.ChildWidgets[i].ChildWidgets[j].ChildWidgets.RemoveAt(itemIndex);
                            }
                            if (i + 1 < m_ltWidget.ChildWidgets.Count && m_ltWidget.ChildWidgets[i + 1].ChildWidgets.Count > columnIndex
                                && (lw.Widget.LayoutInfo as TableLayoutInfo).IsExactlyRowHeight
                                && !(m_ltWidget.ChildWidgets[i + 1].ChildWidgets[columnIndex].Widget.LayoutInfo as TableLayoutInfo).IsRowMergeContinue
                                && lw.ChildWidgets.Count > columnIndex
                                && (m_ltWidget.ChildWidgets[rowMergeStartIndex].Widget as WTableRow) == (m_table.Rows[rowMergeStartIndex] as WTableRow))
                            {
                                float height = lw.ChildWidgets[columnIndex].Bounds.Bottom - m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex].Bounds.Top;
                                LayoutedWidget cellLtWidget = m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex];
                                TableLayoutInfo tableLayoutInfo = (TableWidget.GetCellWidget(rowMergeStartIndex, columnIndex)).LayoutInfo as TableLayoutInfo;
                                if (!cellLtWidget.Widget.LayoutInfo.IsVerticalText)
                                {
                                    RectangleF cellBounds = new RectangleF();
                                    cellBounds.X = (float)(tableLayoutInfo.TableCellLeftMargin - tableLayoutInfo.Margins.Left - tableLayoutInfo.Paddings.Left);
                                    cellBounds.Width = GetCellClientArea((tableLayoutInfo != null) ? tableLayoutInfo.IsColumnMergeStart : false, rowMergeStartIndex, columnIndex).ClientArea.Width;
                                    cellBounds.Height = height;
                                    cellBounds.Y = m_ltWidget.ChildWidgets[rowMergeStartIndex].Bounds.Top;
                                    //Layout Vertical Merge start cell
                                    LayoutedWidget mergeStartLtWidget = new LayoutedWidget(TableWidget.GetCellWidget(rowMergeStartIndex, columnIndex));
                                    LayoutContext lc = LayoutContext.Create(mergeStartLtWidget.Widget, m_lcOperator, (float)m_layoutArea.Width);
                                    cellLtWidget = lc.Layout(cellBounds);
                                    cellLtWidget.Bounds = new RectangleF(cellBounds.X, m_ltWidget.ChildWidgets[rowMergeStartIndex].Bounds.Y, cellBounds.Width, height);
                                    cellLtWidget.TopMargin = cellBounds.Y;
                                    m_ltWidget.ChildWidgets[rowMergeStartIndex].ChildWidgets[columnIndex] = cellLtWidget;
                                }
                            }
                        }
                        else
                        {
                            if (i == 0 && m_isTableSplitted && !colMergeCont && cellLW.Widget is WTableCell && CurrRowIndex != 0)
                            {
                                int cellIndex = GetVerticalMergeCellStartIndex(((lw.ChildWidgets[j].Widget as IWidget).LayoutInfo as TableLayoutInfo).TableCellLeftMargin, j);
                                for (int rowIndex = CurrRowIndex - 1; rowIndex >= 0; rowIndex--)
                                {
                                    if (m_table.Rows[rowIndex].Cells.Count > cellIndex && m_table.Rows[rowIndex].Cells[cellIndex].CellFormat.VerticalMerge == CellMerge.Start)
                                    {
                                        if (!(TableWidget.GetCellWidget(rowIndex, cellIndex)).LayoutInfo.IsVerticalText)
                                        {
                                            float height = lw.ChildWidgets[j].Bounds.Height;
                                            //Update Vertical Merge Continuous Cell Widget
                                            lw.ChildWidgets[j].Bounds = UpdateVerticalMergeCellWidget(rowIndex, cellIndex);
                                            lw.ChildWidgets[j].Bounds = new RectangleF(lw.ChildWidgets[j].Bounds.X, lw.Bounds.Y, lw.ChildWidgets[j].Bounds.Width, height);
                                            lw.ChildWidgets[j].TopMargin = lw.ChildWidgets[j].Bounds.Y;
                                        }
                                        //Remove all the Child items from a cell Widget
                                        for (int itemIndex = lw.ChildWidgets[j].ChildWidgets.Count - 1; itemIndex >= 0; itemIndex--)
                                        {
                                            m_ltWidget.ChildWidgets[i].ChildWidgets[j].ChildWidgets.RemoveAt(itemIndex);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (colMergeCont)
                    {
                        m_ltWidget.ChildWidgets[i].ChildWidgets.RemoveAt(j);
                    }
                    else if (cellLW.ChildWidgets.Count > 0)
                    {
                        // Update cell vertical alignment
                        RectangleF bounds = cellLW.Bounds;
                        LayoutedWidget child = cellLW.ChildWidgets[0];
                        float displacement = 0;
                        bool isHeader = (cellLW.Widget is WTableCell) ? (cellLW.Widget as WTableCell).OwnerRow.IsHeader : false;
                        switch ((child.Widget.LayoutInfo as TableLayoutInfo).VerticalAlignment)
                        {
                            case 1:
                                if (!(child.Widget is SplitWidgetContainer) && !child.Widget.LayoutInfo.IsVerticalText)
                                        displacement = (bounds.Height - child.Bounds.Height) / 2;
                                break;
                            case 2:
                                if (!(child.Widget is SplitWidgetContainer) && !child.Widget.LayoutInfo.IsVerticalText)
                                    displacement = bounds.Height - child.Bounds.Height - ((float)child.Widget.LayoutInfo.Paddings.Bottom + (float)child.Widget.LayoutInfo.Margins.Bottom);//remove the bottom padding and bottom marigin value
                                break;
                        }
                        if (displacement > 0 && IsLayoutedWidgetNeedToBeShifted(child))
                            child.ShiftLocation(0, displacement, false);
                        child.Bounds = RectangleF.Empty;
                    }
                }
            }
        }
        /// <summary>
        /// Update Vertical Merge Cell Widget
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        private RectangleF UpdateVerticalMergeCellWidget(int rowIndex, int colIndex)
        {
            TableLayoutInfo tableLayoutInfo = (TableWidget.GetCellWidget(rowIndex, colIndex)).LayoutInfo as TableLayoutInfo;
            RectangleF cellBounds = new RectangleF();
            cellBounds.X = (float)(tableLayoutInfo.TableCellLeftMargin - tableLayoutInfo.Margins.Left - tableLayoutInfo.Paddings.Left);
            cellBounds.Width = GetCellClientArea((tableLayoutInfo != null) ? tableLayoutInfo.IsColumnMergeStart : false, rowIndex, colIndex).ClientArea.Width;
            //Layout Vertical Merge start cell
            LayoutedWidget mergeStartLtWidget = new LayoutedWidget(TableWidget.GetCellWidget(rowIndex, colIndex));
            LayoutContext lc = LayoutContext.Create(mergeStartLtWidget.Widget, m_lcOperator, (float)m_layoutArea.Width);
            LayoutedWidget cellLtWidget = lc.Layout(cellBounds);
            cellLtWidget.TopMargin = cellBounds.Y;
            cellLtWidget.Bounds = cellBounds;
            return cellLtWidget.Bounds;
        }
        /// <summary>
        /// Determines whether Layouted Widget Need to be Shifted
        /// </summary>
        /// <param name="child"></param>
        /// <returns>
        /// 	<c>true</c> if layouted widget is need to be shifted, set to <c>true</c>.
        /// </returns>
        private bool IsLayoutedWidgetNeedToBeShifted(LayoutedWidget child)
        {
            for (int i = 0; i < child.ChildWidgets.Count; i++)
            {
                for (int j = 0; j < child.ChildWidgets[i].ChildWidgets.Count; j++)
                {
                    if (child.ChildWidgets[i].ChildWidgets[j].Widget is WParagraph
                        || ((child.ChildWidgets[i].ChildWidgets[j].Widget is SplitWidgetContainer)
                        ? (child.ChildWidgets[i].ChildWidgets[j].Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph : false))
                    {
                        for (int k = 0; k < child.ChildWidgets[i].ChildWidgets[j].ChildWidgets.Count; k++)
                        {
                            if (((child.ChildWidgets[i].ChildWidgets[j].ChildWidgets[k].Widget is WPicture)
                                && (child.ChildWidgets[i].ChildWidgets[j].ChildWidgets[k].Widget as WPicture).LayoutInCell
                                && (child.ChildWidgets[i].ChildWidgets[j].ChildWidgets[k].Widget as WPicture).TextWrappingStyle != TextWrappingStyle.Inline)
                                || ((child.ChildWidgets[i].ChildWidgets[j].ChildWidgets[k].Widget is Shape)
                                && (child.ChildWidgets[i].ChildWidgets[j].ChildWidgets[k].Widget as Shape).LayoutInCell
                                && (child.ChildWidgets[i].ChildWidgets[j].ChildWidgets[k].Widget as Shape).WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline))
                                return false;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Creates the table client area.
        /// </summary>
        /// <param name="rect">The rect.</param>
        private void CreateTableClientArea(ref RectangleF rect)
        {
            ITableLayoutInfo info = TableLayoutInfo;
            Paddings padding = new Paddings();

#if FIXED_TABLE_WIDTH      
      float fullWidth = (float)(info.Width
                        + LayoutInfo.Paddings.Left
                        + LayoutInfo.Paddings.Right
                        + LayoutInfo.Margins.Left
                        + LayoutInfo.Margins.Right);  
      if( fullWidth > 0 && fullWidth < rect.Width )
         rect.Width = fullWidth;
#endif

#if !FIXED_TABLE_WIDTH
            CorrectTableClientArea(ref rect);
#endif

            if (TableLayoutInfo.Height > 0 && info.Height < rect.Height)
                rect.Height = info.Height;

            CreateLayoutArea(rect, padding);
        }

        /// <summary>
        /// Does the layout cell.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        /// <param name="cellArea">The cell area.</param>
        private void DoLayoutCell(LayoutContext childContext, RectangleF cellArea)
        {
            (childContext.LayoutInfo as TableLayoutInfo).IsExactlyRowHeight =
              (m_currRowLW.Widget.LayoutInfo as TableLayoutInfo).IsExactlyRowHeight;
            (childContext.LayoutInfo as TableLayoutInfo).TableCellLeftMargin = (float)(cellArea.X + childContext.LayoutInfo.Paddings.Left + childContext.LayoutInfo.Margins.Left);
            (childContext.LayoutInfo as TableLayoutInfo).TableCellTopMargin = (float)(cellArea.Y + childContext.LayoutInfo.Paddings.Top + childContext.LayoutInfo.Margins.Top);
            int count = (m_lcOperator as Layouter).FloatingItems.Count;
            LayoutedWidget child = childContext.Layout(cellArea);
            child.TopMargin = (float)(cellArea.Y + childContext.LayoutInfo.Paddings.Top + childContext.LayoutInfo.Margins.Top);
            for (int i = (m_lcOperator as Layouter).FloatingItems.Count - 1; i >= count; i--)
            {
                TextWrappingStyle textWrappingStyle = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingStyle;
                RectangleF bounds = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingBounds;
                if (bounds.Width > cellArea.Width
                    && textWrappingStyle != TextWrappingStyle.InFrontOfText
                    && textWrappingStyle != TextWrappingStyle.Behind)
                {
                    bounds.Width = cellArea.Width;
                    (m_lcOperator as Layouter).FloatingItems[i].TextWrappingBounds = bounds;
                }
            }
            UpdateCellLWBounds(child);
            LayoutedWidget lw = new LayoutedWidget(childContext.Widget, child.Bounds.Location);
            lw.ChildWidgets.Add(child);
            child.Owner = lw;
            lw.Bounds = child.Bounds;
            (child.Widget.LayoutInfo as TableLayoutInfo).CellHeight = child.Bounds.Height;
            m_currCellLW = lw;
            if (child.Bounds.Right > cellArea.Right)
            {
                m_currCellLW.TextTag = "SkipRight";
            }
        }
        /// <summary>
        /// Updates the skip right position.
        /// </summary>
        /// <param name="cellWidget">The cell widget.</param>
        /// <param name="right">The right.</param>
        private void UpdateSkipRightPosition(LayoutedWidget cellWidget, float right)
        {
            for (int i = 0; i < cellWidget.ChildWidgets[0].ChildWidgets.Count; i++)
            {
                if (cellWidget.ChildWidgets[0].ChildWidgets[i].Widget is WTable
                   && Math.Round(cellWidget.Bounds.Right) <= Math.Round(cellWidget.ChildWidgets[0].ChildWidgets[i].Bounds.Right))
                {
                    LayoutedWidget tableWidget = cellWidget.ChildWidgets[0].ChildWidgets[i];
                    if ((tableWidget.Widget as WTable).m_isTextBox ?
                        (tableWidget.Widget as WTable).m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.InFrontOfText
                        && (tableWidget.Widget as WTable).m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.Behind : true)
                    {
                        tableWidget.TextTag = "Clipped";
                        tableWidget.RightPosition = right;
                        for (int j = 0; j < tableWidget.ChildWidgets.Count; j++)
                        {
                            LayoutedWidget rowWidget = tableWidget.ChildWidgets[j];
                            rowWidget.TextTag = "Clipped";
                            rowWidget.RightPosition = right;
                            if (rowWidget.ChildWidgets.Count > 0)
                            {
                                for (int k = 0; k < rowWidget.ChildWidgets.Count; k++)
                                {
                                    if (rowWidget.ChildWidgets[k].Bounds.Right > right && rowWidget.ChildWidgets[k].Bounds.Left < right)
                                    {
                                        rowWidget.ChildWidgets[k].TextTag = "Clipped";
                                        rowWidget.ChildWidgets[k].RightPosition = right;
                                        rowWidget.UpdateLtWidgetBounds(right - cellWidget.Bounds.X, 0, right - cellWidget.Bounds.X, 0);
                                    }
                                    else if (rowWidget.ChildWidgets[k].ChildWidgets.Count > 0
                                        && rowWidget.ChildWidgets[k].Bounds.Left > right)
                                    {
                                        rowWidget.ChildWidgets[k].Bounds = new RectangleF();
                                        rowWidget.ChildWidgets[k].ChildWidgets.RemoveAt(0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Updates the cell LW bounds.
        /// </summary>
        /// <param name="child">The child.</param>
        private void UpdateCellLWBounds(LayoutedWidget child)
        {
            RectangleF bounds = child.Bounds;
            WTableCell cell = (child.Widget is WTableCell) ? (child.Widget as WTableCell) : ((child.Widget as SplitWidgetContainer).RealWidgetContainer as WTableCell);
            float cellSpacing = 0;
            if (cell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                cellSpacing = cell.OwnerRow.OwnerTable.TableFormat.CellSpacing * 2;
            float top = (float)child.Widget.LayoutInfo.Margins.Top - cellSpacing;
            if (top > 0)
            {
                bounds.Y -= top;
                bounds.Height += top;
            }
            float left = (float)child.Widget.LayoutInfo.Margins.Left - cellSpacing;
            if (left > 0)
            {
                bounds.X -= left;
                bounds.Width += left;
            }
            child.Bounds = bounds;
        }

        /// <summary>
        /// Creates the next cell context.
        /// </summary>
        /// <returns></returns>
        private LayoutContext CreateNextCellContext()
        {
            if (m_currColIndex + 1 < m_table.Rows[CurrRowIndex].Cells.Count)
            {
                m_currColIndex++;
                IWidgetContainer widget = null;

                if (m_spitTableWidget != null &&
                  m_spitTableWidget.SplittedCells != null &&
                  CurrRowIndex == m_spitTableWidget.StartRowNumber - 1
                  && m_currColIndex < m_spitTableWidget.SplittedCells.Length)
                {
                    widget = m_spitTableWidget.SplittedCells[m_currColIndex];
                    //Create the Empty splitted cell Widget 
                    if (widget == null)
                    {
                        widget = new SplitWidgetContainer(TableWidget.GetCellWidget(CurrRowIndex, m_currColIndex) as IWidgetContainer);
                    }
                }

                if (widget == null)
                {
                    widget = TableWidget.GetCellWidget(CurrRowIndex, m_currColIndex);
                }

                return LayoutContext.Create(widget, m_lcOperator, (float)m_layoutArea.Width);
            }

            return null;
        }

        /// <summary>
        /// Commits the cell context.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        private void SaveChildContextState(LayoutContext childContext)
        {
            switch (childContext.State)
            {
                case LayoutState.Unknown:
                    m_currRowLW.ChildWidgets.Add(m_currCellLW);
                    m_currCellLW.Owner = m_currRowLW;
                    m_bAtLastOneCellFitted = true;
                    break;
                case LayoutState.NotFitted:
                    MarkAsNotFitted(childContext);
                    break;
                case LayoutState.Splitted:
                    MarkAsSplitted(childContext);
                    break;
                //case LayoutState.Fitting:
                case LayoutState.Fitted:
                    MarkAsFitted(childContext);
                    break;
                case LayoutState.Breaked:
                    MarkAsBreaked(childContext);
                    break;
            }
        }

        /// <summary>
        /// Marks as splitted.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        private void MarkAsSplitted(LayoutContext childContext)
        {
            TableLayoutInfo tableInfo = m_currRowLW.Widget.LayoutInfo as TableLayoutInfo;

            if (tableInfo.IsExactlyRowHeight || childContext.LayoutInfo.IsVerticalText)
            {
                MarkAsFitted(childContext);
                m_ltState = LayoutState.Unknown;
            }
            else
            {
                (m_currCellLW.Widget.LayoutInfo as TableLayoutInfo).IsRowSplitted = true;
                if (!(m_currCellLW.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart)
                    tableInfo.IsRowSplitted = true;
                MarkAsFitted(childContext);
                if (m_blastRowState == LayoutState.Unknown && !(m_currCellLW.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart)
                {
                    m_blastRowState = LayoutState.Splitted;
                    TableLayoutInfo.IsSplittedTable = true;
                }
            }
        }
        /// <summary>
        /// Marks as breaked.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected virtual void MarkAsBreaked(LayoutContext childContext)
        {
            TableLayoutInfo tableInfo = m_currRowLW.Widget.LayoutInfo as TableLayoutInfo;

            if (tableInfo.IsExactlyRowHeight)
            {
                m_splitedCells[m_currColIndex] = ((childContext.SplittedWidget as SplitWidgetContainer) != null)
                                                     ? childContext.SplittedWidget as SplitWidgetContainer
                                                     : new SplitWidgetContainer(childContext.Widget as IWidgetContainer);
                (m_splitedCells[m_currColIndex].LayoutInfo as TableLayoutInfo).IsRowSplitted = true;
            }

            m_currRowLW.ChildWidgets.Add(m_currCellLW);
            m_currCellLW.Owner = m_currRowLW;
            if (tableInfo.IsRowMergeStart)
            {
                m_rowsHeight[m_currColIndex] = m_currCellLW.Bounds.Height
                                                 + tableInfo.Margins.Bottom
                                                 + tableInfo.Paddings.Bottom
                                                 + tableInfo.Paddings.Top;
                m_mergedRowIndex[m_currColIndex] = m_ltWidget.ChildWidgets.Count;
            }
            else if (!tableInfo.IsRowMergeContinue)
            {
                UpdateRowLWBounds(childContext);
            }
            m_bAtLastOneCellFitted = true;
            m_blastRowState = LayoutState.Breaked;
        }
        /// <summary>
        /// Marks as not fitted.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        private void MarkAsNotFitted(LayoutContext childContext)
        {
            CommitKeepWithNext();
            if (childContext.IsVerticalNotFitted)
            {
                TableLayoutInfo tableInfo = m_currRowLW.Widget.LayoutInfo as TableLayoutInfo;

                if (tableInfo.IsExactlyRowHeight)
                {
                    if (m_currCellLW.Bounds.Height == 0)
                    {
                        RectangleF bounds = m_currCellLW.Bounds;
                        bounds.Height = (float)Math.Abs(tableInfo.RowHeight);
                        m_currCellLW.Bounds = bounds;
                    }

                    MarkAsFitted(childContext);
                    //m_ltState = LayoutState.Fitted;
                }
                else
                {
                    m_ltState = LayoutState.NotFitted;
                }
            }
            else
            {
                if (CurrRowIndex < (TableWidget.RowsCount - 1))
                {
                    //CommitForFitted( childContext );
                    MarkAsSplitted(childContext);
                    m_ltState = LayoutState.NotFitted;
                }
                else
                {
                    //m_bIsHorizontalNotFitted = true;
                    m_ltState = LayoutState.NotFitted;
                    //Trace.WriteLine( "last row" );
                }
            }
        }

        /// <summary>
        /// Commits the keep with next.
        /// </summary>
        private void CommitKeepWithNext()
        {
            bool isKeepWithNext = IsNeedToCommitKeepWithNext();
            while (m_ltWidget.ChildWidgets.Count > 0 && !(m_lcOperator as Layouter).IsLayoutingHeaderFooter && isKeepWithNext)
            {
                LayoutedWidget ltWidget = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1];
                if (ltWidget.Widget.LayoutInfo.IsKeepWithNext && Math.Round(ltWidget.Bounds.Y - m_headerRowHeight, 2) != Math.Round((m_lcOperator as Layouter).PageTopMargin, 2))
                {
                    m_ltWidget.ChildWidgets.RemoveAt(m_ltWidget.ChildWidgets.Count - 1);
                    //Clear the SplittedCells when the row widget removed from TableWidget.
                    if (m_splitedCells != null && m_splitedCells.Length > 0)
                        Array.Clear(m_splitedCells, 0, m_splitedCells.Length);
                    m_currRowIndex -= 1;
                }
                else
                    break;
            }
        }
        /// <summary>
        /// Determine whether current row widget is need to commit with keeepWithNext property
        /// </summary>
        /// <returns></returns>
        private bool IsNeedToCommitKeepWithNext()
        {
            bool isKeepWithNext = false;
            for (int i = 0; i < m_ltWidget.ChildWidgets.Count; i++)
            {
                if (!(m_ltWidget.ChildWidgets[i].Widget.LayoutInfo.IsKeepWithNext))
                {
                    isKeepWithNext = true;
                    break;
                }
            }
            if (!isKeepWithNext && m_ltWidget.ChildWidgets.Count > 0
                && Math.Round(m_ltWidget.ChildWidgets[0].Bounds.Y, 2) != Math.Round((m_lcOperator as Layouter).PageTopMargin, 2)
                && (m_ltWidget.ChildWidgets[0].Widget as WTableRow) == m_table.Rows[0]
                && !(m_table.TableFormat.WrapTextAround || (m_table.OwnerTextBody is WTableCell)))
            {
                bool isAllItemHavingKeepWihtNext = false;
                Entity ent = m_table.PreviousSibling as Entity;
                while (ent != null && !((ent is WTable) && (ent as WTable).TableFormat.WrapTextAround))
                {
                    if (!(ent as TextBodyItem).m_layoutInfo.IsKeepWithNext)
                    {
                        isAllItemHavingKeepWihtNext = false;
                        isKeepWithNext = true;
                        break;
                    }
                    else
                        isAllItemHavingKeepWihtNext = true;

                    if ((ent as TextBodyItem).m_layoutInfo.IsFirstItemInPage)
                    {
                        isAllItemHavingKeepWihtNext = false;
                        if (!Syncfusion.DocIO.DLS.Rendering.DocumentLayouter.IsFirstLayouting)
                            isKeepWithNext = (ent as TextBodyItem).m_layoutInfo.IsKeepWithNext;
                        break;
                    }

                    ent = ent.PreviousSibling as Entity;
                }
                Entity ownerEnt = GetBaseEntity(this.Widget as Entity);
                WSection section = (ownerEnt != null) ? ownerEnt as WSection : null;
                //Check whether the current section greater than 1 and page contains the continues break and all text body items having keepwithnext property true.
                //if all condition true means keepwithnext property true otherwise false
                if (section != null && section.BreakCode == SectionBreakCode.NoBreak && section.GetIndexInOwnerCollection() > 0 && isAllItemHavingKeepWihtNext)
                    isKeepWithNext = true;
            }
            return isKeepWithNext;
        }
        /// <summary>
        /// Marks as fitted.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        private void MarkAsFitted(LayoutContext childContext)
        {
            if (!(m_currRowLW.Widget.LayoutInfo as TableLayoutInfo).IsExactlyRowHeight)
            {
                m_splitedCells[m_currColIndex] = ((childContext.SplittedWidget as SplitWidgetContainer) != null)
                                                     ? childContext.SplittedWidget as SplitWidgetContainer
                                                     : new SplitWidgetContainer(childContext.Widget as IWidgetContainer);
            }

            m_currRowLW.ChildWidgets.Add(m_currCellLW);
            m_currCellLW.Owner = m_currRowLW;
            TableLayoutInfo tableInfo = m_currCellLW.Widget.LayoutInfo as TableLayoutInfo;

            if (tableInfo.IsRowMergeStart)
            {
                m_rowsHeight[m_currColIndex] = m_currCellLW.Bounds.Height
                                                 + tableInfo.Margins.Bottom
                                                 + tableInfo.Paddings.Bottom
                                                 + tableInfo.Paddings.Top;
                m_mergedRowIndex[m_currColIndex] = m_ltWidget.ChildWidgets.Count;
            }
            else if (!tableInfo.IsRowMergeContinue)
            {
                UpdateRowLWBounds(childContext);
            }

            //m_ltState = LayoutState.Fitting;
            m_bAtLastOneCellFitted = true;
        }

        /// <summary>
        /// Updates the client area.
        /// </summary>
        private void UpdateClientArea()
        {
            m_layoutArea.CutFromTop(m_currRowLW.Bounds.Bottom);
        }

        /// <summary>
        /// Gets the cell client area.
        /// </summary>
        /// <param name="horMergeStart">if set to <c>true</c> [hor merge start].</param>
        /// <returns></returns>
        private LayoutArea GetCellClientArea(bool horMergeStart, int rowIndex, int columnIndex)
        {
            RectangleF rect = m_layoutArea.ClientActiveArea;
            WTableCell prev;
            float prev_width = 0.0f;
            double cellX, cellWidth;
            float TableWidth = m_table.Width;
            cellWidth = GetCellWidth(rowIndex, columnIndex);

            //Updates the Grid before width of the current row.
            if (m_table.Rows[rowIndex].RowFormat.GridBeforeWidth.WidthType == FtsWidth.Point
                && m_table.Rows[rowIndex].RowFormat.GridBeforeWidth.Width != 0)
            {
                float diff = (float)(m_table.Rows[rowIndex].RowFormat.GridBeforeWidth.Width - m_table.TableFormat.Paddings.Left);
                prev_width += diff;
            }
            else if (m_table.Rows[rowIndex].RowFormat.GridBeforeWidth.WidthType == FtsWidth.Percentage
                && m_table.Rows[rowIndex].RowFormat.GridBeforeWidth.Width != 0)
            {
                float diff = (float)((TableWidth * (m_table.Rows[rowIndex].RowFormat.GridBeforeWidth.Width / DLSConstants.HundredthsUnit)) - m_table.TableFormat.Paddings.Left);
                prev_width += diff;
            }
            for (int i = 0; i < columnIndex; i++)
            {
                prev = m_table.Rows[rowIndex].Cells[i] as WTableCell;
                if (prev.CellFormat.HorizontalMerge != CellMerge.Continue)
                    prev_width += (float)(prev.m_layoutInfo as TableLayoutInfo).CellWidth;
            }

            if (prev_width == 0.0f)
            {
                cellX = rect.X;
            }
            else
            {
                cellX = prev_width + rect.X;
                if (m_table.Rows[rowIndex].Cells.Count == columnIndex + 1)
                {
                    if (m_table.Width < Convert.ToSingle(cellX) + Convert.ToSingle(cellWidth))
                    {
                        float diff = Convert.ToSingle(TableWidth - (prev_width + cellWidth));
                        if (diff < 0)
                            cellWidth += diff;
                    }

                    if (m_table.Width > Convert.ToSingle(cellX) + Convert.ToSingle(cellWidth))
                    {
                        float diff = Convert.ToSingle(TableWidth - (prev_width + cellWidth));
                        if (cellWidth > diff && diff < 0)
                            cellWidth -= diff;
                    }
                }
            }
            double cellY = rect.Y;
            double cellHeight = rect.Height;

            if (horMergeStart && m_table.Rows[rowIndex].Cells[columnIndex].CellFormat.HorizontalMerge == CellMerge.Start)
                cellWidth = GetCellMergedWidth(rowIndex, columnIndex);

            TableLayoutInfo rowTableInfo = m_currRowLW.Widget.LayoutInfo as TableLayoutInfo;

            bool isExactlyRowHeight = rowTableInfo.IsExactlyRowHeight;

            if (isExactlyRowHeight)
            {
                // TODO: add cell spacing
                cellHeight = rowTableInfo.RowHeight;
            }
            else
            {
                if (cellHeight < rowTableInfo.RowHeight)
                {
                    // Sets minimum height of the row.
                    cellHeight = rowTableInfo.RowHeight;
                }
            }
            if (cellHeight > rect.Height
                && m_table.OwnerTextBody is WTableCell
                && (((m_table.OwnerTextBody as WTableCell).OwnerRow.m_layoutInfo as TableLayoutInfo).IsExactlyRowHeight
                || rowIndex == 0))
                cellHeight = rect.Height;
            RectangleF cellRect =
              new RectangleF((float)cellX, (float)cellY, (float)cellWidth, (float)cellHeight);
            //Update cell width
            (m_table.Rows[rowIndex].Cells[columnIndex].m_layoutInfo as TableLayoutInfo).CellWidth = cellRect.Width;
            if (m_table.Rows[rowIndex].Cells[columnIndex].CellFormat.TextDirection != TextDirection.Horizontal)
            {
                float height = DrawingContext.MeasureString(" ", m_table.Rows[rowIndex].Cells[columnIndex].LastParagraph.BreakCharacterFormat.Font, null, m_table.Rows[rowIndex].Cells[columnIndex].LastParagraph.BreakCharacterFormat,false).Height;
                ILayoutSpacingsInfo spacings = m_table.Rows[rowIndex].Cells[columnIndex].m_layoutInfo as ILayoutSpacingsInfo;
                float topPad = (float)(spacings.Margins.Top + spacings.Paddings.Top);
                float bottomPad = (float)(spacings.Paddings.Bottom + spacings.Margins.Bottom);
                if (!(m_table.Rows[rowIndex].m_layoutInfo as TableLayoutInfo).IsExactlyRowHeight && (m_table.Rows[rowIndex].m_layoutInfo as TableLayoutInfo).IsVerticalText)
                {
                    if (m_table.Rows[rowIndex].RowFormat.Height == 0)
                    {
                        cellRect = new RectangleF((float)cellX, (float)cellY, (float)height + topPad + bottomPad, (float)cellWidth);
                    }
                    else
                        cellRect = new RectangleF((float)cellX, (float)cellY, (float)Math.Abs(m_table.Rows[rowIndex].RowFormat.Height) + topPad + bottomPad, (float)cellWidth);
                }
                else if ((m_table.Rows[rowIndex].m_layoutInfo as TableLayoutInfo).IsExactlyRowHeight)
                    cellRect = new RectangleF((float)cellX, (float)cellY, (float)Math.Abs(m_table.Rows[rowIndex].RowFormat.Height), (float)cellWidth);
                else
                    cellRect = new RectangleF((float)cellX, (float)cellY, (float)cellHeight, (float)cellWidth);
                //Update Vertical cell width
                (m_table.Rows[rowIndex].Cells[columnIndex].m_layoutInfo as TableLayoutInfo).VerticalCellWidth = cellRect.Width;
            }
            return new LayoutArea(cellRect);
        }
        /// <summary>
        /// Gets the width of the cell.
        /// </summary>
        /// <param name="rowIndex">Row index</param>
        /// <param name="colIndex">Column index</param>
        /// <returns></returns>
        private float GetCellWidth(int rowIndex, int colIndex)
        {
            float width = m_table.Rows[rowIndex].Cells[colIndex].Width;
            // Updates the spacing as cell width according to MS Word behavior, if cell width is "0".
            if (width == 0)
                width = (float)((m_table.Rows[rowIndex].Cells[colIndex] as IWidget).LayoutInfo.Paddings.Left
                    + (m_table.Rows[rowIndex].Cells[colIndex] as IWidget).LayoutInfo.Paddings.Right
                    + (m_table.Rows[rowIndex].Cells[colIndex] as IWidget).LayoutInfo.Margins.Left
                    + (m_table.Rows[rowIndex].Cells[colIndex] as IWidget).LayoutInfo.Margins.Right);
            return width;
        }
        /// <summary>
        /// Gets the Height of the cell.
        /// </summary>
        /// <param name="rowIndex">Row index</param>
        /// <param name="colIndex">Column index</param>
        /// <returns></returns>
        private float GetCellHeight(int rowIndex, int colIndex, float cellMinHeight)
        {
            float height = m_table.Rows[rowIndex].Height;
            if (height <= 0)
                height = cellMinHeight;
            height += (float)((m_table.Rows[rowIndex].Cells[colIndex] as IWidget).LayoutInfo.Paddings.Top
            + (m_table.Rows[rowIndex].Cells[colIndex] as IWidget).LayoutInfo.Paddings.Bottom
            + (m_table.Rows[rowIndex].Cells[colIndex] as IWidget).LayoutInfo.Margins.Top
            + (m_table.Rows[rowIndex].Cells[colIndex] as IWidget).LayoutInfo.Margins.Bottom);
            return height;
        }
        /// <summary>
        /// Gets the width of the merged cell.
        /// </summary>
        /// <returns></returns>
        private float GetCellMergedWidth(int rowIndex, int colIndex)
        {
            float cellWidth = m_table.Rows[rowIndex].Cells[colIndex].Width;
            WTableCell nextCell = null;
            int index = colIndex + 1;
            if (index < m_table.Rows[rowIndex].Cells.Count)
                nextCell = m_table.Rows[rowIndex].Cells[index];
            while (nextCell != null && nextCell.CellFormat.HorizontalMerge == CellMerge.Continue)
            {
                cellWidth += GetCellWidth(rowIndex, index);
                index++;
                if (index < m_table.Rows[rowIndex].Cells.Count)
                    nextCell = m_table.Rows[rowIndex].Cells[index];
                else
                    nextCell = null;
            }
            return cellWidth;
        }
        /// <summary>
        /// Updates the LW bounds.
        /// </summary>
        private void UpdateLWBounds()
        {
            RectangleF bounds = m_ltWidget.Bounds;
            bounds.Width = m_currRowLW.Bounds.Width +
              (float)(TableWidget.LayoutInfo.Paddings.Left +
              TableWidget.LayoutInfo.Paddings.Right);
            bounds.Height = m_currRowLW.Bounds.Bottom - bounds.Top
                            + (float)TableWidget.LayoutInfo.Paddings.Bottom;
            m_ltWidget.Bounds = bounds;
        }

        /// <summary>
        /// Searcheds the merge start.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="coll">The collection.</param>
        /// <returns></returns>
        private bool SearchedMergeStart(ref int row, ref int coll)
        {
            bool next = row > 0;
            int rowIndex = row;
            int cellIndex = coll;
            int index = row;
            while (next)
            {
                LayoutedWidget rowLW = m_ltWidget.ChildWidgets[rowIndex - 1];
                int columnIndex = GetAdjacentCellIndex(index, cellIndex, rowIndex - 1);
                if (rowLW.ChildWidgets.Count - 1 <= columnIndex
                    || (rowLW.ChildWidgets[columnIndex].Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart)
                {
                    row = rowIndex - 1;
                    coll = columnIndex;
                    if (rowLW.ChildWidgets.Count > columnIndex && (rowLW.ChildWidgets[columnIndex].Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart)
                        return true;
                }

                rowIndex--;
                next = rowIndex > 0;
            }

            return false;
        }

        /// <summary>
        /// Gets the index of the adjacent cell.
        /// </summary>
        /// <param name="curRowIndex">The curRowIndex.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <param name="adjRowIndex">Index of the adj row.</param>
        /// <returns></returns>
        private int GetAdjacentCellIndex(int curRowIndex, int cellIndex, int adjRowIndex)
        {
            int adjCellIndex = cellIndex;
            float cellStartPos = m_ltWidget.ChildWidgets[curRowIndex].ChildWidgets[cellIndex].Bounds.X;
            float adjCellStartPos = 0;
            for (int i = 0; i < m_ltWidget.ChildWidgets[adjRowIndex].ChildWidgets.Count; i++)
            {
                adjCellStartPos = m_ltWidget.ChildWidgets[adjRowIndex].ChildWidgets[i].Bounds.X;
                if (cellStartPos <= adjCellStartPos)
                    return i;
            }
            return adjCellIndex;
        }
        /// <summary>
        /// Corrects the table client area.
        /// </summary>
        /// <param name="rect">The rect.</param>
        private void CorrectTableClientArea(ref RectangleF rect)
        {
            ITableLayoutInfo info = TableLayoutInfo;
            bool isFixed = false;
            float fixedSize = 0;
            int fixedCount = 0;

            for (int i = 0, count = info.IsDefaultCells.Length; i < count; i++)
            {
                if (info.IsDefaultCells[i])
                {
                    isFixed |= true;
                    fixedSize += info.CellsWidth[i];
                    fixedCount++;
                }
            }

            if (!isFixed && info.Width > rect.Width)
            {
                info.Width = rect.Width - (float)info.CellSpacings - (float)info.CellPaddings;
            }
            else
            {
                if (fixedCount == m_table.Rows[CurrRowIndex + 1].Cells.Count)
                {
                    info.Width = 0;

                    for (int i = 0; i < fixedCount; i++)
                    {
                        info.Width += info.CellsWidth[i];
                    }
                }

            }

            RowFormat table_format = m_table.TableFormat;
            MarginsF pageMargins = InitializePageMargins();
            Entity ent = GetBaseEntity(m_table);
            //rect.Width -= table_format.Paddings.Right;
            if (!m_table.m_isTextBox)
            {
                if ((m_table.IndentFromLeft != float.MinValue && table_format.HorizontalAlignment == RowAlignment.Left))
                {
                    if (m_table.TableFormat.WrapTextAround)
                    {
                        if (m_table.TableFormat.Positioning.HorizPositionAbs == HorizontalPosition.Left)
                        {
                            if (m_table.OwnerTextBody is WTableCell
                                && rect.Width > info.Width)
                                rect.X += m_table.IndentFromLeft;
                            else
                                rect.X += m_table.IndentFromLeft - LeftPad;
                        }
                        else
                            rect.X += m_table.IndentFromLeft;
                    }
                    else
                    {
                        if ((m_table.OwnerTextBody is WTableCell
                        && !(m_table.OwnerTextBody as WTableCell).OwnerRow.OwnerTable.m_isTextBox))
                            rect.X += m_table.IndentFromLeft;
                        else
                            rect.X += m_table.IndentFromLeft - LeftPad;
                    }
                }
            }
            if (!m_table.m_isTextBox && !((ent is WSection) && (ent as WSection).Columns.Count > 1))
            {
                if (m_table.TableFormat.WrapTextAround
                    && m_table.TableFormat.Positioning.HorizPositionAbs == HorizontalPosition.Right
                    && m_table.TableFormat.Positioning.HorizRelationTo == HorizontalRelation.Page
                    && !(m_table.OwnerTextBody is WTableCell)
                    && pageMargins != null)
                {
                    if ((info.Width + table_format.Paddings.Right) < pageMargins.Right)
                        rect.X = (m_lcOperator as Layouter).ClientLayoutArea.Width + pageMargins.Left;
                    else
                        rect.X = (m_lcOperator as Layouter).ClientLayoutArea.Width + pageMargins.Left + pageMargins.Right - info.Width - table_format.Paddings.Right;
                }
                else if (m_table.TableFormat.WrapTextAround
                         && m_table.TableFormat.Positioning.HorizPositionAbs == HorizontalPosition.Left
                         && m_table.TableFormat.Positioning.HorizPosition == 0
                         && m_table.TableFormat.Positioning.HorizRelationTo == HorizontalRelation.Page
                         && !(m_table.OwnerTextBody is WTableCell)
                         && pageMargins != null)
                {
                    if ((info.Width + table_format.Paddings.Left + m_table.IndentFromLeft) < pageMargins.Left)
                        rect.X = pageMargins.Left - info.Width - table_format.Paddings.Left - m_table.IndentFromLeft;
                    else
                        rect.X = m_table.IndentFromLeft;
                }
                else if ((table_format.HorizontalAlignment == RowAlignment.Right || (m_table.TableFormat.WrapTextAround && m_table.TableFormat.Positioning.HorizPositionAbs == Syncfusion.DocIO.HorizontalPosition.Right)))
                {
                    float rightPad = GetRightPad(m_table.Rows[0].Cells[m_table.Rows[0].Cells.Count - 1]);
                    //Updates the layout area for inline table based on the first row last cell - right padding.
                    rect.X += rect.Width - (info.Width - rightPad);
                    //Updates the layout area for absolute positioned table based on the minimum right padding.
                    if (m_table.TableFormat.WrapTextAround)
                        rect.X += GetMinimumRightPad() - rightPad;
                }
                else if ((table_format.HorizontalAlignment == RowAlignment.Center || (m_table.TableFormat.WrapTextAround && m_table.TableFormat.Positioning.HorizPositionAbs == Syncfusion.DocIO.HorizontalPosition.Center)))
                {
                    rect.X += (rect.Width - info.Width) / 2;
                }
            }
            rect.Width = info.Width;

            float cellWidth = (info.Width - fixedSize) / (info.CellsWidth.Length - fixedCount);

            for (int i = 0, count = info.IsDefaultCells.Length; i < count; i++)
            {
                if (!info.IsDefaultCells[i])
                {
                    info.CellsWidth[i] = cellWidth;
                }
            }
        }
        /// <summary>
        /// Get Minimum right padding of the table.
        /// </summary>
        /// <returns></returns>
        private float GetMinimumRightPad()
        {
            int cellsCount = m_table.Rows[0].Cells.Count;
            float minRightPad = GetRightPad(m_table.Rows[0].Cells[cellsCount - 1]);
            for (int i = 1; i < m_table.Rows.Count; i++)
            {
                cellsCount = m_table.Rows[i].Cells.Count;
                float rightPad = GetRightPad(m_table.Rows[i].Cells[cellsCount - 1]);
                if (minRightPad > rightPad)
                {
                    minRightPad = rightPad;
                }
            }
            return minRightPad;
        }
        /// <summary>
        /// Get Right padding of the cell
        /// </summary>
        /// <param name="tableCell"></param>
        /// <returns></returns>
        private float GetRightPad(WTableCell tableCell)
        {
            float rightPad = tableCell.CellFormat.Paddings.Right;
            if (tableCell.CellFormat.SamePaddingsAsTable || rightPad == -0.05f)
            {
                //Right
                if (tableCell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
                {
                    rightPad = tableCell.OwnerRow.RowFormat.Paddings.Right;
                }
                else if (tableCell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
                {
                    rightPad = tableCell.OwnerRow.OwnerTable.TableFormat.Paddings.Right;
                }
                //Doc format document can have default cell margin value as zero.
                else if (tableCell.Document.ActualFormatType == FormatType.Doc)
                    rightPad = 0.0f;
                else
                    rightPad = 5.4f;
            }
            return rightPad;
        }
        #endregion
    }
}

#endif
