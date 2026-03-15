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
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for FOPTEComplex.
    /// </summary>
    internal class FOPTEComplex : FOPTEBase
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_dataLength;
        private byte[] m_data;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal byte[] Value
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isBid"></param>
        /// <param name="valueLength"></param>
        internal FOPTEComplex(int id, bool isBid, int valueLength)
            : base(id, isBid)
        {
            m_dataLength = valueLength;
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void ReadData(Stream stream)
        {
            m_data = new byte[m_dataLength];
            stream.Read(m_data, 0, m_dataLength);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal override void Write(Stream stream)
        {
            int tmp = Id;
            tmp |= (base.IsBid ? 0x4000 : 0);
            tmp |= 0x8000;
            WriteInt16(stream, (short)tmp);
            WriteInt32(stream, m_data.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void WriteData(Stream stream)
        {
            stream.Write(m_data, 0, m_data.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        internal override FOPTEBase Clone()
        {
            FOPTEComplex fopteComplex = new FOPTEComplex(this.Id, this.IsBid, this.m_dataLength);
            fopteComplex.m_data = new byte[m_dataLength];
            m_data.CopyTo(fopteComplex.m_data, 0);

            return fopteComplex;
        }
        #endregion
    }
}
