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
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using System.Collections;
using Syncfusion.XlsIO.Parser;

#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// This is base class for text box shapes such as TextBox and Comment.
  /// </summary>
  public class TextBoxShapeBase : ShapeImpl
  {
    #region Constants
    /// <summary>
    /// Represents default formatting run size.
    /// </summary>
    private const int DEF_CONTINUE_FR_SIZE = 8;
    /// <summary>
    /// Value of text direction property.
    /// </summary>
    private const uint DEF_TEXTDIRECTION = 2;
    #endregion

    #region Members
    /// <summary>
    /// Horizontal alignment of the text.
    /// </summary>
    private ExcelCommentHAlign m_hAlign = ExcelCommentHAlign.Left;
    /// <summary>
    /// Vertical alignment of the text.
    /// </summary>
    private ExcelCommentVAlign m_vAlign = ExcelCommentVAlign.Top;
    /// <summary>
    /// Text rotation.
    /// </summary>
    private ExcelTextRotation m_textRotation = ExcelTextRotation.LeftToRight;
    /// <summary>
    /// Indicates whether comment text is locked.
    /// </summary>
    private bool m_bTextLocked = true;
    /// <summary>
    /// Comment text.
    /// </summary>
    private RichTextString m_strText;
    /// <summary>
    /// Length of text.
    /// </summary>
    private int m_iTextLen;
    /// <summary>
    /// Length of formatting runs.
    /// </summary>
    private int m_iFormattingLen;
    /// <summary>
    /// Shape filling color.
    /// </summary>
    private Color m_fillColor = ColorExtension.Empty;
    private Dictionary<string, string> m_unknownBodyProperties;
    /// <summary>
    /// Represents the RTFReader
    /// </summary>
    private RichTextReader m_richTextReader;
    /// <summary>
    /// Represents worksheet
    /// </summary>
    protected WorksheetImpl m_sheet;
    #endregion

    #region Properties
    /// <summary>
    /// Horizontal alignment of the text.
    /// </summary>
    public ExcelCommentHAlign HAlignment
    {
      get
      {
        return m_hAlign;
      }
      set
      {
        m_hAlign = value;
      }
    }
    /// <summary>
    /// Vertical alignment of the text.
    /// </summary>
    public ExcelCommentVAlign VAlignment
    {
      get
      {
        return m_vAlign;
      }
      set
      {
        m_vAlign = value;
      }
    }
    /// <summary>
    /// Text rotation.
    /// </summary>
    public ExcelTextRotation TextRotation
    {
      get
      {
        return m_textRotation;
      }
      set
      {
        m_textRotation = value;
      }
    }
    /// <summary>
    /// Indicates whether comment text is locked.
    /// </summary>
    public bool IsTextLocked
    {
      get
      {
        return m_bTextLocked;
      }
      set
      {
        m_bTextLocked = value;
      }
    }
    /// <summary>
    /// Comment text.
    /// </summary>
    public IRichTextString RichText
    {
      get
      {
        if( m_strText == null )
          InitializeVariables();

        return m_strText;
      }
      set
      {
          m_strText = (value as RichTextString);
      }
     }
    internal RichTextReader RichTextReader
    {
        get
        {
            if (m_richTextReader == null)
                m_richTextReader = new RichTextReader(m_sheet);

            return m_richTextReader;
        }
    }
    /// <summary>
    /// Gets or sets text box text.
    /// </summary>
    public string Text
    {
      get
      {
        return RichText.Text;
      }
      set
      {
        RichText.Text = value;
      }
    }
    /// <summary>
    /// Gets inner rich text string object. Read-only.
    /// </summary>
    internal RichTextString InnerRichText
    {
      get
      {
        return m_strText;
      }
    }
    /// <summary>
    /// Shape filling color.
    /// </summary>
    public Color FillColor
    {
      get
      {
        return m_fillColor;
      }
      set
      {
        m_fillColor = value;
      }
    }
    public Dictionary<string, string> UnknownBodyProperties
    {
      get
      {
        return m_unknownBodyProperties;
      }
      set
      {
        m_unknownBodyProperties = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="application">Application object for the new item.</param>
    /// <param name="parent">Parent object for the new item.</param>
    public TextBoxShapeBase( IApplication application, object parent )
      : base( application, parent )
    {
      InitializeVariables();
    }
    /// <summary>
    /// Extracts comment from MsofbtSpContainer.
    /// </summary>
    /// <param name="application">Application object for the current object.</param>
    /// <param name="parent">Parent object for the current object.</param>
    /// <param name="container">Container that represents comment.</param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    public TextBoxShapeBase( IApplication application, object parent, MsofbtSpContainer container,
      ExcelParseOptions options )
      : base( application, parent, container, options )
    {
    }
    /// <summary>
    /// Creates a clone of the current shape.
    /// </summary>
    /// <param name="parent">New parent for the shape object.</param>
    /// <param name="hashNewNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="addToCollections">Indicates whether we should add created
    /// shape into all necessary parent collections.</param>
    /// <returns>A copy of the current shape.</returns>
    public override IShape Clone( object parent, Dictionary<string, string> hashNewNames,
      Dictionary<int, int> dicFontIndexes, bool addToCollections )
    {
      TextBoxShapeBase result = ( TextBoxShapeBase )base.Clone( parent, hashNewNames,
        dicFontIndexes, addToCollections );
      int count=0;
      if (result.m_strText != null)
      {
          result.m_strText = (RichTextString)m_strText.Clone(result);//new RichTextString( Application, ParentWorkbook, false, true );

          count = result.m_strText.TextObject.FormattingRunsCount;
      }
      IFont font = null;
      WorkbookImpl destWorkBook = result.Workbook as WorkbookImpl;
      for (int i = 0; i < count; i++)
      {
          if (i < this.m_strText.Text.Length)
          {
              font = this.m_strText.GetFont(i,true);
              FontWrapper fontWrapper = destWorkBook.AddFont(font) as FontWrapper;
              result.m_strText.TextObject.SetFontByIndex(i, fontWrapper.FontIndex);
          }
      }

      if( m_unknownBodyProperties != null )
      {
        result.m_unknownBodyProperties = new Dictionary<string, string>();

        foreach( KeyValuePair<string, string> pair in m_unknownBodyProperties )
        {
          result.m_unknownBodyProperties.Add( pair.Key, pair.Value );
        }
      }
      //result.m_strText.SetTextObject( m_strText.TextObject.Clone() );
      //result.IsVisible = IsVisible;
      //result.CopyFrom( this, hashNewNames, dicFontIndexes );
      //result.CopyCommentOptions( this, dicFontIndexes );

      //if( addToCollections )
      //  result.Worksheet.InnerComments.AddComment( result );

      return result;
    }
    /// <summary>
    /// Sets text to the specified TextWithFormat value.
    /// </summary>
    /// <param name="text">RTF text to set.</param>
    internal void SetText( TextWithFormat text )
    {
      if( text == null )
        throw new ArgumentNullException( "commentText" );

      RichTextString rtf = ( RichTextString )RichText;
      rtf.SetTextObject( text );
    }
    /// <summary>
    /// Creates ClientTextBox record corresponding to this shape.
    /// </summary>
    /// <param name="parent">Parent record for ClientTextBox.</param>
    /// <returns>Extracted Textbox record.</returns>
    [ CLSCompliant( false ) ]
    protected MsofbtClientTextBox GetClientTextBoxRecord( MsoBase parent )
    {
      MsofbtClientTextBox result = ( MsofbtClientTextBox )MsoFactory.GetRecord(
        MsoRecords.msofbtClientTextbox );//new MsofbtClientTextBox( parent );

      TextObjectRecord textObject = ( TextObjectRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.TextObject );

      int iTextLength = ( m_strText != null ) ?
        m_strText.Text.Length :
        0;

      textObject.HAlignment = HAlignment;
      textObject.VAlignment = VAlignment;
      textObject.TextLen = ( ushort )iTextLength;
      textObject.FormattingRunsLen = 0;//16;
      textObject.IsLockText = IsTextLocked;
      textObject.Rotation = TextRotation;

      result.TextObject = textObject;

      //textObject.TextLen = ( ushort )iTextLength;

      if( iTextLength > 0 )
      {
        AddTextContinueRecords( result );
        AddFormattingContinueRecords( result, textObject );
      }
      return result;
    }
    /// <summary>
    /// dds continue records that stores text formatting of the comment shape.
    /// </summary>
    /// <param name="result">Record to put formatting into.</param>
    /// <param name="textObject">Record that stores text settings.</param>
    private void AddFormattingContinueRecords( MsofbtClientTextBox result, TextObjectRecord textObject )
    {
      if( result == null )
        throw new ArgumentNullException( "result" );

      ContinueRecord formattingContinue;

      byte[] arrFormatting = ConvertFromShortToLongFR( SerializeFormattingRuns() );
      int iSize = ( arrFormatting != null ) ?
        arrFormatting.Length :
        0;

      if( arrFormatting != null )
      {
        int iOffset = 0;

        while( iOffset < iSize )
        {
          int iBytesToWrite = Math.Min( iSize - iOffset, BiffRecordRaw.DEF_RECORD_MAX_SIZE );

          formattingContinue = ( ContinueRecord )BiffRecordFactory.GetRecord(
            TBIFFRecord.Continue );

          byte[] arrData = new byte[ iBytesToWrite ];
          Buffer.BlockCopy( arrFormatting, iOffset, arrData, 0, iBytesToWrite );

          formattingContinue.SetData( arrData );
          formattingContinue.SetLength( iBytesToWrite );
          result.AddRecord( formattingContinue );
          iOffset += iBytesToWrite;
        }
      }
      else
      {
        formattingContinue = ( ContinueRecord )BiffRecordFactory.GetRecord(
          TBIFFRecord.Continue );
        formattingContinue.SetLength( 0 );
        result.AddRecord( formattingContinue );
      }

      textObject.FormattingRunsLen = ( ushort )iSize;
    }
    /// <summary>
    /// Adds continue records that stores text of the comment shape.
    /// </summary>
    /// <param name="result">MsofbtClientTextBox record that stores comment settings.</param>
    private void AddTextContinueRecords( MsofbtClientTextBox result )
    {
      const int MaxCharsInContinue = ( BiffRecordRaw.DEF_RECORD_MAX_SIZE - 1 ) / 2;

      if( result == null )
        throw new ArgumentNullException( "result" );

      string strText = m_strText.Text;
      int iTextLength = strText.Length;

      int iOffset = 0;
      while( iOffset < iTextLength )
      {
        int iCharsToWrite = Math.Min( iTextLength - iOffset, MaxCharsInContinue );
        ContinueRecord textContinue = ( ContinueRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.Continue );
        textContinue.AutoGrowData = true;

        // NOTE: here we have to split continue into parts if necessary.
        string strTextPart = strText.Substring( iOffset, iCharsToWrite );
        int len = textContinue.SetStringNoLenDetectEncoding( 0, strTextPart );

        textContinue.SetLength( len );
        iOffset += iCharsToWrite;

        //result.TextContinue = textContinue;
        result.AddRecord( textContinue );
      }
    }
    /// <summary>
    /// Parses TextObjectRecord.
    /// </summary>
    /// <param name="textObject">Record to parse.</param>
    /// <exception cref="System.ArgumentNullException">If textObject is NULL.</exception>
    private void ParseTextObject( TextObjectRecord textObject )
    {
      if( textObject == null )
        throw new ArgumentNullException( "textObject" );

      m_hAlign = textObject.HAlignment;
      m_vAlign = textObject.VAlignment;
      m_textRotation = textObject.Rotation;
      m_bTextLocked = textObject.IsLockText;
      m_iTextLen = textObject.TextLen;
      m_iFormattingLen = textObject.FormattingRunsLen;
    }
    /// <summary>
    /// Parses two continue records. First contains text, second is formatting runs.
    /// </summary>
    /// <param name="strText">Comment text.</param>
    /// <param name="formattingRuns">Comment formatting runs.</param>
    /// <param name="options">Parse options.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When textRecord or formattingRecord is NULL.
    /// </exception>
    private void ParseContinueRecords( string strText, byte[] formattingRuns,
      ExcelParseOptions options )
    {
      //if( textRecord == null )
      //  throw new ArgumentNullException( "textRecord" );

      //if( formattingRecord == null )
      //  throw new ArgumentNullException( "formattingRecord" );

      TextWithFormat text = new TextWithFormat();
      text.Text = strText;//textRecord.GetString( 0, m_iTextLen );

      if( formattingRuns != null )
      {
        int iFRCount = formattingRuns.Length / DEF_CONTINUE_FR_SIZE;

        byte[] arrMergedFRuns = new byte[ iFRCount * TextWithFormat.DEF_FR_SIZE ];

        for( int i = 0; i < iFRCount; i++ )
        {
          //Array.Copy( formattingRecord.Data, i * DEF_CONTINUE_FR_SIZE, arrMergedFRuns
          //  , i * TextWithFormat.DEF_FR_SIZE, 4 );
          Buffer.BlockCopy( formattingRuns, i * DEF_CONTINUE_FR_SIZE, arrMergedFRuns,
            i * TextWithFormat.DEF_FR_SIZE, 4 );
        }

        text.ParseFormattingRuns( arrMergedFRuns );
      }

      m_strText.Parse( text, null, options );
    }
    /// <summary>
    /// Serializes formatting runs.
    /// </summary>
    /// <returns>Array that contains formatting runs in binary format.</returns>
    private byte[] SerializeFormattingRuns()
    {
      byte[] arrFR = m_strText.TextObject.SerializeFormatting();

      if( arrFR == null || arrFR.Length == 0 )
      {
        byte[] arrResult = new byte[ 8 ];
        byte[] arrLength = BitConverter.GetBytes( ( ushort )m_strText.Text.Length );

        arrResult[ 4 ] = arrLength[ 0 ];
        arrResult[ 5 ] = arrLength[ 1 ];

        return arrResult;
      }
      else
      {
        // Insert formatting for first character.
        if( !( arrFR[ 0 ] == 0 && arrFR[ 1 ] == 0 ) )
        {
          byte[] buffer = arrFR;
          arrFR = new byte[ arrFR.Length + TextWithFormat.DEF_FR_SIZE ];
          buffer.CopyTo( arrFR, TextWithFormat.DEF_FR_SIZE );

          BitConverter.GetBytes( ( ushort )0 ).CopyTo( arrFR, 0 );
          BitConverter.GetBytes( ( ushort )0 ).CopyTo( arrFR, 1 );
        }

        int len = arrFR.Length;

        // Insert last formatting for past the end characters.
        if( BitConverter.ToUInt16( arrFR, len - TextWithFormat.DEF_FR_SIZE )
          != m_strText.Text.Length )
        {
          byte[] buffer = arrFR;
          arrFR = new byte[ arrFR.Length + TextWithFormat.DEF_FR_SIZE ];
          buffer.CopyTo( arrFR, 0 );
          BitConverter.GetBytes( ( ushort )m_strText.Text.Length ).CopyTo( arrFR, buffer.Length );
          BitConverter.GetBytes( ( ushort )0 ).CopyTo( arrFR, buffer.Length + 2 );
        }
      }

      return arrFR;
    }
    /// <summary>
    /// Converts short form of formatting runs into long form.
    /// </summary>
    /// <param name="arrShortFR">Formatting runs to convert.</param>
    /// <returns>Long for of formatting runs.</returns>
    private byte[] ConvertFromShortToLongFR( byte[] arrShortFR )
    {
      if( arrShortFR == null )
        return null;

      int iCount = arrShortFR.Length / TextWithFormat.DEF_FR_SIZE;
      byte[] arrResult = new byte[ iCount * DEF_CONTINUE_FR_SIZE ];

      for( int i = 0; i < iCount; i++ )
      {
        Buffer.BlockCopy( arrShortFR, i * TextWithFormat.DEF_FR_SIZE, arrResult
          , i * DEF_CONTINUE_FR_SIZE, TextWithFormat.DEF_FR_SIZE );
      }

      return arrResult;
    }
    /// <summary>
    /// Initializes variables.
    /// </summary>
    protected virtual void InitializeVariables()
    {
      m_strText = new RichTextString( Application, ParentWorkbook, this, false, true );
      m_bSupportOptions = true;
    }
    /// <summary>
    /// Parses ClientTextBox record.
    /// </summary>
    /// <param name="textBox">Record to parse.</param>
    /// <param name="options">Parse options.</param>
    [CLSCompliant( false )]
    protected virtual void ParseClientTextBoxRecord( MsofbtClientTextBox textBox,
      ExcelParseOptions options )
    {
      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      RichText.Text = string.Empty;

      ParseTextObject( textBox.TextObject );

      string strText = textBox.Text;
      byte[] arrRuns = textBox.FormattingRuns;

      if( strText != null && arrRuns != null )
      {
        ParseContinueRecords( strText, arrRuns, options );
      }

    }
    /// <summary>
    /// Copies data from another comment shape.
    /// </summary>
    /// <param name="source">Text shape to copy from.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    public void CopyFrom( TextBoxShapeBase source, Dictionary<int, int> dicFontIndexes )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      m_strText.CopyFrom( source.m_strText, dicFontIndexes );
    }
    /// <summary>
    /// Creates default shape options.
    /// </summary>
    /// <returns>Record containing default options.</returns>
    [CLSCompliant( false )]
    protected override MsofbtOPT CreateDefaultOptions()
    {
      MsofbtOPT result = base.CreateDefaultOptions();
      result.Version = 3;
      result.Instance = 2;

        if(Text.Length!=0)
            SerializeOption(result, MsoOptions.TextId, 19990000);//119315524 );
      //SerializeOption( result, MsoOptions.TextDirection, 2 );
      SerializeTextDirection( result );
      SerializeSizeTextToFit( result );

      //MsofbtOPT.FOPTE option = SerializeOption( result, MsoOptions.BlipId, m_iImageIndex );
      //option.IsValid = true;

      //SerializeOption( result, MsoOptions.PictureId, 1 );//m_iImageIndex );
      //SerializeOption( result, MsoOptions.ForeColor, 134217793 );
      //SerializeOption( result, MsoOptions.LineColor, 134217792 );
      //SerializeOption( result, MsoOptions.NoLineDrawDash, 524288 );
      //SerializeShapeName( result );
      // ShapeName
      //SerializeOption344( result );

      return result;
    }
    /// <summary>
    /// Serializes text direction.
    /// </summary>
    /// <param name="options">MsofbtOPT record to which text ID will be added.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If options argument is NULL.
    /// </exception>
    [CLSCompliant( false )]
    protected void SerializeTextDirection( MsofbtOPT options )
    {
      SerializeOption( options, MsoOptions.TextDirection, DEF_TEXTDIRECTION );
      //      if( options == null )
      //        throw new ArgumentNullException( "options" );
      //
      //      FOPTE option = new FOPTE();
      //      option.Id = MsoOptions.TextDirection;
      //      option.UInt32Value = ( uint )2;
      //      option.IsValid = false;
      //      option.IsComplex = false;
      //      options.AddOptionsOrReplace( option );
    }
    /// <summary>
    /// Serializes comment's options.
    /// </summary>
    /// <param name="parent">Parent record for options.</param>
    /// <returns>All options in MsofbtOPT record.</returns>
    [CLSCompliant( false )]
    protected override MsofbtOPT SerializeOptions( MsoBase parent )
    {
        if (m_options != null)
        {
            if (m_options.Properties.Length != 0)
                return m_options;
        }
      
          MsofbtOPT result = m_options;
          if (m_bUpdateLineFill || m_options == null)
          {
              // TODO: uncomment one of the lines
              //MsofbtOPT result = base.SerializeOptions( parent );
              result = m_options = CreateDefaultOptions();

              result = SerializeMsoOptions(m_options);
              //result.Instance = result.PropertyList.Count;
          }

          SerializeShapeName(result);
          SerializeName(result, MsoOptions.AlternativeText, AlternativeText);
      
      //SerializeHitTest( result );
      //      SerializeForeShadowColor( result );
      //      SerializeOptionShadowObscured( result );
      //      SerializeShapeName( result );

      return m_options;
    }
    /// <summary>
    /// Parses all unknown for ShapeImpl records.
    /// </summary>
    /// <param name="subRecord">Record to parse.</param>
    /// <param name="options">Parse options.</param>
    [CLSCompliant( false )]
    protected override void ParseOtherRecords( MsoBase subRecord, ExcelParseOptions options )
    {
      if( subRecord == null )
        throw new ArgumentNullException( "subRecord" );

      switch( subRecord.MsoRecordType )
      {
        case MsoRecords.msofbtClientTextbox:
          ParseClientTextBoxRecord( subRecord as MsofbtClientTextBox, options );
          break;
      }
    }
    #endregion

      #region Finalization
    public override void Dispose()
    {
        base.Dispose();
        
        if (m_unknownBodyProperties != null)
            m_unknownBodyProperties.Clear();
        if (m_strText != null)
            m_strText = null;
  }
      #endregion
  }
}
