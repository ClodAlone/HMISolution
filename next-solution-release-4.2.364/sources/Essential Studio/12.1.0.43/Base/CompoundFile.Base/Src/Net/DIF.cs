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
    class DIF
    {
        #region Constants
        /// <summary>
        /// Number of items in the file header.
        /// </summary>
        public const int SectorsInHeader = 109;
        #endregion

        #region Members
        /// <summary>
        /// List of all fat sector ids.
        /// </summary>
        private List<int> m_arrSectorID;
        /// <summary>
        /// List with used Dif sectors.
        /// </summary>
        private List<int> m_arrDifSectors = new List<int>();
        #endregion

        #region Methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DIF()
        {
            // In our version, fat sector would be the first one, and no others.
            m_arrSectorID = new List<int>();
            //m_arrSectorID.Add( 0 );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="header"></param>
        public DIF(Stream stream, FileHeader header)
        {
            int iDifNumber = header.DifNumber;
            int iSectorSize = header.SectorSize;
            ushort sectorShift = header.SectorShift;
            int iCount = SectorsInHeader + iDifNumber * (iSectorSize - FileHeader.IntSize) / FileHeader.IntSize;
            m_arrSectorID = new List<int>(iCount);
            m_arrSectorID.AddRange(header.FatStart);

            if (iDifNumber > 0)
            {
                int iSectorId = header.DifStart;
                ushort usSectorShift = header.SectorShift;
                byte[] arrBuffer = new byte[iSectorSize];
                int[] arrData = new int[iSectorSize / FileHeader.IntSize - 1];

                // For some reason there are sectors that ends up with FreeSector instead of EndOfChain.
                while (iSectorId >= 0)
                {
                    long lOffset = CompoundFile.GetSectorOffset(iSectorId, sectorShift);
                    m_arrDifSectors.Add(iSectorId);

                    stream.Position = lOffset;
                    stream.Read(arrBuffer, 0, iSectorSize);
                    Buffer.BlockCopy(arrBuffer, 0, arrData, 0, iSectorSize - 4);
                    m_arrSectorID.AddRange(arrData);
                    iSectorId = BitConverter.ToInt32(arrBuffer, iSectorSize - 4);
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public List<int> SectorIds
        {
            get
            {
                return m_arrSectorID;
            }
        }
        #endregion

        internal void Write(Stream stream, FileHeader header)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            // 1. First 109 go to header
            // 2. others must be written into their sectors.
            // 3. Update header with first dif sector.

            int iFatSectorCount = m_arrSectorID.Count;
            int[] arrFatStart = header.FatStart;
            int index;

            for (index = 0; index < iFatSectorCount && index < SectorsInHeader; index++)
            {
                arrFatStart[index] = m_arrSectorID[index];
            }

            for (; index < SectorsInHeader; index++)
            {
                arrFatStart[index] = SectorTypes.FreeSector;
            }

            if (m_arrDifSectors.Count > 0)
            {
                header.DifStart = m_arrDifSectors[0];
                header.DifNumber = m_arrDifSectors.Count;
            }

            byte[] arrBuffer = new byte[header.SectorSize];
            // -1 - because we have pointer to the next sector at the end
            int iSectorSize = header.SectorSize;
            int iCountInSector = iSectorSize / FileHeader.IntSize - 1;

            for (int i = 0, len = m_arrDifSectors.Count; i < len; i++)
            {
                int iSectorIndex = m_arrDifSectors[i];
                long lOffset = header.GetSectorOffset(iSectorIndex);

                for (int j = 0, offset = 0; j < iCountInSector; j++, index++, offset += FileHeader.IntSize)
                {
                    int value = (index < iFatSectorCount) ? m_arrSectorID[index] : -1;
                    byte[] arrValue = BitConverter.GetBytes(value);
                    Buffer.BlockCopy(arrValue, 0, arrBuffer, offset, FileHeader.IntSize);
                }

                int iNextSector = (i == len - 1) ?
                  SectorTypes.EndOfChain :
                  m_arrDifSectors[i + 1];
                byte[] arrNextSector = BitConverter.GetBytes(iNextSector);
                Buffer.BlockCopy(arrNextSector, 0, arrBuffer, iSectorSize - FileHeader.IntSize, FileHeader.IntSize);

                stream.Position = lOffset;
                stream.Write(arrBuffer, 0, iSectorSize);
            }
        }

        internal void AllocateSectors(int fatSectorsRequired, FAT fat)
        {
            int iAdditionalFatSectors = fatSectorsRequired - SectorsInHeader;

            // 1. Do we need any additional sectors besides that part of file header for dif?
            if (iAdditionalFatSectors > 0)
            {
                // - FileHeader.IntSize because we have to write next dif sector at the end of each dif sector.
                int iAddDifSectors = (int)Math.Ceiling(iAdditionalFatSectors * FileHeader.IntSize /
                  (double)(fat.SectorSize - FileHeader.IntSize));

                AllocateDifSectors(iAddDifSectors, fat);
            }
        }

        private void AllocateDifSectors(int additionalSectors, FAT fat)
        {
            int iCurrentCount = m_arrDifSectors.Count;

            if (iCurrentCount == additionalSectors)
                return;

            if (iCurrentCount > additionalSectors)
            {
                RemoveLastSectors(iCurrentCount - additionalSectors, fat);
            }
            else
            {
                AddDifSectors(additionalSectors - iCurrentCount, fat);
            }
        }

        private void RemoveLastSectors(int sectorCount, FAT fat)
        {
            if (sectorCount < 0)
                throw new ArgumentOutOfRangeException("sectorCount");

            if (sectorCount == 0)
                return;

            if (fat == null)
                throw new ArgumentNullException("fat");

            // Mark sectors as free in the fat.
            for (int i = 0, index = m_arrDifSectors.Count - 1; i < sectorCount; i++, index--)
            {
                int iCurrentSector = m_arrDifSectors[index];
                fat.FreeSector(iCurrentSector);
            }

            // Remove items from the list.
            m_arrDifSectors.RemoveRange(m_arrDifSectors.Count - sectorCount, sectorCount);

            // We have to update m_arrSectorID array and free fat sectors.
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds required number of DIF sectors.
        /// </summary>
        /// <param name="sectorCount">Number of sectors to add.</param>
        /// <param name="fat">FAT object.</param>
        private void AddDifSectors(int sectorCount, FAT fat)
        {
            if (sectorCount < 0)
                throw new ArgumentOutOfRangeException("sectorCount");

            if (fat == null)
                throw new ArgumentNullException("fat");

            for (int i = 0; i < sectorCount; i++)
            {
                int sector = fat.AllocateSector(SectorTypes.DifSector);
                m_arrDifSectors.Add(sector);
            }
        }
    }
}
