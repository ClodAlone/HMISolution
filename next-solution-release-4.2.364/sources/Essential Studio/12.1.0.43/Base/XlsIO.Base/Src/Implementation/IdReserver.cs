#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class is responsible for shape id's reservation.
  /// </summary>
  public class IdReserver
  {
    #region Constants
    /// <summary>
    /// Size of the single id's segment.
    /// </summary>
    public const int SegmentSize = 1024;
    #endregion

    #region Members
    /// <summary>
    /// Dictionary that contains information about reserved id's.
    /// key - segment start
    /// value - id of the collection that reserved current segment.
    /// </summary>
    private Dictionary<int, int> m_id = new Dictionary<int, int>();
    /// <summary>
    /// Dictionary that contains information about number of reserved id's.
    /// key - segment start
    /// value - number of reserved sectors.
    /// </summary>
    private Dictionary<int, int> m_idCount = new Dictionary<int, int>();
    /// <summary>
    /// Number of sectors (key) and first id (value) reserved by some collection.
    /// </summary>
    private Dictionary<int, KeyValuePair<int, int>> m_collectionCount = new Dictionary<int, KeyValuePair<int, int>>();
    /// <summary>
    /// Maximum reserved id.
    /// </summary>
    private int m_iMaximumId;
    /// <summary>
    /// Additional shapes.
    /// </summary>
    private Dictionary<int, int> m_dictAdditionalShapes = new Dictionary<int, int>();
    #endregion

    #region Properties
    /// <summary>
    /// Gets maximum accessed shape id.
    /// </summary>
    public int MaximumId
    {
      get
      {
        return m_iMaximumId;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Evaluates start of the segment containing specified id.
    /// </summary>
    /// <param name="id">Id to get segment start for.</param>
    /// <returns></returns>
    public static int GetSegmentStart( int id )
    {
      int iMod = id % 1024;
      int iSegmentStart = id - iMod;
      return iSegmentStart;
    }
    /// <summary>
    /// Checks whether segment with specified it is already reserved.
    /// </summary>
    /// <param name="id">Id to check.</param>
    /// <returns></returns>
    public bool CheckReserved( int id )
    {
      int iSegmentStart = GetSegmentStart( id );
      return m_id.ContainsKey( iSegmentStart );
    }
    /// <summary>
    /// Checks whether segment with specified id is free.
    /// </summary>
    /// <param name="id">Id to check.</param>
    /// <param name="count">Number of segments to check.</param>
    /// <returns>True if all segments are free.</returns>
    public bool CheckFree( int id, int count )
    {
      int iSegmentStart = GetSegmentStart( id );
      bool bResult = true;

      for( int i = 0; i < count && bResult; i++, iSegmentStart += SegmentSize )
      {
        bResult = !m_id.ContainsKey( iSegmentStart );
      }

      return bResult;
    }
    /// <summary>
    /// Gets index of the collection which reserved specified id.
    /// </summary>
    /// <param name="id">Id to check.</param>
    /// <returns>Index of the collection which reserved id, or 0 if it is not reserved.</returns>
    public int ReservedBy( int id )
    {
      int iMod = id % 1024;
      int iSegmentStart = id - iMod;
      int result;

      m_id.TryGetValue( iSegmentStart, out result );
      return result;
    }
    /// <summary>
    /// Tries to reserve segment or segments containing specified ids for the specified collection.
    /// </summary>
    /// <param name="id">Start id to reserve.</param>
    /// <param name="lastId">Last id to reserve.</param>
    /// <param name="collectionId">Collection owning those ids</param>
    /// <returns>True if reservation succeeded.</returns>
    public bool TryReserve( int id, int lastId, int collectionId )
    {
      int iStartId = id;
      id = GetSegmentStart( id );
      m_iMaximumId = Math.Max( m_iMaximumId, lastId );
      int iLastSectorStart = GetSegmentStart( lastId );
      bool bResult = false;

      int iSegmentsCount = ( iLastSectorStart - id ) / SegmentSize + 1;

      if( CheckFree( id, iSegmentsCount ) )
      {
        bResult = true;

        KeyValuePair<int, int> currentPair;

        if( !m_collectionCount.TryGetValue( collectionId, out currentPair ) )
        {
          m_collectionCount.Add( collectionId, new KeyValuePair<int, int>( iSegmentsCount, id ) );
        }
        else
        {
          int iNewCount = currentPair.Key + ( int )Math.Ceiling( ( iLastSectorStart - id + 1 ) / ( double )SegmentSize );
          KeyValuePair<int, int> newPair = new KeyValuePair<int,int>( iNewCount, currentPair.Value );
          m_collectionCount[ collectionId ] = newPair;
        }

        for( int i = id; i <= lastId && bResult; i += SegmentSize )
        {
          m_id.Add( i, collectionId );
        }

        for( int i = iStartId; i <= lastId; i++ )
        {
          IncreaseCount( i );
        }
      }
      else
      {
        bResult = IsReservedBy( id, iLastSectorStart, collectionId );
      }

      return bResult;
    }
    /// <summary>
    /// Increases number of reserved shapes in the segment.
    /// </summary>
    /// <param name="id">Id that belongs to the segment to increase number for.</param>
    private void IncreaseCount( int id )
    {
      int iSegmentStart = GetSegmentStart( id );
      int iCurrentCount;

      if( m_idCount.TryGetValue( iSegmentStart, out iCurrentCount ) )
      {
        iCurrentCount++;
      }
      else
      {
        iCurrentCount = 1;
      }

      m_idCount[ iSegmentStart ] = iCurrentCount;
    }
    /// <summary>
    /// Checks whether specified id range is reserved by specified shape collection.
    /// </summary>
    /// <param name="id">Start id to check.</param>
    /// <param name="lastId">End id to check.</param>
    /// <param name="collectionId">Collection id to check.</param>
    /// <returns>True if all ids are reserved by the specified collection.</returns>
    private bool IsReservedBy( int id, int lastId, int collectionId )
    {
      bool bResult = true;

      for( int i = id; i <= lastId && bResult; i += SegmentSize )
      {
        bResult = ReservedBy( id ) == collectionId;
      }

      return bResult;
    }
    /// <summary>
    /// Frees segment that contains specified id.
    /// </summary>
    /// <param name="id">Id used to detect segment start.</param>
    private void FreeSegment( int id )
    {
      int iSegmentStart = GetSegmentStart( id );
      m_id.Remove( iSegmentStart );
    }
    /// <summary>
    /// Frees segments sequence if they belong to specified collection.
    /// </summary>
    /// <param name="id">Id to detect first segment.</param>
    /// <param name="collectionId">Collection to free segments for.</param>
    public void FreeSegmentsSequence( int id, int collectionId )
    {
      //int iSegmentStart = GetSegmentStart( id );
      while( ReservedBy( id ) == collectionId )
      {
        FreeSegment( id );
        id += SegmentSize;
      }
    }
    /// <summary>
    /// Frees all segments allocated by the collection.
    /// </summary>
    /// <param name="collectionId">Collection id.</param>
    public void FreeSequence( int collectionId )
    {
      KeyValuePair<int, int> pair;

      if( m_collectionCount.TryGetValue( collectionId, out pair ) )
      {
        FreeSegmentsSequence( pair.Value, collectionId );
      }
    }
    /// <summary>
    /// Allocates specified number of segments.
    /// </summary>
    /// <param name="idNumber">Ids count to allocate.</param>
    /// <param name="collectionId">Collection id.</param>
    /// <returns>Index to the first allocate id.</returns>
    public int Allocate( int idNumber, int collectionId )
    {
      int iCurrentSegment = SegmentSize; // First segment is not used by MS Excel 97-2003.
      int iSegmentsCount = ( int )Math.Ceiling( idNumber / ( double )SegmentSize );

      while( !CheckFree( iCurrentSegment, iSegmentsCount ) )
      {
        iCurrentSegment += SegmentSize;
      }

      int result = iCurrentSegment;

      //for( int i = 0; i < iSegmentsCount; i++, iCurrentSegment += SegmentSize )
      {
        if( !TryReserve( iCurrentSegment,  iCurrentSegment + idNumber, collectionId ) )
          throw new InvalidOperationException();
      }

      return result;
    }
    /// <summary>
    /// Returns number of reserved ids by specified collection.
    /// </summary>
    /// <param name="collectionId">Collection index.</param>
    /// <returns>Number of reserved ids.</returns>
    public int GetReservedCount( int collectionId )
    {
      KeyValuePair<int, int> pair;

      return ( m_collectionCount.TryGetValue( collectionId, out pair ) ) ?
        pair.Key * SegmentSize :
        0;
    }
    /// <summary>
    /// Gets number of reserved shapes inside specified sector.
    /// </summary>
    /// <param name="id">Id that belongs to the segment that is being investigated.</param>
    /// <returns>Number of reserved ids inside sector.</returns>
    public int ReservedCount( int id )
    {
      int result;
      id = GetSegmentStart( id );
      m_idCount.TryGetValue( id, out result );
      return result;
    }
    /// <summary>
    /// Registers any number of additional shapes required by collection.
    /// </summary>
    /// <param name="collectionIndex">Collection index.</param>
    /// <param name="shapesNumber">Number of additional shapes to add.</param>
    public void AddAdditionalShapes( int collectionIndex, int shapesNumber )
    {
      int currentNumber;

      if( !m_dictAdditionalShapes.TryGetValue( collectionIndex, out currentNumber ) )
        currentNumber = 0;

      m_dictAdditionalShapes[ collectionIndex ] = currentNumber + shapesNumber;
    }
    /// <summary>
    /// Gets number of additional shapes reserved by collection.
    /// </summary>
    /// <param name="collectionIndex">Collection index.</param>
    /// <returns>Number of additional shapes.</returns>
    public int GetAdditionalShapesNumber( int collectionIndex )
    {
      int currentNumber;
      m_dictAdditionalShapes.TryGetValue( collectionIndex, out currentNumber );
      return currentNumber;
    }
    #endregion
  }
}
