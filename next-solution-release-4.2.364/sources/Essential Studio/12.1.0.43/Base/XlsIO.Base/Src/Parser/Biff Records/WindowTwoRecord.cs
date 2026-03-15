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

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Sheet window settings.
  /// </summary>
  [ Biff( TBIFFRecord.WindowTwo ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class WindowTwoRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Maximum record size.
    /// </summary>
    private const int DEF_MAX_RECORD_SIZE = 18;
    /// <summary>
    /// Indicates the size when this record is in Chart Sheet.
    /// </summary>
    internal const int DEF_MAX_CHART_SHEET_SIZE = 10;
    /// <summary>
    /// Possible option flags.
    /// </summary>
    [ Flags ]
    private enum OptionFlags : ushort 
    {
      /// <summary>
      /// Indicates whether the window should display formulas.
      /// </summary>
      DisplayFormulas = 1,
      /// <summary>
      /// Indicates whether the window should display gridlines.
      /// </summary>
      DisplayGridlines = 2,
      /// <summary>
      /// Indicates whether the window should display row and column headings.
      /// </summary>
      DisplayRowColHeadings = 4,
      /// <summary>
      /// Indicates whether the window should freeze panes.
      /// </summary>
      FreezePanes = 8,
      /// <summary>
      /// Indicates whether the window should display zero values.
      /// </summary>
      DisplayZeros = 16,
      /// <summary>
      /// Indicates whether the window should display a default header.
      /// </summary>
      DefaultHeader= 32,
      /// <summary>
      /// Indicates whether this is Arabic.
      /// </summary>
      Arabic = 64,
      /// <summary>
      /// Indicates whether the outline symbols are displayed.
      /// </summary>
      DisplayGuts = 128,
      /// <summary>
      /// Indicates whether freeze panes are unsplit or not.
      /// </summary>
      FreezePanesNoSplit = 256,
      /// <summary>
      /// Indicates whether sheet tab is selected.
      /// </summary>
      Selected = 512,
      /// <summary>
      /// Indicates whether sheet is currently displayed in the window.
      /// </summary>
      Paged = 1024,
      /// <summary>
      /// Indicates whether sheet was saved in page break view.
      /// </summary>
      SavedInPageBreakPreview = 2048,
    }
    #endregion

    #region Class members
    /// <summary>
    ///  The option's bitmask (you should use the bit setters).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private OptionFlags  m_options = ( OptionFlags )182;//1718;
    /// <summary>
    /// The top row visible in the window.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort  m_usTopRow = 0;
    /// <summary>
    /// The leftmost column displayed in the window.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort  m_usLeftCol = 0;
    /// <summary>
    /// The palette index for the header color.
    /// </summary>
    [ BiffRecordPos( 6, 4, true ) ]
    private int     m_iHeaderColor = 64;
    /// <summary>
    /// Zoom magification in page break view.
    /// </summary>
    private ushort  m_usPageBreakZoom;
    /// <summary>
    /// The zoom magnification in normal view.
    /// </summary>
    private ushort  m_usNormalZoom;
    /// <summary>
    /// Reserved.
    /// </summary>
    private int     m_iReserved;
    /// <summary>
    /// Length of the original record.
    /// </summary>
    private int     m_iOriginalLength;
    #endregion

    #region Class properties
    /// <summary>
    /// The top row visible in the window.
    /// </summary>
    public ushort TopRow
    {
      get
      {
        return m_usTopRow;
      }
      set
      {
        m_usTopRow = value;
      }
    }

    /// <summary>
    /// The leftmost column displayed in the window.
    /// </summary>
    public ushort LeftColumn
    {
      get
      {
        return m_usLeftCol;
      }
      set
      {
        m_usLeftCol = value;
      }
    }

    /// <summary>
    /// The palette index for the header color.
    /// </summary>
    public int    HeaderColor
    {
      get
      {
        return m_iHeaderColor;
      }
      set
      {
        m_iHeaderColor = value;
      }
    }

    /// <summary>
    /// Whether the window should display formulas.
    /// </summary>
    public bool   IsDisplayFormulas
    {
      get
      {
        return ( m_options & OptionFlags.DisplayFormulas ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DisplayFormulas;
        }
        else
        {
          m_options &= ~OptionFlags.DisplayFormulas;
        }
      }
    }

    /// <summary>
    /// Whether the window should display gridlines.
    /// </summary>
    public bool   IsDisplayGridlines
    {
      get
      {
        return ( m_options & OptionFlags.DisplayGridlines ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DisplayGridlines;
        }
        else
        {
          m_options &= ~OptionFlags.DisplayGridlines;
        }
      }
    }

    /// <summary>
    /// Whether the window should display row and column headings.
    /// </summary>
    public bool   IsDisplayRowColHeadings
    {
      get
      {
        return ( m_options & OptionFlags.DisplayRowColHeadings ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DisplayRowColHeadings;
        }
        else
        {
          m_options &= ~OptionFlags.DisplayRowColHeadings;
        }
      }
    }

    /// <summary>
    /// Whether the window should freeze panes.
    /// </summary>
    public bool   IsFreezePanes
    {
      get
      {
        return ( m_options & OptionFlags.FreezePanes ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.FreezePanes;
        }
        else
        {
          m_options &= ~OptionFlags.FreezePanes;
        }
      }
    }

    /// <summary>
    /// Whether the window should display zero values.
    /// </summary>
    public bool   IsDisplayZeros
    {
      get
      {
        return ( m_options & OptionFlags.DisplayZeros ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DisplayZeros;
        }
        else
        {
          m_options &= ~OptionFlags.DisplayZeros;
        }
      }
    }

    /// <summary>
    /// Whether the window should display a default header.
    /// </summary>
    public bool   IsDefaultHeader
    {
      get
      {
        return ( m_options & OptionFlags.DefaultHeader ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DefaultHeader;
        }
        else
        {
          m_options &= ~OptionFlags.DefaultHeader;
        }
      }
    }

    /// <summary>
    /// Is this Arabic?
    /// </summary>
    public bool   IsArabic
    {
      get
      {
        return ( m_options & OptionFlags.Arabic ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.Arabic;
        }
        else
        {
          m_options &= ~OptionFlags.Arabic;
        }
      }
    }

    /// <summary>
    /// Whether the outline symbols are displayed.
    /// </summary>
    public bool   IsDisplayGuts
    {
      get
      {
        return ( m_options & OptionFlags.DisplayGuts ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DisplayGuts;
        }
        else
        {
          m_options &= ~OptionFlags.DisplayGuts;
        }
      }
    }

    /// <summary>
    /// Freeze unsplit panes or not.
    /// </summary>
    public bool   IsFreezePanesNoSplit
    {
      get
      {
        return ( m_options & OptionFlags.FreezePanesNoSplit ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.FreezePanesNoSplit;
        }
        else
        {
          m_options &= ~OptionFlags.FreezePanesNoSplit;
        }
      }
    }

    /// <summary>
    /// Sheet tab is selected.
    /// </summary>
    public bool   IsSelected
    {
      get
      {
        return ( m_options & OptionFlags.Selected ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.Selected;
        }
        else
        {
          m_options &= ~OptionFlags.Selected;
        }
      }
    }

    /// <summary>
    /// Is the sheet currently displayed in the window?
    /// </summary>
    public bool   IsPaged
    {
      get
      {
        return ( m_options & OptionFlags.Paged ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.Paged;
        }
        else
        {
          m_options &= ~OptionFlags.Paged;
        }
      }
    }

    /// <summary>
    /// Was the sheet saved in page break view?
    /// </summary>
    public bool   IsSavedInPageBreakPreview
    {
      get
      {
        return ( m_options & OptionFlags.SavedInPageBreakPreview ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.SavedInPageBreakPreview;
        }
        else
        {
          m_options &= ~OptionFlags.SavedInPageBreakPreview;
        }
      }
    }
    /// <summary>
    /// Read-only. Option flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return ( ushort )m_options;
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
        return 10;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_MAX_RECORD_SIZE;
      }
    }
    /// <summary>
    /// Length of the original record.
    /// </summary>
    internal int OriginalLength
    {
        get
        {
            return m_iOriginalLength;
        }
        set
        {
            m_iOriginalLength = value;
        }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  WindowTwoRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  WindowTwoRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  WindowTwoRecord( int iReserve )
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
      m_options = ( OptionFlags )provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usTopRow = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usLeftCol = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_iHeaderColor = provider.ReadInt32( iOffset );
      iOffset += 4;

      if( m_iLength > 10 )
      {
        m_usPageBreakZoom = provider.ReadUInt16( iOffset );
        iOffset += 2;

        m_usNormalZoom = provider.ReadUInt16( iOffset );
        iOffset += 2;
      }

      if( m_iLength > 14 )
      {
        m_iReserved = provider.ReadInt32( iOffset );
        //iOffset += 4;
      }

      m_iOriginalLength = m_iLength;
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <returns>Size of the record data.</returns>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = GetStoreSize( version );
      provider.WriteUInt16( iOffset, ( ushort )m_options );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usTopRow );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usLeftCol );
      iOffset += 2;

      provider.WriteInt32( iOffset, m_iHeaderColor );
      iOffset += 4;

      provider.WriteUInt16( iOffset, m_usPageBreakZoom );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usNormalZoom );
      iOffset += 2;

      provider.WriteInt32( iOffset, m_iReserved );
      // iOffset += 2;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return ( m_iOriginalLength > 0 ) ?
        m_iOriginalLength :
        DEF_MAX_RECORD_SIZE;
    }
    #endregion
  }
}
