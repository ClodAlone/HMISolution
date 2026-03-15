#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores two Windows country identifiers.
  /// The first represents the user interface language of the Excel version
  /// that saved this file, and second represents the system regional settings
  /// at the time the file was saved.
  /// </summary>
  [ Biff( TBIFFRecord.Country ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class CountryRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    private const int DefaultRecordSize = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Represents the user interface language of the Excel version
    /// that saved this file.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usDefault = 1;
    /// <summary>
    /// Represents the system regional settings
    /// at the time the file was saved.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usCurrent = 1;
    #endregion

    #region Class properties

    /// <summary>
    /// Represents the user interface language of the Excel version
    /// that saved this file.
    /// </summary>
    public ushort DefaultCountry
    {
      get
      {
        return m_usDefault;
      }
      set
      {
        m_usDefault = value;
      }
    }

    /// <summary>
    /// Represents the system regional settings
    /// at the time the file was saved.
    /// </summary>
    public ushort CurrentCountry
    {
      get
      {
        return m_usCurrent;
      }
      set
      {
        m_usCurrent = value;
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
        return 4;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    override public int MaximumRecordSize
    {
      get
      {
        return 4;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  CountryRecord()
      : base()
    {
    }

    /// <summary>
    /// Read/Initialize constructor
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  CountryRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  CountryRecord( int iReserve )
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
      m_usDefault = provider.ReadUInt16( iOffset + 0 );
      m_usCurrent = provider.ReadUInt16( iOffset + 2 );
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
      provider.WriteUInt16( iOffset + 0, m_usDefault );
      provider.WriteUInt16( iOffset + 2, m_usCurrent );
      m_iLength = DefaultRecordSize;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DefaultRecordSize;
    }
    #endregion
  }
}
