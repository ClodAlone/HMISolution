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

using System;
using System.IO;


namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// The record represents an empty cell.
  /// It contains the cell address and formatting information.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ Biff( TBIFFRecord.FileSharing ) ]
  [ CLSCompliant( false ) ]
  public class FileSharingRecord : BiffRecordRaw
  {
    #region Class members
    /// <summary>
    /// 1 = Recommend Read-only state while loading the file.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usRecommendReadOnly = 0;
    /// <summary>
    /// Hash value calculated from the Read-only password.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usHashPassword;
    /// <summary>
    /// User name of the file creator.
    /// </summary>
    [ BiffRecordPos( 4, 2, TFieldType.String16Bit ) ]
    private string m_strCreatorName;
    #endregion

    #region Class Properties
    /// <summary>
    /// 1 = Recommend Read-only state while loading the file.
    /// </summary>
    public ushort RecommendReadOnly
    {
      get
      {
        return m_usRecommendReadOnly;
      }
      set
      {
        m_usRecommendReadOnly = value;
      }
    }

    /// <summary>
    /// Hash value calculated from the Read-only password.
    /// </summary>
    public ushort HashPassword
    {
      get
      {
        return m_usHashPassword;
      }
      set
      {
        m_usHashPassword = value;
      }
    }

    /// <summary>
    /// Index to XF (extended format) record.
    /// </summary>
    public string CreatorName
    {
      get
      {
        return m_strCreatorName;
      }
      set
      {
        m_strCreatorName = value;
      }
    }

    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    override public int MinimumRecordSize
    {
      get
      {
        return 6;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  FileSharingRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  FileSharingRecord( Stream stream, out int itemSize )
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
    public  FileSharingRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usRecommendReadOnly = provider.ReadUInt16( iOffset + 0 );
      m_usHashPassword = provider.ReadUInt16( iOffset + 2 );
      int iFullLength;
      m_strCreatorName = provider.ReadString16Bit( iOffset + 4, out iFullLength );
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
      provider.WriteUInt16( iOffset + 0, m_usRecommendReadOnly );
      provider.WriteUInt16( iOffset + 2, m_usHashPassword );
      m_iLength = 4;

      m_iLength += provider.WriteString16Bit( iOffset + 4, m_strCreatorName );
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iCreatorLen = ( m_strCreatorName != null ) ?
        m_strCreatorName.Length :
        0;

      return 4 + ( ( iCreatorLen > 0 ) ?
        3 + iCreatorLen * 2 :
        2 );
    }
    #endregion
  }
}
