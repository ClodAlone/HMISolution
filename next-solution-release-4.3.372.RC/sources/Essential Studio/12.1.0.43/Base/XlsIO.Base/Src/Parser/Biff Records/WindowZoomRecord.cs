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
  /// This record stores the magnification of the active view of the current worksheet. 
  /// In BIFF8 this can be either the normal view or the page break preview. 
  /// This is determined in the WINDOW2 record.
  /// The magnification is stored as reduced fraction. The magnification results 
  /// from nscl / dscl.
  /// </summary>
  [ Biff( TBIFFRecord.WindowZoom ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class WindowZoomRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    private const int DEF_RECORD_SIZE = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Numerator of the view magnification fraction.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usNscl = 100;
    /// <summary>
    /// Denominator of the view magnification fraction.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usDscl = 100;
    #endregion

    #region Class Properties
    /// <summary>
    /// Numerator of the view magnification fraction.
    /// The magnification results from NumMagnification / DenumMagnification.
    /// </summary>
    public ushort NumMagnification
    {
      get
      {
        return m_usNscl;
      }
      set
      {
        m_usNscl = value;
      }
    }
    /// <summary>
    /// Denominator of the view magnification fraction.
    /// The magnification results from NumMagnification / DenumMagnification.
    /// </summary>
    public ushort DenumMagnification
    {
      get
      {
        return m_usDscl;
      }
      set
      {
        m_usDscl = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Zoom
    {
      get
      {
        return ( int )(( double )m_usNscl * 100 / ( double )m_usDscl );
      }
      set
      {
        if( value < 10 || value > 400 )
          throw new ArgumentOutOfRangeException( "Zoom", "Zoom must be in range from 10 and 400." );

        m_usNscl = ( ushort )value;
        m_usDscl = 100;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  WindowZoomRecord()
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
    public  WindowZoomRecord( Stream stream, out int itemSize )
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
    public  WindowZoomRecord( int iReserve )
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
      m_usNscl = provider.ReadUInt16( iOffset );
      m_usDscl = provider.ReadUInt16( iOffset + 2 );
    }

    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = DEF_RECORD_SIZE;
      provider.WriteUInt16( iOffset, m_usNscl );
      provider.WriteUInt16( iOffset + 2, m_usDscl );
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