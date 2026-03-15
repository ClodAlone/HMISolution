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
    internal class ListFormatOverrideLevel : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        internal int m_startAt;

        internal int m_ilvl;
        internal bool m_bStartAt;
        internal bool m_bFormatting;
        internal int m_reserved1;
        internal int m_reserved2;
        internal int m_reserved3;

        /// <summary>
        /// 
        /// </summary>
        internal ListLevel m_lvl;
        #endregion

        #region Class constructor
        /// <summary>
        /// 
        /// </summary>
        internal ListFormatOverrideLevel(bool overrideLvl)
        {
            if (overrideLvl)
            {
                m_lvl = new ListLevel();
            }
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal ListFormatOverrideLevel(Stream stream)
        {
            m_startAt = (int)ReadUInt32(stream);
            int temp = stream.ReadByte();
            m_ilvl = temp & 15;
            m_bStartAt = (temp & 0x10) != 0;
            m_bFormatting = (temp & 0x20) != 0;
            m_reserved1 = stream.ReadByte();
            m_reserved2 = stream.ReadByte();
            m_reserved3 = stream.ReadByte();

            if (m_bFormatting)
            {
                m_lvl = new ListLevel(stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="memConvertor"></param>
        internal void Write(Stream stream)
        {
            WriteUInt32(stream, (uint)m_startAt);
            int temp = 0;
            temp |= m_ilvl;
            temp |= (m_bStartAt ? 0x10 : 0);
            temp |= (m_bFormatting ? 0x20 : 0);
            stream.WriteByte((byte)temp);
            stream.WriteByte((byte)m_reserved1);
            stream.WriteByte((byte)m_reserved2);
            stream.WriteByte((byte)m_reserved3);

            if (m_bFormatting)
            {
                m_lvl.Write(stream);
            }
        }
        #endregion
    }
}

