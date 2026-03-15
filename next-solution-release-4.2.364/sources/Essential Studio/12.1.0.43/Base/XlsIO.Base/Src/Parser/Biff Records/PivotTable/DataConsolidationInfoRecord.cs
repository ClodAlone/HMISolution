#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record stores options from the Consolidate dialog box (Data menu).
  /// </summary>
  [ Biff( TBIFFRecord.DCON ) ]
  [ CLSCompliant( false ) ]
  public class DataConsolidationInfoRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Function types.
    /// </summary>
    public enum FunctionTypes
    {
      /// <summary>
      /// Represents the Average function type.
      /// </summary>
      Average   = 0,
      /// <summary>
      /// Represents the CountNums function type.
      /// </summary>
      CountNums = 1,
      /// <summary>
      /// Represents the Count function type.
      /// </summary>
      Count     = 2,
      /// <summary>
      /// Represents the Max function type.
      /// </summary>
      Max       = 3,
      /// <summary>
      /// Represents the Min function type.
      /// </summary>
      Min       = 4,
      /// <summary>
      /// Represents the Product function type.
      /// </summary>
      Product   = 5,
      /// <summary>
      /// Represents the StdDev function type.
      /// </summary>
      StdDev    = 6,
      /// <summary>
      /// Represents the StdDevp function type.
      /// </summary>
      StdDevp   = 7,
      /// <summary>
      /// Represents the Sum function type.
      /// </summary>
      Sum       = 8,
      /// <summary>
      /// Represents the Var function type.
      /// </summary>
      Var       = 9,
      /// <summary>
      /// Represents the Varp function type.
      /// </summary>
      Varp      = 10,
    }
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Index to the data consolidation function.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usFunctionIndex;
    /// <summary>
    /// If it is equal to 1 then the Left Column option is turned on.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usLeftCat;
    /// <summary>
    /// If it is equal to 1 then the Top Row option is turned on.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usTopCat;
    /// <summary>
    /// If it is equal to 1 then the Create Links To Source Data option is turned on.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usLinkConsol;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  DataConsolidationInfoRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  DataConsolidationInfoRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  DataConsolidationInfoRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Index to the data consolidation function.
    /// </summary>
    public FunctionTypes FunctionIndex
    {
      get
      {
        return ( FunctionTypes )m_usFunctionIndex;
      }
      set
      {
        m_usFunctionIndex = ( ushort )value;
      }
    }
    /// <summary>
    /// If it is equal to True then the Left Column option is turned on.
    /// </summary>
    public bool IsLeftCat
    {
      get
      {
        return ( m_usLeftCat == 1 );
      }
      set
      {
        m_usLeftCat = ( ushort )( value ? 1 : 0 );
      }
    }
    /// <summary>
    /// If it is equal to True then the Top Row option is turned on.
    /// </summary>
    public bool IsTopCat
    {
      get
      {
        return m_usTopCat == 1;
      }
      set
      {
        m_usTopCat = ( ushort )( value ? 1 : 0 );
      }
    }
    /// <summary>
    /// If it is equal to 1 then the Create Links To Source Data option is turned on.
    /// </summary>
    public bool IsLinkConsol
    {
      get
      {
        return m_usLinkConsol == 1;
      }
      set
      {
        m_usLinkConsol = ( ushort )( value ? 1 : 0 );
      }
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
      m_usFunctionIndex = provider.ReadUInt16( iOffset + 0 );
      m_usLeftCat = provider.ReadUInt16( iOffset + 2 );
      m_usTopCat = provider.ReadUInt16( iOffset + 4 );
      m_usLinkConsol = provider.ReadUInt16( iOffset + 6 );
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      provider.WriteUInt16( iOffset + 0, m_usFunctionIndex );
      provider.WriteUInt16( iOffset + 2, m_usLeftCat );
      provider.WriteUInt16( iOffset + 4, m_usTopCat );
      provider.WriteUInt16( iOffset + 6, m_usLinkConsol );
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
