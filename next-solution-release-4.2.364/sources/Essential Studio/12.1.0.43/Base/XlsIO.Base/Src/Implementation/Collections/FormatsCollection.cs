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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Syncfusion.XlsIO.FormatParser;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
	/// <summary>
	/// Represents collection of formats in the workbook.
	/// </summary>
	public class FormatsCollection
    : CommonObject
    //, IDictionary<int, FormatImpl>
	{
    #region Class constants
    /// <summary>
    /// Represents the Decimal Seprator.
    /// </summary>
    public const string DecimalSeparator = ".";
    /// <summary>
    /// Represents the Thousand seprator.
    /// </summary>
    public const string ThousandSeparator = ",";
    /// <summary>
    /// Represents the percentage in decimal numbers.
    /// </summary>
    public const string Percentage = "%";
    /// <summary>
    /// Represents the fraction symbol.
    /// </summary>
    public const string Fraction = "/";
    /// <summary>
    /// Represents the index of the date format.
    /// </summary>
    public const string Date = "date";
    /// <summary>
    /// Represents the time separator.
    /// </summary>
    public const string Time = ":";
    /// <summary>
    /// Represents the Exponenet Symbol.
    /// </summary>
    public const string Exponent = "E";
    /// <summary>
    /// Represents the Minus symbol.
    /// </summary>
    public const string Minus = "-";
    /// <summary>
    /// Represents the Currency Symbol. 
    /// TODO: support currency based on the Culture.
    /// </summary>
    public const string Currency = "$";
    public const string DEFAULT_EXPONENTAIL = "E+";
    /// <summary>
    /// Represents the collection of DateFormats.
    /// </summary>
    private string[] AdditionalDateFormats =
    {
        "[$-409]m/d/yy h:mm AM/PM;@",
    };
    /// <summary>
    /// Index to the first user-defined number format.
    /// </summary>
    internal const int DEF_FIRST_CUSTOM_INDEX = 163;
    /// <summary>
    /// Default format strings.
    /// </summary>
    internal string[] DEF_FORMAT_STRING = {
      "General",
      "0",
      "0.00",
      "#,##0",
      "#,##0.00",
      "\"$\"#,##0_);\\( \"$\"#,##0\\ )",
      "\"$\"#,##0_);[Red]\\( \"$\"#,##0\\ )",
      "\"$\"#,##0.00_);\\( \"$\"#,##0.00\\ )",
      "\"$\"#,##0.00_);[Red]\\( \"$\"#,##0.00\\ )",
      "0%",
      "0.00%",
      "0.00E+00",
      "# ?/?",
      "# ??/??",
      "m/d/yyyy",
      @"d\-mmm\-yy",
      @"d\-mmm",
      @"mmm\-yy",
      "h:mm AM/PM",
      "h:mm:ss AM/PM",
      "h:mm",
      "h:mm:ss",
      "m/d/yy h:mm",
      @"_( #,##0_);\( #,##0\ )",
      @"_( #,##0_);[Red]\( #,##0\ )",
      @"_( #,##0.00_);\( #,##0.00\ )",
      @"_( #,##0.00_);[Red]\( #,##0.00\ )",
      "_(* #,##0_);_(* \\( #,##0\\ );_(* \"-\"_);_( @_ )",
      "_(\"$\"* #,##0_);_(\"$\"* \\( #,##0\\ );_(\"$\"* \"-\"_);_( @_ )",
      "_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)",
      "_(\"$\"* #,##0.00_);_(\"$\"* \\( #,##0.00\\ );_(\"$\"* \"-\"??_);_( @_ )",
      "mm:ss",
      "[h]:mm:ss",
      "mm:ss.0",
      "##0.0E+0",
      "@" };
    /// <summary>
    /// Japan code.
    /// </summary>
    private const int CountryJapan = 81;
    #endregion

    #region Class members
    /// <summary>
    /// Index-to-FormatImpl.
    /// </summary>
    private TypedSortedListEx<int, FormatImpl> m_rawFormats = new TypedSortedListEx<int,FormatImpl>();
    /// <summary>
    /// Dictionary. Key - format string, value - FormatImpl.
    /// </summary>
    private Dictionary<string, FormatImpl> m_hashFormatStrings = new Dictionary<string, FormatImpl>();
    /// <summary>
    /// Format parser.
    /// </summary>
    private FormatParserImpl m_parser;
    /// <summary>
    /// Represent the indexes of the Raw Formats.
    /// </summary>
    private Dictionary<string, int[]> m_formatIndexes = new Dictionary<string, int[]>();
    /// <summary>
    /// Number Formats Taken.
    /// </summary>
    private bool m_hasNumFormats;
    #endregion     
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance and sets its application and parent objects.
    /// </summary>
    /// <param name="application">Application object to set.</param>
    /// <param name="parent">Parent object to set.</param>
    public FormatsCollection( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public FormatImpl this[ int iIndex ]
    {
      get
      {
        return m_rawFormats[ iIndex ];
      }
    }
    /// <summary>
    /// Gets or Sets a value indicating whether worksheet contains number formats.
    /// </summary>
    internal bool HasNumberFormats
    {
        get
        {
            return m_hasNumFormats;
        }
        set
        {
            m_hasNumFormats = value;
        }
    }
    /// <summary>
    /// Returns single entry from the collection by format string. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public FormatImpl this[ string strFormat ]
    {
      get
      {
        return m_hashFormatStrings[ strFormat ];
      }
    }
    /// <summary>
    /// Returns format parser. Read-only.
    /// </summary>
    public FormatParserImpl Parser
    {
      get
      {
        if( m_parser == null )
          m_parser = new FormatParserImpl( Application, this );

        return m_parser;
      }
    }
    #endregion

    #region Class parse / serialization methods
    /// <summary>
    /// Parses list of biff records.
    /// </summary>
    /// <param name="data">Records to parse.</param>
    /// <param name="iPos">Offset to the format records.</param>
    /// <returns>Position after format records.</returns>
    public int Parse( IList data, int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      int iCount = data.Count;

      if( iPos < 0 || iPos >= iCount )
        throw new ArgumentOutOfRangeException( "iPos" );

      throw new NotImplementedException();
    }
    /// <summary>
    /// Saves formats into list of biff records.
    /// </summary>
    /// <param name="records">List to save into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.AddRange( GetUsedFormats( ExcelVersion.Excel97to2003 ) );
    }
    /// <summary>
    /// Adds new format to the collection.
    /// </summary>
    /// <param name="format">Format to add.</param>
    [ CLSCompliant( false ) ]
    public void Add( FormatRecord format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      FormatImpl formatImpl = new FormatImpl( Application, this, format );
      int iIndex = format.Index;
      Register( formatImpl );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="formatId"></param>
    /// <param name="formatString"></param>
    internal void Add( int formatId, string formatString )
    {
      if( formatString == null )
        throw new ArgumentOutOfRangeException( "formatString" );

      if( formatId < 0 )
        throw new ArgumentOutOfRangeException( "formatId" );

      if (formatString.Length == 0)
          return;

      FormatImpl format = new FormatImpl( Application, this, formatId, formatString );
      Register( format );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    private void Register( FormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      m_rawFormats[ format.Index ] = format;
      m_hashFormatStrings[ format.FormatString ] = format;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Creates copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for the new collection.</param>
    /// <returns>Copy of the current instance.</returns>
    public FormatsCollection Clone( object parent )
    {
      //throw new NotImplementedException();
      FormatsCollection result = ( FormatsCollection )MemberwiseClone();
      result.SetParent( parent );
      result.m_parser = null;
      result.m_rawFormats = new TypedSortedListEx<int, FormatImpl>();
      result.m_hashFormatStrings = new Dictionary<string, FormatImpl>();

      foreach( KeyValuePair<int, FormatImpl> entry in m_rawFormats )
      {
        FormatImpl item = entry.Value;
        item = ( FormatImpl )item.Clone( result );
        result.m_rawFormats.Add( entry.Key, item );
        result.m_hashFormatStrings.Add( item.FormatString, item );
      }

      return result;
    }
    /// <summary>
    /// Method that creates format object based on the format string
    /// and registers it in the workbook.
    /// </summary>
    /// <param name="formatString">Format string for the new format record.</param>
    /// <returns>Index of created format.</returns>
    public int CreateFormat( string formatString )
    {
      if( formatString == null )
        throw new ArgumentNullException( "formatString" );

      if( formatString.Length == 0 )
        throw new ArgumentException( "formatString - string cannot be empty" );
      if (formatString.Contains(DEFAULT_EXPONENTAIL.ToLower()))
          formatString = formatString.Replace(DEFAULT_EXPONENTAIL.ToLower(), DEFAULT_EXPONENTAIL);
      if (formatString.Contains(DEFAULT_EXPONENTAIL.ToLower()))
          formatString = formatString.Replace(DEFAULT_EXPONENTAIL.ToLower(), DEFAULT_EXPONENTAIL);

      if( ContainsFormat( formatString ) )
      {
        FormatImpl format = m_hashFormatStrings[ formatString ];
        return format.Index;
      }

      int iCount = m_rawFormats.Count;
      int index = m_rawFormats.GetKey( iCount - 1 );
      
      // NOTE: By Microsoft, indexes less than 163 are reserved for build-in formats.
      if( index < DEF_FIRST_CUSTOM_INDEX ) index = DEF_FIRST_CUSTOM_INDEX;
      index++;

      FormatRecord record = ( FormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Format );
      record.FormatString = formatString;
      record.Index = index;
      Add( record );

      return index;
    }
    /// <summary>
    /// Determines whether the IDictionary contains an element with the specified format.
    /// </summary>
    /// <param name="formatString">Format to locate in the collection.</param>
    /// <returns>True if the collection contains an element with the key; otherwise, False.</returns>
    public bool ContainsFormat( string formatString )
    {
      if( formatString == null || formatString.Length == 0 )
        return false;

      return m_hashFormatStrings.ContainsKey( formatString );
    }
    /// <summary>
    /// Searches for format with specified format string
    /// and creates one if a match is not found.
    /// </summary>
    /// <param name="formatString">String describing needed format.</param>
    /// <returns>Found or created format.</returns>
    public int FindOrCreateFormat( string formatString )
    {
      FormatImpl format;

      return ( m_hashFormatStrings.TryGetValue( formatString, out format ) ) ?
        format.Index :
        CreateFormat( formatString );
    }
    /// <summary>
    /// Inserts all default formats into list.
    /// </summary>
    public void InsertDefaultFormats()
    {
      FormatRecord curFormat = ( FormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Format );

      int iFormatIndex = 0;

      for( int iIndex = 0, iLength = DEF_FORMAT_STRING.Length; iIndex < iLength; iIndex++ )
      {
        curFormat.Index = iFormatIndex;
        curFormat.FormatString = DEF_FORMAT_STRING[ iIndex ];
        if( !m_rawFormats.Contains( curFormat.Index ) )
          Add( ( FormatRecord )curFormat.Clone() );

        if( iFormatIndex == 22 )
          iFormatIndex = 36;

        iFormatIndex++;
      }
    }
    /// <summary>
    /// Gets all used formats.
    /// </summary>
    /// <returns>Array that contains all used format records.</returns>
    public List<FormatRecord> GetUsedFormats( ExcelVersion version )
    {
      List<FormatRecord> result = new List<FormatRecord>();

      if( version == ExcelVersion.Excel97to2003 )
      {
        result.Add( this[ 5 ].Record );
        result.Add( this[ 6 ].Record );
        result.Add( this[ 7 ].Record );
        result.Add( this[ 8 ].Record );
        result.Add( this[ 42 ].Record );
        result.Add( this[ 41 ].Record );
        result.Add( this[ 44 ].Record );
        result.Add( this[ 43 ].Record );
      }
      else
      {
        if( this[ 5 ].FormatString != DEF_FORMAT_STRING[ 5 ] )
          result.Add( this[ 5 ].Record );

        if( this[ 6 ].FormatString != DEF_FORMAT_STRING[ 6 ] )
          result.Add( this[ 6 ].Record );

        if( this[ 7 ].FormatString != DEF_FORMAT_STRING[ 7 ] )
          result.Add( this[ 7 ].Record );

        if( this[ 8 ].FormatString != DEF_FORMAT_STRING[ 8 ] )
          result.Add( this[ 8 ].Record );

        if( this[ 42 ].FormatString != DEF_FORMAT_STRING[ 28 ] )
          result.Add( this[ 42 ].Record );

        if( this[ 41 ].FormatString != DEF_FORMAT_STRING[ 27 ] )
          result.Add( this[ 41 ].Record );

        if( this[ 44 ].FormatString != DEF_FORMAT_STRING[ 30 ] )
          result.Add( this[ 44 ].Record );

        if( this[ 43 ].FormatString != DEF_FORMAT_STRING[ 29 ] )
          result.Add( this[ 43 ].Record );
      }

      int index = m_rawFormats.IndexOfKey( 49 ); // 49 is last index of always present formats
      int iCount = m_rawFormats.Count;

      if( index >= 0 && index < iCount - 1 )
      {
        for( int i = index + 1; i < iCount; i++ )
        {
          FormatImpl format = m_rawFormats.GetByIndex( i );

          if( format.Index >= DEF_FIRST_CUSTOM_INDEX || HasNumberFormats)
            result.Add( format.Record );
        }
      }

      return result;
    }
    /// <summary>
    /// Copies all formats from the source collection.
    /// </summary>
    /// <param name="source">Collection to copy formats from.</param>
    /// <returns>Dictionary old format index - to - new format index.</returns>
    public Dictionary<int, int> Merge( FormatsCollection source )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      Dictionary<int, int> result = new Dictionary<int, int>();
      TypedSortedListEx<int, FormatImpl> listFormats = source.m_rawFormats;

      for( int i = 0, len = source.Count; i < len; i++ )
      {
        FormatImpl format = listFormats.GetByIndex( i );

        int iOldIndex = format.Index;
        int iNewIndex = AddCopy( format );
        result.Add( iOldIndex, iNewIndex );
      }

      return result;
    }
    /// <summary>
    /// Adds format range to the collection.
    /// </summary>
    /// <param name="dicIndexes">Dictionary with format indexes to add.</param>
    /// <param name="source">Source collection.</param>
    /// <returns>Dictionary with updated indexes: key - old index, value - new index.</returns>
    public Dictionary<int, int> AddRange( IDictionary dicIndexes, FormatsCollection source )
    {
      if( dicIndexes == null )
        throw new ArgumentNullException( "dicIndexes" );

      if( source == null )
        throw new ArgumentNullException( "source" );

      Dictionary<int, int> result = new Dictionary<int, int>();

      foreach( int iFormatIndex in dicIndexes.Keys )
      {
        FormatImpl format = source[ iFormatIndex ];
        int iNewIndex = AddCopy( format );
        result.Add( iFormatIndex, iNewIndex );
      }

      return result;
    }
    /// <summary>
    /// Tries to add format to the collection.
    /// </summary>
    /// <param name="format">Format to add.</param>
    /// <returns>New format index.</returns>
    private int AddCopy( FormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      return AddCopy( format.Record );
    }
    /// <summary>
    /// Tries to add format to the collection.
    /// </summary>
    /// <param name="format">Format to add.</param>
    /// <returns>New format index.</returns>
    private int AddCopy( FormatRecord format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      string strFormat = format.FormatString;
      return CreateFormat( strFormat );
    }
    /// <summary>
    /// Adds Japanese number formats to the collection.
    /// </summary
    private void AddJapaneseFormats()
    {
      Add( 27, "[$-411]ge.m.d" );
      Add( 28, "[$-411]ggge\\\"\x5E74\\\"m\\\"\x6708\\\"d\\\"\x65E5\\\"" );
      Add( 29, "[$-411]ggge\\\"\x5E74\\\"m\\\"\x6708\\\"d\\\"\x65E5\\\"" );
      Add( 30, "m/d/yy" );
      Add( 31, "yyyy\\\"\x5E74\\\"m\\\"\x6708\\\"d\\\"\x65E5\\\"" );
      Add( 32, "h\\\"\x6642\\\"mm\\\"\x5206\\\"" );
      Add( 33, "h\\\"\x6642\\\"mm\\\"\x5206\\\"ss\\\"\x79D2\\\"" );
      Add( 34, "yyyy\\\"\x5E74\\\"m\\\"\x6708\\\"" );
      Add( 35, "m\\\"\x6708\\\"d\\\"\x65E5\\\"" );
      Add( 36, "[$-411]ge.m.d" );
      Add( 50, "[$-411]ge.m.d" );
      Add( 51, "[$-411]ggge\\\"\x5E74\\\"m\\\"\x6708\\\"d\\\"\x65E5\\\"" );
      Add( 52, "yyyy\\\"\x5E74\\\"m\\\"\x6708\\\"" );
      Add( 53, "m\\\"\x6708\\\"d\\\"\x65E5\\\"" );
      Add( 54, "[$-411]ggge\\\"\x5E74\\\"m\\\"\x6708\\\"d\\\"\x65E5\\\"" );
      Add( 55, "yyyy\\\"\x5E74\\\"m\\\"\x6708\\\"" );
      Add( 56, "m\" \x6708\"d\" \x65E5\"" );
      Add( 57, "[$-411]ge.m.d yyyy\\\"\x5E74\\\"" );
      Add( 58, "[$-411]ggge\\\"\x5E74\\\"m\\\"\x6708\\\"d\\\"\x65E5\\\"" );
      //Add( 27, "yyyy\x5E74 mm\x6708 dd\x65E5" );
      //Add( 28, "mm-dd" );
      //Add( 29, "mm-dd" );
      //Add( 30, "mm-dd-yy" );
      //Add( 31, "yyyy\xB144 mm\xC6D4 dd\xC77C" );
      //Add( 32, "h\xC2DC mm\xBD84" );
      //Add( 33, "h\xC2DC mm\xBD84 ss\xCD08" );
      //Add( 34, "yyyy-mm-dd" );
      //Add( 35, "yyyy-mm-dd" );
      //Add( 36, "yyyy\x5E74 mm\x6708 dd\x65E5" );
      //Add( 50, "yyyy\x5E74 mm\x6708 dd\x65E5" );
      //Add( 51, "mm-dd" );
      //Add( 52, "yyyy-mm-dd" );
      //Add( 53, "yyyy-mm-dd" );
      //Add( 54, "mm-dd" );
      //Add( 55, "yyyy-mm-dd" );
      //Add( 56, "yyyy-mm-dd" );
      //Add( 57, "yyyy\x5E74 mm\x6708 dd\x65E5" );
      //Add( 58, "mm-dd" );
    }
    /// <summary>
    /// Adds country specific number formats to the collection.
    /// </summary
    internal void AddDefaultFormats( int country )
    {
      switch( country )
      {
        case CountryJapan:
          AddJapaneseFormats();
          break;

        default:
          break;
      }
    }
    /// <summary>
    /// Fills the format indexes based on the format types.
    /// </summary>
    internal void FillFormatIndexes()
    {
        if (m_formatIndexes.Count > 0)
            return;
        List<int> dateIndexes = new List<int>();
        dateIndexes.AddRange(new int[] { 14, 15, 16, 17,  22});
        foreach (string dateFormat in AdditionalDateFormats)
        {
            dateIndexes.Add(CreateFormat(dateFormat));
        }
        m_formatIndexes.Add(Percentage,new int[] {9,10});
        m_formatIndexes.Add(ThousandSeparator, new int[] { 3, 4, 5, 6, 7, 8 });
        m_formatIndexes.Add(DecimalSeparator, new int[] { 2,7 });
        m_formatIndexes.Add(Date, dateIndexes.ToArray());
        m_formatIndexes.Add(Time, new int[] { 18, 19, 20, 21,  });
        m_formatIndexes.Add(Fraction, new int[] { 12, 13 });
        m_formatIndexes.Add(Exponent, new int[] { 11 });
    }
    /// <summary>
    /// Detects the DateFormat from the string.
    /// </summary>
    /// <param name="strValue">string value to detect.</param>
    /// <returns>NumberFormat of the given string.</returns>
    internal string GetDateFormat(string strValue)
    {
        string[] dateSeparators = { "-", "/",",", " " };
        string separator = null;
        bool hasTime = false;
        int timeIndex = strValue.IndexOf(Time[0]);
        if (timeIndex != -1)
        {
            hasTime = true;
            if (timeIndex <= 2)
                return GetTimeFormat(strValue);
        }
        foreach (string strSeparator in dateSeparators)
        {
            if (strValue.IndexOf(strSeparator) != -1)
            {
                separator = strSeparator;
                break;
            }
        }
        int []indexes=m_formatIndexes[Date];
        string strDuplicate = strValue;
        if (hasTime)
        {
          int index=  strValue.IndexOf("AM");
          if (index != -1)
          {
              strDuplicate = strValue.Remove(index, 2);
              strDuplicate = strDuplicate.Remove(strDuplicate.Length - 1);
          }
          index=strValue.IndexOf("PM");
          if (index != -1)
          {
              strDuplicate = strValue.Remove(index, 2);
              strDuplicate = strDuplicate.Remove(strDuplicate.Length - 1);
          }
        }
        string formatString=null;
        string[] arrDateTime = strDuplicate.Split(separator[0]);
        int length = arrDateTime.Length;
        if (length == 3)
        {
            if (hasTime)
            {
                if (IsStandardTimeFormat(strValue))
                    formatString = m_rawFormats[indexes[5]].FormatString;
                else
                    formatString = m_rawFormats[indexes[4]].FormatString;
            }
            else if (arrDateTime[1].Length > 1 && Char.IsLetter(arrDateTime[1], 1))
                formatString = m_rawFormats[indexes[1]].FormatString;
            else
                formatString = m_rawFormats[indexes[0]].FormatString;
        }
        else if (length == 2)
        {
            if (Char.IsLetter(arrDateTime[1], 1))
                formatString = m_rawFormats[indexes[2]].FormatString;
            else
                formatString = m_rawFormats[indexes[3]].FormatString;
                
        }
        else
            formatString = GetTimeFormat(strValue);
        return formatString;
    }
    /// <summary>
    /// Detects the Time Format from the string.
    /// </summary>
    /// <param name="strValue">string to detect the format.</param>
    /// <returns>Number Format of the given string.</returns>
    private string GetTimeFormat(string strValue)
    {
        bool hasAMPM = false, hasSeconds = false;
        hasAMPM = IsStandardTimeFormat(strValue);
        hasSeconds = HasSecond(strValue);
        int[] indexes = m_formatIndexes[Time];
        
        string numberFormat=m_rawFormats[indexes[0]].FormatString;
        FormatImpl formatImpl = null;
        foreach (int index in indexes)
        {
            formatImpl = m_rawFormats[index];
            string formatString = formatImpl.FormatString;
            if (IsStandardTimeFormat(formatString) == hasAMPM && HasSecond(formatString) == hasSeconds)
                numberFormat = formatString;
        }

        return numberFormat;
    }
    /// <summary>
    /// Returns True, if strValue contains the time format with seconds.
    /// </summary>
    /// <param name="strValue">Time format in string.</param>
    /// <returns>boolean value</returns>
    private bool HasSecond(string strValue)
    {
        return strValue.Split(Time[0]).Length > 2;
    }
    /// <summary>
    /// Returns True, if the string format of time contains 12 Hours format.
    /// </summary>
    /// <param name="strValue">Time format in string.</param>
    /// <returns>returns boolean.</returns>
    private bool IsStandardTimeFormat(string strValue)
    {
        return (strValue.IndexOf("AM") != -1) || (strValue.IndexOf("PM") != -1);
    }
    /// <summary>
    /// Detects the Number format.
    /// </summary>
    /// <param name="strValue">number as string to detect the number format.</param>
    /// <returns>Number Format of given value.</returns>
    internal string GetNumberFormat(string strValue)
    {
        string numberFormat = ParseNumberFormat(strValue);
        return (numberFormat != null) ? numberFormat : RangeImpl.DEF_NUMBER_FORMAT;
    }
    /// <summary>
    /// Try parse the number format of the given value.
    /// </summary>
    /// <param name="strValue">number as string to parse.</param>
    /// <returns>NumberFormat of the given value.</returns>
    private string ParseNumberFormat(string strValue)
    {
        bool  isDecimal = false, isPercentage = false;
        bool isThousands = false, isScientific = false, isFraction = false;
        bool isCurrency = false;

        isCurrency = strValue.IndexOf(Currency) != -1;
        
        isDecimal = (strValue.IndexOf(DecimalSeparator[0]) != -1);

        isPercentage = (strValue.IndexOf(Percentage[0]) != -1);

        isThousands = (strValue.IndexOf(ThousandSeparator[0]) != -1);

        isScientific = (strValue.IndexOf(Exponent[0]) != -1);

        isFraction = (strValue.IndexOf(Fraction[0]) != -1);

        int[] indexes = null;
        string resultFormat = null;

        FormatImpl formatImpl =null;
        if (isThousands)
        {
            indexes = m_formatIndexes[ThousandSeparator];
            foreach (int index in indexes)
            {
                formatImpl = m_rawFormats[index];

                if (formatImpl.DecimalPlaces > 1 && isDecimal)
                {
                    if ((isCurrency && !(formatImpl.FormatString.IndexOf(Currency) != -1)))
                        continue;
                        resultFormat = formatImpl.FormatString;
                    break;
                }
            }
        }
        else if (isFraction)
        {
            indexes = m_formatIndexes[Fraction];
            if (strValue.Split(Fraction[0])[0].Length > 1)
                formatImpl = m_rawFormats[indexes[1]];
            else
                formatImpl = m_rawFormats[indexes[0]];
            resultFormat = formatImpl.FormatString;
        }
        else if (isPercentage)
        {
            indexes = m_formatIndexes[Percentage];
            if (isDecimal)
                resultFormat = m_rawFormats[indexes[1]].FormatString;
            else
                resultFormat = m_rawFormats[indexes[0]].FormatString;
        }
        else if (isDecimal)
        {
            indexes = m_formatIndexes[DecimalSeparator];
            if (isCurrency)
            {
               indexes = m_formatIndexes[ThousandSeparator];
               resultFormat = Currency + m_rawFormats[indexes[1]].FormatString;
            }
            else
                resultFormat = m_rawFormats[indexes[0]].FormatString;
        }
        else if (isScientific)
        {
            indexes = m_formatIndexes[Exponent];
            formatImpl = m_rawFormats[indexes[0]];
            resultFormat = formatImpl.FormatString;
        }
        else if (isCurrency)
        {
            indexes = m_formatIndexes[ThousandSeparator];
            resultFormat = Currency + m_rawFormats[indexes[1]].FormatString;
        }
        else
            resultFormat = m_rawFormats[1].FormatString;
        return resultFormat;
    }
    #endregion

    #region IDictionary Members
    /// <summary>
    /// Gets a value indicating whether the IDictionary is read-only. Read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Returns an IDictionaryEnumerator for the IDictionary.
    /// </summary>
    /// <returns>An IDictionaryEnumerator for the IDictionary.</returns>
    public IEnumerator<KeyValuePair<int, FormatImpl>> GetEnumerator()
    {
      return m_rawFormats.GetEnumerator();
    }

    /// <summary>
    /// Removes the element with the specified key from the IDictionary.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    public void Remove( int key )
    {
      FormatImpl format = m_rawFormats[ key ];

      if( format != null )
      {
        m_rawFormats.Remove( key );
        m_hashFormatStrings.Remove( format.FormatString );
      }
    }

    /// <summary>
    /// Determines whether the IDictionary contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the IDictionary.</param>
    /// <returns>True if the IDictionary contains an element with the key; otherwise, False.</returns>
    public bool Contains( int key )
    {
      return m_rawFormats.Contains( key );
    }

    /// <summary>
    ///  Removes all elements from the IDictionary.
    /// </summary>
    public void Clear()
    {
        foreach (KeyValuePair<string, FormatImpl> formatImpl in m_hashFormatStrings)
        {
            formatImpl.Value.Clear();
        }
      m_rawFormats.Clear();
      m_hashFormatStrings.Clear();

      m_rawFormats = null;
      m_hashFormatStrings = null;
      if (m_parser != null)
      {
          m_parser.Clear();
          m_parser.Dispose();
      }
      this.Dispose();

    }

    /// <summary>
    /// 
    /// </summary>
    public ICollection Values
    {
      get
      {
        throw new NotImplementedException();
        //return m_rawFormats.Values;
      }
    }

    /// <summary>
    /// Gets an ICollection containing the keys of the collection. Read-only.
    /// </summary>
    public ICollection Keys
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets a value indicating whether the collection has a fixed size. Read-only.
    /// </summary>
    public bool IsFixedSize
    {
      get
      {
        return m_rawFormats.IsFixedSize;
      }
    }

    #endregion

    #region ICollection Members
    /// <summary>
    /// Gets a value indicating whether access to the collection is synchronized (thread-safe).
    /// </summary>
    public bool IsSynchronized
    {
      get
      {
        return m_rawFormats.IsSynchronized;
      }
    }

    /// <summary>
    /// Gets the number of elements contained in the collection. Read-only
    /// </summary>
    public int Count
    {
      get
      {
        return m_rawFormats.Count;
      }
    }

    /// <summary>
    /// Copies the elements of the collection to an Array, starting at a particular Array index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional Array that is the destination of the elements
    /// copied from ICollection. The Array must have zero-based indexing.
    /// </param>
    /// <param name="index">The zero-based index in array at which copying begins.</param>
    public void CopyTo(Array array, int index)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets an object that can be used to synchronize access to the collection.
    /// </summary>
    public object SyncRoot
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    #endregion
  }
}
