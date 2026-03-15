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
#if ( WINRT )
using Windows.Storage;
using System.Threading.Tasks;
#endif
#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
  /// <summary>
  /// .Net compound file implementation.
  /// </summary>
  public class CompoundFile : ICompoundFile
  {
    #region Constants
    /// <summary>
    /// Name of the root entry.
    /// </summary>
    private const string RootEntryName = "Root Entry";
    #endregion

    #region Members
    /// <summary>
    /// Source stream.
    /// </summary>
    private Stream m_stream;
    /// <summary>
    /// File header.
    /// </summary>
    private FileHeader m_header;
    /// <summary>
    /// 
    /// </summary>
    private FAT m_fat;
    /// <summary>
    /// 
    /// </summary>
    private DIF m_dif;
    /// <summary>
    /// 
    /// </summary>
    private Directory m_directory;
    /// <summary>
    /// Root storage.
    /// </summary>
    private CompoundStorage m_root;
    /// <summary>
    /// Short stream.
    /// </summary>
    private Stream m_shortStream;
    /// <summary>
    /// Stream containing items described by minifat.
    /// </summary>
    private Stream m_miniFatStream;
    /// <summary>
    /// MiniFAT.
    /// </summary>
    private FAT m_miniFat;
    /// <summary>
    /// Indicates whether substreams should maintain their own stream or should write
    /// directly into the file's stream.
    /// </summary>
    private bool m_bDirectMode;
    #endregion

    #region Properties
    /// <summary>
    /// 
    /// </summary>
    internal FileHeader Header
    {
      get
      {
        return m_header;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public Directory Directory
    {
      get
      {
        return m_directory;
      }
    }
    /// <summary>
    /// Returns root storage.
    /// </summary>
    public ICompoundStorage Root
    {
      get
      {
        return m_root;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    internal DIF DIF
    {
      get
      {
        return m_dif;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    internal FAT Fat
    {
      get
      {
        return m_fat;
      }
    }
    /// <summary>
    /// Returns base stream. Read-only.
    /// </summary>
    internal Stream BaseStream
    {
      get
      {
        return m_stream;
      }
    }
    /// <summary>
    /// Gets or sets value indicating whether substreams should maintain their own stream
    /// or should write directly into the file's stream.
    /// </summary>
    internal bool DirectMode
    {
      get
      {
        return m_bDirectMode;
      }
      set
      {
        m_bDirectMode = value;
      }
    }
    #endregion

    #region Methods
#if !(WINRT )
    /// <summary>
    /// 
    /// </summary>
    public static void Main()
    {
      CompoundFile file = new CompoundFile();
      string path = "D:\\testfile\\";

      using( FileStream stream = new FileStream(
        //"D:\\VBADialog.xls",
        //"D:\\testchartsheet.xls",
        //"D:\\TestBig.xls",
        //@"D:\!temp\Excel notes\!tests\Output\CellsTests\Cells_BooleanError.xls",
        //@"D:\!temp\Excel notes\!tests\Output\OnTime\Issues10000\OnTimeIssue_10747_Save.xls",
        //@"d:\test.xls",
        //@"d:\CtoF.xls",
        //"data.bin",
        "test.bin",
        FileMode.Open, FileAccess.Read, FileShare.Read ) )
      {
        file.Open( stream );
        ICompoundStorage storage = file.Root;

        WriteDirectory( path, file.Directory );
        WriteStorage( path, storage );
        //CompoundStream workbookStream = storage.OpenStream( "Workbook" );
        //Console.WriteLine( "Workbook length is:", workbookStream.Length );
        //header = new FileHeader( stream );
      }

      //MemoryStream memStream = new MemoryStream();
      //header.Serialize( memStream );
    }
    /// <summary>
    /// Writes directory structure into file.
    /// </summary>
    /// <param name="path">Destination path.</param>
    /// <param name="directory">Directory to write.</param>
    private static void WriteDirectory( string path, Directory directory )
    {
      string fileName = Path.Combine( path, "directory.txt" );

      using( StreamWriter writer = new StreamWriter( fileName ) )//FileStream stream = new FileStream( fileName, FileMode.Create, FileAccess.Write, FileShare.None ) )
      {
        List<DirectoryEntry> lstEntries = directory.Entries;

        for( int i = 0, len = lstEntries.Count; i < len; i++ )
        {
          DirectoryEntry entry = lstEntries[ i ];
          writer.WriteLine( new string( '-', 20 ) );
          writer.WriteLine( "EntryId: {0}", entry.EntryId );
          writer.WriteLine( "Name: {0}, EntryType: {1}", entry.Name, entry.Type );
          writer.WriteLine( "Left: {0}, Right: {1}, Child: {2}", entry.LeftId, entry.RightId, entry.ChildId );
          writer.WriteLine( "Guid: {0}, DateCreate: {1}, DateModify: {2}", entry.StorageGuid, entry.DateCreate, entry.DateModify );
          writer.WriteLine( "StartSector: {0}, Size: {1}", entry.StartSector, entry.Size );
          //byte Color
          //int StorageFlags
          //int Reserved
        }
      }
    }
    /// <summary>
    /// Writes storage to specified path
    /// </summary>
    /// <param name="path">Destination path.</param>
    /// <param name="storage">Storage to write.</param>
    private static void WriteStorage( string path, ICompoundStorage storage )
    {
      foreach( string streamName in storage.Streams )
      {
        WriteStream( path, streamName, storage );
      }

      foreach( string storageName in storage.Storages )
      {
        string newPath = Path.Combine( path, storageName );
        System.IO.Directory.CreateDirectory( newPath );

        using( ICompoundStorage newStorage = storage.OpenStorage( storageName ) )
        {
          WriteStorage( newPath, newStorage );
        }
      }
    }
    /// <summary>
    /// Writes stream into file
    /// </summary>
    /// <param name="path">Destination path.</param>
    /// <param name="streamName">Stream name.</param>
    /// <param name="storage">Parent storage object.</param>
    private static void WriteStream( string path, string streamName, ICompoundStorage storage )
    {
      const int BufferSize = 32768;
      byte[] arrBuffer = new byte[ BufferSize ];

      using( Stream stream = storage.OpenStream( streamName ) )
      {
        if( ( int )streamName[ 0 ] < 32 )
        {
          streamName = streamName.Substring( 1 );
        }

        string streamPath = Path.Combine( path, streamName );
        using( FileStream fileStream = new FileStream( streamPath, FileMode.Create,
          FileAccess.Write, FileShare.None ) )
        {
          int iReadCount;
          while( ( iReadCount = stream.Read( arrBuffer, 0, BufferSize ) ) > 0 )
          {
            fileStream.Write( arrBuffer, 0, iReadCount );
          }
        }
      }
    }
#endif
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CompoundFile()
    {
      //m_header = new FileHeader();
      //m_fat = new FAT();
      //m_directory = new Directory();

      // We should write some simple structure in the stream, maybe allocate some data, etc.
      m_stream = new MemoryStream();
      InitializeVariables();
    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CompoundFile( Stream stream )
    {
      Open( stream );
    }
#if ( WINRT )
        public CompoundFile(string fileName, bool create)
        {
//           await Initialize(fileName,create);
        }
        public async Task Initialize(string fileName, bool create)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(fileName);
            //Open( stream );
            if (!create)
            {
                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    Open(stream);
                }
            }
            else
            {
# if DOCIO || XLSIO
                m_stream = new MemoryStream();
#else
          m_stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);         
# endif
                InitializeVariables();
            }
        }
#else
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CompoundFile( string fileName, bool create )
    {
      //Open( stream );
      if( !create )
      {
        using( FileStream stream = new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read ) )
        {
          Open( stream );
        }
      }
      else
      {
# if DOCIO
          m_stream = new MemoryStream();
#else
          m_stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);         
# endif
          InitializeVariables();
      }
    }
#endif
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    public void Open( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );
      stream.Position = 0;
      long lPosition = stream.Position;
      long lLength = stream.Length;
      int iReadCount = ( int )( lLength - lPosition );
      MemoryStream memStream = new MemoryStream( iReadCount );

      memStream.SetLength( iReadCount );
#if !(WINRT )
      stream.Read( memStream.GetBuffer(), 0, iReadCount );
#else
            byte[] buffer = new byte[iReadCount];
            stream.Read(buffer, 0, buffer.Length);
            memStream.Write(buffer, 0, buffer.Length);
#endif
      memStream.Position = 0;

      m_stream = memStream;

      // Step 1. Extract and check header.
      m_header = new FileHeader( m_stream );

      // Step 2. Extract DIF (sectors that are used by FAT).
      m_dif = new DIF( m_stream, m_header );

      // Step 3. Extract FAT.
      m_fat = new FAT( this, m_stream, m_dif, m_header );

      // Step 4. Extract directory entries.
      byte[] arrDirectoryData = m_fat.GetStream( m_stream, m_header.DirectorySectorStart, this );
      m_directory = new Directory( arrDirectoryData );

      //Console.WriteLine( arrDirectoryData.Length );
      // Let's write directory structure.
      //BuildTree( m_directory, m_directory.Entries[ 0 ], 0 );
      DirectoryEntry rootEntry = m_directory.Entries[ 0 ];
      m_root = new CompoundStorage( this, rootEntry );

      int iShortStreamStart = rootEntry.StartSector;

      if( iShortStreamStart >= 0 )
      {
        // TODO: create mini-fat (mini-sat) objects. using FAT class = stream, sector size, fat chain.
        m_shortStream = new MemoryStream( m_fat.GetStream( m_stream, iShortStreamStart, this ) );
        m_miniFatStream = new MemoryStream( m_fat.GetStream( m_stream, m_header.MiniFastStart, this ) );
        m_miniFat = new FAT( m_shortStream, m_header.MiniSectorShift, m_miniFatStream, 0 );
        // On the current moment we don't support mini streams
        m_fat.CloseChain( iShortStreamStart );
        m_fat.CloseChain( m_header.MiniFastStart );
      }
    }
    /// <summary>
    /// Initializes internal variables.
    /// </summary>
    private void InitializeVariables()
    {
      m_directory = new Directory();
      m_root = new CompoundStorage( this, RootEntryName, 0 );

      m_stream.SetLength( FileHeader.HeaderSize );
      m_header = new FileHeader();
      m_dif = new DIF();

      DirectoryEntry rootEntry = m_root.Entry;
      rootEntry.Type = DirectoryEntry.EntryType.Root;
      m_directory.Add( rootEntry );

      m_fat = new FAT( m_stream, m_header.SectorShift, FileHeader.HeaderSize );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="sectorIndex"></param>
    /// <param name="header"></param>
    internal void ReadSector( byte[] buffer, int offset, int sectorIndex, FileHeader header )
    {
      ushort iSectorShift = header.SectorShift;
      int iSectorSize = header.SectorSize;
      long lOffset = GetSectorOffset( sectorIndex, iSectorShift );
      m_stream.Position = lOffset;
      m_stream.Read( buffer, offset, iSectorSize );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    internal Stream GetEntryStream( DirectoryEntry entry )
    {
      if( entry == null )
        throw new ArgumentNullException( "entry" );

      // NOTE: in the current implementation we simply extract complete stream data into MemoryStream
      Stream result = null;

      if( entry.Type == DirectoryEntry.EntryType.Stream )
      {
        //result = new MemoryStream( ( int )entry.Size );
        Stream stream;
        FAT fat;
        //int headerSize;

        if( m_miniFat != null && entry.Size < m_header.MiniSectorCutoff )
        {
          fat = m_miniFat;
          stream = m_shortStream;
          //headerSize = 0;
        }
        else
        {
          fat = m_fat;
          stream = m_stream;
          //headerSize = FileHeader.HeaderSize;
        }

        byte[] arrStreamData = fat.GetStream( stream, entry.StartSector, this );
        result = ( arrStreamData != null ) ?
          new MemoryStream( arrStreamData ) :
  new MemoryStream();

        result.SetLength( entry.Size );
      }

      return result;
    }
    /// <summary>
    /// Sets stream data for directory entry.
    /// </summary>
    /// <param name="entry">Directory entry to update stream data for.</param>
    /// <param name="stream">Stream to set.</param>
    internal void SetEntryStream( DirectoryEntry entry, Stream stream )
    {
      if( entry == null )
        throw new ArgumentNullException( "entry" );

      // Correct algorithm
      // 1. Storage remain the same
      // a. Was in short stream - left in short stream
      // b. Was in
      // 2. Storage changed
      // a. short -> normal stream
      // b. normmal stream -> short one.

      // Shortened (since we don't edit objects - just add new).

      if( stream.Length >= m_header.MiniSectorCutoff )
      {
        SetEntryLongStream( entry, stream );
      }
      else
      {
        SetEntryShortStream( entry, stream );
      }

      entry.Size = ( uint )stream.Length;
    }
    /// <summary>
    /// Sets entrie's long stream.
    /// </summary>
    /// <param name="entry">Entry to update data for.</param>
    /// <param name="stream">Data to set.</param>
    private void SetEntryLongStream( DirectoryEntry entry, Stream stream )
    {
      // NOTE: this code is used for ordinary streams, not mini-streams.
      // 1. Sector Size
      // 2. Already used sectors (maximum possible size to store using available sectors).
      // 3. How many sectors are freed
      // 4. How many sectors will be additionally used
      int iSectoryShift = m_header.SectorShift;
      int iSectorySize = m_header.SectorSize;
      long lCurrentSize = entry.Size;

      long lStreamSize = stream.Length;

      int iAllocatedSectors = ( int )Math.Ceiling( lCurrentSize / ( double )iSectorySize );
      int iRequiredSectors = ( int )Math.Ceiling( lStreamSize / ( double )iSectorySize );

      AllocateSectors( entry, iAllocatedSectors, iRequiredSectors, m_fat );
      WriteData( m_stream, entry.StartSector, stream, m_fat );
    }
    /// <summary>
    /// Sets entrie's short stream.
    /// </summary>
    /// <param name="entry">Entry to update data for.</param>
    /// <param name="stream">Data to set.</param>
    private void SetEntryShortStream( DirectoryEntry entry, Stream stream )
    {
      if( m_shortStream == null )
        m_shortStream = new MemoryStream();

      if( m_miniFat == null )
        m_miniFat = new FAT( m_shortStream, m_header.MiniSectorShift, 0 );

      // NOTE: this code is used for mini-streams only.
      // 1. Sector Size
      // 2. Already used sectors (maximum possible size to store using available sectors).
      // 3. How many sectors are freed
      // 4. How many sectors will be additionally used
      int iSectoryShift = m_header.MiniSectorShift;
      int iSectorySize = m_miniFat.SectorSize;
      long lCurrentSize = entry.Size;

      long lStreamSize = stream.Length;
      int iAllocatedSectors = ( int )Math.Ceiling( lCurrentSize / ( double )iSectorySize );
      int iRequiredSectors = ( int )Math.Ceiling( lStreamSize / ( double )iSectorySize );

      AllocateSectors( entry, iAllocatedSectors, iRequiredSectors, m_miniFat );
      WriteData( m_shortStream, entry.StartSector, stream, m_miniFat );
    }
    /// <summary>
    /// Writes stream data into compound file main stream
    /// </summary>
    /// <param name="destination">Main stream to write into.</param>
    /// <param name="startSector">Start sector to write.</param>
    /// <param name="stream">Stream to write.</param>
    /// <param name="fat">Fat object.</param>
    private void WriteData( Stream destination, int startSector, Stream stream, FAT fat )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      //int iStartSector = entry.StartSector;
      //ushort usSectorShift = m_header.SectorShift;
      long lOffset = fat.GetSectorOffset( startSector );
      int iSectorSize = fat.SectorSize;
      byte[] arrBuffer = new byte[ iSectorSize ];
      long lOldPosition = stream.Position;
      stream.Position = 0;
      int iReadCount;

      while( ( iReadCount = stream.Read( arrBuffer, 0, iSectorSize ) ) > 0 )
      {
        // write data.
        destination.Position = lOffset;
        destination.Write( arrBuffer, 0, iReadCount );
        // Move to the next sector
        startSector = fat.NextSector( startSector );

        if( startSector < 0 )
          break;
        //throw new ArgumentOutOfRangeException( "start sector" );

        lOffset = fat.GetSectorOffset( startSector );
      }

      stream.Position = lOldPosition;
    }
    /// <summary>
    /// Here we have to allocate required sectors number.
    /// </summary>
    /// <param name="entry">Entry to allocate sectors for.</param>
    /// <param name="iAllocatedSectors">Number of already allocated sectors.</param>
    /// <param name="iRequiredSectors">Number of required sectors.</param>
    /// <param name="fat">FAT object.</param>
    private void AllocateSectors( DirectoryEntry entry, int iAllocatedSectors, int iRequiredSectors, FAT fat )
    {
      // This code is not ready yet.
      if( iAllocatedSectors == iRequiredSectors )
        return;

      int iSector = ( entry.LastSector >= 0 ) ? entry.LastSector : entry.StartSector;
      int iStartSector = AllocateSectors( iSector, iAllocatedSectors, iRequiredSectors, fat );

      if( iSector < 0 )
        entry.StartSector = iStartSector;
    }
    /// <summary>
    /// Allocates sectors.
    /// </summary>
    /// <param name="iSector">Start sector in the chain.</param>
    /// <param name="iAllocatedSectors">Number of already allocated sectors.</param>
    /// <param name="iRequiredSectors">Number of required sectors.</param>
    /// <param name="fat">Fat object.</param>
    /// <returns>Start sector of the added chain.</returns>
    private int AllocateSectors( int iSector, int iAllocatedSectors, int iRequiredSectors, FAT fat )
    {
      int iResult = -1;

      if( iAllocatedSectors == iRequiredSectors )
      {
        iResult = iSector;
      }
      else if( iAllocatedSectors < iRequiredSectors )
      {
        // We need additional sectors
        // 1. Find last sector
        int iNextSector = ( iSector >= 0 ) ? fat.NextSector( iSector ) : iSector;

        //for( int i = 0; i < iAllocatedSectors - 1; i++ )
        while( iNextSector >= 0 )
        {
          iSector = iNextSector;
          iNextSector = fat.NextSector( iSector );
        }

        // 2. Add additional sectors
        int iStartSector = fat.EnlargeChain( iSector, iRequiredSectors - iAllocatedSectors );

        if( iSector < 0 )
          iResult = iStartSector;
      }
      else
      {
        // We have to free some sectors.
        // 1. Skip required number of sectors
        for( int i = 0; i < iRequiredSectors - 1; i++ )
        {
          iSector = fat.NextSector( iSector );
        }

        // 2. Mark not required sectors as free in FAT
        fat.CloseChain( iSector );
      }

      return iResult;
    }
    /// <summary>
    /// Gets offset to the sector.
    /// </summary>
    /// <param name="sectorIndex">Zero-based sector index.</param>
    /// <param name="sectorShift">Sector shift (2^sectorShift = sector size).</param>
    /// <returns>Offset to the required sector.</returns>
    [CLSCompliant( false )]
    public static long GetSectorOffset( int sectorIndex, ushort sectorShift )
    {
      return ( sectorIndex << sectorShift ) + FileHeader.HeaderSize;
    }
    /// <summary>
    /// Gets offset to the sector.
    /// </summary>
    /// <param name="sectorIndex">Zero-based sector index.</param>
    /// <param name="sectorShift">Sector shift (2^sectorShift = sector size).</param>
    /// <param name="headerSize">Size of the header.</param>
    /// <returns>Offset to the required sector.</returns>
    [CLSCompliant( false )]
    public static long GetSectorOffset( int sectorIndex, ushort sectorShift, int headerSize )
    {
      return ( sectorIndex << sectorShift ) + headerSize;
    }
    /// <summary>
    /// Checks whether stream header belongs to compound file.
    /// </summary>
    /// <param name="stream">Stream to check.</param>
    /// <returns>True if stream probably contains compound file data.</returns>
    public static bool CheckHeader( Stream stream )
    {
      return FileHeader.CheckSignature( stream );
    }
    /// <summary>
    /// Allocates new directory entry.
    /// </summary>
    /// <param name="streamName">Name of the stream.</param>
    /// <param name="entryType">Entry type.</param>
    /// <returns>Created directory entry.</returns>
    internal DirectoryEntry AllocateDirectoryEntry( string streamName, DirectoryEntry.EntryType entryType )
    {
      // TODO: 1. Try to find empty one.
      DirectoryEntry entry = new DirectoryEntry( streamName, entryType, m_directory.Entries.Count );
      m_directory.Add( entry );
      entry.DateModify = entry.DateCreate = DateTime.Now;
      return entry;
    }
    /// <summary>
    /// Marks item as free.
    /// </summary>
    /// <param name="directoryEntry">Directory entry to be removed/freed.</param>
    internal void RemoveItem( DirectoryEntry directoryEntry )
    {
      if( directoryEntry == null )
        throw new ArgumentNullException( "directoryEntry" );

      // 1. Mark entry as empty
      directoryEntry.Type = DirectoryEntry.EntryType.Invalid;

      // 2. Mark entry sectors of the stream as free.
      if( directoryEntry.Type == DirectoryEntry.EntryType.Stream )
      {
        m_fat.CloseChain( directoryEntry.StartSector );
        directoryEntry.StartSector = -1;
      }
    }
    /// <summary>
    /// Reads data from internal stream.
    /// </summary>
    /// <param name="entry">Entry to read data from.</param>
    /// <param name="position">Position inside entry stream.</param>
    /// <param name="buffer">Buffer that will cotain read data.</param>
    /// <param name="length">Size of the data to read.</param>
    /// <returns>Number of actually read bytes.</returns>
    internal int ReadData( DirectoryEntry entry, long position, byte[] buffer, int length )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Writes data into internal stream.
    /// </summary>
    /// <param name="entry">Entry to write data into.</param>
    /// <param name="position">Position inside entry stream.</param>
    /// <param name="buffer">Buffer containing data to write.</param>
    /// <param name="offset">Offset inside buffer to the data to write.</param>
    /// <param name="length">Size of the data to write.</param>
    internal void WriteData( DirectoryEntry entry, long position, byte[] buffer, int offset, int length )
    {
      int iSectoryShift = m_header.SectorShift;
      int iSectorySize = m_header.SectorSize;
      long lCurrentSize = entry.Size;

      long lStreamSize = position + length;

      int iAllocatedSectors = ( int )Math.Ceiling( lCurrentSize / ( double )iSectorySize );
      int iRequiredSectors = ( int )Math.Ceiling( lStreamSize / ( double )iSectorySize );

      if( iRequiredSectors > iAllocatedSectors )
        AllocateSectors( entry, iAllocatedSectors, iRequiredSectors, m_fat );

      // 1. Locate last sector
      int iCurrentSector = entry.StartSector;
      int iSectorSize = m_fat.SectorSize;
      int iCurrentOffset = iSectorSize;

      GetOffsets( entry, position, ref iCurrentOffset, ref iCurrentSector );

      entry.LastSector = iCurrentSector;
      entry.LastOffset = iCurrentOffset;
      int iOffsetInsideSector = ( int )( position % iSectorSize );

      while( length > 0 )
      {
        int iMaxSize = Math.Min( length, iSectorSize - iOffsetInsideSector );
        long lPosition = m_fat.GetSectorOffset( iCurrentSector ) + iOffsetInsideSector;

#if DEBUG
        if( lPosition < 0 )
          System.Diagnostics.Debugger.Break();
#endif

        m_stream.Position = lPosition;
        m_stream.Write( buffer, offset, iMaxSize );

        iOffsetInsideSector = 0;
        offset += iMaxSize;
        length -= iMaxSize;
        iCurrentSector = m_fat.NextSector( iCurrentSector );
      }

      entry.Size = ( uint )Math.Max( entry.Size, lStreamSize );
      // 2. Append data.
      //WriteData( m_stream, entry.StartSector, stream, m_fat );
    }
    private void GetOffsets( DirectoryEntry entry, long position, ref int iCurrentOffset, ref int iCurrentSector )
    {
      int iSectorSize = m_fat.SectorSize;

      if( entry.LastSector >= 0 )
      {
        int iAfterSectorStart = ( int )position % iSectorSize;
        long lSectorMargin = ( iAfterSectorStart > 0 ) ? position + iSectorSize - iAfterSectorStart : position;

        if( entry.LastOffset <= lSectorMargin )
        {
          iCurrentOffset = entry.LastOffset;
          iCurrentSector = entry.LastSector;
        }
        else
        {
          System.Diagnostics.Debugger.Break();
        }
      }

      while( iCurrentOffset <= position )
      {
        iCurrentSector = m_fat.NextSector( iCurrentSector );
        iCurrentOffset += iSectorSize;
      }

#if DEBUG
      if( iCurrentSector < 0 )
        System.Diagnostics.Debugger.Break();
#endif
    }
    #endregion

    #region ICompoundFile Members
    /// <summary>
    /// Returns root storage object for this file.
    /// </summary>
    public ICompoundStorage RootStorage
    {
      get
      {
        return m_root;
      }
    }
    public void Flush()
    {
      // We have to write those items from the end because directory can modify fat (allocate new entries
      // fat -> dif, and dif -> header.
      m_root.Flush();
      SaveMiniStream();
      SerializeDirectory();
      m_fat.Write( m_stream, m_dif, m_header );
      m_dif.Write( m_stream, m_header );
      m_header.Write( m_stream );
      m_stream.Position = 0;
    }
    /// <summary>
    /// Saves compound file into stream.
    /// </summary>
    /// <param name="stream">Stream to save data into.</param>
    public void Save( Stream stream )
    {
      Flush();
      WriteStreamTo( stream );

      //byte[] arrBuffer = new byte[ 512 ];
      //stream.Position = 0x1c00200;
      //stream.Read( arrBuffer, 0, 512 );
      //int iFirstValue = BitConverter.ToInt32( arrBuffer, 0 );
      //System.Diagnostics.Debug.WriteLine( iFirstValue );
    }
    /// <summary>
    /// Writes internal stream into specified one.
    /// </summary>
    /// <param name="destination">Destination stream to write into.</param>
    private void WriteStreamTo( Stream destination )
    {
      MemoryStream memStream = m_stream as MemoryStream;

      if( memStream != null )
      {
        memStream.WriteTo( destination );
      }
      else
      {
        const int BufferSize = 32768;
        byte[] arrBuffer = new byte[ BufferSize ];
        int iReadCount;

        while( ( iReadCount = m_stream.Read( arrBuffer, 0, BufferSize ) ) > 0 )
        {
          destination.Write( arrBuffer, 0, iReadCount );
        }
      }
    }
    /// <summary>
    /// Saves mini stream data.
    /// </summary>
    private void SaveMiniStream()
    {
      // fat + stream
      //throw new Exception( "The method or operation is not implemented." );

      if( m_shortStream == null || m_shortStream.Length == 0 )
        return;

      int iSectorsCount = ( int )Math.Ceiling( m_shortStream.Length / ( double )m_header.SectorSize );

      DirectoryEntry rootEntry = m_directory.Entries[ 0 ];
      int iStartSector = rootEntry.StartSector;
      int iAllocatedSectors = ( int )Math.Ceiling( rootEntry.Size / ( double )m_fat.SectorSize );
      iStartSector = AllocateSectors( iStartSector, iAllocatedSectors, iSectorsCount, m_fat );
      WriteData( m_stream, iStartSector, m_shortStream, m_fat );
      DirectoryEntry entry = m_directory.Entries[ 0 ];
      entry.StartSector = iStartSector;
      entry.Size = ( uint )m_shortStream.Length;

      MemoryStream miniFat = new MemoryStream();
      m_miniFat.WriteSimple( miniFat, m_header.SectorSize );

      iSectorsCount = ( int )Math.Ceiling( miniFat.Length / ( double )m_header.SectorSize );
      iStartSector = AllocateSectors( m_header.MiniFastStart, m_header.MiniFatNumber, iSectorsCount, m_fat );
      WriteData( m_stream, iStartSector, miniFat, m_fat );
      m_header.MiniFastStart = iStartSector;
      int iMiniFatSectorSize = 1 << m_header.MiniSectorShift;
      m_header.MiniFatNumber = iSectorsCount;//( int )Math.Ceiling( m_shortStream.Length / ( double )iMiniFatSectorSize );
    }
    /// <summary>
    /// Serializes directory entries.
    /// </summary>
    private void SerializeDirectory()
    {
      MemoryStream directory = new MemoryStream();
      m_directory.Write( directory );

      //int iDirectoryStart = m_header.DirectorySectorStart;
      int iSectorsRequired = ( int )Math.Ceiling( directory.Length / ( double )m_header.SectorSize );

      // TODO: what if sectors are allocated?
      int iDirectoryStart = m_header.DirectorySectorStart;

      int iDirectorySize = ( iDirectoryStart >= 0 ) ?
        m_fat.GetChainLength( iDirectoryStart ) :
        0;

      iDirectoryStart = m_header.DirectorySectorStart =
        AllocateSectors( iDirectoryStart, iDirectorySize, iSectorsRequired, m_fat );
      WriteData( m_stream, iDirectoryStart, directory, m_fat );
    }
#if !(WINRT )
    /// <summary>
    /// Saves compound file into file.
    /// </summary>
    /// <param name="fileName">Name of the file to save into.</param>
    public void Save( string fileName )
    {
      using( FileStream stream = new FileStream( fileName, FileMode.Create, FileAccess.Write, FileShare.None ) )
      {
        Save( stream );
      }
    }
#endif
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Performs application-defined tasks associated with freeing,
    /// releasing, or resetting unmanaged resources. 
    /// </summary>
    public void Dispose()
    {
      if( m_root != null )
      {
        m_root.Dispose();
        m_root = null;

        m_stream.Dispose();
        m_stream = null;

        m_header = null;
        m_fat = null;
        m_directory = null;
      }
    }

    #endregion
  }
}
