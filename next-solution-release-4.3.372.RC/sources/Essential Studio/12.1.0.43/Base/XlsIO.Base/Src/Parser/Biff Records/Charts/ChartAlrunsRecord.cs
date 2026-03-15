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
using Syncfusion.XlsIO.Interfaces;

#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// This record specifies rich-text formatting (bold, italic, font changes, 
  /// etc.) within chart titles and data labels.
  /// </summary>
  [ Biff( TBIFFRecord.ChartAlruns ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartAlrunsRecord : BiffRecordRaw
  {
    #region Class internal declarations
    /// <summary>
    /// 
    /// </summary>
    public class TRuns : ICloneable
    {
      /// <summary>
      /// Size of the TRun item.
      /// </summary>
      internal const int Size = 4;
      /// <summary>
      /// First char index.
      /// </summary>
      private ushort m_usFirstChar;
      /// <summary>
      /// Font index.
      /// </summary>
      private ushort m_usFontIndex;
      /// <summary>
      /// Gets or sets first char index.
      /// </summary>
      public ushort FirstCharIndex
      {
        get
        {
          return m_usFirstChar;
        }
        set
        {
          m_usFirstChar = value;
        }
      }
      /// <summary>
      /// Gets or sets font index.
      /// </summary>
      public ushort FontIndex
      {
        get
        {
          return m_usFontIndex;
        }
        set
        {
          m_usFontIndex = value;
        }
      }
      /// <summary>
      /// Creates new instance of TRuns.
      /// </summary>
      /// <param name="firstChar">Base first char.</param>
      /// <param name="fontIndex">Base font index.</param>
      public TRuns( ushort firstChar, ushort fontIndex )
      {
        m_usFirstChar = firstChar;
        m_usFontIndex = fontIndex;
      }
      /// <summary>
      /// Clones current object.
      /// </summary>
      /// <returns>Returns cloned object.</returns>
      public object Clone()
      {
        return MemberwiseClone();
      }
    }
    #endregion

    #region Class members
    /// <summary>
    /// Number of rich-text runs.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usQuantity;
    /// <summary>
    /// 
    /// </summary>
    private TRuns[] m_array = new TRuns[]{};
    #endregion

    #region Class properties
    /// <summary>
    /// Number of rich-text runs.
    /// </summary>
    public ushort Quantity
    {
      get
      {
        return m_usQuantity;
      }
      set
      {
        if( value != m_usQuantity )
        {
          m_usQuantity = value;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public TRuns[] Runs
    {
      get
      {
        return m_array;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_array = value;
        m_usQuantity = ( ushort )m_array.Length;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartAlrunsRecord()
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
    public  ChartAlrunsRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartAlrunsRecord( int iReserve )
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
      m_usQuantity = provider.ReadUInt16( iOffset + 0 );

      m_array = new TRuns[ m_usQuantity ];
      int offset = iOffset + 2;
      for( int i = 0; i < m_usQuantity; i++, offset += 4 )
      {
        m_array[ i ] = new TRuns( provider.ReadUInt16( offset ), provider.ReadUInt16( offset + 2 ) );
      }
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
      provider.WriteUInt16( iOffset + 0, m_usQuantity );
      
      int offset = iOffset + 2;
      for( int i = 0; i < m_usQuantity; i++, offset += 4 )
      {
        provider.WriteUInt16( offset, m_array[i].FirstCharIndex );
        provider.WriteUInt16( offset + 2, m_array[ i ].FontIndex );
      }

      m_iLength = offset;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return ExcelConstants.ShortSize + m_array.Length * TRuns.Size;
    }
    #endregion

    #region ICloneable method
    /// <summary>
    /// Clones current record.
    /// </summary>
    /// <returns>Returns cloned object.</returns>
    public override object Clone()
    {
      ChartAlrunsRecord result = ( ChartAlrunsRecord )base.Clone();

      if( m_array == null )
        return result;

      int iLen = m_array.Length;

      result.m_array = new TRuns[ iLen ];

      for( int i = 0; i < iLen; i++ )
      {
        result.m_array[ i ] = ( TRuns )CloneUtils.CloneCloneable( m_array[ i ] );
      }

      return result;
    }
    #endregion
  }
}
