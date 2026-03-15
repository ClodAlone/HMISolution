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
    internal enum TokenType
    {
        None,
        Comment,
        Integer,
        Real,
        String,
        HexString,
        UnicodeString,
        UnicodeHexString,
        Name,
        Operator,
        BeginArray,
        EndArray,
        Eof,
    }

    internal class ContentLexer
    {
        private StringBuilder m_operatorParams = new StringBuilder();
        private TokenType m_tType = TokenType.None;
        private char m_currentChar;
        private char m_nextChar;
        private byte[] m_contentStream;
        private int m_charPointer;

        static string[] m_textShowers = new string[] { "Tj", "\'", "TJ", "\"" };

        internal TokenType Token
        {
            get
            {
                return m_tType;
            }

        }

        internal StringBuilder OperatorParams
        {
            get
            {
                return m_operatorParams;
            }
        }

        public ContentLexer(byte[] contentStream)
        {
            this.m_contentStream = contentStream;
        }

        public TokenType GetNextToken()
        {
            ResetToken();
            char ch = MoveToNextChar();
            switch (ch)
            {
                case '%':
                    return this.m_tType = GetComment();

                case '/':
                    return this.m_tType = GetName();

                case '+':
                case '-':
                    return this.m_tType = GetNumber();

                case '[':

                case '(':
                    return this.m_tType = GetLiteralString();

                case '<':
                    return this.m_tType = GetHexadecimalString();

                case '.':
                    return this.m_tType = GetNumber();

                case '"':
                case '\'':
                    return this.m_tType = GetOperator();
            }
            if (Char.IsDigit(ch))
                return this.m_tType = GetNumber();

            if (Char.IsLetter(ch))
                return this.m_tType = GetOperator();

            if (ch == (char)65535)
                return this.m_tType = TokenType.Eof;

            return TokenType.None;
        }

        private void ResetToken()
        {
            m_operatorParams.Length = 0;
        }

        private char MoveToNextChar()
        {
            while (this.m_currentChar != (char)65535)
            {
                switch (this.m_currentChar)
                {
                    case '\0':
                    case '\t':
                    case '\x0A':
                    case '\f':
                    case '\x0D':
                    case '\b':
                    case ' ':
                        GetNextChar(); // line feeds and spaces
                        break;

                    default:
                        return m_currentChar;
                }
            }
            return m_currentChar;
        }

        internal void ResetContentPointer(int count)
        {
            m_charPointer = m_charPointer - count;
        }
        internal char GetNextChar()
        {
            if (this.m_contentStream.Length <= this.m_charPointer)
            {
                this.m_currentChar =(char)65535;
                this.m_nextChar =(char)65535;
            }
            else
            {
                this.m_currentChar = this.m_nextChar;
                this.m_nextChar = (char)this.m_contentStream[this.m_charPointer++];
                if (this.m_currentChar == '\x0D')
                {
                    if (this.m_nextChar == '\x0A')
                    {
                        this.m_currentChar = this.m_nextChar;
                        if (m_contentStream.Length <= this.m_charPointer)
                            this.m_nextChar = (char)65535;
                        else
                            this.m_nextChar = (char)this.m_contentStream[this.m_charPointer++];
                    }
                    else
                    {
                        this.m_currentChar = '\x0A';
                    }
                }
            }
            return m_currentChar;
        }

        internal char GetNextChar(bool value)
        {
            return this.m_nextChar;
        }

        private TokenType GetComment()
        {
            ResetToken();
            char ch;
            while ((ch = ConsumeValue()) != '\x0A' && ch != (char)65535)
            {
            }
            return TokenType.Comment;
        }

        private TokenType GetName()
        {
            ResetToken();

            while (true)
            {
                char ch = ConsumeValue();
                if (IsWhiteSpace(ch) || IsDelimiter(ch))
                    break;
            }
            return TokenType.Name;
        }

        private TokenType GetNumber()
        {
            char ch = this.m_currentChar;
            if (ch == '+' || ch == '-')
            {
                this.m_operatorParams.Append(this.m_currentChar);
                ch = GetNextChar();
            }
            while (true)
            {
                if (Char.IsDigit(ch))
                {
                    this.m_operatorParams.Append(this.m_currentChar);
                }
                else if (ch == '.')
                {
                    this.m_operatorParams.Append(this.m_currentChar);
                }
                else
                    break;
                ch = GetNextChar();
            }           
            return TokenType.Integer;
        }

        private TokenType GetLiteralString()
        {
            char beginChar;
            ResetToken();
           
            if (this.m_currentChar == '(')
            {
                beginChar = m_currentChar;
            }
            else
            {
                beginChar = m_currentChar;
            }
            string literal;
            char ch = ConsumeValue();
            while (true)
            {
                if (beginChar == '(')
                {
                    literal = GetLiterals(ch);
                    m_operatorParams.Append(literal);
                    ch = GetNextChar();
                    break;
                }
                else
                {
                    if (ch == '(')
                    {
                        ch = ConsumeValue();
                        literal = GetLiterals(ch);
                        m_operatorParams.Append(literal);
                        ch = GetNextChar();
                        continue;
                    }
                    else if(ch==']')
                    {
                        ch = ConsumeValue();
                        break;
                    }
                    ch = ConsumeValue();                   
                }                
            }
            return TokenType.String;
        }

        private string GetLiterals(char ch)
        {
            int parenthesesCount = 0;
            string literal = "";
            while (true)
            {
                if (ch == '\\')
                {
                    literal += ch.ToString();
                    ch = GetNextChar();
                    literal += ch.ToString();
                    ch = GetNextChar();
                    continue;
                }

                if (ch == '(')
                {
                    parenthesesCount++;
                    literal += ch.ToString();
                    ch = GetNextChar();
                    continue;
                }

                if (ch == ')' && parenthesesCount != 0)
                {
                    literal += ch.ToString();
                    ch = GetNextChar();
                    parenthesesCount--;
                    continue;
                }

                if (ch == ')' && parenthesesCount == 0)
                {
                    literal += ch.ToString();
                    return literal;
                }
                literal += ch.ToString();
                ch = GetNextChar();
            }
        }

        private TokenType GetHexadecimalString()
        {
            char startChar = '<';
            char endChar = '>';
            int parentLevel =0;
            char ch= ConsumeValue();
            while (true)
            {
                if (ch == startChar)
                {
                    parentLevel++;
                    ch = ConsumeValue();
                }
                else if (ch == endChar)
                {
                    if (parentLevel == 0)
                    {
                        ConsumeValue();
                        break;
                    }
                    else
                    {
                        ch = ConsumeValue();
                        if(ch.Equals('>'))
                        parentLevel--;
                    }
                }
                else
                {
                    ch = ConsumeValue();
                }

            }

            return TokenType.HexString;
        }

        private TokenType GetOperator()
        {
            ResetToken();
            char ch = this.m_currentChar;

            while (IsOperator(ch))
                ch = ConsumeValue();

            return TokenType.Operator;
        }

        private bool IsOperator(char ch)
        {
            if (Char.IsLetter(ch))
                return true;
            switch (ch)
            {
                case '*':
                case '\'':
                case '\"':
                    return true;
            }
            return false;
        }

        private char ConsumeValue()
        {
            this.m_operatorParams.Append(this.m_currentChar);
            return GetNextChar();
        }

        private bool IsWhiteSpace(char ch)
        {
            switch (ch)
            {
                case '\0':
                case '\t':
                case '\x0A':
                case '\f':
                case '\x0D':
                case ' ':
                    return true;
            }
            return false;
        }

        private bool IsDelimiter(char ch)
        {
            switch (ch)
            {

                case '(':
                case ')':
                case '<':
                case '>':
                case '[':
                case ']':               
                case '/':
                case '%':
                    return true;
            }
            return false;
        }

        private bool CheckForTextOperator()
        {
            char ch = this.m_nextChar;
            int pointer =0;
            if (Array.IndexOf(m_textShowers, ch.ToString()) < 0)
            {
                if (IsWhiteSpace(ch))
                {
                    ch = (char)m_contentStream[m_charPointer];
                    pointer++;
                }
                string textOperator = ch.ToString();
                ch = (char)m_contentStream[m_charPointer + pointer];
                textOperator += ch.ToString();

                if (Array.IndexOf(m_textShowers, textOperator) < 0)
                    return false;
                else
                    return true;
            }
            else
                return true;

        }
    }
}