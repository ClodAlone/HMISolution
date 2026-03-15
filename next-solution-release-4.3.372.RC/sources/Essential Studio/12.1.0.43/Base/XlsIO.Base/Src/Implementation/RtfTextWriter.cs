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
using System.IO;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Rtf text writer used for converting rtf string into rtf format.
  /// </summary>
  public class RtfTextWriter : TextWriter
  {
    #region Class constants
    /// <summary>
    /// 0 - font index,
    /// 1 - charset,
    /// 2 - font name.
    /// </summary>
    private const string DEF_FONT = @"{{\f{0}\fnil\fcharset{1} {2};}}";
    /// <summary>
    /// Font attribute.
    /// </summary>
    private const string DEF_FONT_ATTRIBUTE = @"\f{0}\fs{1}";
    /// <summary>
    /// 0 - red component (0-255),
    /// 1 - green,
    /// 2 - blue.
    /// </summary>
    private const string DEF_COLOR_FORMAT = @"\red{0}\green{1}\blue{2};";
    /// <summary>
    /// Underline tags.
    /// </summary>
    private static readonly string[] UnderlineTags = new string[]
    {
      @"\ul",         // Continuous underline.
      @"\ul0",        // Turns off all underlining.
      @"\uld",        // Dotted underline. 
      @"\uldash",     // Dash underline. 
      @"\uldashd",    // Dash dot underline. 
      @"\uldashdd",   // Dash dot dot underline. 
      @"\uldb",       // Double underline. 
      @"\ulhwave",    // Heavy wave underline 
      @"\ulldash",    // Long dash underline 
      @"\ulnone",     // Stops all underlining. 
      @"\ulth",       // Thick underline 
      @"\ulthd",      // Thick dotted underline 
      @"\ulthdash",   // Thick dash underline 
      @"\ulthdashd",  // Thick dash dot underline 
      @"\ulthdashdd", // Thick dash dot dot underline 
      @"\ulthldash",  // Thick long dash underline 
      @"\ululdbwave", // Double wave underline 
      @"\ulw",        // Word underline. 
      @"\ulwave",     // Wave underline. 
    };

    /// <summary>
    /// Strike through tags.
    /// </summary>
    private static readonly string[] StrikeThroughTags = new string[]
    {
      @"\strike1",     //  Strikethrough.
      @"\strike0",     //  Turns off single strikethrough.
      @"\striked1",   // Double strikethrough.
      @"\striked0"    // Turns double strikethrough off.
    };

    /// <summary>
    /// Other tags.
    /// </summary>
    internal static readonly string[] DEF_TAGS = new string[]
    {
//      FontTableStart,
      @"{\fonttbl",
//      FontTableEnd,
      "}",
//      ColorTableStart,
      @"{\colortbl ;",
//      ColorTableEnd,
      "}",
//      BoldOn,
      @"\b",
//      BoldOff,
      @"\b0",
//      ItalicOn,
      @"\i",
//      ItalicOff,
      @"\i0",
//      RtfBegin,
      @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033",
//      RtfEnd,
      "}",
//      GroupStart,
      "{",
//      GroupEnd,
      "}",
//      EndLine,
      @"\par",
//      ForeColor
      @"\cf{0}",
//      BackColor
      @"\cb{0}",
//      SubScript,
      @"\sub",
//      SuperScript,
      @"\super",
//      SubSuperOff,
      @"\nosupersub",
    };
    #endregion

    #region Class members
    /// <summary>
    /// Array list with all used colors.
    /// </summary>
    private List<Color> m_arrColors = new List<Color>();
    /// <summary>
    /// Fonts dictionary. Font - to - font index.
    /// </summary>
    private Dictionary<Font, int> m_hashFonts = new Dictionary<Font, int>();
    /// <summary>
    /// Colors dictionary. Color - to - color index.
    /// </summary>
    private Dictionary<Color, int> m_hashColorTable = new Dictionary<Color, int>();
    /// <summary>
    /// Indicates whether formatting is enabled.
    /// </summary>
    private bool m_bEnableFormatting;
    // TODO: maybe it's better to create two inner writers first for header, second for body.
    /// <summary>
    /// Inner text writer.
    /// </summary>
    private TextWriter m_innerWriter;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bTabsPending;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bEscape;
    /// <summary>
    /// 
    /// </summary>
    private static readonly char[] newLine = "\\line\r\n".ToCharArray();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    public RtfTextWriter()
      : this( new StringWriter(), true )
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enableFormatting"></param>
    public RtfTextWriter( bool enableFormatting )
      : this( new StringWriter(), enableFormatting )
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="underlyingWriter"></param>
    public RtfTextWriter( TextWriter underlyingWriter )
      : this( underlyingWriter, true )
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="underlyingWriter"></param>
    /// <param name="enableFormatting"></param>
    public RtfTextWriter( TextWriter underlyingWriter, bool enableFormatting )
    {
      m_innerWriter = underlyingWriter;
      m_bEnableFormatting = enableFormatting;
    }

    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    protected virtual void OutputTabs()
    {
      if (!m_bTabsPending)
      {
        return;
      }
//      for (int num1 = 0; num1 < m_iIndentLevel; num1++)
//      {
//        m_innerWriter.Write(RtfTextWriter.tab);
//      }
      m_bTabsPending = false;
    }
    /// <summary>
    /// Gets the image RTF.
    /// </summary>
    /// <param name="rtf">The RTF.</param>
    /// <returns></returns>
    protected string GetImageRTF(string rtf)
    {
        int startIndex= rtf.IndexOf("{\\pict");
        int lastIndex= rtf.IndexOf("}",startIndex);
        return rtf.Substring(startIndex,(lastIndex-startIndex)+1);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    private void WriteFontInTable( Font font )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( m_bEnableFormatting )
      {
        if( !m_hashFonts.ContainsKey( font ) )
          throw new ApplicationException( "Collection does not contain font" );

        int iFontIndex = m_hashFonts[ font ];

        Escape = false;
        LOGFONT logFont = new LOGFONT();
        font.ToLogFont( logFont );
        Write( string.Format( DEF_FONT, iFontIndex, logFont.lfCharSet, font.Name ) );
        Escape = true;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iFontId"></param>
    /// <param name="iFontSize"></param>
    private void WriteFontAttribute( int iFontId, int iFontSize )
    {
      if( m_bEnableFormatting )
      {
        Escape = false;

        m_innerWriter.Write( string.Format( DEF_FONT_ATTRIBUTE, iFontId, iFontSize * 2 ) );

        Escape = true;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    private void WriteColorInTable( Color value )
    {
      if( m_bEnableFormatting )
      {
        Escape = false;

        Write( string.Format( DEF_COLOR_FORMAT, value.R,  value.G,  value.B ) );

        Escape = true;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    private void WriteChar( char value )
    {
      if( m_bEscape )
      {
        if( value == '{' )
        {
          m_innerWriter.Write('\\');
          m_innerWriter.Write('{');
        }
        else if( value == '}' )
        {
          m_innerWriter.Write( '\\' );
          m_innerWriter.Write( '}' );
        }
        else if( value == '\\' )
        {
          m_innerWriter.Write( '\\' );
          m_innerWriter.Write( '\\' );
        }
        else
        {
          m_innerWriter.Write( @"\u" + ( ( int ) value ) + "*" );
        }
      }
      else
      {
        m_innerWriter.Write( value );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    private void WriteString( string value )
    {
      if( value == null || value.Length == 0 )
      {
        return;
      }

      if( m_bEscape )
      {
        for( int i = 0, len = value.Length; i < len; i++ )
        {
          Write( value[ i ] );
        }
      }
      else
      {
        m_innerWriter.Write( value );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    private void WriteImageString(string value)
    {
        if (value == null || value.Length == 0)
        {
            return;
        }

            m_innerWriter.Write(value);
        
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    private void WriteString(string value,string image,string align)
    {
        if (value == null || value.Length == 0)
        {
            return;
        }

        if (m_bEscape)
        {
            for (int i = 0, len = value.Length; i < len; i++)
            {
                if (value[i] == '&' && value[i + 1] == 'G' && image != null)
                {                    
                    WriteImageString(GetImageRTF(image));
                    i += 2;
                    if (i == len)
                        break;
                }
                Write(value[i]);
            }
        }
        else
        {
            m_innerWriter.Write(value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void WriteNewLine()
    {
      if( m_bEscape )
      {
        m_innerWriter.Write( newLine );
      }
      else
      {
        m_innerWriter.WriteLine();
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    private void WriteNewLine( string value )
    {
      if( m_bEscape )
      {
        Write(value);
        m_innerWriter.Write( newLine );
      }
      else
      {
        m_innerWriter.WriteLine( value );
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Returns string that implement current object.
    /// </summary>
    /// <returns>Returns string that implement current object.</returns>
    public override string ToString()
    {
      return m_innerWriter.ToString();
    }
    /// <summary>
    /// Writes a bool to the text stream.
    /// </summary>
    /// <param name="value">Bool param to write.</param>
    public override void Write( bool value )
    {
      OutputTabs();
      m_innerWriter.Write( value );
    }
    /// <summary>
    /// Writes a character to the text stream.
    /// </summary>
    /// <param name="value">Char value to write.</param>
    public override void Write( char value )
    {
      OutputTabs();
      WriteChar( value );
    }
    /// <summary>
    /// Writes a array of characters to the text stream.
    /// </summary>
    /// <param name="buffer">Array param to write.</param>
    public override void Write( char[] buffer )
    {
      OutputTabs();
      m_innerWriter.Write( buffer );
    }
    /// <summary>
    /// Writes a double to the text stream.
    /// </summary>
    /// <param name="value">Double param to write.</param>
    public override void Write( double value )
    {
      OutputTabs();
      m_innerWriter.Write( value );
    }
    /// <summary>
    /// Writes a int value to the text stream.
    /// </summary>
    /// <param name="value">Int value to write.</param>
    public override void Write(int value)
    {
      OutputTabs();
      m_innerWriter.Write( value );
    }
    /// <summary>
    /// Writes a long value to the text stream.
    /// </summary>
    /// <param name="value">Long value to write.</param>
    public override void Write( long value )
    {
      OutputTabs();
      m_innerWriter.Write( value );
    }
    /// <summary>
    /// Writes a object to the text stream.
    /// </summary>
    /// <param name="value">Object value to write.</param>
    public override void Write( object value )
    {
      OutputTabs();
      m_innerWriter.Write( value );
    }
    /// <summary>
    /// Writes a float value to the text stream.
    /// </summary>
    /// <param name="value">Float value to write.</param>
    public override void Write( float value )
    {
      OutputTabs();
      m_innerWriter.Write( value );
    }
    /// <summary>
    /// Writes a string to the text stream.
    /// </summary>
    /// <param name="s">String to write.</param>
    public override void Write(string s)
    {
      OutputTabs();
      WriteString( s );
    }
    internal void Write(string value, string image, string align)
    {
        OutputTabs();
        WriteString(value, image, align);
    }
    /// <summary>
    /// Writes a uint value to the text stream.
    /// </summary>
    /// <param name="value">Uint value to write.</param>
    [CLSCompliant(false)]
    public override void Write( uint value )
    {
      OutputTabs();
      m_innerWriter.Write( value );
    }
#if !(WINRT )
    /// <summary>
    /// Writes out a formatted string, using the same semantics as String.Format.
    /// </summary>
    /// <param name="format">The formatting string.</param>
    /// <param name="arg0">An object to write into the formatted string.</param>
    public override void Write( string format, object arg0 )
    {
      OutputTabs();
      m_innerWriter.Write( format, arg0 );
    }
    /// <summary>
    /// Writes out a formatted string, using the same semantics as String.Format.
    /// </summary>
    /// <param name="format">The formatting string.</param>
    /// <param name="arg">The object array to write into the formatted string.</param>
    public override void Write( string format, params object[] arg )
    {
      OutputTabs();
      m_innerWriter.Write( format, arg );
    }
    /// <summary>
    /// Writes out a formatted string, using the same semantics as String.Format.
    /// </summary>
    /// <param name="format">The formatting string.</param>
    /// <param name="arg0">An object to write into the formatted string.</param>
    /// <param name="arg1">An object to write into the formatted string.</param>
    public override void Write( string format, object arg0, object arg1 )
    {
      OutputTabs();
      m_innerWriter.Write( format, arg0, arg1 );
    }
#endif
    /// <summary>
    /// Writes a subarray of characters to the text stream.
    /// </summary>
    /// <param name="buffer">The character array to write data from.</param>
    /// <param name="index">Starting index in the buffer.</param>
    /// <param name="count">The number of characters to write. </param>
    public override void Write( char[] buffer, int index, int count )
    {
      OutputTabs();
      m_innerWriter.Write( buffer, index, count );
    }
    /// <summary>
    /// Writes a line terminator to the text stream.
    /// </summary>
    public override void WriteLine()
    {
      OutputTabs();
      WriteNewLine();
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes the text representation of a Boolean followed by a line 
    /// terminator to the text stream.
    /// </summary>
    /// <param name="value">The Boolean to write.</param>
    public override void WriteLine( bool value )
    {
      OutputTabs();
      m_innerWriter.WriteLine( value );
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes a character followed by a line terminator to the text stream.
    /// </summary>
    /// <param name="value">The character to write to the text stream.</param>
    public override void WriteLine( char value )
    {
      OutputTabs();
      m_innerWriter.WriteLine( value );
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes an array of characters followed by a line terminator to the text stream.
    /// </summary>
    /// <param name="buffer">The character array from which data is read.</param>
    public override void WriteLine( char[] buffer )
    {
      OutputTabs();
      m_innerWriter.WriteLine( buffer );
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes the text representation of a 8-byte floating-point value followed 
    /// by a line terminator to the text stream.
    /// </summary>
    /// <param name="value">The 8-byte floating-point value to write.</param>
    public override void WriteLine( double value )
    {
      OutputTabs();
      m_innerWriter.WriteLine( value );
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes the text representation of a 4-byte signed integer 
    /// followed by a line terminator to the text stream.
    /// </summary>
    /// <param name="value">The 4-byte signed integer to write.</param>
    public override void WriteLine( int value )
    {
      OutputTabs();
      m_innerWriter.WriteLine( value );
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes the text representation of an 8-byte signed 
    /// integer followed by a line terminator to the text stream.
    /// </summary>
    /// <param name="value">The 8-byte signed integer to write.</param>
    public override void WriteLine( long value )
    {
      OutputTabs();
      m_innerWriter.WriteLine( value );
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes the text representation of an object by calling 
    /// ToString on this object, followed by a line terminator to the text stream.
    /// </summary>
    /// <param name="value">The object to write.</param>
    public override void WriteLine( object value )
    {
      OutputTabs();
      m_innerWriter.WriteLine( value );
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes the text representation of a 4-byte floating-point 
    /// value followed by a line terminator to the text stream.
    /// </summary>
    /// <param name="value">The 4-byte floating-point value to write.</param>
    public override void WriteLine( float value )
    {
      OutputTabs();
      m_innerWriter.WriteLine( value );
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes a string followed by a line terminator to the text stream.
    /// </summary>
    /// <param name="s">The string to write.</param>
    public override void WriteLine( string s )
    {
      OutputTabs();
      WriteNewLine( s );
      m_bTabsPending = true;
    }
    /// <summary>
    ///Writes the text representation of a 4-byte unsigned integer followed 
    /// by a line terminator to the text stream. 
    /// </summary>
    /// <param name="value">The 4-byte unsigned integer to write.</param>
    [CLSCompliant(false)]
    public override void WriteLine( uint value )
    {
      OutputTabs();
      m_innerWriter.WriteLine( value );
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes out a formatted string and a new line, using the same semantics as Format.
    /// </summary>
    /// <param name="format">The formatting string.</param>
    /// <param name="arg">The object array to write into format string.</param>
    public override void WriteLine( string format, params object[] arg )
    {
      OutputTabs();
      m_innerWriter.WriteLine( format, arg );
      m_bTabsPending = true;
    }
#if !(WINRT )
    /// <summary>
    /// Writes out a formatted string and a new line, using the same semantics as Format.
    /// </summary>
    /// <param name="format">The formatted string.</param>
    /// <param name="arg0">The object to write into the formatted string.</param>
    public override void WriteLine( string format, object arg0 )
    {
      OutputTabs();
      m_innerWriter.WriteLine( format, arg0 );
      m_bTabsPending = true;
    }
    /// <summary>
    /// Writes out a formatted string and a new line, using the same semantics as Format.
    /// </summary>
    /// <param name="format">The formatting string.</param>
    /// <param name="arg0">The object to write into the format string.</param>
    /// <param name="arg1">The object to write into the format string.</param>
    public override void WriteLine( string format, object arg0, object arg1 )
    {
      OutputTabs();
      m_innerWriter.WriteLine( format, arg0, arg1 );
      m_bTabsPending = true;
    }
#endif
    /// <summary>
    /// Writes a subarray of characters followed by a line terminator to the text stream.
    /// </summary>
    /// <param name="buffer">The character array from which data is read.</param>
    /// <param name="index">The index into buffer at which to begin reading.</param>
    /// <param name="count">The maximum number of characters to write.</param>
    public override void WriteLine( char[] buffer, int index, int count )
    {
      OutputTabs();
      m_innerWriter.WriteLine( buffer, index, count );
      m_bTabsPending = true;
    }

    #endregion

    #region Class Public Methods
    /// <summary>
    /// Adds new font to the collection.
    /// </summary>
    /// <param name="font">Font to add.</param>
    /// <returns>Index of the font.</returns>
    public int AddFont( Font font )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( m_hashFonts.ContainsKey( font ) )
      {
        return m_hashFonts[ font ];
      }
      else
      {
        int iCount = m_hashFonts.Count + 1;
        m_hashFonts.Add( font, iCount );
        return iCount;
      }
    }
    /// <summary>
    /// Adds color to the colors table.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public int AddColor( Color color )
    {
      if( !m_hashColorTable.ContainsKey( color ) )
      {
        m_hashColorTable.Add( color, m_hashColorTable.Count + 1 );
        m_arrColors.Add( color );
      }

      return m_hashColorTable[ color ];
    }
    /// <summary>
    /// Writes fonts table into inner text writer.
    /// </summary>
    public void WriteFontTable()
    {
      if( m_hashFonts.Count == 0 )
        return;

      WriteTag( RtfTags.FontTableBegin );

      foreach( Font font in m_hashFonts.Keys )
      {
        WriteFontInTable( font );
      }

      WriteTag( RtfTags.FontTableEnd );
    }
    /// <summary>
    /// Writes colors table into inner text writer.
    /// </summary>
    public void WriteColorTable()
    {
      if( m_hashColorTable.Count == 0 )
      {
        return;
      }

      WriteTag( RtfTags.ColorTableStart );

      for( int i = 0, len = m_arrColors.Count; i < len; i++ )
      {
        Color color = m_arrColors[ i ];
        WriteColorInTable( color );
      }

      WriteTag( RtfTags.ColorTableEnd );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    /// <param name="strText"></param>
    public void WriteText( Font font, string strText )
    {
      WriteText( font, ColorExtension.Empty, ColorExtension.Empty, strText );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    /// <param name="foreColor"></param>
    /// <param name="strText"></param>
    public void WriteText( Font font, Color foreColor, string strText )
    {
      WriteText( font, foreColor, ColorExtension.Empty, strText );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    /// <param name="foreColor"></param>
    /// <param name="backColor"></param>
    /// <param name="strText"></param>
    public void WriteText( Font font, Color foreColor, Color backColor, string strText )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( strText == null || strText.Length == 0 )
      {
        return;
      }

      WriteTag( RtfTags.GroupStart );
      WriteFont( font );

      if( foreColor != ColorExtension.Empty )
      {
        WriteForeColorAttribute( foreColor );
      }

      if( backColor != ColorExtension.Empty )
      {
        WriteBackColorAttribute( backColor );
      }


      int iPos = 0;
      int iLen = strText.Length;
      bool bFound = true;

      while( iPos < iLen )
      {
        int iEndPos = strText.IndexOf( NewLine, iPos );

        if( iEndPos == -1 )
        {
          iEndPos = strText.Length;
          bFound = false;
        }

        string strToWrite = strText.Substring( iPos, iEndPos - iPos );
        Write( strToWrite );

        if( bFound )
        {
          WriteTag( RtfTags.EndLine );
        }

        iPos = iEndPos + NewLine.Length;
      }

      WriteTag( RtfTags.GroupEnd );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    /// <param name="strText"></param>
    public void WriteText( IFont font, string strText )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( strText == null || strText.Length == 0 )
      {
        return;
      }

      WriteTag( RtfTags.GroupStart );
      WriteFont( font );

      int iPos = 0;
      int iLen = strText.Length;
      bool bFound = true;

      while( iPos < iLen )
      {
        int iEndPos = strText.IndexOf( NewLine, iPos );

        if( iEndPos == -1 )
        {
          iEndPos = strText.Length;
          bFound = false;
        }

        string strToWrite = strText.Substring( iPos, iEndPos - iPos );
        Write( strToWrite );

        if( bFound )
        {
          WriteTag( RtfTags.EndLine );
        }

        iPos = iEndPos + NewLine.Length;
      }

      WriteTag( RtfTags.GroupEnd );
    }
    /// <summary>
    /// Writes the image text.
    /// </summary>
    /// <param name="font">The font.</param>
    /// <param name="strText">The STR text.</param>
    /// <param name="image">The image.</param>
    /// <param name="align">The align.</param>
    internal void WriteImageText(IFont font, string strText,string image,string align)
    {
        if (font == null)
            throw new ArgumentNullException("font");

        if (strText == null || strText.Length == 0)
        {
            return;
        }

        WriteTag(RtfTags.GroupStart);
        WriteFont(font);

        int iPos = 0;
        int iLen = strText.Length;
        bool bFound = true;

        while (iPos < iLen)
        {
            int iEndPos = strText.IndexOf(NewLine, iPos);

            if (iEndPos == -1)
            {
                iEndPos = strText.Length;
                bFound = false;
            }

            string strToWrite = strText.Substring(iPos, iEndPos - iPos);
            Write(strToWrite, image, align);

            if (bFound)
            {
                WriteTag(RtfTags.EndLine);
            }

            iPos = iEndPos + NewLine.Length;
        }

        WriteTag(RtfTags.GroupEnd);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    public void WriteFontAttribute( Font font )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( !m_hashFonts.ContainsKey( font ) )
        throw new ArgumentException( "Unknown font" );

      int iFontId = m_hashFonts[ font ];
      int iFontSize = ( int )font.Size;
      WriteFontAttribute( iFontId, iFontSize );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    public void WriteFont( Font font )
    {
      WriteFontAttribute( font );

      WriteFontItalicBoldStriked( font );

      if( font.Underline )
      {
        WriteUnderlineAttribute();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    public void WriteFont( IFont font )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      FontImpl fontImpl;

      if( font is FontImpl )
      {
        fontImpl = ( FontImpl )font;
      }
      else if( font is FontWrapper )
      {
        fontImpl = ( ( FontWrapper )font ).Wrapped;
      }
      else
      {
        throw new InvalidCastException( "Wrong type of font" );
      }

      Font nativeFont = font.GenerateNativeFont();

      WriteFontAttribute( nativeFont );
      WriteFontItalicBoldStriked( nativeFont );
      WriteUnderline( fontImpl );
      WriteSubSuperScript( fontImpl );
      WriteForeColorAttribute( font.RGBColor );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    public void WriteSubSuperScript( FontImpl font )
    {
      if( font == null )
      {
        throw new ArgumentNullException( "font" );
      }

      if( font.Subscript )
      {
        WriteTag( RtfTags.SubScript );
      }
      else if( font.Superscript )
      {
        WriteTag( RtfTags.SuperScript );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    public void WriteFontItalicBoldStriked( Font font )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( font.Italic )
      {
        WriteTag( RtfTags.ItalicOn );
      }

      if( font.Bold )
      {
        WriteTag( RtfTags.BoldOn );
      }

      if( font.Strikeout )
      {
        WriteStrikeThrough( StrikeThroughStyle.SingleOn );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    public void WriteUnderline( FontImpl font )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      switch( font.Underline )
      {
        case ExcelUnderline.None:
          return;

        case ExcelUnderline.Double:
        case ExcelUnderline.DoubleAccounting:
          WriteUnderlineAttribute( UnderlineStyle.Double );
          break;

        case ExcelUnderline.Single:
        case ExcelUnderline.SingleAccounting:
          WriteUnderlineAttribute( UnderlineStyle.Continuous );
          break;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public void WriteUnderlineAttribute()
    {
      if( m_bEnableFormatting )
      {
        WriteUnderlineAttribute( UnderlineStyle.Continuous );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="style"></param>
    public void WriteUnderlineAttribute( UnderlineStyle style )
    {
      int iStyle = ( int )style;

      if( iStyle < 0 || iStyle >= UnderlineTags.Length )
        throw new ArgumentOutOfRangeException( "style" );

      Escape = false;
      Write( UnderlineTags[ iStyle ] );
      Escape = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="style"></param>
    public void WriteStrikeThrough( StrikeThroughStyle style )
    {
      int iStyle = ( int )style;

      if( iStyle < 0 || iStyle >= StrikeThroughTags.Length )
        throw new ArgumentOutOfRangeException( "style" );

      Escape = false;
      Write( StrikeThroughTags[ iStyle ] );
      Escape = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public void WriteBackColorAttribute( Color color )
    {
      if( !m_hashColorTable.ContainsKey( color ) )
        throw new ArgumentOutOfRangeException( "color", "Unknown color" );

      int id = m_hashColorTable[ color ];

      WriteTag( RtfTags.BackColor,id );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public void WriteForeColorAttribute( Color color )
    {
      if( !m_hashColorTable.ContainsKey( color ) )
        throw new ArgumentOutOfRangeException( "color", "Unknown color" );

      int id = m_hashColorTable[ color ];

      WriteTag( RtfTags.ForeColor, id );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    public void WriteLineNoTabs(string s)
    {
      m_innerWriter.WriteLine(s);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    public void WriteTag( RtfTags tag )
    {
      if( m_bEnableFormatting )
      {
        int index = ( int )tag;

        if( index < 0 || index >= DEF_TAGS.Length )
          throw new ArgumentOutOfRangeException( "tag" );

        Escape = false;
        m_innerWriter.Write( DEF_TAGS[ index ] );
        Escape = true;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="arrParams"></param>
    public void WriteTag( RtfTags tag, params object[] arrParams )
    {
      if( m_bEnableFormatting )
      {
        int index = ( int )tag;

        if( index < 0 || index >= DEF_TAGS.Length )
          throw new ArgumentOutOfRangeException( "tag" );

        Escape = false;
        m_innerWriter.Write( string.Format( DEF_TAGS[ index ], arrParams ) );
        Escape = true;
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public bool Escape
    {
      get
      {
        return m_bEscape;
      }
      set
      {
        m_bEscape = value;
      }
    }
    // Properties
    /// <summary>
    /// Returns current Encoding.
    /// </summary>
    public override System.Text.Encoding Encoding
    {
      get
      {
        return m_innerWriter.Encoding;
      }
    }

    #endregion

    /// <summary>
    /// Writes the alignment.
    /// </summary>
    /// <param name="alignment">The alignment.</param>
    internal void WriteAlignment(string alignment)
    {
        string align = "ql";
        switch (alignment)
        {
            case "Center":
                align = "qc";
                break;
            case "Left":
                align="ql";
                break;
            case "Right":
                align="qr";
                break;
        }
        Escape = false;
        m_innerWriter.Write(string.Format("\\pard\\{0}",align));
        Escape=true;
    }
  }
  /// <summary>
  /// \ul Continuous underline. \ul0 turns off all underlining. 
  /// \ulcN Underline color 
  /// \uld Dotted underline. 
  /// \uldash Dash underline. 
  /// \uldashd Dash dot underline. 
  /// \uldashdd Dash dot dot underline. 
  /// \uldb Double underline. 
  /// \ulhwave Heavy wave underline 
  /// \ulldash  Long dash underline 
  /// \ulnone Stops all underlining. 
  /// \ulth Thick underline 
  /// \ulthd Thick dotted underline 
  /// \ulthdash  Thick dash underline 
  /// \ulthdashd  Thick dash dot underline 
  /// \ulthdashdd  Thick dash dot dot underline
  /// \ulthldash  Thick long dash underline
  /// \ululdbwave Double wave underline
  /// \ulw Word underline.
  /// \ulwave Wave underline.
  /// 
  /// Elements order is very important.
  /// </summary>
  public enum UnderlineStyle
  {
    /// <summary>
    /// Continuous underline.
    /// </summary>
    Continuous,
    /// <summary>
    /// Turns off all underlining.
    /// </summary>
    TurnOff,
    /// <summary>
    /// Dotted underline.
    /// </summary>
    Dotted,
    /// <summary>
    /// Dash underline.
    /// </summary>
    Dash,
    /// <summary>
    /// Dash dot underline. 
    /// </summary>
    DashDot,
    /// <summary>
    /// Dash dot dot underline.
    /// </summary>
    DashDotDot,
    /// <summary>
    /// Double underline.
    /// </summary>
    Double,
    /// <summary>
    /// Heavy wave underline.
    /// </summary>
    HeavyWave,
    /// <summary>
    /// Long dash underline.
    /// </summary>
    LongDash,
    /// <summary>
    /// Stops all underlining.
    /// </summary>
    None,
    /// <summary>
    /// Thick underline.
    /// </summary>
    Thick,
    /// <summary>
    /// Thick dotted underline.
    /// </summary>
    ThickDotted,
    /// <summary>
    /// Thick dash underline.
    /// </summary>
    ThickDash,
    /// <summary>
    /// Thick dash dot underline.
    /// </summary>
    ThickDashDot,
    /// <summary>
    /// Thick dash dot dot underline.
    /// </summary>
    ThickDashDotDot,
    /// <summary>
    /// Thick long dash underline.
    /// </summary>
    ThickLongDash,
    /// <summary>
    /// Double wave underline.
    /// </summary>
    DoubleWave,
    /// <summary>
    /// Word underline.
    /// </summary>
    Word,
    /// <summary>
    /// Wave underline.
    /// </summary>
    Wave,
  }
  /// <summary>
  /// Elements order is very important.
  /// </summary>
  public enum StrikeThroughStyle
  {
    /// <summary>
    /// Single is on.
    /// </summary>
    SingleOn,
    /// <summary>
    /// Single is off.
    /// </summary>
    SingleOff,
    /// <summary>
    /// Double is on.
    /// </summary>
    DoubleOn,
    /// <summary>
    /// Double is off.
    /// </summary>
    DoubleOff,
  }

  /// <summary>
  /// Elements order is very important.
  /// </summary>
  public enum RtfTags
  {
    /// <summary>
    /// Font table begins.
    /// </summary>
    FontTableBegin,
    /// <summary>
    /// Font table ends.
    /// </summary>
    FontTableEnd,
    /// <summary>
    /// Color table starts.
    /// </summary>
    ColorTableStart,
    /// <summary>
    /// Color table ends.
    /// </summary>
    ColorTableEnd,
    /// <summary>
    /// Bold on.
    /// </summary>
    BoldOn,
    /// <summary>
    /// Bold off.
    /// </summary>
    BoldOff,
    /// <summary>
    /// Italic on.
    /// </summary>
    ItalicOn,
    /// <summary>
    /// Italic off.
    /// </summary>
    ItalicOff,
    /// <summary>
    /// Rtf begins.
    /// </summary>
    RtfBegin,
    /// <summary>
    /// Rtf ends.
    /// </summary>
    RtfEnd,
    /// <summary>
    /// Group starts.
    /// </summary>
    GroupStart,
    /// <summary>
    /// Group ends.
    /// </summary>
    GroupEnd,
    /// <summary>
    /// End of line.
    /// </summary>
    EndLine,
    /// <summary>
    /// Foreground color.
    /// </summary>
    ForeColor,
    /// <summary>
    /// Background color.
    /// </summary>
    BackColor,
    /// <summary>
    /// Subscript.
    /// </summary>
    SubScript,
    /// <summary>
    /// SuperScribt.
    /// </summary>
    SuperScript,
  }
}
