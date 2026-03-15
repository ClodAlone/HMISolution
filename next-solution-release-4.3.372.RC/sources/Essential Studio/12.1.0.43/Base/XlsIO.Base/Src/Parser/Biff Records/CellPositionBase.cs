#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// This is parent class for all records that have cell position elements inside.
	/// </summary>
	public abstract class CellPositionBase
    : BiffRecordRaw
    , ICellPositionFormat
	{
    #region Class members
    /// <summary>
    /// Zero-based row index.
    /// </summary>
    protected int m_iRow;
    /// <summary>
    /// Zero-based column index.
    /// </summary>
    protected int m_iColumn;
    /// <summary>
    /// Index to the cell's extended format.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected ushort m_usExtendedFormat;
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor
    /// </summary>
		public CellPositionBase()
		{
    }
    #endregion

    #region ICellPositionFormat Members
    /// <summary>
    /// Row zero-based index.
    /// </summary>
    public int Row
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_iRow;
      }
      [ DebuggerStepThrough ]
      set
      {
        m_iRow = value;
      }
    }
    /// <summary>
    /// Column zero-based index.
    /// </summary>
    public int Column
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_iColumn;
      }
      [ DebuggerStepThrough ]
      set
      {
        m_iColumn = value;
      }
    }

    /// <summary>
    /// Index of extended format.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ushort ExtendedFormatIndex
    {
      get
      {
        return m_usExtendedFormat;
      }
      set
      {
        m_usExtendedFormat = value;
      }
    }

    #endregion

    #region Class methods
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
      switch( version )
      {
        case ExcelVersion.Excel97to2003:
          provider.WriteUInt16( iOffset, ( ushort )m_iRow );
          iOffset += ExcelConstants.ShortSize;

          provider.WriteInt16( iOffset, ( short )m_iColumn );
          iOffset += ExcelConstants.ShortSize;
          break;

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          provider.WriteInt32( iOffset, m_iRow );
          iOffset += ExcelConstants.IntSize;

          provider.WriteInt32( iOffset, m_iColumn );
          iOffset += ExcelConstants.IntSize;
          break;
      }

      provider.WriteUInt16( iOffset, m_usExtendedFormat );
      iOffset += ExcelConstants.ShortSize;

      InfillCellData( provider, iOffset, version );
      m_iLength = GetStoreSize( version );
    }
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
      switch( version )
      {
        case ExcelVersion.Excel97to2003:
          m_iRow = provider.ReadUInt16( iOffset );
          iOffset += ExcelConstants.ShortSize;

          m_iColumn = provider.ReadInt16( iOffset );
          iOffset += ExcelConstants.ShortSize;
          break;

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          m_iRow = provider.ReadInt32( iOffset );
          iOffset += ExcelConstants.IntSize;

          m_iColumn = provider.ReadInt32( iOffset );
          iOffset += ExcelConstants.IntSize;
          break;
      }

      m_usExtendedFormat = provider.ReadUInt16( iOffset );
      iOffset += ExcelConstants.ShortSize;

      ParseCellData( provider, iOffset, version );
    }
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="version">Excel version used to fill data.</param>
    protected abstract void ParseCellData( DataProvider provider, int iOffset, ExcelVersion version );
    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="version">Excel version used to fill data.</param>
    protected abstract void InfillCellData( DataProvider provider, int iOffset, ExcelVersion version );
    /// <summary>
    /// Returns size of the required storage space.
    /// </summary>
    /// <param name="version">Excel version.</param>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = base.GetStoreSize( version );

      if( version != ExcelVersion.Excel97to2003 )
        iResult += 4;

      return iResult;
    }
    #endregion
  }
}
