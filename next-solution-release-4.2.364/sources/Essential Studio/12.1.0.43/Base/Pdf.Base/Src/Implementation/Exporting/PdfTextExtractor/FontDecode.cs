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

namespace Syncfusion.Pdf
{
    internal class FontDecode
    {
        internal MemoryStream m_fontStream = new MemoryStream();
        internal FontHeader m_fontHeader;
        internal int m_tableCount = 0;

        private int tagOffset = 0;
        private int offset = 0;
        private int previousLength = 0;

        /// <summary>
        /// Generate the font stream with respect to the font tables present in the entries
        /// </summary>
        /// <param name="entries">list of font tables in the font</param>
        /// <returns>stream of the font</returns>
        internal MemoryStream CreateFontStream(List<TableEntry> entries)
        {
            m_tableCount = entries.Count;
            FontHeader header = new FontHeader();
            header.scalarType = 65536;
            header.noOfTables = (short)m_tableCount;
            header.searchRange = (short)GetSearchRange(header.noOfTables);
            header.entrySelector = (short)GetEntrySelector(header.noOfTables);
            header.rangeShift = (short)GetRangeShift(header.noOfTables, header.searchRange);
            WriteHeader(header);

            tagOffset = 12 + (entries.Count * 16);

            foreach (TableEntry entry in entries)
            {
                tagOffset += previousLength;
                entry.offset = tagOffset;
                previousLength = entry.length;
                WriteEntry(entry);
            }

            foreach (TableEntry entry in entries)
            {
                WriteBytes(entry.bytes);
            }
            m_fontStream.Capacity = (int)m_fontStream.Length + 1;
            return m_fontStream;
        }

        /// <summary>
        /// Writes the header into the font stream
        /// </summary>
        /// <param name="head">Header information of the font</param>
        private void WriteHeader(FontHeader head)
        {
            m_fontStream.Position = 0;
            WriteInt(head.scalarType);
            WriteShort(head.noOfTables);
            WriteShort(head.searchRange);
            WriteShort(head.entrySelector);
            WriteShort(head.rangeShift);

            offset = (int)m_fontStream.Position;
        }

        /// <summary>
        /// Writes table entry into the font stream
        /// </summary>
        /// <param name="entry">Table entry</param>
        private void WriteEntry(TableEntry entry)
        {
            WriteString(entry.id);
            WriteInt(entry.checkSum);
            WriteInt(entry.offset);
            WriteInt(entry.length);
        }

        /// <summary>
        /// Writes short value into the font stream
        /// </summary>
        /// <param name="value">Short value to be written</param>
        private void WriteShort(short value)
        {
            byte[] buffer = new byte[2];
            buffer[1] = (byte)value;
            buffer[0] = (byte)(value >> 8);

            m_fontStream.Write(buffer, 0, 2);
        }

        /// <summary>
        /// Writes integer value into the font stream
        /// </summary>
        /// <param name="value">Integer value to be written</param>
        private void WriteInt(int value)
        {
            byte[] buffer = new byte[4];

            buffer[3] = (byte)value;
            buffer[2] = (byte)(value >> 8);
            buffer[1] = (byte)(value >> 16);
            buffer[0] = (byte)(value >> 24);

            m_fontStream.Write(buffer, 0, 4);
        }

        /// <summary>
        /// Writes string value into the font stream
        /// </summary>
        /// <param name="value">String value to be written</param>
        private void WriteString(string value)
        {
            byte[] buffer = new byte[value.Length];
            int i = 0;
            foreach (char c in value)
            {
                buffer[i] = (byte)c;
                i++;
            }
            m_fontStream.Write(buffer, 0, 4);
        }

        /// <summary>
        /// Write the bytes into the font stream
        /// </summary>
        /// <param name="buffer">byte array to be written</param>
        public void WriteBytes(byte[] buffer)
        {
            m_fontStream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Calculates the search range of the font
        /// </summary>
        /// <param name="noOfTables">Number of tables in the font</param>
        /// <returns>Value of the search range</returns>
        private int GetSearchRange(int noOfTables)
        {
            int searchRange = 2;
            while (searchRange * 2 <= noOfTables)
            {
                searchRange = (int)searchRange * 2;
            }
            searchRange *= 16;
            return searchRange;
        }

        /// <summary>
        /// Calculates the entry selector of the font
        /// </summary>
        /// <param name="noOfTables">Number of tables in the font</param>
        /// <returns>Value of the entry selector</returns>
        private int GetEntrySelector(int noOfTables)
        {
            int entrySelector = 2;
            while (entrySelector * 2 <= noOfTables)
            {
                entrySelector = (int)entrySelector * 2;
            }
            entrySelector = (int)Math.Log(entrySelector, 2);
            return entrySelector;
        }

        /// <summary>
        /// Calculates the range shift of the font
        /// </summary>
        /// <param name="noOfTables">Number of tables in the font</param>
        /// <param name="searchRange">Search range of the font</param>
        /// <returns>Value of the range shift</returns>
        private int GetRangeShift(int noOfTables, int searchRange)
        {
            return (noOfTables * 16) - searchRange;
        }
    }

    struct FontHeader
    {
        public int scalarType;
        public short noOfTables;
        public short searchRange;
        public short entrySelector;
        public short rangeShift;
    }

    public class TableEntry
    {
        public string id;
        public int checkSum;
        public int offset;
        public int length;
        public byte[] bytes = null;
    }
}
