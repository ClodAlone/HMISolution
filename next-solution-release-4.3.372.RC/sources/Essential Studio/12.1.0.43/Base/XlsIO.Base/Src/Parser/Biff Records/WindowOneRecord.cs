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
  /// Stores the attributes of the workbook window.  This is basically
  /// so that the GUI is aware of the size of the window holding the spreadsheet
  /// document.
  /// </summary>
  [ Biff( TBIFFRecord.WindowOne ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class WindowOneRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 18;
    /// <summary>
    /// Possible option flags.
    /// </summary>
    private enum OptionFlags
    {
      /// <summary>
      /// Indicates whether window is hidden.
      /// </summary>
      Hidden   = 1,
      /// <summary>
      /// Indicates whether window is icon.
      /// </summary>
      Iconic   = 2,
      /// <summary>
      /// Reserved.
      /// </summary>
      Reserved = 4,
      /// <summary>
      /// Indicates whether to display horizontal scrollbar.
      /// </summary>
      HScroll  = 8,
      /// <summary>
      /// Indicates whether to display vertical scrollbar.
      /// </summary>
      VScroll  = 16,
      /// <summary>
      /// Display tabs at the bottom.
      /// </summary>
      Tabs     = 32,
  }
    #endregion

    #region Class members

    /// <summary>
    ///  Horizontal position.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usHHold = 240; //360;

    /// <summary>
    ///  Vertical position.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usVHold = 90; //30;

    /// <summary>
    /// The width of the window.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usWidth = 11340; //15180;

    /// <summary>
    /// The height of the window.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usHeight = 6795; //10875;

    /// <summary>
    /// The option's bitmask (see bit setters).
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private OptionFlags m_options = ( OptionFlags )56;

    /// <summary>
    /// The selected tab number.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usSelectedTab = 0;

    /// <summary>
    /// The displayed tab number.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usDisplayedTab = 0;

    /// <summary>
    /// The number of selected tabs.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usNumSelTabs = 1;

    /// <summary>
    /// Ratio of the width of the tabs to the horizontal scrollbar.
    /// </summary>
    [ BiffRecordPos( 16, 2 ) ]
    private ushort m_usTabWidthRatio = 600;
    #endregion

    #region Class properties
    /// <summary>
    ///  Horizontal position of the window.
    /// </summary>
    public ushort HHold
    {
      get
      {
        return m_usHHold;
      }
      set
      {
        m_usHHold = value;
      }
    }

    /// <summary>
    ///  Vertical position of the window.
    /// </summary>
    public ushort VHold
    {
      get
      {
        return m_usVHold;
      }
      set
      {
        m_usVHold = value;
      }
    }

    /// <summary>
    /// The width of the window.
    /// </summary>
    public ushort Width
    {
      get
      {
        return m_usWidth;
      }
      set
      {
        m_usWidth = value;
      }
    }

    /// <summary>
    /// The height of the window.
    /// </summary>
    public ushort Height
    {
      get
      {
        return m_usHeight;
      }
      set
      {
        m_usHeight = value;
      }
    }

    /// <summary>
    /// The selected tab number.
    /// </summary>
    public ushort SelectedTab
    {
      get
      {
        return m_usSelectedTab;
      }
      set
      {
        m_usSelectedTab = value;
      }
    }

    /// <summary>
    /// The displayed tab number.
    /// </summary>
    public ushort DisplayedTab
    {
      get
      {
        return m_usDisplayedTab;
      }
      set
      {
        m_usDisplayedTab = value;
      }
    }

    /// <summary>
    /// The number of selected tabs.
    /// </summary>
    public ushort NumSelectedTabs
    {
      get
      {
        return m_usNumSelTabs;
      }
      set
      {
        m_usNumSelTabs = value;
      }
    }

    /// <summary>
    /// Ratio of the width of the tabs to the horizontal scrollbar.
    /// </summary>
    public ushort TabWidthRatio
    {
      get
      {
        return m_usTabWidthRatio;
      }
      set
      {
        m_usTabWidthRatio = value;
      }
    }

    /// <summary>
    /// Indicates whether window is hidden.
    /// </summary>
    public bool   IsHidden
    {
      get
      {
        return ( m_options & OptionFlags.Hidden ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.Hidden;
        }
        else
        {
          m_options &= ~OptionFlags.Hidden;
        }
      }
    }

    /// <summary>
    /// Indicates whether window is icon.
    /// </summary>
    public bool   IsIconic
    {
      get
      {
        return ( m_options & OptionFlags.Iconic ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.Iconic;
        }
        else
        {
          m_options &= ~OptionFlags.Iconic;
        }
      }
    }

    /// <summary>
    /// Indicates whether to display horizontal scrollbar.
    /// </summary>
    public bool   IsHScroll
    {
      get
      {
        return ( m_options & OptionFlags.HScroll ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.HScroll;
        }
        else
        {
          m_options &= ~OptionFlags.HScroll;
        }
      }
    }

    /// <summary>
    /// Indicates whether to display vertical scrollbar.
    /// </summary>
    public bool   IsVScroll
    {
      get
      {
        return ( m_options & OptionFlags.VScroll ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.VScroll;
        }
        else
        {
          m_options &= ~OptionFlags.VScroll;
        }
      }
    }

    /// <summary>
    /// Display tabs at the bottom.
    /// </summary>
    public bool   IsTabs
    {
      get
      {
        return ( m_options & OptionFlags.Tabs ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.Tabs;
        }
        else
        {
          m_options &= ~OptionFlags.Tabs;
        }
      }
    }
    /// <summary>
    /// Property value reserved by Microsoft for own values.
    /// </summary>
    public bool   Reserved
    {
      get
      {
        return ( m_options & OptionFlags.Reserved ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.Reserved;
        }
        else
        {
          m_options &= ~OptionFlags.Reserved;
        }
      }
    }
    /// <summary>
    /// Read-only. Options flag.
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
        return DEF_RECORD_SIZE;
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
        return DEF_RECORD_SIZE;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  WindowOneRecord()
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
    public  WindowOneRecord( Stream stream, out int itemSize )
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
    public  WindowOneRecord( int iReserve )
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
      m_usHHold = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usVHold = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usWidth = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usHeight = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_options = ( OptionFlags )provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usSelectedTab = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usDisplayedTab = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usNumSelTabs = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usTabWidthRatio = provider.ReadUInt16( iOffset );
      //iOffset += 2;
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
      m_iLength = DEF_RECORD_SIZE;
      provider.WriteUInt16( iOffset, m_usHHold );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usVHold );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usWidth );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usHeight );
      iOffset += 2;

      provider.WriteUInt16( iOffset, ( ushort )m_options );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usSelectedTab );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usDisplayedTab );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usNumSelTabs );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usTabWidthRatio );
      //iOffset += 2;
    }
    #endregion
  }
}