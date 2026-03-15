#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

#if  SILVERLIGHT || WINRT || WP
using Syncfusion.XlsIO.Interfaces;
#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record stores an array of variable-length SXLI structures, which
  ///  describe the row and column items in a PivotTable. There are two
  ///  LineItemArrayRecords for each PivotTable: the first stores row items,
  ///  and the second stores column items.
  /// </summary>
  [ Biff( TBIFFRecord.LineItemArray ) ]
  [ CLSCompliant( false ) ]
  public class LineItemArrayRecord : BiffRecordWithContinue//BiffRecordRawWithArray
  {
    #region Class members
    /// <summary>
    /// Array of LineItem structures.
    /// </summary>
    private List<LineItem> m_arrItems;
    /// <summary>
    /// Indicates whether internal data array should be preserved.
    /// </summary>
    private bool m_bNeedDataArray = true;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  LineItemArrayRecord()
      : base()
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Array of LineItem structures.
    /// </summary>
    public List<LineItem> Items
    {
      get
      {
        return m_arrItems;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_arrItems = value;
      }
    }
    /// <summary>
    /// Indicates whether record needs internal data array or if it can be cleaned.
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return m_bNeedDataArray;
      }
    }

    /// <summary>
    /// Indicates whether we should add header of continue records to the internal data provider. Read-only.
    /// </summary>
    protected override bool AddHeaderToProvider
    {
      get
      {
        return true;
      }
    }

    #endregion
    
    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      // Do nothing here. For correct parsing we should have information about fields count
    }

    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public void ParseStructure( int iFieldsCount )
    {
      // NOTE: We don't need to implement parsing since we don't support PivotTables creation on the current moment.
//      int iOffset = 0;
//      m_arrItems = new List<LineItem>();
//
//      while( iOffset < m_iLength )
//      //for( int i = 0; i < iFieldsCount; i++ )
//      {
//        LineItem item = new LineItem();
//        iOffset += item.Parse( m_data, iOffset, iFieldsCount );
//        m_arrItems.Add( item );
//      }
//
//      m_bNeedDataArray = false;
    }

    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iFirstLength = ( m_iLength > DEF_RECORD_MAX_SIZE ) ?
        DEF_RECORD_MAX_SIZE :
        -1;
      // NOTE: We don't need to implement infill since we don't support PivotTables creation on the current moment.
