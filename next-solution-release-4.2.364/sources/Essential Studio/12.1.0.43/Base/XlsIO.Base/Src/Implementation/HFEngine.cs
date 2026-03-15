#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT && !WINRT && !WP
#region file using directives
using System;
using System.Text;
using System.Collections;

using Syncfusion.XlsIO.Parser.Biff_Records;
//using Syncfusion.XlsIO.IO.Stream.Win32;
using Syncfusion.XlsIO.Interfaces;
#if ( WINRT )
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WINRT;
#elif  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#else
using System.Drawing;
#endif

using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Engine for parsing and creation of header/footer strings.
  /// </summary>
  public class HFEngine
    : RichTextString
    , IHFEngine
  {
    #region Class constants
    /// <summary>
    /// Ampersand character.
    /// </summary>
    private const string DEF_AMP = "&";
    /// <summary>
    /// Character that indicates start or end of the underline block.
    /// </summary>
    private const char DEF_UNDERLINE = 'U';
    /// <summary>
    /// Character that indicates start or end of the double underline block.
    /// </summary>
    private const char DEF_DOUBLE_UNDERLINE = 'E';
    /// <summary>
    /// Character that indicates start or end of the underline block.
    /// </summary>
    private const char DEF_STRIKEOUT = 'S';
    /// <summary>
    /// Character that indicates start or end of the underline block.
    /// </summary>
    private const char DEF_SUBSCRIPT = 'Y';
    /// <summary>
    /// Character that indicates start or end of the underline block.
    /// </summary>
    private const char DEF_SUPERSCRIPT = 'X';
    /// <summary>
    /// Character that indicates start or end of the underline block.
    /// </summary>
    private const char DEF_FONT_NAME_EDGE = '"';
    /// <summary>
    /// Separator between font style and face name.
    /// </summary>
    private const char DEF_FONT_STYLE_SEPARATOR = ',';
    /// <summary>
    /// Value of Bold font style.
    /// </summary>
    private const string DEF_BOLD_VALUE = "bold";
    /// <summary>
    /// Value of Italic font style.
    /// </summary>
    private const string DEF_ITALIC_VALUE = "italic";
    /// <summary>
    /// Regular font style.
    /// </summary>
    private const string DEF_REGULAR_STYLE = "regular";
    /// <summary>
    /// Space character.
    /// </summary>
    private const char DEF_SPACE = ' ';
    #endregion

    #region Internal data types
    /// <summary>
    /// Contains data used for font locating.
    /// </summary>
    private class FindFontData
    {
      /// <summary>
      /// Font style to locate.
      /// </summary>
      public string strFontStyle;
      /// <summary>
      /// Located font data.
      /// </summary>
      public ENUMLOGFONTEX fontData;
    }
    #endregion

    #region Class members
    /// <summary>
    /// List with all used fonts.
    /// </summary>
    private List<FontImpl> m_arrFonts = new List<FontImpl>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the RichTextString.
    /// </summary>
    /// <param name="application">Application object for the RichTextString.</param>
    /// <param name="parent">Parent object for the RichTextString.</param>
    public HFEngine( IApplication application, object parent )
      : base( application, parent )
    {
      m_text = new TextWithFormat();
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Parses text of header/footer part.
    /// </summary>
    /// <param name="strText">Text of header/footer part.</param>
    public void Parse( string strText )
    {
      if( strText == null || strText.Length == 0 )
      {
        Clear();
        return;
      }

      int iPos = 0;
      int iLength = strText.Length;
      m_text = new TextWithFormat();
      m_arrFonts.Clear();
      m_arrFonts.Add( DefaultFont );
      FontImpl font = DefaultFont;
      StringBuilder builder = new StringBuilder( iLength );

      while( iPos < iLength )
      {
        int iAmpPos = strText.IndexOf( DEF_AMP, iPos );

        if( iAmpPos >= 0 )
        {
          string strPart = strText.Substring( iPos, iAmpPos - iPos );
          builder.Append( strPart );
          iPos = iAmpPos + 1;
          ProcessCharacter( strText, builder, ref iPos, ref font );
        }
        else
        {
          string strPart = strText.Substring( iPos );
          builder.Append( strPart );
          AddTextBlock( builder, font );
          iPos = iLength;
        }
      }
    }
    /// <summary>
    /// Returns string in format that is supported by Excel header/footer.
    /// </summary>
    /// <returns>String in format that is supported by Excel header/footer.</returns>
    public string GetHeaderFooterString()
    {
      StringBuilder builder = new StringBuilder();

      SortedList<int, int> formattingRuns = m_text.FormattingRuns;
      IList<int> lstKeys = formattingRuns.Keys;
      IList<int> lstValues = formattingRuns.Values;

      FontImpl prevFont = null;
      int iPrevPos = 0;
      int iCurPos = -1;

      for( int i = 0, len = m_text.FormattingRunsCount; i < len; i++ )
      {
        int iFontIndex = lstValues[ i ];
        iCurPos = lstKeys[ i ];

        FontImpl curFont = m_arrFonts[ iFontIndex ];
        WritePrevTextBlock( builder, iPrevPos, iCurPos );
        WriteFontDifference( builder, prevFont, curFont );

        iPrevPos = iCurPos;
        prevFont = curFont;
      }

      WritePrevTextBlock( builder, iPrevPos, m_text.Text.Length );

      return builder.ToString();
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Processes next character after ampersand.
    /// </summary>
    /// <param name="strText">String to parse.</param>
    /// <param name="builder">Builder with current text block.</param>
    /// <param name="iPos">Character position.</param>
    /// <param name="font">Font of the current text block.</param>
    private void ProcessCharacter( string strText, StringBuilder builder,
      ref int iPos, ref FontImpl font )
    {
      if( strText == null )
        throw new ArgumentNullException( "strText" );

      int iLength = strText.Length;
      char chNext = strText[ iPos ];

      switch( chNext )
      {
        case DEF_UNDERLINE:
        {
          AddTextBlock( builder, font );
          font = font.TypedClone();
          ExcelUnderline curUnderline = font.Underline;

          font.Underline = ( curUnderline == ExcelUnderline.SingleAccounting )
            ? ExcelUnderline.None
            : ExcelUnderline.SingleAccounting;
          iPos++;
          break;
        }

        case DEF_DOUBLE_UNDERLINE:
        {
          AddTextBlock( builder, font );
          font = font.TypedClone();
          ExcelUnderline curUnderline = font.Underline;

          font.Underline = ( curUnderline == ExcelUnderline.DoubleAccounting )
            ? ExcelUnderline.None
            : ExcelUnderline.DoubleAccounting;
          iPos++;
          break;
        }

        case DEF_STRIKEOUT:
          AddTextBlock( builder, font );
          font = font.TypedClone();
          font.Strikethrough = !font.Strikethrough;
          iPos++;
          break;

        case DEF_SUBSCRIPT:
          AddTextBlock( builder, font );
          font = font.TypedClone();
          font.Subscript = !font.Subscript;
          iPos++;
          break;

        case DEF_SUPERSCRIPT:
          AddTextBlock( builder, font );
          font = font.TypedClone();
          font.Superscript = !font.Superscript;
          iPos++;
          break;

        case DEF_FONT_NAME_EDGE:
          int iStartPos = iPos + 1;
          int iStringEnd = strText.IndexOf( DEF_FONT_NAME_EDGE, iStartPos );

          if( iStringEnd != -1 )
          {
            string strFontName = strText.Substring( iStartPos, iStringEnd - iStartPos );
            AddTextBlock( builder, font );
            font = font.TypedClone();
            SetFont( font, strFontName );
            iPos = iStringEnd + 1;
          }
          else
          {
            goto default;
          }

          break;

        default:
          if( char.IsDigit( chNext ) )
          {
            // It is font size.
            int iFontSize = 0;

            while( char.IsDigit( chNext ) )
            {
              iFontSize = 10 * iFontSize + ( int )chNext - ( int )'0';
              iPos++;
                  
              if( iPos < iLength )
              {
                chNext = strText[ iPos ];
              }
              else
              {
                break;
              }
            }

            AddTextBlock( builder, font );
            font = font.TypedClone();
            font.Size = iFontSize;
          }
          else
          {
            // Simply character to add to the text.
            builder.Append( DEF_AMP );
            builder.Append( chNext );
            iPos++;
          }
          break;
      }
    }
    /// <summary>
    /// Adds text block to the rtf string.
    /// </summary>
    /// <param name="builder">Text to add.</param>
    /// <param name="font">Font of the text block.</param>
    private void AddTextBlock( StringBuilder builder, FontImpl font )
    {
      int iOldLength = m_text.Text.Length;
      m_text.Text += builder.ToString();

      if( builder.Length > 0 )
      {
        int iFontIndex = AddFont( font );
        m_text.SetTextFontIndex( iOldLength, iOldLength + builder.Length - 1, iFontIndex );
        builder.Length = 0;
      }
    }
    /// <summary>
    /// Adds font to the inner fonts array.
    /// </summary>
    /// <param name="fontToAdd">Font to add.</param>
    /// <returns>Index of the added font.</returns>
    protected override int AddFont( IFont fontToAdd )
    {
      if( fontToAdd == null )
        throw new ArgumentNullException( "fontToAdd" );

      // Here we have to check whether same font has been already added to the collection.
      // If not simply add to the List.

      fontToAdd = ( ( IInternalFont )fontToAdd ).Font;

      for( int i = 0, len = m_arrFonts.Count; i < len; i++ )
      {
        FontImpl font = m_arrFonts[ i ];
        if( font == fontToAdd ) return i;
      }

      m_arrFonts.Add( ( FontImpl )fontToAdd );
      return m_arrFonts.Count - 1;
    }
    /// <summary>
    /// Updates font settings.
    /// </summary>
    /// <param name="font">Font to update.</param>
    /// <param name="strFontName">String representation of the font to set.</param>
    private void SetFont( FontImpl font, string strFontName )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( strFontName == null || strFontName.Length == 0 ) return;

      //string[] arrParts = strFontName.Split( DEF_FONT_STYLE_SEPARATOR );

      int iPos = strFontName.IndexOf( DEF_FONT_STYLE_SEPARATOR );
      string strStyle = null;

      if( iPos >= 0 )
      {
        strStyle = strFontName.Substring( iPos + 1 );
        strFontName = strFontName.Substring( 0, iPos );
      }

      ENUMLOGFONTEX fontData = FindFont( strFontName, strStyle );

      font.FontName = strFontName;

      if( fontData != null )
      {
        LOGFONT logFont = fontData.LogFont;
        Font nativeFont = Font.FromLogFont( logFont );
        CopyFontSettings( nativeFont, font );
      }
      else
      {
        // Here we should try to use default style names.
        strStyle = strStyle.ToLower();
        font.Bold = ( strStyle.IndexOf( DEF_BOLD_VALUE ) >= 0 );
        font.Italic = ( strStyle.IndexOf( DEF_ITALIC_VALUE ) >= 0 );
      }
    }
    /// <summary>
    /// Searches for required font.
    /// </summary>
    /// <param name="strFontName">Font name to look for.</param>
    /// <param name="strFontStyle">Font style.</param>
    public ENUMLOGFONTEX FindFont( string strFontName, string strFontStyle )
    {
#if !SILVERLIGHT && !WINRT && !WP
      LOGFONT logFont = new LOGFONT();

      int iLength = strFontName.Length;

      if( strFontName[ iLength - 1 ] != '\0' ) strFontName += '\0';

      Encoding.ASCII.GetBytes( strFontName ).CopyTo( logFont.lfFaceName, 0 );
      logFont.lfCharSet = 1;

      Graphics graphics = m_book.InnerGraphics;
      IntPtr hdc = graphics.GetHdc();
      FindFontData ffData = new FindFontData();
      ffData.strFontStyle = strFontStyle;
      object objData = ffData;
 
      API.EnumFontFamiliesEx( hdc, logFont, new EnumFontFamExProc( FontEnumProc ),
        ref objData, 0 );

      graphics.ReleaseHdc( hdc );

      return ffData.fontData;
#else
      throw new NotImplementedException();
#endif
    }

    /// <summary>
    /// This function is used for font enumeration. It is called by EnumFontFamiliesEx function.
    /// </summary>
    /// <param name="lpelf">Logical-font data.</param>
    /// <param name="lpntm">Physical-font data.</param>
    /// <param name="FontType">Type of font.</param>
    /// <param name="objData">application-defined data.</param>
    /// <returns>0 when required font was found.</returns>
    private static int FontEnumProc( ENUMLOGFONTEX lpelf, IntPtr lpntm,
      int FontType, ref object objData )
    {
      FindFontData ffData = objData as FindFontData;

      if( ffData == null ) return 0;

      if( String.Compare( lpelf.Style, ffData.strFontStyle, StringComparison.CurrentCultureIgnoreCase ) == 0 )
      {
        ffData.fontData = lpelf;
        return 0;
      }

      return 1;
    }
    /// <summary>
    /// Copies font settings from native .Net font into XlsIO font.
    /// </summary>
    /// <param name="sourceFont">Source font.</param>
    /// <param name="destFont">Destination font.</param>
    private static void CopyFontSettings( Font sourceFont, FontImpl destFont )
    {
      if( sourceFont == null )
        throw new ArgumentNullException( "sourceFont" );

      if( destFont == null )
        throw new ArgumentNullException( "destFont" );

      destFont.Bold = sourceFont.Bold;
      destFont.Italic = sourceFont.Italic;
    }
    /// <summary>
    /// Writes text block.
    /// </summary>
    /// <param name="builder">String builder to write into.</param>
    /// <param name="iPrevPos">Start position (included).</param>
    /// <param name="iCurPos">End position (not included).</param>
    private void WritePrevTextBlock( StringBuilder builder, int iPrevPos, int iCurPos )
    {
      if( builder == null )
        throw new ArgumentNullException( "builder" );

      if( iPrevPos >= iCurPos ) return;

      builder.Append( m_text.Text, iPrevPos, iCurPos - iPrevPos );
    }
    /// <summary>
    /// Writes font difference into StringBuilder in header/footer format.
    /// </summary>
    /// <param name="builder">Builder to write difference into.</param>
    /// <param name="prevFont">Previous font to compare.</param>
    /// <param name="curFont">Current font to compare.</param>
    private void WriteFontDifference( StringBuilder builder, FontImpl prevFont, FontImpl curFont )
    {
      if( builder == null )
        throw new ArgumentNullException( "builder" );

      WriteFontName( builder, prevFont, curFont );
      WriteFontSize( builder, prevFont, curFont );
      WriteFontUnderline( builder, prevFont, curFont );
      WriteFontSupSub( builder, prevFont, curFont );
      WriteFontStrikeout( builder, prevFont, curFont );
    }
    /// <summary>
    /// Writes font name if necessary.
    /// </summary>
    /// <param name="builder">Builder to write font name settings into.</param>
    /// <param name="prevFont">Previous font to compare.</param>
    /// <param name="curFont">Font to get settings from.</param>
    private void WriteFontName( StringBuilder builder, FontImpl prevFont, FontImpl curFont )
    {
      if( builder == null )
        throw new ArgumentNullException( "builder" );

      bool bNoPrevFont = prevFont == null;
      bool bNoCurFont = curFont == null;

      if( bNoPrevFont && bNoCurFont ) return;

      if( bNoCurFont )
        throw new ArgumentNullException( "curFont" );

      if( !bNoPrevFont && prevFont.FontName == curFont.FontName
          && prevFont.Bold == curFont.Bold
          && prevFont.Italic == curFont.Italic )
      {
        return;
      }

      builder.Append( DEF_AMP );
      builder.Append( DEF_FONT_NAME_EDGE );
      builder.Append( curFont.FontName );

      if( curFont.Bold || curFont.Italic )
      {
        builder.Append( DEF_FONT_STYLE_SEPARATOR );

        if( curFont.Bold )
        {
          builder.Append( DEF_BOLD_VALUE );
        }

        if( curFont.Italic )
        {
          if( curFont.Bold ) builder.Append( DEF_SPACE );

          builder.Append( DEF_ITALIC_VALUE );
        }
      }
      else if( !bNoPrevFont && ( prevFont.Bold || prevFont.Italic ) )
      {
        builder.Append( DEF_FONT_STYLE_SEPARATOR );
        builder.Append( DEF_REGULAR_STYLE );
      }

      builder.Append( DEF_FONT_NAME_EDGE );
    }
    /// <summary>
    /// Writes font underline settings if necessary.
    /// </summary>
    /// <param name="builder">Builder to write font name settings into.</param>
    /// <param name="prevFont">Previous font to compare.</param>
    /// <param name="curFont">Font to get settings from.</param>
    private void WriteFontUnderline( StringBuilder builder, FontImpl prevFont, FontImpl curFont )
    {
      if( builder == null )
        throw new ArgumentNullException( "builder" );
 
      bool bNoPrevFont = prevFont == null;
      bool bNoCurFont = curFont == null;

      if( bNoPrevFont && bNoCurFont ) return;

      if( bNoCurFont )
        throw new ArgumentNullException( "curFont" );
 
      if( !bNoPrevFont && prevFont.Underline == curFont.Underline ) return;

      switch( curFont.Underline )
      {
        case ExcelUnderline.None:
          // We have to close previous underline tag.
          WriteFontUnderline( builder, null, prevFont );
          break;

        case ExcelUnderline.Double:
        case ExcelUnderline.DoubleAccounting:
          builder.Append( DEF_AMP );
          builder.Append( DEF_DOUBLE_UNDERLINE );
          break;

        case ExcelUnderline.Single:
        case ExcelUnderline.SingleAccounting:
          builder.Append( DEF_AMP );
          builder.Append( DEF_UNDERLINE );
          break;

        default:
          throw new ArgumentOutOfRangeException( "underline" );
      }
    }
    /// <summary>
    /// Writes font superscript/subscript settings if necessary.
    /// </summary>
    /// <param name="builder">Builder to write font name settings into.</param>
    /// <param name="prevFont">Previous font to compare.</param>
    /// <param name="curFont">Font to get settings from.</param>
    private void WriteFontSupSub( StringBuilder builder, FontImpl prevFont, FontImpl curFont )
    {
      if( builder == null )
        throw new ArgumentNullException( "builder" );
 
      bool bNoPrevFont = prevFont == null;
      bool bNoCurFont = curFont == null;

      if( bNoPrevFont && bNoCurFont ) return;

      if( bNoCurFont )
        throw new ArgumentNullException( "curFont" );
 
      if( !bNoPrevFont && prevFont.Superscript == curFont.Superscript
        && prevFont.Subscript == curFont.Subscript ) return;

      if( curFont.Superscript )
      {
        builder.Append( DEF_AMP );
        builder.Append( DEF_SUPERSCRIPT );
      }
      else if( curFont.Subscript )
      {
        builder.Append( DEF_AMP );
        builder.Append( DEF_SUBSCRIPT );
      }
      else
      {
        // Close previous super/sub -script setting.
        WriteFontSupSub( builder, null, prevFont );
      }
    }
    /// <summary>
    /// Writes font strikeout settings if necessary.
    /// </summary>
    /// <param name="builder">Builder to write font name settings into.</param>
    /// <param name="prevFont">Previous font to compare.</param>
    /// <param name="curFont">Font to get settings from.</param>
    private void WriteFontStrikeout( StringBuilder builder, FontImpl prevFont, FontImpl curFont )
    {
      if( builder == null )
        throw new ArgumentNullException( "builder" );
 
      bool bNoPrevFont = prevFont == null;
      bool bNoCurFont = curFont == null;

      if( bNoPrevFont && bNoCurFont ) return;

      if( bNoCurFont )
        throw new ArgumentNullException( "curFont" );
 
      if( !bNoPrevFont && prevFont.Strikethrough == curFont.Strikethrough
        || bNoPrevFont && !curFont.Strikethrough ) return;

      builder.Append( DEF_AMP );
      builder.Append( DEF_STRIKEOUT );
    }
    /// <summary>
    /// Writes font size settings if necessary.
    /// </summary>
    /// <param name="builder">Builder to write font name settings into.</param>
    /// <param name="prevFont">Previous font to compare.</param>
    /// <param name="curFont">Font to get settings from.</param>
    private void WriteFontSize( StringBuilder builder, FontImpl prevFont, FontImpl curFont )
    {
      if( builder == null )
        throw new ArgumentNullException( "builder" );
 
      bool bNoPrevFont = prevFont == null;
      bool bNoCurFont = curFont == null;

      if( bNoPrevFont && bNoCurFont ) return;

      if( bNoCurFont )
        throw new ArgumentNullException( "curFont" );
 
      if( !bNoPrevFont && prevFont.Size == curFont.Size ) return;

      builder.Append( DEF_AMP );
      builder.Append( curFont.Size );

      // TODO: if we want to have same behaviour as MS Excel than we should
      // check whether next character in the string is number and if it is
      // then we have to add space between next string and font size specifier.
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Returns font by its index.
    /// </summary>
    /// <param name="iFontIndex">Font index.</param>
    /// <returns>Font that corresponds to the specified index.</returns>
    protected override FontImpl GetFontByIndex( int iFontIndex )
    {
      return m_arrFonts[ iFontIndex ];
    }
    /// <summary>
    /// Clears string and formatting.
    /// </summary>
    public override void Clear()
    {
      base.Clear();
      m_arrFonts.Clear();
    }
    /// <summary>
    /// This method is called after each change in rich text string.
    /// </summary>
    public override void EndUpdate()
    {
      if( m_text.FormattingRunsCount == 0 )
      {
        int iLength = m_text.Text.Length;

        if( iLength > 0 ) SetFont( 0, iLength - 1, DefaultFont );
      }
    }

    #endregion
  }
}
#endif