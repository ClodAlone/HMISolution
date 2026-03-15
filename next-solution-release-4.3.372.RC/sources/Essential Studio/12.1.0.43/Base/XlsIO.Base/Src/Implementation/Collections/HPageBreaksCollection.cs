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
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;

using THPageBreak = Syncfusion.XlsIO.Parser.Biff_Records.HorizontalPageBreaksRecord.THPageBreak;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// The collection of horizontal page breaks within the print area.
  /// Each horizontal page break is represented by an HPageBreak object.
  /// </summary>
  public class HPageBreaksCollection
    : CollectionBaseEx<IHPageBreak>
    , IHPageBreaks
    , IBiffStorage
  {
    #region Class members
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    #endregion

    #region IHPageBreaks Members
    /// <summary>
    /// 
    /// </summary>
    public IHPageBreak this[ IRange location ]
    {
      get
      {
        int index = GetPageBreakIndex( location );

        return ( index >= 0 ) ?
          List[ index ] as IHPageBreak :
          null;
      }
    }
    /// <summary>
    /// Adds a horizontal page break. Returns an HPageBreak object. Read-only.
    /// </summary>
    /// <param name="location">Object after which new page break must be inserted.</param>
    /// <returns>HPageBreak which was added.</returns>
    /// <exception cref="System.ArgumentException">
    /// If specified object couldn't be found in the collection.
    /// </exception>
    public IHPageBreak Add( IRange location )
    {
      if( location == null )
        throw new ArgumentNullException( "location" );

      if( !( ( RangeImpl )location ).IsSingleCell )
        throw new ArgumentException( "Location should be single cell." );

      // Check if collection already contains such page break.
      //if( m_hashRowToBreak.Contains( location.Row - 1 ) )
      //  return ( IHPageBreak )m_hashRowToBreak[ location.Row - 1 ];

      HPageBreakImpl hbreak = new HPageBreakImpl( Application, this, location );
      Add( hbreak );

      return hbreak;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public IHPageBreak Remove( IRange location )
    {
      IHPageBreak result = null;
      int index = GetPageBreakIndex( location );

      if( index >= 0 )
      {
          result = ( IHPageBreak )List[ index ];
          base.RemoveAt( index );
      }

      return result;
    }
    /// <summary>
    /// Returns page break at the specified row.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <returns>Page break with corresponding row or null if not found.</returns>
    public IHPageBreak GetPageBreak( int iRow )
    {
      IHPageBreak result = null;

      for( int i = 0, len = Count; i < len; i++ )
      {
        IHPageBreak pageBreak = this[ i ] as IHPageBreak;

        if( pageBreak.Location.Row == iRow )
        {
          result = pageBreak;
          break;
        }
      }

      return result;
    }
    /// <summary>
    /// Returns index of the page break
    /// </summary>
    /// <param name="location">Location of the break to find.</param>
    /// <returns>Index of the page break in the collection or -1 if not found.</returns>
    internal int GetPageBreakIndex( IRange location )
    {
      int result = -1;
      int iRow = location.Row;
      int iColumn = location.Column;

      for( int i = 0, len = Count; i < len; i++ )
      {
        IHPageBreak pageBreak = this[ i ] as IHPageBreak;

        IRange breakLocation = pageBreak.Location;

        if( breakLocation.Row == iRow && breakLocation.Column == iColumn )
        {
          result = i;
          break;
        }
      }

      return result;
    }
    internal void InsertRows(int rowIndex, int totalRows)
    {
        for (int i = 0; i < base.InnerList.Count; i++)
        {
            HPageBreakImpl hPageBreak = (HPageBreakImpl)base.InnerList[i];
            if (hPageBreak.Row >= rowIndex)
            {
                hPageBreak.Row = hPageBreak.Row + totalRows;
            }
        }
    }
    internal void DeleteRows(int rowIndex, int totalRow)
    {
        for (int i = 0; i < base.InnerList.Count; i++)
        {
            HPageBreakImpl hPageBreak = (HPageBreakImpl)base.InnerList[i];
            if (hPageBreak.Row >= rowIndex)
            {

                if ((rowIndex + totalRow) > hPageBreak.Row)
                {
                    base.InnerList.RemoveAt(i--);
                }
                else
                {
                    int iRow = hPageBreak.Row - totalRow;
                    if (iRow < rowIndex)
                    {
                        iRow = rowIndex;
                    }
                    hPageBreak.Row = iRow;
                }

            }
        }
    }
   
    /// <summary>
    /// Clears horizontal page breaks from HPageBreaks collection.
    /// </summary>
    public void Clear()
    {
        base.Clear();     
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns manual breaks count. Read-only.
    /// </summary>
    public int ManualBreakCount
    {
      get
      {
        int iCount = 0;

        foreach( HPageBreakImpl hPagebreak in List )
        {
          if( hPagebreak.Type == ExcelPageBreak.PageBreakManual )
            iCount++;
        }

        return iCount;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates collection and sets its Application and Parent properties.
    /// </summary>
    /// <param name="application">
    /// Application object that represents the Microsoft Excel application.
    /// </param>
    /// <param name="parent">The parent object for the specified object.</param>
    public HPageBreaksCollection( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
    }
    /// <summary>
    /// Looks for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentOutOfRangeException( "parent" );
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Parses breaks record.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    [ CLSCompliant( false ) ]
    public void Parse( HorizontalPageBreaksRecord record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      THPageBreak[] arrBreaks = record.PageBreaks;

      for( int i = 0, len = arrBreaks.Length; i < len; i++ )
      {
        THPageBreak pageBreak = arrBreaks[ i ];

        if( pageBreak.StartColumn < m_book.MaxColumnCount )
        {
          HPageBreakImpl breakImpl = new HPageBreakImpl( Application, this, pageBreak );
          base.Add( breakImpl );
          //m_hashRowToBreak.Add( ( int )pageBreak.Row, breakImpl );
        }
      }
    }

    /// <summary>
    /// Serializes collection into as set of Biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      HorizontalPageBreaksRecord pageBreak = PrepareRecord();

      if( pageBreak != null ) records.Add( pageBreak );
    }
    /// <summary>
    /// Converts collection into biff record.
    /// </summary>
    /// <returns>Null if collection is empty; created record otherwise.</returns>
    private HorizontalPageBreaksRecord PrepareRecord()
    {
      int iCount = Count;

      if( iCount == 0 ) return null;

      THPageBreak[] arrBreaks = new THPageBreak[ iCount ];
      List<IHPageBreak> list = InnerList;

      for( int i = 0; i < iCount; i++ )
      {
        HPageBreakImpl curBreak = list[ i ] as HPageBreakImpl;
        arrBreaks[ i ] = curBreak.HPageBreak;
      }

      HorizontalPageBreaksRecord pageBreak = ( HorizontalPageBreaksRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.HorizontalPageBreaks );
      
      pageBreak.PageBreaks = arrBreaks;

      return pageBreak;
    }
    #endregion

    #region Class overrides
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="index"></param>
    ///// <param name="value"></param>
    //protected override void OnRemoveComplete(int index, object value)
    //{
    //  //IHPageBreak pageBreak = ( IHPageBreak )value;
    //  //m_hashRowToBreak.Remove( pageBreak.Location.Row - 1 );
    //  base.OnRemoveComplete ( index, value );
    //}

    #endregion

    #region Class helper methods
    /// <summary>
    /// Creates copy of the collection.
    /// </summary>
    /// <param name="parent">Parent for new collection.</param>
    /// <returns>A clone of the current collection.</returns>
    public override object Clone( object parent )
    {
      HPageBreaksCollection result = ( HPageBreaksCollection )base.Clone( parent );
      //List<IHPageBreak> arrBreaks = result.InnerList;
      result.FindParents();

//      for( int i = 0, len = Count; i < len; i++ )
//      {
//        HPageBreakImpl pageBreak = ( HPageBreakImpl )arrBreaks[ i ];
//        result.m_hashRowToBreak.Add( pageBreak.Row, pageBreak );
////        pageBreak = pageBreak.Clone( result );
////        result.Add( pageBreak );
//      }

      return result;
    }
    /// <summary>
    /// Adds page break to the collection.
    /// </summary>
    /// <param name="pageBreak">Page break to add.</param>
    public void Add( HPageBreakImpl pageBreak )
    {
      if( pageBreak == null )
        throw new ArgumentNullException( "pageBreak" );

      //if( !m_book.Loading || !m_hashRowToBreak.ContainsKey( pageBreak.Row ) )
      //  m_hashRowToBreak.Add( pageBreak.Row, pageBreak );

      if( GetPageBreakIndex( pageBreak.Location ) < 0 )
        base.Add( pageBreak );
    }
    /// <summary>
    /// Converts pagebreaks to Excel97to03 version.
    /// </summary>
    public void ChangeToExcel97to03Version()
    {
      List<HPageBreakImpl> lstBreaksToDelete = new List<HPageBreakImpl>();

      foreach( HPageBreakImpl hPagebreak in List )
      {
        WorksheetImpl sheet = ( ( PageSetupImpl )Parent ).Worksheet;
        WorkbookImpl book = ( WorkbookImpl )sheet.Workbook;
        THPageBreak tHPagebreak = hPagebreak.HPageBreak;

        if( tHPagebreak.Row > book.MaxRowCount )
        {
          lstBreaksToDelete.Add( hPagebreak );
          continue;
        }

        if( tHPagebreak.StartColumn > book.MaxColumnCount - 1 )
          tHPagebreak.StartColumn = ( ushort )( book.MaxColumnCount - 1 );

        if( tHPagebreak.EndColumn > book.MaxColumnCount - 1 )
          tHPagebreak.EndColumn = ( ushort )( book.MaxColumnCount - 1 );
      }

      foreach( HPageBreakImpl hPagebreak in lstBreaksToDelete )
      {
        base.Remove( hPagebreak );
        //m_hashRowToBreak.Remove( hPagebreak );
      }
    }
    #endregion

    #region IBiffStorage Members
    /// <summary>
    /// Returns type code of the biff storage. Read-only.
    /// </summary>
    public TBIFFRecord TypeCode
    {
      get
      {
        return 0;
      }
    }
    /// <summary>
    /// Returns code of the biff storage. Read-only.
    /// </summary>
    public int RecordCode
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// Indicates whether data array is required by this record.
    /// </summary>
    public bool NeedDataArray
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Indicates record position in stream. This is a utility member of class and
    /// is used only in the serialization process. Does not influence the data.
    /// </summary>
    public long StreamPos
    {
      get
      {
        return -1;
      }
      set
      {
      }
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public int GetStoreSize( ExcelVersion version )
    {
      int iCount = Count;

      return ( iCount > 0 )
        ? HorizontalPageBreaksRecord.DEF_SUBITEM_SIZE * iCount + HorizontalPageBreaksRecord.FixedSize
        :  -BiffRecordRaw.DEF_HEADER_SIZE;
    }

    /// <summary>
    /// Save record data to stream.
    /// </summary>
    /// <param name="writer">Writer that will receive record data.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns>Size of the record.</returns>
    /// <exception cref="System.ArgumentNullException">If writer is NULL.</exception>
    /// <exception cref="System.ApplicationException">
    ///   If m_iLength of internal record data array is less than zero.
    /// </exception>
    public int FillStream( BinaryWriter writer, DataProvider provider, IEncryptor encryptor, int streamPosition )
    {
      HorizontalPageBreaksRecord record = PrepareRecord();
      return ( record != null )
        ? record.FillStream( writer, provider, encryptor, streamPosition )
        : 0;
    }

    #endregion
  }
}
