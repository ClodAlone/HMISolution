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
using System;
using System.IO;

using Syncfusion.DocIO.ReaderWriter.Biff_Records;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// File Blip Store Entry.
    /// </summary>
    internal class _FBSE : BaseWordRecord
    {
        #region Class constants
        public const int DEF_GUID_LENGTH = 16;
        public const int DEF_FBSE_LENGTH = 36;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        internal int m_btWin32;
        internal int m_btMacOS;
        internal byte[] m_rgbUid;
        internal int m_tag;
        internal int m_size;
        internal int m_cRef;
        internal int m_foDelay;
        internal int m_usage;
        internal int m_cbName;
        internal int m_unused2;
        internal int m_unused3;
        #endregion

        #region Class public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Read(Stream stream)
        {
            m_btWin32 = stream.ReadByte();
            m_btMacOS = stream.ReadByte();
            m_rgbUid = ReadBytes(stream, DEF_GUID_LENGTH);
            m_tag = ReadInt16(stream);
            m_size = ReadInt32(stream);
            m_cRef = ReadInt32(stream);
            m_foDelay = ReadInt32(stream);
            m_usage = stream.ReadByte();
            m_cbName = stream.ReadByte();
            m_unused2 = stream.ReadByte();
            m_unused3 = stream.ReadByte();
            if (m_cbName > 0)
            {
                throw new NotImplementedException("A BLIP with a name was found.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Write(Stream stream)
        {
            stream.WriteByte((byte)m_btWin32);
            stream.WriteByte((byte)m_btMacOS);
            stream.Write(m_rgbUid, 0, DEF_GUID_LENGTH);
            WriteInt16(stream, (short)m_tag);
            WriteInt32(stream, m_size);
            WriteInt32(stream, m_cRef);
            WriteInt32(stream, m_foDelay);
            stream.WriteByte((byte)m_usage);
            stream.WriteByte((byte)m_cbName);
            stream.WriteByte((byte)m_unused2);
            stream.WriteByte((byte)m_unused3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public _FBSE Clone()
        {
            _FBSE fbse = (_FBSE)this.MemberwiseClone();

            fbse.m_rgbUid = new byte[m_rgbUid.Length];
            m_rgbUid.CopyTo(fbse.m_rgbUid, 0);

            return fbse;
        }
        #endregion

    }
}
