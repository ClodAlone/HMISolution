#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
//using Syncfusion.XlsIO.IO.Stream;

using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Interfaces;

using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.XlsIO.Implementation.Collections;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
#endregion

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
  /// <summary>
  /// Summary description for PivotCachesCollection.
  /// </summary>
  public class PivotCacheCollection :
    //CollectionBaseEx<PivotCacheImpl>,
    ICloneParent,
    IPivotCaches,
    IEnumerable<PivotCacheImpl>,
    IParentApplication
  {
    #region Class constants
    /// <summary>
    /// Name of sub-storage in input file which contains pivot table cache data.
    /// </summary>
    public const string DEF_PIVOT_CACHE_STORAGE = "_SX_DB_CUR";
    #endregion

    #region Members
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Dictionary with pivot caches. Key - cache id, value - cache.
    /// </summary>
    private Dictionary<int, PivotCacheImpl> m_dictCaches= new Dictionary<int,PivotCacheImpl>();
    /// <summary>
    /// List with caches order.
    /// </summary>
    private List<int> m_arrOrder = new List<int>();
    #endregion

    #region Properteis
    /// <summary>
    /// Returns single entry from the collection.
    /// </summary>
    /// <param name="id">Item id to return.</param>
    /// <returns>An entry from the collection.</returns>
    IPivotCache IPivotCaches.this[ int id ]
    {
      get
      {
        //if( index < 0 || index >= Count )
          //throw new ArgumentOutOfRangeException( "index" );

        return m_dictCaches[ id ];//( IPivotCache )List[ index ];
      }
    }
    /// <summary>
    /// Returns single entry from the collection.
    /// </summary>
    /// <param name="id">Item id to return.</param>
    /// <returns>An entry from the collection.</returns>
    public PivotCacheImpl this[ int id ]
    {
      get
      {
          if (m_dictCaches.ContainsKey(id))
              return m_dictCaches[id];
          else
              return null;
      }
    }
    /// <summary>
    /// Gets number of items in the collection.
    /// </summary>
    public int Count
    {
      get
      {
        return m_dictCaches.Count;
      }
    }
    /// <summary>
    /// Gets application object.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return m_book.Application;
      }
    }
    /// <summary>
    /// Gets parent object.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Gets ordered list of pivot caches.
    /// </summary>
    public List<int> Order
    {
      get
      {
        return m_arrOrder;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates collection and sets its Application and Parent values.
    /// </summary>
    /// <param name="application">
    /// Application object that represents the Excel application.
    /// </param>
    /// <param name="parent">Parent object of this collection.</param>
    public PivotCacheCollection( IApplication application, object parent )
    {
      m_book = FindParent( parent );
    }
    /// <summary>
    /// Creates collection and sets its Application and Parent values.
    /// </summary>
    /// <param name="application">
    /// Application object that represents the Excel application.
    /// </param>
    /// <param name="parent">Parent object of this collection.</param>
    /// <param name="storage">Pivot caches storage.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    public PivotCacheCollection( IApplication application, object parent,
      ICompoundStorage storage, IDecryptor decryptor )
      : this( application, parent )
    {
      Parse( storage, decryptor );
    }
    #endregion

    #region Methods
    /// <summary>
    /// Parses pivot caches storage.
    /// </summary>
    /// <param name="storage">Storage to parse.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    public void Parse( ICompoundStorage storage, IDecryptor decryptor )
    {
      if( storage == null )
        throw new ArgumentNullException( "storage" );

      Clear();
      ICompoundStorage pivotStorage = null;

      if( storage.ContainsStorage( DEF_PIVOT_CACHE_STORAGE ) )
      {
        using( pivotStorage = storage.OpenStorage( DEF_PIVOT_CACHE_STORAGE ) )
        {
          string[] arrSubStreams = pivotStorage.Streams;

          for( int i = 0, len = arrSubStreams.Length; i < len; i++ )
          {
            string strStreamName = arrSubStreams[ i ];

            using( CompoundStream pivotStream = pivotStorage.OpenStream( strStreamName ) )
            {
              using( BiffReader reader = new BiffReader( pivotStream ) )
              {
                  PivotCacheImpl cache = new PivotCacheImpl(Application, this, reader, decryptor, strStreamName);
                Add( strStreamName, cache );
              }
            }
          }
        }
      }
    }
    /// <summary>
    /// Clears collection
    /// </summary>
    public void Clear()
    {
      m_dictCaches.Clear();
    }
    /// <summary>
    /// Serializes cache.
    /// </summary>
    /// <param name="storage">Storage to serialize into.</param>
    /// <param name="encryptor">Object that is used to encrypt data.</param>
    public void Serialize( ICompoundStorage storage, IEncryptor encryptor )
    {
      if( storage == null )
        throw new ArgumentNullException( "storage" );

      int iCount = Count;

      if( iCount == 0 )
        return;

      using( ICompoundStorage pivotStorage = storage.CreateStorage( DEF_PIVOT_CACHE_STORAGE ) )
      {
        //for( int i = 0; i < iCount; i++ )
        foreach( PivotCacheImpl cache in m_dictCaches.Values )
        {
          //PivotCacheImpl cache = pair.Value;//( PivotCacheImpl )List[ i ];
          ushort usId = cache.StreamId;
          string strStreamName = usId.ToString( "X4" );

          using( CompoundStream pivotStream = pivotStorage.CreateStream( strStreamName ) )
          {
            //pivotStream.OpenStream( strStreamName );

            OffsetArrayList records = new OffsetArrayList();
            cache.Serialize( records );

            using( BiffWriter writer = new BiffWriter( pivotStream ) )
            {
              writer.WriteRecord( records, encryptor );
            }
          }
        }
      }
    }
    /// <summary>
    /// Adds single item to the collection.
    /// </summary>
    /// <param name="cache">Item to add.</param>
    public void Add( PivotCacheImpl cache )
    {
      //cache.Index = Count;
      //base.Add( cache );
      int index = cache.Index = GetFreeIndex( cache );
      m_dictCaches.Add( index, cache );
      //m_arrOrder.Add( index );
    }
    /// <summary>
    /// Adds single item to the collection.
    /// </summary>
    /// <param name="cache">Item to add.</param>
    public void Add(int index,PivotCacheImpl cache)
    {
        //cache.Index = Count;
        //base.Add( cache );
        int iIndex = cache.Index = index;
        m_dictCaches.Add(iIndex, cache);
        //m_arrOrder.Add( index );
    }
    /// <summary>
    /// Searches for the first available pivot cache index.
    /// </summary>
    /// <param name="cache">Cache to search index for.</param>
    /// <returns>Found free index.</returns>
    private int GetFreeIndex( PivotCacheImpl cache )
    {
      int streamId = cache.StreamId;
      FileDataHolder holder = m_book.DataHolder;
      Dictionary<string, string> preservedCaches = null;

      if( holder != null )
        preservedCaches = holder.PreservedCaches;

      while( m_dictCaches.ContainsKey( streamId ) ||
        ( preservedCaches != null && preservedCaches.ContainsKey( ( streamId + 1 ).ToString() ) ) )
      {
        streamId++;
      }

      cache.StreamId = ( ushort )streamId;
      return streamId;
    }
    /// <summary>
    /// Creates new chache object inside this collection.
    /// </summary>
    /// <param name="range">Range that contains data to cache.</param>
    /// <returns>Newly created object.</returns>
    public IPivotCache Add( IRange range )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      PivotCacheImpl cache = new PivotCacheImpl( Application, this, range );
      Add( cache );
      m_arrOrder.Add( cache.Index );
      return cache;
    }
    /// <summary>
    /// Adds new cache to the collection.
    /// </summary>
    /// <param name="streamName">Name of the stream.</param>
    /// <param name="cache">Cache object to add.</param>
    private void Add( string streamName, PivotCacheImpl cache )
    {
      Add( cache );
      //base.Add( cache );
      //m_hashNameCache.Add( strStreamName, cache );
    }
    /// <summary>
    /// Checks whether cache exist in the collection and adds its copy if necessary.
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="hashWorksheetNames"></param>
    /// <returns></returns>
    internal int CheckAndAddCache( PivotCacheImpl cache, Dictionary<string, string> hashWorksheetNames )
    {
      IRange source = cache.SourceRange;
      PivotCacheImpl result = null;

      if( source != null )
      {
        string sheetName = source.Worksheet.Name;

        if( hashWorksheetNames != null )
        {
          if( hashWorksheetNames.ContainsKey( sheetName ) )
          {
            sheetName = hashWorksheetNames[ sheetName ];
          }
        }

        IRange newSource = m_book.Worksheets[ sheetName ][ source.AddressLocal ];

        for( int i = 0, len = Count; i < len; i++ )
        {
          PivotCacheImpl currentCache = this[ i ];

          if( currentCache.SourceRange.AddressGlobal == newSource.AddressGlobal )
          {
            result = currentCache;
            break;
          }
        }

        if( result == null )
          result = ( PivotCacheImpl )Add( newSource );
      }
      else
      {
        // We have to clone records!!!
        for( int i = 0, len = Count; i < len; i++ )
        {
          PivotCacheImpl currentCache = this[ i ];

          if( currentCache.ComparePreservedData( cache ) )
          {
            result = currentCache;
            break;
          }
        }

        if( result == null )
        {
          result = ( PivotCacheImpl )cache.Clone( this );
          Add( result );
        }
      }

      return result.Index;
    }
      /// <summary>
      /// Removes the Pivot cache.
      /// </summary>
      /// <param name="index"></param>
    public void RemoveAt(int index)
    {
        m_dictCaches.Remove(index);
    }
      /// <summary>
      /// Get Cache indexes
      /// </summary>
      /// <returns>cache indexes</returns>
    public int[] GetIndexes()
    {
        int []indexes=new int[m_dictCaches.Count];
        m_dictCaches.Keys.CopyTo(indexes, 0);
        return indexes;
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent )
    {
      PivotCacheCollection result = new PivotCacheCollection(this.m_book.Application, this.m_book);
      List < PivotCacheImpl > currentCollection = new List<PivotCacheImpl>();
      result.m_book = FindParent( parent );

      foreach( PivotCacheImpl cache in m_dictCaches.Values )
      {
        PivotCacheImpl newCache = ( PivotCacheImpl )cache.Clone( result );
        //result.Add( newCache );
        currentCollection.Add(newCache);
      }

        foreach (PivotCacheImpl cache_1 in currentCollection)
      {
          result.Add(cache_1.Index, cache_1);
      }
      return result;
    }
    /// <summary>
    /// Searches for the parent workbook.
    /// </summary>
    /// <param name="parent">Parent workbook to start searching from.</param>
    /// <returns>Found workbook.</returns>
    private WorkbookImpl FindParent( object parent )
    {
      WorkbookImpl book = ( WorkbookImpl )CommonObject.FindParent( parent, typeof( WorkbookImpl ) );

      if( book == null )
        throw new ArgumentOutOfRangeException( "Cannot find parent workbook" );

      return book;
    }

    #endregion

    #region IEnumerable<PivotCacheImpl> Members

    public IEnumerator<PivotCacheImpl> GetEnumerator()
    {
      foreach( PivotCacheImpl cache in m_dictCaches.Values )
      {
        yield return cache;
      }
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      foreach( PivotCacheImpl cache in m_dictCaches.Values )
      {
        yield return cache;
      }
    }

    #endregion
  }
}
