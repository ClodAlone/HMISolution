#region Copyright Syncfusion Inc. 2001 - 2006
//
//  Copyright Syncfusion Inc. 2001 - 2006. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Text;
using System.Collections;

%%

%{
  #region Fields
  /// <summary>
  /// Holds the current string being read.
  /// </summary>
  private StringBuilder m_string = new StringBuilder();
  /// <summary>
  /// Holds the current parentessis index.
  /// </summary>
  private int m_paren;
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
			int val = ( int )( ( m_yyReader as PdfReader ).Position ) - m_yyBufferRead + m_yyBufferIndex;
			return val;
		}
		}
#endregion //Properties

  #region Public methods
  /// <summary>
  /// Resets the lexer.
  /// </summary>
  internal void Reset()
  {
    m_yyBuffer = new char[ YY_BUFFER_SIZE ];
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
		internal byte[] Read( int count )
		{
			ArrayList list = new ArrayList( count );
			YyMarkStart();

			if( m_yyBufferRead - m_yyBufferStart < count )
			{
				while( m_yyBuffer.Length - m_yyBufferStart < count )
				{
					m_yyBuffer = YyDouble( m_yyBuffer );
				}

			}
			int readCount = YyRead();
			
      if( m_yyBufferRead - m_yyBufferStart < count )
			{
				// Not enough data.
				return null;
			}

			for( int i = m_yyBufferStart; i < m_yyBufferStart + count; ++i, m_yyBufferIndex = i )
			{
        list.Add( ( byte )m_yyBuffer[ i ] );
			}
			
      YyMarkStart();
      YyMarkEnd();

			return ( byte[] )( list.ToArray( typeof( byte ) ) );
		}
		/// <summary>
		/// Skips the new line.
		/// </summary>
		internal void SkipNewLine()
		{
			m_yyBufferIndex = m_yyBufferStart;

			if( m_yyBuffer[ m_yyBufferIndex ] == '\r' )
			{
				if( m_yyBuffer[ m_yyBufferIndex + 1 ] == '\n' )
				{
					m_yyBufferIndex += 2;
				}
			}
			else if( m_yyBuffer[ m_yyBufferIndex ] == '\n' )
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
%}

%eofval{
return TokenType.Eof;
%eofval}

%namespace Syncfusion.Pdf.IO
%class PdfLexer
%function GetNextToken
%line
%full
%type TokenType
%state HexString, String

ws [ \n\r\t\000\f]
digit [0-9]
hexDigit [0-9A-Fa-f]
letter [A-Za-z]
delimeter [\%\(\)\<\>\[\]\{\}\/]
sign ("+"|"-")

comment "%"(.*)
stringStart "("
stringEnd ")"
dictionaryStart "<<"
dictionaryEnd ">>"
hexStringStart "<"
hexStringEnd ">"
arrayStart \[
arrayEnd \]
streamStart "stream"
streamEnd "endstream"
objectStart "obj"
objectEnd "endobj"

any .|\n
unicode ([^)]){any}

name \/[^ \n\r\t\000\f"%()<>[]{}/"]*

bool "true"|"false"
number {sign}?{digit}+
realNumber {sign}?{digit}*\.{digit}+

%%

<YYINITIAL> {comment} {// Skip comment
            break;}
<YYINITIAL> {ws} {// Skip whitespace.
          break;}
<YYINITIAL> {dictionaryStart} { return TokenType.DictionaryStart; }
<YYINITIAL> {dictionaryEnd} { return TokenType.DictionaryEnd; }

  
<YYINITIAL> ({stringStart}\xfe\xff({unicode})*{stringEnd}) {
  // A unicode string.
  return TokenType.UnicodeString; }

<YYINITIAL> {stringStart} { // ordinary string.
  YyBegin( State.String );
  StringText.Length = 0;
  break; }
  
<String> {stringStart} { 
  StringText.Append( YyText() );
  ++m_paren;
  break; }

<String> \\({any}|\r\n) { // Escape sequence.
  StringText.Append( YyText() );
  break; }

<String> ([^\)\\]) { // Other characters.
  StringText.Append( YyText() ); 
  break; }

<String> {stringEnd} { 
  if( m_paren > 0 ) // Balanced parentessis.
  {
    StringText.Append( YyText() );
    --m_paren;
  }
  else // Real end of the string.
  {
    YyBegin( State.YYINITIAL);
    return TokenType.String;
  }
  break; }

<String> {any} {
  break; }
  
<YYINITIAL> {hexStringStart} { YyBegin( State.HexString );
  return TokenType.HexStringStart; }

<HexString> {hexStringEnd} { YyBegin( State.YYINITIAL );
  return TokenType.HexStringEnd; }

<HexString> {hexDigit} { 
  return TokenType.HexDigit; }

<HexString> \\({any}|\r\n) { // Escape sequence.
  return TokenType.HexStringWeirdEscape;
  }

<HexString> {ws} { 
  return TokenType.WhiteSpace; }

<HexString> {any} { 
  return TokenType.HexStringWeird; }

<YYINITIAL> {arrayStart} { return TokenType.ArrayStart; }
<YYINITIAL> {arrayEnd} { return TokenType.ArrayEnd; }

<YYINITIAL> {name} { return TokenType.Name; }
<YYINITIAL> {bool} { return TokenType.Boolean; }

<YYINITIAL> {realNumber} { return TokenType.Real; }
<YYINITIAL> {number} { return TokenType.Number; }

<YYINITIAL> {objectStart} { return TokenType.ObjectStart; }
<YYINITIAL> {objectEnd} { return TokenType.ObjectEnd; }

<YYINITIAL> {streamStart} { return TokenType.StreamStart; }
<YYINITIAL> {streamEnd} { return TokenType.StreamEnd; }

<YYINITIAL> "R" { return TokenType.Reference; }
<YYINITIAL> "trailer" { return TokenType.Trailer; }
<YYINITIAL> "startxref" { return TokenType.StartXRef; }
<YYINITIAL> "xref" { return TokenType.XRef; }
<YYINITIAL> "null" { return TokenType.Null; }
<YYINITIAL> "f"|"n" { return TokenType.ObjectType; }

<YYINITIAL> {any} { 
  Console.WriteLine( "File: '{0}'", ( char )yyLookAhead );
  YyError( YYError.Match, true );
  break; }
