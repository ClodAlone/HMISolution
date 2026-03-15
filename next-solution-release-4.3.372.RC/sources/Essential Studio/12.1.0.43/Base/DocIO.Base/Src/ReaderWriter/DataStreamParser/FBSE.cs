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


namespace Syncfusion.DocIO.ReaderWriter.Escher
{
    /// <summary>
    /// Summary description for _FBSE.
    /// </summary>
    internal class FBSE : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Required type on Win32
        /// </summary>
        private MSOBlipType m_btWin32;
        /// <summary>
        /// Required type on Mac
        /// </summary>
        private MSOBlipType m_btMacOS;
        /// <summary>
        /// Identifier of blip
        /// </summary>
        private byte[] m_rgbUid;
        /// <summary>
        /// currently unused
        /// </summary>
        private ushort m_tag;
        /// <summary>
        /// Blip size in stream
        /// </summary>
        private uint m_size;
        /// <summary>
        /// Reference count on the blip
        /// </summary>
        private uint m_cRef;
        /// <summary>
        /// 
        /// </summary>
        private uint m_foDelay;
        /// <summary>
        /// How this blip is used (MSOBLIPUSAGE)
        /// </summary>
        private MSOBlipUsage m_usage;
        /// <summary>
        /// length of the blip name
        /// </summary>
        private byte m_cbName;
        /// <summary>
        /// for the future
        /// </summary>
        private byte m_unused2;
        /// <summary>
        /// for the future
        /// </summary>
        private byte m_unused3;
        #endregion

        #region Class Initialize/Finalize Methods
        /// <summary>
        /// 
        /// </summary>
        public FBSE()
        {
            m_rgbUid = new byte[16];
        }
        #endregion

        #region Class propeties
        /// <summary>
        /// 
        /// </summary>
        public MSOBlipType Win32
        {
            get
            {
                return m_btWin32;
            }
            set
            {
                m_btWin32 = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public MSOBlipType MacOS
        {
            get
            {
                return m_btMacOS;
            }
            set
            {
                m_btMacOS = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Uid
        {
            get
            {
                return m_rgbUid;
            }
            set
            {
                m_rgbUid = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ushort Tag
        {
            get
            {
                return m_tag;
            }
            set
            {
                m_tag = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public uint Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public uint Ref
        {
            get
            {
                return m_cRef;
            }
            set
            {
                m_cRef = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public uint Delay
        {
            get
            {
                return m_foDelay;
            }
            set
            {
                m_foDelay = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public MSOBlipUsage Usage
        {
            get
            {
                return m_usage;
            }
            set
            {
                m_usage = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte Name
        {
            get
            {
                return m_cbName;
            }
            set
            {
                m_cbName = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte Unused2
        {
            set
            {
                m_unused2 = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte Unused3
        {
            set
            {
                m_unused3 = value;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Read(Stream stream)
        {
            m_btWin32 = (MSOBlipType)stream.ReadByte();
            m_btMacOS = (MSOBlipType)stream.ReadByte();
            m_rgbUid = ReadBytes(stream, 16);
            m_tag = ReadUInt16(stream);
            m_size = ReadUInt32(stream);
            m_cRef = ReadUInt32(stream);
            m_foDelay = ReadUInt32(stream);
            m_usage = (MSOBlipUsage)stream.ReadByte();
            m_cbName = (byte)stream.ReadByte();
            m_unused2 = (byte)stream.ReadByte();
            m_unused3 = (byte)stream.ReadByte();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Write(Stream stream)
        {
            stream.WriteByte((byte)m_btWin32);
            stream.WriteByte((byte)m_btMacOS);
            for (int i = 0; i < 16; i++)
            {
                stream.WriteByte(m_rgbUid[i]);
            }
            WriteUInt16(stream, m_tag);
            WriteUInt32(stream, m_size);
            WriteUInt32(stream, m_cRef);
            WriteUInt32(stream, m_foDelay);
            stream.WriteByte((byte)m_usage);
            stream.WriteByte((byte)m_cbName);
            stream.WriteByte(m_unused2);
            stream.WriteByte(m_unused3);
        }
        #endregion
    }
}
