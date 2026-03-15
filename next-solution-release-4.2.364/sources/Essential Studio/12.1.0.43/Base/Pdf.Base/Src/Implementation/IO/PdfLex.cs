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

using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace Syncfusion.Pdf.IO
{
    internal class PdfLexer
    {
        #region Constants
        private const int YY_BUFFER_SIZE = 4096;
        private const int YY_F = -1;
        private const int YY_NO_STATE = -1;
        private const int YY_NOT_ACCEPT = 0;
        private const int YY_START = 1;
        private const int YY_END = 2;
        private const int YY_NO_ANCHOR = 4;
        private const int YY_BOL = 256;
        private const int YY_EOF = 257;
        /// <summary>
        /// Start marker for dictionary.
        /// </summary>
        private const string Prefix = "<<";
        #endregion //Constants

        #region User Code
        #region Fields
        /// <summary>
        /// Holds the current string being read.
        /// </summary>
        private StringBuilder m_string = new StringBuilder();
        /// <summary>
        /// Holds the current parentessis index.
        /// </summary>
        private int m_paren;

        /// <summary>
        /// Holds the current dictionary  is structured or unstructured.
        /// </summary>
        private bool m_bSkip = false;
        #endregion //Fields

        #region Properties
        /// <summary>
        /// Returns the current text value.
        /// </summary>
        internal string Text
        {
            get
            {
                return YyText();
            }
        }
        /// <summary>
        /// Gets current line number.
        /// </summary>
        internal int Line
        {
            get
            {
                return m_yyLine;
            }
        }
        /// <summary>
        /// Gets the current string being read.
        /// </summary>
        internal StringBuilder StringText
        {
            get
            {
                return m_string;
            }
        }
        /// <summary>
        /// Returns file position.
        /// </summary>
        internal int Position
        {
            get
            {
                int val = (int)((m_yyReader as PdfReader).Position) - m_yyBufferRead + m_yyBufferIndex;
                return val;
            }
        }

        /// <summary>
        /// Gets or sets the current dictionary is structured or unstructured.
        /// </summary>
        internal bool Skip
        {
            get
            {
                return m_bSkip;
            }
            set
            {
                m_bSkip = value;
            }
        }
        #endregion //Properties

        #region Public methods
        /// <summary>
        /// Resets the lexer.
        /// </summary>
        internal void Reset()
        {
            m_yyBuffer = new char[YY_BUFFER_SIZE];
            m_yyBufferRead = 0;
            m_yyBufferIndex = 0;
            m_yyBufferStart = 0;
            m_yyBufferEnd = 0;
            //m_yyChar = 0;
            m_yyLine = 0;
            m_yyAtBol = true;
            m_yyLexicalState = State.YYINITIAL;
        }
        /// <summary>
        /// Reads the count bytes from the stream.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>
        /// The data which were read from the stream.
        /// </returns>
        internal byte[] Read(int count)
        {
            List<byte> list = new List<byte>(count);
            YyMarkStart();

            if (m_yyBufferRead - m_yyBufferStart < count)
            {
                while (m_yyBuffer.Length - m_yyBufferStart < count)
                {
                    m_yyBuffer = YyDouble(m_yyBuffer);
                }
            }
            int readCount = YyRead();

            if (m_yyBufferRead - m_yyBufferStart < readCount)
            {
                if (readCount > count)
                {
                    count = readCount;
                }
            }
            for (int i = m_yyBufferStart; i < m_yyBufferStart + count; ++i, m_yyBufferIndex = i)
            {
                list.Add((byte)m_yyBuffer[i]);
            }
            YyMarkStart();
            YyMarkEnd();

            return list.ToArray();
        }
        /// <summary>
        /// Skips the new line.
        /// </summary>
        internal void SkipNewLine()
        {
            m_yyBufferIndex = m_yyBufferStart + 1;

            if (m_yyBuffer[m_yyBufferIndex] == '\r')
            {
                if (m_yyBuffer[m_yyBufferIndex + 1] == '\n')
                {
                    m_yyBufferIndex += 2;
                }
            }
            else if (m_yyBuffer[m_yyBufferIndex] == '\n')
            {
                m_yyBufferIndex += 1;
            }
            YyMarkStart();
        }
        /// <summary>
        /// Skips the token.
        /// </summary>
        internal void SkipToken()
        {
            m_yyBufferStart = m_yyBufferEnd;
        }
        #endregion // Public methods
        #endregion //User Code

        #region Fields
        private System.IO.TextReader m_yyReader;
        private int m_yyBufferIndex;
        private int m_yyBufferRead;
        private int m_yyBufferStart;
        private int m_yyBufferEnd;
        private char[] m_yyBuffer;
        private int m_yyLine;
        private bool TType = true;
        private bool m_yyAtBol;
        private State m_yyLexicalState;
        private static readonly int[] m_yyStateDtrans = new int[] {
			0,
			81,
			83
		};
        private bool m_yyLastWasCr = false;

        private string[] m_yyErrorString = {
		"Error: Internal error.\n",
		"Error: Unmatched input.\n"
		};

        private int[] m_yyAccept = {
			/* 0 */ YY_NOT_ACCEPT,
			/* 1 */ YY_NO_ANCHOR,
			/* 2 */ YY_NO_ANCHOR,
			/* 3 */ YY_NO_ANCHOR,
			/* 4 */ YY_NO_ANCHOR,
			/* 5 */ YY_NO_ANCHOR,
			/* 6 */ YY_NO_ANCHOR,
			/* 7 */ YY_NO_ANCHOR,
			/* 8 */ YY_NO_ANCHOR,
			/* 9 */ YY_NO_ANCHOR,
			/* 10 */ YY_NO_ANCHOR,
			/* 11 */ YY_NO_ANCHOR,
			/* 12 */ YY_NO_ANCHOR,
			/* 13 */ YY_NO_ANCHOR,
			/* 14 */ YY_NO_ANCHOR,
			/* 15 */ YY_NO_ANCHOR,
			/* 16 */ YY_NO_ANCHOR,
			/* 17 */ YY_NO_ANCHOR,
			/* 18 */ YY_NO_ANCHOR,
			/* 19 */ YY_NO_ANCHOR,
			/* 20 */ YY_NO_ANCHOR,
			/* 21 */ YY_NO_ANCHOR,
			/* 22 */ YY_NO_ANCHOR,
			/* 23 */ YY_NO_ANCHOR,
			/* 24 */ YY_NO_ANCHOR,
			/* 25 */ YY_NO_ANCHOR,
			/* 26 */ YY_NO_ANCHOR,
			/* 27 */ YY_NO_ANCHOR,
			/* 28 */ YY_NO_ANCHOR,
			/* 29 */ YY_NO_ANCHOR,
			/* 30 */ YY_NO_ANCHOR,
			/* 31 */ YY_NO_ANCHOR,
			/* 32 */ YY_NO_ANCHOR,
			/* 33 */ YY_NO_ANCHOR,
			/* 34 */ YY_NO_ANCHOR,
			/* 35 */ YY_NO_ANCHOR,
			/* 36 */ YY_NOT_ACCEPT,
			/* 37 */ YY_NO_ANCHOR,
			/* 38 */ YY_NO_ANCHOR,
			/* 39 */ YY_NO_ANCHOR,
			/* 40 */ YY_NO_ANCHOR,
			/* 41 */ YY_NOT_ACCEPT,
			/* 42 */ YY_NO_ANCHOR,
			/* 43 */ YY_NOT_ACCEPT,
			/* 44 */ YY_NO_ANCHOR,
			/* 45 */ YY_NOT_ACCEPT,
			/* 46 */ YY_NO_ANCHOR,
			/* 47 */ YY_NOT_ACCEPT,
			/* 48 */ YY_NO_ANCHOR,
			/* 49 */ YY_NOT_ACCEPT,
			/* 50 */ YY_NO_ANCHOR,
			/* 51 */ YY_NOT_ACCEPT,
			/* 52 */ YY_NO_ANCHOR,
			/* 53 */ YY_NOT_ACCEPT,
			/* 54 */ YY_NO_ANCHOR,
			/* 55 */ YY_NOT_ACCEPT,
			/* 56 */ YY_NOT_ACCEPT,
			/* 57 */ YY_NOT_ACCEPT,
			/* 58 */ YY_NOT_ACCEPT,
			/* 59 */ YY_NOT_ACCEPT,
			/* 60 */ YY_NOT_ACCEPT,
			/* 61 */ YY_NOT_ACCEPT,
			/* 62 */ YY_NOT_ACCEPT,
			/* 63 */ YY_NOT_ACCEPT,
			/* 64 */ YY_NOT_ACCEPT,
			/* 65 */ YY_NOT_ACCEPT,
			/* 66 */ YY_NOT_ACCEPT,
			/* 67 */ YY_NOT_ACCEPT,
			/* 68 */ YY_NOT_ACCEPT,
			/* 69 */ YY_NOT_ACCEPT,
			/* 70 */ YY_NOT_ACCEPT,
			/* 71 */ YY_NOT_ACCEPT,
			/* 72 */ YY_NOT_ACCEPT,
			/* 73 */ YY_NOT_ACCEPT,
			/* 74 */ YY_NOT_ACCEPT,
			/* 75 */ YY_NOT_ACCEPT,
			/* 76 */ YY_NOT_ACCEPT,
			/* 77 */ YY_NOT_ACCEPT,
			/* 78 */ YY_NOT_ACCEPT,
			/* 79 */ YY_NOT_ACCEPT,
			/* 80 */ YY_NOT_ACCEPT,
			/* 81 */ YY_NOT_ACCEPT,
			/* 82 */ YY_NOT_ACCEPT,
			/* 83 */ YY_NOT_ACCEPT,
			/* 84 */ YY_NOT_ACCEPT,
			/* 85 */ YY_NOT_ACCEPT,
			/* 86 */ YY_NOT_ACCEPT,
			/* 87 */ YY_NOT_ACCEPT
		};
        private int[] m_yyCmap = UnpackFromString(1, 258,
            "3,17:8,3,11,17,3,4,17:18,3,17:4,1,17:2,7,2,17,26,17,26,28,16,27:10,17:2,5,1" +
            "7,6,17:2,13:6,17:11,35,17:8,14,12,15,17:3,23,30,13,33,21,22,17:2,36,31,17,2" +
            "4,34,32,29,17:2,19,25,18,20,17:2,37,17:2,10,17,10,17:128,8,9,0:2")[0];

        private int[] m_yyRmap = UnpackFromString(1, 88,
            "0,1,2,1:2,3,4,1:2,5,6,7,1:3,8,1:18,9,1,10,11,12,13,14,15,16,17,18,19,20,21," +
            "7,8:2,22,23,24,25,13,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43," +
            "44,45,46,47,48,49,50,51,52,53,54,55,56,57")[0];

        private int[][] m_yyNext = UnpackFromString(58, 38,
            "1,2,3,4:2,5,37,6,3:3,4,3:2,7,8,9,3,42,3:2,44,10,3:2,46,48,11,50,52,3:2,38,3" +
            ":2,12,3,54,-1:39,2:3,-1,2:6,-1,2:26,-1:5,13,-1:40,36,-1:37,9:2,-1:2,9:2,-1:" +
            "3,9:21,-1:23,45,-1:41,11,49,-1:36,15,-1:11,35:3,84,35:33,-1:9,55,-1:34,14,-" +
            "1:51,85,-1:18,63,17,63:8,64,63:26,-1,30:3,82,30:33,-1:20,56,-1:2,57,-1:33,4" +
            "1,-1:51,58,-1:36,43,-1:29,59,-1:31,47,-1:38,86,-1:3,60,-1:45,16,-1:36,51,-1" +
            ":28,62,-1:35,53,-1:39,18,-1:52,65,-1:26,66,-1:3,67,-1:33,56,-1:31,87,-1:42," +
            "19,-1:35,20,-1:16,55:3,-1,55:6,-1,55:26,-1,64,39,64,63,64:33,-1:24,69,-1:31" +
            ",70,-1:49,71,-1:30,72,-1:35,74,-1:35,75,-1:49,21,-1:40,22,-1:40,76,-1:19,23" +
            ",-1:39,77,-1:35,78,-1:41,79,-1:35,80,-1:50,24,-1:25,25,-1:15,1,26:2,27:2,26" +
            ",28,26:4,27,40,29,26:7,29:3,26:3,29,26:2,29,26:2,29,26:4,-1:11,30,-1:26,1,3" +
            "1,32,31:4,33,31:4,34,31:25,-1:11,35,-1:50,61,-1:34,68,-1:34,73,-1:19");


        #endregion //Fields

        #region Initialize / Finalize
        internal PdfLexer(System.IO.TextReader reader)
            : this()
        {
            if (null == reader)
            {
                throw (new System.Exception("Error: Bad input stream initializer."));
            }
            m_yyReader = reader;
        }

        internal PdfLexer(System.IO.Stream inStream)
            : this()
        {
            if (null == inStream)
            {
                throw (new System.Exception("Error: Bad input stream initializer."));
            }
            m_yyReader = new System.IO.StreamReader(inStream);
        }

        private PdfLexer()
        {
            m_yyBuffer = new char[YY_BUFFER_SIZE];
            m_yyBufferRead = 0;
            m_yyBufferIndex = 0;
            m_yyBufferStart = 0;
            m_yyBufferEnd = 0;
            m_yyLine = 0;
            m_yyAtBol = true;
            m_yyLexicalState = State.YYINITIAL;
        }
        #endregion //Initialize / Finalize

        #region Public Methods
        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <returns></returns>
        public TokenType GetNextToken()
        {
            int yyLookAhead;
            int yyAnchor = YY_NO_ANCHOR;
            int yyState = m_yyStateDtrans[(int)m_yyLexicalState];
            int yyNextState = YY_NO_STATE;
            int yyLastAcceptState = YY_NO_STATE;
            bool yyInitial = true;
            int yyThisAccept;

            YyMarkStart();
            yyThisAccept = m_yyAccept[yyState];

            if (YY_NOT_ACCEPT != yyThisAccept)
            {
                yyLastAcceptState = yyState;
                YyMarkEnd();
            }

            while (true)
            {
                if (yyInitial && m_yyAtBol)
                {
                    yyLookAhead = YY_BOL;
                }
                else
                {
                    yyLookAhead = YyAdvance();
                }

                yyNextState = YY_F;
                yyNextState = m_yyNext[m_yyRmap[yyState]][m_yyCmap[yyLookAhead]];
                if (YY_EOF == yyLookAhead && yyInitial)
                {
                    return TokenType.Eof;
                }
                if (YY_F != yyNextState)
                {
                    yyState = yyNextState;
                    yyInitial = false;
                    yyThisAccept = m_yyAccept[yyState];
                    if (YY_NOT_ACCEPT != yyThisAccept)
                    {
                        yyLastAcceptState = yyState;
                        YyMarkEnd();
                    }
                }
                else
                {
                    if (YY_NO_STATE == yyLastAcceptState)
                    {
                        throw (new System.Exception("Lexical Error: Unmatched Input."));
                    }
                    else
                    {
                        yyAnchor = m_yyAccept[yyLastAcceptState];

                        if (0 != (YY_END & yyAnchor))
                        {
                            YyMoveEnd();
                        }
                        YyToMark();

                        switch (yyLastAcceptState)
                        {
                            case 1:
                                break;

                            case -2:
                                break;

                            case 2:
                                {// Skip comment
                                    break;
                                }

                            case -3:
                                break;

                            case 3:
                                {
                                    //if (TType)
                                    //{
                                    //    TType = false;
                                    //    return TokenType.StreamEnd;
                                    //}
                                    //else
                                    //{
                                    //    TType = true;
                                    //    return TokenType.ObjectEnd;
                                    //}
                                    //Console.WriteLine("File: '{0}'", (char)yyLookAhead);
                                    //YyError(YYError.Match, true);
                                    //if (YyText() != ")")
                                    //    return TokenType.Unknown;
                                    break;
                                }

                            case -4:
                                break;

                            case 4:
                                {// Skip whitespace.
                                    break;
                                }

                            case -5:
                                break;

                            case 5:
                                {
                                    YyBegin(State.HexString);
                                    return TokenType.HexStringStart;
                                }

                            case -6:
                                break;

                            case 6:
                                {
                                    // ordinary string.
                                    YyBegin(State.String);
                                    StringText.Length = 0;
                                    break;
                                }

                            case -7:
                                break;

                            case 7:
                                {
                                    return TokenType.ArrayStart;
                                }

                            case -8:
                                break;

                            case 8:
                                {
                                    return TokenType.ArrayEnd;
                                }

                            case -9:
                                break;

                            case 9:
                                {
                                    return TokenType.Name;
                                }

                            case -10:
                                break;

                            case 10:
                                {
                                    return TokenType.ObjectType;
                                }

                            case -11:
                                break;

                            case 11:
                                {
                                    //string bufferNext = string.Empty;
                                    //if (m_yyBuffer.Length > (m_yyBufferStart + (m_yyBufferEnd - m_yyBufferStart)) + 2)
                                    //{
                                    //    bufferNext = new string(m_yyBuffer, m_yyBufferStart + (m_yyBufferEnd - m_yyBufferStart), 2);
                                    //}
                                    //if (bufferNext == Prefix)
                                    //    break;
                                    return TokenType.Number;
                                }

                            case -12:
                                break;

                            case 12:
                                {
                                    return TokenType.Reference;
                                }

                            case -13:
                                break;

                            case 13:
                                {
                                    return TokenType.DictionaryStart;
                                }

                            case -14:
                                break;

                            case 14:
                                {
                                    return TokenType.DictionaryEnd;
                                }

                            case -15:
                                break;

                            case 15:
                                {
                                    return TokenType.Real;
                                }

                            case -16:
                                break;

                            case 16:
                                {
                                    return TokenType.ObjectStart;
                                }

                            case -17:
                                break;

                            case 17:
                                {
                                    // A unicode string.
                                    return TokenType.UnicodeString;
                                }

                            case -18:
                                break;

                            case 18:
                                {
                                    return TokenType.Boolean;
                                }

                            case -19:
                                break;

                            case 19:
                                {
                                    return TokenType.Null;
                                }

                            case -20:
                                break;

                            case 20:
                                {
                                    return TokenType.XRef;
                                }

                            case -21:
                                break;

                            case 21:
                                {
                                    return TokenType.ObjectEnd;
                                }

                            case -22:
                                break;

                            case 22:
                                {
                                    return TokenType.StreamStart;
                                }

                            case -23:
                                break;

                            case 23:
                                {
                                    return TokenType.Trailer;
                                }

                            case -24:
                                break;

                            case 24:
                                {
                                    return TokenType.StreamEnd;
                                }

                            case -25:
                                break;

                            case 25:
                                {
                                    return TokenType.StartXRef;
                                }

                            case -26:
                                break;

                            case 26:
                                {
                                    return TokenType.HexStringWeird;
                                }

                            case -27:
                                break;

                            case 27:
                                {
                                    return TokenType.WhiteSpace;
                                }

                            case -28:
                                break;

                            case 28:
                                {
                                    YyBegin(State.YYINITIAL);
                                    return TokenType.HexStringEnd;
                                }

                            case -29:
                                break;

                            case 29:
                                {
                                    return TokenType.HexDigit;
                                }

                            case -30:
                                break;

                            case 30:
                                {
                                    // Escape sequence.
                                    return TokenType.HexStringWeirdEscape;
                                }

                            case -31:
                                break;

                            case 31:
                                {
                                    // Other characters.
                                    StringText.Append(YyText());
                                    break;
                                }

                            case -32:
                                break;

                            case 32:
                                {
                                    if (m_paren > 0) // Balanced parentessis.
                                    {
                                        StringText.Append(YyText());
                                        --m_paren;
                                    }
                                    else // Real end of the string.
                                    {
                                        YyBegin(State.YYINITIAL);
                                        return TokenType.String;
                                    }
                                    break;
                                }

                            case -33:
                                break;

                            case 33:
                                {
                                    StringText.Append(YyText());
                                    ++m_paren;
                                    break;
                                }

                            case -34:
                                break;

                            case 34:
                                {
                                    break;
                                }

                            case -35:
                                break;

                            case 35:
                                {
                                    // Escape sequence.
                                    StringText.Append(YyText());
                                    break;
                                }

                            case -36:
                                break;

                            case 37:
                                {
                                    //Console.WriteLine("File: '{0}'", (char)yyLookAhead);
                                    YyError(YYError.Match, true);
                                    break;
                                }

                            case -37:
                                break;

                            case 38:
                                {
                                    return TokenType.ObjectType;
                                }

                            case -38:
                                break;

                            case 39:
                                {
                                    // A unicode string.
                                    return TokenType.UnicodeString;
                                }

                            case -39:
                                break;

                            case 40:
                                {
                                    return TokenType.HexStringWeird;
                                }

                            case -40:
                                break;

                            case 42:
                                {
                                    return TokenType.Unknown;
                                }

                            case -41:
                                break;

                            case 44:
                                return TokenType.Unknown;

                            case -42:
                                break;

                            case 46:
                                {
                                    //Console.WriteLine("File: '{0}'", (char)yyLookAhead);
                                    YyError(YYError.Match, true);
                                    break;
                                }

                            case -43:
                                break;

                            case 48:
                                {
                                    //Console.WriteLine("File: '{0}'", (char)yyLookAhead);
                                    YyError(YYError.Match, true);
                                    break;
                                }

                            case -44:
                                break;

                            case 50:
                                {
                                    //Console.WriteLine("File: '{0}'", (char)yyLookAhead);
                                    YyError(YYError.Match, true);
                                    break;
                                }

                            case -45:
                                break;

                            case 52:
                                {
                                    //Console.WriteLine("File: '{0}'", (char)yyLookAhead);
                                    //YyError(YYError.Match, true);
                                    break;
                                }

                            case -46:
                                break;

                            case 54:
                                {
                                    //Console.WriteLine("File: '{0}'", (char)yyLookAhead);
                                    YyError(YYError.Match, true);
                                    break;
                                }

                            case -47:
                                break;

                            default:
                                YyError(YYError.Internal, false);
                                break;
                        }
                        yyInitial = true;
                        yyState = m_yyStateDtrans[(int)m_yyLexicalState];
                        yyNextState = YY_NO_STATE;
                        yyLastAcceptState = YY_NO_STATE;
                        YyMarkStart();
                        yyThisAccept = m_yyAccept[yyState];

                        if (YY_NOT_ACCEPT != yyThisAccept)
                        {
                            yyLastAcceptState = yyState;
                            YyMarkEnd();
                        }
                    }
                }
            }
           
        }
        #endregion //Public Methods

        #region Helper Methods
        private void YyBegin(State state)
        {
            m_yyLexicalState = state;
        }

        private int YyAdvance()
        {
            if (m_yyBufferIndex < m_yyBufferRead)
            {
                return m_yyBuffer[m_yyBufferIndex++];
            }

            int nextRead;

            if (0 != m_yyBufferStart)
            {
                int i = m_yyBufferStart;
                int j = 0;

                while (i < m_yyBufferRead)
                {
                    m_yyBuffer[j] = m_yyBuffer[i];
                    ++i;
                    ++j;
                }

                m_yyBufferEnd -= m_yyBufferStart;
                m_yyBufferStart = 0;
                m_yyBufferRead = j;
                m_yyBufferIndex = j;

                nextRead = YyRead();

                if (nextRead <= 0)
                {
                    return YY_EOF;
                }
            }

            while (m_yyBufferIndex >= m_yyBufferRead)
            {
                if (m_yyBufferIndex >= m_yyBuffer.Length)
                {
                    m_yyBuffer = YyDouble(m_yyBuffer);
                }

                nextRead = YyRead();

                if (nextRead <= 0)
                {
                    return YY_EOF;
                }
            }
            return m_yyBuffer[m_yyBufferIndex++];
        }

        private int YyRead()
        {
            int nextRead = m_yyReader.Read(m_yyBuffer, m_yyBufferRead, m_yyBuffer.Length - m_yyBufferRead);

            if (nextRead > 0)
            {
                m_yyBufferRead += nextRead;
            }

            return nextRead;
        }

        private void YyMoveEnd()
        {
            if (m_yyBufferEnd > m_yyBufferStart && '\n' == m_yyBuffer[m_yyBufferEnd - 1])
            {
                m_yyBufferEnd--;
            }

            if (m_yyBufferEnd > m_yyBufferStart && '\r' == m_yyBuffer[m_yyBufferEnd - 1])
            {
                m_yyBufferEnd--;
            }
        }

        private void YyMarkStart()
        {
            for (int i = m_yyBufferStart; i < m_yyBufferIndex; ++i)
            {
                if ('\n' == m_yyBuffer[i] && !m_yyLastWasCr)
                {
                    ++m_yyLine;
                }

                if ('\r' == m_yyBuffer[i])
                {
                    ++m_yyLine;
                    m_yyLastWasCr = true;
                }
                else
                {
                    m_yyLastWasCr = false;
                }
            }
            m_yyBufferStart = m_yyBufferIndex;
        }

        private void YyMarkEnd()
        {
            m_yyBufferEnd = m_yyBufferIndex;
        }

        private void YyToMark()
        {
            m_yyBufferIndex = m_yyBufferEnd;
            m_yyAtBol = (m_yyBufferEnd > m_yyBufferStart) &&
            ('\r' == m_yyBuffer[m_yyBufferEnd - 1] ||
            '\n' == m_yyBuffer[m_yyBufferEnd - 1] ||
            2028/*LS*/ == m_yyBuffer[m_yyBufferEnd - 1] ||
            2029/*PS*/ == m_yyBuffer[m_yyBufferEnd - 1]);
        }

        private string YyText()
        {
            if ((m_yyBuffer.Length > 2) && (m_yyBufferEnd > 2))
            {
                char end = m_yyBuffer[m_yyBufferEnd - 1];
                char start = m_yyBuffer[m_yyBufferEnd - 2];
                int value = m_yyBufferEnd - m_yyBufferStart;
                if ((end == ')') && (start == '\\' || start == '\0') && value > 3)
                {
                    int index = m_yyBufferEnd;
                    string text = new string(m_yyBuffer);
                    index = text.IndexOf(end, m_yyBufferStart) + 1;
                    while (text[index-2]=='\\')
                    {
                        index = text.IndexOf(end, index) + 1;
                    }
                    if (text[index] == '>' && text[index + 1] == '>')
                    {
                        m_yyBufferIndex = index;
                        Skip = false;
                    }
                    else if (text.Length > index + 2)
                    {
                        if (text[index + 2] == '/')
                        {
                            m_yyBufferIndex = index;
                            Skip = false;
                        }
                        else if (text[index + 1] == '/')
                        {
                            m_yyBufferIndex = index;
                            Skip = false;
                        }
                        else
                            if (text[index] == '/')
                            {
                                m_yyBufferIndex = index;
                                Skip = false;
                            }
                            else
                            Skip = true;
                    }
                    else
                        Skip = true;
                    m_yyBufferEnd = index;
                }
                else if (end == ')' && value > 3)
                {
                    int index = m_yyBufferEnd;
                    string text = new string(m_yyBuffer);
                    index = text.IndexOf(end, m_yyBufferStart) + 1;
                    while (text[index - 2] == '\\')
                    {
                        index = text.IndexOf(end, index) + 1;
                    }
                    if (text[index - 1] == ')')
                    {
                        m_yyBufferIndex = index - 1;
                        Skip = false;
                    }
                    else
                        Skip = true;
                }
            }
            
            return (new string(m_yyBuffer, m_yyBufferStart, m_yyBufferEnd - m_yyBufferStart));
        }

        private int YyLength()
        {
            return m_yyBufferEnd - m_yyBufferStart;
        }

        private char[] YyDouble(char[] buffer)
        {
            int length = buffer.Length;
            char[] newBuffer = new char[2 * length];

            int charsize = 2; // sizeof( char ) //Doesn't work in Framework 1.x
            System.Buffer.BlockCopy(buffer, 0, newBuffer, 0, length * charsize);
            return newBuffer;
        }

        private void YyError(YYError code, bool fatal)
        {
            //System.Console.Write(m_yyErrorString[(int)code]);
            //System.Console.Out.Flush();

            if (fatal)
            {
                int position = Position;
                throw new System.Exception(string.Format("Fatal Error at {0}.\n", position));
            }
        }

        private static int[][] UnpackFromString(int size1, int size2, string st)
        {
            int colonIndex = -1;
            string lengthString;
            int sequenceLength = 0;
            int sequenceInteger = 0;

            int commaIndex;
            string workString;

            int[][] res = new int[size1][];

            for (int i = 0; i < size1; ++i)
            {
                res[i] = new int[size2];
            }

            for (int i = 0; i < size1; ++i)
            {
                for (int j = 0; j < size2; ++j)
                {
                    if (sequenceLength != 0)
                    {
                        res[i][j] = sequenceInteger;
                        --sequenceLength;
                        continue;
                    }
                    commaIndex = st.IndexOf(',');
                    workString = (commaIndex == -1) ? st : st.Substring(0, commaIndex);
                    st = st.Substring(commaIndex + 1);
                    colonIndex = workString.IndexOf(':');

                    if (colonIndex == -1)
                    {
                        res[i][j] = System.Int32.Parse(workString);
                        continue;
                    }
                    lengthString = workString.Substring(colonIndex + 1);
                    sequenceLength = System.Int32.Parse(lengthString);
                    workString = workString.Substring(0, colonIndex);
                    sequenceInteger = System.Int32.Parse(workString);
                    res[i][j] = sequenceInteger;
                    --sequenceLength;
                }
            }
            return res;
        }
        #endregion //Helper Methods

        #region Internals
        private enum YYError
        {
            Internal = 0,
            Match = 1,
        }

        private enum State
        {
            HexString = 1,
            String = 2,
            YYINITIAL = 0,
        }
        #endregion //Internals

    }
}
