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
using System.Collections.Generic;

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Collection of hyperlinks.
  /// </summary>
  public class HyperLinksCollection
    : CollectionBaseEx<HyperLinkImpl>
    , IHyperLinks
    , ICloneParent
  {
    #region Class members
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Indicates whether collection is in read-only mode.
    /// </summary>
    private bool m_bReadOnly;
    /// <summary>
    /// Cell to index dictionary: key - cell index, value - List of hyperlinks.
    /// </summary>
    internal Dictionary<long, List<HyperLinkImpl>> m_dicCellToList = new Dictionary<long, List<HyperLinkImpl>>();
    /// <summary>
    /// Hyperlinks list for Read-only mode.
    /// </summary>
    private List< IHyperLink > m_listHyperlinks = new List< IHyperLink >();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates collection and sets its Application and Parent values.
    /// </summary>
    /// <param name="application">
    /// Application object that represents the Excel application.
    /// </param>
    /// <param name="parent">Parent object of this collection.</param>
    public HyperLinksCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
      CreateHyperlinkStyles();
      this.Removed += new CollectionChange(HyperLinksCollection_Removed);
    }

    /// <summary>
    /// Creates collection and sets its Application and Parent values.
    /// </summary>
    /// <param name="application">
    /// Application object that represents the Excel application.
    /// </param>
    /// <param name="parent">Parent object of this collection.</param>
    /// <param name="isReadOnly">Indicates whether read-only collection should be created.</param>
    public HyperLinksCollection( IApplication application, object parent, bool isReadOnly )
      : this( application, parent )
    {
      m_bReadOnly = isReadOnly;

      if( !m_bReadOnly )
      {
        m_listHyperlinks = new List<IHyperLink>();
      }

      SetParents();
      CreateHyperlinkStyles();
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

    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    public IHyperLink this[ int index ]
    {
      get
      {
        if( index < 0 || index > Count - 1 )
          throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than Count - 1" );

        return ( IHyperLink )List[ index ];
      }
    }
    /// <summary>
    /// Indicates whether collection is in read-only mode. Read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return m_bReadOnly;
      }
    }
    #endregion

    #region Interface methods
    /// <summary>
    /// Defines a new hyperlink.
    /// </summary>
    /// <param name="range">Name object to add.</param>
    public IHyperLink Add( IRange range )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      if( m_bReadOnly )
        throw new ReadOnlyException();

      HyperLinkImpl link = AppImplementation.CreateHyperLink( this, range );
      Add( link );
      AddToHash( link );

      return link;
    }

    /// <summary>
    /// Removes item at the specified index.
    /// </summary>
    /// <param name="index">Item index to remove.</param>
    new public void RemoveAt( int index )
    {
      if( index < 0 || index > Count - 1 )
        throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than Count - 1" );

      if( m_bReadOnly )
        throw new ReadOnlyException();

      base.RemoveAt( index );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Adds new hyperlink to the collection.
    /// </summary>
    /// <param name="link">Hyperlink to add.</param>
    /// <returns>Index in the collection of the new hyperlink.</returns>
    public int Add( IHyperLink link )
    {
      if( link == null )
        throw new ArgumentNullException( "link" );

      if( m_bReadOnly )
      {
        if( m_listHyperlinks.Contains( link ) )
          return -1;

        m_listHyperlinks.Add( link );
      }

      base.Add( link as HyperLinkImpl );
      return Count - 1;
    }

    /// <summary>
    /// Creates collection from IList.
    /// </summary>
    /// <param name="data">IList with necessary Biff records.</param>
    /// <param name="iPos">Start offset in the array.</param>
    /// <returns>Index of the first record after hyperlinks records.</returns>
    public int Parse( IList data, int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( iPos < 0 || iPos > data.Count - 1 )
        throw new ArgumentOutOfRangeException( "iPos", "Value cannot be less than 0 and greater than data.Count - 1" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];

      while( record.TypeCode == TBIFFRecord.HLink )
      {
        HyperLinkImpl link = new HyperLinkImpl( Application, this, data, ref iPos );
        Add( link );
        AddToHash( link );
        record = ( BiffRecordRaw )data[ iPos ];
      }

      return iPos;
    }
    /// <summary>
    /// Saves collection into list of BiffRecords.
    /// </summary>
    /// <param name="records">OffsetArrayList with BiffRecords.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If records parameter is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      for( int i = 0, len = Count; i < len; i++ )
      {
        HyperLinkImpl link = ( HyperLinkImpl )List[ i ];
        link.Serialize( records );
      }
    }
    /// <summary>
    /// Creates hyperlink styles if necessary.
    /// </summary>
    public void CreateHyperlinkStyles()
    {
      if( !m_book.InnerStyles.ContainsName( HyperLinkImpl.DEF_STYLE_NAME ) )
      {
        StyleImpl style = m_book.InnerStyles.CreateBuiltInStyle( HyperLinkImpl.DEF_STYLE_NAME );
        IFont font = style.Font;
        font.Underline = ExcelUnderline.Single;
        font.Color = ExcelKnownColors.BlueCustom;
      }
    }    
    /// <summary>
    /// Gets collection of hyperlinks for the specified range.
    /// </summary>
    /// <param name="range">Range to get hyperlinks for.</param>
    /// <returns>Collection of hyperlinks for the specified range.</returns>
    public HyperLinksCollection GetRangeHyperlinks( IRange range )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      if( m_bReadOnly )
        throw new NotSupportedException( "This operation is not supported for read-only Hyperlinks collections" );

      HyperLinksCollection result = new HyperLinksCollection( Application, range, true );
      int iStartRow = range.Row;
      int iStartCol = range.Column;
      int iLastRow = range.LastRow;
      int iLastCol = range.LastColumn;

      for( int iRow = iStartRow; iRow <= iLastRow; iRow++ )
      {
        for( int iCol = iStartCol; iCol <= iLastCol; iCol++ )
        {
          long index = RangeImpl.GetCellIndex( iCol, iRow );
          List<HyperLinkImpl> arrLinks;
          bool bGetValue = m_dicCellToList.TryGetValue( index, out arrLinks );

          if( bGetValue && arrLinks.Count > 0 )
          {
            result.AddRange( arrLinks );
          }
        }
      }

      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="link"></param>
    public void AddToHash( HyperLinkImpl link )
    {
      if( link == null )
        throw new ArgumentNullException( "link" );

      int iStartRow = link.FirstRow + 1;
      int iStartCol = link.FirstColumn + 1;
      int iEndRow = link.LastRow + 1;
      int iEndCol = link.LastColumn + 1;

      for( int iRow = iStartRow; iRow <= iEndRow; iRow++ )
      {
        for( int iCol = iStartCol; iCol <= iEndCol; iCol++ )
        {
          long index = RangeImpl.GetCellIndex( iCol, iRow );

          List<HyperLinkImpl> arrLinks;
          if( !m_dicCellToList.TryGetValue( index, out arrLinks ) )
          {
            arrLinks = new List<HyperLinkImpl>();
            m_dicCellToList[ index ] = arrLinks;
          }

          arrLinks.Add( link );
        }
      }
    }  
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    public void AddRange( IList<HyperLinkImpl> collection )
    {
      if( collection == null )
        throw new ArgumentNullException( "collection" );

      foreach( HyperLinkImpl link in collection )
      {
        Add( link );
      }
    }
    /// <summary>
    /// Gets last hyperlink by cell index.
    /// </summary>
    /// <param name="lCellIndex">Cell index.</param>
    /// <returns>Returns Hyper link if contain; otherwise null.</returns>
    public IHyperLink GetHyperlinkByCellIndex( long lCellIndex )
    {
      List<HyperLinkImpl> list;
      bool bGetValue = m_dicCellToList.TryGetValue( lCellIndex, out list );

      if( bGetValue && list.Count > 0 )
      {
        return list[ list.Count - 1 ] as IHyperLink;
      }

      return null;
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      HyperLinksCollection result = ( HyperLinksCollection )base.Clone( parent );
      result.m_bReadOnly = m_bReadOnly;

      foreach( KeyValuePair< long, List<HyperLinkImpl> > keyValue in m_dicCellToList )
      {
        List<HyperLinkImpl> value = keyValue.Value;
        value = CloneUtils.CloneCloneable( value, result );

        result.m_dicCellToList.Add( keyValue.Key, value );
      }

      return result;
    }
    internal void ClearAll()
    {
        m_dicCellToList.Clear();
        m_listHyperlinks.Clear();
    }
    #endregion

    #region Class event handlers
    private void HyperLinksCollection_Removed( object sender, CollectionChangeEventArgs<HyperLinkImpl> args )
    {
      HyperLinkImpl link = args.Value as HyperLinkImpl;

      if( link == null )
        throw new ArgumentNullException( "link" );

      IRange range = link.Range;
      int iFirstRow = range.Row;
      int iFirstCol = range.Column;
      int iLastRow = range.LastRow;
      int iLastCol = range.LastColumn;

      for( int iRow = iFirstRow; iRow <= iLastRow; iRow++ )
      {
        for( int iCol = iFirstCol; iCol <= iLastCol; iCol++ )
        {
          long index = RangeImpl.GetCellIndex( iCol, iRow );
          List<HyperLinkImpl> list;

          if( m_dicCellToList.TryGetValue( index, out list ) )
          {
            list.Remove( link );
          }
          else
          {
            //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Wrong range in link", "Remove Hyperlink Warning" );
          }
        }
      }
    }
    #endregion
  }
}
