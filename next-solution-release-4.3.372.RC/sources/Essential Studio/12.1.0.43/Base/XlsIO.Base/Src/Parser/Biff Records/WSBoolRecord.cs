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
  /// This record stores a 16-bit value with Boolean options
  /// for the current sheet.
  /// </summary>
  [ CLSCompliant( false ) ]
  [ Biff( TBIFFRecord.WSBool ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class WSBoolRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Bit mask for DisplayGuts property.
    /// </summary>
    public const ushort DisplayGutsMask = 0x0C00;
    /// <summary>
    /// First bit of the DispayGuts value.
    /// </summary>
    public const int    DisplayGutsStartBit = 10;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_RECORD_SIZE = 2;
    /// <summary>
    /// 
    /// </summary>
    [ Flags ]
    private enum OptionFlags : ushort
    {
      /// <summary>
      /// Indicates whether automatic breaks are visible.
      /// </summary>
      AutoBreaks = 1,
      /// <summary>
      /// Indicates whether sheet is a dialog sheet.
      /// </summary>
      Dialog = 16,
      /// <summary>
      /// Indicates whether to apply automatic styles to outlines.
      /// </summary>
      ApplyStyles = 32,
      /// <summary>
      /// Indicates whether summary rows will appear below detail in outlines.
      /// </summary>
      RowSumsBelow = 64,
      /// <summary>
      /// Indicates whether summary rows will appear right of the detail in outlines.
      /// </summary>
      RowSumsRight = 128,
      /// <summary>
      /// Indicates whether to fit stuff to the page.
      /// </summary>
      FitToPage = 256,
      /// <summary>
      /// Indicates whether to use alternate expression eval.
      /// </summary>
      AlternateExpression = 16384,
      /// <summary>
      /// Indicates whether to use alternate formula entry.
      /// </summary>
      AlternateFormula = 32768,
    }
    #endregion

    #region Class members

    /// <summary>
    /// Option flags (you should use bit fields).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private OptionFlags m_options = ( OptionFlags )1217;
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates if automatic breaks are visible.
    /// </summary>
    public bool IsAutoBreaks
    {
      get
      {
        return ( m_options & OptionFlags.AutoBreaks ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.AutoBreaks;
        }
        else
        {
          m_options &= ~OptionFlags.AutoBreaks;
        }
      }
    }

    /// <summary>
    /// Indicates if this is a sheet dialog sheet.
    /// </summary>
    public bool IsDialog
    {
      get
      {
        return ( m_options & OptionFlags.Dialog ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.Dialog;
        }
        else
        {
          m_options &= ~OptionFlags.Dialog;
        }
      }
    }

    /// <summary>
    /// Whether to apply automatic styles to outlines.
    /// </summary>
    public bool IsApplyStyles
    {
      get
      {
        return ( m_options & OptionFlags.ApplyStyles ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.ApplyStyles;
        }
        else
        {
          m_options &= ~OptionFlags.ApplyStyles;
        }
      }
    }

    /// <summary>
    /// Whether summary rows will appear below detail in outlines.
    /// </summary>
    public bool IsRowSumsBelow
    {
      get
      {
        return ( m_options & OptionFlags.RowSumsBelow ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.RowSumsBelow;
        }
        else
        {
          m_options &= ~OptionFlags.RowSumsBelow;
        }
      }
    }

    /// <summary>
    /// Whether summary rows will appear right of the detail in outlines.
    /// </summary>
    public bool IsRowSumsRight
    {
      get
      {
        return ( m_options & OptionFlags.RowSumsRight ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.RowSumsRight;
        }
        else
        {
          m_options &= ~OptionFlags.RowSumsRight;
        }
      }
    }

    /// <summary>
    /// Whether to fit stuff to the page.
    /// </summary>
    public bool IsFitToPage
    {
      get
      {
        return ( m_options & OptionFlags.FitToPage ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.FitToPage;
        }
        else
        {
          m_options &= ~OptionFlags.FitToPage;
        }
      }
    }

    /// <summary>
    /// Whether to display outline symbols (in the gutters).
    /// Changes bits of m_usOptions.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When value is more than 3.
    /// </exception>
    public ushort DisplayGuts
    {
      get
      {
        return (ushort) ( GetUInt16BitsByMask( ( ushort )m_options, DisplayGutsMask )
          >> DisplayGutsStartBit );
      }
      set
      {
        if( value > 3 )
        {
          throw new ArgumentOutOfRangeException();
        }

        ushort usOptions = ( ushort )m_options;
        SetUInt16BitsByMask( ref usOptions, DisplayGutsMask, ( ushort )
          ( value << DisplayGutsStartBit ) );

        m_options = ( OptionFlags )usOptions;
      }
    }

    /// <summary>
    /// Whether to use alternate expression eval.
    /// </summary>
    public bool IsAlternateExpression
    {
      get
      {
        return ( m_options & OptionFlags.AlternateExpression ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.AlternateExpression;
        }
        else
        {
          m_options &= ~OptionFlags.AlternateExpression;
        }
      }
    }

    /// <summary>
    /// Whether to use alternate formula entry.
    /// </summary>
    public bool IsAlternateFormula
    {
      get
      {
        return ( m_options & OptionFlags.AlternateFormula ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.AlternateFormula;
        }
        else
        {
          m_options &= ~OptionFlags.AlternateFormula;
        }
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

    /// <summary>
    /// 
    /// </summary>
    public override int MaximumMemorySize
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
    public  WSBoolRecord()
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
    public  WSBoolRecord( Stream stream, out int itemSize )
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
    public  WSBoolRecord( int iReserve )
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
      m_iLength = MinimumRecordSize;
      provider.WriteUInt16( iOffset, ( ushort )m_options );
    }

    #endregion
  }
}
