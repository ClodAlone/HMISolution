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
using System.Diagnostics;

using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections.Generic;
#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
	/// <summary>
	/// Represents collection of all extended formats in the workbook.
	/// </summary>
  public class ExtendedFormatsCollection : CollectionBaseEx<ExtendedFormatImpl>
	{
    #region Class constants
    /// <summary>
    /// Number of default extended formats.
    /// </summary>
    private const int DEF_DEFAULT_COUNT = 21;
    #endregion

    #region Class members
    /// <summary>
    /// Dictionary with all formats in the collection.
    /// Key - ExtendedFormatImpl, value - same format.
    /// </summary>
    private Dictionary<ExtendedFormatImpl, ExtendedFormatImpl> m_hashFormats = new Dictionary<ExtendedFormatImpl, ExtendedFormatImpl>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates collection and sets its Application and Parent values.
    /// </summary>
    /// <param name="application">
    /// Application object that represents the Excel application.
    /// </param>
    /// <param name="parent">Parent object of this collection.</param>
    public ExtendedFormatsCollection( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    public ExtendedFormatImpl this[ int index ]
    {
      get
      {
        if( index < 0 || index >= Count )
          throw new ArgumentOutOfRangeException( "index" );

        return ( ExtendedFormatImpl )InnerList[ index ];
      }
    }
    /// <summary>
    /// Returns parent workbook object. Read-only.
    /// </summary>
    public WorkbookImpl ParentWorkbook
    {
      get
      {
        return this[ 0 ].Workbook;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Adds format into collection.
    /// </summary>
    /// <param name="format">Format to add.</param>
    /// <returns>
    /// If there is same format in the collection, this method will return it;
    /// otherwise format that was passed as argument will be added.
    /// </returns>
    public ExtendedFormatImpl Add( ExtendedFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      bool bAdd = true;

      if( m_hashFormats.ContainsKey( format ) )
      {
        ExtendedFormatImpl originalFormat = format;
        format = m_hashFormats[ format ];

        if( ParentWorkbook.Version == ExcelVersion.Excel97to2003 && format.Index != 0 &&
          format.Index < RangeImpl.DEF_NORMAL_STYLE_INDEX )
        {
          format = originalFormat;
        }
        else
        {
          bAdd = false;
        }
      }
      else
      {
        int iCount = Count;

        WorkbookImpl book = ( iCount > 0 ) ?
          ParentWorkbook :
          format.Workbook;

        if( Count >= book.MaxXFCount )
        {
#if DEBUG && !(WINRT )
          DumpFormats();
          Console.WriteLine( this[ 80 ].Equals( this[ 95 ] ) );
#endif
          throw new ApplicationException( "Maximum number of extended formats exceeded." );
        }

        m_hashFormats.Add( format, format );
      }

      if( bAdd )
      {
        format.Index = ( ushort )List.Count;
        base.Add( format );

        if (!format.Workbook.m_xfCellCount.ContainsKey(format.Index))
        {
            format.Workbook.m_xfCellCount.Add(format.Index, 1);
        }
        else
        {
            int value;
            if (format.Workbook.m_xfCellCount.TryGetValue(format.Index, out value))
            {
                format.Workbook.m_xfCellCount[format.Index] = value + 1;
            }
        }
      }
      else
      {
          if (format.Workbook.m_xfCellCount.ContainsKey(format.Index))
          {
              int value;
              if (format.Workbook.m_xfCellCount.TryGetValue(format.Index, out value))
              {
                  format.Workbook.m_xfCellCount[format.Index]=value+1;
              }              
          }
      }

      return format;
    }
    /// <summary>
    /// Adds format into collection.
    /// </summary>
    /// <param name="format">Format to add.</param>
    /// <returns>Format that was added.</returns>
    public ExtendedFormatImpl ForceAdd( ExtendedFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( !m_hashFormats.ContainsKey( format ) )
      {
        m_hashFormats.Add( format, format );
      }

      format.Index = ( ushort )List.Count;
      base.Add( format );
      return format;
    }
    /// <summary>
    /// Imports single extended format to the collection.
    /// </summary>
    /// <param name="format">Format to import.</param>
    /// <param name="hashExtFormatIndexes">
    /// Dictionary with new xf indexes, key - old xf index, value - new xf index.
    /// </param>
    /// <returns>Index of the new format.</returns>
    public int Import( ExtendedFormatImpl format, Dictionary<int, int> hashExtFormatIndexes )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( hashExtFormatIndexes == null )
        throw new ArgumentNullException( "hashExtFormatIndexes" );

      int iFormatIndex = format.Index;
      int iCount = Count;

      if( iCount > iFormatIndex && this[ iFormatIndex ] == format )
      {
        return iFormatIndex;
      }

      ExtendedFormatImpl newFormat = format.TypedClone( this );
      int iParent = newFormat.ParentIndex;

      if( hashExtFormatIndexes.ContainsKey( iParent ) )
      {
        iParent = hashExtFormatIndexes[ iParent ];
      }

      newFormat.ParentIndex = iParent;
      newFormat = Add( newFormat );

      return newFormat.Index;
    }
    /// <summary>
    /// Merges extended formats collection with formats from another collection.
    /// </summary>
    /// <param name="arrXFormats">Collection to get formats from.</param>
    /// <param name="dicFontIndexes">Container for new font indexes.</param>
    /// <returns>Dictionary with new indexes.</returns>
    public Dictionary<int, int> Merge( IList<ExtendedFormatImpl> arrXFormats, out Dictionary<int, int> dicFontIndexes )
    {
      if( arrXFormats == null )
        throw new ArgumentNullException( "arrXFormats" );

      dicFontIndexes = null;

      if( arrXFormats == this ) return null;

      int iCount = arrXFormats.Count;

      if( iCount == 0 ) return null;

      int[] arrResult = new int[ iCount ];
      bool[] arrUsed = new bool[ iCount ];

      ExtendedFormatImpl extFormat = ( ExtendedFormatImpl )arrXFormats[ 0 ];
      WorkbookImpl sourceBook = extFormat.Workbook;
      WorkbookImpl destBook = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( destBook == null )
        throw new ArgumentNullException( "Can't find destination workbook." );

      // 1. Copy all necessary fonts and update its indexes.
      dicFontIndexes = destBook.InnerFonts.AddRange( sourceBook.InnerFonts );

      // 2. Copy all necessary formats and update its indexes.
      Dictionary<int, int> hashFormatIndexes = destBook.InnerFormats.Merge( sourceBook.InnerFormats );

      return Merge( arrXFormats, dicFontIndexes, hashFormatIndexes );
    }
    /// <summary>
    /// Adds extended formats to the collection.
    /// </summary>
    /// <param name="arrXFormats">Formats to add.</param>
    /// <returns>Dictionary with new indexes.</returns>
    public Dictionary<int, int> Merge( IList<ExtendedFormatImpl> arrXFormats )
    {
      if( arrXFormats == null )
        throw new ArgumentNullException( "arrXFormats" );

      Dictionary<int, int> hashFontIndexes = GetFontIndexes( arrXFormats );
      Dictionary<int, int> hashFormatIndexes = GetFormatIndexes( arrXFormats );
      return Merge( arrXFormats, hashFontIndexes, hashFormatIndexes );
    }
    /// <summary>
    /// Adds index to the collection.
    /// </summary>
    /// <param name="hashToAdd">Dictionary to add index to.</param>
    /// <param name="arrXFormats">List with extended formats.</param>
    /// <param name="index">Index to add.</param>
    public void AddIndex( Dictionary<int, object> hashToAdd, IList<ExtendedFormatImpl> arrXFormats, int index )
    {
      if( hashToAdd == null )
        throw new ArgumentNullException( "hashToAdd" );

      if( arrXFormats == null )
        throw new ArgumentNullException( "arrXFormats" );

      if( !hashToAdd.ContainsKey( index ) )
      {
        hashToAdd.Add( index, null );
        ExtendedFormatImpl format = this[ index ];
        arrXFormats.Add( format );

        if( format.HasParent )
        {
          AddIndex( hashToAdd, arrXFormats, format.ParentIndex );
        }
      }
    }
    /// <summary>
    /// Gather two formats borders.
    /// </summary>
    /// <param name="iFirstXF">First xf indexes to gathered.</param>
    /// <param name="iEndXF">End xf indexes to gathered.</param>
    /// <returns>Returns gathered two extended formats..</returns>
    public ExtendedFormatImpl GatherTwoFormats( int iFirstXF, int iEndXF )
    {
      if( iFirstXF >= Count || iFirstXF < 0 )
        throw new ArgumentOutOfRangeException( "iFirstXF" );

      if( iEndXF >= Count || iEndXF < 0 )
        throw new ArgumentOutOfRangeException( "iEndXF" );

      if( iFirstXF == iEndXF )
        return ( ExtendedFormatImpl )this[ iFirstXF ].Clone();

      ExtendedFormatImpl firstFormat = this[ iFirstXF ];
      ExtendedFormatImpl endFormat = this[ iEndXF ];
      ExtendedFormatImpl result = ( ExtendedFormatImpl )firstFormat.Clone();

      IBorder rightBorder;
      IBorder bottomBorder;

      if( endFormat.IncludeBorder )
      {
        rightBorder = endFormat.Borders[ ExcelBordersIndex.EdgeRight ];
        bottomBorder = endFormat.Borders[ ExcelBordersIndex.EdgeBottom ];
      }
      else
      {
        ExtendedFormatImpl endParentFormat = this[ endFormat.ParentIndex ];

        rightBorder = endParentFormat.Borders[ ExcelBordersIndex.EdgeRight ];
        bottomBorder = endParentFormat.Borders[ ExcelBordersIndex.EdgeBottom ];
      }

      IBorder border = result.Borders[ ExcelBordersIndex.EdgeRight ];

      border.ColorObject.CopyFrom( rightBorder.ColorObject, true );
      border.LineStyle = rightBorder.LineStyle;

      border = result.Borders[ ExcelBordersIndex.EdgeBottom ];

      border.ColorObject.CopyFrom( bottomBorder.ColorObject, true );
      border.LineStyle = bottomBorder.LineStyle;

      return result;
    }
    /// <summary>
    /// Removes item at the specified position.
    /// </summary>
    /// <param name="xfIndex">Item to remove.</param>
    /// <returns>
    /// Dictionary with new xf indexes.
    /// key - old index
    /// value - new index.
    /// Only those formats that had parent index set
    /// to xfIndex will be placed in this dictionary.
    /// </returns>
    new public Dictionary<int, int> RemoveAt( int xfIndex )
    {
      int iCount = Count;

      if( xfIndex < 0 || xfIndex > iCount )
        return null;

      SortedList<int, ExtendedFormatImpl> dictIndexToFormat = new SortedList<int, ExtendedFormatImpl>();

      ExtendedFormatImpl format = this[ xfIndex ];
      m_hashFormats.Remove( format );
      //base.RemoveAt( xfIndex );
      Dictionary<int, int> dictResult = new Dictionary<int, int>();

      InnerList[ xfIndex ] = null;
      dictResult[ xfIndex ] = 0;

      // Step 1. Find all child extended formats.
      for( int i = 0; i < iCount; i++ )
      {
        format = this[ i ];

        if( format == null ) continue;

        int iParentIndex = format.ParentIndex;

        if( iParentIndex == xfIndex )
        {
          m_hashFormats.Remove( format );
          format.ParentIndex = 0;
          format.SynchronizeWithParent();
          dictIndexToFormat.Add( i, null );
        }
      }

      // Step 2. Check whether collection contains same extended formats as child xf.
      // If yes then one of them should be removed.
      int iUpdateCount = dictIndexToFormat.Count;
      IList<int> keys = dictIndexToFormat.Keys;

      for( int i = 0; i < iUpdateCount; i++ )
      {
        int index = keys[ i ];
        format = this[ index ];

        if( format == null )
          continue;

        ExtendedFormatImpl curFormat;

        if( m_hashFormats.TryGetValue( format, out curFormat ) )
        {
          dictIndexToFormat[ index ] = curFormat;
          InnerList[ index ] = null;
          //base.RemoveAt( index );
        }
        else
        {
          m_hashFormats.Add( format, format );
          dictIndexToFormat[ index ] = format;
        }
      }

      iCount = Count;
      int iRemoveCount = 0;

      for( int i = 0; i < iCount; i++ )
      {
        format = this[ i ];

        if( format != null )
        {
          int iIndex = i - iRemoveCount;
          dictResult.Add( i, iIndex );
          format.Index = ( ushort )iIndex;
        }
        else
        {
          iRemoveCount++;
        }
      }

      keys = dictIndexToFormat.Keys;
      for( int i = 0; i < iUpdateCount; i++ )
      {
        int index = keys[ i ];
        format = dictIndexToFormat[ index ];

        //if( index >= xfIndex ) index;//++;= iRemoveCount;
        dictResult[ index ] = ( int )format.Index;
      }

      // Step 3. Update parent indexes and indexes of all extended formats including hashtable.
      for( int i = 0; i < iCount; i++ )
      {
        format = this[ i ];

        if( format != null )
        {
          int iParentIndex = format.ParentIndex;

          if( iParentIndex != ExtendedFormatImpl.DEF_NO_PARENT_INDEX && dictResult.ContainsKey( i ) )
          {
            format.ParentIndex = ( ushort )dictResult[ iParentIndex ];
          }
        }
      }

      for( int i = 0; i < iCount; i++ )
      {
        format = this[ i ];

        if( format == null )
        {
          InnerList.RemoveAt( i );
          iCount--;
          i--;
        }
      }

      return dictResult;
    }
    /// <summary>
    /// Creates copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for the new collection.</param>
    /// <returns>Copy of the current instance.</returns>
    public override object Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ExtendedFormatsCollection result = ( ExtendedFormatsCollection )base.Clone( parent );

      List<ExtendedFormatImpl> list = result.InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        ExtendedFormatImpl format = list[ i ];

        if( !m_hashFormats.ContainsKey( format ) )
        {
          m_hashFormats.Add( format, format );
        }

        format.Index = i;
      }

      return result;
    }
    /// <summary>
    /// Sets maximum possible count of extended formats in the collections.
    /// Updates ParentIndex and overall number of extended formats if necessary.
    /// </summary>
    /// <param name="maxCount">New value of maximum possible extended formats.</param>
    public void SetMaxCount( int maxCount )
    {
      if( Count > 0 )
      {
        WorkbookImpl book = this[ 0 ].Workbook;
        int iCurrentMax = book.MaxXFCount;
        List<ExtendedFormatImpl> list = InnerList;
        int iCount = list.Count;

        if( iCount >= maxCount )
        {
          list.RemoveRange( maxCount - 1, iCount - maxCount );
        }

        for( int i = 0, len = Count; i < len; i++ )
        {
          ExtendedFormatImpl format = list[ i ];

          if( format.ParentIndex == iCurrentMax )
          {
            format.ParentIndex = maxCount;
          }
        }

        book.UpdateXFIndexes( maxCount );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iXFIndex"></param>
    /// <param name="format"></param>
    internal void SetXF( int iXFIndex, ExtendedFormatImpl format )
    {
      if( iXFIndex >= Count )
        throw new ArgumentOutOfRangeException( "iXFIndex" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      ExtendedFormatImpl existingFormat = this[ iXFIndex ];
      m_hashFormats.Remove( existingFormat );
      InnerList[ iXFIndex ] = format;
      m_hashFormats.Add( format, format );
    }
    /// <summary>
    /// Adds copy of the format to the collection.
    /// </summary>
    /// <param name="arrXFormats">Formats to add.</param>
    /// <param name="dicFontIndexes">Updated font indexes.</param>
    /// <param name="dicFormatIndexes">Dictionary with new format indexes.</param>
    /// <returns>Dictionary with new indexes.</returns>
    private Dictionary<int, int> Merge( IList<ExtendedFormatImpl> arrXFormats,
      Dictionary<int, int> dicFontIndexes, Dictionary<int, int> dicFormatIndexes )
    {
      if( arrXFormats == null )
        throw new ArgumentNullException( "arrXFormats" );

      if( dicFontIndexes == null )
        throw new ArgumentNullException( "dicFontIndexes" );

      if( dicFormatIndexes == null )
        throw new ArgumentNullException( "dicFormatIndexes" );

      int iCount = arrXFormats.Count;
      Dictionary<int, int> hashResult = new Dictionary<int, int>( iCount );

      for( int i = 0; i < iCount; i++ )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, i, "Merging extended format" );

        ExtendedFormatImpl format = ( ExtendedFormatImpl )arrXFormats[ i ];
        int iFormatIndex = format.Index;

        if( !hashResult.ContainsKey( iFormatIndex ) )
        {
          Merge( format, hashResult, dicFontIndexes, dicFormatIndexes );
        }
      }

      return hashResult;
    }
    /// <summary>
    /// Adds copy of the format to the collection.
    /// </summary>
    /// <param name="format">Format to add.</param>
    /// <param name="hashResult">Dictionary with new format indexes.</param>
    /// <param name="dicFontIndexes">Updated font indexes, key - old index, value - new index.</param>
    /// <param name="dicFormatIndexes">Dictionary with new format indexes.</param>
    private void Merge( ExtendedFormatImpl format, Dictionary<int, int> hashResult,
      Dictionary<int, int> dicFontIndexes, Dictionary<int, int> dicFormatIndexes )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( hashResult == null )
        throw new ArgumentNullException( "hashResult" );

      ExtendedFormatsCollection arrXFormats = format.ParentCollection;
      int iParentIndex = format.ParentIndex;

      if( format.HasParent )
      {
        if( !hashResult.ContainsKey( iParentIndex ) )
        {
          Merge( arrXFormats[ iParentIndex ], hashResult, dicFontIndexes, dicFormatIndexes );
        }

        iParentIndex = hashResult[ iParentIndex ];
      }

      int iXFormatIndex = format.Index;
      ExtendedFormatImpl newFormat = format.TypedClone( this );

      int iNumberFormat = format.Record.FormatIndex;
      int iFontIndex = format.Record.FontIndex;

      if( format.HasParent )
        newFormat.ParentIndex = iParentIndex;

      if (iFontIndex >= dicFontIndexes.Count)
          iFontIndex = 0;

      newFormat.Record.FontIndex = ( ushort )dicFontIndexes[ iFontIndex ];

      if( dicFormatIndexes != null && dicFormatIndexes.ContainsKey( iNumberFormat ) )
      {
        iNumberFormat = dicFormatIndexes[ iNumberFormat ];
        newFormat.Record.FormatIndex = ( ushort )iNumberFormat;
      }

      newFormat = Add( newFormat );
      hashResult.Add( iXFormatIndex, newFormat.Index );
    }
    /// <summary>
    /// Returns array with new font indexes used by specified extended formats.
    /// </summary>
    /// <param name="arrXFormats">List with used extended formats.</param>
    /// <returns>Updated font indexes, key - old font index, value - new font index.</returns>
    private Dictionary<int, int> GetFontIndexes( IList<ExtendedFormatImpl> arrXFormats )
    {
      if( arrXFormats == null )
        throw new ArgumentNullException( "arrXFormats" );

      Dictionary<int, object> hashFonts = new Dictionary<int, object>();

      // First of all, we have to find out what fonts were
      // used in those extended formats.
      for( int i = 0, len = arrXFormats.Count; i < len; i++ )
      {
        ExtendedFormatImpl format = arrXFormats[ i ];
        hashFonts[ format.FontIndex ] = -1;
      }

      // Now we have to add all those fonts into fonts collection.
      ExtendedFormatImpl extFormat = arrXFormats[ 0 ];
      WorkbookImpl sourceBook = extFormat.Workbook;

      WorkbookImpl book = ( WorkbookImpl )FindParent( typeof( WorkbookImpl ) );
      FontsCollection arrFonts = book.InnerFonts;

      return arrFonts.AddRange( hashFonts.Keys, sourceBook.InnerFonts );
    }
    /// <summary>
    /// Returns hashtable with new format indexes used by specified extended formats.
    /// </summary>
    /// <param name="arrXFormats">List with used extended formats.</param>
    /// <returns>Dictionary with new format indexes, key - old format index, value - new format index.</returns>
    private Dictionary<int, int> GetFormatIndexes( IList<ExtendedFormatImpl> arrXFormats )
    {
      if( arrXFormats == null )
        throw new ArgumentNullException( "arrXFormats" );

      int iCount = arrXFormats.Count;

      Dictionary<int, int> hashFormats = new Dictionary<int, int>();

      if( iCount == 0 )
        return hashFormats;

      // First of all, we have to find out what number formats were
      // used in those extended formats.
      for( int i = 0, len = arrXFormats.Count; i < len; i++ )
      {
        ExtendedFormatImpl format = arrXFormats[ i ];
        hashFormats[ format.NumberFormatIndex ] = -1;
      }

      // Now we have to add all those formats into formats collection.
      ExtendedFormatImpl extFormat = arrXFormats[ 0 ];
      WorkbookImpl sourceBook = extFormat.Workbook;

      WorkbookImpl book = ( WorkbookImpl )FindParent( typeof( WorkbookImpl ) );
      FormatsCollection arrFormats = book.InnerFormats;

      return arrFormats.AddRange( hashFormats, sourceBook.InnerFormats );
    }
    protected override void OnClearComplete()
    {
        m_hashFormats.Clear();
    }
    
#if DEBUG
#if !(WINRT )
    /// <summary>
    /// Dumps all formats into file on the disk.
    /// </summary>
    public void DumpFormats()
    {
      using( System.IO.StreamWriter streamWriter = new System.IO.StreamWriter( "D:\\XF.txt" ) )
      {
        using( ByteArrayDataProvider provider = new ByteArrayDataProvider( new byte[ 100 ] ) )
        {
          for( int i = 0, len = Count; i < len; i++ )
          {
            streamWriter.WriteLine( "{0}------------------------", i );
            ExtendedFormatImpl format = this[ i ];
            ExtendedFormatRecord record = format.Record;
            record.InfillInternalData( provider, 0, ExcelVersion.Excel97to2003 );
            DumpArray( streamWriter, provider, record.Length );
            //DumpBorders( streamWriter, format.Borders );
            //DumpColors( streamWriter, format );
          }
        }
      }
    }
#endif
    private void DumpArray( System.IO.StreamWriter streamWriter, ByteArrayDataProvider provider, int p )
    {
      System.Text.StringBuilder builder = new System.Text.StringBuilder();
      for( int i = 0; i < p; i++ )
      {
        byte data = provider.ReadByte( i );
        builder.Append( data.ToString( "X" ) );
        builder.Append( " " );
      }

      streamWriter.WriteLine( builder.ToString() );
    }

    private void DumpColors( System.IO.StreamWriter streamWriter, ExtendedFormatImpl format )
    {
      //throw new Exception( "The method or operation is not implemented." );
      streamWriter.WriteLine( "ColorIndex {0}", format.ColorIndex );
      streamWriter.WriteLine( "PatternColorIndex {0}", format.PatternColorIndex );
    }

    private void DumpBorders( System.IO.StreamWriter streamWriter, IBorders borders )
    {
      DumpBorder( streamWriter, borders[ ExcelBordersIndex.EdgeTop ] );
      DumpBorder( streamWriter, borders[ ExcelBordersIndex.EdgeBottom ] );
      DumpBorder( streamWriter, borders[ ExcelBordersIndex.EdgeLeft ] );
      DumpBorder( streamWriter, borders[ ExcelBordersIndex.EdgeRight ] );
    }

    private void DumpBorder( System.IO.StreamWriter streamWriter, IBorder border )
    {
      streamWriter.WriteLine( "ColorIndex {0}", border.Color );
      streamWriter.WriteLine( "LineStyle {0}", border.LineStyle );
    }
#endif
    #endregion


    internal void Dispose()
    {
        foreach (ExtendedFormatImpl extendedFormatImpl in this.InnerList)
        {
            extendedFormatImpl.clearAll();
        }

    }
    }
}
