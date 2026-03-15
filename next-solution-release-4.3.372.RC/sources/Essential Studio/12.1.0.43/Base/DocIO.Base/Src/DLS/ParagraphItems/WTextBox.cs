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

#region File using directives
using System;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WTextBox.
    /// </summary>
    public class WTextBox
    : ParagraphItem
    , IWTextBox
#if !SILVERLIGHT && !WP
    , IWidget
#endif
    
   
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected WTextBody m_textBody;
        protected WTextBoxFormat m_txbxFormat;
        private int m_txbxSpid;
        private WTable m_table;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the child entities.
        /// </summary>
        /// <value>The child entities.</value>
        public EntityCollection ChildEntities
        {
            get
            {
                return m_textBody.ChildEntities;
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
                return EntityType.TextBox;
            }
        }
        /// <summary>
        /// Get/set TextBoxFormat value
        /// </summary>
        public WTextBoxFormat TextBoxFormat
        {
            get
            {
                return m_txbxFormat;
            }
            set
            {
                m_txbxFormat = value;
            }
        }
        /// <summary>
        /// Get/set TextBody value
        /// </summary>
        public WTextBody TextBoxBody
        {
            get
            {
                return m_textBody;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int TextBoxSpid
        {
            get
            {
                return m_txbxSpid;
            }
            set
            {
                m_txbxSpid = value;
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
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="doc"></param>
        public WTextBox(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_charFormat = new WCharacterFormat(doc);
            m_charFormat.SetOwner(this);
            m_txbxFormat = new WTextBoxFormat(Document);
            m_txbxFormat.SetOwner(this);
            m_textBody = new WTextBody(Document, this);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            if (m_textBody != null)
                m_textBody.AddSelf();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            TextBoxBody.CloneRelationsTo(doc, nextOwner);

            if ((nextOwner.OwnerBase != null && nextOwner.OwnerBase is HeaderFooter) ||
                nextOwner is HeaderFooter)
            {
                this.TextBoxFormat.IsHeaderTextBox = true;
                TextBoxFormat.CloneRelationsTo(doc, nextOwner);
            }

            Document.CloneShapeEscher(doc, this);
            this.Cloned = false;
        }       
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WTextBox tb = (WTextBox)base.CloneImpl();
            tb.m_textBody = (WTextBody)TextBoxBody.Clone();

            for (int i = 0, count = tb.m_textBody.Items.Count; i < count; i++)
            {
                (tb.m_textBody.Items[i] as TextBodyItem).SetOwner(tb.m_textBody);
            }
            tb.m_txbxFormat = (WTextBoxFormat)TextBoxFormat.Clone();

            tb.m_textBody.SetOwner(tb);
            tb.m_txbxFormat.SetOwner(tb);
            tb.Cloned = true;
            return tb;
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Horizontal);
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="itemPos"></param>
        internal override void Attach(WParagraph owner, int itemPos)
        {
            base.Attach(owner, itemPos);
            Document.TextBoxes.Add(this);
        }
        /// <summary>
        /// Detaches from owner.
        /// </summary>
        internal override void Detach()
        {
            Document.TextBoxes.Remove(this);
        }
        /// <summary>
        /// Gets the next text body item.
        /// </summary>
        /// <returns></returns>
        internal TextBodyItem GetNextTextBodyItem()
        {
            if (this.OwnerParagraph != null)
            {
                return this.OwnerParagraph.GetNextTextBodyItem();
            }

            return null;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();
            if (m_textBody != null)
            {
                m_textBody.Close();
                m_textBody = null;
            }
            m_txbxFormat = null;
        }
        /// <summary>
        /// Sets the text body.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        internal void SetTextBody(WTextBody textBody)
        {
            m_textBody = textBody;
        }
        #endregion

        #region Helper Methods
#if (!SILVERLIGHT && !WP) || WINRT
        /// <summary>
        /// Gets as table.
        /// </summary>
        /// <returns></returns>
        internal WTable GetAsTable(int currPageIndex)
        {
            float leftMargin = 0.0f, topMargin = 0.0f, bottomMargin = 0.0f,headerDistance=0.0f,footerDistance=0.0f, rightMargin = 0.0f;
            Color bkColor = new Color();
            m_table = new WTable(this.Document);
            m_table.ResetCells(1, 1);
            m_table.TableFormat.HorizontalAlignment = GetHorAlign(TextBoxFormat.HorizontalAlignment);
            m_table.Rows[0].Cells[0].CellFormat.TextDirection = TextBoxFormat.TextDirection;
            bkColor = TextBoxFormat.FillColor;
            float width = TextBoxFormat.Width;
            float height = TextBoxFormat.Height;
            //Get width and height relative to percent
            if (TextBoxFormat.WidthRelativePercent != 0)
                width = TextBoxFormat.GetWidthRelativeToPercent();
            if (TextBoxFormat.HeightRelativePercent != 0)
                height = TextBoxFormat.GetHeightRelativeToPercent();
            float pageWidth = 0.0f;
            float pageHeight = 0.0f;
            float pageClientWidth = 0.0f,pageClientHeight=0.0f;
            bool isSingleColumn = true;
            WSection currentSection = new WSection(this.Document);
            if (this.Owner != null)
            {
                Entity ent = this.Owner as Entity;
                while (!(ent is WSection))
                {
                    if (ent is WTable)
                    {
                        m_table.m_isTextBoxInTable = true;
                        break;
                    }
                    if (ent.Owner == null)
                        break;
                    else
                        ent = ent.Owner as Entity;
                }
                if (ent is WSection)
                {
                    currentSection = ent as WSection;
                    leftMargin = currentSection.PageSetup.Margins.Left;
                    rightMargin =  currentSection.PageSetup.Margins.Right;
                    topMargin = ((currentSection.PageSetup.Margins.Top > 0) ? currentSection.PageSetup.Margins.Top : 36);
                    bottomMargin = ((currentSection.PageSetup.Margins.Bottom > 0) ? currentSection.PageSetup.Margins.Bottom : 36);
                    pageHeight = currentSection.PageSetup.PageSize.Height;
                    pageWidth = currentSection.PageSetup.PageSize.Width;
                    pageClientWidth = currentSection.PageSetup.ClientWidth;
                    pageClientHeight = currentSection.PageSetup.PageSize.Height - (headerDistance + footerDistance);
                    footerDistance = currentSection.PageSetup.FooterDistance;
                    headerDistance = currentSection.PageSetup.HeaderDistance;
                    if (currentSection.Columns.Count > 1)
                        isSingleColumn = false;
                }
            }
            WTableRow tableRow = m_table.Rows[0] as WTableRow;
            WTableCell tableCell = tableRow.Cells[0] as WTableCell;
            tableRow.Height = height;

            if (!TextBoxFormat.NoLine)
            {
                //Update border width
                float lineWidth = TextBoxFormat.LineWidth;
                //Update double border width
                if (TextBoxFormat.LineStyle == TextBoxLineStyle.Double)
                    lineWidth /= 3;
                //Update Triple border width
                else if (TextBoxFormat.LineStyle == TextBoxLineStyle.Triple)
                    lineWidth /= 5;
                tableRow.RowFormat.Borders.LineWidth = lineWidth;
                tableCell.CellFormat.Borders.LineWidth = lineWidth;
                tableRow.RowFormat.Borders.Color = TextBoxFormat.LineColor;
                tableCell.CellFormat.Borders.Color = TextBoxFormat.LineColor;
                tableRow.RowFormat.Borders.BorderType = GetBordersStyle(TextBoxFormat.LineStyle);
                tableCell.CellFormat.Borders.BorderType = GetBordersStyle(TextBoxFormat.LineStyle);
            }
            else
            {
                tableCell.CellFormat.Borders.BorderType = BorderStyle.None;
                tableRow.RowFormat.Borders.BorderType = BorderStyle.None;
            }
            if (TextBoxFormat.TextWrappingStyle != TextWrappingStyle.Inline)
            {
                switch (TextBoxFormat.VerticalOrigin)
                {
                    case VerticalOrigin.Page:
                    case VerticalOrigin .TopMargin:
                        {
                            m_table.TableFormat.Positioning.VertRelationTo = VerticalRelation.Page;
                            switch (TextBoxFormat.VerticalAlignment)
                            {
                                case ShapeVerticalAlignment.Top:
                                    m_table.TableFormat.Positioning.VertPosition -= TextBoxFormat.InternalMargin.Top ;                            
                                    break;
                                case ShapeVerticalAlignment.Center:
                                    m_table.TableFormat.Positioning.VertPosition = (pageHeight - height) / 2;
                                    break;
                                case ShapeVerticalAlignment.Bottom:
                                    m_table.TableFormat.Positioning.VertPosition = pageHeight - height - TextBoxFormat.InternalMargin.Bottom ;                                                                      
                                    break;
                                case ShapeVerticalAlignment.None:
                                    if (Math.Abs(TextBoxFormat.VerticalRelativePercent) <= 1000)
                                        m_table.TableFormat.Positioning.VertPosition = pageHeight * (TextBoxFormat.VerticalRelativePercent / 100);
                                    else
                                        m_table.TableFormat.Positioning.VertPosition = TextBoxFormat.VerticalPosition;
                                    break;
                            }                   
                        }
                        break;
                    case VerticalOrigin.Line:
                    case VerticalOrigin.Paragraph :
                        {
                            m_table.TableFormat.Positioning.VertRelationTo = VerticalRelation.Paragraph;
                            m_table.TableFormat.Positioning.VertPosition = TextBoxFormat.VerticalPosition;                                                          
                        }
                        break;
                    case VerticalOrigin.Margin :
                        {
                            m_table.TableFormat.Positioning.VertRelationTo = VerticalRelation.Margin ;
                            switch (TextBoxFormat.VerticalAlignment)
                            {
                                case ShapeVerticalAlignment.Top:
                                    if (Math.Abs(TextBoxFormat.VerticalRelativePercent) <= 1000)
                                        m_table.TableFormat.Positioning.VertPosition = (topMargin - TextBoxFormat.InternalMargin.Top) * (TextBoxFormat.VerticalRelativePercent / 100);
                                    else
                                        m_table.TableFormat.Positioning.VertPosition = TextBoxFormat.VerticalPosition - TextBoxFormat.InternalMargin.Top + topMargin;
                                    break;
                                case ShapeVerticalAlignment.Center:
                                    if (Math.Abs(TextBoxFormat.VerticalRelativePercent) <= 1000)
                                        m_table.TableFormat.Positioning.VertPosition = (pageClientHeight / 2) * (TextBoxFormat.VerticalRelativePercent / 100);
                                    else
                                        m_table.TableFormat.Positioning.VertPosition = (pageClientHeight - height) / 2;
                                    break;
                                case ShapeVerticalAlignment.Bottom:
                                    if (Math.Abs(TextBoxFormat.VerticalRelativePercent) <= 1000)
                                        m_table.TableFormat.Positioning.VertPosition = (pageClientHeight - TextBoxFormat.InternalMargin.Bottom - bottomMargin) * (TextBoxFormat.VerticalRelativePercent / 100);
                                    else
                                        m_table.TableFormat.Positioning.VertPosition = pageClientHeight - height - TextBoxFormat.InternalMargin.Bottom - bottomMargin ;
                                    break;
                                case ShapeVerticalAlignment.None:
                                    if (Math.Abs(TextBoxFormat.VerticalRelativePercent) <= 1000)
                                        m_table.TableFormat.Positioning.VertPosition = pageClientHeight * (TextBoxFormat.VerticalRelativePercent / 100);
                                    else
                                        m_table.TableFormat.Positioning.VertPosition = TextBoxFormat.VerticalPosition;
                                    break;
                            }
                        }
                        break;
                    default:
                        {
                            if (m_table.TableFormat.Positioning.VertPosition == 0.0f)
                                m_table.TableFormat.Positioning.VertPosition = TextBoxFormat.VerticalPosition;
                        }
                        break;
                }
                switch (TextBoxFormat.HorizontalOrigin)
                {
                    case HorizontalOrigin.Page:
                        {
                            m_table.TableFormat.Positioning.HorizRelationTo = HorizontalRelation.Page;
                            switch (TextBoxFormat.HorizontalAlignment)
                            {
                                case ShapeHorizontalAlignment.Center:
                                    m_table.TableFormat.Positioning.HorizPosition = (pageWidth - width) / 2;                           
                                    break;
                                case ShapeHorizontalAlignment.Left:
                                    m_table.TableFormat.Positioning.HorizPosition -= TextBoxFormat.InternalMargin.Left;
                                    break;
                                case ShapeHorizontalAlignment.Right:
                                    m_table.TableFormat.Positioning.HorizPosition = pageWidth - width - TextBoxFormat.InternalMargin.Right;
                                    break;
                                case ShapeHorizontalAlignment.None:
                                    if (Math.Abs(TextBoxFormat.HorizontalRelativePercent) <= 1000)
                                        m_table.TableFormat.Positioning.HorizPosition = pageWidth * (TextBoxFormat.HorizontalRelativePercent / 100);
                                    else
                                        m_table.TableFormat.Positioning.HorizPosition = TextBoxFormat.HorizontalPosition;
                                    break;                                   
                            }
                        }
                        break;
                    case HorizontalOrigin.Column:
                        {
                            m_table.TableFormat.Positioning.HorizRelationTo = HorizontalRelation.Column;
                            switch (TextBoxFormat.HorizontalAlignment)
                            {
                                case ShapeHorizontalAlignment.Center:
                                    m_table.TableFormat.Positioning.HorizPosition = (pageClientWidth - width) / 2;
                                    break;
                                case ShapeHorizontalAlignment.Left:
                                    m_table.TableFormat.Positioning.HorizPosition = m_table.TableFormat.LeftIndent - TextBoxFormat.InternalMargin.Left;
                                    break;
                                case ShapeHorizontalAlignment.Right:
                                    m_table.TableFormat.Positioning.HorizPosition = pageClientWidth - width - TextBoxFormat.InternalMargin.Right;
                                    break;
                                case ShapeHorizontalAlignment.None:
                                    m_table.TableFormat.Positioning.HorizPosition = TextBoxFormat.HorizontalPosition ;
                                    break;
                            }       
                         }
                        break;
                    case HorizontalOrigin.Margin:
                        {
                            m_table.TableFormat.Positioning.HorizRelationTo = HorizontalRelation.Margin;
                            switch (TextBoxFormat.HorizontalAlignment)
                            {
                                case ShapeHorizontalAlignment.Center:
                                    if (Math.Abs(TextBoxFormat.HorizontalRelativePercent) <= 1000)
                                        m_table.TableFormat.Positioning.HorizPosition = (pageClientWidth / 2) * (TextBoxFormat.HorizontalRelativePercent / 100);
                                    else
                                        m_table.TableFormat.Positioning.HorizPosition = (pageClientWidth - width) / 2;
                                    break;
                                case ShapeHorizontalAlignment.Left:
                                    if (Math.Abs(TextBoxFormat.HorizontalRelativePercent) <= 1000)
                                        m_table.TableFormat.Positioning.HorizPosition = (leftMargin - TextBoxFormat.InternalMargin.Left) * (TextBoxFormat.HorizontalRelativePercent / 100);
                                    else
                                        m_table.TableFormat.Positioning.HorizPosition = m_table.TableFormat.LeftIndent - TextBoxFormat.InternalMargin.Left;
                                    break;
                                case ShapeHorizontalAlignment.Right:
                                    if (Math.Abs(TextBoxFormat.HorizontalRelativePercent) <= 1000)
                                        m_table.TableFormat.Positioning.HorizPosition = (pageClientWidth - TextBoxFormat.InternalMargin.Right) * (TextBoxFormat.HorizontalRelativePercent / 100);
                                    else
                                        m_table.TableFormat.Positioning.HorizPosition = pageClientWidth - width - TextBoxFormat.InternalMargin.Right;
                                    break;
                                case ShapeHorizontalAlignment.None:
                                    if (Math.Abs(TextBoxFormat.HorizontalRelativePercent) <= 1000)
                                        m_table.TableFormat.Positioning.HorizPosition = pageClientWidth * (TextBoxFormat.HorizontalRelativePercent / 100);
                                    else
                                        m_table.TableFormat.Positioning.HorizPosition = TextBoxFormat.HorizontalPosition;
                                    break;
                            }  
                        }
                        break;
                    case HorizontalOrigin.LeftMargin:
                        m_table.TableFormat.Positioning.HorizPosition = GetLeftMarginHorizPosition(leftMargin, TextBoxFormat.HorizontalAlignment, TextBoxFormat.HorizontalPosition, width, TextBoxFormat.TextWrappingStyle);
                        break;
                    case HorizontalOrigin.RightMargin:
                        m_table.TableFormat.Positioning.HorizPosition = GetRightMarginHorizPosition(pageWidth, rightMargin, TextBoxFormat.HorizontalAlignment, TextBoxFormat.HorizontalPosition, width, TextBoxFormat.TextWrappingStyle);
                        break;
                    case HorizontalOrigin.InsideMargin:
                        if (currPageIndex % 2 == 0)
                            m_table.TableFormat.Positioning.HorizPosition = GetRightMarginHorizPosition(pageWidth, rightMargin, TextBoxFormat.HorizontalAlignment, TextBoxFormat.HorizontalPosition, width, TextBoxFormat.TextWrappingStyle);
                        else
                            m_table.TableFormat.Positioning.HorizPosition = GetLeftMarginHorizPosition(leftMargin, TextBoxFormat.HorizontalAlignment, TextBoxFormat.HorizontalPosition, width, TextBoxFormat.TextWrappingStyle);
                        break;
                    case HorizontalOrigin.OutsideMargin:
                        if (currPageIndex % 2 == 0)
                            m_table.TableFormat.Positioning.HorizPosition = GetLeftMarginHorizPosition(leftMargin, TextBoxFormat.HorizontalAlignment, TextBoxFormat.HorizontalPosition, width, TextBoxFormat.TextWrappingStyle);
                        else
                            m_table.TableFormat.Positioning.HorizPosition = GetRightMarginHorizPosition(pageWidth, rightMargin, TextBoxFormat.HorizontalAlignment, TextBoxFormat.HorizontalPosition, width, TextBoxFormat.TextWrappingStyle);
                        break;
                    default:
                        {
                            if (m_table.TableFormat.Positioning.VertPosition == 0.0f)
                                m_table.TableFormat.Positioning.VertPosition = TextBoxFormat.VerticalPosition;          
                        }
                        break;
                }

                if (TextBoxFormat.HorizontalOrigin != HorizontalOrigin.Page && TextBoxFormat.HorizontalOrigin != HorizontalOrigin.Column)
                    m_table.TableFormat.Positioning.HorizPosition += leftMargin;
            }

            if (TextBoxFormat.FillEfects.Type == BackgroundType.NoBackground)
            {
                if (TextBoxFormat.TextWrappingStyle == TextWrappingStyle.InFrontOfText)
                    bkColor = TextBoxFormat.FillColor;
                else
                    bkColor = Color.Transparent;
            }
            else if (TextBoxFormat.FillEfects.Type == BackgroundType.Gradient)
            {
                bkColor = TextBoxFormat.FillEfects.Gradient.Color2;
                tableCell.CellFormat.TextureStyle = TextureStyle.Texture30Percent;
            }

            m_table.TableFormat.BackColor = bkColor;
            //setting padding for the Textbox
            m_table.TableFormat.Paddings.Left = TextBoxFormat.InternalMargin.Left;
            m_table.TableFormat.Paddings.Right = TextBoxFormat.InternalMargin.Right;
            m_table.TableFormat.Paddings.Top = TextBoxFormat.InternalMargin.Top;
            m_table.TableFormat.Paddings.Bottom = TextBoxFormat.InternalMargin.Bottom;
            
            tableCell.Width = width;
            tableCell.CellFormat.BackColor = bkColor;
            tableCell.CellFormat.VerticalAlignment = TextBoxFormat.TextVerticalAlignment;
            m_table.Rows[0].HeightType = TableRowHeightType.Exactly;


            if (TextBoxFormat.LineWidth < 1)
                tableRow.RowFormat.Borders.BorderType = BorderStyle.None;

            for (int i = 0, count = this.TextBoxBody.Items.Count; i < count; i++)
            {
                TextBodyItem item = TextBoxBody.Items[i] as TextBodyItem;
                tableCell.Items.Add(item.Clone());
            }
            m_table.m_isTextBox = true;
            m_table.m_textBoxFormat = TextBoxFormat;
            return m_table;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageWidth"></param>
        /// <param name="rightMargin"></param>
        /// <param name="horzAlignment"></param>
        /// <param name="horzPosition"></param>
        /// <param name="shapeWidth"></param>
        /// <param name="textWrapStyle"></param>
        /// <returns></returns>
        private float GetRightMarginHorizPosition(float pageWidth, float rightMargin, ShapeHorizontalAlignment horzAlignment, float horzPosition, float shapeWidth, TextWrappingStyle textWrapStyle)
        {
            float xPosition = pageWidth - rightMargin;
            float indentX = xPosition + horzPosition;
            switch (horzAlignment)
            {
                case ShapeHorizontalAlignment.Center:
                    indentX = xPosition + (rightMargin - shapeWidth) / 2;
                    break;
                case ShapeHorizontalAlignment.Left:
                    indentX = xPosition;
                    break;
                case ShapeHorizontalAlignment.Right:
                    indentX = pageWidth - shapeWidth;
                    break;
                case ShapeHorizontalAlignment.None:
                    break;
            }
            if ((indentX < 0 || indentX + shapeWidth > pageWidth ) && textWrapStyle != TextWrappingStyle.InFrontOfText && textWrapStyle != TextWrappingStyle.Behind)
                indentX = pageWidth - shapeWidth;
            return indentX;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftMargin"></param>
        /// <param name="horzAlignment"></param>
        /// <param name="horzPosition"></param>
        /// <param name="shapeWidth"></param>
        /// <param name="textWrapStyle"></param>
        /// <returns></returns>
        private float GetLeftMarginHorizPosition(float leftMargin, ShapeHorizontalAlignment horzAlignment, float horzPosition, float shapeWidth, TextWrappingStyle textWrapStyle)
        {
            float indentX = horzPosition;
            switch (horzAlignment)
            {
                case ShapeHorizontalAlignment.Center:
                    indentX = (leftMargin - shapeWidth) / 2;
                    break;
                case ShapeHorizontalAlignment.Left:
                    indentX = 0;
                    break;
                case ShapeHorizontalAlignment.Right:
                    indentX = leftMargin - shapeWidth;
                    break;
                case ShapeHorizontalAlignment.None:
                    break;
            }
            if (indentX < 0 && textWrapStyle != TextWrappingStyle.InFrontOfText && textWrapStyle != TextWrappingStyle.Behind)
                indentX = 0;
            return indentX;
        }
#endif

        /// <summary>
        /// Gets the horizontal alignment.
        /// </summary>
        /// <param name="shapeAlign">The shape align.</param>
        /// <returns></returns>
        private RowAlignment GetHorAlign(ShapeHorizontalAlignment shapeAlign)
        {
            switch (shapeAlign)
            {
                case ShapeHorizontalAlignment.Center:
                    return RowAlignment.Center;
                case ShapeHorizontalAlignment.Right:
                    return RowAlignment.Right;
                default:
                    return RowAlignment.Left;
            }
        }

        /// <summary>
        /// Gets the border style.
        /// </summary>
        /// <param name="lineStyle">The line style.</param>
        /// <returns></returns>
        private BorderStyle GetBordersStyle(TextBoxLineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case TextBoxLineStyle.Simple:
                    return BorderStyle.Single;
                case TextBoxLineStyle.Double:
                    return BorderStyle.Double;
                case TextBoxLineStyle.ThickThin:
                    return BorderStyle.ThickThinMediumGap;
                case TextBoxLineStyle.ThinThick:
                    return BorderStyle.ThinThickMediumGap;
                case TextBoxLineStyle.Triple:
                    return BorderStyle.Triple;
                default:
                    return BorderStyle.None;
            }
        }
        #endregion

        #region IXDLSSerializable implement
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.TextBodyTag, TextBoxBody);
            XDLSHolder.AddElement(XDLSConstants.TextBoxFormatTag, TextBoxFormat);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.TypeTag, ParagraphItemType.TextBox);
        }
//#endif
        #endregion

        #region Impelementation/Layouting
#if !SILVERLIGHT && !WP
        #region IWidget Members

        ILayoutInfo IWidget.LayoutInfo
        {
            get 
            { 
                if(m_layoutInfo == null)
                    CreateLayoutInfo();
                
                return m_layoutInfo;
            }
        }

        void IWidget.Draw(Syncfusion.DocIO.Rendering.DrawingContext dc, LayoutedWidget ltWidget)
        {
            //m_table.DrawImpl(dc, ltWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }

        #endregion
#endif
        #endregion
    }
}
