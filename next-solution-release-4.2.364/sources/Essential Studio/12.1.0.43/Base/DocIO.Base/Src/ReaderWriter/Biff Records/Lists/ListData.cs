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
using System.IO;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ListData : BaseWordRecord
    {
        #region Class constants
        private const int DEF_LEVELS_COUNT = 9;
        private const int DEF_RGISTD = 0xfff;
        private const int DEF_SIMPLE_BIT = 0x01;
        private const int DEF_HYBRID_BIT = 0x10;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_lsid;
        private int m_tplc;
        private int[] m_rgistd;
        private int m_Options;
        private ListLevels m_levels;
        private string m_name;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lsid"></param>
        internal ListData(int lsid)
            : this(lsid, true, false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListData"/> class.
        /// </summary>
        /// <param name="lsid">The lsid.</param>
        /// <param name="isHybrid">if it is hybrid, set to <c>true</c>.</param>
        /// <param name="isSimpleList">if it is simple list, set to <c>true</c>.</param>
        internal ListData(int lsid, bool isHybrid, bool isSimpleList)
        {
            m_rgistd = new int[DEF_LEVELS_COUNT];
            m_levels = new ListLevels();
            m_lsid = lsid;
            m_tplc = ~lsid;
            int rgLen = (isSimpleList) ? 1 : DEF_LEVELS_COUNT;
            for (int i = 0; i < rgLen; i++)
            {
                m_rgistd[i] = DEF_RGISTD;
            }
            if (isSimpleList)
            {
                m_Options = m_Options | DEF_SIMPLE_BIT;
            }

            if (isHybrid)
            {
                m_Options = (m_Options | DEF_HYBRID_BIT);
            }

            m_name = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal ListData(Stream reader)
        {
            m_rgistd = new int[9];
            m_levels = new ListLevels();
            ReadListData(reader);
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets ListLevels object
        /// </summary>
        internal ListLevels Levels
        {
            get
            {
                return m_levels;
            }
        }

        /// <summary>
        /// Gets/sets list name
        /// </summary>
        internal string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// True if restart heading
        /// </summary>
        internal bool RestartHeading
        {
            get
            {
                return ((m_Options & 2) != 0);
            }
        }

        /// <summary>
        /// True if it is simplelist
        /// </summary>
        internal bool SimpleList
        {
            get
            {
                return ((m_Options & DEF_SIMPLE_BIT) != 0);
            }
            set
            {
                m_Options &= 0xFE;
                m_Options |= (value ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is hybrid multilevel.
        /// </summary>
        /// <value>
        /// 	if this instance is hybrid multilevel, set to <c>true</c>.
        /// </value>
        internal bool IsHybridMultilevel
        {
            get
            {
                return ((m_Options & DEF_HYBRID_BIT) != 0);
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
        internal void ReadLvl(Stream stream)
        {
            int count = SimpleList ? 1 : DEF_LEVELS_COUNT;

            for (int i = 0; i < count; i++)
            {
                m_levels.Add(new ListLevel(stream));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void WriteListData(Stream stream)
        {
            if (m_levels.Count == 1)
            {
                SimpleList = true;
            }
            WriteInt32(stream, m_lsid);
            WriteInt32(stream, m_tplc);
            for (int num1 = 0; num1 < m_rgistd.Length; num1++)
            {
                WriteInt16(stream, (short)m_rgistd[num1]);
            }
            WriteUInt16(stream, (ushort)m_Options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void ReadListData(Stream stream)
        {
            m_lsid = ReadInt32(stream);
            m_tplc = ReadInt32(stream);
            for (int num1 = 0; num1 < m_rgistd.Length; num1++)
            {
                m_rgistd[num1] = ReadInt16(stream);
            }
            m_Options = ReadUInt16(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="memConvertor"></param>
        internal void WriteLvl(Stream stream)
        {
            foreach (ListLevel lvl in m_levels)
            {
                lvl.Write(stream);
            }
        }
        #endregion
    }
}

