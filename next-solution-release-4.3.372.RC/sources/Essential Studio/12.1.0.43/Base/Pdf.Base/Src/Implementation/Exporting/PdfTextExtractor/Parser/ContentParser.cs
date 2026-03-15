#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Pdf
{
    class ContentParser
    {
        private ContentLexer m_lexer;
        private StringBuilder m_operatorParams;
        private PdfRecordCollection m_recordCollection;
        private List<string> m_operands = new List<string>();

        //internal enum PdfOperatorType
        //{
        //   b, B, bx, Bx, BDC, BI, BMC, BT, BX, c, cm, CS, cs, d, d0, d1, Do,
        //DP, EI, EMC, ET, EX, f, F, fx, G, g, gs, h, i, ID, j, J, K, k, l, m, M, MP,
        //n, q, Q, re, RG, rg, ri, s, S, SC, sc, SCN, scn, sh, Tx, Tc, Td, TD, Tf, Tj,
        //TJ, TL, Tm, Tr, Ts, Tw, Tz, v, w, W, Wx, y, 
        //}

        //List of known operators
        static string[] operators = new string[]
        {
        "b", "B", "bx", "Bx", "BDC", "BI", "BMC", "BT", "BX", "c", "cm", "CS", "cs", "d", "d0", "d1", "Do",
        "DP", "EI", "EMC", "ET", "EX", "f", "F", "fx", "G", "g", "gs", "h", "i", "ID", "j", "J", "K", "k", "l", "m", "M", "MP",
        "n", "q", "Q", "re", "RG", "rg", "ri", "s", "S", "SC", "sc", "SCN", "scn", "sh", "f*","Tx", "Tc", "Td", "TD", "Tf", "Tj",
        "TJ", "TL", "Tm", "Tr", "Ts", "Tw", "Tz", "v", "w", "W", "W*", "Wx", "y", "T*", "b*", "B*",
        "\'", "\"", "true"
        };


        public ContentParser(byte[] contentStream)
        {
            this.m_lexer = new ContentLexer(contentStream);
            this.m_operatorParams = m_lexer.OperatorParams;
            m_recordCollection = new PdfRecordCollection();
        }

        public PdfRecordCollection ReadContent()
        {
            ParseObject(TokenType.Eof);
            return m_recordCollection;
        }

        void ParseObject(TokenType stop)
        {
            TokenType symbol;
            while ((symbol = GetNextToken()) != TokenType.Eof)
            {
                if (symbol == stop)
                    return;

                switch (symbol)
                {
                    case TokenType.Comment:                        
                        // comment not needed
                        break;

                    case TokenType.Integer:
                        m_operands.Add(m_operatorParams.ToString());
                        break;

                    case TokenType.Real:
                        m_operands.Add(m_operatorParams.ToString());
                        break;

                    case TokenType.String:
                    case TokenType.HexString:
                    case TokenType.UnicodeString:
                    case TokenType.UnicodeHexString:
                        m_operands.Add(m_operatorParams.ToString());
                        break;

                    case TokenType.Name:
                        m_operands.Add(m_operatorParams.ToString());
                        break;

                    case TokenType.Operator:
                        if (m_operatorParams.ToString() == "ID")
                        {
                            CreateRecord();
                            m_operands.Clear();
                            ConsumeValue();
                            break;
                        }
                        else
                        {
                            CreateRecord();
                            m_operands.Clear();
                            break;
                        }

                    case TokenType.BeginArray:

                        break;

                    case TokenType.EndArray:
                        throw new InvalidOperationException("Error while parsing content");
                }
            }
        }

        private TokenType GetNextToken()
        {
            return m_lexer.GetNextToken();
        }
		private void ConsumeValue()
        {
            char currentChar;
            char nextChar;
            char secondNextChar;
            char thirdChar;
            int contentCount = 0;
            while (true)
            {
                currentChar = m_lexer.GetNextChar();
                if (currentChar == 'E')
                {
                    nextChar = m_lexer.GetNextChar();
                    if (nextChar == 'I')
                    {
                        secondNextChar = m_lexer.GetNextChar();
                        thirdChar = m_lexer.GetNextChar(true);
                        while (thirdChar == ' ' || thirdChar == '\r'||thirdChar=='\n')
                        {
                            thirdChar = m_lexer.GetNextChar();
                            contentCount++;
                        }
                        m_lexer.ResetContentPointer(contentCount);
                        if (secondNextChar == ' ' || secondNextChar == '\n')
                        {
                            if (thirdChar == 'Q')
                            {
                                m_operatorParams.Length = 0;
                                m_operatorParams.Append(currentChar);
                                m_operatorParams.Append(nextChar);
                                CreateRecord();
                                m_operands.Clear();
                                nextChar = m_lexer.GetNextChar();
                                break;
                            }
                        }
                    }
                    else
                    {
                        m_operands.Add(currentChar.ToString());
                        m_operands.Add(nextChar.ToString());
                    }
                }
                else
                {
                    m_operands.Add(currentChar.ToString());
                }
            }
        }
        private void CreateRecord()
        {
            string operand = m_operatorParams.ToString();
            int occurence = Array.IndexOf(operators, operand);
            if (occurence < 0)
            {
                //throw new InvalidOperationException("Unknown Operator");
            }
            PdfRecord record = new PdfRecord(operand, m_operands.ToArray());
            m_recordCollection.Add(record);
        }
    }
}
