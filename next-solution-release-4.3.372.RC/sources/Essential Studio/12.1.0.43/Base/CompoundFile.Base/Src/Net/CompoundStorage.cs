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
using System.Diagnostics;

#if DOCIO
using Syncfusion.CompoundFile.DocIO;
namespace Syncfusion.CompoundFile.DocIO.Net
#else

#if (SILVERLIGHT) && !DOCIO
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif (WP)
using Syncfusion.XlsIO.Implementation.WP;
#endif

namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
  class CompoundStorage :
    ICompoundStorage,
    ICompoundItem
  {
    #region Members
    /// <summary>
    /// 
    /// </summary>
    private CompoundFile m_parentFile;
    /// <summary>
    /// RBTree with child elements.
    /// </summary>
    //private MapCollection m_nodes = new MapCollection( new ItemNamesComparer() );
#if ( WINRT )
    //private SortedList<string, ICompoundItem> m_nodes = new SortedList<string, ICompoundItem>(new ItemNamesComparer());
      //private SortedDictionary<string, ICompoundItem> m_nodes = new SortedDictionary<string, ICompoundItem>();
    System.Collections.Generic.SortedDictionary<string, ICompoundItem> m_nodes = new System.Collections.Generic.SortedDictionary<string, ICompoundItem>(new ItemNamesComparer());
#else
      private SortedList<string, ICompoundItem> m_nodes = new SortedList<string, ICompoundItem>( new ItemNamesComparer() );
#endif
    /// <summary>
    /// 
    /// </summary>
    private DirectoryEntry m_entry;

    // TODO: move this functionality to the base class
    private List<string> m_arrStorages = new List<string>();
    private List<string> m_arrStreams = new List<string>();
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the storage.
    /// </summary>
    /// <param name="parent">Parent file.</param>
    /// <param name="name">Name of the new storage.</param>
    /// <param name="entryIndex">Index to the directory entry that stores storage information.</param>
    public CompoundStorage( CompoundFile parent, string name, int entryIndex )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      m_parentFile = parent;
      m_entry = new DirectoryEntry( name, DirectoryEntry.EntryType.Storage, entryIndex );
    }
    /// <summary>
    /// Initializes new instance of the storage.
    /// </summary>
    /// <param name="parentFile">Parent compound file object.</param>
    /// <param name="entry">Entry that describes current storage.</param>
    public CompoundStorage( CompoundFile parentFile, DirectoryEntry entry )
    {
      if( parentFile == null )
        throw new ArgumentNullException( "parentFile" );

      if( entry == null )
        throw new ArgumentNullException( "entry" );

      if( entry.Type != DirectoryEntry.EntryType.Storage && entry.Type != DirectoryEntry.EntryType.Root )
        throw new ArgumentOutOfRangeException( "entry" );

      m_entry = entry;
      m_parentFile = parentFile;

      AddItem( entry.ChildId );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entryIndex"></param>
    private void AddItem( int entryIndex )
    {
      if( entryIndex < 0 )
        return;

      List<DirectoryEntry> entries = m_parentFile.Directory.Entries;
      DirectoryEntry current = entries[ entryIndex ];
      int iLeftId = current.LeftId;
      string itemName = current.Name;

      // 1. add left
      AddItem( iLeftId );

      // 2. add entry
      switch( current.Type )
      {
        case DirectoryEntry.EntryType.Storage:
          m_nodes.Add( itemName, new CompoundStorage( m_parentFile, current ) );
          m_arrStorages.Add( itemName );
          break;

        case DirectoryEntry.EntryType.Stream:
          if( !m_arrStreams.Contains( itemName ) )
          {
            CompoundStreamNet stream = new CompoundStreamNet( m_parentFile, current );
            m_nodes.Add( itemName, stream );
            m_arrStreams.Add( itemName );
          }
          break;

        default:
          throw new NotImplementedException();
      }

      // 3. add right
      int iRightId = current.RightId;
      AddItem( iRightId );
    }
    #endregion

    #region ICompoundStorage Members
    /// <summary>
    /// Creates new stream.
    /// </summary>
    /// <param name="streamName">Name of the stream to create.</param>
    /// <returns>Created stream.</returns>
    public CompoundStream CreateStream( string streamName )
    {
      // 1. check whether name is not used
      if( ContainsStream( streamName ) || ContainsStorage( streamName ) )
        throw new ArgumentOutOfRangeException( "streamName", "Object with such name already exists" );

      // 2. Find free directory entry and use it
      DirectoryEntry entry = m_parentFile.AllocateDirectoryEntry( streamName, DirectoryEntry.EntryType.Stream );

      // 3. Update nodes and streams
      CompoundStreamNet stream = ( m_parentFile.DirectMode ) ?
        new CompoundStreamDirect( m_parentFile, entry ) :
        new CompoundStreamNet( m_parentFile, entry );
      m_arrStreams.Add( streamName );
      m_nodes.Add( streamName, stream );
      stream.Open();

      return new CompoundStreamWrapper( stream );
    }
    /// <summary>
    /// Opens stream.
    /// </summary>
    /// <param name="streamName">Name of the stream to open.</param>
    /// <returns>Opened stream or null if there is no such stream.</returns>
    public CompoundStream OpenStream( string streamName )
    {
      CompoundStreamNet result = m_nodes[ streamName ] as CompoundStreamNet;

      if( result != null )
        result.Open();

      return new CompoundStreamWrapper( result );
    }
    /// <summary>
    /// Removes stream from the storage, if it contains stream with such name.
    /// </summary>
    /// <param name="streamName">Stream name to delete.</param>
    public void DeleteStream( string streamName )
    {
      CompoundStreamNet stream = m_nodes[ streamName ] as CompoundStreamNet;

      if( stream != null )
      {
        m_parentFile.RemoveItem( stream.Entry );
        stream.Dispose();
        m_nodes.Remove( streamName );
      }
    }
    /// <summary>
    /// Checks whether storage contains stream with specified name.
    /// </summary>
    /// <param name="streamName">Name of the stream to check.</param>
    /// <returns>True if storage has stream with such name; false otherwise.</returns>
    public bool ContainsStream( string streamName )
    {
      //CompoundStream result = m_nodes[ streamName ] as CompoundStream;
      //return ( result != null );
      return m_nodes.ContainsKey( streamName );
    }

    public ICompoundStorage CreateStorage( string storageName )
    {
      // TODO: combine with CreateStream method.
      // 1. check whether name is not used
      if( ContainsStream( storageName ) || ContainsStorage( storageName ) )
        throw new ArgumentOutOfRangeException( "streamName", "Object with such name already exists" );

      // 2. Find free directory entry and use it
      DirectoryEntry entry = m_parentFile.AllocateDirectoryEntry( storageName, DirectoryEntry.EntryType.Storage );
      entry.DateCreate = entry.DateModify = DateTime.Now;

      // 3. Update nodes and streams
      //CompoundStreamNet stream = new CompoundStreamNet( m_parentFile, entry );
      CompoundStorage storage = new CompoundStorage( m_parentFile, entry );
      m_arrStorages.Add( storageName );
      m_nodes.Add( storageName, storage );
      storage.Open();

      // TODO: maybe we need wrapper.
      return new CompoundStorageWrapper( storage );
    }
    /// <summary>
    /// Opens existing storage.
    /// </summary>
    /// <param name="storageName">Name of the storage to open.</param>
    /// <returns>Opened storage item or null if it was impossible to open it.</returns>
    public ICompoundStorage OpenStorage( string storageName )
    {
      CompoundStorage result = m_nodes[ storageName ] as CompoundStorage;

      if( result != null )
        result.Open();

      return new CompoundStorageWrapper( result );
    }

    private void Open()
    {
      //AddItem( m_entry.ChildId );
      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Removes substorage from existing storage.
    /// </summary>
    /// <param name="storageName">Name of the storage to remove.</param>
    public void DeleteStorage( string storageName )
    {
      CompoundStorage storage = m_nodes[ storageName ] as CompoundStorage;

      if( storage != null )
      {
        storage.Dispose();
        m_nodes.Remove( storageName );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
      if( m_parentFile != null )
      {
        m_parentFile = null;
        m_nodes = null;
        m_entry = null;
        GC.SuppressFinalize( this );
      }
    }
    /// <summary>
    /// Checks whether this storage contains substorage with specified name.
    /// </summary>
    /// <param name="storageName">Name to check.</param>
    /// <returns>True if there is such storage; false otherwise.</returns>
    public bool ContainsStorage( string storageName )
    {
      //CompoundStorage storage = m_nodes[ storageName ] as CompoundStorage;
      //return storage != null;
      return m_nodes.ContainsKey( storageName );
    }
    /// <summary>
    /// 
    /// </summary>
    public void Flush()
    {
      m_entry.LeftId = -1;

      foreach( ICompoundItem item in m_nodes.Values )
      {
        item.Flush();
      }

      ICompoundItem prev = null;
      foreach( ICompoundItem item in m_nodes.Values )
      {
        if( prev != null )
        {
          prev.Entry.RightId  = item.Entry.EntryId;
          prev.Entry.LeftId = -1;
        }
        else
        {
          m_entry.ChildId = item.Entry.EntryId;
        }

        prev = item;
      }
      //foreach( RBTreeNode node in m_nodes )
      //{
      //  ICompoundItem item = node.Value as ICompoundItem;
      //  item.Flush();
      //}

      //// TODO: rebuild tree and update directory entries
      ////m_nodes.ForAll( UpdateDirectory );
      //RBTreeNode prev = null;

      //foreach( RBTreeNode node in m_nodes )
      //{
      //  if( !node.IsNil )
      //  {
      //    if( prev != null )
      //    {
      //      ( prev.Value as ICompoundItem ).Entry.RightId = ( node.Value as ICompoundItem ).Entry.EntryId;
      //    }
      //    else
      //    {
      //      m_entry.ChildId = ( node.Value as ICompoundItem ).Entry.EntryId;
      //    }

      //    prev = node;
      //  }
      //}
    }

    private void UpdateDirectory( RBTreeNode node )
    {
      object value = node.Value;

      if( value != null )
      {
        ICompoundItem compoundItem = value as ICompoundItem;
        DirectoryEntry entry = compoundItem.Entry;

        entry.Color = ( byte )node.Color;

        entry.LeftId = GetNodeId( node.Left );
        entry.RightId = GetNodeId( node.Right );

        if( m_entry.ChildId < 0 )
        {
          m_entry.ChildId = entry.EntryId;
        }

        CompoundStreamNet stream = value as CompoundStreamNet;
        if( stream != null )
        {
          entry.Size = ( uint )stream.Length;
        }
      }
    }
    /// <summary>
    /// Returns directory entry id that corresponds to the specified node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private int GetNodeId( RBTreeNode node )
    {
      return ( !node.IsNil ) ?
        ( node.Value as ICompoundItem ).Entry.EntryId :
        -1;
    }

    public string[] Streams
    {
      get
      {
        return m_arrStreams.ToArray();
      }
    }

    public string[] Storages
    {
      get
      {
        return m_arrStorages.ToArray();
      }
    }
    /// <summary>
    /// Returns name of the storage.
    /// </summary>
    public string Name
    {
      get
      {
        return m_entry.Name;
      }
    }
    /// <summary>
    /// Returns directory entry for this stream.
    /// </summary>
    public DirectoryEntry Entry
    {
      get
      {
        return m_entry;
      }
    }
    public void InsertCopy( ICompoundStorage storageToCopy )
    {
      //throw new NotImplementedException();
      // TODO: implement case when we have such storage.
      ICompoundStorage storage = CreateStorage( storageToCopy.Name ) as ICompoundStorage;

      string[] arrStreams = storageToCopy.Streams;
      for( int i = 0, len = arrStreams.Length; i < len; i++ )
      {
        using( CompoundStream stream = storageToCopy.OpenStream( arrStreams[ i ] ) )
        {
          storage.InsertCopy( stream );
        }
      }

      string[] arrStorages = storageToCopy.Storages;
      for( int i = 0, len = arrStorages.Length; i < len; i++ )
      {
        using( ICompoundStorage sourceStorage = storageToCopy.OpenStorage( arrStorages[ i ] ) )
        {
          storage.InsertCopy( sourceStorage );
        }
      }
    }
    public void InsertCopy( CompoundStream streamToCopy )
    {
      if( streamToCopy == null )
        throw new ArgumentNullException( "streamToCopy" );

      // TODO: implement case when we have such stream.
      CompoundStream stream = CreateStream( streamToCopy.Name );

      const int BufferSize = 32768;
      byte[] arrBuffer = new byte[ BufferSize ];

      long lStartPosition = streamToCopy.Position;
      streamToCopy.Position = 0;
      int iReadCount;

      while( ( iReadCount = streamToCopy.Read( arrBuffer, 0, BufferSize ) ) > 0 )
      {
        stream.Write( arrBuffer, 0, iReadCount );
      }

      streamToCopy.Position = lStartPosition;
    }
    #endregion
  }
}
