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
  /// Row height for rows with undefined or inexplicitly defined heights.
  /// </summary>
  [ Biff( TBIFFRecord.DefaultRowHeight ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class DefaultRowHeightRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 4;
    #endregion

    #region Class members

    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptionFlags = 0;

    ///<summary>
    /// Default row height for undefined rows / rows with undefined height.
    ///</summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usRowHeigth = 255;

    /// <summary>
    /// Specifies whether the default settings for the row height have been changed..
    /// </summary>
    private bool m_customHeight;
    #endregion

    #region Class properties

    /// <summary>
    /// Option flags.
    /// </summary>
    public ushort OptionFlags
    {
      get
      {
        return m_usOptionFlags;
      }
      set
      {
        m_usOptionFlags = value;
      }
    }

    ///<summary>
    /// Default row height for undefined rows / rows with undefined height.
    ///</summary>
    public ushort Height
    {
      get
      {
        return m_usRowHeigth;
      }
      set
      {
        m_usRowHeigth = value;
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
    /// <summary>
    /// Returns a boolean value which specifies whether the default settings for the row height have been changed.
    /// </summary>
    internal bool CustomHeight
    {
        get
        {
            return m_customHeight;
        }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  DefaultRowHeightRecord()
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
    public  DefaultRowHeightRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  DefaultRowHeightRecord( int iReserve )
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
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usOptionFlags = provider.ReadUInt16( iOffset );
      m_customHeight = provider.ReadBit(iOffset, 0);
      iOffset += 2;

      m_usRowHeigth = provider.ReadUInt16( 2 );
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

      provider.WriteUInt16( iOffset, m_usOptionFlags );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usRowHeigth );
    }

    #endregion
  }
}
