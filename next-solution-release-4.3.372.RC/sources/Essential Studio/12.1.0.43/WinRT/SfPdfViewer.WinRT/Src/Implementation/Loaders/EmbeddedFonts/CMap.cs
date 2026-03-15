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
    internal class CMap
    {
        private MemoryStream m_cmapStream = new MemoryStream();
        int[] m_format6Glyph;

        int[] m_platformId;
        int[] m_platformSpecificId;
        int[] m_offset;
        int m_firstCode, m_entryCount, m_format6TableLength;

        internal CMap()
        {
            
        }

        internal MemoryStream CreateCMapStream()
        {

            m_platformId = new int[] { 1, 3 };
            m_platformSpecificId = new int[] { 0, 1 };
            m_offset = new int[] { 20, 131102 };

            //Format of 1st sub-table
            GenerateFormat6Table();
            WriteCMAPHeader();
            WriteFormat6Table();
            GenerateFormat4Table();
            return m_cmapStream;
        }

        internal void WriteCMAPHeader()
        {
            //id
            WriteShort(0);
            //no of Sub-tables
            WriteShort(2);

            for (int i = 0; i < 2; i++)
            {
                WriteShort((short)m_platformId[i]);
                WriteShort((short)m_platformSpecificId[i]);
                WriteInt(m_offset[i]);
            }
        }

        internal void WriteFormat6Table()
        {
            //Table Format
            WriteShort(6);

            m_format6TableLength /= 4;
            //Subtable length
            WriteShort((short)(m_format6TableLength));

            //Language code
            WriteShort(0);

            WriteShort((short)m_firstCode);
            WriteShort((short)m_entryCount);

            for (int i = 0; i < m_format6Glyph.Length; i++)
            {
                WriteShort((short)m_format6Glyph[i]);
            }
        }

        internal void GenerateFormat6Table()
        {
            Dictionary<double, double> mapTable = null;
            mapTable = new Dictionary<double, double>();
            for (int i = 0; i < 65535; i++)
            {
                mapTable.Add(i, i);
            }
            mapTable.Add(65535, 1);
            List<int> glyphPositions = new List<int>();

            m_firstCode = 0;
            m_entryCount = mapTable.Count;
            m_format6Glyph = new int[m_firstCode + m_entryCount];

            for (int i = 0; i < m_format6Glyph.Length; i++)
            {
                m_format6Glyph[i] = (int)mapTable[i];
            }

            m_format6TableLength = ((m_entryCount - m_firstCode) * 2) + 10;

            m_offset[1] = m_offset[0] + m_format6TableLength;
        }

        internal void GenerateFormat4Table()
        {
            int segCount, segCountX2, searchRange, entrySelector, rangeShift;
            int diff, largestPowerOf2 = 1;
            int[] endCode, startCode, idDelta, idRangeOffset;

            List<int> idGlyph = new List<int>();
            //To calculate segmentCount
            for (int i = 0; i < m_format6Glyph.Length; i++)
            {
                if (m_format6Glyph[i] != 0)
                {
                    diff = i - m_format6Glyph[i];
                    if (!idGlyph.Contains(diff))
                        idGlyph.Add(diff);
                }
            }

            segCount = idGlyph.Count;
            segCountX2 = segCount * 2;

            while (largestPowerOf2 <= segCount)
            {
                largestPowerOf2 *= 2;
            }

            largestPowerOf2 /= 2;

            searchRange = 2 * largestPowerOf2;
            entrySelector = (int)(Math.Log(searchRange / 2, 2));
            rangeShift = (2 * segCount) - searchRange;

            endCode = new int[segCount];
            startCode = new int[segCount];
            idDelta = new int[segCount];
            idRangeOffset = new int[segCount];
            int segCnt = -1;
            Dictionary<int, int> differenceDictionary = new Dictionary<int, int>();
            for (int i = 0; i < m_format6Glyph.Length; i++)
            {
                diff = i - m_format6Glyph[i];
                if (idGlyph.Contains(diff))
                {
                    differenceDictionary.Add(i, diff);
                }
            }
            diff = -1;
            foreach (KeyValuePair<int, int> element in differenceDictionary)
            {
                try
                {
                    if (diff != element.Value)
                    {
                        segCnt++;
                        startCode[segCnt] = element.Key;
                        diff = element.Value;
                    }
                    endCode[segCnt] = element.Key;
                }
                catch
                {
                }
            }
            for (int i = 0; i < segCount; i++)
            {
                idRangeOffset[i] = 0;
            }

            for (int i = 0; i < idGlyph.Count; i++)
            {
                idDelta[i] = 65536 - idGlyph[i];
            }

            startCode[segCount - 1] = 65535;
            endCode[segCount - 1] = 65535;
            //Calculate Stream length
            int length = 16 + (4 * 2 * segCount);

            //format
            WriteShort((short)4);
            //lenght
            WriteShort((short)length);
            //language
            WriteShort((short)0);
            //segCountX2
            WriteShort((short)segCountX2);
            //SearchRange
            WriteShort((short)searchRange);
            //entry Selector
            WriteShort((short)entrySelector);
            //rangeShift
            WriteShort((short)rangeShift);

            //end code
            for (int i = 0; i < segCount; i++)
            {
                WriteShort((short)endCode[i]);
            }

            //Reserved Pad
            WriteShort(0);

            //Start code
            for (int i = 0; i < segCount; i++)
            {
                WriteShort((short)startCode[i]);
            }

            //id Delta
            for (int i = 0; i < segCount; i++)
            {
                WriteShort((short)idDelta[i]);
            }

            //id Range Offset
            for (int i = 0; i < segCount; i++)
            {
                WriteShort((short)idRangeOffset[i]);
            }
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

            m_cmapStream.Write(buffer, 0, 2);
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

            m_cmapStream.Write(buffer, 0, 4);
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
            m_cmapStream.Write(buffer, 0, 4);
        }

        /// <summary>
        /// Write the bytes into the font stream
        /// </summary>
        /// <param name="buffer">byte array to be written</param>
        public void WriteBytes(byte[] buffer)
        {
            m_cmapStream.Write(buffer, 0, buffer.Length);
        }
    }


    internal struct Subtable6Entry
    {
        internal int endCode, startCode, idDelta, idRangeOffset;
    };
}

