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
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Xml;
using System.Net;

using Syncfusion.HTMLUI.Base;
using Syncfusion.HTMLUI.Base.Parser.HTML;
#endregion

namespace Syncfusion.HTMLUI.Base.Utility
{
  /// <summary>
  /// Utility class for different static methods.
  /// </summary>
  public sealed class Utilities
  {
    #region Class constants
    /// <summary>
    /// Options for regular expressions.
    /// </summary>
    public const RegexOptions DEF_REGEX_OPTIONS  = RegexOptions.Compiled | RegexOptions.IgnoreCase;
    /// <summary>
    /// RegEx pattern for searching special HTML symbols.
    /// </summary>
    private const string DEF_HTML_SYMBOLS = @"&[#]*([\w]+);";
    /// <summary>
    /// Pattern for numbers.
    /// </summary>
    private const string DEF_REG_NUMBERS = @"[0-9]+";
    /// <summary>
    /// RegEx object for retrieving symbols from string.
    /// </summary>
    private static Regex m_regSymbols = new Regex( DEF_HTML_SYMBOLS, DEF_REGEX_OPTIONS );
    /// <summary>
    /// RegEx object for retrieving numbers from string.
    /// </summary>
    private static Regex m_RegNumbers = new Regex( DEF_REG_NUMBERS, DEF_REGEX_OPTIONS );
    /// <summary>
    /// Delimiter of Url path.
    /// </summary>
    private const string DEF_DELIMITER = "/";
    /// <summary>
    /// Prefix of the fragment in the Uri object.
    /// </summary>
    public const char DEF_FRAGMENT_PREFIX = '#';
    /// <summary>
    /// Pattern for WhiteSpace.
    /// </summary>
    private const string DEF_WHITESPACE_PATTERN = @"[\s]+";
    /// <summary>
    /// Whitespace string.
    /// </summary>
    private const string DEF_WHITESPACE = " ";
    /// <summary>
    /// Delimiter in local filesystem.
    /// </summary>
    private const string DEF_FILE_PATH_DELIMITER = "\\";
    /// <summary>
    /// Limit number of converting arabic to \"A\" format.
    /// </summary>
    private const float DEF_AR_TO_LETTER_LIMIT = 26.0f;
    /// <summary>
    /// Index of A char in the ASCII table.
    /// </summary>
    private const int DEF_A_ASCII_INDEX = ( int )( 'A' - 1 );
    #endregion

    #region Class static members
    /// <summary>
    /// xPath pattern.
    /// </summary>
    private const string DEF_CHARSET = @"charset=(?<encoding>[\w-]+)";
    /// <summary>
    /// RegEx options.
    /// </summary>
    private const RegexOptions DEF_OPTIONS = RegexOptions.Compiled | RegexOptions.IgnoreCase;
    /// <summary>
    /// Regex object for detecting encoding.
    /// </summary>
    private static Regex m_regEncoding = new Regex( DEF_CHARSET, DEF_OPTIONS );
    /// <summary>
    /// Regex object for deleting multiple WhiteSpaces.
    /// </summary>
    private static Regex m_regDelWhiteSpaces = new Regex( DEF_WHITESPACE_PATTERN, DEF_REGEX_OPTIONS );
    #endregion

    #region Class Static methods
    /// <summary>
    /// Indicates whether two strings are equal or not.
    /// Method is case insensitive.
    /// </summary>
    /// <param name="a">First string.</param>
    /// <param name="b">Second string.</param>
    /// <returns>True if strings are equal by value.</returns>
    public static bool StrEquals( string a, string b )
    {
      return ( string.Compare( a, b, true, CultureInfo.InvariantCulture ) == 0 );
    }
    /// <summary>
    /// Indicates whether file with such path exists. Searches in specified directory also.
    /// </summary>
    /// <param name="currentDir">Additional directory for searching the file.</param>
    /// <param name="path">Path to the file.</param>
    /// <returns>True if file is found.</returns>
    public static bool IsFileExists( string currentDir, string path )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );

      string fullPath;

