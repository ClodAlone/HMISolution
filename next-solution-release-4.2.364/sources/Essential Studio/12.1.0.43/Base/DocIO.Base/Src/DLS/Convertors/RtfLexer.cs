#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.DocIO.DLS.Convertors
{
    public class RtfLexer
    {
        #region Constants
        private const char c_groupStart = '{';
        private const char c_groupEnd = '}';
        private const char c_controlStart = '\\';
        private const char c_space = ' ';
        private const char c_whiteSpace = '\r';
        private const char c_newLine = '\n';
        private const char c_semiColon = ';';
        private const char c_doubleQuotes = '"';
        private const char c_backQuote = '`';
        private const char c_openParenthesis = '(';
        private const char c_closeParenthesis = ')';
        private const char c_ambersion = '&';
        private const char c_percentage = '%';
        private const char c_dollarSign = '$';
        private const char c_hash = '#';
        private const char c_atsymbol = '@';
        private const char c_exclamation = '!';
        private const char c_plus = '+';
        private const char c_caret = '^';
        private const char c_openBracket = '[';
        private const char c_closeBracket = ']';
        private const char c_forwardSlash = '/';
        private const char c_questionmark = '?';
        private const char c_greaterthan = '>';
        private const char c_lesserthan = '<';
        private const char c_comma = ',';
        private const char c_verticalBar = '|';
        private const char c_colon = ':';

        #endregion

        #region Fields
        private RtfTableType m_currRtfTableType;
        RtfReader m_rtfReader;
        private string m_token;
        internal char m_prevChar = '\0';
        private int m_byteStartPosition;
        private int m_byteEndPosition;
        private  bool m_bIsImageBytes = false;
        private bool m_bIsReadNewChar = true;
        private RtfTokenType m_rtfTokenType;
        private char m_newChar = '\0';
        //RTF Delimiters
        private char[] m_delimeters = new char[]{c_groupStart,c_groupEnd,c_controlStart,c_space,c_whiteSpace,c_newLine,c_semiColon,c_doubleQuotes,c_backQuote,
        c_openParenthesis,c_closeParenthesis,c_openBracket,c_closeBracket,c_ambersion,c_percentage,c_dollarSign,c_hash,c_atsymbol,c_exclamation,
        c_plus,c_caret,c_forwardSlash,c_questionmark,c_greaterthan,c_lesserthan,c_comma,c_verticalBar,c_colon};

        #endregion

        #region Properties
        /// <summary>
        /// Gets and Sets the imagebytes
        /// </summary>
        public bool IsImageBytes
        {
            get
            {
                return m_bIsImageBytes;
            }
            set
            {
                m_bIsImageBytes = value;
            }
        }
        /// <summary>
        /// Gets/Sets Rtf table type
        /// </summary>
        public RtfTableType CurrRtfTableType
        {
            get
            {
                return m_currRtfTableType;
            }
            set
            {
                m_currRtfTableType = value;
            }
        }
        /// <summary>
        /// Gets/Sets Rtf token type
        /// </summary>
        public RtfTokenType  CurrRtfTokenType
        {
            get
            {
                return m_rtfTokenType;
            }
            set
            {
                m_rtfTokenType = value;
            }
        }
        #endregion

        #region Constructor
        public RtfLexer(RtfReader rtfReader)
        { 
            m_rtfReader = rtfReader;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Reads the Next token from the stream
        /// </summary>
        /// <returns></returns>
        public string ReadNextToken(string prevTokenKey)
        {
            m_token = null;           
            if (m_bIsReadNewChar)
                m_newChar = m_rtfReader.ReadChar();            
            else
                m_newChar = m_prevChar;
            switch (m_newChar)
            {
                case c_groupStart :
                    {
                        m_bIsReadNewChar = true;
                        return m_newChar.ToString();                       
                    }
                    break;
                case c_groupEnd :
                    {
                        m_bIsReadNewChar = true;
                        return m_newChar.ToString();                     
                    }
                    break;
                case c_controlStart :
                    {
                        m_token = c_controlStart.ToString();
                        m_token = ReadControlWord(m_token, prevTokenKey);
                        return m_token;
                    }
                    break;
                case c_space:
                    {
                        m_token = c_space.ToString();
                        m_token = ReadDocumentElement(m_token, prevTokenKey);
                        return m_token;
                    }
                    break;
                case c_whiteSpace :
                case c_newLine :
                    m_bIsReadNewChar = true;
                    m_token = m_newChar.ToString();
                    return m_token;
                    break;
                default:
                    if (IsImageBytes)
                    {
                        m_token = m_newChar + m_rtfReader.ReadImageBytes();
                        m_bIsReadNewChar = true;
                        return m_token;
                    }
                    else
                    {
                        m_bIsReadNewChar = true;
                        return m_newChar.ToString();
                    }                    
                    break;
            }

        }
        /// <summary>
        /// Reads control word
        /// </summary>
        /// <returns></returns>
        private string ReadControlWord(string token, string prevTokenKey)
        {
            m_newChar = m_rtfReader.ReadChar();
            if (m_newChar == c_controlStart || m_newChar ==c_groupStart || m_newChar ==c_groupEnd)
            {
                m_bIsReadNewChar = true;
                return token + m_newChar;
            }
            m_bIsReadNewChar = false;
            //check whether the character is delimiter or not
            while (System.Array.IndexOf(m_delimeters, m_newChar) == -1)
            {
                if (token.StartsWith("\\u") && token.Length > 2 && char.IsNumber(token[2]) && !char.IsNumber(m_newChar))
                {
                    break;
                }
                token += m_newChar;
                if (m_rtfReader.Position >= m_rtfReader.Length)
                    break;
                m_newChar = m_rtfReader.ReadChar();
                if ((token.StartsWith("\\bin") || prevTokenKey == "bin") && m_newChar == (char)1)
                {
                    m_bIsReadNewChar = true;
                    break;
                }
            }
            m_prevChar = m_newChar;
            if (token == null)
                return m_newChar.ToString();
            else
                return token;
        }
        /// <summary>
        /// Reads Document element
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private string ReadDocumentElement(string token, string prevTokenKey)
        {
            if (IsImageBytes)
            {
                m_newChar = m_rtfReader.ReadChar();
                //Fix to avoid retrieving control word along with image bytes
                if ((m_newChar != c_controlStart) && (m_newChar != c_semiColon) && (m_newChar != c_groupStart) && (m_newChar != c_groupEnd) && m_newChar != c_whiteSpace && m_newChar != c_newLine)
                {
                    token = m_newChar.ToString();
                    if (!(prevTokenKey == "bin" && m_newChar == (char)1))
                        token += m_rtfReader.ReadImageBytes();
                    m_bIsReadNewChar = true;
                    return token;
                }
                else
                {
                    m_bIsReadNewChar = false;
                    m_prevChar = m_newChar;
                    return token;
                }

            }
            else
            {
                m_newChar = m_rtfReader.ReadChar();
                while ((m_newChar != c_controlStart) && (m_newChar != c_semiColon) && (m_newChar != c_groupStart) && (m_newChar != c_groupEnd) && m_newChar != c_whiteSpace && m_newChar != c_newLine)
                {
                    token += m_newChar;
                    if (m_rtfReader.Position >= m_rtfReader.Length)
                        break;
                    m_newChar = m_rtfReader.ReadChar();
                }
                m_prevChar = m_newChar;
                m_bIsReadNewChar = false;
                if (token == null)
                {
                    if (m_prevChar != c_controlStart)
                    {
                        m_bIsReadNewChar = true;
                    }
                    return m_newChar.ToString();

                }
                else
                {
                    return token;
                }
            }

        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            m_rtfReader.Close();
        }
        #endregion
    }
}
