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
  /// Format description for the Chart.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ Biff( TBIFFRecord.ChartFormat ) ]
  public class ChartFormatRecord : BiffRecordRawWithArray
  {
    #region Class members

    [ BiffRecordPos( 0, 4, true ) ]
    private int m_iXPos = 0;

    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iYPos = 0;

    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iWidth = 0;

    [ BiffRecordPos( 12, 4, true ) ]
    private int m_iHeight = 0;

    [ BiffRecordPos( 16, 2 ) ]
    private ushort m_usGrBit = 0;

    #endregion

    #region Class properties

    /// <summary>
    /// Chart X position.
    /// </summary>
    public int X
    {
      get
      {
        return m_iXPos;
      }
      set
      {
        m_iXPos = value;
      }
    }

    /// <summary>
    /// Chart Y position.
    /// </summary>
    public int Y
    {
      get
      {
        return m_iYPos;
      }
      set
      {
        m_iYPos = value;
      }
    }

    /// <summary>
    /// Chart width.
    /// </summary>
    public int Width
    {
      get
      {
        return m_iWidth;
      }
      set
      {
        m_iWidth = value;
      }
    }

    /// <summary>
    /// Chart height.
    /// </summary>
    public int Height
    {
      get
      {
        return m_iHeight;
      }
      set
      {
        m_iHeight = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ushort GrBit
    {
      get
      {
        return m_usGrBit;
      }
      set
      {
        m_usGrBit = value;
      }
    }

    /// <summary>
    /// Returns minimum possible size of record's internal data array.
    /// </summary>
    override public int MinimumRecordSize
    {
      get
      {
        return 18;
      }
    }

    /// <summary>
    /// Returns maximum possible size of record's internal data array.
    /// </summary>
    override public int MaximumRecordSize
    {
      get
      {
        return 20;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default Constructor
    /// </summary>
    public  ChartFormatRecord()
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
    public  ChartFormatRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserves for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartFormatRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization

    /// <summary>
    /// Parse structure of record. Convert data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      AutoExtractFields();
    }

    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iLength = AutoInfillFromFields();
    }

    #endregion
  }
}