      return IsFileExists( currentDir, path, out fullPath );
    }
    /// <summary>
    /// Returns the absolute path for the file.
    /// </summary>
    /// <param name="currentDir">Additional directory for searching the file.</param>
    /// <param name="path">Path to the file.</param>
    /// <returns>Full path for the file if file exists; NULL otherwise.</returns>
    public static string GetFullPath( string currentDir, string path )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );

      if( path.Length == 0 )
        throw new ArgumentException( "path - string cannot be empty" );

      string fullPath;
      if( IsFileExists( currentDir, path, out fullPath ) )
      {
        return fullPath;
      }

      return null;
    }
    /// <summary>
    /// Indicates whether file with specified path exists. Searches in additional directory also.
    /// </summary>
    /// <param name="currentDir">Additional directory for searching the file.</param>
    /// <param name="path">Path to the file.</param>
    /// <param name="fullPath">Full path to the file.</param>
    /// <returns>True if file exists; False otherwise.</returns>
    public static bool IsFileExists( string currentDir, string path, out string fullPath )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );

      if( path.Length == 0 )
        throw new ArgumentException( "path - string cannot be empty" );

      fullPath = string.Empty;
      bool bExists = false;

      if( currentDir == null || currentDir.Length == 0 )
      {
        if( File.Exists( path ) )
        {
          fullPath = Path.GetFullPath( path );
          bExists = true;
        }
      }
      else
      {
        try
        {
          string dir = Path.GetFullPath( currentDir );
          string fileName = Path.GetFileName( path );
          string fullpath = dir + DEF_FILE_PATH_DELIMITER + fileName;

          if( File.Exists( fullpath ) )
          {
            fullPath = Path.GetFullPath( fullpath );

            bExists = true;
          }
        }
        catch( Exception e )
        {
          Debug.WriteLine( e.Message + Environment.NewLine + e.StackTrace );
        }
      }

      return bExists;
    }
    /// <summary>
    /// Indicates whether resource by Uri with defined path exists. Searches in additional Uri base also.
    /// </summary>
    /// <param name="currentDir">Additional root uri for searching resource.</param>
    /// <param name="path">Path to resource.</param>
    /// <param name="fullUriPath">Uri object to the file.</param>
    /// <returns>True if file by Uri exists; False otherwise.</returns>
    public static bool IsUriExists( string currentDir, string path, out Uri fullUriPath )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );

      if( path.Length == 0 )
        throw new ArgumentException( "path - string cannot be empty" );

      bool bFound = true;

      try
      {
        fullUriPath = new Uri( path );
      }
      catch( UriFormatException ex ) // exception while Uri instance was creating
      {
        Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace, "Exception" );

        bFound = false;
        fullUriPath = null;
      }

      try
      {
        if( currentDir != null && currentDir.Length > 0 && !bFound )
        {
          fullUriPath = GetUri( currentDir, path );
          bFound = true;
        }
      }
      catch( UriFormatException ex ) // exception while Uri instance was creating
      {
        Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace, "Exception" );

        bFound = false;
        fullUriPath = null;
      }

      return bFound;
    }
    /// <summary>
    /// Converts string with special HTML symbols to string view.
    /// </summary>
    /// <param name="input">String with special symbols.</param>
    /// <returns>Converted string if symbols exist; input string otherwise.</returns>
    public static string GetConvertedString( string input )
    {
      if( input == null )
        throw new ArgumentNullException( "input" );

      if( input.Length == 0 )
        return input;

      MatchCollection matches = m_regSymbols.Matches( input );

      if( matches.Count == 0 ) return input;

      Match m = null;
      string symbol = string.Empty;
      string code   = string.Empty;

      for( int i = 0, len = matches.Count; i < len; i++ )
      {
        m = matches[ i ] as Match;
        if( m.Groups.Count == 2 )
        {
          symbol = m.Groups[ 0 ].Value;
          code   = m.Groups[ 1 ].Value;

          // symbol has type &strcode;
          if( HTMLConfig.MainConfig.Entities.Contains( code ) )
          {
            int  value = ( int )HTMLConfig.MainConfig.Entities[ code ];
            input		= input.Replace( symbol, ( (char)value ).ToString() );
          }
          else
          {
            // may be symbol has type &#numcode;
            double result;
            if( Double.TryParse( code, NumberStyles.Number, null, out result ) )
            {
              int value = ( int )result;
              int minValue = ( int )char.MinValue;
              int maxValue = ( int )char.MaxValue;

              if( value >= minValue && value <= maxValue )
              {
                input		= input.Replace( symbol, ( (char)value ).ToString() );
              }
            }
          }
        }
      }

      return input;
    }
    /// <summary>
    /// Converts ShortVariant value to string and returns it.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>String interpretation of the variant value.</returns>
    public static string ConvertToString( string value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      // TODO: implement here convert of Variant value to string.

      return string.Empty;
    }

    /// <summary>
    /// Converts ShortVariant value to integer and returns it.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Integer value.</returns>
    public static int ConvertToInteger( string value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      double result;
      if( Double.TryParse( value, NumberStyles.Integer, null, out result ) )
      {
        return ( int )Math.Round( result );
      }

      return int.MinValue;
    }
    /// <summary>
    /// Returns point from its string representation.
    /// </summary>
    /// <param name="value">String representation of the point.</param>
    /// <returns>Point form of string.</returns>
    public static Point ConvertToPoint( string value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( value.Length == 0 )
        throw new ArgumentException( "value - string cannot be empty" );

      Point result = Point.Empty;
      Match m      = m_RegNumbers.Match( value );
      double number;

      if( Double.TryParse( m.Groups[ 0 ].Value, NumberStyles.Number, null, out number ) )
      {
        result.X = ( int )number;
      }

      m = m.NextMatch();

      if( Double.TryParse( m.Groups[ 0 ].Value, NumberStyles.Number, null, out number ) )
      {
        result.Y = ( int )number;
      }

      return result;
    }
    /// <summary>
    /// Returns size from its string representation.
    /// </summary>
    /// <param name="value">String representation of the size.</param>
    /// <returns>Size object.</returns>
    public static Size ConvertToSize( string value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( value.Length == 0 )
        throw new ArgumentException( "value - string cannot be empty" );

      Size result = Size.Empty;
      Point tmpPoint = ConvertToPoint( value );

      result.Width  = tmpPoint.X;
      result.Height = tmpPoint.Y;

      return result;
    }
    /// <summary>
    /// Converts string to its bool representation.
    /// </summary>
    /// <param name="value">String representation of the bool.</param>
    /// <returns>Bool value.</returns>
    public static bool ConvertToBool( string value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( value.Length == 0 )
        throw new ArgumentException( "value - string cannot be empty" );

      if( StrEquals( value, "true" ) )
        return true;
      else if( StrEquals( value, "false" ) )
        return false;

      throw new ArgumentException( "Can't convert value '" + value + "' to BOOL" );
    }
    /// <summary>
    /// Creates bitmap object from the stream.
    /// </summary>
    /// <param name="stream">Bitmap stream data.</param>
    /// <returns>Bitmap object, if bitmap is created; NULL otherwise.</returns>
    public static Bitmap ImageFromStream( Stream stream )
    {
      Bitmap result = null;

      if( stream != null )
      {
        try
        {
          result = new Bitmap( stream );
        }
        catch
        {
          Debug.WriteLine( "Can't load image from stream." );
        }
      }

      return result;
    }
    /// <summary>
    /// Loads and returns bitmap from the file.
    /// </summary>
    /// <param name="fullPath">Full path to the file.</param>
    /// <returns>Bitmap if loaded; Null otherwise.</returns>
    public static Bitmap ImageFromFile( string fullPath )
    {
      Bitmap result = null;

      if( fullPath != null && fullPath.Length > 0 )
        try
        {
          result = Image.FromFile( fullPath ) as Bitmap;
        }
        catch
        {
          Debug.WriteLine( "Can't load image from file." );
        }

      return result;
    }
    /// <summary>
    /// Returns data from the specified Uri address.
    /// </summary>
    /// <param name="uri">Uri path to the resource.</param>
    /// <returns>Data from Uri resource.</returns>
    public static Stream StreamFromUrl( Uri uri )
    {
      if( uri == null )
        throw new ArgumentNullException( "uri" );

      Stream stream = null;

      try
      {
        XmlUrlResolver resolver = new XmlUrlResolver();

        stream = ( Stream )resolver.GetEntity( uri, null, typeof( Stream ) );

        if( stream != null && !stream.CanSeek )
        {
          MemoryStream ms = new MemoryStream();
          int val;
          
          while( ( val = stream.ReadByte() ) != -1 )
          {
            ms.WriteByte( ( byte )val );
          }

          stream.Close();
          stream = ms;
        }

        stream.Position = 0;
      }
      catch( IOException ioe )
      {
        Debug.WriteLine( ioe.Message + Environment.NewLine + ioe.StackTrace );
        stream = null;
      }
      catch( WebException we )
      {
        Debug.WriteLine( we.Message + Environment.NewLine + we.StackTrace );
        stream = null;
      }
      catch( Exception e )
      {
        Debug.WriteLine( e.Message + Environment.NewLine + e.StackTrace );
        throw e;
      }

      return stream;
    }
    /// <summary>
    /// Returns data from the specified Uri address.
    /// </summary>
    /// <param name="uri">Uri path to the resource.</param>
    /// <returns>Data from Uri resource.</returns>
    public static string StringFromUrl( Uri uri )
    {
      string result = string.Empty;

      Stream stream = StreamFromUrl( uri );
      
      if( stream != null )
      {
        byte[] data = ReadToEnd( stream );
        result = Encoding.Default.GetString( data );
        data = null;
      }

      stream.Close();

      return result;
    }
    /// <summary>
    /// Reads data from the specified stream.
    /// </summary>
    /// <param name="stream">Source stream for data reading.</param>
    /// <returns>Array of data from the stream.</returns>
    public static byte[] ReadToEnd( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      byte[] result;

      if( stream.CanSeek )
      {
        int length = ( int )( stream.Length - stream.Position );
        result = new byte[ length ];
        stream.Read( result, 0, result.Length );
      }
      else
      {
        MemoryStream ms = new MemoryStream();
        int val;
          
        while( ( val = stream.ReadByte() ) != -1 )
        {
          ms.WriteByte( ( byte )val );
        }

        ms.Position = 0;
        result = new byte[ ms.Length ];
        ms.Read( result, 0, result.Length );
        ms.Close();
      }

      return result;
    }
    /// <summary>
    /// Returns string data extracted from the specified file with default encoding.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="currentDir">Name of the Directory.</param>
    /// <returns>String data extracted from the file with default encoding.</returns>
    public static string StringFromFile( string currentDir, string fileName )
    {
      Stream stream = StreamFromFile( currentDir, fileName );
      string result = string.Empty;

      if( stream != null )
      {
        byte[] buff = ReadToEnd( stream );
        result = Encoding.Default.GetString( buff );
        buff = null;
      }

      return result;
    }
    /// <summary>
    /// Creates and returns the stream from the specified file.
    /// </summary>
    /// <param name="currentDir">Current directory.</param>
    /// <param name="fileName">Path to the file.</param>
    /// <returns>Stream object if resource found; Null otherwise.</returns>
    public static Stream StreamFromFile( string currentDir, string fileName )
    {
      if( fileName == null )
        throw new ArgumentNullException( "fileName" );
      if( fileName.Length == 0 )
        throw new ArgumentException( "fileName - string can not be empty" );

      string fullPath = GetFullPath( currentDir, fileName );
      Stream stream = null;

      if( fullPath != null )
      {
        stream = File.OpenRead( fullPath );
      }

      return stream;
    }
    /// <summary>
    /// Reads data from the specified stream and converts it to the stream.
    /// </summary>
    /// <param name="stream">Stream with data.</param>
    /// <param name="encoding">Encoding object.</param>
    /// <returns>String data from the stream.</returns>
    public static string StringFromStream( Stream stream, Encoding encoding )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );
      if( encoding == null )
        throw new ArgumentNullException( "encoding" );

      string result = null;

      if( stream.CanRead )
      {
        int len = ( int )( stream.Length - stream.Position );

        byte[] buff = new byte[ len ];
        stream.Read( buff, 0, len );
        result = encoding.GetString( buff, 0, len );
      }

      return result;
    }

	  /// <summary>
	  /// Converts the given string to memory stream.
	  /// </summary>
	  /// <param name="str">The string to convert in to stream.</param>
	  /// <returns>Stream data from string</returns>
	  public static Stream StreamFromString(string str)
	  {
		  if(str == null || str == "")
		  {
			  throw new ArgumentNullException( "string" );
		  }
          
		  Stream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(str));
		  return stream;
	  }
    /// <summary>
    /// Detects and returns the encoding from it's string name.
    /// </summary>
    /// <param name="encType">Name of encoding.</param>
    /// <returns>Encoding data if detected; Null otherwise.</returns>
    public static Encoding DetectEncoding( string encType )
    {
      if( encType == null )
        throw new ArgumentNullException( "encType" );

      Encoding result = null;

      if( encType.Length > 0 )
      {
        try
        {
          Match match = m_regEncoding.Match( encType );

          if( match.Success )
          {
            string encoding = match.Result( "${encoding}" );

            result = Encoding.GetEncoding( encoding );
          }
        }
        catch( ArgumentException )
        {
          result = Encoding.Default;
          Debug.WriteLine( "Can't detect encoding from name: " + encType );
        }
      }

      return result;
    }
    /// <summary>
    /// Extracts bookmark path from the specified full path and returns array
    /// where the first element is the path without bookmark; second - bookmark if found.
    /// </summary>
    /// <param name="path">Path to resource.</param>
    /// <returns>Array where first element is path without bookmark;
    ///  second - bookmark if found.</returns>
    public static string[] ExtractBookmark( string path )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );

      string[] result = new string[]{ path, string.Empty };
      int bookmarkIndex = path.IndexOf( DEF_FRAGMENT_PREFIX );
      
      // bookmark exists.
      if( bookmarkIndex >= 0 )
      {
        result[ 0 ] = path.Substring( 0, bookmarkIndex );
        result[ 1 ] = path.Substring( bookmarkIndex );
      }

      return result;
    }
    /// <summary>
    /// Retrieves path to file from path where bookmark data may exist.
    /// </summary>
    /// <param name="path">Path to file.</param>
    /// <returns>Path to file from path where bookmark data may exist.</returns>
    public static string RemoveBookmark( string path )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );

      string result = path;
      int bookmarkIndex = path.IndexOf( DEF_FRAGMENT_PREFIX );
      
      // bookmark exists.
      if( bookmarkIndex >= 0 )
      {
        result = path.Substring( 0, bookmarkIndex );
      }

      return result;
    }
    /// <summary>
    /// Retrieves bookmark data from path where bookmark data may exist.
    /// </summary>
    /// <param name="path">Path to file.</param>
    /// <returns>Bookmark data from path where bookmark data may exist.</returns>
    public static string GetBookmark( string path )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );

      string result = string.Empty;
      int bookmarkIndex = path.IndexOf( DEF_FRAGMENT_PREFIX );
      
      // bookmark exists.
      if( bookmarkIndex >= 0 )
      {
        result = path.Substring( bookmarkIndex );
      }

      return result;
    }
    /// <summary>
    /// Trims big whitespaces from the specified string to single, skip tabs, new lines, etc.
    /// </summary>
    /// <param name="str">String value.</param>
    /// <returns>String after deleting whitespaces.</returns>
    public static string DeleteWhiteSpace( string str )
    {
      if( str == null || str.Length == 0 ) return str;

      str = m_regDelWhiteSpaces.Replace( str, DEF_WHITESPACE );

      return str;
    }
    /// <summary>
    /// Converts arabic number to roman.
    /// </summary>
    /// <param name="intArabic">Number in arabic format.</param>
    /// <returns>Number in Roman format.</returns>
    public static string ArabicToRoman( int intArabic )
    {
      StringBuilder retval = new StringBuilder();
      
      retval.Append( GenerateNumber( ref intArabic, 1000, "M" ) );
      retval.Append( GenerateNumber( ref intArabic, 900,  "CM" ) );
      retval.Append( GenerateNumber( ref intArabic, 500,  "D" ) );
      retval.Append( GenerateNumber( ref intArabic, 400,  "CD" ) );
      retval.Append( GenerateNumber( ref intArabic, 100,  "C" ) );
      retval.Append( GenerateNumber( ref intArabic, 90,   "XC" ) );
      retval.Append( GenerateNumber( ref intArabic, 50,   "L" ) );
      retval.Append( GenerateNumber( ref intArabic, 40,   "XL" ) );
      retval.Append( GenerateNumber( ref intArabic, 10,   "X" ) );
      retval.Append( GenerateNumber( ref intArabic, 9,    "IX" ) );
      retval.Append( GenerateNumber( ref intArabic, 5,    "V" ) );
      retval.Append( GenerateNumber( ref intArabic, 4,    "IV" ) );
      retval.Append( GenerateNumber( ref intArabic, 1,    "I" ) );

      return retval.ToString();
    }
    /// <summary>
    /// Converts arabic number to \"A\" format.
    /// </summary>
    /// <param name="arabic">Number in arabic format.</param>
    /// <returns>Number in \"A\" format.</returns>
    public static string ArabicToLetter( int arabic )
    {
      Stack stack = ConvertToLetter( arabic );
      StringBuilder result = new StringBuilder();

      while( stack.Count > 0 )
      {
        int num = ( int )stack.Pop();
        AppendChar( result, num );
      }

      return result.ToString();
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Private constructor.
    /// </summary>
    private Utilities()
    {
      // Don't create instance.
    }
    #endregion

    #region Class utility methods
    /// <summary>
    /// Returns uri path from directory path and relative path of the resource.
    /// </summary>
    /// <param name="currentDir">Directory of the document.</param>
    /// <param name="path">Relative path to the resource.</param>
    /// <returns>Uri object of the resource.</returns>
    private static Uri GetUri( string currentDir, string path )
    {
      if( currentDir == null )
        throw new ArgumentNullException( "currentDir" );

      if( path == null )
        throw new ArgumentNullException( "path" );

      Uri result = null;
      try
      {
        if( path.Substring( 0, 1 ) != DEF_DELIMITER )
        {
          string absPath = currentDir + DEF_DELIMITER + path;
          result = new Uri( absPath );
        }
          // Path must be calculated from the root of the server.
        else
        {
        
          Uri baseUri = new Uri( currentDir );
          result = new Uri( baseUri, path );
        }
      }
      catch( UriFormatException ex ) // Exception while Uri instance was creating.
      {
        Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace, "Exception" );
      }
      
      return result;
    }
    /// <summary>
    /// Utility metnod for converting arabic number to roman format.
    /// </summary>
    /// <param name="value">Current number value.</param>
    /// <param name="magnitude">Max current number.</param>
    /// <param name="letter">Roman equivalent.</param>
    /// <returns>Roman equivalent.</returns>
    private static string GenerateNumber( ref int value, int magnitude, string letter )
    {
      StringBuilder numberstring = new StringBuilder();
      
      while( value >= magnitude )
      {
        value -= magnitude;
        numberstring.Append( letter );
      }
      
      return numberstring.ToString();
    }
    /// <summary>
    /// Utility metnod. Helps to convert arabic number to \"A\" format.
    /// </summary>
    /// <param name="arabic">Arabic number.</param>
    /// <returns>Sequence of number.</returns>
    private static Stack ConvertToLetter( float arabic )
    {
      if( arabic <= 0 )
        throw new ArgumentOutOfRangeException( "arabic", arabic, "Value can not be less 0" );
      
      Stack stack = new Stack();

        
      while( ( ( int )arabic ) > DEF_AR_TO_LETTER_LIMIT )
      {
        float remainder = arabic % DEF_AR_TO_LETTER_LIMIT;

        if( remainder == 0.0f )
        {
          arabic = arabic / DEF_AR_TO_LETTER_LIMIT - 1f;
          remainder = DEF_AR_TO_LETTER_LIMIT;
        }
        else
        {
          arabic /= DEF_AR_TO_LETTER_LIMIT;
        }

        stack.Push( ( int )remainder );
      }

      if( arabic > 0f )
      {
        stack.Push( ( int )arabic );
      }
    
      return stack;
    }
    /// <summary>
    /// Adds letter instead of number.
    /// </summary>
    /// <param name="builder">String builder object.</param>
    /// <param name="number">Number to be converted to letter.</param>
    private static void AppendChar( StringBuilder builder, int number )
    {
      if( builder == null )
        throw new ArgumentNullException( "builder" );
      if( number <= 0 || number > 26 )
        throw new ArgumentOutOfRangeException( "number", number, "Value can not be less 0 and greater 26" );

      char letter = ( char ) ( DEF_A_ASCII_INDEX + number );
      builder.Append( letter );
    }
    #endregion
  }
}
