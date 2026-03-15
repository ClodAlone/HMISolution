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

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Interfaces;

using SSTDictionaryEntry = Syncfusion.XlsIO.Parser.Biff_Records.TextWithFormat;

#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
#endif
#if  (SILVERLIGHT) 
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif (WP)
using Syncfusion.XlsIO.Implementation.WP;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Dictionary is used for grouping strings used by Excel in order to prevent
  /// doubling of data. Dictionary stores information as key string and value.
  /// </summary>
  public class SSTDictionary
    : IParseable
    , IDisposable
  {
    #region Class constants
    /// <summary>
    /// Default reserved space by internal collections.
    /// </summary>
    private const int DEF_RESERVE_SPACE = 20;
    /// <summary>
    /// Index of the empty string.
    /// </summary>
    public const int DEF_EMPTY_STRING_INDEX = -1;
    /// <summary>
    /// Default count of empty strings.
    /// Returned by GetStringsCount method.
    /// Arbitrary number but should be larger than 1.
    /// </summary>
    private const int DEF_EMPTY_COUNT = 2;
    /// <summary>
    /// Maximum allowed cell length.
    /// </summary>
    private const int MaxCellLength = 32767;
    #endregion

    #region Class members
    /// <summary>
    /// Dictionary key-to-index in the array.
    /// </summary>
    private Dictionary<object, int> m_hashKeyToIndex = new Dictionary<object, int>( DEF_RESERVE_SPACE );
    /// <summary>
    /// Strings data.
    /// </summary>
    private List<object> m_arrStrings = new List<object>( DEF_RESERVE_SPACE );
    /// <summary>
    /// Array that contains indexes that are free (strings that were removed).
    /// </summary>
    private SortedList<int, int> m_arrFreeIndexes = new SortedList<int, int>( DEF_RESERVE_SPACE );
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Original SSTRecord.
    /// </summary>
    private SSTRecord m_sstOriginal;
    /// <summary>
    /// Indicates whether original SSTRecord was parsed.
    /// </summary>
    private bool m_bParsed = true;
    /// <summary>
    /// Temporary string.
    /// </summary>
    private TextWithFormat m_tempString = new TextWithFormat( 0 );
    /// <summary>
    /// Number of references to each string.
    /// </summary>
    //private DataProvider m_arrRefCount;
    private List<int> newRefCount;
    /// <summary>
    /// Indicates whether we should use hashtable to increase search speed.
    /// </summary>
    private bool m_bUseHash = true;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets references count to specified string.
    /// </summary>
    public int this[ TextWithFormat key ]
    {
      get
      {
        Parse();
        return Find( key );
      }
    }
    /// <summary>
    /// Gets text with format.
    /// </summary>
    public TextWithFormat this[ int index ]
    {
      get
      {
        object objResult = GetSSTContentByIndex( index );

        TextWithFormat result = objResult as TextWithFormat;

        if( result == null )
        {
          m_tempString.Text = objResult.ToString();
          result = m_tempString;
        }

        return result;
      }
    }
    /// <summary>
    /// Gets list of strings.
    /// </summary>
    public object[] Keys
    {
      get
      {
        int iCount = Count;
        object[] arrResult = new object[ iCount ];

        for( int i = 0; i < iCount; i++ )
        {
          arrResult[ i ] = m_arrStrings[ i ];//this[ i ];
        }

        return arrResult;
      }
    }
    /// <summary>
    /// Gets quantity of unique strings.
    /// </summary>
    public int Count
    {
      get
      {
        return ( m_bParsed )
          ? m_arrStrings.Count
          : ( int )m_sstOriginal.NumberOfUniqueStrings;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Gets / sets original SSTRecord.
    /// </summary>
    [ CLSCompliant( false ) ]
    public SSTRecord OriginalSST
    {
      get
      {
        return m_sstOriginal;
      }
      set
      {
        if( value == null )
        {
          throw new ArgumentNullException( "OriginalSST" );
        }

        m_bParsed = false;
        m_sstOriginal = value;

        if( m_sstOriginal.NumberOfUniqueStrings != 0 )
        {
          //m_arrRefCount.Dispose();
          //m_arrRefCount = new UnmanagedArray( ( int )m_sstOriginal.NumberOfUniqueStrings * ExcelConstants.IntSize, true );
          //m_arrRefCount.EnsureCapacity( ( int )m_sstOriginal.NumberOfUniqueStrings * ExcelConstants.IntSize );
          //m_arrRefCount.ZeroMemory();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool UseHashForSearching
    {
      get
      {
        return m_bUseHash;
      }
      set
      {
        if( m_bUseHash != value )
        {
          m_bUseHash = value;

          if( !value )
          {
            m_hashKeyToIndex.Clear();
          }
          else if( m_bParsed )
          {
            FillHash();
          }
        }
      }
    }
    /// <summary>
    /// Returns number of strings without removed items. Read-only.
    /// </summary>
    public int ActiveCount
    {
      get
      {
        return ( m_bParsed ) ?
          m_arrStrings.Count - m_arrFreeIndexes.Count :
          ( int )m_sstOriginal.NumberOfUniqueStrings;
      }
    }
    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Default constructor. Reserves space for strings.
    /// </summary>
    public SSTDictionary( WorkbookImpl book )
    {
      m_book = book;

      newRefCount = new List<int>();
      //m_arrRefCount.ZeroMemory();
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Returns SST content.
    /// </summary>
    /// <param name="index">Index of the target string.</param>
    /// <returns>SST content object.</returns>
    public object GetSSTContentByIndex( int index )
    {
      if( index < 0 || index >= Count )
        throw new ArgumentOutOfRangeException( "index" );

      object objResult = ( m_bParsed )
        ? m_arrStrings[ index ]
        : m_sstOriginal.Strings[ index ];

      return objResult;
    }
    /// <summary>
    /// Clears the list.
    /// </summary>
    public void Clear()
    {
      if( m_book != null )
      {
        if( m_bParsed )
        {
          m_arrStrings.Clear();
          m_hashKeyToIndex.Clear();
        }
        else
        {
          m_sstOriginal = null;
          m_bParsed = true;
        }
      }
    }
    /// <summary>
    /// Searches for all strings with specified text.
    /// </summary>
    /// <param name="value">String to search.</param>
    /// <returns>Dictionary, key - string index, value - null.</returns>
    public Dictionary<int, object> GetStringIndexes( string value )
    {
      object format;// = ( TextWithFormat )value;
      string strText;
      Dictionary<int, object> hashResult = new Dictionary<int, object>();
      object[] arrStrings = ( m_bParsed )
        ? null
        : m_sstOriginal.Strings;

      for( int i = 0, len = Count; i < len; i++ )
      {
        if( m_arrFreeIndexes.ContainsKey( i ) )
          continue;

        // TODO: this can be optimized.
        format = ( m_bParsed )
          ? m_arrStrings[ i ]
          : arrStrings[ i ];

        TextWithFormat temp = format as TextWithFormat;

        strText = ( temp != null )
          ? temp.Text
          : ( string )format;

        if( strText.IndexOf(value,0,StringComparison.CurrentCultureIgnoreCase)!=-1 )
        {
          hashResult.Add( i, null );
        }
      }

      return hashResult;
    }
    /// <summary>
    /// Adds string into dictionary. If the string exists in dictionary, 
    /// add reference count. Method can influence on SST Indexes.
    /// </summary>
    /// <param name="index">Index to the string.</param>
    public void AddIncrease( int index )
    {
      if( index == DEF_EMPTY_STRING_INDEX )
        return;

      //SSTDictionaryEntry entry = this[ index ] as SSTDictionaryEntry;
      //entry.RefCount++;
      int iCount = GetRefCount( index );//( int )m_arrRefCount[ index ];
      //m_arrRefCount[ index ] = iCount + 1;
      SetRefCount( index, iCount + 1 );
    }

    /// <summary>
    /// Adds string into dictionary. If the string exists in dictionary, 
    /// add reference count. Method can influence on SST Indexes.
    /// </summary>
    /// <param name="key">String to add</param>
    /// <returns>Index of the added string.</returns>
    public int AddIncrease( object key )
    {
      return AddIncrease( key, true );
    }

    /// <summary>
    /// Adds string into dictionary. If the string exists in dictionary, 
    /// add reference count. Method can influence on SST Indexes.
    /// </summary>
    /// <param name="key">String to add</param>
    /// <param name="bIncrease">Reference count for new entries.</param>
    /// <returns>Index of the added string.</returns>
    public int AddIncrease( object key, bool bIncrease )
    {
      int iIndex;

      // If we use this code not from Parse method than we should parse before calling.
      if( bIncrease )
        Parse();

      if( m_hashKeyToIndex.TryGetValue( key,out iIndex ) )
      {
        iIndex = m_hashKeyToIndex[ key ];

        if (bIncrease)
            AddIncrease(iIndex);
        else
        {
            m_arrStrings.Add(key);
        }
      }
      else
      {
        //SSTDictionaryEntry entry = new SSTDictionaryEntry( key, 1 );
        //SSTDictionaryEntry entry = key;
        CheckLength( key );
        int iRefCount = ( bIncrease ) ? 1 : 0;

        // Let's check if there is any empty entry in the table.
        if( m_arrFreeIndexes.Count == 0 )
        {
          iIndex = m_arrStrings.Count;
          m_arrStrings.Add( key );
          //m_arrRefCount.Add( iRefCount );

          if( m_bUseHash )
            m_hashKeyToIndex[ key ] = iIndex;

          // NOTE: there is no need to set number of references to this string
          // when it was set during parse stage.
          if( bIncrease )
            SetRefCount( iIndex, iRefCount );
        }
        else
        {
          // TODO: For the moment, we take first element in the m_arrFreeIndexes,
          // but it may be faster to take last element.
          iIndex = m_arrFreeIndexes.Values[ 0 ];
          m_arrStrings[ iIndex ] = key;

          if( m_bUseHash )
            m_hashKeyToIndex[ key ] = iIndex;

          m_arrFreeIndexes.RemoveAt( 0 );
          SetRefCount( iIndex, iRefCount );
        }
      }

      return iIndex;
    }
    /// <summary>
    /// Checks whether new string value is within allowed limits.
    /// </summary>
    /// <param name="key"></param>
    private void CheckLength( object key )
    {
      string str = key as string;
      int iLength = 0;

      if( str != null )
      {
        iLength = str.Length;
      }
      else if( key != null )
      {
        iLength = ( key as TextWithFormat ).Text.Length;
      }

      if( m_book.Version == ExcelVersion.Excel97to2003 && iLength > MaxCellLength )
        throw new ArgumentOutOfRangeException( "Text length cannot be more than " + MaxCellLength );
   }

    /// <summary>
    /// Remove reference of string. When reference count reaches zero, string will be
    /// removed from dictionary. Method can influence the SST indexes.
    /// </summary>
    /// <param name="key">String whose reference must be removed.</param>
    /// <exception cref="System.ArgumentException">
    /// When SST table does not contain the specified string.
    /// </exception>
    public void RemoveDecrease( object key )
    {
      Parse();

      int iIndex = Find( key );//( int )m_hashKeyToIndex[ key ];

      if( iIndex == -1 )
        throw new ArgumentException( "Dictionary does not contain specified string: '" + key + "'" );

      RemoveDecrease( iIndex );
    }
    /// <summary>
    /// Remove reference of string. When reference count reaches zero, string will be
    /// removed from dictionary. Method can influence the SST indexes.
    /// </summary>
    /// <param name="iIndex">String index to remove.</param>
    public void RemoveDecrease( int iIndex )
    {
      if( iIndex < 0 || iIndex >= Count )
        throw new ArgumentOutOfRangeException( "iIndex" );

      int iRefCount = GetRefCount( iIndex );
      iRefCount--;
      SetRefCount( iIndex, iRefCount );

      if( iRefCount <= 0 )
      {
        Parse();

        if( m_bUseHash )
          m_hashKeyToIndex.Remove( m_arrStrings[ iIndex ] );

        m_arrFreeIndexes[ iIndex ] = iIndex;
        m_arrStrings[ iIndex ] = 0;
      }
    }
    /// <summary>
    /// Method decreases reference count of string without removing it.
    /// </summary>
    /// <param name="index">Index of the string to decrease reference.</param>
    public void DecreaseOnly( int index )
    {
      if( index < 0 || index > m_arrStrings.Count )
        throw new ArgumentOutOfRangeException( "index" );

      //SSTDictionaryEntry entry = m_arrStrings[ index ] as SSTDictionaryEntry;
      //entry.RefCount--;
      int iRefCount = GetRefCount( index );//( int )m_arrRefCount[ index ];
      //m_arrRefCount[ index ] = iRefCount - 1;
      SetRefCount( index, iRefCount - 1 );
    }
    /// <summary>
    /// Method checks if the dictionary contains the specified string.
    /// </summary>
    /// <param name="key">String which must be checked.</param>
    /// <returns>True - string exists in dictionary, otherwise False.</returns>
    public bool Contains( object key )
    {
      Parse();
      return Find( key ) != -1;
    }
    /// <summary>
    /// Serialize dictionary.
    /// </summary>
    /// <param name="records">Records where writes serialized data.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( m_bParsed )
      {
        Defragment();
      }

      SaveIntoRecords( records );
    }
    /// <summary>
    /// Returns number of strings in the dictionary.
    /// </summary>
    /// <param name="index">Index of the target string.</param>
    public int GetStringCount( int index )
    {
      if( index == DEF_EMPTY_STRING_INDEX )
        return DEF_EMPTY_COUNT;

      Parse();

      return GetRefCount( index );
    }
    /// <summary>
    /// Returns RTF string by its index in the dictionary.
    /// </summary>
    /// <param name="index">Index of the target string.</param>
    /// <returns>RTF string.</returns>
    public TextWithFormat GetStringByIndex( int index )
    {
      if( index == DEF_EMPTY_STRING_INDEX )
        throw new NotImplementedException();
      //return EmptyString;

      return this[ index ];
    }
    /// <summary>
    /// Adds copy of the string from another collection.
    /// </summary>
    /// <param name="index">Index of the string to add.</param>
    /// <param name="sourceSST">Source string table.</param>
    /// <param name="dicFontIndexes">Dictionary with font indexes.</param>
    /// <returns>Index of the added string.</returns>
    public int AddCopy( int index, SSTDictionary sourceSST, Dictionary<int, int> dicFontIndexes )
    {
      if( sourceSST == null )
        throw new ArgumentNullException( "sourceSST" );

      Parse();
      sourceSST.Parse();

      object itemToAdd = sourceSST.m_arrStrings[ index ];
      TextWithFormat text = itemToAdd as TextWithFormat;

      if( text != null )
      {
        itemToAdd = text.Clone( dicFontIndexes );
      }

      return AddIncrease( itemToAdd, true );
    }
    /// <summary>
    /// Searches for all strings that starts with specified string.
    /// </summary>
    /// <param name="strStart">String prefix to search.</param>
    /// <returns>
    /// List with all string indexes that starts with specified
    /// string, in ascending order.
    /// </returns>
    public List<int> StartWith( string strStart )
    {
      if( strStart == null )
        throw new ArgumentNullException( "strStart" );

      if( strStart.Length == 0 )
        throw new ArgumentException( "strStart - string cannot be empty." );

      List<int> result = new List<int>();

      for( int i = 0, len = Count; i < len; i++ )
      {
        TextWithFormat text = this[ i ];

        if( text.Text.StartsWith( strStart ) )
        {
          result.Add( i );
        }
      }

      return result;
    }
    /// <summary>
    /// Creates copy of the current instance.
    /// </summary>
    /// <returns>Copy of the current instance.</returns>
    public object Clone( WorkbookImpl book )
    {
      SSTDictionary result = ( SSTDictionary )MemberwiseClone();
      result.m_book = book;
      result.m_hashKeyToIndex = CloneUtils.CloneHash( m_hashKeyToIndex );
      result.m_arrStrings = CloneUtils.CloneCloneable( m_arrStrings );
      //result.m_arrFreeIndexes = ( SortedListEx )m_arrFreeIndexes.Clone();
      if( m_arrFreeIndexes != null )
      {
        result.m_arrFreeIndexes = new SortedList<int, int>( m_arrFreeIndexes );
      }

      result.m_sstOriginal = ( SSTRecord )CloneUtils.CloneCloneable( m_sstOriginal );

      result.newRefCount = CloneUtils.CloneCloneable(newRefCount);
      //result.m_arrRefCount.EnsureCapacity( m_arrRefCount.Capacity );

      return result;
    }
    /// <summary>
    /// Updates internal array that stores reference count.
    /// </summary>
    public void UpdateRefCounts()
    {
      //int iBytesCount = Count * ExcelConstants.IntSize;
      //if( m_arrRefCount.Capacity < iBytesCount )
      //{
      //  m_arrRefCount.EnsureCapacity( iBytesCount );
      //  m_arrRefCount.ZeroMemory();
      //}
        int count = Count;
        for (int i = 0; i < count; i++)
        {
            this.newRefCount.Add(0);
        }

    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// This methods looks through all row storages to find out used strings.
    /// And removes all unnecessary strings after that.
    /// </summary>
    public void RemoveUnnecessaryStrings()
    {
      Parse();
      //m_arrRefCount.ZeroMemory();
      m_book.ReAddAllStrings();

      for( int i = 0, len = Count; i < len; i++ )
      {
        int iRefCount = GetRefCount( i );

        if( iRefCount == 0 )
        {
          object item = m_arrStrings[ i ];

          if( item != null )
          {
            if( m_bUseHash )
              m_hashKeyToIndex.Remove( item );

            m_arrStrings[ i ] = null;
            m_arrFreeIndexes[ i ] = i;
          }
        }
      }

      Defragment();
    }
    /// <summary>
    /// Moves strings.
    /// </summary>
    /// <param name="iStartIndex">Start index.</param>
    /// <param name="iEndIndex">End index.</param>
    /// <param name="iDecreaseValue">Decrease index.</param>
    /// <param name="arrNewIndexes">List with new strings indexes.</param>
    private void MoveStrings( int iStartIndex, int iEndIndex, int iDecreaseValue,
      List<int> arrNewIndexes )
    {
      for( int j = iStartIndex + 1; j < iEndIndex; j++ )
      {
        object entry = m_arrStrings[ j ];
        int iNewIndex = j - iDecreaseValue;

        m_arrStrings[ iNewIndex ] = entry;

        if( m_bUseHash )
          m_hashKeyToIndex[ entry ] = iNewIndex;

        arrNewIndexes[ j ] = iNewIndex;
      }
    }
    /// <summary>
    /// Removes all unused strings from inner collections.
    /// </summary>
    private void Defragment()
    {
      int iCount = Count;
      int iFreeCount = m_arrFreeIndexes.Count;

      if( iFreeCount > 0 )
      {
        // Some strings were removed, we need to update indexes.
        int iStartIndex = m_arrFreeIndexes.Values[ 0 ];

        List<int> arrNewIndexes = new List<int>( iCount + 1 );

        for( int i = 0; i < iCount; i++ )
        {
          arrNewIndexes.Add( i );
        }

        IList<int> arrValues = m_arrFreeIndexes.Values;
        for( int i = 1, len = m_arrFreeIndexes.Count; i < len; i++ )
        {
          int iEndIndex = arrValues[ i ];
          MoveStrings( iStartIndex, iEndIndex, i, arrNewIndexes );
          iStartIndex = iEndIndex;
        }

        MoveStrings( iStartIndex, iCount, m_arrFreeIndexes.Count, arrNewIndexes );

        iStartIndex = iCount - iFreeCount;
        m_arrStrings.RemoveRange( iStartIndex, iFreeCount );
        m_arrFreeIndexes.Clear();
        m_book.UpdateStringIndexes( arrNewIndexes );
      }
    }
    /// <summary>
    /// Saves into record.
    /// </summary>
    /// <param name="records">Array where save is.</param>
    private void SaveIntoRecords( OffsetArrayList records )
    {
      SSTRecord sst;
      int iCount;

      if( m_bParsed )
      {
        sst = ( SSTRecord )BiffRecordFactory.GetRecord( TBIFFRecord.SST );
        sst.Strings = Keys;
        iCount = m_arrStrings.Count;
        sst.NumberOfStrings = ( uint )iCount;
      }
      else
      {
        sst = m_sstOriginal;
        iCount = ( int )sst.NumberOfStrings;
      }

      records.Add( sst );

      const int DEF_MAX_ARRAY = 126;
      int optBucketsFill = iCount / DEF_MAX_ARRAY;
      optBucketsFill = ( optBucketsFill < 8 ) ? 8 : optBucketsFill;

      ExtSSTRecord extSST = ( ExtSSTRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtSST );
      extSST.StringPerBucket = ( ushort )optBucketsFill;
      extSST.SSTInfo = new ExtSSTInfoSubRecord[ sst.NumberOfUniqueStrings / optBucketsFill + 1 ];
      extSST.SST = sst;

      for( int i = 0; i < extSST.SSTInfo.Length; i++ )
      {
        extSST.SSTInfo[ i ] = new ExtSSTInfoSubRecord();
      }

      records.Add( extSST );
    }
    /// <summary>
    /// Sets number of references to the string.
    /// </summary>
    /// <param name="index">String index.</param>
    /// <param name="count">References count to set.</param>
    private void SetRefCount( int index, int count )
    {
        if (newRefCount.Count > index)
        {
            newRefCount[index] = count;
        }
        else
        {
            newRefCount.Add(count);
        }
    }
    /// <summary>
    /// Gets number of references to the string.
    /// </summary>
    /// <param name="index">String index.</param>
    /// <returns>number of references to the string.</returns>
    private int GetRefCount( int index )
    {
        if (newRefCount.Count > index)
            return newRefCount[index];
        else
            return 0;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private int Find( TextWithFormat key )
    {
      int iResult = -1;

      if( m_bUseHash )
      {
        if( m_hashKeyToIndex.ContainsKey( key ) )
        {
          iResult = ( int )m_hashKeyToIndex[ key ];
        }
        else if( key.FormattingRunsCount == 0 && m_hashKeyToIndex.ContainsKey( key.Text ) )
        {
          iResult = ( int )m_hashKeyToIndex[ key.Text ];
        }
      }
      else
      {
        for( int i = 0, len = m_arrStrings.Count; i < len; i++ )
        {
          object value = m_arrStrings[ i ];

          if( value == null )
            continue;

          if( key.FormattingRunsCount == 0 && value is string && key.Text == ( string )value
            || key.CompareTo( value ) == 0 )
          {
            iResult = i;
            break;
          }
        }
      }

      return iResult;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private int Find( object key )
    {
      int iResult = -1;

      if( m_bUseHash )
      {
        if( m_hashKeyToIndex.ContainsKey( key ) )
        {
          iResult = ( int )m_hashKeyToIndex[ key ];
        }
      }
      else
      {
        for( int i = 0, len = m_arrStrings.Count; i < len; i++ )
        {
          object value = m_arrStrings[ i ];

          if( value == null )
            continue;

          if( value.Equals( key ) )
          {
            iResult = i;
            break;
          }
        }
      }

      return iResult;
    }
    /// <summary>
    /// Fills internal hash used for string searching.
    /// </summary>
    private void FillHash()
    {
      for( int i = 0, len = m_arrStrings.Count; i < len; i++ )
      {
        object value = m_arrStrings[ i ];

        if( value == null )
          continue;

        m_hashKeyToIndex[ value ] = i;
      }
    }
    #endregion

    #region IParseable Members
    /// <summary>
    /// Here we have to parse our SST record
    /// </summary>
    public void Parse()
    {
      if( !m_bParsed )
      {
        Dictionary<int, int> hashUpdatedIndexes = new Dictionary<int, int>();
        object[] arrStrings = m_sstOriginal.Strings;

        for( int i = 0; i < arrStrings.Length; i++ )
        {
          int index = AddIncrease( arrStrings[ i ], false );

          if( i != index )
            hashUpdatedIndexes.Add( i, index );
        }

        if( hashUpdatedIndexes.Count > 0 )
        {
          UpdateLabelSSTIndexes( hashUpdatedIndexes );
        }

        m_bParsed = true;
        m_sstOriginal = null;
      }
    }
    /// <summary>
    /// Updates LabelSST indexes after SST record parsing.
    /// </summary>
    /// <param name="dictUpdatedIndexes">Dictionary with indexes to update, key - old index, value - new index.</param>
    internal void UpdateLabelSSTIndexes( Dictionary<int, int> dictUpdatedIndexes )
    {
      IWorksheets arrWorksheets = m_book.Worksheets;

      for( int i = 0, len = arrWorksheets.Count; i < len; i++ )
      {
        WorksheetImpl sheet = ( WorksheetImpl )arrWorksheets[ i ];
        sheet.UpdateLabelSSTIndexes( dictUpdatedIndexes, AddIncrease );
      }
    }
      /// <summary>
      /// Returns LabelSST used.
      /// </summary>
      /// <returns></returns>
    internal int GetLabelSSTCount()
    {
        int count = 0;
        if (m_bParsed)
        {
            foreach (KeyValuePair<object, int> values in m_hashKeyToIndex)
            {
                count += GetRefCount(values.Value);
            }
        }
        return count;
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if( m_book != null )
      {
        m_hashKeyToIndex = null;
        m_arrStrings = null;
        m_arrFreeIndexes = null;
        m_book = null;
        m_sstOriginal = null;
        m_tempString = null;
        newRefCount.Clear();
        newRefCount = null;
        GC.SuppressFinalize( this );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    ~SSTDictionary()
    {
      Dispose();
    }
    #endregion
  }
  public delegate void IncreaseIndex( int index );
}

