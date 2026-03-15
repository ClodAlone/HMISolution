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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores a text object. The TXO record is followed
  /// by two CONTINUE records. The first CONTINUE record contains the text
  /// data and the second CONTINUE contains the formatting runs.
  /// If the text box contains no text, these CONTINUE records
  /// are not written to the file.
  /// </summary>
  [ Biff( TBIFFRecord.TextObject ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class TextObjectRecord : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// Bit mask for horizontal alignment.
    /// </summary>
    public const ushort HAlignmentBitMask = 0x000E;
    /// <summary>
    /// Bit mask for vertical alignment.
    /// </summary>
    public const ushort VAlignmentBitMask = 0x0070;
    #endregion

    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort  m_usOptions;
    /// <summary>
    /// Whether the Lock Text option is on.
    /// </summary>
    [ BiffRecordPos( 1, 2, TFieldType.Bit ) ]
    private bool    m_bLockText;
    /// <summary>
    /// Orientation of text within the object boundary.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort  m_usRotation = ( ushort ) ExcelTextRotation.LeftToRight;
    /// <summary>
    /// Reserved, must be zero.
    /// </summary>
    [ BiffRecordPos( 4, 4 ) ]
    private uint    m_uiReserved1 = 0;
    /// <summary>
    /// Reserved, must be zero.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort  m_usReserved2 = 0;
    /// <summary>
    /// Length of text (in first CONTINUE record).
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort  m_usTextLen;
    /// <summary>
    /// Length of formatting runs (in second CONTINUE record).
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort  m_usFormattingRunsLen;
    /// <summary>
    /// Reserved, must be zero.
    /// </summary>
    [ BiffRecordPos( 14, 4 ) ]
    private uint    m_uiReserved3 = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// Horizontal text alignment.
    /// This property changes bits of m_usOptions private field.
    /// </summary>
    public ExcelCommentHAlign HAlignment
    {
      get
      {
        return ( ExcelCommentHAlign )
          ( GetUInt16BitsByMask( m_usOptions, HAlignmentBitMask ) >> 1 );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usOptions, HAlignmentBitMask,
          ( ushort ) ( ( ushort ) value << 1 ) );
      }
    }
    /// <summary>
    /// Vertical text alignment:
    /// This property changes bits of m_usOptions private field.
    /// </summary>
    public ExcelCommentVAlign VAlignment
    {
      get
      {
        return ( ExcelCommentVAlign )
          ( GetUInt16BitsByMask( m_usOptions, VAlignmentBitMask ) >> 4 );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usOptions, VAlignmentBitMask,
          ( ushort ) ( ( ushort ) value << 4 ) );
      }
    }

    /// <summary>
    /// Whether the Lock Text option is on.
    /// </summary>
    public bool IsLockText
    {
      get
      {
        return m_bLockText;
      }
      set
      {
        m_bLockText = value;
      }
    }

    /// <summary>
    /// Orientation of text within the object boundary.
    /// </summary>
    public ExcelTextRotation Rotation
    {
      get
      {
        return ( ExcelTextRotation ) m_usRotation;
      }
      set
      {
        m_usRotation = ( ushort ) value;
      }
    }

    /// <summary>
    /// Length of text (in first CONTINUE record).
    /// </summary>
    public ushort TextLen
    {
      get
      {
        return m_usTextLen;
      }
      set
      {
        m_usTextLen = value;
      }
    }

    /// <summary>
    /// Length of formatting runs (in second CONTINUE record).
    /// </summary>
    public ushort FormattingRunsLen
    {
      get
      {
        return m_usFormattingRunsLen;
      }
      set
      {
        m_usFormattingRunsLen = value;
      }
    }
    /// <summary>
    /// Not used.
    /// </summary>
    public uint   Reserved1
    {
      get
      {
        return m_uiReserved1;
      }
    }
    /// <summary>
    /// Not used.
    /// </summary>
    public ushort Reserved2
    {
      get
      {
        return m_usReserved2;
      }
    }
    /// <summary>
    /// Not used.
    /// </summary>
    public uint Reserved3
    {
      get
      {
        return m_uiReserved3;
      }
    }
    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 18;
      }
    }

    /// <summary>
    /// Read-only. Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return 18;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  TextObjectRecord()
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
    public  TextObjectRecord( Stream stream, out int itemSize )
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
    public  TextObjectRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      m_usOptions = GetUInt16( 0 );
      m_bLockText = GetBit(1, 1);
      m_usRotation = GetUInt16( 2 );
      m_uiReserved1 = GetUInt32( 4 );
      m_usReserved2 = GetUInt16( 8 );
      m_usTextLen = GetUInt16( 10 );
      m_usFormattingRunsLen = GetUInt16( 12 );
      m_uiReserved3 = GetUInt32( 14 );
      m_data = new byte[ 0 ];

      /*
      base.ParseStructure();

      m_arrContinuePos.Add( m_data.Length );
      
      int iCurPos = MaximumRecordSize;
      int iBytes;
      byte[] dataRich, dataEast;
      
      string text = GetUnkTypeString( iCurPos,
          (int[]) m_arrContinuePos.ToArray( typeof(int) ),
          out iBytes, out dataRich, out dataEast );

      iCurPos += iBytes;
      */
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    ///an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      //base.InfillInternalData();
      m_iLength = MinimumRecordSize;
      m_data = new byte[ m_iLength ];

      SetUInt16( 0, m_usOptions );
      SetBit( 1, m_bLockText, 2 );
      SetBit( 1, m_bLockText, 1 );
      SetUInt16( 2, m_usRotation );
      SetUInt32( 4, m_uiReserved1 );
      SetUInt16( 8, m_usReserved2 );
      SetUInt16( 10, m_usTextLen );
      SetUInt16( 12, m_usFormattingRunsLen );
      SetUInt32( 14, m_uiReserved3 );

      /*
      /// NOTE: internally data written to m_data array, that is why we 
      /// need here copy of data before any data saves...
      byte[] tmp = new byte[ m_data.Length ];
      m_data.CopyTo( tmp, 0 );

      Builder.AppendBytes( tmp, m_iLength, tmp.Length );
      */
    }
    #endregion
  }
}
