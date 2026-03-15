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

#define CACHE
#define ALTER_CASE

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Syncfusion.HTMLUI.Base
{
  /// <summary>
  /// Specifies a new line in different OS.
  /// </summary>
  public enum NewLineStyle
  {
    /// <summary>
    /// Windows new line style.
    /// </summary>
    Windows, // \r\n
    /// <summary>
    ///  Mac new line style.
    /// </summary>
    Mac, // \r
    /// <summary>
    /// Unix new line style.
    /// </summary>
    Unix, // \n\r
    /// <summary>
    ///  Control new line style.
    /// </summary>
    Control  // \n
  }

  /// <summary>
  /// Stream which returns and writes data as string tokens into/from file.
  /// Stream supports encoding and data buffering.
  /// </summary>
  public class TokenStream
    : Stream
    , IEnumerable
  {
    #region Class constants
    /// <summary>
    /// Stream buffer used for optimizing speed of read/write operations.
    /// </summary>
    protected const int DEF_BUFFER_SIZE = 8192;
    /// <summary>
    ///
    /// </summary>
    protected const int DEF_READ_CHAR_BUFFER_SIZE = 128;
    /// <summary>
    /// Array of characters which delimit tokens.
    /// </summary>
    public readonly char[] DEF_TOKEN_SPLITS = "\t !\"#$%&'()*+,-./:;<=>?[\\]^{|}~`\r\n".ToCharArray();
    /// <summary>
    /// Array of atomic tokens which can take more than one symbol.
    /// </summary>
    public readonly string[] DEF_MULTI_TOKEN_SPLIT = new string[]
    {
      Environment.NewLine,
    };
    /// <summary>
    /// An array of known preambles of encodings.
    /// </summary>
    private Array[] _preambles = new Array[]
    {
      Encoding.Default.GetPreamble(),
      Encoding.BigEndianUnicode.GetPreamble(),
      Encoding.UTF7.GetPreamble(),
      Encoding.UTF8.GetPreamble(),
      Encoding.Unicode.GetPreamble(),
      Encoding.ASCII.GetPreamble(),
    };
    /// <summary>
    /// An array of known encodings.
    /// </summary>
    private Encoding[] _encodings = new Encoding[]
    {
      Encoding.Default,
      Encoding.BigEndianUnicode,
      Encoding.UTF7,
      Encoding.UTF8,
      Encoding.Unicode,
      Encoding.ASCII
    };
    #endregion

    #region Internal Classes declaration
    /// <summary>
    /// This class implements enumerator which uses its own algorithm for extracting
    /// data from input stream. Algorithm is a little faster than used in
    /// TokenStream class, but it does not support multi-symbols tokens splitter.
    /// </summary>
    protected class StreamTokenEnum
      : IEnumerator
      , IDisposable
    {
      #region Class members
      /// <summary>
      /// Parent TokenStream.
      /// </summary>
      private TokenStream m_parent;
      /// <summary>
      /// Current Position Of Reading.
      /// </summary>
      private long m_lPosition;
      //current snippet will be here
      private string m_current;
      #endregion

      #region Class Initialize/Finalize methods
      /// <summary>
      /// Empty Constructor.
      /// </summary>
      private StreamTokenEnum()
      {
      }
      /// <summary>
      /// Destructor. Disposes all resources.
      /// </summary>
      ~StreamTokenEnum()
      {
        Dispose();
      }
      /// <summary>
      /// Constructor with specified parent.
      /// </summary>
      public StreamTokenEnum( TokenStream parent )
      {
        m_parent = parent;
        m_lPosition = m_parent.Position;
      }
      /// <summary>
      /// Clears all resources.
      /// </summary>
      public void Dispose()
      {
        if( m_parent != null )
        {
          m_parent = null;
        }
      }
      #endregion

      #region Properties
      /// <summary>
      /// Returns current token.
      /// </summary>
      public object Current
      {
        get
        {
          if( m_current == null )
            throw new ArgumentException( "Call first Reset() and then ModeNext() methods. Incorrect use of interface." );

          return m_current;
        }
      }
      #endregion

      #region IEnumerator Members
      /// <summary>
      /// At the beginning Of Enumeration, clears the buffer.
      /// </summary>
      public void Reset()
      {
        m_parent.Position = m_lPosition;
      }
      /// <summary>
      /// NOTE : newline - is one token
      /// </summary>
      public bool MoveNext()
      {
        m_current = m_parent.ReadToken();
        return ( m_current != null && m_current != string.Empty );
      }
      #endregion
    }
    /// <summary>
    /// This class is used for multicharacter token search operations. It holds
    /// case sensitive information.
    /// </summary>
    protected class FastChar
    {
      #region Class members
      /// <summary>
      /// Current character from the Stream.
      /// </summary>
      private char m_ch;
      /// <summary>
      ///
      /// </summary>
      private bool m_bHasEnd;
      /// <summary>
      /// Holds the rest of characters in the word.
      /// </summary>
      private IDictionary m_subchars;
      #endregion

      #region Class Properties
      /// <summary>
      /// Returns one character from the stream.
      /// </summary>
      public char Char
      {
        get
        {
          return m_ch;
        }
      }

      /// <summary>
      /// Indicates whether SubChars collection contains string end symbol '\uffff'.
      /// </summary>
      public bool HasEnd
      {
        get
        {
          return m_bHasEnd;
        }
        set
        {
          m_bHasEnd = value;
        }
      }
      /// <summary>
      /// Returns sub-chars ( the rest of chars in the word )
      /// </summary>
      public IDictionary SubChars
      {
        get
        {
          return m_subchars;
        }
      }

      #endregion

      #region Class Initialize/Finalize methods
      /// <summary>
      /// Default constructor diasbled for end user.
      /// </summary>
      private FastChar()
      {

      }
      /// <summary>
      /// Overloaded constructor.
      /// </summary>
      /// <param name="ch"></param>
      public FastChar( char ch ) : this()
      {
        m_ch = ch;
        m_subchars = new Hashtable();
      }
      /// <summary>
      /// Constructor for inheritors.
      /// </summary>
      /// <param name="ch">char</param>
      /// <param name="dict">Collection for sub-chars</param>
      public FastChar( char ch, IDictionary dict ) : this()
      {
        if( dict == null )
          throw new ArgumentNullException( "dict" );

        m_ch = ch;
        m_subchars = dict;
      }
      #endregion
    }
    /// <summary>
    /// This class is used for multi-symbol token search operation. It holds
    /// case insensitive information.
    /// </summary>
#pragma warning disable
    protected class FastInsensChar : FastChar
    {
      #region Class Initialize/Finalize methods
      /// <summary>
      /// Creates new object.
      /// </summary>
      /// <param name="ch">Character symbol.</param>
      public FastInsensChar( char ch )
        : base( ch, new Hashtable( new CaseInsensCharProvider(), new CaseInsensCharComparer() ) )
      {
      }
      #endregion
    }
    /// <summary>
    /// Fixes Microsoft bug for Case insensitive types. Microsoft provides only
    /// case insensitive hash provider for strings; characters by itself are not supported.
    /// </summary>
    protected class CaseInsensCharProvider : IHashCodeProvider
    {
      #region IHashCodeProvider method
      /// <summary>
      /// Calculates hash code for character.
      /// </summary>
      /// <param name="obj">Object for which hash must be calculated</param>
      /// <returns></returns>
      public int GetHashCode( object obj )
      {
        Debug.Assert( obj is char, "CaseInsensCharProvider can be used only for char objects" );

        return ( int )char.ToLower(( char )obj);
      }
      #endregion
    }
    /// <summary>
    /// This class provides the functionality for comparing two characters. It fixes Microsoft
    /// bug: case insensitive help provided only for strings.
    /// </summary>
#pragma warning enable
    protected class CaseInsensCharComparer : IComparer
    {
      #region IComparer method
      /// <summary>
      /// Compares two characters by its values - case insensitive
      /// </summary>
      /// <param name="x">first value</param>
      /// <param name="y">second value</param>
      /// <returns>0 - if equal; -1 if x is less than y; 1 if x is greater than y</returns>
      public int Compare( object x, object y )
      {
        Debug.Assert( x is char, "first object is not a char" );
        Debug.Assert( y is char, "second object is not a char" );

        char ch1 = char.ToLower( ( char )x );
        char ch2 = char.ToLower( ( char )y );

        return ( int )ch1 - ( int )ch2;
      }
      #endregion
    }
    /// <summary>
    /// This class helps users to sort arrays by Length property value.
    /// </summary>
    protected class ArrayLengthComparer : IComparer
    {
      #region IComparer Members
      /// <summary>
      /// Compares two Array classes by Length property values.
      /// </summary>
      /// <param name="x">first parameter</param>
      /// <param name="y">second parameter</param>
      /// <returns>0 - if equal; -1 if x is less than y; 1 if x is greater than y</returns>
      public int Compare( object x, object y )
      {
        Debug.Assert( x is Array, "first object is not a Array" );
        Debug.Assert( y is Array, "second object is not a Array" );

        return (( Array )y).Length - (( Array )x).Length;
      }
      #endregion
    }
    #endregion

    #region Class members
    /// <summary>
    /// Count of bytes in the beginning of the stream, that must be skipped. (Preambula)
    /// </summary>
    private int m_iSkippedBytes = 0;
    /// <summary>
    /// Array of sorted split characters - used by BinarySearch method.
    /// </summary>
    private char[]        m_OneSorted;
    /// <summary>
    /// Temporary storage for token extracting. Used because it does not
    /// reallocate each string set.
    /// </summary>
    private StringBuilder m_TokenBuffer = new StringBuilder();
    /// <summary>
    /// Array of multi-symbol tokens.
    /// </summary>
    private string[]      m_MultiSorted;
    /// <summary>
    /// Encoding that is to be used for read and write operations.
    /// </summary>
    private Encoding      m_encoding = Encoding.Default;
    /// <summary>
    /// Hash which helps detect multi-symbol token splitters.
    /// </summary>
    private Hashtable     m_multiFirst = new Hashtable();
    /// <summary>
    /// Buffer stream. It can be simply replaced by this reference.
    /// </summary>
    private Stream        m_buffer;
    /// <summary>
    /// New line splitter.
    /// </summary>
    protected string      m_strEndOfLine = Environment.NewLine;
    /// <summary>
    /// Storage of EndLineStyle property.
    /// </summary>
    private NewLineStyle  m_style = NewLineStyle.Windows;
    /// <summary>
    /// Storage of MaxMultiTokenLength property.
    /// </summary>
    private int           m_maxMultiLen = -1;
    /// <summary>
    /// Storage of MinMultiTokenLength property.
    /// </summary>
    private int           m_minMultiLen = -1;
    /// <summary>
    /// Indicates whether stream extracts multichar token in case sensitive
    /// or insensitive mode.
    /// </summary>
    private bool          m_bCaseSensitive = true;
    /// <summary>
    /// Indicates whether class has rights to dispose m_buffer
    /// variable; otherwise Internal stream does not belong to class and must
    /// be freed by user.
    /// </summary>
    private bool          m_bControlStream;
    /// <summary>
    /// Stack that is to be used for multi-character token extract operations.
    /// </summary>
    private Stack         m_stack = new Stack();
    /// <summary>
    /// Temporary buffer for extracting characters from source stream.
    /// </summary>
    private byte[]        m_readCharBuffer = new byte[ DEF_READ_CHAR_BUFFER_SIZE ];
#if CACHE
    /// <summary>
    /// Variable store extracted from stream character on peek operation.
    /// </summary>
    private int           m_chPeekChar = -1;
    /// <summary>
    /// Indicates whether cache contains some data.
    /// </summary>
    private bool          m_bPeekChar;
    /// <summary>
    /// Variable store token extracted from stream by Peek operation.
    /// </summary>
    private string        m_strPeekToken = null;
    /// <summary>
    /// Indicates whether cache contains some data.
    /// </summary>
    private bool          m_bPeekToken;
#endif
    #endregion

    #region Class Properties
    /// <summary>
    /// Returns the maximum size of multi-symbol token.
    /// </summary>
    public virtual int MaxMultiTokenLength
    {
      get
      {
        if( m_MultiSorted == null || m_MultiSorted.Length == 0 ) return 0;
        return m_maxMultiLen;
      }
    }
    /// <summary>
    /// Returns the minimum size of multi-symbol token.
    /// </summary>
    public virtual int MinMultiTokenLength
    {
      get
      {
        if( m_MultiSorted == null || m_MultiSorted.Length == 0 ) return 0;
        return m_minMultiLen;
      }
    }
    /// <summary>
    /// Gets or sets an array of characters which can split tokens in stream.
    /// Each symbol from this array will be returned by Stream as token
    /// with one char, for more complex constructions must use
    /// MultiCharTokens property values.
    /// </summary>
    public virtual char[] OneCharTokens
    {
      get
      {
        return m_OneSorted;
      }
      set
      {
        if( value != null )
        {
          m_OneSorted = new char[ value.Length ];
          value.CopyTo( m_OneSorted, 0 );
          Array.Sort( value, m_OneSorted );

#if CACHE
          m_bPeekChar = false;  // reset char cache
          m_bPeekToken = false; // reset token cache
#endif
        }
      }
    }
    /// <summary>
    /// Gets or sets multi-symbol atomic tokens. This property can used by
    /// top-level abstractions for extracting special lexems, like: ++, --,
    /// +=, -=, !=, ==, *=, etc.
    /// </summary>
    public virtual string[] MultiCharTokens
    {
      get
      {
        return m_MultiSorted;
      }
      set
      {
        if( value != null )
        {
          m_MultiSorted = new string[ value.Length ];
          value.CopyTo( m_MultiSorted, 0 );
          Array.Sort( value, m_MultiSorted );

          m_multiFirst.Clear();

#if ALTER_CASE
          // use alternative method of caseinsensitive search
          if( m_bCaseSensitive == false )
            BuildFastCharTreeEx( m_multiFirst, m_MultiSorted );
          else
            BuildFastCharTree( m_multiFirst, m_MultiSorted );
#else
          BuildFastCharTree( m_multiFirst, m_MultiSorted );
#endif

#if CACHE
          m_bPeekChar = false;  // reset char cache
          m_bPeekToken = false; // reset token cache
#endif
        }
      }
    }
    /// <summary>
    /// Indicates whether Multi-character Tokens are case sensitive or not.
    /// Default is case sensitive - TRUE.
    /// This property greatly reduces performance of TokenStream class
    /// an hence used only where it is really needed.
    /// </summary>
    public virtual bool CaseSensitive
    {
      get
      {
        return m_bCaseSensitive;
      }
      set
      {
        if( value != m_bCaseSensitive )
        {
          m_bCaseSensitive = value;

          if( m_bCaseSensitive == true )
          {
            m_multiFirst = new Hashtable();
          }
          else
          {
#if ALTER_CASE
            m_multiFirst = new Hashtable();
#else
            m_multiFirst = new Hashtable(
              new CaseInsensCharProvider(),
              new CaseInsensCharComparer() );
#endif
          }

#if ALTER_CASE
          // use alternative method of caseinsensitive search
          if( m_bCaseSensitive == false )
            BuildFastCharTreeEx( m_multiFirst, m_MultiSorted );
          else
            BuildFastCharTree( m_multiFirst, m_MultiSorted );
#else
          BuildFastCharTree( m_multiFirst, m_MultiSorted );
#endif

#if CACHE
          m_bPeekChar = false;  // reset char cache
          m_bPeekToken = false; // reset token cache
#endif
        }
      }
    }
    /// <summary>
    /// Gets or sets the encoding that is to be used for file reading.
    /// </summary>
    public virtual Encoding Encoding
    {
      get
      {
        return m_encoding;
      }
      set
      {
        if( value != m_encoding )
        {
          m_encoding = value;

#if CACHE
          m_bPeekChar = false;  // reset char cache
          m_bPeekToken = false; // reset token cache
#endif
        }
      }
    }
    /// <summary>
    /// Gets or sets position in stream.
    /// </summary>
    public override long Position
    {
      get
      {
        return m_buffer.Position;
      }
      set
      {
        if( value == 0 )
          value = m_iSkippedBytes;

        if( value != m_buffer.Position )
        {
#if CACHE
          m_bPeekChar = false;  // reset char cache
          m_bPeekToken = false; // reset token cache
#endif

          m_buffer.Position = value;
        }
      }
    }
    /// <summary>
    /// Indicates whether TokenStream can Seek.
    /// </summary>
    public override bool CanSeek
    {
      get
      {
        return m_buffer.CanSeek;
      }
    }
    /// <summary>
    /// Indicates whether TokenStream can Read.
    /// </summary>
    public override bool CanRead
    {
      get
      {
        return m_buffer.CanRead;
      }
    }
    /// <summary>
    /// Indicates whether TokenStream can Write.
    /// </summary>
    public override bool CanWrite
    {
      get
      {
        return m_buffer.CanWrite;
      }
    }
    /// <summary>
    /// Returns the number of symbols in our Stream.
    /// </summary>
    public override long Length
    {
      get
      {
        return m_buffer.Length;
      }
    }

    /// <summary>
    /// On end of line, string update recalculates lines in stream.
    /// </summary>
    public virtual string NewLine
    {
      get
      {
        return m_strEndOfLine;
      }
      set
      {
        if( value != m_strEndOfLine )
        {
          m_strEndOfLine = value;

#if CACHE
          m_bPeekChar = false;  // reset char cache
          m_bPeekToken = false; // reset token cache
#endif
        }
      }
    }
    /// <summary>
    /// Gets or sets End line style ( For different OS ).
    /// </summary>
    public virtual NewLineStyle EndLineStyle
    {
      get
      {
        return m_style;
      }
      set
      {
        this.NewLine = GetNewLineString( value );
        m_style = value;
      }
    }
    #endregion

    #region IEnumerable Members
    /// <summary>
    /// Returns enumerator which gives us stream data by tokens.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator GetEnumerator()
    {
      return new StreamTokenEnum( this );
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Overloaded. Constructor with a file name in open mode.
    /// </summary>
    public TokenStream( string fileName )
      : this( fileName, FileMode.Open, FileAccess.Read, FileShare.Read, DEF_BUFFER_SIZE )
    {
    }
    /// <summary>
    /// Constructor allows to control mode of file open operation.
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="mode">A FileMode constant that determines how to open or create the file.</param>
    public TokenStream( string fileName, FileMode mode )
      : this( fileName, mode, FileAccess.Read, FileShare.Read, DEF_BUFFER_SIZE )
    {
    }
    /// <summary>
    /// Constructor allows to control File open mode and access flags.
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="mode">A FileMode constant that determines how to open or create the file. </param>
    /// <param name="access">A FileAccess constant that determines how the file can be
    /// accessed by the TokenStream object. This gets the CanRead and CanWrite properties
    /// of the FileStream object. CanSeek is true if path specifies a disk file</param>
    public TokenStream( string fileName, FileMode mode, FileAccess access )
      : this( fileName, mode, access, FileShare.Read, DEF_BUFFER_SIZE )
    {
    }
    /// <summary>
    /// Constructor allows to override file mode, access and share flags.
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="mode">A FileMode constant that determines how to open or create the file. </param>
    /// <param name="access">A FileAccess constant that determines how the file can be
    /// accessed by the TokenStream object. This gets the CanRead and CanWrite properties
    /// of the FileStream object. CanSeek is true if path specifies a disk file</param>
    /// <param name="share">A FileShare constant that determines how the file will be shared
    /// by processes</param>
    public TokenStream( string fileName, FileMode mode, FileAccess access, FileShare share )
      : this( fileName, mode, access, share, DEF_BUFFER_SIZE )
    {
    }
    /// <summary>
    /// Open stream. Constructor allows to specify parameter of stream open.
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="mode">A FileMode constant that determines how to open or create the file. </param>
    /// <param name="access">A FileAccess constant that determines how the file can be
    /// accessed by the TokenStream object. This gets the CanRead and CanWrite properties
    /// of the FileStream object. CanSeek is true if path specifies a disk file</param>
    /// <param name="share">A FileShare constant that determines how the file will be shared
    /// by processes</param>
    /// <param name="bufferSize">Size of internal buffer used for read/write operations
    /// optimizations</param>
    public TokenStream( string fileName, FileMode mode, FileAccess access, FileShare share, int bufferSize )
      : this( new FileStream( fileName, mode, access, share ), true, bufferSize )
    {
    }
    /// <summary>
    /// Constructor. Initializes new object.
    /// </summary>
    /// <param name="input">Input data Stream</param>
    public TokenStream( Stream input )
      : this( input, null, false, DEF_BUFFER_SIZE )
    {
    }
    /// <summary>
    /// Creates Token stream from other stream.
    /// Encoding of input stream class is detected automatically.
    /// </summary>
    /// <param name="input">source stream</param>
    /// <param name="controlStream">Indicates whether TokenStream instance must close input stream on
    /// own.</param>
    public TokenStream( Stream input, bool controlStream )
      : this( input, controlStream, DEF_BUFFER_SIZE )
    {
    }
    /// <summary>
    /// Creates Token stream from other stream with user specified encoding.
    /// </summary>
    /// <param name="input">input stream</param>
    /// <param name="encoding">If encoding of input stream known then you can
    /// specify it; otherwise send null value for auto-detection</param>
    public TokenStream( Stream input, Encoding encoding )
      : this( input, encoding, false, DEF_BUFFER_SIZE )
    {
    }
    /// <summary>
    /// Creates Token stream from other stream with user specified encoding.
    /// Constructor indicates whether to specify class control input stream.
    /// </summary>
    /// <param name="input">input stream</param>
    /// <param name="encoding">If encoding of input stream is known then you can
    /// specify it; otherwise send null value for auto-detection</param>
    /// <param name="controlStream">Indicates whether TokenStream instance must close input stream on
    /// own.</param>
    public TokenStream( Stream input, Encoding encoding, bool controlStream )
      : this( input, encoding, controlStream, DEF_BUFFER_SIZE )
    {
    }
    /// <summary>
    /// Constructor. Initializes new object.
    /// Encoding of input stream class is detected automatically.
    /// </summary>
    /// <param name="input">Input data Stream</param>
    /// <param name="controlStream">Indicates whether TokenStream instance must close input stream on
    /// own.</param>
    /// <param name="bufferSize">Size of the Stream</param>
    public TokenStream( Stream input, bool controlStream, int bufferSize )
      : this( input, null, controlStream, bufferSize )
    {
    }
    /// <summary>
    /// Constructor. Initializes new object.
    /// </summary>
    /// <param name="input">Input data Stream</param>
    /// <param name="encoding">If encoding of input stream is known then you can
    /// specify it; otherwise send null value for auto-detection</param>
    /// <param name="controlStream">Indicates whether TokenStream instance must close input stream on
    /// own.</param>
    /// <param name="bufferSize">Size of the Stream</param>
    public TokenStream( Stream input, Encoding encoding, bool controlStream, int bufferSize )
    {
      if( input == null )
        throw new ArgumentNullException( "input" );

      m_bControlStream = controlStream;

      // here it's look like double buffering
      if( input is MemoryStream || input is BufferedStreamEx || bufferSize == -1 )
      {
        m_buffer = input;
      }
      else
      {
        m_buffer = new BufferedStreamEx( input, bufferSize );
      }

      if( !this.CanSeek )
        throw new ArgumentException( "File open flags make seek operation unavailable. "+
          "To work properly File must support Seek operation." );

      Array.Sort( _preambles, _encodings, new ArrayLengthComparer() );

      // create sorted array of split characters for BinarySearch method
      this.OneCharTokens = DEF_TOKEN_SPLITS;
      this.MultiCharTokens = DEF_MULTI_TOKEN_SPLIT;

      m_encoding = ( encoding == null ) ? DetectFileEncoding() : encoding;
      NewLineStyle style = DetectFileNewLineStyle();

      if( m_style != style ) this.EndLineStyle = style;
    }
    /// <summary>
    /// Initializes TokenStream from string input.
    /// </summary>
    /// <param name="buffer">String with input data.</param>
    /// <returns>TokenStream Object</returns>
    public static TokenStream FromString( string buffer )
    {
      Encoding enc = Encoding.Unicode;
      MemoryStream ms = new MemoryStream( enc.GetBytes( buffer ), false );
      TokenStream ts = new TokenStream( ms, enc, true );
      return ts;
    }
#pragma warning disable
    /// <summary>
    /// Clears resources of Token stream class.
    /// </summary>
    public void Dispose()
    {
      if( m_bControlStream ) (( IDisposable )m_buffer).Dispose();
      m_readCharBuffer = null;
      m_stack = null;
    }
    #endregion
#pragma warning enable
    #region Save/Write Operations
    /// <summary>
    /// Writes data into Stream.
    /// </summary>
    /// <param name="buffer">Data which will be written.</param>
    /// <param name="offset">Start Position</param>
    /// <param name="count">Number of Symbols</param>
    public override void Write( byte[] buffer, int offset, int count )
    {
#if CACHE
      m_bPeekChar = false;  // reset char cache
      m_bPeekToken = false; // reset token cache
#endif

      m_buffer.Write( buffer, offset, count );
    }
    #endregion

    #region Read Operations
    /// <summary>
    /// Peeks one byte from stream.
    /// </summary>
    /// <returns>-1 if end of stream is reached; one byte from stream otherwise.</returns>
    public virtual int    Peek()
    {
#if CACHE
      m_bPeekChar = false;  // reset char cache
      m_bPeekToken = false; // reset token cache
#endif

      long lOld = m_buffer.Position;
      int output = m_buffer.ReadByte();
      m_buffer.Position = lOld;

      return output;
    }
    /// <summary>
    /// Extracts one char from stream.
    /// </summary>
    /// <returns>-1 if end of stream is reached; integer value of character otherwise.</returns>
    public virtual int    PeekChar()
    {
#if CACHE
      // return cached value
      if( m_bPeekChar ) return m_chPeekChar;
#endif

      long lOld = m_buffer.Position;
      int output = ReadChar();
      m_buffer.Position = lOld;

#if CACHE
      m_chPeekChar = output;
      m_bPeekChar  = true;
#endif

      return output;
    }
    /// <summary>
    /// Peeks Token from the Stream.
    /// </summary>
    /// <returns>string.Empty or null if end of stream is reached; extracted token otherwise.</returns>
    public virtual string PeekToken()
    {
#if CACHE
      // return cached value
      if( m_bPeekToken ) return m_strPeekToken;
#endif

      long lOld = m_buffer.Position;
      string output = ReadToken();
      m_buffer.Position = lOld;

#if CACHE
      m_strPeekToken = output;
      m_bPeekToken = true;
#endif

      return output;
    }
    /// <summary>
    /// Reads one byte from stream.
    /// </summary>
    /// <returns>-1 if end of stream is reached; one byte from stream otherwise.</returns>
    public virtual int    Read()
    {
#if CACHE
      m_bPeekChar = false;  // reset char cache
      m_bPeekToken = false; // reset token cache
#endif

      int output = m_buffer.ReadByte();

      return output;
    }
    /// <summary>
    /// Reads one char from stream and shifts position of stream to next character.
    /// </summary>
    /// <returns>-1 if end of stream; integer value of character otherwise.</returns>
    public virtual int    ReadChar()
    {
#if CACHE
      // return previously cached char
      if( m_bPeekChar )
      {
        if( m_chPeekChar == -1 )
          throw new ArgumentException( "m_chPeekChar" );

        int count = m_encoding.GetByteCount( new char[]{ (char)m_chPeekChar } );
        m_buffer.Position += count; // adjust position

        m_bPeekChar = false;  // reset char cache
        m_bPeekToken = false; // reset token cache

        return m_chPeekChar;
      }
#endif

      char[] data = new char[ 1 ];

      int result = this.Read( data, 0, 1 );
      if( result == 0 ) return -1; // end of stream reached

      return ( int )data[0];
    }
    /// <summary>
    /// Reads single line.
    /// </summary>
    /// <returns>String.Empty if end of stream is reached; line from file otherwise.</returns>
    public virtual string ReadLine()
    {
      StringBuilder buffer = new StringBuilder();
      string curToken;

      while( ( curToken = ReadToken() ) != m_strEndOfLine )
      {
        if( null != curToken )
        {
          buffer.Append( curToken );
        }
        else // it means end of file found
          break;
      }

      return buffer.ToString();
    }
    /// <summary>
    /// Moves current position in the stream to the next line.
    /// </summary>
    public virtual void SkipLine()
    {
      string curToken;

      while( ( curToken = ReadToken() ) != m_strEndOfLine )
      {
        if( null == curToken )
          break;
      }
    }

    /// <summary>
    /// Reads one token from stream.
    /// </summary>
    /// <returns>Null if end of stream is reached; token string otherwise.</returns>
    public virtual string ReadToken()
    {
      if( !m_buffer.CanRead )
        throw new ArgumentException( "file opened not in read mode" );

      if( !m_buffer.CanSeek )
        throw new ArgumentException( "file opened not without seek operation support" );

#if CACHE
      // return previously cached token
      if( m_bPeekToken )
      {
        // If end of file reached
        if( m_strPeekToken != null )
        {
          int count = m_encoding.GetByteCount( m_strPeekToken );
          m_buffer.Position += count; // adjust position on token size

          m_bPeekChar = false;  // reset char cache
          m_bPeekToken = false; // reset token cache
        }

        return m_strPeekToken;
      }
#endif

      m_TokenBuffer.Length = 0;
      string token;
      char[] mtoken = new char[ MaxMultiTokenLength ];

      while( PeekChar() >= 0 ) // check is it end of stream or not
      {
        char ch = Convert.ToChar( PeekChar() );

        bool bSplit = IsSplitTokenChar( ch );
        if( bSplit )
        {
          // if this is end of previous token
          if( m_TokenBuffer.Length > 0 ) return m_TokenBuffer.ToString();
        }

        if( m_multiFirst.Contains( ch ) )
        {
          m_TokenBuffer.Append( ch );

          // check for multi-symbols tokens
          long lOldPos = m_buffer.Position;

          int read = Read( mtoken, 0, mtoken.Length );
          int size = GetMultiToken( mtoken, out token );

          // token string can have differ bytes size - according to specified encoding
          m_buffer.Position = lOldPos + (( size < 0 ) ? 0 : m_encoding.GetByteCount( token ) );

          if( size > 0 )
          {
            if( m_TokenBuffer.Length > 1  )
            {
              m_TokenBuffer.Length--;
              m_buffer.Position = lOldPos;
              return m_TokenBuffer.ToString();
            }
            else
              return token;
          }

          m_TokenBuffer.Length--;
        }

        m_TokenBuffer.Append( ch );
        ReadChar(); // move position to next char in stream
        if( bSplit ) return m_TokenBuffer.ToString();
      }

      return ( m_TokenBuffer.Length > 0 ) ? m_TokenBuffer.ToString() : null;
    }
    /// <summary>
    /// Reads multiple tokens from stream into array.
    /// </summary>
    /// <param name="buffer">Array where extracted tokens must be placed.</param>
    /// <param name="position">Start position in the array.</param>
    /// <param name="length">Quantity of tokens to extract.</param>
    /// <returns>Quantity of extracted tokens.</returns>
    public virtual int    ReadTokens( string[] buffer, int position, int length )
    {
      if( buffer == null )
        throw new ArgumentNullException( "buffer" );

      if( position < 0 || position >= buffer.Length )
        throw new ArgumentOutOfRangeException( "position", position, "Position is out of buffer array bounds" );

      if( position + length > buffer.Length )
        throw new ArgumentOutOfRangeException( "length", length, "Length or Position has wrong value. Buffer size is too smal for specified values" );

      int iCount = 0;

      do
      {
        string token = ReadToken();

        if( token == null || token == string.Empty ) break;
        buffer[ position ] = token;
        iCount++;
      }
      while( iCount < length );

      return iCount;
    }
    /// <summary>
    /// Overloaded. Reads data from the Stream.
    /// </summary>
    /// <param name="buffer">Source Stream</param>
    /// <param name="offset">Start position</param>
    /// <param name="count">Number of symbols</param>
    /// <returns>number of extracted characters.</returns>
    public virtual int    Read( char[] buffer, int offset, int count )
    {
      if( buffer == null )
        throw new ArgumentNullException( "buffer" );

      if( offset < 0 || offset >= buffer.Length )
        throw new ArgumentOutOfRangeException( "offset", offset, "Value can not be less 0 and greater buffer.Length" );

      if( count < 0 || count > buffer.Length - offset )
        throw new ArgumentOutOfRangeException( "count", count, "Value can not be less 0 and greater buffer.Length - offset" );

#if CACHE
      m_bPeekChar = false;  // reset char cache
      m_bPeekToken = false; // reset token cache
#endif

      // Get maximum buffer size for needed count of chars
      int size = m_encoding.GetMaxByteCount( count );
      byte[] data = ( size <= DEF_READ_CHAR_BUFFER_SIZE ) ? m_readCharBuffer : new byte[ size ];
      char[] charData = new char[ m_encoding.GetMaxCharCount( data.Length ) ];

      long oldPosition = m_buffer.Position;
      int charsRead = 0;

      while( charsRead < count && m_buffer.Position < m_buffer.Length )
      {
        // Read bytes to byte array
        int bytesRead = m_buffer.Read( data, 0, data.Length );

        // Convert bytes array to chars array
        int charsArrayLength = m_encoding.GetChars( data, 0, bytesRead, charData, 0 );

        // Get count of bytes, needed to encode needed rest of the char array
        int bytesNeeded = m_encoding.GetByteCount( charData, 0, Math.Min( charsArrayLength, count - charsRead ) );

        if( bytesNeeded < bytesRead )
          m_buffer.Position += bytesNeeded - bytesRead;

        if( bytesNeeded == 0 )
          break;

        charsRead += m_encoding.GetChars( data, 0, bytesNeeded, buffer, charsRead );
      }

      return charsRead;
    }
    /// <summary>
    /// Reads data from Stream.
    /// </summary>
    /// <param name="buffer">Input Stream</param>
    /// <param name="offset">Start Position</param>
    /// <param name="count">Number of Symbols.</param>
    /// <returns>Number of extracted bytes.</returns>
    public override int   Read( byte[] buffer, int offset, int count )
    {
#if CACHE
      m_bPeekChar = false;  // reset char cache
      m_bPeekToken = false; // reset token cache
#endif

      return m_buffer.Read( buffer, offset, count );
    }
    #endregion

    #region Token extraction
    /// <summary>
    /// Indicates whether character belongs to one symbol tokens separators.
    /// </summary>
    /// <param name="ch">Character to test</param>
    /// <returns>True if character belongs to split chars; false otherwise.</returns>
    public virtual bool IsSplitTokenChar( char ch )
    {
      return ( Array.BinarySearch( m_OneSorted, ch ) >= 0 );
    }
    /// <summary>
    /// Checks character from array for multi-symbols tokens and returns the
    /// quantity of used characters from array for token. It also returns the token.
    /// In case, nothing is found, it returns -1. It tries to find the longest token
    /// from the list.
    /// </summary>
    /// <param name="ch">Characters to check.</param>
    /// <param name="token">Found token.</param>
    /// <returns>Used characters from input array; -1 otherwise.</returns>
    public virtual int  GetMultiToken( char[] ch, out string token )
    {
      if( ch == null )
        throw new ArgumentNullException( "ch" );

      if( ch.Length < MaxMultiTokenLength )
        throw new ArgumentException( "Size of input array must be equel or greater to MaxMultiTokenLength property value" );

      FastChar  _pos;
      IDictionary _hash = m_multiFirst;
      Stack  stack = m_stack;
      stack.Clear();

      for( int i=0; i<ch.Length; i++ )
      {
        _pos = _hash[ ch[i] ] as FastChar;
        if( _pos == null ) break;

        _hash = _pos.SubChars;

        // if we find one token, then store it
        if( _pos.HasEnd ) // if( _hash.Contains( '\uffff' ) )
        {
          stack.Push( new string( ch, 0, i+1 ) );

          if( _hash.Count == 1 ) break; // no more symbols
        }
      }

      token = ( stack.Count == 0 ) ? null : ( string )stack.Pop();
      return ( token == null ) ? -1 : token.Length;
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Converts offset to Line number and position in line.
    /// </summary>
    /// <param name="offset">Offset which must be converted to line and position values</param>
    /// <param name="line">Line part of offset.</param>
    /// <param name="position">Position part of offset.</param>
    public virtual void ConvertToLinePosition( long offset, out int line, out int position )
    {
      if( offset < 0 || offset > this.Length )
        throw new ArgumentOutOfRangeException( "offset", offset, "Value can not be less 0 and greater this.Length" );

      long lOldPos = this.Position;

      this.Position = 0;  // start from the bigining
      line = CalculateLineCount( this.NewLine, offset, out position );

      this.Position = lOldPos;
    }
    /// <summary>
    /// Converts enumeration to its string representation.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>End line string.</returns>
    public virtual string GetNewLineString( NewLineStyle value )
    {
      switch( value )
      {
        case NewLineStyle.Windows: return "\r\n";
        case NewLineStyle.Mac:     return "\r";
        case NewLineStyle.Unix:    return "\n\r";
        case NewLineStyle.Control: return "\n";
      }

      throw new ArgumentException( "must be unreachable code" );
    }
    /// <summary>
    /// Checks file and tries to detect which line end code is used in file.
    /// </summary>
    /// <returns>In case of error, NewLineStyle.Windows style is returned.</returns>
    public virtual NewLineStyle DetectFileNewLineStyle()
    {
      long lOldPos = this.Position;
      NewLineStyle output = NewLineStyle.Windows;

      do
      {
        int iCode = ReadChar();
        if( iCode <= 0 ) break;

        if( '\r' == iCode )
        {
          iCode = ReadChar();
          if( iCode <= 0 ) break;

          if( '\n' == iCode )
          {
            output = NewLineStyle.Windows;
            break;
          }
          else
          {
            output = NewLineStyle.Mac;
            break;
          }
        }
        else if( '\n' == iCode )
        {
          iCode = ReadChar();
          if( iCode <= 0 ) break;

          if( '\r' == iCode )
          {
            output = NewLineStyle.Unix;
            break;
          }

          output = NewLineStyle.Control;
          break;
        }
      }
      while( true );

      this.Position = lOldPos;
      return output;
    }
    /// <summary>
    /// Detects file encoding.
    /// </summary>
    /// <returns>Detected file encoding by preambles.</returns>
    public virtual Encoding DetectFileEncoding()
    {
      long lOldPos = this.Position;
      Encoding output = Encoding.Default;

      byte[] test = new byte[ (( Array )_preambles.GetValue(0)).Length ];
      int read = this.Read( test, 0, test.Length );

      for( int i=0; i<_preambles.Length; i++ )
      {
        byte[] preamble = ( byte[] )_preambles.GetValue( i );

        if( CompareArrays( test, preamble ) == 0 )
        {
          output = _encodings[ i ];

          m_iSkippedBytes = preamble.Length;

          if( output == Encoding.ASCII )
            output = Encoding.Default;

          lOldPos += preamble.Length;
          break;
        }
      }

      this.Position = lOldPos;

      return output;
    }

    /// <summary>
    /// Compares two arrays and returns the result.
    /// </summary>
    /// <param name="source">Source array</param>
    /// <param name="preamble">Array with preamble symbols</param>
    /// <returns>0 if they are identical; -1 if source array is less than
    /// preamble array.</returns>
    private int CompareArrays( byte[] source, byte[] preamble )
    {
      if( source.Length < preamble.Length ) return -1;

      for( int i=0; i<preamble.Length; i++ )
      {
        int iTest = source[i] - preamble[i];
        if( iTest != 0 ) return iTest;
      }

      return 0;
    }

    /// <summary>
    /// Closes current file.
    /// </summary>
    public override void Close()
    {
#if CACHE
      m_bPeekChar = false;  // reset char cache
      m_bPeekToken = false; // reset token cache
#endif

      m_buffer.Close();
    }
    /// <summary>
    /// Flushes changes from internal buffers to source streams.
    /// </summary>
    public override void Flush()
    {
#if CACHE
      m_bPeekChar = false;  // reset char cache
      m_bPeekToken = false; // reset token cache
#endif

      m_buffer.Flush();
    }

    /// <summary>
    /// Sets position within the stream.
    /// </summary>
    /// <param name="offset">Start position</param>
    /// <param name="origin">Indicates the reference point that is to be used to obtain the new position.</param>
    /// <returns>New position in stream.</returns>
    public override long Seek( long offset, SeekOrigin origin )
    {
#if CACHE
      m_bPeekChar = false;  // reset char cache
      m_bPeekToken = false; // reset token cache
#endif

      return m_buffer.Seek( offset, origin );
    }

    /// <summary>
    /// Sets the position within the Stream.
    /// </summary>
    /// <param name="token">Token</param>
    /// <param name="bBack">Indicates whether to return back.</param>
    /// <param name="origin">Indicates the reference point that is to be used to obtain the new position.</param>
    /// <returns>New position in stream.</returns>
    public virtual  long SeekOnTokenLength( string token, bool bBack, SeekOrigin origin )
    {
      if( token == null )
        throw new ArgumentNullException( "token" );

      if( token.Length == 0 ) return this.Position;

      if( origin == SeekOrigin.Begin && bBack )
        throw new ArgumentException( "bBack or origin parameter has wrong value" );

      if( origin == SeekOrigin.End && !bBack )
        throw new ArgumentException( "bBack or origin parameter has wrong value" );

#if CACHE
      m_bPeekChar = false;  // reset char cache
      m_bPeekToken = false; // reset token cache
#endif

      long count = ( long )m_encoding.GetByteCount( token );
      return this.Seek( ( bBack ) ? -count : count, origin );
    }
    /// <summary>
    /// Sets the length of the Stream.
    /// </summary>
    /// <param name="value">Length of the Stream.</param>
    public override void SetLength( long value )
    {
      m_buffer.SetLength( value );
    }

    /// <summary>
    /// Calculates the number of lines between two positions in the file.
    /// </summary>
    /// <param name="startPos">Start position for calculating.</param>
    /// <param name="endPos">End position for calculating.</param>
    /// <returns>Count of lines.</returns>
    public virtual int GetLinesCount( long startPos, long endPos )
    {
      long OldPosition = this.Position;
      this.Position = startPos;
      int position;
      int lines = CalculateLineCount( this.NewLine, endPos, out position );
      this.Position = OldPosition;
      return lines;
    }

    #endregion

    #region Class Utility methods
    /// <summary>
    /// Creates a tree of FastChar objects which allows to search faster
    /// any word from stream by characters. '\uffff' - symbol indicates end of string
    /// from source array.
    /// </summary>
    /// <param name="source">Hash table which will store FastChar's objects.</param>
    /// <param name="value">Array of strings.</param>
    protected virtual void BuildFastCharTree( Hashtable source, string[] value )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      if( value == null )
        throw new ArgumentNullException( "value" );

      m_minMultiLen = m_maxMultiLen = value[0].Length;

      FastChar    _pos = null;
      IDictionary _hash;

      foreach( string str in value )
      {
        _hash = source;

        m_minMultiLen = Math.Min( m_minMultiLen, str.Length );
        m_maxMultiLen = Math.Max( m_maxMultiLen, str.Length );

        foreach( char ch in str )
        {
          if( !_hash.Contains( ch ) )
          {
            _hash.Add( ch, GetFastChar( ch ) );
          }

          _pos = _hash[ ch ] as FastChar;
          _hash = _pos.SubChars;
        }

        if( !_hash.Contains( '\uffff' ) )
        {
          _hash.Add( '\uffff', GetFastChar( '\uffff' ) );
          _pos.HasEnd = true;
        }
      }
    }
    /// <summary>
    /// Creates a tree of FastChar objects which allows to search faster
    /// any word from stream by characters. '\uffff' - symbol indicates end of string
    /// from source array. Alternative method of FastChar tree building.
    /// </summary>
    /// <param name="source">Hash table which will store FastChar's objects.</param>
    /// <param name="value">Array of strings.</param>
    protected virtual void BuildFastCharTreeEx( Hashtable source, string[] value )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      if( value == null )
        throw new ArgumentNullException( "value" );

      m_minMultiLen = m_maxMultiLen = value[0].Length;

      FastChar    _pos = null, _pos2 = null;
      IDictionary _hash;

      bool old = m_bCaseSensitive;
      m_bCaseSensitive = true;

      // infill hash by lower case symbols
      foreach( string str in value )
      {
        _hash = source;

        m_minMultiLen = Math.Min( m_minMultiLen, str.Length );
        m_maxMultiLen = Math.Max( m_maxMultiLen, str.Length );

        foreach( char ch in str )
        {
          char chrL = char.ToLower( ch );
          char chrU = char.ToUpper( ch );

          if( !_hash.Contains( chrL ) )
          {
            FastChar fc = new FastChar( chrL );
            _hash.Add( chrL, fc );

            if( chrL != chrU ) _hash.Add( chrU, new FastChar( chrU, fc.SubChars ) );
          }

          _pos = _hash[ chrL ] as FastChar;
          _pos2= _hash[ chrU ] as FastChar;

          _hash = _pos.SubChars;
        }

        if( !_hash.Contains( '\uffff' ) )
        {
          _hash.Add( '\uffff', GetFastChar( '\uffff' ) );
          _pos.HasEnd = true;
          _pos2.HasEnd = true;
        }
      }

      m_bCaseSensitive = old;
    }
    /// <summary>
    /// Based on the CaseSensitive property value, returns case sensitive or
    /// case insensitive FastChar class instance.
    /// </summary>
    /// <param name="ch">Initializes new instance by character.</param>
    /// <returns>Reference on new instance of FastChar class.</returns>
    protected virtual FastChar GetFastChar( char ch )
    {
      if( m_bCaseSensitive )
        return new FastChar( ch );

      return new FastInsensChar( ch );
    }
    /// <summary>
    /// Calculates and returns the number of lines in stream.
    /// </summary>
    /// <returns>Returns total lines count in file.</returns>
    protected virtual int CalculateLineCount( string endOfLine )
    {
      int position;
      return CalculateLineCount( endOfLine, this.Length, out position );
    }
    /// <summary>
    /// Calculates the number of lines in stream till specified position.
    /// </summary>
    /// <param name="endOfLine">String which is interpreted as end of line.</param>
    /// <param name="tillOffset">Make calculations till specified offset.</param>
    /// <param name="position">Returns position part of calculation.</param>
    /// <returns>Number of lines in area from current Position till Offset value.</returns>
    protected virtual int CalculateLineCount( string endOfLine, long tillOffset, out int position )
    {
      if( endOfLine == null )
        throw new ArgumentNullException( "endOfLine" );

      if( endOfLine.Length == 0 )
        throw new ArgumentException( "endOfLine - string can not be empty" );

      if( tillOffset < 0 )
        throw new ArgumentOutOfRangeException( "tillOffset", tillOffset, "Value can not be 0 less " );

      if( !CanRead )
        throw new ArgumentException( "file open mode does not suport read operations" );

      if( !CanSeek )
        throw new ArgumentException( "file open mode does not support seek operations" );

      // any file at least has one line
      int iLineCount = 1;
      long lOldPos = this.Position;
      byte[] data = new byte[ DEF_BUFFER_SIZE ];
      byte[] search = m_encoding.GetBytes( endOfLine );
      byte searchFirst = search[ 0 ];
      int searchLength = search.Length;

      int read = 0;
      long offsetAcc = lOldPos;
      long lLastLineOffset = lOldPos;

      do
      {
        read = m_buffer.Read( data, 0, DEF_BUFFER_SIZE );
        if( read == 0 ) break;

        for( int i=0; i<read && offsetAcc < tillOffset; i++, offsetAcc++ )
        {
          if( data[i] == searchFirst )
          {
            if( read - i >= searchLength )
            {
              bool bIsIt = true;
              for( int j=1; j<searchLength; j++ )
              {
                if( search[j] != data[ i + j ] )
                {
                  bIsIt = false;
                  break;
                }
              }

              if( bIsIt )
              {
                lLastLineOffset = offsetAcc + search.Length;
                iLineCount++;
              }
            }
            else if( read == DEF_BUFFER_SIZE ) // if buffer split end line symbols
            {
              this.Position -= ( read - i ); // shift position to left
              offsetAcc = this.Position;
              break;
            }
          }
        }
      }
      while( read == DEF_BUFFER_SIZE && this.Position < tillOffset );

      position = (int)(tillOffset - lLastLineOffset + 1);
      this.Position = lOldPos;
      return iLineCount;
    }
    #endregion
  }
}