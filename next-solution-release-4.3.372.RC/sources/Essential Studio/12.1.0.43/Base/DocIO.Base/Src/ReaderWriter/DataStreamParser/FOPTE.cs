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
    /// Summary description for FOPTE.
    /// </summary>
    internal class FOPTE : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Property ID
        /// </summary>
        private ushort m_pid;
        /// <summary>
        /// value is a blip ID � only valid if fComplex is FALSE
        /// </summary>
        private ushort m_bid;
        /// <summary>
        /// complex property, value is lenght
        /// </summary>
        private ushort m_complex;
        /// <summary>
        /// Value
        /// </summary>
        private uint m_op;
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_name = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Member initializing constructor
        /// </summary>
        public FOPTE()
        {
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// 
        /// </summary>
        public ushort Pid
        {
            get
            {
                return m_pid;
            }
            set
            {
                m_pid = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsBid
        {
            get
            {
                return m_bid == 1;
            }
            set
            {
                if (value)
                {
                    m_bid = 1;
                }
                else
                {
                    m_bid = 0;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsComplex
        {
            get
            {
                return m_complex == 1;
            }
            set
            {
                if (value)
                {
                    m_complex = 1;
                }
                else
                {
                    m_complex = 0;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public uint Op
        {
            get
            {
                return m_op;
            }
            set
            {
                m_op = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] NameBytes
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

        #endregion

        #region Class Public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public int Read(Stream stream)
        {
            ushort val = ReadUInt16(stream);
            m_pid = (ushort)(val & 0x3FFF);
            m_bid = (ushort)((val & 0x4000) >> 14);
            m_complex = (ushort)((val & 0x8000) >> 15);

            m_op = ReadUInt32(stream);

            int size = 6;
            if (IsComplex)
            {
                size += (int)m_op;
                m_name = new byte[m_op];
            }

            return size;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Write(Stream stream)
        {
            short temp = (short)m_pid;
            temp += (short)(m_bid << 14);
            temp += (short)(m_complex << 15);
            WriteUInt16(stream, (ushort)temp);
            WriteUInt32(stream, m_op);
        }
        #endregion
    }
}
