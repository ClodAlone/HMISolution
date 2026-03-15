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

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record contains the definition of all user-defined colors available
  /// for cell and object formatting.
  /// </summary>
  [ Biff( TBIFFRecord.Palette ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class PaletteRecord : BiffRecordRaw
  {
    #region Class classes
    /// <summary>
    /// Structure describing color entry.
    /// </summary>
    public struct TColor
    {
      /// <summary>
      /// Value of the red part of the color.
      /// </summary>                                                          
      public byte R;
      /// <summary>
      /// Value of the green part of the color.
      /// </summary>
      public byte G;
      /// <summary>
      /// Value of the blue part of the color.
      /// </summary>
      public byte B;
      /// <summary>
      /// Not used.
      /// </summary>
      public byte A;

      /// <summary>
      /// Converts object to string.
      /// </summary>
      /// <returns>String representation of the object.</returns>
      public override string ToString()
      {
        return Color.FromArgb( A, R, G, B ).ToString();
      }
    }
    #endregion

    #region Class members
    /// <summary>
    /// Number of  colors (nm). Contains 16 in BIFF3-BIFF4
    /// and 56 in BIFF5-BIFF8.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usColorsCount = 56;
    /// <summary>
    /// Array of colors.
    /// </summary>
    private TColor[] m_arrColor = null;
    #endregion

    #region Class Properties
    /// <summary>
    /// Read-only. Number of  colors (nm). Contains 16
    /// in BIFF3-BIFF4 and 56 in BIFF5-BIFF8.
    /// </summary>
    public ushort ColorsCount
    {
      get
      {
        return m_usColorsCount;
      }
    }
    /// <summary>
    /// Array of colors.
    /// </summary>
    public TColor[] Colors
    {
      get
      {
        return m_arrColor;
      }
      set
      {
        m_arrColor = value;
        m_usColorsCount = ( value != null ) ? ( ushort ) value.Length : ( ushort ) 0;
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
        return 2;
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
        return 226;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  PaletteRecord()
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
    public  PaletteRecord( Stream stream, out int itemSize )
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
    public  PaletteRecord( int iReserve )
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
      m_usColorsCount = provider.ReadUInt16( iOffset + 0 );
      m_arrColor = new TColor[ m_usColorsCount ];
      iOffset += 2;

      for( int i = 0; i < m_usColorsCount; i++, iOffset += 4 )
      {
        m_arrColor[ i ].R = provider.ReadByte( iOffset );
        m_arrColor[ i ].G = provider.ReadByte( iOffset + 1 );
        m_arrColor[ i ].B = provider.ReadByte( iOffset + 2 );
        m_arrColor[ i ].A = provider.ReadByte( iOffset + 3 );
      }
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
      provider.WriteUInt16( iOffset + 0, m_usColorsCount );
      m_iLength = 2;

      for( int i = 0; i < m_usColorsCount; i++ )
      {
        provider.WriteByte( iOffset + m_iLength, m_arrColor[ i ].R );
        m_iLength++;

        provider.WriteByte( iOffset + m_iLength, m_arrColor[ i ].G );
        m_iLength++;

        provider.WriteByte( iOffset + m_iLength, m_arrColor[ i ].B );
        m_iLength++;

        provider.WriteByte( iOffset + m_iLength, m_arrColor[ i ].A );
        m_iLength++;
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return ExcelConstants.ShortSize + m_usColorsCount * ExcelConstants.IntSize;
    }
    #endregion
  }
}
