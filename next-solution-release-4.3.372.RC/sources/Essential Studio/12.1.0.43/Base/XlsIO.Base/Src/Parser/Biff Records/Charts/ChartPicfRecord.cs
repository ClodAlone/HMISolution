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
  /// This record defines the format for a picture attached
  /// to a data series or point.
  /// </summary>
  [ Biff( TBIFFRecord.ChartPicf ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartPicfRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Picture type.
    /// </summary>
    [Flags]
    public enum TPicture : int
    {
      /// <summary>
      /// Represents the Stretched picture type.
      /// </summary>
      Stretched           = 1,
      /// <summary>
      /// Represents the Stacked picture type.
      /// </summary>
      Stacked             = 2,
    }
    /// <summary>
    /// Image format.
    /// </summary>
    public enum TImageFormat : int
    {
      /// <summary>
      /// Represents the WindowsMetafile image format.
      /// </summary>
      WindowsMetafile = 2,
      /// <summary>
      /// Represents the MacintoshPICT image format.
      /// </summary>
      MacintoshPICT   = 2,
      /// <summary>
      /// Represents the WindowsBitmap image format.
      /// </summary>
      WindowsBitmap   = 9,
    }
    /// <summary>
    /// Environment from which the file was written.
    /// </summary>
    public enum TEnvironment : int
    {
      /// <summary>
      /// Represents the Windows environment option.
      /// </summary>
      Windows = 1,
      /// <summary>
      /// Represents the Macintosh environment option.
      /// </summary>
      Macintosh = 2,
    }
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DefaultRecordSize = 14;
    #endregion

    #region Class members
    /// <summary>
    /// Picture type.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usPictureType;
    /// <summary>
    /// Image format.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usImageFormat;
    /// <summary>
    /// Environment from which the file was written.
    /// </summary>
    [ BiffRecordPos( 4, 1 ) ]
    private byte m_Environment;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 5, 1 ) ]
    private byte m_usOptions = 0;
    /// <summary>
    /// Formatting only; no picture attached.
    /// </summary>
    [ BiffRecordPos( 5, 0, TFieldType.Bit ) ]
    private bool m_bFormatOnly;
    /// <summary>
    /// Picture is attached to top and bottom of column.
    /// </summary>
    [ BiffRecordPos( 5, 1, TFieldType.Bit ) ]
    private bool m_bPictureTopBottom;
    /// <summary>
    /// Picture is attached to back and front of column.
    /// </summary>
    [ BiffRecordPos( 5, 2, TFieldType.Bit ) ]
    private bool m_bPictureBackFront;
    /// <summary>
    /// Picture is attached to sides of column.
    /// </summary>
    [ BiffRecordPos( 5, 3, TFieldType.Bit ) ]
    private bool m_bPictureSides;
    /// <summary>
    /// Scaling value for pictures, units/picture
    /// (IEEE floating-point number).
    /// </summary>
    [ BiffRecordPos( 6, 8, TFieldType.Float ) ]
    private double m_numScale;
    #endregion

    #region Class properties
    /// <summary>
    /// Picture type.
    /// </summary>
    public TPicture PictureType
    {
      get
      {
        return (TPicture) m_usPictureType;
      }
      set
      {
        m_usPictureType = (ushort) value;
      }
    }
    /// <summary>
    /// Image format.
    /// </summary>
    public TImageFormat ImageFormat
    {
      get
      {
        return (TImageFormat) m_usImageFormat;
      }
      set
      {
        m_usImageFormat = (ushort) value;
      }
    }
    /// <summary>
    /// Environment from which the file was written.
    /// </summary>
    public TEnvironment Environment
    {
      get
      {
        return (TEnvironment) m_Environment;
      }
      set
      {
        m_Environment = (byte) value;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public byte Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// Formatting only; no picture attached.
    /// </summary>
    public bool IsFormatOnly
    {
      get
      {
        return m_bFormatOnly;
      }
      set
      {
        m_bFormatOnly = value;
      }
    }
    /// <summary>
    /// Picture is attached to top and bottom of column.
    /// </summary>
    public bool IsPictureTopBottom
    {
      get
      {
        return m_bPictureTopBottom;
      }
      set
      {
        m_bPictureTopBottom = value;
      }
    }
    /// <summary>
    /// Picture is attached to back and front of column.
    /// </summary>
    public bool IsPictureBackFront
    {
      get
      {
        return m_bPictureBackFront;
      }
      set
      {
        m_bPictureBackFront = value;
      }
    }
    /// <summary>
    /// Picture is attached to sides of column.
    /// </summary>
    public bool IsPictureSides
    {
      get
      {
        return m_bPictureSides;
      }
      set
      {
        m_bPictureSides = value;
      }
    }
    /// <summary>
    /// Scaling value for pictures, units/picture
    /// (IEEE floating-point number).
    /// </summary>
    public double Scale
    {
      get
      {
        return m_numScale;
      }
      set
      {
        m_numScale = value;
      }
    }
    /// <summary>
    /// Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DefaultRecordSize;
      }
    }
    /// <summary>
    /// Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DefaultRecordSize;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartPicfRecord()
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
    public  ChartPicfRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartPicfRecord( int iReserve )
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
      // TODO: check correctness of data

      m_usPictureType = provider.ReadUInt16( iOffset + 0 );
      m_usImageFormat = provider.ReadUInt16( iOffset + 2 );
      m_Environment = provider.ReadByte( iOffset + 4 );
      m_usOptions = provider.ReadByte( iOffset + 5 );
      m_bFormatOnly = provider.ReadBit( iOffset + 5, 0 );
      m_bPictureTopBottom = provider.ReadBit( iOffset + 5, 1 );
      m_bPictureBackFront = provider.ReadBit( iOffset + 5, 2 );
      m_bPictureSides = provider.ReadBit( iOffset + 5, 3 );
      m_numScale = provider.ReadDouble( iOffset + 6 );
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
      provider.WriteUInt16( iOffset + 0, m_usPictureType );
      provider.WriteUInt16( iOffset + 2, m_usImageFormat );
      provider.WriteByte( iOffset + 4, m_Environment );
      provider.WriteByte( iOffset + 5, m_usOptions );
      provider.WriteBit( iOffset + 5, m_bFormatOnly, 0 );
      provider.WriteBit( iOffset + 5, m_bPictureTopBottom, 1 );
      provider.WriteBit( iOffset + 5, m_bPictureBackFront, 2 );
      provider.WriteBit( iOffset + 5, m_bPictureSides, 3 );
      provider.WriteDouble( iOffset + 6, m_numScale );
      m_iLength = DefaultRecordSize;
    }
    #endregion
  }
}
