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
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
#endregion

namespace Syncfusion.Compression.Zip
{
  /// <summary>
  /// Represents zip archive.
  /// </summary>
  public class ZipArchive : IDisposable
  {
    #region Members
    /// <summary>
    /// Collection of archive items.
    /// </summary>
    private List<ZipArchiveItem> m_arrItems = new List<ZipArchiveItem>();
    /// <summary>
    /// Dictionary that allows quick search operations by item name.
    /// Key - item name,
    /// Value - corresponding ZipArchiveItem.
    /// </summary>
    private Dictionary<string, ZipArchiveItem> m_dicItems = new Dictionary<string, ZipArchiveItem>();
    /// <summary>
    /// File name preprocessor - object that converts full file/folder
    /// name into value that will be written into zip archive.
    /// </summary>
    private IFileNamePreprocessor m_fileNamePreprocessor;
    /// <summary>
    /// Indicates whether we should check Crc value when reading item's data. Check
    /// is performed when user gets access to decompressed data for the first time.
    /// </summary>
    private bool m_bCheckCrc = true;
    /// <summary>
    /// Default compression level.
    /// </summary>
    private CompressionLevel m_defaultLevel = CompressionLevel.Best;
    /// <summary>
    /// Compresses files using custom NetCompressor.
    /// </summary>
    private bool m_netCompression = false;
    #endregion

    #region Delegates
    public delegate Stream CompressorCreator( Stream outputStream );
    #endregion

