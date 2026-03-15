#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Globalization;
using System.Text;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Converts string into set of tokens.
	/// </summary>
	public class FormulaTokenizer
	{
    #region Class constants
    /// <summary>
    /// Indicates end of formula string.
    /// </summary>
    private const char FormulaEnd = '\x01';
    #endregion

    #region Class members
    /// <summary>
    /// Current character.
    /// </summary>
    private char m_chCurrent;
    /// <summary>
    /// Formula length.
    /// </summary>
    private int m_iFormulaLength;
    ///// <summary>
    ///// Formula parser.
    ///// </summary>
    //private FormulaParser m_parser;
    /// <summary>
    /// 
    /// </summary>
    private int m_iPos;
    /// <summary>
    /// Current formula.
    /// </summary>
    private string m_strFormula;
    /// <summary>
    /// Start position of the current token.
    /// </summary>
    private int m_iStartPos;
    /// <summary>
    /// Current token TokenType.
    /// </summary>
    public FormulaToken TokenType;
    /// <summary>
    /// Token type of the previous token.
    /// </summary>
    public FormulaToken PreviousTokenType = FormulaToken.None;
    /// <summary>
    /// Previous character position.
    /// </summary>
    private int m_iPrevPos;
    /// <summary>
    /// String builder that contains string representation of the current token.
    /// </summary>
    private StringBuilder m_value = new StringBuilder();
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Argument separator.
    /// </summary>
    private char m_chArgumentSeparator = ',';
    /// <summary>
    /// Number format info.
    /// </summary>
    private NumberFormatInfo m_numberFormat = NumberFormatInfo.CurrentInfo;
    private int m_lastIndexQuote=-1;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public FormulaTokenizer( WorkbookImpl book )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      m_book = book;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Prepares tokinezer for formula parsing.
    /// </summary>
    /// <param name="formula">Formula to parse.</param>
    public void Prepare( string formula )
    {
      m_strFormula = formula;
      m_iFormulaLength = formula.Length;    
      m_iPos = 0;
      NextChar();
    }
    /// <summary>
    /// Moves pointer to the next character.
    /// </summary>
    private void NextChar()
    {
      if( m_iPos < m_iFormulaLength )
      {
        m_chCurrent = m_strFormula[ m_iPos ];
        m_iPos++;
      }
      else
      {
        m_chCurrent = FormulaEnd;
      }
    }
    /// <summary>
    /// Moves back to the specified character.
    /// </summary>
    /// <param name="charToMoveTo"></param>
    private void MoveBack( char charToMoveTo )
    {
      int iStartIndex = Math.Min( m_strFormula.Length - 1, m_iPos );
      int newIndex = m_strFormula.LastIndexOf( charToMoveTo, iStartIndex );

      if( newIndex >= 0 )
      {
        int difference = m_iPos - newIndex;
        m_iPos = newIndex + 1;
        m_value.Remove( m_value.Length - difference + 1, difference - 1 );
        m_chCurrent = charToMoveTo;
      }
    }

    /// <summary>
    /// Extracts next token from the string.
    /// </summary>
    public void NextToken()
    {
      PreviousTokenType = TokenType;
      m_iPrevPos = m_iStartPos;
      m_value.Length = 0;

      if( TokenType != FormulaToken.DDELink )
        TokenType = FormulaToken.None;

      char chDecimalDot = m_numberFormat.NumberDecimalSeparator[ 0 ];
      char chPrevChar = ' ';
      char chNextChar = ' ';

      if (m_strFormula.IndexOf(m_chCurrent) > 0)
          chPrevChar = m_strFormula[m_strFormula.IndexOf(m_chCurrent) - 1];

      if (m_strFormula.IndexOf(m_chCurrent) < m_strFormula.Length - 1)
          chNextChar = m_strFormula[m_strFormula.IndexOf(m_chCurrent) + 1];

      while (true)
      {
        m_iStartPos = m_iPos;

        if (Char.IsDigit(m_chCurrent))
        {
            ParseNumber();
        }
        else if ((m_chCurrent == '.') || (m_chCurrent == chDecimalDot) && Char.IsDigit(chPrevChar) && Char.IsDigit(chNextChar))
        {
            ParseNumber();
        }
        else
        {
          switch( m_chCurrent )
          {
            case FormulaEnd:
              TokenType = FormulaToken.EndOfFormula;
              break;

            case '-':
              NextChar();
              TokenType = FormulaToken.tSub;
              break;

            case '+':
              NextChar();
              TokenType = FormulaToken.tAdd;
              break;

            case '*':
              NextChar();
              TokenType = FormulaToken.tMul;
              break;

            case '/':
              NextChar();
              TokenType = FormulaToken.tDiv;
              break;

            case '%':
              NextChar();
              TokenType = FormulaToken.tPercent;
              break;

            case '(':
              NextChar();
              TokenType = FormulaToken.tParentheses;
              break;

            case ')':
              NextChar();
              TokenType = FormulaToken.CloseParenthesis;
              break;

            case '<':
              ProcessLess();
              break;

            case '>':
              ProcessGreater();
              break;

            case '=':
              NextChar();
              TokenType = FormulaToken.tEqual;
              break;

            case '\'':
              m_lastIndexQuote = -1;
              if (m_strFormula.Contains("\'"))
              {
                  int count = CharOccurs(m_strFormula, '\'');
                  if (count % 2 != 0 || (count / CharOccurs(m_strFormula, '!', '\'') <= 2) && (((CharOccurs(m_strFormula,'(')+CharOccurs(m_strFormula,')'))%2 !=0)))
                      m_lastIndexQuote = m_strFormula.LastIndexOf('\'');
              }
              

              ParseString( true );

              if( PreviousTokenType == FormulaToken.DDELink )
              {
                NextChar();
              }
              else
              {
                TokenType = FormulaToken.Identifier;
                ParseIdentifier();
              }
              break;

            case '"':
              m_lastIndexQuote = -1;
              ParseString( true );
              TokenType = FormulaToken.tStringConstant;
              break;

            case '#':
              ParseError();
              break;

            case '&':
              NextChar();
              TokenType = FormulaToken.tConcat;
              break;

            case ':':
              NextChar();
              TokenType = FormulaToken.tCellRange;
              break;

            //case '[':
            //  NextChar();
            //  TokenType = FormulaToken.OpenBracket;
            //  break;

            //case ']':
            //  NextChar();
            //  TokenType = FormulaToken.CloseBracket;
            //  break;

            case '{':
              ParseArray();
              TokenType = FormulaToken.tArray1;
              break;

            case '^':
              TokenType = FormulaToken.tPower;
              NextChar();
              break;

            default:
              if( m_chCurrent == m_chArgumentSeparator )
              {
                NextChar();
                TokenType = FormulaToken.Comma;
                break;
              }

              if( m_chCurrent > ' ' )
              {
                ParseIdentifier();
              }
              else if( m_chCurrent == ' ' )
              {
                ParseSpace();
              }
              break;
          }

        }

        if( TokenType != FormulaToken.None )
        {
          return;
        }

        NextChar();
      }
    }
    private int CharOccurs(string stringToSearch, char charToFind)
    {
        int count = 0;
        char[] chars = stringToSearch.ToCharArray();
        foreach (char c in chars)
        {
            if (c == charToFind)
            {
                count++;
            }
        }
        return count;
    }
    private int CharOccurs(string stringToSearch, char charToFind,char previousChar)
    {
        int count = 0;
        int i = 0;
        char[] chars = stringToSearch.ToCharArray();
        foreach (char c in chars)
        {
            if (c == charToFind)
            {           
                if(chars[i-1]==previousChar)
                count++;
            }
            i++;
        }
        return count;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SaveState()
    {
      m_iPrevPos = m_iStartPos;
      PreviousTokenType = TokenType;
    }
    /// <summary>
    /// 
    /// </summary>
    public void RestoreState()
    {
      m_iStartPos = m_iPrevPos;
      TokenType = PreviousTokenType;
      m_chCurrent = m_strFormula[ m_iStartPos ];
      m_value.Length = 0;
    }
    /// <summary>
    /// Extracts number token.
    /// </summary>
    private void ParseNumber()
    {
      TokenType = FormulaToken.tInteger;
      AppendNumbers();

      // TODO: Add support of decimal separator
      if( m_chCurrent == m_numberFormat.NumberDecimalSeparator[ 0 ] )
      {
        TokenType = FormulaToken.tNumber;
        m_value.Append( m_chCurrent );
        NextChar();
        AppendNumbers();
      }

      if( Char.ToUpper( m_chCurrent ) == 'E' )
      {
        TokenType = FormulaToken.tNumber;
        m_value.Append( m_chCurrent );
        NextChar();

        if( m_chCurrent == '-' || m_chCurrent == '+' )
        {
          m_value.Append( m_chCurrent );
          NextChar();
        }

        AppendNumbers();
      }
      else if( m_chCurrent == ':' )
      {
        TokenType = FormulaToken.Identifier;
        m_value.Append( m_chCurrent );
        NextChar();
        AppendNumbers();
      }
    }
    /// <summary>
    /// Process '&gt;' character and creates required token.
    /// </summary>
    private void ProcessGreater()
    {
      NextChar();

      if( m_chCurrent == '=' )
      {
        NextChar();
        TokenType = FormulaToken.tGreaterEqual;
      }
      else
      {
        TokenType = FormulaToken.tGreater;
      }
    }
    /// <summary>
    /// Process '&lt;' character and creates required token.
    /// </summary>
    private void ProcessLess()
    {
      NextChar();

      if( m_chCurrent == '=' )
      {
        NextChar();
        TokenType = FormulaToken.tLessEqual;
      }
      else if( m_chCurrent == '>' )
      {
        NextChar();
        TokenType = FormulaToken.tNotEqual;
      }
      else
      {
        TokenType = FormulaToken.tLessThan;
      }
    }
    /// <summary>
    /// Appends internal string builder while current character is number.
    /// </summary>
    private void AppendNumbers()
    {
      while( Char.IsDigit( m_chCurrent ) )
      {
        m_value.Append( m_chCurrent );
        NextChar();
      }
    }
    /// <summary>
    /// Parses identifier, it can be range address, named range of function name.
    /// </summary>
    private void ParseIdentifier()
    {
      bool b3DReference = false;
      bool bColonAppeared = false;
      int exclamationCount = 0;

      // TODO: create array with all allowed characters
      while( Char.IsLetterOrDigit( m_chCurrent ) || m_chCurrent >= ( char )0x80 || m_chCurrent == '_'
        || m_chCurrent == '!' || m_chCurrent == ':' || m_chCurrent == '.' || m_chCurrent == '$'
        || m_chCurrent == '[' || m_chCurrent == '\'' || m_chCurrent == '#' || m_chCurrent == ']' || m_chCurrent == '?')
      {
        if( m_chCurrent == '!' )
        {
          b3DReference = true;
          exclamationCount++;

          if( TokenType == FormulaToken.DDELink )
          {
            NextChar();
            break;
          }
          else if( exclamationCount > 1 && bColonAppeared )
          {
            MoveBack( ':' );
            break;
          }
        }
        else if( m_chCurrent == ':' )
        {
          bColonAppeared = true;
        }

        m_value.Append( m_chCurrent );
        char chEnd = FormulaEnd;

        if( m_chCurrent == '[' )
        {
          chEnd = ']';
        }
        else if( m_chCurrent == '\'' )
        {
          chEnd = m_chCurrent;
        }

        if( chEnd != FormulaEnd )
        {
            int nDepth = 1;
            while (m_chCurrent != FormulaEnd && nDepth>0)
            {
                NextChar();
                if (m_chCurrent != FormulaEnd)
                    m_value.Append(m_chCurrent);
                if (m_chCurrent == chEnd)
                        nDepth--;
              if (m_chCurrent == '[')
                        nDepth++;
            }
          
        }

        NextChar();
      }

      string strToken = m_value.ToString();

      if( TokenType != FormulaToken.DDELink )
      {
        if( m_chCurrent == '(' )
        {
          TokenType = FormulaToken.tFunction1;
        }
        else if( String.Compare( strToken, "true", StringComparison.CurrentCultureIgnoreCase ) == 0 ) 
        {
          TokenType = FormulaToken.ValueTrue;
        }
        else if (String.Compare(strToken, "false", StringComparison.CurrentCultureIgnoreCase) == 0)
        {
          TokenType = FormulaToken.ValueFalse;
        }
        else if( m_chCurrent == '|' )
        {
          NextChar();
          TokenType = FormulaToken.DDELink;
        }
        else if (m_value.ToString() == "Overview!" + RefErrorPtg.ReferenceError)
        {
            TokenType = FormulaToken.Identifier3D;
        }
        else if( m_value.ToString().EndsWith( "!" + RefErrorPtg.ReferenceError ) )
        {
          TokenType = FormulaToken.tError;
        }
        else
        {
          TokenType = b3DReference ? FormulaToken.Identifier3D : FormulaToken.Identifier;
        }
      }
    }
    /// <summary>
    /// Extracts spaces from the string.
    /// </summary>
    private void ParseSpace()
    {
      while( m_chCurrent == ' ' )
      {
        m_value.Append( m_chCurrent );
        NextChar();
      }

      TokenType = FormulaToken.Space;
    }
    /// <summary>
    /// Parses string token.
    /// </summary>
    /// <param name="InQuote">Indicates whether first character is quotation character or not.</param>
    private void ParseString( bool InQuote )
    {
      char chQuote = char.MinValue;
      if( InQuote )
      {
        chQuote = m_chCurrent;
        NextChar();
      }

      while( m_chCurrent != FormulaEnd )
      {
        if( InQuote && m_chCurrent == chQuote && CheckQuote())
        {
          NextChar();

          if( m_chCurrent == chQuote )
          {
            m_value.Append( m_chCurrent );
            NextChar();
          }
          else
          {
            return;
          }
        }
        else
        {
          m_value.Append( m_chCurrent );
          NextChar();
        }
      }

      if( InQuote )
      {
        RaiseException( "Incomplete string, missing " + chQuote + "; String started", null );
      }
    }

    private bool CheckQuote()
    {
        if (m_lastIndexQuote != -1)
        {
            return m_iPos == m_lastIndexQuote + 1;
        }
        return true;

    }
    /// <summary>
    /// Parses error token.
    /// </summary>
    private void ParseError()
    {
      ICollection<string> colNames = FormulaUtil.ErrorNameToCode.Keys;
      string strResult = null;

      foreach( string strErrorName in colNames )
      {
        int iLen = strErrorName.Length;

        if( String.Compare( strErrorName, 0, m_strFormula, m_iPos - 1, iLen, StringComparison.CurrentCultureIgnoreCase ) == 0 )
        {
          strResult = strErrorName;
          break;
        }
      }

      m_value.Length = 0;
      m_value.Append( strResult );
      m_iPos += strResult.Length - 1;
      TokenType = FormulaToken.tError;
      NextChar();
    }
    /// <summary>
    /// Extracts string representation of an array token.
    /// </summary>
    private void ParseArray()
    {
      m_value.Length = 0;
      //NextChar();

      while( m_chCurrent != '}' && m_chCurrent != FormulaEnd )
      {
        m_value.Append( m_chCurrent );

        // TODO: here we have to skip strings.

        if( m_chCurrent == '"' )
        {
          //NextChar();
          SkipString();
        }
        else
        {
          NextChar();
        }
      }

      m_value.Append( m_chCurrent );

      if( m_chCurrent == FormulaEnd )
      {
        RaiseException( "Couldn't find end of array", null );
      }

      NextChar();
    }
    /// <summary>
    /// Skips string and adds its value into internal string builder.
    /// </summary>
    private void SkipString()
    {
      char chEnd = m_chCurrent;
      NextChar();

      while( m_chCurrent != FormulaEnd )
      {
        m_value.Append( m_chCurrent );

        if( m_chCurrent == chEnd )
        {
          NextChar();

          if( m_chCurrent == chEnd )
          {
            m_value.Append( m_chCurrent );
          }
          else
          {
            break;
          }
        }

        NextChar();
      }

      if( m_chCurrent == FormulaEnd )
        RaiseException( "Can't find end of the string", null );
    }
    /// <summary>
    /// Raises parse exception.
    /// </summary>
    /// <param name="msg">Exception message.</param>
    /// <param name="ex">Inner exception.</param>
    public void RaiseException( string msg, Exception ex )
    {
      if( ex is ParseException )
      {
        msg = msg + ". " + ex.Message;
      }
      else
      {
        msg = msg + "  at position " + m_iStartPos;

        if( ex != null )
        {
          msg = msg + ". " + ex.Message;
        }
      }

      throw new ParseException( msg, m_strFormula, m_iPos, ex );
    }
    /// <summary>
    /// Raises parser exception with message that token was unexpected.
    /// </summary>
    /// <param name="msg">Additional message.</param>
    public void RaiseUnexpectedToken( string msg )
    {
      if( msg == null || msg.Length == 0 )
      {
        msg = string.Empty;
      }

      const string DEF_MESSAGE_FORMAT = "{0}Unexpected token type: {1}, string value: {2}";
      string strMessage = string.Format( DEF_MESSAGE_FORMAT, msg, TokenType, m_value );
      RaiseException( strMessage, null );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns value of the current token. Read-only.
    /// </summary>
    public string TokenString
    {
      get
      {
        return m_value.ToString();
      }
    }
    /// <summary>
    /// Gets / sets argument separator.
    /// </summary>
    public char ArgumentSeparator
    {
      get
      {
        return m_chArgumentSeparator;
      }
      set
      {
        m_chArgumentSeparator = value;
      }
    }
    /// <summary>
    /// Gets / sets number format info.
    /// </summary>
    public NumberFormatInfo NumberFormat
    {
      get
      {
        return m_numberFormat;     
      }
      set
      {
        m_numberFormat = value;
      }
    }
    #endregion
  }
}