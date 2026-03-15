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
    /// This class represents compound file header.
    /// </summary>
    class FileHeader
    {
        #region Constants
        /// <summary>
        /// Size of the header.
        /// </summary>
        public const int HeaderSize = 512;
        /// <summary>
        /// Signature size.
        /// </summary>
        private const int SignatureSize = 8;
        /// <summary>
        /// Default (and the only supported) signature.
        /// </summary>
        private static readonly byte[] DefaultSignature = new byte[SignatureSize] { 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 };

        // TODO: move these constants to another file.
        internal const int ShortSize = 2;
        internal const int IntSize = 4;
        #endregion

        #region Members
        /// <summary>
        /// File signature.
        /// </summary>
        private byte[] m_arrSignature = new byte[SignatureSize];
        /// <summary>
        /// Class id.
        /// </summary>
        //private byte[] m_arrClassId = new byte[ 8 ];
        private Guid m_classId = new Guid();
        /// <summary>
        /// Minor version of the format.
        /// </summary>
        private ushort m_usMinorVersion = 0x3E;
        /// <summary>
        /// Major version of the dll/format.
        /// </summary>
        private ushort m_usDllVersion = 3;
        /// <summary>
        /// Byte order, 0xFFFE for Intel byte-ordering.
        /// </summary>
        private ushort m_usByteOrder = 0xFFFE;
        /// <summary>
        /// Size of sectors in power-of-two (typically 9).
        /// </summary>
        private ushort m_usSectorShift = 9;
        /// <summary>
        /// Size of mini-sectors in power-of-two (typically 6).
        /// </summary>
        private ushort m_usMiniSectorShift = 6;
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        private ushort m_usReserved;
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        private uint m_uiReserved1;
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        private uint m_uiReserved2;
        /// <summary>
        /// Number of sectors in the FAT chain.
        /// </summary>
        private int m_iFatSectorsNumber;
        /// <summary>
        /// First sector in the directory chain.
        /// </summary>
        private int m_iDirectorySectorStart = -1;
        /// <summary>
        /// Signature used for transactioning, must be zero.
        /// </summary>
        private int m_iSignature;
        /// <summary>
        /// Maximum size for mini-streams. Typically 4096 bytes.
        /// </summary>
        private uint m_uiMiniSectorCutoff = 4096;
        /// <summary>
        /// First sector in the mini-FAT chain.
        /// </summary>
        private int m_iMiniFastStart = -2;
        /// <summary>
        /// Number of sectors in the mini-FAT chain.
        /// </summary>
        private int m_iMiniFatNumber;
        /// <summary>
        /// First sector in the DIF chain.
        /// </summary>
        private int m_iDifStart = SectorTypes.EndOfChain;
        /// <summary>
        /// Number of sectors in the DIF chain.
        /// </summary>
        private int m_iDifNumber;
        /// <summary>
        /// First 109 fat sectors.
        /// </summary>
        private int[] m_arrFatStart = new int[109];
        #endregion

        #region Methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileHeader()
        {
            Buffer.BlockCopy(DefaultSignature, 0, m_arrSignature, 0, SignatureSize);
        }
        /// <summary>
        /// Initializes new instance of the file header and extracts data from the stream.
        /// </summary>
        /// <param name="stream">Stream to extract header data from.</param>
        public FileHeader(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (stream.Length < HeaderSize)
                throw new CompoundFileException();

            byte[] arrBuffer = new byte[HeaderSize];
            stream.Read(arrBuffer, 0, HeaderSize);

            Buffer.BlockCopy(arrBuffer, 0, m_arrSignature, 0, SignatureSize);
            CheckSignature();

            int iOffset = SignatureSize;
            const int GuidSize = 16;
            byte[] arrClassId = new byte[GuidSize];
            Buffer.BlockCopy(arrBuffer, iOffset, arrClassId, 0, GuidSize);
            iOffset += GuidSize;
            m_classId = new Guid(arrClassId);

            m_usMinorVersion = BitConverter.ToUInt16(arrBuffer, iOffset);
            iOffset += ShortSize;

            m_usDllVersion = BitConverter.ToUInt16(arrBuffer, iOffset);
            iOffset += ShortSize;

            m_usByteOrder = BitConverter.ToUInt16(arrBuffer, iOffset);
            iOffset += ShortSize;

            m_usSectorShift = BitConverter.ToUInt16(arrBuffer, iOffset);
            iOffset += ShortSize;

            m_usMiniSectorShift = BitConverter.ToUInt16(arrBuffer, iOffset);
            iOffset += ShortSize;

            m_usReserved = BitConverter.ToUInt16(arrBuffer, iOffset);
            iOffset += ShortSize;

            m_uiReserved1 = BitConverter.ToUInt32(arrBuffer, iOffset);
            iOffset += IntSize;

            m_uiReserved2 = BitConverter.ToUInt32(arrBuffer, iOffset);
            iOffset += IntSize;

            m_iFatSectorsNumber = BitConverter.ToInt32(arrBuffer, iOffset);
            iOffset += IntSize;

            m_iDirectorySectorStart = BitConverter.ToInt32(arrBuffer, iOffset);
            iOffset += IntSize;

            m_iSignature = BitConverter.ToInt32(arrBuffer, iOffset);
            iOffset += IntSize;

            m_uiMiniSectorCutoff = BitConverter.ToUInt32(arrBuffer, iOffset);
            iOffset += IntSize;

            m_iMiniFastStart = BitConverter.ToInt32(arrBuffer, iOffset);
            iOffset += IntSize;

            m_iMiniFatNumber = BitConverter.ToInt32(arrBuffer, iOffset);
            iOffset += IntSize;

            m_iDifStart = BitConverter.ToInt32(arrBuffer, iOffset);
            iOffset += IntSize;

            m_iDifNumber = BitConverter.ToInt32(arrBuffer, iOffset);
            iOffset += IntSize;

            Buffer.BlockCopy(arrBuffer, iOffset, m_arrFatStart, 0, m_arrFatStart.Length * IntSize);
        }
        /// <summary>
        /// Saves header into specified stream.
        /// </summary>
        /// <param name="stream">Stream to write header into.</param>
        public void Serialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            byte[] arrBuffer = new byte[HeaderSize];
            Buffer.BlockCopy(m_arrSignature, 0, arrBuffer, 0, SignatureSize);
            int iOffset = SignatureSize;

            const int GuidSize = 16;
            byte[] arrClassId = m_classId.ToByteArray();
            Buffer.BlockCopy(arrClassId, 0, arrBuffer, iOffset, GuidSize);
            iOffset += GuidSize;

            const int ShortSize = 2;
            const int IntSize = 4;

            WriteUInt16(arrBuffer, iOffset, m_usMinorVersion);
            iOffset += ShortSize;

            WriteUInt16(arrBuffer, iOffset, m_usDllVersion);
            iOffset += ShortSize;

            WriteUInt16(arrBuffer, iOffset, m_usByteOrder);
            iOffset += ShortSize;

            WriteUInt16(arrBuffer, iOffset, m_usSectorShift);
            iOffset += ShortSize;

            WriteUInt16(arrBuffer, iOffset, m_usMiniSectorShift);
            iOffset += ShortSize;

            WriteUInt16(arrBuffer, iOffset, m_usReserved);
            iOffset += ShortSize;

            WriteUInt32(arrBuffer, iOffset, m_uiReserved1);
            iOffset += IntSize;

            WriteUInt32(arrBuffer, iOffset, m_uiReserved2);
            iOffset += IntSize;

            WriteInt32(arrBuffer, iOffset, m_iFatSectorsNumber);
            iOffset += IntSize;

            WriteInt32(arrBuffer, iOffset, m_iDirectorySectorStart);
            iOffset += IntSize;

            WriteInt32(arrBuffer, iOffset, m_iSignature);
            iOffset += IntSize;

            WriteUInt32(arrBuffer, iOffset, m_uiMiniSectorCutoff);
            iOffset += IntSize;

            WriteInt32(arrBuffer, iOffset, m_iMiniFastStart);
            iOffset += IntSize;

            WriteInt32(arrBuffer, iOffset, m_iMiniFatNumber);
            iOffset += IntSize;

            WriteInt32(arrBuffer, iOffset, m_iDifStart);
            iOffset += IntSize;

            WriteInt32(arrBuffer, iOffset, m_iDifNumber);
            iOffset += IntSize;

            Buffer.BlockCopy(m_arrFatStart, 0, arrBuffer, iOffset, m_arrFatStart.Length * IntSize);

            stream.Write(arrBuffer, 0, HeaderSize);
        }
        /// <summary>
        /// Checks whether starting bytes of the stream are the same as signature of the compound file.
        /// </summary>
        /// <param name="stream">Stream to check.</param>
        /// <returns>True if stream contains required signature.</returns>
        public static bool CheckSignature(Stream stream)
        {
            bool bResult = false;

            if (stream != null)
            {
                byte[] arrBuffer = new byte[SignatureSize];
                long lPos = stream.Position;

                if (stream.Read(arrBuffer, 0, SignatureSize) == SignatureSize)
                {
                    bResult = CheckSignature(arrBuffer);
                }

                stream.Position = lPos;
            }

            return bResult;
        }
        /// <summary>
        /// Checks whether signature is supported.
        /// </summary>
        private void CheckSignature()
        {
            if (!CheckSignature(m_arrSignature))
                throw new CompoundFileException("Wrong signature");
        }
        /// <summary>
        /// Checks whether signature is supported.
        /// </summary>
        /// <param name="arrSignature">Data to compare with default signature.</param>
        private static bool CheckSignature(byte[] arrSignature)
        {
            bool bResult = false;

            if (arrSignature != null && arrSignature.Length == SignatureSize)
            {
                bResult = true;

                for (int i = 0; i < SignatureSize; i++)
                {
                    if (arrSignature[i] != DefaultSignature[i])
                    {
                        bResult = false;
                        break;
                    }
                }
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        private void WriteUInt16(byte[] buffer, int offset, ushort value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            buffer[offset] = (byte)(value & 0xFF);
            buffer[offset + 1] = (byte)((value & 0xFF00) >> 8);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        private void WriteUInt32(byte[] buffer, int offset, uint value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            buffer[offset] = (byte)(value & 0xFF);
            value >>= 8;

            buffer[offset + 1] = (byte)(value & 0xFF);
            value >>= 8;

            buffer[offset + 2] = (byte)(value & 0xFF);
            value >>= 8;

            buffer[offset + 3] = (byte)(value & 0xFF);
            value >>= 8;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        private void WriteInt32(byte[] buffer, int offset, int value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            buffer[offset] = (byte)(value & 0xFF);
            value >>= 8;

            buffer[offset + 1] = (byte)(value & 0xFF);
            value >>= 8;

            buffer[offset + 2] = (byte)(value & 0xFF);
            value >>= 8;

            buffer[offset + 3] = (byte)(value & 0xFF);
            value >>= 8;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Size of the sector. Read-only.
        /// </summary>
        public int SectorSize
        {
            get
            {
                return 1 << m_usSectorShift;
            }
        }
        /// <summary>
        /// Minor version of the format.
        /// </summary>
        public ushort MinorVersion
        {
            get
            {
                return m_usMinorVersion;
            }
        }
        /// <summary>
        /// Major version of the dll/format.
        /// </summary>
        public ushort DllVersion
        {
            get
            {
                return m_usDllVersion;
            }
        }
        /// <summary>
        /// Byte order, 0xFFFE for Intel byte-ordering.
        /// </summary>
        public ushort ByteOrder
        {
            get
            {
                return m_usByteOrder;
            }
        }
        /// <summary>
        /// Size of sectors in power-of-two (typically 9).
        /// </summary>
        public ushort SectorShift
        {
            get
            {
                return m_usSectorShift;
            }
        }
        /// <summary>
        /// Size of mini-sectors in power-of-two (typically 6).
        /// </summary>
        public ushort MiniSectorShift
        {
            get
            {
                return m_usMiniSectorShift;
            }
        }
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public ushort Reserved
        {
            get
            {
                return m_usReserved;
            }
        }
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public uint Reserved1
        {
            get
            {
                return m_uiReserved1;
            }
        }
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public uint Reserved2
        {
            get
            {
                return m_uiReserved2;
            }
        }
        /// <summary>
        /// Number of sectors in the FAT chain.
        /// </summary>
        public int FatSectorsNumber
        {
            get
            {
                return m_iFatSectorsNumber;
            }
            set
            {
                m_iFatSectorsNumber = value;
            }
        }
        /// <summary>
        /// First sector in the directory chain.
        /// </summary>
        public int DirectorySectorStart
        {
            get
            {
                return m_iDirectorySectorStart;
            }
            set
            {
                m_iDirectorySectorStart = value;
            }
        }
        /// <summary>
        /// Signature used for transactioning, must be zero.
        /// </summary>
        public int Signature
        {
            get
            {
                return m_iSignature;
            }
        }
        /// <summary>
        /// Maximum size for mini-streams. Typically 4096 bytes.
        /// </summary>
        public uint MiniSectorCutoff
        {
            get
            {
                return m_uiMiniSectorCutoff;
            }
        }
        /// <summary>
        /// First sector in the mini-FAT chain.
        /// </summary>
        public int MiniFastStart
        {
            get
            {
                return m_iMiniFastStart;
            }
            set
            {
                m_iMiniFastStart = value;
            }
        }
        /// <summary>
        /// Number of sectors in the mini-FAT chain.
        /// </summary>
        public int MiniFatNumber
        {
            get
            {
                return m_iMiniFatNumber;
            }
            set
            {
                m_iMiniFatNumber = value;
            }
        }
        /// <summary>
        /// First sector in the DIF chain.
        /// </summary>
        public int DifStart
        {
            get
            {
                return m_iDifStart;
            }
            set
            {
                m_iDifStart = value;
            }

        }
        /// <summary>
        /// Number of sectors in the DIF chain.
        /// </summary>
        public int DifNumber
        {
            get
            {
                return m_iDifNumber;
            }
            set
            {
                m_iDifNumber = value;
            }
        }
        /// <summary>
        /// First 109 fat sectors.
        /// </summary>
        public int[] FatStart
        {
            get
            {
                return m_arrFatStart;
            }
        }
        #endregion

        internal void Write(Stream stream)
        {
            byte[] arrBuffer = new byte[HeaderSize];

            Buffer.BlockCopy(m_arrSignature, 0, arrBuffer, 0, SignatureSize);

            int iOffset = SignatureSize;
            const int GuidSize = 16;
            //byte[] arrClassId = new byte[ GuidSize ];
            byte[] arrTemp = m_classId.ToByteArray();
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, GuidSize);
            iOffset += GuidSize;

            arrTemp = BitConverter.GetBytes(m_usMinorVersion);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, ShortSize);
            iOffset += ShortSize;

            arrTemp = BitConverter.GetBytes(m_usDllVersion);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, ShortSize);
            iOffset += ShortSize;

            arrTemp = BitConverter.GetBytes(m_usByteOrder);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, ShortSize);
            iOffset += ShortSize;

            arrTemp = BitConverter.GetBytes(m_usSectorShift);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, ShortSize);
            iOffset += ShortSize;

            arrTemp = BitConverter.GetBytes(m_usMiniSectorShift);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, ShortSize);
            iOffset += ShortSize;

            arrTemp = BitConverter.GetBytes(m_usReserved);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, ShortSize);
            iOffset += ShortSize;

            arrTemp = BitConverter.GetBytes(m_uiReserved1);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, IntSize);
            iOffset += IntSize;

            arrTemp = BitConverter.GetBytes(m_uiReserved2);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, IntSize);
            iOffset += IntSize;

            arrTemp = BitConverter.GetBytes(m_iFatSectorsNumber);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, IntSize);
            iOffset += IntSize;

            arrTemp = BitConverter.GetBytes(m_iDirectorySectorStart);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, IntSize);
            iOffset += IntSize;

            arrTemp = BitConverter.GetBytes(m_iSignature);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, IntSize);
            iOffset += IntSize;

            arrTemp = BitConverter.GetBytes(m_uiMiniSectorCutoff);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, IntSize);
            iOffset += IntSize;

            arrTemp = BitConverter.GetBytes(m_iMiniFastStart);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, IntSize);
            iOffset += IntSize;

            arrTemp = BitConverter.GetBytes(m_iMiniFatNumber);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, IntSize);
            iOffset += IntSize;

            arrTemp = BitConverter.GetBytes(m_iDifStart);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, IntSize);
            iOffset += IntSize;

            arrTemp = BitConverter.GetBytes(m_iDifNumber);
            Buffer.BlockCopy(arrTemp, 0, arrBuffer, iOffset, IntSize);
            iOffset += IntSize;

            Buffer.BlockCopy(m_arrFatStart, 0, arrBuffer, iOffset, m_arrFatStart.Length * IntSize);

            stream.Position = 0;
            stream.Write(arrBuffer, 0, HeaderSize);
        }

        internal long GetSectorOffset(int sectorIndex)
        {
            return (sectorIndex << m_usSectorShift) + FileHeader.HeaderSize;
        }
        internal long GetSectorOffset(int sectorIndex, int headerSize)
        {
            return (sectorIndex << m_usSectorShift) + headerSize;
        }
    }
}
