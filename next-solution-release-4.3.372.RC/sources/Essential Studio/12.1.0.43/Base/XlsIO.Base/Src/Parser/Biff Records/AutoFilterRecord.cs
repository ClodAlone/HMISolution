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
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for AutoFilterInfoRecord.
  /// </summary>
  [ Biff( TBIFFRecord.AutoFilter ) ]
  [ CLSCompliant( false ) ]
  public class AutoFilterRecord
    : BiffRecordRaw
    , ICloneable
  {
    #region Internal classes
    /// <summary>
    /// 
    /// </summary>
    public class DOPER : ICloneable
    {
      #region Class constants
      /// <summary>
      /// Record size
      /// </summary>
      private const int DEF_SIZE = 10;
      /// <summary>
      /// 
      /// </summary>
      private const int DEF_DATATYPE_OFFSET = 0;
      /// <summary>
      /// 
      /// </summary>
      private const int DEF_SIGN_OFFSET = 1;
      /// <summary>
      /// 
      /// </summary>
      private const int DEF_VALUE_OFFSET = 2;
      /// <summary>
      /// 
      /// </summary>
      private const int DEF_STRING_LENGTH_OFFSET = 6;
      /// <summary>
      /// DOPER data type.
      /// </summary>
      public enum DOPERDataType
      {
        /// <summary>
        /// Represents the FilterNotUsed data type.
        /// </summary>
        FilterNotUsed   = 0x0,
        /// <summary>
        /// Represents the RKNumber data type.
        /// </summary>
        RKNumber        = 0x2,
        /// <summary>
        /// Represents the Number data type.
        /// </summary>
        Number          = 0x4,
        /// <summary>
        /// Represents the String data type.
        /// </summary>
        String          = 0x6,
        /// <summary>
        /// Represents the BoolOrError data type.
        /// </summary>
        BoolOrError     = 0x8,
        /// <summary>
        /// Represents the MatchBlanks data type.
        /// </summary>
        MatchBlanks     = 0x0C,
        /// <summary>
        /// Represents the MatchNonBlanks data type.
        /// </summary>
        MatchNonBlanks  = 0x0E,
      }
      /// <summary>
      /// DOPER comparison sign types.
      /// </summary>
      public enum DOPERComparisonSign
      {
        /// <summary>
        /// Represents the Less comparison sign type.
        /// </summary>
        Less            = 1,
        /// <summary>
        /// Represents the Equal comparison sign type.
        /// </summary>
        Equal           = 2,
        /// <summary>
        /// Represents the LessOrEqual comparison sign type.
        /// </summary>
        LessOrEqual     = 3,
        /// <summary>
        /// Represents the Greater comparison sign type.
        /// </summary>
        Greater         = 4,
        /// <summary>
        /// Represents the NotEqual comparison sign type.
        /// </summary>
        NotEqual        = 5,
        /// <summary>
        /// Represents the GreaterOrEqual comparison sign type.
        /// </summary>
        GreaterOrEqual  = 6
      }
      #endregion

      #region Class members
      /// <summary>
      /// 
      /// </summary>
      private byte[] m_data = new byte[ DEF_SIZE ];
      /// <summary>
      /// String value if there is some.
      /// </summary>
      private string m_strValue = string.Empty;
      #endregion

      #region Class properties
      /// <summary>
      /// Data type.
      /// </summary>
      public DOPERDataType DataType
      {
        get
        {
          return ( DOPERDataType )m_data[ DEF_DATATYPE_OFFSET ];
        }
        set
        {
          m_data[ DEF_DATATYPE_OFFSET ] = ( byte )value;
        }
      }
      /// <summary>
      /// Comparison sign.
      /// </summary>
      public DOPERComparisonSign ComparisonSign
      {
        get
        {
          return ( DOPERComparisonSign )m_data[ DEF_SIGN_OFFSET ];
        }
        set
        {
          m_data[ DEF_SIGN_OFFSET ] = ( byte )value;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public int RKNumber
      {
        get
        {
          return BitConverter.ToInt32( m_data, DEF_VALUE_OFFSET );
        }
        set
        {
          DataType = DOPERDataType.RKNumber;
          BitConverter.GetBytes( value ).CopyTo( m_data, DEF_VALUE_OFFSET );
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public double Number
      {
        get
        {
          return BitConverter.ToDouble( m_data, DEF_VALUE_OFFSET );
        }
        set
        {
          DataType = DOPERDataType.Number;
          BitConverter.GetBytes( value ).CopyTo( m_data, DEF_VALUE_OFFSET );
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public bool IsBool
      {
        get
        {
          return ( DataType == DOPERDataType.BoolOrError && m_data[ DEF_VALUE_OFFSET ] == 1 );
        }
      }

      /// <summary>
      /// 
      /// </summary>
      public bool Boolean
      {
        get
        {
          return ( m_data[ DEF_VALUE_OFFSET + 1 ] != 0 );
        }
        set
        {
          DataType = DOPERDataType.BoolOrError;
          m_data[ DEF_VALUE_OFFSET ] = 1;
          m_data[ DEF_VALUE_OFFSET + 1 ] = ( byte )( value ? 1 : 0 );
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public byte ErrorCode
      {
        get
        {
          return m_data[ DEF_VALUE_OFFSET + 1 ];
        }
        set
        {
          DataType = DOPERDataType.BoolOrError;
          m_data[ DEF_VALUE_OFFSET ] = 0;
          m_data[ DEF_VALUE_OFFSET + 1 ] = ( byte )value;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public bool HasAdditionalData
      {
        get
        {
          return ( DataType == DOPERDataType.String );
        }
      }
      /// <summary>
      /// Gets / sets length of the string (the string is stored after DOPER structures).
      /// </summary>
      public byte StringLength
      {
        get
        {
          return m_data[ DEF_STRING_LENGTH_OFFSET ];
        }
        set
        {
          DataType = DOPERDataType.String;
          m_data[ DEF_STRING_LENGTH_OFFSET ] = value;
        }
      }
      /// <summary>
      /// String value if there is some.
      /// </summary>
      public string StringValue
      {
        get
        {
          return m_strValue;
        }
        set
        {
          //For some reason Excel 2003 sets 1 in seven byte in DOPER structure for string values.
          m_data[ 7 ] = 1;
          m_strValue = value;
          StringLength = ( byte )value.Length;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public int Length
      {
        get
        {
          return DEF_SIZE + ( ( StringLength > 0 ) ? StringLength * 2 + 1 : 0 );
        }
      }
      #endregion

      #region Class methods
      /// <summary>
      /// Extracts data from data provider.
      /// </summary>
      /// <param name="provider">Object that provides access to the data.</param>
      /// <param name="iOffset">Offset in the provider to the start of the record's data.</param>
      /// <returns>Size of the parsed data.</returns>
      public int Parse( DataProvider provider, int iOffset )
      {
        provider.ReadArray( iOffset, m_data );

        return DEF_SIZE;
      }
      /// <summary>
      /// Extracts additional data from specified provider, if necessary.
      /// </summary>
      /// <param name="provider">Object that provides access to the data.</param>
      /// <param name="iOffset">Offset in the provider to the start of the record's data.</param>
      /// <returns>Size of the parsed data.</returns>
      public int ParseAdditionalData( DataProvider provider, int iOffset )
      {
        if( !HasAdditionalData ) return 0;

        if( DataType == DOPERDataType.String && StringLength > 0 )
        {
          int iLength;
          m_strValue = provider.ReadString( iOffset, StringLength, out iLength, false );
          return iLength;
        }

        return 0;
      }
      /// <summary>
      /// Serializes record into specified data provider.
      /// </summary>
      /// <param name="provider">Object that provides access to the data.</param>
      /// <param name="iOffset">Offset in the provider to the start of the record's data.</param>
      /// <returns>Size of the serialized data.</returns>
      public int Serialize( DataProvider provider, int iOffset )
      {
        if( provider == null )
          throw new ArgumentNullException( "provider" );

        provider.WriteBytes( iOffset, m_data, 0, DEF_SIZE );
        return DEF_SIZE;
      }
      /// <summary>
      /// Serializes additional data.
      /// </summary>
      /// <param name="provider">Object that provides access to the data.</param>
      /// <param name="iOffset">Offset in the destination array to the start of the data.</param>
      /// <returns>Size of the additional data.</returns>
      /// <exception cref="System.ArgumentNullException">
      /// If data is null.
      /// </exception>
      /// <exception cref="System.ArgumentOutOfRangeException">
      /// When iOffset is too big or size of the data array is too small
      /// and additional data can't be fit into the data array.
      /// </exception>
      public int SerializeAdditionalData( DataProvider provider, int iOffset )
      {
        if( m_strValue == null || m_strValue.Length == 0 )
          return 0;

        if( provider == null )
          throw new ArgumentNullException( "provider" );

        if( iOffset < 0 )
          throw new ArgumentOutOfRangeException( "iOffset" );

        int iStartOffset = iOffset;
        provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strValue, true );
        return iOffset - iStartOffset;
      }
      #endregion

      #region ICloneable Members

      /// <summary>
      /// Creates a new object that is a copy of the current instance.
      /// </summary>
      /// <returns>A new object that is a copy of this instance.</returns>
      public object Clone()
      {
        DOPER result = ( DOPER )MemberwiseClone();
        result.m_data = new byte[ DEF_SIZE ];
        Buffer.BlockCopy( m_data, 0, result.m_data, 0, DEF_SIZE );

        return result;
      }

      #endregion
    }
    #endregion

    #region Class constants
    /// <summary>
    /// Size of the record.
    /// </summary>
    private const int DEF_RECORD_MIN_SIZE = 24;
    /// <summary>
    /// Bit mask of the number of items to show.
    /// </summary>
    private const int DEF_TOP10_BITMASK = 0xFF80;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_TOP10_FIRSTBIT = 7;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_FIRST_CONDITION_OFFSET = 4;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_SECOND_CONDITION_OFFSET = 14;
    /// <summary>
    /// Offset to the start of additional data in the internal data array.
    /// </summary>
    private const int DEF_ADDITIONAL_OFFSET = 24;
    #endregion

    #region Class members
    /// <summary>
    /// Number of AutoFilter drop-down arrows on the sheet.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usIndex;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// True if the custom filter conditions are ANDed;
    /// False if the custom filter conditions are ORed.
    /// </summary>
    [ BiffRecordPos( 2, 0, TFieldType.Bit ) ]
    private bool m_bOr;
    /// <summary>
    /// True if the first condition is a simple equality.
    /// </summary>
    [ BiffRecordPos( 2, 2, TFieldType.Bit ) ]
    private bool m_bSimple1;
    /// <summary>
    /// True if the second condition is a simple equality.
    /// </summary>
    [ BiffRecordPos( 2, 3, TFieldType.Bit ) ]
    private bool m_bSimple2;
    /// <summary>
    /// True if the condition is a Top 10 AutoFilter.
    /// </summary>
    [ BiffRecordPos( 2, 4, TFieldType.Bit ) ]
    private bool m_bTop10;
    /// <summary>
    /// True if the Top 10 AutoFilter shows the top items;
    /// False if it shows the bottom items.
    /// </summary>
    [ BiffRecordPos( 2, 5, TFieldType.Bit ) ]
    private bool m_bTop;
    /// <summary>
    /// True if the Top 10 AutoFilter shows percentage;
    /// False if it shows items.
    /// </summary>
    [ BiffRecordPos( 2, 6, TFieldType.Bit ) ]
    private bool m_bPercent;

    /// <summary>
    /// Structure for the first filter condition.
    /// </summary>
    private DOPER m_firstCondition = new DOPER();
    /// <summary>
    /// Structure for the second filter condition.
    /// </summary>
    private DOPER m_secondCondition = new DOPER();
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_MIN_SIZE;
      }
    }

    /// <summary>
    /// Number of AutoFilter drop-down arrows on the sheet.
    /// </summary>
    public ushort Index
    {
      get
      {
        return m_usIndex;
      }
      set
      {
        m_usIndex = value;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// True if the first condition is a simple equality.
    /// </summary>
    public bool IsSimple1
    {
      get
      {
        return m_bSimple1;
      }
      set
      {
        m_bSimple1 = value;
      }
    }
    /// <summary>
    /// True if the second condition is a simple equality.
    /// </summary>
    public bool IsSimple2
    {
      get
      {
        return m_bSimple2;
      }
      set
      {
        m_bSimple2 = value;
      }
    }
    /// <summary>
    /// True if the condition is a Top 10 AutoFilter.
    /// </summary>
    public bool IsTop10
    {
      get
      {
        return m_bTop10;
      }
      set
      {
        m_bTop10 = value;
      }
    }
    /// <summary>
    /// True if the Top 10 AutoFilter shows the top items;
    /// False if it shows the bottom items.
    /// </summary>
    public bool IsTop
    {
      get
      {
        return m_bTop;
      }
      set
      {
        m_bTop = value;
      }
    }
    /// <summary>
    /// True if the Top 10 AutoFilter shows percentage;
    /// False if it shows items.
    /// </summary>
    public bool IsPercent
    {
      get
      {
        return m_bPercent;
      }
      set
      {
        m_bPercent = value;
      }
      }

    /// <summary>
    /// True if the custom filter conditions are ANDed;
    /// False if the custom filter conditions are ORed.
    /// </summary>
    public bool IsAnd
    {
      get
      {
        return !m_bOr;
      }
      set
      {
        m_bOr = !value;
      }
    }

    /// <summary>
    /// Number of elements to show in Top10 mode.
    /// </summary>
    public int Top10Number
    {
      get
      {
        return GetUInt16BitsByMask( m_usOptions, DEF_TOP10_BITMASK ) >> DEF_TOP10_FIRSTBIT;
      }
      set
      {
        if( value < 0 || value > 500 )
          throw new ArgumentOutOfRangeException( "Top10Number" );

        SetUInt16BitsByMask( ref m_usOptions, DEF_TOP10_BITMASK,
          ( ushort )( value << DEF_TOP10_FIRSTBIT ) );
      }
    }
    /// <summary>
    /// First condition.
    /// </summary>
    public DOPER FirstCondition
    {
      get
      {
        return m_firstCondition;
      }
    }
    /// <summary>
    /// Second condition.
    /// </summary>
    public DOPER SecondCondition
    {
      get
      {
        return m_secondCondition;
      }
    }
    /// <summary>
    /// If filtered to blanks - true. Read-only.
    /// </summary>
    public bool IsBlank
    {
      get
      {
        return FirstCondition.DataType == DOPER.DOPERDataType.MatchBlanks
          && SecondCondition.DataType == DOPER.DOPERDataType.FilterNotUsed;
      }
    }
    /// <summary>
    /// If filtered to nonblanks - true. Read-only.
    /// </summary>
    public bool IsNonBlank
    {
      get
      {
        return FirstCondition.DataType == DOPER.DOPERDataType.MatchNonBlanks
          && SecondCondition.DataType == DOPER.DOPERDataType.FilterNotUsed;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  AutoFilterRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize Constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  AutoFilterRecord( Stream stream, out int itemSize )
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
    public  AutoFilterRecord( int iReserve )
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
      int iStartOffset = iOffset;

      m_usIndex = provider.ReadUInt16( iOffset );
      iOffset += ExcelConstants.ShortSize;

      m_usOptions = provider.ReadUInt16( iOffset );
      iOffset += ExcelConstants.ShortSize;

      m_bOr = GetBitFromVar( m_usOptions, 0 );
      m_bSimple1 = GetBitFromVar( m_usOptions, 2 );
      m_bSimple2 = GetBitFromVar( m_usOptions, 3 );
      m_bTop10 = GetBitFromVar( m_usOptions, 4 );
      m_bTop = GetBitFromVar( m_usOptions, 5 );
      m_bPercent = GetBitFromVar( m_usOptions, 6 );

      m_firstCondition.Parse( provider, iStartOffset + DEF_FIRST_CONDITION_OFFSET );
      m_secondCondition.Parse( provider, iStartOffset + DEF_SECOND_CONDITION_OFFSET );

      iOffset = iStartOffset + DEF_ADDITIONAL_OFFSET;
      iOffset += m_firstCondition.ParseAdditionalData( provider, iOffset );
      m_secondCondition.ParseAdditionalData( provider, iOffset );
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
      m_iLength = GetStoreSize( ExcelVersion.Excel97to2003 );

      provider.WriteUInt16( iOffset, m_usIndex );
      iOffset += ExcelConstants.ShortSize;

      SetBitInVar( ref m_usOptions, m_bOr, 0 );
      SetBitInVar( ref m_usOptions, m_bSimple1, 2 );
      SetBitInVar( ref m_usOptions, m_bSimple2, 3 );
      SetBitInVar( ref m_usOptions, m_bTop10, 4 );
      SetBitInVar( ref m_usOptions, m_bTop, 5 );
      SetBitInVar( ref m_usOptions, m_bPercent, 6 );

      provider.WriteUInt16( iOffset, m_usOptions );
      iOffset += ExcelConstants.ShortSize;

      iOffset += m_firstCondition.Serialize( provider, iOffset );
      iOffset += m_secondCondition.Serialize( provider, iOffset );

      iOffset += m_firstCondition.SerializeAdditionalData( provider, iOffset );
      iOffset += m_secondCondition.SerializeAdditionalData( provider, iOffset );
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_FIRST_CONDITION_OFFSET + m_firstCondition.Length
        + m_secondCondition.Length;
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    new public object Clone()
    {
      AutoFilterRecord filter = ( AutoFilterRecord )base.Clone();

      if( m_firstCondition != null )
      {
        filter.m_firstCondition = ( DOPER )m_firstCondition.Clone();
      }

      if( m_secondCondition != null )
      {
        filter.m_secondCondition = ( DOPER )m_secondCondition.Clone();
      }

      return filter;
    }

    #endregion
  }
}
