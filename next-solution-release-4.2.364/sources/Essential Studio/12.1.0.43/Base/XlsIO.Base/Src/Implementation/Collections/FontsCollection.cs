#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.Collections;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Summary description for FontsCollection.
  /// </summary>
  public class FontsCollection
    : CollectionBaseEx<FontImpl>
  {
    #region Class members
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// 
    /// </summary>
    private Dictionary<FontImpl, FontImpl> m_hashFonts = new Dictionary<FontImpl, FontImpl>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates collection with specified Application and Parent.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public FontsCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single font from collection.
    /// </summary>
    public IFont this[ int index ]
    {
      get
      {
        if( index < 0 || index >= InnerList.Count )
          throw new ArgumentOutOfRangeException( "index", "Index is out of range." );

        return ( IFont )InnerList[ index ];
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Parses font record and adds new font to the collection.
    /// </summary>
    /// <param name="record">Font record to parse.</param>
    /// <returns>Added font.</returns>
    [ CLSCompliant( false ) ]
    public FontImpl Add( FontImpl font, FontRecord record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      FontImpl newFont = AppImplementation.CreateFont(this, font);
      return ( FontImpl )Add( newFont );
    }

    /// <summary>
    /// Adds font to collection.
    /// </summary>
    /// <param name="font">Font to add.</param>
    /// <returns>Added font.</returns>
    public IFont Add( IFont font )
    {
      FontImpl fontImpl = font as FontImpl;

      if( fontImpl == null )
        throw new ArgumentException( "Can't add font." );

      if( m_hashFonts.ContainsKey( fontImpl ) )
      {
        return m_hashFonts[ fontImpl ];
      }
      else
      {
        ForceAdd( fontImpl );
        return fontImpl;
      }
    }

    /// <summary>
    /// Inserts default fonts into collection.
    /// </summary>
    public void InsertDefaultFonts()
    {
      FontRecord font = ( FontRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Font );
      font.FontName = AppImplementation.StandardFont;
      font.FontHeight = ( ushort )FontImpl.SizeInTwips( AppImplementation.StandardFontSize );
      ForceAdd( AppImplementation.CreateFont( this, font ) );
      
      font = ( FontRecord )font.Clone();
      ForceAdd( AppImplementation.CreateFont( this, font ) );
      
      font = ( FontRecord )font.Clone();
      ForceAdd( AppImplementation.CreateFont( this, font ) );
      
      font = ( FontRecord )font.Clone();
      ForceAdd( AppImplementation.CreateFont( this, font ) );

      InnerList.Add( InnerList[ 0 ] );
    }

    /// <summary>
    /// Searches for all necessary parents.
    /// </summary>
    private void SetParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Can't find parent workbook." );
    }

    /// <summary>
    /// Forces add to the collection even if same font is already in the collection.
    /// </summary>
    /// <param name="font">Font to add.</param>
    public void ForceAdd( FontImpl font )
    {
      if( InnerList.Count == FontImpl.DEF_BAD_INDEX )
        InnerList.Add( InnerList[ 0 ] );

      font.Index = InnerList.Count;
      InnerList.Add( font );

      if( !m_hashFonts.ContainsKey( font ) )
      {
        m_hashFonts.Add( font, font );
      }
    }
    
    /// <summary>
    /// Saves fonts collection as a set of biff records.
    /// </summary>
    /// <param name="records">OffssetArrayList that will receive biff records.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      for( int i = 0, len = InnerList.Count; i < len; i++ )
      {
        if( i == FontImpl.DEF_BAD_INDEX )
          continue;

        FontImpl font = ( FontImpl )InnerList[ i ];
        font.Serialize( records );
      }
    }

    /// <summary>
    /// Indicates whether such font is in collection.
    /// </summary>
    /// <param name="font">Font to search.</param>
    /// <returns>True if such font is present in the collection, false otherwise.</returns>
    public bool Contains( FontImpl font )
    {
      return m_hashFonts.ContainsKey( font );
    }
    /// <summary>
    /// Merges fonts with fonts from another fonts collection.
    /// </summary>
    /// <param name="arrFonts">Fonts collection to merge fonts from..</param>
    /// <returns>Dictionary with updated indexes.</returns>
    public Dictionary<int, int> AddRange( FontsCollection arrFonts )
    {
      if( arrFonts == null )
        throw new ArgumentNullException( "arrFonts" );

      if( arrFonts == this ) return null;

    Dictionary<int, int> hashResult = new Dictionary<int, int>();

      for( int i = 0, len = arrFonts.Count; i < len; i++ )
      {
        if( i == FontImpl.DEF_BAD_INDEX )
        {
          hashResult.Add( i, i );
          continue;
        }

        FontImpl font = arrFonts[ i ] as FontImpl;
        int iNewIndex = AddCopy( font );
        hashResult.Add( i, iNewIndex );
      }

      return hashResult;
    }
    /// <summary>
    /// Adds fonts range from other fonts collection.
    /// </summary>
    /// <param name="colFonts">Collection with font indexes to add.</param>
    /// <param name="sourceFonts">Collection with fonts to add.</param>
    /// <returns>Dictionary with updated fonts indexes: key - old font index, value - new font index.</returns>
    public Dictionary<int, int> AddRange( ICollection<int> colFonts, FontsCollection sourceFonts )
    {
      if( colFonts == null )
        throw new ArgumentNullException( "colFonts" );

      if( sourceFonts == null )
        throw new ArgumentNullException( "sourceFonts" );

      Dictionary<int, int> result = new Dictionary<int, int>();

      foreach( int iFontIndex in colFonts )
      {
        FontImpl font = ( FontImpl )sourceFonts[ iFontIndex ];
        int iOldIndex = font.Index;
        int iNewIndex = AddCopy( font );
        result.Add( iOldIndex, iNewIndex );
      }

      return result;
    }
    /// <summary>
    /// Adds copy of the font.
    /// </summary>
    /// <param name="font">Font to copy.</param>
    /// <returns>Index of the new font.</returns>
    private int AddCopy( FontImpl font )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      font = Add( font, font.Record );
      return font.Index;
    }
    protected override void OnClearComplete()
    {
        m_hashFonts.Clear();
    }  
    /// <summary>
    /// Creates copy of the fonts collection.
    /// </summary>
    /// <param name="parent">Parent workbook for the new collection.</param>
    /// <returns>Copy of this collection.</returns>
    public FontsCollection Clone( WorkbookImpl parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      FontsCollection result = new FontsCollection( Application, parent );//( FontsCollection )MemberwiseClone();
      //result.SetParent( parent );
      //m_book = parent;

      List<FontImpl> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        FontImpl font = list[ i ];
        font = font.Clone( result );

        if( i != FontImpl.DEF_BAD_INDEX )
          result.ForceAdd( font );
      }

      return result;
    }
    #endregion

    internal void Dispose()
    {
        foreach (FontImpl fontImpl in this.InnerList)
        {
            fontImpl.Clear();
        }
    }
  }
}
