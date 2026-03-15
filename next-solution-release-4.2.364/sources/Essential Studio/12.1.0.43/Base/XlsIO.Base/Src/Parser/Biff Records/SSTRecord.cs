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
using System.Text;
using System.Collections;
using System.Diagnostics;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Security;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Static String Table Record:
  /// This holds all the strings for LabelSSTRecords.
  /// </summary>
  [ Biff( TBIFFRecord.SST ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class SSTRecord : BiffRecordWithContinue
  {
    #region Class constants
    /// <summary>
    /// Options byte offset in the string.
    /// </summary>
    private const int DEF_OPTIONS_OFFET = 2;
    #endregion

    #region Class members
    /// <summary>
    /// Number of string in workbook.
    /// </summary>
    [ BiffRecordPos( 0, 4 ) ]
    private uint m_uiNumberOfStrings = 0;
    /// <summary>
    /// Number of unique strings in workbook.
    /// </summary>
    [ BiffRecordPos( 4, 4 ) ]
    private uint m_uiNumberOfUniqueStrings = 0;
    /// <summary>
    /// Array of workbook's strings.
    /// </summary>
    private object[] m_arrStrings;
    /// <summary>
    /// Array that stores positions of the strings in the m_data array.
    /// </summary>
    private int[] m_arrStringsPos;
    /// <summary>
    /// Array that stores offsets of the strings starting from the beginning
    /// of the record (Continue or SST).
    /// </summary>
    private int[] m_arrStringOffset;
    /// <summary>
    ///
    /// </summary>
    private bool m_bAutoAttach = true;
#if DEBUG_SST
    /// <summary>
    /// Total time for continue records extraction.
    /// </summary>
    private TimeSpan m_span = new TimeSpan( 0 );
#endif
    #endregion

    #region Class properties
    /// <summary>
    /// Number of string in workbook.
    /// </summary>
    public  uint NumberOfStrings
    {
      get
      {
        return m_uiNumberOfStrings;
      }
      set
      {
        m_uiNumberOfStrings = value;
      }
    }

    /// <summary>
    /// Number of unique strings in workbook.
    /// </summary>
    public uint NumberOfUniqueStrings
    {
      get
      {
        return m_uiNumberOfUniqueStrings;
      }
    }

    /// <summary>
    /// Array of workbook's strings.
    /// </summary>
    public object[] Strings
    {
      get
      {
        return m_arrStrings;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_arrStrings = value;
        m_uiNumberOfUniqueStrings = ( uint ) m_arrStrings.Length;
      }
    }

    /// <summary>
    /// Array that stores positions of the strings in the m_data array.
    /// </summary>
    public int[] StringsStreamPos
    {
      get
      {
        return m_arrStringsPos;
      }
    }
    /// <summary>
    /// Array that stores offsets of the strings starting from the beginning
    /// of the record (Continue or SST).
    /// </summary>
    public int[] StringsOffsets
    {
      get
      {
        return m_arrStringOffset;
      }
    }
    /// <summary>
    /// Configuration property. If value is True, then on detection of dataless
    /// record, the class will try to continue to get records from the stream.
    /// </summary>
    public bool  AutoAttachContinue
    {
      get
      {
        return m_bAutoAttach;
      }
      set
      {
        m_bAutoAttach = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  SSTRecord()
      : base()
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <exception cref="WrongBiffRecordDataException">
    /// If any internal error occurred.
    /// </exception>
    public override void ParseStructure()
    {
      m_uiNumberOfStrings = m_provider.ReadUInt32( 0 );
      m_uiNumberOfUniqueStrings = m_provider.ReadUInt32( 4 );

#if DEBUG_SST
      ////Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, DumpArrayToHex( m_data ) );
      DateTime start = DateTime.Now;
#endif

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_uiNumberOfStrings, "Number of strings" );
      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_uiNumberOfUniqueStrings, "Number of unique strings" );
      m_arrStrings = //new string[ m_uiNumberOfUniqueStrings ];
        //new TextWithFormat[ m_uiNumberOfUniqueStrings ];
        new object[ m_uiNumberOfUniqueStrings ];

      int iCurPos = 8;
      int iBytes;
      byte[] dataRich, dataEast;

      //m_arrContinuePos.Add( m_data.Length );
      int iContinueCount = m_arrContinuePos.Count;
      int iBreakPos = 0;
      TextWithFormat rtfString = new TextWithFormat( 0 );

      for( int i = 0; i < m_uiNumberOfUniqueStrings; i++ )
      {
        if( 3 + iCurPos > m_iLength )
          throw new WrongBiffRecordDataException( "SSTRecord" );

        string text = GetUnkTypeString( iCurPos, m_arrContinuePos, iContinueCount, ref iBreakPos,
          out iBytes, out dataRich, out dataEast );

        object newString = null;

        if( dataRich != null && dataRich.Length > 0 )
        {
          TextWithFormat rtf = rtfString.TypedClone();
          rtf.Text = text;
          rtf.ParseFormattingRuns( dataRich );
          newString = rtf;
        }
        else
        {
          newString = text;
        }

        m_arrStrings[ i ] = newString;
        iCurPos += iBytes;
      }

      InternalDataIntegrityCheck( iCurPos );

#if DEBUG_SST
      Console.WriteLine( "Extract continue time: {0}", m_span );
      //System.Windows.Forms.MessageBox.Show( ( DateTime.Now - start ).ToString(), "SST time" );
      //System.Windows.Forms.MessageBox.Show( ( new StackTrace() ).ToString() );
#endif
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_arrContinuePos.Clear();
      PrognoseRecordSize();
      m_uiNumberOfStrings = m_uiNumberOfUniqueStrings;
      m_provider.WriteUInt32( 0, m_uiNumberOfStrings );
      m_provider.WriteUInt32( 4, m_uiNumberOfUniqueStrings );
      m_iLength = 8;

      m_arrStringsPos = new int[ m_uiNumberOfUniqueStrings ];
      m_arrStringOffset = new int[ m_uiNumberOfUniqueStrings ];
      
      byte[] arrStringData = null;
      byte[] arrFormattingData = null;
      TextWithFormat tempString = new TextWithFormat( 0 );
      IntPtrContinueRecordBuilder builder = new IntPtrContinueRecordBuilder( this, DEF_HEADER_SIZE );

      for( int i = 0; i < m_uiNumberOfUniqueStrings; i++ )
      {
        // Get string from data array.
        object stringFromArray = m_arrStrings[ i ];
        TextWithFormat curString = stringFromArray as TextWithFormat;
        int iFormattingSize = 0;//curString.GetFormattingSize();
        string strText;
        TextWithFormat.StringType options = TextWithFormat.StringType.Unicode;
        Encoding encoding = Encoding.Unicode;

        if( curString == null )
        {
          strText = ( string )stringFromArray;
        }
        else
        {
          strText = curString.Text;
          iFormattingSize = curString.GetFormattingSize();
          options = curString.GetOptions();
        }

        if( BiffRecordRawWithArray.IsAsciiString( strText ) )
        {
          options &= ~TextWithFormat.StringType.Unicode;
          options |= TextWithFormat.StringType.NonUnicode;
          encoding = Encoding.UTF8;
        }

        ushort iLen = ( ushort )strText.Length;
        int totalLen = encoding.GetByteCount( strText );
        int iTextSize = totalLen + 3;//curString.GetTextSize();

        EnsureSize( ref arrStringData, iTextSize );
        EnsureSize( ref arrFormattingData, iFormattingSize );

        encoding.GetBytes( strText, 0, iLen, arrStringData, 0 );

        if( iFormattingSize > 0 )
          curString.SerializeFormatting( arrFormattingData, 0, false );

        m_arrStringsPos[ i ] = builder.Position + BiffRecordRaw.DEF_HEADER_SIZE;
        m_arrStringOffset[ i ] = builder.Offset;

        // If available free space is too small for string headers.
        if( builder.FreeSpace < 20 )
          builder.StartContinueRecord();

        //int totalLen = iLen * 2;
        builder.AppendUInt16( iLen ); // store length

        // Serialize string data.
        InfillText( builder, options, iFormattingSize, totalLen, arrStringData );

        // Serialize formatting.
        InfillFormatting( builder, arrFormattingData, iFormattingSize );
      }

      m_iLength = builder.Total;
      m_iFirstLength = builder.FirstRecordLength;
      builder.Dispose();
      builder = null;
    }
    /// <summary>
    /// Serializes text data.
    /// </summary>
    /// <param name="builder">Record builder to put data into.</param>
    /// <param name="options">String options.</param>
    /// <param name="iFormattingSize">Size of the formatting data.</param>
    /// <param name="totalLen">Total size of the string data.</param>
    /// <param name="arrStringData">Array containing string data.</param>
    private void InfillText( IntPtrContinueRecordBuilder builder, TextWithFormat.StringType options,
      int iFormattingSize, int totalLen, byte[] arrStringData )
    {
      int iDataPos = 0;

      do
      {
        builder.AppendByte( ( byte )options ); // always unicode string

        if( iDataPos == 0 && iFormattingSize > 0 )
        {
          // Store number of formatting runs.
          builder.AppendUInt16( ( ushort )( iFormattingSize / TextWithFormat.DEF_FR_SIZE ) );
        }

        // Length always must be odd
        int iDataLen = ( ( options & TextWithFormat.StringType.Unicode ) != 0 ) ?
          ( Math.Min( builder.FreeSpace, totalLen - iDataPos ) / 2 ) * 2 :
          Math.Min( builder.FreeSpace, totalLen - iDataPos );
          //( int )( Math.Ceiling( Math.Min( builder.FreeSpace, totalLen - iDataPos ) / 2.0 ) ) * 2;

        builder.AppendBytes( arrStringData, iDataPos, iDataLen );
        //builder.UpdateContinueRecordSize();

        iDataPos += iDataLen;

        if( iDataPos < totalLen )
          builder.StartContinueRecord();

        if( ( options & TextWithFormat.StringType.Unicode ) != 0 )
        {
          options = TextWithFormat.StringType.Unicode;
        }
        else
        {
          options = TextWithFormat.StringType.NonUnicode;
        }
      }
      while( iDataPos < totalLen );

      // Serialize formatting runs.
      iDataPos = 0;
    }
    /// <summary>
    /// Infills formatting bytes.
    /// </summary>
    /// <param name="builder">Record builder to put data into.</param>
    /// <param name="arrFormattingData">Array containing formatting data.</param>
    /// <param name="iFormattingSize">Size of the formatting data.</param>
    private void InfillFormatting( IntPtrContinueRecordBuilder builder,
      byte[] arrFormattingData, int iFormattingSize )
    {
      int iDataPos = 0;
      if( /*arrFormattingData != null*/iFormattingSize > 0 )
      {
        int iFormattingLen = 0;

        do
        {
          iFormattingLen = ( Math.Min( builder.FreeSpace, iFormattingSize - iDataPos )
            / TextWithFormat.DEF_FR_SIZE ) * TextWithFormat.DEF_FR_SIZE;

          builder.AppendBytes( arrFormattingData, iDataPos, iFormattingLen );
          builder.UpdateContinueRecordSize();

          iDataPos += iFormattingLen;

          if( iDataPos < iFormattingSize )
            builder.StartContinueRecord();
        }
        while( iDataPos < iFormattingSize );
      }
    }
    /// <summary>
    /// Tries to prognose records size and prepare data storage so it won't require resize operation.
    /// </summary>
    private void PrognoseRecordSize()
    {
      int iPrognoseSize = 0;

      for( int i = 0; i < m_uiNumberOfUniqueStrings; i++ )
      {
        object objString = m_arrStrings[ i ];
        TextWithFormat text = objString as TextWithFormat;
        string strText;

        if( text == null )
        {
          strText = ( string )objString;
        }
        else
        {
          strText = text.Text;
          int iFormattingCount = text.FormattingRunsCount;

          if( iFormattingCount > 0 )
          {
            iPrognoseSize += text.FormattingRunsCount * TextWithFormat.DEF_FR_SIZE + 2;
          }
        }

        iPrognoseSize += strText.Length * 2 + 3;
      }

      iPrognoseSize += iPrognoseSize / 1000; // Try to reserve some space for continue records.
      m_provider.EnsureCapacity( 8 + iPrognoseSize );
    }
    /// <summary>
    /// Resizes buffer if necessary.
    /// </summary>
    /// <param name="arrBuffer">Buffer to check.</param>
    /// <param name="iSize">Desired buffer size.</param>
    public static void EnsureSize( ref byte[] arrBuffer, int iSize )
    {
      int iLength = ( arrBuffer == null ) ? 0 : arrBuffer.Length;

      if( iLength < iSize )
      {
        arrBuffer = new byte[ iSize ];
      }
    }
    /// <summary>
    /// This method checks record's internal data array for integrity.
    /// </summary>
    /// <exception cref="WrongBiffRecordDataException">If there is any internal error.</exception>
    private void InternalDataIntegrityCheck( int iCurPos )
    {
//      if( iCurPos != m_data.Length && iCurPos + 1 != m_data.Length )
//        throw new WrongBiffRecordDataException( "SSTRecord1" );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      if( NeedInfill )
      {
        InfillInternalData( version );
        NeedInfill = false;
      }

      return m_iLength;
    }
    #endregion

    #region Class event handlers
//    /// <summary>
//    /// OnFirstContinue event handler.
//    /// </summary>
//    /// <param name="sender">Event sender.</param>
//    /// <param name="e">Event arguments.</param>
//    private void builder_OnFirstContinue( object sender, EventArgs e )
//    {
//      ContinueRecordBuilder builder = ( ContinueRecordBuilder )sender;
//      builder.OnFirstContinue -= new EventHandler( builder_OnFirstContinue );
//
//      m_iIntLen = builder.Position;
//    }
    #endregion
  }
}