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
using System.Text;
using System.IO;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    internal class EndnotesRW
    : FootnotesRW
    {
        #region Class initialize / finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal EndnotesRW()
            : base()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        internal EndnotesRW(Stream stream, WPFIBData fib)
            : base(stream, fib)
        {
        }
        #endregion

        #region Class overrides / writes
        /// <summary>
        /// 
        /// </summary>
        protected override void WriteTxtPositions()
        {
            if (m_txtPositions.Count > 0)
            {
                m_fib.fcPlcfendTxt = (int)m_writer.BaseStream.Position;
                WriteTxtPositionsBase();
                m_fib.lcbPlcfendTxt = (int)(m_writer.BaseStream.Position - m_fib.fcPlcfendTxt);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void WriteDescriptors()
        {
            if (m_descrFootEndntes.Count > 0)
            {
                m_fib.fcPlcfendRef = (int)m_writer.BaseStream.Position;
                WriteRefPositions(m_endReference);

                short entry = 0;
                //        foreach( short entry in m_descriptors )
                for (int i = 0, cnt = m_descrFootEndntes.Count; i < cnt; i++)
                {
                    entry = m_descrFootEndntes[i];
                    m_writer.Write(entry);
                }

                m_fib.lcbPlcfendRef = (int)(m_writer.BaseStream.Position - m_fib.fcPlcfendRef);
            }
        }
        #endregion

        #region Class overrides / reads
        /// <summary>
        /// 
        /// </summary>
        protected override void ReadTxtPositions()
        {
            int length = m_fib.lcbPlcfendTxt;

            if (length > 0)
            {
                m_reader.BaseStream.Position = m_fib.fcPlcfendTxt;
                int count = length / 4;
                ReadTxtPositions(count);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void ReadDescriptors()
        {
            if (m_fib.lcbPlcfendRef > 0)
            {
                m_reader.BaseStream.Position = m_fib.fcPlcfendRef;
                byte[] data = new byte[m_fib.lcbPlcfendRef];
                data = m_reader.ReadBytes(m_fib.lcbPlcfendRef);
                m_reader.BaseStream.Position = m_fib.fcPlcfendRef;
                ReadDescriptors(m_fib.lcbPlcfendRef, 2);
                base.ReadDescriptors();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="pos"></param>
        /// <param name="posNext"></param>
        protected override void ReadDescriptor(BinaryReader reader, int pos, int posNext)
        {
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                m_descrFootEndntes.Add(reader.ReadInt16());
                base.ReadDescriptor(reader, pos, posNext);
            }
        }
        #endregion
    }
}
