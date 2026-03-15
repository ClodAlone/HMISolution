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

#region file using directives
using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
#endregion

namespace Syncfusion.HTMLUI.Base.Parser.CSS
{
  #region Enum
  /// <summary>
  /// Enumerator for noting expected token.
  /// </summary>
  internal enum CSSToken
  {
    /// <summary>
    /// Name of the CSS element is expected.
    /// </summary>
    Name,
    /// <summary>
    /// Open '{' is expected.
    /// </summary>
    Open,
    /// <summary>
    /// Name of the attribute is expected.
    /// </summary>
    AttrName,
    /// <summary>
    /// Value of the attribute is expected.
    /// </summary>
    AttrValue,
    /// <summary>
    /// Delimiter between attributes is expected.
    /// </summary>
    AttrDelimiter,
    /// <summary>
    /// Close '}' is expected.
    /// </summary>
    Close
  }
  #endregion

  /// <summary>
  /// This class converts files / strings in CSS format to an XML document.
  /// </summary>
  internal class CSSParser
  {
    #region Class constants
    /// <summary>
    /// Buffer size for temporary StringBuilder objects used by the class.
    /// </summary>
    private const int DEF_TEMP_BUFFER = 1024;
    /// <summary>
    /// Default RegEx checks object's options.
    /// </summary>
    private const RegexOptions DEF_REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.IgnoreCase;
    /// <summary>
    /// Pattern for attribute values in style.
    /// </summary>
    private const string DEF_VALUE_REGEX = "([^\r\n\f:;{}=]+)";
    /// <summary>
    /// Pattern for WhiteSpace.
    /// </summary>
    private const string DEF_WHITESPACE_REGEX = @"[ \t\r\n]+";
    /// <summary>
    /// Pattern for new line symbols.
    /// </summary>
    private const string DEF_SKIPNEWLINE_REGEX = @"[\t\r\n]+";
    /// <summary>
    /// Pattern for symbol name.
    /// </summary>
    private const string DEF_NAME_REGEX = @"[^;{}=]+";
    /// <summary>
    /// Pattern for attribute name.
    /// </summary>
    private const string DEF_ATTRNAME_REGEX = @"[^\r\n\f;{} :=]+";
    /// <summary>
    /// Pattern for delimiter between attributes.
    /// </summary>
    private const string DEF_DELIMITER_REGEX = @"([ \t\r\n]*)(;){1}([ \t\r\n]*)";
    /// <summary>
    /// Pattern for beginning of element style.
    /// </summary>
    private const string DEF_OPEN_REGEX = @"([ \t\r\n]*)({){1}([ \t\r\n]*)";
    /// <summary>
    /// Pattern for close of element style.
    /// </summary>
    private const string DEF_CLOSE_REGEX = @"([ \t\r\n]*)(}){1}([ \t\r\n]*)";
    /// <summary>
    /// Trim this values at the beginning and end of the element name
    /// </summary>
    private const string DEF_TRIM_NAME_REGEX = "([\t \r\n\u0000-\u001f,\u003b-\u003f]+)";
    /// <summary>
    /// Pattern for WhiteSpace.
    /// </summary>
    private const string DEF_WHITESPACE = @"[\s]+";
    /// <summary>
    /// Tokens which consist of a few symbols but have some meaning.
    /// </summary>
    private readonly string[] DEF_TOKENS = new string[]
    {
      #region tokens
      "<!--",
      "-->",
      "</",
      "<![",
      "&#",
      "/>",
      "/*",
      "*/",
      "~=",
      "|=",
      "@import",
      "@page",
      "@media",
      "@font-face",
      "@charset",
      "!important",
      "em",
      "ex",
      "px",
      "mm",
      "cm",
      "in",
      "pt",
      "pc",
      "deg",
      "rad",
      "grad",
      "ms",
      "hz",
      "Khz",
      "url(",
      #endregion
    };
    #endregion

