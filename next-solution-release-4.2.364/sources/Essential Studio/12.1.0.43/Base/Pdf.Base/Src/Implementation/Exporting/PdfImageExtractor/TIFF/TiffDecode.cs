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
using System.Globalization;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Decodes the image stream in the PDF document into an image
    /// </summary>
    internal class TiffDecode
    {
        internal MemoryStream m_stream = null;
        internal TiffHeader m_tiffHeader;
        internal TiffDirectoryEntry m_directory = new TiffDirectoryEntry();
        internal const int LittleEndianVersion = 42;
        internal const int BigEndianVersion = 43;
        internal const short BigEndian = 0x4d4d;
        internal const short LittleEndian = 0x4949;
        internal const short MdiLittleEndian = 0x5045;

        internal List<TiffDirectoryEntry> directoryEntries = new List<TiffDirectoryEntry>();

        public TiffDecode()
        {
            m_stream = new MemoryStream();
        }

        /// <summary>
        /// Sets the fields associated with the TIFF image
        /// </summary>
        /// <param name="count">Number of fields</param>
        /// <param name="offset">Value of the field</param>
        /// <param name="tag">Name of the TIFF tag</param>
        /// <param name="type">Type of the tag value</param>
        internal void SetField(int count, int offset, TiffTag tag, TiffType type)
        {
            TiffDirectoryEntry entry = new TiffDirectoryEntry();
            entry.DirectoryCount = count;
            entry.DirectoryOffset = (uint)offset;
            entry.DirectoryTag = tag;
            entry.DirectoryType = type;

            directoryEntries.Add(entry);
        }
        
        /// <summary>
        /// Writes the header to the TIFF image
        /// </summary>
        /// <param name="header">Specifies the header of the TIFF image</param>
        internal void WriteHeader(TiffHeader header)
        {
            WriteShort(header.m_byteOrder);
            WriteShort(header.m_version);
            WriteInt((int)header.m_dirOffset);
        }

        /// <summary>
        /// Writes the list of fields associated with the TIFF image
        /// </summary>
        /// <param name="entries">List of TIFF fields</param>
        internal void WriteDirEntry(List<TiffDirectoryEntry> entries)
        {
            int count = entries.Count;
            WriteShort((short)count);
            for (int i = 0; i < count; i++)
            {
                WriteShort((short)entries[i].DirectoryTag);
                WriteShort((short)entries[i].DirectoryType);
                WriteInt(entries[i].DirectoryCount);
                WriteInt((int)entries[i].DirectoryOffset);
            }
            WriteInt(0);
        }

        /// <summary>
        /// Writes short value into the TIFF stream
        /// </summary>
        /// <param name="value">Short value to be written</param>
        private void WriteShort(short value)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);

            m_stream.Write(buffer, 0, 2);
        }

        /// <summary>
        /// Writes integer value into the TIFF stream
        /// </summary>
        /// <param name="value">Integer value to be written</param>
        private void WriteInt(int value)
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);

            m_stream.Write(buffer, 0, 4);
        }
    }

    /// <summary>
    /// Structure of the TIFF header
    /// </summary>
    struct TiffHeader
    {
        public const int ByteOrderSize = 2;
        public const int VersionSize = 2;
        public const int DirOffsetSize = 4;

        public const int SizeInBytes = ByteOrderSize + VersionSize + DirOffsetSize;

        /// <summary>
        /// Size of the byte order of the tiff image
        /// </summary>
        public short m_byteOrder;

        /// <summary>
        /// TIFF version number
        /// </summary>
        public short m_version;

        /// <summary>
        /// byte offset to first directory
        /// </summary>
        public uint m_dirOffset;
    }

    /// <summary>
    /// Tag entry to the TIFF stream
    /// </summary>
    class TiffDirectoryEntry
    {
        public const int SizeInBytes = 12;

        /// <summary>
        /// Represents the TIFF tag
        /// </summary>
        public TiffTag DirectoryTag;

        /// <summary>
        /// Represents the type of the TIFF tag
        /// </summary>
        public TiffType DirectoryType;

        /// <summary>
        /// number of items; length in spec
        /// </summary>
        public int DirectoryCount;

        /// <summary>
        /// byte offset to field data
        /// </summary>
        public uint DirectoryOffset;
    }
}
