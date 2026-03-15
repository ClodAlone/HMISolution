#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Utility class for working with strings.
    /// </summary>
    internal class StringTokenizer
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
        /// Array of spaces.
        /// </summary>
        public static readonly char[] Spaces = new char[] { WhiteSpace, Tab };

        /// <summary>
        /// Default RegEx checks object's options.
        /// </summary>
        private const RegexOptions c_regexOptions =
#if !SILVERLIGHT && !NETFX_CORE && !WP
            RegexOptions.Compiled |
#endif
 RegexOptions.IgnoreCase;

        /// <summary>
        /// Pattern for WhiteSpace.
        /// </summary>
        private const string c_whiteSpacePatterm = @"^[ \t]+$";
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

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StringTokenizer"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public StringTokenizer(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            m_text = text;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this <see cref="StringTokenizer"/> is EOF.
        /// </summary>
        /// <value><c>true</c> if EOF; otherwise, <c>false</c>.</value>
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
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
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


        #region Public methods
        /// <property name="flag" value="Finished" />
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
            {
                throw new ArgumentNullException("wholeText");
            }

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
            {
                throw new ArgumentNullException("text");
            }

            if (symbols == null)
            {
                throw new ArgumentNullException("symbols");
            }

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
                        {
                            if (pos == m_position)
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
            {
                throw new ArgumentNullException("array");
            }

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
}
