#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// This record saves printer settings and printer driver information.
	/// </summary>
  [ Biff( TBIFFRecord.PrinterSettings ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class PrinterSettingsRecord : BiffRecordWithContinue
	{
    #region Class properties
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

    /// <summary>
    /// Indicates whether record needs internal data array
    /// or if it can be cleaned. Read-only.
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }

    #endregion

    #region Class initialize / finilize methods
    /// <summary>
    /// Default constructor
    /// </summary>
		public PrinterSettingsRecord()
		{
		}
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// When string's length does not fit to internal data length or
    /// when last string ends before data (some extra data at the
    /// end of m_data array).
    /// </exception>
    public override void ParseStructure()
    {
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iFirstLength = ( m_iLength > DEF_RECORD_MAX_SIZE ) ?
        DEF_RECORD_MAX_SIZE :
        -1;
    }

    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public override object Clone()
    {
      PrinterSettingsRecord result = ( PrinterSettingsRecord )base.Clone();

      if( m_provider != null && !m_provider.IsCleared )
      {
        result.m_provider.EnsureCapacity( m_iLength );
        m_provider.CopyTo( 0, result.m_provider, 0, m_iLength );
      }

      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return m_iLength;
    }
    #endregion
	}
}
