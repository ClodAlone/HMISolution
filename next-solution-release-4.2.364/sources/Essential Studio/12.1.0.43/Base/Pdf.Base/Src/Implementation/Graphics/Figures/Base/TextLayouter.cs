#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Class that layouts the text.
    /// </summary>
    internal class TextLayouter : ElementLayouter
    {
        #region Fields
        /// <summary>
        /// String format.
        /// </summary>
        private PdfStringFormat m_format;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextLayouter"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public TextLayouter(PdfTextElement element)
            : base(element)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets  element`s layout.
        /// </summary>
        /// <value></value>
        new public PdfTextElement Element
        {
            get
            {
                return (base.Element as PdfTextElement);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Lay outing result.</returns>
        protected override PdfLayoutResult LayoutInternal(PdfLayoutParams param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            m_format = (Element.StringFormat != null) ? (PdfStringFormat)Element.StringFormat.Clone() : null;

            PdfPage currentPage = param.Page;
            RectangleF currentBounds = param.Bounds;
            string text = Element.Value;
            PdfTextLayoutResult result = null;
            TextPageLayoutResult pageResult = new TextPageLayoutResult();

            pageResult.Page = currentPage;
            pageResult.Remainder = text;

            while (true)
            {
                // Raise event.
                bool cancel = RaiseBeforePageLayout(currentPage, ref currentBounds);
                EndTextPageLayoutEventArgs endArgs = null;

                if (!cancel)
                {
                    pageResult = LayoutOnPage(text, currentPage, currentBounds, param);

                    // Raise event.
                    endArgs = RaisePageLayouted(pageResult);
                    cancel = (endArgs == null) ? false : endArgs.Cancel;
                }

                if (!pageResult.End && !cancel)
                {
                    currentBounds = GetPaginateBounds(param);
                    text = pageResult.Remainder;
                    currentPage = (endArgs == null || endArgs.NextPage == null) ?
                        GetNextPage(currentPage) : endArgs.NextPage;
                }
                else
                {
                    result = GetLayoutResult(pageResult);
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates layout result.
        /// </summary>
        /// <param name="pageResult">Page layout result.</param>
        /// <returns>Layout result.</returns>
        private PdfTextLayoutResult GetLayoutResult(TextPageLayoutResult pageResult)
        {
            PdfTextLayoutResult result = new PdfTextLayoutResult(pageResult.Page,
                pageResult.Bounds, pageResult.Remainder, pageResult.LastLineBounds);

            return result;
        }

        /// <summary>
        /// Layouts the text on the page.
        /// </summary>
        /// <param name="text">The text that should be printed.</param>
        /// <param name="currentPage">Current page.</param>
        /// <param name="currentBounds">Current bounds.</param>
        /// <param name="param">Layout parameters.</param>
        /// <returns>Page layout result.</returns>
        private TextPageLayoutResult LayoutOnPage(string text, PdfPage currentPage,
            RectangleF currentBounds, PdfLayoutParams param)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (currentPage == null)
            {
                throw new ArgumentNullException("currentPage");
            }

            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            TextPageLayoutResult result = new TextPageLayoutResult();

            result.Remainder = text;
            result.Page = currentPage;
            currentBounds = CheckCorrectBounds(currentPage, currentBounds);

            if (currentBounds.Height < 0)
            {
                currentPage = GetNextPage(currentPage);
                PdfMargins margins = currentPage.Section.PageSettings.Margins;
                result.Page = currentPage;
                currentBounds = new RectangleF(currentBounds.X, 0, currentBounds.Width, currentBounds.Height);
            }

            PdfStringLayouter layouter = new PdfStringLayouter();
            PdfStringLayoutResult stringResult = layouter.Layout(text, Element.Font,
                m_format, currentBounds, currentPage.GetClientSize().Height);

            bool textFinished = (stringResult.Remainder == null || stringResult.Remainder.Length == 0);
            bool doesntFit = (param.Format.Break == PdfLayoutBreakType.FitElement &&
                currentPage == param.Page && !textFinished);

            bool canDraw = !(doesntFit || stringResult.Empty);

            if (canDraw)
            {
                // Draw the text.
                PdfGraphics graphics = currentPage.Graphics;
                graphics.DrawStringLayoutResult(stringResult, Element.Font, Element.Pen, Element.GetBrush(), currentBounds, m_format);

                LineInfo lineInfo = stringResult.Lines[stringResult.LineCount - 1];

                result.LastLineBounds = graphics.GetLineBounds(stringResult.LineCount - 1, stringResult, Element.Font, currentBounds, m_format);
                result.Bounds = GetTextPageBounds(currentPage, currentBounds, stringResult);
                result.Remainder = stringResult.Remainder;
                CheckCorectStringFormat(lineInfo);
            }

            // Eliminate dead loop because the text line doesn't fit to the bounds.
            bool stopLayouting = (stringResult.Empty &&
                ((param.Format.Break != PdfLayoutBreakType.FitElement) ||
                (param.Format.Break == PdfLayoutBreakType.FitElement && currentPage != param.Page)));

            result.End = (textFinished || stopLayouting || param.Format.Layout == PdfLayoutType.OnePage);

            return result;
        }

        /// <summary>
        /// Corrects current bounds on the page.
        /// </summary>
        /// <param name="currentPage">Current page.</param>
        /// <param name="currentBounds">Current lay outing bounds.</param>
        /// <returns>Corrected lay outing bounds.</returns>
        private RectangleF CheckCorrectBounds(PdfPage currentPage, RectangleF currentBounds)
        {
            if (currentPage == null)
            {
                throw new ArgumentNullException("currentPage");
            }

            SizeF pageSize = currentPage.Graphics.ClientSize;

            currentBounds.Height = (currentBounds.Height > 0) ? currentBounds.Height :
                pageSize.Height - currentBounds.Y;

            return currentBounds;
        }

        /// <summary>
        /// Returns a rectangle where the text was printed on the page.
        /// </summary>
        /// <param name="currentPage">Current page.</param>
        /// <param name="currentBounds">Current page text bounds.</param>
        /// <param name="stringResult">Layout result.</param>
        /// <returns>Returns a rectangle where the text was printed on the page.</returns>
        private RectangleF GetTextPageBounds(PdfPage currentPage, RectangleF currentBounds, PdfStringLayoutResult stringResult)
        {
            if (currentPage == null)
            {
                throw new ArgumentNullException("currentPage");
            }

            if (stringResult == null)
            {
                throw new ArgumentNullException("stringResult");
            }

            SizeF textSize = stringResult.ActualSize;
            float x = currentBounds.X;
            float y = currentBounds.Y;
            float width = (currentBounds.Width > 0) ? currentBounds.Width : textSize.Width;
            float height = textSize.Height;

            RectangleF shiftedRect = currentPage.Graphics.CheckCorrectLayoutRectangle(
                textSize, currentBounds.X, currentBounds.Y, m_format);

            if (currentBounds.Width <= 0)
            {
                x = shiftedRect.X;
            }

            if (currentBounds.Height <= 0)
            {
                y = shiftedRect.Y;
            }

            float verticalShift = currentPage.Graphics.GetTextVerticalAlignShift(
                textSize.Height, currentBounds.Height, m_format);

            y += verticalShift;

            RectangleF bounds = new RectangleF(x, y, width, height);

            return bounds;
        }

        /// <summary>
        /// Raises PageLayout event if needed.
        /// </summary>
        /// <param name="pageResult">Page layout result.</param>
        /// <returns>Event arguments.</returns>
        private EndTextPageLayoutEventArgs RaisePageLayouted(TextPageLayoutResult pageResult)
        {
            EndTextPageLayoutEventArgs args = null;

            if (Element.RaiseEndPageLayout)
            {
                PdfTextLayoutResult res = GetLayoutResult(pageResult);
                args = new EndTextPageLayoutEventArgs(res);

                Element.OnEndPageLayout(args);
            }

            return args;
        }

        /// <summary>
        /// Raises BeforePageLayout event.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="currentBounds">The current bounds.</param>
        /// <returns>If true, stops the layout.</returns>
        private bool RaiseBeforePageLayout(PdfPage currentPage, ref RectangleF currentBounds)
        {
            bool cancel = false;

            if (Element.RaiseBeginPageLayout)
            {
                BeginPageLayoutEventArgs args = new BeginPageLayoutEventArgs(currentBounds, currentPage);

                Element.OnBeginPageLayout(args);

                cancel = args.Cancel;
                currentBounds = args.Bounds;
            }

            return cancel;
        }

        /// <summary>
        /// Corrects string format.
        /// </summary>
        /// <param name="lineInfo">The last line infor layouted.</param>
        private void CheckCorectStringFormat(LineInfo lineInfo)
        {
            if (m_format != null)
            {
                m_format.FirstLineIndent = ((lineInfo.LineType & LineType.NewLineBreak) > 0) ?
                    Element.StringFormat.FirstLineIndent : 0f;
            }
        }
        #endregion

        #region Internal declaration
        /// <summary>
        /// Contains lay outing result settings.
        /// </summary>
        private struct TextPageLayoutResult
        {
            /// <summary>
            /// The last page where the text was drawn.
            /// </summary>
            public PdfPage Page;

            /// <summary>
            /// The bounds of the element on the last page where it was drawn.
            /// </summary>
            public RectangleF Bounds;

            /// <summary>
            /// Indicates whether the lay outing has been finished.
            /// </summary>
            public bool End;

            /// <summary>
            /// The text that was  not printed.
            /// </summary>
            public string Remainder;

            /// <summary>
            /// Gets or sets a bounds of the last text line that was printed.
            /// </summary>
            public RectangleF LastLineBounds;
        }
        #endregion
    }

    /// <summary>
    /// Represents the text lay outing result settings.
    /// </summary>
    public class PdfTextLayoutResult : PdfLayoutResult
    {
        #region Fields
        /// <summary>
        /// The text that was not printed.
        /// </summary>
        private string m_remainder;

        /// <summary>
        /// The bounds of the last line that was printed.
        /// </summary>
        private RectangleF m_lastLineBounds;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value that contains the text that was not printed.
        /// </summary>
        public string Remainder
        {
            get
            {
                return m_remainder;
            }
        }

        /// <summary>
        /// Gets a value that indicates the bounds of the last line that was printed on the page.
        /// </summary>
        public RectangleF LastLineBounds
        {
            get
            {
                return m_lastLineBounds;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextLayoutResult"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="remainder">The remainder.</param>
        /// <param name="lastLineBounds">The last line bounds.</param>
        internal PdfTextLayoutResult(PdfPage page, RectangleF bounds, string remainder, RectangleF lastLineBounds)
            : base(page, bounds)
        {
            m_remainder = remainder;
            m_lastLineBounds = lastLineBounds;
        }
        #endregion
    }
}
