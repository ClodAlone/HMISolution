#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents collection of ChartFormatImpl.
  /// </summary>
  public class ChartFormatCollection
    : CollectionBaseEx<ChartFormatImpl>
  {
    #region Class constants
    /// <summary>
    /// Represents default array value.
    /// </summary>
    private const int DEF_ARRAY_VALUE = -1;
    /// <summary>
    /// Represents capacity of array.
    /// </summary>
    public const int DEF_ARRAY_CAPACITY = 8;
    /// <summary>
    /// Represents chart types that need secondary axis.
    /// </summary>
    private static readonly TBIFFRecord[] DEF_NEED_SECONDARY_AXIS =
    {
      TBIFFRecord.ChartPie,
      TBIFFRecord.ChartRadar,
      TBIFFRecord.ChartRadarArea,
      TBIFFRecord.ChartBoppop
    };
    #endregion

    #region Class members
    /// <summary>
    /// Key - ZOrder, value - index.
    /// </summary>
    private int[] m_arrOrder;
    /// <summary>
    /// Represents parent axis.
    /// </summary>
    private ChartParentAxisImpl m_parentAxis;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates new instance of collection.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    public ChartFormatCollection( IApplication application, object parent )
      : base( application, parent )
    {
      m_arrOrder = new int[ DEF_ARRAY_CAPACITY ];

      for( int i = 0; i < DEF_ARRAY_CAPACITY; i++ )
      {
        m_arrOrder[ i ] = DEF_ARRAY_VALUE;
      }

      SetParents();
    }
    /// <summary>
    /// Sets parent objects.
    /// </summary>
    public void SetParents()
    {
      m_parentAxis = ( ChartParentAxisImpl )FindParent( typeof( ChartParentAxisImpl ) );

      if( m_parentAxis == null )
        throw new ApplicationException( "Can't find parent axis." );
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serializes current collection.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      for( int i = 0; i < Count; i++ )
        ( ( ChartFormatImpl )List[ i ] ).Serialize( records );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single format by DrawingZOrder. Read-only.
    /// </summary>
    public ChartFormatImpl this[ int index ]
    {
      get
      {
        if( m_arrOrder[ index ] == DEF_ARRAY_VALUE )
          throw new ArgumentException( "Index out of bounds." );

        return ( ChartFormatImpl )List[ ( int )m_arrOrder[ index ] ];
      }
    }
    /// <summary>
    /// If true - this collection represents formats for primary axis;
    /// otherwise - secondary collection.
    /// </summary>
    public bool IsPrimary
    {
      get
      {
        return m_parentAxis.IsPrimary;
      }
    }
    /// <summary>
    /// Returns true if this collection is primary and contain series that need
    /// secondary axis.
    /// </summary>
    public bool NeedSecondaryAxis
    {
      get
      {
        if( !IsPrimary || Count < 1 )
          return false;

        ChartFormatImpl format = ( ChartFormatImpl )List[ 0 ];
        TBIFFRecord recordType = format.FormatRecordType;

        bool flag = !format.Is3D && ( ( Array.IndexOf( DEF_NEED_SECONDARY_AXIS
          , recordType ) != -1 ) || ( recordType == TBIFFRecord.ChartBar
          && format.IsHorizontalBar ) );

        return flag;
      }
    }
    #endregion

    #region Class methods
    public ChartFormatImpl Add( ChartFormatImpl formatToAdd )
    {
      return Add( formatToAdd, false );
    }

    /// <summary>
    /// Adds new format.
    /// </summary>
    /// <param name="formatToAdd">Chartformat to add.</param>
    /// <returns>Returns just added format.</returns>
    public ChartFormatImpl Add( ChartFormatImpl formatToAdd, bool bCanReplace )
    {
      if( formatToAdd == null )
        throw new ArgumentNullException( "formatToAdd" );

      int iOrder = formatToAdd.DrawingZOrder;
      int index = m_arrOrder[ iOrder ];

      if( index < 0 || !bCanReplace )
      {
        base.Add( formatToAdd );
        index = Count - 1;
        formatToAdd = m_parentAxis.Formats.AddFormat( formatToAdd, iOrder, index, IsPrimary );
      }
      else
      {
        base[ index ] = formatToAdd;
      }

      return formatToAdd;
    }
    /// <summary>
    /// Checks whether similar format is already present in the collection
    /// and returns it, otherwise it add new format.
    /// </summary>
    /// <param name="formatToAdd">Format that should be placed in the collection.</param>
    /// <returns>Format from the collection.</returns>
    public ChartFormatImpl FindOrAdd( ChartFormatImpl formatToAdd )
    {
      ChartFormatImpl result = null;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ChartFormatImpl format = ( ChartFormatImpl )InnerList[ i ];

        if( formatToAdd == format )
        {
          result = format;
          break;
        }
      }

      if( result == null )
      {
        result = Add( formatToAdd, false );
      }

      return result;
    }
    /// <summary>
    /// Checks for containing index in collection.
    /// </summary>
    /// <param name="index">Index to check.</param>
    /// <returns>If contains - returns true; otherwise - false.</returns>
    public bool ContainsIndex( int index )
    {
      return !( index >= DEF_ARRAY_CAPACITY || index < 0 || m_arrOrder[ index ] == DEF_ARRAY_VALUE );
    }
    /// <summary>
    /// Removes formats by instance.
    /// </summary>
    /// <param name="toRemove">Removes current instance.</param>
    public void Remove( ChartFormatImpl toRemove )
    {
      if( toRemove == null )
        throw new ArgumentNullException( "toRemove" );

      int iOrder = toRemove.DrawingZOrder;

      ChartImpl chart = m_parentAxis.m_parentChart;
      ChartSeriesCollection series = ( ChartSeriesCollection )chart.Series;

      if( series.GetCountOfSeriesWithSameDrawingOrder( iOrder ) != 0 )
        throw new ArgumentException( "Can't remove format." );

      int indexToRemove = m_arrOrder[ iOrder ];

      base.RemoveAt( indexToRemove );
      m_arrOrder[ iOrder ] = DEF_ARRAY_VALUE;

      m_parentAxis.Formats.RemoveFormat( indexToRemove, iOrder, IsPrimary );
    }
    /// <summary>
    /// Updates indexes in collection after remove.
    /// </summary>
    /// <param name="removeIndex">Index of removed format.</param>
    public void UpdateIndexesAfterRemove( int removeIndex )
    {
      for( int i = 0; i < DEF_ARRAY_CAPACITY; i++ )
      {
        if( m_arrOrder[ i ] > removeIndex )
          m_arrOrder[ i ] = m_arrOrder[ i ] - 1;
      }
    }
    /// <summary>
    /// Changes Series chart group in all series.
    /// </summary>
    /// <param name="newIndex">New index.</param>
    /// <param name="OldIndex">Old Index.</param>
    public void UpdateSeriesByChartGroup( int newIndex, int OldIndex )
    {
      ChartImpl chart = m_parentAxis.m_parentChart;
      ChartSeriesCollection series = ( ChartSeriesCollection )chart.Series;

      for( int i = 0, iLen = series.Count; i < iLen; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )series[ i ];

        if( serie.ChartGroup == OldIndex )
          serie.ChartGroup = newIndex;
      }
    }
    /// <summary>
    /// Clears current collection.
    /// </summary>
    public new void Clear()
    {
      base.Clear();

      for( int i = 0; i < DEF_ARRAY_CAPACITY; i++ )
      {
        m_arrOrder[ i ] = DEF_ARRAY_VALUE;
      }
    }
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns cloned object.</returns>
    public override object Clone( object parent )
    {
      ChartFormatCollection result = ( ChartFormatCollection )base.Clone( parent );//new ChartFormatCollection( Application, parent );

      result.m_arrOrder = CloneUtils.CloneIntArray( m_arrOrder );

//      int iLen = List.Count;
//      for( int i = 0; i < iLen; i++ )
//      {
//        ChartFormatImpl format = ( ChartFormatImpl )InnerList[ i ];
//        result.InnerList.Add( format.Clone( result ) );
//      }

      return result;
    }
    /// <summary>
    /// Sets value by index.
    /// </summary>
    /// <param name="index">Index to set.</param>
    /// <param name="Value">Value to set.</param>
    public void SetIndex( int index, int Value )
    {
      if( index >= DEF_ARRAY_CAPACITY || index < 0
        || Value < 0 || Value >= List.Count )
      {
        throw new ArgumentException( "Index is out of bounds" );
      }

      m_arrOrder[ index ] = Value;
    }
    /// <summary>
    /// Updates formats on adding format.
    /// </summary>
    /// <param name="index">Index to updates.</param>
    public void UpdateFormatsOnAdding( int index )
    {
      ChartFormatImpl format = ( ChartFormatImpl )this[ index ];

      format.DrawingZOrder = index + 1; 
      UpdateSeriesByChartGroup( index + 1, index );

      m_arrOrder[ index + 1 ] = m_arrOrder[ index ];
      m_arrOrder[ index ] = DEF_ARRAY_VALUE;
    }
    /// <summary>
    /// Updates formats on removing.
    /// </summary>
    /// <param name="index">Index to update.</param>
    public void UpdateFormatsOnRemoving( int index )
    {
      ChartFormatImpl format = ( ChartFormatImpl )this[ index ];

      format.DrawingZOrder = index - 1;
      UpdateSeriesByChartGroup( index - 1, index );

      m_arrOrder[ index - 1 ] = m_arrOrder[ index ];
      m_arrOrder[ index ] = DEF_ARRAY_VALUE;
    }
    /// <summary>
    /// Gets format by index, and shallow removes current format.
    /// </summary>
    /// <param name="iOrder">Format order.</param>
    /// <param name="bDelete">If true - delete current format.</param>
    /// <returns>Format by index.</returns>
    public ChartFormatImpl GetFormat( int iOrder, bool bDelete )
    {
      int index = m_arrOrder[ iOrder ];

      if( index == DEF_ARRAY_VALUE )
        throw new ArgumentException( "Can't find format by current index." );
      
      ChartFormatImpl format = ( ChartFormatImpl )List[ index ];

      if( bDelete )
      {
        m_arrOrder[ iOrder ] = DEF_ARRAY_VALUE;
        base.RemoveAt( index );

        for( int i = 0, len = m_arrOrder.Length; i < len; i++ )
        {
          int iCurrentIndex = m_arrOrder[ i ];

          if( iCurrentIndex > index )
            m_arrOrder[ i ] = --iCurrentIndex;
        }
      }

      return format;
    }
    /// <summary>
    /// Shallow adds format to collection.
    /// </summary>
    /// <param name="format">Format to add.</param>
    public void AddFormat( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      int order = format.DrawingZOrder;
      base.Add( format );
      int index = Count - 1;

      m_arrOrder[ order ] = index;
    }
    #endregion
  }
}
