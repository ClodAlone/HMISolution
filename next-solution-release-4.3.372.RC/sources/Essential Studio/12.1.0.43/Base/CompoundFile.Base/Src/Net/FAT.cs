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
  /// This class represents FAT object in the compound file.
  /// </summary>
  class FAT
  {
    #region Members
    ///// <summary>
    ///// 
    ///// </summary>
    //private CompoundFile m_parentFile;
    /// <summary>
    /// 
    /// </summary>
    private List<int> m_lstFatChains = new List<int>();
    /// <summary>
    /// List with free sectors.
    /// </summary>
    private List<int> m_freeSectors = new List<int>();
    /// <summary>
    /// Sector size.
    /// </summary>
    private ushort m_usSectorShift;
    /// <summary>
    /// 
    /// </summary>
    private Stream m_stream;
    private int m_iHeaderSize;
    #endregion

    #region Properties
    /// <summary>
    /// Sector size.
    /// </summary>
    public int SectorSize
    {
      get
      {
        return 1 << m_usSectorShift;
      }
    }
    ///// <summary>
    ///// Sector size.
    ///// </summary>
    //public ushort SectorShift
    //{
    //  get
    //  {
    //    return m_usSectorShift;
    //  }
    //}
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public FAT( Stream parentStream, ushort sectorShift, int headerSize )
    {
      m_stream = parentStream;
      //m_lstFatChains.Add( SectorTypes.FatSector );
      //m_iSectorSize = sectorSize;
      m_usSectorShift = sectorShift;
      m_iHeaderSize = headerSize;
    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    public FAT( Stream parentStream, ushort sectorShift, Stream fatStreamToParse, int headerSize )
    {
      m_usSectorShift = sectorShift;
      m_iHeaderSize = headerSize;
      m_stream = parentStream;

      fatStreamToParse.Position = 0;
      byte[] buffer = new byte[ FileHeader.IntSize ];

      while( fatStreamToParse.Read( buffer, 0, FileHeader.IntSize ) > 0 )
      {
        m_lstFatChains.Add( BitConverter.ToInt32( buffer, 0 ) );
      }
    }
    /// <summary>
    /// Initializes new instance of the fat.
    /// </summary>
    /// <param name="file">Parent compound file object.</param>
    /// <param name="stream">Stream to extract fat from.</param>
    /// <param name="dif">DIF object to help in parsing</param>
    /// <param name="header">File header object.</param>
    public FAT( CompoundFile file, Stream stream, DIF dif, FileHeader header )
    {
      if( file == null )
        throw new ArgumentNullException( "file" );

      if( stream == null )
        throw new ArgumentNullException( "stream" );

      m_stream = file.BaseStream;

      List<int> lstFatSectors = dif.SectorIds;
      //FileHeader header = file.Header;
      int iSectorSize = header.SectorSize;
      m_usSectorShift = header.SectorShift;
      byte[] arrBuffer = new byte[ iSectorSize ];
      int[] arrIntBuffer = new int[ iSectorSize >> 2 ];
      m_iHeaderSize = FileHeader.HeaderSize;

      for( int i = 0, len = lstFatSectors.Count; i < len; i++ )
      {
        int iSectorIndex = lstFatSectors[ i ];

        if( iSectorIndex >= 0 )
        {
          file.ReadSector( arrBuffer, 0, iSectorIndex, header );
          Buffer.BlockCopy( arrBuffer, 0, arrIntBuffer, 0, iSectorSize );
          m_lstFatChains.AddRange( arrIntBuffer );
        }
      }
    }
    /// <summary>
    /// Gets data of the compound file substream.
    /// </summary>
    /// <param name="stream">Stream with compound file data.</param>
    /// <param name="firstSector">First sector of the stream to get.</param>
    /// <param name="file">Parent compound file object.</param>
    /// <returns></returns>
    public byte[] GetStream( Stream stream, int firstSector, CompoundFile file )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( file == null )
        throw new ArgumentNullException( "file" );

      // Is this a new stream or it already exists?
      if( firstSector < 0 )
        return null;

      List<int> lstChain = new List<int>();
      FileHeader header = file.Header;
      //lstChain.Add( firstSector );
      int iNextSector = firstSector;

      while( iNextSector != SectorTypes.EndOfChain )
      {
          if (iNextSector < 0 || iNextSector >= m_lstFatChains.Count)
#if ( WINRT )
        //System.Diagnostics.Debug.WriteLine( iNextSector );
        
          throw new Exception();
#else
          throw new ApplicationException();
#endif
          lstChain.Add( iNextSector );
        iNextSector = m_lstFatChains[ iNextSector ];
      }

      //ushort iSectorShift = header.SectorShift;
      //int iSectorSize = header.SectorSize;
      int iCount = lstChain.Count;
      byte[] arrResult = new byte[ iCount << m_usSectorShift ];
      int iSectorSize = 1 << m_usSectorShift;

      for( int i = 0, iOffset = 0; i < iCount; i++, iOffset += iSectorSize )
      {
        long lSectorOffset = GetSectorOffset( lstChain[ i ] );
        stream.Position = lSectorOffset;
        stream.Read( arrResult, iOffset, iSectorSize );
        //file.ReadSector( arrResult, iOffset, lstChain[ i ], header );
      }

      return arrResult;
    }
    /// <summary>
    /// Gets index of the next sector in the chain.
    /// </summary>
    /// <param name="sectorIndex">Index of the current sector in the chain.</param>
    /// <returns>Next sector in the chain.</returns>
    internal int NextSector( int sectorIndex )
    {
#if DEBUG
      if( sectorIndex < 0 || sectorIndex > m_lstFatChains.Count )
        throw new ArgumentOutOfRangeException( "sectorIndex" );
#endif

       return m_lstFatChains[ sectorIndex ];
    }
    /// <summary>
    /// Closes sectors chain by marking all those sectors as free starting from specified one.
    /// </summary>
    /// <param name="iSector"></param>
    internal void CloseChain( int iSector )
    {
      int iNextSector = m_lstFatChains[ iSector ];
      m_lstFatChains[ iSector ] = SectorTypes.EndOfChain;

      while( iNextSector != SectorTypes.EndOfChain )
      {
        iSector = iNextSector;
        iNextSector = m_lstFatChains[ iSector ];
        m_lstFatChains[ iSector ] = SectorTypes.FreeSector;
        m_freeSectors.Add( iSector );
      }

      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Enlarges existing sectors chain.
    /// </summary>
    /// <param name="sector">Last sector in the chain that requires enlargment.</param>
    /// <param name="sectorCount">Number of sectors to add.</param>
    /// <returns>Index of the chain start (used when chain wasn't created before).</returns>
    internal int EnlargeChain( int sector, int sectorCount )
    {
      if( sectorCount <= 0 )
        return sector;

      int iFreeSectorsCount = m_freeSectors.Count;
      // Number of free sectors that we can use.
      int iFreeSectorsToUse = Math.Min( sectorCount, iFreeSectorsCount );
      // Number of not allocated sectors that should be allocated.
      int iNewSectors = sectorCount - iFreeSectorsToUse;

      int iStartSector = AllocateFreeSectors( ref sector, iFreeSectorsToUse );
      iStartSector = AllocateNewSectors( ref sector, iNewSectors );

      // We should close the chain after all operations.
      m_lstFatChains[ sector ] = SectorTypes.EndOfChain;
      return iStartSector;
    }
    /// <summary>
    /// Frees specified sector.
    /// </summary>
    /// <param name="sector">Sector to free.</param>
    internal void FreeSector( int sector )
    {
      int iCount = m_lstFatChains.Count;

      if( sector < 0 || sector >= iCount )
        throw new ArgumentOutOfRangeException( "sector" );

      if( sector != iCount - 1 )
      {
        // Mark sector as free
        m_lstFatChains[ sector ] = SectorTypes.FreeSector;
        m_freeSectors.Add( sector );
      }
      else
      {
        // TODO: there is probability that we can remove more than one sector, maybe we should do it on Save.
        m_lstFatChains.RemoveAt( sector );
        FreeLastSector();
      }
    }

    private void FreeLastSector()
    {
      long lNewLength = Math.Max( 0, m_stream.Length - SectorSize );
      m_stream.SetLength( lNewLength );
      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Allocates required number of new sectors.
    /// </summary>
    /// <param name="sector">Start sector index.</param>
    /// <param name="count">Number of sectors to allocate.</param>
    /// <returns>First sector in the new part of the chain.</returns>
    private int AllocateNewSectors( ref int sector, int count )
    {
      int result = sector;
      int prevSector = sector;

      sector = AddSectors( count );
      
      if( result < 0 )
        result = sector;

      // Allocate new ones.
      for( int i = 0; i < count; i++, sector++ )
      {
#if DEBUG
        if( sector != m_lstFatChains.Count )
          throw new ArgumentOutOfRangeException();
#endif
        m_lstFatChains.Add( sector + 1 );

        if( prevSector >= 0 )
          m_lstFatChains[ prevSector ] = sector;

        prevSector = sector;
      }

      //m_lstFatChains.Add( SectorTypes.EndOfChain );
      sector--;
      m_lstFatChains[ sector ] = SectorTypes.EndOfChain;
      return result;
    }

    private int AddSectors( int count )
    {
      long lCurrentLength = m_stream.Length;
      //int iSectorSize = SectorSize;
      m_stream.SetLength( lCurrentLength + ( count << m_usSectorShift ) );
      return ( int )( ( lCurrentLength - m_iHeaderSize ) >> m_usSectorShift );
    }
    /// <summary>
    /// Allocates required number of free sectors.
    /// </summary>
    /// <param name="sector">Start sector index (this value points to the last used sector after this operation).</param>
    /// <param name="count">Number of sectors to allocate.</param>
    /// <returns>First sector in the new part of the chain</returns>
    private int AllocateFreeSectors( ref int sector, int count )
    {
      int result = sector;

      for( int i = 0; i < count; i++ )
      {
        int iNextSector = m_freeSectors[ i ];

        if( sector >= 0 )
        {
          m_lstFatChains[ sector ] = iNextSector;
        }
        else
        {
          result = iNextSector;
        }

        sector = iNextSector;
      }

      // These sectors are no longer free
      m_freeSectors.RemoveRange( 0, count );
      return result;
    }
    /// <summary>
    /// Saves fat data into stream.
    /// </summary>
    /// <param name="stream">Stream to write fat data into.</param>
    /// <param name="dif">DIF object to update after writing.</param>
    /// <param name="header">File header.</param>
    public void Write( Stream stream, DIF dif, FileHeader header )
    {
      int iSectorCount = m_lstFatChains.Count;
      int sectorSize = header.SectorSize;
      ushort sectorShift = header.SectorShift;

      // Number of required sectors for fat = number of elements in dif. -2 - because each sector takes 4 bytes.
      //int iFatSectorsRequired = iSectorCount >> header.SectorShift - 2; // == iSectorCount * IntSize /

      // Number of fat sectors.
      // To find out total number of fat sectors we have to solve following equations:
      // ( items + fat + dif ) / 128 = fat
      // dif = ( fat - 109 ) / 127
      // where items - number of sectors used by file items (streams and storages)
      // fat - number of sectors used by fat
      // dif - number of sectors used by dif
      // 109 - part of the dif stored in the header
      // 128 - number of sector indexes in single fat sector
      // 127 - number of items in single dif sector
      int iFatRecordInSector = SectorSize / FileHeader.IntSize;
      int iDifInSector = iFatRecordInSector - 1;

      double dNumerator = ( double )iDifInSector * iSectorCount - DIF.SectorsInHeader;
      double dDenumerator = ( double )iDifInSector * iDifInSector - 1;
      int iFatSectorsRequired = ( int )Math.Ceiling( dNumerator / dDenumerator );
      header.FatSectorsNumber = iFatSectorsRequired;
      // Single sector - to reduce number of stream.Write calls.
      byte[] arrSector = new byte[ sectorSize ];

      // Allocates required number of dif sectors and marks sectors as used by dif.
      dif.AllocateSectors( iFatSectorsRequired, this );
      // Allocate required number of fat sectors and mark them as used by fat.
      AllocateFatSectors( iFatSectorsRequired, dif );

      List<int> arrFatSectors = dif.SectorIds;

      for( int i = 0, iCurrentItem = 0; i < iFatSectorsRequired; i++ )
      {
        iCurrentItem = FillNextSector( iCurrentItem, arrSector );
        int sectorIndex = arrFatSectors[ i ];
        long sectorOffset = CompoundFile.GetSectorOffset( sectorIndex, sectorShift );
        stream.Seek( sectorOffset, SeekOrigin.Begin );
        stream.Write( arrSector, 0, sectorSize );
      }
    }
    /// <summary>
    /// Allocates required number of fat sectors.
    /// </summary>
    /// <param name="fatSectorsCount">Number of sectors that must be allocated.</param>
    /// <param name="dif">DIF structure that contains info about fat sectors sequence.</param>
    private void AllocateFatSectors( int fatSectorsCount, DIF dif )
    {
      if( dif == null )
        throw new ArgumentNullException( "dif" );

      List<int> arrSectorIds = dif.SectorIds;
      int iSectorCount = arrSectorIds.Count;

      if( iSectorCount < fatSectorsCount )
      {
        for( int i = iSectorCount; i < fatSectorsCount; i++ )
        {
          int newSector = AllocateSector( SectorTypes.FatSector );
          arrSectorIds.Add( newSector );
        }
      }
    }
    /// <summary>
    /// Fills single fat sector.
    /// </summary>
    /// <param name="fatItemToStart">Index in the fat to start writing from.</param>
    /// <param name="arrSector">Sector to fill.</param>
    /// <returns>First item that wasn't saved inside sector.</returns>
    private int FillNextSector( int fatItemToStart, byte[] arrSector )
    {
      //int iEndFatItem = fatItemToStart + arrSector.Length / FileHeader.IntSize;
      int iCount = m_lstFatChains.Count;
      int iSectorLength = arrSector.Length;
      int iOffset = 0;

      for( ; iOffset < iSectorLength && fatItemToStart < iCount;
        iOffset += FileHeader.IntSize, fatItemToStart++ )
      {
        // TODO: optimize this thing if possible.
        Buffer.BlockCopy( BitConverter.GetBytes( m_lstFatChains[ fatItemToStart ] ),
          0, arrSector, iOffset, FileHeader.IntSize );
      }

      if( iOffset < iSectorLength )
      {
        // Fill end of it with invalid sector id.
        byte[] arrInvalidSector = BitConverter.GetBytes( SectorTypes.FreeSector );//SectorTypes.EndOfChain );
        while( iOffset < iSectorLength )
        {
          Buffer.BlockCopy( arrInvalidSector, 0, arrSector, iOffset, FileHeader.IntSize );
          iOffset += FileHeader.IntSize;
        }
      }

      return fatItemToStart;
    }
    /// <summary>
    /// Allocates new sector of the specified sector type.
    /// </summary>
    /// <param name="sectorType">Sector type to allocate.</param>
    /// <returns>Allocated sector index.</returns>
    internal int AllocateSector( int sectorType )
    {
      int sector;
      int iFreeCount = m_freeSectors.Count;

      if( iFreeCount > 0 )
      {
        int index = iFreeCount - 1;
        sector = m_freeSectors[ index ];
        m_freeSectors.RemoveAt( index );
        m_lstFatChains[ sector ] = sectorType;
      }
      else
      {
        sector = AddSector();
        m_lstFatChains.Add( sectorType );
      }

      return sector;
    }
    /// <summary>
    /// Adds single sector to the stream.
    /// </summary>
    /// <returns>Index of the added sector.</returns>
    internal int AddSector()
    {
      long lCurrentLength = m_stream.Length;
      int iSectorSize = SectorSize;
      m_stream.SetLength( lCurrentLength + iSectorSize );
      return ( int )( ( lCurrentLength - m_iHeaderSize ) >> m_usSectorShift );
    }
    /// <summary>
    /// Writes fat data directly into a stream.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
    /// <param name="sectorSize">Size of the sector to use for writing.</param>
    internal void WriteSimple( MemoryStream stream, int sectorSize )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( sectorSize <= 0 )
        throw new ArgumentOutOfRangeException( "sectorSize" );

      int iSectorCount = m_lstFatChains.Count;
      int iFatSectorsRequired = ( int )Math.Ceiling( iSectorCount * FileHeader.IntSize / ( double )sectorSize );
      byte[] arrSector = new byte[ sectorSize ];
      int itemsPerSector = sectorSize / FileHeader.IntSize;

      for( int i = 0; i < iFatSectorsRequired; i++ )
      {
        int startItem = i * itemsPerSector;
        FillNextSector( startItem, arrSector );
        stream.Write( arrSector, 0, sectorSize );
      }
    }
    /// <summary>
    /// Evaluates sector offset.
    /// </summary>
    /// <param name="sectorIndex">Zero-based sector index to evaluate offset for.</param>
    /// <returns>Offset to the sector start.</returns>
    internal long GetSectorOffset( int sectorIndex )
    {
      return CompoundFile.GetSectorOffset( sectorIndex, m_usSectorShift, m_iHeaderSize );
    }
    /// <summary>
    /// Evaluates number of sectors in the sector chain starting from the specified sector.
    /// </summary>
    /// <param name="firstSector">Starting sector of the entry to enumerate.</param>
    /// <returns>Number of sectros in the sector chain.</returns>
    internal int GetChainLength( int firstSector )
    {
      int iCount = 1;

      while( ( firstSector = NextSector( firstSector ) ) >= 0 )
      {
        iCount++;
      }

      return iCount;
    }
    #endregion
  }
}
