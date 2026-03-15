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
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// 
    /// </summary>
    internal class ListFormatOverride : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_lsid;
        private int m_unused1;
        private int m_unused2;
        internal int m_res1;
        internal int m_res2;
        private List<Object> m_levels;
        private int m_clfolvl;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal ListFormatOverride()
        {
            m_levels = new ListLevels();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal ListFormatOverride(Stream stream)
        {
            m_levels = new ListLevels();
            ReadLfo(stream);
        }
        #endregion      

        #region Class properties
        /// <summary>
        /// Gets arraylist of ListFormatOverrride levels
        /// </summary>
        internal List<Object> Levels
        {
            get
            {
                return m_levels;
            }
        }

        /// <summary>
        /// Gets/sets list identifier.
        /// </summary>
        internal int ListID
        {
            get
            {
                return m_lsid;
            }

            set
            {
                m_lsid = value;
            }
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void WriteLfo(Stream stream)
        {
            WriteInt32(stream, m_lsid);

            WriteInt32(stream, m_unused1);

            WriteInt32(stream, m_unused2);

            stream.WriteByte((byte)m_levels.Count);
            stream.WriteByte((byte)m_res1);

            WriteInt16(stream, (short)m_res2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void ReadLfo(Stream stream)
        {
            long startPos = stream.Position;

            m_lsid = ReadInt32(stream);
            m_unused1 = ReadInt32(stream);
            m_unused2 = ReadInt32(stream);
            m_clfolvl = stream.ReadByte();
            m_res1 = stream.ReadByte();
            m_res2 = ReadInt16(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="memConvertor"></param>
        internal void WriteLfoLvls(Stream stream)
        {
            WriteUInt32(stream, uint.MaxValue);
            ListFormatOverrideLevel listLevel = null;
            for (int i = 0, cnt = m_levels.Count; i < cnt; i++)
            {
                listLevel = (ListFormatOverrideLevel)m_levels[i];
                listLevel.Write(stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void ReadLfoLvls(Stream stream)
        {
            uint separator = ReadUInt32(stream);
            for (int i = 0; i < m_clfolvl; i++)
            {
                m_levels.Add(new ListFormatOverrideLevel(stream));
            }
        }
        #endregion
    }
}

