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

using Syncfusion.XlsIO.Implementation;
using System.Collections.Generic;

#if WINRT
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#elif WINRT
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// The begin record defines the start of a block of records for a (Graphing)
  /// data object. This record is matched with a corresponding EndRecord.
  /// </summary>
  [ Biff( TBIFFRecord.CondFMT ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class CondFMTRecord :
    BiffRecordRaw,
    ICloneable
  {
    #region Class constants
    /// <summary>
    /// Minimum size of the record.
    /// </summary>
    private const ushort DEF_MINIMUM_RECORD_SIZE = 14;
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_SIZE = 14;
    /// <summary>
    /// Subitem size.
    /// </summary>
    private const int DEF_SUB_ITEM_SIZE = 8;
    /// <summary>
    /// Need to redraw the conditional format ON status.
    /// </summary>
    private const ushort DEF_REDRAW_ON = 1;
    /// <summary>
    /// Need to redraw the conditional format OFF status.
    /// </summary>
    private const ushort DEF_REDRAW_OFF = 0;
    #endregion

    #region Class members
    /// <summary>
    /// Number of following CF records.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usCFNumber;

    /// <summary>
    /// 1 = Conditionally formatted cells need recalculation or redraw.
    /// </summary>
    [BiffRecordPos(2, 0, TFieldType.Bit)]
    private bool m_usNeedRecalc = false;

    /// <summary>
    /// Index of the record.
    /// </summary>
    private ushort m_index;

    /// <summary>
    /// Cell range address of the range enclosing all
    /// conditionally formatted ranges.
    /// </summary>
    private TAddr m_addrEncloseRange = new TAddr();

    /// <summary>
    /// Number of conditionally formatted cells.
    /// </summary>
    private ushort m_usCellsCount;

    /// <summary>
    /// Cell range address list of all conditionally formatted ranges.
    /// </summary>
    private List<Rectangle> m_arrCells = new List<Rectangle>();
    /// <summary>
    /// Check whether the rule parsed.
    /// </summary>
    private bool m_isparsed = false;
    #endregion

    #region Class properties
    /// <summary>
    /// Number of following CF records.
    /// </summary>
    public ushort   CFNumber
    {
      get
      {
        return m_usCFNumber;
      }
      set
      {
        m_usCFNumber = value;
      }
    }

    /// <summary>
    /// 1 = Conditionally formatted cells need recalculation or redraw.
    /// </summary>
    public bool     NeedRecalc
    {
      get
      {
        return ( m_usNeedRecalc == false );
      }
      set
      {
        m_usNeedRecalc = ( value ? true : false );
      }
    }

    /// <summary>
    /// Index of this record.
    /// </summary>
    public ushort Index
    {
        get
        {
            return m_index;
        }
        set
        {
            m_index = value;
        }
    }

    /// <summary>
    /// Cell range address of the range enclosing all
    /// conditionally formatted ranges.
    /// </summary>
    public TAddr    EncloseRange
    {
      get
      {
        return m_addrEncloseRange;
      }
      set
      {
        m_addrEncloseRange = value;
      }
    }

    /// <summary>
    /// Number of conditionally formatted cells. Read-only.
    /// </summary>
    public ushort   CellsCount
    {
      get
      {
        return m_usCellsCount;
      }
      set
      {
          m_usCellsCount = (ushort)m_arrCells.Count;
      }
    }

    /// <summary>
    /// Returns list with all conditionally formatted ranges. Read-only.
    /// </summary>
    public List<Rectangle> CellList
    {
      get
      {
        return m_arrCells;
      }
      internal set
      {
        m_arrCells = value;
      }
    }
    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_MINIMUM_RECORD_SIZE;
      }
    }
    /// <summary>
    /// Check whether the rule is parsed or not.
    /// </summary>
    public bool IsParsed
    {
        get
        {
            return m_isparsed;
        }
        set
        {
            m_isparsed = value;
        }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  CondFMTRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  CondFMTRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserves for record's internal data array iReserve bytes.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  CondFMTRecord( int iReserve )
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
      m_isparsed = true;

      m_usCFNumber = provider.ReadUInt16( iOffset );
      iOffset += ExcelConstants.ShortSize;

      ushort value = provider.ReadUInt16(iOffset);
      m_usNeedRecalc = ((value & (1 << 0)) == (1 << 0));
      m_index = (ushort)(value >> 1);
      iOffset += ExcelConstants.ShortSize;

      m_addrEncloseRange = provider.ReadAddr( iOffset );
      iOffset += 8;

      ExtractCellsList( provider, ref iOffset );
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
      m_usCellsCount = ( ushort )m_arrCells.Count;
      m_iLength = GetStoreSize( version );
      
      provider.WriteUInt16( iOffset, m_usCFNumber );
      iOffset += 2;

      int value = m_usNeedRecalc ? ((m_index << 1) | 1) : ((m_index << 1) | 0);
      provider.WriteUInt16(iOffset, (ushort)value);
      iOffset += 2;

      provider.WriteAddr( iOffset, m_addrEncloseRange );
      iOffset += 8;
      
      provider.WriteUInt16( iOffset, m_usCellsCount );
      iOffset += ExcelConstants.ShortSize;

      for( int i = 0; i < m_usCellsCount; i++, iOffset += 8 )
      {
        Rectangle addr = m_arrCells[ i ];
        provider.WriteAddr( iOffset, addr );
      }
    }
    /// <summary>
    /// Extracts list of cells from the internal data array.
    /// </summary>
    /// <param name="provider">Data provider to extract cell list from.</param>
    /// <param name="offset">Position of the list in the internal data array.</param>
    private void ExtractCellsList( DataProvider provider, ref int offset )
    {
      m_usCellsCount = provider.ReadUInt16( offset );
      offset += 2;

      for( int i = 0; i < m_usCellsCount; i++, offset += 8 )
      {
        Rectangle newRange = provider.ReadAddrAsRectangle( offset );
        m_arrCells.Add( newRange );
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_FIXED_SIZE + m_arrCells.Count * DEF_SUB_ITEM_SIZE;
    }
    #endregion

    #region Class Helper Methods
//    /// <summary>
//    /// Adds cell to the cells list.
//    /// </summary>
//    /// <param name="addr">Cell to add to the list.</param>
//    public void AddCell( TAddr addr )
//    {
//      m_arrCells.Add( addr.GetRectangle() );
//      m_usCellsCount++;
//    }
    /// <summary>
    /// Adds cell to the cells list.
    /// </summary>
    /// <param name="addr">Cell to add to the list.</param>
    public void AddCell( Rectangle addr )
    {
      m_arrCells.Add( addr );
      m_usCellsCount++;
    }
//    /// <summary>
//    /// Clones CondFMTRecord.
//    /// </summary>
//    /// <param name="addr">Base TAddr.</param>
//    /// <returns>Returns new instance.</returns>
//    public CondFMTRecord Clone( TAddr addr )
//    {
//      CondFMTRecord result = ( CondFMTRecord )base.Clone();
//
//      result.m_arrCells = new List<Rectangle>( m_arrCells.Count );
//
//      result.EncloseRange = addr;
//      result.AddCell( addr.GetRectangle() );
//
//      return result;
//    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Clone current record.
    /// </summary>
    /// <returns>Returns clone of the current object.</returns>
    public override object Clone()
    {
      CondFMTRecord result = ( CondFMTRecord )base.Clone();

      result.m_arrCells = new List<Rectangle>( m_arrCells.Count );

      for( int i = 0, iLen = m_arrCells.Count; i < iLen; i++ )
      {
        result.AddCell( m_arrCells[ i ] );
      }

      return result;
    }
    #endregion
  }
}
