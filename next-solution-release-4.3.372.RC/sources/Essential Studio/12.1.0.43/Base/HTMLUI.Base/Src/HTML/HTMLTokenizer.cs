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
using System.IO;
using System.Collections;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;

using Syncfusion.HTMLUI.Base.Utility;
#endregion

namespace Syncfusion.HTMLUI.Base.Parser.HTML
{
  /// <summary>
  /// This class tokenizes input stream and returns it as a high-level abstraction.
  /// </summary>
  internal class HTMLTokenizer
  {
    #region Class constants
    /// <summary>
    /// Buffer size for temporary StringBuilder objects used by class.
    /// </summary>
    private const int DEF_TEMP_BUFFER = 1024;
    /// <summary>
    /// Default RegEx checks object's options.
    /// </summary>
    private const RegexOptions DEF_REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.IgnoreCase;
    /// <summary>
    /// IsValueData method checks regular expression.
    /// </summary>
    private const string DEF_VALUE_REGEX = "[=a-z0-9*;.()%~`!@#$%^&*()_+'&\"" + @"\[\]\\\/]+"; // @"[^ \t\r\n]+"; //
    /// <summary>
    /// IsWhitespace method checks regular expression.
    /// </summary>
    private const string DEF_WHITESPACE_REGEX = @"[ \t\r\n]+";
    /// <summary>
    /// IsName method checks regular expression.
    /// </summary>
    private const string DEF_NAME_REGEX = DEF_START_NAME_REGEX + @"[a-z0-9.:_-]*";
    /// <summary>
    /// IsStartName method checks regular expression.
    /// </summary>
    private const string DEF_START_NAME_REGEX = @"[a-z_:]+";
    /// <summary>
    /// Pattern for WhiteSpace.
    /// </summary>
    private const string DEF_WHITESPACE = @"[ \t]+";
    /// <summary>
    /// Pattern for new lines.
    /// </summary>
    private const string DEF_NEW_LINE = @"[\r\n]+";
    /// <summary>
    /// Pattern for CDATA.
    /// </summary>
    private const string DEF_CDATA = @"\<!\[CDATA\[([\w\W]*)\]\]\>";
    /// <summary>
    /// Array of tokens which indicates end of attribute value.
    /// </summary>
    private readonly string[] DEF_END_OF_VAL_AR1 = {"\"", ">", "/>" /*, Environment.NewLine*/};
    /// <summary>
    /// Array of tokens which indicate end of attribute value.
    /// </summary>
    private readonly string[] DEF_END_OF_VAL_AR2 = {"'", ">", "/>"/*, Environment.NewLine*/};
    #endregion

