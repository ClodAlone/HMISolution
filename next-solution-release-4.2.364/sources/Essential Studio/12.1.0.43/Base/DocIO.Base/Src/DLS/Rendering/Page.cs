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
using System;
using System.Drawing;
using System.Collections;
using Syncfusion.Layouting;

using Syncfusion.DocIO.DLS;
using System.Collections.Generic;
using Syncfusion.DocIO.Rendering;

#endregion

namespace Syncfusion.DocIO.DLS.Rendering
{
    /// <summary>
    /// Represents a page.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class Page
    {
        #region Fields
        private LayoutedWidgetList m_pageWidgets = new LayoutedWidgetList();
        private LayoutedWidgetList m_footnoteWidgets = new LayoutedWidgetList();
        private LayoutedWidgetList m_endnoteWidgets = new LayoutedWidgetList();
        private List<int> m_endNotesectionIndex = new List<int>();
        private List<int> m_footNotesectionIndex = new List<int>();
        private WPageSetup m_pageSetup;
        private WHeadersFooters m_headersFooters;
        private IWSection m_docSection;
        private int m_iNumber;
        private Image m_backgroundImage = null;
        private Color m_backgroundColor = Color.Empty;
        private List<IWField> m_cachedFields = new List<IWField>();
        #endregion

        #region Properties
        /// <summary>
        /// Collection of lay outed widget.
        /// </summary>
        /// <value>The page widgets.</value>
        public LayoutedWidgetList PageWidgets
        {
            get
            {
                return m_pageWidgets;
            }
        }

        /// <summary>
        /// Collection of Footnote lay outed widget.
        /// </summary>
        /// <value>The Footnote widgets.</value>
        internal LayoutedWidgetList FootnoteWidgets
        {
            get
            {
                return m_footnoteWidgets;
            }
        }
        /// <summary>
        /// Collection of Endnote layouted widget.
        /// </summary>
        /// <value>The Endnote widgets.</value>
        internal LayoutedWidgetList EndnoteWidgets
        {
            get
            {
                return m_endnoteWidgets;
            }
        }
        /// Collection for section id's of End note Widgets.
        /// </summary>
        /// <value>The Section's ID's.</value>
        internal List<int> EndNoteSectionIndex
        {
            get
            {
                return m_endNotesectionIndex;
            }
        }
        /// Collection for section id's of Foot note Widgets.
        /// </summary>
        /// <value>The Section's ID's.</value>
        internal List<int> FootNoteSectionIndex
        {
            get
            {
                return m_footNotesectionIndex;
            }
        }
        /// <summary>
        /// Gets page Setup info.
        /// </summary>
        public WPageSetup Setup
        {
            get
            {
                return m_pageSetup;
            }
        }

        /// <summary>
        /// Gets page number.
        /// </summary>
        public int Number
        {
            get
            {
                return m_iNumber;
            }
            set
            {
                m_iNumber = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="iNumber">The i number.</param>
        public Page(IWSection section, int iNumber)
        {
            m_docSection = section;
            m_pageSetup = section.PageSetup;
            m_headersFooters = section.HeadersFooters;
            m_iNumber = iNumber;

            IWordDocument doc = section.Document;
            m_backgroundImage = doc.BackgroundImage;
            m_backgroundColor = doc.Background.Color;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        public void InitLayoutInfo()
        {
            for (int i = 0; i < m_pageWidgets.Count; i++)
            {
                LayoutedWidget ltWidget = m_pageWidgets[i];
                ltWidget.InitLayoutInfo();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        public void Draw(DrawingContext dc)
        {
            dc.m_pageMarginLeft = Setup.Margins.Left;
            if (m_pageWidgets.Count != 0)
            {
                if (m_docSection.Document.Background.Type == BackgroundType.Picture && m_backgroundImage != null)
                {
                    dc.DrawBackgroundImage(m_backgroundImage, Setup);
                }
                if (m_docSection.Document.Background.Type == BackgroundType.Color)
                    dc.DrawBackgroundColor(m_backgroundColor, (int)Setup.PageSize.Width, (int)Setup.PageSize.Height);
                //Check whether the picture watermark having wrapping style is infrontoftext means draw
                //this picture watermark after drawing the header footer contents.
                if ((m_pageWidgets[0].Widget as HeaderFooter).WriteWatermark
                    && !(m_docSection.Document.Watermark.Type == WatermarkType.PictureWatermark
                    && ((m_docSection.Document.Watermark as PictureWatermark).WordPicture as WPicture).TextWrappingStyle == TextWrappingStyle.InFrontOfText))
                    dc.DrawWatermark(m_docSection.Document.Watermark, Setup, new RectangleF(m_pageWidgets[2].Bounds.X, m_pageWidgets[2].Bounds.Y, Setup.ClientWidth, (m_pageWidgets[1].Bounds.Y - m_pageWidgets[2].Bounds.Y)));
            }
#if DEBUG_PAGEDRAWING
        RectangleF rect = new RectangleF(
          Setup.Margins.Left, Setup.Margins.Top,
          Setup.PageSize.Width - (Setup.Margins.Left + Setup.Margins.Right),
          Setup.PageSize.Height - (Setup.Margins.Top + Setup.Margins.Bottom)
        );
        context.DrawBounds(Color.Magenta, rect);
#endif

            for (int i = 0; i < m_pageWidgets.Count; i++)
            {
                LayoutedWidget widget = m_pageWidgets[i];
                //Draw the footnote and end note widgets
                if (!(widget.Widget is HeaderFooter))
                {
                    for (int j = 0; j < FootnoteWidgets.Count; j++)
                        FootnoteWidgets[j].Draw(dc);
                    for (int j = 0; j < EndnoteWidgets.Count; j++)
                        EndnoteWidgets[j].Draw(dc);
                }
                widget.Draw(dc);
                // Draw the widgets with OverLapping Wrapping style
                dc.DrawOverLappedShapeWidgets();
#if DEBUG_PAGEDRAWING
            context.DrawBounds(Color.Yellow, widget.Bounds);
#endif
                //write picture water mark after drawing the header footer content if it wrapping style is infrontoftext.
                if (i == 1
                     && (m_pageWidgets[0].Widget as HeaderFooter).WriteWatermark
                     && (m_docSection.Document.Watermark.Type == WatermarkType.PictureWatermark
                     && ((m_docSection.Document.Watermark as PictureWatermark).WordPicture as WPicture).TextWrappingStyle == TextWrappingStyle.InFrontOfText))
                    dc.DrawWatermark(m_docSection.Document.Watermark, Setup, new RectangleF(m_pageWidgets[2].Bounds.X, m_pageWidgets[2].Bounds.Y, Setup.ClientWidth, (m_pageWidgets[1].Bounds.Y - m_pageWidgets[2].Bounds.Y)));

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numPages"></param>
        public void UpdateFieldsNumPages(int numPages)
        {
            for (int i = 0; i < m_cachedFields.Count; i++)
            {
                IWField field = m_cachedFields[i];

                if (field != null && field.FieldType == FieldType.FieldNumPages)
                {
                    //If simple field set page number to field text.
                    (field as WTextRange).Text = numPages.ToString();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public void AddCachedFields(IWField field)
        {
            m_cachedFields.Add(field);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the header area.
        /// </summary>
        /// <returns></returns>
        internal protected RectangleF GetHeaderArea()
        {
            float left = (m_pageSetup.Margins.Left != -0.05f) ? m_pageSetup.Margins.Left : 0;
            float right = (m_pageSetup.Margins.Right != -0.05f) ? m_pageSetup.Margins.Right : 0;
            float top = (m_pageSetup.Margins.Top != -0.05f) ? m_pageSetup.Margins.Top : 0;
            float bottom = (m_pageSetup.Margins.Bottom != -0.05f) ? m_pageSetup.Margins.Bottom : 0;
            float hdrDistance = (m_pageSetup.HeaderDistance != -0.05f) ? m_pageSetup.HeaderDistance : 36;
            float width = m_pageSetup.PageSize.Width;
            float height = m_pageSetup.PageSize.Height;

            return new RectangleF
              (left,
              hdrDistance,
              width - (left + right),
              (height / 2) + hdrDistance
              );
        }

        /// <summary>
        /// Gets the footer area.
        /// </summary>
        /// <returns></returns>
        internal protected RectangleF GetFooterArea()
        {
            float left = (m_pageSetup.Margins.Left != -0.05f) ? m_pageSetup.Margins.Left : 0;
            float right = (m_pageSetup.Margins.Right != -0.05f) ? m_pageSetup.Margins.Right : 0;
            float top = (m_pageSetup.Margins.Top != -0.05f) ? m_pageSetup.Margins.Top : 0;
            float bottom = (m_pageSetup.Margins.Bottom != -0.05f) ? m_pageSetup.Margins.Bottom : 0;
            float ftrDistance = (m_pageSetup.FooterDistance != -0.05f) ? m_pageSetup.FooterDistance : 36;
            float width = m_pageSetup.PageSize.Width;
            float height = m_pageSetup.PageSize.Height;

            return new RectangleF(
              left,
              height - ftrDistance,
              width - (left + right),
              (height / 2)
              );
        }

        /// <summary>
        /// Gets the column area.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="prevWidth">Width of the prev.</param>
        /// <returns></returns>
        internal protected RectangleF GetColumnArea(Column column, float prevWidth)
        {
            MarginsF pageMargins = m_pageSetup.Margins;
            float pageWidth = m_pageSetup.PageSize.Width;
            float pageHeight = m_pageSetup.PageSize.Height;
            float columnWidth;

            float top = Math.Abs((pageMargins.Top != -0.05f) ? pageMargins.Top : 0f);
            float left = (pageMargins.Left != -0.05f) ? pageMargins.Left : 0f;
            float right = (pageMargins.Right != -0.05f) ? pageMargins.Right : 0f;
            float bottom = Math.Abs((pageMargins.Bottom != -0.05f) ? pageMargins.Bottom : 0f);

            float headerHeight = (m_pageWidgets[0].ChildWidgets.Count == 0 || (pageMargins.Top < 0 && top > 0)) ? 0
              : m_pageWidgets[0].Bounds.Height  + ((m_pageSetup.HeaderDistance != -0.05f) ? m_pageSetup.HeaderDistance : 36f);
            float footerHeight = (m_pageWidgets[1].ChildWidgets.Count == 0 || (pageMargins.Bottom < 0 && bottom > 0)) ? 0
              : (m_pageWidgets[1].Bounds.Height + Rendering.DocumentLayouter.SpaceBetweenTextbodyAndFooter)
                + ((m_pageSetup.FooterDistance != -0.05f) ? m_pageSetup.FooterDistance : 36f);
            if (m_docSection.Columns.Count > 1)
                columnWidth = (column == null) ? pageWidth - (left + right) : column.Width;
            else
                columnWidth = m_docSection.PageSetup.ClientWidth;
            return new RectangleF
              (
                left + prevWidth,
                Math.Max(top, headerHeight),
                columnWidth,
                pageHeight -
                (
                  Math.Max(top, headerHeight) +
                  Math.Max(bottom, footerHeight)
                )
              );
        }

        /// <summary>
        /// Gets the column area.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="prevColumnsWidth">Width of the prev columns.</param>
        /// <returns></returns>
        internal protected RectangleF GetColumnArea(int columnIndex, ref float prevColumnsWidth)
        {
            Column col = (m_docSection.Columns.Count > columnIndex)
                           ? m_docSection.Columns[columnIndex]
                           : null;

            RectangleF resRect = GetColumnArea(col, prevColumnsWidth);

            if (col != null) prevColumnsWidth += col.Width + col.Space;

            return resRect;
        }

        /// <summary>
        /// Gets the section area.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="prevWidth">Width of the prev.</param>
        /// <returns></returns>
        internal protected RectangleF GetSectionArea(Column column, float prevWidth)
        {
            MarginsF pageMargins = m_docSection.PageSetup.Margins;
            float pageWidth = m_docSection.PageSetup.PageSize.Width;
            float pageHeight = m_docSection.PageSetup.PageSize.Height;
            float columnWidth;

            float top = Math.Abs((pageMargins.Top != -0.05f) ? pageMargins.Top : 0f);
            float left = (pageMargins.Left != -0.05f) ? pageMargins.Left : 0f;
            float right = (pageMargins.Right != -0.05f) ? pageMargins.Right : 0f;
            float bottom = Math.Abs((pageMargins.Bottom != -0.05f) ? pageMargins.Bottom : 0f);

            if (m_docSection.Columns.Count > 1)
                columnWidth = (column == null) ? pageWidth - (left + right) : column.Width;
            else
                columnWidth = m_docSection.PageSetup.ClientWidth;
            return new RectangleF
              (
                left + prevWidth,
                top,
                columnWidth,
                pageHeight - (top + bottom)
              );
        }

        /// <summary>
        /// Gets the section area.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="prevColumnsWidth">Width of the prev columns.</param>
        /// <param name="isNextSection">isNextSection.</param>
        /// <returns></returns>
        internal protected RectangleF GetSectionArea(int columnIndex, ref float prevColumnsWidth, bool isNextSection, bool isSplittedWidget)
        {
            int index = m_docSection.Document.Sections.IndexOf(m_docSection);
            if (!isSplittedWidget)
                index -= 1;
            if (m_docSection.Document.Sections.Count - 1 > index && columnIndex == 0 && !isNextSection)
            {
                m_docSection = m_docSection.Document.Sections[index + 1];
            }
            Column col = (m_docSection.Columns.Count > columnIndex)
                           ? m_docSection.Columns[columnIndex]
                           : null;
            RectangleF resRect = GetSectionArea(col, prevColumnsWidth);

            if (col != null) prevColumnsWidth += col.Width + col.Space;

            return resRect;
        }
        #endregion
    }
}

#endif