//      if( m_bNeedDataArray ) return;
//
//      m_iLength = AutoInfillFromFields();
//
//      int iOffset = 0;
//      m_iLength = 0;
//      int iCount = m_arrItems.Count;
//      
//      for( int i = 0; i < iCount; i++ )
//      {
//        m_iLength += ( ( LineItem )m_arrItems[ i ] ).Length;
//      }
//
//      // Ensure size.
//      AutoGrowData = true;
//      SetByte( m_iLength - 1, 0 );
//
//      for( int i = 0; i < iCount; i++ )
//      {
//        LineItem item = m_arrItems[ i ];
//        iOffset += item.Serialize( m_data, iOffset );
//      }
    }
    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return m_iLength;
    }

    public override object Clone()
    {
      LineItemArrayRecord result = ( LineItemArrayRecord )base.Clone();
      result.m_arrItems = CloneUtils.CloneCloneable<LineItem>( m_arrItems );
      return result;
    }

    #endregion
  }

  /// <summary>
  /// LineItem class.
  /// </summary>
  [ CLSCompliant( false ) ]
  public class LineItem : ICloneable
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXEDPART_SIZE = 8;
    /// <summary>
    /// Bit for IsMultiDataName property.
    /// </summary>
    private const int DEF_BIT_MULTIDATANAME = 0;
    /// <summary>
    /// Bit for IsSubtotal property.
    /// </summary>
    private const int DEF_BIT_SUBTOTAL = 9;
    /// <summary>
    /// Bit for IsBlock property.
    /// </summary>
    private const int DEF_BIT_BLOCK = 10;
    /// <summary>
    /// Bit for IsGrand property.
    /// </summary>
    private const int DEF_BIT_GRAND = 11;
    /// <summary>
    /// Bit for IsMultiDataOnAxis property.
    /// </summary>
    private const int DEF_BIT_MULTIDATAONAXIS = 12;
    /// <summary>
    /// Item type.
    /// </summary>
    public enum LineItemType
    {
      /// <summary>
      /// Represents the Data line item type.
      /// </summary>
      Data,
      /// <summary>
      /// Represents the Default line item type.
      /// </summary>
      Default,
      /// <summary>
      /// Represents the Sum line item type.
      /// </summary>
      Sum,
      /// <summary>
      /// Represents the CountA line item type.
      /// </summary>
      CountA,
      /// <summary>
      /// Represents the Count line item type.
      /// </summary>
      Count,
      /// <summary>
      /// Represents the Average line item type.
      /// </summary>
      Average,
      /// <summary>
      /// Represents the Max line item type.
      /// </summary>
      Max,
      /// <summary>
      /// Represents the Min line item type.
      /// </summary>
      Min,
      /// <summary>
      /// Represents the Product line item type.
      /// </summary>
      Product,
      /// <summary>
      /// Represents the Stdev line item type.
      /// </summary>
      Stdev,
      /// <summary>
      /// Represents the StdevP line item type.
      /// </summary>
      StdevP,
      /// <summary>
      /// Represents the Var line item type.
      /// </summary>
      Var,
      /// <summary>
      /// Represents the VarP line item type.
      /// </summary>
      VarP,
      /// <summary>
      /// Represents the GrandTotal line item type.
      /// </summary>
      GrandTotal,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Count of items that are identical to the previous element in items array.
    /// </summary>
    private ushort m_usIdenticalItemsCount;
    /// <summary>
    /// Item type.
    /// </summary>
    private ushort m_usItemType;
    /// <summary>
    /// Maximum index to the items array.
    /// </summary>
    private ushort m_usMaxIndex;
    /// <summary>
    /// Option flags.
    /// </summary>
    private ushort m_usOptions;
    /// <summary>
    /// Array of indexes to SXVI records; the number of elements in the array is m_usMaxType + 1;
    /// </summary>
    private ushort[] m_arrIndexes;
    #endregion

    #region Class properties
    /// <summary>
    /// Count of items that are identical to the previous element in items array.
    /// </summary>
    public ushort IdenticalItemsCount
    {
      get
      {
        return m_usIdenticalItemsCount;
      }
      set
      {
        m_usIdenticalItemsCount = value;
      }
    }
    /// <summary>
    /// Item type.
    /// </summary>
    public ushort ItemType
    {
      get
      {
        return m_usItemType;
      }
      set
      {
        m_usItemType = value;
      }
    }
    /// <summary>
    /// Maximum index to the items array. Read-only.
    /// </summary>
    public ushort MaxIndex
    {
      get
      {
        return m_usMaxIndex;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
#if DEBUG
      set
      {
        m_usOptions = value;
      }
#endif
    }
    /// <summary>
    /// Array of indexes to SXVI records.
    /// </summary>
    public ushort[] Indexes
    {
      get
      {
        return m_arrIndexes;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_arrIndexes = value;
        m_usMaxIndex = ( ushort )( m_arrIndexes.Length - 1 );
      }
    }
    /// <summary>
    /// Indicates whether to use data field name for the subtotal (instead of using "Total").
    /// </summary>
    public bool IsMultiDataName
    {
      get
      {
        return BiffRecordRaw.GetBitFromVar( m_usOptions, DEF_BIT_MULTIDATANAME );
      }
      set
      {
        m_usOptions = ( ushort )BiffRecordRaw.SetBit( m_usOptions, DEF_BIT_MULTIDATANAME, value );
      }
    }
    /// <summary>
    /// Indicates whether this item is a subtotal.
    /// </summary>
    public bool IsSubtotal
    {
      get
      {
        return BiffRecordRaw.GetBitFromVar( m_usOptions, DEF_BIT_SUBTOTAL );
      }
      set
      {
        m_usOptions = ( ushort ) BiffRecordRaw.SetBit( m_usOptions, DEF_BIT_SUBTOTAL, value );
      }
    }
    /// <summary>
    /// Indicates whether this item is a block total.
    /// </summary>
    public bool IsBlock
    {
      get
      {
        return BiffRecordRaw.GetBitFromVar( m_usOptions, DEF_BIT_BLOCK );
      }
      set
      {
        m_usOptions = ( ushort ) BiffRecordRaw.SetBit( m_usOptions, DEF_BIT_BLOCK, value );
      }
    }
    /// <summary>
    /// Indicates whether this item is a grand total.
    /// </summary>
    public bool IsGrand
    {
      get
      {
        return BiffRecordRaw.GetBitFromVar( m_usOptions, DEF_BIT_GRAND );
      }
      set
      {
        m_usOptions = ( ushort ) BiffRecordRaw.SetBit( m_usOptions, DEF_BIT_GRAND, value );
      }
    }
    /// <summary>
    /// Indicates whether 
    /// </summary>
    public bool IsMultiDataOnAxis
    {
      get
      {
        return BiffRecordRaw.GetBitFromVar( m_usOptions, DEF_BIT_MULTIDATAONAXIS );
      }
      set
      {
        m_usOptions = ( ushort ) BiffRecordRaw.SetBit( m_usOptions, DEF_BIT_MULTIDATAONAXIS, value );
      }
    }
    /// <summary>
    /// Size of the record. Read-only.
    /// </summary>
    public int Length
    {
      get
      {
        return DEF_FIXEDPART_SIZE + m_arrIndexes.Length * 2;
      }
    }
    #endregion

    #region Record serialization
    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="iOffset"></param>
    /// <param name="iFieldsCount"></param>
    /// <returns></returns>
    public int Parse( DataProvider provider, int iOffset, int iFieldsCount )
    {
      m_usIdenticalItemsCount = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usItemType = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usMaxIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usOptions = provider.ReadUInt16( iOffset );
      iOffset += 2;

      //int iCount = m_usMaxIndex + 1;
      int iCount = iFieldsCount;

      m_arrIndexes = new ushort[ iCount ];

      //Buffer.BlockCopy( arrData, iOffset, m_arrIndexes, 0, iCount * 2 );
      //provider.Rea
      for( int i = 0; i < iCount; i++, iOffset += 2 )
      {
        m_arrIndexes[ i ] = provider.ReadUInt16( iOffset );
      }

      return iCount * 2 + DEF_FIXEDPART_SIZE;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrData"></param>
    /// <param name="iOffset"></param>
    /// <returns></returns>
    public int Serialize( byte[] arrData, int iOffset )
    {
      BiffRecordRaw.SetUInt16( arrData, iOffset, m_usIdenticalItemsCount );
      iOffset += 2;

      BiffRecordRaw.SetUInt16( arrData, iOffset, m_usItemType );
      iOffset += 2;

      BiffRecordRaw.SetUInt16( arrData, iOffset, m_usMaxIndex );
      iOffset += 2;

      BiffRecordRaw.SetUInt16( arrData, iOffset, m_usOptions );
      iOffset += 2;

      int iCount = m_arrIndexes.Length;

      if( iCount > 0 )
      {
        Buffer.BlockCopy( m_arrIndexes, 0, arrData, iOffset, iCount * 2 );
      }

      return iCount * 2 + DEF_FIXEDPART_SIZE;
    }
    #endregion


    #region ICloneable Members

    public object Clone()
    {
      LineItem result = ( LineItem )MemberwiseClone();
      result.m_arrIndexes = CloneUtils.CloneUshortArray( m_arrIndexes );
      return result;
    }

    #endregion
  }
}
