#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Class lay outing the text.
    /// </summary>
    public class PdfStringLayouter
    {
        #region Fields
        /// <summary>
        /// Text data.
        /// </summary>
        private string m_text;

        /// <summary>
        /// Pdf font.
        /// </summary>
        private PdfFont m_font;

        /// <summary>
        /// String format.
        /// </summary>
        private PdfStringFormat m_format;

        /// <summary>
        /// Bounds of the text.
        /// </summary>
        private SizeF m_size;
        private RectangleF m_rect;
        private float m_pageHeight;
        /// <summary>
        /// String tokenizer.
        /// </summary>
        private StringTokenizer m_reader;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLayouter"/> class.
        /// </summary>
        public PdfStringLayouter()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Layouts the text.
        /// </summary>
        /// <param name="text">String text.</param>
        /// <param name="font">Font for the text.</param>
        /// <param name="format">String format.</param>
        /// <param name="size">Bounds of the text.</param>
        /// <returns>Layout result.</returns>
        internal PdfStringLayoutResult Layout(string text, PdfFont font, PdfStringFormat format, RectangleF rect, float pageHeight)
        {
            Initialize(text, font, format, rect, pageHeight);

            PdfStringLayoutResult result = DoLayout();

            Clear();

            return result;
        }

        public PdfStringLayoutResult Layout(string text, PdfFont font, PdfStringFormat format, SizeF size)
        {
            Initialize(text, font, format, size);

            PdfStringLayoutResult result = DoLayout();

            Clear();

            return result;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes internal data.
        /// </summary>
        /// <param name="text">String text.</param>
        /// <param name="font">Font for the text.</param>
        /// <param name="format">String format.</param>
        /// <param name="size">Bounds of the text.</param>
        private void Initialize(string text, PdfFont font, PdfStringFormat format, RectangleF rect, float pageHeight)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            m_text = text;
            m_font = font;
            m_format = format;
            m_size = rect.Size;
            m_rect = rect;
            m_pageHeight = pageHeight;

            m_reader = new StringTokenizer(text);
        }

        private void Initialize(string text, PdfFont font, PdfStringFormat format, SizeF size)
        {
            Initialize(text, font, format, new RectangleF(PointF.Empty, size), 0);
        }

        /// <summary>
        /// Layouts the text.
        /// </summary>
        /// <returns>Lay outing data.</returns>
        private PdfStringLayoutResult DoLayout()
        {
            PdfStringLayoutResult result = new PdfStringLayoutResult();
            PdfStringLayoutResult lineResult = new PdfStringLayoutResult();
            List<LineInfo> lines = new List<LineInfo>();

            string line = m_reader.PeekLine();
            float lineIndent = GetLineIndent(true);

            while (line != null)
            {
                lineResult = LayoutLine(line, lineIndent);

                if (!lineResult.Empty)
                {
                    int numSymbolsInserted = 0;
                    bool success = CopyToResult(result, lineResult, lines, out numSymbolsInserted);

                    if (!success)
                    {
                        m_reader.Read(numSymbolsInserted);
                        break;
                    }
                }

                if (lineResult.Remainder != null && lineResult.Remainder.Length > 0)
                {
                    break;
                }

                m_reader.ReadLine();
                line = m_reader.PeekLine();
                lineIndent = GetLineIndent(false);
            }

            FinalizeResult(result, lines);

            return result;
        }

        /// <summary>
        /// Copies layout result from line result to entire result. Checks whether we can proceed lay outing or not.
        /// </summary>
        /// <param name="result">Final result.</param>
        /// <param name="lineResult">Line result.</param>
        /// <param name="lines">Lines array.</param>
        /// <param name="numInserted">Number of symbols inserted.</param>
        /// <returns>True if we can proceed, False - to stop lay outing.</returns>
        private bool CopyToResult(PdfStringLayoutResult result, PdfStringLayoutResult lineResult,
            List<LineInfo> lines, out int numInserted)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (lineResult == null)
            {
                throw new ArgumentNullException("lineResult");
            }

            if (lines == null)
            {
                throw new ArgumentNullException("lines");
            }

            bool success = true;
            bool allowPartialLines = (m_format != null && !m_format.LineLimit);
            float height = result.ActualSize.Height;

            float maxHeight = m_size.Height;
            if ((m_pageHeight > 0) && (maxHeight + m_rect.Y > m_pageHeight))
            {
                maxHeight = m_rect.Y - m_pageHeight;
                maxHeight = Math.Max(maxHeight, -maxHeight);
            }
            
            numInserted = 0;

            if (lineResult.Lines != null)
            {
                for (int i = 0, len = lineResult.Lines.Length; i < len; i++)
                {
                    float expHeight = /*Utils.Round(*/ height + lineResult.LineHeight /*)*/;

                    if (expHeight <= maxHeight || maxHeight <= 0 || allowPartialLines)
                    {
                        LineInfo info = lineResult.Lines[i];
                        numInserted += info.Text.Length;

                        info = TrimLine(info, (lines.Count == 0));
                        lines.Add(info);

                        // Update width.
                        SizeF size = result.ActualSize;
                        size.Width = Math.Max(size.Width, info.Width);
                        result.m_actualSize = size;

                        // The part of the line fits only and it's allowed to use partial lines.
                        if (expHeight >= maxHeight && maxHeight > 0 && allowPartialLines)
                        {
                            bool shouldClip = (m_format == null || !m_format.NoClip);

                            if (shouldClip)
                            {
                                float exceededHeight = expHeight - maxHeight;
                                float fitHeight = /*Utils.Round(*/ lineResult.LineHeight - exceededHeight /*)*/;
                                height = /*Utils.Round(*/ height + fitHeight /*)*/;
                            }
                            else
                            {
                                height = expHeight;
                            }

                            success = false;
                            break;
                        }
                        else
                        {
                            height = expHeight;
                        }
                    }
                    else
                    {
                        success = false;
                        break;
                    }
                }
            }

            if (height != result.ActualSize.Height)
            {
                SizeF size = result.ActualSize;
                size.Height = height;
                result.m_actualSize = size;
            }

            return success;
        }

        /// <summary>
        /// Finalizes final result.
        /// </summary>
        /// <param name="result">Final result.</param>
        /// <param name="lines">Lines array.</param>
        private void FinalizeResult(PdfStringLayoutResult result, List<LineInfo> lines)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (lines == null)
            {
                throw new ArgumentNullException("lines");
            }

            result.m_lines = lines.ToArray();
            result.m_lineHeight = GetLineHeight();

            if (!m_reader.EOF)
            {
                result.m_remainder = m_reader.ReadToEnd();
            }

            lines.Clear();
        }

        /// <summary>
        /// Cleares all resources.
        /// </summary>
        private void Clear()
        {
            m_font = null;
            m_format = null;
            m_reader.Close();
            m_reader = null;
            m_text = null;
        }

        /// <summary>
        /// Calculates height of the line.
        /// </summary>
        /// <returns>Height of the line.</returns>
        private float GetLineHeight()
        {
            float height = m_font.Height;

            if (m_format != null && m_format.LineSpacing != 0)
            {
                height = m_format.LineSpacing;
            }

            return height;
        }

        /// <summary>
        /// Layouts line.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <param name="lineIndent">Line indent.</param>
        /// <returns>Layout result.</returns>
        private PdfStringLayoutResult LayoutLine(string line, float lineIndent)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            line = line.Replace("\t", "    ");
            PdfStringLayoutResult lineResult = new PdfStringLayoutResult();

            lineResult.m_lineHeight = GetLineHeight();

            List<LineInfo> lines = new List<LineInfo>();
            float maxWidth = /*Utils.Round(*/ m_size.Width /*)*/;
            float lineWidth = /*Utils.Round(*/ GetLineWidth(line) + lineIndent /*)*/;
            LineType lineType = LineType.FirstParagraphLine;
            bool readWord = true;

            // line is in bounds.
            if (maxWidth <= 0 || Math.Round(lineWidth, 2) <= Math.Round(maxWidth, 2))
            {
                AddToLineResult(lineResult, lines, line, lineWidth, LineType.NewLineBreak | lineType);
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                StringBuilder curLine = new StringBuilder();

                lineWidth = lineIndent;
                float curIndent = lineIndent;

                StringTokenizer reader = new StringTokenizer(line);
                string word = reader.PeekWord();

                if (word.Length != reader.Length)
                {
                    if (word == " ")
                    {
                        reader.Position += 1;
                        word = reader.PeekWord();
                    }
                }

                while (word != null)
                {
                    curLine.Append(word);
                    float curLineWidth = /*Utils.Round(*/ GetLineWidth(curLine.ToString()) + curIndent /*)*/;

                    if (curLineWidth > maxWidth)
                    {
                        if (GetWrapType() == PdfWordWrapType.None) break;
                        
                        // First word in the line exceeds bounds.
                        if (curLine.Length == word.Length)
                        {
                            //  Character wrap is disabled or one symbol is greater than bounds.
                            if (GetWrapType() == PdfWordWrapType.WordOnly || curLine.Length == 1)
                            {
                                lineResult.m_remainder = line.Substring(reader.Position);
                                break;
                            }
                            else
                            {
                                readWord = false;
                                curLine.Length = 0;
                                word = reader.Peek().ToString();
                                continue;
                            }
                        }
                        else
                        {
                            if (GetWrapType() != PdfWordWrapType.Character || !readWord)
                            {
                                string ln = builder.ToString();
                                if(ln!=" ")
                                AddToLineResult(lineResult, lines, ln, lineWidth, LineType.LayoutBreak | lineType);
                                curLine.Length = 0;
                                builder.Length = 0;
                                lineWidth = 0f;
                                curIndent = 0f;
                                curLineWidth = 0f;
                                lineType = LineType.None;
                                word = (readWord) ? word : reader.PeekWord();
                                readWord = true;
                            }
                            else
                            {
                                readWord = false;
                                curLine.Length = 0;
                                curLine.Append(builder.ToString());
                                word = reader.Peek().ToString();
                            }

                            continue;
                        }
                    }

                    builder.Append(word);
                    lineWidth = curLineWidth;

                    if (readWord)
                    {
                        reader.ReadWord();
                        word = reader.PeekWord();
                    }
                    else
                    {
                        reader.Read();
                        word = reader.Peek().ToString();
                    }
                }

                if (builder.Length > 0)
                {
                    string ln = builder.ToString();
                    AddToLineResult(lineResult, lines, ln, lineWidth,
                        LineType.NewLineBreak | LineType.LastParagraphLine);
                }

                reader.Close();
            }

            lineResult.m_lines = lines.ToArray();
            lines.Clear();

            return lineResult;
        }

        /// <summary>
        /// Adds line to line result.
        /// </summary>
        /// <param name="lineResult">Line resut.</param>
        /// <param name="lines">Array of the lines.</param>
        /// <param name="line">Text line.</param>
        /// <param name="lineWidth">Line width.</param>
        /// <param name="breakType">Line break type.</param>
        private void AddToLineResult(PdfStringLayoutResult lineResult, List<LineInfo> lines,
            string line, float lineWidth, LineType breakType)
        {
            if (lineResult == null)
            {
                throw new ArgumentNullException("lineResult");
            }

            if (lines == null)
            {
                throw new ArgumentNullException("lines");
            }

            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            LineInfo info = new LineInfo();

            info.Text = line;
            info.Width = lineWidth;
            info.LineType = breakType;
            lines.Add(info);

            SizeF size = lineResult.ActualSize;

            size.Height += GetLineHeight();
            size.Width = Math.Max(size.Width, lineWidth);
            lineResult.m_actualSize = size;
        }

        /// <summary>
        /// Trims whitespaces at the line.
        /// </summary>
        /// <param name="info">Line info.</param>
        /// <param name="firstLine">Indicates whether the line is the first in the text.</param>
        /// <returns>Trimed line info.</returns>
        private LineInfo TrimLine(LineInfo info, bool firstLine)
        {
            string line = info.Text;
            float lineWidth = info.Width;

            // Trim start whitespaces if the line is not a start of the paragraph only.
            bool trimStartSpaces = ((info.LineType & LineType.FirstParagraphLine) == 0);
            bool start = (m_format == null || !m_format.RightToLeft);
            char[] spaces = StringTokenizer.Spaces;

            if (trimStartSpaces)
            {
                line = (start) ? line.TrimStart(spaces) : line.TrimEnd(spaces);
            }

            // Trim end whitespaces.
            bool trimEndSpaces = (m_format == null || !m_format.MeasureTrailingSpaces);
            if (trimEndSpaces)
            {
                // If the line is a start of the paragraph and is whitespaces only - leave one space symbol.
                if ((info.LineType & LineType.FirstParagraphLine) > 0 && StringTokenizer.IsWhitespace(line))
                {
                    line = new string(StringTokenizer.WhiteSpace, 1);
                }
                else
                {
                    line = (start) ? line.TrimEnd(spaces) : line.TrimStart(spaces);
                }
            }

            // Recalculate line width.
            if (line.Length != info.Text.Length)
            {
                lineWidth = GetLineWidth(line);
                if ((info.LineType & LineType.FirstParagraphLine) > 0)
                {
                    lineWidth += GetLineIndent(firstLine);
                }
                //lineWidth = /*Utils.Round(*/ lineWidth /*)*/;
            }

            info.Text = line;
            info.Width = lineWidth;

            return info;
        }

        /// <summary>
        /// Calculates width of the line.
        /// </summary>
        /// <param name="line">String line.</param>
        /// <returns>Width of the line.</returns>
        private float GetLineWidth(string line)
        {
            //if (m_font is PdfTrueTypeFont)
            //{
            //    if ((m_font as PdfTrueTypeFont).Unicode
            //        && m_format != null && line.Length > 1)
            //        m_format.RightToLeft = true;
            //}

            float width = m_font.GetLineWidth(line, m_format);
            return width;
        }

        /// <summary>
        /// Returns line indent for the line.
        /// </summary>
        /// <param name="firstLine">If true - the line is the first in the text.</param>
        /// <returns>Line indent for the line.</returns>
        private float GetLineIndent(bool firstLine)
        {
            float lineIndent = 0f;

            if (m_format != null)
            {
                lineIndent = (firstLine) ? m_format.FirstLineIndent : m_format.ParagraphIndent;
                lineIndent = (m_size.Width > 0) ? Math.Min(m_size.Width, lineIndent) : lineIndent;
                //lineIndent = Utils.Round( lineIndent );
            }

            return lineIndent;
        }

        /// <summary>
        /// Returns wrap type.
        /// </summary>
        /// <returns>Returns wrap type.</returns>
        private PdfWordWrapType GetWrapType()
        {
            PdfWordWrapType wrapType = (m_format != null) ? m_format.WordWrap : PdfWordWrapType.Word;
            return wrapType;
        }
        #endregion
    }

    #region Internal declaration
    /// <summary>
    /// Layouter result.
    /// </summary>
    public class PdfStringLayoutResult
    {
        #region Fields
        /// <summary>
        /// Layouted lines.
        /// </summary>
        internal LineInfo[] m_lines;

        /// <summary>
        /// The text wasn't lay outed.
        /// </summary>
        internal string m_remainder;

        /// <summary>
        /// Actual layouted text bounds.
        /// </summary>
        internal SizeF m_actualSize;

        /// <summary>
        /// Height of the line.
        /// </summary>
        internal float m_lineHeight;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the text which is not lay outed
        /// </summary>
        public string Remainder
        {
            get
            {
                return m_remainder;
            }
        }

        /// <summary>
        /// Gets the actual layouted text bounds
        /// </summary>
        public SizeF ActualSize
        {
            get
            {
                return m_actualSize;
            }
        }


        /// <summary>
        /// Gets layouted lines information.
        /// </summary>
        public LineInfo[] Lines
        {
            get
            {
                return m_lines;
            }
        }

        /// <summary>
        /// Gets the height of the line.
        /// </summary>
        public float LineHeight
        {
            get
            {
                return m_lineHeight;
            }
        }

        /// <summary>
        /// Gets value that indicates whether any text was layouted.
        /// </summary>
        internal bool Empty
        {
            get
            {
                return (m_lines == null || m_lines.Length == 0);
            }
        }

        /// <summary>
        /// Gets number of the lines layouted.
        /// </summary>
        internal int LineCount
        {
            get
            {
                int count = (!Empty) ? m_lines.Length : 0;
                return count;
            }
        }
        #endregion
    }

    /// <summary>
    /// Contains information about the line.
    /// </summary>
    public class LineInfo
    {
        #region Fields
        /// <summary>
        /// Line text.
        /// </summary>
        internal string m_text;

        /// <summary>
        /// Width of the text.
        /// </summary>
        internal float m_width;

        /// <summary>
        /// Breaking type of the line.
        /// </summary>
        internal LineType m_lineType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets width of the line text.
        /// </summary>
        public LineType LineType
        {
            get
            {
                return m_lineType;
            }
            internal set
            {
                m_lineType = value;
            }
        }

        /// <summary>
        /// Gets line text.
        /// </summary>
        public string Text
        {
            get
            {
                return m_text;
            }
            internal set
            {
                m_text = value;
            }
        }

        /// <summary>
        /// Gets width of the line text.
        /// </summary>
        public float Width
        {
            get
            {
                return m_width;
            }
            internal set
            {
                m_width = value;
            }
        }
        #endregion

    }

    /// <summary>
    /// Break type of the line.
    /// </summary>
    [Flags]
    public enum LineType
    {
        /// <summary>
        /// Unknown type line.
        /// </summary>
        None = 0,

        /// <summary>
        /// The line has new line symbol.
        /// </summary>
        NewLineBreak = 0x0001,

        /// <summary>
        /// layout break.
        /// </summary>
        LayoutBreak = 0x0002,

        /// <summary>
        /// The line is the first in the paragraph.
        /// </summary>
        FirstParagraphLine = 0x0004,

        /// <summary>
        /// The line is the last in the paragraph.
        /// </summary>
        LastParagraphLine = 0x0008
    }
    #endregion

}
