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

#region file using directives
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Syncfusion.DocIO.Rendering;
using Syncfusion.DocIO.DLS;
using System;
using Syncfusion.DocIO;

#endregion

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Summary description for ILayoutedRange.
    /// </summary>
    internal class LayoutedWidget
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private RectangleF m_bounds = RectangleF.Empty;

        /// <summary>
        /// 
        /// </summary>
        private IWidget m_widget = null;

        /// <summary>
        /// 
        /// </summary>
        private LayoutedWidgetList m_ltWidgets = new LayoutedWidgetList();

        /// <summary>
        /// 
        /// </summary>
        private string m_textTag = null;
        /// <summary>
        /// 
        /// </summary>
        private float m_wordSpace = 0.0f;
        /// <summary>
        /// 
        /// </summary>
        private HorizontalAlignment m_horizontalAlign;
        /// <summary>
        /// 
        /// </summary>
        private float m_subWidth = 0.0f;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bLastLine = false;
        /// <summary>
        /// 
        /// </summary>
        private int m_spaces = 0;
        /// <summary>
        /// 
        /// </summary>
        private float m_rightPosition = 0;
        /// <summary>
        /// 
        /// </summary>
        private float m_skipLeftPosition = 0;
        /// <summary>
        /// 
        /// </summary>
        private TabJustification m_prevTabJustification = TabJustification.Left;
        /// <summary>
        /// 
        /// </summary>
        private LayoutedWidget m_owner = null;
        /// <summary>
        /// 
        /// </summary>
        private float m_topMargin = 0;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isLastItemInPage;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isNotFitted;
        /// <summary>
        /// Specifies the footnote textbody height of the current line
        /// </summary>
        internal float m_footnoteHeight;
        /// <summary>
        /// Specifies the endnote textbody height of the current line
        /// </summary>
        internal float m_endnoteHeight;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the right position.
        /// </summary>
        /// <value>The right position.</value>
        internal float RightPosition
        {
            get
            {
                return m_rightPosition;
            }
            set
            {
                m_rightPosition = value;
            }
        }
        /// <summary>
        /// Gets or sets the left position to skip.
        /// </summary>
        /// <value>The left position.</value>
        internal float SkipLeftPosition
        {
            get
            {
                return m_skipLeftPosition;
            }
            set
            {
                m_skipLeftPosition = value;
            }
        }
        /// <summary>
        /// Gets or sets the prev tab justification.
        /// </summary>
        /// <value>The prev tab justification.</value>
        internal TabJustification PrevTabJustification
        {
            get
            {
                return m_prevTabJustification;
            }
            set
            {
                m_prevTabJustification = value;
            }
        }
        /// <summary>
        /// Gets or sets the no. of spaces
        /// </summary>
        public int Spaces
        {
            get
            {
                return m_spaces;
            }
            set
            {
                m_spaces = value;
            }
        }
        /// <summary>
        /// True, if it is last line of the paragraph
        /// </summary>
        public bool IsLastLine
        {
            get
            {
                return m_bLastLine;
            }
            set
            {
                m_bLastLine = value;
            }
        }
        /// <summary>
        /// Gets or Sets the SubWidth
        /// </summary>
        public float SubWidth
        {
            get
            {
                return m_subWidth;
            }
            set
            {
                m_subWidth = value;
            }
        }
        /// <summary>
        /// Gets or sets the HorizontalAlignment
        /// </summary>
        public HorizontalAlignment HorizontalAlign
        {
            get
            {
                return m_horizontalAlign;
            }
            set
            {
                m_horizontalAlign = value;
            }
        }
        /// <summary>
        /// Gets or sets the Extra Wordspace, needs to be added while justifying
        /// </summary>
        public float WordSpace
        {
            get
            {
                return m_wordSpace;
            }
            set
            {
                m_wordSpace = Math.Abs(Convert.ToSingle(value));
            }
        }
        /// <summary>
        /// Gets or sets the text tag.
        /// </summary>
        /// <value>The text tag.</value>
        public string TextTag
        {
            get
            {
                return m_textTag;
            }
            set
            {
                m_textTag = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }
            set
            {
                m_bounds = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal IWidget Widget
        {
            get
            {
                return m_widget;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LayoutedWidgetList ChildWidgets
        {
            get
            {
                return m_ltWidgets;
            }
        }
        /// <summary>
        /// Get/set Owner Layouted Widget
        /// </summary>
        public LayoutedWidget Owner
        {
            get
            {
                return m_owner;
            }
            set
            {
                m_owner = value;
            }
        }
        /// <summary>
        /// Get/set Top MArgin of Cell Widget
        /// </summary>
        public float TopMargin
        {
            get
            {
                return m_topMargin;
            }
            set
            {
                m_topMargin = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is last Text Body Item of current page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is  last Text Body Item of current page; otherwise, <c>false</c>.
        internal bool IsLastItemInPage
        {
            get
            {
                return m_isLastItemInPage;
            }
            set
            {
                m_isLastItemInPage = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is need to be split into next page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is split into next page; otherwise, <c>false</c>.
        internal bool IsNotFitted
        {
            get
            {
                return m_isNotFitted;
            }
            set
            {
                m_isNotFitted = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutedWidget"/> class.
        /// </summary>
        /// <param name="widget">The widget.</param>
        public LayoutedWidget(IWidget widget)
        {
            m_widget = widget;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutedWidget"/> class.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="location">The location.</param>
        public LayoutedWidget(IWidget widget, PointF location)
        {
            m_widget = widget;
            m_bounds = new RectangleF(location, new SizeF());
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Get the footnote textbody height
        /// </summary>
        /// <param name="ltWidget"></param>
        /// <returns></returns>
        internal void GetFootnoteHeight(ref float height)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (ChildWidgets[i].Widget is WFootnote)
                    height += (ChildWidgets[i].Widget.LayoutInfo as FootnoteLayoutInfo).FootnoteHeight;
                else if (ChildWidgets[i].Widget is IWidgetContainer)
                    ChildWidgets[i].GetFootnoteHeight(ref height);
            }
        }
        /// <summary>
        /// Draws the specified dc.
        /// </summary>
        /// <param name="dc">The dc.</param>
        public void Draw(DrawingContext dc)
        {
            m_widget.Draw(dc, this);

            for (int i = 0, len = m_ltWidgets.Count; i < len; i++)
            {
                LayoutedWidget ltWidget = m_ltWidgets[i];
                if (IsOverLappedShapeWidget(ltWidget))
                {
                    //Currently handled only for Docx format documents.
                    int orderIndex = 0;
                    if (ltWidget.Widget is WPicture)
                        orderIndex = (ltWidget.Widget as WPicture).OrderIndex;
                    else if (ltWidget.Widget is Shape)
                        orderIndex = (ltWidget.Widget as Shape).ZOrderPosition;
                    else if (ltWidget.Widget is WTable)
                        orderIndex = (ltWidget.Widget as WTable).m_textBoxFormat.OrderIndex;
                    if (orderIndex == int.MaxValue || orderIndex == 0)
                    {
                        orderIndex = dc.m_orderIndex;
                        dc.m_orderIndex++;
                    }
                    dc.OverLappedShapeWidgets.Add(orderIndex,ltWidget);
                    m_ltWidgets.RemoveAt(i);
                    i--;
                    len--;
                    continue;
                }
                if (ltWidget != null)
                {
                    ltWidget.Draw(dc);
                }
                else
                {
                    Trace.WriteLine("object is null", "LayoutedWidget.Draw()");
                }
            }
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        public void InitLayoutInfo()
        {
            m_widget.InitLayoutInfo();

            for (int i = 0, len = m_ltWidgets.Count; i < len; i++)
            {
                LayoutedWidget ltWidget = m_ltWidgets[i];
                if (ltWidget != null)
                {
                    ltWidget.InitLayoutInfo();
                }
            }
        }
        /// <summary>
        /// Determines whether the layouted widget is Overlapping Widget
        /// </summary>
        /// <param name="ltWidget">The lt widget.</param>
        /// <returns>
        /// 	<c>true</c> if the layouted widget is Overlapping shape widget; otherwise, <c>false</c>.
        /// </returns>
        private bool IsOverLappedShapeWidget(LayoutedWidget ltWidget)
        {
            return ((ltWidget.Widget is WPicture
                    && (ltWidget.Widget as WPicture).TextWrappingStyle != TextWrappingStyle.Inline
                    && (ltWidget.Widget as WPicture).TextWrappingStyle != TextWrappingStyle.Behind)
                    || (ltWidget.Widget is Shape
                    && (ltWidget.Widget as Shape).WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline
                    && (ltWidget.Widget as Shape).WrapFormat.TextWrappingStyle != TextWrappingStyle.Behind)
                    || (ltWidget.Widget is WTable
                    && (ltWidget.Widget as WTable).m_isTextBox
                    && (ltWidget.Widget as WTable).m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.Inline
                    && (ltWidget.Widget as WTable).m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.Behind));
        }
        /// <summary>
        /// Shifts the location.
        /// </summary>
        /// <param name="xOffset">The x offset.</param>
        /// <param name="yOffset">The y offset.</param>
        public void ShiftLocation(double xOffset, double yOffset,bool isPictureNeedToBeShifted)
        {
            m_bounds = new RectangleF(
              new PointF((float)(m_bounds.X + xOffset), (float)(m_bounds.Y + yOffset)),
              m_bounds.Size
              );

            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (m_widget is WTableCell && !(m_widget as WTableCell).OwnerRow.OwnerTable.m_isTextBox)
                {
                    (m_widget.LayoutInfo as TableLayoutInfo).TableCellTopMargin = m_bounds.Y;
                    TopMargin = m_bounds.Y;
                }
                LayoutedWidget ltWidget = ChildWidgets[i];
                if (ltWidget != null)
                {
                    if (ltWidget.Widget is WTable)
                    {
                        WTable table = ltWidget.Widget as WTable;
                        if (table.TableFormat.WrapTextAround)
                        {
                            if (!IsShiftAbsTableBasedOnPageBottom(ltWidget, xOffset) && table.TableFormat.Positioning.VertRelationTo == Syncfusion.DocIO.VerticalRelation.Paragraph
                                && isPictureNeedToBeShifted)
                                ltWidget.ShiftLocation(0, yOffset,isPictureNeedToBeShifted);
                            continue;
                        }
                    }
                    if (ltWidget.Widget is WPicture || ltWidget.Widget is Shape)
                    {
                        TextWrappingStyle textWrapStyle = (ltWidget.Widget is Shape) ? (ltWidget.Widget as Shape).WrapFormat.TextWrappingStyle : (ltWidget.Widget as WPicture).TextWrappingStyle;
                        VerticalOrigin vertOrigin = (ltWidget.Widget is Shape) ? (ltWidget.Widget as Shape).VerticalOrigin : (ltWidget.Widget as WPicture).VerticalOrigin;
                        if (textWrapStyle != TextWrappingStyle.Inline)
                        {
                            if ((vertOrigin == VerticalOrigin.Paragraph
                                || vertOrigin == VerticalOrigin.Line)
                                && isPictureNeedToBeShifted)
                                ltWidget.ShiftLocation(0, yOffset, isPictureNeedToBeShifted);
                            continue;
                        }
                    }
                    ltWidget.ShiftLocation(xOffset, yOffset,isPictureNeedToBeShifted);
                }
            }
        }
        /// <summary>
        /// The absolute table shifted based on bottom if absolute table bottom exceeded the page bottom. 
        /// </summary>
        /// <param name="ltWidget">The widget</param>
        /// <param name="xOffset">The x offset.</param>
        private bool IsShiftAbsTableBasedOnPageBottom(LayoutedWidget ltWidget, double xOffset)
        {
            bool IsShifted = false;
            Entity ent = GetBaseEntity(ltWidget.Widget as Entity);
            if (ent.Owner != null
            && ((ent is HeaderFooter)//Check whether the current table is in footer or not 
            && ((ent as HeaderFooter).Type == HeaderFooterType.OddFooter
            || (ent as HeaderFooter).Type == HeaderFooterType.FirstPageFooter
            || (ent as HeaderFooter).Type == HeaderFooterType.EvenFooter)))
            {
                float totalHeight = (ent.Owner as WSection).PageSetup.PageSize.Height;
                WTable table = ltWidget.Widget as WTable;
                //check the absolute table bottom position greater than equal to page bottom position
                if (totalHeight <= ltWidget.Bounds.Bottom
                    && (ltWidget.Widget as WTable).TableFormat.Positioning.VertRelationTo == VerticalRelation.Paragraph)
                {
                    IsShifted = true;
                    float yPosition = (ltWidget.Bounds.Y) - totalHeight;
                    yPosition = yPosition + ltWidget.Bounds.Height;
                    ltWidget.ShiftLocation(xOffset, -yPosition, true);//Shift the current table position based on page bottom margin
                }
                if (table.Document.Settings.CompatibilityMode != CompatibilityMode.Word2003 && IsShifted)
                    return true;
            }
            return false;
        }
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
        /// Update Layouted Widget Bounds for the Nested Table
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="totalWidth">Owner cell Width</param>
        /// <param name="totalHeight">Owner cell Height</param>
        public void UpdateLtWidgetBounds(float width, float height, float totalWidth, float totalHeight)
        {
            if (m_bounds.Width > width && totalHeight == 0)
                m_bounds = new RectangleF(m_bounds.X, m_bounds.Y, width, m_bounds.Height);
            else if (m_bounds.Height > totalHeight && totalWidth == 0)
                m_bounds = new RectangleF(m_bounds.X, m_bounds.Y, m_bounds.Width, height);

            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                LayoutedWidget ltWidget = ChildWidgets[i];
                float prevChildWdith = 0;
                //Get the previous child item width
                for (int j = i - 1; j >= 0 && i != 0; j--)
                {
                    if (ChildWidgets[j] != null && !IsLtWidgetBoundsNeedToUpdate(ChildWidgets[j]))
                        continue;
                    if (ChildWidgets[j].Widget is ParagraphItem)
                        prevChildWdith += ChildWidgets[j].Bounds.Width;
                }
                if (ltWidget != null)
                {
                    //Check whether the Layouted Widget Bounds is need to be update or not.
                    if (!IsLtWidgetBoundsNeedToUpdate(ltWidget))
                        continue;
                    Entity ent = ltWidget.Widget as Entity;
                    while (!(ent is WTableCell))
                    {
                        if (ent == null || ent is WTable)
                            break;
                        else
                            ent = ent.Owner;
                    }
                    //Get the width and height to update
                    if (ent is WTableCell && !(ltWidget.Widget is WTableCell))
                    {
                        if (totalHeight == 0)
                            width = totalWidth - (float)(((ent as WTableCell) as IWidget).LayoutInfo.Margins.Left + ((ent as WTableCell) as IWidget).LayoutInfo.Paddings.Left) - prevChildWdith;
                        else
                            height = totalHeight - (float)(((ent as WTableCell) as IWidget).LayoutInfo.Margins.Top + ((ent as WTableCell) as IWidget).LayoutInfo.Paddings.Top);
                    }
                    if (width < 0)
                        width = 0;
                    //Update Size of the Layouted Widget
                    ltWidget.UpdateLtWidgetBounds(width, height, totalWidth, totalHeight);
                }
            }
        }
        /// <summary>
        /// Determine Whether the layouted Widget Bounds is need to be update or not.
        /// </summary>
        /// <param name="ltWidget"></param>
        /// <returns></returns>
        private bool IsLtWidgetBoundsNeedToUpdate(LayoutedWidget ltWidget)
        {
            if (((ltWidget.Widget is WPicture)
                && ((ltWidget.Widget as WPicture).TextWrappingStyle == TextWrappingStyle.InFrontOfText
                || (ltWidget.Widget as WPicture).TextWrappingStyle == TextWrappingStyle.Behind))
                || ((ltWidget.Widget is Shape)
                && ((ltWidget.Widget as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.InFrontOfText
                || (ltWidget.Widget as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.Behind)))
                return false;
            else if ((ltWidget.Widget is WTable)
                && (ltWidget.Widget as WTable).m_isTextBox
                && ((ltWidget.Widget as WTable).m_textBoxFormat.TextWrappingStyle == TextWrappingStyle.InFrontOfText
                || (ltWidget.Widget as WTable).m_textBoxFormat.TextWrappingStyle == TextWrappingStyle.Behind))
                return false;
            return true;
        }
        /// <summary>
        /// Aligns the bottom.
        /// </summary>
        /// <param name="g">The g.</param>
        public double AlignBottom(DrawingContext dc)
        {
            double maxHeight;
            double maxAscent;
            float exceededLineSpaceFactor = float.MinValue;
            bool isMaxPictureHeight = false;//which is indicate to whether picture have been highest height in corresponding paragraph collection or not.
            WParagraph paragraph = GetParagraph();
            //To hold the extra line ascent value for the "Arial unicode MS"
            float extraLineAscent= float.MinValue;
            CalculateMaxChildWidget(dc, out maxHeight, out maxAscent);

            if (maxHeight > m_bounds.Height)
            {
                maxHeight = m_bounds.Height;
            }

            if (maxAscent > maxHeight)
            {
                maxAscent = maxHeight;
            }

            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                LayoutedWidget ltWidget = ChildWidgets[i];
                if (ltWidget.Widget is WTextRange)
                {
                    float ascent = dc.GetAscent((ltWidget.Widget as IWTextRange).CharacterFormat.Font);
                    if (ascent > exceededLineSpaceFactor)
                        exceededLineSpaceFactor = ascent;
                }
                if (maxHeight <= ltWidget.Bounds.Height
                    && ltWidget.Widget is WPicture
                    && (ltWidget.Widget as WPicture).TextWrappingStyle == TextWrappingStyle.Inline
                    && paragraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.Multiple
                    && paragraph.ParagraphFormat.LineSpacing >= 12)
                {
                    if (exceededLineSpaceFactor == float.MinValue
                        && IsLastLineOfParagrph(ChildWidgets[ChildWidgets.Count - 1], paragraph))
                    {
                        exceededLineSpaceFactor = dc.GetAscent(paragraph.BreakCharacterFormat.Font);
                    }
                    isMaxPictureHeight = true;
                }
                if (ltWidget != null && !ltWidget.Widget.LayoutInfo.IsSkipBottomAlign)
                {
                    double shiftY = 0;
                    FootnoteLayoutInfo footnoteInfo = (ltWidget.Widget.LayoutInfo as FootnoteLayoutInfo);
                    IStringWidget sWidget = ltWidget.Widget as IStringWidget;

                    if (footnoteInfo != null)
                        sWidget = footnoteInfo.TextRange as IStringWidget;
                    if (sWidget == null)
                    {
                        SplitStringWidget splitWidget = ltWidget.Widget as SplitStringWidget;

                        if (splitWidget != null)
                        {
                            sWidget = splitWidget.RealStringWidget;
                        }
                    }
                    float exceededLineAscent = float.MinValue;
                    shiftY = (sWidget != null)
                        ? ((maxAscent > sWidget.GetTextAscent(dc, ref exceededLineAscent)) ? (maxAscent - sWidget.GetTextAscent(dc, ref exceededLineAscent)) : 0)
                               : (ltWidget.Widget is WSymbol) ? (maxAscent > dc.GetAscent((ltWidget.Widget as WSymbol).GetFont(dc)) ? (maxAscent - dc.GetAscent((ltWidget.Widget as WSymbol).GetFont(dc))) : 0)
                               : ((maxAscent > ltWidget.Bounds.Height) ? maxAscent - ltWidget.Bounds.Height : 0);
                    WCharacterFormat charFormat = (sWidget is WTextRange) ? (sWidget as WTextRange).CharacterFormat : null;
                    if (Math.Round(m_bounds.Bottom, 1) != Math.Round(ltWidget.Bounds.Bottom, 1)
                        || (charFormat != null && charFormat.SubSuperScript != SubSuperScript.None))
                    {
                        shiftY += AlignSubScript(dc, charFormat);
                        ltWidget.ShiftLocation(0, shiftY, true);
                    }
                    //Set the extra line ascent 
                    if (exceededLineAscent != float.MinValue && extraLineAscent < exceededLineAscent)
                        extraLineAscent = exceededLineAscent;
                }
            }
            //Update Bottom Position of the Current Layouted Widget
            float maxBottom = m_bounds.Bottom;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                bool isExactlyLineSpace = paragraph != null && paragraph.ParagraphFormat.LineSpacingRule == DocIO.LineSpacingRule.Exactly ? true : false;
                if (ChildWidgets[i].Widget != null
                    && !ChildWidgets[i].Widget.LayoutInfo.IsSkipBottomAlign
                    && maxBottom < ChildWidgets[i].Bounds.Bottom && !isExactlyLineSpace)
                    maxBottom = ChildWidgets[i].Bounds.Bottom;
            }
            //Align the line y position  based on the exceeded line ascent value for the "Arial Unicode MS"
            if (extraLineAscent != float.MinValue)
                this.ShiftLocation(0, extraLineAscent, true);
            m_bounds = new RectangleF(m_bounds.X, m_bounds.Y, m_bounds.Width, maxBottom - m_bounds.Top);
            double lineSpace = float.MinValue;
            if (isMaxPictureHeight && exceededLineSpaceFactor != float.MinValue)
            {
                float paraLineSpacing = paragraph.ParagraphFormat.LineSpacing;
                float exceededLineSpace = (paraLineSpacing / 12.0f) - 1;
                lineSpace = (float)maxHeight + (exceededLineSpace * exceededLineSpaceFactor);
            }
            return lineSpace;
        }
        /// <summary>
        /// is last line of paragraph
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <returns></returns>
        private bool IsLastLineOfParagrph(LayoutedWidget ltWidget, WParagraph paragraph)
        {
            int paracount = paragraph.ChildEntities.Count-1;
            //if the paragraph last item is field mark we reduce the count because we are not render the field mark
            while (paragraph.ChildEntities[paracount] is WFieldMark)
                paracount--;
            if (paracount < 0)
                return false;
            if (paragraph.ChildEntities[paracount] is StructureDocumentTagInline)
            {
                StructureDocumentTagInline sdtInlineContent = paragraph.ChildEntities[paracount] as StructureDocumentTagInline;
                //iterate the nested SDTInlineContent to find the last item of the paragraph
                while (sdtInlineContent.SDTContent.ParagraphItems[sdtInlineContent.SDTContent.ParagraphItems.Count - 1] is StructureDocumentTagInline)
                    sdtInlineContent = sdtInlineContent.SDTContent.ParagraphItems[sdtInlineContent.SDTContent.ParagraphItems.Count - 1] as StructureDocumentTagInline;
                //check the last of item of this line is last item of the paragraph
                if (sdtInlineContent.SDTContent.ParagraphItems.IndexOf(ltWidget.Widget as Entity) == sdtInlineContent.SDTContent.ParagraphItems.IndexOf(sdtInlineContent.SDTContent.ParagraphItems[sdtInlineContent.SDTContent.ParagraphItems.Count - 1]))
                    return true;
            }
            else
                if (paragraph.ChildEntities.IndexOf(ltWidget.Widget as Entity) == paragraph.ChildEntities.IndexOf(paragraph.ChildEntities[paracount]))
                    return true;
            return false;
        }
        /// <summary>
        /// Gets the paragraph.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <returns></returns>
        private WParagraph GetParagraph()
        {
            WParagraph paragraph = null;
            if (this != null && this.Widget is WParagraph)
                paragraph = this.Widget as WParagraph;
            else if (this != null && this.Widget is SplitWidgetContainer
                && (this.Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph)
                paragraph = (this.Widget as SplitWidgetContainer).RealWidgetContainer as WParagraph;
            return paragraph;
        }
        /// <summary>
        /// Aligns the Subscript
        /// </summary>
        /// <param name="dc">The Custom graphics.</param>
        /// <param name="ltWidget">ltWidget</param>
        private float AlignSubScript(DrawingContext dc, WCharacterFormat charFormat)
        {
            float shiftY = 0.0f;
            if (charFormat != null && charFormat.SubSuperScript == SubSuperScript.SubScript)
            {
                float ascent = dc.GetAscent(charFormat.Font);
                int fontHeight = charFormat.Font.Height;
                shiftY = (float)(shiftY + ascent + ((fontHeight / 1.70f) - fontHeight));
            }
            return shiftY;
        }
       /// <summary>
       /// Aligns the Right
       /// </summary>
       /// <param name="dc">The Custom graphics.</param>
       /// <param name="subWidth">Width of the sub</param>
        public void AlignRight(DrawingContext dc, double subWidth)
        {
            int[] widgetSpaces = new int[ChildWidgets.Count];
            string[] data = new string[ChildWidgets.Count];
            int countAllSpaces = 0;
            int TrimmedSpaces = 0;

            if (ChildWidgets.Count > 0)
            {
                int ChildCount = ChildWidgets.Count;
                LayoutedWidget LastWidget = ChildWidgets[ChildCount - 1];
                SplitStringWidget spW = LastWidget.Widget as SplitStringWidget;
                if (spW != null && spW.SplittedText != null)
                {
                    WTextRange tr = spW.RealStringWidget as WTextRange;
                    int length = spW.SplittedText.Length;
                    spW.SplittedText = spW.SplittedText.TrimEnd();
                    TrimmedSpaces = length - spW.SplittedText.Length;
                    if (TrimmedSpaces > 0)
                    {
                         SizeF TextSize = spW.Measure(dc);
                         float diff = LastWidget.Bounds.Width - TextSize.Width;
                         subWidth += diff;
                         LastWidget.Bounds = new RectangleF(LastWidget.Bounds.X, LastWidget.Bounds.Y, TextSize.Width, LastWidget.Bounds.Height);
                            
                    }
                }
            }
            ShiftLocation(subWidth, 0,true);
        }

        /// <summary>
        /// Aligns the justify.
        /// </summary>
        /// <param name="GraphicsContext">The custom graphics.</param>
        /// <param name="subWidth">Width of the sub.</param>
        public void AlignJustify(DrawingContext dc, double subWidth)
        {
            m_bounds.Width += (float)subWidth;
            int[] widgetSpaces = new int[ChildWidgets.Count];
            int countAllSpaces = 0;
            int TrimmedSpaces = 0;
            string[] data = new string[ChildWidgets.Count];
            int tabIndex = GetTabIndex();
            // Calculate whitespaces for each child l-widget
            for (int i = tabIndex; i < widgetSpaces.Length; i++)
            {
                LayoutedWidget ltW = ChildWidgets[i];
                IStringWidget sW = ltW.Widget as IStringWidget;
                string text = null;

                if (sW == null)
                {
                    SplitStringWidget spW = ltW.Widget as SplitStringWidget;
                    if (spW == null || spW.SplittedText == null)
                    {
                        widgetSpaces[i] = 0;
                    }
                    else
                    {
                        WTextRange tr = spW.RealStringWidget as WTextRange;
                        int length = spW.SplittedText.Length;
                        if (widgetSpaces.Length == 1)
                        {
                            spW.SplittedText = spW.SplittedText.TrimStart();
                            int len = spW.SplittedText.Length;
                            spW.SplittedText = spW.SplittedText.TrimEnd();
                            TrimmedSpaces += len - spW.SplittedText.Length;
                            if (TrimmedSpaces > 0)
                            {
                                SizeF TextSize = spW.Measure(dc);
                                subWidth += ltW.Bounds.Width - TextSize.Width;
                                ltW.Bounds = new RectangleF(ltW.Bounds.X, ltW.Bounds.Y, TextSize.Width, ltW.Bounds.Height);
                            }
                       }
                        else if (i == 0)
                        {
                            spW.SplittedText = spW.SplittedText.TrimStart();
                           
                        }
                        else if (i == widgetSpaces.Length - 1)
                        {
                            int len = spW.SplittedText.Length;
                            spW.SplittedText = spW.SplittedText.TrimEnd();
                            TrimmedSpaces += len - spW.SplittedText.Length;
                            if (TrimmedSpaces > 0)
                            {
                                SizeF TextSize = spW.Measure(dc);
                                subWidth += ltW.Bounds.Width - TextSize.Width;
                                ltW.Bounds = new RectangleF(ltW.Bounds.X, ltW.Bounds.Y, TextSize.Width, ltW.Bounds.Height);
                            }
                        }

                        text = spW.GetText();
                    }
                }
                else
                {
                    text = sW.Text;
                    WTextRange tr = null;
                    if (ltW.Widget is WTextRange)
                    {
                        tr = ltW.Widget as WTextRange;
                    }
                    if (tr != null)
                    {
                        int length = tr.Text.Length;
                        if (i == 0)
                        {
                            text = tr.Text.TrimStart();
                        }
                        if (i == widgetSpaces.Length - 1)
                        {
                            text = tr.Text.TrimEnd();
                        }
                    }
                }

                if (text != null)
                {
                    string[] parts = null;
                    parts = text.Split(new char[] { ' ' });
                    int cnt = parts.Length - 1;
                    widgetSpaces[i] = cnt;
                    this.ChildWidgets[i].Spaces = cnt;
                    countAllSpaces += cnt;
                    data[i] = text;
                }
            }

            // Changes width & position for child l-widgets
            float spaceDelta = GetSpaceDelta(countAllSpaces, subWidth, widgetSpaces, 0);
            double shiftDelta = 0f;
            int remainingSpaces = countAllSpaces;
            for (int i = tabIndex; i < widgetSpaces.Length; i++)
            {
                LayoutedWidget ltW = ChildWidgets[i];
                IStringWidget sW = ltW.Widget as IStringWidget;
                SplitStringWidget spW = null;
                ltW.ShiftLocation(shiftDelta, 0f,true);

                if (sW == null)
                {
                    spW = ltW.Widget as SplitStringWidget;
                }

                if (sW != null || spW != null)
                {
                    RectangleF bounds = ltW.Bounds;
                    double currSpacesDelta = spaceDelta * widgetSpaces[i];
                    if (i != widgetSpaces.Length - 1 && ChildWidgets[i + 1].Widget != null && (ChildWidgets[i + 1].Widget.LayoutInfo as TabsLayoutInfo) != null)
                    {
                        currSpacesDelta = 0;
                        remainingSpaces -= widgetSpaces[i];
                        spaceDelta = GetSpaceDelta(remainingSpaces, subWidth - shiftDelta, widgetSpaces, i + 1);
                    }
                    bounds.Width += (float)currSpacesDelta;
                    ltW.Bounds = bounds;
                    shiftDelta += currSpacesDelta;
                }
            }
        }
        /// <summary>
        /// Get index of the tab from the current line
        /// </summary>
        /// <returns></returns>
        private int GetTabIndex()
        {
            int tabIndex = 0;
            bool isHaveTab = false;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if ((ChildWidgets[i].Widget.LayoutInfo as TabsLayoutInfo) != null)
                {
                    tabIndex = i + 1;
                    isHaveTab = true;
                }
                else if (tabIndex >= i && isHaveTab)
                {
                    WTextRange textRange = GetTextRange(ChildWidgets[i].Widget);
                    if ((textRange != null && textRange.Text.Trim((char)32) == string.Empty) || textRange == null)
                        tabIndex = i + 1;
                    else
                        isHaveTab = false;
                }
            }
            return tabIndex;
        }
        /// <summary>
        /// Get the Text Range
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        private WTextRange GetTextRange(IWidget widget)
        {
            return ((widget is WTextRange) ? (widget as WTextRange)
                     : ((widget is SplitStringWidget) ? (widget as SplitStringWidget).RealStringWidget as WTextRange : null));
        }
        /// <summary>
        /// Get Space Delta
        /// </summary>
        /// <param name="countAllSpaces"></param>
        /// <param name="subWidth"></param>
        /// <param name="widgetSpaces"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private float GetSpaceDelta(int countAllSpaces,double subWidth,int[] widgetSpaces,int index)
        {
            float spaceDelta = (countAllSpaces != 0) ? Convert.ToSingle(subWidth) / (countAllSpaces) : 0.0f;
            this.SubWidth = Convert.ToSingle(subWidth);// Convert.ToSingle(spaceDelta);
            for (int i = index; i < this.ChildWidgets.Count; i++)
            {
                if (spaceDelta < 1)
                    this.ChildWidgets[i].WordSpace = Convert.ToSingle(spaceDelta) * -1;
                else
                    this.ChildWidgets[i].WordSpace = Convert.ToSingle(spaceDelta);
                this.ChildWidgets[i].SubWidth = widgetSpaces[i] * spaceDelta;
            }
            return spaceDelta;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calculates the max child widget.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="maxHeight">Height of the max.</param>
        /// <param name="maxAscent">The max ascent.</param>
        private void CalculateMaxChildWidget(DrawingContext dc, out double maxHeight, out double maxAscent)
        {
            maxHeight = 0;
            maxAscent = 0;
            //Update List character height
            if (Widget is WParagraph && (Widget.LayoutInfo as ParagraphLayoutInfo).ListValue != string.Empty)
            {
                //Update List character ascent as a maxAscent
                maxAscent = dc.GetAscent((Widget.LayoutInfo as ParagraphLayoutInfo).CharacterFormat.Font);
                //Update List character Height as maxHeight
                maxHeight = dc.MeasureString(" ", (Widget.LayoutInfo as ParagraphLayoutInfo).CharacterFormat.Font, null,null,true).Height;
            }
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                LayoutedWidget ltWidget = ChildWidgets[i];

                if (ltWidget != null && !ltWidget.Widget.LayoutInfo.IsSkipBottomAlign)
                {
                    if (m_ltWidgets.Count == 1 || maxHeight < ltWidget.Bounds.Height)
                    {
                        maxHeight = ltWidget.Bounds.Height;
                    }

                    FootnoteLayoutInfo footnoteInfo = (ltWidget.Widget.LayoutInfo as FootnoteLayoutInfo);
                    IStringWidget sWidget = ltWidget.Widget as IStringWidget;

                    if (footnoteInfo != null)
                        sWidget = footnoteInfo.TextRange as IStringWidget;

                    if (sWidget == null)
                    {
                        SplitStringWidget splitWidget = ltWidget.Widget as SplitStringWidget;

                        if (splitWidget != null)
                        {
                            sWidget = splitWidget.RealStringWidget;
                        }
                    }

                    if (sWidget != null)
                    {
                        float exceededLineAscent = float.MinValue;
                        double textAscent = sWidget.GetTextAscent(dc, ref exceededLineAscent);
                        if (m_ltWidgets.Count == 1 || maxAscent < textAscent)
                        {
                            maxAscent = textAscent;
                        }
                    }
                    else if (ltWidget.Widget is WSymbol)
                    {
                        double textAscent = dc.GetAscent((ltWidget.Widget as WSymbol).GetFont(dc));
                        if (m_ltWidgets.Count == 1 || maxAscent < textAscent)
                        {
                            maxAscent = textAscent;
                        }
                    }
                    else if (!(ltWidget.Widget is BookmarkEnd || ltWidget.Widget is BookmarkStart || ltWidget.Widget is WFieldMark))
                    {
                        maxAscent = maxHeight;
                    }
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents the collection of layouted widget.
    /// </summary>
    internal class LayoutedWidgetList : List<LayoutedWidget>
    {
    }
}

#endif