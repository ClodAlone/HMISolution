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
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.Rendering;

#if SILVERLIGHT
using Font = Syncfusion.DocIO.DLS.Font;
#else
using Font = System.Drawing.Font;
using Syncfusion.DocIO;
#endif

namespace Syncfusion.Layouting
{
    /// <summary>
    /// 
    /// </summary>
    internal class SplitStringWidget : ISplitLeafWidget
    {
        #region Fields
        private IStringWidget m_strWidget;
        private string m_splittedText;
        //Specifies the index of previous layouted widget from widget collection
        internal int m_prevWidgetIndex = -1;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitStringWidget"/> class.
        /// </summary>
        /// <param name="strWidget">The STR widget.</param>
        /// <param name="text">The text.</param>
        public SplitStringWidget(IStringWidget strWidget, string text)
        {
            m_strWidget = strWidget;
            m_splittedText = text;
        }

        public SplitStringWidget(IStringWidget strWidget, StringSplitInfo splitInfo, bool firstPart)
        {
            string textToSplit = string.Empty;
            textToSplit = (strWidget as WTextRange).TextToSplit;
            
            m_strWidget = strWidget;
            string text = (firstPart) ? textToSplit.Substring(0, splitInfo.LastPos + 1)
                : textToSplit.Substring(splitInfo.FirstPos);
            m_splittedText = text;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets and sets the Splitted Text
        /// </summary>
        public string SplittedText
        {
            get
            {
                return m_splittedText;
            }
            set
            {
                m_splittedText = value;
            }
        }
        /// <summary>
        /// Gets the real string widget.
        /// </summary>
        /// <value>The real string widget.</value>
        public IStringWidget RealStringWidget
        {
            get
            {
                return m_strWidget;
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return SplittedText;
        }
        #endregion

        #region IWidget Members
        /// <summary>
        /// Gets layout info.
        /// </summary>
        public ILayoutInfo LayoutInfo
        {
            get
            {
                return m_strWidget.LayoutInfo;
            }
        }

        /// <summary>
        /// Draw range to graphics.
        /// </summary>
        public void Draw(DrawingContext dc, LayoutedWidget layoutedWidget)
        {
            if (m_strWidget is WField &&  SplittedText != null
                && SplittedText != string.Empty)
            {
                string text = (m_strWidget as WField).Text;
                (m_strWidget as WField).Text = SplittedText;
                (m_strWidget as ILeafWidget).Draw(dc, layoutedWidget);
                (m_strWidget as WField).Text = text;
            }
            else
            {
                m_strWidget.Draw(dc, layoutedWidget, SplittedText);
            }
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
        }
        #endregion

        #region ILeafWidget Members
        /// <summary>
        /// Splits the size of the by.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="clientWidth">The clientWidth.</param>
        /// <returns></returns>
        public ISplitLeafWidget[] SplitBySize(DrawingContext dc, SizeF offset, float clientWidth, float clientActiveAreaWidth, ref bool isLastWordFit)
        {
            return SplitBySize(dc, offset.Width, m_strWidget, SplittedText, clientWidth, clientActiveAreaWidth, ref isLastWordFit);
        }

        /// <summary>
        /// Measures the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns></returns>
        public SizeF Measure(DrawingContext dc)
        {
            return m_strWidget.Measure(dc, GetText());
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Splits the by offset.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="strWidget">The STR widget.</param>
        /// <param name="splittedText">The splitted text.</param>
        /// <returns></returns>
        public static ISplitLeafWidget[] SplitBySize(DrawingContext dc, double offset, IStringWidget strWidget, string splittedText, float clientWidth, float clientActiveAreaWidth, ref bool isLastWordFit)
        {
            StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
            //remove the linelimit flag from create stringformat object.
            //The behavior of the linelimit flag is "only entire lines are laid out in the formatting rectangle�, 
            //By default layout continues until the end of the text, or until no more lines are visible as a result of clipping�.
            stringFormat.FormatFlags &= ~StringFormatFlags.LineLimit;
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Near;
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox;

            string textToSplit = (splittedText != null) ? splittedText : strWidget.Text;
            bool isTextAllCaps = (strWidget is WField) ? (strWidget as WField).CharacterFormat.AllCaps : (strWidget as WTextRange).CharacterFormat.AllCaps;
            if (isTextAllCaps)
            {
                textToSplit = textToSplit.ToUpper();
            }
            WCharacterFormat charFormat = (strWidget is WField) ? (strWidget as WField).CharacterFormat : (strWidget as WTextRange).CharacterFormat;
            Font font = dc.GetFont(charFormat, splittedText);
            Font defaultFont = dc.GetDefaultFont(font, charFormat);
            float charSpacing = charFormat.CharacterSpacing;

            StringSplitter layouter = new StringSplitter();
            StringSplitResult result = layouter.Split(textToSplit, dc.Graphics, font, defaultFont, stringFormat, new SizeF((float)offset, float.MaxValue), charFormat, ref isLastWordFit);

            if (result.Lines.Length > 0)
            {
                ISplitLeafWidget[] spLeafWidgets = new ISplitLeafWidget[2];
                spLeafWidgets[0] = new SplitStringWidget(strWidget, result.Lines[0].Line);
                string remText = string.Empty;
                if (result.Remainder == null && result.Lines.Length > 1)
                {
                    for (int i = 1; i < result.Count; i++)
                    {
                        if (result.Lines[i].Line == ControlChar.Space)
                            remText += ControlChar.LineFeed;
                        else
                            remText += ControlChar.LineFeed + result.Lines[i].Line;
                    }
                }
                else
                    remText = result.Remainder;
                if (remText == ControlChar.LineFeed || remText == ControlChar.ParagraphBreak)
                    remText = ControlChar.Space;
                if (remText != null)
                {
                    if (remText.StartsWith(ControlChar.LineFeed) || remText.StartsWith(ControlChar.ParagraphBreak))
                        remText = remText.Remove(0, 1);
                    else if (remText.StartsWith(ControlChar.Space))
                    {
                        remText = remText.TrimStart(ControlChar.SpaceChar);
                    }
                }
                spLeafWidgets[1] = new SplitStringWidget(strWidget, remText);
                return spLeafWidgets;
            }
            else if (strWidget.Text != null && strWidget.Text != string.Empty)
            {
                (strWidget as WTextRange).TextToSplit = result.Remainder;
                if (dc.MeasureTextRange((strWidget as WTextRange), (strWidget as WTextRange).TextToSplit).Width > offset)
                {
                    ISplitLeafWidget[] spLeafWidgets = SplitByOffset(dc, offset, strWidget, null, clientWidth,clientActiveAreaWidth);
                    return spLeafWidgets;
                }
            }
            return null;
        }

        public static ISplitLeafWidget[] SplitByOffset(DrawingContext dc, double offset, IStringWidget strWidget, StringSplitInfo splitInfo, float clientWidth,float clientActiveAreaWidth)
        {
            string textToSplit = string.Empty;
            textToSplit = (strWidget as WTextRange).TextToSplit;

            if (splitInfo == null)
                splitInfo = new StringSplitInfo(0, textToSplit.Length - 1);
            
            int index = strWidget.OffsetToIndex(dc, offset,
              splitInfo.GetSubstring(textToSplit), clientWidth,clientActiveAreaWidth);

            if (index > -1 && index < splitInfo.Length - 1)
            {
                ISplitLeafWidget[] spLeafWidgets = new ISplitLeafWidget[2];
                spLeafWidgets[0] =
                  new SplitStringWidget(strWidget, splitInfo.GetSplitFirstPart(index + 1), true);
                spLeafWidgets[1] =
                  new SplitStringWidget(strWidget, splitInfo.GetSplitSecondPart(index + 1), false);
                (strWidget as WTextRange).TextToSplit = (strWidget as WTextRange).TextToSplit.Remove(0, index + 1);
                return spLeafWidgets;
            }

            return null;
        }
        #endregion
    }

    /// <summary>
    /// Utility class for working with strings.
    /// </summary>
    internal class StringParser
    {
        #region Constants
        /// <summary>
        /// Whitespace symbol.
        /// </summary>
        public const char WhiteSpace = ' ';

        /// <summary>
        /// Whitespace symbol.
        /// </summary>
        public const char Tab = '\t';

        /// <summary>
        /// Hyphen symbol.
        /// </summary>
        public const char Hyphen = '-';

        /// <summary>
        /// Array of spaces.
        /// </summary>
        public static readonly char[] Spaces = new char[] { WhiteSpace, Tab, Hyphen };

        /// <summary>
        /// Default RegEx checks object's options.
        /// </summary>
        private const RegexOptions c_regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        /// <summary>
        /// Pattern for WhiteSpace.
        /// </summary>
        private const string c_whiteSpacePatterm = @"^[ \t-]+$";
        #endregion

        #region Fields
        /// <summary>
        /// Whitespace regex.
        /// </summary>
        private static Regex s_whiteSpaceRegex = new Regex(c_whiteSpacePatterm, c_regexOptions);

        /// <summary>
        /// Text data.
        /// </summary>
        private string m_text;

        /// <summary>
        /// Current position.
        /// </summary>
        private int m_position;
        #endregion

        #region Properties
        /// <summary>
        /// Gets value indicating whether there is and of the data.
        /// </summary>
        public bool EOF
        {
            get
            {
                return (m_position == m_text.Length);
            }
        }

        /// <summary>
        /// Gets text length.
        /// </summary>
        public int Length
        {
            get
            {
                return m_text.Length;
            }
        }

        /// <summary>
        /// Gets current position.
        /// </summary>
        public int Position
        {
            get
            {
                return m_position;
            }

            set
            {
                m_position = value;
            }

        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StringParser"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public StringParser(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            m_text = text;
        }
        #endregion

        #region Public methods
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Returns number of symbols occurred in the text.
        /// </summary>
        /// <param name="text">Text data.</param>
        /// <param name="symbol">Symbol to be searched.</param>
        /// <returns>
        /// Number of symbols occurred in the text.
        /// </returns>
        public static int GetCharsCount(string text, char symbol)
        {
            if (text == null)
                throw new ArgumentNullException("wholeText");

            int numSymbols = 0;
            int curIndex = 0;

            while (true)
            {
                curIndex = text.IndexOf(symbol, curIndex);

                if (curIndex == -1)
                {
                    break;
                }
                else
                {
                    numSymbols++;
                    curIndex++;

                    if (curIndex == text.Length)
                    {
                        break;
                    }
                }
            }

            return numSymbols;
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Returns number of symbols occurred in the text.
        /// </summary>
        /// <param name="text">Text data.</param>
        /// <param name="symbols"> Array of symbols to be searched.</param>
        /// <returns>
        /// Number of symbols occurred in the text.
        /// </returns>
        public static int GetCharsCount(string text, char[] symbols)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (symbols == null)
                throw new ArgumentNullException("symbols");

            int count = 0;

            for (int i = 0, len = text.Length; i < len; i++)
            {
                char ch = text[i];

                if (Contains(symbols, ch))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Reads line of the text.
        /// </summary>
        /// <returns>Text line.</returns>
        public string ReadLine()
        {
            int pos = m_position;

            while (pos < Length)
            {
                char ch = m_text[pos];

                switch (ch)
                {
                    case '\r':
                    case '\n':
                        {
                            string text = m_text.Substring(m_position, pos - m_position);

                            m_position = pos + 1;

                            if (((ch == '\r') && (m_position < Length)) && (m_text[m_position] == '\n'))
                            {
                                m_position++;
                            }
                            if (text == "")
                                text = " ";
                            return text;
                        }
                }
                pos++;
            }

            // The remaining text.
            if (pos > m_position)
            {
                string text2 = m_text.Substring(m_position, pos - m_position);

                m_position = pos;

                return text2;
            }

            if (pos > 0 && pos == m_position && pos == Length && (m_text[pos - 1] == '\n' || m_text[pos - 1] == '\r'))
                return " ";
            
            return null;
        }

        /// <summary>
        /// Reads line of the text.
        /// </summary>
        /// <returns>Text line.</returns>
        public string PeekLine()
        {
            int pos = m_position;
            string line = ReadLine();

            m_position = pos;

            return line;
        }

        /// <summary>
        /// reads a word from the text.
        /// </summary>
        /// <returns>A word from the data.</returns>
        public string ReadWord()
        {
            int pos = m_position;

            while (pos < Length)
            {
                char ch = m_text[pos];

                switch (ch)
                {
                    case '\r':
                    case '\n':
                        {
                            string text = m_text.Substring(m_position, pos - m_position);
                            m_position = pos + 1;
                            if (((ch == '\r') && (m_position < Length)) && (m_text[m_position] == '\n'))
                            {
                                m_position++;
                            }

                            return text;
                        }

                    case ' ':
                    case '\t':
                    case '-':
                        {
                            if (pos == m_position || ch == '-')
                            {
                                pos++;
                            }

                            string text = m_text.Substring(m_position, pos - m_position);
                            m_position = pos;
                            return text;
                        }

                }
                pos++;
            }

            // The remaining text.
            if (pos > m_position)
            {
                string text2 = m_text.Substring(m_position, pos - m_position);

                m_position = pos;

                return text2;
            }

            return null;
        }

        /// <summary>
        /// Peeks a word from the text.
        /// </summary>
        /// <returns>A word from the data.</returns>
        public string PeekWord()
        {
            int pos = m_position;
            string word = ReadWord();

            m_position = pos;

            return word;
        }

        /// <summary>
        /// Reads char form the data.
        /// </summary>
        /// <returns>Char symbol.</returns>
        public char Read()
        {
            char ch = (char)0;

            if (!EOF)
            {
                ch = m_text[m_position];
                m_position++;
            }

            return ch;
        }

        /// <summary>
        /// Reads count of the symbols.
        /// </summary>
        /// <param name="count">Number of symbols.</param>
        /// <returns>String text.</returns>
        public string Read(int count)
        {
            int num = 0;
            StringBuilder builder = new StringBuilder();

            while (!EOF && num < count)
            {
                char ch = Read();

                builder.Append(ch);
                num++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Reads data till the symbol.
        /// </summary>
        /// <param name="symbol">Specified symbol.</param>
        /// <param name="readSymbol">If true - to read the symbol.</param>
        /// <returns>The data read.</returns>
        public string ReadToSymbol(char symbol, bool readSymbol)
        {
            StringBuilder builder = new StringBuilder();

            while (!EOF)
            {
                char ch = Peek();

                if (ch == symbol)
                {
                    if (readSymbol)
                    {
                        Read();
                        builder.Append(ch);
                    }
                    break;
                }

                builder.Append(ch);
                Read();
            }

            string result = builder.ToString();

            return result;
        }

        /// <summary>
        /// Peeks char form the data.
        /// </summary>
        /// <returns>Char symbol.</returns>
        public char Peek()
        {
            char ch = (char)0;

            if (!EOF)
            {
                ch = m_text[m_position];
            }

            return ch;
        }

        /// <summary>
        /// Closes a reader.
        /// </summary>
        public void Close()
        {
            m_text = null;
        }

        /// <summary>
        /// Reads text to the end.
        /// </summary>
        /// <returns>Reads text to the end.</returns>
        public string ReadToEnd()
        {
            string text;

            if (m_position == 0)
            {
                text = m_text;
            }
            else
            {
                text = m_text.Substring(m_position, Length - m_position);
            }

            m_position = Length;

            return text;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Indicates whether user specified token is whitespace symbols or not.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>True if token is whitespace; False otherwise.</returns>
        internal static bool IsWhitespace(string token)
        {
            if (token == null) return false;

            try
            {
                bool result = s_whiteSpaceRegex.Match(token).Success;
                return result;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates whether user specified token is whitespace symbols or not.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>True if token is whitespace; False otherwise.</returns>
        internal static bool IsSpace(char token)
        {
            bool result = (token == WhiteSpace);

            return result;
        }

        /// <summary>
        /// Indicates whether user specified token is tab symbols or not.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>True if token is whitespace; False otherwise.</returns>
        internal static bool IsTab(char token)
        {
            bool result = (token == Tab);

            return result;
        }

        /// <summary>
        /// Calculates number of the whitespace symbols at the start or at the end of the line.
        /// </summary>
        /// <param name="line">String line.</param>
        /// <param name="start">If true - check start of the line, end of the line otherwise.</param>
        /// <returns>Number of the whitespace symbols at the start or at the end of the line.</returns>
        internal static int GetWhitespaceCount(string line, bool start)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            int count = 0;

            if (line.Length > 0)
            {
                for (int i = (start) ? 0 : line.Length - 1; i >= 0 && i < line.Length; i = (start) ? (i + 1) : (i - 1))
                {
                    char ch = line[i];

                    if (!IsSpace(ch) && !IsTab(ch))
                    {
                        break;
                    }

                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Checks whether array contains a symbol.
        /// </summary>
        /// <param name="array">Array of symbols.</param>
        /// <param name="symbol">Char symbol.</param>
        /// <returns>True - if comtains, False otherwise.</returns>
        private static bool Contains(char[] array, char symbol)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            bool contains = false;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == symbol)
                {
                    contains = true;
                    break;
                }
            }

            return contains;
        }
        #endregion
    }

    /// <summary>
    /// Class for splitting text.
    /// </summary>
    internal class StringSplitter
    {
        #region Fields
        /// <summary>
        /// Text data.
        /// </summary>
        private string m_text;

        /// <summary>
        /// The font.
        /// </summary>
        private Font m_font;

        /// <summary>
        /// The font.
        /// </summary>
        private Font m_defaultFont;
        /// <summary>
        /// String format.
        /// </summary>
        private StringFormat m_format;

        /// <summary>
        /// Character Spacing
        /// </summary>
        private WCharacterFormat m_charFormat;

        /// <summary>
        /// Bounds of the text.
        /// </summary>
        private SizeF m_size;

        /// <summary>
        /// String parser.
        /// </summary>
        private StringParser m_reader;

        /// <summary>
        /// Current Graphics
        /// </summary>
        private Graphics m_graphics;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StringSplitter"/> class.
        /// </summary>
        public StringSplitter()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Splits the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="font">The font.</param>
        /// <param name="format">The format.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public StringSplitResult Split(string text, Graphics graphics, Font font, Font defaultFont, StringFormat format, SizeF size, WCharacterFormat charFormat, ref bool isLastWordFit)
        {
            Initialize(text, graphics, font, defaultFont, format, size, charFormat);

            StringSplitResult result = DoSplit(ref isLastWordFit);

            Clear();

            return result;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="font">The font.</param>
        /// <param name="format">The format.</param>
        /// <param name="size">The size.</param>
        private void Initialize(string text, Graphics graphics, Font font,Font defaultFont, StringFormat format, SizeF size, WCharacterFormat charFormat)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (font == null)
                throw new ArgumentNullException("font");

            m_text = text;
            m_graphics = graphics;
            m_font = font;
            m_defaultFont = defaultFont;
            m_format = format;
            m_size = size;
            m_charFormat = charFormat;
            m_reader = new StringParser(text);
        }

        /// <summary>
        /// Does the split.
        /// </summary>
        /// <returns></returns>
        private StringSplitResult DoSplit(ref bool isLastWordFit)
        {
            StringSplitResult result = new StringSplitResult();
            StringSplitResult lineResult = new StringSplitResult();
            List<TextLineInfo> lines = new List<TextLineInfo>();

            string line = m_reader.PeekLine();
            float lineIndent = GetLineIndent(true);

            while (line != null)
            {
                lineResult = SplitLine(line, lineIndent, ref isLastWordFit);
                if (m_reader.Length == line.Length)
                {
                    return lineResult;
                }

                if (!lineResult.Empty)
                {
                    int numSymbolsInserted = 0;
                    bool success = CopyToResult(result, lineResult, lines, out numSymbolsInserted);

                    if (!success)
                    {
                        m_reader.Read(numSymbolsInserted);
                        break;
                    }
                    if(m_reader.Length == m_reader.Position)
                        break;
                }

                if (lineResult.Remainder != null && lineResult.Remainder.Length > 0)
                {
                    break;
                }

                m_reader.ReadLine();
                line = m_reader.PeekLine();
                lineIndent = GetLineIndent(false);
            }

            SaveResult(result, lines);

            return result;
        }

        /// <summary>
        /// Copies to result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="lineResult">The line result.</param>
        /// <param name="lines">The lines.</param>
        /// <param name="numInserted">The num inserted.</param>
        /// <returns></returns>
        private bool CopyToResult(StringSplitResult result, StringSplitResult lineResult,
           List<TextLineInfo> lines, out int numInserted)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (lineResult == null)
                throw new ArgumentNullException("lineResult");

            if (lines == null)
                throw new ArgumentNullException("lines");

            bool success = true;
            bool allowPartialLines = (m_format != null && !(m_format.FormatFlags == StringFormatFlags.LineLimit));
            float height = result.ActualSize.Height;
            float maxHeight = /*Utils.Round(*/ m_size.Height /*)*/;

            numInserted = 0;

            if (lineResult.Lines != null)
            {
                for (int i = 0, len = lineResult.Lines.Length; i < len; i++)
                {
                    float expHeight = /*Utils.Round(*/ height + lineResult.LineHeight /*)*/;

                    if (expHeight <= maxHeight || maxHeight <= 0 || allowPartialLines)
                    {
                        TextLineInfo info = lineResult.Lines[i];
                        numInserted += info.Line.Length;

                        info = TrimLine(info, (lines.Count == 0));
                        lines.Add(info);

                        // Update width.
                        SizeF size = result.ActualSize;
                        size.Width = Math.Max(size.Width, info.Width);
                        result.ActualSize = size;

                        // The part of the line fits only and it's allowed to use partial lines.
                        if (expHeight >= maxHeight && maxHeight > 0 && allowPartialLines)
                        {
                            bool shouldClip = (m_format == null || !(m_format.FormatFlags == StringFormatFlags.NoClip));

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
                result.ActualSize = size;
            }

            return success;
        }

        /// <summary>
        /// Finalizes final result.
        /// </summary>
        /// <param name="result">Final result.</param>
        /// <param name="lines">Lines array.</param>
        private void SaveResult(StringSplitResult result, List<TextLineInfo> lines)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (lines == null)
                throw new ArgumentNullException("lines");

            result.Lines = (TextLineInfo[])lines.ToArray();
            result.LineHeight = GetLineHeight();

            if (!m_reader.EOF)
            {
                int position = 0;
                if (lines.Count > 0)
                    position = (int)((TextLineInfo)lines[0]).Line.Length;
                result.Remainder = m_text.Substring(position, m_text.Length - position).TrimStart(StringParser.Spaces);
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

            return height;
        }

        /// <summary>
        /// Splits line.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <param name="lineIndent">Line indent.</param>
        private StringSplitResult SplitLine(string line, float lineIndent, ref bool isLastWordFit)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            line = line.Replace("\t", "    ");
            StringSplitResult lineResult = new StringSplitResult();

            lineResult.LineHeight = GetLineHeight();

            List<TextLineInfo> lines = new List<TextLineInfo>();
            float maxWidth = m_size.Width;
            float lineWidth = GetLineWidth(line) + lineIndent;
            TextLineType lineType = TextLineType.FirstParagraphLine;
            bool readWord = true;

            // line is in bounds.
            if ((maxWidth <= 0 || lineWidth <= maxWidth) && maxWidth > 0.0f)
            {
                AddToLineResult(lineResult, lines, line, lineWidth, TextLineType.NewLineBreak | lineType);
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                StringBuilder curLine = new StringBuilder();

                lineWidth = lineIndent;
                float curIndent = lineIndent;

                StringParser reader = new StringParser(line);
                string word = reader.PeekWord();

                if (word.Length != reader.Length)
                {
                    if (word == " ")
                    {
                        reader.Position += 1;
                        word = " " + reader.PeekWord();
                    }
                }

                while (word != null)
                {
                    curLine.Append(word);
                    float curLineWidth = GetLineWidth(curLine.ToString());
                    if (curLineWidth > maxWidth)
                    {
                        Char[] textArr = curLine.ToString().ToCharArray();
                        float minCharacterWidth = 3f;
                        float lastCharWidth = GetLineWidth(textArr[textArr.Length - 1].ToString());
                        if (lastCharWidth < minCharacterWidth)
                        {
                            curLineWidth -= lastCharWidth;
                            isLastWordFit = true;
                        }
                    }

                    if (curLineWidth > maxWidth)
                    {
                        if (GetWrapType() == StringTrimming.None) break;

                        // First word in the line exceeds bounds.
                        if (curLine.Length == word.Length)
                        {
                            //  Character wrap is disabled or one symbol is greater than bounds.
                            int wordCount = line.Split(null).Length;
                            if (GetWrapType() == StringTrimming.Word || wordCount == 1)
                            {
                                lineResult.Remainder = line.Substring(reader.Position);
                                // Starts with space character.
                                if (word.StartsWith(" "))
                                    builder.Append(line.Substring(0, reader.Position));
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
                            if (GetWrapType() != StringTrimming.Character || !readWord)
                            {
                                string ln = builder.ToString();
                                break;

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
                        TextLineType.NewLineBreak | TextLineType.LastParagraphLine);
                    lineResult.Remainder = reader.ReadToEnd();

                }

                reader.Close();
            }

            lineResult.Lines = (TextLineInfo[])lines.ToArray();
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
        private void AddToLineResult(StringSplitResult lineResult, List<TextLineInfo> lines,
            string line, float lineWidth, TextLineType breakType)
        {
            if (lineResult == null)
                throw new ArgumentNullException("lineResult");

            if (lines == null)
                throw new ArgumentNullException("lines");

            if (line == null)
                throw new ArgumentNullException("line");

            TextLineInfo info = new TextLineInfo();

            info.Line = line;
            info.Width = lineWidth;
            info.LineType = breakType;
            lines.Add(info);

            SizeF size = lineResult.ActualSize;

            size.Height += GetLineHeight();
            size.Width = Math.Max(size.Width, lineWidth);
            lineResult.ActualSize = size;
        }

        /// <summary>
        /// Trims whitespaces at the line.
        /// </summary>
        /// <param name="info">Line info.</param>
        /// <param name="firstLine">Indicates whether the line is the first in the text.</param>
        /// <returns>Trimed line info.</returns>
        private TextLineInfo TrimLine(TextLineInfo info, bool firstLine)
        {
            string line = info.Line;
            float lineWidth = info.Width;

            // Trim start whitespaces if the line is not a start of the paragraph only.
            bool trimStartSpaces = ((info.LineType & TextLineType.FirstParagraphLine) == 0);
            bool start = (m_format == null || !(m_format.FormatFlags == StringFormatFlags.DirectionRightToLeft));
            char[] spaces = StringParser.Spaces;

            if (trimStartSpaces)
            {
                line = (start) ? line.TrimStart(spaces) : line.TrimEnd(spaces);
            }

            // Trim end whitespaces.
            bool trimEndSpaces = (m_format == null || !(m_format.FormatFlags == StringFormatFlags.MeasureTrailingSpaces));
            if (trimEndSpaces)
            {
                // If the line is a start of the paragraph and is whitespaces only - leave one space symbol.
                if ((info.LineType & TextLineType.FirstParagraphLine) > 0 && StringParser.IsWhitespace(line))
                {
                    line = new string(StringParser.WhiteSpace, 1);
                }
                else
                {
                    line = (start) ? line.TrimEnd(spaces) : line.TrimStart(spaces);
                }
            }

            // Recalculate line width.
            if (line.Length != info.Line.Length)
            {
                lineWidth = GetLineWidth(line);
                if ((info.LineType & TextLineType.FirstParagraphLine) > 0)
                {
                    lineWidth += GetLineIndent(firstLine);
                }
            }

            info.Line = line;
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
            DrawingContext dc = new DrawingContext();
            if (m_defaultFont.Name != m_font.Name && dc.IsUnicodeText(line))
                return dc.MeasureString(line, m_font, m_defaultFont, null, m_charFormat).Width;
            else
                return dc.MeasureString(line, m_font, null, m_charFormat,false).Width;
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
                lineIndent = (m_size.Width > 0) ? Math.Min(m_size.Width, lineIndent) : lineIndent;
                //lineIndent = Utils.Round( lineIndent );
            }

            return lineIndent;
        }

        /// <summary>
        /// Returns wrap type.
        /// </summary>
        /// <returns>Returns wrap type.</returns>
        private StringTrimming GetWrapType()
        {
            StringTrimming wrapType = (m_format != null) ? m_format.Trimming : StringTrimming.Word;
            return wrapType;
        }
        #endregion
    }

    /// <summary>
    /// Represents the result.
    /// </summary>
    internal class StringSplitResult
    {
        #region Fields
        /// <summary>
        /// Layouted lines.
        /// </summary>
        public TextLineInfo[] Lines;

        /// <summary>
        /// The text wasn't lay outed.
        /// </summary>
        public string Remainder;

        /// <summary>
        /// Actual layouted text bounds.
        /// </summary>
        public SizeF ActualSize;

        /// <summary>
        /// Height of the line.
        /// </summary>
        public float LineHeight;
        #endregion

        #region Properties
        /// <summary>
        /// Gets value that indicates whether any text was layouted.
        /// </summary>
        public bool Empty
        {
            get
            {
                return (Lines == null || Lines.Length == 0);
            }
        }

        /// <summary>
        /// Gets number of the lines layouted.
        /// </summary>
        public int Count
        {
            get
            {
                int count = (!Empty) ? Lines.Length : 0;
                return count;
            }
        }
        #endregion
    }

    /// <summary>
    /// Contains information about the line.
    /// </summary>
    internal struct TextLineInfo
    {
        /// <summary>
        /// Line text.
        /// </summary>
        public string Line;

        /// <summary>
        /// Width of the text.
        /// </summary>
        public float Width;

        /// <summary>
        /// Breaking type of the line.
        /// </summary>
        public TextLineType LineType;
    }

    /// <summary>
    /// 
    /// </summary>
    internal class StringSplitInfo
    {
        #region Class members
        /// <summary>
        /// The position of first symbol
        /// </summary>
        private int m_firstPos = 0;
        /// <summary>
        /// The position of last symbol
        /// </summary>
        private int m_lastPos = 0;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the first position.
        /// </summary>
        /// <value>The first position.</value>
        public int FirstPos
        {
            get
            {
                return m_firstPos;
            }
        }
        /// <summary>
        /// Gets the last position.
        /// </summary>
        /// <value>The last position.</value>
        public int LastPos
        {
            get
            {
                return m_lastPos;
            }
        }
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get
            {
                return LastPos - FirstPos + 1;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Disabled default constructor.
        /// </summary>
        private StringSplitInfo()
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StringSplitInfo"/> class.
        /// </summary>
        /// <param name="firstPos">The first pos.</param>
        /// <param name="lastPos">The last pos.</param>
        public StringSplitInfo(int firstPos, int lastPos)
        {
            if (firstPos < 0)
                throw new ArgumentException("firstPos");
            if (firstPos > lastPos)
                throw new ArgumentException("lastPos");

            m_lastPos = lastPos;
            m_firstPos = firstPos;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Checks the specified length.
        /// </summary>
        /// <param name="length">The length.</param>
        public void Check(int length)
        {
            if (m_firstPos < 0 || m_firstPos > length)
                throw new ArgumentOutOfRangeException("SplitInfo.FirstPos");
            if (m_lastPos < m_firstPos || m_lastPos > length)
                throw new ArgumentOutOfRangeException("SplitInfo.LastPos");
        }
        /// <summary>
        /// Extends the specified STR split info.
        /// </summary>
        /// <param name="strSplitInfo">The STR split info.</param>
        public void Extend(StringSplitInfo strSplitInfo)
        {
            m_firstPos += strSplitInfo.FirstPos;
            m_lastPos += strSplitInfo.FirstPos;
        }
        /// <summary>
        /// Gets the split first part.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public StringSplitInfo GetSplitFirstPart(int position)
        {
            return new StringSplitInfo(m_firstPos, m_firstPos + position - 1);
        }
        /// <summary>
        /// Gets the split second part.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public StringSplitInfo GetSplitSecondPart(int position)
        {
            return new StringSplitInfo(m_firstPos + position, m_lastPos);
        }
        /// <summary>
        /// Gets the substring.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public string GetSubstring(string text)
        {
            return text.Substring(m_firstPos, m_lastPos - m_firstPos + 1);
        }
        #endregion
    }
}

#endif