    #region Class Regex checkers
    /// <summary>
    /// Object used by IsValueData method for checks.
    /// </summary>
    private static Regex m_regValue = new Regex( DEF_VALUE_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsWhitespace method for checks.
    /// </summary>
    private static Regex m_regWhiteSpace = new Regex( DEF_WHITESPACE_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsWhitespace method for checks.
    /// </summary>
    private static Regex m_regNewLine = new Regex( DEF_NEW_LINE, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsName method for checks.
    /// </summary>
    private static Regex m_regName = new Regex( DEF_NAME_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by IsName method for checks.
    /// </summary>
    private static Regex m_regStartName = new Regex( DEF_START_NAME_REGEX, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Object used by ExtractDataFromCDATA method for extracting data.
    /// </summary>
    private static Regex m_regCDATA = new Regex( DEF_CDATA, DEF_REGEX_OPTIONS );
    #endregion

    #region  Class members
    /// <summary>
    /// Input stream which helps users to read HTML input files.
    /// </summary>
    private TokenStream     m_reader;
    /// <summary>
    /// Storage for CDataElementName object.
    /// </summary>
    private string          m_CDataElementName = string.Empty;
    /// <summary>
    /// Global position of parser. For internal use only.
    /// </summary>
    private ParserPosition  m_pointInParser = ParserPosition.Text;
    /// <summary>
    /// Token type. Type of data extracted from stream.
    /// </summary>
    private ParserToken     m_tokenType = ParserToken.None;
    /// <summary>
    /// Dictionary of entities which must be converted to char symbols.
    /// </summary>
    private IDictionary     m_entities;
    /// <summary>
    /// Token which was last extracted by ReadToken method.
    /// </summary>
    private string          m_token = string.Empty;
    /// <summary>
    /// Temporary buffer for class methods. To remove memory allocation
    /// code from methods, all temporary objects become class members.
    /// </summary>
    private StringBuilder   m_temp1 = new StringBuilder( DEF_TEMP_BUFFER );
    /// <summary>
    /// Temporary buffer for class methods. To remove memory allocation
    /// code from methods, all temporary objects become class members.
    /// </summary>
    private StringBuilder   m_temp2 = new StringBuilder( DEF_TEMP_BUFFER );
    /// <summary>
    /// Temporary buffer for class methods. To remove memory allocation
    /// code from methods, all temporary objects become class members.
    /// </summary>
    private StringBuilder   m_temp3 = new StringBuilder( DEF_TEMP_BUFFER );
    /// <summary>
    /// Temporary buffer for class methods. To remove memory allocation
    /// code from methods, all temporary objects become class members.
    /// </summary>
    private StringBuilder   m_temp4 = new StringBuilder( DEF_TEMP_BUFFER );
    /// <summary>
    /// Temporary buffer for class methods. To remove memory allocation
    /// code from methods, all temporary objects become class members.
    /// </summary>
    private StringBuilder   m_temp5 = new StringBuilder( DEF_TEMP_BUFFER );
    /// <summary>
    /// Temporary buffer for class methods. To remove memory allocation
    /// code from methods, all temporary objects become class members.
    /// </summary>
    private StringBuilder   m_temp6 = new StringBuilder( DEF_TEMP_BUFFER );
    /// <summary>
    /// Temporary buffer for class methods. To remove memory allocation
    /// code from methods, all temporary objects become class members.
    /// </summary>
    private StringBuilder   m_temp7 = new StringBuilder( DEF_TEMP_BUFFER );
    #endregion

    #region  Class Properties
    /// <summary>
    /// Gets or sets the encoding of currently opened stream for tokenizing.
    /// </summary>
    public Encoding     Encoding
    {
      get
      {
        return m_reader.Encoding;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "Encoding" );

        m_reader.Encoding = value;
      }
    }
    /// <summary>
    /// Returns the last extracted token.
    /// </summary>
    public string       TokenString
    {
      get
      {
        return m_token;
      }
    }

    /// <summary>
    /// Returns the last extracted token's type.
    /// </summary>
    internal ParserToken  Token
    {
      get
      {
        return m_tokenType;
      }
    }

    /// <summary>
    /// Gets or sets the dictionary of entities which must be converted to unicode chars.
    /// </summary>
    public IDictionary  Entities
    {
      get
      {
        return m_entities;
      }
      set
      {
        m_entities = value;
      }
    }
    /// <summary>
    /// CData element which must be found by parser for closing CDATA section of
    /// stream and start work in proper manner.
    /// </summary>
    internal string     CDataElement
    {
      get
      {
        return m_CDataElementName;
      }
      set
      {
        m_CDataElementName = value;
      }
    }
    /// <summary>
    /// Parser point. Parser has four states with their own parse algorithms.
    /// This property is used for manually switching from one parse algorithm to another.
    /// </summary>
    internal ParserPosition Position
    {
      get
      {
        return m_pointInParser;
      }
      set
      {
        m_pointInParser = value;
      }
    }
    /// <summary>
    /// Indicates whether the end of file is reached.
    /// </summary>
    /// <returns>True if end of file.</returns>
    public bool         IsEof
    {
      get
      {
        return ( m_reader.PeekChar() == -1 );
      }
    }
    #endregion

    #region  Class Initialize/Finalize methods
    /// <summary>
    /// Overloaded. Class can not be constructed by user without input stream.
    /// </summary>
    private HTMLTokenizer()
    {

    }
    /// <summary>
    /// Main constructor.
    /// </summary>
    public HTMLTokenizer( TokenStream reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( !reader.CanRead || !reader.CanSeek )
        throw new ArgumentException( "Input stream does not support read or seek operations.", "reader" );

      m_reader = reader;

      // NOTE: Here we configure the token stream. After configuration, token stream
      // will return data in a better way for parsing.

      // Add NewLine symbol to array.
      ArrayList list = new ArrayList( m_reader.MultiCharTokens );

      list.Add( "<!" );
      list.Add( "<?" );
      list.Add( "<!--" );
      list.Add( "-->" );
      list.Add( "</" );
      list.Add( "<![" );
      list.Add( "&#" );
      list.Add( "/>" );

      // Configure token stream.
      m_reader.OneCharTokens = " !\"$%&'()*+,/;<=>?[\\]^{|}~`\t\r\n".ToCharArray();
      m_reader.MultiCharTokens = ( string[] )list.ToArray( typeof( string ) );
    }
    #endregion

    #region Token Test methods
    /// <summary>
    /// Indicates whether user specified token is whitespace symbols or not.
    /// </summary>
    /// <param name="token">Token to check.</param>
    /// <returns>True if token is whitespace; False otherwise.</returns>
    private bool IsWhitespace( string token )
    {
      if( token == null ) return false;

      return m_regWhiteSpace.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether user specified token is new line symbols or not.
    /// </summary>
    /// <param name="token">Token to check.</param>
    /// <returns>True if token is new line; False otherwise.</returns>
    private bool IsNewLine( string token )
    {
      if( token == null ) return false;

      return m_regNewLine.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether token is an attribute or tag name.
    /// </summary>
    /// <param name="token">Token to check.</param>
    /// <returns>True if token is good for name; False otherwise.</returns>
    private bool IsName( string token )
    {
      if( token == null ) return false;

      return m_regName.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether token is good to be in the start of an attribute or tag name.
    /// </summary>
    /// <param name="token">Token for checking.</param>
    /// <returns>True if token is good for start name; False otherwise.</returns>
    private bool IsStartName( string token )
    {
      if( token == null ) return false;

      return m_regStartName.Match( token ).Success;
    }
    /// <summary>
    /// Indicates whether token is good to be part of an attribute value.
    /// </summary>
    /// <param name="token">Token to check.</param>
    /// <returns>True if token is good for attribute value; False otherwise.</returns>
    private bool IsValueData( string token )
    {
      if( token == null ) return false;

      return m_regValue.Match( token ).Success;
    }
    #endregion

    #region Extract Utilities
    /// <summary>
    /// Overloaded Helper method. Extracts from stream data until specified by user token.
    /// </summary>
    /// <param name="end">Token which ends read operation.</param>
    /// <returns>String between start point and end token.</returns>
    private string ExtractTillEndToken( string end )
    {
      return ExtractTillEndToken( end, false );
    }
    /// <summary>
    /// Helper method. Extracts from stream data until specified by user token.
    /// </summary>
    /// <param name="end">Token which ends read operation.</param>
    /// <param name="bAddEnd">Indicates whether to include end token into returned string.</param>
    /// <returns>String between start point and end token.</returns>
    private string ExtractTillEndToken( string end, bool bAddEnd )
    {
      m_temp1.Length = 0;

      do
      {
        string token = m_reader.ReadToken();
        if( token == null ) break; // if end of stream

        if( token == end )
        {
          if( bAddEnd ) m_temp1.Append( token );
          break;
        }

        m_temp1.Append( token );
      }
      while( true );

      return m_temp1.ToString();
    }
    /// <summary>
    /// Overloaded Helper method. Extracts text from stream until the end token is reached.
    /// End token can be any item from ends array.
    /// </summary>
    /// <param name="ends">Array of end tokens.</param>
    /// <param name="found">Found end token.</param>
    /// <returns>Returns extracted text until end token.</returns>
    private string ExtractTillOneEndTokens( string[] ends, out string found )
    {
      return ExtractTillOneEndTokens( ends, false, out found );
    }
    /// <summary>
    /// Helper method. Extracts text from stream until the end token is reached.
    /// End token can be any item from ends array.
    /// </summary>
    /// <param name="ends">Array of end tokens.</param>
    /// <param name="bAddEnd">Indicates whether to include end token into output string.</param>
    /// <param name="found">Found end token.</param>
    /// <returns>Returns extracted text until end token.</returns>
    private string ExtractTillOneEndTokens( string[] ends, bool bAddEnd, out string found )
    {
      if( ends == null )
        throw new ArgumentNullException( "ends" );

      if( ends.Length == 0 )
        throw new ArgumentException( "Method does not work with empty arrays of end tokens." );

      m_temp2.Length = 0;
      found = string.Empty;

      do
      {
        string token = m_reader.PeekToken();
        if( token == null ) break; // if end of stream

        if( Array.IndexOf( ends, token ) >= 0 )
        {
          found = token;
          if( bAddEnd ) m_temp2.Append( m_reader.ReadToken() );
          break;
        }

        m_temp2.Append( m_reader.ReadToken() );

      }
      while( true );

      return m_temp2.ToString();
    }
    #endregion

    #region Class types extracting
    /// <summary>
    /// Extracts whitespace from all whitespace tokens.
    /// </summary>
    /// <returns>Returns extracted whitespace from stream.</returns>
    private string ExtractWhiteSpace()
    {
      m_temp7.Length = 0;

      do
      {
        string token = m_reader.PeekToken();
        if( !IsWhitespace( token ) ) break;

        m_temp7.Append( token );
        m_reader.ReadToken();
      }
      while( true );

      m_tokenType = ParserToken.Space;
      return m_temp7.ToString();
    }
    /// <summary>
    /// Extracts new line from the token.
    /// </summary>
    /// <returns>Returns extracted new line from stream.</returns>
    private string ExtractNewLine()
    {
      m_temp7.Length = 0;

      do
      {
        string token = m_reader.PeekToken();
        if( !IsNewLine( token ) ) break;

        m_temp7.Append( token );
        m_reader.ReadToken();
      }
      while( true );

      //m_tokenType = ParserToken.Space;
      return m_temp7.ToString();
    }
    /// <summary>
    /// Extract tag name from stream.
    /// </summary>
    /// <returns>Tag name.</returns>
    private string ExtractTagStart()
    {
      m_tokenType = ParserToken.StartTag;
      string output = ExtractName();

      if( output != null && output.Length > 0 )
      {
        m_pointInParser = ParserPosition.Tag;
      }
      else
      {
        m_tokenType = ParserToken.Text;
      }

      return output;
    }
    /// <summary>
    /// Extracts name from stream.
    /// </summary>
    /// <returns>Extracted name.</returns>
    private string ExtractName()
    {
      SkipToNameBegin();
      m_temp3.Length = 0;

      do
      {
        string token = m_reader.PeekToken();
        if( !IsName( token ) ) break;

        m_temp3.Append( token );
        m_reader.ReadToken();
      }
      while( true );

      return m_temp3.ToString();
    }
    /// <summary>
    /// Extracts definitions from stream.
    /// </summary>
    /// <returns>Extracted definition.</returns>
    private string ExtractDefinition()
    {
      m_tokenType = ParserToken.ProcessingInstruction;
      return ExtractTillEndToken( ">" );
    }
    /// <summary>
    /// Extracts comment from stream.
    /// </summary>
    /// <returns>Extracted comment.</returns>
    private string ExtractComment()
    {
      m_tokenType = ParserToken.Comment;
      return ExtractTillEndToken( "-->" );
    }
    /// <summary>
    /// Extracts end of tag symbol.
    /// </summary>
    /// <returns>Extracted text from current position until end of tag symbol.</returns>
    private string ExtractEndOfTag()
    {
      m_tokenType = ParserToken.EndTag;
      return ExtractTillEndToken( ">" );
    }
    /// <summary>
    /// Extracts marked section from stream.
    /// </summary>
    /// <returns>Value of marked section.</returns>
    private string ExtractMarkedSection()
    {
      m_tokenType = ParserToken.MarkedSection;
      return ExtractTillEndToken( "]>" );
    }
    /// <summary>
    /// Extracts entity from stream and returns it as unicode char.
    /// If entity is not recognized, returns tested text.
    /// </summary>
    /// <returns>Entity to unicode symbol or text.</returns>
    private string ExtractEntity()
    {
      string found;
      string output = ExtractTillOneEndTokens( new string[]{ ";", "<", "</" }, out found );

      m_tokenType = ParserToken.Text;

      if( m_entities.Contains( output ) )
      {
        int chInt = ( int )m_entities[ output ];
        output = new string( (char)chInt, 1 );
      }
      else // not known to us Entity
      {
        if( found != ";" )
        {
          output = "&" + output;
          //m_reader.SeekOnTokenLength( found, true, SeekOrigin.Current );
          
          return output;
        }

        output = "&" + output + found;
      }

      // Read next token because last found ";" is not still read.
      m_reader.ReadToken();
      
      return output;
    }
    /// <summary>
    /// Extracts entity where it was written as integer value.
    /// </summary>
    /// <returns>Entity as unicode symbol or the unrecognized text.</returns>
    private string ExtractNumberEntity()
    {
      string found = string.Empty;
      string output = ExtractTillOneEndTokens( new string[]{ ";", "<" }, out found );

      m_tokenType = ParserToken.Text;

      int value = -1;

      try
      {
        double result;
        bool bParsed = false;

#if( SyncfusionFramework2_0 )
        bParsed = Double.TryParse( output, out result );
#else
        bParsed = Double.TryParse( output, NumberStyles.HexNumber,
          null, out result );
#endif
        if( bParsed )
        {
          value = (int)result;
        }
        else
        {
          value = -1;
        }
      }
      finally
      {
        if( found == "<" )
        {
          m_reader.SeekOnTokenLength( found, true, SeekOrigin.Current );
          found = string.Empty;
        }
        else if( found == ";" )
        {
          m_reader.ReadToken();
        }
      }

      if( value < 0 ) return output + found;
      return new string( ( char )value, 1 );
    }
    /// <summary>
    /// Extracts text from stream until next element for parsing.
    /// </summary>
    /// <returns>Extracted text.</returns>
    private string ExtractText()
    {
      m_temp4.Length = 0;

      do
      {
        string token = m_reader.PeekToken();

        if( token == null ) break; // if end of stream
        if( token[0] == '<' || token[0] == '&' ) break;

        m_temp4.Append( m_reader.ReadToken() );
      }
      while( true );

      m_tokenType = ParserToken.Text;
      return m_temp4.ToString();
    }
    /// <summary>
    /// Extracts attribute value.
    /// </summary>
    /// <returns>Value of attribute.</returns>
    private string ExtractAttributeValue()
    {
      ExtractWhiteSpace();

      m_temp5.Length = 0;
      string token = m_reader.PeekToken();
      string delimiter;
      if( token == "\"" )
      {
        m_reader.ReadToken();
        m_temp5.Append( ExtractTillOneEndTokens( DEF_END_OF_VAL_AR1, false, out delimiter ) );
        if( delimiter == token ) m_reader.ReadToken();
      }
      else if( token == "'" )
      {
        m_reader.ReadToken();
        m_temp5.Append( ExtractTillOneEndTokens( DEF_END_OF_VAL_AR2, false, out delimiter ) );
        if( delimiter == token ) m_reader.ReadToken();
      }
      else
      {
        do
        {
          token = m_reader.PeekToken();
          if( !IsValueData( token ) ) break;

          m_temp5.Append( m_reader.ReadToken() );
        }
        while( true );
      }

      m_tokenType = ParserToken.AttributeValue;
      return m_temp5.ToString();
    }
    #endregion

    #region  Class utility methods
    /// <summary>
    /// Skips all bad symbols before attribute value.
    /// </summary>
    private void SkipToAttrValueBegin()
    {
      do
      {
        string token = m_reader.PeekToken();
        if( IsValueData( token ) ) break;

        m_reader.ReadToken();
      }
      while( true );
    }
    /// <summary>
    /// Skips all bad symbols after '&lt;'.
    /// </summary>
    private void SkipToNameBegin()
    {
      do
      {
        string token = m_reader.PeekToken();
        if( IsStartName( token ) || token == null ) break;

        m_reader.ReadToken();
      }
      while( true );
    }
    /// <summary>
    /// Parses data if next will be text.
    /// </summary>
    private void ParseTokenIfText()
    {
      string token = m_reader.ReadToken();

      if( IsWhitespace( token ) )
      {
        m_token = token + ExtractWhiteSpace();
      }
      else
      {
        string next = string.Empty;

        switch( token )
        {
          case "<":     next = ExtractTagStart();     break;
          case "<?":
          case "<!":    next = ExtractDefinition();   break;
          case "<!--":  m_token = ExtractComment();      return;
          case "</":    next = ExtractEndOfTag();     break;
          case "<![":   m_token = ExtractMarkedSection(); return;
          case "&":     m_token = ExtractEntity();       return;
          case "&#":    next = ExtractNumberEntity(); break;
          default:      m_token = token + ExtractText(); return;
        }

        if( next != null && next.Length > 0 )
        {
          m_token = next;
        }
        else
        {
          m_token = token + next;
          m_tokenType = ParserToken.Text;
        }
      }
    }
    /// <summary>
    /// Parses data if next will be tag.
    /// </summary>
    private void ParseTokenIfTag()
    {
      string token = m_reader.PeekToken();

      if( IsWhitespace( token ) )
      {
        m_token = token + ExtractWhiteSpace();
      }
      else
      {
        switch( token )
        {
          case ">":
            m_tokenType = ParserToken.StartTagEnd;
            m_pointInParser = ParserPosition.Text;
            m_reader.ReadToken();
            // Just after closed start tag we have to ignore new lines.
            ExtractNewLine();
            break;

          case "/>":
            m_tokenType = ParserToken.StartTagClosedEnd;
            m_pointInParser = ParserPosition.Text;
            m_reader.ReadToken();
            break;

          case "=":
            m_pointInParser = ParserPosition.Attribute;
            ParseTokenIfAttribute();
            break;

          default:
          {
            m_token = token;
            if( IsName( token ) ) // if token is attribute name
            {
              m_tokenType = ParserToken.Attribute;
              m_pointInParser = ParserPosition.Attribute;
              m_reader.ReadToken();
            }
            else // else if bad data
            {
              if( token == "<" ) m_reader.ReadToken();

              string[] delimiters = { ">", "/>" };
              string delimiter;
              ExtractTillOneEndTokens( delimiters, true, out delimiter );
              m_tokenType = ParserToken.StartTagEnd;
              m_pointInParser = ParserPosition.Text;
              // Just after closed start tag we have to ignore new lines.
              ExtractNewLine();
            }
          }
            break;
        }
      }
    }
    /// <summary>
    /// Parses data if next will be attribute.
    /// </summary>
    private void ParseTokenIfAttribute()
    {
      string token = m_reader.PeekToken();

      if( IsWhitespace( token ) )
      {
        m_token = ExtractWhiteSpace();
      }
      else
      {
        switch( token )
        {
          case "=":
            m_reader.ReadToken();
            m_token = ExtractAttributeValue();
            break;

          case ">":
            m_reader.ReadToken();
            m_tokenType = ParserToken.StartTagEnd;
            m_pointInParser = ParserPosition.Text;
            // Just after closed start tag we have to ignore new lines.
            ExtractNewLine();
            break;

          default:
            m_pointInParser = ParserPosition.Tag;
            ParseTokenIfTag();
            break;
        }
      }
    }

    /// <summary>
    /// Parses data if next will be CData.
    /// </summary>
    private void ParseTokenIfCDataElement()
    {
      m_temp6.Length = 0;

      do
      {
        string token = m_reader.ReadToken();
        if( token == null ) break; // end of stream

        if( token == "</" ) // if we found close tag symbols
        {
          if( Utilities.StrEquals( m_reader.PeekToken(), m_CDataElementName ) )
          {
            ExtractTillEndToken( ">" );
            break;
          }
        }

        m_temp6.Append( token );
      }
      while( true );

      m_token = ExtractDataFromCDATA( m_temp6.ToString() );
      m_tokenType = ParserToken.Text;
      m_pointInParser = ParserPosition.Text;
    }
    /// <summary>
    /// Reads one token.
    /// </summary>
    /// <returns>Value of the token.</returns>
    internal ParserToken ReadToken()
    {
      m_token = null;

      switch( m_pointInParser )
      {
        case ParserPosition.Text:       ParseTokenIfText(); break;
        case ParserPosition.CData:      ParseTokenIfCDataElement(); break;
        case ParserPosition.Tag:        ParseTokenIfTag(); break;
        case ParserPosition.Attribute:  ParseTokenIfAttribute(); break;
      }

      if( m_tokenType == ParserToken.StartTag ||
        m_tokenType == ParserToken.EndTag ||
        m_tokenType == ParserToken.Attribute )
      {
        m_token = m_token.ToLower();
      }

      if( m_token != null && m_token.Length >0 && m_token.IndexOf( '\0' ) >= 0 )
        throw new ArgumentException( "Token stream opened with wrong encoding. Text looks like Unicode one." );

      return m_tokenType;
    }
    /// <summary>
    /// Extracts data from CDATA block.
    /// </summary>
    /// <param name="data">Income data.</param>
    /// <returns>Real data of CDATA block.</returns>
    private string ExtractDataFromCDATA( string data )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( data.Length == 0 ) return data;
      
      string result = data;

      MatchCollection matches = m_regCDATA.Matches( data );

      if( matches.Count > 0 && matches[ 0 ].Groups.Count == 2 )
      {
        result = matches[ 0 ].Groups[ 1 ].Value;
      }

      return result;
    }
    #endregion
  }
}
