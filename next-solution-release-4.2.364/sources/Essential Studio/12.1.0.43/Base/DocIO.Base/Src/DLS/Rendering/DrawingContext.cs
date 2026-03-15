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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.Convertors;
using Syncfusion.Layouting;
using Font = System.Drawing.Font;
using Syncfusion.DocIO.DLS.Rendering;
using System.Xml;



namespace Syncfusion.DocIO.Rendering
{

    /// <summary>
    /// Represents the class which acts as an drawing context.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DrawingContext : DocumentLayouter
    {
        #region Constants
        /// <summary>
        /// The default script factor for sub/super script.
        /// </summary>
        private const float DEF_SCRIPT_FACTOR = 1.50f;
        private const float DEF_EMBOSS_ENGRAVE_FACTOR = .2f;
        //Special Characters
        private const char NONBREAK_HYPHEN = (char)0x1E;
        private const char SOFT_HYPHEN = (char)0xAD;
        private const char SPACE = (char)0x20;
        private float[] ThinThickThinArray = new float[] { -0.75f, -0.75f, 1f, -0.75f, -0.75f };
        #endregion

        #region Fields
        /// <summary>
        /// Drawing Graphics.
        /// </summary>
        private System.Drawing.Graphics m_graphics;
        internal int m_footnoteId = 1;
        internal int m_endnoteId = 1;
        /// <summary>
        /// Holds the list of hyperlinks and its corrsponding bounds.
        /// </summary>
        List<Dictionary<string, RectangleF>> m_hyperLinks = new List<Dictionary<string, RectangleF>>();
        internal WParagraph currParagraph = null;
        internal WTextRange currTextRange = null;
        internal Hyperlink currHyperlink = null;
        internal RectangleF CurrParagraphBounds = new RectangleF();
        private bool IsListCharacter = false;
        internal int m_orderIndex = 1;
        internal float m_pageMarginLeft;
        /// <summary>
        /// Holds the list of Background colors for which the auto fore color is need to be set as white color. 
        /// </summary>
        List<Color> m_backgroundColorCheckList = new List<Color>(new Color[] {
            //Theme colors
            //Black, Text 1, Lighter 15%
            Color.FromArgb(255,38,38,38),
            //Black, Text 1, Lighter 5%
            Color.FromArgb(255,13,13,13), 
            //Tan, Background 2, Darker 90%
            Color.FromArgb(255,29,27,17), 
            //Dark Blue, Text 2, Darker 25%
            Color.FromArgb(255,23,54,93),
            //Dark Blue, Text 2, Darker 50%
            Color.FromArgb(255,15,36,62), 
            //Blue, Accent 1, Darker 50%
            Color.FromArgb(255,36,64,97),
            //Red, Accent 2, Darker 50%
            Color.FromArgb(255,99,36,35),
            //Purple, Accent 4, Darker 50%
            Color.FromArgb(255,64,49,82),
            //Standard colors
            //Dark Red
            Color.FromArgb(255,192,0,0), 
            //Dark Blue
            Color.FromArgb(255,0,32,96),
            //Custom colors
            Color.FromArgb(255,0,0,255), 
            Color.FromArgb(255,0,0,0) });
        private Dictionary<int, LayoutedWidget> m_overLappedShapeWidgets;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        internal System.Drawing.Graphics Graphics
        {
            get
            {
                return m_graphics;
            }
            set
            {
                if (m_graphics != value)
                {
                    m_graphics = value;
                }
            }
        }

        /// <summary>
        /// Gets the hyperlinks.
        /// </summary>
        /// <value>The hyperlinks.</value>
        public List<Dictionary<string, RectangleF>> Hyperlinks
        {
            get
            {
                return m_hyperLinks;
            }
        }
        /// <summary>
        /// Gets the bookmark hyperlinks list.
        /// </summary>
        public List<Dictionary<string, BookmarkHyperlink>> BookmarkHyperlinksList
        {
            get
            {
                return BookmarkHyperlinks;
            }
        }
        /// <summary>
        /// Gets the overlapping shape widgets.
        /// </summary>
        /// <value>The overlapping shape widgets.</value>
        internal Dictionary<int, LayoutedWidget> OverLappedShapeWidgets
        {
            get
            {
                if (m_overLappedShapeWidgets == null)
                    m_overLappedShapeWidgets = new Dictionary<int, LayoutedWidget>();
                return m_overLappedShapeWidgets;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRenderer"/> class.
        /// </summary>
        internal DrawingContext()
        {
            Bitmap bmp = new Bitmap(1, 1);
            Graphics = Graphics.FromImage(bmp);
            bmp.SetResolution(120, 120);
            Graphics.PageUnit = GraphicsUnit.Point;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRenderer"/> class.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pageUnit">The page unit.</param>
        internal DrawingContext(Graphics graphics, GraphicsUnit pageUnit)
        {
            if (graphics == null)
                throw new ArgumentException("Graphics");

            m_graphics = graphics;
            m_graphics.PageUnit = pageUnit;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws the Overlapping shape widgets.
        /// Currently handled only for Docx format documents
        /// </summary>
        internal void DrawOverLappedShapeWidgets()
        {
            List<int> sortedOverLappedShapeWidgets = new List<int>(OverLappedShapeWidgets.Keys);
            sortedOverLappedShapeWidgets.Sort();
            foreach (int key in sortedOverLappedShapeWidgets)
            {
                OverLappedShapeWidgets[key].Draw(this);
            }
            OverLappedShapeWidgets.Clear();
            m_orderIndex = 1;
        }
        /// <summary>
        /// Draws the paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="ltWidget">The lt widget.</param>
        internal void DrawParagraph(WParagraph paragraph, LayoutedWidget ltWidget)
        {
            WParagraphStyle pStyle = paragraph.GetStyle() as WParagraphStyle;
            WListFormat listFormat = null;
            currParagraph = paragraph;
            currTextRange = null;
            CurrParagraphBounds = ltWidget.Bounds;
            if (pStyle != null && pStyle.ListFormat.CurrentListLevel != null && pStyle.ListFormat.HasValue(0))
                listFormat = pStyle.ListFormat;
            else
                listFormat = paragraph.ListFormat;
            LayoutedWidget LastWidget = null;
            // Skip drawing paragraph, if paragraph is section end mark.
            if (paragraph.SectionEndMark)
                return;
            RectangleF bounds= GetBoundsToDrawParagraphBackGroundColor(paragraph,ltWidget);
            if (paragraph.ParagraphFormat.TextureStyle != TextureStyle.TextureNone)
                DrawTextureStyle(paragraph.ParagraphFormat.TextureStyle, paragraph.ParagraphFormat.ForeColor, paragraph.ParagraphFormat.BackColor, bounds);
            else if (!paragraph.ParagraphFormat.BackColor.IsEmpty)
                Graphics.FillRectangle(new SolidBrush(paragraph.ParagraphFormat.BackColor), bounds);

            if (ltWidget.TextTag != "Splitted" && ltWidget.ChildWidgets.Count > 0)
            {
                LastWidget = ltWidget.ChildWidgets[ltWidget.ChildWidgets.Count - 1];
                if (LastWidget.ChildWidgets.Count > 0 && LastWidget.ChildWidgets[0].HorizontalAlign == Syncfusion.Layouting.HorizontalAlignment.Justify)
                {
                    for (int i = 0; i < LastWidget.ChildWidgets.Count; i++)
                    {
                        LastWidget.ChildWidgets[i].IsLastLine = true;
                    }

                    LastWidget.ChildWidgets[0].Bounds = new RectangleF(LastWidget.ChildWidgets[0].Bounds.X, LastWidget.ChildWidgets[0].Bounds.Y,
                                                       LastWidget.ChildWidgets[0].Bounds.Width - Convert.ToSingle(/*LastWidget.ChildWidgets[0].m_subWidth +*/ LastWidget.ChildWidgets[0].Spaces * LastWidget.ChildWidgets[0].WordSpace), LastWidget.ChildWidgets[0].Bounds.Height);
                    for (int i = 1; i < LastWidget.ChildWidgets.Count; i++)
                    {
                        LayoutedWidget prev = LastWidget.ChildWidgets[i - 1];
                        LayoutedWidget curr = LastWidget.ChildWidgets[i];
                        float width = curr.Bounds.Width - ((curr.SubWidth != 0.0f) ? (Convert.ToSingle(/*curr.m_subWidth +*/ curr.Spaces * curr.WordSpace)) : 0.0f);
                        if (!((curr.Widget is WPicture) || (curr.Widget is Shape)))
                        {
                            width = curr.Bounds.Width - ((curr.SubWidth != 0.0f) ? (Convert.ToSingle(/*curr.m_subWidth +*/ curr.Spaces * curr.WordSpace)) : 0.0f);
                            if (curr.PrevTabJustification != Syncfusion.Layouting.TabJustification.Right && curr.PrevTabJustification != Syncfusion.Layouting.TabJustification.Centered)
                                curr.Bounds = new RectangleF(prev.Bounds.X + prev.Bounds.Width, curr.Bounds.Y, width, curr.Bounds.Height);
                        }
                    }
                }
                DrawBarTabStop(paragraph, ltWidget);

            }
            LayoutedWidget Widget;
            if (!IsEmptyParagraph(paragraph))
            {
                for (int i = 0; i < ltWidget.ChildWidgets.Count; i++)
                {
                    Widget = ltWidget.ChildWidgets[i];
                    UpdateTabPosition(Widget);
                }
            }

            if (!paragraph.ParagraphFormat.Borders.NoBorder && listFormat.ListType == ListType.NoList)
            {
                DrawParagraphBorders(paragraph, paragraph.ParagraphFormat, ltWidget);
            }
            else
            {
                DrawParagraphBorders(paragraph, paragraph.ParagraphFormat, ltWidget);
            }
            if (!(IsParagraphContainingListHasBreak(ltWidget)))
                DrawList(paragraph, ltWidget);
        }
        /// <summary>
        /// Draw Bar Tab Stop of the paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        private void DrawBarTabStop(WParagraph paragraph, LayoutedWidget ltWidget)
        {
            for (int i = 0; i < paragraph.ParagraphFormat.Tabs.Count; i++)
            {
                if (paragraph.ParagraphFormat.Tabs[i].Justification == Syncfusion.DocIO.DLS.TabJustification.Bar)
                {
                    PointF startPoint = new PointF(paragraph.ParagraphFormat.Tabs[i].Position + ltWidget.Bounds.X, ltWidget.Bounds.Y);
                    PointF endPoint = new PointF(paragraph.ParagraphFormat.Tabs[i].Position + ltWidget.Bounds.X, ltWidget.Bounds.Bottom + paragraph.ParagraphFormat.AfterSpacing);
                    Graphics.DrawLine(Pens.Black, startPoint, endPoint);
                }
            }
        }
        /// <summary>
        /// Get bounds to draw a paragraph back ground color and texture styles
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="ltWidget"></param>
        /// <returns></returns>
        private RectangleF GetBoundsToDrawParagraphBackGroundColor(WParagraph paragraph, LayoutedWidget ltWidget)
        {
            //Update Paragraph bounds
            RectangleF bounds = ltWidget.Bounds;
            if (ltWidget.ChildWidgets.Count > 0 && Math.Round(ltWidget.Bounds.Y, 2) != Math.Round(ltWidget.ChildWidgets[0].Bounds.Y, 2))
            {
                bounds.Y = ltWidget.ChildWidgets[0].Bounds.Y;
                bounds.Height = ltWidget.Bounds.Bottom - ltWidget.ChildWidgets[0].Bounds.Y;
            }
            Entity ent = paragraph.Owner as Entity;
            WSection sec = null;
            if (!(ent is WSection))
            {
                while (ent != null)
                {
                    if (ent is WSection)
                        break;
                    if (ent is WTableCell)
                        break;
                    ent = ent.Owner;
                }
            }
            ParagraphLayoutInfo paragraphInfo = (paragraph as IWidget).LayoutInfo as ParagraphLayoutInfo;
            float firstLineIndent = paragraphInfo.FirstLineIndent;//Get the first line indent value.
            //Check whether the indent's special is hanging or firstlineindent.if firstlineindent means clear the values in firstlineindent.
            if (firstLineIndent > 0)
                firstLineIndent = 0;
            //Update Top Padding based on the previous paragraph border
            if (paragraph.PreviousSibling != null && (paragraph.PreviousSibling is WParagraph)
                && !(paragraph.PreviousSibling as WParagraph).ParagraphFormat.Borders.NoBorder
                && !(paragraph.PreviousSibling as WParagraph).SectionEndMark)
            {
                float topPad = 0;
                if (paragraph.ParagraphFormat.Borders.Top.BorderType == BorderStyle.None
                    && (paragraph.PreviousSibling as WParagraph).ParagraphFormat.Borders.Bottom.BorderType != BorderStyle.None)
                    topPad += (paragraph.PreviousSibling as WParagraph).ParagraphFormat.Borders.Bottom.LineWidth / 2;
                bounds.Y += topPad;
                bounds.Height -= topPad;
            }
            float leftPad = (float)(ltWidget.Widget.LayoutInfo.Margins.Left + ltWidget.Widget.LayoutInfo.Paddings.Left) + firstLineIndent;
            float rightPad = (float)(ltWidget.Widget.LayoutInfo.Margins.Right + ltWidget.Widget.LayoutInfo.Paddings.Right);
            float height = bounds.Height;
            if (paragraph.NextSibling != null
                && paragraph.NextSibling is WParagraph
                && !(paragraph.NextSibling as WParagraph).ParagraphFormat.BackColor.IsEmpty
                && !ltWidget.IsLastItemInPage)
                height += (float)ltWidget.Widget.LayoutInfo.Margins.Bottom;
            if (ent is WSection)
            {
                sec = ent as WSection;
                return new RectangleF(sec.PageSetup.Margins.Left + leftPad, bounds.Y, sec.PageSetup.ClientWidth - leftPad - rightPad, height);
            }
            else if (ent is WTableCell)
            {
                WTableCell tableCell = ent as WTableCell;
                TableLayoutInfo tableInfo = (tableCell.m_layoutInfo as TableLayoutInfo);
                float left = tableInfo.TableCellLeftMargin + leftPad;
                float top = bounds.Y;
                float width = (float)(tableInfo.CellWidth - tableCell.m_layoutInfo.Paddings.Left - tableCell.m_layoutInfo.Paddings.Right - tableCell.m_layoutInfo.Margins.Left - tableCell.m_layoutInfo.Margins.Right);
                if (top == GetCellTopMargin(ltWidget))
                {
                    top += (float)tableCell.m_layoutInfo.Margins.Top;
                    height -= (float)tableCell.m_layoutInfo.Margins.Top;
                }
                return new RectangleF(left, top, width - leftPad - rightPad, height);
            }
            else
                return bounds;
        }
        /// <summary>
        /// Check whether Paragraph Containing list has break.
        /// </summary>
        /// <param name="ltWidget">ltWidget</param>
        /// <returns></returns>
        internal bool IsParagraphContainingListHasBreak(LayoutedWidget ltWidget)
        {
            if (ltWidget != null && ltWidget.ChildWidgets.Count > 0)
            {
                for (int i = 0; i < ltWidget.ChildWidgets[0].ChildWidgets.Count; i++)
                {
                    if (ltWidget.ChildWidgets[0].ChildWidgets[i].Widget is BookmarkStart || ltWidget.ChildWidgets[0].ChildWidgets[i].Widget is BookmarkEnd)
                        continue;
                    if (ltWidget.ChildWidgets[0].ChildWidgets[i].Widget is Break && (ltWidget.ChildWidgets[0].ChildWidgets[i].Widget as Break).BreakType != BreakType.LineBreak)
                    {
                        return true;
                    }
                    else
                        break;
                }
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
        /// Get Left margin of the page
        /// </summary>
        /// <param name="pageMarginLeft"></param>
        /// <returns></returns>
        private float GetPageMarginLeft(LayoutedWidget ltWidget, float pageMarginLeft)
        {
            WParagraph para = GetOwnerParagraph(ltWidget);
            if (para != null && !para.IsInCell)
            {
                Entity ent = GetBaseEntity(para);
                if ((ent is HeaderFooter))
                    return m_pageMarginLeft;
            }
            return pageMarginLeft;
        }
        /// <summary>
        /// Updates Tab Position.
        /// </summary>
        /// <param name="Widget"></param>
        internal void UpdateTabPosition(LayoutedWidget Widget)
        {
            if (Widget != null)
            {
                float WidthToShift = 0f;
                int rightTabStart = 0;
                bool isRightTabStart = true;
                for (int i = 0; i < Widget.ChildWidgets.Count; i++)
                {
                    TabsLayoutInfo tabsInfo = (Widget.ChildWidgets[i]).Widget.LayoutInfo as TabsLayoutInfo;
                    if (Widget.ChildWidgets[i].PrevTabJustification == Syncfusion.Layouting.TabJustification.Right && tabsInfo == null)
                    {
                        if (isRightTabStart)
                        {
                            rightTabStart = i;
                            isRightTabStart = false;
                            TabsLayoutInfo tabsLayoutInfo = (Widget.ChildWidgets[i - 1]).Widget.LayoutInfo as TabsLayoutInfo;
                            int rightTabEnd = GetTabEndIndex(Widget, rightTabStart);
                            float pageMarginLeft = GetPageMarginLeft(Widget, (float)tabsLayoutInfo.PageMarginLeft);
                            float width = Widget.ChildWidgets[rightTabEnd].Bounds.Right - Widget.ChildWidgets[rightTabStart].Bounds.X;
                            float tabWidth = (tabsLayoutInfo.m_currTab.Position + pageMarginLeft) - Widget.ChildWidgets[rightTabStart].Bounds.X;
                            if (width < tabWidth)
                            {
                                WidthToShift = tabWidth - width;
                                Widget.ChildWidgets[rightTabStart - 1].Bounds = new RectangleF(Widget.ChildWidgets[rightTabStart - 1].Bounds.X, Widget.ChildWidgets[rightTabStart - 1].Bounds.Y, WidthToShift, Widget.ChildWidgets[rightTabStart - 1].Bounds.Height);
                            }
                            else
                                WidthToShift = 0;
                        }
                    }
                    else
                    {
                        for (; rightTabStart <= i; rightTabStart++)
                        {
                            if (Widget.ChildWidgets[rightTabStart].PrevTabJustification == Syncfusion.Layouting.TabJustification.Right && ((Widget.ChildWidgets[rightTabStart]).Widget.LayoutInfo as TabsLayoutInfo) == null)
                            {
                                Widget.ChildWidgets[rightTabStart].Bounds = new RectangleF(Widget.ChildWidgets[rightTabStart].Bounds.X + WidthToShift, Widget.ChildWidgets[rightTabStart].Bounds.Y, Widget.ChildWidgets[rightTabStart].Bounds.Width, Widget.ChildWidgets[rightTabStart].Bounds.Height);
                            }
                            isRightTabStart = true;
                        }
                    }
                    if (i == Widget.ChildWidgets.Count - 1)
                    {
                        for (; rightTabStart <= i; rightTabStart++)
                        {
                            if (Widget.ChildWidgets[rightTabStart].PrevTabJustification == Syncfusion.Layouting.TabJustification.Right && ((Widget.ChildWidgets[rightTabStart]).Widget.LayoutInfo as TabsLayoutInfo) == null)
                            {
                                Widget.ChildWidgets[rightTabStart].Bounds = new RectangleF(Widget.ChildWidgets[rightTabStart].Bounds.X + WidthToShift, Widget.ChildWidgets[rightTabStart].Bounds.Y, Widget.ChildWidgets[rightTabStart].Bounds.Width, Widget.ChildWidgets[rightTabStart].Bounds.Height);
                            }
                            isRightTabStart = true;
                        }
                        TabsLayoutInfo tabsLayoutInfo = (Widget.ChildWidgets[i]).Widget.LayoutInfo as TabsLayoutInfo;
                        if (tabsLayoutInfo != null
                            && tabsLayoutInfo.m_currTab.Justification == Syncfusion.Layouting.TabJustification.Right)
                            Widget.ChildWidgets[i].Bounds = new RectangleF(Widget.ChildWidgets[i].Bounds.X, Widget.ChildWidgets[i].Bounds.Y, (tabsLayoutInfo.m_currTab.Position + GetPageMarginLeft(Widget, (float)tabsLayoutInfo.PageMarginLeft)) - Widget.ChildWidgets[i].Bounds.X, Widget.ChildWidgets[i].Bounds.Height);
                    }
                }
                bool isCenterTabStart = true;
                int centerTabStart = 0;
                for (int i = 0; i < Widget.ChildWidgets.Count; i++)
                {
                    TabsLayoutInfo tabsInfo = (Widget.ChildWidgets[i]).Widget.LayoutInfo as TabsLayoutInfo;

                    if (Widget.ChildWidgets[i].PrevTabJustification == Syncfusion.Layouting.TabJustification.Centered && tabsInfo == null)
                    {
                        if (isCenterTabStart)
                        {
                            centerTabStart = i;
                            isCenterTabStart = false;
                            TabsLayoutInfo tabsLayoutInfo = (Widget.ChildWidgets[i - 1]).Widget.LayoutInfo as TabsLayoutInfo;
                            int centerTabEnd = GetTabEndIndex(Widget, centerTabStart);
                            float width = (Widget.ChildWidgets[centerTabEnd].Bounds.Right - Widget.ChildWidgets[centerTabStart].Bounds.X) / 2;
                            float tabWidth = (tabsLayoutInfo.m_currTab.Position + GetPageMarginLeft(Widget, (float)tabsLayoutInfo.PageMarginLeft)) - Widget.ChildWidgets[centerTabStart].Bounds.X;
                            if (width < tabWidth)
                            {
                                WidthToShift = tabWidth - width;
                                Widget.ChildWidgets[centerTabStart - 1].Bounds = new RectangleF(Widget.ChildWidgets[centerTabStart - 1].Bounds.X, Widget.ChildWidgets[centerTabStart - 1].Bounds.Y, WidthToShift, Widget.ChildWidgets[centerTabStart - 1].Bounds.Height);
                            }
                        }
                    }
                    else
                    {
                        for (; centerTabStart <= i; centerTabStart++)
                        {
                            if (Widget.ChildWidgets[centerTabStart].PrevTabJustification == Syncfusion.Layouting.TabJustification.Centered && ((Widget.ChildWidgets[centerTabStart]).Widget.LayoutInfo as TabsLayoutInfo) == null)
                            {
                                Widget.ChildWidgets[centerTabStart].Bounds = new RectangleF(Widget.ChildWidgets[centerTabStart].Bounds.X + WidthToShift, Widget.ChildWidgets[centerTabStart].Bounds.Y, Widget.ChildWidgets[centerTabStart].Bounds.Width, Widget.ChildWidgets[centerTabStart].Bounds.Height);
                            }
                        }
                        isCenterTabStart = true;
                    }
                    if (i == Widget.ChildWidgets.Count - 1)
                    {
                        for (; centerTabStart <= i; centerTabStart++)
                        {
                            if (Widget.ChildWidgets[centerTabStart].PrevTabJustification == Syncfusion.Layouting.TabJustification.Centered && ((Widget.ChildWidgets[centerTabStart]).Widget.LayoutInfo as TabsLayoutInfo) == null)
                            {
                                Widget.ChildWidgets[centerTabStart].Bounds = new RectangleF(Widget.ChildWidgets[centerTabStart].Bounds.X + WidthToShift, Widget.ChildWidgets[centerTabStart].Bounds.Y, Widget.ChildWidgets[centerTabStart].Bounds.Width, Widget.ChildWidgets[centerTabStart].Bounds.Height);
                            }
                        }
                        isCenterTabStart = true;
                        TabsLayoutInfo tabsLayoutInfo = (Widget.ChildWidgets[i]).Widget.LayoutInfo as TabsLayoutInfo;
                        if (tabsLayoutInfo != null
                            && tabsLayoutInfo.m_currTab.Justification == Syncfusion.Layouting.TabJustification.Centered)
                            Widget.ChildWidgets[i].Bounds = new RectangleF(Widget.ChildWidgets[i].Bounds.X, Widget.ChildWidgets[i].Bounds.Y, (tabsLayoutInfo.m_currTab.Position + GetPageMarginLeft(Widget, (float)tabsLayoutInfo.PageMarginLeft)) - Widget.ChildWidgets[i].Bounds.X, Widget.ChildWidgets[i].Bounds.Height);
                    }
                }
                UpdateDecimalTabPosition(Widget);
                UpdateDecimalTabPositionInCell(Widget);
            }
        }
        /// <summary>
        /// Update Decimal Tab Position for the paragraph outside the table
        /// </summary>
        /// <param name="ltWidget"></param>
        private void UpdateDecimalTabPosition(LayoutedWidget ltWidget)
        {
            bool isDecimalTabStart = false;
            bool isDecimalTab = false;
            int decimalTabStart = 0;
            float widthToShift = 0f;
            for (int i = 0; i < ltWidget.ChildWidgets.Count; i++)
            {
                TabsLayoutInfo tabsInfo = ltWidget.ChildWidgets[i].Widget.LayoutInfo as TabsLayoutInfo;
                if (ltWidget.ChildWidgets[i].PrevTabJustification == Syncfusion.Layouting.TabJustification.Decimal
                    && tabsInfo == null)
                {
                    if (isDecimalTabStart)
                    {
                        decimalTabStart = i;
                        isDecimalTabStart = false;
                        isDecimalTab = true;
                        widthToShift = GetWidthToShift(ltWidget, decimalTabStart, false);
                        ltWidget.ChildWidgets[decimalTabStart - 1].Bounds = new RectangleF(ltWidget.ChildWidgets[decimalTabStart - 1].Bounds.X, ltWidget.ChildWidgets[decimalTabStart - 1].Bounds.Y, widthToShift, ltWidget.ChildWidgets[decimalTabStart - 1].Bounds.Height);
                    }
                }
                else
                {
                    isDecimalTabStart = IsDecimalTabStart(ltWidget, decimalTabStart, isDecimalTab, i, widthToShift, false);
                    isDecimalTab = false;
                }
                if (i == ltWidget.ChildWidgets.Count - 1)
                {
                    isDecimalTabStart = IsDecimalTabStart(ltWidget, decimalTabStart, isDecimalTab, i, widthToShift, false);
                    TabsLayoutInfo tabsLayoutInfo = (ltWidget.ChildWidgets[i]).Widget.LayoutInfo as TabsLayoutInfo;
                    if (tabsLayoutInfo != null
                        && tabsLayoutInfo.m_currTab.Justification == Syncfusion.Layouting.TabJustification.Decimal)
                        ltWidget.ChildWidgets[i].Bounds = new RectangleF(ltWidget.ChildWidgets[i].Bounds.X, ltWidget.ChildWidgets[i].Bounds.Y, (tabsLayoutInfo.m_currTab.Position + GetPageMarginLeft(ltWidget, (float)tabsLayoutInfo.PageMarginLeft)) - ltWidget.ChildWidgets[i].Bounds.X, ltWidget.ChildWidgets[i].Bounds.Height);
                }
            }
        }
        /// <summary>
        /// Update Decimal Tab Position for the paragraph inside the table cell
        /// </summary>
        /// <param name="ltWidget"></param>
        private void UpdateDecimalTabPositionInCell(LayoutedWidget ltWidget)
        {
            int decimalTabStart = 0;
            float widthToShift = 0f;
            bool isDecimalTabStart = false;
            WParagraph ownerParagraph = GetOwnerParagraph(ltWidget);
            if (ownerParagraph == null)
                return;
            for (int i = 0; i < ltWidget.ChildWidgets.Count; i++)
            {
                if ((ownerParagraph.IsInCell
                    && ownerParagraph.ParagraphFormat.Tabs.Count != 0)
                    ? ownerParagraph.ParagraphFormat.Tabs[0].Justification == Syncfusion.DocIO.DLS.TabJustification.Decimal : false)
                {
                    if (isDecimalTabStart || ltWidget.ChildWidgets.Count == 1)
                    {
                        isDecimalTabStart = false;
                        widthToShift = GetWidthToShift(ltWidget, decimalTabStart, true);
                    }
                    else
                        isDecimalTabStart = IsDecimalTabStart(ltWidget, decimalTabStart, false, i, widthToShift, true);
                    if (i == 0)
                        isDecimalTabStart = IsDecimalTabStart(ltWidget, decimalTabStart, false, i, widthToShift, true);
                }
            }
        }
        /// <summary>
        /// Determine whether is Decimal Tab Start
        /// </summary>
        /// <param name="ltWidget"></param>
        /// <param name="decimalTabStart"></param>
        /// <param name="isDecimalTab"></param>
        /// <param name="i"></param>
        /// <param name="widthToShift"></param>
        /// <param name="isInCell"></param>
        /// <returns></returns>
        private bool IsDecimalTabStart(LayoutedWidget ltWidget, int decimalTabStart, bool isDecimalTab, int i, float widthToShift, bool isInCell)
        {
            while (decimalTabStart <= i)
            {
                if (isInCell)
                    ltWidget.ChildWidgets[decimalTabStart].Bounds = new RectangleF(ltWidget.ChildWidgets[decimalTabStart].Bounds.X + widthToShift, ltWidget.ChildWidgets[decimalTabStart].Bounds.Y, ltWidget.ChildWidgets[decimalTabStart].Bounds.Width, ltWidget.ChildWidgets[decimalTabStart].Bounds.Height);
                else if (ltWidget.ChildWidgets[decimalTabStart].PrevTabJustification == Syncfusion.Layouting.TabJustification.Decimal
                        && ((ltWidget.ChildWidgets[decimalTabStart]).Widget.LayoutInfo as TabsLayoutInfo) == null && isDecimalTab)
                    ltWidget.ChildWidgets[decimalTabStart].Bounds = new RectangleF(ltWidget.ChildWidgets[decimalTabStart].Bounds.X + widthToShift, ltWidget.ChildWidgets[decimalTabStart].Bounds.Y, ltWidget.ChildWidgets[decimalTabStart].Bounds.Width, ltWidget.ChildWidgets[decimalTabStart].Bounds.Height);
                decimalTabStart++;
            }
            return true;
        }
        /// <summary>
        /// Get Width To Shift the xposition of childwidget
        /// </summary>
        /// <param name="ltWidget"></param>
        /// <param name="decimalTabStart"></param>
        /// <param name="isInCell"></param>
        /// <returns></returns>
        private float GetWidthToShift(LayoutedWidget ltWidget, int decimalTabStart, bool isInCell)
        {
            float widthToShift = 0f;
            float totalWidth = 0f;
            float clientWidth = 0f;
            int decimalTabEnd = GetTabEndIndex(ltWidget, decimalTabStart);
            WParagraph ownerParagraph = GetOwnerParagraph(ltWidget);
            float leftWidth = GetLeftWidth(ltWidget, decimalTabStart, decimalTabEnd);
            WParagraphFormat pFormat = GetCurrentTabFormat(ownerParagraph);//Get paragraph format for tab
            float tabWidth = 0;
            if (pFormat != null && pFormat.Tabs.Count>0)
                tabWidth = pFormat.Tabs[0].Position;//get the first tab poistion from the tab collection.
            float pageMarginLeft = 0;
            for (int index = decimalTabStart; index <= decimalTabEnd; index++)
                totalWidth += ltWidget.ChildWidgets[index].Bounds.Width;
            if (isInCell)
            {
                clientWidth = (float)((ownerParagraph.Owner as WTableCell).Width
                      - ((ownerParagraph.Owner as WTableCell).m_layoutInfo.Paddings.Left
                      + (ownerParagraph.Owner as WTableCell).m_layoutInfo.Paddings.Right));
                pageMarginLeft = ((ownerParagraph.Owner as WTableCell).m_layoutInfo as TableLayoutInfo).TableCellLeftMargin;
            }
            else
            {
                clientWidth = GetColumnWidth(ownerParagraph) > tabWidth ? GetColumnWidth(ownerParagraph) : LayoutContext.MAX_WIDTH;
                TabsLayoutInfo tabsLayoutInfo = ltWidget.ChildWidgets[decimalTabStart - 1].Widget.LayoutInfo as TabsLayoutInfo;
                pageMarginLeft = GetPageMarginLeft(ltWidget, (float)tabsLayoutInfo.PageMarginLeft);
                tabWidth = (tabsLayoutInfo.m_currTab.Position + pageMarginLeft) - ltWidget.ChildWidgets[decimalTabStart].Bounds.X;
            }
            if (leftWidth < tabWidth)
            {
                if ((totalWidth - leftWidth) < (clientWidth - tabWidth))
                {
                    widthToShift = tabWidth - leftWidth;
					//Update WidthToShift based on client width
                    float layoutedWidth = (float)(ltWidget.ChildWidgets[decimalTabStart].Bounds.X - pageMarginLeft);
                    if (!isInCell && clientWidth < ((layoutedWidth + tabWidth + totalWidth - leftWidth)))
                    {
                        float diff = layoutedWidth + tabWidth + totalWidth - leftWidth - clientWidth;
                        widthToShift -= diff;
                    }
                }
                else
                    widthToShift = clientWidth - totalWidth;
            }
            return widthToShift;
        }
        /// <summary>
        /// Get the paragraph format for current tab
        /// </summary>
        /// <param name="paragraph">The Paragraph</param>
        /// <returns>Paragrph format</returns>
        private WParagraphFormat GetCurrentTabFormat(WParagraph paragraph)
        {
            WParagraphFormat pFormat = paragraph.ParagraphFormat as WParagraphFormat;
           
                while (pFormat != null)
                {
                    if (pFormat.Tabs.Count > 0)
                       break;
                    else
                        pFormat = pFormat.BaseFormat as WParagraphFormat;
                }
            
            return pFormat;
        }
       /// <summary>
        /// Get Column Width
        /// </summary>
        /// <param name="paragraph">The Paragraph</param>
        /// <returns></returns>
        private float GetColumnWidth(WParagraph paragraph)
        {
            Entity ent = paragraph as Entity;
            float columnWidth = 0f;
            while (!(ent is WSection))
            {
                if (ent == null)
                    break;
                else
                    ent = ent.Owner as Entity;
            }
            if (ent is WSection)
                columnWidth = (ent as WSection).PageSetup.ClientWidth;
            return columnWidth;
        }
        /// <summary>
        /// Get Left width of the Decimal seperator
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="decimalTabStart"></param>
        /// <param name="decimalTabEnd"></param>
        /// <returns></returns>
        internal float GetLeftWidth(WParagraph paragraph, int decimalTabStart, int decimalTabEnd)
        {
            float leftWidth = 0f;
            if (paragraph.ChildEntities.Count != 0)
            {
                int indexOfDecimalSeperator = 0;
                int decimalSeparator = 0;
                bool isDecimalSeparator = false;
                indexOfDecimalSeperator = GetIndexOfDecimalseparator(paragraph, decimalTabStart, decimalTabEnd, ref leftWidth, ref decimalSeparator, ref isDecimalSeparator);
                if (paragraph.ChildEntities[indexOfDecimalSeperator] is WTextRange && isDecimalSeparator)
                {
                    string[] text = (paragraph.ChildEntities[indexOfDecimalSeperator] as WTextRange).Text.Split((char)decimalSeparator);
                    SizeF size = MeasureTextRange(paragraph.ChildEntities[indexOfDecimalSeperator] as WTextRange, text[0]);
                    leftWidth += size.Width;
                }
            }
            return leftWidth;
        }
        /// <summary>
        /// Get Left width of the Decimal seperator
        /// </summary>
        /// <param name="ltWidget">The lt widget.</param>
        /// <param name="decimalTabStart">The decimal tab start.</param>
        /// <param name="decimalTabEnd">The decimal tab end.</param>
        /// <returns></returns>
        internal float GetLeftWidth(LayoutedWidget ltWidget, int decimalTabStart, int decimalTabEnd)
        {
            float leftWidth = 0f;
            if (ltWidget.ChildWidgets.Count != 0)
            {
                int indexOfDecimalSeperator = 0;
                int decimalSeparator = 0;
                bool isDecimalSeparator = false;
                indexOfDecimalSeperator = GetIndexOfDecimalseparator(ltWidget, decimalTabStart, decimalTabEnd, ref leftWidth, ref decimalSeparator, ref isDecimalSeparator);
                if (ltWidget.ChildWidgets[indexOfDecimalSeperator].Widget is WTextRange && isDecimalSeparator)
                {
                    string[] text = (ltWidget.ChildWidgets[indexOfDecimalSeperator].Widget as WTextRange).Text.Split((char)decimalSeparator);
                    SizeF size = MeasureTextRange(ltWidget.ChildWidgets[indexOfDecimalSeperator].Widget as WTextRange, text[0]);
                    leftWidth += size.Width;
                }
                else if (ltWidget.ChildWidgets[indexOfDecimalSeperator].Widget is SplitStringWidget && isDecimalSeparator)
                {
                    string[] text = (ltWidget.ChildWidgets[indexOfDecimalSeperator].Widget as SplitStringWidget).SplittedText.Split((char)decimalSeparator);
                    SizeF size = MeasureTextRange((ltWidget.ChildWidgets[indexOfDecimalSeperator].Widget as SplitStringWidget).RealStringWidget as WTextRange, text[0]);
                    leftWidth += size.Width;
                }
            }
            return leftWidth;
        }
        /// <summary>
        /// Get Index of Decimal Separator
        /// Index denotes the TextRange which have a decimal separator
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="decimalTabStart"></param>
        /// <param name="decimalTabEnd"></param>
        /// <param name="leftWidth"></param>
        /// <param name="decimalSeparator"></param>
        /// <param name="isSeparator"></param>
        /// <returns></returns>
        private int GetIndexOfDecimalseparator(WParagraph paragraph, int decimalTabStart, int decimalTabEnd, ref float leftWidth, ref int decimalSeparator, ref bool isSeparator)
        {
            //Current Culture Decimal Seperator
            Char numberDecimalSeperator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            bool isDecimalSeparator = false;
            bool isNumber = false;
            int indexOfDecimalSeparator = 0;
            for (int index = decimalTabStart; index <= decimalTabEnd; index++)
            {
                if (paragraph.ChildEntities[index] is WTextRange)
                {
                    decimalSeparator = 0;
                    char[] character = (paragraph.ChildEntities[index] as WTextRange).Text.ToCharArray();
                    for (int j = 0; j < character.Length; j++)
                    {
                        if (char.IsNumber(character[j]))
                        {
                            isNumber = true;
                            break;
                        }
                    }
                    if (isNumber)
                        isDecimalSeparator = IsDecimalSeparator(character, ref decimalSeparator);
                    if (!isNumber && !isDecimalSeparator)
                    {
                        if ((paragraph.ChildEntities[index] as WTextRange).Text.Contains(numberDecimalSeperator.ToString()))
                            isDecimalSeparator = true;
                        decimalSeparator = (int)numberDecimalSeperator;
                    }
                    if (!isDecimalSeparator)
                    {
                        SizeF size = ((paragraph.ChildEntities[index] as WTextRange) as IWidget).LayoutInfo.Size;
                        leftWidth += size.Width;
                    }
                    else
                    {
                        indexOfDecimalSeparator = index;
                        isSeparator = true;
                        break;
                    }
                }
            }
            return indexOfDecimalSeparator;
        }
        /// <summary>
        /// Get Index of Decimal Separator
        /// Index denotes the TextRange which have a decimal separator
        /// </summary>
        /// <param name="ltWidget">The lt widget.</param>
        /// <param name="decimalTabStart">The decimal tab start.</param>
        /// <param name="decimalTabEnd">The decimal tab end.</param>
        /// <param name="leftWidth">Width of the left.</param>
        /// <param name="decimalSeparator">The decimal separator.</param>
        /// <param name="isSeparator">if set to <c>true</c> [is separator].</param>
        /// <returns></returns>
        private int GetIndexOfDecimalseparator(LayoutedWidget ltWidget, int decimalTabStart, int decimalTabEnd, ref float leftWidth, ref int decimalSeparator, ref bool isSeparator)
        {
            //Current Culture Decimal Seperator
            Char numberDecimalSeperator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            bool isDecimalSeparator = false;
            bool isNumber = false;
            int indexOfDecimalSeparator = decimalTabStart;
            for (int index = decimalTabStart; index <= decimalTabEnd; index++)
            {
                if (ltWidget.ChildWidgets[index].Widget is WTextRange || ltWidget.ChildWidgets[index].Widget is SplitStringWidget)
                {
                    bool isTextRange = ltWidget.ChildWidgets[index].Widget is WTextRange;
                    decimalSeparator = 0;
                    char[] character = isTextRange ? (ltWidget.ChildWidgets[index].Widget as WTextRange).Text.ToCharArray() : (ltWidget.ChildWidgets[index].Widget as SplitStringWidget).SplittedText.ToCharArray();
                    for (int j = 0; j < character.Length; j++)
                    {
                        if (char.IsNumber(character[j]))
                        {
                            isNumber = true;
                            break;
                        }
                    }
                    if (isNumber)
                        isDecimalSeparator = IsDecimalSeparator(character, ref decimalSeparator);
                    if (!isNumber && !isDecimalSeparator)
                    {
                        if (isTextRange)
                            isDecimalSeparator = (ltWidget.ChildWidgets[index].Widget as WTextRange).Text.Contains(numberDecimalSeperator.ToString());
                        else
                            isDecimalSeparator = (ltWidget.ChildWidgets[index].Widget as SplitStringWidget).SplittedText.Contains(numberDecimalSeperator.ToString());
                        decimalSeparator = (int)numberDecimalSeperator;
                    }
                    if (!isDecimalSeparator)
                    {
                        SizeF size = isTextRange ? ((ltWidget.ChildWidgets[index].Widget as WTextRange) as IWidget).LayoutInfo.Size
                            : MeasureTextRange((ltWidget.ChildWidgets[index].Widget as SplitStringWidget).RealStringWidget as WTextRange, (ltWidget.ChildWidgets[index].Widget as SplitStringWidget).SplittedText);  
                        leftWidth += size.Width;
                    }
                    else
                    {
                        indexOfDecimalSeparator = index;
                        isSeparator = true;
                        break;
                    }
                }
            }
            return indexOfDecimalSeparator;
        }
        /// <summary>
        /// Determine whether is Decimal Separator
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="decimalSeparator"></param>
        /// <returns></returns>
        private bool IsDecimalSeparator(char[] ch, ref int decimalSeparator)
        {
            //Default ASCII value for single quote
            int singleQuote = 8217;
            //Default ASCII value for double quote
            int doubleQuote = 8221;
            for (int j = 0; j < ch.Length; j++)
            {
                int character = (int)ch[j];
                if (char.IsNumber(ch[j]))
                    continue;
                else if (((character > 31 && character < 127)
                    || character == singleQuote || character == doubleQuote)
                    && (ch.Length == 1 || (j > 0 && char.IsNumber(ch[j - 1])))
                    && character != (int)CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator[0])
                {
                    decimalSeparator = character;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Get Owner paragraph of the LayoutedWidget
        /// </summary>
        /// <param name="ltWidget"></param>
        /// <returns></returns>
        private WParagraph GetOwnerParagraph(LayoutedWidget ltWidget)
        {
            WParagraph paragraph = null;
            if (ltWidget.Widget != null)
                if (ltWidget.Widget is WParagraph)
                    paragraph = ltWidget.Widget as WParagraph;
                else if (ltWidget.Widget is SplitWidgetContainer
                    ? (ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph : false)
                    paragraph = (ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer as WParagraph;
            return paragraph;
        }
        /// <summary>
        /// Gets the tab end index.
        /// Index denotes the item, previous of next subsequent tab.
        /// </summary>
        /// <param name="ltWidget">The lt widget.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        private int GetTabEndIndex(LayoutedWidget ltWidget, int startIndex)
        {
            for (int i = startIndex; i < ltWidget.ChildWidgets.Count; i++)
            {
                TabsLayoutInfo tabsInfo = ltWidget.ChildWidgets[i].Widget.LayoutInfo as TabsLayoutInfo;
                // Returns the index of the item, if next item is tab.
                if (tabsInfo != null)
                    return (i - 1);
            }
            return (ltWidget.ChildWidgets.Count - 1);
        }
        /// <summary>
        /// Draws the list
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="ltWidget"></param>
        internal void DrawList(WParagraph paragraph, LayoutedWidget ltWidget)
        {
            ParagraphLayoutInfo paragraphInfo = ltWidget.Widget.LayoutInfo as ParagraphLayoutInfo;
            if (ltWidget.ChildWidgets.Count == 0
                || ltWidget.Bounds.Width == 0
                || paragraphInfo.ListValue == string.Empty)
                return;

            float max = 0.0f;
            max = -Math.Abs(paragraphInfo.ListTab);
            SizeF size = MeasureString(paragraphInfo.ListValue, paragraphInfo.CharacterFormat.Font, null, paragraphInfo.CharacterFormat,true);
            if (paragraphInfo.ListAlignment == ListNumberAlignment.Center)
                max -= size.Width / 2;
            else if (paragraphInfo.ListAlignment == ListNumberAlignment.Right)
                max -= size.Width;
            float x = ltWidget.ChildWidgets[0].Bounds.X + max;
            float y = GetYPosition(ltWidget, size.Height, paragraphInfo.CurrentListType);
            if (size.Height > ltWidget.ChildWidgets[0].Bounds.Height)
                y -= (size.Height - ltWidget.ChildWidgets[0].Bounds.Height) / 2;
            IsListCharacter = true;
            DrawString(paragraphInfo.ListValue, paragraphInfo.CharacterFormat, paragraph.ParagraphFormat, new RectangleF(x, y, Math.Abs(max), size.Height), Math.Abs(max), ltWidget);

            if (paragraphInfo.ListTabStop != null
                && paragraphInfo.ListTabStop.TabLeader != Syncfusion.Layouting.TabLeader.NoLeader)
                DrawListTabLeader(paragraph, paragraphInfo, (paragraphInfo.ListTab + size.Width + max), ltWidget.ChildWidgets[0].Bounds.X, y);
        }
        /// <summary> 
        /// Draws the list tab leader.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="paragraphInfo">The paragraph info.</param>
        /// <param name="listWidth">Width of the list.</param>
        /// <param name="xPosition">The x position.</param>
        /// <param name="yPosition">The y position.</param>
        private void DrawListTabLeader(WParagraph paragraph, ParagraphLayoutInfo paragraphInfo, float listWidth, float xPosition, float yPosition)
        {
            string tabLeader = GetTabLeader(paragraphInfo);
            if (tabLeader != string.Empty)
            {
                float tabLeaderWidth = MeasureString(tabLeader, paragraphInfo.CharacterFormat.Font, null).Width;
                float tabWidth = paragraphInfo.ListTab - (float)Math.Ceiling(listWidth / tabLeaderWidth) * tabLeaderWidth;
                string text = string.Empty;
                int count = (int)Math.Floor(tabWidth / tabLeaderWidth);
                for (int i = 0; i < count; i++)
                {
                    text += tabLeader;
                }
                SizeF size = MeasureString(text, paragraphInfo.CharacterFormat.Font, null);
                DrawString(text, paragraphInfo.CharacterFormat, paragraph.ParagraphFormat, new RectangleF(xPosition - tabWidth, yPosition, size.Width, size.Height), size.Width, null);
            }
        }
        /// <summary>
        /// Gets the tab leader.
        /// </summary>
        /// <param name="paragraphInfo">The paragraph info.</param>
        /// <returns></returns>
        private string GetTabLeader(ParagraphLayoutInfo paragraphInfo)
        {
            string text = string.Empty;
            switch (paragraphInfo.ListTabStop.TabLeader)
            {
                case Layouting.TabLeader.Dotted:
                    text = ".";
                    break;
                case Layouting.TabLeader.Single:
                    text = "_";
                    break;
                case Layouting.TabLeader.Hyphenated:
                    text = "-";
                    break;
            }
            return text;
        }
        /// <summary>
        /// Gets the Y position.
        /// </summary>
        /// <param name="ltWidget">The ltwidget.</param>
        /// <returns></returns>
        private float GetYPosition(LayoutedWidget ltWidget, float height, ListType listType)
        {
            for (int i = 0; i < ltWidget.ChildWidgets.Count; i++)
            {
                LayoutedWidget childWidget = ltWidget.ChildWidgets[i];
                if (((childWidget.Widget is WTextRange) || (childWidget.Widget is SplitStringWidget && i == 0)
                    || ((childWidget.Widget is WPicture) && (childWidget.Widget as WPicture).TextWrappingStyle == TextWrappingStyle.Inline)
                    || ((childWidget.Widget is Shape) && (childWidget.Widget as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.Inline)
                    || ((childWidget.Widget is WTable) && (childWidget.Widget as WTable).m_isTextBox && (childWidget.Widget as WTable).m_textBoxFormat.TextWrappingStyle == TextWrappingStyle.Inline)))
                {
                    if (childWidget.Bounds.Height == 0)
                        return childWidget.Bounds.Y;
                    else
                        return (childWidget.Bounds.Bottom - height);
                }
                if ((childWidget.Widget is WParagraph)
                    || ((childWidget.Widget is SplitWidgetContainer)
                    && (childWidget.Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph))
                    return GetYPosition(childWidget, height, listType);
            }
            if (ltWidget.Bounds.Height == 0)
                return ltWidget.Bounds.Y;
            else
                return (ltWidget.Bounds.Bottom - height);
        }
        /// <summary>
        /// Determines whether the paragraph is empty
        /// </summary>
        /// <param name="para">paragraph</param>
        /// <returns> returns true if paragraph is empty </returns>
        internal bool IsEmptyParagraph(WParagraph para)
        {
            bool isEmptyParagraph = false;
            if (para != null && para.Text == "" && para.Items.Count == 0)
                isEmptyParagraph = true;
            return isEmptyParagraph;
        }
        /// <summary>
        /// Draw AbsoluteTab
        /// </summary>
        /// <param name="absoluteTab"></param>
        /// <param name="ltWidget"></param>
        internal void DrawAbsoluteTab(WAbsoluteTab absoluteTab, LayoutedWidget ltWidget)
        {
            float x = ltWidget.Bounds.Left;
            float y = ltWidget.Bounds.Top;
            float height = ltWidget.Bounds.Height;
            StringFormat format = GetStringFormat(absoluteTab.CharacterFormat);
            string text = string.Empty;
            TabsLayoutInfo tabsInfo = absoluteTab.m_layoutInfo as TabsLayoutInfo;
            if (tabsInfo != null)
            {
                if (absoluteTab.Alignment != AbsoluteTabAlignment.Left)
                    UpdateAbsoluteTabLeader(absoluteTab, ltWidget, ref text);
            }
            SizeF textSize = MeasureString(text, absoluteTab.CharacterFormat.Font, format);
            RectangleF bounds = new RectangleF(x, y, textSize.Width + ltWidget.SubWidth, height);
            DrawString(text, absoluteTab.CharacterFormat, absoluteTab.GetOwnerParagraph().ParagraphFormat, bounds, ltWidget.Bounds.Width, ltWidget);
        }
        /// <summary>
        /// Update AbsoluteTab leader
        /// </summary>
        /// <param name="absoluteTab"></param>
        /// <param name="ltWidget"></param>
        /// <param name="text"></param>
        private void UpdateAbsoluteTabLeader(WAbsoluteTab absoluteTab, LayoutedWidget ltWidget, ref string text)
        {
            TabsLayoutInfo tabsInfo = absoluteTab.m_layoutInfo as TabsLayoutInfo;
            text = string.Empty;
            StringFormat format = GetStringFormat(absoluteTab.CharacterFormat);
            WTextRange textRange = new WTextRange(absoluteTab.Document);
            textRange.ApplyCharacterFormat(absoluteTab.CharacterFormat);
            if (absoluteTab.CharacterFormat != null
                && (absoluteTab.CharacterFormat.Font.Underline
                || absoluteTab.CharacterFormat.Font.Strikeout))
                FillSpace(textRange, ltWidget, format, ref text);
            switch (tabsInfo.CurrTabLeader)
            {
                case Layouting.TabLeader.Dotted:
                    FillDots(textRange, ltWidget, format, ref text);
                    break;
                case Layouting.TabLeader.Single:
                    FillSingle(textRange, ltWidget, format, ref text);
                    break;
                case Layouting.TabLeader.Hyphenated:
                    FillHyphens(textRange, ltWidget, format, ref text);
                    break;
            }
        }
        /// <summary>
        /// Draws the Footnote separator
        /// </summary>
        /// <param name="txtRange">The TXT range.</param>
        /// <param name="ltWidget">The lt widget.</param>
        internal void DrawSeparator(WTextRange txtRange, LayoutedWidget ltWidget)
        {
            Pen pen = new Pen(Color.Black, 0.5f);
            Graphics.DrawLine(pen, new PointF(ltWidget.Bounds.X, ltWidget.Bounds.Y + (ltWidget.Bounds.Height / 2)), new PointF(ltWidget.Bounds.Right, ltWidget.Bounds.Y + (ltWidget.Bounds.Height / 2)));
        }
        /// <summary>
        /// Draws the text range.
        /// </summary>
        /// <param name="txtRange">The TXT range.</param>
        /// <param name="ltWidget">The lt widget.</param>
        /// <param name="text">The text.</param>
        internal void DrawTextRange(WTextRange txtRange, LayoutedWidget ltWidget, string text)
        {
            //Draw Footnote separator
            if (text == ((char)3).ToString() || text == ((char)4).ToString())
            {
                DrawSeparator(txtRange,ltWidget);
                return;
            }
            IEntity ent = txtRange as IEntity;
            currTextRange = txtRange;
            if (ent.PreviousSibling is BookmarkStart)
            {
                BookmarkStart bookmarkstart = ent.PreviousSibling as BookmarkStart;
                if (IsOwnerParagraphContainBookMarkEnd(bookmarkstart.Name))
                {
                    for (int i = 0; i < BookmarkHyperlinksList.Count; i++)
                    {
                        foreach (KeyValuePair<string, BookmarkHyperlink> link in BookmarkHyperlinksList[i])
                        {
                            if (link.Key == bookmarkstart.Name)
                            {
                                link.Value.TargetBounds = ltWidget.Bounds;
                                link.Value.TargetPageNumber = Syncfusion.DocIO.DLS.Rendering.DocumentLayouter.PageNumber;
                                UpdateTOCLevel(txtRange.OwnerParagraph, link.Value);
                            }
                        }
                    }
                }
            }
            //if (ent.PreviousSibling != null && ent.PreviousSibling.PreviousSibling != null && ent.PreviousSibling.PreviousSibling.PreviousSibling != null
            //    && ent.PreviousSibling.PreviousSibling.PreviousSibling is WField)
            //{
            //    WField field = ent.PreviousSibling.PreviousSibling.PreviousSibling as WField;
            //    if (field != null && field.FieldType == FieldType.FieldHyperlink
            //        && field.FieldValue.Replace("\"", string.Empty).ToLower().StartsWith("_toc"))
            //        return;
            //}
            float x = ltWidget.Bounds.Left;
            float y = ltWidget.Bounds.Top;
            float width = ltWidget.Bounds.Width;
            float height = ltWidget.Bounds.Height;
            StringFormat format = GetStringFormat(txtRange.CharacterFormat);
            TabsLayoutInfo tabsInfo = txtRange.m_layoutInfo as TabsLayoutInfo;
            if (tabsInfo != null)
            {
                UpdateTabLeader(txtRange, ltWidget, ref text);
                txtRange.Text = "\t";
            }
            SizeF Textsize = ltWidget.Widget.LayoutInfo.Size;
            if (text == ((char)2).ToString() && txtRange.GetOwnerParagraph().OwnerTextBody.Owner is WFootnote)
                text = ((txtRange.GetOwnerParagraph().OwnerTextBody.Owner as WFootnote).m_layoutInfo as FootnoteLayoutInfo).FootnoteID;
            if (txtRange.OwnerParagraph != null
                && txtRange.Text == text
                && txtRange.GetIndexInOwnerCollection() == txtRange.OwnerParagraph.Items.Count - 1 //Checks for last item
                && txtRange.Text.Trim() != string.Empty) //Text should not be equal to empty when trimmed.
                text = text.TrimEnd();
            else
                Textsize = MeasureTextRange(txtRange, text);
            RectangleF bounds = new RectangleF(x, y, Textsize.Width + ltWidget.SubWidth, height);
#if DEBUG_LAYOUTING
      Pen pen = new Pen( Color.Gray );
      Graphics.DrawRectangle(pen, ltWidget.Bounds.X, ltWidget.Bounds.Y,
        ltWidget.Bounds.Width, ltWidget.Bounds.Height);
#endif
            WParagraphFormat paraFormat = null;
            if (txtRange.OwnerParagraph != null)
                paraFormat = txtRange.OwnerParagraph.ParagraphFormat;

            WCharacterFormat charFormat = txtRange.CharacterFormat;

            if (ltWidget.SkipLeftPosition > 0)
            {
                if (ltWidget.Bounds.Width < ltWidget.SkipLeftPosition)
                    return;
                text = GetText(txtRange, ltWidget, ref bounds);
            }
            /* if (currParagraph != null && currParagraph.StyleName != null && currParagraph.ParaStyle != null &&
                 currParagraph.StyleName != string.Empty && currParagraph.StyleName != "Normal" &&
                 currParagraph.StyleName.ToLower() != "header" && currParagraph.StyleName.ToLower() != "footer")
             {
                 //charFormat = currParagraph.GetStyle().CharacterFormat as WCharacterFormat;
             }
            

             if (txtRange.CharacterFormat.CharStyleName != null && txtRange.CharacterFormat.CharStyleName != "Normal"
                 && txtRange.CharacterFormat.CharStyle != null)
             {
                 //Style cStyle = txtRange.Document.Styles.FindByName(txtRange.CharacterFormat.CharStyleName) as Style;
                 //charFormat = cStyle.CharacterFormat as WCharacterFormat;
             }*/

            SolidBrush brush = new SolidBrush(charFormat.TextColor);
            bool IsJustified = (ltWidget.HorizontalAlign == Syncfusion.Layouting.HorizontalAlignment.Distributed || ltWidget.HorizontalAlign == Syncfusion.Layouting.HorizontalAlignment.Justify);
            if ((!IsJustified || ltWidget.IsLastLine) && ltWidget.HorizontalAlign != Syncfusion.Layouting.HorizontalAlignment.Distributed && !IsOwnerParagraphEmpty(text))
                DrawString(text, charFormat, paraFormat, bounds, ltWidget.Bounds.Width, ltWidget);
            else if (IsJustified)
            {
                if (IsTextRangeFollowWithTab(ltWidget))
                    DrawString(text, charFormat, paraFormat, bounds, ltWidget.Bounds.Width, ltWidget);
                else
                    DrawJustifiedLine(text, charFormat, paraFormat, bounds, ltWidget);
            }
            //Add Hyperlink
            if (currHyperlink != null)
            {
                AddHyperLink(currHyperlink, ltWidget.Bounds);
                if ((txtRange.NextSibling is WFieldMark) && (txtRange.NextSibling as WFieldMark).Type == FieldMarkType.FieldEnd)
                    currHyperlink = null;
            }
        }
        /// <summary>
        /// Determine whether the paragraph containing bookmark end
        /// </summary>
        /// <param name="bk_Name"></param>
        /// <returns></returns>
        private bool IsOwnerParagraphContainBookMarkEnd(string bk_Name)
        {
            WParagraph ownerParagraph = currTextRange.GetOwnerParagraph();
            int index = ownerParagraph.ChildEntities.IndexOf(currTextRange) + 1;
            for (int i = index; i < ownerParagraph.ChildEntities.Count; i++)
            {
                if (ownerParagraph.ChildEntities[i] is BookmarkEnd
                   && (ownerParagraph.ChildEntities[i] as BookmarkEnd).Name == bk_Name)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Determine whether the tab stop is preserved after the text range in the current line
        /// </summary>
        /// <param name="ltWidget"></param>
        /// <returns></returns>
        private bool IsTextRangeFollowWithTab(LayoutedWidget ltWidget)
        {
            int index = ltWidget.Owner.ChildWidgets.IndexOf(ltWidget);
            for (int i = index; i < ltWidget.Owner.ChildWidgets.Count; i++)
            {
                if ((ltWidget.Owner.ChildWidgets[i].Widget.LayoutInfo as TabsLayoutInfo) != null)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Get the text to Draw
        /// </summary>
        /// <param name="textRange"></param>
        /// <param name="ltWidget"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private string GetText(WTextRange textRange, LayoutedWidget ltWidget, ref RectangleF bounds)
        {
            char[] character = textRange.Text.ToCharArray();
            float textWidth = 0;
            int index = 0;
            string splittedText = null;
            for (int i = 0; i < textRange.Text.Length; i++)
            {
                SizeF textSize = MeasureString(character[i].ToString(), GetFont(textRange, textRange.CharacterFormat, character[i].ToString()), GetStringFormat(textRange.CharacterFormat), textRange.CharacterFormat,false);
                textWidth += textSize.Width;
                if (textWidth >= ltWidget.SkipLeftPosition)
                {
                    index = i;
                    break;
                }
            }
            for (int i = index; i < textRange.Text.Length; i++)
                splittedText += character[i];
            bounds = new RectangleF(ltWidget.Bounds.X + ltWidget.SkipLeftPosition, ltWidget.Bounds.Y, ltWidget.Bounds.Width - ltWidget.SkipLeftPosition, ltWidget.Bounds.Height);
            return splittedText;
        }
        /// <summary>
        /// Update Tab Width.
        /// </summary>
        private void UpdateTabLeader(WTextRange txtRange, LayoutedWidget ltWidget, ref string text)
        {
            TabsLayoutInfo tabsInfo = txtRange.m_layoutInfo as TabsLayoutInfo;
            text = string.Empty;
            StringFormat format = GetStringFormat(txtRange.CharacterFormat);
            if (IsTOC(txtRange))
                format = txtRange.GetOwnerParagraph().ParaStyle != null ? GetStringFormat(txtRange.GetOwnerParagraph().ParaStyle.CharacterFormat) : format;
            if (txtRange.CharacterFormat != null
                && (txtRange.CharacterFormat.Font.Underline
                || txtRange.CharacterFormat.Font.Strikeout))
                FillSpace(txtRange, ltWidget, format, ref text);
            switch (tabsInfo.CurrTabLeader)
            {
                case Layouting.TabLeader.Dotted:
                    FillDots(txtRange, ltWidget, format, ref text);
                    break;
                case Layouting.TabLeader.Single:
                    FillSingle(txtRange, ltWidget, format, ref text);
                    break;
                case Layouting.TabLeader.Hyphenated:
                    FillHyphens(txtRange, ltWidget, format, ref text);
                    break;
            }
        }

        /// <summary>
        /// Fills with dots
        /// </summary>
        /// <param name="leafWidget"></param>
        /// <param name="width"></param>
        private void FillDots(WTextRange txtRange, LayoutedWidget ltWidget, StringFormat format, ref string text)
        {
            text = string.Empty;
            float Width = MeasureString(".", GetFont(txtRange, txtRange.CharacterFormat,"."), format).Width;
            float temp = Width;
            while (temp <= ltWidget.Bounds.Width)
            {
                text += ".";
                temp += Width;
            }
        }
        /// <summary>
        /// Fills with Single
        /// </summary>
        /// <param name="leafWidget"></param>
        /// <param name="width"></param>
        private void FillSingle(WTextRange txtRange, LayoutedWidget ltWidget, StringFormat format, ref string text)
        {
            text = string.Empty;
            float width = MeasureString("_", GetFont(txtRange, txtRange.CharacterFormat,"_"), format).Width;
            float temp = width;
            while (temp <= ltWidget.Bounds.Width)
            {
                text += "_";
                temp += width;
            }
        }
        /// <summary>
        /// Fills with Hyphens
        /// </summary>
        /// <param name="leafWidget"></param>
        /// <param name="width"></param>
        private void FillHyphens(WTextRange txtRange, LayoutedWidget ltWidget, StringFormat format, ref string text)
        {
            text = string.Empty;
            float width = MeasureString("-", GetFont(txtRange, txtRange.CharacterFormat,"-"), format).Width;
            float temp = width;
            while (temp <= ltWidget.Bounds.Width)
            {
                text += "-";
                temp += width;
            }
        }
        /// <summary>
        /// Fills with space
        /// </summary>
        /// <param name="leafWidget">The leaf widget</param>
        /// <param name="width">Tab width</param>
        private void FillSpace(WTextRange txtRange, LayoutedWidget ltWidget, StringFormat format, ref string text)
        {
            text = string.Empty;
            while (MeasureString(text, GetFont(txtRange, txtRange.CharacterFormat, text), format).Width <= ltWidget.Bounds.Width)
            {
                text += " ";
            }
        }
        /// <summary>
        /// Draws the Symbol
        /// </summary>
        /// <param name="wSymbol"></param>
        /// <param name="ltWidget"></param>
        internal void DrawSymbol(WSymbol symbol, LayoutedWidget ltWidget)
        {
            string text = Char.ConvertFromUtf32(symbol.CharacterCode);
            WCharacterFormat charFormat = new WCharacterFormat(symbol.Document);
            // Set clipping bounds
            float clipWidth = ltWidget.Bounds.Width;
            if (clipWidth == 0 || IsWidgetNeedToClipBasedOnXPosition(ltWidget, ref clipWidth, ltWidget.Bounds))
            {
                Graphics.ResetTransform();
                return;
            }
            RectangleF clipBounds = new RectangleF(ltWidget.Bounds.X, ltWidget.Bounds.Y, clipWidth, ltWidget.Bounds.Height);
            Graphics.SetClip(clipBounds, CombineMode.Replace);
            if (!symbol.CharacterFormat.HasValue(WCharacterFormat.FontKey) && symbol.FontName != string.Empty && symbol.FontName != symbol.CharacterFormat.FontName)
            {
                charFormat.ImportContainer(symbol.CharacterFormat);
                charFormat.CopyProperties(symbol.CharacterFormat);
                charFormat.ApplyBase(symbol.GetOwnerParagraph().BreakCharacterFormat.BaseFormat);
                charFormat.FontName = symbol.FontName;
                Graphics.DrawString(text, GetFont(charFormat, text), GetBrush(GetTextColor(symbol.CharacterFormat)), ltWidget.Bounds, GetStringFormat(symbol.CharacterFormat));
            }
            else
                Graphics.DrawString(text, GetFont(symbol.GetCharFormat(), text), GetBrush(GetTextColor(symbol.CharacterFormat)), ltWidget.Bounds, GetStringFormat(symbol.CharacterFormat));
            // Reset clipping bounds
            Graphics.ResetClip();
        }
        /// <summary>
        /// Draws the mail-merge field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="ltWidget">The lt widget.</param>
        internal void DrawMergeField(WMergeField field, LayoutedWidget ltWidget)
        {
            string fieldText = (field.FieldValue == string.Empty || ltWidget.Widget is SplitStringWidget) ? field.UpdateMergeFieldText(field) : field.FieldValue;
            WCharacterFormat charFormat = field.CharacterFormat;
            WParagraphFormat paraFormat = field.GetOwnerParagraph().ParagraphFormat;
            currTextRange = field as WTextRange;
            RectangleF bounds = ltWidget.Bounds;

            //Reset Graphics Transform Position
            Graphics.ResetTransform();
            //Transform Graphics Position
            TransformGraphicsPosition(ltWidget);

            string text = null;
            for (int i = 0; i < field.TextItems.Count; i++)
            {
                text += (field.TextItems[i] as WTextRange).Text;
            }
            if (field.TextItems.Count > 1
                && (field.TextItems[0] as WTextRange).CharacterFormat != (field.TextItems[1] as WTextRange).CharacterFormat
                && text == fieldText)
            {
                for (int i = 0; i < field.TextItems.Count; i++)
                {
                    if (bounds.Width <= 0 || bounds.Height <= 0)
                        break;
                    currTextRange = field.TextItems[i] as WTextRange;
                    text = (field.FieldValue == string.Empty || ltWidget.Widget is SplitStringWidget) ? currTextRange.Text : field.FieldValue;
                    if ((field.TextItems[i] as WTextRange).CharacterFormat.HasKey(WCharacterFormat.FontKey))
                        charFormat = currTextRange.CharacterFormat;
                    else
                        charFormat = field.CharacterFormat;
                    SizeF textSize = MeasureString(text, currTextRange.CharacterFormat.Font, GetStringFormat(currTextRange.CharacterFormat), currTextRange.CharacterFormat,false);
                    DrawString(text, charFormat, paraFormat, new RectangleF(bounds.X, bounds.Y, textSize.Width, bounds.Height), textSize.Width, ltWidget);
                    bounds.X += textSize.Width;
                    bounds.Width -= textSize.Width;
                }
            }
            else
            {
                DrawString(fieldText, charFormat, paraFormat, bounds, ltWidget.Bounds.Width, ltWidget);
            }
            //Reset Graphics Transform Position
            Graphics.ResetTransform();
        }
        #endregion
        #region Implementation-Drawings

        #region Text & Images
        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="unScaled">if set to <c>true</c> [un scaled].</param>
        internal void DrawPicture(WPicture picture, LayoutedWidget widget)
        {
            if (picture != null)
            {
                Image image = picture.Image;
                if (image == null)
                    return;
                SizeF size = MeasureImage(picture);
                RectangleF bounds = widget.Bounds;

                if (float.IsNaN(bounds.X))
                {
                    bounds.X = 0;
                }
                //Reset Graphics Transform Position
                Graphics.ResetTransform();
                //Transform Graphics position
                if (widget.Widget.LayoutInfo.IsVerticalText)
                {
                    WParagraph ownerParagraph = (widget.Widget as WPicture).GetOwnerParagraph();
                    if (ownerParagraph.IsInCell)
                    {
                        WTableCell tableCell = ownerParagraph.OwnerTextBody as WTableCell;
                        float cellLeftMargin = (tableCell.m_layoutInfo as TableLayoutInfo).TableCellLeftMargin;
                        float cellTopMArgin = GetCellTopMargin(widget);
                        float cellWidth = (float)(tableCell.m_layoutInfo as TableLayoutInfo).VerticalCellWidth;
                        float leftPad = (float)(picture.GetOwnerParagraph().OwnerTextBody as WTableCell).m_layoutInfo.Paddings.Left;
                        float yposition = cellTopMArgin - bounds.Y + (bounds.X - cellLeftMargin) + leftPad;
                        if (tableCell.CellFormat.TextDirection == TextDirection.VerticalTopToBottom)
                        {
                            float xPosition = GetCellHeightForVerticalText(picture) + cellLeftMargin - bounds.Y - bounds.Width - leftPad;
                            Graphics.TranslateTransform(xPosition, yposition);
                        }
                        else
                        {
                            Graphics.TranslateTransform((bounds.Y - cellTopMArgin), yposition);
                        }
                    }
                    else if (ownerParagraph.Owner.Owner is Shape)
                    {
                        Shape shape = ownerParagraph.Owner.Owner as Shape;
                        float cellLeftMargin = (shape.m_layoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Left;
                        float cellTopMArgin = (shape.m_layoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Top;
                        float cellWidth = (float)(shape.m_layoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Width;
                        float leftPad = (float)shape.TextFrame.InternalMargin.Left;
                        float yposition = cellTopMArgin - bounds.Y + (bounds.X - cellLeftMargin) + leftPad;
                        if (shape.TextFrame.TextDirection == TextDirection.VerticalTopToBottom)
                        {
                            float xPosition = GetCellHeightForVerticalText(picture) + cellLeftMargin - bounds.Y - bounds.Width - leftPad;
                            Graphics.TranslateTransform(xPosition, yposition);
                        }
                        else
                        {
                            Graphics.TranslateTransform((bounds.Y - cellTopMArgin), yposition);
                        }
                    }
                }
                float clipTop = 0;
                if (picture.TextWrappingStyle == TextWrappingStyle.Inline)
                {
                    clipTop = GetClipTopPosition(bounds, true) * 2;
                    if (clipTop > 0)
                    {
                        FontMetric fontMetric = new FontMetric(picture.GetOwnerParagraph().BreakCharacterFormat.Font, Graphics);
                        clipTop += (float)fontMetric.Descent;
                    }
                }
                float clipWidth = bounds.Width;
                if (clipWidth == 0 ||IsWidgetNeedToClipBasedOnXPosition(widget, ref clipWidth, bounds))
                {
                    Graphics.ResetTransform();
                    return;
                }
                //Clip the image based on the cell bounds only when it contains allowInTableCell property is true.
                if (picture.LayoutInCell)
                    Graphics.SetClip(GetClipBounds(bounds, clipWidth, clipTop));
                Graphics.DrawImage(image, bounds.X, bounds.Y, size.Width, size.Height);
                Graphics.ResetClip();
                image.Dispose();
                //Add Hyperlink
                if (currHyperlink != null)
                {
                    AddHyperLink(currHyperlink, widget.Bounds);
                    if ((picture.NextSibling is WFieldMark) && (picture.NextSibling as WFieldMark).Type == FieldMarkType.FieldEnd)
                        currHyperlink = null;
                }
                //Reset Graphics Transform position
                Graphics.ResetTransform();
            }
        }

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="charFormat">The char format.</param>
        /// <param name="paraFormat">The para format.</param>
        /// <param name="bounds">The bounds.</param>
        internal void DrawString(string text, WCharacterFormat charFormat, WParagraphFormat paraFormat, RectangleF bounds, float clipWidth,LayoutedWidget ltWidget)
        {
            if (text == null || bounds.Height == 0)
                return;

            text = text.Replace(NONBREAK_HYPHEN.ToString(), "-");
            text = text.Replace(SOFT_HYPHEN.ToString(), "-");

            Font font = null;
            if (!IsListCharacter && currTextRange != null)
                font = GetFont(currTextRange, charFormat, text);
            else
                font = GetFont(charFormat, text);
            Color textColor = Color.Black;
            StringFormat format = GetStringFormat(charFormat);
            if (IsTOC(currTextRange))
            {
                format = currParagraph != null && currParagraph.ParaStyle != null ? GetStringFormat(currParagraph.ParaStyle.CharacterFormat) : format;
            }

            textColor = GetTextColor(charFormat);

            Brush textBrush = GetBrush(textColor);
            float height = bounds.Height;
            float Y = bounds.Y;
            if (paraFormat != null)
            {
                height = bounds.Height - paraFormat.Borders.Bottom.LineWidth - paraFormat.Borders.Top.LineWidth;
                Y = bounds.Y + paraFormat.Borders.Top.LineWidth;
                //Check whether the current paragraph is in table cell 
                if (currParagraph != null && currParagraph.IsInCell && Y == GetCellTopMargin(ltWidget))
                {
                    //Add the top cell border to the highlight color y position and reduce it from the height of the highlight color.
                    Y += (float)(currParagraph.OwnerTextBody as WTableCell).m_layoutInfo.Margins.Top;
                    height -= (float)(currParagraph.OwnerTextBody as WTableCell).m_layoutInfo.Margins.Top;
                }
                else if (currParagraph != null && ((currParagraph.OwnerTextBody.Owner is Shape)
                    && ((currParagraph.OwnerTextBody.Owner as Shape).m_layoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Y == Y))
                {
                    Y += (float)(currParagraph.OwnerTextBody.Owner as Shape).LineFormat.Weight;
                    height -= (float)(float)(currParagraph.OwnerTextBody.Owner as Shape).LineFormat.Weight;
                }
            }
            // Draw text background
            if (!charFormat.TextBackgroundColor.IsEmpty)
            {
                SolidBrush bgBrush = GetBrush(charFormat.TextBackgroundColor);
                Graphics.FillRectangle(bgBrush, bounds.X, Y, bounds.Width, height);
            }

            if (!charFormat.HighlightColor.IsEmpty)
            {
                Color hightLightColor = charFormat.HighlightColor;
                //Bright Green
                if (hightLightColor == Color.Green)
                    hightLightColor = Color.FromArgb(255, 0, 255, 0);
                //Dark Yellow
                if (hightLightColor == Color.Gold)
                    hightLightColor = Color.FromArgb(255, 128, 128, 0);
                //Gray-50% 
                if (hightLightColor.Name == "808080")
                    hightLightColor = Color.FromArgb(255, 128, 128, 128);
                SolidBrush hltBrush = new SolidBrush(hightLightColor);
                Graphics.FillRectangle(hltBrush, bounds.X, Y, bounds.Width, height);
            }
            if (IsListCharacter)
                bounds.Width = MeasureString(text, font, null, charFormat,true).Width;

            //Reset Graphics Transform Position
            Graphics.ResetTransform();
            //Transform Graphics Position
            TransformGraphicsPosition(ltWidget);
            // Set clipping bounds
            float clipTop = GetClipTopPosition(bounds, false);
            if (clipWidth == 0 || IsTextNeedToClip(ltWidget) || IsWidgetNeedToClipBasedOnXPosition(ltWidget, ref clipWidth, bounds))
            {
                Graphics.ResetTransform();
                return;
            }
            RectangleF clipBounds = GetClipBounds(bounds, clipWidth, clipTop);
            Graphics.SetClip(clipBounds, CombineMode.Replace);
            // Embos writing
            if (charFormat.Emboss)
            {
                SolidBrush brush2 = new SolidBrush(Color.Gray);
                if (IsUnicode(text))
                    Graphics.DrawString(text, charFormat.Font, brush2, bounds.X + DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Y + DEF_EMBOSS_ENGRAVE_FACTOR, format);
                else
                    Graphics.DrawString(text, charFormat.Font, brush2, new RectangleF(bounds.X + DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Y + DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Width, bounds.Height), format);
            }

            // Engrave writing
            if (charFormat.Engrave)
            {
                SolidBrush brush2 = new SolidBrush(Color.Gray);
                if (IsUnicode(text))
                    Graphics.DrawString(text, charFormat.Font, brush2, bounds.X - DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Y - DEF_EMBOSS_ENGRAVE_FACTOR, format);
                else
                    Graphics.DrawString(text, charFormat.Font, brush2, new RectangleF(bounds.X - DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Y - DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Width, bounds.Height), format);
            }
            // Apply Allcaps
            if (charFormat.AllCaps)
            {
                text = text.ToUpper();
            }
            if (IsOwnerParagraphEmpty(text))
            {
                if (IsUnicode(text))
                    Graphics.DrawString("", font, textBrush, bounds.X, bounds.Y, format);
                else
                    Graphics.DrawString("", font, textBrush, new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height), format);
            }
            else
            {
                if (IsUnicode(text))
                {
                    //Draw Unicode Text
                    DrawChineseText(text, charFormat, font, textBrush, bounds, format);
                }
                else
                {
                    // Apply Smallcaps
                    if (charFormat.SmallCaps)
                    {
                        text = text.ToUpper();
                    }
                    //Draw String based on CharacterSpacing
                    if (charFormat.CharacterSpacing != 0)
                        DrawStringBasedOnCharSpacing(font, textBrush, bounds, text, format);
                    else
                        Graphics.DrawString(text, font, textBrush, new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height), format);
                }
            }
            // Reset clipping bounds
            Graphics.ResetClip();
            IsListCharacter = false;
            //Reset Graphics Transform Position
            Graphics.ResetTransform();
        }
        /// <summary>
        /// Determine whether the text is need to clip when the text range y position is greater than the owner row bottom position
        /// </summary>
        /// <returns></returns>
        private bool IsTextNeedToClip(LayoutedWidget ltWidget)
        {
            if (ltWidget != null && !ltWidget.Widget.LayoutInfo.IsVerticalText && currParagraph != null && currParagraph.IsInCell)
            {
                LayoutedWidget ownerLtWidget = ltWidget.Owner;
                LayoutedWidget ownerCellWidget = ltWidget.Owner;
                while (ownerLtWidget != null && (!(ownerLtWidget.Widget is WTableRow) || !(ownerLtWidget.Widget is Shape)))
                {
                    if (ownerLtWidget.Widget is WTableCell)
                        ownerCellWidget = ownerLtWidget;
                    ownerLtWidget = ownerLtWidget.Owner;
                }
                if (ownerCellWidget != null && (ownerCellWidget.Widget is WTableCell)
                    && !(ownerCellWidget.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart
                    && ownerLtWidget != null && ownerLtWidget.Widget is WTableRow)
                    return (Math.Round(ltWidget.Bounds.Y, 2) >= Math.Round(ownerLtWidget.Bounds.Bottom, 2));
                else if (ownerLtWidget != null && (ownerLtWidget.Widget is Shape))
                    return (Math.Round(ltWidget.Bounds.Y, 2) >= Math.Round((ownerLtWidget.Widget.LayoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Bottom, 2));
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// Determine whether the text is need to clip when the text range x position is beyond the cell bounds or crossing the cell bounds
        /// </summary>
        /// <returns></returns>
        private bool IsWidgetNeedToClipBasedOnXPosition(LayoutedWidget ltWidget, ref float clipWidth, RectangleF bounds)
        {
            float right = -1;
            if (ltWidget != null && !ltWidget.Widget.LayoutInfo.IsVerticalText && currParagraph != null)
            {
                LayoutedWidget ownerLtWidget = ltWidget.Owner;
                if (currParagraph.IsInCell)
                {
                    LayoutedWidget ownerCellWidget = ltWidget.Owner;
                    while (ownerLtWidget != null && !(ownerLtWidget.Widget is WTableRow))
                    {
                        if (ownerLtWidget.Widget is WTableCell)
                            ownerCellWidget = ownerLtWidget;
                        ownerLtWidget = ownerLtWidget.Owner;
                    }
                    if (ownerCellWidget != null && (ownerCellWidget.Widget is WTableCell))
                        right = ownerCellWidget.Bounds.Right;
                }
                else if (currParagraph.OwnerTextBody.Owner is Shape)
                    right = ((currParagraph.OwnerTextBody.Owner as Shape).m_layoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Right;
                else
                {
                    Entity baseEntity = GetBaseEntity(currParagraph as Entity);
                    while (ownerLtWidget != null && !(ownerLtWidget.Widget is WSection || (ownerLtWidget.Widget is SplitWidgetContainer && (ownerLtWidget.Widget as SplitWidgetContainer).RealWidgetContainer is WSection)))
                    {
                        ownerLtWidget = ownerLtWidget.Owner;
                    }
                    //If section has more than 1 columns
                    if (ownerLtWidget != null && (baseEntity is WSection && (baseEntity as WSection).Columns.Count > 1))
                    {
                        int columnIndex = GetColumnIndex(baseEntity as WSection, ownerLtWidget.Bounds);
                        // If current column is not the last column
                        if (columnIndex != (baseEntity as WSection).Columns.Count - 1)
                            right = ownerLtWidget.Bounds.X + (baseEntity as WSection).Columns[columnIndex].Width + ((baseEntity as WSection).Columns[columnIndex].Space / 2);
                    }
                }
            }
            //If text bounds entirely outside the column bounds
            if (right > 0 && bounds.X > right)
                return true;
            //If text bound crossing the column bounds, update the text bounds width
            else if (right > 0 && bounds.Right > right)
                clipWidth = right - bounds.X;
            return false;
        }
        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="sectionBounds">The section bounds.</param>
        /// <returns></returns>
        private int GetColumnIndex(WSection section,RectangleF sectionBounds)
        {
            int columnIndex = 0;
            float columnRight = m_pageMarginLeft;
            for (int i = 0; i < section.Columns.Count; i++)
            {
                columnRight += section.Columns[i].Width;
                if (sectionBounds.X < columnRight)
                {
                    columnIndex = i;
                    break;
                }
                columnRight += section.Columns[i].Space;
            }
            return columnIndex;
        }
        /// <summary>
        /// Get Y position to clip paragraph items
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private float GetClipTopPosition(RectangleF bounds, bool isInlinePicture)
        {
            float clipTop = 0;
            if (currParagraph != null && (currParagraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.Exactly
                || (bounds.Height * (Math.Abs(currParagraph.ParagraphFormat.LineSpacing) / 12) < 12
                && currParagraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.Multiple
                && !isInlinePicture))
                && Math.Abs(currParagraph.ParagraphFormat.LineSpacing) < bounds.Height)
            {
                float topMargin = (float)((currParagraph as IWidget).LayoutInfo as ParagraphLayoutInfo).Margins.Top;
                float lineSpacing = Math.Abs(currParagraph.ParagraphFormat.LineSpacing);
                if (bounds.Height * (Math.Abs(currParagraph.ParagraphFormat.LineSpacing) / 12) < 12
                    && currParagraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.Multiple)
                    lineSpacing = bounds.Height * (Math.Abs(currParagraph.ParagraphFormat.LineSpacing) / 12);
                if (topMargin + lineSpacing < bounds.Height)
                    clipTop = (bounds.Height - lineSpacing - topMargin) / 2;
            }
            return clipTop;
        }
        /// <summary>
        /// Get Default font to render non east characters
        /// </summary>
        /// <param name="font"></param>
        /// <param name="charFormat"></param>
        /// <returns></returns>
        internal Font GetDefaultFont(Font font, WCharacterFormat charFormat)
        {
            if (charFormat.HasValue(WCharacterFormat.IdctHintKey))
            {
                string fontName = charFormat.GetFontNameFromHint();
                if (charFormat.FontNameNonFarEast != fontName)
                    return new Font(charFormat.FontNameNonFarEast, font.Size, font.Style);
            }
            return font;
        }
        /// <summary>
        /// Draw String based on CharacterSpacing
        /// </summary>
        /// <param name="font"></param>
        /// <param name="textBrush"></param>
        /// <param name="bounds"></param>
        /// <param name="text"></param>
        /// <param name="format"></param>
        public void DrawStringBasedOnCharSpacing(Font font, Brush textBrush, RectangleF bounds, string text, StringFormat format)
        {
            //Calculate spacing between each character
            float widthNeeded = 0;
            foreach (char c in text)
            {
                widthNeeded += MeasureString(c.ToString(), font, format).Width;
            }
            float spacing = (bounds.Width - widthNeeded) / (text.Length - 1);

            //Draw string
            float indent = 0;
            foreach (char c in text)
            {
                float width = MeasureString(c.ToString(), font, format).Width;
                if (bounds.X <= bounds.X + indent)
                {
                    Graphics.DrawString(c.ToString(), font, textBrush, new RectangleF(bounds.X + indent, bounds.Y, width, bounds.Height), format);
                    //Draw Empty space with character spacing width to fill the underline gap between two characters
                    if (spacing > 0)
                        Graphics.DrawString(" ", font, textBrush, new RectangleF(bounds.X + indent + width, bounds.Y, spacing, bounds.Height), format);
                }
                else
                    Graphics.DrawString(c.ToString(), font, textBrush, new RectangleF(bounds.X, bounds.Y, width, bounds.Height), format);
                indent += width + spacing;
            }
        }
        /// <summary>
        /// Transform Graphics Position
        /// </summary>
        private void TransformGraphicsPosition(LayoutedWidget ltWidget)
        {
            if (currTextRange != null && currTextRange.m_layoutInfo != null && currTextRange.m_layoutInfo.IsVerticalText)
            {
                WParagraph ownerParagraph = currTextRange.OwnerParagraph;
                if (ownerParagraph == null)
                    ownerParagraph = currTextRange.GetOwnerParagraph();
                if (ownerParagraph.IsInCell)
                {
                    WTableCell tableCell = ownerParagraph.OwnerTextBody as WTableCell;
                    float cellLeftMargin = (tableCell.m_layoutInfo as TableLayoutInfo).TableCellLeftMargin;
                    float cellTopMArgin = (tableCell.m_layoutInfo as TableLayoutInfo).TableCellTopMargin;
                    if (ltWidget != null)
                        cellTopMArgin = GetCellTopMargin(ltWidget);
                    float cellWidth = (float)(tableCell.m_layoutInfo as TableLayoutInfo).VerticalCellWidth;
                    float cellHeight = GetCellHeightForVerticalText(currTextRange);
                    float leftPad = (float)(ownerParagraph.OwnerTextBody as WTableCell).m_layoutInfo.Paddings.Left;
                    float bottomPad = (float)(ownerParagraph.OwnerTextBody as WTableCell).m_layoutInfo.Paddings.Bottom;
                    float topPad = (float)(ownerParagraph.OwnerTextBody as WTableCell).m_layoutInfo.Paddings.Top;
                    float shiftWidth = GetWidthToShiftVerticalText(tableCell, cellHeight);
                    //Rotate Graphics object into 90 degree for the Vertical Text Direction as Top to Bottom
                    if (tableCell.CellFormat.TextDirection == TextDirection.VerticalTopToBottom)
                    {
                        Graphics.TranslateTransform(cellHeight + cellLeftMargin + cellTopMArgin + leftPad - shiftWidth, leftPad - (cellLeftMargin - cellTopMArgin) - topPad);
                        Graphics.RotateTransform(90);
                    }
                    //Rotate Graphics object into 270 degree for the Vertical Text Direction as Bottom to Top
                    else
                    {
                        Graphics.TranslateTransform(cellLeftMargin - cellTopMArgin - leftPad + shiftWidth, cellWidth + cellLeftMargin + cellTopMArgin - leftPad - bottomPad);
                        Graphics.RotateTransform(270);
                    }
                }
                else if (ownerParagraph.Owner.Owner is Shape)
                {
                    Shape shape = ownerParagraph.Owner.Owner as Shape;
                    float cellLeftMargin = (shape.m_layoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Left;
                    float cellTopMArgin = (shape.m_layoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Top;
                    float cellWidth = (float)(shape.m_layoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Width;
                    float cellHeight = (float)(shape.m_layoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Height;
                    float leftPad = (float)shape.TextFrame.InternalMargin.Left;
                    float bottomPad = (float)shape.TextFrame.InternalMargin.Bottom;
                    float topPad = (float)shape.TextFrame.InternalMargin.Top;
                    if (shape.TextFrame.TextDirection == TextDirection.VerticalTopToBottom)
                    {
                        Graphics.TranslateTransform(cellHeight + cellLeftMargin + cellTopMArgin + leftPad, leftPad - (cellLeftMargin - cellTopMArgin) - topPad);
                        Graphics.RotateTransform(90);
                    }
                    else
                    {
                        Graphics.TranslateTransform(cellLeftMargin - cellTopMArgin - leftPad, cellWidth + cellLeftMargin + cellTopMArgin - leftPad - bottomPad);
                        Graphics.RotateTransform(270);
                    }
                }
            }
        }
        /// <summary>
        /// Get Top Margin of the cell
        /// </summary>
        /// <param name="ltWidget"></param>
        /// <returns></returns>
        private float GetCellTopMargin(LayoutedWidget ltWidget)
        {
            while (ltWidget != null && !(ltWidget.Widget is WTableCell))
            {
                ltWidget = ltWidget.Owner;
            }
            if (ltWidget != null && ltWidget.Widget is WTableCell)
                return ltWidget.TopMargin;
            else
                return 0;
        }
        /// <summary>
        /// Get Width to shift vertical text
        /// </summary>
        /// <param name="tableCell"></param>
        /// <returns></returns>
        private float GetWidthToShiftVerticalText(WTableCell tableCell,float cellHeight)
        {
            float shiftWidth = 0;
            float cellLayoutedHeight =(float) (tableCell.m_layoutInfo as TableLayoutInfo).CellHeight;
            switch ((byte)tableCell.CellFormat.VerticalAlignment)
            {
                case 1:
                    shiftWidth = (cellHeight - cellLayoutedHeight) / 2;
                    break;
                case 2:
                    shiftWidth = cellHeight - cellLayoutedHeight;
                    break;
            }
            if (shiftWidth < 0)
                shiftWidth = 0;
            return shiftWidth;
        }
        /// <summary>
        /// Get Bounds to clip the text
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="clipWidth"></param>
        /// <returns></returns>
        private RectangleF GetClipBounds(RectangleF bounds, float clipWidth, float clipTop)
        {
            float clipX = bounds.X;
            float clipY = bounds.Y + clipTop;
            if (clipX % 0.75 != 0 && Math.Round(clipX % 0.75, 2) > 0.02)
            {
                float diff = (float)(clipX - Math.Round(clipX % 0.75, 2));
                if (diff % 0.75 < 0.03)
                    clipX = (float)Math.Round((clipX - Math.Round(clipX % 0.75, 2)), 2);
                else
                    clipX = (float)Math.Round((clipX - Math.Round(clipX % 0.75, 2) - 0.75), 2);
            }
            if (clipY % 0.75 != 0 && Math.Round(clipY % 0.75, 2) > 0.02)
            {
                float diff = (float)(clipY - Math.Round(clipY % 0.75, 2));
                if (diff % 0.75 < 0.03)
                    clipY = (float)Math.Round((clipY - Math.Round(clipY % 0.75, 2)), 2);
                else
                    clipY = (float)Math.Round((clipY - Math.Round(clipY % 0.75, 2) - 0.75), 2);
            }
            clipWidth += bounds.X - clipX;
            float clipheight = bounds.Height + bounds.Y - clipY;
            return new RectangleF(clipX, clipY, clipWidth, clipheight);
        }
        /// <summary>
        /// Get Height of the cell with text direction as vertical
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        private float GetCellHeightForVerticalText(Entity ent)
        {
            WTableCell tableCell = (ent as ParagraphItem).GetOwnerParagraph().OwnerTextBody as WTableCell;
            float cellSpacing = 0;
            if (tableCell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                cellSpacing = tableCell.OwnerRow.OwnerTable.TableFormat.CellSpacing * 2;
            float height = tableCell.Width;

            float leftPadding = (float)((tableCell as IWidget).LayoutInfo.Paddings.Left + (tableCell as IWidget).LayoutInfo.Margins.Left - cellSpacing);
            float rightPadding = (float)((tableCell as IWidget).LayoutInfo.Paddings.Right + (tableCell as IWidget).LayoutInfo.Margins.Right - cellSpacing);
            height = height - leftPadding - rightPadding;
            return height;
        }
        /// <summary>
        /// Draw Chinese Text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textBrush"></param>
        /// <param name="bounds"></param>
        private void DrawChineseText(String text,WCharacterFormat charFormat, Font font, Brush textBrush, RectangleF bounds, StringFormat format)
        {
            if (charFormat.CharacterSpacing != 0)
            {
                DrawStringBasedOnCharSpacing(font, textBrush, bounds, text, format);
                return;
            }
            char[] ch = text.ToCharArray();
            bool isUnicode = false;
            string normalText = null;
            string chineseText = null;
            Font unicodeFont = font;
            bool isChineseCharacter = false;
            if (unicodeFont.Name == "Arial" || unicodeFont.Name == "Times New Roman" || unicodeFont.Name == "Trebuchet MS")
                unicodeFont = new Font("Arial Unicode MS", unicodeFont.Size, unicodeFont.Style);
            //Get Default font to render non east characters
            font = GetDefaultFont(font, charFormat);
            float width = 0;
            //Get ascent for default font
            float normalAscent = GetAscent(font);
            //Get ascent for unicode font
            float unicodeAscent = GetAscent(unicodeFont);
            float normalDiff = 0;
            float unicodeDiff = 0;
            //Check whether the text contain chinese characters
            bool isContainChineseChar = font.Name != unicodeFont.Name && IsUnicodeText(text);
            if (isContainChineseChar)
            {
                //Diff to shift the normal text
                if (unicodeAscent > normalAscent)
                    normalDiff = unicodeAscent - normalAscent;
                //Diff to shift the unicode text
                if (normalAscent > unicodeAscent)
                    unicodeDiff = normalAscent - unicodeAscent;
                for (int i = 0; i < ch.Length; i++)
                {
                    if (!IsUnicodeText(ch[i].ToString()))
                    {
                        normalText += ch[i];
                        if (chineseText != null)
                        {
                            width = MeasureString(chineseText, unicodeFont, format, charFormat,false).Width;
                            // Apply Smallcaps
                            if (charFormat.SmallCaps)
                                chineseText = chineseText.ToUpper();
                            Graphics.DrawString(chineseText, unicodeFont, textBrush, bounds.X, bounds.Y - unicodeDiff, format);
                            bounds.X += width;
                            chineseText = null;
                        }
                    }
                    else
                    {
                        chineseText += ch[i];
                        if (normalText != null)
                        {
                            width = MeasureString(normalText, font, format, charFormat,false).Width;
                            DrawUnicodeText(normalText, charFormat, font, textBrush, new RectangleF(bounds.X, bounds.Y + normalDiff, bounds.Width, bounds.Height), format);
                            bounds.X += width;
                            normalText = null;
                        }
                    }
                }
                if (chineseText != null)
                {
                    // Apply Smallcaps
                    if (charFormat.SmallCaps)
                        chineseText = chineseText.ToUpper();
                    Graphics.DrawString(chineseText, unicodeFont, textBrush, bounds.X, bounds.Y - unicodeDiff, format);
                }
                else if (normalText != null)
                {
                    DrawUnicodeText(normalText, charFormat, font, textBrush, new RectangleF(bounds.X, bounds.Y + normalDiff, bounds.Width, bounds.Height), format);
                }
            }
            else
                DrawUnicodeText(text, charFormat, font, textBrush, bounds, format);
        }
        /// <summary>
        /// Draw Unicode Text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textBrush"></param>
        /// <param name="bounds"></param>
        private void DrawUnicodeText(String text, WCharacterFormat charFormat, Font font, Brush textBrush, RectangleF bounds, StringFormat format)
        {
            //Draw unicode bidi text
            if (charFormat.Bidi)
            {
                DrawUnicodeBidiText(text, charFormat, font, textBrush, bounds, format);
                return;
            }

            char[] ch = text.ToCharArray();
            string normalText = null;
            string unicodeText = null;
            float width = 0;
            for (int i = 0; i < ch.Length; i++)
            {
                if ((int)ch[i] < 255)
                {
                    normalText += ch[i];
                    if (unicodeText != null)
                    {
                        width = MeasureString(unicodeText, font, format, charFormat,false).Width;
                        // Apply Smallcaps
                        if (charFormat.SmallCaps)
                            unicodeText = unicodeText.ToUpper();
                        Graphics.DrawString(unicodeText, font, textBrush, bounds.X, bounds.Y, format);
                        bounds.X += width;
                        unicodeText = null;
                    }
                }
                else
                {
                    unicodeText += ch[i];
                    if (normalText != null)
                    {
                        width = MeasureString(normalText, font, format, charFormat,false).Width;
                        DrawUnicodeString(normalText, charFormat, font, textBrush, new RectangleF(bounds.X, bounds.Y, width, bounds.Height), format);
                        bounds.X += width;
                        normalText = null;
                    }
                }
            }
            if (unicodeText != null)
            {
                // Apply Smallcaps
                if (charFormat.SmallCaps)
                    unicodeText = unicodeText.ToUpper();
                    Graphics.DrawString(unicodeText, font, textBrush, bounds.X, bounds.Y, format);
            }
            else if (normalText != null)
            {
                width = MeasureString(normalText, font, format, charFormat,false).Width;
                DrawUnicodeString(normalText, charFormat, font, textBrush, new RectangleF(bounds.X, bounds.Y, width, bounds.Height), format);
            }
        }
        /// <summary>
        /// Draw Unicode bidi Text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textBrush"></param>
        /// <param name="bounds"></param>
        private void DrawUnicodeBidiText(String text, WCharacterFormat charFormat, Font font, Brush textBrush, RectangleF bounds, StringFormat format)
        {
            char[] ch = text.ToCharArray();
            string normalText = null;
            string unicodeText = null;
            float width = 0;
            for (int i = ch.Length - 1; i >= 0; i--)
            {
                if ((int)ch[i] < 255)
                {
                    normalText += ch[i];
                    if (unicodeText != null)
                    {
                        char[] arr = unicodeText.ToCharArray();
                        Array.Reverse(arr);
                        unicodeText = new string(arr);
                        width = MeasureString(unicodeText, font, format, charFormat,false).Width;
                        DrawUnicodeString(unicodeText, charFormat, font, textBrush, new RectangleF(bounds.X, bounds.Y, width, bounds.Height), format);
                        bounds.X += width;
                        unicodeText = null;
                    }
                }
                else
                {
                    unicodeText += ch[i].ToString();
                    if (normalText != null)
                    {
                        width = MeasureString(normalText, font, format, charFormat,false).Width;
                        DrawUnicodeString(normalText, charFormat, font, textBrush, new RectangleF(bounds.X, bounds.Y, width, bounds.Height), format);
                        bounds.X += width;
                        normalText = null;
                    }
                }
            }
            if (unicodeText != null)
            {
                char[] arr = unicodeText.ToCharArray();
                Array.Reverse(arr);
                unicodeText = new string(arr);
                width = MeasureString(unicodeText, font, format, charFormat,false).Width;
                DrawUnicodeString(unicodeText, charFormat, font, textBrush, new RectangleF(bounds.X, bounds.Y, width, bounds.Height), format);
            }
            else if (normalText != null)
            {
                width = MeasureString(normalText, font, format, charFormat,false).Width;
                DrawUnicodeString(normalText, charFormat, font, textBrush, new RectangleF(bounds.X, bounds.Y, width, bounds.Height), format);
            }
        }
        /// <summary>
        /// Draw unicode string
        /// </summary>
        /// <param name="text">text</param>
        internal void DrawUnicodeString(String text, WCharacterFormat charFormat, Font font, Brush textBrush, RectangleF bounds, StringFormat format)
        {
            // Apply Smallcaps
            if (charFormat.SmallCaps)
                text = text.ToUpper();
            Graphics.DrawString(text, font, textBrush, bounds, format);
        }

        /// <summary>
        /// Determines whether the owner paragraph is empty
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>
        ///     <c>true</c> if owner paragraph is empty, set to <c>true</c>.
        /// </returns>
        internal bool IsOwnerParagraphEmpty(string text)
        {
            bool isOwnerParagraphEmpty = false;
            if (text == " " && currParagraph != null && currParagraph.Text == "" && currParagraph.Items.Count == 0)
                isOwnerParagraphEmpty = true;
            return isOwnerParagraphEmpty;
        }
        internal void DrawJustifiedLine(string text, WCharacterFormat charFormat, WParagraphFormat paraFormat, RectangleF bounds, LayoutedWidget ltWidget)
        {
            if (text == null)
                return;

            char noBreakHyphen = (char)0x1E;
            if (text.Contains(noBreakHyphen.ToString()))
                text = text.Replace(noBreakHyphen.ToString(), "-");

            string[] data = text.Split(' ');

            Font font = GetFont(currTextRange, charFormat, text);
            StringFormat format = GetStringFormat(charFormat);

            Color textColor = GetTextColor(charFormat);

            Brush textBrush = GetBrush(textColor);
            Font defaultFont = GetDefaultFont(font, charFormat);
            bool isContainChineseCharacter = font.Name != defaultFont.Name && IsUnicodeText(text);
            // Draw text background
            if (!charFormat.TextBackgroundColor.IsEmpty)
            {
                SolidBrush bgBrush = GetBrush(charFormat.TextBackgroundColor);
                Graphics.FillRectangle(bgBrush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }

            if (!charFormat.HighlightColor.IsEmpty)
            {
                SolidBrush hltBrush = new SolidBrush(charFormat.HighlightColor);

                SizeF size = new SizeF();
                if (isContainChineseCharacter)
                    size = MeasureString(text, font, defaultFont, format, charFormat);
                else
                    size = MeasureString(text, font, format, charFormat,false);
                Graphics.FillRectangle(hltBrush, bounds.X, bounds.Y, size.Width, size.Height);
            }
            // Set clipping bounds
            float clipWidth = ltWidget.Bounds.Width;
            if (clipWidth == 0 || IsWidgetNeedToClipBasedOnXPosition(ltWidget, ref clipWidth, ltWidget.Bounds))
            {
                Graphics.ResetTransform();
                return;
            }
            

            // Embos writing
            if (charFormat.Emboss)
            {
                SolidBrush brush2 = new SolidBrush(Color.Gray);
                if (IsUnicode(text))
                    Graphics.DrawString(text, charFormat.Font, brush2, bounds.X + DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Y + DEF_EMBOSS_ENGRAVE_FACTOR, format);
                else
                    Graphics.DrawString(text, charFormat.Font, brush2, new RectangleF(bounds.X + DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Y + DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Width, bounds.Height), format);
            }

            // Engrave writing
            if (charFormat.Engrave)
            {
                SolidBrush brush2 = new SolidBrush(Color.Gray);
                if (IsUnicode(text))
                    Graphics.DrawString(text, charFormat.Font, brush2, bounds.X - DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Y - DEF_EMBOSS_ENGRAVE_FACTOR, format);
                else
                    Graphics.DrawString(text, charFormat.Font, brush2, new RectangleF(bounds.X - DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Y - DEF_EMBOSS_ENGRAVE_FACTOR, bounds.Width, bounds.Height), format);
            }

            // Allcaps and smallcaps
            if (charFormat.AllCaps)
            {
                text = text.ToUpper();
            }
            SizeF TextSize = new SizeF();
            if (isContainChineseCharacter)
                TextSize = MeasureString(text, font, defaultFont, format, charFormat);
            else
                TextSize = MeasureString(text, font, format, charFormat,false);
            if (ltWidget.Bounds.Width != Convert.ToSingle(TextSize.Width + ltWidget.SubWidth))
            {
                if (text != string.Empty)
                {
                    float diff = ltWidget.Bounds.Width - Convert.ToSingle(TextSize.Width + ltWidget.SubWidth);
                    ltWidget.SubWidth += diff;
                    ltWidget.WordSpace = (ltWidget.Spaces != 0) ? Convert.ToSingle(ltWidget.SubWidth / ltWidget.Spaces) : 0.0f;
                }
            }
            StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
            //remove the linelimit flag from create stringformat object.
            //The behavior of the linelimit flag is “only entire lines are laid out in the formatting rectangle”, 
            //By default layout continues until the end of the text, or until no more lines are visible as a result of clipping”.
            format.FormatFlags &= ~StringFormatFlags.LineLimit;
            sf.FormatFlags |= StringFormatFlags.NoClip;
            string TextWithoutSpaces = text.Replace(" ", string.Empty);
            float total = 0;
            if (isContainChineseCharacter)
                total = MeasureString(text, font, defaultFont, format, charFormat).Width;
            else
                total = MeasureString(text, font, format, charFormat,false).Width;
            float textWidth = 0;
            if (isContainChineseCharacter)
                textWidth = MeasureString(TextWithoutSpaces, font, defaultFont, format, charFormat).Width;
            else
                textWidth = MeasureString(TextWithoutSpaces, font, format, charFormat,false).Width;
            float spacewidth = 0.0f;
            if (ltWidget.Spaces > 0)
                spacewidth = (total - textWidth) / ltWidget.Spaces;
            float whole = textWidth + (ltWidget.WordSpace * ltWidget.Spaces) + (spacewidth * ltWidget.Spaces);
            float width = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (font.Name != defaultFont.Name && IsUnicodeText(data[i]))
                    width = MeasureString(data[i], font, defaultFont, format, charFormat).Width;
                else
                    width = MeasureString(data[i], font, format, charFormat,false).Width;
                if (i == data.Length - 1)
                {
                    if (bounds.X + width > ltWidget.Bounds.Right)
                    {
                        float diff = (bounds.X + width) - ltWidget.Bounds.Right;
                        bounds.X -= diff;
                    }
                    else if (bounds.X + width < ltWidget.Bounds.Right)
                    {
                        float diff = ltWidget.Bounds.Right - (bounds.X + width);
                        bounds.X += diff;
                    }
                }
                //Reset Graphics Transform Position
                Graphics.ResetTransform();
                //Transform Graphics Position
                TransformGraphicsPosition(ltWidget);

                if (charFormat.SmallCaps || charFormat.AllCaps)
                {
                    data[i] = data[i].ToUpper();
                }
                if (IsUnicode(data[i]))
                    DrawChineseText(data[i], charFormat, font, textBrush, new RectangleF(bounds.X, bounds.Y, width, bounds.Height), sf);
                else
                {
                    float subWidth = ltWidget.SubWidth;
                    //update the subwidth to zero when subwidth have negative value. 
                    if (subWidth < 0)
                        subWidth = 0;
                    //Calculate the width of the clip area. 
                    float setClipWidth = (clipWidth - (bounds.X - ltWidget.Bounds.X)) + subWidth;
                    if (setClipWidth < 0)
                        setClipWidth = 0;
                    //Set the clip to for the each and every word 
                    Graphics.SetClip(new RectangleF(bounds.X, bounds.Y, setClipWidth, bounds.Height), CombineMode.Replace);
                    Graphics.DrawString(data[i], defaultFont, textBrush, new RectangleF(bounds.X, bounds.Y, width, bounds.Height), sf);
                }

                //Calculating the total width that has been shifted between words
                float shiftWidth = spacewidth + ltWidget.WordSpace;
                if (font.Underline && i != data.Length - 1)
                {
                    string emptySpace = "";
                    //Appending empty spaces for the shiftwidth
                    for (int j = 0; shiftWidth > 0; j++)
                    {
                        emptySpace += " ";
                        shiftWidth -= MeasureString(" ", defaultFont, format).Width;
                    }
                    float emptySpaceWidth = MeasureString(emptySpace, defaultFont, format).Width;
                    //Drawing the empty spaces with underline font.
                    Graphics.DrawString(emptySpace, font, textBrush, new RectangleF(bounds.X + width, bounds.Y, emptySpaceWidth, bounds.Height), sf);
                }

                if (!ltWidget.IsLastLine)
                {
                    bounds.X = (bounds.X + width);
                    RectangleF rect = new RectangleF(bounds.X, bounds.Y, spacewidth + ltWidget.WordSpace, bounds.Height);
                    Graphics.DrawString(" ", font, textBrush, rect);
                    bounds.X += spacewidth + ltWidget.WordSpace;
                }
                else
                {
                    bounds.X = (bounds.X + width) + spacewidth;
                }
                //Reset Graphics Transform Position
                Graphics.ResetTransform();
                // Reset clipping bounds
                Graphics.ResetClip();
            }
      }

        #endregion

        #region Borders
        /// <summary>
        /// Draws the paragraph borders.
        /// </summary>
        /// <param name="borders">The paragraph format.</param>
        /// <param name="ltWidget">The lt widget.</param>
        private void DrawParagraphBorders(WParagraph paragraph, WParagraphFormat paraFormat, LayoutedWidget ltWidget)
        {
            RectangleF bounds = ltWidget.Bounds;
            if (ltWidget.ChildWidgets.Count > 0 && Math.Round(ltWidget.Bounds.Y, 2) != Math.Round(ltWidget.ChildWidgets[0].Bounds.Y, 2))
            {
                bounds.Y = ltWidget.ChildWidgets[0].Bounds.Y;
                bounds.Height = ltWidget.Bounds.Bottom - ltWidget.ChildWidgets[0].Bounds.Y;
            }
            WSection sec = null;
            bool isNextParagraphBottomBorderSet = false;
            bool isPreviousParagraphTopBorderSet = false;
            ILayoutInfo layoutInfo = ltWidget.Widget.LayoutInfo;
            float bottom = bounds.Bottom + (float)layoutInfo.Margins.Bottom;
            float firstLineIndent = (layoutInfo as ParagraphLayoutInfo).FirstLineIndent;//Get the first line indent value.
            if (firstLineIndent > 0)
                firstLineIndent = 0;
            float leftPad = (float)layoutInfo.Paddings.Left + firstLineIndent;
            float listTabWidth = 0;
            //Update list tab width in left padding
            if (!IsParagraphContainingListHasBreak(ltWidget)
                && !(ltWidget.ChildWidgets.Count == 0
                || ltWidget.Bounds.Width == 0
                || (layoutInfo as ParagraphLayoutInfo).ListValue == string.Empty))
                listTabWidth += Math.Abs((layoutInfo as ParagraphLayoutInfo).ListTab);
            float rightPad = (float)layoutInfo.Paddings.Right;
            #region nextSiblingCheck
            if (ltWidget.IsLastItemInPage && ltWidget.TextTag == "Splitted")
                isNextParagraphBottomBorderSet = true;
            if (paragraph.NextSibling != null && paragraph.NextSibling is WParagraph && !ltWidget.IsLastItemInPage)
            {
                WParagraphFormat paragraphFormat = (paragraph.NextSibling as WParagraph).ParagraphFormat;
                Borders border = paragraphFormat.Borders;
                if (!border.NoBorder)
                {
                    if (border.Bottom.BorderType != BorderStyle.None)
                        isNextParagraphBottomBorderSet = true;
                }
                if ((paragraphFormat.IsFrame && !paraFormat.IsNextParagraphInSameFrame())
                    || (!IsAdjacentParagraphHaveSameBorders(paragraph, paragraph.NextSibling as WParagraph)))
                {
                    isNextParagraphBottomBorderSet = false;
                }
            }
            #endregion

            #region previousSiblingCheck
            if (paragraph.PreviousSibling != null && paragraph.PreviousSibling is WParagraph && !layoutInfo.IsFirstItemInPage)
            {
                WParagraphFormat paragraphFormat = (paragraph.PreviousSibling as WParagraph).ParagraphFormat;
                Borders border = paragraphFormat.Borders;
                if (!border.NoBorder)
                {
                    if (border.Top.BorderType != BorderStyle.None)
                        isPreviousParagraphTopBorderSet = true;

                }
                if ((paragraphFormat.IsFrame && !paraFormat.IsPreviousParagraphInSameFrame())
                    || (!IsAdjacentParagraphHaveSameBorders(paragraph, paragraph.PreviousSibling as WParagraph)))
                {
                    isPreviousParagraphTopBorderSet = false;
                }
            }
            #endregion

            Entity ent = paragraph.Owner as Entity;
            while (ent != null)
            {
                if (ent is WSection)
                {
                    sec = ent as WSection;
                    break;
                }
                ent = ent.Owner;
            }
            bounds.Width = bounds.Width - paraFormat.LeftIndent - paraFormat.RightIndent;
            float right = bounds.Right;
            if (sec != null)
            {
                float clientWidth = sec.PageSetup.ClientWidth;
                right = sec.PageSetup.PageSize.Width - sec.PageSetup.Margins.Right;
            }
            if (bounds.X + bounds.Width >= right)
            {

                bounds.Width = right - bounds.X;

            }

            bounds.X += leftPad;
            bounds.X -= listTabWidth;
            bounds.Width = bounds.Width + Math.Abs(leftPad + rightPad);
            if (paragraph.IsInCell)
            {
                TableLayoutInfo tableInfo = (paragraph.OwnerTextBody as WTableCell).m_layoutInfo as TableLayoutInfo;
                if (tableInfo.TableCellLeftMargin - tableInfo.Paddings.Left > bounds.X)
                    bounds.X = (float)(tableInfo.TableCellLeftMargin - tableInfo.Paddings.Left);
                if (tableInfo.CellWidth - tableInfo.Paddings.Left - tableInfo.Paddings.Right < bounds.Width)
                    bounds.Width = (float)(tableInfo.CellWidth - tableInfo.Margins.Left - tableInfo.Margins.Right)
                        - (bounds.X - (float)(tableInfo.TableCellLeftMargin - tableInfo.Paddings.Left));
            }

            Borders borders = paraFormat.Borders;

            // left border
            if (borders.Left.BorderType != BorderStyle.None)
            {
                if (isNextParagraphBottomBorderSet)
                    DrawBorder(borders.Left,
                                new PointF(bounds.Left, bounds.Top + borders.Left.LineWidth / 2),
                                new PointF(bounds.Left, bottom - borders.Left.LineWidth / 2));
                else
                    DrawBorder(borders.Left,
                                new PointF(bounds.Left, bounds.Top + borders.Left.LineWidth / 2),
                                new PointF(bounds.Left, bounds.Bottom - borders.Left.LineWidth / 2));
            }

            // top border
            if (borders.Top.BorderType != BorderStyle.None && !isPreviousParagraphTopBorderSet)
            {
                DrawBorder(borders.Top,
                            new PointF(bounds.Left, bounds.Top),
                            new PointF(bounds.Right, bounds.Top));
            }

            // right border
            if (borders.Right.BorderType != BorderStyle.None)
            {
                if (isNextParagraphBottomBorderSet)
                    DrawBorder(borders.Right,
                                new PointF(bounds.Right, bounds.Top + borders.Right.LineWidth / 2),
                                new PointF(bounds.Right, bottom - borders.Right.LineWidth / 2));
                else
                    DrawBorder(borders.Right,
                                new PointF(bounds.Right, bounds.Top + borders.Right.LineWidth / 2),
                                new PointF(bounds.Right, bounds.Bottom - borders.Right.LineWidth / 2));
            }

            // bottom border
            if (borders.Bottom.BorderType != BorderStyle.None && !isNextParagraphBottomBorderSet)
            {
                DrawBorder(borders.Bottom,
                            new PointF(bounds.Left, bounds.Bottom),
                            new PointF(bounds.Right, bounds.Bottom));
            }


        }
        /// <summary>
        /// Determine whether the current paragraph is having the same border values of Adjacent paragraphs
        /// </summary>
        /// <param name="currParagraph"></param>
        /// <param name="adjacentParagraph"></param>
        /// <returns></returns>
        private bool IsAdjacentParagraphHaveSameBorders(WParagraph currParagraph, WParagraph adjacentParagraph)
        {
            if (IsSameAdjacentBorder(currParagraph.ParagraphFormat.Borders.Bottom, adjacentParagraph.ParagraphFormat.Borders.Bottom)
                && IsSameAdjacentBorder(currParagraph.ParagraphFormat.Borders.Top, adjacentParagraph.ParagraphFormat.Borders.Top)
                && IsSameAdjacentBorder(currParagraph.ParagraphFormat.Borders.Right, adjacentParagraph.ParagraphFormat.Borders.Right)
                && IsSameAdjacentBorder(currParagraph.ParagraphFormat.Borders.Left, adjacentParagraph.ParagraphFormat.Borders.Left))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Determine whether the border properties are equivalent to Adjacent border
        /// </summary>
        /// <param name="border"></param>
        /// <param name="adjacentBorder"></param>
        /// <returns></returns>
        private bool IsSameAdjacentBorder(Border border, Border adjacentBorder)
        {
            if (border.BorderType == adjacentBorder.BorderType
                && border.LineWidth == adjacentBorder.LineWidth
                && border.Space == adjacentBorder.Space
                && border.Color == adjacentBorder.Color)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="borders">The borders.</param>
        /// <param name="ltWidget">The lt widget.</param>
        private void DrawBorders(Borders borders, LayoutedWidget ltWidget)
        {
            RectangleF bounds = ltWidget.Bounds;

            // left border
            DrawBorder(borders.Left,
                        new PointF(bounds.Left, bounds.Top),
                        new PointF(bounds.Left, bounds.Bottom));

            // top border
            DrawBorder(borders.Top,
                        new PointF(bounds.Left, bounds.Top),
                        new PointF(bounds.Right, bounds.Top));

            // right border
            DrawBorder(borders.Right,
                        new PointF(bounds.Right, bounds.Top),
                        new PointF(bounds.Right, bounds.Bottom));

            // bottom border
            DrawBorder(borders.Bottom,
                        new PointF(bounds.Left, bounds.Bottom),
                        new PointF(bounds.Right, bounds.Bottom));
        }

        /// <summary>
        /// Draws the border.
        /// </summary>
        /// <param name="border">The border.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        private void DrawBorder(Border border, PointF start, PointF end)
        {
            if (border.BorderType == BorderStyle.Cleared)
                return;
            if ((border.BorderType == BorderStyle.None
                && !border.HasNoneStyle)
                || border.BorderType != BorderStyle.None)
            {
                Pen pen = GetPen(border);
                Graphics.DrawLine(pen, start, end);
            }
        }

        /// <summary>
        /// Draws the bounds.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <param name="rect">The Rectangle.</param>
        internal void DrawBounds(Color color, RectangleF rect)
        {
            using (Pen pen = new Pen(color))
            {
                Graphics.DrawRectangle(pen, System.Drawing.Rectangle.Ceiling(rect));
            }
        }
        #endregion

        #region Tables
        /// <summary>
        /// Draws the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="ltWidget">The lt widget.</param>
        internal virtual void DrawTable(WTable table, LayoutedWidget ltWidget)
        {
            if (table.TableFormat.CellSpacing > 0)
            {
                if (table.TableFormat.TextureStyle != TextureStyle.TextureNone)
                {
                    DrawTextureStyle(table.TableFormat.TextureStyle, table.TableFormat.ForeColor, table.TableFormat.BackColor, ltWidget.Bounds);
                }
                else
                {
                    if (!table.TableFormat.BackColor.IsEmpty)
                    {
                        //Draw Table background color.
                        Graphics.FillRectangle(new SolidBrush(table.TableFormat.BackColor), ltWidget.Bounds);
                    }
                }
                DrawBorders(table.TableFormat.Borders, ltWidget);
            }
            if (ltWidget.ChildWidgets.Count > 0)
            {
                // Updates first layouted row of current page.
                ltWidget.ChildWidgets[0].Widget.LayoutInfo.IsFirstItemInPage = true;
                // Updates last layouted row of current page.
                ltWidget.ChildWidgets[ltWidget.ChildWidgets.Count - 1].IsLastItemInPage = true;
                for (int i = 0; i < ltWidget.ChildWidgets[ltWidget.ChildWidgets.Count - 1].ChildWidgets.Count; i++)
                {
                    ltWidget.ChildWidgets[ltWidget.ChildWidgets.Count - 1].ChildWidgets[i].IsLastItemInPage = true;
                }
            }
        }

        /// <summary>
        /// Draws the table row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="ltWidget">The lt widget.</param>
        internal virtual void DrawTableRow(WTableRow row, LayoutedWidget ltWidget)
        {
            //DrawBounds( Color.Black, ltWidget.Bounds );
        }

        /// <summary>
        /// Draws the table cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="ltWidget">The lt widget.</param>
        internal virtual void DrawTableCell(WTableCell cell, LayoutedWidget ltWidget)
        {
            if (cell == null)
                throw new ArgumentNullException("cell");
            if (ltWidget == null)
                throw new ArgumentNullException("ltWidget");

            RectangleF bounds = ltWidget.Bounds;
            // Updates the bounds for nested table cells.
            if (ltWidget.TextTag == "Clipped" && bounds.Width > 0)
                bounds.Width -= (bounds.Right - ltWidget.RightPosition);
            float cellSpacing = (cell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0) ?
                cell.OwnerRow.OwnerTable.TableFormat.CellSpacing * 2 : 0;
            float topMargin = (float)(ltWidget.Widget.LayoutInfo.Margins.Top - cellSpacing);
            float bottomMargin = (float)(ltWidget.Widget.LayoutInfo.Margins.Bottom - cellSpacing);
            float leftMargin = (float)(ltWidget.Widget.LayoutInfo.Margins.Left - cellSpacing);
            float rightMargin = (float)(ltWidget.Widget.LayoutInfo.Margins.Right - cellSpacing);
            bounds = new RectangleF(bounds.X + leftMargin, bounds.Y + topMargin, bounds.Width - leftMargin - rightMargin, bounds.Height - topMargin - bottomMargin);
            if (!(ltWidget.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeContinue && ltWidget.Bounds != new RectangleF())
            {
                if (cell.TextureStyle != TextureStyle.TextureNone)
                {
                    DrawTextureStyle(cell.CellFormat.TextureStyle, cell.CellFormat.ForeColor, cell.CellFormat.BackColor, bounds);
                }
                else
                {
                    if (!cell.CellFormat.BackColor.IsEmpty)
                    {
                        //Draw Table Cell background.
                        Graphics.FillRectangle(new SolidBrush(cell.CellFormat.BackColor), bounds);
                        float width = Math.Max(cell.CellFormat.Borders.Top.LineWidth / 2, cell.CellFormat.Borders.Bottom.LineWidth / 2);
                        int cellIndex = cell.GetCellIndex();
                        if (cellIndex == 0 && cell.CellFormat.Borders.Left.BorderType == BorderStyle.Cleared)
                            Graphics.FillRectangle(new SolidBrush(cell.CellFormat.BackColor), new RectangleF(bounds.X - width, bounds.Y, width, bounds.Height));
                        if (cellIndex == cell.OwnerRow.Cells.Count - 1 && cell.CellFormat.Borders.Right.BorderType == BorderStyle.Cleared)
                            Graphics.FillRectangle(new SolidBrush(cell.CellFormat.BackColor), new RectangleF(bounds.Right, bounds.Y, width, bounds.Height));
                    }
                }
            }
            if (ltWidget.Bounds != new RectangleF())
                DrawCellBorders(cell, ltWidget);
            if (IsTexBoxHaveBackgroundPicture(cell))
            {
                Graphics.DrawImage((cell.Owner.Owner as WTable).m_textBoxFormat.FillEfects.Picture, ltWidget.Bounds);
            }
        }
        /// <summary>
        /// Checks the TextBox for Background picture.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        internal bool IsTexBoxHaveBackgroundPicture(WTableCell cell)
        {
            if (cell.Owner != null && cell.Owner.Owner != null && cell.Owner.Owner is WTable && (cell.Owner.Owner as WTable).m_isTextBox)
                if ((cell.Owner.Owner as WTable).m_textBoxFormat != null && (cell.Owner.Owner as WTable).m_textBoxFormat.FillEfects.Picture != null)
                    return true;
            return false;
        }
        /// <summary>
        /// Draws the texture style.
        /// </summary>
        /// <param name="textureStyle"></param>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        /// <param name="bounds"></param>
        internal void DrawTextureStyle(TextureStyle textureStyle, Color foreColor, Color backColor, RectangleF bounds)
        {
            if (backColor.IsEmpty)
                backColor = Color.White;
            if (textureStyle.ToString().Contains("Percent"))
            {
                string text = textureStyle.ToString().Replace("Texture", "").Replace("Percent", "").Replace("Pt", ".");
                float percent = float.Parse(text, CultureInfo.InvariantCulture);
                Graphics.FillRectangle(new SolidBrush(GetForeColor(foreColor, backColor, percent)), bounds);
            }
            FillTexture(textureStyle, foreColor, backColor, bounds);
        }
        /// <summary>
        /// Gets the fore color.
        /// </summary>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        private Color GetForeColor(Color foreColor, Color backColor, float percent)
        {
            int r = 0, g = 0, b = 0;
            r = GetColorValue(foreColor.R, backColor.R, percent, foreColor.IsEmpty, backColor.IsEmpty);
            g = GetColorValue(foreColor.G, backColor.G, percent, foreColor.IsEmpty, backColor.IsEmpty);
            b = GetColorValue(foreColor.B, backColor.B, percent, foreColor.IsEmpty, backColor.IsEmpty);
            foreColor = Color.FromArgb(r, g, b);
            return foreColor;
        }
        /// <summary>
        /// Gets the color value.
        /// </summary>
        /// <param name="foreColorValue">The fore color value.</param>
        /// <param name="backColorValue">The back color value.</param>
        /// <param name="percent">The percent.</param>
        /// <returns></returns>
        private int GetColorValue(int foreColorValue, int backColorValue, float percent, bool isForeColorEmpty, bool isBackColorEmpty)
        {
            int colorValue = 0;

            if (percent == 100)
            {
                colorValue = foreColorValue;
            }
            else
            {
                if (isForeColorEmpty)
                {
                    if (isBackColorEmpty)
                        colorValue = (int)Math.Round(255 * (1 - percent / 100));
                    else
                        colorValue = (int)Math.Round(backColorValue * (1 - percent / 100));
                }
                else
                {
                    if (isBackColorEmpty)
                        colorValue = (int)Math.Round(foreColorValue * (percent / 100));
                    else
                        colorValue = backColorValue + (int)Math.Round(foreColorValue * (percent / 100)) - (int)Math.Round(backColorValue * (percent / 100));
                }
            }

            return colorValue;
        }
        /// <summary>
        /// Fill Texture within the bounds
        /// </summary>
        /// <param name="textureStyle">Texture Style</param>
        /// <param name="foreColor">Fore Color</param>
        /// <param name="backColor">Back Color</param>
        /// <param name="bounds">Bounds</param>
        private void FillTexture(TextureStyle textureStyle, Color foreColor, Color backColor, RectangleF bounds)
        {
            if (foreColor.IsEmpty)
                if (m_backgroundColorCheckList.Contains(backColor))
                    foreColor = Color.White;
                else
                    foreColor = Color.Black;
            switch (textureStyle)
            {
                case TextureStyle.TextureSolid:
                    Graphics.FillRectangle(new SolidBrush(foreColor), bounds);
                    break;
                case TextureStyle.TextureCross:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.Cross, foreColor, backColor), bounds);
                    }
                    break;
                case TextureStyle.TextureDarkDiagonalCross:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.DiagonalCross, foreColor, backColor), bounds);
                    }
                    break;
                case TextureStyle.TextureDarkDiagonalDown:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.DarkDownwardDiagonal, foreColor, backColor), bounds);
                    }
                    break;
                case TextureStyle.TextureDarkDiagonalUp:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.DarkUpwardDiagonal, foreColor, backColor), bounds);
                    }
                    break;
                case TextureStyle.TextureDarkHorizontal:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.Horizontal, foreColor, backColor), bounds);
                    }
                    break;
                case TextureStyle.TextureDarkVertical:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.DarkVertical, foreColor, backColor), bounds);
                    }
                    break;
                case TextureStyle.TextureDiagonalCross:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.DiagonalCross, foreColor, backColor), bounds);
                    }
                    break;
                case TextureStyle.TextureDiagonalDown:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.LightDownwardDiagonal, foreColor, backColor), bounds);
                    }
                    break;
                case TextureStyle.TextureDiagonalUp:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.LightUpwardDiagonal, foreColor, backColor), bounds);
                    }
                    break;
                case TextureStyle.TextureHorizontal:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.LightHorizontal, foreColor, backColor), bounds);
                    }
                    break;
                case TextureStyle.TextureVertical:
                    {
                        Graphics.FillRectangle(new HatchBrush(HatchStyle.LightVertical, foreColor, backColor), bounds);
                    }
                    break;
            }
        }
        /// <summary>
        /// Draws the cell borders.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="ltWidget">The lt widget.</param>
        internal virtual void DrawCellBorders(WTableCell cell, LayoutedWidget ltWidget)
        {
            RectangleF bounds = ltWidget.Bounds;
            Borders cellBorders = cell.CellFormat.Borders;
            RowFormat tableFormat = cell.OwnerRow.OwnerTable.TableFormat;
            Borders tableBorders = tableFormat.Borders;
            DefaultBorders defaultBorders = new DefaultBorders(tableFormat);
            Borders rowBorders = cell.OwnerRow.RowFormat.Borders;
            Border verticalMergedCellBottomBorder = null;
            bool isVerticalMergedCell = false;
            Border horizontalMergedCellRightBorder = null;
            bool isHorizontalMergedCell = false;
            //if (defaultBorders.Horizontal.BorderType == BorderStyle.None
            //    && defaultBorders.Vertical.BorderType == BorderStyle.None)
            //{
            //    return;
            //}

            int cellIndex = cell.GetCellIndex();
            int rowIndex = cell.OwnerRow.GetRowIndex();
            int cellLast = cell.OwnerRow.Cells.Count - 1;
            int rowlLast = cell.OwnerRow.OwnerTable.Rows.Count - 1;
            int nextCellIndex = cellIndex;
            int nextRowIndex = rowIndex;

            for (int i = cellIndex + 1; i < cellLast + 1; i++)
            {
                if (cell.OwnerRow.Cells[i].CellFormat.HorizontalMerge != CellMerge.Continue)
                {
                    nextCellIndex = i;
                    break;
                }
                else
                {
                    horizontalMergedCellRightBorder = cell.OwnerRow.Cells[i].CellFormat.Borders.Right;
                    isHorizontalMergedCell = true;
                }
            }
            //Handle for vertical merged cell
            for (int i = rowIndex + 1; i < rowlLast + 1; i++)
            {
                int index = GetAdjacentCellIndex(cell, cellIndex, i);
                if (index < cell.OwnerRow.OwnerTable.Rows[i].Cells.Count)
                {
                    if (cell.OwnerRow.OwnerTable.Rows[i].Cells[index].CellFormat.VerticalMerge != CellMerge.Continue)
                    {
                        nextRowIndex = i;
                        break;
                    }
                    else
                    {
                        if (cell.OwnerRow.OwnerTable.Rows[i].Cells[index].CellFormat.Borders.Bottom.BorderType == BorderStyle.None && i == rowlLast)
                            verticalMergedCellBottomBorder = tableBorders.Bottom;
                        else
                            verticalMergedCellBottomBorder = cell.OwnerRow.OwnerTable.Rows[i].Cells[index].CellFormat.Borders.Bottom;
                        isVerticalMergedCell = true;
                    }
                }
                else
                {
                    nextRowIndex = i;
                    break;
                }
            }
            Border NextLeftBorder = cell.OwnerRow.Cells[nextCellIndex].CellFormat.Borders.Left;
            bool bNoSpacing = tableFormat.CellSpacing < 0;
            TableLayoutInfo tableInfo = ltWidget.Widget.LayoutInfo as TableLayoutInfo;
            ILayoutInfo rowInfo = (cell.OwnerRow as IWidget).LayoutInfo;
            WTableCell.LayoutCellInfo cellInfo = ltWidget.Widget.LayoutInfo as WTableCell.LayoutCellInfo;
            bool bSkipLeftBorder = cellInfo.SkipLeftBorder;
            bool bSkipRightBorder = cellInfo.SkipRightBorder;
            bool bSkipTopBorder = cellInfo.SkipTopBorder;
            bool bSkipBottomBorder = cellInfo.SkipBottomBorder;
            bSkipRightBorder &= (NextLeftBorder.BorderType != BorderStyle.Cleared);
            if (nextCellIndex == cellIndex)
                bSkipRightBorder = false;
            if (bSkipBottomBorder && isVerticalMergedCell)
            {
                if (nextRowIndex == rowIndex)
                    bSkipBottomBorder = false;
                else
                {
                    int index = GetAdjacentCellIndex(cell, cellIndex, nextRowIndex);
                    if (cell.OwnerRow.OwnerTable.Rows[nextRowIndex].Cells[index].CellFormat.Borders.Top.BorderType == BorderStyle.Cleared
                        && verticalMergedCellBottomBorder.BorderType != BorderStyle.None)
                        bSkipBottomBorder = false;
                }
            }
            if (bSkipBottomBorder && isVerticalMergedCell && (cellBorders.Bottom.BorderType == BorderStyle.None || cellBorders.Bottom.BorderType == BorderStyle.Cleared))
            {
                bSkipBottomBorder &= (verticalMergedCellBottomBorder.BorderType == BorderStyle.None || verticalMergedCellBottomBorder.BorderType == BorderStyle.Cleared);
            }
            if (ltWidget.TextTag == "Clipped" && bounds.Width > 0)
            {
                bounds.Width = bounds.Width - (bounds.Right - ltWidget.RightPosition);
                bSkipRightBorder = true;
            }
            #region Draw left border
            if (!bSkipLeftBorder)
            {
                Border leftBorder = ((cellBorders.Left.BorderType != BorderStyle.None) ?
                    cellBorders.Left
                    : ((cellIndex == 0) ?
                    (rowBorders.Left.IsBorderDefined ?
                    rowBorders.Left : tableBorders.Left)
                    : (rowBorders.Vertical.IsBorderDefined ?
                    rowBorders.Vertical : defaultBorders.Vertical)));
                if (leftBorder.BorderType != BorderStyle.None)
                {
                    // get the privious cell top and bottom border for adjust the edges.
                    WTableCell prevCell = null;
                    Border prevCellTopBorder = null;
                    Border prevCellBottomBorder = null;
                    if (cellIndex > 0)
                        prevCell = cellInfo.GetAdjacentCell(cellIndex - 1);
                    if (prevCell != null)
                    {
                        prevCellTopBorder = GetCellTopBorder(prevCell, tableBorders, defaultBorders);
                        prevCellBottomBorder = GetCellBottomBorder(prevCell, tableBorders, defaultBorders);
                    }

                    if (leftBorder.BorderType == BorderStyle.ThinThickThinSmallGap && bounds.X != 0 && bounds.Y != 0)
                    {
                        DrawThinThickThinLeftBorder(leftBorder, cellInfo.CellBorders,
                                new PointF(bounds.Left, bounds.Top),
                                new PointF(bounds.Left, bounds.Bottom), prevCellTopBorder,prevCellBottomBorder);
                    }
                    else
                    {
                        PointF start = new PointF(bounds.Left, bounds.Top);
                        PointF end = new PointF(bounds.Left, bounds.Bottom);
                        if((prevCellTopBorder != null && prevCellTopBorder.BorderType == BorderStyle.ThinThickThinSmallGap) ||cellInfo.CellBorders.Top.BorderType==BorderStyle.ThinThickThinSmallGap)
                        {
                            start = new PointF(start.X, start.Y + 4.875f);
                        }
                        DrawBorder(leftBorder,
                                start,
                                end);
                    }
                }
            }
            #endregion

            #region Draw top border
            if (!bSkipTopBorder && (((ltWidget.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart)
                || (!isVerticalMergedCell && !(ltWidget.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeContinue)
                || rowInfo.IsFirstItemInPage))
            {
                Border topBorder = ((cellBorders.Top.BorderType != BorderStyle.None) ?
                    cellBorders.Top
                    : (rowInfo.IsFirstItemInPage ?
                    (rowBorders.Top.IsBorderDefined ?
                    rowBorders.Top : tableBorders.Top)
                    : (rowBorders.Horizontal.IsBorderDefined ?
                    rowBorders.Horizontal : defaultBorders.Horizontal)));
                if (topBorder.BorderType == BorderStyle.Cleared)
                    topBorder = cellInfo.CellBorders.Top;
                if (topBorder.BorderType != BorderStyle.None)
                {
                    //get the top cell left border and right border for edge adjustment.
                    WTableCell topCell = null;
                    Border topCellLeftBorder = null;
                    Border topCellRightBorder = null;
                    if (rowIndex > 0)
                        topCell = cellInfo.GetAdjacentRowCell(rowIndex - 1);
                    if (topCell != null)
                    {
                        topCellLeftBorder = GetCellLeftBorder(topCell, tableBorders, defaultBorders);
                        topCellRightBorder = GetCellRightBorder(topCell, tableBorders, defaultBorders);
                    }
                    if (topBorder.BorderType == BorderStyle.ThinThickThinSmallGap && bounds.X != 0 && bounds.Y != 0)
                    {
                        DrawThinThickThinTopBorder(topBorder, cellInfo.CellBorders,cellIndex,cellLast,rowIndex,
                                new PointF(bounds.Left, bounds.Top),
                                new PointF(bounds.Right, bounds.Top),topCellLeftBorder,topCellRightBorder);
                    }
                    else
                    {
                        PointF start = new PointF(bounds.Left, bounds.Top);
                        PointF end = new PointF(bounds.Right, bounds.Top);
                        if (topCellLeftBorder != null && topCellLeftBorder.BorderType == BorderStyle.ThinThickThinSmallGap) 
                        {
                            start = new PointF(start.X + 4.875f, start.Y);
                        }
                        DrawBorder(topBorder,
                                    start,
                                    end);
                    }
                }
            }
            #endregion

            #region Draw right border
            if (!bSkipRightBorder)
            {
                // right border
                Border rightBorder = (isHorizontalMergedCell ? horizontalMergedCellRightBorder : cellBorders.Right);
                rightBorder = ((rightBorder.BorderType != BorderStyle.None) ?
                    rightBorder
                    : ((cellIndex == cellLast || nextCellIndex == cellIndex) ?
                    (rowBorders.Right.IsBorderDefined ?
                    rowBorders.Right : tableBorders.Right)
                    : (rowBorders.Vertical.IsBorderDefined ?
                    rowBorders.Vertical : defaultBorders.Vertical)));
                if (rightBorder.BorderType != BorderStyle.None)
                {
                    if (rightBorder.BorderType == BorderStyle.ThinThickThinSmallGap && bounds.X != 0 && bounds.Y != 0)
                    {
                        //Get the next cell top border and bottom border for edge adjustment.
                        WTableCell nextCell=null;
                        Border nextCellTopBorder = null;
                        Border nextCellBottomBorder = null;
                        if (cellIndex < cellLast)
                        {
                            nextCell = cellInfo.GetAdjacentCell(nextCellIndex);
                        }
                        if (nextCell != null)
                        {
                            nextCellTopBorder = GetCellTopBorder(nextCell, tableBorders, defaultBorders);
                            nextCellBottomBorder = GetCellBottomBorder(nextCell, tableBorders, defaultBorders);
                        }

                        DrawThinThickThinRightBorder(rightBorder, cellInfo.CellBorders,
                               new PointF(bounds.Right, bounds.Top),
                               new PointF(bounds.Right, bounds.Bottom), nextCellTopBorder,nextCellBottomBorder);
                    }
                    else
                        DrawBorder(rightBorder,
                                    new PointF(bounds.Right, bounds.Top),
                                    new PointF(bounds.Right, bounds.Bottom));
                }
            }
            #endregion

            #region Draw bottom border
            if (!bSkipBottomBorder
                && (!isVerticalMergedCell && !(ltWidget.Widget.LayoutInfo as TableLayoutInfo).IsRowMergeStart
                || ltWidget.IsLastItemInPage))
            {
                // bottom border
                Border bottomBorder = ((cellBorders.Bottom.BorderType != BorderStyle.None) ?
                   cellBorders.Bottom
                    : (ltWidget.IsLastItemInPage ?
                    (rowBorders.Bottom.IsBorderDefined ?
                    rowBorders.Bottom : tableBorders.Bottom)
                    : (rowBorders.Horizontal.IsBorderDefined ?
                    rowBorders.Horizontal : defaultBorders.Horizontal)));

                if (bottomBorder.BorderType != BorderStyle.None)
                {
                    if (bottomBorder.BorderType == BorderStyle.ThinThickThinSmallGap && bounds.X != 0 && bounds.Y != 0)
                    {
                        // Get the below cell's left and right border for edge adjustment.
                        WTableCell belowCell = null;
                        Border belowCellLeftBorder = null;
                        Border belowCellRightBorder = null;
                        if (rowIndex < rowlLast)
                        {
                            belowCell = cellInfo.GetAdjacentRowCell(nextRowIndex);
                        }
                        if (belowCell != null)
                        {
                            belowCellLeftBorder = GetCellLeftBorder(belowCell, tableBorders, defaultBorders);
                            belowCellRightBorder = GetCellRightBorder(belowCell, tableBorders, defaultBorders);
                        }
                        
                        DrawThinThickThinBottomBorder(bottomBorder, cellInfo.CellBorders, 
                                new PointF(bounds.Left, bounds.Bottom),
                                new PointF(bounds.Right, bounds.Bottom), belowCellLeftBorder,belowCellRightBorder);
                    }
                    else
                        DrawBorder(bottomBorder,
                                    new PointF(bounds.Left, bounds.Bottom),
                                    new PointF(bounds.Right, bounds.Bottom));
                }
            }
            #endregion

            #region Draw diagonal down border
            // diagonal down border
            Border diagonalDownBorder = (cellBorders.DiagonalDown.BorderType != BorderStyle.None)
                ? cellBorders.DiagonalDown
                : tableBorders.DiagonalDown;
            if (diagonalDownBorder.BorderType != BorderStyle.None)
            {
                DrawBorder(diagonalDownBorder,
                                new PointF(bounds.Left, bounds.Top),
                                new PointF(bounds.Right, bounds.Bottom));
            }
            #endregion

            #region Draw diagonal up border
            // diagonal up border
            Border diagonalUpBorder = (cellBorders.DiagonalUp.BorderType != BorderStyle.None)
                ? cellBorders.DiagonalUp
                : tableBorders.DiagonalUp;
            if (diagonalUpBorder.BorderType != BorderStyle.None)
            {
                DrawBorder(diagonalUpBorder,
                                new PointF(bounds.Left, bounds.Bottom),
                                new PointF(bounds.Right, bounds.Top));
            }
            #endregion
        }

        /// <summary>
        /// Gets the cell left border.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="tableBorders">The table borders.</param>
        /// <param name="defaultBorders">The default borders.</param>
        /// <returns></returns>
        private Border GetCellLeftBorder(WTableCell cell, Borders tableBorders, DefaultBorders defaultBorders)
        {
            return ((cell.CellFormat.Borders.Left.BorderType != BorderStyle.None) ?
                    cell.CellFormat.Borders.Left
                    : ((cell.GetCellIndex() == 0) ?
                    (cell.OwnerRow.RowFormat.Borders.Left.IsBorderDefined ?
                    cell.OwnerRow.RowFormat.Borders.Left : tableBorders.Left)
                    : (cell.OwnerRow.RowFormat.Borders.Vertical.IsBorderDefined ?
                    cell.OwnerRow.RowFormat.Borders.Vertical : defaultBorders.Vertical)));
        }

        /// <summary>
        /// Gets the cell right border.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="tableBorders">The table borders.</param>
        /// <param name="defaultBorders">The default borders.</param>
        /// <returns></returns>
        private Border GetCellRightBorder(WTableCell cell, Borders tableBorders, DefaultBorders defaultBorders)
        {
            Border topCellHorizontalMergedCellRightBorder = null;
            bool isTopCellHorizantalMergedcell = false;
            int topCellNextCellIndex = cell.GetCellIndex();
            for (int i = cell.GetCellIndex() + 1; i < (cell.OwnerRow.Cells.Count - 1) + 1; i++)
            {
                if (cell.OwnerRow.Cells[i].CellFormat.HorizontalMerge != CellMerge.Continue)
                {
                    topCellNextCellIndex = i;
                    break;
                }
                else
                {
                    topCellHorizontalMergedCellRightBorder = cell.OwnerRow.Cells[i].CellFormat.Borders.Right;
                    isTopCellHorizantalMergedcell = true;
                }
            }
            Border rightBorder = (isTopCellHorizantalMergedcell ? topCellHorizontalMergedCellRightBorder : cell.CellFormat.Borders.Right);
            rightBorder = ((rightBorder.BorderType != BorderStyle.None) ?
                rightBorder    : ((cell.GetCellIndex() == (cell.OwnerRow.Cells.Count - 1) || topCellNextCellIndex == cell.GetCellIndex()) ?
                (cell.OwnerRow.RowFormat.Borders.Right.IsBorderDefined ?
                cell.OwnerRow.RowFormat.Borders.Right : tableBorders.Right)
                : (cell.OwnerRow.RowFormat.Borders.Vertical.IsBorderDefined ?
                cell.OwnerRow.RowFormat.Borders.Vertical : defaultBorders.Vertical)));
            return rightBorder;
        }

        /// <summary>
        /// Gets the cell top border.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="tableBorders">The table borders.</param>
        /// <param name="defaultBorders">The default borders.</param>
        /// <returns></returns>
        private Border GetCellTopBorder(WTableCell cell, Borders tableBorders, DefaultBorders defaultBorders)
        {
            int rowIndex = cell.OwnerRow.GetRowIndex();
            Borders rowBorders = cell.OwnerRow.RowFormat.Borders;
            ILayoutInfo rowInfo = (cell.OwnerRow as IWidget).LayoutInfo;
            Border topBorder = ((cell.CellFormat.Borders.Top.BorderType != BorderStyle.None) ?
                       cell.CellFormat.Borders.Top
                       : (rowInfo.IsFirstItemInPage ?
                       (rowBorders.Top.IsBorderDefined ?
                       rowBorders.Top : tableBorders.Top)
                       : (rowBorders.Horizontal.IsBorderDefined ?
                       rowBorders.Horizontal : defaultBorders.Horizontal)));

            if (topBorder.BorderType == BorderStyle.Cleared && rowIndex != 0 && rowBorders.Horizontal.IsBorderDefined)
                topBorder = rowBorders.Horizontal;
            return topBorder;
        }

        /// <summary>
        /// Gets the cell bottom border.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="tableBorders">The table borders.</param>
        /// <param name="defaultBorders">The default borders.</param>
        /// <returns></returns>
        private Border GetCellBottomBorder(WTableCell cell, Borders tableBorders, DefaultBorders defaultBorders)
        {
            Borders rowBorders = cell.OwnerRow.RowFormat.Borders;
            int rowIndex = cell.OwnerRow.GetRowIndex();
            int rowlLast = cell.OwnerRow.OwnerTable.Rows.Count - 1;
            Border verticalMergedCellBottomBorder = null;
            bool isVerticalMergedCell = false;
            int nextRowIndex = rowIndex;
            for (int i = rowIndex + 1; i < rowlLast + 1; i++)
            {
                int index = GetAdjacentCellIndex(cell, cell.GetCellIndex(), i);
                if (index < cell.OwnerRow.OwnerTable.Rows[i].Cells.Count)
                {
                    if (cell.OwnerRow.OwnerTable.Rows[i].Cells[index].CellFormat.VerticalMerge != CellMerge.Continue)
                    {
                        nextRowIndex = i;
                        break;
                    }
                    else
                    {
                        if (cell.OwnerRow.OwnerTable.Rows[i].Cells[index].CellFormat.Borders.Bottom.BorderType == BorderStyle.None && i == rowlLast)
                            verticalMergedCellBottomBorder = tableBorders.Bottom;
                        else
                            verticalMergedCellBottomBorder = cell.OwnerRow.OwnerTable.Rows[i].Cells[index].CellFormat.Borders.Bottom;
                        isVerticalMergedCell = true;
                    }
                }
                else
                {
                    nextRowIndex = i;
                    break;
                }
            }
            Border bottomBorder = (isVerticalMergedCell) ? verticalMergedCellBottomBorder : cell.CellFormat.Borders.Bottom;
            bottomBorder = ((cell.CellFormat.Borders.Bottom.BorderType != BorderStyle.None) ?
                   cell.CellFormat.Borders.Bottom
                    : rowBorders.Bottom.IsBorderDefined ?
                    rowBorders.Bottom : tableBorders.Bottom);
            return bottomBorder;
        }
        /// <summary>
        /// Draw ThinThickThin Vertical border style
        /// </summary>
        /// <param name="leftBorder"></param>
        /// <param name="bounds"></param>
        private void DrawThinThickThinLeftBorder(Border border, Borders cellBorders,  PointF cellStart, PointF cellEnd, Border prevCellTopBorder, Border prevCellBottomBorder)
        {
            float[] penWidth = GetThinThickThinPenWidth(border.LineWidth);
            float lineGap = (0.8f * 3.25f);// line gap between center line and outer lines
            float startGap = (penWidth[0] * 0.5f);// Start gap to adjust the center and outer lines edges.
            float endGap = (penWidth[0] * 0.5f);//end gap to adjust the center and outer lines edges.
            PointF start = cellStart;
            PointF end = cellEnd;
            Pen pen = GetPen(border);
            //Check the current cell's top and bottom type for adjusting line edges
            if ((cellBorders.Top.BorderType == BorderStyle.ThinThickThinSmallGap) || (prevCellTopBorder != null && prevCellTopBorder.BorderType == BorderStyle.ThinThickThinSmallGap))
                startGap = lineGap;
            if ((cellBorders.Bottom.BorderType == BorderStyle.ThinThickThinSmallGap) || (prevCellBottomBorder != null && prevCellBottomBorder.BorderType == BorderStyle.ThinThickThinSmallGap))
                endGap = -(lineGap);
            //Draw outer line
            pen.Width = penWidth[0];
            start = new PointF(start.X, start.Y + (lineGap * 2));
            //join the edges if the previous cell top and bottom border type is not equal to thin thick border.
            if ((prevCellTopBorder != null && prevCellTopBorder.BorderType != BorderStyle.ThinThickThinSmallGap)||prevCellTopBorder ==null)
                start = new PointF(start.X, start.Y - (lineGap * 2));
            if ((prevCellBottomBorder != null && prevCellBottomBorder.BorderType != BorderStyle.ThinThickThinSmallGap)||prevCellBottomBorder ==null)
                end = new PointF(end.X, end.Y + (lineGap * 2));
            Graphics.DrawLine(pen, start, end);
            //Draw center line
            pen.Width = penWidth[2] - 0.25f;
            start = new PointF(cellStart.X + lineGap, cellStart.Y + startGap);
            end = new PointF(cellEnd.X + lineGap, cellEnd.Y - endGap);
            Graphics.DrawLine(pen, start, end);
            //Draw inner line
            pen.Width = penWidth[0];
            if (cellBorders.Top.BorderType != BorderStyle.ThinThickThinSmallGap)
                start = new PointF(start.X + lineGap, start.Y- startGap);
            else
                start = new PointF(start.X + lineGap, start.Y + (lineGap * 2) - startGap);
            end = new PointF(end.X + lineGap, end.Y  + endGap);
            if (cellBorders.Bottom.BorderType != BorderStyle.ThinThickThinSmallGap)
                end = new PointF(end.X, end.Y + (lineGap * 2));
            Graphics.DrawLine(pen, start, end);
        }
        /// <summary>
        /// Draw ThinThickThin Vertical border style
        /// </summary>
        /// <param name="leftBorder"></param>
        /// <param name="bounds"></param>
        private void DrawThinThickThinRightBorder(Border border, Borders cellBorders, PointF cellStart, PointF cellEnd, Border nextCellTopBorder, Border nextCellBottomBorder)
        {
            float[] penWidth = GetThinThickThinPenWidth(border.LineWidth);
            float lineGap = (0.8f * 3.25f);
            float startGap = (penWidth[0] * 0.5f);
            float endGap = (penWidth[0] * 0.5f);
            PointF start = cellStart;
            PointF end = cellEnd;
            Pen pen = GetPen(border);
            if ((cellBorders.Top.BorderType == BorderStyle.ThinThickThinSmallGap) || (nextCellTopBorder != null && nextCellTopBorder.BorderType == BorderStyle.ThinThickThinSmallGap))
                startGap = lineGap;
            if ((cellBorders.Bottom.BorderType == BorderStyle.ThinThickThinSmallGap) || (nextCellBottomBorder != null && nextCellBottomBorder.BorderType == BorderStyle.ThinThickThinSmallGap))
                endGap = -lineGap;
            //Draw outer line
            pen.Width = penWidth[0];
            start = new PointF(start.X, start.Y + (lineGap * 2));
            if (cellBorders.Top.BorderType != BorderStyle.ThinThickThinSmallGap)
                start = new PointF(start.X, start.Y - (lineGap * 2));
            if (cellBorders.Bottom.BorderType != BorderStyle.ThinThickThinSmallGap)
                end = new PointF(end.X, end.Y + (lineGap * 2));
            Graphics.DrawLine(pen, start, end);
            //Draw center line
            pen.Width = penWidth[2] - 0.25f;
            start = new PointF(cellStart.X + lineGap, cellStart.Y + startGap);
            end = new PointF(cellEnd.X + lineGap, cellEnd.Y - endGap);
            Graphics.DrawLine(pen, start, end);
            //Draw inner line
            pen.Width = penWidth[0];
            //start = new PointF(start.X  + lineGap, start.Y -startGap);
            //end = new PointF(end.X + lineGap, end.Y + (lineGap * 2) + endGap);

            if ((nextCellTopBorder != null && nextCellTopBorder.BorderType != BorderStyle.ThinThickThinSmallGap) || nextCellTopBorder == null)
                start = new PointF(start.X + lineGap, start.Y - startGap);
            else
                start = new PointF(start.X + lineGap, start.Y + (lineGap * 2) - startGap);
            end = new PointF(end.X + lineGap, end.Y + endGap);
            if ((nextCellBottomBorder != null && nextCellBottomBorder.BorderType != BorderStyle.ThinThickThinSmallGap) || nextCellBottomBorder == null)
                end = new PointF(end.X, end.Y + (lineGap * 2));
            Graphics.DrawLine(pen, start, end);
        }
        /// <summary>
        /// Draw ThinThickThin Horizontal border style
        /// </summary>
        /// <param name="leftBorder"></param>
        /// <param name="bounds"></param>
        private void DrawThinThickThinTopBorder(Border border, Borders cellBorders, int cellIndex, int cellLast, int rowIndex, PointF cellStart, PointF cellEnd, Border topCellLeftBorder, Border topCellRightBorder)
        {
            float[] penWidth = GetThinThickThinPenWidth(border.LineWidth);
            float lineGap = (0.8f * 3.25f);
            float startGap = (penWidth[0] * 0.5f);
            float endGap = (penWidth[0] * 0.5f);
            PointF start = cellStart;
            PointF end = cellEnd;
            Pen pen = GetPen(border);
            if ((cellBorders.Left.BorderType == BorderStyle.ThinThickThinSmallGap)||(topCellLeftBorder != null && topCellLeftBorder.BorderType == BorderStyle.ThinThickThinSmallGap))
                startGap = lineGap;
            if ((cellBorders.Right.BorderType == BorderStyle.ThinThickThinSmallGap)||(topCellRightBorder != null && topCellRightBorder.BorderType == BorderStyle.ThinThickThinSmallGap))
                endGap = -(lineGap);
            if (rowIndex >0 )
            {
                start = new PointF(start.X + (lineGap * 2), start.Y);
                end = new PointF(end.X - (lineGap * 2), end.Y);
            }
            //Draw outer line
            pen.Width = penWidth[0];
            end = new PointF(end.X + (lineGap * 2), end.Y);
            if ((topCellLeftBorder != null && topCellLeftBorder.BorderType != BorderStyle.ThinThickThinSmallGap))
                start = new PointF(start.X - (lineGap * 2), start.Y);
            if ((topCellRightBorder != null && topCellRightBorder.BorderType != BorderStyle.ThinThickThinSmallGap)&&(cellIndex !=cellLast && topCellRightBorder .BorderType !=BorderStyle.Single))
                end = new PointF(end.X + (lineGap * 2), end.Y);
            Graphics.DrawLine(pen, start, end);
            //Draw center line
            pen.Width = penWidth[2] - 0.25f;
            start = new PointF(cellStart.X+ startGap , cellStart.Y+ lineGap );
            end = new PointF(cellEnd.X-endGap , cellEnd.Y  + lineGap);
            Graphics.DrawLine(pen, start, end);
            //Draw inner line
            pen.Width = penWidth[0];
            if (cellBorders.Left.BorderType != BorderStyle.ThinThickThinSmallGap)
                start = new PointF(start.X - startGap, start.Y + lineGap);
            else
                start = new PointF(start.X + (lineGap * 2) - startGap, start.Y + lineGap);
            end = new PointF(end.X + endGap, end.Y + lineGap);
            if (cellBorders.Right.BorderType != BorderStyle.ThinThickThinSmallGap && cellIndex !=cellLast && cellBorders.Right.BorderType!=BorderStyle.Single)
                end = new PointF(end.X + (lineGap * 2), end.Y);
            Graphics.DrawLine(pen, start, end);
        }
        /// <summary>
        /// Draw ThinThickThin Horizontal border style
        /// </summary>
        /// <param name="leftBorder"></param>
        /// <param name="bounds"></param>
        private void DrawThinThickThinBottomBorder(Border border, Borders cellBorders, PointF cellStart, PointF cellEnd, Border belowCellLeftBorder, Border belowCellRightBorder)
        {
            float[] penWidth = GetThinThickThinPenWidth(border.LineWidth);
            float lineGap = (0.8f * 3.25f);
            float startGap = (penWidth[0] * 0.5f);
            float endGap = (penWidth[0] * 0.5f);
            PointF start = cellStart;
            PointF end = cellEnd;
            Pen pen = GetPen(border);
            if ((cellBorders.Left.BorderType == BorderStyle.ThinThickThinSmallGap) || (belowCellLeftBorder != null && belowCellLeftBorder.BorderType == BorderStyle.ThinThickThinSmallGap))
                startGap = lineGap;
            if ((cellBorders.Right.BorderType == BorderStyle.ThinThickThinSmallGap) || (belowCellRightBorder != null && belowCellRightBorder.BorderType == BorderStyle.ThinThickThinSmallGap))
                endGap = -(lineGap);
                        
            //Draw outer line
            pen.Width = penWidth[0];
            start = new PointF(start.X + (lineGap * 2), start.Y);
            if (cellBorders.Left.BorderType != BorderStyle.ThinThickThinSmallGap)
                start = new PointF(start.X - (lineGap * 2), start.Y);
            if (cellBorders.Right.BorderType != BorderStyle.ThinThickThinSmallGap)
                end = new PointF(end.X + (lineGap * 2), end.Y);
            Graphics.DrawLine(pen, start, end);
            //Draw center line
            pen.Width = penWidth[2] - 0.25f;
            start = new PointF(cellStart.X+ startGap , cellStart.Y+ lineGap );
            end = new PointF(cellEnd.X-endGap , cellEnd.Y  + lineGap);
            Graphics.DrawLine(pen, start, end);
            //Draw inner line
            pen.Width = penWidth[0];
            if ((belowCellLeftBorder != null && belowCellLeftBorder.BorderType != BorderStyle.ThinThickThinSmallGap) || belowCellLeftBorder == null)
                start = new PointF(start.X  - startGap, start.Y + lineGap);
            else
                start = new PointF(start.X + (lineGap * 2) - startGap, start.Y + lineGap);
            end = new PointF(end.X + endGap, end.Y + lineGap);
            if ((belowCellRightBorder != null && belowCellRightBorder.BorderType != BorderStyle.ThinThickThinSmallGap)||belowCellRightBorder==null)
                end = new PointF(end.X + (lineGap * 2), end.Y);
            Graphics.DrawLine(pen, start, end);
        }
        /// <summary>
        /// Get Array values of ThinThickThinPenWidth
        /// </summary>
        /// <returns></returns>
        private float[] GetThinThickThinPenWidth(float lineWidth)
        {
            float[] numArray = (float[])ThinThickThinArray.Clone();
            for (int i = 0; i < numArray.Length; i++)
            {
                if (numArray[i] >= 0)
                {
                    numArray[i] *= lineWidth;
                }
                else
                {
                    numArray[i] = Math.Abs(numArray[i]);
                }
            }
            return numArray;
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
                if (cellStartPos == adjCellStartPos)
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
        #endregion

        #region Document Background
        /// <summary>
        /// Draws the color of the background.
        /// </summary>
        /// <param name="bgColor">Color of the bg.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        internal void DrawBackgroundColor(Color bgColor, int width, int height)
        {
            using (SolidBrush brush = new SolidBrush(bgColor))
            {
                Graphics.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, width, height));
                brush.Dispose();
            }
        }

        /// <summary>
        /// Draws the back ground image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="pageSetup">The page setup.</param>
        internal void DrawBackgroundImage(Image image, WPageSetup pageSetup)
        {
            Graphics.DrawImage(image, 0, 0, pageSetup.PageSize.Width, pageSetup.PageSize.Height);
        }
        /// <summary>
        /// Draws the Watermark
        /// </summary>
        /// <param name="watermark">Watermark</param>
        /// <param name="width">Page width</param>
        /// <param name="height">Page height</param>
        internal void DrawWatermark(Watermark watermark,WPageSetup pageSetup, RectangleF bounds)
        {
            switch (watermark.Type)
            {
                case WatermarkType.PictureWatermark:
                    DrawImageWatermark(watermark as PictureWatermark, bounds, pageSetup);
                    break;
                case WatermarkType.TextWatermark:
                    DrawTextWatermark(watermark as TextWatermark, bounds);
                    break;
                case WatermarkType.NoWatermark:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Draws the Text Watermark
        /// </summary>
        /// <param name="textWatermark">Text Watermark</param>
        /// <param name="bounds">Bounds</param>
        private void DrawTextWatermark(TextWatermark textWatermark, RectangleF bounds)
        {
            TextWatermark txtWatermark = textWatermark as TextWatermark;
            Font font = new Font(txtWatermark.FontName, (txtWatermark.Size == 1.0f) ? GetFontSize(textWatermark) : txtWatermark.Size);
            bool isPortrait = false;
            float diagonalWidth = 0.0f;
            if (bounds.Width <= bounds.Height)
            {
                isPortrait = true;
                diagonalWidth = (float)Math.Sqrt(2 * (bounds.Width * bounds.Width));
            }
            else
            {
                diagonalWidth = (float)Math.Sqrt(2 * (bounds.Height * bounds.Height));
            }
            if (txtWatermark.Size == 1.0f)
            {
                bounds.X += bounds.Width / 2 - (textWatermark.ShapeWidthInPixels / 20) / 2;
                bounds.Y += bounds.Height / 2 - (textWatermark.ShapeHeightInPixels / 20) / 2;
                bounds.Height = textWatermark.ShapeHeightInPixels / 20;
                bounds.Width = textWatermark.ShapeWidthInPixels / 20;
            }
            else
            {
                bounds.X += bounds.Width / 2 - (textWatermark.ShapeSize.Width) / 2;
                bounds.Y += bounds.Height / 2 - (textWatermark.ShapeSize.Height) / 2;
                bounds.Height = textWatermark.ShapeSize.Height;
                bounds.Width = textWatermark.ShapeSize.Width;
            }
            if (txtWatermark.Layout == WatermarkLayout.Diagonal)
            {
                if (txtWatermark.Semitransparent)
                {
                    Bitmap bmp = new Bitmap(1, 1);
                    int intWidth = 0;
                    int intHeight = 0;
                    Graphics objGraphics = Graphics.FromImage(bmp);
                    intWidth = (int)objGraphics.MeasureString(textWatermark.Text, font).Width;
                    intHeight = (int)objGraphics.MeasureString(textWatermark.Text, font).Height;
                    bmp = new Bitmap(bmp, new Size(intWidth, intHeight));
                    objGraphics = Graphics.FromImage(bmp);
                    objGraphics.Clear(Color.White);
                    objGraphics.SmoothingMode = SmoothingMode.HighQuality;
                    objGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    objGraphics.DrawString(textWatermark.Text, font, new SolidBrush(textWatermark.Color), 0, 0);
                    ColorMatrix cmxBmp = new ColorMatrix();
                    cmxBmp.Matrix33 = 0.2f;
                    ImageAttributes iaBmp = new ImageAttributes();
                    iaBmp.SetColorMatrix(cmxBmp, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    GraphicsState gs = Graphics.Save();
                    Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    float translateTransformX = bounds.X + bounds.Width / 2;
                    Graphics.RotateTransform(-45);
                    if (isPortrait)
                    {
                        Graphics.TranslateTransform(-translateTransformX, -50);
                        bounds.Width = (float)(bounds.Width * 0.83);
                    }
                    else
                    {
                        Graphics.TranslateTransform(-(3 * diagonalWidth / 4) / 2, 50);
                        bounds.Width = 3 * bounds.Width / 4;
                    }
                    Graphics.DrawImage(bmp, new Rectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height), 0, 0, bounds.Width, bounds.Height, GraphicsUnit.Pixel, iaBmp);
                    Graphics.Restore(gs);
                }
                else
                {
                    Bitmap bmp = new Bitmap(1, 1);
                    int intWidth = 0;
                    int intHeight = 0;
                    Graphics objGraphics = Graphics.FromImage(bmp);
                    intWidth = (int)objGraphics.MeasureString(textWatermark.Text, font).Width;
                    intHeight = (int)objGraphics.MeasureString(textWatermark.Text, font).Height;
                    bmp = new Bitmap(bmp, new Size(intWidth, intHeight));
                    objGraphics = Graphics.FromImage(bmp);
                    objGraphics.Clear(Color.White);
                    objGraphics.SmoothingMode = SmoothingMode.HighQuality;
                    objGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    objGraphics.DrawString(textWatermark.Text, font, new SolidBrush(textWatermark.Color), 0, 0);
                    ColorMatrix cmxBmp = new ColorMatrix();
                    cmxBmp.Matrix33 = 0.5f;
                    ImageAttributes iaBmp = new ImageAttributes();
                    iaBmp.SetColorMatrix(cmxBmp, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    GraphicsState gs = Graphics.Save();
                    Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    float translateTransformX = bounds.X + bounds.Width / 2;
                    Graphics.RotateTransform(-45);
                    if (isPortrait)
                    {
                        Graphics.TranslateTransform(-translateTransformX, -50);
                        bounds.Width = (float)(bounds.Width * 0.83);
                    }
                    else
                    {
                        Graphics.TranslateTransform(-(3 * diagonalWidth / 4) / 2, 50);
                        bounds.Width = 3 * bounds.Width / 4;
                    }
                    Graphics.DrawImage(bmp, new Rectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height), 0, 0, bounds.Width, bounds.Height, GraphicsUnit.Pixel, iaBmp);
                    Graphics.Restore(gs);
                }
            }
            else
            {
                if (txtWatermark.Semitransparent)
                {
                    Bitmap bmp = new Bitmap(1, 1);
                    int intWidth = 0;
                    int intHeight = 0;
                    Graphics objGraphics = Graphics.FromImage(bmp);
                    intWidth = (int)objGraphics.MeasureString(textWatermark.Text, font).Width;
                    intHeight = (int)objGraphics.MeasureString(textWatermark.Text, font).Height;
                    bmp = new Bitmap(bmp, new Size(intWidth, intHeight));
                    objGraphics = Graphics.FromImage(bmp);
                    objGraphics.Clear(Color.White);
                    objGraphics.SmoothingMode = SmoothingMode.HighQuality;
                    objGraphics.DrawString(textWatermark.Text, font, new SolidBrush(textWatermark.Color), 0, 0);
                    ColorMatrix cmxBmp = new ColorMatrix();
                    cmxBmp.Matrix33 = 0.2f;
                    ImageAttributes iaBmp = new ImageAttributes();
                    iaBmp.SetColorMatrix(cmxBmp, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    Graphics.DrawImage(bmp, new Rectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height), 0, 0, bounds.Width, bounds.Height, GraphicsUnit.Pixel, iaBmp);
                }
                else
                {
                    Bitmap bmp = new Bitmap(1, 1);
                    int intWidth = 0;
                    int intHeight = 0;
                    Graphics objGraphics = Graphics.FromImage(bmp);
                    intWidth = (int)objGraphics.MeasureString(textWatermark.Text, font).Width;
                    intHeight = (int)objGraphics.MeasureString(textWatermark.Text, font).Height;
                    bmp = new Bitmap(bmp, new Size(intWidth, intHeight));
                    objGraphics = Graphics.FromImage(bmp);
                    objGraphics.Clear(Color.White);
                    objGraphics.SmoothingMode = SmoothingMode.HighQuality;
                    objGraphics.DrawString(textWatermark.Text, font, new SolidBrush(textWatermark.Color), 0, 0);
                    ColorMatrix cmxBmp = new ColorMatrix();
                    cmxBmp.Matrix33 = 0.5f;
                    ImageAttributes iaBmp = new ImageAttributes();
                    iaBmp.SetColorMatrix(cmxBmp, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    Graphics.DrawImage(bmp, new Rectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height), 0, 0, bounds.Width, bounds.Height, GraphicsUnit.Pixel, iaBmp);
                }
            }

        }
        /// <summary>
        /// Draw the Page Border
        /// </summary>
        /// <param name="sec">Section</param>
        /// <param name="bounds">Bounds</param>
        internal void DrawPageBorder(WPageSetup pageSetup, RectangleF headerBounds, RectangleF footerBounds)
        {
            float left, right, top, bottom;
            switch (pageSetup.PageBorderOffsetFrom)
            {
                case PageBorderOffsetFrom.PageEdge:
                    left = pageSetup.Borders.Left.Space;
                    right = pageSetup.Borders.Right.Space;
                    top = pageSetup.Borders.Top.Space;
                    bottom = pageSetup.Borders.Bottom.Space;
                    if (pageSetup.Borders.Left.BorderType != BorderStyle.None)
                        DrawBorder(pageSetup.Borders.Left, new PointF(left, top), new PointF(left, pageSetup.PageSize.Height - bottom));
                    if (pageSetup.Borders.Right.BorderType != BorderStyle.None)
                        DrawBorder(pageSetup.Borders.Right, new PointF(pageSetup.PageSize.Width - right, top), new PointF(pageSetup.PageSize.Width - right, pageSetup.PageSize.Height - bottom));
                    if (pageSetup.Borders.Top.BorderType != BorderStyle.None)
                        DrawBorder(pageSetup.Borders.Top, new PointF(left, top), new PointF(pageSetup.PageSize.Width - right, top));
                    if (pageSetup.Borders.Bottom.BorderType != BorderStyle.None)
                        DrawBorder(pageSetup.Borders.Bottom, new PointF(left, pageSetup.PageSize.Height - bottom), new PointF(pageSetup.PageSize.Width - right, pageSetup.PageSize.Height - bottom));
                    break;
                case PageBorderOffsetFrom.Text:
                    left = pageSetup.Borders.Left.Space;
                    right = pageSetup.Borders.Right.Space;
                    top = pageSetup.Borders.Top.Space;
                    bottom = pageSetup.Borders.Bottom.Space;
                    if (pageSetup.Borders.Left.BorderType != BorderStyle.None)
                        DrawBorder(pageSetup.Borders.Left, new PointF(headerBounds.X - left, headerBounds.Y - top), new PointF(headerBounds.X - left, footerBounds.Bottom + bottom));
                    if (pageSetup.Borders.Right.BorderType != BorderStyle.None)
                        DrawBorder(pageSetup.Borders.Right, new PointF(pageSetup.ClientWidth + headerBounds.X + right, headerBounds.Y - top), new PointF(pageSetup.ClientWidth + headerBounds.X + right, footerBounds.Bottom + bottom));
                    if (pageSetup.Borders.Top.BorderType != BorderStyle.None)
                        DrawBorder(pageSetup.Borders.Top, new PointF(headerBounds.X - left, headerBounds.Y - top), new PointF(pageSetup.ClientWidth + headerBounds.X + right, headerBounds.Y - top));
                    if (pageSetup.Borders.Bottom.BorderType != BorderStyle.None)
                        DrawBorder(pageSetup.Borders.Bottom, new PointF(headerBounds.X - left, footerBounds.Bottom + bottom), new PointF(pageSetup.ClientWidth + headerBounds.X + right, footerBounds.Bottom + bottom));
                    break;
            }
        }
        /// <summary>
        /// Gets the font size for Text Watermark
        /// </summary>
        /// <param name="textWatermark">Text Watermark</param>
        private float GetFontSize(TextWatermark textWatermark)
        {
            float fontSize = 8.0f;
            float diagonalWidth = 0.0f;
            diagonalWidth = textWatermark.ShapeWidthInPixels / 20;
            Bitmap bmp = new Bitmap(1, 1);
            int intWidth = 0;
            float x = 8f;
            Graphics objGraphics = Graphics.FromImage(bmp);
            while (intWidth <= diagonalWidth)
            {
                Font font = new Font(textWatermark.FontName, x);
                intWidth = (int)objGraphics.MeasureString(textWatermark.Text, font).Width;
                if (intWidth <= diagonalWidth)
                {
                    fontSize = x;
                    x++;
                }
            }
            return fontSize;
        }
        /// <summary>
        /// Draws the Picture Watermark
        /// </summary>
        /// <param name="pictureWatermark">Picture Watermark</param>
        /// <param name="bounds">Bounds</param>
        private void DrawImageWatermark(PictureWatermark pictureWatermark, RectangleF bounds, WPageSetup pageSetup)
        {
            PictureWatermark pictWatermark = pictureWatermark as PictureWatermark;
            Bitmap bmp = new Bitmap(pictureWatermark.Picture);
            bounds = UpdatePictureWaterMarkPosition(pictureWatermark.WordPicture, pageSetup, bounds);
            if (pictureWatermark.Washout)
            {
                float brightness = 0.86f; //brightness
                float constrast = 0.3f; //contrast
                float gamma = 1.0f; // no change in gamma

                Graphics.FromImage(bmp);
                ColorMatrix cmxBmp = new ColorMatrix();

                // create matrix that adjust image brighten and contrast based on given value
                float[][] colorMatrix ={
                    new float[] {(float)constrast, 0, 0, 0, 0}, // scale red
                    new float[] {0, (float)constrast, 0, 0, 0}, // scale green
                    new float[] {0, 0, (float)constrast, 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                    new float[] {brightness, brightness, brightness, 1, 1}};

                //Create image attribute to set custom matrix for adjust image brightness and contrast.  
                ImageAttributes imageAttributes = new ImageAttributes();

                //Apply the custom matrix 
                imageAttributes.SetColorMatrix(new ColorMatrix(colorMatrix), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                imageAttributes.SetGamma((float)gamma, ColorAdjustType.Bitmap);
                Graphics.DrawImage(bmp, new Rectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height), 0, 0, bounds.Width, bounds.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            else
            {
                Graphics.DrawImage(bmp, new Rectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height), 0, 0, bounds.Width, bounds.Height, GraphicsUnit.Pixel);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private RectangleF UpdatePictureWaterMarkPosition(WPicture pic, WPageSetup pageSetup, RectangleF bounds)
        {
            if (pic.TextWrappingStyle != TextWrappingStyle.Inline)
            {
                float indentY = bounds.Top + pic.VerticalPosition;
                float indentX = bounds.Left + pic.HorizontalPosition;
                //Update Vertical Position
                switch (pic.VerticalOrigin)
                {
                    case VerticalOrigin.Page:
                    case VerticalOrigin .TopMargin:
                        {
                            indentY = pic.VerticalPosition;
                            switch (pic.VerticalAlignment)
                            {
                                case ShapeVerticalAlignment.Top:
                                    indentY = 0;
                                    break;
                                case ShapeVerticalAlignment.Center:
                                    indentY = (pageSetup.PageSize.Height - pic.Height) / 2;
                                    break;
                                case ShapeVerticalAlignment.Bottom:
                                    indentY = (pageSetup.PageSize.Height - pic.Height);
                                    break;
                            }
                        }
                        break;
                    case VerticalOrigin.Margin:
                        {
                            switch (pic.VerticalAlignment)
                            {
                                case ShapeVerticalAlignment.Top:
                                    indentY = bounds.Y;
                                    break;
                                case ShapeVerticalAlignment.Center:
                                    indentY = bounds.Y + (bounds.Height - pic.Height) / 2;
                                    break;
                                case ShapeVerticalAlignment.Bottom:
                                    indentY = bounds.Y + bounds.Height - pic.Height;
                                    break;
                            }
                        }
                        break;
                }
                switch (pic.HorizontalOrigin)
                {
                    case HorizontalOrigin.Page:
                        {
                            indentX = pic.HorizontalPosition;
                            switch (pic.HorizontalAlignment)
                            {
                                case ShapeHorizontalAlignment.Center:
                                    indentX = (pageSetup.PageSize.Width - pic.Width) / 2;
                                    break;
                                case ShapeHorizontalAlignment.Left:
                                    indentX = 0;
                                    break;
                                case ShapeHorizontalAlignment.Right:
                                    indentX = (pageSetup.PageSize.Width - pic.Width);
                                    break;
                            }
                        }
                        break;
                    case HorizontalOrigin.Column:
                    case HorizontalOrigin.Margin:
                        {
                            switch (pic.HorizontalAlignment)
                            {
                                case ShapeHorizontalAlignment.Center:
                                    indentX = bounds.X + (bounds.Width - pic.Width) / 2;
                                    break;
                                case ShapeHorizontalAlignment.Left:
                                    indentX = bounds.Left;
                                    break;
                                case ShapeHorizontalAlignment.Right:
                                    indentX = bounds.Left + bounds.Width - pic.Width;
                                    break;
                                case ShapeHorizontalAlignment.None:
                                    break;
                            }
                        }
                        break;
                }
                bounds.X = indentX;
                bounds.Y = indentY;
                if (bounds.Y < 0)
                    bounds.Y = 0;
                if (bounds.X < 0)
                    bounds.X = 0;
            }
            bounds.Height = pic.Height;
            bounds.Width = pic.Width;
            return bounds;
        }
        #endregion

        #region Form Controls
        /// <summary>
        /// Draws the check box.
        /// </summary>
        /// <param name="checkbox">The checkbox.</param>
        /// <param name="ltBounds">The lt bounds.</param>
        internal void DrawCheckBox(WCheckBox checkbox, LayoutedWidget ltWidget)
        {
            Pen borderPen = new Pen(GetTextColor(checkbox.CharacterFormat));
            borderPen.Width = .6f;
            RectangleF rect = new RectangleF((ltWidget.Bounds.X + borderPen.Width), (ltWidget.Bounds.Y + borderPen.Width), (ltWidget.Bounds.Width - 2 * borderPen.Width), (ltWidget.Bounds.Height - 2 * borderPen.Width));
            Graphics.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);

            if (checkbox.Checked)
            {
                System.Drawing.PointF pt1 = new System.Drawing.PointF(rect.X, rect.Y);
                System.Drawing.PointF pt2 = new System.Drawing.PointF(rect.Right, rect.Bottom);
                Graphics.DrawLine(borderPen, pt1, pt2);

                pt1 = new System.Drawing.PointF(rect.Right, rect.Top);
                pt2 = new System.Drawing.PointF(rect.Left, rect.Bottom);
                Graphics.DrawLine(borderPen, pt1, pt2);
            }
        }
        #endregion
        #region Draw AutoShape
        #region Get Theme Color
        /// <summary>
        /// Parse TextBox Graphics data
        /// </summary>
        /// <param name="textbox"></param>
        /// <param name="choiceItem"></param>
        private Color GetThemeColor(Dictionary<string, Stream> docxProps,WordDocument document, string localName)
        {
            Stream memoryStream = new MemoryStream();
            if (docxProps.TryGetValue("Style", out memoryStream))
            {
                if (memoryStream != null && memoryStream.Length > 0)
                {
                    memoryStream.Position = 0;
                    XmlReader reader = CreateReader(memoryStream);
                    reader.ReadToFollowing(localName, DocxConstants.A_namespace);
                    if (reader.NodeType != XmlNodeType.None)
                    {
                        if (localName == "fillRef")
                        {
                            string value = reader.GetAttribute("idx");
                            if (value != null && (value == "0" || value =="1000"))
                                return Color.Empty;
                        }
                        reader.Read();
                        SkipWhitespaces(reader);
                        if (reader.LocalName == "schemeClr")
                        {
                            return GetSchemeColor(reader, document);
                        }
                    }
                }
            }
            return Color.Empty;
        }
        /// <summary>
        /// Skip whitespaces and moves the reader to the next node.
        /// </summary>
        /// <param name="reader">The xml reader</param>
        private void SkipWhitespaces(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element)
                return;

            while (reader.NodeType == XmlNodeType.Whitespace)
                reader.Read();
        }
        /// <summary>
        /// Create xml reader
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns>returns xml reader</returns>
        private XmlReader CreateReader(Stream stream)
        {
            stream.Position = 0;
            XmlReader reader = XmlReader.Create(stream);
            reader.Read();

            while (reader.NodeType == XmlNodeType.XmlDeclaration)
                reader.Read();

            return reader;
        }
        /// <summary>
        /// Gets the color of the scheme.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private Color GetSchemeColor(XmlReader reader, WordDocument document)
        {
            Color themeColor = Color.Empty;
            string value = reader.GetAttribute("val");
            if (value != null)
            {
                if (document.SchemeColor.ContainsKey(value))
                    themeColor = document.SchemeColor[value];
                else
                {
                    switch (value)
                    {
                        case "accent1":
                            themeColor = Color.FromArgb(0xFF, 0x4F, 0x81, 0xBD);
                            break;
                        case "accent2":
                            themeColor = Color.FromArgb(0xFF, 0xC0, 0x50, 0x4D);
                            break;
                        case "accent3":
                            themeColor = Color.FromArgb(0xFF, 0x9B, 0xBB, 0x59);
                            break;
                        case "accent4":
                            themeColor = Color.FromArgb(0xFF, 0x80, 0x64, 0xA2);
                            break;
                        case "accent5":
                            themeColor = Color.FromArgb(0xFF, 0x4B, 0xAC, 0xC6);
                            break;
                        case "accent6":
                            themeColor = Color.FromArgb(0xFF, 0xF7, 0x96, 0x46);
                            break;
                        case "dk1":
                        case "tx1":
                        case "phClr":
                            themeColor = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
                            break;
                        case "dk2":
                        case "tx2":
                            themeColor = Color.FromArgb(0xFF, 0x1F, 0x49, 0x7D);
                            break;
                        case "folHlink":
                            themeColor = Color.FromArgb(0xFF, 0x80, 0x00, 0x80);
                            break;
                        case "hlink":
                            themeColor = Color.FromArgb(0xFF, 0x00, 0x00, 0xFF);
                            break;
                        case "bg1":
                        case "lt1":
                            themeColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                            break;
                        case "bg2":
                        case "lt2":
                            themeColor = Color.FromArgb(0xFF, 0xEE, 0xEC, 0xE1);
                            break;
                    }
                }
            }
            //Apply Shade
            if (themeColor != Color.Empty)
            {
                reader.ReadToFollowing("shade", DocxConstants.A_namespace);
                if (reader.NodeType != XmlNodeType.None)
                {
                    value = reader.GetAttribute("val");
                    if (value != null)
                    {
                        double percent = GetPercentage(value) / DLSConstants.HundredthsUnit;
                        themeColor = WordColor.ConvertColorByShade(themeColor, percent);
                    }
                }
            }
            return themeColor;
        }
        /// <summary>
        /// Gets the percentage.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private double GetPercentage(string value)
        {
            double percent;
            if (value.EndsWith("%"))
                percent = double.Parse(value.Replace("%", ""), NumberStyles.Number, CultureInfo.InvariantCulture);
            else
                percent = double.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.ThousandthsUnit;
            return percent;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="ltWidget"></param>
        internal void DrawShape(Shape shape, LayoutedWidget ltWidget)
        {
            RectangleF bounds = ltWidget.Bounds;
            //Reset Graphics Transform Position
            Graphics.ResetTransform();
            //Transform Graphics position
            if (ltWidget.Widget.LayoutInfo.IsVerticalText)
            {
                WTableCell tableCell = (ltWidget.Widget as WPicture).GetOwnerParagraph().OwnerTextBody as WTableCell;
                float cellLeftMargin = (tableCell.m_layoutInfo as TableLayoutInfo).TableCellLeftMargin;
                float cellTopMArgin = GetCellTopMargin(ltWidget);
                float cellWidth = (float)(tableCell.m_layoutInfo as TableLayoutInfo).VerticalCellWidth;
                float leftPad = (float)(shape.GetOwnerParagraph().OwnerTextBody as WTableCell).m_layoutInfo.Paddings.Left;
                float yposition = cellTopMArgin - bounds.Y + (bounds.X - cellLeftMargin) + leftPad;
                if (tableCell.CellFormat.TextDirection == TextDirection.VerticalTopToBottom)
                {
                    float xPosition = GetCellHeightForVerticalText(shape) + cellLeftMargin - bounds.Y - bounds.Width - leftPad;
                    Graphics.TranslateTransform(xPosition, yposition);
                }
                else
                {
                    Graphics.TranslateTransform((bounds.Y - cellTopMArgin), yposition);
                }
            }
            //Draw Shape
            Color shapePenColor = GetThemeColor(shape.DocxProps, shape.Document, "lnRef");
            Color penColor = !shape.LineFormat.Color.IsEmpty ? shape.LineFormat.Color : shapePenColor != Color.Empty ? shapePenColor : Color.Black;
            Pen pen = new Pen(penColor);
            pen.Alignment = PenAlignment.Inset;
            pen.Width = shape.LineFormat.Weight;
            GraphicsPath path = GetGraphicsPath(shape, ltWidget, ref pen);
            if (path.PointCount > 0)
            {
                if (shape.FillFormat.Fill && IsShapeNeedToBeFill(shape.AutoShapeType))
                {
                    Color shapeFillColor = GetThemeColor(shape.DocxProps, shape.Document, "fillRef");
                    Color fillColor = !shape.FillFormat.Color.IsEmpty ? shape.FillFormat.Color : shapeFillColor != Color.Empty ? shapeFillColor : Color.Empty;
                    if (fillColor != Color.Empty)
                        Graphics.FillPath(new SolidBrush(fillColor), path);
                }
                if (shape.LineFormat.Line)
                    Graphics.DrawPath(pen, path);
            }
            //Reset Graphics Transform position
            Graphics.ResetTransform();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeType"></param>
        /// <returns></returns>
        private bool IsShapeNeedToBeFill(AutoShapeType shapeType)
        {
            switch(shapeType)
            {
                case AutoShapeType.Line:
                case AutoShapeType.StraightConnector:
                case AutoShapeType.CurvedConnector:
                    return false;
                default:
                    return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private GraphicsPath GetGraphicsPath(Shape shape, LayoutedWidget ltWidget, ref Pen pen)
        {
            ShapePath shapePath = new ShapePath(ltWidget.Bounds, shape.ShapeGuide);
            GraphicsPath path = new GraphicsPath();
            RectangleF bounds = ltWidget.Bounds;
            switch (shape.AutoShapeType)
            {
                #region Rectangles
                case AutoShapeType.Rectangle:
                case AutoShapeType.FlowChartProcess:
                    path.AddRectangle(bounds);
                    return path;
                case AutoShapeType.RoundedRectangle:
                    return shapePath.GetRoundedRectanglePath();
                case AutoShapeType.SnipSingleCornerRectangle:
                    return shapePath.GetSnipSingleCornerRectanglePath();
                case AutoShapeType.SnipSameSideCornerRectangle:
                    return shapePath.GetSnipSameSideCornerRectanglePath();
                case AutoShapeType.SnipDiagonalCornerRectangle:
                    return shapePath.GetSnipDiagonalCornerRectanglePath();
                case AutoShapeType.SnipAndRoundSingleCornerRectangle:
                    return shapePath.GetSnipAndRoundSingleCornerRectanglePath();
                case AutoShapeType.RoundSingleCornerRectangle:
                    return shapePath.GetRoundSingleCornerRectanglePath();
                case AutoShapeType.RoundSameSideCornerRectangle:
                    return shapePath.GetRoundSameSideCornerRectanglePath();
                case AutoShapeType.RoundDiagonalCornerRectangle:
                    return shapePath.GetRoundDiagonalCornerRectanglePath();
                #endregion
                #region Lines
                case AutoShapeType.Line:
                    path.AddLine(bounds.X, bounds.Y, bounds.Right, bounds.Bottom);
                    return path;
                case AutoShapeType.StraightConnector:
                    path.AddLine(bounds.X, bounds.Y, bounds.Right, bounds.Bottom);
                    return path;
                case AutoShapeType.ElbowConnector:
                    return shapePath.GetBentConnectorPath();
                case AutoShapeType.CurvedConnector:
                    return shapePath.GetCurvedConnectorPath();
                #endregion
                #region Basic Shapes
                case AutoShapeType.Oval:
                    path.AddEllipse(bounds);
                    return path;
                case AutoShapeType.RightTriangle:
                    PointF[] linePoints = new PointF[3];
                    linePoints[0] = new PointF(bounds.X, bounds.Bottom);
                    linePoints[1] = new PointF(bounds.X, bounds.Y);
                    linePoints[2] = new PointF(bounds.Right, bounds.Bottom);
                    path.AddLines(linePoints);
                    path.CloseFigure();
                    return path;
                case AutoShapeType.IsoscelesTriangle:
                    return shapePath.GetTrianglePath();
                case AutoShapeType.Parallelogram:
                case AutoShapeType.FlowChartData:
                    return shapePath.GetParallelogramPath();
                case AutoShapeType.Trapezoid:
                    return shapePath.GetTrapezoidPath();
                case AutoShapeType.Diamond:
                case AutoShapeType.FlowChartDecision:
                    linePoints = new PointF[4];
                    linePoints[0] = new PointF(bounds.X, bounds.Y + bounds.Height / 2);
                    linePoints[1] = new PointF(bounds.X + bounds.Width / 2, bounds.Y);
                    linePoints[2] = new PointF(bounds.Right, bounds.Y + bounds.Height / 2);
                    linePoints[3] = new PointF(bounds.X + bounds.Width / 2, bounds.Bottom);
                    path.AddLines(linePoints);
                    path.CloseFigure();
                    break;
                case AutoShapeType.RegularPentagon:
                    return shapePath.GetRegularPentagonPath();
                case AutoShapeType.Hexagon:
                    return shapePath.GetHexagonPath();
                case AutoShapeType.Heptagon:
                    return shapePath.GetHeptagonPath();
                case AutoShapeType.Octagon:
                    return shapePath.GetOctagonPath();
                case AutoShapeType.Decagon:
                    return shapePath.GetDecagonPath();
                case AutoShapeType.Dodecagon:
                    return shapePath.GetDodecagonPath();
                case AutoShapeType.Pie:
                    return shapePath.GetPiePath();
                case AutoShapeType.Chord:
                    return shapePath.GetChordPath();
                case AutoShapeType.Teardrop:
                    return shapePath.GetTearDropPath();
                case AutoShapeType.Frame:
                    return shapePath.GetFramePath();
                case AutoShapeType.HalfFrame:
                    return shapePath.GetHalfFramePath();
                case AutoShapeType.L_Shape:
                    return shapePath.GetL_ShapePath();
                case AutoShapeType.DiagonalStripe:
                    return shapePath.GetDiagonalStripePath();
                case AutoShapeType.Cross:
                    return shapePath.GetCrossPath();
                case AutoShapeType.Plaque:
                    return shapePath.GetPlaquePath();
                case AutoShapeType.Can:
                    return shapePath.GetCanPath();
                case AutoShapeType.Cube:
                    return shapePath.GetCubePath();
                case AutoShapeType.Bevel:
                    return shapePath.GetBevelPath();
                case AutoShapeType.Donut:
                    return shapePath.GetDonutPath();
                case AutoShapeType.NoSymbol:
                    return shapePath.GetNoSymbolPath();
                case AutoShapeType.BlockArc:
                    return shapePath.GetBlockArcPath();
                case AutoShapeType.FoldedCorner:
                    return shapePath.GetFoldedCornerPath();
                case AutoShapeType.SmileyFace:
                    GraphicsPath[] pathColl = shapePath.GetSmileyFacePath();
                    Color shapeFillColor = GetThemeColor(shape.DocxProps, shape.Document, "fillRef");
                    Color fillColor = !shape.FillFormat.Color.IsEmpty ? shape.FillFormat.Color : shapeFillColor != Color.Empty ? shapeFillColor : Color.Empty;
                    foreach (GraphicsPath drawPath in pathColl)
                    {
                        if (fillColor != Color.Empty)
                            Graphics.FillPath(new SolidBrush(fillColor), drawPath);
                        Graphics.DrawPath(pen, drawPath);
                    }
                    break;
                case AutoShapeType.Heart:
                    return shapePath.GetHeartPath();
                case AutoShapeType.LightningBolt:
                    return shapePath.GetLightningBoltPath();
                case AutoShapeType.Sun:
                    return shapePath.GetSunPath();
                case AutoShapeType.Moon:
                    return shapePath.GetMoonPath();
                case AutoShapeType.Cloud:
                    return shapePath.GetCloudPath();
                case AutoShapeType.Arc:
                    pathColl = shapePath.GetArcPath();
                    shapeFillColor = GetThemeColor(shape.DocxProps, shape.Document, "fillRef");
                    fillColor = !shape.FillFormat.Color.IsEmpty ? shape.FillFormat.Color : shapeFillColor != Color.Empty ? shapeFillColor : Color.Empty;
                    if (fillColor != Color.Empty)
                        Graphics.FillPath(new SolidBrush(fillColor), pathColl[1]);
                    Graphics.DrawPath(pen, pathColl[0]);
                    break;
                case AutoShapeType.DoubleBracket:
                    return shapePath.GetDoubleBracketPath();
                case AutoShapeType.DoubleBrace:
                    return shapePath.GetDoubleBracePath();
                case AutoShapeType.LeftBracket:
                    return shapePath.GetLeftBracketPath();
                case AutoShapeType.RightBracket:
                    return shapePath.GetRightBracketPath();
                case AutoShapeType.LeftBrace:
                    return shapePath.GetLeftBracePath();
                case AutoShapeType.RightBrace:
                    return shapePath.GetRightBracePath();
                #endregion
                #region Block Arrows
                case AutoShapeType.RightArrow:
                    return shapePath.GetRightArrowPath();
                case AutoShapeType.LeftArrow:
                    return shapePath.GetLeftArrowPath();
                case AutoShapeType.UpArrow:
                    return shapePath.GetUpArrowPath();
                case AutoShapeType.DownArrow:
                    return shapePath.GetDownArrowPath();
                case AutoShapeType.LeftRightArrow:
                    return shapePath.GetLeftRightArrowPath();
                case AutoShapeType.UpDownArrow:
                    return shapePath.GetUpDownArrowPath();
                case AutoShapeType.QuadArrow:
                    return shapePath.GetQuadArrowPath();
                case AutoShapeType.BentArrow:
                    return shapePath.GetBentArrowPath();
                case AutoShapeType.LeftRightUpArrow:
                    return shapePath.GetLeftRightUpArrowPath();
                case AutoShapeType.UTurnArrow:
                    return shapePath.GetUTrunArrowPath();
                case AutoShapeType.LeftUpArrow:
                    return shapePath.GetLeftUpArrowPath();
                case AutoShapeType.BentUpArrow:
                    return shapePath.GetBentUpArrowPath();
                case AutoShapeType.CurvedRightArrow:
                    return shapePath.GetCurvedRightArrowPath();
                case AutoShapeType.CurvedLeftArrow:
                    return shapePath.GetCurvedLeftArrowPath();
                case AutoShapeType.CurvedDownArrow:
                    return shapePath.GetCurvedDownArrowPath();
                case AutoShapeType.CurvedUpArrow:
                    return shapePath.GetCurvedUpArrowPath();
                case AutoShapeType.StripedRightArrow:
                    return shapePath.GetStripedRightArrowPath();
                case AutoShapeType.NotchedRightArrow:
                    return shapePath.GetNotchedRightArrowPath();
                case AutoShapeType.Pentagon:
                    return shapePath.GetPentagonPath();
                case AutoShapeType.Chevron:
                    return shapePath.GetChevronPath();
                case AutoShapeType.RightArrowCallout:
                    return shapePath.GetRightArrowCalloutPath();
                case AutoShapeType.DownArrowCallout:
                    return shapePath.GetDownArrowCalloutPath();
                case AutoShapeType.LeftArrowCallout:
                    return shapePath.GetLeftArrowCalloutPath();
                case AutoShapeType.UpArrowCallout:
                    return shapePath.GetUpArrowCalloutPath();
                case AutoShapeType.LeftRightArrowCallout:
                    return shapePath.GetLeftRightArrowCalloutPath();
                case AutoShapeType.QuadArrowCallout:
                    return shapePath.GetQuadArrowCalloutPath();
                case AutoShapeType.CircularArrow:
                    return shapePath.GetCircularArrowPath();
                #endregion
                #region Equation Shapes
                case AutoShapeType.MathPlus:
                    return shapePath.GetMathPlusPath();
                case AutoShapeType.MathMinus:
                    return shapePath.GetMathMinusPath();
                case AutoShapeType.MathMultiply:
                    return shapePath.GetMathMultiplyPath();
                case AutoShapeType.MathDivision:
                    return shapePath.GetMathDivisionPath();
                case AutoShapeType.MathEqual:
                    return shapePath.GetMathEqualPath();
                case AutoShapeType.MathNotEqual:
                    return shapePath.GetMathNotEqualPath();
                #endregion
                #region Flowcharts
                case AutoShapeType.FlowChartAlternateProcess:
                    return shapePath.GetFlowChartAlternateProcessPath();
                case AutoShapeType.FlowChartPredefinedProcess:
                    return shapePath.GetFlowChartPredefinedProcessPath();
                case AutoShapeType.FlowChartInternalStorage:
                    return shapePath.GetFlowChartInternalStoragePath();
                case AutoShapeType.FlowChartDocument:
                    return shapePath.GetFlowChartDocumentPath();
                case AutoShapeType.FlowChartMultiDocument:
                    return shapePath.GetFlowChartMultiDocumentPath();
                case AutoShapeType.FlowChartPreparation:
                    return shapePath.GetFlowChartPreparationPath();
                case AutoShapeType.FlowChartManualInput:
                    return shapePath.GetFlowChartManualInputPath();
                case AutoShapeType.FlowChartManualOperation:
                    return shapePath.GetFlowChartManualOperationPath();
                case AutoShapeType.FlowChartConnector:
                    return shapePath.GetFlowChartConnectorPath();
                case AutoShapeType.FlowChartOffPageConnector:
                    return shapePath.GetFlowChartOffPageConnectorPath();
                case AutoShapeType.FlowChartCard:
                    return shapePath.GetFlowChartCardPath();
                case AutoShapeType.FlowChartTerminator:
                    return shapePath.GetFlowChartTerminatorPath();
                case AutoShapeType.FlowChartPunchedTape:
                    return shapePath.GetFlowChartPunchedTapePath();
                case AutoShapeType.FlowChartSummingJunction:
                    return shapePath.GetFlowChartSummingJunctionPath();
                case AutoShapeType.FlowChartOr:
                    return shapePath.GetFlowChartOrPath();
                case AutoShapeType.FlowChartCollate:
                    return shapePath.GetFlowChartCollatePath();
                case AutoShapeType.FlowChartSort:
                    return shapePath.GetFlowChartSortPath();
                case AutoShapeType.FlowChartExtract:
                    return shapePath.GetFlowChartExtractPath();
                case AutoShapeType.FlowChartMerge:
                    return shapePath.GetFlowChartMergePath();
                case AutoShapeType.FlowChartStoredData:
                    return shapePath.GetFlowChartOnlineStoragePath();
                case AutoShapeType.FlowChartDelay:
                    return shapePath.GetFlowChartDelayPath();
                case AutoShapeType.FlowChartSequentialAccessStorage:
                    return shapePath.GetFlowChartSequentialAccessStoragePath();
                case AutoShapeType.FlowChartMagneticDisk:
                    return shapePath.GetFlowChartMagneticDiskPath();
                case AutoShapeType.FlowChartDirectAccessStorage:
                    return shapePath.GetFlowChartDirectAccessStoragePath();
                case AutoShapeType.FlowChartDisplay:
                    return shapePath.GetFlowChartDisplayPath();
                #endregion
                #region Callouts
                case AutoShapeType.RectangularCallout:
                    return shapePath.GetRectangularCalloutPath();
                case AutoShapeType.RoundedRectangularCallout:
                    return shapePath.GetRoundedRectangularCalloutPath();
                case AutoShapeType.OvalCallout:
                    return shapePath.GetOvalCalloutPath();
                case AutoShapeType.CloudCallout:
                    return shapePath.GetCloudCalloutPath();
                case AutoShapeType.LineCallout1:
                case AutoShapeType.LineCallout1NoBorder:
                    return shapePath.GetLineCallout1Path();
                case AutoShapeType.LineCallout2:
                case AutoShapeType.LineCallout2NoBorder:
                    return shapePath.GetLineCallout2Path();
                case AutoShapeType.LineCallout3:
                case AutoShapeType.LineCallout3NoBorder:
                    return shapePath.GetLineCallout3Path();
                case AutoShapeType.LineCallout1AccentBar:
                 case   AutoShapeType.LineCallout1BorderAndAccentBar:
                    return shapePath.GetLineCallout1AccentBarPath();
                case AutoShapeType.LineCallout2AccentBar:
                  case  AutoShapeType.LineCallout2BorderAndAccentBar:
                    return shapePath.GetLineCallout2AccentBarPath();
                case AutoShapeType.LineCallout3AccentBar:
                case AutoShapeType.LineCallout3BorderAndAccentBar:
                    return shapePath.GetLineCallout3AccentBarPath();
                //case AutoShapeType.LineCallout1NoBorder:
                    //return shapePath.GetLineCallout1NoBorderPath();
                #endregion
                #region Stars and Banners
                case AutoShapeType.Explosion1:
                    return shapePath.GetExplosion1();
                    break;
                case AutoShapeType.Explosion2:
                    return shapePath.GetExplosion2();
                    break;
                case AutoShapeType.Star4Point:
                    return shapePath.GetStar4Point();
                    break;
                case AutoShapeType.Star5Point:
                    return shapePath.GetStar5Point();
                    break;
                case AutoShapeType.Star6Point:
                    return shapePath.GetStar6Point();
                    break;
                case AutoShapeType.Star7Point:
                    return shapePath.GetStar7Point();
                    break;
                case AutoShapeType.Star8Point:
                    return shapePath.GetStar8Point();
                    break;
                case AutoShapeType.Star10Point:
                    return shapePath.GetStar10Point();
                    break;
                case AutoShapeType.Star12Point:
                    return shapePath.GetStar12Point();
                    break;
                case AutoShapeType.Star16Point:
                    return shapePath.GetStar16Point();
                    break;
                case AutoShapeType.Star24Point:
                    return shapePath.GetStar24Point();
                    break;
                case AutoShapeType.Star32Point:
                    return shapePath.GetStar32Point();
                    break;
                case AutoShapeType.UpRibbon:
                    return shapePath.GetUpRibbon();
                    break;
                case AutoShapeType.DownRibbon:
                    return shapePath.GetDownRibbon();
                    break;
                case AutoShapeType.CurvedUpRibbon:
                    return shapePath.GetCurvedUpRibbon();
                    break;
                case AutoShapeType.CurvedDownRibbon:
                    return shapePath.GetCurvedDownRibbon();
                    break;
                case AutoShapeType.VerticalScroll:
                    return shapePath.GetVerticalScroll();
                    break;
                case AutoShapeType.HorizontalScroll:
                    pathColl = shapePath.GetHorizontalScroll();
                    shapeFillColor = GetThemeColor(shape.DocxProps, shape.Document, "fillRef");
                    fillColor = !shape.FillFormat.Color.IsEmpty ? shape.FillFormat.Color : shapeFillColor != Color.Empty ? shapeFillColor : Color.Empty;
                    foreach(GraphicsPath drawPath in pathColl)
                    {
                        if (fillColor != Color.Empty)
                            Graphics.FillPath(new SolidBrush(fillColor), drawPath);
                        Graphics.DrawPath(pen, drawPath);
                    }
                    break;
                case AutoShapeType.Wave:
                    return shapePath.GetWave();
                    break;
                case AutoShapeType.DoubleWave:
                    return shapePath.GetDoubleWave();
                    break;
                #endregion

            }
            return path;
        }
        #endregion
        #endregion

        #region Implementation-Measurings
        /// <summary>
        /// Measures the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        internal SizeF MeasureImage(WPicture image)
        {
            float width = (image.Size.Width * image.WidthScale) / 100;
            float height = (image.Size.Height * image.HeightScale) / 100;
            return new SizeF(width, height);
        }

        /// <summary>
        /// Measures the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="layoutArea">The layout area.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        internal SizeF MeasureString(string text, Font font, StringFormat format)
        {
            return MeasureString(text, font, format, null,false);
        }
        /// <summary>
        /// Measures the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="layoutArea">The layout area.</param>
        /// <param name="format">The format.</param>
        /// <param name="charSpacing">The CharacterSpacing</param>
        /// <returns></returns>
        internal SizeF MeasureString(string text, Font font, StringFormat format, WCharacterFormat charFormat, bool isMeasureFromTabList)
        {
            float charSpacing = 0.0f;
            if (charFormat != null)
                charSpacing = charFormat.CharacterSpacing;

            if (text == null || text.Length == 0)
                return SizeF.Empty;

            text = text.Replace(NONBREAK_HYPHEN.ToString(), "-");
            text = text.Replace(SOFT_HYPHEN.ToString(), "-");

            if (format == null)
            {
                format = new StringFormat(StringFormat.GenericTypographic);
                //remove the linelimit flag from create stringformat object.
                //The behavior of the linelimit flag is “only entire lines are laid out in the formatting rectangle”, 
                //By default layout continues until the end of the text, or until no more lines are visible as a result of clipping”.
                format.FormatFlags &= ~StringFormatFlags.LineLimit;
                format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                format.FormatFlags |= StringFormatFlags.NoClip;
                format.Trimming = StringTrimming.Word;
            }
            //Measure the string
            SizeF size = SizeF.Empty;
            //To handle the external exception, checking whether the string length is greater than 32000.
            try
            {
                size = Graphics.MeasureString(text, font, new PointF(0, 0), format);
            }
            catch
            {
                string subText = string.Empty;
                int subTextLength = 0;
                int maxLength = 32000;
                while (text.Length != subTextLength)
                {
                    subText = text.Substring(subTextLength, maxLength);
                    subTextLength += subText.Length;
                    if ((text.Length - subTextLength) > 32000)
                    {
                        maxLength = 32000;
                    }
                    else
                        maxLength = text.Length - subTextLength;
                    size += Graphics.MeasureString(subText, font, new PointF(0, 0), format);
                }
            }
            //Update width based on CharacterSpacing
            if (charSpacing != 0)
                size.Width += (text.Length * charSpacing);
            if (!isMeasureFromTabList && (charFormat == null || (!charFormat.ComplexScript && text.Trim(' ') != "")) && font.Name == "Arial Unicode MS")
                size.Height = GetExceededLineHeightForArialUnicodeMSFont(font);

            return size;
        }
        /// <summary>
        /// Get the Exceeded line height of the Arial unicode MS font.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <returns></returns>
        internal float GetExceededLineHeightForArialUnicodeMSFont(Font font)
        {
            float em_height = font.FontFamily.GetEmHeight(font.Style);
            float fontSize = font.Size;
            float design_to_points = fontSize / em_height;
            float ascent = font.FontFamily.GetCellAscent(font.Style);
            float descent = font.FontFamily.GetCellDescent(font.Style);
            float linespacing = font.FontFamily.GetLineSpacing(font.Style);
            int temp1 = (int)((ascent + descent) - em_height);
            int temp2 = (int)(temp1 / 2);
            int temp3 = (int)(temp1 - temp2);
            int temp4 = (int)((int)(0.3 * (ascent + descent)));
            temp4 = (int)(temp4 - temp1);
            temp1 = (temp4 > 0) ? temp4 : ((int)0);
            ascent = (int)(ascent + temp2);
            descent = (int)(descent + temp3);
            linespacing = (int)((ascent + descent) + temp1);
            return design_to_points * linespacing;
        }
        /// <summary>
        /// Measures the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="layoutArea">The layout area.</param>
        /// <param name="format">The format.</param>
        /// <param name="charSpacing">The CharacterSpacing</param>
        /// <returns></returns>
        internal SizeF MeasureString(string text, Font font,Font defaultFont, StringFormat format, WCharacterFormat charFormat)
        {
            float charSpacing = 0.0f;
            if (charFormat != null)
                charSpacing = charFormat.CharacterSpacing;
            if (text == null || text.Length == 0)
                return SizeF.Empty;

            text = text.Replace(NONBREAK_HYPHEN.ToString(), "-");
            text = text.Replace(SOFT_HYPHEN.ToString(), "-");

            if (format == null)
            {
                format = new StringFormat(StringFormat.GenericTypographic);
                //remove the linelimit flag from create stringformat object.
                //The behavior of the linelimit flag is “only entire lines are laid out in the formatting rectangle”, 
                //By default layout continues until the end of the text, or until no more lines are visible as a result of clipping”.
                format.FormatFlags &= ~StringFormatFlags.LineLimit;
                format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                format.FormatFlags |= StringFormatFlags.NoClip;
                format.Trimming = StringTrimming.Word;
            }
            //Measure the string
            SizeF size = MeasureUnicodeString(text, font, defaultFont, format);
            //Update width based on CharacterSpacing
            if (charSpacing != 0)
                size.Width += (text.Length * charSpacing);
            if ((charFormat == null || (!charFormat.ComplexScript && text.Trim(' ') != "")) && font.Name == "Arial Unicode MS")
            {
                size.Height = GetExceededLineHeightForArialUnicodeMSFont(font);
            }
            return size;
        }
        /// <summary>
        /// Draw Unicode Text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textBrush"></param>
        /// <param name="bounds"></param>
        private SizeF MeasureUnicodeString(String text, Font font, Font defaultFont, StringFormat format)
        {
            char[] ch = text.ToCharArray();
            string normalText = null;
            string unicodeText = null;
            if (font.Name == "Arial" || font.Name == "Times New Roman")
                font = new Font("Arial Unicode MS", font.Size, font.Style);
            float width = 0;
            for (int i = 0; i < ch.Length; i++)
            {
                if (!IsUnicodeText(ch[i].ToString()))
                {
                    normalText += ch[i];
                    if (unicodeText != null)
                    {
                        width += Graphics.MeasureString(unicodeText, font, new PointF(0, 0), format).Width;
                        unicodeText = null;
                    }
                }
                else
                {
                    unicodeText += ch[i];
                    if (normalText != null)
                    {
                        width += Graphics.MeasureString(normalText, defaultFont, new PointF(0, 0), format).Width;
                        normalText = null;
                    }
                }
            }
            if (unicodeText != null)
            {
                width += Graphics.MeasureString(unicodeText, font, new PointF(0, 0), format).Width;
            }
            else if (normalText != null)
            {
                width += Graphics.MeasureString(normalText, defaultFont, new PointF(0, 0), format).Width;
            }
            SizeF size = Graphics.MeasureString(text, font, new PointF(0, 0), format);
            size.Width = width;
            return size;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="txtRange"></param>
        /// <returns></returns>
        internal SizeF MeasureTextRange(WTextRange txtRange, string text)
        {
            WParagraph paragraph = txtRange.OwnerParagraph as WParagraph;
            WCharacterFormat characterFormat = txtRange.CharacterFormat;
            string txt = (txtRange.Text).Trim(SPACE);
            if (txt == string.Empty && paragraph != null && paragraph.Text == txtRange.Text)
            {
                characterFormat = paragraph.BreakCharacterFormat;
            }
            /*if (paragraph != null && paragraph.StyleName != null && paragraph.ParaStyle != null &&
                paragraph.StyleName != string.Empty && paragraph.StyleName != "Normal" &&
                paragraph.StyleName.ToLower() != "header" && paragraph.StyleName.ToLower() != "footer" && paragraph.StyleName.ToLower() != "default")
            {
                //characterFormat = paragraph.GetStyle().CharacterFormat as WCharacterFormat;
            }
            if (txtRange.CharacterFormat.CharStyleName != null && txtRange.CharacterFormat.CharStyleName != "Normal"
                && txtRange.CharacterFormat.CharStyle != null)
            {
                //Style cStyle = txtRange.Document.Styles.FindByName(txtRange.CharacterFormat.CharStyleName) as Style;
                //characterFormat = cStyle.CharacterFormat as WCharacterFormat;
            }*/
            Font font = GetFont(txtRange, characterFormat, text);

            StringFormat format = GetStringFormat(characterFormat);
            if (IsTOC(txtRange))
            {
                format = txtRange.GetOwnerParagraph().ParaStyle != null ? GetStringFormat(txtRange.OwnerParagraph.ParaStyle.CharacterFormat) : format;
            }
            SizeF size;

            if (text != null && characterFormat.AllCaps == true)
            {
                text = text.ToUpper();
            }
            Font defaultFont = GetDefaultFont(font, characterFormat);
            if (font.Name != defaultFont.Name && IsUnicodeText(text))
                size = MeasureString(text, font, defaultFont, format, characterFormat);
            else
                size = MeasureString(text, font, format, characterFormat,false);

            return size;
        }
        /// <summary>
        /// Gets the ascent
        /// </summary>
        /// <param name="font">The Font.</param>
        /// <returns></returns>
        internal float GetAscent(Font font)
        {
            FontMetric fMetric = new FontMetric(font, Graphics);
            return (float)fMetric.Ascent;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the string format.
        /// </summary>
        /// <param name="charFormat">The char format.</param>
        /// <param name="paraFormat">The para format.</param>
        /// <returns></returns>
        private StringFormat GetStringFormat(WCharacterFormat charFormat)
        {
            StringFormat format = new StringFormat(StringFormat.GenericTypographic);
            //remove the linelimit flag from create stringformat object.
            //The behavior of the linelimit flag is “only entire lines are laid out in the formatting rectangle”, 
            //By default layout continues until the end of the text, or until no more lines are visible as a result of clipping”.
            format.FormatFlags &= ~StringFormatFlags.LineLimit;
            format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            format.FormatFlags |= StringFormatFlags.NoClip;

            format.Trimming = StringTrimming.Word;

            if (charFormat.Bidi)
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

            return format;
        }

        /// <summary>
        /// Gets the brush.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private SolidBrush GetBrush(Color color)
        {
            return new SolidBrush(color);
        }

        /// <summary>
        /// Gets the color of the text.
        /// </summary>
        /// <param name="charFormat">The char format.</param>
        /// <returns></returns>
        private Color GetTextColor(WCharacterFormat charFormat)
        {
            Color textColor = Color.Black;
            bool isAutoTextColor = false;
            WParagraph paragraph=currParagraph;
            if (currTextRange != null)
                paragraph = currTextRange.OwnerParagraph;
            if (IsTOC(currTextRange))
            {
                textColor = paragraph.ParaStyle != null ? paragraph.ParaStyle.CharacterFormat.TextColor : Color.Black;
                isAutoTextColor = (textColor == Color.Empty);
            }
            else if (!charFormat.TextColor.IsEmpty)
                textColor = charFormat.TextColor;
            else if (paragraph != null && paragraph.IsInCell
                && (paragraph.OwnerTextBody as WTableCell).OwnerRow.OwnerTable.m_isTextBox
                && !(paragraph.OwnerTextBody as WTableCell).OwnerRow.OwnerTable.m_textBoxFormat.TextThemeColor.IsEmpty)
                    textColor = (paragraph.OwnerTextBody as WTableCell).OwnerRow.OwnerTable.m_textBoxFormat.TextThemeColor;
            else if (paragraph != null && paragraph.OwnerTextBody.Owner is Shape)
            {
                Shape shape = paragraph.OwnerTextBody.Owner as Shape;
                Color textThemeColor = GetThemeColor(shape.DocxProps, shape.Document, "fontRef");
                if (textThemeColor != Color.Empty)
                    textColor = textThemeColor;
                else
                    isAutoTextColor = true;
            }
            else
                isAutoTextColor = true;
            if (isAutoTextColor && paragraph != null)
            {
                textColor = Color.Black;
                if (paragraph.IsInCell)
                {
                    CellFormat cellFormat = (paragraph.OwnerTextBody as WTableCell).CellFormat;
                    if (!cellFormat.BackColor.IsEmpty && m_backgroundColorCheckList.Contains(cellFormat.BackColor))
                        textColor = Color.FromArgb(255, 255, 255, 255);
                    if (cellFormat.TextureStyle != TextureStyle.TextureNone
                        && !cellFormat.TextureStyle.ToString().Contains("Percent")
                        && !cellFormat.ForeColor.IsEmpty
                        && m_backgroundColorCheckList.Contains(cellFormat.ForeColor))
                        textColor = Color.FromArgb(255, 255, 255, 255);
                }
                if (!paragraph.ParagraphFormat.BackColor.IsEmpty && m_backgroundColorCheckList.Contains(paragraph.ParagraphFormat.BackColor))
                    textColor = Color.FromArgb(255, 255, 255, 255);
                if (!charFormat.TextBackgroundColor.IsEmpty)
                {
                    if (m_backgroundColorCheckList.Contains(charFormat.TextBackgroundColor))
                        textColor = Color.FromArgb(255, 255, 255, 255);
                    else
                        textColor = Color.Black;
                }
            }
            return textColor;
        }
        internal Font GetFont(WTextRange txtRange, WCharacterFormat charFormat,string text)
        {
            Font font = null;
            string fontName = null;
            float fontSize = 0.0f;
            FontStyle fontStyle = FontStyle.Regular;
            if (txtRange == null)
            {
                return GetFont(charFormat, text);
            }
            fontName = txtRange.CharacterFormat.FontName;
            fontSize = UpdateFontSize(charFormat);
            WParagraph paragraph = txtRange.OwnerParagraph as WParagraph;
            if (paragraph == null)
            {
                return GetFont(charFormat, text);
            }
            if (txtRange != null && txtRange.CharacterFormat != null)
            {
                string temp = txtRange.CharacterFormat.CharStyleName;
                if (!(temp == "Hyperlink" && IsTOC(txtRange)))
                {
                    fontStyle = txtRange.CharacterFormat.Font.Style;
                }
                if (IsTOC(txtRange))
                    fontStyle = txtRange.OwnerParagraph.ParaStyle != null ? txtRange.OwnerParagraph.ParaStyle.CharacterFormat.Font.Style : txtRange.CharacterFormat.Font.Style;
            }
            //Updates fontstyle underline information based on subsequent text ranges present in the paragraph.
            if (txtRange.Text.Trim(SPACE) == string.Empty
                && ((txtRange as IWidget).LayoutInfo as TabsLayoutInfo) == null
                && (charFormat.UnderlineStyle != UnderlineStyle.None
                || charFormat.Font.Strikeout))
            {
                if (txtRange.NextSibling == null)
                {
                    fontStyle &= ~FontStyle.Strikeout;
                    fontStyle &= ~FontStyle.Underline;
                }
                else if (txtRange.NextSibling != null)
                {
                    Entity ent = txtRange.NextSibling as Entity;
                    while (ent is WTextRange && (ent as WTextRange).Text.Trim(SPACE) == string.Empty
                        && ((ent as WTextRange).m_layoutInfo as TabsLayoutInfo) == null)
                    {
                        ent = ent.NextSibling as Entity;
                        if (ent == null)
                            break;
                    }
                    if (!(ent != null && ent is WTextRange
                        && ((ent as WTextRange).Text.Trim(SPACE) != string.Empty
                        || ((ent as WTextRange).m_layoutInfo as TabsLayoutInfo) != null)))
                    {
                        fontStyle &= ~FontStyle.Strikeout;
                        fontStyle &= ~FontStyle.Underline;
                    }
                }
            }
            if (charFormat.HasValue(WCharacterFormat.IdctHintKey) && IsUnicodeText(text))
            {
                fontName = charFormat.GetFontNameFromHint();
            }
            //By Default MSWord can replace "Times New Roman" font instead of "Times New Roman Bold"
            if (fontName == "Times New Roman Bold")
                fontName = "Times New Roman";
            if (fontSize == 0)
                fontSize = 0.5f;
            //If misspelled "ArialUnicodeMS" font exist means apply "Arial" font
            if (fontName == "ArialUnicodeMS")
                font = new Font("Arial", fontSize, fontStyle);
            else
            {
                font = new Font(fontName, fontSize, fontStyle);
                UpdateAlternateFont(charFormat, fontName, ref font);
            }


            if (txtRange.CharacterFormat.Bidi == true && txtRange.CharacterFormat.FontNameBidi != null &&
               txtRange.CharacterFormat.FontNameBidi != string.Empty &&
               txtRange.CharacterFormat.FontName != txtRange.CharacterFormat.FontNameBidi)
            {
                return new Font(txtRange.CharacterFormat.FontNameBidi, fontSize, txtRange.CharacterFormat.Font.Style);
            }


            return font;

        }
        /// <summary>
        /// Updates the alternate font for the font not installed in the system.
        /// </summary>
        /// <param name="charFormat">The char format.</param>
        /// <param name="fontName">Name of the font.</param>
        /// <param name="font">The font.</param>
        private void UpdateAlternateFont(WCharacterFormat charFormat, string fontName, ref Font font)
        {
            if (charFormat.Document != null
                && charFormat.Document.FontSubstitutionTable.ContainsKey(fontName)
                && fontName != font.Name)
                font = new Font(charFormat.Document.FontSubstitutionTable[fontName], font.Size, font.Style);
        }
        /// <summary>
        /// Checks the owner paragraph of the textrange is a TOC
        /// </summary>
        /// <param name="txtRange">Text Range</param>
        /// <returns></returns>
        internal bool IsTOC(WTextRange txtRange)
        {
            if (txtRange != null && txtRange.OwnerParagraph != null && txtRange.OwnerParagraph.ChildEntities.FirstItem != null && ((txtRange.OwnerParagraph.ChildEntities.FirstItem is TableOfContent) || ((txtRange.OwnerParagraph.ChildEntities.FirstItem is WField) && (txtRange.OwnerParagraph.ChildEntities.FirstItem as WField).FieldType == FieldType.FieldHyperlink) && (new Hyperlink(txtRange.OwnerParagraph.ChildEntities.FirstItem as WField).BookmarkName != null && new Hyperlink(txtRange.OwnerParagraph.ChildEntities.FirstItem as WField).BookmarkName.StartsWith("_Toc"))))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Updates Font Size
        /// </summary>
        /// <param name="txtRange">Text Range</param>
        /// <param name="charFormat">Character format</param>
        /// <param name="bounds">Bounds</param>
        /// <returns></returns>
        internal float UpdateFontSize(WCharacterFormat charFormat)
        {
            float fontSize = charFormat.Font.SizeInPoints;
            if (charFormat.SubSuperScript != SubSuperScript.None)
            {
                int fontHeight = charFormat.Font.Height;
                switch (charFormat.SubSuperScript)
                {
                    case SubSuperScript.SubScript:
                        double ascent = GetAscent(charFormat.Font);
                        fontSize /= DEF_SCRIPT_FACTOR;
                        break;
                    case SubSuperScript.SuperScript:
                        fontSize /= DEF_SCRIPT_FACTOR;
                        break;
                }
            }
            return fontSize;
        }
        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <param name="charFormat">The char format.</param>
        /// <returns></returns>
        internal Font GetFont(WCharacterFormat charFormat,string text)
        {
            Font font = null;
            string fontName = charFormat.FontName;
            float fontSize = UpdateFontSize(charFormat);
            FontStyle fontStyle = charFormat.Font.Style;
            if (charFormat.CharStyleName != null &&
                charFormat.CharStyleName.ToLower().Equals("hyperlink")
                && !(currTextRange != null && IsTOC(currTextRange)) && !charFormat.HasKey(WCharacterFormat.UnderlineKey))
            {
                fontStyle |= FontStyle.Underline;
            }
            //Set font style for List number
            if (IsListCharacter && currParagraph != null
                && currParagraph.ListFormat.ListType == ListType.Numbered
                && currParagraph.ListFormat.CurrentListLevel != null)
            {
                if (currParagraph.ListFormat.CurrentListLevel.CharacterFormat.Font.Underline)
                {
                    if ((fontStyle & FontStyle.Underline) == 0)
                        fontStyle |= FontStyle.Underline;
                }
                else
                {
                    if ((fontStyle & FontStyle.Underline) != 0)
                        fontStyle &= ~FontStyle.Underline;
                }
            }
            if (charFormat.HasValue(WCharacterFormat.IdctHintKey) && IsUnicodeText(text))
            {
                fontName = charFormat.GetFontNameFromHint();
            }
            //By Default MSWord can replace "Times New Roman" font instead of "Times New Roman Bold"
            if (fontName == "Times New Roman Bold")
                fontName = "Times New Roman";
            if (fontSize == 0)
                fontSize = 0.5f;
            //If misspelled "ArialUnicodeMS" font exist means apply "Arial" font
            if (fontName == "ArialUnicodeMS")
                font = new Font("Arial", fontSize, fontStyle);
            else
            {
                font = new Font(fontName, fontSize, fontStyle);
                UpdateAlternateFont(charFormat, fontName, ref font);
            }


            if (charFormat.Bidi == true && charFormat.FontNameBidi != null &&
               charFormat.FontNameBidi != string.Empty &&
               charFormat.FontName != charFormat.FontNameBidi)
            {
                return new Font(charFormat.FontNameBidi, fontSize, charFormat.Font.Style);
            }


            return font;
        }

        /// <summary>
        /// Gets the string alignment.
        /// </summary>
        /// <param name="paraFormat">The para format.</param>
        /// <returns></returns>
        private StringAlignment GetStringAlignment(WParagraphFormat paraFormat)
        {
            StringAlignment alignment = StringAlignment.Near;

            if (paraFormat.HorizontalAlignment == Syncfusion.DocIO.DLS.HorizontalAlignment.Right)
                alignment = StringAlignment.Far;
            else if (paraFormat.HorizontalAlignment == Syncfusion.DocIO.DLS.HorizontalAlignment.Center)
                alignment = StringAlignment.Center;

            return alignment;
        }

        /// <summary>
        /// Gets the pen.
        /// </summary>
        /// <param name="border">The border.</param>
        /// <returns></returns>
        private Pen GetPen(Border border)
        {
            Pen pen = null;
            float lineWidth = border.LineWidth;

            Color penColor = Color.Black;

            if (!border.Color.IsEmpty && border.Color.ToArgb() != 0)
            {
                penColor = border.Color;
            }

            pen = new Pen(penColor, border.LineWidth);

            switch (border.BorderType)
            {
                case BorderStyle.DashLargeGap:
                    pen.DashPattern = new float[] { 3f, 5f };
                    break;

                case BorderStyle.DashSmallGap:
                    pen.DashPattern = new float[] { 3f, 3f };
                    break;
                case BorderStyle.Dot:
                    pen.DashPattern = new float[] { 1f, 2f };
                    break;

                case BorderStyle.DotDash:
                    pen.DashPattern = new float[] { 4f, 3f, 1f, 3f };
                    break;

                case BorderStyle.DotDotDash:
                    pen.DashPattern = new float[] { 8f, 3f, 1f, 3f, 1f, 3f };
                    break;

                case BorderStyle.Double:
                    pen.Width = (lineWidth * 3f);
                    pen.CompoundArray = new float[] { 0f, 0.3333333f, 0.6666667f, 1f };
                    break;

                case BorderStyle.Triple:
                    pen.Width = lineWidth * 5f;
                    pen.CompoundArray = new float[] { 0f, 0.1666667f, 0.3333333f, 0.6666667f, 0.8333333f, 1f };
                    break;

                case BorderStyle.Inset:
                    pen.Alignment = PenAlignment.Inset;
                    break;

                case BorderStyle.Outset:
                    pen.Alignment = PenAlignment.Outset;
                    break;

                case BorderStyle.Emboss3D:
                case BorderStyle.Engrave3D:
                case BorderStyle.Hairline:
                case BorderStyle.Single:
                case BorderStyle.Thick:
                case BorderStyle.ThickThickThinMediumGap:
                case BorderStyle.ThickThinLargeGap:
                case BorderStyle.ThickThinMediumGap:
                case BorderStyle.ThinThickLargeGap:
                case BorderStyle.ThinThickMediumGap:
                case BorderStyle.ThinThickSmallGap:
                case BorderStyle.ThinThickThinLargeGap:
                case BorderStyle.ThinThickThinSmallGap:
                case BorderStyle.ThinThinSmallGap:
                case BorderStyle.TwistedLines1:
                case BorderStyle.Wave:
                case BorderStyle.DoubleWave:
                default:
                    break;
            }

            pen.StartCap = LineCap.Square;
            pen.EndCap = LineCap.Square;

            return pen;
        }

        /// <summary>
        /// Scales the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        private Image ScaleImage(Image srcImage, float width, float height)
        {
            if (srcImage.Width <= width && srcImage.Height <= height)
                return srcImage;

            int sourceWidth = srcImage.Width;
            int sourceHeight = srcImage.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = (int)(width);
            int destHeight = (int)(height);

            if (destWidth <= 0)
                destWidth = 1;

            if (destHeight <= 0)
                destHeight = 1;

            Bitmap destImage = new Bitmap(destWidth, destHeight);
            destImage.SetResolution(srcImage.HorizontalResolution,
                                    srcImage.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(srcImage,
                    new System.Drawing.Rectangle(destX, destY, destWidth, destHeight),
                    new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);

                graphics.Dispose();
            }
            return destImage;
        }

        /// <summary>
        /// Adds the hyper link to collection.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="bounds">The bounds.</param>
        private void AddHyperLink(Hyperlink hyperlink, RectangleF bounds)
        {
            string text = "";

            switch (hyperlink.Type)
            {
                case HyperlinkType.FileLink:
                    text = hyperlink.FilePath;
                    break;
                case HyperlinkType.WebLink:
                case HyperlinkType.EMailLink:
                    text = hyperlink.Uri;
                    break;
                case HyperlinkType.Bookmark:
                    Dictionary<string, BookmarkHyperlink> bookmarkHyperlinkDic = new Dictionary<string, BookmarkHyperlink>();
                    BookmarkHyperlink bmhyperlink = new BookmarkHyperlink();
                    bmhyperlink.HyperlinkValue = hyperlink.Field.FieldValue.Replace("\"", string.Empty);
                    if (hyperlink.BookmarkName.ToLower().StartsWith("_toc"))
                        bmhyperlink.SourceBounds = CurrParagraphBounds;
                    else
                        bmhyperlink.SourceBounds = bounds;
                    bmhyperlink.SourcePageNumber = Syncfusion.DocIO.DLS.Rendering.DocumentLayouter.PageNumber;
                    bookmarkHyperlinkDic.Add(bmhyperlink.HyperlinkValue, bmhyperlink);
                    BookmarkHyperlinksList.Add(bookmarkHyperlinkDic);
                    return;
                default:
                    break;
            }

            Dictionary<string, RectangleF> hyperlinkDic = new Dictionary<string, RectangleF>();
            hyperlinkDic.Add(text, bounds);
            m_hyperLinks.Add(hyperlinkDic);
        }
        /// <summary>
        /// Updates the TOC level.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="bookmark">The bookmark.</param>
        private void UpdateTOCLevel(WParagraph paragraph, BookmarkHyperlink bookmark)
        {
            if (bookmark.HyperlinkValue != null
                && bookmark.HyperlinkValue.StartsWith("_Toc"))
            {
                string styleName = paragraph.StyleName;
                if (styleName != null)
                    styleName = styleName.ToLower().Replace(" ", "");
                else
                    styleName = "normal";
                paragraph.Document.TOC.UpdateTOCStyleLevels();
                int level = 1;
                foreach (KeyValuePair<int, List<string>> tocLevel in paragraph.Document.TOC.TOCLevels)
                {
                    foreach (string paragraphStyleName in tocLevel.Value)
                    {
                        if (styleName == paragraphStyleName.ToLower().Replace(" ", ""))
                        {
                            bookmark.TOCLevel = level;
                            bookmark.TOCText = paragraph.Text;
                            break;
                        }
                    }
                    level++;
                }
            }
        }

        internal int GetSplitIndexByOffset(string text, ITextMeasurable measurer, double offset, bool bSplitByChar, bool bIsInCell, float clientWidth, float clientActiveAreaWidth)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (measurer == null)
                throw new ArgumentNullException("strWidget");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", offset, "Value can not be less 0");

            int resIndex = -1;
            //If client width zero assign offset
            if (clientWidth == 0.0f)
                clientWidth = (float)offset;

            if (text.Length != 0 && (offset > 0 || clientWidth < 0))
            {
                /* Algorithm:
                      1. Find WORD which consist OFFSET
                      2. If WORD not fit find character that fit to OFFSET
                    */
                int wordStartIndex = 0;
                int wordLength = 0;
                double prevWordsWidth = 0f;

                // Measuring by words
                while ((wordLength = GetWordLength(text, wordStartIndex)) > -1)
                {
                    SizeF size = measurer.Measure(text.Substring(wordStartIndex, wordLength));

                    if (size.Width + prevWordsWidth > offset)
                    {
                        wordStartIndex = 0;
                        break;
                    }

                    prevWordsWidth += size.Width;
                    wordStartIndex += wordLength;
                }

                resIndex = wordStartIndex - 1;
                resIndex = UpdateResIndex(text, measurer, resIndex, bSplitByChar, bIsInCell, offset, clientWidth, clientActiveAreaWidth);
            }
            return resIndex;
        }
        /// <summary>
        /// Updates Result index
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="measurer">the measurer</param>
        /// <param name="wordLength">Word length</param>
        /// <param name="resIndex">res index</param>
        /// <param name="bSplitByChar">bSplitByChar</param>
        /// <param name="bIsInCell">bIsInCell</param>
        /// <param name="offset">offset</param>
        /// <param name="clientWidth">Client Width</param>
        /// <returns>res Index</returns>
        internal int UpdateResIndex(string text, ITextMeasurable measurer, int resIndex, bool bSplitByChar, bool bIsInCell, double offset, float clientWidth, float clientActiveAreaWidth)
        {
            // If nothing word fit in specified offset
            if ((resIndex < 0 || bSplitByChar || bIsInCell))
            {
                for (int i = 0; i < text.Length; i++)
                {
                    SizeF size = measurer.Measure(text.Substring(0, i + 1));

                    if (size.Width > offset)
                    {
                        resIndex = i - 1;
                        if (resIndex == -1 && bIsInCell)
                        {
                            float width = GetCellWidth(measurer as WTextRange);
                            if (size.Width > width)
                                resIndex = 0;
                        }
                        if (clientWidth < 0)
                            resIndex = 0;
                        break;
                    }
                }
            }
            string[] st = text.Split(' ');
            if (IsUnicodeText(text))
            {
                //Update ResIndex for unicode text
                if (resIndex > -1)
                {
                    if ((text.Length > resIndex + 1 && IsBeginCharacter(text[resIndex + 1]))
                        || IsLeadingCharacter(text[resIndex]))
                    {
                        resIndex -= 1;
                    }
                    if (text.Length > resIndex + 1 && IsOverFlowCharacter(text[resIndex + 1]))
                    {
                        resIndex += 1;
                    }
                }
            }
            else if (st.Length >= 1)
            {
                SizeF size = measurer.Measure(st[0]);
                if (Math.Round(clientActiveAreaWidth, 2) == Math.Round(offset, 2)
                    && !IsTextRangeNeedToBeSplitted(measurer as WTextRange, size, clientWidth)
                    && (!(size.Width > clientWidth)
                    || Math.Round(clientWidth, 2) > Math.Round(offset, 2))
                    && (size.Width + GetPreviousTextRangeWidth(measurer as WTextRange) < clientWidth))
                {
                    resIndex = -1;
                }
            }
            if (resIndex < 0)
            {
                resIndex = text.Length - 1;
            }

            return resIndex;
        }
        /// <summary>
        /// Determine whether the character is CJK leading character
        /// A line of text cannot end with any leading characters, which are listed below
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal bool IsLeadingCharacter(char c)
        {
            switch (c)
            {
                case '\u0028':
                case '\u0024':
                case '\u005B':
                case '\u007B':
                case '\u300C':
                case '\u300E':
                case '\u0027':
                case '\u0022':
                case '\uff09':
                case '\u3014':
                case '\uff4B':
                case '\uff5A':
                case '\u3008':
                case '\u300A':
                case '\u3010':
                case '\u009D':
                case '\uff04':
                case '\uffe1':
                case '\uffe5':
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Determine whether the character is Begin CJK character
        /// A line of text cannot begin with any following characters
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal bool IsBeginCharacter(char c)
        {
            switch (c)
            {
                case '\u0021':
                case '\u0025':
                case '\u0029':
                case '\u002C':
                case '\u002E':
                case '\u003F':
                case '\u005D':
                case '\uff3f':
                case '\u301f':
                case '\u306D':
                case '\u3064':
                case '\uff09':
                case '\u007D':
                case '\u302a':
                case '\u300D':
                case '\u3001':
                case '\uff0e':
                case '\u30a4':
                case '\u30a8':
                case '\u3015':
                case '\u3084':
                case '\uff3D':
                case '\u3086':
                case '\uff5d':
                case '\u306f':
                case '\u3009':
                case '\u300b':
                case '\u30fc':
                case '\u30af':
                case '\u30a2':
                case '\u3046':
                case '\u3048':
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Determine whether the character is CJK overflow character
        /// Overflow characters are allowed to render in the same line when it doesn't have required client width to fit the character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal bool IsOverFlowCharacter(char c)
        {
            switch (c)
            {
                case '\u002C':
                case '\u002E':
                case '\u0060':
                case '\uff61':
                case '\uff0e':
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal bool IsUnicodeText(string text)
        {
            bool isUnicode = false;
            if (text != null)
            {
                char[] ch = text.ToCharArray();
                for (int i = 0; i < ch.Length; i++)
                {
                    char temp = ch[i];
                    if ((temp >= '\u3000' && temp <= '\u30ff') // Japanese characters
                        || (temp >= '\uff00' && temp <= '\uffef') // Full-width roman characters and half-width katakana
                        || (temp >= '\u4e00' && temp <= '\u9faf') //CJK unifed ideographs - Common and uncommon kanji
                        || (temp >= '\u3400' && temp <= '\u4dbf') //CJK unified ideographs Extension A - Rare kanji
                        || (temp >= '\uac00' && temp <= '\uffef')) //Korean Hangul characters
                    {
                        isUnicode = true;
                        break;
                    }
                }
            }
            return isUnicode;
        }
        /// <summary>
        /// Get Previous TextRanges Width
        /// </summary>
        /// <returns></returns>
        internal float GetPreviousTextRangeWidth(WTextRange textRange)
        {
            SizeF sizePrev = new SizeF();
            WTextRange prevSiblingTextRange = GetPreviousSibling(textRange) as WTextRange;
            if (prevSiblingTextRange != null
                && prevSiblingTextRange is WTextRange
                && !(prevSiblingTextRange.Text.EndsWith(" ")
                || ((prevSiblingTextRange as IWidget).LayoutInfo as TabsLayoutInfo) != null))
            {
                string[] st = prevSiblingTextRange.Text.Split(' ');
                string prevSiblingText = st[st.Length - 1];
                if (prevSiblingText == prevSiblingTextRange.Text)
                    sizePrev = (prevSiblingTextRange as IWidget).LayoutInfo.Size;
                else
                    sizePrev = MeasureTextRange(prevSiblingTextRange, prevSiblingText);
                prevSiblingTextRange = GetPreviousSibling(prevSiblingTextRange) as WTextRange;
                while (prevSiblingTextRange != null && prevSiblingTextRange is WTextRange && st.Length == 1)
                {
                    if (((prevSiblingTextRange as IWidget).LayoutInfo as TabsLayoutInfo) != null)
                        break;
                    else if (prevSiblingTextRange.Text.Contains(" "))
                    {
                        st = prevSiblingTextRange.Text.Split(' ');
                        prevSiblingText = st[st.Length - 1];
                        if (prevSiblingText == prevSiblingTextRange.Text)
                            sizePrev += (prevSiblingTextRange as IWidget).LayoutInfo.Size;
                        else
                            sizePrev += MeasureTextRange(prevSiblingTextRange, prevSiblingText);
                        break;
                    }
                    else
                    {
                        sizePrev += (prevSiblingTextRange as IWidget).LayoutInfo.Size;
                    }
                    prevSiblingTextRange = GetPreviousSibling(prevSiblingTextRange) as WTextRange;
                }
            }
            return sizePrev.Width;
        }
        /// <summary>
        /// Get previous text range
        /// </summary>
        /// <returns></returns>
       internal Entity GetPreviousSibling(WTextRange textRange)
       {
           Entity previousSibling = textRange.PreviousSibling as Entity;
           while (previousSibling != null && !(previousSibling is WTextRange))
           {
               if (previousSibling is BookmarkStart || previousSibling is BookmarkEnd || previousSibling is WFieldMark)
                   previousSibling = previousSibling.PreviousSibling as Entity;
               else
                   return previousSibling;

           }
           return previousSibling;
       }
        /// <summary>
        /// Determine whether the text range need to be splitted
        /// </summary>
        /// <param name="textRange"></param>
        /// <param name="size"></param>
        /// <param name="clientWidth"></param>
        /// <returns></returns>
        private bool IsTextRangeNeedToBeSplitted(WTextRange textRange, SizeF size, float clientWidth)
        {
            return (textRange.PreviousSibling != null
                && (textRange.PreviousSibling is WTextRange)
                && ((textRange.PreviousSibling as WTextRange).m_layoutInfo as TabsLayoutInfo) != null
                && ((textRange.PreviousSibling as WTextRange).m_layoutInfo as TabsLayoutInfo).CurrTabJustification == Syncfusion.Layouting.TabJustification.Left
                && ((textRange.PreviousSibling as WTextRange).m_layoutInfo as TabsLayoutInfo).TabWidth + size.Width > clientWidth);
        }
        /// <summary>
        /// Get cell Width
        /// </summary>
        /// <param name="textRange">The TextRange</param>
        /// <returns></returns>
        internal float GetCellWidth(ParagraphItem paraItem)
        {
            //Get Owner paragraph of the paragraph item.
            WParagraph ownerParagraph = paraItem.Owner as WParagraph;
            if (ownerParagraph == null)
            {
                if (paraItem.Owner is SDTInlineContent)
                    ownerParagraph = (paraItem as ParagraphItem).GetOwnerParagraph();
                else if (paraItem is WTextRange
                    && (paraItem as WTextRange).ParaItemCharFormat.BaseFormat.OwnerBase is WParagraph)
                    ownerParagraph = (paraItem as WTextRange).ParaItemCharFormat.BaseFormat.OwnerBase as WParagraph;
            }
            WTableCell tableCell = ownerParagraph.OwnerTextBody as WTableCell;
            float cellSpacing = 0;
            if (tableCell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                cellSpacing = (float)Math.Round(tableCell.OwnerRow.OwnerTable.TableFormat.CellSpacing, 2) * 2;
            ILayoutSpacingsInfo spacingInfo = (tableCell as IWidget).LayoutInfo as ILayoutSpacingsInfo;
            float leftPadding = (float)Math.Round((spacingInfo.Paddings.Left + spacingInfo.Margins.Left - cellSpacing), 2);
            float rightPadding = (float)Math.Round((spacingInfo.Paddings.Right + spacingInfo.Margins.Right - cellSpacing), 2);
            float topPad = (float)Math.Round((spacingInfo.Paddings.Top + spacingInfo.Margins.Top - cellSpacing), 2);
            float bottomPad = (float)Math.Round((spacingInfo.Paddings.Bottom + spacingInfo.Margins.Bottom - cellSpacing), 2);
            float width = (float)((tableCell as IWidget).LayoutInfo as TableLayoutInfo).CellWidth - leftPadding - rightPadding - cellSpacing;
            if (tableCell.CellFormat.TextDirection != TextDirection.Horizontal && (paraItem is WTextRange))
            {
                width = (float)((tableCell as IWidget).LayoutInfo as TableLayoutInfo).VerticalCellWidth - topPad - bottomPad - cellSpacing;
            }
            ParagraphLayoutInfo paragraphInfo = (ownerParagraph as IWidget).LayoutInfo as ParagraphLayoutInfo;
            if (paragraphInfo.IsFirstLine)
                width = width - (float)(paragraphInfo.Margins.Left + paragraphInfo.Margins.Right + paragraphInfo.FirstLineIndent + paragraphInfo.ListTab);
            else
                width = width - (float)(paragraphInfo.Margins.Left + paragraphInfo.Margins.Right + paragraphInfo.ListTab);
            if (width < 0)
                width = 0;
            return width;
        }
        /// <summary>
        /// Determines whether the text is unicode
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>
        /// 	<c>true</c> if text is unicode, set to <c>true</c>.
        /// </returns>
        internal bool IsUnicode(string text)
        {
            if (text == null)
                return false;
            char[] ch = text.ToCharArray();
            bool isUnicode = false;
            for (int i = 0; i < ch.Length; i++)
            {
                int temp = (int)ch[i];
                if ((int)ch[i] > 255)
                {
                    isUnicode = true;
                    break;
                }
            }
            return isUnicode;
        }
        /// <summary>
        /// Gets length of WORD.
        /// NOTE:
        /// - WORD: text run that finished by last space letter 
        ///   (sample: "text  " or "  "; wrong sample: " text" or "text  text" )
        /// EXCLUSION:
        /// - If text working part have zero symbols return (-1)
        /// - If text working part don't consist SPACE letters return legth of 
        ///   text working part
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex">Index of word first letter</param>
        /// <returns>Length of found word</returns>
        private int GetWordLength(string text, int startIndex)
        {
            const char cSPACE = ' ';
            int resLength = -1;
            int wordMaxLength = text.Length - startIndex;

            if (wordMaxLength > 0)
            {
                for (int i = startIndex, len = text.Length; i < len; i++)
                {
                    if (i == len - 1) // i == last letter
                    {
                        resLength = 1;
                        //resLength = wordMaxLength;
                        break;
                    }
                    else if (text[i] == cSPACE && text[i + 1] != cSPACE)
                    {
                        resLength = i - startIndex + 1;
                        break;
                    }
                }
            }

            return resLength;
        }
        #endregion

        #region Class internal declaration
        internal class DefaultBorders
          : RowFormat
        {
            #region Class properties
            /// <summary>
            /// Gets the vertical border
            /// </summary>
            public Border Vertical
            {
                get
                {
                    return Borders.Vertical;
                }
            }
            /// <summary>
            /// Gets / sets the horizontal border
            /// </summary>
            public Border Horizontal
            {
                get
                {
                    return Borders.Horizontal;
                }
            }
            #endregion

            #region Class Initialise / Finalise
            /// <summary>
            /// 
            /// </summary>
            /// <param name="format"></param>
            public DefaultBorders(RowFormat format)
            {
                InitBorder(base.Borders.Horizontal, format.Borders.Horizontal);
                InitBorder(base.Borders.Vertical, format.Borders.Vertical);
            }
            #endregion

            #region Class helper methods
            /// <summary>
            /// 
            /// </summary>
            /// <param name="destination"></param>
            /// <param name="sourse"></param>
            private void InitBorder(Border destination, Border sourse)
            {
                destination.BorderType = sourse.BorderType;
                destination.Color = sourse.Color;
                destination.LineWidth = sourse.LineWidth;
                destination.Shadow = sourse.Shadow;
                destination.Space = sourse.Space;
                destination.HasNoneStyle = sourse.HasNoneStyle;
            }

            #endregion
        }

        #endregion
    }
    internal class BookmarkHyperlink
    {
        # region Fields
        private RectangleF m_sourceBounds;
        private RectangleF m_targetBounds;
        private int m_targetPageNumber;
        private int m_sourcePageNumber;
        private string m_hyperlinkValue;
        private int m_tocLevel;
        private string m_tocText;
        # endregion

        # region Properties
        public RectangleF SourceBounds
        {
            get
            {
                return m_sourceBounds;
            }
            set
            {
                m_sourceBounds = value;
            }
        }
        public RectangleF TargetBounds
        {
            get
            {
                return m_targetBounds;
            }
            set
            {
                m_targetBounds = value;
            }
        }
        public int TargetPageNumber
        {
            get
            {
                return m_targetPageNumber;
            }
            set
            {
                m_targetPageNumber = value;
            }
        }
        public int SourcePageNumber
        {
            get
            {
                return m_sourcePageNumber;
            }
            set
            {
                m_sourcePageNumber = value;
            }
        }
        public string HyperlinkValue
        {
            get
            {
                return m_hyperlinkValue;
            }
            set
            {
                m_hyperlinkValue = value;
            }
        }
        public int TOCLevel
        {
            get
            {
                return m_tocLevel;
            }
            set
            {
                m_tocLevel = value;
            }
        }
        public string TOCText
        {
            get
            {
                return m_tocText;
            }
            set
            {
                m_tocText = value;
            }
        }
        public BookmarkHyperlink()
        {
            SourceBounds = new RectangleF();
            TargetBounds = new RectangleF();
            TargetPageNumber = 0;
            SourcePageNumber = 0;
            TOCLevel = 0;
            TOCText = string.Empty;
            HyperlinkValue = string.Empty;
        }
        # endregion
    }
    internal class BookmarkPosition
    {
        # region Fields
        private RectangleF m_bounds;
        private int m_pageNumber;
        private string m_bookmarkName;
        # endregion

        # region Properties
        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
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
        /// Gets or sets the page number.
        /// </summary>
        /// <value>The page number.</value>
        public int PageNumber
        {
            get
            {
                return m_pageNumber;
            }
            set
            {
                m_pageNumber = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the bookmark.
        /// </summary>
        /// <value>The name of the bookmark.</value>
        public string BookmarkName
        {
            get
            {
                return m_bookmarkName;
            }
            set
            {
                m_bookmarkName = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkPosition"/> class.
        /// </summary>
        /// <param name="bookmarkName">Name of the bookmark.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="bounds">The bounds.</param>
        public BookmarkPosition(string bookmarkName, int pageNumber, RectangleF bounds)
        {
            BookmarkName = bookmarkName;
            PageNumber = pageNumber;
            Bounds = bounds;
        }
        # endregion
    }
}
#endif
