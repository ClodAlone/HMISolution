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

using Syncfusion.XlsIO;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Stores print setup options.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  [ Biff( TBIFFRecord.PrintSetup ) ]
  public class PrintSetupRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Bit mask for error print options.
    /// </summary>
    public const ushort ErrorBitMask = 0x0C00;
    /// <summary>
    /// Start bit of error print options in m_usOptions.
    /// </summary>
    public const int ErrorStartBit = 10;
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 34;
    #endregion

    #region Class members
    /// <summary>
    /// Paper size.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usPaperSize = 9;

    /// <summary>
    /// Scaling factor in percent.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usScale = 100;

    /// <summary>
    /// Start page number.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private short m_sPageStart = 1;

    /// <summary>
    /// Fit worksheet width to this number of pages (0 = use as many as needed).
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usFitWidth = 1;

    /// <summary>
    /// Fit worksheet height to this number of pages (0 = use as many as needed).
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usFitHeight = 1;

    /// <summary>
    /// Options flag.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usOptions = 4;

    #region Options Bit fields

    /// <summary>
    /// False to print pages in columns;
    /// True to print pages in rows.
    /// </summary>
    [ BiffRecordPos( 10, 0, TFieldType.Bit ) ]
    private bool m_bLeftToRight = false;

    /// <summary>
    /// False for landscape; True for portrait.
    /// </summary>
    [ BiffRecordPos( 10, 1, TFieldType.Bit ) ]
    private bool m_bNotLandscape = true;

    /// <summary>
    /// True if paper size, scaling factor, paper orientation (portrait / landscape),
    /// print resolution, and number of copies are not initialized.
    /// </summary>
    [ BiffRecordPos( 10, 2, TFieldType.Bit ) ]
    private bool m_bNotValidSettings = true;

    /// <summary>
    /// False to print in color; True to print in black and white.
    /// </summary>
    [ BiffRecordPos( 10, 3, TFieldType.Bit ) ]
    private bool m_bNoColor = false;

    /// <summary>
    /// False for default print quality; True for draft quality.
    /// </summary>
    [ BiffRecordPos( 10, 4, TFieldType.Bit ) ]
    private bool m_bDraft = false;

    /// <summary>
    /// Indicates whether to print cell notes.
    /// </summary>
    [ BiffRecordPos( 10, 5, TFieldType.Bit ) ]
    private bool m_bNotes = false;

    /// <summary>
    /// False if paper orientation setting is valid;
    /// True if paper orientation setting is not initialized.
    /// </summary>
    [ BiffRecordPos( 10, 6, TFieldType.Bit ) ]
    private bool m_bNoOrientation = true;

    /// <summary>
    /// False for automatic page numbers;
    /// True to use starting page number.
    /// </summary>
    [ BiffRecordPos( 10, 7, TFieldType.Bit ) ]
    private bool m_bUsePage = false;

    /// <summary>
    /// False if print notes are displayed;
    /// True if print notes are at the end of sheet.
    /// </summary>
    [ BiffRecordPos( 11, 1, TFieldType.Bit ) ]
    private bool m_bPrintNotes = false;

    #endregion

    /// <summary>
    /// Print resolution in dpi.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usHResolution = 600;

    /// <summary>
    /// Vertical print resolution in dpi.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usVResolution = 600;

    /// <summary>
    /// Header margin (IEEE floating-point value).
    /// </summary>
    [ BiffRecordPos( 16, 8, TFieldType.Float ) ]
    private double m_dbHeaderMargin = 0.5;

    /// <summary>
    /// Footer margin (IEEE floating-point value).
    /// </summary>
    [ BiffRecordPos( 24, 8, TFieldType.Float ) ]
    private double m_dbFooterMargin = 0.5;

    /// <summary>
    /// Number of copies to print.
    /// </summary>
    [ BiffRecordPos( 32, 2 ) ]
    private ushort m_usCopies = 1;
    #endregion

    #region Class properties
    /// <summary>
    /// Paper size.
    /// </summary>
    public ushort PaperSize
    {
      get
      {
        return m_usPaperSize;
      }
      set
      {
        m_usPaperSize = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// Scaling factor in percent.
    /// </summary>
    public ushort Scale
    {
      get
      {
        return m_usScale;
      }
      set
      {
        m_usScale = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// Start page number.
    /// </summary>
    public short PageStart
    {
      get
      {
        return m_sPageStart;
      }
      set
      {
        m_sPageStart = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// Fit worksheet width to this number of pages (0 = use as many as needed).
    /// </summary>
    public ushort FitWidth
    {
      get
      {
        return m_usFitWidth;
      }
      set
      {
        m_usFitWidth = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// Fit worksheet height to this number of pages (0 = use as many as needed).
    /// </summary>
    public ushort FitHeight
    {
      get
      {
        return m_usFitHeight;
      }
      set
      {
        m_usFitHeight = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// Print resolution in dpi.
    /// </summary>
    public ushort HResolution
    {
      get
      {
        return m_usHResolution;
      }
      set
      {
        m_usHResolution = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// Vertical print resolution in dpi.
    /// </summary>
    public ushort VResolution
    {
      get
      {
        return m_usVResolution;
      }
      set
      {
        m_usVResolution = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// Header margin (IEEE floating-point value).
    /// </summary>
    public double HeaderMargin
    {
      get
      {
        return m_dbHeaderMargin;
      }
      set
      {
        m_dbHeaderMargin = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// Footer margin (IEEE floating-point value).
    /// </summary>
    public double FooterMargin
    {
      get
      {
        return m_dbFooterMargin;
      }
      set
      {
        m_dbFooterMargin = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// Number of copies to print.
    /// </summary>
    public ushort Copies
    {
      get
      {
        return m_usCopies;
      }
      set
      {
        m_usCopies = value;
        m_bNotValidSettings = false;
      }
    }
    /// <summary>
    /// False to print pages in columns;
    /// True to print pages in rows.
    /// </summary>
    public bool   IsLeftToRight
    {
      get
      {
        return m_bLeftToRight;
      }
      set
      {
        m_bLeftToRight = value;
        m_bNotValidSettings = false;
      }
    }
    /// <summary>
    /// False if landscape; True if portrait.
    /// </summary>
    public bool   IsNotLandscape
    {
      get
      {
        return m_bNotLandscape;
      }
      set
      {
        m_bNotLandscape = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// True if paper size, scaling factor, paper orientation (portrait / landscape),
    /// print resolution, and number of copies are not initialized.
    /// </summary>
    public bool   IsNotValidSettings
    {
      get
      {
        return m_bNotValidSettings;
      }
      set
      {
        m_bNotValidSettings = value;
      }
    }

    /// <summary>
    /// False to print in color; True to print in black and white.
    /// </summary>
    public bool   IsNoColor
    {
      get
      {
        return m_bNoColor;
      }
      set
      {
        m_bNoColor = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// False for default print quality; True for draft quality.
    /// </summary>
    public bool   IsDraft
    {
      get
      {
        return m_bDraft;
      }
      set
      {
        m_bDraft = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// Indicates whether to print cell notes.
    /// </summary>
    public bool   IsNotes
    {
      get
      {
        return m_bNotes;
      }
      set
      {
        m_bNotes = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// False if paper orientation setting is valid;
    /// True if paper orientation setting is not initialized.
    /// </summary>
    public bool   IsNoOrientation
    {
      get
      {
        return m_bNoOrientation;
      }
      set
      {
        m_bNoOrientation = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// False for automatic page numbers;
    /// True to use starting page number.
    /// </summary>
    public bool   IsUsePage
    {
      get
      {
        return m_bUsePage;
      }
      set
      {
        m_bUsePage = value;
        m_bNotValidSettings = false;
      }
    }

    /// <summary>
    /// False if print notes as displayed;
    /// True if print notes are at the end of sheet.
    /// </summary>
    public bool   IsPrintNotesAsDisplayed
    {
      get
      {
        return m_bPrintNotes;
      }
      set
      {
        m_bPrintNotes = value;
        m_bNotValidSettings = false;
      }
    }
    /// <summary>
    /// Indicates how to print errors.
    /// </summary>
    public ExcelPrintErrors PrintErrors
    {
      get
      {
        return ( ExcelPrintErrors ) ( GetUInt16BitsByMask( m_usOptions, ErrorBitMask )
          >> ErrorStartBit );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usOptions, ErrorBitMask,
          (ushort) ( (int) value << ErrorStartBit ) );
        m_bNotValidSettings = false;
      }

    }
    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  PrintSetupRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  PrintSetupRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  PrintSetupRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_iLength = DEF_RECORD_SIZE;

      m_usPaperSize = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usScale = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_sPageStart = provider.ReadInt16( iOffset );
      iOffset += 2;

      m_usFitWidth = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usFitHeight = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bLeftToRight = provider.ReadBit( iOffset, 0 );
      m_bNotLandscape = provider.ReadBit( iOffset, 1 );
      m_bNotValidSettings = provider.ReadBit( iOffset, 2 );
      if (m_bNotValidSettings)
          m_usScale = 100;
      m_bNoColor = provider.ReadBit( iOffset, 3 );
      m_bDraft = provider.ReadBit( iOffset, 4 );
      m_bNotes = provider.ReadBit( iOffset, 5 );
      m_bNoOrientation = provider.ReadBit( iOffset, 6 );
      if (m_bNoOrientation || m_bNotValidSettings)
          m_bNotLandscape = true;
      m_bUsePage = provider.ReadBit( iOffset, 7 );
      iOffset++;
       
      m_bPrintNotes = provider.ReadBit( iOffset, 1 );
      iOffset++;

      m_usHResolution = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usVResolution = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_dbHeaderMargin = provider.ReadDouble( iOffset );
      iOffset += 8;

      m_dbFooterMargin = provider.ReadDouble( iOffset );
      iOffset += 8;

      m_usCopies = provider.ReadUInt16( iOffset );
      iOffset += 2;
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <returns>Size of the record data.</returns>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = DEF_RECORD_SIZE;
      provider.WriteUInt16( iOffset, m_usPaperSize );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usScale );
      iOffset += 2;

      provider.WriteInt16( iOffset, m_sPageStart );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usFitWidth );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usFitHeight );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bLeftToRight, 0 );
      provider.WriteBit( iOffset, m_bNotLandscape, 1 );
      provider.WriteBit( iOffset, m_bNotValidSettings, 2 );
      provider.WriteBit( iOffset, m_bNoColor, 3 );
      provider.WriteBit( iOffset, m_bDraft, 4 );
      provider.WriteBit( iOffset, m_bNotes, 5 );
      provider.WriteBit( iOffset, m_bNoOrientation, 6 );
      provider.WriteBit( iOffset, m_bUsePage, 7 );
      iOffset++;

      provider.WriteBit( iOffset, m_bPrintNotes, 1 );
      iOffset++;

      provider.WriteUInt16( iOffset, m_usHResolution );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usVResolution );
      iOffset += 2;

      provider.WriteDouble( iOffset, m_dbHeaderMargin );
      iOffset += 8;

      provider.WriteDouble( iOffset, m_dbFooterMargin );
      iOffset += 8;

      provider.WriteUInt16( iOffset, m_usCopies );
      //iOffset += 2;
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion
  }
}
