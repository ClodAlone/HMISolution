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
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using TVPageBreak = Syncfusion.XlsIO.Parser.Biff_Records.VerticalPageBreaksRecord.TVPageBreak;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// A collection of vertical page breaks within the print area.
  /// Each vertical page break is represented by a VPageBreak object.
  /// </summary>
  public class VPageBreaksCollection
    : CollectionBaseEx<IVPageBreak>
    , IVPageBreaks
    , IBiffStorage
  {
    #region Class members
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    #endregion

    #region IVPageBreaks Members
    /// <summary>
    /// Returns single page break from the collection.
    /// </summary>
    public IVPageBreak this[ IRange location ]
    {
      get
      {
        return GetPageBreak( location.Column );
      }
    }
    /// <summary>
    /// Adds a vertical page break. Returns a VPageBreak object.
    /// </summary>
    /// <param name="location">Page break location.</param>
    /// <returns>Newly added page break.</returns>
    /// <exception cref="System.ArgumentException">
    /// When specified object cannot be found in the collection.
    /// </exception>
    public IVPageBreak Add( IRange location )
    {
      if( location == null )
        throw new ArgumentNullException( "location" );

      //if( !( ( RangeImpl )location ).IsSingleCell )
      //  throw new ArgumentException( "Location should be single cell." );

      //int iColumn = location.Column - 1;
      // Check if collection already contains such page break.
      //if( m_hashRowToBreak.Contains( iColumn ) )
      //  return ( IVPageBreak )m_hashRowToBreak[ iColumn ];

      IVPageBreak vbreak = new VPageBreakImpl( Application, this, location );

      base.Add( vbreak );
      //m_hashRowToBreak.Add( iColumn, vbreak );

      return vbreak;
    }
    /// <summary>
    /// Removes vertical page break.
    /// </summary>
    /// <param name="location">Page break location.</param>
    /// <returns>Page break that was removed from the collection.</returns>
    public IVPageBreak Remove( IRange location )
    {
      int index = GetPageBreakIndex( location );
      IVPageBreak result = null;

      if( index >= 0 )
      {
        result = ( IVPageBreak )List[ index ];
        base.RemoveAt( index );
      }

      return result;
    }
    /// <summary>
    /// Returns page break at the specified column.
    /// </summary>
    /// <param name="iColumn">One-based column index.</param>
    /// <returns>Page break with corresponding column or null if not found.</returns>
    public IVPageBreak GetPageBreak( int iColumn )
    {
      IVPageBreak result = null;

      for( int i = 0, len = Count; i < len; i++ )
      {
        IVPageBreak pageBreak = this[ i ] as IVPageBreak;

        if( pageBreak.Location.Column == iColumn )
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
    private int GetPageBreakIndex( IRange location )
    {
      int result = -1;
      int iRow = location.Row;
      int iColumn = location.Column;

      for( int i = 0, len = Count; i < len; i++ )
      {
        IVPageBreak pageBreak = this[ i ] as IVPageBreak;

        IRange breakLocation = pageBreak.Location;

        if( breakLocation.Row == iRow && breakLocation.Column == iColumn )
        {
          result = i;
          break;
        }
      }

      return result;
    }
    /// <summary>
    /// Clears Vertical page breaks from VPageBreaks collection.
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

        foreach( VPageBreakImpl vPagebreak in List )
        {
          if( vPagebreak.Type == ExcelPageBreak.PageBreakManual )
            iCount++;
        }

        return iCount;
      }
    }
    #endregion
    #region class Members
    internal void InsertColumns(int columnIndex, int iColumnCount)
    {
        for (int i = 0; i < base.InnerList.Count; i++)
        {
            VPageBreakImpl hPageBreak = (VPageBreakImpl)base.InnerList[i];
            if (hPageBreak.Column >= columnIndex)
            {
                hPageBreak.Column = hPageBreak.Column + iColumnCount;
            }
        }
    }
    internal void DeleteColumns(int columnIndex, int columnCount)
    {
        for (int i = 0; i < base.InnerList.Count; i++)
        {
            VPageBreakImpl hPageBreak = (VPageBreakImpl)base.InnerList[i];
            if (hPageBreak.Column >= columnIndex)
            {

                if ((columnIndex + columnCount) > hPageBreak.Column)
                {
                    base.InnerList.RemoveAt(i--);
                }
                else
                {
                    int iColumn = hPageBreak.Column - columnCount;
                    if (iColumn < columnIndex)
                    {
                        iColumn = columnIndex;
                    }
                    hPageBreak.Column = iColumn;
                }

            }
        }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates collection and sets its Application and Parent properties.
    /// </summary>
    /// <param name="application">Application object for this collection.</param>
    /// <param name="parent">Parent object for this collection.</param>
    public VPageBreaksCollection( IApplication application, object parent )
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
    public void Parse( VerticalPageBreaksRecord record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      TVPageBreak[] arrBreaks = record.PageBreaks;

      for( int i = 0, len = arrBreaks.Length; i < len; i++ )
      {
        TVPageBreak pageBreak = arrBreaks[ i ];
        VPageBreakImpl breakImpl = new VPageBreakImpl( Application, this, pageBreak );
        base.Add( breakImpl );
        //m_hashRowToBreak.Add( ( int ) pageBreak.Column, breakImpl );
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

      VerticalPageBreaksRecord record = PrepareRecord();
      if( record != null )records.Add( record );
    }
    /// <summary>
    /// Prepares record.
    /// </summary>
    /// <returns>Prepared record.</returns>
    private VerticalPageBreaksRecord PrepareRecord()
    {
      int iCount = Count;

      if( iCount == 0 ) return null;

      TVPageBreak[] arrBreaks = new TVPageBreak[ iCount ];
      List<IVPageBreak> list = InnerList;

      for( int i = 0; i < iCount; i++ )
      {
        VPageBreakImpl curBreak = list[ i ] as VPageBreakImpl;
        arrBreaks[ i ] = curBreak.VPageBreak;
      }

      VerticalPageBreaksRecord pageBreak = ( VerticalPageBreaksRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.VerticalPageBreaks );
      
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
    //  IVPageBreak pageBreak = ( IVPageBreak )value;
    //  int iColumn = pageBreak.Location.Column - 1;
    //  m_hashRowToBreak.Remove( iColumn );
    //  base.OnRemoveComplete( index, value );
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
      VPageBreaksCollection result = ( VPageBreaksCollection )base.Clone( parent );
      //List<IVPageBreak> arrBreaks = result.InnerList;
      result.FindParents();

      //for( int i = 0, len = Count; i < len; i++ )
      //{
      //  VPageBreakImpl pageBreak = ( VPageBreakImpl )arrBreaks[ i ];
      //  result.m_hashRowToBreak.Add( pageBreak.Column, pageBreak );
      //}

      return result;
    }
    /// <summary>
    /// Adds page break to the collection.
    /// </summary>
    /// <param name="pageBreak">Page break to add.</param>
    public void Add( VPageBreakImpl pageBreak )
    {
      if( pageBreak == null )
        throw new ArgumentNullException( "pageBreak" );

      //if( !m_book.Loading || !m_hashRowToBreak.ContainsKey( pageBreak.Column ) )
      //  m_hashRowToBreak.Add( pageBreak.Column, pageBreak );

      if( GetPageBreakIndex( pageBreak.Location ) < 0 )
        base.Add( pageBreak );
    }
    /// <summary>
    /// Converts pagebreaks to Excel97to03 version.
    /// </summary>
    public void ChangeToExcel97to03Version()
    {
      List<VPageBreakImpl> lstBreaksToDelete = new List<VPageBreakImpl>();

      foreach( VPageBreakImpl vPagebreak in List )
      {
        WorksheetImpl sheet = ( ( PageSetupImpl )Parent ).Worksheet;
        WorkbookImpl book = ( WorkbookImpl )sheet.Workbook;
        TVPageBreak tVPagebreak = vPagebreak.VPageBreak;

        if( tVPagebreak.Column > book.MaxColumnCount )
        {
          lstBreaksToDelete.Add( vPagebreak );
          continue;
        }

        if( tVPagebreak.StartRow > book.MaxRowCount - 1 )
          tVPagebreak.StartRow = ( ushort )( book.MaxRowCount - 1 );

        if( tVPagebreak.EndRow > book.MaxRowCount - 1 )
          tVPagebreak.EndRow = ( ushort )( book.MaxRowCount - 1 );
      }

      foreach( VPageBreakImpl vPagebreak in lstBreaksToDelete )
      {
        base.Remove( vPagebreak );
        //m_hashRowToBreak.Remove( vPagebreak );
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
        ? VerticalPageBreaksRecord.DEF_SUBITEM_SIZE * iCount + VerticalPageBreaksRecord.DEF_FIXED_PART_SIZE
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
      VerticalPageBreaksRecord record = PrepareRecord();
      return ( record != null )
        ? record.FillStream( writer, provider, encryptor, streamPosition )
        : 0;
    }

    #endregion
  }
}
