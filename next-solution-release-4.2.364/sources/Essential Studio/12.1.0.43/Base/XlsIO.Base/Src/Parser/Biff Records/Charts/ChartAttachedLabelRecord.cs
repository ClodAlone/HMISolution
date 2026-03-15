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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// The ATTACHEDLABEL record defines the data label type. The ATTACHEDLABEL 
  /// record applies to the label data identified in the associated 
  /// DATAFORMAT record.
  /// </summary>
  [ Biff( TBIFFRecord.ChartAttachedLabel ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartAttachedLabelRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 2;
    /// <summary>
    /// Possible option flags
    /// </summary>
    [ Flags ]
    private enum OptionFlags
    {
      /// <summary>
      /// No option flag set.
      /// </summary>
      None = 0,
      /// <summary>
      /// Show the actual value of the data point.
      /// </summary>
      ActiveValue = 1,
      /// <summary>
      /// Show value as a percent of the total. This bit applies only to pie charts.
      /// </summary>
      PiePercents = 2,
      /// <summary>
      /// Show category label and value as a percentage (pie charts only).
      /// </summary>
      PieCategoryLabel = 4,
      /// <summary>
      /// Show smoothed line.
      /// </summary>
      SmoothLine = 8,
      /// <summary>
      /// Show category label.
      /// </summary>
      CategoryLabel = 16,
      /// <summary>
      /// Show bubble sizes.
      /// </summary>
      Bubble = 32,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Holder of all record flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private OptionFlags m_options;
    #endregion

    #region Class properties
    /// <summary>
    /// Holder of all record flags... changes of record setting does not
    /// influence on this property value till serialization.
    /// </summary>
    public ushort Options
    {
      get
      {
        return ( ushort )m_options;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ShowActiveValue
    {
      get
      {
        return ( m_options & OptionFlags.ActiveValue ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.ActiveValue;
        }
        else
        {
          m_options &= ~OptionFlags.ActiveValue;
        }
      }
    }
    /// <summary>
    /// Show value as a percent of the total. This bit applies only to pie charts.
    /// </summary>
    public bool ShowPieInPercents
    {
      get
      {
        return ( m_options & OptionFlags.PiePercents ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.PiePercents;
        }
        else
        {
          m_options &= ~OptionFlags.PiePercents;
        }
      }
    }

    /// <summary>
    /// Show category label and value as a percentage (pie charts only).
    /// </summary>
    public bool ShowPieCategoryLabel
    {
      get
      {
        return ( m_options & OptionFlags.PieCategoryLabel ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.PieCategoryLabel;
        }
        else
        {
          m_options &= ~OptionFlags.PieCategoryLabel;
        }
      }
    }

    /// <summary>
    /// Show smoothed line.
    /// </summary>
    public bool SmoothLine
    {
      get
      {
        return ( m_options & OptionFlags.SmoothLine ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.SmoothLine;
        }
        else
        {
          m_options &= ~OptionFlags.SmoothLine;
        }
      }
    }

    /// <summary>
    /// Show category label.
    /// </summary>
    public bool ShowCategoryLabel
    {
      get
      {
        return ( m_options & OptionFlags.CategoryLabel ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.CategoryLabel;
        }
        else
        {
          m_options &= ~OptionFlags.CategoryLabel;
        }
      }
    }

    /// <summary>
    /// Show bubble sizes.
    /// </summary>
    public bool ShowBubble
    {
      get
      {
        return ( m_options & OptionFlags.Bubble ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.Bubble;
        }
        else
        {
          m_options &= ~OptionFlags.Bubble;
        }
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartAttachedLabelRecord()
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
    public  ChartAttachedLabelRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartAttachedLabelRecord( int iReserve )
      : base( iReserve )
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
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      // TODO: check correctness of data

      //AutoExtractFields();
      m_options = ( OptionFlags )provider.ReadUInt16( iOffset );
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
      m_iLength = GetStoreSize( version );
      provider.WriteUInt16( iOffset, ( ushort )m_options );
    }
    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion
  }
}