    #region Regex checkers
    /// <summary>
    /// Object used by IsName method for checks.
    /// </summary>
    private static Regex m_regName = new Regex( DEF_NAME_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsAttrName method for checks.
    /// </summary>
    private static Regex m_regAttrName = new Regex( DEF_ATTRNAME_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsAttrValue method for checks.
    /// </summary>
    private static Regex m_regValue = new Regex( DEF_VALUE_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsDelimiter method for checks.
    /// </summary>
    private static Regex m_regDelimiter = new Regex( DEF_DELIMITER_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsOpen method for checks.
    /// </summary>
    private static Regex m_regOpen = new Regex( DEF_OPEN_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsDClose method for checks.
    /// </summary>
    private static Regex m_regClose = new Regex( DEF_CLOSE_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsWhitespace method for checks.
    /// </summary>
    private static Regex m_regWhiteSpace = new Regex( DEF_WHITESPACE_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsNewLine method for checks.
    /// </summary>
    private static Regex m_regSkipNewLine = new Regex( DEF_SKIPNEWLINE_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by TrimElementName method for trimming bad symbols in element names.
    /// </summary>
    private static Regex m_regTrimBName = new Regex( "^" + DEF_TRIM_NAME_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by TrimElementName method for trimming bad symbols in element names.
    /// </summary>
    private static Regex m_regTrimEName = new Regex( DEF_TRIM_NAME_REGEX + "$", DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by Whitespace method for deleting multiple WhiteSpaces from string.
    /// </summary>
    private static Regex m_regDelWhiteSpaces = new Regex( DEF_WHITESPACE, DEF_REGEX_OPTIONS );
    #endregion

    #region Class members
    /// <summary>
    /// Holds input CSS data.
    /// </summary>
    private TokenStream m_reader;
    /// <summary>
    /// Writes output XML document.
    /// </summary>
    private XmlWriter m_writer;
    /// <summary>
    /// Enumerator. Corresponds to tokens in document.
    /// </summary>
    private CSSToken m_token;
    /// <summary>
    /// Indicates whether some data was read from reader or not.
    /// </summary>
    private bool m_wasAnything;
    /// <summary>
    /// Indicates whether to skip bad styles or throw exceptions (skip if True; make exception if False.)
    /// </summary>
    private bool m_SkipBadOrError;
    /// <summary>
    /// Temporary object. Is member for performance needs.
    /// </summary>
    private StringBuilder m_temp1 = new StringBuilder( DEF_TEMP_BUFFER );
    /// <summary>
    /// Temporary object. Is member for performance needs.
    /// </summary>
    private StringBuilder m_temp2 = new StringBuilder( DEF_TEMP_BUFFER );
    /// <summary>
    /// Temporary object. Is member for performance needs.
    /// </summary>
    private StringBuilder m_temp3 = new StringBuilder( DEF_TEMP_BUFFER );
    #endregion

    #region Class Properties
    /// <summary>
    /// Indicates whether to skip bad styles or throw exceptions (skip if True; make exception if False.)
    /// </summary>
    internal bool SkipBadStyles
    {
      get
      {
        return m_SkipBadOrError;
      }
      set
      {
        if( m_SkipBadOrError != value )
        {
          m_SkipBadOrError = value;
        }
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Overloaded constructor. Initializes new object.
    /// </summary>
    public CSSParser()
    {
      m_wasAnything = false;
      m_SkipBadOrError = true;
    }
    /// <summary>
    /// Initializes new object with specified reader and writer.
    /// </summary>
    public CSSParser( TokenStream reader, XmlWriter writer )
      : this()
    {
      ConfigureParser( reader, writer );
    }
    #endregion

    #region Class Public methods
    /// <summary>
    /// Prepares our parser for work.
    /// </summary>
    /// <param name="reader">Source object.</param>
    /// <param name="writer">Destination object.</param>
    public void ConfigureParser( TokenStream reader, XmlWriter writer )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      m_reader = reader;
      m_writer = writer;

      // Configure token stream.
      ArrayList list = new ArrayList( m_reader.MultiCharTokens );
      list.AddRange( DEF_TOKENS );

      m_reader.OneCharTokens = " !\"#$%&'()*+,/.<=>?;:[\\]^{|}~`\t\n\r".ToCharArray();
      m_reader.CaseSensitive = false;
      m_reader.MultiCharTokens = ( string[] )list.ToArray( typeof( string ) );

      m_token = CSSToken.Name;
    }
    /// <summary>
    /// Main method. Converts CSS document into an XML document.
    /// </summary>
    public void Process()
    {
      ConvertStylesheet();
    }
    #endregion

    #region Class utility methods
    /// <summary>
    /// Private method. Writes root element and converts CSS document into XML document.
    /// </summary>
    private void ConvertStylesheet()
    {
      m_writer.WriteStartElement( "stylesheet", "http://www.syncfusion.com/NS/CSS-stylesheet" );
      ConvertData();
      m_writer.WriteEndElement();
    }
    /// <summary>
    /// Runs through all tokens in the document and invokes the corresponding methods.
    /// </summary>
    private void ConvertData()
    {
      while( !Eof() )
      {
        switch( m_token )
        {
          case CSSToken.Name:           ReadName(); break;
          case CSSToken.Open:           ReadOpen(); break;
          case CSSToken.AttrName:       ReadAttrName(); break;
          case CSSToken.AttrValue:      ReadAttrValue(); break;
          case CSSToken.AttrDelimiter:  ReadDelimiter(); break;
          case CSSToken.Close:          ReadClose(); break;
        }
      }
    }
    /// <summary>
    /// Reads element name from the document (until '{' is found).
    /// </summary>
    private void ReadName()
    {
      // Get name
      string tokenValue = ExtractName();

      // If name exists.
      if( !Eof() )
      {
        if( m_wasAnything )
        {
          m_writer.WriteStartElement( "selector" );
          m_writer.WriteAttributeString( "name", tokenValue );
          m_token = CSSToken.Open;
          m_wasAnything = false;
        }
        else if( !this.SkipBadStyles )
        {
          throw new ParseException( "Invalid Document, Style Name is not correct!" );
        }
      }
    }
    /// <summary>
    /// Reads start of data of style name ( '{' ).
    /// </summary>
    private void ReadOpen()
    {
      ExtractOpen();
      m_token = CSSToken.AttrName;
      m_wasAnything = false;
    }
    /// <summary>
    /// Reads attribute name. From 'name:value', 'name' is the result.
    /// </summary>
    private void ReadAttrName()
    {
      string tokenValue = ExtractAttrName();  // Get name of the attribute.
      if( m_wasAnything )                     // Name exists.
      {
        m_writer.WriteStartElement( "declaration" );
        m_writer.WriteAttributeString( "name", tokenValue  );
        m_token = CSSToken.AttrValue;
        m_wasAnything = false;
      }
    }
    /// <summary>
    /// Reads attribute value. From 'name:value', 'value' is the result.
    /// </summary>
    private void ReadAttrValue()
    {
      string tokenValue = ExtractAttrValue(); // Get Value

      if( m_wasAnything )                     // Value exists
      {
        m_writer.WriteString( tokenValue );
        m_writer.WriteEndElement();
        m_wasAnything = false;
        m_token = CSSToken.AttrDelimiter;
      }
      else
      {
        m_writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Reads delimiter between attributes ';'.
    /// </summary>
    private void ReadDelimiter()
    {
      ExtractDelimiter();
      m_wasAnything = false;
      m_token = CSSToken.AttrName;
    }
    /// <summary>
    /// Reads close element '}'.
    /// </summary>
    private void ReadClose()
    {
      ExtractClose();
      m_writer.WriteEndElement();

      SkipToName();

      m_wasAnything = false;
      m_token = CSSToken.Name;
    }
    /// <summary>
    /// Extracts name of element. From 'someStyle { ... }', 'someStyle' is the result.
    /// </summary>
    /// <returns>Name of style.</returns>
    private string ExtractName()
    {
      m_temp1.Length = 0;
      m_temp2.Length = 0;
      m_temp3.Length = 0;

      SkipSpaces();

      while( !Eof() )
      {
        string token = PeekToken();
        if( !IsName( token ) ) break;

        if( !IsNewLine( token ) ) m_temp1.Append( token );

        ReadToken();
        m_wasAnything = true;
      }

      return TrimElementName( m_temp1.ToString() );
    }
    /// <summary>
    /// Extracts '{' element.
    /// </summary>
    private void ExtractOpen()
    {
      SkipSpaces();
      if( Eof() ) return;
      string token = PeekToken();

      if( !IsOpen( token ) )
      {
        if( !this.SkipBadStyles )
          throw new ParseException( "Invalid Document: '{' is expected, but '" + token + "' is found!" );
        else
        {
          SkipTillEndToken( "}" , false );
          m_wasAnything = false;
          m_token = CSSToken.Close;
        }
      }

      ReadToken();
      m_wasAnything = true;
    }
    /// <summary>
    /// Extracts attribute name of the element (untill ':').
    /// </summary>
    /// <returns>Name of the attribute.</returns>
    private string ExtractAttrName()
    {
      SkipSpaces();

      m_temp2.Length = 0;
      string token = PeekToken();

      if( !IsAttrName( token ) && ! IsClose( token ) )
      {
        SkipTillEndToken( "}" , false );
        m_wasAnything = false;
        m_token = CSSToken.Close;
        return PeekToken();
      }

      if( IsClose( token ) )
      {
        m_wasAnything = false;
        m_token = CSSToken.Close;
        return token;
      }

      do
      {
        if( Eof() )
        {
          m_writer.WriteEndElement();
          break;
        }

        token = PeekToken();
        if( !IsAttrName( token ) && ! IsClose( token ) ) break;
        if( !IsNewLine( token ) )
        {
          m_temp2.Append( token );
        }

        ReadToken();
        m_wasAnything = true;
      }
      while( true );

      m_token = CSSToken.AttrValue;
      return m_temp2.ToString().Trim();

    }
    /// <summary>
    /// Extracts attribute value of the element.
    /// </summary>
    /// <returns>Value of the attribute.</returns>
    private string ExtractAttrValue()
    {
      m_temp3.Length = 0;
      m_wasAnything = false;

      SkipSpaces();

      string token = PeekToken();

      if( !IsAttrValue( token ) && token != ":" )
      {
        SkipTillEndToken( "}" , false );
        m_wasAnything = false;
        m_token = CSSToken.Close;
        return PeekToken();
      }

      if( token == ":" )
      {
        ReadToken();
        SkipSpaces();
      }

      do
      {
        if( Eof() )
        {
          m_writer.WriteEndElement();
          break;
        }

        token = PeekToken();
        if( !IsAttrValue( token ) ) break;
        if( !IsNewLine( token ) )
        {
          m_temp3.Append( token );
        }
        ReadToken();
        m_wasAnything = true;
      }
      while( true );

      m_token = CSSToken.AttrDelimiter;
      return m_temp3.ToString().Trim();
    }
    /// <summary>
    /// Extracts delimiter ';' between attributes.
    /// </summary>
    private void ExtractDelimiter()
    {
      SkipSpaces();

      if( Eof() )
      {
        m_writer.WriteEndElement();
        return;
      }

      string token = PeekToken();

      if( !IsDelimiter( token ) && !IsClose( token ) )
      {
        SkipTillEndToken( "}" , false );
        m_wasAnything = false;
        m_token = CSSToken.Close;
        return;
      }

      if( IsClose( token ) )
      {
        m_token = CSSToken.Close;
        m_wasAnything = false;
        return;
      }

      ReadToken();
      m_wasAnything = true;
    }
    /// <summary>
    /// Extracts '}' element.
    /// </summary>
    private void ExtractClose()
    {
      SkipSpaces();

      if( Eof() ) return;

      string token = PeekToken();

      if( !IsClose( token ) )
      {
        SkipTillEndToken( "}" , false );
        m_wasAnything = false;
        m_token = CSSToken.Close;
        return;
      }

      ReadToken();
      m_wasAnything = true;
    }
    /// <summary>
    /// Peeks next token from the TokenStream.
    /// </summary>
    /// <returns>Token from the stream.</returns>
    private string PeekToken()
    {
      SkipComments();
      return m_reader.PeekToken();

    }
    /// <summary>
    /// Reads next token from the TokenStream.
    /// </summary>
    /// <returns>Token from the stream.</returns>
    private string ReadToken()
    {
      SkipComments();
      return m_reader.ReadToken();
    }
    /// <summary>
    ///  Skips all next WhiteSpaces.
    /// </summary>
    private void  SkipSpaces()
    {
      while( !Eof() )
      {
        string token = PeekToken();
        if( !IsWhiteSpace( token ) ) break;

        ReadToken();
      }
    }
    /// <summary>
    /// If bad attributes, skip all data for current element.
    /// </summary>
    private void SkipAttribute()
    {
      SkipSpaces();

      while( !Eof() )
      {
        string token = PeekToken();
        if( IsWhiteSpace( token ) || IsDelimiter( token ) || IsClose( token ) ) break;

        ReadToken();
      }

      m_token = CSSToken.AttrName;
    }
    /// <summary>
    /// Helper method. Extract from stream data till specified by user token.
    /// </summary>
    /// <param name="end">Token which ends read operation.</param>
    /// <param name="bAddEnd">Include or not end token into returned string.</param>
    /// <returns>String between start point and end token.</returns>
    private void SkipTillEndToken( string end, bool bAddEnd )
    {
      string token = string.Empty;
      bool first = true;

      do
      {
        if( Eof() )
        {
          m_writer.WriteEndElement();
          break;
        }

        token = m_reader.PeekToken();
        if( token == null ) break; // if end of stream

        if( token == end && first )
        {
          m_reader.ReadToken();
        }
        else if( token == end )
        {
          if( bAddEnd ) m_reader.ReadToken();
          break;
        }

        m_reader.ReadToken();
        first = false;
      }
      while( true );
    }
    /// <summary>
    /// Skips data within comments in a CSS document.
    /// </summary>
    private void SkipComments()
    {
      if( Eof() ) return;
      string token = m_reader.PeekToken();

      if( token == "<!--" )
      {
        SkipTillEndToken( "-->", true );
      }
      else if( token == "/*" )
      {
        SkipTillEndToken( "*/", true );
      }
    }
    /// <summary>
    /// Skips all bad tokens between elements (From '}' to 'someStyle { ... }').
    /// </summary>
    private void SkipToName()
    {
      SkipSpaces();

      while( !Eof() )
      {
        string token = PeekToken();
        if( IsName( token ) ) break;

        ReadToken();
      }
    }
    /// <summary>
    /// Trims bad values from the beginning and end of the element name.
    /// </summary>
    /// <param name="elementName">Name of the style.</param>
    private string TrimElementName( string elementName )
    {
      elementName =  m_regTrimBName.Replace( elementName, "" );
      elementName =  m_regTrimEName.Replace( elementName, "" );

      return RemoveBigSpaces( elementName );
    }
    /// <summary>
    /// Removes all whitespaces except simple space from the string.
    /// </summary>
    /// <param name="val">String token.</param>
    /// <returns>Result string.</returns>
    private string RemoveBigSpaces( string val )
    {
      return m_regDelWhiteSpaces.Replace( val, " " );
    }
    #endregion

    #region Class Test Methods
    /// <summary>
    /// Indicates whether the end of file is reached.
    /// </summary>
    /// <returns></returns>
    public  bool Eof()
    {
      return ( m_reader.PeekChar() == -1 );
    }
    /// <summary>
    /// Indicates whether token is good to be a name of an element.
    /// </summary>
    /// <param name="token">Token to check.</param>
    /// <returns>True if token is good for name; False otherwise.</returns>
    private bool IsName( string token )
    {
      if( token == null ) return false;
      return m_regName.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether token is good to be an attribute name.
    /// </summary>
    /// <param name="token">Token to check.</param>
    /// <returns>True if token is good for name; False otherwise.</returns>
    private bool IsAttrName( string token )
    {
      if( token == null ) return false;
      return m_regAttrName.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether token is good to be an attribute value.
    /// </summary>
    /// <param name="token">Token to check.</param>
    /// <returns>True if token is good for name; False otherwise.</returns>
    private bool IsAttrValue( string token )
    {
      if( token == null ) return false;
      return m_regValue.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether token is good to be a delimiter between attributes ';'.
    /// </summary>
    /// <param name="token">Token to check.</param>
    /// <returns>True if token is good for name; False otherwise.</returns>
    private bool IsDelimiter( string token )
    {
      if( token == null ) return false;
      return m_regDelimiter.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether token is good to be '{' - an open element.
    /// </summary>
    /// <param name="token">String token.</param>
    /// <returns>True if we are inside style.</returns>
    private bool IsOpen( string token )
    {
      if( token == null ) return false;
      return m_regOpen.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether token is good to be '}' - a close element.
    /// </summary>
    /// <param name="token">String token.</param>
    /// <returns>True if we are going to exit from style.</returns>
    private bool IsClose( string token )
    {
      if( token == null ) return false;
      return m_regClose.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether token is a whitespace symbol.
    /// </summary>
    /// <param name="token">Token to check.</param>
    /// <returns>True if token is whitespace, False otherwise.</returns>
    private bool IsWhiteSpace( string token )
    {
      if( token == null ) return false;
      return m_regWhiteSpace.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether token is a new line symbol.
    /// </summary>
    /// <param name="token">String token.</param>
    /// <returns>True if we are going to jump on new line.</returns>
    private bool IsNewLine( string token )
    {
      if( token == null ) return false;
      return m_regSkipNewLine.Match( token ).Success;
    }
    #endregion
  }
}
