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
using System.Reflection;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Attribute provides link information between class and Biff8.
  /// record types
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ AttributeUsage( AttributeTargets.Class, AllowMultiple = true ) ]
  sealed public class BiffAttribute : Attribute
  {
    #region Class members
    /// <summary>
    /// Biff record code.
    /// </summary>
    private TBIFFRecord m_code;
    #endregion

    #region Class Properties
    /// <summary>
    /// Read-only. Returns code of record.
    /// </summary>
    public TBIFFRecord Code
    {
      get
      {
        return m_code;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. To prevent creation of attribute without attributes.
    /// </summary>
    private BiffAttribute()
    {
    }
    /// <summary>
    /// Creates attribute by record code.
    /// </summary>
    /// <param name="code">Biff record code.</param>
    public BiffAttribute( TBIFFRecord code )
    {
      m_code = code;
    }
    #endregion
  }

  /// <summary>
  /// Known field types to parser.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum TFieldType
  {
    /// <summary>
    /// Represents the Integer field type.
    /// </summary>
    Integer,
    /// <summary>
    /// Represents the Bit field type.
    /// </summary>
    Bit,
    /// <summary>
    /// Represents the String field type.
    /// </summary>
    String,
    /// <summary>
    /// Represents the String16Bit field type.
    /// </summary>
    String16Bit,
    /// <summary>
    /// Represents the OEMString field type.
    /// </summary>
    OEMString,
    /// <summary>
    /// Represents the OEMString16Bit field type.
    /// </summary>
    OEMString16Bit,
    /// <summary>
    /// Represents the Float field type.
    /// </summary>
    Float,
  }


  /// <summary>
  /// Attribute of records class members that provide information
  /// about location of the variable in binary data. It also indicates
  /// type to which the data must be converted to.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ AttributeUsage( AttributeTargets.Field ) ]
  sealed public class BiffRecordPosAttribute :
      Attribute,
      IComparable
  {
    #region Class members
    /// <summary>
    /// Position of the field in the record.
    /// </summary>
    private int   m_iPos;
    /// <summary>
    /// Size of the field.
    /// </summary>
    private int   m_iSize;
    /// <summary>
    /// True if attribute describes bit field.
    /// </summary>
    private bool  m_bIsBit;
    /// <summary>
    /// True if attribute describes string field.
    /// </summary>
    private bool  m_bIsString;
    /// <summary>
    /// Indicates whether attribute describes string field with 16 bit length.
    /// </summary>
    private bool  m_bIsString16Bit;
    /// <summary>
    /// True if attribute describes OEM string field.
    /// </summary>
    private bool  m_bIsOEMString;
    /// <summary>
    /// True if attribute describes OEM string field.
    /// </summary>
    private bool  m_bIsOEMString16Bit;
    /// <summary>
    /// True if attribute describes float field.
    /// </summary>
    private bool  m_bIsFloat;
    /// <summary>
    /// True if attribute describes signed field.
    /// </summary>
    private bool  m_bSigned;
    #endregion

    #region Class Properties
    /// <summary>
    /// Read-only. Returns position of the field in the record data.
    /// </summary>
    public int  Position
    {
      get
      {
        return m_iPos;
      }
    }

    /// <summary>
    /// Read-only. Returns size of the filed or bit position (for bit fields).
    /// </summary>
    public int  SizeOrBitPosition
    {
      get
      {
        return m_iSize;
      }
    }

    /// <summary>
    /// Read-only. Returns True if attribute describes bit field.
    /// </summary>
    public bool IsBit
    {
      get
      {
        return m_bIsBit;
      }
    }

    /// <summary>
    /// Read-only. Returns True if attribute describes signed field.
    /// </summary>
    public bool IsSigned
    {
      get
      {
        return m_bSigned;
      }
    }

    /// <summary>
    /// Read-only. Returns True if attribute describes string field.
    /// </summary>
    public bool IsString
    {
      get
      {
        return m_bIsString;
      }
    }

    /// <summary>
    /// Indicates whether attribute describes string field with 16 bit length.
    /// </summary>
    public bool IsString16Bit
    {
      get
      {
        return m_bIsString16Bit;
      }
    }

    /// <summary>
    /// Read-only. Returns True if attribute describes float field.
    /// </summary>
    public bool IsFloat
    {
      get
      {
        return m_bIsFloat;
      }
    }
    /// <summary>
    /// Read-only. Returns True if this attribute describes OEM string.
    /// </summary>
    public bool IsOEMString
    {
      get
      {
        return m_bIsOEMString;
      }
    }
    /// <summary>
    /// Read-only. Returns True if this attribute describes OEM string with 16 bit length field.
    /// </summary>
    public bool IsOEMString16Bit
    {
      get
      {
        return m_bIsOEMString16Bit;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates attribute by field position, size, signed flag, and field type.
    /// </summary>
    /// <param name="pos">
    /// Position of the filed data in the record data array.
    /// </param>
    /// <param name="size">
    /// Size of the field data or position of the bit in the byte.
    /// </param>
    /// <param name="isSigned">Is field signed or not?</param>
    /// <param name="type">Type of the field.</param>
    public BiffRecordPosAttribute( int pos, int size, bool isSigned, TFieldType type )
    {
      m_iPos = pos;
      m_iSize = size;
      m_bSigned = isSigned;

      m_bIsBit = ( type == TFieldType.Bit );
      m_bIsString = ( type == TFieldType.String );
      m_bIsString16Bit = ( type == TFieldType.String16Bit );
      m_bIsOEMString = ( type == TFieldType.OEMString );
      m_bIsOEMString16Bit = ( type == TFieldType.OEMString16Bit );
      m_bIsFloat = ( type == TFieldType.Float );
    }

    /// <summary>
    /// Creates attribute for integer field by its position,
    /// size, and signed flag.
    /// </summary>
    /// <param name="pos">
    /// Position of the filed data in the record data array.
    /// </param>
    /// <param name="size">
    /// Size of the field data or position of the bit in the byte.
    /// </param>
    /// <param name="isSigned">Is field signed or not?</param>
    public BiffRecordPosAttribute( int pos, int size, bool isSigned )
      : this( pos, size, isSigned, TFieldType.Integer )
    {
    }

    /// <summary>
    /// Creates attribute for unsigned field by field position,
    /// size, and field type.
    /// </summary>
    /// <param name="pos">
    /// Position of the filed data in the record data array.
    /// </param>
    /// <param name="size">
    /// Size of the field data or position of the bit in the byte.
    /// </param>
    /// <param name="type">Type of the field.</param>
    public BiffRecordPosAttribute( int pos, int size, TFieldType type )
      : this( pos, size, false, type )
    {
    }

    /// <summary>
    /// Creates attribute by field position and field type.
    /// Field size is zero and field is unsigned.
    /// </summary>
    /// <param name="pos">
    /// Position of the filed data in the record data array.
    /// </param>
    /// <param name="type">Type of the field.</param>
    public BiffRecordPosAttribute( int pos, TFieldType type )
      : this( pos, 0, false, type )
    {
    }

    /// <summary>
    /// Creates attribute for unsigned field by field position and size.
    /// </summary>
    /// <param name="pos">
    /// Position of the filed data in the record data array.
    /// </param>
    /// <param name="size">
    /// Size of the field data or position of the bit in the byte.
    /// </param>
    public BiffRecordPosAttribute( int pos, int size )
      : this( pos, size, false )
    {
    }
    #endregion

    #region IComparable Members

    public int CompareTo(object obj)
    {
      RecordsPosComparer comparer = new RecordsPosComparer();
      BiffRecordPosAttribute toCompare = obj as BiffRecordPosAttribute;
      return comparer.Compare( this, toCompare );
    }

    #endregion
  }


  /// <summary>
  /// Existence of such attribute in class metadata indicates to the Excel writer that
  /// the record class contains offset field which can be calculated only after
  /// all records are saved into an array and are in place.
  /// </summary>
  [ AttributeUsage( AttributeTargets.Class, AllowMultiple = true ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  sealed public class BiffOffsetsRecordsAttribute : Attribute
  {
    #region Class members
    /// <summary>
    /// Type of the Biff record.
    /// </summary>
    private TBIFFRecord m_type;
    #endregion

    #region Class Properties
    /// <summary>
    /// Get type of records used for offset calculations.
    /// </summary>
    public TBIFFRecord OffsetsRecordsType
    {
      get
      {
        return m_type;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// No public default constructor.
    /// </summary>
    private BiffOffsetsRecordsAttribute()
    {

    }
    /// <summary>
    /// Constructs attribute for specified Biff record type.
    /// </summary>
    /// <param name="type">Type of the Biff record.</param>
    public BiffOffsetsRecordsAttribute( TBIFFRecord type )
    {
      m_type = type;
    }
    #endregion
  }

  /// <summary>
  /// This attribute tells the Excel Writer the order in which order offsets should be calculated.
  /// </summary>
  [ AttributeUsage( AttributeTargets.Class, AllowMultiple = false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  sealed public class BiffOffsetOrderAttribute : Attribute
  {
    #region Class members
    /// <summary>
    /// Array that stores the Biff records order.
    /// </summary>
    private TBIFFRecord[] m_order;
    #endregion

    #region Class Properties
    /// <summary>
    /// Read-only. Returns the array of Biff records order.
    /// </summary>
    public TBIFFRecord[] OrderArray
    {
      get
      {
        return m_order;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. Prevents creating attribute without parameters.
    /// </summary>
    private BiffOffsetOrderAttribute()
    {
    }

    /// <summary>
    /// Constructs attribute and fills array of records
    /// order with specified values.
    /// </summary>
    /// <param name="order">Order of the biff records.</param>
    public BiffOffsetOrderAttribute( params TBIFFRecord[] order )
    {
      m_order = order;
    }
    #endregion
  }
}