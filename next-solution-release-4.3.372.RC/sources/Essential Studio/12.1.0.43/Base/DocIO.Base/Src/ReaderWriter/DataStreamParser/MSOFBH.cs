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
    /// Summary description for MSOFBH.
    /// </summary>
    internal class MSOFBH : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Version
        /// </summary>
        private uint m_ver;
        /// <summary>
        /// Record instance
        /// </summary>
        private uint m_inst;
        /// <summary>
        /// Record type
        /// </summary>
        private MSOFBT m_fbt;
        /// <summary>
        /// Length of record
        /// </summary>
        private uint m_cbLength;
        #endregion

        #region Class Initialize/Finalize Methods
        /// <summary>
        /// Defoult .ctor
        /// </summary>
        public MSOFBH()
        {
        }
        #endregion

        #region class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Read(Stream stream)
        {
            uint prop = ReadUInt32(stream);
            m_ver = prop & 0x000F;
            m_inst = (prop & 0xFFF0) >> 4;
            m_fbt = (MSOFBT)((prop & 0xFFFF0000) >> 16);

            m_cbLength = ReadUInt32(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Write(Stream stream)
        {
            uint temp = (uint)m_ver;
            temp += (uint)(m_inst << 4);
            temp += (uint)((int)m_fbt << 16);

            WriteUInt32(stream, temp);
            WriteUInt32(stream, m_cbLength);
        }

        #endregion

        #region Class Propertyes
        /// <summary>
        /// 
        /// </summary>
        public uint Version
        {
            get
            {
                return m_ver;
            }
            set
            {
                m_ver = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public uint Inst
        {
            get
            {
                return m_inst;
            }
            set
            {
                m_inst = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal MSOFBT Msofbt
        {
            get
            {
                return m_fbt;
            }
            set
            {
                m_fbt = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public new uint Length
        {
            get
            {
                return m_cbLength;
            }
            set
            {
                m_cbLength = value;
            }
        }
        #endregion
    }
}
