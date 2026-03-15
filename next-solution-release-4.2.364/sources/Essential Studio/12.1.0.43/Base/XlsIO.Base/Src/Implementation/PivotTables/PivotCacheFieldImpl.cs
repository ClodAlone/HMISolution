#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections.Generic;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.PivotTable;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
  /// <summary>
  /// Describes single pivot field in the pivot cache.
  /// </summary>
  public class PivotCacheFieldImpl
  {
    #region Constants
    /// <summary>
    /// Represents the Maximum number of character supported by cache field string value.
    /// </summary>
    internal const int MaxStringLength = 255;
    #endregion

    #region Class members
    /// <summary>
    /// Main field record.
    /// </summary>
    private PivotFieldRecord m_field = ( PivotFieldRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.PivotField );
    /// <summary>
    /// Type id.
    /// </summary>
    private SQLDataTypeIdRecord m_typeId = ( SQLDataTypeIdRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.SQLDataTypeId );
    /// <summary>
    /// List with fields data.
    /// </summary>
    private List<IValueHolder> m_arrFieldsData = new List<IValueHolder>();
      /// <summary>
    /// Represents an item within a PivotTable field that uses a formula
      /// </summary>
    private PivotCalculatedItems m_calculatedItems;
    /// <summary>
    /// Index in the pivot caches collection.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Type of the field's data.
    /// </summary>
    private PivotDataType m_fieldType;
      /// <summary>
      /// Specifies a boolean value that indicates whether this field came from the source
      /// database 
      /// </summary>
    private bool m_bIsDataBaseField;
      /// <summary>
    /// Specifies the formula for the calculated field
      /// </summary>
    private string m_formula;
    /// <summary>
    /// Represents the field group
    /// </summary>
    private FieldGroupImpl m_fieldGroup;
      /// <summary>
    /// Specifies the caption of the cache field
      /// </summary>
    private string m_caption;
      /// <summary>
    /// Specifies the number format that is applied to all items in the field
      /// </summary>
    private int m_iNumFormatIndex;
      /// <summary>
      /// Field group index
      /// </summary>
    private int m_iParentFieldGroupIndex = -1;
    /// <summary>
    /// Represents the hierarchy that this field is part of.
    /// </summary>
    private int m_iHierarchy;
    /// <summary>
    /// Specifies the hierarchy level that this field is part of.
    /// </summary>
    private int m_iLevel;
    /// <summary>
    /// Indicates whether the shared items are parsed.
    /// </summary>
    private bool? m_bIsParsed;
    /// <summary>
    /// Represents the Fields items range.
    /// </summary>
    private IRange m_range;
    /// <summary>
    /// Represents the list of field items.
    /// </summary>
    private IList<object> m_items;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Main class constructor.
    /// </summary>
    public PivotCacheFieldImpl()
    {
        m_items = new List<object>();
    }
    /// <summary>
    /// Initializes pivot cache and extracts its values from BiffReader.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    [ CLSCompliant( false ) ]
    public PivotCacheFieldImpl( BiffReader reader )
    {
      Parse( reader );
    }
    #endregion

    #region Class properties
      /// <summary>
    /// Specifies the formula for the calculated field
      /// </summary>
    public string Formula
    {
        get
        {
            return m_formula;
        }
        set
        {
            m_formula = value;
        }
    }
    /// <summary>
    /// Specifies a boolean value that indicates whether this field came from the source
    /// database 
    /// </summary>
    public bool IsDataBaseField
    {
        get
        {
            return m_bIsDataBaseField;
        }
        set
        {
            m_bIsDataBaseField = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsInIndexList
    {
      get
      {
        return m_field.IsInIndexList;
      }
      set
      {
        m_field.IsInIndexList = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsDouble
    {
      get
      {
        return m_field.IsDouble;
      }
      set
      {
        m_field.IsDouble = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsDoubleInt
    {
      get
      {
        return m_field.IsDoubleInt;
      }
      set
      {
        m_field.IsDoubleInt = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsString
    {
      get
      {
        return m_field.IsString;
      }
      set
      {
        m_field.IsString = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsUnknown
    {
      get
      {
        return m_field.IsUnknown;
      }
      set
      {
        m_field.IsUnknown = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsLongIndex
    {
      get
      {
        return m_field.IsLongIndex;
      }
      set
      {
        m_field.IsLongIndex = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsUnknown2
    {
      get
      {
        return m_field.IsUnknown2;
      }
      set
      {
        m_field.IsUnknown2 = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsDate
    {
      get
      {
        return m_field.IsDate;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int ItemCount
    {
      get
      {
          if(m_items!=null)
              return m_items.Count ;
          return 0;
      }
//      set
//      {
//        if( value < 0 )
//          throw new ArgumentOutOfRangeException( "value", value, "Value cannot be less than 0" );
//
//        m_field.ItemCount1 = m_field.ItemCount2 = ( ushort )value;
//      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string Name
    {
      get
      {
        return m_field.Name;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        if( value.Length == 0 )
          throw new ArgumentException( "value - string cannot be empty" );

        m_field.Name = value;
      }
    }
    /// <summary>
    /// Gets / sets item's index in the parent collection.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
      set
      {
        m_iIndex = value;
      }
    }
    /// <summary>
    /// Gets type of the field's data. Read-only.
    /// </summary>
    public PivotDataType DataType
    {
      get
      {
        return m_fieldType;
      }
        internal set
        {
            m_fieldType=value;
        }
    }
      /// <summary>
      /// Indicates the field is formula field
      /// </summary>
    public bool IsFormulaField
    {
        get
        {
            return Formula != null;
        }
    }
      /// <summary>
      /// Represents the Field group in the Cache Field
      /// </summary>
    internal FieldGroupImpl FieldGroup
    {
        get
        {
            return m_fieldGroup;
        }
        set
        {
            m_fieldGroup = value;
        }
    }
    internal FieldGroupImpl InternalFieldGroup
    {
        get
        {
            if (m_fieldGroup == null)
                m_fieldGroup = new FieldGroupImpl(this);
            return m_fieldGroup;
        }
    }
    /// <summary>
    /// Specifies the caption of the cache field
    /// </summary>
    public string Caption
    {
        get
        {
            return m_caption;
        }
        set
        {
            m_caption = value;
        }
    }
    /// <summary>
    /// Specifies the number format that is applied to all items in the field
    /// </summary>
    public int NumFormatIndex
    {
        get
        {
            return m_iNumFormatIndex;
        }
        set
        {
            m_iNumFormatIndex = value;
        }
    }
    /// <summary>
    /// Represents an item within a PivotTable field that uses a formula
    /// </summary>
    public PivotCalculatedItems CalculatedItems
    {
        get
        {
            if (m_calculatedItems == null)
                m_calculatedItems = new PivotCalculatedItems();
            return m_calculatedItems;
        }
    }
      
    /// <summary>
    /// Field group index
    /// </summary>
    public int ParentFeildGroupIndex
    {
        get
        {
            return m_iParentFieldGroupIndex;
        }
        set
        {
            m_iParentFieldGroupIndex = value;
        }
    }
      /// <summary>
      /// Represents the cache field is Field group
      /// </summary>
    public bool IsFieldGroup
    {
        get
        {
            return m_fieldGroup != null;
        }
    }
    /// <summary>
    /// Represents the hierarchy that this field is part of.
    /// </summary>
    internal int Hierarchy
    {
        get
        {
            return m_iHierarchy;
        }
        set
        {
            m_iHierarchy = value;
        }
    }
    /// <summary>
    /// Specifies the hierarchy level that this field is part of.
    /// </summary>
    internal int Level
    {
        get
        {
            return m_iLevel;
        }
        set
        {
            m_iLevel = value;
        }
    }
    /// <summary>
    /// Indicates whether the shared items are parsed.
    /// </summary>
    public bool? IsParsed
    {
        get
        {
            return m_bIsParsed;
        }
        set
        {
            m_bIsParsed = value;
        }
    }
    /// <summary>
    /// Represents the Fields items range.
    /// </summary>
    internal IRange ItemRange
    {
        get
        {
            return m_range;
        }
        set
        {
            m_range = value;
        }
    }
    /// <summary>
    /// Gets the field items.
    /// </summary>
    internal IList<object> Items
    {
        get
        {
            if (m_items == null)
                m_items = new List<object>();
            return m_items;
        }
        set
        {
            m_items = value;
        }
    }

    #endregion

    #region Class methods
    /// <summary>
    /// Returns value at index.
    /// </summary>
    /// <param name="index">Index of the value to get.</param>
    /// <returns>Requested value.</returns>
    public object GetValue( int index )
    {
      if( m_items==null && index < 0 || index >= ItemCount)
        throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than ItemCount" );

      return m_items[index];
    }
    /// <summary>
    /// Fills field with data.
    /// </summary>
    /// <param name="sheet">Worksheet to get data from.</param>
    /// <param name="row">First row of the source data.</param>
    /// <param name="lastRow">Last row of the source data.</param>
    /// <param name="column">Column index of the source data.</param>
    internal void Fill( IWorksheet sheet, int row, int lastRow, int column )
    {
        m_items = new List<object>();
        MigrantRangeImpl range = new MigrantRangeImpl(sheet.Application, sheet);
        m_typeId.DataType = SQLDataTypeIdRecord.SQLDataType.SQL_UNKNOWN_TYPE;
        Dictionary<object, int> dictUsedValues = new Dictionary<object, int>();
        int[] result = new int[lastRow - row + 1];
        int iUniqueItemsCount = 0;
        RangeImpl rangeImpl = m_range as RangeImpl;
        rangeImpl.LastRow = lastRow;
        m_items = rangeImpl.GetUniqueValues(ref m_fieldType);
    }
    /// <summary>
    /// Adds new value to the cache.
    /// </summary>
    /// <param name="value">Value to add.</param>
    internal int AddValue( object value )
    {
      // TODO: add Error and Empty support.
      m_items.Add(value);
      if( value is double )
      {
          
        m_fieldType |= PivotDataType.Number;

        double dValue = ( double )value;
        m_fieldType |= ( dValue <= int.MaxValue && dValue >= int.MinValue && Math.Round( dValue ) == dValue ) ?
          PivotDataType.Integer:
          PivotDataType.Float;

      }
      else if( value is TimeSpan )
      {
        m_fieldType |= PivotDataType.Date;
      }
      else if( value is DateTime )
      {
        m_fieldType |= PivotDataType.Date;
      }
      else if( value is string )
      {
        string strValue = ( string )value;

        if( strValue.Length > 0 || strValue== string.Empty )
        {
            if (strValue.Length > MaxStringLength)
                m_fieldType |= PivotDataType.LongText;
          m_fieldType |= PivotDataType.String;
        }
        else
        {
          m_fieldType |= PivotDataType.Blank;
        }
      }
      else if( value is Boolean )
      {
        m_fieldType |= PivotDataType.Boolean;
      }
      else if (value is ushort)
      {
          m_fieldType |= PivotDataType.Boolean;
      }
      else if (value == null)
      {
          m_fieldType |= PivotDataType.Blank;
      }
      else
      {
          throw new NotImplementedException("cache Value");    
      }
      m_field.ItemCount1 = m_field.ItemCount2 = ( ushort )m_items.Count;
      return ItemCount - 1;
    }
    #endregion

    #region Class parse / serialization methods
    /// <summary>
    /// Parses pivot cache field.
    /// </summary>
    /// <param name="reader">Reader to extract data from.</param>
    [ CLSCompliant( false ) ]
    public void Parse( BiffReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.PeekRecordType() != TBIFFRecord.PivotField )
        throw new ArgumentOutOfRangeException( "Unexpected record" );

      m_arrFieldsData.Clear();

      m_field = ( PivotFieldRecord )reader.GetRecord();
      m_typeId = ( SQLDataTypeIdRecord )reader.GetRecord();

      //TBIFFRecord currRecord = reader.PeekRecordType();

      for( int i = 0, len = m_field.ItemCount1; i < len; i++ )
      {
        IValueHolder record = ( IValueHolder )reader.GetRecord();
        m_arrFieldsData.Add( record );
      }
    }
    /// <summary>
    /// Serializes pivot cache field into list of Biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( m_field );
      records.Add( m_typeId );
      records.AddList( m_arrFieldsData );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Returns Biff record that corresponds to the specified value.
    /// </summary>
    /// <param name="value">Value to get record for.</param>
    /// <returns>Created record that contains specified value.</returns>
    private BiffRecordRaw CreateRecordForValue( object value )
    {
      if( value == null )
      {
        return BiffRecordFactory.GetRecord( TBIFFRecord.PivotEmpty );
      }
      else if( value is Boolean )
      {
        PivotBooleanRecord boolean = ( PivotBooleanRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.PivotBoolean );
        boolean.Value = ( bool )value;

        return boolean;
      }
      else if( value is double )
      {
        PivotDoubleRecord doubleRecord = ( PivotDoubleRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.PivotDouble );
        doubleRecord.Value = ( double )value;

        return doubleRecord;
      }
      else if( value is string )
      {
        PivotStringRecord stringRecord = ( PivotStringRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.PivotString );
        stringRecord.String = ( string )value;

        return stringRecord;
      }
      else if( value is ushort )
      {
        PivotErrorRecord error = ( PivotErrorRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.PivotError );

        return error;
      }

      throw new NotSupportedException();
    }
      
  
    #endregion
  }
}
