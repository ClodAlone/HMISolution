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
using System.IO;

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
  /// <summary>
  /// This class represents directory structure in the compound file.
  /// </summary>
  public class Directory
  {
    #region Members
    /// <summary>
    /// List of directory entries.
    /// </summary>
    List<DirectoryEntry> m_lstEntries = new List<DirectoryEntry>();
    #endregion

    #region Properties
    /// <summary>
    /// Returns list of directory entries.
    /// </summary>
    public List<DirectoryEntry> Entries
    {
      get
      {
        return m_lstEntries;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Directory()
    {
    }
    /// <summary>
    /// Initializes new instance of the directory.
    /// </summary>
    /// <param name="data">Data to parse.</param>
    public Directory( byte[] data )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      int iOffset = 0;
      int iLength = data.Length;
      m_lstEntries = new List<DirectoryEntry>();
      int index = 0;

      while( iOffset < iLength )
      {
        m_lstEntries.Add( new DirectoryEntry( data, iOffset, index ) );
        iOffset += DirectoryEntry.SizeInFile;
        index++;
      }
    }
    /// <summary>
    /// Searches for empty entry index.
    /// </summary>
    /// <returns>Index of the first empty directory entry.</returns>
    public int FindEmpty()
    {
      int iResult = -1;
      for( int i = 0, len = m_lstEntries.Count; i < len; i++ )
      {
        DirectoryEntry entry = m_lstEntries[ i ];

        if( entry.Type == DirectoryEntry.EntryType.Invalid )
        {
          iResult = i;
          break;
        }
      }

      return iResult;
    }
    /// <summary>
    /// Adds new entry to the collection or replaces existing empty entry with this one.
    /// </summary>
    /// <param name="entry">Entry to add.</param>
    public void Add( DirectoryEntry entry )
    {
      int iEntryIndex = FindEmpty();

      if( iEntryIndex >= 0 )
      {
        m_lstEntries[ iEntryIndex ] = entry;
        entry.EntryId = iEntryIndex;
      }
      else
      {
        m_lstEntries.Add( entry );
      }
    }
    /// <summary>
    /// Saves directory entries into specified stream.
    /// </summary>
    /// <param name="stream">Stream to save directory into.</param>
    public void Write( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      for( int i = 0, len = m_lstEntries.Count; i < len; i++ )
      {
        DirectoryEntry entry = m_lstEntries[ i ];
        entry.Write( stream );
      }
    }
    #endregion
  }
}
