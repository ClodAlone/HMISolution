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

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections.Generic;
using Syncfusion.XlsIO.Drawing;
using Syncfusion.XlsIO.Implementation.Shapes;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
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
using Syncfusion.XlsIO.Implementation.Shapes;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents rich text string in the workbook.
  /// </summary>
  public class RichTextString
    : CommonWrapper
    , IRichTextString
    , IOptimizedUpdate
  {
    #region Class members
    /// <summary>
    /// Low level text object.
    /// </summary>
    protected TextWithFormat m_text;
    /// <summary>
    /// Represents the RTF string
    /// </summary>
    private string m_rtfText;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    protected WorkbookImpl m_book;
    /// <summary>
    /// Indicates whether string is read-only.
    /// </summary>
    private bool m_bIsReadOnly;
    /// <summary>
    /// Represents the parent RTF object.
    /// </summary>
    private object m_rtfParent;
    /// <summary>
    /// All digits without zero.
    /// </summary>
    private static readonly char[] DEF_DIGITS = new char[]
    {
      '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
    };
    /// <summary>
    /// Zero character.
    /// </summary>
    private const char DEF_ZERO = 'X';
    /// <summary>
    /// 
    /// </summary>
    private object m_parent;
    /// <summary>
    /// Default font index.
    /// </summary>
    private int m_iFontIndex = 0;
    private string m_imageRTF;
    private PreservationLogger m_logger;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the RichTextString.
    /// </summary>
    /// <param name="application">Application object for the RichTextString.</param>
    /// <param name="parent">Parent object for the RichTextString.</param>
    public RichTextString( IApplication application, object parent )
      //: base() application, parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      m_parent = parent;
      SetParents();
      m_logger = new PreservationLogger();
    }
    /// <summary>
    /// Initializes new instance of the RichTextString.
    /// </summary>
    /// <param name="application">Application object for the RichTextString.</param>
    /// <param name="parent">Parent object for the RichTextString.</param>
    /// <param name="isReadOnly">Indicates whether string is read-only.</param>
    public RichTextString( IApplication application, object parent, bool isReadOnly )
      : this( application, parent, isReadOnly, false )
    {
    }
    /// <summary>
    /// Initializes new instance of the RichTextString.
    /// </summary>
    /// <param name="application">Application object for the RichTextString.</param>
    /// <param name="parent">Parent object for the RichTextString.</param>
    /// <param name="isReadOnly">Indicates whether string is read-only.</param>
    /// <param name="bCreateText">Indicates whether to create inner TextWithFormat.</param>
    public RichTextString( IApplication application, object parent, object rtfParent, bool isReadOnly, bool bCreateText )
      : this( application, parent, isReadOnly, bCreateText )
    {
        m_rtfParent = rtfParent;
    }
    /// <summary>
    /// Initializes new instance of the RichTextString.
    /// </summary>
    /// <param name="application">Application object for the RichTextString.</param>
    /// <param name="parent">Parent object for the RichTextString.</param>
    /// <param name="isReadOnly">Indicates whether string is read-only.</param>
    /// <param name="bCreateText">Indicates whether to create inner TextWithFormat.</param>
    public RichTextString(IApplication application, object parent, bool isReadOnly, bool bCreateText)
        : this(application, parent)
    {
        m_bIsReadOnly = isReadOnly;

        if (bCreateText)
        {
            m_text = new TextWithFormat();
        }
    }
    /// <summary>
    /// Initializes new instance of the RichTextString.
    /// </summary>
    /// <param name="application">Application object for the RichTextString.</param>
    /// <param name="parent">Parent object for the RichTextString.</param>
    /// <param name="isReadOnly">Indicates whether string is read-only.</param>
    /// <param name="bCreateText">>Indicates whether to create inner TextWithFormat.</param>
    /// <param name="logger">Logger information for AutoShapes</param>
    internal RichTextString(IApplication application, object parent, bool isReadOnly, bool bCreateText, PreservationLogger logger)
        : this(application, parent)
    {
        m_bIsReadOnly = isReadOnly;

        if (bCreateText)
        {
            m_text = new TextWithFormat();
        }
        m_logger = logger;
    }
    /// <summary>
    /// Initializes new instance of the RichTextString.
    /// </summary>
    /// <param name="application">Application object for the RichTextString.</param>
    /// <param name="parent">Parent object for the RichTextString.</param>
    /// <param name="text">Text to wrap.</param>
    public RichTextString( IApplication application, object parent, TextWithFormat text )
      : this( application, parent )
    {
      m_text = text;
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    protected virtual void SetParents()
    {
      m_book = CommonObject.FindParent( m_parent, typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Can't find parent workbook." );
    }
    #endregion

    #region IRichTextString Members
    /// <summary>
    /// Returns font which is applied to character at the specified position.
    /// </summary>
    /// <param name="iPosition">Character index.</param>
    /// <returns>Font which is applied to character at the specified position.</returns>
    public IFont GetFont( int iPosition )
    {
      if( iPosition < 0 || iPosition >= m_text.Text.Length )
        throw new ArgumentOutOfRangeException( "iPosition" );

      int index = m_text.GetTextFontIndex( iPosition );
      FontImpl font = GetFontByIndex( index );

      return new FontWrapper( font, true, false );
    }
    /// <summary>
    /// Returns font which is applied to character at the specified position.
    /// </summary>
    /// <param name="iPosition">Character index.</param>
    /// <returns>Font which is applied to character at the specified position.</returns>
    public IFont GetFont(int iPosition,bool isCopy)
    {
        if (iPosition < 0 || iPosition >= m_text.Text.Length)
            throw new ArgumentOutOfRangeException("iPosition");

        int index = m_text.GetTextFontIndex(iPosition, isCopy);
        FontImpl font = GetFontByIndex(index);

        return new FontWrapper(font, true, false);
    }
    /// <summary>
    /// Sets font for range of characters.
    /// </summary>
    /// <param name="iStartPos">First character of the range.</param>
    /// <param name="iEndPos">Last character of the range.</param>
    /// <param name="font">Font to set.</param>
    public void SetFont(int iStartPos, int iEndPos, IFont font)
    {
      BeginUpdate();

      int iFontIndex = AddFont( font );

      if( iStartPos == 0 )
      {
        if( m_text.FormattingRunsCount > 0 )
        {
          int iTextLen = m_text.Text.Length;
          int iDefaultFont = DefaultFontIndex;
          int iOldFontIndex = ( iTextLen < iEndPos + 1 ) ? m_text.GetTextFontIndex( iEndPos + 1 ) : -1;

          m_text.ReplaceFont( 0, iDefaultFont );

          if( iOldFontIndex >= 0 )
            m_text.FormattingRuns[ iEndPos + 1 ] = iOldFontIndex;

          m_text.SetTextFontIndex( iStartPos, iEndPos, iFontIndex );
        }
        else if( iEndPos < m_text.Text.Length - 1 )
        {
          SetFont( iEndPos + 1, m_text.Text.Length - 1, DefaultFont );
        }

        FontImpl fontImpl = ( font is FontWrapper ) ?
          ( font as FontWrapper ).Wrapped :
          font as FontImpl;

        DefaultFont = fontImpl;
        //m_text.AddDefaultReference( iDefaultFont );
        if (iFontIndex < 0)
        {
            iFontIndex = 0;
        }
        else
        {
            DefaultFontIndex = iFontIndex;
        }
          if(m_text .Text .Length >0)
        m_text.SetTextFontIndex(iStartPos, iEndPos, iFontIndex);
      }
      else
      {
        m_text.SetTextFontIndex( iStartPos, iEndPos, iFontIndex );
      }

      EndUpdate();
    }
    /// <summary>
    /// Clears string formatting.
    /// </summary>
    public void ClearFormatting()
    {
      if( m_text != null && IsFormatted )
      {
        BeginUpdate();
        m_text.ClearFormatting();
        EndUpdate();
      }
    }
    /// <summary>
    /// Gets / sets text of the string.
    /// </summary>
    public string Text
    {
      get
      {
          if (m_text == null)
              m_text = new TextWithFormat();
        return m_text.Text;
      }
      set
      {
        BeginUpdate();
		string preTxt = m_text.Text;
        m_text.Text = value;
		if (preTxt.Length < m_text.Text.Length && IsFormatted)
            m_text.SetTextFontIndex(preTxt.Length, m_text.Text.Length - 1, m_text.FormattingRuns.Values[m_text.FormattingRuns.Values.Count - 1]);
		else if (IsFormatted)
		{
			int fontIndex=m_text.FormattingRuns.Values[0];
			m_text.ClearFormatting();
			m_text.SetTextFontIndex(0, m_text.Text.Length - 1, fontIndex);
		}
        EndUpdate();
      }
    }
    /// <summary>
    /// Returns text in rtf format. Read-only.
    /// </summary>
    public string RtfText
    {
      get
      {
#if !SILVERLIGHT && !WINRT && !WP
          if (m_rtfParent != null)
              return m_rtfText;
          else
              return GenerateRtfText();
#else
        throw new NotImplementedException();
#endif
      }
      set
      {
          m_rtfText = value;

          if (m_rtfParent != null)
          {
              if (m_rtfParent is TextBoxShapeBase)
              {
                  (m_rtfParent as TextBoxShapeImpl).RichTextReader.SetRTF(m_rtfParent, m_rtfText);
              }
          }
      }
    }
    /// <summary>
    /// Indicates whether rich text string has formatting runs. Read-only.
    /// </summary>
    public bool IsFormatted
    {
      get
      {
        return m_text.FormattingRunsCount > 0 ;
      }
    }
    /// <summary>
    /// Appends rich text string with specified text and font.
    /// </summary>
    /// <param name="text">Text to append.</param>
    /// <param name="font">Font to use.</param>
    public void Append( string text, IFont font )
    {
      BeginUpdate();
      int iStartPos = m_text.Text.Length;
      m_text.Text += text;
      SetFont( iStartPos, iStartPos + text.Length - 1, font );
      EndUpdate();
    }
    public void Substring( int startIndex, int length )
    {
      string currentText = m_text.Text;

      if( startIndex > 0 )
      {
        if( startIndex >= currentText.Length )
        {
          m_text.Text = string.Empty;
          ClearFormatting();
        }
        else
        {
          m_text.RemoveAtStart( startIndex );
        }
      }

      m_text.RemoveAtEnd( m_text.Text.Length - length );
    }
    /// <summary>
    /// Appends the text.
    /// </summary>
    /// <param name="text">text to append.</param>
    internal void SetText(string text)
    {
        BeginUpdate();
        string preTxt = m_text.Text;
        m_text.Text = text;
        EndUpdate();
    }
    #endregion

    #region IParentApplication members
    /// <summary>
    /// Returns parent object. Read-only.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_parent;
      }
    }
    /// <summary>
    /// Returns parent application object. Read-only.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return m_book.Application;
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns size of the string. Read-only.
    /// </summary>
    public SizeF  StringSize
    {
      get
      {
        SizeF resultSize = new SizeF( 0, 0 );
        SizeF curSize;
        int startPos = 0;
        int endPos;

        for( int i = 0, len = m_text.FormattingRunsCount; i < len; i++ )
        {
          endPos = m_text.GetPositionByIndex( i );

          curSize = GetSizePart( startPos, endPos );
          
          resultSize.Width += curSize.Width;
          resultSize.Height = Math.Max( curSize.Height, resultSize.Height );

          startPos = endPos;
        }

        curSize = GetSizePart( startPos, Text.Length );
          
        resultSize.Width += curSize.Width;
        resultSize.Height = Math.Max( curSize.Height, resultSize.Height );

        return resultSize;
      }
    }
    /// <summary>
    /// Returns default font. Read-only.
    /// </summary>
    public virtual FontImpl DefaultFont
    {
      get
      {
        return ( FontImpl )m_book.InnerFonts[ m_iFontIndex ];
      }
      internal set
      {
        m_iFontIndex = value.Index;
      }
    }
    /// <summary>
    /// Returns text object. Read-only.
    /// </summary>
    public TextWithFormat TextObject
    {
      get
      {
        return m_text;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Gets / sets default font index.
    /// </summary>
    public int DefaultFontIndex
    {
      get
      {
        return m_iFontIndex;
      }
      set
      {
        m_iFontIndex = value;
      }
    }
    /// <summary>
    /// Gets or sets the image RTF.
    /// </summary>
    /// <value>The image RTF.</value>
    internal string ImageRTF
    {
        get
        {
            return m_imageRTF;
        }
        set
        {
            m_imageRTF = value;
        }
    }
    #endregion

    #region Class virtual methods
    /// <summary>
    /// Returns font index at the specified position.
    /// </summary>
    /// <param name="iPosition">Character index.</param>
    /// <returns>Font index.</returns>
    protected virtual int GetFontIndex( int iPosition )
    {
      return m_text.GetFontByIndex( iPosition );
    }
    /// <summary>
    /// Returns font by its index.
    /// </summary>
    /// <param name="iFontIndex">Font index.</param>
    /// <returns>Font that corresponds to the specified index.</returns>
    protected virtual FontImpl GetFontByIndex( int iFontIndex )
    {
      FontImpl font = null;

      if( iFontIndex == 0 && DefaultFontIndex >= 0 )
      {
        font = DefaultFont;
      }
      else
      {
        font =  ( FontImpl )m_book.InnerFonts[ iFontIndex ];
      }

      return font;
    }
    /// <summary>
    /// This method is called before any changes made to the rich text string.
    /// </summary>
    public override void BeginUpdate()
    {
      if( m_bIsReadOnly )
        throw new ReadOnlyException();

      base.BeginUpdate();
    }
    /// <summary>
    /// This method is called after any changes made to the rich text string.
    /// </summary>
    public override void EndUpdate()
    {
      base.EndUpdate();
      this.m_logger.SetFlag(PreservedFlag.RichText);
    }
    /// <summary>
    /// Copies data from another rich text string.
    /// </summary>
    /// <param name="source">String to copy data from.</param>
    /// <param name="dicFontIndexes">Dictionary with updated font indexes.</param>
    public virtual void CopyFrom( RichTextString source, Dictionary<int, int> dicFontIndexes )
    {
      BeginUpdate();
      m_text = source.m_text.Clone( dicFontIndexes );
      EndUpdate();
    }
    /// <summary>
    /// Parse rich text string.
    /// </summary>
    /// <param name="text">TextWithFormat to parse.</param>
    /// <param name="dicFontIndexes">Dictionary with updated font indexes.</param>
    /// <param name="options">Parse options.</param>
    public virtual void Parse( TextWithFormat text, Dictionary<int, int> dicFontIndexes,
      ExcelParseOptions options )
    {
      if( text == null )
        throw new ArgumentNullException( "text" );

      m_text = text.TypedClone();
      
      // Old index - new Index
      int iFontIndex = 0;
      
      FontsCollection arrFonts = m_book.InnerFonts;
      int iRunsCount = text.FormattingRunsCount;

      if( iRunsCount > 0 )
      {
        //Dictionary<int, object> hash = new Dictionary<int, object>();

        for( int i = 0; i < iRunsCount; i++ )
        {
          iFontIndex = text.GetFontByIndex( i );
          iFontIndex = FontImpl.UpdateFontIndexes( iFontIndex, dicFontIndexes, options );

          //if( !hash.ContainsKey( iFontIndex ) )
          {
            if( iFontIndex > arrFonts.Count )
            {
              //System.Diagnostics.Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Font index too large " + iFontIndex, "Warning:" );
              iFontIndex = 0;
            }
          }

          m_text.SetFontByIndex( i, iFontIndex );
        }
      }
    }
    /// <summary>
    /// Creates a copy of the current object.
    /// </summary>
    /// <param name="parent">Parent object for the new object.</param>
    /// <returns>A copy of the current object.</returns>
    public override object Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      RichTextString result = ( RichTextString )base.Clone( parent );
      result.m_parent = parent;
      result.m_text = m_text.TypedClone();
      SetParents();

      return result;
    }

    #endregion

    #region Class helper methods
    /// <summary>
    /// Clears string and formatting.
    /// </summary>
    public virtual void Clear()
    {
      m_text = m_text.TypedClone();
      m_text.ClearFormatting();
      m_text.Text = string.Empty;
    }
    /// <summary>
    /// Adds font to all required collections..
    /// </summary>
    /// <param name="font">Font to add.</param>
    /// <returns>Font index in the collection.</returns>
    protected virtual int AddFont( IFont font )
    {
      IInternalFont fontInternal = ( IInternalFont )font;

      FontImpl fontImpl = fontInternal.Font;
      fontImpl = m_book.InnerFonts.Add( fontImpl ) as FontImpl;

      return fontImpl.Index;
    }
    /// <summary>
    /// Sets internal text object that stores rtf string.
    /// </summary>
    /// <param name="commentText">New value for the text object.</param>
    internal void SetTextObject( TextWithFormat commentText )
    {
      if( commentText == null )
        throw new ArgumentNullException( "commentText" );

      m_text = commentText;
    }
    /// <summary>
    /// Returns font which is applied to character at the specified position.
    /// </summary>
    /// <param name="iPosition">Character index.</param>
    /// <returns>Font which is applied to character at the specified position.</returns>
    internal FontImpl GetFontObject(int iPosition)
    {
      if (iPosition < 0 || iPosition >= m_text.Text.Length)
        throw new ArgumentOutOfRangeException("iPosition");

      int index = m_text.GetTextFontIndex(iPosition);
      return GetFontByIndex(index);
    }
    /// <summary>
    /// Returns size of the string part.
    /// </summary>
    /// <param name="iStartPos">Start position.</param>
    /// <param name="iEndPos">End position.</param>
    /// <returns>Size of the string part.</returns>
    private SizeF GetSizePart(int iStartPos, int iEndPos)
    {
      if (iStartPos < iEndPos)
      {
        // TODO: optimization: remove wrapper creation.
        //FontWrapper wrapper = GetFont( iStartPos ) as FontWrapper;
        FontImpl font = GetFontObject(iStartPos);//wrapper.Wrapped;

        int iLength = iEndPos - iStartPos;
        string strCurText = Text.Substring(iStartPos, iLength);
        int iIndex = strCurText.IndexOfAny(DEF_DIGITS);

        //while( iIndex >= 0 )
        //{
        //  strCurText = strCurText.Replace( strCurText[ iIndex ], DEF_ZERO );
        //  iIndex = strCurText.IndexOfAny( DEF_DIGITS );
        //}

        SizeF result = //( font.Size > 20 )
          //? font.MeasureStringSpecial( strCurText )
          //:
              font.MeasureStringSpecial(strCurText);

        return result;

        //return font.MeasureCharacterRanges( strCurText, new CharacterRange[]{ new CharacterRange( 0, strCurText.Length ) } )[ 0 ];
      }

      return new SizeF(0, 0);
    }
    /// <summary>
    /// Generates text in rtf format.
    /// </summary>
    /// <returns>Generated text.</returns>
    internal string GenerateRtfText()
    {
      m_text.Defragment();

      RtfTextWriter writer = new RtfTextWriter();

      AddFonts( writer );

      string strText = m_text.Text;
      int iCount = m_text.FormattingRunsCount;
      int iStartPos = 0;

      if( strText.Length > 0 )
      {
        if( iCount > 0 )
        {
          for( int i = 0; i <= iCount; i++ )
          {
            iStartPos = WriteFormattingRun( writer, i, iStartPos );
          }
        }
        else
        {
          WriteText( writer, DefaultFont.Index, strText );
        }
      }

      writer.WriteTag( RtfTags.RtfEnd );

      return writer.ToString();
    }
    /// <summary>
    /// Generates text in rtf format.
    /// </summary>
    /// <returns>Generated text.</returns>
    internal string GenerateRtfText(string alignment)
    {
        m_text.Defragment();

        RtfTextWriter writer = new RtfTextWriter();

        AddFonts(writer,alignment);

        string strText = m_text.Text;
        int iCount = m_text.FormattingRunsCount;
        int iStartPos = 0;

        if (strText.Length > 0)
        {
            if (iCount > 0)
            {
                for (int i = 0; i <= iCount; i++)
                {
                    iStartPos = WriteFormattingRun(writer, i, iStartPos,alignment);
                }
            }
            else
            {
                WriteText(writer, DefaultFont.Index, strText,alignment);
            }
        }

        writer.WriteTag(RtfTags.RtfEnd);

        return writer.ToString();
    }
    /// <summary>
    /// Writes formatting run with corresponding text into writer.
    /// </summary>
    /// <param name="writer">Writer to write text and formatting into.</param>
    /// <param name="iRunIndex">Index of the formatting run.</param>
    /// <param name="iStartPos">First character in the text range.</param>
    /// <returns>End position of the text range.</returns>
    private int WriteFormattingRun( RtfTextWriter writer, int iRunIndex, int iStartPos )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      int iCount = m_text.FormattingRunsCount;

      if( iRunIndex < 0 || iRunIndex > iCount )
      {
        throw new ArgumentOutOfRangeException( "iRunIndex", 
          "Value cannot be less than 0 and greater than iCount - 1" );
      }

      string strText = m_text.Text;

      int iEndPos = ( iRunIndex == iCount )
        ? strText.Length
        : m_text.GetPositionByIndex( iRunIndex );

      if( iEndPos == iStartPos ) return iStartPos;

      string strToWrite = strText.Substring( iStartPos, iEndPos - iStartPos );

      int iFontIndex = ( iRunIndex == 0 ) ? 0 : m_text.GetFontByIndex( iRunIndex - 1 );
      //int iFontIndex = m_text.GetFontByIndex( iRunIndex );

      if( iFontIndex == 0 ) iFontIndex = DefaultFont.Index;

      WriteText( writer, iFontIndex, strToWrite );

      return iEndPos;
    }
    /// <summary>
    /// Writes formatting run with corresponding text into writer.
    /// </summary>
    /// <param name="writer">Writer to write text and formatting into.</param>
    /// <param name="iRunIndex">Index of the formatting run.</param>
    /// <param name="iStartPos">First character in the text range.</param>
    /// <returns>End position of the text range.</returns>
    private int WriteFormattingRun(RtfTextWriter writer, int iRunIndex, int iStartPos,string alignment)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        int iCount = m_text.FormattingRunsCount;

        if (iRunIndex < 0 || iRunIndex > iCount)
        {
            throw new ArgumentOutOfRangeException("iRunIndex",
              "Value cannot be less than 0 and greater than iCount - 1");
        }

        string strText = m_text.Text;

        int iEndPos = (iRunIndex == iCount)
          ? strText.Length
          : m_text.GetPositionByIndex(iRunIndex);

        if (iEndPos == iStartPos) return iStartPos;

        string strToWrite = strText.Substring(iStartPos, iEndPos - iStartPos);

        int iFontIndex = (iRunIndex == 0) ? 0 : m_text.GetFontByIndex(iRunIndex - 1);
        //int iFontIndex = m_text.GetFontByIndex( iRunIndex );

        if (iFontIndex == 0) iFontIndex = DefaultFont.Index;

        WriteText(writer, iFontIndex, strToWrite,alignment);

        return iEndPos;
    }
    /// <summary>
    /// Adds all used fonts to the rtf text writer.
    /// </summary>
    /// <param name="writer">RtfTextWriter to write into.</param>
    private void AddFonts( RtfTextWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      for( int i = 0, len = m_text.FormattingRunsCount; i < len; i++ )
      {
        int iFontIndex = m_text.GetFontByIndex( i );
        FontImpl font;

        if( iFontIndex != 0 )
        {
          font = GetFontByIndex( iFontIndex );
          AddFont( font, writer );
        }
      }

      // Ensure that default font is in the collection.
      AddFont( DefaultFont, writer );

      writer.WriteTag( RtfTags.RtfBegin );
      writer.WriteFontTable();
      writer.WriteColorTable();      
    }
    /// <summary>
    /// Adds all used fonts to the rtf text writer.
    /// </summary>
    /// <param name="writer">RtfTextWriter to write into.</param>
    private void AddFonts(RtfTextWriter writer, string alignment)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        for (int i = 0, len = m_text.FormattingRunsCount; i < len; i++)
        {
            int iFontIndex = m_text.GetFontByIndex(i);
            FontImpl font;

            if (iFontIndex != 0)
            {
                font = GetFontByIndex(iFontIndex);
                AddFont(font, writer);
            }
        }

        // Ensure that default font is in the collection.
        AddFont(DefaultFont, writer);

        writer.WriteTag(RtfTags.RtfBegin);
        writer.WriteFontTable();
        writer.WriteColorTable();
        writer.WriteAlignment(alignment);
    }
    /// <summary>
    /// Adds single font to the fonts table.
    /// </summary>
    /// <param name="fontToAdd">Font to add.</param>
    /// <param name="writer">RtfTextWriter to write into.</param>
    private void AddFont( FontImpl fontToAdd, RtfTextWriter writer )
    {
      if( fontToAdd == null )
        throw new ArgumentNullException( "fontToAdd" );

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      Font nativeFont = fontToAdd.GenerateNativeFont();
      writer.AddFont( nativeFont );
      writer.AddColor( fontToAdd.RGBColor );
    }
    /// <summary>
    /// Writes text into RtfTextWriter.
    /// </summary>
    /// <param name="writer">RtfTextWriter to write into.</param>
    /// <param name="iFontIndex">Font index.</param>
    /// <param name="strText">Text value.</param>
    private void WriteText( RtfTextWriter writer, int iFontIndex, string strText )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( strText == null )
        throw new ArgumentNullException( "strText" );

      if( strText.Length == 0 )
        throw new ArgumentException( "strText - string cannot be empty" );

      IFont font = GetFontByIndex( iFontIndex );
      
      writer.WriteText(font, strText);
      
    }
    /// <summary>
    /// Writes text into RtfTextWriter.
    /// </summary>
    /// <param name="writer">RtfTextWriter to write into.</param>
    /// <param name="iFontIndex">Font index.</param>
    /// <param name="strText">Text value.</param>
    private void WriteText(RtfTextWriter writer, int iFontIndex, string strText,string alignment)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (strText == null)
            throw new ArgumentNullException("strText");

        if (strText.Length == 0)
            throw new ArgumentException("strText - string cannot be empty");

        IFont font = GetFontByIndex(iFontIndex);

        writer.WriteImageText(font, strText, ImageRTF,alignment);

    }
    internal void AddText( string text, IFont font )
    {
      string currentText = m_text.Text;
      int iCurrentTextLength = currentText.Length;
      m_text.Text += text;
      SetFont( iCurrentTextLength, m_text.Text.Length - 1, font );
      //throw new Exception( "The method or operation is not implemented." );
    }
    #endregion
  }
}