    #region Properties
    /// <summary>
    /// Returns single archive item from the collection. Read-only.
    /// </summary>
    /// <param name="index">Zero-based index of the item to return.</param>
    /// <returns>Single archive item from the collection.</returns>
    public ZipArchiveItem this[ int index ]
    {
      get
      {
        if( index < 0 || index > m_arrItems.Count )
          throw new ArgumentOutOfRangeException( "index" );

        return m_arrItems[ index ];
      }
    }
    /// <summary>
    /// Returns item by its name. Null if item wasn't found. Read-only.
    /// </summary>
    public ZipArchiveItem this[ string itemName ]
    {
      get
      {
        ZipArchiveItem result;
        m_dicItems.TryGetValue( itemName, out result );
        return result;
      }
    }
    /// <summary>
    /// Returns number of items inside archive. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        return ( m_arrItems != null ) ? m_arrItems.Count : 0;
      }
    }
	
	/// <summary>
    /// Returns the items inside archive. Read-only.
    /// </summary>
	public ZipArchiveItem[] Items
    {
        get
        {
            if (m_arrItems != null)
                return m_arrItems.ToArray();
            else
                throw new ArgumentOutOfRangeException("Items");
        }
    }

    /// <summary>
    /// Gets / sets file name preprocessor - object that converts full file/folder
    /// name into value that will be written into zip archive.
    /// </summary>
    public IFileNamePreprocessor FileNamePreprocessor
    {
      get
      {
        return m_fileNamePreprocessor;
      }
      set
      {
        m_fileNamePreprocessor = value;
      }
    }
    /// <summary>
    /// Gets / sets default compression level - compression level for new items.
    /// By default is equal to CompressionLevel.Best.
    /// </summary>
    public CompressionLevel DefaultCompressionLevel
    {
      get
      {
        return m_defaultLevel;
      }
      set
      {
        m_defaultLevel = value;
      }
    }
    /// <summary>
    /// Indicates whether we should check Crc value when reading item's data. Check
    /// is performed when user gets access to item's decompressed data for the first time.
    /// </summary>
    public bool CheckCrc
    {
      get
      {
        return m_bCheckCrc;
      }
      set
      {
        m_bCheckCrc = value;
      }
    }
    /// <summary>
    /// Uses custom compressed stream reader and writer.
    /// </summary>
    public bool UseNetCompression
    {
        get
        {
            return m_netCompression;
        }
        set
        {
            m_netCompression = value;
        }
    }
    /// <summary>
    /// Creates compressor.
    /// </summary>
    public CompressorCreator CreateCompressor;
    #endregion

    #region Static methods
    /// <summary>
    /// Searches for integer value from the end of the stream.
    /// </summary>
    /// <param name="stream">Stream to search value in.</param>
    /// <param name="value">Value to locate.</param>
    /// <param name="maxCount">Maximum number of bytes to scan.</param>
    /// <returns>Offset to the value, or -1 if it wasn't found.</returns>
    [ CLSCompliant( false ) ]
    public static long FindValueFromEnd( Stream stream, uint value, int maxCount )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( !stream.CanSeek || !stream.CanRead )
        throw new ArgumentOutOfRangeException( "We need to have seekable and readable stream." );

      // read last 4 bytes and compare with required value
      long lStreamSize = stream.Length;

      if( lStreamSize < 4 )
        return -1;

      byte[] arrBuffer = new byte[ 4 ];
      long lLastPos = Math.Max( 0, lStreamSize - maxCount );
      long lCurrentPosition = lStreamSize - 1 - Constants.IntSize;

      stream.Position = lCurrentPosition;
      stream.Read( arrBuffer, 0, Constants.IntSize );
      uint uiCurValue = BitConverter.ToUInt32( arrBuffer, 0 );
      bool bFound = ( uiCurValue == value );

      if( !bFound )
      {
        while( lCurrentPosition > lLastPos )
        {
          // remove unnecessary byte and replace it with new value.
          uiCurValue <<= 8;
          lCurrentPosition--;
          stream.Position = lCurrentPosition;
          uiCurValue += ( uint )stream.ReadByte();

          if( uiCurValue == value )
          {
            bFound = true;
            break;
          }
        }
      }

      return bFound ? lCurrentPosition : -1;
    }
    /// <summary>
    /// Extracts Int32 value from the stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <returns>Extracted value.</returns>
    public static int ReadInt32( Stream stream )
    {
      byte[] buffer = new byte[ Constants.IntSize ];
      if (stream.Read(buffer, 0, Constants.IntSize) != Constants.IntSize)
      {
        throw new ZipException( "Unable to read value at the specified position - end of stream was reached." );
      }

      return BitConverter.ToInt32(buffer, 0);
    }
    /// <summary>
    /// Extracts Int16 value from the stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <returns>Extracted value.</returns>
    public static short ReadInt16( Stream stream )
    {
        byte[] buffer = new byte[Constants.ShortSize];
        if (stream.Read(buffer, 0, Constants.ShortSize) != Constants.ShortSize)
      {
        throw new ZipException( "Unable to read value at the specified position - end of stream was reached." );
      }

        return BitConverter.ToInt16(buffer, 0);
    }
    /// <summary>
    /// Extracts unsigned Int16 value from the stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <returns>Extracted value.</returns>
    public static ushort ReadUInt16(Stream stream)
    {
        byte[] buffer = new byte[Constants.ShortSize];
        if (stream.Read(buffer, 0, Constants.ShortSize) != Constants.ShortSize)
        {
            throw new ZipException("Unable to read value at the specified position - end of stream was reached.");
        }
        
        return BitConverter.ToUInt16(buffer, 0);
    }
    #endregion

    #region Methods
    public ZipArchive()
    {
      CreateCompressor = CreateNativeCompressor;
    }
    private Stream CreateNativeCompressor( Stream outputStream )
    {
        if (m_netCompression)
            return new NetCompressor(CompressionLevel.Best, outputStream);
#if !(SILVERLIGHT || WP)
      return new System.IO.Compression.DeflateStream( outputStream, System.IO.Compression.CompressionMode.Compress, true );
#else
      return new NetCompressor( CompressionLevel.Best, outputStream );
#endif
    }
    /// <summary>
    /// Adds empty directory to the archive.
    /// </summary>
    /// <param name="directoryName">Directory path.</param>
    /// <returns>Item that has been added.</returns>
    public ZipArchiveItem AddDirectory( string directoryName )
    {
      if( directoryName == null || directoryName.Length == 0 )
        throw new ArgumentOutOfRangeException( "directoryName" );

      DirectoryInfo info = new DirectoryInfo( directoryName );
      FileAttributes attributes = info.Attributes;
        
      if(!Directory.Exists(directoryName))
          attributes = FileAttributes.Directory;
      
      if( m_fileNamePreprocessor != null )
      {
        directoryName = m_fileNamePreprocessor.PreprocessName( directoryName );
      }

      return AddItem( directoryName, null, false, attributes );
    }
    /// <summary>
    /// Adds specified file to the archive.
    /// </summary>
    /// <param name="fileName">File to add.</param>
    /// <returns>Item that has been added.</returns>
    public ZipArchiveItem AddFile( string fileName )
    {
      Stream stream = new FileStream( fileName, FileMode.Open, FileAccess.Read );
      FileInfo info = new FileInfo( fileName );
      FileAttributes attributes = info.Attributes;

      if( m_fileNamePreprocessor != null )
      {
        fileName = m_fileNamePreprocessor.PreprocessName( fileName );
      }
      fileName = Path.GetFileName(fileName);
      return AddItem( fileName, stream, true, attributes );
    }
    /// <summary>
    /// Adds new item to the archive
    /// </summary>
    /// <param name="itemName">Item name to add.</param>
    /// <param name="data">Items data stream (can be null for empty files or folders).</param>
    /// <param name="bControlStream">Indicates whether ZipArchive is responsible for stream closing.</param>
    /// <param name="attributes">File attributes.</param>
    /// <returns>Item that has been added.</returns>
    public ZipArchiveItem AddItem( string itemName, Stream data, bool bControlStream, FileAttributes attributes )
    {
      itemName = itemName.Replace( '\\', '/' );

      if( itemName.IndexOf( ':' ) != itemName.LastIndexOf(':') )
        throw new ArgumentOutOfRangeException( "ZipItem name contains illegal characters.", "itemName" );

      if( m_dicItems.ContainsKey( itemName ) )
        throw new ArgumentOutOfRangeException( "Item " + itemName + " already exists in the archive" );

      ZipArchiveItem item = new ZipArchiveItem( this, itemName, data, bControlStream, attributes );
      item.CompressionLevel = m_defaultLevel;
      return AddItem( item );
    }
    /// <summary>
    /// Adds existing item to the archive.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <returns>Added item.</returns>
    public ZipArchiveItem AddItem( ZipArchiveItem item )
    {
      if( item == null )
        throw new ArgumentNullException( "item" );

      m_arrItems.Add( item );
      m_dicItems.Add( item.ItemName, item );
      return item;
    }
    /// <summary>
    /// Removes item from the archive.
    /// </summary>
    /// <param name="itemName">Item name to remove.</param>
    public void RemoveItem( string itemName )
    {
      int iItemIndex = Find( itemName );

      if( iItemIndex >= 0 )
      {
        RemoveAt( iItemIndex );
      }
    }
    /// <summary>
    /// Removes item at the specified position.
    /// </summary>
    /// <param name="index">Item index to remove.</param>
    public void RemoveAt( int index )
    {
      if( index < 0 || index >= m_arrItems.Count )
        throw new ArgumentOutOfRangeException( "index" );

      ZipArchiveItem item = this[ index ];
      m_arrItems.RemoveAt( index );
      m_dicItems.Remove( item.ItemName );
    }
    /// <summary>
    /// Removes items that matches specified regular expression from the collection.
    /// </summary>
    /// <param name="mask">Regular expression used to decide whether to remove item or not.</param>
    public void Remove( Regex mask )
    {
      for( int i = 0, len = m_arrItems.Count; i < len; i++ )
      {
        ZipArchiveItem item = m_arrItems[ i ];
        string strItemName = item.ItemName;

        if( mask.IsMatch( strItemName ) )
        {
          //RemoveAt( i );
          m_arrItems.RemoveAt( i );
          m_dicItems.Remove( strItemName );
          i--;
          len--;
        }
      }
    }
    /// <summary>
    /// Updates item inside existing archive.
    /// </summary>
    /// <param name="itemName">Item name to update.</param>
    /// <param name="newDataStream">New data for the item.</param>
    /// <param name="controlStream">Indicates whether item should control its stream after update.</param>
    public void UpdateItem( string itemName, Stream newDataStream, bool controlStream )
    {
      ZipArchiveItem item = this[ itemName ];

      if( item == null )
        throw new ArgumentOutOfRangeException( "itemName", "Cannot find specified item." );

      item.Update( newDataStream, controlStream );
    }
    /// <summary>
    /// Updates existing item or creates new one.
    /// </summary>
    /// <param name="itemName">Item to update or create.</param>
    /// <param name="newDataStream">New data for the item.</param>
    /// <param name="controlStream">Indicates whether item should control its stream after update.</param>
    /// <param name="attributes">File attributes for the item. This argument is only used if item is created.</param>
    public void UpdateItem( string itemName, Stream newDataStream, bool controlStream,
      FileAttributes attributes )
    {
      ZipArchiveItem item = this[ itemName ];

      if( item != null )
      {
        item.Update( newDataStream, controlStream );
      }
      else
      {
        AddItem( itemName, newDataStream, controlStream, attributes );
      }
    }
    /// <summary>
    /// Updates item inside existing archive.
    /// </summary>
    /// <param name="itemName">Item name to update.</param>
    /// <param name="newData">New data for the item.</param>
    public void UpdateItem( string itemName, byte[] newData )
    {
      ZipArchiveItem item = this[ itemName ];

      if( item == null )
        throw new ArgumentOutOfRangeException( "itemName", "Cannot find specified item." );

      MemoryStream newDataStream = new MemoryStream( newData );
      item.Update( newDataStream, true );
    }
    /// <summary>
    /// Saves archive into specified file.
    /// </summary>
    /// <param name="outputFileName">Output file name.</param>
    public void Save( string outputFileName )
    {
      if( outputFileName == null || outputFileName.Length == 0 )
      {
        throw new ArgumentOutOfRangeException( "outputFileName" );
      }

      Save( outputFileName, false );
    }
    /// <summary>
    /// Saves archive into specified file.
    /// </summary>
    /// <param name="outputFileName">Output file name.</param>
    /// <param name="createFilePath">Indicates whether we should create full path to the file if it doesn't exist.</param>
    public void Save( string outputFileName, bool createFilePath )
    {
      if( outputFileName == null || outputFileName.Length == 0 )
      {
        throw new ArgumentOutOfRangeException( "outputFileName" );
      }

      if( createFilePath )
      {
        string strPath = Path.GetFullPath( outputFileName );
        string strFolderName = Path.GetDirectoryName( strPath );

        if( !Directory.Exists( strFolderName ) )
        {
          Directory.CreateDirectory( strFolderName );
        }
      }

      using( FileStream stream = new FileStream( outputFileName, FileMode.Create, FileAccess.Write ) )
      {
        Save( stream, false );
      }
    }
    /// <summary>
    /// Saves archive into specified stream.
    /// </summary>
    /// <param name="stream">Output stream.</param>
    /// <param name="closeStream">Indicates whether method should close stream after saving.</param>
    public void Save( Stream stream, bool closeStream )
    {
      if( stream == null )
      {
        throw new ArgumentNullException();
      }

      Stream originalStream = null;

      if( !stream.CanSeek )
      {
        originalStream = stream;
        stream = new MemoryStream();
      }

      stream.Position = 0;

      for( int i = 0, len = m_arrItems.Count ; i < len ; i++ )
      {
        ZipArchiveItem item = m_arrItems[ i ];
        item.Write( stream );
      }

      WriteCentralDirectory( stream );

      if( originalStream != null )
      {
        stream.Position = 0;
        ( ( MemoryStream )stream ).WriteTo( originalStream );
        stream.Close();
        stream = originalStream;
      }

      if( closeStream )
      {
        stream.Close();
      }
    }
    /// <summary>
    /// Reads archive data from the file.
    /// </summary>
    /// <param name="inputFileName">Filename to read.</param>
    public void Open( string inputFileName )
    {
      if( inputFileName == null || inputFileName.Length == 0 )
        throw new ArgumentOutOfRangeException( "inputFileName" );

      using( FileStream stream = new FileStream( inputFileName, FileMode.Open, FileAccess.Read ) )
      {
        Open( stream, false );
      }
    }
    /// <summary>
    /// Reads archive data from the stream. In the current implementation
    /// stream must be seekable and readable to extract data.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="closeStream">Indicates whether method should close stream after reading.</param>
    public void Open( Stream stream, bool closeStream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      byte[] arrBuffer = new byte[ Constants.IntSize ];

      // Step1. Locate central directory end record.
      // We use 65557 because there can be comment at the end, maximum of 65535 bytes
      // + 2 bytes length + 20 bytes of End of central directory record
      long lCentralDirEndPosition = FindValueFromEnd( stream, Constants.CentralDirectoryEndSignature, 65557 );

      if( lCentralDirEndPosition < 0 )
        throw new ZipException( "Can't locate end of central directory record. Possible wrong file format or archive is corrupt." );

      // Step2. Locate central directory and iterate through all items
      stream.Position = lCentralDirEndPosition + Constants.CentralDirSizeOffset;
      int iCentralDirSize = ReadInt32( stream );

      long lCentralDirPosition = lCentralDirEndPosition - iCentralDirSize;

      // verify that this is really central directory
      stream.Position = lCentralDirPosition;

      //if( ReadInt32( stream ) != Constants.CentralHeaderSignature )
      //    throw new ZipException( "Can't locate central directory record. Possible wrong file format or archive is corrupt." );

      //stream.Position -= 4;
      ReadCentralDirectoryData( stream );
      ExtractItems( stream );
    }
    /// <summary>
    /// Clears all internal data.
    /// </summary>
    public void Close()
    {
      for( int i = 0, len = m_arrItems.Count ; i < len ; i++ )
      {
        ZipArchiveItem item = m_arrItems[ i ];
        item.Close();
      }

      m_arrItems.Clear();

      m_dicItems.Clear();
      m_dicItems = null;
    }
    /// <summary>
    /// Searches for the item with specified name.
    /// </summary>
    /// <param name="itemName">Item to find.</param>
    /// <returns>Zero-based item index if found; -1 otherwise.</returns>
    public int Find( string itemName )
    {
      // TODO: maybe this can (or should) be optimized.
      ZipArchiveItem item;
      int iResult = -1;

      if( m_dicItems.TryGetValue( itemName, out item ) )
      {
        for( int i = 0, len = m_arrItems.Count; i < len; i++ )
        {
          ZipArchiveItem currentItem = m_arrItems[ i ];

          if( currentItem == item )
          {
            iResult = i;
            break;
          }
        }
      }

      return iResult;
    }
    /// <summary>
    /// Searches for the item with specified name.
    /// </summary>
    /// <param name="itemRegex">Regular expression that defines item to find.</param>
    /// <returns>Zero-based item index if found; -1 otherwise.</returns>
    public int Find( Regex itemRegex )
    {
      int iResult = -1;

      for( int i = 0, len = m_arrItems.Count; i < len; i++ )
      {
        ZipArchiveItem currentItem = m_arrItems[ i ];
        string strItemName = currentItem.ItemName;

        if( itemRegex.IsMatch( strItemName ) )
        {
          iResult = i;
          break;
        }
      }

      return iResult;
    }
    /// <summary>
    /// Writes central directory to the stream.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
    private void WriteCentralDirectory( Stream stream )
    {
      long lStartPosition = stream.Position;

      for( int i = 0, len = m_arrItems.Count ; i < len ; i++ )
      {
        ZipArchiveItem item = m_arrItems[ i ];
        item.WriteFileHeader( stream );
      }

      WriteCentralDirectoryEnd( stream, lStartPosition );
    }
    /// <summary>
    /// Writes End of central directory record into stream.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
    /// <param name="directoryStart">Offset to the central directory start.</param>
    private void WriteCentralDirectoryEnd( Stream stream, long directoryStart )
    {
      if( stream == null )
      {
        throw new ArgumentNullException( "stream" );
      }

      int iDirectorySize = ( int )( stream.Position - directoryStart );
      stream.Write( BitConverter.GetBytes( Constants.CentralDirectoryEndSignature ),
        0, Constants.IntSize );

      // number of this disk.
      stream.WriteByte( 0 );
      stream.WriteByte( 0 );

      // number of the disk with the start of the central directory
      stream.WriteByte( 0 );
      stream.WriteByte( 0 );

      // total number of entries in the central directory on this disk
      byte[ ] arrData = BitConverter.GetBytes( ( short )m_arrItems.Count );
      stream.Write( arrData, 0, Constants.ShortSize );

      // total number of entries in the central directory.
      // We don't need to support different disks so we use the same data.
      stream.Write( arrData, 0, Constants.ShortSize );

      stream.Write( BitConverter.GetBytes( iDirectorySize ), 0, Constants.IntSize );

      // offset of start of central directory with respect to the starting disk number.
      stream.Write( BitConverter.GetBytes( ( int )directoryStart ), 0, Constants.IntSize );

      // zip comment length
      stream.WriteByte( 0 );
      stream.WriteByte( 0 );
    }
    /// <summary>
    /// Read central directory record from the stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    private void ReadCentralDirectoryData( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      while( ReadInt32( stream ) == Constants.CentralHeaderSignature )
      {
        ZipArchiveItem item = new ZipArchiveItem( this );
        item.ReadCentralDirectoryData( stream );
        m_arrItems.Add( item );
      }
    }
    /// <summary>
    /// Extracts items' data from the stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    private void ExtractItems( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException();

      if( !stream.CanSeek || !stream.CanRead )
        throw new ArgumentOutOfRangeException( "stream", "We need seekable and readable stream to parse items." );

      for( int i = 0, len = m_arrItems.Count;i < len;i++ )
      {
        ZipArchiveItem item = m_arrItems[ i ];
        item.ReadData( stream, m_bCheckCrc );
        m_dicItems.Add( item.ItemName, item );
      }
    }
    /// <summary>
    /// Creates a copy of the current instance.
    /// </summary>
    /// <returns>A copy of the current instance.</returns>
    public ZipArchive Clone()
    {
      ZipArchive result = ( ZipArchive )MemberwiseClone();
      result.m_arrItems = new List<ZipArchiveItem>();
      result.m_dicItems = new Dictionary<string,ZipArchiveItem>();

      for( int i = 0, len = m_arrItems.Count; i < len; i++ )
      {
        ZipArchiveItem item = ( ZipArchiveItem )m_arrItems[ i ];
        item = item.Clone();
        result.AddItem( item );
      }

      return result;
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// A method to release allocated unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if( m_arrItems != null )
      {
        for( int i = 0, len = m_arrItems.Count ; i < len ; i++ )
        {
          ZipArchiveItem item = ( ZipArchiveItem )m_arrItems[ i ];
          item.Dispose();
        }

        GC.SuppressFinalize( this );
      }
    }

    /// <summary>
    /// Class finilizer.
    /// </summary>
    ~ZipArchive()
    {
      if( m_arrItems != null )
      {
        Dispose();
      }
    }
    #endregion
  }
}
