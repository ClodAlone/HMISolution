#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;
using System.Text;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record represents a cell that contains a string.
  /// In BIFF8, it is replaced by the LABELSST record.
  /// Nevertheless, Excel can import a LABEL record contained in a BIFF8 file.
  /// </summary>
  [ Biff( TBIFFRecord.Label ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class LabelRecord :
    CellPositionBase,
    IStringValue,
    IValueHolder
  {
    #region Class constants
    /// <summary>
    /// Size of fixed part. 6 bytes - row, column and xf index, 2 bytes string len, 1 byte string type.
    /// </summary>
    private const int DEF_FIXED_PART = 9;
    #endregion

    #region Class members
    /// <summary>
    /// Label - Unicode string
    /// </summary>
//    [ BiffRecordPos( 6, TFieldType.String ) ]
    private string m_strLabel = string.Empty;
    #endregion

    #region Class properties
    /// <summary>
    /// Label - Unicode string
    /// </summary>
    public string Label
    {
      get
      {
        return m_strLabel;
      }
      set
      {
        m_strLabel = value;
      }
    }

    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 8;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  LabelRecord()
      : base()
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
    /// <param name="version">Excel version used to fill data.</param>
    protected override void ParseCellData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      int iFullLen;
      m_strLabel = provider.ReadString16Bit( iOffset, out iFullLen );
    }
    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="version">Excel version used to fill data.</param>
    protected override void InfillCellData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = iOffset;
      provider.WriteString16BitUpdateOffset( ref m_iLength, m_strLabel );
      m_iLength -= iOffset;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      if( m_strLabel == null )
        m_strLabel = string.Empty;

      int iResult = DEF_FIXED_PART + Encoding.Unicode.GetByteCount( m_strLabel );

      if( version != ExcelVersion.Excel97to2003 )
        iResult += 4;

      return iResult;
    }
    #endregion

    #region IStringValue Members

    /// <summary>
    /// Returns string value. Read-only.
    /// </summary>
    string IStringValue.StringValue
    {
      get
      {
        return Label;
      }
    }

    #endregion

    #region IValueHolder Members
    /// <summary>
    /// Value of the record.
    /// </summary>
    public object Value
    {
      get
      {
        return Label;
      }
      set
      {
        Label = ( string )value;
      }
    }

    #endregion
  }
}
