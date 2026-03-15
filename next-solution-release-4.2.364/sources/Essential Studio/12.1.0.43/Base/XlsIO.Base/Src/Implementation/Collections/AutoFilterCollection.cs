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

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Shapes;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Summary description for AutoFilterImpl.
  /// </summary>
  public class AutoFiltersCollection
    : CollectionBaseEx<object>
    , IAutoFilters
  {
    #region Class constants
    /// <summary>
    /// Name of the named range for autofilter range.
    /// </summary>
    public const string DEF_AUTOFILTER_NAMEDRANGE = "_FilterDatabase";
    /// <summary>
    /// Name of the named range for autofilter range in MS Excel 2007.
    /// </summary>
    public const string DEF_EXCEL07_AUTOFILTER_NAMEDRANGE = "_xlnm._FilterDatabase";
    #endregion

    #region Class members
    /// <summary>
    /// Range to be filtered.
    /// </summary>
    private IRange m_range;
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetImpl m_worksheet;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public AutoFiltersCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();

      Cleared += new CollectionClear(AutoFiltersCollection_Cleared);
    }

    /// <summary>
    /// Searches for all needed parents.
    /// </summary>
    private void SetParents()
    {
      m_worksheet = FindParent( typeof( WorksheetImpl ) ) as WorksheetImpl;

      if( m_worksheet == null )
        throw new ArgumentException( "Can't find parent worksheet.", "parent" );
    }
    #endregion

    #region IAutoFilters Members

    /// <summary>
    /// Range to be filtered.
    /// </summary>
    public IRange FilterRange
    {
      get
      {
        return m_range;
      }
      set
      {
        if( value != null && value.Worksheet != Worksheet )
          throw new ArgumentOutOfRangeException( "Can't filter ranges from another worksheet" );

        for( int i = 0, len = Count; i < len; i++ )
        {
          //FormControlShapeImpl shape = ( FormControlShapeImpl )m_arrComboBox[ i ];
          //shape.Remove();
          ( ( AutoFilterImpl )this[ i ] ).Clear();
        }

        Clear();
        m_range = value;

        INames names = Worksheet.Names;

        if( value == null )
        {
          names.Remove( DefaultNamedRangeName );
        }
        else if( names.Contains( DefaultNamedRangeName ) )
        {
          IName name = names[ DefaultNamedRangeName ];
          name.RefersToRange = m_range;
        }
        else
        {
          IName name = names.Add( DefaultNamedRangeName, m_range );
          name.Visible = false;
          ( ( NameImpl )name ).IsBuiltIn = true;
        }

        InnerList.Clear();

        if( value != null )
        {
          int iRowIndex = m_range.Row;

          for( int i = value.Column, iLast = value.LastColumn; i <= iLast; i++ )
          {
            IRange merge = m_worksheet[ iRowIndex, i ].MergeArea;
            

            int iLastColumn = ( merge != null ) ?
              merge.LastColumn :
              i;

            InnerList.Add( new AutoFilterImpl( this, i, iLastColumn, iRowIndex ) );
            AutoFilterImpl filter = (AutoFilterImpl)InnerList[InnerList.Count - 1];
            filter.Index = InnerList.Count - 1;
            filter.m_colIndex = i;
            i = iLastColumn;
          }
        }
      }
    }

    /// <summary>
    /// Returns single autofilter object by column index. Read-only.
    /// </summary>
    public IAutoFilter this[ int columnIndex ]
    {
      get
      {
        return ( IAutoFilter )InnerList[ columnIndex ];
      }
    }

    /// <summary>
    /// Gets address of filtered range in R1C1 style. Read-only.
    /// </summary>
    public string AddressR1C1
    {
      get
      {
        INames names = Worksheet.Names;

        if( names.Contains( DEF_AUTOFILTER_NAMEDRANGE ) )
        {
          IName name = names[ DEF_AUTOFILTER_NAMEDRANGE ];
          return name.ValueR1C1;
        }

        throw new ApplicationException( "cannot find filtered range." );
      }
    }
    #endregion

    #region Implementation properties
    /// <summary>
    /// Returns parent worksheet. Read-only.
    /// </summary>
    public WorksheetImpl Worksheet
    {
      get
      {
        return m_worksheet;
      }
    }
    /// <summary>
    /// Indicates whether sheet contains filtered range.
    /// </summary>
    public bool IsFiltered
    {
      get
      {
        if( Count == 0 ) return false;

        for( int i = 0; i < Count; i++ )
        {
          if( this[ i ].IsFiltered ) return true;
        }

        return false;
      }
    }
    /// <summary>
    /// Gets default autofilter named range name.
    /// </summary>
    public string DefaultNamedRangeName
    {
      get
      {
        switch( Worksheet.Version )
        {
          case ExcelVersion.Excel97to2003:
            return DEF_AUTOFILTER_NAMEDRANGE;

          case ExcelVersion.Excel2007:
          case ExcelVersion.Excel2010:
          case ExcelVersion.Excel2013:
            return DEF_EXCEL07_AUTOFILTER_NAMEDRANGE;

          default:
            throw new ArgumentOutOfRangeException( "Unexpected excel version" );
        }
      }
    }
    #endregion

    #region Serialization methods
    /// <summary>
    /// Parses filter records. Must be called after parsing of all names.
    /// </summary>
    /// <param name="records">Records to parse</param>
    public void Parse( List<BiffRecordRaw> records )
    {
      if( records == null || records.Count == 0 ) return;

      int iCurFilter = 0;

      if( !m_worksheet.Names.Contains( DEF_AUTOFILTER_NAMEDRANGE ) )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Can't find named range for autofilter", "Warning" );
        return;
      }

      IName name = m_worksheet.Names[ DEF_AUTOFILTER_NAMEDRANGE ];
      m_range = name.RefersToRange;

      if( m_range == null )
        return;

      int iRowIndex = 0;
      int iColumnIndex = 0;

      if( m_range != null )
      {
        iRowIndex = m_range.Row;
        iColumnIndex = m_range.Column;

        for( int i = 0, len = m_range.LastColumn - m_range.Column + 1; i < len; i++ )
        {
          AutoFilterImpl filter = new AutoFilterImpl( this );
          InnerList.Add( filter );
          filter.Index = InnerList.Count - 1;
        }
      }

      for( int i = 0, len = records.Count; i < len; i++ )
      {
        BiffRecordRaw raw = ( BiffRecordRaw )records[ i ];

        switch( raw.TypeCode )
        {
          case TBIFFRecord.FilterMode:
            // No useful info, just skip this record.
            break;

          case TBIFFRecord.AutoFilter:
            AutoFilterRecord filter = ( AutoFilterRecord )raw;
            
            if( InnerList.Count <= iCurFilter )
            {
              AutoFilterImpl newFilter = new AutoFilterImpl( this );
              InnerList.Add( newFilter );
            }

            ( ( AutoFilterImpl )this[ iCurFilter ] ).Parse( filter, iColumnIndex, iRowIndex );
            iCurFilter++;
            break;

          case TBIFFRecord.AutoFilterInfo:
            // No useful info, just skip this record.
            //AutoFilterInfoRecord info = ( AutoFilterInfoRecord )raw;
            iCurFilter = 0;
            break;

          default:
            throw new ArgumentOutOfRangeException( "Unknown record" );
        }
      }
    }
    /// <summary>
    /// Serializes collection into set of records.
    /// </summary>
    /// <param name="records">Place where serialized records will be placed.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      int iCount = Count;

      if( iCount == 0 )
        return;

      if( records == null )
        throw new ArgumentNullException( "records" );

      bool bFiltered = IsFiltered;

      if( bFiltered )
      {
        FilterModeRecord filterMode = ( FilterModeRecord )BiffRecordFactory.GetRecord(
          TBIFFRecord.FilterMode );
        records.Add( filterMode );
      }

      AutoFilterInfoRecord info = ( AutoFilterInfoRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.AutoFilterInfo );

      info.ArrowsCount = ( ushort )iCount;
      records.Add( info );

      if( bFiltered )
      {
        for( int i = 0; i < iCount; i++ )
        {
          AutoFilterImpl filter = ( AutoFilterImpl ) this[ i ];
          filter.Serialize( records );
        }
      }
    }
    #endregion

    #region Class event handlers
    /// <summary>
    /// Cleared event handler.
    /// </summary>
    private void AutoFiltersCollection_Cleared()
    {
      INames names = Worksheet.Names;

      if( names.Contains( DEF_AUTOFILTER_NAMEDRANGE ) )
      {
        names.Remove( DEF_AUTOFILTER_NAMEDRANGE );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns just cloned object.</returns>
    public AutoFiltersCollection Clone( WorksheetImpl parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      AutoFiltersCollection result = ( AutoFiltersCollection )base.Clone( parent );

      if( m_range != null )
      {
        result.m_range = parent[ m_range.AddressLocal ];
      }

      return result;
    }
    /// <summary>
    /// Changes Excel versions.
    /// </summary>
    /// <param name="iLastRow">Max last row.</param>
    /// <param name="iLastColumn">Max last column.</param>
    /// <param name="version">Excel version to make conversion into.</param>
    public void ChangeVersions( int iLastRow, int iLastColumn, ExcelVersion version )
    {
      if( version !=ExcelVersion.Excel97to2003 )
      {
        INames names = m_worksheet.Names;
        IName name = names[ DEF_AUTOFILTER_NAMEDRANGE ];
        name.Name = DefaultNamedRangeName;
      }
      else
      {
        int iNewLastColumn = FilterRange.LastColumn;
        int iNewLastRow = FilterRange.LastRow;

        if( FilterRange.Column > iLastColumn || FilterRange.Row > iLastRow )
        {
          for( int i = 0, len = Count; i < len; i++ )
          {
            ( ( AutoFilterImpl )this[ i ] ).Clear();
          }

          Clear();
        }
        else
        {
          if( iNewLastColumn > iLastColumn )
          {
            iNewLastColumn = iLastColumn;
          }

          if( iNewLastRow > iLastRow )
          {
            iNewLastRow = iLastRow;
          }

          INames names = m_worksheet.Names;
          IName name = names[ DEF_EXCEL07_AUTOFILTER_NAMEDRANGE ];
          name.Name = DefaultNamedRangeName;

          FilterRange = Worksheet[ FilterRange.Row, FilterRange.Column, iNewLastRow, iNewLastColumn ];
        }
      }
    }
    /// <summary>
    /// This method updates internal range object that stores filtered range.
    /// </summary>
    public void UpdateFilterRange()
    {
      INames names = m_worksheet.Names;
      IName name = names[ DefaultNamedRangeName ];

      if( name != null )
      {
        m_range = name.RefersToRange;
      }
      else
      {
        m_range = null;
      }
    }
    #endregion
  }
}
